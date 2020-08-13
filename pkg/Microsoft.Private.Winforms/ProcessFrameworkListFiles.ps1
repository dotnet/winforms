[CmdletBinding(PositionalBinding=$false)]
Param(
  [Parameter(Mandatory=$True, Position=1)]
  [string] $NuspecFile,
  [Parameter(Mandatory=$True, Position=2)]
  [string] $TargetFile,
  [Parameter(Mandatory=$True, Position=3)]
  [string] $GenerateManifest,
  [Parameter(ValueFromRemainingArguments=$true)][String[]] $properties
)

$assemblies = @( );
# this isn't expicitly present in the list
$assemblies += 'System.Drawing.Common.dll';

[xml] $xmlDoc = Get-Content -Path $NuspecFile -Force;
$xmlDoc.package.files.file | `
    Where-Object { 
            # take only assemblies placed in \lib\netcoreappX.Y, and that are not resources
            # also exclude Accessibility.dll as it is explicitly added to WindowsDesktop bundle
            $_.target.StartsWith('lib\') `
                -and $_.target.EndsWith('.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('resources.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('\Accessibility.dll', [System.StringComparison]::OrdinalIgnoreCase)
        } | `
    ForEach-Object { 
        $assembly = Split-Path $_.target -Leaf;
        $assemblies += $assembly;
    };

$needGenerate = $null;
[bool]::TryParse($GenerateManifest, [ref]$needGenerate) | Out-Null;

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
    $output = "<Project>
      <ItemGroup Condition=`"'`$(PackageTargetRuntime)' == ''`">
    ";
    $assemblies | `
        Sort-Object | `
        ForEach-Object {
            $assembly = $_;
            $output += "    <FrameworkListFileClass Include=`"$assembly`" Profile=`"WindowsForms`" />
    "
        }
    $output += "  </ItemGroup>
    </Project>";
    $output | Out-File -FilePath $TargetFile -Encoding utf8 -Force;
}
