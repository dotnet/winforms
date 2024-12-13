// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace System.Drawing.Text;

/// <summary>
///  Represents the fonts installed on the system.
/// </summary>
public sealed unsafe class InstalledFontCollection : FontCollection
{
    /// <summary>
    ///  Initializes a new instance of the <see cref='InstalledFontCollection'/> class.
    /// </summary>
    public InstalledFontCollection() : base(Create())
    {
        // The installed font collection is a static in GDI+. We don't need to track it.
        GC.SuppressFinalize(this);
    }

    internal static InstalledFontCollection Instance { get; } = new();

    private static GpFontCollection* Create()
    {
        GpFontCollection* fontCollection;
        PInvokeGdiPlus.GdipNewInstalledFontCollection(&fontCollection).ThrowIfFailed();
        return fontCollection;
    }

    protected override void Dispose(bool disposing)
    {
        // Do nothing.
    }
}
