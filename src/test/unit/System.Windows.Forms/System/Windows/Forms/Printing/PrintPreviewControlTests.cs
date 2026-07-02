// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Tests;

// NB: doesn't require thread affinity
public class PrintPreviewControlTests
{
    [Fact]
    public void ShowPrintPreviewControl_BackColorIsCorrect()
    {
        PrintPreviewControl control = new();

        int actualBackColorArgb = control.TestAccessor.Dynamic.GetBackColor(false).ToArgb();
        Assert.Equal(SystemColors.AppWorkspace.ToArgb(), actualBackColorArgb);

        control.BackColor = Color.Green;

        actualBackColorArgb = control.TestAccessor.Dynamic.GetBackColor(false).ToArgb();
        Assert.Equal(Color.Green.ToArgb(), actualBackColorArgb);
    }

    [Fact]
    public void PrintPreviewControl_ForeColor_DefaultIsControlText()
    {
        PrintPreviewControl control = new();

        Assert.Equal(SystemColors.ControlText.ToArgb(), control.ForeColor.ToArgb());
    }

    [Fact]
    public void PrintPreviewControl_ForeColorNotSet_ShouldSerializeReturnsFalse()
    {
        PrintPreviewControl control = new();

        // ForeColor not explicitly set: ShouldSerializeForeColor must return false.
        Assert.False(control.TestAccessor.Dynamic.ShouldSerializeForeColor());
    }

    [Fact]
    public void PrintPreviewControl_ForeColorWhiteExplicitlySet_ShouldSerializeReturnsTrue()
    {
        // Regression: #14420 - explicitly setting White must be distinguishable from the default.
        PrintPreviewControl control = new() { ForeColor = Color.White };

        Assert.True(control.TestAccessor.Dynamic.ShouldSerializeForeColor());
    }

    [Fact]
    public void PrintPreviewControl_ForeColorNonDefault_ShouldSerializeReturnsTrue()
    {
        PrintPreviewControl control = new() { ForeColor = Color.Red };

        Assert.True(control.TestAccessor.Dynamic.ShouldSerializeForeColor());
    }

    [Fact]
    public void PrintPreviewControl_ForeColorReset_ShouldSerializeReturnsFalse()
    {
        PrintPreviewControl control = new() { ForeColor = Color.Red };
        control.ResetForeColor();

        Assert.False(control.TestAccessor.Dynamic.ShouldSerializeForeColor());
        Assert.Equal(SystemColors.ControlText.ToArgb(), control.ForeColor.ToArgb());
    }

    [Fact]
    public void ShowPrintPreviewControlHighContrast_BackColorIsCorrect()
    {
        PrintPreviewControl control = new();

        int actualBackColorArgb = control.TestAccessor.Dynamic.GetBackColor(true).ToArgb();

        Assert.Equal(SystemColors.ControlDarkDark.ToArgb(), actualBackColorArgb);
        // Default AppWorkSpace color in HC theme does not allow to follow HC standards.
        Assert.False(SystemColors.AppWorkspace.ToArgb().Equals(actualBackColorArgb));

        control.BackColor = Color.Green;

        actualBackColorArgb = control.TestAccessor.Dynamic.GetBackColor(true).ToArgb();

        Assert.Equal(Color.Green.ToArgb(), actualBackColorArgb);
        Assert.False(SystemColors.AppWorkspace.ToArgb().Equals(actualBackColorArgb));
    }
}
