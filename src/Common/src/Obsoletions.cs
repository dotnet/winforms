// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System;

internal static class Obsoletions
{
    internal const string SharedUrlFormat = "https://aka.ms/winforms-warnings/{0}";

    // Please see docs\list-of-diagnostics.md for instructions on the steps required
    // to introduce a new obsoletion, apply it to downlevel builds, claim a diagnostic id,
    // and ensure the "aka.ms/dotnet-warnings/{0}" URL points to documentation for the obsoletion
    // The diagnostic ids reserved for obsoletions are WFDEV### (WFDEV001 - WFDEV999).

    internal const string DomainUpDownAccessibleObjectMessage = $"DomainUpDownAccessibleObject is no longer used to provide accessible support for {nameof(DomainUpDown)} controls. Use {nameof(Control.ControlAccessibleObject)} instead.";
    internal const string DomainUpDownAccessibleObjectDiagnosticId = "WFDEV002";

#pragma warning disable WFDEV003 // Type or member is obsolete
    internal const string DomainItemAccessibleObjectMessage = $"{nameof(DomainUpDown.DomainItemAccessibleObject)} is no longer used to provide accessible support for {nameof(DomainUpDown)} items.";
#pragma warning restore WFDEV003
    internal const string DomainItemAccessibleObjectDiagnosticId = "WFDEV003";

#pragma warning disable WFDEV004 // Type or member is obsolete
    internal const string ContextMenuMessage = $"{nameof(ContextMenu)} has been deprecated. Use {nameof(ContextMenuStrip)} instead.";
#pragma warning restore WFDEV004 // Type or member is obsolete
    internal const string ContextMenuDiagnosticId = "WFDEV004";

#pragma warning disable WFDEV005 // Type or member is obsolete
    internal const string MenuMessage = $"{nameof(Menu)} has been deprecated. Use {nameof(ToolStripDropDown)} and {nameof(ToolStripDropDownMenu)} instead.";
#pragma warning restore WFDEV005 // Type or member is obsolete
    internal const string MenuDiagnosticId = "WFDEV005";

    internal const string MenuItemCollectionMessage = $"Menu.MenuItemCollection has been deprecated.";
    internal const string MenuItemCollectionDiagnosticId = "WFDEV006";

#pragma warning disable WFDEV007 // Type or member is obsolete
    internal const string MenuItemMessage = $"{nameof(MenuItem)} has been deprecated. Use {nameof(ToolStripMenuItem)} instead.";
#pragma warning restore WFDEV007 // Type or member is obsolete
    internal const string MenuItemDiagnosticId = "WFDEV007";

#pragma warning disable WFDEV008 // Type or member is obsolete
    internal const string MenuMergeMessage = $"{nameof(MenuMerge)} has been deprecated.";
#pragma warning restore WFDEV008 // Type or member is obsolete
    internal const string MenuMergeDiagnosticId = "WFDEV008";

#pragma warning disable WFDEV009 // Type or member is obsolete
    internal const string DataGridMessage = $"{nameof(DataGrid)} has been deprecated. Use {nameof(DataGridView)} instead.";
#pragma warning restore WFDEV009 // Type or member is obsolete
    internal const string DataGridDiagnosticId = "WFDEV009";

    internal const string DataGridHitTestInfoMessage = $"DataGrid.HitTestInfo has been deprecated.";
    internal const string DataGridHitTestInfoDiagnosticId = "WFDEV010";

    internal const string DataGridHitTestTypeMessage = $"DataGrid.HitTestType has been deprecated.";
    internal const string DataGridHitTestTypeDiagnosticId = "WFDEV011";

