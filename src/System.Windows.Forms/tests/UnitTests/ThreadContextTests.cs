// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Xunit;
using Moq;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ThreadContextTests : IClassFixture<ThreadExceptionFixture>
    {
        private delegate bool MessageCallback(ref Message m);

        // WM_USER is 0x400, just need to be above that
        const User32.WM TestMessageId1 = (User32.WM)0x441;
        const User32.WM TestMessageId2 = (User32.WM)0x442;
        const User32.WM TestMessageId3 = (User32.WM)0x443;

        [StaFact]
        public void ThreadContext_EmptyProcessFiltersWorks()
        {
            // Test that no filters at all does not throw, and that returns false from translation
            Application.ThreadContext threadContext = new Application.ThreadContext();
            var msg = new User32.MSG();
            Assert.False(threadContext.PreTranslateMessage(ref msg));
        }

        [StaFact]
        public void ThreadContext_WrongProcessFiltersPassesThrough()
        {
            // Test that a filter for the wrong ID returns false, but does get called
            Application.ThreadContext threadContext = new Application.ThreadContext();

            User32.WM filterId = TestMessageId2;
            var mockContext = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId));

            threadContext.AddMessageFilter(mockContext.Object);
            var msg = new User32.MSG
            {
                message = TestMessageId1
            };
            Assert.False(threadContext.PreTranslateMessage(ref msg));
            mockContext.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
        }

        [StaFact]
        public void ThreadContext_CorrectProcessFiltersProcesses()
        {
            // Test that a filter with the correct ID returns true
            Application.ThreadContext threadContext = new Application.ThreadContext();

            User32.WM filterId = TestMessageId2;
            var mockContext = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId));

            threadContext.AddMessageFilter(mockContext.Object);
            var msg = new User32.MSG
            {
                message = filterId
            };
            Assert.True(threadContext.PreTranslateMessage(ref msg));
            mockContext.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
        }

        [StaFact]
        public void ThreadContext_MultipleProcessFiltersProcesses()
        {
            // Test that multiple filters work
            Application.ThreadContext threadContext = new Application.ThreadContext();

            User32.WM filterId2 = TestMessageId2;
            var mockContext2 = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext2.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId2));
            threadContext.AddMessageFilter(mockContext2.Object);

            User32.WM filterId3 = TestMessageId3;
            var mockContext3 = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext3.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == (int)filterId3));
            threadContext.AddMessageFilter(mockContext3.Object);

            var msg = new User32.MSG
            {
                message = TestMessageId1
            };
            Assert.False(threadContext.PreTranslateMessage(ref msg));

            mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
            mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));

            msg = new User32.MSG
            {
                message = TestMessageId2
            };
            Assert.True(threadContext.PreTranslateMessage(ref msg));

            mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(2));
            mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));

            msg = new User32.MSG
            {
                message = TestMessageId3
            };
            Assert.True(threadContext.PreTranslateMessage(ref msg));

            mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(3));
            mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(2));
        }
    }
}
