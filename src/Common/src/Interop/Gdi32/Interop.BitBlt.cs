﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern BOOL BitBlt(
            IntPtr hdc,
            int x,
            int y,
            int cx,
            int cy,
            IntPtr hdcSrc,
            int x1,
            int y1,
            ROP rop);

        public static BOOL BitBlt(
            HandleRef hdc,
            int x,
            int y,
            int cx,
            int cy,
            HandleRef hdcSrc,
            int x1,
            int y1,
            ROP rop)
        {
            BOOL result = BitBlt(
                hdc.Handle,
                x,
                y,
                cx,
                cy,
                hdcSrc.Handle,
                x1,
                y1,
                rop
            );
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(hdcSrc.Wrapper);
            return result;
        }
    }
}
