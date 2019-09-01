﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectPalette(IntPtr hdc, IntPtr hPal, BOOL bForceBkgd);

        public static IntPtr SelectPalette(HandleRef hdc, IntPtr hPal, BOOL bForceBkgd)
        {
            IntPtr result = SelectPalette(hdc.Handle, hPal, bForceBkgd);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }

        public static IntPtr SelectPalette(HandleRef hdc, HandleRef hPal, BOOL bForceBkgd)
        {
            IntPtr result = SelectPalette(hdc.Handle, hPal.Handle, bForceBkgd);
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(hPal.Wrapper);
            return result;
        }
    }
}
