// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewCellCancelEventArgs : CancelEventArgs, IDataGridViewCellEventArgs
{
    internal DataGridViewCellCancelEventArgs(DataGridViewCell dataGridViewCell)
        : this(dataGridViewCell.ColumnIndex, dataGridViewCell.RowIndex)
    {
    }

    public DataGridViewCellCancelEventArgs(int columnIndex, int rowIndex)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columnIndex, -1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rowIndex, -1);

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    public int ColumnIndex { get; }

    public int RowIndex { get; }
}
