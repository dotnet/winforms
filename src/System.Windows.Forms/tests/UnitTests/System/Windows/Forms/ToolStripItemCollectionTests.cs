// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        Assert.Equal(0, contextMenuStrip.Items.Count);

        // Add 3 items.
        toolStripDropDownButton.DropDownItems.Add("a");
        toolStripDropDownButton.DropDownItems.Add("b");
        toolStripDropDownButton.DropDownItems.Add("c");
        contextMenuStrip.Items.AddRange(toolStripDropDownButton.DropDownItems);
        Assert.Equal(3, contextMenuStrip.Items.Count);

        // Validate order.
        Assert.Equal("a", contextMenuStrip.Items[0].Text);
        Assert.Equal("b", contextMenuStrip.Items[1].Text);
        Assert.Equal("c", contextMenuStrip.Items[2].Text);
    }
}
