﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class ButtonBaseTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ButtonBase_Ctor_Default()
        {
            using var control = new SubButtonBase();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoEllipsis);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(23, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.Bounds);
            Assert.False(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(75, 23), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.Default, control.Cursor);
            Assert.Same(Cursors.Default, control.DefaultCursor);
            Assert.Equal(ImeMode.Disable, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(75, 23), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.True(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.NotNull(control.FlatAppearance);
            Assert.Same(control.FlatAppearance, control.FlatAppearance);
            Assert.Equal(FlatStyle.Standard, control.FlatStyle);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(23, control.Height);
            Assert.Null(control.Image);
            Assert.Equal(ContentAlignment.MiddleCenter, control.ImageAlign);
            Assert.Equal(-1, control.ImageIndex);
            Assert.Empty(control.ImageKey);
            Assert.Null(control.ImageList);
            Assert.Equal(ImeMode.Disable, control.ImeMode);
            Assert.Equal(ImeMode.Disable, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsDefault);
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
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            // This causes an assertion which we want to keep internally.
            using (new NoAssertContext())
            {
                Assert.Throws<NullReferenceException>(() => control.PreferredSize);
            }
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.True(control.ResizeRedraw);
            Assert.Equal(75, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(75, 23), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(ContentAlignment.MiddleCenter, control.TextAlign);
            Assert.Equal(TextImageRelation.Overlay, control.TextImageRelation);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.True(control.UseMnemonic);
            Assert.True(control.UseCompatibleTextRendering);
            Assert.True(control.UseVisualStyleBackColor);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(75, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ButtonBase_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubButtonBase();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(23, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(75, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(TextImageRelation))]
        public void ToolStripItem_TextImageRelation_Set_GetReturnsExpected(TextImageRelation value)
        {
            var button = new SubButtonBase
            {
                TextImageRelation = value
            };
            Assert.Equal(value, button.TextImageRelation);

            // Set same.
            button.TextImageRelation = value;
            Assert.Equal(value, button.TextImageRelation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(TextImageRelation))]
        [InlineData((TextImageRelation)3)]
        [InlineData((TextImageRelation)5)]
        [InlineData((TextImageRelation)6)]
        [InlineData((TextImageRelation)7)]
        public void ButtonBase_TextImageRelation_SetInvalid_ThrowsInvalidEnumArgumentException(TextImageRelation value)
        {
            var button = new SubButtonBase();
            Assert.Throws<InvalidEnumArgumentException>("value", () => button.TextImageRelation = value);
        }

        [WinFormsFact]
        public void ButtonBase_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubButtonBase();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, true)]
        [InlineData(ControlStyles.Opaque, true)]
        [InlineData(ControlStyles.ResizeRedraw, true)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
        [InlineData(ControlStyles.StandardClick, true)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, true)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, true)]
        [InlineData(ControlStyles.StandardDoubleClick, true)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, true)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, true)]
        [InlineData(ControlStyles.UseTextForAccessibility, true)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void ButtonBase_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubButtonBase();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        public class SubButtonBase : ButtonBase
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

            public new bool IsDefault
            {
                get => base.IsDefault;
                set => base.IsDefault = value;
            }

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);
        }
    }
}
