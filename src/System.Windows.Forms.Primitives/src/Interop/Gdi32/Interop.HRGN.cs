// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public readonly struct HRGN
        {
            public IntPtr Handle { get; }

            public HRGN(IntPtr handle) => Handle = handle;

            public bool IsNull => Handle == IntPtr.Zero;

            public static implicit operator HGDIOBJ(HRGN hrgn) => new HGDIOBJ(hrgn.Handle);
            public static explicit operator IntPtr(HRGN hrgn) => hrgn.Handle;
            public static explicit operator HRGN(IntPtr hrgn) => new HRGN(hrgn);

            public unsafe RECT[] GetRegionRects()
            {
                uint regionDataSize = GetRegionData(Handle, 0, IntPtr.Zero);
                if (regionDataSize == 0)
                {
                    return Array.Empty<RECT>();
                }

                byte[] buffer = ArrayPool<byte>.Shared.Rent((int)regionDataSize);

                fixed (byte* b = buffer)
                {
                    if (GetRegionData(Handle, regionDataSize, (IntPtr)b) != regionDataSize)
                    {
                        return Array.Empty<RECT>();
                    }

                    RECT[] result = RGNDATAHEADER.GetRegionRects((RGNDATAHEADER*)b);
                    ArrayPool<byte>.Shared.Return(buffer);
                    return result;
                }
            }
        }
    }
}
