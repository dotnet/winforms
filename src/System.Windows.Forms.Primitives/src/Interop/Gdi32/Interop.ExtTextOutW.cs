// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = false, CharSet = CharSet.Unicode)]
        public unsafe static extern BOOL ExtTextOutW(HDC hdc, int x, int y, ETO options, ref RECT lprect, string lpString, int c, int* lpDx);
    }
}
