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

    [WinFormsTheory]
    [InlineData(typeof(Button))]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void ButtonBase_AppearanceButton_Popup_ClassicDoesNotUseSharedRenderer(
        Type controlType)
    {
        using ButtonBase control = (ButtonBase)Activator.CreateInstance(
            controlType);
        control.FlatStyle = FlatStyle.Popup;
        control.VisualStylesMode = VisualStylesMode.Classic;

        if (control is CheckBox checkBox)
        {
            checkBox.Appearance = Appearance.Button;
        }
        else if (control is RadioButton radioButton)
        {
            radioButton.Appearance = Appearance.Button;
        }

        Assert.False(
            (bool)control.TestAccessor.Dynamic.IsPopupKeyCapAppearance);
    }

    [WinFormsFact]
    public void ButtonBase_AppearanceButton_Popup_HighContrastDoesNotUseSharedRenderer()
    {
        using Button button = new HighContrastButton
        {
            FlatStyle = FlatStyle.Popup,
            VisualStylesMode = VisualStylesMode.Net11
        };

        Assert.False(
            (bool)button.TestAccessor.Dynamic.IsPopupKeyCapAppearance);
    }

    [WinFormsFact]
    public void ButtonDarkModeAdapter_ClassicPopup_UsesClassicRenderer()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Popup,
            VisualStylesMode = VisualStylesMode.Classic
        };
        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        object renderer = adapter.TestAccessor.Dynamic._buttonDarkModeRenderer;

        Assert.IsType<PopupButtonDarkModeRenderer>(renderer);
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
    public void AnimatedPopupButtonRenderer_DefaultButton_UsesNeutralStateColors()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Popup,
            VisualStylesMode = VisualStylesMode.Net11
        };
        button.NotifyDefault(true);
        using AnimatedPopupButtonRenderer renderer = new(button);
        ModernButtonDarkModeRenderer neutralRenderer = new()
        {
            DeviceDpi = button.DeviceDpi,
            FlatAppearance = button.FlatAppearance
        };

        (Color baseColor, Color hoverColor, Color pressedColor) = renderer.GetStateColors();

        Assert.Equal(
            neutralRenderer.GetBackgroundColor(VisualStyles.PushButtonState.Normal, isDefault: false),
            baseColor);
        Assert.Equal(
            neutralRenderer.GetBackgroundColor(VisualStyles.PushButtonState.Hot, isDefault: false),
            hoverColor);
        Assert.Equal(
            neutralRenderer.GetBackgroundColor(VisualStyles.PushButtonState.Pressed, isDefault: false),
            pressedColor);
    }

    [WinFormsFact]
    public void AnimatedPopupButtonRenderer_ExplicitStateColorsWin()
    {
        using Button button = new()
        {
            BackColor = Color.Red,
            FlatStyle = FlatStyle.Popup,
            VisualStylesMode = VisualStylesMode.Net11
        };
        button.FlatAppearance.MouseOverBackColor = Color.Green;
        button.FlatAppearance.MouseDownBackColor = Color.Blue;
        using AnimatedPopupButtonRenderer renderer = new(button);

        (Color baseColor, Color hoverColor, Color pressedColor) = renderer.GetStateColors();

        Assert.Equal(Color.Red, baseColor);
        Assert.Equal(Color.Green, hoverColor);
        Assert.Equal(Color.Blue, pressedColor);
    }

    [WinFormsFact]
    public void AnimatedPopupButtonRenderer_CustomBackColor_DerivesNeutralStateColors()
    {
        using Button button = new()
        {
            BackColor = Color.FromArgb(40, 80, 120),
            FlatStyle = FlatStyle.Popup,
            VisualStylesMode = VisualStylesMode.Net11
        };
        using AnimatedPopupButtonRenderer renderer = new(button);

        (Color baseColor, Color hoverColor, Color pressedColor) = renderer.GetStateColors();

        Assert.Equal(button.BackColor, baseColor);
        Assert.Equal(PopupButtonColorMath.TowardsContrast(button.BackColor, 0.05f), hoverColor);
        Assert.Equal(PopupButtonColorMath.TowardsContrast(button.BackColor, 0.12f), pressedColor);
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

    [WinFormsTheory]
    [InlineData(typeof(Button))]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void PopupButton_PreferredSize_FitsSharedContentLayout(Type controlType)
    {
        using Bitmap image = new(16, 16);
        using ButtonBase control = CreateAppearanceButton(controlType);
        control.Image = image;
        control.Padding = new Padding(3, 2, 4, 3);
        control.Text = "Popup button with a complete caption";
        control.TextImageRelation = TextImageRelation.ImageBeforeText;
        Size preferredSize = control.GetPreferredSize(Size.Empty);
        control.Size = preferredSize;

        PopupButtonRenderContext context = CreateContext(
            deviceDpi: control.DeviceDpi,
            bounds: control.ClientRectangle,
            padding: control.Padding,
            text: control.Text);
        Rectangle contentBounds = PopupButtonKeyCapRenderer.GetContentBounds(context);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(control);
        ButtonInternal.ButtonBaseAdapter.LayoutData actual = adapter.GetLayoutData(contentBounds);
        ButtonInternal.ButtonBaseAdapter.LayoutData unconstrained = adapter.GetLayoutData(
            new Rectangle(0, 0, 2_000, 500));

        Assert.True(
            actual.TextBounds.Width >= unconstrained.TextBounds.Width,
            $"Preferred={preferredSize}, content={contentBounds}, client={actual.Client}, field={actual.Field}, actual={actual.TextBounds}, unconstrained={unconstrained.TextBounds}");
        Assert.True(
            actual.TextBounds.Height >= unconstrained.TextBounds.Height,
            $"Preferred={preferredSize}, content={contentBounds}, client={actual.Client}, field={actual.Field}, actual={actual.TextBounds}, unconstrained={unconstrained.TextBounds}");
        Assert.True(contentBounds.Contains(actual.TextBounds));
    }

    [WinFormsTheory]
    [InlineData(96)]
    [InlineData(144)]
    [InlineData(192)]
    public void PopupButton_PreferredSizeChrome_ContainsReleasedAndDefaultGeometry(int deviceDpi)
    {
        Size chrome = PopupButtonKeyCapRenderer.GetPreferredSizeChrome(deviceDpi, borderWidth: 1);
        PopupButtonRenderContext context = CreateContext(
            deviceDpi: deviceDpi,
            isDefault: true,
            bounds: new Rectangle(0, 0, 300, 100));
        Rectangle contentBounds = PopupButtonKeyCapRenderer.GetContentBounds(context);
        Size actualChrome = new(
            context.Bounds.Width - contentBounds.Width,
            context.Bounds.Height - contentBounds.Height);

        Assert.True(chrome.Width >= actualChrome.Width);
        Assert.True(chrome.Height >= actualChrome.Height);
    }

    [WinFormsTheory]
    [InlineData(0, 120, 215)]
    [InlineData(255, 185, 0)]
    [InlineData(240, 240, 240)]
    [InlineData(24, 24, 24)]
    [InlineData(180, 20, 90)]
    public void PopupButtonKeyCapRenderer_AutomaticForeground_MeetsWcagContrast(
        byte red,
        byte green,
        byte blue)
    {
        PopupButtonRenderContext context = CreateContext(
            backColor: Color.FromArgb(red, green, blue),
            useAutomaticForeColor: true);

        AssertAutomaticPaletteContrast(context);
    }

    [WinFormsFact]
    public void PopupButtonKeyCapRenderer_TranslucentAutomaticForeground_UsesCompositedSurface()
    {
        PopupButtonRenderContext context = CreateContext(
            backColor: Color.FromArgb(24, Color.White),
            surfaceColor: Color.Black,
            useAutomaticForeColor: true);

        AssertAutomaticPaletteContrast(context);
    }

    [WinFormsFact]
    public void PopupButtonKeyCapRenderer_DisabledText_IsFlatAndReadable()
    {
        PopupButtonRenderContext context = CreateContext(
            enabled: false,
            textEffect: PopupButtonTextEffect.Raised);

        Assert.Equal(
            PopupButtonTextEffect.Flat,
            PopupButtonKeyCapRenderer.GetTextEffect(context));
        AssertDisabledPaletteContrast(context);
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
        RightToLeft rightToLeft = RightToLeft.No,
        Color? backColor = null,
        Color? surfaceColor = null,
        bool useAutomaticForeColor = false,
        bool enabled = true,
        PopupButtonTextEffect textEffect = PopupButtonTextEffect.Raised)
        => new()
        {
            Bounds = bounds ?? new Rectangle(0, 0, 120, 40),
            Font = Control.DefaultFont,
            BackColor = backColor ?? Color.FromArgb(100, 120, 140),
            ForeColor = Color.White,
            SurfaceColor = surfaceColor ?? SystemColors.Control,
            UseAutomaticForeColor = useAutomaticForeColor,
            BorderColor = Color.Black,
            BorderWidth = 1,
            Enabled = enabled,
            Focused = focused,
            IsDefault = isDefault,
            IsDarkMode = isDarkMode,
            DeviceDpi = deviceDpi,
            Padding = padding,
            AnimationState = animationState,
            Text = text,
            ImageSize = imageSize,
            ImageAlign = imageAlign,
            RightToLeft = rightToLeft,
            TextEffect = textEffect,
            HighContrast = false
        };

    private static void AssertAutomaticPaletteContrast(PopupButtonRenderContext context)
    {
        object metrics = CreateMetrics(context);
        Type paletteType = typeof(PopupButtonKeyCapRenderer).GetNestedType(
            "Palette",
            BindingFlags.NonPublic);
        MethodInfo createMethod = paletteType.GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static);
        object palette = createMethod.Invoke(
            null,
            [context, metrics, InvokeGetSurfaceBackColor(context)]);
        Color text = (Color)paletteType.GetProperty("Text").GetValue(palette);
        Color textOutline = (Color)paletteType.GetProperty("TextOutline").GetValue(palette);
        Color darkestBowl = (Color)paletteType.GetProperty("DarkestTextBackground").GetValue(palette);
        Color lightestBowl = (Color)paletteType.GetProperty("LightestTextBackground").GetValue(palette);

        AssertSurfaceContrast(darkestBowl);
        AssertSurfaceContrast(lightestBowl);

        void AssertSurfaceContrast(Color surface)
        {
            float textContrast = PopupButtonColorMath.GetContrastRatio(text, surface);
            float outlineContrast = textOutline.IsEmpty
                ? 0f
                : PopupButtonColorMath.GetContrastRatio(textOutline, surface);

            Assert.True(
                Math.Max(textContrast, outlineContrast)
                    >= ModernButtonColorMath.MinimumTextContrastRatio);

            if (!textOutline.IsEmpty)
            {
                Assert.True(
                    PopupButtonColorMath.GetContrastRatio(text, textOutline)
                        >= ModernButtonColorMath.MinimumTextContrastRatio);
            }
        }
    }

    private static void AssertDisabledPaletteContrast(
        PopupButtonRenderContext context)
    {
        object metrics = CreateMetrics(context);
        Type paletteType = typeof(PopupButtonKeyCapRenderer).GetNestedType(
            "Palette",
            BindingFlags.NonPublic);
        MethodInfo createMethod = paletteType.GetMethod(
            "Create",
            BindingFlags.Public | BindingFlags.Static);
        object palette = createMethod.Invoke(
            null,
            [context, metrics, InvokeGetSurfaceBackColor(context)]);
        Color text = (Color)paletteType.GetProperty("Text").GetValue(palette);
        Color darkestBowl = (Color)paletteType.GetProperty("DarkestTextBackground").GetValue(palette);
        Color lightestBowl = (Color)paletteType.GetProperty("LightestTextBackground").GetValue(palette);

        Assert.True(
            PopupButtonColorMath.GetContrastRatio(text, darkestBowl)
                >= ModernControlColorMath.MinimumDisabledTextContrastRatio);
        Assert.True(
            PopupButtonColorMath.GetContrastRatio(text, lightestBowl)
                >= ModernControlColorMath.MinimumDisabledTextContrastRatio);
    }

    private static ButtonBase CreateAppearanceButton(Type controlType)
    {
        ButtonBase control = (ButtonBase)Activator.CreateInstance(controlType);
        control.AutoSize = true;
        control.FlatStyle = FlatStyle.Popup;
        control.VisualStylesMode = VisualStylesMode.Net11;

        if (control is Button button)
        {
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
        else if (control is CheckBox checkBox)
        {
            checkBox.Appearance = Appearance.Button;
        }
        else
        {
            ((RadioButton)control).Appearance = Appearance.Button;
        }

        return control;
    }

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

    /// <summary>
    ///  Provides a deterministic High Contrast effective mode for renderer-selection tests.
    /// </summary>
    private sealed class HighContrastButton : Button
    {
        internal override bool IsHighContrast => true;
    }
}
