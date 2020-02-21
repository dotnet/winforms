// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [DllImport(Libraries.Gdi32, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

        public static IntPtr SelectObject(HandleRef hdc, IntPtr h)
        {
            IntPtr lastObject = SelectObject(hdc.Handle, h);
            GC.KeepAlive(hdc.Wrapper);
            return lastObject;
        }

        public static IntPtr SelectObject(HandleRef hdc, HandleRef h)
        {
            IntPtr lastObject = SelectObject(hdc.Handle, h.Handle);
            GC.KeepAlive(hdc.Wrapper);
            GC.KeepAlive(h.Wrapper);
            return lastObject;
        }
    }
}