    internal const string DataGridAccessibleObjectMessage = $"DataGrid.DataGridAccessibleObject has been deprecated.";
    internal const string DataGridAccessibleObjectDiagnosticId = "WFDEV012";

#pragma warning disable WFDEV013 // Type or member is obsolete
    internal const string DataGridAddNewRowMessage = $"{nameof(DataGridAddNewRow)} has been deprecated.";
#pragma warning restore WFDEV013 // Type or member is obsolete
    internal const string DataGridAddNewRowDiagnosticId = "WFDEV013";

#pragma warning disable WFDEV014 // Type or member is obsolete
    internal const string DataGridBoolColumnMessage = $"{nameof(DataGridBoolColumn)} has been deprecated.";
#pragma warning restore WFDEV014 // Type or member is obsolete
    internal const string DataGridBoolColumnDiagnosticId = "WFDEV014";

#pragma warning disable WFDEV015 // Type or member is obsolete
    internal const string DataGridCaptionMessage = $"{nameof(DataGridCaption)} has been deprecated.";
#pragma warning restore WFDEV015 // Type or member is obsolete
    internal const string DataGridCaptionDiagnosticId = "WFDEV015";

#pragma warning disable WFDEV016 // Type or member is obsolete
    internal const string DataGridCellMessage = $"{nameof(DataGridCell)} has been deprecated.";
#pragma warning restore WFDEV016 // Type or member is obsolete
    internal const string DataGridCellDiagnosticId = "WFDEV016";

#pragma warning disable WFDEV017 // Type or member is obsolete
    internal const string DataGridColumnStyleMessage = $"{nameof(DataGridColumnStyle)} has been deprecated.";
#pragma warning restore WFDEV017 // Type or member is obsolete
    internal const string DataGridColumnStyleDiagnosticId = "WFDEV017";

    internal const string DataGridColumnHeaderAccessibleObjectMessage = $"DataGridColumnHeaderAccessibleObject has been deprecated.";
    internal const string DataGridColumnHeaderAccessibleObjectDiagnosticId = "WFDEV018";

#pragma warning disable WFDEV019 // Type or member is obsolete
    internal const string DataGridLineStyleMessage = $"{nameof(DataGridLineStyle)} has been deprecated.";
#pragma warning restore WFDEV019 // Type or member is obsolete
    internal const string DataGridLineStyleDiagnosticId = "WFDEV019";

#pragma warning disable WFDEV020 // Type or member is obsolete
    internal const string DataGridParentRowsMessage = $"{nameof(DataGridParentRows)} has been deprecated.";
#pragma warning restore WFDEV020 // Type or member is obsolete
    internal const string DataGridParentRowsDiagnosticId = "WFDEV020";

    internal const string DataGridParentRowsAccessibleObjectMessage = $"DataGridParentRowsAccessibleObject has been deprecated.";
    internal const string DataGridParentRowsAccessibleObjectDiagnosticId = "WFDEV021";

#pragma warning disable WFDEV022 // Type or member is obsolete
    internal const string DataGridParentRowsLabelStyleMessage = $"{nameof(DataGridParentRowsLabelStyle)} has been deprecated.";
#pragma warning restore WFDEV022 // Type or member is obsolete
    internal const string DataGridParentRowsLabelStyleDiagnosticId = "WFDEV022";

#pragma warning disable WFDEV023 // Type or member is obsolete
    internal const string DataGridPreferredColumnWidthTypeConverterMessage = $"{nameof(DataGridPreferredColumnWidthTypeConverter)} has been deprecated.";
#pragma warning restore WFDEV023 // Type or member is obsolete
    internal const string DataGridPreferredColumnWidthTypeConverterDiagnosticId = "WFDEV023";

#pragma warning disable WFDEV024 // Type or member is obsolete
    internal const string DataGridRelationshipRowMessage = $"{nameof(DataGridRelationshipRow)} has been deprecated.";
#pragma warning restore WFDEV024 // Type or member is obsolete
    internal const string DataGridRelationshipRowDiagnosticId = "WFDEV024";

    internal const string DataGridRelationshipRowAccessibleObjectMessage = $"DataGridRelationshipRowAccessibleObject has been deprecated.";
    internal const string DataGridRelationshipRowAccessibleObjectDiagnosticId = "WFDEV025";

