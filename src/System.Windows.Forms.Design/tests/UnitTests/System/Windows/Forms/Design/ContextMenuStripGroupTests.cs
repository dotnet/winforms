// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace System.Windows.Forms.Design.Tests;

public class ContextMenuStripGroupTests
{
    [Fact]
    public void Items_ShouldInitializeCorrectly()
    {
        ContextMenuStripGroup contextMenuStripGroup = new();

        IList<ToolStripItem> items = contextMenuStripGroup.Items;

        items.Should().NotBeNull();
        items.Should().BeEmpty();
    }

    [Fact]
    public void Items_ShouldReturnSameInstance()
    {
        ContextMenuStripGroup contextMenuStripGroup = new();

        IList<ToolStripItem> items1 = contextMenuStripGroup.Items;
        IList<ToolStripItem> items2 = contextMenuStripGroup.Items;

        items1.Should().NotBeNull();
        items2.Should().NotBeNull();
        items1.Should().BeSameAs(items2);
    }

    [Fact]
    public void Items_ShouldAllowAddingItems()
    {
        ContextMenuStripGroup contextMenuStripGroup = new();
        ToolStripMenuItem newItem = new("Test Item");

        contextMenuStripGroup.Items.Add(newItem);

        contextMenuStripGroup.Items.Should().ContainSingle()
            .Which.Should().Be(newItem);
    }

    [Fact]
    public void Items_ShouldAllowAddingMultipleItems()
    {
        ContextMenuStripGroup contextMenuStripGroup = new();
        ToolStripMenuItem newItem1 = new("Test Item1");
        ToolStripMenuItem newItem2 = new("Test Item2");

        contextMenuStripGroup.Items.Add(newItem1);
        contextMenuStripGroup.Items.Add(newItem2);

        contextMenuStripGroup.Items.Should().HaveCount(2);
        contextMenuStripGroup.Items.Should().Contain(newItem1);
        contextMenuStripGroup.Items.Should().Contain(newItem2);
    }

    [Fact]
    public void Items_ShouldAllowClearingItems()
    {
        ContextMenuStripGroup contextMenuStripGroup = new();
        ToolStripMenuItem newItem = new("Test Item");

        contextMenuStripGroup.Items.Add(newItem);
        contextMenuStripGroup.Items.Clear();

        contextMenuStripGroup.Items.Should().BeEmpty();
    }
}
