// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Metafiles;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class CheckBoxRendererTests : AbstractButtonBaseTests
{
    [WinFormsTheory]
    [InlineData(CheckBoxState.CheckedNormal)]
    [InlineData(CheckBoxState.MixedNormal)]
    public void CheckBoxRenderer_DrawCheckBox(CheckBoxState cBState)
    {
        using Form form = new();
        using CheckBox control = (CheckBox)CreateButton();
        form.Controls.Add(control);

        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;

        CheckBoxRenderer.DrawCheckBox(graphics, point, bounds, control.Text, SystemFonts.DefaultFont, false, cBState);

        emf.Validate(
            state,
            Application.RenderWithVisualStyles
                ? Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_ALPHABLEND)
                : Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS), 1)
        );
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11496")]
    [WinFormsTheory]
    [InlineData(CheckBoxState.CheckedNormal)]
    [InlineData(CheckBoxState.MixedNormal)]
    public void CheckBoxRenderer_DrawCheckBox_OverloadWithSizeAndText(CheckBoxState cBState)
    {
        // Skip validation of CheckBoxState.CheckedNormal on X86 due to the active issue "https://github.com/dotnet/winforms/issues/11496"
        if (cBState == CheckBoxState.CheckedNormal
            && RuntimeInformation.ProcessArchitecture == Architecture.X86)
        {
            return;
        }

        using Form form = new();
        using CheckBox control = (CheckBox)CreateButton();
        form.Controls.Add(control);

        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;
        control.Text = "Text";

        CheckBoxRenderer.DrawCheckBox(graphics, point, bounds, control.Text, SystemFonts.DefaultFont, false, cBState);

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
    [InlineData(TextFormatFlags.Default, CheckBoxState.CheckedNormal)]
    [InlineData(TextFormatFlags.Default, CheckBoxState.MixedNormal)]
    [InlineData(TextFormatFlags.GlyphOverhangPadding, CheckBoxState.MixedHot)]
    [InlineData(TextFormatFlags.PreserveGraphicsTranslateTransform, CheckBoxState.CheckedPressed)]
    [InlineData(TextFormatFlags.TextBoxControl, CheckBoxState.UncheckedNormal)]
    public void CheckBoxRenderer_DrawCheckBox_VisualStyleOn_OverloadWithTextFormat(TextFormatFlags textFormat, CheckBoxState cBState)
    {
        using Form form = new();
        using CheckBox control = (CheckBox)CreateButton();
        form.Controls.Add(control);

        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;
        control.Text = "Text";

        CheckBoxRenderer.DrawCheckBox(graphics, point, bounds, control.Text, SystemFonts.DefaultFont, textFormat, false, cBState);

        emf.Validate(
            state,
            Application.RenderWithVisualStyles
                ? Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_ALPHABLEND)
                : Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_STRETCHDIBITS), 1),
            Validate.TextOut(
                control.Text,
                bounds: new Rectangle(3, 0, 20, 12),
                State.FontFace(SystemFonts.DefaultFont.Name)
            )
        );
    }

    [WinFormsTheory]
    [InlineData(CheckBoxState.CheckedNormal, true)]
    [InlineData(CheckBoxState.MixedNormal, true)]
    [InlineData(CheckBoxState.CheckedNormal, false)]
    [InlineData(CheckBoxState.MixedNormal, false)]
    public void CheckBoxRenderer_DrawCheckBox_OverloadWithHandle(CheckBoxState cBState, bool focus)
    {
        using Form form = new();
        using CheckBox control = (CheckBox)CreateButton();
        form.Controls.Add(control);
        form.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);
        Point point = new(control.Location.X, control.Location.Y);
        Rectangle bounds = control.Bounds;
        control.Text = "Text";

        CheckBoxRenderer.DrawCheckBox(graphics, point, bounds, control.Text, SystemFonts.DefaultFont, TextFormatFlags.Default, focus, cBState, HWND.Null);

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

    protected override ButtonBase CreateButton() => new CheckBox();
}
