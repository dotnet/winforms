﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ScrollableControlTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ScrollableControl_Ctor_Default()
        {
            using var control = new SubScrollableControl();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
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
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
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
            Assert.False(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(0, control.Height);
            Assert.NotNull(control.HorizontalScroll);
            Assert.Same(control.HorizontalScroll, control.HorizontalScroll);
            Assert.False(control.HScroll);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(Size.Empty, control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(0, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(0, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollableControl_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubScrollableControl();
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
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0x56110000)]
        [InlineData(true, false, 0x56110000)]
        [InlineData(false, true, 0x56110000)]
        [InlineData(false, false, 0x56010000)]
        public void ScrollableControl_CreateParams_GetHScroll_ReturnsExpected(bool hScroll, bool horizontalScrollVisible, int expectedStyle)
        {
            using var control = new SubScrollableControl();
            control.HorizontalScroll.Visible = horizontalScrollVisible;
            control.HScroll = hScroll;

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(0, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(0, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, true, 0x56210000)]
        [InlineData(true, false, 0x56210000)]
        [InlineData(false, true, 0x56210000)]
        [InlineData(false, false, 0x56010000)]
        public void ScrollableControl_CreateParams_GetVScroll_ReturnsExpected(bool vScroll, bool verticalScrollVisbile, int expectedStyle)
        {
            using var control = new SubScrollableControl();
            control.VerticalScroll.Visible = verticalScrollVisbile;
            control.VScroll = vScroll;

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(0, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(0, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> SetClientRectangle_TestData()
        {
            const int width = 70;
            const int height = 80;

            // create handle
            yield return new object[] { true, width, height, width - 20, height + 50, new Rectangle(0, 0, width - SystemInformation.VerticalScrollBarWidth, height) };
            yield return new object[] { true, width, height, width + 50, height - 20, new Rectangle(0, 0, width, height - SystemInformation.HorizontalScrollBarHeight) };
            yield return new object[] { true, width, height, width + 50, height + 50, new Rectangle(0, 0, width - SystemInformation.VerticalScrollBarWidth, height - SystemInformation.HorizontalScrollBarHeight) };
            yield return new object[] { true, width, height, width - 20, height - 20, new Rectangle(0, 0, width, height) };

            // no handle
            yield return new object[] { false, width, height, width + 50, height - 20, new Rectangle(0, 0, width, height) };
            yield return new object[] { false, width, height, width - 20, height + 50, new Rectangle(0, 0, width, height) };
            yield return new object[] { false, width, height, width + 50, height + 50, new Rectangle(0, 0, width, height) };
            yield return new object[] { false, width, height, width - 20, height - 20, new Rectangle(0, 0, width, height) };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetClientRectangle_TestData))]
        public void ScrollableControl_ClientRectangle_should_reduce_if_scrollbars_shown(bool createHandle, int width, int height, int childWidth, int childHeight, Rectangle expected)
        {
            using var control = new SubScrollableControl
            {
                AutoScroll = true,
                ClientSize = new Size(width, height)
            };

            // if handle isn't created, scrollbars won't be rendered, which in turn affects the size of ClientRectangle
            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, control.Handle);
            }

            // add a child control
            var child = new Button
            {
                Width = childWidth,
                Height = childHeight
            };
            control.Controls.Add(child);

            Assert.Equal(expected, control.ClientRectangle);
        }

        [WinFormsTheory]
        [InlineData(true, 2, 5)]
        [InlineData(false, 1, 4)]
        public void ScrollableControl_AutoScroll_Set_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new SubScrollableControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoScroll", e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoScroll = !value;
            Assert.Equal(!value, control.AutoScroll);
            Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2, 5)]
        [InlineData(false, 1, 4)]
        public void ScrollableControl_AutoScroll_SetWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new SubScrollableControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("AutoScroll", e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AutoScroll = !value;
            Assert.Equal(!value, control.AutoScroll);
            Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> AutoScrollMargin_Set_TestData()
        {
            yield return new object[] { true, new Size(0, 0), 0 };
            yield return new object[] { true, new Size(1, 0), 1 };
            yield return new object[] { true, new Size(0, 1), 1 };
            yield return new object[] { true, new Size(1, 2), 1 };
            yield return new object[] { false, new Size(0, 0), 0 };
            yield return new object[] { false, new Size(1, 0), 0 };
            yield return new object[] { false, new Size(0, 1), 0 };
            yield return new object[] { false, new Size(1, 2), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(AutoScrollMargin_Set_TestData))]
        public void ScrollableControl_AutoScrollMargin_Set_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
        {
            using var control = new ScrollableControl
            {
                AutoScroll = autoScroll
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Null(e.AffectedControl);
                Assert.Null(e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoScrollMargin = value;
            Assert.Equal(value, control.AutoScrollMargin);
            Assert.Equal(autoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollMargin = value;
            Assert.Equal(value, control.AutoScrollMargin);
            Assert.Equal(autoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(AutoScrollMargin_Set_TestData))]
        public void ScrollableControl_AutoScrollMargin_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
        {
            using var control = new ScrollableControl
            {
                AutoScroll = autoScroll
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Null(e.AffectedControl);
                Assert.Null(e.AffectedProperty);
                layoutCallCount++;
            };

            control.AutoScrollMargin = value;
            Assert.Equal(value, control.AutoScrollMargin);
            Assert.Equal(autoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoScrollMargin = value;
            Assert.Equal(value, control.AutoScrollMargin);
            Assert.Equal(autoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ScrollableControl_AutoScrollMargin_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMargin)];
            using var control = new ScrollableControl();
            Assert.False(property.CanResetValue(control));

            control.AutoScrollMargin = new Size(1, 0);
            Assert.Equal(new Size(1, 0), control.AutoScrollMargin);
            Assert.True(property.CanResetValue(control));

            control.AutoScrollMargin = new Size(0, 1);
            Assert.Equal(new Size(0, 1), control.AutoScrollMargin);
            Assert.True(property.CanResetValue(control));

            control.AutoScrollMargin = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.AutoScrollMargin);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ScrollableControl_AutoScrollMargin_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMargin)];
            using var control = new ScrollableControl();
            Assert.False(property.ShouldSerializeValue(control));

            control.AutoScrollMargin = new Size(1, 0);
            Assert.Equal(new Size(1, 0), control.AutoScrollMargin);
            Assert.True(property.ShouldSerializeValue(control));

            control.AutoScrollMargin = new Size(0, 1);
            Assert.Equal(new Size(0, 1), control.AutoScrollMargin);
            Assert.True(property.ShouldSerializeValue(control));

            control.AutoScrollMargin = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.AutoScrollMargin);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.AutoScrollMargin);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(-1, -2)]
        public void ScrollableControl_AutoScrollMargin_SetInvalid_ThrowsArgumentOutOfRangeException(int x, int y)
        {
            using var control = new ScrollableControl();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.AutoScrollMargin = new Size(x, y));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ScrollableControl_AutoScrollPosition_Set_GetReturnsExpected(Point value)
        {
            using var control = new ScrollableControl
            {
                AutoScrollPosition = value
            };
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ScrollableControl_AutoScrollPosition_SetWithHandle_GetReturnsExpected(Point value)
        {
            using var control = new ScrollableControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ScrollableControl_AutoScrollPosition_SetWithAutoScroll_GetReturnsExpected(Point value)
        {
            using var control = new ScrollableControl
            {
                AutoScroll = true,
                AutoScrollPosition = value
            };
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetPointTheoryData))]
        public void ScrollableControl_AutoScrollPosition_SetWithVisibleBars_GetReturnsExpected(Point value)
        {
            using var control = new ScrollableControl();
            control.HorizontalScroll.Visible = true;
            control.VerticalScroll.Visible = true;

            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollPosition = value;
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> AutoScrollMinSize_TestData()
        {
            yield return new object[] { true, new Size(-1, -2), true, 3 };
            yield return new object[] { true, new Size(0, 0), true, 0 };
            yield return new object[] { true, new Size(1, 0), true, 4 };
            yield return new object[] { true, new Size(0, 1), true, 4 };
            yield return new object[] { true, new Size(1, 2), true, 4 };

            yield return new object[] { false, new Size(-1, -2), true, 3 };
            yield return new object[] { false, new Size(0, 0), false, 0 };
            yield return new object[] { false, new Size(1, 0), true, 4 };
            yield return new object[] { false, new Size(0, 1), true, 4 };
            yield return new object[] { false, new Size(1, 2), true, 4 };
        }

        [WinFormsTheory]
        [MemberData(nameof(AutoScrollMinSize_TestData))]
        public void ScrollableControl_AutoScrollMinSize_Set_GetReturnsExpected(bool autoScroll, Size value, bool expectedAutoScroll, int expectedLayoutCallCount)
        {
            using var control = new ScrollableControl
            {
                AutoScroll = autoScroll
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                layoutCallCount++;
            };

            control.AutoScrollMinSize = value;
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(expectedAutoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScrollMinSize = value;
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(expectedAutoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> AutoScrollMinSize_WithHandle_TestData()
        {
            yield return new object[] { true, new Size(-1, -2), true, 3, 0 };
            yield return new object[] { true, new Size(0, 0), true, 0, 0 };
            yield return new object[] { true, new Size(1, 0), true, 4, 2 };
            yield return new object[] { true, new Size(0, 1), true, 4, 2 };
            yield return new object[] { true, new Size(1, 2), true, 4, 2 };

            yield return new object[] { false, new Size(-1, -2), true, 3, 0 };
            yield return new object[] { false, new Size(0, 0), false, 0, 0 };
            yield return new object[] { false, new Size(1, 0), true, 4, 2 };
            yield return new object[] { false, new Size(0, 1), true, 4, 2 };
            yield return new object[] { false, new Size(1, 2), true, 4, 2 };
        }

        [WinFormsTheory]
        [MemberData(nameof(AutoScrollMinSize_WithHandle_TestData))]
        public void ScrollableControl_AutoScrollMinSize_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value, bool expectedAutoScroll, int expectedLayoutCallCount, int expectedStyleChangedCallCount)
        {
            using var control = new ScrollableControl
            {
                AutoScroll = autoScroll
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                layoutCallCount++;
            };

            control.AutoScrollMinSize = value;
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(expectedAutoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoScrollMinSize = value;
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(expectedAutoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedStyleChangedCallCount, invalidatedCallCount);
            Assert.Equal(expectedStyleChangedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ScrollableControl_AutoScrollMinSize_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMinSize)];
            using var control = new ScrollableControl();
            Assert.False(property.CanResetValue(control));

            control.AutoScrollMinSize = new Size(1, 0);
            Assert.Equal(new Size(1, 0), control.AutoScrollMinSize);
            Assert.True(property.CanResetValue(control));

            control.AutoScrollMinSize = new Size(0, 1);
            Assert.Equal(new Size(0, 1), control.AutoScrollMinSize);
            Assert.True(property.CanResetValue(control));

            control.AutoScrollMinSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.AutoScrollMinSize);
            Assert.True(property.CanResetValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.False(property.CanResetValue(control));
        }

        [WinFormsFact]
        public void ScrollableControl_AutoScrollMinSize_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ScrollableControl))[nameof(ScrollableControl.AutoScrollMinSize)];
            using var control = new ScrollableControl();
            Assert.False(property.ShouldSerializeValue(control));

            control.AutoScrollMinSize = new Size(1, 0);
            Assert.Equal(new Size(1, 0), control.AutoScrollMinSize);
            Assert.True(property.ShouldSerializeValue(control));

            control.AutoScrollMinSize = new Size(0, 1);
            Assert.Equal(new Size(0, 1), control.AutoScrollMinSize);
            Assert.True(property.ShouldSerializeValue(control));

            control.AutoScrollMinSize = new Size(1, 2);
            Assert.Equal(new Size(1, 2), control.AutoScrollMinSize);
            Assert.True(property.ShouldSerializeValue(control));

            property.ResetValue(control);
            Assert.Equal(Size.Empty, control.AutoScrollMinSize);
            Assert.False(property.ShouldSerializeValue(control));
        }

        [WinFormsTheory]
        [InlineData(true, false, false)]
        [InlineData(false, false, false)]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        public void ScrollableControl_DisplayRectangle_GetWithClientRectangle_ReturnsExpected(bool autoScroll, bool hScroll, bool vScroll)
        {
            using var control = new SubScrollableControl
            {
                ClientSize = new Size(70, 80),
                Padding = new Padding(1, 2, 3, 4),
                AutoScroll = autoScroll,
                HScroll = hScroll,
                VScroll = vScroll
            };
            Assert.Equal(new Rectangle(1, 2, 66, 74), control.DisplayRectangle);

            // Get again.
            Assert.Equal(new Rectangle(1, 2, 66, 74), control.DisplayRectangle);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_HScroll_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubScrollableControl
            {
                HScroll = value
            };
            Assert.Equal(value, control.HScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.HScroll = value;
            Assert.Equal(value, control.HScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.HScroll = !value;
            Assert.Equal(!value, control.HScroll);
            Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_HScroll_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new SubScrollableControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.HScroll = value;
            Assert.Equal(value, control.HScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.HScroll = value;
            Assert.Equal(value, control.HScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.HScroll = !value;
            Assert.Equal(!value, control.HScroll);
            Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateHScrollVisible));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlTests.Padding_Set_TestData), MemberType = typeof(ControlTests))]
        public void ScrollableControl_Padding_Set_GetReturnsExpected(Padding value, Padding expected, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new ScrollableControl();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Padding", e.AffectedProperty);
                layoutCallCount++;
            };

            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Padding = value;
            Assert.Equal(expected, control.Padding);
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ScrollableControl_Padding_SetWithHandler_CallsPaddingChanged()
        {
            using var control = new ScrollableControl();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Equal(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.PaddingChanged += handler;

            // Set different.
            var padding1 = new Padding(1);
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set same.
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(1, callCount);

            // Set different.
            var padding2 = new Padding(2);
            control.Padding = padding2;
            Assert.Equal(padding2, control.Padding);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.PaddingChanged -= handler;
            control.Padding = padding1;
            Assert.Equal(padding1, control.Padding);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void ScrollableControl_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            using var control = new ScrollableControl
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_Visible_Set_GetReturnsExpected(bool value)
        {
            using var control = new ScrollableControl
            {
                Visible = value
            };
            Assert.Equal(value, control.Visible);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_VScroll_Set_GetReturnsExpected(bool value)
        {
            using var control = new SubScrollableControl
            {
                VScroll = value
            };
            Assert.Equal(value, control.VScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.VScroll = value;
            Assert.Equal(value, control.VScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.VScroll = !value;
            Assert.Equal(!value, control.VScroll);
            Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ScrollableControl_VScroll_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new SubScrollableControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.VScroll = value;
            Assert.Equal(value, control.VScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.VScroll = value;
            Assert.Equal(value, control.VScroll);
            Assert.Equal(value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.VScroll = !value;
            Assert.Equal(!value, control.VScroll);
            Assert.Equal(!value, control.GetScrollState(SubScrollableControl.ScrollStateVScrollVisible));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ScrollableControl_ScrollStateAutoScrolling_Get_ReturnsExpected()
        {
            Assert.Equal(0x0001, SubScrollableControl.ScrollStateAutoScrolling);
        }

        [WinFormsFact]
        public void ScrollableControl_ScrollStateHScrollVisible_Get_ReturnsExpected()
        {
            Assert.Equal(0x0002, SubScrollableControl.ScrollStateHScrollVisible);
        }

        [WinFormsFact]
        public void ScrollableControl_ScrollStateVScrollVisible_Get_ReturnsExpected()
        {
            Assert.Equal(0x0004, SubScrollableControl.ScrollStateVScrollVisible);
        }

        [WinFormsFact]
        public void ScrollableControl_ScrollStateUserHasScrolled_Get_ReturnsExpected()
        {
            Assert.Equal(0x0008, SubScrollableControl.ScrollStateUserHasScrolled);
        }

        [WinFormsFact]
        public void ScrollableControl_ScrollStateFullDrag_Get_ReturnsExpected()
        {
            Assert.Equal(0x0010, SubScrollableControl.ScrollStateFullDrag);
        }

        public static IEnumerable<object[]> AdjustFormScrollbars_TestData()
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                yield return new object[] { rightToLeft, true, 0, true, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 0, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 1, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 1, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 0, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 1, false, false, 0, false, 1 };
                yield return new object[] { rightToLeft, true, 1, true, 1, true, false, 0, false, 1 };

                yield return new object[] { rightToLeft, true, 0, false, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 0, false, 0, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 0, false, 1, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 0, false, 1, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 0, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 1, false, false, 0, false, 1 };
                yield return new object[] { rightToLeft, true, 1, false, 1, true, false, 0, false, 1 };

                yield return new object[] { rightToLeft, false, 0, true, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 0, true, 0, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 0, true, 1, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 0, true, 1, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 0, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 1, false, false, 0, false, 1 };
                yield return new object[] { rightToLeft, false, 1, true, 1, true, false, 0, false, 1 };

                yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 0, true, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 1, false, false, 0, false, 1 };
                yield return new object[] { rightToLeft, false, 0, false, 1, true, false, 0, false, 1 };
                yield return new object[] { rightToLeft, false, 1, false, 0, false, false, 1, false, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 0, true, false, 1, false, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 1, false, false, 1, false, 1 };
                yield return new object[] { rightToLeft, false, 1, false, 1, true, false, 1, false, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustFormScrollbars_TestData))]
        public void ScrollableControl_AdjustFormScrollbars_Invoke_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue)
        {
            using var control = new SubScrollableControl
            {
                RightToLeft = rightToLeft,
                HScroll = hScroll,
                VScroll = vScroll
            };
            control.HorizontalScroll.Value = hValue;
            control.VerticalScroll.Value = vValue;

            control.AdjustFormScrollbars(displayScrollbars);
            Assert.Equal(expectedHScroll, control.HScroll);
            Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
            Assert.False(control.HorizontalScroll.Visible);
            Assert.Equal(expectedVScroll, control.VScroll);
            Assert.Equal(expectedVValue, control.VerticalScroll.Value);
            Assert.False(control.VerticalScroll.Visible);
            Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustFormScrollbars_TestData))]
        public void ScrollableControl_AdjustFormScrollbars_InvokeAutoScroll_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue)
        {
            using var control = new SubScrollableControl
            {
                AutoScroll = true,
                RightToLeft = rightToLeft,
                HScroll = hScroll,
                VScroll = vScroll
            };
            control.HorizontalScroll.Value = hValue;
            control.VerticalScroll.Value = vValue;

            control.AdjustFormScrollbars(displayScrollbars);
            Assert.Equal(expectedHScroll, control.HScroll);
            Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
            Assert.False(control.HorizontalScroll.Visible);
            Assert.Equal(expectedVScroll, control.VScroll);
            Assert.Equal(expectedVValue, control.VerticalScroll.Value);
            Assert.False(control.VerticalScroll.Visible);
            Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> AdjustFormScrollbars_AutoScrollMinSize_TestData()
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                yield return new object[] { rightToLeft, true, 0, true, 0, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 0, true, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 1, true, true, 0, true, 1 };
                yield return new object[] { rightToLeft, true, 1, true, 0, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 0, true, true, 1, true, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 1, true, true, 1, true, 1 };

                yield return new object[] { rightToLeft, true, 0, false, 0, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 0, false, 0, true, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 0, false, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 0, false, 1, true, true, 0, true, 1 };
                yield return new object[] { rightToLeft, true, 1, false, 0, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 0, true, true, 1, true, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 1, true, true, 1, true, 1 };

                yield return new object[] { rightToLeft, false, 0, true, 0, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 0, true, 0, true, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 0, true, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 0, true, 1, true, true, 0, true, 1 };
                yield return new object[] { rightToLeft, false, 1, true, 0, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 0, true, true, 1, true, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 1, true, true, 1, true, 1 };

                yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 0, true, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 1, true, true, 0, true, 1 };
                yield return new object[] { rightToLeft, false, 1, false, 0, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 0, true, true, 1, true, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 1, false, true, 0, true, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 1, true, true, 1, true, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustFormScrollbars_AutoScrollMinSize_TestData))]
        public void ScrollableControl_AdjustFormScrollbars_InvokeAutoScrollMinSize_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue)
        {
            using var control = new SubScrollableControl
            {
                AutoScrollMinSize = new Size(10, 20),
                RightToLeft = rightToLeft,
                HScroll = hScroll,
                VScroll = vScroll
            };
            control.HorizontalScroll.Value = hValue;
            control.VerticalScroll.Value = vValue;

            control.AdjustFormScrollbars(displayScrollbars);
            Assert.Equal(expectedHScroll, control.HScroll);
            Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
            Assert.True(control.HorizontalScroll.Visible);
            Assert.Equal(expectedVScroll, control.VScroll);
            Assert.Equal(expectedVValue, control.VerticalScroll.Value);
            Assert.True(control.VerticalScroll.Visible);
            Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> AdjustFormScrollbars_WithHandle_TestData()
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                yield return new object[] { rightToLeft, true, 0, true, 0, false, false, 0, false, 0, 1 };
                yield return new object[] { rightToLeft, true, 0, true, 0, true, false, 0, false, 0, 1 };
                yield return new object[] { rightToLeft, true, 0, true, 1, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 1, true, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 0, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 0, true, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 1, false, false, 0, false, 1, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 1, true, false, 0, false, 1, 0 };

                yield return new object[] { rightToLeft, true, 0, false, 0, false, false, 0, false, 0, 1 };
                yield return new object[] { rightToLeft, true, 0, false, 0, true, false, 0, false, 0, 1 };
                yield return new object[] { rightToLeft, true, 0, false, 1, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 0, false, 1, true, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 0, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 0, true, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 1, false, false, 0, false, 1, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 1, true, false, 0, false, 1, 0 };

                yield return new object[] { rightToLeft, false, 0, true, 0, false, false, 0, false, 0, 1 };
                yield return new object[] { rightToLeft, false, 0, true, 0, true, false, 0, false, 0, 1 };
                yield return new object[] { rightToLeft, false, 0, true, 1, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 0, true, 1, true, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 0, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 0, true, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 1, false, false, 0, false, 1, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 1, true, false, 0, false, 1, 0 };

                yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 0, true, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 1, false, false, 0, false, 1, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 1, true, false, 0, false, 1, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 0, false, false, 1, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 0, true, false, 1, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 1, false, false, 1, false, 1, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 1, true, false, 1, false, 1, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustFormScrollbars_WithHandle_TestData))]
        public void ScrollableControl_AdjustFormScrollbars_InvokeWithHandle_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue, int expectedInvalidatedCallCount)
        {
            using var control = new SubScrollableControl
            {
                RightToLeft = rightToLeft,
                HScroll = hScroll,
                VScroll = vScroll
            };
            control.HorizontalScroll.Value = hValue;
            control.VerticalScroll.Value = vValue;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AdjustFormScrollbars(displayScrollbars);
            Assert.Equal(expectedHScroll, control.HScroll);
            Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
            Assert.False(control.HorizontalScroll.Visible);
            Assert.Equal(expectedVScroll, control.VScroll);
            Assert.Equal(expectedVValue, control.VerticalScroll.Value);
            Assert.False(control.VerticalScroll.Visible);
            Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustFormScrollbars_WithHandle_TestData))]
        public void ScrollableControl_AdjustFormScrollbars_InvokeWithHandleAutoScroll_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue, int expectedInvalidatedCallCount)
        {
            using var control = new SubScrollableControl
            {
                AutoScroll = true,
                RightToLeft = rightToLeft,
                HScroll = hScroll,
                VScroll = vScroll
            };
            control.HorizontalScroll.Value = hValue;
            control.VerticalScroll.Value = vValue;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AdjustFormScrollbars(displayScrollbars);
            Assert.Equal(expectedHScroll, control.HScroll);
            Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
            Assert.False(control.HorizontalScroll.Visible);
            Assert.Equal(expectedVScroll, control.VScroll);
            Assert.Equal(expectedVValue, control.VerticalScroll.Value);
            Assert.False(control.VerticalScroll.Visible);
            Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> AdjustFormScrollbars_WithHandleAutoScrollMinSize_TestData()
        {
            foreach (RightToLeft rightToLeft in Enum.GetValues(typeof(RightToLeft)))
            {
                yield return new object[] { rightToLeft, true, 0, true, 0, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 0, true, 0, true, true, 0, true, 0, 0 };
                yield return new object[] { rightToLeft, true, 0, true, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 0, true, 1, true, true, 0, true, 1, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 0, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 1, true, 0, true, true, 1, true, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, true, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 1, true, 1, true, true, 1, true, 1, 0 };

                yield return new object[] { rightToLeft, true, 0, false, 0, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 0, false, 0, true, true, 0, true, 0, 1 };
                yield return new object[] { rightToLeft, true, 0, false, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 0, false, 1, true, true, 0, true, 1, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 0, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 1, false, 0, true, true, 1, true, 0, 0 };
                yield return new object[] { rightToLeft, true, 1, false, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, true, 1, false, 1, true, true, 1, true, 1, 0 };

                yield return new object[] { rightToLeft, false, 0, true, 0, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, false, 0, true, 0, true, true, 0, true, 0, 1 };
                yield return new object[] { rightToLeft, false, 0, true, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, false, 0, true, 1, true, true, 0, true, 1, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 0, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, false, 1, true, 0, true, true, 1, true, 0, 0 };
                yield return new object[] { rightToLeft, false, 1, true, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, false, 1, true, 1, true, true, 1, true, 1, 0 };

                yield return new object[] { rightToLeft, false, 0, false, 0, false, false, 0, false, 0, 0 };
                yield return new object[] { rightToLeft, false, 0, false, 0, true, true, 0, true, 0, 1 };
                yield return new object[] { rightToLeft, false, 0, false, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, false, 0, false, 1, true, true, 0, true, 1, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 0, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, false, 1, false, 0, true, true, 1, true, 0, 0 };
                yield return new object[] { rightToLeft, false, 1, false, 1, false, true, 0, true, 0, 2 };
                yield return new object[] { rightToLeft, false, 1, false, 1, true, true, 1, true, 1, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(AdjustFormScrollbars_WithHandleAutoScrollMinSize_TestData))]
        public void ScrollableControl_AdjustFormScrollbars_InvokeWithHandleAutoScrollMinSize_Success(RightToLeft rightToLeft, bool hScroll, int hValue, bool vScroll, int vValue, bool displayScrollbars, bool expectedHScroll, int expectedHValue, bool expectedVScroll, int expectedVValue, int expectedInvalidatedCallCount)
        {
            using var control = new SubScrollableControl
            {
                AutoScrollMinSize = new Size(10, 20),
                RightToLeft = rightToLeft,
                HScroll = hScroll,
                VScroll = vScroll
            };
            control.HorizontalScroll.Value = hValue;
            control.VerticalScroll.Value = vValue;
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AdjustFormScrollbars(displayScrollbars);
            Assert.Equal(expectedHScroll, control.HScroll);
            Assert.Equal(expectedHValue, control.HorizontalScroll.Value);
            Assert.True(control.HorizontalScroll.Visible);
            Assert.Equal(expectedVScroll, control.VScroll);
            Assert.Equal(expectedVValue, control.VerticalScroll.Value);
            Assert.True(control.VerticalScroll.Visible);
            Assert.False(control.GetScrollState(SubScrollableControl.ScrollStateUserHasScrolled));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ScrollableControl_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubScrollableControl();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubScrollableControl.ScrollStateAutoScrolling, false)]
        [InlineData(SubScrollableControl.ScrollStateFullDrag, false)]
        [InlineData(SubScrollableControl.ScrollStateHScrollVisible, false)]
        [InlineData(SubScrollableControl.ScrollStateUserHasScrolled, false)]
        [InlineData(SubScrollableControl.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void ScrollableControl_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubScrollableControl();
            Assert.Equal(expected, control.GetScrollState(bit));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, true)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, false)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void ScrollableControl_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubScrollableControl();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void ScrollableControl_OnScroll_Invoke_CallsHandler()
        {
            using var control = new SubScrollableControl();
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

        [WinFormsFact]
        public void ScrollableControl_OnPaddingChanged_Invoke_CallsHandler()
        {
            using var control = new SubScrollableControl();
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

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetLayoutEventArgsTheoryData))]
        public void ScrollableControl_OnLayout_Invoke_CallsLayout(LayoutEventArgs eventArgs)
        {
            using var control = new SubScrollableControl();
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
        [WinFormsFact]
        public void ScrollableControl_ScaleCore_InvokeWithDockPadding_Success()
        {
            using var control = new ScrollableControl
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

        [WinFormsFact]
        public void ScrollableControl_ScaleCore_InvokeWithoutDockPadding_Success()
        {
            using var control = new ScrollableControl();
            control.Scale(10, 20);
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(Padding.Empty, control.Padding);
        }
#pragma warning restore 0618

        [WinFormsFact]
        public void ScrollableControl_ScaleControl_InvokeWithDockPadding_Success()
        {
            using var control = new ScrollableControl
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

        [WinFormsFact]
        public void ScrollableControl_ScaleControl_InvokeWithoutDockPadding_Success()
        {
            using var control = new ScrollableControl();
            control.Scale(new SizeF(10, 20));
            Assert.Equal(0, control.DockPadding.Left);
            Assert.Equal(0, control.DockPadding.Top);
            Assert.Equal(0, control.DockPadding.Right);
            Assert.Equal(0, control.DockPadding.Bottom);
            Assert.Equal(Padding.Empty, control.Padding);
        }

        public static IEnumerable<object[]> SetAutoScrollMargin_TestData()
        {
            yield return new object[] { true, -1, -1, new Size(0, 0), 0 };
            yield return new object[] { true, 0, 0, new Size(0, 0), 0 };
            yield return new object[] { true, 0, 1, new Size(0, 1), 1 };
            yield return new object[] { true, 1, 0, new Size(1, 0), 1 };
            yield return new object[] { true, 1, 2, new Size(1, 2), 1 };
            yield return new object[] { false, -1, -1, new Size(0, 0), 0 };
            yield return new object[] { false, 0, 0, new Size(0, 0), 0 };
            yield return new object[] { false, 0, 1, new Size(0, 1), 0 };
            yield return new object[] { false, 1, 0, new Size(1, 0), 0 };
            yield return new object[] { false, 1, 2, new Size(1, 2), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetAutoScrollMargin_TestData))]
        public void ScrollableControl_SetAutoScrollMargin_Invoke_Success(bool autoScroll, int width, int height, Size expectedAutoScrollMargin, int expectedLayoutCallCount)
        {
            using var control = new ScrollableControl
            {
                AutoScroll = autoScroll
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Null(e.AffectedControl);
                Assert.Null(e.AffectedProperty);
                layoutCallCount++;
            };

            control.SetAutoScrollMargin(width, height);
            Assert.Equal(expectedAutoScrollMargin, control.AutoScrollMargin);
            Assert.Equal(autoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetAutoScrollMargin_TestData))]
        public void ScrollableControl_SetAutoScrollMargin_InvokeWithHandle_Success(bool autoScroll, int width, int height, Size expectedAutoScrollMargin, int expectedLayoutCallCount)
        {
            using var control = new ScrollableControl
            {
                AutoScroll = autoScroll
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Null(e.AffectedControl);
                Assert.Null(e.AffectedProperty);
                layoutCallCount++;
            };

            control.SetAutoScrollMargin(width, height);
            Assert.Equal(expectedAutoScrollMargin, control.AutoScrollMargin);
            Assert.Equal(autoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SetDisplayRectLocation_TestData()
        {
            const int width = 70;
            const int height = 80;
            Size scrollableSize = new Size(100, 150);
            Size nonScrollableSize = new Size(width, height);

            yield return new object[] { true, width, height, 0, 0, new Point(0, 0), scrollableSize };
            yield return new object[] { false, width, height, 0, 0, new Point(0, 0), nonScrollableSize };

            yield return new object[] { true, width, height, -10, 0, new Point(-10, 0), scrollableSize };
            yield return new object[] { false, width, height, -10, 0, new Point(0, 0), nonScrollableSize };

            yield return new object[] { true, width, height, 0, -20, new Point(0, -20), scrollableSize };
            yield return new object[] { false, width, height, 0, -20, new Point(0, 0), nonScrollableSize };

            yield return new object[] { true, width, height, -10, -20, new Point(-10, -20), scrollableSize };
            yield return new object[] { false, width, height, -10, -20, new Point(0, 0), nonScrollableSize };

            // Overflow.
            yield return new object[] { true, width, height, -100, -20, new Point(-30, -20), scrollableSize };
            yield return new object[] { false, width, height, -100, -20, new Point(0, 0), nonScrollableSize };

            yield return new object[] { true, width, height, -10, -200, new Point(-10, -70), scrollableSize };
            yield return new object[] { false, width, height, -10, -200, new Point(0, 0), nonScrollableSize };

            // Underflow.
            yield return new object[] { true, width, height, 1, 20, new Point(0, 0), scrollableSize };
            yield return new object[] { false, width, height, 1, 20, new Point(0, 0), nonScrollableSize };

            yield return new object[] { true, width, height, 10, 2, new Point(0, 0), scrollableSize };
            yield return new object[] { false, width, height, 10, 2, new Point(0, 0), nonScrollableSize };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetDisplayRectLocation_TestData))]
        public void ScrollableControl_SetDisplayRectLocation_Invoke_Success(bool autoScroll, int width, int height, int scrollX, int scrollY, Point expectedDisplayRectangleLocation, Size expectedDisplayRectangleSize)
        {
            using var control = new SubScrollableControl
            {
                AutoScroll = autoScroll,
                ClientSize = new Size(width, height)
            };

            // Without child.
            control.SetDisplayRectLocation(scrollX, scrollY);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);

            // With child.
            using var child = new LargeControl();
            control.Controls.Add(child);
            Assert.Equal(child.ExpectedSize, child.Bounds);

            control.SetDisplayRectLocation(scrollX, scrollY);
            Assert.Equal(expectedDisplayRectangleSize, control.DisplayRectangle.Size);
            Assert.Equal(expectedDisplayRectangleLocation, control.DisplayRectangle.Location);
            Assert.Equal(expectedDisplayRectangleLocation, control.AutoScrollPosition);
            Assert.Equal(child.ExpectedSize, child.Bounds);
        }

        public static IEnumerable<object[]> SetDisplayRectLocation_WithHandle_TestData()
        {
            const int width = 70;
            const int height = 80;
            Size scrollableSize = new Size(100, 150);
            Size nonScrollableSize = new Size(width, height);

            yield return new object[] { true, width, height, 0, 0, new Point(0, 0), scrollableSize, 1 };
            yield return new object[] { false, width, height, 0, 0, new Point(0, 0), nonScrollableSize, 0 };

            yield return new object[] { true, width, height, -10, 0, new Point(-10, 0), scrollableSize, 1 };
            yield return new object[] { false, width, height, -10, 0, new Point(0, 0), nonScrollableSize, 0 };

            yield return new object[] { true, width, height, 0, -20, new Point(0, -20), scrollableSize, 1 };
            yield return new object[] { false, width, height, 0, -20, new Point(0, 0), nonScrollableSize, 0 };

            yield return new object[] { true, width, height, -10, -20, new Point(-10, -20), scrollableSize, 1 };
            yield return new object[] { false, width, height, -10, -20, new Point(0, 0), nonScrollableSize, 0 };

            // Overflow.
            yield return new object[] { true, width, height, -100, -20, new Point(-47, -20), scrollableSize, 1 };
            yield return new object[] { false, width, height, -100, -20, new Point(0, 0), nonScrollableSize, 0 };

            yield return new object[] { true, width, height, -10, -200, new Point(-10, -87), scrollableSize, 1 };
            yield return new object[] { false, width, height, -10, -200, new Point(0, 0), nonScrollableSize, 0 };

            // Underflow.
            yield return new object[] { true, width, height, 1, 20, new Point(0, 0), scrollableSize, 1 };
            yield return new object[] { false, width, height, 1, 20, new Point(0, 0), nonScrollableSize, 0 };

            yield return new object[] { true, width, height, 10, 2, new Point(0, 0), scrollableSize, 1 };
            yield return new object[] { false, width, height, 10, 2, new Point(0, 0), nonScrollableSize, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetDisplayRectLocation_WithHandle_TestData))]
        public void ScrollableControl_SetDisplayRectLocation_InvokeWithHandle_Success(bool autoScroll, int width, int height, int scrollX, int scrollY, Point expectedDisplayRectangleLocation, Size expectedDisplayRectangleSize, int expectedInvalidatedCallCount)
        {
            using var control = new SubScrollableControl
            {
                AutoScroll = autoScroll,
                ClientSize = new Size(width, height)
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            // Without child.
            control.SetDisplayRectLocation(scrollX, scrollY);
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(Point.Empty, control.AutoScrollPosition);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // With child.
            using var child = new LargeControl();
            control.Controls.Add(child);
            Assert.Equal(child.ExpectedSize, child.Bounds);

            control.SetDisplayRectLocation(scrollX, scrollY);
            Assert.Equal(expectedDisplayRectangleSize, control.DisplayRectangle.Size);
            Assert.Equal(expectedDisplayRectangleLocation, control.DisplayRectangle.Location);
            Assert.Equal(expectedDisplayRectangleLocation, control.AutoScrollPosition);
            Assert.Equal(new Rectangle(expectedDisplayRectangleLocation, child.ExpectedSize.Size), child.Bounds);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SetScrollState_TestData()
        {
            yield return new object[] { 1, true, true };
            yield return new object[] { 1, false, false };
            yield return new object[] { 0, true, true };
            yield return new object[] { 0, false, true };
        }

        [Theory]
        [MemberData(nameof(SetScrollState_TestData))]
        public void ScrollableControl_SetScrollState_Invoke_GetScrollStateReturnsExpected(int bit, bool value, bool expected)
        {
            var control = new SubScrollableControl();
            control.SetScrollState(bit, value);
            Assert.Equal(expected, control.GetScrollState(bit));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.SetScrollState(bit, value);
            Assert.Equal(expected, control.GetScrollState(bit));
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [MemberData(nameof(SetScrollState_TestData))]
        public void ScrollableControl_GetScrollState_InvokeWithHandle_GetStyleReturnsExpected(int bit, bool value, bool expected)
        {
            var control = new SubScrollableControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SetScrollState(bit, value);
            Assert.Equal(expected, control.GetScrollState(bit));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SetScrollState(bit, value);
            Assert.Equal(expected, control.GetScrollState(bit));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
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

        [WinFormsTheory]
        [MemberData(nameof(ScrollControlIntoView_TestData))]
        public void ScrollableControl_ScrollControlIntoView_Invoke_Success(bool autoScroll, bool hScroll, bool vScroll, Size clientSize, Control activeControl, Rectangle expectedDisplayRectangle)
        {
            using var control = new SubScrollableControl
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

            control.Controls.Clear();
        }

        private class LargeControl : Control
        {
            protected override Size DefaultSize => new Size(100, 150);

            public Rectangle ExpectedSize => new Rectangle(new Point(0, 0), DefaultSize);
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

            public new bool DoubleBuffered
            {
                get => base.DoubleBuffered;
                set => base.DoubleBuffered = value;
            }

            public new EventHandlerList Events => base.Events;

            public new int FontHeight
            {
                get => base.FontHeight;
                set => base.FontHeight = value;
            }

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool HScroll
            {
                get => base.HScroll;
                set => base.HScroll = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }

            public new void AdjustFormScrollbars(bool displayScrollbars) => base.AdjustFormScrollbars(displayScrollbars);

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

            public new void OnScroll(ScrollEventArgs se) => base.OnScroll(se);

            public new void OnPaddingChanged(EventArgs e) => base.OnPaddingChanged(e);

            public new void SetDisplayRectLocation(int x, int y) => base.SetDisplayRectLocation(x, y);

            public new void SetScrollState(int bit, bool value) => base.SetScrollState(bit, value);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
