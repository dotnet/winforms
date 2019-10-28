// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class FlowLayoutPanelTests
    {
        [WinFormsFact]
        public void FlowLayoutPanel_Ctor_Default()
        {
            using var control = new SubFlowLayoutPanel();
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
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.False(control.Created);
            Assert.Null(control.Container);
            Assert.Null(control.ContextMenu);
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
            Assert.Null(control.Site);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.False(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.NotNull(control.VerticalScroll);
            Assert.Same(control.VerticalScroll, control.VerticalScroll);
            Assert.True(control.Visible);
            Assert.False(control.VScroll);
            Assert.Equal(200, control.Width);
            Assert.True(control.WrapContents);

            Assert.False(control.IsHandleCreated);
        }

        [Fact]
        public void FlowLayoutPanel_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubFlowLayoutPanel();
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
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlowDirection))]
        public void FlowLayoutPanel_FlowDirection_Set_GetReturnsExpected(FlowDirection value)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection = value
            };
            Assert.Equal(value, panel.FlowDirection);

            // Set same
            panel.FlowDirection = value;
            Assert.Equal(value, panel.FlowDirection);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(FlowDirection))]
        public void FlowLayoutPanel_FlowDirection_SetInvalidValue_ThrowsInvalidEnumArgumentException(FlowDirection value)
        {
            var panel = new FlowLayoutPanel();
            Assert.Throws<InvalidEnumArgumentException>("value", () => panel.FlowDirection = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FlowLayoutPanel_WrapContents_Set_GetReturnsExpected(bool value)
        {
            var panel = new FlowLayoutPanel
            {
                WrapContents = value
            };
            Assert.Equal(value, panel.WrapContents);

            // Set same
            panel.WrapContents = value;
            Assert.Equal(value, panel.WrapContents);

            // Set different
            panel.WrapContents = !value;
            Assert.Equal(!value, panel.WrapContents);
        }

        [Fact]
        public void CanExtend_InvokeWithParent_ReturnsTrue()
        {
            var panel = new FlowLayoutPanel();
            var control = new Control
            {
                Parent = panel
            };
            IExtenderProvider extenderProvider = panel;
            Assert.True(extenderProvider.CanExtend(control));
        }

        public static IEnumerable<object[]> CanExtend_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new Control() };
            yield return new object[] { new Control { Parent = new Control() } };
        }

        [Theory]
        [MemberData(nameof(CanExtend_TestData))]
        public void CanExtend_InvokeNoParent_ReturnsFalse(object extendee)
        {
            var panel = new FlowLayoutPanel();
            IExtenderProvider extenderProvider = panel;
            Assert.False(extenderProvider.CanExtend(extendee));
        }

        [Fact]
        public void GetFlowBreak_ValidControl_ReturnsExpected()
        {
            var panel = new FlowLayoutPanel();
            Assert.False(panel.GetFlowBreak(new Control()));
        }

        [Fact]
        public void GetFlowBreak_NullControl_ThrowsArgumentNullException()
        {
            var panel = new FlowLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.GetFlowBreak(null));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SetFlowBreak_Invoke_GetFlowBreakReturnsExpected(bool value)
        {
            var panel = new FlowLayoutPanel();
            var control = new Control();
            panel.SetFlowBreak(control, value);
            Assert.Equal(value, panel.GetFlowBreak(control));

            // Set same.
            panel.SetFlowBreak(control, value);
            Assert.Equal(value, panel.GetFlowBreak(control));

            // Set different.
            panel.SetFlowBreak(control, !value);
            Assert.Equal(!value, panel.GetFlowBreak(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SetFlowBreak_NullControl_ThrowsArgumentNullException(bool value)
        {
            var panel = new FlowLayoutPanel();
            Assert.Throws<ArgumentNullException>("control", () => panel.SetFlowBreak(null, value));
        }

        private class SubFlowLayoutPanel : FlowLayoutPanel
        {
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

            public new bool VScroll
            {
                get => base.VScroll;
                set => base.VScroll = value;
            }
        }
    }
}
