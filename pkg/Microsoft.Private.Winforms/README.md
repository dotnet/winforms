## Overview

This is a transport package consumed by [WPF](https://github.com/dotnet/wpf/) and [WindowsDesktop](https://github.com/dotnet/windowsdesktop/). Some packaging configurations are defined in this project, and others are defined in \<repo root\>\eng\packageContent.targets.


## `sdk\dotnet-windowsdesktop` folder

This folder contains props and targets used to ingest our assemblies into the [Windows Desktop SDK](https://github.com/dotnet/windowsdesktop/), bundling the correct set of assemblies into either the Microsoft.WindowsDesktop.App.Ref pack or Microsoft.WindowsDesktop.App.Runtime pack.

* [`System.Windows.Forms.FileClassification.props`](sdk\dotnet-windowsdesktop\System.Windows.Forms.FileClassification.props) contains a manifest for the "WindowsForms" SDK[&#x00B9;](#ref1), i.e. a list of our assemblies that form it.
The file is imported by [Microsoft.WindowsDesktop.App.Ref project](https://github.com/dotnet/windowsdesktop/blob/main/src/windowsdesktop/src/sfx/Microsoft.WindowsDesktop.App.Ref.sfxproj).<br/>
The manifest will need to be rebuilt if there are changes in the solution with respect to the SDK assemblies, e.g. a new assembly is added or an existing assembly is removed.

### How to update System.Windows.Forms.FileClassification.props

The existing `System.Windows.Forms.FileClassification.props` is be compared against the list of assemblies in `Microsoft.Private.Winforms.[version].nuspec`[&#x00B2;](#ref2) generated as part of the `pack` command.

If the content of these files differ - the build will fail.

:warning: The process is purposefully made manual to ensure changes in the manifest are made consciously.

To update the manifest run the following command and check in the updated files manifest:

```
.\build.cmd -pack /p:_GenerateManifest=true
```

To debug the script run the following command:

```
dotnet build .\pkg\Microsoft.Private.Winforms\Microsoft.Private.Winforms.csproj /t:UpdateTransportPackage /p:_GenerateManifest=true /v:m /bl /p:CommonLibrary_NativeInstallDir=$env:UserProfile\.netcoreeng\native\
```




## `sdk\dotnet-wpf` folder

This folder contains props and targets that are part of [Windows Desktop SDK](https://github.com/dotnet/wpf/blob/main/packaging/Microsoft.NET.Sdk.WindowsDesktop/) (which is hosted and assembled in [dotnet/wpf](https://github.com/dotnet/wpf/)).
These files are referenced  the [Microsoft.NET.Sdk.WindowsDesktop project](https://github.com/dotnet/wpf/blob/main/packaging/Microsoft.NET.Sdk.WindowsDesktop/Microsoft.NET.Sdk.WindowsDesktop.ArchNeutral.csproj)'s props and targets located [here](https://github.com/dotnet/wpf/blob/main/packaging/Microsoft.NET.Sdk.WindowsDesktop/targets). When this project is being built, it copies the files from our transport NuGet package to a Microsoft.NET.Sdk.WindowsDesktop bundle.

* [`Microsoft.NET.Sdk.WindowsDesktop.WindowsForms.props`](sdk\dotnet-wpf\Microsoft.NET.Sdk.WindowsDesktop.WindowsForms.props) contains various Windows Forms specific configurations, such as  our default `using` imports.

* [`Microsoft.NET.Sdk.WindowsDesktop.WindowsForms.targets`](sdk\dotnet-wpfp\Microsoft.NET.Sdk.WindowsDesktop.WindowsForms.targets) contains various Windows Forms specific targets.

* [`System.Windows.Forms.Analyzers.props`](sdk\dotnet-wpf\System.Windows.Forms.Analyzers.props) contains a list of properties required by our source generators.


<br/>
<br/>

----

<a name="ref1"></a>1. Except for `Accessibility.dll` which is [noted explicitly](https://github.com/dotnet/windowsdesktop/blob/f497549de5c2dba4d296c3311de71e12db808c65/pkg/windowsdesktop/sfx/Microsoft.WindowsDesktop.App.Ref.sfxproj#L18).

<a name="ref2"></a>2. Generated in .\winforms\artifacts\obj\Microsoft.Private.Winforms\\[Debug|Release].
