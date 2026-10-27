[CmdletBinding(PositionalBinding = $false)]
param(
    [Parameter(Mandatory = $true)]
    [string] $SdkRoot,
    [Parameter(Mandatory = $true)]
    [string] $RuntimeVersion,
    [string] $TargetArchitecture,
    [string] $RuntimeSourceFeed,
    [string] $RuntimeSourceFeedKey
)

$ErrorActionPreference = 'Stop'

try {
    . "$PSScriptRoot\common\tools.ps1"
    . "$PSScriptRoot\dotnet-architecture.ps1"

    $sdkArchitecture = Get-DotNetExecutableArchitecture (Join-Path $SdkRoot 'dotnet.exe')
    $requests = @()
    foreach ($runtime in $GlobalJson.tools.runtimes.PSObject.Properties) {
        $parts = $runtime.Name.Split('/')
        if ($parts[0] -ne 'dotnet' -or $parts.Length -gt 2) {
            throw "Unsupported WinForms runtime specification: $($runtime.Name)"
        }

        $architecture = if ($parts.Length -eq 2) { $parts[1].ToLowerInvariant() } else { $sdkArchitecture }
        foreach ($value in $runtime.Value) {
            $version = if ($value -eq '$(MicrosoftNETCoreAppRefPackageVersion)') { $RuntimeVersion } else { $value }
            if ($version -notmatch '^\d+\.\d+\.\d+([+-][0-9A-Za-z.-]+)?$') {
                throw "Unresolved or invalid runtime version: $value"
            }

            $requests += @{ Architecture = $architecture; Version = $version }
            if ($parts.Length -eq 1 -and $TargetArchitecture) {
                $requests += @{ Architecture = $TargetArchitecture.ToLowerInvariant(); Version = $version }
            }
        }
    }

    $installed = @{}
    foreach ($request in $requests) {
        $key = "$($request.Architecture)/$($request.Version)"
        if (!$installed.ContainsKey($key)) {
            Install-WinFormsDotNetRuntime $SdkRoot $sdkArchitecture $request.Architecture $request.Version $RuntimeSourceFeed $RuntimeSourceFeedKey
            $installed[$key] = $true
        }
    }
}
catch {
    Write-Error $_
    exit 1
}
