// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
namespace System.Drawing.Text;

/// <summary>
///  Represents the fonts installed on the system.
/// </summary>
public unsafe sealed class InstalledFontCollection : FontCollection
{
    /// <summary>
    ///  Initializes a new instance of the <see cref='InstalledFontCollection'/> class.
    /// </summary>
    public InstalledFontCollection() : base()
    {
        GpFontCollection* fontCollection;
        PInvoke.GdipNewInstalledFontCollection(&fontCollection).ThrowIfFailed();
        _nativeFontCollection = fontCollection;
    }
}
