﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern HDC GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DCX flags);

        public static HDC GetDCEx(IHandle hWnd, IntPtr hrgnClip, DCX flags)
        {
            HDC result = GetDCEx(hWnd.Handle, hrgnClip, flags);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
