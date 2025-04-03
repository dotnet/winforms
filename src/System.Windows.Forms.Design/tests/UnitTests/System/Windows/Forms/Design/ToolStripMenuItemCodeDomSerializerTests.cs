// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripMenuItemCodeDomSerializerTests
{
    private readonly Mock<IDesignerSerializationManager> _mockManager;
    private readonly Mock<CodeDomSerializer> _mockBaseSerializer;
    private readonly ToolStripMenuItemCodeDomSerializer _serializer;

    public ToolStripMenuItemCodeDomSerializerTests()
    {
        _mockManager = new();
        _mockBaseSerializer = new();

        _mockManager.Setup(m => m.GetSerializer(typeof(Component), typeof(CodeDomSerializer)))
                    .Returns(_mockBaseSerializer.Object);

        _serializer = new();
    }

    [Fact]
    public void Deserialize_CallsBaseSerializer()
    {
        object codeObject = new();
        object? result = _serializer.Deserialize(_mockManager.Object, codeObject);

        _mockBaseSerializer.Verify(s => s.Deserialize(_mockManager.Object, codeObject), Times.Once);
    }

    [Fact]
    public void Serialize_DoesNotSerializeDummyItem()
    {
        _mockManager.Setup(m => m.GetSerializer(typeof(ImageList).BaseType, typeof(CodeDomSerializer)))
                    .Returns(_mockBaseSerializer.Object);

        using ToolStripMenuItem dummyItem = new();
        Mock<ToolStrip> mockParent = new();
        mockParent.Setup(p => p.Site).Returns((ISite?)null);
        dummyItem.TestAccessor().Dynamic.Parent = mockParent.Object;

        object? result = _serializer.Serialize(_mockManager.Object, dummyItem);

        result.Should().BeNull();
    }

    [Fact]
    public void Serialize_CallsBaseSerializerForNonDummyItem()
    {
        _mockManager.Setup(m => m.GetSerializer(typeof(ImageList).BaseType, typeof(CodeDomSerializer)))
                    .Returns(_mockBaseSerializer.Object);

        using ToolStripMenuItem nonDummyItem = new();
        using ToolStripDropDown dropDown = new();
        dropDown.Items.Add(nonDummyItem);

        Mock<ISite> mockSite = new();
        dropDown.Site = mockSite.Object;

        Mock<ISite> mockMenuItemSite = new();
        nonDummyItem.Site = mockMenuItemSite.Object;

        _mockBaseSerializer.Setup(s => s.Serialize(_mockManager.Object, nonDummyItem))
                           .Returns(new object());

        object? result = _serializer.Serialize(_mockManager.Object, nonDummyItem);

        result.Should().NotBeNull();
        _mockBaseSerializer.Verify(s => s.Serialize(_mockManager.Object, nonDummyItem), Times.Once);
    }

    [Fact]
    public void Serialize_SerializesDropDownItemsCorrectly()
    {
        _mockManager
            .Setup(m => m.GetSerializer(typeof(ToolStripMenuItem), typeof(CodeDomSerializer)))
            .Returns(_mockBaseSerializer.Object);

        using ToolStripMenuItem parentItem = new() { Text = "Edit" };
        using ToolStripMenuItem childItem = new() { Text = "Undo" };
        parentItem.DropDownItems.Add(childItem);

        _mockBaseSerializer.Setup(s => s.Serialize(_mockManager.Object, parentItem))
                           .Returns(new { parentItem.Text, DropDownItems = new[] { new { childItem.Text } } });

        object? result = _serializer.Serialize(_mockManager.Object, parentItem);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new { Text = "Edit", DropDownItems = new[] { new { Text = "Undo" } } });
        _mockBaseSerializer.Verify(s => s.Serialize(_mockManager.Object, parentItem), Times.Once);
    }

    [Fact]
    public void Serialize_SerializesEnabledPropertyCorrectly()
    {
        _mockManager.Setup(m => m.GetSerializer(typeof(ToolStripMenuItem), typeof(CodeDomSerializer)))
                    .Returns(_mockBaseSerializer.Object);

        using ToolStripMenuItem item = new() { Enabled = false };
        _mockBaseSerializer.Setup(s => s.Serialize(_mockManager.Object, item))
                           .Returns(new { item.Enabled });

        object? result = _serializer.Serialize(_mockManager.Object, item);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new { Enabled = false });
        _mockBaseSerializer.Verify(s => s.Serialize(_mockManager.Object, item), Times.Once);
    }

    [Fact]
    public void Serialize_SerializesCheckedAndTextCorrectly()
    {
        _mockManager.Setup(m => m.GetSerializer(typeof(ToolStripMenuItem), typeof(CodeDomSerializer)))
                    .Returns(_mockBaseSerializer.Object);

        using ToolStripMenuItem parentItem = new() { Text = "File", Checked = true };
        using ToolStripMenuItem childItem = new() { Text = "New", Checked = false };
        parentItem.DropDownItems.Add(childItem);

        _mockBaseSerializer.Setup(s => s.Serialize(_mockManager.Object, parentItem))
                           .Returns(new { parentItem.Text, parentItem.Checked, DropDownItems = new[] { new { childItem.Text, childItem.Checked } } });

        object? result = _serializer.Serialize(_mockManager.Object, parentItem);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new { Text = "File", Checked = true, DropDownItems = new[] { new { Text = "New", Checked = false } } });
        _mockBaseSerializer.Verify(s => s.Serialize(_mockManager.Object, parentItem), Times.Once);
    }

    [Fact]
    public void Serialize_SerializesImagePropertyCorrectly()
    {
        _mockManager.Setup(m => m.GetSerializer(typeof(ToolStripMenuItem), typeof(CodeDomSerializer)))
                    .Returns(_mockBaseSerializer.Object);

        using Image testImage = new Bitmap(1, 1);
        using ToolStripMenuItem item = new() { Image = testImage };

        _mockBaseSerializer.Setup(s => s.Serialize(_mockManager.Object, item))
                           .Returns(new { item.Image });

        object? result = _serializer.Serialize(_mockManager.Object, item);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new { Image = testImage });
        _mockBaseSerializer.Verify(s => s.Serialize(_mockManager.Object, item), Times.Once);
    }
}
