// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.StatusBarPanelAutoSizeMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarPanelAutoSizeDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum StatusBarPanelAutoSize
{
    None = 1,
    Spring = 2,
    Contents = 3,
}
