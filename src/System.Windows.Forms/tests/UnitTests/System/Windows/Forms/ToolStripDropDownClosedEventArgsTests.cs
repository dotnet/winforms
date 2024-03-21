// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class ToolStripDropDownClosedEventArgsTests
{
    [Theory]
    [InlineData(ToolStripDropDownCloseReason.AppClicked)]
    [InlineData((ToolStripDropDownCloseReason.AppFocusChange - 1))]
    public void Ctor_CloseReason(ToolStripDropDownCloseReason closeReason)
    {
        ToolStripDropDownClosedEventArgs e = new(closeReason);
        Assert.Equal(closeReason, e.CloseReason);
    }
}
