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
        public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public extern static BOOL EnumThreadWindows(uint dwThreadId, EnumThreadWindowsCallback lpfn, IntPtr lParam);

        public static BOOL EnumThreadWindows(uint dwThreadId, EnumThreadWindowsCallback lpfn, IHandle lParam)
        {
            BOOL result = EnumThreadWindows(dwThreadId, lpfn, lParam.Handle);
            GC.KeepAlive(lParam);
            return result;
        }
    }
}
