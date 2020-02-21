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
        public static extern IntPtr SetTimer(IntPtr hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc);

        public static IntPtr SetTimer(IHandle hWnd, IntPtr nIDEvent, uint uElapse, IntPtr lpTimerFunc)
        {
            IntPtr result = SetTimer(hWnd.Handle, nIDEvent, uElapse, lpTimerFunc);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
