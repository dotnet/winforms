// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListBoxSelectedObjectCollectionTests
{
    [WinFormsFact]
    public void ListBoxSelectedObjectCollection_Ctor_ListBox()
    {
        using ListBox owner = new();
        var collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Empty(collection);
        Assert.True(collection.IsReadOnly);
    }

    [WinFormsFact]
    public void ListBoxSelectedObjectCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        Assert.Throws<ArgumentNullException>("owner", () => new ListBox.SelectedObjectCollection(null));
    }

    [WinFormsFact]
    public void ListBoxSelectedObjectCollection_ICollection_Properties_GetReturnsExpected()
    {
        using ListBox owner = new();
        ICollection collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Empty(collection);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsFact]
    public void ListBoxSelectedObjectCollection_IList_Properties_GetReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Empty(collection);
        Assert.True(collection.IsFixedSize);
        Assert.True(collection.IsReadOnly);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsTheory]
    [InlineData(-1, null)]
    [InlineData(0, null)]
    [InlineData(1, null)]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(-1, "1")]
    [InlineData(0, "1")]
    [InlineData(1, "1")]
    public void ListBoxSelectedObjectCollection_IListItem_Set_ThrowsNotSupportedException(int index, object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection[index] = value);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedObjectCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Add(value));
    }

    [WinFormsFact]
    public void ListBoxSelectedObjectCollection_IListClear_Invoke_ThrowsNotSupportedException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(collection.Clear);
    }

    [WinFormsTheory]
    [InlineData(-1, null)]
    [InlineData(0, null)]
    [InlineData(1, null)]
    [InlineData(-1, 1)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(-1, "1")]
    [InlineData(0, "1")]
    [InlineData(1, "1")]
    public void ListBoxSelectedObjectCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Insert(index, value));
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedObjectCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Remove(value));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxSelectedObjectCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.RemoveAt(index));
    }
}
