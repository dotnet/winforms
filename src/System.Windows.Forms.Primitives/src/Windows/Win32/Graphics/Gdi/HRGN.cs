// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace Windows.Win32.Graphics.Gdi
{
    internal readonly partial struct HRGN
    {
        public unsafe RECT[] GetRegionRects()
        {
            uint regionDataSize = Interop.Gdi32.GetRegionData(this, 0, IntPtr.Zero);
            if (regionDataSize == 0)
            {
                return Array.Empty<RECT>();
            }

            byte[] buffer = ArrayPool<byte>.Shared.Rent((int)regionDataSize);

            fixed (byte* b = buffer)
            {
                if (Interop.Gdi32.GetRegionData(this, regionDataSize, (IntPtr)b) != regionDataSize)
                {
                    return Array.Empty<RECT>();
                }

                RECT[] result = Interop.Gdi32.RGNDATAHEADER.GetRegionRects((Interop.Gdi32.RGNDATAHEADER*)b);
                ArrayPool<byte>.Shared.Return(buffer);
                return result;
            }
        }
    }
}
