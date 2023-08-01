// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

internal readonly partial struct HRGN
{
    public unsafe RECT[] GetRegionRects()
    {
        uint regionDataSize = PInvoke.GetRegionData(this, 0, lpRgnData: null);
        if (regionDataSize == 0)
        {
            return Array.Empty<RECT>();
        }

        using BufferScope<byte> buffer = new((int)regionDataSize);

        fixed (byte* b = buffer)
        {
            if (PInvoke.GetRegionData(this, regionDataSize, (RGNDATA*)b) != regionDataSize)
            {
                return Array.Empty<RECT>();
            }

            RECT[] result = RGNDATAHEADER.GetRegionRects((RGNDATAHEADER*)b);
            return result;
        }
    }
}
