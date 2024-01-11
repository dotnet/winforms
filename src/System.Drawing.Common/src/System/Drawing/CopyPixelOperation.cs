// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

/// <summary>
///  Specifies the Copy Pixel (ROP) operation.
/// </summary>
public enum CopyPixelOperation
{
    /// <summary>
    ///  Fills the Destination Rectangle using the color associated with the index 0 in the physical palette.
    /// </summary>
    Blackness = (int)ROP_CODE.BLACKNESS,

    /// <summary>
    ///  Includes any windows that are Layered on Top.
    /// </summary>
    CaptureBlt = (int)ROP_CODE.CAPTUREBLT,
    DestinationInvert = (int)ROP_CODE.DSTINVERT,
    MergeCopy = (int)ROP_CODE.MERGECOPY,
    MergePaint = (int)ROP_CODE.MERGEPAINT,
    NoMirrorBitmap = unchecked((int)ROP_CODE.NOMIRRORBITMAP),
    NotSourceCopy = (int)ROP_CODE.NOTSRCCOPY,
    NotSourceErase = (int)ROP_CODE.NOTSRCERASE,
    PatCopy = (int)ROP_CODE.PATCOPY,
    PatInvert = (int)ROP_CODE.PATINVERT,
    PatPaint = (int)ROP_CODE.PATPAINT,
    SourceAnd = (int)ROP_CODE.SRCAND,
    SourceCopy = (int)ROP_CODE.SRCCOPY,
    SourceErase = (int)ROP_CODE.SRCERASE,
    SourceInvert = (int)ROP_CODE.SRCINVERT,
    SourcePaint = (int)ROP_CODE.SRCPAINT,
    Whiteness = (int)ROP_CODE.WHITENESS,
}
