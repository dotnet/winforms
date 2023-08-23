// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewColumnDividerDoubleClickEventArgs : HandledMouseEventArgs
{
    public DataGridViewColumnDividerDoubleClickEventArgs(int columnIndex, HandledMouseEventArgs? e)
        : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, e?.X ?? 0, e?.Y ?? 0, e?.Delta ?? 0, e?.Handled ?? false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columnIndex, -1);
        ArgumentNullException.ThrowIfNull(e);

        ColumnIndex = columnIndex;
    }

    public int ColumnIndex { get; }
}
