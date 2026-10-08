$ErrorActionPreference = 'Stop'
. "$PSScriptRoot\..\dotnet-architecture.ps1"

function Assert-True([bool] $condition, [string] $message) {
    if (!$condition) { throw $message }
}

function Assert-Throws([scriptblock] $action, [string] $messagePattern) {
    $errorMessage = $null
    try { & $action }
    catch { $errorMessage = $_.Exception.Message }
    Assert-True ($errorMessage -like $messagePattern) "Expected '$messagePattern', received '$errorMessage'."
}

function Write-TestPE([string] $path, [string] $architecture) {
    $machine = switch ($architecture) {
        'arm64' { [UInt16] 0xAA64 }
        'x64' { [UInt16] 0x8664 }
        'x86' { [UInt16] 0x014C }
        default { throw "Invalid fixture architecture: $architecture" }
    }
    New-Item -ItemType Directory -Path (Split-Path $path -Parent) -Force | Out-Null
    $bytes = [byte[]]::new(128)
    $bytes[0] = 0x4D
    $bytes[1] = 0x5A
    $bytes[0x3C] = 0x40
    $bytes[0x40] = 0x50
    $bytes[0x41] = 0x45
    [BitConverter]::GetBytes($machine).CopyTo($bytes, 0x44)
    [IO.File]::WriteAllBytes($path, $bytes)
}

# The real installer exits early for an existing runtime directory. Require its caller
# to distinguish complete caches from partial or wrong-architecture entries first.
function InstallDotNet($root, $version, $architecture, $runtime, $skipNonVersionedFiles, $runtimeSourceFeed, $runtimeSourceFeedKey, [switch] $noPath) {
    $installCalls.Add(@{ Root = $root; Architecture = $architecture; Runtime = $runtime; Version = $version })
    Assert-True (!(Test-Path "$root\shared\Microsoft.NETCore.App\$version")) 'Installer would skip a cached runtime directory.'
    Write-TestPE "$root\dotnet.exe" $architecture
    Write-TestPE "$root\host\fxr\$version\hostfxr.dll" $architecture
    Write-TestPE "$root\shared\Microsoft.NETCore.App\$version\coreclr.dll" $architecture
    if (!$runtime) {
        New-Item -ItemType Directory -Path "$root\sdk\$version" -Force | Out-Null
    }
}

$testRoot = Join-Path ([IO.Path]::GetTempPath()) "WinForms-architecture-tests-$([Guid]::NewGuid().ToString('N'))"
$savedEnvironment = @{}
foreach ($name in @('PROCESSOR_ARCHITECTURE', 'PROCESSOR_ARCHITEW6432', 'DOTNET_GLOBAL_INSTALL_DIR', 'PATH')) {
    $savedEnvironment[$name] = [Environment]::GetEnvironmentVariable($name)
}

