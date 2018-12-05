// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Internal;
    using System;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Diagnostics;

    
    /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer"]/*' />
    /// <devdoc>
    ///    <para>
    ///     This class provides API for drawing GDI text.
    ///    </para>
    /// </devdoc>
    public sealed class TextRenderer
    {
        //cannot instantiate
        private TextRenderer()
        {
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            IntPtr hdc = dc.GetHdc();

            try
            {
                using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ))
                {
                    using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                        wg.DrawText(text, wf, pt, foreColor);
                    }
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText1"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, Color backColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            IntPtr hdc = dc.GetHdc();

            try
            {
                using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ))
                {
                    using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                        wg.DrawText(text, wf, pt, foreColor, backColor);
                    }
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText2"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, flags ))
            {
                using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                    wgr.WindowsGraphics.DrawText(text, wf, pt, foreColor, GetIntTextFormatFlags(flags));
                }
            }
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText3"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Point pt, Color foreColor, Color backColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, flags ))
            {
                using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                    wgr.WindowsGraphics.DrawText(text, wf, pt, foreColor, backColor, GetIntTextFormatFlags(flags));
                }
            }
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText4"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            IntPtr hdc = dc.GetHdc();

            try
            {
                using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ))
                {
                    using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                        wg.DrawText(text, wf, bounds, foreColor);
                    }
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText5"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, Color backColor)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            IntPtr hdc = dc.GetHdc();

            try
            {
                using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ))
                {
                    using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                        wg.DrawText(text, wf, bounds, foreColor, backColor);
                    }
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText6"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, flags ))
            {
                using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont( font, fontQuality )) {
                    wgr.WindowsGraphics.DrawText( text, wf, bounds, foreColor, GetIntTextFormatFlags( flags ) );
                }
            }
        }

        /// <include file='doc\TextRenderer.uex' path='docs/doc[@for="TextRenderer.DrawText7"]/*' />
        public static void DrawText(IDeviceContext dc, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, TextFormatFlags flags)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using( WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper( dc, flags ))
            {
                using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont( font, fontQuality )) {
                    wgr.WindowsGraphics.DrawText(text, wf, bounds, foreColor, backColor, GetIntTextFormatFlags(flags));
                }
            }
        }

        private static IntTextFormatFlags GetIntTextFormatFlags(TextFormatFlags flags)
        {
            if( ((uint)flags & WindowsGraphics.GdiUnsupportedFlagMask) == 0 )
            {
                return (IntTextFormatFlags) flags;
            }

            // Clear TextRenderer custom flags.
            IntTextFormatFlags windowsGraphicsSupportedFlags = (IntTextFormatFlags) ( ((uint)flags) & ~WindowsGraphics.GdiUnsupportedFlagMask );

            return windowsGraphicsSupportedFlags;
        }

        /// MeasureText wrappers.
       
        public static Size MeasureText(string text, Font font )
        {
            if (string.IsNullOrEmpty(text)) 
            {
                return Size.Empty;
            }
            
            using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font)) {
                return WindowsGraphicsCacheManager.MeasurementGraphics.MeasureText(text, wf);
            }           
        }

        public static Size MeasureText(string text, Font font, Size proposedSize )
        {
            if (string.IsNullOrEmpty(text)) 
            {
                return Size.Empty;
            }
            
            using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font)) {
                return WindowsGraphicsCacheManager.MeasurementGraphics.MeasureText(text, WindowsGraphicsCacheManager.GetWindowsFont(font), proposedSize);
            }
        }

        public static Size MeasureText(string text, Font font, Size proposedSize, TextFormatFlags flags )
        {
            if (string.IsNullOrEmpty(text)) 
            {
                return Size.Empty;
            }
            using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font)) {
                return WindowsGraphicsCacheManager.MeasurementGraphics.MeasureText(text, wf, proposedSize, GetIntTextFormatFlags(flags));
            }
        }

        public static Size MeasureText(IDeviceContext dc, string text, Font font)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (string.IsNullOrEmpty(text)) 
            {
                return Size.Empty;
            }
            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            IntPtr hdc = dc.GetHdc();

            try
            {
                using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ))
                {
                    using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                        return wg.MeasureText(text, wf);
                    }
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public static Size MeasureText(IDeviceContext dc, string text, Font font, Size proposedSize )
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (string.IsNullOrEmpty(text)) 
            {
                return Size.Empty;
            }

            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            IntPtr hdc = dc.GetHdc();

            try
            {
                using( WindowsGraphics wg = WindowsGraphics.FromHdc( hdc ))
                {
                    using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                        return wg.MeasureText(text, wf, proposedSize);
                    }
                }
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }

        public static Size MeasureText(IDeviceContext dc, string text, Font font, Size proposedSize, TextFormatFlags flags )
        {            
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }
            if (string.IsNullOrEmpty(text)) 
            {
                return Size.Empty;
            }
            WindowsFontQuality fontQuality = WindowsFont.WindowsFontQualityFromTextRenderingHint(dc as Graphics);

            using (WindowsGraphicsWrapper wgr = new WindowsGraphicsWrapper(dc, flags))
            {
                using (WindowsFont wf = WindowsGraphicsCacheManager.GetWindowsFont(font, fontQuality)) {
                    return wgr.WindowsGraphics.MeasureText(text, wf, proposedSize, GetIntTextFormatFlags(flags));
                }
            }
        }


        internal static Color DisabledTextColor(Color backColor) {
            if (SystemInformation.HighContrast && AccessibilityImprovements.Level1) {
                return SystemColors.GrayText;
            }

            //Theme specs -- if the backcolor is darker than Control, we use
            // ControlPaint.Dark(backcolor).  Otherwise we use ControlDark.
            Color disabledTextForeColor = SystemColors.ControlDark;
            if (ControlPaint.IsDarker(backColor, SystemColors.Control)) {
                disabledTextForeColor = ControlPaint.Dark(backColor);
            }
            return disabledTextForeColor;
        }
    }
}

