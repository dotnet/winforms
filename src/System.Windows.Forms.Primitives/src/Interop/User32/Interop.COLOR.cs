// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public enum COLOR : int
        {
            SCROLLBAR               = 0,
            BACKGROUND              = 1,
            ACTIVECAPTION           = 2,
            INACTIVECAPTION         = 3,
            MENU                    = 4,
            WINDOW                  = 5,
            WINDOWFRAME             = 6,
            MENUTEXT                = 7,
            WINDOWTEXT              = 8,
            CAPTIONTEXT             = 9,
            ACTIVEBORDER            = 10,
            INACTIVEBORDER          = 11,
            APPWORKSPACE            = 12,
            HIGHLIGHT               = 13,
            HIGHLIGHTTEXT           = 14,
            BTNFACE                 = 15,
            BTNSHADOW               = 16,
            GRAYTEXT                = 17,
            BTNTEXT                 = 18,
            INACTIVECAPTIONTEXT     = 19,
            BTNHIGHLIGHT            = 20,
            DKSHADOW3D              = 21,
            LIGHT3D                 = 22,
            INFOTEXT                = 23,
            INFOBK                  = 24,
            HOTLIGHT                = 26,
            GRADIENTACTIVECAPTION   = 27,
            GRADIENTINACTIVECAPTION = 28,
            MENUHILIGHT             = 29,
            MENUBAR                 = 30,
            DESKTOP                 = BACKGROUND,
            FACE3D                  = BTNFACE,
            SHADOW3D                = BTNSHADOW,
            HIGHLIGHT3D             = BTNHIGHLIGHT,
            HILIGHT3D               = BTNHIGHLIGHT,
            BTNHILIGHT              = BTNHIGHLIGHT
        }
    }
}
