// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Text;
using System.Globalization;

namespace System.Drawing;

/// <summary>
///  Abstracts a group of type faces having a similar basic design but having certain variation in styles.
/// </summary>
public sealed unsafe class FontFamily : MarshalByRefObject, IDisposable, IPointer<GpFontFamily>
{
    private const ushort NeutralLanguage = 0;

    private GpFontFamily* _nativeFamily;
    private bool _fromInstalledFontCollection;

    nint IPointer<GpFontFamily>.Pointer => (nint)_nativeFamily;

    private void SetNativeFamily(GpFontFamily* family)
    {
        Debug.Assert(_nativeFamily is null, "Setting GDI+ native font family when already initialized.");
        _nativeFamily = family;
    }

    internal FontFamily(GpFontFamily* family, bool fromInstalledFontCollection)
    {
        _fromInstalledFontCollection = fromInstalledFontCollection;
        if (fromInstalledFontCollection)
        {
            // No need to clean up FontFamily objects from the installed font collection.
            GC.SuppressFinalize(this);
        }
        else
        {
            GpFontFamily* clonedFamily;
            PInvokeGdiPlus.GdipCloneFontFamily(family, &clonedFamily).ThrowIfFailed();

            // Only the font collection is ref counted, new font family instances are not created.
            Debug.Assert(clonedFamily == family);

            family = clonedFamily;
        }

        SetNativeFamily(family);
    }

    internal FontFamily Clone()
    {
        if (_fromInstalledFontCollection)
        {
            // No need to copy, we're never going to dispose of the native object.
            return this;
        }

        return new(_nativeFamily, fromInstalledFontCollection: false);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='FontFamily'/> class with the specified name.
    /// </summary>
    /// <param name="createDefaultOnFail">
    ///  Determines how errors are handled when creating a font based on a font family that does not exist on the end
    ///  user's system at run time. If this parameter is true, then a fall-back font will always be used instead.
    ///  If this parameter is false, an exception will be thrown.
    /// </param>
    internal FontFamily(ReadOnlySpan<char> name, bool createDefaultOnFail) =>
        CreateFontFamily(name, fontCollection: null, createDefaultOnFail);

    /// <summary>
    ///  Initializes a new instance of the <see cref='FontFamily'/> class with the specified name.
    /// </summary>
    public FontFamily(string name) => CreateFontFamily(name.OrThrowIfNull(), fontCollection: null);

    /// <summary>
    ///  Initializes a new instance of the <see cref='FontFamily'/> class in the specified
    ///  <see cref='FontCollection'/> and with the specified name.
    /// </summary>
    public FontFamily(string name, FontCollection? fontCollection) => CreateFontFamily(name.OrThrowIfNull(), fontCollection);

    // Creates the native font family object.
    // Note: GDI+ creates singleton font family objects (from the corresponding font file) and reference count them.
    private void CreateFontFamily(ReadOnlySpan<char> name, FontCollection? fontCollection, bool createDefaultOnFail = false)
    {
        GpFontFamily* fontFamily;
        GpFontCollection* nativeFontCollection = fontCollection.Pointer();

        _fromInstalledFontCollection = nativeFontCollection is null
            || nativeFontCollection == InstalledFontCollection.Instance.Pointer();

        Status status = Status.Ok;
        fixed (char* n = name)
        {
            Debug.Assert(n is null || n[name.Length] == '\0', "Expected null-terminated string.");
            status = PInvokeGdiPlus.GdipCreateFontFamilyFromName(n, nativeFontCollection, &fontFamily);
        }

        if (status != Status.Ok)
        {
            if (createDefaultOnFail)
            {
                fontFamily = GetGdipGenericSansSerif();
            }
            else
            {
                // Special case this incredibly common error message to give more information.
                if (status == Status.FontFamilyNotFound)
                {
                    throw new ArgumentException(SR.Format(SR.GdiplusFontFamilyNotFound, name.ToString()));
                }
                else if (status == Status.NotTrueTypeFont)
                {
                    throw new ArgumentException(SR.Format(SR.GdiplusNotTrueTypeFont, name.ToString()));
                }
                else
                {
                    status.ThrowIfFailed();
                }
            }
        }

        if (_fromInstalledFontCollection)
        {
            // No need to clean up FontFamily objects from the installed font collection.
            GC.SuppressFinalize(this);
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
                PInvokeGdiPlus.GdipGetGenericFontFamilySerif(&nativeFamily).ThrowIfFailed();
                break;
            case GenericFontFamilies.SansSerif:
                PInvokeGdiPlus.GdipGetGenericFontFamilySansSerif(&nativeFamily).ThrowIfFailed();
                break;
            case GenericFontFamilies.Monospace:
            default:
                PInvokeGdiPlus.GdipGetGenericFontFamilyMonospace(&nativeFamily).ThrowIfFailed();
                break;
        }

        _fromInstalledFontCollection = true;

        SetNativeFamily(nativeFamily);
    }

    ~FontFamily() => Dispose(disposing: false);

    internal GpFontFamily* NativeFamily => _nativeFamily;

    /// <summary>
    /// Converts this <see cref='FontFamily'/> to a human-readable string.
    /// </summary>
    public override string ToString() => $"[{nameof(FontFamily)}: Name={Name}]";

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj == this)
        {
            return true;
        }

