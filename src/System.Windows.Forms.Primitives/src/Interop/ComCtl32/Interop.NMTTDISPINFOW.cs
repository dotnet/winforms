// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct NMTTDISPINFOW
        {
            public User32.NMHDR hdr;
            public IntPtr lpszText;
            public fixed char szText[80];
            public IntPtr hinst;
            public TTF uFlags;
            public IntPtr lParam;
        }
    }
}
