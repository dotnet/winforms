// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListBoxSelectedObjectCollectionTests : IDisposable
{
    private readonly ListBox _owner;
    private readonly ListBox.SelectedObjectCollection _collection;

    public ListBoxSelectedObjectCollectionTests()
    {
        _owner = new();
        _collection = new(_owner);
    }

    public void Dispose()
    {
        _owner.Dispose();
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Ctor_ListBox()
    {
        Assert.Empty(_collection);
        Assert.True(_collection.IsReadOnly);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("owner", () => new ListBox.SelectedObjectCollection(null!));
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_ICollection_Properties_GetReturnsExpected()
    {
        ICollection collection = _collection;
        Assert.Empty(collection);
        Assert.False(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_IList_Properties_GetReturnsExpected()
    {
        IList collection = _collection;
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
        IList collection = _collection;
        Assert.Throws<NotSupportedException>(() => collection[index] = value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedObjectCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
    {
        IList collection = _collection;
        Assert.Throws<NotSupportedException>(() => collection.Add(value));
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_IListClear_Invoke_ThrowsNotSupportedException()
    {
        IList collection = _collection;
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
        IList collection = _collection;
        Assert.Throws<NotSupportedException>(() => collection.Insert(index, value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedObjectCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
    {
        IList collection = _collection;
        Assert.Throws<NotSupportedException>(() => collection.Remove(value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxSelectedObjectCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
    {
        IList collection = _collection;
        Assert.Throws<NotSupportedException>(() => collection.RemoveAt(index));
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData("item1", false)]
    [InlineData("item2", true)]
    public void ListBoxSelectedObjectCollection_Contains_ReturnsExpected(object value, bool expected)
    {
        _owner.Items.Add("item1");
        _owner.Items.Add("item2");
        _owner.SelectedItems.Add("item2");
        _collection.Contains(value).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, -1)]
    [InlineData("item1", -1)]
    [InlineData("item2", 0)]
    public void ListBoxSelectedObjectCollection_IndexOf_ReturnsExpected(object value, int expected)
    {
        _owner.Items.Add("item1");
        _owner.Items.Add("item2");
        _owner.SelectedItems.Add("item2");
        _collection.EnsureUpToDate();
        _collection.IndexOf(value).Should().Be(expected);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_CopyTo_EmptyCollection_DoesNothing()
    {
        object[] destination = new object[1];
        _collection.CopyTo(destination, 0);
        destination.Should().AllBeEquivalentTo<object>(null!);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_CopyTo_NonEmptyCollection_CopiesItems()
    {
        _owner.Items.Add("item1");
        _owner.Items.Add("item2");
        _owner.SelectedItems.Add("item2");
        object[] destination = new object[1];
        _collection.CopyTo(destination, 0);
        destination[0].Should().Be("item2");
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Clear_EmptyCollection_DoesNothing()
    {
        _collection.Clear();
        _collection.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Clear_NonEmptyCollection_ClearsItems()
    {
        _owner.Items.Add("item1");
        _owner.Items.Add("item2");
        _owner.SelectedItems.Add("item2");
        _collection.Clear();
        _collection.Count.Should().Be(0);
        _owner.SelectedItems.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_EmptyCollection_DoesNothing()
    {
        _collection.Remove("item1");
        _collection.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_NonEmptyCollection_RemovesItem()
    {
        _owner.Items.Add("item1");
        _owner.Items.Add("item2");
        _owner.SelectedItems.Add("item2");
        _collection.Remove("item2");
        _collection.Count.Should().Be(0);
        _owner.SelectedItems.Count.Should().Be(0);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_ItemNotInCollection_DoesNothing()
    {
        _owner.Items.Add("item1");
        _owner.Items.Add("item2");
        _owner.SelectedItems.Add("item2");
        _collection.Remove("item3");
        _collection.Count.Should().Be(1);
        _owner.SelectedItems.Count.Should().Be(1);
    }

    [Fact]
    public void ListBoxSelectedObjectCollection_Remove_NullItem_DoesNothing()
    {
        _owner.Items.Add("item1");
        _owner.Items.Add("item2");
        _owner.SelectedItems.Add("item2");
        _collection.Remove(null!);
        _collection.Count.Should().Be(1);
        _owner.SelectedItems.Count.Should().Be(1);
    }
}
