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
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static uint GetWindowThreadProcessId(IHandle hWnd, out uint lpdwProcessId)
        {
            uint result = GetWindowThreadProcessId(hWnd.Handle, out lpdwProcessId);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}
