// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Encapsulates a GDI Font object.
    /// </summary>
    internal sealed partial class WindowsFont : MarshalByRefObject, ICloneable, IDisposable
    {
        const int LogFontNameOffset = 28;

        // Handle to the native Windows font object.
        //
        private IntPtr hFont;
        private float fontSize = -1.0f; //invalid value.
        private int lineSpacing;
        private bool ownHandle;
        private bool ownedByCacheManager;
        private bool everOwnedByCacheManager;

        private readonly IntNativeMethods.LOGFONT logFont;
        private readonly FontStyle style;

        // Note: These defaults are according to the ones in GDI+ but those are not necessarily the same as the system
        // default font.  The GetSystemDefaultHFont() method should be used if needed.
        private const string defaultFaceName = "Microsoft Sans Serif";
        private const float defaultFontSize = 8.25f;
        private const int defaultFontHeight = 13;

#if GDI_FINALIZATION_WATCH
       private string AllocationSite = DbgUtil.StackTrace;
#endif

        /// <summary>
        ///  Creates the font handle.
        /// </summary>
        private void CreateFont()
        {
            Debug.Assert(hFont == IntPtr.Zero, "hFont is not null, this will generate a handle leak.");
            Debug.Assert(logFont != null, "WindowsFont.logFont not initialized.");

            hFont = IntUnsafeNativeMethods.CreateFontIndirect(logFont);

#if TRACK_HFONT
            Debug.WriteLine( DbgUtil.StackTraceToStr(String.Format( "HFONT[0x{0:x8}] = CreateFontIndirect( LOGFONT={1} )", (int) this.hFont, this.logFont)));
#endif
            if (hFont == IntPtr.Zero)
            {
                logFont.lfFaceName = defaultFaceName;
                logFont.lfOutPrecision = IntNativeMethods.OUT_TT_ONLY_PRECIS; // True Type only.

                hFont = IntUnsafeNativeMethods.CreateFontIndirect(logFont);

#if TRACK_HFONT
            Debug.WriteLine( DbgUtil.StackTraceToStr(String.Format( "HFONT[0x{0:x8}] = CreateFontIndirect( LOGFONT={1} )", (int) this.hFont, this.logFont)));
#endif

            }

            // Update logFont height and other adjusted parameters.
            //
            IntUnsafeNativeMethods.GetObject(new HandleRef(this, hFont), logFont);

            // We created the hFont, we will delete it on dispose.
            ownHandle = true;
        }

        /// Constructors.

        /// <summary>
        ///  Contructor to construct font from a face name.
        /// </summary>
        public WindowsFont(string faceName) :
            this(faceName, defaultFontSize, FontStyle.Regular, IntNativeMethods.DEFAULT_CHARSET, WindowsFontQuality.Default)
        {
            // Default size in WinForms is 8.25f.
        }

        /// <summary>
        ///  Contructor to construct font from a face name, a desired size and with the specified style.
        /// </summary>
        public WindowsFont(string faceName, float size) :
            this(faceName, size, FontStyle.Regular, IntNativeMethods.DEFAULT_CHARSET, WindowsFontQuality.Default)
        {
        }

        /// <summary>
        ///  Contructor to construct font from a face name, a desired size and with the specified style.
        /// </summary>
        public WindowsFont(string faceName, float size, FontStyle style) :
            this(faceName, size, style, IntNativeMethods.DEFAULT_CHARSET, WindowsFontQuality.Default)
        {
        }

        /// <summary>
        ///  Contructor to construct font from a face name, a desired size in points and with the specified style
        ///  and character set. The screen dc is used for calculating the font em height.
        /// </summary>
        public WindowsFont(string faceName, float size, FontStyle style, byte charSet, WindowsFontQuality fontQuality)
        {
            Debug.Assert(size > 0.0f, "size has a negative value.");
            const byte True = 1;
            const byte False = 0;
            logFont = new IntNativeMethods.LOGFONT();

            //
            // Get the font height from the specified size.  size is in point units and height in logical
            // units (pixels when using MM_TEXT) so we need to make the conversion using the number of
            // pixels per logical inch along the screen height.
            //
            int pixelsY = (int)Math.Ceiling(WindowsGraphicsCacheManager.MeasurementGraphics.DeviceContext.DpiY * size / 72); // 1 point = 1/72 inch.;

            //
            // The lfHeight represents the font cell height (line spacing) which includes the internal
            // leading; we specify a negative size value (in pixels) for the height so the font mapper
            // provides the closest match for the character height rather than the cell height (MSDN).
            //
            logFont.lfHeight = -pixelsY;
            logFont.lfFaceName = faceName ?? defaultFaceName;
            logFont.lfCharSet = charSet;
            logFont.lfOutPrecision = IntNativeMethods.OUT_TT_PRECIS;
            logFont.lfQuality = (byte)fontQuality;
            logFont.lfWeight = (style & FontStyle.Bold) == FontStyle.Bold ? IntNativeMethods.FW_BOLD : IntNativeMethods.FW_NORMAL;
            logFont.lfItalic = (style & FontStyle.Italic) == FontStyle.Italic ? True : False;
            logFont.lfUnderline = (style & FontStyle.Underline) == FontStyle.Underline ? True : False;
            logFont.lfStrikeOut = (style & FontStyle.Strikeout) == FontStyle.Strikeout ? True : False;

            // Let the Size be recomputed to be consistent with the Height (there may be some precision loss coming from size to height).
            // this.fontSize = size;
            this.style = style;

            CreateFont();
        }

        /// <summary>
        ///  Contructor to construct font from a LOGFONT structure.
        ///  Pass false in the createHandle param to create a 'compatible' font (handle-less, to be used for measuring/comparing) or
        ///  when the handle has already been created.
        /// </summary>
        private WindowsFont(IntNativeMethods.LOGFONT lf, bool createHandle)
        {
            Debug.Assert(lf != null, "lf is null");

            logFont = lf;

            if (logFont.lfFaceName == null)
            {
                logFont.lfFaceName = defaultFaceName;
            }

            style = FontStyle.Regular;
            if (lf.lfWeight == IntNativeMethods.FW_BOLD)
            {
                style |= FontStyle.Bold;
            }
            if (lf.lfItalic == 1)
            {
                style |= FontStyle.Italic;
            }
            if (lf.lfUnderline == 1)
            {
                style |= FontStyle.Underline;
            }
            if (lf.lfStrikeOut == 1)
            {
                style |= FontStyle.Strikeout;
            }

            if (createHandle)
            {
                CreateFont();
            }
        }

        /// <summary>
        ///  Contructs a WindowsFont object from an existing System.Drawing.Font object (GDI+), based on the screen dc MapMode
        ///  and resolution (normally: MM_TEXT and 96 dpi).
        /// </summary>
        public static WindowsFont FromFont(Font font)
        {
            return FromFont(font, WindowsFontQuality.Default);
        }

        public static WindowsFont FromFont(Font font, WindowsFontQuality fontQuality)
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

            return new WindowsFont(familyName, font.SizeInPoints, font.Style, font.GdiCharSet, fontQuality);
        }

        /// <summary>
        ///  Creates a WindowsFont from the font selected in the supplied dc.
        /// </summary>
        public static WindowsFont FromHdc(IntPtr hdc)
        {
            IntPtr hFont = IntUnsafeNativeMethods.GetCurrentObject(new HandleRef(null, hdc), IntNativeMethods.OBJ_FONT);

            // don't call DeleteObject on handle from GetCurrentObject, it is the one selected in the hdc.

            return FromHfont(hFont);
        }

        /// <summary>
        ///  Creates a WindowsFont from the handle to a native GDI font.  It does not take ownership of the
        ///  passed-in handle, the caller needs to delete the hFont when done with the WindowsFont.
        /// </summary>
        public static WindowsFont FromHfont(IntPtr hFont)
        {
            return FromHfont(hFont, false);
        }

        /// <summary>
        ///  Creates a WindowsFont from the handle to a native GDI font and optionally takes ownership of managing
        ///  the lifetime of the handle.
        /// </summary>
        public static WindowsFont FromHfont(IntPtr hFont, bool takeOwnership)
        {
            IntNativeMethods.LOGFONT lf = new IntNativeMethods.LOGFONT();
            IntUnsafeNativeMethods.GetObject(new HandleRef(null, hFont), lf);

            WindowsFont wf = new WindowsFont(lf, /*createHandle*/ false)
            {
                hFont = hFont,
                ownHandle = takeOwnership // if true, hFont will be deleted on dispose.
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
            if (ownHandle)
            {
                if (!ownedByCacheManager || !disposing)
                {

                    // If we were ever owned by the CacheManger and we're being disposed
                    // we can be sure that we're not in use by any DC's (otherwise Dispose() wouldn't have been called)
                    // skip the check IsFontInUse check in this case.
                    // Also skip the check if disposing == false, because the cache is thread-static
                    // and that means we're being called from the finalizer.
                    if (everOwnedByCacheManager || !disposing || !DeviceContexts.IsFontInUse(this))
                    {
                        Debug.Assert(hFont != IntPtr.Zero, "Unexpected null hFont.");
                        DbgUtil.AssertFinalization(this, disposing);

                        IntUnsafeNativeMethods.DeleteObject(new HandleRef(this, hFont));
#if TRACK_HFONT
                        Debug.WriteLine( DbgUtil.StackTraceToStr(String.Format( "DeleteObject(HFONT[0x{0:x8}]))", (int) this.hFont)));
#endif
                        hFont = IntPtr.Zero;
                        ownHandle = false;
                        deletedHandle = true;
                    }
                }
            }

            if (disposing && (deletedHandle || !ownHandle))
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///  Returns a value indicating whether the specified object is a WindowsFont equivalent to this object.
        /// </summary>
        public override bool Equals(object font)
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
        public override int GetHashCode()
        {
            // similar to Font.GetHashCode().
            return (int)((((uint)Style << 13) | ((uint)Style >> 19)) ^
                         (((uint)CharSet << 26) | ((uint)CharSet >> 6)) ^
                         (((uint)Size << 7) | ((uint)Size >> 25)));
        }

        /// <summary>
        ///  Clones this object.
        /// </summary>
        public object Clone()
        {
            return new WindowsFont(logFont, true);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[{0}: Name={1}, Size={2} points, Height={3} pixels, Sytle={4}]", GetType().Name, logFont.lfFaceName, Size, Height, Style);
        }

        ////////////////////////////////////////////
        ///  Properties

        /// <summary>
        ///  Returns this object's native Win32 font handle.  Should NOT be deleted externally.
        ///  Compare with ToHfont method.
        /// </summary>
        public IntPtr Hfont
        {
            get
            {
                //Assert removed. We need to be able to check for Hfont == IntPtr.Zero to determine if the object was disposed.
                //Debug.Assert(this.hFont != IntPtr.Zero, "hFont is null, are you using a disposed object?");
                return hFont;
            }
        }

        /// <summary>
        ///  Determines whether the font has the italic style or not.
        /// </summary>
        public bool Italic
        {
            get
            {
                return logFont.lfItalic == 1;
            }
        }

        public bool OwnedByCacheManager
        {
            get
            {
                return ownedByCacheManager;
            }
            set
            {
                if (value)
                {
                    everOwnedByCacheManager = true;
                }
                ownedByCacheManager = value;
            }
        }

        /// <summary>
        ///  Rendering quality.
        /// </summary>
        public WindowsFontQuality Quality
        {
            get
            {
                return (WindowsFontQuality)logFont.lfQuality;
            }
        }

        /// <summary>
        ///  Gets the font style.
        /// </summary>
        public FontStyle Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        ///  Gets the line spacing (cell height) of this font in (screen) pixels using the screen resolution.
        ///  Gets the line spacing (cell height), in pixels (using the screen DC resolution), of this font.
        ///  The line spacing is the vertical distance between the base lines of two consecutive lines of text.
        ///  Thus, the line spacing includes the blank space between lines along with the height of the character
        ///  itself.
        /// </summary>
        public int Height
        {
            //

            get
            {
                if (lineSpacing == 0)
                {
                    // Observe that the font text metrics are obtained using the resolution of the screen.
                    WindowsGraphics wg = WindowsGraphicsCacheManager.MeasurementGraphics;

                    // No need to reset the font (if changed) since we always set the font before using the MeasurementGraphics
                    // in WindowsGraphics methods.
                    wg.DeviceContext.SelectFont(this);

                    IntNativeMethods.TEXTMETRIC tm = (IntNativeMethods.TEXTMETRIC)wg.GetTextMetrics();
                    lineSpacing = tm.tmHeight;
                }

                return lineSpacing;
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
                return logFont.lfCharSet;
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
                return logFont.lfHeight;
            }
        }

        /// <summary>
        ///  The font's face name.
        /// </summary>
        public string Name
        {
            get
            {
                return logFont.lfFaceName;
            }
        }

        /// <summary>
        ///  Gets the character height (as opposed to the cell height) of the font represented by this object in points.
        ///  Consider
        /// </summary>
        public float Size
        {
            get
            {
                if (fontSize < 0.0f)
                {
                    WindowsGraphics wg = WindowsGraphicsCacheManager.MeasurementGraphics;

                    // No need to reset the font (if changed) since we always set the font before using the MeasurementGraphics
                    // in WindowsGraphics methods.
                    wg.DeviceContext.SelectFont(this);

                    IntNativeMethods.TEXTMETRIC tm = (IntNativeMethods.TEXTMETRIC)wg.GetTextMetrics();

                    //
                    // Convert the font character height to points.  If lfHeight is negative, Windows
                    // treats the absolute value of that number as a desired font height compatible with
                    // the point size; in this case lfHeight will roughly match the tmHeight field of
                    // the TEXTMETRIC structure less the tmInternalLeading field.
                    //
                    int height = logFont.lfHeight > 0 ? tm.tmHeight : (tm.tmHeight - tm.tmInternalLeading);

                    fontSize = height * 72f / wg.DeviceContext.DpiY;
                }

                return fontSize;
            }
        }

        /// <summary>
        ///  Attempts to match the TextRenderingHint of the specified Graphics object with a LOGFONT.lfQuality value.
        /// </summary>
        public static WindowsFontQuality WindowsFontQualityFromTextRenderingHint(Graphics g)
        {
            if (g == null)
            {
                return WindowsFontQuality.Default;
            }

            switch (g.TextRenderingHint)
            {
                case TextRenderingHint.ClearTypeGridFit:
                    return WindowsFontQuality.ClearType;
                case TextRenderingHint.AntiAliasGridFit:
                    return WindowsFontQuality.AntiAliased;
                case TextRenderingHint.AntiAlias:
                    return WindowsFontQuality.AntiAliased;
                case TextRenderingHint.SingleBitPerPixelGridFit:
                    return WindowsFontQuality.Proof;
                case TextRenderingHint.SingleBitPerPixel:
                    return WindowsFontQuality.Draft;
                default:
                case TextRenderingHint.SystemDefault:
                    return WindowsFontQuality.Default;
            }
        }
    }
}
