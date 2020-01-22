[CmdletBinding(PositionalBinding=$false)]
Param(
  [Parameter(Mandatory=$True, Position=1)]
  [string] $NuspecFile,
  [Parameter(Mandatory=$True, Position=2)]
  [string] $TargetFile,
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

