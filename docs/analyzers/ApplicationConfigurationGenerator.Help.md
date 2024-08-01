# How to use System.Windows.Forms.Analyzers.CSharp

System.Windows.Forms.Analyzers.CSharp analyzers and source generators are shipped inbox with Windows Desktop .NET SDK, and
are automatically referenced for Window Forms .NET applications.

## `ApplicationConfigurationGenerator`

`ApplicationConfigurationGenerator` is automatically invoked when `ApplicationConfiguration.Initialize()` call is detected,
and it emits the application bootstrap code such as:
```cs
public static void Initialize()
{
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    Application.SetHighDpiMode(HighDpiMode.SystemAware);
}
```

For more information on application configuration refer to https://aka.ms/applicationconfiguration.


## [WFO0001](https://aka.ms/winforms-warnings/WFAC001): Unsupported project type.

Only projects with `OutputType` set to "Exe" or "WinExe" are supported, because only applications projects define an application entry point,
where the application bootstrap code must reside.

|Item|Value|
|-|-|
| Category | ApplicationConfiguration |
| Enabled | True |
| Severity | Error |
| CodeFix | False |
---


## [WFO0002](https://aka.ms/winforms-warnings/WFAC002): Unsupported property value.

The specified project property cannot be set to the given value.

|Item|Value|
|-|-|
| Category | ApplicationConfiguration |
| Enabled | True |
| Severity | Error |
| CodeFix | False |
---