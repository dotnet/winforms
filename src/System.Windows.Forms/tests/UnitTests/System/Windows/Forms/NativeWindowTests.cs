﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using Moq.Protected;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class NativeWindowTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void NativeWindow_Ctor_Default()
        {
            var window = new NativeWindow();
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_AssignHandle_ControlHandle_Success()
        {
            using var control = new Control();
            var window = new NativeWindow();
            window.AssignHandle(control.Handle);
            Assert.Equal(control.Handle, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_AssignHandle_TwoNativeWindows_Success()
        {
            using Form control = new Form();
            var window1 = new NativeWindow();

            // ControlNativeWindow (via Form) will be registered with the Handle (by calling .Handle) already.
            // Invoking AssignHandle on it will set ControlNativeWindow to Previous and assign window1
            // in the dictionary of window handles (s_windowHandles).
            window1.AssignHandle(control.Handle);
            Assert.Equal(control.Handle, window1.Handle);

            // This will further chain window2, putting window1, then ControlNativeWindow as previous
            // (Previous) entries in the chain.
            NativeWindow window2 = new NativeWindow();
            window2.AssignHandle(control.Handle);
            Assert.Equal(control.Handle, window1.Handle);
            Assert.Equal(control.Handle, window2.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_AssignHandle_ControlHandleAfterRelease_Success()
        {
            using var control = new Control();
            var window = new NativeWindow();
            window.AssignHandle(control.Handle);
            Assert.Equal(control.Handle, window.Handle);

            window.ReleaseHandle();
            Assert.Equal(IntPtr.Zero, window.Handle);

            window.AssignHandle(control.Handle);
            Assert.Equal(control.Handle, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_AssignHandle_ZeroHandle_Success()
        {
            using (new NoAssertContext())
            {
                var window = new NativeWindow();
                window.AssignHandle(IntPtr.Zero);
                Assert.Equal(IntPtr.Zero, window.Handle);
            }
        }

        [WinFormsFact]
        public void NativeWindow_AssignHandle_InvalidHandle_Success()
        {
            using (new NoAssertContext())
            {
                var window = new NativeWindow();
                window.AssignHandle((IntPtr)250);
                Assert.Equal((IntPtr)250, window.Handle);
            }
        }

        [WinFormsFact]
        public void NativeWindow_AssignHandle_AlreadyHasHandle_Success()
        {
            using var control = new Control();
            var window = new NativeWindow();
            window.AssignHandle(control.Handle);
            Assert.Equal(control.Handle, window.Handle);
            Assert.Throws<InvalidOperationException>(() => window.AssignHandle((IntPtr)250));
            Assert.Equal(control.Handle, window.Handle);
        }

        [WinFormsTheory]
        [InlineData(null, 0)]
        [InlineData("SysTabControl32", 0)]
        [InlineData("SysTabControl32", 100)]
        public void NativeWindow_CreateHandle_Invoke_Success(string className, int classStyle)
        {
            var window1 = new NativeWindow();
            var cp = new CreateParams
            {
                ClassName = className,
                ClassStyle = classStyle
            };
            window1.CreateHandle(cp);
            try
            {
                Assert.NotEqual(IntPtr.Zero, window1.Handle);
                Assert.Null(cp.Caption);

                // Call again on another class to test caching.
                var window2 = new NativeWindow();
                window2.CreateHandle(cp);
                try
                {
                    Assert.NotEqual(IntPtr.Zero, window2.Handle);
                    Assert.NotEqual(window1.Handle, window2.Handle);
                    Assert.Null(cp.Caption);
                }
                finally
                {
                    window2.DestroyHandle();
                }
            }
            finally
            {
                window1.DestroyHandle();
            }
        }

        public static IEnumerable<object[]> CreateHandle_Caption_TestData()
        {
            yield return new object[] { string.Empty, string.Empty };
            yield return new object[] { "abc", "abc" };
            yield return new object[] { new string('a', short.MaxValue + 1), new string('a', short.MaxValue) };
        }

        [WinFormsTheory]
        [MemberData(nameof(CreateHandle_Caption_TestData))]
        public void NativeWindow_CreateHandle_InvokeWithCaption_Success(string caption, string expectedCaption)
        {
            using var control = new Control();
            var window = new NativeWindow();
            var cp = new CreateParams
            {
                Caption = caption
            };
            window.CreateHandle(cp);
            try
            {
                Assert.NotEqual(IntPtr.Zero, window.Handle);
                Assert.Equal(expectedCaption, cp.Caption);
            }
            finally
            {
                window.DestroyHandle();
            }
        }

        [WinFormsFact]
        public void NativeWindow_CreateHandle_NullCp_ThrowsNullReferenceException()
        {
            var window = new NativeWindow();
            Assert.Throws<NullReferenceException>(() => window.CreateHandle(null));
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_CreateHandle_AlreadyHasHandle_ThrowsInvalidOperationException()
        {
            using var control = new Control();
            var window = new NativeWindow();
            window.AssignHandle(control.Handle);
            IntPtr handle = window.Handle;

            var cp = new CreateParams();
            Assert.Throws<InvalidOperationException>(() => window.CreateHandle(cp));
            Assert.Equal(handle, window.Handle);
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData("noSuchClassName")]
        public void NativeWindow_CreateHandle_InvalidWindowClassName_ThrowsWin32Exception(string className)
        {
            var window = new NativeWindow();
            var cp = new CreateParams
            {
                ClassName = className
            };
            Assert.Throws<Win32Exception>(() => window.CreateHandle(cp));
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_CreateHandle_InvokeAlreadyExists_ThrowsWin32Exception()
        {
            var window1 = new NativeWindow();
            var cp1 = new CreateParams
            {
                ClassName = "SysTabControl32"
            };
            window1.CreateHandle(cp1);
            try
            {
                Assert.NotEqual(IntPtr.Zero, window1.Handle);

                var window2 = new NativeWindow();
                var cp2 = new CreateParams
                {
                    ClassName = "systabcontrol32"
                };
                Assert.Throws<Win32Exception>(() => window2.CreateHandle(cp2));
                Assert.Equal(IntPtr.Zero, window2.Handle);
            }
            finally
            {
                window1.DestroyHandle();
            }
        }

        [WinFormsFact]
        public void NativeWindow_DestroyHandle_InvokeWithCreatedHandle_Success()
        {
            var window = new NativeWindow();
            var cp = new CreateParams();
            window.CreateHandle(cp);
            window.DestroyHandle();
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_DestroyHandle_InvokeWithValidAssignedHandle_Success()
        {
            using var control = new Control();
            var window = new NativeWindow();
            window.AssignHandle(control.Handle);
            window.DestroyHandle();
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_DestroyHandle_InvokeWithInvalidAssignedHandle_Nop()
        {
            using (new NoAssertContext())
            {
                var window = new NativeWindow();
                window.AssignHandle((IntPtr)250);
                window.DestroyHandle();
                Assert.Equal(IntPtr.Zero, window.Handle);
            }
        }

        [WinFormsFact]
        public void NativeWindow_DestroyHandle_InvokeWithoutHandle_Nop()
        {
            var window = new NativeWindow();
            window.DestroyHandle();
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        [InlineData((int)User32.WM.NCDESTROY)]
        public void NativeWindow_DefWndProc_InvokeWithCreatedHandle_Nop(int msg)
        {
            var window = new WndProcTrackingNativeWindow();
            window.CreateHandle(new CreateParams());
            try
            {
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.DefWndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.NotEqual(IntPtr.Zero, window.Handle);
            }
            finally
            {
                window.DestroyHandle();
            }
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        public void NativeWindow_DefWndProc_InvokeWithValidAssignedHandle_Nop(int msg)
        {
            using var control = new Control();
            var window = new WndProcTrackingNativeWindow();
            window.AssignHandle(control.Handle);

            try
            {
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.DefWndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(control.Handle, window.Handle);
            }
            finally
            {
                window.ReleaseHandle();
            }
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        [InlineData((int)User32.WM.NCDESTROY)]
        public void NativeWindow_DefWndProc_InvokeWithInvalidHandle_Nop(int msg)
        {
            using (new NoAssertContext())
            {
                var window = new WndProcTrackingNativeWindow();
                window.AssignHandle((IntPtr)250);
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.DefWndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal((IntPtr)250, window.Handle);
            }
        }

        [WinFormsFact]
        public void NativeWindow_DefWindProc_InvokeAfterAssignHandle_Success()
        {
            using var control = new Control();
            var window1 = new WndProcTrackingNativeWindow
            {
                MessageTypePredicate = (msg) => msg == 123456
            };
            window1.AssignHandle(control.Handle);

            Message m = new Message
            {
                Msg = 123456
            };

            // As we don't have a "Previous" the default window procedure gets called
            window1.DefWndProc(ref m);
            Assert.Empty(window1.Messages);

            // This will set the existing window as Previous. When Previous NativeWindows
            // are registered for the same handle, a chain of calls will happen until the original
            // registered NativeWindow for a given handle is reached (i.e. no Previous). The
            // call chain is like this:
            //
            //   DefWndProc() -> PreviousWindow.CallBack() -> WndProc() -> DefWndProc()
            var window2 = new WndProcTrackingNativeWindow
            {
                MessageTypePredicate = (msg) => msg == 123456
            };
            window2.AssignHandle(window1.Handle);
            window2.DefWndProc(ref m);
            Assert.Single(window1.Messages);
            Assert.Empty(window2.Messages);

            // Check that the message continues to work back.
            var window3 = new WndProcTrackingNativeWindow
            {
                MessageTypePredicate = (msg) => msg == 123456
            };
            window3.AssignHandle(window1.Handle);
            window3.DefWndProc(ref m);
            Assert.Equal(2, window1.Messages.Count);
            Assert.Single(window2.Messages);
            Assert.Empty(window3.Messages);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        [InlineData((int)User32.WM.NCDESTROY)]
        public void NativeWindow_DefWndProc_InvokeWithoutHandle_Nop(int msg)
        {
            using (new NoAssertContext())
            {
                var window = new WndProcTrackingNativeWindow();
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.DefWndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Empty(window.Messages);
            }
        }

        [WinFormsFact]
        public void NativeWindow_ReleaseHandle_InvokeWithCreatedHandle_Success()
        {
            using var control = new Control();
            var window = new NativeWindow();
            window.AssignHandle(control.Handle);
            window.ReleaseHandle();
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_ReleaseHandle_InvokeWithValidAssignedHandle_Success()
        {
            using var control = new Control();
            var window = new NativeWindow();
            window.AssignHandle(control.Handle);
            window.ReleaseHandle();
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsFact]
        public void NativeWindow_ReleaseHandle_InvokeWithInvalidAssignedHandle_Success()
        {
            using (new NoAssertContext())
            {
                using var control = new Control();
                var window = new NativeWindow();
                window.AssignHandle((IntPtr)250);
                window.ReleaseHandle();
                Assert.Equal(IntPtr.Zero, window.Handle);
            }
        }

        [WinFormsFact]
        public void NativeWindow_ReleaseHandle_InvokeWithoutHandle_Nop()
        {
            var window = new NativeWindow();
            window.ReleaseHandle();
            Assert.Equal(IntPtr.Zero, window.Handle);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        [InlineData((int)User32.WM.NCDESTROY)]
        public void NativeWindow_WndProc_InvokeWithCreatedHandle_Success(int msg)
        {
            var window = new SubNativeWindow();
            window.CreateHandle(new CreateParams());

            try
            {
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
            }
            finally
            {
                window.DestroyHandle();
            }
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        public void NativeWindow_WndProc_InvokeWithValidAssignedHandle_Success(int msg)
        {
            using var control = new Control();
            var window = new SubNativeWindow();
            window.AssignHandle(control.Handle);

            try
            {
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal(control.Handle, window.Handle);
            }
            finally
            {
                window.ReleaseHandle();
            }
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        [InlineData((int)User32.WM.NCDESTROY)]
        public void NativeWindow_WndProc_InvokeWithInvalidAssignedHandle_Nop(int msg)
        {
            using (new NoAssertContext())
            {
                var window = new SubNativeWindow();
                window.AssignHandle((IntPtr)250);
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
                Assert.Equal((IntPtr)250, window.Handle);
            }
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1234)]
        [InlineData((int)User32.WM.NCDESTROY)]
        public void NativeWindow_WndProc_InvokeWithoutHandle_Nop(int msg)
        {
            using (new NoAssertContext())
            {
                var window = new SubNativeWindow();
                var m = new Message
                {
                    Msg = msg,
                    Result = (IntPtr)1
                };
                window.WndProc(ref m);
                Assert.Equal(IntPtr.Zero, m.Result);
            }
        }

        private class SubNativeWindow : NativeWindow
        {
            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }

        private class WndProcTrackingNativeWindow : NativeWindow
        {
            public Predicate<int> MessageTypePredicate { get; set; }

            public List<Message> Messages { get; } = new List<Message>();

            protected override void WndProc(ref Message m)
            {
                if (MessageTypePredicate is null || MessageTypePredicate(m.Msg))
                {
                    Messages.Add(m);
                }

                base.WndProc(ref m);
            }
        }
    }
}
