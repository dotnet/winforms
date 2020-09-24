// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Buffers;
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

        public RECT[] ClippingRectangles => GetRectsFromRegion(RegionDataHeader);

        public override string ToString()
        {
            if (RegionDataHeader is null)
            {
                return $"[{nameof(EMREXTSELECTCLIPRGN)}] Mode: Set Default";
            }

            StringBuilder sb = new StringBuilder(512);
            sb.Append($@"[{nameof(EMREXTSELECTCLIPRGN)}] Mode: {iMode} Bounds: {RegionDataHeader->rcBound} Rects: {
                    RegionDataHeader->nCount}");

            RECT[] clippingRects = ClippingRectangles;
            for (int i = 0; i < clippingRects.Length; i++)
            {
                sb.AppendFormat("\n\tRect index {0}: {1}", i, clippingRects[i]);
            }

            return sb.ToString();
        }

        public unsafe static RECT[] GetRectsFromRegion(Gdi32.HRGN handle)
        {
            uint regionDataSize = Gdi32.GetRegionData(handle.Handle, 0, IntPtr.Zero);
            if (regionDataSize == 0)
            {
                return Array.Empty<RECT>();
            }

            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)regionDataSize);

            fixed (byte* b = buffer)
            {
                if (Gdi32.GetRegionData(handle.Handle, regionDataSize, (IntPtr)b) != regionDataSize)
                {
                    return Array.Empty<RECT>();
                }

                RECT[] result = GetRectsFromRegion((Gdi32.RGNDATAHEADER*)b);
                ArrayPool<byte>.Shared.Return(buffer);
                return result;
            }
        }

        public unsafe static RECT[] GetRectsFromRegion(Gdi32.RGNDATAHEADER* regionData)
        {
            int count = (int)regionData->nCount;
            if (count == 0)
            {
                return Array.Empty<RECT>();
            }

            var regionRects = new RECT[count];

            Span<RECT> sourceRects = new Span<RECT>((byte*)regionData + regionData->dwSize, count);
            sourceRects.CopyTo(regionRects.AsSpan());

            return regionRects;
        }
    }
}
