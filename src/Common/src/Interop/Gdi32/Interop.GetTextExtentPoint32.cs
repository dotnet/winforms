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
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetTextExtentPoint32W(IntPtr hdc, string lpString, int c, ref Size psizl);

        public static int GetTextExtentPoint32W(HandleRef hdc, string lpString, int c, ref Size psizl)
        {
            int result = GetTextExtentPoint32W(hdc.Handle, lpString, c, ref psizl);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
