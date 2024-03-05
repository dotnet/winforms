// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016
[Obsolete("DataGridLineStyle has been deprecated.")]
public enum DataGridLineStyle
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    None,
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Solid
}
