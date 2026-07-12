// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Reflection;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Verifies the shared modern Popup button renderer.
/// </summary>
public class PopupButtonVisualStylesTests
{
    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void PopupButtonKeyCapRenderer_DefaultSurface_AdjustsForColorScheme(bool isDarkMode)
    {
        PopupButtonRenderContext context = CreateContext(
            isDefault: true,
            isDarkMode: isDarkMode);

        Color actual = InvokeGetSurfaceBackColor(context);
        Color expected = isDarkMode
            ? PopupButtonColorMath.Lighten(context.BackColor, 0.1f)
            : PopupButtonColorMath.Darken(context.BackColor, 0.1f);

        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void PopupButtonKeyCapRenderer_DefaultBorder_AddsOneLogicalPixel()
    {
        PopupButtonRenderContext normal = CreateContext(deviceDpi: 144);
        PopupButtonRenderContext defaultButton = CreateContext(
            deviceDpi: 144,
            isDefault: true);

        int normalWidth = GetMetricBorderWidth(normal);
        int defaultWidth = GetMetricBorderWidth(defaultButton);

        Assert.Equal(normalWidth + 2, defaultWidth);
    }

    [WinFormsTheory]
    [InlineData(typeof(Button))]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void ButtonBase_AppearanceButton_Popup_UsesSharedRenderer(Type controlType)
    {
        using ButtonBase control = (ButtonBase)Activator.CreateInstance(controlType);
        control.FlatStyle = FlatStyle.Popup;
        control.VisualStylesMode = VisualStylesMode.Net11;
        control.Size = new Size(120, 40);

        if (control is CheckBox checkBox)
        {
            checkBox.Appearance = Appearance.Button;
        }
        else if (control is RadioButton radioButton)
        {
            radioButton.Appearance = Appearance.Button;
        }

        control.CreateControl();
        using Bitmap bitmap = new(control.Width, control.Height);

        control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, control.Size));

        Assert.NotNull(control.TestAccessor.Dynamic._popupKeyCapRenderer);
    }

    [WinFormsFact]
    public void AnimatedPopupButtonRenderer_SelectedState_TargetsPressedPosition()
    {
        using CheckBox checkBox = new();
        using AnimatedPopupButtonRenderer renderer = new(checkBox);

        renderer.SetInteractionState(hovered: false, pressed: false, selected: true);

        Assert.Equal(1f, (float)renderer.TestAccessor.Dynamic._pressTarget);
    }

    [WinFormsFact]
    public void PopupButtonKeyCapRenderer_FocusedDefault_RendersWithoutThrow()
    {
        PopupButtonRenderContext context = CreateContext(
            focused: true,
            isDefault: true);
        using Bitmap bitmap = new(context.Bounds.Width, context.Bounds.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);

        Exception exception = Record.Exception(
            () => PopupButtonKeyCapRenderer.Render(graphics, context));

        Assert.Null(exception);
    }

    private static PopupButtonRenderContext CreateContext(
        int deviceDpi = 96,
        bool focused = false,
        bool isDefault = false,
        bool isDarkMode = false)
        => new()
        {
            Bounds = new Rectangle(0, 0, 120, 40),
            Font = Control.DefaultFont,
            BackColor = Color.FromArgb(100, 120, 140),
            ForeColor = Color.White,
            BorderColor = Color.Black,
            BorderWidth = 1,
            Enabled = true,
            Focused = focused,
            IsDefault = isDefault,
            IsDarkMode = isDarkMode,
            DeviceDpi = deviceDpi
        };

    private static Color InvokeGetSurfaceBackColor(PopupButtonRenderContext context)
    {
        MethodInfo method = typeof(PopupButtonKeyCapRenderer).GetMethod(
            "GetSurfaceBackColor",
            BindingFlags.NonPublic | BindingFlags.Static);

        return (Color)method.Invoke(null, [context]);
    }

    private static int GetMetricBorderWidth(PopupButtonRenderContext context)
    {
        Type metricsType = typeof(PopupButtonKeyCapRenderer).GetNestedType(
            "Metrics",
            BindingFlags.NonPublic);
        MethodInfo createMethod = metricsType.GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static);
        object metrics = createMethod.Invoke(null, [context]);
        PropertyInfo borderWidth = metricsType.GetProperty(nameof(context.BorderWidth));

        return (int)borderWidth.GetValue(metrics);
    }
}
