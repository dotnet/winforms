// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    /// <summary>
    ///  RichTextBox Control Messages. Note that some messages have the same name but different value compared to normal Edit Control Messages.
    ///  Copied form richedit.h
    /// </summary>
    public static class RichEditMessages
    {
        public const int EM_CANPASTE = WindowMessages.WM_USER + 50;
        public const int EM_DISPLAYBAND = WindowMessages.WM_USER + 51;
        public const int EM_EXGETSEL = WindowMessages.WM_USER + 52;
        public const int EM_EXLIMITTEXT = WindowMessages.WM_USER + 53;
        public const int EM_EXLINEFROMCHAR = WindowMessages.WM_USER + 54;
        public const int EM_EXSETSEL = WindowMessages.WM_USER + 55;
        public const int EM_FINDTEXT = WindowMessages.WM_USER + 56;
        public const int EM_FORMATRANGE = WindowMessages.WM_USER + 57;
        public const int EM_GETCHARFORMAT = WindowMessages.WM_USER + 58;
        public const int EM_GETEVENTMASK = WindowMessages.WM_USER + 59;
        public const int EM_GETOLEINTERFACE = WindowMessages.WM_USER + 60;
        public const int EM_GETPARAFORMAT = WindowMessages.WM_USER + 61;
        public const int EM_GETSELTEXT = WindowMessages.WM_USER + 62;
        public const int EM_HIDESELECTION = WindowMessages.WM_USER + 63;
        public const int EM_PASTESPECIAL = WindowMessages.WM_USER + 64;
        public const int EM_REQUESTRESIZE = WindowMessages.WM_USER + 65;
        public const int EM_SELECTIONTYPE = WindowMessages.WM_USER + 66;
        public const int EM_SETBKGNDCOLOR = WindowMessages.WM_USER + 67;
        public const int EM_SETCHARFORMAT = WindowMessages.WM_USER + 68;
        public const int EM_SETEVENTMASK = WindowMessages.WM_USER + 69;
        public const int EM_SETOLECALLBACK = WindowMessages.WM_USER + 70;
        public const int EM_SETPARAFORMAT = WindowMessages.WM_USER + 71;
        public const int EM_SETTARGETDEVICE = WindowMessages.WM_USER + 72;
        public const int EM_STREAMIN = WindowMessages.WM_USER + 73;
        public const int EM_STREAMOUT = WindowMessages.WM_USER + 74;
        public const int EM_GETTEXTRANGE = WindowMessages.WM_USER + 75;
        public const int EM_FINDWORDBREAK = WindowMessages.WM_USER + 76;
        public const int EM_SETOPTIONS = WindowMessages.WM_USER + 77;
        public const int EM_GETOPTIONS = WindowMessages.WM_USER + 78;
        public const int EM_FINDTEXTEX = WindowMessages.WM_USER + 79;
        public const int EM_GETWORDBREAKPROCEX = WindowMessages.WM_USER + 80;
        public const int EM_SETWORDBREAKPROCEX = WindowMessages.WM_USER + 81;

        // Richedit v2.0 messages
        public const int EM_SETUNDOLIMIT = WindowMessages.WM_USER + 82;
        public const int EM_REDO = WindowMessages.WM_USER + 84;
        public const int EM_CANREDO = WindowMessages.WM_USER + 85;
        public const int EM_GETUNDONAME = WindowMessages.WM_USER + 86;
        public const int EM_GETREDONAME = WindowMessages.WM_USER + 87;
        public const int EM_STOPGROUPTYPING = WindowMessages.WM_USER + 88;

        public const int EM_SETTEXTMODE = WindowMessages.WM_USER + 89;
        public const int EM_GETTEXTMODE = WindowMessages.WM_USER + 90;

        public const int EM_AUTOURLDETECT = WindowMessages.WM_USER + 91;
        public const int EM_GETAUTOURLDETECT = WindowMessages.WM_USER + 92;
        public const int EM_SETPALETTE = WindowMessages.WM_USER + 93;
        public const int EM_GETTEXTEX = WindowMessages.WM_USER + 94;
        public const int EM_GETTEXTLENGTHEX = WindowMessages.WM_USER + 95;

        // Asia specific messages
        public const int EM_SETPUNCTUATION = WindowMessages.WM_USER + 100;
        public const int EM_GETPUNCTUATION = WindowMessages.WM_USER + 101;
        public const int EM_SETWORDWRAPMODE = WindowMessages.WM_USER + 102;
        public const int EM_GETWORDWRAPMODE = WindowMessages.WM_USER + 103;
        public const int EM_SETIMECOLOR = WindowMessages.WM_USER + 104;
        public const int EM_GETIMECOLOR = WindowMessages.WM_USER + 105;
        public const int EM_SETIMEOPTIONS = WindowMessages.WM_USER + 106;
        public const int EM_GETIMEOPTIONS = WindowMessages.WM_USER + 107;
        public const int EM_CONVPOSITION = WindowMessages.WM_USER + 108;

        public const int EM_SETLANGOPTIONS = WindowMessages.WM_USER + 120;
        public const int EM_GETLANGOPTIONS = WindowMessages.WM_USER + 121;
        public const int EM_GETIMECOMPMODE = WindowMessages.WM_USER + 122;

        public const int EM_FINDTEXTW = WindowMessages.WM_USER + 123;
        public const int EM_FINDTEXTEXW = WindowMessages.WM_USER + 124;

        // Rich TextBox 3.0 Asia msgs
        public const int EM_RECONVERSION = WindowMessages.WM_USER + 125;
        public const int EM_SETIMEMODEBIAS = WindowMessages.WM_USER + 126;
        public const int EM_GETIMEMODEBIAS = WindowMessages.WM_USER + 127;

        // BiDi Specific messages
        public const int EM_SETBIDIOPTIONS = WindowMessages.WM_USER + 200;
        public const int EM_GETBIDIOPTIONS = WindowMessages.WM_USER + 201;

        public const int EM_SETTYPOGRAPHYOPTIONS = WindowMessages.WM_USER + 202;
        public const int EM_GETTYPOGRAPHYOPTIONS = WindowMessages.WM_USER + 203;

        // Extended TextBox style specific messages
        public const int EM_SETEDITSTYLE = WindowMessages.WM_USER + 204;
        public const int EM_GETEDITSTYLE = WindowMessages.WM_USER + 205;

        // Ole Objects Disabling message
        public const int EM_SETQUERYRTFOBJ = WindowMessages.WM_USER + 270;

        // Pegasus outline mode messages (RE 3.0)

        // Outline mode message
        public const int EM_OUTLINE = WindowMessages.WM_USER + 220;

        // Message for getting and restoring scroll pos
        public const int EM_GETSCROLLPOS = WindowMessages.WM_USER + 221;
        public const int EM_SETSCROLLPOS = WindowMessages.WM_USER + 222;

        // Change fontsize in current selection by wparam
        public const int EM_SETFONTSIZE = WindowMessages.WM_USER + 223;
        public const int EM_GETZOOM = WindowMessages.WM_USER + 224;
        public const int EM_SETZOOM = WindowMessages.WM_USER + 225;
    }
}
