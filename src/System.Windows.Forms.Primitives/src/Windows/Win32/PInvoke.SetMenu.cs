﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static BOOL SetMenu<T>(T hWnd, HMENU hMenu)
            where T : IHandle<HWND>
        {
            BOOL result = SetMenu(hWnd.Handle, hMenu);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public static BOOL SetMenu<T1, T2>(T1 hWnd, T2 hMenu)
            where T1 : IHandle<HWND>
            where T2: IHandle<HMENU>
        {
            BOOL result = SetMenu(hWnd.Handle, hMenu.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            GC.KeepAlive(hMenu.Wrapper);
            return result;
        }
    }
}
