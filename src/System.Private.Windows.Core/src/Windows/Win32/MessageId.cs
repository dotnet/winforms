// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

/// <summary>
///  Simple internal wrapper that enables showing the message identifier string in the debugger.
/// </summary>
internal readonly struct MessageId
{
    private readonly uint _id;

    private MessageId(uint id) => _id = id;

    public static explicit operator int(MessageId id) => (int)id._id;
    public static explicit operator MessageId(int id) => new((uint)id);
    public static implicit operator uint(MessageId id) => id._id;
    public static implicit operator MessageId(uint id) => new(id);

    public override string ToString() => MessageIdToString() ?? $"Id: {_id} (0x{_id:X8})";

    public const uint WM_REFLECT = PInvokeCore.WM_USER + 0x1C00;
    public const uint WM_REFLECT_NOTIFY = WM_REFLECT + PInvokeCore.WM_NOTIFY;
    public const uint WM_REFLECT_NOTIFYFORMAT = WM_REFLECT + PInvokeCore.WM_NOTIFYFORMAT;
    public const uint WM_REFLECT_COMMAND = WM_REFLECT + PInvokeCore.WM_COMMAND;
    public const uint WM_REFLECT_CHARTOITEM = WM_REFLECT + PInvokeCore.WM_CHARTOITEM;
    public const uint WM_REFLECT_VKEYTOITEM = WM_REFLECT + PInvokeCore.WM_VKEYTOITEM;
    public const uint WM_REFLECT_DRAWITEM = WM_REFLECT + PInvokeCore.WM_DRAWITEM;
    public const uint WM_REFLECT_MEASUREITEM = WM_REFLECT + PInvokeCore.WM_MEASUREITEM;
    public const uint WM_REFLECT_HSCROLL = WM_REFLECT + PInvokeCore.WM_HSCROLL;
    public const uint WM_REFLECT_VSCROLL = WM_REFLECT + PInvokeCore.WM_VSCROLL;
    public const uint WM_REFLECT_CTLCOLOR = WM_REFLECT + PInvokeCore.WM_CTLCOLOR;
    public const uint WM_REFLECT_CTLCOLORBTN = WM_REFLECT + PInvokeCore.WM_CTLCOLORBTN;
    public const uint WM_REFLECT_CTLCOLORDLG = WM_REFLECT + PInvokeCore.WM_CTLCOLORDLG;
    public const uint WM_REFLECT_CTLCOLORMSGBOX = WM_REFLECT + PInvokeCore.WM_CTLCOLORMSGBOX;
    public const uint WM_REFLECT_CTLCOLORSCROLLBAR = WM_REFLECT + PInvokeCore.WM_CTLCOLORSCROLLBAR;
    public const uint WM_REFLECT_CTLCOLOREDIT = WM_REFLECT + PInvokeCore.WM_CTLCOLOREDIT;
    public const uint WM_REFLECT_CTLCOLORLISTBOX = WM_REFLECT + PInvokeCore.WM_CTLCOLORLISTBOX;
    public const uint WM_REFLECT_CTLCOLORSTATIC = WM_REFLECT + PInvokeCore.WM_CTLCOLORSTATIC;

    /// <summary>
    ///  Returns the symbolic name of the message value, or null if it isn't one of the existing constants.
    /// </summary>
    internal string? MessageIdToString()
    {
        string? text = _id switch
        {
            PInvokeCore.WM_NULL => "WM_NULL",
            PInvokeCore.WM_CREATE => "WM_CREATE",
            PInvokeCore.WM_DESTROY => "WM_DESTROY",
            PInvokeCore.WM_MOVE => "WM_MOVE",
            PInvokeCore.WM_SIZE => "WM_SIZE",
            PInvokeCore.WM_ACTIVATE => "WM_ACTIVATE",
            PInvokeCore.WM_SETFOCUS => "WM_SETFOCUS",
            PInvokeCore.WM_KILLFOCUS => "WM_KILLFOCUS",
            PInvokeCore.WM_ENABLE => "WM_ENABLE",
            PInvokeCore.WM_SETREDRAW => "WM_SETREDRAW",
            PInvokeCore.WM_SETTEXT => "WM_SETTEXT",
            PInvokeCore.WM_GETTEXT => "WM_GETTEXT",
            PInvokeCore.WM_GETTEXTLENGTH => "WM_GETTEXTLENGTH",
            PInvokeCore.WM_PAINT => "WM_PAINT",
            PInvokeCore.WM_CLOSE => "WM_CLOSE",
            PInvokeCore.WM_QUERYENDSESSION => "WM_QUERYENDSESSION",
            PInvokeCore.WM_QUIT => "WM_QUIT",
            PInvokeCore.WM_QUERYOPEN => "WM_QUERYOPEN",
            PInvokeCore.WM_ERASEBKGND => "WM_ERASEBKGND",
            PInvokeCore.WM_SYSCOLORCHANGE => "WM_SYSCOLORCHANGE",
            PInvokeCore.WM_ENDSESSION => "WM_ENDSESSION",
            PInvokeCore.WM_SHOWWINDOW => "WM_SHOWWINDOW",
            PInvokeCore.WM_WININICHANGE => "WM_WININICHANGE",
            PInvokeCore.WM_DEVMODECHANGE => "WM_DEVMODECHANGE",
            PInvokeCore.WM_ACTIVATEAPP => "WM_ACTIVATEAPP",
            PInvokeCore.WM_FONTCHANGE => "WM_FONTCHANGE",
            PInvokeCore.WM_TIMECHANGE => "WM_TIMECHANGE",
            PInvokeCore.WM_CANCELMODE => "WM_CANCELMODE",
            PInvokeCore.WM_SETCURSOR => "WM_SETCURSOR",
            PInvokeCore.WM_MOUSEACTIVATE => "WM_MOUSEACTIVATE",
            PInvokeCore.WM_CHILDACTIVATE => "WM_CHILDACTIVATE",
            PInvokeCore.WM_QUEUESYNC => "WM_QUEUESYNC",
            PInvokeCore.WM_GETMINMAXINFO => "WM_GETMINMAXINFO",
            PInvokeCore.WM_PAINTICON => "WM_PAINTICON",
            PInvokeCore.WM_ICONERASEBKGND => "WM_ICONERASEBKGND",
            PInvokeCore.WM_NEXTDLGCTL => "WM_NEXTDLGCTL",
            PInvokeCore.WM_SPOOLERSTATUS => "WM_SPOOLERSTATUS",
            PInvokeCore.WM_DRAWITEM => "WM_DRAWITEM",
            PInvokeCore.WM_MEASUREITEM => "WM_MEASUREITEM",
            PInvokeCore.WM_DELETEITEM => "WM_DELETEITEM",
            PInvokeCore.WM_VKEYTOITEM => "WM_VKEYTOITEM",
            PInvokeCore.WM_CHARTOITEM => "WM_CHARTOITEM",
            PInvokeCore.WM_SETFONT => "WM_SETFONT",
            PInvokeCore.WM_GETFONT => "WM_GETFONT",
            PInvokeCore.WM_SETHOTKEY => "WM_SETHOTKEY",
            PInvokeCore.WM_GETHOTKEY => "WM_GETHOTKEY",
            PInvokeCore.WM_QUERYDRAGICON => "WM_QUERYDRAGICON",
            PInvokeCore.WM_COMPAREITEM => "WM_COMPAREITEM",
            PInvokeCore.WM_GETOBJECT => "WM_GETOBJECT",
            PInvokeCore.WM_COMPACTING => "WM_COMPACTING",
            PInvokeCore.WM_COMMNOTIFY => "WM_COMMNOTIFY",
            PInvokeCore.WM_WINDOWPOSCHANGING => "WM_WINDOWPOSCHANGING",
            PInvokeCore.WM_WINDOWPOSCHANGED => "WM_WINDOWPOSCHANGED",
            PInvokeCore.WM_POWER => "WM_POWER",
            PInvokeCore.WM_COPYDATA => "WM_COPYDATA",
            PInvokeCore.WM_CANCELJOURNAL => "WM_CANCELJOURNAL",
            PInvokeCore.WM_NOTIFY => "WM_NOTIFY",
            PInvokeCore.WM_INPUTLANGCHANGEREQUEST => "WM_INPUTLANGCHANGEREQUEST",
            PInvokeCore.WM_INPUTLANGCHANGE => "WM_INPUTLANGCHANGE",
            PInvokeCore.WM_TCARD => "WM_TCARD",
            PInvokeCore.WM_HELP => "WM_HELP",
            PInvokeCore.WM_USERCHANGED => "WM_USERCHANGED",
            PInvokeCore.WM_NOTIFYFORMAT => "WM_NOTIFYFORMAT",
            PInvokeCore.WM_CONTEXTMENU => "WM_CONTEXTMENU",
            PInvokeCore.WM_STYLECHANGING => "WM_STYLECHANGING",
            PInvokeCore.WM_STYLECHANGED => "WM_STYLECHANGED",
            PInvokeCore.WM_DISPLAYCHANGE => "WM_DISPLAYCHANGE",
            PInvokeCore.WM_GETICON => "WM_GETICON",
            PInvokeCore.WM_SETICON => "WM_SETICON",
            PInvokeCore.WM_NCCREATE => "WM_NCCREATE",
            PInvokeCore.WM_NCDESTROY => "WM_NCDESTROY",
            PInvokeCore.WM_NCCALCSIZE => "WM_NCCALCSIZE",
            PInvokeCore.WM_NCHITTEST => "WM_NCHITTEST",
            PInvokeCore.WM_NCPAINT => "WM_NCPAINT",
            PInvokeCore.WM_NCACTIVATE => "WM_NCACTIVATE",
            PInvokeCore.WM_GETDLGCODE => "WM_GETDLGCODE",
            PInvokeCore.WM_NCMOUSEMOVE => "WM_NCMOUSEMOVE",
            PInvokeCore.WM_NCLBUTTONDOWN => "WM_NCLBUTTONDOWN",
            PInvokeCore.WM_NCLBUTTONUP => "WM_NCLBUTTONUP",
            PInvokeCore.WM_NCLBUTTONDBLCLK => "WM_NCLBUTTONDBLCLK",
            PInvokeCore.WM_NCRBUTTONDOWN => "WM_NCRBUTTONDOWN",
            PInvokeCore.WM_NCRBUTTONUP => "WM_NCRBUTTONUP",
            PInvokeCore.WM_NCRBUTTONDBLCLK => "WM_NCRBUTTONDBLCLK",
            PInvokeCore.WM_NCMBUTTONDOWN => "WM_NCMBUTTONDOWN",
            PInvokeCore.WM_NCMBUTTONUP => "WM_NCMBUTTONUP",
            PInvokeCore.WM_NCMBUTTONDBLCLK => "WM_NCMBUTTONDBLCLK",
            PInvokeCore.WM_KEYDOWN => "WM_KEYDOWN",
            PInvokeCore.WM_KEYUP => "WM_KEYUP",
            PInvokeCore.WM_CHAR => "WM_CHAR",
            PInvokeCore.WM_DEADCHAR => "WM_DEADCHAR",
            PInvokeCore.WM_SYSKEYDOWN => "WM_SYSKEYDOWN",
            PInvokeCore.WM_SYSKEYUP => "WM_SYSKEYUP",
            PInvokeCore.WM_SYSCHAR => "WM_SYSCHAR",
            PInvokeCore.WM_SYSDEADCHAR => "WM_SYSDEADCHAR",
            PInvokeCore.WM_KEYLAST => "WM_KEYLAST",
            PInvokeCore.WM_IME_STARTCOMPOSITION => "WM_IME_STARTCOMPOSITION",
            PInvokeCore.WM_IME_ENDCOMPOSITION => "WM_IME_ENDCOMPOSITION",
            PInvokeCore.WM_IME_COMPOSITION => "WM_IME_COMPOSITION",
            PInvokeCore.WM_INITDIALOG => "WM_INITDIALOG",
            PInvokeCore.WM_COMMAND => "WM_COMMAND",
            PInvokeCore.WM_SYSCOMMAND => "WM_SYSCOMMAND",
            PInvokeCore.WM_TIMER => "WM_TIMER",
            PInvokeCore.WM_HSCROLL => "WM_HSCROLL",
            PInvokeCore.WM_VSCROLL => "WM_VSCROLL",
            PInvokeCore.WM_INITMENU => "WM_INITMENU",
            PInvokeCore.WM_INITMENUPOPUP => "WM_INITMENUPOPUP",
            PInvokeCore.WM_MENUSELECT => "WM_MENUSELECT",
            PInvokeCore.WM_MENUCHAR => "WM_MENUCHAR",
            PInvokeCore.WM_ENTERIDLE => "WM_ENTERIDLE",
            PInvokeCore.WM_CTLCOLORMSGBOX => "WM_CTLCOLORMSGBOX",
            PInvokeCore.WM_CTLCOLOREDIT => "WM_CTLCOLOREDIT",
            PInvokeCore.WM_CTLCOLORLISTBOX => "WM_CTLCOLORLISTBOX",
            PInvokeCore.WM_CTLCOLORBTN => "WM_CTLCOLORBTN",
            PInvokeCore.WM_CTLCOLORDLG => "WM_CTLCOLORDLG",
            PInvokeCore.WM_CTLCOLORSCROLLBAR => "WM_CTLCOLORSCROLLBAR",
            PInvokeCore.WM_CTLCOLORSTATIC => "WM_CTLCOLORSTATIC",
            PInvokeCore.WM_MOUSEMOVE => "WM_MOUSEMOVE",
            PInvokeCore.WM_LBUTTONDOWN => "WM_LBUTTONDOWN",
            PInvokeCore.WM_LBUTTONUP => "WM_LBUTTONUP",
            PInvokeCore.WM_LBUTTONDBLCLK => "WM_LBUTTONDBLCLK",
            PInvokeCore.WM_RBUTTONDOWN => "WM_RBUTTONDOWN",
            PInvokeCore.WM_RBUTTONUP => "WM_RBUTTONUP",
            PInvokeCore.WM_RBUTTONDBLCLK => "WM_RBUTTONDBLCLK",
            PInvokeCore.WM_MBUTTONDOWN => "WM_MBUTTONDOWN",
            PInvokeCore.WM_MBUTTONUP => "WM_MBUTTONUP",
            PInvokeCore.WM_MBUTTONDBLCLK => "WM_MBUTTONDBLCLK",
            PInvokeCore.WM_MOUSEWHEEL => "WM_MOUSEWHEEL",
            PInvokeCore.WM_PARENTNOTIFY => "WM_PARENTNOTIFY",
            PInvokeCore.WM_ENTERMENULOOP => "WM_ENTERMENULOOP",
            PInvokeCore.WM_EXITMENULOOP => "WM_EXITMENULOOP",
            PInvokeCore.WM_NEXTMENU => "WM_NEXTMENU",
            PInvokeCore.WM_SIZING => "WM_SIZING",
            PInvokeCore.WM_CAPTURECHANGED => "WM_CAPTURECHANGED",
            PInvokeCore.WM_MOVING => "WM_MOVING",
            PInvokeCore.WM_POWERBROADCAST => "WM_POWERBROADCAST",
            PInvokeCore.WM_DEVICECHANGE => "WM_DEVICECHANGE",
            PInvokeCore.WM_IME_SETCONTEXT => "WM_IME_SETCONTEXT",
            PInvokeCore.WM_IME_NOTIFY => "WM_IME_NOTIFY",
            PInvokeCore.WM_IME_CONTROL => "WM_IME_CONTROL",
            PInvokeCore.WM_IME_COMPOSITIONFULL => "WM_IME_COMPOSITIONFULL",
            PInvokeCore.WM_IME_SELECT => "WM_IME_SELECT",
            PInvokeCore.WM_IME_CHAR => "WM_IME_CHAR",
            PInvokeCore.WM_IME_KEYDOWN => "WM_IME_KEYDOWN",
            PInvokeCore.WM_IME_KEYUP => "WM_IME_KEYUP",
            PInvokeCore.WM_MDICREATE => "WM_MDICREATE",
            PInvokeCore.WM_MDIDESTROY => "WM_MDIDESTROY",
            PInvokeCore.WM_MDIACTIVATE => "WM_MDIACTIVATE",
            PInvokeCore.WM_MDIRESTORE => "WM_MDIRESTORE",
            PInvokeCore.WM_MDINEXT => "WM_MDINEXT",
            PInvokeCore.WM_MDIMAXIMIZE => "WM_MDIMAXIMIZE",
            PInvokeCore.WM_MDITILE => "WM_MDITILE",
            PInvokeCore.WM_MDICASCADE => "WM_MDICASCADE",
            PInvokeCore.WM_MDIICONARRANGE => "WM_MDIICONARRANGE",
            PInvokeCore.WM_MDIGETACTIVE => "WM_MDIGETACTIVE",
            PInvokeCore.WM_MDISETMENU => "WM_MDISETMENU",
            PInvokeCore.WM_ENTERSIZEMOVE => "WM_ENTERSIZEMOVE",
            PInvokeCore.WM_EXITSIZEMOVE => "WM_EXITSIZEMOVE",
            PInvokeCore.WM_DROPFILES => "WM_DROPFILES",
            PInvokeCore.WM_MDIREFRESHMENU => "WM_MDIREFRESHMENU",
            PInvokeCore.WM_MOUSEHOVER => "WM_MOUSEHOVER",
            PInvokeCore.WM_MOUSELEAVE => "WM_MOUSELEAVE",
            PInvokeCore.WM_CUT => "WM_CUT",
            PInvokeCore.WM_COPY => "WM_COPY",
            PInvokeCore.WM_PASTE => "WM_PASTE",
            PInvokeCore.WM_CLEAR => "WM_CLEAR",
            PInvokeCore.WM_UNDO => "WM_UNDO",
            PInvokeCore.WM_RENDERFORMAT => "WM_RENDERFORMAT",
            PInvokeCore.WM_RENDERALLFORMATS => "WM_RENDERALLFORMATS",
            PInvokeCore.WM_DESTROYCLIPBOARD => "WM_DESTROYCLIPBOARD",
            PInvokeCore.WM_DRAWCLIPBOARD => "WM_DRAWCLIPBOARD",
            PInvokeCore.WM_PAINTCLIPBOARD => "WM_PAINTCLIPBOARD",
            PInvokeCore.WM_VSCROLLCLIPBOARD => "WM_VSCROLLCLIPBOARD",
            PInvokeCore.WM_SIZECLIPBOARD => "WM_SIZECLIPBOARD",
            PInvokeCore.WM_ASKCBFORMATNAME => "WM_ASKCBFORMATNAME",
            PInvokeCore.WM_CHANGECBCHAIN => "WM_CHANGECBCHAIN",
            PInvokeCore.WM_HSCROLLCLIPBOARD => "WM_HSCROLLCLIPBOARD",
            PInvokeCore.WM_QUERYNEWPALETTE => "WM_QUERYNEWPALETTE",
            PInvokeCore.WM_PALETTEISCHANGING => "WM_PALETTEISCHANGING",
            PInvokeCore.WM_PALETTECHANGED => "WM_PALETTECHANGED",
            PInvokeCore.WM_HOTKEY => "WM_HOTKEY",
            PInvokeCore.WM_PRINT => "WM_PRINT",
            PInvokeCore.WM_PRINTCLIENT => "WM_PRINTCLIENT",
            PInvokeCore.WM_HANDHELDFIRST => "WM_HANDHELDFIRST",
            PInvokeCore.WM_HANDHELDLAST => "WM_HANDHELDLAST",
            PInvokeCore.WM_AFXFIRST => "WM_AFXFIRST",
            PInvokeCore.WM_AFXLAST => "WM_AFXLAST",
            PInvokeCore.WM_PENWINFIRST => "WM_PENWINFIRST",
            PInvokeCore.WM_PENWINLAST => "WM_PENWINLAST",
            PInvokeCore.WM_APP => "WM_APP",
            PInvokeCore.WM_USER => "WM_USER",
            PInvokeCore.WM_CTLCOLOR => "WM_CTLCOLOR",

            // RichEdit messages
            PInvokeCore.EM_GETLIMITTEXT => "EM_GETLIMITTEXT",
            PInvokeCore.EM_POSFROMCHAR => "EM_POSFROMCHAR",
            PInvokeCore.EM_CHARFROMPOS => "EM_CHARFROMPOS",
            PInvokeCore.EM_SCROLLCARET => "EM_SCROLLCARET",
            PInvokeCore.EM_CANPASTE => "EM_CANPASTE",
            PInvokeCore.EM_DISPLAYBAND => "EM_DISPLAYBAND",
            PInvokeCore.EM_EXGETSEL => "EM_EXGETSEL",
            PInvokeCore.EM_EXLIMITTEXT => "EM_EXLIMITTEXT",
            PInvokeCore.EM_EXLINEFROMCHAR => "EM_EXLINEFROMCHAR",
            PInvokeCore.EM_EXSETSEL => "EM_EXSETSEL",
            PInvokeCore.EM_FINDTEXT => "EM_FINDTEXT",
            PInvokeCore.EM_FORMATRANGE => "EM_FORMATRANGE",
            PInvokeCore.EM_GETCHARFORMAT => "EM_GETCHARFORMAT",
            PInvokeCore.EM_GETEVENTMASK => "EM_GETEVENTMASK",
            PInvokeCore.EM_GETOLEINTERFACE => "EM_GETOLEINTERFACE",
            PInvokeCore.EM_GETPARAFORMAT => "EM_GETPARAFORMAT",
            PInvokeCore.EM_GETSELTEXT => "EM_GETSELTEXT",
            PInvokeCore.EM_HIDESELECTION => "EM_HIDESELECTION",
            PInvokeCore.EM_PASTESPECIAL => "EM_PASTESPECIAL",
            PInvokeCore.EM_REQUESTRESIZE => "EM_REQUESTRESIZE",
            PInvokeCore.EM_SELECTIONTYPE => "EM_SELECTIONTYPE",
            PInvokeCore.EM_SETBKGNDCOLOR => "EM_SETBKGNDCOLOR",
            PInvokeCore.EM_SETCHARFORMAT => "EM_SETCHARFORMAT",
            PInvokeCore.EM_SETEVENTMASK => "EM_SETEVENTMASK",
            PInvokeCore.EM_SETOLECALLBACK => "EM_SETOLECALLBACK",
            PInvokeCore.EM_SETPARAFORMAT => "EM_SETPARAFORMAT",
            PInvokeCore.EM_SETTARGETDEVICE => "EM_SETTARGETDEVICE",
            PInvokeCore.EM_STREAMIN => "EM_STREAMIN",
            PInvokeCore.EM_STREAMOUT => "EM_STREAMOUT",
            PInvokeCore.EM_GETTEXTRANGE => "EM_GETTEXTRANGE",
            PInvokeCore.EM_FINDWORDBREAK => "EM_FINDWORDBREAK",
            PInvokeCore.EM_SETOPTIONS => "EM_SETOPTIONS",
            PInvokeCore.EM_GETOPTIONS => "EM_GETOPTIONS",
            PInvokeCore.EM_FINDTEXTEX => "EM_FINDTEXTEX",
            PInvokeCore.EM_GETWORDBREAKPROCEX => "EM_GETWORDBREAKPROCEX",
            PInvokeCore.EM_SETWORDBREAKPROCEX => "EM_SETWORDBREAKPROCEX",

            // Richedit v2.0 messages
            PInvokeCore.EM_SETUNDOLIMIT => "EM_SETUNDOLIMIT",
            PInvokeCore.EM_REDO => "EM_REDO",
            PInvokeCore.EM_CANREDO => "EM_CANREDO",
            PInvokeCore.EM_GETUNDONAME => "EM_GETUNDONAME",
            PInvokeCore.EM_GETREDONAME => "EM_GETREDONAME",
            PInvokeCore.EM_STOPGROUPTYPING => "EM_STOPGROUPTYPING",
            PInvokeCore.EM_SETTEXTMODE => "EM_SETTEXTMODE",
            PInvokeCore.EM_GETTEXTMODE => "EM_GETTEXTMODE",
            PInvokeCore.EM_AUTOURLDETECT => "EM_AUTOURLDETECT",
            PInvokeCore.EM_GETAUTOURLDETECT => "EM_GETAUTOURLDETECT",
            PInvokeCore.EM_SETPALETTE => "EM_SETPALETTE",
            PInvokeCore.EM_GETTEXTEX => "EM_GETTEXTEX",
            PInvokeCore.EM_GETTEXTLENGTHEX => "EM_GETTEXTLENGTHEX",

            // Asia specific messages
            PInvokeCore.EM_SETPUNCTUATION => "EM_SETPUNCTUATION",
            PInvokeCore.EM_GETPUNCTUATION => "EM_GETPUNCTUATION",
            PInvokeCore.EM_SETWORDWRAPMODE => "EM_SETWORDWRAPMODE",
            PInvokeCore.EM_GETWORDWRAPMODE => "EM_GETWORDWRAPMODE",
            PInvokeCore.EM_SETIMECOLOR => "EM_SETIMECOLOR",
            PInvokeCore.EM_GETIMECOLOR => "EM_GETIMECOLOR",
            PInvokeCore.EM_SETIMEOPTIONS => "EM_SETIMEOPTIONS",
            PInvokeCore.EM_GETIMEOPTIONS => "EM_GETIMEOPTIONS",
            PInvokeCore.EM_CONVPOSITION => "EM_CONVPOSITION",
            PInvokeCore.EM_SETLANGOPTIONS => "EM_SETLANGOPTIONS",
            PInvokeCore.EM_GETLANGOPTIONS => "EM_GETLANGOPTIONS",
            PInvokeCore.EM_GETIMECOMPMODE => "EM_GETIMECOMPMODE",
            PInvokeCore.EM_FINDTEXTW => "EM_FINDTEXTW",
            PInvokeCore.EM_FINDTEXTEXW => "EM_FINDTEXTEXW",

            // Rich Edit 3.0 Asia messages
            PInvokeCore.EM_RECONVERSION => "EM_RECONVERSION",
            PInvokeCore.EM_SETIMEMODEBIAS => "EM_SETIMEMODEBIAS",
            PInvokeCore.EM_GETIMEMODEBIAS => "EM_GETIMEMODEBIAS",

            // BiDi Specific messages
            PInvokeCore.EM_SETBIDIOPTIONS => "EM_SETBIDIOPTIONS",
            PInvokeCore.EM_GETBIDIOPTIONS => "EM_GETBIDIOPTIONS",
            PInvokeCore.EM_SETTYPOGRAPHYOPTIONS => "EM_SETTYPOGRAPHYOPTIONS",
            PInvokeCore.EM_GETTYPOGRAPHYOPTIONS => "EM_GETTYPOGRAPHYOPTIONS",

            // Extended Edit style specific messages
            PInvokeCore.EM_SETEDITSTYLE => "EM_SETEDITSTYLE",
            PInvokeCore.EM_GETEDITSTYLE => "EM_GETEDITSTYLE",
            _ => null,
        };

        if (text is null && ((_id & WM_REFLECT) == WM_REFLECT))
        {
            string subtext = ((MessageId)(_id - WM_REFLECT)).MessageIdToString() ?? "???";

            text = $"WM_REFLECT + {subtext}";
        }

        return text;
    }
}
