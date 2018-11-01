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
$waitTime = 200
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

function TabOpenClose 
{
    param (
        [parameter(Mandatory=$true)] [System.Diagnostics.Process] $p
    )

    if($p.HasExited -ne $true -and (SeeActiveWindow -eq $p.ProcessName))
    {
        [System.Windows.Forms.SendKeys]::SendWait('{TAB}') #DateTimePickerButton
    } 
    
    OpenClose($p)
}

function OpenClose 
{
    param (
        [parameter(Mandatory=$true)] [System.Diagnostics.Process] $p
    )

    if($p.HasExited -ne $true -and (SeeActiveWindow -eq $p.ProcessName))
    {
        [System.Windows.Forms.SendKeys]::SendWait('~')
    }
    Start-Sleep -m $waitTime
    if($p.HasExited -ne $true -and (SeeActiveWindow -eq $p.ProcessName))
    {
        [System.Windows.Forms.SendKeys]::SendWait('%{F4}')
    } 
}

$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = 'WinformsControlsTest.exe' # path to exe in adjacent folder ..\WinformsControlsTest\bin\Debug\netcoreapp3.0\WinformsControlsTest.exe
$pinfo.RedirectStandardError = $true
$pinfo.RedirectStandardOutput = $true
$pinfo.UseShellExecute = $false
$pinfo.Arguments = ""
$p = New-Object System.Diagnostics.Process
$p.StartInfo = $pinfo

$p.Start() | Out-Null

$wshell = New-Object -ComObject wscript.shell
$wshell.AppActivate('MenuForm') *>$null
Start-Sleep -m 2000

If ($testInner -eq $true)
{
    OpenClose($p) #Buttons

    TabOpenClose($p) #Calendar

    TabOpenClose($p) #TreeView, ImageList

    TabOpenClose($p) #Content alignment

    TabOpenClose($p) #Multiple controls

    TabOpenClose($p) #DataGridView

    TabOpenClose($p) #Menus

    TabOpenClose($p) #Panels
    
    TabOpenClose($p) #Splitter

    TabOpenClose($p) #ComboBoxes

    TabOpenClose($p) #MDI Parent

    TabOpenClose($p) #Property Grid

    TabOpenClose($p) #ListView

    TabOpenClose($p) #DateTimePickerButton
}

# final close
if ($p.HasExited -ne $true)
{
    Stop-Process -name $p.Name
    $p.WaitForExit() 
    0 #no issues have occured  
}
else 
{
    $p.ExitCode #there was some kind of issue
}
