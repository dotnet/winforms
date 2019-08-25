// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

internal static partial class Interop
{
    public static partial class Shell32
    {
        [DllImport(Libraries.Shell32, ExactSpelling = true)]
        public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, out CoTaskMemSafeHandle ppidl);
    }
}
