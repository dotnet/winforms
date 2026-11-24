// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Data;

namespace System.Windows.Forms.Tests;

public partial class DataGridViewTests
{
    // Scenario: A DataTable bound with AutoGenerateColumns creates one column per data column, with the
    // column type, header text, property name and value type derived from the data column, and one row per record.
    [WinFormsFact]
    public void DataGridView_DataSource_DataTable_AutoGeneratesColumnsFromDescriptors()
    {
        using DataTable table = new();
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Age", typeof(int));
        table.Columns.Add("Active", typeof(bool));
        table.Rows.Add("A", 1, true);
        table.Rows.Add("B", 2, false);

        using DataGridView control = CreateBindingGrid();
        control.DataSource = table;

        Assert.True(control.AutoGenerateColumns);
        Assert.Equal(3, control.Columns.Count);
        Assert.IsType<DataGridViewTextBoxColumn>(control.Columns[0]);
        Assert.IsType<DataGridViewTextBoxColumn>(control.Columns[1]);
        Assert.IsType<DataGridViewCheckBoxColumn>(control.Columns[2]);
        Assert.Equal(["Name", "Age", "Active"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));
        Assert.Equal(["Name", "Age", "Active"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText));
        Assert.Equal(typeof(string), control.Columns[0].ValueType);
        Assert.Equal(typeof(int), control.Columns[1].ValueType);
        Assert.Equal(typeof(bool), control.Columns[2].ValueType);
        Assert.Equal(2, control.Rows.Count);
        Assert.Equal("B", control[0, 1].Value);
        Assert.Equal(2, control[1, 1].Value);
        Assert.Equal(false, control[2, 1].Value);
    }

    // Scenario: Binding a list of objects generates columns only for browsable public properties and marks
    // columns for read-only properties as read-only.
    [WinFormsFact]
    public void DataGridView_DataSource_ObjectList_SkipsNonBrowsableAndMarksReadOnlyProperties()
    {
        BindingList<BoundItem> items = [new() { Name = "A", Count = 3 }];

        using DataGridView control = CreateBindingGrid();
        control.DataSource = items;

        Assert.Equal(["Name", "Count", "ReadOnlyValue"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));
        Assert.False(control.Columns["Name"].ReadOnly);
        Assert.False(control.Columns["Count"].ReadOnly);
        Assert.True(control.Columns["ReadOnlyValue"].ReadOnly);
        Assert.Equal("A", control[0, 0].Value);
        Assert.Equal(3, control[1, 0].Value);
        Assert.Equal("fixed", control[2, 0].Value);
    }

    // Scenario: With AutoGenerateColumns disabled, binding creates no columns of its own; explicit columns are bound
    // through DataPropertyName and unmapped columns stay in place with empty cells.
    [WinFormsFact]
    public void DataGridView_DataSource_AutoGenerateColumnsFalse_UsesExplicitColumnMapping()
    {
        using DataTable table = new();
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Age", typeof(int));
        table.Rows.Add("A", 1);

        using DataGridView control = CreateBindingGrid();
        control.AutoGenerateColumns = false;
        control.Columns.Add(new DataGridViewTextBoxColumn { Name = "AgeColumn", DataPropertyName = "Age" });
        control.Columns.Add(new DataGridViewTextBoxColumn { Name = "Unmapped" });
        control.Columns.Add(new DataGridViewTextBoxColumn { Name = "Missing", DataPropertyName = "NoSuchProperty" });

        control.DataSource = table;

        Assert.Equal(["AgeColumn", "Unmapped", "Missing"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.Name));
        Assert.Equal(1, control.Rows.Count);
        Assert.Equal(1, control[0, 0].Value);
        Assert.Null(control[1, 0].Value);
        Assert.Null(control[2, 0].Value);
    }

    // Scenario: Assigning a data source raises DataBindingComplete once, with the Reset change type,
    // after the rows have already been populated.
    [WinFormsFact]
    public void DataGridView_DataSource_Assigned_RaisesDataBindingCompleteAfterRowsPopulated()
    {
        BindingList<BoundItem> items = [new() { Name = "A" }, new() { Name = "B" }];
        using DataGridView control = CreateBindingGrid();
        List<(ListChangedType Type, int Rows, int Columns)> calls = [];
        control.DataBindingComplete += (sender, e) =>
        {
            Assert.Same(control, sender);
            calls.Add((e.ListChangedType, control.Rows.Count, control.Columns.Count));
        };

        control.DataSource = items;

        Assert.Equal([(ListChangedType.Reset, 2, 3)], calls);
    }

