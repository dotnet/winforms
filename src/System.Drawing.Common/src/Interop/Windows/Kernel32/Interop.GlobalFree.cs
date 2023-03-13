// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Kernel32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Kernel32, SetLastError = true)]
        public static partial IntPtr GlobalFree(
#else
        [DllImport(Libraries.Kernel32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GlobalFree(
#endif
            IntPtr handle);

        public static IntPtr GlobalFree(HandleRef handle)
        {
            IntPtr result = GlobalFree(handle.Handle);
            GC.KeepAlive(handle.Wrapper);
            return result;
        }
    }
}
