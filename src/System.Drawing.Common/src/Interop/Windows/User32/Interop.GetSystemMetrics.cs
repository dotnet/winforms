// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.User32, SetLastError = true)]
        public static partial int GetSystemMetrics(
#else
        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true)]
        public static extern int GetSystemMetrics(
#endif
            int nIndex);
    }
}
