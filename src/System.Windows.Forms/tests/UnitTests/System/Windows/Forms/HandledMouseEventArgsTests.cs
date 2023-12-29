// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class HandledMouseEventArgsTests
{
    [Theory]
    [InlineData(MouseButtons.Left, 1, 2, 3, 4)]
    [InlineData((MouseButtons)1, 0, 0, 0, 0)]
    [InlineData((MouseButtons)3, -1, -1, -1, -2)]
    public void Ctor_MouseButtons_Int_Int_Int_Int(MouseButtons button, int clicks, int x, int y, int delta)
    {
        HandledMouseEventArgs e = new(button, clicks, x, y, delta);
        Assert.Equal(button, e.Button);
        Assert.Equal(clicks, e.Clicks);
        Assert.Equal(x, e.X);
        Assert.Equal(y, e.Y);
        Assert.Equal(delta, e.Delta);
        Assert.Equal(new Point(x, y), e.Location);
        Assert.False(e.Handled);
    }

    [Theory]
    [InlineData(MouseButtons.Left, 1, 2, 3, 4, true)]
    [InlineData((MouseButtons)1, 0, 0, 0, 0, true)]
    [InlineData((MouseButtons)3, -1, -1, -1, -2, false)]
    public void Ctor_MouseButtons_Int_Int_Int_Int_Bool(MouseButtons button, int clicks, int x, int y, int delta, bool handled)
    {
        HandledMouseEventArgs e = new(button, clicks, x, y, delta, handled);
        Assert.Equal(button, e.Button);
        Assert.Equal(clicks, e.Clicks);
        Assert.Equal(x, e.X);
        Assert.Equal(y, e.Y);
        Assert.Equal(delta, e.Delta);
        Assert.Equal(new Point(x, y), e.Location);
        Assert.Equal(handled, e.Handled);
    }
}
