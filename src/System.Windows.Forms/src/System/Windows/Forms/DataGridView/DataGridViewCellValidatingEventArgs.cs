// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public class DataGridViewCellValidatingEventArgs : CancelEventArgs, IDataGridViewCellEventArgs
{
    internal DataGridViewCellValidatingEventArgs(int columnIndex, int rowIndex, object? formattedValue)
    {
        ColumnIndex = columnIndex;
        RowIndex = rowIndex;
        FormattedValue = formattedValue;
    }

    public int ColumnIndex { get; }

    public int RowIndex { get; }

    public object? FormattedValue { get; }
}
