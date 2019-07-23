$DoNotAbortNativeToolsInstallationOnFailure = $true
$DoNotDisplayNativeToolsInstallationWarnings = $true

. $PsScriptRoot\common\init-tools-native.ps1 -InstallDirectory $PSScriptRoot\..\.tools\native -GlobalJsonFile $PSScriptRoot\..\global.json