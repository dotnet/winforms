// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        public static IntPtr GetParent(IHandle hWnd)
        {
            IntPtr result = GetParent(hWnd.Handle);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static IntPtr GetParent(HandleRef hWnd)
        {
            IntPtr result = GetParent(hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
