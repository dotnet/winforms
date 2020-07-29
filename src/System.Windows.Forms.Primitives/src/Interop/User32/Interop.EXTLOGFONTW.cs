// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct EXTLOGFONTW
        {
            public const int LF_FULLFACESIZE = 64;
            public const int ELF_VENDOR_SIZE = 4;

            public LOGFONTW elfLogFont;
            private fixed char _elfFullName[LF_FULLFACESIZE];
            private fixed char _elfStyle[LOGFONTW.LF_FACESIZE];
            public uint elfVersion;
            public uint elfStyleSize;
            public uint elfMatch;
            public uint elfReserved;
            public fixed char _elfVendorId[ELF_VENDOR_SIZE];
            public uint elfCulture;
            public PANOSE elfPanose;

            public ReadOnlySpan<char> elfFullName
            {
                get { fixed (char* c = _elfFullName) { return new Span<char>(c, LF_FULLFACESIZE).SliceAtFirstNull(); } }
            }

            public ReadOnlySpan<char> elfStyle
            {
                get { fixed (char* c = _elfStyle) { return new Span<char>(c, LOGFONTW.LF_FACESIZE).SliceAtFirstNull(); } }
            }
        }
    }
}
