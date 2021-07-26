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


[xml] $xmlDoc = Get-Content -Path $NuspecFile -Force;

$assemblies = $xmlDoc.package.files.file | `
    Where-Object { 
            # take only assemblies placed in \lib\netcoreappX.Y, and that are not resources
            # also exclude Accessibility.dll as it is explicitly added to WindowsDesktop bundle
            ($_.target.StartsWith('lib\') -or $_.target.StartsWith('ref\') -or $_.target.StartsWith('sdk\analyzers\'))`
                -and $_.target.EndsWith('.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('resources.dll', [System.StringComparison]::OrdinalIgnoreCase) `
                -and !$_.target.EndsWith('\Accessibility.dll', [System.StringComparison]::OrdinalIgnoreCase)
        } | `
    Select-Object -Unique @{Name="Path";Expression={Split-Path $_.target -Leaf}} | `
    Select-Object  -ExpandProperty Path;

# this isn't explicitly present in the list
$assemblies += 'System.Drawing.Common.dll';


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

    $output = "<!--
    This props file comes from dotnet/winforms. It gets ingested by dotnet/windowsdesktop and processed by
    pkg\windowsdesktop\sfx\Microsoft.WindowsDesktop.App.Ref.sfxproj.
   -->
<Project>
  <ItemGroup Condition=`"'`$(PackageTargetRuntime)' == ''`">`r`n";
    $assemblies | `
        Sort-Object | `
        ForEach-Object {
            $assembly = $_;
            $output += "    <FrameworkListFileClass Include=`"$assembly`" Profile=`"WindowsForms`" />`r`n"
        }
    $output += "  </ItemGroup>
</Project>";
    $output | Out-File -FilePath $TargetFile -Encoding utf8 -Force;
}
