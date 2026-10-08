param([string] $ResolveSdkRoot, [switch] $ResolveRuntimeRoots)

function Get-DotNetExecutableArchitecture([string] $path) {
    $stream = [System.IO.File]::OpenRead($path)
    try {
        $reader = [System.IO.BinaryReader]::new($stream)
        if ($reader.ReadUInt16() -ne 0x5A4D) {
            throw "Invalid PE file: $path"
        }

        $stream.Position = 0x3C
        $stream.Position = $reader.ReadInt32()
        if ($reader.ReadUInt32() -ne 0x00004550) {
            throw "Invalid PE header: $path"
        }

        switch ($reader.ReadUInt16()) {
            0xAA64 { return 'arm64' }
            0x8664 { return 'x64' }
            0x014C { return 'x86' }
            default { throw "Unsupported .NET executable architecture: $path" }
        }
    }
    finally {
        $stream.Dispose()
    }
}

function Get-WindowsBuildArchitecture {
    $architecture = $env:PROCESSOR_ARCHITECTURE
    if ($env:PROCESSOR_ARCHITEW6432) {
        $architecture = $env:PROCESSOR_ARCHITEW6432
    }

    switch ($architecture) {
        'ARM64' { return 'arm64' }
        'AMD64' { return 'x64' }
        'x86' { return 'x86' }
        default { throw "Unsupported Windows build architecture: $architecture" }
    }
}

function Get-DotNetInstallationArchitecture([string] $dotnetRoot) {
    $muxer = Join-Path $dotnetRoot 'dotnet.exe'
    if (Test-Path -LiteralPath $muxer) {
        return Get-DotNetExecutableArchitecture $muxer
    }

    return Get-WindowsBuildArchitecture
}

function Get-DotNetSdkDirectory([string] $dotnetRoot) {
    $osArchitecture = Get-WindowsBuildArchitecture
    $sdkArchitecture = Get-DotNetInstallationArchitecture $dotnetRoot
    if ($osArchitecture -eq 'arm64' -or $sdkArchitecture -eq $osArchitecture -or $sdkArchitecture -eq 'x86') {
        return $dotnetRoot
    }

    # An ARM64 SDK cannot execute on x64. Keep it intact and bootstrap the native
    # SDK separately, also covering a checkout reused on a different machine.
    $nativeRoot = Join-Path $dotnetRoot $osArchitecture
    if ((Get-DotNetInstallationArchitecture $nativeRoot) -ne $osArchitecture) {
        throw "The SDK in '$nativeRoot' cannot execute on $osArchitecture. Choose a different DOTNET_GLOBAL_INSTALL_DIR."
    }

    return $nativeRoot
}

function Get-DotNetRuntimeDirectory([string] $dotnetRoot, [string] $sdkArchitecture, [string] $runtimeArchitecture) {
    if ($runtimeArchitecture -notin @('x64', 'arm64', 'x86')) {
        throw "Unsupported .NET runtime architecture: $runtimeArchitecture"
    }

    if ($runtimeArchitecture -eq $sdkArchitecture) {
        return $dotnetRoot
    }

    return Join-Path $dotnetRoot $runtimeArchitecture
}

function Move-DotNetRuntimeVersion([string] $dotnetRoot, [string] $component, [System.IO.DirectoryInfo] $versionDir) {
    $backupDir = Join-Path $dotnetRoot "legacy-runtimes\$component"
    New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
    $destination = Join-Path $backupDir "$($versionDir.Name)-$([Guid]::NewGuid().ToString('N'))"
    Write-Host "Preserving incompatible or incomplete .NET files from '$($versionDir.FullName)' in '$destination'."
    Move-Item -LiteralPath $versionDir.FullName -Destination $destination
}

