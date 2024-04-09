# List of Diagnostics Produced by Windows Forms .NET APIs

## Obsoletions

Per https://github.com/dotnet/designs/blob/main/accepted/2020/better-obsoletion/better-obsoletion.md and similar to https://github.com/dotnet/runtime/blob/main/docs/project/list-of-diagnostics.md, we now have a strategy for marking existing APIs as `[Obsolete]`. This takes advantage of the new diagnostic id and URL template mechanisms introduced to `ObsoleteAttribute` in .NET 5.

The diagnostic id values reserved for obsoletions are `WFDEV001` through `WFDEV999`. When obsoleting an API, claim the next three-digit identifier in the `WFDEV###` sequence and add it to the list below. The URL template for all obsoletions is `https://aka.ms/winforms-warnings/{0}`. The `{0}` placeholder is replaced by the compiler with the `WFDEV###` identifier.

The acceptance criteria for adding an obsoletion includes:

* Add the obsoletion to the table below, claiming the next diagnostic id
    * Ensure the description is meaningful within the context of this table, and without requiring the context of the calling code
* Add new constants to `src\Common\src\Obsoletions.cs`, following the existing conventions
    * A `...Message` const using the same description added to the table below
    * A `...DiagnosticId` const for the `WFDEV###` id
* If adding <Obsolete> attribute to Microsoft.VisualBasic.Forms assembly, edit src\Microsoft.VisualBasic.Forms\src\Obsoletions.vb file
* Annotate `src` files by referring to the constants defined from `Obsoletions.cs`
    * Specify the `UrlFormat = Obsoletions.SharedUrlFormat`
    * Example: `[Obsolete(Obsoletions.DomainUpDownAccessibleObjectMessage, DiagnosticId = Obsoletions.DomainUpDownAccessibleObjectDiagnosticId, UrlFormat = Obsoletions.SharedUrlFormat)]`
    * If the `Obsoletions` type is not available in the project, link it into the project
        * `<Compile Include="..\..\Common\src\Obsoletions.cs" Link="Common\Obsoletions.cs" />`
