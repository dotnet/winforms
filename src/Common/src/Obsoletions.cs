// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System;

internal static class Obsoletions
{
    internal const string SharedUrlFormat = "https://aka.ms/winforms-warnings/{0}";

    // Please see docs\project\list-of-diagnostics.md for instructions on the steps required
    // to introduce a new obsoletion, apply it to downlevel builds, claim a diagnostic id,
    // and ensure the "aka.ms/dotnet-warnings/{0}" URL points to documentation for the obsoletion
    // The diagnostic ids reserved for obsoletions are WFDEV### (WFDEV001 - WFDEV999).

    internal const string DomainUpDownAccessibleObjectMessage = $"DomainUpDownAccessibleObject is no longer used to provide accessible support for {nameof(DomainUpDown)} controls. Use {nameof(Control.ControlAccessibleObject)} instead.";
    internal const string DomainUpDownAccessibleObjectDiagnosticId = "WFDEV002";

#pragma warning disable WFDEV003 // Type or member is obsolete
    internal const string DomainItemAccessibleObjectMessage = $"{nameof(DomainUpDown.DomainItemAccessibleObject)} is no longer used to provide accessible support for {nameof(DomainUpDown)} items.";
#pragma warning restore WFDEV003
    internal const string DomainItemAccessibleObjectDiagnosticId = "WFDEV003";

    internal const string FormOnClosingClosedMessage = "Form.OnClosing, Form.OnClosed and the corresponding events are obsolete. Use Form.OnFormClosing, Form.OnFormClosed, Form.FormClosing and Form.FormClosed instead.";
    internal const string FormOnClosingClosedDiagnosticId = "WFDEV004";

#pragma warning disable WFDEV005 // Type or member is obsolete
    internal const string ContextMenuMessage = $"{nameof(ContextMenu)} has been deprecated. Use {nameof(ContextMenuStrip)} instead.";
#pragma warning restore WFDEV005 // Type or member is obsolete
    internal const string ContextMenuDiagnosticId = "WFDEV005";

#pragma warning disable WFDEV006 // Type or member is obsolete
    internal const string DataGridMessage = $"{nameof(DataGrid)} has been deprecated. Use {nameof(DataGridView)} instead.";
#pragma warning restore WFDEV006 // Type or member is obsolete
    internal const string DataGridDiagnosticId = "WFDEV006";

    internal const string DataGridHitTestInfoMessage = $"HitTestInfo has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
    internal const string DataGridHitTestInfoDiagnosticId = "WFDEV007";

    internal const string DataGridHitTestTypeMessage = $"HitTestType has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
    internal const string DataGridHitTestTypeDiagnosticId = "WFDEV008";

#pragma warning disable WFDEV009 // Type or member is obsolete
    internal const string DataGridBoolColumnMessage = $"{nameof(DataGridBoolColumn)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV009 // Type or member is obsolete
    internal const string DataGridBoolColumnDiagnosticId = "WFDEV009";

#pragma warning disable WFDEV010 // Type or member is obsolete
    internal const string DataGridCellMessage = $"{nameof(DataGridCell)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV010 // Type or member is obsolete
    internal const string DataGridCellDiagnosticId = "WFDEV010";

#pragma warning disable WFDEV011 // Type or member is obsolete
    internal const string DataGridColumnStyleMessage = $"{nameof(DataGridColumnStyle)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV011 // Type or member is obsolete
    internal const string DataGridColumnStyleDiagnosticId = "WFDEV011";

#pragma warning disable WFDEV012 // Type or member is obsolete
    internal const string DataGridLineStyleMessage = $"{nameof(DataGridLineStyle)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV012 // Type or member is obsolete
    internal const string DataGridLineStyleDiagnosticId = "WFDEV012";

#pragma warning disable WFDEV013 // Type or member is obsolete
    internal const string DataGridParentRowsLabelStyleMessage = $"{nameof(DataGridParentRowsLabelStyle)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV013 // Type or member is obsolete
    internal const string DataGridParentRowsLabelStyleDiagnosticId = "WFDEV013";

#pragma warning disable WFDEV014 // Type or member is obsolete
    internal const string DataGridPreferredColumnWidthTypeConverterMessage = $"{nameof(DataGridPreferredColumnWidthTypeConverter)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV014 // Type or member is obsolete
    internal const string DataGridPreferredColumnWidthTypeConverterDiagnosticId = "WFDEV014";

#pragma warning disable WFDEV015 // Type or member is obsolete
    internal const string DataGridTableStyleMessage = $"{nameof(DataGridTableStyle)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV015 // Type or member is obsolete
    internal const string DataGridTableStyleDiagnosticId = "WFDEV015";

#pragma warning disable WFDEV016 // Type or member is obsolete
    internal const string DataGridTextBoxMessage = $"{nameof(DataGridTextBox)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV016 // Type or member is obsolete
    internal const string DataGridTextBoxDiagnosticId = "WFDEV016";

#pragma warning disable WFDEV017 // Type or member is obsolete
    internal const string DataGridTextBoxColumnMessage = $"{nameof(DataGridTextBoxColumn)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV017 // Type or member is obsolete
    internal const string DataGridTextBoxColumnDiagnosticId = "WFDEV017";

#pragma warning disable WFDEV018 // Type or member is obsolete
    internal const string GridColumnStylesCollectionMessage = $"{nameof(GridColumnStylesCollection)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV018 // Type or member is obsolete
    internal const string GridColumnStylesCollectionDiagnosticId = "WFDEV018";

#pragma warning disable WFDEV019 // Type or member is obsolete
    internal const string GridTableStylesCollectionMessage = $"{nameof(GridTableStylesCollection)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV019 // Type or member is obsolete
    internal const string GridTableStylesCollectionDiagnosticId = "WFDEV019";

#pragma warning disable WFDEV020 // Type or member is obsolete
    internal const string GridTablesFactoryMessage = $"{nameof(GridTablesFactory)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV020 // Type or member is obsolete
    internal const string GridTablesFactoryDiagnosticId = "WFDEV020";

#pragma warning disable WFDEV021 // Type or member is obsolete
    internal const string IDataGridEditingServiceMessage = $"{nameof(IDataGridEditingService)} has been deprecated. Use {nameof(DataGridView)} control and related classes in your application.";
#pragma warning restore WFDEV021 // Type or member is obsolete
    internal const string IDataGridEditingServiceDiagnosticId = "WFDEV021";

#pragma warning disable WFDEV022 // Type or member is obsolete
    internal const string MainMenuMessage = $"{nameof(MainMenu)} has been deprecated. Use {nameof(MenuStrip)} instead.";
#pragma warning restore WFDEV022 // Type or member is obsolete
    internal const string MainMenuDiagnosticId = "WFDEV022";

#pragma warning disable WFDEV023 // Type or member is obsolete
    internal const string MenuMessage = $"{nameof(Menu)} has been deprecated. Use {nameof(ToolStripDropDown)} and {nameof(ToolStripDropDownMenu)} instead.";
#pragma warning restore WFDEV023 // Type or member is obsolete
    internal const string MenuDiagnosticId = "WFDEV023";

