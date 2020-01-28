// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

internal static partial class Interop
{
    /// <summary>
    ///  RichTextBox Control Messages. Note that some messages have the same name but different value compared to normal Edit Control Messages.
    ///  Copied form richedit.h
    /// </summary>
    public static class RichEditMessages
    {
        public const int EM_CANPASTE = (int)WM.USER + 50;
        public const int EM_DISPLAYBAND = (int)WM.USER + 51;
        public const int EM_EXGETSEL = (int)WM.USER + 52;
        public const int EM_EXLIMITTEXT = (int)WM.USER + 53;
        public const int EM_EXLINEFROMCHAR = (int)WM.USER + 54;
        public const int EM_EXSETSEL = (int)WM.USER + 55;
        public const int EM_FINDTEXT = (int)WM.USER + 56;
        public const int EM_FORMATRANGE = (int)WM.USER + 57;
        public const int EM_GETCHARFORMAT = (int)WM.USER + 58;
        public const int EM_GETEVENTMASK = (int)WM.USER + 59;
        public const int EM_GETOLEINTERFACE = (int)WM.USER + 60;
        public const int EM_GETPARAFORMAT = (int)WM.USER + 61;
        public const int EM_GETSELTEXT = (int)WM.USER + 62;
        public const int EM_HIDESELECTION = (int)WM.USER + 63;
        public const int EM_PASTESPECIAL = (int)WM.USER + 64;
        public const int EM_REQUESTRESIZE = (int)WM.USER + 65;
        public const int EM_SELECTIONTYPE = (int)WM.USER + 66;
        public const int EM_SETBKGNDCOLOR = (int)WM.USER + 67;
        public const int EM_SETCHARFORMAT = (int)WM.USER + 68;
        public const int EM_SETEVENTMASK = (int)WM.USER + 69;
        public const int EM_SETOLECALLBACK = (int)WM.USER + 70;
        public const int EM_SETPARAFORMAT = (int)WM.USER + 71;
        public const int EM_SETTARGETDEVICE = (int)WM.USER + 72;
        public const int EM_STREAMIN = (int)WM.USER + 73;
        public const int EM_STREAMOUT = (int)WM.USER + 74;
        public const int EM_GETTEXTRANGE = (int)WM.USER + 75;
        public const int EM_FINDWORDBREAK = (int)WM.USER + 76;
        public const int EM_SETOPTIONS = (int)WM.USER + 77;
        public const int EM_GETOPTIONS = (int)WM.USER + 78;
        public const int EM_FINDTEXTEX = (int)WM.USER + 79;
        public const int EM_GETWORDBREAKPROCEX = (int)WM.USER + 80;
        public const int EM_SETWORDBREAKPROCEX = (int)WM.USER + 81;

        // Richedit v2.0 messages
        public const int EM_SETUNDOLIMIT = (int)WM.USER + 82;
        public const int EM_REDO = (int)WM.USER + 84;
        public const int EM_CANREDO = (int)WM.USER + 85;
        public const int EM_GETUNDONAME = (int)WM.USER + 86;
        public const int EM_GETREDONAME = (int)WM.USER + 87;
        public const int EM_STOPGROUPTYPING = (int)WM.USER + 88;

        public const int EM_SETTEXTMODE = (int)WM.USER + 89;
        public const int EM_GETTEXTMODE = (int)WM.USER + 90;

        public const int EM_AUTOURLDETECT = (int)WM.USER + 91;
        public const int EM_GETAUTOURLDETECT = (int)WM.USER + 92;
        public const int EM_SETPALETTE = (int)WM.USER + 93;
        public const int EM_GETTEXTEX = (int)WM.USER + 94;
        public const int EM_GETTEXTLENGTHEX = (int)WM.USER + 95;

        // Asia specific messages
        public const int EM_SETPUNCTUATION = (int)WM.USER + 100;
        public const int EM_GETPUNCTUATION = (int)WM.USER + 101;
        public const int EM_SETWORDWRAPMODE = (int)WM.USER + 102;
        public const int EM_GETWORDWRAPMODE = (int)WM.USER + 103;
        public const int EM_SETIMECOLOR = (int)WM.USER + 104;
        public const int EM_GETIMECOLOR = (int)WM.USER + 105;
        public const int EM_SETIMEOPTIONS = (int)WM.USER + 106;
        public const int EM_GETIMEOPTIONS = (int)WM.USER + 107;
        public const int EM_CONVPOSITION = (int)WM.USER + 108;

        public const int EM_SETLANGOPTIONS = (int)WM.USER + 120;
        public const int EM_GETLANGOPTIONS = (int)WM.USER + 121;
        public const int EM_GETIMECOMPMODE = (int)WM.USER + 122;

        public const int EM_FINDTEXTW = (int)WM.USER + 123;
        public const int EM_FINDTEXTEXW = (int)WM.USER + 124;

        // Rich TextBox 3.0 Asia msgs
        public const int EM_RECONVERSION = (int)WM.USER + 125;
        public const int EM_SETIMEMODEBIAS = (int)WM.USER + 126;
        public const int EM_GETIMEMODEBIAS = (int)WM.USER + 127;

        // BiDi Specific messages
        public const int EM_SETBIDIOPTIONS = (int)WM.USER + 200;
        public const int EM_GETBIDIOPTIONS = (int)WM.USER + 201;

        public const int EM_SETTYPOGRAPHYOPTIONS = (int)WM.USER + 202;
        public const int EM_GETTYPOGRAPHYOPTIONS = (int)WM.USER + 203;

        // Extended TextBox style specific messages
        public const int EM_SETEDITSTYLE = (int)WM.USER + 204;
        public const int EM_GETEDITSTYLE = (int)WM.USER + 205;

        // Ole Objects Disabling message
        public const int EM_SETQUERYRTFOBJ = (int)WM.USER + 270;

        // Pegasus outline mode messages (RE 3.0)

        // Outline mode message
        public const int EM_OUTLINE = (int)WM.USER + 220;

        // Message for getting and restoring scroll pos
        public const int EM_GETSCROLLPOS = (int)WM.USER + 221;
        public const int EM_SETSCROLLPOS = (int)WM.USER + 222;

        // Change fontsize in current selection by wparam
        public const int EM_SETFONTSIZE = (int)WM.USER + 223;
        public const int EM_GETZOOM = (int)WM.USER + 224;
        public const int EM_SETZOOM = (int)WM.USER + 225;
    }
}
