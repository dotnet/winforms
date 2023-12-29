// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class HelpEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Point_TestData()
    {
        yield return new object[] { Point.Empty };
        yield return new object[] { new Point(1, 2) };
        yield return new object[] { new Point(-1, -2) };
    }

    [Theory]
    [MemberData(nameof(Ctor_Point_TestData))]
    public void Ctor_Point(Point mousePos)
    {
        HelpEventArgs e = new(mousePos);
        Assert.Equal(mousePos, e.MousePos);
        Assert.False(e.Handled);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Handled_Set_GetReturnsExpected(bool value)
    {
        HelpEventArgs e = new(new Point(1, 2))
        {
            Handled = value
        };
        Assert.Equal(value, e.Handled);
    }
}
