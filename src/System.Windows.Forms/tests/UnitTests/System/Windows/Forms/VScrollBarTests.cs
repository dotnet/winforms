// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class VScrollBarTests
    {
        [Fact]
        public void VScrollBar_Ctor_Default()
        {
            var scrollBar = new SubVScrollBar();
            Assert.False(scrollBar.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, scrollBar.Anchor);
            Assert.False(scrollBar.AutoSize);
            Assert.Equal(Control.DefaultBackColor, scrollBar.BackColor);
            Assert.Null(scrollBar.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, scrollBar.BackgroundImageLayout);
            Assert.Null(scrollBar.BindingContext);
            Assert.Equal(0, scrollBar.Bounds.X);
            Assert.Equal(0, scrollBar.Bounds.Y);
            Assert.True(scrollBar.Bounds.Width > 0);
            Assert.True(scrollBar.Bounds.Height > 0);
            Assert.True(scrollBar.Bottom > 0);
            Assert.False(scrollBar.CanEnableIme);
            Assert.True(scrollBar.CanRaiseEvents);
            Assert.True(scrollBar.CausesValidation);
            Assert.Equal(0, scrollBar.ClientRectangle.X);
            Assert.Equal(0, scrollBar.ClientRectangle.Y);
            Assert.True(scrollBar.ClientRectangle.Width > 0);
            Assert.True(scrollBar.ClientRectangle.Height > 0);
            Assert.True(scrollBar.ClientSize.Width > 0);
            Assert.True(scrollBar.ClientSize.Height > 0);
            Assert.Null(scrollBar.Container);
            Assert.Empty(scrollBar.Controls);
            Assert.Same(scrollBar.Controls, scrollBar.Controls);
            Assert.False(scrollBar.Created);
            Assert.Same(Cursors.Default, scrollBar.Cursor);
            Assert.Same(Cursors.Default, scrollBar.DefaultCursor);
            Assert.Equal(ImeMode.Disable, scrollBar.DefaultImeMode);
            Assert.Equal(Padding.Empty, scrollBar.DefaultMargin);
            Assert.Equal(Size.Empty, scrollBar.DefaultMaximumSize);
            Assert.Equal(Size.Empty, scrollBar.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, scrollBar.DefaultPadding);
            Assert.True(scrollBar.DefaultSize.Width > 0);
            Assert.True(scrollBar.DefaultSize.Height > 0);
            Assert.False(scrollBar.DesignMode);
            Assert.Equal(0, scrollBar.DisplayRectangle.X);
            Assert.Equal(0, scrollBar.DisplayRectangle.Y);
            Assert.True(scrollBar.DisplayRectangle.Width > 0);
            Assert.True(scrollBar.DisplayRectangle.Height > 0);
            Assert.Equal(DockStyle.None, scrollBar.Dock);
            Assert.True(scrollBar.Enabled);
            Assert.NotNull(scrollBar.Events);
            Assert.Same(scrollBar.Events, scrollBar.Events);
            Assert.Equal(Control.DefaultFont, scrollBar.Font);
            Assert.Equal(Control.DefaultForeColor, scrollBar.ForeColor);
            Assert.False(scrollBar.HasChildren);
            Assert.True(scrollBar.Height > 0);
            Assert.Equal(ImeMode.Disable, scrollBar.ImeMode);
            Assert.Equal(ImeMode.Disable, scrollBar.ImeModeBase);
            Assert.Equal(10, scrollBar.LargeChange);
            Assert.Equal(0, scrollBar.Left);
            Assert.Equal(Point.Empty, scrollBar.Location);
            Assert.Equal(Padding.Empty, scrollBar.Margin);
            Assert.Equal(100, scrollBar.Maximum);
            Assert.Equal(0, scrollBar.Minimum);
            Assert.Equal(Padding.Empty, scrollBar.Padding);
            Assert.True(scrollBar.Right > 0);
            Assert.Equal(RightToLeft.No, scrollBar.RightToLeft);
            Assert.True(scrollBar.ScaleScrollBarForDpiChange);
            Assert.Null(scrollBar.Site);
            Assert.True(scrollBar.Size.Width > 0);
            Assert.True(scrollBar.Size.Height > 0);
            Assert.Equal(1, scrollBar.SmallChange);
            Assert.Equal(0, scrollBar.TabIndex);
            Assert.False(scrollBar.TabStop);
            Assert.Empty(scrollBar.Text);
            Assert.Equal(0, scrollBar.Top);
            Assert.Equal(0, scrollBar.Value);
            Assert.True(scrollBar.Visible);
            Assert.True(scrollBar.Width > 0);
        }

        [Fact]
        public void VScrollBar_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubVScrollBar();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("SCROLLBAR", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.True(createParams.Height > 0);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56000001, createParams.Style);
            Assert.True(createParams.Width > 0);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(RightToLeft))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void VScrollBar_RightToLeft_Set_GetReturnsNo(RightToLeft value)
        {
            var control = new SubVScrollBar
            {
                RightToLeft = value
            };
            Assert.Equal(RightToLeft.No, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
        }

        [Fact]
        public void VScrollBar_RightToLeft_SetWithHandler_DoesNotCallRightToLeftChanged()
        {
            var control = new SubVScrollBar();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RightToLeftChanged += handler;

            // Set different.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, callCount);

            // Set same.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, callCount);

            // Set different.
            control.RightToLeft = RightToLeft.Inherit;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(0, callCount);
        }

        private class SubVScrollBar : VScrollBar
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

            public new EventHandlerList Events => base.Events;

            public new ImeMode ImeModeBase => base.ImeModeBase;
        }
    }
}
