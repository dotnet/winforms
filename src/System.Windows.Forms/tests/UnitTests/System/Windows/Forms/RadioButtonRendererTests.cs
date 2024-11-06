// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
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

    [WinFormsTheory]
    [BoolData]
    public void RadioButtonRenderer_DrawRadioButton_OverloadWithHandle(bool focus)
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

    protected override ButtonBase CreateButton() => new RadioButton();
}
