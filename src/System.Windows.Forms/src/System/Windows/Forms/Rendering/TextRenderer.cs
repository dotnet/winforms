// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Text;

namespace System.Windows.Forms;

/// <summary>
///  This class provides API for drawing GDI text.
/// </summary>
public static class TextRenderer
{
#if DEBUG
    // In various cases the DC may have already been modified, and we don't pass TextFormatFlags.PreserveGraphicsClipping
    // or TextFormatFlags.PreserveGraphicsTranslateTransform flags, that set off the asserts in GetApplyStateFlags
    // method. This flags allows us to skip those assert for the cases we know we don't need these flags.
    internal const TextFormatFlags SkipAssertFlag = (TextFormatFlags)0x4000_0000;
#endif

    internal static FONT_QUALITY DefaultQuality { get; } = GetDefaultFontQuality();

    internal static Size MaxSize { get; } = new(int.MaxValue, int.MaxValue);

    public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color foreColor)
        => DrawTextInternal(dc, text, font, pt, foreColor, Color.Empty);

    /// <summary>
    ///  Draws the specified text at the specified location using the specified device context, font, and color.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="pt">The <see cref="Point"/> that represents the upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    public static void DrawText(IDeviceContext dc, ReadOnlySpan<char> text, Font font, Point pt, Color foreColor)
        => DrawTextInternal(dc, text, font, pt, foreColor, Color.Empty);

    public static void DrawText(
        IDeviceContext dc,
        string? text,
        Font? font,
        Point pt,
        Color foreColor,
        Color backColor)
        => DrawTextInternal(dc, text, font, pt, foreColor, backColor);

    /// <summary>
    ///  Draws the specified text at the specified location, using the specified device context, font, color, and back color.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="pt">The <see cref="Point"/> that represents the upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <param name="backColor">The <see cref="Color"/> to apply to the background area of the drawn text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    public static void DrawText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font font,
        Point pt,
        Color foreColor,
        Color backColor)
        => DrawTextInternal(dc, text, font, pt, foreColor, backColor);

    public static void DrawText(
        IDeviceContext dc,
        string? text,
        Font? font,
        Point pt,
        Color foreColor,
        TextFormatFlags flags)
        => DrawTextInternal(dc, text, font, pt, foreColor, Color.Empty, flags);

    /// <summary>
    ///  Draws the specified text at the specified location using the specified device context, font, color, and
    ///  formatting instructions.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="pt">The <see cref="Point"/> that represents the upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <param name="flags">A bitwise combination of the <see cref="TextFormatFlags"/> values.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown if <see cref="TextFormatFlags.ModifyString"/> is set.
    /// </exception>
    public static void DrawText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Point pt,
        Color foreColor,
        TextFormatFlags flags)
        => DrawTextInternal(
            dc,
            text,
            font,
            pt,
            foreColor,
            Color.Empty,
            BlockModifyString(flags));

    public static void DrawText(
        IDeviceContext dc,
        string? text,
        Font? font,
        Point pt,
        Color foreColor,
        Color backColor,
        TextFormatFlags flags)
        => DrawTextInternal(dc, text, font, pt, foreColor, backColor, flags);

    /// <summary>
    ///  Draws the specified text at the specified location using the specified device context, font, color, back
    ///  color, and formatting instructions.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="pt">The <see cref="Point"/> that represents the upper-left corner of the drawn text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <param name="backColor">The <see cref="Color"/> to apply to the background area of the drawn text.</param>
    /// <param name="flags">A bitwise combination of the <see cref="TextFormatFlags"/> values.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown if <see cref="TextFormatFlags.ModifyString"/> is set.
    /// </exception>
    public static void DrawText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Point pt,
        Color foreColor,
        Color backColor,
        TextFormatFlags flags)
        => DrawTextInternal(
            dc,
            text,
            font,
            pt,
            foreColor,
            backColor,
            BlockModifyString(flags));

    public static void DrawText(IDeviceContext dc, string? text, Font? font, Rectangle bounds, Color foreColor)
        => DrawTextInternal(dc, text, font, bounds, foreColor, Color.Empty);

    /// <summary>
    ///  Draws the specified text within the specified bounds, using the specified device context, font, and color.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    public static void DrawText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Rectangle bounds,
        Color foreColor)
        => DrawTextInternal(dc, text, font, bounds, foreColor, Color.Empty);

    public static void DrawText(
        IDeviceContext dc,
        string? text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        Color backColor)
        => DrawTextInternal(dc, text, font, bounds, foreColor, backColor);

    /// <summary>
    ///  Draws the specified text within the specified bounds using the specified device context, font, color, and
    ///  back color.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <param name="backColor">The <see cref="Color"/> to apply to the background area of the drawn text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    public static void DrawText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        Color backColor)
        => DrawTextInternal(dc, text, font, bounds, foreColor, backColor);

    public static void DrawText(
        IDeviceContext dc,
        string? text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        TextFormatFlags flags)
        => DrawTextInternal(dc, text, font, bounds, foreColor, Color.Empty, flags);

    /// <summary>
    ///  Draws the specified text within the specified bounds using the specified device context, font, color, and
    ///  formatting instructions.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <param name="flags">A bitwise combination of the <see cref="TextFormatFlags"/> values.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown if <see cref="TextFormatFlags.ModifyString"/> is set.
    /// </exception>
    public static void DrawText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        TextFormatFlags flags)
        => DrawTextInternal(
            dc,
            text,
            font,
            bounds,
            foreColor,
            Color.Empty,
            BlockModifyString(flags));

    public static void DrawText(
        IDeviceContext dc,
        string? text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        Color backColor,
        TextFormatFlags flags)
        => DrawTextInternal(dc, text, font, bounds, foreColor, backColor, flags);

    /// <summary>
    ///  Draws the specified text within the specified bounds using the specified device context, font, color,
    ///  back color, and formatting instructions.
    /// </summary>
    /// <param name="dc">The device context in which to draw the text.</param>
    /// <param name="text">The text to draw.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the drawn text.</param>
    /// <param name="bounds">The <see cref="Rectangle"/> that represents the bounds of the text.</param>
    /// <param name="foreColor">The <see cref="Color"/> to apply to the drawn text.</param>
    /// <param name="backColor">The <see cref="Color"/> to apply to the background area of the drawn text.</param>
    /// <param name="flags">A bitwise combination of the <see cref="TextFormatFlags"/> values.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown if <see cref="TextFormatFlags.ModifyString"/> is set.
    /// </exception>
    public static void DrawText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        Color backColor,
        TextFormatFlags flags)
        => DrawTextInternal(
            dc,
            text,
            font,
            bounds,
            foreColor,
            backColor,
            BlockModifyString(flags));

    private static void DrawTextInternal(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Point pt,
        Color foreColor,
        Color backColor,
        TextFormatFlags flags = TextFormatFlags.Default)
        => DrawTextInternal(dc, text, font, new Rectangle(pt, MaxSize), foreColor, backColor, flags);

    internal static void DrawTextInternal(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        Color backColor,
        TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter)
    {
        ArgumentNullException.ThrowIfNull(dc);

        // Avoid creating the HDC, etc if we're not going to do any drawing
        if (text.IsEmpty || foreColor == Color.Transparent)
            return;

        // This MUST come before retrieving the HDC, which locks the Graphics object
        FONT_QUALITY quality = FontQualityFromTextRenderingHint(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope(GetApplyStateFlags(dc, flags));

        DrawTextInternal(hdc, text, font, bounds, foreColor, quality, backColor, flags);
    }

    internal static void DrawTextInternal(
        PaintEventArgs e,
        string? text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        TextFormatFlags flags)
        => DrawTextInternal(e, text, font, bounds, foreColor, Color.Empty, flags);

    internal static void DrawTextInternal(
        PaintEventArgs e,
        string? text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        Color backColor,
        TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter)
    {
        HDC hdc = e.HDC;
        if (hdc.IsNull)
        {
            // This MUST come before retrieving the HDC, which locks the Graphics object
            FONT_QUALITY quality = FontQualityFromTextRenderingHint(e.GraphicsInternal);

            using DeviceContextHdcScope graphicsHdc = new(e.GraphicsInternal, applyGraphicsState: false);
            DrawTextInternal(graphicsHdc, text, font, bounds, foreColor, quality, backColor, flags);
        }
        else
        {
            DrawTextInternal(hdc, text, font, bounds, foreColor, DefaultQuality, backColor, flags);
        }
    }

    internal static void DrawTextInternal(
        HDC hdc,
        string? text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        FONT_QUALITY fontQuality,
        TextFormatFlags flags)
        => DrawTextInternal(hdc, text, font, bounds, foreColor, fontQuality, Color.Empty, flags);

    private static void DrawTextInternal(
        HDC hdc,
        ReadOnlySpan<char> text,
        Font? font,
        Rectangle bounds,
        Color foreColor,
        FONT_QUALITY fontQuality,
        Color backColor,
        TextFormatFlags flags)
    {
        using var hfont = GdiCache.GetHFONT(font, fontQuality, hdc);
        hdc.DrawText(text, hfont, bounds, foreColor, flags, backColor);
    }

    private static TextFormatFlags BlockModifyString(TextFormatFlags flags)
    {
#pragma warning disable CS0618 // Type or member is obsolete - ModifyString is obsolete
        if (flags.HasFlag(TextFormatFlags.ModifyString))
        {
            throw new ArgumentOutOfRangeException(nameof(flags), SR.TextFormatFlagsModifyStringNotAllowed);
        }
#pragma warning restore CS0618

        return flags;
    }

    public static Size MeasureText(string? text, Font? font)
        => MeasureTextInternal(text, font, MaxSize);

    /// <summary>
    ///  Provides the size, in pixels, of the specified text when drawn with the specified font.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the measured text.</param>
    /// <returns>
    ///  The <see cref="Size"/>, in pixels, of text drawn on a single line with the specified font. You can
    ///  manipulate how the text is drawn by using one of the
    ///  <see cref="DrawText(IDeviceContext, ReadOnlySpan{char}, Font?, Rectangle, Color, TextFormatFlags)"/>
    ///  overloads that takes a <see cref="TextFormatFlags"/> parameter. For example, the default behavior of the
    ///  <see cref="TextRenderer"/> is to add padding to the bounding rectangle of the drawn text to accommodate
    ///  overhanging glyphs. If you need to draw a line of text without these extra spaces you should use the
    ///  versions of <see cref="DrawText(IDeviceContext, ReadOnlySpan{char}, Font, Point, Color)"/> and
    ///  <see cref="MeasureText(IDeviceContext, ReadOnlySpan{char}, Font?)"/> that take a Size and
    ///  <see cref="TextFormatFlags"/> parameter. For an example, see
    ///  <see cref="MeasureText(IDeviceContext, string?, Font?, Size, TextFormatFlags)"/>.
    /// </returns>
    public static Size MeasureText(ReadOnlySpan<char> text, Font? font)
        => MeasureTextInternal(text, font, MaxSize);

    public static Size MeasureText(string? text, Font? font, Size proposedSize)
        => MeasureTextInternal(text, font, proposedSize);

    /// <summary>
    ///  Provides the size, in pixels, of the specified text when drawn with the specified font, using the
    ///  specified size to create an initial bounding rectangle.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the measured text.</param>
    /// <param name="proposedSize">The <see cref="Size"/> of the initial bounding rectangle.</param>
    /// <returns>
    ///  The <see cref="Size"/>, in pixels, of <paramref name="text"/> drawn with the specified
    ///  <paramref name="font"/>.
    /// </returns>
    public static Size MeasureText(ReadOnlySpan<char> text, Font? font, Size proposedSize)
        => MeasureTextInternal(text, font, proposedSize);

    public static Size MeasureText(string? text, Font? font, Size proposedSize, TextFormatFlags flags)
        => MeasureTextInternal(text, font, proposedSize, flags);

    /// <summary>
    ///  Provides the size, in pixels, of the specified text when drawn with the specified font and formatting
    ///  instructions, using the specified size to create the initial bounding rectangle for the text.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the measured text.</param>
    /// <param name="proposedSize">The <see cref="Size"/> of the initial bounding rectangle.</param>
    /// <param name="flags">The formatting instructions to apply to the measured text.</param>
    /// <returns>
    ///  The <see cref="Size"/>, in pixels, of <paramref name="text"/> drawn with the specified
    ///  <paramref name="font"/> and format.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown if <see cref="TextFormatFlags.ModifyString"/> is set.
    /// </exception>
    public static Size MeasureText(ReadOnlySpan<char> text, Font? font, Size proposedSize, TextFormatFlags flags)
        => MeasureTextInternal(text, font, proposedSize, BlockModifyString(flags));

    public static Size MeasureText(IDeviceContext dc, string? text, Font? font)
        => MeasureTextInternal(dc, text, font, MaxSize);

    /// <summary>
    ///  Provides the size, in pixels, of the specified text drawn with the specified font in the specified device
    ///  context.
    /// </summary>
    /// <param name="dc">The device context in which to measure the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <returns>
    ///  The <see cref="Size"/>, in pixels, of <paramref name="text"/> drawn with the specified
    ///  <paramref name="font"/> in the specified device context.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    public static Size MeasureText(IDeviceContext dc, ReadOnlySpan<char> text, Font? font)
        => MeasureTextInternal(dc, text, font, MaxSize);

    public static Size MeasureText(IDeviceContext dc, string? text, Font? font, Size proposedSize)
        => MeasureTextInternal(dc, text, font, proposedSize);

    /// <summary>
    ///  Provides the size, in pixels, of the specified text when drawn with the specified font in the specified
    ///  device context, using the specified size to create an initial bounding rectangle for the text.
    /// </summary>
    /// <param name="dc">The device context in which to measure the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the measured text.</param>
    /// <param name="proposedSize">The <see cref="Size"/> of the initial bounding rectangle.</param>
    /// <returns>
    ///  The <see cref="Size"/>, in pixels, of <paramref name="text"/> drawn with the specified
    ///  <paramref name="font"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    public static Size MeasureText(IDeviceContext dc, ReadOnlySpan<char> text, Font? font, Size proposedSize)
        => MeasureTextInternal(dc, text, font, proposedSize);

    public static Size MeasureText(
        IDeviceContext dc,
        string? text,
        Font? font,
        Size proposedSize,
        TextFormatFlags flags)
        => MeasureTextInternal(dc, text, font, proposedSize, flags);

    /// <summary>
    ///  Provides the size, in pixels, of the specified text when drawn with the specified device context, font,
    ///  and formatting instructions, using the specified size to create the initial bounding rectangle for the text.
    /// </summary>
    /// <param name="dc">The device context in which to measure the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The <see cref="Font"/> to apply to the measured text.</param>
    /// <param name="proposedSize">The <see cref="Size"/> of the initial bounding rectangle.</param>
    /// <param name="flags">The formatting instructions to apply to the measured text.</param>
    /// <returns>
    ///  The <see cref="Size"/>, in pixels, of <paramref name="text"/> drawn with the specified
    ///  <paramref name="font"/> and format.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="dc"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  Thrown if <see cref="TextFormatFlags.ModifyString"/> is set.
    /// </exception>
    public static Size MeasureText(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Size proposedSize,
        TextFormatFlags flags)
        => MeasureTextInternal(dc, text, font, proposedSize, BlockModifyString(flags));

    private static Size MeasureTextInternal(
        ReadOnlySpan<char> text,
        Font? font,
        Size proposedSize,
        TextFormatFlags flags = TextFormatFlags.Bottom)
    {
        if (text.IsEmpty)
            return Size.Empty;

        using var screen = GdiCache.GetScreenHdc();
        using var hfont = GdiCache.GetHFONT(font, FONT_QUALITY.DEFAULT_QUALITY, screen);

        return screen.HDC.MeasureText(text, hfont, proposedSize, flags);
    }

    private static Size MeasureTextInternal(
        IDeviceContext dc,
        ReadOnlySpan<char> text,
        Font? font,
        Size proposedSize,
        TextFormatFlags flags = TextFormatFlags.Bottom)
    {
        ArgumentNullException.ThrowIfNull(dc);

        if (text.IsEmpty)
            return Size.Empty;

        // This MUST come before retrieving the HDC, which locks the Graphics object
        FONT_QUALITY quality = FontQualityFromTextRenderingHint(dc);

        // Applying state may not impact text size measurements. Rather than risk missing some
        // case we'll apply as we have historically to avoid surprise regressions.
        using DeviceContextHdcScope hdc = dc.ToHdcScope(GetApplyStateFlags(dc, flags));
        using var hfont = GdiCache.GetHFONT(font, quality, hdc);
        return hdc.HDC.MeasureText(text, hfont, proposedSize, flags);
    }

    internal static Color DisabledTextColor(Color backColor)
    {
        if (SystemInformation.HighContrast)
        {
            return Application.SystemColors.GrayText;
        }

        // If the color is darker than Application.SystemColors.Control make it slightly darker,
        // otherwise use the standard control dark color.

        return ControlPaint.IsDarker(backColor, Application.SystemColors.Control)
            ? ControlPaint.Dark(backColor)
            : Application.SystemColors.ControlDark;
    }

    /// <summary>
    ///  Attempts to match the TextRenderingHint of the specified Graphics object with a LOGFONT.lfQuality value.
    /// </summary>
    internal static FONT_QUALITY FontQualityFromTextRenderingHint(IDeviceContext? deviceContext)
    {
        if (deviceContext is not Graphics g)
        {
            return FONT_QUALITY.DEFAULT_QUALITY;
        }

        return g.TextRenderingHint switch
        {
            TextRenderingHint.ClearTypeGridFit => FONT_QUALITY.CLEARTYPE_QUALITY,
            TextRenderingHint.AntiAliasGridFit or TextRenderingHint.AntiAlias => FONT_QUALITY.ANTIALIASED_QUALITY,
            TextRenderingHint.SingleBitPerPixelGridFit => FONT_QUALITY.PROOF_QUALITY,
            TextRenderingHint.SingleBitPerPixel => FONT_QUALITY.DRAFT_QUALITY,
            _ => FONT_QUALITY.DEFAULT_QUALITY,
        };
    }

    /// <summary>
    ///  Returns what <see cref="FontQualityFromTextRenderingHint(IDeviceContext?)"/> would return in an
    ///  unmodified <see cref="Graphics"/> object (i.e. the default).
    /// </summary>
    private static FONT_QUALITY GetDefaultFontQuality()
    {
        if (!SystemInformation.IsFontSmoothingEnabled)
        {
            return FONT_QUALITY.PROOF_QUALITY;
        }

        // FE_FONTSMOOTHINGCLEARTYPE = 0x0002
        return SystemInformation.FontSmoothingType == 0x0002
            ? FONT_QUALITY.CLEARTYPE_QUALITY : FONT_QUALITY.ANTIALIASED_QUALITY;
    }

    /// <summary>
    ///  Gets the proper <see cref="ApplyGraphicsProperties"/> flags for the given <paramref name="textFormatFlags"/>.
    /// </summary>
    internal static ApplyGraphicsProperties GetApplyStateFlags(IDeviceContext deviceContext, TextFormatFlags textFormatFlags)
    {
        if (deviceContext is not Graphics graphics)
        {
            return ApplyGraphicsProperties.None;
        }

        var apply = ApplyGraphicsProperties.None;
        if (textFormatFlags.HasFlag(TextFormatFlags.PreserveGraphicsClipping))
        {
            apply |= ApplyGraphicsProperties.Clipping;
        }

        if (textFormatFlags.HasFlag(TextFormatFlags.PreserveGraphicsTranslateTransform))
        {
            apply |= ApplyGraphicsProperties.TranslateTransform;
        }

#if DEBUG
        if ((textFormatFlags & SkipAssertFlag) == 0)
        {
            // Clipping and translation transforms applied to Graphics objects are not done on the underlying HDC.
            // When we're rendering text to the HDC we, by default, should apply both. If it is *known* that these
            // aren't wanted we can get a _slight_ performance benefit by not applying them and in that case the
            // SkipAssertFlag bit can be set to skip this check.
            //
            // This application of clipping and translation is meant to make Graphics.DrawText and TextRenderer.DrawText
            // roughly equivalent in the way they render.
            //
            // Note that there aren't flags for other transforms. Windows 9x doesn't support HDC transforms outside of
            // translation (rotation for example), and this likely impacted the decision to only have a translation
            // flag when this was originally written.

            Debug.Assert(apply.HasFlag(ApplyGraphicsProperties.Clipping)
                || graphics.Clip is null
                || graphics.Clip.GetHrgn(graphics) == IntPtr.Zero,
                "Must preserve Graphics clipping region!");

            Debug.Assert(apply.HasFlag(ApplyGraphicsProperties.TranslateTransform)
                || graphics.Transform is null
                || graphics.Transform.IsIdentity,
                "Must preserve Graphics transformation!");
        }
#endif

        return apply;
    }
}
