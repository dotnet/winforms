// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ScrollableControlTests
    {
        [Fact]
        public void ScrollableControl_Ctor_Default()
        {
            var control = new SubScrollableControl();
            Assert.False(control.AutoScroll);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.False(control.HScrollEntry);
            Assert.False(control.VScrollEntry);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_AutoScroll_Set_GetReturnsExpected(bool value)
        {
            var control = new ScrollableControl()
            {
                AutoScroll = value
            };
            Assert.Equal(value, control.AutoScroll);
        }

        [Theory]
        [MemberData(nameof(TestHelper.GetSizeTheoryData), TestIncludeType.NoNegatives, MemberType = typeof(TestHelper))]
        public void ScrollableControl_AutoScrollMargin_Set_GetReturnsExpected(Size value)
        {
            var control = new ScrollableControl()
            {
                AutoScrollMargin = value
            };
            Assert.Equal(value, control.AutoScrollMargin);
        }

        public static IEnumerable<object[]> AutoScrollMargin_Invalid_TestData()
        {
            yield return new object[] { new Size(-1, 0) };
            yield return new object[] { new Size(0, -1) };
        }

        [Theory]
        [MemberData(nameof(AutoScrollMargin_Invalid_TestData))]
        public void ScrollableControl_AutoScrollMargin_SetInvalid_ThrowsArgumentOutOfRangeException(Size value)
        {
            var control = new ScrollableControl();
            Assert.Throws<ArgumentOutOfRangeException>("AutoScrollMargin", () => control.AutoScrollMargin = value);
        }

        public static IEnumerable<object[]> SetAutoScrollMargin_TestData()
        {
            yield return new object[] { -1, -1, new Size(0, 0) };
            yield return new object[] { 0, 0, new Size(0, 0) };
            yield return new object[] { 0, 1, new Size(0, 1) };
            yield return new object[] { 1, 0, new Size(1, 0) };
            yield return new object[] { 1, 2, new Size(1, 2) };
        }

        [Theory]
        [MemberData(nameof(SetAutoScrollMargin_TestData))]
        public void ScrollableControl_SetAutoScrollMargin_Invoke_Success(int width, int height, Size expectedAutoScrollMargin)
        {
            var control = new ScrollableControl();
            control.SetAutoScrollMargin(width, height);
            Assert.Equal(expectedAutoScrollMargin, control.AutoScrollMargin);
        }

        [Theory]
        [MemberData(nameof(TestHelper.GetPointTheoryData), MemberType = typeof(TestHelper))]
        public void ScrollableControl_AutoScrollPosition_Set_GetReturnsExpected(Point value)
        {
            var control = new ScrollableControl()
            {
                AutoScrollPosition = value
            };
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
        }

        [Theory]
        [MemberData(nameof(TestHelper.GetSizeTheoryData), MemberType = typeof(TestHelper))]
        public void ScrollableControl_AutoScrollMinSize_Set_GetReturnsExpected(Size value)
        {
            var control = new ScrollableControl()
            {
                AutoScrollMinSize = value
            };
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(value != Size.Empty, control.AutoScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_HScroll_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollableControl()
            {
                HScrollEntry = value
            };
            Assert.Equal(value, control.HScrollEntry);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_VScroll_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollableControl()
            {
                VScrollEntry = value
            };
            Assert.Equal(value, control.VScrollEntry);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public void ScrollableControl_DisplayRectangle_Get_ReturnsExpected(bool hScroll, bool vScroll)
        {
            var control = new SubScrollableControl
            {
                ClientSize = new Size(70, 80),
                Padding = new Padding(1, 2, 3, 4),
                HScrollEntry = hScroll,
                VScrollEntry = vScroll
            };
            Assert.Equal(new Rectangle(1, 2, 66, 74), control.DisplayRectangle);
        }

        [Fact]
        public void ScrollableControl_OnScroll_Invoke_Success()
        {
            var control = new SubScrollableControl();

            // No handler.
            control.OnScrollEntry(null);

            // Handler.
            int callCount = 0;
            ScrollEventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                callCount++;
            };

            control.Scroll += handler;
            control.OnScrollEntry(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.Scroll -= handler;
            control.OnScrollEntry(null);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ScrollableControl_OnPaddingChanged_Invoke_Success()
        {
            var control = new SubScrollableControl();

            // No handler.
            control.OnPaddingChangedEntry(null);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                callCount++;
            };

            control.PaddingChanged += handler;
            control.OnPaddingChangedEntry(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            control.PaddingChanged -= handler;
            control.OnPaddingChangedEntry(null);
            Assert.Equal(1, callCount);
        }

        private class SubScrollableControl : ScrollableControl
        {
            public bool HScrollEntry
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public bool VScrollEntry
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public void OnScrollEntry(ScrollEventArgs se) => OnScroll(se);

            public void OnPaddingChangedEntry(EventArgs e) => OnPaddingChanged(e);
        }
    }
}
