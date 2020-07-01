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
        public static extern IntPtr WindowFromDC(Gdi32.HDC hDC);

        public static IntPtr WindowFromDC(IHandle hDC)
        {
            IntPtr result = WindowFromDC((Gdi32.HDC)hDC.Handle);
            GC.KeepAlive(hDC);
            return result;
        }
    }
}
