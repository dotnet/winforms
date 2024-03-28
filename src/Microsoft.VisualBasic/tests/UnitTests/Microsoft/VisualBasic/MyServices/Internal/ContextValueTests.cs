// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.MyServices.Internal.Tests;

public class ContextValueTests
{
    [Fact]
    public void NoValue()
    {
        Assert.Null((new ContextValue<string>()).Value);
        Assert.Throws<NullReferenceException>(() => (new ContextValue<int>()).Value);
    }

    [Fact]
    public void MultipleInstances()
    {
        ContextValue<int> context1 = new()
        {
            Value = 1
        };
        ContextValue<int> context2 = new()
        {
            Value = 2
        };
        Assert.Equal(1, context1.Value);
        Assert.Equal(2, context2.Value);
    }

    [Fact]
    public void MultipleThreads()
    {
        ContextValue<string> context = new()
        {
            Value = "Hello"
        };
        Thread thread = new(() =>
        {
            Assert.Null(context.Value);
            context.Value = "World";
            Assert.Equal("World", context.Value);
        });
        thread.Start();
        thread.Join();
        Assert.Equal("Hello", context.Value);
    }
}
