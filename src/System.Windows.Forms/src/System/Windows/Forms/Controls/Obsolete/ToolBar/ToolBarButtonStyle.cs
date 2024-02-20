// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete("ToolBarButtonStyle has been deprecated.")]
#pragma warning disable RS0016 // Add public types and members to the declared API
public enum ToolBarButtonStyle
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    PushButton = 1,

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    ToggleButton = 2,

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Separator = 3,

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    DropDownButton = 4,
}
