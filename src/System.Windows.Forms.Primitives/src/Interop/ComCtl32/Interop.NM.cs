// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum NM : int
        {
            FIRST = 0 - 0,
            OUTOFMEMORY = FIRST - 1,
            CLICK = FIRST - 2,
            DBLCLK = FIRST - 3,
            RETURN = FIRST - 4,
            RCLICK = FIRST - 5,
            RDBLCLK = FIRST - 6,
            SETFOCUS = FIRST - 7,
            KILLFOCUS = FIRST - 8,
            CUSTOMDRAW = FIRST - 12,
            HOVER = FIRST - 13,
            NCHITTEST = FIRST - 14,
            KEYDOWN = FIRST - 15,
            RELEASEDCAPTURE = FIRST - 16,
            SETCURSOR = FIRST - 17,
            CHAR = FIRST - 18,
            TOOLTIPSCREATED = FIRST - 19,
            LDOWN = FIRST - 20,
            RDOWN = FIRST - 21,
            THEMECHANGED = FIRST - 22,
            FONTCHANGED = FIRST - 23,
            CUSTOMTEXT = FIRST - 24,
            TVSTATEIMAGECHANGING = FIRST - 24,
        }
    }
}
