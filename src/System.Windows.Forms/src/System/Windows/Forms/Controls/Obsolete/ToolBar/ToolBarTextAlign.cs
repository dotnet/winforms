// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete("ToolBarTextAlign has been deprecated.")]
#pragma warning disable RS0016 // Add public types and members to the declared API
public enum ToolBarTextAlign
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Underneath = 0,

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Right = 1,
}
