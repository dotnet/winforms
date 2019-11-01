// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class User32
    {
        [Flags]
        public enum DFCS : uint
        {
            CAPTIONCLOSE = 0x0000,
            CAPTIONMIN = 0x0001,
            CAPTIONMAX = 0x0002,
            CAPTIONRESTORE = 0x0003,
            CAPTIONHELP = 0x0004,
            MENUARROW = 0x0000,
            MENUCHECK = 0x0001,
            MENUBULLET = 0x0002,
            MENUARROWRIGHT = 0x0004,
            SCROLLUP = 0x0000,
            SCROLLDOWN = 0x0001,
            SCROLLLEFT = 0x0002,
            SCROLLRIGHT = 0x0003,
            SCROLLCOMBOBOX = 0x0005,
            SCROLLSIZEGRIP = 0x0008,
            SCROLLSIZEGRIPRIGHT = 0x0010,
            BUTTONCHECK = 0x0000,
            BUTTONRADIOIMAGE = 0x0001,
            BUTTONRADIOMASK = 0x0002,
            BUTTONRADIO = 0x0004,
            BUTTON3STATE = 0x0008,
            BUTTONPUSH = 0x0010,
            INACTIVE = 0x0100,
            PUSHED = 0x0200,
            CHECKED = 0x0400,
            TRANSPARENT = 0x0800,
            HOT = 0x1000,
            ADJUSTRECT = 0x2000,
            FLAT = 0x4000,
            MONO = 0x8000
        }
    }
}
