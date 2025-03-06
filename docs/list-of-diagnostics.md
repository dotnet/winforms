# List of Diagnostics Produced by Windows Forms .NET

## Obsoletions

Per the [Better Obsoletion](https://github.com/dotnet/designs/blob/main/accepted/2020/better-obsoletion/better-obsoletion.md) design and similar to the [runtime diagnostics](https://github.com/dotnet/runtime/blob/main/docs/project/list-of-diagnostics.md), we now have a strategy for marking existing APIs as `[Obsolete]`. This takes advantage of the new diagnostic ID and URL template mechanisms introduced to `ObsoleteAttribute` in .NET 5.

The diagnostic ID values reserved for obsoletions are `WFDEV001` through `WFDEV999`. When obsoleting an API, claim the next three-digit identifier in the `WFDEV###` sequence and add it to the list below. The URL template for all obsoletions is `https://aka.ms/winforms-warnings/{0}`. The `{0}` placeholder is replaced by the compiler with the `WFDEV###` identifier.

### Acceptance Criteria for Adding an Obsoletion

1. **Add the obsoletion to the table below**, claiming the next diagnostic ID.
    - Ensure the description is meaningful within the context of this table, and without requiring the context of the calling code.
2. **Add new constants to `src\Common\src\Obsoletions.cs`**, following the existing conventions:
    - A `...Message` const using the same description added to the table below.
    - A `...DiagnosticId` const for the `WFDEV###` ID.
3. **If adding `<Obsolete>` attribute to Microsoft.VisualBasic.Forms assembly**, edit `src\Microsoft.VisualBasic.Forms\src\Obsoletions.vb` file.
4. **Annotate `src` files by referring to the constants defined from `Obsoletions.cs`**:
    - Specify the `UrlFormat = Obsoletions.SharedUrlFormat`.
    - Example: 
        ```C#
        [Obsolete(
            Obsoletions.DomainUpDownAccessibleObjectMessage,
            DiagnosticId = Obsoletions.DomainUpDownAccessibleObjectDiagnosticId,
            UrlFormat = Obsoletions.SharedUrlFormat)]
        ```
    - If the `Obsoletions` type is not available in the C# project, link it into the project:
        ```xml
        <Compile Include="..\..\Common\src\Obsoletions.cs" Link="Common\Obsoletions.cs" />
        ```
5. **Apply the `:book: documentation: breaking` label** to the PR that introduces the obsoletion.
6. **Document the breaking change**
    - In the breaking-change issue filed in [dotnet/docs](https://github.com/dotnet/docs), specifically mention that this breaking change is an obsoletion with a `WFDEV` diagnostic ID.
    - Create another issue to add warning documentation to the learn web site [here](https://github.com/dotnet/docs-desktop/issues/new?template=diagnostic-compiler.yml)
    - The documentation team will produce a PR that adds the obsoletion to the [WFDEV warnings](https://learn.microsoft.com/dotnet/desktop/winforms/wfdev-diagnostics/wfdev003) page and we will review it.
    - Once the docs PR is published, get the newly create link to the doc and associate it with the corresponding `aka.ms` name in [redirection manager](https://akalinkmanager.trafficmanager.net/am/redirection/home?options=host:aka.ms )
    - Connect with `@gewarren`, `@adegeo` or `@merriemcgaw` with any questions.

### Obsoletion Diagnostics (`WFDEV001` - `WFDEV999`)

| Diagnostic ID     | Description |
| :---------------- | :---------- |
| `WFDEV001` | Casting to/from IntPtr is unsafe, use `WParamInternal`. |
| `WFDEV001` | Casting to/from IntPtr is unsafe, use `LParamInternal`. |
| `WFDEV001` | Casting to/from IntPtr is unsafe, use `ResultInternal`. |
| `WFDEV002` | `DomainUpDown.DomainUpDownAccessibleObject` is no longer used to provide accessible support for `DomainUpDown` controls. Use `ControlAccessibleObject` instead. |
| `WFDEV003` | `DomainUpDown.DomainItemAccessibleObject` is no longer used to provide accessible support for `DomainUpDown` items. |
| `WFDEV004` | `Form.OnClosing`, `Form.OnClosed` and the corresponding events are obsolete. Use `Form.OnFormClosing`, `Form.OnFormClosed`, `Form.FormClosing` and `Form.FormClosed` instead. |
| `WFDEV005` | `Clipboard.GetData(string)` method is obsolete. Use `Clipboard.TryGetData<T>` methods instead. |
| `WFDEV005` | `DataObject.GetData` methods are obsolete. Use the corresponding `DataObject.TryGetData<T>` instead. |
| `WFDEV005` | `ClipboardProxy.GetData(As String)` method is obsolete. Use `ClipboardProxy.TryGetData(Of T)(As String, As T)` instead. |
| `WFDEV006` | `ContextMenu` is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code. Use `ContextMenuStrip` instead. |
| `WFDEV006` | `DataGrid` is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code. Use `DataGridView` instead. |
| `WFDEV006` | `MainMenu` is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code. Use `MenuStrip` instead. |
| `WFDEV006` | `Menu` is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code. Use `ToolStripDropDown` and `ToolStripDropDownMenu` instead. |
| `WFDEV006` | `StatusBar` is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code. Use `StatusStrip` instead. |
| `WFDEV006` | `ToolBar` is provided for binary compatibility with .NET Framework and is not intended to be used directly from your code. Use `ToolStrip` instead. |

## Analyzer Diagnostics in the order they were shipped.

The current IDs are defined in [DiagnosticIDs.cs](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms.Analyzers/src/System/Windows/Forms/Analyzers/Diagnostics/DiagnosticIDs.cs). A complete list of IDs, including those that were shipped and then replaced, is available in the `AnalyzerReleases.Shipped.md` and `AnalyzerReleases.Unshipped.md` files.

| Diagnostic ID     | Description |
| :---------------- | :---------- |
| `WFAC001` | Unsupported project type. |
| `WFAC002` | Unsupported property value. |
| `WFAC010` | Unsupported high DPI configuration. |
| `WFO0001` | Only projects with `OutputType=WindowsApplication` supported. |
| `WFO0002` | ArgumentException: Project property '{0}' cannot be set to '{1}'|
| `WFO0002` | ArgumentException: Project property '{0}' cannot be set to '{1}'. Reason: {2}.|
| `WFO0003` | Remove high DPI settings from {0} and configure via Application.SetHighDpiMode API or '{1}' project property.|
| `WFO0003` | Remove high DPI settings from {0} and configure via '{1}' property in Application Framework.|
| `WFO1000` | Property '{0}' does not configure the code serialization for its property content.|
| `WFO2001` | Task is being passed to `InvokeAsync` without a cancellation token |
| `WFO1001` | Type `{0}` does not implement `ITypedDataObject` interface|

## Experimental Feature compiler errors

| Diagnostic ID     | Description |
| :---------------- | :---------- |
| `WFO5001` | `System.Windows.Forms.Application.SetColorMode`(System.Windows.Forms.SystemColorMode) is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.|
| `WFO5001` | `System.Windows.Forms.SystemColorMode` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.|
| `WFO5002` | `System.Windows.Forms.Form.ShowAsync` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.|
