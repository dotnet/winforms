// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringArrays
{
    [Fact]
    public void ByteArray()
    {
        byte[] b = new byte[10];
        Value value;

        value = Value.Create(b);
        Assert.Equal(typeof(byte[]), value.Type);
        Assert.Same(b, value.GetValue<byte[]>());
        Assert.Equal(b, (byte[])value.GetValue<object>());

        Assert.Throws<InvalidCastException>(() => value.GetValue<ArraySegment<byte>>());
    }

    [Fact]
    public void CharArray()
    {
        char[] b = new char[10];
        Value value;

        value = Value.Create(b);
        Assert.Equal(typeof(char[]), value.Type);
        Assert.Same(b, value.GetValue<char[]>());
        Assert.Equal(b, (char[])value.GetValue<object>());

        Assert.Throws<InvalidCastException>(() => value.GetValue<ArraySegment<char>>());
    }

    [Fact]
    public void ByteSegment()
    {
        byte[] b = new byte[10];
        Value value;

        ArraySegment<byte> segment = new(b);
        value = Value.Create(segment);
        Assert.Equal(typeof(ArraySegment<byte>), value.Type);
        Assert.Equal(segment, value.GetValue<ArraySegment<byte>>());
        Assert.Equal(segment, (ArraySegment<byte>)value.GetValue<object>());
        Assert.Throws<InvalidCastException>(() => value.GetValue<byte[]>());

        segment = new(b, 0, 0);
        value = Value.Create(segment);
        Assert.Equal(typeof(ArraySegment<byte>), value.Type);
        Assert.Equal(segment, value.GetValue<ArraySegment<byte>>());
        Assert.Equal(segment, (ArraySegment<byte>)value.GetValue<object>());
        Assert.Throws<InvalidCastException>(() => value.GetValue<byte[]>());

        segment = new(b, 1, 1);
        value = Value.Create(segment);
        Assert.Equal(typeof(ArraySegment<byte>), value.Type);
        Assert.Equal(segment, value.GetValue<ArraySegment<byte>>());
        Assert.Equal(segment, (ArraySegment<byte>)value.GetValue<object>());
        Assert.Throws<InvalidCastException>(() => value.GetValue<byte[]>());
    }

    [Fact]
    public void CharSegment()
    {
        char[] b = new char[10];
        Value value;

        ArraySegment<char> segment = new(b);
        value = Value.Create(segment);
        Assert.Equal(typeof(ArraySegment<char>), value.Type);
        Assert.Equal(segment, value.GetValue<ArraySegment<char>>());
        Assert.Equal(segment, (ArraySegment<char>)value.GetValue<object>());
        Assert.Throws<InvalidCastException>(() => value.GetValue<char[]>());

        segment = new(b, 0, 0);
        value = Value.Create(segment);
        Assert.Equal(typeof(ArraySegment<char>), value.Type);
        Assert.Equal(segment, value.GetValue<ArraySegment<char>>());
        Assert.Equal(segment, (ArraySegment<char>)value.GetValue<object>());
        Assert.Throws<InvalidCastException>(() => value.GetValue<char[]>());

        segment = new(b, 1, 1);
        value = Value.Create(segment);
        Assert.Equal(typeof(ArraySegment<char>), value.Type);
        Assert.Equal(segment, value.GetValue<ArraySegment<char>>());
        Assert.Equal(segment, (ArraySegment<char>)value.GetValue<object>());
        Assert.Throws<InvalidCastException>(() => value.GetValue<char[]>());
    }
}
