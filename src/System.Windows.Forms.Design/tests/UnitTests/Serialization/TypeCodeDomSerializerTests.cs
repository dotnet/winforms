// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Moq;

namespace System.ComponentModel.Design.Serialization.Tests;

public class TypeCodeDomSerializerTests
{
    [Fact]
    public void TypeCodeDomSerializer_Constructor()
    {
        TypeCodeDomSerializer underTest = new();
        Assert.NotNull(underTest);
    }

    [Fact]
    public void TypeCodeDomSerializer_Serialize_Manager_Null()
    {
        TypeCodeDomSerializer underTest = new();
        Assert.Throws<ArgumentNullException>(() => underTest.Serialize(null, null, null));
    }

    [Fact]
    public void TypeCodeDomSerializer_Serialize_Root_Null()
    {
        Mock<DesignerSerializationManager> mockSerializationManager = new(MockBehavior.Strict);
        TypeCodeDomSerializer underTest = new();
        Assert.Throws<ArgumentNullException>(() => underTest.Serialize(mockSerializationManager.Object, null, null));
    }

    [Fact]
    public void TypeCodeDomSerializer_Deserialize_Manager_Null()
    {
        TypeCodeDomSerializer underTest = new();
        Assert.Throws<ArgumentNullException>(() => underTest.Deserialize(null, null));
    }

    [Fact]
    public void TypeCodeDomSerializer_Deserialize_CodeTypeDec_Null()
    {
        Mock<DesignerSerializationManager> mockSerializationManager = new(MockBehavior.Strict);
        TypeCodeDomSerializer underTest = new();
        Assert.Throws<ArgumentNullException>(() => underTest.Deserialize(mockSerializationManager.Object, null));
    }
}
