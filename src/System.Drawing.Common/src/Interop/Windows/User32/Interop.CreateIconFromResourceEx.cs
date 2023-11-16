// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.User32, SetLastError = true)]
        internal static unsafe partial IntPtr CreateIconFromResourceEx(
#else
        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        internal static extern unsafe IntPtr CreateIconFromResourceEx(
#endif
            byte* pbIconBits,
            uint cbIconBits,
            [MarshalAs(UnmanagedType.Bool)] bool fIcon,
            int dwVersion,
            int csDesired,
            int cyDesired,
            int flags);
    }
}
