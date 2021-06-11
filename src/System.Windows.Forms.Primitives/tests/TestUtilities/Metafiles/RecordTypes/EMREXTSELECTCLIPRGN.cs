// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using System.Text;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct EMREXTSELECTCLIPRGN
    {
        public EMR emr;
        public uint cbRgnData;          // Size of region data in bytes
        public Gdi32.RGN iMode;
        public byte RgnData;

        public Gdi32.RGNDATAHEADER* RegionDataHeader
        {
            get
            {
                fixed (void* data = &RgnData)
                {
                    return cbRgnData >= sizeof(Gdi32.RGNDATAHEADER) ? (Gdi32.RGNDATAHEADER*)data : null;
                }
            }
        }

        public RECT[] ClippingRectangles => Gdi32.RGNDATAHEADER.GetRegionRects(RegionDataHeader);

        public override string ToString()
        {
            if (RegionDataHeader is null)
            {
                return $"[{nameof(EMREXTSELECTCLIPRGN)}] Mode: Set Default";
            }

            StringBuilder sb = new StringBuilder(512);
            sb.Append($@"[{nameof(EMREXTSELECTCLIPRGN)}] Mode: {iMode} Bounds: {RegionDataHeader->rcBound} Rects: {RegionDataHeader->nCount}");

            RECT[] clippingRects = ClippingRectangles;
            for (int i = 0; i < clippingRects.Length; i++)
            {
                sb.AppendFormat("\n\tRect index {0}: {1}", i, clippingRects[i]);
            }

            return sb.ToString();
        }
    }
}
