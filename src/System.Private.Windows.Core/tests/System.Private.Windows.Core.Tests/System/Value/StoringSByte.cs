// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringSByte
{
    public static TheoryData<sbyte> SByteData => new()
    {
        { 0 },
        { 42 },
        { sbyte.MaxValue },
        { sbyte.MinValue }
    };

    [Theory]
    [MemberData(nameof(SByteData))]
    public void SByteImplicit(sbyte @sbyte)
    {
        Value value = @sbyte;
        Assert.Equal(@sbyte, value.GetValue<sbyte>());
        Assert.Equal(typeof(sbyte), value.Type);

        sbyte? source = @sbyte;
        value = source;
        Assert.Equal(source, value.GetValue<sbyte?>());
        Assert.Equal(typeof(sbyte), value.Type);
    }

    [Theory]
    [MemberData(nameof(SByteData))]
    public void SByteCreate(sbyte @sbyte)
    {
        Value value;
        using (MemoryWatch.Create)
        {
            value = Value.Create(@sbyte);
        }

        Assert.Equal(@sbyte, value.GetValue<sbyte>());
        Assert.Equal(typeof(sbyte), value.Type);

        sbyte? source = @sbyte;

        using (MemoryWatch.Create)
        {
            value = Value.Create(source);
        }

        Assert.Equal(source, value.GetValue<sbyte?>());
        Assert.Equal(typeof(sbyte), value.Type);
    }

    [Theory]
    [MemberData(nameof(SByteData))]
    public void SByteInOut(sbyte @sbyte)
    {
        Value value = new(@sbyte);
        bool success = value.TryGetValue(out sbyte result);
        Assert.True(success);
        Assert.Equal(@sbyte, result);

        Assert.Equal(@sbyte, value.GetValue<sbyte>());
        Assert.Equal(@sbyte, (sbyte)value);
    }

    [Theory]
    [MemberData(nameof(SByteData))]
    public void NullableSByteInSByteOut(sbyte @sbyte)
    {
        sbyte? source = @sbyte;
        Value value = new(source);

        bool success = value.TryGetValue(out sbyte result);
        Assert.True(success);
        Assert.Equal(@sbyte, result);

        Assert.Equal(@sbyte, value.GetValue<sbyte>());

        Assert.Equal(@sbyte, (sbyte)value);
    }

    [Theory]
    [MemberData(nameof(SByteData))]
    public void SByteInNullableSByteOut(sbyte @sbyte)
    {
        sbyte source = @sbyte;
        Value value = new(source);
        bool success = value.TryGetValue(out sbyte? result);
        Assert.True(success);
        Assert.Equal(@sbyte, result);

        Assert.Equal(@sbyte, (sbyte?)value);
    }

    [Fact]
    public void NullSByte()
    {
        sbyte? source = null;
        Value value = source;
        Assert.Null(value.Type);
        Assert.Equal(source, value.GetValue<sbyte?>());
        Assert.False(value.GetValue<sbyte?>().HasValue);
    }

    [Theory]
    [MemberData(nameof(SByteData))]
    public void OutAsObject(sbyte @sbyte)
    {
        Value value = new(@sbyte);
        object o = value.GetValue<object>();
        Assert.Equal(typeof(sbyte), o.GetType());
        Assert.Equal(@sbyte, (sbyte)o);

        sbyte? n = @sbyte;
        value = new(n);
        o = value.GetValue<object>();
        Assert.Equal(typeof(sbyte), o.GetType());
        Assert.Equal(@sbyte, (sbyte)o);
    }
}
