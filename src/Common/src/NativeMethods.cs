// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    internal static class NativeMethods
    {
        public static IntPtr InvalidIntPtr = (IntPtr)(-1);
        public static IntPtr LPSTR_TEXTCALLBACK = (IntPtr)(-1);
        public static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        public const int BITMAPINFO_MAX_COLORSIZE = 256;

        public const int STATUS_PENDING = 0x103; //259 = STILL_ALIVE

        public const int
            DESKTOP_SWITCHDESKTOP = 0x0100,
            ERROR_ACCESS_DENIED = 0x0005;

        public const int BS_SOLID = 0;

        public const int SHGFI_ICON = 0x000000100,   // get icon
        SHGFI_DISPLAYNAME = 0x000000200,     // get display name
        SHGFI_TYPENAME = 0x000000400,     // get type name
        SHGFI_ATTRIBUTES = 0x000000800,     // get attributes
        SHGFI_ICONLOCATION = 0x000001000,     // get icon location
        SHGFI_EXETYPE = 0x000002000,     // return exe type
        SHGFI_SYSICONINDEX = 0x000004000,     // get system icon index
        SHGFI_LINKOVERLAY = 0x000008000,     // put a link overlay on icon
        SHGFI_SELECTED = 0x000010000,     // show icon in selected state
        SHGFI_ATTR_SPECIFIED = 0x000020000,     // get only specified attributes
        SHGFI_LARGEICON = 0x000000000,     // get large icon
        SHGFI_SMALLICON = 0x000000001,     // get small icon
        SHGFI_OPENICON = 0x000000002,     // get open icon
        SHGFI_SHELLICONSIZE = 0x000000004,     // get shell size icon
        SHGFI_PIDL = 0x000000008,     // pszPath is a pidl
        SHGFI_USEFILEATTRIBUTES = 0x000000010,     // use passed dwFileAttribute
        SHGFI_ADDOVERLAYS = 0x000000020,     // apply the appropriate overlays
        SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay

        public const int DM_DISPLAYORIENTATION = 0x00000080;

        public const int AUTOSUGGEST = 0x10000000,
        AUTOSUGGEST_OFF = 0x20000000,
        AUTOAPPEND = 0x40000000,
        AUTOAPPEND_OFF = (unchecked((int)0x80000000));

        public const int ARW_BOTTOMLEFT = 0x0000,
        ARW_BOTTOMRIGHT = 0x0001,
        ARW_TOPLEFT = 0x0002,
        ARW_TOPRIGHT = 0x0003,
        ARW_LEFT = 0x0000,
        ARW_RIGHT = 0x0000,
        ARW_UP = 0x0004,
        ARW_DOWN = 0x0004,
        ARW_HIDE = 0x0008,
        ACM_OPENA = (0x0400 + 100),
        ACM_OPENW = (0x0400 + 103),
        ADVF_NODATA = 1,
        ADVF_ONLYONCE = 4,
        ADVF_PRIMEFIRST = 2;
        // Note: ADVF_ONLYONCE and ADVF_PRIMEFIRST values now conform with objidl.dll but are backwards from
        // Platform SDK documentation as of 07/21/2003.
        // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/com/htm/oen_a2z_8jxi.asp.

        public const int BCM_GETIDEALSIZE = 0x1601,
        BI_RGB = 0,
        BS_PATTERN = 3,
        BDR_RAISEDOUTER = 0x0001,
        BDR_SUNKENOUTER = 0x0002,
        BDR_RAISEDINNER = 0x0004,
        BDR_SUNKENINNER = 0x0008,
        BDR_RAISED = 0x0005,
        BDR_SUNKEN = 0x000a,
        BF_LEFT = 0x0001,
        BF_TOP = 0x0002,
        BF_RIGHT = 0x0004,
        BF_BOTTOM = 0x0008,
        BF_ADJUST = 0x2000,
        BF_FLAT = 0x4000,
        BF_MIDDLE = 0x0800,
        BFFM_INITIALIZED = 1,
        BFFM_SELCHANGED = 2,
        BFFM_SETSELECTION = 0x400 + 103,
        BFFM_ENABLEOK = 0x400 + 101,
        BS_PUSHBUTTON = 0x00000000,
        BS_DEFPUSHBUTTON = 0x00000001,
        BS_MULTILINE = 0x00002000,
        BS_PUSHLIKE = 0x00001000,
        BS_OWNERDRAW = 0x0000000B,
        BS_RADIOBUTTON = 0x00000004,
        BS_3STATE = 0x00000005,
        BS_GROUPBOX = 0x00000007,
        BS_LEFT = 0x00000100,
        BS_RIGHT = 0x00000200,
        BS_CENTER = 0x00000300,
        BS_TOP = 0x00000400,
        BS_BOTTOM = 0x00000800,
        BS_VCENTER = 0x00000C00,
        BS_RIGHTBUTTON = 0x00000020,
        BN_CLICKED = 0,
        BM_SETCHECK = 0x00F1,
        BM_SETSTATE = 0x00F3,
        BM_CLICK = 0x00F5;

        public const int CDERR_DIALOGFAILURE = 0xFFFF,
        CDERR_STRUCTSIZE = 0x0001,
        CDERR_INITIALIZATION = 0x0002,
        CDERR_NOTEMPLATE = 0x0003,
        CDERR_NOHINSTANCE = 0x0004,
        CDERR_LOADSTRFAILURE = 0x0005,
        CDERR_FINDRESFAILURE = 0x0006,
        CDERR_LOADRESFAILURE = 0x0007,
        CDERR_LOCKRESFAILURE = 0x0008,
        CDERR_MEMALLOCFAILURE = 0x0009,
        CDERR_MEMLOCKFAILURE = 0x000A,
        CDERR_NOHOOK = 0x000B,
        CDERR_REGISTERMSGFAIL = 0x000C,
        CFERR_NOFONTS = 0x2001,
        CFERR_MAXLESSTHANMIN = 0x2002,
        CC_RGBINIT = 0x00000001,
        CC_FULLOPEN = 0x00000002,
        CC_PREVENTFULLOPEN = 0x00000004,
        CC_SHOWHELP = 0x00000008,
        CC_ENABLEHOOK = 0x00000010,
        CC_SOLIDCOLOR = 0x00000080,
        CC_ANYCOLOR = 0x00000100,
        CF_SCREENFONTS = 0x00000001,
        CF_SHOWHELP = 0x00000004,
        CF_ENABLEHOOK = 0x00000008,
        CF_INITTOLOGFONTSTRUCT = 0x00000040,
        CF_EFFECTS = 0x00000100,
        CF_APPLY = 0x00000200,
        CF_SCRIPTSONLY = 0x00000400,
        CF_NOVECTORFONTS = 0x00000800,
        CF_NOSIMULATIONS = 0x00001000,
        CF_LIMITSIZE = 0x00002000,
        CF_FIXEDPITCHONLY = 0x00004000,
        CF_FORCEFONTEXIST = 0x00010000,
        CF_TTONLY = 0x00040000,
        CF_SELECTSCRIPT = 0x00400000,
        CF_NOVERTFONTS = 0x01000000,
        CP_WINANSI = 1004;

        public const int cmb4 = 0x0473;

        public enum ClassStyle : uint
        {
            CS_DBLCLKS = 0x0008,
            CS_DROPSHADOW = 0x00020000,
            CS_SAVEBITS = 0x0800
        }

        public const int CF_TEXT = 1,
        CF_BITMAP = 2,
        CF_METAFILEPICT = 3,
        CF_SYLK = 4,
        CF_DIF = 5,
        CF_TIFF = 6,
        CF_OEMTEXT = 7,
        CF_DIB = 8,
        CF_PALETTE = 9,
        CF_PENDATA = 10,
        CF_RIFF = 11,
        CF_WAVE = 12,
        CF_UNICODETEXT = 13,
        CF_ENHMETAFILE = 14,
        CF_HDROP = 15,
        CF_LOCALE = 16,
        CLSCTX_INPROC_SERVER = 0x1,
        CLSCTX_LOCAL_SERVER = 0x4,
        CW_USEDEFAULT = (unchecked((int)0x80000000)),
        CWP_SKIPINVISIBLE = 0x0001,
        COLOR_WINDOW = 5,
        CB_ERR = (-1),
        CBN_SELCHANGE = 1,
        CBN_DBLCLK = 2,
        CBN_EDITCHANGE = 5,
        CBN_EDITUPDATE = 6,
        CBN_DROPDOWN = 7,
        CBN_CLOSEUP = 8,
        CBN_SELENDOK = 9,
        CBS_SIMPLE = 0x0001,
        CBS_DROPDOWN = 0x0002,
        CBS_DROPDOWNLIST = 0x0003,
        CBS_OWNERDRAWFIXED = 0x0010,
        CBS_OWNERDRAWVARIABLE = 0x0020,
        CBS_AUTOHSCROLL = 0x0040,
        CBS_HASSTRINGS = 0x0200,
        CBS_NOINTEGRALHEIGHT = 0x0400,
        CB_GETEDITSEL = 0x0140,
        CB_LIMITTEXT = 0x0141,
        CB_SETEDITSEL = 0x0142,
        CB_ADDSTRING = 0x0143,
        CB_DELETESTRING = 0x0144,
        CB_GETCURSEL = 0x0147,
        CB_GETLBTEXT = 0x0148,
        CB_GETLBTEXTLEN = 0x0149,
        CB_INSERTSTRING = 0x014A,
        CB_RESETCONTENT = 0x014B,
        CB_FINDSTRING = 0x014C,
        CB_SETCURSEL = 0x014E,
        CB_SHOWDROPDOWN = 0x014F,
        CB_GETITEMDATA = 0x0150,
        CB_SETITEMHEIGHT = 0x0153,
        CB_GETITEMHEIGHT = 0x0154,
        CB_GETDROPPEDSTATE = 0x0157,
        CB_FINDSTRINGEXACT = 0x0158,
        CB_GETDROPPEDWIDTH = 0x015F,
        CB_SETDROPPEDWIDTH = 0x0160,
        CDRF_DODEFAULT = 0x00000000,
        CDRF_NEWFONT = 0x00000002,
        CDRF_SKIPDEFAULT = 0x00000004,
        CDRF_NOTIFYPOSTPAINT = 0x00000010,
        CDRF_NOTIFYITEMDRAW = 0x00000020,
        CDRF_NOTIFYSUBITEMDRAW = CDRF_NOTIFYITEMDRAW,
        CDDS_PREPAINT = 0x00000001,
        CDDS_POSTPAINT = 0x00000002,
        CDDS_ITEM = 0x00010000,
        CDDS_SUBITEM = 0x00020000,
        CDDS_ITEMPREPAINT = (0x00010000 | 0x00000001),
        CDDS_ITEMPOSTPAINT = (0x00010000 | 0x00000002),
        CDIS_SELECTED = 0x0001,
        CDIS_GRAYED = 0x0002,
        CDIS_DISABLED = 0x0004,
        CDIS_CHECKED = 0x0008,
        CDIS_FOCUS = 0x0010,
        CDIS_DEFAULT = 0x0020,
        CDIS_HOT = 0x0040,
        CDIS_MARKED = 0x0080,
        CDIS_INDETERMINATE = 0x0100,
        CDIS_SHOWKEYBOARDCUES = 0x0200,
        CLR_NONE = unchecked((int)0xFFFFFFFF),
        CLR_DEFAULT = unchecked((int)0xFF000000),
        CCM_SETVERSION = (0x2000 + 0x7),
        CCM_GETVERSION = (0x2000 + 0x8),
        CCS_NORESIZE = 0x00000004,
        CCS_NOPARENTALIGN = 0x00000008,
        CCS_NODIVIDER = 0x00000040,
        CBEM_INSERTITEM = (0x0400 + 11),
        CBEM_SETITEM = (0x0400 + 12),
        CBEM_GETITEM = (0x0400 + 13),
        CBEN_ENDEDIT = ((0 - 800) - 6),
        CONNECT_E_NOCONNECTION = unchecked((int)0x80040200),
        CONNECT_E_CANNOTCONNECT = unchecked((int)0x80040202),
        CTRLINFO_EATS_RETURN = 1,
        CTRLINFO_EATS_ESCAPE = 2;

        public const int DUPLICATE = 0x06,
        DISPID_UNKNOWN = (-1),
        DISPID_PROPERTYPUT = (-3),
        DISPATCH_METHOD = 0x1,
        DISPATCH_PROPERTYGET = 0x2,
        DISPATCH_PROPERTYPUT = 0x4,
        DV_E_DVASPECT = unchecked((int)0x8004006B),
        DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003),
        DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004),
        DISP_E_EXCEPTION = unchecked((int)0x80020009),
        DIB_RGB_COLORS = 0,
        DUPLICATE_SAME_ACCESS = 0x00000002,
        DFC_CAPTION = 1,
        DFC_MENU = 2,
        DFC_SCROLL = 3,
        DFC_BUTTON = 4,
        DFCS_CAPTIONCLOSE = 0x0000,
        DFCS_CAPTIONMIN = 0x0001,
        DFCS_CAPTIONMAX = 0x0002,
        DFCS_CAPTIONRESTORE = 0x0003,
        DFCS_CAPTIONHELP = 0x0004,
        DFCS_MENUARROW = 0x0000,
        DFCS_MENUCHECK = 0x0001,
        DFCS_MENUBULLET = 0x0002,
        DFCS_SCROLLUP = 0x0000,
        DFCS_SCROLLDOWN = 0x0001,
        DFCS_SCROLLLEFT = 0x0002,
        DFCS_SCROLLRIGHT = 0x0003,
        DFCS_SCROLLCOMBOBOX = 0x0005,
        DFCS_BUTTONCHECK = 0x0000,
        DFCS_BUTTONRADIO = 0x0004,
        DFCS_BUTTON3STATE = 0x0008,
        DFCS_BUTTONPUSH = 0x0010,
        DFCS_INACTIVE = 0x0100,
        DFCS_PUSHED = 0x0200,
        DFCS_CHECKED = 0x0400,
        DFCS_FLAT = 0x4000,
        DCX_WINDOW = 0x00000001,
        DCX_CACHE = 0x00000002,
        DCX_LOCKWINDOWUPDATE = 0x00000400,
        DCX_INTERSECTRGN = 0x00000080,
        DI_NORMAL = 0x0003,
        DLGC_WANTARROWS = 0x0001,
        DLGC_WANTTAB = 0x0002,
        DLGC_WANTALLKEYS = 0x0004,
        DLGC_WANTCHARS = 0x0080,
        DLGC_WANTMESSAGE = 0x0004,      /* Pass message to control          */
        DLGC_HASSETSEL = 0x0008,      /* Understands EM_SETSEL message    */
        DTM_GETSYSTEMTIME = (0x1000 + 1),
        DTM_SETSYSTEMTIME = (0x1000 + 2),
        DTM_SETRANGE = (0x1000 + 4),
        DTM_SETFORMAT = (0x1000 + 50),
        DTM_SETMCCOLOR = (0x1000 + 6),
        DTM_GETMONTHCAL = (0x1000 + 8),
        DTM_SETMCFONT = (0x1000 + 9),
        DTS_UPDOWN = 0x0001,
        DTS_SHOWNONE = 0x0002,
        DTS_LONGDATEFORMAT = 0x0004,
        DTS_TIMEFORMAT = 0x0009,
        DTS_RIGHTALIGN = 0x0020,
        DTN_DATETIMECHANGE = ((0 - 760) + 1),
        DTN_USERSTRING = ((0 - 760) + 15),
        DTN_WMKEYDOWN = ((0 - 760) + 16),
        DTN_FORMAT = ((0 - 760) + 17),
        DTN_FORMATQUERY = ((0 - 760) + 18),
        DTN_DROPDOWN = ((0 - 760) + 6),
        DTN_CLOSEUP = ((0 - 760) + 7),
        DVASPECT_CONTENT = 1,
        DVASPECT_TRANSPARENT = 32,
        DVASPECT_OPAQUE = 16;

        public const int E_NOTIMPL = unchecked((int)0x80004001),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
        E_INVALIDARG = unchecked((int)0x80070057),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_FAIL = unchecked((int)0x80004005),
        E_ABORT = unchecked((int)0x80004004),
        E_UNEXPECTED = unchecked((int)0x8000FFFF),
        INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011),
        ETO_OPAQUE = 0x0002,
        ETO_CLIPPED = 0x0004,
        EMR_POLYTEXTOUT = 97,
        EDGE_RAISED = (0x0001 | 0x0004),
        EDGE_SUNKEN = (0x0002 | 0x0008),
        EDGE_ETCHED = (0x0002 | 0x0004),
        EDGE_BUMP = (0x0001 | 0x0008),
        ES_LEFT = 0x0000,
        ES_CENTER = 0x0001,
        ES_RIGHT = 0x0002,
        ES_MULTILINE = 0x0004,
        ES_UPPERCASE = 0x0008,
        ES_LOWERCASE = 0x0010,
        ES_AUTOVSCROLL = 0x0040,
        ES_AUTOHSCROLL = 0x0080,
        ES_NOHIDESEL = 0x0100,
        ES_READONLY = 0x0800,
        ES_PASSWORD = 0x0020,
        EN_CHANGE = 0x0300,
        EN_UPDATE = 0x0400,
        EN_HSCROLL = 0x0601,
        EN_VSCROLL = 0x0602,
        EN_ALIGN_LTR_EC = 0x0700,
        EN_ALIGN_RTL_EC = 0x0701,
        EC_LEFTMARGIN = 0x0001,
        EC_RIGHTMARGIN = 0x0002;

        public const int ERROR_INVALID_HANDLE = 6;
        public const int ERROR_CLASS_ALREADY_EXISTS = 1410;

        public const int FNERR_SUBCLASSFAILURE = 0x3001,
        FNERR_INVALIDFILENAME = 0x3002,
        FNERR_BUFFERTOOSMALL = 0x3003,
        FRERR_BUFFERLENGTHZERO = 0x4001,
        FADF_BSTR = (0x100),
        FADF_UNKNOWN = (0x200),
        FADF_DISPATCH = (0x400),
        FADF_VARIANT = (unchecked((int)0x800)),
        FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
        FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
        FVIRTKEY = 0x01,
        FSHIFT = 0x04,
        FALT = 0x10;

        public const int GMEM_FIXED = 0x0000,
        GMEM_MOVEABLE = 0x0002,
        GMEM_NOCOMPACT = 0x0010,
        GMEM_NODISCARD = 0x0020,
        GMEM_ZEROINIT = 0x0040,
        GMEM_MODIFY = 0x0080,
        GMEM_DISCARDABLE = 0x0100,
        GMEM_NOT_BANKED = 0x1000,
        GMEM_SHARE = 0x2000,
        GMEM_DDESHARE = 0x2000,
        GMEM_NOTIFY = 0x4000,
        GMEM_LOWER = GMEM_NOT_BANKED,
        GMEM_VALID_FLAGS = 0x7F72,
        GMEM_INVALID_HANDLE = 0x8000,
        GHND = (GMEM_MOVEABLE | GMEM_ZEROINIT),
        GPTR = (GMEM_FIXED | GMEM_ZEROINIT),
        GCL_WNDPROC = (-24),
        GWL_WNDPROC = (-4),
        GWL_HWNDPARENT = (-8),
        GWL_STYLE = (-16),
        GWL_EXSTYLE = (-20),
        GWL_ID = (-12),
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_CHILD = 5,
        GMR_VISIBLE = 0,
        GMR_DAYSTATE = 1,
        GDI_ERROR = (unchecked((int)0xFFFFFFFF)),
        GDTR_MIN = 0x0001,
        GDTR_MAX = 0x0002,
        GDT_VALID = 0,
        GDT_NONE = 1,
        GA_PARENT = 1,
        GA_ROOT = 2;

        // ImmGetCompostionString index.
        public const int
        GCS_COMPSTR = 0x0008,
        GCS_COMPATTR = 0x0010,
        GCS_RESULTSTR = 0x0800,

        // attribute for COMPOSITIONSTRING Structure
        ATTR_INPUT = 0x00,
        ATTR_TARGET_CONVERTED = 0x01,
        ATTR_CONVERTED = 0x02,
        ATTR_TARGET_NOTCONVERTED = 0x03,
        ATTR_INPUT_ERROR = 0x04,
        ATTR_FIXEDCONVERTED = 0x05,

        // dwAction for ImmNotifyIME
        NI_COMPOSITIONSTR = 0x0015,

        // dwIndex for ImmNotifyIME/NI_COMPOSITIONSTR
        CPS_COMPLETE = 0x01,
        CPS_CANCEL = 0x04;

        public const int
        HC_ACTION = 0,
        HC_GETNEXT = 1,
        HC_SKIP = 2,
        HTTRANSPARENT = (-1),
        HTNOWHERE = 0,
        HTCLIENT = 1,
        HTLEFT = 10,
        HTBOTTOM = 15,
        HTBOTTOMLEFT = 16,
        HTBOTTOMRIGHT = 17,
        HTBORDER = 18,
        HELPINFO_WINDOW = 0x0001,
        HCF_HIGHCONTRASTON = 0x00000001,
        HDI_ORDER = 0x0080,
        HDI_WIDTH = 0x0001,
        HDM_GETITEMCOUNT = (0x1200 + 0),
        HDM_INSERTITEMW = (0x1200 + 10),
        HDM_GETITEMW = (0x1200 + 11),
        HDM_LAYOUT = (0x1200 + 5),
        HDM_SETITEMW = (0x1200 + 12),
        HDN_ITEMCHANGING = ((0 - 300) - 20),
        HDN_ITEMCHANGED = ((0 - 300) - 21),
        HDN_ITEMCLICK = ((0 - 300) - 22),
        HDN_ITEMDBLCLICK = ((0 - 300) - 23),
        HDN_DIVIDERDBLCLICK = ((0 - 300) - 25),
        HDN_BEGINTDRAG = ((0 - 300) - 10),
        HDN_BEGINTRACK = ((0 - 300) - 26),
        HDN_ENDDRAG = ((0 - 300) - 11),
        HDN_ENDTRACK = ((0 - 300) - 27),
        HDN_TRACK = ((0 - 300) - 28),
        HDN_GETDISPINFO = ((0 - 300) - 29);
        // HOVER_DEFAULT = Do not use this value ever! It crashes entire servers.

        public const int HDS_FULLDRAG = 0x0080;

        // Corresponds to bitmaps in MENUITEMINFO
        public const int HBMMENU_CALLBACK = -1,
        HBMMENU_SYSTEM = 1,
        HBMMENU_MBAR_RESTORE = 2,
        HBMMENU_MBAR_MINIMIZE = 3,
        HBMMENU_MBAR_CLOSE = 5,
        HBMMENU_MBAR_CLOSE_D = 6,
        HBMMENU_MBAR_MINIMIZE_D = 7,
        HBMMENU_POPUP_CLOSE = 8,
        HBMMENU_POPUP_RESTORE = 9,
        HBMMENU_POPUP_MAXIMIZE = 10,
        HBMMENU_POPUP_MINIMIZE = 11;

        public static HandleRef HWND_TOP = new HandleRef(null, (IntPtr)0);
        public static HandleRef HWND_BOTTOM = new HandleRef(null, (IntPtr)1);
        public static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
        public static HandleRef HWND_NOTOPMOST = new HandleRef(null, new IntPtr(-2));
        public static HandleRef HWND_MESSAGE = new HandleRef(null, new IntPtr(-3));

        public const int IME_CMODE_NATIVE = 0x0001,
        IME_CMODE_KATAKANA = 0x0002,
        IME_CMODE_FULLSHAPE = 0x0008,
        INPLACE_E_NOTOOLSPACE = unchecked((int)0x800401A1),
        ICON_SMALL = 0,
        ICON_BIG = 1,
        IMAGE_ICON = 1,
        IMAGE_CURSOR = 2,
        ICC_LISTVIEW_CLASSES = 0x00000001,
        ICC_TREEVIEW_CLASSES = 0x00000002,
        ICC_BAR_CLASSES = 0x00000004,
        ICC_TAB_CLASSES = 0x00000008,
        ICC_PROGRESS_CLASS = 0x00000020,
        ICC_DATE_CLASSES = 0x00000100,
        ILC_MASK = 0x0001,
        ILC_COLOR = 0x0000,
        ILC_COLOR4 = 0x0004,
        ILC_COLOR8 = 0x0008,
        ILC_COLOR16 = 0x0010,
        ILC_COLOR24 = 0x0018,
        ILC_COLOR32 = 0x0020,
        ILC_MIRROR = 0x00002000,
        ILD_NORMAL = 0x0000,
        ILD_TRANSPARENT = 0x0001,
        ILD_MASK = 0x0010,
        ILD_ROP = 0x0040,

        // ImageList
        //
        ILP_NORMAL = 0,
        ILP_DOWNLEVEL = 1,
        ILS_NORMAL = 0x0,
        ILS_GLOW = 0x1,
        ILS_SHADOW = 0x2,
        ILS_SATURATE = 0x4,
        ILS_ALPHA = 0x8;

        public const int IDM_PRINT = 27,
        IDM_PAGESETUP = 2004,
        IDM_PRINTPREVIEW = 2003,
        IDM_PROPERTIES = 28,
        IDM_SAVEAS = 71;

        public const int CSC_NAVIGATEFORWARD = 0x00000001,
        CSC_NAVIGATEBACK = 0x00000002;

        public const int INPUT_KEYBOARD = 1;

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int KEYEVENTF_UNICODE = 0x0004;

        public const int LB_ERR = (-1),
        LB_ERRSPACE = (-2),
        LBN_SELCHANGE = 1,
        LBN_DBLCLK = 2,
        LB_ADDSTRING = 0x0180,
        LB_INSERTSTRING = 0x0181,
        LB_DELETESTRING = 0x0182,
        LB_RESETCONTENT = 0x0184,
        LB_SETSEL = 0x0185,
        LB_SETCURSEL = 0x0186,
        LB_GETSEL = 0x0187,
        LB_SETCARETINDEX = 0x019E,
        LB_GETCARETINDEX = 0x019F,
        LB_GETCURSEL = 0x0188,
        LB_GETTEXT = 0x0189,
        LB_GETTEXTLEN = 0x018A,
        LB_GETCOUNT = 0x018B,
        LB_GETTOPINDEX = 0x018E,
        LB_FINDSTRING = 0x018F,
        LB_GETSELCOUNT = 0x0190,
        LB_GETSELITEMS = 0x0191,
        LB_SETTABSTOPS = 0x0192,
        LB_SETHORIZONTALEXTENT = 0x0194,
        LB_SETCOLUMNWIDTH = 0x0195,
        LB_SETTOPINDEX = 0x0197,
        LB_GETITEMRECT = 0x0198,
        LB_SETITEMHEIGHT = 0x01A0,
        LB_GETITEMHEIGHT = 0x01A1,
        LB_FINDSTRINGEXACT = 0x01A2,
        LB_ITEMFROMPOINT = 0x01A9,
        LB_SETLOCALE = 0x01A5;

        public const int LBS_NOTIFY = 0x0001,
        LBS_MULTIPLESEL = 0x0008,
        LBS_OWNERDRAWFIXED = 0x0010,
        LBS_OWNERDRAWVARIABLE = 0x0020,
        LBS_HASSTRINGS = 0x0040,
        LBS_USETABSTOPS = 0x0080,
        LBS_NOINTEGRALHEIGHT = 0x0100,
        LBS_MULTICOLUMN = 0x0200,
        LBS_WANTKEYBOARDINPUT = 0x0400,
        LBS_EXTENDEDSEL = 0x0800,
        LBS_DISABLENOSCROLL = 0x1000,
        LBS_NOSEL = 0x4000,
        LOCK_WRITE = 0x1,
        LOCK_EXCLUSIVE = 0x2,
        LOCK_ONLYONCE = 0x4,
        LV_VIEW_TILE = 0x0004,
        LVBKIF_SOURCE_NONE = 0x0000,
        LVBKIF_SOURCE_URL = 0x0002,
        LVBKIF_STYLE_NORMAL = 0x0000,
        LVBKIF_STYLE_TILE = 0x0010,
        LVS_ICON = 0x0000,
        LVS_REPORT = 0x0001,
        LVS_SMALLICON = 0x0002,
        LVS_LIST = 0x0003,
        LVS_SINGLESEL = 0x0004,
        LVS_SHOWSELALWAYS = 0x0008,
        LVS_SORTASCENDING = 0x0010,
        LVS_SORTDESCENDING = 0x0020,
        LVS_SHAREIMAGELISTS = 0x0040,
        LVS_NOLABELWRAP = 0x0080,
        LVS_AUTOARRANGE = 0x0100,
        LVS_EDITLABELS = 0x0200,
        LVS_NOSCROLL = 0x2000,
        LVS_ALIGNTOP = 0x0000,
        LVS_ALIGNLEFT = 0x0800,
        LVS_NOCOLUMNHEADER = 0x4000,
        LVS_NOSORTHEADER = unchecked((int)0x8000),
        LVS_OWNERDATA = 0x1000,
        LVSCW_AUTOSIZE = -1,
        LVSCW_AUTOSIZE_USEHEADER = -2,
        LVM_REDRAWITEMS = (0x1000 + 21),
        LVM_SCROLL = (0x1000 + 20),
        LVM_SETBKCOLOR = (0x1000 + 1),
        LVM_SETBKIMAGE = (0x1000 + 138),
        LVM_SETCALLBACKMASK = (0x1000 + 11),
        LVM_GETCALLBACKMASK = (0x1000 + 10),
        LVM_GETCOLUMNORDERARRAY = (0x1000 + 59),
        LVM_GETITEMCOUNT = (0x1000 + 4),
        LVM_SETCOLUMNORDERARRAY = (0x1000 + 58),
        LVM_SETINFOTIP = (0x1000 + 173),
        LVSIL_NORMAL = 0,
        LVSIL_SMALL = 1,
        LVSIL_STATE = 2,
        LVM_SETIMAGELIST = (0x1000 + 3),
        LVM_SETSELECTIONMARK = (0x1000 + 67),
        LVM_SETTOOLTIPS = (0x1000 + 74),
        LVIF_TEXT = 0x0001,
        LVIF_IMAGE = 0x0002,
        LVIF_INDENT = 0x0010,
        LVIF_PARAM = 0x0004,
        LVIF_STATE = 0x0008,
        LVIF_GROUPID = 0x0100,
        LVIF_COLUMNS = 0x0200,
        LVIS_FOCUSED = 0x0001,
        LVIS_SELECTED = 0x0002,
        LVIS_CUT = 0x0004,
        LVIS_DROPHILITED = 0x0008,
        LVIS_OVERLAYMASK = 0x0F00,
        LVIS_STATEIMAGEMASK = 0xF000,
        LVM_GETITEM = (0x1000 + 75),
        LVM_SETITEM = (0x1000 + 76),
        LVM_SETITEMPOSITION32 = (0x01000 + 49),
        LVM_INSERTITEM = (0x1000 + 77),
        LVM_DELETEITEM = (0x1000 + 8),
        LVM_DELETECOLUMN = (0x1000 + 28),
        LVM_DELETEALLITEMS = (0x1000 + 9),
        LVM_UPDATE = (0x1000 + 42),
        LVNI_FOCUSED = 0x0001,
        LVNI_SELECTED = 0x0002,
        LVM_GETNEXTITEM = (0x1000 + 12),
        LVFI_PARAM = 0x0001,
        LVFI_NEARESTXY = 0x0040,
        LVFI_PARTIAL = 0x0008,
        LVFI_STRING = 0x0002,
        LVM_FINDITEM = (0x1000 + 83),
        LVIR_BOUNDS = 0,
        LVIR_ICON = 1,
        LVIR_LABEL = 2,
        LVIR_SELECTBOUNDS = 3,
        LVM_GETITEMPOSITION = (0x1000 + 16),
        LVM_GETITEMRECT = (0x1000 + 14),
        LVM_GETSUBITEMRECT = (0x1000 + 56),
        LVM_GETSTRINGWIDTH = (0x1000 + 87),
        LVHT_NOWHERE = 0x0001,
        LVHT_ONITEMICON = 0x0002,
        LVHT_ONITEMLABEL = 0x0004,
        LVHT_ABOVE = 0x0008,
        LVHT_BELOW = 0x0010,
        LVHT_RIGHT = 0x0020,
        LVHT_LEFT = 0x0040,
        LVHT_ONITEM = (0x0002 | 0x0004 | 0x0008),
        LVHT_ONITEMSTATEICON = 0x0008,
        LVM_SUBITEMHITTEST = (0x1000 + 57),
        LVM_HITTEST = (0x1000 + 18),
        LVM_ENSUREVISIBLE = (0x1000 + 19),
        LVA_DEFAULT = 0x0000,
        LVA_ALIGNLEFT = 0x0001,
        LVA_ALIGNTOP = 0x0002,
        LVA_SNAPTOGRID = 0x0005,
        LVM_ARRANGE = (0x1000 + 22),
        LVM_EDITLABEL = (0x1000 + 118),
        LVCDI_ITEM = 0x0000,
        LVCDI_GROUP = 0x00000001,
        LVCF_FMT = 0x0001,
        LVCF_WIDTH = 0x0002,
        LVCF_TEXT = 0x0004,
        LVCF_SUBITEM = 0x0008,
        LVCF_IMAGE = 0x0010,
        LVCF_ORDER = 0x0020,
        LVCFMT_IMAGE = 0x0800,
        LVGA_HEADER_LEFT = 0x00000001,
        LVGA_HEADER_CENTER = 0x00000002,
        LVGA_HEADER_RIGHT = 0x00000004,
        LVGA_FOOTER_LEFT = 0x00000008,
        LVGA_FOOTER_CENTER = 0x00000010,
        LVGA_FOOTER_RIGHT = 0x00000020,
        LVGF_NONE = 0x00000000,
        LVGF_HEADER = 0x00000001,
        LVGF_FOOTER = 0x00000002,
        LVGF_STATE = 0x00000004,
        LVGF_ALIGN = 0x00000008,
        LVGF_GROUPID = 0x00000010,
        LVGS_NORMAL = 0x00000000,
        LVGS_COLLAPSED = 0x00000001,
        LVGS_HIDDEN = 0x00000002,
        LVIM_AFTER = 0x00000001,
        LVTVIF_FIXEDSIZE = 0x00000003,
        LVTVIM_TILESIZE = 0x00000001,
        LVTVIM_COLUMNS = 0x00000002,
        LVM_ENABLEGROUPVIEW = (0x1000 + 157),
        LVM_MOVEITEMTOGROUP = (0x1000 + 154),
        LVM_GETCOLUMN = (0x1000 + 95),
        LVM_SETCOLUMN = (0x1000 + 96),
        LVM_INSERTCOLUMN = (0x1000 + 97),
        LVM_INSERTGROUP = (0x1000 + 145),
        LVM_REMOVEGROUP = (0x1000 + 150),
        LVM_INSERTMARKHITTEST = (0x1000 + 168),
        LVM_REMOVEALLGROUPS = (0x1000 + 160),
        LVM_GETCOLUMNWIDTH = (0x1000 + 29),
        LVM_SETCOLUMNWIDTH = (0x1000 + 30),
        LVM_SETINSERTMARK = (0x1000 + 166),
        LVM_GETHEADER = (0x1000 + 31),
        LVM_SETTEXTCOLOR = (0x1000 + 36),
        LVM_SETTEXTBKCOLOR = (0x1000 + 38),
        LVM_GETTOPINDEX = (0x1000 + 39),
        LVM_SETITEMPOSITION = (0x1000 + 15),
        LVM_SETITEMSTATE = (0x1000 + 43),
        LVM_GETITEMSTATE = (0x1000 + 44),
        LVM_GETITEMTEXT = (0x1000 + 115),
        LVM_GETHOTITEM = (0x1000 + 61),
        LVM_SETITEMTEXT = (0x1000 + 116),
        LVM_SETITEMCOUNT = (0x1000 + 47),
        LVM_SORTITEMS = (0x1000 + 48),
        LVM_GETSELECTEDCOUNT = (0x1000 + 50),
        LVM_GETISEARCHSTRING = (0x1000 + 117),
        LVM_SETEXTENDEDLISTVIEWSTYLE = (0x1000 + 54),
        LVM_SETVIEW = (0x1000 + 142),
        LVM_GETGROUPINFO = (0x1000 + 149),
        LVM_SETGROUPINFO = (0x1000 + 147),
        LVM_HASGROUP = (0x1000 + 161),
        LVM_SETTILEVIEWINFO = (0x1000 + 162),
        LVM_GETTILEVIEWINFO = (0x1000 + 163),
        LVM_GETINSERTMARK = (0x1000 + 167),
        LVM_GETINSERTMARKRECT = (0x1000 + 169),
        LVM_SETINSERTMARKCOLOR = (0x1000 + 170),
        LVM_GETINSERTMARKCOLOR = (0x1000 + 171),
        LVM_ISGROUPVIEWENABLED = (0x1000 + 175),
        LVS_EX_GRIDLINES = 0x00000001,
        LVS_EX_CHECKBOXES = 0x00000004,
        LVS_EX_TRACKSELECT = 0x00000008,
        LVS_EX_HEADERDRAGDROP = 0x00000010,
        LVS_EX_FULLROWSELECT = 0x00000020,
        LVS_EX_ONECLICKACTIVATE = 0x00000040,
        LVS_EX_TWOCLICKACTIVATE = 0x00000080,
        LVS_EX_INFOTIP = 0x00000400,
        LVS_EX_UNDERLINEHOT = 0x00000800,
        LVS_EX_DOUBLEBUFFER = 0x00010000,
        LVN_ITEMCHANGING = ((0 - 100) - 0),
        LVN_ITEMCHANGED = ((0 - 100) - 1),
        LVN_BEGINLABELEDIT = ((0 - 100) - 75),
        LVN_ENDLABELEDIT = ((0 - 100) - 76),
        LVN_COLUMNCLICK = ((0 - 100) - 8),
        LVN_BEGINDRAG = ((0 - 100) - 9),
        LVN_BEGINRDRAG = ((0 - 100) - 11),
        LVN_ODFINDITEM = ((0 - 100) - 79),
        LVN_ITEMACTIVATE = ((0 - 100) - 14),
        LVN_GETDISPINFO = ((0 - 100) - 77),
        LVN_ODCACHEHINT = ((0 - 100) - 13),
        LVN_ODSTATECHANGED = ((0 - 100) - 15),
        LVN_SETDISPINFO = ((0 - 100) - 78),
        LVN_GETINFOTIP = ((0 - 100) - 58),
        LVN_KEYDOWN = ((0 - 100) - 55),

        LWA_COLORKEY = 0x00000001,
        LWA_ALPHA = 0x00000002;

        public const int LANG_NEUTRAL = 0x00,
                         LOCALE_IFIRSTDAYOFWEEK = 0x0000100C;   /* first day of week specifier */

        public const int LOCALE_IMEASURE = 0x0000000D;   // 0 = metric, 1 = US

        public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
        public const int TVM_GETEXTENDEDSTYLE = TV_FIRST + 45;

        public const int TVS_EX_FADEINOUTEXPANDOS = 0x0040;
        public const int TVS_EX_DOUBLEBUFFER = 0x0004;

        public static readonly int LOCALE_USER_DEFAULT = MAKELCID(LANG_USER_DEFAULT);
        public static readonly int LANG_USER_DEFAULT = MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT);

        public static int MAKELANGID(int primary, int sub)
        {
            return ((((ushort)(sub)) << 10) | (ushort)(primary));
        }

        /// <summary>
        ///  Creates an LCID from a LangId
        /// </summary>
        public static int MAKELCID(int lgid)
        {
            return MAKELCID(lgid, SORT_DEFAULT);
        }

        /// <summary>
        ///  Creates an LCID from a LangId
        /// </summary>
        public static int MAKELCID(int lgid, int sort)
        {
            return ((0xFFFF & lgid) | (((0x000f) & sort) << 16));
        }

        public const int MEMBERID_NIL = (-1),
        ERROR_INSUFFICIENT_BUFFER = 122, //https://msdn.microsoft.com/en-us/library/windows/desktop/ms681382(v=vs.85).aspx
        MA_ACTIVATE = 0x0001,
        MA_ACTIVATEANDEAT = 0x0002,
        MA_NOACTIVATE = 0x0003,
        MA_NOACTIVATEANDEAT = 0x0004,
        MM_TEXT = 1,
        MM_ANISOTROPIC = 8,
        MK_LBUTTON = 0x0001,
        MK_RBUTTON = 0x0002,
        MK_SHIFT = 0x0004,
        MK_CONTROL = 0x0008,
        MK_MBUTTON = 0x0010,
        MNC_EXECUTE = 2,
        MNC_SELECT = 3,
        MIIM_STATE = 0x00000001,
        MIIM_ID = 0x00000002,
        MIIM_SUBMENU = 0x00000004,
        MIIM_TYPE = 0x00000010,
        MIIM_DATA = 0x00000020,
        MIIM_STRING = 0x00000040,
        MIIM_BITMAP = 0x00000080,
        MIIM_FTYPE = 0x00000100,
        MB_OK = 0x00000000,
        MF_BYCOMMAND = 0x00000000,
        MF_BYPOSITION = 0x00000400,
        MF_ENABLED = 0x00000000,
        MF_GRAYED = 0x00000001,
        MF_POPUP = 0x00000010,
        MF_SYSMENU = 0x00002000,
        MFS_DISABLED = 0x00000003,
        MFT_MENUBREAK = 0x00000040,
        MFT_SEPARATOR = 0x00000800,
        MFT_RIGHTORDER = 0x00002000,
        MFT_RIGHTJUSTIFY = 0x00004000,
        MDIS_ALLCHILDSTYLES = 0x0001,
        MDITILE_VERTICAL = 0x0000,
        MDITILE_HORIZONTAL = 0x0001,
        MDITILE_SKIPDISABLED = 0x0002,
        MCM_SETMAXSELCOUNT = (0x1000 + 4),
        MCM_SETSELRANGE = (0x1000 + 6),
        MCM_GETMONTHRANGE = (0x1000 + 7),
        MCM_GETMINREQRECT = (0x1000 + 9),
        MCM_SETCOLOR = (0x1000 + 10),
        MCM_SETTODAY = (0x1000 + 12),
        MCM_GETTODAY = (0x1000 + 13),
        MCM_HITTEST = (0x1000 + 14),
        MCM_SETFIRSTDAYOFWEEK = (0x1000 + 15),
        MCM_SETRANGE = (0x1000 + 18),
        MCM_SETMONTHDELTA = (0x1000 + 20),
        MCM_GETMAXTODAYWIDTH = (0x1000 + 21),
        MCHT_TITLE = 0x00010000,
        MCHT_CALENDAR = 0x00020000,
        MCHT_TODAYLINK = 0x00030000,
        MCHT_TITLEBK = (0x00010000),
        MCHT_TITLEMONTH = (0x00010000 | 0x0001),
        MCHT_TITLEYEAR = (0x00010000 | 0x0002),
        MCHT_TITLEBTNNEXT = (0x00010000 | 0x01000000 | 0x0003),
        MCHT_TITLEBTNPREV = (0x00010000 | 0x02000000 | 0x0003),
        MCHT_CALENDARBK = (0x00020000),
        MCHT_CALENDARDATE = (0x00020000 | 0x0001),
        MCHT_CALENDARDATENEXT = ((0x00020000 | 0x0001) | 0x01000000),
        MCHT_CALENDARDATEPREV = ((0x00020000 | 0x0001) | 0x02000000),
        MCHT_CALENDARDAY = (0x00020000 | 0x0002),
        MCHT_CALENDARWEEKNUM = (0x00020000 | 0x0003),
        MCSC_TEXT = 1,
        MCSC_TITLEBK = 2,
        MCSC_TITLETEXT = 3,
        MCSC_MONTHBK = 4,
        MCSC_TRAILINGTEXT = 5,
        MCN_VIEWCHANGE = (0 - 750), // MCN_SELECT -4  - give state of calendar view
        MCN_SELCHANGE = ((0 - 750) + 1),
        MCN_GETDAYSTATE = ((0 - 750) + 3),
        MCN_SELECT = ((0 - 750) + 4),
        MCS_DAYSTATE = 0x0001,
        MCS_MULTISELECT = 0x0002,
        MCS_WEEKNUMBERS = 0x0004,
        MCS_NOTODAYCIRCLE = 0x0008,
        MCS_NOTODAY = 0x0010,
        MSAA_MENU_SIG = (unchecked((int)0xAA0DF00D));

        public const int NIM_ADD = 0x00000000,
        NIM_MODIFY = 0x00000001,
        NIM_DELETE = 0x00000002,
        NIF_MESSAGE = 0x00000001,
        NIM_SETVERSION = 0x00000004,
        NIF_ICON = 0x00000002,
        NIF_INFO = 0x00000010,
        NIF_TIP = 0x00000004,
        NIIF_NONE = 0x00000000,
        NIIF_INFO = 0x00000001,
        NIIF_WARNING = 0x00000002,
        NIIF_ERROR = 0x00000003,
        NIN_BALLOONSHOW = (Interop.WindowMessages.WM_USER + 2),
        NIN_BALLOONHIDE = (Interop.WindowMessages.WM_USER + 3),
        NIN_BALLOONTIMEOUT = (Interop.WindowMessages.WM_USER + 4),
        NIN_BALLOONUSERCLICK = (Interop.WindowMessages.WM_USER + 5),
        NFR_ANSI = 1,
        NFR_UNICODE = 2,
        NM_CLICK = ((0 - 0) - 2),
        NM_DBLCLK = ((0 - 0) - 3),
        NM_RCLICK = ((0 - 0) - 5),
        NM_RDBLCLK = ((0 - 0) - 6),
        NM_CUSTOMDRAW = ((0 - 0) - 12),
        NM_RELEASEDCAPTURE = ((0 - 0) - 16);

        public const int OFN_READONLY = 0x00000001,
        OFN_OVERWRITEPROMPT = 0x00000002,
        OFN_HIDEREADONLY = 0x00000004,
        OFN_NOCHANGEDIR = 0x00000008,
        OFN_SHOWHELP = 0x00000010,
        OFN_ENABLEHOOK = 0x00000020,
        OFN_NOVALIDATE = 0x00000100,
        OFN_ALLOWMULTISELECT = 0x00000200,
        OFN_PATHMUSTEXIST = 0x00000800,
        OFN_FILEMUSTEXIST = 0x00001000,
        OFN_CREATEPROMPT = 0x00002000,
        OFN_EXPLORER = 0x00080000,
        OFN_NODEREFERENCELINKS = 0x00100000,
        OFN_ENABLESIZING = 0x00800000,
        OFN_USESHELLITEM = 0x01000000,
        OLEIVERB_PRIMARY = 0,
        OLEIVERB_SHOW = -1,
        OLEIVERB_HIDE = -3,
        OLEIVERB_UIACTIVATE = -4,
        OLEIVERB_INPLACEACTIVATE = -5,
        OLEIVERB_DISCARDUNDOSTATE = -6,
        OLEIVERB_PROPERTIES = -7,
        OLE_E_INVALIDRECT = unchecked((int)0x8004000D),
        OLE_E_NOCONNECTION = unchecked((int)0x80040004),
        OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C),
        OLEMISC_RECOMPOSEONRESIZE = 0x00000001,
        OLEMISC_INSIDEOUT = 0x00000080,
        OLEMISC_ACTIVATEWHENVISIBLE = 0x0000100,
        OLEMISC_ACTSLIKEBUTTON = 0x00001000,
        OLEMISC_SETCLIENTSITEFIRST = 0x00020000,
        ODS_CHECKED = 0x0008,
        ODS_COMBOBOXEDIT = 0x1000,
        ODS_DEFAULT = 0x0020,
        ODS_DISABLED = 0x0004,
        ODS_FOCUS = 0x0010,
        ODS_GRAYED = 0x0002,
        ODS_HOTLIGHT = 0x0040,
        ODS_INACTIVE = 0x0080,
        ODS_NOACCEL = 0x0100,
        ODS_NOFOCUSRECT = 0x0200,
        ODS_SELECTED = 0x0001,
        OLECLOSE_SAVEIFDIRTY = 0,
        OLECLOSE_PROMPTSAVE = 2;

        public const int PDERR_SETUPFAILURE = 0x1001,
        PDERR_PARSEFAILURE = 0x1002,
        PDERR_RETDEFFAILURE = 0x1003,
        PDERR_LOADDRVFAILURE = 0x1004,
        PDERR_GETDEVMODEFAIL = 0x1005,
        PDERR_INITFAILURE = 0x1006,
        PDERR_NODEVICES = 0x1007,
        PDERR_NODEFAULTPRN = 0x1008,
        PDERR_DNDMMISMATCH = 0x1009,
        PDERR_CREATEICFAILURE = 0x100A,
        PDERR_PRINTERNOTFOUND = 0x100B,
        PDERR_DEFAULTDIFFERENT = 0x100C,
        PD_ALLPAGES = 0x00000000,
        PD_SELECTION = 0x00000001,
        PD_PAGENUMS = 0x00000002,
        PD_NOSELECTION = 0x00000004,
        PD_NOPAGENUMS = 0x00000008,
        PD_COLLATE = 0x00000010,
        PD_PRINTTOFILE = 0x00000020,
        PD_PRINTSETUP = 0x00000040,
        PD_NOWARNING = 0x00000080,
        PD_RETURNDC = 0x00000100,
        PD_RETURNIC = 0x00000200,
        PD_RETURNDEFAULT = 0x00000400,
        PD_SHOWHELP = 0x00000800,
        PD_ENABLEPRINTHOOK = 0x00001000,
        PD_ENABLESETUPHOOK = 0x00002000,
        PD_ENABLEPRINTTEMPLATE = 0x00004000,
        PD_ENABLESETUPTEMPLATE = 0x00008000,
        PD_ENABLEPRINTTEMPLATEHANDLE = 0x00010000,
        PD_ENABLESETUPTEMPLATEHANDLE = 0x00020000,
        PD_USEDEVMODECOPIES = 0x00040000,
        PD_USEDEVMODECOPIESANDCOLLATE = 0x00040000,
        PD_DISABLEPRINTTOFILE = 0x00080000,
        PD_HIDEPRINTTOFILE = 0x00100000,
        PD_NONETWORKBUTTON = 0x00200000,
        PD_CURRENTPAGE = 0x00400000,
        PD_NOCURRENTPAGE = 0x00800000,
        PD_EXCLUSIONFLAGS = 0x01000000,
        PD_USELARGETEMPLATE = 0x10000000,
        PSD_MINMARGINS = 0x00000001,
        PSD_MARGINS = 0x00000002,
        PSD_INHUNDREDTHSOFMILLIMETERS = 0x00000008,
        PSD_DISABLEMARGINS = 0x00000010,
        PSD_DISABLEPRINTER = 0x00000020,
        PSD_DISABLEORIENTATION = 0x00000100,
        PSD_DISABLEPAPER = 0x00000200,
        PSD_SHOWHELP = 0x00000800,
        PSD_ENABLEPAGESETUPHOOK = 0x00002000,
        PSD_NONETWORKBUTTON = 0x00200000,
        PS_SOLID = 0,
        PS_DOT = 2,
        PRF_CHECKVISIBLE = 0x00000001,
        PRF_NONCLIENT = 0x00000002,
        PRF_CLIENT = 0x00000004,
        PRF_ERASEBKGND = 0x00000008,
        PRF_CHILDREN = 0x00000010,
        PM_NOREMOVE = 0x0000,
        PM_REMOVE = 0x0001,
        PM_NOYIELD = 0x0002,
        PBM_SETRANGE = (0x0400 + 1),
        PBM_SETPOS = (0x0400 + 2),
        PBM_SETSTEP = (0x0400 + 4),
        PBM_SETRANGE32 = (0x0400 + 6),
        PBM_SETBARCOLOR = (0x0400 + 9),
        PBM_SETMARQUEE = (0x0400 + 10),
        PBM_SETBKCOLOR = (0x2000 + 1),
        PSM_SETTITLEA = (0x0400 + 111),
        PSM_SETTITLEW = (0x0400 + 120),
        PSM_SETFINISHTEXTA = (0x0400 + 115),
        PSM_SETFINISHTEXTW = (0x0400 + 121),
        PATCOPY = 0x00F00021,
        PATINVERT = 0x005A0049;

        public const int PBS_SMOOTH = 0x01,
        PBS_MARQUEE = 0x08;

        public const int QS_KEY = 0x0001,
        QS_MOUSEMOVE = 0x0002,
        QS_MOUSEBUTTON = 0x0004,
        QS_POSTMESSAGE = 0x0008,
        QS_TIMER = 0x0010,
        QS_PAINT = 0x0020,
        QS_SENDMESSAGE = 0x0040,
        QS_HOTKEY = 0x0080,
        QS_ALLPOSTMESSAGE = 0x0100,
        QS_MOUSE = QS_MOUSEMOVE | QS_MOUSEBUTTON,
        QS_INPUT = QS_MOUSE | QS_KEY,
        QS_ALLEVENTS = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY,
        QS_ALLINPUT = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY | QS_SENDMESSAGE;

        public const int MWMO_INPUTAVAILABLE = 0x0004;  // don't use MWMO_WAITALL, see ddb#176342

        //public const int RECO_PASTE = 0x00000000;   // paste from clipboard
        public const int RECO_DROP = 0x00000001;    // drop
                                                    //public const int RECO_COPY  = 0x00000002;    // copy to the clipboard
                                                    //public const int RECO_CUT   = 0x00000003; // cut to the clipboard
                                                    //public const int RECO_DRAG  = 0x00000004;    // drag

        public const int RPC_E_CHANGED_MODE = unchecked((int)0x80010106),
            RPC_E_CANTCALLOUT_ININPUTSYNCCALL = unchecked((int)0x8001010D),
            RDW_INVALIDATE = 0x0001,
            RDW_ERASE = 0x0004,
            RDW_ALLCHILDREN = 0x0080,
            RDW_ERASENOW = 0x0200,
            RDW_UPDATENOW = 0x0100,
            RDW_FRAME = 0x0400,
            RB_INSERTBANDA = (0x0400 + 1),
            RB_INSERTBANDW = (0x0400 + 10);

        public const int stc4 = 0x0443,
        STARTF_USESHOWWINDOW = 0x00000001,
        SB_HORZ = 0,
        SB_VERT = 1,
        SB_CTL = 2,
        SB_LINEUP = 0,
        SB_LINELEFT = 0,
        SB_LINEDOWN = 1,
        SB_LINERIGHT = 1,
        SB_PAGEUP = 2,
        SB_PAGELEFT = 2,
        SB_PAGEDOWN = 3,
        SB_PAGERIGHT = 3,
        SB_THUMBPOSITION = 4,
        SB_THUMBTRACK = 5,
        SB_LEFT = 6,
        SB_RIGHT = 7,
        SB_ENDSCROLL = 8,
        SB_TOP = 6,
        SB_BOTTOM = 7,
        SIZE_RESTORED = 0,
        SIZE_MAXIMIZED = 2,
        ESB_ENABLE_BOTH = 0x0000,
        ESB_DISABLE_BOTH = 0x0003,
        SORT_DEFAULT = 0x0,
        SUBLANG_DEFAULT = 0x01,
        SW_HIDE = 0,
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_MAX = 10,
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOACTIVATE = 0x0010,
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_DRAWFRAME = 0x0020,
        SWP_NOOWNERZORDER = 0x0200;

        public const int HLP_FILE = 1,
        HLP_KEYWORD = 2,
        HLP_NAVIGATOR = 3,
        HLP_OBJECT = 4;

        public const int SW_SCROLLCHILDREN = 0x0001,
        SW_INVALIDATE = 0x0002,
        SW_ERASE = 0x0004,
        SW_SMOOTHSCROLL = 0x0010,
        SC_SIZE = 0xF000,
        SC_MINIMIZE = 0xF020,
        SC_MAXIMIZE = 0xF030,
        SC_CLOSE = 0xF060,
        SC_KEYMENU = 0xF100,
        SC_RESTORE = 0xF120,
        SC_MOVE = 0xF010,
        SC_CONTEXTHELP = 0xF180,
        SS_LEFT = 0x00000000,
        SS_CENTER = 0x00000001,
        SS_RIGHT = 0x00000002,
        SS_OWNERDRAW = 0x0000000D,
        SS_NOPREFIX = 0x00000080,
        SS_SUNKEN = 0x00001000,
        SBS_HORZ = 0x0000,
        SBS_VERT = 0x0001,
        SIF_RANGE = 0x0001,
        SIF_PAGE = 0x0002,
        SIF_POS = 0x0004,
        SIF_TRACKPOS = 0x0010,
        SIF_ALL = (0x0001 | 0x0002 | 0x0004 | 0x0010),
        SPI_GETFONTSMOOTHING = 0x004A,
        SPI_GETDROPSHADOW = 0x1024,
        SPI_GETFLATMENU = 0x1022,
        SPI_GETFONTSMOOTHINGTYPE = 0x200A,
        SPI_GETFONTSMOOTHINGCONTRAST = 0x200C,
        SPI_ICONHORIZONTALSPACING = 0x000D,
        SPI_ICONVERTICALSPACING = 0x0018,
        // SPI_GETICONMETRICS =        0x002D,
        SPI_GETICONTITLEWRAP = 0x0019,
        SPI_GETKEYBOARDCUES = 0x100A,
        SPI_GETKEYBOARDDELAY = 0x0016,
        SPI_GETKEYBOARDPREF = 0x0044,
        SPI_GETKEYBOARDSPEED = 0x000A,
        SPI_GETMOUSEHOVERWIDTH = 0x0062,
        SPI_GETMOUSEHOVERHEIGHT = 0x0064,
        SPI_GETMOUSEHOVERTIME = 0x0066,
        SPI_GETMOUSESPEED = 0x0070,
        SPI_GETMENUDROPALIGNMENT = 0x001B,
        SPI_GETMENUFADE = 0x1012,
        SPI_GETMENUSHOWDELAY = 0x006A,
        SPI_GETCOMBOBOXANIMATION = 0x1004,
        SPI_GETGRADIENTCAPTIONS = 0x1008,
        SPI_GETHOTTRACKING = 0x100E,
        SPI_GETLISTBOXSMOOTHSCROLLING = 0x1006,
        SPI_GETMENUANIMATION = 0x1002,
        SPI_GETSELECTIONFADE = 0x1014,
        SPI_GETTOOLTIPANIMATION = 0x1016,
        SPI_GETUIEFFECTS = 0x103E,
        SPI_GETACTIVEWINDOWTRACKING = 0x1000,
        SPI_GETACTIVEWNDTRKTIMEOUT = 0x2002,
        SPI_GETANIMATION = 0x0048,
        SPI_GETBORDER = 0x0005,
        SPI_GETCARETWIDTH = 0x2006,
        SPI_GETDRAGFULLWINDOWS = 38,
        SPI_GETNONCLIENTMETRICS = 41,
        SPI_GETWORKAREA = 48,
        SPI_GETHIGHCONTRAST = 66,
        SPI_GETDEFAULTINPUTLANG = 89,
        SPI_GETSNAPTODEFBUTTON = 95,
        SPI_GETWHEELSCROLLLINES = 104,
        SBARS_SIZEGRIP = 0x0100,
        SB_SETTEXT = (0x0400 + 11),
        SB_GETTEXT = (0x0400 + 13),
        SB_GETTEXTLENGTH = (0x0400 + 12),
        SB_SETPARTS = (0x0400 + 4),
        SB_SIMPLE = (0x0400 + 9),
        SB_GETRECT = (0x0400 + 10),
        SB_SETICON = (0x0400 + 15),
        SB_SETTIPTEXT = (0x0400 + 17),
        SB_GETTIPTEXT = (0x0400 + 19),
        SBT_OWNERDRAW = 0x1000,
        SBT_NOBORDERS = 0x0100,
        SBT_POPOUT = 0x0200,
        SBT_RTLREADING = 0x0400,
        SRCCOPY = 0x00CC0020;

        public const int S_OK = 0x00000000;
        public const int S_FALSE = 0x00000001;

        public static bool Succeeded(int hr)
        {
            return (hr >= 0);
        }

        public static bool Failed(int hr)
        {
            return (hr < 0);
        }

        public const int TRANSPARENT = 1,
        OPAQUE = 2,
        TME_HOVER = 0x00000001,
        TME_LEAVE = 0x00000002,
        TPM_LEFTBUTTON = 0x0000,
        TPM_RIGHTBUTTON = 0x0002,
        TPM_LEFTALIGN = 0x0000,
        TPM_RIGHTALIGN = 0x0008,
        TPM_VERTICAL = 0x0040,
        TV_FIRST = 0x1100,
        TBSTATE_CHECKED = 0x01,
        TBSTATE_ENABLED = 0x04,
        TBSTATE_HIDDEN = 0x08,
        TBSTATE_INDETERMINATE = 0x10,
        TBSTYLE_BUTTON = 0x00,
        TBSTYLE_SEP = 0x01,
        TBSTYLE_CHECK = 0x02,
        TBSTYLE_DROPDOWN = 0x08,
        TBSTYLE_TOOLTIPS = 0x0100,
        TBSTYLE_FLAT = 0x0800,
        TBSTYLE_LIST = 0x1000,
        TBSTYLE_EX_DRAWDDARROWS = 0x00000001,
        TB_ENABLEBUTTON = (0x0400 + 1),
        TB_ISBUTTONCHECKED = (0x0400 + 10),
        TB_ISBUTTONINDETERMINATE = (0x0400 + 13),
        TB_ADDBUTTONS = (0x0400 + 68),
        TB_INSERTBUTTON = (0x0400 + 67),
        TB_DELETEBUTTON = (0x0400 + 22),
        TB_GETBUTTON = (0x0400 + 23),
        TB_SAVERESTORE = (0x0400 + 76),
        TB_ADDSTRING = (0x0400 + 77),
        TB_BUTTONSTRUCTSIZE = (0x0400 + 30),
        TB_SETBUTTONSIZE = (0x0400 + 31),
        TB_AUTOSIZE = (0x0400 + 33),
        TB_GETROWS = (0x0400 + 40),
        TB_GETBUTTONTEXT = (0x0400 + 75),
        TB_SETIMAGELIST = (0x0400 + 48),
        TB_GETRECT = (0x0400 + 51),
        TB_GETBUTTONSIZE = (0x0400 + 58),
        TB_GETBUTTONINFO = (0x0400 + 63),
        TB_SETBUTTONINFO = (0x0400 + 64),
        TB_SETEXTENDEDSTYLE = (0x0400 + 84),
        TB_MAPACCELERATOR = (0x0400 + 90),
        TB_GETTOOLTIPS = (0x0400 + 35),
        TB_SETTOOLTIPS = (0x0400 + 36),
        TBIF_IMAGE = 0x00000001,
        TBIF_TEXT = 0x00000002,
        TBIF_STATE = 0x00000004,
        TBIF_STYLE = 0x00000008,
        TBIF_COMMAND = 0x00000020,
        TBIF_SIZE = 0x00000040,
        TBN_GETBUTTONINFO = ((0 - 700) - 20),
        TBN_QUERYINSERT = ((0 - 700) - 6),
        TBN_DROPDOWN = ((0 - 700) - 10),
        TBN_HOTITEMCHANGE = ((0 - 700) - 13),
        TBN_GETDISPINFO = ((0 - 700) - 17),
        TBN_GETINFOTIP = ((0 - 700) - 19),
        TTS_ALWAYSTIP = 0x01,
        TTS_NOPREFIX = 0x02,
        TTS_NOANIMATE = 0x10,
        TTS_NOFADE = 0x20,
        TTS_BALLOON = 0x40,
        //TTI_NONE                =0,
        //TTI_INFO                =1,
        TTI_WARNING = 2,
        //TTI_ERROR               =3,
        TTN_GETDISPINFO = ((0 - 520) - 10),
        TTN_SHOW = ((0 - 520) - 1),
        TTN_POP = ((0 - 520) - 2),
        TTN_NEEDTEXT = ((0 - 520) - 10),
        TBS_AUTOTICKS = 0x0001,
        TBS_VERT = 0x0002,
        TBS_TOP = 0x0004,
        TBS_BOTTOM = 0x0000,
        TBS_BOTH = 0x0008,
        TBS_NOTICKS = 0x0010,
        TBM_GETPOS = (0x0400),
        TBM_SETTIC = (0x0400 + 4),
        TBM_SETPOS = (0x0400 + 5),
        TBM_SETRANGE = (0x0400 + 6),
        TBM_SETRANGEMIN = (0x0400 + 7),
        TBM_SETRANGEMAX = (0x0400 + 8),
        TBM_SETTICFREQ = (0x0400 + 20),
        TBM_SETPAGESIZE = (0x0400 + 21),
        TBM_SETLINESIZE = (0x0400 + 23),
        TB_LINEUP = 0,
        TB_LINEDOWN = 1,
        TB_PAGEUP = 2,
        TB_PAGEDOWN = 3,
        TB_THUMBPOSITION = 4,
        TB_THUMBTRACK = 5,
        TB_TOP = 6,
        TB_BOTTOM = 7,
        TB_ENDTRACK = 8,
        TVS_HASBUTTONS = 0x0001,
        TVS_HASLINES = 0x0002,
        TVS_LINESATROOT = 0x0004,
        TVS_EDITLABELS = 0x0008,
        TVS_SHOWSELALWAYS = 0x0020,
        TVS_RTLREADING = 0x0040,
        TVS_CHECKBOXES = 0x0100,
        TVS_TRACKSELECT = 0x0200,
        TVS_FULLROWSELECT = 0x1000,
        TVS_NONEVENHEIGHT = 0x4000,
        TVS_INFOTIP = 0x0800,
        TVS_NOTOOLTIPS = 0x0080,
        TVIF_TEXT = 0x0001,
        TVIF_IMAGE = 0x0002,
        TVIF_PARAM = 0x0004,
        TVIF_STATE = 0x0008,
        TVIF_HANDLE = 0x0010,
        TVIF_SELECTEDIMAGE = 0x0020,
        TVIS_SELECTED = 0x0002,
        TVIS_EXPANDED = 0x0020,
        TVIS_EXPANDEDONCE = 0x0040,
        TVIS_STATEIMAGEMASK = 0xF000,
        TVI_ROOT = (unchecked((int)0xFFFF0000)),
        TVI_FIRST = (unchecked((int)0xFFFF0001)),
        TVM_INSERTITEM = (0x1100 + 50),
        TVM_DELETEITEM = (0x1100 + 1),
        TVM_EXPAND = (0x1100 + 2),
        TVE_COLLAPSE = 0x0001,
        TVE_EXPAND = 0x0002,
        TVM_GETITEMRECT = (0x1100 + 4),
        TVM_GETINDENT = (0x1100 + 6),
        TVM_SETINDENT = (0x1100 + 7),
        TVM_GETIMAGELIST = (0x1100 + 8),
        TVM_SETIMAGELIST = (0x1100 + 9),
        TVM_GETNEXTITEM = (0x1100 + 10),
        TVGN_NEXT = 0x0001,
        TVGN_PREVIOUS = 0x0002,
        TVGN_FIRSTVISIBLE = 0x0005,
        TVGN_NEXTVISIBLE = 0x0006,
        TVGN_PREVIOUSVISIBLE = 0x0007,
        TVGN_DROPHILITE = 0x0008,
        TVGN_CARET = 0x0009,
        TVM_SELECTITEM = (0x1100 + 11),
        TVM_GETITEM = (0x1100 + 62),
        TVM_SETITEM = (0x1100 + 63),
        TVM_EDITLABEL = (0x1100 + 65),
        TVM_GETEDITCONTROL = (0x1100 + 15),
        TVM_GETVISIBLECOUNT = (0x1100 + 16),
        TVM_HITTEST = (0x1100 + 17),
        TVM_ENSUREVISIBLE = (0x1100 + 20),
        TVM_ENDEDITLABELNOW = (0x1100 + 22),
        TVM_GETISEARCHSTRING = (0x1100 + 64),
        TVM_SETITEMHEIGHT = (0x1100 + 27),
        TVM_GETITEMHEIGHT = (0x1100 + 28),
        TVN_SELCHANGING = ((0 - 400) - 50),
        TVN_GETINFOTIP = ((0 - 400) - 14),
        TVN_SELCHANGED = ((0 - 400) - 51),
        TVC_UNKNOWN = 0x0000,
        TVC_BYMOUSE = 0x0001,
        TVC_BYKEYBOARD = 0x0002,
        TVN_GETDISPINFO = ((0 - 400) - 52),
        TVN_SETDISPINFO = ((0 - 400) - 53),
        TVN_ITEMEXPANDING = ((0 - 400) - 54),
        TVN_ITEMEXPANDED = ((0 - 400) - 55),
        TVN_BEGINDRAG = ((0 - 400) - 56),
        TVN_BEGINRDRAG = ((0 - 400) - 57),
        TVN_BEGINLABELEDIT = ((0 - 400) - 59),
        TVN_ENDLABELEDIT = ((0 - 400) - 60),
        TCS_BOTTOM = 0x0002,
        TCS_RIGHT = 0x0002,
        TCS_FLATBUTTONS = 0x0008,
        TCS_HOTTRACK = 0x0040,
        TCS_VERTICAL = 0x0080,
        TCS_TABS = 0x0000,
        TCS_BUTTONS = 0x0100,
        TCS_MULTILINE = 0x0200,
        TCS_RIGHTJUSTIFY = 0x0000,
        TCS_FIXEDWIDTH = 0x0400,
        TCS_RAGGEDRIGHT = 0x0800,
        TCS_OWNERDRAWFIXED = 0x2000,
        TCS_TOOLTIPS = 0x4000,
        TCM_SETIMAGELIST = (0x1300 + 3),
        TCIF_TEXT = 0x0001,
        TCIF_IMAGE = 0x0002,
        TCM_GETITEM = (0x1300 + 60),
        TCM_SETITEM = (0x1300 + 61),
        TCM_INSERTITEM = (0x1300 + 62),
        TCM_DELETEITEM = (0x1300 + 8),
        TCM_DELETEALLITEMS = (0x1300 + 9),
        TCM_GETITEMRECT = (0x1300 + 10),
        TCM_GETCURSEL = (0x1300 + 11),
        TCM_SETCURSEL = (0x1300 + 12),
        TCM_ADJUSTRECT = (0x1300 + 40),
        TCM_SETITEMSIZE = (0x1300 + 41),
        TCM_SETPADDING = (0x1300 + 43),
        TCM_GETROWCOUNT = (0x1300 + 44),
        TCM_GETTOOLTIPS = (0x1300 + 45),
        TCM_SETTOOLTIPS = (0x1300 + 46),
        TCN_SELCHANGE = ((0 - 550) - 1),
        TCN_SELCHANGING = ((0 - 550) - 2),
        TBSTYLE_WRAPPABLE = 0x0200,
        TVM_SETBKCOLOR = (TV_FIRST + 29),
        TVM_SETTEXTCOLOR = (TV_FIRST + 30),
        TYMED_NULL = 0,
        TVM_GETLINECOLOR = (TV_FIRST + 41),
        TVM_SETLINECOLOR = (TV_FIRST + 40),
        TVM_SETTOOLTIPS = (TV_FIRST + 24),
        TVSIL_STATE = 2,
        TVM_SORTCHILDRENCB = (TV_FIRST + 21),
        TMPF_FIXED_PITCH = 0x01;

        public const int TVHT_NOWHERE = 0x0001,
        TVHT_ONITEMICON = 0x0002,
        TVHT_ONITEMLABEL = 0x0004,
        TVHT_ONITEM = (TVHT_ONITEMICON | TVHT_ONITEMLABEL | TVHT_ONITEMSTATEICON),
        TVHT_ONITEMINDENT = 0x0008,
        TVHT_ONITEMBUTTON = 0x0010,
        TVHT_ONITEMRIGHT = 0x0020,
        TVHT_ONITEMSTATEICON = 0x0040,
        TVHT_ABOVE = 0x0100,
        TVHT_BELOW = 0x0200,
        TVHT_TORIGHT = 0x0400,
        TVHT_TOLEFT = 0x0800;

        public const int UIS_SET = 1,
        UIS_CLEAR = 2,
        UIS_INITIALIZE = 3,
        UISF_HIDEFOCUS = 0x1,
        UISF_HIDEACCEL = 0x2,
        USERCLASSTYPE_FULL = 1,
        USERCLASSTYPE_SHORT = 2,
        USERCLASSTYPE_APPNAME = 3,
        UOI_FLAGS = 1;

        public const int VIEW_E_DRAW = unchecked((int)0x80040140),
        VK_PRIOR = 0x21,
        VK_NEXT = 0x22,
        VK_LEFT = 0x25,
        VK_UP = 0x26,
        VK_RIGHT = 0x27,
        VK_DOWN = 0x28,
        VK_TAB = 0x09,
        VK_SHIFT = 0x10,
        VK_CONTROL = 0x11,
        VK_MENU = 0x12,
        VK_CAPITAL = 0x14,
        VK_KANA = 0x15,
        VK_ESCAPE = 0x1B,
        VK_END = 0x23,
        VK_HOME = 0x24,
        VK_NUMLOCK = 0x90,
        VK_SCROLL = 0x91,
        VK_INSERT = 0x002D,
        VK_DELETE = 0x002E;

        public const int WH_JOURNALPLAYBACK = 1;
        public const int WH_GETMESSAGE = 3;
        public const int WH_MOUSE = 7;
        public const int WSF_VISIBLE = 0x0001;

        public const int WA_INACTIVE = 0;
        public const int WA_ACTIVE = 1;
        public const int WA_CLICKACTIVE = 2;

        public const int WS_OVERLAPPED = 0x00000000;
        public const int WS_POPUP = unchecked((int)0x80000000);
        public const int WS_CHILD = 0x40000000;
        public const int WS_MINIMIZE = 0x20000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int WS_DISABLED = 0x08000000;
        public const int WS_CLIPSIBLINGS = 0x04000000;
        public const int WS_CLIPCHILDREN = 0x02000000;
        public const int WS_MAXIMIZE = 0x01000000;
        public const int WS_CAPTION = 0x00C00000;
        public const int WS_BORDER = 0x00800000;
        public const int WS_DLGFRAME = 0x00400000;
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_THICKFRAME = 0x00040000;
        public const int WS_TABSTOP = 0x00010000;
        public const int WS_MINIMIZEBOX = 0x00020000;
        public const int WS_MAXIMIZEBOX = 0x00010000;
        public const int WS_EX_DLGMODALFRAME = 0x00000001;
        public const int WS_EX_MDICHILD = 0x00000040;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_CLIENTEDGE = 0x00000200;
        public const int WS_EX_CONTEXTHELP = 0x00000400;
        public const int WS_EX_RIGHT = 0x00001000;
        public const int WS_EX_LEFT = 0x00000000;
        public const int WS_EX_RTLREADING = 0x00002000;
        public const int WS_EX_LEFTSCROLLBAR = 0x00004000;
        public const int WS_EX_CONTROLPARENT = 0x00010000;
        public const int WS_EX_STATICEDGE = 0x00020000;
        public const int WS_EX_APPWINDOW = 0x00040000;
        public const int WS_EX_LAYERED = 0x00080000;
        public const int WS_EX_TOPMOST = 0x00000008;
        public const int WS_EX_LAYOUTRTL = 0x00400000;
        public const int WS_EX_NOINHERITLAYOUT = 0x00100000;
        public const int WPF_SETMINPOSITION = 0x0001;

        public const int WHEEL_DELTA = 120;

        // wParam of report message WM_IME_NOTIFY (public\sdk\imm.h)
        public const int
        //IMN_CLOSESTATUSWINDOW = 0x0001,
        IMN_OPENSTATUSWINDOW = 0x0002,
        //IMN_CHANGECANDIDATE   = 0x0003,
        //IMN_CLOSECANDIDATE    = 0x0004,
        //IMN_OPENCANDIDATE     = 0x0005,
        IMN_SETCONVERSIONMODE = 0x0006,
        //IMN_SETSENTENCEMODE   = 0x0007,
        IMN_SETOPENSTATUS = 0x0008;
        //IMN_SETCANDIDATEPOS    = 0x0009,
        //IMN_SETCOMPOSITIONFONT = 0x000A,
        //IMN_SETCOMPOSITIONWINDOW = 0x000B,
        //IMN_SETSTATUSWINDOWPOS   = 0x000C,
        //IMN_GUIDELINE            = 0x000D,
        //IMN_PRIVATE              = 0x000E;

        public static int START_PAGE_GENERAL = unchecked((int)0xffffffff);

        //  Result action ids for PrintDlgEx.
        public const int PD_RESULT_CANCEL = 0;
        public const int PD_RESULT_PRINT = 1;
        public const int PD_RESULT_APPLY = 2;

        private static int wmMouseEnterMessage = -1;

        public static int WM_MOUSEENTER
        {
            get
            {
                if (wmMouseEnterMessage == -1)
                {
                    wmMouseEnterMessage = SafeNativeMethods.RegisterWindowMessage("WinFormsMouseEnter");
                }
                return wmMouseEnterMessage;
            }
        }

        private static int wmUnSubclass = -1;
        public static int WM_UIUNSUBCLASS
        {
            get
            {
                if (wmUnSubclass == -1)
                {
                    wmUnSubclass = SafeNativeMethods.RegisterWindowMessage("WinFormsUnSubclass");
                }
                return wmUnSubclass;
            }
        }

        public const int XBUTTON1 = 0x0001;
        public const int XBUTTON2 = 0x0002;

        public const string TOOLTIPS_CLASS = "tooltips_class32";

        public const string WC_DATETIMEPICK = "SysDateTimePick32",
        WC_LISTVIEW = "SysListView32",
        WC_MONTHCAL = "SysMonthCal32",
        WC_PROGRESS = "msctls_progress32",
        WC_STATUSBAR = "msctls_statusbar32",
        WC_TOOLBAR = "ToolbarWindow32",
        WC_TRACKBAR = "msctls_trackbar32",
        WC_TREEVIEW = "SysTreeView32",
        WC_TABCONTROL = "SysTabControl32",
        MSH_MOUSEWHEEL = "MSWHEEL_ROLLMSG",
        MSH_SCROLL_LINES = "MSH_SCROLL_LINES_MSG";

        public const int CHILDID_SELF = 0;

        public const int OBJID_QUERYCLASSNAMEIDX = unchecked(unchecked((int)0xFFFFFFF4));
        public const int OBJID_CLIENT = unchecked(unchecked((int)0xFFFFFFFC));
        public const int OBJID_WINDOW = unchecked(unchecked((int)0x00000000));

        public const int UiaRootObjectId = -25;
        public const int UiaAppendRuntimeId = 3;

        public const string uuid_IAccessible = "{618736E0-3C3D-11CF-810C-00AA00389B71}";
        public const string uuid_IEnumVariant = "{00020404-0000-0000-C000-000000000046}";

        public const string WinFormFrameworkId = "WinForm";

        /*
        * MISCELLANEOUS
        */

        [StructLayout(LayoutKind.Sequential)]
        public class OLECMD
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cmdID = 0;
            [MarshalAs(UnmanagedType.U4)]
            public int cmdf = 0;

        }

        [ComVisible(true)]
        [ComImport]
        [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleCommandTarget
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryStatus(
                ref Guid pguidCmdGroup,
                int cCmds,
                [In, Out]
                OLECMD prgCmds,
                [In, Out]
                IntPtr pCmdText);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int Exec(
                ref Guid pguidCmdGroup,
                int nCmdID,
                int nCmdexecopt,
                // we need to have this an array because callers need to be able to specify NULL or VT_NULL
                [In, MarshalAs(UnmanagedType.LPArray)]
                object[] pvaIn,
                int pvaOut);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class FONTDESC
        {
            public int cbSizeOfStruct = Marshal.SizeOf<FONTDESC>();
            public string lpstrName;
            public long cySize;
            public short sWeight;
            public short sCharset;
            public bool fItalic;
            public bool fUnderline;
            public bool fStrikethrough;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PICTDESCbmp
        {
            internal int cbSizeOfStruct = Marshal.SizeOf<PICTDESCbmp>();
            internal int picType = Ole.PICTYPE_BITMAP;
            internal IntPtr hbitmap = IntPtr.Zero;
            internal IntPtr hpalette = IntPtr.Zero;
            internal int unused = 0;

            public PICTDESCbmp(Drawing.Bitmap bitmap)
            {
                hbitmap = bitmap.GetHbitmap();
                // gpr: What about palettes?
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PICTDESCicon
        {
            internal int cbSizeOfStruct = Marshal.SizeOf<PICTDESCicon>();
            internal int picType = Ole.PICTYPE_ICON;
            internal IntPtr hicon = IntPtr.Zero;
            internal int unused1 = 0;
            internal int unused2 = 0;

            public PICTDESCicon(Drawing.Icon icon)
            {
                hicon = SafeNativeMethods.CopyImage(new HandleRef(icon, icon.Handle), NativeMethods.IMAGE_ICON, icon.Size.Width, icon.Size.Height, 0);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PICTDESCemf
        {
            internal int cbSizeOfStruct = Marshal.SizeOf<PICTDESCemf>();
            internal int picType = Ole.PICTYPE_ENHMETAFILE;
            internal IntPtr hemf = IntPtr.Zero;
            internal int unused1 = 0;
            internal int unused2 = 0;

            public PICTDESCemf(Drawing.Imaging.Metafile metafile)
            {
                //gpr                hemf = metafile.CopyHandle();
            }
        }

        public struct USEROBJECTFLAGS
        {
            public int fInherit;
            public int fReserved;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class SYSTEMTIMEARRAY
        {
            public short wYear1;
            public short wMonth1;
            public short wDayOfWeek1;
            public short wDay1;
            public short wHour1;
            public short wMinute1;
            public short wSecond1;
            public short wMilliseconds1;
            public short wYear2;
            public short wMonth2;
            public short wDayOfWeek2;
            public short wDay2;
            public short wHour2;
            public short wMinute2;
            public short wSecond2;
            public short wMilliseconds2;
        }

        public delegate bool EnumChildrenCallback(IntPtr hwnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_AKLINK
        {
            internal int cbStruct = Marshal.SizeOf<HH_AKLINK>();
            internal bool fReserved = false;
            internal string pszKeywords = null;
            internal string pszUrl = null;
            internal string pszMsgText = null;
            internal string pszMsgTitle = null;
            internal string pszWindow = null;
            internal bool fIndexOnFail = false;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_POPUP
        {
            internal int cbStruct = Marshal.SizeOf<HH_POPUP>();
            internal IntPtr hinst = IntPtr.Zero;
            internal int idString = 0;
            internal IntPtr pszText;
            internal Point pt;
            internal int clrForeground = -1;
            internal int clrBackground = -1;
            internal Interop.RECT rcMargins = new Interop.RECT(-1, -1, -1, -1);     // amount of space between edges of window and text, -1 for each member to ignore
            internal string pszFont = null;
        }

        public const int HH_FTS_DEFAULT_PROXIMITY = -1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_FTS_QUERY
        {
            internal int cbStruct = Marshal.SizeOf<HH_FTS_QUERY>();
            internal bool fUniCodeStrings = false;
            [MarshalAs(UnmanagedType.LPStr)]
            internal string pszSearchQuery = null;
            internal int iProximity = NativeMethods.HH_FTS_DEFAULT_PROXIMITY;
            internal bool fStemmedSearch = false;
            internal bool fTitleOnly = false;
            internal bool fExecute = true;
            [MarshalAs(UnmanagedType.LPStr)]
            internal string pszWindow = null;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFOEX
        {
            internal int cbSize = Marshal.SizeOf<MONITORINFOEX>();
            internal Interop.RECT rcMonitor = new Interop.RECT();
            internal Interop.RECT rcWork = new Interop.RECT();
            internal int dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal char[] szDevice = new char[32];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        public class MONITORINFO
        {
            internal int cbSize = Marshal.SizeOf<MONITORINFO>();
            internal Interop.RECT rcMonitor = new Interop.RECT();
            internal Interop.RECT rcWork = new Interop.RECT();
            internal int dwFlags = 0;
        }

        public delegate int EditStreamCallback(IntPtr dwCookie, IntPtr buf, int cb, out int transferred);

        [StructLayout(LayoutKind.Sequential)]
        public class EDITSTREAM
        {
            public IntPtr dwCookie = IntPtr.Zero;
            public int dwError = 0;
            public EditStreamCallback pfnCallback = null;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class EDITSTREAM64
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] contents = new byte[20];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [ComImport]
        [Guid("0FF510A3-5FA5-49F1-8CCC-190D71083F3E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IVsPerPropertyBrowsing
        {
            /// <summary>
            ///  Hides the property at the given dispid from the properties window
            ///  implmentors should can return E_NOTIMPL to show all properties that
            ///  are otherwise browsable.
            /// </summary>
            [PreserveSig]
            int HideProperty(int dispid, ref bool pfHide);

            /// <summary>
            ///  Will have the "+" expandable glyph next to them and can be expanded or collapsed by the user
            ///  Returning a non-S_OK return code or false for pfDisplay will suppress this feature
            /// </summary>
            [PreserveSig]
            int DisplayChildProperties(int dispid,
                                       ref bool pfDisplay);

            /// <summary>
            ///  Retrieves the localized name and description for a property.
            ///  returning a non-S_OK return code will display the default values
            /// </summary>
            [PreserveSig]
            int GetLocalizedPropertyInfo(int dispid, int localeID,
                                         [Out, MarshalAs(UnmanagedType.LPArray)]
                                         string[] pbstrLocalizedName,
                                         [Out, MarshalAs(UnmanagedType.LPArray)]
                                         string[] pbstrLocalizeDescription);

            /// <summary>
            ///  Determines if the given (usually current) value for a property is the default.  If it is not default,
            ///  the property will be shown as bold in the browser to indcate that it has been modified from the default.
            /// </summary>
            [PreserveSig]
            int HasDefaultValue(int dispid,
                               ref bool fDefault);

            /// <summary>
            ///  Determines if a property should be made read only.  This only applies to properties that are writeable,
            /// </summary>
            [PreserveSig]
            int IsPropertyReadOnly(int dispid,
                                   ref bool fReadOnly);

            /// <summary>
            ///  Returns the classname for this object. The class name is the non-bolded text
            ///  that appears in the properties window selection combo.  If this method returns
            ///  a non-S_OK return code, the default will be used. The default is the name
            ///  string from a call to ITypeInfo::GetDocumentation(MEMID_NIL, ...);
            [PreserveSig]
            int GetClassName([In, Out]ref string pbstrClassName);

            /// <summary>
            ///  Checks whether the given property can be reset to some default value.
            ///  If return value is non-S_OK or *pfCanReset is
            /// </summary>
            [PreserveSig]
            int CanResetPropertyValue(int dispid, [In, Out]ref bool pfCanReset);

            /// <summary>
            ///  If the return value is S_OK, the property's value will then be refreshed to the
            ///  new default values.
            /// </summary>
            [PreserveSig]
            int ResetPropertyValue(int dispid);
        }

        [ComImport]
        [Guid("7494683C-37A0-11d2-A273-00C04F8EF4FF")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IManagedPerPropertyBrowsing
        {
            [PreserveSig]
            int GetPropertyAttributes(int dispid,
                                      ref int pcAttributes,
                                      ref IntPtr pbstrAttrNames,
                                      ref IntPtr pvariantInitValues);
        }

        [ComImport]
        [Guid("33C0C1D8-33CF-11d3-BFF2-00C04F990235")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IProvidePropertyBuilder
        {
            [PreserveSig]
            int MapPropertyToBuilder(
                int dispid,
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                int[] pdwCtlBldType,
                [In, Out, MarshalAs(UnmanagedType.LPArray)]
                string[] pbstrGuidBldr,
                [In, Out, MarshalAs(UnmanagedType.Bool)]
                ref bool builderAvailable);

            [PreserveSig]
            int ExecuteBuilder(
                int dispid,
                [In, MarshalAs(UnmanagedType.BStr)]
                string bstrGuidBldr,
                [In, MarshalAs(UnmanagedType.Interface)]
                object pdispApp,

                HandleRef hwndBldrOwner,
                [Out, In, MarshalAs(UnmanagedType.Struct)]
                ref object pvarValue,
                [In, Out, MarshalAs(UnmanagedType.Bool)]
                ref bool actionCommitted);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class INITCOMMONCONTROLSEX
        {
            public int dwSize = 8; //ndirect.DllLib.sizeOf(this);
            public int dwICC;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class IMAGELISTDRAWPARAMS
        {
            public int cbSize = Marshal.SizeOf<IMAGELISTDRAWPARAMS>();
            public IntPtr himl = IntPtr.Zero;
            public int i = 0;
            public IntPtr hdcDst = IntPtr.Zero;
            public int x = 0;
            public int y = 0;
            public int cx = 0;
            public int cy = 0;
            public int xBitmap = 0;
            public int yBitmap = 0;
            public int rgbBk = 0;
            public int rgbFg = 0;
            public int fStyle = 0;
            public int dwRop = 0;
            public int fState = 0;
            public int Frame = 0;
            public int crEffect = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class IMAGEINFO
        {
            public IntPtr hbmImage = IntPtr.Zero;
            public IntPtr hbmMask = IntPtr.Zero;
            public int Unused1 = 0;
            public int Unused2 = 0;
            // rcImage was a by-value RECT structure
            public int rcImage_left = 0;
            public int rcImage_top = 0;
            public int rcImage_right = 0;
            public int rcImage_bottom = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TRACKMOUSEEVENT
        {
            public int cbSize = Marshal.SizeOf<TRACKMOUSEEVENT>();
            public int dwFlags;
            public IntPtr hwndTrack;
            public int dwHoverTime = 100; // Never set this to field ZERO, or to HOVER_DEFAULT, ever!
        }

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        public delegate int ListViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);

        public delegate int TreeViewCompareCallback(IntPtr lParam1, IntPtr lParam2, IntPtr lParamSort);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NONCLIENTMETRICSW
        {
            public uint cbSize;
            public int iBorderWidth;
            public int iScrollWidth;
            public int iScrollHeight;
            public int iCaptionWidth;
            public int iCaptionHeight;
            public LOGFONTW lfCaptionFont;
            public int iSmCaptionWidth;
            public int iSmCaptionHeight;
            public LOGFONTW lfSmCaptionFont;
            public int iMenuWidth;
            public int iMenuHeight;
            public LOGFONTW lfMenuFont;
            public LOGFONTW lfStatusFont;
            public LOGFONTW lfMessageFont;

            // This is supported on Windows vista and later
            public int iPaddedBorderWidth;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public int time;
            public Point pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            // rcPaint was a by-value RECT structure
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public bool fRestore;
            public bool fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SCROLLINFO
        {
            public int cbSize = Marshal.SizeOf<SCROLLINFO>();
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;

            public SCROLLINFO()
            {
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TPMPARAMS
        {
            public int cbSize = Marshal.SizeOf<TPMPARAMS>();
            // rcExclude was a by-value RECT structure
            public int rcExclude_left;
            public int rcExclude_top;
            public int rcExclude_right;
            public int rcExclude_bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            // rcNormalPosition was a by-value RECT structure
            public int rcNormalPosition_left;
            public int rcNormalPosition_top;
            public int rcNormalPosition_right;
            public int rcNormalPosition_bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class STARTUPINFO_I
        {
            public int cb = 0;
            public IntPtr lpReserved = IntPtr.Zero;
            public IntPtr lpDesktop = IntPtr.Zero;
            public IntPtr lpTitle = IntPtr.Zero;
            public int dwX = 0;
            public int dwY = 0;
            public int dwXSize = 0;
            public int dwYSize = 0;
            public int dwXCountChars = 0;
            public int dwYCountChars = 0;
            public int dwFillAttribute = 0;
            public int dwFlags = 0;
            public short wShowWindow = 0;
            public short cbReserved2 = 0;
            public IntPtr lpReserved2 = IntPtr.Zero;
            public IntPtr hStdInput = IntPtr.Zero;
            public IntPtr hStdOutput = IntPtr.Zero;
            public IntPtr hStdError = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PAGESETUPDLG
        {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hDevMode;
            public IntPtr hDevNames;
            public int Flags;
            public Point paperSize;

            // RECT            rtMinMargin;
            public int minMarginLeft;
            public int minMarginTop;
            public int minMarginRight;
            public int minMarginBottom;

            // RECT            rtMargin;
            public int marginLeft;
            public int marginTop;
            public int marginRight;
            public int marginBottom;

            public IntPtr hInstance = IntPtr.Zero;
            public IntPtr lCustData = IntPtr.Zero;
            public WndProc lpfnPageSetupHook = null;
            public WndProc lpfnPagePaintHook = null;
            public string lpPageSetupTemplateName = null;
            public IntPtr hPageSetupTemplate = IntPtr.Zero;
        }

        // Any change in PRINTDLG, should also be in PRINTDLG_32 and PRINTDLG_64
        public interface PRINTDLG
        {
            int lStructSize { get; set; }

            IntPtr hwndOwner { get; set; }
            IntPtr hDevMode { get; set; }
            IntPtr hDevNames { get; set; }
            IntPtr hDC { get; set; }

            int Flags { get; set; }

            short nFromPage { get; set; }
            short nToPage { get; set; }
            short nMinPage { get; set; }
            short nMaxPage { get; set; }
            short nCopies { get; set; }

            IntPtr hInstance { get; set; }
            IntPtr lCustData { get; set; }

            WndProc lpfnPrintHook { get; set; }
            WndProc lpfnSetupHook { get; set; }

            string lpPrintTemplateName { get; set; }
            string lpSetupTemplateName { get; set; }

            IntPtr hPrintTemplate { get; set; }
            IntPtr hSetupTemplate { get; set; }
        }

        // Any change in PRINTDLG_32, should also be in PRINTDLG and PRINTDLG_64
        // x86 requires EXPLICIT packing of 1.
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
        public class PRINTDLG_32 : PRINTDLG
        {
            int m_lStructSize;

            IntPtr m_hwndOwner;
            IntPtr m_hDevMode;
            IntPtr m_hDevNames;
            IntPtr m_hDC;

            int m_Flags;

            short m_nFromPage;
            short m_nToPage;
            short m_nMinPage;
            short m_nMaxPage;
            short m_nCopies;

            IntPtr m_hInstance;
            IntPtr m_lCustData;

            WndProc m_lpfnPrintHook;
            WndProc m_lpfnSetupHook;

            string m_lpPrintTemplateName;
            string m_lpSetupTemplateName;

            IntPtr m_hPrintTemplate;
            IntPtr m_hSetupTemplate;

            public int lStructSize { get { return m_lStructSize; } set { m_lStructSize = value; } }

            public IntPtr hwndOwner { get { return m_hwndOwner; } set { m_hwndOwner = value; } }
            public IntPtr hDevMode { get { return m_hDevMode; } set { m_hDevMode = value; } }
            public IntPtr hDevNames { get { return m_hDevNames; } set { m_hDevNames = value; } }
            public IntPtr hDC { get { return m_hDC; } set { m_hDC = value; } }

            public int Flags { get { return m_Flags; } set { m_Flags = value; } }

            public short nFromPage { get { return m_nFromPage; } set { m_nFromPage = value; } }
            public short nToPage { get { return m_nToPage; } set { m_nToPage = value; } }
            public short nMinPage { get { return m_nMinPage; } set { m_nMinPage = value; } }
            public short nMaxPage { get { return m_nMaxPage; } set { m_nMaxPage = value; } }
            public short nCopies { get { return m_nCopies; } set { m_nCopies = value; } }

            public IntPtr hInstance { get { return m_hInstance; } set { m_hInstance = value; } }
            public IntPtr lCustData { get { return m_lCustData; } set { m_lCustData = value; } }

            public WndProc lpfnPrintHook { get { return m_lpfnPrintHook; } set { m_lpfnPrintHook = value; } }
            public WndProc lpfnSetupHook { get { return m_lpfnSetupHook; } set { m_lpfnSetupHook = value; } }

            public string lpPrintTemplateName { get { return m_lpPrintTemplateName; } set { m_lpPrintTemplateName = value; } }
            public string lpSetupTemplateName { get { return m_lpSetupTemplateName; } set { m_lpSetupTemplateName = value; } }

            public IntPtr hPrintTemplate { get { return m_hPrintTemplate; } set { m_hPrintTemplate = value; } }
            public IntPtr hSetupTemplate { get { return m_hSetupTemplate; } set { m_hSetupTemplate = value; } }
        }

        // Any change in PRINTDLG_64, should also be in PRINTDLG_32 and PRINTDLG
        // x64 does not require EXPLICIT packing of 1.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class PRINTDLG_64 : PRINTDLG
        {
            int m_lStructSize;

            IntPtr m_hwndOwner;
            IntPtr m_hDevMode;
            IntPtr m_hDevNames;
            IntPtr m_hDC;

            int m_Flags;

            short m_nFromPage;
            short m_nToPage;
            short m_nMinPage;
            short m_nMaxPage;
            short m_nCopies;

            IntPtr m_hInstance;
            IntPtr m_lCustData;

            WndProc m_lpfnPrintHook;
            WndProc m_lpfnSetupHook;

            string m_lpPrintTemplateName;
            string m_lpSetupTemplateName;

            IntPtr m_hPrintTemplate;
            IntPtr m_hSetupTemplate;

            public int lStructSize { get { return m_lStructSize; } set { m_lStructSize = value; } }

            public IntPtr hwndOwner { get { return m_hwndOwner; } set { m_hwndOwner = value; } }
            public IntPtr hDevMode { get { return m_hDevMode; } set { m_hDevMode = value; } }
            public IntPtr hDevNames { get { return m_hDevNames; } set { m_hDevNames = value; } }
            public IntPtr hDC { get { return m_hDC; } set { m_hDC = value; } }

            public int Flags { get { return m_Flags; } set { m_Flags = value; } }

            public short nFromPage { get { return m_nFromPage; } set { m_nFromPage = value; } }
            public short nToPage { get { return m_nToPage; } set { m_nToPage = value; } }
            public short nMinPage { get { return m_nMinPage; } set { m_nMinPage = value; } }
            public short nMaxPage { get { return m_nMaxPage; } set { m_nMaxPage = value; } }
            public short nCopies { get { return m_nCopies; } set { m_nCopies = value; } }

            public IntPtr hInstance { get { return m_hInstance; } set { m_hInstance = value; } }
            public IntPtr lCustData { get { return m_lCustData; } set { m_lCustData = value; } }

            public WndProc lpfnPrintHook { get { return m_lpfnPrintHook; } set { m_lpfnPrintHook = value; } }
            public WndProc lpfnSetupHook { get { return m_lpfnSetupHook; } set { m_lpfnSetupHook = value; } }

            public string lpPrintTemplateName { get { return m_lpPrintTemplateName; } set { m_lpPrintTemplateName = value; } }
            public string lpSetupTemplateName { get { return m_lpSetupTemplateName; } set { m_lpSetupTemplateName = value; } }

            public IntPtr hPrintTemplate { get { return m_hPrintTemplate; } set { m_hPrintTemplate = value; } }
            public IntPtr hSetupTemplate { get { return m_hSetupTemplate; } set { m_hSetupTemplate = value; } }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class PRINTDLGEX
        {
            public int lStructSize;

            public IntPtr hwndOwner;
            public IntPtr hDevMode;
            public IntPtr hDevNames;
            public IntPtr hDC;

            public int Flags;
            public int Flags2;

            public int ExclusionFlags;

            public int nPageRanges;
            public int nMaxPageRanges;

            public IntPtr pageRanges;

            public int nMinPage;
            public int nMaxPage;
            public int nCopies;

            public IntPtr hInstance;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpPrintTemplateName;

            public WndProc lpCallback = null;

            public int nPropertyPages;

            public IntPtr lphPropertyPages;

            public int nStartPage;
            public int dwResultAction;

        }

        // x86 requires EXPLICIT packing of 1.
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
        public class PRINTPAGERANGE
        {
            public int nFromPage = 0;
            public int nToPage = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PICTDESC
        {
            internal int cbSizeOfStruct;
            public int picType;
            internal IntPtr union1;
            internal int union2;
            internal int union3;

            public static PICTDESC CreateBitmapPICTDESC(IntPtr hbitmap, IntPtr hpal)
            {
                PICTDESC pictdesc = new PICTDESC
                {
                    cbSizeOfStruct = 16,
                    picType = Ole.PICTYPE_BITMAP,
                    union1 = hbitmap,
                    union2 = (int)(((long)hpal) & 0xffffffff),
                    union3 = (int)(((long)hpal) >> 32)
                };
                return pictdesc;
            }

            public static PICTDESC CreateIconPICTDESC(IntPtr hicon)
            {
                PICTDESC pictdesc = new PICTDESC
                {
                    cbSizeOfStruct = 12,
                    picType = Ole.PICTYPE_ICON,
                    union1 = hicon
                };
                return pictdesc;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagFONTDESC
        {
            public int cbSizeofstruct = Marshal.SizeOf<tagFONTDESC>();

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpstrName;

            [MarshalAs(UnmanagedType.U8)]
            public long cySize;

            [MarshalAs(UnmanagedType.U2)]
            public short sWeight;

            [MarshalAs(UnmanagedType.U2)]
            public short sCharset;

            [MarshalAs(UnmanagedType.Bool)]
            public bool fItalic;

            [MarshalAs(UnmanagedType.Bool)]
            public bool fUnderline;

            [MarshalAs(UnmanagedType.Bool)]
            public bool fStrikethrough;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class CHOOSECOLOR
        {
            public int lStructSize = Marshal.SizeOf<CHOOSECOLOR>(); //ndirect.DllLib.sizeOf(this);
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public int rgbResult;
            public IntPtr lpCustColors;
            public int Flags;
            public IntPtr lCustData = IntPtr.Zero;
            public WndProc lpfnHook;
            public string lpTemplateName = null;
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAP
        {
            public int bmType = 0;
            public int bmWidth = 0;
            public int bmHeight = 0;
            public int bmWidthBytes = 0;
            public short bmPlanes = 0;
            public short bmBitsPixel = 0;
            public IntPtr bmBits = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public int fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LOGBRUSH
        {
            public int lbStyle;
            public int lbColor;
            public IntPtr lbHatch;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct LOGFONTW
        {
            private const int LF_FACESIZE = 32;

            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            private fixed char _lfFaceName[LF_FACESIZE];
            private Span<char> lfFaceName
            {
                get { fixed (char* c = _lfFaceName) { return new Span<char>(c, LF_FACESIZE); } }
            }

            public ReadOnlySpan<char> FaceName
            {
                get => lfFaceName.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, lfFaceName);
            }

            // Font.ToLogFont will copy LOGFONT into a blittable struct,
            // but we need to box it upfront so we can unbox.

            public static LOGFONTW FromFont(Font font)
            {
                object logFont = new LOGFONTW();
                font.ToLogFont(logFont);
                return (LOGFONTW)logFont;
            }

            public static LOGFONTW FromFont(Font font, Graphics graphics)
            {
                object logFont = new LOGFONTW();
                font.ToLogFont(logFont, graphics);
                return (LOGFONTW)logFont;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TEXTMETRIC
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NOTIFYICONDATA
        {
            public int cbSize = Marshal.SizeOf<NOTIFYICONDATA>();
            public IntPtr hWnd;
            public int uID;
            public int uFlags;
            public int uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;
            public int dwState = 0;
            public int dwStateMask = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;
            public int uTimeoutOrVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;
            public int dwInfoFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MENUITEMINFO_T
        {
            public int cbSize = Marshal.SizeOf<MENUITEMINFO_T>();
            public int fMask;
            public int fType;
            public int fState;
            public int wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public string dwTypeData;
            public int cch;
        }

        // This version allows you to read the string that's stuffed
        // in the native menu item.  You have to do the marshaling on
        // your own though.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MENUITEMINFO_T_RW
        {
            public int cbSize = Marshal.SizeOf<MENUITEMINFO_T_RW>();
            public int fMask = 0;
            public int fType = 0;
            public int fState = 0;
            public int wID = 0;
            public IntPtr hSubMenu = IntPtr.Zero;
            public IntPtr hbmpChecked = IntPtr.Zero;
            public IntPtr hbmpUnchecked = IntPtr.Zero;
            public IntPtr dwItemData = IntPtr.Zero;
            public IntPtr dwTypeData = IntPtr.Zero;
            public int cch = 0;
            public IntPtr hbmpItem = IntPtr.Zero;  // requires WINVER > 5
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MSAAMENUINFO
        {
            public int dwMSAASignature;
            public int cchWText;
            public string pszWText;

            public MSAAMENUINFO(string text)
            {
                dwMSAASignature = unchecked((int)MSAA_MENU_SIG);
                cchWText = text.Length;
                pszWText = text;
            }
        }

        public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OPENFILENAME_I
        {
            public int lStructSize = Marshal.SizeOf<OPENFILENAME_I>(); //ndirect.DllLib.sizeOf(this);
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string lpstrFilter;   // use embedded nulls to separate filters
            public IntPtr lpstrCustomFilter = IntPtr.Zero;
            public int nMaxCustFilter = 0;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile = Interop.Kernel32.MAX_PATH;
            public IntPtr lpstrFileTitle = IntPtr.Zero;
            public int nMaxFileTitle = Interop.Kernel32.MAX_PATH;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset = 0;
            public short nFileExtension = 0;
            public string lpstrDefExt;
            public IntPtr lCustData = IntPtr.Zero;
            public WndProc lpfnHook;
            public string lpTemplateName = null;
            public IntPtr pvReserved = IntPtr.Zero;
            public int dwReserved = 0;
            public int FlagsEx;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class CHOOSEFONT
        {
            public int lStructSize = Marshal.SizeOf<CHOOSEFONT>();
            public IntPtr hwndOwner;
            public IntPtr hDC;
            public IntPtr lpLogFont;
            public int iPointSize = 0;
            public int Flags;
            public int rgbColors;
            public IntPtr lCustData = IntPtr.Zero;
            public WndProc lpfnHook;
            public string lpTemplateName = null;
            public IntPtr hInstance;
            public string lpszStyle = null;
            public short nFontType = 0;
            public short ___MISSING_ALIGNMENT__ = 0;
            public int nSizeMin;
            public int nSizeMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAPINFOHEADER
        {
            public int biSize = 40;    // ndirect.DllLib.sizeOf( this );
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage = 0;
            public int biXPelsPerMeter = 0;
            public int biYPelsPerMeter = 0;
            public int biClrUsed = 0;
            public int biClrImportant = 0;
        }

        public class Ole
        {
            public const int PICTYPE_UNINITIALIZED = -1;
            public const int PICTYPE_NONE = 0;
            public const int PICTYPE_BITMAP = 1;
            public const int PICTYPE_METAFILE = 2;
            public const int PICTYPE_ICON = 3;
            public const int PICTYPE_ENHMETAFILE = 4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;

            public override string ToString()
            {
                return "[SYSTEMTIME: "
                + wDay.ToString(CultureInfo.InvariantCulture) + "/" + wMonth.ToString(CultureInfo.InvariantCulture) + "/" + wYear.ToString(CultureInfo.InvariantCulture)
                + " " + wHour.ToString(CultureInfo.InvariantCulture) + ":" + wMinute.ToString(CultureInfo.InvariantCulture) + ":" + wSecond.ToString(CultureInfo.InvariantCulture)
                + "]";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class COMRECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public COMRECT()
            {
            }

            public COMRECT(Rectangle r)
            {
                left = r.X;
                top = r.Y;
                right = r.Right;
                bottom = r.Bottom;
            }

            public COMRECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static COMRECT FromXYWH(int x, int y, int width, int height)
            {
                return new COMRECT(x, y, x + width, y + height);
            }

            public override string ToString()
            {
                return "Left = " + left + " Top " + top + " Right = " + right + " Bottom = " + bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagOleMenuGroupWidths
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)/*leftover(offset=0, widths)*/]
            public int[] widths = new int[6];
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MSOCRINFOSTRUCT
        {
            public int cbSize = Marshal.SizeOf<MSOCRINFOSTRUCT>();              // size of MSOCRINFO structure in bytes.
            public int uIdleTimeInterval;   // If olecrfNeedPeriodicIdleTime is registered
                                            // in grfcrf, component needs to perform
                                            // periodic idle time tasks during an idle phase
                                            // every uIdleTimeInterval milliseconds.
            public int grfcrf;              // bit flags taken from olecrf values (above)
            public int grfcadvf;            // bit flags taken from olecadvf values (above)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLISTVIEW
        {
            public NMHDR hdr;
            public int iItem;
            public int iSubItem;
            public int uNewState;
            public int uOldState;
            public int uChanged;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagOIFI
        {
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=0, cb)*/]
            public int cb;

            public bool fMDIApp;
            public IntPtr hwndFrame;
            public IntPtr hAccel;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=16, cAccelEntries)*/]
            public int cAccelEntries;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVFINDITEM
        {
            public NMHDR hdr;
            public int iStart;
            public LVFINDINFO lvfi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom; //This is declared as UINT_PTR in winuser.h
            public int code;
        }

        [ComImport]
        [Guid("376BD3AA-3845-101B-84ED-08002B2EC713")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPerPropertyBrowsing
        {
            [PreserveSig]
            int GetDisplayString(
                int dispID,
                [Out, MarshalAs(UnmanagedType.LPArray)]
                string[] pBstr);

            [PreserveSig]
            int MapPropertyToPage(
                int dispID,
                [Out]
                out Guid pGuid);

            [PreserveSig]
            int GetPredefinedStrings(
                int dispID,
                [Out]
                CA_STRUCT pCaStringsOut,
                [Out]
                CA_STRUCT pCaCookiesOut);

            [PreserveSig]
            int GetPredefinedValue(
                int dispID,
                [In, MarshalAs(UnmanagedType.U4)]
                int dwCookie,
                [Out]
                VARIANT pVarOut);
        }

        [ComImport]
        [Guid("4D07FC10-F931-11CE-B001-00AA006884E5")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ICategorizeProperties
        {
            [PreserveSig]
            int MapPropertyToCategory(
                int dispID,
                ref int categoryID);

            [PreserveSig]
            int GetCategoryName(
                int propcat,
                [In, MarshalAs(UnmanagedType.U4)]
                int lcid,
                out string categoryName);
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagOLEVERB
        {
            public int lVerb;

            [MarshalAs(UnmanagedType.LPWStr)/*leftover(offset=4, customMarshal="UniStringMarshaller", lpszVerbName)*/]
            public string lpszVerbName;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=8, fuFlags)*/]
            public int fuFlags;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=12, grfAttribs)*/]
            public int grfAttribs;
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagLOGPALETTE
        {
            [MarshalAs(UnmanagedType.U2)/*leftover(offset=0, palVersion)*/]
            public short palVersion = 0;

            [MarshalAs(UnmanagedType.U2)/*leftover(offset=2, palNumEntries)*/]
            public short palNumEntries = 0;
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagCONTROLINFO
        {
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=0, cb)*/]
            public int cb = Marshal.SizeOf<tagCONTROLINFO>();

            public IntPtr hAccel;

            [MarshalAs(UnmanagedType.U2)/*leftover(offset=8, cAccel)*/]
            public short cAccel;

            [MarshalAs(UnmanagedType.U4)/*leftover(offset=10, dwFlags)*/]
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class CA_STRUCT
        {
            public int cElems = 0;
            public IntPtr pElems = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class VARIANT
        {
            [MarshalAs(UnmanagedType.I2)]
            public short vt;
            [MarshalAs(UnmanagedType.I2)]
            public short reserved1 = 0;
            [MarshalAs(UnmanagedType.I2)]
            public short reserved2 = 0;
            [MarshalAs(UnmanagedType.I2)]
            public short reserved3 = 0;

            public IntPtr data1;

            public IntPtr data2;

            public bool Byref
            {
                get
                {
                    return 0 != (vt & (int)tagVT.VT_BYREF);
                }
            }

            public void Clear()
            {
                if ((vt == (int)tagVT.VT_UNKNOWN || vt == (int)tagVT.VT_DISPATCH) && data1 != IntPtr.Zero)
                {
                    Marshal.Release(data1);
                }

                if (vt == (int)tagVT.VT_BSTR && data1 != IntPtr.Zero)
                {
                    SysFreeString(data1);
                }

                data1 = data2 = IntPtr.Zero;
                vt = (int)tagVT.VT_EMPTY;
            }

            ~VARIANT()
            {
                Clear();
            }

            [DllImport(ExternDll.Oleaut32, CharSet = CharSet.Auto)]
            private static extern void SysFreeString(IntPtr pbstr);

            public object ToObject()
            {
                IntPtr val = data1;
                long longVal;

                int vtType = (int)(vt & (short)tagVT.VT_TYPEMASK);

                switch (vtType)
                {
                    case (int)tagVT.VT_EMPTY:
                        return null;
                    case (int)tagVT.VT_NULL:
                        return Convert.DBNull;

                    case (int)tagVT.VT_I1:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadByte(val);
                        }
                        return (sbyte)(0xFF & (sbyte)val);

                    case (int)tagVT.VT_UI1:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadByte(val);
                        }

                        return (byte)(0xFF & (byte)val);

                    case (int)tagVT.VT_I2:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt16(val);
                        }
                        return (short)(0xFFFF & (short)val);

                    case (int)tagVT.VT_UI2:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt16(val);
                        }
                        return (ushort)(0xFFFF & (ushort)val);

                    case (int)tagVT.VT_I4:
                    case (int)tagVT.VT_INT:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt32(val);
                        }
                        return (int)val;

                    case (int)tagVT.VT_UI4:
                    case (int)tagVT.VT_UINT:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt32(val);
                        }
                        return (uint)val;

                    case (int)tagVT.VT_I8:
                    case (int)tagVT.VT_UI8:
                        if (Byref)
                        {
                            longVal = Marshal.ReadInt64(val);
                        }
                        else
                        {
                            longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        }

                        if (vt == (int)tagVT.VT_I8)
                        {
                            return (long)longVal;
                        }
                        else
                        {
                            return (ulong)longVal;
                        }
                }

                if (Byref)
                {
                    val = GetRefInt(val);
                }

                switch (vtType)
                {
                    case (int)tagVT.VT_R4:
                    case (int)tagVT.VT_R8:

                        // can I use unsafe here?
                        throw new FormatException(SR.CannotConvertIntToFloat);

                    case (int)tagVT.VT_CY:
                        // internally currency is 8-byte int scaled by 10,000
                        longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        return new decimal(longVal);
                    case (int)tagVT.VT_DATE:
                        throw new FormatException(SR.CannotConvertDoubleToDate);

                    case (int)tagVT.VT_BSTR:
                    case (int)tagVT.VT_LPWSTR:
                        return Marshal.PtrToStringUni(val);

                    case (int)tagVT.VT_LPSTR:
                        return Marshal.PtrToStringAnsi(val);

                    case (int)tagVT.VT_DISPATCH:
                    case (int)tagVT.VT_UNKNOWN:
                        {
                            return Marshal.GetObjectForIUnknown(val);
                        }

                    case (int)tagVT.VT_HRESULT:
                        return val;

                    case (int)tagVT.VT_DECIMAL:
                        longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        return new decimal(longVal);

                    case (int)tagVT.VT_BOOL:
                        return (val != IntPtr.Zero);

                    case (int)tagVT.VT_VARIANT:
                        VARIANT varStruct = Marshal.PtrToStructure<VARIANT>(val);
                        return varStruct.ToObject();

                    case (int)tagVT.VT_CLSID:
                        Guid guid = Marshal.PtrToStructure<Guid>(val);
                        return guid;

                    case (int)tagVT.VT_FILETIME:
                        longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        return new DateTime(longVal);

                    case (int)tagVT.VT_USERDEFINED:
                        throw new ArgumentException(string.Format(SR.COM2UnhandledVT, "VT_USERDEFINED"));

                    case (int)tagVT.VT_ARRAY:
                    case (int)tagVT.VT_VOID:
                    case (int)tagVT.VT_PTR:
                    case (int)tagVT.VT_SAFEARRAY:
                    case (int)tagVT.VT_CARRAY:

                    case (int)tagVT.VT_RECORD:
                    case (int)tagVT.VT_BLOB:
                    case (int)tagVT.VT_STREAM:
                    case (int)tagVT.VT_STORAGE:
                    case (int)tagVT.VT_STREAMED_OBJECT:
                    case (int)tagVT.VT_STORED_OBJECT:
                    case (int)tagVT.VT_BLOB_OBJECT:
                    case (int)tagVT.VT_CF:
                    case (int)tagVT.VT_BSTR_BLOB:
                    case (int)tagVT.VT_VECTOR:
                    case (int)tagVT.VT_BYREF:
                    default:
                        int iVt = vt;
                        throw new ArgumentException(string.Format(SR.COM2UnhandledVT, iVt.ToString(CultureInfo.InvariantCulture)));
                }
            }

            private static IntPtr GetRefInt(IntPtr value)
            {
                return Marshal.ReadIntPtr(value);
            }
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagLICINFO
        {
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=0, cb)*/]
            public int cbLicInfo = Marshal.SizeOf<tagLICINFO>();

            public int fRuntimeAvailable = 0;
            public int fLicVerified = 0;
        }

        public enum tagVT
        {
            VT_EMPTY = 0,
            VT_NULL = 1,
            VT_I2 = 2,
            VT_I4 = 3,
            VT_R4 = 4,
            VT_R8 = 5,
            VT_CY = 6,
            VT_DATE = 7,
            VT_BSTR = 8,
            VT_DISPATCH = 9,
            VT_ERROR = 10,
            VT_BOOL = 11,
            VT_VARIANT = 12,
            VT_UNKNOWN = 13,
            VT_DECIMAL = 14,
            VT_I1 = 16,
            VT_UI1 = 17,
            VT_UI2 = 18,
            VT_UI4 = 19,
            VT_I8 = 20,
            VT_UI8 = 21,
            VT_INT = 22,
            VT_UINT = 23,
            VT_VOID = 24,
            VT_HRESULT = 25,
            VT_PTR = 26,
            VT_SAFEARRAY = 27,
            VT_CARRAY = 28,
            VT_USERDEFINED = 29,
            VT_LPSTR = 30,
            VT_LPWSTR = 31,
            VT_RECORD = 36,
            VT_FILETIME = 64,
            VT_BLOB = 65,
            VT_STREAM = 66,
            VT_STORAGE = 67,
            VT_STREAMED_OBJECT = 68,
            VT_STORED_OBJECT = 69,
            VT_BLOB_OBJECT = 70,
            VT_CF = 71,
            VT_CLSID = 72,
            VT_BSTR_BLOB = 4095,
            VT_VECTOR = 4096,
            VT_ARRAY = 8192,
            VT_BYREF = 16384,
            VT_RESERVED = 32768,
            VT_ILLEGAL = 65535,
            VT_ILLEGALMASKED = 4095,
            VT_TYPEMASK = 4095
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct WNDCLASS
        {
            public ClassStyle style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public char* lpszMenuName;
            public char* lpszClassName;
        }

        public class MSOCM
        {
            // MSO Component registration flags
            public const int msocrfNeedIdleTime = 1;
            public const int msocrfNeedPeriodicIdleTime = 2;
            public const int msocrfPreTranslateKeys = 4;
            public const int msocrfPreTranslateAll = 8;
            public const int msocrfNeedSpecActiveNotifs = 16;
            public const int msocrfNeedAllActiveNotifs = 32;
            public const int msocrfExclusiveBorderSpace = 64;
            public const int msocrfExclusiveActivation = 128;
            public const int msocrfNeedAllMacEvents = 256;
            public const int msocrfMaster = 512;

            // MSO Component registration advise flags (see msocstate enumeration)
            public const int msocadvfModal = 1;
            public const int msocadvfRedrawOff = 2;
            public const int msocadvfWarningsOff = 4;
            public const int msocadvfRecording = 8;

            // MSO Component Host flags
            public const int msochostfExclusiveBorderSpace = 1;

            // MSO idle flags, passed to IMsoComponent::FDoIdle and
            // IMsoStdComponentMgr::FDoIdle.
            public const int msoidlefPeriodic = 1;
            public const int msoidlefNonPeriodic = 2;
            public const int msoidlefPriority = 4;
            public const int msoidlefAll = -1;

            // MSO Reasons for pushing a message loop, passed to
            // IMsoComponentManager::FPushMessageLoop and
            // IMsoComponentHost::FPushMessageLoop.  The host should remain in message
            // loop until IMsoComponent::FContinueMessageLoop
            public const int msoloopDoEventsModal = -2; // Note this is not an official MSO loop -- it just must be distinct.
            public const int msoloopMain = -1; // Note this is not an official MSO loop -- it just must be distinct.
            public const int msoloopFocusWait = 1;
            public const int msoloopDoEvents = 2;
            public const int msoloopDebug = 3;
            public const int msoloopModalForm = 4;
            public const int msoloopModalAlert = 5;

            /* msocstate values: state IDs passed to
                IMsoComponent::OnEnterState,
                IMsoComponentManager::OnComponentEnterState/FOnComponentExitState/FInState,
                IMsoComponentHost::OnComponentEnterState,
                IMsoStdComponentMgr::OnHostEnterState/FOnHostExitState/FInState.
                When the host or a component is notified through one of these methods that
                another entity (component or host) is entering or exiting a state
                identified by one of these state IDs, the host/component should take
                appropriate action:
                    msocstateModal (modal state):
                        If app is entering modal state, host/component should disable
                        its toplevel windows, and reenable them when app exits this
                        state.  Also, when this state is entered or exited, host/component
                        should notify approprate inplace objects via
                        IOleInPlaceActiveObject::EnableModeless.
                    msocstateRedrawOff (redrawOff state):
                        If app is entering redrawOff state, host/component should disable
                        repainting of its windows, and reenable repainting when app exits
                        this state.
                    msocstateWarningsOff (warningsOff state):
                        If app is entering warningsOff state, host/component should disable
                        the presentation of any user warnings, and reenable this when
                        app exits this state.
                    msocstateRecording (Recording state):
                        Used to notify host/component when Recording is turned on or off. */
            public const int msocstateModal = 1;
            public const int msocstateRedrawOff = 2;
            public const int msocstateWarningsOff = 3;
            public const int msocstateRecording = 4;

            /*             ** Comments on State Contexts **
            IMsoComponentManager::FCreateSubComponentManager allows one to create a
            hierarchical tree of component managers.  This tree is used to maintain
            multiple contexts with regard to msocstateXXX states.  These contexts are
            referred to as 'state contexts'.
            Each component manager in the tree defines a state context.  The
            components registered with a particular component manager or any of its
            descendents live within that component manager's state context.  Calls
            to IMsoComponentManager::OnComponentEnterState/FOnComponentExitState
            can be used to  affect all components, only components within the component
            manager's state context, or only those components that are outside of the
            component manager's state context.  IMsoComponentManager::FInState is used
            to query the state of the component manager's state context at its root.

            msoccontext values: context indicators passed to
            IMsoComponentManager::OnComponentEnterState/FOnComponentExitState.
            These values indicate the state context that is to be affected by the
            state change.
            In IMsoComponentManager::OnComponentEnterState/FOnComponentExitState,
            the comp mgr informs only those components/host that are within the
            specified state context. */
            public const int msoccontextAll = 0;
            public const int msoccontextMine = 1;
            public const int msoccontextOthers = 2;

            /*     ** WM_MOUSEACTIVATE Note (for top level compoenents and host) **
            If the active (or tracking) comp's reg info indicates that it
            wants mouse messages, then no MA_xxxANDEAT value should be returned
            from WM_MOUSEACTIVATE, so that the active (or tracking) comp will be able
            to process the resulting mouse message.  If one does not want to examine
            the reg info, no MA_xxxANDEAT value should be returned from
            WM_MOUSEACTIVATE if any comp is active (or tracking).
            One can query the reg info of the active (or tracking) component at any
            time via IMsoComponentManager::FGetActiveComponent. */

            /* msogac values: values passed to
            IMsoComponentManager::FGetActiveComponent. */
            public const int msogacActive = 0;
            public const int msogacTracking = 1;
            public const int msogacTrackingOrActive = 2;

            /* msocWindow values: values passed to IMsoComponent::HwndGetWindow. */
            public const int msocWindowFrameToplevel = 0;
            public const int msocWindowFrameOwner = 1;
            public const int msocWindowComponent = 2;
            public const int msocWindowDlgOwner = 3;
        }

        [StructLayout(LayoutKind.Sequential)]
        public sealed class tagDVTARGETDEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int tdSize = 0;
            [MarshalAs(UnmanagedType.U2)]
            public short tdDriverNameOffset = 0;
            [MarshalAs(UnmanagedType.U2)]
            public short tdDeviceNameOffset = 0;
            [MarshalAs(UnmanagedType.U2)]
            public short tdPortNameOffset = 0;
            [MarshalAs(UnmanagedType.U2)]
            public short tdExtDevmodeOffset = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct TV_ITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public IntPtr /* LPTSTR */ pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct TV_INSERTSTRUCT
        {
            public IntPtr hParent;
            public IntPtr hInsertAfter;
            public int item_mask;
            public IntPtr item_hItem;
            public int item_state;
            public int item_stateMask;
            public IntPtr /* LPTSTR */ item_pszText;
            public int item_cchTextMax;
            public int item_iImage;
            public int item_iSelectedImage;
            public int item_cChildren;
            public IntPtr item_lParam;
            public int item_iIntegral;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMTREEVIEW
        {
            public NMHDR nmhdr;
            public int action;
            public TV_ITEM itemOld;
            public TV_ITEM itemNew;
            public Point ptDrag;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NMTVDISPINFO
        {
            public NMHDR hdr;
            public TV_ITEM item;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct HIGHCONTRASTW
        {
            public uint cbSize;
            public uint dwFlags;
            public IntPtr lpszDefaultScheme;
        }

        public enum HRESULT : long
        {
            S_FALSE = 0x0001,
            S_OK = 0x0000,
            E_INVALIDARG = 0x80070057,
            E_OUTOFMEMORY = 0x8007000E,
            ERROR_CANCELLED = 0x800704C7
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TCITEM_T
        {
            public int mask;
            public int dwState = 0;
            public int dwStateMask = 0;
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagDISPPARAMS
        {
            public IntPtr rgvarg;
            public IntPtr rgdispidNamedArgs;
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=8, cArgs)*/]
            public int cArgs;
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=12, cNamedArgs)*/]
            public int cNamedArgs;
        }

        public enum tagINVOKEKIND
        {
            INVOKE_FUNC = 1,
            INVOKE_PROPERTYGET = 2,
            INVOKE_PROPERTYPUT = 4,
            INVOKE_PROPERTYPUTREF = 8
        }

        [StructLayout(LayoutKind.Sequential)]
        public class tagEXCEPINFO
        {
            [MarshalAs(UnmanagedType.U2)]
            public short wCode = 0;
            [MarshalAs(UnmanagedType.U2)]
            public short wReserved = 0;
            [MarshalAs(UnmanagedType.BStr)]
            public string bstrSource = null;
            [MarshalAs(UnmanagedType.BStr)]
            public string bstrDescription = null;
            [MarshalAs(UnmanagedType.BStr)]
            public string bstrHelpFile = null;
            [MarshalAs(UnmanagedType.U4)]
            public int dwHelpContext = 0;

            public IntPtr pvReserved = IntPtr.Zero;

            public IntPtr pfnDeferredFillIn = IntPtr.Zero;
            [MarshalAs(UnmanagedType.U4)]
            public int scode = 0;
        }

        public enum tagDESCKIND
        {
            DESCKIND_NONE = 0,
            DESCKIND_FUNCDESC = 1,
            DESCKIND_VARDESC = 2,
            DESCKIND_TYPECOMP = 3,
            DESCKIND_IMPLICITAPPOBJ = 4,
            DESCKIND_MAX = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct tagFUNCDESC
        {
            public int memid;

            public IntPtr lprgscode;

            // This is marked as NATIVE_TYPE_PTR,
            // but the EE doesn't look for that, tries to handle it as
            // a ELEMENT_TYPE_VALUECLASS and fails because it
            // isn't a NATIVE_TYPE_NESTEDSTRUCT
            /*[MarshalAs(UnmanagedType.PTR)]*/

            public    /*NativeMethods.tagELEMDESC*/ IntPtr lprgelemdescParam;

            // cpb, Microsoft, the EE chokes on Enums in structs

            public    /*NativeMethods.tagFUNCKIND*/ int funckind;

            public    /*NativeMethods.tagINVOKEKIND*/ int invkind;

            public    /*NativeMethods.tagCALLCONV*/ int callconv;
            [MarshalAs(UnmanagedType.I2)]
            public short cParams;
            [MarshalAs(UnmanagedType.I2)]
            public short cParamsOpt;
            [MarshalAs(UnmanagedType.I2)]
            public short oVft;
            [MarshalAs(UnmanagedType.I2)]
            public short cScodesi;
            public tagELEMDESC elemdescFunc;
            [MarshalAs(UnmanagedType.U2)]
            public short wFuncFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct tagVARDESC
        {
            public int memid;
            public IntPtr lpstrSchema;
            public IntPtr unionMember;
            public tagELEMDESC elemdescVar;
            [MarshalAs(UnmanagedType.U2)]
            public short wVarFlags;
            public    /*NativeMethods.tagVARKIND*/ int varkind;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct HDLAYOUT
        {
            public IntPtr prc;        // pointer to a RECT
            public IntPtr pwpos;      // pointer to a WINDOWPOS
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DRAWITEMSTRUCT
        {
            public int CtlType = 0;
            public int CtlID = 0;
            public int itemID = 0;
            public int itemAction = 0;
            public int itemState = 0;
            public IntPtr hwndItem = IntPtr.Zero;
            public IntPtr hDC = IntPtr.Zero;
            public Interop.RECT rcItem;
            public IntPtr itemData = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MEASUREITEMSTRUCT
        {
            public int CtlType = 0;
            public int CtlID = 0;
            public int itemID = 0;
            public int itemWidth = 0;
            public int itemHeight = 0;
            public IntPtr itemData = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class HELPINFO
        {
            public int cbSize = Marshal.SizeOf<HELPINFO>();
            public int iContextType = 0;
            public int iCtrlId = 0;
            public IntPtr hItemHandle = IntPtr.Zero;
            public IntPtr dwContextId = IntPtr.Zero;
            public Point MousePos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ACCEL
        {
            public byte fVirt;
            public short key;
            public short cmd;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MINMAXINFO
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }

        [ComImport]
        [Guid("B196B28B-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISpecifyPropertyPages
        {
            void GetPages(
               [Out]
                tagCAUUID pPages);
        }

        [StructLayout(LayoutKind.Sequential)/*leftover(noAutoOffset)*/]
        public sealed class tagCAUUID
        {
            [MarshalAs(UnmanagedType.U4)/*leftover(offset=0, cElems)*/]
            public int cElems = 0;
            public IntPtr pElems = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMTOOLBAR
        {
            public NMHDR hdr;
            public int iItem;
            public TBBUTTON tbButton;
            public int cchText;
            public IntPtr pszText;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TBBUTTON
        {
            public int iBitmap;
            public int idCommand;
            public byte fsState;
            public byte fsStyle;
            public byte bReserved0;
            public byte bReserved1;
            public IntPtr dwData;
            public IntPtr iString;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TOOLTIPTEXT
        {
            public NMHDR hdr;
            public string lpszText;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szText = null;

            public IntPtr hinst;
            public int uFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMTBHOTITEM
        {
            public NMHDR hdr;
            public int idOld;
            public int idNew;
            public int dwFlags;
        }

        public const int HICF_OTHER = 0x00000000;
        public const int HICF_MOUSE = 0x00000001;          // Triggered by mouse
        public const int HICF_ARROWKEYS = 0x00000002;          // Triggered by arrow keys
        public const int HICF_ACCELERATOR = 0x00000004;          // Triggered by accelerator
        public const int HICF_DUPACCEL = 0x00000008;          // This accelerator is not unique
        public const int HICF_ENTERING = 0x00000010;          // idOld is invalid
        public const int HICF_LEAVING = 0x00000020;          // idNew is invalid
        public const int HICF_RESELECT = 0x00000040;          // hot item reselected
        public const int HICF_LMOUSE = 0x00000080;          // left mouse button selected
        public const int HICF_TOGGLEDROPDOWN = 0x00000100;          // Toggle button's dropdown state

        // HDN_ITEMCHANGING will send us an HDITEM w/ pszText set to some random pointer.
        // Marshal.PtrToStructure chokes when it has to convert a random pointer to a string.
        // For HDN_ITEMCHANGING map pszText to an IntPtr
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HDITEM2
        {
            public int mask = 0;
            public int cxy = 0;
            public IntPtr pszText_notUsed = IntPtr.Zero;
            public IntPtr hbm = IntPtr.Zero;
            public int cchTextMax = 0;
            public int fmt = 0;
            public IntPtr lParam = IntPtr.Zero;
            public int iImage = 0;
            public int iOrder = 0;
            public int type = 0;
            public IntPtr pvFilter = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct TBBUTTONINFO
        {
            public int cbSize;
            public int dwMask;
            public int idCommand;
            public int iImage;
            public byte fsState;
            public byte fsStyle;
            public short cx;
            public IntPtr lParam;
            public IntPtr pszText;
            public int cchTest;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TV_HITTESTINFO
        {
            public int pt_x;
            public int pt_y;
            public int flags = 0;
            public IntPtr hItem = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NMTVCUSTOMDRAW
        {
            public NMCUSTOMDRAW nmcd;
            public int clrText;
            public int clrTextBk;
            public int iLevel = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMCUSTOMDRAW
        {
            public NMHDR nmcd;
            public int dwDrawStage;
            public IntPtr hdc;
            public Interop.RECT rc;
            public IntPtr dwItemSpec;
            public int uItemState;
            public IntPtr lItemlParam;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MCHITTESTINFO
        {
            public int cbSize = Marshal.SizeOf<MCHITTESTINFO>();
            public int pt_x = 0;
            public int pt_y = 0;
            public int uHit = 0;
            public short st_wYear = 0;
            public short st_wMonth = 0;
            public short st_wDayOfWeek = 0;
            public short st_wDay = 0;
            public short st_wHour = 0;
            public short st_wMinute = 0;
            public short st_wSecond = 0;
            public short st_wMilliseconds = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMSELCHANGE
        {
            public NMHDR nmhdr;
            public SYSTEMTIME stSelStart = null;
            public SYSTEMTIME stSelEnd = null;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMDAYSTATE
        {
            public NMHDR nmhdr;
            public SYSTEMTIME stStart = null;
            public int cDayState = 0;
            public IntPtr prgDayState;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMVIEWCHANGE
        {
            public NMHDR nmhdr;
            public uint uOldView;
            public uint uNewView;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMLVCUSTOMDRAW
        {
            public NMCUSTOMDRAW nmcd;
            public int clrText;
            public int clrTextBk;
            public int iSubItem;
            public int dwItemType;
            // Item Custom Draw
            public int clrFace;
            public int iIconEffect;
            public int iIconPhase;
            public int iPartId;
            public int iStateId;
            // Group Custom Draw
            public Interop.RECT rcText;
            public uint uAlign;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMLVGETINFOTIP
        {
            public NMHDR nmhdr;
            public int flags = 0;
            public IntPtr lpszText = IntPtr.Zero;
            public int cchTextMax = 0;
            public int item = 0;
            public int subItem = 0;
            public IntPtr lParam = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NMLVKEYDOWN
        {
            public NMHDR hdr;
            public short wVKey = 0;
            public uint flags = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVHITTESTINFO
        {
            public int pt_x;
            public int pt_y;
            public int flags = 0;
            public int iItem = 0;
            public int iSubItem = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVBKIMAGE
        {
            public int ulFlags;
            public IntPtr hBmp = IntPtr.Zero; // not used
            public string pszImage;
            public int cchImageMax;
            public int xOffset;
            public int yOffset;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVCOLUMN_T
        {
            public int mask = 0;
            public int fmt = 0;
            public int cx = 0;
            public string pszText = null;
            public int cchTextMax = 0;
            public int iSubItem = 0;
            public int iImage = 0;
            public int iOrder = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVFINDINFO
        {
            public int flags;
            public string psz;
            public IntPtr lParam;
            public Point pt;
            public int vkDirection;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns; // tile view columns
            public IntPtr puColumns;

            public unsafe void Reset()
            {
                pszText = null;
                mask = 0;
                iItem = 0;
                iSubItem = 0;
                stateMask = 0;
                state = 0;
                cchTextMax = 0;
                iImage = 0;
                lParam = IntPtr.Zero;
                iIndent = 0;
                iGroupId = 0;
                cColumns = 0;
                puColumns = IntPtr.Zero;
            }

            public override string ToString()
            {
                return "LVITEM: pszText = " + pszText
                     + ", iItem = " + iItem.ToString(CultureInfo.InvariantCulture)
                     + ", iSubItem = " + iSubItem.ToString(CultureInfo.InvariantCulture)
                     + ", state = " + state.ToString(CultureInfo.InvariantCulture)
                     + ", iGroupId = " + iGroupId.ToString(CultureInfo.InvariantCulture)
                     + ", cColumns = " + cColumns.ToString(CultureInfo.InvariantCulture);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM_NOTEXT
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public IntPtr /*string*/   pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVCOLUMN
        {
            public int mask;
            public int fmt;
            public int cx = 0;
            public IntPtr /* LPWSTR */ pszText;
            public int cchTextMax = 0;
            public int iSubItem = 0;
            public int iImage;
            public int iOrder = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class LVGROUP
        {
            public uint cbSize = (uint)Marshal.SizeOf<LVGROUP>();
            public uint mask;
            public IntPtr pszHeader;
            public int cchHeader;
            public IntPtr pszFooter = IntPtr.Zero;
            public int cchFooter = 0;
            public int iGroupId;
            public uint stateMask = 0;
            public uint state = 0;
            public uint uAlign;

            public override string ToString()
            {
                return "LVGROUP: header = " + pszHeader.ToString() + ", iGroupId = " + iGroupId.ToString(CultureInfo.InvariantCulture);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVINSERTMARK
        {
            public uint cbSize = (uint)Marshal.SizeOf<LVINSERTMARK>();
            public int dwFlags;
            public int iItem;
            public int dwReserved = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LVTILEVIEWINFO
        {
            public uint cbSize = (uint)Marshal.SizeOf<LVTILEVIEWINFO>();
            public int dwMask;
            public int dwFlags;
            public Size sizeTile;
            public int cLines;
            public Interop.RECT rcLabelMargin;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMLVCACHEHINT
        {
            public NMHDR hdr;
            public int iFrom = 0;
            public int iTo = 0;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMLVDISPINFO
        {
            public NMHDR hdr;
            public LVITEM item;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMLVDISPINFO_NOTEXT
        {
            public NMHDR hdr;
            public LVITEM_NOTEXT item;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NMLVODSTATECHANGE
        {
            public NMHDR hdr;
            public int iFrom = 0;
            public int iTo = 0;
            public int uNewState = 0;
            public int uOldState = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CLIENTCREATESTRUCT
        {
            public IntPtr hWindowMenu;
            public int idFirstChild;

            public CLIENTCREATESTRUCT(IntPtr hmenu, int idFirst)
            {
                hWindowMenu = hmenu;
                idFirstChild = idFirst;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class NMDATETIMECHANGE
        {
            public NMHDR nmhdr;
            public int dwFlags = 0;
            public SYSTEMTIME st = null;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class COPYDATASTRUCT
        {
            public int dwData = 0;
            public int cbData = 0;
            public IntPtr lpData = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NMHEADER
        {
            public NMHDR nmhdr;
            public int iItem = 0;
            public int iButton = 0;
            public IntPtr pItem = IntPtr.Zero;    // HDITEM*
        }

        [StructLayout(LayoutKind.Sequential)]
        public class MOUSEHOOKSTRUCT
        {
            public Point pt;
            public IntPtr hWnd = IntPtr.Zero;
            public int wHitTestCode = 0;
            public int dwExtraInfo = 0;
        }

        #region SendKeys SendInput functionality

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public INPUTUNION inputUnion;
        }

        // We need to split the field offset out into a union struct to avoid
        // silent problems in 64 bit
        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        #endregion

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct CHARFORMATW
        {
            private const int LF_FACESIZE = 32;

            public int cbSize;
            public int dwMask;
            public int dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;

            private fixed char _szFaceName[LF_FACESIZE];
            private Span<char> szFaceName
            {
                get { fixed (char* c = _szFaceName) { return new Span<char>(c, LF_FACESIZE); } }
            }

            public ReadOnlySpan<char> FaceName
            {
                get => szFaceName.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szFaceName);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public class CHARFORMATA
        {
            public int cbSize = Marshal.SizeOf<CHARFORMATA>();
            public int dwMask;
            public int dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] szFaceName = new byte[32];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public class CHARFORMAT2A
        {
            public int cbSize = Marshal.SizeOf<CHARFORMAT2A>();
            public int dwMask = 0;
            public int dwEffects = 0;
            public int yHeight = 0;
            public int yOffset = 0;
            public int crTextColor = 0;
            public byte bCharSet = 0;
            public byte bPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] szFaceName = new byte[32];
            public short wWeight = 0;
            public short sSpacing = 0;
            public int crBackColor = 0;
            public int lcid = 0;
            public int dwReserved = 0;
            public short sStyle = 0;
            public short wKerning = 0;
            public byte bUnderlineType = 0;
            public byte bAnimation = 0;
            public byte bRevAuthor = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class TEXTRANGE
        {
            public Interop.Richedit.CHARRANGE chrg;
            public IntPtr lpstrText; /* allocated by caller, zero terminated by RichEdit */
        }

        [StructLayout(LayoutKind.Sequential)]
        public class GETTEXTLENGTHEX
        {                               // Taken from richedit.h:
            public uint flags;          // Flags (see GTL_XXX defines)
            public uint codepage;       // Code page for translation (CP_ACP for default, 1200 for Unicode)
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        public class SELCHANGE
        {
            public NMHDR nmhdr;
            public Interop.Richedit.CHARRANGE chrg;
            public int seltyp = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class PARAFORMAT
        {
            public int cbSize = Marshal.SizeOf<PARAFORMAT>();
            public int dwMask;
            public short wNumbering;
            public short wReserved = 0;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class FINDTEXT
        {
            public Interop.Richedit.CHARRANGE chrg;
            public string lpstrText;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ENLINK
        {
            public NMHDR nmhdr;
            public int msg = 0;
            public IntPtr wParam = IntPtr.Zero;
            public IntPtr lParam = IntPtr.Zero;
            public Interop.Richedit.CHARRANGE charrange;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ENLINK64
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
            public byte[] contents = new byte[56];
        }

        // GetRegionData structures
        [StructLayout(LayoutKind.Sequential)]
        public struct RGNDATAHEADER
        {
            public int cbSizeOfStruct;
            public int iType;
            public int nCount;
            public int nRgnSize;
            // public Interop.RECT rcBound; // Note that we don't define this field as part of the marshaling
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class OCPFIPARAMS
        {
            public int cbSizeOfStruct = Marshal.SizeOf<OCPFIPARAMS>();
            public IntPtr hwndOwner;
            public int x = 0;
            public int y = 0;
            public string lpszCaption;
            public int cObjects = 1;
            public IntPtr ppUnk;
            public int pageCount = 1;
            public IntPtr uuid;
            public int lcid = Application.CurrentCulture.LCID;
            public int dispidInitial;
        }

        [ComVisible(true), StructLayout(LayoutKind.Sequential)]
        public class DOCHOSTUIINFO
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize = Marshal.SizeOf<DOCHOSTUIINFO>();
            [MarshalAs(UnmanagedType.I4)]
            public int dwFlags;
            [MarshalAs(UnmanagedType.I4)]
            public int dwDoubleClick;
            [MarshalAs(UnmanagedType.I4)]
            public int dwReserved1 = 0;
            [MarshalAs(UnmanagedType.I4)]
            public int dwReserved2 = 0;
        }

        public enum DOCHOSTUIFLAG
        {
            DIALOG = 0x1,
            DISABLE_HELP_MENU = 0x2,
            NO3DBORDER = 0x4,
            SCROLL_NO = 0x8,
            DISABLE_SCRIPT_INACTIVE = 0x10,
            OPENNEWWIN = 0x20,
            DISABLE_OFFSCREEN = 0x40,
            FLAT_SCROLLBAR = 0x80,
            DIV_BLOCKDEFAULT = 0x100,
            ACTIVATE_CLIENTHIT_ONLY = 0x200,
            NO3DOUTERBORDER = 0x00200000,
            THEME = 0x00040000,
            NOTHEME = 0x80000,
            DISABLE_COOKIE = 0x400
        }

        public enum DOCHOSTUIDBLCLICK
        {
            DEFAULT = 0x0,
            SHOWPROPERTIES = 0x1,
            SHOWCODE = 0x2
        }

#pragma warning disable CA1712 // Don't prefix enum values with enum type
        public enum OLECMDID
        {
            OLECMDID_SAVEAS = 4,
            OLECMDID_PRINT = 6,
            OLECMDID_PRINTPREVIEW = 7,
            OLECMDID_PAGESETUP = 8,
            OLECMDID_PROPERTIES = 10
        }

        public enum OLECMDEXECOPT
        {
            OLECMDEXECOPT_DODEFAULT = 0,
            OLECMDEXECOPT_PROMPTUSER = 1,
            OLECMDEXECOPT_DONTPROMPTUSER = 2,
            OLECMDEXECOPT_SHOWHELP = 3
        }

        public enum OLECMDF
        {
            OLECMDF_SUPPORTED = 0x00000001,
            OLECMDF_ENABLED = 0x00000002,
            OLECMDF_LATCHED = 0x00000004,
            OLECMDF_NINCHED = 0x00000008,
            OLECMDF_INVISIBLE = 0x00000010,
            OLECMDF_DEFHIDEONCTXTMENU = 0x00000020
        }
#pragma warning restore CA1712

        [StructLayout(LayoutKind.Sequential)]
        public class ENDROPFILES
        {
            public NMHDR nmhdr;
            public IntPtr hDrop = IntPtr.Zero;
            public int cp = 0;
            public bool fProtected = false;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class REQRESIZE
        {
            public NMHDR nmhdr;
            public Interop.RECT rc;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class ENPROTECTED
        {
            public NMHDR nmhdr;
            public int msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public Interop.Richedit.CHARRANGE chrg;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class ENPROTECTED64
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
            public byte[] contents = new byte[56];
        }

        public class ActiveX
        {
            public const int OCM__BASE = 0x2000;
            public const int DISPID_VALUE = unchecked((int)0x0);
            public const int DISPID_UNKNOWN = unchecked((int)0xFFFFFFFF);
            public const int DISPID_AUTOSIZE = unchecked((int)0xFFFFFE0C);
            public const int DISPID_BACKCOLOR = unchecked((int)0xFFFFFE0B);
            public const int DISPID_BACKSTYLE = unchecked((int)0xFFFFFE0A);
            public const int DISPID_BORDERCOLOR = unchecked((int)0xFFFFFE09);
            public const int DISPID_BORDERSTYLE = unchecked((int)0xFFFFFE08);
            public const int DISPID_BORDERWIDTH = unchecked((int)0xFFFFFE07);
            public const int DISPID_DRAWMODE = unchecked((int)0xFFFFFE05);
            public const int DISPID_DRAWSTYLE = unchecked((int)0xFFFFFE04);
            public const int DISPID_DRAWWIDTH = unchecked((int)0xFFFFFE03);
            public const int DISPID_FILLCOLOR = unchecked((int)0xFFFFFE02);
            public const int DISPID_FILLSTYLE = unchecked((int)0xFFFFFE01);
            public const int DISPID_FONT = unchecked((int)0xFFFFFE00);
            public const int DISPID_FORECOLOR = unchecked((int)0xFFFFFDFF);
            public const int DISPID_ENABLED = unchecked((int)0xFFFFFDFE);
            public const int DISPID_HWND = unchecked((int)0xFFFFFDFD);
            public const int DISPID_TABSTOP = unchecked((int)0xFFFFFDFC);
            public const int DISPID_TEXT = unchecked((int)0xFFFFFDFB);
            public const int DISPID_CAPTION = unchecked((int)0xFFFFFDFA);
            public const int DISPID_BORDERVISIBLE = unchecked((int)0xFFFFFDF9);
            public const int DISPID_APPEARANCE = unchecked((int)0xFFFFFDF8);
            public const int DISPID_MOUSEPOINTER = unchecked((int)0xFFFFFDF7);
            public const int DISPID_MOUSEICON = unchecked((int)0xFFFFFDF6);
            public const int DISPID_PICTURE = unchecked((int)0xFFFFFDF5);
            public const int DISPID_VALID = unchecked((int)0xFFFFFDF4);
            public const int DISPID_READYSTATE = unchecked((int)0xFFFFFDF3);
            public const int DISPID_REFRESH = unchecked((int)0xFFFFFDDA);
            public const int DISPID_DOCLICK = unchecked((int)0xFFFFFDD9);
            public const int DISPID_ABOUTBOX = unchecked((int)0xFFFFFDD8);
            public const int DISPID_CLICK = unchecked((int)0xFFFFFDA8);
            public const int DISPID_DBLCLICK = unchecked((int)0xFFFFFDA7);
            public const int DISPID_KEYDOWN = unchecked((int)0xFFFFFDA6);
            public const int DISPID_KEYPRESS = unchecked((int)0xFFFFFDA5);
            public const int DISPID_KEYUP = unchecked((int)0xFFFFFDA4);
            public const int DISPID_MOUSEDOWN = unchecked((int)0xFFFFFDA3);
            public const int DISPID_MOUSEMOVE = unchecked((int)0xFFFFFDA2);
            public const int DISPID_MOUSEUP = unchecked((int)0xFFFFFDA1);
            public const int DISPID_ERROREVENT = unchecked((int)0xFFFFFDA0);
            public const int DISPID_RIGHTTOLEFT = unchecked((int)0xFFFFFD9D);
            public const int DISPID_READYSTATECHANGE = unchecked((int)0xFFFFFD9F);
            public const int DISPID_AMBIENT_BACKCOLOR = unchecked((int)0xFFFFFD43);
            public const int DISPID_AMBIENT_DISPLAYNAME = unchecked((int)0xFFFFFD42);
            public const int DISPID_AMBIENT_FONT = unchecked((int)0xFFFFFD41);
            public const int DISPID_AMBIENT_FORECOLOR = unchecked((int)0xFFFFFD40);
            public const int DISPID_AMBIENT_LOCALEID = unchecked((int)0xFFFFFD3F);
            public const int DISPID_AMBIENT_MESSAGEREFLECT = unchecked((int)0xFFFFFD3E);
            public const int DISPID_AMBIENT_SCALEUNITS = unchecked((int)0xFFFFFD3D);
            public const int DISPID_AMBIENT_TEXTALIGN = unchecked((int)0xFFFFFD3C);
            public const int DISPID_AMBIENT_USERMODE = unchecked((int)0xFFFFFD3B);
            public const int DISPID_AMBIENT_UIDEAD = unchecked((int)0xFFFFFD3A);
            public const int DISPID_AMBIENT_SHOWGRABHANDLES = unchecked((int)0xFFFFFD39);
            public const int DISPID_AMBIENT_SHOWHATCHING = unchecked((int)0xFFFFFD38);
            public const int DISPID_AMBIENT_DISPLAYASDEFAULT = unchecked((int)0xFFFFFD37);
            public const int DISPID_AMBIENT_SUPPORTSMNEMONICS = unchecked((int)0xFFFFFD36);
            public const int DISPID_AMBIENT_AUTOCLIP = unchecked((int)0xFFFFFD35);
            public const int DISPID_AMBIENT_APPEARANCE = unchecked((int)0xFFFFFD34);
            public const int DISPID_AMBIENT_PALETTE = unchecked((int)0xFFFFFD2A);
            public const int DISPID_AMBIENT_TRANSFERPRIORITY = unchecked((int)0xFFFFFD28);
            public const int DISPID_AMBIENT_RIGHTTOLEFT = unchecked((int)0xFFFFFD24);
            public const int DISPID_Name = unchecked((int)0xFFFFFCE0);
            public const int DISPID_Delete = unchecked((int)0xFFFFFCDF);
            public const int DISPID_Object = unchecked((int)0xFFFFFCDE);
            public const int DISPID_Parent = unchecked((int)0xFFFFFCDD);
            public const int DVASPECT_CONTENT = 0x1;
            public const int DVASPECT_THUMBNAIL = 0x2;
            public const int DVASPECT_ICON = 0x4;
            public const int DVASPECT_DOCPRINT = 0x8;
            public const int OLEMISC_RECOMPOSEONRESIZE = 0x1;
            public const int OLEMISC_ONLYICONIC = 0x2;
            public const int OLEMISC_INSERTNOTREPLACE = 0x4;
            public const int OLEMISC_STATIC = 0x8;
            public const int OLEMISC_CANTLINKINSIDE = 0x10;
            public const int OLEMISC_CANLINKBYOLE1 = 0x20;
            public const int OLEMISC_ISLINKOBJECT = 0x40;
            public const int OLEMISC_INSIDEOUT = 0x80;
            public const int OLEMISC_ACTIVATEWHENVISIBLE = 0x100;
            public const int OLEMISC_RENDERINGISDEVICEINDEPENDENT = 0x200;
            public const int OLEMISC_INVISIBLEATRUNTIME = 0x400;
            public const int OLEMISC_ALWAYSRUN = 0x800;
            public const int OLEMISC_ACTSLIKEBUTTON = 0x1000;
            public const int OLEMISC_ACTSLIKELABEL = 0x2000;
            public const int OLEMISC_NOUIACTIVATE = 0x4000;
            public const int OLEMISC_ALIGNABLE = 0x8000;
            public const int OLEMISC_SIMPLEFRAME = 0x10000;
            public const int OLEMISC_SETCLIENTSITEFIRST = 0x20000;
            public const int OLEMISC_IMEMODE = 0x40000;
            public const int OLEMISC_IGNOREACTIVATEWHENVISIBLE = 0x80000;
            public const int OLEMISC_WANTSTOMENUMERGE = 0x100000;
            public const int OLEMISC_SUPPORTSMULTILEVELUNDO = 0x200000;
            public const int QACONTAINER_SHOWHATCHING = 0x1;
            public const int QACONTAINER_SHOWGRABHANDLES = 0x2;
            public const int QACONTAINER_USERMODE = 0x4;
            public const int QACONTAINER_DISPLAYASDEFAULT = 0x8;
            public const int QACONTAINER_UIDEAD = 0x10;
            public const int QACONTAINER_AUTOCLIP = 0x20;
            public const int QACONTAINER_MESSAGEREFLECT = 0x40;
            public const int QACONTAINER_SUPPORTSMNEMONICS = 0x80;
            public const int XFORMCOORDS_POSITION = 0x1;
            public const int XFORMCOORDS_SIZE = 0x2;
            public const int XFORMCOORDS_HIMETRICTOCONTAINER = 0x4;
            public const int XFORMCOORDS_CONTAINERTOHIMETRIC = 0x8;
            public const int PROPCAT_Nil = unchecked((int)0xFFFFFFFF);
            public const int PROPCAT_Misc = unchecked((int)0xFFFFFFFE);
            public const int PROPCAT_Font = unchecked((int)0xFFFFFFFD);
            public const int PROPCAT_Position = unchecked((int)0xFFFFFFFC);
            public const int PROPCAT_Appearance = unchecked((int)0xFFFFFFFB);
            public const int PROPCAT_Behavior = unchecked((int)0xFFFFFFFA);
            public const int PROPCAT_Data = unchecked((int)0xFFFFFFF9);
            public const int PROPCAT_List = unchecked((int)0xFFFFFFF8);
            public const int PROPCAT_Text = unchecked((int)0xFFFFFFF7);
            public const int PROPCAT_Scale = unchecked((int)0xFFFFFFF6);
            public const int PROPCAT_DDE = unchecked((int)0xFFFFFFF5);
            public const int GC_WCH_SIBLING = 0x1;
            public const int GC_WCH_CONTAINER = 0x2;
            public const int GC_WCH_CONTAINED = 0x3;
            public const int GC_WCH_ALL = 0x4;
            public const int GC_WCH_FREVERSEDIR = 0x8000000;
            public const int GC_WCH_FONLYNEXT = 0x10000000;
            public const int GC_WCH_FONLYPREV = 0x20000000;
            public const int GC_WCH_FSELECTED = 0x40000000;
            public const int OLECONTF_EMBEDDINGS = 0x1;
            public const int OLECONTF_LINKS = 0x2;
            public const int OLECONTF_OTHERS = 0x4;
            public const int OLECONTF_ONLYUSER = 0x8;
            public const int OLECONTF_ONLYIFRUNNING = 0x10;
            public const int ALIGN_MIN = 0x0;
            public const int ALIGN_NO_CHANGE = 0x0;
            public const int ALIGN_TOP = 0x1;
            public const int ALIGN_BOTTOM = 0x2;
            public const int ALIGN_LEFT = 0x3;
            public const int ALIGN_RIGHT = 0x4;
            public const int ALIGN_MAX = 0x4;
            public const int OLEVERBATTRIB_NEVERDIRTIES = 0x1;
            public const int OLEVERBATTRIB_ONCONTAINERMENU = 0x2;

            public static Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");

            private ActiveX()
            {
            }
        }

        public static class Util
        {
            public static int MAKELONG(int low, int high)
            {
                return (high << 16) | (low & 0xffff);
            }

            public static IntPtr MAKELPARAM(int low, int high)
            {
                return (IntPtr)((high << 16) | (low & 0xffff));
            }

            public static int HIWORD(int n)
            {
                return (n >> 16) & 0xffff;
            }

            public static int HIWORD(IntPtr n)
            {
                return HIWORD(unchecked((int)(long)n));
            }

            public static int LOWORD(int n)
            {
                return n & 0xffff;
            }

            public static int LOWORD(IntPtr n)
            {
                return LOWORD(unchecked((int)(long)n));
            }

            public static int SignedHIWORD(IntPtr n)
            {
                return SignedHIWORD(unchecked((int)(long)n));
            }
            public static int SignedLOWORD(IntPtr n)
            {
                return SignedLOWORD(unchecked((int)(long)n));
            }

            public static int SignedHIWORD(int n)
            {
                int i = (int)(short)((n >> 16) & 0xffff);

                return i;
            }

            public static int SignedLOWORD(int n)
            {
                int i = (int)(short)(n & 0xFFFF);

                return i;
            }

            private static int GetEmbeddedNullStringLengthAnsi(string s)
            {
                int n = s.IndexOf('\0');
                if (n > -1)
                {
                    string left = s.Substring(0, n);
                    string right = s.Substring(n + 1);
                    return left.Length + GetEmbeddedNullStringLengthAnsi(right) + 1;
                }
                else
                {
                    return s.Length;
                }
            }

            [DllImport(ExternDll.Kernel32, CharSet = CharSet.Auto)]

            private static extern int lstrlen(string s);
        }

        public enum tagTYPEKIND
        {
            TKIND_ENUM = 0,
            TKIND_RECORD = 1,
            TKIND_MODULE = 2,
            TKIND_INTERFACE = 3,
            TKIND_DISPATCH = 4,
            TKIND_COCLASS = 5,
            TKIND_ALIAS = 6,
            TKIND_UNION = 7,
            TKIND_MAX = 8
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct tagTYPEDESC
        {
            public IntPtr unionMember;
            public short vt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct tagPARAMDESC
        {
            public IntPtr pparamdescex;

            [MarshalAs(UnmanagedType.U2)]
            public short wParamFlags;
        }

        public delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

        [ComImport]
        [Guid("A7ABA9C1-8983-11cf-8F20-00805F2CD064")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IProvideMultipleClassInfo
        {
            // since the inheritance doesn't seem to work...
            // these are from IProvideClassInfo & IProvideClassInfo2
            [PreserveSig]
            UnsafeNativeMethods.ITypeInfo GetClassInfo();

            [PreserveSig]
            int GetGUID(int dwGuidKind, [In, Out] ref Guid pGuid);

            [PreserveSig]
            int GetMultiTypeInfoCount([In, Out] ref int pcti);

            // we use arrays for most of these since we never use them anyway.
            [PreserveSig]
            int GetInfoOfIndex(int iti, int dwFlags,
                                [In, Out]
                                ref UnsafeNativeMethods.ITypeInfo pTypeInfo,
                                int pTIFlags,
                                int pcdispidReserved,
                                IntPtr piidPrimary,
                                IntPtr piidSource);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class EVENTMSG
        {
            public int message;
            public int paramL;
            public int paramH;
            public int time;
            public IntPtr hwnd;
        }

        [ComImport]
        [Guid("B196B283-BAB4-101A-B69C-00AA00341D07")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IProvideClassInfo
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            UnsafeNativeMethods.ITypeInfo GetClassInfo();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct tagTYPEATTR
        {
            public Guid guid;
            [MarshalAs(UnmanagedType.U4)]
            public int lcid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwReserved;
            public int memidConstructor;
            public int memidDestructor;
            public IntPtr lpstrSchema;
            [MarshalAs(UnmanagedType.U4)]
            public int cbSizeInstance;
            public    /*NativeMethods.tagTYPEKIND*/ int typekind;
            [MarshalAs(UnmanagedType.U2)]
            public short cFuncs;
            [MarshalAs(UnmanagedType.U2)]
            public short cVars;
            [MarshalAs(UnmanagedType.U2)]
            public short cImplTypes;
            [MarshalAs(UnmanagedType.U2)]
            public short cbSizeVft;
            [MarshalAs(UnmanagedType.U2)]
            public short cbAlignment;
            [MarshalAs(UnmanagedType.U2)]
            public short wTypeFlags;
            [MarshalAs(UnmanagedType.U2)]
            public short wMajorVerNum;
            [MarshalAs(UnmanagedType.U2)]
            public short wMinorVerNum;

            //these are inline too
            //public    NativeMethods.tagTYPEDESC tdescAlias;
            [MarshalAs(UnmanagedType.U4)]
            public int tdescAlias_unionMember;

            [MarshalAs(UnmanagedType.U2)]
            public short tdescAlias_vt;

            //public    NativeMethods.tagIDLDESC idldescType;
            [MarshalAs(UnmanagedType.U4)]
            public int idldescType_dwReserved;

            [MarshalAs(UnmanagedType.U2)]
            public short idldescType_wIDLFlags;

            public tagTYPEDESC Get_tdescAlias()
            {
                tagTYPEDESC td;
                td.unionMember = (IntPtr)tdescAlias_unionMember;
                td.vt = tdescAlias_vt;
                return td;
            }

            public tagIDLDESC Get_idldescType()
            {
                tagIDLDESC id = new tagIDLDESC
                {
                    dwReserved = idldescType_dwReserved,
                    wIDLFlags = idldescType_wIDLFlags
                };
                return id;
            }
        }

        public enum tagVARFLAGS
        {
            VARFLAG_FREADONLY = 1,
            VARFLAG_FSOURCE = 0x2,
            VARFLAG_FBINDABLE = 0x4,
            VARFLAG_FREQUESTEDIT = 0x8,
            VARFLAG_FDISPLAYBIND = 0x10,
            VARFLAG_FDEFAULTBIND = 0x20,
            VARFLAG_FHIDDEN = 0x40,
            VARFLAG_FDEFAULTCOLLELEM = 0x100,
            VARFLAG_FUIDEFAULT = 0x200,
            VARFLAG_FNONBROWSABLE = 0x400,
            VARFLAG_FREPLACEABLE = 0x800,
            VARFLAG_FIMMEDIATEBIND = 0x1000
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct tagELEMDESC
        {
            public tagTYPEDESC tdesc;
            public tagPARAMDESC paramdesc;
        }

        public enum tagVARKIND
        {
            VAR_PERINSTANCE = 0,
            VAR_STATIC = 1,
            VAR_CONST = 2,
            VAR_DISPATCH = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct tagIDLDESC
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwReserved;
            [MarshalAs(UnmanagedType.U2)]
            public short wIDLFlags;
        }

        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PALETTEENTRY
        {
            public byte peRed;
            public byte peGreen;
            public byte peBlue;
            public byte peFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO_FLAT
        {
            public int bmiHeader_biSize;// = Marshal.SizeOf<BITMAPINFOHEADER>();
            public int bmiHeader_biWidth;
            public int bmiHeader_biHeight;
            public short bmiHeader_biPlanes;
            public short bmiHeader_biBitCount;
            public int bmiHeader_biCompression;
            public int bmiHeader_biSizeImage;
            public int bmiHeader_biXPelsPerMeter;
            public int bmiHeader_biYPelsPerMeter;
            public int bmiHeader_biClrUsed;
            public int bmiHeader_biClrImportant;

            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = BITMAPINFO_MAX_COLORSIZE * 4)]
            public byte[] bmiColors; // RGBQUAD structs... Blue-Green-Red-Reserved, repeat...
        }

        /// <summary>
        ///  This method takes a file URL and converts it to a local path.  The trick here is that
        ///  if there is a '#' in the path, everything after this is treated as a fragment.  So
        ///  we need to append the fragment to the end of the path.
        /// </summary>
        internal static string GetLocalPath(string fileName)
        {
            System.Diagnostics.Debug.Assert(fileName != null && fileName.Length > 0, "Cannot get local path, fileName is not valid");

            Uri uri = new Uri(fileName);
            return uri.LocalPath + uri.Fragment;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_POWER_STATUS
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
        }

        public enum PROCESS_DPI_AWARENESS
        {
            PROCESS_DPI_UNINITIALIZED = -1,
            PROCESS_DPI_UNAWARE = 0,
            PROCESS_SYSTEM_DPI_AWARE = 1,
            PROCESS_PER_MONITOR_DPI_AWARE = 2
        }

        public enum MONTCALENDAR_VIEW_MODE
        {
            MCMV_MONTH = 0,
            MCMV_YEAR = 1,
            MCMV_DECADE = 2,
            MCMV_CENTURY = 3
        }

        public const int DPI_AWARENESS_CONTEXT_UNAWARE = -1;
        public const int DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = -2;
        public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = -3;
        public const int DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4;
        public const int DPI_AWARENESS_CONTEXT_UNAWARE_GDISCALED = -5;

        // Theming/Visual Styles stuff
        public const int STAP_ALLOW_NONCLIENT = (1 << 0);
        public const int STAP_ALLOW_CONTROLS = (1 << 1);
        public const int STAP_ALLOW_WEBCONTENT = (1 << 2);

        public const int PS_NULL = 5;
        public const int PS_INSIDEFRAME = 6;

        public const int PS_GEOMETRIC = 0x00010000;
        public const int PS_ENDCAP_SQUARE = 0x00000100;

        public const int WS_EX_TRANSPARENT = 0x00000020;

        public const int NULL_BRUSH = 5;
        public const int MM_HIMETRIC = 3;

        // Threading stuff
        public const uint STILL_ACTIVE = 259;

        [StructLayout(LayoutKind.Sequential)]
        public struct UiaRect
        {
            public double left;
            public double top;
            public double width;
            public double height;

            public UiaRect(Drawing.Rectangle r)
            {
                left = r.Left;
                top = r.Top;
                width = r.Width;
                height = r.Height;
            }
        }

        // UIAutomation IDs
        // obtained from uiautomationclient.idl and uiautomationcore.idl

        // UIA_PatternIds
        internal const int UIA_InvokePatternId = 10000;
        internal const int UIA_SelectionPatternId = 10001;
        internal const int UIA_ValuePatternId = 10002;
        internal const int UIA_RangeValuePatternId = 10003;
        internal const int UIA_ScrollPatternId = 10004;
        internal const int UIA_ExpandCollapsePatternId = 10005;
        internal const int UIA_GridPatternId = 10006;
        internal const int UIA_GridItemPatternId = 10007;
        internal const int UIA_MultipleViewPatternId = 10008;
        internal const int UIA_WindowPatternId = 10009;
        internal const int UIA_SelectionItemPatternId = 10010;
        internal const int UIA_DockPatternId = 10011;
        internal const int UIA_TablePatternId = 10012;
        internal const int UIA_TableItemPatternId = 10013;
        internal const int UIA_TextPatternId = 10014;
        internal const int UIA_TogglePatternId = 10015;
        internal const int UIA_TransformPatternId = 10016;
        internal const int UIA_ScrollItemPatternId = 10017;
        internal const int UIA_LegacyIAccessiblePatternId = 10018;
        internal const int UIA_ItemContainerPatternId = 10019;
        internal const int UIA_VirtualizedItemPatternId = 10020;
        internal const int UIA_SynchronizedInputPatternId = 10021;
        internal const int UIA_ObjectModelPatternId = 10022;
        internal const int UIA_AnnotationPatternId = 10023;
        internal const int UIA_TextPattern2Id = 10024;
        internal const int UIA_StylesPatternId = 10025;
        internal const int UIA_SpreadsheetPatternId = 10026;
        internal const int UIA_SpreadsheetItemPatternId = 10027;
        internal const int UIA_TransformPattern2Id = 10028;
        internal const int UIA_TextChildPatternId = 10029;
        internal const int UIA_DragPatternId = 10030;
        internal const int UIA_DropTargetPatternId = 10031;
        internal const int UIA_TextEditPatternId = 10032;
        internal const int UIA_CustomNavigationPatternId = 10033;

        // UIA_EventIds
        internal const int UIA_ToolTipOpenedEventId = 20000;
        internal const int UIA_ToolTipClosedEventId = 20001;
        internal const int UIA_StructureChangedEventId = 20002;
        internal const int UIA_MenuOpenedEventId = 20003;
        internal const int UIA_AutomationPropertyChangedEventId = 20004;
        internal const int UIA_AutomationFocusChangedEventId = 20005;
        internal const int UIA_AsyncContentLoadedEventId = 20006;
        internal const int UIA_MenuClosedEventId = 20007;
        internal const int UIA_LayoutInvalidatedEventId = 20008;
        internal const int UIA_Invoke_InvokedEventId = 20009;
        internal const int UIA_SelectionItem_ElementAddedToSelectionEventId = 20010;
        internal const int UIA_SelectionItem_ElementRemovedFromSelectionEventId = 20011;
        internal const int UIA_SelectionItem_ElementSelectedEventId = 20012;
        internal const int UIA_Selection_InvalidatedEventId = 20013;
        internal const int UIA_Text_TextSelectionChangedEventId = 20014;
        internal const int UIA_Text_TextChangedEventId = 20015;
        internal const int UIA_Window_WindowOpenedEventId = 20016;
        internal const int UIA_Window_WindowClosedEventId = 20017;
        internal const int UIA_MenuModeStartEventId = 20018;
        internal const int UIA_MenuModeEndEventId = 20019;
        internal const int UIA_InputReachedTargetEventId = 20020;
        internal const int UIA_InputReachedOtherElementEventId = 20021;
        internal const int UIA_InputDiscardedEventId = 20022;
        internal const int UIA_SystemAlertEventId = 20023;
        internal const int UIA_LiveRegionChangedEventId = 20024;
        internal const int UIA_HostedFragmentRootsInvalidatedEventId = 20025;
        internal const int UIA_Drag_DragStartEventId = 20026;
        internal const int UIA_Drag_DragCancelEventId = 20027;
        internal const int UIA_Drag_DragCompleteEventId = 20028;
        internal const int UIA_DropTarget_DragEnterEventId = 20029;
        internal const int UIA_DropTarget_DragLeaveEventId = 20030;
        internal const int UIA_DropTarget_DroppedEventId = 20031;
        internal const int UIA_TextEdit_TextChangedEventId = 20032;
        internal const int UIA_TextEdit_ConversionTargetChangedEventId = 20033;
        internal const int UIA_ChangesEventId = 20034;

        // UIAutomation PropertyIds
        internal const int UIA_RuntimeIdPropertyId = 30000;
        internal const int UIA_BoundingRectanglePropertyId = 30001;
        internal const int UIA_ProcessIdPropertyId = 30002;
        internal const int UIA_ControlTypePropertyId = 30003;
        internal const int UIA_LocalizedControlTypePropertyId = 30004;
        internal const int UIA_NamePropertyId = 30005;
        internal const int UIA_AcceleratorKeyPropertyId = 30006;
        internal const int UIA_AccessKeyPropertyId = 30007;
        internal const int UIA_HasKeyboardFocusPropertyId = 30008;
        internal const int UIA_IsKeyboardFocusablePropertyId = 30009;
        internal const int UIA_IsEnabledPropertyId = 30010;
        internal const int UIA_AutomationIdPropertyId = 30011;
        internal const int UIA_ClassNamePropertyId = 30012;
        internal const int UIA_HelpTextPropertyId = 30013;
        internal const int UIA_ClickablePointPropertyId = 30014;
        internal const int UIA_CulturePropertyId = 30015;
        internal const int UIA_IsControlElementPropertyId = 30016;
        internal const int UIA_IsContentElementPropertyId = 30017;
        internal const int UIA_LabeledByPropertyId = 30018;
        internal const int UIA_IsPasswordPropertyId = 30019;
        internal const int UIA_NativeWindowHandlePropertyId = 30020;
        internal const int UIA_ItemTypePropertyId = 30021;
        internal const int UIA_IsOffscreenPropertyId = 30022;
        internal const int UIA_OrientationPropertyId = 30023;
        internal const int UIA_FrameworkIdPropertyId = 30024;
        internal const int UIA_IsRequiredForFormPropertyId = 30025;
        internal const int UIA_ItemStatusPropertyId = 30026;
        internal const int UIA_IsDockPatternAvailablePropertyId = 30027;
        internal const int UIA_IsExpandCollapsePatternAvailablePropertyId = 30028;
        internal const int UIA_IsGridItemPatternAvailablePropertyId = 30029;
        internal const int UIA_IsGridPatternAvailablePropertyId = 30030;
        internal const int UIA_IsInvokePatternAvailablePropertyId = 30031;
        internal const int UIA_IsMultipleViewPatternAvailablePropertyId = 30032;
        internal const int UIA_IsRangeValuePatternAvailablePropertyId = 30033;
        internal const int UIA_IsScrollPatternAvailablePropertyId = 30034;
        internal const int UIA_IsScrollItemPatternAvailablePropertyId = 30035;
        internal const int UIA_IsSelectionItemPatternAvailablePropertyId = 30036;
        internal const int UIA_IsSelectionPatternAvailablePropertyId = 30037;
        internal const int UIA_IsTablePatternAvailablePropertyId = 30038;
        internal const int UIA_IsTableItemPatternAvailablePropertyId = 30039;
        internal const int UIA_IsTextPatternAvailablePropertyId = 30040;
        internal const int UIA_IsTogglePatternAvailablePropertyId = 30041;
        internal const int UIA_IsTransformPatternAvailablePropertyId = 30042;
        internal const int UIA_IsValuePatternAvailablePropertyId = 30043;
        internal const int UIA_IsWindowPatternAvailablePropertyId = 30044;
        internal const int UIA_ValueValuePropertyId = 30045;
        internal const int UIA_ValueIsReadOnlyPropertyId = 30046;
        internal const int UIA_RangeValueValuePropertyId = 30047;
        internal const int UIA_RangeValueIsReadOnlyPropertyId = 30048;
        internal const int UIA_RangeValueMinimumPropertyId = 30049;
        internal const int UIA_RangeValueMaximumPropertyId = 30050;
        internal const int UIA_RangeValueLargeChangePropertyId = 30051;
        internal const int UIA_RangeValueSmallChangePropertyId = 30052;
        internal const int UIA_ScrollHorizontalScrollPercentPropertyId = 30053;
        internal const int UIA_ScrollHorizontalViewSizePropertyId = 30054;
        internal const int UIA_ScrollVerticalScrollPercentPropertyId = 30055;
        internal const int UIA_ScrollVerticalViewSizePropertyId = 30056;
        internal const int UIA_ScrollHorizontallyScrollablePropertyId = 30057;
        internal const int UIA_ScrollVerticallyScrollablePropertyId = 30058;
        internal const int UIA_SelectionSelectionPropertyId = 30059;
        internal const int UIA_SelectionCanSelectMultiplePropertyId = 30060;
        internal const int UIA_SelectionIsSelectionRequiredPropertyId = 30061;
        internal const int UIA_GridRowCountPropertyId = 30062;
        internal const int UIA_GridColumnCountPropertyId = 30063;
        internal const int UIA_GridItemRowPropertyId = 30064;
        internal const int UIA_GridItemColumnPropertyId = 30065;
        internal const int UIA_GridItemRowSpanPropertyId = 30066;
        internal const int UIA_GridItemColumnSpanPropertyId = 30067;
        internal const int UIA_GridItemContainingGridPropertyId = 30068;
        internal const int UIA_DockDockPositionPropertyId = 30069;
        internal const int UIA_ExpandCollapseExpandCollapseStatePropertyId = 30070;
        internal const int UIA_MultipleViewCurrentViewPropertyId = 30071;
        internal const int UIA_MultipleViewSupportedViewsPropertyId = 30072;
        internal const int UIA_WindowCanMaximizePropertyId = 30073;
        internal const int UIA_WindowCanMinimizePropertyId = 30074;
        internal const int UIA_WindowWindowVisualStatePropertyId = 30075;
        internal const int UIA_WindowWindowInteractionStatePropertyId = 30076;
        internal const int UIA_WindowIsModalPropertyId = 30077;
        internal const int UIA_WindowIsTopmostPropertyId = 30078;
        internal const int UIA_SelectionItemIsSelectedPropertyId = 30079;
        internal const int UIA_SelectionItemSelectionContainerPropertyId = 30080;
        internal const int UIA_TableRowHeadersPropertyId = 30081;
        internal const int UIA_TableColumnHeadersPropertyId = 30082;
        internal const int UIA_TableRowOrColumnMajorPropertyId = 30083;
        internal const int UIA_TableItemRowHeaderItemsPropertyId = 30084;
        internal const int UIA_TableItemColumnHeaderItemsPropertyId = 30085;
        internal const int UIA_ToggleToggleStatePropertyId = 30086;
        internal const int UIA_TransformCanMovePropertyId = 30087;
        internal const int UIA_TransformCanResizePropertyId = 30088;
        internal const int UIA_TransformCanRotatePropertyId = 30089;
        internal const int UIA_IsLegacyIAccessiblePatternAvailablePropertyId = 30090;
        internal const int UIA_LegacyIAccessibleChildIdPropertyId = 30091;
        internal const int UIA_LegacyIAccessibleNamePropertyId = 30092;
        internal const int UIA_LegacyIAccessibleValuePropertyId = 30093;
        internal const int UIA_LegacyIAccessibleDescriptionPropertyId = 30094;
        internal const int UIA_LegacyIAccessibleRolePropertyId = 30095;
        internal const int UIA_LegacyIAccessibleStatePropertyId = 30096;
        internal const int UIA_LegacyIAccessibleHelpPropertyId = 30097;
        internal const int UIA_LegacyIAccessibleKeyboardShortcutPropertyId = 30098;
        internal const int UIA_LegacyIAccessibleSelectionPropertyId = 30099;
        internal const int UIA_LegacyIAccessibleDefaultActionPropertyId = 30100;
        internal const int UIA_AriaRolePropertyId = 30101;
        internal const int UIA_AriaPropertiesPropertyId = 30102;
        internal const int UIA_IsDataValidForFormPropertyId = 30103;
        internal const int UIA_ControllerForPropertyId = 30104;
        internal const int UIA_DescribedByPropertyId = 30105;
        internal const int UIA_FlowsToPropertyId = 30106;
        internal const int UIA_ProviderDescriptionPropertyId = 30107;
        internal const int UIA_IsItemContainerPatternAvailablePropertyId = 30108;
        internal const int UIA_IsVirtualizedItemPatternAvailablePropertyId = 30109;
        internal const int UIA_IsSynchronizedInputPatternAvailablePropertyId = 30110;
        internal const int UIA_OptimizeForVisualContentPropertyId = 30111;
        internal const int UIA_IsObjectModelPatternAvailablePropertyId = 30112;
        internal const int UIA_AnnotationAnnotationTypeIdPropertyId = 30113;
        internal const int UIA_AnnotationAnnotationTypeNamePropertyId = 30114;
        internal const int UIA_AnnotationAuthorPropertyId = 30115;
        internal const int UIA_AnnotationDateTimePropertyId = 30116;
        internal const int UIA_AnnotationTargetPropertyId = 30117;
        internal const int UIA_IsAnnotationPatternAvailablePropertyId = 30118;
        internal const int UIA_IsTextPattern2AvailablePropertyId = 30119;
        internal const int UIA_StylesStyleIdPropertyId = 30120;
        internal const int UIA_StylesStyleNamePropertyId = 30121;
        internal const int UIA_StylesFillColorPropertyId = 30122;
        internal const int UIA_StylesFillPatternStylePropertyId = 30123;
        internal const int UIA_StylesShapePropertyId = 30124;
        internal const int UIA_StylesFillPatternColorPropertyId = 30125;
        internal const int UIA_StylesExtendedPropertiesPropertyId = 30126;
        internal const int UIA_IsStylesPatternAvailablePropertyId = 30127;
        internal const int UIA_IsSpreadsheetPatternAvailablePropertyId = 30128;
        internal const int UIA_SpreadsheetItemFormulaPropertyId = 30129;
        internal const int UIA_SpreadsheetItemAnnotationObjectsPropertyId = 30130;
        internal const int UIA_SpreadsheetItemAnnotationTypesPropertyId = 30131;
        internal const int UIA_IsSpreadsheetItemPatternAvailablePropertyId = 30132;
        internal const int UIA_Transform2CanZoomPropertyId = 30133;
        internal const int UIA_IsTransformPattern2AvailablePropertyId = 30134;
        internal const int UIA_LiveSettingPropertyId = 30135;
        internal const int UIA_IsTextChildPatternAvailablePropertyId = 30136;
        internal const int UIA_IsDragPatternAvailablePropertyId = 30137;
        internal const int UIA_DragIsGrabbedPropertyId = 30138;
        internal const int UIA_DragDropEffectPropertyId = 30139;
        internal const int UIA_DragDropEffectsPropertyId = 30140;
        internal const int UIA_IsDropTargetPatternAvailablePropertyId = 30141;
        internal const int UIA_DropTargetDropTargetEffectPropertyId = 30142;
        internal const int UIA_DropTargetDropTargetEffectsPropertyId = 30143;
        internal const int UIA_DragGrabbedItemsPropertyId = 30144;
        internal const int UIA_Transform2ZoomLevelPropertyId = 30145;
        internal const int UIA_Transform2ZoomMinimumPropertyId = 30146;
        internal const int UIA_Transform2ZoomMaximumPropertyId = 30147;
        internal const int UIA_FlowsFromPropertyId = 30148;
        internal const int UIA_IsTextEditPatternAvailablePropertyId = 30149;
        internal const int UIA_IsPeripheralPropertyId = 30150;
        internal const int UIA_IsCustomNavigationPatternAvailablePropertyId = 30151;
        internal const int UIA_PositionInSetPropertyId = 30152;
        internal const int UIA_SizeOfSetPropertyId = 30153;
        internal const int UIA_LevelPropertyId = 30154;
        internal const int UIA_AnnotationTypesPropertyId = 30155;
        internal const int UIA_AnnotationObjectsPropertyId = 30156;
        internal const int UIA_LandmarkTypePropertyId = 30157;
        internal const int UIA_LocalizedLandmarkTypePropertyId = 30158;
        internal const int UIA_FullDescriptionPropertyId = 30159;
        internal const int UIA_FillColorPropertyId = 30160;
        internal const int UIA_OutlineColorPropertyId = 30161;
        internal const int UIA_FillTypePropertyId = 30162;
        internal const int UIA_VisualEffectsPropertyId = 30163;
        internal const int UIA_OutlineThicknessPropertyId = 30164;
        internal const int UIA_CenterPointPropertyId = 30165;
        internal const int UIA_RotationPropertyId = 30166;
        internal const int UIA_SizePropertyId = 30167;

        // UIA_ControlTypeIds
        internal const int UIA_ButtonControlTypeId = 50000;
        internal const int UIA_CalendarControlTypeId = 50001;
        internal const int UIA_CheckBoxControlTypeId = 50002;
        internal const int UIA_ComboBoxControlTypeId = 50003;
        internal const int UIA_EditControlTypeId = 50004;
        internal const int UIA_HyperlinkControlTypeId = 50005;
        internal const int UIA_ImageControlTypeId = 50006;
        internal const int UIA_ListItemControlTypeId = 50007;
        internal const int UIA_ListControlTypeId = 50008;
        internal const int UIA_MenuControlTypeId = 50009;
        internal const int UIA_MenuBarControlTypeId = 50010;
        internal const int UIA_MenuItemControlTypeId = 50011;
        internal const int UIA_ProgressBarControlTypeId = 50012;
        internal const int UIA_RadioButtonControlTypeId = 50013;
        internal const int UIA_ScrollBarControlTypeId = 50014;
        internal const int UIA_SliderControlTypeId = 50015;
        internal const int UIA_SpinnerControlTypeId = 50016;
        internal const int UIA_StatusBarControlTypeId = 50017;
        internal const int UIA_TabControlTypeId = 50018;
        internal const int UIA_TabItemControlTypeId = 50019;
        internal const int UIA_TextControlTypeId = 50020;
        internal const int UIA_ToolBarControlTypeId = 50021;
        internal const int UIA_ToolTipControlTypeId = 50022;
        internal const int UIA_TreeControlTypeId = 50023;
        internal const int UIA_TreeItemControlTypeId = 50024;
        internal const int UIA_CustomControlTypeId = 50025;
        internal const int UIA_GroupControlTypeId = 50026;
        internal const int UIA_ThumbControlTypeId = 50027;
        internal const int UIA_DataGridControlTypeId = 50028;
        internal const int UIA_DataItemControlTypeId = 50029;
        internal const int UIA_DocumentControlTypeId = 50030;
        internal const int UIA_SplitButtonControlTypeId = 50031;
        internal const int UIA_WindowControlTypeId = 50032;
        internal const int UIA_PaneControlTypeId = 50033;
        internal const int UIA_HeaderControlTypeId = 50034;
        internal const int UIA_HeaderItemControlTypeId = 50035;
        internal const int UIA_TableControlTypeId = 50036;
        internal const int UIA_TitleBarControlTypeId = 50037;
        internal const int UIA_SeparatorControlTypeId = 50038;
        internal const int UIA_SemanticZoomControlTypeId = 50039;
        internal const int UIA_AppBarControlTypeId = 50040;

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref Interop.RECT rect, int cPoints);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref Point pt, uint cPoints);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
        public extern static IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [In, Out] TV_HITTESTINFO lParam);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int keyCode);

        [DllImport(ExternDll.User32, ExactSpelling = true)]
        public static extern bool GetUpdateRect(IntPtr hwnd, ref Interop.RECT rc, bool fErase);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetCursor();

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);
    }
}


