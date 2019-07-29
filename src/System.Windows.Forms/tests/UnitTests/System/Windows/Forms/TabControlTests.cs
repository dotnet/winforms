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
    public class TabControlTests
    {
        [Fact]
        public void TabControl_Ctor_Default()
        {
            var control = new SubTabControl();
            Assert.Equal(TabAlignment.Top, control.Alignment);
            Assert.False(control.AllowDrop);
            Assert.Equal(TabAppearance.Normal, control.Appearance);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(100, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.Bounds);
            Assert.True(control.CanRaiseEvents);
            Assert.Equal(new Rectangle(0, 0, 200, 100), control.ClientRectangle);
            Assert.Equal(new Size(200, 100), control.ClientSize);
            Assert.Null(control.Container);
            Assert.True(control.CausesValidation);
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
            Assert.Equal(new Size(200, 100), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.Equal(TabDrawMode.Normal, control.DrawMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(100, control.Height);
            Assert.False(control.HotTrack);
            Assert.Null(control.ImageList);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(Size.Empty, control.ItemSize);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.False(control.Multiline);
            Assert.Equal(new Point(6, 3), control.Padding);
            Assert.Equal(200, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.False(control.RightToLeftLayout);
            Assert.Equal(-1, control.SelectedIndex);
            Assert.Null(control.SelectedTab);
            Assert.False(control.ShowToolTips);
            Assert.Null(control.Site);
            Assert.Equal(new Size(200, 100), control.Size);
            Assert.Equal(TabSizeMode.Normal, control.SizeMode);
            Assert.Equal(0, control.TabCount);
            Assert.Equal(0, control.TabIndex);
            Assert.Empty(control.TabPages);
            Assert.Same(control.TabPages, control.TabPages);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(200, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [InlineData(TabAlignment.Top, false)]
        [InlineData(TabAlignment.Bottom, false)]
        [InlineData(TabAlignment.Left, true)]
        [InlineData(TabAlignment.Right, true)]
        public void TabControl_Alignment_Set_GetReturnsExpected(TabAlignment value, bool expectedMultiline)
        {
            var control = new TabControl
            {
                Alignment = value
            };
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);

            // Set same.
            control.Alignment = value;
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);
        }

        [Theory]
        [InlineData(TabAlignment.Bottom, false)]
        [InlineData(TabAlignment.Left, true)]
        [InlineData(TabAlignment.Right, true)]
        public void TabControl_Alignment_SetWithHandle_GetReturnsExpected(TabAlignment value, bool expectedMultiline)
        {
            var control = new TabControl();
            IntPtr oldHandle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, oldHandle);

            control.Alignment = value;
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);
            IntPtr newHandle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, newHandle);
            Assert.NotEqual(oldHandle, newHandle);

            // Set same.
            control.Alignment = value;
            Assert.Equal(value, control.Alignment);
            Assert.Equal(expectedMultiline, control.Multiline);
            Assert.Equal(newHandle, control.Handle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TabAlignment))]
        public void TabControl_Alignment_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabAlignment value)
        {
            var control = new TabControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Alignment = value);
        }

        [Theory]
        [InlineData(TabAlignment.Bottom)]
        [InlineData(TabAlignment.Left)]
        [InlineData(TabAlignment.Right)]
        public void TabControl_Appearance_GetFlatButtonsWithAlignment_ReturnsExpected(TabAlignment alignment)
        {
            var control = new TabControl
            {
                Appearance = TabAppearance.FlatButtons,
                Alignment = alignment
            };
            Assert.Equal(TabAppearance.Buttons, control.Appearance);
            Assert.Equal(alignment, control.Alignment);

            control.Alignment = TabAlignment.Top;
            Assert.Equal(TabAppearance.FlatButtons, control.Appearance);
            Assert.Equal(TabAlignment.Top, control.Alignment);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TabAppearance))]
        public void TabControl_Appearance_Set_GetReturnsExpected(TabAppearance value)
        {
            var control = new TabControl
            {
                Appearance = value
            };
            Assert.Equal(value, control.Appearance);

            // Set same.
            control.Appearance = value;
            Assert.Equal(value, control.Appearance);
        }

        [Theory]
        [InlineData(TabAppearance.Buttons)]
        [InlineData(TabAppearance.FlatButtons)]
        public void TabControl_Appearance_SetWithHandle_GetReturnsExpected(TabAppearance value)
        {
            var control = new TabControl();
            IntPtr oldHandle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, oldHandle);

            control.Appearance = value;
            Assert.Equal(value, control.Appearance);
            IntPtr newHandle = control.Handle;
            Assert.NotEqual(IntPtr.Zero, newHandle);
            Assert.NotEqual(oldHandle, newHandle);

            // Set same.
            control.Appearance = value;
            Assert.Equal(value, control.Appearance);
            Assert.Equal(newHandle, control.Handle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TabAppearance))]
        public void TabControl_Appearance_SetInvalidValue_ThrowsInvalidEnumArgumentException(TabAppearance value)
        {
            var control = new TabControl();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.Appearance = value);
        }

        [Fact]
        public void TabControl_DisplayRectangle_Get_ReturnsExpectedAndCreatesHandle()
        {
            var control = new TabControl();
            Rectangle displayRectangle = control.DisplayRectangle;
            Assert.True(displayRectangle.X >= 0);
            Assert.True(displayRectangle.Y >= 0);
            Assert.Equal(200 - displayRectangle.X * 2, control.DisplayRectangle.Width);
            Assert.Equal(100 - displayRectangle.Y * 2, control.DisplayRectangle.Height);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(displayRectangle, control.DisplayRectangle);
        }

        [Fact]
        public void TabControl_RowCount_Get_ReturnsExpectedAndCreatesHandle()
        {
            var control = new TabControl();
            Assert.Equal(0, control.RowCount);
            Assert.True(control.IsHandleCreated);
        }

        [Fact]
        public void TabControl_SelectedIndex_GetWithHandle_ReturnsExpected()
        {
            var control = new TabControl();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(-1, control.SelectedIndex);
        }

        public class SubTabControl : TabControl
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

            public new bool DoubleBuffered => base.DoubleBuffered;

            public new EventHandlerList Events => base.Events;

            public new ImeMode ImeModeBase => base.ImeModeBase;
        }
    }
}
