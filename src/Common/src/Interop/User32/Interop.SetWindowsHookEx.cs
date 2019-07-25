// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum WindowHookProcedure : int
        {
            WH_JOURNALPLAYBACK = 1,
            WH_GETMESSAGE = 3,
            WH_MOUSE = 7,
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport(Libraries.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowsHookExW(WindowHookProcedure idHook, HookProc lpfn, IntPtr hmod, int dwThreadId);
    }
}
