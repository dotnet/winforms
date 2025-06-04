[CmdletBinding(PositionalBinding=$false)]
Param(
  [Parameter(Mandatory=$True, Position=1)]
  [string] $NuspecFile,
  [Parameter(Mandatory=$True, Position=2)]
  [string] $TargetFile,
  [Parameter(Mandatory=$True, Position=3)]
  [string] $GenerateManifest,
  [Parameter(Mandatory=$True, Position=4)]
  [string] $ExpectedAssemblyVersion,
  [Parameter(Mandatory=$True, Position=5)]
  [string] $IsServicingRelease,
  [Parameter(ValueFromRemainingArguments=$true)][String[]] $properties
)


[xml] $xmlDoc = Get-Content -Path $NuspecFile -Force;

$assemblies = $xmlDoc.package.files.file | `
    Where-Object { 
            # take assemblies that are not analyzer resources. Also exclude Accessibility.dll as it is explicitly added to WindowsDesktop bundle.
            ($_.target.StartsWith('lib\') -or $_.target.StartsWith('ref\') -or $_.target.StartsWith('sdk\analyzers\'))`
                -and $_.target.EndsWith('.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('\Accessibility.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and $_.target -notmatch "..*Analyzers..*.resources.dll"
        } | `
    Select-Object -Unique @{Name="Path";Expression={Split-Path $_.target -Leaf}} | `
    Select-Object -ExpandProperty Path;

$needGenerate = $null;
[bool]::TryParse($GenerateManifest, [ref]$needGenerate) | Out-Null;
$servicingRelease = $null;
[bool]::TryParse($IsServicingRelease, [ref]$servicingRelease) | Out-Null;


if (!$needGenerate) {
    <#
        Compare the list of assemblies in the nuspec file against the list of assemblies
        in the existing manifest.
        If they differ - fail the build to force the developer to ack the difference
        and update the manifest.
    #>
    Write-Host "Comparing manifest" -ForegroundColor Yellow;
    [xml] $xmlDoc = Get-Content -Path $TargetFile -Force;

    $knownAssemblies = @( );
    $xmlDoc.Project.ItemGroup.FrameworkListFileClass.Include | `
        ForEach-Object { $knownAssemblies += $_ }

    $result = Compare-Object -ReferenceObject $knownAssemblies -DifferenceObject $assemblies;
    if ($null -ne $result) {
        $output = $result | Out-String;
        throw "The transport package manifest changed. Follow the README to update the manifest`r`n$output"
        exit -1;
    }
}
else {
    <#
        Update the existing manifest
    #>
    Write-Host "Regenerating the manifest" -ForegroundColor Green

    $sorted = $assemblies | Sort-Object;

    $output = "<!--
    This props file comes from dotnet/winforms. It gets ingested by dotnet/windowsdesktop and processed by
    src\windowsdesktop\src\sfx\Microsoft.WindowsDesktop.App.Ref.sfxproj and src\windowsdesktop\src\sfx\Microsoft.WindowsDesktop.App.Runtime.sfxproj.
-->
<Project>
  <!-- File classifications that should be included for both the ref and runtime pack. -->
  <ItemGroup>`r`n";
    $sorted | `
        ForEach-Object {
            $assembly = $_;
            if (!$assembly.Contains("Analyzers") `
                -and !$assembly.EndsWith("resources.dll")) {
                if ($assembly.Equals("System.Private.Windows.Core.dll")) {
                    $output += "    <!-- System.Private.Windows.Core is now used by both WPF and Windows Forms -->`r`n    <FrameworkListFileClass Include=`"$assembly`" Profile=`"WindowsForms;WPF`" />`r`n"
                }
                else {
                    $output += "    <FrameworkListFileClass Include=`"$assembly`" Profile=`"WindowsForms`" />`r`n"
                }
            }
        }
    $output += "  </ItemGroup>
        
  <!-- File classifications that should only be included for the ref pack. -->
  <ItemGroup Condition=`"'`$(PackageTargetRuntime)' == ''`">`r`n";
    $sorted | `
        ForEach-Object {
            $assembly = $_;
            if ($assembly.Contains("Analyzers")) {
                $output += "    <FrameworkListFileClass Include=`"$assembly`" Profile=`"WindowsForms`" />`r`n"
            }
        }
    $output += "  </ItemGroup>

  <!-- File classifications that should only be included for the runtime pack. Note analyzers should not be included in the runtime pack. -->
  <ItemGroup Condition=`"'`$(PackageTargetRuntime)' != ''`">`r`n";
    $sorted | `
        ForEach-Object {
            $assembly = $_;
            if ($assembly.EndsWith("resources.dll")) {
                $output += "    <FrameworkListFileClass Include=`"$assembly`" Profile=`"WindowsForms`" />`r`n"
            }
        }
    $output += "  </ItemGroup>
</Project>";
    $output | Out-File -FilePath $TargetFile -Encoding utf8 -Force;
}


# 
# Verify that components that are exposed as references in the targeting packs don't have their versions revved.
# See https://github.com/dotnet/winforms/pull/6667 for more details.
[xml] $xmlDoc = Get-Content -Path $NuspecFile -Force;

# Iterate over files that MUST NOT have their versions revved with every release
$nonRevAssemblies = $xmlDoc.package.files.file | `
    Where-Object { 
            ($_.target.StartsWith('lib\') -or $_.target.StartsWith('ref\')) `
                -and $_.target.EndsWith('.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('resources.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('\Accessibility.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('\Microsoft.VisualBasic.dll', [System.StringComparison]::OrdinalIgnoreCase)
        } | `
    Select-Object -Unique src | `
    Select-Object -ExpandProperty src;

$nonRevAssemblies | `
    sort-object | `
    foreach-object {
        $assembly = $_;
        [string] $version = ([Reflection.AssemblyName]::GetAssemblyName($assembly).Version).ToString()

        Write-Host "$assembly`: $version"
        if (![string]::Equals($version, $ExpectedAssemblyVersion)) {
            throw "$assembly is not versioned correctly. Expected: '$ExpectedAssemblyVersion', found: '$version'."
            exit -1;
        }
    }

# Iterate over files that MUST have their versions revved with every release
$revAssemblies = $xmlDoc.package.files.file | `
    Where-Object { 
            $_.target.StartsWith('sdk\analyzers\') `
                -and $_.target.EndsWith('.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('resources.dll', [System.StringComparison]::OrdinalIgnoreCase)
        } | `
    Select-Object -Unique src | `
    Select-Object -ExpandProperty src;

$revAssemblies | `
    sort-object | `
    foreach-object {
        $assembly = $_;
        [string] $version = ([Reflection.AssemblyName]::GetAssemblyName($assembly).Version).ToString()

        Write-Host "$assembly`: $version"
        if ($servicingRelease -and [string]::Equals($version, $ExpectedAssemblyVersion)) {
            throw "$assembly is not versioned correctly. '$version' is not expected."
            exit -1;
        }
    }
