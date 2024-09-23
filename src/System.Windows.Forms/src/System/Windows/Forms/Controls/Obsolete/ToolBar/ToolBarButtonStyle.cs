// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.ToolBarButtonStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarButtonStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public enum ToolBarButtonStyle
{
    PushButton = 1,
    ToggleButton = 2,
    Separator = 3,
    DropDownButton = 4,
}
