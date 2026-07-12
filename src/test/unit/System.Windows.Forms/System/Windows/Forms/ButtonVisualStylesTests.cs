// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Rendering.Button;

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
            focused: false,
            backColor: background);

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

    [WinFormsTheory]
    [InlineData(VisualStylesMode.Classic)]
    [InlineData(VisualStylesMode.Disabled)]
    public void FlatButtonAppearance_ModernStateColors_ClassicModeReturnsEmpty(
        VisualStylesMode visualStylesMode)
    {
        using Button button = new() { VisualStylesMode = visualStylesMode };

        Assert.Equal(Color.Empty, button.FlatAppearance.MouseDownBackColor);
        Assert.Equal(Color.Empty, button.FlatAppearance.MouseOverBackColor);
    }

    [WinFormsFact]
    public void FlatButtonAppearance_ModernStateColors_UnsetResolvesFromAccentAndRenderedBase()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Flat,
            VisualStylesMode = VisualStylesMode.Net11
        };

        Color accent = Application.GetWindowsAccentColor();
        Color expectedBase = ModernButtonColorMath.GetRenderedBaseColor(button, button.FlatAppearance);

        Assert.Equal(accent, button.FlatAppearance.MouseDownBackColor);
        Assert.Equal(
            PopupButtonColorMath.Blend(
                expectedBase,
                accent,
                ModernButtonColorMath.AccentBlendAmount),
            button.FlatAppearance.MouseOverBackColor);
    }

    [WinFormsFact]
    public void FlatButtonAppearance_ModernStateColors_ExplicitValuesWin()
    {
        using Button button = new() { VisualStylesMode = VisualStylesMode.Net11 };
        Color mouseDown = Color.FromArgb(1, 2, 3);
        Color mouseOver = Color.FromArgb(4, 5, 6);

        button.FlatAppearance.MouseDownBackColor = mouseDown;
        button.FlatAppearance.MouseOverBackColor = mouseOver;

        Assert.Equal(mouseDown, button.FlatAppearance.MouseDownBackColor);
        Assert.Equal(mouseOver, button.FlatAppearance.MouseOverBackColor);
    }

    [WinFormsFact]
    public void FlatButtonAppearance_ModernStateColors_ResetAndShouldSerializeUseBackingFields()
    {
        using Button button = new() { VisualStylesMode = VisualStylesMode.Net11 };
        FlatButtonAppearance appearance = button.FlatAppearance;
        PropertyDescriptor mouseDown = TypeDescriptor.GetProperties(appearance)[nameof(appearance.MouseDownBackColor)]!;
        PropertyDescriptor mouseOver = TypeDescriptor.GetProperties(appearance)[nameof(appearance.MouseOverBackColor)]!;

        Assert.False(mouseDown.ShouldSerializeValue(appearance));
        Assert.False(mouseOver.ShouldSerializeValue(appearance));

        appearance.MouseDownBackColor = Color.Red;
        appearance.MouseOverBackColor = Color.Blue;
        Assert.True(mouseDown.ShouldSerializeValue(appearance));
        Assert.True(mouseOver.ShouldSerializeValue(appearance));

        mouseDown.ResetValue(appearance);
        mouseOver.ResetValue(appearance);

        Assert.Equal(Color.Empty, appearance.MouseDownBackColorCore);
        Assert.Equal(Color.Empty, appearance.MouseOverBackColorCore);
        Assert.False(mouseDown.ShouldSerializeValue(appearance));
        Assert.False(mouseOver.ShouldSerializeValue(appearance));
    }

    [WinFormsFact]
    public void ModernButtonColorMath_ExplicitBackColorIsTheRenderedBase()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Standard,
            VisualStylesMode = VisualStylesMode.Net11,
            BackColor = Color.FromArgb(10, 20, 30)
        };

        Assert.Equal(button.BackColor, ModernButtonColorMath.GetRenderedBaseColor(button, button.FlatAppearance));

        Color accent = Application.GetWindowsAccentColor();
        Assert.Equal(
            PopupButtonColorMath.Blend(
                button.BackColor,
                accent,
                ModernButtonColorMath.AccentBlendAmount),
            button.FlatAppearance.MouseOverBackColor);
    }

    [WinFormsFact]
    public void ModernButtonDarkModeRenderer_CornerRadiusDependsOnFocusAndDefaultState()
    {
        ModernButtonDarkModeRenderer renderer = new() { DeviceDpi = 96 };
        dynamic accessor = renderer.TestAccessor.Dynamic;

        Assert.Equal(8, (int)accessor.GetCornerRadius(focused: false, isDefault: false));
        Assert.Equal(6, (int)accessor.GetCornerRadius(focused: true, isDefault: false));
        Assert.Equal(6, (int)accessor.GetCornerRadius(focused: false, isDefault: true));
    }

    [WinFormsFact]
    public void ModernButtonDarkModeRenderer_ModernStateDefaultsUseAccentAndExplicitValuesWin()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Standard,
            VisualStylesMode = VisualStylesMode.Net11
        };
        ModernButtonDarkModeRenderer renderer = new()
        {
            FlatAppearance = button.FlatAppearance
        };
        Color accent = Application.GetWindowsAccentColor();

        Assert.Equal(accent, renderer.GetBackgroundColor(VisualStyles.PushButtonState.Pressed, false, Color.Empty));
        Assert.Equal(
            button.FlatAppearance.MouseOverBackColor,
            renderer.GetBackgroundColor(VisualStyles.PushButtonState.Hot, false, Color.Empty));

        button.FlatAppearance.MouseDownBackColor = Color.Red;
        button.FlatAppearance.MouseOverBackColor = Color.Blue;
        Assert.Equal(Color.Red, renderer.GetBackgroundColor(VisualStyles.PushButtonState.Pressed, false, Color.Empty));
        Assert.Equal(Color.Blue, renderer.GetBackgroundColor(VisualStyles.PushButtonState.Hot, false, Color.Empty));
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Standard, 96)]
    [InlineData(FlatStyle.Standard, 144)]
    [InlineData(FlatStyle.Standard, 192)]
    [InlineData(FlatStyle.System, 96)]
    [InlineData(FlatStyle.System, 144)]
    [InlineData(FlatStyle.System, 192)]
    public void RoundedButtonRenderers_FocusedBitmap_UsesUniformEdgeWeight(
        FlatStyle flatStyle,
        int deviceDpi)
    {
        using Button button = new()
        {
            FlatStyle = flatStyle
        };
        button.FlatAppearance.BorderColor = Color.Lime;
        button.FlatAppearance.BorderSize = 1;

        ButtonDarkModeRendererBase renderer = flatStyle == FlatStyle.Standard
            ? new ModernButtonDarkModeRenderer()
            : new SystemButtonDarkModeRenderer();
        renderer.DeviceDpi = deviceDpi;
        renderer.FlatAppearance = button.FlatAppearance;

        int scale = Math.Max(1, (int)Math.Round(deviceDpi / 96f));
        int margin = (int)Math.Round(12 * deviceDpi / 96f);
        Rectangle bounds = new(
            margin,
            margin,
            (int)Math.Round(120 * deviceDpi / 96f),
            (int)Math.Round(40 * deviceDpi / 96f));
        Color parentColor = Color.Black;
        Color bodyColor = Color.FromArgb(96, 96, 96);

        using Bitmap bitmap = new(bounds.Right + margin, bounds.Bottom + margin);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(parentColor);
        renderer.RenderButton(
            graphics,
            button,
            bounds,
            flatStyle,
            VisualStyles.PushButtonState.Normal,
            isDefault: true,
            focused: true,
            showFocusCues: true,
            parentBackgroundColor: parentColor,
            backColor: bodyColor,
            paintImage: _ => { },
            paintField: () => { });

        Assert.True(ContainsPixel(bitmap, Color.Lime));

        int bodyOffset = FindBodyOffset(bitmap, bounds, bodyColor, vertical: true);
        int leftBodyOffset = FindBodyOffset(bitmap, bounds, bodyColor, vertical: false);
        Assert.InRange(bodyOffset, 1, 12 * scale);
        Assert.InRange(Math.Abs(bodyOffset - leftBodyOffset), 0, 1);

        int topLuminance = GetLuminance(bitmap.GetPixel(bounds.Left + (bounds.Width / 2), bounds.Top));
        int leftLuminance = GetLuminance(bitmap.GetPixel(bounds.Left, bounds.Top + (bounds.Height / 2)));
        Assert.InRange(Math.Abs(topLuminance - leftLuminance), 0, 4);

        int cornerLuminance = GetLuminance(bitmap.GetPixel(bounds.Left + scale, bounds.Top + scale));
        int parentLuminance = GetLuminance(parentColor);
        Assert.InRange(cornerLuminance, parentLuminance - 3, 255);
    }

    [WinFormsFact]
    public void ButtonBackColorAnimator_InterpolatesReversesAndStops()
    {
        using Button button = new();
        using ButtonInternal.ButtonBackColorAnimator animator = new(button);

        animator.AnimateTo(Color.Red);
        animator.AnimateTo(Color.Blue);
        animator.AnimationProc(0.5f);
        Assert.Equal(Color.FromArgb(255, 128, 0, 127), animator.CurrentColor);

        animator.AnimateTo(Color.Green);
        animator.AnimationProc(0.5f);
        Assert.Equal(Color.FromArgb(255, 64, 64, 64), animator.CurrentColor);

        animator.StopAnimation();
        Assert.False(animator.IsRunning);
        Assert.Equal(Color.FromArgb(255, 64, 64, 64), animator.CurrentColor);
    }

    [WinFormsFact]
    public void ButtonDarkModeAdapter_InteractionPaintStateStartsColorAnimation()
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        using Button button = new()
        {
            FlatStyle = FlatStyle.Standard,
            VisualStylesMode = VisualStylesMode.Net11
        };
        ButtonInternal.ButtonDarkModeAdapter adapter = (ButtonInternal.ButtonDarkModeAdapter)button.CreateFlatAdapter();
        dynamic accessor = adapter.TestAccessor.Dynamic;
        ButtonInternal.ButtonBackColorAnimator animator = button.BackColorAnimator;

        _ = accessor.GetButtonBackColor(VisualStyles.PushButtonState.Normal);
        Assert.False(animator.IsRunning);

        _ = accessor.GetButtonBackColor(VisualStyles.PushButtonState.Hot);
        Assert.True(animator.IsRunning);
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

    private static int FindBodyOffset(
        Bitmap bitmap,
        Rectangle bounds,
        Color bodyColor,
        bool vertical)
    {
        int targetLuminance = GetLuminance(bodyColor);
        int maximumOffset = vertical ? bounds.Height / 2 : bounds.Width / 2;

        for (int offset = 0; offset < maximumOffset; offset++)
        {
            int x = vertical
                ? bounds.Left + (bounds.Width / 2)
                : bounds.Left + offset;
            int y = vertical
                ? bounds.Top + offset
                : bounds.Top + (bounds.Height / 2);
            Color pixel = bitmap.GetPixel(x, y);

            if (Math.Abs(GetLuminance(pixel) - targetLuminance) <= 2)
            {
                return offset;
            }
        }

        return maximumOffset;
    }

    private static int GetLuminance(Color color)
        => ((299 * color.R) + (587 * color.G) + (114 * color.B)) / 1000;
}
