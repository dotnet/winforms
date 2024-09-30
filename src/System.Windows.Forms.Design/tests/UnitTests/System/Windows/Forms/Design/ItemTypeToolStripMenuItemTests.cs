// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design.Tests
{
    public class ItemTypeToolStripMenuItemTests
    {
        [Fact]
        public void Constructor_SetsItemType()
        {
            using ItemTypeToolStripMenuItem item = new(typeof(string));
            item.ItemType.Should().Be(typeof(string));
        }

        [Fact]
        public void ConvertTo_SetAndGet()
        {
            using ItemTypeToolStripMenuItem item = new(typeof(string)) { ConvertTo = true };
            item.ConvertTo.Should().BeTrue();
        }

        [Fact]
        public void Image_Get_ReturnsCorrectImage()
        {
            using ItemTypeToolStripMenuItem item = new(typeof(string));
            item.Image.Should().BeOfType<Bitmap>().Which.Should().Be(ToolStripDesignerUtils.GetToolboxBitmap(typeof(string)));
        }

        [Fact]
        public void Text_ReturnsCorrectDescription()
        {
            using ItemTypeToolStripMenuItem item = new(typeof(string));
            item.Text.Should().BeOfType<string>().Which.Should().Be(ToolStripDesignerUtils.GetToolboxDescription(typeof(string)));
        }

        [Fact]
        public void TbxItem_SetAndGet()
        {
            ToolboxItem toolboxItem = new(typeof(string));
            using ItemTypeToolStripMenuItem item = new(typeof(string)) { TbxItem = toolboxItem };
            item.TbxItem.Should().Be(toolboxItem);
        }

        [Fact]
        public void Dispose_SetsTbxItemToNull()
        {
            using ItemTypeToolStripMenuItem item = new(typeof(string));
            item.Dispose();
            item.TbxItem.Should().BeNull();
        }
    }
}
