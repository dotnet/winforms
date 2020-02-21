// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        public enum TVM : uint
        {
            FIRST = 0x1100,
            DELETEITEM = FIRST + 1,
            EXPAND = FIRST + 2,
            GETITEMRECT = FIRST + 4,
            GETCOUNT = FIRST + 5,
            GETINDENT = FIRST + 6,
            SETINDENT = FIRST + 7,
            GETIMAGELIST = FIRST + 8,
            SETIMAGELIST = FIRST + 9,
            GETNEXTITEM = FIRST + 10,
            SELECTITEM = FIRST + 11,
            GETEDITCONTROL = FIRST + 15,
            GETVISIBLECOUNT = FIRST + 16,
            HITTEST = FIRST + 17,
            CREATEDRAGIMAGE = FIRST + 18,
            SORTCHILDREN = FIRST + 19,
            ENSUREVISIBLE = FIRST + 20,
            SORTCHILDRENCB = FIRST + 21,
            ENDEDITLABELNOW = FIRST + 22,
            SETTOOLTIPS = FIRST + 24,
            GETTOOLTIPS = FIRST + 25,
            SETINSERTMARK = FIRST + 26,
            SETITEMHEIGHT = FIRST + 27,
            GETITEMHEIGHT = FIRST + 28,
            SETBKCOLOR = FIRST + 29,
            SETTEXTCOLOR = FIRST + 30,
            GETBKCOLOR = FIRST + 31,
            GETTEXTCOLOR = FIRST + 32,
            SETSCROLLTIME = FIRST + 33,
            GETSCROLLTIME = FIRST + 34,
            SETBORDER = FIRST + 35,
            SETINSERTMARKCOLOR = FIRST + 37,
            GETINSERTMARKCOLOR = FIRST + 38,
            GETITEMSTATE = FIRST + 39,
            SETLINECOLOR = FIRST + 40,
            GETLINECOLOR = FIRST + 41,
            MAPACCIDTOHTREEITEM = FIRST + 42,
            MAPHTREEITEMTOACCID = FIRST + 43,
            SETEXTENDEDSTYLE = FIRST + 44,
            GETEXTENDEDSTYLE = FIRST + 45,
            INSERTITEMW = FIRST + 50,
            SETHOT = FIRST + 58,
            SETAUTOSCROLLINFO = FIRST + 59,
            GETITEMW = FIRST + 62,
            SETITEMW = FIRST + 63,
            GETISEARCHSTRINGW = FIRST + 64,
            EDITLABELW = FIRST + 65,
            GETSELECTEDCOUNT = FIRST + 70,
            SHOWINFOTIP = FIRST + 71,
            GETITEMPARTRECT = FIRST + 72,
            SETUNICODEFORMAT = CCM.SETUNICODEFORMAT,
            GETUNICODEFORMAT = CCM.GETUNICODEFORMAT,
        }
    }
}
