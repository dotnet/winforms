// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Runtime.InteropServices;
using System.Text;

namespace System.Windows.Forms.Metafiles;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EMREXTSELECTCLIPRGN
{
    public EMR emr;
    public uint cbRgnData;          // Size of region data in bytes
    public RGN_COMBINE_MODE iMode;
    public byte RgnData;

    public RGNDATAHEADER* RegionDataHeader
    {
        get
        {
            fixed (void* data = &RgnData)
            {
                return cbRgnData >= sizeof(RGNDATAHEADER) ? (RGNDATAHEADER*)data : null;
            }
        }
    }

    public RECT[] ClippingRectangles => RGNDATAHEADER.GetRegionRects(RegionDataHeader);

    public override string ToString()
    {
        if (RegionDataHeader is null)
        {
            return $"[{nameof(EMREXTSELECTCLIPRGN)}] Mode: Set Default";
        }

        StringBuilder sb = new(512);
        sb.Append($@"[{nameof(EMREXTSELECTCLIPRGN)}] Mode: {iMode} Bounds: {RegionDataHeader->rcBound} Rects: {RegionDataHeader->nCount}");

        RECT[] clippingRects = ClippingRectangles;
        for (int i = 0; i < clippingRects.Length; i++)
        {
            sb.Append($"\n\tRect index {i}: {clippingRects[i]}");
        }

        return sb.ToString();
    }
}
