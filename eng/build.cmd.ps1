[CmdletBinding(PositionalBinding = $false)]
param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $BuildArgs
)

$forwardArgs = [System.Collections.Generic.List[string]]::new()
$useNativeTools = $true
$isArm64Platform = $false
$hasTargetArchitecture = $false

for ($i = 0; $i -lt $BuildArgs.Length; $i++) {
    $arg = $BuildArgs[$i]

    if ($arg.StartsWith('/p:TargetArchitecture=', [StringComparison]::OrdinalIgnoreCase) -or
        $arg.StartsWith('-p:TargetArchitecture=', [StringComparison]::OrdinalIgnoreCase)) {
        $hasTargetArchitecture = $true
    }

    if ($arg.Equals('-platform', [StringComparison]::OrdinalIgnoreCase) -and
        $i + 1 -lt $BuildArgs.Length -and
        $BuildArgs[$i + 1].Equals('arm64', [StringComparison]::OrdinalIgnoreCase)) {
        $isArm64Platform = $true
        $i++
        continue
    }

    if ($arg.Equals('/p:Platform=arm64', [StringComparison]::OrdinalIgnoreCase) -or
        $arg.Equals('-p:Platform=arm64', [StringComparison]::OrdinalIgnoreCase)) {
        $isArm64Platform = $true
        continue
    }

    $forwardArgs.Add($arg)
}

if ($isArm64Platform) {
    $useNativeTools = $false

    if (!$hasTargetArchitecture) {
        $forwardArgs.Add('/p:TargetArchitecture=arm64')
    }
}

$buildScript = Join-Path $PSScriptRoot 'common\build.ps1'
$baseArgs = @()

if ($useNativeTools) {
    $baseArgs += '-NativeToolsOnMachine'
}

$processArgs = @(
    '-ExecutionPolicy'
    'ByPass'
    '-NoProfile'
    '-File'
    $buildScript
) + $baseArgs + @(
    '-restore'
    '-build'
    '-bl'
) + $forwardArgs

& powershell @processArgs
exit $LASTEXITCODE
