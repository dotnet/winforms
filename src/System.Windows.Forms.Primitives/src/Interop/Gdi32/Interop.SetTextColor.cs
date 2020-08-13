// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern int SetTextColor(HDC hdc, int color);

        public static int SetTextColor(IHandle hdc, int color)
        {
            int result = SetTextColor((HDC)hdc.Handle, color);
            GC.KeepAlive(hdc);
            return result;
        }

        public static int SetTextColor(HandleRef hdc, int color)
        {
            int result = SetTextColor((HDC)hdc.Handle, color);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
