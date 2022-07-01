﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        public static partial BOOL SetMenu(IntPtr hWnd, IntPtr hMenu);

        public static BOOL SetMenu(IHandle hWnd, IntPtr hMenu)
        {
            BOOL result = SetMenu(hWnd.Handle, hMenu);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static BOOL SetMenu(IHandle hWnd, HandleRef hMenu)
        {
            BOOL result = SetMenu(hWnd.Handle, hMenu.Handle);
            GC.KeepAlive(hWnd);
            GC.KeepAlive(hMenu.Wrapper);
            return result;
        }
    }
}
