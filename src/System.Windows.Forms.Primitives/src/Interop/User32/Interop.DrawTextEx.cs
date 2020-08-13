// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern int DrawTextExW(
            Gdi32.HDC hdc,
            char* lpchText,
            int cchText,
            ref RECT lprc,
            DT format,
            ref DRAWTEXTPARAMS lpdtp);

        public static unsafe int DrawTextExW(
            Gdi32.HDC hdc,
            ReadOnlySpan<char> lpchText,
            ref RECT lprc,
            DT format,
            ref DRAWTEXTPARAMS lpdtp)
        {
            lpdtp.cbSize = (uint)sizeof(DRAWTEXTPARAMS);

            fixed (char* c = lpchText)
            {
                return DrawTextExW(hdc, c, lpchText.Length, ref lprc, format, ref lpdtp);
            }
        }
    }
}
