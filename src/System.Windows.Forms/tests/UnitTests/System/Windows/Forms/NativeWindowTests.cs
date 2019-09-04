// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class NativeWindowTests
    {
        [Fact]
        public void AssignHandle_TwiceThrowsArgumentException()
        {
            using Form form = new Form();
            NativeWindow nativeWindow = new NativeWindow();
            nativeWindow.AssignHandle(form.Handle);
            Assert.Throws<InvalidOperationException>(() => nativeWindow.AssignHandle(form.Handle));
        }

        [Fact]
        public void AssignHandle_AfterRelease()
        {
            using Form form = new Form();
            NativeWindow nativeWindow = new NativeWindow();
            nativeWindow.AssignHandle(form.Handle);
            nativeWindow.ReleaseHandle();
            nativeWindow.AssignHandle(form.Handle);
        }

        [Fact]
        public void AssignHandle_TwoNativeWindows()
        {
            using Form form = new Form();
            NativeWindow nativeWindow1 = new NativeWindow();

            // ControlNativeWindow (via Form) will be registered with the Handle (by calling .Handle) already.
            // Invoking AssignHandle on it will set ControlNativeWindow to Previous and assign nativeWindow1
            // in the dictionary of window handles (s_windowHandles).
            nativeWindow1.AssignHandle(form.Handle);

            // This will further chain nativeWindow2, putting nativeWindow1, then ControlNativeWindow as previous
            // (Previous) entries in the chain.
            NativeWindow nativeWindow2 = new NativeWindow();
            nativeWindow2.AssignHandle(form.Handle);
        }

        [Fact]
        public void DefWindProc_NoHandle()
        {
            MyNativeWindow nativeWindow = new MyNativeWindow();
            Message message = default;

            // This isn't supported per the docs, but we always have called DefWindowProc and shouldn't fail.
            // We assert as it isn't behavior we want to allow internally.
            using (new NoAssertContext())
            {
                nativeWindow.DefWndProc(ref message);
            }

            Assert.Empty(nativeWindow.Messages);
        }

        [Fact]
        public void DefWindProc_AfterAssignHandle()
        {
            MyNativeWindow nativeWindow = new MyNativeWindow
            {
                MessageTypePredicate = (int msg) => msg == 123456
            };

            nativeWindow.CreateHandle(new CreateParams());

            Message message = Message.Create(IntPtr.Zero, 123456, IntPtr.Zero, IntPtr.Zero);

            // As we don't have a "Previous" the default window procedure gets called
            nativeWindow.DefWndProc(ref message);
            Assert.Empty(nativeWindow.Messages);

            MyNativeWindow nativeWindow2 = new MyNativeWindow
            {
                MessageTypePredicate = (int msg) => msg == 123456
            };

            // This will set the existing nativeWindow as Previous. When Previous NativeWindows
            // are registered for the same handle, a chain of calls will happen until the original
            // registered NativeWindow for a given handle is reached (i.e. no Previous). The
            // call chain is like this:
            //
            //   DefWndProc() -> PreviousWindow.CallBack() -> WndProc() -> DefWndProc()

            nativeWindow2.AssignHandle(nativeWindow.Handle);
            nativeWindow2.DefWndProc(ref message);
            Assert.Single(nativeWindow.Messages);
            Assert.Empty(nativeWindow2.Messages);

            // Check that the message continues to work back.

            MyNativeWindow nativeWindow3 = new MyNativeWindow
            {
                MessageTypePredicate = (int msg) => msg == 123456
            };

            nativeWindow3.AssignHandle(nativeWindow.Handle);
            nativeWindow3.DefWndProc(ref message);
            Assert.Equal(2, nativeWindow.Messages.Count);
            Assert.Single(nativeWindow2.Messages);
            Assert.Empty(nativeWindow3.Messages);
        }

        public class MyNativeWindow : NativeWindow
        {
            public Predicate<int> MessageTypePredicate { get; set; }

            public List<Message> Messages { get; } = new List<Message>();

            protected override void WndProc(ref Message m)
            {
                if (MessageTypePredicate == null || MessageTypePredicate(m.Msg))
                    Messages.Add(m);

                base.WndProc(ref m);
            }
        }
    }
}
