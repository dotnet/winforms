// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TCM : uint
        {
            FIRST = 0x1300,
            GETIMAGELIST = FIRST + 2,
            SETIMAGELIST = FIRST + 3,
            GETITEMCOUNT = FIRST + 4,
            GETITEMA = FIRST + 5,
            SETITEMA = FIRST + 6,
            INSERTITEMA = FIRST + 7,
            DELETEITEM = FIRST + 8,
            DELETEALLITEMS = FIRST + 9,
            GETITEMRECT = FIRST + 10,
            GETCURSEL = FIRST + 11,
            SETCURSEL = FIRST + 12,
            HITTEST = FIRST + 13,
            SETITEMEXTRA = FIRST + 14,
            ADJUSTRECT = FIRST + 40,
            SETITEMSIZE = FIRST + 41,
            REMOVEIMAGE = FIRST + 42,
            SETPADDING = FIRST + 43,
            GETROWCOUNT = FIRST + 44,
            GETTOOLTIPS = FIRST + 45,
            SETTOOLTIPS = FIRST + 46,
            GETCURFOCUS = FIRST + 47,
            SETCURFOCUS = FIRST + 48,
            SETMINTABWIDTH = FIRST + 49,
            DESELECTALL = FIRST + 50,
            HIGHLIGHTITEM = FIRST + 51,
            SETEXTENDEDSTYLE = FIRST + 52,
            GETEXTENDEDSTYLE = FIRST + 53,
            SETUNICODEFORMAT = CCM.SETUNICODEFORMAT,
            GETUNICODEFORMAT = CCM.GETUNICODEFORMAT,
            GETITEMW = FIRST + 60,
            SETITEMW = FIRST + 61,
            INSERTITEMW = FIRST + 62,
        }
    }
}
