// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Tests;

public class ListBoxSelectedIndexCollectionTests
{
    [Fact]
    public void ListBoxSelectedIndexCollection_Ctor_ListBox()
    {
        using ListBox owner = new();
        var collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Empty(collection);
        Assert.True(collection.IsReadOnly);
    }

    [Fact]
    public void ListBoxSelectedIndexCollection_Ctor_NullOwner_ThrowsArgumentNullException()
    {
        using ListBox owner = new();
        Assert.Throws<ArgumentNullException>("owner", () => new ListBox.SelectedIndexCollection(null!));
    }

    [Fact]
    public void ListBoxSelectedIndexCollection_ICollection_Properties_GetReturnsExpected()
    {
        using ListBox owner = new();
        ICollection collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Empty(collection);
        Assert.True(collection.IsSynchronized);
        Assert.Same(collection, collection.SyncRoot);
    }

    [Fact]
    public void ListBoxSelectedIndexCollection_IList_Properties_GetReturnsExpected()
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Empty(collection);
        Assert.True(collection.IsFixedSize);
        Assert.True(collection.IsReadOnly);
        Assert.True(collection.IsSynchronized);
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
    public void ListBoxSelectedIndexCollection_IListItem_Set_ThrowsNotSupportedException(int index, object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection[index] = value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedIndexCollection_IListAdd_Invoke_ThrowsNotSupportedException(object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Add(value));
    }

    [Fact]
    public void ListBoxSelectedIndexCollection_IListClear_Invoke_ThrowsNotSupportedException()
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedIndexCollection(owner);
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
    public void ListBoxSelectedIndexCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Insert(index, value));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData("1")]
    public void ListBoxSelectedIndexCollection_IListRemove_Invoke_ThrowsNotSupportedException(object value)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.Remove(value));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ListBoxSelectedIndexCollection_IListRemoveAt_Invoke_ThrowsNotSupportedException(int index)
    {
        using ListBox owner = new();
        IList collection = new ListBox.SelectedIndexCollection(owner);
        Assert.Throws<NotSupportedException>(() => collection.RemoveAt(index));
    }

    [Fact]
    public void CopyTo_CopiesItemsCorrectly()
    {
        using ListBox listBox = new()
        {
            Items = { "Item1", "Item2", "Item3" },
            SelectedIndices = { 0, 2 }
        };

        int[] destination = new int[2];
        listBox.SelectedIndices.CopyTo(destination, 0);

        destination.Should().BeEquivalentTo(new int[] { 0, 2 });
    }

    [Fact]
    public void Clear_ClearsSelectedIndices()
    {
        using ListBox listBox = new()
        {
            Items = { "Item1", "Item2", "Item3" },
            SelectedIndices = { 0, 2 }
        };

        listBox.SelectedIndices.Clear();

        listBox.SelectedIndices.Count.Should().Be(0);
        listBox.SelectedIndices.Contains(0).Should().BeFalse();
        listBox.SelectedIndices.Contains(2).Should().BeFalse();
    }

    [Fact]
    public void Remove_RemovesSelectedIndex()
    {
        using ListBox listBox = new()
        {
            Items = { "Item1", "Item2", "Item3" },
            SelectedIndices = { 0, 2 }
        };

        listBox.SelectedIndices.Remove(2);

        listBox.SelectedIndices.Count.Should().Be(1);
        listBox.SelectedIndices.Contains(0).Should().BeTrue();
        listBox.SelectedIndices.Contains(2).Should().BeFalse();
    }
}
