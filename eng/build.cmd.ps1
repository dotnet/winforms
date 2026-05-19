[CmdletBinding(PositionalBinding = $false)]
param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $BuildArgs
)

# Machine type constants from the PE COFF header (IMAGE_FILE_HEADER.Machine).
$ImageFileMachineArm64 = 0xAA64

<#
.SYNOPSIS
Reads the PE Machine field of a native DLL/EXE, identifying the architecture it was built for.
#>
function Get-PEMachineType([string] $filePath) {
    $stream = [System.IO.File]::OpenRead($filePath)
    try {
        $reader = [System.IO.BinaryReader]::new($stream)
        $stream.Seek(0x3C, [System.IO.SeekOrigin]::Begin) | Out-Null
        $peHeaderOffset = $reader.ReadInt32()
        $stream.Seek($peHeaderOffset + 4, [System.IO.SeekOrigin]::Begin) | Out-Null
        return $reader.ReadUInt16()
    }
    finally {
        $stream.Dispose()
    }
}

<#
.SYNOPSIS
Works around an Arcade limitation (only x86 secondary runtimes are redirected to an
architecture-specific subfolder; x64 is always installed into the shared .dotnet root -
see eng/common/dotnet-install.ps1). On an ARM64 host, installing the "dotnet/x64" runtime
listed in global.json's tools.runtimes therefore drops an x64 hostfxr.dll next to the
native ARM64 one under .dotnet/host/fxr. The dotnet muxer picks the highest-versioned
hostfxr it finds without regard to architecture, so once the x64 copy is present with a
higher version, every subsequent "dotnet" invocation fails to load hostfxr on this ARM64
machine. Removing the mismatched-architecture hostfxr folder(s) restores resolution to the
native one; the corresponding shared runtime (still on disk) is left alone so dotnet-install
continues to treat that runtime as already installed and won't recreate the problem.
#>
function Remove-MismatchedHostFxr([string] $dotnetRoot) {
    $fxrDir = Join-Path $dotnetRoot 'host\fxr'
    if (!(Test-Path $fxrDir)) {
        return
    }

    $versionDirs = Get-ChildItem -Path $fxrDir -Directory -ErrorAction SilentlyContinue
    $nativeDirs = @()
    $mismatchedDirs = @()

    foreach ($versionDir in $versionDirs) {
        $hostfxrPath = Join-Path $versionDir.FullName 'hostfxr.dll'
        if (!(Test-Path $hostfxrPath)) {
            continue
        }

        if ((Get-PEMachineType $hostfxrPath) -eq $ImageFileMachineArm64) {
            $nativeDirs += $versionDir
        }
        else {
            $mismatchedDirs += $versionDir
        }
    }

    # Only clean up if a native ARM64 hostfxr remains available; never remove the last copy.
    if ($nativeDirs.Count -gt 0) {
        foreach ($mismatchedDir in $mismatchedDirs) {
            Write-Host "Removing mismatched-architecture hostfxr: $($mismatchedDir.FullName)"
            Remove-Item -Path $mismatchedDir.FullName -Recurse -Force
        }
    }
}

$forwardArgs = [System.Collections.Generic.List[string]]::new()
$useNativeTools = $true
$isArm64Platform = $false
$hasTargetArchitecture = $false
$hasRestore = $false
$hasBuild = $false
$hasBinaryLog = $false

for ($i = 0; $i -lt $BuildArgs.Length; $i++) {
    $arg = $BuildArgs[$i]

    if ($arg.StartsWith('/p:TargetArchitecture=', [StringComparison]::OrdinalIgnoreCase) -or
        $arg.StartsWith('-p:TargetArchitecture=', [StringComparison]::OrdinalIgnoreCase)) {
        $hasTargetArchitecture = $true
    }

    if ($arg.Equals('-restore', [StringComparison]::OrdinalIgnoreCase)) {
        $hasRestore = $true
    }

    if ($arg.Equals('-build', [StringComparison]::OrdinalIgnoreCase)) {
        $hasBuild = $true
    }

    if ($arg.Equals('-bl', [StringComparison]::OrdinalIgnoreCase)) {
        $hasBinaryLog = $true
    }

    if ($arg.Equals('-platform', [StringComparison]::OrdinalIgnoreCase) -and
        $i + 1 -lt $BuildArgs.Length -and
        $BuildArgs[$i + 1].Equals('arm64', [StringComparison]::OrdinalIgnoreCase)) {
        $isArm64Platform = $true
        $i++
        continue
    }

    if ($arg.Equals('/p:Platform=arm64', [StringComparison]::OrdinalIgnoreCase) -or
        $arg.Equals('-p:Platform=arm64', [StringComparison]::OrdinalIgnoreCase)) {
        $isArm64Platform = $true
        continue
    }

    $forwardArgs.Add($arg)
}

if ($isArm64Platform) {
    $useNativeTools = $false

    if (!$hasTargetArchitecture) {
        $forwardArgs.Add('/p:TargetArchitecture=arm64')
    }
}

$buildScript = Join-Path $PSScriptRoot 'common\build.ps1'
$baseArgs = @()

if ($useNativeTools) {
    $baseArgs += '-NativeToolsOnMachine'
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$dotnetRoot = if ($env:DOTNET_GLOBAL_INSTALL_DIR) { $env:DOTNET_GLOBAL_INSTALL_DIR } else { Join-Path $repoRoot '.dotnet' }

$defaultArgs = @()
if (!$hasRestore) { $defaultArgs += '-restore' }
if (!$hasBuild) { $defaultArgs += '-build' }
if (!$hasBinaryLog) { $defaultArgs += '-bl' }

$processArgs = @(
    '-ExecutionPolicy'
    'ByPass'
    '-NoProfile'
    '-File'
    $buildScript
) + $baseArgs + $defaultArgs + $forwardArgs

if ($isArm64Platform) {
    # Clean up any leftover clobbered hostfxr from a previous run before we start.
    Remove-MismatchedHostFxr $dotnetRoot
}

& powershell @processArgs
$exitCode = $LASTEXITCODE

if ($isArm64Platform -and $exitCode -ne 0) {
    # A restore step may have just installed a non-native "dotnet/x64" runtime from
    # global.json, clobbering hostfxr resolution (see Remove-MismatchedHostFxr above).
    # Clean up and retry once so a single invocation self-heals.
    $fxrDir = Join-Path $dotnetRoot 'host\fxr'
    $hadMismatch = (Test-Path $fxrDir) -and
        (Get-ChildItem -Path $fxrDir -Directory -ErrorAction SilentlyContinue | Where-Object {
            $hostfxrPath = Join-Path $_.FullName 'hostfxr.dll'
            (Test-Path $hostfxrPath) -and ((Get-PEMachineType $hostfxrPath) -ne $ImageFileMachineArm64)
        })

    if ($hadMismatch) {
        Remove-MismatchedHostFxr $dotnetRoot
        Write-Host 'Retrying build after removing mismatched-architecture hostfxr...'
        & powershell @processArgs
        $exitCode = $LASTEXITCODE
    }
}

exit $exitCode
