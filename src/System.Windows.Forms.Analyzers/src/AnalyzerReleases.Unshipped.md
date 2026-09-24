; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
WFO2000 | WinForms Usage | Warning | Flags fresh instances returned by property getters (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo2000)
WFO3000 | WinForms Designer | Error | Rejects unsupported InitializeComponent code (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3000)
WFO3001 | WinForms Designer | Error | Keeps custom members out of Designer files (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3001)
WFO3002 | WinForms Designer | Warning | Keeps generated fields at the end (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3002)
WFO3003 | WinForms Designer | Error | Keeps events and delegates out of Designer files (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3003)
WFO3004 | WinForms Designer | Error | Rejects collection expressions in Designer files (C#), [Documentation](https://aka.ms/winforms-warnings/wfo3004)
WFO3005 | WinForms Designer | Warning | Rejects nameof expressions in InitializeComponent (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3005)
WFO3006 | WinForms Designer | Error | Rejects conditional expressions in InitializeComponent (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3006)
WFO3007 | WinForms Designer | Error | Rejects null-coalescing in InitializeComponent (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3007)
WFO3008 | WinForms Designer | Error | Rejects null-conditional access in InitializeComponent (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3008)
WFO3009 | WinForms Designer | Warning | Rejects interpolated strings in InitializeComponent (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3009)
WFO3010 | WinForms Designer | Error | Rejects anonymous functions in InitializeComponent (C#, Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3010)
WFO3011 | WinForms Designer | Error | Requires WithEvents on Designer component members (Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3011)
WFO3012 | WinForms Designer | Error | Requires Handles for Designer member events (Visual Basic), [Documentation](https://aka.ms/winforms-warnings/wfo3012)
