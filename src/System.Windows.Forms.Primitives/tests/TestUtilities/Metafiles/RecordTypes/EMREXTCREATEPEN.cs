// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal struct EMREXTCREATEPEN
{
    public EMR emr;
    public uint ihPen;              // Pen handle index
    public uint offBmi;             // Offset to the BITMAPINFO structure if any
    public uint cbBmi;              // Size of the BITMAPINFO structure if any
                                    // The bitmap info is followed by the bitmap
                                    // bits to form a packed DIB.
    public uint offBits;            // Offset to the brush bitmap bits if any
    public uint cbBits;             // Size of the brush bitmap bits if any
    public EXTLOGPEN32 elp;         // The extended pen with the style array.

    public override readonly string ToString()
        => $@"[{nameof(EMREXTCREATEPEN)}] Index: {ihPen} Style: {elp.elpPenStyle} Width: {elp.elpWidth}BrushStyle: {elp.elpBrushStyle} Color: {elp.elpColor.ToSystemColorString()}";
}

[StructLayout(LayoutKind.Sequential)]
internal struct EXTLOGPEN32
{
    public PEN_STYLE elpPenStyle;
    public uint elpWidth;
    public BRUSH_STYLE elpBrushStyle;
    public COLORREF elpColor;
    public uint elpHatch;
    public uint elpNumEntries;
    public uint elpStyleEntry;

    public static implicit operator EXTLOGPEN32(LOGPEN logPen) => new()
    {
        elpPenStyle = logPen.lopnStyle,
        elpWidth = (uint)logPen.lopnWidth.X,
        elpColor = logPen.lopnColor
    };
}
