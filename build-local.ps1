[CmdletBinding(PositionalBinding=$false)]
Param(
  [switch] $FullClean,
  [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

function Get-SystemSdkPath {
    return [string][System.IO.Path]::Combine($env:ProgramFiles, 'dotnet\sdk');
}

function Start-ElevatedModeIfRequired {
    Param(
      [Parameter(Mandatory=$true, Position=0)]
      [bool] $IsFullCleanRequested,
      [Parameter(Mandatory=$true, Position=1)]
      [string] $LocalSdkLocation,
      [Parameter(Mandatory=$true, Position=2)]
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

    # if the SDK is found and not doing a full clean - we're done here
    if ($isSdkPresent -eq $true -and $IsFullCleanRequested -ne $true) {
        return;
    }


    # To perform full cleanup we must run the script in an elevated mode
    if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) { 
        $args = if ($IsFullCleanRequested -eq $true) { "-FullClean" } else { '' }
        Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" $args" -Verb RunAs; 
        exit 0
    }
}

function Create-SymLink {
    param (
      [Parameter(Mandatory=$true, Position=0)]
      [string] $LocalSdkLocation,
      [Parameter(Mandatory=$true, Position=1)]
      [string] $SystemSdkLocation
    )

    if ([System.IO.Directory]::Exists($SystemSdkLocation)) {
        return;
    }

    try {
        # Create a symbolic link, see MKLINK command
        $dummy = New-Item -ItemType SymbolicLink -Path $SystemSdkLocation -Value "$LocalSdkLocation" -ErrorAction Stop
    }
    catch [System.IO.IOException] {
        if ($_.Exception.Message -eq 'NewItemIOError') {
            Write-Warning "Symlink to '$SystemSdkLocation' already exists, ignore"
        }
        else {
            throw
        }
    }

}

function Invoke-FullCleanup {
    param (
      [Parameter(Mandatory=$true, Position=0)]
      [string] $SdkVersion
    )

    Stop-Process -Name 'dotnet' -Force -ErrorAction Ignore
    Stop-Process -Name 'MSBuild' -Force -ErrorAction Ignore

    Get-ChildItem -Path $(Get-SystemSdkPath) -Filter $SdkVersion | `
        ForEach-Object {
            $path = $_.FullName
            Write-Host "Removing $path";

            if ($_.LinkType -eq 'SymbolicLink') {
                (Get-Item $path).Delete()
            }
            else {
                Remove-Item -Path $path -Force -Recurse
            }
        }

    Write-Host "Cleaning local folder"
    git clean -xfd
}


# break on errors
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSDefaultParameterValues['*:ErrorAction']='Stop'


try {
    $repoPath = $PSScriptRoot;
    Push-Location $repoPath;

    $localSdkPath = (Join-Path -Path $repoPath -ChildPath '.dotnet\sdk')

    # Detect the require version of SDK and see if we have it already
    $globalJson = Get-Content -Raw -Path global.json | ConvertFrom-Json
    $sdkVersion = [string]$globalJson.sdk.version
    $localSdkLocation = [System.IO.Path]::Combine($localSdkPath, $sdkVersion);
    $systemSdkLocation = [string][System.IO.Path]::Combine($(Get-SystemSdkPath), $sdkVersion);

    Start-ElevatedModeIfRequired -IsFullCleanRequested $FullClean.ToBool() -LocalSdkLocation $localSdkLocation -SystemSdkLocation $systemSdkLocation

    if ($FullClean -eq $true) {
        Invoke-FullCleanup -SdkVersion $sdkVersion
    }

    .\build.cmd

    Create-SymLink -LocalSdkLocation $localSdkLocation -SystemSdkLocation $systemSdkLocation

    dotnet restore

    Start-Process .\Winforms.sln
}
catch {
    Write-Host $_.Exception -ForegroundColor Red
    pause
}
finally {
    Pop-Location
}
