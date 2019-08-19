// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewCellTests
    {
        [Fact]
        public void DataGridViewCell_Ctor_Default()
        {
            var cell = new SubDataGridViewCell();
            Assert.Equal(DataGridViewElementStates.None, cell.State);
            Assert.Null(cell.DataGridView);
            Assert.Equal(-1, cell.ColumnIndex);
            Assert.Null(cell.OwningColumn);
            Assert.Equal(-1, cell.RowIndex);
            Assert.Null(cell.OwningRow);

            Assert.Null(cell.ContextMenuStrip);
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
            Assert.Null(cell.DefaultNewRowValue);
            Assert.Equal(typeof(DataGridViewTextBoxEditingControl), cell.EditType);
            Assert.Null(cell.FormattedValueType);
            Assert.False(cell.HasStyle);
            Assert.False(cell.IsInEditMode);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.Null(cell.Tag);
            Assert.Empty(cell.ToolTipText);
            Assert.Null(cell.ValueType);
        }

        public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
        {
            yield return new object[] { new SubDataGridViewCell() };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0] };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0] };
            yield return new object[] { dataGridView.Rows.SharedRow(1).Cells[0] };
            yield return new object[] { dataGridView.Columns[0].HeaderCell };
        }

        [Theory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_SetWithoutDataGridView_GetReturnsExpected(DataGridViewCell cell)
        {
            // Set non-null.
            var menu1 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);

            // Set same.
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);

            // Set different.
            var menu2 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu2;
            Assert.Same(menu2, cell.ContextMenuStrip);

            // Set null.
            cell.ContextMenuStrip = null;
            Assert.Null(cell.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewCell_ContextMenuStrip_SetWithDataGridView_CallsCellContextMenuStripChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewCell cell = dataGridView.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(0, e.ColumnIndex);
            };
            dataGridView.CellContextMenuStripChanged += handler;

            // Set non-null.
            var menu1 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set same.
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set different.
            var menu2 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu2;
            Assert.Same(menu2, cell.ContextMenuStrip);
            Assert.Equal(2, callCount);

            // Set null.
            cell.ContextMenuStrip = null;
            Assert.Null(cell.ContextMenuStrip);
            Assert.Equal(3, callCount);

            // Remove handler.
            dataGridView.CellContextMenuStripChanged -= handler;
            cell.ContextMenuStrip = menu1;
            Assert.Equal(menu1, cell.ContextMenuStrip);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void DataGridViewCell_ContextMenuStrip_Dispose_SetsToNull()
        {
            var cell = new SubDataGridViewCell();
            var menu = new ContextMenuStrip();
            cell.ContextMenuStrip = menu;
            Assert.Same(menu, cell.ContextMenuStrip);

            menu.Dispose();
            Assert.Null(cell.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewCell_ContextMenuStrip_ResetThenDispose_Nop()
        {
            var cell = new SubDataGridViewCell();
            var menu1 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);

            var menu2 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu2;

            menu1.Dispose();
            Assert.Same(menu2, cell.ContextMenuStrip);
        }

        public static IEnumerable<object[]> Displayed_Get_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), false };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0], false };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell, false };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], false };
            yield return new object[] { dataGridView.Rows.SharedRow(1).Cells[0], false };
            yield return new object[] { dataGridView.Columns[0].HeaderCell, false };
        }

        [Theory]
        [MemberData(nameof(Displayed_Get_TestData))]
        public void DataGridViewCell_Displayed_Get_ReturnsExpected(DataGridViewCell cell, bool expected)
        {
            Assert.Equal(expected, cell.Displayed);
        }

        public static IEnumerable<object[]> EditedFormattedValue_Get_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), null };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0], null };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell, null };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], string.Empty };
            yield return new object[] { dataGridView.Columns[0].HeaderCell, string.Empty };

            DataGridViewCell cell = dataGridView.Rows[1].Cells[0];
            cell.Value = "value";
            yield return new object[] { cell, "value" };
        }

        [Theory]
        [MemberData(nameof(EditedFormattedValue_Get_TestData))]
        public void DataGridViewCell_EditedFormattedValue_Get_ReturnsExpected(DataGridViewCell cell, object expected)
        {
            Assert.Equal(expected, cell.EditedFormattedValue);
        }

        [Fact]
        public void DataGridViewCell_EditedFormattedValue_GetSharedRow_ThrowsArgumentOutOfRangeException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewCell cell = dataGridView.Rows.SharedRow(1).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.EditedFormattedValue);
        }

        public static IEnumerable<object[]> ErrorIconBounds_Get_TestData()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], Rectangle.Empty };
            yield return new object[] { dataGridView.Columns[0].HeaderCell, Rectangle.Empty };
        }

        [Theory]
        [MemberData(nameof(ErrorIconBounds_Get_TestData))]
        public void DataGridViewCell_ErrorIconBounds_Get_ReturnsExpected(DataGridViewCell cell, Rectangle expected)
        {
            Assert.Equal(expected, cell.ErrorIconBounds);
        }

        [Fact]
        public void DataGridViewCell_ErrorIconBounds_GetSharedRow_ThrowsArgumentOutOfRangeException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewCell cell = dataGridView.Rows.SharedRow(1).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.ErrorIconBounds);
        }

        public static IEnumerable<object[]> NoDataGridView_TestData()
        {
            yield return new object[] { new SubDataGridViewCell() };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0] };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell };
        }

        [Theory]
        [MemberData(nameof(NoDataGridView_TestData))]
        public void DataGridViewCell_ErrorIconBounds_GetNoDataGridView_ThrowsInvalidOperationException(DataGridViewCell cell)
        {
            Assert.Throws<InvalidOperationException>(() => cell.ErrorIconBounds);
        }

        public static IEnumerable<object[]> ErrorText_Get_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), string.Empty };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0], string.Empty };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell, string.Empty };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], string.Empty };
            yield return new object[] { dataGridView.Rows.SharedRow(1).Cells[0], string.Empty };
            yield return new object[] { dataGridView.Columns[0].HeaderCell, string.Empty };
        }

        [Theory]
        [MemberData(nameof(ErrorText_Get_TestData))]
        public void DataGridViewCell_ErrorText_Get_ReturnsExpected(DataGridViewCell cell, string expected)
        {
            Assert.Equal(expected, cell.ErrorText);
        }

        public static IEnumerable<object[]> ErrorText_GetNeedsErrorText_TestData()
        {
            yield return new object[] { new DataGridView { ColumnCount = 1, VirtualMode = true } };

            var bound = new DataGridView { DataSource = new[] { new { Name = "Name" } } };
            new Form().Controls.Add(bound);
            Assert.NotNull(bound.BindingContext);
            yield return new object[] { bound };
        }

        [Theory]
        [MemberData(nameof(ErrorText_GetNeedsErrorText_TestData))]
        public void DataGridViewCell_ErrorText_GetNeedsErrorText_CallsCellErrorTextNeeded(DataGridView dataGridView)
        {
            DataGridViewCell cell = dataGridView.Rows[0].Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.ColumnIndex);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            dataGridView.CellErrorTextNeeded += handler;

            Assert.Same("errorText2", cell.ErrorText);
            Assert.Equal(1, callCount);

            // Remove the handler.
            dataGridView.CellErrorTextNeeded -= handler;
            Assert.Same("errorText1", cell.ErrorText);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ErrorText_Set_TestData()
        {
            foreach (string errorText in new string[] { null, "", "reasonable" })
            {
                yield return new object[] { new SubDataGridViewCell(), errorText, errorText ?? string.Empty };

                var row = new DataGridViewRow();
                row.Cells.Add(new SubDataGridViewCell());
                yield return new object[] { row.Cells[0], errorText, errorText ?? string.Empty };

                var column = new DataGridViewColumn();
                yield return new object[] { column.HeaderCell, errorText, errorText ?? string.Empty };

                var dataGridView = new DataGridView { ColumnCount = 1 };
                dataGridView.Rows.Add(new DataGridViewRow());
                yield return new object[] { dataGridView.Rows[0].Cells[0], errorText, errorText ?? string.Empty };
                yield return new object[] { dataGridView.Rows.SharedRow(1).Cells[0], errorText, errorText ?? string.Empty };
                yield return new object[] { dataGridView.Columns[0].HeaderCell, errorText, errorText ?? string.Empty };
            }
        }

        [Theory]
        [MemberData(nameof(ErrorText_Set_TestData))]
        public void DataGridViewCell_ErrorText_Set_GetReturnsExpected(DataGridViewCell cell, string value, string expected)
        {
            cell.ErrorText = value;
            Assert.Same(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Same(expected, cell.ErrorText);
        }

        [Theory]
        [MemberData(nameof(ErrorText_Set_TestData))]
        public void DataGridViewCell_ErrorText_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewCell cell, string value, string expected)
        {
            cell.ErrorText = "value";
            cell.ErrorText = value;
            Assert.Same(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Same(expected, cell.ErrorText);
        }

        [Fact]
        public void DataGridViewCell_ErrorText_SetWithDataGridView_CallsCellErrorTextChanged()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            DataGridViewCell cell = dataGridView.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Equal(0, e.ColumnIndex);
                Assert.Equal(0, e.RowIndex);
            };
            dataGridView.CellErrorTextChanged += handler;

            // Set non-null.
            cell.ErrorText = "errorText";
            Assert.Equal("errorText", cell.ErrorText);
            Assert.Equal(1, callCount);

            // Set same.
            cell.ErrorText = "errorText";
            Assert.Equal("errorText", cell.ErrorText);
            Assert.Equal(1, callCount);

            // Set null.
            cell.ErrorText = null;
            Assert.Empty(cell.ErrorText);
            Assert.Equal(2, callCount);

            // Set different.
            cell.ErrorText = "other";
            Assert.Equal("other", cell.ErrorText);
            Assert.Equal(3, callCount);

            // Remove handler.
            dataGridView.CellErrorTextChanged -= handler;
            cell.ErrorText = "errorText";
            Assert.Equal("errorText", cell.ErrorText);
            Assert.Equal(3, callCount);
        }

        public static IEnumerable<object[]> FormattedValue_Get_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), null };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0], null };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell, null };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], string.Empty };
            yield return new object[] { dataGridView.Columns[0].HeaderCell, string.Empty };

            DataGridViewCell cell = dataGridView.Rows[1].Cells[0];
            cell.Value = "value";
            yield return new object[] { cell, "value" };
        }

        [Theory]
        [MemberData(nameof(FormattedValue_Get_TestData))]
        public void DataGridViewCell_FormattedValue_Get_ReturnsExpected(DataGridViewCell cell, object expected)
        {
            Assert.Equal(expected, cell.FormattedValue);
        }

        [Fact]
        public void DataGridViewCell_FormattedValue_GetSharedRow_ThrowsArgumentOutOfRangeException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewCell cell = dataGridView.Rows.SharedRow(1).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.FormattedValue);
        }

        public static IEnumerable<object[]> Frozen_Get_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), false };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0], false };

            var frozenRow1 = new DataGridViewRow { Frozen = true };
            var frozenCell1 = new SubDataGridViewCell();
            frozenRow1.Cells.Add(frozenCell1);
            yield return new object[] { frozenCell1, true };

            var frozenRow2 = new DataGridViewRow();
            var frozenCell2 = new SubDataGridViewCell();
            frozenRow2.Cells.Add(frozenCell2);
            frozenRow2.Frozen = true;
            yield return new object[] { frozenCell2, true };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell, false };

            var frozenColumn = new DataGridViewColumn { Frozen = true };
            yield return new object[] { frozenColumn.HeaderCell, true };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], false };
            yield return new object[] { dataGridView.Rows.SharedRow(1).Cells[0], false };
            yield return new object[] { dataGridView.Columns[0].HeaderCell, false };

            var frozenRowDataGridView = new DataGridView { ColumnCount = 1 };
            frozenRowDataGridView.Rows[0].Frozen = true;
            yield return new object[] { frozenRowDataGridView.Rows[0].Cells[0], false };

            var frozenColumnDataGridView = new DataGridView { ColumnCount = 1 };
            frozenColumnDataGridView.Columns[0].Frozen = true;
            yield return new object[] { frozenColumnDataGridView.Rows[0].Cells[0], false };

            var frozenDataGridView = new DataGridView { ColumnCount = 1 };
            frozenDataGridView.Columns[0].Frozen = true;
            frozenDataGridView.Rows[0].Frozen = true;
            yield return new object[] { frozenDataGridView.Rows[0].Cells[0], true };
        }

        [Theory]
        [MemberData(nameof(Frozen_Get_TestData))]
        public void DataGridViewCell_Frozen_Get_ReturnsExpected(DataGridViewCell cell, bool expected)
        {
            Assert.Equal(expected, cell.Frozen);
        }

        public static IEnumerable<object[]> InheritedState_Get_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), DataGridViewElementStates.ResizableSet };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { cell, DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible };

            var customRow = new DataGridViewRow { Frozen = true, ReadOnly = true, Visible = false };
            customRow.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { customRow.Cells[0], DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet };

            var resizableRow = new DataGridViewRow { Resizable = DataGridViewTriState.True };
            resizableRow.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { resizableRow.Cells[0], DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell, DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible };

            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            dataGridView.Rows.Add(new DataGridViewRow());
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible };

            DataGridViewRow customDataGridViewRow = dataGridView.Rows[1];
            customDataGridViewRow.Frozen = true;
            customDataGridViewRow.ReadOnly = true;
            customDataGridViewRow.Selected = true;
            customDataGridViewRow.Resizable = DataGridViewTriState.False;
            customDataGridViewRow.Visible = false;
            yield return new object[] { customDataGridViewRow.Cells[0], DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Selected };

            DataGridViewCell dataGridViewCustomCell = dataGridView.Rows[2].Cells[0];
            dataGridViewCustomCell.ReadOnly = true;
            dataGridViewCustomCell.Selected = true;
            yield return new object[] { dataGridViewCustomCell, DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Selected | DataGridViewElementStates.Visible };

            var frozenDataGridView = new DataGridView { ColumnCount = 2 };
            frozenDataGridView.Rows.Add(new DataGridViewRow());
            frozenDataGridView.Rows.Add(new DataGridViewRow());

            DataGridViewColumn frozenDataGridViewColumn = frozenDataGridView.Columns[0];
            frozenDataGridViewColumn.Frozen = true;
            frozenDataGridViewColumn.ReadOnly = true;
            frozenDataGridViewColumn.Selected = true;
            frozenDataGridViewColumn.Resizable = DataGridViewTriState.False;
            frozenDataGridViewColumn.Visible = false;
            yield return new object[] { frozenDataGridView.Rows[0].Cells[0], DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet };
            yield return new object[] { frozenDataGridViewColumn.HeaderCell, DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet };

            DataGridViewColumn nonResizableDataGridViewColumn = frozenDataGridView.Columns[1];
            nonResizableDataGridViewColumn.Resizable = DataGridViewTriState.False;

            DataGridViewRow nonResizableDataGridViewRow = frozenDataGridView.Rows[1];
            nonResizableDataGridViewRow.Resizable = DataGridViewTriState.False;
            nonResizableDataGridViewRow.Frozen = true;
            yield return new object[] { nonResizableDataGridViewRow.Cells[0], DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet };
            yield return new object[] { nonResizableDataGridViewRow.Cells[1], DataGridViewElementStates.Visible | DataGridViewElementStates.ResizableSet };
        }

        [Theory]
        [MemberData(nameof(InheritedState_Get_TestData))]
        public void DataGridViewCell_InheritedState_Get_ReturnsExpected(DataGridViewCell cell, DataGridViewElementStates expected)
        {
            Assert.Equal(expected, cell.InheritedState);
        }

        [Fact]
        public void DataGridViewCell_InheritedState_GetSharedRow_ThrowsArgumentOutOfRangeException()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewCell cell = dataGridView.Rows.SharedRow(1).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.InheritedState);
        }

        [Fact]
        public void DataGridViewCell_InheritedStyle_NoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.InheritedStyle);
        }

        [Fact]
        public void DataGridViewCell_IsInEditMode_GetSharedRow_ThrowsInvalidOperationExceptio()
        {
            var dataGridView = new DataGridView { ColumnCount = 1 };
            dataGridView.Rows.Add(new DataGridViewRow());
            DataGridViewCell cell = dataGridView.Rows.SharedRow(1).Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.IsInEditMode);
        }

        public static IEnumerable<object[]> ReadOnly_Get_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), false };

            var row = new DataGridViewRow();
            row.Cells.Add(new SubDataGridViewCell());
            yield return new object[] { row.Cells[0], false };

            var readOnlyRow1 = new DataGridViewRow { ReadOnly = true };
            var readOnlyCell1 = new SubDataGridViewCell();
            readOnlyRow1.Cells.Add(readOnlyCell1);
            yield return new object[] { readOnlyCell1, true };

            var readOnlyRow2 = new DataGridViewRow();
            var readOnlyCell2 = new SubDataGridViewCell();
            readOnlyRow2.Cells.Add(readOnlyCell2);
            readOnlyRow2.ReadOnly = true;
            yield return new object[] { readOnlyCell2, true };

            var column = new DataGridViewColumn();
            yield return new object[] { column.HeaderCell, true };

            var dataGridView = new DataGridView { ColumnCount = 3 };
            dataGridView.Rows.Add(new DataGridViewRow());
            dataGridView.Rows.Add(new DataGridViewRow());
            yield return new object[] { dataGridView.Rows[0].Cells[0], false };
            yield return new object[] { dataGridView.Rows[1].Cells[0], false };
            yield return new object[] { dataGridView.Columns[0].HeaderCell, true };

            DataGridViewCell readOnlyCell = dataGridView.Rows[1].Cells[1];
            readOnlyCell.ReadOnly = true;
            yield return new object[] { readOnlyCell, true };

            DataGridViewColumn readOnlyColumn = dataGridView.Columns[2];
            readOnlyColumn.ReadOnly = true;
            yield return new object[] { dataGridView.Rows[2].Cells[0], false };

            var readOnlyDataGridView = new DataGridView { ColumnCount = 1 };
            readOnlyDataGridView.Rows.Add(new DataGridViewRow());
            readOnlyDataGridView.ReadOnly = true;
            yield return new object[] { readOnlyDataGridView.Rows[0].Cells[0], true };
        }

        [Theory]
        [MemberData(nameof(ReadOnly_Get_TestData))]
        public void DataGridViewCell_ReadOnly_Get_ReturnsExpected(DataGridViewCell cell, bool expected)
        {
            Assert.Equal(expected, cell.ReadOnly);
        }

        [Fact]
        public void DataGridViewCell_ReadOnly_SetWithoutOwningRow_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = true);
            Assert.False(cell.ReadOnly);

            cell.ReadOnly = false;
            Assert.False(cell.ReadOnly);
        }

        public static IEnumerable<object[]> Resizable_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), false };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { cell, false };

            var nonResizableRow = new DataGridViewRow { Resizable = DataGridViewTriState.False };
            var nonResizableCell = new SubDataGridViewCell();
            nonResizableRow.Cells.Add(nonResizableCell);
            yield return new object[] { nonResizableCell, false };

            var resizableRow = new DataGridViewRow { Resizable = DataGridViewTriState.True };
            var resizableCell = new SubDataGridViewCell();
            resizableRow.Cells.Add(resizableCell);
            yield return new object[] { resizableCell, true };
        }

        [Theory]
        [MemberData(nameof(Resizable_TestData))]
        public void DataGridViewCell_Resizable_Get_ReturnsExpected(DataGridViewCell cell, bool expected)
        {
            Assert.Equal(expected, cell.Resizable);
        }

        public static IEnumerable<object[]> Selected_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), false };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { cell, false };
        }

        [Theory]
        [MemberData(nameof(Selected_TestData))]
        public void DataGridViewCell_Selected_Get_ReturnsExpected(DataGridViewCell cell, bool expected)
        {
            Assert.Equal(expected, cell.Selected);
        }

        [Fact]
        public void DataGridViewCell_Selected_SetNoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.Selected = true);
            Assert.False(cell.Selected);

            cell.Selected = false;
            Assert.False(cell.Selected);
        }

        [Fact]
        public void DataGridViewCell_Style_Get_ReturnsSameInstance()
        {
            var cell = new SubDataGridViewCell();
            Assert.Same(cell.Style, cell.Style);
            Assert.Equal(DataGridViewCellStyleScopes.Cell, cell.Style.Scope);
        }

        public static IEnumerable<object[]> Style_TestData()
        {
            yield return new object[] { null, new DataGridViewCellStyle() };

            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            yield return new object[] { style, style };
        }

        [Theory]
        [MemberData(nameof(Style_TestData))]
        public void DataGridViewCell_Style_SetWithoutDataGrid_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var cell = new SubDataGridViewCell
            {
                Style = value
            };
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [Theory]
        [MemberData(nameof(Style_TestData))]
        public void DataGridViewCell_Style_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var cell = new SubDataGridViewCell
            {
                Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomCenter }
            };
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewCell_Tag_Set_GetReturnsExpected(object value)
        {
            var cell = new SubDataGridViewCell
            {
                Tag = value
            };
            Assert.Same(value, cell.Tag);

            // Set same.
            cell.Tag = value;
            Assert.Same(value, cell.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewCell_Tag_SetWithNonNullOldValue_GetReturnsExpected(object value)
        {
            var cell = new SubDataGridViewCell
            {
                Tag = "tag"
            };
            cell.Tag = value;
            Assert.Same(value, cell.Tag);

            // Set same.
            cell.Tag = value;
            Assert.Same(value, cell.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_Set_GetReturnsExpected(string value, string expected)
        {
            var cell = new SubDataGridViewCell
            {
                ToolTipText = value
            };
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            var cell = new SubDataGridViewCell
            {
                ToolTipText = "ToolTipText"
            };
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        public static IEnumerable<object[]> Value_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), null };
            yield return new object[] { new SubDataGridViewCell(), "value" };
        }

        [Theory]
        [MemberData(nameof(Value_TestData))]
        public void DataGridViewCell_Value_SetWithoutDataGridView_GetReturnsExpected(DataGridViewCell cell, object value)
        {
            cell.Value = value;
            Assert.Equal(value, cell.Value);
            Assert.Null(cell.ValueType);
            Assert.Null(cell.FormattedValueType);

            // Set same.
            cell.Value = value;
            Assert.Equal(value, cell.Value);
            Assert.Null(cell.ValueType);
            Assert.Null(cell.FormattedValueType);
        }

        [Theory]
        [MemberData(nameof(Value_TestData))]
        public void DataGridViewCell_Value_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewCell cell, object value)
        {
            cell.Value = "oldValue";

            cell.Value = value;
            Assert.Equal(value, cell.Value);
            Assert.Null(cell.ValueType);
            Assert.Null(cell.FormattedValueType);

            // Set same.
            cell.Value = value;
            Assert.Equal(value, cell.Value);
            Assert.Null(cell.ValueType);
            Assert.Null(cell.FormattedValueType);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_SetWithoutDataGridView_GetReturnsExpected(Type value)
        {
            var cell = new SubDataGridViewCell
            {
                ValueType = value
            };
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_SetWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            var cell = new SubDataGridViewCell
            {
                ValueType = typeof(string)
            };
            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
        }

        public static IEnumerable<object[]> Visible_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), false };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { cell, true };

            var invisibleRow = new DataGridViewRow { Visible = false };
            var invisibleCell = new SubDataGridViewCell();
            invisibleRow.Cells.Add(invisibleCell);
            yield return new object[] { invisibleCell, false };
        }

        [Theory]
        [MemberData(nameof(Visible_TestData))]
        public void DataGridViewCell_Visible_Get_ReturnsExpected(DataGridViewCell cell, bool expected)
        {
            Assert.Equal(expected, cell.Visible);
        }

        [Theory]
        [InlineData(true, true, true, true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(true, true, false, true, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(false, true, true, true, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(true, true, true, false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(true, false, true, true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single)]
        public void DataGridViewCell_AdjustCellBorderStyle_AllSingleWithoutDataGridView_ReturnsExpected(bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow, DataGridViewAdvancedCellBorderStyle expectedLeft, DataGridViewAdvancedCellBorderStyle expectedRight, DataGridViewAdvancedCellBorderStyle expectedTop, DataGridViewAdvancedCellBorderStyle expectedBottom)
        {
            var cell = new SubDataGridViewCell();
            var dataGridViewAdvancedBorderStyleInput = new DataGridViewAdvancedBorderStyle
            {
                All = DataGridViewAdvancedCellBorderStyle.Single
            };
            var dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Same(dataGridViewAdvancedBorderStylePlaceholder, cell.AdjustCellBorderStyle(dataGridViewAdvancedBorderStyleInput, dataGridViewAdvancedBorderStylePlaceholder, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow));
            Assert.Equal(expectedLeft, dataGridViewAdvancedBorderStylePlaceholder.Left);
            Assert.Equal(expectedRight, dataGridViewAdvancedBorderStylePlaceholder.Right);
            Assert.Equal(expectedTop, dataGridViewAdvancedBorderStylePlaceholder.Top);
            Assert.Equal(expectedBottom, dataGridViewAdvancedBorderStylePlaceholder.Bottom);
        }

        [Theory]
        [InlineData(DataGridViewAdvancedCellBorderStyle.None)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.Inset)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.InsetDouble)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.Outset)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.OutsetDouble)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.OutsetPartial)]
        public void DataGridViewCell_AdjustCellBorderStyle_InvalidAll_ReturnsExpected(DataGridViewAdvancedCellBorderStyle all)
        {
            var cell = new SubDataGridViewCell();
            var dataGridViewAdvancedBorderStyleInput = new DataGridViewAdvancedBorderStyle
            {
                All = all
            };
            var dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Same(dataGridViewAdvancedBorderStyleInput, cell.AdjustCellBorderStyle(dataGridViewAdvancedBorderStyleInput, dataGridViewAdvancedBorderStylePlaceholder, true, true, true, true));
        }

        [Fact]
        public void DataGridViewCell_AdjustCellBorderStyle_NullDataGridViewAdvancedBorderStyleInput_ThrowsArgumentNullException()
        {
            var cell = new SubDataGridViewCell();
            var dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Throws<ArgumentNullException>("dataGridViewAdvancedBorderStyleInput", () => cell.AdjustCellBorderStyle(null, dataGridViewAdvancedBorderStylePlaceholder, true, true, true, true));
        }

        [Fact]
        public void DataGridViewCell_AdjustCellBorderStyle_AllNotSetWithoutDataGridView_ReturnsExpected()
        {
            var cell = new SubDataGridViewCell();
            var dataGridViewAdvancedBorderStyleInput = new DataGridViewAdvancedBorderStyle
            {
                Left = DataGridViewAdvancedCellBorderStyle.Single,
                Right = DataGridViewAdvancedCellBorderStyle.None
            };
            var dataGridViewAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Same(dataGridViewAdvancedBorderStyleInput, cell.AdjustCellBorderStyle(dataGridViewAdvancedBorderStyleInput, dataGridViewAdvancedBorderStylePlaceholder, true, true, true, true));
        }

        public static IEnumerable<object[]> AdjustCellBorderStyle_NullDataGridViewAdvancedBorderStylePlaceholder_TestData()
        {
            yield return new object[] { new DataGridViewAdvancedBorderStyle { All = DataGridViewAdvancedCellBorderStyle.None } };
            yield return new object[] { new DataGridViewAdvancedBorderStyle { All = DataGridViewAdvancedCellBorderStyle.Inset } };
            yield return new object[] { new DataGridViewAdvancedBorderStyle { All = DataGridViewAdvancedCellBorderStyle.InsetDouble } };
            yield return new object[] { new DataGridViewAdvancedBorderStyle { All = DataGridViewAdvancedCellBorderStyle.Outset } };
            yield return new object[] { new DataGridViewAdvancedBorderStyle { All = DataGridViewAdvancedCellBorderStyle.OutsetDouble } };
            yield return new object[] { new DataGridViewAdvancedBorderStyle { All = DataGridViewAdvancedCellBorderStyle.OutsetPartial } };
            yield return new object[] { new DataGridViewAdvancedBorderStyle { Left = DataGridViewAdvancedCellBorderStyle.Inset, Right = DataGridViewAdvancedCellBorderStyle.Outset } };
        }

        [Theory]
        [MemberData(nameof(AdjustCellBorderStyle_NullDataGridViewAdvancedBorderStylePlaceholder_TestData))]
        public void DataGridViewCell_AdjustCellBorderStyle_NullDataGridViewAdvancedBorderStylePlaceholder_ThrowsArgumentNullException(DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStyleInput)
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("dataGridViewAdvancedBorderStylePlaceholder", () => cell.AdjustCellBorderStyle(dataGridViewAdvancedBorderStyleInput, null, true, true, true, true));
        }

        public static IEnumerable<object[]> BorderWidths_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), new DataGridViewAdvancedBorderStyle(), new Rectangle(0, 0, 0, 0) };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle(),
                new Rectangle(0, 0, 0, 0)
            };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle
                {
                    All = DataGridViewAdvancedCellBorderStyle.OutsetDouble
                },
                new Rectangle(2, 2, 2, 2)
            };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle
                {
                    All = DataGridViewAdvancedCellBorderStyle.InsetDouble
                },
                new Rectangle(2, 2, 2, 2)
            };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle
                {
                    All = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(1, 1, 1, 1)
            };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle
                {
                    Left = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(1, 0, 0, 0)
            };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle
                {
                    Right = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(0, 0, 1, 0)
            };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle
                {
                    Top = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(0, 1, 0, 0)
            };
            yield return new object[]
            {
                new SubDataGridViewCell(),
                new DataGridViewAdvancedBorderStyle
                {
                    Bottom = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(0, 0, 0, 1)
            };

            var row = new DataGridViewRow { DividerHeight = 10 };
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[]
            {
                cell,
                new DataGridViewAdvancedBorderStyle(),
                new Rectangle(0, 0, 0, 10)
            };
        }

        [Theory]
        [MemberData(nameof(BorderWidths_TestData))]
        public void DataGridViewCell_BorderWidths_Invoke_ReturnsExpected(SubDataGridViewCell cell, DataGridViewAdvancedBorderStyle advancedBorderStyle, Rectangle expected)
        {
            Assert.Equal(expected, cell.BorderWidths(advancedBorderStyle));
        }

        [Fact]
        public void DataGridViewCell_BorderWidths_NullAdvancedBorderStyleInput_ThrowsArgumentNullException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("advancedBorderStyle", () => cell.BorderWidths(null));
        }

        [Fact]
        public void DataGridViewCell_ClickUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.ClickUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_Clone_NonEmpty_Success()
        {
            var source = new SubDataGridViewCell
            {
                ContextMenuStrip = new ContextMenuStrip(),
                ErrorText = "errorText",
                Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft },
                Tag = "tag",
                ToolTipText = "toolTipText",
                Value = "value",
                ValueType = typeof(int)
            };

            SubDataGridViewCell cell = Assert.IsType<SubDataGridViewCell>(source.Clone());
            Assert.Equal(DataGridViewElementStates.None, cell.State);
            Assert.Null(cell.DataGridView);
            Assert.Equal(-1, cell.ColumnIndex);
            Assert.Null(cell.OwningColumn);
            Assert.Equal(-1, cell.RowIndex);
            Assert.Null(cell.OwningRow);

            Assert.NotNull(cell.ContextMenuStrip);
            Assert.NotSame(source.ContextMenuStrip, cell.ContextMenuStrip);
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
            Assert.Null(cell.DefaultNewRowValue);
            Assert.Null(cell.EditedFormattedValue);
            Assert.Equal(typeof(DataGridViewTextBoxEditingControl), cell.EditType);
            Assert.Equal("errorText", cell.ErrorText);
            Assert.Null(cell.FormattedValue);
            Assert.Equal(typeof(int), cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.True(cell.HasStyle);
            Assert.False(cell.IsInEditMode);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.False(cell.ReadOnly);
            Assert.False(cell.Resizable);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.False(cell.Selected);
            Assert.Equal(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft }, cell.Style);
            Assert.NotSame(source.Style, cell.Style);
            Assert.Equal("tag", cell.Tag);
            Assert.Equal("toolTipText", cell.ToolTipText);
            Assert.Null(cell.Value);
            Assert.Equal(typeof(int), cell.ValueType);
            Assert.False(cell.Visible);
        }

        [Fact]
        public void DataGridViewCell_Clone_Empty_Success()
        {
            var source = new SubDataGridViewCell();
            SubDataGridViewCell cell = Assert.IsType<SubDataGridViewCell>(source.Clone());
            Assert.Equal(DataGridViewElementStates.None, cell.State);
            Assert.Null(cell.DataGridView);
            Assert.Equal(-1, cell.ColumnIndex);
            Assert.Null(cell.OwningColumn);
            Assert.Equal(-1, cell.RowIndex);
            Assert.Null(cell.OwningRow);

            Assert.Null(cell.ContextMenuStrip);
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
            Assert.Null(cell.DefaultNewRowValue);
            Assert.Null(cell.EditedFormattedValue);
            Assert.Equal(typeof(DataGridViewTextBoxEditingControl), cell.EditType);
            Assert.Empty(cell.ErrorText);
            Assert.Null(cell.FormattedValue);
            Assert.Null(cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.False(cell.HasStyle);
            Assert.False(cell.IsInEditMode);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.False(cell.ReadOnly);
            Assert.False(cell.Resizable);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.False(cell.Selected);
            Assert.Equal(new DataGridViewCellStyle(), cell.Style);
            Assert.Null(cell.Tag);
            Assert.Empty(cell.ToolTipText);
            Assert.Null(cell.Value);
            Assert.Null(cell.ValueType);
            Assert.False(cell.Visible);
        }

        [Fact]
        public void DataGridViewCell_ContentClickUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.ContentClickUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_ContentDoubleClickUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.ContentDoubleClickUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_DetachEditingControl_SetNoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.DetachEditingControl());
        }

        [Fact]
        public void DataGridViewCell_Dispose_WithoutContextMenuStrip_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.Dispose();
            Assert.Null(cell.ContextMenuStrip);

            // Call multiple times.
            cell.Dispose();
            Assert.Null(cell.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewCell_Dispose_WithContextMenuStrip_Success()
        {
            var cell = new SubDataGridViewCell
            {
                ContextMenuStrip = new ContextMenuStrip()
            };
            cell.Dispose();
            Assert.NotNull(cell.ContextMenuStrip);

            // Call multiple times.
            cell.Dispose();
            Assert.NotNull(cell.ContextMenuStrip);
        }

        [Fact]
        public void DataGridViewCell_DoubleClickUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.DoubleClickUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_EnterUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.EnterUnsharesRow(-1, true));
        }

        public static IEnumerable<object[]> GetClipboardContent_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), rowIndex, true, true, true, true, "format", null };
                yield return new object[] { new SubDataGridViewCell(), rowIndex, true, true, true, true, null, null };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, rowIndex, true, true, true, true, "format", null };
            }
        }

        [Theory]
        [MemberData(nameof(GetClipboardContent_TestData))]
        public void DataGridViewCell_GetClipboardContent_Invoke_ReturnsExpected(SubDataGridViewCell cell, int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format, object expected)
        {
            Assert.Equal(expected, cell.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format));
        }

        public static IEnumerable<object[]> GetContentBounds_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), rowIndex };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, rowIndex };
            }
        }

        [Theory]
        [MemberData(nameof(GetContentBounds_TestData))]
        public void DataGridViewCell_GetContentBounds_InvokePublic_ReturnsEmpty(DataGridViewCell cell, int rowIndex)
        {
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(rowIndex));
        }

        [Fact]
        public void DataGridViewCell_GetContentBounds_InvokeProtected_ReturnsEmpty()
        {
            var cell = new SubDataGridViewCell();
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(null, null, -1));
        }

        public static IEnumerable<object[]> GetEditedFormattedValue_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), rowIndex, DataGridViewDataErrorContexts.Formatting, null };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, rowIndex, DataGridViewDataErrorContexts.Formatting, null };
            }
        }

        [Theory]
        [MemberData(nameof(GetEditedFormattedValue_TestData))]
        public void DataGridViewCell_GetEditedFormattedValue_Invoke_ReturnsExpected(DataGridViewCell cell, int rowIndex, DataGridViewDataErrorContexts context, object expected)
        {
            Assert.Equal(expected, cell.GetEditedFormattedValue(rowIndex, context));
        }

        [Fact]
        public void DataGridViewCell_GetErrorIconBounds_InvokeProtected_ReturnsEmpty()
        {
            var cell = new SubDataGridViewCell();
            Assert.Equal(Rectangle.Empty, cell.GetErrorIconBounds(null, null, -1));
        }

        public static IEnumerable<object[]> GetErrorText_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), rowIndex, string.Empty };
                yield return new object[] { new SubDataGridViewCell { ErrorText = null }, rowIndex, string.Empty };
                yield return new object[] { new SubDataGridViewCell { ErrorText = "errorText" }, rowIndex, "errorText" };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, rowIndex, string.Empty };
            }
        }

        [Theory]
        [MemberData(nameof(GetErrorText_TestData))]
        public void DataGridViewCell_GetErrorText_Invoke_ReturnsExpected(DataGridViewCell cell, int rowIndex, string expected)
        {
            Assert.Equal(expected, cell.GetErrorText(rowIndex));
        }

        public static IEnumerable<object[]> GetFormattedValue_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), "value", rowIndex, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter(), DataGridViewDataErrorContexts.Formatting, null };
                yield return new object[] { new SubDataGridViewCell(), null, rowIndex, null, null, null, (DataGridViewDataErrorContexts)(DataGridViewDataErrorContexts.Formatting - 1), null };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, "value", rowIndex, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter(), DataGridViewDataErrorContexts.Formatting, null };
            }
        }

        [Theory]
        [MemberData(nameof(GetFormattedValue_TestData))]
        public void DataGridViewCell_GetFormattedValue_Invoke_ReturnsExpected(SubDataGridViewCell cell, object value, int rowIndex, DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context, object expected)
        {
            Assert.Equal(expected, cell.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context));
        }

        public static IEnumerable<object[]> GetInheritedContextMenuStrip_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                var cellMenu1 = new ContextMenuStrip();
                yield return new object[] { new SubDataGridViewCell(), rowIndex, null };
                yield return new object[] { new SubDataGridViewCell { ContextMenuStrip = cellMenu1 }, rowIndex, cellMenu1 };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, rowIndex, null };

                var rowMenu1 = new ContextMenuStrip();
                var rowWithMenu = new DataGridViewRow { ContextMenuStrip = rowMenu1 };
                var cellMenu2 = new ContextMenuStrip();
                var cellWithMenu = new SubDataGridViewCell { ContextMenuStrip = cellMenu2 };
                var cellWithoutMenu = new SubDataGridViewCell();
                rowWithMenu.Cells.Add(cellWithMenu);
                rowWithMenu.Cells.Add(cellWithoutMenu);
                yield return new object[] { cellWithMenu, rowIndex, cellMenu2 };
                yield return new object[] { cellWithoutMenu, rowIndex, rowMenu1 };
            }
        }

        [Theory]
        [MemberData(nameof(GetInheritedContextMenuStrip_TestData))]
        public void DataGridViewCell_GetInheritedContextMenuStrip_Invoke_ReturnsExpected(DataGridViewCell cell, int rowIndex, ContextMenuStrip expected)
        {
            Assert.Equal(expected, cell.GetInheritedContextMenuStrip(rowIndex));
        }

        public static IEnumerable<object[]> GetInheritedState_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), -1, DataGridViewElementStates.ResizableSet };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { cell, -1, DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible };

            var customRow = new DataGridViewRow { Frozen = true, ReadOnly = true, Visible = false };
            var customCell = new SubDataGridViewCell();
            customRow.Cells.Add(customCell);
            yield return new object[] { customCell, -1, DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet };

            var resizableRow = new DataGridViewRow { Resizable = DataGridViewTriState.True };
            var resizableCell = new SubDataGridViewCell();
            resizableRow.Cells.Add(resizableCell);
            yield return new object[] { resizableCell, -1, DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible };
        }

        [Theory]
        [MemberData(nameof(GetInheritedState_TestData))]
        public void DataGridViewCell_GetInheritedState_Invoke_ReturnsExpected(DataGridViewCell cell, int rowIndex, DataGridViewElementStates expected)
        {
            Assert.Equal(expected, cell.GetInheritedState(rowIndex));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewCell_GetInheritedState_InvalidRowIndexNoDataGridView_ThrowsArgumentException(int rowIndex)
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentException>(null, () => cell.GetInheritedState(rowIndex));
        }

        [Fact]
        public void DataGridViewCell_GetInheritedState_ColorNoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.GetInheritedStyle(new DataGridViewCellStyle(), -1, true));
        }

        [Fact]
        public void DataGridViewCell_GetInheritedStyle_NoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.GetInheritedStyle(new DataGridViewCellStyle(), -1, true));
        }

        [StaFact]
        public void DataGridViewCell_GetNeighboringToolsRectangles_ReturnsCorrectRectangles()
        {
            DataGridView dataGridView = new DataGridView();
            dataGridView.Size = new Size(600, 200);
            dataGridView.CreateControl();

            DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column4 = new DataGridViewTextBoxColumn();

            dataGridView.Columns.Add(column1);
            dataGridView.Columns.Add(column2);
            dataGridView.Columns.Add(column3);
            dataGridView.Columns.Add(column4);

            dataGridView.Rows.Add();
            dataGridView.Rows.Add();
            dataGridView.Rows.Add();
            dataGridView.Rows.Add();

            dataGridView.Rows[0].Cells[1].Value = "Text";
            dataGridView.Rows[1].Cells[2].Value = "Text";
            dataGridView.Rows[1].Cells[3].Value = "Text";
            dataGridView.Rows[2].Cells[0].Value = "Text";
            dataGridView.Rows[2].Cells[1].Value = "Text";
            dataGridView.Rows[2].Cells[3].Value = "Text";
            dataGridView.Rows[3].Cells[1].Value = "Text";
            dataGridView.Rows[3].Cells[2].Value = "Text";

            IList<Rectangle> neighbors00 = ((IKeyboardToolTip)dataGridView.Rows[0].Cells[0]).GetNeighboringToolsRectangles();
            Assert.True(neighbors00.Contains(dataGridView.Rows[0].Cells[1].AccessibilityObject.Bounds));
            Assert.False(neighbors00.Contains(dataGridView.Rows[1].Cells[1].AccessibilityObject.Bounds));
            Assert.False(neighbors00.Contains(dataGridView.Rows[1].Cells[0].AccessibilityObject.Bounds));

            IList<Rectangle> neighbors21 = ((IKeyboardToolTip)dataGridView.Rows[2].Cells[1]).GetNeighboringToolsRectangles();
            Assert.True(neighbors21.Contains(dataGridView.Rows[1].Cells[2].AccessibilityObject.Bounds));
            Assert.True(neighbors21.Contains(dataGridView.Rows[2].Cells[0].AccessibilityObject.Bounds));
            Assert.True(neighbors21.Contains(dataGridView.Rows[2].Cells[1].AccessibilityObject.Bounds));
            Assert.True(neighbors21.Contains(dataGridView.Rows[3].Cells[1].AccessibilityObject.Bounds));
            Assert.False(neighbors21.Contains(dataGridView.Rows[1].Cells[1].AccessibilityObject.Bounds));

            IList<Rectangle> neighbors33 = ((IKeyboardToolTip)dataGridView.Rows[3].Cells[3]).GetNeighboringToolsRectangles();
            Assert.True(neighbors33.Contains(dataGridView.Rows[2].Cells[3].AccessibilityObject.Bounds));
            Assert.True(neighbors33.Contains(dataGridView.Rows[3].Cells[2].AccessibilityObject.Bounds));
            Assert.False(neighbors33.Contains(dataGridView.Rows[2].Cells[2].AccessibilityObject.Bounds));
        }

        [Fact]
        public void DataGridViewCell_GetPreferredSize_Invoke_ReturnsExpected()
        {
            var cell = new SubDataGridViewCell();
            Assert.Equal(new Size(-1, -1), cell.GetPreferredSize(null, null, -1, Size.Empty));
        }

        public static IEnumerable<object[]> GetSize_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), rowIndex, new Size(-1, -1) };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, rowIndex, new Size(-1, -1) };
            }
        }

        [Theory]
        [MemberData(nameof(GetSize_TestData))]
        public void DataGridViewCell_GetSize_Invoke_ReturnsExpected(SubDataGridViewCell cell, int rowIndex, Size expected)
        {
            Assert.Equal(expected, cell.GetSize(rowIndex));
        }

        public static IEnumerable<object[]> GetValue_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), rowIndex, null };

                var row = new DataGridViewRow();
                var cell = new SubDataGridViewCell();
                row.Cells.Add(cell);
                yield return new object[] { cell, rowIndex, null };
            }
        }

        [Theory]
        [MemberData(nameof(GetValue_TestData))]
        public void DataGridViewCell_GetValue_Invoke_ReturnsExpected(SubDataGridViewCell cell, int rowIndex, object expected)
        {
            Assert.Equal(expected, cell.GetValue(rowIndex));
        }

        [Fact]
        public void DataGridViewCell_InitializeEditingControl_SetNoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.InitializeEditingControl(-1, null, null));
        }

        [Fact]
        public void DataGridViewCell_KeyDownUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyDownUnsharesRow(null, -1));
        }

        [Fact]
        public void DataGridViewCell_KeyEntersEditMode_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyEntersEditMode(null));
        }

        [Fact]
        public void DataGridViewCell_KeyPressUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyPressUnsharesRow(null, -1));
        }

        [Fact]
        public void DataGridViewCell_KeyUpUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyUpUnsharesRow(null, -1));
        }

        [Fact]
        public void DataGridViewCell_LeaveUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.LeaveUnsharesRow(-1, true));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextHeight_NullOrEmptyText_ReturnsExpected(string text)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                int height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 10, TextFormatFlags.Default);
                Assert.Equal(0, height);

                height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 10, TextFormatFlags.Default, out bool widthTruncated);
                Assert.Equal(0, height);
                Assert.False(widthTruncated);
            }
        }

        [Theory]
        [InlineData("a", false)]
        [InlineData("truncate_me", true)]
        public void DataGridViewCell_MeasureTextHeight_NonEmptyText_ReturnsExpected(string text, bool expectedWidthTruncated)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                int height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 20, TextFormatFlags.Default);
                Assert.NotEqual(0, height);

                height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 20, TextFormatFlags.Default, out bool widthTruncated);
                Assert.NotEqual(0, height);
                Assert.Equal(expectedWidthTruncated, widthTruncated);
            }
        }

        [Fact]
        public void DataGridViewCell_MeasureTextHeight_NullGraphics_ThrowsArgumentNullException()
        {
            bool widthTruncated = true;
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextHeight(null, "text", SystemFonts.DefaultFont, 10, TextFormatFlags.Default));
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextHeight(null, "text", SystemFonts.DefaultFont, 10, TextFormatFlags.Default, out widthTruncated));
            Assert.True(widthTruncated);
        }

        [Fact]
        public void DataGridViewCell_MeasureTextHeight_NullFont_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                bool widthTruncated = true;
                Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextHeight(graphics, "text", null, 10, TextFormatFlags.Default));
                Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextHeight(graphics, "text", null, 10, TextFormatFlags.Default, out widthTruncated));
                Assert.True(widthTruncated);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void DataGridViewCell_MeasureTextHeight_InvalidMaxWidth_ThrowsArgumentOutOfRangeException(int maxWidth)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                bool widthTruncated = true;
                Assert.Throws<ArgumentOutOfRangeException>("maxWidth", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, maxWidth, TextFormatFlags.Default));
                Assert.Throws<ArgumentOutOfRangeException>("maxWidth", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, maxWidth, TextFormatFlags.Default, out widthTruncated));
                Assert.True(widthTruncated);
            }
        }

        [Theory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextHeight_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                bool widthTruncated = true;
                Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, 10, flags));
                Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, 10, flags, out widthTruncated));
                Assert.True(widthTruncated);
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextPreferredSize_NullOrEmptyText_ReturnsExpected(string text)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Equal(Size.Empty, DataGridViewCell.MeasureTextPreferredSize(graphics, text, SystemFonts.DefaultFont, 0.2f, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData("a", TextFormatFlags.Default)]
        [InlineData("truncate_me", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        public void DataGridViewCell_MeasureTextPreferredSize_NonEmptyText_ReturnsExpected(string text, TextFormatFlags flags)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.NotEqual(Size.Empty, DataGridViewCell.MeasureTextPreferredSize(graphics, text, SystemFonts.DefaultFont, 0.2f, flags));
            }
        }

        [Fact]
        public void DataGridViewCell_MeasureTextPreferredSize_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextPreferredSize(null, "text", SystemFonts.DefaultFont, 0.2f, TextFormatFlags.Default));
        }

        [Fact]
        public void DataGridViewCell_MeasureTextPreferredSize_NullFont_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextPreferredSize(graphics, "text", null, 0.2f, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void DataGridViewCell_MeasureTextPreferredSize_InvalidMaxHeight_ThrowsArgumentOutOfRangeException(float maxRatio)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentOutOfRangeException>("maxRatio", () => DataGridViewCell.MeasureTextPreferredSize(graphics, "text", SystemFonts.DefaultFont, maxRatio, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextPreferredSize_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextPreferredSize(graphics, "text", SystemFonts.DefaultFont, 0.2f, flags));
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextSize_NullOrEmptyText_ReturnsExpected(string text)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Equal(Size.Empty, DataGridViewCell.MeasureTextSize(graphics, text, SystemFonts.DefaultFont, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData("a", TextFormatFlags.Default)]
        [InlineData("truncate_me", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        public void DataGridViewCell_MeasureTextSize_NonEmptyText_ReturnsExpected(string text, TextFormatFlags flags)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.NotEqual(Size.Empty, DataGridViewCell.MeasureTextSize(graphics, text, SystemFonts.DefaultFont, flags));
            }
        }

        [Fact]
        public void DataGridViewCell_MeasureTextSize_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextSize(null, "text", SystemFonts.DefaultFont, TextFormatFlags.Default));
        }

        [Fact]
        public void DataGridViewCell_MeasureTextSize_NullFont_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextSize(graphics, "text", null, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextSize_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextSize(graphics, "text", SystemFonts.DefaultFont, flags));
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextWidth_NullOrEmptyText_ReturnsExpected(string text)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Equal(0, DataGridViewCell.MeasureTextWidth(graphics, text, SystemFonts.DefaultFont, 10, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData("a", TextFormatFlags.Default)]
        [InlineData("truncate_me", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        public void DataGridViewCell_MeasureTextWidth_NonEmptyText_ReturnsExpected(string text, TextFormatFlags flags)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.NotEqual(0, DataGridViewCell.MeasureTextWidth(graphics, text, SystemFonts.DefaultFont, 10, flags));
            }
        }

        [Fact]
        public void DataGridViewCell_MeasureTextWidth_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextWidth(null, "text", SystemFonts.DefaultFont, 10, TextFormatFlags.Default));
        }

        [Fact]
        public void DataGridViewCell_MeasureTextWidth_NullFont_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextWidth(graphics, "text", null, 10, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void DataGridViewCell_MeasureTextWidth_InvalidMaxHeight_ThrowsArgumentOutOfRangeException(int maxHeight)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<ArgumentOutOfRangeException>("maxHeight", () => DataGridViewCell.MeasureTextWidth(graphics, "text", SystemFonts.DefaultFont, maxHeight, TextFormatFlags.Default));
            }
        }

        [Theory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextWidth_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextWidth(graphics, "text", SystemFonts.DefaultFont, 10, flags));
            }
        }

        [Fact]
        public void DataGridViewCell_MouseClickUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseClickUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_MouseDoubleClickUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseDoubleClickUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_MouseDownUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseDownUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_MouseEnterUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseEnterUnsharesRow(-1));
        }

        [Fact]
        public void DataGridViewCell_MouseLeaveUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseLeaveUnsharesRow(-1));
        }

        [Fact]
        public void DataGridViewCell_MouseMoveUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseMoveUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_MouseUpUnsharesRow_Invoke_ReturnsFalse()
        {
            var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseUpUnsharesRow(null));
        }

        [Fact]
        public void DataGridViewCell_OnClick_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnClick(null);
        }

        [Fact]
        public void DataGridViewCell_OnContentClick_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnContentClick(null);
        }

        [Fact]
        public void DataGridViewCell_OnContentDoubleClick_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnContentDoubleClick(null);
        }

        [Fact]
        public void DataGridViewCell_OnDataGridViewChanged_InvokeWithoutStyle_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnDataGridViewChanged();
            Assert.Equal(DataGridViewCellStyleScopes.Cell, cell.Style.Scope);
        }

        [Fact]
        public void DataGridViewCell_OnDataGridViewChanged_InvokeWithStyle_SetsScopeToNone()
        {
            var cell = new SubDataGridViewCell
            {
                Style = new DataGridViewCellStyle()
            };
            Assert.Equal(DataGridViewCellStyleScopes.Cell, cell.Style.Scope);
            cell.OnDataGridViewChanged();
            Assert.Equal(DataGridViewCellStyleScopes.None, cell.Style.Scope);
        }

        [Fact]
        public void DataGridViewCell_OnDoubleClick_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnDoubleClick(null);
        }

        [Fact]
        public void DataGridViewCell_OnEnter_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnEnter(-1, true);
        }

        [Fact]
        public void DataGridViewCell_OnKeyDown_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnKeyDown(null, -1);
        }

        [Fact]
        public void DataGridViewCell_OnKeyPress_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnKeyPress(null, -1);
        }

        [Fact]
        public void DataGridViewCell_OnKeyUp_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnKeyUp(null, -1);
        }

        [Fact]
        public void DataGridViewCell_OnLeave_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnLeave(-1, true);
        }

        [Fact]
        public void DataGridViewCell_OnMouseClick_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnMouseClick(null);
        }

        [Fact]
        public void DataGridViewCell_OnMouseDoubleClick_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnMouseDoubleClick(null);
        }

        [Fact]
        public void DataGridViewCell_OnMouseDown_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnMouseDown(null);
        }

        [Fact]
        public void DataGridViewCell_OnMouseEnter_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnMouseEnter(-1);
        }

        [Fact]
        public void DataGridViewCell_OnMouseLeave_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnMouseLeave(-1);
        }

        [Fact]
        public void DataGridViewCell_OnMouseMove_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnMouseMove(null);
        }

        [Fact]
        public void DataGridViewCell_OnMouseUp_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.OnMouseUp(null);
        }

        [Fact]
        public void DataGridViewCell_Paint_Invoke_Nop()
        {
            var cell = new SubDataGridViewCell();
            cell.Paint(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, null, null, null, null, null, DataGridViewPaintParts.All);
        }

        [Fact]
        public void DataGridViewCell_PaintBorder_NoDataGridView_Nop()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var cell = new SubDataGridViewCell();
                cell.PaintBorder(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle());
            }
        }

        [Fact]
        public void DataGridViewCell_PaintBorder_NullGraphics_ThrowsArgumentNullException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("graphics", () => cell.PaintBorder(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle()));
        }

        [Fact]
        public void DataGridViewCell_PaintBorder_NullCellStyle_ThrowsArgumentNullException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var cell = new SubDataGridViewCell();
                Assert.Throws<ArgumentNullException>("cellStyle", () => cell.PaintBorder(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), null, new DataGridViewAdvancedBorderStyle()));
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_PaintErrorIcon_InvokeNullOrEmptyText_Success(string errorText)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var cell = new SubDataGridViewCell();
                cell.PaintErrorIcon(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 20, 19), errorText);
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewCell_PaintErrorIcon_NoDataGridView_Nop(string errorText)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var cell = new SubDataGridViewCell();
                cell.PaintErrorIcon(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 20, 19), errorText);
            }
        }

        [Theory]
        [InlineData(-1, 100, "errorText")]
        [InlineData(0, 100, "errorText")]
        [InlineData(19, 100, "errorText")]
        [InlineData(100, -1, "errorText")]
        [InlineData(100, 0, "errorText")]
        [InlineData(100, 18, "errorText")]
        [InlineData(3, 4, "errorText")]
        [InlineData(3, 4, "")]
        [InlineData(3, 4, null)]
        public void DataGridViewCell_PaintErrorIcon_NullGraphicsInvalidSize_ThrowsArgumentNullException(int cellValueBoundsWidth, int cellValueBoundsHeight, string errorText)
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("graphics", () => cell.PaintErrorIcon(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, cellValueBoundsWidth, cellValueBoundsHeight), errorText));
        }

        public static IEnumerable<object[]> ParseFormattedValue_TestData()
        {
            // Simple assignment.
            yield return new object[] { typeof(object), typeof(string), "value", new DataGridViewCellStyle(), null, null, "value" };
            yield return new object[] { typeof(string), typeof(string), "value", new DataGridViewCellStyle(), null, null, "value" };
            yield return new object[] { typeof(object), typeof(int), 1, new DataGridViewCellStyle(), null, null, 1 };
            yield return new object[] { typeof(int), typeof(int), 1, new DataGridViewCellStyle(), null, null, 1 };

            // Null.
            yield return new object[] { typeof(object), typeof(DBNull), DBNull.Value, new DataGridViewCellStyle(), null, null, null };
            yield return new object[] { typeof(string), typeof(string), "null", new DataGridViewCellStyle { NullValue = "null" }, null, null, null };
            yield return new object[] { typeof(object), typeof(string), "null", new DataGridViewCellStyle { NullValue = "null" }, null, null, null };
            yield return new object[] { typeof(object), typeof(string), "s", new DataGridViewCellStyle { NullValue = "null" }, null, null, "s" };
            yield return new object[] { typeof(object), typeof(string), "longer", new DataGridViewCellStyle { NullValue = "null" }, null, null, "longer" };
            yield return new object[] { typeof(object), typeof(string), "abcd", new DataGridViewCellStyle { NullValue = "null" }, null, null, "abcd" };
            yield return new object[] { typeof(object), typeof(string), "null", new DataGridViewCellStyle { NullValue = "null" }, null, null, null };
            yield return new object[] { typeof(int), typeof(int), 1, new DataGridViewCellStyle { NullValue = 1 }, null, null, DBNull.Value };
            yield return new object[] { typeof(int), typeof(int), 2, new DataGridViewCellStyle { NullValue = 1 }, null, null, 2 };
            yield return new object[] { typeof(string), typeof(string), "null", new DataGridViewCellStyle { NullValue = "null", DataSourceNullValue = "dbNull" }, null, null, "dbNull" };
            yield return new object[] { typeof(int), typeof(int), 1, new DataGridViewCellStyle { NullValue = 1, DataSourceNullValue = "dbNull" }, null, null, "dbNull" };

            // Converter.
            yield return new object[] { typeof(string), typeof(int), 123, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter(), "123" };
            yield return new object[] { typeof(string), typeof(int), 123, new DataGridViewCellStyle(), new Int32Converter(), null, "123" };
            yield return new object[] { typeof(string), typeof(int), 123, new DataGridViewCellStyle(), null, new Int32Converter(), "123" };
            yield return new object[] { typeof(string), typeof(int?), 123, new DataGridViewCellStyle(), null, null, "123" };
            yield return new object[] { typeof(int), typeof(string), "123", new DataGridViewCellStyle(), null, null, 123 };
            yield return new object[] { typeof(int), typeof(string), "123", new DataGridViewCellStyle { FormatProvider = CultureInfo.InvariantCulture }, null, null, 123 };
            yield return new object[] { typeof(int), typeof(string), "123", new DataGridViewCellStyle { FormatProvider = new NumberFormatInfo() }, null, null, 123 };

            // Converter can't convert.
            yield return new object[] { typeof(string), typeof(int), 123, new DataGridViewCellStyle(), new Int16Converter(), null, "123" };
            yield return new object[] { typeof(string), typeof(int), 123, new DataGridViewCellStyle(), null, new Int16Converter(), "123" };

            // Convert.ChangeType.
            yield return new object[] { typeof(short), typeof(int), 123, new DataGridViewCellStyle(), null, null, (short)123 };
            yield return new object[] { typeof(short), typeof(int), 123, new DataGridViewCellStyle { FormatProvider = CultureInfo.InvariantCulture }, null, null, (short)123 };
            yield return new object[] { typeof(short), typeof(int), 123, new DataGridViewCellStyle { FormatProvider = new NumberFormatInfo() }, null, null, (short)123 };
            yield return new object[] { typeof(short?), typeof(int), 123, new DataGridViewCellStyle(), null, null, (short)123 };
            yield return new object[] { typeof(short), typeof(int?), 123, new DataGridViewCellStyle(), null, null, (short)123 };

            // Nullable.
            yield return new object[] { typeof(int?), typeof(int?), 123, new DataGridViewCellStyle(), null, null, 123 };
            yield return new object[] { typeof(int?), typeof(int), 123, new DataGridViewCellStyle(), null, null, 123 };
            yield return new object[] { typeof(int), typeof(int?), 123, new DataGridViewCellStyle(), null, null, 123 };
            yield return new object[] { typeof(string), typeof(int?), 123, new DataGridViewCellStyle(), null, null, "123" };

            // CheckState.
            yield return new object[] { typeof(bool), typeof(CheckState), CheckState.Indeterminate, new DataGridViewCellStyle(), null, null, DBNull.Value };
            yield return new object[] { typeof(bool), typeof(CheckState), CheckState.Checked, new DataGridViewCellStyle(), null, null, true };
            yield return new object[] { typeof(bool), typeof(CheckState), CheckState.Unchecked, new DataGridViewCellStyle(), null, null, false };
            yield return new object[] { typeof(bool), typeof(CheckState), (CheckState)(CheckState.Unchecked - 1), new DataGridViewCellStyle(), null, null, false };
            yield return new object[] { typeof(bool), typeof(CheckState), (CheckState)(CheckState.Indeterminate + 1), new DataGridViewCellStyle(), null, null, false };
            yield return new object[] { typeof(CheckState), typeof(CheckState), CheckState.Checked, new DataGridViewCellStyle(), null, null, CheckState.Checked };
            yield return new object[] { typeof(int), typeof(CheckState), CheckState.Checked, new DataGridViewCellStyle(), null, null, 1 };
            yield return new object[] { typeof(int), typeof(CheckState), CheckState.Checked, new DataGridViewCellStyle(), new EnumConverter(typeof(CheckState)), null, 1 };
            yield return new object[] { typeof(int), typeof(CheckState), CheckState.Checked, new DataGridViewCellStyle(), null, new EnumConverter(typeof(CheckState)), 1 };
        }

        [Theory]
        [MemberData(nameof(ParseFormattedValue_TestData))]
        public void DataGridViewCell_ParseFormattedValue_Invoke_ReturnsExpected(Type valueType, Type formattedValueType, object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter, object expected)
        {
            var cell = new CustomFormattedValueType
            {
                ValueType = valueType,
                FormattedValueTypeResult = formattedValueType
            };
            Assert.Equal(expected, cell.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter));

            // Call same.
            Assert.Equal(expected, cell.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter));
        }

        [Fact]
        public void DataGridViewCell_ParseFormattedValue_NullCellStyle_ThrowsArgumentNullException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("cellStyle", () => cell.ParseFormattedValue(1, null, new Int32Converter(), new Int32Converter()));
        }

        [Fact]
        public void DataGridViewCell_ParseFormattedValue_NullValueTypeAndFormattedValueType_ThrowsFormatException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(1, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter()));
        }

        [Fact]
        public void DataGridViewCell_ParseFormattedValue_NullValueType_ThrowsFormatException()
        {
            var cell = new CustomFormattedValueType { FormattedValueTypeResult = typeof(int) };
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(1, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter()));
        }

        [Fact]
        public void DataGridViewCell_ParseFormattedValue_NullFormattedValueType_ThrowsFormatException()
        {
            var cell = new CustomFormattedValueType { ValueType = typeof(int) };
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(1, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter()));
        }

        public static IEnumerable<object[]> ParseFormattedValue_CantConvert_TestData()
        {
            yield return new object[] { typeof(DataGridViewCellStyleTests), typeof(int), 123, new DataGridViewCellStyle(), null, null };
            yield return new object[] { typeof(DataGridViewCell), typeof(DataGridViewCellTests), new DataGridViewCellTests(), new DataGridViewCellStyle(), null, null };
            yield return new object[] { typeof(int), typeof(string), "Invalid", new DataGridViewCellStyle(), null, null };
        }

        [Theory]
        [MemberData(nameof(ParseFormattedValue_CantConvert_TestData))]
        public void DataGridViewCell_ParseFormattedValue_CantConvert_ThrowsFormatException(Type valueType, Type formattedValueType, object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            var cell = new CustomFormattedValueType
            {
                ValueType = valueType,
                FormattedValueTypeResult = formattedValueType
            };
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void DataGridViewCell_ParseFormattedValue_InvalidFormattedValue_ThrowsArgumentException(object formattedValue)
        {
            var cell = new CustomFormattedValueType
            {
                ValueType = typeof(int),
                FormattedValueTypeResult = typeof(string)
            };
            Assert.Throws<ArgumentException>("formattedValue", () => cell.ParseFormattedValue(formattedValue, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter()));
        }

        private class CustomFormattedValueType : DataGridViewCell
        {
            public Type FormattedValueTypeResult { get; set; }

            public override Type FormattedValueType => FormattedValueTypeResult;
        }

        [Fact]
        public void DataGridViewCell_PositionEditingControl_NoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.PositionEditingControl(true, true, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), true, true, true, true));
        }

        [Fact]
        public void DataGridViewCell_PositionEditingPanel_NoDataGridView_ThrowsInvalidOperationException()
        {
            var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.PositionEditingPanel(new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), true, true, true, true));
        }

        public static IEnumerable<object[]> SetValue_TestData()
        {
            foreach (int rowIndex in new int[] { -2, -1, 0 })
            {
                yield return new object[] { new SubDataGridViewCell(), rowIndex, null };
                yield return new object[] { new SubDataGridViewCell(), rowIndex, "value" };
            }
        }

        [Theory]
        [MemberData(nameof(SetValue_TestData))]
        public void DataGridViewCell_SetValue_WithoutDataGridView_GetReturnsExpected(SubDataGridViewCell cell, int rowIndex, object value)
        {
            Assert.True(cell.SetValue(rowIndex, value));
            Assert.Equal(value, cell.GetValue(rowIndex));

            // Set same.
            Assert.True(cell.SetValue(rowIndex, value));
            Assert.Equal(value, cell.GetValue(rowIndex));
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { new SubDataGridViewCell(), "DataGridViewCell { ColumnIndex=-1, RowIndex=-1 }" };

            var row = new DataGridViewRow();
            var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            yield return new object[] { cell, "DataGridViewCell { ColumnIndex=-1, RowIndex=-1 }" };
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public void DataGridViewCell_ToString_Invoke_ReturnsExpected(DataGridViewCell cell, string expected)
        {
            Assert.Equal(expected, cell.ToString());
        }

        public class SubDataGridViewCell : DataGridViewCell
        {
            public new Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle)
            {
                return base.BorderWidths(advancedBorderStyle);
            }

            public new bool ClickUnsharesRow(DataGridViewCellEventArgs e) => base.ClickUnsharesRow(e);

            public new bool ContentClickUnsharesRow(DataGridViewCellEventArgs e) => base.ContentClickUnsharesRow(e);

            public new bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e) => base.ContentDoubleClickUnsharesRow(e);

            public new bool DoubleClickUnsharesRow(DataGridViewCellEventArgs e) => base.DoubleClickUnsharesRow(e);

            public new bool EnterUnsharesRow(int rowIndex, bool throughMouseClick) => base.EnterUnsharesRow(rowIndex, throughMouseClick);

            public new object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
            {
                return base.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format);
            }

            public new Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
            {
                return base.GetContentBounds(graphics, cellStyle, rowIndex);
            }

            public new Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
            {
                return base.GetErrorIconBounds(graphics, cellStyle, rowIndex);
            }

            public new object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
            {
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }

            public new Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
            {
                return base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
            }

            public new Size GetSize(int rowIndex) => base.GetSize(rowIndex);

            public new object GetValue(int rowIndex) => base.GetValue(rowIndex);

            public new bool KeyDownUnsharesRow(KeyEventArgs e, int rowIndex) => base.KeyDownUnsharesRow(e, rowIndex);

            public new bool KeyEntersEditMode(KeyEventArgs e) => base.KeyEntersEditMode(e);

            public new bool KeyPressUnsharesRow(KeyPressEventArgs e, int rowIndex) => base.KeyPressUnsharesRow(e, rowIndex);

            public new bool KeyUpUnsharesRow(KeyEventArgs e, int rowIndex) => base.KeyUpUnsharesRow(e, rowIndex);

            public new bool LeaveUnsharesRow(int rowIndex, bool throughMouseClick) => base.LeaveUnsharesRow(rowIndex, throughMouseClick);

            public new bool MouseClickUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseClickUnsharesRow(e);

            public new bool MouseDoubleClickUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseDoubleClickUnsharesRow(e);

            public new bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseDownUnsharesRow(e);

            public new bool MouseEnterUnsharesRow(int rowIndex) => base.MouseEnterUnsharesRow(rowIndex);

            public new bool MouseLeaveUnsharesRow(int rowIndex) => base.MouseLeaveUnsharesRow(rowIndex);

            public new bool MouseMoveUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseMoveUnsharesRow(e);

            public new bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseUpUnsharesRow(e);

