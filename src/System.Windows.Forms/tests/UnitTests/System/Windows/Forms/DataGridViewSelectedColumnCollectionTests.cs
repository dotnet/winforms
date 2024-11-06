// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class DataGridViewSelectedColumnCollectionTests
{
    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_Properties_GetEmpty_ReturnsExpected()
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_Properties_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.Equal(2, collection.Count);
        Assert.False(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_Item_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.Equal(control.Columns[2], collection[0]);
        Assert.Equal(control.Columns[0], collection[1]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void DataGridViewSelectedColumnCollection_Item_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    public void DataGridViewSelectedColumnCollection_Item_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_Clear_Invoke_ThrowsNotSupportedException()
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.Throws<NotSupportedException>(collection.Clear);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_Contains_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.True(collection.Contains(control.Columns[0]));
        Assert.False(collection.Contains(control.Columns[1]));
        Assert.True(collection.Contains(control.Columns[2]));
        Assert.False(collection.Contains(new DataGridViewColumn()));
        Assert.False(collection.Contains(null));
    }

    public static IEnumerable<object[]> Contains_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new DataGridViewColumn() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Contains_TestData))]
    public void DataGridViewSelectedColumnCollection_Contains_InvokeEmpty_ReturnsFalse(DataGridViewColumn dataGridViewColumn)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.False(collection.Contains(dataGridViewColumn));
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_CopyTo_InvokeEmpty_Success()
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        object[] array = [1, 2, 3];
        collection.CopyTo(array, 1);
        Assert.Equal([1, 2, 3], array);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_CopyTo_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        object[] array = [1, 2, 3];
        collection.CopyTo(array, 1);
        Assert.Equal([1, control.Columns[2], control.Columns[0]], array);
    }

    public static IEnumerable<object[]> Insert_TestData()
    {
        foreach (int index in new int[] { -1, 0, 1 })
        {
            yield return new object[] { index, null };
            yield return new object[] { index, new DataGridViewColumn() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Insert_TestData))]
    public void DataGridViewSelectedColumnCollection_Insert_Invoke_ThrowsNotSupportedException(int index, DataGridViewColumn dataGridViewColumn)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        Assert.Throws<NotSupportedException>(() => collection.Insert(index, dataGridViewColumn));
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListProperties_GetEmpty_ReturnsExpected()
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Empty(iList);
        Assert.True(iList.IsFixedSize);
        Assert.True(iList.IsReadOnly);
        Assert.False(iList.IsSynchronized);
        Assert.Same(collection, iList.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListProperties_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Equal(2, iList.Count);
        Assert.True(iList.IsFixedSize);
        Assert.True(iList.IsReadOnly);
        Assert.False(iList.IsSynchronized);
        Assert.Same(collection, iList.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListItem_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Equal(control.Columns[2], iList[0]);
        Assert.Equal(control.Columns[0], iList[1]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void DataGridViewSelectedColumnCollection_IListItem_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    public void DataGridViewSelectedColumnCollection_IListItem_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
    }

    public static IEnumerable<object[]> IListItem_SetTestData()
    {
        yield return new object[] { 0, null };
        yield return new object[] { -1, new() };
        yield return new object[] { 1, new DataGridViewColumn() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListItem_SetTestData))]
    public void DataGridViewSelectedColumnCollection_IListItem_Set_ThrowsNotSupportedException(int index, object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList[index] = value);
    }

    public static IEnumerable<object[]> IListAdd_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataGridViewColumn() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListAdd_TestData))]
    public void DataGridViewSelectedColumnCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.Add(value));
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListClear_Invoke_ThrowsNotSupportedException()
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(iList.Clear);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListCopyTo_InvokeEmpty_Success()
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        object[] array = [1, 2, 3];
        iList.CopyTo(array, 1);
        Assert.Equal([1, 2, 3], array);
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListCopyTo_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        object[] array = [1, 2, 3];
        iList.CopyTo(array, 1);
        Assert.Equal([1, control.Columns[2], control.Columns[0]], array);
    }

    public static IEnumerable<object[]> IListContains_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataGridViewColumn() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListContains_TestData))]
    public void DataGridViewSelectedColumnCollection_IListContains_InvokeEmpty_ReturnsFalse(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.False(iList.Contains(value));
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListContains_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.True(iList.Contains(control.Columns[0]));
        Assert.False(iList.Contains(control.Columns[1]));
        Assert.True(iList.Contains(control.Columns[2]));
        Assert.False(iList.Contains(new DataGridViewColumn()));
        Assert.False(iList.Contains(null));
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListGetEnumerator_InvokeEmpty_Success()
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
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
    public void DataGridViewSelectedColumnCollection_IListGetEnumerator_InvokeNotEmpty_Success()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        IEnumerator enumerator = iList.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(control.Columns[2], enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(control.Columns[0], enumerator.Current);

            Assert.False(enumerator.MoveNext());
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            enumerator.Reset();
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);
        }
    }

    public static IEnumerable<object[]> IListIndexOf_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataGridViewColumn() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListIndexOf_TestData))]
    public void DataGridViewSelectedColumnCollection_IListIndexOf_InvokeEmpty_ReturnsMinusOne(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Equal(-1, iList.IndexOf(value));
    }

    [WinFormsFact]
    public void DataGridViewSelectedColumnCollection_IListIndexOf_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 3,
            RowCount = 1
        };
        control.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
        control.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

        control.Columns[0].Selected = true;
        control.Columns[1].Selected = false;
        control.Columns[2].Selected = true;

        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Equal(1, iList.IndexOf(control.Columns[0]));
        Assert.Equal(-1, iList.IndexOf(control.Columns[1]));
        Assert.Equal(0, iList.IndexOf(control.Columns[2]));
        Assert.Equal(-1, iList.IndexOf(new DataGridViewColumn()));
        Assert.Equal(-1, iList.IndexOf(null));
    }

    public static IEnumerable<object[]> IListInsert_TestData()
    {
        foreach (int index in new int[] { -1, 0, 1 })
        {
            yield return new object[] { index, null };
            yield return new object[] { index, new() };
            yield return new object[] { index, new DataGridViewColumn() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(IListInsert_TestData))]
    public void DataGridViewSelectedColumnCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.Insert(index, value));
    }

    public static IEnumerable<object[]> IListRemove_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataGridViewColumn() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListRemove_TestData))]
    public void DataGridViewSelectedColumnCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.Remove(value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void DataGridViewSelectedColumnCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
    {
        using DataGridView control = new();
        DataGridViewSelectedColumnCollection collection = control.SelectedColumns;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.RemoveAt(index));
    }
}
