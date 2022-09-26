// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static LRESULT SendMessage<T>(
            T hWnd,
            WM Msg,
            WPARAM wParam = default,
            LPARAM lParam = default) where T : IHandle<HWND>
        {
            LRESULT result = SendMessage(hWnd.Handle, (uint)Msg, wParam, lParam);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public static LRESULT SendMessage<THwnd, TWParam>(
            THwnd hWnd,
            WM Msg,
            TWParam wParam,
            LPARAM lParam = default) where THwnd : IHandle<HWND> where TWParam : IHandle<HWND>
        {
            LRESULT result = SendMessage(hWnd.Handle, (uint)Msg, (WPARAM)wParam.Handle, lParam);
            GC.KeepAlive(wParam.Wrapper);
            return result;
        }

        public unsafe static LRESULT SendMessage<T>(
            T hWnd,
            WM Msg,
            WPARAM wParam,
            string? lParam) where T : IHandle<HWND>
        {
            fixed (char* c = lParam)
            {
                return SendMessage(hWnd, Msg, wParam, (LPARAM)c);
            }
        }

        public unsafe static nint SendMessage<THwnd, TLParam>(
            THwnd hWnd,
            WM Msg,
            WPARAM wParam,
            ref TLParam lParam)
            where THwnd : IHandle<HWND>
            where TLParam : unmanaged
        {
            fixed (void* l = &lParam)
            {
                return SendMessage(hWnd, Msg, wParam, (LPARAM)l);
            }
        }

        public unsafe static nint SendMessage<THwnd, TWParam, TLParam>(
            THwnd hWnd,
            WM Msg,
            ref TWParam wParam,
            ref TLParam lParam)
            where THwnd : IHandle<HWND>
            where TWParam : unmanaged
            where TLParam : unmanaged
        {
            fixed (void* w = &wParam, l = &lParam)
            {
                return SendMessage(hWnd, Msg, (WPARAM)(nuint)w, (LPARAM)(nint)l);
            }
        }
    }
}
