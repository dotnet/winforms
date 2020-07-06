// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Encapsulates a GDI Font object.
    /// </summary>
    internal sealed partial class WindowsFont : ICloneable, IDisposable
    {
        private float _fontSize = -1.0f;        // invalid value.
        private int _lineSpacing;
        private bool _ownHandle;
        private bool _ownedByCacheManager;
        private bool _everOwnedByCacheManager;

        private readonly User32.LOGFONTW _logFont;

        // Note: These defaults are according to the ones in GDI+ but those are not necessarily the same as the system
        // default font.  The GetSystemDefaultHFont() method should be used if needed.
        private const string DefaultFaceName = "Microsoft Sans Serif";
        private const byte True = 1;
        private const byte False = 0;

        /// <summary>
        ///  Creates the font handle.
        /// </summary>
        private unsafe WindowsFont(User32.LOGFONTW logFont, FontStyle style, bool createHandle)
        {
            Debug.Assert(Hfont.IsNull, "hFont is not null, this will generate a handle leak.");

            _logFont = logFont;
            if (_logFont.FaceName.Length == 0)
            {
                _logFont.FaceName = DefaultFaceName;
            }
            Style = style;

            if (createHandle)
            {
                Hfont = Gdi32.CreateFontIndirectW(ref _logFont);

                if (Hfont.IsNull)
                {
                    _logFont.FaceName = DefaultFaceName;
                    _logFont.lfOutPrecision = Gdi32.OUT_PRECIS.TT_ONLY; // TrueType only.

                    Hfont = Gdi32.CreateFontIndirectW(ref _logFont);
                }

                // Update logFont height and other adjusted parameters.
                Gdi32.GetObjectW(Hfont, out _logFont);

                // We created the hFont, we will delete it on dispose.
                _ownHandle = true;
            }
        }

        /// <summary>
        ///  Contructs a WindowsFont object from an existing System.Drawing.Font object (GDI+), based on the screen dc MapMode
        ///  and resolution (normally: MM_TEXT and 96 dpi).
        /// </summary>
        public static WindowsFont FromFont(Font font, Gdi32.QUALITY fontQuality = Gdi32.QUALITY.DEFAULT)
        {
            string familyName = font.FontFamily.Name;

            // Strip vertical-font mark from the name if needed.
            if (familyName != null && familyName.Length > 1 && familyName[0] == '@')
            {
                familyName = familyName.Substring(1);
            }

            // Note: Creating the WindowsFont from Font using a LOGFONT structure from GDI+ (Font.ToLogFont(logFont)) may sound like
            // a better choice (more accurate) for doing this but tests show that is not the case (see WindowsFontTests test suite),
            // the results are the same.  Also, that approach has some issues when the Font is created in a different application
            // domain since the LOGFONT cannot be marshalled properly.

            // Now, creating it using the Font.SizeInPoints makes it GraphicsUnit-independent.

            Debug.Assert(font.SizeInPoints > 0.0f, "size has a negative value.");

            // Get the font height from the specified size.  size is in point units and height in logical
            // units (pixels when using MM_TEXT) so we need to make the conversion using the number of
            // pixels per logical inch along the screen height. (1 point = 1/72 inch.)
            int pixelsY = (int)Math.Ceiling(WindowsGraphicsCacheManager.MeasurementGraphics.DeviceContext.DpiY * font.SizeInPoints / 72);

            // The lfHeight represents the font cell height (line spacing) which includes the internal
            // leading; we specify a negative size value (in pixels) for the height so the font mapper
            // provides the closest match for the character height rather than the cell height (MSDN).

            User32.LOGFONTW logFont = new User32.LOGFONTW()
            {
                lfHeight = -pixelsY,
                lfCharSet = font.GdiCharSet,
                lfOutPrecision = Gdi32.OUT_PRECIS.TT,
                lfQuality = fontQuality,
                lfWeight = (font.Style & FontStyle.Bold) == FontStyle.Bold ? Gdi32.FW.BOLD : Gdi32.FW.NORMAL,
                lfItalic = (font.Style & FontStyle.Italic) == FontStyle.Italic ? True : False,
                lfUnderline = (font.Style & FontStyle.Underline) == FontStyle.Underline ? True : False,
                lfStrikeOut = (font.Style & FontStyle.Strikeout) == FontStyle.Strikeout ? True : False,
                FaceName = familyName
            };

            return new WindowsFont(logFont, font.Style, createHandle: true);
        }

        /// <summary>
        ///  Creates a WindowsFont from the font selected in the supplied dc.
        /// </summary>
        public static WindowsFont FromHdc(Gdi32.HDC hdc)
        {
            Gdi32.HFONT hFont = (Gdi32.HFONT)Gdi32.GetCurrentObject(hdc, Gdi32.ObjectType.OBJ_FONT);

            // don't call DeleteObject on handle from GetCurrentObject, it is the one selected in the hdc.
            return FromHfont(hFont);
        }

        /// <summary>
        ///  Creates a WindowsFont from the handle to a native GDI font and optionally takes ownership of managing
        ///  the lifetime of the handle.
        /// </summary>
        public unsafe static WindowsFont FromHfont(Gdi32.HFONT hFont, bool takeOwnership = false)
        {
            Gdi32.GetObjectW(hFont, out User32.LOGFONTW logFont);

            FontStyle style = FontStyle.Regular;
            if (logFont.lfWeight == Gdi32.FW.BOLD)
            {
                style |= FontStyle.Bold;
            }
            if (logFont.lfItalic == True)
            {
                style |= FontStyle.Italic;
            }
            if (logFont.lfUnderline == True)
            {
                style |= FontStyle.Underline;
            }
            if (logFont.lfStrikeOut == True)
            {
                style |= FontStyle.Strikeout;
            }

            WindowsFont wf = new WindowsFont(logFont, style, createHandle: false)
            {
                Hfont = hFont,
                _ownHandle = takeOwnership // if true, hFont will be deleted on dispose.
            };

            return wf;
        }

        ~WindowsFont()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        internal void Dispose(bool disposing)
        {
            bool deletedHandle = false;
            if (_ownHandle)
            {
                if (!_ownedByCacheManager || !disposing)
                {
                    // If we were ever owned by the CacheManger and we're being disposed
                    // we can be sure that we're not in use by any DC's (otherwise Dispose() wouldn't have been called)
                    // skip the check IsFontInUse check in this case.
                    // Also skip the check if disposing == false, because the cache is thread-static
                    // and that means we're being called from the finalizer.
                    if (_everOwnedByCacheManager || !disposing || !DeviceContexts.IsFontInUse(this))
                    {
                        Debug.Assert(!Hfont.IsNull, "Unexpected null hFont.");

                        Gdi32.DeleteObject(Hfont);
                        Hfont = default;
                        _ownHandle = false;
                        deletedHandle = true;
                    }
                }
            }

            if (disposing && (deletedHandle || !_ownHandle))
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///  Returns a value indicating whether the specified object is a WindowsFont equivalent to this object.
        /// </summary>
        public override bool Equals(object? font)
        {
            if (!(font is WindowsFont winFont))
            {
                return false;
            }

            if (winFont == this)
            {
                return true;
            }

            // WARNING: don't use non-public fields/properties here, the passed-in font object could be a proxy in a
            //          remoting scenario and proxies cannot access internal or private members.

            // Compare params used to create the font.
            return Name == winFont.Name &&
                    LogFontHeight == winFont.LogFontHeight && // Equivalent to comparing Size but always at hand.
                    Style == winFont.Style &&
                    CharSet == winFont.CharSet &&
                    Quality == winFont.Quality;
        }

        /// <summary>
        ///  Gets the hash code for this WindowsFont.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(Style, CharSet, Size);

        /// <summary>
        ///  Clones this object.
        /// </summary>
        public object Clone()
        {
            return new WindowsFont(_logFont, Style, createHandle: true);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[{0}: Name={1}, Size={2} points, Height={3} pixels, Sytle={4}]", GetType().Name, _logFont.FaceName.ToString(), Size, Height, Style);
        }

        ////////////////////////////////////////////
        ///  Properties

        /// <summary>
        ///  Returns this object's native Win32 font handle.  Should NOT be deleted externally.
        ///  Compare with ToHfont method.
        /// </summary>
        public Gdi32.HFONT Hfont { get; private set; }

        public bool OwnedByCacheManager
        {
            get
            {
                return _ownedByCacheManager;
            }
            set
            {
                if (value)
                {
                    _everOwnedByCacheManager = true;
                }
                _ownedByCacheManager = value;
            }
        }

        /// <summary>
        ///  Rendering quality.
        /// </summary>
        public Gdi32.QUALITY Quality
        {
            get
            {
                return _logFont.lfQuality;
            }
        }

        /// <summary>
        ///  Gets the font style.
        /// </summary>
        public FontStyle Style { get; }

        /// <summary>
        ///  Gets the line spacing (cell height) of this font in (screen) pixels using the screen resolution.
        ///  Gets the line spacing (cell height), in pixels (using the screen DC resolution), of this font.
        ///  The line spacing is the vertical distance between the base lines of two consecutive lines of text.
        ///  Thus, the line spacing includes the blank space between lines along with the height of the character
        ///  itself.
        /// </summary>
        public int Height
        {
            get
            {
                if (_lineSpacing == 0)
                {
                    // Observe that the font text metrics are obtained using the resolution of the screen.
                    WindowsGraphics wg = WindowsGraphicsCacheManager.MeasurementGraphics;

                    // No need to reset the font (if changed) since we always set the font before using the MeasurementGraphics
                    // in WindowsGraphics methods.
                    wg.DeviceContext.SelectFont(this);

                    Gdi32.TEXTMETRICW tm = wg.GetTextMetrics();
                    _lineSpacing = tm.tmHeight;
                }

                return _lineSpacing;
            }
        }

        /// <summary>
        ///  Gets the font character set.
        ///  This is used by the system font mapper when searching for the physical font that best matches the logical font.
        /// </summary>
        public byte CharSet
        {
            get
            {
                return _logFont.lfCharSet;
            }
        }

        /// <summary>
        ///  Specifies the height, in logical units, of the font's character cell or character. The character height value (em height)
        ///  is the character cell height value minus the internal-leading value.
        /// </summary>
        public int LogFontHeight
        {
            get
            {
                return _logFont.lfHeight;
            }
        }

        /// <summary>
        ///  The font's face name.
        /// </summary>
        public string Name => _logFont.FaceName.ToString();

        /// <summary>
        ///  Gets the character height (as opposed to the cell height) of the font represented by this object in points.
        ///  Consider
        /// </summary>
        public float Size
        {
            get
            {
                if (_fontSize < 0.0f)
                {
                    WindowsGraphics wg = WindowsGraphicsCacheManager.MeasurementGraphics;

                    // No need to reset the font (if changed) since we always set the font before using the MeasurementGraphics
                    // in WindowsGraphics methods.
                    wg.DeviceContext.SelectFont(this);

                    Gdi32.TEXTMETRICW tm = wg.GetTextMetrics();

                    // Convert the font character height to points.  If lfHeight is negative, Windows
                    // treats the absolute value of that number as a desired font height compatible with
                    // the point size; in this case lfHeight will roughly match the tmHeight field of
                    // the TEXTMETRIC structure less the tmInternalLeading field.
                    int height = _logFont.lfHeight > 0 ? tm.tmHeight : (tm.tmHeight - tm.tmInternalLeading);

                    _fontSize = height * 72f / wg.DeviceContext.DpiY;
                }

                return _fontSize;
            }
        }

        /// <summary>
        ///  Attempts to match the TextRenderingHint of the specified Graphics object with a LOGFONT.lfQuality value.
        /// </summary>
        public static Gdi32.QUALITY WindowsFontQualityFromTextRenderingHint(Graphics? g)
        {
            if (g == null)
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
    }
}