try {
    foreach ($os in @('AMD64', 'ARM64')) {
        $env:PROCESSOR_ARCHITECTURE = $os
        $env:PROCESSOR_ARCHITEW6432 = $null
        $expected = if ($os -eq 'ARM64') { 'arm64' } else { 'x64' }
        Assert-True ((Get-DotNetInstallationArchitecture "$testRoot\fresh-$os") -eq $expected) 'Fresh SDK did not select the OS architecture.'

        foreach ($sdk in @('x64', 'arm64')) {
            $root = "$testRoot\$os-$sdk"
            Write-TestPE "$root\dotnet.exe" $sdk
            Assert-True ((Get-DotNetInstallationArchitecture $root) -eq $sdk) 'Existing SDK architecture was replaced by the OS architecture.'
            $expectedSdkRoot = if ($os -eq 'AMD64' -and $sdk -eq 'arm64') { "$root\x64" } else { $root }
            Assert-True ((Get-DotNetSdkDirectory $root) -eq $expectedSdkRoot) 'SDK selection did not preserve a compatible host or isolate an incompatible host.'
            Assert-True (Test-Path "$root\dotnet.exe") 'SDK selection removed the old muxer.'
            $launcherRoots = @(& "$PSScriptRoot\..\dotnet-architecture.ps1" -ResolveSdkRoot $root -ResolveRuntimeRoots)
            $launcherSdk = Get-DotNetInstallationArchitecture $expectedSdkRoot
            foreach ($target in @('x64', 'arm64', 'x86')) {
                $expectedRoot = if ($sdk -eq $target) { $root } else { "$root\$target" }
                Assert-True ((Get-DotNetRuntimeDirectory $root $sdk $target) -eq $expectedRoot) 'Runtime routing mixed SDK and target architectures.'
                $expectedLauncherRoot = Get-DotNetRuntimeDirectory $expectedSdkRoot $launcherSdk $target
                Assert-True ($launcherRoots -contains "DOTNET_ROOT_$($target.ToUpperInvariant())=$expectedLauncherRoot") 'IDE runtime roots depended on whether restore had already installed the target runtime.'
            }
        }
    }

    $env:PROCESSOR_ARCHITECTURE = 'AMD64'
    $env:PROCESSOR_ARCHITEW6432 = 'ARM64'
    Assert-True ((Get-WindowsBuildArchitecture) -eq 'arm64') 'An emulated shell hid the ARM64 OS architecture.'

    foreach ($sdk in @('x64', 'arm64')) {
        $other = if ($sdk -eq 'x64') { 'arm64' } else { 'x64' }
        $root = "$testRoot\repair-$sdk"
        Write-TestPE "$root\dotnet.exe" $sdk
        Write-TestPE "$root\host\fxr\1.0.0\hostfxr.dll" $sdk
        Write-TestPE "$root\shared\Microsoft.NETCore.App\1.0.0\coreclr.dll" $sdk
        Write-TestPE "$root\host\fxr\2.0.0\hostfxr.dll" $other
        Write-TestPE "$root\shared\Microsoft.NETCore.App\2.0.0\coreclr.dll" $other
        Repair-DotNetInstallation $root
        Assert-True ((Test-Path "$root\host\fxr\1.0.0\hostfxr.dll") -and (Test-Path "$root\shared\Microsoft.NETCore.App\1.0.0\coreclr.dll")) 'Compatible files were moved.'
        Assert-True (!(Test-Path "$root\host\fxr\2.0.0") -and !(Test-Path "$root\shared\Microsoft.NETCore.App\2.0.0")) 'Foreign files remain in the live installation.'
        Assert-True (@(Get-ChildItem "$root\legacy-runtimes" -Recurse -File).Count -eq 2) 'Foreign files were not preserved.'
        Repair-DotNetInstallation $root
        Assert-True (@(Get-ChildItem "$root\legacy-runtimes" -Recurse -File).Count -eq 2) 'Repair is not idempotent.'

        $unsafeRoot = "$testRoot\no-compatible-resolver-$sdk"
        Write-TestPE "$unsafeRoot\dotnet.exe" $sdk
        Write-TestPE "$unsafeRoot\host\fxr\1.0.0\hostfxr.dll" $other
        Write-TestPE "$unsafeRoot\shared\Microsoft.NETCore.App\1.0.0\coreclr.dll" $other
        Assert-Throws { Repair-DotNetInstallation $unsafeRoot } 'No hostfxr compatible*'
        Assert-True ((Test-Path "$unsafeRoot\host\fxr\1.0.0\hostfxr.dll") -and (Test-Path "$unsafeRoot\shared\Microsoft.NETCore.App\1.0.0\coreclr.dll")) 'Failed preflight moved files.'

        foreach ($target in @('x64', 'arm64', 'x86')) {
            $runtimeRoot = "$testRoot\runtime-$sdk-$target"
            Write-TestPE "$runtimeRoot\dotnet.exe" $sdk
            Write-TestPE "$runtimeRoot\host\fxr\1.0.0\hostfxr.dll" $sdk
            $installCalls = [System.Collections.Generic.List[object]]::new()
            Install-WinFormsDotNetRuntime $runtimeRoot $sdk $target '2.0.0' '' ''
            $expectedRoot = Get-DotNetRuntimeDirectory $runtimeRoot $sdk $target
            Assert-True ($installCalls.Count -eq 1 -and $installCalls[0].Root -eq $expectedRoot -and $installCalls[0].Architecture -eq $target) 'Runtime installer received the wrong directory or architecture.'
            Assert-True ((Get-DotNetExecutableArchitecture "$runtimeRoot\dotnet.exe") -eq $sdk) 'Cross-target runtime replaced the SDK muxer.'
            Install-WinFormsDotNetRuntime $runtimeRoot $sdk $target '2.0.0' '' ''
            Assert-True ($installCalls.Count -eq 1) 'Complete runtime cache was not reused.'

            Write-TestPE "$expectedRoot\shared\Microsoft.NETCore.App\3.0.0\coreclr.dll" $other
            Install-WinFormsDotNetRuntime $runtimeRoot $sdk $target '3.0.0' '' ''
            Assert-True ((Get-DotNetExecutableArchitecture "$expectedRoot\shared\Microsoft.NETCore.App\3.0.0\coreclr.dll") -eq $target) 'Partial or foreign runtime cache was not repaired.'
        }
    }

    # Exercise the repository's Arcade extension, including native fresh bootstrap and
    # preserving an x64 SDK while the OS (and requested target) is ARM64.
    $RepoRoot = $testRoot
    $GlobalJson = @{ tools = @{ dotnet = '4.0.0' } }
    $runtimeSourceFeed = ''
    $runtimeSourceFeedKey = ''
    foreach ($os in @('AMD64', 'ARM64')) {
        $env:PROCESSOR_ARCHITECTURE = $os
        $env:PROCESSOR_ARCHITEW6432 = $null
        $env:DOTNET_GLOBAL_INSTALL_DIR = "$testRoot\bootstrap-$os"
        $installCalls = [System.Collections.Generic.List[object]]::new()
        . "$PSScriptRoot\..\configure-toolset.ps1"
        InstallDotNetSdk $env:DOTNET_GLOBAL_INSTALL_DIR '4.0.0'
        $expected = if ($os -eq 'ARM64') { 'arm64' } else { 'x64' }
        Assert-True ($installCalls[0].Architecture -eq $expected) 'Fresh SDK bootstrap used the shell instead of the OS architecture.'
    }

    $env:PROCESSOR_ARCHITECTURE = 'ARM64'
    $env:DOTNET_GLOBAL_INSTALL_DIR = "$testRoot\emulated-sdk"
    Write-TestPE "$env:DOTNET_GLOBAL_INSTALL_DIR\dotnet.exe" 'x64'
    Write-TestPE "$env:DOTNET_GLOBAL_INSTALL_DIR\host\fxr\1.0.0\hostfxr.dll" 'x64'
    $installCalls = [System.Collections.Generic.List[object]]::new()
    . "$PSScriptRoot\..\configure-toolset.ps1"
    InstallDotNetSdk $env:DOTNET_GLOBAL_INSTALL_DIR '4.0.0'
    Assert-True ($installCalls[0].Architecture -eq 'x64') 'SDK upgrade replaced an emulated x64 installation.'

    $env:PROCESSOR_ARCHITECTURE = 'AMD64'
    $originalRoot = "$testRoot\arm64-sdk-on-x64"
    $env:DOTNET_GLOBAL_INSTALL_DIR = $originalRoot
    Write-TestPE "$originalRoot\dotnet.exe" 'arm64'
    Write-TestPE "$originalRoot\host\fxr\1.0.0\hostfxr.dll" 'arm64'
    $installCalls = [System.Collections.Generic.List[object]]::new()
    . "$PSScriptRoot\..\configure-toolset.ps1"
    Assert-True ($env:DOTNET_GLOBAL_INSTALL_DIR -eq "$originalRoot\x64") 'An unexecutable SDK was not isolated before bootstrap.'
    InstallDotNetSdk $env:DOTNET_GLOBAL_INSTALL_DIR '4.0.0'
    Assert-True ($installCalls[0].Architecture -eq 'x64' -and (Get-DotNetExecutableArchitecture "$originalRoot\dotnet.exe") -eq 'arm64') 'Native bootstrap replaced the preserved ARM64 SDK.'

    $installerRepo = "$testRoot\installer"
    New-Item -ItemType Directory -Path "$installerRepo\eng\common" -Force | Out-Null
    Copy-Item "$PSScriptRoot\..\install-runtimes.ps1" "$installerRepo\eng\install-runtimes.ps1"
    Copy-Item "$PSScriptRoot\..\dotnet-architecture.ps1" "$installerRepo\eng\dotnet-architecture.ps1"
    '$GlobalJson = Get-Content "$PSScriptRoot\..\..\global.json" -Raw | ConvertFrom-Json' |
        Set-Content "$installerRepo\eng\common\tools.ps1"
    '{"tools":{"runtimes":{"dotnet":["$(MicrosoftNETCoreAppRefPackageVersion)"],"dotnet/x86":["$(MicrosoftNETCoreAppRefPackageVersion)"]}}}' |
        Set-Content "$installerRepo\global.json"
    foreach ($sdk in @('x64', 'arm64')) {
        $sdkRoot = "$installerRepo\sdk-$sdk"
        Write-TestPE "$sdkRoot\dotnet.exe" $sdk
        Write-TestPE "$sdkRoot\host\fxr\1.0.0\hostfxr.dll" $sdk
        $installCalls = [System.Collections.Generic.List[object]]::new()
        $target = if ($sdk -eq 'x64') { 'arm64' } else { 'x64' }
        & "$installerRepo\eng\install-runtimes.ps1" -SdkRoot $sdkRoot -RuntimeVersion '5.0.0' -TargetArchitecture $target
        Assert-True ($installCalls.Count -eq 3) 'Runtime manifest did not install SDK, cross-target, and x86 runtimes.'
        foreach ($architecture in @($sdk, $target, 'x86')) {
            $expectedRoot = Get-DotNetRuntimeDirectory $sdkRoot $sdk $architecture
            Assert-True (@($installCalls | Where-Object { $_.Root -eq $expectedRoot -and $_.Architecture -eq $architecture -and $_.Version -eq '5.0.0' }).Count -eq 1) 'Manifest installation used the wrong root, version, or architecture.'
        }
        & "$installerRepo\eng\install-runtimes.ps1" -SdkRoot $sdkRoot -RuntimeVersion '5.0.0' -TargetArchitecture $sdk
        Assert-True ($installCalls.Count -eq 3) 'Duplicate manifest requests bypassed the complete runtime cache.'
    }

    Assert-Throws { Get-DotNetRuntimeDirectory $testRoot 'x64' 'invalid' } 'Unsupported .NET runtime architecture*'
    [IO.File]::WriteAllBytes("$testRoot\invalid.exe", [byte[]]::new(128))
    Assert-Throws { Get-DotNetExecutableArchitecture "$testRoot\invalid.exe" } 'Invalid PE file*'
    Write-Host 'Bidirectional SDK and runtime architecture checks passed.'
}
finally {
    foreach ($name in $savedEnvironment.Keys) {
        [Environment]::SetEnvironmentVariable($name, $savedEnvironment[$name])
    }
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
}
