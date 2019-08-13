// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern IntPtr SendMessageW(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam);

        public static IntPtr SendMessageW(
            IHandle hWnd,
            uint Msg,
            IntPtr wParam = default,
            IntPtr lParam = default)
        {
            IntPtr result = SendMessageW(hWnd.Handle, Msg, wParam, lParam);
            GC.KeepAlive(hWnd);
            return result;
        }

        public unsafe static IntPtr SendMessageW(
            IHandle hWnd,
            uint Msg,
            IntPtr wParam,
            string lParam)
        {
            fixed (char* c = lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (IntPtr)(void*)c);
            }
        }

        public unsafe static IntPtr SendMessageW(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            RECT lParam)
        {
            return SendMessageW(hWnd, Msg, wParam, (IntPtr)(void*)&lParam);
        }

        public unsafe static IntPtr SendMessageW(
            IHandle hWnd,
            uint Msg,
            IntPtr wParam,
            RECT lParam)
        {
             return SendMessageW(hWnd, Msg, wParam, (IntPtr)(void*)&lParam);
        }
    }
}
