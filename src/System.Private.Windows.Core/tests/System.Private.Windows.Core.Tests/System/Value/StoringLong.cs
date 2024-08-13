// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringLong
{
    public static TheoryData<long> LongData => new()
    {
        { 0 },
        { 42 },
        { long.MaxValue },
        { long.MinValue }
    };

    [Theory]
    [MemberData(nameof(LongData))]
    public void LongImplicit(long @long)
    {
        Value value = @long;
        Assert.Equal(@long, value.GetValue<long>());
        Assert.Equal(typeof(long), value.Type);

        long? source = @long;
        value = source;
        Assert.Equal(source, value.GetValue<long>());
        Assert.Equal(typeof(long), value.Type);
    }

    [Theory]
    [MemberData(nameof(LongData))]
    public void LongCreate(long @long)
    {
        Value value;
        using (MemoryWatch.Create)
        {
            value = Value.Create(@long);
        }

        Assert.Equal(@long, value.GetValue<long>());
        Assert.Equal(typeof(long), value.Type);

        long? source = @long;

        using (MemoryWatch.Create)
        {
            value = Value.Create(source);
        }

        Assert.Equal(source, value.GetValue<long?>());
        Assert.Equal(typeof(long), value.Type);
    }

    [Theory]
    [MemberData(nameof(LongData))]
    public void LongInOut(long @long)
    {
        Value value = new(@long);
        bool success = value.TryGetValue(out long result);
        Assert.True(success);
        Assert.Equal(@long, result);

        Assert.Equal(@long, value.GetValue<long>());
        Assert.Equal(@long, (long)value);
    }

    [Theory]
    [MemberData(nameof(LongData))]
    public void NullableLongInLongOut(long @long)
    {
        long? source = @long;
        Value value = new(source);

        bool success = value.TryGetValue(out long result);
        Assert.True(success);
        Assert.Equal(@long, result);

        Assert.Equal(@long, value.GetValue<long>());

        Assert.Equal(@long, (long)value);
    }

    [Theory]
    [MemberData(nameof(LongData))]
    public void LongInNullableLongOut(long @long)
    {
        long source = @long;
        Value value = new(source);
        bool success = value.TryGetValue(out long? result);
        Assert.True(success);
        Assert.Equal(@long, result);

        Assert.Equal(@long, (long?)value);
    }

    [Theory]
    [MemberData(nameof(LongData))]
    public void BoxedLong(long @long)
    {
        long i = @long;
        object o = i;
        Value value = new(o);

        Assert.Equal(typeof(long), value.Type);
        Assert.True(value.TryGetValue(out long result));
        Assert.Equal(@long, result);
        Assert.True(value.TryGetValue(out long? nullableResult));
        Assert.Equal(@long, nullableResult!.Value);

        long? n = @long;
        o = n;
        value = new(o);

        Assert.Equal(typeof(long), value.Type);
        Assert.True(value.TryGetValue(out result));
        Assert.Equal(@long, result);
        Assert.True(value.TryGetValue(out nullableResult));
        Assert.Equal(@long, nullableResult!.Value);
    }

    [Fact]
    public void NullLong()
    {
        long? source = null;
        Value value = source;
        Assert.Null(value.Type);
        Assert.Equal(source, value.GetValue<long?>());
        Assert.False(value.GetValue<long?>().HasValue);
    }

    [Theory]
    [MemberData(nameof(LongData))]
    public void OutAsObject(long @long)
    {
        Value value = new(@long);
        object o = value.GetValue<object>();
        Assert.Equal(typeof(long), o.GetType());
        Assert.Equal(@long, (long)o);

        long? n = @long;
        value = new(n);
        o = value.GetValue<object>();
        Assert.Equal(typeof(long), o.GetType());
        Assert.Equal(@long, (long)o);
    }
}
