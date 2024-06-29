; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### Removed Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
WFAC001 | Application Configuration | Error | ApplicationConfigurationGenerator, [Documentation](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms.Analyzers.CSharp/ApplicationConfigurationGenerator.Help.md)
WFAC002 | Application Configuration | Error | ApplicationConfigurationGenerator, [Documentation](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms.Analyzers.CSharp/ApplicationConfigurationGenerator.Help.md)

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
WFCA001 | Application Configuration | Error | CSharpDiagnosticDescriptors
WFCA002 | Application Configuration | Error | CSharpDiagnosticDescriptors
WFCA100 | WinForms Security | Error | ControlPropertySerializationDiagnosticAnalyzer
