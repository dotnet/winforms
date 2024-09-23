// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.StatusBarPanelStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarPanelStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum StatusBarPanelStyle
{
    Text = 1,
    OwnerDraw = 2,
}
