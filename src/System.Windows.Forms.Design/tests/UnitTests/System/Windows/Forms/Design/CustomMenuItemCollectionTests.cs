// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class CustomMenuItemCollectionTests
{
    [Fact]
    public void Add_ShouldAddItemToCollection()
    {
        CustomMenuItemCollection collection = new();
        ToolStripMenuItem item = new("TestItem");

        int index = collection.Add(item);

        index.Should().Be(0);
        collection.Count.Should().Be(1);
    }

    [Fact]
    public void AddRange_ShouldAddMultipleItemsToCollection()
    {
        CustomMenuItemCollection collection = new();
        ToolStripMenuItem[] items =
        [
            new ToolStripMenuItem("Item1"),
            new ToolStripMenuItem("Item2")
        ];

        collection.AddRange(items);

        collection.Count.Should().Be(2);
    }

    [Fact]
    public void RefreshItems_ShouldNotThrowException()
    {
        CustomMenuItemCollection collection = new();

        Action action = () => collection.RefreshItems();

        action.Should().NotThrow();
    }

    [Fact]
    public void AddRange_EmptyArray_ShouldNotThrowException()
    {
        CustomMenuItemCollection collection = new();
        ToolStripMenuItem[] items = [];

        Action action = () => collection.AddRange(items);

        action.Should().NotThrow();
        collection.Count.Should().Be(0);
    }

    [Fact]
    public void Clear_ShouldClearCollection()
    {
        CustomMenuItemCollection collection = new();
        ToolStripMenuItem[] items =
        [
            new ToolStripMenuItem("Item1"),
            new ToolStripMenuItem("Item2")
        ];

        collection.AddRange(items);
        collection.Clear();

        collection.Count.Should().Be(0);
    }
}