#pragma warning disable xUnit1013

            public new void OnClick(DataGridViewCellEventArgs e) => base.OnClick(e);

            public new void OnContentClick(DataGridViewCellEventArgs e) => base.OnContentClick(e);

            public new void OnContentDoubleClick(DataGridViewCellEventArgs e) => base.OnContentDoubleClick(e);

            public new void OnDataGridViewChanged() => base.OnDataGridViewChanged();

            public new void OnDoubleClick(DataGridViewCellEventArgs e) => base.OnDoubleClick(e);

            public new void OnEnter(int rowIndex, bool throughMouseClick) => base.OnEnter(rowIndex, throughMouseClick);

            public new void OnKeyDown(KeyEventArgs e, int rowIndex) => base.OnKeyDown(e, rowIndex);

            public new void OnKeyPress(KeyPressEventArgs e, int rowIndex) => base.OnKeyPress(e, rowIndex);

            public new void OnKeyUp(KeyEventArgs e, int rowIndex) => base.OnKeyUp(e, rowIndex);

            public new void OnLeave(int rowIndex, bool throughMouseClick) => base.OnLeave(rowIndex, throughMouseClick);

            public new void OnMouseClick(DataGridViewCellMouseEventArgs e) => base.OnMouseClick(e);

            public new void OnMouseDoubleClick(DataGridViewCellMouseEventArgs e) => base.OnMouseDoubleClick(e);

            public new void OnMouseDown(DataGridViewCellMouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(int rowIndex) => base.OnMouseEnter(rowIndex);

            public new void OnMouseLeave(int rowIndex) => base.OnMouseLeave(rowIndex);

            public new void OnMouseMove(DataGridViewCellMouseEventArgs e) => base.OnMouseMove(e);

            public new void OnMouseUp(DataGridViewCellMouseEventArgs e) => base.OnMouseUp(e);

            public new void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }

            public new void PaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle bounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle)
            {
                base.PaintBorder(graphics, clipBounds, bounds, cellStyle, advancedBorderStyle);
            }

            public new void PaintErrorIcon(Graphics graphics, Rectangle clipBounds, Rectangle cellValueBounds, string errorText)
            {
                base.PaintErrorIcon(graphics, clipBounds, cellValueBounds, errorText);
            }

#pragma warning restore xUnit1013

            public new bool SetValue(int rowIndex, object value) => base.SetValue(rowIndex, value);

            [Fact]
            public void DataGridViewCell_AccessibilityObject_Get_ReturnsSameInstance()
            {
                Assert.Same(AccessibilityObject, AccessibilityObject);
                DataGridViewCellAccessibleObject accessibilityObject = Assert.IsType<DataGridViewCellAccessibleObject>(AccessibilityObject);
                Assert.Equal(this, accessibilityObject.Owner);
            }

            [Fact]
            public void DataGridViewCell_CreateAccessibilityInstance_Invoke_ReturnsExpected()
            {
                DataGridViewCellAccessibleObject accessibilityObject = Assert.IsType<DataGridViewCellAccessibleObject>(CreateAccessibilityInstance());
                Assert.Equal(this, accessibilityObject.Owner);
            }
        }
    }
}
