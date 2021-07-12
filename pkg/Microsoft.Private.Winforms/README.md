## Overview

This is a transport package consumed by [WPF](https://github.com/dotnet/wpf/) and [WindowsDesktop](https://github.com/dotnet/windowsdesktop/).


## `sdk\dotnet-windowsdesktop` folder

This folder contains props and targets used to ingest our assemblies into the [Windows Desktop SDK](https://github.com/dotnet/windowsdesktop/) for purpose of bundling of our analyzers into Microsoft.WindowsDesktop.App.Ref pack.

* [`System.Windows.Forms.Analyzers.props`](sdk\dotnet-windowsdesktop\System.Windows.Forms.Analyzers.props) contains a manifest for the "WindowsForms" SDK[&#x00B9;](#ref1), i.e. a list of our assemblies that form it.
The file is imported by [Microsoft.WindowsDesktop.App.Ref project](https://github.com/dotnet/windowsdesktop/blob/main/pkg/windowsdesktop/sfx/Microsoft.WindowsDesktop.App.Ref.sfxproj).<br/>
The manifest will need to be rebuilt if there are changes in the solution with respect to the SDK assemblies, e.g. a new assembly is added or an existing assembly is removed.

* [`System.Windows.Forms.Analyzers.targets`](sdk\dotnet-windowsdesktop\System.Windows.Forms.Analyzers.targets) contains targets that help packaging our analyzers into the Microsoft.WindowsDesktop.App.Ref pack. The file is imported by [Microsoft.WindowsDesktop.App.Ref project](https://github.com/dotnet/windowsdesktop/blob/main/pkg/windowsdesktop/sfx/Microsoft.WindowsDesktop.App.Ref.sfxproj).



### How to update System.Windows.Forms.Analyzers.props

The existing `System.Windows.Forms.Analyzers.props` is be compared against the list of assemblies in `Microsoft.Private.Winforms.[version].nuspec`[&#x00B2;](#ref2) generated as part of the `pack` command.

If the content of these files differ - the build will fail.

:warning: The process is purposefully made manual to ensure changes in the manifest are made consciously.

To update the manifest

* Build the solution as normal:
    ```
    .\build.cmd
    ```
* To update the manifest [`System.Windows.Forms.Analyzers.props`](sdk\dotnet-windowsdesktop\System.Windows.Forms.Analyzers.props) run the following command from a developer prompt:
    ```
    msbuild .\pkg\Microsoft.Private.Winforms\Microsoft.Private.Winforms.csproj /t:UpdateTransportPackage /p:GenerateManifest=true /v:m
    ```

## `sdk\dotnet-wpf` folder

This folder contains props for our analyzers and targets that reference our analyzers into a consumer Windows Forms application.

* [`System.Windows.Forms.Analyzers.props`](sdk\dotnet-wpf\System.Windows.Forms.Analyzers.props) contains the list of properties required by our source generators.

* [`System.Windows.Forms.Analyzers.targets`](sdk\dotnet-wpfp\System.Windows.Forms.Analyzers.targets) contains targets that reference our analyzers into comsumer projects.

These files are ingested by the WPF via [Microsoft.NET.Sdk.WindowsDesktop project](https://github.com/dotnet/wpf/blob/main/packaging/Microsoft.NET.Sdk.WindowsDesktop/Microsoft.NET.Sdk.WindowsDesktop.ArchNeutral.csproj). When this project is being built, it copies the files from our transport NuGet package to a Microsoft.NET.Sdk.WindowsDesktop bundle, and thus facilitates the seamless refernce of our analyzers by Windows Forms apps.




<br/>
<br/>

----

<a name="ref1"></a>1. Except for `Accessibility.dll` which is [noted explicitly](https://github.com/dotnet/windowsdesktop/blob/f497549de5c2dba4d296c3311de71e12db808c65/pkg/windowsdesktop/sfx/Microsoft.WindowsDesktop.App.Ref.sfxproj#L18).

<a name="ref2"></a>2. Generated in .\winforms\artifacts\obj\Microsoft.Private.Winforms\\[Debug|Release].
