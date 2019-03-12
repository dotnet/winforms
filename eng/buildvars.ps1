# Open global.json
$jsondata = Get-Content -Raw -Path $env:BUILD_SOURCESDIRECTORY | ConvertFrom-Json

# Set DotNetCliVersion to global.json.tools.dotnet
$dotnetcliver = $jsondata.tools.dotnet
Write-Host "##vso[task.setvariable variable=DotNetCliVersion;]$dotnetcliver"