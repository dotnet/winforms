# Windows Forms Building Guidelines

## Building from the command line
* To build the source, run ```.\build``` from the repo root.
 * This will build the System.Windows.Forms.sln using the default config (Debug|Any CPU)
* To specify the config, add -configuration followed by the config
 * For example, ```.\build -configuration Release```

If your build is successful, you should see something like this:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Note that this does **not** build using your machine-wide installed version of the dotnet sdk. It builds using the dotnet sdk specified in the global.json in the repo root.

## Building from Visual Studio
* To build from Visual Studio, open System.Windows.Forms.sln in Visual Studio and build how you normally would.
Note that VS behaves slightly differently than the command line. **Therefore, we highly recommend building from the command line.**

## Troubleshooting build errors
* Most build errors are normal compile errors and can be dealt with accordingly.
* Other times, the error occurs elsewhere in MSBuild, and you need to examine the logs.
 * The logs are generated at log\$(BuildConfig)\Build.binlog
 * The file format is an MSBuild Binary Log. Install the [MSBuild Structured Log Viewer](http://msbuildlog.com/) to read them.