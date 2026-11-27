// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class SplitContainerTests
{
    [WinFormsTheory]
    [InlineData(Orientation.Horizontal)]
    [InlineData(Orientation.Vertical)]
    public void SplitContainer_SplitterDistance_MaxValue_ClampsToAvailableSpace(Orientation orientation)
    {
        using SplitContainer control = new() { Size = new Size(320, 240), Orientation = orientation };
        bool vertical = orientation == Orientation.Vertical;
        int extent = vertical ? control.Width : control.Height;
        int maximum = extent - control.Panel2MinSize - control.SplitterWidth;

        control.SplitterDistance = int.MaxValue;

        Assert.Equal(maximum, control.SplitterDistance);
        Assert.Equal(maximum, vertical ? control.Panel1.Width : control.Panel1.Height);
        Assert.Equal(control.Panel2MinSize, vertical ? control.Panel2.Width : control.Panel2.Height);
        Assert.False(control.IsHandleCreated);
    }
}