    // Scenario: Rebinding a populated grid to another source replaces the generated columns and rows
    // with those of the new source.
    [WinFormsFact]
    public void DataGridView_DataSource_Replaced_RegeneratesColumnsAndRows()
    {
        using DataTable first = new();
        first.Columns.Add("A");
        first.Rows.Add("1");
        first.Rows.Add("2");

        using DataTable second = new();
        second.Columns.Add("X");
        second.Columns.Add("Y");
        second.Rows.Add("x", "y");

        using DataGridView control = CreateBindingGrid();
        control.DataSource = first;
        Assert.Equal(2, control.Rows.Count);

        control.DataSource = second;

        Assert.Equal(["X", "Y"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));
        Assert.Equal(1, control.Rows.Count);
        Assert.Equal("y", control[1, 0].Value);
    }

    // Scenario: Clearing the data source removes the generated columns and all bound rows.
    [WinFormsFact]
    public void DataGridView_DataSource_SetToNull_ClearsGeneratedColumnsAndRows()
    {
        using DataTable table = new();
        table.Columns.Add("A");
        table.Rows.Add("1");

        using DataGridView control = CreateBindingGrid();
        control.DataSource = table;
        Assert.Equal(1, control.Rows.Count);

        control.DataSource = null;

        Assert.Null(control.DataSource);
        Assert.Empty(control.Columns);
        Assert.Empty(control.Rows);
    }

    // Scenario: Clearing the data source of a grid with explicit columns keeps those columns and drops the rows.
    [WinFormsFact]
    public void DataGridView_DataSource_SetToNull_KeepsExplicitColumns()
    {
        using DataTable table = new();
        table.Columns.Add("A");
        table.Rows.Add("1");

        using DataGridView control = CreateBindingGrid();
        control.AutoGenerateColumns = false;
        DataGridViewTextBoxColumn explicitColumn = new() { Name = "Column", DataPropertyName = "A" };
        control.Columns.Add(explicitColumn);
        control.DataSource = table;
        Assert.Equal(1, control.Rows.Count);

        control.DataSource = null;

        Assert.Same(explicitColumn, Assert.Single(control.Columns));
        Assert.Empty(control.Rows);
    }

    // Scenario: DataMember selects the table within a multi-table DataSet; changing it rebinds the grid
    // to the other table's columns and rows.
    [WinFormsFact]
    public void DataGridView_DataMember_SelectsAndSwitchesTableInDataSet()
    {
        using DataSet dataSet = new();
        DataTable parents = dataSet.Tables.Add("Parents");
        parents.Columns.Add("ParentName");
        parents.Rows.Add("P1");
        parents.Rows.Add("P2");
        DataTable children = dataSet.Tables.Add("Children");
        children.Columns.Add("ChildName");
        children.Columns.Add("ChildAge", typeof(int));
        children.Rows.Add("C1", 5);

        using DataGridView control = CreateBindingGrid();
        control.DataSource = dataSet;
        control.DataMember = "Parents";

        Assert.Equal(["ParentName"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));
        Assert.Equal(2, control.Rows.Count);

        control.DataMember = "Children";

        Assert.Equal("Children", control.DataMember);
        Assert.Equal(["ChildName", "ChildAge"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));
        Assert.Equal(1, control.Rows.Count);
        Assert.Equal(5, control[1, 0].Value);
    }

    // Scenario: A DataMember that does not exist in the data source is rejected with an explicit error.
    [WinFormsFact]
    public void DataGridView_DataMember_NotInDataSource_Throws()
    {
        using DataSet dataSet = new();
        dataSet.Tables.Add("Parents").Columns.Add("ParentName");

        using DataGridView control = CreateBindingGrid();
        control.DataSource = dataSet;

        Assert.ThrowsAny<ArgumentException>(() => control.DataMember = "Missing");
    }

