// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization.Tests;

public class RootContextTests
{
    [Fact]
    public void RootContext_Ctor_CodeExpression_Object()
    {
        CodeExpression expression = new();
        object value = new();
        RootContext context = new(expression, value);
        Assert.Same(expression, context.Expression);
        Assert.Same(value, context.Value);
    }

    [Fact]
    public void RootContext_Ctor_NullExpression_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("expression", () => new RootContext(null, new object()));
    }

    [Fact]
    public void RootContext_Ctor_NullValue_ThrowsArgumentNullException()
    {
        CodeExpression expression = new();
        Assert.Throws<ArgumentNullException>("value", () => new RootContext(expression, null));
    }
}
