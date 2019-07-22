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
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoScroll);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(0, control.Bottom);
            Assert.Equal(Rectangle.Empty, control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Null(control.Container);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(Size.Empty, control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(Rectangle.Empty, control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.NotNull(control.DockPadding);
            Assert.Same(control.DockPadding, control.DockPadding);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(0, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Equal(0, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(0, control.Width);
        }

        [Fact]
        public void ScrollableControl_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubScrollableControl();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(0, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(0, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_AutoScroll_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollableControl
            {
                AutoScroll = value
            };
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));

            // Set same.
            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoNegatives)]
        public void ScrollableControl_AutoScrollMargin_Set_GetReturnsExpected(Size value)
        {
            var control = new ScrollableControl
            {
                AutoScrollMargin = value
            };
            Assert.Equal(value, control.AutoScrollMargin);

            // Set same.
            control.AutoScrollMargin = value;
            Assert.Equal(value, control.AutoScrollMargin);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData), TestIncludeType.NoPositives)]
        public void ScrollableControl_AutoScrollMargin_SetInvalid_ThrowsArgumentOutOfRangeException(Size value)
        {
            var control = new ScrollableControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.AutoScrollMargin = value);
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
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ScrollableControl_AutoScrollPosition_Set_GetReturnsExpected(Point value)
        {
            var control = new ScrollableControl
            {
                AutoScrollPosition = value
            };
            Assert.Equal(Point.Empty, control.AutoScrollPosition);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ScrollableControl_AutoScrollPosition_SetWithAutoScroll_GetReturnsExpected(Point value)
        {
            var control = new ScrollableControl
            {
                AutoScroll = true,
                AutoScrollPosition = value
            };
            Assert.Equal(Point.Empty, control.AutoScrollPosition);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ScrollableControl_AutoScrollPosition_SetWithVisibleBars_GetReturnsExpected(Point value)
        {
            var control = new ScrollableControl();
            control.HorizontalScroll.Visible = true;
            control.VerticalScroll.Visible = true;

            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetSizeTheoryData))]
        public void ScrollableControl_AutoScrollMinSize_Set_GetReturnsExpected(Size value)
        {
            var control = new ScrollableControl
            {
                AutoScrollMinSize = value
            };
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(value != Size.Empty, control.AutoScroll);

            // Set same.
            control.AutoScrollMinSize = value;
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(value != Size.Empty, control.AutoScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_HScroll_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollableControl
            {
                HScroll = value
            };
            Assert.Equal(value, control.HScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));

            // Set same.
            control.HScroll = value;
            Assert.Equal(value, control.HScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_VScroll_Set_GetReturnsExpected(bool value)
        {
            var control = new SubScrollableControl
            {
                VScroll = value
            };
            Assert.Equal(value, control.VScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));

            // Set same.
            control.HScroll = value;
            Assert.Equal(value, control.HScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
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
                HScroll = hScroll,
                VScroll = vScroll
            };
            Assert.Equal(new Rectangle(1, 2, 66, 74), control.DisplayRectangle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaddingNormalizedTheoryData))]
        public void ScrollableControl_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
        {
            var control = new ScrollableControl
            {
                Padding = value
            };
            Assert.Equal(expected, control.Padding);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ScrollableControl_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var control = new ScrollableControl
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Fact]
        public void ScrollableControl_Visible_Set_GetReturnsExpected()
        {
            var control = new ScrollableControl
            {
                Visible = false
            };
            Assert.False(control.Visible);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
        }

        [Fact]
        public void ScrollableControl_ScrollStateAutoScrolling_Get_ReturnsExpected()
        {
            Assert.Equal(0x0001, SubScrollableControl.ScrollStateAutoScrolling);
        }

        [Fact]
        public void ScrollableControl_ScrollStateHScrollVisible_Get_ReturnsExpected()
        {
            Assert.Equal(0x0002, SubScrollableControl.ScrollStateHScrollVisible);
        }

        [Fact]
        public void ScrollableControl_ScrollStateVScrollVisible_Get_ReturnsExpected()
        {
            Assert.Equal(0x0004, SubScrollableControl.ScrollStateVScrollVisible);
        }

        [Fact]
        public void ScrollableControl_ScrollStateUserHasScrolled_Get_ReturnsExpected()
        {
            Assert.Equal(0x0008, SubScrollableControl.ScrollStateUserHasScrolled);
        }

        [Fact]
        public void ScrollableControl_ScrollStateFullDrag_Get_ReturnsExpected()
        {
            Assert.Equal(0x0010, SubScrollableControl.ScrollStateFullDrag);
        }

        [Fact]
        public void ScrollableControl_OnScroll_Invoke_CallsHandler()
        {
            var control = new SubScrollableControl();
            var eventArgs = new ScrollEventArgs(ScrollEventType.First, 0);
            int callCount = 0;
            ScrollEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Scroll += handler;
            control.OnScroll(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Scroll -= handler;
            control.OnScroll(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void ScrollableControl_OnPaddingChanged_Invoke_CallsHandler()
        {
            var control = new SubScrollableControl();
            var eventArgs = new EventArgs();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.PaddingChanged += handler;
            control.OnPaddingChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.OnPaddingChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> AdjustFormScrollbars_TestData()
        {
            foreach (bool hScroll in new bool[] { true, false })
            {
                foreach (bool vScroll in new bool[] { true, false })
                {
                    var control1 = new SubScrollableControl
                    {
                        HScroll = hScroll,
                        VScroll = vScroll
                    };
                    yield return new object[] { control1, false, false, false };

                    var control2 = new SubScrollableControl
                    {
                        HScroll = hScroll,
                        VScroll = vScroll
                    };
                    yield return new object[] { control2, true, false, false };
                }
            }

            var controlWithAutoScrollMinSize = new SubScrollableControl
            {
                HScroll = true,
                VScroll = true,
                AutoScrollMinSize = new Size(10, 20)
            };
            yield return new object[] { controlWithAutoScrollMinSize, true, true, true };
        }

        [Theory]
        [MemberData(nameof(AdjustFormScrollbars_TestData))]
        public void ScrollableControl_AdjustFormScrollbars_Invoke_Success(SubScrollableControl control, bool displayScrollbars, bool expectedHScroll, bool expectedVScroll)
        {
            control.AdjustFormScrollbars(displayScrollbars);
            Assert.Equal(expectedHScroll, control.HScroll);
            Assert.Equal(expectedVScroll, control.VScroll);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void ScrollableControl_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
        {
            var control = new SubScrollableControl();
            int callCount = 0;
            LayoutEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            control.Layout += handler;
            control.OnLayout(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           control.Layout -= handler;
           control.OnLayout(eventArgs);
           Assert.Equal(1, callCount);
        }

#pragma warning disable 0618
        [Fact]
        public void ScrollableControl_ScaleCore_InvokeWithDockPadding_Success()
        {
            var control = new ScrollableControl
            {
                Padding = new Padding(1, 2, 3, 4)
            };
            Assert.Equal(1, control.DockPadding.Left);
            Assert.Equal(2, control.DockPadding.Top);
            Assert.Equal(3, control.DockPadding.Right);
            Assert.Equal(4, control.DockPadding.Bottom);
            control.Scale(10, 20);

            Assert.Equal(1, control.DockPadding.Left);
            Assert.Equal(2, control.DockPadding.Top);
            Assert.Equal(3, control.DockPadding.Right);
            Assert.Equal(4, control.DockPadding.Bottom);
            Assert.Equal(new Padding(1, 2, 3, 4), control.Padding);
        }

        [Fact]
        public void ScrollableControl_ScaleCore_InvokeWithoutDockPadding_Success()
        {
            var control = new ScrollableControl();
            control.Scale(10, 20);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(Padding.Empty, control.Padding);
        }
#pragma warning restore 0618

        [Fact]
        public void ScrollableControl_ScaleControl_InvokeWithDockPadding_Success()
        {
            var control = new ScrollableControl
            {
                Padding = new Padding(1, 2, 3, 4)
            };
            Assert.Equal(1, control.DockPadding.Left);
            Assert.Equal(2, control.DockPadding.Top);
            Assert.Equal(3, control.DockPadding.Right);
            Assert.Equal(4, control.DockPadding.Bottom);
            control.Scale(new SizeF(10, 20));

            Assert.Equal(10, control.DockPadding.Left);
            Assert.Equal(40, control.DockPadding.Top);
            Assert.Equal(30, control.DockPadding.Right);
            Assert.Equal(80, control.DockPadding.Bottom);
            Assert.Equal(new Padding(10, 40, 30, 80), control.Padding);
        }

        [Fact]
        public void ScrollableControl_ScaleControl_InvokeWithoutDockPadding_Success()
        {
            var control = new ScrollableControl();
            control.Scale(new SizeF(10, 20));
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(Padding.Empty, control.Padding);
        }

        public static IEnumerable<object[]> SetDisplayRectLocation_TestData()
        {
            yield return new object[] { true, 0, 0, new Rectangle(0, 0, 100, 150), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { false, 0, 0, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };

            yield return new object[] { true, -10, 0, new Rectangle(-10, 0, 110, 150), new Rectangle(-10, 0, 100, 150) };
            yield return new object[] { false, -10, 0, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };

            yield return new object[] { true, 0, -20, new Rectangle(0, -20, 100, 150), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { false, 0, -20, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };

            yield return new object[] { true, -10, -20, new Rectangle(-10, -20, 110, 170), new Rectangle(-10, -20, 100, 150) };
            yield return new object[] { false, -10, -20, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };

            // Overflow.
            yield return new object[] { true, -100, -20, new Rectangle(-30, -20, 130, 170), new Rectangle(-30, -20, 100, 150) };
            yield return new object[] { false, -100, -20, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };

            yield return new object[] { true, -10, -200, new Rectangle(-10, -70, 110, 220), new Rectangle(-10, -70, 100, 150) };
            yield return new object[] { false, -10, -200, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };

            // Underflow.
            yield return new object[] { true, 1, 20, new Rectangle(0, 0, 100, 150), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { false, 1, 20, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };

            yield return new object[] { true, 10, 2, new Rectangle(0, 0, 100, 150), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { false, 10, 2, new Rectangle(0, 0, 70, 80), new Rectangle(0, 0, 100, 150) };
        }

        [Theory]
        [MemberData(nameof(SetDisplayRectLocation_TestData))]
        public void SetDisplayRectLocation_InvokeWithoutHandle_Success(bool autoScroll, int x, int y, Rectangle expectedDisplayRectangle, Rectangle expectedBounds)
        {
            var control = new SubScrollableControl
            {
                AutoScroll = autoScroll,
                ClientSize = new Size(70, 80)
            };

            // Without child.
            control.SetDisplayRectLocation(x, y);
            Assert.Equal(new Rectangle(0, 0, 70, 80), control.DisplayRectangle);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);

            // With child.
            var child = new LargeControl();
            control.Controls.Add(child);
            Assert.Equal(new Rectangle(0, 0, 100, 150), child.Bounds);

            control.SetDisplayRectLocation(x, y);
            Assert.Equal(expectedDisplayRectangle, control.DisplayRectangle);
            Assert.Equal(expectedDisplayRectangle.Location, control.AutoScrollPosition);
            Assert.Equal(expectedBounds, child.Bounds);
        }

        [Theory]
        [MemberData(nameof(SetDisplayRectLocation_TestData))]
        public void SetDisplayRectLocation_InvokeWithHandle_Success(bool autoScroll, int x, int y, Rectangle expectedDisplayRectangle, Rectangle expectedBounds)
        {
            var control = new SubScrollableControl
            {
                AutoScroll = autoScroll,
                ClientSize = new Size(70, 80)
            };

            // Without child.
            control.SetDisplayRectLocation(x, y);
            Assert.Equal(new Rectangle(0, 0, 70, 80), control.DisplayRectangle);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);

            // With child.
            var child = new LargeControl();
            control.Controls.Add(child);
            Assert.Equal(new Rectangle(0, 0, 100, 150), child.Bounds);

            // With created handle.
            Assert.NotEqual(IntPtr.Zero, child.Handle);
            control.SetDisplayRectLocation(x, y);
            Assert.Equal(expectedDisplayRectangle, control.DisplayRectangle);
            Assert.Equal(expectedDisplayRectangle.Location, control.AutoScrollPosition);
            Assert.Equal(expectedBounds, child.Bounds);
        }

        public static IEnumerable<object[]> ScrollControlIntoView_TestData()
        {
            // Can't scroll - invalid child.
            yield return new object[] { true, true, true, new Size(70, 80), null, new Rectangle(0, 0, 70, 80) };
            yield return new object[] { false, true, true, new Size(70, 80), null, new Rectangle(0, 0, 70, 80) };

            yield return new object[] { true, true, true, new Size(70, 80), new Control(), new Rectangle(0, 0, 70, 80) };

            // Can't scroll - not AutoScroll.
            yield return new object[] { false, true, true, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 70, 80) };

            // Can't scroll - not HScroll or VScroll.
            yield return new object[] { true, false, false, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };

            // Can't scroll - empty.
            yield return new object[] { true, false, false, new Size(0, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { true, false, false, new Size(-1, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { true, false, false, new Size(70, 0), new LargeControl(), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { true, false, false, new Size(70, -1), new LargeControl(), new Rectangle(0, 0, 100, 150) };

            // Can scroll.
            yield return new object[] { true, true, false, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { true, false, true, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };
            yield return new object[] { true, true, true, new Size(70, 80), new LargeControl(), new Rectangle(0, 0, 100, 150) };

            yield return new object[] { true, true, false, new Size(70, 80), new SmallControl(), new Rectangle(0, 0, 70, 80) };
            yield return new object[] { true, false, true, new Size(70, 80), new SmallControl(), new Rectangle(0, 0, 70, 80) };
            yield return new object[] { true, true, true, new Size(70, 80), new SmallControl(), new Rectangle(0, 0, 70, 80) };

            foreach (bool hScroll in new bool[] { true, false })
            {
                var childControl = new SmallControl();
                var parentControl = new LargeControl();
                parentControl.Controls.Add(childControl);
                yield return new object[] { true, true, true, new Size(70, 80), parentControl, new Rectangle(0, 0, 100, 150) };
                yield return new object[] { true, hScroll, true, new Size(70, 80), childControl, new Rectangle(0, 0, 100, 150) };
            }

            foreach (bool vScroll in new bool[] { true, false })
            {
                var childControl = new SmallControl();
                var parentControl = new LargeControl();
                parentControl.Controls.Add(childControl);
                yield return new object[] { true, true, true, new Size(70, 80), parentControl, new Rectangle(0, 0, 100, 150) };
                yield return new object[] { true, true, vScroll, new Size(70, 80), childControl, new Rectangle(0, 0, 100, 150) };
            }
        }

        [Theory]
        [MemberData(nameof(ScrollControlIntoView_TestData))]
        public void ScrollControlIntoView_Invoke_Success(bool autoScroll, bool hScroll, bool vScroll, Size clientSize, Control activeControl, Rectangle expectedDisplayRectangle)
        {
            var control = new SubScrollableControl
            {
                AutoScroll = autoScroll,
                HScroll = hScroll,
                VScroll = vScroll,
                ClientSize = clientSize
            };
            if ((activeControl is LargeControl || activeControl is SmallControl))
            {
                control.Controls.Add(activeControl.Parent ?? activeControl);
            }
            control.ScrollControlIntoView(activeControl);
            Assert.Equal(expectedDisplayRectangle, control.DisplayRectangle);
        }

        private class LargeControl : Control
        {
            protected override Size DefaultSize => new Size(100, 150);
        }

        private class SmallControl : Control
        {
            protected override Size DefaultSize => new Size(50, 60);
        }

        public class SubScrollableControl : ScrollableControl
        {
            public new const int ScrollStateAutoScrolling = ScrollableControl.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = ScrollableControl.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = ScrollableControl.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = ScrollableControl.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = ScrollableControl.ScrollStateFullDrag;

            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new ImeMode ImeModeBase => base.ImeModeBase;

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new void AdjustFormScrollbars(bool displayScrollbars) => base.AdjustFormScrollbars(displayScrollbars);

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

            public new void OnScroll(ScrollEventArgs se) => base.OnScroll(se);

            public new void OnPaddingChanged(EventArgs e) => base.OnPaddingChanged(e);

            public new void SetDisplayRectLocation(int x, int y) => base.SetDisplayRectLocation(x, y);
        }
    }
}
