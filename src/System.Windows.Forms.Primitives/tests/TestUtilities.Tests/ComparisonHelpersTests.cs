// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Tests;

public class ComparisonHelpersTests
{
    [Theory]
    [InlineData(0, 0, 0, true)]
    [InlineData(int.MaxValue, int.MaxValue, int.MinValue, true)]
    [InlineData(int.MinValue, int.MinValue, int.MaxValue, true)]
    public void EqualsInteger(int x, int y, int tolerance, bool expected)
    {
        Assert.Equal(expected, ComparisonHelpers.EqualsInteger(x, y, tolerance));
    }
}
