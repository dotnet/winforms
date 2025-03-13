// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

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

        ToolStripMenuItem dummyItem = new();
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

        ToolStripMenuItem nonDummyItem = new();
        ToolStripDropDown dropDown = new();
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
}
