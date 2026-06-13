// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Tests;

// Behavioral tests for the modern/conservative button renderers driven by VisualStylesMode. Renderer
// selection itself is internal; these tests exercise the public surface and ensure the owner-drawn paths
// do not throw. The visuals are verified through the WinformsControlsTest exploratory harness.
public class ButtonVisualStylesTests
{
    [WinFormsTheory]
    [InlineData(FlatStyle.Standard)]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    [InlineData(FlatStyle.System)]
    public void Button_VisualStylesMode_Net11_DoesNotThrowOnPaint(FlatStyle flatStyle)
    {
        using Button button = new()
        {
            FlatStyle = flatStyle,
            VisualStylesMode = VisualStylesMode.Net11,
            Text = "Go to",
            Size = new Size(120, 32)
        };

        button.CreateControl();

        using Bitmap bitmap = new(button.Width, button.Height);
        button.DrawToBitmap(bitmap, new Rectangle(Point.Empty, button.Size));

        Assert.Equal(VisualStylesMode.Net11, button.VisualStylesMode);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Standard)]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    public void Button_VisualStylesMode_Net11_WithImageRenders(FlatStyle flatStyle)
    {
        using Bitmap image = new(16, 16);
        using Button button = new()
        {
            FlatStyle = flatStyle,
            VisualStylesMode = VisualStylesMode.Net11,
            Text = "Go to",
            Image = image,
            Size = new Size(120, 32)
        };

        button.CreateControl();

        using Bitmap bitmap = new(button.Width, button.Height);

        // Should not throw with an image in any owner-drawn flat style.
        button.DrawToBitmap(bitmap, new Rectangle(Point.Empty, button.Size));
    }

    [WinFormsFact]
    public void Button_VisualStylesMode_ChangedToNet11_Invalidates()
    {
        using Button button = new() { Text = "Go to", Size = new Size(120, 32) };
        button.CreateControl();

        int invalidatedCount = 0;
        button.Invalidated += (s, e) => invalidatedCount++;

        button.VisualStylesMode = VisualStylesMode.Net11;

        Assert.True(invalidatedCount >= 1);
        Assert.Equal(VisualStylesMode.Net11, button.VisualStylesMode);
    }
}
