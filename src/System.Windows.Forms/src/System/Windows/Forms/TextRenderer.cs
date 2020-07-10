// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Text;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class provides API for drawing GDI text.
    /// </summary>
    public static class TextRenderer
    {
        private static readonly Gdi32.QUALITY _defaultQuality = GetDefaultFontQuality();

        // Used to clear TextRenderer specific flags from TextFormatFlags
        internal const int GdiUnsupportedFlagMask = (unchecked((int)0xFF000000));

        internal static Size MaxSize { get; } = new Size(int.MaxValue, int.MaxValue);

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color foreColor)
            => DrawTextInternal(dc, text, font, pt, foreColor);

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color foreColor, Color backColor)
            => DrawTextInternal(dc, text, font, pt, foreColor, backColor);

        public static void DrawText(
            IDeviceContext dc,
            string? text,
            Font? font,
            Point pt,
            Color foreColor,
            TextFormatFlags flags)
            => DrawTextInternal(dc, text, font, pt, foreColor, flags: GetTextFormatFlags(flags));

        public static void DrawText(
            IDeviceContext dc,
            string? text,
            Font? font,
            Point pt,
            Color foreColor,
            Color backColor,
            TextFormatFlags flags)
            => DrawTextInternal(dc, text, font, pt, foreColor, backColor, flags: GetTextFormatFlags(flags));

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Rectangle bounds, Color foreColor)
            => DrawTextInternal(dc, text, font, bounds, foreColor);

        public static void DrawText(
            IDeviceContext dc,
            string? text, Font?
            font, Rectangle bounds,
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
            => DrawTextInternal(dc, text, font, bounds, foreColor, flags: GetTextFormatFlags(flags));

        public static void DrawText(
            IDeviceContext dc,
            string? text,
            Font? font,
            Rectangle bounds,
            Color foreColor,
            Color backColor,
            TextFormatFlags flags)
            => DrawTextInternal(dc, text, font, bounds, foreColor, backColor, flags: GetTextFormatFlags(flags));

        private static void DrawTextInternal(
            IDeviceContext dc,
            string? text,
            Font? font,
            Point pt,
            Color foreColor,
            Color backColor = default,
            User32.DT flags = User32.DT.DEFAULT)
            => DrawTextInternal(dc, text, font, new Rectangle(pt, MaxSize), foreColor, backColor, flags);

        private static void DrawTextInternal(
            IDeviceContext dc,
            string? text,
            Font? font,
            Rectangle bounds,
            Color foreColor,
            Color backColor = default,
            User32.DT flags = User32.DT.CENTER | User32.DT.VCENTER)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            // Avoid creating the HDC, etc if we're not going to do any drawing
            if (string.IsNullOrEmpty(text) || foreColor == Color.Transparent)
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
            => DrawTextInternal(e, text, font, bounds, foreColor, flags: GetTextFormatFlags(flags));

        internal static void DrawTextInternal(
            PaintEventArgs e,
            string? text,
            Font? font,
            Rectangle bounds,
            Color foreColor,
            Color backColor = default,
            User32.DT flags = User32.DT.CENTER | User32.DT.VCENTER)
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
                DrawTextInternal(hdc, text, font, bounds, foreColor, _defaultQuality, backColor, flags);
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
            => DrawTextInternal(hdc, text, font, bounds, foreColor, fontQuality, default, GetTextFormatFlags(flags));

        internal static void DrawTextInternal(
            Gdi32.HDC hdc,
            string? text,
            Font? font,
            Rectangle bounds,
            Color foreColor,
            Gdi32.QUALITY fontQuality,
            Color backColor,
            User32.DT flags)
        {
            using var hfont = GdiCache.GetHFONT(font, fontQuality);
            hdc.DrawText(text, hfont, bounds, foreColor, flags, backColor);
        }

        private static User32.DT GetTextFormatFlags(TextFormatFlags flags)
        {
            if (((uint)flags & GdiUnsupportedFlagMask) == 0)
            {
                return (User32.DT)flags;
            }

            // Clear TextRenderer custom flags.
            User32.DT windowsGraphicsSupportedFlags = (User32.DT)((uint)flags & ~GdiUnsupportedFlagMask);

            return windowsGraphicsSupportedFlags;
        }

        public static Size MeasureText(string? text, Font? font)
            => MeasureTextInternal(text, font, MaxSize);

        public static Size MeasureText(string? text, Font? font, Size proposedSize)
            => MeasureTextInternal(text, font, proposedSize);

        public static Size MeasureText(string? text, Font? font, Size proposedSize, TextFormatFlags flags)
            => MeasureTextInternal(text, font, proposedSize, flags);

        public static Size MeasureText(IDeviceContext dc, string? text, Font? font)
            => MeasureTextInternal(dc, text, font, MaxSize);

        public static Size MeasureText(IDeviceContext dc, string? text, Font? font, Size proposedSize)
            => MeasureTextInternal(dc, text, font, proposedSize);

        public static Size MeasureText(
            IDeviceContext dc,
            string? text,
            Font? font,
            Size proposedSize,
            TextFormatFlags flags)
            => MeasureTextInternal(dc, text, font, proposedSize, flags);

        private static Size MeasureTextInternal(
            string? text,
            Font? font,
            Size proposedSize,
            TextFormatFlags flags = TextFormatFlags.Bottom)
        {
            if (string.IsNullOrEmpty(text))
                return Size.Empty;

            using var hfont = GdiCache.GetHFONT(font);
            using var screen = GdiCache.GetScreenDC();

            return screen.HDC.MeasureText(text, hfont, proposedSize, GetTextFormatFlags(flags));
        }

        private static Size MeasureTextInternal(
            IDeviceContext dc,
            string? text,
            Font? font,
            Size proposedSize,
            TextFormatFlags flags = TextFormatFlags.Bottom)
        {
            if (dc == null)
                throw new ArgumentNullException(nameof(dc));

            if (string.IsNullOrEmpty(text))
                return Size.Empty;

            // This MUST come before retreiving the HDC, which locks the Graphics object
            Gdi32.QUALITY quality = FontQualityFromTextRenderingHint(dc);

            using var hdc = new DeviceContextHdcScope(dc);
            using var hfont = GdiCache.GetHFONT(font, quality);
            return hdc.MeasureText(text, hfont, proposedSize, GetTextFormatFlags(flags));
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
