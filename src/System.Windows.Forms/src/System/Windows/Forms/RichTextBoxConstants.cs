// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\RichTextBoxConstants.uex' path='docs/doc[@for="RichTextBoxConstants"]/*' />
    /// <devdoc>
    /// </devdoc>
    internal static class RichTextBoxConstants {

        // flags for enum that we don't want public
        //
        internal const int RTB_HORIZ = 0x0001;
        internal const int RTB_VERT = 0x0002;
        internal const int RTB_FORCE = 0x0010;

        /* Richedit dll name */
        internal const string RICHEDIT_DLL10       = "RichEd32.DLL";
        internal const string RICHEDIT_DLL20       = "RichEd20.DLL";
        internal const string RICHEDIT_DLL30       = RICHEDIT_DLL20;
        internal const string RICHEDIT_DLL41       = "MsftEdit.DLL";

        /* Richedit1.0 Window Class */
        internal const string RICHEDIT_CLASS10A    = "RICHEDIT";

        /* Richedit2.0 Window Class. */
        internal const string RICHEDIT_CLASS20A    = "RichEdit20A";
        internal const string RICHEDIT_CLASS20W    = "RichEdit20W";

        /* Richedit3.0 Window Class */
        internal const string RICHEDIT_CLASS30A    = RICHEDIT_CLASS20A;
        internal const string RICHEDIT_CLASS30W    = RICHEDIT_CLASS20W;

        internal const string DLL_RICHEDIT = RICHEDIT_DLL30;
        internal const string WC_RICHEDITA = RICHEDIT_CLASS30A;
        internal const string WC_RICHEDITW = RICHEDIT_CLASS30W;

        /* Richedit4.1 Window Class */
        internal const string RICHEDIT_CLASS41A     = "RICHEDIT50A";
        internal const string RICHEDIT_CLASS41W     = "RICHEDIT50W";

        internal const string DLL_RICHEDIT_41       = RICHEDIT_DLL41;
        internal const string WC_RICHEDITA_41       = RICHEDIT_CLASS41A;
        internal const string WC_RICHEDITW_41       = RICHEDIT_CLASS41W;

        /* RichTextBox messages */
        internal const int WM_CONTEXTMENU          = 0x007B;

        internal const int WM_PRINTCLIENT          = 0x0318;

        internal const int EM_GETLIMITTEXT         = (NativeMethods.WM_USER + 37);

        internal const int EM_POSFROMCHAR          = (NativeMethods.WM_USER + 38);
        internal const int EM_CHARFROMPOS          = (NativeMethods.WM_USER + 39);

        internal const int EM_SCROLLCARET          = (NativeMethods.WM_USER + 49);
        internal const int EM_CANPASTE             = (NativeMethods.WM_USER + 50);
        internal const int EM_DISPLAYBAND          = (NativeMethods.WM_USER + 51);
        internal const int EM_EXGETSEL             = (NativeMethods.WM_USER + 52);
        internal const int EM_EXLIMITTEXT          = (NativeMethods.WM_USER + 53);
        internal const int EM_EXLINEFROMCHAR       = (NativeMethods.WM_USER + 54);
        internal const int EM_EXSETSEL             = (NativeMethods.WM_USER + 55);
        internal const int EM_FINDTEXT             = (NativeMethods.WM_USER + 56);
        internal const int EM_FORMATRANGE          = (NativeMethods.WM_USER + 57);
        internal const int EM_GETCHARFORMAT        = (NativeMethods.WM_USER + 58);
        internal const int EM_GETEVENTMASK         = (NativeMethods.WM_USER + 59);
        internal const int EM_GETOLEINTERFACE      = (NativeMethods.WM_USER + 60);
        internal const int EM_GETPARAFORMAT        = (NativeMethods.WM_USER + 61);
        internal const int EM_GETSELTEXT           = (NativeMethods.WM_USER + 62);
        internal const int EM_HIDESELECTION        = (NativeMethods.WM_USER + 63);
        internal const int EM_PASTESPECIAL         = (NativeMethods.WM_USER + 64);
        internal const int EM_REQUESTRESIZE        = (NativeMethods.WM_USER + 65);
        internal const int EM_SELECTIONTYPE        = (NativeMethods.WM_USER + 66);
        internal const int EM_SETBKGNDCOLOR        = (NativeMethods.WM_USER + 67);
        internal const int EM_SETCHARFORMAT        = (NativeMethods.WM_USER + 68);
        internal const int EM_SETEVENTMASK         = (NativeMethods.WM_USER + 69);
        internal const int EM_SETOLECALLBACK       = (NativeMethods.WM_USER + 70);
        internal const int EM_SETPARAFORMAT        = (NativeMethods.WM_USER + 71);
        internal const int EM_SETTARGETDEVICE      = (NativeMethods.WM_USER + 72);
        internal const int EM_STREAMIN             = (NativeMethods.WM_USER + 73);
        internal const int EM_STREAMOUT            = (NativeMethods.WM_USER + 74);
        internal const int EM_GETTEXTRANGE         = (NativeMethods.WM_USER + 75);
        internal const int EM_FINDWORDBREAK        = (NativeMethods.WM_USER + 76);
        internal const int EM_SETOPTIONS           = (NativeMethods.WM_USER + 77);
        internal const int EM_GETOPTIONS           = (NativeMethods.WM_USER + 78);
        internal const int EM_FINDTEXTEX           = (NativeMethods.WM_USER + 79);
        internal const int EM_GETWORDBREAKPROCEX   = (NativeMethods.WM_USER + 80);
        internal const int EM_SETWORDBREAKPROCEX   = (NativeMethods.WM_USER + 81);

        // Richedit v2.0 messages
        internal const int EM_SETUNDOLIMIT         = (NativeMethods.WM_USER + 82);
        internal const int EM_REDO                 = (NativeMethods.WM_USER + 84);
        internal const int EM_CANREDO              = (NativeMethods.WM_USER + 85);
        internal const int EM_GETUNDONAME          = (NativeMethods.WM_USER + 86);
        internal const int EM_GETREDONAME          = (NativeMethods.WM_USER + 87);
        internal const int EM_STOPGROUPTYPING      = (NativeMethods.WM_USER + 88);

        internal const int EM_SETTEXTMODE          = (NativeMethods.WM_USER + 89);
        internal const int EM_GETTEXTMODE          = (NativeMethods.WM_USER + 90);

        internal const int EM_AUTOURLDETECT        = (NativeMethods.WM_USER + 91);
        internal const int EM_GETAUTOURLDETECT     = (NativeMethods.WM_USER + 92);
        internal const int EM_SETPALETTE           = (NativeMethods.WM_USER + 93);
        internal const int EM_GETTEXTEX            = (NativeMethods.WM_USER + 94);
        internal const int EM_GETTEXTLENGTHEX      = (NativeMethods.WM_USER + 95);

        // Asia specific messages
        internal const int EM_SETPUNCTUATION       = (NativeMethods.WM_USER + 100);
        internal const int EM_GETPUNCTUATION       = (NativeMethods.WM_USER + 101);
        internal const int EM_SETWORDWRAPMODE      = (NativeMethods.WM_USER + 102);
        internal const int EM_GETWORDWRAPMODE      = (NativeMethods.WM_USER + 103);
        internal const int EM_SETIMECOLOR          = (NativeMethods.WM_USER + 104);
        internal const int EM_GETIMECOLOR          = (NativeMethods.WM_USER + 105);
        internal const int EM_SETIMEOPTIONS        = (NativeMethods.WM_USER + 106);
        internal const int EM_GETIMEOPTIONS        = (NativeMethods.WM_USER + 107);
        internal const int EM_CONVPOSITION         = (NativeMethods.WM_USER + 108);

        internal const int EM_SETLANGOPTIONS       = (NativeMethods.WM_USER + 120);
        internal const int EM_GETLANGOPTIONS       = (NativeMethods.WM_USER + 121);
        internal const int EM_GETIMECOMPMODE       = (NativeMethods.WM_USER + 122);

        internal const int EM_FINDTEXTW            = (NativeMethods.WM_USER + 123);
        internal const int EM_FINDTEXTEXW          = (NativeMethods.WM_USER + 124);

        //Rich TextBox 3.0 Asia msgs
        internal const int EM_RECONVERSION         = (NativeMethods.WM_USER + 125);
        internal const int EM_SETIMEMODEBIAS       = (NativeMethods.WM_USER + 126);
        internal const int EM_GETIMEMODEBIAS       = (NativeMethods.WM_USER + 127);

        // BiDi Specific messages
        internal const int EM_SETBIDIOPTIONS       = (NativeMethods.WM_USER + 200);
        internal const int EM_GETBIDIOPTIONS       = (NativeMethods.WM_USER + 201);

        internal const int EM_SETTYPOGRAPHYOPTIONS = (NativeMethods.WM_USER + 202);
        internal const int EM_GETTYPOGRAPHYOPTIONS = (NativeMethods.WM_USER + 203);

        // Extended TextBox style specific messages
        internal const int EM_SETEDITSTYLE         = (NativeMethods.WM_USER + 204);
        internal const int EM_GETEDITSTYLE         = (NativeMethods.WM_USER + 205);

        // Ole Objects Disabling message
        internal const int EM_SETQUERYRTFOBJ       = (NativeMethods.WM_USER + 270);

        // Extended edit style masks
        internal const int SES_EMULATESYSEDIT      = 1;
        internal const int SES_BEEPONMAXTEXT       = 2;
        internal const int SES_EXTENDBACKCOLOR     = 4;
        internal const int SES_MAPCPS              = 8;
        internal const int SES_EMULATE10           = 16;
        internal const int SES_USECRLF             = 32;
        internal const int SES_USEAIMM             = 64;
        internal const int SES_NOIME               = 128;
        internal const int SES_ALLOWBEEPS          = 256;
        internal const int SES_UPPERCASE           = 512;
        internal const int SES_LOWERCASE           = 1024;
        internal const int SES_NOINPUTSEQUENCECHK  = 2048;
        internal const int SES_BIDI                = 4096;
        internal const int SES_SCROLLONKILLFOCUS   = 8192;
        internal const int SES_XLTCRCRLFTOCR       = 16384;

        // Options for EM_SETLANGOPTIONS and EM_GETLANGOPTIONS
        internal const int IMF_AUTOKEYBOARD        = 0x0001;
        internal const int IMF_AUTOFONT            = 0x0002;
        internal const int IMF_IMECANCELCOMPLETE   = 0x0004;   // high completes the comp string when aborting, low cancels.
        internal const int IMF_IMEALWAYSSENDNOTIFY = 0x0008;
        internal const int IMF_AUTOFONTSIZEADJUST  = 0x0010;
        internal const int IMF_UIFONTS             = 0x0020;
        internal const int IMF_DUALFONT            = 0x0080;

        // Values for EM_GETIMECOMPMODE
        internal const int ICM_NOTOPEN             = 0x0000;
        internal const int ICM_LEVEL3              = 0x0001;
        internal const int ICM_LEVEL2              = 0x0002;
        internal const int ICM_LEVEL2_5            = 0x0003;
        internal const int ICM_LEVEL2_SUI          = 0x0004;

        // Pegasus outline mode messages (RE 3.0)

        // Outline mode message
        internal const int EM_OUTLINE              = NativeMethods.WM_USER + 220;

        // Message for getting and restoring scroll pos
        internal const int EM_GETSCROLLPOS         = NativeMethods.WM_USER + 221;
        internal const int EM_SETSCROLLPOS         = NativeMethods.WM_USER + 222;

        // Change fontsize in current selection by wparam
        internal const int EM_SETFONTSIZE          = NativeMethods.WM_USER + 223;
        internal const int EM_GETZOOM              = NativeMethods.WM_USER + 224;
        internal const int EM_SETZOOM              = NativeMethods.WM_USER + 225;

        // Outline mode wparam values
        internal const int EMO_EXIT    = 0; // enter normal mode,  lparam ignored
        internal const int EMO_ENTER   = 1; // enter outline mode, lparam ignored
        internal const int EMO_PROMOTE = 2; // LOWORD(lparam) == 0 ==>
                                            // promote  to body-text
                                            // LOWORD(lparam) != 0 ==>
                                            // promote/demote current selection
                                            // by indicated number of levels
        internal const int EMO_EXPAND  = 3; // HIWORD(lparam) = EMO_EXPANDSELECTION
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
        internal const int EMO_GETVIEWMODE   = 5; // Returns VM_NORMAL or VM_OUTLINE

        // EMO_EXPAND options

        internal const int EMO_EXPANDSELECTION    = 0;
        internal const int EMO_EXPANDDOCUMENT     = 1;
        internal const int VM_NORMAL              = 4; // Agrees with RTF \viewkindN
        internal const int VM_OUTLINE             = 2;

        // New notifications
        internal const int EN_MSGFILTER            = 0x0700;
        internal const int EN_REQUESTRESIZE        = 0x0701;
        internal const int EN_SELCHANGE            = 0x0702;
        internal const int EN_DROPFILES            = 0x0703;
        internal const int EN_PROTECTED            = 0x0704;
        internal const int EN_CORRECTTEXT          = 0x0705;       /* PenWin specific */
        internal const int EN_STOPNOUNDO           = 0x0706;
        internal const int EN_IMECHANGE            = 0x0707;       /* Asia specific */
        internal const int EN_SAVECLIPBOARD        = 0x0708;
        internal const int EN_OLEOPFAILED          = 0x0709;
        internal const int EN_OBJECTPOSITIONS      = 0x070a;
        internal const int EN_LINK                 = 0x070b;
        internal const int EN_DRAGDROPDONE         = 0x070c;
        internal const int EN_PARAGRAPHEXPANDED    = 0x070d;

        // BiDi specific notifications
        internal const int EN_ALIGNLTR             = 0x0710;
        internal const int EN_ALIGNRTL             = 0x0711;

        // Event notification masks */
        internal const int ENM_NONE                = 0x00000000;
        internal const int ENM_CHANGE              = 0x00000001;
        internal const int ENM_UPDATE              = 0x00000002;
        internal const int ENM_SCROLL              = 0x00000004;
        internal const int ENM_KEYEVENTS           = 0x00010000;
        internal const int ENM_MOUSEEVENTS         = 0x00020000;
        internal const int ENM_REQUESTRESIZE       = 0x00040000;
        internal const int ENM_SELCHANGE           = 0x00080000;
        internal const int ENM_DROPFILES           = 0x00100000;
        internal const int ENM_PROTECTED           = 0x00200000;
        internal const int ENM_CORRECTTEXT         = 0x00400000;   /* PenWin specific */
        internal const int ENM_SCROLLEVENTS        = 0x00000008;
        internal const int ENM_DRAGDROPDONE        = 0x00000010;
        internal const int ENM_PARAGRAPHEXPANDED   = 0x00000020;

        /* Asia specific notification mask */
        internal const int ENM_IMECHANGE           = 0x00800000;   /* unused by RE2.0 */
        internal const int ENM_LANGCHANGE          = 0x01000000;
        internal const int ENM_OBJECTPOSITIONS     = 0x02000000;
        internal const int ENM_LINK                = 0x04000000;

        /* New edit control styles */
        internal const int ES_SAVESEL              = 0x00008000;
        internal const int ES_SUNKEN               = 0x00004000;
        internal const int ES_DISABLENOSCROLL      = 0x00002000;
        /* same as WS_MAXIMIZE, but that doesn't make sense so we re-use the value */
        internal const int ES_SELECTIONBAR         = 0x01000000;
        /* same as ES_UPPERCASE, but re-used to completely disable OLE drag'n'drop */
        internal const int ES_NOOLEDRAGDROP        = 0x00000008;

        /* New edit control extended style */
        internal const int ES_EX_NOCALLOLEINIT     = 0x01000000;

        /* These flags are used in FE Windows */
        internal const int ES_VERTICAL             = 0x00400000; // NOT IN RE3.0/2.0
        internal const int ES_NOIME                = 0x00080000;
        internal const int ES_SELFIME              = 0x00040000;

        /* TextBox control options */
        internal const int ECO_AUTOWORDSELECTION   = 0x00000001;
        internal const int ECO_AUTOVSCROLL         = 0x00000040;
        internal const int ECO_AUTOHSCROLL         = 0x00000080;
        internal const int ECO_NOHIDESEL           = 0x00000100;
        internal const int ECO_READONLY            = 0x00000800;
        internal const int ECO_WANTRETURN          = 0x00001000;
        internal const int ECO_SAVESEL             = 0x00008000;
        internal const int ECO_SELECTIONBAR        = 0x01000000; // guessing this is selection margin
        internal const int ECO_VERTICAL            = 0x00400000;   /* FE specific */


        /* ECO operations */
        internal const int ECOOP_SET               = 0x0001;
        internal const int ECOOP_OR                = 0x0002;
        internal const int ECOOP_AND               = 0x0003;
        internal const int ECOOP_XOR               = 0x0004;

        /* new word break function actions */
        internal const int WB_CLASSIFY             = 3;
        internal const int WB_MOVEWORDLEFT         = 4;
        internal const int WB_MOVEWORDRIGHT        = 5;
        internal const int WB_LEFTBREAK            = 6;
        internal const int WB_RIGHTBREAK           = 7;

        /* Asia specific flags */
        internal const int WB_MOVEWORDPREV         = 4;
        internal const int WB_MOVEWORDNEXT         = 5;
        internal const int WB_PREVBREAK            = 6;
        internal const int WB_NEXTBREAK            = 7;

        internal const int PC_FOLLOWING            = 1;
        internal const int PC_LEADING              = 2;
        internal const int PC_OVERFLOW             = 3;
        internal const int PC_DELIMITER            = 4;

        internal const int WBF_WORDWRAP            = 0x010;
        internal const int WBF_WORDBREAK           = 0x020;
        internal const int WBF_OVERFLOW            = 0x040;
        internal const int WBF_LEVEL1              = 0x080;
        internal const int WBF_LEVEL2              = 0x100;
        internal const int WBF_CUSTOM              = 0x200;

        /* for use with EM_GET/SETTEXTMODE */
        internal const int TM_PLAINTEXT                        = 1;
        internal const int TM_RICHTEXT                         = 2;    /* default behavior */
        internal const int TM_SINGLELEVELUNDO          = 4;
        internal const int TM_MULTILEVELUNDO           = 8;    /* default behavior */
        internal const int TM_SINGLECODEPAGE           = 16;
        internal const int TM_MULTICODEPAGE            = 32;   /* default behavior */

        /* Asia specific flags */
        internal const int IMF_FORCENONE           = 0x0001;
        internal const int IMF_FORCEENABLE         = 0x0002;
        internal const int IMF_FORCEDISABLE        = 0x0004;
        internal const int IMF_CLOSESTATUSWINDOW   = 0x0008;
        internal const int IMF_VERTICAL            = 0x0020;
        internal const int IMF_FORCEACTIVE         = 0x0040;
        internal const int IMF_FORCEINACTIVE       = 0x0080;
        internal const int IMF_FORCEREMEMBER       = 0x0100;
        internal const int IMF_MULTIPLEEDIT        = 0x0400;

        /* Word break flags (used with WB_CLASSIFY) */
        internal const int WBF_CLASS               = 0x0F;
        internal const int WBF_ISWHITE             = 0x10;
        internal const int WBF_BREAKLINE           = 0x20;
        internal const int WBF_BREAKAFTER          = 0x40;

        internal const int cchTextLimitDefault     = 32767;

        /* CHARFORMAT masks */
        internal const int CFM_BOLD                = 0x00000001;
        internal const int CFM_ITALIC              = 0x00000002;
        internal const int CFM_UNDERLINE           = 0x00000004;
        internal const int CFM_STRIKEOUT           = 0x00000008;
        internal const int CFM_PROTECTED           = 0x00000010;
        internal const int CFM_LINK                = 0x00000020;   /* Exchange hyperlink extension */
        internal const int CFM_SIZE                = unchecked((int)0x80000000);
        internal const int CFM_COLOR               = 0x40000000;
        internal const int CFM_FACE                = 0x20000000;
        internal const int CFM_OFFSET              = 0x10000000;
        internal const int CFM_CHARSET             = 0x08000000;

        /* CHARFORMAT effects */
        internal const int CFE_BOLD                = 0x0001;
        internal const int CFE_ITALIC              = 0x0002;
        internal const int CFE_UNDERLINE           = 0x0004;
        internal const int CFE_STRIKEOUT           = 0x0008;
        internal const int CFE_PROTECTED           = 0x0010;
        internal const int CFE_LINK                = 0x0020;
        internal const int CFE_AUTOCOLOR           = 0x40000000;   /* NOTE: this corresponds to */
                                                                 /* CFM_COLOR, which controls it */
        internal const int yHeightCharPtsMost      = 1638;

        /* EM_SETCHARFORMAT wparam masks */
        internal const int SCF_SELECTION           = 0x0001;
        internal const int SCF_WORD                = 0x0002;
        internal const int SCF_DEFAULT             = 0x0000;   // set the default charformat or paraformat
        internal const int SCF_ALL                 = 0x0004;   // not valid with SCF_SELECTION or SCF_WORD
        internal const int SCF_USEUIRULES          = 0x0008;   // modifier for SCF_SELECTION; says that
                                                                    // the format came from a toolbar, etc. and
                                                                    // therefore UI formatting rules should be
                                                                    // used instead of strictly formatting the
                                                                    // selection.

        /* stream formats */
        internal const int SF_TEXT = 0x0001;
        internal const int SF_RTF = 0x0002;
        internal const int SF_RTFNOOBJS = 0x0003; /* outbound only */
        internal const int SF_TEXTIZED = 0x0004;  /* outbound only */
        internal const int SF_UNICODE = 0x0010;   /* Unicode file of some kind */

        /* Flag telling stream operations to operate on the selection only */
        /* EM_STREAMIN will replace the current selection */
        /* EM_STREAMOUT will stream out the current selection */
        internal const int SFF_SELECTION           = 0x8000;

        /* Flag telling stream operations to operate on the common RTF keyword only */
        /* EM_STREAMIN will accept the only common RTF keyword */
        /* EM_STREAMOUT will stream out the only common RTF keyword */
        internal const int SFF_PLAINRTF            = 0x4000;

        /* all paragraph measurements are in twips */

        internal const int MAX_TAB_STOPS           = 32;
        internal const int lDefaultTab             = 720;

        /* PARAFORMAT mask values */
        internal const int PFM_STARTINDENT         = 0x00000001;
        internal const int PFM_RIGHTINDENT         = 0x00000002;
        internal const int PFM_OFFSET              = 0x00000004;
        internal const int PFM_ALIGNMENT           = 0x00000008;
        internal const int PFM_TABSTOPS            = 0x00000010;
        internal const int PFM_NUMBERING           = 0x00000020;
        internal const int PFM_OFFSETINDENT        = unchecked((int)0x80000000);

        /* PARAFORMAT numbering options */
        internal const int PFN_BULLET              = 0x0001;

        /* PARAFORMAT alignment options */
        internal const int PFA_LEFT                = 0x0001;
        internal const int PFA_RIGHT               = 0x0002;
        internal const int PFA_CENTER              = 0x0003;

        /* CHARFORMAT and PARAFORMAT "ALL" masks
           CFM_COLOR mirrors CFE_AUTOCOLOR, a little
           code to easily deal with autocolor */
        internal const int CFM_EFFECTS             = (CFM_BOLD | CFM_ITALIC |
                                                           CFM_UNDERLINE | CFM_COLOR |
                                                           CFM_STRIKEOUT | CFE_PROTECTED |
                                                           CFM_LINK);
        internal const int CFM_ALL                 = (CFM_EFFECTS | CFM_SIZE |
                                                           CFM_FACE | CFM_OFFSET | CFM_CHARSET);
        internal const int PFM_ALL                 = (PFM_STARTINDENT | PFM_RIGHTINDENT |
                                                           PFM_OFFSET | PFM_ALIGNMENT |
                                                           PFM_TABSTOPS | PFM_NUMBERING |
                                                           PFM_OFFSETINDENT);

        /* New masks and effects -- a parenthesized asterisk indicates that
           the data is stored by RichEdit2.0, but not displayed */

        internal const int CFM_SMALLCAPS           = 0x0040;                   /* (*)  */
        internal const int CFM_ALLCAPS             = 0x0080;                   /* (*)  */
        internal const int CFM_HIDDEN              = 0x0100;                   /* (*)  */
        internal const int CFM_OUTLINE             = 0x0200;                   /* (*)  */
        internal const int CFM_SHADOW              = 0x0400;                   /* (*)  */
        internal const int CFM_EMBOSS              = 0x0800;                   /* (*)  */
        internal const int CFM_IMPRINT             = 0x1000;                   /* (*)  */
        internal const int CFM_DISABLED            = 0x2000;
        internal const int CFM_REVISED             = 0x4000;

        internal const int CFM_BACKCOLOR           = 0x04000000;
        internal const int CFM_LCID                = 0x02000000;
        internal const int CFM_UNDERLINETYPE       = 0x00800000;               /* (*)  */
        internal const int CFM_WEIGHT              = 0x00400000;
        internal const int CFM_SPACING             = 0x00200000;               /* (*)  */
        internal const int CFM_KERNING             = 0x00100000;               /* (*)  */
        internal const int CFM_STYLE               = 0x00080000;               /* (*)  */
        internal const int CFM_ANIMATION           = 0x00040000;               /* (*)  */
        internal const int CFM_REVAUTHOR           = 0x00008000;

        internal const int CFE_SUBSCRIPT           = 0x00010000;               /* Superscript and subscript are */
        internal const int CFE_SUPERSCRIPT         = 0x00020000;               /*  mutually exclusive                   */

        internal const int CFM_SUBSCRIPT           = (CFE_SUBSCRIPT | CFE_SUPERSCRIPT);
        internal const int CFM_SUPERSCRIPT         = CFM_SUBSCRIPT;

        internal const int CFM_EFFECTS2            = (CFM_EFFECTS | CFM_DISABLED |
                                                           CFM_SMALLCAPS | CFM_ALLCAPS |
                                                           CFM_HIDDEN | CFM_OUTLINE |
                                                           CFM_SHADOW | CFM_EMBOSS |
                                                           CFM_IMPRINT | CFM_DISABLED |
                                                           CFM_REVISED | CFM_SUBSCRIPT |
                                                           CFM_SUPERSCRIPT | CFM_BACKCOLOR);

        internal const int CFM_ALL2                = (CFM_ALL | CFM_EFFECTS2 |
                                                           CFM_BACKCOLOR | CFM_LCID |
                                                           CFM_UNDERLINETYPE | CFM_WEIGHT |
                                                           CFM_REVAUTHOR | CFM_SPACING |
                                                           CFM_KERNING | CFM_STYLE |
                                                           CFM_ANIMATION);

        internal const int CFE_SMALLCAPS           = CFM_SMALLCAPS;
        internal const int CFE_ALLCAPS             = CFM_ALLCAPS;
        internal const int CFE_HIDDEN              = CFM_HIDDEN;
        internal const int CFE_OUTLINE             = CFM_OUTLINE;
        internal const int CFE_SHADOW              = CFM_SHADOW;
        internal const int CFE_EMBOSS              = CFM_EMBOSS;
        internal const int CFE_IMPRINT             = CFM_IMPRINT;
        internal const int CFE_DISABLED            = CFM_DISABLED;
        internal const int CFE_REVISED             = CFM_REVISED;

        /* NOTE: CFE_AUTOCOLOR and CFE_AUTOBACKCOLOR correspond to CFM_COLOR and
           CFM_BACKCOLOR, respectively, which control them */
        internal const int CFE_AUTOBACKCOLOR       = CFM_BACKCOLOR;

        /* Underline types */
        internal const int CFU_CF1UNDERLINE        = 0xFF; /* map charformat's bit underline to CF2.*/
        internal const int CFU_INVERT              = 0xFE; /* For IME composition fake a selection.*/
        internal const int CFU_UNDERLINEDOTTED     = 0x4;  /* (*) displayed as ordinary underline      */
        internal const int CFU_UNDERLINEDOUBLE     = 0x3;  /* (*) displayed as ordinary underline      */
        internal const int CFU_UNDERLINEWORD       = 0x2;  /* (*) displayed as ordinary underline      */
        internal const int CFU_UNDERLINE           = 0x1;
        internal const int CFU_UNDERLINENONE       = 0;

        /* PARAFORMAT 2.0 masks and effects */

        internal const int PFM_SPACEBEFORE         = 0x00000040;
        internal const int PFM_SPACEAFTER          = 0x00000080;
        internal const int PFM_LINESPACING         = 0x00000100;
        internal const int PFM_STYLE               = 0x00000400;
        internal const int PFM_BORDER              = 0x00000800;       /* (*)  */
        internal const int PFM_SHADING             = 0x00001000;       /* (*)  */
        internal const int PFM_NUMBERINGSTYLE      = 0x00002000;       /* (*)  */
        internal const int PFM_NUMBERINGTAB        = 0x00004000;       /* (*)  */
        internal const int PFM_NUMBERINGSTART      = 0x00008000;       /* (*)  */

        internal const int PFM_RTLPARA             = 0x00010000;
        internal const int PFM_KEEP                = 0x00020000;       /* (*)  */
        internal const int PFM_KEEPNEXT            = 0x00040000;       /* (*)  */
        internal const int PFM_PAGEBREAKBEFORE     = 0x00080000;       /* (*)  */
        internal const int PFM_NOLINENUMBER        = 0x00100000;       /* (*)  */
        internal const int PFM_NOWIDOWCONTROL      = 0x00200000;       /* (*)  */
        internal const int PFM_DONOTHYPHEN         = 0x00400000;       /* (*)  */
        internal const int PFM_SIDEBYSIDE          = 0x00800000;       /* (*)  */

        internal const int PFM_TABLE               = unchecked((int)0xc0000000);       /* (*)  */

        /* Note: PARAFORMAT has no effects */
        internal const int PFM_EFFECTS             = (PFM_RTLPARA | PFM_KEEP |
                                                           PFM_KEEPNEXT | PFM_TABLE |
                                                           PFM_PAGEBREAKBEFORE | PFM_NOLINENUMBER |
                                                           PFM_NOWIDOWCONTROL | PFM_DONOTHYPHEN |
                                                           PFM_SIDEBYSIDE | PFM_TABLE);

        internal const int PFM_ALL2                = (PFM_ALL | PFM_EFFECTS |
                                                           PFM_SPACEBEFORE | PFM_SPACEAFTER |
                                                           PFM_LINESPACING | PFM_STYLE |
                                                           PFM_SHADING | PFM_BORDER |
                                                           PFM_NUMBERINGTAB | PFM_NUMBERINGSTART |
                                                           PFM_NUMBERINGSTYLE);

        internal const int PFE_RTLPARA             = (PFM_RTLPARA               >> 16);
        internal const int PFE_KEEP                = (PFM_KEEP                  >> 16);        /* (*)  */
        internal const int PFE_KEEPNEXT            = (PFM_KEEPNEXT              >> 16);        /* (*)  */
        internal const int PFE_PAGEBREAKBEFORE     = (PFM_PAGEBREAKBEFORE >> 16);      /* (*)  */
        internal const int PFE_NOLINENUMBER        = (PFM_NOLINENUMBER  >> 16);        /* (*)  */
        internal const int PFE_NOWIDOWCONTROL      = (PFM_NOWIDOWCONTROL >> 16);       /* (*)  */
        internal const int PFE_DONOTHYPHEN         = (PFM_DONOTHYPHEN   >> 16);        /* (*)  */
        internal const int PFE_SIDEBYSIDE          = (PFM_SIDEBYSIDE    >> 16);        /* (*)  */

        internal const int PFE_TABLEROW            = 0xc000;           /* These 3 options are mutually */
        internal const int PFE_TABLECELLEND        = 0x8000;           /*  exclusive and each imply    */
        internal const int PFE_TABLECELL           = 0x4000;           /*  that para is part of a table*/

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
        internal const int PFA_JUSTIFY             = 4;        /* New paragraph-alignment option 2.0 (*) */

        internal const int SEL_EMPTY               = 0x0000;
        internal const int SEL_TEXT                = 0x0001;
        internal const int SEL_OBJECT              = 0x0002;
        internal const int SEL_MULTICHAR           = 0x0004;
        internal const int SEL_MULTIOBJECT         = 0x0008;

        internal const int  tomTrue = -1,
                            tomFalse = 0,
                            tomNone = 0,
                            tomUndefined = -9999999,
                            tomAutoColor = -9999997;

        /* used with IRichEditOleCallback::GetContextMenu, this flag will be
           passed as a "selection type".  It indicates that a context menu for
           a right-mouse drag drop should be generated.  The IOleObject parameter
           will really be the IDataObject for the drop
         */
        internal const int GCM_RIGHTMOUSEDROP      = 0x8000;

        internal const int OLEOP_DOVERB            = 1;

        /* clipboard formats - use as parameter to RegisterClipboardFormat() */
        internal const string CF_RTF               = "Rich Text Format";
        internal const string CF_RTFNOOBJS         = "Rich Text Format Without Objects";
        internal const string CF_RETEXTOBJ         = "RichEdit Text and Objects";

        /*  UndoName info */
        internal const int UID_UNKNOWN             = 0;
        internal const int UID_TYPING                  = 1;
        internal const int UID_DELETE                  = 2;
        internal const int UID_DRAGDROP                = 3;
        internal const int UID_CUT                             = 4;
        internal const int UID_PASTE                   = 5;

        /* flags for the GETEXTEX data structure */
        internal const int GT_DEFAULT              = 0;
        internal const int GT_USECRLF              = 1;

        /* flags for the GETTEXTLENGTHEX data structure */
        internal const int GTL_DEFAULT             = 0;        /* do the default (return # of chars)           */
        internal const int GTL_USECRLF             = 1;        /* compute answer using CRLFs for paragraphs*/
        internal const int GTL_PRECISE             = 2;        /* compute a precise answer                                     */
        internal const int GTL_CLOSE               = 4;        /* fast computation of a "close" answer         */
        internal const int GTL_NUMCHARS            = 8;        /* return the number of characters                      */
        internal const int GTL_NUMBYTES            = 16;       /* return the number of _bytes_                         */

        /* UNICODE embedding character */
// disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
        internal static readonly char WCH_EMBEDDING          = (char)0xFFFC;
#pragma warning restore 0414

        /* flags for the find text options */
        internal const int FR_DOWN                 = 0x00000001;
        internal const int FR_WHOLEWORD            = 0x00000002;
        internal const int FR_MATCHCASE            = 0x00000004;
    }
}
