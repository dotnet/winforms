// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Drawing.Printing;

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
        // Regression: #14420 (explicitly setting White must be distinguishable from the default).
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

    [WinFormsFact]
    public void PrintPreviewControl_PageWithNoImage_DefaultForeColor_RendersWhite()
    {
        // Regression test for #14838: a page with no drawable content (e.g. from an empty
        // PrintDocument), with ForeColor left at its default, must render as white paper, not as
        // a solid black rectangle.
        using PrintPreviewControl control = new()
        {
            Size = new Size(200, 200)
        };

        control.CreateControl();

        PreviewPageInfo[] pageInfo = [new(image: null, physicalSize: new Size(850, 1100))];
        control.TestAccessor.Dynamic._pageInfo = pageInfo;

        using Bitmap bitmap = new(control.Width, control.Height);
        control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, control.Size));

        // The single page fills nearly the whole control, so the center pixel lands well
        // inside the page interior, away from its 1px black border.
        Color centerPixel = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);

        Assert.Equal(Color.White.ToArgb(), centerPixel.ToArgb());
    }

    [WinFormsFact]
    public void PrintPreviewControl_PageWithNoImage_ExplicitForeColor_RendersForeColor()
    {
        // ForeColor has driven the page background since the .NET Framework days; an explicit
        // value must still be honored, not overridden by the white-when-unset default.
        using PrintPreviewControl control = new()
        {
            ForeColor = Color.Red,
            Size = new Size(200, 200)
        };

        control.CreateControl();

        PreviewPageInfo[] pageInfo = [new(image: null, physicalSize: new Size(850, 1100))];
        control.TestAccessor.Dynamic._pageInfo = pageInfo;

        using Bitmap bitmap = new(control.Width, control.Height);
        control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, control.Size));

        Color centerPixel = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);

        Assert.Equal(Color.Red.ToArgb(), centerPixel.ToArgb());
    }

    [WinFormsFact]
    public void PrintPreviewControl_PageWithNoImage_ExplicitForeColorMatchingDefault_RendersForeColor()
    {
        // Assigning ForeColor to the value it already holds must still count as explicit, even
        // though ForeColorChanged doesn't fire for a same-value reassignment.
        using PrintPreviewControl control = new()
        {
            Size = new Size(200, 200)
        };
        control.ForeColor = SystemColors.ControlText;

        control.CreateControl();

        PreviewPageInfo[] pageInfo = [new(image: null, physicalSize: new Size(850, 1100))];
        control.TestAccessor.Dynamic._pageInfo = pageInfo;

        using Bitmap bitmap = new(control.Width, control.Height);
        control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, control.Size));

        Color centerPixel = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);

        Assert.Equal(SystemColors.ControlText.ToArgb(), centerPixel.ToArgb());
        Assert.True(control.TestAccessor.Dynamic.ShouldSerializeForeColor());
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
