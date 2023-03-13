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
        public static partial IntPtr GlobalLock(
#else
        [DllImport(Libraries.Kernel32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GlobalLock(
#endif
            IntPtr hMem);

        public static IntPtr GlobalLock(HandleRef hMem)
        {
            IntPtr result = GlobalLock(hMem.Handle);
            GC.KeepAlive(hMem.Wrapper);
            return result;
        }

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Kernel32)]
        public static partial IntPtr GlobalUnlock(
#else
        [DllImport(Libraries.Kernel32, ExactSpelling = true)]
        public static extern IntPtr GlobalUnlock(
#endif
            IntPtr hMem);

        public static IntPtr GlobalUnlock(HandleRef hMem)
        {
            IntPtr result = GlobalUnlock(hMem.Handle);
            GC.KeepAlive(hMem.Wrapper);
            return result;
        }
    }
}
