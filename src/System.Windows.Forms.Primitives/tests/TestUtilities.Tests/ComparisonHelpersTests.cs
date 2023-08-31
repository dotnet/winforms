// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Tests;

public class ComparisonHelpersTests
{
    [Theory]
    [InlineData(0, 0, 0, true)]
    [InlineData(1, -1, 2, true)]
    [InlineData(1, -1, 1, false)]
    [InlineData(-1, 1, 2, true)]
    [InlineData(-1, 1, 1, false)]
    [InlineData(-1, -1, 0, true)]
    [InlineData(-1, -2, 1, true)]
    [InlineData(-2, -1, 1, true)]
    [InlineData(int.MinValue, int.MinValue, int.MaxValue, true)]
    public void EqualsInteger(int x, int y, int tolerance, bool expected)
    {
        Assert.Equal(expected, ComparisonHelpers.EqualsInteger(x, y, tolerance));
    }

    [Theory]
    [InlineData(0, 0, 0, true)]
    [InlineData(1, -1, 2, true)]
    [InlineData(-1, 1, 2, true)]
    [InlineData(-1, -1, 0, true)]
    [InlineData(-1, -2, 1, true)]
    [InlineData(-2, -1, 1, true)]
    [InlineData(1.00000f, 1.00001f, 0.00002f, true)]
    [InlineData(1.00000f, 1.00001f, 0.000001f, false)]
    [InlineData(int.MinValue, int.MinValue, int.MaxValue, true)]
    public void EqualsFloat(float x, float y, float tolerance, bool expected)
    {
        Assert.Equal(expected, ComparisonHelpers.EqualsFloating(x, y, tolerance));
    }
}
