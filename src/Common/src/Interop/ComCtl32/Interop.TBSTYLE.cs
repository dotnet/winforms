// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [Flags]
        public enum TBSTYLE : int
        {
            BUTTON = 0x0000,
            SEP = 0x0001,
            CHECK = 0x0002,
            GROUP = 0x0004,
            CHECKGROUP = GROUP,
            DROPDOWN = 0x0008,
            AUTOSIZE = 0x0010,
            NOPREFIX = 0x0020,
            TOOLTIPS = 0x0100,
            WRAPABLE = 0x0200,
            ALTDRAG = 0x0400,
            FLAT = 0x0800,
            LIST = 0x1000,
            CUSTOMERASE = 0x2000,
            REGISTERDROP = 0x4000,
            TRANSPARENT = 0x800,
            EX_DRAWDDARROWS = 0x00000001,
            EX_MIXEDBUTTONS = 0x00000008,
            EX_HIDECLIPPEDBUTTONS = 0x00000010,
            EX_MULTICOLUMN = 0x00000002,
            EX_VERTICAL = 0x00000004,
            EX_DOUBLEBUFFER = 0x00000080,
        }
    }
}
