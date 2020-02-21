// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        public static extern BOOL SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, LWA dwFlags);

        public static BOOL SetLayeredWindowAttributes(IHandle hwnd, int crKey, byte bAlpha, LWA dwFlags)
        {
            BOOL result = SetLayeredWindowAttributes(hwnd.Handle, crKey, bAlpha, dwFlags);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
