// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Text;
using System.Globalization;

namespace System.Drawing;

/// <summary>
///  Abstracts a group of type faces having a similar basic design but having certain variation in styles.
/// </summary>
public unsafe sealed class FontFamily : MarshalByRefObject, IDisposable
{
    private const int NeutralLanguage = 0;
    private GpFontFamily* _nativeFamily;
    private readonly bool _createDefaultOnFail;

#if DEBUG
    private static readonly object s_lockObj = new();
    private static int s_idCount;
    private int _id;
#endif

    private void SetNativeFamily(GpFontFamily* family)
    {
        Debug.Assert(_nativeFamily is null, "Setting GDI+ native font family when already initialized.");

        _nativeFamily = family;
#if DEBUG
        lock (s_lockObj)
        {
            _id = ++s_idCount;
        }
#endif
    }

    internal FontFamily(GpFontFamily* family) => SetNativeFamily(family);

    /// <summary>
    ///  Initializes a new instance of the <see cref='FontFamily'/> class with the specified name.
    /// </summary>
    /// <param name="createDefaultOnFail">
    ///  Determines how errors are handled when creating a font based on a font family that does not exist on the end
    ///  user's system at run time. If this parameter is true, then a fall-back font will always be used instead.
    ///  If this parameter is false, an exception will be thrown.
    /// </param>
    internal FontFamily(string name, bool createDefaultOnFail)
    {
        _createDefaultOnFail = createDefaultOnFail;
        CreateFontFamily(name, null);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='FontFamily'/> class with the specified name.
    /// </summary>
    public FontFamily(string name) => CreateFontFamily(name, null);

    /// <summary>
    ///  Initializes a new instance of the <see cref='FontFamily'/> class in the specified
    ///  <see cref='FontCollection'/> and with the specified name.
    /// </summary>
    public FontFamily(string name, FontCollection? fontCollection) => CreateFontFamily(name, fontCollection);

    // Creates the native font family object.
    // Note: GDI+ creates singleton font family objects (from the corresponding font file) and reference count them.
    private void CreateFontFamily(string name, FontCollection? fontCollection)
    {
        GpFontFamily* fontFamily;
        GpFontCollection* nativeFontCollection = fontCollection is null ? null : fontCollection._nativeFontCollection;

        Status status = Status.Ok;
        fixed (char* n = name)
        {
            status = PInvoke.GdipCreateFontFamilyFromName(n, nativeFontCollection, &fontFamily);
        }

        if (status != Status.Ok)
        {
            if (_createDefaultOnFail)
            {
                fontFamily = GetGdipGenericSansSerif(); // This throws if failed.
            }
            else
            {
                // Special case this incredibly common error message to give more information.
                if (status == Status.FontFamilyNotFound)
                {
                    throw new ArgumentException(SR.Format(SR.GdiplusFontFamilyNotFound, name));
                }
                else if (status == Status.NotTrueTypeFont)
                {
                    throw new ArgumentException(SR.Format(SR.GdiplusNotTrueTypeFont, name));
                }
                else
                {
                    status.ThrowIfFailed();
                }
            }
        }

        GC.KeepAlive(fontCollection);
        SetNativeFamily(fontFamily);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='FontFamily'/> class from the specified generic font family.
    /// </summary>
    public FontFamily(GenericFontFamilies genericFamily)
    {
        GpFontFamily* nativeFamily;

        switch (genericFamily)
        {
            case GenericFontFamilies.Serif:
                PInvoke.GdipGetGenericFontFamilySerif(&nativeFamily).ThrowIfFailed();
                break;
            case GenericFontFamilies.SansSerif:
                PInvoke.GdipGetGenericFontFamilySansSerif(&nativeFamily).ThrowIfFailed();
                break;
            case GenericFontFamilies.Monospace:
            default:
                PInvoke.GdipGetGenericFontFamilyMonospace(&nativeFamily).ThrowIfFailed();
                break;
        }

        SetNativeFamily(nativeFamily);
    }

    ~FontFamily() => Dispose(disposing: false);

    internal GpFontFamily* NativeFamily => _nativeFamily;

    /// <summary>
    /// Converts this <see cref='FontFamily'/> to a human-readable string.
    /// </summary>
    public override string ToString() => $"[{GetType().Name}: Name={Name}]";

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == this)
        {
            return true;
        }

        // if obj = null then (obj is FontFamily) = false.
        if (obj is not FontFamily otherFamily)
        {
            return false;
        }

        // We can safely use the ptr to the native GDI+ FontFamily because in windows it is common to
        // all objects of the same family (singleton RO object).
        return otherFamily.NativeFamily == NativeFamily;
    }

