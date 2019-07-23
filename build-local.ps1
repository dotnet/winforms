[CmdletBinding(PositionalBinding=$false)]
Param(
  [switch] $FullClean,
  [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)


function Check-SdkExists {
<#
.SYNOPSIS

Checks whether a system-wise NETCore SDK and a matching local NETCore SDK installations exist

.DESCRIPTION

Checks whether a system-wise NETCore SDK and a matching local NETCore SDK installations exist

.PARAMETER LocalSdkLocation
Specifies the expected location of local NETCore SDK (e.g. D:\winforms\.dotnet\sdk\3.0.100-preview5-011568).

.PARAMETER SystemSdkLocation
Specifies the expected location of local NETCore SDK (e.g. C:\Program Files\dotnet\sdk\3.0.100-preview5-011568).

.INPUTS

None.

.OUTPUTS

None.

#>
    Param(
      [Parameter(Mandatory=$true, Position=0)]
      [string] $LocalSdkLocation,
      [Parameter(Mandatory=$true, Position=1)]
      [string] $SystemSdkLocation
    )

    # check that we have the right version of SDK installed correctly
    # if not, we will need to run in an elevated mode
    $isSdkPresent = $false;
    if (![string]::IsNullOrWhiteSpace($LocalSdkLocation) -and ![string]::IsNullOrWhiteSpace($SystemSdkLocation)) {

        $isLocalSdkPresent = [System.IO.Directory]::Exists($LocalSdkLocation);
        $isSystemSdkPresent = [System.IO.Directory]::Exists($SystemSdkLocation);

        if ($isLocalSdkPresent -eq $true -and $isSystemSdkPresent -eq $true) {
            # the required SDK exists both locally and system-wide, all good            
            $isSdkPresent = $true;
        }
    }

    return $isSdkPresent;
}

function Create-SymLink {
<#
.SYNOPSIS

Creates a symbolic link from a system-wise NETCore SDK to a local NETCore SDK installation

.DESCRIPTION

Creates a symbolic link from a system-wise NETCore SDK to a local NETCore SDK installation

.PARAMETER LocalSdkLocation
Specifies the expected location of local NETCore SDK (e.g. D:\winforms\.dotnet\sdk\3.0.100-preview5-011568).

.PARAMETER SystemSdkLocation
Specifies the expected location of local NETCore SDK (e.g. C:\Program Files\dotnet\sdk\3.0.100-preview5-011568).

.INPUTS

None.

.OUTPUTS

None.

#>
    param (
      [Parameter(Mandatory=$true, Position=0)]
      [string] $LocalSdkLocation,
      [Parameter(Mandatory=$true, Position=1)]
      [string] $SystemSdkLocation
    )

    if ([System.IO.Directory]::Exists($SystemSdkLocation)) {
        return;
    }

    Write-Host "Creating a symlink..."
    try {
        # Create a symbolic link, see MKLINK command
        $dummy = New-Item -ItemType SymbolicLink -Path $SystemSdkLocation -Value "$LocalSdkLocation" -ErrorAction Stop;
        Write-Host "√ Symlink created: $LocalSdkLocation --> $SystemSdkLocation" -ForegroundColor Green;
    }
    catch [System.IO.IOException] {
        if ($_.Exception.Message -eq 'NewItemIOError') {
            Write-Warning "! Symlink to '$SystemSdkLocation' already exists, ignore"
        }
        else {
            throw
        }
    }

}

function Get-SystemDotnetPath {
    return [string][System.IO.Path]::Combine($env:ProgramFiles, 'dotnet');
}

function Invoke-FullCleanup {
    param (
      [Parameter(Mandatory=$true, Position=0)]
      [string] $SdkVersion,
      [Parameter(Mandatory=$true, Position=1)]
      [string] $NETCoreAppVersion
    )

    Stop-Process -Name 'dotnet' -Force -ErrorAction Ignore
    Stop-Process -Name 'MSBuild' -Force -ErrorAction Ignore

    $systenDotnetLocation = Get-SystemDotnetPath
    $locations = @()
    $locations += Get-ChildItem -Path $systenDotnetLocation -Filter sdk/$SdkVersion;
    $locations += Get-ChildItem -Path $systenDotnetLocation -Filter shared/Microsoft.NETCore.App/$NETCoreAppVersion;

    $locations | `
        ForEach-Object {
            $path = $_.FullName

            if ($_.LinkType -eq 'SymbolicLink') {
                (Get-Item $path).Delete()
            }
            else {
                Remove-Item -Path $path -Force -Recurse
            }

            Write-Host "√ Path removed: $path" -ForegroundColor Green;
        }

    Write-Host "Cleaning local folder...";
    git clean -xfd
    Write-Host "√ Local folder cleaned" -ForegroundColor Green;
}

function Start-ElevatedModeIfRequired {
<#
.SYNOPSIS

Checks whether the script needs to be restarted in the elevated mode

.DESCRIPTION

The elevated mode is required in the following cases:
- FullClean is requested, or
- LocalSdkLocation or SystemSdkLocation does not exist, or
- LocalNETCoreAppLocation or SystemNETCoreAppLocation does not exist.


.PARAMETER IsFullCleanRequested
Specifies whether a full clean up is required.

.PARAMETER LocalSdkLocation
Specifies the expected location of local NETCore SDK (e.g. D:\winforms\.dotnet\sdk\3.0.100-preview5-011568).

.PARAMETER SystemSdkLocation
Specifies the expected location of local NETCore SDK (e.g. C:\Program Files\dotnet\sdk\3.0.100-preview5-011568).

.PARAMETER LocalNETCoreAppLocation
Specifies the expected location of local NETCore SDK (e.g. D:\winforms\.dotnet\shared\Microsoft.NETCore.App\3.0.0-preview7-27819-07).

.PARAMETER SystemNETCoreAppLocation
Specifies the expected location of local NETCore SDK (e.g. C:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.0.0-preview7-27819-07).

.INPUTS

None.

.OUTPUTS

None.

#>

    Param(
      [Parameter(Mandatory=$true, Position=0)]
      [bool] $IsFullCleanRequested,
      [Parameter(Mandatory=$true, Position=1)]
      [string] $LocalSdkLocation,
      [Parameter(Mandatory=$true, Position=2)]
      [string] $SystemSdkLocation,
      [Parameter(Mandatory=$true, Position=2)]
      [string] $LocalNETCoreAppLocation,
      [Parameter(Mandatory=$true, Position=3)]
      [string] $SystemNETCoreAppLocation
    )

    $isSdkPresent = Check-SdkExists -LocalSdkLocation $LocalSdkLocation -SystemSdkLocation $SystemSdkLocation;
    $isNETCoreAppPresent = Check-SdkExists -LocalSdkLocation $LocalNETCoreAppLocation -SystemSdkLocation $SystemNETCoreAppLocation;

    # if the SDK is found and not doing a full clean - we're done here
    if ($isSdkPresent -eq $true -and `
        $isNETCoreAppPresent -eq $true -and `
        $IsFullCleanRequested -ne $true) {
        return;
    }


    # To perform full cleanup we must run the script in an elevated mode
    if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { 
        $args = if ($IsFullCleanRequested -eq $true) { "-FullClean" } else { '' }
        Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" $args" -Verb RunAs; 
        exit 0
    }
}




# break on errors
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSDefaultParameterValues['*:ErrorAction'] = 'Stop'


try {
    $repoPath = $PSScriptRoot;
    Push-Location $repoPath;

    $localSdkPath = (Join-Path -Path $repoPath -ChildPath '.dotnet\sdk')
    $localSharedPath = (Join-Path -Path $repoPath -ChildPath '.dotnet\shared')

    # Detect the require version of SDK and see if we have it already
    $globalJson = Get-Content -Raw -Path global.json | ConvertFrom-Json
    $sdkVersion = [string]$globalJson.sdk.version
    $localSdkLocation = [System.IO.Path]::Combine($localSdkPath, $sdkVersion);
    $systemSdkLocation = [string][System.IO.Path]::Combine($(Get-SystemDotnetPath), 'sdk', $sdkVersion);

    # Detect the require version of NETCoreApp and see if we have it already
    [xml]$versionsProps = Get-Content -Raw -Path ./eng/Versions.props
    $value = $versionsProps.Project.PropertyGroup | Where-Object { $_['MicrosoftNETCoreAppPackageVersion'] -ne $null }
    $netCoreAppVersion = $value.InnerText
    $localNETCoreAppLocation = [System.IO.Path]::Combine($localSharedPath, 'Microsoft.NETCore.App', $netCoreAppVersion);
    $systemNETCoreAppLocation = [string][System.IO.Path]::Combine($(Get-SystemDotnetPath), 'shared\Microsoft.NETCore.App', $netCoreAppVersion);

    Start-ElevatedModeIfRequired -IsFullCleanRequested $FullClean.ToBool() `
                                 -LocalSdkLocation $localSdkLocation -SystemSdkLocation $systemSdkLocation `
                                 -LocalNETCoreAppLocation $localNETCoreAppLocation -SystemNETCoreAppLocation $systemNETCoreAppLocation

    if ($FullClean -eq $true) {
        Invoke-FullCleanup -SdkVersion $sdkVersion -NETCoreAppVersion $netCoreAppVersion
    }

    Write-Host "Building the solution...";
    .\build.cmd
    $LastCode = $LASTEXITCODE;

    Write-Host "√ Solution built" -ForegroundColor Green;

    Create-SymLink -LocalSdkLocation $localSdkLocation -SystemSdkLocation $systemSdkLocation
    Create-SymLink -LocalSdkLocation $localNETCoreAppLocation -SystemSdkLocation $systemNETCoreAppLocation

    if ($LastCode -ne 0) {
        Write-Host 'Χ Build failed....' -ForegroundColor Red
        pause
        exit -1;
    }

    Start-Process .\Winforms.sln
}
catch {
    Write-Host $_.Exception -ForegroundColor Red
    pause
}
finally {
    Pop-Location
}
