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

    public const uint WM_REFLECT = PInvoke.WM_USER + 0x1C00;
    public const uint WM_REFLECT_NOTIFY = WM_REFLECT + PInvoke.WM_NOTIFY;
    public const uint WM_REFLECT_NOTIFYFORMAT = WM_REFLECT + PInvoke.WM_NOTIFYFORMAT;
    public const uint WM_REFLECT_COMMAND = WM_REFLECT + PInvoke.WM_COMMAND;
    public const uint WM_REFLECT_CHARTOITEM = WM_REFLECT + PInvoke.WM_CHARTOITEM;
    public const uint WM_REFLECT_VKEYTOITEM = WM_REFLECT + PInvoke.WM_VKEYTOITEM;
    public const uint WM_REFLECT_DRAWITEM = WM_REFLECT + PInvoke.WM_DRAWITEM;
    public const uint WM_REFLECT_MEASUREITEM = WM_REFLECT + PInvoke.WM_MEASUREITEM;
    public const uint WM_REFLECT_HSCROLL = WM_REFLECT + PInvoke.WM_HSCROLL;
    public const uint WM_REFLECT_VSCROLL = WM_REFLECT + PInvoke.WM_VSCROLL;
    public const uint WM_REFLECT_CTLCOLOR = WM_REFLECT + PInvoke.WM_CTLCOLOR;
    public const uint WM_REFLECT_CTLCOLORBTN = WM_REFLECT + PInvoke.WM_CTLCOLORBTN;
    public const uint WM_REFLECT_CTLCOLORDLG = WM_REFLECT + PInvoke.WM_CTLCOLORDLG;
    public const uint WM_REFLECT_CTLCOLORMSGBOX = WM_REFLECT + PInvoke.WM_CTLCOLORMSGBOX;
    public const uint WM_REFLECT_CTLCOLORSCROLLBAR = WM_REFLECT + PInvoke.WM_CTLCOLORSCROLLBAR;
    public const uint WM_REFLECT_CTLCOLOREDIT = WM_REFLECT + PInvoke.WM_CTLCOLOREDIT;
    public const uint WM_REFLECT_CTLCOLORLISTBOX = WM_REFLECT + PInvoke.WM_CTLCOLORLISTBOX;
    public const uint WM_REFLECT_CTLCOLORSTATIC = WM_REFLECT + PInvoke.WM_CTLCOLORSTATIC;

    /// <summary>
    ///  Returns the symbolic name of the message value, or null if it isn't one of the existing constants.
    /// </summary>
    internal string? MessageIdToString()
    {
        string? text = _id switch
        {
            PInvoke.WM_NULL => "WM_NULL",
            PInvoke.WM_CREATE => "WM_CREATE",
            PInvoke.WM_DESTROY => "WM_DESTROY",
            PInvoke.WM_MOVE => "WM_MOVE",
            PInvoke.WM_SIZE => "WM_SIZE",
            PInvoke.WM_ACTIVATE => "WM_ACTIVATE",
            PInvoke.WM_SETFOCUS => "WM_SETFOCUS",
            PInvoke.WM_KILLFOCUS => "WM_KILLFOCUS",
            PInvoke.WM_ENABLE => "WM_ENABLE",
            PInvoke.WM_SETREDRAW => "WM_SETREDRAW",
            PInvoke.WM_SETTEXT => "WM_SETTEXT",
            PInvoke.WM_GETTEXT => "WM_GETTEXT",
            PInvoke.WM_GETTEXTLENGTH => "WM_GETTEXTLENGTH",
            PInvoke.WM_PAINT => "WM_PAINT",
            PInvoke.WM_CLOSE => "WM_CLOSE",
            PInvoke.WM_QUERYENDSESSION => "WM_QUERYENDSESSION",
            PInvoke.WM_QUIT => "WM_QUIT",
            PInvoke.WM_QUERYOPEN => "WM_QUERYOPEN",
            PInvoke.WM_ERASEBKGND => "WM_ERASEBKGND",
            PInvoke.WM_SYSCOLORCHANGE => "WM_SYSCOLORCHANGE",
            PInvoke.WM_ENDSESSION => "WM_ENDSESSION",
            PInvoke.WM_SHOWWINDOW => "WM_SHOWWINDOW",
            PInvoke.WM_WININICHANGE => "WM_WININICHANGE",
            PInvoke.WM_DEVMODECHANGE => "WM_DEVMODECHANGE",
            PInvoke.WM_ACTIVATEAPP => "WM_ACTIVATEAPP",
            PInvoke.WM_FONTCHANGE => "WM_FONTCHANGE",
            PInvoke.WM_TIMECHANGE => "WM_TIMECHANGE",
            PInvoke.WM_CANCELMODE => "WM_CANCELMODE",
            PInvoke.WM_SETCURSOR => "WM_SETCURSOR",
            PInvoke.WM_MOUSEACTIVATE => "WM_MOUSEACTIVATE",
            PInvoke.WM_CHILDACTIVATE => "WM_CHILDACTIVATE",
            PInvoke.WM_QUEUESYNC => "WM_QUEUESYNC",
            PInvoke.WM_GETMINMAXINFO => "WM_GETMINMAXINFO",
            PInvoke.WM_PAINTICON => "WM_PAINTICON",
            PInvoke.WM_ICONERASEBKGND => "WM_ICONERASEBKGND",
            PInvoke.WM_NEXTDLGCTL => "WM_NEXTDLGCTL",
            PInvoke.WM_SPOOLERSTATUS => "WM_SPOOLERSTATUS",
            PInvoke.WM_DRAWITEM => "WM_DRAWITEM",
            PInvoke.WM_MEASUREITEM => "WM_MEASUREITEM",
            PInvoke.WM_DELETEITEM => "WM_DELETEITEM",
            PInvoke.WM_VKEYTOITEM => "WM_VKEYTOITEM",
            PInvoke.WM_CHARTOITEM => "WM_CHARTOITEM",
            PInvoke.WM_SETFONT => "WM_SETFONT",
            PInvoke.WM_GETFONT => "WM_GETFONT",
            PInvoke.WM_SETHOTKEY => "WM_SETHOTKEY",
            PInvoke.WM_GETHOTKEY => "WM_GETHOTKEY",
            PInvoke.WM_QUERYDRAGICON => "WM_QUERYDRAGICON",
            PInvoke.WM_COMPAREITEM => "WM_COMPAREITEM",
            PInvoke.WM_GETOBJECT => "WM_GETOBJECT",
            PInvoke.WM_COMPACTING => "WM_COMPACTING",
            PInvoke.WM_COMMNOTIFY => "WM_COMMNOTIFY",
            PInvoke.WM_WINDOWPOSCHANGING => "WM_WINDOWPOSCHANGING",
            PInvoke.WM_WINDOWPOSCHANGED => "WM_WINDOWPOSCHANGED",
            PInvoke.WM_POWER => "WM_POWER",
            PInvoke.WM_COPYDATA => "WM_COPYDATA",
            PInvoke.WM_CANCELJOURNAL => "WM_CANCELJOURNAL",
            PInvoke.WM_NOTIFY => "WM_NOTIFY",
            PInvoke.WM_INPUTLANGCHANGEREQUEST => "WM_INPUTLANGCHANGEREQUEST",
            PInvoke.WM_INPUTLANGCHANGE => "WM_INPUTLANGCHANGE",
            PInvoke.WM_TCARD => "WM_TCARD",
            PInvoke.WM_HELP => "WM_HELP",
            PInvoke.WM_USERCHANGED => "WM_USERCHANGED",
            PInvoke.WM_NOTIFYFORMAT => "WM_NOTIFYFORMAT",
            PInvoke.WM_CONTEXTMENU => "WM_CONTEXTMENU",
            PInvoke.WM_STYLECHANGING => "WM_STYLECHANGING",
            PInvoke.WM_STYLECHANGED => "WM_STYLECHANGED",
            PInvoke.WM_DISPLAYCHANGE => "WM_DISPLAYCHANGE",
            PInvoke.WM_GETICON => "WM_GETICON",
            PInvoke.WM_SETICON => "WM_SETICON",
            PInvoke.WM_NCCREATE => "WM_NCCREATE",
            PInvoke.WM_NCDESTROY => "WM_NCDESTROY",
            PInvoke.WM_NCCALCSIZE => "WM_NCCALCSIZE",
            PInvoke.WM_NCHITTEST => "WM_NCHITTEST",
            PInvoke.WM_NCPAINT => "WM_NCPAINT",
            PInvoke.WM_NCACTIVATE => "WM_NCACTIVATE",
            PInvoke.WM_GETDLGCODE => "WM_GETDLGCODE",
            PInvoke.WM_NCMOUSEMOVE => "WM_NCMOUSEMOVE",
            PInvoke.WM_NCLBUTTONDOWN => "WM_NCLBUTTONDOWN",
            PInvoke.WM_NCLBUTTONUP => "WM_NCLBUTTONUP",
            PInvoke.WM_NCLBUTTONDBLCLK => "WM_NCLBUTTONDBLCLK",
            PInvoke.WM_NCRBUTTONDOWN => "WM_NCRBUTTONDOWN",
            PInvoke.WM_NCRBUTTONUP => "WM_NCRBUTTONUP",
            PInvoke.WM_NCRBUTTONDBLCLK => "WM_NCRBUTTONDBLCLK",
            PInvoke.WM_NCMBUTTONDOWN => "WM_NCMBUTTONDOWN",
            PInvoke.WM_NCMBUTTONUP => "WM_NCMBUTTONUP",
            PInvoke.WM_NCMBUTTONDBLCLK => "WM_NCMBUTTONDBLCLK",
            PInvoke.WM_KEYDOWN => "WM_KEYDOWN",
            PInvoke.WM_KEYUP => "WM_KEYUP",
            PInvoke.WM_CHAR => "WM_CHAR",
            PInvoke.WM_DEADCHAR => "WM_DEADCHAR",
            PInvoke.WM_SYSKEYDOWN => "WM_SYSKEYDOWN",
            PInvoke.WM_SYSKEYUP => "WM_SYSKEYUP",
            PInvoke.WM_SYSCHAR => "WM_SYSCHAR",
            PInvoke.WM_SYSDEADCHAR => "WM_SYSDEADCHAR",
            PInvoke.WM_KEYLAST => "WM_KEYLAST",
            PInvoke.WM_IME_STARTCOMPOSITION => "WM_IME_STARTCOMPOSITION",
            PInvoke.WM_IME_ENDCOMPOSITION => "WM_IME_ENDCOMPOSITION",
            PInvoke.WM_IME_COMPOSITION => "WM_IME_COMPOSITION",
            PInvoke.WM_INITDIALOG => "WM_INITDIALOG",
            PInvoke.WM_COMMAND => "WM_COMMAND",
            PInvoke.WM_SYSCOMMAND => "WM_SYSCOMMAND",
            PInvoke.WM_TIMER => "WM_TIMER",
            PInvoke.WM_HSCROLL => "WM_HSCROLL",
            PInvoke.WM_VSCROLL => "WM_VSCROLL",
            PInvoke.WM_INITMENU => "WM_INITMENU",
            PInvoke.WM_INITMENUPOPUP => "WM_INITMENUPOPUP",
            PInvoke.WM_MENUSELECT => "WM_MENUSELECT",
            PInvoke.WM_MENUCHAR => "WM_MENUCHAR",
            PInvoke.WM_ENTERIDLE => "WM_ENTERIDLE",
            PInvoke.WM_CTLCOLORMSGBOX => "WM_CTLCOLORMSGBOX",
            PInvoke.WM_CTLCOLOREDIT => "WM_CTLCOLOREDIT",
            PInvoke.WM_CTLCOLORLISTBOX => "WM_CTLCOLORLISTBOX",
            PInvoke.WM_CTLCOLORBTN => "WM_CTLCOLORBTN",
            PInvoke.WM_CTLCOLORDLG => "WM_CTLCOLORDLG",
            PInvoke.WM_CTLCOLORSCROLLBAR => "WM_CTLCOLORSCROLLBAR",
            PInvoke.WM_CTLCOLORSTATIC => "WM_CTLCOLORSTATIC",
            PInvoke.WM_MOUSEMOVE => "WM_MOUSEMOVE",
            PInvoke.WM_LBUTTONDOWN => "WM_LBUTTONDOWN",
            PInvoke.WM_LBUTTONUP => "WM_LBUTTONUP",
            PInvoke.WM_LBUTTONDBLCLK => "WM_LBUTTONDBLCLK",
            PInvoke.WM_RBUTTONDOWN => "WM_RBUTTONDOWN",
            PInvoke.WM_RBUTTONUP => "WM_RBUTTONUP",
            PInvoke.WM_RBUTTONDBLCLK => "WM_RBUTTONDBLCLK",
            PInvoke.WM_MBUTTONDOWN => "WM_MBUTTONDOWN",
            PInvoke.WM_MBUTTONUP => "WM_MBUTTONUP",
            PInvoke.WM_MBUTTONDBLCLK => "WM_MBUTTONDBLCLK",
            PInvoke.WM_MOUSEWHEEL => "WM_MOUSEWHEEL",
            PInvoke.WM_PARENTNOTIFY => "WM_PARENTNOTIFY",
            PInvoke.WM_ENTERMENULOOP => "WM_ENTERMENULOOP",
            PInvoke.WM_EXITMENULOOP => "WM_EXITMENULOOP",
            PInvoke.WM_NEXTMENU => "WM_NEXTMENU",
            PInvoke.WM_SIZING => "WM_SIZING",
            PInvoke.WM_CAPTURECHANGED => "WM_CAPTURECHANGED",
            PInvoke.WM_MOVING => "WM_MOVING",
            PInvoke.WM_POWERBROADCAST => "WM_POWERBROADCAST",
            PInvoke.WM_DEVICECHANGE => "WM_DEVICECHANGE",
            PInvoke.WM_IME_SETCONTEXT => "WM_IME_SETCONTEXT",
            PInvoke.WM_IME_NOTIFY => "WM_IME_NOTIFY",
            PInvoke.WM_IME_CONTROL => "WM_IME_CONTROL",
            PInvoke.WM_IME_COMPOSITIONFULL => "WM_IME_COMPOSITIONFULL",
            PInvoke.WM_IME_SELECT => "WM_IME_SELECT",
            PInvoke.WM_IME_CHAR => "WM_IME_CHAR",
            PInvoke.WM_IME_KEYDOWN => "WM_IME_KEYDOWN",
            PInvoke.WM_IME_KEYUP => "WM_IME_KEYUP",
            PInvoke.WM_MDICREATE => "WM_MDICREATE",
            PInvoke.WM_MDIDESTROY => "WM_MDIDESTROY",
            PInvoke.WM_MDIACTIVATE => "WM_MDIACTIVATE",
            PInvoke.WM_MDIRESTORE => "WM_MDIRESTORE",
            PInvoke.WM_MDINEXT => "WM_MDINEXT",
            PInvoke.WM_MDIMAXIMIZE => "WM_MDIMAXIMIZE",
            PInvoke.WM_MDITILE => "WM_MDITILE",
            PInvoke.WM_MDICASCADE => "WM_MDICASCADE",
            PInvoke.WM_MDIICONARRANGE => "WM_MDIICONARRANGE",
            PInvoke.WM_MDIGETACTIVE => "WM_MDIGETACTIVE",
            PInvoke.WM_MDISETMENU => "WM_MDISETMENU",
            PInvoke.WM_ENTERSIZEMOVE => "WM_ENTERSIZEMOVE",
            PInvoke.WM_EXITSIZEMOVE => "WM_EXITSIZEMOVE",
            PInvoke.WM_DROPFILES => "WM_DROPFILES",
            PInvoke.WM_MDIREFRESHMENU => "WM_MDIREFRESHMENU",
            PInvoke.WM_MOUSEHOVER => "WM_MOUSEHOVER",
            PInvoke.WM_MOUSELEAVE => "WM_MOUSELEAVE",
            PInvoke.WM_CUT => "WM_CUT",
            PInvoke.WM_COPY => "WM_COPY",
            PInvoke.WM_PASTE => "WM_PASTE",
            PInvoke.WM_CLEAR => "WM_CLEAR",
            PInvoke.WM_UNDO => "WM_UNDO",
            PInvoke.WM_RENDERFORMAT => "WM_RENDERFORMAT",
            PInvoke.WM_RENDERALLFORMATS => "WM_RENDERALLFORMATS",
            PInvoke.WM_DESTROYCLIPBOARD => "WM_DESTROYCLIPBOARD",
            PInvoke.WM_DRAWCLIPBOARD => "WM_DRAWCLIPBOARD",
            PInvoke.WM_PAINTCLIPBOARD => "WM_PAINTCLIPBOARD",
            PInvoke.WM_VSCROLLCLIPBOARD => "WM_VSCROLLCLIPBOARD",
            PInvoke.WM_SIZECLIPBOARD => "WM_SIZECLIPBOARD",
            PInvoke.WM_ASKCBFORMATNAME => "WM_ASKCBFORMATNAME",
            PInvoke.WM_CHANGECBCHAIN => "WM_CHANGECBCHAIN",
            PInvoke.WM_HSCROLLCLIPBOARD => "WM_HSCROLLCLIPBOARD",
            PInvoke.WM_QUERYNEWPALETTE => "WM_QUERYNEWPALETTE",
            PInvoke.WM_PALETTEISCHANGING => "WM_PALETTEISCHANGING",
            PInvoke.WM_PALETTECHANGED => "WM_PALETTECHANGED",
            PInvoke.WM_HOTKEY => "WM_HOTKEY",
            PInvoke.WM_PRINT => "WM_PRINT",
            PInvoke.WM_PRINTCLIENT => "WM_PRINTCLIENT",
            PInvoke.WM_HANDHELDFIRST => "WM_HANDHELDFIRST",
            PInvoke.WM_HANDHELDLAST => "WM_HANDHELDLAST",
            PInvoke.WM_AFXFIRST => "WM_AFXFIRST",
            PInvoke.WM_AFXLAST => "WM_AFXLAST",
            PInvoke.WM_PENWINFIRST => "WM_PENWINFIRST",
            PInvoke.WM_PENWINLAST => "WM_PENWINLAST",
            PInvoke.WM_APP => "WM_APP",
            PInvoke.WM_USER => "WM_USER",
            PInvoke.WM_CTLCOLOR => "WM_CTLCOLOR",

            // RichEdit messages
            PInvoke.EM_GETLIMITTEXT => "EM_GETLIMITTEXT",
            PInvoke.EM_POSFROMCHAR => "EM_POSFROMCHAR",
            PInvoke.EM_CHARFROMPOS => "EM_CHARFROMPOS",
            PInvoke.EM_SCROLLCARET => "EM_SCROLLCARET",
            PInvoke.EM_CANPASTE => "EM_CANPASTE",
            PInvoke.EM_DISPLAYBAND => "EM_DISPLAYBAND",
            PInvoke.EM_EXGETSEL => "EM_EXGETSEL",
            PInvoke.EM_EXLIMITTEXT => "EM_EXLIMITTEXT",
            PInvoke.EM_EXLINEFROMCHAR => "EM_EXLINEFROMCHAR",
            PInvoke.EM_EXSETSEL => "EM_EXSETSEL",
            PInvoke.EM_FINDTEXT => "EM_FINDTEXT",
            PInvoke.EM_FORMATRANGE => "EM_FORMATRANGE",
            PInvoke.EM_GETCHARFORMAT => "EM_GETCHARFORMAT",
            PInvoke.EM_GETEVENTMASK => "EM_GETEVENTMASK",
            PInvoke.EM_GETOLEINTERFACE => "EM_GETOLEINTERFACE",
            PInvoke.EM_GETPARAFORMAT => "EM_GETPARAFORMAT",
            PInvoke.EM_GETSELTEXT => "EM_GETSELTEXT",
            PInvoke.EM_HIDESELECTION => "EM_HIDESELECTION",
            PInvoke.EM_PASTESPECIAL => "EM_PASTESPECIAL",
            PInvoke.EM_REQUESTRESIZE => "EM_REQUESTRESIZE",
            PInvoke.EM_SELECTIONTYPE => "EM_SELECTIONTYPE",
            PInvoke.EM_SETBKGNDCOLOR => "EM_SETBKGNDCOLOR",
            PInvoke.EM_SETCHARFORMAT => "EM_SETCHARFORMAT",
            PInvoke.EM_SETEVENTMASK => "EM_SETEVENTMASK",
            PInvoke.EM_SETOLECALLBACK => "EM_SETOLECALLBACK",
            PInvoke.EM_SETPARAFORMAT => "EM_SETPARAFORMAT",
            PInvoke.EM_SETTARGETDEVICE => "EM_SETTARGETDEVICE",
            PInvoke.EM_STREAMIN => "EM_STREAMIN",
            PInvoke.EM_STREAMOUT => "EM_STREAMOUT",
            PInvoke.EM_GETTEXTRANGE => "EM_GETTEXTRANGE",
            PInvoke.EM_FINDWORDBREAK => "EM_FINDWORDBREAK",
            PInvoke.EM_SETOPTIONS => "EM_SETOPTIONS",
            PInvoke.EM_GETOPTIONS => "EM_GETOPTIONS",
            PInvoke.EM_FINDTEXTEX => "EM_FINDTEXTEX",
            PInvoke.EM_GETWORDBREAKPROCEX => "EM_GETWORDBREAKPROCEX",
            PInvoke.EM_SETWORDBREAKPROCEX => "EM_SETWORDBREAKPROCEX",

            // Richedit v2.0 messages
            PInvoke.EM_SETUNDOLIMIT => "EM_SETUNDOLIMIT",
            PInvoke.EM_REDO => "EM_REDO",
            PInvoke.EM_CANREDO => "EM_CANREDO",
            PInvoke.EM_GETUNDONAME => "EM_GETUNDONAME",
            PInvoke.EM_GETREDONAME => "EM_GETREDONAME",
            PInvoke.EM_STOPGROUPTYPING => "EM_STOPGROUPTYPING",
            PInvoke.EM_SETTEXTMODE => "EM_SETTEXTMODE",
            PInvoke.EM_GETTEXTMODE => "EM_GETTEXTMODE",
            PInvoke.EM_AUTOURLDETECT => "EM_AUTOURLDETECT",
            PInvoke.EM_GETAUTOURLDETECT => "EM_GETAUTOURLDETECT",
            PInvoke.EM_SETPALETTE => "EM_SETPALETTE",
            PInvoke.EM_GETTEXTEX => "EM_GETTEXTEX",
            PInvoke.EM_GETTEXTLENGTHEX => "EM_GETTEXTLENGTHEX",

            // Asia specific messages
            PInvoke.EM_SETPUNCTUATION => "EM_SETPUNCTUATION",
            PInvoke.EM_GETPUNCTUATION => "EM_GETPUNCTUATION",
            PInvoke.EM_SETWORDWRAPMODE => "EM_SETWORDWRAPMODE",
            PInvoke.EM_GETWORDWRAPMODE => "EM_GETWORDWRAPMODE",
            PInvoke.EM_SETIMECOLOR => "EM_SETIMECOLOR",
            PInvoke.EM_GETIMECOLOR => "EM_GETIMECOLOR",
            PInvoke.EM_SETIMEOPTIONS => "EM_SETIMEOPTIONS",
            PInvoke.EM_GETIMEOPTIONS => "EM_GETIMEOPTIONS",
            PInvoke.EM_CONVPOSITION => "EM_CONVPOSITION",
            PInvoke.EM_SETLANGOPTIONS => "EM_SETLANGOPTIONS",
            PInvoke.EM_GETLANGOPTIONS => "EM_GETLANGOPTIONS",
            PInvoke.EM_GETIMECOMPMODE => "EM_GETIMECOMPMODE",
            PInvoke.EM_FINDTEXTW => "EM_FINDTEXTW",
            PInvoke.EM_FINDTEXTEXW => "EM_FINDTEXTEXW",

            // Rich Edit 3.0 Asia messages
            PInvoke.EM_RECONVERSION => "EM_RECONVERSION",
            PInvoke.EM_SETIMEMODEBIAS => "EM_SETIMEMODEBIAS",
            PInvoke.EM_GETIMEMODEBIAS => "EM_GETIMEMODEBIAS",

            // BiDi Specific messages
            PInvoke.EM_SETBIDIOPTIONS => "EM_SETBIDIOPTIONS",
            PInvoke.EM_GETBIDIOPTIONS => "EM_GETBIDIOPTIONS",
            PInvoke.EM_SETTYPOGRAPHYOPTIONS => "EM_SETTYPOGRAPHYOPTIONS",
            PInvoke.EM_GETTYPOGRAPHYOPTIONS => "EM_GETTYPOGRAPHYOPTIONS",

            // Extended Edit style specific messages
            PInvoke.EM_SETEDITSTYLE => "EM_SETEDITSTYLE",
            PInvoke.EM_GETEDITSTYLE => "EM_GETEDITSTYLE",
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
