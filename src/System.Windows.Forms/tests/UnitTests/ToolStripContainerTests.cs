// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class ToolStripContainerTests
{
    [WinFormsFact]
    public void ToolStripContainer_Constructor()
    {
        using ToolStripContainer tsc = new();

        Assert.NotNull(tsc);
        Assert.NotNull(tsc.TopToolStripPanel);
        Assert.NotNull(tsc.BottomToolStripPanel);
        Assert.NotNull(tsc.LeftToolStripPanel);
        Assert.NotNull(tsc.RightToolStripPanel);
        Assert.NotNull(tsc.ContentPanel);
        Assert.Equal(DockStyle.Top, tsc.TopToolStripPanel.Dock);
        Assert.Equal(DockStyle.Bottom, tsc.BottomToolStripPanel.Dock);
        Assert.Equal(DockStyle.Left, tsc.LeftToolStripPanel.Dock);
        Assert.Equal(DockStyle.Right, tsc.RightToolStripPanel.Dock);

        Assert.NotNull(tsc.Controls);
        Assert.Equal(5, tsc.Controls.Count);
    }
}
