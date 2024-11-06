// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging.Tests;

public class ColorMapTests
{
    [Fact]
    public void Ctor_Default()
    {
        ColorMap cm = new();
        Assert.Equal(default, cm.OldColor);
        Assert.Equal(default, cm.NewColor);
    }

    [Fact]
    public void NewColor_SetValid_ReturnsExpected()
    {
        ColorMap cm = new()
        {
            NewColor = Color.AliceBlue
        };
        Assert.Equal(Color.AliceBlue, cm.NewColor);
    }

    [Fact]
    public void OldColor_SetValid_ReturnsExpected()
    {
        ColorMap cm = new()
        {
            OldColor = Color.AliceBlue
        };
        Assert.Equal(Color.AliceBlue, cm.OldColor);
    }
}