    /// <summary>
    ///  Gets a hash code for this <see cref='FontFamily'/>.
    /// </summary>
    public override int GetHashCode() => GetName(NeutralLanguage).GetHashCode();

    private static int CurrentLanguage => CultureInfo.CurrentUICulture.LCID;

    /// <summary>
    ///  Disposes of this <see cref='FontFamily'/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_nativeFamily is not null)
        {
            try
            {
#if DEBUG
                Status status = !Gdip.Initialized ? Status.Ok :
#endif
                PInvoke.GdipDeleteFontFamily(_nativeFamily);
#if DEBUG
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
#endif
            }
            catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
            {
            }
            finally
            {
                _nativeFamily = null;
            }
        }
    }

    /// <summary>
    ///  Gets the name of this <see cref='FontFamily'/>.
    /// </summary>
    public string Name => GetName(CurrentLanguage);

    /// <summary>
    ///  Returns the name of this <see cref='FontFamily'/> in the specified language.
    /// </summary>
    public unsafe string GetName(int language)
    {
        char* name = stackalloc char[33]; // LF_FACESIZE is 32, leave one extra for null terminator.
        PInvoke.GdipGetFamilyName(NativeFamily, name, (ushort)language).ThrowIfFailed();
        return new(name);
    }

    /// <summary>
    ///  Returns an array that contains all of the <see cref='FontFamily'/> objects associated with the current
    ///  graphics context.
    /// </summary>
    public static FontFamily[] Families => new InstalledFontCollection().Families;

    /// <summary>
    ///  Gets a generic SansSerif <see cref='FontFamily'/>.
    /// </summary>
    public static FontFamily GenericSansSerif => new(GetGdipGenericSansSerif());

    private static GpFontFamily* GetGdipGenericSansSerif()
    {
        GpFontFamily* nativeFamily;
        PInvoke.GdipGetGenericFontFamilySansSerif(&nativeFamily).ThrowIfFailed();
        return nativeFamily;
    }

    /// <summary>
    ///  Gets a generic Serif <see cref='FontFamily'/>.
    /// </summary>
    public static FontFamily GenericSerif => new(GenericFontFamilies.Serif);

    /// <summary>
    ///  Gets a generic monospace <see cref='FontFamily'/>.
    /// </summary>
    public static FontFamily GenericMonospace => new(GenericFontFamilies.Monospace);

    /// <summary>
    ///  Returns an array that contains all of the <see cref='FontFamily'/> objects associated with the specified
    ///  graphics context.
    /// </summary>
    [Obsolete("FontFamily.GetFamilies has been deprecated. Use Families instead.")]
    public static FontFamily[] GetFamilies(Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        return new InstalledFontCollection().Families;
    }

    /// <summary>
    ///  Indicates whether the specified <see cref='FontStyle'/> is available.
    /// </summary>
    public bool IsStyleAvailable(FontStyle style)
    {
        BOOL isStyleAvailable;
        PInvoke.GdipIsStyleAvailable(NativeFamily, (int)style, &isStyleAvailable).ThrowIfFailed();
        GC.KeepAlive(this);
        return isStyleAvailable;
    }

    /// <summary>
    ///  Gets the size of the Em square for the specified style in font design units.
    /// </summary>
    public int GetEmHeight(FontStyle style)
    {
        ushort emHeight;
        PInvoke.GdipGetEmHeight(NativeFamily, (int)style, &emHeight).ThrowIfFailed();
        GC.KeepAlive(this);
        return emHeight;
    }

    /// <summary>
    ///  Returns the ascender metric for Windows.
    /// </summary>
    public int GetCellAscent(FontStyle style)
    {
        ushort cellAscent;
        PInvoke.GdipGetCellAscent(NativeFamily, (int)style, &cellAscent).ThrowIfFailed();
        GC.KeepAlive(this);
        return cellAscent;
    }

    /// <summary>
    ///  Returns the descender metric for Windows.
    /// </summary>
    public int GetCellDescent(FontStyle style)
    {
        ushort cellDescent;
        PInvoke.GdipGetCellDescent(NativeFamily, (int)style, &cellDescent).ThrowIfFailed();
        GC.KeepAlive(this);
        return cellDescent;
    }

    /// <summary>
    ///  Returns the distance between two consecutive lines of text for this <see cref='FontFamily'/> with the
    ///  specified <see cref='FontStyle'/>.
    /// </summary>
    public int GetLineSpacing(FontStyle style)
    {
        ushort lineSpacing;
        PInvoke.GdipGetLineSpacing(NativeFamily, (int)style, &lineSpacing).ThrowIfFailed();
        GC.KeepAlive(this);
        return lineSpacing;
    }
}
