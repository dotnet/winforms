// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum HT
        {
            ERROR = -2,
            TRANSPARENT = -1,
            NOWHERE = 0,
            CLIENT = 1,
            CAPTION = 2,
            SYSMENU = 3,
            GROWBOX = 4,
            SIZE = GROWBOX,
            MENU = 5,
            HSCROLL = 6,
            VSCROLL = 7,
            MINBUTTON = 8,
            MAXBUTTON = 9,
            LEFT = 10,
            RIGHT = 11,
            TOP = 12,
            TOPLEFT = 13,
            TOPRIGHT = 14,
            BOTTOM = 15,
            BOTTOMLEFT = 16,
            BOTTOMRIGHT = 17,
            BORDER = 18,
            REDUCE = MINBUTTON,
            ZOOM = MAXBUTTON,
            SIZEFIRST = LEFT,
            SIZELAST = BOTTOMRIGHT,
            OBJECT = 19,
            CLOSE = 20,
            HELP = 21
        }
    }
}
