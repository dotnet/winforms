// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Drawing;

/// <summary>
/// Specifies the Copy Pixel (ROP) operation.
/// </summary>
public enum CopyPixelOperation
{
    /// <summary>
    /// Fills the Destination Rectangle using the color associated with the index 0 in the physical palette.
    /// </summary>
    Blackness = Gdi32.RasterOp.BLACKNESS,
    /// <summary>
    /// Includes any windows that are Layered on Top.
    /// </summary>
    CaptureBlt = Gdi32.RasterOp.CAPTUREBLT,
    DestinationInvert = Gdi32.RasterOp.DSTINVERT,
    MergeCopy = Gdi32.RasterOp.MERGECOPY,
    MergePaint = Gdi32.RasterOp.MERGEPAINT,
    NoMirrorBitmap = Gdi32.RasterOp.NOMIRRORBITMAP,
    NotSourceCopy = Gdi32.RasterOp.NOTSRCCOPY,
    NotSourceErase = Gdi32.RasterOp.NOTSRCERASE,
    PatCopy = Gdi32.RasterOp.PATCOPY,
    PatInvert = Gdi32.RasterOp.PATINVERT,
    PatPaint = Gdi32.RasterOp.PATPAINT,
    SourceAnd = Gdi32.RasterOp.SRCAND,
    SourceCopy = Gdi32.RasterOp.SRCCOPY,
    SourceErase = Gdi32.RasterOp.SRCERASE,
    SourceInvert = Gdi32.RasterOp.SRCINVERT,
    SourcePaint = Gdi32.RasterOp.SRCPAINT,
    Whiteness = Gdi32.RasterOp.WHITENESS,
}
