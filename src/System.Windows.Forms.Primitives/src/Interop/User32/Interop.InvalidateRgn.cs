﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        private static partial BOOL InvalidateRgn(IntPtr hWnd, Gdi32.HRGN hrgn, BOOL erase);

        public static BOOL InvalidateRgn(IHandle hWnd, Gdi32.HRGN hrgn, BOOL erase)
        {
            BOOL result = InvalidateRgn(hWnd.Handle, hrgn, erase);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
