# How to use System.Windows.Forms.Analyzers

System.Windows.Forms.Analyzers analyzers and source generators are shipped inbox with Windows Desktop .NET SDK, and
are automatically referenced for Window Forms .NET applications.

## `AppManifestAnalyzer`

`AppManifestAnalyzer` is automatically invoked when a Windows Forms application (`OutputType=Exe` or `OutputType=WinExe`) has a custom app.manifest.

## [WFO0003](https://aka.ms/winforms-warnings/wfo0003): Unsupported high DPI configuration.

Windows Forms applications should specify application DPI-awareness via the [application configuration](https://aka.ms/applicationconfiguration) or
[`Application.SetHighDpiMode` API](https://learn.microsoft.com/dotnet/api/system.windows.forms.application.sethighdpimode).

In NET6.0 the same warning was shipped with ID `WFAC010`.

| Item      | Value                    |
|-----------|--------------------------|
| Category  | ApplicationConfiguration |
| Enabled   | True                     |
| Severity  | Warning                  |
| CodeFix   | False                    |
| Changed in| NET9.0                   |

---
