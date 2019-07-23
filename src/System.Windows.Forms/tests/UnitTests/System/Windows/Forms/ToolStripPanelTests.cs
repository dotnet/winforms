// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripPanelTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var panel = new SubToolStripPanel();
            Assert.False(panel.AllowDrop);
            Assert.False(panel.AutoScroll);
            Assert.Equal(Size.Empty, panel.AutoScrollMargin);
            Assert.Equal(Point.Empty, panel.AutoScrollPosition);
            Assert.True(panel.AutoSize);
            Assert.Equal(Padding.Empty, panel.DefaultMargin);
            Assert.Equal(Padding.Empty, panel.DefaultPadding);
            Assert.NotNull(panel.DockPadding);
            Assert.Same(panel.DockPadding, panel.DockPadding);
            Assert.Equal(0, panel.DockPadding.Top);
            Assert.Equal(0, panel.DockPadding.Bottom);
            Assert.Equal(0, panel.DockPadding.Left);
            Assert.Equal(0, panel.DockPadding.Right);
            Assert.Equal(Rectangle.Empty, panel.DisplayRectangle);
            Assert.NotNull(panel.HorizontalScroll);
            Assert.Same(panel.HorizontalScroll, panel.HorizontalScroll);
            Assert.False(panel.HScroll);
            Assert.Equal(Padding.Empty, panel.Margin);
            Assert.Equal(Padding.Empty, panel.Padding);
            Assert.Equal(new Padding(3, 0, 0, 0), panel.RowMargin);
            Assert.NotNull(panel.VerticalScroll);
            Assert.Same(panel.VerticalScroll, panel.VerticalScroll);
            Assert.False(panel.VScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AllowDrop_Set_GetReturnsExpected(bool value)
        {
            var panel = new ToolStripPanel
            {
                AllowDrop = value
            };
            Assert.Equal(value, panel.AllowDrop);

            // Set same.
            panel.AllowDrop = value;
            Assert.Equal(value, panel.AllowDrop);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AutoScroll_Set_GetReturnsExpected(bool value)
        {
            var panel = new ToolStripPanel
            {
                AutoScroll = value
            };
            Assert.Equal(value, panel.AutoScroll);

            // Set same.
            panel.AutoScroll = value;
            Assert.Equal(value, panel.AutoScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoNegatives)]
        public void AutoScrollMargin_Set_GetReturnsExpected(Size value)
        {
            var panel = new ToolStripPanel
            {
                AutoScrollMargin = value
            };
            Assert.Equal(value, panel.AutoScrollMargin);

            // Set same.
            panel.AutoScrollMargin = value;
            Assert.Equal(value, panel.AutoScrollMargin);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoPositives)]
        public void AutoScrollMargin_SetInvalid_ThrowsArgumentOutOfRangeException(Size value)
        {
            var panel = new ToolStripPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => panel.AutoScrollMargin = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void AutoScrollMinSize_Set_GetReturnsExpected(Size value)
        {
            var panel = new ToolStripPanel
            {
                AutoScrollMinSize = value
            };
            Assert.Equal(value, panel.AutoScrollMinSize);
            Assert.Equal(value != Size.Empty, panel.AutoScroll);

            // Set same.
            panel.AutoScrollMinSize = value;
            Assert.Equal(value, panel.AutoScrollMinSize);
            Assert.Equal(value != Size.Empty, panel.AutoScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AutoSize_Set_GetReturnsExpected(bool value)
        {
            var panel = new ToolStripPanel
            {
                AutoSize = value
            };
            Assert.Equal(value, panel.AutoSize);

            // Set same.
            panel.AutoSize = value;
            Assert.Equal(value, panel.AutoSize);

            // Set different.
            panel.AutoSize = !value;
            Assert.Equal(!value, panel.AutoSize);
        }

        [Fact]
        public void AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            var panel = new ToolStripPanel
            {
                AutoSize = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(panel, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            panel.AutoSizeChanged += handler;

            // Set different.
            panel.AutoSize = false;
            Assert.False(panel.AutoSize);
            Assert.Equal(1, callCount);

            // Set same.
            panel.AutoSize = false;
            Assert.False(panel.AutoSize);
            Assert.Equal(1, callCount);

            // Set different.
            panel.AutoSize = true;
            Assert.True(panel.AutoSize);
            Assert.Equal(2, callCount);

            // Remove handler.
            panel.AutoSizeChanged -= handler;
            panel.AutoSize = false;
            Assert.False(panel.AutoSize);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingTheoryData))]
        public void RowMargin_Set_GetReturnsExpected(Padding value)
        {
            var panel = new ToolStripPanel
            {
                RowMargin = value
            };
            Assert.Equal(value, panel.RowMargin);

            // Set same.
            panel.RowMargin = value;
            Assert.Equal(value, panel.RowMargin);
        }

        private class SubToolStripPanel : ToolStripPanel
        {
            public new Padding DefaultPadding => base.DefaultPadding;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new bool HScroll => base.HScroll;

            public new bool VScroll => base.VScroll;
        }
    }
}
