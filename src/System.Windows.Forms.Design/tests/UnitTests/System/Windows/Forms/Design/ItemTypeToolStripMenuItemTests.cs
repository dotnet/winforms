// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design.Tests;

public class ItemTypeToolStripMenuItemTests : IDisposable
{
    private readonly ItemTypeToolStripMenuItem _item;

    public ItemTypeToolStripMenuItemTests()
    {
        _item = new ItemTypeToolStripMenuItem(typeof(string));
    }

    [Fact]
    public void Constructor_SetsItemType()
    {
        _item.ItemType.Should().Be(typeof(string));
    }

    [Fact]
    public void ConvertTo_SetAndGet()
    {
        _item.ConvertTo = true;
        _item.ConvertTo.Should().BeTrue();
    }

    [Fact]
    public void Image_Get_ReturnsCorrectImage()
    {
        _item.Image.Should().BeOfType<Bitmap>().Which.Should().Be(ToolStripDesignerUtils.GetToolboxBitmap(typeof(string)));
    }

    [Fact]
    public void Text_ReturnsCorrectDescription()
    {
        _item.Text.Should().BeOfType<string>().Which.Should().Be(ToolStripDesignerUtils.GetToolboxDescription(typeof(string)));
    }

    [Fact]
    public void ToolboxItem_SetAndGet()
    {
        ToolboxItem toolboxItem = new(typeof(string));
        _item.ToolboxItem = toolboxItem;
        _item.ToolboxItem.Should().Be(toolboxItem);
    }

    [Fact]
    public void Dispose_SetsToolboxItemToNull()
    {
        ItemTypeToolStripMenuItem item = new(typeof(string));
        item.Dispose();
        item.ToolboxItem.Should().BeNull();
    }

    public void Dispose()
    {
        _item.Dispose();
    }
}
