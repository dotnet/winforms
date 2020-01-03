// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        public static BOOL ClientToScreen(HandleRef hWnd, ref Point lpPoint)
        {
            BOOL result = ClientToScreen(hWnd.Handle, ref lpPoint);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
