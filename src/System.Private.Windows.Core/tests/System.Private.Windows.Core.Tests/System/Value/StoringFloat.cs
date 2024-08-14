// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ValueTests;

public class StoringFloat
{
    public static TheoryData<float> FloatData => new()
    {
        { 0f },
        { 42f },
        { float.MaxValue },
        { float.MinValue },
        { float.NaN },
        { float.NegativeInfinity },
        { float.PositiveInfinity }
    };

    [Theory]
    [MemberData(nameof(FloatData))]
    public void FloatImplicit(float @float)
    {
        Value value = @float;
        Assert.Equal(@float, value.GetValue<float>());
        Assert.Equal(typeof(float), value.Type);

        float? source = @float;
        value = source;
        Assert.Equal(source, value.GetValue<float?>());
        Assert.Equal(typeof(float), value.Type);
    }

    [Theory]
    [MemberData(nameof(FloatData))]
    public void FloatCreate(float @float)
    {
        Value value;
        using (MemoryWatch.Create)
        {
            value = Value.Create(@float);
        }

        Assert.Equal(@float, value.GetValue<float>());
        Assert.Equal(typeof(float), value.Type);

        float? source = @float;

        using (MemoryWatch.Create)
        {
            value = Value.Create(source);
        }

        Assert.Equal(source, value.GetValue<float?>());
        Assert.Equal(typeof(float), value.Type);
    }

    [Theory]
    [MemberData(nameof(FloatData))]
    public void FloatInOut(float @float)
    {
        Value value = new(@float);
        bool success = value.TryGetValue(out float result);
        Assert.True(success);
        Assert.Equal(@float, result);

        Assert.Equal(@float, value.GetValue<float>());
        Assert.Equal(@float, (float)value);
    }

    [Theory]
    [MemberData(nameof(FloatData))]
    public void NullableFloatInFloatOut(float @float)
    {
        float? source = @float;
        Value value = new(source);

        bool success = value.TryGetValue(out float result);
        Assert.True(success);
        Assert.Equal(@float, result);

        Assert.Equal(@float, value.GetValue<float>());

        Assert.Equal(@float, (float)value);
    }

    [Theory]
    [MemberData(nameof(FloatData))]
    public void FloatInNullableFloatOut(float @float)
    {
        float source = @float;
        Value value = new(source);
        bool success = value.TryGetValue(out float? result);
        Assert.True(success);
        Assert.Equal(@float, result);

        Assert.Equal(@float, (float?)value);
    }

    [Theory]
    [MemberData(nameof(FloatData))]
    public void BoxedFloat(float @float)
    {
        float i = @float;
        object o = i;
        Value value = new(o);

        Assert.Equal(typeof(float), value.Type);
        Assert.True(value.TryGetValue(out float result));
        Assert.Equal(@float, result);
        Assert.True(value.TryGetValue(out float? nullableResult));
        Assert.Equal(@float, nullableResult!.Value);

        float? n = @float;
        o = n;
        value = new(o);

        Assert.Equal(typeof(float), value.Type);
        Assert.True(value.TryGetValue(out result));
        Assert.Equal(@float, result);
        Assert.True(value.TryGetValue(out nullableResult));
        Assert.Equal(@float, nullableResult!.Value);
    }

    [Fact]
    public void NullFloat()
    {
        float? source = null;
        Value value = source;
        Assert.Null(value.Type);
        Assert.Equal(source, value.GetValue<float?>());
        Assert.False(value.GetValue<float?>().HasValue);
    }

    [Theory]
    [MemberData(nameof(FloatData))]
    public void OutAsObject(float @float)
    {
        Value value = new(@float);
        object o = value.GetValue<object>();
        Assert.Equal(typeof(float), o.GetType());
        Assert.Equal(@float, (float)o);

        float? n = @float;
        value = new(n);
        o = value.GetValue<object>();
        Assert.Equal(typeof(float), o.GetType());
        Assert.Equal(@float, (float)o);
    }
}
