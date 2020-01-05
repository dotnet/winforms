// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndChildNewParent);

        public static IntPtr SetParent(IntPtr hWndChild, HandleRef hWndChildNewParent)
        {
            IntPtr result = SetParent(hWndChild, hWndChildNewParent.Handle);
            GC.KeepAlive(hWndChildNewParent.Wrapper);
            return result;
        }

        public static IntPtr SetParent(HandleRef hWndChild, IntPtr hWndChildNewParent)
        {
            IntPtr result = SetParent(hWndChild.Handle, hWndChildNewParent);
            GC.KeepAlive(hWndChild.Wrapper);
            return result;
        }

        public static IntPtr SetParent(HandleRef hWndChild, HandleRef hWndChildNewParent)
        {
            IntPtr result = SetParent(hWndChild.Handle, hWndChildNewParent.Handle);
            GC.KeepAlive(hWndChild.Wrapper);
            GC.KeepAlive(hWndChildNewParent.Wrapper);
            return result;
        }
    }
}
