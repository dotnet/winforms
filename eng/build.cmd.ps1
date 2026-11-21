$ErrorActionPreference = 'Stop'
$BuildArgs = @($args)

try {
    . "$PSScriptRoot\dotnet-architecture.ps1"
    $hostArchitecture = Get-WindowsBuildArchitecture
    $buildScriptPath = "$PSScriptRoot\common\build.ps1"
    $buildParameters = (Get-Command $buildScriptPath).Parameters.Values

    function Get-ArcadeParameter([string] $argument) {
        if ($argument -notmatch '^-(?<Name>[A-Za-z]+)(:.*)?$') {
            return
        }

        $name = $Matches.Name
        $parameters = @($buildParameters | Where-Object { $_.Name -eq $name -or $_.Aliases -contains $name })
        if ($parameters.Count -eq 0) {
            $parameters = @($buildParameters | Where-Object {
                $names = @($_.Name) + @($_.Aliases)
                @($names | Where-Object { $_.StartsWith($name, [StringComparison]::OrdinalIgnoreCase) }).Count -gt 0
            })
        }

        if ($parameters.Count -eq 1) {
            return $parameters[0]
        }
    }

    $forwardArgs = [System.Collections.Generic.List[string]]::new()
    $platform = $null
    $propertyPlatform = $null
    $hasPropertyPlatform = $false
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

        if ($arg -match '^[-/](p|property):(.+)$') {
            $propertyList = $Matches[2]
            foreach ($property in [regex]::Matches($propertyList, '(?:^|;)\s*(?<Name>[^=;]+)=(?<Value>(?:"[^"]*"|[^;"])*)')) {
                switch ($property.Groups['Name'].Value.Trim()) {
                    'TargetArchitecture' { $hasTargetArchitecture = $true }
                    'Platform' {
                        $propertyPlatform = $property.Groups['Value'].Value.Trim().Trim('"')
                        $hasPropertyPlatform = $true
                    }
                }
            }
        }

        $parameter = Get-ArcadeParameter $arg
        if ($parameter.Name -eq 'restore') { $hasRestore = $true }
        if ($parameter.Name -eq 'build') { $hasBuild = $true }
        if ($parameter.Name -in @('binaryLog', 'binaryLogName', 'excludeCIBinarylog')) { $hasBinaryLog = $true }
        if ($parameter.Name -eq 'platform') {
            $parts = $arg.Split(':', 2)
            if ($parts.Length -eq 2) { $platform = $parts[1] }
            elseif ($i + 1 -lt $BuildArgs.Length) { $platform = $BuildArgs[$i + 1] }
        }

        $forwardArgs.Add($arg)
    }

    if (!$hasTargetArchitecture) {
        # Arcade emits -platform before its remaining MSBuild properties.
        $targetArchitecture = if ($hasPropertyPlatform) { $propertyPlatform } else { $platform }
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
    $buildScript = $buildScriptPath.Replace("'", "''")
    $command = "`$ErrorActionPreference = 'Stop'; `$ProgressPreference = 'SilentlyContinue'; & '$buildScript' -NativeToolsOnMachine"
    for ($i = 0; $i -lt $forwardArgs.Count; $i++) {
        $arg = $forwardArgs[$i]
        $parameter = Get-ArcadeParameter $arg
        $parts = $arg.Split(':', 2)
        if ($parameter.ParameterType -eq [bool] -or
            ($parameter.ParameterType -eq [switch] -and $parts.Length -eq 2)) {
            $value = if ($parts.Length -eq 2) { $parts[1] } else {
                $i++
                if ($i -ge $forwardArgs.Count) { throw "Missing Boolean value for -$($parameter.Name)." }
                $forwardArgs[$i]
            }
            $separator = if ($parameter.ParameterType -eq [switch]) { ':' } else { ' ' }
            switch ($value.TrimStart('$').ToLowerInvariant()) {
                { $_ -in @('true', '1') } { $command += " -$($parameter.Name)$separator`$true" }
                { $_ -in @('false', '0') } { $command += " -$($parameter.Name)$separator`$false" }
                default { throw "Invalid Boolean value for -$($parameter.Name): $value" }
            }
        }
        elseif ($parameter) {
            $command += " -$($parameter.Name)"
            if ($parts.Length -eq 2) {
                $command += " '$($parts[1].Replace("'", "''"))'"
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
    Write-Error $_ -ErrorAction Continue
    exit 1
}
