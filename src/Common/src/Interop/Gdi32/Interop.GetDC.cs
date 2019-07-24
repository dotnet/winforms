﻿
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Gdi32
    {
        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        public static IntPtr GetDC(HandleRef hWnd)
        {
            IntPtr dc = GetDC(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return dc;
        }
    }
}
