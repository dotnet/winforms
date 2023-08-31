// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32, SetLastError = true)]
        public static partial RegionType SelectClipRgn(
#else
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern RegionType SelectClipRgn(
#endif
            IntPtr hdc,
            IntPtr hrgn);

        public static RegionType SelectClipRgn(HandleRef hdc, HandleRef hrgn)
        {
            RegionType result = SelectClipRgn(hdc.Handle, hrgn.Handle);
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}
