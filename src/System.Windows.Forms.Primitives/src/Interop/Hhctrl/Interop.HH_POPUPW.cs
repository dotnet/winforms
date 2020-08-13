// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

internal partial class Interop
{
    internal static partial class Hhctl
    {
        public unsafe struct HH_POPUPW
        {
            public int cbStruct;
            public IntPtr hinst;
            public uint idString;
            public char* pszText;
            public Point pt;
            public COLORREF clrForeground;
            public COLORREF clrBackground;
            public RECT rcMargins;
            public char* pszFont;
        }
    }
}
