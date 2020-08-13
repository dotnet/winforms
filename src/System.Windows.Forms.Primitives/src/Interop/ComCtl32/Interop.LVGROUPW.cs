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
        public unsafe struct LVGROUPW
        {
            public uint cbSize;
            public LVGF mask;
            public char* pszHeader;
            public int cchHeader;
            public char* pszFooter;
            public int cchFooter;
            public int iGroupId;
            public LVGS stateMask;
            public LVGS state;
            public LVGA uAlign;

            public char* pszSubtitle;
            public uint cchSubtitle;
            public char* pszTask;
            public uint cchTask;
            public char* pszDescriptionTop;
            public uint cchDescriptionTop;
            public char* pszDescriptionBottom;
            public uint cchDescriptionBottom;
            public int iTitleImage;
            public int iExtendedImage;
            public int iFirstItem;
            public uint cItems;
            public char* pszSubsetTitle;
            public uint cchSubsetTitle;
        }
    }
}
