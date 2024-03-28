// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class DataGridViewCellErrorTextNeededEventArgsTests
{
    [WinFormsTheory]
    [StringWithNullData]
    public void DataGridViewCellErrorTextNeededEventArgs_ErrorText_Set_GetReturnsExpected(string value)
    {
        DataGridView dataGridView = new()
        {
            ColumnCount = 1,
            VirtualMode = true
        };
        DataGridViewCell cell = dataGridView.Rows[0].Cells[0];

        int callCount = 0;
        DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
        {
            callCount++;
            e.ErrorText = value;
            Assert.Equal(value, e.ErrorText);
        };
        dataGridView.CellErrorTextNeeded += handler;

        Assert.Same(value, cell.GetErrorText(0));
        Assert.Equal(1, callCount);
    }
}
