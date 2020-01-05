// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public delegate bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam);

        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        public static extern BOOL EnumWindows(EnumWindowsCallback callback, IntPtr extraData);
    }
}
