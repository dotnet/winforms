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
        private static extern BOOL ScrollWindow(IntPtr hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip);

        public static BOOL ScrollWindow(IHandle hWnd, int nXAmount, int nYAmount, ref RECT rectScrollRegion, ref RECT rectClip)
        {
            BOOL result = ScrollWindow(hWnd.Handle, nXAmount, nYAmount, ref rectScrollRegion, ref rectClip);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
