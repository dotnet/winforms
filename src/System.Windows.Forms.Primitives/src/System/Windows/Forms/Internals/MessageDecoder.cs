// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;
using static Interop.User32;

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
        private static string? MsgToString(int msg)
        {
            string? text;
            switch (msg)
            {
                case (int)WM.NULL:
                    text = "WM_NULL";
                    break;
                case (int)WM.CREATE:
                    text = "WM_CREATE";
                    break;
                case (int)WM.DESTROY:
                    text = "WM_DESTROY";
                    break;
                case (int)WM.MOVE:
                    text = "WM_MOVE";
                    break;
                case (int)WM.SIZE:
                    text = "WM_SIZE";
                    break;
                case (int)WM.ACTIVATE:
                    text = "WM_ACTIVATE";
                    break;
                case (int)WM.SETFOCUS:
                    text = "WM_SETFOCUS";
                    break;
                case (int)WM.KILLFOCUS:
                    text = "WM_KILLFOCUS";
                    break;
                case (int)WM.ENABLE:
                    text = "WM_ENABLE";
                    break;
                case (int)WM.SETREDRAW:
                    text = "WM_SETREDRAW";
                    break;
                case (int)WM.SETTEXT:
                    text = "WM_SETTEXT";
                    break;
                case (int)WM.GETTEXT:
                    text = "WM_GETTEXT";
                    break;
                case (int)WM.GETTEXTLENGTH:
                    text = "WM_GETTEXTLENGTH";
                    break;
                case (int)WM.PAINT:
                    text = "WM_PAINT";
                    break;
                case (int)WM.CLOSE:
                    text = "WM_CLOSE";
                    break;
                case (int)WM.QUERYENDSESSION:
                    text = "WM_QUERYENDSESSION";
                    break;
                case (int)WM.QUIT:
                    text = "WM_QUIT";
                    break;
                case (int)WM.QUERYOPEN:
                    text = "WM_QUERYOPEN";
                    break;
                case (int)WM.ERASEBKGND:
                    text = "WM_ERASEBKGND";
                    break;
                case (int)WM.SYSCOLORCHANGE:
                    text = "WM_SYSCOLORCHANGE";
                    break;
                case (int)WM.ENDSESSION:
                    text = "WM_ENDSESSION";
                    break;
                case (int)WM.SHOWWINDOW:
                    text = "WM_SHOWWINDOW";
                    break;
                case (int)WM.WININICHANGE:
                    text = "WM_WININICHANGE";
                    break;
                case (int)WM.DEVMODECHANGE:
                    text = "WM_DEVMODECHANGE";
                    break;
                case (int)WM.ACTIVATEAPP:
                    text = "WM_ACTIVATEAPP";
                    break;
                case (int)WM.FONTCHANGE:
                    text = "WM_FONTCHANGE";
                    break;
                case (int)WM.TIMECHANGE:
                    text = "WM_TIMECHANGE";
                    break;
                case (int)WM.CANCELMODE:
                    text = "WM_CANCELMODE";
                    break;
                case (int)WM.SETCURSOR:
                    text = "WM_SETCURSOR";
                    break;
                case (int)WM.MOUSEACTIVATE:
                    text = "WM_MOUSEACTIVATE";
                    break;
                case (int)WM.CHILDACTIVATE:
                    text = "WM_CHILDACTIVATE";
                    break;
                case (int)WM.QUEUESYNC:
                    text = "WM_QUEUESYNC";
                    break;
                case (int)WM.GETMINMAXINFO:
                    text = "WM_GETMINMAXINFO";
                    break;
                case (int)WM.PAINTICON:
                    text = "WM_PAINTICON";
                    break;
                case (int)WM.ICONERASEBKGND:
                    text = "WM_ICONERASEBKGND";
                    break;
                case (int)WM.NEXTDLGCTL:
                    text = "WM_NEXTDLGCTL";
                    break;
                case (int)WM.SPOOLERSTATUS:
                    text = "WM_SPOOLERSTATUS";
                    break;
                case (int)WM.DRAWITEM:
                    text = "WM_DRAWITEM";
                    break;
                case (int)WM.MEASUREITEM:
                    text = "WM_MEASUREITEM";
                    break;
                case (int)WM.DELETEITEM:
                    text = "WM_DELETEITEM";
                    break;
                case (int)WM.VKEYTOITEM:
                    text = "WM_VKEYTOITEM";
                    break;
                case (int)WM.CHARTOITEM:
                    text = "WM_CHARTOITEM";
                    break;
                case (int)WM.SETFONT:
                    text = "WM_SETFONT";
                    break;
                case (int)WM.GETFONT:
                    text = "WM_GETFONT";
                    break;
                case (int)WM.SETHOTKEY:
                    text = "WM_SETHOTKEY";
                    break;
                case (int)WM.GETHOTKEY:
                    text = "WM_GETHOTKEY";
                    break;
                case (int)WM.QUERYDRAGICON:
                    text = "WM_QUERYDRAGICON";
                    break;
                case (int)WM.COMPAREITEM:
                    text = "WM_COMPAREITEM";
                    break;
                case (int)WM.GETOBJECT:
                    text = "WM_GETOBJECT";
                    break;
                case (int)WM.COMPACTING:
                    text = "WM_COMPACTING";
                    break;
                case (int)WM.COMMNOTIFY:
                    text = "WM_COMMNOTIFY";
                    break;
                case (int)WM.WINDOWPOSCHANGING:
                    text = "WM_WINDOWPOSCHANGING";
                    break;
                case (int)WM.WINDOWPOSCHANGED:
                    text = "WM_WINDOWPOSCHANGED";
                    break;
                case (int)WM.POWER:
                    text = "WM_POWER";
                    break;
                case (int)WM.COPYDATA:
                    text = "WM_COPYDATA";
                    break;
                case (int)WM.CANCELJOURNAL:
                    text = "WM_CANCELJOURNAL";
                    break;
                case (int)WM.NOTIFY:
                    text = "WM_NOTIFY";
                    break;
                case (int)WM.INPUTLANGCHANGEREQUEST:
                    text = "WM_INPUTLANGCHANGEREQUEST";
                    break;
                case (int)WM.INPUTLANGCHANGE:
                    text = "WM_INPUTLANGCHANGE";
                    break;
                case (int)WM.TCARD:
                    text = "WM_TCARD";
                    break;
                case (int)WM.HELP:
                    text = "WM_HELP";
                    break;
                case (int)WM.USERCHANGED:
                    text = "WM_USERCHANGED";
                    break;
                case (int)WM.NOTIFYFORMAT:
                    text = "WM_NOTIFYFORMAT";
                    break;
                case (int)WM.CONTEXTMENU:
                    text = "WM_CONTEXTMENU";
                    break;
                case (int)WM.STYLECHANGING:
                    text = "WM_STYLECHANGING";
                    break;
                case (int)WM.STYLECHANGED:
                    text = "WM_STYLECHANGED";
                    break;
                case (int)WM.DISPLAYCHANGE:
                    text = "WM_DISPLAYCHANGE";
                    break;
                case (int)WM.GETICON:
                    text = "WM_GETICON";
                    break;
                case (int)WM.SETICON:
                    text = "WM_SETICON";
                    break;
                case (int)WM.NCCREATE:
                    text = "WM_NCCREATE";
                    break;
                case (int)WM.NCDESTROY:
                    text = "WM_NCDESTROY";
                    break;
                case (int)WM.NCCALCSIZE:
                    text = "WM_NCCALCSIZE";
                    break;
                case (int)WM.NCHITTEST:
                    text = "WM_NCHITTEST";
                    break;
                case (int)WM.NCPAINT:
                    text = "WM_NCPAINT";
                    break;
                case (int)WM.NCACTIVATE:
                    text = "WM_NCACTIVATE";
                    break;
                case (int)WM.GETDLGCODE:
                    text = "WM_GETDLGCODE";
                    break;
                case (int)WM.NCMOUSEMOVE:
                    text = "WM_NCMOUSEMOVE";
                    break;
                case (int)WM.NCLBUTTONDOWN:
                    text = "WM_NCLBUTTONDOWN";
                    break;
                case (int)WM.NCLBUTTONUP:
                    text = "WM_NCLBUTTONUP";
                    break;
                case (int)WM.NCLBUTTONDBLCLK:
                    text = "WM_NCLBUTTONDBLCLK";
                    break;
                case (int)WM.NCRBUTTONDOWN:
                    text = "WM_NCRBUTTONDOWN";
                    break;
                case (int)WM.NCRBUTTONUP:
                    text = "WM_NCRBUTTONUP";
                    break;
                case (int)WM.NCRBUTTONDBLCLK:
                    text = "WM_NCRBUTTONDBLCLK";
                    break;
                case (int)WM.NCMBUTTONDOWN:
                    text = "WM_NCMBUTTONDOWN";
                    break;
                case (int)WM.NCMBUTTONUP:
                    text = "WM_NCMBUTTONUP";
                    break;
                case (int)WM.NCMBUTTONDBLCLK:
                    text = "WM_NCMBUTTONDBLCLK";
                    break;
                case (int)WM.KEYDOWN:
                    text = "WM_KEYDOWN";
                    break;
                case (int)WM.KEYUP:
                    text = "WM_KEYUP";
                    break;
                case (int)WM.CHAR:
                    text = "WM_CHAR";
                    break;
                case (int)WM.DEADCHAR:
                    text = "WM_DEADCHAR";
                    break;
                case (int)WM.SYSKEYDOWN:
                    text = "WM_SYSKEYDOWN";
                    break;
                case (int)WM.SYSKEYUP:
                    text = "WM_SYSKEYUP";
                    break;
                case (int)WM.SYSCHAR:
                    text = "WM_SYSCHAR";
                    break;
                case (int)WM.SYSDEADCHAR:
                    text = "WM_SYSDEADCHAR";
                    break;
                case (int)WM.KEYLAST:
                    text = "WM_KEYLAST";
                    break;
                case (int)WM.IME_STARTCOMPOSITION:
                    text = "WM_IME_STARTCOMPOSITION";
                    break;
                case (int)WM.IME_ENDCOMPOSITION:
                    text = "WM_IME_ENDCOMPOSITION";
                    break;
                case (int)WM.IME_COMPOSITION:
                    text = "WM_IME_COMPOSITION";
                    break;
                case (int)WM.INITDIALOG:
                    text = "WM_INITDIALOG";
                    break;
                case (int)WM.COMMAND:
                    text = "WM_COMMAND";
                    break;
                case (int)WM.SYSCOMMAND:
                    text = "WM_SYSCOMMAND";
                    break;
                case (int)WM.TIMER:
                    text = "WM_TIMER";
                    break;
                case (int)WM.HSCROLL:
                    text = "WM_HSCROLL";
                    break;
                case (int)WM.VSCROLL:
                    text = "WM_VSCROLL";
                    break;
                case (int)WM.INITMENU:
                    text = "WM_INITMENU";
                    break;
                case (int)WM.INITMENUPOPUP:
                    text = "WM_INITMENUPOPUP";
                    break;
                case (int)WM.MENUSELECT:
                    text = "WM_MENUSELECT";
                    break;
                case (int)WM.MENUCHAR:
                    text = "WM_MENUCHAR";
                    break;
                case (int)WM.ENTERIDLE:
                    text = "WM_ENTERIDLE";
                    break;
                case (int)WM.CTLCOLORMSGBOX:
                    text = "WM_CTLCOLORMSGBOX";
                    break;
                case (int)WM.CTLCOLOREDIT:
                    text = "WM_CTLCOLOREDIT";
                    break;
                case (int)WM.CTLCOLORLISTBOX:
                    text = "WM_CTLCOLORLISTBOX";
                    break;
                case (int)WM.CTLCOLORBTN:
                    text = "WM_CTLCOLORBTN";
                    break;
                case (int)WM.CTLCOLORDLG:
                    text = "WM_CTLCOLORDLG";
                    break;
                case (int)WM.CTLCOLORSCROLLBAR:
                    text = "WM_CTLCOLORSCROLLBAR";
                    break;
                case (int)WM.CTLCOLORSTATIC:
                    text = "WM_CTLCOLORSTATIC";
                    break;
                case (int)WM.MOUSEMOVE:
                    text = "WM_MOUSEMOVE";
                    break;
                case (int)WM.LBUTTONDOWN:
                    text = "WM_LBUTTONDOWN";
                    break;
                case (int)WM.LBUTTONUP:
                    text = "WM_LBUTTONUP";
                    break;
                case (int)WM.LBUTTONDBLCLK:
                    text = "WM_LBUTTONDBLCLK";
                    break;
                case (int)WM.RBUTTONDOWN:
                    text = "WM_RBUTTONDOWN";
                    break;
                case (int)WM.RBUTTONUP:
                    text = "WM_RBUTTONUP";
                    break;
                case (int)WM.RBUTTONDBLCLK:
                    text = "WM_RBUTTONDBLCLK";
                    break;
                case (int)WM.MBUTTONDOWN:
                    text = "WM_MBUTTONDOWN";
                    break;
                case (int)WM.MBUTTONUP:
                    text = "WM_MBUTTONUP";
                    break;
                case (int)WM.MBUTTONDBLCLK:
                    text = "WM_MBUTTONDBLCLK";
                    break;
                case (int)WM.MOUSEWHEEL:
                    text = "WM_MOUSEWHEEL";
                    break;
                case (int)WM.PARENTNOTIFY:
                    text = "WM_PARENTNOTIFY";
                    break;
                case (int)WM.ENTERMENULOOP:
                    text = "WM_ENTERMENULOOP";
                    break;
                case (int)WM.EXITMENULOOP:
                    text = "WM_EXITMENULOOP";
                    break;
                case (int)WM.NEXTMENU:
                    text = "WM_NEXTMENU";
                    break;
                case (int)WM.SIZING:
                    text = "WM_SIZING";
                    break;
                case (int)WM.CAPTURECHANGED:
                    text = "WM_CAPTURECHANGED";
                    break;
                case (int)WM.MOVING:
                    text = "WM_MOVING";
                    break;
                case (int)WM.POWERBROADCAST:
                    text = "WM_POWERBROADCAST";
                    break;
                case (int)WM.DEVICECHANGE:
                    text = "WM_DEVICECHANGE";
                    break;
                case (int)WM.IME_SETCONTEXT:
                    text = "WM_IME_SETCONTEXT";
                    break;
                case (int)WM.IME_NOTIFY:
                    text = "WM_IME_NOTIFY";
                    break;
                case (int)WM.IME_CONTROL:
                    text = "WM_IME_CONTROL";
                    break;
                case (int)WM.IME_COMPOSITIONFULL:
                    text = "WM_IME_COMPOSITIONFULL";
                    break;
                case (int)WM.IME_SELECT:
                    text = "WM_IME_SELECT";
                    break;
                case (int)WM.IME_CHAR:
                    text = "WM_IME_CHAR";
                    break;
                case (int)WM.IME_KEYDOWN:
                    text = "WM_IME_KEYDOWN";
                    break;
                case (int)WM.IME_KEYUP:
                    text = "WM_IME_KEYUP";
                    break;
                case (int)WM.MDICREATE:
                    text = "WM_MDICREATE";
                    break;
                case (int)WM.MDIDESTROY:
                    text = "WM_MDIDESTROY";
                    break;
                case (int)WM.MDIACTIVATE:
                    text = "WM_MDIACTIVATE";
                    break;
                case (int)WM.MDIRESTORE:
                    text = "WM_MDIRESTORE";
                    break;
                case (int)WM.MDINEXT:
                    text = "WM_MDINEXT";
                    break;
                case (int)WM.MDIMAXIMIZE:
                    text = "WM_MDIMAXIMIZE";
                    break;
                case (int)WM.MDITILE:
                    text = "WM_MDITILE";
                    break;
                case (int)WM.MDICASCADE:
                    text = "WM_MDICASCADE";
                    break;
                case (int)WM.MDIICONARRANGE:
                    text = "WM_MDIICONARRANGE";
                    break;
                case (int)WM.MDIGETACTIVE:
                    text = "WM_MDIGETACTIVE";
                    break;
                case (int)WM.MDISETMENU:
                    text = "WM_MDISETMENU";
                    break;
                case (int)WM.ENTERSIZEMOVE:
                    text = "WM_ENTERSIZEMOVE";
                    break;
                case (int)WM.EXITSIZEMOVE:
                    text = "WM_EXITSIZEMOVE";
                    break;
                case (int)WM.DROPFILES:
                    text = "WM_DROPFILES";
                    break;
                case (int)WM.MDIREFRESHMENU:
                    text = "WM_MDIREFRESHMENU";
                    break;
                case (int)WM.MOUSEHOVER:
                    text = "WM_MOUSEHOVER";
                    break;
                case (int)WM.MOUSELEAVE:
                    text = "WM_MOUSELEAVE";
                    break;
                case (int)WM.CUT:
                    text = "WM_CUT";
                    break;
                case (int)WM.COPY:
                    text = "WM_COPY";
                    break;
                case (int)WM.PASTE:
                    text = "WM_PASTE";
                    break;
                case (int)WM.CLEAR:
                    text = "WM_CLEAR";
                    break;
                case (int)WM.UNDO:
                    text = "WM_UNDO";
                    break;
                case (int)WM.RENDERFORMAT:
                    text = "WM_RENDERFORMAT";
                    break;
                case (int)WM.RENDERALLFORMATS:
                    text = "WM_RENDERALLFORMATS";
                    break;
                case (int)WM.DESTROYCLIPBOARD:
                    text = "WM_DESTROYCLIPBOARD";
                    break;
                case (int)WM.DRAWCLIPBOARD:
                    text = "WM_DRAWCLIPBOARD";
                    break;
                case (int)WM.PAINTCLIPBOARD:
                    text = "WM_PAINTCLIPBOARD";
                    break;
                case (int)WM.VSCROLLCLIPBOARD:
                    text = "WM_VSCROLLCLIPBOARD";
                    break;
                case (int)WM.SIZECLIPBOARD:
                    text = "WM_SIZECLIPBOARD";
                    break;
                case (int)WM.ASKCBFORMATNAME:
                    text = "WM_ASKCBFORMATNAME";
                    break;
                case (int)WM.CHANGECBCHAIN:
                    text = "WM_CHANGECBCHAIN";
                    break;
                case (int)WM.HSCROLLCLIPBOARD:
                    text = "WM_HSCROLLCLIPBOARD";
                    break;
                case (int)WM.QUERYNEWPALETTE:
                    text = "WM_QUERYNEWPALETTE";
                    break;
                case (int)WM.PALETTEISCHANGING:
                    text = "WM_PALETTEISCHANGING";
                    break;
                case (int)WM.PALETTECHANGED:
                    text = "WM_PALETTECHANGED";
                    break;
                case (int)WM.HOTKEY:
                    text = "WM_HOTKEY";
                    break;
                case (int)WM.PRINT:
                    text = "WM_PRINT";
                    break;
                case (int)WM.PRINTCLIENT:
                    text = "WM_PRINTCLIENT";
                    break;
                case (int)WM.HANDHELDFIRST:
                    text = "WM_HANDHELDFIRST";
                    break;
                case (int)WM.HANDHELDLAST:
                    text = "WM_HANDHELDLAST";
                    break;
                case (int)WM.AFXFIRST:
                    text = "WM_AFXFIRST";
                    break;
                case (int)WM.AFXLAST:
                    text = "WM_AFXLAST";
                    break;
                case (int)WM.PENWINFIRST:
                    text = "WM_PENWINFIRST";
                    break;
                case (int)WM.PENWINLAST:
                    text = "WM_PENWINLAST";
                    break;
                case (int)WM.APP:
                    text = "WM_APP";
                    break;
                case (int)WM.USER:
                    text = "WM_USER";
                    break;
                case (int)WM.CTLCOLOR:
                    text = "WM_CTLCOLOR";
                    break;

                // RichEdit messages
                case (int)User32.EM.GETLIMITTEXT:
                    text = "EM_GETLIMITTEXT";
                    break;
                case (int)User32.EM.POSFROMCHAR:
                    text = "EM_POSFROMCHAR";
                    break;
                case (int)User32.EM.CHARFROMPOS:
                    text = "EM_CHARFROMPOS";
                    break;
                case (int)User32.EM.SCROLLCARET:
                    text = "EM_SCROLLCARET";
                    break;
                case (int)Richedit.EM.CANPASTE:
                    text = "EM_CANPASTE";
                    break;
                case (int)Richedit.EM.DISPLAYBAND:
                    text = "EM_DISPLAYBAND";
                    break;
                case (int)Richedit.EM.EXGETSEL:
                    text = "EM_EXGETSEL";
                    break;
                case (int)Richedit.EM.EXLIMITTEXT:
                    text = "EM_EXLIMITTEXT";
                    break;
                case (int)Richedit.EM.EXLINEFROMCHAR:
                    text = "EM_EXLINEFROMCHAR";
                    break;
                case (int)Richedit.EM.EXSETSEL:
                    text = "EM_EXSETSEL";
                    break;
                case (int)Richedit.EM.FINDTEXT:
                    text = "EM_FINDTEXT";
                    break;
                case (int)Richedit.EM.FORMATRANGE:
                    text = "EM_FORMATRANGE";
                    break;
                case (int)Richedit.EM.GETCHARFORMAT:
                    text = "EM_GETCHARFORMAT";
                    break;
                case (int)Richedit.EM.GETEVENTMASK:
                    text = "EM_GETEVENTMASK";
                    break;
                case (int)Richedit.EM.GETOLEINTERFACE:
                    text = "EM_GETOLEINTERFACE";
                    break;
                case (int)Richedit.EM.GETPARAFORMAT:
                    text = "EM_GETPARAFORMAT";
                    break;
                case (int)Richedit.EM.GETSELTEXT:
                    text = "EM_GETSELTEXT";
                    break;
                case (int)Richedit.EM.HIDESELECTION:
                    text = "EM_HIDESELECTION";
                    break;
                case (int)Richedit.EM.PASTESPECIAL:
                    text = "EM_PASTESPECIAL";
                    break;
                case (int)Richedit.EM.REQUESTRESIZE:
                    text = "EM_REQUESTRESIZE";
                    break;
                case (int)Richedit.EM.SELECTIONTYPE:
                    text = "EM_SELECTIONTYPE";
                    break;
                case (int)Richedit.EM.SETBKGNDCOLOR:
                    text = "EM_SETBKGNDCOLOR";
                    break;
                case (int)Richedit.EM.SETCHARFORMAT:
                    text = "EM_SETCHARFORMAT";
                    break;
                case (int)Richedit.EM.SETEVENTMASK:
                    text = "EM_SETEVENTMASK";
                    break;
                case (int)Richedit.EM.SETOLECALLBACK:
                    text = "EM_SETOLECALLBACK";
                    break;
                case (int)Richedit.EM.SETPARAFORMAT:
                    text = "EM_SETPARAFORMAT";
                    break;
                case (int)Richedit.EM.SETTARGETDEVICE:
                    text = "EM_SETTARGETDEVICE";
                    break;
                case (int)Richedit.EM.STREAMIN:
                    text = "EM_STREAMIN";
                    break;
                case (int)Richedit.EM.STREAMOUT:
                    text = "EM_STREAMOUT";
                    break;
                case (int)Richedit.EM.GETTEXTRANGE:
                    text = "EM_GETTEXTRANGE";
                    break;
                case (int)Richedit.EM.FINDWORDBREAK:
                    text = "EM_FINDWORDBREAK";
                    break;
                case (int)Richedit.EM.SETOPTIONS:
                    text = "EM_SETOPTIONS";
                    break;
                case (int)Richedit.EM.GETOPTIONS:
                    text = "EM_GETOPTIONS";
                    break;
                case (int)Richedit.EM.FINDTEXTEX:
                    text = "EM_FINDTEXTEX";
                    break;
                case (int)Richedit.EM.GETWORDBREAKPROCEX:
                    text = "EM_GETWORDBREAKPROCEX";
                    break;
                case (int)Richedit.EM.SETWORDBREAKPROCEX:
                    text = "EM_SETWORDBREAKPROCEX";
                    break;

                // Richedit v2.0 messages
                case (int)Richedit.EM.SETUNDOLIMIT:
                    text = "EM_SETUNDOLIMIT";
                    break;
                case (int)Richedit.EM.REDO:
                    text = "EM_REDO";
                    break;
                case (int)Richedit.EM.CANREDO:
                    text = "EM_CANREDO";
                    break;
                case (int)Richedit.EM.GETUNDONAME:
                    text = "EM_GETUNDONAME";
                    break;
                case (int)Richedit.EM.GETREDONAME:
                    text = "EM_GETREDONAME";
                    break;
                case (int)Richedit.EM.STOPGROUPTYPING:
                    text = "EM_STOPGROUPTYPING";
                    break;
                case (int)Richedit.EM.SETTEXTMODE:
                    text = "EM_SETTEXTMODE";
                    break;
                case (int)Richedit.EM.GETTEXTMODE:
                    text = "EM_GETTEXTMODE";
                    break;
                case (int)Richedit.EM.AUTOURLDETECT:
                    text = "EM_AUTOURLDETECT";
                    break;
                case (int)Richedit.EM.GETAUTOURLDETECT:
                    text = "EM_GETAUTOURLDETECT";
                    break;
                case (int)Richedit.EM.SETPALETTE:
                    text = "EM_SETPALETTE";
                    break;
                case (int)Richedit.EM.GETTEXTEX:
                    text = "EM_GETTEXTEX";
                    break;
                case (int)Richedit.EM.GETTEXTLENGTHEX:
                    text = "EM_GETTEXTLENGTHEX";
                    break;

                // Asia specific messages
                case (int)Richedit.EM.SETPUNCTUATION:
                    text = "EM_SETPUNCTUATION";
                    break;
                case (int)Richedit.EM.GETPUNCTUATION:
                    text = "EM_GETPUNCTUATION";
                    break;
                case (int)Richedit.EM.SETWORDWRAPMODE:
                    text = "EM_SETWORDWRAPMODE";
                    break;
                case (int)Richedit.EM.GETWORDWRAPMODE:
                    text = "EM_GETWORDWRAPMODE";
                    break;
                case (int)Richedit.EM.SETIMECOLOR:
                    text = "EM_SETIMECOLOR";
                    break;
                case (int)Richedit.EM.GETIMECOLOR:
                    text = "EM_GETIMECOLOR";
                    break;
                case (int)Richedit.EM.SETIMEOPTIONS:
                    text = "EM_SETIMEOPTIONS";
                    break;
                case (int)Richedit.EM.GETIMEOPTIONS:
                    text = "EM_GETIMEOPTIONS";
                    break;
                case (int)Richedit.EM.CONVPOSITION:
                    text = "EM_CONVPOSITION";
                    break;
                case (int)Richedit.EM.SETLANGOPTIONS:
                    text = "EM_SETLANGOPTIONS";
                    break;
                case (int)Richedit.EM.GETLANGOPTIONS:
                    text = "EM_GETLANGOPTIONS";
                    break;
                case (int)Richedit.EM.GETIMECOMPMODE:
                    text = "EM_GETIMECOMPMODE";
                    break;
                case (int)Richedit.EM.FINDTEXTW:
                    text = "EM_FINDTEXTW";
                    break;
                case (int)Richedit.EM.FINDTEXTEXW:
                    text = "EM_FINDTEXTEXW";
                    break;

                // Rich Edit 3.0 Asia msgs
                case (int)Richedit.EM.RECONVERSION:
                    text = "EM_RECONVERSION";
                    break;
                case (int)Richedit.EM.SETIMEMODEBIAS:
                    text = "EM_SETIMEMODEBIAS";
                    break;
                case (int)Richedit.EM.GETIMEMODEBIAS:
                    text = "EM_GETIMEMODEBIAS";
                    break;

                // BiDi Specific messages
                case (int)Richedit.EM.SETBIDIOPTIONS:
                    text = "EM_SETBIDIOPTIONS";
                    break;
                case (int)Richedit.EM.GETBIDIOPTIONS:
                    text = "EM_GETBIDIOPTIONS";
                    break;
                case (int)Richedit.EM.SETTYPOGRAPHYOPTIONS:
                    text = "EM_SETTYPOGRAPHYOPTIONS";
                    break;
                case (int)Richedit.EM.GETTYPOGRAPHYOPTIONS:
                    text = "EM_GETTYPOGRAPHYOPTIONS";
                    break;

                // Extended Edit style specific messages
                case (int)Richedit.EM.SETEDITSTYLE:
                    text = "EM_SETEDITSTYLE";
                    break;
                case (int)Richedit.EM.GETEDITSTYLE:
                    text = "EM_GETEDITSTYLE";
                    break;

                default:
                    text = null;
                    break;
            }

            if (text is null && ((msg & (int)WM.REFLECT) == (int)WM.REFLECT))
            {
                string subtext = MsgToString(msg - (int)WM.REFLECT) ?? "???";

                text = "WM_REFLECT + " + subtext;
            }

            return text;
        }

        private static string Parenthesize(string? input)
        {
            if (input is null)
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
            if (msg == (int)WM.PARENTNOTIFY)
            {
                lDescription = Parenthesize(MsgToString(PARAM.LOWORD(wparam)));
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

