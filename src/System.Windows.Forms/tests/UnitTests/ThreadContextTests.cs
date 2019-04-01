// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ThreadContextTests
    {
      // WM_USER is 0x400, just need to be above that
      const int TestMessageId1 = 0x441;
      const int TestMessageId2 = 0x442;
      const int TestMessageId3 = 0x443;

      internal class MessageFilter : IMessageFilter {
        private int filterId;

        public MessageFilter(int filterId) {
          this.filterId = filterId;
        }

        public bool filteredMessage = false;

        public bool PreFilterMessage(ref Message msg) {
          if (msg.Msg == filterId) {
            filteredMessage = true;
            return true;
          }
          filteredMessage = false;
          return false;
        }
      }


      [Fact]
      public void EmptyProcessFiltersWorks() {
        // Test that no filters at all does not throw, and that returns false from translation
        Application.ThreadContext threadContext = new Application.ThreadContext();
        NativeMethods.MSG msg = new NativeMethods.MSG();
        bool result = threadContext.PreTranslateMessage(ref msg);
        Assert.False(result);
      }

      [Fact]
      public void WrongProcessFiltersPassesThrough() {
        // Test that a filter for the wrong ID returns false, but does get called
        Application.ThreadContext threadContext = new Application.ThreadContext();
        MessageFilter filter = new MessageFilter(TestMessageId2);
        threadContext.AddMessageFilter(filter);
        filter.filteredMessage = true;
        NativeMethods.MSG msg = new NativeMethods.MSG();
        msg.message = TestMessageId1;
        bool result = threadContext.PreTranslateMessage(ref msg);
        Assert.False(result);
        Assert.False(filter.filteredMessage);
      }

      [Fact]
      public void CorrectProcessFiltersProcesses() {
        // Test that a filter with the correct ID returns true
        Application.ThreadContext threadContext = new Application.ThreadContext();
        MessageFilter filter = new MessageFilter(TestMessageId2);
        threadContext.AddMessageFilter(filter);
        filter.filteredMessage = false;
        NativeMethods.MSG msg = new NativeMethods.MSG();
        msg.message = TestMessageId2;
        bool result = threadContext.PreTranslateMessage(ref msg);
        Assert.True(result);
        Assert.True(filter.filteredMessage);
      }

      [Fact]
      public void MultipleProcessFiltersProcesses() {
        // Test that multiple filters work
        Application.ThreadContext threadContext = new Application.ThreadContext();
        MessageFilter filter = new MessageFilter(TestMessageId2);
        threadContext.AddMessageFilter(filter);
        MessageFilter filter2 = new MessageFilter(TestMessageId3);
        threadContext.AddMessageFilter(filter2);
        NativeMethods.MSG msg = new NativeMethods.MSG();
        msg.message = TestMessageId1;
        bool result = threadContext.PreTranslateMessage(ref msg);
        Assert.False(result);
        Assert.False(filter.filteredMessage);
        Assert.False(filter2.filteredMessage);

        msg = new NativeMethods.MSG();
        msg.message = TestMessageId2;
        result = threadContext.PreTranslateMessage(ref msg);
        Assert.True(result);
        Assert.True(filter.filteredMessage);
        Assert.False(filter2.filteredMessage);

        filter.filteredMessage = false;

        msg = new NativeMethods.MSG();
        msg.message = TestMessageId3;
        result = threadContext.PreTranslateMessage(ref msg);
        Assert.True(result);
        Assert.False(filter.filteredMessage);
        Assert.True(filter2.filteredMessage);
      }
    }
}
