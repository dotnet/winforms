// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class MessageTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntPtrTheoryData))]
        public void Message_HWnd_Set_GetReturnsExpected(IntPtr value)
        {
            var message = new Message
            {
                HWnd = value
            };
            Assert.Equal(value, message.HWnd);

            // Set same.
            message.HWnd = value;
            Assert.Equal(value, message.HWnd);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void Message_Msg_Set_GetReturnsExpected(int value)
        {
            var message = new Message
            {
                Msg = value
            };
            Assert.Equal(value, message.Msg);

            // Set same.
            message.Msg = value;
            Assert.Equal(value, message.Msg);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntPtrTheoryData))]
        public void Message_WParam_Set_GetReturnsExpected(IntPtr value)
        {
            var message = new Message
            {
                WParam = value
            };
            Assert.Equal(value, message.WParam);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntPtrTheoryData))]
        public void Message_LParam_Set_GetReturnsExpected(IntPtr value)
        {
            var message = new Message
            {
                LParam = value
            };
            Assert.Equal(value, message.LParam);

            // Set same.
            message.LParam = value;
            Assert.Equal(value, message.LParam);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntPtrTheoryData))]
        public void Message_Result_Set_GetReturnsExpected(IntPtr value)
        {
            var message = new Message
            {
                Result = value
            };
            Assert.Equal(value, message.Result);

            // Set same.
            message.Result = value;
            Assert.Equal(value, message.Result);
        }

        public static IEnumerable<object[]> Create_TestData()
        {
            yield return new object[] { (IntPtr)(-1), -1, (IntPtr)(-1), (IntPtr)(-1) };
            yield return new object[] { IntPtr.Zero, 1, IntPtr.Zero, IntPtr.Zero };
            yield return new object[] { (IntPtr)1, 1, (IntPtr)1, (IntPtr)1 };
        }

        [Theory]
        [MemberData(nameof(Create_TestData))]
        public void Message_Create_Invoke_ReturnsExpected(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            Message message = Message.Create(hWnd, msg, wparam, lparam);
            Assert.Equal(hWnd, message.HWnd);
            Assert.Equal(msg, message.Msg);
            Assert.Equal(wparam, message.WParam);
            Assert.Equal(lparam, message.LParam);
            Assert.Equal(IntPtr.Zero, message.Result);
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam, IntPtr result)
            {
                Message message = Message.Create(hWnd, msg, wparam, lparam);
                message.Result = result;
                return message;
            }

            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                true
            };
            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                Create((IntPtr)2, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                false
            };
            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                Create((IntPtr)1, 3, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                false
            };
            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                Create((IntPtr)1, 2, (IntPtr)4, (IntPtr)4, (IntPtr)5),
                false
            };
            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)5, (IntPtr)5),
                false
            };
            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)6),
                false
            };

            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                new object(),
                false
            };
            yield return new object[]
            {
                Create((IntPtr)1, 2, (IntPtr)3, (IntPtr)4, (IntPtr)5),
                null,
                false
            };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void Message_Equals_Invoke_ReturnsExpected(Message message, object other, bool expected)
        {
            if (other is Message otherMessage)
            {
                Assert.Equal(expected, message == otherMessage);
                Assert.Equal(!expected, message != otherMessage);
            }

            Assert.Equal(expected, message.Equals(other));
        }

        [Fact]
        public void Message_GetHashCode_Invoke_ReturnsExpected()
        {
            Message message = Message.Create((IntPtr)1, 1, (IntPtr)1, (IntPtr)1);
            Assert.NotEqual(0, message.GetHashCode());
            Assert.Equal(message.GetHashCode(), message.GetHashCode());
        }

        [Fact]
        public void Message_GetLParam_Invoke_ReturnsExpected()
        {
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf<TestStruct>());
            try
            {
                var original = new TestStruct
                {
                    _field1 = 1,
                    _field2 = 2
                };
                Marshal.StructureToPtr(original, ptr, fDeleteOld: false);

                var message = new Message
                {
                    LParam = ptr
                };
                var lparam = Assert.IsType<TestStruct>(message.GetLParam(typeof(TestStruct)));
                Assert.Equal(1, lparam._field1);
                Assert.Equal(2, lparam._field2);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TestStruct
        {
            public int _field1;
            public int _field2;
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { 0x1FFF, null };
            yield return new object[] { 0x2000, " (WM_REFLECT + WM_NULL)" };
            yield return new object[] { -1 | 0x2000, " (WM_REFLECT + ???)" };

            yield return new object[] { WM_NULL, " (WM_NULL)" };
            yield return new object[] { WM_CREATE, " (WM_CREATE)" };
            yield return new object[] { WM_DESTROY, " (WM_DESTROY)" };
            yield return new object[] { WM_MOVE, " (WM_MOVE)" };
            yield return new object[] { WM_SIZE, " (WM_SIZE)" };
            yield return new object[] { WM_ACTIVATE, " (WM_ACTIVATE)" };
            yield return new object[] { WM_SETFOCUS, " (WM_SETFOCUS)" };
            yield return new object[] { WM_KILLFOCUS, " (WM_KILLFOCUS)" };
            yield return new object[] { WM_ENABLE, " (WM_ENABLE)" };
            yield return new object[] { WM_SETREDRAW, " (WM_SETREDRAW)" };
            yield return new object[] { WM_SETTEXT, " (WM_SETTEXT)" };
            yield return new object[] { WM_GETTEXT, " (WM_GETTEXT)" };
            yield return new object[] { WM_GETTEXTLENGTH, " (WM_GETTEXTLENGTH)" };
            yield return new object[] { WM_PAINT, " (WM_PAINT)" };
            yield return new object[] { WM_CLOSE, " (WM_CLOSE)" };
            yield return new object[] { WM_QUERYENDSESSION, " (WM_QUERYENDSESSION)" };
            yield return new object[] { WM_QUIT, " (WM_QUIT)" };
            yield return new object[] { WM_QUERYOPEN, " (WM_QUERYOPEN)" };
            yield return new object[] { WM_ERASEBKGND, " (WM_ERASEBKGND)" };
            yield return new object[] { WM_SYSCOLORCHANGE, " (WM_SYSCOLORCHANGE)" };
            yield return new object[] { WM_ENDSESSION, " (WM_ENDSESSION)" };
            yield return new object[] { WM_SHOWWINDOW, " (WM_SHOWWINDOW)" };
            yield return new object[] { WM_WININICHANGE, " (WM_WININICHANGE)" };
            yield return new object[] { WM_DEVMODECHANGE, " (WM_DEVMODECHANGE)" };
            yield return new object[] { WM_ACTIVATEAPP, " (WM_ACTIVATEAPP)" };
            yield return new object[] { WM_FONTCHANGE, " (WM_FONTCHANGE)" };
            yield return new object[] { WM_TIMECHANGE, " (WM_TIMECHANGE)" };
            yield return new object[] { WM_CANCELMODE, " (WM_CANCELMODE)" };
            yield return new object[] { WM_SETCURSOR, " (WM_SETCURSOR)" };
            yield return new object[] { WM_MOUSEACTIVATE, " (WM_MOUSEACTIVATE)" };
            yield return new object[] { WM_CHILDACTIVATE, " (WM_CHILDACTIVATE)" };
            yield return new object[] { WM_QUEUESYNC, " (WM_QUEUESYNC)" };
            yield return new object[] { WM_GETMINMAXINFO, " (WM_GETMINMAXINFO)" };
            yield return new object[] { WM_PAINTICON, " (WM_PAINTICON)" };
            yield return new object[] { WM_ICONERASEBKGND, " (WM_ICONERASEBKGND)" };
            yield return new object[] { WM_NEXTDLGCTL, " (WM_NEXTDLGCTL)" };
            yield return new object[] { WM_SPOOLERSTATUS, " (WM_SPOOLERSTATUS)" };
            yield return new object[] { WM_DRAWITEM, " (WM_DRAWITEM)" };
            yield return new object[] { WM_MEASUREITEM, " (WM_MEASUREITEM)" };
            yield return new object[] { WM_DELETEITEM, " (WM_DELETEITEM)" };
            yield return new object[] { WM_VKEYTOITEM, " (WM_VKEYTOITEM)" };
            yield return new object[] { WM_CHARTOITEM, " (WM_CHARTOITEM)" };
            yield return new object[] { WM_SETFONT, " (WM_SETFONT)" };
            yield return new object[] { WM_GETFONT, " (WM_GETFONT)" };
            yield return new object[] { WM_SETHOTKEY, " (WM_SETHOTKEY)" };
            yield return new object[] { WM_GETHOTKEY, " (WM_GETHOTKEY)" };
            yield return new object[] { WM_QUERYDRAGICON, " (WM_QUERYDRAGICON)" };
            yield return new object[] { WM_COMPAREITEM, " (WM_COMPAREITEM)" };
            yield return new object[] { WM_GETOBJECT, " (WM_GETOBJECT)" };
            yield return new object[] { WM_COMPACTING, " (WM_COMPACTING)" };
            yield return new object[] { WM_COMMNOTIFY, " (WM_COMMNOTIFY)" };
            yield return new object[] { WM_WINDOWPOSCHANGING, " (WM_WINDOWPOSCHANGING)" };
            yield return new object[] { WM_WINDOWPOSCHANGED, " (WM_WINDOWPOSCHANGED)" };
            yield return new object[] { WM_POWER, " (WM_POWER)" };
            yield return new object[] { WM_COPYDATA, " (WM_COPYDATA)" };
            yield return new object[] { WM_CANCELJOURNAL, " (WM_CANCELJOURNAL)" };
            yield return new object[] { WM_NOTIFY, " (WM_NOTIFY)" };
            yield return new object[] { WM_INPUTLANGCHANGEREQUEST, " (WM_INPUTLANGCHANGEREQUEST)" };
            yield return new object[] { WM_INPUTLANGCHANGE, " (WM_INPUTLANGCHANGE)" };
            yield return new object[] { WM_TCARD, " (WM_TCARD)" };
            yield return new object[] { WM_HELP, " (WM_HELP)" };
            yield return new object[] { WM_USERCHANGED, " (WM_USERCHANGED)" };
            yield return new object[] { WM_NOTIFYFORMAT, " (WM_NOTIFYFORMAT)" };
            yield return new object[] { WM_CONTEXTMENU, " (WM_CONTEXTMENU)" };
            yield return new object[] { WM_STYLECHANGING, " (WM_STYLECHANGING)" };
            yield return new object[] { WM_STYLECHANGED, " (WM_STYLECHANGED)" };
            yield return new object[] { WM_DISPLAYCHANGE, " (WM_DISPLAYCHANGE)" };
            yield return new object[] { WM_GETICON, " (WM_GETICON)" };
            yield return new object[] { WM_SETICON, " (WM_SETICON)" };
            yield return new object[] { WM_NCCREATE, " (WM_NCCREATE)" };
            yield return new object[] { WM_NCDESTROY, " (WM_NCDESTROY)" };
            yield return new object[] { WM_NCCALCSIZE, " (WM_NCCALCSIZE)" };
            yield return new object[] { WM_NCHITTEST, " (WM_NCHITTEST)" };
            yield return new object[] { WM_NCPAINT, " (WM_NCPAINT)" };
            yield return new object[] { WM_NCACTIVATE, " (WM_NCACTIVATE)" };
            yield return new object[] { WM_GETDLGCODE, " (WM_GETDLGCODE)" };
            yield return new object[] { WM_NCMOUSEMOVE, " (WM_NCMOUSEMOVE)" };
            yield return new object[] { WM_NCLBUTTONDOWN, " (WM_NCLBUTTONDOWN)" };
            yield return new object[] { WM_NCLBUTTONUP, " (WM_NCLBUTTONUP)" };
            yield return new object[] { WM_NCLBUTTONDBLCLK, " (WM_NCLBUTTONDBLCLK)" };
            yield return new object[] { WM_NCRBUTTONDOWN, " (WM_NCRBUTTONDOWN)" };
            yield return new object[] { WM_NCRBUTTONUP, " (WM_NCRBUTTONUP)" };
            yield return new object[] { WM_NCRBUTTONDBLCLK, " (WM_NCRBUTTONDBLCLK)" };
            yield return new object[] { WM_NCMBUTTONDOWN, " (WM_NCMBUTTONDOWN)" };
            yield return new object[] { WM_NCMBUTTONUP, " (WM_NCMBUTTONUP)" };
            yield return new object[] { WM_NCMBUTTONDBLCLK, " (WM_NCMBUTTONDBLCLK)" };
            yield return new object[] { WM_KEYDOWN, " (WM_KEYDOWN)" };
            yield return new object[] { WM_KEYUP, " (WM_KEYUP)" };
            yield return new object[] { WM_CHAR, " (WM_CHAR)" };
            yield return new object[] { WM_DEADCHAR, " (WM_DEADCHAR)" };
            yield return new object[] { WM_SYSKEYDOWN, " (WM_SYSKEYDOWN)" };
            yield return new object[] { WM_SYSKEYUP, " (WM_SYSKEYUP)" };
            yield return new object[] { WM_SYSCHAR, " (WM_SYSCHAR)" };
            yield return new object[] { WM_SYSDEADCHAR, " (WM_SYSDEADCHAR)" };
            yield return new object[] { WM_KEYLAST, " (WM_KEYLAST)" };
            yield return new object[] { WM_IME_STARTCOMPOSITION, " (WM_IME_STARTCOMPOSITION)" };
            yield return new object[] { WM_IME_ENDCOMPOSITION, " (WM_IME_ENDCOMPOSITION)" };
            yield return new object[] { WM_IME_COMPOSITION, " (WM_IME_COMPOSITION)" };
            yield return new object[] { WM_INITDIALOG, " (WM_INITDIALOG)" };
            yield return new object[] { WM_COMMAND, " (WM_COMMAND)" };
            yield return new object[] { WM_SYSCOMMAND, " (WM_SYSCOMMAND)" };
            yield return new object[] { WM_TIMER, " (WM_TIMER)" };
            yield return new object[] { WM_HSCROLL, " (WM_HSCROLL)" };
            yield return new object[] { WM_VSCROLL, " (WM_VSCROLL)" };
            yield return new object[] { WM_INITMENU, " (WM_INITMENU)" };
            yield return new object[] { WM_INITMENUPOPUP, " (WM_INITMENUPOPUP)" };
            yield return new object[] { WM_MENUSELECT, " (WM_MENUSELECT)" };
            yield return new object[] { WM_MENUCHAR, " (WM_MENUCHAR)" };
            yield return new object[] { WM_ENTERIDLE, " (WM_ENTERIDLE)" };
            yield return new object[] { WM_CTLCOLORMSGBOX, " (WM_CTLCOLORMSGBOX)" };
            yield return new object[] { WM_CTLCOLOREDIT, " (WM_CTLCOLOREDIT)" };
            yield return new object[] { WM_CTLCOLORLISTBOX, " (WM_CTLCOLORLISTBOX)" };
            yield return new object[] { WM_CTLCOLORBTN, " (WM_CTLCOLORBTN)" };
            yield return new object[] { WM_CTLCOLORDLG, " (WM_CTLCOLORDLG)" };
            yield return new object[] { WM_CTLCOLORSCROLLBAR, " (WM_CTLCOLORSCROLLBAR)" };
            yield return new object[] { WM_CTLCOLORSTATIC, " (WM_CTLCOLORSTATIC)" };
            yield return new object[] { WM_MOUSEMOVE, " (WM_MOUSEMOVE)" };
            yield return new object[] { WM_LBUTTONDOWN, " (WM_LBUTTONDOWN)" };
            yield return new object[] { WM_LBUTTONUP, " (WM_LBUTTONUP)" };
            yield return new object[] { WM_LBUTTONDBLCLK, " (WM_LBUTTONDBLCLK)" };
            yield return new object[] { WM_RBUTTONDOWN, " (WM_RBUTTONDOWN)" };
            yield return new object[] { WM_RBUTTONUP, " (WM_RBUTTONUP)" };
            yield return new object[] { WM_RBUTTONDBLCLK, " (WM_RBUTTONDBLCLK)" };
            yield return new object[] { WM_MBUTTONDOWN, " (WM_MBUTTONDOWN)" };
            yield return new object[] { WM_MBUTTONUP, " (WM_MBUTTONUP)" };
            yield return new object[] { WM_MBUTTONDBLCLK, " (WM_MBUTTONDBLCLK)" };
            yield return new object[] { WM_MOUSEWHEEL, " (WM_MOUSEWHEEL)" };
            yield return new object[] { WM_PARENTNOTIFY, " (WM_PARENTNOTIFY)", " (WM_DESTROY)" };
            yield return new object[] { WM_ENTERMENULOOP, " (WM_ENTERMENULOOP)" };
            yield return new object[] { WM_EXITMENULOOP, " (WM_EXITMENULOOP)" };
            yield return new object[] { WM_NEXTMENU, " (WM_NEXTMENU)" };
            yield return new object[] { WM_SIZING, " (WM_SIZING)" };
            yield return new object[] { WM_CAPTURECHANGED, " (WM_CAPTURECHANGED)" };
            yield return new object[] { WM_MOVING, " (WM_MOVING)" };
            yield return new object[] { WM_POWERBROADCAST, " (WM_POWERBROADCAST)" };
            yield return new object[] { WM_DEVICECHANGE, " (WM_DEVICECHANGE)" };
            yield return new object[] { WM_IME_SETCONTEXT, " (WM_IME_SETCONTEXT)" };
            yield return new object[] { WM_IME_NOTIFY, " (WM_IME_NOTIFY)" };
            yield return new object[] { WM_IME_CONTROL, " (WM_IME_CONTROL)" };
            yield return new object[] { WM_IME_COMPOSITIONFULL, " (WM_IME_COMPOSITIONFULL)" };
            yield return new object[] { WM_IME_SELECT, " (WM_IME_SELECT)" };
            yield return new object[] { WM_IME_CHAR, " (WM_IME_CHAR)" };
            yield return new object[] { WM_IME_KEYDOWN, " (WM_IME_KEYDOWN)" };
            yield return new object[] { WM_IME_KEYUP, " (WM_IME_KEYUP)" };
            yield return new object[] { WM_MDICREATE, " (WM_MDICREATE)" };
            yield return new object[] { WM_MDIDESTROY, " (WM_MDIDESTROY)" };
            yield return new object[] { WM_MDIACTIVATE, " (WM_MDIACTIVATE)" };
            yield return new object[] { WM_MDIRESTORE, " (WM_MDIRESTORE)" };
            yield return new object[] { WM_MDINEXT, " (WM_MDINEXT)" };
            yield return new object[] { WM_MDIMAXIMIZE, " (WM_MDIMAXIMIZE)" };
            yield return new object[] { WM_MDITILE, " (WM_MDITILE)" };
            yield return new object[] { WM_MDICASCADE, " (WM_MDICASCADE)" };
            yield return new object[] { WM_MDIICONARRANGE, " (WM_MDIICONARRANGE)" };
            yield return new object[] { WM_MDIGETACTIVE, " (WM_MDIGETACTIVE)" };
            yield return new object[] { WM_MDISETMENU, " (WM_MDISETMENU)" };
            yield return new object[] { WM_ENTERSIZEMOVE, " (WM_ENTERSIZEMOVE)" };
            yield return new object[] { WM_EXITSIZEMOVE, " (WM_EXITSIZEMOVE)" };
            yield return new object[] { WM_DROPFILES, " (WM_DROPFILES)" };
            yield return new object[] { WM_MDIREFRESHMENU, " (WM_MDIREFRESHMENU)" };
            yield return new object[] { WM_MOUSEHOVER, " (WM_MOUSEHOVER)" };
            yield return new object[] { WM_MOUSELEAVE, " (WM_MOUSELEAVE)" };
            yield return new object[] { WM_CUT, " (WM_CUT)" };
            yield return new object[] { WM_COPY, " (WM_COPY)" };
            yield return new object[] { WM_PASTE, " (WM_PASTE)" };
            yield return new object[] { WM_CLEAR, " (WM_CLEAR)" };
            yield return new object[] { WM_UNDO, " (WM_UNDO)" };
            yield return new object[] { WM_RENDERFORMAT, " (WM_RENDERFORMAT)" };
            yield return new object[] { WM_RENDERALLFORMATS, " (WM_RENDERALLFORMATS)" };
            yield return new object[] { WM_DESTROYCLIPBOARD, " (WM_DESTROYCLIPBOARD)" };
            yield return new object[] { WM_DRAWCLIPBOARD, " (WM_DRAWCLIPBOARD)" };
            yield return new object[] { WM_PAINTCLIPBOARD, " (WM_PAINTCLIPBOARD)" };
            yield return new object[] { WM_VSCROLLCLIPBOARD, " (WM_VSCROLLCLIPBOARD)" };
            yield return new object[] { WM_SIZECLIPBOARD, " (WM_SIZECLIPBOARD)" };
            yield return new object[] { WM_ASKCBFORMATNAME, " (WM_ASKCBFORMATNAME)" };
            yield return new object[] { WM_CHANGECBCHAIN, " (WM_CHANGECBCHAIN)" };
            yield return new object[] { WM_HSCROLLCLIPBOARD, " (WM_HSCROLLCLIPBOARD)" };
            yield return new object[] { WM_QUERYNEWPALETTE, " (WM_QUERYNEWPALETTE)" };
            yield return new object[] { WM_PALETTEISCHANGING, " (WM_PALETTEISCHANGING)" };
            yield return new object[] { WM_PALETTECHANGED, " (WM_PALETTECHANGED)" };
            yield return new object[] { WM_HOTKEY, " (WM_HOTKEY)" };
            yield return new object[] { WM_PRINT, " (WM_PRINT)" };
            yield return new object[] { WM_PRINTCLIENT, " (WM_PRINTCLIENT)" };
            yield return new object[] { WM_HANDHELDFIRST, " (WM_HANDHELDFIRST)" };
            yield return new object[] { WM_HANDHELDLAST, " (WM_HANDHELDLAST)" };
            yield return new object[] { WM_AFXFIRST, " (WM_AFXFIRST)" };
            yield return new object[] { WM_AFXLAST, " (WM_AFXLAST)" };
            yield return new object[] { WM_PENWINFIRST, " (WM_PENWINFIRST)" };
            yield return new object[] { WM_PENWINLAST, " (WM_PENWINLAST)" };
            yield return new object[] { WM_APP, " (WM_APP)" };
            yield return new object[] { WM_USER, " (WM_USER)" };
            yield return new object[] { WM_CTLCOLOR, " (WM_CTLCOLOR)" };

            // RichEdit messages
            yield return new object[] { EM_GETLIMITTEXT, " (EM_GETLIMITTEXT)" };
            yield return new object[] { EM_POSFROMCHAR, " (EM_POSFROMCHAR)" };
            yield return new object[] { EM_CHARFROMPOS, " (EM_CHARFROMPOS)" };
            yield return new object[] { EM_SCROLLCARET, " (EM_SCROLLCARET)" };
            yield return new object[] { EM_CANPASTE, " (EM_CANPASTE)" };
            yield return new object[] { EM_DISPLAYBAND, " (EM_DISPLAYBAND)" };
            yield return new object[] { EM_EXGETSEL, " (EM_EXGETSEL)" };
            yield return new object[] { EM_EXLIMITTEXT, " (EM_EXLIMITTEXT)" };
            yield return new object[] { EM_EXLINEFROMCHAR, " (EM_EXLINEFROMCHAR)" };
            yield return new object[] { EM_EXSETSEL, " (EM_EXSETSEL)" };
            yield return new object[] { EM_FINDTEXT, " (EM_FINDTEXT)" };
            yield return new object[] { EM_FORMATRANGE, " (EM_FORMATRANGE)" };
            yield return new object[] { EM_GETCHARFORMAT, " (EM_GETCHARFORMAT)" };
            yield return new object[] { EM_GETEVENTMASK, " (EM_GETEVENTMASK)" };
            yield return new object[] { EM_GETOLEINTERFACE, " (EM_GETOLEINTERFACE)" };
            yield return new object[] { EM_GETPARAFORMAT, " (EM_GETPARAFORMAT)" };
            yield return new object[] { EM_GETSELTEXT, " (EM_GETSELTEXT)" };
            yield return new object[] { EM_HIDESELECTION, " (EM_HIDESELECTION)" };
            yield return new object[] { EM_PASTESPECIAL, " (EM_PASTESPECIAL)" };
            yield return new object[] { EM_REQUESTRESIZE, " (EM_REQUESTRESIZE)" };
            yield return new object[] { EM_SELECTIONTYPE, " (EM_SELECTIONTYPE)" };
            yield return new object[] { EM_SETBKGNDCOLOR, " (EM_SETBKGNDCOLOR)" };
            yield return new object[] { EM_SETCHARFORMAT, " (EM_SETCHARFORMAT)" };
            yield return new object[] { EM_SETEVENTMASK, " (EM_SETEVENTMASK)" };
            yield return new object[] { EM_SETOLECALLBACK, " (EM_SETOLECALLBACK)" };
            yield return new object[] { EM_SETPARAFORMAT, " (EM_SETPARAFORMAT)" };
            yield return new object[] { EM_SETTARGETDEVICE, " (EM_SETTARGETDEVICE)" };
            yield return new object[] { EM_STREAMIN, " (EM_STREAMIN)" };
            yield return new object[] { EM_STREAMOUT, " (EM_STREAMOUT)" };
            yield return new object[] { EM_GETTEXTRANGE, " (EM_GETTEXTRANGE)" };
            yield return new object[] { EM_FINDWORDBREAK, " (EM_FINDWORDBREAK)" };
            yield return new object[] { EM_SETOPTIONS, " (EM_SETOPTIONS)" };
            yield return new object[] { EM_GETOPTIONS, " (EM_GETOPTIONS)" };
            yield return new object[] { EM_FINDTEXTEX, " (EM_FINDTEXTEX)" };
            yield return new object[] { EM_GETWORDBREAKPROCEX, " (EM_GETWORDBREAKPROCEX)" };
            yield return new object[] { EM_SETWORDBREAKPROCEX, " (EM_SETWORDBREAKPROCEX)" };

            // Richedit v2.0 messages
            yield return new object[] { EM_SETUNDOLIMIT, " (EM_SETUNDOLIMIT)" };
            yield return new object[] { EM_REDO, " (EM_REDO)" };
            yield return new object[] { EM_CANREDO, " (EM_CANREDO)" };
            yield return new object[] { EM_GETUNDONAME, " (EM_GETUNDONAME)" };
            yield return new object[] { EM_GETREDONAME, " (EM_GETREDONAME)" };
            yield return new object[] { EM_STOPGROUPTYPING, " (EM_STOPGROUPTYPING)" };
            yield return new object[] { EM_SETTEXTMODE, " (EM_SETTEXTMODE)" };
            yield return new object[] { EM_GETTEXTMODE, " (EM_GETTEXTMODE)" };
            yield return new object[] { EM_AUTOURLDETECT, " (EM_AUTOURLDETECT)" };
            yield return new object[] { EM_GETAUTOURLDETECT, " (EM_GETAUTOURLDETECT)" };
            yield return new object[] { EM_SETPALETTE, " (EM_SETPALETTE)" };
            yield return new object[] { EM_GETTEXTEX, " (EM_GETTEXTEX)" };
            yield return new object[] { EM_GETTEXTLENGTHEX, " (EM_GETTEXTLENGTHEX)" };

            // Asia specific messages
            yield return new object[] { EM_SETPUNCTUATION, " (EM_SETPUNCTUATION)" };
            yield return new object[] { EM_GETPUNCTUATION, " (EM_GETPUNCTUATION)" };
            yield return new object[] { EM_SETWORDWRAPMODE, " (EM_SETWORDWRAPMODE)" };
            yield return new object[] { EM_GETWORDWRAPMODE, " (EM_GETWORDWRAPMODE)" };
            yield return new object[] { EM_SETIMECOLOR, " (EM_SETIMECOLOR)" };
            yield return new object[] { EM_GETIMECOLOR, " (EM_GETIMECOLOR)" };
            yield return new object[] { EM_SETIMEOPTIONS, " (EM_SETIMEOPTIONS)" };
            yield return new object[] { EM_GETIMEOPTIONS, " (EM_GETIMEOPTIONS)" };
            yield return new object[] { EM_CONVPOSITION, " (EM_CONVPOSITION)" };
            yield return new object[] { EM_SETLANGOPTIONS, " (EM_SETLANGOPTIONS)" };
            yield return new object[] { EM_GETLANGOPTIONS, " (EM_GETLANGOPTIONS)" };
            yield return new object[] { EM_GETIMECOMPMODE, " (EM_GETIMECOMPMODE)" };
            yield return new object[] { EM_FINDTEXTW, " (EM_FINDTEXTW)" };
            yield return new object[] { EM_FINDTEXTEXW, " (EM_FINDTEXTEXW)" };

            // Rich Edit 3.0 Asia msgs
            yield return new object[] { EM_RECONVERSION, " (EM_RECONVERSION)" };
            yield return new object[] { EM_SETIMEMODEBIAS, " (EM_SETIMEMODEBIAS)" };
            yield return new object[] { EM_GETIMEMODEBIAS, " (EM_GETIMEMODEBIAS)" };

            // BiDi Specific messages
            yield return new object[] { EM_SETBIDIOPTIONS, " (EM_SETBIDIOPTIONS)" };
            yield return new object[] { EM_GETBIDIOPTIONS, " (EM_GETBIDIOPTIONS)" };
            yield return new object[] { EM_SETTYPOGRAPHYOPTIONS, " (EM_SETTYPOGRAPHYOPTIONS)" };
            yield return new object[] { EM_GETTYPOGRAPHYOPTIONS, " (EM_GETTYPOGRAPHYOPTIONS)" };

            // Extended Edit style specific messages
            yield return new object[] { EM_SETEDITSTYLE, " (EM_SETEDITSTYLE)" };
            yield return new object[] { EM_GETEDITSTYLE, " (EM_GETEDITSTYLE)" };
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public void Message_ToString_Invoke_ReturnsExpected(int msg, string expected, string additionalMsg = null)
        {
            Message message = Message.Create((IntPtr)1, msg, (IntPtr)2, (IntPtr)3);
            message.Result = (IntPtr)4;
            Assert.Equal("msg=0x" + Convert.ToString(msg, 16) + expected + " hwnd=0x1 wparam=0x2 lparam=0x3" + additionalMsg + " result=0x4", message.ToString());
        }

        private const int WM_NULL = 0x0000;
        private const int WM_CREATE = 0x0001;
        private const int WM_DELETEITEM = 0x002D;
        private const int WM_DESTROY = 0x0002;
        private const int WM_MOVE = 0x0003;
        private const int WM_SIZE = 0x0005;
        private const int WM_ACTIVATE = 0x0006;
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_KILLFOCUS = 0x0008;
        private const int WM_ENABLE = 0x000A;
        private const int WM_SETREDRAW = 0x000B;
        private const int WM_SETTEXT = 0x000C;
        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;
        private const int WM_PAINT = 0x000F;
        private const int WM_CLOSE = 0x0010;
        private const int WM_QUERYENDSESSION = 0x0011;
        private const int WM_QUIT = 0x0012;
        private const int WM_QUERYOPEN = 0x0013;
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_SYSCOLORCHANGE = 0x0015;
        private const int WM_ENDSESSION = 0x0016;
        private const int WM_SHOWWINDOW = 0x0018;
        private const int WM_WININICHANGE = 0x001A;
        private const int WM_SETTINGCHANGE = 0x001A;
        private const int WM_DEVMODECHANGE = 0x001B;
        private const int WM_ACTIVATEAPP = 0x001C;
        private const int WM_FONTCHANGE = 0x001D;
        private const int WM_TIMECHANGE = 0x001E;
        private const int WM_CANCELMODE = 0x001F;
        private const int WM_SETCURSOR = 0x0020;
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int WM_CHILDACTIVATE = 0x0022;
        private const int WM_QUEUESYNC = 0x0023;
        private const int WM_GETMINMAXINFO = 0x0024;
        private const int WM_PAINTICON = 0x0026;
        private const int WM_ICONERASEBKGND = 0x0027;
        private const int WM_NEXTDLGCTL = 0x0028;
        private const int WM_SPOOLERSTATUS = 0x002A;
        private const int WM_DRAWITEM = 0x002B;
        private const int WM_MEASUREITEM = 0x002C;
        private const int WM_VKEYTOITEM = 0x002E;
        private const int WM_CHARTOITEM = 0x002F;
        private const int WM_SETFONT = 0x0030;
        private const int WM_GETFONT = 0x0031;
        private const int WM_SETHOTKEY = 0x0032;
        private const int WM_GETHOTKEY = 0x0033;
        private const int WM_QUERYDRAGICON = 0x0037;
        private const int WM_COMPAREITEM = 0x0039;
        private const int WM_GETOBJECT = 0x003D;
        private const int WM_COMPACTING = 0x0041;
        private const int WM_COMMNOTIFY = 0x0044;
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private const int WM_WINDOWPOSCHANGED = 0x0047;
        private const int WM_POWER = 0x0048;
        private const int WM_COPYDATA = 0x004A;
        private const int WM_CANCELJOURNAL = 0x004B;
        private const int WM_NOTIFY = 0x004E;
        private const int WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const int WM_INPUTLANGCHANGE = 0x0051;
        private const int WM_TCARD = 0x0052;
        private const int WM_HELP = 0x0053;
        private const int WM_USERCHANGED = 0x0054;
        private const int WM_NOTIFYFORMAT = 0x0055;
        private const int WM_CONTEXTMENU = 0x007B;
        private const int WM_STYLECHANGING = 0x007C;
        private const int WM_STYLECHANGED = 0x007D;
        private const int WM_DISPLAYCHANGE = 0x007E;
        private const int WM_GETICON = 0x007F;
        private const int WM_SETICON = 0x0080;
        private const int WM_NCCREATE = 0x0081;
        private const int WM_NCDESTROY = 0x0082;
        private const int WM_NCCALCSIZE = 0x0083;
        private const int WM_NCHITTEST = 0x0084;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_NCACTIVATE = 0x0086;
        private const int WM_GETDLGCODE = 0x0087;
        private const int WM_NCMOUSEMOVE = 0x00A0;
        private const int WM_NCMOUSELEAVE = 0x02A2;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_NCLBUTTONUP = 0x00A2;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        private const int WM_NCRBUTTONDOWN = 0x00A4;
        private const int WM_NCRBUTTONUP = 0x00A5;
        private const int WM_NCRBUTTONDBLCLK = 0x00A6;
        private const int WM_NCMBUTTONDOWN = 0x00A7;
        private const int WM_NCMBUTTONUP = 0x00A8;
        private const int WM_NCMBUTTONDBLCLK = 0x00A9;
        private const int WM_NCXBUTTONDOWN               = 0x00AB;
        private const int WM_NCXBUTTONUP                 = 0x00AC;
        private const int WM_NCXBUTTONDBLCLK             = 0x00AD;
        private const int WM_KEYFIRST = 0x0100;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_CHAR = 0x0102;
        private const int WM_DEADCHAR = 0x0103;
        private const int WM_CTLCOLOR = 0x0019;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private const int WM_SYSCHAR = 0x0106;
        private const int WM_SYSDEADCHAR = 0x0107;
        private const int WM_KEYLAST = 0x0108;
        private const int WM_IME_STARTCOMPOSITION = 0x010D;
        private const int WM_IME_ENDCOMPOSITION = 0x010E;
        private const int WM_IME_COMPOSITION = 0x010F;
        private const int WM_IME_KEYLAST = 0x010F;
        private const int WM_INITDIALOG = 0x0110;
        private const int WM_COMMAND = 0x0111;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int WM_TIMER = 0x0113;
        private const int WM_HSCROLL = 0x0114;
        private const int WM_VSCROLL = 0x0115;
        private const int WM_INITMENU = 0x0116;
        private const int WM_INITMENUPOPUP = 0x0117;
        private const int WM_MENUSELECT = 0x011F;
        private const int WM_MENUCHAR = 0x0120;
        private const int WM_ENTERIDLE = 0x0121;
        private const int WM_UNINITMENUPOPUP = 0x0125;
        private const int WM_CHANGEUISTATE = 0x0127;
        private const int WM_UPDATEUISTATE = 0x0128;
        private const int WM_QUERYUISTATE = 0x0129;
        private const int WM_CTLCOLORMSGBOX = 0x0132;
        private const int WM_CTLCOLOREDIT = 0x0133;
        private const int WM_CTLCOLORLISTBOX = 0x0134;
        private const int WM_CTLCOLORBTN = 0x0135;
        private const int WM_CTLCOLORDLG = 0x0136;
        private const int WM_CTLCOLORSCROLLBAR = 0x0137;
        private const int WM_CTLCOLORSTATIC = 0x0138;
        private const int WM_MOUSEFIRST = 0x0200;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_RBUTTONDBLCLK = 0x0206;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MBUTTONUP = 0x0208;
        private const int WM_MBUTTONDBLCLK = 0x0209;
        private const int WM_XBUTTONDOWN                 = 0x020B;
        private const int WM_XBUTTONUP                   = 0x020C;
        private const int WM_XBUTTONDBLCLK               = 0x020D;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_MOUSELAST = 0x020A;

        public const int WHEEL_DELTA = 120;
        private const int WM_PARENTNOTIFY = 0x0210;
        private const int WM_ENTERMENULOOP = 0x0211;
        private const int WM_EXITMENULOOP = 0x0212;
        private const int WM_NEXTMENU = 0x0213;
        private const int WM_SIZING = 0x0214;
        private const int WM_CAPTURECHANGED = 0x0215;
        private const int WM_MOVING = 0x0216;
        private const int WM_POWERBROADCAST = 0x0218;
        private const int WM_DEVICECHANGE = 0x0219;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_IME_NOTIFY = 0x0282;
        private const int WM_IME_CONTROL = 0x0283;
        private const int WM_IME_COMPOSITIONFULL = 0x0284;
        private const int WM_IME_SELECT = 0x0285;
        private const int WM_IME_CHAR = 0x0286;
        private const int WM_IME_KEYDOWN = 0x0290;
        private const int WM_IME_KEYUP = 0x0291;
        private const int WM_MDICREATE = 0x0220;
        private const int WM_MDIDESTROY = 0x0221;
        private const int WM_MDIACTIVATE = 0x0222;
        private const int WM_MDIRESTORE = 0x0223;
        private const int WM_MDINEXT = 0x0224;
        private const int WM_MDIMAXIMIZE = 0x0225;
        private const int WM_MDITILE = 0x0226;
        private const int WM_MDICASCADE = 0x0227;
        private const int WM_MDIICONARRANGE = 0x0228;
        private const int WM_MDIGETACTIVE = 0x0229;
        private const int WM_MDISETMENU = 0x0230;
        private const int WM_ENTERSIZEMOVE = 0x0231;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int WM_DROPFILES = 0x0233;
        private const int WM_MDIREFRESHMENU = 0x0234;
        private const int WM_MOUSEHOVER = 0x02A1;
        private const int WM_MOUSELEAVE = 0x02A3;
        private const int WM_DPICHANGED = 0x02E0;
        private const int WM_GETDPISCALEDSIZE = 0x02e1;
        private const int WM_DPICHANGED_BEFOREPARENT = 0x02E2;
        private const int WM_DPICHANGED_AFTERPARENT = 0x02E3;
        private const int WM_CUT = 0x0300;
        private const int WM_COPY = 0x0301;
        private const int WM_PASTE = 0x0302;
        private const int WM_CLEAR = 0x0303;
        private const int WM_UNDO = 0x0304;
        private const int WM_RENDERFORMAT = 0x0305;
        private const int WM_RENDERALLFORMATS = 0x0306;
        private const int WM_DESTROYCLIPBOARD = 0x0307;
        private const int WM_DRAWCLIPBOARD = 0x0308;
        private const int WM_PAINTCLIPBOARD = 0x0309;
        private const int WM_VSCROLLCLIPBOARD = 0x030A;
        private const int WM_SIZECLIPBOARD = 0x030B;
        private const int WM_ASKCBFORMATNAME = 0x030C;
        private const int WM_CHANGECBCHAIN = 0x030D;
        private const int WM_HSCROLLCLIPBOARD = 0x030E;
        private const int WM_QUERYNEWPALETTE = 0x030F;
        private const int WM_PALETTEISCHANGING = 0x0310;
        private const int WM_PALETTECHANGED = 0x0311;
        private const int WM_HOTKEY = 0x0312;
        private const int WM_PRINT = 0x0317;
        private const int WM_PRINTCLIENT = 0x0318;
        private const int WM_THEMECHANGED = 0x031A;
        private const int WM_HANDHELDFIRST = 0x0358;
        private const int WM_HANDHELDLAST = 0x035F;
        private const int WM_AFXFIRST = 0x0360;
        private const int WM_AFXLAST = 0x037F;
        private const int WM_PENWINFIRST = 0x0380;
        private const int WM_PENWINLAST = 0x038F;
        private const int WM_APP = unchecked((int)0x8000);
        private const int WM_USER = 0x0400;
        private const int EM_GETLIMITTEXT = (WM_USER + 37);
        private const int EM_POSFROMCHAR = (WM_USER + 38);
        private const int EM_CHARFROMPOS = (WM_USER + 39);
        private const int EM_SCROLLCARET = (WM_USER + 49);
        private const int EM_CANPASTE = (WM_USER + 50);
        private const int EM_DISPLAYBAND = (WM_USER + 51);
        private const int EM_EXGETSEL = (WM_USER + 52);
        private const int EM_EXLIMITTEXT = (WM_USER + 53);
        private const int EM_EXLINEFROMCHAR = (WM_USER + 54);
        private const int EM_EXSETSEL = (WM_USER + 55);
        private const int EM_FINDTEXT = (WM_USER + 56);
        private const int EM_FORMATRANGE = (WM_USER + 57);
        private const int EM_GETCHARFORMAT = (WM_USER + 58);
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_GETOLEINTERFACE = (WM_USER + 60);
        private const int EM_GETPARAFORMAT = (WM_USER + 61);
        private const int EM_GETSELTEXT = (WM_USER + 62);
        private const int EM_HIDESELECTION = (WM_USER + 63);
        private const int EM_PASTESPECIAL = (WM_USER + 64);
        private const int EM_REQUESTRESIZE = (WM_USER + 65);
        private const int EM_SELECTIONTYPE = (WM_USER + 66);
        private const int EM_SETBKGNDCOLOR = (WM_USER + 67);
        private const int EM_SETCHARFORMAT = (WM_USER + 68);
        private const int EM_SETEVENTMASK = (WM_USER + 69);
        private const int EM_SETOLECALLBACK = (WM_USER + 70);
        private const int EM_SETPARAFORMAT = (WM_USER + 71);
        private const int EM_SETTARGETDEVICE = (WM_USER + 72);
        private const int EM_STREAMIN = (WM_USER + 73);
        private const int EM_STREAMOUT = (WM_USER + 74);
        private const int EM_GETTEXTRANGE = (WM_USER + 75);
        private const int EM_FINDWORDBREAK = (WM_USER + 76);
        private const int EM_SETOPTIONS = (WM_USER + 77);
        private const int EM_GETOPTIONS = (WM_USER + 78);
        private const int EM_FINDTEXTEX = (WM_USER + 79);
        private const int EM_GETWORDBREAKPROCEX = (WM_USER + 80);
        private const int EM_SETWORDBREAKPROCEX = (WM_USER + 81);

        // Richedit v2.0 messages
        private const int EM_SETUNDOLIMIT = (WM_USER + 82);
        private const int EM_REDO = (WM_USER + 84);
        private const int EM_CANREDO = (WM_USER + 85);
        private const int EM_GETUNDONAME = (WM_USER + 86);
        private const int EM_GETREDONAME = (WM_USER + 87);
        private const int EM_STOPGROUPTYPING = (WM_USER + 88);
        private const int EM_SETTEXTMODE = (WM_USER + 89);
        private const int EM_GETTEXTMODE = (WM_USER + 90);
        private const int EM_AUTOURLDETECT = (WM_USER + 91);
        private const int EM_GETAUTOURLDETECT = (WM_USER + 92);
        private const int EM_SETPALETTE = (WM_USER + 93);
        private const int EM_GETTEXTEX = (WM_USER + 94);
        private const int EM_GETTEXTLENGTHEX = (WM_USER + 95);

        // Asia specific messages
        private const int EM_SETPUNCTUATION = (WM_USER + 100);
        private const int EM_GETPUNCTUATION = (WM_USER + 101);
        private const int EM_SETWORDWRAPMODE = (WM_USER + 102);
        private const int EM_GETWORDWRAPMODE = (WM_USER + 103);
        private const int EM_SETIMECOLOR = (WM_USER + 104);
        private const int EM_GETIMECOLOR = (WM_USER + 105);
        private const int EM_SETIMEOPTIONS = (WM_USER + 106);
        private const int EM_GETIMEOPTIONS = (WM_USER + 107);
        private const int EM_CONVPOSITION = (WM_USER + 108);
        private const int EM_SETLANGOPTIONS = (WM_USER + 120);
        private const int EM_GETLANGOPTIONS = (WM_USER + 121);
        private const int EM_GETIMECOMPMODE = (WM_USER + 122);
        private const int EM_FINDTEXTW = (WM_USER + 123);
        private const int EM_FINDTEXTEXW = (WM_USER + 124);

        // Rich TextBox 3.0 Asia msgs
        private const int EM_RECONVERSION = (WM_USER + 125);
        private const int EM_SETIMEMODEBIAS = (WM_USER + 126);
        private const int EM_GETIMEMODEBIAS = (WM_USER + 127);

        // BiDi Specific messages
        private const int EM_SETBIDIOPTIONS = (WM_USER + 200);
        private const int EM_GETBIDIOPTIONS = (WM_USER + 201);
        private const int EM_SETTYPOGRAPHYOPTIONS = (WM_USER + 202);
        private const int EM_GETTYPOGRAPHYOPTIONS = (WM_USER + 203);

        // Extended TextBox style specific messages
        private const int EM_SETEDITSTYLE = (WM_USER + 204);
        private const int EM_GETEDITSTYLE = (WM_USER + 205);
    }
}
