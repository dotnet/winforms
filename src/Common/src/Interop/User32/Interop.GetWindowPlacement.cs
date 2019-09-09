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
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "GetWindowPlacement")]
        private static extern BOOL GetWindowPlacementInternal(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public unsafe static BOOL GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl)
        {
            lpwndpl = new WINDOWPLACEMENT
            {
                length = (uint)sizeof(WINDOWPLACEMENT)
            };
            return GetWindowPlacementInternal(hWnd, ref lpwndpl);
        }

        public static BOOL GetWindowPlacement(IHandle hWnd, out WINDOWPLACEMENT lpwndpl)
        {
            BOOL result = GetWindowPlacement(hWnd.Handle, out lpwndpl);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
