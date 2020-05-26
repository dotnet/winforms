// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public partial class WindowsFormsSynchronizationContextTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void WindowsFormsSynchronizationContext_CreateCopy_Invoke_Success()
        {
            var context = new WindowsFormsSynchronizationContext();
            WindowsFormsSynchronizationContext copy = Assert.IsType<WindowsFormsSynchronizationContext>(context.CreateCopy());
            Assert.NotSame(context, copy);

            // Send something.
            object state = new object();
            int callCount = 0;
            SendOrPostCallback callback = (actualState) =>
            {
                Assert.Same(state, actualState);
                callCount++;
            };
            copy.Send(callback, state);
            Assert.Equal(1, callCount);

            // Call again.
            copy.Send(callback, state);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact(Skip = "WindowsFormsSynchronizationContext disposed of too early. See: https://github.com/dotnet/winforms/issues/3297")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3297")]
        public void WindowsFormsSynchronizationContext_Dispose_MultipleTimes_Success()
        {
            var context = new WindowsFormsSynchronizationContext();
            int callCount = 0;
            SendOrPostCallback callback = (state) => callCount++;
            context.Dispose();
            context.Send(callback, new object());
            Assert.Equal(0, callCount);

            // Call again.
            context.Dispose();
            context.Send(callback, new object());
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> Send_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Send_TestData))]
        public void WindowsFormsSynchronizationContext_Send_InvokeSameThread_Success(object state)
        {
            int callCount = 0;
            SendOrPostCallback callback = (actualState) =>
            {
                Assert.Same(state, actualState);
                callCount++;
            };
            var context = new WindowsFormsSynchronizationContext();
            context.Send(callback, state);
            Assert.Equal(1, callCount);

            // Call again.
            context.Send(callback, state);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void WindowsFormsSynchronizationContext_Send_InvokeDeletedThread_ThrowsInvalidAsynchronousStateException()
        {
            int callCount = 0;
            SendOrPostCallback callback = (actualState) => callCount++;
            WindowsFormsSynchronizationContext context = null;
            Thread thread = new Thread(() =>
            {
                context = new WindowsFormsSynchronizationContext();
            });
            thread.Start();
            thread.Join();
            Assert.Throws<InvalidAsynchronousStateException>(() => context.Send(callback, new object()));
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory(Skip = "WindowsFormsSynchronizationContext disposed of too early. See: https://github.com/dotnet/winforms/issues/3297")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3297")]
        [MemberData(nameof(Send_TestData))]
        public void WindowsFormsSynchronizationContext_Send_InvokeDisposed_Nop(object state)
        {
            int callCount = 0;
            SendOrPostCallback callback = (actualState) => callCount++;
            var context = new WindowsFormsSynchronizationContext();
            context.Dispose();

            context.Send(callback, state);
            Assert.Equal(0, callCount);

            // Call again.
            context.Send(callback, state);
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> Post_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Post_TestData))]
        public void WindowsFormsSynchronizationContext_Post_InvokeSameThread_Success(object state)
        {
            int callCount = 0;
            SendOrPostCallback callback = (actualState) =>
            {
                Assert.Same(state, actualState);
                callCount++;
            };
            var context = new WindowsFormsSynchronizationContext();
            context.Post(callback, state);
            Assert.Equal(0, callCount);

            // Call again.
            context.Post(callback, state);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory(Skip = "WindowsFormsSynchronizationContext disposed of too early. See: https://github.com/dotnet/winforms/issues/3297")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3297")]
        [MemberData(nameof(Send_TestData))]
        public void WindowsFormsSynchronizationContext_Post_InvokeDisposed_Nop(object state)
        {
            int callCount = 0;
            SendOrPostCallback callback = (actualState) => callCount++;
            var context = new WindowsFormsSynchronizationContext();
            context.Dispose();

            context.Post(callback, state);
            Assert.Equal(0, callCount);

            // Call again.
            context.Post(callback, state);
            Assert.Equal(0, callCount);
        }
    }
}
