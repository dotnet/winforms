// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms;

public class DataGridViewSortCompareEventArgsTests : IDisposable
{
    private readonly DataGridView _dataGridView;
    private readonly DataGridViewTextBoxColumn _column;

    public DataGridViewSortCompareEventArgsTests()
    {
        _dataGridView = new();
        _column = new DataGridViewTextBoxColumn { Index = 1 };
        _dataGridView.Columns.Add(_column);
    }

    public void Dispose()
    {
        _dataGridView.Dispose();
        _column.Dispose();
    }

    [Fact]
    public void DataGridViewSortCompareEventArgs_Constructor_InitializesProperties()
    {
        var cellValue1 = "Value1";
        var cellValue2 = "Value2";
        int rowIndex1 = 0;
        int rowIndex2 = 1;

        var e = new DataGridViewSortCompareEventArgs(_column, cellValue1, cellValue2, rowIndex1, rowIndex2);

        e.Column.Should().Be(_column);
        e.CellValue1.Should().Be(cellValue1);
        e.CellValue2.Should().Be(cellValue2);
        e.RowIndex1.Should().Be(rowIndex1);
        e.RowIndex2.Should().Be(rowIndex2);
        e.SortResult.Should().Be(0);
    }

    [Fact]
    public void DataGridViewSortCompareEventArgs_SortResult_Set_GetReturnsExpected()
    {
        var e = new DataGridViewSortCompareEventArgs(
            _column,
            cellValue1: null,
            cellValue2: null,
            rowIndex1: 0,
            rowIndex2: 1)
        {
            SortResult = 1
        };

        e.SortResult.Should().Be(1);
    }
}
