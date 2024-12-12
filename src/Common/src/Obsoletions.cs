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

    internal const string FormOnClosingClosedMessage = "Form.OnClosing, Form.OnClosed and the corresponding events are obsolete. Use Form.OnFormClosing, Form.OnFormClosed, Form.FormClosing and Form.FormClosed instead.";
    internal const string FormOnClosingClosedDiagnosticId = "WFDEV004";

    internal const string ClipboardGetDataMessage = "`Clipboard.GetData(string)` method is obsolete. Use `Clipboard.TryGetData<T>` methods instead.";
    internal const string ClipboardGetDataDiagnosticId = "WFDEV005";

    internal const string DataObjectGetDataMessage = "`DataObject.GetData` methods are obsolete. Use the corresponding `DataObject.TryGetData<T>` instead.";

    internal const string ClipboardProxyGetDataMessage = "`ClipboardProxy.GetData(As String)` method is obsolete. Use `ClipboardProxy.TryGetData(Of T)(As String, As T)` instead.";
}
