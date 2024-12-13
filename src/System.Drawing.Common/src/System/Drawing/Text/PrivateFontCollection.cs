// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;

namespace System.Drawing.Text;

/// <summary>
///  Encapsulates a collection of <see cref='Font'/> objects.
/// </summary>
public sealed unsafe class PrivateFontCollection : FontCollection
{
    /// <summary>
    ///  Initializes a new instance of the <see cref='PrivateFontCollection'/> class.
    /// </summary>
    public PrivateFontCollection() : base(Create())
    {
    }

    private static GpFontCollection* Create()
    {
        GpFontCollection* fontCollection;
        PInvokeGdiPlus.GdipNewPrivateFontCollection(&fontCollection).ThrowIfFailed();
        return fontCollection;
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='PrivateFontCollection'/>.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        GpFontCollection* nativeFontCollection = this.Pointer();

        if (nativeFontCollection is not null)
        {
            Status status = PInvokeGdiPlus.GdipDeletePrivateFontCollection(&nativeFontCollection);
            if (disposing)
            {
                Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Adds a font from the specified file to this <see cref='PrivateFontCollection'/>.
    /// </summary>
    public void AddFontFile(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        if (!File.Exists(filename))
        {
            throw new FileNotFoundException();
        }

        fixed (char* p = filename)
        {
            PInvokeGdiPlus.GdipPrivateAddFontFile(this.Pointer(), p).ThrowIfFailed();
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
        PInvokeGdiPlus.GdipPrivateAddMemoryFont(this.Pointer(), (void*)memory, length).ThrowIfFailed();
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
