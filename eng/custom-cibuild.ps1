[CmdletBinding(PositionalBinding=$false)]
Param(
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

. $PSScriptRoot\common\tools.ps1

# CI mode - set to true on CI server for PR validation build or official build.
[bool]$ci = $properties.Contains('-ci')

# Tests - set to true if running tests.
[bool]$test = $properties.Contains('-test')
[bool]$integrationTest = $properties.Contains('-integrationTest')
[bool]$performanceTest = $properties.Contains('-performanceTest')

$moduleLocation = Resolve-Path "$PSScriptRoot\Screenshots.win.psm1"
$initScreenshotsModule = [scriptblock]::Create("Import-Module $moduleLocation")

$TakeScreenshots = $ci -and ($test -or $integrationTest -or $performanceTest);
$ImageLogs = '';
if ($TakeScreenshots) {
    $ImageLogs = Join-Path $LogDir 'screenshots'
    Create-Directory $ImageLogs

    [ScriptBlock] $ScreenshotCaptureScript = {
        param($ImageLogs)
        Start-CaptureScreenshots "$ImageLogs"
    };

    $job = Start-Job -InitializationScript $initScreenshotsModule `
                -ScriptBlock $ScreenshotCaptureScript `
                -ArgumentList $ImageLogs
}

try {
    # Run the build script that does the actual work
    powershell.exe -File $PSScriptRoot\common\Build.ps1 @properties
    $exitCode = $LASTEXITCODE
    Exit $exitCode;
}
finally {
    if ($TakeScreenshots) {
        [ScriptBlock] $ScreenshotCaptureScript = {
            param($ImageLogs)
            Stop-CaptureScreenshots -TargetDir $ImageLogs
        };
        Start-Job -InitializationScript $initScreenshotsModule `
                  -ScriptBlock $ScreenshotCaptureScript `
                  -ArgumentList $ImageLogs | Receive-Job -AutoRemoveJob -Wait
      }
}
