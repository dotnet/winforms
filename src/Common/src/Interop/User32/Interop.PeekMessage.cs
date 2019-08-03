// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern BOOL PeekMessageA(
            ref MSG msg,
            IntPtr hwnd = default,
            uint msgMin = 0,
            uint msgMax = 0,
            PeekMessageFlags remove = PeekMessageFlags.PM_NOREMOVE);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern BOOL PeekMessageW(
            ref MSG msg,
            IntPtr hwnd = default,
            uint msgMin = 0,
            uint msgMax = 0,
            PeekMessageFlags remove = PeekMessageFlags.PM_NOREMOVE);

        [Flags]
        public enum PeekMessageFlags : uint
        {
            PM_NOREMOVE = 0x0000,
            PM_REMOVE = 0x0001,
            PM_NOYIELD = 0x0002
        }
    }
}
