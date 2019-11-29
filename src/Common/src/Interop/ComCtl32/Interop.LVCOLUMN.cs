// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVCOLUMN
        {
            public int mask;
            public int fmt;
            public int cx = 0;
            public IntPtr /* LPWSTR */ pszText;
            public int cchTextMax = 0;
            public int iSubItem = 0;
            public int iImage;
            public int iOrder = 0;
        }
    }
}