    internal const string DataGridRelationshipAccessibleObjectMessage = $"DataGridRelationshipAccessibleObject has been deprecated.";
    internal const string DataGridRelationshipAccessibleObjectDiagnosticId = "WFDEV026";

#pragma warning disable WFDEV027 // Type or member is obsolete
    internal const string DataGridRowMessage = $"{nameof(DataGridRow)} has been deprecated.";
#pragma warning restore WFDEV027 // Type or member is obsolete
    internal const string DataGridRowDiagnosticId = "WFDEV027";

    internal const string DataGridRowAccessibleObjectMessage = $"DataGridRowAccessibleObject has been deprecated.";
    internal const string DataGridRowAccessibleObjectDiagnosticId = "WFDEV028";

    internal const string DataGridCellAccessibleObjectMessage = $"DataGridCellAccessibleObject has been deprecated.";
    internal const string DataGridCellAccessibleObjectDiagnosticId = "WFDEV029";

#pragma warning disable WFDEV030 // Type or member is obsolete
    internal const string DataGridStateMessage = $"{nameof(DataGridState)} has been deprecated.";
#pragma warning restore WFDEV030 // Type or member is obsolete
    internal const string DataGridStateDiagnosticId = "WFDEV030";

    internal const string DataGridStateParentRowAccessibleObjectMessage = $"DataGridStateParentRowAccessibleObject has been deprecated.";
    internal const string DataGridStateParentRowAccessibleObjectDiagnosticId = "WFDEV031";

#pragma warning disable WFDEV032 // Type or member is obsolete
    internal const string DataGridTableStyleMessage = $"{nameof(DataGridTableStyle)} has been deprecated.";
#pragma warning restore WFDEV032 // Type or member is obsolete
    internal const string DataGridTableStyleDiagnosticId = "WFDEV032";

#pragma warning disable WFDEV033 // Type or member is obsolete
    internal const string GridTableStylesCollectionMessage = $"{nameof(GridTableStylesCollection)} has been deprecated.";
#pragma warning restore WFDEV033 // Type or member is obsolete
    internal const string GridTableStylesCollectionDiagnosticId = "WFDEV033";

#pragma warning disable WFDEV034 // Type or member is obsolete
    internal const string GridTablesFactoryMessage = $"{nameof(GridTablesFactory)} has been deprecated.";
#pragma warning restore WFDEV034 // Type or member is obsolete
    internal const string GridTablesFactoryDiagnosticId = "WFDEV034";

#pragma warning disable WFDEV035 // Type or member is obsolete
    internal const string DataGridTextBoxMessage = $"{nameof(DataGridTextBox)} has been deprecated.";
#pragma warning restore WFDEV035 // Type or member is obsolete
    internal const string DataGridTextBoxDiagnosticId = "WFDEV035";

#pragma warning disable WFDEV036 // Type or member is obsolete
    internal const string DataGridTextBoxColumnMessage = $"{nameof(DataGridTextBoxColumn)} has been deprecated.";
#pragma warning restore WFDEV036 // Type or member is obsolete
    internal const string DataGridTextBoxColumnDiagnosticId = "WFDEV036";

#pragma warning disable WFDEV037 // Type or member is obsolete
    internal const string DataGridToolTipMessage = $"{nameof(DataGridToolTip)} has been deprecated.";
#pragma warning restore WFDEV037 // Type or member is obsolete
    internal const string DataGridToolTipDiagnosticId = "WFDEV037";

#pragma warning disable WFDEV038 // Type or member is obsolete
    internal const string GridColumnStylesCollectionMessage = $"{nameof(GridColumnStylesCollection)} has been deprecated.";
#pragma warning restore WFDEV038 // Type or member is obsolete
    internal const string GridColumnStylesCollectionDiagnosticId = "WFDEV038";

#pragma warning disable WFDEV039 // Type or member is obsolete
    internal const string IDataGridEditingServiceMessage = $"{nameof(IDataGridEditingService)} has been deprecated.";
#pragma warning restore WFDEV039 // Type or member is obsolete
    internal const string IDataGridEditingServiceDiagnosticId = "WFDEV039";

#pragma warning disable WFDEV040 // Type or member is obsolete
    internal const string MainMenuMessage = $"{nameof(MainMenu)} has been deprecated. Use {nameof(MenuStrip)} instead.";
#pragma warning restore WFDEV040 // Type or member is obsolete
    internal const string MainMenuDiagnosticId = "WFDEV040";

#pragma warning disable WFDEV041 // Type or member is obsolete
    internal const string StatusBarMessage = $"{nameof(StatusBar)} has been deprecated. Use {nameof(StatusStrip)} instead.";
#pragma warning restore WFDEV041 // Type or member is obsolete
    internal const string StatusBarDiagnosticId = "WFDEV041";

