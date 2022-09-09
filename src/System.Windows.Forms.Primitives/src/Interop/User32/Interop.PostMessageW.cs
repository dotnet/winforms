// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern BOOL PostMessageW(
            IntPtr hWnd,
            WM Msg,
            nint wParam = default,
            nint lParam = default);

        public static BOOL PostMessageW(
            IHandle hWnd,
            WM Msg,
            nint wParam = default,
            nint lParam = default)
        {
            BOOL result = PostMessageW(hWnd.Handle, Msg, wParam, lParam);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static BOOL PostMessageW(
            HandleRef hWnd,
            WM Msg,
            nint wParam = default,
            nint lParam = default)
        {
            BOOL result = PostMessageW(hWnd.Handle, Msg, wParam, lParam);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
