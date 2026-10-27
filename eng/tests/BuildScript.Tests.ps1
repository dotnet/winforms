$ErrorActionPreference = 'Stop'

function Assert-True([bool] $condition, [string] $message) {
    if (!$condition) {
        throw $message
    }
}

function Invoke-TestBuild([string[]] $arguments, [int] $expectedExitCode = 0) {
    $ErrorActionPreference = 'Continue'
    $output = @(& "$testRoot\eng\build.cmd.ps1" @arguments 2>&1)
    Assert-True ($LASTEXITCODE -eq $expectedExitCode) "Unexpected exit code $LASTEXITCODE for: $arguments`n$output"
    if ($expectedExitCode -eq 0) {
        return Get-Content -LiteralPath $env:WINFORMS_TEST_ARGUMENTS -Raw | ConvertFrom-Json
    }
}

$testRoot = Join-Path ([System.IO.Path]::GetTempPath()) "WinForms-build-tests-$([Guid]::NewGuid().ToString('N'))"
$savedEnvironment = @{}
foreach ($name in @('PROCESSOR_ARCHITECTURE', 'PROCESSOR_ARCHITEW6432', 'DOTNET_GLOBAL_INSTALL_DIR', 'WINFORMS_TEST_ARGUMENTS', 'WINFORMS_TEST_EXIT_CODE')) {
    $savedEnvironment[$name] = [Environment]::GetEnvironmentVariable($name)
}

