// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class QuestionEventArgsTests
{
    [Fact]
    public void Ctor_Default()
    {
        QuestionEventArgs e = new();
        Assert.False(e.Response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ctor_Bool(bool response)
    {
        QuestionEventArgs e = new(response);
        Assert.Equal(response, e.Response);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Response_Set_GetReturnsExpected(bool value)
    {
        QuestionEventArgs e = new(!value)
        {
            Response = value
        };
        Assert.Equal(value, e.Response);
    }
}
