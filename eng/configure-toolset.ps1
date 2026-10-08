$script:DoNotAbortNativeToolsInstallationOnFailure = $true
$script:DoNotDisplayNativeToolsInstallationWarnings = $true

# Add CMake to path.
$env:PATH = "$PSScriptRoot\..\.tools\bin;$env:PATH"

if ([Environment]::OSVersion.Platform -eq [PlatformID]::Win32NT) {
  . "$PSScriptRoot\dotnet-architecture.ps1"
  $_WinFormsDotNetRoot = if ($env:DOTNET_GLOBAL_INSTALL_DIR) { $env:DOTNET_GLOBAL_INSTALL_DIR } else { Join-Path $RepoRoot '.dotnet' }
  $_WinFormsSdkRoot = Get-DotNetSdkDirectory $_WinFormsDotNetRoot
  if ($_WinFormsSdkRoot -ne $_WinFormsDotNetRoot) {
    Write-Host "Using compatible SDK installation '$_WinFormsSdkRoot'; preserving '$_WinFormsDotNetRoot'."
    $env:DOTNET_GLOBAL_INSTALL_DIR = $_WinFormsSdkRoot
    $_WinFormsDotNetRoot = $_WinFormsSdkRoot
  }

  if (Test-Path -LiteralPath (Join-Path $_WinFormsDotNetRoot "sdk\$($GlobalJson.tools.dotnet)")) {
    Repair-DotNetInstallation $_WinFormsDotNetRoot
  }

  # Keep an existing SDK's execution architecture, including x64 under emulation.
  # For a fresh installation, select the OS architecture explicitly.
  function InstallDotNetSdk([string] $dotnetRoot, [string] $version, [string] $architecture = '', [switch] $noPath) {
    if (!$architecture) {
      $architecture = Get-DotNetInstallationArchitecture $dotnetRoot
    }

    InstallDotNet $dotnetRoot $version $architecture '' $false $runtimeSourceFeed $runtimeSourceFeedKey -noPath:$noPath
    Repair-DotNetInstallation $dotnetRoot
  }
}
