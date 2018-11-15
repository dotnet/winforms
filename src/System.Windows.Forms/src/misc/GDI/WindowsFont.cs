// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINFORMS_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    using System;
    using System.Internal;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Security;
    using System.Security.Permissions;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <devdoc>
    ///     <para>
    ///         Encapsulates a GDI Font object.
    ///     </para>
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    sealed partial class WindowsFont : MarshalByRefObject, ICloneable, IDisposable
    {
        const int LogFontNameOffset = 28;
        
        // Handle to the native Windows font object.
        // 
        private IntPtr hFont;
        private float  fontSize = -1.0f; //invalid value.
        private int    lineSpacing;
        private bool   ownHandle;
        private bool   ownedByCacheManager;
        private bool   everOwnedByCacheManager;

        private IntNativeMethods.LOGFONT logFont; 
        private FontStyle style;

        // Note: These defaults are according to the ones in GDI+ but those are not necessarily the same as the system
        // default font.  The GetSystemDefaultHFont() method should be used if needed.
        private const string defaultFaceName   = "Microsoft Sans Serif";
        private const float  defaultFontSize   = 8.25f;
        private const int    defaultFontHeight = 13;

#if GDI_FINALIZATION_WATCH
       private string AllocationSite = DbgUtil.StackTrace;
#endif

        /// <devdoc>
        ///     Creates the font handle.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        private void CreateFont()
        {
            Debug.Assert( hFont == IntPtr.Zero, "hFont is not null, this will generate a handle leak." );
            Debug.Assert( this.logFont != null, "WindowsFont.logFont not initialized." );

            this.hFont = IntUnsafeNativeMethods.CreateFontIndirect(this.logFont);

#if TRACK_HFONT
            Debug.WriteLine( DbgUtil.StackTraceToStr(String.Format( "HFONT[0x{0:x8}] = CreateFontIndirect( LOGFONT={1} )", (int) this.hFont, this.logFont)));
#endif
            if (this.hFont == IntPtr.Zero)
            {
                this.logFont.lfFaceName     = defaultFaceName;
                this.logFont.lfOutPrecision = IntNativeMethods.OUT_TT_ONLY_PRECIS; // True Type only.

                this.hFont = IntUnsafeNativeMethods.CreateFontIndirect(this.logFont);

#if TRACK_HFONT
            Debug.WriteLine( DbgUtil.StackTraceToStr(String.Format( "HFONT[0x{0:x8}] = CreateFontIndirect( LOGFONT={1} )", (int) this.hFont, this.logFont)));
#endif

            }

            // Update logFont height and other adjusted parameters.
            //
            IntUnsafeNativeMethods.GetObject(new HandleRef(this, this.hFont), this.logFont);

            // We created the hFont, we will delete it on dispose.
            this.ownHandle = true;
        }

        /// Constructors.

        /// <devdoc>
        ///     Contructor to construct font from a face name.
        /// </devdoc>>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsFont( string faceName ) :
            this(faceName, defaultFontSize, FontStyle.Regular, IntNativeMethods.DEFAULT_CHARSET, WindowsFontQuality.Default)
        {
         // Default size in WinForms is 8.25f.  
        }

        /// <devdoc>
        ///     Contructor to construct font from a face name, a desired size and with the specified style.
        /// </devdoc>>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsFont( string faceName, float size ) :
            this(faceName, size, FontStyle.Regular, IntNativeMethods.DEFAULT_CHARSET, WindowsFontQuality.Default)
        {
        }

        /// <devdoc>
        ///     Contructor to construct font from a face name, a desired size and with the specified style.
        /// </devdoc>>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsFont( string faceName, float size, FontStyle style ) :
            this(faceName, size, style, IntNativeMethods.DEFAULT_CHARSET, WindowsFontQuality.Default)
        {
        }
        
        /// <devdoc>
        ///     Contructor to construct font from a face name, a desired size in points and with the specified style
        ///     and character set. The screen dc is used for calculating the font em height.
        /// </devdoc>>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public WindowsFont( string faceName, float size, FontStyle style, byte charSet, WindowsFontQuality fontQuality )
        {   
            Debug.Assert( size > 0.0f, "size has a negative value." );
            const byte True  = 1;
            const byte False = 0;
            this.logFont = new IntNativeMethods.LOGFONT();

            //
            // Get the font height from the specified size.  size is in point units and height in logical 
            // units (pixels when using MM_TEXT) so we need to make the conversion using the number of 
            // pixels per logical inch along the screen height.
            //
            int pixelsY = (int) Math.Ceiling( WindowsGraphicsCacheManager.MeasurementGraphics.DeviceContext.DpiY * size / 72); // 1 point = 1/72 inch.;

            //
            // The lfHeight represents the font cell height (line spacing) which includes the internal 
            // leading; we specify a negative size value (in pixels) for the height so the font mapper 
            // provides the closest match for the character height rather than the cell height (MSDN).
            //
            this.logFont.lfHeight       = -pixelsY;
            this.logFont.lfFaceName     = faceName != null ? faceName : defaultFaceName;
            this.logFont.lfCharSet      = charSet;
            this.logFont.lfOutPrecision = IntNativeMethods.OUT_TT_PRECIS;
            this.logFont.lfQuality      = (byte) fontQuality;
            this.logFont.lfWeight       = (style & FontStyle.Bold)      == FontStyle.Bold      ? IntNativeMethods.FW_BOLD : IntNativeMethods.FW_NORMAL;
            this.logFont.lfItalic       = (style & FontStyle.Italic)    == FontStyle.Italic    ? True : False;
            this.logFont.lfUnderline    = (style & FontStyle.Underline) == FontStyle.Underline ? True : False;
            this.logFont.lfStrikeOut    = (style & FontStyle.Strikeout) == FontStyle.Strikeout ? True : False;

            // Let the Size be recomputed to be consistent with the Height (there may be some precision loss coming from size to height).
            // this.fontSize = size;
            this.style    = style;

            CreateFont();
        }

        /// <devdoc>
        ///     Contructor to construct font from a LOGFONT structure.
        ///     Pass false in the createHandle param to create a 'compatible' font (handle-less, to be used for measuring/comparing) or
        ///     when the handle has already been created.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        private WindowsFont( IntNativeMethods.LOGFONT lf, bool createHandle )
        {
            Debug.Assert( lf != null, "lf is null" );

            this.logFont = lf;

            if (this.logFont.lfFaceName == null)
            {
                this.logFont.lfFaceName = defaultFaceName;
            }

            this.style = FontStyle.Regular;
            if (lf.lfWeight == IntNativeMethods.FW_BOLD)
            {
                this.style |= FontStyle.Bold;
            }
            if (lf.lfItalic == 1)
            {
                this.style |= FontStyle.Italic;
            }
            if (lf.lfUnderline == 1)
            {
                this.style |= FontStyle.Underline;
            }
            if (lf.lfStrikeOut == 1)
            {
                this.style |= FontStyle.Strikeout;
            }

            if( createHandle )
            {
                CreateFont();
            }
        }

        /// <devdoc>
        ///     Contructs a WindowsFont object from an existing System.Drawing.Font object (GDI+), based on the screen dc MapMode
        ///     and resolution (normally: MM_TEXT and 96 dpi).
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsFont FromFont(Font font)
        {
            return FromFont(font, WindowsFontQuality.Default);
        }
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
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

        
        /// <devdoc>
        ///     Creates a WindowsFont from the font selected in the supplied dc.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsFont FromHdc( IntPtr hdc )
        {
            IntPtr hFont = IntUnsafeNativeMethods.GetCurrentObject(new HandleRef(null, hdc), IntNativeMethods.OBJ_FONT);

            // don't call DeleteObject on handle from GetCurrentObject, it is the one selected in the hdc.

            return FromHfont( hFont );
        }

        /// <devdoc>
        ///     Creates a WindowsFont from the handle to a native GDI font.  It does not take ownership of the 
        ///     passed-in handle, the caller needs to delete the hFont when done with the WindowsFont.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsFont FromHfont( IntPtr hFont )
        {
            return FromHfont( hFont, false );
        }

        /// <devdoc>
        ///     Creates a WindowsFont from the handle to a native GDI font and optionally takes ownership of managing
        ///     the lifetime of the handle. 
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public static WindowsFont FromHfont( IntPtr hFont, bool takeOwnership )
        {
            IntNativeMethods.LOGFONT lf = new IntNativeMethods.LOGFONT();
            IntUnsafeNativeMethods.GetObject(new HandleRef(null, hFont), lf);
            
            WindowsFont wf = new WindowsFont( lf, /*createHandle*/ false );
            wf.hFont = hFont;
            wf.ownHandle = takeOwnership; // if true, hFont will be deleted on dispose.

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
            if (this.ownHandle)
            {
                if (!ownedByCacheManager || !disposing) {

                    // If we were ever owned by the CacheManger and we're being disposed
                    // we can be sure that we're not in use by any DC's (otherwise Dispose() wouldn't have been called)                    
                    // skip the check IsFontInUse check in this case.
                    // Also skip the check if disposing == false, because the cache is thread-static
                    // and that means we're being called from the finalizer.
                    if (everOwnedByCacheManager || !disposing || !DeviceContexts.IsFontInUse(this)) {
                        Debug.Assert( this.hFont != IntPtr.Zero, "Unexpected null hFont." );
                        DbgUtil.AssertFinalization(this, disposing);
                        
                        IntUnsafeNativeMethods.DeleteObject(new HandleRef(this, this.hFont));
#if TRACK_HFONT
                        Debug.WriteLine( DbgUtil.StackTraceToStr(String.Format( "DeleteObject(HFONT[0x{0:x8}]))", (int) this.hFont)));
#endif
                        this.hFont = IntPtr.Zero;
                        this.ownHandle = false;
                        deletedHandle = true;
                    }
                }
            }

            if (disposing && (deletedHandle || !ownHandle))
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <devdoc>
        ///    Returns a value indicating whether the specified object is a WindowsFont equivalent to this object.
        /// </devdoc>
        public override bool Equals( object font )
        {
            WindowsFont winFont = font as WindowsFont;

            if( winFont == null )
            {
                return false;
            }

            if( winFont == this )
            {
                return true;
            }

            // WARNING: don't use non-public fields/properties here, the passed-in font object could be a proxy in a
            //          remoting scenario and proxies cannot access internal or private members.

            // Compare params used to create the font.
            return  this.Name           == winFont.Name            &&
                    this.LogFontHeight  == winFont.LogFontHeight   && // Equivalent to comparing Size but always at hand.
                    this.Style          == winFont.Style           &&
                    this.CharSet        == winFont.CharSet         &&
                    this.Quality        == winFont.Quality;
        }

        /// <devdoc>
        ///    Gets the hash code for this WindowsFont.
        /// </devdoc>
        public override int GetHashCode() 
        {
            // similar to Font.GetHashCode().
            return (int)((((UInt32)this.Style   << 13) | ((UInt32)this.Style   >> 19)) ^
                         (((UInt32)this.CharSet << 26) | ((UInt32)this.CharSet >>  6)) ^
                         (((UInt32)this.Size    <<  7) | ((UInt32)this.Size    >> 25)));
        }

        /// <devdoc>
        ///     Clones this object.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public object Clone()
        {
            return new WindowsFont( this.logFont, true );
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[{0}: Name={1}, Size={2} points, Height={3} pixels, Sytle={4}]", GetType().Name,  logFont.lfFaceName, this.Size, this.Height, this.Style); 
        }

        ////////////////////////////////////////////
        ///  Properties
        
        /// <devdoc>
        ///       Returns this object's native Win32 font handle.  Should NOT be deleted externally.
        ///       Compare with ToHfont method.
        /// </devdoc>
        public IntPtr Hfont
        { 
            get
            {
                //Assert removed. We need to be able to check for Hfont == IntPtr.Zero to determine if the object was disposed.
                //Debug.Assert(this.hFont != IntPtr.Zero, "hFont is null, are you using a disposed object?");
                return this.hFont;
            }
        }

        /// <devdoc>
        ///     Determines whether the font has the italic style or not.
        /// </devdoc>
        public bool Italic
        {
            get
            {
                return logFont.lfItalic == 1;
            }
        }

        public bool OwnedByCacheManager
        {
            get {
                return ownedByCacheManager;
            }
            set {
                if (value) {
                    everOwnedByCacheManager = true;
                }
                ownedByCacheManager = value;
            }
        }

        /// <devdoc>
        ///     Rendering quality.
        /// </devdoc>
        public WindowsFontQuality Quality
        {
            get
            {
                return (WindowsFontQuality) this.logFont.lfQuality;
            }
        }

        /// <devdoc>
        ///     Gets the font style.
        /// </devdoc>
        public FontStyle Style
        {
            get
            {
                return this.style;
            }
        }

        /// <devdoc>
        ///     Gets the line spacing (cell height) of this font in (screen) pixels using the screen resolution.
        ///     Gets the line spacing (cell height), in pixels (using the screen DC resolution), of this font. 
        ///     The line spacing is the vertical distance between the base lines of two consecutive lines of text. 
        ///     Thus, the line spacing includes the blank space between lines along with the height of the character 
        ///     itself.
        /// </devdoc>
        public int Height
        {
            // 

            get
            {
                if( this.lineSpacing == 0 )
                {
                    // Observe that the font text metrics are obtained using the resolution of the screen.
                    WindowsGraphics wg  = WindowsGraphicsCacheManager.MeasurementGraphics;

                    // No need to reset the font (if changed) since we always set the font before using the MeasurementGraphics
                    // in WindowsGraphics methods.
                    wg.DeviceContext.SelectFont(this);

                    IntNativeMethods.TEXTMETRIC tm = (IntNativeMethods.TEXTMETRIC) wg.GetTextMetrics();
                    this.lineSpacing = tm.tmHeight;
                }

                return this.lineSpacing;
            }
        }

        /// <devdoc>
        ///     Gets the font character set.  
        ///     This is used by the system font mapper when searching for the physical font that best matches the logical font.
        /// </devdoc>
        public byte CharSet
        {
            get
            {
                return logFont.lfCharSet;
            }
        }

        /// <devdoc>
        ///     Specifies the height, in logical units, of the font's character cell or character. The character height value (em height)
        ///     is the character cell height value minus the internal-leading value. 
        /// </devdoc>
        public int LogFontHeight
        {
            get
            {
                return logFont.lfHeight;
            }
        }

        /// <devdoc>
        ///     The font's face name.
        /// </devdoc>
        public string Name
        {
            get
            {
                return logFont.lfFaceName;
            }
        }

        /// <devdoc>
        ///     Gets the character height (as opposed to the cell height) of the font represented by this object in points.
        ///     Consider
        /// </devdoc>
        public float Size 
        {
            get
            {
                if( this.fontSize < 0.0f )
                {
                    WindowsGraphics wg  = WindowsGraphicsCacheManager.MeasurementGraphics;

                    // No need to reset the font (if changed) since we always set the font before using the MeasurementGraphics
                    // in WindowsGraphics methods.
                    wg.DeviceContext.SelectFont(this);

                    IntNativeMethods.TEXTMETRIC tm = (IntNativeMethods.TEXTMETRIC) wg.GetTextMetrics();

                    //
                    // Convert the font character height to points.  If lfHeight is negative, Windows 
                    // treats the absolute value of that number as a desired font height compatible with 
                    // the point size; in this case lfHeight will roughly match the tmHeight field of 
                    // the TEXTMETRIC structure less the tmInternalLeading field. 
                    //
                    int height = this.logFont.lfHeight > 0 ? tm.tmHeight : (tm.tmHeight - tm.tmInternalLeading);

                    // 
                    /*
                    switch (this.unit)
                    {
                        case GraphicsUnit.Pixel:       worldEmSize = height * dpi / 72.0f;   break;
                        case GraphicsUnit.Point:       worldEmSize = height * dpi / 72.0f;   break;
                        case GraphicsUnit.Inch:        worldEmSize = height * dpi;           break;
                        case GraphicsUnit.Document:    worldEmSize = height * dpi / 300.0f;  break;
                        case GraphicsUnit.Millimeter:  worldEmSize = height * dpi / 25.4f;   break;
                    }
                    */
                
                    this.fontSize = height * 72f / wg.DeviceContext.DpiY;
                }
                
                return this.fontSize;
            }
        }

        /// <devdoc>
        ///     Attempts to match the TextRenderingHint of the specified Graphics object with a LOGFONT.lfQuality value.
        /// </devdoc>
        public static WindowsFontQuality WindowsFontQualityFromTextRenderingHint(Graphics g)
        {
            if (g == null)
            {
                return WindowsFontQuality.Default;
            }

            switch (g.TextRenderingHint)
            {
                case TextRenderingHint.ClearTypeGridFit:
                    // See WindowsFontQuality enum for the flags supported in the different OS systems.
                    if (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1)
                    {
                        return WindowsFontQuality.ClearTypeNatural;
                    }
                    else
                    {
                        return WindowsFontQuality.ClearType;
                    }

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
