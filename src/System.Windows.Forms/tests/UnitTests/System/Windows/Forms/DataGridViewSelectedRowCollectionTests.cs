// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class DataGridViewSelectedRowCollectionTests
{
    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_Properties_GetEmpty_ReturnsExpected()
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.Empty(collection);
        Assert.False(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_Properties_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.Equal(2, collection.Count);
        Assert.False(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_Item_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.Equal(control.Rows[2], collection[0]);
        Assert.Equal(control.Rows[0], collection[1]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void DataGridViewSelectedRowCollection_Item_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    public void DataGridViewSelectedRowCollection_Item_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_Clear_Invoke_ThrowsNotSupportedException()
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.Throws<NotSupportedException>(collection.Clear);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_Contains_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.True(collection.Contains(control.Rows[0]));
        Assert.False(collection.Contains(control.Rows[1]));
        Assert.True(collection.Contains(control.Rows[2]));
        Assert.False(collection.Contains(new DataGridViewRow()));
        Assert.False(collection.Contains(null));
    }

    public static IEnumerable<object[]> Contains_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new DataGridViewRow() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Contains_TestData))]
    public void DataGridViewSelectedRowCollection_Contains_InvokeEmpty_ReturnsFalse(DataGridViewRow dataGridViewRow)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.False(collection.Contains(dataGridViewRow));
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_CopyTo_InvokeEmpty_Success()
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        object[] array = [1, 2, 3];
        collection.CopyTo(array, 1);
        Assert.Equal([1, 2, 3], array);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_CopyTo_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        object[] array = [1, 2, 3];
        collection.CopyTo(array, 1);
        Assert.Equal([1, control.Rows[2], control.Rows[0]], array);
    }

    public static IEnumerable<object[]> Insert_TestData()
    {
        foreach (int index in new int[] { -1, 0, 1 })
        {
            yield return new object[] { index, null };
            yield return new object[] { index, new DataGridViewRow() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Insert_TestData))]
    public void DataGridViewSelectedRowCollection_Insert_Invoke_ThrowsNotSupportedException(int index, DataGridViewRow dataGridRow)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        Assert.Throws<NotSupportedException>(() => collection.Insert(index, dataGridRow));
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListProperties_GetEmpty_ReturnsExpected()
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Empty(iList);
        Assert.True(iList.IsFixedSize);
        Assert.True(iList.IsReadOnly);
        Assert.False(iList.IsSynchronized);
        Assert.Same(collection, iList.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListProperties_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Equal(2, iList.Count);
        Assert.True(iList.IsFixedSize);
        Assert.True(iList.IsReadOnly);
        Assert.False(iList.IsSynchronized);
        Assert.Same(collection, iList.SyncRoot);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListItem_GetNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Equal(control.Rows[2], iList[0]);
        Assert.Equal(control.Rows[0], iList[1]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void DataGridViewSelectedRowCollection_IListItem_GetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    public void DataGridViewSelectedRowCollection_IListItem_GetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => iList[index]);
    }

    public static IEnumerable<object[]> IListItem_SetTestData()
    {
        yield return new object[] { 0, null };
        yield return new object[] { -1, new() };
        yield return new object[] { 1, new DataGridViewRow() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListItem_SetTestData))]
    public void DataGridViewSelectedRowCollection_IListItem_Set_ThrowsNotSupportedException(int index, object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList[index] = value);
    }

    public static IEnumerable<object[]> IListAdd_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataGridViewRow() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListAdd_TestData))]
    public void DataGridViewSelectedRowCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.Add(value));
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListClear_Invoke_ThrowsNotSupportedException()
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(iList.Clear);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListCopyTo_InvokeEmpty_Success()
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        object[] array = [1, 2, 3];
        iList.CopyTo(array, 1);
        Assert.Equal([1, 2, 3], array);
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListCopyTo_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        object[] array = [1, 2, 3];
        iList.CopyTo(array, 1);
        Assert.Equal([1, control.Rows[2], control.Rows[0]], array);
    }

    public static IEnumerable<object[]> IListContains_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataGridViewRow() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListContains_TestData))]
    public void DataGridViewSelectedRowCollection_IListContains_InvokeEmpty_ReturnsFalse(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.False(iList.Contains(value));
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListContains_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.True(iList.Contains(control.Rows[0]));
        Assert.False(iList.Contains(control.Rows[1]));
        Assert.True(iList.Contains(control.Rows[2]));
        Assert.False(iList.Contains(new DataGridViewRow()));
        Assert.False(iList.Contains(null));
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListGetEnumerator_InvokeEmpty_Success()
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
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
    public void DataGridViewSelectedRowCollection_IListGetEnumerator_InvokeNotEmpty_Success()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        IEnumerator enumerator = iList.GetEnumerator();
        for (int i = 0; i < 2; i++)
        {
            Assert.Throws<InvalidOperationException>(() => enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(control.Rows[2], enumerator.Current);

            Assert.True(enumerator.MoveNext());
            Assert.Same(control.Rows[0], enumerator.Current);

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
        yield return new object[] { new DataGridViewRow() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListIndexOf_TestData))]
    public void DataGridViewSelectedRowCollection_IListIndexOf_InvokeEmpty_ReturnsMinusOne(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Equal(-1, iList.IndexOf(value));
    }

    [WinFormsFact]
    public void DataGridViewSelectedRowCollection_IListIndexOf_InvokeNotEmpty_ReturnsExpected()
    {
        using DataGridView control = new()
        {
            ColumnCount = 1,
            RowCount = 3
        };
        control.Rows[0].Selected = true;
        control.Rows[1].Selected = false;
        control.Rows[2].Selected = true;

        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Equal(1, iList.IndexOf(control.Rows[0]));
        Assert.Equal(-1, iList.IndexOf(control.Rows[1]));
        Assert.Equal(0, iList.IndexOf(control.Rows[2]));
        Assert.Equal(-1, iList.IndexOf(new DataGridViewRow()));
        Assert.Equal(-1, iList.IndexOf(null));
    }

    public static IEnumerable<object[]> IListInsert_TestData()
    {
        foreach (int index in new int[] { -1, 0, 1 })
        {
            yield return new object[] { index, null };
            yield return new object[] { index, new() };
            yield return new object[] { index, new DataGridViewRow() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(IListInsert_TestData))]
    public void DataGridViewSelectedRowCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.Insert(index, value));
    }

    public static IEnumerable<object[]> IListRemove_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataGridViewRow() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListRemove_TestData))]
    public void DataGridViewSelectedRowCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.Remove(value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void DataGridViewSelectedRowCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
    {
        using DataGridView control = new();
        DataGridViewSelectedRowCollection collection = control.SelectedRows;
        IList iList = collection;
        Assert.Throws<NotSupportedException>(() => iList.RemoveAt(index));
    }
}
