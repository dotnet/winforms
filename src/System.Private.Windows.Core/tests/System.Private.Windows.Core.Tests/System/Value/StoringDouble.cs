// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringDouble
{
    public static TheoryData<double> DoubleData => new()
    {
        { 0d },
        { 42d },
        { double.MaxValue },
        { double.MinValue },
        { double.NaN },
        { double.NegativeInfinity },
        { double.PositiveInfinity }
    };

    [Theory]
    [MemberData(nameof(DoubleData))]
    public void DoubleImplicit(double @double)
    {
        Value value = @double;
        Assert.Equal(@double, value.GetValue<double>());
        Assert.Equal(typeof(double), value.Type);

        double? source = @double;
        value = source;
        Assert.Equal(source, value.GetValue<double?>());
        Assert.Equal(typeof(double), value.Type);
    }

    [Theory]
    [MemberData(nameof(DoubleData))]
    public void DoubleCreate(double @double)
    {
        Value value;
        using (MemoryWatch.Create)
        {
            value = Value.Create(@double);
        }

        Assert.Equal(@double, value.GetValue<double>());
        Assert.Equal(typeof(double), value.Type);

        double? source = @double;

        using (MemoryWatch.Create)
        {
            value = Value.Create(source);
        }

        Assert.Equal(source, value.GetValue<double?>());
        Assert.Equal(typeof(double), value.Type);
    }

    [Theory]
    [MemberData(nameof(DoubleData))]
    public void DoubleInOut(double @double)
    {
        Value value = new(@double);
        bool success = value.TryGetValue(out double result);
        Assert.True(success);
        Assert.Equal(@double, result);

        Assert.Equal(@double, value.GetValue<double>());
        Assert.Equal(@double, (double)value);
    }

    [Theory]
    [MemberData(nameof(DoubleData))]
    public void NullableDoubleInDoubleOut(double @double)
    {
        double? source = @double;
        Value value = new(source);

        bool success = value.TryGetValue(out double result);
        Assert.True(success);
        Assert.Equal(@double, result);

        Assert.Equal(@double, value.GetValue<double>());

        Assert.Equal(@double, (double)value);
    }

    [Theory]
    [MemberData(nameof(DoubleData))]
    public void DoubleInNullableDoubleOut(double @double)
    {
        double source = @double;
        Value value = new(source);
        bool success = value.TryGetValue(out double? result);
        Assert.True(success);
        Assert.Equal(@double, result);

        Assert.Equal(@double, (double)value);
    }

    [Theory]
    [MemberData(nameof(DoubleData))]
    public void BoxedDouble(double @double)
    {
        double i = @double;
        object o = i;
        Value value = new(o);

        Assert.Equal(typeof(double), value.Type);
        Assert.True(value.TryGetValue(out double result));
        Assert.Equal(@double, result);
        Assert.True(value.TryGetValue(out double? nullableResult));
        Assert.Equal(@double, nullableResult!.Value);

        double? n = @double;
        o = n;
        value = new(o);

        Assert.Equal(typeof(double), value.Type);
        Assert.True(value.TryGetValue(out result));
        Assert.Equal(@double, result);
        Assert.True(value.TryGetValue(out nullableResult));
        Assert.Equal(@double, nullableResult!.Value);
    }

    [Fact]
    public void NullDouble()
    {
        double? source = null;
        Value value = source;
        Assert.Null(value.Type);
        Assert.Equal(source, value.GetValue<double?>());
        Assert.False(value.GetValue<double?>().HasValue);
    }

    [Theory]
    [MemberData(nameof(DoubleData))]
    public void OutAsObject(double @double)
    {
        Value value = new(@double);
        object o = value.GetValue<object>();
        Assert.Equal(typeof(double), o.GetType());
        Assert.Equal(@double, (double)o);

        double? n = @double;
        value = new(n);
        o = value.GetValue<object>();
        Assert.Equal(typeof(double), o.GetType());
        Assert.Equal(@double, (double)o);
    }
}
