[CmdletBinding(PositionalBinding=$false)]
Param(
    [string][Alias('c')]$configuration = "Debug",
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

function _kill($processName) {
    try {
       if (Get-Process -Name "ServerManager" -ErrorAction SilentlyContinue) {
            Write-Host "killing process ${processName}."
            # Redirect stderr to stdout to avoid big red blocks of output in Azure Pipeline logging
            # when there are no instances of the process
            & cmd /c "taskkill /T /F /IM ${processName} 2>&1"
        }
        else {
            Write-Host "ServerManager process is not running"
        }
    } catch {
        Write-Host "Failed to kill ${processName} or delete ServerManager.exe file."
    }
}

# kill server manager process if running on build agents.
_kill severmanager.exe

# If running in admin mode, try deleting ServerManager.exe to prevent it from launching later on.
$isAdmin = ([System.Security.Principal.WindowsIdentity]::GetCurrent().Token).Groups -contains ([System.Security.Principal.SecurityIdentifier]"S-1-5-32-544")
if ($isAdmin ) {
	$filePath = "C:\Windows\system32\ServerManager.exe"
	if (Test-Path $filePath) {
		## Remove binary in case process has not started yet.
		if (Remove-Item "C:\Windows\System32\ServerManager.exe")
		{
			Write-Host "ServerManager.exe file is deleted."
		}
		else
		{
			Write-Host "Failed to delete ServerManager.exe file."
		}
	}
	else{
		Write-Host "ServerManager.exe file does not exist. Skipping deletion."
	}

	# Wait to delete the file.
	Start-Sleep -Seconds 3
}

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
