// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests.Interop.GdiPlus;

public class ARGBTests
{
    [Theory]
    [InlineData(0x0000_0000, 0x00, 0x00, 0x00, 0x00)]
    [InlineData(0xFFFF_FFFF, 0xFF, 0xFF, 0xFF, 0xFF)]
    [InlineData(0xFF00_0000, 0x00, 0x00, 0x00, 0xFF)]
    [InlineData(0x00AA_0000, 0xAA, 0x00, 0x00, 0x00)]
    [InlineData(0x0000_BB00, 0x00, 0xBB, 0x00, 0x00)]
    [InlineData(0x0000_00CC, 0x00, 0x00, 0xCC, 0x00)]
    public void Construction_Raw(uint value, byte r, byte g, byte b, byte a)
    {
        ARGB fromValue = new(value);
        Assert.Equal(a, fromValue.A);
        Assert.Equal(r, fromValue.R);
        Assert.Equal(g, fromValue.G);
        Assert.Equal(b, fromValue.B);
        ARGB fromBytes = new(a, r, g, b);
        Assert.Equal(value, fromBytes.Value);
    }

    [Theory]
    [MemberData(nameof(Colors))]
    public void ToFromColor(Color color)
    {
        ARGB argb = color;
        Assert.Equal((uint)color.ToArgb(), argb.Value);
        Color backAgain = argb;
        Assert.Equal(color.ToArgb(), backAgain.ToArgb());
    }

    public static TheoryData<Color> Colors =>
        new()
        {
            Color.CornflowerBlue,
            Color.Transparent,
            Color.BurlyWood
        };
}
