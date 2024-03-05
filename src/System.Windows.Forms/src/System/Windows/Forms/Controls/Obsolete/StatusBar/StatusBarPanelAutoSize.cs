// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete("StatusBarPanelAutoSize has been deprecated.")]
public enum StatusBarPanelAutoSize
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    None = 1,
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Spring = 2,
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Contents = 3,
}
