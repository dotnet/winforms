// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class DataGridViewButtonColumnTests : IDisposable
{
    private readonly DataGridView _dataGridView;
    private readonly DataGridViewButtonColumn _column;

    public DataGridViewButtonColumnTests()
    {
        _dataGridView = new();
        _column = new();
    }

    public void Dispose()
    {
        _dataGridView?.Dispose();
        _column?.Dispose();
    }

    [Fact]
    public void DataGridViewButtonColumn_FlatStyle_GetSet_ReturnsExpected()
    {
        _column.FlatStyle.Should().Be(FlatStyle.Standard);

        _column.FlatStyle = FlatStyle.Flat;
        _column.FlatStyle.Should().Be(FlatStyle.Flat);

        _column.FlatStyle = FlatStyle.Popup;
        _column.FlatStyle.Should().Be(FlatStyle.Popup);

        _column.FlatStyle = FlatStyle.System;
        _column.FlatStyle.Should().Be(FlatStyle.System);
    }

    [Fact]
    public void DataGridViewButtonColumn_FlatStyle_SetWithDataGridView_UpdatesRows()
    {
        _dataGridView.Columns.Add(_column);

        using DataGridViewRow row = new();
        row.Cells.Add(new DataGridViewButtonCell());
        _dataGridView.Rows.Add(row);

        _column.FlatStyle = FlatStyle.Flat;
        ((DataGridViewButtonCell)row.Cells[0]).FlatStyle.Should().Be(FlatStyle.Flat);

        _column.FlatStyle = FlatStyle.Popup;
        ((DataGridViewButtonCell)row.Cells[0]).FlatStyle.Should().Be(FlatStyle.Popup);
    }

    [Fact]
    public void DataGridViewButtonColumn_FlatStyle_SetNullButtonCellTemplate_ThrowsInvalidOperationException()
    {
        _column.GetType().GetProperty("CellTemplate")!.SetValue(_column, null);

        Action action = () => { var flatStyle = _column.FlatStyle; };
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DataGridViewButtonColumn_Text_Set_GetReturnsExpected()
    {
        _column.Text.Should().BeNull();

        _column.Text = "Test";
        _column.Text.Should().Be("Test");
    }

    [Fact]
    public void DataGridViewButtonColumn_UseColumnTextForButtonValue_Set_GetReturnsExpected()
    {
        _column.UseColumnTextForButtonValue.Should().BeFalse();

        _column.UseColumnTextForButtonValue = true;
        _column.UseColumnTextForButtonValue.Should().BeTrue();
    }

    [Fact]
    public void DataGridViewButtonColumn_UseColumnTextForButtonValue_SetWithDataGridView_UpdatesRows()
    {
        _dataGridView.Columns.Add(_column);

        using DataGridViewRow row = new();
        using DataGridViewButtonCell cell = new();
        row.Cells.Add(cell);
        _dataGridView.Rows.Add(row);

        ((DataGridViewButtonCell)row.Cells[0]).UseColumnTextForButtonValue.Should().BeFalse();

        _column.UseColumnTextForButtonValue = true;
        ((DataGridViewButtonCell)row.Cells[0]).UseColumnTextForButtonValue.Should().BeTrue();
    }

    [Fact]
    public void DataGridViewButtonColumn_UseColumnTextForButtonValue_SetNullButtonCellTemplate_ThrowsInvalidOperationException()
    {
        _column.GetType().GetProperty("CellTemplate")!.SetValue(_column, null);

        Action action = () => { bool useColumnTextForButtonValue = _column.UseColumnTextForButtonValue; };
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DataGridViewButtonColumn_Clone_ReturnsExpected()
    {
        _column.Text = "Test";
        _column.FlatStyle = FlatStyle.Flat;
        _column.UseColumnTextForButtonValue = true;

        var clone = (DataGridViewButtonColumn)_column.Clone();
        clone.Text.Should().Be("Test");
        clone.FlatStyle.Should().Be(FlatStyle.Flat);
        clone.UseColumnTextForButtonValue.Should().BeTrue();
    }

    [Fact]
    public void DataGridViewButtonColumn_Clone_WithDifferentType_ReturnsExpected()
    {
        CustomDataGridViewButtonColumn customColumn = new()
        {
            Text = "Test",
            FlatStyle = FlatStyle.Flat,
            UseColumnTextForButtonValue = true
        };

        var clone = (CustomDataGridViewButtonColumn)customColumn.Clone();
        clone.Text.Should().Be("Test");
        clone.FlatStyle.Should().Be(FlatStyle.Flat);
        clone.UseColumnTextForButtonValue.Should().BeTrue();
    }

    [Fact]
    public void DataGridViewButtonColumn_ToString_ReturnsExpected()
    {
        _column.ToString().Should().Be("DataGridViewButtonColumn { Name=, Index=-1 }");

        _column.Name = "TestColumn";
        _column.ToString().Should().Be("DataGridViewButtonColumn { Name=TestColumn, Index=-1 }");
    }

    [Fact]
    public void DataGridViewButtonColumn_DefaultCellStyle_GetSet_ReturnsExpected()
    {
        DataGridViewCellStyle style = _column.DefaultCellStyle;
        style.Alignment.Should().Be(DataGridViewContentAlignment.MiddleCenter);

        DataGridViewCellStyle newStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
        _column.DefaultCellStyle = newStyle;
        _column.DefaultCellStyle.Alignment.Should().Be(DataGridViewContentAlignment.BottomRight);
    }

    [Fact]
    public void DataGridViewButtonColumn_CellTemplate_SetInvalidType_ThrowsInvalidCastException()
    {
        Action action = () => _column.CellTemplate = new DataGridViewTextBoxCell();
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void DataGridViewButtonColumn_CellTemplate_SetValidType_GetReturnsExpected()
    {
        using DataGridViewButtonCell cell = new();
        _column.CellTemplate = cell;
        _column.CellTemplate.Should().Be(cell);
    }

    [Fact]
    public void DataGridViewButtonColumn_Clone_CopiesProperties()
    {
        _column.Text = "Test";
        _column.FlatStyle = FlatStyle.Flat;
        _column.UseColumnTextForButtonValue = true;
        _column.DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };

        var clone = (DataGridViewButtonColumn)_column.Clone();
        clone.Text.Should().Be("Test");
        clone.FlatStyle.Should().Be(FlatStyle.Flat);
        clone.UseColumnTextForButtonValue.Should().BeTrue();
        clone.DefaultCellStyle.Alignment.Should().Be(DataGridViewContentAlignment.BottomRight);
    }

    [Fact]
    public void DataGridViewButtonColumn_ShouldSerializeDefaultCellStyle_ReturnsExpected()
    {
        dynamic accessor = _column.TestAccessor().Dynamic;

        bool result = accessor.ShouldSerializeDefaultCellStyle();
        result.Should().BeFalse();

        _column.DefaultCellStyle.BackColor = Color.Red;
        result = accessor.ShouldSerializeDefaultCellStyle();
        result.Should().BeTrue();
    }

    private class CustomDataGridViewButtonColumn : DataGridViewButtonColumn
    {
    }
}
