// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum SPI : uint
        {
            GETFONTSMOOTHING = 0x004A,
            GETDROPSHADOW = 0x1024,
            GETFLATMENU = 0x1022,
            GETFONTSMOOTHINGTYPE = 0x200A,
            GETFONTSMOOTHINGCONTRAST = 0x200C,
            ICONHORIZONTALSPACING = 0x000D,
            ICONVERTICALSPACING = 0x0018,
            GETICONMETRICS = 0x002D,
            GETICONTITLEWRAP = 0x0019,
            GETKEYBOARDCUES = 0x100A,
            GETKEYBOARDDELAY = 0x0016,
            GETKEYBOARDPREF = 0x0044,
            GETKEYBOARDSPEED = 0x000A,
            GETMOUSEHOVERWIDTH = 0x0062,
            GETMOUSEHOVERHEIGHT = 0x0064,
            GETMOUSEHOVERTIME = 0x0066,
            GETMOUSESPEED = 0x0070,
            GETMENUDROPALIGNMENT = 0x001B,
            GETMENUFADE = 0x1012,
            GETMENUSHOWDELAY = 0x006A,
            GETCOMBOBOXANIMATION = 0x1004,
            GETGRADIENTCAPTIONS = 0x1008,
            GETHOTTRACKING = 0x100E,
            GETLISTBOXSMOOTHSCROLLING = 0x1006,
            GETMENUANIMATION = 0x1002,
            GETSELECTIONFADE = 0x1014,
            GETTOOLTIPANIMATION = 0x1016,
            GETUIEFFECTS = 0x103E,
            GETACTIVEWINDOWTRACKING = 0x1000,
            GETACTIVEWNDTRKTIMEOUT = 0x2002,
            GETANIMATION = 0x0048,
            GETBORDER = 0x0005,
            GETCARETWIDTH = 0x2006,
            GETDRAGFULLWINDOWS = 38,
            GETNONCLIENTMETRICS = 41,
            GETWORKAREA = 48,
            GETHIGHCONTRAST = 66,
            GETDEFAULTINPUTLANG = 89,
            GETSNAPTODEFBUTTON = 95,
            GETWHEELSCROLLLINES = 104,
        }
    }
}
