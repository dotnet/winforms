// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripItemCollectionTests
{
    [WinFormsTheory]
    [InlineData("name2")]
    [InlineData("NAME2")]
    public void ToolStripItemCollection_Find_InvokeKeyExists_ReturnsExpected(string key)
    {
        using ToolStripMenuItem toolStrip = new();

        ToolStripMenuItem child1 = new()
        {
            Name = "name1"
        };
        ToolStripMenuItem child2 = new()
        {
            Name = "name2"
        };
        ToolStripMenuItem child3 = new()
        {
            Name = "name2"
        };

        ToolStripMenuItem grandchild1 = new()
        {
            Name = "name1"
        };
        ToolStripMenuItem grandchild2 = new()
        {
            Name = "name2"
        };
        ToolStripMenuItem grandchild3 = new()
        {
            Name = "name2"
        };
        child3.DropDownItems.Add(grandchild1);
        child3.DropDownItems.Add(grandchild2);
        child3.DropDownItems.Add(grandchild3);
        ToolStripItemCollection collection = toolStrip.DropDownItems;
        collection.Add(child1);
        collection.Add(child2);
        collection.Add(child3);

        // Search all children.
        Assert.Equal(new ToolStripMenuItem[] { child2, child3, grandchild2, grandchild3 }, collection.Find(key, searchAllChildren: true));

        // Call again.
        Assert.Equal(new ToolStripMenuItem[] { child2, child3, grandchild2, grandchild3 }, collection.Find(key, searchAllChildren: true));

        // Don't search all children.
        Assert.Equal(new ToolStripMenuItem[] { child2, child3 }, collection.Find(key, searchAllChildren: false));

        // Call again.
        Assert.Equal(new ToolStripMenuItem[] { child2, child3 }, collection.Find(key, searchAllChildren: false));
    }

    [WinFormsTheory]
    [InlineData("NoSuchName")]
    [InlineData("abcd")]
    [InlineData("abcde")]
    [InlineData("abcdef")]
    public void ToolStripItemCollection_Find_InvokeNoSuchKey_ReturnsEmpty(string key)
    {
        using ToolStripMenuItem toolStrip = new();

        ToolStripMenuItem child1 = new()
        {
            Name = "name1"
        };
        ToolStripMenuItem child2 = new()
        {
            Name = "name2"
        };
        ToolStripMenuItem child3 = new()
        {
            Name = "name2"
        };
        var collection = toolStrip.DropDown.DisplayedItems;
        collection.Add(child1);
        collection.Add(child2);
        collection.Add(child3);

        Assert.Empty(collection.Find(key, searchAllChildren: true));
        Assert.Empty(collection.Find(key, searchAllChildren: false));
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void ToolStripItemCollection_Find_NullOrEmptyKey_ThrowsArgumentNullException(string key)
    {
        using ToolStripMenuItem toolStrip = new();
        var collection = toolStrip.DropDown.DisplayedItems;
        Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: true));
        Assert.Throws<ArgumentNullException>("key", () => collection.Find(key, searchAllChildren: false));
    }

    [WinFormsFact]
    public void ToolStripItemCollection_AddRange_ToolStripItemCollection_Success()
    {
        using ContextMenuStrip contextMenuStrip = new();
        using ToolStripDropDownButton toolStripDropDownButton = new();

        // Add 0 items.
        contextMenuStrip.Items.AddRange(toolStripDropDownButton.DropDownItems);
        Assert.Empty(contextMenuStrip.Items);

        // Add 3 items.
        toolStripDropDownButton.DropDownItems.Add("a");
        toolStripDropDownButton.DropDownItems.Add("b");
        toolStripDropDownButton.DropDownItems.Add("c");
        contextMenuStrip.Items.AddRange(toolStripDropDownButton.DropDownItems);

        Assert.Empty(toolStripDropDownButton.DropDownItems);
        Assert.Equal(3, contextMenuStrip.Items.Count);

        // Validate order.
        Assert.Equal("a", contextMenuStrip.Items[0].Text);
        Assert.Equal("b", contextMenuStrip.Items[1].Text);
        Assert.Equal("c", contextMenuStrip.Items[2].Text);
    }

    [WinFormsFact]
    public void ToolStripItemCollection_AddRange_ToolStripItemCollection_SameOwner_Success()
    {
        using ToolStrip toolStrip = new();

        // Create a ToolStripItemCollection with 2 items
        ToolStripItemCollection itemCollection = new(toolStrip,
            [
                new ToolStripButton("Button 1"),
                new ToolStripButton("Button 2")
            ]);

        toolStrip.Items.Count.Should().Be(0);

        toolStrip.Items.AddRange(itemCollection);

        itemCollection.Count.Should().Be(2);
        toolStrip.Items.Count.Should().Be(2);

        toolStrip.Items[0].Text.Should().Be("Button 1");
        toolStrip.Items[1].Text.Should().Be("Button 2");
    }

    private ToolStripItem[] CreateToolStripItems(params (string Key, string Name)[] items) =>
        items.Select(item => new ToolStripButton(item.Name) { Name = item.Key }).ToArray();

    [WinFormsFact]
    public void ToolStripItemCollection_Constructor_ToolStripOwnerAndItems_Success()
    {
        using ToolStrip toolStrip = new();
        ToolStripItem[] items = CreateToolStripItems(("key1", "Item1"), ("key2", "Item2"));

        ToolStripItemCollection collection = new(toolStrip, items);

        collection.Count.Should().Be(2);
        collection[0].Text.Should().Be("Item1");
        collection[1].Text.Should().Be("Item2");
    }

    private ToolStripItemCollection CreateToolStripItemCollectionWithItems(params (string Key, string Name)[] items)
    {
        using ToolStrip toolStrip = new();
        ToolStripItem[] toolStripItems = items.Select(item => new ToolStripButton(item.Name) { Name = item.Key }).ToArray();
        return new ToolStripItemCollection(toolStrip, toolStripItems);
    }

    [WinFormsFact]
    public void ToolStripItemCollection_Indexer_ValidKey_ReturnsExpected()
    {
        ToolStripItemCollection collection = CreateToolStripItemCollectionWithItems(
            ("key1", "Item1"),
            ("key2", "Item2")
        );

        collection["key1"].Should().Be(collection[0]);
        collection["key2"].Should().Be(collection[1]);
    }

    [WinFormsFact]
    public void ToolStripItemCollection_Indexer_InvalidOrNullOrEmptyKey_ReturnsNull()
    {
        ToolStripItemCollection collection = CreateToolStripItemCollectionWithItems(
            ("key1", "Item1"),
            ("key2", "Item2")
        );

        collection["invalidKey"].Should().BeNull();
        collection[null].Should().BeNull();
        collection[string.Empty].Should().BeNull();
    }

    [WinFormsFact]
    public void ToolStripItemCollection_ContainsKey_ValidKey_ReturnsTrue()
    {
        ToolStripItemCollection collection = CreateToolStripItemCollectionWithItems(
            ("key1", "Item1"),
            ("key2", "Item2")
        );

        collection.ContainsKey("key1").Should().BeTrue();
        collection.ContainsKey("key2").Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripItemCollection_ContainsKey_InvalidOrNullOrEmptyKey_ReturnsFalse()
    {
        ToolStripItemCollection collection = CreateToolStripItemCollectionWithItems(
            ("key1", "Item1"),
            ("key2", "Item2")
        );

        collection.ContainsKey("invalidKey").Should().BeFalse();
        collection.ContainsKey(null).Should().BeFalse();
        collection.ContainsKey(string.Empty).Should().BeFalse();
    }

    [WinFormsTheory]
    [InlineData("key1", 0)]
    [InlineData("key2", 1)]
    [InlineData("key3", 2)]
    public void ToolStripItemCollection_IndexOfKey_ValidKey_ReturnsExpected(string key, int expectedIndex)
    {
        ToolStripItemCollection collection = CreateToolStripItemCollectionWithItems(
            ("key1", "Item1"),
            ("key2", "Item2"),
            ("key3", "Item3")
        );

        collection.IndexOfKey(key).Should().Be(expectedIndex);
    }

    [WinFormsTheory]
    [InlineData("invalidKey", -1)]
    [InlineData("key4", -1)]
    [InlineData("", -1)]
    public void ToolStripItemCollection_IndexOfKey_InvalidOrEmptyKey_ReturnsMinusOne(string key, int expectedIndex)
    {
        ToolStripItemCollection collection = CreateToolStripItemCollectionWithItems(
            ("key1", "Item1"),
            ("key2", "Item2"),
            ("key3", "Item3")
        );

        collection.IndexOfKey(key).Should().Be(expectedIndex);
    }

    [WinFormsFact]
    public void ToolStripItemCollection_IsValidIndex_ValidIndex_ReturnsTrue()
    {
        using ToolStrip toolStrip = new();
        ToolStripItem[] items = CreateToolStripItems(("key1", "Item1"), ("key2", "Item2"));
        ToolStripItemCollection collection = new(toolStrip, items);

        dynamic accessor = collection.TestAccessor().Dynamic;

        ((bool)accessor.IsValidIndex(0)).Should().BeTrue();
        ((bool)accessor.IsValidIndex(1)).Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripItemCollection_IsValidIndex_InvalidIndex_ReturnsFalse()
    {
        using ToolStrip toolStrip = new();
        ToolStripItem[] items = CreateToolStripItems(("key1", "Item1"), ("key2", "Item2"));
        ToolStripItemCollection collection = new(toolStrip, items);

        dynamic accessor = collection.TestAccessor().Dynamic;

        ((bool)accessor.IsValidIndex(-1)).Should().BeFalse();
        ((bool)accessor.IsValidIndex(2)).Should().BeFalse();
    }

    [WinFormsFact]
    public void ToolStripItemCollection_RemoveByKey_ValidKey_RemovesItem()
    {
        using ToolStrip toolStrip = new();
        ToolStripItem[] items = CreateToolStripItems(("key1", "Item1"), ("key2", "Item2"));
        ToolStripItemCollection collection = new(toolStrip, items);

        collection.ContainsKey("key1").Should().BeTrue();

        collection.RemoveByKey("key1");

        collection.ContainsKey("key1").Should().BeFalse();
        collection.Count.Should().Be(1);
        collection[0].Text.Should().Be("Item2");
    }

    [WinFormsFact]
    public void ToolStripItemCollection_RemoveByKey_InvalidKey_DoesNothing()
    {
        ToolStripItemCollection collection = CreateToolStripItemCollectionWithItems(
            ("key1", "Item1"),
            ("key2", "Item2")
        );

        collection.RemoveByKey("invalidKey");

        collection.Contains(collection[0]).Should().BeTrue();
        collection.Contains(collection[1]).Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripItemCollection_MoveItem_ItemWithParent_MovesItem()
    {
        using ToolStrip toolStrip1 = new();
        using ToolStrip toolStrip2 = new();
        ToolStripItem[] items = CreateToolStripItems(("key1", "Item1"), ("key2", "Item2"));
        toolStrip1.Items.AddRange(items);

        ToolStripItemCollection collection = new(toolStrip2, Array.Empty<ToolStripItem>());
        collection.MoveItem(items[0]);

        toolStrip1.Items.Contains(items[0]).Should().BeFalse();
        toolStrip1.Items.Contains(items[1]).Should().BeTrue();
        collection.Contains(items[0]).Should().BeTrue();
    }

    [WinFormsFact]
    public void ToolStripItemCollection_MoveItem_Index_ValidItem_MovesItem()
    {
        using ToolStrip toolStrip = new();
        ToolStripItem[] items = CreateToolStripItems(("key1", "Item1"), ("key2", "Item2"), ("key3", "Item3"));
        ToolStripItemCollection collection = new(toolStrip, items);

        collection.MoveItem(index: 1, items[2]);

        collection.IndexOf(items[2]).Should().Be(1);
        collection.IndexOf(items[0]).Should().Be(0);
        collection.IndexOf(items[1]).Should().Be(2);
    }

    [WinFormsFact]
    public void ToolStripItemCollection_MoveItem_MovesItemFromToolStripToCollection()
    {
        using ToolStrip toolStrip1 = new();
        using ToolStrip toolStrip2 = new();
        ToolStripItem[] items = CreateToolStripItems(("key1", "Item1"), ("key2", "Item2"));
        toolStrip1.Items.AddRange(items);

        ToolStripItemCollection collection = new(toolStrip2, Array.Empty<ToolStripItem>());
        collection.MoveItem(index: 0, items[0]);

        toolStrip1.Items.Contains(items[0]).Should().BeFalse();
        toolStrip1.Items.Contains(items[1]).Should().BeTrue();
        collection.Contains(items[0]).Should().BeTrue();
        collection.IndexOf(items[0]).Should().Be(0);
    }

    [WinFormsFact]
    public void ToolStripItemCollection_Add_Image_Success()
    {
        using ToolStrip toolStrip = new();
        ToolStripItemCollection collection = new(toolStrip, Array.Empty<ToolStripItem>());
        using Bitmap image = new(10, 10);

        ToolStripItem item = collection.Add(image);

        item.Should().NotBeNull();
        item.Image.Should().Be(image);
    }

    [WinFormsFact]
    public void ToolStripItemCollection_Add_TextAndImage_Success()
    {
        using ToolStrip toolStrip = new();
        ToolStripItemCollection collection = new(toolStrip, Array.Empty<ToolStripItem>());
        string text = "Test";
        using Bitmap image = new(10, 10);

        ToolStripItem item = collection.Add(text, image);

        item.Should().NotBeNull();
        item.Text.Should().Be(text);
        item.Image.Should().Be(image);
    }
}
