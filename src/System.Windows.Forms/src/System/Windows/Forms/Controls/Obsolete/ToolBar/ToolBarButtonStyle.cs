// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete(
    Obsoletions.ToolBarButtonStyleMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarButtonStyleDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public enum ToolBarButtonStyle
{
    PushButton = 1,
    ToggleButton = 2,
    Separator = 3,
    DropDownButton = 4,
}
