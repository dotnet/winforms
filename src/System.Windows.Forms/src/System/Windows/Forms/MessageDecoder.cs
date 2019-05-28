// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms
{
    /// <summary>
    /// Decodes Windows messages. This is in a separate class from Message so we can avoid
    /// loading it in the 99% case where we don't need it.
    /// </summary>
    internal static class MessageDecoder
    {
        /// <summary>
        /// Returns the symbolic name of the msg value, or null if it isn't one of the
        /// existing constants.
        /// </summary>
        private static string MsgToString(int msg)
        {
            string text;
            switch (msg)
            {
                case Interop.WindowMessages.WM_NULL:
                    text = "WM_NULL";
                    break;
                case Interop.WindowMessages.WM_CREATE:
                    text = "WM_CREATE";
                    break;
                case Interop.WindowMessages.WM_DESTROY:
                    text = "WM_DESTROY";
                    break;
                case Interop.WindowMessages.WM_MOVE:
                    text = "WM_MOVE";
                    break;
                case Interop.WindowMessages.WM_SIZE:
                    text = "WM_SIZE";
                    break;
                case Interop.WindowMessages.WM_ACTIVATE:
                    text = "WM_ACTIVATE";
                    break;
                case Interop.WindowMessages.WM_SETFOCUS:
                    text = "WM_SETFOCUS";
                    break;
                case Interop.WindowMessages.WM_KILLFOCUS:
                    text = "WM_KILLFOCUS";
                    break;
                case Interop.WindowMessages.WM_ENABLE:
                    text = "WM_ENABLE";
                    break;
                case Interop.WindowMessages.WM_SETREDRAW:
                    text = "WM_SETREDRAW";
                    break;
                case Interop.WindowMessages.WM_SETTEXT:
                    text = "WM_SETTEXT";
                    break;
                case Interop.WindowMessages.WM_GETTEXT:
                    text = "WM_GETTEXT";
                    break;
                case Interop.WindowMessages.WM_GETTEXTLENGTH:
                    text = "WM_GETTEXTLENGTH";
                    break;
                case Interop.WindowMessages.WM_PAINT:
                    text = "WM_PAINT";
                    break;
                case Interop.WindowMessages.WM_CLOSE:
                    text = "WM_CLOSE";
                    break;
                case Interop.WindowMessages.WM_QUERYENDSESSION:
                    text = "WM_QUERYENDSESSION";
                    break;
                case Interop.WindowMessages.WM_QUIT:
                    text = "WM_QUIT";
                    break;
                case Interop.WindowMessages.WM_QUERYOPEN:
                    text = "WM_QUERYOPEN";
                    break;
                case Interop.WindowMessages.WM_ERASEBKGND:
                    text = "WM_ERASEBKGND";
                    break;
                case Interop.WindowMessages.WM_SYSCOLORCHANGE:
                    text = "WM_SYSCOLORCHANGE";
                    break;
                case Interop.WindowMessages.WM_ENDSESSION:
                    text = "WM_ENDSESSION";
                    break;
                case Interop.WindowMessages.WM_SHOWWINDOW:
                    text = "WM_SHOWWINDOW";
                    break;
                case Interop.WindowMessages.WM_WININICHANGE:
                    text = "WM_WININICHANGE";
                    break;
                case Interop.WindowMessages.WM_DEVMODECHANGE:
                    text = "WM_DEVMODECHANGE";
                    break;
                case Interop.WindowMessages.WM_ACTIVATEAPP:
                    text = "WM_ACTIVATEAPP";
                    break;
                case Interop.WindowMessages.WM_FONTCHANGE:
                    text = "WM_FONTCHANGE";
                    break;
                case Interop.WindowMessages.WM_TIMECHANGE:
                    text = "WM_TIMECHANGE";
                    break;
                case Interop.WindowMessages.WM_CANCELMODE:
                    text = "WM_CANCELMODE";
                    break;
                case Interop.WindowMessages.WM_SETCURSOR:
                    text = "WM_SETCURSOR";
                    break;
                case Interop.WindowMessages.WM_MOUSEACTIVATE:
                    text = "WM_MOUSEACTIVATE";
                    break;
                case Interop.WindowMessages.WM_CHILDACTIVATE:
                    text = "WM_CHILDACTIVATE";
                    break;
                case Interop.WindowMessages.WM_QUEUESYNC:
                    text = "WM_QUEUESYNC";
                    break;
                case Interop.WindowMessages.WM_GETMINMAXINFO:
                    text = "WM_GETMINMAXINFO";
                    break;
                case Interop.WindowMessages.WM_PAINTICON:
                    text = "WM_PAINTICON";
                    break;
                case Interop.WindowMessages.WM_ICONERASEBKGND:
                    text = "WM_ICONERASEBKGND";
                    break;
                case Interop.WindowMessages.WM_NEXTDLGCTL:
                    text = "WM_NEXTDLGCTL";
                    break;
                case Interop.WindowMessages.WM_SPOOLERSTATUS:
                    text = "WM_SPOOLERSTATUS";
                    break;
                case Interop.WindowMessages.WM_DRAWITEM:
                    text = "WM_DRAWITEM";
                    break;
                case Interop.WindowMessages.WM_MEASUREITEM:
                    text = "WM_MEASUREITEM";
                    break;
                case Interop.WindowMessages.WM_DELETEITEM:
                    text = "WM_DELETEITEM";
                    break;
                case Interop.WindowMessages.WM_VKEYTOITEM:
                    text = "WM_VKEYTOITEM";
                    break;
                case Interop.WindowMessages.WM_CHARTOITEM:
                    text = "WM_CHARTOITEM";
                    break;
                case Interop.WindowMessages.WM_SETFONT:
                    text = "WM_SETFONT";
                    break;
                case Interop.WindowMessages.WM_GETFONT:
                    text = "WM_GETFONT";
                    break;
                case Interop.WindowMessages.WM_SETHOTKEY:
                    text = "WM_SETHOTKEY";
                    break;
                case Interop.WindowMessages.WM_GETHOTKEY:
                    text = "WM_GETHOTKEY";
                    break;
                case Interop.WindowMessages.WM_QUERYDRAGICON:
                    text = "WM_QUERYDRAGICON";
                    break;
                case Interop.WindowMessages.WM_COMPAREITEM:
                    text = "WM_COMPAREITEM";
                    break;
                case Interop.WindowMessages.WM_GETOBJECT:
                    text = "WM_GETOBJECT";
                    break;
                case Interop.WindowMessages.WM_COMPACTING:
                    text = "WM_COMPACTING";
                    break;
                case Interop.WindowMessages.WM_COMMNOTIFY:
                    text = "WM_COMMNOTIFY";
                    break;
                case Interop.WindowMessages.WM_WINDOWPOSCHANGING:
                    text = "WM_WINDOWPOSCHANGING";
                    break;
                case Interop.WindowMessages.WM_WINDOWPOSCHANGED:
                    text = "WM_WINDOWPOSCHANGED";
                    break;
                case Interop.WindowMessages.WM_POWER:
                    text = "WM_POWER";
                    break;
                case Interop.WindowMessages.WM_COPYDATA:
                    text = "WM_COPYDATA";
                    break;
                case Interop.WindowMessages.WM_CANCELJOURNAL:
                    text = "WM_CANCELJOURNAL";
                    break;
                case Interop.WindowMessages.WM_NOTIFY:
                    text = "WM_NOTIFY";
                    break;
                case Interop.WindowMessages.WM_INPUTLANGCHANGEREQUEST:
                    text = "WM_INPUTLANGCHANGEREQUEST";
                    break;
                case Interop.WindowMessages.WM_INPUTLANGCHANGE:
                    text = "WM_INPUTLANGCHANGE";
                    break;
                case Interop.WindowMessages.WM_TCARD:
                    text = "WM_TCARD";
                    break;
                case Interop.WindowMessages.WM_HELP:
                    text = "WM_HELP";
                    break;
                case Interop.WindowMessages.WM_USERCHANGED:
                    text = "WM_USERCHANGED";
                    break;
                case Interop.WindowMessages.WM_NOTIFYFORMAT:
                    text = "WM_NOTIFYFORMAT";
                    break;
                case Interop.WindowMessages.WM_CONTEXTMENU:
                    text = "WM_CONTEXTMENU";
                    break;
                case Interop.WindowMessages.WM_STYLECHANGING:
                    text = "WM_STYLECHANGING";
                    break;
                case Interop.WindowMessages.WM_STYLECHANGED:
                    text = "WM_STYLECHANGED";
                    break;
                case Interop.WindowMessages.WM_DISPLAYCHANGE:
                    text = "WM_DISPLAYCHANGE";
                    break;
                case Interop.WindowMessages.WM_GETICON:
                    text = "WM_GETICON";
                    break;
                case Interop.WindowMessages.WM_SETICON:
                    text = "WM_SETICON";
                    break;
                case Interop.WindowMessages.WM_NCCREATE:
                    text = "WM_NCCREATE";
                    break;
                case Interop.WindowMessages.WM_NCDESTROY:
                    text = "WM_NCDESTROY";
                    break;
                case Interop.WindowMessages.WM_NCCALCSIZE:
                    text = "WM_NCCALCSIZE";
                    break;
                case Interop.WindowMessages.WM_NCHITTEST:
                    text = "WM_NCHITTEST";
                    break;
                case Interop.WindowMessages.WM_NCPAINT:
                    text = "WM_NCPAINT";
                    break;
                case Interop.WindowMessages.WM_NCACTIVATE:
                    text = "WM_NCACTIVATE";
                    break;
                case Interop.WindowMessages.WM_GETDLGCODE:
                    text = "WM_GETDLGCODE";
                    break;
                case Interop.WindowMessages.WM_NCMOUSEMOVE:
                    text = "WM_NCMOUSEMOVE";
                    break;
                case Interop.WindowMessages.WM_NCLBUTTONDOWN:
                    text = "WM_NCLBUTTONDOWN";
                    break;
                case Interop.WindowMessages.WM_NCLBUTTONUP:
                    text = "WM_NCLBUTTONUP";
                    break;
                case Interop.WindowMessages.WM_NCLBUTTONDBLCLK:
                    text = "WM_NCLBUTTONDBLCLK";
                    break;
                case Interop.WindowMessages.WM_NCRBUTTONDOWN:
                    text = "WM_NCRBUTTONDOWN";
                    break;
                case Interop.WindowMessages.WM_NCRBUTTONUP:
                    text = "WM_NCRBUTTONUP";
                    break;
                case Interop.WindowMessages.WM_NCRBUTTONDBLCLK:
                    text = "WM_NCRBUTTONDBLCLK";
                    break;
                case Interop.WindowMessages.WM_NCMBUTTONDOWN:
                    text = "WM_NCMBUTTONDOWN";
                    break;
                case Interop.WindowMessages.WM_NCMBUTTONUP:
                    text = "WM_NCMBUTTONUP";
                    break;
                case Interop.WindowMessages.WM_NCMBUTTONDBLCLK:
                    text = "WM_NCMBUTTONDBLCLK";
                    break;
                case Interop.WindowMessages.WM_KEYDOWN:
                    text = "WM_KEYDOWN";
                    break;
                case Interop.WindowMessages.WM_KEYUP:
                    text = "WM_KEYUP";
                    break;
                case Interop.WindowMessages.WM_CHAR:
                    text = "WM_CHAR";
                    break;
                case Interop.WindowMessages.WM_DEADCHAR:
                    text = "WM_DEADCHAR";
                    break;
                case Interop.WindowMessages.WM_SYSKEYDOWN:
                    text = "WM_SYSKEYDOWN";
                    break;
                case Interop.WindowMessages.WM_SYSKEYUP:
                    text = "WM_SYSKEYUP";
                    break;
                case Interop.WindowMessages.WM_SYSCHAR:
                    text = "WM_SYSCHAR";
                    break;
                case Interop.WindowMessages.WM_SYSDEADCHAR:
                    text = "WM_SYSDEADCHAR";
                    break;
                case Interop.WindowMessages.WM_KEYLAST:
                    text = "WM_KEYLAST";
                    break;
                case Interop.WindowMessages.WM_IME_STARTCOMPOSITION:
                    text = "WM_IME_STARTCOMPOSITION";
                    break;
                case Interop.WindowMessages.WM_IME_ENDCOMPOSITION:
                    text = "WM_IME_ENDCOMPOSITION";
                    break;
                case Interop.WindowMessages.WM_IME_COMPOSITION:
                    text = "WM_IME_COMPOSITION";
                    break;
                case Interop.WindowMessages.WM_INITDIALOG:
                    text = "WM_INITDIALOG";
                    break;
                case Interop.WindowMessages.WM_COMMAND:
                    text = "WM_COMMAND";
                    break;
                case Interop.WindowMessages.WM_SYSCOMMAND:
                    text = "WM_SYSCOMMAND";
                    break;
                case Interop.WindowMessages.WM_TIMER:
                    text = "WM_TIMER";
                    break;
                case Interop.WindowMessages.WM_HSCROLL:
                    text = "WM_HSCROLL";
                    break;
                case Interop.WindowMessages.WM_VSCROLL:
                    text = "WM_VSCROLL";
                    break;
                case Interop.WindowMessages.WM_INITMENU:
                    text = "WM_INITMENU";
                    break;
                case Interop.WindowMessages.WM_INITMENUPOPUP:
                    text = "WM_INITMENUPOPUP";
                    break;
                case Interop.WindowMessages.WM_MENUSELECT:
                    text = "WM_MENUSELECT";
                    break;
                case Interop.WindowMessages.WM_MENUCHAR:
                    text = "WM_MENUCHAR";
                    break;
                case Interop.WindowMessages.WM_ENTERIDLE:
                    text = "WM_ENTERIDLE";
                    break;
                case Interop.WindowMessages.WM_CTLCOLORMSGBOX:
                    text = "WM_CTLCOLORMSGBOX";
                    break;
                case Interop.WindowMessages.WM_CTLCOLOREDIT:
                    text = "WM_CTLCOLOREDIT";
                    break;
                case Interop.WindowMessages.WM_CTLCOLORLISTBOX:
                    text = "WM_CTLCOLORLISTBOX";
                    break;
                case Interop.WindowMessages.WM_CTLCOLORBTN:
                    text = "WM_CTLCOLORBTN";
                    break;
                case Interop.WindowMessages.WM_CTLCOLORDLG:
                    text = "WM_CTLCOLORDLG";
                    break;
                case Interop.WindowMessages.WM_CTLCOLORSCROLLBAR:
                    text = "WM_CTLCOLORSCROLLBAR";
                    break;
                case Interop.WindowMessages.WM_CTLCOLORSTATIC:
                    text = "WM_CTLCOLORSTATIC";
                    break;
                case Interop.WindowMessages.WM_MOUSEMOVE:
                    text = "WM_MOUSEMOVE";
                    break;
                case Interop.WindowMessages.WM_LBUTTONDOWN:
                    text = "WM_LBUTTONDOWN";
                    break;
                case Interop.WindowMessages.WM_LBUTTONUP:
                    text = "WM_LBUTTONUP";
                    break;
                case Interop.WindowMessages.WM_LBUTTONDBLCLK:
                    text = "WM_LBUTTONDBLCLK";
                    break;
                case Interop.WindowMessages.WM_RBUTTONDOWN:
                    text = "WM_RBUTTONDOWN";
                    break;
                case Interop.WindowMessages.WM_RBUTTONUP:
                    text = "WM_RBUTTONUP";
                    break;
                case Interop.WindowMessages.WM_RBUTTONDBLCLK:
                    text = "WM_RBUTTONDBLCLK";
                    break;
                case Interop.WindowMessages.WM_MBUTTONDOWN:
                    text = "WM_MBUTTONDOWN";
                    break;
                case Interop.WindowMessages.WM_MBUTTONUP:
                    text = "WM_MBUTTONUP";
                    break;
                case Interop.WindowMessages.WM_MBUTTONDBLCLK:
                    text = "WM_MBUTTONDBLCLK";
                    break;
                case Interop.WindowMessages.WM_MOUSEWHEEL:
                    text = "WM_MOUSEWHEEL";
                    break;
                case Interop.WindowMessages.WM_PARENTNOTIFY:
                    text = "WM_PARENTNOTIFY";
                    break;
                case Interop.WindowMessages.WM_ENTERMENULOOP:
                    text = "WM_ENTERMENULOOP";
                    break;
                case Interop.WindowMessages.WM_EXITMENULOOP:
                    text = "WM_EXITMENULOOP";
                    break;
                case Interop.WindowMessages.WM_NEXTMENU:
                    text = "WM_NEXTMENU";
                    break;
                case Interop.WindowMessages.WM_SIZING:
                    text = "WM_SIZING";
                    break;
                case Interop.WindowMessages.WM_CAPTURECHANGED:
                    text = "WM_CAPTURECHANGED";
                    break;
                case Interop.WindowMessages.WM_MOVING:
                    text = "WM_MOVING";
                    break;
                case Interop.WindowMessages.WM_POWERBROADCAST:
                    text = "WM_POWERBROADCAST";
                    break;
                case Interop.WindowMessages.WM_DEVICECHANGE:
                    text = "WM_DEVICECHANGE";
                    break;
                case Interop.WindowMessages.WM_IME_SETCONTEXT:
                    text = "WM_IME_SETCONTEXT";
                    break;
                case Interop.WindowMessages.WM_IME_NOTIFY:
                    text = "WM_IME_NOTIFY";
                    break;
                case Interop.WindowMessages.WM_IME_CONTROL:
                    text = "WM_IME_CONTROL";
                    break;
                case Interop.WindowMessages.WM_IME_COMPOSITIONFULL:
                    text = "WM_IME_COMPOSITIONFULL";
                    break;
                case Interop.WindowMessages.WM_IME_SELECT:
                    text = "WM_IME_SELECT";
                    break;
                case Interop.WindowMessages.WM_IME_CHAR:
                    text = "WM_IME_CHAR";
                    break;
                case Interop.WindowMessages.WM_IME_KEYDOWN:
                    text = "WM_IME_KEYDOWN";
                    break;
                case Interop.WindowMessages.WM_IME_KEYUP:
                    text = "WM_IME_KEYUP";
                    break;
                case Interop.WindowMessages.WM_MDICREATE:
                    text = "WM_MDICREATE";
                    break;
                case Interop.WindowMessages.WM_MDIDESTROY:
                    text = "WM_MDIDESTROY";
                    break;
                case Interop.WindowMessages.WM_MDIACTIVATE:
                    text = "WM_MDIACTIVATE";
                    break;
                case Interop.WindowMessages.WM_MDIRESTORE:
                    text = "WM_MDIRESTORE";
                    break;
                case Interop.WindowMessages.WM_MDINEXT:
                    text = "WM_MDINEXT";
                    break;
                case Interop.WindowMessages.WM_MDIMAXIMIZE:
                    text = "WM_MDIMAXIMIZE";
                    break;
                case Interop.WindowMessages.WM_MDITILE:
                    text = "WM_MDITILE";
                    break;
                case Interop.WindowMessages.WM_MDICASCADE:
                    text = "WM_MDICASCADE";
                    break;
                case Interop.WindowMessages.WM_MDIICONARRANGE:
                    text = "WM_MDIICONARRANGE";
                    break;
                case Interop.WindowMessages.WM_MDIGETACTIVE:
                    text = "WM_MDIGETACTIVE";
                    break;
                case Interop.WindowMessages.WM_MDISETMENU:
                    text = "WM_MDISETMENU";
                    break;
                case Interop.WindowMessages.WM_ENTERSIZEMOVE:
                    text = "WM_ENTERSIZEMOVE";
                    break;
                case Interop.WindowMessages.WM_EXITSIZEMOVE:
                    text = "WM_EXITSIZEMOVE";
                    break;
                case Interop.WindowMessages.WM_DROPFILES:
                    text = "WM_DROPFILES";
                    break;
                case Interop.WindowMessages.WM_MDIREFRESHMENU:
                    text = "WM_MDIREFRESHMENU";
                    break;
                case Interop.WindowMessages.WM_MOUSEHOVER:
                    text = "WM_MOUSEHOVER";
                    break;
                case Interop.WindowMessages.WM_MOUSELEAVE:
                    text = "WM_MOUSELEAVE";
                    break;
                case Interop.WindowMessages.WM_CUT:
                    text = "WM_CUT";
                    break;
                case Interop.WindowMessages.WM_COPY:
                    text = "WM_COPY";
                    break;
                case Interop.WindowMessages.WM_PASTE:
                    text = "WM_PASTE";
                    break;
                case Interop.WindowMessages.WM_CLEAR:
                    text = "WM_CLEAR";
                    break;
                case Interop.WindowMessages.WM_UNDO:
                    text = "WM_UNDO";
                    break;
                case Interop.WindowMessages.WM_RENDERFORMAT:
                    text = "WM_RENDERFORMAT";
                    break;
                case Interop.WindowMessages.WM_RENDERALLFORMATS:
                    text = "WM_RENDERALLFORMATS";
                    break;
                case Interop.WindowMessages.WM_DESTROYCLIPBOARD:
                    text = "WM_DESTROYCLIPBOARD";
                    break;
                case Interop.WindowMessages.WM_DRAWCLIPBOARD:
                    text = "WM_DRAWCLIPBOARD";
                    break;
                case Interop.WindowMessages.WM_PAINTCLIPBOARD:
                    text = "WM_PAINTCLIPBOARD";
                    break;
                case Interop.WindowMessages.WM_VSCROLLCLIPBOARD:
                    text = "WM_VSCROLLCLIPBOARD";
                    break;
                case Interop.WindowMessages.WM_SIZECLIPBOARD:
                    text = "WM_SIZECLIPBOARD";
                    break;
                case Interop.WindowMessages.WM_ASKCBFORMATNAME:
                    text = "WM_ASKCBFORMATNAME";
                    break;
                case Interop.WindowMessages.WM_CHANGECBCHAIN:
                    text = "WM_CHANGECBCHAIN";
                    break;
                case Interop.WindowMessages.WM_HSCROLLCLIPBOARD:
                    text = "WM_HSCROLLCLIPBOARD";
                    break;
                case Interop.WindowMessages.WM_QUERYNEWPALETTE:
                    text = "WM_QUERYNEWPALETTE";
                    break;
                case Interop.WindowMessages.WM_PALETTEISCHANGING:
                    text = "WM_PALETTEISCHANGING";
                    break;
                case Interop.WindowMessages.WM_PALETTECHANGED:
                    text = "WM_PALETTECHANGED";
                    break;
                case Interop.WindowMessages.WM_HOTKEY:
                    text = "WM_HOTKEY";
                    break;
                case Interop.WindowMessages.WM_PRINT:
                    text = "WM_PRINT";
                    break;
                case Interop.WindowMessages.WM_PRINTCLIENT:
                    text = "WM_PRINTCLIENT";
                    break;
                case Interop.WindowMessages.WM_HANDHELDFIRST:
                    text = "WM_HANDHELDFIRST";
                    break;
                case Interop.WindowMessages.WM_HANDHELDLAST:
                    text = "WM_HANDHELDLAST";
                    break;
                case Interop.WindowMessages.WM_AFXFIRST:
                    text = "WM_AFXFIRST";
                    break;
                case Interop.WindowMessages.WM_AFXLAST:
                    text = "WM_AFXLAST";
                    break;
                case Interop.WindowMessages.WM_PENWINFIRST:
                    text = "WM_PENWINFIRST";
                    break;
                case Interop.WindowMessages.WM_PENWINLAST:
                    text = "WM_PENWINLAST";
                    break;
                case Interop.WindowMessages.WM_APP:
                    text = "WM_APP";
                    break;
                case Interop.WindowMessages.WM_USER:
                    text = "WM_USER";
                    break;
                case Interop.WindowMessages.WM_CTLCOLOR:
                    text = "WM_CTLCOLOR";
                    break;

                // RichEdit messages
                case Interop.EditMessages.EM_GETLIMITTEXT:
                    text = "EM_GETLIMITTEXT";
                    break;
                case Interop.EditMessages.EM_POSFROMCHAR:
                    text = "EM_POSFROMCHAR";
                    break;
                case Interop.EditMessages.EM_CHARFROMPOS:
                    text = "EM_CHARFROMPOS";
                    break;
                case Interop.EditMessages.EM_SCROLLCARET:
                    text = "EM_SCROLLCARET";
                    break;
                case Interop.EditMessages.EM_CANPASTE:
                    text = "EM_CANPASTE";
                    break;
                case Interop.EditMessages.EM_DISPLAYBAND:
                    text = "EM_DISPLAYBAND";
                    break;
                case Interop.EditMessages.EM_EXGETSEL:
                    text = "EM_EXGETSEL";
                    break;
                case Interop.EditMessages.EM_EXLIMITTEXT:
                    text = "EM_EXLIMITTEXT";
                    break;
                case Interop.EditMessages.EM_EXLINEFROMCHAR:
                    text = "EM_EXLINEFROMCHAR";
                    break;
                case Interop.EditMessages.EM_EXSETSEL:
                    text = "EM_EXSETSEL";
                    break;
                case Interop.EditMessages.EM_FINDTEXT:
                    text = "EM_FINDTEXT";
                    break;
                case Interop.EditMessages.EM_FORMATRANGE:
                    text = "EM_FORMATRANGE";
                    break;
                case Interop.EditMessages.EM_GETCHARFORMAT:
                    text = "EM_GETCHARFORMAT";
                    break;
                case Interop.EditMessages.EM_GETEVENTMASK:
                    text = "EM_GETEVENTMASK";
                    break;
                case Interop.EditMessages.EM_GETOLEINTERFACE:
                    text = "EM_GETOLEINTERFACE";
                    break;
                case Interop.EditMessages.EM_GETPARAFORMAT:
                    text = "EM_GETPARAFORMAT";
                    break;
                case Interop.EditMessages.EM_GETSELTEXT:
                    text = "EM_GETSELTEXT";
                    break;
                case Interop.EditMessages.EM_HIDESELECTION:
                    text = "EM_HIDESELECTION";
                    break;
                case Interop.EditMessages.EM_PASTESPECIAL:
                    text = "EM_PASTESPECIAL";
                    break;
                case Interop.EditMessages.EM_REQUESTRESIZE:
                    text = "EM_REQUESTRESIZE";
                    break;
                case Interop.EditMessages.EM_SELECTIONTYPE:
                    text = "EM_SELECTIONTYPE";
                    break;
                case Interop.EditMessages.EM_SETBKGNDCOLOR:
                    text = "EM_SETBKGNDCOLOR";
                    break;
                case Interop.EditMessages.EM_SETCHARFORMAT:
                    text = "EM_SETCHARFORMAT";
                    break;
                case Interop.EditMessages.EM_SETEVENTMASK:
                    text = "EM_SETEVENTMASK";
                    break;
                case Interop.EditMessages.EM_SETOLECALLBACK:
                    text = "EM_SETOLECALLBACK";
                    break;
                case Interop.EditMessages.EM_SETPARAFORMAT:
                    text = "EM_SETPARAFORMAT";
                    break;
                case Interop.EditMessages.EM_SETTARGETDEVICE:
                    text = "EM_SETTARGETDEVICE";
                    break;
                case Interop.EditMessages.EM_STREAMIN:
                    text = "EM_STREAMIN";
                    break;
                case Interop.EditMessages.EM_STREAMOUT:
                    text = "EM_STREAMOUT";
                    break;
                case Interop.EditMessages.EM_GETTEXTRANGE:
                    text = "EM_GETTEXTRANGE";
                    break;
                case Interop.EditMessages.EM_FINDWORDBREAK:
                    text = "EM_FINDWORDBREAK";
                    break;
                case Interop.EditMessages.EM_SETOPTIONS:
                    text = "EM_SETOPTIONS";
                    break;
                case Interop.EditMessages.EM_GETOPTIONS:
                    text = "EM_GETOPTIONS";
                    break;
                case Interop.EditMessages.EM_FINDTEXTEX:
                    text = "EM_FINDTEXTEX";
                    break;
                case Interop.EditMessages.EM_GETWORDBREAKPROCEX:
                    text = "EM_GETWORDBREAKPROCEX";
                    break;
                case Interop.EditMessages.EM_SETWORDBREAKPROCEX:
                    text = "EM_SETWORDBREAKPROCEX";
                    break;

                // Richedit v2.0 messages
                case Interop.EditMessages.EM_SETUNDOLIMIT:
                    text = "EM_SETUNDOLIMIT";
                    break;
                case Interop.EditMessages.EM_REDO:
                    text = "EM_REDO";
                    break;
                case Interop.EditMessages.EM_CANREDO:
                    text = "EM_CANREDO";
                    break;
                case Interop.EditMessages.EM_GETUNDONAME:
                    text = "EM_GETUNDONAME";
                    break;
                case Interop.EditMessages.EM_GETREDONAME:
                    text = "EM_GETREDONAME";
                    break;
                case Interop.EditMessages.EM_STOPGROUPTYPING:
                    text = "EM_STOPGROUPTYPING";
                    break;
                case Interop.EditMessages.EM_SETTEXTMODE:
                    text = "EM_SETTEXTMODE";
                    break;
                case Interop.EditMessages.EM_GETTEXTMODE:
                    text = "EM_GETTEXTMODE";
                    break;
                case Interop.EditMessages.EM_AUTOURLDETECT:
                    text = "EM_AUTOURLDETECT";
                    break;
                case Interop.EditMessages.EM_GETAUTOURLDETECT:
                    text = "EM_GETAUTOURLDETECT";
                    break;
                case Interop.EditMessages.EM_SETPALETTE:
                    text = "EM_SETPALETTE";
                    break;
                case Interop.EditMessages.EM_GETTEXTEX:
                    text = "EM_GETTEXTEX";
                    break;
                case Interop.EditMessages.EM_GETTEXTLENGTHEX:
                    text = "EM_GETTEXTLENGTHEX";
                    break;

                // Asia specific messages
                case Interop.EditMessages.EM_SETPUNCTUATION:
                    text = "EM_SETPUNCTUATION";
                    break;
                case Interop.EditMessages.EM_GETPUNCTUATION:
                    text = "EM_GETPUNCTUATION";
                    break;
                case Interop.EditMessages.EM_SETWORDWRAPMODE:
                    text = "EM_SETWORDWRAPMODE";
                    break;
                case Interop.EditMessages.EM_GETWORDWRAPMODE:
                    text = "EM_GETWORDWRAPMODE";
                    break;
                case Interop.EditMessages.EM_SETIMECOLOR:
                    text = "EM_SETIMECOLOR";
                    break;
                case Interop.EditMessages.EM_GETIMECOLOR:
                    text = "EM_GETIMECOLOR";
                    break;
                case Interop.EditMessages.EM_SETIMEOPTIONS:
                    text = "EM_SETIMEOPTIONS";
                    break;
                case Interop.EditMessages.EM_GETIMEOPTIONS:
                    text = "EM_GETIMEOPTIONS";
                    break;
                case Interop.EditMessages.EM_CONVPOSITION:
                    text = "EM_CONVPOSITION";
                    break;
                case Interop.EditMessages.EM_SETLANGOPTIONS:
                    text = "EM_SETLANGOPTIONS";
                    break;
                case Interop.EditMessages.EM_GETLANGOPTIONS:
                    text = "EM_GETLANGOPTIONS";
                    break;
                case Interop.EditMessages.EM_GETIMECOMPMODE:
                    text = "EM_GETIMECOMPMODE";
                    break;
                case Interop.EditMessages.EM_FINDTEXTW:
                    text = "EM_FINDTEXTW";
                    break;
                case Interop.EditMessages.EM_FINDTEXTEXW:
                    text = "EM_FINDTEXTEXW";
                    break;

                // Rich Edit 3.0 Asia msgs
                case Interop.EditMessages.EM_RECONVERSION:
                    text = "EM_RECONVERSION";
                    break;
                case Interop.EditMessages.EM_SETIMEMODEBIAS:
                    text = "EM_SETIMEMODEBIAS";
                    break;
                case Interop.EditMessages.EM_GETIMEMODEBIAS:
                    text = "EM_GETIMEMODEBIAS";
                    break;

                // BiDi Specific messages
                case Interop.EditMessages.EM_SETBIDIOPTIONS:
                    text = "EM_SETBIDIOPTIONS";
                    break;
                case Interop.EditMessages.EM_GETBIDIOPTIONS:
                    text = "EM_GETBIDIOPTIONS";
                    break;
                case Interop.EditMessages.EM_SETTYPOGRAPHYOPTIONS:
                    text = "EM_SETTYPOGRAPHYOPTIONS";
                    break;
                case Interop.EditMessages.EM_GETTYPOGRAPHYOPTIONS:
                    text = "EM_GETTYPOGRAPHYOPTIONS";
                    break;

                // Extended Edit style specific messages
                case Interop.EditMessages.EM_SETEDITSTYLE:
                    text = "EM_SETEDITSTYLE";
                    break;
                case Interop.EditMessages.EM_GETEDITSTYLE:
                    text = "EM_GETEDITSTYLE";
                    break;

                default:
                    text = null;
                    break;
            }

            if (text == null && ((msg & Interop.WindowMessages.WM_REFLECT) == Interop.WindowMessages.WM_REFLECT))
            {
                string subtext = MsgToString(msg - Interop.WindowMessages.WM_REFLECT) ?? "???";

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
            if (msg == Interop.WindowMessages.WM_PARENTNOTIFY)
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

