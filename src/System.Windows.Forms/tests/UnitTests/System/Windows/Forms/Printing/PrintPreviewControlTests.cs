// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PrintPreviewControlTests
{
    [Fact]
    public void ShowPrintPreviewControl_BackColorIsCorrect()
    {
        PrintPreviewControl control = new();

        int actualBackColorArgb = control.TestAccessor().Dynamic.GetBackColor(false).ToArgb();
        Assert.Equal(SystemColors.AppWorkspace.ToArgb(), actualBackColorArgb);

        control.BackColor = Color.Green;

        actualBackColorArgb = control.TestAccessor().Dynamic.GetBackColor(false).ToArgb();
        Assert.Equal(Color.Green.ToArgb(), actualBackColorArgb);
    }

    [Fact]
    public void ShowPrintPreviewControlHighContrast_BackColorIsCorrect()
    {
        PrintPreviewControl control = new();

        int actualBackColorArgb = control.TestAccessor().Dynamic.GetBackColor(true).ToArgb();

        Assert.Equal(SystemColors.ControlDarkDark.ToArgb(), actualBackColorArgb);
        // Default AppWorkSpace color in HC theme does not allow to follow HC standards.
        Assert.False(SystemColors.AppWorkspace.ToArgb().Equals(actualBackColorArgb));

        control.BackColor = Color.Green;

        actualBackColorArgb = control.TestAccessor().Dynamic.GetBackColor(true).ToArgb();

        Assert.Equal(Color.Green.ToArgb(), actualBackColorArgb);
        Assert.False(SystemColors.AppWorkspace.ToArgb().Equals(actualBackColorArgb));
    }
}
