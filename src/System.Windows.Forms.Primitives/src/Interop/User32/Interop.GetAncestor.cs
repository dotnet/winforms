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
        public static extern IntPtr GetAncestor(IntPtr hwnd, GA flags);

        public static IntPtr GetAncestor(IHandle hwnd, GA flags)
        {
            IntPtr result = GetAncestor(hwnd.Handle, flags);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
