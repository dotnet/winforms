// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        /// <summary>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/commctrl/ns-commctrl-mchittestinfo">MCHITTESTINFO structure (Microsoft Docs)</see>
        /// </summary>
        public struct MCHITTESTINFO
        {
            public uint cbSize;
            public Point pt;
            public MCHT uHit;
            public Kernel32.SYSTEMTIME st;
            public RECT rc;
            public int iOffset;
            public int iRow;
            public int iCol;
        }
    }
}
