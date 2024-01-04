[CmdletBinding(PositionalBinding=$false)]
Param(
  [string][Alias('c')]$configuration = "Debug",
  [string] $artifactsDir
)

$converageToolsPath = Join-Path $artifactsDir "tools"
if (!(Test-Path -Path $converageToolsPath)) {
  New-Item -Path $converageToolsPath -ItemType Directory
}
$TestresultPath = Join-Path $artifactsDir "bin\CodeCoverage\coverage\Cobertura.xml"
if(Test-Path -Path $TestresultPath){
  echo "Merge Data"
}
dotnet tool install dotnet-reportgenerator-globaltool --tool-path $converageToolsPath
$converageTools = Join-Path $converageToolsPath "reportgenerator.exe"
Test-Path -Path $converageTools
$reportCoverage = Join-Path $artifactsDir "bin\*\Debug\*\coverage\*.coverage"
$resultDir = Join-Path $artifactsDir "CoverageResult\Debug"
$reporttype = 'Cobertura'
$assemblyFilters = '+System.Windows.Forms*;-*Tests.dll'
$fileFilters = '-*.g.cs'
& $converageTools -reports:$reportCoverage -targetDir:$resultDir -reporttypes:$reporttype -assemblyfilters:$assemblyFilters -filefilters:$fileFilters
