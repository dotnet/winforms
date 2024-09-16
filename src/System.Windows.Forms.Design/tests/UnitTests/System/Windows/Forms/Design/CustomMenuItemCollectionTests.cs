// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class CustomMenuItemCollectionTests : IDisposable
{
    private readonly CustomMenuItemCollection _collection;

    public CustomMenuItemCollectionTests() => _collection = [];

    public void Dispose() => _collection.Clear();

    [Fact]
    public void Add_ShouldAddItemToCollection()
    {
        ToolStripMenuItem item = new("TestItem");

        int index = _collection.Add(item);

        index.Should().Be(0);
        _collection.Count.Should().Be(1);
    }

    [Fact]
    public void AddRange_ShouldAddMultipleItemsToCollection()
    {
        ToolStripMenuItem[] items =
        [
            new("Item1"),
            new("Item2")
        ];

        _collection.AddRange(items);
        _collection.Count.Should().Be(2);

        _collection.RefreshItems();
        _collection.Count.Should().Be(2);
    }

    [Fact]
    public void AddRange_EmptyArray_ShouldNotThrowException()
    {
        ToolStripMenuItem[] items = [];

        Action action = () => _collection.AddRange(items);

        action.Should().NotThrow();
        _collection.Count.Should().Be(0);
    }

    [Fact]
    public void Clear_ShouldClearCollection()
    {
        ToolStripMenuItem[] items =
        [
            new("Item1"),
            new("Item2")
        ];

        _collection.AddRange(items);
        _collection.Count.Should().Be(2);

        _collection.Clear();
        _collection.Count.Should().Be(0);
    }
}
