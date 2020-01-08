﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using static Interop.User32;

internal static partial class Interop
{
    internal static partial class User32
    {
        public const uint WM_KEYFIRST       = 0x0100;
        public const uint WM_KEYLAST        = 0x0109;
        public const uint WM_IME_KEYLAST    = 0x010F;
        public const WindowMessage WM_MOUSEFIRST = (WindowMessage)0x0200;
        public const WindowMessage WM_MOUSELAST = (WindowMessage)0x020E;
        public const uint WM_USER           = 0x0400;
        public const uint WM_APP            = 0x8000;

        public const uint DTM_FIRST         = 0x1000;      // DateTimePicker messages
        public const uint TV_FIRST          = 0x1100;      // TreeView messages
        public const uint PGM_FIRST         = 0x1400;      // Pager control messages
        public const uint ECM_FIRST         = 0x1500;      // Edit control messages
        public const uint BCM_FIRST         = 0x1600;      // Button control messages
        public const uint CBM_FIRST         = 0x1700;      // Combobox control messages

        // https://docs.microsoft.com/en-us/cpp/mfc/tn062-message-reflection-for-windows-controls?view=vs-2019
        public const uint WM_REFLECT = WM_USER + 0x1C00;
        public const uint OCM__BASE  = WM_USER + 0x1C00;

        public static class RegisteredMessage
        {
            private static uint s_wmUnSubclass = uint.MaxValue;
            public static WindowMessage WM_UIUNSUBCLASS
            {
                get
                {
                    if (s_wmUnSubclass == uint.MaxValue)
                    {
                        s_wmUnSubclass = (uint)RegisterWindowMessageW("WinFormsUnSubclass");
                    }

                    return (WindowMessage)s_wmUnSubclass;
                }
            }
        }

