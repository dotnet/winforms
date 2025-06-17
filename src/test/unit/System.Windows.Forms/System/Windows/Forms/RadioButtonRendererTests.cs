// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Metafiles;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class RadioButtonRendererTests : AbstractButtonBaseTests
{
    [WinFormsTheory]
    [InlineData(RadioButtonState.CheckedNormal)]
    [InlineData(RadioButtonState.CheckedPressed)]
    public void RadioButtonRenderer_DrawRadioButton(RadioButtonState rBState)
    {
        using Form form = new Form();
        using RadioButton control = (RadioButton)CreateButton();
        form.Controls.Add(control);

        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;

        RadioButtonRenderer.DrawRadioButton(graphics, point, rBState);

        if (Application.RenderWithVisualStyles)
        {
            emf.Validate(
                state,
                Application.RenderWithVisualStyles
                    ? Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_ALPHABLEND)
                    : Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS), 1)
            );
        }
    }

    [WinFormsTheory]
    [InlineData(RadioButtonState.CheckedNormal)]
    [InlineData(RadioButtonState.CheckedPressed)]
    public void RadioButtonRenderer_DrawRadioButton_OverloadWithSizeAndText(RadioButtonState rBState)
    {
        using Form form = new Form();
        using RadioButton control = (RadioButton)CreateButton();
        form.Controls.Add(control);

        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;
        control.Text = "Text";

        RadioButtonRenderer.DrawRadioButton(graphics, point, bounds, control.Text, SystemFonts.DefaultFont, false, rBState);

        emf.Validate(
            state,
            Application.RenderWithVisualStyles
                ? Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_ALPHABLEND)
                : Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS), 1),
            Validate.TextOut(
                control.Text,
                bounds: new Rectangle(41, 5, 20, 12),
                State.FontFace(SystemFonts.DefaultFont.Name)
            )
        );
    }

    [WinFormsTheory]
    [InlineData(TextFormatFlags.Default, RadioButtonState.CheckedNormal)]
    [InlineData(TextFormatFlags.Default, RadioButtonState.CheckedPressed)]
    [InlineData(TextFormatFlags.PreserveGraphicsTranslateTransform, RadioButtonState.CheckedPressed)]
    [InlineData(TextFormatFlags.TextBoxControl, RadioButtonState.UncheckedNormal)]
    public void RadioButtonRenderer_DrawRadioButton_OverloadWithTextFormat(TextFormatFlags textFormat,
        RadioButtonState rBState)
    {
        using Form form = new Form();
        using RadioButton control = (RadioButton)CreateButton();
        form.Controls.Add(control);

        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;
        control.Text = "Text";

        RadioButtonRenderer.DrawRadioButton(graphics, point, bounds, control.Text, SystemFonts.DefaultFont, textFormat, false, rBState);
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/12935")]
    [WinFormsTheory]
    [BoolData]
    public void RadioButtonRenderer_DrawRadioButton_OverloadWithHandle(bool focus)
    {
        // Skip verification of focus = true in X86
        // due to the active issue https://github.com/dotnet/winforms/issues/12935
        if (RuntimeInformation.ProcessArchitecture == Architecture.X86 && focus)
        {
            return;
        }

        using Form form = new Form();
        using RadioButton control = (RadioButton)CreateButton();
        form.Controls.Add(control);
        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);
        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;
        control.Text = "Text";

        RadioButtonRenderer.DrawRadioButton(
            graphics,
            point,
            bounds,
            control.Text,
            SystemFonts.DefaultFont,
            TextFormatFlags.Default,
            focus,
            RadioButtonState.CheckedNormal,
            HWND.Null
);

        emf.Validate(
            state,
            Application.RenderWithVisualStyles
                ? Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_ALPHABLEND)
                : Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS), 1),
            Validate.TextOut(
                control.Text,
                bounds: new Rectangle(3, 0, 20, 12),
                State.FontFace(SystemFonts.DefaultFont.Name)
            ),
            (focus
                ? Validate.PolyPolygon16(new(new(bounds.X, bounds.Y), new Size(-1, -1)))
                : null)!,
            (focus
                ? Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS), 2)
                : null)!
            );
    }

    [WinFormsTheory]
    [InlineData(RadioButtonState.CheckedNormal)]
    [InlineData(RadioButtonState.CheckedPressed)]
    [InlineData(RadioButtonState.CheckedDisabled)]
    [InlineData(RadioButtonState.UncheckedNormal)]
    [InlineData(RadioButtonState.UncheckedPressed)]
    [InlineData(RadioButtonState.UncheckedDisabled)]
    public void IsBackgroundPartiallyTransparent_ReturnsExpected(RadioButtonState state)
    {
        bool original = RadioButtonRenderer.RenderMatchingApplicationState;

        try
        {
            RadioButtonRenderer.RenderMatchingApplicationState = false;
            bool resultWithVisualStyles = RadioButtonRenderer.IsBackgroundPartiallyTransparent(state);

            RadioButtonRenderer.RenderMatchingApplicationState = true;
            bool resultWithoutVisualStyles = RadioButtonRenderer.IsBackgroundPartiallyTransparent(state);

            if (Application.RenderWithVisualStyles)
            {
                resultWithVisualStyles.Should().BeTrue();
            }
            else
            {
                resultWithVisualStyles.Should().BeFalse();
            }

            if (!Application.RenderWithVisualStyles)
            {
                resultWithoutVisualStyles.Should().BeFalse();
            }
        }
        finally
        {
            RadioButtonRenderer.RenderMatchingApplicationState = original;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void DrawParentBackground_DoesNotThrow_Or_CallsRenderer(bool renderMatchingApplicationState)
    {
        using Form parent = new();
        using Panel child = new();
        parent.Controls.Add(child);
        parent.Show();
        using Bitmap bmp = new(10, 10);
        using Graphics g = Graphics.FromImage(bmp);
        bool original = RadioButtonRenderer.RenderMatchingApplicationState;
        try
        {
            RadioButtonRenderer.RenderMatchingApplicationState = renderMatchingApplicationState;
            RadioButtonRenderer.DrawParentBackground(g, new Rectangle(0, 0, 10, 10), child);
        }
        finally
        {
            RadioButtonRenderer.RenderMatchingApplicationState = original;
        }
    }

    [WinFormsFact]
    public void DrawRadioButton_WithImage_Overload_CallsMainOverload()
    {
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using Image image = new Bitmap(10, 10);

        Point glyphLocation = new(2, 2);
        Rectangle textBounds = new(5, 5, 30, 15);
        Rectangle imageBounds = new(7, 7, 10, 10);
        string radioButtonText = "Radio";
        Font font = SystemFonts.DefaultFont;
        bool focused = false;
        RadioButtonState state = RadioButtonState.CheckedNormal;

        RadioButtonRenderer.DrawRadioButton(
            g,
            glyphLocation,
            textBounds,
            radioButtonText,
            font,
            image,
            imageBounds,
            focused,
            state
        );
    }

    [WinFormsFact]
    public void DrawRadioButton_WithImage_Overload_AllowsNullTextAndFont()
    {
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using Image image = new Bitmap(10, 10);

        Point glyphLocation = new(0, 0);
        Rectangle textBounds = new(0, 0, 10, 10);
        Rectangle imageBounds = new(0, 0, 10, 10);
        bool focused = false;
        RadioButtonState state = RadioButtonState.UncheckedNormal;

        RadioButtonRenderer.DrawRadioButton(
            g,
            glyphLocation,
            textBounds,
            null,
            null,
            image,
            imageBounds,
            focused,
            state
        );
    }

    [WinFormsTheory]
    [InlineData(RadioButtonState.CheckedNormal)]
    [InlineData(RadioButtonState.CheckedPressed)]
    [InlineData(RadioButtonState.UncheckedNormal)]
    [InlineData(RadioButtonState.UncheckedPressed)]
    public void GetGlyphSize_ReturnsExpectedSize(RadioButtonState state)
    {
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);

        Size size = RadioButtonRenderer.GetGlyphSize(g, state);

        if (Application.RenderWithVisualStyles)
        {
            size.Width.Should().BeGreaterThan(0);
            size.Height.Should().BeGreaterThan(0);
        }
        else
        {
            size.Should().Be(new Size(13, 13));
        }
    }

    [WinFormsFact]
    public void DrawRadioButton_Internal_WithImageAndText_DoesNotThrow()
    {
        using Bitmap bmp = new(30, 30);
        using Graphics g = Graphics.FromImage(bmp);
        using Image image = new Bitmap(10, 10);

        Point glyphLocation = new(1, 1);
        Rectangle textBounds = new(12, 1, 15, 15);
        Rectangle imageBounds = new(2, 2, 10, 10);
        string radioButtonText = "Test";
        Font font = SystemFonts.DefaultFont;
        TextFormatFlags flags = TextFormatFlags.Default;
        bool focused = true;
        RadioButtonState state = RadioButtonState.CheckedNormal;
        HWND hwnd = HWND.Null;

        typeof(RadioButtonRenderer)
            .TestAccessor()
            .Dynamic
            .DrawRadioButton(
                g,
                glyphLocation,
                textBounds,
                radioButtonText,
                font,
                flags,
                image,
                imageBounds,
                focused,
                state,
                hwnd
            );
    }

    [Theory]
    [InlineData(RadioButtonState.CheckedNormal, ButtonState.Checked)]
    [InlineData(RadioButtonState.CheckedHot, ButtonState.Checked)]
    [InlineData(RadioButtonState.CheckedPressed, ButtonState.Checked | ButtonState.Pushed)]
    [InlineData(RadioButtonState.CheckedDisabled, ButtonState.Checked | ButtonState.Inactive)]
    [InlineData(RadioButtonState.UncheckedPressed, ButtonState.Pushed)]
    [InlineData(RadioButtonState.UncheckedDisabled, ButtonState.Inactive)]
    [InlineData(RadioButtonState.UncheckedNormal, ButtonState.Normal)]
    public void ConvertToButtonState_ReturnsExpected(RadioButtonState radioState, ButtonState expected)
    {
        RadioButtonRenderer.ConvertToButtonState(radioState).Should().Be(expected);
    }

    [Theory]
    [InlineData(ButtonState.Checked, false, RadioButtonState.CheckedNormal)]
    [InlineData(ButtonState.Checked, true, RadioButtonState.CheckedHot)]
    [InlineData(ButtonState.Checked | ButtonState.Pushed, false, RadioButtonState.CheckedPressed)]
    [InlineData(ButtonState.Checked | ButtonState.Inactive, false, RadioButtonState.CheckedDisabled)]
    [InlineData(ButtonState.Pushed, false, RadioButtonState.UncheckedPressed)]
    [InlineData(ButtonState.Inactive, false, RadioButtonState.UncheckedDisabled)]
    [InlineData(ButtonState.Normal, false, RadioButtonState.UncheckedNormal)]
    [InlineData(ButtonState.Normal, true, RadioButtonState.UncheckedHot)]
    public void ConvertFromButtonState_ReturnsExpected(ButtonState buttonState, bool isHot, RadioButtonState expected)
    {
        RadioButtonRenderer.ConvertFromButtonState(buttonState, isHot).Should().Be(expected);
    }

    protected override ButtonBase CreateButton() => new RadioButton();
}
