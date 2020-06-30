// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern BOOL GetTextExtentPoint32W(HDC hdc, string lpString, int c, ref Size psizl);

        public static BOOL GetTextExtentPoint32W(HandleRef hdc, string lpString, int c, ref Size psizl)
        {
            BOOL result = GetTextExtentPoint32W((HDC)hdc.Handle, lpString, c, ref psizl);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
