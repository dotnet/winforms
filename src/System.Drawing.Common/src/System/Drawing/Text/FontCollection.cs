// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Text;

/// <summary>
///  When inherited, enumerates the FontFamily objects in a collection of fonts.
/// </summary>
public unsafe abstract class FontCollection : IDisposable
{
    internal GpFontCollection* _nativeFontCollection;

    internal FontCollection() => _nativeFontCollection = null;

    /// <summary>
    ///  Disposes of this <see cref='FontCollection'/>
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) { }

    /// <summary>
    ///  Gets the array of <see cref='FontFamily'/> objects associated with this <see cref='FontCollection'/>.
    /// </summary>
    public FontFamily[] Families
    {
        get
        {
            int numFound;
            PInvoke.GdipGetFontCollectionFamilyCount(_nativeFontCollection, &numFound).ThrowIfFailed();

            if (numFound == 0)
            {
                return [];
            }

            GpFontFamily*[] gpFamilies = new GpFontFamily*[numFound];
            fixed (GpFontFamily** f = gpFamilies)
            {
                PInvoke.GdipGetFontCollectionFamilyList(_nativeFontCollection, numFound, f, &numFound).ThrowIfFailed();
            }

            Debug.Assert(gpFamilies.Length == numFound, "GDI+ can't give a straight answer about how many fonts there are");
            FontFamily[] families = new FontFamily[numFound];
            for (int f = 0; f < numFound; f++)
            {
                GpFontFamily* native;
                PInvoke.GdipCloneFontFamily(gpFamilies[f], &native).ThrowIfFailed();
                families[f] = new FontFamily(native);
            }

            GC.KeepAlive(this);
            return families;
        }
    }

    ~FontCollection() => Dispose(disposing: false);
}
