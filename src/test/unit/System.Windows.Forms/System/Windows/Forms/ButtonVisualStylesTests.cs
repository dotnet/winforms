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

    [WinFormsFact]
    public void Button_VisualStylesMode_ChangedToNet11_PerformsAutoSizeLayout()
    {
        using FlowLayoutPanel parent = new();
        using Button button = new()
        {
            AutoSize = true,
            Text = "Go to"
        };

        parent.Controls.Add(button);
        parent.CreateControl();
        parent.PerformLayout();

        List<string> affectedProperties = [];
        parent.Layout += (_, e) => affectedProperties.Add(e.AffectedProperty);

        button.VisualStylesMode = VisualStylesMode.Net11;

        Assert.Contains(Layout.PropertyNames.VisualStylesMode, affectedProperties);
    }

    [WinFormsFact]
    public void ButtonDarkModeAdapter_ModernFlatStyle_UsesModernFlatRenderer()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Flat,
            VisualStylesMode = VisualStylesMode.Net11
        };

        var adapter = (ButtonInternal.ButtonDarkModeAdapter)button.CreateFlatAdapter();
        object renderer = adapter.TestAccessor.Dynamic._buttonDarkModeRenderer;

        Assert.IsType<ModernFlatButtonRenderer>(renderer);
    }

    [WinFormsFact]
    public void ModernButtonDarkModeRenderer_HighDpi_CorrectsRingAndGapWithoutChangingBodyInset()
    {
        ModernButtonDarkModeRenderer renderer = new() { DeviceDpi = 144 };
        dynamic accessor = renderer.TestAccessor.Dynamic;

        Assert.Equal(2, (int)accessor.FocusRingThickness);
        Assert.Equal(1, (int)accessor.FocusGapThickness);
        Assert.Equal(5, (int)accessor.FocusBodyInset);
    }

    [WinFormsFact]
    public void ModernButtonDarkModeRenderer_LargeBorder_ReservesFocusRingSpace()
    {
        using Button button = new();
        button.FlatAppearance.BorderSize = 4;

        ModernButtonDarkModeRenderer renderer = new()
        {
            DeviceDpi = 96,
            FlatAppearance = button.FlatAppearance
        };
        dynamic accessor = renderer.TestAccessor.Dynamic;

        Assert.Equal(8, (int)accessor.FocusRingThickness);
        Assert.Equal(9, (int)accessor.FocusBodyInset);
    }

    [WinFormsFact]
    public void ModernFlatButtonRenderer_FlatAppearance_OverridesStateAndBorderColors()
    {
        using Button button = new();
        button.FlatAppearance.BorderColor = Color.Lime;
        button.FlatAppearance.MouseOverBackColor = Color.Blue;

        ModernFlatButtonRenderer renderer = new()
        {
            DeviceDpi = 96,
            FlatAppearance = button.FlatAppearance
        };

        Color background = renderer.GetBackgroundColor(
            VisualStyles.PushButtonState.Hot,
            isDefault: false,
            customBaseColor: Color.Red);

        using Bitmap bitmap = new(30, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        renderer.DrawButtonBackground(
            graphics,
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            VisualStyles.PushButtonState.Hot,
            isDefault: false,
            background);

        Assert.Equal(Color.Blue, background);
        Assert.True(ContainsPixel(bitmap, Color.Lime));
    }

    [WinFormsFact]
    public void ButtonBackColorAnimator_EndAnimation_StopsAndSettles()
    {
        using Button button = new();
        using ButtonInternal.ButtonBackColorAnimator animator = new(button);

        animator.AnimateTo(Color.Red);
        animator.AnimateTo(Color.Blue);
        Assert.True(animator.IsRunning);

        animator.EndAnimation();

        Assert.False(animator.IsRunning);
        Assert.Equal(Color.Blue, animator.CurrentColor);
    }

    private static bool ContainsPixel(Bitmap bitmap, Color color)
    {
        int argb = color.ToArgb();
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).ToArgb() == argb)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