        if (obj is not FontFamily otherFamily)
        {
            return false;
        }

        // GDI+ font families are instances in their font collection, so we can compare the pointers.
        return otherFamily.NativeFamily == NativeFamily;
    }

    /// <summary>
    ///  Gets a hash code for this <see cref='FontFamily'/>.
    /// </summary>
    public override int GetHashCode()
    {
        Span<char> name = stackalloc char[(int)PInvokeCore.LF_FACESIZE];
        GetName(name, NeutralLanguage);
        return string.GetHashCode(name.SliceAtFirstNull());
    }

    /// <summary>
    ///  Disposes of this <see cref='FontFamily'/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_nativeFamily is null || _fromInstalledFontCollection)
        {
            return;
        }

        // This will ref count the associated FontCollection object. This is only strictly necessary for
        // manually loaded FontCollections (PrivateFontCollection).
        Status status = PInvokeGdiPlus.GdipDeleteFontFamily(_nativeFamily);
        _nativeFamily = null;

        if (disposing)
        {
            Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
        }
    }

    /// <summary>
    ///  Gets the name of this <see cref='FontFamily'/>.
    /// </summary>
    public string Name => GetName(CultureInfo.CurrentUICulture.LCID);

    /// <summary>
    ///  Returns the name of this <see cref='FontFamily'/> in the specified language.
    /// </summary>
    public unsafe string GetName(int language)
    {
        Span<char> name = stackalloc char[(int)PInvokeCore.LF_FACESIZE];
        GetName(name, (ushort)language);
        return new(name.SliceAtFirstNull());
    }

    private unsafe void GetName(Span<char> span, ushort language)
    {
        Debug.Assert(span.Length == (int)PInvokeCore.LF_FACESIZE);

        fixed (char* name = span)
        {
            PInvokeGdiPlus.GdipGetFamilyName(NativeFamily, name, language).ThrowIfFailed();
        }
    }

    /// <summary>
    ///  Returns an array that contains all of the <see cref='FontFamily'/> objects associated with the current
    ///  graphics context.
    /// </summary>
    public static FontFamily[] Families => InstalledFontCollection.Instance.Families;

    /// <summary>
    ///  Gets a generic SansSerif <see cref='FontFamily'/>.
    /// </summary>
    public static FontFamily GenericSansSerif => new(GetGdipGenericSansSerif(), fromInstalledFontCollection: true);

    private static GpFontFamily* GetGdipGenericSansSerif()
    {
        GpFontFamily* nativeFamily;
        PInvokeGdiPlus.GdipGetGenericFontFamilySansSerif(&nativeFamily).ThrowIfFailed();
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
        return InstalledFontCollection.Instance.Families;
    }

    /// <summary>
    ///  Indicates whether the specified <see cref='FontStyle'/> is available.
    /// </summary>
    public bool IsStyleAvailable(FontStyle style)
    {
        BOOL isStyleAvailable;
        PInvokeGdiPlus.GdipIsStyleAvailable(NativeFamily, (int)style, &isStyleAvailable).ThrowIfFailed();
        GC.KeepAlive(this);
        return isStyleAvailable;
    }

    /// <summary>
    ///  Gets the size of the Em square for the specified style in font design units.
    /// </summary>
    public int GetEmHeight(FontStyle style)
    {
        ushort emHeight;
        PInvokeGdiPlus.GdipGetEmHeight(NativeFamily, (int)style, &emHeight).ThrowIfFailed();
        GC.KeepAlive(this);
        return emHeight;
    }

    /// <summary>
    ///  Returns the ascender metric for Windows.
    /// </summary>
    public int GetCellAscent(FontStyle style)
    {
        ushort cellAscent;
        PInvokeGdiPlus.GdipGetCellAscent(NativeFamily, (int)style, &cellAscent).ThrowIfFailed();
        GC.KeepAlive(this);
        return cellAscent;
    }

    /// <summary>
    ///  Returns the descender metric for Windows.
    /// </summary>
    public int GetCellDescent(FontStyle style)
    {
        ushort cellDescent;
        PInvokeGdiPlus.GdipGetCellDescent(NativeFamily, (int)style, &cellDescent).ThrowIfFailed();
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
        PInvokeGdiPlus.GdipGetLineSpacing(NativeFamily, (int)style, &lineSpacing).ThrowIfFailed();
        GC.KeepAlive(this);
        return lineSpacing;
    }
}
