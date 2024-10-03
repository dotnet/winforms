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
    internal const string ContextMenuMessage = $"{nameof(ContextMenu)} is not supported. Use {nameof(ContextMenuStrip)} instead.";
#pragma warning restore WFDEV005
    internal const string ContextMenuDiagnosticId = "WFDEV005";

#pragma warning disable WFDEV006 // Type or member is obsolete
    internal const string DataGridMessage = $"{nameof(DataGrid)} is not supported. Use {nameof(DataGridView)} instead.";
#pragma warning restore WFDEV006
    internal const string DataGridDiagnosticId = "WFDEV006";

#pragma warning disable WFDEV007 // Type or member is obsolete
    internal const string MainMenuMessage = $"{nameof(MainMenu)} is not supported. Use {nameof(MenuStrip)} instead.";
#pragma warning restore WFDEV007
    internal const string MainMenuDiagnosticId = "WFDEV007";

#pragma warning disable WFDEV008 // Type or member is obsolete
    internal const string MenuMessage = $"{nameof(Menu)} is not supported. Use {nameof(ToolStripDropDown)} and {nameof(ToolStripDropDownMenu)} instead.";
#pragma warning restore WFDEV008
    internal const string MenuDiagnosticId = "WFDEV008";

#pragma warning disable WFDEV009 // Type or member is obsolete
    internal const string StatusBarMessage = $"{nameof(StatusBar)} is not supported. Use {nameof(StatusStrip)} instead.";
#pragma warning restore WFDEV009
    internal const string StatusBarDiagnosticId = "WFDEV009";

#pragma warning disable WFDEV010 // Type or member is obsolete
    internal const string ToolBarMessage = $"{nameof(ToolBar)} is not supported. Use {nameof(ToolStrip)} instead.";
#pragma warning restore WFDEV010
    internal const string ToolBarDiagnosticId = "WFDEV010";
}