    internal const string StatusBarPanelCollectionMessage = $"StatusBarPanelCollection has been deprecated.";
    internal const string StatusBarPanelCollectionDiagnosticId = "WFDEV042";

#pragma warning disable WFDEV043 // Type or member is obsolete
    internal const string StatusBarDrawItemEventArgsMessage = $"{nameof(StatusBarDrawItemEventArgs)} has been deprecated. Use {nameof(DrawItemEventArgs)} instead.";
#pragma warning restore WFDEV043 // Type or member is obsolete
    internal const string StatusBarDrawItemEventArgsDiagnosticId = "WFDEV043";

#pragma warning disable WFDEV044 // Type or member is obsolete
    internal const string StatusBarDrawItemEventHandlerMessage = $"{nameof(StatusBarDrawItemEventHandler)} has been deprecated. Use {nameof(DrawItemEventHandler)} instead.";
#pragma warning restore WFDEV044 // Type or member is obsolete
    internal const string StatusBarDrawItemEventHandlerDiagnosticId = "WFDEV044";

#pragma warning disable WFDEV045 // Type or member is obsolete
    internal const string StatusBarPanelMessage = $"{nameof(StatusBarPanel)} has been deprecated. Use {nameof(StatusStrip)} instead.";
#pragma warning restore WFDEV045 // Type or member is obsolete
    internal const string StatusBarPanelDiagnosticId = "WFDEV045";

#pragma warning disable WFDEV046 // Type or member is obsolete
    internal const string StatusBarPanelAutoSizeMessage = $"{nameof(StatusBarPanelAutoSize)} has been deprecated.";
#pragma warning restore WFDEV046 // Type or member is obsolete
    internal const string StatusBarPanelAutoSizeDiagnosticId = "WFDEV046";

#pragma warning disable WFDEV047 // Type or member is obsolete
    internal const string StatusBarPanelBorderStyleMessage = $"{nameof(StatusBarPanelBorderStyle)} has been deprecated.";
#pragma warning restore WFDEV047 // Type or member is obsolete
    internal const string StatusBarPanelBorderStyleDiagnosticId = "WFDEV047";

#pragma warning disable WFDEV048 // Type or member is obsolete
    internal const string StatusBarPanelClickEventArgsMessage = $"{nameof(StatusBarPanelClickEventArgs)} has been deprecated.";
#pragma warning restore WFDEV048 // Type or member is obsolete
    internal const string StatusBarPanelClickEventArgsDiagnosticId = "WFDEV048";

#pragma warning disable WFDEV049 // Type or member is obsolete
    internal const string StatusBarPanelStyleMessage = $"{nameof(StatusBarPanelStyle)} has been deprecated.";
#pragma warning restore WFDEV049 // Type or member is obsolete
    internal const string StatusBarPanelStyleDiagnosticId = "WFDEV049";

#pragma warning disable WFDEV050 // Type or member is obsolete
    internal const string StatusBarPanelClickEventHandlerMessage = $"{nameof(StatusBarPanelClickEventHandler)} has been deprecated.";
#pragma warning restore WFDEV050 // Type or member is obsolete
    internal const string StatusBarPanelClickEventHandlerDiagnosticId = "WFDEV050";

#pragma warning disable WFDEV051 // Type or member is obsolete
    internal const string ToolBarMessage = $"{nameof(ToolBar)} has been deprecated. Use {nameof(ToolStrip)} instead.";
#pragma warning restore WFDEV051 // Type or member is obsolete
    internal const string ToolBarDiagnosticId = "WFDEV051";

