﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        private static partial int SetWindowRgn(IntPtr hwnd, IntPtr hrgn, BOOL fRedraw);

        public static int SetWindowRgn(IHandle hwnd, Gdi32.HRGN hrgn, BOOL fRedraw)
        {
            int result = SetWindowRgn(hwnd.Handle, hrgn.Handle, fRedraw);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
