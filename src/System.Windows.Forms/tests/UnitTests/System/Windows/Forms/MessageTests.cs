// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

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
                TestStruct lparam = Assert.IsType<TestStruct>(message.GetLParam(typeof(TestStruct)));
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

            yield return new object[] { WindowMessages.WM_NULL, " (WM_NULL)" };
            yield return new object[] { WindowMessages.WM_CREATE, " (WM_CREATE)" };
            yield return new object[] { WindowMessages.WM_DESTROY, " (WM_DESTROY)" };
            yield return new object[] { WindowMessages.WM_MOVE, " (WM_MOVE)" };
            yield return new object[] { WindowMessages.WM_SIZE, " (WM_SIZE)" };
            yield return new object[] { WindowMessages.WM_ACTIVATE, " (WM_ACTIVATE)" };
            yield return new object[] { WindowMessages.WM_SETFOCUS, " (WM_SETFOCUS)" };
            yield return new object[] { WindowMessages.WM_KILLFOCUS, " (WM_KILLFOCUS)" };
            yield return new object[] { WindowMessages.WM_ENABLE, " (WM_ENABLE)" };
            yield return new object[] { WindowMessages.WM_SETREDRAW, " (WM_SETREDRAW)" };
            yield return new object[] { WindowMessages.WM_SETTEXT, " (WM_SETTEXT)" };
            yield return new object[] { WindowMessages.WM_GETTEXT, " (WM_GETTEXT)" };
            yield return new object[] { WindowMessages.WM_GETTEXTLENGTH, " (WM_GETTEXTLENGTH)" };
            yield return new object[] { WindowMessages.WM_PAINT, " (WM_PAINT)" };
            yield return new object[] { WindowMessages.WM_CLOSE, " (WM_CLOSE)" };
            yield return new object[] { WindowMessages.WM_QUERYENDSESSION, " (WM_QUERYENDSESSION)" };
            yield return new object[] { WindowMessages.WM_QUIT, " (WM_QUIT)" };
            yield return new object[] { WindowMessages.WM_QUERYOPEN, " (WM_QUERYOPEN)" };
            yield return new object[] { WindowMessages.WM_ERASEBKGND, " (WM_ERASEBKGND)" };
            yield return new object[] { WindowMessages.WM_SYSCOLORCHANGE, " (WM_SYSCOLORCHANGE)" };
            yield return new object[] { WindowMessages.WM_ENDSESSION, " (WM_ENDSESSION)" };
            yield return new object[] { WindowMessages.WM_SHOWWINDOW, " (WM_SHOWWINDOW)" };
            yield return new object[] { WindowMessages.WM_WININICHANGE, " (WM_WININICHANGE)" };
            yield return new object[] { WindowMessages.WM_DEVMODECHANGE, " (WM_DEVMODECHANGE)" };
            yield return new object[] { WindowMessages.WM_ACTIVATEAPP, " (WM_ACTIVATEAPP)" };
            yield return new object[] { WindowMessages.WM_FONTCHANGE, " (WM_FONTCHANGE)" };
            yield return new object[] { WindowMessages.WM_TIMECHANGE, " (WM_TIMECHANGE)" };
            yield return new object[] { WindowMessages.WM_CANCELMODE, " (WM_CANCELMODE)" };
            yield return new object[] { WindowMessages.WM_SETCURSOR, " (WM_SETCURSOR)" };
            yield return new object[] { WindowMessages.WM_MOUSEACTIVATE, " (WM_MOUSEACTIVATE)" };
            yield return new object[] { WindowMessages.WM_CHILDACTIVATE, " (WM_CHILDACTIVATE)" };
            yield return new object[] { WindowMessages.WM_QUEUESYNC, " (WM_QUEUESYNC)" };
            yield return new object[] { WindowMessages.WM_GETMINMAXINFO, " (WM_GETMINMAXINFO)" };
            yield return new object[] { WindowMessages.WM_PAINTICON, " (WM_PAINTICON)" };
            yield return new object[] { WindowMessages.WM_ICONERASEBKGND, " (WM_ICONERASEBKGND)" };
            yield return new object[] { WindowMessages.WM_NEXTDLGCTL, " (WM_NEXTDLGCTL)" };
            yield return new object[] { WindowMessages.WM_SPOOLERSTATUS, " (WM_SPOOLERSTATUS)" };
            yield return new object[] { WindowMessages.WM_DRAWITEM, " (WM_DRAWITEM)" };
            yield return new object[] { WindowMessages.WM_MEASUREITEM, " (WM_MEASUREITEM)" };
            yield return new object[] { WindowMessages.WM_DELETEITEM, " (WM_DELETEITEM)" };
            yield return new object[] { WindowMessages.WM_VKEYTOITEM, " (WM_VKEYTOITEM)" };
            yield return new object[] { WindowMessages.WM_CHARTOITEM, " (WM_CHARTOITEM)" };
            yield return new object[] { WindowMessages.WM_SETFONT, " (WM_SETFONT)" };
            yield return new object[] { WindowMessages.WM_GETFONT, " (WM_GETFONT)" };
            yield return new object[] { WindowMessages.WM_SETHOTKEY, " (WM_SETHOTKEY)" };
            yield return new object[] { WindowMessages.WM_GETHOTKEY, " (WM_GETHOTKEY)" };
            yield return new object[] { WindowMessages.WM_QUERYDRAGICON, " (WM_QUERYDRAGICON)" };
            yield return new object[] { WindowMessages.WM_COMPAREITEM, " (WM_COMPAREITEM)" };
            yield return new object[] { WindowMessages.WM_GETOBJECT, " (WM_GETOBJECT)" };
            yield return new object[] { WindowMessages.WM_COMPACTING, " (WM_COMPACTING)" };
            yield return new object[] { WindowMessages.WM_COMMNOTIFY, " (WM_COMMNOTIFY)" };
            yield return new object[] { WindowMessages.WM_WINDOWPOSCHANGING, " (WM_WINDOWPOSCHANGING)" };
            yield return new object[] { WindowMessages.WM_WINDOWPOSCHANGED, " (WM_WINDOWPOSCHANGED)" };
            yield return new object[] { WindowMessages.WM_POWER, " (WM_POWER)" };
            yield return new object[] { WindowMessages.WM_COPYDATA, " (WM_COPYDATA)" };
            yield return new object[] { WindowMessages.WM_CANCELJOURNAL, " (WM_CANCELJOURNAL)" };
            yield return new object[] { WindowMessages.WM_NOTIFY, " (WM_NOTIFY)" };
            yield return new object[] { WindowMessages.WM_INPUTLANGCHANGEREQUEST, " (WM_INPUTLANGCHANGEREQUEST)" };
            yield return new object[] { WindowMessages.WM_INPUTLANGCHANGE, " (WM_INPUTLANGCHANGE)" };
            yield return new object[] { WindowMessages.WM_TCARD, " (WM_TCARD)" };
            yield return new object[] { WindowMessages.WM_HELP, " (WM_HELP)" };
            yield return new object[] { WindowMessages.WM_USERCHANGED, " (WM_USERCHANGED)" };
            yield return new object[] { WindowMessages.WM_NOTIFYFORMAT, " (WM_NOTIFYFORMAT)" };
            yield return new object[] { WindowMessages.WM_CONTEXTMENU, " (WM_CONTEXTMENU)" };
            yield return new object[] { WindowMessages.WM_STYLECHANGING, " (WM_STYLECHANGING)" };
            yield return new object[] { WindowMessages.WM_STYLECHANGED, " (WM_STYLECHANGED)" };
            yield return new object[] { WindowMessages.WM_DISPLAYCHANGE, " (WM_DISPLAYCHANGE)" };
            yield return new object[] { WindowMessages.WM_GETICON, " (WM_GETICON)" };
            yield return new object[] { WindowMessages.WM_SETICON, " (WM_SETICON)" };
            yield return new object[] { WindowMessages.WM_NCCREATE, " (WM_NCCREATE)" };
            yield return new object[] { WindowMessages.WM_NCDESTROY, " (WM_NCDESTROY)" };
            yield return new object[] { WindowMessages.WM_NCCALCSIZE, " (WM_NCCALCSIZE)" };
            yield return new object[] { WindowMessages.WM_NCHITTEST, " (WM_NCHITTEST)" };
            yield return new object[] { WindowMessages.WM_NCPAINT, " (WM_NCPAINT)" };
            yield return new object[] { WindowMessages.WM_NCACTIVATE, " (WM_NCACTIVATE)" };
            yield return new object[] { WindowMessages.WM_GETDLGCODE, " (WM_GETDLGCODE)" };
            yield return new object[] { WindowMessages.WM_NCMOUSEMOVE, " (WM_NCMOUSEMOVE)" };
            yield return new object[] { WindowMessages.WM_NCLBUTTONDOWN, " (WM_NCLBUTTONDOWN)" };
            yield return new object[] { WindowMessages.WM_NCLBUTTONUP, " (WM_NCLBUTTONUP)" };
            yield return new object[] { WindowMessages.WM_NCLBUTTONDBLCLK, " (WM_NCLBUTTONDBLCLK)" };
            yield return new object[] { WindowMessages.WM_NCRBUTTONDOWN, " (WM_NCRBUTTONDOWN)" };
            yield return new object[] { WindowMessages.WM_NCRBUTTONUP, " (WM_NCRBUTTONUP)" };
            yield return new object[] { WindowMessages.WM_NCRBUTTONDBLCLK, " (WM_NCRBUTTONDBLCLK)" };
            yield return new object[] { WindowMessages.WM_NCMBUTTONDOWN, " (WM_NCMBUTTONDOWN)" };
            yield return new object[] { WindowMessages.WM_NCMBUTTONUP, " (WM_NCMBUTTONUP)" };
            yield return new object[] { WindowMessages.WM_NCMBUTTONDBLCLK, " (WM_NCMBUTTONDBLCLK)" };
            yield return new object[] { WindowMessages.WM_KEYDOWN, " (WM_KEYDOWN)" };
            yield return new object[] { WindowMessages.WM_KEYUP, " (WM_KEYUP)" };
            yield return new object[] { WindowMessages.WM_CHAR, " (WM_CHAR)" };
            yield return new object[] { WindowMessages.WM_DEADCHAR, " (WM_DEADCHAR)" };
            yield return new object[] { WindowMessages.WM_SYSKEYDOWN, " (WM_SYSKEYDOWN)" };
            yield return new object[] { WindowMessages.WM_SYSKEYUP, " (WM_SYSKEYUP)" };
            yield return new object[] { WindowMessages.WM_SYSCHAR, " (WM_SYSCHAR)" };
            yield return new object[] { WindowMessages.WM_SYSDEADCHAR, " (WM_SYSDEADCHAR)" };
            yield return new object[] { WindowMessages.WM_KEYLAST, " (WM_KEYLAST)" };
            yield return new object[] { WindowMessages.WM_IME_STARTCOMPOSITION, " (WM_IME_STARTCOMPOSITION)" };
            yield return new object[] { WindowMessages.WM_IME_ENDCOMPOSITION, " (WM_IME_ENDCOMPOSITION)" };
            yield return new object[] { WindowMessages.WM_IME_COMPOSITION, " (WM_IME_COMPOSITION)" };
            yield return new object[] { WindowMessages.WM_INITDIALOG, " (WM_INITDIALOG)" };
            yield return new object[] { WindowMessages.WM_COMMAND, " (WM_COMMAND)" };
            yield return new object[] { WindowMessages.WM_SYSCOMMAND, " (WM_SYSCOMMAND)" };
            yield return new object[] { WindowMessages.WM_TIMER, " (WM_TIMER)" };
            yield return new object[] { WindowMessages.WM_HSCROLL, " (WM_HSCROLL)" };
            yield return new object[] { WindowMessages.WM_VSCROLL, " (WM_VSCROLL)" };
            yield return new object[] { WindowMessages.WM_INITMENU, " (WM_INITMENU)" };
            yield return new object[] { WindowMessages.WM_INITMENUPOPUP, " (WM_INITMENUPOPUP)" };
            yield return new object[] { WindowMessages.WM_MENUSELECT, " (WM_MENUSELECT)" };
            yield return new object[] { WindowMessages.WM_MENUCHAR, " (WM_MENUCHAR)" };
            yield return new object[] { WindowMessages.WM_ENTERIDLE, " (WM_ENTERIDLE)" };
            yield return new object[] { WindowMessages.WM_CTLCOLORMSGBOX, " (WM_CTLCOLORMSGBOX)" };
            yield return new object[] { WindowMessages.WM_CTLCOLOREDIT, " (WM_CTLCOLOREDIT)" };
            yield return new object[] { WindowMessages.WM_CTLCOLORLISTBOX, " (WM_CTLCOLORLISTBOX)" };
            yield return new object[] { WindowMessages.WM_CTLCOLORBTN, " (WM_CTLCOLORBTN)" };
            yield return new object[] { WindowMessages.WM_CTLCOLORDLG, " (WM_CTLCOLORDLG)" };
            yield return new object[] { WindowMessages.WM_CTLCOLORSCROLLBAR, " (WM_CTLCOLORSCROLLBAR)" };
            yield return new object[] { WindowMessages.WM_CTLCOLORSTATIC, " (WM_CTLCOLORSTATIC)" };
            yield return new object[] { WindowMessages.WM_MOUSEMOVE, " (WM_MOUSEMOVE)" };
            yield return new object[] { WindowMessages.WM_LBUTTONDOWN, " (WM_LBUTTONDOWN)" };
            yield return new object[] { WindowMessages.WM_LBUTTONUP, " (WM_LBUTTONUP)" };
            yield return new object[] { WindowMessages.WM_LBUTTONDBLCLK, " (WM_LBUTTONDBLCLK)" };
            yield return new object[] { WindowMessages.WM_RBUTTONDOWN, " (WM_RBUTTONDOWN)" };
            yield return new object[] { WindowMessages.WM_RBUTTONUP, " (WM_RBUTTONUP)" };
            yield return new object[] { WindowMessages.WM_RBUTTONDBLCLK, " (WM_RBUTTONDBLCLK)" };
            yield return new object[] { WindowMessages.WM_MBUTTONDOWN, " (WM_MBUTTONDOWN)" };
            yield return new object[] { WindowMessages.WM_MBUTTONUP, " (WM_MBUTTONUP)" };
            yield return new object[] { WindowMessages.WM_MBUTTONDBLCLK, " (WM_MBUTTONDBLCLK)" };
            yield return new object[] { WindowMessages.WM_MOUSEWHEEL, " (WM_MOUSEWHEEL)" };
            yield return new object[] { WindowMessages.WM_PARENTNOTIFY, " (WM_PARENTNOTIFY)", " (WM_DESTROY)" };
            yield return new object[] { WindowMessages.WM_ENTERMENULOOP, " (WM_ENTERMENULOOP)" };
            yield return new object[] { WindowMessages.WM_EXITMENULOOP, " (WM_EXITMENULOOP)" };
            yield return new object[] { WindowMessages.WM_NEXTMENU, " (WM_NEXTMENU)" };
            yield return new object[] { WindowMessages.WM_SIZING, " (WM_SIZING)" };
            yield return new object[] { WindowMessages.WM_CAPTURECHANGED, " (WM_CAPTURECHANGED)" };
            yield return new object[] { WindowMessages.WM_MOVING, " (WM_MOVING)" };
            yield return new object[] { WindowMessages.WM_POWERBROADCAST, " (WM_POWERBROADCAST)" };
            yield return new object[] { WindowMessages.WM_DEVICECHANGE, " (WM_DEVICECHANGE)" };
            yield return new object[] { WindowMessages.WM_IME_SETCONTEXT, " (WM_IME_SETCONTEXT)" };
            yield return new object[] { WindowMessages.WM_IME_NOTIFY, " (WM_IME_NOTIFY)" };
            yield return new object[] { WindowMessages.WM_IME_CONTROL, " (WM_IME_CONTROL)" };
            yield return new object[] { WindowMessages.WM_IME_COMPOSITIONFULL, " (WM_IME_COMPOSITIONFULL)" };
            yield return new object[] { WindowMessages.WM_IME_SELECT, " (WM_IME_SELECT)" };
            yield return new object[] { WindowMessages.WM_IME_CHAR, " (WM_IME_CHAR)" };
            yield return new object[] { WindowMessages.WM_IME_KEYDOWN, " (WM_IME_KEYDOWN)" };
            yield return new object[] { WindowMessages.WM_IME_KEYUP, " (WM_IME_KEYUP)" };
            yield return new object[] { WindowMessages.WM_MDICREATE, " (WM_MDICREATE)" };
            yield return new object[] { WindowMessages.WM_MDIDESTROY, " (WM_MDIDESTROY)" };
            yield return new object[] { WindowMessages.WM_MDIACTIVATE, " (WM_MDIACTIVATE)" };
            yield return new object[] { WindowMessages.WM_MDIRESTORE, " (WM_MDIRESTORE)" };
            yield return new object[] { WindowMessages.WM_MDINEXT, " (WM_MDINEXT)" };
            yield return new object[] { WindowMessages.WM_MDIMAXIMIZE, " (WM_MDIMAXIMIZE)" };
            yield return new object[] { WindowMessages.WM_MDITILE, " (WM_MDITILE)" };
            yield return new object[] { WindowMessages.WM_MDICASCADE, " (WM_MDICASCADE)" };
            yield return new object[] { WindowMessages.WM_MDIICONARRANGE, " (WM_MDIICONARRANGE)" };
            yield return new object[] { WindowMessages.WM_MDIGETACTIVE, " (WM_MDIGETACTIVE)" };
            yield return new object[] { WindowMessages.WM_MDISETMENU, " (WM_MDISETMENU)" };
            yield return new object[] { WindowMessages.WM_ENTERSIZEMOVE, " (WM_ENTERSIZEMOVE)" };
            yield return new object[] { WindowMessages.WM_EXITSIZEMOVE, " (WM_EXITSIZEMOVE)" };
            yield return new object[] { WindowMessages.WM_DROPFILES, " (WM_DROPFILES)" };
            yield return new object[] { WindowMessages.WM_MDIREFRESHMENU, " (WM_MDIREFRESHMENU)" };
            yield return new object[] { WindowMessages.WM_MOUSEHOVER, " (WM_MOUSEHOVER)" };
            yield return new object[] { WindowMessages.WM_MOUSELEAVE, " (WM_MOUSELEAVE)" };
            yield return new object[] { WindowMessages.WM_CUT, " (WM_CUT)" };
            yield return new object[] { WindowMessages.WM_COPY, " (WM_COPY)" };
            yield return new object[] { WindowMessages.WM_PASTE, " (WM_PASTE)" };
            yield return new object[] { WindowMessages.WM_CLEAR, " (WM_CLEAR)" };
            yield return new object[] { WindowMessages.WM_UNDO, " (WM_UNDO)" };
            yield return new object[] { WindowMessages.WM_RENDERFORMAT, " (WM_RENDERFORMAT)" };
            yield return new object[] { WindowMessages.WM_RENDERALLFORMATS, " (WM_RENDERALLFORMATS)" };
            yield return new object[] { WindowMessages.WM_DESTROYCLIPBOARD, " (WM_DESTROYCLIPBOARD)" };
            yield return new object[] { WindowMessages.WM_DRAWCLIPBOARD, " (WM_DRAWCLIPBOARD)" };
            yield return new object[] { WindowMessages.WM_PAINTCLIPBOARD, " (WM_PAINTCLIPBOARD)" };
            yield return new object[] { WindowMessages.WM_VSCROLLCLIPBOARD, " (WM_VSCROLLCLIPBOARD)" };
            yield return new object[] { WindowMessages.WM_SIZECLIPBOARD, " (WM_SIZECLIPBOARD)" };
            yield return new object[] { WindowMessages.WM_ASKCBFORMATNAME, " (WM_ASKCBFORMATNAME)" };
            yield return new object[] { WindowMessages.WM_CHANGECBCHAIN, " (WM_CHANGECBCHAIN)" };
            yield return new object[] { WindowMessages.WM_HSCROLLCLIPBOARD, " (WM_HSCROLLCLIPBOARD)" };
            yield return new object[] { WindowMessages.WM_QUERYNEWPALETTE, " (WM_QUERYNEWPALETTE)" };
            yield return new object[] { WindowMessages.WM_PALETTEISCHANGING, " (WM_PALETTEISCHANGING)" };
            yield return new object[] { WindowMessages.WM_PALETTECHANGED, " (WM_PALETTECHANGED)" };
            yield return new object[] { WindowMessages.WM_HOTKEY, " (WM_HOTKEY)" };
            yield return new object[] { WindowMessages.WM_PRINT, " (WM_PRINT)" };
            yield return new object[] { WindowMessages.WM_PRINTCLIENT, " (WM_PRINTCLIENT)" };
            yield return new object[] { WindowMessages.WM_HANDHELDFIRST, " (WM_HANDHELDFIRST)" };
            yield return new object[] { WindowMessages.WM_HANDHELDLAST, " (WM_HANDHELDLAST)" };
            yield return new object[] { WindowMessages.WM_AFXFIRST, " (WM_AFXFIRST)" };
            yield return new object[] { WindowMessages.WM_AFXLAST, " (WM_AFXLAST)" };
            yield return new object[] { WindowMessages.WM_PENWINFIRST, " (WM_PENWINFIRST)" };
            yield return new object[] { WindowMessages.WM_PENWINLAST, " (WM_PENWINLAST)" };
            yield return new object[] { WindowMessages.WM_APP, " (WM_APP)" };
            yield return new object[] { WindowMessages.WM_USER, " (WM_USER)" };
            yield return new object[] { WindowMessages.WM_CTLCOLOR, " (WM_CTLCOLOR)" };

            // RichEdit messages
            yield return new object[] { EditMessages.EM_GETLIMITTEXT, " (EM_GETLIMITTEXT)" };
            yield return new object[] { EditMessages.EM_POSFROMCHAR, " (EM_POSFROMCHAR)" };
            yield return new object[] { EditMessages.EM_CHARFROMPOS, " (EM_CHARFROMPOS)" };
            yield return new object[] { EditMessages.EM_SCROLLCARET, " (EM_SCROLLCARET)" };
            yield return new object[] { RichEditMessages.EM_CANPASTE, " (EM_CANPASTE)" };
            yield return new object[] { RichEditMessages.EM_DISPLAYBAND, " (EM_DISPLAYBAND)" };
            yield return new object[] { RichEditMessages.EM_EXGETSEL, " (EM_EXGETSEL)" };
            yield return new object[] { RichEditMessages.EM_EXLIMITTEXT, " (EM_EXLIMITTEXT)" };
            yield return new object[] { RichEditMessages.EM_EXLINEFROMCHAR, " (EM_EXLINEFROMCHAR)" };
            yield return new object[] { RichEditMessages.EM_EXSETSEL, " (EM_EXSETSEL)" };
            yield return new object[] { RichEditMessages.EM_FINDTEXT, " (EM_FINDTEXT)" };
            yield return new object[] { RichEditMessages.EM_FORMATRANGE, " (EM_FORMATRANGE)" };
            yield return new object[] { RichEditMessages.EM_GETCHARFORMAT, " (EM_GETCHARFORMAT)" };
            yield return new object[] { RichEditMessages.EM_GETEVENTMASK, " (EM_GETEVENTMASK)" };
            yield return new object[] { RichEditMessages.EM_GETOLEINTERFACE, " (EM_GETOLEINTERFACE)" };
            yield return new object[] { RichEditMessages.EM_GETPARAFORMAT, " (EM_GETPARAFORMAT)" };
            yield return new object[] { RichEditMessages.EM_GETSELTEXT, " (EM_GETSELTEXT)" };
            yield return new object[] { RichEditMessages.EM_HIDESELECTION, " (EM_HIDESELECTION)" };
            yield return new object[] { RichEditMessages.EM_PASTESPECIAL, " (EM_PASTESPECIAL)" };
            yield return new object[] { RichEditMessages.EM_REQUESTRESIZE, " (EM_REQUESTRESIZE)" };
            yield return new object[] { RichEditMessages.EM_SELECTIONTYPE, " (EM_SELECTIONTYPE)" };
            yield return new object[] { RichEditMessages.EM_SETBKGNDCOLOR, " (EM_SETBKGNDCOLOR)" };
            yield return new object[] { RichEditMessages.EM_SETCHARFORMAT, " (EM_SETCHARFORMAT)" };
            yield return new object[] { RichEditMessages.EM_SETEVENTMASK, " (EM_SETEVENTMASK)" };
            yield return new object[] { RichEditMessages.EM_SETOLECALLBACK, " (EM_SETOLECALLBACK)" };
            yield return new object[] { RichEditMessages.EM_SETPARAFORMAT, " (EM_SETPARAFORMAT)" };
            yield return new object[] { RichEditMessages.EM_SETTARGETDEVICE, " (EM_SETTARGETDEVICE)" };
            yield return new object[] { RichEditMessages.EM_STREAMIN, " (EM_STREAMIN)" };
            yield return new object[] { RichEditMessages.EM_STREAMOUT, " (EM_STREAMOUT)" };
            yield return new object[] { RichEditMessages.EM_GETTEXTRANGE, " (EM_GETTEXTRANGE)" };
            yield return new object[] { RichEditMessages.EM_FINDWORDBREAK, " (EM_FINDWORDBREAK)" };
            yield return new object[] { RichEditMessages.EM_SETOPTIONS, " (EM_SETOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_GETOPTIONS, " (EM_GETOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_FINDTEXTEX, " (EM_FINDTEXTEX)" };
            yield return new object[] { RichEditMessages.EM_GETWORDBREAKPROCEX, " (EM_GETWORDBREAKPROCEX)" };
            yield return new object[] { RichEditMessages.EM_SETWORDBREAKPROCEX, " (EM_SETWORDBREAKPROCEX)" };

            // Richedit v2.0 messages
            yield return new object[] { RichEditMessages.EM_SETUNDOLIMIT, " (EM_SETUNDOLIMIT)" };
            yield return new object[] { RichEditMessages.EM_REDO, " (EM_REDO)" };
            yield return new object[] { RichEditMessages.EM_CANREDO, " (EM_CANREDO)" };
            yield return new object[] { RichEditMessages.EM_GETUNDONAME, " (EM_GETUNDONAME)" };
            yield return new object[] { RichEditMessages.EM_GETREDONAME, " (EM_GETREDONAME)" };
            yield return new object[] { RichEditMessages.EM_STOPGROUPTYPING, " (EM_STOPGROUPTYPING)" };
            yield return new object[] { RichEditMessages.EM_SETTEXTMODE, " (EM_SETTEXTMODE)" };
            yield return new object[] { RichEditMessages.EM_GETTEXTMODE, " (EM_GETTEXTMODE)" };
            yield return new object[] { RichEditMessages.EM_AUTOURLDETECT, " (EM_AUTOURLDETECT)" };
            yield return new object[] { RichEditMessages.EM_GETAUTOURLDETECT, " (EM_GETAUTOURLDETECT)" };
            yield return new object[] { RichEditMessages.EM_SETPALETTE, " (EM_SETPALETTE)" };
            yield return new object[] { RichEditMessages.EM_GETTEXTEX, " (EM_GETTEXTEX)" };
            yield return new object[] { RichEditMessages.EM_GETTEXTLENGTHEX, " (EM_GETTEXTLENGTHEX)" };

            // Asia specific messages
            yield return new object[] { RichEditMessages.EM_SETPUNCTUATION, " (EM_SETPUNCTUATION)" };
            yield return new object[] { RichEditMessages.EM_GETPUNCTUATION, " (EM_GETPUNCTUATION)" };
            yield return new object[] { RichEditMessages.EM_SETWORDWRAPMODE, " (EM_SETWORDWRAPMODE)" };
            yield return new object[] { RichEditMessages.EM_GETWORDWRAPMODE, " (EM_GETWORDWRAPMODE)" };
            yield return new object[] { RichEditMessages.EM_SETIMECOLOR, " (EM_SETIMECOLOR)" };
            yield return new object[] { RichEditMessages.EM_GETIMECOLOR, " (EM_GETIMECOLOR)" };
            yield return new object[] { RichEditMessages.EM_SETIMEOPTIONS, " (EM_SETIMEOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_GETIMEOPTIONS, " (EM_GETIMEOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_CONVPOSITION, " (EM_CONVPOSITION)" };
            yield return new object[] { RichEditMessages.EM_SETLANGOPTIONS, " (EM_SETLANGOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_GETLANGOPTIONS, " (EM_GETLANGOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_GETIMECOMPMODE, " (EM_GETIMECOMPMODE)" };
            yield return new object[] { RichEditMessages.EM_FINDTEXTW, " (EM_FINDTEXTW)" };
            yield return new object[] { RichEditMessages.EM_FINDTEXTEXW, " (EM_FINDTEXTEXW)" };

            // Rich Edit 3.0 Asia msgs
            yield return new object[] { RichEditMessages.EM_RECONVERSION, " (EM_RECONVERSION)" };
            yield return new object[] { RichEditMessages.EM_SETIMEMODEBIAS, " (EM_SETIMEMODEBIAS)" };
            yield return new object[] { RichEditMessages.EM_GETIMEMODEBIAS, " (EM_GETIMEMODEBIAS)" };

            // BiDi Specific messages
            yield return new object[] { RichEditMessages.EM_SETBIDIOPTIONS, " (EM_SETBIDIOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_GETBIDIOPTIONS, " (EM_GETBIDIOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_SETTYPOGRAPHYOPTIONS, " (EM_SETTYPOGRAPHYOPTIONS)" };
            yield return new object[] { RichEditMessages.EM_GETTYPOGRAPHYOPTIONS, " (EM_GETTYPOGRAPHYOPTIONS)" };

            // Extended Edit style specific messages
            yield return new object[] { RichEditMessages.EM_SETEDITSTYLE, " (EM_SETEDITSTYLE)" };
            yield return new object[] { RichEditMessages.EM_GETEDITSTYLE, " (EM_GETEDITSTYLE)" };
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public void Message_ToString_Invoke_ReturnsExpected(int msg, string expected, string additionalMsg = null)
        {
            Message message = Message.Create((IntPtr)1, msg, (IntPtr)2, (IntPtr)3);
            message.Result = (IntPtr)4;
            Assert.Equal("msg=0x" + Convert.ToString(msg, 16) + expected + " hwnd=0x1 wparam=0x2 lparam=0x3" + additionalMsg + " result=0x4", message.ToString());
        }
    }
}
