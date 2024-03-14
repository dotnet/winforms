// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging.Tests;

public class EncoderParametersTests
{
    [Fact]
    public void Ctor_Default()
    {
        using EncoderParameters ep = new();
        Assert.NotNull(ep.Param);
        Assert.Equal(new EncoderParameter[1], ep.Param);
    }

    [Theory]
    [InlineData(1)]
    public void Ctor_Count_Default(int count)
    {
        using EncoderParameters ep = new(count);
        Assert.NotNull(ep.Param);
        Assert.Equal(new EncoderParameter[count], ep.Param);
    }

    public static IEnumerable<object[]> Param_TestData
    {
        get
        {
            yield return new object[] { new EncoderParameter[1] };
            yield return new object[] { new EncoderParameter[1] { new(Encoder.ChrominanceTable, 0) } };
            yield return new object[] { new EncoderParameter[1] { null } };
        }
    }

    [Theory]
    [MemberData(nameof(Param_TestData))]
    public void Param_Success(EncoderParameter[] param)
    {
        using EncoderParameters ep = new();
        ep.Param = param;
        Assert.Equal(param, ep.Param);
    }

    [Theory]
    [MemberData(nameof(Param_TestData))]
    public void Dispose_Success(EncoderParameter[] param)
    {
        EncoderParameters ep = new()
        {
            Param = param
        };
        ep.Dispose();
        Assert.Null(ep.Param);
    }
}
