// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms;

public class DataGridViewTextBoxColumnTests : IDisposable
{
    private readonly DataGridView _dataGridView;
    private readonly DataGridViewTextBoxColumn _column;

    public DataGridViewTextBoxColumnTests()
    {
        _dataGridView = new();
        _column = new();
        _dataGridView.Columns.Add(_column);
    }

    public void Dispose()
    {
        _dataGridView.Dispose();
        _column.Dispose();
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_Constructor_Default()
    {
        _column.CellTemplate.Should().NotBeNull();
        _column.CellTemplate.Should().BeOfType<DataGridViewTextBoxCell>();
        _column.SortMode.Should().Be(DataGridViewColumnSortMode.Automatic);
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_CellTemplate_Set_GetReturnsExpected()
    {
        using DataGridViewTextBoxCell cell = new();

        _column.CellTemplate = cell;

        _column.CellTemplate.Should().Be(cell);
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_CellTemplate_SetInvalidType_ThrowsInvalidCastException()
    {
        using DataGridViewButtonCell cell = new();

        Action action = () => _column.CellTemplate = cell;
        action.Should().Throw<InvalidCastException>()
            .WithMessage(string.Format(SR.DataGridViewTypeColumn_WrongCellTemplateType, "System.Windows.Forms.DataGridViewTextBoxCell"));
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_MaxInputLength_GetSet()
    {
        _column.MaxInputLength = 100;

        _column.MaxInputLength.Should().Be(100);
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_MaxInputLength_SetUpdatesExistingCells()
    {
        _dataGridView.Rows.Add();
        _dataGridView.Rows.Add();

        _column.MaxInputLength = 100;

        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewTextBoxCell)row.Cells[0]).MaxInputLength.Should().Be(100);
        }
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_MaxInputLength_SetSameValue_DoesNotUpdateCells()
    {
        _dataGridView.Rows.Add();
        _dataGridView.Rows.Add();

        _column.MaxInputLength = 32767;

        foreach (DataGridViewRow row in _dataGridView.Rows)
        {
            ((DataGridViewTextBoxCell)row.Cells[0]).MaxInputLength.Should().Be(32767);
        }
    }

    [WinFormsFact]
    public static void DataGridViewTextBoxColumn_MaxInputLength_SetNull_DoesNotThrow()
    {
        DataGridView? dataGridView = null;
        DataGridViewTextBoxColumn column = new();
        dataGridView?.Columns.Add(column);

        Action action = () => column.MaxInputLength = 100;
        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_MaxInputLength_NullCellTemplate_ThrowsInvalidOperationException()
    {
        _column.CellTemplate = null;
        int maxInput = 0;

        Action action = () => maxInput = _column.MaxInputLength;

        action.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_SortMode_GetSet()
    {
        _column.SortMode = DataGridViewColumnSortMode.Programmatic;

        _column.SortMode.Should().Be(DataGridViewColumnSortMode.Programmatic);
    }

    [WinFormsFact]
    public void DataGridViewTextBoxColumn_ToString_ReturnsExpected()
    {
        _column.Name = "ColumnName";
        _column.Index = 1;

        _column.ToString().Should().Be("DataGridViewTextBoxColumn { Name=ColumnName, Index=1 }");
    }
}
