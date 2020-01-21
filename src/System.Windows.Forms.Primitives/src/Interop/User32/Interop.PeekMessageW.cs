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
        public static extern BOOL PeekMessageA(
            ref MSG msg,
            IntPtr hwnd = default,
            WM msgMin = (WM)0,
            WM msgMax = (WM)0,
            PM remove = PM.NOREMOVE);

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL PeekMessageW(
            ref MSG msg,
            IntPtr hwnd = default,
            WM msgMin = (WM)0,
            WM msgMax = (WM)0,
            PM remove = PM.NOREMOVE);

        public static BOOL PeekMessageW(
            ref MSG msg,
            IHandle hwnd,
            WM msgMin = (WM)0,
            WM msgMax = (WM)0,
            PM remove = PM.NOREMOVE)
        {
            BOOL result = PeekMessageW(ref msg, hwnd.Handle, msgMin, msgMax, remove);
            GC.KeepAlive(hwnd);
            return result;
        }

        [Flags]
        public enum PM : uint
        {
            NOREMOVE = 0x0000,
            REMOVE = 0x0001,
            NOYIELD = 0x0002
        }
    }
}
