// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringInt
{
    public static TheoryData<int> IntData => new()
    {
        { 0 },
        { 42 },
        { int.MaxValue },
        { int.MinValue }
    };

    [Theory]
    [MemberData(nameof(IntData))]
    public void IntImplicit(int @int)
    {
        Value value = @int;
        Assert.Equal(@int, value.GetValue<int>());
        Assert.Equal(typeof(int), value.Type);

        int? source = @int;
        value = source;
        Assert.Equal(source, value.GetValue<int?>());
        Assert.Equal(typeof(int), value.Type);
    }

    [Theory]
    [MemberData(nameof(IntData))]
    public void IntCreate(int @int)
    {
        Value value;
        using (MemoryWatch.Create)
        {
            value = Value.Create(@int);
        }

        Assert.Equal(@int, value.GetValue<int>());
        Assert.Equal(typeof(int), value.Type);

        int? source = @int;

        using (MemoryWatch.Create)
        {
            value = Value.Create(source);
        }

        Assert.Equal(source, value.GetValue<int?>());
        Assert.Equal(typeof(int), value.Type);
    }

    [Theory]
    [MemberData(nameof(IntData))]
    public void IntInOut(int @int)
    {
        Value value = new(@int);
        bool success = value.TryGetValue(out int result);
        Assert.True(success);
        Assert.Equal(@int, result);

        Assert.Equal(@int, value.GetValue<int>());
        Assert.Equal(@int, (int)value);
    }

    [Theory]
    [MemberData(nameof(IntData))]
    public void NullableIntInIntOut(int @int)
    {
        int? source = @int;
        Value value = new(source);

        bool success = value.TryGetValue(out int result);
        Assert.True(success);
        Assert.Equal(@int, result);

        Assert.Equal(@int, value.GetValue<int>());

        Assert.Equal(@int, (int)value);
    }

    [Theory]
    [MemberData(nameof(IntData))]
    public void IntInNullableIntOut(int @int)
    {
        int source = @int;
        Value value = new(source);
        Assert.True(value.TryGetValue(out int? result));
        Assert.Equal(@int, result);

        Assert.Equal(@int, (int?)value);
    }

    [Theory]
    [MemberData(nameof(IntData))]
    public void BoxedInt(int @int)
    {
        int i = @int;
        object o = i;
        Value value = new(o);

        Assert.Equal(typeof(int), value.Type);
        Assert.True(value.TryGetValue(out int result));
        Assert.Equal(@int, result);
        Assert.True(value.TryGetValue(out int? nullableResult));
        Assert.Equal(@int, nullableResult!.Value);

        int? n = @int;
        o = n;
        value = new(o);

        Assert.Equal(typeof(int), value.Type);
        Assert.True(value.TryGetValue(out result));
        Assert.Equal(@int, result);
        Assert.True(value.TryGetValue(out nullableResult));
        Assert.Equal(@int, nullableResult!.Value);
    }

    [Fact]
    public void NullInt()
    {
        int? source = null;
        Value value = source;
        Assert.Null(value.Type);
        Assert.Equal(source, value.GetValue<int?>());
        Assert.False(value.GetValue<int?>().HasValue);
    }

    [Theory]
    [MemberData(nameof(IntData))]
    public void OutAsObject(int @int)
    {
        Value value = new(@int);
        object o = value.GetValue<object>();
        Assert.Equal(typeof(int), o.GetType());
        Assert.Equal(@int, (int)o);

        int? n = @int;
        value = new(n);
        o = value.GetValue<object>();
        Assert.Equal(typeof(int), o.GetType());
        Assert.Equal(@int, (int)o);
    }
}
