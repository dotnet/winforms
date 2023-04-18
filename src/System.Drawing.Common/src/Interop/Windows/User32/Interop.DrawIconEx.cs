// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

internal static partial class Interop
{
    internal static partial class User32
    {
        [return: MarshalAs(UnmanagedType.Bool)]
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.User32, SetLastError = true)]
        internal static partial bool DrawIconEx(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern bool DrawIconEx(
#endif
            HandleRef hDC,
            int x,
            int y,
#if NET7_0_OR_GREATER
            [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
            HandleRef hIcon,
            int width,
            int height,
            int iStepIfAniCursor,
#if NET7_0_OR_GREATER
            [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
            HandleRef hBrushFlickerFree,
            int diFlags);
    }
}
