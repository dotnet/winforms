// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Tests;

public class DataGridViewColumnTests
{
    [WinFormsFact]
    public void DataGridViewColumn_Ctor_Default()
    {
        using SubDataGridViewColumn column = new();
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.AutoSizeMode);
        Assert.Null(column.CellTemplate);
        Assert.Null(column.CellType);
        Assert.Null(column.ContextMenuStrip);
        Assert.Null(column.DataGridView);
        Assert.Empty(column.DataPropertyName);
        Assert.NotNull(column.DefaultCellStyle);
        Assert.Same(column.DefaultCellStyle, column.DefaultCellStyle);
        Assert.Same(typeof(DataGridViewColumnHeaderCell), column.DefaultHeaderCellType);
        Assert.Equal(-1, column.DisplayIndex);
        Assert.False(column.Displayed);
        Assert.Equal(0, column.DividerWidth);
        Assert.Equal(100, column.FillWeight);
        Assert.False(column.Frozen);
        Assert.True(column.HasDefaultCellStyle);
        Assert.IsType<DataGridViewColumnHeaderCell>(column.HeaderCell);
        Assert.Same(column.HeaderCell, column.HeaderCell);
        Assert.IsType<DataGridViewColumnHeaderCell>(column.HeaderCellCore);
        Assert.Same(column.HeaderCellCore, column.HeaderCellCore);
        Assert.Empty(column.HeaderText);
        Assert.Equal(column.Index, column.Index);
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.InheritedAutoSizeMode);
        Assert.Same(column.DefaultCellStyle, column.InheritedStyle);
        Assert.False(column.IsDataBound);
        Assert.False(column.IsRow);
        Assert.Equal(5, column.MinimumWidth);
        Assert.Empty(column.Name);
        Assert.False(column.ReadOnly);
        Assert.Equal(DataGridViewTriState.NotSet, column.Resizable);
        Assert.False(column.Selected);
        Assert.Null(column.Site);
        Assert.Equal(DataGridViewElementStates.Visible, column.State);
        Assert.Equal(DataGridViewColumnSortMode.NotSortable, column.SortMode);
        Assert.Null(column.Tag);
        Assert.Empty(column.ToolTipText);
        Assert.Null(column.ValueType);
        Assert.True(column.Visible);
        Assert.Equal(100, column.Width);
    }

    public static IEnumerable<object[]> Ctor_DataGridViewCell_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new SubDataGridViewCell() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_DataGridViewCell_TestData))]
    public void DataGridViewColumn_Ctor_DataGridViewCell(DataGridViewCell cellTemplate)
    {
        using SubDataGridViewColumn column = new(cellTemplate);
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.AutoSizeMode);
        Assert.Same(cellTemplate, column.CellTemplate);
        Assert.Equal(cellTemplate?.GetType(), column.CellType);
        Assert.Null(column.ContextMenuStrip);
        Assert.Null(column.DataGridView);
        Assert.Empty(column.DataPropertyName);
        Assert.NotNull(column.DefaultCellStyle);
        Assert.Same(column.DefaultCellStyle, column.DefaultCellStyle);
        Assert.Same(typeof(DataGridViewColumnHeaderCell), column.DefaultHeaderCellType);
        Assert.Equal(-1, column.DisplayIndex);
        Assert.False(column.Displayed);
        Assert.Equal(0, column.DividerWidth);
        Assert.Equal(100, column.FillWeight);
        Assert.False(column.Frozen);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Empty(column.HeaderText);
        Assert.Equal(-1, column.Index);
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.InheritedAutoSizeMode);
        Assert.Same(column.DefaultCellStyle, column.InheritedStyle);
        Assert.False(column.IsDataBound);
        Assert.False(column.IsRow);
        Assert.Equal(5, column.MinimumWidth);
        Assert.Empty(column.Name);
        Assert.False(column.ReadOnly);
        Assert.Equal(DataGridViewTriState.NotSet, column.Resizable);
        Assert.False(column.Selected);
        Assert.Null(column.Site);
        Assert.Equal(DataGridViewElementStates.Visible, column.State);
        Assert.Equal(DataGridViewColumnSortMode.NotSortable, column.SortMode);
        Assert.Null(column.Tag);
        Assert.Empty(column.ToolTipText);
        Assert.Null(column.ValueType);
        Assert.True(column.Visible);
        Assert.Equal(100, column.Width);
    }

    [WinFormsTheory]
    [EnumData<DataGridViewAutoSizeColumnMode>]
    public void DataGridViewColumn_AutoSizeMode_Set_GetReturnsExpected(DataGridViewAutoSizeColumnMode value)
    {
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = value
        };
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);

        // Set same.
        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);
    }

    [WinFormsTheory]
    [EnumData<DataGridViewAutoSizeColumnMode>]
    public void DataGridViewColumn_AutoSizeMode_SetNotVisible_GetReturnsExpected(DataGridViewAutoSizeColumnMode value)
    {
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = value,
            Visible = false
        };
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);

        // Set same.
        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);
    }

    public static IEnumerable<object[]> AutoSizeMode_SetWithDataGridView_TestData()
    {
        foreach (DataGridViewAutoSizeColumnsMode dataGridMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnsMode)))
        {
            foreach (bool columnHeadersVisible in new bool[] { true, false })
            {
                if (dataGridMode == DataGridViewAutoSizeColumnsMode.ColumnHeader && !columnHeadersVisible)
                {
                    continue;
                }

                foreach (bool frozen in new bool[] { true, false })
                {
                    if (dataGridMode == DataGridViewAutoSizeColumnsMode.Fill && frozen)
                    {
                        continue;
                    }

                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.AllCells };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.DisplayedCells, DataGridViewAutoSizeColumnMode.DisplayedCells };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader, DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.None, DataGridViewAutoSizeColumnMode.None };
                }

                yield return new object[] { dataGridMode, columnHeadersVisible, false, DataGridViewAutoSizeColumnMode.Fill, DataGridViewAutoSizeColumnMode.Fill };
            }

            if (dataGridMode != DataGridViewAutoSizeColumnsMode.Fill)
            {
                yield return new object[] { dataGridMode, true, true, DataGridViewAutoSizeColumnMode.ColumnHeader, DataGridViewAutoSizeColumnMode.ColumnHeader };
            }

            yield return new object[] { dataGridMode, true, false, DataGridViewAutoSizeColumnMode.ColumnHeader, DataGridViewAutoSizeColumnMode.ColumnHeader };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoSizeMode_SetWithDataGridView_TestData))]
    public void DataGridViewColumn_AutoSizeMode_SetWithDataGridView_GetReturnsExpected(DataGridViewAutoSizeColumnsMode parentMode, bool columnHeadersVisible, bool frozen, DataGridViewAutoSizeColumnMode value, DataGridViewAutoSizeColumnMode expectedInherited)
    {
        using DataGridView control = new()
        {
            AutoSizeColumnsMode = parentMode,
            ColumnHeadersVisible = columnHeadersVisible
        };
        using DataGridViewColumn column = new()
        {
            CellTemplate = new SubDataGridViewCell(),
            Frozen = frozen
        };
        control.Columns.Add(column);

        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(expectedInherited, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);

        // Set same.
        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(expectedInherited, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);
    }

    public static IEnumerable<object[]> AutoSizeMode_SetWithDataGridViewNotVisible_TestData()
    {
        foreach (DataGridViewAutoSizeColumnsMode dataGridMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnsMode)))
        {
            foreach (bool columnHeadersVisible in new bool[] { true, false })
            {
                foreach (bool frozen in new bool[] { true, false })
                {
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.AllCells };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.ColumnHeader, DataGridViewAutoSizeColumnMode.ColumnHeader };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.DisplayedCells, DataGridViewAutoSizeColumnMode.DisplayedCells };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader, DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.Fill, DataGridViewAutoSizeColumnMode.Fill };
                    yield return new object[] { dataGridMode, columnHeadersVisible, frozen, DataGridViewAutoSizeColumnMode.None, DataGridViewAutoSizeColumnMode.None };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoSizeMode_SetWithDataGridViewNotVisible_TestData))]
    public void DataGridViewColumn_AutoSizeMode_SetWithDataGridViewNotVisible_GetReturnsExpected(DataGridViewAutoSizeColumnsMode parentMode, bool columnHeadersVisible, bool frozen, DataGridViewAutoSizeColumnMode value, DataGridViewAutoSizeColumnMode expectedInherited)
    {
        using DataGridView control = new()
        {
            AutoSizeColumnsMode = parentMode,
            ColumnHeadersVisible = columnHeadersVisible
        };
        using DataGridViewColumn column = new()
        {
            CellTemplate = new SubDataGridViewCell(),
            Visible = false,
            Frozen = frozen
        };
        control.Columns.Add(column);

        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(expectedInherited, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);

        // Set same.
        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(expectedInherited, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);
    }

    [WinFormsFact]
    public void DataGridViewColumn_AutoSizeMode_SetColumnHeaderWithDataGridViewColumnHeadersNotVisible_ThrowsInvalidOperationException()
    {
        using DataGridView control = new()
        {
            ColumnHeadersVisible = false
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        Assert.Throws<InvalidOperationException>(() => column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader);
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.AutoSizeMode);
        Assert.Equal(DataGridViewAutoSizeColumnMode.None, column.InheritedAutoSizeMode);
    }

    [WinFormsFact]
    public void DataGridViewColumn_AutoSizeMode_SetNotSetWithDataGridViewColumnHeadersNotVisible_GetReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnHeadersVisible = false
        };
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            CellTemplate = new SubDataGridViewCell()
        };
        control.Columns.Add(column);

        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.AutoSizeMode);
        Assert.Equal(DataGridViewAutoSizeColumnMode.None, column.InheritedAutoSizeMode);
    }

    [WinFormsFact]
    public void DataGridViewColumn_AutoSizeMode_SetNotSetWithDataGridViewColumnHeadersNotVisible_ThrowsInvalidOperationException()
    {
        using DataGridView control = new()
        {
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader,
            ColumnHeadersVisible = false
        };
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            CellTemplate = new SubDataGridViewCell()
        };
        control.Columns.Add(column);

        Assert.Throws<InvalidOperationException>(() => column.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet);
        Assert.Equal(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
        Assert.Equal(DataGridViewAutoSizeColumnMode.AllCells, column.InheritedAutoSizeMode);
    }

    [WinFormsFact]
    public void DataGridViewColumn_AutoSizeMode_SetFillWithDataGridViewFrozen_ThrowsInvalidOperationException()
    {
        using DataGridView control = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = new SubDataGridViewCell(),
            Frozen = true
        };
        control.Columns.Add(column);

        Assert.Throws<InvalidOperationException>(() => column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill);
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.AutoSizeMode);
        Assert.Equal(DataGridViewAutoSizeColumnMode.None, column.InheritedAutoSizeMode);
    }

    [WinFormsFact]
    public void DataGridViewColumn_AutoSizeMode_SetNotSetWithDataGridViewFrozen_GetReturnsExpected()
    {
        using DataGridView control = new();
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            CellTemplate = new SubDataGridViewCell(),
            Frozen = true
        };
        control.Columns.Add(column);

        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
        Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.AutoSizeMode);
        Assert.Equal(DataGridViewAutoSizeColumnMode.None, column.InheritedAutoSizeMode);
    }

    [WinFormsFact]
    public void DataGridViewColumn_AutoSizeMode_SetNotSetWithDataGridViewFrozen_ThrowsInvalidOperationException()
    {
        using DataGridView control = new()
        {
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
            CellTemplate = new SubDataGridViewCell(),
            Frozen = true
        };
        control.Columns.Add(column);

        Assert.Throws<InvalidOperationException>(() => column.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet);
        Assert.Equal(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
        Assert.Equal(DataGridViewAutoSizeColumnMode.AllCells, column.InheritedAutoSizeMode);
    }

    public static IEnumerable<object[]> AutoSizeMode_SetWithOldValue_TestData()
    {
        foreach (DataGridViewAutoSizeColumnMode oldValue in Enum.GetValues(typeof(DataGridViewAutoSizeColumnMode)))
        {
            foreach (DataGridViewAutoSizeColumnMode value in Enum.GetValues(typeof(DataGridViewAutoSizeColumnMode)))
            {
                yield return new object[] { oldValue, value };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoSizeMode_SetWithOldValue_TestData))]
    public void DataGridViewColumn_AutoSizeMode_SetWithOldValue_GetReturnsExpected(DataGridViewAutoSizeColumnMode oldValue, DataGridViewAutoSizeColumnMode value)
    {
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = oldValue
        };

        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);

        // Set same.
        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(100, column.Width);
    }

    public static IEnumerable<object[]> AutoSizeMode_SetWithWidth_TestData()
    {
        foreach (DataGridViewAutoSizeColumnMode previous in new DataGridViewAutoSizeColumnMode[] { DataGridViewAutoSizeColumnMode.NotSet, DataGridViewAutoSizeColumnMode.None, DataGridViewAutoSizeColumnMode.Fill })
        {
            yield return new object[] { previous, DataGridViewAutoSizeColumnMode.AllCells, 20 };
            yield return new object[] { previous, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, 20 };
            yield return new object[] { previous, DataGridViewAutoSizeColumnMode.None, 20 };
            yield return new object[] { previous, DataGridViewAutoSizeColumnMode.NotSet, 20 };
            yield return new object[] { previous, DataGridViewAutoSizeColumnMode.Fill, 20 };
        }

        yield return new object[] { DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.AllCells, 10 };
        yield return new object[] { DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, 10 };
        yield return new object[] { DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.None, 20 };
        yield return new object[] { DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.NotSet, 20 };
        yield return new object[] { DataGridViewAutoSizeColumnMode.AllCells, DataGridViewAutoSizeColumnMode.Fill, 20 };
    }

    [WinFormsTheory]
    [MemberData(nameof(AutoSizeMode_SetWithWidth_TestData))]
    public void DataGridViewColumn_AutoSizeMode_SetWithWidth_GetReturnsExpected(DataGridViewAutoSizeColumnMode oldValue, DataGridViewAutoSizeColumnMode value, int expectedWidth)
    {
        using DataGridViewColumn column = new()
        {
            Width = 10,
            AutoSizeMode = oldValue
        };
        column.Width = 20;

        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(expectedWidth, column.Width);

        // Set same.
        column.AutoSizeMode = value;
        Assert.Equal(value, column.AutoSizeMode);
        Assert.Equal(value, column.InheritedAutoSizeMode);
        Assert.Equal(expectedWidth, column.Width);
    }

    [WinFormsFact]
    public void ToolStripItem_AutoSizeMode_SetWithHandler_CallsAutoSizeModeChanged()
    {
        using DataGridView control = new()
        {
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        int callCount = 0;
        object expectedPreviousMode = DataGridViewAutoSizeColumnMode.Fill;
        DataGridViewAutoSizeColumnModeEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(column, e.Column);
            Assert.Equal(expectedPreviousMode, e.PreviousMode);
            callCount++;
        };
        control.AutoSizeColumnModeChanged += handler;

        // Set different.
        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        Assert.Equal(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
        Assert.Equal(1, callCount);

        // Set same.
        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        Assert.Equal(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
        Assert.Equal(1, callCount);

        // Set different.
        expectedPreviousMode = DataGridViewAutoSizeColumnMode.AllCells;
        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        Assert.Equal(DataGridViewAutoSizeColumnMode.Fill, column.AutoSizeMode);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.AutoSizeColumnModeChanged -= handler;
        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        Assert.Equal(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InvalidEnumData<DataGridViewAutoSizeColumnMode>]
    public void DataGridViewColumn_AutoSizeMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(DataGridViewAutoSizeColumnMode value)
    {
        using DataGridViewColumn column = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => column.AutoSizeMode = value);
    }

    public static IEnumerable<object[]> CellTemplate_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new SubDataGridViewCell() };
    }

    [WinFormsTheory]
    [MemberData(nameof(CellTemplate_Set_TestData))]
    public void DataGridViewColumn_CellTemplate_Set_GetReturnsExpected(DataGridViewCell value)
    {
        using SubDataGridViewColumn column = new()
        {
            CellTemplate = value
        };
        Assert.Same(value, column.CellTemplate);
        Assert.Same(value?.GetType(), column.CellType);

        // Set same.
        column.CellTemplate = value;
        Assert.Same(value, column.CellTemplate);
        Assert.Same(value?.GetType(), column.CellType);
    }

    public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ContextMenuStrip() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ContextMenuStrip_Set_TestData))]
    public void DataGridViewColumn_ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
    {
        using DataGridViewColumn column = new()
        {
            ContextMenuStrip = value
        };
        Assert.Same(value, column.ContextMenuStrip);

        // Set same.
        column.ContextMenuStrip = value;
        Assert.Same(value, column.ContextMenuStrip);
    }

    [WinFormsTheory]
    [MemberData(nameof(ContextMenuStrip_Set_TestData))]
    public void DataGridViewColumn_ContextMenuStrip_SetWithCustomOldValue_GetReturnsExpected(ContextMenuStrip value)
    {
        using ContextMenuStrip oldValue = new();
        using DataGridViewColumn column = new()
        {
            ContextMenuStrip = oldValue
        };

        column.ContextMenuStrip = value;
        Assert.Same(value, column.ContextMenuStrip);

        // Set same.
        column.ContextMenuStrip = value;
        Assert.Same(value, column.ContextMenuStrip);
    }

    [WinFormsTheory]
    [MemberData(nameof(ContextMenuStrip_Set_TestData))]
    public void DataGridViewColumn_ContextMenuStrip_SetWithDataGridView_GetReturnsExpected(ContextMenuStrip value)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        column.ContextMenuStrip = value;
        Assert.Same(value, column.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);

        // Set same.
        column.ContextMenuStrip = value;
        Assert.Same(value, column.ContextMenuStrip);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumn_ContextMenuStrip_SetDisposeNew_RemovesContextMenuStrip()
    {
        using ContextMenuStrip menu = new();
        using DataGridViewColumn control = new()
        {
            ContextMenuStrip = menu
        };
        Assert.Same(menu, control.ContextMenuStrip);

        menu.Dispose();
        Assert.Null(control.ContextMenuStrip);
    }

    [WinFormsFact]
    public void DataGridViewColumn_ContextMenuStrip_SetDisposeOld_RemovesContextMenuStrip()
    {
        using ContextMenuStrip menu1 = new();
        using ContextMenuStrip menu2 = new();
        using DataGridViewColumn control = new()
        {
            ContextMenuStrip = menu1
        };
        Assert.Same(menu1, control.ContextMenuStrip);

        control.ContextMenuStrip = menu2;
        Assert.Same(menu2, control.ContextMenuStrip);

        menu1.Dispose();
        Assert.Same(menu2, control.ContextMenuStrip);
    }

    [WinFormsFact]
    public void DataGridViewColumn_ContextMenuStrip_SetWithHandler_CallsContextMenuStripChanged()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int callCount = 0;
        DataGridViewColumnEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(column, e.Column);
            callCount++;
        };
        control.ColumnContextMenuStripChanged += handler;

        // Set different.
        using ContextMenuStrip menu1 = new();
        column.ContextMenuStrip = menu1;
        Assert.Same(menu1, column.ContextMenuStrip);
        Assert.Equal(1, callCount);

        // Set same.
        column.ContextMenuStrip = menu1;
        Assert.Same(menu1, column.ContextMenuStrip);
        Assert.Equal(1, callCount);

        // Set different.
        using ContextMenuStrip menu2 = new();
        column.ContextMenuStrip = menu2;
        Assert.Same(menu2, column.ContextMenuStrip);
        Assert.Equal(2, callCount);

        // Set null.
        column.ContextMenuStrip = null;
        Assert.Null(column.ContextMenuStrip);
        Assert.Equal(3, callCount);

        // Remove handler.
        control.ColumnContextMenuStripChanged -= handler;
        column.ContextMenuStrip = menu1;
        Assert.Same(menu1, column.ContextMenuStrip);
        Assert.Equal(3, callCount);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void DataGridViewColumn_DataPropertyName_SetWithoutDataGridView_GetReturnsExpected(string value, string expected)
    {
        using DataGridViewColumn column = new()
        {
            DataPropertyName = value
        };
        Assert.Equal(expected, column.DataPropertyName);

        // Set same.
        column.DataPropertyName = value;
        Assert.Equal(expected, column.DataPropertyName);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void DataGridViewColumn_DataPropertyName_SetWithDataGridView_GetReturnsExpected(string value, string expected)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        column.DataPropertyName = value;
        Assert.Equal(expected, column.DataPropertyName);
        Assert.False(control.IsHandleCreated);

        // Set same.
        column.DataPropertyName = value;
        Assert.Equal(expected, column.DataPropertyName);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumn_DataPropertyName_SetWithHandler_CallsColumnDataPropertyNameChanged()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        int callCount = 0;
        DataGridViewColumnEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(column, e.Column);
            callCount++;
        };
        control.ColumnDataPropertyNameChanged += handler;

        // Set different.
        column.DataPropertyName = "text";
        Assert.Equal("text", column.DataPropertyName);
        Assert.Equal(1, callCount);

        // Set same.
        column.DataPropertyName = "text";
        Assert.Equal("text", column.DataPropertyName);
        Assert.Equal(1, callCount);

        // Set different.
        column.DataPropertyName = null;
        Assert.Empty(column.DataPropertyName);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ColumnDataPropertyNameChanged -= handler;
        column.DataPropertyName = "text";
        Assert.Equal("text", column.DataPropertyName);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> DefaultCellStyle_Set_TestData()
    {
        yield return new object[] { null, new DataGridViewCellStyle() };

        DataGridViewCellStyle style1 = new() { Alignment = DataGridViewContentAlignment.MiddleCenter };
        DataGridViewCellStyle style2 = new() { Alignment = DataGridViewContentAlignment.BottomLeft };
        yield return new object[] { style1, style1 };
        yield return new object[] { style2, style2 };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultCellStyle_Set_TestData))]
    public void DataGridViewColumn_DefaultCellStyle_Set_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
    {
        using DataGridViewColumn column = new()
        {
            DefaultCellStyle = value
        };
        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);

        // Set same.
        column.DefaultCellStyle = value;
        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultCellStyle_Set_TestData))]
    public void DataGridViewColumn_DefaultCellStyle_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
    {
        DataGridViewCellStyle oldValue = new()
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter
        };
        using DataGridViewColumn column = new()
        {
            DefaultCellStyle = oldValue
        };

        column.DefaultCellStyle = value;
        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);

        // Set same.
        column.DefaultCellStyle = value;
        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultCellStyle_Set_TestData))]
    public void DataGridViewColumn_DefaultCellStyle_SetWithDataGridView_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        column.DefaultCellStyle = value;

        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.False(control.IsHandleCreated);

        // Set same.
        column.DefaultCellStyle = value;
        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultCellStyle_Set_TestData))]
    public void DataGridViewColumn_DefaultCellStyle_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
    {
        DataGridViewCellStyle oldValue = new()
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter
        };
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate,
            DefaultCellStyle = oldValue
        };
        control.Columns.Add(column);

        column.DefaultCellStyle = value;
        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.False(control.IsHandleCreated);

        // Set same.
        column.DefaultCellStyle = value;
        Assert.Equal(expected, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumn_DefaultCellStyle_SetWithDataGridView_CallsColumnDefaultCellStyleChanged()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        int callCount = 0;
        DataGridViewColumnEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(column, e.Column);
            callCount++;
        };
        control.ColumnDefaultCellStyleChanged += handler;

        DataGridViewCellStyle style1 = new()
        {
            Alignment = DataGridViewContentAlignment.MiddleCenter
        };

        // Set non-null.
        column.DefaultCellStyle = style1;
        Assert.Equal(style1, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Equal(1, callCount);

        // Set same.
        column.DefaultCellStyle = style1;
        Assert.Equal(style1, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Equal(1, callCount);

        // Set different.
        DataGridViewCellStyle style2 = new()
        {
            Alignment = DataGridViewContentAlignment.BottomCenter
        };
        column.DefaultCellStyle = style2;
        Assert.Same(style2, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Equal(2, callCount);

        // Set null.
        column.DefaultCellStyle = null;
        Assert.NotNull(column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Equal(3, callCount);

        // Set null again.
        column.DefaultCellStyle = null;
        Assert.NotNull(column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Equal(4, callCount);

        // Set non-null.
        column.DefaultCellStyle = style2;
        Assert.NotNull(column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Equal(5, callCount);

        // Remove handler.
        control.ColumnDefaultCellStyleChanged -= handler;
        column.DefaultCellStyle = style1;
        Assert.Equal(style1, column.DefaultCellStyle);
        Assert.True(column.HasDefaultCellStyle);
        Assert.Equal(5, callCount);
    }

    public static IEnumerable<object[]> DefaultHeaderCellType_Set_TestData()
    {
        yield return new object[] { null, typeof(DataGridViewColumnHeaderCell) };
        yield return new object[] { typeof(DataGridViewRowHeaderCell), typeof(DataGridViewRowHeaderCell) };
        yield return new object[] { typeof(DataGridViewColumnHeaderCell), typeof(DataGridViewColumnHeaderCell) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultHeaderCellType_Set_TestData))]
    public void DataGridViewColumn_DefaultHeaderCellType_Set_GetReturnsExpected(Type value, Type expected)
    {
        using DataGridViewColumn column = new()
        {
            DefaultHeaderCellType = value
        };
        Assert.Equal(expected, column.DefaultHeaderCellType);

        // Set same.
        column.DefaultHeaderCellType = value;
        Assert.Equal(expected, column.DefaultHeaderCellType);
    }

    [WinFormsTheory]
    [InlineData(typeof(DataGridViewRowHeaderCell))]
    [InlineData(typeof(DataGridViewColumnHeaderCell))]
    [InlineData(typeof(DataGridViewHeaderCell))]
    public void DataGridViewColumn_DefaultHeaderCellType_SetWithNonNullOldValue_GetReturnsExpected(Type value)
    {
        using SubDataGridViewColumn column = new()
        {
            DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell)
        };
        column.DefaultHeaderCellType = value;
        Assert.Equal(value, column.DefaultHeaderCellType);

        // Set same.
        column.DefaultHeaderCellType = value;
        Assert.Equal(value, column.DefaultHeaderCellType);
    }

    [WinFormsTheory]
    [MemberData(nameof(DefaultHeaderCellType_Set_TestData))]
    public void DataGridViewColumn_DefaultHeaderCellType_SetWithDataGridView_GetReturnsExpected(Type value, Type expected)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        column.DefaultHeaderCellType = value;
        Assert.Equal(expected, column.DefaultHeaderCellType);

        // Set same.
        column.DefaultHeaderCellType = value;
        Assert.Equal(expected, column.DefaultHeaderCellType);
    }

    [WinFormsTheory]
    [InlineData(typeof(DataGridViewRowHeaderCell))]
    [InlineData(typeof(DataGridViewColumnHeaderCell))]
    [InlineData(typeof(DataGridViewHeaderCell))]
    public void DataGridViewColumn_DefaultHeaderCellType_SetWithDataGridViewNonNullOldValue_GetReturnsExpected(Type value)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        column.DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell);

        column.DefaultHeaderCellType = value;
        Assert.Equal(value, column.DefaultHeaderCellType);

        // Set same.
        column.DefaultHeaderCellType = value;
        Assert.Equal(value, column.DefaultHeaderCellType);
    }

    [WinFormsTheory]
    [InlineData(typeof(int))]
    public void DataGridViewColumn_DefaultHeaderCellType_SetInvalidWithNullOldValue_GetReturnsExpected(Type value)
    {
        using DataGridViewRow column = new();
        Assert.Throws<ArgumentException>("value", () => column.DefaultHeaderCellType = value);
    }

    [WinFormsTheory]
    [InlineData(typeof(int))]
    public void DataGridViewColumn_DefaultHeaderCellType_SetInvalidWithNonNullOldValue_GetReturnsExpected(Type value)
    {
        using DataGridViewColumn column = new()
        {
            DefaultHeaderCellType = typeof(DataGridViewRowHeaderCell)
        };
        Assert.Throws<ArgumentException>("value", () => column.DefaultHeaderCellType = value);
    }

    [WinFormsFact]
    public void DataGridViewColumn_Displayed_Get_ReturnsExpected()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        Assert.False(column.Displayed);
    }

    public static IEnumerable<object[]> DividerWidth_Set_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 65536 };
    }

    [WinFormsTheory]
    [MemberData(nameof(DividerWidth_Set_TestData))]
    public void DataGridViewColumn_DividerWidth_Set_GetReturnsExpected(int value)
    {
        using DataGridViewColumn column = new()
        {
            DividerWidth = value
        };
        Assert.Equal(value, column.DividerWidth);

        // Set same.
        column.DividerWidth = value;
        Assert.Equal(value, column.DividerWidth);
    }

    [WinFormsTheory]
    [MemberData(nameof(DividerWidth_Set_TestData))]
    public void DataGridViewColumn_DividerWidth_SetWithDataGridView_GetReturnsExpected(int value)
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 1
        };
        DataGridViewColumn column = control.Columns[0];

        column.DividerWidth = value;
        Assert.Equal(value, column.DividerWidth);
        Assert.False(control.IsHandleCreated);

        // Set same.
        column.DividerWidth = value;
        Assert.Equal(value, column.DividerWidth);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void DataGridViewColumn_DividerWidth_SetWithDataGridView_CallsColumnDividerWidthChanged()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        int callCount = 0;
        DataGridViewColumnEventHandler handler = (sender, e) =>
        {
            callCount++;
            Assert.Same(control, sender);
            Assert.Same(column, e.Column);
        };
        control.ColumnDividerWidthChanged += handler;

        // Set non-zero.
        column.DividerWidth = 4;
        Assert.Equal(4, column.DividerWidth);
        Assert.Equal(1, callCount);

        // Set same.
        column.DividerWidth = 4;
        Assert.Equal(4, column.DividerWidth);
        Assert.Equal(1, callCount);

        // Set different.
        column.DividerWidth = 3;
        Assert.Equal(3, column.DividerWidth);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ColumnDividerWidthChanged -= handler;
        column.DividerWidth = 4;
        Assert.Equal(4, column.DividerWidth);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(65537)]
    public void DataGridViewColumn_DividerWidth_SetInvalid_ThrowsArgumentOutOfRangeException(int value)
    {
        using DataGridViewColumn column = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => column.DividerWidth = value);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewElementStates.None, false)]
    [InlineData(DataGridViewElementStates.Frozen, true)]
    [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly, true)]
    [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.Selected, true)]
    public void DataGridViewColumn_Frozen_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
    {
        using CustomStateDataGridViewColumn row = new()
        {
            StateResult = state
        };
        Assert.Equal(expected, row.Frozen);
    }

    [WinFormsFact]
    public void DataGridViewColumn_Frozen_GetWithDataGridView_ReturnsExpected()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        Assert.False(control.Columns[0].Frozen);
    }

    public static IEnumerable<object[]> Frozen_Set_TestData()
    {
        foreach (bool visible in new bool[] { true, false })
        {
            foreach (DataGridViewAutoSizeColumnMode autoSizeMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnMode)))
            {
                yield return new object[] { visible, autoSizeMode, true };
                yield return new object[] { visible, autoSizeMode, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Frozen_Set_TestData))]
    public void DataGridViewColumn_Frozen_Set_GetReturnsExpected(bool visible, DataGridViewAutoSizeColumnMode autoSizeMode, bool value)
    {
        using DataGridViewColumn column = new()
        {
            Visible = visible,
            AutoSizeMode = autoSizeMode,
            Frozen = value
        };
        Assert.Equal(value, column.Frozen);
        Assert.Equal(autoSizeMode, column.AutoSizeMode);

        // Set same.
        column.Frozen = value;
        Assert.Equal(value, column.Frozen);
        Assert.Equal(autoSizeMode, column.AutoSizeMode);

        // Set different.
        column.Frozen = !value;
        Assert.Equal(!value, column.Frozen);
        Assert.Equal(autoSizeMode, column.AutoSizeMode);
    }

    public static IEnumerable<object[]> Frozen_SetWithDataGridView_TestData()
    {
        foreach (bool visible in new bool[] { true, false })
        {
            foreach (DataGridViewAutoSizeColumnMode autoSizeMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnMode)))
            {
                if (autoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
                {
                    continue;
                }

                yield return new object[] { visible, autoSizeMode, true, autoSizeMode, autoSizeMode };
                yield return new object[] { visible, autoSizeMode, false, autoSizeMode, autoSizeMode };
            }
        }

        yield return new object[] { true, DataGridViewAutoSizeColumnMode.Fill, true, DataGridViewAutoSizeColumnsMode.None, DataGridViewAutoSizeColumnsMode.None };
        yield return new object[] { true, DataGridViewAutoSizeColumnMode.Fill, false, DataGridViewAutoSizeColumnsMode.Fill, DataGridViewAutoSizeColumnsMode.None };
        yield return new object[] { false, DataGridViewAutoSizeColumnMode.Fill, true, DataGridViewAutoSizeColumnsMode.Fill, DataGridViewAutoSizeColumnsMode.Fill };
        yield return new object[] { false, DataGridViewAutoSizeColumnMode.Fill, false, DataGridViewAutoSizeColumnsMode.Fill, DataGridViewAutoSizeColumnsMode.Fill };
    }

    [WinFormsTheory]
    [MemberData(nameof(Frozen_SetWithDataGridView_TestData))]
    public void DataGridViewColumn_Frozen_SetWithDataGridView_GetReturnsExpected(bool visible, DataGridViewAutoSizeColumnMode autoSizeMode, bool value, DataGridViewAutoSizeColumnMode expectedAutoSizeMode1, DataGridViewAutoSizeColumnMode expectedAutoSizeMode2)
    {
        using DataGridView control = new()
        {
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader
        };
        using DataGridViewColumn column = new()
        {
            Visible = visible,
            AutoSizeMode = autoSizeMode,
            CellTemplate = new SubDataGridViewCell()
        };
        control.Columns.Add(column);

        column.Frozen = value;
        Assert.Equal(value, column.Frozen);
        Assert.Equal(expectedAutoSizeMode1, column.AutoSizeMode);

        // Set same.
        column.Frozen = value;
        Assert.Equal(value, column.Frozen);
        Assert.Equal(expectedAutoSizeMode1, column.AutoSizeMode);

        // Set different.
        column.Frozen = !value;
        Assert.Equal(!value, column.Frozen);
        Assert.Equal(expectedAutoSizeMode2, column.AutoSizeMode);
    }

    [WinFormsTheory]
    [BoolData]
    public void DataGridViewColumn_Frozen_SetWithPreviousColumns_SetsToFrozen(bool previousVisible)
    {
        using DataGridView control = new();
        using DataGridViewColumn column1 = new()
        {
            CellTemplate = new SubDataGridViewCell()
        };
        using DataGridViewColumn column2 = new()
        {
            CellTemplate = new SubDataGridViewCell(),
            Visible = previousVisible
        };
        using DataGridViewColumn column3 = new()
        {
            CellTemplate = new SubDataGridViewCell()
        };
        using DataGridViewColumn column4 = new()
        {
            CellTemplate = new SubDataGridViewCell()
        };
        control.Columns.Add(column1);
        control.Columns.Add(column2);
        control.Columns.Add(column3);
        control.Columns.Add(column4);

        // Freeze middle.
        column3.Frozen = true;
        Assert.True(column1.Frozen);
        Assert.Equal(previousVisible, column2.Frozen);
        Assert.True(column3.Frozen);
        Assert.False(column4.Frozen);

        // Freeze again.
        column3.Frozen = true;
        Assert.True(column1.Frozen);
        Assert.Equal(previousVisible, column2.Frozen);
        Assert.True(column3.Frozen);
        Assert.False(column4.Frozen);

        // Freeze later.
        column4.Frozen = true;
        Assert.True(column1.Frozen);
        Assert.Equal(previousVisible, column2.Frozen);
        Assert.True(column3.Frozen);
        Assert.True(column4.Frozen);

        // Unfreeze middle.
        column3.Frozen = false;
        Assert.True(column1.Frozen);
        Assert.Equal(previousVisible, column2.Frozen);
        Assert.False(column3.Frozen);
        Assert.False(column4.Frozen);

        // Unfreeze first.
        column1.Frozen = false;
        Assert.False(column1.Frozen);
        Assert.False(column2.Frozen);
        Assert.False(column3.Frozen);
        Assert.False(column4.Frozen);
    }

    [WinFormsFact]
    public void DataGridViewColumn_Frozen_SetWithDataGridView_CallsColumnStateChanged()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate,
            Frozen = true
        };
        control.Columns.Add(column);

        int callCount = 0;
        DataGridViewColumnStateChangedEventHandler handler = (sender, e) =>
        {
            callCount++;
            Assert.Same(control, sender);
            Assert.Same(column, e.Column);
            Assert.Equal(DataGridViewElementStates.Frozen, e.StateChanged);
        };
        control.ColumnStateChanged += handler;

        // Set different.
        column.Frozen = false;
        Assert.False(column.Frozen);
        Assert.Equal(1, callCount);

        // Set same.
        column.Frozen = false;
        Assert.False(column.Frozen);
        Assert.Equal(1, callCount);

        // Set different.
        column.Frozen = true;
        Assert.True(column.Frozen);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ColumnStateChanged -= handler;
        column.Frozen = false;
        Assert.False(column.Frozen);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void DataGridViewColumn_HeaderCell_Get_ReturnsExpected()
    {
        using SubDataGridViewColumn column = new();
        Assert.IsType<DataGridViewColumnHeaderCell>(column.HeaderCell);
        Assert.Same(column, column.HeaderCell.OwningColumn);
        Assert.Null(column.HeaderCell.OwningRow);
        Assert.Same(column.HeaderCell, column.HeaderCell);
        Assert.Same(column.HeaderCell, column.HeaderCellCore);
        Assert.Empty(column.HeaderText);
    }

    [WinFormsFact]
    public void DataGridViewColumn_HeaderCellCore_Get_ReturnsExpected()
    {
        using SubDataGridViewColumn column = new();
        Assert.IsType<DataGridViewColumnHeaderCell>(column.HeaderCellCore);
        Assert.Same(column, column.HeaderCellCore.OwningColumn);
        Assert.Null(column.HeaderCellCore.OwningRow);
        Assert.Same(column.HeaderCellCore, column.HeaderCellCore);
        Assert.Same(column.HeaderCell, column.HeaderCellCore);
        Assert.Empty(column.HeaderText);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewAutoSizeColumnsMode.AllCells, DataGridViewAutoSizeColumnMode.AllCells)]
    [InlineData(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader)]
    [InlineData(DataGridViewAutoSizeColumnsMode.ColumnHeader, DataGridViewAutoSizeColumnMode.ColumnHeader)]
    [InlineData(DataGridViewAutoSizeColumnsMode.DisplayedCells, DataGridViewAutoSizeColumnMode.DisplayedCells)]
    [InlineData(DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader, DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader)]
    [InlineData(DataGridViewAutoSizeColumnsMode.Fill, DataGridViewAutoSizeColumnMode.Fill)]
    [InlineData(DataGridViewAutoSizeColumnsMode.None, DataGridViewAutoSizeColumnMode.None)]
    public void DataGridViewColumn_InheritedAutoSizeMode_GetWithDataGridView_ReturnsExpected(DataGridViewAutoSizeColumnsMode mode, DataGridViewAutoSizeColumnMode expected)
    {
        using DataGridView control = new()
        {
            AutoSizeColumnsMode = mode
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        Assert.Equal(expected, column.InheritedAutoSizeMode);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewElementStates.None, false)]
    [InlineData(DataGridViewElementStates.ReadOnly, true)]
    [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Visible, true)]
    [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected, true)]
    public void DataGridViewColumn_ReadOnly_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
    {
        using CustomStateDataGridViewColumn column = new()
        {
            StateResult = state
        };
        Assert.Equal(expected, column.ReadOnly);
    }

    [WinFormsTheory]
    [BoolData]
    public void DataGridViewRow_ReadOnly_GetWithDataGridView_ReturnsExpected(bool dataGridViewReadOnly)
    {
        using DataGridView control = new()
        {
            ReadOnly = dataGridViewReadOnly
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        Assert.Equal(dataGridViewReadOnly, column.ReadOnly);
    }

    [WinFormsTheory]
    [BoolData]
    public void DataGridViewColumn_ReadOnly_Set_GetReturnsExpected(bool value)
    {
        using DataGridViewColumn column = new()
        {
            ReadOnly = value
        };
        Assert.Equal(value, column.ReadOnly);
        Assert.Equal(value, (column.State & DataGridViewElementStates.ReadOnly) != 0);

        // Set same.
        column.ReadOnly = value;
        Assert.Equal(value, column.ReadOnly);
        Assert.Equal(value, (column.State & DataGridViewElementStates.ReadOnly) != 0);

        // Set different.
        column.ReadOnly = !value;
        Assert.Equal(!value, column.ReadOnly);
        Assert.Equal(!value, (column.State & DataGridViewElementStates.ReadOnly) != 0);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void DataGridViewColumn_ReadOnly_SetWithDataGridView_GetReturnsExpected(bool controlReadOnly, bool value)
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        control.ReadOnly = controlReadOnly;

        column.ReadOnly = value;
        Assert.Equal(controlReadOnly || value, column.ReadOnly);

        // Set same.
        column.ReadOnly = value;
        Assert.Equal(controlReadOnly || value, column.ReadOnly);

        // Set different.
        column.ReadOnly = !value;
        Assert.Equal(controlReadOnly || !value, column.ReadOnly);

        control.ReadOnly = false;
        Assert.Equal(!controlReadOnly && !value, column.ReadOnly);
    }

    [WinFormsFact]
    public void DataGridViewColumn_ReadOnly_SetWithDataGridView_CallsColumnStateChanged()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        int callCount = 0;
        DataGridViewColumnStateChangedEventHandler handler = (sender, e) =>
        {
            callCount++;
            Assert.Same(control, sender);
            Assert.Same(column, e.Column);
            Assert.Equal(DataGridViewElementStates.ReadOnly, e.StateChanged);
        };
        control.ColumnStateChanged += handler;

        // Set true.
        column.ReadOnly = true;
        Assert.True(column.ReadOnly);
        Assert.Equal(1, callCount);

        // Set same.
        column.ReadOnly = true;
        Assert.True(column.ReadOnly);
        Assert.Equal(1, callCount);

        // Set different.
        column.ReadOnly = false;
        Assert.False(column.ReadOnly);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ColumnStateChanged -= handler;
        column.ReadOnly = true;
        Assert.True(column.ReadOnly);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [InlineData(DataGridViewElementStates.None, false)]
    [InlineData(DataGridViewElementStates.Visible, true)]
    [InlineData(DataGridViewElementStates.Visible | DataGridViewElementStates.ReadOnly, true)]
    [InlineData(DataGridViewElementStates.Visible | DataGridViewElementStates.Selected, true)]
    public void DataGridViewColumn_Visible_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
    {
        using CustomStateDataGridViewColumn row = new()
        {
            StateResult = state
        };
        Assert.Equal(expected, row.Visible);
    }

    [WinFormsTheory]
    [BoolData]
    public void DataGridViewColumn_Visible_GetWithDataGridView_ReturnsExpected(bool visible)
    {
        using DataGridView control = new()
        {
            Visible = visible
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);
        Assert.True(control.Columns[0].Visible);
    }

    public static IEnumerable<object[]> Visible_Set_TestData()
    {
        foreach (DataGridViewAutoSizeColumnMode autoSizeMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnMode)))
        {
            foreach (bool frozen in new bool[] { true, false })
            {
                yield return new object[] { autoSizeMode, frozen, true };
                yield return new object[] { autoSizeMode, frozen, false };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_Set_TestData))]
    public void DataGridViewColumn_Visible_Set_GetReturnsExpected(DataGridViewAutoSizeColumnMode autoSizeMode, bool frozen, bool value)
    {
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = autoSizeMode,
            Frozen = frozen,
            Visible = value
        };
        Assert.Equal(value, column.Visible);
        Assert.Equal(autoSizeMode, column.AutoSizeMode);

        // Set same.
        column.Visible = value;
        Assert.Equal(value, column.Visible);
        Assert.Equal(autoSizeMode, column.AutoSizeMode);

        // Set different.
        column.Visible = !value;
        Assert.Equal(!value, column.Visible);
        Assert.Equal(autoSizeMode, column.AutoSizeMode);
    }

    public static IEnumerable<object[]> Visible_SetWithDataGridView_TestData()
    {
        foreach (bool visible in new bool[] { true, false })
        {
            foreach (bool columnHeadersVisible in new bool[] { true, false })
            {
                foreach (DataGridViewAutoSizeColumnMode autoSizeMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnMode)))
                {
                    if (!columnHeadersVisible && autoSizeMode == DataGridViewAutoSizeColumnMode.ColumnHeader)
                    {
                        continue;
                    }

                    yield return new object[] { visible, columnHeadersVisible, autoSizeMode, true, autoSizeMode, autoSizeMode };
                    yield return new object[] { visible, columnHeadersVisible, autoSizeMode, false, autoSizeMode, autoSizeMode };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Visible_SetWithDataGridView_TestData))]
    public void DataGridViewColumn_Visible_SetWithDataGridView_GetReturnsExpected(bool visible, bool columnHeadersVisible, DataGridViewAutoSizeColumnMode autoSizeMode, bool value, DataGridViewAutoSizeColumnMode expectedAutoSizeMode1, DataGridViewAutoSizeColumnMode expectedAutoSizeMode2)
    {
        using DataGridView control = new()
        {
            Visible = visible,
            ColumnHeadersVisible = columnHeadersVisible,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader
        };
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            AutoSizeMode = autoSizeMode,
            CellTemplate = cellTemplate
        };
        control.Columns.Add(column);

        column.Visible = value;
        Assert.Equal(value, column.Visible);
        Assert.Equal(expectedAutoSizeMode1, column.AutoSizeMode);

        // Set same.
        column.Visible = value;
        Assert.Equal(value, column.Visible);
        Assert.Equal(expectedAutoSizeMode1, column.AutoSizeMode);

        // Set different.
        column.Visible = !value;
        Assert.Equal(!value, column.Visible);
        Assert.Equal(expectedAutoSizeMode2, column.AutoSizeMode);
    }

    [WinFormsTheory]
    [BoolData]
    public void DataGridViewColumn_Visible_SetWithPreviousColumns_SetsToVisible(bool previousVisible)
    {
        using DataGridView control = new();
        using DataGridViewColumn column1 = new()
        {
            CellTemplate = new SubDataGridViewCell()
        };
        using DataGridViewColumn column2 = new()
        {
            CellTemplate = new SubDataGridViewCell(),
            Visible = previousVisible
        };
        using DataGridViewColumn column3 = new()
        {
            CellTemplate = new SubDataGridViewCell()
        };
        using DataGridViewColumn column4 = new()
        {
            CellTemplate = new SubDataGridViewCell()
        };
        control.Columns.Add(column1);
        control.Columns.Add(column2);
        control.Columns.Add(column3);
        control.Columns.Add(column4);

        // Freeze middle.
        column3.Visible = true;
        Assert.True(column1.Visible);
        Assert.Equal(previousVisible, column2.Visible);
        Assert.True(column3.Visible);
        Assert.True(column4.Visible);

        // Freeze again.
        column3.Visible = true;
        Assert.True(column1.Visible);
        Assert.Equal(previousVisible, column2.Visible);
        Assert.True(column3.Visible);
        Assert.True(column4.Visible);

        // Freeze later.
        column4.Visible = true;
        Assert.True(column1.Visible);
        Assert.Equal(previousVisible, column2.Visible);
        Assert.True(column3.Visible);
        Assert.True(column4.Visible);

        // Unfreeze middle.
        column3.Visible = false;
        Assert.True(column1.Visible);
        Assert.Equal(previousVisible, column2.Visible);
        Assert.False(column3.Visible);
        Assert.True(column4.Visible);

        // Unfreeze first.
        column1.Visible = false;
        Assert.False(column1.Visible);
        Assert.Equal(previousVisible, column2.Visible);
        Assert.False(column3.Visible);
        Assert.True(column4.Visible);
    }

    [WinFormsFact]
    public void DataGridViewColumn_Visible_SetWithDataGridView_CallsColumnStateChanged()
    {
        using DataGridView control = new();
        using SubDataGridViewCell cellTemplate = new();
        using DataGridViewColumn column = new()
        {
            CellTemplate = cellTemplate,
            Visible = true
        };
        control.Columns.Add(column);

        int callCount = 0;
        DataGridViewColumnStateChangedEventHandler handler = (sender, e) =>
        {
            callCount++;
            Assert.Same(control, sender);
            Assert.Same(column, e.Column);
            Assert.Equal(DataGridViewElementStates.Visible, e.StateChanged);
        };
        control.ColumnStateChanged += handler;

        // Set different.
        column.Visible = false;
        Assert.False(column.Visible);
        Assert.Equal(1, callCount);

        // Set same.
        column.Visible = false;
        Assert.False(column.Visible);
        Assert.Equal(1, callCount);

        // Set different.
        column.Visible = true;
        Assert.True(column.Visible);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ColumnStateChanged -= handler;
        column.Visible = false;
        Assert.False(column.Visible);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> GetPreferred_WithoutDataGridView_TestData()
    {
        foreach (bool fixedHeight in new bool[] { true, false })
        {
            yield return new object[] { DataGridViewAutoSizeColumnMode.ColumnHeader, fixedHeight };
            yield return new object[] { DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, fixedHeight };
            yield return new object[] { DataGridViewAutoSizeColumnMode.AllCells, fixedHeight };
            yield return new object[] { DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader, fixedHeight };
            yield return new object[] { DataGridViewAutoSizeColumnMode.DisplayedCells, fixedHeight };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(GetPreferred_WithoutDataGridView_TestData))]
    public void DataGridViewColumn_GetPreferredWidth_InvokeWithoutDataGridView_ReturnsExpected(DataGridViewAutoSizeColumnMode autoSizeColumnMode, bool fixedHeight)
    {
        using SubDataGridViewColumn column = new();
        Assert.Equal(-1, column.GetPreferredWidth(autoSizeColumnMode, fixedHeight));
    }

    [WinFormsTheory]
    [InlineData(DataGridViewAutoSizeColumnMode.NotSet)]
    [InlineData(DataGridViewAutoSizeColumnMode.None)]
    [InlineData(DataGridViewAutoSizeColumnMode.Fill)]
    public void DataGridViewColumn_GetPreferredWidth_NotApplicableAutoSizeColumnMode_ThrowsArgumentException(DataGridViewAutoSizeColumnMode autoSizeColumnMode)
    {
        using SubDataGridViewColumn column = new();
        Assert.Throws<ArgumentException>(() => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: true));
        Assert.Throws<ArgumentException>(() => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: false));
    }

    [WinFormsTheory]
    [InvalidEnumData<DataGridViewAutoSizeColumnMode>]
    public void DataGridViewColumn_GetPreferredWidth_NotApplicableAutoSizeColumnMode_ThrowsInvalidEnumArgumentException(DataGridViewAutoSizeColumnMode autoSizeColumnMode)
    {
        using SubDataGridViewColumn column = new();
        Assert.Throws<InvalidEnumArgumentException>("autoSizeColumnMode", () => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: true));
        Assert.Throws<InvalidEnumArgumentException>("autoSizeColumnMode", () => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: false));
    }

    [WinFormsFact]
    public void DataGridViewColumn_ToString_Invoke_ReturnsExpected()
    {
        using DataGridViewColumn column = new();
        Assert.Equal("DataGridViewColumn { Name=, Index=-1 }", column.ToString());
    }

    [WinFormsFact]
    public void DataGridViewColumn_ToString_InvokeWithName_ReturnsExpected()
    {
        using DataGridViewColumn column = new()
        {
            Name = "name"
        };
        Assert.Equal("DataGridViewColumn { Name=name, Index=-1 }", column.ToString());
    }

    private class CustomStateDataGridViewColumn : DataGridViewColumn
    {
        public DataGridViewElementStates StateResult { get; set; }

        public override DataGridViewElementStates State => StateResult;
    }

    private class SubDataGridViewCell : DataGridViewCell
    {
    }

    private class SubDataGridViewColumn : DataGridViewColumn
    {
        public SubDataGridViewColumn() : base()
        {
        }

        public SubDataGridViewColumn(DataGridViewCell cellTemplate) : base(cellTemplate)
        {
        }

        public new DataGridViewHeaderCell HeaderCellCore => base.HeaderCellCore;

        public new bool IsRow => base.IsRow;
    }
}
