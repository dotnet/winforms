<#
  Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license.
  See the LICENSE file in the project root for more information.
#>

# Collection of powershell build utility functions that we use across our scripts.
# Copied from https://raw.githubusercontent.com/dotnet/roslyn/3f851026b30b335ae328ce38a7817d93b96ad4ad/eng/build-utils-win.ps1

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"
$FLAG_FILE = "memdump.lock"

. $PSScriptRoot\common\pipeline-logging-functions.ps1

function Get-Now {
  return (Get-Date).ToString("yyyy-MM-yy HH:mm:ss")
}
function Start-MemDumpTimer() {
<#
.SYNOPSIS

Initiates the timer to monitor a process.

.DESCRIPTION

Starts process monitoring by setting a lock file in the specified directory.
If the target folder already contains a lock file, then it is assumed there is another
process that is monitoring processes, this is the end.

The monitoring is performed either until the lock file
in the target folder is deleted, or the allotted time has run out.

.PARAMETER WorkingDir
The full path to a repository root folder.

.PARAMETER TargetDir
The full path to a folder that contains the lock file.

.PARAMETER WaitSeconds
The time (in seconds) allowed for a process to run before it is considered unresponsive.
At which point a memory dump is captured, and the process is terminated

.INPUTS

None.

.OUTPUTS

None.

#>
  Param (
    [string] $WorkingDir,
    [string] $TargetDir,
    [int] $WaitSeconds
  )

  $flagFile = Join-Path $TargetDir $FLAG_FILE;
  $logFile = $flagFile.Replace('.lock', '.log');
  "[START] $WorkingDir, flag: $flagFile" >> $logFile;
  Push-Location $WorkingDir

  $hasFlagFile = Test-Path $flagFile
  if ($hasFlagFile) {
    Write-PipelineTaskError -Message "MemDump timer is already running!" -Type 'warning'
    "[$(Get-Now)] MemDump timer is already running!" >> $logFile;
    return;
  }

  '' | Out-File $flagFile

  $processes = @();

  do {
    $hasFlagFile = Test-Path $flagFile
    if (!$hasFlagFile) {
      Write-PipelineTaskError -Message "MemDump timer is stopped" -Type 'warning'
      "[$(Get-Now)] MemDump timer is stopped" >> $logFile;
      return;
    }

    "[$(Get-Now)] MemDump wait for $WaitSeconds seconds" >> $logFile;
    Start-Sleep -Seconds $WaitSeconds

    Get-WmiObject win32_process -Filter "name like 'dotnet.exe'" | `
    Where-Object { $_.CommandLine.Contains('System.Windows.Forms') } | `
    Select-Object ProcessId, ProcessName, CommandLine | `
    ForEach-Object {
      $processes += $_;
    }

    # collect all memory dump before killing processes, as killing one process may lead to cascade kill
    $processes | ForEach-Object {
      $processId = $_.ProcessId;
      $dumpFile = Join-Path $TargetDir "dotnet.exe.$processId.dmp";

      "[$(Get-Now)] MemDump collect for pid: $processId, file: $dumpFile" >> $logFile;
      try {
        .\.tools\dotnet-dump collect --process-id $processId --type Heap --output $dumpFile
        "[$(Get-Now)] MemDump collected!" >> $logFile;
      }
      catch {
        $_ >> $logFile;
      }
    }

    # kill all identified processes
    $processes | ForEach-Object {
      $processId = $_.ProcessId;
      "[$(Get-Now)] MemDump kill pid: $processId" >> $logFile;
      Stop-Process -Id $processId
      "[$(Get-Now)] MemDump killed" >> $logFile;
    }

    $hasFlagFile = Test-Path $flagFile
  }
  until (!$hasFlagFile)
}

function Stop-MemDumpTimer() {
<#
.SYNOPSIS

Stop process monitoring by removing the lock file.

.DESCRIPTION

Stop process monitoring by removing the lock file.

.PARAMETER TargetDir
The full path to a folder that contains the lock file.

.INPUTS

None.

.OUTPUTS

None.

#>
  Param (
    [string] $TargetDir
  )

  $flagFile = Join-Path $TargetDir $FLAG_FILE;
  $logFile = $flagFile.Replace('.lock', '.log');

  $hasFlagFile = Test-Path $flagFile
  if ($hasFlagFile) {
    Remove-Item -Path $flagFile -Force
    "[$(Get-Now)] Flag file removed" >> $logFile;
  }

  "[$(Get-Now)] Stopped`r`n`r`n" >> $logFile;
}

Export-ModuleMember -Function Start-MemDumpTimer
Export-ModuleMember -Function Stop-MemDumpTimer