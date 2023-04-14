// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Drawing.Interop;
using System.Runtime.InteropServices;
#if NET7_0_OR_GREATER
using System.Runtime.InteropServices.Marshalling;
#endif

internal static partial class Interop
{
    internal static partial class Gdi32
    {
#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32, EntryPoint = "GetObjectW", SetLastError = true)]
        internal static partial int GetObject(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.Gdi32, SetLastError = true)]
        internal static extern int GetObject(
#endif
            HandleRef hObject,
            int nSize,
            ref BITMAP bm);

#if NET7_0_OR_GREATER
        [LibraryImport(Libraries.Gdi32, EntryPoint = "GetObjectW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        internal static partial int GetObject(
            [MarshalUsing(typeof(HandleRefMarshaller))]
#else
        [DllImport(Libraries.Gdi32, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern int GetObject(
#endif
            HandleRef hObject,
            int nSize,
            ref LOGFONT lf);

        internal static unsafe int GetObject(
#if NET7_0_OR_GREATER
            [MarshalUsing(typeof(HandleRefMarshaller))]
#endif
            HandleRef hObject,
            ref LOGFONT lp)
            => GetObject(hObject, sizeof(LOGFONT), ref lp);

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public uint bmType;
            public uint bmWidth;
            public uint bmHeight;
            public uint bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public IntPtr bmBits;
        }
    }
}
