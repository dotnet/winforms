// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum CCM : uint
        {
            FIRST = 0x2000,
            SETBKCOLOR = FIRST + 1,
            SETCOLORSCHEME = FIRST + 2,
            GETCOLORSCHEME = FIRST + 3,
            GETDROPTARGET = FIRST + 4,
            SETUNICODEFORMAT = FIRST + 5,
            GETUNICODEFORMAT = FIRST + 6,
            SETVERSION = FIRST + 0x7,
            GETVERSION = FIRST + 0x8,
            SETNOTIFYWINDOW = FIRST + 0x9,
            SETWINDOWTHEME = FIRST + 0xB,
            DPISCALE = FIRST + 0xC,
        }
    }
}
