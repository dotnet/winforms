
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
        [Fact]
        public void FlowLayoutPanel_Ctor_Default()
        {
            var panel = new SubFlowLayoutPanel();
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, panel.Anchor);
            Assert.False(panel.AutoScroll);
            Assert.Equal(Size.Empty, panel.AutoScrollMargin);
            Assert.Equal(Size.Empty, panel.AutoScrollMinSize);
            Assert.Equal(Point.Empty, panel.AutoScrollPosition);
            Assert.False(panel.AutoSize);
            Assert.Equal(AutoSizeMode.GrowOnly, panel.AutoSizeMode);
            Assert.Equal(Control.DefaultBackColor, panel.BackColor);
            Assert.Null(panel.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, panel.BackgroundImageLayout);
            Assert.Null(panel.BindingContext);
            Assert.Equal(BorderStyle.None, panel.BorderStyle);
            Assert.Equal(100, panel.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), panel.Bounds);
            Assert.True(panel.CanRaiseEvents);
            Assert.True(panel.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, 200, 100), panel.ClientRectangle);
            Assert.Equal(new Size(200, 100), panel.ClientSize);
            Assert.False(panel.Created);
            Assert.Null(panel.Container);
            Assert.Empty(panel.Controls);
            Assert.Same(panel.Controls, panel.Controls);
            Assert.Same(Cursors.Default, panel.Cursor);
            Assert.Same(Cursors.Default, panel.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, panel.DefaultImeMode);
            Assert.Equal(new Padding(3), panel.DefaultMargin);
            Assert.Equal(Size.Empty, panel.DefaultMaximumSize);
            Assert.Equal(Size.Empty, panel.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, panel.DefaultPadding);
            Assert.Equal(new Size(200, 100), panel.DefaultSize);
            Assert.False(panel.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 200, 100), panel.DisplayRectangle);
            Assert.Equal(DockStyle.None, panel.Dock);
            Assert.NotNull(panel.DockPadding);
            Assert.Same(panel.DockPadding, panel.DockPadding);
            Assert.Equal(0, panel.DockPadding.Top);
            Assert.Equal(0, panel.DockPadding.Bottom);
            Assert.Equal(0, panel.DockPadding.Left);
            Assert.Equal(0, panel.DockPadding.Right);
            Assert.True(panel.Enabled);
            Assert.NotNull(panel.Events);
            Assert.Same(panel.Events, panel.Events);
            Assert.Equal(FlowDirection.LeftToRight, panel.FlowDirection);
            Assert.Equal(Control.DefaultFont, panel.Font);
            Assert.Equal(Control.DefaultForeColor, panel.ForeColor);
            Assert.False(panel.HasChildren);
            Assert.Equal(100, panel.Height);
            Assert.NotNull(panel.HorizontalScroll);
            Assert.Same(panel.HorizontalScroll, panel.HorizontalScroll);
            Assert.False(panel.HScroll);
            Assert.Equal(ImeMode.NoControl, panel.ImeMode);
            Assert.Equal(ImeMode.NoControl, panel.ImeModeBase);
            Assert.NotNull(panel.LayoutEngine);
            Assert.Same(panel.LayoutEngine, panel.LayoutEngine);
            Assert.Equal(0, panel.Left);
            Assert.Equal(Point.Empty, panel.Location);
            Assert.Equal(new Padding(3), panel.Margin);
            Assert.Equal(Padding.Empty, panel.Padding);
            Assert.Equal(200, panel.Right);
            Assert.Equal(RightToLeft.No, panel.RightToLeft);
            Assert.Null(panel.Site);
            Assert.Equal(new Size(200, 100), panel.Size);
            Assert.Equal(0, panel.TabIndex);
            Assert.False(panel.TabStop);
            Assert.Empty(panel.Text);
            Assert.Equal(0, panel.Top);
            Assert.NotNull(panel.VerticalScroll);
            Assert.Same(panel.VerticalScroll, panel.VerticalScroll);
            Assert.True(panel.Visible);
            Assert.False(panel.VScroll);
            Assert.Equal(200, panel.Width);
            Assert.True(panel.WrapContents);
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
        }
    }
}
