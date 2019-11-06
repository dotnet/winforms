﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL KillTimer(IntPtr hwnd, int idEvent);

        public static BOOL KillTimer(IHandle hWnd, int idEvent)
        {
            BOOL result = KillTimer(hWnd.Handle, idEvent);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static BOOL KillTimer(HandleRef hWnd, int idEvent)
        {
            BOOL result = KillTimer(hWnd.Handle, idEvent);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
