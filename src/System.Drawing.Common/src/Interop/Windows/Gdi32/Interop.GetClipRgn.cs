// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32)]
        public static partial int GetClipRgn(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern int GetClipRgn(
#endif
            IntPtr hdc,
            IntPtr hrgn);

        public static int GetClipRgn(HandleRef hdc, IntPtr hrgn)
        {
            int result = GetClipRgn(hdc.Handle, hrgn);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }

        public static int GetClipRgn(HandleRef hdc, HandleRef hrgn)
        {
            int result = GetClipRgn(hdc.Handle, hrgn.Handle);
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}
