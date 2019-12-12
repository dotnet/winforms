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
        public unsafe struct LVITEMW
        {
            public LVIF mask;
            public int iItem;
            public int iSubItem;
            public LVIS state;
            public LVIS stateMask;
            public char* /* LPWSTR */ pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns; // tile view columns
            public IntPtr puColumns;
            public LVCFMT* piColFmt;
            public int iGroup; // readonly. only valid for owner data.
        }
    }
}
