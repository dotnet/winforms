// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        // HDN_ITEMCHANGING will send us an HDITEM w/ pszText set to some random pointer.
        // Marshal.PtrToStructure chokes when it has to convert a random pointer to a string.
        // For HDN_ITEMCHANGING map pszText to an IntPtr
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HDITEM2
        {
            public int mask = 0;
            public int cxy = 0;
            public IntPtr pszText_notUsed = IntPtr.Zero;
            public IntPtr hbm = IntPtr.Zero;
            public int cchTextMax = 0;
            public int fmt = 0;
            public IntPtr lParam = IntPtr.Zero;
            public int iImage = 0;
            public int iOrder = 0;
            public int type = 0;
            public IntPtr pvFilter = IntPtr.Zero;
        }
    }
}
