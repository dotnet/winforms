// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
        private unsafe static extern uint GetPaletteEntries(IntPtr hpal, uint iStart, uint nEntries, PALETTEENTRY* lppe);

        public unsafe static uint GetPaletteEntries(IntPtr hpal, Span<PALETTEENTRY> entries)
        {
            fixed (PALETTEENTRY* entry = &MemoryMarshal.GetReference(entries))
            {
                return GetPaletteEntries(hpal, 0, (uint)entries.Length, entry);
            }
        }

        public struct PALETTEENTRY
        {
            public byte peRed;
            public byte peGreen;
            public byte peBlue;
            public byte peFlags;
        }
    }
}
