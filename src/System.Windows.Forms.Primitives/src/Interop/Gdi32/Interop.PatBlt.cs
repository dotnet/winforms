// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [LibraryImport(Libraries.Gdi32)]
        public static partial BOOL PatBlt(HDC hdc, int x, int y, int w, int h, ROP rop);

        public static BOOL PatBlt(HandleRef hdc, int x, int y, int w, int h, ROP rop)
        {
            BOOL result = PatBlt((HDC)hdc.Handle, x, y, w, h, rop);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
