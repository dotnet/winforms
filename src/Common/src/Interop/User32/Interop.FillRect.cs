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
        public static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

        public static int FillRect(HandleRef hDC, ref RECT lprc, HandleRef hbr)
        {
            int result = FillRect(hDC.Handle, ref lprc, hbr.Handle);
            GC.KeepAlive(hDC.Wrapper);
            GC.KeepAlive(hbr.Wrapper);
            return result;
        }
    }
}
