// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.CodeDom;
using System.ComponentModel.Design.Serialization;

namespace System.Windows.Forms.Design.Serialization.Tests;

public class PrimitiveCodeDomSerializerTests
{
    [Fact]
    public void PrimitiveCodeDomSerializerTests_Constructor()
    {
        PrimitiveCodeDomSerializer underTest = new();
        Assert.NotNull(underTest);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(false)]
    [InlineData("some string")]
    [InlineData('c')]
    [InlineData(42)]
    [InlineData(42F)]
    [InlineData(42.123)]
    public void PrimitiveCodeDomSerializerTests_Serialize(object input)
    {
        DesignerSerializationManager manager = new();
        PrimitiveCodeDomSerializer underTest = PrimitiveCodeDomSerializer.Default;
        Assert.NotNull(underTest);

        var result = underTest.Serialize(manager, input) as CodePrimitiveExpression;
        Assert.NotNull(result);
        Assert.Equal(input, result.Value);
        Assert.Empty(result.UserData);
    }

    [Fact]
    public void PrimitiveCodeDomSerializerTests_Serialize_Cast()
    {
        DesignerSerializationManager manager = new();
        PrimitiveCodeDomSerializer underTest = PrimitiveCodeDomSerializer.Default;
        Assert.NotNull(underTest);

        var cast = underTest.Serialize(manager, (byte)42) as CodeCastExpression;

        Assert.NotNull(cast);
        Assert.Equal(typeof(byte).ToString(), cast.TargetType.BaseType);
        Assert.IsType<CodePrimitiveExpression>(cast.Expression);
        var primitive = cast.Expression as CodePrimitiveExpression;
        Assert.Equal((byte)42, primitive.Value);
        Assert.Empty(cast.UserData);
    }
}
