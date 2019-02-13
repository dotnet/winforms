[CmdletBinding(PositionalBinding=$false)]
Param(
    [Parameter(Mandatory=$true,
    Position=0,
    HelpMessage='pathToFile must be given')][string][Alias('p')]$pathToFile,
    [Parameter(Mandatory=$true,
    Position=0,
    HelpMessage='newName must be given')][string][Alias('n')]$newName,
    [switch] $help,
    [Parameter(ValueFromRemainingArguments=$true)][String[]]$properties
)

function Print-Usage 
{
    Write-Host "  -pathToFile <value>     The fully qualified path to the file to rename (short: -p)"
    Write-Host "  -newName <value>        The name of the file after this operation (short: -n)"
    Write-Host "  -help                   Print help and exit"
    Write-Host ""
}

function Rename
{
    if (-not (Test-Path -Path $pathToFile))
    {
        throw [System.IO.FileNotFoundException] "$pathToFile not found." 
    }
    if (-not (Test-Path -Path $pathToFile -PathType leaf))
    {
        throw [System.IO.ArgumentException] "$pathToFile must lead to a file, not a directory." 
    }

    Rename-Item -Path $pathToFile -NewName $newName
}

try 
{
    if ($help -or (($null -ne $properties) -and ($properties.Contains("/help") -or $properties.Contains("/?")))) 
    {
      Print-Usage
      exit 0
    }
  
    Rename
}
catch 
{
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    ExitWithExitCode 1
}