try {
    New-Item -ItemType Directory -Path "$testRoot\eng\common" -Force | Out-Null
    Copy-Item -LiteralPath "$PSScriptRoot\..\build.cmd.ps1" -Destination "$testRoot\eng\build.cmd.ps1"
    Copy-Item -LiteralPath "$PSScriptRoot\..\dotnet-architecture.ps1" -Destination "$testRoot\eng\dotnet-architecture.ps1"
    $tokens = $null
    $parseErrors = $null
    $arcadeScript = [System.Management.Automation.Language.Parser]::ParseFile(
        "$PSScriptRoot\..\common\build.ps1", [ref] $tokens, [ref] $parseErrors)
    Assert-True ($parseErrors.Count -eq 0) 'Could not read Arcade build parameters.'
    $fixture = '[CmdletBinding(PositionalBinding=$false)]' + "`n" + $arcadeScript.ParamBlock.Extent.Text + @'

$bound = @{}
foreach ($parameter in $PSBoundParameters.GetEnumerator()) {
    $bound[$parameter.Key] = if ($parameter.Value -is [switch]) { [bool] $parameter.Value } else { $parameter.Value }
}
ConvertTo-Json -InputObject $bound -Compress | Set-Content -LiteralPath $env:WINFORMS_TEST_ARGUMENTS
exit ([int] $env:WINFORMS_TEST_EXIT_CODE)
'@
    $fixture | Set-Content -LiteralPath "$testRoot\eng\common\build.ps1"

    $env:WINFORMS_TEST_ARGUMENTS = "$testRoot\arguments.json"
    $env:WINFORMS_TEST_EXIT_CODE = '0'
    $env:DOTNET_GLOBAL_INSTALL_DIR = "$testRoot\SDK with spaces"
    $env:PROCESSOR_ARCHITECTURE = 'ARM64'
    $env:PROCESSOR_ARCHITEW6432 = $null

    $actual = Invoke-TestBuild @()
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=arm64') "ARM64 host did not default to ARM64: $($actual | ConvertTo-Json -Compress)"
    Assert-True ($actual.restore -and $actual.build -and $actual.binaryLog) 'Default build actions missing.'

    $actual = Invoke-TestBuild @('-platform', 'ARM64')
    Assert-True ($actual.platform -eq 'ARM64') 'ARM64 solution platform was discarded.'
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=arm64') 'ARM64 platform did not set target architecture.'

    foreach ($platform in @('x64', 'x86')) {
        $actual = Invoke-TestBuild @('-platform', $platform)
        Assert-True ($actual.properties -contains "/p:TargetArchitecture=$platform") "Explicit $platform target was ignored."
    }

    $actual = Invoke-TestBuild @('/p:Platform=arm64', '/p:TargetArchitecture=x64')
    Assert-True ($actual.properties -contains '/p:Platform=arm64') 'MSBuild platform was discarded.'
    Assert-True ($actual.properties -notcontains '/p:TargetArchitecture=arm64') 'Explicit target architecture was overwritten.'

    $actual = Invoke-TestBuild @('-r', '-b', '-nobl', '-projects', 'project with spaces.csproj')
    Assert-True ($actual.restore -and $actual.build -and $actual.excludeCIBinarylog -and !$actual.binaryLog) 'Action aliases were duplicated.'
    Assert-True ($actual.projects -eq 'project with spaces.csproj') 'Argument containing spaces was split.'

    $actual = Invoke-TestBuild @('-nodeReuse', 'false', '-warnAsError:$true', '-mt', '1', '/p:Example=O''Brien;$notCode')
    Assert-True ($actual.nodeReuse -eq $false -and $actual.warnAsError -eq $true -and $actual.msbuildMultiThreaded -eq $true) 'Boolean arguments lost their types.'
    Assert-True ($actual.properties -contains '/p:Example=O''Brien;$notCode') 'Quoted property value was evaluated as code.'

    $actual = Invoke-TestBuild @('-RestoreOnly')
    Assert-True ($actual.restore -and !$actual.build -and !$actual.binaryLog) 'Restore-only requested a build.'

    $env:PROCESSOR_ARCHITECTURE = 'AMD64'
    $env:PROCESSOR_ARCHITEW6432 = 'ARM64'
    $actual = Invoke-TestBuild @()
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=arm64') 'Emulated shell did not detect the ARM64 OS.'

    $env:PROCESSOR_ARCHITEW6432 = $null
    $actual = Invoke-TestBuild @()
    Assert-True ($actual.properties -notcontains '/p:TargetArchitecture=arm64') 'x64 host was treated as ARM64.'
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=x64') 'x64 host did not default to x64.'
    $actual = Invoke-TestBuild @('-platform', 'arm64')
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=arm64') 'Cross-build target was ignored.'

    foreach ($property in @(
        '/property:TargetArchitecture=arm64',
        '/PROPERTY:Configuration=Debug;TargetArchitecture=arm64',
        '/p:TargetArchitecture=arm64;Platform=x64')) {
        $actual = Invoke-TestBuild @($property)
        Assert-True ($actual.properties -contains $property -and $actual.properties.Count -eq 1) 'Explicit target architecture was overridden.'
    }

    foreach ($property in @('/property:Platform=ARM64', '/p:Configuration=Debug;Platform=arm64')) {
        $actual = Invoke-TestBuild @($property, '-platform', 'x64')
        Assert-True ($actual.properties -contains '/p:TargetArchitecture=arm64') 'MSBuild Platform did not override the Arcade platform.'
    }

    $actual = Invoke-TestBuild @('-platform', 'x86', '/p:Platform=x64', '/property:Platform=ARM64')
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=arm64') 'Last MSBuild Platform did not win.'
    $actual = Invoke-TestBuild @('/p:Example="text;TargetArchitecture=arm64"')
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=x64') 'Quoted property contents were treated as architecture arguments.'

    $actual = Invoke-TestBuild @('-plat', 'arm64', '-rest', '-nobl')
    Assert-True ($actual.properties -contains '/p:TargetArchitecture=arm64' -and $actual.restore) 'Unambiguous Arcade abbreviations were not honored.'
    $actual = Invoke-TestBuild @('-plat:ARM64', '-nodeR:false', '-restore:$false')
    Assert-True ($actual.platform -eq 'ARM64' -and $actual.nodeReuse -eq $false -and $actual.restore -eq $false) 'Inline parameter values were not preserved.'

    Invoke-TestBuild @('-configuration', 'Debug', '-c', 'Release') 1
    Invoke-TestBuild @('-restore', '-rest') 1

    # A failed compile must propagate its exit code without retrying or masking it.
    $env:WINFORMS_TEST_EXIT_CODE = '7'
    Invoke-TestBuild @() 7
    $env:WINFORMS_TEST_EXIT_CODE = '0'
    $null = Invoke-TestBuild @()

    Write-Host 'Build script regression checks passed.'
}
finally {
    foreach ($name in $savedEnvironment.Keys) {
        [Environment]::SetEnvironmentVariable($name, $savedEnvironment[$name])
    }
    if (Test-Path -LiteralPath $testRoot) {
        Remove-Item -LiteralPath $testRoot -Recurse -Force
    }
}
