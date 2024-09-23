// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.StatusBarPanelBorderStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarPanelBorderStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum StatusBarPanelBorderStyle
{
    None = 1,
    Raised = 2,
    Sunken = 3,
}
