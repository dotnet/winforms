// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal partial class Interop
{
    internal static partial class Richedit
    {
        public enum EM
        {
            CANPASTE = (int)WM.USER + 50,
            DISPLAYBAND = (int)WM.USER + 51,
            EXGETSEL = (int)WM.USER + 52,
            EXLIMITTEXT = (int)WM.USER + 53,
            EXLINEFROMCHAR = (int)WM.USER + 54,
            EXSETSEL = (int)WM.USER + 55,
            FINDTEXT = (int)WM.USER + 56,
            FORMATRANGE = (int)WM.USER + 57,
            GETCHARFORMAT = (int)WM.USER + 58,
            GETEVENTMASK = (int)WM.USER + 59,
            GETOLEINTERFACE = (int)WM.USER + 60,
            GETPARAFORMAT = (int)WM.USER + 61,
            GETSELTEXT = (int)WM.USER + 62,
            HIDESELECTION = (int)WM.USER + 63,
            PASTESPECIAL = (int)WM.USER + 64,
            REQUESTRESIZE = (int)WM.USER + 65,
            SELECTIONTYPE = (int)WM.USER + 66,
            SETBKGNDCOLOR = (int)WM.USER + 67,
            SETCHARFORMAT = (int)WM.USER + 68,
            SETEVENTMASK = (int)WM.USER + 69,
            SETOLECALLBACK = (int)WM.USER + 70,
            SETPARAFORMAT = (int)WM.USER + 71,
            SETTARGETDEVICE = (int)WM.USER + 72,
            STREAMIN = (int)WM.USER + 73,
            STREAMOUT = (int)WM.USER + 74,
            GETTEXTRANGE = (int)WM.USER + 75,
            FINDWORDBREAK = (int)WM.USER + 76,
            SETOPTIONS = (int)WM.USER + 77,
            GETOPTIONS = (int)WM.USER + 78,
            FINDTEXTEX = (int)WM.USER + 79,
            GETWORDBREAKPROCEX = (int)WM.USER + 80,
            SETWORDBREAKPROCEX = (int)WM.USER + 81,

            // Richedit v2.0 messages
            SETUNDOLIMIT = (int)WM.USER + 82,
            REDO = (int)WM.USER + 84,
            CANREDO = (int)WM.USER + 85,
            GETUNDONAME = (int)WM.USER + 86,
            GETREDONAME = (int)WM.USER + 87,
            STOPGROUPTYPING = (int)WM.USER + 88,

            SETTEXTMODE = (int)WM.USER + 89,
            GETTEXTMODE = (int)WM.USER + 90,

            AUTOURLDETECT = (int)WM.USER + 91,
            GETAUTOURLDETECT = (int)WM.USER + 92,
            SETPALETTE = (int)WM.USER + 93,
            GETTEXTEX = (int)WM.USER + 94,
            GETTEXTLENGTHEX = (int)WM.USER + 95,

            // Asia specific messages
            SETPUNCTUATION = (int)WM.USER + 100,
            GETPUNCTUATION = (int)WM.USER + 101,
            SETWORDWRAPMODE = (int)WM.USER + 102,
            GETWORDWRAPMODE = (int)WM.USER + 103,
            SETIMECOLOR = (int)WM.USER + 104,
            GETIMECOLOR = (int)WM.USER + 105,
            SETIMEOPTIONS = (int)WM.USER + 106,
            GETIMEOPTIONS = (int)WM.USER + 107,
            CONVPOSITION = (int)WM.USER + 108,

            SETLANGOPTIONS = (int)WM.USER + 120,
            GETLANGOPTIONS = (int)WM.USER + 121,
            GETIMECOMPMODE = (int)WM.USER + 122,

            FINDTEXTW = (int)WM.USER + 123,
            FINDTEXTEXW = (int)WM.USER + 124,

            // Rich TextBox 3.0 Asia msgs
            RECONVERSION = (int)WM.USER + 125,
            SETIMEMODEBIAS = (int)WM.USER + 126,
            GETIMEMODEBIAS = (int)WM.USER + 127,

            // BiDi Specific messages
            SETBIDIOPTIONS = (int)WM.USER + 200,
            GETBIDIOPTIONS = (int)WM.USER + 201,

            SETTYPOGRAPHYOPTIONS = (int)WM.USER + 202,
            GETTYPOGRAPHYOPTIONS = (int)WM.USER + 203,

            // Extended TextBox style specific messages
            SETEDITSTYLE = (int)WM.USER + 204,
            GETEDITSTYLE = (int)WM.USER + 205,

            // Ole Objects Disabling message
            SETQUERYRTFOBJ = (int)WM.USER + 270,

            // Pegasus outline mode messages (RE 3.0)

            // Outline mode message
            OUTLINE = (int)WM.USER + 220,

            // Message for getting and restoring scroll pos
            GETSCROLLPOS = (int)WM.USER + 221,
            SETSCROLLPOS = (int)WM.USER + 222,

            // Change fontsize in current selection by wparam
            SETFONTSIZE = (int)WM.USER + 223,
            GETZOOM = (int)WM.USER + 224,
            SETZOOM = (int)WM.USER + 225,
        }
    }
}
