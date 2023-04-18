// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Interop;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32)]
        public static partial nint CreateFontIndirectW(
#else
        [DllImport(Libraries.Gdi32, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern nint CreateFontIndirectW(
#endif
            ref LOGFONT lplf);
    }
}
