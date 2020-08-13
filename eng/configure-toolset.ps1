$script:DoNotAbortNativeToolsInstallationOnFailure = $true
$script:DoNotDisplayNativeToolsInstallationWarnings = $true

# Add CMake to path.
$env:PATH = "$PSScriptRoot\..\.tools\bin;$env:PATH"
