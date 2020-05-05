// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewColumnTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewColumn_Ctor_Default()
        {
            using var column = new SubDataGridViewColumn();
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
            using var column = new SubDataGridViewColumn(cellTemplate);
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewAutoSizeColumnMode))]
        public void DataGridViewColumn_AutoSizeMode_Set_GetReturnsExpected(DataGridViewAutoSizeColumnMode value)
        {
            using var column = new DataGridViewColumn
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DataGridViewAutoSizeColumnMode))]
        public void DataGridViewColumn_AutoSizeMode_SetNotVisible_GetReturnsExpected(DataGridViewAutoSizeColumnMode value)
        {
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = parentMode,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = parentMode,
                ColumnHeadersVisible = columnHeadersVisible
            };
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView
            {
                ColumnHeadersVisible = false
            };
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            control.Columns.Add(column);

            Assert.Throws<InvalidOperationException>(() => column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader);
            Assert.Equal(DataGridViewAutoSizeColumnMode.NotSet, column.AutoSizeMode);
            Assert.Equal(DataGridViewAutoSizeColumnMode.None, column.InheritedAutoSizeMode);
        }

        [WinFormsFact]
        public void DataGridViewColumn_AutoSizeMode_SetNotSetWithDataGridViewColumnHeadersNotVisible_GetReturnsExpected()
        {
            using var control = new DataGridView
            {
                ColumnHeadersVisible = false
            };
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader,
                ColumnHeadersVisible = false
            };
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView();
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView();
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            using var column = new DataGridViewColumn
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
            using var column = new DataGridViewColumn
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
            using var column = new DataGridViewColumn
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
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
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
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridViewAutoSizeColumnMode))]
        public void DataGridViewColumn_AutoSizeMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(DataGridViewAutoSizeColumnMode value)
        {
            using var column = new DataGridViewColumn();
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
            using var column = new SubDataGridViewColumn
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
            using var column = new DataGridViewColumn
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
            using var oldValue = new ContextMenuStrip();
            using var column = new DataGridViewColumn
            {
                ContextMenuStrip = oldValue
            };

            column.ContextMenuStrip = value;
            Assert.Same(value, column.ContextMenuStrip);

            // Set same.
            column.ContextMenuStrip = value;
            Assert.Same(value, column.ContextMenuStrip);
        }

        [WinFormsFact]
        public void DataGridViewColumn_ContextMenuStrip_SetDisposeNew_RemovesContextMenuStrip()
        {
            using var menu = new ContextMenuStrip();
            using var control = new DataGridViewColumn
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
            using var menu1 = new ContextMenuStrip();
            using var menu2 = new ContextMenuStrip();
            using var control = new DataGridViewColumn
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
            using var dataGridView = new DataGridView();
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            dataGridView.Columns.Add(column);
            int callCount = 0;
            DataGridViewColumnEventHandler handler = (sender, e) =>
            {
                Assert.Same(dataGridView, sender);
                Assert.Same(column, e.Column);
                callCount++;
            };
            dataGridView.ColumnContextMenuStripChanged += handler;

            // Set different.
            using var menu1 = new ContextMenuStrip();
            column.ContextMenuStrip = menu1;
            Assert.Same(menu1, column.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set same.
            column.ContextMenuStrip = menu1;
            Assert.Same(menu1, column.ContextMenuStrip);
            Assert.Equal(1, callCount);

            // Set different.
            using var menu2 = new ContextMenuStrip();
            column.ContextMenuStrip = menu2;
            Assert.Same(menu2, column.ContextMenuStrip);
            Assert.Equal(2, callCount);

            // Set null.
            column.ContextMenuStrip = null;
            Assert.Null(column.ContextMenuStrip);
            Assert.Equal(3, callCount);

            // Remove handler.
            dataGridView.ColumnContextMenuStripChanged -= handler;
            column.ContextMenuStrip = menu1;
            Assert.Same(menu1, column.ContextMenuStrip);
            Assert.Equal(3, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewColumn_DataPropertyName_SetWithoutDataGridView_GetReturnsExpected(string value, string expected)
        {
            var column = new DataGridViewColumn
            {
                DataPropertyName = value
            };
            Assert.Equal(expected, column.DataPropertyName);

            // Set same.
            column.DataPropertyName = value;
            Assert.Equal(expected, column.DataPropertyName);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void DataGridViewColumn_DataPropertyName_SetWithDataGridView_GetReturnsExpected(string value, string expected)
        {
            using var control = new DataGridView();
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            control.Columns.Add(column);

            column.DataPropertyName = value;
            Assert.Equal(expected, column.DataPropertyName);

            // Set same.
            column.DataPropertyName = value;
            Assert.Equal(expected, column.DataPropertyName);
        }

        [WinFormsFact]
        public void DataGridViewColumn_DataPropertyName_SetWithHandler_CallsColumnDataPropertyNameChanged()
        {
            using var control = new DataGridView();
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
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

            var style1 = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter };
            var style2 = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.BottomLeft };
            yield return new object[] { style1, style1 };
            yield return new object[] { style2, style2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(DefaultCellStyle_Set_TestData))]
        public void DataGridViewColumn_DefaultCellStyle_Set_GetReturnsExpected(DataGridViewCellStyle value, DataGridViewCellStyle expected)
        {
            using var column = new DataGridViewColumn
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
            var oldValue = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
            using var column = new DataGridViewColumn
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

        [WinFormsFact]
        public void DataGridViewColumn_DefaultCellStyle_SetWithDataGridView_CallsColumnDefaultCellStyleChanged()
        {
            using var dataGridView = new DataGridView();
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            dataGridView.Columns.Add(column);

            int callCount = 0;
            DataGridViewColumnEventHandler handler = (sender, e) =>
            {
                Assert.Same(dataGridView, sender);
                Assert.Same(column, e.Column);
                callCount++;
            };
            dataGridView.ColumnDefaultCellStyleChanged += handler;

            var style1 = new DataGridViewCellStyle
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
            var style2 = new DataGridViewCellStyle
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
            dataGridView.ColumnDefaultCellStyleChanged -= handler;
            column.DefaultCellStyle = style1;
            Assert.Equal(style1, column.DefaultCellStyle);
            Assert.True(column.HasDefaultCellStyle);
            Assert.Equal(5, callCount);
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
            using var column = new DataGridViewColumn
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
            using var dataGridView = new DataGridView
            {
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader
            };
            using var column = new DataGridViewColumn
            {
                Visible = visible,
                AutoSizeMode = autoSizeMode,
                CellTemplate = new SubDataGridViewCell()
            };
            dataGridView.Columns.Add(column);

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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewColumn_Frozen_SetWithPreviousColumns_SetsToFrozen(bool previousVisible)
        {
            using var dataGridView = new DataGridView();
            using var column1 = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            using var column2 = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell(),
                Visible = previousVisible
            };
            using var column3 = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            using var column4 = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            dataGridView.Columns.Add(column1);
            dataGridView.Columns.Add(column2);
            dataGridView.Columns.Add(column3);
            dataGridView.Columns.Add(column4);

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
            using var dataGridView = new DataGridView();
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            dataGridView.Columns.Add(column);

            int callCount = 0;
            DataGridViewColumnStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(column, e.Column);
                Assert.Equal(DataGridViewElementStates.Frozen, e.StateChanged);
            };
            dataGridView.ColumnStateChanged += handler;

            // Set true.
            column.Frozen = true;
            Assert.True(column.Frozen);
            Assert.Equal(1, callCount);

            // Set same.
            column.Frozen = true;
            Assert.True(column.Frozen);
            Assert.Equal(1, callCount);

            // Set different.
            column.Frozen = false;
            Assert.False(column.Frozen);
            Assert.Equal(2, callCount);

            // Remove handler.
            dataGridView.ColumnStateChanged -= handler;
            column.Frozen = true;
            Assert.True(column.Frozen);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void DataGridViewColumn_HeaderCell_Get_ReturnsExpected()
        {
            using var column = new SubDataGridViewColumn();
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
            using var column = new SubDataGridViewColumn();
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
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = mode
            };
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            control.Columns.Add(column);
            Assert.Equal(expected, column.InheritedAutoSizeMode);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DataGridViewColumn_ReadOnly_Set_GetReturnsExpected(bool value)
        {
            using var column = new DataGridViewColumn
            {
                ReadOnly = value
            };
            Assert.Equal(value, column.ReadOnly);

            // Set same.
            column.ReadOnly = value;
            Assert.Equal(value, column.ReadOnly);

            // Set different.
            column.ReadOnly = !value;
            Assert.Equal(!value, column.ReadOnly);
        }

        [WinFormsTheory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void DataGridViewColumn_ReadOnly_SetWithDataGridView_GetReturnsExpected(bool dataGridViewReadOnly, bool value)
        {
            using var dataGridView = new DataGridView();
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            dataGridView.Columns.Add(column);
            dataGridView.ReadOnly = dataGridViewReadOnly;

            column.ReadOnly = value;
            Assert.Equal(dataGridViewReadOnly || value, column.ReadOnly);

            // Set same.
            column.ReadOnly = value;
            Assert.Equal(dataGridViewReadOnly || value, column.ReadOnly);

            // Set different.
            column.ReadOnly = !value;
            Assert.Equal(dataGridViewReadOnly || !value, column.ReadOnly);

            dataGridView.ReadOnly = false;
            Assert.Equal(!dataGridViewReadOnly && !value, column.ReadOnly);
        }

        [WinFormsFact]
        public void DataGridViewColumn_ReadOnly_SetWithDataGridView_CallsColumnStateChanged()
        {
            using var dataGridView = new DataGridView();
            using var column = new DataGridViewColumn
            {
                CellTemplate = new SubDataGridViewCell()
            };
            dataGridView.Columns.Add(column);

            int callCount = 0;
            DataGridViewColumnStateChangedEventHandler handler = (sender, e) =>
            {
                callCount++;
                Assert.Same(dataGridView, sender);
                Assert.Same(column, e.Column);
                Assert.Equal(DataGridViewElementStates.ReadOnly, e.StateChanged);
            };
            dataGridView.ColumnStateChanged += handler;

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
            dataGridView.ColumnStateChanged -= handler;
            column.ReadOnly = true;
            Assert.True(column.ReadOnly);
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
            using var column = new SubDataGridViewColumn();
            Assert.Equal(-1, column.GetPreferredWidth(autoSizeColumnMode, fixedHeight));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewAutoSizeColumnMode.NotSet)]
        [InlineData(DataGridViewAutoSizeColumnMode.None)]
        [InlineData(DataGridViewAutoSizeColumnMode.Fill)]
        public void DataGridViewColumn_GetPreferredWidth_NotApplicableAutoSizeColumnMode_ThrowsArgumentException(DataGridViewAutoSizeColumnMode autoSizeColumnMode)
        {
            using var column = new SubDataGridViewColumn();
            Assert.Throws<ArgumentException>(null, () => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: true));
            Assert.Throws<ArgumentException>(null, () => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: false));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DataGridViewAutoSizeColumnMode))]
        public void DataGridViewColumn_GetPreferredWidth_NotApplicableAutoSizeColumnMode_ThrowsInvalidEnumArgumentException(DataGridViewAutoSizeColumnMode autoSizeColumnMode)
        {
            using var column = new SubDataGridViewColumn();
            Assert.Throws<InvalidEnumArgumentException>("autoSizeColumnMode", () => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: true));
            Assert.Throws<InvalidEnumArgumentException>("autoSizeColumnMode", () => column.GetPreferredWidth(autoSizeColumnMode, fixedHeight: false));
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { new DataGridViewColumn(), "DataGridViewColumn { Name=, Index=-1 }" };
            yield return new object[] { new DataGridViewColumn { Name = "name" }, "DataGridViewColumn { Name=name, Index=-1 }" };
        }

        [WinFormsTheory]
        [MemberData(nameof(ToString_TestData))]
        public void DataGridViewColumn_ToString_Invoke_ReturnsExpected(DataGridViewColumn column, string expected)
        {
            Assert.Equal(expected, column.ToString());
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
}
