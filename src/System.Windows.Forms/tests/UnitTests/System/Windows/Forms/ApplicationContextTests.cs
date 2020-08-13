// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Moq;
using Moq.Protected;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ApplicationContextTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Ctor_Default()
        {
            using var context = new ApplicationContext();

            Assert.Null(context.MainForm);
            Assert.Null(context.Tag);
        }

        public static IEnumerable<object[]> Ctor_Form_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Form() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Form_TestData))]
        public void Ctor_Form(Form mainForm)
        {
            using var context = new ApplicationContext(mainForm);

            Assert.Same(mainForm, context.MainForm);
            Assert.Null(context.Tag);
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Form_TestData))]
        public void MainForm_Set_GetReturnsExpected(Form value)
        {
            using var context = new ApplicationContext
            {
                MainForm = value
            };
            Assert.Equal(value, context.MainForm);

            // Set same
            context.MainForm = value;
            Assert.Equal(value, context.MainForm);
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Form_TestData))]
        public void MainForm_SetWithNonNullOldValue_GetReturnsExpected(Form value)
        {
            using var context = new ApplicationContext
            {
                MainForm = new Form()
            };

            context.MainForm = value;
            Assert.Equal(value, context.MainForm);

            // Set same
            context.MainForm = value;
            Assert.Equal(value, context.MainForm);
        }

        [WinFormsFact]
        public void MainForm_DestroyHandleWithThreadExit_CallsHandler()
        {
            using var mainForm = new SubForm();
            using var context = new ApplicationContext(mainForm);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(context, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            context.ThreadExit += handler;

            Assert.NotEqual(IntPtr.Zero, mainForm.Handle);
            Assert.Equal(0, callCount);

            mainForm.OnHandleDestroyed(EventArgs.Empty);
            Assert.Equal(1, callCount);

            // Call again.
            mainForm.OnHandleDestroyed(EventArgs.Empty);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void MainForm_DestroyOldHandleWithThreadExit_CallsHandler()
        {
            using var mainForm = new SubForm();
            using var newMainForm = new SubForm();
            using var context = new ApplicationContext(mainForm)
            {
                MainForm = newMainForm
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(context, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            context.ThreadExit += handler;

            Assert.NotEqual(IntPtr.Zero, mainForm.Handle);
            Assert.Equal(0, callCount);

            Assert.NotEqual(IntPtr.Zero, newMainForm.Handle);
            Assert.Equal(0, callCount);

            mainForm.OnHandleDestroyed(EventArgs.Empty);
            Assert.Equal(0, callCount);

            newMainForm.OnHandleDestroyed(EventArgs.Empty);
            Assert.Equal(1, callCount);

            // Call again.
            mainForm.OnHandleDestroyed(EventArgs.Empty);
            Assert.Equal(1, callCount);

            newMainForm.OnHandleDestroyed(EventArgs.Empty);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public void MainForm_RecreateHandleWithThreadExit_DoesNotCallHandler()
        {
            using var mainForm = new SubForm();
            using var context = new ApplicationContext(mainForm);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(context, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            context.ThreadExit += handler;

            Assert.NotEqual(IntPtr.Zero, mainForm.Handle);
            Assert.Equal(0, callCount);

            mainForm.RecreateHandle();
            Assert.Equal(0, callCount);

            // Call again.
            mainForm.RecreateHandle();
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Tag_Set_GetReturnsExpected(string value)
        {
            using var context = new ApplicationContext
            {
                Tag = value
            };
            Assert.Equal(value, context.Tag);

            // Set same
            context.Tag = value;
            Assert.Equal(value, context.Tag);
        }

        [WinFormsFact]
        public void Dispose_InvokeWithForm_Success()
        {
            using var mainForm = new Form();
            using var context = new ApplicationContext(mainForm);
            context.Dispose();
            Assert.Null(context.MainForm);
            Assert.True(mainForm.IsDisposed);

            context.Dispose();
            Assert.Null(context.MainForm);
            Assert.True(mainForm.IsDisposed);
        }

        [WinFormsFact]
        public void Dispose_InvokeWithDisposedForm_Success()
        {
            using var mainForm = new Form();
            using var context = new ApplicationContext(mainForm);
            mainForm.Dispose();
            Assert.True(mainForm.IsDisposed);

            context.Dispose();
            Assert.Null(context.MainForm);
            Assert.True(mainForm.IsDisposed);

            context.Dispose();
            Assert.Null(context.MainForm);
            Assert.True(mainForm.IsDisposed);
        }

        [WinFormsFact]
        public void Dispose_InvokeWithoutForm_Success()
        {
            using var context = new ApplicationContext();
            context.Dispose();
            Assert.Null(context.MainForm);

            context.Dispose();
            Assert.Null(context.MainForm);
        }

        [WinFormsFact]
        public void Dispose_Invoke_CallsDisposeDisposing()
        {
            var mockContext = new Mock<ApplicationContext>(MockBehavior.Strict);
            mockContext
                .Protected()
                .Setup("Dispose", true)
                .Verifiable();
            mockContext.Object.Dispose();
            mockContext.Protected().Verify("Dispose", Times.Once(), true);

            // Call again.
            mockContext.Object.Dispose();
            mockContext.Protected().Verify("Dispose", Times.Exactly(2), true);
        }

        [WinFormsFact]
        public void Dispose_InvokeDisposingWithForm_Success()
        {
            using var mainForm = new Form();
            using var context = new SubApplicationContext(mainForm);
            context.Dispose(true);
            Assert.Null(context.MainForm);
            Assert.True(mainForm.IsDisposed);

            context.Dispose(true);
            Assert.Null(context.MainForm);
            Assert.True(mainForm.IsDisposed);
        }

        [WinFormsFact]
        public void Dispose_InvokeNotDisposingWithForm_Nop()
        {
            using var mainForm = new Form();
            using var context = new SubApplicationContext(mainForm);
            context.Dispose(false);
            Assert.Same(mainForm, context.MainForm);
            Assert.False(mainForm.IsDisposed);

            context.Dispose(false);
            Assert.Same(mainForm, context.MainForm);
            Assert.False(mainForm.IsDisposed);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void Dispose_InvokeDisposingNoForm_Nop(bool disposing)
        {
            using var context = new SubApplicationContext();
            context.Dispose(disposing);
            Assert.Null(context.MainForm);

            context.Dispose(disposing);
            Assert.Null(context.MainForm);
        }

        [WinFormsFact]
        public void ExitThread_InvokeWithThreadExit_CallsHandler()
        {
            using var context = new ApplicationContext();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(context, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            context.ThreadExit += handler;

            // Call with handler.
            context.ExitThread();
            Assert.Equal(1, callCount);

            // Call again.
            context.ExitThread();
            Assert.Equal(2, callCount);

            // Remove handler.
            context.ThreadExit -= handler;
            context.ExitThread();
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void ExitThread_Invoke_CallsExitThreadCore()
        {
            var mockContext = new Mock<ApplicationContext>(MockBehavior.Strict);
            mockContext
                .Protected()
                .Setup("ExitThreadCore")
                .Verifiable();
            mockContext
                .Protected()
                .Setup("Dispose", false);
            mockContext.Object.ExitThread();
            mockContext.Protected().Verify("ExitThreadCore", Times.Once());

            // Call again.
            mockContext.Object.ExitThread();
            mockContext.Protected().Verify("ExitThreadCore", Times.Exactly(2));
        }

        [WinFormsFact]
        public void ExitThreadCore_InvokeWithThreadExit_CallsHandler()
        {
            using var context = new SubApplicationContext();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(context, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            context.ThreadExit += handler;

            // Call with handler.
            context.ExitThreadCore();
            Assert.Equal(1, callCount);

            // Call again.
            context.ExitThreadCore();
            Assert.Equal(2, callCount);

            // Remove handler.
            context.ThreadExit -= handler;
            context.ExitThreadCore();
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> OnMainFormClosed_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new object(), new EventArgs() };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnMainFormClosed_TestData))]
        public void OnMainFormClosed_InvokeWithThreadExit_CallsHandler(object sender, EventArgs e)
        {
            using var context = new SubApplicationContext();
            int callCount = 0;
            EventHandler handler = (actualSender, actualE) =>
            {
                Assert.Same(context, actualSender);
                Assert.Same(EventArgs.Empty, actualE);
                callCount++;
            };
            context.ThreadExit += handler;

            // Call with handler.
            context.OnMainFormClosed(sender, e);
            Assert.Equal(1, callCount);

            // Call again.
            context.OnMainFormClosed(sender, e);
            Assert.Equal(2, callCount);

            // Remove handler.
            context.ThreadExit -= handler;
            context.OnMainFormClosed(sender, e);
            Assert.Equal(2, callCount);
        }

        private class SubApplicationContext : ApplicationContext
        {
            public SubApplicationContext() : base()
            {
            }

            public SubApplicationContext(Form mainForm) : base(mainForm)
            {
            }

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new void ExitThreadCore() => base.ExitThreadCore();

            public new void OnMainFormClosed(object sender, EventArgs e) => base.OnMainFormClosed(sender, e);
        }

        private class SubForm : Form
        {
            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void RecreateHandle() => base.RecreateHandle();
        }
    }
}
