// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public class DataGridViewRowDividerDoubleClickEventArgs : HandledMouseEventArgs
{
    public DataGridViewRowDividerDoubleClickEventArgs(int rowIndex, HandledMouseEventArgs e)
        : base((e.OrThrowIfNull()).Button, e.Clicks, e.X, e.Y, e.Delta, e.Handled)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rowIndex, -1);

        RowIndex = rowIndex;
    }

    public int RowIndex { get; }
}
