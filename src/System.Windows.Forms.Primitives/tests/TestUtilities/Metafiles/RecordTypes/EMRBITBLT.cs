// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMRBITBLT
{
    public EMR emr;
    public RECT rclBounds;          // Inclusive-inclusive bounds in device units
    public int xDest;
    public int yDest;
    public int cxDest;
    public int cyDest;
    public ROP_CODE dwRop;
    public int xSrc;
    public int ySrc;
    public Matrix3x2 xformSrc;     // Source DC transform
    public COLORREF crBkColorSrc;  // Source DC BkColor in RGB
    public DIB_USAGE iUsageSrc;    // Source bitmap info color table usage
                                   // (DIB_RGB_COLORS)
    public uint offBmiSrc;         // Offset to the source BITMAPINFO structure
    public uint cbBmiSrc;          // Size of the source BITMAPINFO structure
    public uint offBitsSrc;        // Offset to the source bitmap bits
    public uint cbBitsSrc;         // Size of the source bitmap bits

    public override readonly string ToString()
    {
        RECT dest = new Rectangle(xDest, yDest, cxDest, cyDest);
        return $@"[{nameof(EMRBITBLT)}] Bounds: {rclBounds} Destination: {dest} Rop: {dwRop}Source DC Color: {crBkColorSrc.ToSystemColorString()}";
    }
}
