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
        public static extern BOOL GetScrollInfo(IntPtr hwnd, SB nBar, ref SCROLLINFO lpsi);

        public static BOOL GetScrollInfo(IHandle hwnd, SB nBar, ref SCROLLINFO lpsi)
        {
            BOOL result = GetScrollInfo(hwnd.Handle, nBar, ref lpsi);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}
