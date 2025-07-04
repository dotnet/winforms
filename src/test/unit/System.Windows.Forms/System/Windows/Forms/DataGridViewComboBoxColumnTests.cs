// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class DataGridViewComboBoxColumnTests : IDisposable
{
    private readonly DataGridViewComboBoxColumn _dataGridViewComboBoxColumn;

    public void Dispose() => _dataGridViewComboBoxColumn?.Dispose();

    public DataGridViewComboBoxColumnTests() => _dataGridViewComboBoxColumn = new();

    [WinFormsFact]
    public void DataGridViewComboBoxColumn_DefaultValues_AreExpected()
    {
        _dataGridViewComboBoxColumn.AutoComplete.Should().BeTrue();
        _dataGridViewComboBoxColumn.DisplayMember.Should().BeEmpty();
        _dataGridViewComboBoxColumn.DataSource.Should().BeNull();
        _dataGridViewComboBoxColumn.ValueMember.Should().BeEmpty();
        _dataGridViewComboBoxColumn.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.DropDownButton);
        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly.Should().BeFalse();
        _dataGridViewComboBoxColumn.DropDownWidth.Should().Be(1);
        _dataGridViewComboBoxColumn.FlatStyle.Should().Be(FlatStyle.Standard);
        _dataGridViewComboBoxColumn.MaxDropDownItems.Should().Be(DataGridViewComboBoxCell.DefaultMaxDropDownItems);
        _dataGridViewComboBoxColumn.Sorted.Should().BeFalse();
    }

    [Fact]
    public void AutoComplete_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.AutoComplete = false;
        _dataGridViewComboBoxColumn.AutoComplete.Should().BeFalse();
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).AutoComplete.Should().BeFalse();
    }

    [Fact]
    public void AutoComplete_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.AutoComplete = false;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.AutoComplete.Should().BeFalse();
        }
    }

    [WinFormsFact]
    public void DataSource_SetValue_PropagatesToTemplate()
    {
        object dataSource = new[] { "A", "B" };
        _dataGridViewComboBoxColumn.DataSource = dataSource;

        _dataGridViewComboBoxColumn.DataSource.Should().BeSameAs(dataSource);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DataSource.Should().BeSameAs(dataSource);
    }

    [WinFormsFact]
    public void DataSource_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        object dataSource = new[] { "A", "B" };
        _dataGridViewComboBoxColumn.DataSource = dataSource;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.DataSource.Should().BeSameAs(dataSource);
        }
    }

    [WinFormsFact]
    public void DisplayMember_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayMember = "Name";
        _dataGridViewComboBoxColumn.DisplayMember.Should().Be("Name");

        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayMember.Should().Be("Name");
    }

    [WinFormsFact]
    public void DisplayMember_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.DisplayMember = "Name";

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.DisplayMember.Should().Be("Name");
        }
    }

    [WinFormsFact]
    public void DisplayStyle_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
        _dataGridViewComboBoxColumn.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
    }

    [WinFormsFact]
    public void DisplayStyle_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
        }
    }

    [WinFormsFact]
    public void DisplayStyleForCurrentCellOnly_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;

        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly.Should().BeTrue();
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayStyleForCurrentCellOnly.Should().BeTrue();
    }

    [WinFormsFact]
    public void DisplayStyleForCurrentCellOnly_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.DisplayStyleForCurrentCellOnly.Should().BeTrue();
        }
    }

    [Fact]
    public void DropDownWidth_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.DropDownWidth = 123;

        _dataGridViewComboBoxColumn.DropDownWidth.Should().Be(123);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DropDownWidth.Should().Be(123);
    }

    [Fact]
    public void DropDownWidth_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.DropDownWidth = 77;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.DropDownWidth.Should().Be(77);
        }
    }

    [WinFormsFact]
    public void FlatStyle_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Popup;
        _dataGridViewComboBoxColumn.FlatStyle.Should().Be(FlatStyle.Popup);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).FlatStyle.Should().Be(FlatStyle.Popup);
    }

    [WinFormsFact]
    public void FlatStyle_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Flat;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.FlatStyle.Should().Be(FlatStyle.Flat);
        }
    }

    [Fact]
    public void Items_ReturnsTemplateItems()
    {
        var items = _dataGridViewComboBoxColumn.Items;
        items.Should().NotBeNull();
        items.Count.Should().Be(0);

        items.Add("A");
        items.Add("B");
        _dataGridViewComboBoxColumn.Items.Count.Should().Be(2);
        _dataGridViewComboBoxColumn.Items[0].Should().Be("A");
        _dataGridViewComboBoxColumn.Items[1].Should().Be("B");
    }

    [WinFormsFact]
    public void ValueMember_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.ValueMember = "Id";
        _dataGridViewComboBoxColumn.ValueMember.Should().Be("Id");
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).ValueMember.Should().Be("Id");
    }

    [WinFormsFact]
    public void ValueMember_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.ValueMember = "Id";

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell.ValueMember.Should().Be("Id");
        }
    }

    [Fact]
    public void MaxDropDownItems_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.MaxDropDownItems = 15;
        _dataGridViewComboBoxColumn.MaxDropDownItems.Should().Be(15);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).MaxDropDownItems.Should().Be(15);
    }

    [Fact]
    public void MaxDropDownItems_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.MaxDropDownItems = 12;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;

            cell.Should().NotBeNull();
            cell.MaxDropDownItems.Should().Be(12);
        }
    }

    [Fact]
    public void Sorted_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.Sorted = true;

        _dataGridViewComboBoxColumn.Sorted.Should().BeTrue();
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).Sorted.Should().BeTrue();
    }

    [Fact]
    public void Sorted_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add(2);

        _dataGridViewComboBoxColumn.Sorted = true;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            using DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;

            cell.Should().NotBeNull();
            cell.Sorted.Should().BeTrue();
        }
    }

    [WinFormsFact]
    public void ComboBoxColumnAllProperties_ThrowInvalidOperationException_IfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;

        Action getAutoComplete = () => { bool _ = _dataGridViewComboBoxColumn.AutoComplete; };
        Action setAutoComplete = () => _dataGridViewComboBoxColumn.AutoComplete = false;

        Action getDataSource = () => { object? _ = _dataGridViewComboBoxColumn.DataSource; };
        Action setDataSource = () => _dataGridViewComboBoxColumn.DataSource = new[] { "A" };

        Action getDisplayMember = () => { string _ = _dataGridViewComboBoxColumn.DisplayMember; };
        Action setDisplayMember = () => _dataGridViewComboBoxColumn.DisplayMember = "Name";

        Action getValueMember = () => { string _ = _dataGridViewComboBoxColumn.ValueMember; };
        Action setValueMember = () => _dataGridViewComboBoxColumn.ValueMember = "Id";

        Action getItems = () => { var _ = _dataGridViewComboBoxColumn.Items; };

        Action getDisplayStyle = () => { DataGridViewComboBoxDisplayStyle _ = _dataGridViewComboBoxColumn.DisplayStyle; };
        Action setDisplayStyle = () => _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;

        Action getDisplayStyleForCurrentCellOnly = () => { bool _ = _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly; };
        Action setDisplayStyleForCurrentCellOnly = () => _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;

        Action getDropDownWidth = () => { int _ = _dataGridViewComboBoxColumn.DropDownWidth; };
        Action setDropDownWidth = () => _dataGridViewComboBoxColumn.DropDownWidth = 5;

        Action getMaxDropDownItems = () => { int _ = _dataGridViewComboBoxColumn.MaxDropDownItems; };
        Action setMaxDropDownItems = () => _dataGridViewComboBoxColumn.MaxDropDownItems = 5;

        Action getSorted = () => { bool _ = _dataGridViewComboBoxColumn.Sorted; };
        Action setSorted = () => _dataGridViewComboBoxColumn.Sorted = true;

        Action getFlatStyle = () => { FlatStyle _ = _dataGridViewComboBoxColumn.FlatStyle; };
        Action setFlatStyle = () => _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Flat;

        getAutoComplete.Should().Throw<InvalidOperationException>();
        setAutoComplete.Should().Throw<InvalidOperationException>();

        getDataSource.Should().Throw<InvalidOperationException>();
        setDataSource.Should().Throw<InvalidOperationException>();

        getDisplayMember.Should().Throw<InvalidOperationException>();
        setDisplayMember.Should().Throw<InvalidOperationException>();

        getValueMember.Should().Throw<InvalidOperationException>();
        setValueMember.Should().Throw<InvalidOperationException>();

        getItems.Should().Throw<InvalidOperationException>();

        getDisplayStyle.Should().Throw<InvalidOperationException>();
        setDisplayStyle.Should().Throw<InvalidOperationException>();

        getDisplayStyleForCurrentCellOnly.Should().Throw<InvalidOperationException>();
        setDisplayStyleForCurrentCellOnly.Should().Throw<InvalidOperationException>();

        getDropDownWidth.Should().Throw<InvalidOperationException>();
        setDropDownWidth.Should().Throw<InvalidOperationException>();

        getMaxDropDownItems.Should().Throw<InvalidOperationException>();
        setMaxDropDownItems.Should().Throw<InvalidOperationException>();

        getSorted.Should().Throw<InvalidOperationException>();
        setSorted.Should().Throw<InvalidOperationException>();

        getFlatStyle.Should().Throw<InvalidOperationException>();
        setFlatStyle.Should().Throw<InvalidOperationException>();
    }

    [WinFormsFact]
    public void Clone_ReturnsDeepCopyWithSameProperties()
    {
        _dataGridViewComboBoxColumn.AutoComplete = false;
        _dataGridViewComboBoxColumn.DisplayMember = "Name";
        _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;
        _dataGridViewComboBoxColumn.DropDownWidth = 42;
        _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Flat;
        _dataGridViewComboBoxColumn.Items.Add("X");
        _dataGridViewComboBoxColumn.ValueMember = "Id";
        _dataGridViewComboBoxColumn.MaxDropDownItems = 5;
        _dataGridViewComboBoxColumn.Sorted = true;

        using DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)_dataGridViewComboBoxColumn.Clone();

        dataGridViewComboBoxColumn.Should().NotBeSameAs(_dataGridViewComboBoxColumn);
        dataGridViewComboBoxColumn.AutoComplete.Should().BeFalse();
        dataGridViewComboBoxColumn.DataSource.Should().BeSameAs(_dataGridViewComboBoxColumn.DataSource);
        dataGridViewComboBoxColumn.DisplayMember.Should().Be("Name");
        dataGridViewComboBoxColumn.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
        dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly.Should().BeTrue();
        dataGridViewComboBoxColumn.DropDownWidth.Should().Be(42);
        dataGridViewComboBoxColumn.FlatStyle.Should().Be(FlatStyle.Flat);
        dataGridViewComboBoxColumn.Items.Count.Should().Be(1);
        dataGridViewComboBoxColumn.Items[0].Should().Be("X");
        dataGridViewComboBoxColumn.ValueMember.Should().Be("Id");
        dataGridViewComboBoxColumn.MaxDropDownItems.Should().Be(5);
        dataGridViewComboBoxColumn.Sorted.Should().BeTrue();
        dataGridViewComboBoxColumn.CellTemplate.Should().NotBeNull();
        dataGridViewComboBoxColumn.CellTemplate.Should().NotBeSameAs(_dataGridViewComboBoxColumn.CellTemplate);
        ((DataGridViewComboBoxCell)dataGridViewComboBoxColumn.CellTemplate!).TemplateComboBoxColumn.Should().BeSameAs(dataGridViewComboBoxColumn);
    }

    [WinFormsFact]
    public void ToString_ReturnsExpectedFormat_WithNameAndIndex()
    {
        _dataGridViewComboBoxColumn.Name = "MyComboCol";
        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=MyComboCol, Index=-1 }");
    }

    [WinFormsFact]
    public void ToString_ReturnsExpectedFormat_WhenInDataGridView()
    {
        using DataGridView dataGridView = new();
        _dataGridViewComboBoxColumn.Name = "Combo";
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);

        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=Combo, Index=0 }");
    }

    [WinFormsFact]
    public void ToString_ReturnsExpectedFormat_WhenNameIsNullOrEmpty()
    {
        _dataGridViewComboBoxColumn.Name = "";
        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=, Index=-1 }");

        _dataGridViewComboBoxColumn.Name = null;
        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=, Index=-1 }");
    }
}
