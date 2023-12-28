<#
.SYNOPSIS
Downloads 32-bit and 64-bit procdump executables and returns the path to where they were installed.
eng\MergeCoverageData.ps1
#>
[CmdletBinding(PositionalBinding=$false)]
Param(
  [string][Alias('c')]$configuration = "Debug",
  [string] $artifactsDir
)

$converageToolsPath = Join-Path $artifactsDir "tools"
if (!(Test-Path -Path $converageToolsPath)) {
  New-Item -Path $converageToolsPath -ItemType Directory
}
dotnet tool install dotnet-reportgenerator-globaltool --tool-path $converageToolsPath

$converageTools = Join-Path $converageToolsPath "reportgenerator.exe"
Test-Path -Path $converageTools
$reportCoverage = Join-Path $artifactsDir "bin\*\Debug\*\coverage\*.coverage"
$resultDir = Join-Path $artifactsDir "CoverageResult\Debug"
$reporttype = 'Html;HtmlInline_AzurePipelines'
$reporttypeXml = 'Cobertura'
$filter = '+System.Windows.Forms*;-*Tests.dll'
& $converageTools -reports:$reportCoverage -targetDir:$resultDir -reporttypes:$reporttypeXml -assemblyfilters:$filter
