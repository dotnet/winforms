// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    internal static class RichTextBoxConstants
    {
        // flags for enum that we don't want public
        //
        internal const int RTB_HORIZ = 0x0001;
        internal const int RTB_VERT = 0x0002;
        internal const int RTB_FORCE = 0x0010;

        /* RichTextBox messages */

        // Extended edit style masks
        internal const int SES_EMULATESYSEDIT = 1;
        internal const int SES_BEEPONMAXTEXT = 2;
        internal const int SES_EXTENDBACKCOLOR = 4;
        internal const int SES_MAPCPS = 8;
        internal const int SES_EMULATE10 = 16;
        internal const int SES_USECRLF = 32;
        internal const int SES_USEAIMM = 64;
        internal const int SES_NOIME = 128;
        internal const int SES_ALLOWBEEPS = 256;
        internal const int SES_UPPERCASE = 512;
        internal const int SES_LOWERCASE = 1024;
        internal const int SES_NOINPUTSEQUENCECHK = 2048;
        internal const int SES_BIDI = 4096;
        internal const int SES_SCROLLONKILLFOCUS = 8192;
        internal const int SES_XLTCRCRLFTOCR = 16384;

        // Options for EM_SETLANGOPTIONS and EM_GETLANGOPTIONS
        internal const int IMF_AUTOKEYBOARD = 0x0001;
        internal const int IMF_AUTOFONT = 0x0002;
        internal const int IMF_IMECANCELCOMPLETE = 0x0004;   // high completes the comp string when aborting, low cancels.
        internal const int IMF_IMEALWAYSSENDNOTIFY = 0x0008;
        internal const int IMF_AUTOFONTSIZEADJUST = 0x0010;
        internal const int IMF_UIFONTS = 0x0020;
        internal const int IMF_DUALFONT = 0x0080;

        // Values for EM_GETIMECOMPMODE
        internal const int ICM_NOTOPEN = 0x0000;
        internal const int ICM_LEVEL3 = 0x0001;
        internal const int ICM_LEVEL2 = 0x0002;
        internal const int ICM_LEVEL2_5 = 0x0003;
        internal const int ICM_LEVEL2_SUI = 0x0004;

        // Outline mode wparam values
        internal const int EMO_EXIT = 0; // enter normal mode,  lparam ignored
        internal const int EMO_ENTER = 1; // enter outline mode, lparam ignored
        internal const int EMO_PROMOTE = 2; // LOWORD(lparam) == 0 ==>
                                            // promote  to body-text
                                            // LOWORD(lparam) != 0 ==>
                                            // promote/demote current selection
                                            // by indicated number of levels
        internal const int EMO_EXPAND = 3; // HIWORD(lparam) = EMO_EXPANDSELECTION
                                           // -> expands selection to level
                                           // indicated in LOWORD(lparam)
                                           // LOWORD(lparam) = -1/+1 corresponds
                                           // to collapse/expand button presses
                                           // in winword (other values are
                                           // equivalent to having pressed these
                                           // buttons more than once)
                                           // HIWORD(lparam) = EMO_EXPANDDOCUMENT
                                           // -> expands whole document to
                                           // indicated level
        internal const int EMO_MOVESELECTION = 4; // LOWORD(lparam) != 0 -> move current
                                                  // selection up/down by indicated
                                                  // amount
        internal const int EMO_GETVIEWMODE = 5; // Returns VM_NORMAL or VM_OUTLINE

        // EMO_EXPAND options

        internal const int EMO_EXPANDSELECTION = 0;
        internal const int EMO_EXPANDDOCUMENT = 1;
        internal const int VM_NORMAL = 4; // Agrees with RTF \viewkindN
        internal const int VM_OUTLINE = 2;

        // New notifications
        internal const int EN_MSGFILTER = 0x0700;
        internal const int EN_REQUESTRESIZE = 0x0701;
        internal const int EN_SELCHANGE = 0x0702;
        internal const int EN_DROPFILES = 0x0703;
        internal const int EN_PROTECTED = 0x0704;
        internal const int EN_CORRECTTEXT = 0x0705;       /* PenWin specific */
        internal const int EN_STOPNOUNDO = 0x0706;
        internal const int EN_IMECHANGE = 0x0707;       /* Asia specific */
        internal const int EN_SAVECLIPBOARD = 0x0708;
        internal const int EN_OLEOPFAILED = 0x0709;
        internal const int EN_OBJECTPOSITIONS = 0x070a;
        internal const int EN_LINK = 0x070b;
        internal const int EN_DRAGDROPDONE = 0x070c;
        internal const int EN_PARAGRAPHEXPANDED = 0x070d;

        // BiDi specific notifications
        internal const int EN_ALIGNLTR = 0x0710;
        internal const int EN_ALIGNRTL = 0x0711;

        // Event notification masks */
        internal const int ENM_NONE = 0x00000000;
        internal const int ENM_CHANGE = 0x00000001;
        internal const int ENM_UPDATE = 0x00000002;
        internal const int ENM_SCROLL = 0x00000004;
        internal const int ENM_KEYEVENTS = 0x00010000;
        internal const int ENM_MOUSEEVENTS = 0x00020000;
        internal const int ENM_REQUESTRESIZE = 0x00040000;
        internal const int ENM_SELCHANGE = 0x00080000;
        internal const int ENM_DROPFILES = 0x00100000;
        internal const int ENM_PROTECTED = 0x00200000;
        internal const int ENM_CORRECTTEXT = 0x00400000;   /* PenWin specific */
        internal const int ENM_SCROLLEVENTS = 0x00000008;
        internal const int ENM_DRAGDROPDONE = 0x00000010;
        internal const int ENM_PARAGRAPHEXPANDED = 0x00000020;

        /* Asia specific notification mask */
        internal const int ENM_IMECHANGE = 0x00800000;   /* unused by RE2.0 */
        internal const int ENM_LANGCHANGE = 0x01000000;
        internal const int ENM_OBJECTPOSITIONS = 0x02000000;
        internal const int ENM_LINK = 0x04000000;

        /* New edit control styles */
        internal const int ES_SAVESEL = 0x00008000;
        internal const int ES_SUNKEN = 0x00004000;
        internal const int ES_DISABLENOSCROLL = 0x00002000;
        /* same as WS_MAXIMIZE, but that doesn't make sense so we re-use the value */
        internal const int ES_SELECTIONBAR = 0x01000000;
        /* same as ES_UPPERCASE, but re-used to completely disable OLE drag'n'drop */
        internal const int ES_NOOLEDRAGDROP = 0x00000008;

        /* New edit control extended style */
        internal const int ES_EX_NOCALLOLEINIT = 0x01000000;

        /* These flags are used in FE Windows */
        internal const int ES_VERTICAL = 0x00400000; // NOT IN RE3.0/2.0
        internal const int ES_NOIME = 0x00080000;
        internal const int ES_SELFIME = 0x00040000;

        /* TextBox control options */
        internal const int ECO_AUTOWORDSELECTION = 0x00000001;
        internal const int ECO_AUTOVSCROLL = 0x00000040;
        internal const int ECO_AUTOHSCROLL = 0x00000080;
        internal const int ECO_NOHIDESEL = 0x00000100;
        internal const int ECO_READONLY = 0x00000800;
        internal const int ECO_WANTRETURN = 0x00001000;
        internal const int ECO_SAVESEL = 0x00008000;
        internal const int ECO_SELECTIONBAR = 0x01000000; // guessing this is selection margin
        internal const int ECO_VERTICAL = 0x00400000;   /* FE specific */

        /* ECO operations */
        internal const int ECOOP_SET = 0x0001;
        internal const int ECOOP_OR = 0x0002;
        internal const int ECOOP_AND = 0x0003;
        internal const int ECOOP_XOR = 0x0004;

        /* new word break function actions */
        internal const int WB_CLASSIFY = 3;
        internal const int WB_MOVEWORDLEFT = 4;
        internal const int WB_MOVEWORDRIGHT = 5;
        internal const int WB_LEFTBREAK = 6;
        internal const int WB_RIGHTBREAK = 7;

        /* Asia specific flags */
        internal const int WB_MOVEWORDPREV = 4;
        internal const int WB_MOVEWORDNEXT = 5;
        internal const int WB_PREVBREAK = 6;
        internal const int WB_NEXTBREAK = 7;

        internal const int PC_FOLLOWING = 1;
        internal const int PC_LEADING = 2;
        internal const int PC_OVERFLOW = 3;
        internal const int PC_DELIMITER = 4;

        internal const int WBF_WORDWRAP = 0x010;
        internal const int WBF_WORDBREAK = 0x020;
        internal const int WBF_OVERFLOW = 0x040;
        internal const int WBF_LEVEL1 = 0x080;
        internal const int WBF_LEVEL2 = 0x100;
        internal const int WBF_CUSTOM = 0x200;

        /* for use with EM_GET/SETTEXTMODE */
        internal const int TM_PLAINTEXT = 1;
        internal const int TM_RICHTEXT = 2;    /* default behavior */
        internal const int TM_SINGLELEVELUNDO = 4;
        internal const int TM_MULTILEVELUNDO = 8;    /* default behavior */
        internal const int TM_SINGLECODEPAGE = 16;
        internal const int TM_MULTICODEPAGE = 32;   /* default behavior */

        /* Asia specific flags */
        internal const int IMF_FORCENONE = 0x0001;
        internal const int IMF_FORCEENABLE = 0x0002;
        internal const int IMF_FORCEDISABLE = 0x0004;
        internal const int IMF_CLOSESTATUSWINDOW = 0x0008;
        internal const int IMF_VERTICAL = 0x0020;
        internal const int IMF_FORCEACTIVE = 0x0040;
        internal const int IMF_FORCEINACTIVE = 0x0080;
        internal const int IMF_FORCEREMEMBER = 0x0100;
        internal const int IMF_MULTIPLEEDIT = 0x0400;

        /* Word break flags (used with WB_CLASSIFY) */
        internal const int WBF_CLASS = 0x0F;
        internal const int WBF_ISWHITE = 0x10;
        internal const int WBF_BREAKLINE = 0x20;
        internal const int WBF_BREAKAFTER = 0x40;

        internal const int cchTextLimitDefault = 32767;

        /* all paragraph measurements are in twips */
        internal const int lDefaultTab = 720;

        /*
         *  PARAFORMAT numbering options (values for wNumbering):
         *
         *          Numbering Type          Value   Meaning
         *          tomNoNumbering            0             Turn off paragraph numbering
         *          tomNumberAsLCLetter       1             a, b, c, ...
         *          tomNumberAsUCLetter       2             A, B, C, ...
         *          tomNumberAsLCRoman        3             i, ii, iii, ...
         *          tomNumberAsUCRoman        4             I, II, III, ...
         *          tomNumberAsSymbols        5             default is bullet
         *          tomNumberAsNumber         6             0, 1, 2, ...
         *          tomNumberAsSequence       7             tomNumberingStart is first Unicode to use
         *
         *  Other valid Unicode chars are Unicodes for bullets.
         */

        internal const int tomTrue = -1,
                            tomFalse = 0,
                            tomNone = 0,
                            tomUndefined = -9999999,
                            tomAutoColor = -9999997;

        /* used with IRichEditOleCallback::GetContextMenu, this flag will be
           passed as a "selection type".  It indicates that a context menu for
           a right-mouse drag drop should be generated.  The IOleObject parameter
           will really be the IDataObject for the drop
         */
        internal const int GCM_RIGHTMOUSEDROP = 0x8000;

        internal const int OLEOP_DOVERB = 1;

        /*  UndoName info */
        internal const int UID_UNKNOWN = 0;
        internal const int UID_TYPING = 1;
        internal const int UID_DELETE = 2;
        internal const int UID_DRAGDROP = 3;
        internal const int UID_CUT = 4;
        internal const int UID_PASTE = 5;

        /* flags for the GETEXTEX data structure */
        internal const int GT_DEFAULT = 0;
        internal const int GT_USECRLF = 1;

        /* UNICODE embedding character */
        // disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
        internal static readonly char WCH_EMBEDDING = (char)0xFFFC;
#pragma warning restore 0414
    }
}
