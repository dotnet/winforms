// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Win32.Foundation;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static HWND SetParent<T>(in T hWndChild, HWND hWndNewParent) where T : IHandle<HWND>
        {
            HWND result = SetParent(hWndChild.Handle, hWndNewParent);
            GC.KeepAlive(hWndChild);
            return result;
        }

        public static HWND SetParent<T>(HWND hWndChild, in T hWndNewParent) where T : IHandle<HWND>
        {
            HWND result = SetParent(hWndChild, hWndNewParent.Handle);
            GC.KeepAlive(hWndNewParent);
            return result;
        }

        public static HWND SetParent<T>(in T hWndChild, in T hWndNewParent) where T : IHandle<HWND>
        {
            HWND result = SetParent(hWndChild.Handle, hWndNewParent.Handle);
            GC.KeepAlive(hWndChild);
            GC.KeepAlive(hWndNewParent);
            return result;
        }
    }
}
