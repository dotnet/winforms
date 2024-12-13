' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Friend NotInheritable Class Obsoletions

    Friend Const SharedUrlFormat As String = "https://aka.ms/winforms-warnings/{0}"

    ' Please see docs\list-Of-diagnostics.md for how to claim a diagnostic id.
    ' The diagnostic ids reserved for obsoletions are WFDEV### (WFDEV001 - WFDEV999).

    Friend Const ClipboardProxyGetDataMessage As String = "`ClipboardProxy.GetData(As String)` method is obsolete. Use `ClipboardProxy.TryGetData(Of T)(As String, As T)` instead."
    Friend Const ClipboardGetDataDiagnosticId As String = "WFDEV005"
End Class
