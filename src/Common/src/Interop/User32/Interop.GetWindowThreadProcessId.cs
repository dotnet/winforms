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
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public static int GetWindowThreadProcessId(HandleRef hWnd, out int lpdwProcessId)
        {
            int result = GetWindowThreadProcessId(hWnd.Handle, out lpdwProcessId);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
