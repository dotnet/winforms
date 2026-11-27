// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class DataGridViewTests
{
    private static DataGridView CreateSelectionGrid(
        DataGridViewSelectionMode selectionMode,
        int columnCount = 3,
        int rowCount = 3,
        bool multiSelect = true)
    {
        DataGridView grid = new()
        {
            AllowUserToAddRows = false,
            ColumnCount = columnCount,
            RowCount = rowCount
        };

        // Automatic sort columns are not allowed with column selection modes.
        foreach (DataGridViewColumn column in grid.Columns)
        {
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        grid.SelectionMode = selectionMode;
        grid.MultiSelect = multiSelect;
        grid.ClearSelection();
        return grid;
    }

    private static List<(int Column, int Row)> GetSelectedCellAddresses(DataGridView grid)
    {
        List<(int Column, int Row)> addresses = [];
        foreach (DataGridViewCell cell in grid.SelectedCells)
        {
            addresses.Add((cell.ColumnIndex, cell.RowIndex));
        }

        addresses.Sort();
        return addresses;
    }

    // Scenario: In CellSelect mode with MultiSelect on, individual cells are selected and
    // deselected independently and the row/column selection collections stay empty.
    [WinFormsFact]
    public void DataGridView_CellSelect_MultiSelect_SelectsIndividualCells()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);

        grid[0, 0].Selected = true;
        grid[2, 1].Selected = true;

        Assert.Equal([(0, 0), (2, 1)], GetSelectedCellAddresses(grid));
        Assert.Empty(grid.SelectedRows);
        Assert.Empty(grid.SelectedColumns);

        grid[0, 0].Selected = false;

        Assert.Equal([(2, 1)], GetSelectedCellAddresses(grid));
        Assert.False(grid[0, 0].Selected);
        Assert.True(grid[2, 1].Selected);
    }

    // Scenario: With MultiSelect off, selecting a second cell replaces the first, and SelectAll
    // does not select anything.
    [WinFormsFact]
    public void DataGridView_CellSelect_MultiSelectFalse_KeepsSingleSelection()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect, multiSelect: false);

        grid[0, 0].Selected = true;
        grid[1, 1].Selected = true;

        Assert.Equal([(1, 1)], GetSelectedCellAddresses(grid));

        grid.ClearSelection();
        grid.SelectAll();

        Assert.Empty(grid.SelectedCells);
    }

    // Scenario: Turning MultiSelect off while several cells are selected is accepted and later
    // selections are limited to a single cell.
    [WinFormsFact]
    public void DataGridView_MultiSelect_ChangedToFalse_LimitsSubsequentSelection()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        grid[0, 0].Selected = true;
        grid[1, 1].Selected = true;

        grid.MultiSelect = false;
        grid[2, 2].Selected = true;

        Assert.Equal([(2, 2)], GetSelectedCellAddresses(grid));
    }

    // Scenario: In CellSelect mode, assigning CurrentCell selects only that cell, moving it moves
    // the selection, and clearing it resets the current address.
    [WinFormsFact]
    public void DataGridView_CellSelect_CurrentCell_DrivesSelection()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);

        grid.CurrentCell = grid[1, 1];

        Assert.Same(grid[1, 1], grid.CurrentCell);
        Assert.Equal(new Point(1, 1), grid.CurrentCellAddress);
        Assert.Same(grid.Rows[1], grid.CurrentRow);
        Assert.Equal([(1, 1)], GetSelectedCellAddresses(grid));

        grid.CurrentCell = grid[2, 0];

        Assert.Equal(new Point(2, 0), grid.CurrentCellAddress);
        Assert.Equal([(2, 0)], GetSelectedCellAddresses(grid));

        grid.CurrentCell = null;

        Assert.Null(grid.CurrentCell);
        Assert.Equal(new Point(-1, -1), grid.CurrentCellAddress);
        Assert.Null(grid.CurrentRow);
    }

    // Scenario: In FullRowSelect mode, the current cell selects its whole row, every cell in the
    // row reports as selected and column selection stays empty.
    [WinFormsFact]
    public void DataGridView_FullRowSelect_CurrentCell_SelectsWholeRow()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.FullRowSelect);

        grid.CurrentCell = grid[1, 2];

        Assert.Same(grid.Rows[2], Assert.Single(grid.SelectedRows));
        Assert.Equal([(0, 2), (1, 2), (2, 2)], GetSelectedCellAddresses(grid));
        Assert.Empty(grid.SelectedColumns);
        Assert.True(grid[0, 2].Selected);
        Assert.False(grid[0, 1].Selected);
    }

    // Scenario: In FullRowSelect mode with MultiSelect on, extra rows can be added to the selection
    // and removed again, and the selected cells follow the selected rows.
    [WinFormsFact]
    public void DataGridView_FullRowSelect_MultiSelect_SelectsAndDeselectsRows()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.FullRowSelect);
        grid.CurrentCell = grid[0, 0];

        grid.Rows[2].Selected = true;

        Assert.Equal(2, grid.SelectedRows.Count);
        Assert.Contains(grid.Rows[0], grid.SelectedRows.Cast<DataGridViewRow>());
        Assert.Contains(grid.Rows[2], grid.SelectedRows.Cast<DataGridViewRow>());
        Assert.Equal(6, grid.SelectedCells.Count);

        grid.Rows[0].Selected = false;

        Assert.Same(grid.Rows[2], Assert.Single(grid.SelectedRows));
        Assert.Equal([(0, 2), (1, 2), (2, 2)], GetSelectedCellAddresses(grid));
    }

    // Scenario: In FullRowSelect mode with MultiSelect off, selecting another row replaces the
    // previous selection.
    [WinFormsFact]
    public void DataGridView_FullRowSelect_MultiSelectFalse_ReplacesSelectedRow()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.FullRowSelect, multiSelect: false);
        grid.CurrentCell = grid[0, 0];

        grid.Rows[2].Selected = true;

        Assert.Same(grid.Rows[2], Assert.Single(grid.SelectedRows));
        Assert.Equal(3, grid.SelectedCells.Count);
    }

    // Scenario: In FullColumnSelect mode, the current cell selects its whole column, every cell in
    // the column reports as selected and row selection stays empty.
    [WinFormsFact]
    public void DataGridView_FullColumnSelect_CurrentCell_SelectsWholeColumn()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.FullColumnSelect);

        grid.CurrentCell = grid[1, 2];

        Assert.Same(grid.Columns[1], Assert.Single(grid.SelectedColumns));
        Assert.Equal([(1, 0), (1, 1), (1, 2)], GetSelectedCellAddresses(grid));
        Assert.Empty(grid.SelectedRows);
        Assert.True(grid[1, 0].Selected);
        Assert.False(grid[0, 0].Selected);
    }

    // Scenario: In FullColumnSelect mode with MultiSelect on, extra columns can be added to and
    // removed from the selection.
    [WinFormsFact]
    public void DataGridView_FullColumnSelect_MultiSelect_SelectsAndDeselectsColumns()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.FullColumnSelect);
        grid.CurrentCell = grid[0, 0];

        grid.Columns[2].Selected = true;

        Assert.Equal(2, grid.SelectedColumns.Count);
        Assert.Equal(6, grid.SelectedCells.Count);

        grid.Columns[0].Selected = false;

        Assert.Same(grid.Columns[2], Assert.Single(grid.SelectedColumns));
        Assert.Equal([(2, 0), (2, 1), (2, 2)], GetSelectedCellAddresses(grid));
    }

    // Scenario: In RowHeaderSelect mode, a selected row selects all its cells while individual cells
    // elsewhere can still be selected without selecting their rows.
    [WinFormsFact]
    public void DataGridView_RowHeaderSelect_MixesRowAndCellSelection()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.RowHeaderSelect);

        grid.Rows[1].Selected = true;
        grid[2, 2].Selected = true;

        Assert.Same(grid.Rows[1], Assert.Single(grid.SelectedRows));
        Assert.Equal([(0, 1), (1, 1), (2, 1), (2, 2)], GetSelectedCellAddresses(grid));
        Assert.Empty(grid.SelectedColumns);
    }

    // Scenario: In ColumnHeaderSelect mode, a selected column selects all its cells while individual
    // cells elsewhere can still be selected without selecting their columns.
    [WinFormsFact]
    public void DataGridView_ColumnHeaderSelect_MixesColumnAndCellSelection()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.ColumnHeaderSelect);

        grid.Columns[1].Selected = true;
        grid[2, 2].Selected = true;

        Assert.Same(grid.Columns[1], Assert.Single(grid.SelectedColumns));
        Assert.Equal([(1, 0), (1, 1), (1, 2), (2, 2)], GetSelectedCellAddresses(grid));
        Assert.Empty(grid.SelectedRows);
    }

    // Scenario: Setting Selected on a row while the grid uses a cell or column selection mode is
    // ignored rather than changing the selection.
    [WinFormsTheory]
    [InlineData(DataGridViewSelectionMode.CellSelect)]
    [InlineData(DataGridViewSelectionMode.FullColumnSelect)]
    [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect)]
    public void DataGridView_RowSelected_IgnoredWhenModeHasNoRowSelection(DataGridViewSelectionMode mode)
    {
        using DataGridView grid = CreateSelectionGrid(mode);

        grid.Rows[1].Selected = true;

        Assert.False(grid.Rows[1].Selected);
        Assert.Empty(grid.SelectedRows);
        Assert.Empty(grid.SelectedCells);
    }

    // Scenario: Setting Selected on a column while the grid uses a cell or row selection mode is
    // ignored rather than changing the selection.
    [WinFormsTheory]
    [InlineData(DataGridViewSelectionMode.CellSelect)]
    [InlineData(DataGridViewSelectionMode.FullRowSelect)]
    [InlineData(DataGridViewSelectionMode.RowHeaderSelect)]
    public void DataGridView_ColumnSelected_IgnoredWhenModeHasNoColumnSelection(DataGridViewSelectionMode mode)
    {
        using DataGridView grid = CreateSelectionGrid(mode);

        grid.Columns[1].Selected = true;

        Assert.False(grid.Columns[1].Selected);
        Assert.Empty(grid.SelectedColumns);
        Assert.Empty(grid.SelectedCells);
    }

    // Scenario: A row, column or cell that has not been added to a grid cannot be marked selected,
    // while deselecting it is harmless.
    [WinFormsFact]
    public void DataGridView_Selected_BeforeAddingToGrid_Throws()
    {
        using DataGridViewTextBoxCell cell = new();
        using DataGridViewRow row = new();
        using DataGridViewTextBoxColumn column = new();

        Assert.Throws<InvalidOperationException>(() => cell.Selected = true);
        Assert.Throws<InvalidOperationException>(() => row.Selected = true);
        Assert.Throws<InvalidOperationException>(() => column.Selected = true);

        cell.Selected = false;
        row.Selected = false;
        column.Selected = false;
        Assert.False(cell.Selected);
        Assert.False(row.Selected);
        Assert.False(column.Selected);
    }

    // Scenario: Changing SelectionMode discards the existing selection and raises SelectionChanged,
    // while assigning the same mode leaves the selection alone.
    [WinFormsFact]
    public void DataGridView_SelectionMode_Changed_ClearsSelection()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        grid[0, 0].Selected = true;
        grid[1, 1].Selected = true;

        grid.SelectionMode = DataGridViewSelectionMode.CellSelect;

        Assert.Equal(2, grid.SelectedCells.Count);

        int selectionChangedCount = 0;
        grid.SelectionChanged += (sender, e) => selectionChangedCount++;

        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        Assert.Empty(grid.SelectedCells);
        Assert.Empty(grid.SelectedRows);
        Assert.Equal(1, selectionChangedCount);
    }

    // Scenario: Switching to full-column selection is rejected while any column uses automatic sort
    // mode, and the previous mode is kept; once the columns are not sortable it succeeds.
    [WinFormsFact]
    public void DataGridView_FullColumnSelect_WithAutomaticSortColumn_Throws()
    {
        using DataGridView grid = new()
        {
            AllowUserToAddRows = false,
            ColumnCount = 2,
            RowCount = 2
        };

        Assert.Equal(DataGridViewColumnSortMode.Automatic, grid.Columns[0].SortMode);
        Assert.Throws<InvalidOperationException>(() => grid.SelectionMode = DataGridViewSelectionMode.FullColumnSelect);
        Assert.Throws<InvalidOperationException>(() => grid.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect);
        Assert.Equal(DataGridViewSelectionMode.RowHeaderSelect, grid.SelectionMode);

        grid.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        grid.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        grid.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

        Assert.Equal(DataGridViewSelectionMode.FullColumnSelect, grid.SelectionMode);
        Assert.Throws<InvalidOperationException>(() => grid.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic);
    }

    // Scenario: An undefined SelectionMode value is rejected.
    [WinFormsFact]
    public void DataGridView_SelectionMode_InvalidValue_ThrowsInvalidEnumArgumentException()
    {
        using DataGridView grid = new();

        Assert.Throws<InvalidEnumArgumentException>(() => grid.SelectionMode = (DataGridViewSelectionMode)100);
    }

    // Scenario: SelectAll selects cells, rows or columns according to the selection mode, and
    // ClearSelection removes everything again.
    [WinFormsTheory]
    [InlineData(DataGridViewSelectionMode.CellSelect, 9, 0, 0)]
    [InlineData(DataGridViewSelectionMode.FullRowSelect, 9, 3, 0)]
    [InlineData(DataGridViewSelectionMode.RowHeaderSelect, 9, 3, 0)]
    [InlineData(DataGridViewSelectionMode.FullColumnSelect, 9, 0, 3)]
    [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect, 9, 0, 3)]
    public void DataGridView_SelectAll_ReflectsSelectionMode(
        DataGridViewSelectionMode mode,
        int expectedCells,
        int expectedRows,
        int expectedColumns)
    {
        using DataGridView grid = CreateSelectionGrid(mode);

        grid.SelectAll();

        Assert.Equal(expectedCells, grid.SelectedCells.Count);
        Assert.Equal(expectedRows, grid.SelectedRows.Count);
        Assert.Equal(expectedColumns, grid.SelectedColumns.Count);

        grid.ClearSelection();

        Assert.Empty(grid.SelectedCells);
        Assert.Empty(grid.SelectedRows);
        Assert.Empty(grid.SelectedColumns);
    }

    // Scenario: A cell in a hidden row or hidden column cannot become the current cell.
    [WinFormsFact]
    public void DataGridView_CurrentCell_HiddenCell_Throws()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        grid.Rows[1].Visible = false;
        grid.Columns[2].Visible = false;

        Assert.Throws<InvalidOperationException>(() => grid.CurrentCell = grid[0, 1]);
        Assert.Throws<InvalidOperationException>(() => grid.CurrentCell = grid[2, 0]);
        Assert.Null(grid.CurrentCell);
    }

    // Scenario: Hiding the row that holds the current cell clears the current cell, and the grid
    // then accepts a cell in a visible row again.
    [WinFormsFact]
    public void DataGridView_HideCurrentRow_ClearsCurrentCell()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.FullRowSelect);
        grid.CurrentCell = grid[0, 1];

        grid.Rows[1].Visible = false;

        Assert.Null(grid.CurrentCell);
        Assert.Null(grid.CurrentRow);
        Assert.Equal(new Point(-1, -1), grid.CurrentCellAddress);

        grid.CurrentCell = grid[0, 2];

        Assert.Same(grid.Rows[2], grid.CurrentRow);
    }

    // Scenario: Hiding the column that holds the current cell clears the current cell, and the grid
    // then accepts a cell in a visible column again.
    [WinFormsFact]
    public void DataGridView_HideCurrentColumn_ClearsCurrentCell()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        grid.CurrentCell = grid[1, 1];

        grid.Columns[1].Visible = false;

        Assert.Null(grid.CurrentCell);
        Assert.Equal(new Point(-1, -1), grid.CurrentCellAddress);

        grid.CurrentCell = grid[2, 1];

        Assert.Same(grid[2, 1], grid.CurrentCell);
    }

    // Scenario: Removing the only row leaves the grid without a current cell or selection.
    [WinFormsFact]
    public void DataGridView_RemoveOnlyRow_ClearsCurrentCellAndSelection()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.FullRowSelect, rowCount: 1);
        grid.CurrentCell = grid[0, 0];

        grid.Rows.RemoveAt(0);

        Assert.Null(grid.CurrentCell);
        Assert.Null(grid.CurrentRow);
        Assert.Empty(grid.SelectedRows);
        Assert.Empty(grid.SelectedCells);
    }

    // Scenario: Removing a selected row drops it from the selection and keeps the other selected rows.
    [WinFormsFact]
    public void DataGridView_RemoveSelectedRow_UpdatesSelectedRows()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.RowHeaderSelect, rowCount: 4);
        DataGridViewRow kept = grid.Rows[3];
        grid.Rows[1].Selected = true;
        kept.Selected = true;

        grid.Rows.RemoveAt(1);

        Assert.Same(kept, Assert.Single(grid.SelectedRows));
        Assert.Equal(3, grid.SelectedCells.Count);
        Assert.Equal(2, kept.Index);
    }

    // Scenario: Removing a selected column drops it from the selection and keeps the other selected columns.
    [WinFormsFact]
    public void DataGridView_RemoveSelectedColumn_UpdatesSelectedColumns()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.ColumnHeaderSelect, columnCount: 4);
        DataGridViewColumn kept = grid.Columns[3];
        grid.Columns[1].Selected = true;
        kept.Selected = true;

        grid.Columns.RemoveAt(1);

        Assert.Same(kept, Assert.Single(grid.SelectedColumns));
        Assert.Equal(3, grid.SelectedCells.Count);
    }

    // Scenario: A read-only cell can become the current cell and be selected, but it cannot enter edit mode.
    [WinFormsFact]
    public void DataGridView_ReadOnlyCell_CanBeCurrentButNotEdited()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        _ = grid.Handle;
        grid[0, 0].ReadOnly = true;

        try
        {
            grid.CurrentCell = grid[0, 0];

            Assert.Same(grid[0, 0], grid.CurrentCell);
            Assert.True(grid[0, 0].Selected);
            Assert.False(grid.BeginEdit(false));
            Assert.False(grid.IsCurrentCellInEditMode);

            grid.CurrentCell = grid[1, 0];

            Assert.True(grid.BeginEdit(false));
            Assert.True(grid.IsCurrentCellInEditMode);
        }
        finally
        {
            // Disposing a handle-backed grid that still has a current cell trips a debug assertion.
            grid.CancelEdit();
            grid.CurrentCell = null;
        }
    }

    // Scenario: A read-only row or column does not prevent the current cell from moving into it.
    [WinFormsFact]
    public void DataGridView_ReadOnlyRowAndColumn_DoNotBlockCurrentCellMovement()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        grid.Rows[1].ReadOnly = true;
        grid.Columns[2].ReadOnly = true;

        grid.CurrentCell = grid[0, 1];
        Assert.Same(grid[0, 1], grid.CurrentCell);

        grid.CurrentCell = grid[2, 0];
        Assert.Same(grid[2, 0], grid.CurrentCell);
        Assert.Equal([(2, 0)], GetSelectedCellAddresses(grid));
    }

    // Scenario: Moving the current cell raises CurrentCellChanged and SelectionChanged exactly once
    // each, and assigning the already-current cell raises neither.
    [WinFormsFact]
    public void DataGridView_CurrentCell_Changed_RaisesEventsOncePerMove()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        grid.CurrentCell = grid[0, 0];
        List<string> events = [];
        grid.CurrentCellChanged += (sender, e) => events.Add("CurrentCellChanged");
        grid.SelectionChanged += (sender, e) => events.Add("SelectionChanged");

        grid.CurrentCell = grid[1, 1];

        Assert.Equal(1, events.Count(e => e == "CurrentCellChanged"));
        Assert.Equal(1, events.Count(e => e == "SelectionChanged"));

        events.Clear();
        grid.CurrentCell = grid[1, 1];

        Assert.Empty(events);
    }

    // Scenario: Moving the current cell to another row raises the leave events for the old cell first,
    // then SelectionChanged, then the enter and CurrentCellChanged events for the new cell.
    [WinFormsFact]
    public void DataGridView_CurrentCell_Moved_RaisesLeaveBeforeEnterEvents()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        grid.CurrentCell = grid[0, 0];
        List<string> events = [];
        grid.CellLeave += (sender, e) => events.Add($"CellLeave({e.ColumnIndex},{e.RowIndex})");
        grid.RowLeave += (sender, e) => events.Add($"RowLeave({e.ColumnIndex},{e.RowIndex})");
        grid.RowEnter += (sender, e) => events.Add($"RowEnter({e.ColumnIndex},{e.RowIndex})");
        grid.CellEnter += (sender, e) => events.Add($"CellEnter({e.ColumnIndex},{e.RowIndex})");
        grid.CurrentCellChanged += (sender, e) => events.Add("CurrentCellChanged");
        grid.SelectionChanged += (sender, e) => events.Add("SelectionChanged");

        grid.CurrentCell = grid[1, 2];

        Assert.Equal(
            [
                "CellLeave(0,0)",
                "RowLeave(0,0)",
                "SelectionChanged",
                "RowEnter(1,2)",
                "CurrentCellChanged",
                "CellEnter(1,2)"
            ],
            events);
    }

    // Scenario: ClearSelection raises SelectionChanged only when something was selected, and
    // SelectAll raises it when the selection grows.
    [WinFormsFact]
    public void DataGridView_ClearSelectionAndSelectAll_RaiseSelectionChangedOnlyOnChange()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.CellSelect);
        int selectionChangedCount = 0;
        grid.SelectionChanged += (sender, e) => selectionChangedCount++;

        grid.ClearSelection();
        Assert.Equal(0, selectionChangedCount);

        grid.SelectAll();
        Assert.Equal(1, selectionChangedCount);
        Assert.Equal(9, grid.SelectedCells.Count);

        grid.ClearSelection();
        Assert.Equal(2, selectionChangedCount);
        Assert.Empty(grid.SelectedCells);
    }

    // Scenario: Selecting the last row of a grid with many rows selects only that row.
    [WinFormsFact]
    public void DataGridView_RowHeaderSelect_LastRowOfLargeGrid_SelectsOnlyThatRow()
    {
        using DataGridView grid = CreateSelectionGrid(DataGridViewSelectionMode.RowHeaderSelect, columnCount: 2, rowCount: 2000);

        grid.Rows[1999].Selected = true;

        Assert.Same(grid.Rows[1999], Assert.Single(grid.SelectedRows));
        Assert.Equal(2, grid.SelectedCells.Count);
    }
}
