Add-Type -AssemblyName System.Windows.Forms
Add-Type @" 
  using System; 
  using System.Runtime.InteropServices; 
  public class UserWindows { 
    [DllImport("user32.dll")] 
    public static extern IntPtr GetForegroundWindow(); 
} 
"@

# changeable parameters
$testInner = $true
$logfile = 'results.log'
# end of changable parameters

function SeeActiveWindow()
{
    try 
    { 
        $ActiveHandle = [UserWindows]::GetForegroundWindow() 
        $Process = Get-Process | Where-Object {$_.MainWindowHandle -eq $activeHandle} 
        return $Process.ProcessName
    }
    catch 
    { 
        Write-Error "Failed to get active Window details. More Info: $_" 
    } 
}

function LogWrite
{
    param (
        [parameter(Mandatory=$true)] [string] $logstring
    )

    $date = Get-Date -Format g
    $logadd = -join($date,": ",$logstring)
    Add-Content $logfile -value $logadd
}

function OpenTabTimesEnter()
{
    param (
        [parameter(Mandatory=$true)] [System.Diagnostics.Process] $p,
        [parameter(Mandatory=$true)] [Int] $tabs,
        [parameter(Mandatory=$true)] [string] $nameOfTest,
        [parameter(Mandatory=$true)] [Int] $enters
    )

    # Open the form
    $p.Start() | Out-Null 

    $wshell = New-Object -ComObject wscript.shell
    $wshell.AppActivate('MenuForm') *>$null
    Start-Sleep -m 200

    # Tab over to the appropriate button
    For($i = 0; $i -lt $tabs; $i++)
    {
        if($p.HasExited -ne $true -and (SeeActiveWindow -eq $p.ProcessName))
        {
            [System.Windows.Forms.SendKeys]::SendWait('{TAB}') 
        } 
    }

    # Open that button
    For($j = 0; $j -lt $enters; $j++)
    {
        if($p.HasExited -ne $true -and (SeeActiveWindow -eq $p.ProcessName))
        {
            [System.Windows.Forms.SendKeys]::SendWait('~') 
        } 
    }    
    
    # Wait for new window to be open for half a second
    Start-Sleep -m 500

    if ($p.HasExited -ne $true) # if the window has not closed due to an error
    {
        Stop-Process -name $p.Name # shut 'er down
        $stopwatch =  [system.diagnostics.stopwatch]::StartNew()
        $timeout = $true
        while ($stopwatch.Elapsed.TotalSeconds -lt 5) # if we can't turn it off in 5 seconds, this will be considered a timeout
        {
            if ($p.HasExited -eq $true) # if Stop-Process has worked
            {
                $timeout = $false # then we're a-ok!
                break
            }
            $timeout = $true # if Stop-Process has not worked...
        }
        if ($timeout -eq $true) # ... then we timed out
        {
            LogWrite($nameOfTest + ' timed out.')
            return -2 # new error code for timing out; application returns -1 for all its failures
        }
        else 
        {
            LogWrite($nameOfTest + ' passed.')
            return 0 # no issues have occured  
        }
    }
    else # the window closed due to an error
    {
        LogWrite($nameOfTest + ' failed.')
        return $p.ExitCode 
    }
}

$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = '..\..\..\..\..\artifacts\bin\WinformsControlsTest\Debug\netcoreapp3.0\WinformsControlsTest.exe' # 'WinformsControlsTest.exe' 
$pinfo.RedirectStandardError = $true
$pinfo.RedirectStandardOutput = $true
$pinfo.UseShellExecute = $false
$pinfo.Arguments = ""

# Set the dotnet root to the .dotnet folder that the build installed so we don't use the machine-wide installed one
# TODO: Change this to a script parameter instead of being hardcoded here
$pinfo.EnvironmentVariables['DOTNET_ROOT'] = '..\..\..\..\..\.dotnet'

$p = New-Object System.Diagnostics.Process
$p.StartInfo = $pinfo

LogWrite ("***************************")

$overall = $true

$overall = ((OpenTabTimesEnter -p $p -tabs 0 -nameOfTest 'Overall Form Open' -enters 0) -eq 0) -and $overall #do not click enter

If ($testInner -eq $true)
{
    $overall = ((OpenTabTimesEnter -p $p -tabs 0 -nameOfTest 'Buttons' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 1 -nameOfTest 'Calendar' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 2 -nameOfTest 'TreeView, ImageList' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 3 -nameOfTest 'Content alignment' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 4 -nameOfTest 'Multiple controls' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 5 -nameOfTest 'DataGridView' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 6 -nameOfTest 'Menus' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 7 -nameOfTest 'Panels' -enters 1) -eq 0) -and $overall
    
    $overall = ((OpenTabTimesEnter -p $p -tabs 8 -nameOfTest 'Splitter' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 9 -nameOfTest 'ComboBoxes' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 10 -nameOfTest 'MDI Parent' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 11 -nameOfTest 'Property Grid' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 12 -nameOfTest 'ListView' -enters 1) -eq 0) -and $overall

    $overall = ((OpenTabTimesEnter -p $p -tabs 13 -nameOfTest 'DateTimePickerButton' -enters 1) -eq 0) -and $overall
    
    $overall = ((OpenTabTimesEnter -p $p -tabs 14 -nameOfTest 'FolderBrowserDialogButton' -enters 1) -eq 0) -and $overall
}

if ($overall -eq $true)
{
    0
}
else 
{
    -1
}