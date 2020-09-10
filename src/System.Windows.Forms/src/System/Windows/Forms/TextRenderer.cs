// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms.Internal;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class provides API for drawing GDI text.
    /// </summary>
    public static class TextRenderer
    {
        private static readonly Gdi32.QUALITY s_defaultQuality = GetDefaultFontQuality();

        internal static Size MaxSize { get; } = new Size(int.MaxValue, int.MaxValue);

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
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            // Avoid creating the HDC, etc if we're not going to do any drawing
            if (text.IsEmpty || foreColor == Color.Transparent)
                return;

            // This MUST come before retreiving the HDC, which locks the Graphics object
            Gdi32.QUALITY quality = FontQualityFromTextRenderingHint(dc);

            using var hdc = new DeviceContextHdcScope(dc);

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
            Gdi32.HDC hdc = e.HDC;
            if (hdc.IsNull)
            {
                // This MUST come before retreiving the HDC, which locks the Graphics object
                Gdi32.QUALITY quality = FontQualityFromTextRenderingHint(e.GraphicsInternal);

                using var graphicsHdc = new DeviceContextHdcScope(e.GraphicsInternal, applyGraphicsState: false);
                DrawTextInternal(graphicsHdc, text, font, bounds, foreColor, quality, backColor, flags);
            }
            else
            {
                DrawTextInternal(hdc, text, font, bounds, foreColor, s_defaultQuality, backColor, flags);
            }
        }

        internal static void DrawTextInternal(
            Gdi32.HDC hdc,
            string? text,
            Font? font,
            Rectangle bounds,
            Color foreColor,
            Gdi32.QUALITY fontQuality,
            TextFormatFlags flags)
            => DrawTextInternal(hdc, text, font, bounds, foreColor, fontQuality, Color.Empty, flags);

        private static void DrawTextInternal(
            Gdi32.HDC hdc,
            ReadOnlySpan<char> text,
            Font? font,
            Rectangle bounds,
            Color foreColor,
            Gdi32.QUALITY fontQuality,
            Color backColor,
            TextFormatFlags flags)
        {
            using var hfont = GdiCache.GetHFONT(font, fontQuality, hdc);
            hdc.DrawText(text, hfont, bounds, foreColor, flags, backColor);
        }

        private static TextFormatFlags BlockModifyString(TextFormatFlags flags)
        {
            if (flags.HasFlag(TextFormatFlags.ModifyString))
            {
                throw new ArgumentOutOfRangeException(nameof(flags), SR.TextFormatFlagsModifyStringNotAllowed);
            }

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
            using var hfont = GdiCache.GetHFONT(font, Gdi32.QUALITY.DEFAULT, screen);

            return screen.HDC.MeasureText(text, hfont, proposedSize, flags);
        }

        private static Size MeasureTextInternal(
            IDeviceContext dc,
            ReadOnlySpan<char> text,
            Font? font,
            Size proposedSize,
            TextFormatFlags flags = TextFormatFlags.Bottom)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            if (text.IsEmpty)
                return Size.Empty;

            // This MUST come before retreiving the HDC, which locks the Graphics object
            Gdi32.QUALITY quality = FontQualityFromTextRenderingHint(dc);

            using var hdc = new DeviceContextHdcScope(dc);
            using var hfont = GdiCache.GetHFONT(font, quality, hdc);
            return hdc.HDC.MeasureText(text, hfont, proposedSize, flags);
        }

        internal static Color DisabledTextColor(Color backColor)
        {
            if (SystemInformation.HighContrast)
            {
                return SystemColors.GrayText;
            }

            // If the color is darker than SystemColors.Control make it slightly darker,
            // otherwise use the standard control dark color.

            return ControlPaint.IsDarker(backColor, SystemColors.Control)
                ? ControlPaint.Dark(backColor)
                : SystemColors.ControlDark;
        }

        /// <summary>
        ///  Attempts to match the TextRenderingHint of the specified Graphics object with a LOGFONT.lfQuality value.
        /// </summary>
        internal static Gdi32.QUALITY FontQualityFromTextRenderingHint(IDeviceContext? deviceContext)
        {
            if (!(deviceContext is Graphics g))
            {
                return Gdi32.QUALITY.DEFAULT;
            }

            switch (g.TextRenderingHint)
            {
                case TextRenderingHint.ClearTypeGridFit:
                    return Gdi32.QUALITY.CLEARTYPE;
                case TextRenderingHint.AntiAliasGridFit:
                case TextRenderingHint.AntiAlias:
                    return Gdi32.QUALITY.ANTIALIASED;
                case TextRenderingHint.SingleBitPerPixelGridFit:
                    return Gdi32.QUALITY.PROOF;
                case TextRenderingHint.SingleBitPerPixel:
                    return Gdi32.QUALITY.DRAFT;
                default:
                case TextRenderingHint.SystemDefault:
                    return Gdi32.QUALITY.DEFAULT;
            }
        }

        /// <summary>
        ///  Returns what <see cref="FontQualityFromTextRenderingHint(IDeviceContext?)"/> would return in an
        ///  unmodified <see cref="Graphics"/> object (i.e. the default).
        /// </summary>
        private static Gdi32.QUALITY GetDefaultFontQuality()
        {
            if (!SystemInformation.IsFontSmoothingEnabled)
            {
                return Gdi32.QUALITY.PROOF;
            }

            // FE_FONTSMOOTHINGCLEARTYPE = 0x0002
            return SystemInformation.FontSmoothingType == 0x0002
                ? Gdi32.QUALITY.CLEARTYPE : Gdi32.QUALITY.ANTIALIASED;
        }
    }
}
