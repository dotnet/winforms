// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32)]
        public static partial RegionType GetRgnBox(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern RegionType GetRgnBox(
#endif
            IntPtr hrgn,
            ref RECT lprc);

        public static RegionType GetRgnBox(HandleRef hrgn, ref RECT lprc)
        {
            RegionType result = GetRgnBox(hrgn.Handle, ref lprc);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}
