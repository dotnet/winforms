## Overview

This is a transport package consumed by [WPF](https://github.com/dotnet/wpf/) and [WindowsDesktop](https://github.com/dotnet/windowsdesktop/).

WindowsDesktop relies on [`FrameworkListFiles.props` manifest](FrameworkListFiles.props) to list all our assemblies that form "WindowsForms" SDK[&#x00B9;](#ref1).
The props file is then imported by [WindowsDesktop projects](https://github.com/dotnet/windowsdesktop/blob/master/pkg/windowsdesktop/pkg/Directory.Build.props).

The manifest will need to be rebuilt if there are changes in the solution with respect to the SDK assemblies, e.g. a new assembly is added or an existing assembly is removed.

## How it works

[`FrameworkListFiles.props`](FrameworkListFiles.props) will be compared against the list of assemblies in `Microsoft.Private.Winforms.[version].nuspec`[&#x00B2;](#ref2) generated as part of the `pack` command.

If the content of these files differ - the build will fail.

:warning: The process is purposefully made manual to ensure changes in the manifest are made consciously.

## How to update the manifest

* Build the solution as normal:
    ```
    .\build.cmd
    ```
* To update the manifest [`FrameworkListFiles.props`](FrameworkListFiles.props) run the following command from a developer prompt:
    ```
    msbuild .\pkg\Microsoft.Private.Winforms\Microsoft.Private.Winforms.csproj /t:UpdateTransportPackage /p:GenerateManifest=true /v:m
    ```




----

<a name="ref1"></a>1. Except for `Accessibility.dll` which is [noted explicitly](https://github.com/dotnet/windowsdesktop/blob/0d4ab955ce71069f5f1e035d448a7999c4311e0c/pkg/windowsdesktop/pkg/Directory.Build.props#L28).

<a name="ref2"></a>2. Generated in .\winforms\artifacts\obj\Microsoft.Private.Winforms\\[Debug|Release].
