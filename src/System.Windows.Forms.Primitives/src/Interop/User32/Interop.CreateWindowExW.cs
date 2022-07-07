// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, CharSet = CharSet.Unicode, SetLastError = true)]
        public unsafe static extern IntPtr CreateWindowExW(
            WS_EX dwExStyle,
            char* lpClassName,
            string? lpWindowName,
            WS dwStyle,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInst,
            // AsAny is obsoleted, but we currently allow (via CreateParams) specifying arbitrary objects
            // when creating Controls. We could remove this if it is fully removed by manually doing the
            // same conversions that the marshaller currently does.
#pragma warning disable CS0618
            [MarshalAs(UnmanagedType.AsAny)] object? lpParam);
#pragma warning restore CS0618

        public unsafe static IntPtr CreateWindowExW(
            WS_EX dwExStyle,
            string? lpClassName,
            string? lpWindowName,
            WS dwStyle,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInst,
            object? lpParam)
        {
            fixed (char* c = lpClassName)
            {
                return CreateWindowExW(dwExStyle, c, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInst, lpParam);
            }
        }
    }
}
