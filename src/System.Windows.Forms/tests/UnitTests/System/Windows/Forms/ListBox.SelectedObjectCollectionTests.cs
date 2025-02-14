// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListBoxSelectedObjectCollectionTests
{
    [Fact]
    public void ListBoxSelectedObjectCollection_Ctor_ListBox()
    {
        using ListBox owner = new();
        var collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Empty(collection);
        Assert.True(collection.IsReadOnly);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        Assert.Throws<ArgumentNullException>("owner", () => new ListBox.SelectedObjectCollection(null!));
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_ICollection_Properties_GetReturnsExpected()
    {
        using ListBox owner = new();
        ICollection collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Empty(collection);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [Fact]
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

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
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

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedObjectCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Add(value));
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_IListClear_Invoke_ThrowsNotSupportedException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(collection.Clear);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
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

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedObjectCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Remove(value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxSelectedObjectCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedObjectCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.RemoveAt(index));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData("item1", false)]
    [InlineData("item2", true)]
    public void ListBoxSelectedObjectCollection_Contains_ReturnsExpected(object value, bool expected)
    {
        using ListBox owner = new();
        owner.Items.Add("item1");
        owner.Items.Add("item2");
        owner.SelectedItems.Add("item2");
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.Contains(value).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, -1)]
    [InlineData("item1", -1)]
    [InlineData("item2", 0)]
    public void ListBoxSelectedObjectCollection_IndexOf_ReturnsExpected(object value, int expected)
    {
        using ListBox owner = new();
        owner.Items.Add("item1");
        owner.Items.Add("item2");
        owner.SelectedItems.Add("item2");
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.EnsureUpToDate();
        collection.IndexOf(value).Should().Be(expected);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_CopyTo_EmptyCollection_DoesNothing()
    {
        using ListBox owner = new();
        ListBox.SelectedObjectCollection collection = new(owner);
        object[] destination = new object[1];
        collection.CopyTo(destination, 0);
        destination.Should().AllBeEquivalentTo<object>(null!);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_CopyTo_NonEmptyCollection_CopiesItems()
    {
        using ListBox owner = new();
        owner.Items.Add("item1");
        owner.Items.Add("item2");
        owner.SelectedItems.Add("item2");
        ListBox.SelectedObjectCollection collection = new(owner);
        object[] destination = new object[1];
        collection.CopyTo(destination, 0);
        destination[0].Should().Be("item2");
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Clear_EmptyCollection_DoesNothing()
    {
        using ListBox owner = new();
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.Clear();
        collection.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Clear_NonEmptyCollection_ClearsItems()
    {
        using ListBox owner = new();
        owner.Items.Add("item1");
        owner.Items.Add("item2");
        owner.SelectedItems.Add("item2");
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.Clear();
        collection.Count.Should().Be(0);
        owner.SelectedItems.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_EmptyCollection_DoesNothing()
    {
        using ListBox owner = new();
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.Remove("item1");
        collection.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_NonEmptyCollection_RemovesItem()
    {
        using ListBox owner = new();
        owner.Items.Add("item1");
        owner.Items.Add("item2");
        owner.SelectedItems.Add("item2");
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.Remove("item2");
        collection.Count.Should().Be(0);
        owner.SelectedItems.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_ItemNotInCollection_DoesNothing()
    {
        using ListBox owner = new();
        owner.Items.Add("item1");
        owner.Items.Add("item2");
        owner.SelectedItems.Add("item2");
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.Remove("item3");
        collection.Count.Should().Be(1);
        owner.SelectedItems.Count.Should().Be(1);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_NullItem_DoesNothing()
    {
        using ListBox owner = new();
        owner.Items.Add("item1");
        owner.Items.Add("item2");
        owner.SelectedItems.Add("item2");
        ListBox.SelectedObjectCollection collection = new(owner);
        collection.Remove(null!);
        collection.Count.Should().Be(1);
        owner.SelectedItems.Count.Should().Be(1);
    }
}
