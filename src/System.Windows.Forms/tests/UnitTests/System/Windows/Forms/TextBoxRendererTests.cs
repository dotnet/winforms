// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Metafiles;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms.Tests;

public class TextBoxRendererTests
{
    public static TheoryData<TextBoxState> DrawTextBox_State_TheoryData() =>
    [
            TextBoxState.Normal,
            TextBoxState.Hot,
            TextBoxState.Selected,
            TextBoxState.Disabled
    ];

    [WinFormsTheory]
    [MemberData(nameof(DrawTextBox_State_TheoryData))]
    public void TextBoxRenderer_DrawTextBox(TextBoxState tBState)
    {
        using Form form = new();
        using TextBox textbox = new();

        form.Controls.Add(textbox);

        form.Handle.Should().NotBe(IntPtr.Zero);
        textbox.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Rectangle bounds = textbox.Bounds;

        TextBoxRenderer.DrawTextBox(graphics, bounds, tBState);

        emf.Validate(
            state,
            Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTTEXTOUTW), 10),
            tBState != TextBoxState.Disabled
                ? Validate.Polygon16(new(new(1, 1), new(bounds.Width - 2, bounds.Height - 2)))
                : null
                );
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawTextBox_State_TheoryData))]
    public void TextBoxRenderer_DrawTextBox_OverloadWithTextAndFont(TextBoxState tBState)
    {
        using Form form = new();
        using TextBox textbox = new();

        form.Controls.Add(textbox);

        form.Handle.Should().NotBe(IntPtr.Zero);
        textbox.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Rectangle bounds = textbox.Bounds;

        TextBoxRenderer.DrawTextBox(graphics, bounds, "text", SystemFonts.DefaultFont, tBState);

        emf.Validate(
            state,
            Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTTEXTOUTW),
                            tBState != TextBoxState.Disabled ? 10 : 11),
            tBState != TextBoxState.Disabled
                ? Validate.Polygon16(new(new(1, 1), new(bounds.Width - 2, bounds.Height - 2)))
                : null,
            Validate.TextOut(
                "text",
                bounds: new Rectangle(6, 3, 16, 12),
                State.FontFace(SystemFonts.DefaultFont.Name)
                )
            );
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawTextBox_State_TheoryData))]
    public void TextBoxRenderer_DrawTextBox_OverloadWithTextFontAndTextBounds(TextBoxState tBState)
    {
        using Form form = new();
        using TextBox textbox = new();

        form.Controls.Add(textbox);

        form.Handle.Should().NotBe(IntPtr.Zero);
        textbox.Handle.Should().NotBe(IntPtr.Zero);

        using EmfScope emf = new();
        DeviceContextState state = new(emf);
        using Graphics graphics = Graphics.FromHdc((IntPtr)emf.HDC);

        Rectangle bounds = textbox.Bounds;
        Rectangle textBounds = new(10, 20, 30, 40);

        TextBoxRenderer.DrawTextBox(graphics, bounds, "text", SystemFonts.DefaultFont, textBounds, tBState);

        emf.Validate(
            state,
            Validate.Repeat(Validate.SkipType(ENHANCED_METAFILE_RECORD_TYPE.EMR_EXTTEXTOUTW),
                            tBState != TextBoxState.Disabled ? 10 : 11),
            tBState != TextBoxState.Disabled
            ? Validate.Polygon16(new(new(1, 1), new(bounds.Width - 2, bounds.Height - 2)))
            : null,
            Validate.TextOut(
                "text",
                bounds: new Rectangle(13, 20, 16, 12),
                State.FontFace(SystemFonts.DefaultFont.Name)
                ));
    }
}
