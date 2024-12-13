// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Text;

/// <summary>
///  When inherited, enumerates the FontFamily objects in a collection of fonts.
/// </summary>
public abstract unsafe class FontCollection : IDisposable, IPointer<GpFontCollection>
{
    private GpFontCollection* _nativeFontCollection;
    nint IPointer<GpFontCollection>.Pointer => (nint)_nativeFontCollection;

    private protected FontCollection(GpFontCollection* nativeFontCollection) => _nativeFontCollection = nativeFontCollection;

    /// <summary>
    ///  Disposes of this <see cref='FontCollection'/>
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) => _nativeFontCollection = null;

    /// <summary>
    ///  Gets the array of <see cref='FontFamily'/> objects associated with this <see cref='FontCollection'/>.
    /// </summary>
    public FontFamily[] Families
    {
        get
        {
            int numFound;
            PInvokeGdiPlus.GdipGetFontCollectionFamilyCount(_nativeFontCollection, &numFound).ThrowIfFailed();

            if (numFound == 0)
            {
                return [];
            }

            bool installedFontCollection = GetType() == typeof(InstalledFontCollection);

            GpFontFamily*[] gpFamilies = new GpFontFamily*[numFound];
            fixed (GpFontFamily** f = gpFamilies)
            {
                PInvokeGdiPlus.GdipGetFontCollectionFamilyList(_nativeFontCollection, numFound, f, &numFound).ThrowIfFailed();
            }

            Debug.Assert(gpFamilies.Length == numFound, "GDI+ can't give a straight answer about how many fonts there are");
            FontFamily[] families = new FontFamily[numFound];
            for (int f = 0; f < numFound; f++)
            {
                // GetFontCollectionFamilyList doesn't ref count the returned families. The internal constructor
                // here will add a ref if the font collection is not the installed font collection.
                families[f] = new FontFamily(gpFamilies[f], installedFontCollection);
            }

            GC.KeepAlive(this);
            return families;
        }
    }

    ~FontCollection() => Dispose(disposing: false);
}
