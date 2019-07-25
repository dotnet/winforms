// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "GetWindowPlacement")]
        private static extern int GetWindowPlacementInternal(IntPtr hWnd, ref WINDOWPLACEMENT placement);

        public static int GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT placement)
        {
            placement = new WINDOWPLACEMENT
            {
                length = Marshal.SizeOf<WINDOWPLACEMENT>()
            };
            return GetWindowPlacementInternal(hWnd, ref placement);
        }

        public static int GetWindowPlacement(HandleRef hWnd, out WINDOWPLACEMENT placement)
        {
            int result = GetWindowPlacement(hWnd.Handle, out placement);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
