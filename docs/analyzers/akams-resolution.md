# aka.ms link resolution for WinForms analyzer diagnostics

This file lists the `aka.ms` links which are emitted by WinForms analyzers, source generators, and obsoletions
(via `DiagnosticIDs.UrlFormat` / `Obsoletions.SharedUrlFormat`, `https://aka.ms/winforms-warnings/{id}`) or which are
referenced from the analyzer documentation, together with where they currently resolve to.

Last verified: 2026-11-13 (following all redirects with `curl -L`).

Mappings are maintained in the [redirection manager](https://akalinkmanager.trafficmanager.net/am/redirection/home?options=host:aka.ms);
see also [akams.md](akams.md).

## Links which need to be created or updated

| Short URL | Action | Current target | Proposed target |
|-----------|--------|----------------|-----------------|
| https://aka.ms/winforms-warnings/wfo2000 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo2000 |
| https://aka.ms/winforms-warnings/wfo3000 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3000 |
| https://aka.ms/winforms-warnings/wfo3001 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3001 |
| https://aka.ms/winforms-warnings/wfo3002 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3002 |
| https://aka.ms/winforms-warnings/wfo3003 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3003 |
| https://aka.ms/winforms-warnings/wfo3004 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3004 |
| https://aka.ms/winforms-warnings/wfo3005 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3005 |
| https://aka.ms/winforms-warnings/wfo3006 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3006 |
| https://aka.ms/winforms-warnings/wfo3007 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3007 |
| https://aka.ms/winforms-warnings/wfo3008 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3008 |
| https://aka.ms/winforms-warnings/wfo3009 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3009 |
| https://aka.ms/winforms-warnings/wfo3010 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#wfo3010 |
| https://aka.ms/winforms-warnings/wfo3011 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsVisualBasicAnalyzers.Help.md#wfo3011 |
| https://aka.ms/winforms-warnings/wfo3012 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsVisualBasicAnalyzers.Help.md#wfo3012 |
| https://aka.ms/winforms-warnings/wfo5003 | Create | _not registered (falls back to Bing search)_ | https://github.com/dotnet/winforms/blob/main/docs/analyzers/Experimental.Help.md (no Learn page exists yet) |
| https://aka.ms/winforms-warnings/wfac002 | Update | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo0002 | https://learn.microsoft.com/dotnet/desktop/winforms/wfdev-diagnostics/wfac002 (exists; matches `wfac001`/`wfac010`) |
| https://aka.ms/winforms-warnings/wfo1001 | Update (recommended) | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md#implementitypeddataobject | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo1001 (exists; matches the other `wfo1xxx`/`wfo2001` links) |

Notes:

* The `#wfo2000` and `#wfo3000`–`#wfo3012` anchors are explicit `<a id="...">` anchors in the help documents. They
  resolve only once the updated `WinFormsCSharpAnalyzers.Help.md` / `WinFormsVisualBasicAnalyzers.Help.md` are merged
  into `main`.
* WFO2000 and WFO3000–WFO3010 are reported for both C# and Visual Basic, but one short URL can only have one target.
  The C# document is proposed as the target; each rule section there links to the Visual Basic document where the
  rules differ. Once Learn pages exist under `https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/`,
  all of these links should be re-pointed there, consistent with WFO0001–WFO2001 and WFO5001/WFO5002.

## Links which resolve correctly

| Short URL | Resolves to |
|-----------|-------------|
| https://aka.ms/winforms-warnings/wfac001 | https://learn.microsoft.com/dotnet/desktop/winforms/wfdev-diagnostics/wfac001 |
| https://aka.ms/winforms-warnings/wfac010 | https://learn.microsoft.com/dotnet/desktop/winforms/wfdev-diagnostics/wfac010 |
| https://aka.ms/winforms-warnings/wfo0001 | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo0001 |
| https://aka.ms/winforms-warnings/wfo0002 | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo0002 |
| https://aka.ms/winforms-warnings/wfo0003 | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo0003 |
| https://aka.ms/winforms-warnings/wfo1000 | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo1000 |
| https://aka.ms/winforms-warnings/wfo2001 | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo2001 |
| https://aka.ms/winforms-warnings/wfo5001 | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo5001 |
| https://aka.ms/winforms-warnings/wfo5002 | https://learn.microsoft.com/dotnet/desktop/winforms/compiler-messages/wfo5002 |
| https://aka.ms/winforms-warnings/wfdev001 – wfdev006 | https://learn.microsoft.com/dotnet/desktop/winforms/wfdev-diagnostics/wfdev001 – wfdev006 |
| https://aka.ms/applicationconfiguration | https://learn.microsoft.com/dotnet/desktop/winforms/whats-new/net60?view=netdesktop-6.0#new-application-bootstrap |
| https://aka.ms/WfoGenManifest | https://github.com/dotnet/winforms/blob/main/docs/analyzers/AppManifestAnalyzer.Help.md |
| https://aka.ms/WfoExperimental | https://github.com/dotnet/winforms/blob/main/docs/analyzers/Experimental.Help.md |
| https://aka.ms/WfoCsAppConfig | https://github.com/dotnet/winforms/blob/main/docs/analyzers/ApplicationConfigurationGenerator.Help.md |
| https://aka.ms/WfoCsAnalyzers | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsCSharpAnalyzers.Help.md |
| https://aka.ms/WfoVbAnalyzers | https://github.com/dotnet/winforms/blob/main/docs/analyzers/WinFormsVisualBasicAnalyzers.Help.md |
