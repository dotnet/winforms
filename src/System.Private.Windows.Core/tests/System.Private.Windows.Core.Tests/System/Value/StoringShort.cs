// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringShort
{
    public static TheoryData<short> ShortData => new()
    {
        { 0 },
        { 42 },
        { short.MaxValue },
        { short.MinValue }
    };

    [Theory]
    [MemberData(nameof(ShortData))]
    public void ShortImplicit(short @short)
    {
        Value value = @short;
        Assert.Equal(@short, value.GetValue<short>());
        Assert.Equal(typeof(short), value.Type);

        short? source = @short;
        value = source;
        Assert.Equal(source, value.GetValue<short?>());
        Assert.Equal(typeof(short), value.Type);
    }

    [Theory]
    [MemberData(nameof(ShortData))]
    public void ShortCreate(short @short)
    {
        Value value;
        using (MemoryWatch.Create)
        {
            value = Value.Create(@short);
        }

        Assert.Equal(@short, value.GetValue<short>());
        Assert.Equal(typeof(short), value.Type);

        short? source = @short;

        using (MemoryWatch.Create)
        {
            value = Value.Create(source);
        }

        Assert.Equal(source, value.GetValue<short?>());
        Assert.Equal(typeof(short), value.Type);
    }

    [Theory]
    [MemberData(nameof(ShortData))]
    public void ShortInOut(short @short)
    {
        Value value = new(@short);
        bool success = value.TryGetValue(out short result);
        Assert.True(success);
        Assert.Equal(@short, result);

        Assert.Equal(@short, value.GetValue<short>());
        Assert.Equal(@short, (short)value);
    }

    [Theory]
    [MemberData(nameof(ShortData))]
    public void NullableShortInShortOut(short @short)
    {
        short? source = @short;
        Value value = new(source);

        bool success = value.TryGetValue(out short result);
        Assert.True(success);
        Assert.Equal(@short, result);

        Assert.Equal(@short, value.GetValue<short>());

        Assert.Equal(@short, (short)value);
    }

    [Theory]
    [MemberData(nameof(ShortData))]
    public void ShortInNullableShortOut(short @short)
    {
        short source = @short;
        Value value = new(source);
        bool success = value.TryGetValue(out short? result);
        Assert.True(success);
        Assert.Equal(@short, result);

        Assert.Equal(@short, (short?)value);
    }

    [Theory]
    [MemberData(nameof(ShortData))]
    public void BoxedShort(short @short)
    {
        short i = @short;
        object o = i;
        Value value = new(o);

        Assert.Equal(typeof(short), value.Type);
        Assert.True(value.TryGetValue(out short result));
        Assert.Equal(@short, result);
        Assert.True(value.TryGetValue(out short? nullableResult));
        Assert.Equal(@short, nullableResult!.Value);

        short? n = @short;
        o = n;
        value = new(o);

        Assert.Equal(typeof(short), value.Type);
        Assert.True(value.TryGetValue(out result));
        Assert.Equal(@short, result);
        Assert.True(value.TryGetValue(out nullableResult));
        Assert.Equal(@short, nullableResult!.Value);
    }

    [Fact]
    public void NullShort()
    {
        short? source = null;
        Value value = source;
        Assert.Null(value.Type);
        Assert.Equal(source, value.GetValue<short?>());
        Assert.False(value.GetValue<short?>().HasValue);
    }

    [Theory]
    [MemberData(nameof(ShortData))]
    public void OutAsObject(short @short)
    {
        Value value = new(@short);
        object o = value.GetValue<object>();
        Assert.Equal(typeof(short), o.GetType());
        Assert.Equal(@short, (short)o);

        short? n = @short;
        value = new(n);
        o = value.GetValue<object>();
        Assert.Equal(typeof(short), o.GetType());
        Assert.Equal(@short, (short)o);
    }
}
