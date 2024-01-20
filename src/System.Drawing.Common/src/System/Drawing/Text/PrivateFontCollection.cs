// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;

namespace System.Drawing.Text;

/// <summary>
///  Encapsulates a collection of <see cref='Font'/> objects.
/// </summary>
public unsafe sealed class PrivateFontCollection : FontCollection
{
    /// <summary>
    ///  Initializes a new instance of the <see cref='PrivateFontCollection'/> class.
    /// </summary>
    public PrivateFontCollection() : base()
    {
        GpFontCollection* fontCollection;
        PInvoke.GdipNewPrivateFontCollection(&fontCollection).ThrowIfFailed();
        _nativeFontCollection = fontCollection;
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='PrivateFontCollection'/>.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (_nativeFontCollection is not null)
        {
            GpFontCollection* nativeFontCollection = _nativeFontCollection;

            try
            {
#if DEBUG
                Status status = !Gdip.Initialized ? Status.Ok :
#endif
                PInvoke.GdipDeletePrivateFontCollection(&nativeFontCollection);
#if DEBUG
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
#endif
            }
            catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
            {
            }
            finally
            {
                _nativeFontCollection = null;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Adds a font from the specified file to this <see cref='PrivateFontCollection'/>.
    /// </summary>
    public void AddFontFile(string filename)
    {
        if (_nativeFontCollection is null)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            // This is the default behavior on Desktop. The ArgumentException originates from GdipPrivateAddFontFile which would
            // refuse the null pointer.
            throw new ArgumentException();
#pragma warning restore CA2208
        }

        if (filename is null)
        {
            throw new ArgumentNullException(nameof(filename));
        }

        // this ensure the filename is valid (or throw the correct exception)
        string fullPath = Path.GetFullPath(filename);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException();
        }

        fixed (char* p = fullPath)
        {
            PInvoke.GdipPrivateAddFontFile(_nativeFontCollection, p).ThrowIfFailed();
            GC.KeepAlive(this);
        }

        // Register private font with GDI as well so pure GDI-based controls (TextBox, Button for instance) can access it.
        GdiAddFontFile(filename);
    }

    /// <summary>
    ///  Adds a font contained in system memory to this <see cref='PrivateFontCollection'/>.
    /// </summary>
    public void AddMemoryFont(IntPtr memory, int length)
    {
        PInvoke.GdipPrivateAddMemoryFont(_nativeFontCollection, (void*)memory, length).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    private static void GdiAddFontFile(string filename)
    {
        fixed (char* fn = filename)
        {
            PInvoke.AddFontResourceEx(fn, FONT_RESOURCE_CHARACTERISTICS.FR_PRIVATE);
        }
    }
}