        public enum WindowMessage : uint
        {
            WM_NULL                             = 0x0000,
            WM_CREATE                           = 0x0001,
            WM_DESTROY                          = 0x0002,
            WM_MOVE                             = 0x0003,
            WM_SIZE                             = 0x0005,
            WM_ACTIVATE                         = 0x0006,
            WM_SETFOCUS                         = 0x0007,
            WM_KILLFOCUS                        = 0x0008,
            WM_ENABLE                           = 0x000A,
            WM_SETREDRAW                        = 0x000B,
            WM_SETTEXT                          = 0x000C,
            WM_GETTEXT                          = 0x000D,
            WM_GETTEXTLENGTH                    = 0x000E,
            WM_PAINT                            = 0x000F,
            WM_CLOSE                            = 0x0010,
            WM_QUERYENDSESSION                  = 0x0011,
            WM_QUERYOPEN                        = 0x0013,
            WM_ENDSESSION                       = 0x0016,
            WM_QUIT                             = 0x0012,
            WM_ERASEBKGND                       = 0x0014,
            WM_SYSCOLORCHANGE                   = 0x0015,
            WM_SHOWWINDOW                       = 0x0018,
            WM_CTLCOLOR                         = 0x0019,
            WM_SETTINGCHANGE                    = 0x001A,
            WM_DEVMODECHANGE                    = 0x001B,
            WM_ACTIVATEAPP                      = 0x001C,
            WM_FONTCHANGE                       = 0x001D,
            WM_TIMECHANGE                       = 0x001E,
            WM_CANCELMODE                       = 0x001F,
            WM_SETCURSOR                        = 0x0020,
            WM_MOUSEACTIVATE                    = 0x0021,
            WM_CHILDACTIVATE                    = 0x0022,
            WM_QUEUESYNC                        = 0x0023,
            WM_GETMINMAXINFO                    = 0x0024,
            WM_PAINTICON                        = 0x0026,
            WM_ICONERASEBKGND                   = 0x0027,
            WM_NEXTDLGCTL                       = 0x0028,
            WM_SPOOLERSTATUS                    = 0x002A,
            WM_DRAWITEM                         = 0x002B,
            WM_MEASUREITEM                      = 0x002C,
            WM_DELETEITEM                       = 0x002D,
            WM_VKEYTOITEM                       = 0x002E,
            WM_CHARTOITEM                       = 0x002F,
            WM_SETFONT                          = 0x0030,
            WM_GETFONT                          = 0x0031,
            WM_SETHOTKEY                        = 0x0032,
            WM_GETHOTKEY                        = 0x0033,
            WM_QUERYDRAGICON                    = 0x0037,
            WM_COMPAREITEM                      = 0x0039,
            WM_GETOBJECT                        = 0x003D,
            WM_COMPACTING                       = 0x0041,
            WM_COMMNOTIFY                       = 0x0044,
            WM_WINDOWPOSCHANGING                = 0x0046,
            WM_WINDOWPOSCHANGED                 = 0x0047,
            WM_POWER                            = 0x0048,
            WM_COPYDATA                         = 0x004A,
            WM_CANCELJOURNAL                    = 0x004B,
            WM_NOTIFY                           = 0x004E,
            WM_INPUTLANGCHANGEREQUEST           = 0x0050,
            WM_INPUTLANGCHANGE                  = 0x0051,
            WM_TCARD                            = 0x0052,
            WM_HELP                             = 0x0053,
            WM_USERCHANGED                      = 0x0054,
            WM_NOTIFYFORMAT                     = 0x0055,
            WM_CONTEXTMENU                      = 0x007B,
            WM_STYLECHANGING                    = 0x007C,
            WM_STYLECHANGED                     = 0x007D,
            WM_DISPLAYCHANGE                    = 0x007E,
            WM_GETICON                          = 0x007F,
            WM_SETICON                          = 0x0080,
            WM_NCCREATE                         = 0x0081,
            WM_NCDESTROY                        = 0x0082,
            WM_NCCALCSIZE                       = 0x0083,
            WM_NCHITTEST                        = 0x0084,
            WM_NCPAINT                          = 0x0085,
            WM_NCACTIVATE                       = 0x0086,
            WM_GETDLGCODE                       = 0x0087,
            WM_SYNCPAINT                        = 0x0088,
            WM_NCMOUSEMOVE                      = 0x00A0,
            WM_NCLBUTTONDOWN                    = 0x00A1,
            WM_NCLBUTTONUP                      = 0x00A2,
            WM_NCLBUTTONDBLCLK                  = 0x00A3,
            WM_NCRBUTTONDOWN                    = 0x00A4,
            WM_NCRBUTTONUP                      = 0x00A5,
            WM_NCRBUTTONDBLCLK                  = 0x00A6,
            WM_NCMBUTTONDOWN                    = 0x00A7,
            WM_NCMBUTTONUP                      = 0x00A8,
            WM_NCMBUTTONDBLCLK                  = 0x00A9,
            WM_NCXBUTTONDOWN                    = 0x00AB,
            WM_NCXBUTTONUP                      = 0x00AC,
            WM_NCXBUTTONDBLCLK                  = 0x00AD,
            EM_GETSEL                           = 0x00B0,
            EM_SETSEL                           = 0x00B1,
            EM_GETRECT                          = 0x00B2,
            EM_SETRECT                          = 0x00B3,
            EM_SETRECTNP                        = 0x00B4,
            EM_SCROLL                           = 0x00B5,
            EM_LINESCROLL                       = 0x00B6,
            EM_SCROLLCARET                      = 0x00B7,
            EM_GETMODIFY                        = 0x00B8,
            EM_SETMODIFY                        = 0x00B9,
            EM_GETLINECOUNT                     = 0x00BA,
            EM_LINEINDEX                        = 0x00BB,
            EM_SETHANDLE                        = 0x00BC,
            EM_GETHANDLE                        = 0x00BD,
            EM_GETTHUMB                         = 0x00BE,
            EM_LINELENGTH                       = 0x00C1,
            EM_REPLACESEL                       = 0x00C2,
            EM_GETLINE                          = 0x00C4,
            EM_SETLIMITTEXT                     = 0x00C5,
            EM_CANUNDO                          = 0x00C6,
            EM_UNDO                             = 0x00C7,
            EM_FMTLINES                         = 0x00C8,
            EM_LINEFROMCHAR                     = 0x00C9,
            EM_SETTABSTOPS                      = 0x00CB,
            EM_SETPASSWORDCHAR                  = 0x00CC,
            EM_EMPTYUNDOBUFFER                  = 0x00CD,
            EM_GETFIRSTVISIBLELINE              = 0x00CE,
            EM_SETREADONLY                      = 0x00CF,
            EM_SETWORDBREAKPROC                 = 0x00D0,
            EM_GETWORDBREAKPROC                 = 0x00D1,
            EM_GETPASSWORDCHAR                  = 0x00D2,
            EM_SETMARGINS                       = 0x00D3,
            EM_GETMARGINS                       = 0x00D4,
            EM_GETLIMITTEXT                     = 0x00D5,
            EM_POSFROMCHAR                      = 0x00D6,
            EM_CHARFROMPOS                      = 0x00D7,
            WM_INPUT_DEVICE_CHANGE              = 0x00FE,
            WM_INPUT                            = 0x00FF,
            WM_KEYDOWN                          = 0x0100,
            WM_KEYUP                            = 0x0101,
            WM_CHAR                             = 0x0102,
            WM_DEADCHAR                         = 0x0103,
            WM_SYSKEYDOWN                       = 0x0104,
            WM_SYSKEYUP                         = 0x0105,
            WM_SYSCHAR                          = 0x0106,
            WM_SYSDEADCHAR                      = 0x0107,
            WM_UNICHAR                          = 0x0109,
            WM_IME_STARTCOMPOSITION             = 0x010D,
            WM_IME_ENDCOMPOSITION               = 0x010E,
            WM_IME_COMPOSITION                  = 0x010F,
            WM_INITDIALOG                       = 0x0110,
            WM_COMMAND                          = 0x0111,
            WM_SYSCOMMAND                       = 0x0112,
            WM_TIMER                            = 0x0113,
            WM_HSCROLL                          = 0x0114,
            WM_VSCROLL                          = 0x0115,
            WM_INITMENU                         = 0x0116,
            WM_INITMENUPOPUP                    = 0x0117,
            WM_GESTURE                          = 0x0119,
            WM_GESTURENOTIFY                    = 0x011A,
            WM_MENUSELECT                       = 0x011F,
            WM_MENUCHAR                         = 0x0120,
            WM_ENTERIDLE                        = 0x0121,
            WM_MENURBUTTONUP                    = 0x0122,
            WM_MENUDRAG                         = 0x0123,
            WM_MENUGETOBJECT                    = 0x0124,
            WM_UNINITMENUPOPUP                  = 0x0125,
            WM_MENUCOMMAND                      = 0x0126,
            WM_CHANGEUISTATE                    = 0x0127,
            WM_UPDATEUISTATE                    = 0x0128,
            WM_QUERYUISTATE                     = 0x0129,
            WM_CTLCOLORMSGBOX                   = 0x0132,
            WM_CTLCOLOREDIT                     = 0x0133,
            WM_CTLCOLORLISTBOX                  = 0x0134,
            WM_CTLCOLORBTN                      = 0x0135,
            WM_CTLCOLORDLG                      = 0x0136,
            WM_CTLCOLORSCROLLBAR                = 0x0137,
            WM_CTLCOLORSTATIC                   = 0x0138,
            CB_GETEDITSEL                       = 0x0140,
            CB_LIMITTEXT                        = 0x0141,
            CB_SETEDITSEL                       = 0x0142,
            CB_ADDSTRING                        = 0x0143,
            CB_DELETESTRING                     = 0x0144,
            CB_DIR                              = 0x0145,
            CB_GETCOUNT                         = 0x0146,
            CB_GETCURSEL                        = 0x0147,
            CB_GETLBTEXT                        = 0x0148,
            CB_GETLBTEXTLEN                     = 0x0149,
            CB_INSERTSTRING                     = 0x014A,
            CB_RESETCONTENT                     = 0x014B,
            CB_FINDSTRING                       = 0x014C,
            CB_SELECTSTRING                     = 0x014D,
            CB_SETCURSEL                        = 0x014E,
            CB_SHOWDROPDOWN                     = 0x014F,
            CB_GETITEMDATA                      = 0x0150,
            CB_SETITEMDATA                      = 0x0151,
            CB_GETDROPPEDCONTROLRECT            = 0x0152,
            CB_SETITEMHEIGHT                    = 0x0153,
            CB_GETITEMHEIGHT                    = 0x0154,
            CB_SETEXTENDEDUI                    = 0x0155,
            CB_GETEXTENDEDUI                    = 0x0156,
            CB_GETDROPPEDSTATE                  = 0x0157,
            CB_FINDSTRINGEXACT                  = 0x0158,
            CB_SETLOCALE                        = 0x0159,
            CB_GETLOCALE                        = 0x015A,
            CB_GETTOPINDEX                      = 0x015b,
            CB_SETTOPINDEX                      = 0x015c,
            CB_GETHORIZONTALEXTENT              = 0x015d,
            CB_SETHORIZONTALEXTENT              = 0x015e,
            CB_GETDROPPEDWIDTH                  = 0x015f,
            CB_SETDROPPEDWIDTH                  = 0x0160,
            CB_INITSTORAGE                      = 0x0161,
            MN_GETHMENU                         = 0x01E1,
            WM_MOUSEMOVE                        = 0x0200,
            WM_LBUTTONDOWN                      = 0x0201,
            WM_LBUTTONUP                        = 0x0202,
            WM_LBUTTONDBLCLK                    = 0x0203,
            WM_RBUTTONDOWN                      = 0x0204,
            WM_RBUTTONUP                        = 0x0205,
            WM_RBUTTONDBLCLK                    = 0x0206,
            WM_MBUTTONDOWN                      = 0x0207,
            WM_MBUTTONUP                        = 0x0208,
            WM_MBUTTONDBLCLK                    = 0x0209,
            WM_MOUSEWHEEL                       = 0x020A,
            WM_XBUTTONDOWN                      = 0x020B,
            WM_XBUTTONUP                        = 0x020C,
            WM_XBUTTONDBLCLK                    = 0x020D,
            WM_MOUSEHWHEEL                      = 0x020E,
            WM_PARENTNOTIFY                     = 0x0210,
            WM_ENTERMENULOOP                    = 0x0211,
            WM_EXITMENULOOP                     = 0x0212,
            WM_NEXTMENU                         = 0x0213,
            WM_SIZING                           = 0x0214,
            WM_CAPTURECHANGED                   = 0x0215,
            WM_MOVING                           = 0x0216,
            WM_POWERBROADCAST                   = 0x0218,
            WM_DEVICECHANGE                     = 0x0219,
            WM_MDICREATE                        = 0x0220,
            WM_MDIDESTROY                       = 0x0221,
            WM_MDIACTIVATE                      = 0x0222,
            WM_MDIRESTORE                       = 0x0223,
            WM_MDINEXT                          = 0x0224,
            WM_MDIMAXIMIZE                      = 0x0225,
            WM_MDITILE                          = 0x0226,
            WM_MDICASCADE                       = 0x0227,
            WM_MDIICONARRANGE                   = 0x0228,
            WM_MDIGETACTIVE                     = 0x0229,
            WM_MDISETMENU                       = 0x0230,
            WM_ENTERSIZEMOVE                    = 0x0231,
            WM_EXITSIZEMOVE                     = 0x0232,
            WM_DROPFILES                        = 0x0233,
            WM_MDIREFRESHMENU                   = 0x0234,
            WM_POINTERDEVICECHANGE              = 0x0238,
            WM_POINTERDEVICEINRANGE             = 0x0239,
            WM_POINTERDEVICEOUTOFRANGE          = 0x023A,
            WM_TOUCH                            = 0x0240,
            WM_NCPOINTERUPDATE                  = 0x0241,
            WM_NCPOINTERDOWN                    = 0x0242,
            WM_NCPOINTERUP                      = 0x0243,
            WM_POINTERUPDATE                    = 0x0245,
            WM_POINTERDOWN                      = 0x0246,
            WM_POINTERUP                        = 0x0247,
            WM_POINTERENTER                     = 0x0249,
            WM_POINTERLEAVE                     = 0x024A,
            WM_POINTERACTIVATE                  = 0x024B,
            WM_POINTERCAPTURECHANGED            = 0x024C,
            WM_TOUCHHITTESTING                  = 0x024D,
            WM_POINTERWHEEL                     = 0x024E,
            WM_POINTERHWHEEL                    = 0x024F,
            DM_POINTERHITTEST                   = 0x0250,
            WM_POINTERROUTEDTO                  = 0x0251,
            WM_POINTERROUTEDAWAY                = 0x0252,
            WM_POINTERROUTEDRELEASED            = 0x0253,
            WM_IME_SETCONTEXT                   = 0x0281,
            WM_IME_NOTIFY                       = 0x0282,
            WM_IME_CONTROL                      = 0x0283,
            WM_IME_COMPOSITIONFULL              = 0x0284,
            WM_IME_SELECT                       = 0x0285,
            WM_IME_CHAR                         = 0x0286,
            WM_IME_REQUEST                      = 0x0288,
            WM_IME_KEYDOWN                      = 0x0290,
            WM_IME_KEYUP                        = 0x0291,
            WM_MOUSEHOVER                       = 0x02A1,
            WM_MOUSELEAVE                       = 0x02A3,
            WM_NCMOUSEHOVER                     = 0x02A0,
            WM_NCMOUSELEAVE                     = 0x02A2,
            WM_WTSSESSION_CHANGE                = 0x02B1,
            WM_DPICHANGED                       = 0x02E0,
            WM_DPICHANGED_BEFOREPARENT          = 0x02E2,
            WM_DPICHANGED_AFTERPARENT           = 0x02E3,
            WM_GETDPISCALEDSIZE                 = 0x02E4,
            WM_CUT                              = 0x0300,
            WM_COPY                             = 0x0301,
            WM_PASTE                            = 0x0302,
            WM_CLEAR                            = 0x0303,
            WM_UNDO                             = 0x0304,
            WM_RENDERFORMAT                     = 0x0305,
            WM_RENDERALLFORMATS                 = 0x0306,
            WM_DESTROYCLIPBOARD                 = 0x0307,
            WM_DRAWCLIPBOARD                    = 0x0308,
            WM_PAINTCLIPBOARD                   = 0x0309,
            WM_VSCROLLCLIPBOARD                 = 0x030A,
            WM_SIZECLIPBOARD                    = 0x030B,
            WM_ASKCBFORMATNAME                  = 0x030C,
            WM_CHANGECBCHAIN                    = 0x030D,
            WM_HSCROLLCLIPBOARD                 = 0x030E,
            WM_QUERYNEWPALETTE                  = 0x030F,
            WM_PALETTEISCHANGING                = 0x0310,
            WM_PALETTECHANGED                   = 0x0311,
            WM_HOTKEY                           = 0x0312,
            WM_PRINT                            = 0x0317,
            WM_PRINTCLIENT                      = 0x0318,
            WM_APPCOMMAND                       = 0x0319,
            WM_THEMECHANGED                     = 0x031A,
            WM_CLIPBOARDUPDATE                  = 0x031D,
            WM_DWMCOMPOSITIONCHANGED            = 0x031E,
            WM_DWMNCRENDERINGCHANGED            = 0x031F,
            WM_DWMCOLORIZATIONCOLORCHANGED      = 0x0320,
            WM_DWMWINDOWMAXIMIZEDCHANGE         = 0x0321,
            WM_DWMSENDICONICTHUMBNAIL           = 0x0323,
            WM_DWMSENDICONICLIVEPREVIEWBITMAP   = 0x0326,
            WM_GETTITLEBARINFOEX                = 0x033F,
            WM_CHOOSEFONT_GETLOGFONT            = WM_USER + 1,
            CBEM_INSERTITEMA                    = WM_USER + 1,
            CBEM_SETIMAGELIST                   = WM_USER + 2,
            CBEM_GETIMAGELIST                   = WM_USER + 3,
            CBEM_GETITEMA                       = WM_USER + 4,
            CBEM_SETITEMA                       = WM_USER + 5,
            CBEM_GETCOMBOCONTROL                = WM_USER + 6,
            CBEM_GETEDITCONTROL                 = WM_USER + 7,
            CBEM_SETEXSTYLE                     = WM_USER + 8,
            CBEM_GETEXSTYLE                     = WM_USER + 9,
            CBEM_GETEXTENDEDSTYLE               = WM_USER + 9,
            CBEM_HASEDITCHANGED                 = WM_USER + 10,
            CBEM_INSERTITEMW                    = WM_USER + 11,
            CBEM_SETITEMW                       = WM_USER + 12,
            CBEM_GETITEMW                       = WM_USER + 13,
            CBEM_SETEXTENDEDSTYLE               = WM_USER + 14,
            TTM_ACTIVATE                        = WM_USER + 1,
            TTM_SETDELAYTIME                    = WM_USER + 3,
            TTM_RELAYEVENT                      = WM_USER + 7,
            TTM_WINDOWFROMPOINT                 = WM_USER + 16,
            TTM_TRACKACTIVATE                   = WM_USER + 17,
            TTM_TRACKPOSITION                   = WM_USER + 18,
            TTM_SETTIPBKCOLOR                   = WM_USER + 19,
            TTM_SETTIPTEXTCOLOR                 = WM_USER + 20,
            TTM_GETDELAYTIME                    = WM_USER + 21,
            TTM_GETTIPBKCOLOR                   = WM_USER + 22,
            TTM_GETTIPTEXTCOLOR                 = WM_USER + 23,
            TTM_SETMAXTIPWIDTH                  = WM_USER + 24,
            TTM_POP                             = WM_USER + 28,
            TTM_UPDATE                          = WM_USER + 29,
            TTM_GETBUBBLESIZE                   = WM_USER + 30,
            TTM_ADJUSTRECT                      = WM_USER + 31,
            TTM_SETTITLEW                       = WM_USER + 33,
            TTM_ADDTOOLW                        = WM_USER + 50,
            TTM_DELTOOLW                        = WM_USER + 51,
            TTM_NEWTOOLRECTW                    = WM_USER + 52,
            TTM_GETTOOLINFOW                    = WM_USER + 53,
            TTM_SETTOOLINFOW                    = WM_USER + 54,
            TTM_HITTESTW                        = WM_USER + 55,
            TTM_GETTEXTW                        = WM_USER + 56,
            TTM_UPDATETIPTEXTW                  = WM_USER + 57,
            TTM_ENUMTOOLSW                      = WM_USER + 58,
            TTM_GETCURRENTTOOLW                 = WM_USER + 59,
            DTM_GETSYSTEMTIME                   = DTM_FIRST + 1,
            DTM_SETSYSTEMTIME                   = DTM_FIRST + 2,
            DTM_SETRANGE                        = DTM_FIRST + 4,
            DTM_SETMCCOLOR                      = DTM_FIRST + 6,
            DTM_GETMONTHCAL                     = DTM_FIRST + 8,
            DTM_SETMCFONT                       = DTM_FIRST + 9,
            DTM_SETFORMATW                      = DTM_FIRST + 50,
            TVM_DELETEITEM                      = TV_FIRST + 1,
            TVM_EXPAND                          = TV_FIRST + 2,
            TVM_GETITEMRECT                     = TV_FIRST + 4,
            TVM_GETCOUNT                        = TV_FIRST + 5,
            TVM_GETINDENT                       = TV_FIRST + 6,
            TVM_SETINDENT                       = TV_FIRST + 7,
            TVM_GETIMAGELIST                    = TV_FIRST + 8,
            TVM_SETIMAGELIST                    = TV_FIRST + 9,
            TVM_GETNEXTITEM                     = TV_FIRST + 10,
            TVM_SELECTITEM                      = TV_FIRST + 11,
            TVM_GETEDITCONTROL                  = TV_FIRST + 15,
            TVM_GETVISIBLECOUNT                 = TV_FIRST + 16,
            TVM_HITTEST                         = TV_FIRST + 17,
            TVM_CREATEDRAGIMAGE                 = TV_FIRST + 18,
            TVM_SORTCHILDREN                    = TV_FIRST + 19,
            TVM_ENSUREVISIBLE                   = TV_FIRST + 20,
            TVM_SORTCHILDRENCB                  = TV_FIRST + 21,
            TVM_ENDEDITLABELNOW                 = TV_FIRST + 22,
            TVM_SETTOOLTIPS                     = TV_FIRST + 24,
            TVM_GETTOOLTIPS                     = TV_FIRST + 25,
            TVM_SETINSERTMARK                   = TV_FIRST + 26,
            TVM_SETITEMHEIGHT                   = TV_FIRST + 27,
            TVM_GETITEMHEIGHT                   = TV_FIRST + 28,
            TVM_SETBKCOLOR                      = TV_FIRST + 29,
            TVM_SETTEXTCOLOR                    = TV_FIRST + 30,
            TVM_GETBKCOLOR                      = TV_FIRST + 31,
            TVM_GETTEXTCOLOR                    = TV_FIRST + 32,
            TVM_SETSCROLLTIME                   = TV_FIRST + 33,
            TVM_GETSCROLLTIME                   = TV_FIRST + 34,
            TVM_SETBORDER                       = TV_FIRST + 35,
            TVM_SETINSERTMARKCOLOR              = TV_FIRST + 37,
            TVM_GETINSERTMARKCOLOR              = TV_FIRST + 38,
            TVM_GETITEMSTATE                    = TV_FIRST + 39,
            TVM_SETLINECOLOR                    = TV_FIRST + 40,
            TVM_GETLINECOLOR                    = TV_FIRST + 41,
            TVM_MAPACCIDTOHTREEITEM             = TV_FIRST + 42,
            TVM_MAPHTREEITEMTOACCID             = TV_FIRST + 43,
            TVM_SETEXTENDEDSTYLE                = TV_FIRST + 44,
            TVM_GETEXTENDEDSTYLE                = TV_FIRST + 45,
            TVM_INSERTITEMW                     = TV_FIRST + 50,
            TVM_SETHOT                          = TV_FIRST + 58,
            TVM_SETAUTOSCROLLINFO               = TV_FIRST + 59,
            TVM_GETITEMW                        = TV_FIRST + 62,
            TVM_SETITEMW                        = TV_FIRST + 63,
            TVM_GETISEARCHSTRINGW               = TV_FIRST + 64,
            TVM_EDITLABELW                      = TV_FIRST + 65,
            TVM_GETSELECTEDCOUNT                = TV_FIRST + 70,
            TVM_SHOWINFOTIP                     = TV_FIRST + 71,
            TVM_GETITEMPARTRECT                 = TV_FIRST + 72,
            WM_REFLECT_NOTIFY                   = WM_REFLECT + WM_NOTIFY,
            OCM_COMMAND                         = OCM__BASE + WM_COMMAND,
            OCM_CTLCOLORBTN                     = OCM__BASE + WM_CTLCOLORBTN,
            OCM_CTLCOLOREDIT                    = OCM__BASE + WM_CTLCOLOREDIT,
            OCM_CTLCOLORDLG                     = OCM__BASE + WM_CTLCOLORDLG,
            OCM_CTLCOLORLISTBOX                 = OCM__BASE + WM_CTLCOLORLISTBOX,
            OCM_CTLCOLORMSGBOX                  = OCM__BASE + WM_CTLCOLORMSGBOX,
            OCM_CTLCOLORSCROLLBAR               = OCM__BASE + WM_CTLCOLORSCROLLBAR,
            OCM_CTLCOLORSTATIC                  = OCM__BASE + WM_CTLCOLORSTATIC,
            OCM_DRAWITEM                        = OCM__BASE + WM_DRAWITEM,
            OCM_MEASUREITEM                     = OCM__BASE + WM_MEASUREITEM,
            OCM_DELETEITEM                      = OCM__BASE + WM_DELETEITEM,
            OCM_VKEYTOITEM                      = OCM__BASE + WM_VKEYTOITEM,
            OCM_CHARTOITEM                      = OCM__BASE + WM_CHARTOITEM,
            OCM_COMPAREITEM                     = OCM__BASE + WM_COMPAREITEM,
            OCM_HSCROLL                         = OCM__BASE + WM_HSCROLL,
            OCM_VSCROLL                         = OCM__BASE + WM_VSCROLL,
            OCM_PARENTNOTIFY                    = OCM__BASE + WM_PARENTNOTIFY
            // OCM_NOTIFY            (OCM__BASE + WM_NOTIFY)
        }
    }
}

