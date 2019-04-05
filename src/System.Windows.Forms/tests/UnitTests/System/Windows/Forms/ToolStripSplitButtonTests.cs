// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSplitButtonTests
    {
        public static IEnumerable<object[]> ToolStripItem_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new SubToolStripItem() };
        }

        [Theory]
        [MemberData(nameof(ToolStripItem_Set_TestData))]
        public void ToolStripSplitButton_DefaultItem_Set_GetReturnsExpected(ToolStripItem value)
        {
            var button = new ToolStripSplitButton
            {
                DefaultItem = value
            };
            Assert.Same(value, button.DefaultItem);

            // Set same.
            button.DefaultItem = value;
            Assert.Same(value, button.DefaultItem);
        }

        [Fact]
        public void ToolStripSplitButton_DefaultItem_SetWithHandler_CallsOnDefaultItemChanged()
        {
            var button = new ToolStripSplitButton();

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(button, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            button.DefaultItemChanged += handler;

            // Set non-null.
            var item1 = new SubToolStripItem();
            button.DefaultItem = item1;
            Assert.Same(item1, button.DefaultItem);
            Assert.Equal(1, callCount);

            // Set same.
            button.DefaultItem = item1;
            Assert.Same(item1, button.DefaultItem);
            Assert.Equal(1, callCount);

            // Set different.
            var item2 = new SubToolStripItem();
            button.DefaultItem = item2;
            Assert.Same(item2, button.DefaultItem);
            Assert.Equal(2, callCount);

            // Set null.
            button.DefaultItem = null;
            Assert.Null(button.DefaultItem);
            Assert.Equal(3, callCount);

            // Remove handler.
            button.DefaultItemChanged -= handler;
            button.DefaultItem = item1;
            Assert.Equal(item1, button.DefaultItem);
            Assert.Equal(3, callCount);
        }

        private class SubToolStripItem : ToolStripItem
        {
            public SubToolStripItem() : base()
            {
            }
        }
    }
}
