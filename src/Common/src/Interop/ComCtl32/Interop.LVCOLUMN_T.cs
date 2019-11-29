// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVCOLUMN_T
        {
            public int mask = 0;
            public int fmt = 0;
            public int cx = 0;
            public string pszText = null;
            public int cchTextMax = 0;
            public int iSubItem = 0;
            public int iImage = 0;
            public int iOrder = 0;
        }
    }
}
