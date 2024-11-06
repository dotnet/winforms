// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Moq;

namespace System.Windows.Forms.Design.Serialization.Tests;

public class CodeDomSerializerExceptionTests
{
    public static IEnumerable<object[]> Ctor_String_CodeLinePragma_TestData()
    {
        yield return new object[] { "message", new CodeLinePragma() };
        yield return new object[] { null, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_String_CodeLinePragma_TestData))]
    public void CodeDomSerializerException_Ctor_String_CodeLinePragma(string message, CodeLinePragma linePragma)
    {
        CodeDomSerializerException exception = new(message, linePragma);
        Assert.NotEmpty(exception.Message);
        Assert.Null(exception.InnerException);
        Assert.Same(linePragma, exception.LinePragma);
    }

    public static IEnumerable<object[]> Ctor_Exception_CodeLinePragma_TestData()
    {
        yield return new object[] { new InvalidOperationException(), new CodeLinePragma() };
        yield return new object[] { null, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_Exception_CodeLinePragma_TestData))]
    public void CodeDomSerializerException_Ctor_Exception_CodeLinePragma(Exception innerException, CodeLinePragma linePragma)
    {
        CodeDomSerializerException exception = new(innerException, linePragma);
        Assert.NotEmpty(exception.Message);
        Assert.Same(innerException, exception.InnerException);
        Assert.Same(linePragma, exception.LinePragma);
    }

    public static IEnumerable<object[]> Ctor_String_IDesignerSerializationManager_TestData()
    {
        Mock<IDesignerSerializationManager> mockDesignerSerializationManager = new(MockBehavior.Strict);
        yield return new object[] { "message", mockDesignerSerializationManager.Object };
        yield return new object[] { null, mockDesignerSerializationManager.Object };
    }

    [Theory]
    [MemberData(nameof(Ctor_String_IDesignerSerializationManager_TestData))]
    public void CodeDomSerializerException_Ctor_String_IDesignerSerializationManager(string message, IDesignerSerializationManager manager)
    {
        CodeDomSerializerException exception = new(message, manager);
        Assert.NotEmpty(exception.Message);
        Assert.Null(exception.InnerException);
        Assert.Null(exception.LinePragma);
    }

    public static IEnumerable<object[]> Ctor_Exception_IDesignerSerializationManager_TestData()
    {
        Mock<IDesignerSerializationManager> mockDesignerSerializationManager = new(MockBehavior.Strict);
        yield return new object[] { new InvalidOperationException(), mockDesignerSerializationManager.Object };
        yield return new object[] { null, mockDesignerSerializationManager.Object };
    }

    [Theory]
    [MemberData(nameof(Ctor_Exception_IDesignerSerializationManager_TestData))]
    public void CodeDomSerializerException_Ctor_Exception_IDesignerSerializationManager(Exception innerException, IDesignerSerializationManager manager)
    {
        CodeDomSerializerException exception = new(innerException, manager);
        Assert.NotEmpty(exception.Message);
        Assert.Same(innerException, exception.InnerException);
        Assert.Null(exception.LinePragma);
    }

    [Fact]
    public void CodeDomSerializerException_NullManager_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("manager", () => new CodeDomSerializerException("message", (IDesignerSerializationManager)null));
        Assert.Throws<ArgumentNullException>("manager", () => new CodeDomSerializerException(new InvalidOperationException(), (IDesignerSerializationManager)null));
    }

    [Theory]
    [BoolData]
    public void CodeDomSerializerException_Serialize_ThrowsSerializationException(bool formatterEnabled)
    {
        using BinaryFormatterScope formatterScope = new(enable: formatterEnabled);
        using MemoryStream stream = new();
        BinaryFormatter formatter = new();
        CodeDomSerializerException exception = new("message", new CodeLinePragma("fileName.cs", 11));
        if (formatterEnabled)
        {
            Assert.Throws<SerializationException>(() => formatter.Serialize(stream, exception));
        }
        else
        {
            Assert.Throws<NotSupportedException>(() => formatter.Serialize(stream, exception));
        }
    }

    [Fact]
    public void CodeDomSerializerException_GetObjectData_ThrowsPlatformNotSupportedException()
    {
        CodeDomSerializerException exception = new("message", new CodeLinePragma("fileName.cs", 11));
        Assert.Throws<PlatformNotSupportedException>(() => exception.GetObjectData(null, default));
    }
}
