// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridViewSelectedCellCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_Properties_GetEmpty_ReturnsExpected()
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.Equal(0, collection.Count);
            Assert.False(collection.IsReadOnly);
            Assert.False(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_Properties_GetNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.Equal(2, collection.Count);
            Assert.False(collection.IsReadOnly);
            Assert.False(collection.IsSynchronized);
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_Item_GetNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.Equal(control.Rows[2].Cells[0], collection[0]);
            Assert.Equal(control.Rows[0].Cells[0], collection[1]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewSelectedCellCollection_Item_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(2)]
        public void DataGridViewSelectedCellCollection_Item_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_Clear_Invoke_ThrowsNotSupportedException()
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.Throws<NotSupportedException>(() => collection.Clear());
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_Contains_InvokeNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.True(collection.Contains(control.Rows[0].Cells[0]));
            Assert.False(collection.Contains(control.Rows[1].Cells[0]));
            Assert.True(collection.Contains(control.Rows[2].Cells[0]));
            Assert.False(collection.Contains(new SubDataGridViewCell()));
            Assert.False(collection.Contains(null));
        }

        public static IEnumerable<object[]> Contains_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new SubDataGridViewCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Contains_TestData))]
        public void DataGridViewSelectedCellCollection_Contains_InvokeEmpty_ReturnsFalse(DataGridViewCell dataGridViewCell)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.False(collection.Contains(dataGridViewCell));
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_CopyTo_InvokeEmpty_Success()
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, 2, 3 }, array);
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_CopyTo_InvokeNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            var array = new object[] { 1, 2, 3 };
            collection.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, control.Rows[2].Cells[0], control.Rows[0].Cells[0] }, array);
        }

        public static IEnumerable<object[]> Insert_TestData()
        {
            foreach (int index in new int[] { -1, 0, 1 })
            {
                yield return new object[] { index, null };
                yield return new object[] { index, new SubDataGridViewCell() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Insert_TestData))]
        public void DataGridViewSelectedCellCollection_Insert_Invoke_ThrowsNotSupportedException(int index, DataGridViewCell dataGridViewCell)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            Assert.Throws<NotSupportedException>(() => collection.Insert(index, dataGridViewCell));
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListProperties_GetEmpty_ReturnsExpected()
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Equal(0, iList.Count);
            Assert.True(iList.IsFixedSize);
            Assert.True(iList.IsReadOnly);
            Assert.False(iList.IsSynchronized);
            Assert.Same(collection, iList.SyncRoot);
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListProperties_GetNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Equal(2, iList.Count);
            Assert.True(iList.IsFixedSize);
            Assert.True(iList.IsReadOnly);
            Assert.False(iList.IsSynchronized);
            Assert.Same(collection, iList.SyncRoot);
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListItem_GetNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Equal(control.Rows[2].Cells[0], iList[0]);
            Assert.Equal(control.Rows[0].Cells[0], iList[1]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewSelectedCellCollection_IListItem_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(2)]
        public void DataGridViewSelectedCellCollection_IListItem_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
        }

        public static IEnumerable<object[]> IListItem_SetTestData()
        {
            yield return new object[] { 0, null };
            yield return new object[] { -1, new object() };
            yield return new object[] { 1, new SubDataGridViewCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListItem_SetTestData))]
        public void DataGridViewSelectedCellCollection_IListItem_Set_ThrowsNotSupportedException(int index, object value)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<NotSupportedException>(() => iList[index] = value);
        }

        public static IEnumerable<object[]> IListAdd_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new SubDataGridViewCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListAdd_TestData))]
        public void DataGridViewSelectedCellCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<NotSupportedException>(() => iList.Add(value));
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListClear_Invoke_ThrowsNotSupportedException()
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<NotSupportedException>(() => iList.Clear());
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListCopyTo_InvokeEmpty_Success()
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            var array = new object[] { 1, 2, 3 };
            iList.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, 2, 3 }, array);
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListCopyTo_InvokeNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            var array = new object[] { 1, 2, 3 };
            iList.CopyTo(array, 1);
            Assert.Equal(new object[] { 1, control.Rows[2].Cells[0], control.Rows[0].Cells[0] }, array);
        }

        public static IEnumerable<object[]> IListContains_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new SubDataGridViewCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListContains_TestData))]
        public void DataGridViewSelectedCellCollection_IListContains_InvokeEmpty_ReturnsFalse(object value)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.False(iList.Contains(value));
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListContains_InvokeNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.True(iList.Contains(control.Rows[0].Cells[0]));
            Assert.False(iList.Contains(control.Rows[1].Cells[0]));
            Assert.True(iList.Contains(control.Rows[2].Cells[0]));
            Assert.False(iList.Contains(new SubDataGridViewCell()));
            Assert.False(iList.Contains(null));
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListGetEnumerator_InvokeEmpty_Success()
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            IEnumerator enumerator = iList.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                enumerator.Reset();
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            }
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListGetEnumerator_InvokeNotEmpty_Success()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            IEnumerator enumerator = iList.GetEnumerator();
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(control.Rows[2].Cells[0], enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Assert.Same(control.Rows[0].Cells[0], enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                enumerator.Reset();
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);
            }
        }

        public static IEnumerable<object[]> IListIndexOf_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new SubDataGridViewCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListIndexOf_TestData))]
        public void DataGridViewSelectedCellCollection_IListIndexOf_InvokeEmpty_ReturnsMinusOne(object value)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Equal(-1, iList.IndexOf(value));
        }

        [WinFormsFact]
        public void DataGridViewSelectedCellCollection_IListIndexOf_InvokeNotEmpty_ReturnsExpected()
        {
            using var control = new DataGridView
            {
                RowCount = 3,
                ColumnCount = 1,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            control.Rows[0].Cells[0].Selected = true;
            control.Rows[1].Cells[0].Selected = false;
            control.Rows[2].Cells[0].Selected = true;

            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Equal(1, iList.IndexOf(control.Rows[0].Cells[0]));
            Assert.Equal(-1, iList.IndexOf(control.Rows[1].Cells[0]));
            Assert.Equal(0, iList.IndexOf(control.Rows[2].Cells[0]));
            Assert.Equal(-1, iList.IndexOf(new SubDataGridViewCell()));
            Assert.Equal(-1, iList.IndexOf(null));
        }

        public static IEnumerable<object[]> IListInsert_TestData()
        {
            foreach (int index in new int[] { -1, 0, 1 })
            {
                yield return new object[] { index, null };
                yield return new object[] { index, new object() };
                yield return new object[] { index, new SubDataGridViewCell() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(IListInsert_TestData))]
        public void DataGridViewSelectedCellCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<NotSupportedException>(() => iList.Insert(index, value));
        }

        public static IEnumerable<object[]> IListRemove_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new SubDataGridViewCell() };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListRemove_TestData))]
        public void DataGridViewSelectedCellCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<NotSupportedException>(() => iList.Remove(value));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void DataGridViewSelectedCellCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
        {
            using var control = new DataGridView();
            DataGridViewSelectedCellCollection collection = control.SelectedCells;
            IList iList = collection;
            Assert.Throws<NotSupportedException>(() => iList.RemoveAt(index));
        }

        private class SubDataGridViewCell : DataGridViewCell
        {
        }
    }
}
