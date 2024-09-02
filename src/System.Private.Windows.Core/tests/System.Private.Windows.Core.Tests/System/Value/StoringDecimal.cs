// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringDecimal
{
    public static TheoryData<decimal> DecimalData => new()
    {
        { 42 },
        { decimal.MaxValue },
        { decimal.MinValue }
    };

    [Fact]
    public void DecimalImplicit()
    {
        Value value = (decimal)42.0;
        Assert.Equal((decimal)42.0, value.GetValue<decimal>());
        Assert.Equal(typeof(decimal), value.Type);

        decimal? source = (decimal?)42.0;
        value = source;
        Assert.Equal(source, value.GetValue<decimal?>());
        Assert.Equal(typeof(decimal), value.Type);
    }

    [Theory]
    [MemberData(nameof(DecimalData))]
    public void DecimalInOut(decimal @decimal)
    {
        Value value = new(@decimal);
        bool success = value.TryGetValue(out decimal result);
        Assert.True(success);
        Assert.Equal(@decimal, result);

        Assert.Equal(@decimal, value.GetValue<decimal>());
        Assert.Equal(@decimal, (decimal)value);
    }

    [Theory]
    [MemberData(nameof(DecimalData))]
    public void NullableDecimalInDecimalOut(decimal @decimal)
    {
        decimal? source = @decimal;
        Value value = new(source);

        bool success = value.TryGetValue(out decimal result);
        Assert.True(success);
        Assert.Equal(@decimal, result);

        Assert.Equal(@decimal, value.GetValue<decimal>());

        Assert.Equal(@decimal, (decimal)value);
    }

    [Theory]
    [MemberData(nameof(DecimalData))]
    public void DecimalInNullableDecimalOut(decimal @decimal)
    {
        decimal source = @decimal;
        Value value = new(source);
        bool success = value.TryGetValue(out decimal? result);
        Assert.True(success);
        Assert.Equal(@decimal, result);

        Assert.Equal(@decimal, (decimal?)value);
    }

    [Fact]
    public void NullDecimal()
    {
        decimal? source = null;
        Value value = source;
        Assert.Null(value.Type);
        Assert.Equal(source, value.GetValue<decimal?>());
        Assert.False(value.GetValue<decimal?>().HasValue);
    }

    [Theory]
    [MemberData(nameof(DecimalData))]
    public void OutAsObject(decimal @decimal)
    {
        Value value = new(@decimal);
        object o = value.GetValue<object>();
        Assert.Equal(typeof(decimal), o.GetType());
        Assert.Equal(@decimal, (decimal)o);

        decimal? n = @decimal;
        value = new(n);
        o = value.GetValue<object>();
        Assert.Equal(typeof(decimal), o.GetType());
        Assert.Equal(@decimal, (decimal)o);
    }
}
