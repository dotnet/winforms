[CmdletBinding(PositionalBinding=$false)]
Param(
  [string][Alias('c')]$configuration = "Debug",
  [string] $artifactsDir
)

$converageToolsPath = Join-Path $artifactsDir "tools"
if (!(Test-Path -Path $converageToolsPath)) {
  New-Item -Path $converageToolsPath -ItemType Directory
}
if(!(Test-Path -Path $PkgReportGenerator)){
 echo 'Can not find the path'
}
if(Test-Path -Path $PkgReportGenerator){
 $reportTools = Join-Path $PkgReportGenerator "tools\net47\ReportGenerator.exe"
  if(Test-Path -Path $reportTools){
  echo 'Can find the ReportGenerator'
  }
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
