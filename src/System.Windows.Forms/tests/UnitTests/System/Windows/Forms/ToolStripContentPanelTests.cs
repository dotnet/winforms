// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using Moq.Protected;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ToolStripContentPanelTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripContentPanel_Ctor_SplitContainer()
        {
            using var control = new SubToolStripContentPanel();
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
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.None, control.BorderStyle);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Equal(Cursors.Default, control.Cursor);
            Assert.Equal(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(200, 100), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
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
            Assert.Equal(100, control.Height);
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
            Assert.IsType<ToolStripSystemRenderer>(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
            Assert.True(control.ResizeRedraw);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.False(control.VScroll);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlTests.Anchor_Set_TestData), MemberType = typeof(ControlTests))]
        public void ToolStripContentPanel_Anchor_Set_GetReturnsExpected(AnchorStyles value, AnchorStyles expected)
        {
            using var control = new ToolStripContentPanel
            {
                Anchor = value
            };
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Anchor = value;
            Assert.Equal(expected, control.Anchor);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2, 5)]
        [InlineData(false, 1, 4)]
        public void ToolStripContentPanel_AutoScroll_Set_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new SubToolStripContentPanel();
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
            Assert.Equal(value, control.GetScrollState(SubToolStripContentPanel.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubToolStripContentPanel.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoScroll = !value;
            Assert.Equal(!value, control.AutoScroll);
            Assert.Equal(!value, control.GetScrollState(SubToolStripContentPanel.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2, 5)]
        [InlineData(false, 1, 4)]
        public void ToolStripContentPanel_AutoScroll_SetWithHandle_GetReturnsExpected(bool value, int expectedLayoutCallCount1, int expectedLayoutCallCount2)
        {
            using var control = new SubToolStripContentPanel();
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
            Assert.Equal(value, control.GetScrollState(SubToolStripContentPanel.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoScroll = value;
            Assert.Equal(value, control.AutoScroll);
            Assert.Equal(value, control.GetScrollState(SubToolStripContentPanel.ScrollStateAutoScrolling));
            Assert.Equal(expectedLayoutCallCount1 * 2, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AutoScroll = !value;
            Assert.Equal(!value, control.AutoScroll);
            Assert.Equal(!value, control.GetScrollState(SubToolStripContentPanel.ScrollStateAutoScrolling));
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
        public void ToolStripContentPanel_AutoScrollMargin_Set_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
        {
            using var control = new ToolStripContentPanel
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
        public void ToolStripContentPanel_AutoScrollMargin_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value, int expectedLayoutCallCount)
        {
            using var control = new ToolStripContentPanel
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

        public static IEnumerable<object[]> AutoScrollMinSize_TestData()
        {
            yield return new object[] { true, new Size(-1, -2), true, 3 };
            yield return new object[] { true, new Size(0, 0), true, 0 };
            yield return new object[] { true, new Size(1, 0), true, 3 };
            yield return new object[] { true, new Size(0, 1), true, 3 };
            yield return new object[] { true, new Size(1, 2), true, 3 };

            yield return new object[] { false, new Size(-1, -2), true, 3 };
            yield return new object[] { false, new Size(0, 0), false, 0 };
            yield return new object[] { false, new Size(1, 0), true, 3 };
            yield return new object[] { false, new Size(0, 1), true, 3 };
            yield return new object[] { false, new Size(1, 2), true, 3 };
        }

        [WinFormsTheory]
        [MemberData(nameof(AutoScrollMinSize_TestData))]
        public void ToolStripContentPanel_AutoScrollMinSize_Set_GetReturnsExpected(bool autoScroll, Size value, bool expectedAutoScroll, int expectedLayoutCallCount)
        {
            using var control = new ToolStripContentPanel
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

        [WinFormsTheory]
        [MemberData(nameof(AutoScrollMinSize_TestData))]
        public void ToolStripContentPanel_AutoScrollMinSize_SetWithHandle_GetReturnsExpected(bool autoScroll, Size value, bool expectedAutoScroll, int expectedLayoutCallCount)
        {
            using var control = new ToolStripContentPanel
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
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoScrollMinSize = value;
            Assert.Equal(value, control.AutoScrollMinSize);
            Assert.Equal(expectedAutoScroll, control.AutoScroll);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripContentPanel_AutoSize_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStripContentPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoSize = value;
            Assert.Equal(value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoSize = !value;
            Assert.Equal(!value, control.AutoSize);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_AutoSize_SetWithHandler_CallsAutoSizeChanged()
        {
            using var control = new ToolStripContentPanel
            {
                AutoSize = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.AutoSizeChanged += handler;

            // Set different.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set same.
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(1, callCount);

            // Set different.
            control.AutoSize = true;
            Assert.True(control.AutoSize);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.AutoSizeChanged -= handler;
            control.AutoSize = false;
            Assert.False(control.AutoSize);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void ToolStripContentPanel_AutoSizeMode_Set_GetReturnsExpected(AutoSizeMode value)
        {
            using var control = new ToolStripContentPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void ToolStripContentPanel_AutoSizeMode_SetWithParent_GetReturnsExpected(AutoSizeMode value)
        {
            using var parent = new Control();
            using var control = new ToolStripContentPanel
            {
                Parent = parent
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            parent.Layout += (sender, e) => parentLayoutCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(0, parentLayoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.False(control.IsHandleCreated);
            Assert.Equal(0, layoutCallCount);
            Assert.False(parent.IsHandleCreated);
            Assert.Equal(0, parentLayoutCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void ToolStripContentPanel_AutoSizeMode_SetWithHandle_GetReturnsExpected(AutoSizeMode value)
        {
            using var control = new ToolStripContentPanel();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AutoSizeMode))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(AutoSizeMode))]
        public void ToolStripContentPanel_AutoSizeMode_SetWithHandleWithParent_GetReturnsExpected(AutoSizeMode value)
        {
            using var parent = new Control();
            using var control = new ToolStripContentPanel
            {
                Parent = parent
            };
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int parentInvalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            parent.HandleCreated += (sender, e) => parentCreatedCallCount++;
            int parentLayoutCallCount = 0;
            parent.Layout += (sender, e) => parentLayoutCallCount++;

            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.Equal(0, parentLayoutCallCount);

            // Set same.
            control.AutoSizeMode = value;
            Assert.Equal(AutoSizeMode.GrowOnly, control.AutoSizeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.True(parent.IsHandleCreated);
            Assert.Equal(0, parentInvalidatedCallCount);
            Assert.Equal(0, parentStyleChangedCallCount);
            Assert.Equal(0, parentCreatedCallCount);
            Assert.Equal(0, parentLayoutCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void ToolStripContentPanel_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new ToolStripContentPanel
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_SetWithParent_TestData()
        {
            yield return new object[] { Color.Empty, Color.Blue };
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.Transparent, Color.Transparent };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetWithParent_TestData))]
        public void ToolStripContentPanel_BackColor_SetWithParent_GetReturnsExpected(Color value, Color expected)
        {
            using var parent = new Control
            {
                BackColor = Color.Blue
            };
            using var control = new ToolStripContentPanel
            {
                Parent = parent,
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(Color.Blue, parent.BackColor);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(Color.Blue, parent.BackColor);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }

        public static IEnumerable<object[]> BackColor_SetWithToolStripContainerParent_TestData()
        {
            yield return new object[] { Color.Empty, Color.Blue, Color.Blue };
            yield return new object[] { Color.Red, Color.Red, Color.Blue };
            yield return new object[] { Color.Transparent, Color.Transparent, Color.Transparent };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetWithToolStripContainerParent_TestData))]
        public void ToolStripContentPanel_BackColor_SetWithToolStripContainerParent_GetReturnsExpected(Color value, Color expected, Color expectedBackColor)
        {
            using var parent = new ToolStripContainer
            {
                BackColor = Color.Blue
            };
            ToolStripContentPanel control = parent.ContentPanel;

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expectedBackColor, parent.BackColor);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);

            // Set same.
            Assert.Equal(expected, control.BackColor);
            Assert.Equal(expectedBackColor, parent.BackColor);
            Assert.False(control.IsHandleCreated);
            Assert.False(parent.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new ToolStripContentPanel();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.BackColorChanged += handler;

            // Set different.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.BackColor = Color.Empty;
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void ToolStripContentPanel_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            using var control = new ToolStripContentPanel
            {
                BorderStyle = value
            };
            Assert.Equal(value, control.BorderStyle);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(BorderStyle.Fixed3D, 1)]
        [InlineData(BorderStyle.FixedSingle, 1)]
        [InlineData(BorderStyle.None, 0)]
        public void ToolStripContentPanel_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value, int expectedInvalidatedCallCount)
        {
            using var control = new ToolStripContentPanel();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BorderStyle = value;
            Assert.Equal(value, control.BorderStyle);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
            Assert.Equal(expectedInvalidatedCallCount, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void ToolStripContentPanel_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            using var control = new ToolStripContentPanel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BorderStyle = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripContentPanel_CausesValidation_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStripContentPanel
            {
                CausesValidation = value
            };
            Assert.Equal(value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);

            // Set same
            control.CausesValidation = value;
            Assert.Equal(value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);

            // Set different
            control.CausesValidation = !value;
            Assert.Equal(!value, control.CausesValidation);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
        {
            using var control = new ToolStripContentPanel
            {
                CausesValidation = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.CausesValidationChanged += handler;

            // Set different.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set same.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set different.
            control.CausesValidation = true;
            Assert.True(control.CausesValidation);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.CausesValidationChanged -= handler;
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(DockStyle))]
        public void ToolStripContentPanel_Dock_Set_GetReturnsExpected(DockStyle value)
        {
            using var control = new ToolStripContentPanel
            {
                Dock = value
            };
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);

            // Set same.
            control.Dock = value;
            Assert.Equal(value, control.Dock);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_Dock_SetWithHandler_CallsDockChanged()
        {
            using var control = new ToolStripContentPanel
            {
                Dock = DockStyle.None
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.DockChanged += handler;

            // Set different.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set same.
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(1, callCount);

            // Set different.
            control.Dock = DockStyle.Left;
            Assert.Equal(DockStyle.Left, control.Dock);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.DockChanged -= handler;
            control.Dock = DockStyle.Top;
            Assert.Equal(DockStyle.Top, control.Dock);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(DockStyle))]
        public void ToolStripContentPanel_Dock_SetInvalid_ThrowsInvalidEnumArgumentException(DockStyle value)
        {
            using var control = new ToolStripContentPanel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Dock = value);
        }

        public static IEnumerable<object[]> Location_Set_TestData()
        {
            yield return new object[] { new Point(0, 0), 0 };
            yield return new object[] { new Point(-1, -2), 1 };
            yield return new object[] { new Point(1, 0), 1 };
            yield return new object[] { new Point(0, 2), 1 };
            yield return new object[] { new Point(1, 2), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Location_Set_TestData))]
        public void ToolStripContentPanel_Location_Set_GetReturnsExpected(Point value, int expectedLocationChangedCallCount)
        {
            using var control = new ToolStripContentPanel();
            int moveCallCount = 0;
            int locationChangedCallCount = 0;
            int layoutCallCount = 0;
            int resizeCallCount = 0;
            int sizeChangedCallCount = 0;
            int clientSizeChangedCallCount = 0;
            control.Move += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(locationChangedCallCount, moveCallCount);
                moveCallCount++;
            };
            control.LocationChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.Equal(moveCallCount - 1, locationChangedCallCount);
                locationChangedCallCount++;
            };
            control.Layout += (sender, e) => layoutCallCount++;
            control.Resize += (sender, e) => resizeCallCount++;
            control.SizeChanged += (sender, e) => sizeChangedCallCount++;
            control.ClientSizeChanged += (sender, e) => clientSizeChangedCallCount++;

            control.Location = value;
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X + 200, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y + 100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 200, 100), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Call again.
            control.Location = value;
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.DisplayRectangle);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(value.X, control.Left);
            Assert.Equal(value.X + 200, control.Right);
            Assert.Equal(value, control.Location);
            Assert.Equal(value.Y, control.Top);
            Assert.Equal(value.Y + 100, control.Bottom);
            Assert.Equal(200, control.Width);
            Assert.Equal(100, control.Height);
            Assert.Equal(new Rectangle(value.X, value.Y, 200, 100), control.Bounds);
            Assert.Equal(expectedLocationChangedCallCount, moveCallCount);
            Assert.Equal(expectedLocationChangedCallCount, locationChangedCallCount);
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, resizeCallCount);
            Assert.Equal(0, sizeChangedCallCount);
            Assert.Equal(0, clientSizeChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_Location_SetWithHandler_CallsLocationChanged()
        {
            using var control = new ToolStripContentPanel();
            int locationChangedCallCount = 0;
            EventHandler locationChangedHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                locationChangedCallCount++;
            };
            control.LocationChanged += locationChangedHandler;
            int moveCallCount = 0;
            EventHandler moveHandler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                moveCallCount++;
            };
            control.Move += moveHandler;

            // Set different.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set same.
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(1, locationChangedCallCount);
            Assert.Equal(1, moveCallCount);

            // Set different x.
            control.Location = new Point(2, 2);
            Assert.Equal(new Point(2, 2), control.Location);
            Assert.Equal(2, locationChangedCallCount);
            Assert.Equal(2, moveCallCount);

            // Set different y.
            control.Location = new Point(2, 3);
            Assert.Equal(new Point(2, 3), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);

            // Remove handler.
            control.LocationChanged -= locationChangedHandler;
            control.Move -= moveHandler;
            control.Location = new Point(1, 2);
            Assert.Equal(new Point(1, 2), control.Location);
            Assert.Equal(3, locationChangedCallCount);
            Assert.Equal(3, moveCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlTests.MaximumSize_Set_TestData), MemberType = typeof(ControlTests))]
        public void ToolStripContentPanel_MaximumSize_Set_GetReturnsExpected(Size value)
        {
            using var control = new ToolStripContentPanel
            {
                Size = Size.Empty
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;

            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MaximumSize = value;
            Assert.Equal(value, control.MaximumSize);
            Assert.Equal(Size.Empty, control.Size);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlTests.MinimumSize_Set_TestData), MemberType = typeof(ControlTests))]
        public void ToolStripContentPanel_MinimumSize_Set_GetReturnsExpected(Size value, Size expectedSize, int expectedLayoutCallCount)
        {
            using var control = new ToolStripContentPanel
            {
                Size = Size.Empty
            };
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Bounds", e.AffectedProperty);
                layoutCallCount++;
            };

            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MinimumSize = value;
            Assert.Equal(value, control.MinimumSize);
            Assert.Equal(expectedSize, control.Size);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolStripContentPanel_Name_Set_GetReturnsExpected(string value, string expected)
        {
            using var control = new ToolStripContentPanel
            {
                Name = value
            };
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);

            // Get again.
            control.Name = value;
            Assert.Equal(expected, control.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_Renderer_Get_CallsOnRendererChanged()
        {
            using var control = new ToolStripContentPanel();
            int rendererChangedCallCount = 0;
            control.RendererChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                rendererChangedCallCount++;
            };
            Assert.IsType<ToolStripSystemRenderer>(control.Renderer);
            Assert.Equal(1, rendererChangedCallCount);

            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(1, rendererChangedCallCount);
        }

        public static IEnumerable<object[]> Renderer_Set_TestData()
        {
            foreach (bool visible in new bool[] { true, false })
            {
                foreach (bool doubleBuffered in new bool[] { true, false })
                {
                    yield return new object[] { visible, doubleBuffered, new SubToolStripRenderer(), false };
                    yield return new object[] { visible, doubleBuffered, new ToolStripSystemRenderer(), false };
                    yield return new object[] { visible, doubleBuffered, new ToolStripProfessionalRenderer(), true };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Renderer_Set_TestData))]
        public void ToolStripContentPanel_Renderer_Set_ReturnsExpected(bool visible, bool doubleBuffered, ToolStripRenderer value, bool expectedDoubleBuffered)
        {
            using var control = new SubToolStripContentPanel
            {
                Visible = visible
            };
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);

            control.Renderer = value;
            Assert.Same(value, control.Renderer);
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Renderer = value;
            Assert.Same(value, control.Renderer);
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Set null.
            control.Renderer = null;
            Assert.NotNull(control.Renderer);
            Assert.NotSame(value, control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.True(control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Renderer_Set_TestData))]
        public void ToolStripContentPanel_Renderer_SetWithHandle_ReturnsExpected(bool visible, bool doubleBuffered, ToolStripRenderer value, bool expectedDoubleBuffered)
        {
            using var control = new SubToolStripContentPanel
            {
                Visible = visible
            };
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Renderer = value;
            Assert.Same(value, control.Renderer);
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Renderer = value;
            Assert.Same(value, control.Renderer);
            Assert.Equal(ToolStripRenderMode.Custom, control.RenderMode);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set null.
            control.Renderer = null;
            Assert.NotNull(control.Renderer);
            Assert.NotSame(value, control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.True(control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_Renderer_Set_CallsInitializeContentPanel()
        {
            using var control = new SubToolStripContentPanel();
            var mockToolStripRenderer = new Mock<ToolStripRenderer>(MockBehavior.Strict);
            mockToolStripRenderer
                .Protected()
                .Setup("InitializeContentPanel", control)
                .Verifiable();
            control.Renderer = mockToolStripRenderer.Object;
            Assert.Same(mockToolStripRenderer.Object, control.Renderer);
            mockToolStripRenderer.Protected().Verify("InitializeContentPanel", Times.Once(), control);

            // Call again.
            control.Renderer = mockToolStripRenderer.Object;
            Assert.Same(mockToolStripRenderer.Object, control.Renderer);
            mockToolStripRenderer.Protected().Verify("InitializeContentPanel", Times.Once(), control);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_Renderer_SetWithHandler_CallsRendererChanged()
        {
            using var control = new ToolStripContentPanel();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RendererChanged += handler;

            // Set different.
            var renderer = new SubToolStripRenderer();
            control.Renderer = renderer;
            Assert.Same(renderer, control.Renderer);
            Assert.Equal(2, callCount);

            // Set same.
            control.Renderer = renderer;
            Assert.Same(renderer, control.Renderer);
            Assert.Equal(2, callCount);

            // Set different.
            control.Renderer = null;
            Assert.NotNull(control.Renderer);
            Assert.NotSame(renderer, control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.RendererChanged -= handler;
            control.Renderer = renderer;
            Assert.Same(renderer, control.Renderer);
            Assert.Equal(3, callCount);
        }

        private class SubToolStripRenderer : ToolStripRenderer
        {
        }

        [WinFormsTheory]
        [InlineData(ToolStripRenderMode.ManagerRenderMode, typeof(ToolStripProfessionalRenderer), 3)]
        [InlineData(ToolStripRenderMode.Professional, typeof(ToolStripProfessionalRenderer), 4)]
        [InlineData(ToolStripRenderMode.System, typeof(ToolStripSystemRenderer), 4)]
        public void ToolStripContentPanel_RenderMode_Set_ReturnsExpected(ToolStripRenderMode value, Type expectedRendererType, int expectedSetSameCallCount)
        {
            using var control = new ToolStripContentPanel();
            int rendererChangedCallCount = 0;
            control.RendererChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                rendererChangedCallCount++;
            };

            // Set same.
            control.RenderMode = ToolStripRenderMode.System;
            Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.IsType<ToolStripSystemRenderer>(control.Renderer);
            Assert.Equal(2, rendererChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(3, rendererChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(expectedSetSameCallCount, rendererChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ToolStripRenderMode.Professional, typeof(ToolStripProfessionalRenderer))]
        [InlineData(ToolStripRenderMode.System, typeof(ToolStripSystemRenderer))]
        public void ToolStripContentPanel_RenderMode_SetWithCustomRenderer_ReturnsExpected(ToolStripRenderMode value, Type expectedRendererType)
        {
            using var control = new ToolStrip
            {
                Renderer = new SubToolStripRenderer()
            };
            int rendererChangedCallCount = 0;
            control.RendererChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                rendererChangedCallCount++;
            };

            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(1, rendererChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(2, rendererChangedCallCount);
            Assert.False(control.IsHandleCreated);

            // Set ManagerRenderMode.
            control.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, control.RenderMode);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.IsType<ToolStripProfessionalRenderer>(control.Renderer);
            Assert.Equal(3, rendererChangedCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(ToolStripRenderMode.ManagerRenderMode, typeof(ToolStripProfessionalRenderer), 3)]
        [InlineData(ToolStripRenderMode.Professional, typeof(ToolStripProfessionalRenderer), 4)]
        [InlineData(ToolStripRenderMode.System, typeof(ToolStripSystemRenderer), 4)]
        public void ToolStripContentPanel_RenderMode_SetWithHandle_ReturnsExpected(ToolStripRenderMode value, Type expectedRendererType, int expectedSetSameCallCount)
        {
            using var control = new ToolStripContentPanel();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int rendererChangedCallCount = 0;
            control.RendererChanged += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                rendererChangedCallCount++;
            };

            // Set same.
            control.RenderMode = ToolStripRenderMode.System;
            Assert.Equal(ToolStripRenderMode.System, control.RenderMode);
            Assert.NotNull(control.Renderer);
            Assert.Same(control.Renderer, control.Renderer);
            Assert.IsType<ToolStripSystemRenderer>(control.Renderer);
            Assert.Equal(2, rendererChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(3, rendererChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(3, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.RenderMode = value;
            Assert.Equal(value, control.RenderMode);
            Assert.IsType(expectedRendererType, control.Renderer);
            Assert.Equal(expectedSetSameCallCount, rendererChangedCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedSetSameCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolStripRenderMode))]
        public void ToolStripContentPanel_RenderMode_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolStripRenderMode value)
        {
            using var control = new ToolStripContentPanel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RenderMode = value);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_RenderMode_SetCustomThrowsInvalidEnumArgumentException()
        {
            using var control = new ToolStripContentPanel();
            Assert.Throws<NotSupportedException>(() => control.RenderMode = ToolStripRenderMode.Custom);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_RenderMode_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripContentPanel))[nameof(ToolStripContentPanel.RenderMode)];
            using var item = new SubToolStripContentPanel();
            Assert.False(property.CanResetValue(item));

            item.RenderMode = ToolStripRenderMode.Professional;
            Assert.Equal(ToolStripRenderMode.Professional, item.RenderMode);
            Assert.True(property.CanResetValue(item));

            item.RenderMode = ToolStripRenderMode.System;
            Assert.Equal(ToolStripRenderMode.System, item.RenderMode);
            Assert.False(property.CanResetValue(item));

            item.Renderer = new SubToolStripRenderer();
            Assert.Equal(ToolStripRenderMode.Custom, item.RenderMode);
            Assert.False(property.CanResetValue(item));

            item.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, item.RenderMode);
            Assert.True(property.CanResetValue(item));

            property.ResetValue(item);
            Assert.Equal(ToolStripRenderMode.System, item.RenderMode);
            Assert.False(property.CanResetValue(item));
        }

        [WinFormsFact]
        public void ToolStripContentPanel_RenderMode_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolStripContentPanel))[nameof(ToolStripContentPanel.RenderMode)];
            using var item = new SubToolStripContentPanel();
            Assert.False(property.ShouldSerializeValue(item));

            item.RenderMode = ToolStripRenderMode.Professional;
            Assert.Equal(ToolStripRenderMode.Professional, item.RenderMode);
            Assert.True(property.ShouldSerializeValue(item));

            item.RenderMode = ToolStripRenderMode.System;
            Assert.Equal(ToolStripRenderMode.System, item.RenderMode);
            Assert.False(property.ShouldSerializeValue(item));

            item.Renderer = new SubToolStripRenderer();
            Assert.Equal(ToolStripRenderMode.Custom, item.RenderMode);
            Assert.False(property.ShouldSerializeValue(item));

            item.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Assert.Equal(ToolStripRenderMode.ManagerRenderMode, item.RenderMode);
            Assert.True(property.ShouldSerializeValue(item));

            property.ResetValue(item);
            Assert.Equal(ToolStripRenderMode.System, item.RenderMode);
            Assert.False(property.ShouldSerializeValue(item));
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void ToolStripContentPanel_TabIndex_Set_GetReturnsExpected(int value)
        {
            using var control = new ToolStripContentPanel
            {
                TabIndex = value
            };
            Assert.Equal(value, control.TabIndex);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TabIndex = value;
            Assert.Equal(value, control.TabIndex);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_TabIndex_SetWithHandler_CallsTabIndexChanged()
        {
            using var control = new ToolStripContentPanel
            {
                TabIndex = 0
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabIndexChanged += handler;

            // Set different.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabIndex = 2;
            Assert.Equal(2, control.TabIndex);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabIndexChanged -= handler;
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
        {
            using var control = new ToolStripContentPanel();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripContentPanel_TabStop_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStripContentPanel
            {
                TabStop = value
            };
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripContentPanel_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new ToolStripContentPanel();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            using var control = new ToolStripContentPanel
            {
                TabStop = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabStopChanged += handler;

            // Set different.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabStop = true;
            Assert.True(control.TabStop);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubToolStripContentPanel();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubToolStripContentPanel.ScrollStateAutoScrolling, false)]
        [InlineData(SubToolStripContentPanel.ScrollStateFullDrag, false)]
        [InlineData(SubToolStripContentPanel.ScrollStateHScrollVisible, false)]
        [InlineData(SubToolStripContentPanel.ScrollStateUserHasScrolled, false)]
        [InlineData(SubToolStripContentPanel.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void ToolStripContentPanel_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubToolStripContentPanel();
            Assert.Equal(expected, control.GetScrollState(bit));
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, true)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, true)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, false)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
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
        public void ToolStripContentPanel_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubToolStripContentPanel();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void ToolStripContentPanel_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubToolStripContentPanel();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripContentPanel_Visible_Set_GetReturnsExpected(bool value)
        {
            using var control = new ToolStripContentPanel
            {
                Visible = value
            };
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripContentPanel_Visible_SetWithRenderer_GetReturnsExpected(bool value)
        {
            using var control = new ToolStripContentPanel();
            Assert.NotNull(control.Renderer);

            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.Visible = !value;
            Assert.Equal(!value, control.Visible);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_Visible_SetWithHandler_CallsVisibleChanged()
        {
            using var control = new ToolStripContentPanel
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripContentPanel_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubToolStripContentPanel();
            int loadCallCount = 0;
            control.Load += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                loadCallCount++;
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, loadCallCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(2, loadCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripContentPanel_OnLoad_Invoke_CallsLoad(EventArgs eventArgs)
        {
            using var control = new SubToolStripContentPanel();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Load += handler;
            control.OnLoad(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Load -= handler;
            control.OnLoad(eventArgs);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolStripContentPanel_OnPaintBackground_Invoke_CallsRenderToolStripContentPanelBackground(bool handled)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var eventArgs = new PaintEventArgs(graphics, new Rectangle(1, 2, 3, 4));

            var renderer = new SubToolStripRenderer();
            using var control = new SubToolStripContentPanel
            {
                Renderer = renderer
            };

            int callCount = 0;
            renderer.RenderToolStripContentPanelBackground += (sender, e) =>
            {
                Assert.Same(renderer, sender);
                Assert.Same(graphics, e.Graphics);
                Assert.Same(control, e.ToolStripContentPanel);
                Assert.False(e.Handled);

                e.Handled = handled;
                callCount++;
            };

            control.OnPaintBackground(eventArgs);
            Assert.Equal(1, callCount);

            // Call again.
            control.OnPaintBackground(eventArgs);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void ToolStripContentPanel_OnPaintBackground_InvokeNullE_ThrowsNullReferenceException()
        {
            using var control = new SubToolStripContentPanel();
            Assert.Throws<NullReferenceException>(() => control.OnPaintBackground(null));
        }

        public static IEnumerable<object[]> OnRendererChanged_TestData()
        {
            foreach (bool doubleBuffered in new bool[] { true, false })
            {
                yield return new object[] { doubleBuffered, null, null, true };
                yield return new object[] { doubleBuffered, null, new EventArgs(), true };
                yield return new object[] { doubleBuffered, new SubToolStripRenderer(), null, false };
                yield return new object[] { doubleBuffered, new SubToolStripRenderer(), new EventArgs(), false };
                yield return new object[] { doubleBuffered, new ToolStripSystemRenderer(), null, false };
                yield return new object[] { doubleBuffered, new ToolStripSystemRenderer(), new EventArgs(), false };
                yield return new object[] { doubleBuffered, new ToolStripProfessionalRenderer(), null, true };
                yield return new object[] { doubleBuffered, new ToolStripProfessionalRenderer(), new EventArgs(), true };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnRendererChanged_TestData))]
        public void ToolStripContentPanel_OnRendererChanged_Invoke_CallsRendererChanged(bool doubleBuffered, ToolStripRenderer renderer, EventArgs eventArgs, bool expectedDoubleBuffered)
        {
            using var control = new SubToolStripContentPanel
            {
                Renderer = renderer
            };
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.RendererChanged += handler;
            control.OnRendererChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.RendererChanged -= handler;
            control.OnRendererChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnRendererChanged_TestData))]
        public void ToolStripContentPanel_OnRendererChanged_InvokeWithHandle_CallsRendererChanged(bool doubleBuffered, ToolStripRenderer renderer, EventArgs eventArgs, bool expectedDoubleBuffered)
        {
            using var control = new SubToolStripContentPanel
            {
                Renderer = renderer
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            control.SetStyle(ControlStyles.OptimizedDoubleBuffer, doubleBuffered);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.RendererChanged += handler;
            control.OnRendererChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.RendererChanged -= handler;
            control.OnRendererChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedDoubleBuffered, control.GetStyle(ControlStyles.OptimizedDoubleBuffer));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ToolStripContentPanel_OnRendererChanged_Invoke_CallsInitializeContentPanel(EventArgs eventArgs)
        {
            using var control = new SubToolStripContentPanel();
            var mockToolStripRenderer = new Mock<ToolStripRenderer>(MockBehavior.Strict);
            mockToolStripRenderer
                .Protected()
                .Setup("InitializeContentPanel", control)
                .Verifiable();
            control.Renderer = mockToolStripRenderer.Object;
            Assert.Same(mockToolStripRenderer.Object, control.Renderer);
            mockToolStripRenderer.Protected().Verify("InitializeContentPanel", Times.Once(), control);

            control.OnRendererChanged(eventArgs);
            mockToolStripRenderer.Protected().Verify("InitializeContentPanel", Times.Exactly(2), control);

            // Call again.
            control.OnRendererChanged(eventArgs);
            mockToolStripRenderer.Protected().Verify("InitializeContentPanel", Times.Exactly(3), control);
        }

        private class SubToolStripContentPanel : ToolStripContentPanel
        {
            public new const int ScrollStateAutoScrolling = ToolStripContentPanel.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = ToolStripContentPanel.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = ToolStripContentPanel.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = ToolStripContentPanel.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = ToolStripContentPanel.ScrollStateFullDrag;

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

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetScrollState(int bit) => base.GetScrollState(bit);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnLoad(EventArgs e) => base.OnLoad(e);

            public new void OnPaintBackground(PaintEventArgs e) => base.OnPaintBackground(e);

            public new void OnRendererChanged(EventArgs e) => base.OnRendererChanged(e);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