    // Scenario: Adding and removing items in the bound list adds and removes grid rows and reports
    // each change through DataBindingComplete with the matching change type.
    [WinFormsFact]
    public void DataGridView_BoundList_AddAndRemove_UpdatesRowsAndReportsChangeTypes()
    {
        BindingList<BoundItem> items = [new() { Name = "A" }];
        using DataGridView control = CreateBindingGrid();
        control.DataSource = items;
        List<ListChangedType> changes = [];
        List<int> rowCounts = [];
        control.DataBindingComplete += (sender, e) =>
        {
            changes.Add(e.ListChangedType);
            rowCounts.Add(control.Rows.Count);
        };

        items.Add(new() { Name = "B" });
        items.Add(new() { Name = "C" });
        items.RemoveAt(0);

        Assert.Equal([ListChangedType.ItemAdded, ListChangedType.ItemAdded, ListChangedType.ItemDeleted], changes);
        Assert.Equal([2, 3, 2], rowCounts);
        Assert.Equal("B", control[0, 0].Value);
        Assert.Equal("C", control[0, 1].Value);
    }

    // Scenario: With the new-row placeholder enabled, the grid keeps exactly one extra row beyond the bound
    // records as items are added to and removed from the list.
    [WinFormsFact]
    public void DataGridView_BoundList_AddAndRemove_KeepsNewRowPlaceholder()
    {
        BindingList<BoundItem> items = [new() { Name = "A" }];
        using DataGridView control = CreateBindingGrid();
        control.AllowUserToAddRows = true;
        control.DataSource = items;

        Assert.Equal(2, control.Rows.Count);
        Assert.Equal(1, control.NewRowIndex);

        items.Add(new() { Name = "B" });

        Assert.Equal(3, control.Rows.Count);
        Assert.Equal(2, control.NewRowIndex);

        items.RemoveAt(0);
        items.RemoveAt(0);

        Assert.Equal(1, control.Rows.Count);
        Assert.Equal(0, control.NewRowIndex);
    }

    // Scenario: A property change on a bound item refreshes the displayed cell value without rebuilding rows.
    [WinFormsFact]
    public void DataGridView_BoundList_ItemPropertyChanged_UpdatesCellValue()
    {
        BoundItem item = new() { Name = "A" };
        BindingList<BoundItem> items = [item];
        using DataGridView control = CreateBindingGrid();
        control.DataSource = items;
        DataGridViewRow row = control.Rows[0];
        List<ListChangedType> changes = [];
        control.DataBindingComplete += (sender, e) => changes.Add(e.ListChangedType);

        item.Name = "Changed";

        Assert.Equal([ListChangedType.ItemChanged], changes);
        Assert.Equal("Changed", control[0, 0].Value);
        Assert.Same(row, control.Rows[0]);
    }

    // Scenario: BindingSource.ResetBindings(false) after the underlying list changed without notifications
    // refreshes the grid rows and raises DataBindingComplete with the Reset change type.
    [WinFormsFact]
    public void DataGridView_BindingSource_ResetBindings_RefreshesRows()
    {
        List<BoundItem> list = [new() { Name = "A" }];
        using BindingSource source = new() { DataSource = list };
        using DataGridView control = CreateBindingGrid();
        control.DataSource = source;
        Assert.Equal(1, control.Rows.Count);

        list.Add(new() { Name = "B" });
        list.Add(new() { Name = "C" });
        List<ListChangedType> changes = [];
        control.DataBindingComplete += (sender, e) => changes.Add(e.ListChangedType);

        source.ResetBindings(metadataChanged: false);

        Assert.Equal(3, control.Rows.Count);
        Assert.Equal("C", control[0, 2].Value);
        Assert.Equal([ListChangedType.Reset], changes);
        Assert.Equal(["Name", "Count", "ReadOnlyValue"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));
    }

    // Scenario: Resetting bindings with changed metadata regenerates auto-generated columns from the new
    // property descriptors.
    [WinFormsFact]
    public void DataGridView_BindingSource_ResetBindingsWithMetadata_RegeneratesGeneratedColumns()
    {
        using DataTable table = new();
        table.Columns.Add("A");
        table.Rows.Add("1");
        using BindingSource source = new() { DataSource = table };
        using DataGridView control = CreateBindingGrid();
        control.DataSource = source;
        Assert.Equal(["A"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));

        // Suppress notifications so only the explicit reset can make the grid see the new column.
        source.RaiseListChangedEvents = false;
        table.Columns.Add("B");
        source.RaiseListChangedEvents = true;
        Assert.Equal(["A"], control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));