    internal const string ToolBarButtonCollectionMessage = $"ToolBarButtonCollection has been deprecated.";
    internal const string ToolBarButtonCollectionDiagnosticId = "WFDEV052";

#pragma warning disable WFDEV053 // Type or member is obsolete
    internal const string ToolBarAppearanceMessage = $"{nameof(ToolBarAppearance)} has been deprecated.";
#pragma warning restore WFDEV053 // Type or member is obsolete
    internal const string ToolBarAppearanceDiagnosticId = "WFDEV053";

#pragma warning disable WFDEV054 // Type or member is obsolete
    internal const string ToolBarButtonMessage = $"{nameof(ToolBarButton)} has been deprecated. Use {nameof(ToolStripButton)} instead.";
#pragma warning restore WFDEV054 // Type or member is obsolete
    internal const string ToolBarButtonDiagnosticId = "WFDEV054";

#pragma warning disable WFDEV055 // Type or member is obsolete
    internal const string ToolBarButtonClickEventArgsMessage = $"{nameof(ToolBarButtonClickEventArgs)} has been deprecated.";
#pragma warning restore WFDEV055 // Type or member is obsolete
    internal const string ToolBarButtonClickEventArgsDiagnosticId = "WFDEV055";

#pragma warning disable WFDEV056 // Type or member is obsolete
    internal const string ToolBarButtonClickEventHandlerMessage = $"{nameof(ToolBarButtonClickEventHandler)} has been deprecated.";
#pragma warning restore WFDEV056 // Type or member is obsolete
    internal const string ToolBarButtonClickEventHandlerDiagnosticId = "WFDEV056";

#pragma warning disable WFDEV057 // Type or member is obsolete
    internal const string ToolBarButtonStyleMessage = $"{nameof(ToolBarButtonStyle)} has been deprecated.";
#pragma warning restore WFDEV057 // Type or member is obsolete
    internal const string ToolBarButtonStyleDiagnosticId = "WFDEV057";

#pragma warning disable WFDEV058 // Type or member is obsolete
    internal const string ToolBarTextAlignMessage = $"{nameof(ToolBarTextAlign)} has been deprecated.";
#pragma warning restore WFDEV058 // Type or member is obsolete
    internal const string ToolBarTextAlignDiagnosticId = "WFDEV058";

    internal const string FormOnClosingClosedMessage = "Form.OnClosing, Form.OnClosed and the corresponding events are obsolete. Use Form.OnFormClosing, Form.OnFormClosed, Form.FormClosing and Form.FormClosed instead.";
    internal const string FormOnClosingClosedDiagnosticId = "WFDEV004";

    internal const string ClipboardGetDataMessage = "`Clipboard.GetData(string)` method is obsolete. Use `Clipboard.TryGetData<T>` methods instead.";
    internal const string ClipboardGetDataDiagnosticId = "WFDEV005";

    internal const string DataObjectGetDataMessage = "`DataObject.GetData` methods are obsolete. Use the corresponding `DataObject.TryGetData<T>` instead.";

    internal const string ClipboardProxyGetDataMessage = "`ClipboardProxy.GetData(As String)` method is obsolete. Use `ClipboardProxy.TryGetData(Of T)(As String, As T)` instead.";
}
