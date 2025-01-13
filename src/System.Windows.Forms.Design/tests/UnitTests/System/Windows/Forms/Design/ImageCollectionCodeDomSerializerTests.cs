// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class ImageCollectionCodeDomSerializerTests
{
    private readonly ImageListCodeDomSerializer _serializer;
    public ImageCollectionCodeDomSerializerTests()
    {
        _serializer = new();
    }

    [Fact]
    public void Deserialize_ShouldThrowArgumentNullException_WhenManagerIsNull()
    {
        Action act = () => _serializer.Deserialize(null!, new object());
        act.Should().Throw<ArgumentNullException>().WithMessage("*manager*");
    }

    [Fact]
    public void Deserialize_ShouldThrowArgumentNullException_WhenCodeObjectIsNull()
    {
        Action act = () => _serializer.Deserialize(new DesignerSerializationManager(), null!);
        act.Should().Throw<ArgumentNullException>().WithMessage("*codeObject*");
    }

    [Fact]
    public void Serialize_ThrowsArgumentNullException()
    {
        Action act = () => _serializer.Serialize(new DesignerSerializationManager(), null!);
        act.Should().Throw<ArgumentNullException>().WithMessage("*value*");
    }

    [Fact]
    public void Deserialize_ShouldCallBaseSerializer_WhenBaseSerializerFound()
    {
        Mock<IDesignerSerializationManager> managerMock = new();
        Mock<CodeDomSerializer> baseSerializerMock = new();

        managerMock
            .Setup(m => m.GetSerializer(typeof(Component), typeof(CodeDomSerializer)))
            .Returns(baseSerializerMock.Object);

        object codeObject = new();
        object expectedResult = new();

        baseSerializerMock
            .Setup(s => s.Deserialize(managerMock.Object, codeObject))
            .Returns(expectedResult);

        object? result = _serializer.Deserialize(managerMock.Object, codeObject);
        baseSerializerMock.Verify(s => s.Deserialize(managerMock.Object, codeObject), Times.Once);
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Deserialize_ShouldReturnNull_WhenBaseSerializerNotFound()
    {
        Mock<IDesignerSerializationManager> managerMock = new();
        Mock<CodeDomSerializer> baseSerializerMock = new();

        managerMock
            .Setup(m => m.GetSerializer(typeof(Component), typeof(CodeDomSerializer)))
            .Returns(false);

        Trace.Listeners.Clear();

        object codeObject = new();
        object? result = _serializer.Deserialize(managerMock.Object, codeObject);

        result.Should().BeNull();
    }

    [Fact]
    public void Serialize_ShouldGenerateCodeStatements_ForImageListWithKeys()
    {
        Mock<IDesignerSerializationManager> managerMock = new();
        Mock<CodeDomSerializer> baseSerializerMock = new();
        ImageList imageList = new();
        imageList.Images.Add("Key1", new Bitmap(1, 1));
        imageList.Images.Add("Key2", new Bitmap(1, 1));

        baseSerializerMock.Setup(b => b.Serialize(managerMock.Object, imageList)).Returns(new CodeStatementCollection());

        managerMock
            .Setup(m => m.GetSerializer(typeof(Component), typeof(CodeDomSerializer)))
            .Returns(baseSerializerMock.Object);

        ContextStack contextStack = new();
        contextStack.Append(new ImageListCodeDomSerializer());
        managerMock.Setup(m => m.Context).Returns(contextStack);

        object? result = _serializer.Serialize(managerMock.Object, imageList);
        result.Should().BeOfType<CodeStatementCollection>();
    }
}
