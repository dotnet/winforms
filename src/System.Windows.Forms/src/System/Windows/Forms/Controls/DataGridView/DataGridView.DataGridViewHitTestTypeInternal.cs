// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridView
{
    internal enum DataGridViewHitTestTypeInternal
    {
        None,
        Cell,
        ColumnHeader,
        RowHeader,
        ColumnResizeLeft,
        ColumnResizeRight,
        RowResizeTop,
        RowResizeBottom,
        FirstColumnHeaderLeft,
        TopLeftHeader,
        TopLeftHeaderResizeLeft,
        TopLeftHeaderResizeRight,
        TopLeftHeaderResizeTop,
        TopLeftHeaderResizeBottom,
        ColumnHeadersResizeBottom,
        ColumnHeadersResizeTop,
        RowHeadersResizeRight,
        RowHeadersResizeLeft,
        ColumnHeaderLeft,
        ColumnHeaderRight
    }
}
