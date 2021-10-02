// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public unsafe static extern BOOL GetWindowRect(IntPtr hWnd, ref RECT rect);

        public static BOOL GetWindowRect(HandleRef hWnd, ref RECT rect)
        {
            BOOL result = GetWindowRect(hWnd.Handle, ref rect);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public static BOOL GetWindowRect(IHandle hWnd, ref RECT rect)
        {
            BOOL result = GetWindowRect(hWnd.Handle, ref rect);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static Rectangle GetWindowRect(IHandle hWnd)
        {
            Unsafe.SkipInit(out RECT rectangle);
            return User32.GetWindowRect(hWnd, ref rectangle).IsTrue() ? rectangle : default;
        }
    }
}
