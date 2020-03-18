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
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class FlowLayoutPanelTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void FlowLayoutPanel_Ctor_Default()
        {
            using var control = new SubFlowLayoutPanel();
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
            Assert.False(control.Created);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
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
            Assert.Equal(FlowDirection.LeftToRight, control.FlowDirection);
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
            Assert.False(control.ResizeRedraw);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.True(control.Visible);
            Assert.False(control.VScroll);
            Assert.Equal(200, control.Width);
            Assert.True(control.WrapContents);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void FlowLayoutPanel_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubFlowLayoutPanel();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x10000, createParams.ExStyle);
            Assert.Equal(100, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56000000, createParams.Style);
            Assert.Equal(200, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(FlowDirection.BottomUp, 1)]
        [InlineData(FlowDirection.LeftToRight, 1)]
        [InlineData(FlowDirection.RightToLeft, 1)]
        [InlineData(FlowDirection.TopDown, 1)]
        public void FlowLayoutPanel_FlowDirection_Set_GetReturnsExpected(FlowDirection value, int expectedLayoutCallCount)
        {
            using var control = new FlowLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("FlowDirection", e.AffectedProperty);
                layoutCallCount++;
            };

            control.FlowDirection = value;
            Assert.Equal(value, control.FlowDirection);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same
            control.FlowDirection = value;
            Assert.Equal(value, control.FlowDirection);
            Assert.Equal(expectedLayoutCallCount * 2, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(FlowDirection))]
        public void FlowLayoutPanel_FlowDirection_SetInvalidValue_ThrowsInvalidEnumArgumentException(FlowDirection value)
        {
            using var control = new FlowLayoutPanel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.FlowDirection = value);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 1)]
        public void FlowLayoutPanel_WrapContents_Set_GetReturnsExpected(bool value, int expectedLayoutCallCount)
        {
            using var control = new FlowLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("WrapContents", e.AffectedProperty);
                layoutCallCount++;
            };

            control.WrapContents = value;
            Assert.Equal(value, control.WrapContents);
            Assert.Equal(expectedLayoutCallCount, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same
            control.WrapContents = value;
            Assert.Equal(value, control.WrapContents);
            Assert.Equal(expectedLayoutCallCount + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set different
            control.WrapContents = !value;
            Assert.Equal(!value, control.WrapContents);
            Assert.Equal(expectedLayoutCallCount + 1 + 1, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void FlowLayoutPanel_CanExtend_InvokeWithParent_ReturnsTrue()
        {
            using var control = new FlowLayoutPanel();
            using var extendee = new Control
            {
                Parent = control
            };
            IExtenderProvider extenderProvider = control;
            Assert.True(extenderProvider.CanExtend(extendee));
        }

        public static IEnumerable<object[]> CanExtend_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new Control() };
            yield return new object[] { new Control { Parent = new Control() } };
        }

        [WinFormsTheory]
        [MemberData(nameof(CanExtend_TestData))]
        public void FlowLayoutPanel_CanExtend_InvokeNoParent_ReturnsFalse(object extendee)
        {
            using var control = new FlowLayoutPanel();
            IExtenderProvider extenderProvider = control;
            Assert.False(extenderProvider.CanExtend(extendee));
        }

        [WinFormsFact]
        public void FlowLayoutPanel_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubFlowLayoutPanel();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsFact]
        public void FlowLayoutPanel_GetFlowBreak_InvokeValidControl_ReturnsExpected()
        {
            using var child = new Control();
            using var control = new FlowLayoutPanel();
            Assert.False(control.GetFlowBreak(child));
        }

        [WinFormsFact]
        public void FlowLayoutPanel_GetFlowBreak_NullControl_ThrowsArgumentNullException()
        {
            using var control = new FlowLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.GetFlowBreak(null));
        }

        [WinFormsTheory]
        [InlineData(0, true)]
        [InlineData(SubFlowLayoutPanel.ScrollStateAutoScrolling, false)]
        [InlineData(SubFlowLayoutPanel.ScrollStateFullDrag, false)]
        [InlineData(SubFlowLayoutPanel.ScrollStateHScrollVisible, false)]
        [InlineData(SubFlowLayoutPanel.ScrollStateUserHasScrolled, false)]
        [InlineData(SubFlowLayoutPanel.ScrollStateVScrollVisible, false)]
        [InlineData(int.MaxValue, false)]
        [InlineData((-1), false)]
        public void FlowLayoutPanel_GetScrollState_Invoke_ReturnsExpected(int bit, bool expected)
        {
            using var control = new SubFlowLayoutPanel();
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
        public void FlowLayoutPanel_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubFlowLayoutPanel();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void FlowLayoutPanel_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubFlowLayoutPanel();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FlowLayoutPanel_SetFlowBreak_Invoke_GetFlowBreakReturnsExpected(bool value)
        {
            using var child = new Control();
            using var control = new FlowLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;

            control.SetFlowBreak(child, value);
            Assert.Equal(value, control.GetFlowBreak(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set same.
            control.SetFlowBreak(child, value);
            Assert.Equal(value, control.GetFlowBreak(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);

            // Set different.
            control.SetFlowBreak(child, !value);
            Assert.Equal(!value, control.GetFlowBreak(child));
            Assert.Equal(0, layoutCallCount);
            Assert.Equal(0, childLayoutCallCount);
            Assert.False(control.IsHandleCreated);
            Assert.False(child.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void FlowLayoutPanel_SetFlowBreak_InvokeControlWithParent_GetFlowBreakReturnsExpected(bool value, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var child = new Control
            {
                Parent = parent
            };
            using var control = new FlowLayoutPanel();
            int layoutCallCount = 0;
            control.Layout += (sender, e) => layoutCallCount++;
            int childLayoutCallCount = 0;
            child.Layout += (sender, e) => childLayoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs eventArgs)
            {
                Assert.Same(parent, sender);
                Assert.Same(child, eventArgs.AffectedControl);
                Assert.Equal("FlowBreak", eventArgs.AffectedProperty);
                parentLayoutCallCount++;
            }
            parent.Layout += parentHandler;

            try
            {
                control.SetFlowBreak(child, value);
                Assert.Equal(value, control.GetFlowBreak(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set same.
                control.SetFlowBreak(child, value);
                Assert.Equal(value, control.GetFlowBreak(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);

                // Set different.
                control.SetFlowBreak(child, !value);
                Assert.Equal(!value, control.GetFlowBreak(child));
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(0, childLayoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount + 1, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
                Assert.False(child.IsHandleCreated);
                Assert.False(parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FlowLayoutPanel_SetFlowBreak_NullControl_ThrowsArgumentNullException(bool value)
        {
            using var control = new FlowLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => control.SetFlowBreak(null, value));
        }

        private class SubFlowLayoutPanel : FlowLayoutPanel
        {
            public new const int ScrollStateAutoScrolling = FlowLayoutPanel.ScrollStateAutoScrolling;

            public new const int ScrollStateHScrollVisible = FlowLayoutPanel.ScrollStateHScrollVisible;

            public new const int ScrollStateVScrollVisible = FlowLayoutPanel.ScrollStateVScrollVisible;

            public new const int ScrollStateUserHasScrolled = FlowLayoutPanel.ScrollStateUserHasScrolled;

            public new const int ScrollStateFullDrag = FlowLayoutPanel.ScrollStateFullDrag;

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
        }
    }
}
