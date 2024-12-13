// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Drawing.Interop;

namespace System.Drawing;

/// <summary>
/// Defines a particular format for text, including font face, size, and style attributes.
/// </summary>
[Editor($"System.Drawing.Design.FontEditor, {AssemblyRef.SystemDrawingDesign}",
        $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
[TypeConverter(typeof(FontConverter))]
[Serializable]
[Runtime.CompilerServices.TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public sealed unsafe class Font : MarshalByRefObject, ICloneable, IDisposable, ISerializable
{
    [NonSerialized]
    private GpFont* _nativeFont;
    private float _fontSize;
    private FontStyle _fontStyle;
    private FontFamily _fontFamily = null!;
    private GraphicsUnit _fontUnit;
    private byte _gdiCharSet = (byte)FONT_CHARSET.DEFAULT_CHARSET;
    private bool _gdiVerticalFont;
    private string _systemFontName = string.Empty;
    private string? _originalFontName;

    // Return value is in Unit (the unit the font was created in)
    /// <summary>
    /// Gets the size of this <see cref='Font'/>.
    /// </summary>
    public float Size => _fontSize;

    /// <summary>
    /// Gets style information for this <see cref='Font'/>.
    /// </summary>
    [Browsable(false)]
    public FontStyle Style => _fontStyle;

    /// <summary>
    /// Gets a value indicating whether this <see cref='Font'/> is bold.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Bold => (Style & FontStyle.Bold) != 0;

    /// <summary>
    /// Gets a value indicating whether this <see cref='Font'/> is Italic.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Italic => (Style & FontStyle.Italic) != 0;

    /// <summary>
    /// Gets a value indicating whether this <see cref='Font'/> is strikeout (has a line through it).
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Strikeout => (Style & FontStyle.Strikeout) != 0;

    /// <summary>
    /// Gets a value indicating whether this <see cref='Font'/> is underlined.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Underline => (Style & FontStyle.Underline) != 0;

    /// <summary>
    /// Gets the <see cref='Drawing.FontFamily'/> of this <see cref='Font'/>.
    /// </summary>
    [Browsable(false)]
    public FontFamily FontFamily => _fontFamily;

    /// <summary>
    /// Gets the face name of this <see cref='Font'/> .
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Editor($"System.Drawing.Design.FontNameEditor, {AssemblyRef.SystemDrawingDesign}",
            $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
    [TypeConverter(typeof(FontConverter.FontNameConverter))]
    public string Name => FontFamily.Name;

    /// <summary>
    /// Gets the unit of measure for this <see cref='Font'/>.
    /// </summary>
    [TypeConverter(typeof(FontConverter.FontUnitConverter))]
    public GraphicsUnit Unit => _fontUnit;

    /// <summary>
    /// Returns the GDI char set for this instance of a font. This will only
    /// be valid if this font was created from a classic GDI font definition,
    /// like a LOGFONT or HFONT, or it was passed into the constructor.
    ///
    /// This is here for compatibility with native Win32 intrinsic controls
    /// on non-Unicode platforms.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public byte GdiCharSet => _gdiCharSet;

    /// <summary>
    /// Determines if this font was created to represent a GDI vertical font. This will only be valid if this font
    /// was created from a classic GDIfont definition, like a LOGFONT or HFONT, or it was passed into the constructor.
    ///
    /// This is here for compatibility with native Win32 intrinsic controls on non-Unicode platforms.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool GdiVerticalFont => _gdiVerticalFont;

    /// <summary>
    /// This property is required by the framework and not intended to be used directly.
    /// </summary>
    [Browsable(false)]
    public string? OriginalFontName => _originalFontName;

    /// <summary>
    /// Gets the name of this <see cref='Font'/>.
    /// </summary>
    [Browsable(false)]
    public string SystemFontName => _systemFontName;

    /// <summary>
    /// Returns true if this <see cref='Font'/> is a SystemFont.
    /// </summary>
    [Browsable(false)]
    public bool IsSystemFont => !string.IsNullOrEmpty(_systemFontName);

    /// <summary>
    /// Gets the height of this <see cref='Font'/>.
    /// </summary>
    [Browsable(false)]
    public int Height => (int)Math.Ceiling(GetHeight());

    /// <summary>
    ///  Get native GDI+ object pointer. This property triggers the creation of the GDI+ native object if not initialized yet.
    /// </summary>
    internal GpFont* NativeFont => _nativeFont;

    /// <summary>
    /// Cleans up Windows resources for this <see cref='Font'/>.
    /// </summary>
    ~Font() => Dispose(false);

    private Font(SerializationInfo info, StreamingContext context)
    {
        string name = info.GetString("Name")!; // Do not rename (binary serialization)
        FontStyle style = (FontStyle)info.GetValue("Style", typeof(FontStyle))!; // Do not rename (binary serialization)
        GraphicsUnit unit = (GraphicsUnit)info.GetValue("Unit", typeof(GraphicsUnit))!; // Do not rename (binary serialization)
        float size = info.GetSingle("Size"); // Do not rename (binary serialization)

        Initialize(name, size, style, unit, (byte)FONT_CHARSET.DEFAULT_CHARSET, IsVerticalName(name));
    }

    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
    {
        string name = string.IsNullOrEmpty(OriginalFontName) ? Name : OriginalFontName;
        si.AddValue("Name", name); // Do not rename (binary serialization)
        si.AddValue("Size", Size); // Do not rename (binary serialization)
        si.AddValue("Style", Style); // Do not rename (binary serialization)
        si.AddValue("Unit", Unit); // Do not rename (binary serialization)
    }

    private static bool IsVerticalName(string familyName) => familyName?.Length > 0 && familyName[0] == '@';

    /// <summary>
    /// Cleans up Windows resources for this <see cref='Font'/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_nativeFont is not null)
        {
            try
            {
#if DEBUG
                Status status = !Gdip.Initialized ? Status.Ok :
#endif
                PInvokeGdiPlus.GdipDeleteFont(_nativeFont);
#if DEBUG
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
#endif
            }
            catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
            {
            }
            finally
            {
                _nativeFont = null;
            }
        }
    }

    /// <summary>
    ///  Returns the height of this Font in the specified graphics context.
    /// </summary>
    public float GetHeight(Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        if (graphics.NativeGraphics is null)
        {
            throw new ArgumentException(message: null, nameof(graphics));
        }

        float height;
        PInvokeGdiPlus.GdipGetFontHeight(NativeFont, graphics.Pointer(), &height).ThrowIfFailed();
        GC.KeepAlive(this);
        GC.KeepAlive(graphics);
        return height;
    }

    public float GetHeight(float dpi)
    {
        float height;
        PInvokeGdiPlus.GdipGetFontHeightGivenDPI(NativeFont, dpi, &height).ThrowIfFailed();
        GC.KeepAlive(this);
        return height;
    }

    /// <summary>
    /// Returns a value indicating whether the specified object is a <see cref='Font'/> equivalent to this
    /// <see cref='Font'/>.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == this)
        {
            return true;
        }

        if (obj is not Font font)
        {
            return false;
        }

        // Note: If this and/or the passed-in font are disposed, this method can still return true since we check for cached properties
        // here.
        // We need to call properties on the passed-in object since it could be a proxy in a remoting scenario and proxies don't
        // have access to private/internal fields.
        return font.FontFamily.Equals(FontFamily)
            && font.GdiVerticalFont == GdiVerticalFont
            && font.GdiCharSet == GdiCharSet
            && font.Style == Style
            && font.Size == Size
            && font.Unit == Unit;
    }

    /// <summary>
    ///  Gets the hash code for this <see cref='Font'/>.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Name, Style, Size, Unit);

    /// <summary>
    ///  Returns a human-readable string representation of this <see cref='Font'/>.
    /// </summary>
    public override string ToString() =>
        $"[{GetType().Name}: Name={FontFamily.Name}, Size={_fontSize}, Units={(int)_fontUnit}, GdiCharSet={_gdiCharSet}, GdiVerticalFont={_gdiVerticalFont}]";

    // This is used by SystemFonts when constructing a system Font objects.
    internal void SetSystemFontName(string systemFontName) => _systemFontName = systemFontName;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToLogFont(object logFont, Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(logFont);

        Type type = logFont.GetType();
        int nativeSize = sizeof(LOGFONT);

        // Marshal is necessary here. ToLogFont(LogFont, Graphics) is the marshal free version.

#pragma warning disable CA1421 // This method uses runtime marshalling even when the 'DisableRuntimeMarshallingAttribute' is applied
        if (Marshal.SizeOf(type) != nativeSize)
        {
            // If we don't actually have an object that is LOGFONT in size, trying to pass
            // it to GDI+ is likely to cause an AV.
            throw new ArgumentException(null, nameof(logFont));
        }

        ToLogFont(out LOGFONT nativeLogFont, graphics);

        // PtrToStructure requires that the passed in object not be a value type.
        if (!type.IsValueType)
        {
            Marshal.PtrToStructure(new IntPtr(&nativeLogFont), logFont);
        }
        else
        {
            GCHandle handle = GCHandle.Alloc(logFont, GCHandleType.Pinned);
            Buffer.MemoryCopy(&nativeLogFont, (byte*)handle.AddrOfPinnedObject(), nativeSize, nativeSize);
            handle.Free();
        }
#pragma warning restore CA1421
    }

#if NET8_0_OR_GREATER
    public
#else
    private
#endif
    void ToLogFont(out LOGFONT logFont, Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        fixed (LOGFONT* lf = &logFont)
        {
            PInvokeGdiPlus.GdipGetLogFont(NativeFont, graphics.Pointer(), (LOGFONTW*)lf).ThrowIfFailed();
            GC.KeepAlive(this);
            GC.KeepAlive(graphics);
        }

        // Prefix the string with '@' if this is a gdiVerticalFont.
        if (_gdiVerticalFont)
        {
            Span<char> faceName = logFont.lfFaceName;
            faceName[..^1].CopyTo(faceName[1..]);
            faceName[0] = '@';

            // Docs require this to be null terminated
            faceName[^1] = '\0';
        }

        if (logFont.lfCharSet == 0)
        {
            logFont.lfCharSet = _gdiCharSet;
        }
    }

    /// <summary>
    ///  Creates the GDI+ native font object.
    /// </summary>
    private void CreateNativeFont()
    {
        Debug.Assert(_nativeFont is null, "nativeFont already initialized, this will generate a handle leak.");
        Debug.Assert(_fontFamily is not null, "fontFamily not initialized.");

        // Note: GDI+ creates singleton font family objects (from the corresponding font file) and reference count them so
        // if creating the font object from an external FontFamily, this object's FontFamily will share the same native object.

        GpFont* font;
        Status status = PInvokeGdiPlus.GdipCreateFont(_fontFamily.Pointer(), _fontSize, (int)_fontStyle, (Unit)_fontUnit, &font);
        GC.KeepAlive(this);
        _nativeFont = font;

        // Special case this common error message to give more information
        if (status == Status.FontStyleNotFound)
        {
            throw new ArgumentException(SR.Format(SR.GdiplusFontStyleNotFound, _fontFamily.Name, _fontStyle.ToString()));
        }
        else if (status != Status.Ok)
        {
            throw status.GetException();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class from the specified existing <see cref='Font'/>
    /// and <see cref='FontStyle'/>.
    /// </summary>
    public Font(Font prototype, FontStyle newStyle)
    {
        // Copy over the originalFontName because it won't get initialized
        _originalFontName = prototype.OriginalFontName;
        Initialize(prototype.FontFamily, prototype.Size, newStyle, prototype.Unit, (byte)FONT_CHARSET.DEFAULT_CHARSET, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit)
    {
        Initialize(family, emSize, style, unit, (byte)FONT_CHARSET.DEFAULT_CHARSET, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
    {
        Initialize(family, emSize, style, unit, gdiCharSet, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
    {
        Initialize(family, emSize, style, unit, gdiCharSet, gdiVerticalFont);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet)
    {
        Initialize(familyName, emSize, style, unit, gdiCharSet, IsVerticalName(familyName));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
    {
        if (float.IsNaN(emSize) || float.IsInfinity(emSize) || emSize <= 0)
        {
            throw new ArgumentException(SR.Format(SR.InvalidBoundArgument, nameof(emSize), emSize, 0, "System.Single.MaxValue"), nameof(emSize));
        }

        Initialize(familyName, emSize, style, unit, gdiCharSet, gdiVerticalFont);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(FontFamily family, float emSize, FontStyle style)
    {
        Initialize(family, emSize, style, GraphicsUnit.Point, (byte)FONT_CHARSET.DEFAULT_CHARSET, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(FontFamily family, float emSize, GraphicsUnit unit)
    {
        Initialize(family, emSize, FontStyle.Regular, unit, (byte)FONT_CHARSET.DEFAULT_CHARSET, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(FontFamily family, float emSize)
    {
        Initialize(family, emSize, FontStyle.Regular, GraphicsUnit.Point, (byte)FONT_CHARSET.DEFAULT_CHARSET, false);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit)
    {
        Initialize(familyName, emSize, style, unit, (byte)FONT_CHARSET.DEFAULT_CHARSET, IsVerticalName(familyName));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(string familyName, float emSize, FontStyle style)
    {
        Initialize(familyName, emSize, style, GraphicsUnit.Point, (byte)FONT_CHARSET.DEFAULT_CHARSET, IsVerticalName(familyName));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(string familyName, float emSize, GraphicsUnit unit)
    {
        Initialize(familyName, emSize, FontStyle.Regular, unit, (byte)FONT_CHARSET.DEFAULT_CHARSET, IsVerticalName(familyName));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref='Font'/> class with the specified attributes.
    /// </summary>
    public Font(string familyName, float emSize)
    {
        Initialize(familyName, emSize, FontStyle.Regular, GraphicsUnit.Point, (byte)FONT_CHARSET.DEFAULT_CHARSET, IsVerticalName(familyName));
    }

    /// <summary>
    /// Constructor to initialize fields from an existing native GDI+ object reference. Used by ToLogFont.
    /// </summary>
    private Font(GpFont* nativeFont, byte gdiCharSet, bool gdiVerticalFont)
    {
        Debug.Assert(_nativeFont is null, "GDI+ native font already initialized, this will generate a handle leak");
        Debug.Assert(nativeFont is not null, "nativeFont is null");

        _nativeFont = nativeFont;
        GraphicsUnit unit;
        float size;
        FontStyle style;
        GpFontFamily* family;
        PInvokeGdiPlus.GdipGetFontUnit(_nativeFont, (Unit*)&unit).ThrowIfFailed();
        PInvokeGdiPlus.GdipGetFontSize(_nativeFont, &size).ThrowIfFailed();
        PInvokeGdiPlus.GdipGetFontStyle(_nativeFont, (int*)&style).ThrowIfFailed();
        PInvokeGdiPlus.GdipGetFamily(_nativeFont, &family).ThrowIfFailed();

        // Fonts from native HFONTs are always from the installed font collection.
        SetFontFamily(new FontFamily(family, fromInstalledFontCollection: true));
        Initialize(_fontFamily, size, style, unit, gdiCharSet, gdiVerticalFont);
    }

    private void Initialize(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
    {
        _originalFontName = familyName;

        ReadOnlySpan<char> name = familyName;

        // Strip the vertical tag ('@') if present.
        if (name.Length > 1 && name[0] == '@')
        {
            name = name[1..];
        }

        SetFontFamily(new FontFamily(name, createDefaultOnFail: true));
        Initialize(_fontFamily, emSize, style, unit, gdiCharSet, gdiVerticalFont);
    }

    private void Initialize(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont)
    {
        ArgumentNullException.ThrowIfNull(family);

        if (float.IsNaN(emSize) || float.IsInfinity(emSize) || emSize <= 0)
        {
            throw new ArgumentException(SR.Format(SR.InvalidBoundArgument, nameof(emSize), emSize, 0, "System.Single.MaxValue"), nameof(emSize));
        }

        Status status;

        _fontSize = emSize;
        _fontStyle = style;
        _fontUnit = unit;
        _gdiCharSet = gdiCharSet;
        _gdiVerticalFont = gdiVerticalFont;

        if (_fontFamily is null)
        {
            SetFontFamily(family.Clone());
        }

        if (_nativeFont is null)
        {
            CreateNativeFont();
        }

        // Get actual size.
        float size;
        status = PInvokeGdiPlus.GdipGetFontSize(_nativeFont, &size);
        _fontSize = size;
        GC.KeepAlive(this);
        Gdip.CheckStatus(status);
    }

    /// <summary>
    /// Creates a <see cref='Font'/> from the specified Windows handle.
    /// </summary>
    public static Font FromHfont(IntPtr hfont)
    {
        PInvokeCore.GetObject((HGDIOBJ)hfont, out LOGFONT logFont);

        using var hdc = GetDcScope.ScreenDC;
        return FromLogFont(in logFont, hdc);
    }

    /// <summary>
    /// Creates a <see cref="Font"/> from the given LOGFONT using the screen device context.
    /// </summary>
    /// <param name="lf">A boxed LOGFONT.</param>
    /// <returns>The newly created <see cref="Font"/>.</returns>
    public static Font FromLogFont(object lf)
    {
        using var hdc = GetDcScope.ScreenDC;
        return FromLogFont(lf, hdc);
    }

#if NET8_0_OR_GREATER
    public
#else
    internal
#endif
    static Font FromLogFont(in LOGFONT logFont)
    {
        using var hdc = GetDcScope.ScreenDC;
        return FromLogFont(logFont, hdc);
    }

#if NET8_0_OR_GREATER
    public
#else
    internal
#endif
    static Font FromLogFont(in LOGFONT logFont, IntPtr hdc)
    {
        Status status;
        GpFont* font;
        fixed (LOGFONT* lf = &logFont)
        {
            status = PInvokeGdiPlus.GdipCreateFontFromLogfont((HDC)hdc, (LOGFONTW*)lf, &font);
        }

        // Special case this incredibly common error message to give more information
        if (status == Status.NotTrueTypeFont)
        {
            throw new ArgumentException(SR.GdiplusNotTrueTypeFont_NoName);
        }
        else if (status != Status.Ok)
        {
            throw Gdip.StatusException(status);
        }

        // GDI+ returns font = 0 even though the status is Ok.
        if (font is null)
        {
            throw new ArgumentException(SR.Format(SR.GdiplusNotTrueTypeFont, logFont.AsString()));
        }

        return new Font(font, logFont.lfCharSet, logFont.IsGdiVerticalFont);
    }

    /// <summary>
    ///  Creates a <see cref="Font"/> from the given LOGFONT using the given device context.
    /// </summary>
    /// <param name="lf">A boxed LOGFONT.</param>
    /// <param name="hdc">Handle to a device context (HDC).</param>
    /// <returns>The newly created <see cref="Font"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Font FromLogFont(object lf, IntPtr hdc)
    {
        ArgumentNullException.ThrowIfNull(lf);

        if (lf is LOGFONT logFont)
        {
            // A boxed LOGFONT, just use it to create the font
            return FromLogFont(in logFont, hdc);
        }

        Type type = lf.GetType();
        int nativeSize = sizeof(LOGFONT);

        // Marshal is necessary here. FromLogFont(LogFont, IntPtr) is the marshal free version.
#pragma warning disable CA1421 // This method uses runtime marshalling even when the 'DisableRuntimeMarshallingAttribute' is applied
        if (Marshal.SizeOf(type) != nativeSize)
        {
            // If we don't actually have an object that is LOGFONT in size, trying to pass
            // it to GDI+ is likely to cause an AV.
            throw new ArgumentException(null, nameof(lf));
        }

        // Now that we know the marshalled size is the same as LOGFONT, copy in the data
        logFont = default;

        Marshal.StructureToPtr(lf, new IntPtr(&logFont), fDeleteOld: false);
#pragma warning restore CA1421

        return FromLogFont(in logFont, hdc);
    }

    /// <summary>
    /// Creates a <see cref="Font"/> from the specified handle to a device context (HDC).
    /// </summary>
    /// <returns>The newly created <see cref="Font"/>.</returns>
    public static Font FromHdc(IntPtr hdc)
    {
        GpFont* font;
        Status status = PInvokeGdiPlus.GdipCreateFontFromDC((HDC)hdc, &font);

        // Special case this incredibly common error message to give more information
        if (status == Status.NotTrueTypeFont)
        {
            throw new ArgumentException(SR.GdiplusNotTrueTypeFont_NoName);
        }
        else if (status != Status.Ok)
        {
            throw Gdip.StatusException(status);
        }

        return new Font(font, 0, gdiVerticalFont: false);
    }

    /// <summary>
    ///  Creates an exact copy of this <see cref='Font'/>.
    /// </summary>
    public object Clone()
    {
        GpFont* font;
        PInvokeGdiPlus.GdipCloneFont(_nativeFont, &font).ThrowIfFailed();
        GC.KeepAlive(this);
        return new Font(font, _gdiCharSet, _gdiVerticalFont);
    }

    private void SetFontFamily(FontFamily family)
    {
        _fontFamily = family;

        // GDI+ creates ref-counted singleton FontFamily objects based on the family name so all managed
        // objects with same family name share the underlying GDI+ native pointer. The unmanaged object is
        // destroyed when its ref-count gets to zero.
        //
        // Make sure _fontFamily is not finalized so the underlying singleton object is kept alive.
        GC.SuppressFinalize(_fontFamily);
    }

    public void ToLogFont(object logFont)
    {
        using var hdc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdcInternal(hdc);
        ToLogFont(logFont, graphics);
    }

#if NET8_0_OR_GREATER
    public void ToLogFont(out LOGFONT logFont)
    {
        using var hdc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdcInternal(hdc);
        ToLogFont(out logFont, graphics);
    }
#endif

    /// <summary>
    ///  Returns a handle to this <see cref='Font'/>.
    /// </summary>
    public IntPtr ToHfont()
    {
        using var hdc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdcInternal(hdc);
        ToLogFont(out LOGFONT lf, graphics);
        HFONT handle = PInvokeCore.CreateFontIndirect((LOGFONTW*)&lf);
        return handle.IsNull ? throw new Win32Exception() : handle;
    }

    public float GetHeight()
    {
        using var hdc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdcInternal(hdc);
        return GetHeight(graphics);
    }

    /// <summary>
    /// Gets the size, in points, of this <see cref='Font'/>.
    /// </summary>
    [Browsable(false)]
    public float SizeInPoints
    {
        get
        {
            if (Unit == GraphicsUnit.Point)
            {
                return Size;
            }

            using var hdc = GetDcScope.ScreenDC;
            using Graphics graphics = Graphics.FromHdcInternal(hdc);

            float pixelsPerPoint = (float)(graphics.DpiY / 72.0);
            float lineSpacingInPixels = GetHeight(graphics);
            float emHeightInPixels = lineSpacingInPixels * FontFamily.GetEmHeight(Style) / FontFamily.GetLineSpacing(Style);

            return emHeightInPixels / pixelsPerPoint;
        }
    }
}
