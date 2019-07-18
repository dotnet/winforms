// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Xunit;
using Moq;

namespace System.Windows.Forms.Tests
{
    public class ThreadContextTests
    {
        private delegate bool MessageCallback(ref Message m);

        // WM_USER is 0x400, just need to be above that
        const int TestMessageId1 = 0x441;
        const int TestMessageId2 = 0x442;
        const int TestMessageId3 = 0x443;

        [Fact]
        public void ThreadContext_EmptyProcessFiltersWorks()
        {
            // Test that no filters at all does not throw, and that returns false from translation
            Application.ThreadContext threadContext = new Application.ThreadContext();
            NativeMethods.MSG msg = new NativeMethods.MSG();
            bool result = threadContext.PreTranslateMessage(ref msg);
            Assert.False(result);
        }

        [Fact]
        public void ThreadContext_WrongProcessFiltersPassesThrough()
        {
            // Test that a filter for the wrong ID returns false, but does get called
            Application.ThreadContext threadContext = new Application.ThreadContext();

            int filterId = TestMessageId2;
            var mockContext = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == filterId));

            threadContext.AddMessageFilter(mockContext.Object);
            NativeMethods.MSG msg = new NativeMethods.MSG
            {
                message = TestMessageId1
            };
            bool result = threadContext.PreTranslateMessage(ref msg);
            Assert.False(result);
            mockContext.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
        }

        [Fact]
        public void ThreadContext_CorrectProcessFiltersProcesses()
        {
            // Test that a filter with the correct ID returns true
            Application.ThreadContext threadContext = new Application.ThreadContext();

            int filterId = TestMessageId2;
            var mockContext = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == filterId));

            threadContext.AddMessageFilter(mockContext.Object);
            NativeMethods.MSG msg = new NativeMethods.MSG
            {
                message = filterId
            };
            bool result = threadContext.PreTranslateMessage(ref msg);
            Assert.True(result);
            mockContext.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
        }

        [Fact]
        public void ThreadContext_MultipleProcessFiltersProcesses()
        {
            // Test that multiple filters work
            Application.ThreadContext threadContext = new Application.ThreadContext();

            int filterId2 = TestMessageId2;
            var mockContext2 = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext2.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == filterId2));
            threadContext.AddMessageFilter(mockContext2.Object);

            int filterId3 = TestMessageId3;
            var mockContext3 = new Mock<IMessageFilter>(MockBehavior.Strict);
            mockContext3.Setup(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny))
                       .Returns((MessageCallback)((ref Message m) => m.Msg == filterId3));
            threadContext.AddMessageFilter(mockContext3.Object);

            NativeMethods.MSG msg = new NativeMethods.MSG
            {
                message = TestMessageId1
            };
            bool result = threadContext.PreTranslateMessage(ref msg);
            Assert.False(result);

            mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));
            mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));

            msg = new NativeMethods.MSG
            {
                message = TestMessageId2
            };
            result = threadContext.PreTranslateMessage(ref msg);
            Assert.True(result);

            mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(2));
            mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(1));

            msg = new NativeMethods.MSG
            {
                message = TestMessageId3
            };
            result = threadContext.PreTranslateMessage(ref msg);
            Assert.True(result);

            mockContext2.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(3));
            mockContext3.Verify(c => c.PreFilterMessage(ref It.Ref<Message>.IsAny), Times.Exactly(2));
        }
    }
}
