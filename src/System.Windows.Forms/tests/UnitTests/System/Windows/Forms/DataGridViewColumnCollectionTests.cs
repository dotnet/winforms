// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewColumnCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewColumnCollection_Ctor_DataGridView()
        {
            using var control = new DataGridView();
            var collection = new SubDataGridViewColumnCollection(control);
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.Same(control, collection.DataGridView);
            Assert.False(collection.IsReadOnly);
            Assert.False(collection.IsSynchronized);
            Assert.IsType<ArrayList>(collection.List);
            Assert.Empty(collection.List);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Ctor_NullOwner_ThrowsArgumentNullException()
        {
            using var control = new DataGridView();
#if false
            Assert.Throws<ArgumentNullException>("control", () => new SubDataGridViewColumnCollection(null));
#endif
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_Invoke_Success()
        {
            using var control = new DataGridView();
            var collection = new DataGridViewColumnCollection(control);
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell());
            using var column2 = new DataGridViewColumn();

            // Add one.
            collection.Add(column1);
            Assert.Equal(new DataGridViewColumn[] { column1 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.False(control.IsHandleCreated);

            // Add another.
            collection.Add(column2);
            Assert.Equal(new DataGridViewColumn[] { column1, column2 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.Same(control, column2.DataGridView);
            Assert.Equal(1, column2.Index);
            Assert.Equal(1, column2.DisplayIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_InvokeColumns_Success()
        {
            using var control = new DataGridView();
            DataGridViewColumnCollection collection = control.Columns;
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell());
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell());

            // Add one.
            collection.Add(column1);
            Assert.Equal(new DataGridViewColumn[] { column1 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.False(control.IsHandleCreated);

            // Add another.
            collection.Add(column2);
            Assert.Equal(new DataGridViewColumn[] { column1, column2 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.Same(control, column2.DataGridView);
            Assert.Equal(1, column2.Index);
            Assert.Equal(1, column2.DisplayIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewSelectionMode.CellSelect, DataGridViewColumnSortMode.Automatic)]
        [InlineData(DataGridViewSelectionMode.CellSelect, DataGridViewColumnSortMode.NotSortable)]
        [InlineData(DataGridViewSelectionMode.CellSelect, DataGridViewColumnSortMode.Programmatic)]
        [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect, DataGridViewColumnSortMode.NotSortable)]
        [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect, DataGridViewColumnSortMode.Programmatic)]
        [InlineData(DataGridViewSelectionMode.FullColumnSelect, DataGridViewColumnSortMode.NotSortable)]
        [InlineData(DataGridViewSelectionMode.FullColumnSelect, DataGridViewColumnSortMode.Programmatic)]
        [InlineData(DataGridViewSelectionMode.FullRowSelect, DataGridViewColumnSortMode.Automatic)]
        [InlineData(DataGridViewSelectionMode.FullRowSelect, DataGridViewColumnSortMode.NotSortable)]
        [InlineData(DataGridViewSelectionMode.FullRowSelect, DataGridViewColumnSortMode.Programmatic)]
        [InlineData(DataGridViewSelectionMode.RowHeaderSelect, DataGridViewColumnSortMode.Automatic)]
        [InlineData(DataGridViewSelectionMode.RowHeaderSelect, DataGridViewColumnSortMode.NotSortable)]
        [InlineData(DataGridViewSelectionMode.RowHeaderSelect, DataGridViewColumnSortMode.Programmatic)]
        public void DataGridViewColumnCollection_Add_CustomColumnSortModeAndSelectionMode_Success(DataGridViewSelectionMode selectionMode, DataGridViewColumnSortMode sortMode)
        {
            using var control = new DataGridView
            {
                SelectionMode = selectionMode
            };
            DataGridViewColumnCollection collection = control.Columns;
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                SortMode = sortMode
            };
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                SortMode = sortMode
            };

            // Add one.
            collection.Add(column1);
            Assert.Equal(new DataGridViewColumn[] { column1 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.False(control.IsHandleCreated);

            // Add another.
            collection.Add(column2);
            Assert.Equal(new DataGridViewColumn[] { column1, column2 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.Same(control, column2.DataGridView);
            Assert.Equal(1, column2.Index);
            Assert.Equal(1, column2.DisplayIndex);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Add_CustomAutoSizeMode_TestData()
        {
            foreach (DataGridViewAutoSizeColumnsMode columnsAutoSizeMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnsMode)))
            {
                foreach (DataGridViewAutoSizeColumnMode autoSizeMode in Enum.GetValues(typeof(DataGridViewAutoSizeColumnMode)))
                {
                    bool canBeFrozen = autoSizeMode != DataGridViewAutoSizeColumnMode.Fill && !(columnsAutoSizeMode == DataGridViewAutoSizeColumnsMode.Fill && autoSizeMode == DataGridViewAutoSizeColumnMode.NotSet);
                    if (canBeFrozen)
                    {
                        yield return new object[] { true, true, true, columnsAutoSizeMode, autoSizeMode };
                    }

                    yield return new object[] { true, true, false, columnsAutoSizeMode, autoSizeMode };
                    yield return new object[] { true, false, true, columnsAutoSizeMode, autoSizeMode };
                    yield return new object[] { true, false, false, columnsAutoSizeMode, autoSizeMode };
                    if (autoSizeMode != DataGridViewAutoSizeColumnMode.ColumnHeader && !(columnsAutoSizeMode == DataGridViewAutoSizeColumnsMode.ColumnHeader && autoSizeMode == DataGridViewAutoSizeColumnMode.NotSet))
                    {
                        if (canBeFrozen)
                        {
                            yield return new object[] { false, true, true, columnsAutoSizeMode, autoSizeMode };
                        }
                        yield return new object[] { false, true, false, columnsAutoSizeMode, autoSizeMode };
                    }
                    yield return new object[] { false, false, true, columnsAutoSizeMode, autoSizeMode };
                    yield return new object[] { false, false, false, columnsAutoSizeMode, autoSizeMode };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_CustomAutoSizeMode_TestData))]
        public void DataGridViewColumnCollection_Add_CustomAutoSizeMode_Success(bool columnHeadersVisible, bool visible, bool frozen, DataGridViewAutoSizeColumnsMode columnsAutoSizeMode, DataGridViewAutoSizeColumnMode autoSizeMode)
        {
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = columnsAutoSizeMode,
                ColumnHeadersVisible = columnHeadersVisible
            };
            DataGridViewColumnCollection collection = control.Columns;
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                AutoSizeMode = autoSizeMode,
                Frozen = frozen,
                Visible = visible
            };
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                AutoSizeMode = autoSizeMode,
                Frozen = frozen,
                Visible = visible
            };

            // Add one.
            collection.Add(column1);
            Assert.Equal(new DataGridViewColumn[] { column1 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.False(control.IsHandleCreated);

            // Add another.
            collection.Add(column2);
            Assert.Equal(new DataGridViewColumn[] { column1, column2 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.Same(control, column2.DataGridView);
            Assert.Equal(1, column2.Index);
            Assert.Equal(1, column2.DisplayIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_InvokeFrozen_Success()
        {
            using var control = new DataGridView();
            DataGridViewColumnCollection collection = control.Columns;
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                Frozen = true
            };
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                Frozen = true
            };

            // Add one.
            collection.Add(column1);
            Assert.Equal(new DataGridViewColumn[] { column1 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.False(control.IsHandleCreated);

            // Add another.
            collection.Add(column2);
            Assert.Equal(new DataGridViewColumn[] { column1, column2 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.Same(control, column2.DataGridView);
            Assert.Equal(1, column2.Index);
            Assert.Equal(1, column2.DisplayIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void DataGridViewColumnCollection_Add_CustomDisplayIndex_Success(int displayIndex)
        {
            using var control = new DataGridView();
            DataGridViewColumnCollection collection = control.Columns;
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                DisplayIndex = displayIndex
            };
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                DisplayIndex = 0
            };
            using var column3 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                DisplayIndex = 1
            };

            // Add one.
            collection.Add(column1);
            Assert.Equal(new DataGridViewColumn[] { column1 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(0, column1.DisplayIndex);
            Assert.False(control.IsHandleCreated);

            // Add another.
            collection.Add(column2);
            Assert.Equal(new DataGridViewColumn[] { column1, column2 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(1, column1.DisplayIndex);
            Assert.Same(control, column2.DataGridView);
            Assert.Equal(1, column2.Index);
            Assert.Equal(0, column2.DisplayIndex);
            Assert.False(control.IsHandleCreated);

            // Add another.
            collection.Add(column3);
            Assert.Equal(new DataGridViewColumn[] { column1, column2, column3 }, collection.Cast<DataGridViewColumn>());
            Assert.Same(control, column1.DataGridView);
            Assert.Equal(0, column1.Index);
            Assert.Equal(2, column1.DisplayIndex);
            Assert.Same(control, column2.DataGridView);
            Assert.Equal(1, column2.Index);
            Assert.Equal(0, column2.DisplayIndex);
            Assert.Same(control, column3.DataGridView);
            Assert.Equal(2, column3.Index);
            Assert.Equal(1, column3.DisplayIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_AddWithHeaderCell_Invoke_Success()
        {
            using var control = new DataGridView();
            DataGridViewColumnCollection collection = control.Columns;
            using var headerCell = new DataGridViewColumnHeaderCell();
            using var column = new DataGridViewColumn(new SubDataGridViewCell())
            {
                HeaderCell = headerCell
            };

            // Add one.
            collection.Add(column);
            Assert.Same(control, column.DataGridView);
            Assert.Equal(0, column.Index);
            Assert.Equal(0, column.DisplayIndex);
            Assert.Same(control, headerCell.DataGridView);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_NullDataGridViewColumn_ThrowsArgumentNullException()
        {
            using var control = new DataGridView();
            var collection = new DataGridViewColumnCollection(control);
            Assert.Throws<ArgumentNullException>("dataGridViewColumn", () => collection.Add(null));
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_NoDimensionChangeAllowed_ThrowsInvalidOperationException()
        {
            using var control = new SubDataGridView();
            var collection = new DataGridViewColumnCollection(control);
            int callCount = 0;
            control.RowValidating += (sender, e) =>
            {
                var column = new DataGridViewColumn(new SubDataGridViewCell());
                Assert.Throws<InvalidOperationException>(() => collection.Add(null));
                Assert.Throws<InvalidOperationException>(() => collection.Add(column));
                callCount++;
            };
            control.OnRowValidating(null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_InDisplayIndexAdjustment_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            var collection = new DataGridViewColumnCollection(control);
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell());
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell());
            control.Columns.Add(column1);
            control.Columns.Add(column2);

            int callCount = 0;
            control.ColumnDisplayIndexChanged += (sender, e) =>
            {
                var column = new DataGridViewColumn(new SubDataGridViewCell());
                Assert.Throws<InvalidOperationException>(() => collection.Add(null));
                Assert.Throws<InvalidOperationException>(() => collection.Add(column));
                callCount++;
            };
            column1.DisplayIndex = 1;
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_ColumnHasDataGridView_ThrowsInvalidOperationException()
        {
            using var control1 = new DataGridView();
            using var control2 = new DataGridView();
            var collection = new DataGridViewColumnCollection(control1);
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell());
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell());
            control1.Columns.Add(column1);
            control2.Columns.Add(column2);

            Assert.Throws<InvalidOperationException>(() => collection.Add(column1));
            Assert.Throws<InvalidOperationException>(() => collection.Add(column2));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewSelectionMode.FullColumnSelect)]
        [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect)]
        public void DataGridViewColumnCollection_Add_ColumnAutomaticSortModeColumnSelectionMode_ThrowsInvalidOperationException(DataGridViewSelectionMode selectionMode)
        {
            using var control = new DataGridView
            {
                SelectionMode = selectionMode
            };
            var collection = new DataGridViewColumnCollection(control);
            using var column = new DataGridViewColumn(new SubDataGridViewCell())
            {
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            Assert.Throws<InvalidOperationException>(() => collection.Add(column));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewSelectionMode.FullColumnSelect)]
        [InlineData(DataGridViewSelectionMode.ColumnHeaderSelect)]
        public void DataGridViewColumnCollection_Add_ColumnAutomaticSortModeColumnSelectionModeInInitialization_ThrowsInvalidOperationException(DataGridViewSelectionMode selectionMode)
        {
            using var control = new DataGridView
            {
                SelectionMode = selectionMode
            };
            ISupportInitialize iSupportInitialize = (ISupportInitialize)control;
            DataGridViewColumnCollection collection = control.Columns;
            using var column = new DataGridViewColumn(new SubDataGridViewCell())
            {
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            iSupportInitialize.BeginInit();
            collection.Add(column);

            // End init.
            Assert.Throws<InvalidOperationException>(() => iSupportInitialize.EndInit());
            Assert.Equal(DataGridViewSelectionMode.RowHeaderSelect, control.SelectionMode);
        }

        [WinFormsTheory]
        [InlineData(DataGridViewAutoSizeColumnsMode.None, DataGridViewAutoSizeColumnMode.ColumnHeader)]
        [InlineData(DataGridViewAutoSizeColumnsMode.ColumnHeader, DataGridViewAutoSizeColumnMode.NotSet)]
        public void DataGridViewColumnCollection_Add_VisibleColumnHeadersNotVisibleInvalidAutoSize_ThrowsInvalidOperationException(DataGridViewAutoSizeColumnsMode autoSizeColumnsMode, DataGridViewAutoSizeColumnMode autoSizeMode)
        {
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = autoSizeColumnsMode,
                ColumnHeadersVisible = false
            };
            var collection = new DataGridViewColumnCollection(control);
            using var column = new DataGridViewColumn(new SubDataGridViewCell())
            {
                AutoSizeMode = autoSizeMode
            };
            Assert.Throws<InvalidOperationException>(() => control.Columns.Add(column));
        }

        [WinFormsTheory]
        [InlineData(DataGridViewAutoSizeColumnsMode.None, DataGridViewAutoSizeColumnMode.Fill)]
        [InlineData(DataGridViewAutoSizeColumnsMode.Fill, DataGridViewAutoSizeColumnMode.NotSet)]
        public void DataGridViewColumnCollection_Add_VisibleFrozenColumnInvalidAutoSize_ThrowsInvalidOperationException(DataGridViewAutoSizeColumnsMode autoSizeColumnsMode, DataGridViewAutoSizeColumnMode autoSizeMode)
        {
            using var control = new DataGridView
            {
                AutoSizeColumnsMode = autoSizeColumnsMode
            };
            var collection = new DataGridViewColumnCollection(control);
            using var column = new DataGridViewColumn(new SubDataGridViewCell())
            {
                AutoSizeMode = autoSizeMode,
                Frozen = true
            };
            Assert.Throws<InvalidOperationException>(() => control.Columns.Add(column));
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_FillWeightsOverflow_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell());
            control.Columns.Add(column1);
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                FillWeight = ushort.MaxValue - 99
            };
            Assert.Throws<InvalidOperationException>(() => control.Columns.Add(column2));
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_FrozenPreviousColumnNotFrozen_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell());
            control.Columns.Add(column1);
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                Frozen = true
            };
            Assert.Throws<InvalidOperationException>(() => control.Columns.Add(column2));
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_FrozenPreviousColumnNotFrozenDisplayIndex_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                DisplayIndex = 1
            };
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                Frozen = true,
                DisplayIndex = 0,
            };
            control.Columns.Add(column1);
            control.Columns.Add(column2);
            using var column3 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                Frozen = true,
                DisplayIndex = 1
            };
            control.Columns.Add(column3);
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_NotFrozenPreviousColumnFrozenDisplayIndex_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            using var column1 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                Frozen = true,
                DisplayIndex = 1
            };
            using var column2 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                Frozen = true,
                DisplayIndex = 0
            };
            control.Columns.Add(column1);
            control.Columns.Add(column2);
            using var column3 = new DataGridViewColumn(new SubDataGridViewCell())
            {
                DisplayIndex = 1
            };
            Assert.Throws<InvalidOperationException>(() => control.Columns.Add(column3));
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_ColumnHasNoCellTemplateEmpty_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView();
            using var column = new DataGridViewColumn();
            Assert.Throws<InvalidOperationException>(() => control.Columns.Add(column));
        }

        [WinFormsFact]
        public void DataGridViewColumnCollection_Add_ColumnHasNoCellTemplateNotEmpty_ThrowsInvalidOperationException()
        {
            using var control = new DataGridView
            {
                RowCount = 1
            };
            using var column = new DataGridViewColumn();
            Assert.Throws<InvalidOperationException>(() => control.Columns.Add(column));
        }

        private class SubDataGridView : DataGridView
        {
            public new void OnRowValidating(DataGridViewCellCancelEventArgs e) => base.OnRowValidating(e);
        }

        private class SubDataGridViewCell : DataGridViewCell
        {
        }

        private class SubDataGridViewColumnCollection : DataGridViewColumnCollection
        {
            public SubDataGridViewColumnCollection(DataGridView control) : base(control)
            {
            }

            public new DataGridView DataGridView => base.DataGridView;

            public new ArrayList List => base.List;
        }
    }
}
