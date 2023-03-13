// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [return: MarshalAs(UnmanagedType.Bool)]
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32)]
        public static partial bool RestoreDC(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern bool RestoreDC(
#endif
            IntPtr hdc,
            int nSavedDC);

        public static bool RestoreDC(HandleRef hdc, int nSavedDC)
        {
            bool result = RestoreDC(hdc.Handle, nSavedDC);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
