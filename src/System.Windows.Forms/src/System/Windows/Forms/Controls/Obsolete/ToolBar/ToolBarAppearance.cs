// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete("ToolBarAppearance has been deprecated.")]
#pragma warning disable RS0016 // Add public types and members to the declared API
public enum ToolBarAppearance
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Normal = 0,

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Flat = 1,
}
