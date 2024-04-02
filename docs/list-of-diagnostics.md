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
|  __`WFDEV005`__ | `Menu` has been deprecated. Use `ToolStripDropDown` and `ToolStripDropDownMenu` instead.  |
|  __`WFDEV006`__ | `Menu.MenuItemCollection` has been deprecated. |
|  __`WFDEV007`__ | `MenuItem` has been deprecated. Use `ToolStripMenuItem` instead. |
|  __`WFDEV008`__ | `MenuMerge` has been deprecated. |
|  __`WFDEV009`__ | `DataGrid` has been deprecated. Use `DataGridView` instead. |
|  __`WFDEV010`__ | `DataGrid.HitTestInfo` has been deprecated.  |
|  __`WFDEV011`__ | `DataGrid.HitTestType` has been deprecated. |
|  __`WFDEV012`__ | `DataGrid.DataGridAccessibleObject` has been deprecated. |
|  __`WFDEV013`__ | `DataGridAddNewRow` has been deprecated. |
|  __`WFDEV014`__ | `DataGridBoolColumn` has been deprecated.  |
|  __`WFDEV015`__ | `DataGridCaption` has been deprecated. |
|  __`WFDEV016`__ | `DataGridCell` has been deprecated. |
|  __`WFDEV017`__ | `DataGridColumnStyle` has been deprecated. |
|  __`WFDEV018`__ | `DataGridColumnHeaderAccessibleObject` has been deprecated. |
|  __`WFDEV019`__ | `DataGridLineStyle` has been deprecated. |
|  __`WFDEV020`__ | `DataGridParentRows` has been deprecated. |
|  __`WFDEV021`__ | `DataGridParentRowsAccessibleObject` has been deprecated. |
|  __`WFDEV022`__ | `DataGridParentRowsLabelStyle` has been deprecated. |
|  __`WFDEV023`__ | `DataGridPreferredColumnWidthTypeConverter` has been deprecated. |
|  __`WFDEV024`__ | `DataGridRelationshipRow` has been deprecated. |
|  __`WFDEV025`__ | `DataGridRelationshipRowAccessibleObject` has been deprecated.  |
|  __`WFDEV026`__ | `DataGridRelationshipAccessibleObject` has been deprecated. |
|  __`WFDEV027`__ | `DataGridRow` has been deprecated. |
|  __`WFDEV028`__ | `DataGridRowAccessibleObject` has been deprecated. |
|  __`WFDEV029`__ | `DataGridCellAccessibleObject` has been deprecated. |
|  __`WFDEV030`__ | `DataGridState` has been deprecated. |
|  __`WFDEV031`__ | `DataGridStateParentRowAccessibleObject` has been deprecated. |
|  __`WFDEV032`__ | `DataGridTableStyle` has been deprecated. |
|  __`WFDEV033`__ | `GridTableStylesCollection` has been deprecated. |
|  __`WFDEV034`__ | `GridTablesFactory` has been deprecated. |
|  __`WFDEV035`__ | `DataGridTextBox` has been deprecated.  |
|  __`WFDEV036`__ | `DataGridTextBoxColumn` has been deprecated. |
|  __`WFDEV037`__ | `DataGridToolTip` has been deprecated. |
|  __`WFDEV038`__ | `GridColumnStylesCollection` has been deprecated. |
|  __`WFDEV039`__ | `IDataGridEditingService` has been deprecated. |
|  __`WFDEV040`__ | `MainMenu` has been deprecated. Use `MenuStrip` instead. |
|  __`WFDEV041`__ | `StatusBar` has been deprecated. Use `StatusStrip` instead. |
|  __`WFDEV042`__ | `StatusBarPanelCollection` has been deprecated. |
|  __`WFDEV043`__ | `StatusBarDrawItemEventArgs` has been deprecated. Use `DrawItemEventArgs` instead. |
|  __`WFDEV044`__ | `StatusBarDrawItemEventHandler` has been deprecated. Use `DrawItemEventHandler` instead. |
|  __`WFDEV045`__ | `StatusBarPanel` has been deprecated. Use `StatusStrip` instead.  |
|  __`WFDEV046`__ | `StatusBarPanelAutoSize` has been deprecated. |
|  __`WFDEV047`__ | `StatusBarPanelBorderStyle` has been deprecated. Use the `BorderStyle` property of the `StatusBarPanel` class instead. |
|  __`WFDEV048`__ | `StatusBarPanelClickEventArgs` has been deprecated. |
|  __`WFDEV049`__ | `StatusBarPanelStyle` has been deprecated. Use `StatusBarPanel.Style` instead. |
|  __`WFDEV050`__ | `StatusBarPanelClickEventHandler` has been deprecated. |
|  __`WFDEV051`__ | `ToolBar` has been deprecated. Use `ToolStrip` instead. |
|  __`WFDEV052`__ | `ToolBarButtonCollection` has been deprecated. |
|  __`WFDEV053`__ | `ToolBarAppearance` has been deprecated. |
|  __`WFDEV054`__ | `ToolBarButton` has been deprecated. Use `ToolStripButton` instead. |
|  __`WFDEV055`__ | `ToolBarButtonClickEventArgs` has been deprecated.  |
|  __`WFDEV056`__ | `ToolBarButtonClickEventHandler` has been deprecated. |
|  __`WFDEV057`__ | `ToolBarButtonStyle` has been deprecated. |
|  __`WFDEV058`__ | `ToolBarTextAlign` has been deprecated. |

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

