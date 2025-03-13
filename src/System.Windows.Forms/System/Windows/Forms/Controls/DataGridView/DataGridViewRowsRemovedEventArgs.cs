// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewRowsRemovedEventArgs : EventArgs
{
    public DataGridViewRowsRemovedEventArgs(int rowIndex, int rowCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);
        ArgumentOutOfRangeException.ThrowIfLessThan(rowCount, 1);

        RowIndex = rowIndex;
        RowCount = rowCount;
    }

    public int RowIndex { get; }

    public int RowCount { get; }
}