        source.ResetBindings(metadataChanged: true);

        Assert.Equal(["A", "B"],  control.Columns.Cast<DataGridViewColumn>().Select(c => c.DataPropertyName));
        Assert.Equal(1, control.Rows.Count);
    }

    // Scenario: Resetting bindings with changed metadata keeps explicitly defined columns, including their
    // custom header text, and does not add generated columns.
    [WinFormsFact]
    public void DataGridView_BindingSource_ResetBindingsWithMetadata_RetainsExplicitColumns()
    {
        using DataTable table = new();
        table.Columns.Add("A");
        table.Rows.Add("1");
        using BindingSource source = new() { DataSource = table };
        using DataGridView control = CreateBindingGrid();
        control.AutoGenerateColumns = false;
        DataGridViewTextBoxColumn explicitColumn = new() { Name = "Explicit", DataPropertyName = "A", HeaderText = "Custom header" };
        control.Columns.Add(explicitColumn);
        control.DataSource = source;

        table.Columns.Add("B");
        source.ResetBindings(metadataChanged: true);

        Assert.Same(explicitColumn, Assert.Single(control.Columns));
        Assert.Equal("Custom header", explicitColumn.HeaderText);
        Assert.Equal("1", control[0, 0].Value);
    }

    // Scenario: Moving the binding source position moves the grid's current cell to the same row, and
    // moving the current cell moves the binding source position.
    [WinFormsFact]
    public void DataGridView_BindingSource_Position_SynchronizesWithCurrentCell()
    {
        BindingList<BoundItem> items = [new() { Name = "A" }, new() { Name = "B" }, new() { Name = "C" }];
        using BindingSource source = new() { DataSource = items };
        using DataGridView control = CreateBindingGrid();
        control.DataSource = source;

        source.Position = 2;

        Assert.Equal(2, control.CurrentCell.RowIndex);
        Assert.Same(control.Rows[2], control.CurrentRow);

        control.CurrentCell = control[0, 1];

        Assert.Equal(1, source.Position);
        Assert.Same(items[1], source.Current);
    }

    // Scenario: Removing the current record from the binding source leaves the grid on a valid current
    // row that matches the binding source position.
    [WinFormsFact]
    public void DataGridView_BindingSource_RemoveCurrent_KeepsCurrentRowInSync()
    {
        BindingList<BoundItem> items = [new() { Name = "A" }, new() { Name = "B" }, new() { Name = "C" }];
        using BindingSource source = new() { DataSource = items };
        using DataGridView control = CreateBindingGrid();
        control.DataSource = source;
        source.Position = 1;

        source.RemoveCurrent();

        Assert.Equal(2, control.Rows.Count);
        Assert.Equal(source.Position, control.CurrentCell.RowIndex);
        Assert.Same(source.Current, items[control.CurrentCell.RowIndex]);
    }

    // Scenario: Binding an empty list keeps the generated columns but produces no rows, and rows appear when
    // items are added later.
    [WinFormsFact]
    public void DataGridView_DataSource_EmptyList_HasColumnsWithoutRows()
    {
        BindingList<BoundItem> items = [];
        using DataGridView control = CreateBindingGrid();
        control.DataSource = items;

        Assert.Equal(3, control.Columns.Count);
        Assert.Empty(control.Rows);

        items.Add(new() { Name = "A" });

        Assert.Equal(1, control.Rows.Count);
    }

    private static DataGridView CreateBindingGrid() => new()
    {
        AllowUserToAddRows = false,
        BindingContext = new BindingContext()
    };

    private class BoundItem : INotifyPropertyChanged
    {
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                PropertyChanged?.Invoke(this, new(nameof(Name)));
            }
        }

        public int Count { get; set; }

        public string ReadOnlyValue => "fixed";

        [Browsable(false)]
        public string Hidden { get; set; }
    }
}
