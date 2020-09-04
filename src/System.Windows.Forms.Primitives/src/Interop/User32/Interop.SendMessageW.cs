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
        public static extern IntPtr SendMessageW(
            IntPtr hWnd,
            WM Msg,
            IntPtr wParam = default,
            IntPtr lParam = default);

        public static IntPtr SendMessageW(
            HandleRef hWnd,
            WM Msg,
            IntPtr wParam = default,
            IntPtr lParam = default)
        {
            IntPtr result = SendMessageW(hWnd.Handle, Msg, wParam, lParam);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public static IntPtr SendMessageW(
            IHandle hWnd,
            WM Msg,
            IntPtr wParam = default,
            IntPtr lParam = default)
        {
            IntPtr result = SendMessageW(hWnd.Handle, Msg, wParam, lParam);
            GC.KeepAlive(hWnd);
            return result;
        }

        public unsafe static IntPtr SendMessageW(
            IntPtr hWnd,
            WM Msg,
            IntPtr wParam,
            string lParam)
        {
            fixed (char* c = lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (IntPtr)(void*)c);
            }
        }

        public unsafe static IntPtr SendMessageW(
            HandleRef hWnd,
            WM Msg,
            IntPtr wParam,
            string lParam)
        {
            fixed (char* c = lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (IntPtr)(void*)c);
            }
        }

        public unsafe static IntPtr SendMessageW(
            IHandle hWnd,
            WM Msg,
            IntPtr wParam,
            string lParam)
        {
            fixed (char* c = lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (IntPtr)(void*)c);
            }
        }

        public unsafe static IntPtr SendMessageW<T>(
            IntPtr hWnd,
            WM Msg,
            IntPtr wParam,
            ref T lParam) where T : unmanaged
        {
            fixed (void* l = &lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (IntPtr)l);
            }
        }

        public unsafe static IntPtr SendMessageW<T>(
            IHandle hWnd,
            WM Msg,
            IntPtr wParam,
            ref T lParam) where T : unmanaged
        {
            fixed (void* l = &lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (IntPtr)l);
            }
        }

        public unsafe static IntPtr SendMessageW<TWParam, TLParam>(
            IHandle hWnd,
            WM Msg,
            ref TWParam wParam,
            ref TLParam lParam)
            where TWParam : unmanaged
            where TLParam : unmanaged
        {
            fixed (void* w = &wParam, l = &lParam)
            {
                return SendMessageW(hWnd, Msg, (IntPtr)w, (IntPtr)l);
            }
        }
    }
}
