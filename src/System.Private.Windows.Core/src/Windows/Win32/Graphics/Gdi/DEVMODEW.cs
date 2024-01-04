// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

internal partial struct DEVMODEW
{
    [UnscopedRef]
    internal ref short dmOrientation => ref Anonymous1.Anonymous1.dmOrientation;

    [UnscopedRef]
    internal ref short dmPaperSize => ref Anonymous1.Anonymous1.dmPaperSize;

    [UnscopedRef]
    internal ref short dmPaperLength => ref Anonymous1.Anonymous1.dmPaperLength;

    [UnscopedRef]
    internal ref short dmPaperWidth => ref Anonymous1.Anonymous1.dmPaperWidth;

    [UnscopedRef]
    internal ref short dmScale => ref Anonymous1.Anonymous1.dmScale;

    [UnscopedRef]
    internal ref short dmCopies => ref Anonymous1.Anonymous1.dmCopies;

    [UnscopedRef]
    internal ref short dmDefaultSource => ref Anonymous1.Anonymous1.dmDefaultSource;

    [UnscopedRef]
    internal ref short dmPrintQuality => ref Anonymous1.Anonymous1.dmPrintQuality;
}
