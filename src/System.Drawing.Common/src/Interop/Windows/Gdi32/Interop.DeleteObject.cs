// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        [return: MarshalAs(UnmanagedType.Bool)]
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32)]
        public static partial bool DeleteObject(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern bool DeleteObject(
#endif
            IntPtr ho);

        public static bool DeleteObject(HandleRef ho)
        {
            bool result = DeleteObject(ho.Handle);
            GC.KeepAlive(ho.Wrapper);
            return result;
        }
    }
}
