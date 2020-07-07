// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Internal;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class provides API for drawing GDI text.
    /// </summary>
    public static class TextRenderer
    {
        public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color foreColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var hdc = new DeviceContextHdcScope(dc, applyGraphicsState: false);
            using WindowsGraphics wg = WindowsGraphics.FromHdc(hdc);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wg.DrawText(text, wf, pt, foreColor);
        }

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color foreColor, Color backColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var hdc = new DeviceContextHdcScope(dc, applyGraphicsState: false);
            using WindowsGraphics wg = WindowsGraphics.FromHdc(hdc);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wg.DrawText(text, wf, pt, foreColor, backColor);
        }

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color foreColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var wgr = new WindowsGraphicsWrapper(dc, flags);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wgr.WindowsGraphics.DrawText(text, wf, pt, foreColor, GetTextFormatFlags(flags));
        }

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Point pt, Color foreColor, Color backColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var wgr = new WindowsGraphicsWrapper(dc, flags);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wgr.WindowsGraphics.DrawText(text, wf, pt, foreColor, backColor, GetTextFormatFlags(flags));
        }

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Rectangle bounds, Color foreColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var hdc = new DeviceContextHdcScope(dc, applyGraphicsState: false);
            using WindowsGraphics wg = WindowsGraphics.FromHdc(hdc);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wg.DrawText(text, wf, bounds, foreColor);
        }

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Rectangle bounds, Color foreColor, Color backColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var hdc = new DeviceContextHdcScope(dc, applyGraphicsState: false);
            using WindowsGraphics wg = WindowsGraphics.FromHdc(hdc);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wg.DrawText(text, wf, bounds, foreColor, backColor);
        }

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Rectangle bounds, Color foreColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var wgr = new WindowsGraphicsWrapper(dc, flags);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wgr.WindowsGraphics.DrawText(text, wf, bounds, foreColor, GetTextFormatFlags(flags));
        }

        public static void DrawText(IDeviceContext dc, string? text, Font? font, Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var wgr = new WindowsGraphicsWrapper(dc, flags);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            wgr.WindowsGraphics.DrawText(text, wf, bounds, foreColor, backColor, GetTextFormatFlags(flags));
        }

        private static User32.DT GetTextFormatFlags(TextFormatFlags flags)
        {
            if (((uint)flags & WindowsGraphics.GdiUnsupportedFlagMask) == 0)
            {
                return (User32.DT)flags;
            }

            // Clear TextRenderer custom flags.
            User32.DT windowsGraphicsSupportedFlags = (User32.DT)(((uint)flags) & ~WindowsGraphics.GdiUnsupportedFlagMask);

            return windowsGraphicsSupportedFlags;
        }

        public static Size MeasureText(string? text, Font? font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font);
            return WindowsGraphicsCacheManager.MeasurementGraphics.MeasureText(text, wf);
        }

        public static Size MeasureText(string? text, Font? font, Size proposedSize)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font);
            return WindowsGraphicsCacheManager.MeasurementGraphics.MeasureText(text, wf, proposedSize);
        }

        public static Size MeasureText(string? text, Font? font, Size proposedSize, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font);
            return WindowsGraphicsCacheManager.MeasurementGraphics.MeasureText(text, wf, proposedSize, GetTextFormatFlags(flags));
        }

        public static Size MeasureText(IDeviceContext dc, string? text, Font? font)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var hdc = new DeviceContextHdcScope(dc, applyGraphicsState: false);
            using WindowsGraphics wg = WindowsGraphics.FromHdc(hdc);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            return wg.MeasureText(text, wf);
        }

        public static Size MeasureText(IDeviceContext dc, string? text, Font? font, Size proposedSize)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var hdc = new DeviceContextHdcScope(dc, applyGraphicsState: false);
            using WindowsGraphics wg = WindowsGraphics.FromHdc(hdc);
            using WindowsFont? wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            return wg.MeasureText(text, wf, proposedSize);
        }

        public static Size MeasureText(IDeviceContext dc, string? text, Font? font, Size proposedSize, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            Gdi32.QUALITY fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using var wgr = new WindowsGraphicsWrapper(dc, flags);
            using var wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality);
            return wgr.WindowsGraphics.MeasureText(text, wf, proposedSize, GetTextFormatFlags(flags));
        }

        internal static Color DisabledTextColor(Color backColor)
        {
            if (SystemInformation.HighContrast)
            {
                return SystemColors.GrayText;
            }

            //Theme specs -- if the backcolor is darker than Control, we use
            // ControlPaint.Dark(backcolor).  Otherwise we use ControlDark.
            Color disabledTextForeColor = SystemColors.ControlDark;
            if (ControlPaint.IsDarker(backColor, SystemColors.Control))
            {
                disabledTextForeColor = ControlPaint.Dark(backColor);
            }
            return disabledTextForeColor;
        }
    }
}