    internal const string MenuItemCollectionMessage = $"MenuItemCollection has been deprecated. Use {nameof(ToolStripDropDown)} and {nameof(ToolStripDropDownMenu)} control and related classes in your application.";
    internal const string MenuItemCollectionDiagnosticId = "WFDEV024";

#pragma warning disable WFDEV025 // Type or member is obsolete
    internal const string MenuItemMessage = $"{nameof(MenuItem)} has been deprecated. Use {nameof(ToolStripMenuItem)} instead.";
#pragma warning restore WFDEV025 // Type or member is obsolete
    internal const string MenuItemDiagnosticId = "WFDEV025";

#pragma warning disable WFDEV026 // Type or member is obsolete
    internal const string MenuMergeMessage = $"{nameof(MenuMerge)} has been deprecated. Use {nameof(ToolStripDropDown)} and {nameof(ToolStripDropDownMenu)} control and related classes in your application.";
#pragma warning restore WFDEV026 // Type or member is obsolete
    internal const string MenuMergeDiagnosticId = "WFDEV026";

#pragma warning disable WFDEV027 // Type or member is obsolete
    internal const string StatusBarMessage = $"{nameof(StatusBar)} has been deprecated. Use {nameof(StatusStrip)} instead.";
#pragma warning restore WFDEV027 // Type or member is obsolete
    internal const string StatusBarDiagnosticId = "WFDEV027";

