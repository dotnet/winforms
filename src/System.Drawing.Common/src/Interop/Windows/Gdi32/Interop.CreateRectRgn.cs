// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32)]
        public static partial IntPtr CreateRectRgn(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true)]
        public static extern IntPtr CreateRectRgn(
#endif
            int x1,
            int y1,
            int x2,
            int y2);
    }
}
