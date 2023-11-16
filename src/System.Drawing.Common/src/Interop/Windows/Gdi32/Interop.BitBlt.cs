// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32, SetLastError = true)]
        public static partial int BitBlt(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern int BitBlt(
#endif
            IntPtr hdc,
            int x,
            int y,
            int cx,
            int cy,
            IntPtr hdcSrc,
            int x1,
            int y1,
            RasterOp rop);

        public static int BitBlt(HandleRef hdc, int x, int y, int cx, int cy,
                                 HandleRef hdcSrc, int x1, int y1, RasterOp rop)
        {
            int result = BitBlt(hdc.Handle, x, y, cx, cy, hdcSrc.Handle, x1, y1, rop);
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(hdcSrc.Wrapper);
            return result;
        }
    }
}
