// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using Microsoft.DotNet.RemoteExecutor;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    public class DataGridViewHeaderCellTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewHeaderCell_Ctor_Default()
        {
            var cell = new SubDataGridViewHeaderCell();
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
            Assert.Equal(-1, cell.ColumnIndex);
            Assert.Equal(Rectangle.Empty, cell.ContentBounds);
            Assert.Null(cell.ContextMenuStrip);
            Assert.Null(cell.DataGridView);
            Assert.Null(cell.DefaultNewRowValue);
            Assert.False(cell.Displayed);
            Assert.Null(cell.EditedFormattedValue);
            Assert.Empty(cell.ErrorText);
            Assert.Null(cell.FormattedValue);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.False(cell.HasStyle);
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet, cell.InheritedState);
            Assert.False(cell.IsInEditMode);
            Assert.Null(cell.OwningColumn);
            Assert.Null(cell.OwningRow);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.True(cell.ReadOnly);
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
            Assert.Equal(typeof(object), cell.ValueType);
            Assert.False(cell.Visible);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Displayed_GetWithRow_ReturnsExpected(bool rowVisible)
        {
            using var row = new DataGridViewRow
            {
                Visible = rowVisible
            };
            using var cell = new SubDataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.False(cell.Displayed);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Displayed_GetWithRowHeaderCell_ReturnsExpected(bool rowVisible)
        {
            using var row = new DataGridViewRow
            {
                Visible = rowVisible
            };
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.False(cell.Displayed);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Displayed_GetColumnHeaderCellHeaderCell_ReturnsExpected(bool columnVisible)
        {
            using var column = new DataGridViewColumn
            {
                Visible = columnVisible
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
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
        public void DataGridViewHeaderCell_Displayed_GetWithDataGridView_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            row.Visible = rowVisible;
            DataGridViewCell cell = Assert.IsType<DataGridViewHeaderCell>(row.Cells[0]);
            Assert.False(cell.Displayed);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetShared_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetColumnHeaderCellWithDataGridViewHeaderCell_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.False(cell.Displayed);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetRowHeaderCellWithDataGridView_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible, bool rowVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            row.Visible = rowVisible;
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.False(cell.Displayed);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Displayed_GetTopLeftHeaderCell_TestData()
        {
            foreach (bool gridVisible in new bool[] { true, false })
            {
                foreach (bool rowHeadersVisible in new bool[] { true, false })
                {
                    foreach (bool columnHeadersVisible in new bool[] { true, false })
                    {
                        yield return new object[] { gridVisible, rowHeadersVisible, columnHeadersVisible };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetTopLeftHeaderCell_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetTopLeftHeaderCell_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible,
                TopLeftHeaderCell = cell
            };
            Assert.False(cell.Displayed);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Displayed_GetShared_TestData()
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
        [MemberData(nameof(Displayed_GetShared_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetShared_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows.SharedRow(0);

            DataGridViewCell cell = Assert.IsType<DataGridViewHeaderCell>(row.Cells[0]);
            if (gridVisible && rowHeadersVisible)
            {
                Assert.Throws<InvalidOperationException>(() => cell.Displayed);
            }
            else
            {
                Assert.Equal(gridVisible && rowHeadersVisible && columnHeadersVisible && columnVisible, cell.Displayed);
            }
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetWithDataGridViewWithHandle_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
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

            DataGridViewCell cell = Assert.IsType<DataGridViewHeaderCell>(row.Cells[0]);
            Assert.Equal(gridVisible && rowHeadersVisible && rowVisible, cell.Displayed);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetShared_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetColumnHeaderCellWithDataGridViewWithHandle_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(gridVisible && columnHeadersVisible && columnVisible, cell.Displayed);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetRowHeaderCellWithDataGridViewWithHandle_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible, bool rowVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Visible = columnVisible
            };
            control.Columns.Add(column);
            control.Rows.Add();
            DataGridViewRow row = control.Rows[0];
            row.Visible = rowVisible;
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(gridVisible && rowHeadersVisible && rowVisible, cell.Displayed);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Displayed_GetTopLeftHeaderCellWithHandle_ReturnsExpected(bool visible)
        {
            using var control = new DataGridView
            {
                Visible = visible
            };
            using var cell = new DataGridViewHeaderCell();
            control.TopLeftHeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(visible, cell.Displayed);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Displayed_GetShared_TestData))]
        public void DataGridViewHeaderCell_Displayed_GetSharedWithHandle_ReturnsExpected(bool gridVisible, bool rowHeadersVisible, bool columnHeadersVisible, bool columnVisible)
        {
            using var control = new DataGridView
            {
                Visible = gridVisible,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var cellTemplate = new DataGridViewHeaderCell();
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

            DataGridViewCell cell = Assert.IsType<DataGridViewHeaderCell>(row.Cells[0]);
            if (gridVisible && rowHeadersVisible)
            {
                Assert.Throws<InvalidOperationException>(() => cell.Displayed);
            }
            else
            {
                Assert.Equal(gridVisible && rowHeadersVisible && columnHeadersVisible && columnVisible, cell.Displayed);
            }
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None, false)]
        [InlineData(DataGridViewElementStates.Frozen, false)]
        [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly, false)]
        [InlineData(DataGridViewElementStates.Frozen | DataGridViewElementStates.Selected, false)]
        public void DataGridViewHeaderCell_Frozen_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewHeaderCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.Frozen);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Frozen_GetWithRow_ReturnsExpected(bool rowFrozen)
        {
            using var row = new DataGridViewRow
            {
                Frozen = rowFrozen
            };
            using var cell = new SubDataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(rowFrozen, cell.Frozen);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Frozen_GetColumnHeaderCellHeaderCell_ReturnsExpected(bool columnFrozen)
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
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, false, false)]
        public void DataGridViewHeaderCell_Frozen_GetWithDataGridView_ReturnsExpected(bool rowFrozen, bool columnFrozen, bool expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Frozen = rowFrozen;
            column.Frozen = columnFrozen;
            DataGridViewCell cell = Assert.IsType<DataGridViewHeaderCell>(row.Cells[0]);
            Assert.Equal(expected, cell.Frozen);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewHeaderCell_Frozen_GetTopLeftHeaderCell_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible,
                TopLeftHeaderCell = cell
            };
            Assert.True(cell.Frozen);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Frozen_GetShared_ThrowsInvalidOperationException(bool columnFrozen)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.Frozen = columnFrozen;
            DataGridViewCell cell = Assert.IsType<DataGridViewHeaderCell>(row.Cells[0]);
            Assert.Throws<InvalidOperationException>(() => cell.Frozen);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None)]
        [InlineData(DataGridViewElementStates.ReadOnly)]
        [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Frozen)]
        [InlineData(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Selected)]
        public void DataGridViewHeaderCell_ReadOnly_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state)
        {
            using var cell = new CustomStateDataGridViewHeaderCell
            {
                StateResult = state
            };
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_GetWithRow_ReturnsExpected(bool rowReadOnly)
        {
            using var row = new DataGridViewRow
            {
                ReadOnly = rowReadOnly
            };
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_GetWithRowHeaderCell_ReturnsExpected(bool rowReadOnly)
        {
            using var row = new DataGridViewRow
            {
                ReadOnly = rowReadOnly
            };
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_GetColumnHeaderCellHeaderCell_ReturnsExpected(bool columnReadOnly)
        {
            using var column = new DataGridViewColumn
            {
                ReadOnly = columnReadOnly
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.True(cell.ReadOnly);
        }

        public static IEnumerable<object[]> ReadOnly_Get_TestData()
        {
            foreach (bool readOnly in new bool[] { true, false })
            {
                foreach (bool rowReadOnly in new bool[] { true, false })
                {
                    foreach (bool columnReadOnly in new bool[] { true, false })
                    {
                        yield return new object[] { readOnly, rowReadOnly, columnReadOnly };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ReadOnly_Get_TestData))]
        public void DataGridViewHeaderCell_ReadOnly_GetWithDataGridView_ReturnsExpected(bool readOnly, bool rowReadOnly, bool columnReadOnly)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ReadOnly_Get_TestData))]
        public void DataGridViewHeaderCell_ReadOnly_GetRowHeaderWithDataGridView_ReturnsExpected(bool readOnly, bool rowReadOnly, bool columnReadOnly)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ReadOnly_Get_TestData))]
        public void DataGridViewHeaderCell_ReadOnly_GetColumnHeaderCellWithDataGridView_ReturnsExpected(bool readOnly, bool rowReadOnly, bool columnReadOnly)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_GetTopLeftHeaderCell_ReturnsExpected(bool readOnly)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                ReadOnly = readOnly,
                TopLeftHeaderCell = cell
            };
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewHeaderCell_ReadOnly_GetShared_ReturnsExpected(bool readOnly, bool columnReadOnly)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_Set_ThrowsInvalidOperationException(bool value)
        {
            using var cell = new DataGridViewHeaderCell();
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetWithRow_ThrowsInvalidOperationException(bool value)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);

            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetWithRowHeaderCell_ThrowsInvalidOperationException(bool value)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetColumnHeaderCellHeaderCell_ThrowsInvalidOperationException(bool value)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetWithDataGridView_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetRowHeaderCellWithDataGridView_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;

            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetColumnHeaderCellWithDataGridView_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetTopLeftHeaderCell_ThrowsInvalidOperationException(bool value)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                TopLeftHeaderCell = cell
            };
            Assert.Throws<InvalidOperationException>(() => cell.ReadOnly = value);
            Assert.True(cell.ReadOnly);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_ReadOnly_SetShared_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
        public void DataGridViewHeaderCell_Resizable_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state, bool expected)
        {
            using var cell = new CustomStateDataGridViewHeaderCell
            {
                StateResult = state
            };
            Assert.Equal(expected, cell.Resizable);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, true)]
        [InlineData(DataGridViewTriState.False, false)]
        [InlineData(DataGridViewTriState.NotSet, false)]
        public void DataGridViewHeaderCell_Resizable_GetWithRow_ReturnsExpected(DataGridViewTriState rowResizable, bool expected)
        {
            using var row = new DataGridViewRow
            {
                Resizable = rowResizable
            };
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(expected, cell.Resizable);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, true)]
        [InlineData(DataGridViewTriState.False, false)]
        [InlineData(DataGridViewTriState.NotSet, false)]
        public void DataGridViewHeaderCell_Resizable_GetWithRowHeaderCell_ReturnsExpected(DataGridViewTriState columnResizable, bool expected)
        {
            using var row = new DataGridViewRow
            {
                Resizable = columnResizable
            };
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(expected, cell.Resizable);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, true)]
        [InlineData(DataGridViewTriState.False, false)]
        [InlineData(DataGridViewTriState.NotSet, false)]
        public void DataGridViewHeaderCell_Resizable_GetColumnHeaderCellHeaderCell_ReturnsExpected(DataGridViewTriState columnResizable, bool expected)
        {
            using var column = new DataGridViewColumn
            {
                Resizable = columnResizable
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(expected, cell.Resizable);
        }

        public static IEnumerable<object[]> Resizable_GetRowHeaderCellWithDataGridView_TestData()
        {
            foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode in Enum.GetValues(typeof(DataGridViewColumnHeadersHeightSizeMode)))
            {
                foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in Enum.GetValues(typeof(DataGridViewRowHeadersWidthSizeMode)))
                {
                    foreach (DataGridViewTriState columnResizable in Enum.GetValues(typeof(DataGridViewTriState)))
                    {
                        bool expected = rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                        yield return new object[] { columnHeadersHeightSizeMode, rowHeadersWidthSizeMode, DataGridViewTriState.True, columnResizable, true };
                        yield return new object[] { columnHeadersHeightSizeMode, rowHeadersWidthSizeMode, DataGridViewTriState.False, columnResizable, expected };
                        yield return new object[] { columnHeadersHeightSizeMode, rowHeadersWidthSizeMode, DataGridViewTriState.NotSet, columnResizable, true };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Resizable_GetRowHeaderCellWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Resizable_GetWithDataGridView_ReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode, DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewTriState rowResizable, DataGridViewTriState columnResizable, bool expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ColumnHeadersHeightSizeMode = columnHeadersHeightSizeMode,
                RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Resizable = rowResizable;
            column.Resizable = columnResizable;

            DataGridViewCell cell = row.Cells[0];
            Assert.Equal(expected, cell.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Resizable_GetRowHeaderCellWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Resizable_GetRowHeaderCellWithDataGridView_ReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode, DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewTriState rowResizable, DataGridViewTriState columnResizable, bool expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ColumnHeadersHeightSizeMode = columnHeadersHeightSizeMode,
                RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Resizable = rowResizable;
            column.Resizable = columnResizable;

            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(expected, cell.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Resizable_GetColumnHeaderCellWithDataGridView_TestData()
        {
            foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode in Enum.GetValues(typeof(DataGridViewColumnHeadersHeightSizeMode)))
            {
                foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in Enum.GetValues(typeof(DataGridViewRowHeadersWidthSizeMode)))
                {
                    foreach (DataGridViewTriState rowResizable in Enum.GetValues(typeof(DataGridViewTriState)))
                    {
                        bool expected = columnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                        yield return new object[] { columnHeadersHeightSizeMode, rowHeadersWidthSizeMode, rowResizable, DataGridViewTriState.True, true };
                        yield return new object[] { columnHeadersHeightSizeMode, rowHeadersWidthSizeMode, rowResizable, DataGridViewTriState.False, expected };
                        yield return new object[] { columnHeadersHeightSizeMode, rowHeadersWidthSizeMode, rowResizable, DataGridViewTriState.NotSet, true };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Resizable_GetColumnHeaderCellWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Resizable_GetColumnHeaderCellWithDataGridView_ReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode, DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, DataGridViewTriState rowResizable, DataGridViewTriState columnResizable, bool expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ColumnHeadersHeightSizeMode = columnHeadersHeightSizeMode,
                RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Resizable = rowResizable;
            column.Resizable = columnResizable;

            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(expected, cell.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Resizable_GetTopLeftHeaderCell_TestData()
        {
            foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode in Enum.GetValues(typeof(DataGridViewColumnHeadersHeightSizeMode)))
            {
                foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in Enum.GetValues(typeof(DataGridViewRowHeadersWidthSizeMode)))
                {
                    foreach (DataGridViewTriState rowResizable in Enum.GetValues(typeof(DataGridViewTriState)))
                    {
                        bool expected = columnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing || rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                        yield return new object[] { columnHeadersHeightSizeMode, rowHeadersWidthSizeMode, expected };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Resizable_GetTopLeftHeaderCell_TestData))]
        public void DataGridViewHeaderCell_Resizable_GetTopLeftHeaderCell_ReturnsExpected(DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode, DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                ColumnHeadersHeightSizeMode = columnHeadersHeightSizeMode,
                RowHeadersWidthSizeMode = rowHeadersWidthSizeMode
            };

            using var cell = new DataGridViewHeaderCell();
            control.TopLeftHeaderCell = cell;
            Assert.Equal(expected, cell.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewTriState))]
        public void DataGridViewHeaderCell_Resizable_GetShared_ThrowsInvalidOperationException(DataGridViewTriState columnResizable)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.Resizable = columnResizable;
            DataGridViewCell cell = row.Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.Resizable);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None)]
        [InlineData(DataGridViewElementStates.Selected)]
        [InlineData(DataGridViewElementStates.Selected | DataGridViewElementStates.ReadOnly)]
        [InlineData(DataGridViewElementStates.Selected | DataGridViewElementStates.Frozen)]
        public void DataGridViewHeaderCell_Selected_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state)
        {
            using var cell = new CustomStateDataGridViewHeaderCell
            {
                StateResult = state
            };
            Assert.False(cell.Selected);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Selected_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.False(cell.Selected);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Selected_GetWithRowHeaderCell_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.False(cell.Selected);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Selected_GetColumnHeaderCellHeaderCell_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewHeaderCell_Selected_GetWithDataGridView_ReturnsExpected(bool rowSelected, bool columnSelected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewHeaderCell_Selected_GetRowHeaderCellWithDataGridView_ReturnsExpected(bool rowSelected, bool columnSelected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Selected = rowSelected;
            column.Selected = columnSelected;
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewHeaderCell_Selected_GetColumnHeaderCellWithDataGridView_ReturnsExpected(bool rowSelected, bool columnSelected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Selected = rowSelected;
            column.Selected = columnSelected;
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Selected_GetTopLeftHeaderCell_ReturnsExpected()
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                TopLeftHeaderCell = cell
            };
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_GetShared_ReturnsExpected(bool columnSelected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_Set_ThrowsInvalidOperationException(bool value)
        {
            using var cell = new DataGridViewHeaderCell();
            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetWithRow_ThrowsInvalidOperationException(bool value)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetWithRowHeaderCell_ThrowsInvalidOperationException(bool value)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetColumnHeaderCellHeaderCell_ThrowsInvalidOperationException(bool value)
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetWithDataGridView_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetRowHeaderCellWithDataGridView_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetColumnHeaderCellWithDataGridView_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetTopLeftHeaderCell_ThrowsInvalidOperationException(bool value)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                TopLeftHeaderCell = cell
            };

            Assert.Throws<InvalidOperationException>(() => cell.Selected = value);
            Assert.False(cell.Selected);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Selected_SetShared_ThrowsInvalidOperationException(bool value)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
        public void DataGridViewHeaderCell_ValueType_GetWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(typeof(object), cell.ValueType);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_ValueType_GetWithRowHeaderCell_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(typeof(object), cell.ValueType);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewHeaderCell_ValueType_GetColumnHeaderCellHeaderCell_ReturnsExpected(Type valueType)
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
        public void DataGridViewHeaderCell_ValueType_GetWithDataGridView_ReturnsExpected(Type valueType)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                ValueType = valueType
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Equal(typeof(object), cell.ValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewHeaderCell_ValueType_GetRowHeaderCellWithDataGridView_ReturnsExpected(Type valueType)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                ValueType = valueType
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(typeof(object), cell.ValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void DataGridViewHeaderCell_ValueType_GetColumnHeaderCellWithDataGridView_ReturnsExpected(Type valueType)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                ValueType = valueType
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(typeof(object), cell.ValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_ValueType_GetTopLeftHeaderCell_ReturnsExpected()
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                TopLeftHeaderCell = cell
            };
            Assert.Equal(typeof(object), cell.ValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_Set_GetReturnsExpected(Type value, Type expected)
        {
            using var cell = new DataGridViewHeaderCell
            {
                ValueType = value
            };
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var cell = new DataGridViewHeaderCell
            {
                ValueType = typeof(string)
            };
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetWithRow_GetReturnsExpected(Type value, Type expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetWithRowWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell
            {
                ValueType = typeof(string)
            };
            row.Cells.Add(cell);

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        public static IEnumerable<object[]> ValueType_SetColumnHeaderCellHeaderCell_TestData()
        {
            yield return new object[] { null, typeof(object) };
            yield return new object[] { typeof(object), typeof(object) };
            yield return new object[] { typeof(int), typeof(int) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ValueType_SetColumnHeaderCellHeaderCell_TestData))]
        public void DataGridViewHeaderCell_ValueType_SetWithRowHeaderCell_GetReturnsExpected(Type value, Type expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        [WinFormsTheory]
        [MemberData(nameof(ValueType_SetColumnHeaderCellHeaderCell_TestData))]
        public void DataGridViewHeaderCell_ValueType_SetWithRowHeaderCellWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewRowHeaderCell
            {
                ValueType = typeof(string)
            };
            row.HeaderCell = cell;

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
        }

        [WinFormsTheory]
        [MemberData(nameof(ValueType_SetColumnHeaderCellHeaderCell_TestData))]
        public void DataGridViewHeaderCell_ValueType_SetColumnHeaderCellHeaderCell_GetReturnsExpected(Type value, Type expected)
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
        [MemberData(nameof(ValueType_SetColumnHeaderCellHeaderCell_TestData))]
        public void DataGridViewHeaderCell_ValueType_SetColumnHeaderCellWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
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
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetWithDataGridView_GetReturnsExpected(Type value, Type expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetWithDataGridViewWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ValueType = typeof(string);

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetRowHeaderCellWithDataGridView_GetReturnsExpected(Type value, Type expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetRowHeaderCellWithDataGridViewWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            cell.ValueType = typeof(string);

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetColumnHeaderCellWithDataGridView_GetReturnsExpected(Type value, Type expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetColumnHeaderCellWithDataGridViewWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            cell.ValueType = typeof(string);

            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetTopLeftHeaderCell_GetReturnsExpected(Type value, Type expected)
        {
            using var cell = new DataGridViewHeaderCell
            {
                ValueType = value
            };
            using var control = new DataGridView
            {
                TopLeftHeaderCell = cell
            };
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(object), typeof(object))]
        [InlineData(null, typeof(object))]
        public void DataGridViewHeaderCell_ValueType_SetTopLeftHeaderCellWithNonNullOldValue_GetReturnsExpected(Type value, Type expected)
        {
            using var cell = new DataGridViewHeaderCell
            {
                ValueType = typeof(string)
            };
            using var control = new DataGridView
            {
                TopLeftHeaderCell = cell
            };
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);

            // Set same.
            cell.ValueType = value;
            Assert.Equal(expected, cell.ValueType);
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewElementStates.None)]
        [InlineData(DataGridViewElementStates.Visible)]
        [InlineData(DataGridViewElementStates.Visible | DataGridViewElementStates.ReadOnly)]
        [InlineData(DataGridViewElementStates.Visible | DataGridViewElementStates.Selected)]
        public void DataGridViewHeaderCell_Visible_GetWithCustomState_ReturnsExpected(DataGridViewElementStates state)
        {
            using var cell = new CustomStateDataGridViewHeaderCell
            {
                StateResult = state
            };
            Assert.False(cell.Visible);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Visible_GetWithRow_ReturnsExpected(bool rowVisible)
        {
            using var row = new DataGridViewRow
            {
                Visible = rowVisible
            };
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(rowVisible, cell.Visible);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Visible_GetRowHeaderCell_ReturnsExpected(bool rowVisible)
        {
            using var row = new DataGridViewRow
            {
                Visible = rowVisible
            };
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(rowVisible, row.Visible);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Visible_GetColumnHeaderCell_ReturnsExpected(bool columnVisible)
        {
            using var column = new DataGridViewColumn
            {
                Visible = columnVisible
            };
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(columnVisible, cell.Visible);
        }

        public static IEnumerable<object[]> Visible_GetWithDataGridView_TestData()
        {
            foreach (bool rowHeadersVisible in new bool[] { true, false })
            {
                foreach (bool columnHeadersVisible in new bool[] { true, false })
                {
                    foreach (bool rowVisible in new bool[] { true, false })
                    {
                        foreach (bool columnVisible in new bool[] { true, false })
                        {
                            yield return new object[] { rowHeadersVisible, columnHeadersVisible, rowVisible, columnVisible };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Visible_GetWithDataGridView_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            Assert.Equal(rowVisible && rowHeadersVisible, cell.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Visible_GetRowHeaderCellWithDataGridView_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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

            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(rowVisible && rowHeadersVisible, cell.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_Visible_GetColumnHeaderCellWithDataGridView_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible, bool rowVisible, bool columnVisible)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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

            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(columnVisible && columnHeadersVisible, cell.Visible);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Visible_GetTopLeftHeaderCell_TestData()
        {
            foreach (bool rowHeadersVisible in new bool[] { true, false })
            {
                foreach (bool columnHeadersVisible in new bool[] { true, false })
                {
                    yield return new object[] { rowHeadersVisible, columnHeadersVisible };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Visible_GetTopLeftHeaderCell_TestData))]
        public void DataGridViewHeaderCell_Visible_GetTopLeftHeaderCell_ReturnsExpected(bool rowHeadersVisible, bool columnHeadersVisible)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersVisible = columnHeadersVisible,
                TopLeftHeaderCell = cell
            };

            Assert.Equal(rowHeadersVisible && columnHeadersVisible, cell.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewHeaderCell_Visible_GetShared_ThrowsInvalidOperationException(bool columnVisible)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows.SharedRow(0);
            column.Visible = columnVisible;

            DataGridViewCell cell = row.Cells[0];
            Assert.Throws<InvalidOperationException>(() => cell.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Clone_NonEmpty_Success()
        {
            using var menu = new ContextMenuStrip();
            using var source = new DataGridViewHeaderCell
            {
                ContextMenuStrip = menu,
                ErrorText = "errorText",
                Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft },
                Tag = "tag",
                ToolTipText = "toolTipText",
                Value = "value",
                ValueType = typeof(int)
            };

            DataGridViewHeaderCell cell = Assert.IsType<DataGridViewHeaderCell>(source.Clone());
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
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.True(cell.HasStyle);
            Assert.False(cell.IsInEditMode);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.True(cell.ReadOnly);
            Assert.False(cell.Resizable);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.False(cell.Selected);
            Assert.Equal(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft }, cell.Style);
            Assert.NotSame(source.Style, cell.Style);
            Assert.Equal("tag", cell.Tag);
            Assert.Equal("toolTipText", cell.ToolTipText);
            Assert.Equal("value", cell.Value);
            Assert.Equal(typeof(int), cell.ValueType);
            Assert.False(cell.Visible);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Clone_Empty_Success()
        {
            using var source = new DataGridViewHeaderCell();
            DataGridViewHeaderCell cell = Assert.IsType<DataGridViewHeaderCell>(source.Clone());
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
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.False(cell.HasStyle);
            Assert.False(cell.IsInEditMode);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.True(cell.ReadOnly);
            Assert.False(cell.Resizable);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.False(cell.Selected);
            Assert.Equal(new DataGridViewCellStyle(), cell.Style);
            Assert.Null(cell.Tag);
            Assert.Empty(cell.ToolTipText);
            Assert.Null(cell.Value);
            Assert.Equal(typeof(object), cell.ValueType);
            Assert.False(cell.Visible);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Clone_NonEmptySubclass_Success()
        {
            using var menu = new ContextMenuStrip();
            using var source = new SubDataGridViewHeaderCell
            {
                ContextMenuStrip = menu,
                ErrorText = "errorText",
                Style = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft },
                Tag = "tag",
                ToolTipText = "toolTipText",
                Value = "value",
                ValueType = typeof(int)
            };

            SubDataGridViewHeaderCell cell = Assert.IsType<SubDataGridViewHeaderCell>(source.Clone());
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
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.True(cell.HasStyle);
            Assert.False(cell.IsInEditMode);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.True(cell.ReadOnly);
            Assert.False(cell.Resizable);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.False(cell.Selected);
            Assert.Equal(new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft }, cell.Style);
            Assert.NotSame(source.Style, cell.Style);
            Assert.Equal("tag", cell.Tag);
            Assert.Equal("toolTipText", cell.ToolTipText);
            Assert.Equal("value", cell.Value);
            Assert.Equal(typeof(int), cell.ValueType);
            Assert.False(cell.Visible);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Clone_EmptySubclass_Success()
        {
            using var source = new SubDataGridViewHeaderCell();
            SubDataGridViewHeaderCell cell = Assert.IsType<SubDataGridViewHeaderCell>(source.Clone());
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
            Assert.Equal(typeof(string), cell.FormattedValueType);
            Assert.False(cell.Frozen);
            Assert.False(cell.HasStyle);
            Assert.False(cell.IsInEditMode);
            Assert.Equal(new Size(-1, -1), cell.PreferredSize);
            Assert.True(cell.ReadOnly);
            Assert.False(cell.Resizable);
            Assert.Equal(new Size(-1, -1), cell.Size);
            Assert.False(cell.Selected);
            Assert.Equal(new DataGridViewCellStyle(), cell.Style);
            Assert.Null(cell.Tag);
            Assert.Empty(cell.ToolTipText);
            Assert.Null(cell.Value);
            Assert.Equal(typeof(object), cell.ValueType);
            Assert.False(cell.Visible);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_Invoke_ReturnsExpected(int rowIndex)
        {
            using var cell = new DataGridViewHeaderCell();
            Assert.Null(cell.GetInheritedContextMenuStrip(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeWithMenu_ReturnsExpected(int rowIndex)
        {
            using var cellMenu = new ContextMenuStrip();
            using var cell = new DataGridViewHeaderCell
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
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeWithRow_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
        {
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = menu
            };
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Null(cell.GetInheritedContextMenuStrip(rowIndex));
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedContextMenuStrip_TestData))]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeWithMenuWithRow_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
        {
            using var row = new DataGridViewRow
            {
                ContextMenuStrip = menu
            };
            using var cellMenu = new ContextMenuStrip();
            using var cell = new DataGridViewHeaderCell
            {
                ContextMenuStrip = cellMenu
            };
            row.Cells.Add(cell);
            Assert.Same(cellMenu, cell.GetInheritedContextMenuStrip(rowIndex));
        }

        public static IEnumerable<object[]> ContextMenuStrip_GetWithDataGridView_TestData()
        {
            yield return new object[] { -2, null };
            yield return new object[] { -2, new ContextMenuStrip() };
            yield return new object[] { -1, null };
            yield return new object[] { -1, new ContextMenuStrip() };
            yield return new object[] { 0, null };
            yield return new object[] { 0, new ContextMenuStrip() };
            yield return new object[] { 1, null };
            yield return new object[] { 1, new ContextMenuStrip() };
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeWithDataGridView_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
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
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;

            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Same(menu, cell.GetInheritedContextMenuStrip(rowIndex));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeWithMenuWithDataGridView_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
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
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;

            using var cellMenu = new ContextMenuStrip();
            DataGridViewCell cell = control.Rows[0].Cells[0];
            cell.ContextMenuStrip = cellMenu;
            Assert.Same(cellMenu, cell.GetInheritedContextMenuStrip(rowIndex));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeTopLeftHeaderCell_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
        {
            using var control = new DataGridView
            {
                ContextMenuStrip = menu
            };
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;

            using var cell = new DataGridViewHeaderCell();
            control.TopLeftHeaderCell = cell;
            Assert.Same(menu, cell.GetInheritedContextMenuStrip(rowIndex));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeTopLeftHeaderCellWithMenu_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
        {
            using var control = new DataGridView
            {
                ContextMenuStrip = menu
            };
            int callCount = 0;
            control.CellContextMenuStripNeeded += (sender, e) => callCount++;

            using var cellMenu = new ContextMenuStrip();
            using var cell = new DataGridViewHeaderCell
            {
                ContextMenuStrip = cellMenu
            };
            control.TopLeftHeaderCell = cell;
            Assert.Same(cellMenu, cell.GetInheritedContextMenuStrip(rowIndex));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ContextMenuStrip_GetWithDataGridView_TestData))]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeShared_ReturnsExpected(int rowIndex, ContextMenuStrip menu)
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
            Assert.Same(menu, cell.GetInheritedContextMenuStrip(rowIndex));
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeWithDataGridViewVirtualMode_CallsCellContextMenuStripNeeded()
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
        public void DataGridViewHeaderCell_GetInheritedContextMenuStrip_InvokeWithDataGridViewDataSource_CallsCellContextMenuStripNeeded()
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

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_Invoke_ReturnsExpected()
        {
            using var cell = new DataGridViewHeaderCell();
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet, cell.GetInheritedState(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(-1));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.False, DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewElementStates.ResizableSet)]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithRowCustomState_ReturnsExpected(DataGridViewTriState resizable, DataGridViewElementStates expected)
        {
            using var row = new DataGridViewRow
            {
                Frozen = true,
                ReadOnly = true,
                Visible = false,
                Resizable = resizable
            };
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | expected, cell.GetInheritedState(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeColumnHeaderCell_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(-1));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewTriState.True, DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.False, DataGridViewElementStates.ResizableSet)]
        [InlineData(DataGridViewTriState.NotSet, DataGridViewElementStates.ResizableSet)]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeColumnHeaderCellCustomState_ReturnsExpected(DataGridViewTriState resizable, DataGridViewElementStates expected)
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
            Assert.Equal(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | expected, cell.GetInheritedState(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithDataGrid_ReturnsExpected()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);

            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(0));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetInheritedState_DataGridViewCustomState_TestData()
        {
            // Frozen.
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };

            // Visible.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };

            // Resizable.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_DataGridViewCustomState_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithDataGridCustomState_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithRowHeaderCell_ReturnsExpected()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);

            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_DataGridViewCustomState_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithRowHeaderCellCustomState_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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

            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeColumnHeaderCellHeaderCell_ReturnsExpected()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);

            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible, cell.GetInheritedState(-1));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetInheritedState_ColumnHeaderCellCustomState_TestData()
        {
            // Frozen.
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };

            // Visible.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };

            // Resizable.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_ColumnHeaderCellCustomState_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeColumnHeaderCellHeaderCellCustomState_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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

            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(-1));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> GetInheritedState_TopLeftHeaderCell_TestData()
        {
            foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in Enum.GetValues(typeof(DataGridViewRowHeadersWidthSizeMode)))
            {
                foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode in Enum.GetValues(typeof(DataGridViewColumnHeadersHeightSizeMode)))
                {
                    DataGridViewElementStates expected = rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || columnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing ? DataGridViewElementStates.Resizable : (DataGridViewElementStates)0;
                    yield return new object[] { rowHeadersWidthSizeMode, true, columnHeadersHeightSizeMode, true, DataGridViewElementStates.Visible | expected };
                    yield return new object[] { rowHeadersWidthSizeMode, true, columnHeadersHeightSizeMode, false, expected };
                    yield return new object[] { rowHeadersWidthSizeMode, false, columnHeadersHeightSizeMode, false, expected };
                    yield return new object[] { rowHeadersWidthSizeMode, false, columnHeadersHeightSizeMode, false, expected };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_TopLeftHeaderCell_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeTopLeftHeaderCell_ReturnsExpected(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode, bool columnHeadersVisible, DataGridViewElementStates expected)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersHeightSizeMode = columnHeadersHeightSizeMode,
                ColumnHeadersVisible = columnHeadersVisible
            };
            control.TopLeftHeaderCell = cell;
            Assert.Equal(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(-1));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithDataGridWithHandle_ReturnsExpected()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed, cell.GetInheritedState(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetInheritedState_DataGridViewCustomStateWithHandle_TestData()
        {
            // Frozen.
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };

            // Visible.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };

            // Resizable.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_DataGridViewCustomStateWithHandle_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithDataGridCustomStateWithHandle_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithRowHeaderCellWithHandle_ReturnsExpected()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed, cell.GetInheritedState(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_DataGridViewCustomStateWithHandle_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeWithRowHeaderCellCustomStateWithHandle_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(0));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeColumnHeaderCellHeaderCellWithHandle_ReturnsExpected()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed, cell.GetInheritedState(-1));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetInheritedState_ColumnHeaderCellCustomStateWithHandle_TestData()
        {
            // Frozen.
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, true, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible  | DataGridViewElementStates.Displayed};
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, true, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible  | DataGridViewElementStates.Displayed};

            // Visible.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.True, false, false, DataGridViewTriState.True, false, false, DataGridViewElementStates.Resizable };

            // Resizable.
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.True, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.False, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.True, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.False, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
            yield return new object[] { DataGridViewTriState.NotSet, false, true, DataGridViewTriState.NotSet, false, true, DataGridViewElementStates.Resizable | DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_ColumnHeaderCellCustomStateWithHandle_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeColumnHeaderCellHeaderCellCustomStateWithHandle_ReturnsExpected(DataGridViewTriState rowResizable, bool rowFrozen, bool rowVisible, DataGridViewTriState columnResizable, bool columnFrozen, bool columnVisible, DataGridViewElementStates expected)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
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
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(-1));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

#if false
        public static IEnumerable<object[]> GetInheritedState_TopLeftHeaderCellWithHandle_TestData()
        {
            foreach (DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode in Enum.GetValues(typeof(DataGridViewRowHeadersWidthSizeMode)))
            {
                foreach (DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode in Enum.GetValues(typeof(DataGridViewColumnHeadersHeightSizeMode)))
                {
                    DataGridViewElementStates expected = rowHeadersWidthSizeMode == DataGridViewRowHeadersWidthSizeMode.EnableResizing || columnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.EnableResizing ? DataGridViewElementStates.Resizable : (DataGridViewElementStates)0;
                    yield return new object[] { rowHeadersWidthSizeMode, true, columnHeadersHeightSizeMode, true, DataGridViewElementStates.Visible | DataGridViewElementStates.Displayed | expected };
                    yield return new object[] { rowHeadersWidthSizeMode, true, columnHeadersHeightSizeMode, false, expected };
                    yield return new object[] { rowHeadersWidthSizeMode, false, columnHeadersHeightSizeMode, false, expected };
                    yield return new object[] { rowHeadersWidthSizeMode, false, columnHeadersHeightSizeMode, false, expected };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(GetInheritedState_TopLeftHeaderCellWithHandle_TestData))]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeTopLeftHeaderCellWithHandle_ReturnsExpected(DataGridViewRowHeadersWidthSizeMode rowHeadersWidthSizeMode, bool rowHeadersVisible, DataGridViewColumnHeadersHeightSizeMode columnHeadersHeightSizeMode, bool columnHeadersVisible, DataGridViewElementStates expected)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView
            {
                RowHeadersWidthSizeMode = rowHeadersWidthSizeMode,
                RowHeadersVisible = rowHeadersVisible,
                ColumnHeadersHeightSizeMode = columnHeadersHeightSizeMode,
                ColumnHeadersVisible = columnHeadersVisible
            };
            control.TopLeftHeaderCell = cell;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.ResizableSet | expected, cell.GetInheritedState(-1));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
#endif

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetInheritedState_InvokeSharedRow_ThrowsArgumentException()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentException>("rowIndex", () => cell.GetInheritedState(-1));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexWithRow_ThrowsArgumentException(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Throws<ArgumentException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexRowHeaderCell_ThrowsArgumentException(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Throws<ArgumentException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexColumnHeaderCell_ThrowsArgumentOutOfRangeException(int rowIndex)
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
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexWithDataGridView_ThrowsArgumentException(int rowIndex)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Throws<ArgumentException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexRowHeaderCellWithDataGridView_ThrowsArgumentException(int rowIndex)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);

            DataGridViewRow row = control.Rows[0];
            using var cell = new DataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Throws<ArgumentException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexColumnHeaderCellWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);

            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexTopLeftHeaderCell_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cell = new DataGridViewHeaderCell();
            using var control = new DataGridView();
            control.TopLeftHeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetInheritedState_InvalidRowIndexShared_ThrowsArgumentException(int rowIndex)
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentException>("rowIndex", () => cell.GetInheritedState(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_Invoke_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(new Size(-1, -1), cell.GetSize(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal(new Size(-1, -1), cell.GetSize(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeRowHeaderCell_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(new Size(-1, -1), cell.GetSize(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeColumnHeaderCell_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(new Size(-1, -1), cell.GetSize(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView
            {
                RowHeadersWidth = 20,
                ColumnHeadersHeight = 25
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Height = 11;

            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)row.Cells[0];
            Assert.Equal(new Size(10, 25), cell.GetSize(-1));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeRowHeaderCellWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView
            {
                RowHeadersWidth = 20,
                ColumnHeadersHeight = 25
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Height = 11;

            using var cell = new SubDataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Equal(new Size(20, 11), cell.GetSize(0));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeColumnHeaderCellWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView
            {
                RowHeadersWidth = 20,
                ColumnHeadersHeight = 25
            };
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            row.Height = 11;

            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal(new Size(10, 25), cell.GetSize(-1));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeTopLeftHeaderCell_ReturnsExpected()
        {
            using var cell = new SubDataGridViewColumnHeaderCell();
            using var control = new DataGridView
            {
                RowHeadersWidth = 20,
                ColumnHeadersHeight = 25,
                TopLeftHeaderCell = cell
            };
            Assert.Equal(new Size(20, 25), cell.GetSize(-1));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetSize_InvokeShared_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView
            {
                RowHeadersWidth = 20,
                ColumnHeadersHeight = 25
            };
            control.Columns.Add(column);

            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Equal(new Size(10, 25), cell.GetSize(-1));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetSize_InvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetSize(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetSize_InvalidRowIndexWithRow_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetSize(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetSize_InvalidRowIndexRowHeaderCell_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetSize(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetSize_InvalidRowIndexColumnHeaderCell_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetSize(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetSize_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetSize(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetSize_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetSize(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_Invoke_ReturnsExpected()
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Null(cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValue_ReturnsExpected()
        {
            var value = new object();
            using var cell = new SubDataGridViewHeaderCell
            {
                Value = value
            };
            Assert.Same(value, cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Null(cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValueWithRow_ReturnsExpected()
        {
            var value = new object();
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewHeaderCell
            {
                Value = value
            };
            row.Cells.Add(cell);
            Assert.Same(value, cell.GetValue(-1));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvokeRowHeaderCell_ReturnsExpected(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Null(cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValueRowHeaderCell_ReturnsExpected(int rowIndex)
        {
            var value = new object();
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewRowHeaderCell
            {
                Value = value
            };
            row.HeaderCell = cell;
            Assert.Same(value, cell.GetValue(rowIndex));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeColumnHeaderCell_ReturnsExpected()
        {
            using var row = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            row.HeaderCell = cell;
            Assert.Empty((string)cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValueColumnHeaderCell_ReturnsExpected()
        {
            var value = new object();
            using var row = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell
            {
                Value = value
            };
            row.HeaderCell = cell;
            Assert.Same(value, cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Null(cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValueWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            var value = new object();
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.Value = value);
            Assert.Null(cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeRowHeaderCellWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new SubDataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            Assert.Null(cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValueRowHeaderCellWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewRow row = control.Rows[0];
            using var cell = new SubDataGridViewRowHeaderCell();
            row.HeaderCell = cell;
            var value = new object();
            cell.Value = value;
            Assert.Same(value, cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeColumnHeaderCellWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Empty((string)cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValueColumnHeaderCellWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate,
                Width = 10
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            var value = new object();
            cell.Value = value;
            Assert.Same(value, cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeTopLeftHeaderCell_ReturnsExpected()
        {
            using var control = new DataGridView();
            using var cell = new SubDataGridViewHeaderCell();
            control.TopLeftHeaderCell = cell;
            Assert.Null(cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithValueTopLeftHeaderCell_ReturnsExpected()
        {
            using var control = new DataGridView();
            using var cell = new SubDataGridViewHeaderCell();
            control.TopLeftHeaderCell = cell;
            var value = new object();
            cell.Value = value;
            Assert.Same(value, cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeShared_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Null(cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeSharedWithValue_ReturnsExpected()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows.SharedRow(0).Cells[0];
            var value = new object();
            cell.Value = value;
            Assert.Same(value, cell.GetValue(-1));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithDataGridViewVirtualMode_CallsCellValueNeeded()
        {
            var value1 = new object();
            var value2 = new object();
            using var cellTemplate = new SubDataGridViewHeaderCell();
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
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.Value = value1);

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

            Assert.Null(cell.GetValue(-1));
            Assert.Equal(0, callCount);

            // Remove the handler.
            control.CellValueNeeded -= handler;
            Assert.Null(cell.GetValue(-1));
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithDataGridViewNewRowVirtualMode_CallsCellValueNeeded()
        {
            var value1 = new object();
            var value2 = new object();
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                VirtualMode = true
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.Value = value1);

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

            Assert.Null(cell.GetValue(-1));
            Assert.Equal(0, callCount);

            // Remove the handler.
            control.CellValueNeeded -= handler;
            Assert.Null(cell.GetValue(-1));
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_GetValue_InvokeWithDataGridViewNewRowDataSource_CallsCellValueNeeded()
        {
            var value1 = new object();
            var value2 = new object();
            using var cellTemplate = new SubDataGridViewHeaderCell();
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
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.Value = value1);

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

            Assert.Null(cell.GetValue(-1));
            Assert.Equal(0, callCount);

            // Remove the handler.
            control.CellValueNeeded -= handler;
            Assert.Null(cell.GetValue(-1));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvokeInvalidRowIndex_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvokeInvalidRowIndexWithRow_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var row = new DataGridViewRow();
            using var cell = new SubDataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvokeInvalidColumnIndexColumnHeaderCell_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var column = new DataGridViewColumn();
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvalidRowIndexWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvalidRowIndexColumnHeaderCellWithDataGridView_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            using var cell = new SubDataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvalidRowIndexTopLeftHeaderCell_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cell = new SubDataGridViewHeaderCell();
            using var control = new DataGridView
            {
                TopLeftHeaderCell = cell
            };
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_GetValue_InvalidRowIndexShared_ThrowsArgumentOutOfRangeException(int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows.SharedRow(0).Cells[0];
            Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.GetValue(rowIndex));
        }

        public static IEnumerable<object[]> MouseDownUnsharesRow_TestData()
        {
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            //yield return new object[] { new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            //yield return new object[] { new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            //yield return new object[] { new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            //yield return new object[] { new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            //yield return new object[] { new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)) };
            //yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)) };
        }

        [WinFormsTheory]
        [MemberData(nameof(MouseDownUnsharesRow_TestData))]
        public void DataGridViewHeaderCell_MouseDownUnsharesRow_Invoke_ReturnsFalse(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.False(cell.MouseDownUnsharesRow(e));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        public static IEnumerable<object[]> MouseDownUnsharesRow_WithDataGridView_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), false };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), false };
            }
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void DataGridViewHeaderCell_MouseDownUnsharesRow_InvokeWithDataGridView_ReturnsExpected()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke(() =>
            {
                foreach (object[] testData in MouseDownUnsharesRow_WithDataGridView_TestData())
                {
                    bool enableHeadersVisualStyles = (bool)testData[0];
                    DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                    bool expected = (bool)testData[2];

                    Application.EnableVisualStyles();

                    using var cellTemplate = new SubDataGridViewHeaderCell();
                    using var column = new DataGridViewColumn
                    {
                        CellTemplate = cellTemplate
                    };
                    using var control = new DataGridView
                    {
                        EnableHeadersVisualStyles = enableHeadersVisualStyles
                    };
                    control.Columns.Add(column);
                    SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                    Assert.Equal(expected, cell.MouseDownUnsharesRow(e));
                    Assert.Equal(ButtonState.Normal, cell.ButtonState);
                    Assert.False(control.IsHandleCreated);
                }
            }).Dispose();
        }

        public static IEnumerable<object[]> MouseDownUnsharesRow_ButtonLeftNullDataGridView_TestData()
        {
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
            yield return new object[] { new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)) };
        }

        [WinFormsTheory]
        [MemberData(nameof(MouseDownUnsharesRow_ButtonLeftNullDataGridView_TestData))]
        public void DataGridViewHeaderCell_MouseDownUnsharesRow_ButtonLeftNullDataGridView_ThrowsNullReferenceException(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Throws<NullReferenceException>(() => cell.MouseDownUnsharesRow(e));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_MouseDownUnsharesRow_NullE_ThrowsNullReferenceException()
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Throws<NullReferenceException>(() => cell.MouseDownUnsharesRow(null));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_MouseEnterUnsharesRow_InvokeWithoutDataGridView_ThrowsNullReferenceException(int rowIndex)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Throws<NullReferenceException>(() => cell.MouseEnterUnsharesRow(rowIndex));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
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
        public void DataGridViewHeaderCell_MouseEnterUnsharesRow_InvokeWithDataGridView_ReturnsExpected(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.False(cell.MouseEnterUnsharesRow(rowIndex));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_MouseLeaveUnsharesRow_InvokeWithoutDataGridView_ReturnsFalse(int rowIndex)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.False(cell.MouseLeaveUnsharesRow(rowIndex));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
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
        public void DataGridViewHeaderCell_MouseLeaveUnsharesRow_InvokeWithDataGridView_ReturnsExpected(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.False(cell.MouseLeaveUnsharesRow(rowIndex));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> MouseLeaveUnsharesRow_WithDataGridViewMouseDown_TestData()
        {
            ButtonState expected = VisualStyleRenderer.IsSupported ? ButtonState.Pushed : ButtonState.Normal;
            yield return new object[] { true, -2, expected };
            yield return new object[] { true, -1, expected };
            yield return new object[] { true, 0, expected };
            yield return new object[] { true, 1, expected };
            yield return new object[] { false, -2, ButtonState.Normal };
            yield return new object[] { false, -1, ButtonState.Normal };
            yield return new object[] { false, 0, ButtonState.Normal };
            yield return new object[] { false, 1, ButtonState.Normal };
        }

        [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        [MemberData(nameof(MouseLeaveUnsharesRow_WithDataGridViewMouseDown_TestData))]
        public void DataGridViewHeaderCell_MouseLeaveUnsharesRow_InvokeWithDataGridViewMouseDown_ReturnsExpected(bool enableHeadersVisualStylesParam, int rowIndexParam, ButtonState expectedButtonStateParam)
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke((enableHeadersVisualStylesString, rowIndexString, expectedButtonStateString) =>
            {
                bool enableHeadersVisualStyles = bool.Parse(enableHeadersVisualStylesString);
                int rowIndex = int.Parse(rowIndexString);
                ButtonState expectedButtonState = (ButtonState)Enum.Parse(typeof(ButtonState), expectedButtonStateString);

                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = enableHeadersVisualStyles
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
                Assert.Equal(enableHeadersVisualStyles && VisualStyleRenderer.IsSupported, cell.MouseLeaveUnsharesRow(rowIndex));
                Assert.Equal(expectedButtonState, cell.ButtonState);
                Assert.False(control.IsHandleCreated);
            }, enableHeadersVisualStylesParam.ToString(), rowIndexParam.ToString(), expectedButtonStateParam.ToString()).Dispose();
        }

        [WinFormsTheory]
        [MemberData(nameof(MouseDownUnsharesRow_TestData))]
        public void DataGridViewHeaderCell_MouseUpUnsharesRow_Invoke_ReturnsFalse(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.False(cell.MouseUpUnsharesRow(e));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void DataGridViewHeaderCell_MouseUpUnsharesRow_InvokeWithDataGridView_ReturnsExpected()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke(() =>
            {
                foreach (object[] testData in MouseDownUnsharesRow_WithDataGridView_TestData())
                {
                    bool enableHeadersVisualStyles = (bool)testData[0];
                    DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                    bool expected = (bool)testData[2];

                    Application.EnableVisualStyles();

                    using var cellTemplate = new SubDataGridViewHeaderCell();
                    using var column = new DataGridViewColumn
                    {
                        CellTemplate = cellTemplate
                    };
                    using var control = new DataGridView
                    {
                        EnableHeadersVisualStyles = enableHeadersVisualStyles
                    };
                    control.Columns.Add(column);
                    SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                    Assert.Equal(expected, cell.MouseUpUnsharesRow(e));
                    Assert.Equal(ButtonState.Normal, cell.ButtonState);
                    Assert.False(control.IsHandleCreated);
                }
            }).Dispose();
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_MouseUpUnsharesRow_NullE_ThrowsNullReferenceException()
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Throws<NullReferenceException>(() => cell.MouseUpUnsharesRow(null));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsTheory]
        [MemberData(nameof(MouseDownUnsharesRow_ButtonLeftNullDataGridView_TestData))]
        public void DataGridViewHeaderCell_MouseUpUnsharesRow_ButtonLeftNullDataGridView_ThrowsNullReferenceException(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewHeaderCell();
            Assert.Throws<NullReferenceException>(() => cell.MouseUpUnsharesRow(e));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        public static IEnumerable<object[]> OnMouseDown_TestData()
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
        [MemberData(nameof(OnMouseDown_TestData))]
        public void DataGridViewHeaderCell_OnMouseDown_Invoke_Nop(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewHeaderCell();
            cell.OnMouseDown(e);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        public static IEnumerable<object[]> OnMouseDown_WithDataGridView_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), VisualStyleRenderer.IsSupported && enableHeadersVisualStyles ? ButtonState.Pushed : ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), ButtonState.Normal };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), ButtonState.Normal };
            }

            yield return new object[] { false, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), ButtonState.Normal };
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void DataGridViewHeaderCell_OnMouseDown_InvokeWithDataGridView_Nop()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke(() =>
            {
                foreach (object[] testData in OnMouseDown_WithDataGridView_TestData())
                {
                    bool enableHeadersVisualStyles = (bool)testData[0];
                    DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                    ButtonState expectedButtonState = (ButtonState)testData[2];

                    Application.EnableVisualStyles();

                    using var cellTemplate = new SubDataGridViewHeaderCell();
                    using var column = new DataGridViewColumn
                    {
                        CellTemplate = cellTemplate
                    };
                    using var control = new DataGridView
                    {
                        EnableHeadersVisualStyles = enableHeadersVisualStyles
                    };
                    control.Columns.Add(column);
                    SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                    cell.OnMouseDown(e);
                    Assert.Equal(expectedButtonState, cell.ButtonState);
                    Assert.False(control.IsHandleCreated);
                }
            }).Dispose();
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void DataGridViewHeaderCell_OnMouseDown_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke(() =>
            {
                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = true
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                var e = new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseDown(e));
                Assert.Equal(VisualStyleRenderer.IsSupported ? ButtonState.Pushed : ButtonState.Normal, cell.ButtonState);
            }).Dispose();
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_OnMouseDown_NullEWithDataGridView_ThrowsNullReferenceException()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<NullReferenceException>(() => cell.OnMouseDown(null));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_OnMouseEnter_Invoke_Nop(int rowIndex)
        {
            using var cell = new SubDataGridViewHeaderCell();
            cell.OnMouseEnter(rowIndex);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
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
        public void DataGridViewHeaderCell_OnMouseEnter_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            cell.OnMouseEnter(rowIndex);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_OnMouseLeave_Invoke_Nop(int rowIndex)
        {
            using var cell = new SubDataGridViewHeaderCell();
            cell.OnMouseLeave(rowIndex);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
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
        public void DataGridViewHeaderCell_OnMouseLeave_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, int rowIndex)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            cell.OnMouseLeave(rowIndex);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
            Assert.False(control.IsHandleCreated);
        }
        [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        [InlineData(true, -1)]
        [InlineData(true, 0)]
        [InlineData(false, -2)]
        [InlineData(false, -1)]
        [InlineData(false, 0)]
        [InlineData(false, 1)]
        public void DataGridViewHeaderCell_OnMouseLeave_InvokeWithDataGridViewMouseDown_Nop(bool enableHeadersVisualStylesParam, int rowIndexParam)
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke((enableHeadersVisualStylesString, rowIndexString) =>
            {
                bool enableHeadersVisualStyles = bool.Parse(enableHeadersVisualStylesString);
                int rowIndex = int.Parse(rowIndexString);

                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = enableHeadersVisualStyles
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
                cell.OnMouseLeave(rowIndex);
                Assert.Equal(ButtonState.Normal, cell.ButtonState);
                Assert.False(control.IsHandleCreated);
            }, enableHeadersVisualStylesParam.ToString(), rowIndexParam.ToString()).Dispose();
        }

        [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        [InlineData(-2)]
        [InlineData(1)]
        public void DataGridViewHeaderCell_OnMouseLeave_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException(int rowIndexParam)
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke((rowIndexString) =>
            {
                int rowIndex = int.Parse(rowIndexString);

                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = true
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
                Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseLeave(rowIndex));
                Assert.Equal(ButtonState.Normal, cell.ButtonState);
            }, rowIndexParam.ToString()).Dispose();
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMouseDown_TestData))]
        public void DataGridViewHeaderCell_OnMouseUp_Invoke_Nop(DataGridViewCellMouseEventArgs e)
        {
            using var cell = new SubDataGridViewHeaderCell();
            cell.OnMouseUp(e);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        public static IEnumerable<object[]> OnMouseUp_WithDataGridView_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
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
        [MemberData(nameof(OnMouseUp_WithDataGridView_TestData))]
        public void DataGridViewHeaderCell_OnMouseUp_InvokeWithDataGridView_Nop(bool enableHeadersVisualStyles, DataGridViewCellMouseEventArgs e)
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView
            {
                EnableHeadersVisualStyles = enableHeadersVisualStyles
            };
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            cell.OnMouseUp(e);
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnMouseUp_WithDataGridViewMouseDown_TestData()
        {
            foreach (bool enableHeadersVisualStyles in new bool[] { true, false })
            {
                ButtonState expectedButtonState1 = enableHeadersVisualStyles && VisualStyleRenderer.IsSupported ? ButtonState.Pushed : ButtonState.Normal;
                ButtonState expectedButtonState2 = enableHeadersVisualStyles && VisualStyleRenderer.IsSupported ? ButtonState.Normal : expectedButtonState1;
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, -1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(-1, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 0, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(1, 0, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), expectedButtonState2 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0)), expectedButtonState1 };
                yield return new object[] { enableHeadersVisualStyles, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 0)), expectedButtonState1 };
            }

            yield return new object[] { false, new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)), ButtonState.Normal };
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void DataGridViewHeaderCell_OnMouseUp_InvokeWithDataGridViewMouseDown_ReturnsExpected()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke(() =>
            {
                foreach (object[] testData in OnMouseUp_WithDataGridViewMouseDown_TestData())
                {
                    bool enableHeadersVisualStyles = (bool)testData[0];
                    DataGridViewCellMouseEventArgs e = (DataGridViewCellMouseEventArgs)testData[1];
                    ButtonState expectedButtonState = (ButtonState)testData[2];

                    Application.EnableVisualStyles();

                    using var cellTemplate = new SubDataGridViewHeaderCell();
                    using var column = new DataGridViewColumn
                    {
                        CellTemplate = cellTemplate
                    };
                    using var control = new DataGridView
                    {
                        EnableHeadersVisualStyles = enableHeadersVisualStyles
                    };
                    control.Columns.Add(column);
                    SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                    cell.OnMouseDown(new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0)));
                    cell.OnMouseUp(e);
                    Assert.Equal(expectedButtonState, cell.ButtonState);
                    Assert.False(control.IsHandleCreated);
                }
            }).Dispose();
        }

        [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
        public void DataGridViewHeaderCell_OnMouseUp_InvalidRowIndexVisualStyles_ThrowsArgumentOutOfRangeException()
        {
            // Run this from another thread as we call Application.EnableVisualStyles.
            RemoteExecutor.Invoke(() =>
            {
                Application.EnableVisualStyles();

                using var cellTemplate = new SubDataGridViewHeaderCell();
                using var column = new DataGridViewColumn
                {
                    CellTemplate = cellTemplate
                };
                using var control = new DataGridView
                {
                    EnableHeadersVisualStyles = true
                };
                control.Columns.Add(column);
                SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
                var e = new DataGridViewCellMouseEventArgs(0, 1, 0, 0, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>("rowIndex", () => cell.OnMouseUp(e));
                Assert.Equal(ButtonState.Normal, cell.ButtonState);
            }).Dispose();
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_OnMouseUp_NullEWithDataGridView_ThrowsNullReferenceException()
        {
            using var cellTemplate = new SubDataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            SubDataGridViewHeaderCell cell = (SubDataGridViewHeaderCell)control.Rows[0].Cells[0];
            Assert.Throws<NullReferenceException>(() => cell.OnMouseUp(null));
            Assert.Equal(ButtonState.Normal, cell.ButtonState);
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_Paint_NullCellStyle_ThrowsArgumentNullException()
        {
            using var cell = new SubDataGridViewHeaderCell();
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Throws<ArgumentNullException>("cellStyle", () => cell.Paint(graphics, Rectangle.Empty, Rectangle.Empty, -1, DataGridViewElementStates.None, null, null, null, null, null, DataGridViewPaintParts.All));
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_ToString_Invoke_ReturnsExpected()
        {
            using var cell = new DataGridViewHeaderCell();
            Assert.Equal("DataGridViewHeaderCell { ColumnIndex=-1, RowIndex=-1 }", cell.ToString());
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_ToString_InvokeWithRow_ReturnsExpected()
        {
            using var row = new DataGridViewRow();
            using var cell = new DataGridViewHeaderCell();
            row.Cells.Add(cell);
            Assert.Equal("DataGridViewHeaderCell { ColumnIndex=-1, RowIndex=-1 }", cell.ToString());
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_ToString_InvokeColumnHeaderCell_ReturnsExpected()
        {
            using var column = new DataGridViewColumn();
            using var cell = new DataGridViewColumnHeaderCell();
            column.HeaderCell = cell;
            Assert.Equal("DataGridViewColumnHeaderCell { ColumnIndex=-1 }", cell.ToString());
        }

        [WinFormsFact]
        public void DataGridViewHeaderCell_ToString_InvokeWithDataGridView_ReturnsExpected()
        {
            using var cellTemplate = new DataGridViewHeaderCell();
            using var column = new DataGridViewColumn
            {
                CellTemplate = cellTemplate
            };
            using var control = new DataGridView();
            control.Columns.Add(column);
            DataGridViewCell cell = control.Rows[0].Cells[0];
            Assert.Equal("DataGridViewHeaderCell { ColumnIndex=0, RowIndex=0 }", cell.ToString());
        }

        private class SubDataGridViewRowHeaderCell : DataGridViewRowHeaderCell
        {
            public new Size GetSize(int rowIndex) => base.GetSize(rowIndex);

            public new object GetValue(int rowIndex) => base.GetValue(rowIndex);
        }

        private class SubDataGridViewColumnHeaderCell : DataGridViewColumnHeaderCell
        {
            public new Size GetSize(int rowIndex) => base.GetSize(rowIndex);

            public new object GetValue(int rowIndex) => base.GetValue(rowIndex);
        }

        private class CustomStateDataGridViewHeaderCell : DataGridViewHeaderCell
        {
            public DataGridViewElementStates StateResult { get; set; }

            public override DataGridViewElementStates State => StateResult;
        }

        public class SubDataGridViewHeaderCell : DataGridViewHeaderCell
        {
            public new ButtonState ButtonState => base.ButtonState;

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new Size GetSize(int rowIndex) => base.GetSize(rowIndex);

            public new object GetValue(int rowIndex) => base.GetValue(rowIndex);

            public new bool MouseDownUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseDownUnsharesRow(e);

            public new bool MouseEnterUnsharesRow(int rowIndex) => base.MouseEnterUnsharesRow(rowIndex);

            public new bool MouseLeaveUnsharesRow(int rowIndex) => base.MouseLeaveUnsharesRow(rowIndex);

            public new bool MouseUpUnsharesRow(DataGridViewCellMouseEventArgs e) => base.MouseUpUnsharesRow(e);

            public new void OnMouseDown(DataGridViewCellMouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(int rowIndex) => base.OnMouseEnter(rowIndex);

            public new void OnMouseLeave(int rowIndex) => base.OnMouseLeave(rowIndex);

            public new void OnMouseUp(DataGridViewCellMouseEventArgs e) => base.OnMouseUp(e);

            public new void Paint(Graphics graphics,
                                  Rectangle clipBounds,
                                  Rectangle cellBounds,
                                  int rowIndex,
                                  DataGridViewElementStates dataGridViewElementState,
                                  object value,
                                  object formattedValue,
                                  string errorText,
                                  DataGridViewCellStyle cellStyle,
                                  DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                  DataGridViewPaintParts paintParts)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }
    }
}
