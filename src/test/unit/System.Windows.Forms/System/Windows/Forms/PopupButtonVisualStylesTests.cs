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

    [WinFormsFact]
    public void PopupButtonKeyCapRenderer_RightToLeft_MirrorsImageAlignment()
    {
        PopupButtonRenderContext context = CreateContext(
            text: string.Empty,
            imageSize: new Size(8, 8),
            imageAlign: ContentAlignment.MiddleLeft,
            rightToLeft: RightToLeft.Yes);
        using Bitmap bitmap = new(context.Bounds.Width, context.Bounds.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);

        PopupButtonKeyCapRenderer.Render(
            graphics,
            context,
            imageBounds =>
            {
                using SolidBrush brush = new(Color.Red);
                graphics.FillRectangle(brush, imageBounds);
            });

        int leftPixels = CountPixels(bitmap, Color.Red, new Rectangle(0, 0, bitmap.Width / 2, bitmap.Height));
        int rightPixels = CountPixels(
            bitmap,
            Color.Red,
            new Rectangle(bitmap.Width / 2, 0, bitmap.Width - (bitmap.Width / 2), bitmap.Height));

        Assert.Equal(0, leftPixels);
        Assert.True(rightPixels > 0);
    }

    [WinFormsTheory]
    [InlineData(0f)]
    [InlineData(0.5f)]
    [InlineData(1f)]
    public void PopupButtonKeyCapRenderer_Press_TranslatesWholeKeyWithoutClipping(float pressProgress)
    {
        PopupButtonRenderContext releasedContext = CreateContext();
        PopupButtonRenderContext pressedContext = CreateContext(
            animationState: new PopupButtonAnimationState(
                hoverProgress: 0,
                pressProgress: pressProgress));

        object releasedMetrics = CreateMetrics(releasedContext);
        object pressedMetrics = CreateMetrics(pressedContext);
        Rectangle releasedKey = GetMetricRectangle(releasedMetrics, "KeyRect");
        Rectangle pressedKey = GetMetricRectangle(pressedMetrics, "KeyRect");
        int pressOffset = GetMetricInt(pressedMetrics, "PressOffset");

        Assert.Equal(releasedKey.Height, pressedKey.Height);
        Assert.Equal(releasedKey.Y + pressOffset, pressedKey.Y);
        Assert.True(pressedKey.Bottom <= pressedContext.Bounds.Bottom - 1);
        Assert.True(pressedKey.Left >= pressedContext.Bounds.Left + 1);
        Assert.True(pressedKey.Right <= pressedContext.Bounds.Right - 1);
    }

    [WinFormsFact]
    public void PopupButtonKeyCapRenderer_DefaultAndFocusCues_DoNotReserveOuterBand()
    {
        Rectangle normalKey = GetMetricRectangle(CreateMetrics(CreateContext()), "KeyRect");
        Rectangle focusedDefaultKey = GetMetricRectangle(
            CreateMetrics(CreateContext(focused: true, isDefault: true)),
            "KeyRect");

        Assert.Equal(normalKey, focusedDefaultKey);
    }

    [WinFormsTheory]
    [InlineData(typeof(Button))]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void PopupButton_PreferredSize_UsesClientForKeyBody(Type controlType)
    {
        using ButtonBase control = (ButtonBase)Activator.CreateInstance(controlType);
        control.FlatStyle = FlatStyle.Popup;
        control.VisualStylesMode = VisualStylesMode.Net11;
        control.Text = "Popup";
        control.Padding = new Padding(2);
        Size preferredSize = control.GetPreferredSize(Size.Empty);
        control.Size = preferredSize;

        PopupButtonRenderContext context = CreateContext(
            bounds: control.ClientRectangle,
            padding: control.Padding);
        Rectangle key = GetMetricRectangle(CreateMetrics(context), "KeyRect");

        Assert.Equal(1, key.Left);
        Assert.Equal(1, key.Top);
        Assert.Equal(control.ClientRectangle.Width - 2, key.Width);
        Assert.True(key.Bottom <= control.ClientRectangle.Bottom - 1);
    }

    [WinFormsTheory]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void PopupAppearanceButton_PreferredSize_DoesNotReserveCheckGlyph(Type controlType)
    {
        using Button button = new()
        {
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlatStyle = FlatStyle.Popup,
            Padding = new Padding(2),
            Text = "Popup",
            VisualStylesMode = VisualStylesMode.Net11
        };
        using ButtonBase checkable = (ButtonBase)Activator.CreateInstance(controlType);
        checkable.FlatStyle = FlatStyle.Popup;
        checkable.Padding = button.Padding;
        checkable.Text = button.Text;
        checkable.VisualStylesMode = button.VisualStylesMode;

        if (checkable is CheckBox checkBox)
        {
            checkBox.Appearance = Appearance.Button;
        }
        else
        {
            ((RadioButton)checkable).Appearance = Appearance.Button;
        }

        Assert.Equal(button.GetPreferredSize(Size.Empty), checkable.GetPreferredSize(Size.Empty));
    }

    private static PopupButtonRenderContext CreateContext(
        int deviceDpi = 96,
        bool focused = false,
        bool isDefault = false,
        bool isDarkMode = false,
        Rectangle? bounds = null,
        Padding padding = default,
        PopupButtonAnimationState animationState = default,
        string text = null,
        Size imageSize = default,
        ContentAlignment imageAlign = ContentAlignment.MiddleCenter,
        RightToLeft rightToLeft = RightToLeft.No)
        => new()
        {
            Bounds = bounds ?? new Rectangle(0, 0, 120, 40),
            Font = Control.DefaultFont,
            BackColor = Color.FromArgb(100, 120, 140),
            ForeColor = Color.White,
            BorderColor = Color.Black,
            BorderWidth = 1,
            Enabled = true,
            Focused = focused,
            IsDefault = isDefault,
            IsDarkMode = isDarkMode,
            DeviceDpi = deviceDpi,
            Padding = padding,
            AnimationState = animationState,
            Text = text,
            ImageSize = imageSize,
            ImageAlign = imageAlign,
            RightToLeft = rightToLeft
        };

    private static object CreateMetrics(PopupButtonRenderContext context)
    {
        Type metricsType = typeof(PopupButtonKeyCapRenderer).GetNestedType(
            "Metrics",
            BindingFlags.NonPublic);
        MethodInfo createMethod = metricsType.GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static);

        return createMethod.Invoke(null, [context]);
    }

    private static Rectangle GetMetricRectangle(object metrics, string name)
        => (Rectangle)metrics.GetType().GetProperty(name).GetValue(metrics);

    private static int GetMetricInt(object metrics, string name)
        => (int)metrics.GetType().GetProperty(name).GetValue(metrics);

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

    private static int CountPixels(Bitmap bitmap, Color color, Rectangle bounds)
    {
        int argb = color.ToArgb();
        int count = 0;

        for (int y = bounds.Top; y < bounds.Bottom; y++)
        {
            for (int x = bounds.Left; x < bounds.Right; x++)
            {
                if (bitmap.GetPixel(x, y).ToArgb() == argb)
                {
                    count++;
                }
            }
        }

        return count;
    }
}
