// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using Moq;

namespace System.Windows.Forms.Design.Tests.System.Windows.Forms.Design;

public class TableLayoutPanelCodeDomSerializerTests
{
    private readonly Mock<IDesignerSerializationManager> _mockManager;
    private readonly Mock<CodeDomSerializer> _mockBaseSerializer;
    private readonly TableLayoutPanelCodeDomSerializer _serializer;

    public TableLayoutPanelCodeDomSerializerTests()
    {
        _mockManager = new();
        _mockBaseSerializer = new();
        _serializer = new();
        _mockManager.Setup(m => m.GetSerializer(typeof(TableLayoutPanel).BaseType, typeof(CodeDomSerializer)))
            .Returns(_mockBaseSerializer.Object);
    }

    [Fact]
    public void Deserialize_ValidManager_CallsBaseSerializer()
    {
        _mockManager.Setup(m => m.GetSerializer(typeof(TableLayoutPanel).BaseType, typeof(CodeDomSerializer)))
            .Returns(_mockBaseSerializer.Object);

        object codeObject = new();

        object? result = _serializer.Deserialize(_mockManager.Object, codeObject);

        _mockBaseSerializer.Verify(s => s.Deserialize(_mockManager.Object, codeObject), Times.Once);
        result.Should().Be(_mockBaseSerializer.Object.Deserialize(_mockManager.Object, codeObject));
    }

    [Fact]
    public void Serialize_TableLayoutPanel_Not_Localizable_ReturnsBaseSerializerResult()
    {
        TableLayoutPanel tableLayoutPanel = new();

        object? result = _serializer.Serialize(_mockManager.Object, tableLayoutPanel);

        _mockBaseSerializer.Verify(s => s.Serialize(_mockManager.Object, tableLayoutPanel), Times.Once);
        result.Should().Be(_mockBaseSerializer.Object.Serialize(_mockManager.Object, tableLayoutPanel));
    }
}
