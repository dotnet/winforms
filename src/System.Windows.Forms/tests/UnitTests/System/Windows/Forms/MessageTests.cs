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
    // NB: doesn't require thread affinity
    public class MessageTests : IClassFixture<ThreadExceptionFixture>
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

            yield return new object[] { User32.WM.NULL, " (WM_NULL)" };
            yield return new object[] { User32.WM.CREATE, " (WM_CREATE)" };
            yield return new object[] { User32.WM.DESTROY, " (WM_DESTROY)" };
            yield return new object[] { User32.WM.MOVE, " (WM_MOVE)" };
            yield return new object[] { User32.WM.SIZE, " (WM_SIZE)" };
            yield return new object[] { User32.WM.ACTIVATE, " (WM_ACTIVATE)" };
            yield return new object[] { User32.WM.SETFOCUS, " (WM_SETFOCUS)" };
            yield return new object[] { User32.WM.KILLFOCUS, " (WM_KILLFOCUS)" };
            yield return new object[] { User32.WM.ENABLE, " (WM_ENABLE)" };
            yield return new object[] { User32.WM.SETREDRAW, " (WM_SETREDRAW)" };
            yield return new object[] { User32.WM.SETTEXT, " (WM_SETTEXT)" };
            yield return new object[] { User32.WM.GETTEXT, " (WM_GETTEXT)" };
            yield return new object[] { User32.WM.GETTEXTLENGTH, " (WM_GETTEXTLENGTH)" };
            yield return new object[] { User32.WM.PAINT, " (WM_PAINT)" };
            yield return new object[] { User32.WM.CLOSE, " (WM_CLOSE)" };
            yield return new object[] { User32.WM.QUERYENDSESSION, " (WM_QUERYENDSESSION)" };
            yield return new object[] { User32.WM.QUIT, " (WM_QUIT)" };
            yield return new object[] { User32.WM.QUERYOPEN, " (WM_QUERYOPEN)" };
            yield return new object[] { User32.WM.ERASEBKGND, " (WM_ERASEBKGND)" };
            yield return new object[] { User32.WM.SYSCOLORCHANGE, " (WM_SYSCOLORCHANGE)" };
            yield return new object[] { User32.WM.ENDSESSION, " (WM_ENDSESSION)" };
            yield return new object[] { User32.WM.SHOWWINDOW, " (WM_SHOWWINDOW)" };
            yield return new object[] { User32.WM.WININICHANGE, " (WM_WININICHANGE)" };
            yield return new object[] { User32.WM.DEVMODECHANGE, " (WM_DEVMODECHANGE)" };
            yield return new object[] { User32.WM.ACTIVATEAPP, " (WM_ACTIVATEAPP)" };
            yield return new object[] { User32.WM.FONTCHANGE, " (WM_FONTCHANGE)" };
            yield return new object[] { User32.WM.TIMECHANGE, " (WM_TIMECHANGE)" };
            yield return new object[] { User32.WM.CANCELMODE, " (WM_CANCELMODE)" };
            yield return new object[] { User32.WM.SETCURSOR, " (WM_SETCURSOR)" };
            yield return new object[] { User32.WM.MOUSEACTIVATE, " (WM_MOUSEACTIVATE)" };
            yield return new object[] { User32.WM.CHILDACTIVATE, " (WM_CHILDACTIVATE)" };
            yield return new object[] { User32.WM.QUEUESYNC, " (WM_QUEUESYNC)" };
            yield return new object[] { User32.WM.GETMINMAXINFO, " (WM_GETMINMAXINFO)" };
            yield return new object[] { User32.WM.PAINTICON, " (WM_PAINTICON)" };
            yield return new object[] { User32.WM.ICONERASEBKGND, " (WM_ICONERASEBKGND)" };
            yield return new object[] { User32.WM.NEXTDLGCTL, " (WM_NEXTDLGCTL)" };
            yield return new object[] { User32.WM.SPOOLERSTATUS, " (WM_SPOOLERSTATUS)" };
            yield return new object[] { User32.WM.DRAWITEM, " (WM_DRAWITEM)" };
            yield return new object[] { User32.WM.MEASUREITEM, " (WM_MEASUREITEM)" };
            yield return new object[] { User32.WM.DELETEITEM, " (WM_DELETEITEM)" };
            yield return new object[] { User32.WM.VKEYTOITEM, " (WM_VKEYTOITEM)" };
            yield return new object[] { User32.WM.CHARTOITEM, " (WM_CHARTOITEM)" };
            yield return new object[] { User32.WM.SETFONT, " (WM_SETFONT)" };
            yield return new object[] { User32.WM.GETFONT, " (WM_GETFONT)" };
            yield return new object[] { User32.WM.SETHOTKEY, " (WM_SETHOTKEY)" };
            yield return new object[] { User32.WM.GETHOTKEY, " (WM_GETHOTKEY)" };
            yield return new object[] { User32.WM.QUERYDRAGICON, " (WM_QUERYDRAGICON)" };
            yield return new object[] { User32.WM.COMPAREITEM, " (WM_COMPAREITEM)" };
            yield return new object[] { User32.WM.GETOBJECT, " (WM_GETOBJECT)" };
            yield return new object[] { User32.WM.COMPACTING, " (WM_COMPACTING)" };
            yield return new object[] { User32.WM.COMMNOTIFY, " (WM_COMMNOTIFY)" };
            yield return new object[] { User32.WM.WINDOWPOSCHANGING, " (WM_WINDOWPOSCHANGING)" };
            yield return new object[] { User32.WM.WINDOWPOSCHANGED, " (WM_WINDOWPOSCHANGED)" };
            yield return new object[] { User32.WM.POWER, " (WM_POWER)" };
            yield return new object[] { User32.WM.COPYDATA, " (WM_COPYDATA)" };
            yield return new object[] { User32.WM.CANCELJOURNAL, " (WM_CANCELJOURNAL)" };
            yield return new object[] { User32.WM.NOTIFY, " (WM_NOTIFY)" };
            yield return new object[] { User32.WM.INPUTLANGCHANGEREQUEST, " (WM_INPUTLANGCHANGEREQUEST)" };
            yield return new object[] { User32.WM.INPUTLANGCHANGE, " (WM_INPUTLANGCHANGE)" };
            yield return new object[] { User32.WM.TCARD, " (WM_TCARD)" };
            yield return new object[] { User32.WM.HELP, " (WM_HELP)" };
            yield return new object[] { User32.WM.USERCHANGED, " (WM_USERCHANGED)" };
            yield return new object[] { User32.WM.NOTIFYFORMAT, " (WM_NOTIFYFORMAT)" };
            yield return new object[] { User32.WM.CONTEXTMENU, " (WM_CONTEXTMENU)" };
            yield return new object[] { User32.WM.STYLECHANGING, " (WM_STYLECHANGING)" };
            yield return new object[] { User32.WM.STYLECHANGED, " (WM_STYLECHANGED)" };
            yield return new object[] { User32.WM.DISPLAYCHANGE, " (WM_DISPLAYCHANGE)" };
            yield return new object[] { User32.WM.GETICON, " (WM_GETICON)" };
            yield return new object[] { User32.WM.SETICON, " (WM_SETICON)" };
            yield return new object[] { User32.WM.NCCREATE, " (WM_NCCREATE)" };
            yield return new object[] { User32.WM.NCDESTROY, " (WM_NCDESTROY)" };
            yield return new object[] { User32.WM.NCCALCSIZE, " (WM_NCCALCSIZE)" };
            yield return new object[] { User32.WM.NCHITTEST, " (WM_NCHITTEST)" };
            yield return new object[] { User32.WM.NCPAINT, " (WM_NCPAINT)" };
            yield return new object[] { User32.WM.NCACTIVATE, " (WM_NCACTIVATE)" };
            yield return new object[] { User32.WM.GETDLGCODE, " (WM_GETDLGCODE)" };
            yield return new object[] { User32.WM.NCMOUSEMOVE, " (WM_NCMOUSEMOVE)" };
            yield return new object[] { User32.WM.NCLBUTTONDOWN, " (WM_NCLBUTTONDOWN)" };
            yield return new object[] { User32.WM.NCLBUTTONUP, " (WM_NCLBUTTONUP)" };
            yield return new object[] { User32.WM.NCLBUTTONDBLCLK, " (WM_NCLBUTTONDBLCLK)" };
            yield return new object[] { User32.WM.NCRBUTTONDOWN, " (WM_NCRBUTTONDOWN)" };
            yield return new object[] { User32.WM.NCRBUTTONUP, " (WM_NCRBUTTONUP)" };
            yield return new object[] { User32.WM.NCRBUTTONDBLCLK, " (WM_NCRBUTTONDBLCLK)" };
            yield return new object[] { User32.WM.NCMBUTTONDOWN, " (WM_NCMBUTTONDOWN)" };
            yield return new object[] { User32.WM.NCMBUTTONUP, " (WM_NCMBUTTONUP)" };
            yield return new object[] { User32.WM.NCMBUTTONDBLCLK, " (WM_NCMBUTTONDBLCLK)" };
            yield return new object[] { User32.WM.KEYDOWN, " (WM_KEYDOWN)" };
            yield return new object[] { User32.WM.KEYUP, " (WM_KEYUP)" };
            yield return new object[] { User32.WM.CHAR, " (WM_CHAR)" };
            yield return new object[] { User32.WM.DEADCHAR, " (WM_DEADCHAR)" };
            yield return new object[] { User32.WM.SYSKEYDOWN, " (WM_SYSKEYDOWN)" };
            yield return new object[] { User32.WM.SYSKEYUP, " (WM_SYSKEYUP)" };
            yield return new object[] { User32.WM.SYSCHAR, " (WM_SYSCHAR)" };
            yield return new object[] { User32.WM.SYSDEADCHAR, " (WM_SYSDEADCHAR)" };
            yield return new object[] { User32.WM.KEYLAST, " (WM_KEYLAST)" };
            yield return new object[] { User32.WM.IME_STARTCOMPOSITION, " (WM_IME_STARTCOMPOSITION)" };
            yield return new object[] { User32.WM.IME_ENDCOMPOSITION, " (WM_IME_ENDCOMPOSITION)" };
            yield return new object[] { User32.WM.IME_COMPOSITION, " (WM_IME_COMPOSITION)" };
            yield return new object[] { User32.WM.INITDIALOG, " (WM_INITDIALOG)" };
            yield return new object[] { User32.WM.COMMAND, " (WM_COMMAND)" };
            yield return new object[] { User32.WM.SYSCOMMAND, " (WM_SYSCOMMAND)" };
            yield return new object[] { User32.WM.TIMER, " (WM_TIMER)" };
            yield return new object[] { User32.WM.HSCROLL, " (WM_HSCROLL)" };
            yield return new object[] { User32.WM.VSCROLL, " (WM_VSCROLL)" };
            yield return new object[] { User32.WM.INITMENU, " (WM_INITMENU)" };
            yield return new object[] { User32.WM.INITMENUPOPUP, " (WM_INITMENUPOPUP)" };
            yield return new object[] { User32.WM.MENUSELECT, " (WM_MENUSELECT)" };
            yield return new object[] { User32.WM.MENUCHAR, " (WM_MENUCHAR)" };
            yield return new object[] { User32.WM.ENTERIDLE, " (WM_ENTERIDLE)" };
            yield return new object[] { User32.WM.CTLCOLORMSGBOX, " (WM_CTLCOLORMSGBOX)" };
            yield return new object[] { User32.WM.CTLCOLOREDIT, " (WM_CTLCOLOREDIT)" };
            yield return new object[] { User32.WM.CTLCOLORLISTBOX, " (WM_CTLCOLORLISTBOX)" };
            yield return new object[] { User32.WM.CTLCOLORBTN, " (WM_CTLCOLORBTN)" };
            yield return new object[] { User32.WM.CTLCOLORDLG, " (WM_CTLCOLORDLG)" };
            yield return new object[] { User32.WM.CTLCOLORSCROLLBAR, " (WM_CTLCOLORSCROLLBAR)" };
            yield return new object[] { User32.WM.CTLCOLORSTATIC, " (WM_CTLCOLORSTATIC)" };
            yield return new object[] { User32.WM.MOUSEMOVE, " (WM_MOUSEMOVE)" };
            yield return new object[] { User32.WM.LBUTTONDOWN, " (WM_LBUTTONDOWN)" };
            yield return new object[] { User32.WM.LBUTTONUP, " (WM_LBUTTONUP)" };
            yield return new object[] { User32.WM.LBUTTONDBLCLK, " (WM_LBUTTONDBLCLK)" };
            yield return new object[] { User32.WM.RBUTTONDOWN, " (WM_RBUTTONDOWN)" };
            yield return new object[] { User32.WM.RBUTTONUP, " (WM_RBUTTONUP)" };
            yield return new object[] { User32.WM.RBUTTONDBLCLK, " (WM_RBUTTONDBLCLK)" };
            yield return new object[] { User32.WM.MBUTTONDOWN, " (WM_MBUTTONDOWN)" };
            yield return new object[] { User32.WM.MBUTTONUP, " (WM_MBUTTONUP)" };
            yield return new object[] { User32.WM.MBUTTONDBLCLK, " (WM_MBUTTONDBLCLK)" };
            yield return new object[] { User32.WM.MOUSEWHEEL, " (WM_MOUSEWHEEL)" };
            yield return new object[] { User32.WM.PARENTNOTIFY, " (WM_PARENTNOTIFY)", " (WM_DESTROY)" };
            yield return new object[] { User32.WM.ENTERMENULOOP, " (WM_ENTERMENULOOP)" };
            yield return new object[] { User32.WM.EXITMENULOOP, " (WM_EXITMENULOOP)" };
            yield return new object[] { User32.WM.NEXTMENU, " (WM_NEXTMENU)" };
            yield return new object[] { User32.WM.SIZING, " (WM_SIZING)" };
            yield return new object[] { User32.WM.CAPTURECHANGED, " (WM_CAPTURECHANGED)" };
            yield return new object[] { User32.WM.MOVING, " (WM_MOVING)" };
            yield return new object[] { User32.WM.POWERBROADCAST, " (WM_POWERBROADCAST)" };
            yield return new object[] { User32.WM.DEVICECHANGE, " (WM_DEVICECHANGE)" };
            yield return new object[] { User32.WM.IME_SETCONTEXT, " (WM_IME_SETCONTEXT)" };
            yield return new object[] { User32.WM.IME_NOTIFY, " (WM_IME_NOTIFY)" };
            yield return new object[] { User32.WM.IME_CONTROL, " (WM_IME_CONTROL)" };
            yield return new object[] { User32.WM.IME_COMPOSITIONFULL, " (WM_IME_COMPOSITIONFULL)" };
            yield return new object[] { User32.WM.IME_SELECT, " (WM_IME_SELECT)" };
            yield return new object[] { User32.WM.IME_CHAR, " (WM_IME_CHAR)" };
            yield return new object[] { User32.WM.IME_KEYDOWN, " (WM_IME_KEYDOWN)" };
            yield return new object[] { User32.WM.IME_KEYUP, " (WM_IME_KEYUP)" };
            yield return new object[] { User32.WM.MDICREATE, " (WM_MDICREATE)" };
            yield return new object[] { User32.WM.MDIDESTROY, " (WM_MDIDESTROY)" };
            yield return new object[] { User32.WM.MDIACTIVATE, " (WM_MDIACTIVATE)" };
            yield return new object[] { User32.WM.MDIRESTORE, " (WM_MDIRESTORE)" };
            yield return new object[] { User32.WM.MDINEXT, " (WM_MDINEXT)" };
            yield return new object[] { User32.WM.MDIMAXIMIZE, " (WM_MDIMAXIMIZE)" };
            yield return new object[] { User32.WM.MDITILE, " (WM_MDITILE)" };
            yield return new object[] { User32.WM.MDICASCADE, " (WM_MDICASCADE)" };
            yield return new object[] { User32.WM.MDIICONARRANGE, " (WM_MDIICONARRANGE)" };
            yield return new object[] { User32.WM.MDIGETACTIVE, " (WM_MDIGETACTIVE)" };
            yield return new object[] { User32.WM.MDISETMENU, " (WM_MDISETMENU)" };
            yield return new object[] { User32.WM.ENTERSIZEMOVE, " (WM_ENTERSIZEMOVE)" };
            yield return new object[] { User32.WM.EXITSIZEMOVE, " (WM_EXITSIZEMOVE)" };
            yield return new object[] { User32.WM.DROPFILES, " (WM_DROPFILES)" };
            yield return new object[] { User32.WM.MDIREFRESHMENU, " (WM_MDIREFRESHMENU)" };
            yield return new object[] { User32.WM.MOUSEHOVER, " (WM_MOUSEHOVER)" };
            yield return new object[] { User32.WM.MOUSELEAVE, " (WM_MOUSELEAVE)" };
            yield return new object[] { User32.WM.CUT, " (WM_CUT)" };
            yield return new object[] { User32.WM.COPY, " (WM_COPY)" };
            yield return new object[] { User32.WM.PASTE, " (WM_PASTE)" };
            yield return new object[] { User32.WM.CLEAR, " (WM_CLEAR)" };
            yield return new object[] { User32.WM.UNDO, " (WM_UNDO)" };
            yield return new object[] { User32.WM.RENDERFORMAT, " (WM_RENDERFORMAT)" };
            yield return new object[] { User32.WM.RENDERALLFORMATS, " (WM_RENDERALLFORMATS)" };
            yield return new object[] { User32.WM.DESTROYCLIPBOARD, " (WM_DESTROYCLIPBOARD)" };
            yield return new object[] { User32.WM.DRAWCLIPBOARD, " (WM_DRAWCLIPBOARD)" };
            yield return new object[] { User32.WM.PAINTCLIPBOARD, " (WM_PAINTCLIPBOARD)" };
            yield return new object[] { User32.WM.VSCROLLCLIPBOARD, " (WM_VSCROLLCLIPBOARD)" };
            yield return new object[] { User32.WM.SIZECLIPBOARD, " (WM_SIZECLIPBOARD)" };
            yield return new object[] { User32.WM.ASKCBFORMATNAME, " (WM_ASKCBFORMATNAME)" };
            yield return new object[] { User32.WM.CHANGECBCHAIN, " (WM_CHANGECBCHAIN)" };
            yield return new object[] { User32.WM.HSCROLLCLIPBOARD, " (WM_HSCROLLCLIPBOARD)" };
            yield return new object[] { User32.WM.QUERYNEWPALETTE, " (WM_QUERYNEWPALETTE)" };
            yield return new object[] { User32.WM.PALETTEISCHANGING, " (WM_PALETTEISCHANGING)" };
            yield return new object[] { User32.WM.PALETTECHANGED, " (WM_PALETTECHANGED)" };
            yield return new object[] { User32.WM.HOTKEY, " (WM_HOTKEY)" };
            yield return new object[] { User32.WM.PRINT, " (WM_PRINT)" };
            yield return new object[] { User32.WM.PRINTCLIENT, " (WM_PRINTCLIENT)" };
            yield return new object[] { User32.WM.HANDHELDFIRST, " (WM_HANDHELDFIRST)" };
            yield return new object[] { User32.WM.HANDHELDLAST, " (WM_HANDHELDLAST)" };
            yield return new object[] { User32.WM.AFXFIRST, " (WM_AFXFIRST)" };
            yield return new object[] { User32.WM.AFXLAST, " (WM_AFXLAST)" };
            yield return new object[] { User32.WM.PENWINFIRST, " (WM_PENWINFIRST)" };
            yield return new object[] { User32.WM.PENWINLAST, " (WM_PENWINLAST)" };
            yield return new object[] { User32.WM.APP, " (WM_APP)" };
            yield return new object[] { User32.WM.USER, " (WM_USER)" };
            yield return new object[] { User32.WM.CTLCOLOR, " (WM_CTLCOLOR)" };

            // RichEdit messages
            yield return new object[] { User32.EM.GETLIMITTEXT, " (EM_GETLIMITTEXT)" };
            yield return new object[] { User32.EM.POSFROMCHAR, " (EM_POSFROMCHAR)" };
            yield return new object[] { User32.EM.CHARFROMPOS, " (EM_CHARFROMPOS)" };
            yield return new object[] { User32.EM.SCROLLCARET, " (EM_SCROLLCARET)" };
            yield return new object[] { Richedit.EM.CANPASTE, " (EM_CANPASTE)" };
            yield return new object[] { Richedit.EM.DISPLAYBAND, " (EM_DISPLAYBAND)" };
            yield return new object[] { Richedit.EM.EXGETSEL, " (EM_EXGETSEL)" };
            yield return new object[] { Richedit.EM.EXLIMITTEXT, " (EM_EXLIMITTEXT)" };
            yield return new object[] { Richedit.EM.EXLINEFROMCHAR, " (EM_EXLINEFROMCHAR)" };
            yield return new object[] { Richedit.EM.EXSETSEL, " (EM_EXSETSEL)" };
            yield return new object[] { Richedit.EM.FINDTEXT, " (EM_FINDTEXT)" };
            yield return new object[] { Richedit.EM.FORMATRANGE, " (EM_FORMATRANGE)" };
            yield return new object[] { Richedit.EM.GETCHARFORMAT, " (EM_GETCHARFORMAT)" };
            yield return new object[] { Richedit.EM.GETEVENTMASK, " (EM_GETEVENTMASK)" };
            yield return new object[] { Richedit.EM.GETOLEINTERFACE, " (EM_GETOLEINTERFACE)" };
            yield return new object[] { Richedit.EM.GETPARAFORMAT, " (EM_GETPARAFORMAT)" };
            yield return new object[] { Richedit.EM.GETSELTEXT, " (EM_GETSELTEXT)" };
            yield return new object[] { Richedit.EM.HIDESELECTION, " (EM_HIDESELECTION)" };
            yield return new object[] { Richedit.EM.PASTESPECIAL, " (EM_PASTESPECIAL)" };
            yield return new object[] { Richedit.EM.REQUESTRESIZE, " (EM_REQUESTRESIZE)" };
            yield return new object[] { Richedit.EM.SELECTIONTYPE, " (EM_SELECTIONTYPE)" };
            yield return new object[] { Richedit.EM.SETBKGNDCOLOR, " (EM_SETBKGNDCOLOR)" };
            yield return new object[] { Richedit.EM.SETCHARFORMAT, " (EM_SETCHARFORMAT)" };
            yield return new object[] { Richedit.EM.SETEVENTMASK, " (EM_SETEVENTMASK)" };
            yield return new object[] { Richedit.EM.SETOLECALLBACK, " (EM_SETOLECALLBACK)" };
            yield return new object[] { Richedit.EM.SETPARAFORMAT, " (EM_SETPARAFORMAT)" };
            yield return new object[] { Richedit.EM.SETTARGETDEVICE, " (EM_SETTARGETDEVICE)" };
            yield return new object[] { Richedit.EM.STREAMIN, " (EM_STREAMIN)" };
            yield return new object[] { Richedit.EM.STREAMOUT, " (EM_STREAMOUT)" };
            yield return new object[] { Richedit.EM.GETTEXTRANGE, " (EM_GETTEXTRANGE)" };
            yield return new object[] { Richedit.EM.FINDWORDBREAK, " (EM_FINDWORDBREAK)" };
            yield return new object[] { Richedit.EM.SETOPTIONS, " (EM_SETOPTIONS)" };
            yield return new object[] { Richedit.EM.GETOPTIONS, " (EM_GETOPTIONS)" };
            yield return new object[] { Richedit.EM.FINDTEXTEX, " (EM_FINDTEXTEX)" };
            yield return new object[] { Richedit.EM.GETWORDBREAKPROCEX, " (EM_GETWORDBREAKPROCEX)" };
            yield return new object[] { Richedit.EM.SETWORDBREAKPROCEX, " (EM_SETWORDBREAKPROCEX)" };

            // Richedit v2.0 messages
            yield return new object[] { Richedit.EM.SETUNDOLIMIT, " (EM_SETUNDOLIMIT)" };
            yield return new object[] { Richedit.EM.REDO, " (EM_REDO)" };
            yield return new object[] { Richedit.EM.CANREDO, " (EM_CANREDO)" };
            yield return new object[] { Richedit.EM.GETUNDONAME, " (EM_GETUNDONAME)" };
            yield return new object[] { Richedit.EM.GETREDONAME, " (EM_GETREDONAME)" };
            yield return new object[] { Richedit.EM.STOPGROUPTYPING, " (EM_STOPGROUPTYPING)" };
            yield return new object[] { Richedit.EM.SETTEXTMODE, " (EM_SETTEXTMODE)" };
            yield return new object[] { Richedit.EM.GETTEXTMODE, " (EM_GETTEXTMODE)" };
            yield return new object[] { Richedit.EM.AUTOURLDETECT, " (EM_AUTOURLDETECT)" };
            yield return new object[] { Richedit.EM.GETAUTOURLDETECT, " (EM_GETAUTOURLDETECT)" };
            yield return new object[] { Richedit.EM.SETPALETTE, " (EM_SETPALETTE)" };
            yield return new object[] { Richedit.EM.GETTEXTEX, " (EM_GETTEXTEX)" };
            yield return new object[] { Richedit.EM.GETTEXTLENGTHEX, " (EM_GETTEXTLENGTHEX)" };

            // Asia specific messages
            yield return new object[] { Richedit.EM.SETPUNCTUATION, " (EM_SETPUNCTUATION)" };
            yield return new object[] { Richedit.EM.GETPUNCTUATION, " (EM_GETPUNCTUATION)" };
            yield return new object[] { Richedit.EM.SETWORDWRAPMODE, " (EM_SETWORDWRAPMODE)" };
            yield return new object[] { Richedit.EM.GETWORDWRAPMODE, " (EM_GETWORDWRAPMODE)" };
            yield return new object[] { Richedit.EM.SETIMECOLOR, " (EM_SETIMECOLOR)" };
            yield return new object[] { Richedit.EM.GETIMECOLOR, " (EM_GETIMECOLOR)" };
            yield return new object[] { Richedit.EM.SETIMEOPTIONS, " (EM_SETIMEOPTIONS)" };
            yield return new object[] { Richedit.EM.GETIMEOPTIONS, " (EM_GETIMEOPTIONS)" };
            yield return new object[] { Richedit.EM.CONVPOSITION, " (EM_CONVPOSITION)" };
            yield return new object[] { Richedit.EM.SETLANGOPTIONS, " (EM_SETLANGOPTIONS)" };
            yield return new object[] { Richedit.EM.GETLANGOPTIONS, " (EM_GETLANGOPTIONS)" };
            yield return new object[] { Richedit.EM.GETIMECOMPMODE, " (EM_GETIMECOMPMODE)" };
            yield return new object[] { Richedit.EM.FINDTEXTW, " (EM_FINDTEXTW)" };
            yield return new object[] { Richedit.EM.FINDTEXTEXW, " (EM_FINDTEXTEXW)" };

            // Rich Edit 3.0 Asia msgs
            yield return new object[] { Richedit.EM.RECONVERSION, " (EM_RECONVERSION)" };
            yield return new object[] { Richedit.EM.SETIMEMODEBIAS, " (EM_SETIMEMODEBIAS)" };
            yield return new object[] { Richedit.EM.GETIMEMODEBIAS, " (EM_GETIMEMODEBIAS)" };

            // BiDi Specific messages
            yield return new object[] { Richedit.EM.SETBIDIOPTIONS, " (EM_SETBIDIOPTIONS)" };
            yield return new object[] { Richedit.EM.GETBIDIOPTIONS, " (EM_GETBIDIOPTIONS)" };
            yield return new object[] { Richedit.EM.SETTYPOGRAPHYOPTIONS, " (EM_SETTYPOGRAPHYOPTIONS)" };
            yield return new object[] { Richedit.EM.GETTYPOGRAPHYOPTIONS, " (EM_GETTYPOGRAPHYOPTIONS)" };

            // Extended Edit style specific messages
            yield return new object[] { Richedit.EM.SETEDITSTYLE, " (EM_SETEDITSTYLE)" };
            yield return new object[] { Richedit.EM.GETEDITSTYLE, " (EM_GETEDITSTYLE)" };
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
