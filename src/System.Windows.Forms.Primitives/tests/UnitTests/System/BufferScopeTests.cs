// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public unsafe class BufferScopeTests
{
    [Fact]
    public void Construct_WithStackAlloc()
    {
        using BufferScope<char> buffer = new(stackalloc char[10]);
        Assert.Equal(10, buffer.Length);
        buffer[0] = 'Y';
        Assert.Equal("Y", buffer[..1].ToString());
    }

    [Fact]
    public void Construct_WithStackAlloc_GrowAndCopy()
    {
        using BufferScope<char> buffer = new(stackalloc char[10]);
        Assert.Equal(10, buffer.Length);
        buffer[0] = 'Y';
        buffer.EnsureCapacity(64, copy: true);
        Assert.True(buffer.Length >= 64);
        Assert.Equal("Y", buffer[..1].ToString());
    }

    [Fact]
    public void Construct_WithStackAlloc_Pin()
    {
        using BufferScope<char> buffer = new(stackalloc char[10]);
        Assert.Equal(10, buffer.Length);
        buffer[0] = 'Y';
        fixed (char* c = buffer)
        {
            Assert.Equal('Y', *c);
            *c = 'Z';
        }

        Assert.Equal("Z", buffer[..1].ToString());
    }

    [Fact]
    public void Construct_GrowAndCopy()
    {
        using BufferScope<char> buffer = new(32);
        Assert.True(buffer.Length >= 32);
        buffer[0] = 'Y';
        buffer.EnsureCapacity(64, copy: true);
        Assert.True(buffer.Length >= 64);
        Assert.Equal("Y", buffer[..1].ToString());
    }

    [Fact]
    public void Construct_Pin()
    {
        using BufferScope<char> buffer = new(64);
        Assert.True(buffer.Length >= 64);
        buffer[0] = 'Y';
        fixed (char* c = buffer)
        {
            Assert.Equal('Y', *c);
            *c = 'Z';
        }

        Assert.Equal("Z", buffer[..1].ToString());
    }
}
