// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum SPI : uint
        {
            GETBORDER = 0x0005,
            GETKEYBOARDSPEED = 0x000A,
            ICONHORIZONTALSPACING = 0x000D,
            SETSCREENSAVEACTIVE = 0x0011,
            SETDESKWALLPAPER = 0x0014,
            GETKEYBOARDDELAY = 0x0016,
            SETKEYBOARDDELAY = 0x0017,
            ICONVERTICALSPACING = 0x0018,
            GETICONTITLEWRAP = 0x0019,
            GETMENUDROPALIGNMENT = 0x001B,
            SETMENUDROPALIGNMENT = 0x001C,
            SETDOUBLECLICKTIME = 0x0020,
            GETDRAGFULLWINDOWS = 0x0026,
            GETNONCLIENTMETRICS = 0x0029,
            GETICONMETRICS = 0x002D,
            GETWORKAREA = 0x0030,
            GETHIGHCONTRAST = 0x0042,
            SETHIGHCONTRAST = 0x0043,
            GETKEYBOARDPREF = 0x0044,
            GETANIMATION = 0x0048,
            GETFONTSMOOTHING = 0x004A,
            SETLOWPOWERACTIVE = 0x0055,
            GETDEFAULTINPUTLANG = 0x0059,
            GETSNAPTODEFBUTTON = 0x005F,
            GETMOUSEHOVERWIDTH = 0x0062,
            GETMOUSEHOVERHEIGHT = 0x0064,
            GETMOUSEHOVERTIME = 0x0066,
            GETWHEELSCROLLLINES = 0x0068,
            GETMENUSHOWDELAY = 0x006A,
            GETMOUSESPEED = 0x0070,
            GETACTIVEWINDOWTRACKING = 0x1000,
            GETMENUANIMATION = 0x1002,
            GETCOMBOBOXANIMATION = 0x1004,
            GETLISTBOXSMOOTHSCROLLING = 0x1006,
            GETGRADIENTCAPTIONS = 0x1008,
            GETKEYBOARDCUES = 0x100A,
            SETKEYBOARDCUES = 0x100B,
            GETHOTTRACKING = 0x100E,
            GETMENUFADE = 0x1012,
            GETSELECTIONFADE = 0x1014,
            GETTOOLTIPANIMATION = 0x1016,
            GETFLATMENU = 0x1022,
            GETDROPSHADOW = 0x1024,
            GETUIEFFECTS = 0x103E,
            GETACTIVEWNDTRKTIMEOUT = 0x2002,
            GETCARETWIDTH = 0x2006,
            GETFONTSMOOTHINGTYPE = 0x200A,
            GETFONTSMOOTHINGCONTRAST = 0x200C,
        }
    }
}
