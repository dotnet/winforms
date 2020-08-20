# Building Windows Forms

## Prerequisites

Follow the prerequisites listed at [Building CoreFX on Windows][corefx-windows-instructions]

## Building from the command line

* Run `.\build` from the repository root.
  * Builds the `Winforms.sln` using the default config (Debug|Any CPU)
* To specify a config, add `-configuration` followed by the config such as `.\build -configuration Release`

If your build is successful, you should see something like this:

```console
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Note that this does **not** build using your machine-wide installed version of the dotnet sdk. It builds using the dotnet sdk specified in the global.json in the repository root.

## Building from Visual Studio

* To build from Visual Studio, open `Winforms.sln` in Visual Studio and build how you normally would.
* Visual Studio behaves slightly differently than the command line. It uses the machine-wide installed SDK instead of the one specified in the global.json.
  * Please make sure you have the [latest .Net Core Daily Build][latest-core-build] installed.

## Build outputs

* All build outputs are generated under the `artifacts` folder.
* Binaries are under `artifacts\bin`
  * For example, `System.Windows.Forms.dll` can be found under `artifacts\bin\System.Windows.Forms\Debug\netcoreapp5.0`
* Logs are found under `artifacts\log`
* Packages are found under `artifacts\packages`

## Troubleshooting build errors

* Most build errors are compile errors and can be dealt with accordingly.
* Other error may be from MSBuild tasks. You need to examine the build logs to investigate.
  * The logs are generated at `artifacts\log\Debug\Build.binlog`
  * The file format is an MSBuild Binary Log. Install the [MSBuild Structured Log Viewer][msbuild-log-viewer] to view them.
* WinForms uses Visual Studio MSBuild but sometimes with a preview .NET Core SDK; so if you have a non-preview version of [Visual Studio][VS-download] (i.e. a release version), then you may need to enable `use preview` for .NET Core SDKs in VS.
  * you can do this in VS at Tools :arrow_right: options :arrow_right: Projects and Solutions :arrow_right: .net core :arrow_right: use previews

## Creating a package

To create the Microsoft.Private.Winforms package, run `.\build -pack`

[comment]: <> (URI Links)

[corefx-windows-instructions]: https://github.com/dotnet/corefx/blob/master/Documentation/building/windows-instructions.md
[latest-core-build]: https://github.com/dotnet/core/blob/master/daily-builds.md
[msbuild-log-viewer]: http://msbuildlog.com/
[VS-download]: https://visualstudio.microsoft.com/downloads/
