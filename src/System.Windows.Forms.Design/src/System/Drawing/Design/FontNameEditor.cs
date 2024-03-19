// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Design;

/// <summary>
///  Provides a <see cref="UITypeEditor" /> that paints a glyph for the font name.
/// </summary>
public class FontNameEditor : UITypeEditor
{
    private const float ScaleFactor = 1.5f;

    private static readonly FontStyle[] s_fontStyles =
    [
         FontStyle.Regular,
         FontStyle.Italic,
         FontStyle.Bold,
         FontStyle.Bold | FontStyle.Italic
    ];

    /// <inheritdoc />
    /// <returns>
    ///  <see langword="true" /> as this editor supports the painting of a representation of an object's value.
    /// </returns>
    public override bool GetPaintValueSupported(ITypeDescriptorContext? context) => true;

    /// <inheritdoc />
    public override void PaintValue(PaintValueEventArgs e)
    {
        string? fontName = e.Value as string;
        if (string.IsNullOrWhiteSpace(fontName))
        {
            // Don't draw anything if we don't have a value.
            return;
        }

        try
        {
            using FontFamily fontFamily = new(fontName);
            e.Graphics.FillRectangle(SystemBrushes.ActiveCaption, e.Bounds);

            // Believe it or not, not all font families have a "normal" face. Try normal, then italic,
            // then bold, then bold italic, then give up.
            foreach (var fontStyle in s_fontStyles)
            {
                try
                {
                    DrawFontSample(e, fontFamily, fontStyle);
                    break;
                }
                catch
                {
                    // no-op
                }
            }
        }
        catch
        {
            // Ignore the exception if the fontName does not exist or is invalid,
            // we just won't render a preview of the font.
        }
    }

    /// <summary>
    ///  Tries to render sample of text in specified font and style.
    /// </summary>
    private static void DrawFontSample(PaintValueEventArgs e, FontFamily fontFamily, FontStyle fontStyle)
    {
        float fontSize = e.Bounds.Height / ScaleFactor;
        using Font font = new(fontFamily, fontSize, fontStyle, GraphicsUnit.Pixel);

        StringFormat format = new(StringFormatFlags.NoWrap | StringFormatFlags.NoFontFallback)
        {
            LineAlignment = StringAlignment.Far
        };

        e.Graphics.DrawString("abcd", font, SystemBrushes.ActiveCaptionText, e.Bounds, format);
    }
}
