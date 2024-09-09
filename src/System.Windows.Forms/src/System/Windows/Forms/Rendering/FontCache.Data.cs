// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal sealed partial class FontCache
{
    internal struct Data : IDisposable
    {
        // Note: These defaults are according to the ones in GDI+ but those are not necessarily the same as the system
        // default font. The GetSystemDefaultHFont() method should be used if needed.
        private const string DefaultFaceName = "Microsoft Sans Serif";
        private const byte True = 1;
        private const byte False = 0;

        public WeakReference<Font> Font { get; }
        public HFONT HFONT { get; private set; }
        public FONT_QUALITY Quality { get; }

        private int? _tmHeight;

        public Data(Font font, FONT_QUALITY quality)
        {
            Font = new WeakReference<Font>(font);
            Quality = quality;
            HFONT = FromFont(font, quality);
            _tmHeight = null;
        }

        public unsafe int Height
        {
            get
            {
                if (!_tmHeight.HasValue)
                {
                    using var screenDC = GdiCache.GetScreenHdc();
                    HDC hdc = screenDC.HDC;
                    using SelectObjectScope fontSelection = new(hdc, HFONT);
                    Debug.Assert(PInvoke.GetMapMode(hdc) == HDC_MAP_MODE.MM_TEXT);

                    TEXTMETRICW tm = default;
                    PInvoke.GetTextMetrics(hdc, &tm);
                    _tmHeight = tm.tmHeight;
                }

                return _tmHeight.Value;
            }
        }

        public void Dispose()
        {
            if (!HFONT.IsNull)
            {
                PInvokeCore.DeleteObject(HFONT);
            }

            HFONT = default;
        }

        /// <summary>
        ///  Constructs a WindowsFont object from an existing System.Drawing.Font object (GDI+), based on the screen dc
        ///  MapMode and resolution (normally: MM_TEXT and 96 dpi).
        /// </summary>
        private static unsafe HFONT FromFont(Font font, FONT_QUALITY quality = FONT_QUALITY.DEFAULT_QUALITY)
        {
            string familyName = font.FontFamily.Name;

            // Strip vertical-font mark from the name if needed.
            if (familyName is not null && familyName.Length > 1 && familyName[0] == '@')
            {
                familyName = familyName[1..];
            }

            // Now, creating it using the Font.SizeInPoints makes it GraphicsUnit-independent.

            Debug.Assert(font.SizeInPoints > 0.0f, "size has a negative value.");

            // Get the font height from the specified size. The size is in point units and height in logical units
            // (pixels when using MM_TEXT) so we need to make the conversion using the number of pixels per logical
            // inch along the screen height. (1 point = 1/72 inch.)
            int pixelsY = (int)Math.Ceiling(ScaleHelper.InitialSystemDpi * font.SizeInPoints / 72);

            // The lfHeight represents the font cell height (line spacing) which includes the internal leading; we
            // specify a negative size value (in pixels) for the height so the font mapper provides the closest match
            // for the character height rather than the cell height.

            LOGFONTW logFont = new()
            {
                lfHeight = -pixelsY,
                lfCharSet = (FONT_CHARSET)font.GdiCharSet,
                lfQuality = quality,
                lfWeight = (int)((font.Style & FontStyle.Bold) == FontStyle.Bold ? FW.BOLD : FW.NORMAL),
                lfItalic = (font.Style & FontStyle.Italic) == FontStyle.Italic ? True : False,
                lfUnderline = (font.Style & FontStyle.Underline) == FontStyle.Underline ? True : False,
                lfStrikeOut = (font.Style & FontStyle.Strikeout) == FontStyle.Strikeout ? True : False,
                FaceName = familyName,

                // This is the only difference in what `Font.ToHfont()` ultimately does. GDI+ uses OUT_DEFAULT_PRECIS
                // which appears to be the same thing as OUT_TT_PRECIS. GDI+ also applies the transforms from the
                // screen device context
                lfOutPrecision = FONT_OUTPUT_PRECISION.OUT_TT_PRECIS,
            };

            if (logFont.FaceName.IsEmpty)
            {
                logFont.FaceName = DefaultFaceName;
            }

            HFONT hfont = PInvokeCore.CreateFontIndirect(&logFont);

            if (hfont.IsNull)
            {
                // Get the default font if we couldn't get what we requested.
                logFont.FaceName = DefaultFaceName;
                logFont.lfOutPrecision = FONT_OUTPUT_PRECISION.OUT_TT_ONLY_PRECIS;
                hfont = PInvokeCore.CreateFontIndirect(&logFont);

                Debug.Assert(!hfont.IsNull);
            }

            return hfont;
        }
    }
}
