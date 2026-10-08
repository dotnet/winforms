$ErrorActionPreference = 'Stop'

function Assert-True([bool] $condition, [string] $message) {
    if (!$condition) {
        throw $message
    }
}

function Invoke-TestBuild([string[]] $arguments, [int] $expectedExitCode = 0) {
    & "$testRoot\eng\build.cmd.ps1" @arguments
    Assert-True ($LASTEXITCODE -eq $expectedExitCode) "Unexpected exit code for: $arguments"
    if ($expectedExitCode -eq 0) {
        return @(Get-Content -LiteralPath $env:WINFORMS_TEST_ARGUMENTS -Raw | ConvertFrom-Json)
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
    @'
ConvertTo-Json -InputObject @($args) -Compress | Set-Content -LiteralPath $env:WINFORMS_TEST_ARGUMENTS
exit ([int] $env:WINFORMS_TEST_EXIT_CODE)
'@ | Set-Content -LiteralPath "$testRoot\eng\common\build.ps1"

    $env:WINFORMS_TEST_ARGUMENTS = "$testRoot\arguments.json"
    $env:WINFORMS_TEST_EXIT_CODE = '0'
    $env:DOTNET_GLOBAL_INSTALL_DIR = "$testRoot\SDK with spaces"
    $env:PROCESSOR_ARCHITECTURE = 'ARM64'
    $env:PROCESSOR_ARCHITEW6432 = $null

    $actual = Invoke-TestBuild @()
    Assert-True ($actual -contains '/p:TargetArchitecture=arm64') 'ARM64 host did not default to ARM64.'
    Assert-True ($actual -contains '-restore' -and $actual -contains '-build' -and $actual -contains '-bl') 'Default build actions missing.'

    $actual = Invoke-TestBuild @('-platform', 'ARM64')
    Assert-True ($actual -contains '-platform' -and $actual -contains 'ARM64') 'ARM64 solution platform was discarded.'
    Assert-True ($actual -contains '/p:TargetArchitecture=arm64') 'ARM64 platform did not set target architecture.'

    foreach ($platform in @('x64', 'x86')) {
        $actual = Invoke-TestBuild @('-platform', $platform)
        Assert-True ($actual -contains "/p:TargetArchitecture=$platform") "Explicit $platform target was ignored."
    }

    $actual = Invoke-TestBuild @('/p:Platform=arm64', '/p:TargetArchitecture=x64')
    Assert-True ($actual -contains '/p:Platform=arm64') 'MSBuild platform was discarded.'
    Assert-True ($actual -notcontains '/p:TargetArchitecture=arm64') 'Explicit target architecture was overwritten.'

    $actual = Invoke-TestBuild @('-r', '-b', '-nobl', '-projects', 'project with spaces.csproj')
    Assert-True ($actual -notcontains '-restore' -and $actual -notcontains '-build' -and $actual -notcontains '-bl') 'Action aliases were duplicated.'
    Assert-True ($actual -contains 'project with spaces.csproj') 'Argument containing spaces was split.'

    $actual = Invoke-TestBuild @('-nodeReuse', 'false', '-warnAsError:$true', '-mt', '1', '/p:Example=O''Brien;$notCode')
    Assert-True ($actual -contains $false -and $actual -contains $true) 'Boolean arguments lost their types.'
    Assert-True ($actual -contains '/p:Example=O''Brien;$notCode') 'Quoted property value was evaluated as code.'

    $actual = Invoke-TestBuild @('-RestoreOnly')
    Assert-True ($actual -contains '-restore' -and $actual -notcontains '-build' -and $actual -notcontains '-bl') 'Restore-only requested a build.'

    $env:PROCESSOR_ARCHITECTURE = 'AMD64'
    $env:PROCESSOR_ARCHITEW6432 = 'ARM64'
    $actual = Invoke-TestBuild @()
    Assert-True ($actual -contains '/p:TargetArchitecture=arm64') 'Emulated shell did not detect the ARM64 OS.'

    $env:PROCESSOR_ARCHITEW6432 = $null
    $actual = Invoke-TestBuild @()
    Assert-True ($actual -notcontains '/p:TargetArchitecture=arm64') 'x64 host was treated as ARM64.'
    Assert-True ($actual -contains '/p:TargetArchitecture=x64') 'x64 host did not default to x64.'
    $actual = Invoke-TestBuild @('-platform', 'arm64')
    Assert-True ($actual -contains '/p:TargetArchitecture=arm64') 'Cross-build target was ignored.'

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
