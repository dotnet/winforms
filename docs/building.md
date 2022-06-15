# Building Windows Forms

## Prerequisites

Follow the prerequisites listed at [Developer Guide](developer-guide.md).

## Building

### Building from command line (Preferred)

* Run `.\build.cmd` from the repository root. This builds the `Winforms.sln` using the default config (Debug|Any CPU).
* To specify a build configuration, add `-configuration` followed by the config such as `.\build -configuration Release`.

Note that this does **not** build using your machine-wide installed version of the dotnet sdk. It builds using the repo-local .NET SDK specified in the global.json in the repository root.

### Building from Visual Studio

1. .NET 6.0 and above branches need VisualStudio 2022 to build.
2. Run `.\restore.cmd` from the repository root.
3. Run `.\start-vs.cmd` from the repository root. This will prepend the repo-local .NET SDK to the path, and open `Winforms.sln` in Visual Studio.
4. You should now be able to build as you normally would.

### Building from Visual Studio Code

1. (Optional) Run `.\restore.cmd` from the repository root.
1. Run `.\start-code.cmd` from the repository root. This will prepend the repo-local .NET SDK to the path, and open `Winforms.sln` in Visual Studio Code.
1. You should now be able to build and test as you normally would from command line (or VS Code, if you have set it).

## Build outputs

* All build outputs are generated under the `artifacts` folder.
* Binaries are under `artifacts\bin`.
  * For example, `System.Windows.Forms.dll` can be found under `artifacts\bin\System.Windows.Forms\Debug\net6.0`.
* Logs are found under `artifacts\log`.
* Packages are found under `artifacts\packages`.

## Running apps from VS

1. Right click on the project you wish to run and select 'Set as startup project'.
2. <kbd>F5</kbd>.

## Running apps from command line

1. Build the solution: `.\build.cmd`
2. Navigate to the project you wish to run, e.g. to run AccessibilityTests test app:
    ```
    winforms> pushd .\src\System.Windows.Forms\tests\AccessibilityTests
    winforms\src\System.Windows.Forms\tests\AccessibilityTests> dotnet run
    ```
3. (Alternatively) Navigate to `.\artifacts\bin\AccessibilityTests\Debug\net6.0` and run the app manually.

## Troubleshooting build errors

* Most build errors are compile errors and can be dealt with accordingly.
* Other error may be from MSBuild tasks. You need to examine the build logs to investigate.
  * The logs are generated at `.\artifacts\log\Debug\Build.binlog`
  * The file format is an MSBuild Binary Log. Install the [MSBuild Structured Log Viewer][msbuild-log-viewer] to view them.
* Windows Forms uses Visual Studio MSBuild but for certain features we require the latest MSBuild from .NET Core/.NET SDK. If you are on an official version of [Visual Studio][VS-download] (i.e. not a Preview version), then you may need to enable previews for .NET Core/.NET SDKs in VS.
  * you can do this in VS under Tools->Options->Environment->Preview Features->Use previews of the .Net Core SDK (Requires restart)

## Creating a package

To create the Microsoft.Private.Winforms package, run `.\build -pack`

## Localization

If you need to add a new localization string or update an existing one, follow these steps:

- Modify `Resource\SR.resx` file adding or updating necessary strings in the project that contains the said strings. (It is often faster/easier to open SR.resx in an XML editor).
- Regenerate the localization files by rebuilding the solution/project from Visual Studio, or by executing `.\build.cmd` command. You can also build just the modified project by running  `dotnet build` from the project's root.

[comment]: <> (URI Links)

[corefx-windows-instructions]: https://github.com/dotnet/corefx/blob/master/Documentation/building/windows-instructions.md
[latest-core-build]: https://github.com/dotnet/core/blob/master/daily-builds.md
[msbuild-log-viewer]: https://msbuildlog.com/
[VS-download]: https://visualstudio.microsoft.com/downloads/
