// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern Gdi32.HDC GetDC(IntPtr hWnd);

        public static Gdi32.HDC GetDC(HandleRef hWnd)
        {
            Gdi32.HDC dc = GetDC(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return dc;
        }
    }
}
