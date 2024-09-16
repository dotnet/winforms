// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringObject
{
    [Fact]
    public void BasicStorage()
    {
        A a = new();
        Value value = new(a);
        Assert.Equal(typeof(A), value.Type);
        Assert.Same(a, value.GetValue<A>());

        bool success = value.TryGetValue(out B result);
        Assert.False(success);
        Assert.Null(result);
    }

    [Fact]
    public void DerivedRetrieval()
    {
        B b = new();
        Value value = new(b);
        Assert.Equal(typeof(B), value.Type);
        Assert.Same(b, value.GetValue<A>());
        Assert.Same(b, value.GetValue<B>());

        bool success = value.TryGetValue(out C result);
        Assert.False(success);
        Assert.Null(result);

        Assert.Throws<InvalidCastException>(() => value.GetValue<C>());

        A a = new B();
        value = new(a);
        Assert.Equal(typeof(B), value.Type);
    }

    [Fact]
    public void AsInterface()
    {
        I a = new A();
        Value value = new(a);
        Assert.Equal(typeof(A), value.Type);

        Assert.Same(a, value.GetValue<A>());
        Assert.Same(a, value.GetValue<I>());
    }

    private class A : I { }
    private class B : A, I { }
    private class C : B, I { }

    private interface I
    {
        string? ToString();
    }
}
