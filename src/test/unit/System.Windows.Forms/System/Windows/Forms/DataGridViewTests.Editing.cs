// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Data;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class DataGridViewTests
{
    // Creates a grid with two text columns and two rows ("A1","B1" / "A2","B2") hosted by a form with a created handle.
    private static DataGridView CreateEditingGrid(Form form, DataGridViewEditMode editMode = DataGridViewEditMode.EditOnKeystrokeOrF2)
    {
        DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false, EditMode = editMode };
        grid.Columns.Add("Column1", "Column1");
        grid.Columns.Add("Column2", "Column2");
        grid.Rows.Add("A1", "B1");
        grid.Rows.Add("A2", "B2");
        form.Controls.Add(grid);
        _ = grid.Handle;
        return grid;
    }

    // Scenario: BeginEdit has nothing to edit when the grid has no current cell, so it fails with a clear exception.
    [WinFormsFact]
    public void DataGridView_BeginEdit_NoCurrentCell_Throws()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        grid.CurrentCell = null;

        Assert.Throws<InvalidOperationException>(() => grid.BeginEdit(selectAll: false));
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.False(grid.IsCurrentCellDirty);
        Assert.Null(grid.EditingControl);
    }

    // Scenario: EndEdit and CancelEdit are harmless no-ops (reporting success) when no cell is being edited,
    // and no editing events are raised.
    [WinFormsFact]
    public void DataGridView_EndEditAndCancelEdit_NotEditing_SucceedWithoutEvents()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        int eventCount = 0;
        grid.CellBeginEdit += (s, e) => eventCount++;
        grid.CellEndEdit += (s, e) => eventCount++;
        grid.CellValidating += (s, e) => eventCount++;
        grid.CellValueChanged += (s, e) => eventCount++;

        // No current cell at all.
        grid.CurrentCell = null;
        eventCount = 0;
        Assert.True(grid.EndEdit());
        Assert.True(grid.CancelEdit());
        Assert.True(grid.CommitEdit(DataGridViewDataErrorContexts.Commit));
        Assert.Equal(0, eventCount);

        // A current cell that has not entered edit mode.
        grid.CurrentCell = grid[0, 0];
        eventCount = 0;
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.True(grid.EndEdit());
        Assert.True(grid.CancelEdit());
        Assert.True(grid.CommitEdit(DataGridViewDataErrorContexts.Commit));
        Assert.Equal(0, eventCount);
        Assert.Equal("A1", grid[0, 0].Value);
    }

    // Scenario: BeginEdit on a text cell installs a text editing control, raises CellBeginEdit and EditingControlShowing
    // once, reports edit mode without marking the cell dirty, and a repeated BeginEdit reuses the same session.
    [WinFormsFact]
    public void DataGridView_BeginEdit_TextCell_InstallsEditingControl()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        List<string> events = [];
        Control showingControl = null;
        grid.CellBeginEdit += (s, e) => events.Add($"CellBeginEdit {e.ColumnIndex},{e.RowIndex}");
        grid.EditingControlShowing += (s, e) =>
        {
            events.Add("EditingControlShowing");
            showingControl = e.Control;
        };

        grid.CurrentCell = grid[1, 1];
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Null(grid.EditingControl);

        Assert.True(grid.BeginEdit(selectAll: true));

        Assert.True(grid.IsCurrentCellInEditMode);
        Assert.False(grid.IsCurrentCellDirty);
        Assert.IsType<DataGridViewTextBoxEditingControl>(grid.EditingControl);
        Assert.Same(grid.EditingControl, showingControl);
        IDataGridViewEditingControl editor = (IDataGridViewEditingControl)grid.EditingControl;
        Assert.Same(grid, editor.EditingControlDataGridView);
        Assert.Equal(1, editor.EditingControlRowIndex);
        Assert.Equal("B2", editor.EditingControlFormattedValue);
        Assert.False(editor.EditingControlValueChanged);
        Assert.Equal(["CellBeginEdit 1,1", "EditingControlShowing"], events);

        // Already editing: succeeds without starting a second session.
        Assert.True(grid.BeginEdit(selectAll: false));
        Assert.Equal(2, events.Count);
        Assert.Same(showingControl, grid.EditingControl);
    }

    // Scenario: BeginEdit refuses to start when the cell, its column or the whole grid is read-only,
    // and no CellBeginEdit notification is raised.
    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void DataGridView_BeginEdit_ReadOnly_ReturnsFalse(int readOnlyLevel)
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        int beginEditCount = 0;
        grid.CellBeginEdit += (s, e) => beginEditCount++;
        switch (readOnlyLevel)
        {
            case 0:
                grid[0, 0].ReadOnly = true;
                break;
            case 1:
                grid.Columns[0].ReadOnly = true;
                break;
            default:
                grid.ReadOnly = true;
                break;
        }

        grid.CurrentCell = grid[0, 0];

        Assert.False(grid.BeginEdit(selectAll: false));
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Null(grid.EditingControl);
        Assert.Equal(0, beginEditCount);
    }

    // Scenario: A cell type that has neither an editing control nor in-cell editing support (a button cell)
    // cannot enter edit mode.
    [WinFormsFact]
    public void DataGridView_BeginEdit_CellWithoutEditor_ReturnsFalse()
    {
        using Form form = new();
        using DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false };
        grid.Columns.Add(new DataGridViewButtonColumn());
        grid.Rows.Add("Go");
        form.Controls.Add(grid);
        _ = grid.Handle;
        grid.CurrentCell = grid[0, 0];
        int beginEditCount = 0;
        grid.CellBeginEdit += (s, e) => beginEditCount++;

        Assert.False(grid.BeginEdit(selectAll: false));
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Null(grid.EditingControl);
        Assert.Equal(0, beginEditCount);
    }

    // Scenario: A CellBeginEdit handler that sets Cancel prevents the edit from starting: no editing control is
    // shown, and the same cell can be edited once the handler allows it.
    [WinFormsFact]
    public void DataGridView_BeginEdit_CellBeginEditCanceled_DoesNotEnterEditMode()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        bool cancel = true;
        int showingCount = 0;
        grid.CellBeginEdit += (s, e) => e.Cancel = cancel;
        grid.EditingControlShowing += (s, e) => showingCount++;
        grid.CurrentCell = grid[0, 0];

        Assert.False(grid.BeginEdit(selectAll: false));
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Null(grid.EditingControl);
        Assert.Equal(0, showingCount);

        cancel = false;
        Assert.True(grid.BeginEdit(selectAll: false));
        Assert.True(grid.IsCurrentCellInEditMode);
        Assert.Equal(1, showingCount);
    }

    // Scenario: Typing into the editing control marks the current cell dirty and raises CurrentCellDirtyStateChanged
    // without touching the stored cell value; EndEdit then commits the value, clears the dirty state, leaves edit mode
    // and raises the value-changed, dirty-state-cleared and CellEndEdit notifications in that order.
    [WinFormsFact]
    public void DataGridView_EndEdit_AfterTyping_CommitsValueAndLeavesEditMode()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        List<string> events = [];
        grid.CurrentCellDirtyStateChanged += (s, e) => events.Add($"Dirty={grid.IsCurrentCellDirty}");
        grid.CellValueChanged += (s, e) => events.Add("ValueChanged");
        grid.CellEndEdit += (s, e) => events.Add("CellEndEdit");
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));

        grid.EditingControl.Text = "Edited";

        Assert.True(grid.IsCurrentCellDirty);
        Assert.True(grid.IsCurrentCellInEditMode);
        Assert.Equal("A1", grid[0, 0].Value);
        Assert.Equal(["Dirty=True"], events);

        events.Clear();
        Assert.True(grid.EndEdit());

        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.False(grid.IsCurrentCellDirty);
        Assert.Null(grid.EditingControl);
        Assert.Equal("Edited", grid[0, 0].Value);
        Assert.Equal("Edited", grid[0, 0].FormattedValue);
        Assert.Equal(["ValueChanged", "Dirty=False", "CellEndEdit"], events);
    }

    // Scenario: EndEdit without any typing leaves the original value untouched and does not report a value change,
    // while still ending the session and raising CellEndEdit.
    [WinFormsFact]
    public void DataGridView_EndEdit_NoChange_KeepsValue()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        int valueChangedCount = 0;
        int endEditCount = 0;
        grid.CellValueChanged += (s, e) => valueChangedCount++;
        grid.CellEndEdit += (s, e) => endEditCount++;
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));

        Assert.True(grid.EndEdit());

        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Equal("A1", grid[0, 0].Value);
        Assert.Equal(0, valueChangedCount);
        Assert.Equal(1, endEditCount);
    }

    // Scenario: CancelEdit discards typed text: the editor is restored to its original text and clean state, the cell
    // value is unchanged, and a following EndEdit commits nothing.
    [WinFormsFact]
    public void DataGridView_CancelEdit_AfterTyping_RestoresOriginalValue()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        int valueChangedCount = 0;
        grid.CellValueChanged += (s, e) => valueChangedCount++;
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "Discarded";
        Assert.True(grid.IsCurrentCellDirty);

        Assert.True(grid.CancelEdit());

        Assert.False(grid.IsCurrentCellDirty);
        IDataGridViewEditingControl editor = (IDataGridViewEditingControl)grid.EditingControl;
        Assert.NotNull(editor);
        Assert.Equal("A1", editor.EditingControlFormattedValue);
        Assert.False(editor.EditingControlValueChanged);
        Assert.Equal("A1", grid[0, 0].Value);

        Assert.True(grid.EndEdit());
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Equal("A1", grid[0, 0].Value);
        Assert.Equal(0, valueChangedCount);
    }

    // Scenario: CommitEdit pushes the typed value into the cell while the cell stays in edit mode,
    // and the committed value remains after the edit later ends.
    [WinFormsFact]
    public void DataGridView_CommitEdit_AfterTyping_PushesValueAndKeepsEditing()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        int endEditCount = 0;
        grid.CellEndEdit += (s, e) => endEditCount++;
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "Committed";

        Assert.True(grid.CommitEdit(DataGridViewDataErrorContexts.Commit));

        Assert.True(grid.IsCurrentCellInEditMode);
        Assert.False(grid.IsCurrentCellDirty);
        Assert.Equal("Committed", grid[0, 0].Value);
        Assert.Equal(0, endEditCount);

        Assert.True(grid.EndEdit());
        Assert.Equal("Committed", grid[0, 0].Value);
        Assert.Equal(1, endEditCount);
    }

    // Scenario: A CellValidating handler that cancels blocks leaving the edited cell: the current cell does not move,
    // the value is not committed, the cell stays in edit mode and dirty, and once validation passes the move
    // commits the pending edit.
    [WinFormsFact]
    public void DataGridView_CurrentCellChange_ValidationCanceled_StaysInEditMode()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        bool reject = true;
        object validatedValue = null;
        int endEditCount = 0;
        grid.CellValidating += (s, e) =>
        {
            validatedValue = e.FormattedValue;
            e.Cancel = reject;
        };
        grid.CellEndEdit += (s, e) => endEditCount++;
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "Invalid";

        Assert.Throws<InvalidOperationException>(() => grid.CurrentCell = grid[1, 1]);

        Assert.Equal("Invalid", validatedValue);
        Assert.Same(grid[0, 0], grid.CurrentCell);
        Assert.True(grid.IsCurrentCellInEditMode);
        Assert.True(grid.IsCurrentCellDirty);
        Assert.Equal("A1", grid[0, 0].Value);
        Assert.Equal(0, endEditCount);

        reject = false;
        grid.CurrentCell = grid[1, 1];
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Equal("Invalid", grid[0, 0].Value);
        Assert.Equal(1, endEditCount);
    }

    // Scenario: After a canceled validation blocks leaving the cell, CancelEdit discards the pending text so the
    // current cell can be left without committing anything.
    [WinFormsFact]
    public void DataGridView_CancelEdit_AfterValidationCanceled_AllowsLeavingCell()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        grid.CellValidating += (s, e) => e.Cancel = e.FormattedValue is "Invalid";
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "Invalid";
        Assert.Throws<InvalidOperationException>(() => grid.CurrentCell = grid[1, 1]);

        Assert.True(grid.CancelEdit());
        Assert.False(grid.IsCurrentCellDirty);

        grid.CurrentCell = grid[1, 1];
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Equal("A1", grid[0, 0].Value);
        Assert.Same(grid[1, 1], grid.CurrentCell);
    }

    // Scenario: Moving the current cell while a cell is being edited commits the pending edit to the old cell
    // and ends its edit session before the new cell becomes current.
    [WinFormsFact]
    public void DataGridView_CurrentCellChange_WhileEditing_CommitsPendingEdit()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        int endEditCount = 0;
        grid.CellEndEdit += (s, e) => endEditCount++;
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "Moved";

        grid.CurrentCell = grid[1, 1];

        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Equal("Moved", grid[0, 0].Value);
        Assert.Equal(1, endEditCount);
        Assert.Same(grid[1, 1], grid.CurrentCell);
    }

    // Scenario: After an edit ends, editing another cell of the same column type reuses the previously created
    // editing control instance and refreshes it with the new cell's value and row.
    [WinFormsFact]
    public void DataGridView_BeginEdit_AfterEndEdit_ReusesEditingControl()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        Control first = grid.EditingControl;
        Assert.True(grid.EndEdit());
        Assert.Null(grid.EditingControl);

        grid.CurrentCell = grid[1, 1];
        Assert.True(grid.BeginEdit(selectAll: false));

        Assert.Same(first, grid.EditingControl);
        IDataGridViewEditingControl editor = (IDataGridViewEditingControl)grid.EditingControl;
        Assert.Equal(1, editor.EditingControlRowIndex);
        Assert.Equal("B2", editor.EditingControlFormattedValue);
        Assert.False(editor.EditingControlValueChanged);
        Assert.True(grid.EndEdit());
    }

    // Scenario: An EditingControlShowing handler can restyle the editor: the cell style it changes is applied to
    // the editing control that is displayed.
    [WinFormsFact]
    public void DataGridView_EditingControlShowing_CellStyleChange_AppliedToEditor()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        grid.EditingControlShowing += (s, e) => e.CellStyle.BackColor = Color.Orange;
        grid.CurrentCell = grid[0, 0];

        Assert.True(grid.BeginEdit(selectAll: false));

        Assert.Equal(Color.Orange, grid.EditingControl.BackColor);
    }

    // Scenario: A combo box column uses a combo box editing control; selecting an item makes the cell dirty and
    // EndEdit commits the selected item, and switching back to a text column replaces the editor type.
    [WinFormsFact]
    public void DataGridView_Edit_ComboBoxCell_CommitsSelectedItem()
    {
        using Form form = new();
        using DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false };
        DataGridViewComboBoxColumn comboColumn = new() { Name = "Combo" };
        comboColumn.Items.AddRange("One", "Two", "Three");
        grid.Columns.Add(comboColumn);
        grid.Columns.Add("Text", "Text");
        grid.Rows.Add("One", "t");
        form.Controls.Add(grid);
        _ = grid.Handle;
        grid.CurrentCell = grid[0, 0];

        Assert.True(grid.BeginEdit(selectAll: false));
        DataGridViewComboBoxEditingControl combo = Assert.IsType<DataGridViewComboBoxEditingControl>(grid.EditingControl);
        Assert.False(grid.IsCurrentCellDirty);

        combo.SelectedIndex = 2;

        Assert.True(grid.IsCurrentCellDirty);
        Assert.Equal("One", grid[0, 0].Value);
        Assert.True(grid.EndEdit());
        Assert.Equal("Three", grid[0, 0].Value);
        Assert.False(grid.IsCurrentCellInEditMode);

        grid.CurrentCell = grid[1, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        Assert.IsType<DataGridViewTextBoxEditingControl>(grid.EditingControl);
        Assert.True(grid.EndEdit());
    }

    // Scenario: A check box cell edits in place without an editing control; toggling it reports edit mode and
    // dirtiness through the cell itself, EndEdit commits the new value, and CancelEdit restores the original.
    [WinFormsFact]
    public void DataGridView_Edit_CheckBoxCell_UsesInCellEditing()
    {
        using Form form = new();
        using DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false };
        grid.Columns.Add(new DataGridViewCheckBoxColumn());
        grid.Rows.Add(false);
        form.Controls.Add(grid);
        _ = grid.Handle;
        grid.CurrentCell = grid[0, 0];

        Assert.True(grid.BeginEdit(selectAll: false));

        Assert.True(grid.IsCurrentCellInEditMode);
        Assert.Null(grid.EditingControl);
        IDataGridViewEditingCell editingCell = (IDataGridViewEditingCell)grid[0, 0];
        Assert.False(editingCell.EditingCellValueChanged);

        editingCell.EditingCellFormattedValue = true;
        editingCell.EditingCellValueChanged = true;
        grid.NotifyCurrentCellDirty(true);
        Assert.True(grid.IsCurrentCellDirty);
        Assert.Equal(false, grid[0, 0].Value);

        Assert.True(grid.EndEdit());
        Assert.Equal(true, grid[0, 0].Value);
        Assert.False(grid.IsCurrentCellInEditMode);

        Assert.True(grid.BeginEdit(selectAll: false));
        editingCell.EditingCellFormattedValue = false;
        editingCell.EditingCellValueChanged = true;
        grid.NotifyCurrentCellDirty(true);
        Assert.True(grid.CancelEdit());
        Assert.False(grid.IsCurrentCellDirty);
        Assert.True(grid.EndEdit());
        Assert.Equal(true, grid[0, 0].Value);
    }

    // Scenario: NotifyCurrentCellDirty toggles the dirty state and raises CurrentCellDirtyStateChanged
    // for each notification.
    [WinFormsFact]
    public void DataGridView_NotifyCurrentCellDirty_TogglesStateAndRaisesEvent()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form);
        List<bool> states = [];
        grid.CurrentCellDirtyStateChanged += (s, e) => states.Add(grid.IsCurrentCellDirty);
        grid.CurrentCell = grid[0, 0];
        Assert.False(grid.IsCurrentCellDirty);

        grid.NotifyCurrentCellDirty(true);
        grid.NotifyCurrentCellDirty(false);

        Assert.Equal([true, false], states);
        Assert.False(grid.IsCurrentCellDirty);
    }

    // Scenario: In EditOnEnter mode, EndEdit commits the pending value of an explicitly started edit without leaving
    // edit mode.
    [WinFormsFact]
    public void DataGridView_EditOnEnter_EndEdit_CommitsAndKeepsEditing()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form, DataGridViewEditMode.EditOnEnter);

        grid.CurrentCell = grid[0, 1];
        Assert.True(grid.BeginEdit(selectAll: false));
        Assert.True(grid.IsCurrentCellInEditMode);
        grid.EditingControl.Text = "Entered";

        Assert.True(grid.EndEdit());

        Assert.Equal("Entered", grid[0, 1].Value);
        Assert.True(grid.IsCurrentCellInEditMode);
    }

    // Scenario: With EditProgrammatically, selecting a cell does not put it in edit mode, and an explicit BeginEdit does.
    [WinFormsFact]
    public void DataGridView_EditProgrammatically_SelectingCellDoesNotStartEdit()
    {
        using Form form = new();
        using DataGridView grid = CreateEditingGrid(form, DataGridViewEditMode.EditProgrammatically);

        grid.CurrentCell = grid[0, 0];
        Assert.False(grid.IsCurrentCellInEditMode);

        Assert.True(grid.BeginEdit(selectAll: false));
        Assert.True(grid.IsCurrentCellInEditMode);
    }

    // Scenario: Text that cannot be converted to the column's value type reports a data error with the parsing
    // context; when the handler cancels, EndEdit fails, the stored value stays unchanged and editing continues.
    [WinFormsFact]
    public void DataGridView_EndEdit_ParseFailure_RaisesDataErrorAndStaysInEditMode()
    {
        using Form form = new();
        using DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false };
        grid.Columns.Add(new DataGridViewTextBoxColumn { ValueType = typeof(int) });
        grid.Rows.Add(new object[] { 5 });
        form.Controls.Add(grid);
        _ = grid.Handle;
        DataGridViewDataErrorContexts? context = null;
        Exception error = null;
        grid.DataError += (s, e) =>
        {
            context = e.Context;
            error = e.Exception;
            e.ThrowException = false;
            e.Cancel = true;
        };
        grid.CurrentCell = grid[0, 0];
        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "not a number";

        Assert.False(grid.EndEdit());

        Assert.NotNull(error);
        Assert.True(context.Value.HasFlag(DataGridViewDataErrorContexts.Parsing));
        Assert.Equal(5, grid[0, 0].Value);
        Assert.True(grid.IsCurrentCellInEditMode);

        // A valid number later commits normally.
        grid.EditingControl.Text = "42";
        Assert.True(grid.EndEdit());
        Assert.Equal(42, grid[0, 0].Value);
    }

    // Scenario: A bound grid writes the committed edit to the data source on EndEdit, while a canceled edit
    // leaves the source row untouched.
    [WinFormsFact]
    public void DataGridView_Edit_DataBound_CommitsToSourceAndCancelLeavesSource()
    {
        using DataTable table = new();
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add("Original");
        table.AcceptChanges();
        using Form form = new();
        using DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false };
        form.Controls.Add(grid);
        _ = grid.Handle;
        grid.DataSource = table;
        grid.CurrentCell = grid[0, 0];

        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "Discarded";
        Assert.True(grid.CancelEdit());
        Assert.True(grid.EndEdit());
        Assert.Equal("Original", table.Rows[0]["Name"]);

        Assert.True(grid.BeginEdit(selectAll: false));
        grid.EditingControl.Text = "Saved";
        Assert.True(grid.EndEdit());

        Assert.Equal("Saved", table.Rows[0]["Name"]);
    }

    // Scenario: A custom editing control follows the IDataGridViewEditingControl contract: the grid assigns the
    // grid and row, initializes the value, applies the cell style and prepares the editor on BeginEdit; typing flows
    // back as dirtiness; and commit asks the editor for its formatted value with the commit context.
    [WinFormsFact]
    public void DataGridView_Edit_CustomEditingControl_ContractIsHonored()
    {
        using Form form = new();
        using DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false };
        grid.Columns.Add(new DataGridViewColumn(new EditingContractCell()) { Name = "Custom" });
        grid.Rows.Add("Initial");
        form.Controls.Add(grid);
        _ = grid.Handle;
        grid.CurrentCell = grid[0, 0];

        Assert.True(grid.BeginEdit(selectAll: true));

        EditingContractControl editor = Assert.IsType<EditingContractControl>(grid.EditingControl);
        Assert.Same(grid, editor.EditingControlDataGridView);
        Assert.Equal(0, editor.EditingControlRowIndex);
        Assert.Equal("Initial", editor.EditingControlFormattedValue);
        Assert.NotNull(editor.AppliedStyle);
        Assert.True(editor.PreparedSelectAll);
        Assert.False(grid.IsCurrentCellDirty);

        editor.Text = "Custom value";

        Assert.True(editor.EditingControlValueChanged);
        Assert.True(grid.IsCurrentCellDirty);

        Assert.True(grid.CommitEdit(DataGridViewDataErrorContexts.Commit));
        Assert.Equal(DataGridViewDataErrorContexts.Commit, editor.LastFormattedValueContext);
        Assert.Equal("Custom value", grid[0, 0].Value);

        Assert.True(grid.EndEdit());
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Equal("Custom value", grid[0, 0].Value);
    }

    // Scenario: A cell whose EditType is not an editing control is rejected with an InvalidCastException when
    // editing starts, and the grid does not enter edit mode.
    [WinFormsFact]
    public void DataGridView_BeginEdit_InvalidEditType_Throws()
    {
        using Form form = new();
        using DataGridView grid = new EditingTestGrid { AllowUserToAddRows = false };
        grid.Columns.Add(new DataGridViewColumn(new InvalidEditTypeCell()) { Name = "Invalid" });
        grid.Rows.Add("Value");
        form.Controls.Add(grid);
        _ = grid.Handle;
        grid.CurrentCell = grid[0, 0];

        Assert.Throws<InvalidCastException>(() => grid.BeginEdit(selectAll: false));
        Assert.False(grid.IsCurrentCellInEditMode);
        Assert.Null(grid.EditingControl);
    }

    // Grid that leaves edit mode before disposal; disposing a grid that is still editing trips a debug assertion
    // in the grid's column cleanup, which is unrelated to the behavior under test.
    private sealed class EditingTestGrid : DataGridView
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing && IsCurrentCellInEditMode)
            {
                CancelEdit();
                EndEdit();
                CurrentCell = null;
            }

            base.Dispose(disposing);
        }
    }

    private class EditingContractCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(EditingContractControl);
    }

    private class InvalidEditTypeCell : DataGridViewTextBoxCell
    {
        public override Type EditType => typeof(string);
    }

    private class EditingContractControl : TextBox, IDataGridViewEditingControl
    {
        public DataGridViewCellStyle AppliedStyle { get; private set; }

        public bool PreparedSelectAll { get; private set; }

        public DataGridViewDataErrorContexts LastFormattedValueContext { get; private set; }

        public DataGridView EditingControlDataGridView { get; set; }

        public object EditingControlFormattedValue
        {
            get => Text;
            set => Text = (string)value;
        }

        public int EditingControlRowIndex { get; set; }

        public bool EditingControlValueChanged { get; set; }

        public Cursor EditingPanelCursor => Cursors.Default;

        public bool RepositionEditingControlOnValueChange => false;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle) => AppliedStyle = dataGridViewCellStyle;

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey) => false;

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            LastFormattedValueContext = context;
            return Text;
        }

        public void PrepareEditingControlForEdit(bool selectAll) => PreparedSelectAll = selectAll;

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            EditingControlValueChanged = true;
            EditingControlDataGridView?.NotifyCurrentCellDirty(true);
        }
    }
}
