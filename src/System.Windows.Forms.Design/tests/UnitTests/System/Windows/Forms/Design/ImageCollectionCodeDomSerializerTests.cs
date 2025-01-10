// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
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
        var managerMock = new Mock<IDesignerSerializationManager>();
        var baseSerializerMock = new Mock<CodeDomSerializer>();

        managerMock.Setup(m => m.GetSerializer(typeof(Component), typeof(CodeDomSerializer)))
                 .Returns(baseSerializerMock.Object);

        var codeObject = new object();
        var expectedResult = new object();

        baseSerializerMock.Setup(s => s.Deserialize(managerMock.Object, codeObject))
                          .Returns(expectedResult);

        var result = _serializer.Deserialize(managerMock.Object, codeObject);

        baseSerializerMock.Verify(s => s.Deserialize(managerMock.Object, codeObject), Times.Once);
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Deserialize_ShouldReturnNull_WhenBaseSerializerNotFound()
    {
        var managerMock = new Mock<IDesignerSerializationManager>();
        var baseSerializerMock = new Mock<CodeDomSerializer>();

        managerMock.Setup(m => m.GetSerializer(typeof(Component), typeof(CodeDomSerializer)))
                 .Returns(baseSerializerMock.Object);

        var codeObject = new object();
        var result = _serializer.Deserialize(managerMock.Object, codeObject);

        result.Should().BeNull();
    }
}
