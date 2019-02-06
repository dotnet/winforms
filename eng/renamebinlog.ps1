[CmdletBinding(PositionalBinding=$false)]
Param(
    [string][Alias('blrn')]$binaryLogRename
)

function Rename
{
    Rename-Item -Path Join-Path $Logdir "Build.binlog" -NewName $binaryLogRename
}

try 
{
    Rename
}
catch 
{
    Write-Host $_
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    ExitWithExitCode 1
}