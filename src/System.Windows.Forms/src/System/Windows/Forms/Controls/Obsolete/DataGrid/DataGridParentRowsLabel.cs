// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016
[Obsolete("DataGridParentRowsLabelStyle has been deprecated.")]
public enum DataGridParentRowsLabelStyle
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    None = 0,
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    TableName = 1,
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    ColumnName = 2,
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Both = 3,
}
