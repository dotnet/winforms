# Building Windows Forms

## Prerequisites

Follow the prerequisites listed at [Building CoreFX on Windows][corefx-windows-instructions]

## Building

### Building from command line

* Run `.\build.cmd` from the repository root. This builds the `Winforms.sln` using the default config (Debug|Any CPU).
* To specify a build configuration, add `-configuration` followed by the config such as `.\build -configuration Release`.

Note that this does **not** build using your machine-wide installed version of the dotnet sdk. It builds using the repo-local .NET SDK specified in the global.json in the repository root.

### Building from Visual Studio

1. Run `.\restore.cmd` from the repository root.
1. Run `.\start-vs.cmd` from the repository root. This will prepend the repo-local .NET SDK to the path, and open `Winforms.sln` in Visual Studio.
1. You should now be able to build as you normally would.

### Building from Visual Studio Code

1. (Optional) Run `.\restore.cmd` from the repository root.
1. Run `.\start-code.cmd` from the repository root. This will prepend the repo-local .NET SDK to the path, and open `Winforms.sln` in Visual Studio.
1. You should now be able to build and test as you normally would from command line.

## Build outputs

* All build outputs are generated under the `artifacts` folder.
* Binaries are under `artifacts\bin`.
  * For example, `System.Windows.Forms.dll` can be found under `artifacts\bin\System.Windows.Forms\Debug\netcoreapp5.0`.
* Logs are found under `artifacts\log`.
* Packages are found under `artifacts\packages`.

## Running apps from VS

1. Right click on the project you wish to run and select 'Set as startup project'.
2. <kbd>F5</kbd>.

## Running apps from command line

1. Build the solution: `.\build.cmd`
2. Navigate to the project you wish to run, e.g. to run AccessibilityTests test app:
    ```
    winforms> pushd .\src\System.Windows.Forms\tests\AccessibilityTests>
    winforms\src\System.Windows.Forms\tests\AccessibilityTests> dotnet run
    ```
3. (Alternatively) Navigate to .\artifacts\bin\AccessibilityTests\Debug\netcoreapp5.0 and run the app manually.

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
