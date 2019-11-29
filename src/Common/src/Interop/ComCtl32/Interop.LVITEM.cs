// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public LVIF mask;
            public int iItem;
            public int iSubItem;
            public LVIS state;
            public LVIS stateMask;
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns; // tile view columns
            public IntPtr puColumns;

            public override string ToString()
            {
                return "LVITEM: pszText = " + pszText
                     + ", iItem = " + iItem.ToString(CultureInfo.InvariantCulture)
                     + ", iSubItem = " + iSubItem.ToString(CultureInfo.InvariantCulture)
                     + ", state = " + state.ToString(CultureInfo.InvariantCulture)
                     + ", iGroupId = " + iGroupId.ToString(CultureInfo.InvariantCulture)
                     + ", cColumns = " + cColumns.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
