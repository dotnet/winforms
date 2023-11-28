// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewCellFormattingEventArgs : ConvertEventArgs, IDataGridViewCellEventArgs
{
    public DataGridViewCellFormattingEventArgs(
        int columnIndex,
        int rowIndex,
        object? value,
        Type? desiredType,
        DataGridViewCellStyle cellStyle)
        : base(value, desiredType)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(columnIndex, -1);
        ArgumentOutOfRangeException.ThrowIfLessThan(rowIndex, -1);

        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
        CellStyle = cellStyle;
    }

    public DataGridViewCellStyle CellStyle { get; set; }

    public int ColumnIndex { get; }

    public bool FormattingApplied { get; set; }

    public int RowIndex { get; }
}
