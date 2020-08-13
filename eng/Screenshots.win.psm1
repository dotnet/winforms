<#
  Licensed to the .NET Foundation under one or more agreements. The .NET Foundation licenses this file to you under the MIT license.
  See the LICENSE file in the project root for more information.
#>

# Collection of powershell build utility functions that we use across our scripts.
# Copied from https://raw.githubusercontent.com/dotnet/roslyn/3f851026b30b335ae328ce38a7817d93b96ad4ad/eng/build-utils-win.ps1

Set-StrictMode -version 2.0
$ErrorActionPreference="Stop"
$FLAG_FILE = "screenshots.lock"

Add-Type -AssemblyName 'System.Drawing'
Add-Type -AssemblyName 'System.Windows.Forms'

. $PSScriptRoot\common\pipeline-logging-functions.ps1

function Get-Now {
  return (Get-Date).ToString("yyyy-MM-yy HH:mm:ss")
}

function Save-Screenshot() {
  Param (
    [string] $TargetFileName,
    [string] $LogFile
  )

  $width = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Width
  $height = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Height

  $bitmap = New-Object System.Drawing.Bitmap $width, $height
  try {
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    try {
      $graphics.CopyFromScreen( `
        [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.X, `
        [System.Windows.Forms.Screen]::PrimaryScreen.Bounds.Y, `
        0, `
        0, `
        $bitmap.Size, `
        [System.Drawing.CopyPixelOperation]::SourceCopy)
    } finally {
      $graphics.Dispose()
    }

    $bitmap.Save($TargetFileName, [System.Drawing.Imaging.ImageFormat]::Png)

    $fullPath = Resolve-Path $TargetFileName
    Write-Host "##vso[task.uploadfile]$fullPath";
    "[SAVE] $fullPath" >> $LogFile;
  } finally {
    $bitmap.Dispose()
  }
}

function Capture-Screenshot() {
  Param (
    [string] $TargetDir,
    [string] $LogFile
  )

  $screenshotPath = (Join-Path $TargetDir "$(Get-Date -Format 'yyyyMMdd.HHmmss').png")
  try {
    Save-Screenshot -TargetFileName $screenshotPath -LogFile $LogFile
  }
  catch {
    Write-PipelineTaskError -Message "Screenshot failed; attempt to connect to the console?"
    "[CAPTURE] Screenshot failed; attempt to connect to the console?" >> $LogFile;
  }
}

function Start-CaptureScreenshots() {
<#
.SYNOPSIS

Starts capturing screenshots.

.DESCRIPTION

Starts capturing screenshots by setting a lock file in the specified directory.
If the target folder already contains a lock file, then it is assumed there is another
process that is capturing screenshots, this is the end.

Screenshots are captured indefinitely every 3 minutes until the lock file
in the target folder is deleted.

.PARAMETER TargetDir
The full path to a folder that contains the lock file.

.INPUTS

None.

.OUTPUTS

None.

#>
  Param (
    [string] $TargetDir,
    [int] $WaitSeconds
  )

  $flagFile = Join-Path $TargetDir $FLAG_FILE;
  $logFile = $flagFile.Replace('.lock', '.log');
  "[START] Flag: $flagFile" >> $logFile;

  $hasFlagFile = Test-Path $flagFile
  if ($hasFlagFile) {
    Write-PipelineTaskError -Message "Screenshots are already being taken!" -Type 'warning'
    "[$(Get-Now)] Screenshots are already being taken!" >> $logFile;
    return;
  }

  '' | Out-File $flagFile

  do
  {
    $hasFlagFile = Test-Path $flagFile
    if (!$hasFlagFile) {
      Write-PipelineTaskError -Message "Screenshots no longer being taken" -Type 'warning'
      "[$(Get-Now)] Screenshots no longer being taken" >> $logFile;
      return;
    }

    Start-Sleep -Seconds $WaitSeconds

    Capture-Screenshot -TargetDir $TargetDir -LogFile $logFile;
    "[$(Get-Now)] Screenshot taken" >> $logFile;

    $hasFlagFile = Test-Path $flagFile
  }
  until (!$hasFlagFile)
}

function Stop-CaptureScreenShots()
{
<#
.SYNOPSIS

Stop capturing screenshots by removing the lock file.

.DESCRIPTION

Stop capturing screenshots by removing the lock file.

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

  "[$(Get-Now)] Stopped" >> $logFile;
}

Export-ModuleMember -Function Start-CaptureScreenshots
Export-ModuleMember -Function Stop-CaptureScreenShots