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

    [WinFormsTheory]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void ModernGlyphControl_PaintsParentBackgroundImage(Type controlType)
    {
        using Bitmap backgroundImage = CreateSolidBitmap(new Size(2, 1), Color.Red);
        backgroundImage.SetPixel(1, 0, Color.Blue);
        using Panel parent = new()
        {
            BackgroundImage = backgroundImage,
            BackgroundImageLayout = ImageLayout.Tile,
            Size = new Size(40, 30)
        };
        using ButtonBase control = (ButtonBase)Activator.CreateInstance(controlType);
        control.Size = parent.Size;
        control.Text = string.Empty;
        control.VisualStylesMode = VisualStylesMode.Net11;
        parent.Controls.Add(control);
        parent.CreateControl();
        control.CreateControl();
        using Bitmap actual = new(control.Width, control.Height);

        control.DrawToBitmap(actual, new Rectangle(Point.Empty, control.Size));

        Assert.Equal(Color.Red.ToArgb(), actual.GetPixel(control.Width - 2, 2).ToArgb());
        Assert.Equal(Color.Blue.ToArgb(), actual.GetPixel(control.Width - 1, 2).ToArgb());
    }

    [WinFormsTheory]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void ModernGlyphControl_ThemedTabPage_PaintsOpaqueBackground(Type controlType)
    {
        if (!Application.RenderWithVisualStyles)
        {
            return;
        }

        using TabControl tabControl = new() { Size = new Size(120, 80) };
        using TabPage tabPage = new()
        {
            Size = tabControl.Size,
            UseVisualStyleBackColor = true
        };
        using ButtonBase control = (ButtonBase)Activator.CreateInstance(controlType);
        control.Size = new Size(40, 30);
        control.Text = string.Empty;
        control.VisualStylesMode = VisualStylesMode.Net11;
        tabPage.Controls.Add(control);
        tabControl.TabPages.Add(tabPage);
        tabControl.CreateControl();
        tabPage.CreateControl();
        control.CreateControl();
        using Bitmap actual = new(control.Width, control.Height);

        control.DrawToBitmap(actual, new Rectangle(Point.Empty, control.Size));

        Assert.Equal(255, actual.GetPixel(control.Width - 2, 2).A);
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
    public void ButtonDarkModeAdapter_ClassicStandard_UsesFlatRenderer()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Standard,
            VisualStylesMode = VisualStylesMode.Classic
        };

        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        object renderer = adapter.TestAccessor.Dynamic._buttonDarkModeRenderer;

        Assert.IsType<FlatButtonDarkModeRenderer>(renderer);
    }

    [WinFormsTheory]
    [InlineData(245, 245, 245, 0, 0, 0)]
    [InlineData(32, 32, 32, 240, 240, 240)]
    [InlineData(0, 120, 215, 255, 255, 255)]
    [InlineData(255, 185, 0, 0, 0, 0)]
    public void ModernControlColorMath_DisabledText_MeetsMinimumContrast(
        byte backRed,
        byte backGreen,
        byte backBlue,
        byte foreRed,
        byte foreGreen,
        byte foreBlue)
    {
        Color backColor = Color.FromArgb(backRed, backGreen, backBlue);
        Color preferredForeColor = Color.FromArgb(foreRed, foreGreen, foreBlue);

        Color actual = ModernControlColorMath.GetDisabledTextColor(
            preferredForeColor,
            backColor);

        Assert.True(
            PopupButtonColorMath.GetContrastRatio(actual, backColor)
                >= ModernControlColorMath.MinimumDisabledTextContrastRatio);
    }

    [WinFormsTheory]
    [InlineData(typeof(CheckBox))]
    [InlineData(typeof(RadioButton))]
    public void ModernGlyphControl_DisabledText_IsNotShadowed(Type controlType)
    {
        using ButtonBase control = (ButtonBase)Activator.CreateInstance(controlType);
        control.Enabled = false;
        control.FlatStyle = FlatStyle.Popup;
        control.VisualStylesMode = VisualStylesMode.Net11;

        ButtonInternal.ButtonBaseAdapter adapter = control.CreatePopupAdapter();

        Assert.False(adapter.CommonLayout().ShadowedText);
    }

    [WinFormsFact]
    public void ToggleSwitch_DisabledText_UsesControlBackground()
    {
        using Panel parent = new() { BackColor = Color.White };
        using CheckBox checkBox = new()
        {
            Appearance = Appearance.ToggleSwitch,
            BackColor = Color.FromArgb(20, 40, 80),
            Enabled = false,
            ForeColor = Color.Black,
            VisualStylesMode = VisualStylesMode.Net11
        };
        parent.Controls.Add(checkBox);
        using Rendering.CheckBox.AnimatedToggleSwitchRenderer renderer = new(
            checkBox,
            Rendering.CheckBox.ModernCheckBoxStyle.Rounded);

        Color actual = renderer.GetTextColor();

        Assert.True(
            PopupButtonColorMath.GetContrastRatio(actual, checkBox.BackColor)
                >= ModernControlColorMath.MinimumDisabledTextContrastRatio);
    }

    [WinFormsFact]
    public void ButtonDarkModeAdapter_ModernDisabledExplicitForeColor_MeetsMinimumContrast()
    {
        using Button button = new()
        {
            BackColor = Color.Black,
            Enabled = false,
            FlatStyle = FlatStyle.Standard,
            ForeColor = Color.White,
            VisualStylesMode = VisualStylesMode.Net11
        };
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        dynamic accessor = adapter.TestAccessor.Dynamic;

        Color actual = (Color)accessor.GetButtonTextColor(
            graphics,
            VisualStyles.PushButtonState.Disabled,
            Color.Black);

        Assert.True(
            PopupButtonColorMath.GetContrastRatio(actual, Color.Black)
                >= ModernControlColorMath.MinimumDisabledTextContrastRatio);
    }

    [WinFormsFact]
    public void PopupButtonDarkModeRenderer_PaintsParentBackgroundAtRoundedCorners()
    {
        using Bitmap backgroundImage = CreateSolidBitmap(
            new Size(2, 1),
            Color.Red);
        backgroundImage.SetPixel(1, 0, Color.Blue);
        using Panel parent = new()
        {
            BackgroundImage = backgroundImage,
            BackgroundImageLayout = ImageLayout.Tile,
            Size = new Size(40, 30)
        };
        using Button button = new() { Size = parent.Size };
        parent.Controls.Add(button);
        parent.CreateControl();
        button.CreateControl();
        using Bitmap actual = new(button.Width, button.Height);
        using Graphics graphics = Graphics.FromImage(actual);
        PopupButtonDarkModeRenderer renderer = new();

        renderer.RenderButton(
            graphics,
            button,
            button.ClientRectangle,
            FlatStyle.Popup,
            VisualStyles.PushButtonState.Normal,
            isDefault: false,
            focused: false,
            showFocusCues: false,
            parent.BackColor,
            Color.FromArgb(0x33, 0x33, 0x33),
            _ => { });

        Assert.Equal(
            Color.Red.ToArgb(),
            actual.GetPixel(0, 0).ToArgb());
        Assert.Equal(
            Color.Blue.ToArgb(),
            actual.GetPixel(1, 0).ToArgb());
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
        using SystemVisualSettingsTestScope settingsScope = new(clientAreaAnimationEnabled: true);
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

        Color accent = Application.SystemVisualSettings.AccentColor;
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
    public void FlatButtonAppearance_ModernStateColors_PopupReturnsOnlyExplicitValues()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Popup,
            VisualStylesMode = VisualStylesMode.Net11
        };

        Assert.Equal(Color.Empty, button.FlatAppearance.MouseDownBackColor);
        Assert.Equal(Color.Empty, button.FlatAppearance.MouseOverBackColor);

        button.FlatAppearance.MouseDownBackColor = Color.Red;
        button.FlatAppearance.MouseOverBackColor = Color.Blue;

        Assert.Equal(Color.Red, button.FlatAppearance.MouseDownBackColor);
        Assert.Equal(Color.Blue, button.FlatAppearance.MouseOverBackColor);
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

        Color accent = Application.SystemVisualSettings.AccentColor;
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
        Color accent = Application.SystemVisualSettings.AccentColor;

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
            paintContent: _ => { });

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
        using SystemVisualSettingsTestScope settingsScope = new(clientAreaAnimationEnabled: true);
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
        using SystemVisualSettingsTestScope settingsScope = new(clientAreaAnimationEnabled: true);

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

    public static TheoryData<Type, FlatStyle, ContentAlignment, TextImageRelation> ModernImageLayoutData
    {
        get
        {
            Type[] controlTypes = [typeof(Button), typeof(CheckBox), typeof(RadioButton)];
            FlatStyle[] flatStyles = [FlatStyle.Standard, FlatStyle.Flat, FlatStyle.Popup];
            (ContentAlignment Alignment, TextImageRelation Relation)[] layouts =
            [
                (ContentAlignment.TopLeft, TextImageRelation.Overlay),
                (ContentAlignment.MiddleLeft, TextImageRelation.ImageBeforeText),
                (ContentAlignment.MiddleRight, TextImageRelation.TextBeforeImage),
                (ContentAlignment.TopCenter, TextImageRelation.ImageAboveText),
                (ContentAlignment.BottomCenter, TextImageRelation.TextAboveImage)
            ];
            TheoryData<Type, FlatStyle, ContentAlignment, TextImageRelation> data = new();

            foreach (Type controlType in controlTypes)
            {
                foreach (FlatStyle flatStyle in flatStyles)
                {
                    foreach ((ContentAlignment alignment, TextImageRelation relation) in layouts)
                    {
                        data.Add(controlType, flatStyle, alignment, relation);
                    }
                }
            }

            return data;
        }
    }

    public static TheoryData<Type, FlatStyle> ModernButtonTypeAndStyleData
    {
        get
        {
            TheoryData<Type, FlatStyle> data = new();

            foreach (Type controlType in new[] { typeof(Button), typeof(CheckBox), typeof(RadioButton) })
            {
                foreach (FlatStyle flatStyle in new[] { FlatStyle.Standard, FlatStyle.Flat, FlatStyle.Popup })
                {
                    data.Add(controlType, flatStyle);
                }
            }

            return data;
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ModernImageLayoutData))]
    public void ButtonBase_ModernImageLayout_MatchesRendererContentBounds(
        Type controlType,
        FlatStyle flatStyle,
        ContentAlignment imageAlign,
        TextImageRelation textImageRelation)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap image = CreateSolidBitmap(new Size(9, 7), Color.Fuchsia);
        using ButtonBase control = CreateAppearanceButton(controlType, flatStyle);
        control.Image = image;
        control.ImageAlign = imageAlign;
        control.Size = new Size(140, 52);
        control.Text = " ";
        control.TextImageRelation = textImageRelation;
        control.CreateControl();

        Rectangle contentBounds = GetModernContentBounds(control, flatStyle);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(control);
        ButtonInternal.ButtonBaseAdapter.LayoutData expected = adapter.GetLayoutData(contentBounds);
        using Bitmap actual = new(control.Width, control.Height);

        control.DrawToBitmap(actual, new Rectangle(Point.Empty, control.Size));

        Rectangle actualImageBounds = FindColorBounds(actual, Color.Fuchsia);
        Assert.InRange(actualImageBounds.X - expected.ImageBounds.X, 0, 1);
        Assert.InRange(actualImageBounds.Y - expected.ImageBounds.Y, 0, 1);
        Assert.InRange(image.Width - actualImageBounds.Width, 0, 1);
        Assert.InRange(image.Height - actualImageBounds.Height, 0, 1);
        Assert.True(contentBounds.Contains(actualImageBounds));
    }

    [WinFormsTheory]
    [MemberData(nameof(ModernButtonTypeAndStyleData))]
    public void ButtonBase_ModernBackgroundImage_RendersWithoutForegroundImage(
        Type controlType,
        FlatStyle flatStyle)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap backgroundImage = CreateSolidBitmap(
            new Size(9, 7),
            Color.Fuchsia);
        using ButtonBase control = (ButtonBase)Activator.CreateInstance(
            controlType);
        control.BackgroundImage = backgroundImage;
        control.BackgroundImageLayout = ImageLayout.Center;
        control.FlatStyle = flatStyle;
        control.Size = new Size(120, 44);
        control.Text = string.Empty;
        control.VisualStylesMode = VisualStylesMode.Net11;
        control.CreateControl();
        using Bitmap actual = new(control.Width, control.Height);

        control.DrawToBitmap(
            actual,
            new Rectangle(Point.Empty, control.Size));

        Assert.False(FindColorBounds(actual, Color.Fuchsia).IsEmpty);
    }

    [WinFormsTheory]
    [InlineData(ImageLayout.None)]
    [InlineData(ImageLayout.Tile)]
    [InlineData(ImageLayout.Center)]
    [InlineData(ImageLayout.Stretch)]
    [InlineData(ImageLayout.Zoom)]
    public void Button_ModernBackgroundImageLayout_PositionsImage(
        ImageLayout imageLayout)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap backgroundImage = CreateSolidBitmap(
            new Size(8, 8),
            Color.Fuchsia);
        using Button button = new()
        {
            BackgroundImage = backgroundImage,
            BackgroundImageLayout = imageLayout,
            FlatStyle = FlatStyle.Standard,
            Size = new Size(80, 40),
            VisualStylesMode = VisualStylesMode.Net11
        };
        button.CreateControl();
        using Bitmap actual = new(button.Width, button.Height);
        using Bitmap withoutBackgroundImage = new(
            button.Width,
            button.Height);

        button.DrawToBitmap(
            actual,
            new Rectangle(Point.Empty, button.Size));
        button.BackgroundImage = null;
        button.DrawToBitmap(
            withoutBackgroundImage,
            new Rectangle(Point.Empty, button.Size));

        Rectangle clipRectangle = Rectangle.Inflate(
            button.ClientRectangle,
            -4,
            -4);
        Rectangle expected = GetExpectedBackgroundImageBounds(
            button.ClientRectangle,
            clipRectangle,
            backgroundImage.Size,
            imageLayout);
        Rectangle actualImageBounds = FindDifferenceBounds(
            actual,
            withoutBackgroundImage);
        Assert.InRange(
            Math.Abs(expected.X - actualImageBounds.X),
            0,
            1);
        Assert.InRange(
            Math.Abs(expected.Y - actualImageBounds.Y),
            0,
            1);
        Assert.InRange(
            Math.Abs(expected.Width - actualImageBounds.Width),
            0,
            1);
        Assert.InRange(
            Math.Abs(expected.Height - actualImageBounds.Height),
            0,
            1);
    }

    [WinFormsFact]
    public void Button_ModernBackgroundImageNone_RightToLeftAlignsRight()
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap backgroundImage = CreateSolidBitmap(
            new Size(8, 8),
            Color.Fuchsia);
        using Button button = new()
        {
            BackgroundImage = backgroundImage,
            BackgroundImageLayout = ImageLayout.None,
            FlatStyle = FlatStyle.Standard,
            RightToLeft = RightToLeft.Yes,
            Size = new Size(80, 40),
            VisualStylesMode = VisualStylesMode.Net11
        };
        button.CreateControl();
        using Bitmap actual = new(button.Width, button.Height);
        using Bitmap withoutBackgroundImage = new(
            button.Width,
            button.Height);

        button.DrawToBitmap(
            actual,
            new Rectangle(Point.Empty, button.Size));
        button.BackgroundImage = null;
        button.DrawToBitmap(
            withoutBackgroundImage,
            new Rectangle(Point.Empty, button.Size));

        Rectangle actualImageBounds = FindDifferenceBounds(
            actual,
            withoutBackgroundImage);
        Rectangle clipRectangle = Rectangle.Inflate(
            button.ClientRectangle,
            -4,
            -4);
        Assert.InRange(
            Math.Abs(actualImageBounds.Right - clipRectangle.Right),
            0,
            1);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Standard)]
    [InlineData(FlatStyle.Flat)]
    [InlineData(FlatStyle.Popup)]
    public void Button_ModernDisabledImage_RendersWithinContentBounds(FlatStyle flatStyle)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap image = CreateSolidBitmap(new Size(9, 7), Color.Black);
        using Button button = new()
        {
            Enabled = false,
            FlatStyle = flatStyle,
            Image = image,
            ImageAlign = ContentAlignment.TopLeft,
            Size = new Size(120, 44),
            VisualStylesMode = VisualStylesMode.Net11
        };
        button.CreateControl();

        Rectangle contentBounds = GetModernContentBounds(button, flatStyle);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        Rectangle imageBounds = adapter.GetLayoutData(contentBounds).ImageBounds;
        using Bitmap withImage = new(button.Width, button.Height);
        using Bitmap withoutImage = new(button.Width, button.Height);

        button.DrawToBitmap(withImage, new Rectangle(Point.Empty, button.Size));
        button.Image = null;
        button.DrawToBitmap(withoutImage, new Rectangle(Point.Empty, button.Size));

        Assert.True(CountDifferentPixels(withImage, withoutImage, imageBounds) > 0);
        Assert.True(contentBounds.Contains(imageBounds));
    }

    [WinFormsTheory]
    [MemberData(nameof(ModernButtonTypeAndStyleData))]
    public void ButtonBase_ModernPreferredSize_FitsRendererContent(
        Type controlType,
        FlatStyle flatStyle)
    {
        using ButtonBase control = CreateAppearanceButton(controlType, flatStyle);
        control.AutoSize = true;
        control.Padding = new Padding(3, 2, 4, 3);
        control.Text = "Modern button with a complete caption";
        if (control is Button button)
        {
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        Size preferredSize = control.GetPreferredSize(Size.Empty);
        control.Size = preferredSize;
        Rectangle contentBounds = GetModernContentBounds(control, flatStyle);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(control);
        ButtonInternal.ButtonBaseAdapter.LayoutData actual = adapter.GetLayoutData(contentBounds);
        ButtonInternal.ButtonBaseAdapter.LayoutData unconstrained = adapter.GetLayoutData(
            new Rectangle(0, 0, 2_000, 500));

        Assert.True(actual.TextBounds.Width >= unconstrained.TextBounds.Width);
        Assert.True(actual.TextBounds.Height >= unconstrained.TextBounds.Height);
        Assert.True(contentBounds.Contains(actual.TextBounds));
    }

    [WinFormsFact]
    public void Button_StandardPreferredSize_UsesFocusedContentConstraint()
    {
        using Button button = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlatStyle = FlatStyle.Standard,
            Padding = new Padding(3, 2, 4, 3),
            Text = "Modern Standard button with a complete caption",
            VisualStylesMode = VisualStylesMode.Net11
        };

        Size preferredSize = button.GetPreferredSize(Size.Empty);
        button.Size = preferredSize;
        Rectangle unfocusedContent = GetModernStandardContentBounds(button, focused: false);
        Rectangle focusedContent = GetModernStandardContentBounds(button, focused: true);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        ButtonInternal.ButtonBaseAdapter.LayoutData focusedLayout = adapter.GetLayoutData(focusedContent);
        ButtonInternal.ButtonBaseAdapter.LayoutData unconstrained = adapter.GetLayoutData(
            new Rectangle(0, 0, 2_000, 500));
        ModernButtonDarkModeRenderer renderer = new()
        {
            DeviceDpi = button.DeviceDpi,
            FlatAppearance = button.FlatAppearance
        };

        Assert.True(focusedContent.Width < unfocusedContent.Width);
        Assert.True(focusedContent.Height < unfocusedContent.Height);
        Assert.Equal(
            preferredSize.Width - renderer.GetPreferredSizePadding().Horizontal,
            focusedContent.Width);
        Assert.Equal(
            preferredSize.Height - renderer.GetPreferredSizePadding().Vertical,
            focusedContent.Height);
        Assert.True(focusedLayout.TextBounds.Width >= unconstrained.TextBounds.Width);
        Assert.True(focusedLayout.TextBounds.Height >= unconstrained.TextBounds.Height);
        Assert.True(focusedContent.Contains(focusedLayout.TextBounds));
        Assert.Equal(preferredSize, button.GetPreferredSize(Size.Empty));
    }

    [WinFormsTheory]
    [MemberData(nameof(ModernButtonTypeAndStyleData))]
    public void ButtonBase_ModernOversizedImage_DoesNotPaintOverChrome(
        Type controlType,
        FlatStyle flatStyle)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        using Bitmap image = CreateSolidBitmap(new Size(200, 100), Color.Fuchsia);
        using ButtonBase control = CreateAppearanceButton(controlType, flatStyle);
        control.Image = image;
        control.Size = new Size(120, 44);
        control.CreateControl();
        Rectangle contentBounds = GetModernContentBounds(control, flatStyle);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(control);
        Rectangle imageClient = adapter.GetLayoutData(contentBounds).Client;
        using Bitmap actual = new(control.Width, control.Height);

        control.DrawToBitmap(actual, new Rectangle(Point.Empty, control.Size));

        Rectangle actualImageBounds = FindColorBounds(actual, Color.Fuchsia);
        Assert.True(imageClient.Contains(actualImageBounds));
    }

    [WinFormsTheory]
    [MemberData(nameof(ModernButtonTypeAndStyleData))]
    public void ButtonBase_ModernImageOnlyPreferredSize_FitsImage(
        Type controlType,
        FlatStyle flatStyle)
    {
        using Bitmap image = new(16, 16);
        using ButtonBase control = CreateAppearanceButton(controlType, flatStyle);
        control.AutoSize = true;
        control.Image = image;
        control.Padding = new Padding(2);
        control.Text = string.Empty;
        if (control is Button button)
        {
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        control.Size = control.GetPreferredSize(Size.Empty);
        Rectangle contentBounds = GetModernContentBounds(control, flatStyle);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(control);
        ButtonInternal.ButtonBaseAdapter.LayoutData layout = adapter.GetLayoutData(contentBounds);

        Assert.Equal(image.Size, layout.ImageBounds.Size);
        Assert.True(layout.Client.Contains(layout.ImageBounds));
    }

    [WinFormsTheory]
    [InlineData(VisualStyles.PushButtonState.Normal)]
    [InlineData(VisualStyles.PushButtonState.Hot)]
    [InlineData(VisualStyles.PushButtonState.Pressed)]
    public void ModernButtonRenderers_DefaultBackground_UsesCurrentAccent(
        VisualStyles.PushButtonState state)
    {
        Color accent = Application.SystemVisualSettings.AccentColor;
        Color expected = ModernButtonColorMath.GetDefaultButtonColor(accent, state);
        ModernButtonDarkModeRenderer standardRenderer = new();
        ModernFlatButtonRenderer flatRenderer = new();

        Assert.Equal(expected, standardRenderer.GetBackgroundColor(state, isDefault: true));
        Assert.Equal(expected, flatRenderer.GetBackgroundColor(state, isDefault: true));
    }

    [WinFormsTheory]
    [InlineData(0, 120, 215)]
    [InlineData(255, 185, 0)]
    [InlineData(240, 240, 240)]
    [InlineData(24, 24, 24)]
    [InlineData(180, 20, 90)]
    public void ModernButtonColorMath_AutomaticForeground_MeetsWcagContrast(
        byte red,
        byte green,
        byte blue)
    {
        Color accent = Color.FromArgb(red, green, blue);
        Color normal = ModernButtonColorMath.GetDefaultButtonColor(
            accent,
            VisualStyles.PushButtonState.Normal);
        Color pressed = ModernButtonColorMath.GetDefaultButtonColor(
            accent,
            VisualStyles.PushButtonState.Pressed);

        for (int step = 0; step <= 10; step++)
        {
            Color backColor = PopupButtonColorMath.Blend(normal, pressed, step / 10f);
            Color foreColor = ModernButtonColorMath.GetReadableForeColor(backColor);

            Assert.True(
                PopupButtonColorMath.GetContrastRatio(foreColor, backColor)
                    >= ModernButtonColorMath.MinimumTextContrastRatio);
        }
    }

    [WinFormsFact]
    public void ButtonDarkModeAdapter_ExplicitForeColor_IsPreserved()
    {
        using Button button = new()
        {
            FlatStyle = FlatStyle.Standard,
            ForeColor = Color.FromArgb(30, 80, 130),
            VisualStylesMode = VisualStylesMode.Net11
        };
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        dynamic accessor = adapter.TestAccessor.Dynamic;

        Color actual = (Color)accessor.GetButtonTextColor(
            graphics,
            VisualStyles.PushButtonState.Normal,
            Color.White);

        Assert.Equal(button.ForeColor, actual);
    }

    [WinFormsFact]
    public void ButtonDarkModeAdapter_InheritedForeColor_UsesAutomaticContrast()
    {
        using Panel parent = new() { ForeColor = Color.Red };
        using Button button = new()
        {
            FlatStyle = FlatStyle.Standard,
            VisualStylesMode = VisualStylesMode.Net11
        };
        parent.Controls.Add(button);
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        dynamic accessor = adapter.TestAccessor.Dynamic;

        Color actual = (Color)accessor.GetButtonTextColor(
            graphics,
            VisualStyles.PushButtonState.Normal,
            Color.White);

        Assert.Equal(Color.Black, actual);
        Assert.False(button.ShouldSerializeForeColor());
    }

    [WinFormsFact]
    public void ButtonDarkModeAdapter_LegacyInheritedForeColor_IsPreserved()
    {
        using Panel parent = new() { ForeColor = Color.Red };
        using Button button = new()
        {
            FlatStyle = FlatStyle.Standard,
            VisualStylesMode = VisualStylesMode.Classic
        };
        parent.Controls.Add(button);
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        ButtonInternal.ButtonDarkModeAdapter adapter = new(button);
        dynamic accessor = adapter.TestAccessor.Dynamic;

        Color actual = (Color)accessor.GetButtonTextColor(
            graphics,
            VisualStyles.PushButtonState.Normal,
            Color.White);

        Assert.Equal(Color.Red, actual);
    }

    [WinFormsFact]
    public void CheckBox_AppearanceButton_SystemPreferredSize_IsUnchangedByNet11Mode()
    {
        using CheckBox classic = new()
        {
            Appearance = Appearance.Button,
            FlatStyle = FlatStyle.System,
            Text = "System",
            VisualStylesMode = VisualStylesMode.Classic
        };
        using CheckBox modern = new()
        {
            Appearance = classic.Appearance,
            FlatStyle = classic.FlatStyle,
            Text = classic.Text,
            VisualStylesMode = VisualStylesMode.Net11
        };

        Assert.Equal(classic.GetPreferredSize(Size.Empty), modern.GetPreferredSize(Size.Empty));
    }

    [WinFormsFact]
    public void ButtonBase_SystemColorsChanged_ResetsBackgroundAnimator()
    {
        using Button button = new() { VisualStylesMode = VisualStylesMode.Net11 };
        _ = button.BackColorAnimator;
        _ = button.TestAccessor.Dynamic.PopupKeyCapRenderer;

        button.TestAccessor.Dynamic.OnSystemColorsChanged(EventArgs.Empty);

        Assert.Null(button.TestAccessor.Dynamic._backColorAnimator);
        Assert.Null(button.TestAccessor.Dynamic._popupKeyCapRenderer);
    }

    [WinFormsFact]
    public void Control_DwmColorizationColorChanged_RefreshesChildButtonRenderers()
    {
        using AccentMessageForm form = new();
        using Button button = new() { VisualStylesMode = VisualStylesMode.Net11 };
        form.Controls.Add(button);
        form.CreateControl();
        _ = button.BackColorAnimator;
        _ = button.TestAccessor.Dynamic.PopupKeyCapRenderer;
        Message message = new() { Msg = (int)PInvokeCore.WM_DWMCOLORIZATIONCOLORCHANGED };

        form.WndProc(ref message);

        Assert.Null(button.TestAccessor.Dynamic._backColorAnimator);
        Assert.Null(button.TestAccessor.Dynamic._popupKeyCapRenderer);
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

    private static ButtonBase CreateAppearanceButton(Type controlType, FlatStyle flatStyle)
    {
        ButtonBase control = (ButtonBase)Activator.CreateInstance(controlType);
        control.FlatStyle = flatStyle;
        control.VisualStylesMode = VisualStylesMode.Net11;

        if (control is CheckBox checkBox)
        {
            checkBox.Appearance = Appearance.Button;
        }
        else if (control is RadioButton radioButton)
        {
            radioButton.Appearance = Appearance.Button;
        }

        return control;
    }

    private static Rectangle GetModernContentBounds(ButtonBase control, FlatStyle flatStyle)
    {
        if (flatStyle == FlatStyle.Popup)
        {
            PopupButtonRenderContext context = new()
            {
                Bounds = control.ClientRectangle,
                Text = control.Text,
                Font = control.Font,
                BorderWidth = control.FlatAppearance.BorderSize,
                Enabled = control.Enabled,
                ImageSize = control.Image?.Size ?? Size.Empty,
                ImageAlign = control.ImageAlign,
                TextAlign = control.TextAlign,
                TextImageRelation = control.TextImageRelation,
                RightToLeft = control.RightToLeft,
                DeviceDpi = control.DeviceDpi,
                HighContrast = false
            };

            return PopupButtonKeyCapRenderer.GetContentBounds(context);
        }

        ButtonDarkModeRendererBase renderer = flatStyle == FlatStyle.Standard
            ? new ModernButtonDarkModeRenderer()
            : new ModernFlatButtonRenderer();
        renderer.DeviceDpi = control.DeviceDpi;
        renderer.FlatAppearance = control.FlatAppearance;
        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        VisualStyles.PushButtonState state = control.Enabled
            ? VisualStyles.PushButtonState.Normal
            : VisualStyles.PushButtonState.Disabled;
        Color backColor = renderer.GetBackgroundColor(state, isDefault: false);

        return renderer.DrawButtonBackground(
            graphics,
            control.ClientRectangle,
            state,
            isDefault: false,
            focused: false,
            backColor: backColor);
    }

    private static Rectangle GetModernStandardContentBounds(Button button, bool focused)
    {
        ModernButtonDarkModeRenderer renderer = new()
        {
            DeviceDpi = button.DeviceDpi,
            FlatAppearance = button.FlatAppearance
        };
        using Bitmap bitmap = new(button.Width, button.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle contentBounds = Rectangle.Empty;
        Color backColor = renderer.GetBackgroundColor(
            VisualStyles.PushButtonState.Normal,
            button.IsDefault);

        renderer.RenderButton(
            graphics,
            button,
            button.ClientRectangle,
            button.FlatStyle,
            VisualStyles.PushButtonState.Normal,
            button.IsDefault,
            focused,
            showFocusCues: true,
            parentBackgroundColor: button.Parent?.BackColor ?? button.BackColor,
            backColor: backColor,
            paintContent: bounds => contentBounds = bounds);

        return contentBounds;
    }

    private static Bitmap CreateSolidBitmap(Size size, Color color)
    {
        Bitmap bitmap = new(size.Width, size.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(color);

        return bitmap;
    }

    private static Rectangle GetExpectedBackgroundImageBounds(
        Rectangle bounds,
        Rectangle clipRectangle,
        Size imageSize,
        ImageLayout imageLayout)
    {
        if (imageLayout == ImageLayout.Tile)
        {
            return clipRectangle;
        }

        Rectangle imageRectangle = ControlPaint.CalculateBackgroundImageRectangle(
            bounds,
            imageSize,
            imageLayout);
        if (imageLayout == ImageLayout.None
            && !clipRectangle.Contains(imageRectangle))
        {
            imageRectangle.Offset(clipRectangle.Location);
        }

        imageRectangle.Intersect(clipRectangle);
        return imageRectangle;
    }

    private static Rectangle FindColorBounds(Bitmap bitmap, Color color)
    {
        int argb = color.ToArgb();
        int left = bitmap.Width;
        int top = bitmap.Height;
        int right = -1;
        int bottom = -1;

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).ToArgb() != argb)
                {
                    continue;
                }

                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x);
                bottom = Math.Max(bottom, y);
            }
        }

        return right >= left && bottom >= top
            ? Rectangle.FromLTRB(left, top, right + 1, bottom + 1)
            : Rectangle.Empty;
    }

    private static Rectangle FindDifferenceBounds(
        Bitmap first,
        Bitmap second)
    {
        int left = first.Width;
        int top = first.Height;
        int right = -1;
        int bottom = -1;

        for (int y = 0; y < first.Height; y++)
        {
            for (int x = 0; x < first.Width; x++)
            {
                if (first.GetPixel(x, y).ToArgb()
                    == second.GetPixel(x, y).ToArgb())
                {
                    continue;
                }

                left = Math.Min(left, x);
                top = Math.Min(top, y);
                right = Math.Max(right, x);
                bottom = Math.Max(bottom, y);
            }
        }

        return right >= left && bottom >= top
            ? Rectangle.FromLTRB(left, top, right + 1, bottom + 1)
            : Rectangle.Empty;
    }

    private static int CountDifferentPixels(Bitmap first, Bitmap second, Rectangle bounds)
    {
        int count = 0;

        for (int y = bounds.Top; y < bounds.Bottom; y++)
        {
            for (int x = bounds.Left; x < bounds.Right; x++)
            {
                if (first.GetPixel(x, y).ToArgb() != second.GetPixel(x, y).ToArgb())
                {
                    count++;
                }
            }
        }

        return count;
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

    /// <summary>
    ///  Exposes colorization messages for renderer-refresh tests.
    /// </summary>
    private sealed class AccentMessageForm : Form
    {
        public new void WndProc(ref Message message) => base.WndProc(ref message);
    }
}
