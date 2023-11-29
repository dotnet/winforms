// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewCellParsingEventArgs : ConvertEventArgs, IDataGridViewCellEventArgs
{
    public DataGridViewCellParsingEventArgs(
        int rowIndex,
        int columnIndex,
        object? value,
        Type? desiredType,
        DataGridViewCellStyle? inheritedCellStyle)
        : base(value, desiredType)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        InheritedCellStyle = inheritedCellStyle;
    }

    public int RowIndex { get; }

    public int ColumnIndex { get; }

    public DataGridViewCellStyle? InheritedCellStyle { get; set; }

    public bool ParsingApplied { get; set; }
}
