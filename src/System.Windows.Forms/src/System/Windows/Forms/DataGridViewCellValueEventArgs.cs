// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewCellValueEventArgs : EventArgs, IDataGridViewCellEventArgs
{
    internal DataGridViewCellValueEventArgs()
    {
        ColumnIndex = -1;
        RowIndex = -1;
    }

    public DataGridViewCellValueEventArgs(int columnIndex, int rowIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(columnIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(rowIndex);

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
    }

    public int ColumnIndex { get; private set; }

    public int RowIndex { get; private set; }

    public object? Value { get; set; }

    internal void SetProperties(int columnIndex, int rowIndex, object? value)
    {
        Debug.Assert(columnIndex >= -1);
        Debug.Assert(rowIndex >= -1);
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
        Value = value;
    }
}