function Repair-DotNetInstallation([string] $dotnetRoot) {
    $muxer = Join-Path $dotnetRoot 'dotnet.exe'
    if (!(Test-Path -LiteralPath $muxer)) {
        return
    }

    $architecture = Get-DotNetExecutableArchitecture $muxer
    $foreignVersions = @()
    $compatibleResolvers = 0
    $foreignResolvers = 0

    foreach ($component in @('host\fxr', 'shared\Microsoft.NETCore.App')) {
        $componentDir = Join-Path $dotnetRoot $component
        if (!(Test-Path -LiteralPath $componentDir)) {
            continue
        }

        $binaryName = if ($component -eq 'host\fxr') { 'hostfxr.dll' } else { 'coreclr.dll' }
        foreach ($versionDir in Get-ChildItem -LiteralPath $componentDir -Directory) {
            $binaryPath = Join-Path $versionDir.FullName $binaryName
            if (!(Test-Path -LiteralPath $binaryPath)) {
                continue
            }

            if ((Get-DotNetExecutableArchitecture $binaryPath) -eq $architecture) {
                if ($component -eq 'host\fxr') { $compatibleResolvers++ }
            }
            else {
                if ($component -eq 'host\fxr') { $foreignResolvers++ }
                $foreignVersions += @{ Component = $component; Directory = $versionDir }
            }
        }
    }

    # Preflight the whole installation before moving anything, including shared runtimes.
    if ($foreignResolvers -gt 0 -and $compatibleResolvers -eq 0) {
        throw "No hostfxr compatible with the $architecture SDK in '$dotnetRoot'. No files were moved. Restore into a new DOTNET_GLOBAL_INSTALL_DIR or reinstall this SDK."
    }

    foreach ($version in $foreignVersions) {
        Move-DotNetRuntimeVersion $dotnetRoot $version.Component $version.Directory
    }
}

function Install-WinFormsDotNetRuntime(
    [string] $dotnetRoot,
    [string] $sdkArchitecture,
    [string] $runtimeArchitecture,
    [string] $version,
    [string] $runtimeSourceFeed,
    [string] $runtimeSourceFeedKey) {

    $installDir = Get-DotNetRuntimeDirectory $dotnetRoot $sdkArchitecture $runtimeArchitecture
    $muxer = Join-Path $installDir 'dotnet.exe'
    if ((Test-Path -LiteralPath $muxer) -and (Get-DotNetExecutableArchitecture $muxer) -ne $runtimeArchitecture) {
        throw "The .NET host in '$installDir' is not $runtimeArchitecture. No files were moved."
    }

    $runtimeDir = Join-Path $installDir "shared\Microsoft.NETCore.App\$version"
    $coreclr = Join-Path $runtimeDir 'coreclr.dll'
    $resolver = Join-Path $installDir "host\fxr\$version\hostfxr.dll"
    $complete = (Test-Path -LiteralPath $muxer) -and
        (Test-Path -LiteralPath $coreclr) -and
        (Test-Path -LiteralPath $resolver)
    if ($complete) {
        $complete = (Get-DotNetExecutableArchitecture $coreclr) -eq $runtimeArchitecture -and
            (Get-DotNetExecutableArchitecture $resolver) -eq $runtimeArchitecture
    }

    if (!$complete) {
        # Arcade's cache check only tests the directory's existence. Move an incomplete
        # entry aside so the shared installer downloads it rather than skipping it.
        if (Test-Path -LiteralPath $runtimeDir) {
            Move-DotNetRuntimeVersion $installDir 'shared\Microsoft.NETCore.App' (Get-Item -LiteralPath $runtimeDir)
        }

        InstallDotNet $installDir $version $runtimeArchitecture 'dotnet' $true $runtimeSourceFeed $runtimeSourceFeedKey -noPath
        foreach ($binary in @($muxer, $coreclr, $resolver)) {
            if (!(Test-Path -LiteralPath $binary) -or (Get-DotNetExecutableArchitecture $binary) -ne $runtimeArchitecture) {
                throw "The $runtimeArchitecture runtime $version installation is incomplete or incompatible: $binary"
            }
        }
    }
    else {
        Write-Host "Runtime dotnet/$runtimeArchitecture $version already installed in '$installDir'."
    }

    Repair-DotNetInstallation $installDir
}

if ($ResolveSdkRoot) {
    $sdkRoot = Get-DotNetSdkDirectory $ResolveSdkRoot
    if ($ResolveRuntimeRoots) {
        $sdkArchitecture = Get-DotNetInstallationArchitecture $sdkRoot
        foreach ($architecture in @('x86', 'x64', 'arm64')) {
            $runtimeRoot = Get-DotNetRuntimeDirectory $sdkRoot $sdkArchitecture $architecture
            "DOTNET_ROOT_$($architecture.ToUpperInvariant())=$runtimeRoot"
        }
    }
    else {
        $sdkRoot
    }
}
