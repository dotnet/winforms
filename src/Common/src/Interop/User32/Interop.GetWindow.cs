// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum GetWindowOption : uint
        {
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_CHILD = 5,
        }

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowOption uCmd);

        public static IntPtr GetWindow(HandleRef hWnd, GetWindowOption uCmd)
        {
            IntPtr result = GetWindow(hWnd.Handle, uCmd);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
