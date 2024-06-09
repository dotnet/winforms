; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### Removed Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
WFAC001 | ApplicationConfiguration | Error | ApplicationConfigurationGenerator, [Documentation](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms.Analyzers.CSharp/ApplicationConfigurationGenerator.Help.md)
WFAC002 | ApplicationConfiguration | Error | ApplicationConfigurationGenerator, [Documentation](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms.Analyzers.CSharp/ApplicationConfigurationGenerator.Help.md)

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
WFCA001 | ApplicationConfiguration | Error | CSharpDiagnosticDescriptors
WFCA002 | ApplicationConfiguration | Error | CSharpDiagnosticDescriptors
WFCA100 | WinForms Security | Error | ControlPropertySerializationDiagnosticAnalyzer