* Apply the `:book: documentation: breaking` label to the PR that introduces the obsoletion
* Follow up with the breaking change process to communicate and document the breaking change
    * In the breaking-change issue filed in [dotnet/docs](https://github.com/dotnet/docs), specifically mention that this breaking change is an obsoletion with a `WFDEV` diagnostic id
    * The documentation team will produce a PR that adds the obsoletion to the [WFDEV warnings](https://learn.microsoft.com/dotnet/desktop/winforms/wfdev-diagnostics/obsoletions-overview) page
    * That PR will also add a new URL specific to this diagnostic ID; e.g. [WFDEV001](https://learn.microsoft.com/dotnet/desktop/winforms/wfdev-diagnostics/wfdev001)
    * Connect with `@gewarren` or `@BillWagner` with any questions
* Register the `WFDEV###` URL in `aka.ms`
    * The vanity name will be `winforms-warnings/WFDEV###`
    * Ensure the link's group owner matches the group owner of `winforms-warnings/WFDEV001`
    * Connect with `@lonitra` or `@gewarren` with any questions

### Obsoletion Diagnostics (`WFDEV001` - `WFDEV999`)

| Diagnostic ID     | Description |
| :---------------- | :---------- |
|  __`WFDEV001`__ | Casting to/from IntPtr is unsafe, use `WParamInternal`. |
|  __`WFDEV001`__ | Casting to/from IntPtr is unsafe, use `LParamInternal`. |
|  __`WFDEV001`__ | Casting to/from IntPtr is unsafe, use `ResultInternal`. |
|  __`WFDEV002`__ | `DomainUpDown.DomainUpDownAccessibleObject` is no longer used to provide accessible support for `DomainUpDown` controls. Use `ControlAccessibleObject` instead. |
|  __`WFDEV003`__ | `DomainUpDown.DomainItemAccessibleObject` is no longer used to provide accessible support for `DomainUpDown` items. |
|  __`WFDEV004`__ | `ContextMenu` has been deprecated. Use `ContextMenuStrip` instead. |
|  __`WFDEV005`__ | `DataGrid` has been deprecated. Use `DataGridView` instead. |
|  __`WFDEV006`__ | `DataGrid.HitTestInfo` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV007`__ | `DataGrid.HitTestType` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV008`__ | `DataGridBoolColumn` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV009`__ | `DataGridCell` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV010`__ | `DataGridColumnStyle` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV011`__ | `DataGridLineStyle` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV012`__ | `DataGridParentRowsLabelStyle` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV013`__ | `DataGridPreferredColumnWidthTypeConverter` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV014`__ | `DataGridTableStyle` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV015`__ | `DataGridTextBox` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV016`__ | `DataGridTextBoxColumn` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV017`__ | `GridColumnStylesCollection` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV018`__ | `GridTableStylesCollection` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV019`__ | `GridTablesFactory` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV020`__ | `IDataGridEditingService` has been deprecated. Use `DataGridView` control and related classes in your application. |
|  __`WFDEV021`__ | `MainMenu` has been deprecated. Use `MenuStrip` instead. |
|  __`WFDEV022`__ | `Menu` has been deprecated. Use `ToolStripDropDown` and `ToolStripDropDownMenu` instead. |
|  __`WFDEV023`__ | `Menu.MenuItemCollection` has been deprecated. Use `ToolStripDropDown` and `ToolStripDropDownMenu` control and related classes in your application. |
|  __`WFDEV024`__ | `MenuItem` has been deprecated. Use `ToolStripMenuItem` instead. |
|  __`WFDEV025`__ | `MenuMerge` has been deprecated. Use `ToolStripDropDown` and `ToolStripDropDownMenu` control and related classes in your application. |
|  __`WFDEV026`__ | `StatusBar` has been deprecated. Use `StatusStrip` instead. |
|  __`WFDEV027`__ | `StatusBarPanelCollection` has been deprecated. Use `StatusStrip` control and related classes in your application. |
|  __`WFDEV028`__ | `StatusBarDrawItemEventArgs` has been deprecated. Use `DrawItemEventArgs` instead. |
|  __`WFDEV029`__ | `StatusBarDrawItemEventHandler` has been deprecated. Use `DrawItemEventHandler` instead. |
|  __`WFDEV030`__ | `StatusBarPanel` has been deprecated. Use `StatusStrip` control and related classes in your application. |
|  __`WFDEV031`__ | `StatusBarPanelAutoSize` has been deprecated. Use `StatusStrip` control and related classes in your application. |
|  __`WFDEV032`__ | `StatusBarPanelBorderStyle` has been deprecated. Use `StatusStrip` control and related classes in your application. |
|  __`WFDEV033`__ | `StatusBarPanelClickEventArgs` has been deprecated. Use `StatusStrip` control and related classes in your application. |
|  __`WFDEV034`__ | `StatusBarPanelStyle` has been deprecated. Use `StatusStrip` control and related classes in your application. |
|  __`WFDEV035`__ | `StatusBarPanelClickEventHandler` has been deprecated. Use `StatusStrip` control and related classes in your application. |
|  __`WFDEV036`__ | `ToolBar` has been deprecated. Use `ToolStrip` instead. |
|  __`WFDEV037`__ | `ToolBarButtonCollection` has been deprecated. Use `ToolStrip` control and related classes in your application. |
|  __`WFDEV038`__ | `ToolBarAppearance` has been deprecated. Use `ToolStrip` control and related classes in your application. |
|  __`WFDEV039`__ | `ToolBarButton` has been deprecated. Use `ToolStripButton` instead. |
|  __`WFDEV040`__ | `ToolBarButtonClickEventArgs` has been deprecated. Use `ToolStrip` control and related classes in your application. |
|  __`WFDEV041`__ | `ToolBarButtonClickEventHandler` has been deprecated. Use `ToolStrip` control and related classes in your application. |
|  __`WFDEV042`__ | `ToolBarButtonStyle` has been deprecated. Use `ToolStrip` control and related classes in your application. |
|  __`WFDEV043`__ | `ToolBarTextAlign` has been deprecated. Use `ToolStrip` control and related classes in your application. |

|  __`WFDEV004`__ | `Form.OnClosing`, `Form.OnClosed` and the corresponding events are obsolete. Use `Form.OnFormClosing`, `Form.OnFormClosed`, `Form.FormClosing` and `Form.FormClosed` instead. |
|  __`WFDEV005`__ | `Clipboard.GetData(string)` method is obsolete. Use `Clipboard.TryGetData<T>` methods instead. |
|  __`WFDEV005`__ | `DataObject.GetData` methods are obsolete. Use the corresponding `DataObject.TryGetData<T>` instead. |
|  __`WFDEV005`__ | `ClipboardProxy.GetData(As String)` method is obsolete. Use `ClipboardProxy.TryGetData(Of T)(As String, As T)` instead. |


## Analyzer Warnings

The diagnostic id values reserved for Windows Forms .NET analyzer warnings are WF??001 through WF??999. 

### Analyzer Diagnostics (`WF??001` - `WF??999`)

| Diagnostic ID     | Description |
| :---------------- | :---------- |
|  __`WFAC001`__ | Unsupported project type. |
|  __`WFAC002`__ | Unsupported property value. |
|  __`WFAC010`__ | Unsupported high DPI configuration. |

