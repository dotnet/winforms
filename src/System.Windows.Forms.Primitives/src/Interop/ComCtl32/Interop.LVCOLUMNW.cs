// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct LVCOLUMNW
        {
            public LVCF mask;
            public LVCFMT fmt;
            public int cx;
            public char* /* LPWSTR */ pszText;
            public int cchTextMax;
            public int iSubItem;
            public int iImage;
            public int iOrder;
            public int cxMin;
            public int cxDefault;
            public int cxIdeal;
        }
    }
}