    internal const string StatusBarPanelCollectionMessage = $"StatusBarPanelCollection has been deprecated. Use {nameof(StatusStrip)} control and related classes in your application.";
    internal const string StatusBarPanelCollectionDiagnosticId = "WFDEV028";

#pragma warning disable WFDEV029 // Type or member is obsolete
    internal const string StatusBarDrawItemEventArgsMessage = $"{nameof(StatusBarDrawItemEventArgs)} has been deprecated. Use {nameof(DrawItemEventArgs)} instead.";
#pragma warning restore WFDEV029 // Type or member is obsolete
    internal const string StatusBarDrawItemEventArgsDiagnosticId = "WFDEV029";

#pragma warning disable WFDEV030 // Type or member is obsolete
    internal const string StatusBarDrawItemEventHandlerMessage = $"{nameof(StatusBarDrawItemEventHandler)} has been deprecated. Use {nameof(DrawItemEventHandler)} instead.";
#pragma warning restore WFDEV030 // Type or member is obsolete
    internal const string StatusBarDrawItemEventHandlerDiagnosticId = "WFDEV030";

#pragma warning disable WFDEV031 // Type or member is obsolete
    internal const string StatusBarPanelMessage = $"{nameof(StatusBarPanel)} has been deprecated. Use {nameof(StatusStrip)} control and related classes in your application.";
#pragma warning restore WFDEV031 // Type or member is obsolete
    internal const string StatusBarPanelDiagnosticId = "WFDEV031";

#pragma warning disable WFDEV032 // Type or member is obsolete
    internal const string StatusBarPanelAutoSizeMessage = $"{nameof(StatusBarPanelAutoSize)} has been deprecated. Use {nameof(StatusStrip)} control and related classes in your application.";
#pragma warning restore WFDEV032 // Type or member is obsolete
    internal const string StatusBarPanelAutoSizeDiagnosticId = "WFDEV032";

#pragma warning disable WFDEV033 // Type or member is obsolete
    internal const string StatusBarPanelBorderStyleMessage = $"{nameof(StatusBarPanelBorderStyle)} has been deprecated. Use {nameof(StatusStrip)} control and related classes in your application.";
#pragma warning restore WFDEV033 // Type or member is obsolete
    internal const string StatusBarPanelBorderStyleDiagnosticId = "WFDEV033";

#pragma warning disable WFDEV034 // Type or member is obsolete
    internal const string StatusBarPanelClickEventArgsMessage = $"{nameof(StatusBarPanelClickEventArgs)} has been deprecated. Use {nameof(StatusStrip)} control and related classes in your application.";
#pragma warning restore WFDEV034 // Type or member is obsolete
    internal const string StatusBarPanelClickEventArgsDiagnosticId = "WFDEV034";

#pragma warning disable WFDEV035 // Type or member is obsolete
    internal const string StatusBarPanelStyleMessage = $"{nameof(StatusBarPanelStyle)} has been deprecated. Use {nameof(StatusStrip)} control and related classes in your application.";
#pragma warning restore WFDEV035 // Type or member is obsolete
    internal const string StatusBarPanelStyleDiagnosticId = "WFDEV035";

#pragma warning disable WFDEV036 // Type or member is obsolete
    internal const string StatusBarPanelClickEventHandlerMessage = $"{nameof(StatusBarPanelClickEventHandler)} has been deprecated. Use {nameof(StatusStrip)} control and related classes in your application.";
#pragma warning restore WFDEV036 // Type or member is obsolete
    internal const string StatusBarPanelClickEventHandlerDiagnosticId = "WFDEV036";

#pragma warning disable WFDEV037 // Type or member is obsolete
    internal const string ToolBarMessage = $"{nameof(ToolBar)} has been deprecated. Use {nameof(ToolStrip)} instead.";
#pragma warning restore WFDEV037 // Type or member is obsolete
    internal const string ToolBarDiagnosticId = "WFDEV037";

    internal const string ToolBarButtonCollectionMessage = $"ToolBarButtonCollection has been deprecated. Use {nameof(ToolStrip)} control and related classes in your application.";
    internal const string ToolBarButtonCollectionDiagnosticId = "WFDEV038";

#pragma warning disable WFDEV039 // Type or member is obsolete
    internal const string ToolBarAppearanceMessage = $"{nameof(ToolBarAppearance)} has been deprecated. Use {nameof(ToolStrip)} control and related classes in your application.";
#pragma warning restore WFDEV039 // Type or member is obsolete
    internal const string ToolBarAppearanceDiagnosticId = "WFDEV039";

#pragma warning disable WFDEV040 // Type or member is obsolete
    internal const string ToolBarButtonMessage = $"{nameof(ToolBarButton)} has been deprecated. Use {nameof(ToolStripButton)} instead.";
#pragma warning restore WFDEV040 // Type or member is obsolete
    internal const string ToolBarButtonDiagnosticId = "WFDEV040";

#pragma warning disable WFDEV041 // Type or member is obsolete
    internal const string ToolBarButtonClickEventArgsMessage = $"{nameof(ToolBarButtonClickEventArgs)} has been deprecated. Use {nameof(ToolStrip)} control and related classes in your application.";
#pragma warning restore WFDEV041 // Type or member is obsolete
    internal const string ToolBarButtonClickEventArgsDiagnosticId = "WFDEV041";

#pragma warning disable WFDEV042 // Type or member is obsolete
    internal const string ToolBarButtonClickEventHandlerMessage = $"{nameof(ToolBarButtonClickEventHandler)} has been deprecated. Use {nameof(ToolStrip)} control and related classes in your application.";
#pragma warning restore WFDEV042 // Type or member is obsolete
    internal const string ToolBarButtonClickEventHandlerDiagnosticId = "WFDEV042";

#pragma warning disable WFDEV043 // Type or member is obsolete
    internal const string ToolBarButtonStyleMessage = $"{nameof(ToolBarButtonStyle)} has been deprecated. Use {nameof(ToolStrip)} control and related classes in your application.";
#pragma warning restore WFDEV043 // Type or member is obsolete
    internal const string ToolBarButtonStyleDiagnosticId = "WFDEV043";

#pragma warning disable WFDEV044 // Type or member is obsolete
    internal const string ToolBarTextAlignMessage = $"{nameof(ToolBarTextAlign)} has been deprecated. Use {nameof(ToolStrip)} control and related classes in your application.";
#pragma warning restore WFDEV044 // Type or member is obsolete
    internal const string ToolBarTextAlignDiagnosticId = "WFDEV044";
}
