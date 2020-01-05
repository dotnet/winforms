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
        public static extern IntPtr CallNextHookEx(IntPtr hhook, User32.HC nCode, IntPtr wparam, IntPtr lparam);

        public static IntPtr CallNextHookEx(HandleRef hhook, User32.HC nCode, IntPtr wparam, IntPtr lparam)
        {
            IntPtr result = CallNextHookEx(hhook.Handle, nCode, wparam, lparam);
            GC.KeepAlive(hhook.Wrapper);
            return result;
        }
    }
}
