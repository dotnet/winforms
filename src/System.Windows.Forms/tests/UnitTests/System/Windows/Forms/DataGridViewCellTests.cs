// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms.Automation;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    public class DataGridViewCellTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewCell_Ctor_Default()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(-1, cell.ColumnIndex);
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
            Assert.Null(cell.ContextMenuStrip);
            Assert.Null(cell.DataGridView);
            Assert.Null(cell.DefaultNewRowValue);
            Assert.False(cell.Displayed);
            Assert.Null(cell.EditedFormattedValue);
            Assert.Equal(typeof(DataGridViewTextBoxEditingControl), cell.EditType);
            Assert.Empty(cell.ErrorText);
            Assert.Null(cell.FormattedValue);
            Assert.Null(cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.False(cell.HasStyle);
            Assert.Equal(DataGridViewElementStates.ResizableSet, cell.InheritedState);
            Assert.False(cell.IsInEditMode);
            Assert.Null(cell.OwningColumn);
            Assert.Null(cell.OwningRow);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.False(cell.ReadOnly);
            Assert.False(cell.Resizable);
            Assert.Equal(-1, cell.RowIndex);
            Assert.False(cell.Selected);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.Equal(DataGridViewElementStates.None, cell.State);
            Assert.NotNull(cell.Style);
            Assert.Same(cell.Style, cell.Style);
            Assert.Null(cell.Tag);
            Assert.Empty(cell.ToolTipText);
            Assert.Null(cell.Value);
            Assert.Null(cell.ValueType);
            Assert.False(cell.Visible);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContentBounds_Get_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContentBounds_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContentBounds_GetWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContentBounds_GetWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_ContentBounds_GetShared_ThrowsArgumentOutOfRangeException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.ContentBounds);
        }

        public static IEnumerable<object[]> ContextMenuStrip_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContextMenuStrip() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_GetWithRow_ReturnsExpected(ContextMenuStrip menu)
        {
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = menu
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Null(cell.ContextMenuStrip);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_GetWithColumn_ReturnsExpected(ContextMenuStrip menu)
        {
            using var column = new DataGridViewColumn
            {
                ContextMenuStrip = menu
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Null(cell.ContextMenuStrip);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_GetWithDataGridView_ReturnsExpected(ContextMenuStrip menu)
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                ContextMenuStrip = menu
            };
            DataGridViewCell cell = control.Rows[0].Cells[0];
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;
            Assert.Null(cell.ContextMenuStrip);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContextMenuStrip_GetWithDataGridViewVirtualMode_CallsCellContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewCellContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.CellContextMenuStripNeeded += handler;

            Assert.Same(menu2, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.CellContextMenuStripNeeded -= handler;
            Assert.Same(menu1, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContextMenuStrip_GetWithDataGridViewDataSource_CallsCellContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewCellContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.CellContextMenuStripNeeded += handler;

            Assert.Same(menu2, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.CellContextMenuStripNeeded -= handler;
            Assert.Same(menu1, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_Set_GetReturnsExpected(ContextMenuStrip value)
        {
            using var cell = new SubDataGridViewCell
            {
                ContextMenuStrip = value
            };
            Assert.Equal(value, cell.ContextMenuStrip);

            // Set same.
            cell.ContextMenuStrip = value;
            Assert.Equal(value, cell.ContextMenuStrip);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_SetWithRow_GetReturnsExpected(ContextMenuStrip value)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            cell.ContextMenuStrip = value;
            Assert.Equal(value, cell.ContextMenuStrip);

            // Set same.
            cell.ContextMenuStrip = value;
            Assert.Equal(value, cell.ContextMenuStrip);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_SetWithColumn_GetReturnsExpected(ContextMenuStrip value)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            cell.ContextMenuStrip = value;
            Assert.Equal(value, cell.ContextMenuStrip);

            // Set same.
            cell.ContextMenuStrip = value;
            Assert.Equal(value, cell.ContextMenuStrip);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_Set_TestData))]
        public void DataGridViewCell_ContextMenuStrip_SetWithDataGridView_GetReturnsExpected(ContextMenuStrip value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            cell.ContextMenuStrip = value;
            Assert.Equal(value, cell.ContextMenuStrip);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ContextMenuStrip = value;
            Assert.Equal(value, cell.ContextMenuStrip);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContextMenuStrip_SetWithDataGridView_CallsCellContextMenuStripChanged()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal(0, e.ColumnIndex);
            };
            control.CellContextMenuStripChanged += handler;

            // Set non-null.
            using var menu1 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set same.
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set different.
            using var menu2 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu2;
            Assert.Same(menu2, cell.ContextMenuStrip);
            Assert.Equal(2, callCount);

            // Set null.
            cell.ContextMenuStrip = null;
            Assert.Null(cell.ContextMenuStrip);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.CellContextMenuStripChanged -= handler;
            cell.ContextMenuStrip = menu1;
            Assert.Equal(menu1, cell.ContextMenuStrip);
            Assert.Equal(3, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContextMenuStrip_Dispose_SetsToNull()
        {
            using var cell = new SubDataGridViewCell();
            using var menu = new ContextMenuStrip();
            cell.ContextMenuStrip = menu;
            Assert.Same(menu, cell.ContextMenuStrip);
            Assert.False(menu.IsDisposed);

            menu.Dispose();
            Assert.Null(cell.ContextMenuStrip);
        }

        [WinFormsFact]
        public void DataGridViewCell_ContextMenuStrip_ResetThenDispose_Nop()
        {
            using var cell = new SubDataGridViewCell();
            using var menu1 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu1;
            Assert.Same(menu1, cell.ContextMenuStrip);

            using var menu2 = new ContextMenuStrip();
            cell.ContextMenuStrip = menu2;

            menu1.Dispose();
            Assert.Same(menu2, cell.ContextMenuStrip);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.Displayed, false)]
        [InlineData(DataGridViewElementStates.Displayed | DataGridViewElementStates.ReadOnly, false)]
        [InlineData(DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected, false)]
        public void DataGridViewCell_Displayed_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.Displayed);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Displayed_GetWithRow_ReturnsExpected(bool rowVisible)
        {
            using var row = new DataGridViewRow
            {
                Visible = rowVisible
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.False(cell.Displayed);
        }

        public static IEnumerable<object[]> Displayed_GetWithDataGridView_TestData()
        {
            foreach (bool gridVisible in new bool[] { true, false })
            {
                foreach (bool rowHeadersVisible in new bool[] { true, false })
                {
                    foreach (bool columnHeadersVisible in new bool[] { true, false })
                    {
                        foreach (bool rowVisible in new bool[] { true, false })
                        {
                            foreach (bool columnVisible in new bool[] { true, false })
                            {
                                yield return new object[] { gridVisible, rowHeadersVisible, columnHeadersVisible, rowVisible, columnVisible };
                            }
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetWithDataGridView_TestData))]
        public void DataGridViewCell_Displayed_GetWithDataGridView_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            row.Visible = rowVisible;
            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.Displayed);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Displayed_GetWithSharedDataGridView_TestData()
        {
            foreach (bool gridVisible in new bool[] { true, false })
            {
                foreach (bool rowHeadersVisible in new bool[] { true, false })
                {
                    foreach (bool columnHeadersVisible in new bool[] { true, false })
                    {
                        foreach (bool columnVisible in new bool[] { true, false })
                        {
                            yield return new object[] { gridVisible, rowHeadersVisible, columnHeadersVisible, columnVisible };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetWithSharedDataGridView_TestData))]
        public void DataGridViewCell_Displayed_GetWithSharedDataGridView_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows.SharedRow(0);

            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.Displayed);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetWithDataGridView_TestData))]
        public void DataGridViewCell_Displayed_GetWithDataGridViewWithHandle_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            row.Visible = rowVisible;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(gridVisible && rowVisible && columnVisible, cell.Displayed);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetWithSharedDataGridView_TestData))]
        public void DataGridViewCell_Displayed_GetWithSharedDataGridViewWithHandle_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows.SharedRow(0);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.Displayed);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_EditedFormattedValue_Get_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Null(cell.EditedFormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_EditedFormattedValue_GetWithValue_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell
            {
                Value = "Value"
            };
            Assert.Null(cell.EditedFormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_EditedFormattedValue_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Null(cell.EditedFormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_EditedFormattedValue_GetWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Null(cell.EditedFormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_EditedFormattedValue_GetWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ValueType = typeof(string);
            Assert.Empty((string)cell.EditedFormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_EditedFormattedValue_GetWithDataGridViewWithValue_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.Value = "Value";
            cell.ValueType = typeof(string);
            Assert.Equal("Value", (string)cell.EditedFormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_EditedFormattedValue_GetShared_ThrowsArgumentOutOfRangeException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.EditedFormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorIconBounds_GetWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Equal(Rectangle.Empty, cell.ErrorIconBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorIconBounds_Get_ThrowInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.ErrorIconBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorIconBounds_GetWithRow_ThrowsInvalidOperationException()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Throws<InvalidOperationException>(() => cell.ErrorIconBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorIconBounds_GetWithColumn_ThrowsInvalidOperationException()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<InvalidOperationException>(() => cell.ErrorIconBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorIconBounds_GetShared_ThrowsArgumentOutOfRangeException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.ErrorIconBounds);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Empty(cell.ErrorText);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Empty(cell.ErrorText);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Empty(cell.ErrorText);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetShared_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Empty(cell.ErrorText);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetVirtualMode_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.CellErrorTextNeeded += handler;

            Assert.Equal("errorText2", cell.ErrorText);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.ErrorText);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetSharedVirtualMode_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) => callCount++;
            control.CellErrorTextNeeded += handler;

            Assert.Equal("errorText1", cell.ErrorText);
            Assert.Equal(0, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.ErrorText);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetDataGridViewWithDataSource_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);

            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.CellErrorTextNeeded += handler;

            Assert.Equal("errorText2", cell.ErrorText);
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.ErrorText);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_GetSharedWithDataSource_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);

            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) => callCount++;
            control.CellErrorTextNeeded += handler;

            Assert.Equal("errorText1", cell.ErrorText);
            Assert.Equal(0, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.ErrorText);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_Set_GetReturnsExpected(string value, string expected)
        {
            using var cell = new SubDataGridViewCell
            {
                ErrorText = value
            };
            Assert.Equal(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SetWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var cell = new SubDataGridViewCell
            {
                ErrorText = "OldValue"
            };
            cell.ErrorText = value;

            Assert.Equal(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SetWithRow_GetReturnsExpected(string value, string expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SetWithRowWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell
            {
                ErrorText = "OldValue"
            };
            row.Cells.Add(cell);

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SetWithColumn_GetReturnsExpected(string value, string expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SetWithColumnWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell
            {
                ErrorText = "OldValue"
            };
            column.HeaderCell = cell;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SetWithDataGridView_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ErrorText = "OldValue";
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_Shared_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ErrorText_SharedWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            cell.ErrorText = "OldValue";
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "", 0)]
        [InlineData("", "", 0)]
        [InlineData("ErrorText", "ErrorText", 1)]
        public void DataGridViewCell_ErrorText_SetWithDataGridViewWithHandle_GetReturnsExpected(string value, string expected, int expectedInvalidatedCallCount)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(null, "", 1)]
        [InlineData("", "", 1)]
        [InlineData("ErrorText", "ErrorText", 1)]
        [InlineData("OldValue", "OldValue", 0)]
        public void DataGridViewCell_ErrorText_SetWithDataGridViewWithNonNullOldValueWithHandle_GetReturnsExpected(string value, string expected, int expectedInvalidCallCount)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ErrorText = "OldValue";
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            cell.ErrorText = value;
            Assert.Equal(expected, cell.ErrorText);
            Assert.Equal(0, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_ErrorText_SetWithDataGridView_CallsCellErrorTextChanged()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.ColumnIndex);
                Assert.Equal(0, e.RowIndex);
            };
            control.CellErrorTextChanged += handler;

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
            control.CellErrorTextChanged -= handler;
            cell.ErrorText = "errorText";
            Assert.Equal("errorText", cell.ErrorText);
            Assert.Equal(3, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_FormattedValue_Get_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Null(cell.FormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_FormattedValue_GetWithValue_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell
            {
                Value = "Value"
            };
            Assert.Null(cell.FormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_FormattedValue_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Null(cell.FormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_FormattedValue_GetWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Null(cell.FormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_FormattedValue_GetWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ValueType = typeof(string);
            Assert.Empty((string)cell.FormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_FormattedValue_GetWithDataGridViewWithValue_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.Value = "Value";
            cell.ValueType = typeof(string);
            Assert.Equal("Value", (string)cell.FormattedValue);
        }

        [WinFormsFact]
        public void DataGridViewCell_FormattedValue_GetShared_ThrowsArgumentOutOfRangeException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.FormattedValue);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.Frozen, false)]
        [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly, false)]
        [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.Selected, false)]
        public void DataGridViewCell_Frozen_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.Frozen);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Frozen_GetWithRow_ReturnsExpected(bool rowFrozen)
        {
            using var row = new DataGridViewRow
            {
                Frozen = rowFrozen
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(rowFrozen, cell.Frozen);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Frozen_GetWithColumn_ReturnsExpected(bool columnFrozen)
        {
            using var column = new DataGridViewColumn
            {
                Frozen = columnFrozen
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(columnFrozen, cell.Frozen);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewCell_Frozen_GetWithDataGridView_ReturnsExpected(bool rowFrozen, bool columnFrozen)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Frozen = rowFrozen;
            column.Frozen = columnFrozen;
            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(rowFrozen && columnFrozen, cell.Frozen);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Frozen_GetShared_ReturnsExpected(bool columnFrozen)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.Frozen = columnFrozen;
            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.Frozen);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedState_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.InheritedState);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.False, DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewElementStates.ResizableSet)]
        public void DataGridViewCell_InheritedState_GetWithRowCustomState_ReturnsExpected(DataGridViewTriState resizable, DataGridViewElementStates expected)
        {
            using var row = new DataGridViewRow
            {
                Frozen = true,
                ReadOnly = true,
                Visible = false,
                Resizable = resizable
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | expected, cell.InheritedState);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedState_GetWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.InheritedState);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.False, DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewElementStates.ResizableSet)]
        public void DataGridViewCell_InheritedState_GetWithColumnCustomState_ReturnsExpected(DataGridViewTriState resizable, DataGridViewElementStates expected)
        {
            using var column = new DataGridViewColumn
            {
                Frozen = true,
                ReadOnly = true,
                Visible = false,
                Resizable = resizable
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | expected, cell.InheritedState);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedState_GetWithDataGrid_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Equal(DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.InheritedState);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedState_GetWithDataGridCustomStateRow_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.ReadOnly = true;
            row.Frozen = true;
            row.Resizable = DataGridViewTriState.True;
            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.InheritedState);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedState_GetSharedRow_ThrowsArgumentOutOfRangeException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.InheritedState);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedStyle_Get_ThrowInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.InheritedStyle);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedStyle_GetWithRow_ThrowsInvalidOperationException()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Throws<InvalidOperationException>(() => cell.InheritedStyle);
        }

        [WinFormsFact]
        public void DataGridViewCell_InheritedStyle_GetWithColumn_ThrowsInvalidOperationException()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<InvalidOperationException>(() => cell.InheritedStyle);
        }

        [WinFormsFact]
        public void DataGridViewCell_IsInEditMode_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.False(cell.IsInEditMode);
        }

        [WinFormsFact]
        public void DataGridViewCell_IsInEditMode_GetWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.False(cell.IsInEditMode);
        }

        [WinFormsFact]
        public void DataGridViewCell_IsInEditMode_GetWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.False(cell.IsInEditMode);
        }

        [WinFormsFact]
        public void DataGridViewCell_IsInEditMode_GetSharedRow_ThrowsInvalidOperationExceptio()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.IsInEditMode);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.ReadOnly, true)]
        [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Frozen, true)]
        [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected, true)]
        public void DataGridViewCell_ReadOnly_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_ReadOnly_GetWithRow_ReturnsExpected(bool rowReadOnly)
        {
            using var row = new DataGridViewRow
            {
                ReadOnly = rowReadOnly
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(rowReadOnly, cell.ReadOnly);
        }

        [WinFormsTheory]
        [InlineData(false, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, false, false)]
        public void DataGridViewCell_ReadOnly_GetWithDataGridView_ReturnsExpected(bool readOnly, bool rowReadOnly, bool columnReadOnly)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ReadOnly = readOnly
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.ReadOnly = rowReadOnly;
            column.ReadOnly = columnReadOnly;
            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(readOnly || rowReadOnly || columnReadOnly, cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewCell_ReadOnly_GetShared_ReturnsExpected(bool readOnly, bool columnReadOnly)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ReadOnly = readOnly
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.ReadOnly = columnReadOnly;
            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.ReadOnly);
        }

        [WinFormsFact]
        public void DataGridViewCell_ReadOnly_SetWithoutOwningRow_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = true);
            Assert.False(cell.ReadOnly);

            cell.ReadOnly = false;
            Assert.False(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SubDataGridViewCell_ReadOnly_SetWithRow_GetReturnsExpected(bool value)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            cell.ReadOnly = value;
            Assert.Equal(value, cell.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.Equal(value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set same.
            cell.ReadOnly = value;
            Assert.Equal(value, cell.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.Equal(value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set different.
            cell.ReadOnly = !value;
            Assert.True(cell.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.True((cell.State & DataGridViewElementStates.ReadOnly) != 0);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SubDataGridViewCell_ReadOnly_SetWithRowReadOnlySet_GetReturnsExpected(bool rowReadOnly, bool value)
        {
            using var row = new DataGridViewRow
            {
                ReadOnly = rowReadOnly
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            cell.ReadOnly = value;
            Assert.Equal(value, cell.ReadOnly);
            Assert.Equal(rowReadOnly && value, row.ReadOnly);
            Assert.Equal(!rowReadOnly && value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set same.
            cell.ReadOnly = value;
            Assert.Equal(value, cell.ReadOnly);
            Assert.Equal(rowReadOnly && value, row.ReadOnly);
            Assert.Equal(!rowReadOnly && value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set different.
            cell.ReadOnly = !value;
            Assert.Equal(!rowReadOnly || !value, cell.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.Equal(!rowReadOnly || !value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SubDataGridViewCell_ReadOnly_SetWithRowReadOnlySetWithCells_GetReturnsExpected(bool rowReadOnly, bool value)
        {
            using var row = new DataGridViewRow
            {
                ReadOnly = rowReadOnly
            };
            using var cell1 = new SubDataGridViewCell();
            using var cell2 = new SubDataGridViewCell();
            row.Cells.Add(cell1);
            row.Cells.Add(cell2);

            cell1.ReadOnly = value;
            Assert.Equal(value, cell1.ReadOnly);
            Assert.Equal(rowReadOnly, cell2.ReadOnly);
            Assert.Equal(rowReadOnly && value, row.ReadOnly);
            Assert.Equal(!rowReadOnly && value, (cell1.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set same.
            cell1.ReadOnly = value;
            Assert.Equal(value, cell1.ReadOnly);
            Assert.Equal(rowReadOnly, cell2.ReadOnly);
            Assert.Equal(rowReadOnly && value, row.ReadOnly);
            Assert.Equal(!rowReadOnly && value, (cell1.State & DataGridViewElementStates.ReadOnly) != 0);

            // Set different.
            cell1.ReadOnly = !value;
            Assert.Equal(!rowReadOnly || !value, cell1.ReadOnly);
            Assert.Equal(rowReadOnly, cell2.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.Equal(!rowReadOnly || !value, (cell1.State & DataGridViewElementStates.ReadOnly) != 0);
        }

        [WinFormsFact]
        public void DataGridViewCell_ReadOnly_SetWithColumn_ThrowsInvalidOperationException()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = true);
            Assert.True(cell.ReadOnly);

            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = false);
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SubDataGridViewCell_ReadOnly_SetWithDataGridView_GetReturnsExpected(bool value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            DataGridViewCell cell = row.Cells[0];

            cell.ReadOnly = value;
            Assert.Equal(value, cell.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.Equal(value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ReadOnly = value;
            Assert.Equal(value, cell.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.Equal(value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);

            // Set different.
            cell.ReadOnly = !value;
            Assert.Equal(!value, cell.ReadOnly);
            Assert.False(row.ReadOnly);
            Assert.Equal(!value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void SubDataGridViewCell_ReadOnly_SetWithDataGridViewCellReadOnlySet_GetReturnsExpected(bool readOnly, bool rowReadOnly, bool value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ReadOnly = readOnly
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.ReadOnly = rowReadOnly;
            DataGridViewCell cell = row.Cells[0];

            cell.ReadOnly = value;
            Assert.Equal(readOnly || value, cell.ReadOnly);
            Assert.Equal(readOnly || (rowReadOnly && value), row.ReadOnly);
            Assert.Equal(!readOnly && !rowReadOnly && value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ReadOnly = value;
            Assert.Equal(readOnly || value, cell.ReadOnly);
            Assert.Equal(readOnly || (rowReadOnly && value), row.ReadOnly);
            Assert.Equal(!readOnly && !rowReadOnly && value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);

            // Set different.
            cell.ReadOnly = !value;
            Assert.Equal(readOnly || !value, cell.ReadOnly);
            Assert.Equal(readOnly, row.ReadOnly);
            Assert.Equal(!readOnly && !value, (cell.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(false, false, false)]
        public void SubDataGridViewCell_ReadOnly_SetWithDataGridViewCellReadOnlySetWithCells_GetReturnsExpected(bool readOnly, bool rowReadOnly, bool value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column1 = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var column2 = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ReadOnly = readOnly
            };
            control.Columns.Add(column1);
            control.Columns.Add(column2);
            DataGridViewRow row = control.Rows[0];
            row.ReadOnly = rowReadOnly;
            DataGridViewCell cell1 = row.Cells[0];
            DataGridViewCell cell2 = row.Cells[1];

            cell1.ReadOnly = value;
            Assert.Equal(readOnly || value, cell1.ReadOnly);
            Assert.Equal(readOnly || rowReadOnly, cell2.ReadOnly);
            Assert.Equal(readOnly || (rowReadOnly && value), row.ReadOnly);
            Assert.Equal(!readOnly && !rowReadOnly && value, (cell1.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell1.ReadOnly = value;
            Assert.Equal(readOnly || value, cell1.ReadOnly);
            Assert.Equal(readOnly || rowReadOnly, cell2.ReadOnly);
            Assert.Equal(readOnly || (rowReadOnly && value), row.ReadOnly);
            Assert.Equal(!readOnly && !rowReadOnly && value, (cell1.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);

            // Set different.
            cell1.ReadOnly = !value;
            Assert.Equal(readOnly || !value, cell1.ReadOnly);
            Assert.Equal(readOnly || rowReadOnly, cell2.ReadOnly);
            Assert.Equal(readOnly, row.ReadOnly);
            Assert.Equal(!readOnly && !value, (cell1.State & DataGridViewElementStates.ReadOnly) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_ReadOnly_SetWithDataGridView_CallsCellStateChanged()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(cell, e.Cell);
                Assert.Equal(DataGridViewElementStates.ReadOnly, e.StateChanged);
            };
            control.CellStateChanged += handler;

            // Set true.
            cell.ReadOnly = true;
            Assert.True(cell.ReadOnly);
            Assert.Equal(1, callCount);

            // Set same.
            cell.ReadOnly = true;
            Assert.True(cell.ReadOnly);
            Assert.Equal(1, callCount);

            // Set different.
            cell.ReadOnly = false;
            Assert.False(cell.ReadOnly);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.CellStateChanged -= handler;
            cell.ReadOnly = true;
            Assert.True(cell.ReadOnly);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_ReadOnly_SetShared_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.Resizable, false)]
        [InlineData(DataGridViewElementStates.Resizable | DataGridViewElementStates.ReadOnly, false)]
        [InlineData(DataGridViewElementStates.Resizable | DataGridViewElementStates.Selected, false)]
        public void DataGridViewCell_Resizable_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.Resizable);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, true)]
        [InlineData(DataGridViewTriState.False, false)]
        [InlineData(DataGridViewTriState.NotSet, false)]
        public void DataGridViewCell_Resizable_GetWithRow_ReturnsExpected(DataGridViewTriState rowResizable, bool expected)
        {
            using var row = new DataGridViewRow
            {
                Resizable = rowResizable
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(expected, cell.Resizable);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, DataGridViewTriState.True, true)]
        [InlineData(DataGridViewTriState.True, DataGridViewTriState.False, true)]
        [InlineData(DataGridViewTriState.True, DataGridViewTriState.NotSet, true)]
        [InlineData(DataGridViewTriState.False, DataGridViewTriState.True, true)]
        [InlineData(DataGridViewTriState.False, DataGridViewTriState.False, false)]
        [InlineData(DataGridViewTriState.False, DataGridViewTriState.NotSet, true)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewTriState.True, true)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewTriState.False, true)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewTriState.NotSet, true)]
        public void DataGridViewCell_Resizable_GetWithDataGridView_ReturnsExpected(DataGridViewTriState rowResizable, DataGridViewTriState columnResizable, bool expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Resizable = rowResizable;
            column.Resizable = columnResizable;
            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(expected, cell.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewTriState))]
        public void DataGridViewCell_Resizable_GetShared_ReturnsExpected(DataGridViewTriState columnResizable)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.Resizable = columnResizable;
            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.Selected, true)]
        [InlineData(DataGridViewElementStates.Selected | DataGridViewElementStates.ReadOnly, true)]
        [InlineData(DataGridViewElementStates.Selected | DataGridViewElementStates.Frozen, true)]
        public void DataGridViewCell_Selected_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.Selected);
        }

        [WinFormsFact]
        public void DataGridViewCell_Selected_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewCell_Selected_GetWithDataGridView_ReturnsExpected(bool rowSelected, bool columnSelected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Selected = rowSelected;
            column.Selected = columnSelected;
            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(rowSelected, cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Selected_GetShared_ReturnsExpected(bool columnSelected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.Selected = columnSelected;
            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_Selected_Set_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.Selected = true);
            Assert.False(cell.Selected);

            cell.Selected = false;
            Assert.False(cell.Selected);
        }

        [WinFormsFact]
        public void DataGridViewCell_Selected_SetWithRow_ThrowsInvalidOperationException()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            Assert.Throws<InvalidOperationException>(() => cell.Selected = true);
            Assert.False(cell.Selected);

            cell.Selected = false;
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Selected_SetWithColumn_ThrowsInvalidOperationException(bool value)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Selected_SetWithDataGridView_ReturnsExpected(bool value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            cell.Selected = value;
            Assert.Equal(value, cell.Selected);
            Assert.Equal(value, (cell.State & DataGridViewElementStates.Selected) != 0);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.Selected = value;
            Assert.Equal(value, cell.Selected);
            Assert.Equal(value, (cell.State & DataGridViewElementStates.Selected) != 0);
            Assert.False(control.IsHandleCreated);

            // Set different.
            cell.Selected = !value;
            Assert.Equal(!value, cell.Selected);
            Assert.Equal(!value, (cell.State & DataGridViewElementStates.Selected) != 0);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Selected_SetShared_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_Selected_SetWithDataGridView_CallsCellStateChanged()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Same(cell, e.Cell);
                Assert.Equal(DataGridViewElementStates.Selected, e.StateChanged);
            };
            control.CellStateChanged += handler;

            // Set true.
            cell.Selected = true;
            Assert.True(cell.Selected);
            Assert.Equal(1, callCount);

            // Set same.
            cell.Selected = true;
            Assert.True(cell.Selected);
            Assert.Equal(1, callCount);

            // Set different.
            cell.Selected = false;
            Assert.False(cell.Selected);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.CellStateChanged -= handler;
            cell.Selected = true;
            Assert.True(cell.Selected);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> Style_Set_TestData()
        {
            yield return new object[] { null, new DataGridViewCellStyle() };

            var style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomRight };
            yield return new object[] { style, style };
        }

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_Set_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            using var cell = new SubDataGridViewCell
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

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_SetWithNonNullOldValue_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var oldValue = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomCenter
            };
            using var cell = new SubDataGridViewCell
            {
                Style = oldValue
            };
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_SetWithRow_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_SetWithNonNullOldValueWithRow_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var oldValue = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomCenter
            };
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell
            {
                Style = oldValue
            };
            row.Cells.Add(cell);

            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_SetWithColumn_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_SetWithNonNullOldValueWithColumn_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var oldValue = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomCenter
            };
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell
            {
                Style = oldValue
            };
            column.HeaderCell = cell;

            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_SetWithDataGridView_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [WinFormsTheory]
        [MemberData(nameof(Style_Set_TestData))]
        public void DataGridViewCell_Style_SetWithNonNullOldValueWithDataGridView_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            var oldValue = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomCenter
            };
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.Style = oldValue;

            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);

            // Set same.
            cell.Style = value;
            Assert.Equal(expected, cell.Style);
            Assert.True(cell.HasStyle);
        }

        [WinFormsFact]
        public void DataGridViewCell_Style_SetWithDataGridView_CallsCellStyleChanged()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            int callCount = 0;
            DataGridViewCellEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(0, e.ColumnIndex);
                Assert.Equal(0, e.RowIndex);
                callCount++;
            };
            control.CellStyleChanged += handler;

            var style1 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // Set non-null.
            cell.Style = style1;
            Assert.Equal(style1, cell.Style);
            Assert.True(cell.HasStyle);
            Assert.Equal(1, callCount);

            // Set same.
            cell.Style = style1;
            Assert.Equal(style1, cell.Style);
            Assert.True(cell.HasStyle);
            Assert.Equal(1, callCount);

            // Set different.
            var style2 = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.BottomCenter
            };
            cell.Style = style2;
            Assert.Same(style2, cell.Style);
            Assert.True(cell.HasStyle);
            Assert.Equal(2, callCount);

            // Set null.
            cell.Style = null;
            Assert.NotNull(cell.Style);
            Assert.True(cell.HasStyle);
            Assert.Equal(3, callCount);

            // Set null again.
            cell.Style = null;
            Assert.NotNull(cell.Style);
            Assert.True(cell.HasStyle);
            Assert.Equal(4, callCount);

            // Set non-null.
            cell.Style = style2;
            Assert.NotNull(cell.Style);
            Assert.True(cell.HasStyle);
            Assert.Equal(5, callCount);

            // Remove handler.
            control.CellStyleChanged -= handler;
            cell.Style = style1;
            Assert.Equal(style1, cell.Style);
            Assert.True(cell.HasStyle);
            Assert.Equal(5, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewCell_Tag_Set_GetReturnsExpected(object value)
        {
            using var cell = new SubDataGridViewCell
            {
                Tag = value
            };
            Assert.Same(value, cell.Tag);

            // Set same.
            cell.Tag = value;
            Assert.Same(value, cell.Tag);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewCell_Tag_SetWithNonNullOldValue_GetReturnsExpected(object value)
        {
            using var cell = new SubDataGridViewCell
            {
                Tag = "tag"
            };
            cell.Tag = value;
            Assert.Same(value, cell.Tag);

            // Set same.
            cell.Tag = value;
            Assert.Same(value, cell.Tag);
        }

        [WinFormsFact]
        public void DataGridViewCell_ToolTipText_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Empty(cell.ToolTipText);
        }

        [WinFormsFact]
        public void DataGridViewCell_ToolTipText_GetWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Empty(cell.ToolTipText);
        }

        [WinFormsFact]
        public void DataGridViewCell_ToolTipText_GetWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Empty(cell.ToolTipText);
        }

        [WinFormsFact]
        public void DataGridViewCell_ToolTipText_GetSharedRow_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Empty(cell.ToolTipText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_Set_GetReturnsExpected(string value, string expected)
        {
            using var cell = new SubDataGridViewCell
            {
                ToolTipText = value
            };
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var cell = new SubDataGridViewCell
            {
                ToolTipText = "ToolTipText"
            };
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithRow_GetReturnsExpected(string value, string expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithRowWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell
            {
                ToolTipText = "ToolTipText"
            };
            row.Cells.Add(cell);

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithColumn_GetReturnsExpected(string value, string expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithColumnWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell
            {
                ToolTipText = "ToolTipText"
            };
            column.HeaderCell = cell;

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithDataGridView_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ToolTipText = "ToolTipText";

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetShared_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewCell_ToolTipText_SetSharedWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            cell.ToolTipText = "ToolTipText";

            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ToolTipText = value;
            Assert.Equal(expected, cell.ToolTipText);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_ValueType_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Null(cell.ValueType);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_GetWithColumn_ReturnsExpected(Type valueType)
        {
            using var column = new DataGridViewColumn
            {
                ValueType = valueType
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(typeof(object), cell.ValueType);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_GetWithDataGridView_ReturnsExpected(Type valueType)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                ValueType = valueType
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Same(valueType, cell.ValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_Set_GetReturnsExpected(Type value)
        {
            using var cell = new SubDataGridViewCell
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_SetWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            using var cell = new SubDataGridViewCell
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_SetWithRow_GetReturnsExpected(Type value)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_SetWithRowWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell
            {
                ValueType = typeof(string)
            };
            row.Cells.Add(cell);

            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
        }

        public static IEnumerable<object[]> ValueType_SetWithColumn_TestData()
        {
            yield return new object[] { null, typeof(object) };
            yield return new object[] { typeof(object), typeof(object) };
            yield return new object[] { typeof(int), typeof(int) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ValueType_SetWithColumn_TestData))]
        public void DataGridViewCell_ValueType_SetWithColumn_GetReturnsExpected(Type value, Type expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        [WinFormsTheory]
        [MemberData(nameof(ValueType_SetWithColumn_TestData))]
        public void DataGridViewCell_ValueType_SetWithColumnWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell
            {
                ValueType = typeof(string)
            };
            column.HeaderCell = cell;

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_SetWithDataGridView_GetReturnsExpected(Type value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewCell_ValueType_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(Type value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ValueType = typeof(string);

            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(value, cell.ValueType);
            Assert.Equal(value, cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.Visible, false)]
        [InlineData(DataGridViewElementStates.Visible | DataGridViewElementStates.ReadOnly, false)]
        [InlineData(DataGridViewElementStates.Visible | DataGridViewElementStates.Selected, false)]
        public void DataGridViewCell_Visible_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.Visible);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Visible_GetWithRow_ReturnsExpected(bool rowVisible)
        {
            using var row = new DataGridViewRow
            {
                Visible = rowVisible
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(rowVisible, cell.Visible);
        }

        [WinFormsTheory]
        [InlineData(true, true, true, true)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, true, true)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, false, true)]
        [InlineData(true, false, false, false)]
        [InlineData(false, true, true, true)]
        [InlineData(false, true, true, false)]
        [InlineData(false, true, false, true)]
        [InlineData(false, true, false, false)]
        [InlineData(false, false, true, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, false, false)]
        public void DataGridViewCell_Visible_GetWithDataGridView_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            control.Columns.Add(column);
            control.Rows.Add(new DataGridViewRow());
            DataGridViewRow row = control.Rows[0];
            row.Visible = rowVisible;
            column.Visible = columnVisible;

            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(rowVisible && columnVisible, cell.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Visible_GetShared_ReturnsExpected(bool columnVisible)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.Visible = columnVisible;

            DataGridViewCell cell = row.Cells[0];
            Assert.False(cell.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, true, true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(true, true, false, true, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(false, true, true, true, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(true, true, true, false, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single)]
        [InlineData(true, false, true, true, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.Single, DataGridViewAdvancedCellBorderStyle.None, DataGridViewAdvancedCellBorderStyle.Single)]
        public void DataGridViewCell_AdjustCellBorderStyle_AllSingleWithoutDataGridView_ReturnsExpected(bool singleVerticalBorderAdded, bool singleHorizontalBorderAdded, bool isFirstDisplayedColumn, bool isFirstDisplayedRow, DataGridViewAdvancedCellBorderStyle expectedLeft, DataGridViewAdvancedCellBorderStyle expectedRight, DataGridViewAdvancedCellBorderStyle expectedTop, DataGridViewAdvancedCellBorderStyle expectedBottom)
        {
            using var cell = new SubDataGridViewCell();
            var controlAdvancedBorderStyleInput = new DataGridViewAdvancedBorderStyle
            {
                All = DataGridViewAdvancedCellBorderStyle.Single
            };
            var controlAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Same(controlAdvancedBorderStylePlaceholder, cell.AdjustCellBorderStyle(controlAdvancedBorderStyleInput, controlAdvancedBorderStylePlaceholder, singleVerticalBorderAdded, singleHorizontalBorderAdded, isFirstDisplayedColumn, isFirstDisplayedRow));
            Assert.Equal(expectedLeft, controlAdvancedBorderStylePlaceholder.Left);
            Assert.Equal(expectedRight, controlAdvancedBorderStylePlaceholder.Right);
            Assert.Equal(expectedTop, controlAdvancedBorderStylePlaceholder.Top);
            Assert.Equal(expectedBottom, controlAdvancedBorderStylePlaceholder.Bottom);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewAdvancedCellBorderStyle.None)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.Inset)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.InsetDouble)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.Outset)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.OutsetDouble)]
        [InlineData(DataGridViewAdvancedCellBorderStyle.OutsetPartial)]
        public void DataGridViewCell_AdjustCellBorderStyle_InvalidAll_ReturnsExpected(DataGridViewAdvancedCellBorderStyle all)
        {
            using var cell = new SubDataGridViewCell();
            var controlAdvancedBorderStyleInput = new DataGridViewAdvancedBorderStyle
            {
                All = all
            };
            var controlAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Same(controlAdvancedBorderStyleInput, cell.AdjustCellBorderStyle(controlAdvancedBorderStyleInput, controlAdvancedBorderStylePlaceholder, true, true, true, true));
        }

        [WinFormsFact]
        public void DataGridViewCell_AdjustCellBorderStyle_NullDataGridViewAdvancedBorderStyleInput_ThrowsArgumentNullException()
        {
            using var cell = new SubDataGridViewCell();
            var controlAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Throws<ArgumentNullException>("dataGridViewAdvancedBorderStyleInput", () => cell.AdjustCellBorderStyle(null, controlAdvancedBorderStylePlaceholder, true, true, true, true));
        }

        [WinFormsFact]
        public void DataGridViewCell_AdjustCellBorderStyle_AllNotSetWithoutDataGridView_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell();
            var controlAdvancedBorderStyleInput = new DataGridViewAdvancedBorderStyle
            {
                Left = DataGridViewAdvancedCellBorderStyle.Single,
                Right = DataGridViewAdvancedCellBorderStyle.None
            };
            var controlAdvancedBorderStylePlaceholder = new DataGridViewAdvancedBorderStyle();
            Assert.Same(controlAdvancedBorderStyleInput, cell.AdjustCellBorderStyle(controlAdvancedBorderStyleInput, controlAdvancedBorderStylePlaceholder, true, true, true, true));
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

        [WinFormsTheory]
        [MemberData(nameof(AdjustCellBorderStyle_NullDataGridViewAdvancedBorderStylePlaceholder_TestData))]
        public void DataGridViewCell_AdjustCellBorderStyle_NullDataGridViewAdvancedBorderStylePlaceholder_ThrowsArgumentNullException(DataGridViewAdvancedBorderStyle controlAdvancedBorderStyleInput)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("dataGridViewAdvancedBorderStylePlaceholder", () => cell.AdjustCellBorderStyle(controlAdvancedBorderStyleInput, null, true, true, true, true));
        }

        public static IEnumerable<object[]> BorderWidths_TestData()
        {
            yield return new object[] { new DataGridViewAdvancedBorderStyle(), new Rectangle(0, 0, 0, 0) };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle(),
                new Rectangle(0, 0, 0, 0)
            };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle
                {
                    All = DataGridViewAdvancedCellBorderStyle.OutsetDouble
                },
                new Rectangle(2, 2, 2, 2)
            };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle
                {
                    All = DataGridViewAdvancedCellBorderStyle.InsetDouble
                },
                new Rectangle(2, 2, 2, 2)
            };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle
                {
                    All = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(1, 1, 1, 1)
            };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle
                {
                    Left = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(1, 0, 0, 0)
            };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle
                {
                    Right = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(0, 0, 1, 0)
            };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle
                {
                    Top = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(0, 1, 0, 0)
            };
            yield return new object[]
            {
                new DataGridViewAdvancedBorderStyle
                {
                    Bottom = DataGridViewAdvancedCellBorderStyle.Single
                },
                new Rectangle(0, 0, 0, 1)
            };
        }

        [WinFormsTheory]
        [MemberData(nameof(BorderWidths_TestData))]
        public void DataGridViewCell_BorderWidths_Invoke_ReturnsExpected(DataGridViewAdvancedBorderStyle advancedBorderStyle, Rectangle expected)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(expected, cell.BorderWidths(advancedBorderStyle));
        }

        [WinFormsTheory]
        [MemberData(nameof(BorderWidths_TestData))]
        public void DataGridViewCell_BorderWidths_InvokeWithRow_ReturnsExpected(DataGridViewAdvancedBorderStyle advancedBorderStyle, Rectangle expected)
        {
            using var row = new DataGridViewRow
            {
                DividerHeight = 10
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(new Rectangle(expected.X, expected.Y, expected.Width, expected.Height + 10), cell.BorderWidths(advancedBorderStyle));
        }

        [WinFormsTheory]
        [MemberData(nameof(BorderWidths_TestData))]
        public void DataGridViewCell_BorderWidths_InvokeWithColumn_ReturnsExpected(DataGridViewAdvancedBorderStyle advancedBorderStyle, Rectangle expected)
        {
            using var column = new DataGridViewColumn
            {
                DividerWidth = 10
            };
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(new Rectangle(expected.X, expected.Y, expected.Width + 10, expected.Height), cell.BorderWidths(advancedBorderStyle));
        }

        [WinFormsTheory]
        [MemberData(nameof(BorderWidths_TestData))]
        public void DataGridViewCell_BorderWidths_InvokeWithDataGridView_ReturnsExpected(DataGridViewAdvancedBorderStyle advancedBorderStyle, Rectangle expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            column.DividerWidth = 10;
            row.DividerHeight = 11;
            SubDataGridViewCell cell = (SubDataGridViewCell)row.Cells[0];
            Assert.Equal(new Rectangle(expected.X, expected.Y, expected.Width + 10, expected.Height + 11), cell.BorderWidths(advancedBorderStyle));
        }

        [WinFormsTheory]
        [MemberData(nameof(BorderWidths_TestData))]
        public void DataGridViewCell_BorderWidths_InvokeWithDataGridViewRightToLeft_ReturnsExpected(DataGridViewAdvancedBorderStyle advancedBorderStyle, Rectangle expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                RightToLeft = RightToLeft.Yes
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            column.DividerWidth = 10;
            row.DividerHeight = 11;
            SubDataGridViewCell cell = (SubDataGridViewCell)row.Cells[0];
            Assert.Equal(new Rectangle(expected.X + 10, expected.Y, expected.Width, expected.Height + 11), cell.BorderWidths(advancedBorderStyle));
        }

        [WinFormsTheory]
        [MemberData(nameof(BorderWidths_TestData))]
        public void DataGridViewCell_BorderWidths_InvokeSharedReturnsExpected(DataGridViewAdvancedBorderStyle advancedBorderStyle, Rectangle expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            column.DividerWidth = 10;
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Equal(new Rectangle(expected.X, expected.Y, expected.Width + 10, expected.Height), cell.BorderWidths(advancedBorderStyle));
        }

        [WinFormsFact]
        public void DataGridViewCell_BorderWidths_NullAdvancedBorderStyleInput_ThrowsArgumentNullException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("advancedBorderStyle", () => cell.BorderWidths(null));
        }

        [WinFormsFact]
        public void DataGridViewCell_ClickUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.ClickUnsharesRow(null));
        }

        [WinFormsFact]
        public void DataGridViewCell_Clone_NonEmpty_Success()
        {
            using var menu = new ContextMenuStrip();
            using var source = new SubDataGridViewCell
            {
                ContextMenuStrip = menu,
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

            Assert.False(menu.IsDisposed);
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

        [WinFormsFact]
        public void DataGridViewCell_Clone_Empty_Success()
        {
            using var source = new SubDataGridViewCell();
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

        [WinFormsFact]
        public void DataGridViewCell_ContentClickUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.ContentClickUnsharesRow(null));
        }

        [WinFormsFact]
        public void DataGridViewCell_ContentDoubleClickUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.ContentDoubleClickUnsharesRow(null));
        }

        public class CreateAccessibilityInstanceTests : DataGridViewCell
        {
            [WinFormsFact]
            public void DataGridViewCell_AccessibilityObject_Get_ReturnsExpected()
            {
                using var cell = new SubDataGridViewCell();
                DataGridViewCell.DataGridViewCellAccessibleObject accessibleObject = Assert.IsType<DataGridViewCell.DataGridViewCellAccessibleObject>(cell.AccessibilityObject);
                Assert.Same(accessibleObject, cell.AccessibilityObject);
                Assert.Same(cell, accessibleObject.Owner);
            }

            public static IEnumerable<object[]> AccessibilityObject_CustomCreateAccessibilityInstance_TestData()
            {
                yield return new object[] { null };
                yield return new object[] { new AccessibleObject() };
                yield return new object[] { new DataGridViewCell.DataGridViewCellAccessibleObject(null) };
                yield return new object[] { new DataGridViewCell.DataGridViewCellAccessibleObject(new SubDataGridViewCell()) };
            }

            [WinFormsTheory]
            [MemberData(nameof(AccessibilityObject_CustomCreateAccessibilityInstance_TestData))]
            public void DataGridViewCell_AccessibilityObject_GetCustomCreateAccessibilityInstance_ReturnsExpected(AccessibleObject result)
            {
                using var control = new CustomCreateAccessibilityInstanceDataGridViewCell
                {
                    CreateAccessibilityResult = result
                };
                Assert.Same(result, control.AccessibilityObject);
                Assert.Same(control.AccessibilityObject, control.AccessibilityObject);
            }

            [WinFormsFact]
            public void DataGridViewCell_CreateAccessibilityInstance_Invoke_ReturnsExpected()
            {
                using var cell = new SubDataGridViewCell();
                DataGridViewCell.DataGridViewCellAccessibleObject instance = Assert.IsAssignableFrom<DataGridViewCell.DataGridViewCellAccessibleObject>(cell.CreateAccessibilityInstance());
                Assert.NotNull(instance);
                Assert.Same(cell, instance.Owner);
                Assert.Equal(AccessibleRole.Cell, instance.Role);
                Assert.NotSame(cell.CreateAccessibilityInstance(), instance);
                Assert.NotSame(cell.AccessibilityObject, instance);
            }

            private class CustomCreateAccessibilityInstanceDataGridViewCell : DataGridViewCell
            {
                public AccessibleObject CreateAccessibilityResult { get; set; }

                protected override AccessibleObject CreateAccessibilityInstance() => CreateAccessibilityResult;
            }
        }

        [WinFormsFact]
        public void DataGridViewCell_DetachEditingDataGridViewCell_InvokeNoDataGridView_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.DetachEditingControl());
        }

        [WinFormsFact]
        public void DataGridViewCell_DetachEditingDataGridViewCell_InvokeNoEditingControl_ThrowsInvalidOperationException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.DetachEditingControl());
        }

        [WinFormsFact]
        public void DataGridViewCell_Dispose_InvokeWithoutContextMenuStrip_Nop()
        {
            using var cell = new SubDataGridViewCell();
            cell.Dispose();
            Assert.Null(cell.ContextMenuStrip);

            // Call multiple times.
            cell.Dispose();
            Assert.Null(cell.ContextMenuStrip);
        }

        [WinFormsFact]
        public void DataGridViewCell_Dispose_InvokeWithContextMenuStrip_Success()
        {
            using var menu = new ContextMenuStrip();
            using var cell = new SubDataGridViewCell
            {
                ContextMenuStrip = menu
            };
            cell.Dispose();
            Assert.Same(menu, cell.ContextMenuStrip);
            Assert.False(menu.IsDisposed);

            // Call multiple times.
            cell.Dispose();
            Assert.Same(menu, cell.ContextMenuStrip);
            Assert.False(menu.IsDisposed);

            // Dispose menu.
            menu.Dispose();
            Assert.Same(menu, cell.ContextMenuStrip);
            Assert.True(menu.IsDisposed);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Dispose_InvokeDisposingWithoutContextMenuStrip_Nop(bool disposing)
        {
            using var cell = new SubDataGridViewCell();
            cell.Dispose(disposing);
            Assert.Null(cell.ContextMenuStrip);

            // Call multiple times.
            cell.Dispose(disposing);
            Assert.Null(cell.ContextMenuStrip);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewCell_Dispose_InvokeDisposingWithContextMenuStrip_Success(bool disposing)
        {
            using var menu = new ContextMenuStrip();
            using var cell = new SubDataGridViewCell
            {
                ContextMenuStrip = menu
            };
            cell.Dispose(disposing);
            Assert.Same(menu, cell.ContextMenuStrip);
            Assert.False(menu.IsDisposed);

            // Call multiple times.
            cell.Dispose(disposing);
            Assert.Same(menu, cell.ContextMenuStrip);
            Assert.False(menu.IsDisposed);

            // Dispose menu.
            menu.Dispose();
            Assert.Equal(disposing, cell.ContextMenuStrip == menu);
            Assert.True(menu.IsDisposed);
        }

        [WinFormsFact]
        public void DataGridViewCell_DoubleClickUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.DoubleClickUnsharesRow(null));
        }

        [WinFormsFact]
        public void DataGridViewCell_EnterUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.EnterUnsharesRow(-1, true));
        }

        public static IEnumerable<object[]> GetClipboardContent_TestData()
        {
            yield return new object[] { -2, true, true, true, true, "format", null };
            yield return new object[] { -2, true, true, true, true, null, null };
            yield return new object[] { -1, true, true, true, true, "format", null };
            yield return new object[] { -1, true, true, true, true, null, null };
            yield return new object[] { 0, true, true, true, true, "format", null };
            yield return new object[] { 0, true, true, true, true, null, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_TestData))]
        public void DataGridViewCell_GetClipboardContent_Invoke_ReturnsExpected(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format, object expected)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(expected, cell.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_TestData))]
        public void DataGridViewCell_GetClipboardContent_InvokeWithRow_ReturnsExpected(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format, object expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(expected, cell.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format));
        }

        public static IEnumerable<object[]> GetClipboardContent_WithColumn_TestData()
        {
            yield return new object[] { true, true, true, true, "format", null };
            yield return new object[] { true, true, true, true, null, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_WithColumn_TestData))]
        public void DataGridViewCell_GetClipboardContent_InvokeWithColumn_ReturnsExpected(bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format, object expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(expected, cell.GetClipboardContent(-1, firstCell, lastCell, inFirstRow, inLastRow, format));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_WithColumn_TestData))]
        public void DataGridViewCell_GetClipboardContent_InvokeWithDataGridView_ReturnsExpected(bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format, object expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Equal(expected, cell.GetClipboardContent(0, firstCell, lastCell, inFirstRow, inLastRow, format));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetClipboardContent_WithColumn_TestData))]
        public void DataGridViewCell_GetClipboardContent_InvokeShared_ReturnsExpected(bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format, object expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Equal(expected, cell.GetClipboardContent(0, firstCell, lastCell, inFirstRow, inLastRow, format));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewCell_GetClipboardContent_InvalidRowIndexWithColumn_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetClipboardContent(rowIndex, true, true, true, true, "format"));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetClipboardContent_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetClipboardContent(rowIndex, true, true, true, true, "format"));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetClipboardContent_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetClipboardContent(rowIndex, true, true, true, true, "format"));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetContentBounds_Invoke_ReturnsExpected(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetContentBounds_InvokeWithRow_ReturnsExpected(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetContentBounds_InvokeWithColumn_ReturnsExpected(int rowIndex)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetContentBounds_InvokeWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetContentBounds_InvokeShared_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(0));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetContentBounds_InvokeInvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetContentBounds(rowIndex));
        }

        public static IEnumerable<object[]> GetContentBounds_TestData()
        {
            yield return new object[] { null, -2 };
            yield return new object[] { null, -1 };
            yield return new object[] { null, 0 };
            yield return new object[] { null, 1 };
            yield return new object[] { new DataGridViewCellStyle(), -2 };
            yield return new object[] { new DataGridViewCellStyle(), -1 };
            yield return new object[] { new DataGridViewCellStyle(), 0 };
            yield return new object[] { new DataGridViewCellStyle(), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetContentBounds_TestData))]
        public void DataGridViewCell_GetContentBounds_InvokeWithoutGraphics_ReturnsExpected(DataGridViewCellStyle cellStyle, int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(null, cellStyle, rowIndex));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetContentBounds_TestData))]
        public void DataGridViewCell_GetContentBounds_InvokeWithGraphics_ReturnsExpected(DataGridViewCellStyle cellStyle, int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Equal(Rectangle.Empty, cell.GetContentBounds(graphics, cellStyle, rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(-1, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(0, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(1, DataGridViewDataErrorContexts.Formatting)]
        public void DataGridViewCell_GetEditedFormattedValue_Invoke_ReturnsExpected(int rowIndex, DataGridViewDataErrorContexts context)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Null(cell.GetEditedFormattedValue(rowIndex, context));
        }

        [WinFormsTheory]
        [InlineData(-2, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(-1, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(0, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(1, DataGridViewDataErrorContexts.Formatting)]
        public void DataGridViewCell_GetEditedFormattedValue_InvokeWithValue_ReturnsExpected(int rowIndex, DataGridViewDataErrorContexts context)
        {
            using var cell = new SubDataGridViewCell
            {
                Value = "Value"
            };
            Assert.Null(cell.GetEditedFormattedValue(rowIndex, context));
        }

        [WinFormsTheory]
        [InlineData(-2, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(-1, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(0, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(1, DataGridViewDataErrorContexts.Formatting)]
        public void DataGridViewCell_GetEditedFormattedValue_InvokeWithRow_ReturnsExpected(int rowIndex, DataGridViewDataErrorContexts context)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Null(cell.GetEditedFormattedValue(rowIndex, context));
        }

        [WinFormsTheory]
        [InlineData(-2, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(-1, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(0, DataGridViewDataErrorContexts.Formatting)]
        [InlineData(1, DataGridViewDataErrorContexts.Formatting)]
        public void DataGridViewCell_GetEditedFormattedValue_InvokeWithColumn_ReturnsExpected(int rowIndex, DataGridViewDataErrorContexts context)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Null(cell.GetEditedFormattedValue(rowIndex, context));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewDataErrorContexts.Formatting)]
        public void DataGridViewCell_GetEditedFormattedValue_InvokeWithDataGridView_ReturnsExpected(DataGridViewDataErrorContexts context)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ValueType = typeof(string);
            Assert.Empty((string)cell.GetEditedFormattedValue(0, context));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewDataErrorContexts.Formatting)]
        public void DataGridViewCell_GetEditedFormattedValue_InvokeWithDataGridViewWithValue_ReturnsExpected(DataGridViewDataErrorContexts context)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.Value = "Value";
            cell.ValueType = typeof(string);
            Assert.Equal("Value", (string)cell.GetEditedFormattedValue(0, context));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewDataErrorContexts.Formatting)]
        public void DataGridViewCell_GetEditedFormattedValue_InvokeShared_ReturnsExpected(DataGridViewDataErrorContexts context)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            cell.ValueType = typeof(string);
            Assert.Empty((string)cell.GetEditedFormattedValue(0, context));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewRow_GetEditedFormattedValue_InvalidRowIndex_ThrowsArgumentOutRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                VirtualMode = true
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetEditedFormattedValue(rowIndex, DataGridViewDataErrorContexts.Formatting));
        }

        public static IEnumerable<object[]> GetErrorIconBounds_TestData()
        {
            yield return new object[] { null, -2 };
            yield return new object[] { null, -1 };
            yield return new object[] { null, 0 };
            yield return new object[] { null, 1 };
            yield return new object[] { new DataGridViewCellStyle(), -2 };
            yield return new object[] { new DataGridViewCellStyle(), -1 };
            yield return new object[] { new DataGridViewCellStyle(), 0 };
            yield return new object[] { new DataGridViewCellStyle(), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetErrorIconBounds_TestData))]
        public void DataGridViewCell_GetErrorIconBounds_InvokeWithoutGraphics_ReturnsExpected(DataGridViewCellStyle cellStyle, int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(Rectangle.Empty, cell.GetErrorIconBounds(null, cellStyle, rowIndex));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetErrorIconBounds_TestData))]
        public void DataGridViewCell_GetErrorIconBounds_InvokeWithGraphics_ReturnsExpected(DataGridViewCellStyle cellStyle, int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Equal(Rectangle.Empty, cell.GetErrorIconBounds(graphics, cellStyle, rowIndex));
        }

        [WinFormsTheory]
        [InlineData(null, -2, "")]
        [InlineData(null, -1, "")]
        [InlineData(null, 0, "")]
        [InlineData("", -2, "")]
        [InlineData("", -1, "")]
        [InlineData("", 0, "")]
        [InlineData("ErrorText", -2, "ErrorText")]
        [InlineData("ErrorText", -1, "ErrorText")]
        [InlineData("ErrorText", 0, "ErrorText")]
        public void DataGridViewCell_GetErrorText_Invoke_ReturnsExpected(string errorText, int rowIndex, string expected)
        {
            using var cell = new SubDataGridViewCell
            {
                ErrorText = errorText
            };
            Assert.Equal(expected, cell.GetErrorText(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(null, -2, "")]
        [InlineData(null, -1, "")]
        [InlineData(null, 0, "")]
        [InlineData("", -2, "")]
        [InlineData("", -1, "")]
        [InlineData("", 0, "")]
        [InlineData("ErrorText", -2, "ErrorText")]
        [InlineData("ErrorText", -1, "ErrorText")]
        [InlineData("ErrorText", 0, "ErrorText")]
        public void DataGridViewCell_GetErrorText_InvokeWithRow_ReturnsExpected(string errorText, int rowIndex, string expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell
            {
                ErrorText = errorText
            };
            row.Cells.Add(cell);
            Assert.Equal(expected, cell.GetErrorText(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(null, -2, "")]
        [InlineData(null, -1, "")]
        [InlineData(null, 0, "")]
        [InlineData("", -2, "")]
        [InlineData("", -1, "")]
        [InlineData("", 0, "")]
        [InlineData("ErrorText", -2, "ErrorText")]
        [InlineData("ErrorText", -1, "ErrorText")]
        [InlineData("ErrorText", 0, "ErrorText")]
        public void DataGridViewCell_GetErrorText_InvokeWithColumn_ReturnsExpected(string errorText, int rowIndex, string expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell
            {
                ErrorText = errorText
            };
            column.HeaderCell = cell;
            Assert.Equal(expected, cell.GetErrorText(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(null, -2, "")]
        [InlineData(null, -1, "")]
        [InlineData(null, 0, "")]
        [InlineData("", -2, "")]
        [InlineData("", -1, "")]
        [InlineData("", 0, "")]
        [InlineData("ErrorText", -2, "ErrorText")]
        [InlineData("ErrorText", -1, "ErrorText")]
        [InlineData("ErrorText", 0, "ErrorText")]
        public void DataGridViewCell_GetErrorText_InvokeWithDataGridView_ReturnsExpected(string errorText, int rowIndex, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.ErrorText = errorText;
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            Assert.Equal(expected, cell.GetErrorText(rowIndex));
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, -2, "")]
        [InlineData(null, -1, "")]
        [InlineData(null, 0, "")]
        [InlineData("", -2, "")]
        [InlineData("", -1, "")]
        [InlineData("", 0, "")]
        [InlineData("ErrorText", -2, "ErrorText")]
        [InlineData("ErrorText", -1, "ErrorText")]
        [InlineData("ErrorText", 0, "ErrorText")]
        public void DataGridViewCell_GetErrorText_InvokeShared_ReturnsExpected(string errorText, int rowIndex, string expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            cell.ErrorText = errorText;
            int callCount = 0;
            control.CellErrorTextNeeded += (sender, e) => callCount++;

            Assert.Equal(expected, cell.GetErrorText(rowIndex));
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2, 0, "errorText1")]
        [InlineData(-1, 0, "errorText1")]
        [InlineData(0, 1, "errorText2")]
        public void DataGridViewCell_GetErrorText_InvokeVirtualMode_ReturnsExpected(int rowIndex, int expectedCallCount, string expectedErrorText)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                VirtualMode = true
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(rowIndex, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.CellErrorTextNeeded += handler;

            Assert.Equal(expectedErrorText, cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2, 0, "errorText1")]
        [InlineData(-1, 0, "errorText1")]
        [InlineData(0, 1, "errorText2")]
        public void DataGridViewCell_GetErrorText_InvokeSharedWithVirtualMode_ReturnsExpected(int rowIndex, int expectedCallCount, string expectedErrorText)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                VirtualMode = true
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(rowIndex, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.CellErrorTextNeeded += handler;

            Assert.Equal(expectedErrorText, cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2, 0, "errorText1")]
        [InlineData(-1, 0, "errorText1")]
        [InlineData(0, 1, "errorText2")]
        public void DataGridViewCell_GetErrorText_InvokeDataGridViewWithDataSource_ReturnsExpected(int rowIndex, int expectedCallCount, string expectedErrorText)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                DataSource = new[] { new { Name = "Name" } }
            };
            control.Columns.Add(column);
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);

            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(rowIndex, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.CellErrorTextNeeded += handler;

            Assert.Equal(expectedErrorText, cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2, 0, "errorText1")]
        [InlineData(-1, 0, "errorText1")]
        [InlineData(0, 1, "errorText2")]
        public void DataGridViewCell_GetErrorText_InvokeSharedWithDataSource_ReturnsExpected(int rowIndex, int expectedCallCount, string expectedErrorText)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                DataSource = new[] { new { Name = "Name" } }
            };
            control.Columns.Add(column);
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);

            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            cell.ErrorText = "errorText1";

            int callCount = 0;
            DataGridViewCellErrorTextNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(rowIndex, e.RowIndex);
                Assert.Equal("errorText1", e.ErrorText);
                e.ErrorText = "errorText2";
            };
            control.CellErrorTextNeeded += handler;

            Assert.Equal(expectedErrorText, cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);

            // Remove the handler.
            control.CellErrorTextNeeded -= handler;
            Assert.Equal("errorText1", cell.GetErrorText(rowIndex));
            Assert.Equal(expectedCallCount, callCount);
        }

        [WinFormsFact]
        public void DataGridViewRow_GetErrorText_InvalidRowIndex_ThrowsArgumentOutRangeException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                VirtualMode = true
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("e.RowIndex", () => cell.GetErrorText(1));
        }

        public static IEnumerable<object[]> GetFormattedValue_TestData()
        {
            yield return new object[] { "value", -2, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter(), DataGridViewDataErrorContexts.Formatting, null };
            yield return new object[] { "value", -1, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter(), DataGridViewDataErrorContexts.Formatting, null };
            yield return new object[] { "value", 0, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter(), DataGridViewDataErrorContexts.Formatting, null };
            yield return new object[] { null, -2, null, null, null, (DataGridViewDataErrorContexts)(DataGridViewDataErrorContexts.Formatting - 1), null };
            yield return new object[] { null, -1, null, null, null, (DataGridViewDataErrorContexts)(DataGridViewDataErrorContexts.Formatting - 1), null };
            yield return new object[] { null, 0, null, null, null, (DataGridViewDataErrorContexts)(DataGridViewDataErrorContexts.Formatting - 1), null };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetFormattedValue_TestData))]
        public void DataGridViewCell_GetFormattedValue_Invoke_ReturnsExpected(object value, int rowIndex, DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context, object expected)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(expected, cell.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetFormattedValue_TestData))]
        public void DataGridViewCell_GetFormattedValue_InvokeWithRow_ReturnsExpected(object value, int rowIndex, DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context, object expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(expected, cell.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetFormattedValue_TestData))]
        public void DataGridViewCell_GetFormattedValue_InvokeWithColumn_ReturnsExpected(object value, int rowIndex, DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context, object expected)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(expected, cell.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        public void DataGridViewCell_GetInheritedContextMenuStrip_Invoke_ReturnsExpected(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Null(cell.GetInheritedContextMenuStrip(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeWithMenu_ReturnsExpected(int rowIndex)
        {
            using var cellMenu = new ContextMenuStrip();
            using var cell = new SubDataGridViewCell
            {
                ContextMenuStrip = cellMenu
            };
            Assert.Same(cellMenu, cell.GetInheritedContextMenuStrip(rowIndex));
        }

        public static IEnumerable<object[]> GetInheritedContextMenuStrip_TestData()
        {
            yield return new object[] { -2, null };
            yield return new object[] { -2, new ContextMenuStrip() };
            yield return new object[] { -1, null };
            yield return new object[] { -1, new ContextMenuStrip() };
            yield return new object[] { 0, null };
            yield return new object[] { 0, new ContextMenuStrip() };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedContextMenuStrip_TestData))]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeWithRow_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
        {
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = menu
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Same(menu, cell.GetInheritedContextMenuStrip(rowIndex));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedContextMenuStrip_TestData))]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeWithMenuWithRow_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
        {
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = menu
            };
            using var cellMenu = new ContextMenuStrip();
            using var cell = new SubDataGridViewCell
            {
                ContextMenuStrip = cellMenu
            };
            row.Cells.Add(cell);
            Assert.Same(cellMenu, cell.GetInheritedContextMenuStrip(rowIndex));
        }

        public static IEnumerable<object[]> ContextMenuStrip_GetWithDataGridView_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContextMenuStrip() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeWithDataGridView_ReturnsExpected(ContextMenuStrip menu)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ContextMenuStrip = menu
            };
            control.Columns.Add(column);
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;

            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Same(menu, cell.GetInheritedContextMenuStrip(0));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeWithMenuWithDataGridView_ReturnsExpected(ContextMenuStrip menu)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ContextMenuStrip = menu
            };
            control.Columns.Add(column);
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;

            using var cellMenu = new ContextMenuStrip();
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ContextMenuStrip = cellMenu;
            Assert.Same(cellMenu, cell.GetInheritedContextMenuStrip(0));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeShared_ReturnsExpected(ContextMenuStrip menu)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ContextMenuStrip = menu
            };
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;
            Assert.Same(menu, cell.GetInheritedContextMenuStrip(0));
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeWithDataGridViewVirtualMode_CallsCellContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                VirtualMode = true
            };
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewCellContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.CellContextMenuStripNeeded += handler;

            Assert.Same(menu2, cell.GetInheritedContextMenuStrip(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.CellContextMenuStripNeeded -= handler;
            Assert.Same(menu1, cell.GetInheritedContextMenuStrip(0));
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvokeWithDataGridViewDataSource_CallsCellContextMenuStripNeeded()
        {
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridView
            {
                ColumnCount = 1,
                RowCount = 1,
                DataSource = new[] { new { Name = "Name" } }
            };
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ContextMenuStrip = menu1;

            int callCount = 0;
            DataGridViewCellContextMenuStripNeededEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Same(menu1, e.ContextMenuStrip);
                e.ContextMenuStrip = menu2;
            };
            control.CellContextMenuStripNeeded += handler;

            Assert.Same(menu2, cell.GetInheritedContextMenuStrip(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.CellContextMenuStripNeeded -= handler;
            Assert.Same(menu1, cell.GetInheritedContextMenuStrip(0));
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedContextMenuStrip(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetInheritedContextMenuStrip_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedContextMenuStrip(rowIndex));
        }
        [WinFormsFact]
        public void DataGridViewCell_GetInheritedState_Invoke_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(DataGridViewElementStates.ResizableSet, cell.GetInheritedState(-1));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedState_InvokeWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(-1));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.False, DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewElementStates.ResizableSet)]
        public void DataGridViewCell_GetInheritedState_InvokeWithRowCustomState_ReturnsExpected(DataGridViewTriState resizable, DataGridViewElementStates expected)
        {
            using var row = new DataGridViewRow
            {
                Frozen = true,
                ReadOnly = true,
                Visible = false,
                Resizable = resizable
            };
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | expected, cell.GetInheritedState(-1));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedState_InvokeWithDataGrid_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);

            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Equal(DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(0));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetInheritedState_DataGridViewCustomState_TestData()
        {
            // Frozen.
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };

            // Visible.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };

            // Resizable.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_DataGridViewCustomState_TestData))]
        public void DataGridViewCell_GetInheritedState_InvokeWithDataGridCustomState_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            row.Resizable = rowResizable;
            row.Frozen = rowFrozen;
            row.Visible = rowVisible;
            column.Resizable = columnResizable;
            column.Frozen = columnFrozen;
            column.Visible = columnVisible;
            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedState_InvokeWithDataGridWithHandle_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Equal(DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed, cell.GetInheritedState(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetInheritedState_DataGridViewCustomStateWithHandle_TestData()
        {
            // Frozen.
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };

            // Visible.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };

            // Resizable.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Selected };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_DataGridViewCustomStateWithHandle_TestData))]
        public void DataGridViewCell_GetInheritedState_InvokeWithDataGridCustomStateWithHandle_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            row.Resizable = rowResizable;
            row.Frozen = rowFrozen;
            row.Visible = rowVisible;
            column.Resizable = columnResizable;
            column.Frozen = columnFrozen;
            column.Visible = columnVisible;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedState_InvokeWithDataGridCustomStateRow_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.ReadOnly = true;
            row.Frozen = true;
            row.Resizable = DataGridViewTriState.True;
            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedState_InvokeSharedRow_ThrowsArgumentOutOfRangeException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedState(-1));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewCell_GetInheritedState_InvalidRowIndexNoDataGridView_ThrowsArgumentException(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentException>(null, () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewCell_GetInheritedState_InvalidRowIndexWithRow_ThrowsArgumentException(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Throws<ArgumentException>(null, () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewCell_GetInheritedState_InvalidRowIndexWithColumn_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetInheritedState_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetInheritedState_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedStyle_NoDataGridView_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.GetInheritedStyle(new DataGridViewCellStyle(), -1, true));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedStyle_InvokeWithoutDataGridView_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.GetInheritedStyle(new DataGridViewCellStyle(), -1, true));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedStyle_InvokeWithRow_ThrowsInvalidOperationException()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Throws<InvalidOperationException>(() => cell.GetInheritedStyle(new DataGridViewCellStyle(), -1, true));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetInheritedStyle_InvokeWithColumn_ThrowsInvalidOperationException()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<InvalidOperationException>(() => cell.GetInheritedStyle(new DataGridViewCellStyle(), -1, true));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetInheritedStyle_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedStyle(new DataGridViewCellStyle(), rowIndex, true));
        }

        [StaFact]
        public void DataGridViewCell_GetNeighboringToolsRectangles_ReturnsCorrectRectangles()
        {
            DataGridView control = new DataGridView();
            control.Size = new Size(600, 200);
            control.CreateControl();

            DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn column4 = new DataGridViewTextBoxColumn();

            control.Columns.Add(column1);
            control.Columns.Add(column2);
            control.Columns.Add(column3);
            control.Columns.Add(column4);

            control.Rows.Add();
            control.Rows.Add();
            control.Rows.Add();
            control.Rows.Add();

            control.Rows[0].Cells[1].Value = "Text";
            control.Rows[1].Cells[2].Value = "Text";
            control.Rows[1].Cells[3].Value = "Text";
            control.Rows[2].Cells[0].Value = "Text";
            control.Rows[2].Cells[1].Value = "Text";
            control.Rows[2].Cells[3].Value = "Text";
            control.Rows[3].Cells[1].Value = "Text";
            control.Rows[3].Cells[2].Value = "Text";

            IList<Rectangle> neighbors00 = ((IKeyboardToolTip)control.Rows[0].Cells[0]).GetNeighboringToolsRectangles();
            Assert.True(neighbors00.Contains(control.Rows[0].Cells[1].AccessibilityObject.Bounds));
            Assert.False(neighbors00.Contains(control.Rows[1].Cells[1].AccessibilityObject.Bounds));
            Assert.False(neighbors00.Contains(control.Rows[1].Cells[0].AccessibilityObject.Bounds));

            IList<Rectangle> neighbors21 = ((IKeyboardToolTip)control.Rows[2].Cells[1]).GetNeighboringToolsRectangles();
            Assert.True(neighbors21.Contains(control.Rows[1].Cells[2].AccessibilityObject.Bounds));
            Assert.True(neighbors21.Contains(control.Rows[2].Cells[0].AccessibilityObject.Bounds));
            Assert.True(neighbors21.Contains(control.Rows[2].Cells[1].AccessibilityObject.Bounds));
            Assert.True(neighbors21.Contains(control.Rows[3].Cells[1].AccessibilityObject.Bounds));
            Assert.False(neighbors21.Contains(control.Rows[1].Cells[1].AccessibilityObject.Bounds));

            IList<Rectangle> neighbors33 = ((IKeyboardToolTip)control.Rows[3].Cells[3]).GetNeighboringToolsRectangles();
            Assert.True(neighbors33.Contains(control.Rows[2].Cells[3].AccessibilityObject.Bounds));
            Assert.True(neighbors33.Contains(control.Rows[3].Cells[2].AccessibilityObject.Bounds));
            Assert.False(neighbors33.Contains(control.Rows[2].Cells[2].AccessibilityObject.Bounds));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetPreferredSize_Invoke_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(new Size(-1, -1), cell.GetPreferredSize(null, null, -1, Size.Empty));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetSize_Invoke_ReturnsExpected(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal(new Size(-1, -1), cell.GetSize(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetSize_InvokeWithRow_ReturnsExpected(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal(new Size(-1, -1), cell.GetSize(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetSize_InvokeWithDataGridView_ReturnsExpected(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Height = 11;
            SubDataGridViewCell cell = (SubDataGridViewCell)row.Cells[0];
            Assert.Equal(new Size(10, 11), cell.GetSize(rowIndex));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetSize_InvokeShared_ReturnsExpected(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Equal(new Size(10, Control.DefaultFont.Height + 9), cell.GetSize(rowIndex));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewCell_GetSize_InvalidRowIndexWithColumn_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetSize(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetSize_InvalidRowIndexWithDataGridView_ThrowsInvalidOperationException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.GetSize(-1));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetSize_InvalidRowIndexShared_ThrowsInvalidOperationException()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.GetSize(-1));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetValue_Invoke_ReturnsExpected(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.Null(cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetValue_InvokeWithValue_ReturnsExpected(int rowIndex)
        {
            var value = new object();
            using var cell = new SubDataGridViewCell
            {
                Value = value
            };
            Assert.Same(value, cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetValue_InvokeWithRow_ReturnsExpected(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Null(cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_GetValue_InvokeWithValueWithRow_ReturnsExpected(int rowIndex)
        {
            var value = new object();
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell
            {
                Value = value
            };
            row.Cells.Add(cell);
            Assert.Same(value, cell.GetValue(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetValue_InvokeWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Null(cell.GetValue(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetValue_InvokeWithValueWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            var value = new object();
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.Value = value;
            Assert.Same(value, cell.GetValue(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetValue_InvokeShared_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Null(cell.GetValue(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetValue_InvokeSharedWithValue_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            var value = new object();
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.Value = value);
            Assert.Null(cell.GetValue(0));
        }

        [WinFormsFact]
        public void DataGridViewCell_GetValue_InvokeWithDataGridViewVirtualMode_CallsCellValueNeeded()
        {
            var value1 = new object();
            var value2 = new object();
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                VirtualMode = true
            };
            control.Columns.Add(column);
            control.Rows.Add();
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.Value = value1;

            int callCount = 0;
            DataGridViewCellValueEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Null(e.Value);
                e.Value = value2;
            };
            control.CellValueNeeded += handler;

            Assert.Same(value2, cell.GetValue(0));
            Assert.Equal(1, callCount);

            // Remove the handler.
            control.CellValueNeeded -= handler;
            Assert.Null(cell.GetValue(0));
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_GetValue_InvokeWithDataGridViewNewRowVirtualMode_CallsCellValueNeeded()
        {
            var value1 = new object();
            var value2 = new object();
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                VirtualMode = true
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.Value = value1;

            int callCount = 0;
            DataGridViewCellValueEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Null(e.Value);
                e.Value = value2;
            };
            control.CellValueNeeded += handler;

            Assert.Null(cell.GetValue(0));
            Assert.Equal(0, callCount);

            // Remove the handler.
            control.CellValueNeeded -= handler;
            Assert.Null(cell.GetValue(0));
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewCell_GetValue_InvokeWithDataGridViewNewRowDataSource_CallsCellValueNeeded()
        {
            var value1 = new object();
            var value2 = new object();
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                DataSource = new[] { new { Name = "Name" } }
            };
            control.Columns.Add(column);
            using var form = new Form();
            form.Controls.Add(control);
            Assert.NotNull(control.BindingContext);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.Value = value1;

            int callCount = 0;
            DataGridViewCellValueEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(control, sender);
                Assert.Equal(0, e.RowIndex);
                Assert.Null(e.Value);
                e.Value = value2;
            };
            control.CellValueNeeded += handler;

            Assert.Same(value1, cell.GetValue(0));
            Assert.Equal(0, callCount);

            // Remove the handler.
            control.CellValueNeeded -= handler;
            Assert.Same(value1, cell.GetValue(0));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetValue_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_GetValue_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewCell_InitializeEditingDataGridViewCell_SetNoDataGridView_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.InitializeEditingControl(-1, null, null));
        }

        [WinFormsFact]
        public void DataGridViewCell_KeyDownUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyDownUnsharesRow(null, -1));
        }

        [WinFormsFact]
        public void DataGridViewCell_KeyEntersEditMode_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyEntersEditMode(null));
        }

        [WinFormsFact]
        public void DataGridViewCell_KeyPressUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyPressUnsharesRow(null, -1));
        }

        [WinFormsFact]
        public void DataGridViewCell_KeyUpUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.KeyUpUnsharesRow(null, -1));
        }

        [WinFormsFact]
        public void DataGridViewCell_LeaveUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.LeaveUnsharesRow(-1, true));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextHeight_NullOrEmptyText_ReturnsExpected(string text)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            int height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 10, TextFormatFlags.Default);
            Assert.Equal(0, height);

            height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 10, TextFormatFlags.Default, out bool widthTruncated);
            Assert.Equal(0, height);
            Assert.False(widthTruncated);
        }

        [WinFormsTheory]
        [InlineData("a", false)]
        [InlineData("truncate_me", true)]
        public void DataGridViewCell_MeasureTextHeight_NonEmptyText_ReturnsExpected(string text, bool expectedWidthTruncated)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            int height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 20, TextFormatFlags.Default);
            Assert.NotEqual(0, height);

            height = DataGridViewCell.MeasureTextHeight(graphics, text, SystemFonts.DefaultFont, 20, TextFormatFlags.Default, out bool widthTruncated);
            Assert.NotEqual(0, height);
            Assert.Equal(expectedWidthTruncated, widthTruncated);
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextHeight_NullGraphics_ThrowsArgumentNullException()
        {
            bool widthTruncated = true;
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextHeight(null, "text", SystemFonts.DefaultFont, 10, TextFormatFlags.Default));
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextHeight(null, "text", SystemFonts.DefaultFont, 10, TextFormatFlags.Default, out widthTruncated));
            Assert.True(widthTruncated);
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextHeight_NullFont_ThrowsArgumentNullException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            bool widthTruncated = true;
            Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextHeight(graphics, "text", null, 10, TextFormatFlags.Default));
            Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextHeight(graphics, "text", null, 10, TextFormatFlags.Default, out widthTruncated));
            Assert.True(widthTruncated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(-1)]
        public void DataGridViewCell_MeasureTextHeight_InvalidMaxWidth_ThrowsArgumentOutOfRangeException(int maxWidth)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            bool widthTruncated = true;
            Assert.Throws<ArgumentOutOfRangeException>("maxWidth", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, maxWidth, TextFormatFlags.Default));
            Assert.Throws<ArgumentOutOfRangeException>("maxWidth", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, maxWidth, TextFormatFlags.Default, out widthTruncated));
            Assert.True(widthTruncated);
        }

        [WinFormsTheory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextHeight_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            bool widthTruncated = true;
            Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, 10, flags));
            Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextHeight(graphics, "text", SystemFonts.DefaultFont, 10, flags, out widthTruncated));
            Assert.True(widthTruncated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextPreferredSize_NullOrEmptyText_ReturnsExpected(string text)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Equal(Size.Empty, DataGridViewCell.MeasureTextPreferredSize(graphics, text, SystemFonts.DefaultFont, 0.2f, TextFormatFlags.Default));
        }

        [WinFormsTheory]
        [InlineData("a", TextFormatFlags.Default)]
        [InlineData("truncate_me", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        public void DataGridViewCell_MeasureTextPreferredSize_NonEmptyText_ReturnsExpected(string text, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.NotEqual(Size.Empty, DataGridViewCell.MeasureTextPreferredSize(graphics, text, SystemFonts.DefaultFont, 0.2f, flags));
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextPreferredSize_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextPreferredSize(null, "text", SystemFonts.DefaultFont, 0.2f, TextFormatFlags.Default));
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextPreferredSize_NullFont_ThrowsArgumentNullException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextPreferredSize(graphics, "text", null, 0.2f, TextFormatFlags.Default));
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(-1)]
        public void DataGridViewCell_MeasureTextPreferredSize_InvalidMaxHeight_ThrowsArgumentOutOfRangeException(float maxRatio)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<ArgumentOutOfRangeException>("maxRatio", () => DataGridViewCell.MeasureTextPreferredSize(graphics, "text", SystemFonts.DefaultFont, maxRatio, TextFormatFlags.Default));
        }

        [WinFormsTheory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextPreferredSize_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextPreferredSize(graphics, "text", SystemFonts.DefaultFont, 0.2f, flags));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextSize_NullOrEmptyText_ReturnsExpected(string text)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            {
                Assert.Equal(Size.Empty, DataGridViewCell.MeasureTextSize(graphics, text, SystemFonts.DefaultFont, TextFormatFlags.Default));
            }
        }

        [WinFormsTheory]
        [InlineData("a", TextFormatFlags.Default)]
        [InlineData("truncate_me", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        public void DataGridViewCell_MeasureTextSize_NonEmptyText_ReturnsExpected(string text, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.NotEqual(Size.Empty, DataGridViewCell.MeasureTextSize(graphics, text, SystemFonts.DefaultFont, flags));
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextSize_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextSize(null, "text", SystemFonts.DefaultFont, TextFormatFlags.Default));
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextSize_NullFont_ThrowsArgumentNullException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextSize(graphics, "text", null, TextFormatFlags.Default));
        }

        [WinFormsTheory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextSize_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextSize(graphics, "text", SystemFonts.DefaultFont, flags));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_MeasureTextWidth_NullOrEmptyText_ReturnsExpected(string text)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Equal(0, DataGridViewCell.MeasureTextWidth(graphics, text, SystemFonts.DefaultFont, 10, TextFormatFlags.Default));
        }

        [WinFormsTheory]
        [InlineData("a", TextFormatFlags.Default)]
        [InlineData("truncate_me", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\nnew\rn\nnew", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default)]
        [InlineData("truncate_me\r\truncate_me_again", TextFormatFlags.Default | TextFormatFlags.SingleLine)]
        public void DataGridViewCell_MeasureTextWidth_NonEmptyText_ReturnsExpected(string text, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.NotEqual(0, DataGridViewCell.MeasureTextWidth(graphics, text, SystemFonts.DefaultFont, 10, flags));
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextWidth_NullGraphics_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("graphics", () => DataGridViewCell.MeasureTextWidth(null, "text", SystemFonts.DefaultFont, 10, TextFormatFlags.Default));
        }

        [WinFormsFact]
        public void DataGridViewCell_MeasureTextWidth_NullFont_ThrowsArgumentNullException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<ArgumentNullException>("font", () => DataGridViewCell.MeasureTextWidth(graphics, "text", null, 10, TextFormatFlags.Default));
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(-1)]
        public void DataGridViewCell_MeasureTextWidth_InvalidMaxHeight_ThrowsArgumentOutOfRangeException(int maxHeight)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<ArgumentOutOfRangeException>("maxHeight", () => DataGridViewCell.MeasureTextWidth(graphics, "text", SystemFonts.DefaultFont, maxHeight, TextFormatFlags.Default));
        }

        [WinFormsTheory]
        [InlineData((TextFormatFlags)(-1))]
        public void DataGridViewCell_MeasureTextWidth_InvalidFlags_ThrowsInvalidEnumArgumentException(TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<InvalidEnumArgumentException>("flags", () => DataGridViewCell.MeasureTextWidth(graphics, "text", SystemFonts.DefaultFont, 10, flags));
        }

        [WinFormsFact]
        public void DataGridViewCell_MouseClickUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseClickUnsharesRow(null));
        }

        [WinFormsFact]
        public void DataGridViewCell_MouseDoubleClickUnsharesRow_Invoke_ReturnsFalse()
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseDoubleClickUnsharesRow(null));
        }

        public static IEnumerable<object[]> DataGridViewCellMouseEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_MouseDownUnsharesRow_Invoke_ReturnsFalse(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseDownUnsharesRow(e));
        }

        public static IEnumerable<object[]> DataGridViewCellMouseEventArgs_WithDataGridView_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
                yield return new object[] { enableHeadersVisualStyles, null };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_MouseDownUnsharesRow_InvokeWithDataGridView_ReturnsFalse(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.False(cell.MouseDownUnsharesRow(e));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_MouseEnterUnsharesRow_Invoke_ReturnsFalse(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseEnterUnsharesRow(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(true, -2)]
        [InlineData(true, -1)]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(false, -2)]
        [InlineData(false, -1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void DataGridViewCell_MouseEnterUnsharesRow_InvokeWithDataGridView_ReturnsFalse(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.False(cell.MouseEnterUnsharesRow(rowIndex));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_MouseLeaveUnsharesRow_Invoke_ReturnsFalse(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseLeaveUnsharesRow(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(true, -2)]
        [InlineData(true, -1)]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(false, -2)]
        [InlineData(false, -1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void DataGridViewCell_MouseLeaveUnsharesRow_InvokeWithDataGridView_ReturnsFalse(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.False(cell.MouseLeaveUnsharesRow(rowIndex));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_MouseMoveUnsharesRow_Invoke_ReturnsFalse(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseMoveUnsharesRow(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_MouseMoveUnsharesRow_InvokeWithDataGridView_ReturnsFalse(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.False(cell.MouseMoveUnsharesRow(e));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_MouseUpUnsharesRow_Invoke_ReturnsFalse(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            Assert.False(cell.MouseUpUnsharesRow(e));
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_MouseUpUnsharesRow_InvokeWithDataGridView_ReturnsFalse(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.False(cell.MouseUpUnsharesRow(e));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_OnClick_Invoke_Nop()
        {
            using var cell = new SubDataGridViewCell();
            cell.OnClick(null);
        }

        public static IEnumerable<object[]> DataGridViewCellEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new DataGridViewCellEventArgs(-1, -1) };
            yield return new object[] { new DataGridViewCellEventArgs(0, -1) };
            yield return new object[] { new DataGridViewCellEventArgs(-1, 0) };
            yield return new object[] { new DataGridViewCellEventArgs(0, 0) };
            yield return new object[] { new DataGridViewCellEventArgs(1, 0) };
            yield return new object[] { new DataGridViewCellEventArgs(0, 1) };
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
        public void DataGridViewCell_OnContentClick_Invoke_Nop(DataGridViewCellEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnContentClick(e);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
        public void DataGridViewCell_OnContentDoubleClick_Invoke_Nop(DataGridViewCellEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnContentDoubleClick(e);
        }

        [WinFormsFact]
        public void DataGridViewCell_OnDataGridViewChanged_InvokeWithoutStyle_Nop()
        {
            using var cell = new SubDataGridViewCell();
            cell.OnDataGridViewChanged();
            Assert.Equal(DataGridViewCellStyleScopes.Cell, cell.Style.Scope);
        }

        [WinFormsFact]
        public void DataGridViewCell_OnDataGridViewChanged_InvokeWithStyle_SetsScopeToNone()
        {
            using var cell = new SubDataGridViewCell
            {
                Style = new DataGridViewCellStyle()
            };
            Assert.Equal(DataGridViewCellStyleScopes.Cell, cell.Style.Scope);
            cell.OnDataGridViewChanged();
            Assert.Equal(DataGridViewCellStyleScopes.None, cell.Style.Scope);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellEventArgs_TestData))]
        public void DataGridViewCell_OnDoubleClick_Invoke_Nop(DataGridViewCellEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnDoubleClick(e);
        }

        [WinFormsFact]
        public void DataGridViewCell_OnEnter_Invoke_Nop()
        {
            using var cell = new SubDataGridViewCell();
            cell.OnEnter(-1, true);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void DataGridViewCell_OnKeyDown_Invoke_Nop(KeyEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnKeyDown(e, -1);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void DataGridViewCell_OnKeyPress_Invoke_Nop(KeyPressEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnKeyPress(e, -1);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void DataGridViewCell_OnKeyUp_Invoke_Nop(KeyEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnKeyUp(e, -1);
        }

        [WinFormsFact]
        public void DataGridViewCell_OnLeave_Invoke_Nop()
        {
            using var cell = new SubDataGridViewCell();
            cell.OnLeave(-1, true);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_OnMouseClick_Invoke_Nop(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnMouseClick(e);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_OnMouseClick_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.OnMouseClick(e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_OnMouseDoubleClick_Invoke_Nop(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnMouseDoubleClick(e);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_OnMouseDoubleClick_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.OnMouseDoubleClick(e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_OnMouseDown_Invoke_Nop(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnMouseDown(e);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_OnMouseDown_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.OnMouseDown(e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_OnMouseEnter_Invoke_Nop(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnMouseEnter(rowIndex);
        }

        [WinFormsTheory]
        [InlineData(true, -2)]
        [InlineData(true, -1)]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(false, -2)]
        [InlineData(false, -1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void DataGridViewCell_OnMouseEnter_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.OnMouseEnter(rowIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewCell_OnMouseLeave_Invoke_Nop(int rowIndex)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnMouseLeave(rowIndex);
        }

        [WinFormsTheory]
        [InlineData(true, -2)]
        [InlineData(true, -1)]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(false, -2)]
        [InlineData(false, -1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void DataGridViewCell_OnMouseLeave_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.OnMouseLeave(rowIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_OnMouseMove_Invoke_Nop(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnMouseMove(e);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_OnMouseMove_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.OnMouseMove(e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_TestData))]
        public void DataGridViewCell_OnMouseUp_Invoke_Nop(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewCell();
            cell.OnMouseUp(e);
        }

        [WinFormsTheory]
        [MemberData(nameof(DataGridViewCellMouseEventArgs_WithDataGridView_TestData))]
        public void DataGridViewCell_OnMouseUp_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            cell.OnMouseUp(e);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewCell_Paint_Invoke_Nop()
        {
            using var cell = new SubDataGridViewCell();
            cell.Paint(null, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, null, null, null, null, null, DataGridViewPaintParts.All);
        }

        [WinFormsFact]
        public void DataGridViewCell_Paint_InvokeWithGraphics_Nop()
        {
            using var cell = new SubDataGridViewCell();
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            cell.Paint(graphics, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, null, null, null, null, null, DataGridViewPaintParts.All);
        }

        [WinFormsFact]
        public void DataGridViewCell_PaintBorder_NoDataGridView_Nop()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var cell = new SubDataGridViewCell();
            cell.PaintBorder(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle());
        }

        [WinFormsFact]
        public void DataGridViewCell_PaintBorder_NullGraphics_ThrowsArgumentNullException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("graphics", () => cell.PaintBorder(null, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), new DataGridViewAdvancedBorderStyle()));
        }

        [WinFormsFact]
        public void DataGridViewCell_PaintBorder_NullCellStyle_ThrowsArgumentNullException()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("cellStyle", () => cell.PaintBorder(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), null, new DataGridViewAdvancedBorderStyle()));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DataGridViewCell_PaintErrorIcon_InvokeNullOrEmptyText_Success(string errorText)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var cell = new SubDataGridViewCell();
            cell.PaintErrorIcon(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 20, 19), errorText);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DataGridViewCell_PaintErrorIcon_NoDataGridView_Nop(string errorText)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var cell = new SubDataGridViewCell();
            cell.PaintErrorIcon(graphics, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 20, 19), errorText);
        }

        [WinFormsTheory]
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
            using var cell = new SubDataGridViewCell();
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

        [WinFormsTheory]
        [MemberData(nameof(ParseFormattedValue_TestData))]
        public void DataGridViewCell_ParseFormattedValue_Invoke_ReturnsExpected(Type valueType, Type formattedValueType, object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter, object expected)
        {
            using var cell = new CustomFormattedValueType
            {
                ValueType = valueType,
                FormattedValueTypeResult = formattedValueType
            };
            Assert.Equal(expected, cell.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter));

            // Call same.
            Assert.Equal(expected, cell.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter));
        }

        [WinFormsFact]
        public void DataGridViewCell_ParseFormattedValue_NullCellStyle_ThrowsArgumentNullException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<ArgumentNullException>("cellStyle", () => cell.ParseFormattedValue(1, null, new Int32Converter(), new Int32Converter()));
        }

        [WinFormsFact]
        public void DataGridViewCell_ParseFormattedValue_NullValueTypeAndFormattedValueType_ThrowsFormatException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(1, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter()));
        }

        [WinFormsFact]
        public void DataGridViewCell_ParseFormattedValue_NullValueType_ThrowsFormatException()
        {
            using var cell = new CustomFormattedValueType { FormattedValueTypeResult = typeof(int) };
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(1, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter()));
        }

        [WinFormsFact]
        public void DataGridViewCell_ParseFormattedValue_NullFormattedValueType_ThrowsFormatException()
        {
            using var cell = new CustomFormattedValueType { ValueType = typeof(int) };
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(1, new DataGridViewCellStyle(), new Int32Converter(), new Int32Converter()));
        }

        public static IEnumerable<object[]> ParseFormattedValue_CantConvert_TestData()
        {
            yield return new object[] { typeof(DataGridViewCellTests), typeof(int), 123, new DataGridViewCellStyle(), null, null };
            yield return new object[] { typeof(DataGridViewCell), typeof(DataGridViewCellTests), new DataGridViewCellTests(), new DataGridViewCellStyle(), null, null };
            yield return new object[] { typeof(int), typeof(string), "Invalid", new DataGridViewCellStyle(), null, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(ParseFormattedValue_CantConvert_TestData))]
        public void DataGridViewCell_ParseFormattedValue_CantConvert_ThrowsFormatException(Type valueType, Type formattedValueType, object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            using var cell = new CustomFormattedValueType
            {
                ValueType = valueType,
                FormattedValueTypeResult = formattedValueType
            };
            Assert.Throws<FormatException>(() => cell.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData(1)]
        public void DataGridViewCell_ParseFormattedValue_InvalidFormattedValue_ThrowsArgumentException(object formattedValue)
        {
            using var cell = new CustomFormattedValueType
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

        [WinFormsFact]
        public void DataGridViewCell_PositionEditingDataGridViewCell_NoDataGridView_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.PositionEditingControl(true, true, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), true, true, true, true));
        }

        [WinFormsFact]
        public void DataGridViewCell_PositionEditingPanel_NoDataGridView_ThrowsInvalidOperationException()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Throws<InvalidOperationException>(() => cell.PositionEditingPanel(new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), new DataGridViewCellStyle(), true, true, true, true));
        }

        public static IEnumerable<object[]> SetValue_TestData()
        {
            yield return new object[] { -2, null };
            yield return new object[] { -2, "value" };
            yield return new object[] { -1, null };
            yield return new object[] { -1, "value" };
            yield return new object[] { 0, null };
            yield return new object[] { 0, "value" };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetValue_TestData))]
        public void DataGridViewCell_SetValue_Invoke_GetReturnsExpected(int rowIndex, object value)
        {
            using var cell = new SubDataGridViewCell();
            Assert.True(cell.SetValue(rowIndex, value));
            Assert.Equal(value, cell.GetValue(rowIndex));

            // Set same.
            Assert.True(cell.SetValue(rowIndex, value));
            Assert.Equal(value, cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [MemberData(nameof(SetValue_TestData))]
        public void DataGridViewCell_SetValue_InvokeWithRow_GetReturnsExpected(int rowIndex, object value)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);

            Assert.True(cell.SetValue(rowIndex, value));
            Assert.Equal(value, cell.GetValue(rowIndex));

            // Set same.
            Assert.True(cell.SetValue(rowIndex, value));
            Assert.Equal(value, cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("value")]
        public void DataGridViewCell_SetValue_InvokeWithColumn_GetReturnsExpected(object value)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            Assert.True(cell.SetValue(-1, value));
            Assert.Equal(value, cell.GetValue(-1));

            // Set same.
            Assert.True(cell.SetValue(-1, value));
            Assert.Equal(value, cell.GetValue(-1));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("value")]
        public void DataGridViewCell_SetValue_InvokeWithDataGridView_GetReturnsExpected(object value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];

            Assert.True(cell.SetValue(0, value));
            Assert.Equal(value, cell.GetValue(0));

            // Set same.
            Assert.True(cell.SetValue(0, value));
            Assert.Equal(value, cell.GetValue(0));
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("value")]
        public void DataGridViewCell_SetValue_InvokeShared_GetReturnsExpected(object value)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];

            Assert.True(cell.SetValue(0, value));
            Assert.Equal(value, cell.GetValue(0));

            // Set same.
            Assert.True(cell.SetValue(0, value));
            Assert.Equal(value, cell.GetValue(0));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewCell_SetValue_InvalidRowIndexWithColumn_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.SetValue(rowIndex, "value"));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_SetValue_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.SetValue(rowIndex, "value"));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewCell_SetValue_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewCell cell = (SubDataGridViewCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.SetValue(rowIndex, "value"));
        }

        [WinFormsFact]
        public void DataGridViewCell_ToString_Invoke_ReturnsExpected()
        {
            using var cell = new SubDataGridViewCell();
            Assert.Equal("DataGridViewCell { ColumnIndex=-1, RowIndex=-1 }", cell.ToString());
        }

        [WinFormsFact]
        public void DataGridViewCell_ToString_InvokeWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewCell();
            row.Cells.Add(cell);
            Assert.Equal("DataGridViewCell { ColumnIndex=-1, RowIndex=-1 }", cell.ToString());
        }

        [WinFormsFact]
        public void DataGridViewCell_ToString_InvokeWithColumn_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal("DataGridViewColumnHeaderCell { ColumnIndex=-1 }", cell.ToString());
        }

        [WinFormsFact]
        public void DataGridViewCell_ToString_InvokeWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Equal("DataGridViewCell { ColumnIndex=0, RowIndex=0 }", cell.ToString());
        }

        [WinFormsFact]
        public void DataGridViewCell_OnContentClick_InvokeInternalRaiseAutomationNotification()
        {
            using var cellTemplate = new SubDataGridViewCheckBoxCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };

            using var dataGridView = new DataGridView();
            dataGridView.Columns.Add(column);
            SubDataGridViewCheckBoxCell cell = (SubDataGridViewCheckBoxCell)dataGridView.Rows[0].Cells[0];
            var mockAccessibleObject = new Mock<AccessibleObject>(MockBehavior.Strict);
            mockAccessibleObject
                .Setup(a => a.InternalRaiseAutomationNotification(
                    It.IsAny<AutomationNotificationKind>(),
                    It.IsAny<AutomationNotificationProcessing>(),
                    It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            var cellName = "TestCellName";

            mockAccessibleObject
                .Setup(a => a.Name)
                .Returns(cellName);

            mockAccessibleObject
                .Setup(a => a.DoDefaultAction()).CallBase();

            cell.MockAccessibleObject = mockAccessibleObject.Object;
            cell.Value = false;
            dataGridView.CurrentCell = cell;

            // Checkbox is checked
            dataGridView.BeginEdit(false);
            cell.MouseClick(new DataGridViewCellMouseEventArgs(0, 0, 10, 10, new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0)));
            mockAccessibleObject.Verify(a => a.InternalRaiseAutomationNotification(AutomationNotificationKind.Other,
                AutomationNotificationProcessing.MostRecent,
                string.Format(SR.DataGridViewCheckBoxCellCheckedStateDescription, cellName)),
                Times.Once());

            // Checkbox is unchecked
            dataGridView.BeginEdit(false);
            cell.MouseClick(new DataGridViewCellMouseEventArgs(0, 0, 10, 10, new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0)));
            mockAccessibleObject.Verify(a => a.InternalRaiseAutomationNotification(AutomationNotificationKind.Other,
                AutomationNotificationProcessing.MostRecent,
                string.Format(SR.DataGridViewCheckBoxCellUncheckedStateDescription, cellName)),
                Times.Once());

            // Checkbox is checked
            dataGridView.BeginEdit(false);
            cell.OnKeyClick(new KeyEventArgs(Keys.Space), 0);
            mockAccessibleObject.Verify(a => a.InternalRaiseAutomationNotification(AutomationNotificationKind.Other,
                AutomationNotificationProcessing.MostRecent,
                string.Format(SR.DataGridViewCheckBoxCellCheckedStateDescription, cellName)),
                Times.Exactly(2));

            // Checkbox is unchecked
            dataGridView.BeginEdit(false);
            cell.OnKeyClick(new KeyEventArgs(Keys.Space), 0);
            mockAccessibleObject.Verify(a => a.InternalRaiseAutomationNotification(AutomationNotificationKind.Other,
                AutomationNotificationProcessing.MostRecent,
                string.Format(SR.DataGridViewCheckBoxCellUncheckedStateDescription, cellName)),
                Times.Exactly(2));
        }

        private class SubDataGridViewCheckBoxCell: DataGridViewCheckBoxCell
        {
            public SubDataGridViewCheckBoxCell()
            {
            }

            public AccessibleObject MockAccessibleObject;

            protected override AccessibleObject CreateAccessibilityInstance() => MockAccessibleObject;

            public void MouseClick(DataGridViewCellMouseEventArgs e) => base.OnMouseUpInternal(e);

            public void OnKeyClick(KeyEventArgs e, int rowIndex) => base.OnKeyUp(e, rowIndex);
        }

        private class SubDataGridViewColumnHeaderCell : DataGridViewColumnHeaderCell
        {
            public new Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle) => base.BorderWidths(advancedBorderStyle);

            public new object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
            {
                return base.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format);
            }

            public new string GetErrorText(int rowIndex) => base.GetErrorText(rowIndex);

            public new object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
            {
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }

            public new Size GetSize(int rowIndex) => base.GetSize(rowIndex);

            public new object GetValue(int rowIndex) => base.GetValue(rowIndex);

            public new bool SetValue(int rowIndex, object value) => base.SetValue(rowIndex, value);
        }

        private class CustomStateDataGridViewCell : DataGridViewCell
        {
            public DataGridViewElementStates StateResult { get; set; }

            public override DataGridViewElementStates State => StateResult;
        }

        private class SubDataGridViewCell : DataGridViewCell
        {
            public new Rectangle BorderWidths(DataGridViewAdvancedBorderStyle advancedBorderStyle) => base.BorderWidths(advancedBorderStyle);

            public new bool ClickUnsharesRow(DataGridViewCellEventArgs e) => base.ClickUnsharesRow(e);

            public new bool ContentClickUnsharesRow(DataGridViewCellEventArgs e) => base.ContentClickUnsharesRow(e);

            public new bool ContentDoubleClickUnsharesRow(DataGridViewCellEventArgs e) => base.ContentDoubleClickUnsharesRow(e);

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new bool DoubleClickUnsharesRow(DataGridViewCellEventArgs e) => base.DoubleClickUnsharesRow(e);

            public new bool EnterUnsharesRow(int rowIndex, bool throughMouseClick) => base.EnterUnsharesRow(rowIndex, throughMouseClick);

            public new object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
            {
                return base.GetClipboardContent(rowIndex, firstCell, lastCell, inFirstRow, inLastRow, format);
            }

            public new Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex) =>  base.GetContentBounds(graphics, cellStyle, rowIndex);

            public new Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex) => base.GetErrorIconBounds(graphics, cellStyle, rowIndex);

            public new string GetErrorText(int rowIndex) => base.GetErrorText(rowIndex);

            public new object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
            {
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }

            public new Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize) => base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);

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

            public new void PaintErrorIcon(Graphics graphics, Rectangle clipBounds, Rectangle cellValueBounds, string errorText) => base.PaintErrorIcon(graphics, clipBounds, cellValueBounds, errorText);

            public new bool SetValue(int rowIndex, object value) => base.SetValue(rowIndex, value);
        }
    }
}
