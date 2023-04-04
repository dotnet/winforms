[CmdletBinding(PositionalBinding=$false)]
Param(
    [string][Alias('c')]$configuration = "Debug",
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

function _endProcess($processName) {
    try {
       if (Get-Process -Name ${processName} -ErrorAction SilentlyContinue) {
            Write-Host "Ending process ${processName}."
            # Redirect stderr to stdout to avoid big red blocks of output in Azure Pipeline logging.
            & cmd /c "taskkill /T /F /IM ${processName}.exe 2>&1"
        }
        else {
            Write-Host "${processName} process is not running"
        }
    } catch {
        Write-Host "Failed to end process ${processName}"
    }
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

$memDumpModuleLocation = Resolve-Path "$PSScriptRoot\MemDump.win.psm1"
$initMemDumpModule = [scriptblock]::Create("Import-Module $memDumpModuleLocation")

$CollectDebugInfo = $ci -and ($test -or $integrationTest -or $performanceTest);
$ImageLogs = '';
if ($CollectDebugInfo) {

    dotnet tool install dotnet-dump --tool-path .\.tools

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

 if ($ci -and $integrationTest) {
      # Minimize all windows to avoid interference during integration test runs
      $shell = New-Object -ComObject "Shell.Application"
      $shell.MinimizeAll()
	  Write-Host "Minimized all windows"
	  
	  # end server manager process if running on build agents.
	  _endProcess ServerManager
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
