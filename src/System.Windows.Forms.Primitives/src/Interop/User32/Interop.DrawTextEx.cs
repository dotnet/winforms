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
        [DllImport(Libraries.User32, EntryPoint = "DrawTextExW", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int DrawTextExWInternal(Gdi32.HDC hdc, string lpchText, int cchText, ref RECT lprc, DT format, ref DRAWTEXTPARAMS lpdtp);

        public static unsafe int DrawTextExW(Gdi32.HDC hdc, string lpchText, int cchText, ref RECT lprc, DT format, ref DRAWTEXTPARAMS lpdtp)
        {
            lpdtp.cbSize = (uint)sizeof(DRAWTEXTPARAMS);
            return DrawTextExWInternal(hdc, lpchText, cchText, ref lprc, format, ref lpdtp);
        }

        public static int DrawTextExW(IHandle hdc, string lpchText, int cchText, ref RECT lprc, DT format, ref DRAWTEXTPARAMS lpdtp)
        {
            int result = DrawTextExW((Gdi32.HDC)hdc.Handle, lpchText, cchText, ref lprc, format, ref lpdtp);
            GC.KeepAlive(hdc);
            return result;
        }
    }
}
