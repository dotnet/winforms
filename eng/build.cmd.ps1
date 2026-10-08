$ErrorActionPreference = 'Stop'
$BuildArgs = @($args)

try {
    . "$PSScriptRoot\dotnet-architecture.ps1"
    $hostArchitecture = Get-WindowsBuildArchitecture

    $forwardArgs = [System.Collections.Generic.List[string]]::new()
    $targetArchitecture = $null
    $hasTargetArchitecture = $false
    $hasRestore = $false
    $hasBuild = $false
    $hasBinaryLog = $false
    $restoreOnly = $false

    for ($i = 0; $i -lt $BuildArgs.Length; $i++) {
        $arg = $BuildArgs[$i]
        if ($arg -eq '-RestoreOnly') {
            $restoreOnly = $true
            continue
        }

        if ($arg -match '^[-/]p:TargetArchitecture=') { $hasTargetArchitecture = $true }
        if ($arg -match '^-(restore|r)$') { $hasRestore = $true }
        if ($arg -match '^-(build|b)$') { $hasBuild = $true }
        if ($arg -match '^-(binaryLog|bl|binaryLogName|bln|excludeCIBinarylog|nobl)$') { $hasBinaryLog = $true }

        if ($arg -eq '-platform' -and $i + 1 -lt $BuildArgs.Length) {
            $targetArchitecture = $BuildArgs[$i + 1]
        }
        elseif ($arg -match '^[-/]p:Platform=(.+)$') {
            $targetArchitecture = $Matches[1]
        }

        $forwardArgs.Add($arg)
    }

    if (!$hasTargetArchitecture) {
        if ($targetArchitecture -match '^(arm64|x64|x86)$') {
            $forwardArgs.Add("/p:TargetArchitecture=$($targetArchitecture.ToLowerInvariant())")
        }
        else {
            $forwardArgs.Add("/p:TargetArchitecture=$hostArchitecture")
        }
    }

    if (!$hasRestore) { $forwardArgs.Add('-restore') }
    if (!$restoreOnly -and !$hasBuild) { $forwardArgs.Add('-build') }
    if (!$restoreOnly -and !$hasBinaryLog) { $forwardArgs.Add('-bl') }

    # -File cannot bind Boolean parameters in Windows PowerShell. Encode a command
    # with quoted values and literal Boolean arguments instead of losing their types.
    $buildScript = "$PSScriptRoot\common\build.ps1".Replace("'", "''")
    $command = "`$ProgressPreference = 'SilentlyContinue'; & '$buildScript' -NativeToolsOnMachine"
    for ($i = 0; $i -lt $forwardArgs.Count; $i++) {
        $arg = $forwardArgs[$i]
        if ($arg -match '^-(nodeReuse|warnAsError|msbuildMultiThreaded|mt)(:(.+))?$') {
            $parameter = $Matches[1]
            $value = if ($Matches[3]) { $Matches[3] } else {
                $i++
                if ($i -ge $forwardArgs.Count) { throw "Missing Boolean value for -$parameter." }
                $forwardArgs[$i]
            }
            switch ($value.TrimStart('$').ToLowerInvariant()) {
                { $_ -in @('true', '1') } { $command += " -$parameter `$true" }
                { $_ -in @('false', '0') } { $command += " -$parameter `$false" }
                default { throw "Invalid Boolean value for -${parameter}: $value" }
            }
        }
        elseif ($arg -match '^-[A-Za-z]+$') {
            $command += " $arg"
        }
        else {
            $command += " '$($arg.Replace("'", "''"))'"
        }
    }

    $command += ' 6>&1; exit $LASTEXITCODE'
    $encodedCommand = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($command))
    & powershell -ExecutionPolicy ByPass -NoProfile -OutputFormat Text -EncodedCommand $encodedCommand
    exit $LASTEXITCODE
}
catch {
    Write-Error $_
    exit 1
}
