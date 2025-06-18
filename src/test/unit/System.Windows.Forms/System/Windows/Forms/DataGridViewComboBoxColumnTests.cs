// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class DataGridViewComboBoxColumnTests : IDisposable
{
    private readonly DataGridViewComboBoxColumn _dataGridViewComboBoxColumn;

    public void Dispose() => _dataGridViewComboBoxColumn?.Dispose();

    public DataGridViewComboBoxColumnTests() => _dataGridViewComboBoxColumn = new();

    [Fact]
    public void AutoComplete_DefaultValue_IsTrue() =>
        _dataGridViewComboBoxColumn.AutoComplete.Should().BeTrue();

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
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.AutoComplete = false;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.AutoComplete.Should().BeFalse();
        }
    }

    [Fact]
    public void AutoComplete_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.AutoComplete = false;
        _dataGridViewComboBoxColumn.AutoComplete = false;

        _dataGridViewComboBoxColumn.AutoComplete.Should().BeFalse();
    }

    [Fact]
    public void AutoComplete_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.AutoComplete; };
        Action set = () => _dataGridViewComboBoxColumn.AutoComplete = false;

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AutoComplete_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.AutoComplete = false;
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).AutoComplete.Should().BeFalse();
    }

    [Fact]
    public void DataSource_DefaultValue_IsNull() =>
        _dataGridViewComboBoxColumn.DataSource.Should().BeNull();

    [Fact]
    public void DataSource_SetValue_PropagatesToTemplate()
    {
        object dataSource = new[] { "A", "B" };
        _dataGridViewComboBoxColumn.DataSource = dataSource;

        _dataGridViewComboBoxColumn.DataSource.Should().BeSameAs(dataSource);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DataSource.Should().BeSameAs(dataSource);
    }

    [Fact]
    public void DataSource_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        object dataSource = new[] { "A", "B" };
        _dataGridViewComboBoxColumn.DataSource = dataSource;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.DataSource.Should().BeSameAs(dataSource);
        }
    }

    [Fact]
    public void DataSource_SetSameValue_DoesNotPropagate()
    {
        object dataSource = new[] { "A", "B" };
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.DataSource = dataSource;
        _dataGridViewComboBoxColumn.DataSource = dataSource;

        _dataGridViewComboBoxColumn.DataSource.Should().BeSameAs(dataSource);
    }

    [Fact]
    public void DataSource_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.DataSource; };
        Action set = () => _dataGridViewComboBoxColumn.DataSource = new[] { "A" };

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DataSource_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        object dataSource = new[] { "A", "B" };
        _dataGridViewComboBoxColumn.DataSource = dataSource;

        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DataSource.Should().BeSameAs(dataSource);
    }

    [Fact]
    public void DisplayMember_DefaultValue_IsEmpty() =>
        _dataGridViewComboBoxColumn.DisplayMember.Should().BeEmpty();

    [Fact]
    public void DisplayMember_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayMember = "Name";
        _dataGridViewComboBoxColumn.DisplayMember.Should().Be("Name");

        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayMember.Should().Be("Name");
    }

    [Fact]
    public void DisplayMember_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.DisplayMember = "Name";

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.DisplayMember.Should().Be("Name");
        }
    }

    [Fact]
    public void DisplayMember_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.DisplayMember = "Name";
        _dataGridViewComboBoxColumn.DisplayMember = "Name";

        _dataGridViewComboBoxColumn.DisplayMember.Should().Be("Name");
    }

    [Fact]
    public void DisplayMember_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.DisplayMember; };
        Action set = () => _dataGridViewComboBoxColumn.DisplayMember = "Name";

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DisplayMember_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayMember = "Name";
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayMember.Should().Be("Name");
    }

    [Fact]
    public void DisplayStyle_DefaultValue_IsDropDownButton() =>
        _dataGridViewComboBoxColumn.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.DropDownButton);

    [Fact]
    public void DisplayStyle_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
        _dataGridViewComboBoxColumn.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
    }

    [Fact]
    public void DisplayStyle_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
        }
    }

    [Fact]
    public void DisplayStyle_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
        _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;

        _dataGridViewComboBoxColumn.DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
    }

    [Fact]
    public void DisplayStyle_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.DisplayStyle; };
        Action set = () => _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DisplayStyle_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayStyle.Should().Be(DataGridViewComboBoxDisplayStyle.ComboBox);
    }

    [Fact]
    public void DisplayStyleForCurrentCellOnly_DefaultValue_IsFalse() =>
        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly.Should().BeFalse();

    [Fact]
    public void DisplayStyleForCurrentCellOnly_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;

        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly.Should().BeTrue();
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayStyleForCurrentCellOnly.Should().BeTrue();
    }

    [Fact]
    public void DisplayStyleForCurrentCellOnly_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.DisplayStyleForCurrentCellOnly.Should().BeTrue();
        }
    }

    [Fact]
    public void DisplayStyleForCurrentCellOnly_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.DisplayStyleForCurrentCellOnly = true;
        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;

        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly.Should().BeTrue();
    }

    [Fact]
    public void DisplayStyleForCurrentCellOnly_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly; };
        Action set = () => _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DisplayStyleForCurrentCellOnly_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.DisplayStyleForCurrentCellOnly = true;
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DisplayStyleForCurrentCellOnly.Should().BeTrue();
    }

    [Fact]
    public void DropDownWidth_DefaultValue_IsOne() =>
        _dataGridViewComboBoxColumn.DropDownWidth.Should().Be(1);

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
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.DropDownWidth = 77;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.DropDownWidth.Should().Be(77);
        }
    }

    [Fact]
    public void DropDownWidth_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.DropDownWidth = 42;
        _dataGridViewComboBoxColumn.DropDownWidth = 42;

        _dataGridViewComboBoxColumn.DropDownWidth.Should().Be(42);
    }

    [Fact]
    public void DropDownWidth_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.DropDownWidth; };
        Action set = () => _dataGridViewComboBoxColumn.DropDownWidth = 5;

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DropDownWidth_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.DropDownWidth = 99;
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).DropDownWidth.Should().Be(99);
    }

    [Fact]
    public void FlatStyle_DefaultValue_IsStandard() =>
        _dataGridViewComboBoxColumn.FlatStyle.Should().Be(FlatStyle.Standard);

    [Fact]
    public void FlatStyle_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Popup;
        _dataGridViewComboBoxColumn.FlatStyle.Should().Be(FlatStyle.Popup);
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).FlatStyle.Should().Be(FlatStyle.Popup);
    }

    [Fact]
    public void FlatStyle_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Flat;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.FlatStyle.Should().Be(FlatStyle.Flat);
        }
    }

    [Fact]
    public void FlatStyle_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.FlatStyle = FlatStyle.Popup;
        _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Popup;

        _dataGridViewComboBoxColumn.FlatStyle.Should().Be(FlatStyle.Popup);
    }

    [Fact]
    public void FlatStyle_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.FlatStyle; };
        Action set = () => _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.Flat;

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void FlatStyle_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.FlatStyle = FlatStyle.System;
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).FlatStyle.Should().Be(FlatStyle.System);
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

    [Fact]
    public void Items_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.Items; };

        get.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ValueMember_DefaultValue_IsEmpty() =>
        _dataGridViewComboBoxColumn.ValueMember.Should().BeEmpty();

    [Fact]
    public void ValueMember_SetValue_PropagatesToTemplate()
    {
        _dataGridViewComboBoxColumn.ValueMember = "Id";
        _dataGridViewComboBoxColumn.ValueMember.Should().Be("Id");
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).ValueMember.Should().Be("Id");
    }

    [Fact]
    public void ValueMember_SetValue_PropagatesToCellsInGrid()
    {
        using DataGridView dataGridView = new();
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.ValueMember = "Id";

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;
            cell.Should().NotBeNull();
            cell!.ValueMember.Should().Be("Id");
        }
    }

    [Fact]
    public void ValueMember_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.ValueMember = "Id";
        _dataGridViewComboBoxColumn.ValueMember = "Id";

        _dataGridViewComboBoxColumn.ValueMember.Should().Be("Id");
    }

    [Fact]
    public void ValueMember_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.ValueMember; };
        Action set = () => _dataGridViewComboBoxColumn.ValueMember = "Id";

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ValueMember_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.ValueMember = "Id";
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).ValueMember.Should().Be("Id");
    }

    [Fact]
    public void MaxDropDownItems_DefaultValue_IsExpected() =>
        _dataGridViewComboBoxColumn.MaxDropDownItems.Should().Be(DataGridViewComboBoxCell.DefaultMaxDropDownItems);

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
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.MaxDropDownItems = 12;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;

            cell.Should().NotBeNull();
            cell!.MaxDropDownItems.Should().Be(12);
        }
    }

    [Fact]
    public void MaxDropDownItems_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.MaxDropDownItems = 7;
        _dataGridViewComboBoxColumn.MaxDropDownItems = 7;

        _dataGridViewComboBoxColumn.MaxDropDownItems.Should().Be(7);
    }

    [Fact]
    public void MaxDropDownItems_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.MaxDropDownItems; };
        Action set = () => _dataGridViewComboBoxColumn.MaxDropDownItems = 5;

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MaxDropDownItems_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.MaxDropDownItems = 9;
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).MaxDropDownItems.Should().Be(9);
    }

    [Fact]
    public void Sorted_DefaultValue_IsFalse() =>
        _dataGridViewComboBoxColumn.Sorted.Should().BeFalse();

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
        dataGridView.Rows.Add();
        dataGridView.Rows.Add();

        _dataGridViewComboBoxColumn.Sorted = true;

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow)
                continue;
            DataGridViewComboBoxCell? cell = row.Cells[0] as DataGridViewComboBoxCell;

            cell.Should().NotBeNull();
            cell!.Sorted.Should().BeTrue();
        }
    }

    [Fact]
    public void Sorted_SetSameValue_DoesNotPropagate()
    {
        DataGridViewComboBoxCell cellTemplate = (DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!;
        cellTemplate.Sorted = true;
        _dataGridViewComboBoxColumn.Sorted = true;

        _dataGridViewComboBoxColumn.Sorted.Should().BeTrue();
    }

    [Fact]
    public void Sorted_ThrowsIfCellTemplateIsNull()
    {
        _dataGridViewComboBoxColumn.CellTemplate = null;
        Action get = () => { var _ = _dataGridViewComboBoxColumn.Sorted; };
        Action set = () => _dataGridViewComboBoxColumn.Sorted = true;

        get.Should().Throw<InvalidOperationException>();
        set.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Sorted_SetValue_WhenNotInGrid_OnlyAffectsTemplate()
    {
        _dataGridViewComboBoxColumn.Sorted = true;
        ((DataGridViewComboBoxCell)_dataGridViewComboBoxColumn.CellTemplate!).Sorted.Should().BeTrue();
    }

    [Fact]
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

        DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)_dataGridViewComboBoxColumn.Clone();

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

    [Fact]
    public void ToString_ReturnsExpectedFormat_WithNameAndIndex()
    {
        _dataGridViewComboBoxColumn.Name = "MyComboCol";
        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=MyComboCol, Index=-1 }");
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat_WhenInDataGridView()
    {
        using DataGridView dataGridView = new();
        _dataGridViewComboBoxColumn.Name = "Combo";
        dataGridView.Columns.Add(_dataGridViewComboBoxColumn);

        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=Combo, Index=0 }");
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat_WhenNameIsNullOrEmpty()
    {
        _dataGridViewComboBoxColumn.Name = "";
        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=, Index=-1 }");

        _dataGridViewComboBoxColumn.Name = null;
        _dataGridViewComboBoxColumn.ToString().Should().Be("DataGridViewComboBoxColumn { Name=, Index=-1 }");
    }
}
