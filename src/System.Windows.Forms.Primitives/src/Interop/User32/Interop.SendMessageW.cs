// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern nint SendMessageW(
            HWND hWnd,
            WM Msg,
            nint wParam = default,
            nint lParam = default);

        public static nint SendMessageW(
            IHandle hWnd,
            WM Msg,
            nint wParam = default,
            nint lParam = default)
        {
            nint result = SendMessageW((HWND)hWnd.Handle, Msg, wParam, lParam);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static nint SendMessageW<T>(
            T hWnd,
            WM Msg,
            nint wParam = default,
            nint lParam = default) where T : IHandle<HWND>
        {
            nint result = SendMessageW(hWnd.Handle, Msg, wParam, lParam);
            GC.KeepAlive(hWnd);
            return result;
        }

        public unsafe static nint SendMessageW(
            HWND hWnd,
            WM Msg,
            nint wParam,
            string? lParam)
        {
            fixed (char* c = lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (nint)c);
            }
        }

        public unsafe static nint SendMessageW(
            IHandle hWnd,
            WM Msg,
            nint wParam,
            string? lParam)
        {
            fixed (char* c = lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (nint)c);
            }
        }

        public unsafe static nint SendMessageW<T>(
            HWND hWnd,
            WM Msg,
            nint wParam,
            ref T lParam) where T : unmanaged
        {
            fixed (void* l = &lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (nint)l);
            }
        }

        public unsafe static nint SendMessageW<T>(
            IHandle hWnd,
            WM Msg,
            nint wParam,
            ref T lParam) where T : unmanaged
        {
            fixed (void* l = &lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (nint)l);
            }
        }

        public unsafe static nint SendMessageW<TWParam, TLParam>(
            IHandle hWnd,
            WM Msg,
            ref TWParam wParam,
            ref TLParam lParam)
            where TWParam : unmanaged
            where TLParam : unmanaged
        {
            fixed (void* w = &wParam, l = &lParam)
            {
                return SendMessageW(hWnd, Msg, (nint)w, (nint)l);
            }
        }
    }
}
