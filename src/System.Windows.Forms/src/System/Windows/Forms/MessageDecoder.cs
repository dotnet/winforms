// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Decodes Windows messages. This is in a separate class from Message so we can avoid
    ///  loading it in the 99% case where we don't need it.
    /// </summary>
    internal static class MessageDecoder
    {
        /// <summary>
        ///  Returns the symbolic name of the msg value, or null if it isn't one of the
        ///  existing constants.
        /// </summary>
        private static string MsgToString(int msg)
        {
            string text;
            switch (msg)
            {
                case WindowMessages.WM_NULL:
                    text = "WM_NULL";
                    break;
                case WindowMessages.WM_CREATE:
                    text = "WM_CREATE";
                    break;
                case WindowMessages.WM_DESTROY:
                    text = "WM_DESTROY";
                    break;
                case WindowMessages.WM_MOVE:
                    text = "WM_MOVE";
                    break;
                case WindowMessages.WM_SIZE:
                    text = "WM_SIZE";
                    break;
                case WindowMessages.WM_ACTIVATE:
                    text = "WM_ACTIVATE";
                    break;
                case WindowMessages.WM_SETFOCUS:
                    text = "WM_SETFOCUS";
                    break;
                case WindowMessages.WM_KILLFOCUS:
                    text = "WM_KILLFOCUS";
                    break;
                case WindowMessages.WM_ENABLE:
                    text = "WM_ENABLE";
                    break;
                case WindowMessages.WM_SETREDRAW:
                    text = "WM_SETREDRAW";
                    break;
                case WindowMessages.WM_SETTEXT:
                    text = "WM_SETTEXT";
                    break;
                case WindowMessages.WM_GETTEXT:
                    text = "WM_GETTEXT";
                    break;
                case WindowMessages.WM_GETTEXTLENGTH:
                    text = "WM_GETTEXTLENGTH";
                    break;
                case WindowMessages.WM_PAINT:
                    text = "WM_PAINT";
                    break;
                case WindowMessages.WM_CLOSE:
                    text = "WM_CLOSE";
                    break;
                case WindowMessages.WM_QUERYENDSESSION:
                    text = "WM_QUERYENDSESSION";
                    break;
                case WindowMessages.WM_QUIT:
                    text = "WM_QUIT";
                    break;
                case WindowMessages.WM_QUERYOPEN:
                    text = "WM_QUERYOPEN";
                    break;
                case WindowMessages.WM_ERASEBKGND:
                    text = "WM_ERASEBKGND";
                    break;
                case WindowMessages.WM_SYSCOLORCHANGE:
                    text = "WM_SYSCOLORCHANGE";
                    break;
                case WindowMessages.WM_ENDSESSION:
                    text = "WM_ENDSESSION";
                    break;
                case WindowMessages.WM_SHOWWINDOW:
                    text = "WM_SHOWWINDOW";
                    break;
                case WindowMessages.WM_WININICHANGE:
                    text = "WM_WININICHANGE";
                    break;
                case WindowMessages.WM_DEVMODECHANGE:
                    text = "WM_DEVMODECHANGE";
                    break;
                case WindowMessages.WM_ACTIVATEAPP:
                    text = "WM_ACTIVATEAPP";
                    break;
                case WindowMessages.WM_FONTCHANGE:
                    text = "WM_FONTCHANGE";
                    break;
                case WindowMessages.WM_TIMECHANGE:
                    text = "WM_TIMECHANGE";
                    break;
                case WindowMessages.WM_CANCELMODE:
                    text = "WM_CANCELMODE";
                    break;
                case WindowMessages.WM_SETCURSOR:
                    text = "WM_SETCURSOR";
                    break;
                case WindowMessages.WM_MOUSEACTIVATE:
                    text = "WM_MOUSEACTIVATE";
                    break;
                case WindowMessages.WM_CHILDACTIVATE:
                    text = "WM_CHILDACTIVATE";
                    break;
                case WindowMessages.WM_QUEUESYNC:
                    text = "WM_QUEUESYNC";
                    break;
                case WindowMessages.WM_GETMINMAXINFO:
                    text = "WM_GETMINMAXINFO";
                    break;
                case WindowMessages.WM_PAINTICON:
                    text = "WM_PAINTICON";
                    break;
                case WindowMessages.WM_ICONERASEBKGND:
                    text = "WM_ICONERASEBKGND";
                    break;
                case WindowMessages.WM_NEXTDLGCTL:
                    text = "WM_NEXTDLGCTL";
                    break;
                case WindowMessages.WM_SPOOLERSTATUS:
                    text = "WM_SPOOLERSTATUS";
                    break;
                case WindowMessages.WM_DRAWITEM:
                    text = "WM_DRAWITEM";
                    break;
                case WindowMessages.WM_MEASUREITEM:
                    text = "WM_MEASUREITEM";
                    break;
                case WindowMessages.WM_DELETEITEM:
                    text = "WM_DELETEITEM";
                    break;
                case WindowMessages.WM_VKEYTOITEM:
                    text = "WM_VKEYTOITEM";
                    break;
                case WindowMessages.WM_CHARTOITEM:
                    text = "WM_CHARTOITEM";
                    break;
                case WindowMessages.WM_SETFONT:
                    text = "WM_SETFONT";
                    break;
                case WindowMessages.WM_GETFONT:
                    text = "WM_GETFONT";
                    break;
                case WindowMessages.WM_SETHOTKEY:
                    text = "WM_SETHOTKEY";
                    break;
                case WindowMessages.WM_GETHOTKEY:
                    text = "WM_GETHOTKEY";
                    break;
                case WindowMessages.WM_QUERYDRAGICON:
                    text = "WM_QUERYDRAGICON";
                    break;
                case WindowMessages.WM_COMPAREITEM:
                    text = "WM_COMPAREITEM";
                    break;
                case WindowMessages.WM_GETOBJECT:
                    text = "WM_GETOBJECT";
                    break;
                case WindowMessages.WM_COMPACTING:
                    text = "WM_COMPACTING";
                    break;
                case WindowMessages.WM_COMMNOTIFY:
                    text = "WM_COMMNOTIFY";
                    break;
                case WindowMessages.WM_WINDOWPOSCHANGING:
                    text = "WM_WINDOWPOSCHANGING";
                    break;
                case WindowMessages.WM_WINDOWPOSCHANGED:
                    text = "WM_WINDOWPOSCHANGED";
                    break;
                case WindowMessages.WM_POWER:
                    text = "WM_POWER";
                    break;
                case WindowMessages.WM_COPYDATA:
                    text = "WM_COPYDATA";
                    break;
                case WindowMessages.WM_CANCELJOURNAL:
                    text = "WM_CANCELJOURNAL";
                    break;
                case WindowMessages.WM_NOTIFY:
                    text = "WM_NOTIFY";
                    break;
                case WindowMessages.WM_INPUTLANGCHANGEREQUEST:
                    text = "WM_INPUTLANGCHANGEREQUEST";
                    break;
                case WindowMessages.WM_INPUTLANGCHANGE:
                    text = "WM_INPUTLANGCHANGE";
                    break;
                case WindowMessages.WM_TCARD:
                    text = "WM_TCARD";
                    break;
                case WindowMessages.WM_HELP:
                    text = "WM_HELP";
                    break;
                case WindowMessages.WM_USERCHANGED:
                    text = "WM_USERCHANGED";
                    break;
                case WindowMessages.WM_NOTIFYFORMAT:
                    text = "WM_NOTIFYFORMAT";
                    break;
                case WindowMessages.WM_CONTEXTMENU:
                    text = "WM_CONTEXTMENU";
                    break;
                case WindowMessages.WM_STYLECHANGING:
                    text = "WM_STYLECHANGING";
                    break;
                case WindowMessages.WM_STYLECHANGED:
                    text = "WM_STYLECHANGED";
                    break;
                case WindowMessages.WM_DISPLAYCHANGE:
                    text = "WM_DISPLAYCHANGE";
                    break;
                case WindowMessages.WM_GETICON:
                    text = "WM_GETICON";
                    break;
                case WindowMessages.WM_SETICON:
                    text = "WM_SETICON";
                    break;
                case WindowMessages.WM_NCCREATE:
                    text = "WM_NCCREATE";
                    break;
                case WindowMessages.WM_NCDESTROY:
                    text = "WM_NCDESTROY";
                    break;
                case WindowMessages.WM_NCCALCSIZE:
                    text = "WM_NCCALCSIZE";
                    break;
                case WindowMessages.WM_NCHITTEST:
                    text = "WM_NCHITTEST";
                    break;
                case WindowMessages.WM_NCPAINT:
                    text = "WM_NCPAINT";
                    break;
                case WindowMessages.WM_NCACTIVATE:
                    text = "WM_NCACTIVATE";
                    break;
                case WindowMessages.WM_GETDLGCODE:
                    text = "WM_GETDLGCODE";
                    break;
                case WindowMessages.WM_NCMOUSEMOVE:
                    text = "WM_NCMOUSEMOVE";
                    break;
                case WindowMessages.WM_NCLBUTTONDOWN:
                    text = "WM_NCLBUTTONDOWN";
                    break;
                case WindowMessages.WM_NCLBUTTONUP:
                    text = "WM_NCLBUTTONUP";
                    break;
                case WindowMessages.WM_NCLBUTTONDBLCLK:
                    text = "WM_NCLBUTTONDBLCLK";
                    break;
                case WindowMessages.WM_NCRBUTTONDOWN:
                    text = "WM_NCRBUTTONDOWN";
                    break;
                case WindowMessages.WM_NCRBUTTONUP:
                    text = "WM_NCRBUTTONUP";
                    break;
                case WindowMessages.WM_NCRBUTTONDBLCLK:
                    text = "WM_NCRBUTTONDBLCLK";
                    break;
                case WindowMessages.WM_NCMBUTTONDOWN:
                    text = "WM_NCMBUTTONDOWN";
                    break;
                case WindowMessages.WM_NCMBUTTONUP:
                    text = "WM_NCMBUTTONUP";
                    break;
                case WindowMessages.WM_NCMBUTTONDBLCLK:
                    text = "WM_NCMBUTTONDBLCLK";
                    break;
                case WindowMessages.WM_KEYDOWN:
                    text = "WM_KEYDOWN";
                    break;
                case WindowMessages.WM_KEYUP:
                    text = "WM_KEYUP";
                    break;
                case WindowMessages.WM_CHAR:
                    text = "WM_CHAR";
                    break;
                case WindowMessages.WM_DEADCHAR:
                    text = "WM_DEADCHAR";
                    break;
                case WindowMessages.WM_SYSKEYDOWN:
                    text = "WM_SYSKEYDOWN";
                    break;
                case WindowMessages.WM_SYSKEYUP:
                    text = "WM_SYSKEYUP";
                    break;
                case WindowMessages.WM_SYSCHAR:
                    text = "WM_SYSCHAR";
                    break;
                case WindowMessages.WM_SYSDEADCHAR:
                    text = "WM_SYSDEADCHAR";
                    break;
                case WindowMessages.WM_KEYLAST:
                    text = "WM_KEYLAST";
                    break;
                case WindowMessages.WM_IME_STARTCOMPOSITION:
                    text = "WM_IME_STARTCOMPOSITION";
                    break;
                case WindowMessages.WM_IME_ENDCOMPOSITION:
                    text = "WM_IME_ENDCOMPOSITION";
                    break;
                case WindowMessages.WM_IME_COMPOSITION:
                    text = "WM_IME_COMPOSITION";
                    break;
                case WindowMessages.WM_INITDIALOG:
                    text = "WM_INITDIALOG";
                    break;
                case WindowMessages.WM_COMMAND:
                    text = "WM_COMMAND";
                    break;
                case WindowMessages.WM_SYSCOMMAND:
                    text = "WM_SYSCOMMAND";
                    break;
                case WindowMessages.WM_TIMER:
                    text = "WM_TIMER";
                    break;
                case WindowMessages.WM_HSCROLL:
                    text = "WM_HSCROLL";
                    break;
                case WindowMessages.WM_VSCROLL:
                    text = "WM_VSCROLL";
                    break;
                case WindowMessages.WM_INITMENU:
                    text = "WM_INITMENU";
                    break;
                case WindowMessages.WM_INITMENUPOPUP:
                    text = "WM_INITMENUPOPUP";
                    break;
                case WindowMessages.WM_MENUSELECT:
                    text = "WM_MENUSELECT";
                    break;
                case WindowMessages.WM_MENUCHAR:
                    text = "WM_MENUCHAR";
                    break;
                case WindowMessages.WM_ENTERIDLE:
                    text = "WM_ENTERIDLE";
                    break;
                case WindowMessages.WM_CTLCOLORMSGBOX:
                    text = "WM_CTLCOLORMSGBOX";
                    break;
                case WindowMessages.WM_CTLCOLOREDIT:
                    text = "WM_CTLCOLOREDIT";
                    break;
                case WindowMessages.WM_CTLCOLORLISTBOX:
                    text = "WM_CTLCOLORLISTBOX";
                    break;
                case WindowMessages.WM_CTLCOLORBTN:
                    text = "WM_CTLCOLORBTN";
                    break;
                case WindowMessages.WM_CTLCOLORDLG:
                    text = "WM_CTLCOLORDLG";
                    break;
                case WindowMessages.WM_CTLCOLORSCROLLBAR:
                    text = "WM_CTLCOLORSCROLLBAR";
                    break;
                case WindowMessages.WM_CTLCOLORSTATIC:
                    text = "WM_CTLCOLORSTATIC";
                    break;
                case WindowMessages.WM_MOUSEMOVE:
                    text = "WM_MOUSEMOVE";
                    break;
                case WindowMessages.WM_LBUTTONDOWN:
                    text = "WM_LBUTTONDOWN";
                    break;
                case WindowMessages.WM_LBUTTONUP:
                    text = "WM_LBUTTONUP";
                    break;
                case WindowMessages.WM_LBUTTONDBLCLK:
                    text = "WM_LBUTTONDBLCLK";
                    break;
                case WindowMessages.WM_RBUTTONDOWN:
                    text = "WM_RBUTTONDOWN";
                    break;
                case WindowMessages.WM_RBUTTONUP:
                    text = "WM_RBUTTONUP";
                    break;
                case WindowMessages.WM_RBUTTONDBLCLK:
                    text = "WM_RBUTTONDBLCLK";
                    break;
                case WindowMessages.WM_MBUTTONDOWN:
                    text = "WM_MBUTTONDOWN";
                    break;
                case WindowMessages.WM_MBUTTONUP:
                    text = "WM_MBUTTONUP";
                    break;
                case WindowMessages.WM_MBUTTONDBLCLK:
                    text = "WM_MBUTTONDBLCLK";
                    break;
                case WindowMessages.WM_MOUSEWHEEL:
                    text = "WM_MOUSEWHEEL";
                    break;
                case WindowMessages.WM_PARENTNOTIFY:
                    text = "WM_PARENTNOTIFY";
                    break;
                case WindowMessages.WM_ENTERMENULOOP:
                    text = "WM_ENTERMENULOOP";
                    break;
                case WindowMessages.WM_EXITMENULOOP:
                    text = "WM_EXITMENULOOP";
                    break;
                case WindowMessages.WM_NEXTMENU:
                    text = "WM_NEXTMENU";
                    break;
                case WindowMessages.WM_SIZING:
                    text = "WM_SIZING";
                    break;
                case WindowMessages.WM_CAPTURECHANGED:
                    text = "WM_CAPTURECHANGED";
                    break;
                case WindowMessages.WM_MOVING:
                    text = "WM_MOVING";
                    break;
                case WindowMessages.WM_POWERBROADCAST:
                    text = "WM_POWERBROADCAST";
                    break;
                case WindowMessages.WM_DEVICECHANGE:
                    text = "WM_DEVICECHANGE";
                    break;
                case WindowMessages.WM_IME_SETCONTEXT:
                    text = "WM_IME_SETCONTEXT";
                    break;
                case WindowMessages.WM_IME_NOTIFY:
                    text = "WM_IME_NOTIFY";
                    break;
                case WindowMessages.WM_IME_CONTROL:
                    text = "WM_IME_CONTROL";
                    break;
                case WindowMessages.WM_IME_COMPOSITIONFULL:
                    text = "WM_IME_COMPOSITIONFULL";
                    break;
                case WindowMessages.WM_IME_SELECT:
                    text = "WM_IME_SELECT";
                    break;
                case WindowMessages.WM_IME_CHAR:
                    text = "WM_IME_CHAR";
                    break;
                case WindowMessages.WM_IME_KEYDOWN:
                    text = "WM_IME_KEYDOWN";
                    break;
                case WindowMessages.WM_IME_KEYUP:
                    text = "WM_IME_KEYUP";
                    break;
                case WindowMessages.WM_MDICREATE:
                    text = "WM_MDICREATE";
                    break;
                case WindowMessages.WM_MDIDESTROY:
                    text = "WM_MDIDESTROY";
                    break;
                case WindowMessages.WM_MDIACTIVATE:
                    text = "WM_MDIACTIVATE";
                    break;
                case WindowMessages.WM_MDIRESTORE:
                    text = "WM_MDIRESTORE";
                    break;
                case WindowMessages.WM_MDINEXT:
                    text = "WM_MDINEXT";
                    break;
                case WindowMessages.WM_MDIMAXIMIZE:
                    text = "WM_MDIMAXIMIZE";
                    break;
                case WindowMessages.WM_MDITILE:
                    text = "WM_MDITILE";
                    break;
                case WindowMessages.WM_MDICASCADE:
                    text = "WM_MDICASCADE";
                    break;
                case WindowMessages.WM_MDIICONARRANGE:
                    text = "WM_MDIICONARRANGE";
                    break;
                case WindowMessages.WM_MDIGETACTIVE:
                    text = "WM_MDIGETACTIVE";
                    break;
                case WindowMessages.WM_MDISETMENU:
                    text = "WM_MDISETMENU";
                    break;
                case WindowMessages.WM_ENTERSIZEMOVE:
                    text = "WM_ENTERSIZEMOVE";
                    break;
                case WindowMessages.WM_EXITSIZEMOVE:
                    text = "WM_EXITSIZEMOVE";
                    break;
                case WindowMessages.WM_DROPFILES:
                    text = "WM_DROPFILES";
                    break;
                case WindowMessages.WM_MDIREFRESHMENU:
                    text = "WM_MDIREFRESHMENU";
                    break;
                case WindowMessages.WM_MOUSEHOVER:
                    text = "WM_MOUSEHOVER";
                    break;
                case WindowMessages.WM_MOUSELEAVE:
                    text = "WM_MOUSELEAVE";
                    break;
                case WindowMessages.WM_CUT:
                    text = "WM_CUT";
                    break;
                case WindowMessages.WM_COPY:
                    text = "WM_COPY";
                    break;
                case WindowMessages.WM_PASTE:
                    text = "WM_PASTE";
                    break;
                case WindowMessages.WM_CLEAR:
                    text = "WM_CLEAR";
                    break;
                case WindowMessages.WM_UNDO:
                    text = "WM_UNDO";
                    break;
                case WindowMessages.WM_RENDERFORMAT:
                    text = "WM_RENDERFORMAT";
                    break;
                case WindowMessages.WM_RENDERALLFORMATS:
                    text = "WM_RENDERALLFORMATS";
                    break;
                case WindowMessages.WM_DESTROYCLIPBOARD:
                    text = "WM_DESTROYCLIPBOARD";
                    break;
                case WindowMessages.WM_DRAWCLIPBOARD:
                    text = "WM_DRAWCLIPBOARD";
                    break;
                case WindowMessages.WM_PAINTCLIPBOARD:
                    text = "WM_PAINTCLIPBOARD";
                    break;
                case WindowMessages.WM_VSCROLLCLIPBOARD:
                    text = "WM_VSCROLLCLIPBOARD";
                    break;
                case WindowMessages.WM_SIZECLIPBOARD:
                    text = "WM_SIZECLIPBOARD";
                    break;
                case WindowMessages.WM_ASKCBFORMATNAME:
                    text = "WM_ASKCBFORMATNAME";
                    break;
                case WindowMessages.WM_CHANGECBCHAIN:
                    text = "WM_CHANGECBCHAIN";
                    break;
                case WindowMessages.WM_HSCROLLCLIPBOARD:
                    text = "WM_HSCROLLCLIPBOARD";
                    break;
                case WindowMessages.WM_QUERYNEWPALETTE:
                    text = "WM_QUERYNEWPALETTE";
                    break;
                case WindowMessages.WM_PALETTEISCHANGING:
                    text = "WM_PALETTEISCHANGING";
                    break;
                case WindowMessages.WM_PALETTECHANGED:
                    text = "WM_PALETTECHANGED";
                    break;
                case WindowMessages.WM_HOTKEY:
                    text = "WM_HOTKEY";
                    break;
                case WindowMessages.WM_PRINT:
                    text = "WM_PRINT";
                    break;
                case WindowMessages.WM_PRINTCLIENT:
                    text = "WM_PRINTCLIENT";
                    break;
                case WindowMessages.WM_HANDHELDFIRST:
                    text = "WM_HANDHELDFIRST";
                    break;
                case WindowMessages.WM_HANDHELDLAST:
                    text = "WM_HANDHELDLAST";
                    break;
                case WindowMessages.WM_AFXFIRST:
                    text = "WM_AFXFIRST";
                    break;
                case WindowMessages.WM_AFXLAST:
                    text = "WM_AFXLAST";
                    break;
                case WindowMessages.WM_PENWINFIRST:
                    text = "WM_PENWINFIRST";
                    break;
                case WindowMessages.WM_PENWINLAST:
                    text = "WM_PENWINLAST";
                    break;
                case WindowMessages.WM_APP:
                    text = "WM_APP";
                    break;
                case WindowMessages.WM_USER:
                    text = "WM_USER";
                    break;
                case WindowMessages.WM_CTLCOLOR:
                    text = "WM_CTLCOLOR";
                    break;

                // RichEdit messages
                case EditMessages.EM_GETLIMITTEXT:
                    text = "EM_GETLIMITTEXT";
                    break;
                case EditMessages.EM_POSFROMCHAR:
                    text = "EM_POSFROMCHAR";
                    break;
                case EditMessages.EM_CHARFROMPOS:
                    text = "EM_CHARFROMPOS";
                    break;
                case EditMessages.EM_SCROLLCARET:
                    text = "EM_SCROLLCARET";
                    break;
                case RichEditMessages.EM_CANPASTE:
                    text = "EM_CANPASTE";
                    break;
                case RichEditMessages.EM_DISPLAYBAND:
                    text = "EM_DISPLAYBAND";
                    break;
                case RichEditMessages.EM_EXGETSEL:
                    text = "EM_EXGETSEL";
                    break;
                case RichEditMessages.EM_EXLIMITTEXT:
                    text = "EM_EXLIMITTEXT";
                    break;
                case RichEditMessages.EM_EXLINEFROMCHAR:
                    text = "EM_EXLINEFROMCHAR";
                    break;
                case RichEditMessages.EM_EXSETSEL:
                    text = "EM_EXSETSEL";
                    break;
                case RichEditMessages.EM_FINDTEXT:
                    text = "EM_FINDTEXT";
                    break;
                case RichEditMessages.EM_FORMATRANGE:
                    text = "EM_FORMATRANGE";
                    break;
                case RichEditMessages.EM_GETCHARFORMAT:
                    text = "EM_GETCHARFORMAT";
                    break;
                case RichEditMessages.EM_GETEVENTMASK:
                    text = "EM_GETEVENTMASK";
                    break;
                case RichEditMessages.EM_GETOLEINTERFACE:
                    text = "EM_GETOLEINTERFACE";
                    break;
                case RichEditMessages.EM_GETPARAFORMAT:
                    text = "EM_GETPARAFORMAT";
                    break;
                case RichEditMessages.EM_GETSELTEXT:
                    text = "EM_GETSELTEXT";
                    break;
                case RichEditMessages.EM_HIDESELECTION:
                    text = "EM_HIDESELECTION";
                    break;
                case RichEditMessages.EM_PASTESPECIAL:
                    text = "EM_PASTESPECIAL";
                    break;
                case RichEditMessages.EM_REQUESTRESIZE:
                    text = "EM_REQUESTRESIZE";
                    break;
                case RichEditMessages.EM_SELECTIONTYPE:
                    text = "EM_SELECTIONTYPE";
                    break;
                case RichEditMessages.EM_SETBKGNDCOLOR:
                    text = "EM_SETBKGNDCOLOR";
                    break;
                case RichEditMessages.EM_SETCHARFORMAT:
                    text = "EM_SETCHARFORMAT";
                    break;
                case RichEditMessages.EM_SETEVENTMASK:
                    text = "EM_SETEVENTMASK";
                    break;
                case RichEditMessages.EM_SETOLECALLBACK:
                    text = "EM_SETOLECALLBACK";
                    break;
                case RichEditMessages.EM_SETPARAFORMAT:
                    text = "EM_SETPARAFORMAT";
                    break;
                case RichEditMessages.EM_SETTARGETDEVICE:
                    text = "EM_SETTARGETDEVICE";
                    break;
                case RichEditMessages.EM_STREAMIN:
                    text = "EM_STREAMIN";
                    break;
                case RichEditMessages.EM_STREAMOUT:
                    text = "EM_STREAMOUT";
                    break;
                case RichEditMessages.EM_GETTEXTRANGE:
                    text = "EM_GETTEXTRANGE";
                    break;
                case RichEditMessages.EM_FINDWORDBREAK:
                    text = "EM_FINDWORDBREAK";
                    break;
                case RichEditMessages.EM_SETOPTIONS:
                    text = "EM_SETOPTIONS";
                    break;
                case RichEditMessages.EM_GETOPTIONS:
                    text = "EM_GETOPTIONS";
                    break;
                case RichEditMessages.EM_FINDTEXTEX:
                    text = "EM_FINDTEXTEX";
                    break;
                case RichEditMessages.EM_GETWORDBREAKPROCEX:
                    text = "EM_GETWORDBREAKPROCEX";
                    break;
                case RichEditMessages.EM_SETWORDBREAKPROCEX:
                    text = "EM_SETWORDBREAKPROCEX";
                    break;

                // Richedit v2.0 messages
                case RichEditMessages.EM_SETUNDOLIMIT:
                    text = "EM_SETUNDOLIMIT";
                    break;
                case RichEditMessages.EM_REDO:
                    text = "EM_REDO";
                    break;
                case RichEditMessages.EM_CANREDO:
                    text = "EM_CANREDO";
                    break;
                case RichEditMessages.EM_GETUNDONAME:
                    text = "EM_GETUNDONAME";
                    break;
                case RichEditMessages.EM_GETREDONAME:
                    text = "EM_GETREDONAME";
                    break;
                case RichEditMessages.EM_STOPGROUPTYPING:
                    text = "EM_STOPGROUPTYPING";
                    break;
                case RichEditMessages.EM_SETTEXTMODE:
                    text = "EM_SETTEXTMODE";
                    break;
                case RichEditMessages.EM_GETTEXTMODE:
                    text = "EM_GETTEXTMODE";
                    break;
                case RichEditMessages.EM_AUTOURLDETECT:
                    text = "EM_AUTOURLDETECT";
                    break;
                case RichEditMessages.EM_GETAUTOURLDETECT:
                    text = "EM_GETAUTOURLDETECT";
                    break;
                case RichEditMessages.EM_SETPALETTE:
                    text = "EM_SETPALETTE";
                    break;
                case RichEditMessages.EM_GETTEXTEX:
                    text = "EM_GETTEXTEX";
                    break;
                case RichEditMessages.EM_GETTEXTLENGTHEX:
                    text = "EM_GETTEXTLENGTHEX";
                    break;

                // Asia specific messages
                case RichEditMessages.EM_SETPUNCTUATION:
                    text = "EM_SETPUNCTUATION";
                    break;
                case RichEditMessages.EM_GETPUNCTUATION:
                    text = "EM_GETPUNCTUATION";
                    break;
                case RichEditMessages.EM_SETWORDWRAPMODE:
                    text = "EM_SETWORDWRAPMODE";
                    break;
                case RichEditMessages.EM_GETWORDWRAPMODE:
                    text = "EM_GETWORDWRAPMODE";
                    break;
                case RichEditMessages.EM_SETIMECOLOR:
                    text = "EM_SETIMECOLOR";
                    break;
                case RichEditMessages.EM_GETIMECOLOR:
                    text = "EM_GETIMECOLOR";
                    break;
                case RichEditMessages.EM_SETIMEOPTIONS:
                    text = "EM_SETIMEOPTIONS";
                    break;
                case RichEditMessages.EM_GETIMEOPTIONS:
                    text = "EM_GETIMEOPTIONS";
                    break;
                case RichEditMessages.EM_CONVPOSITION:
                    text = "EM_CONVPOSITION";
                    break;
                case RichEditMessages.EM_SETLANGOPTIONS:
                    text = "EM_SETLANGOPTIONS";
                    break;
                case RichEditMessages.EM_GETLANGOPTIONS:
                    text = "EM_GETLANGOPTIONS";
                    break;
                case RichEditMessages.EM_GETIMECOMPMODE:
                    text = "EM_GETIMECOMPMODE";
                    break;
                case RichEditMessages.EM_FINDTEXTW:
                    text = "EM_FINDTEXTW";
                    break;
                case RichEditMessages.EM_FINDTEXTEXW:
                    text = "EM_FINDTEXTEXW";
                    break;

                // Rich Edit 3.0 Asia msgs
                case RichEditMessages.EM_RECONVERSION:
                    text = "EM_RECONVERSION";
                    break;
                case RichEditMessages.EM_SETIMEMODEBIAS:
                    text = "EM_SETIMEMODEBIAS";
                    break;
                case RichEditMessages.EM_GETIMEMODEBIAS:
                    text = "EM_GETIMEMODEBIAS";
                    break;

                // BiDi Specific messages
                case RichEditMessages.EM_SETBIDIOPTIONS:
                    text = "EM_SETBIDIOPTIONS";
                    break;
                case RichEditMessages.EM_GETBIDIOPTIONS:
                    text = "EM_GETBIDIOPTIONS";
                    break;
                case RichEditMessages.EM_SETTYPOGRAPHYOPTIONS:
                    text = "EM_SETTYPOGRAPHYOPTIONS";
                    break;
                case RichEditMessages.EM_GETTYPOGRAPHYOPTIONS:
                    text = "EM_GETTYPOGRAPHYOPTIONS";
                    break;

                // Extended Edit style specific messages
                case RichEditMessages.EM_SETEDITSTYLE:
                    text = "EM_SETEDITSTYLE";
                    break;
                case RichEditMessages.EM_GETEDITSTYLE:
                    text = "EM_GETEDITSTYLE";
                    break;

                default:
                    text = null;
                    break;
            }

            if (text == null && ((msg & WindowMessages.WM_REFLECT) == WindowMessages.WM_REFLECT))
            {
                string subtext = MsgToString(msg - WindowMessages.WM_REFLECT) ?? "???";

                text = "WM_REFLECT + " + subtext;
            }

            return text;
        }

        private static string Parenthesize(string input)
        {
            if (input == null)
            {
                return string.Empty;
            }

            return " (" + input + ")";
        }

        public static string ToString(Message message)
        {
            return ToString(message.HWnd, message.Msg, message.WParam, message.LParam, message.Result);
        }

        public static string ToString(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam, IntPtr result)
        {
            string id = Parenthesize(MsgToString(msg));

            string lDescription = string.Empty;
            if (msg == WindowMessages.WM_PARENTNOTIFY)
            {
                lDescription = Parenthesize(MsgToString(NativeMethods.Util.LOWORD(wparam)));
            }

            return
                "msg=0x" + Convert.ToString(msg, 16) + id +
                " hwnd=0x" + Convert.ToString((long)hWnd, 16) +
                " wparam=0x" + Convert.ToString((long)wparam, 16) +
                " lparam=0x" + Convert.ToString((long)lparam, 16) + lDescription +
                " result=0x" + Convert.ToString((long)result, 16);
        }
    }
}

