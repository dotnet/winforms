// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static unsafe extern int GetClassNameW(IntPtr hWnd, char*lpClassName, int nMaxCount);

        private const int MaxClassNameLength = 256;

        public static unsafe string GetClassName(IntPtr hWnd)
        {
            char* className = stackalloc char[MaxClassNameLength];
            int count = GetClassNameW(hWnd, className, MaxClassNameLength);
            return new string(className, 0, count);
        }
    }
}
