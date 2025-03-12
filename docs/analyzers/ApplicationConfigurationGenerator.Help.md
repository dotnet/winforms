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

## [WFO0001](https://aka.ms/winforms-warnings/wfo0001): Unsupported project type.

Only projects with `OutputType` set to "Exe" or "WinExe" are supported, because only applications projects define an application entry point,
where the application bootstrap code must reside.

In NET6.0 the same error was shipped with ID `WFAC001`.

| Item      | Value                      |
|-----------|----------------------------|
| Category  | ApplicationConfiguration   |
| Enabled   | True                       |
| Severity  | Error                      |
| CodeFix   | False                      |
| Changed in| NET9.0                     |

---

## [WFO0002](https://aka.ms/winforms-warnings/wfo0002): Unsupported property value.

The specified project property cannot be set to the given value.
In NET6.0 the same error was shipped with ID `WFAC002`.

| Item      | Value                      |
|-----------|----------------------------|
| Category  | ApplicationConfiguration   |
| Enabled   | True                       |
| Severity  | Error                      |
| CodeFix   | False                      |
| Changed in| NET9.0                     |

---