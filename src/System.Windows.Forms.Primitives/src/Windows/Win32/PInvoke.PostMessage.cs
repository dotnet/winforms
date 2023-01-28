// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <inheritdoc cref="PostMessage(HWND, uint, WPARAM, LPARAM)"/>
        public static BOOL PostMessage<T>(
           T hWnd,
           WM Msg,
           WPARAM wParam = default,
           LPARAM lParam = default)
           where T : IHandle<HWND>
        {
            BOOL result = PostMessage(hWnd.Handle, (uint)Msg, wParam, lParam);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}
