[CmdletBinding(PositionalBinding=$false)]
Param(
    [string][Alias('c')]$configuration = "Debug",
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

# How long to wait before we consider a build/test run to be unresponsive
$WaitSeconds = 900 # 15 min

. $PSScriptRoot\common\tools.ps1

# CI mode - set to true on CI server for PR validation build or official build.
[bool]$ci = $properties.Contains('-ci')

# Tests - set to true if running tests.
[bool]$test = $properties.Contains('-test')
[bool]$integrationTest = $properties.Contains('-integrationTest')
[bool]$performanceTest = $properties.Contains('-performanceTest')

$screenshotsModuleLocation = Resolve-Path "$PSScriptRoot\Screenshots.win.psm1"
$initScreenshotsModule = [scriptblock]::Create("Import-Module $screenshotsModuleLocation")
$memDumpModuleLocation = Resolve-Path "$PSScriptRoot\MemDump.win.psm1"
$initMemDumpModule = [scriptblock]::Create("Import-Module $memDumpModuleLocation")

$CollectDebugInfo = $ci -and ($test -or $integrationTest -or $performanceTest);
$ImageLogs = '';
if ($CollectDebugInfo) {

    dotnet tool install dotnet-dump --tool-path .\.tools

    # Collect screenshots
    # -----------------------------------------------------------
    $ImageLogs = Join-Path $LogDir 'screenshots'
    Create-Directory $ImageLogs

    [ScriptBlock] $ScreenshotCaptureScript = {
        param($ImageLogs, $WaitSeconds)
        Start-CaptureScreenshots -TargetDir $ImageLogs -WaitSeconds $WaitSeconds
    };
    $job = Start-Job -InitializationScript $initScreenshotsModule `
                -ScriptBlock $ScreenshotCaptureScript `
                -ArgumentList @( $ImageLogs, $WaitSeconds );

    # Collect memory dump
    # -----------------------------------------------------------
    [ScriptBlock] $MemDumpScript = {
        param($WorkingDir, $LogDir, $WaitSeconds)
        Start-MemDumpTimer -WorkingDir $WorkingDir -TargetDir $LogDir -WaitSeconds $WaitSeconds
    };
    $job = Start-Job -InitializationScript $initMemDumpModule `
                -ScriptBlock $MemDumpScript `
                -ArgumentList @( $(Get-Location), $LogDir, $WaitSeconds )

}

try {

    # Run the build script that does the actual work
    # -----------------------------------------------------------
    powershell.exe -File $PSScriptRoot\common\Build.ps1 -c $configuration @properties
    $exitCode = $LASTEXITCODE
    Exit $exitCode;

}
finally {
    if ($CollectDebugInfo) {

        # Stop collecting screenshots
        # -----------------------------------------------------------
        [ScriptBlock] $ScreenshotCaptureScript = {
            param($ImageLogs)
            Stop-CaptureScreenshots -TargetDir $ImageLogs
        };
        Start-Job -InitializationScript $initScreenshotsModule `
                  -ScriptBlock $ScreenshotCaptureScript `
                  -ArgumentList $ImageLogs | Receive-Job -AutoRemoveJob -Wait;

        # Stop collect memory dumps
        # -----------------------------------------------------------
        [ScriptBlock] $MemDumpScript = {
            param($LogDir)
            Stop-MemDumpTimer -TargetDir $LogDir
        };
        Start-Job -InitializationScript $initMemDumpModule `
                  -ScriptBlock $MemDumpScript `
                  -ArgumentList $LogDir | Receive-Job -AutoRemoveJob -Wait
    }
}
