// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewCellMouseEventArgs : MouseEventArgs, IDataGridViewCellEventArgs
{
    public DataGridViewCellMouseEventArgs(
        int columnIndex,
        int rowIndex,
        int localX,
        int localY,
        MouseEventArgs? e)
        : base(e?.Button ?? MouseButtons.None, e?.Clicks ?? 0, localX, localY, e?.Delta ?? 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columnIndex, -1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rowIndex, -1);
        ArgumentNullException.ThrowIfNull(e);

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    public int ColumnIndex { get; }

    public int RowIndex { get; }
}