internal static class WindowMessageExtensions
{
    public static WindowMessage WindowMessage(this ref Message message)
        => (WindowMessage)message.Msg;

    public static bool Is(this ref Message message, WindowMessage windowMessage)
        => message.Msg == (int)windowMessage;

    public static bool IsMouseMessage(this ref Message message)
        => message.IsBetween(WM_MOUSEFIRST, WM_MOUSELAST);

    public static bool IsMouseMessage(this ref MSG message)
        => message.IsBetween(WM_MOUSEFIRST, WM_MOUSELAST);

    public static bool IsKeyMessage(this ref Message message)
        => message.IsBetween((WindowMessage)WM_KEYFIRST, (WindowMessage)WM_KEYLAST);

    public static bool IsKeyMessage(this ref MSG message)
        => message.IsBetween((WindowMessage)WM_KEYFIRST, (WindowMessage)WM_KEYLAST);

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref Message message,
        WindowMessage firstMessage,
        WindowMessage secondMessage)
        => message.Msg >= (int)firstMessage && message.Msg <= (int)secondMessage;

    /// <summary>
    /// Returns true if the message is between <paramref name="firstMessage"/> and
    /// <paramref name="secondMessage"/>, inclusive.
    /// </summary>
    public static bool IsBetween(
        this ref MSG message,
        WindowMessage firstMessage,
        WindowMessage secondMessage)
        => (uint)message.message >= (uint)firstMessage && (uint)message.message <= (uint)secondMessage;
}

