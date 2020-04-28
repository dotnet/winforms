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
    using Size = System.Drawing.Size;
    using Point = System.Drawing.Point;

    public partial class ControlTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void Control_Ctor_Default()
        {
            using var control = new SubControl();
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
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
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
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
            Assert.Equal(0, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void Control_Ctor_String(string text, string expectedText)
        {
            using var control = new SubControl(text);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
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
            Assert.Equal(Size.Empty, control.ClientSize);
            Assert.Equal(Rectangle.Empty, control.ClientRectangle);
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
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(0, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Ctor_String_Int_Int_Int_Int_TestData()
        {
            yield return new object[] { null, -1, -2, -3, -4, string.Empty };
            yield return new object[] { string.Empty, 0, 0, 0, 0, string.Empty };
            yield return new object[] { "Text", 1, 2, 3, 4, "Text" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_String_Int_Int_Int_Int_TestData))]
        public void Ctor_String_Int_Int_Int_Int(string text, int left, int top, int width, int height, string expectedText)
        {
            using var control = new SubControl(text, left, top, width, height);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(top + height, control.Bottom);
            Assert.Equal(new Rectangle(left, top, width, height), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Size(width, height), control.ClientSize);
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
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(height, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(left, control.Left);
            Assert.Equal(new Point(left, top), control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal(new Size(width, height), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(left + width, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Equal(expectedText, control.Text);
            Assert.Equal(top, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(width, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Ctor_Control_String_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { new Control(), string.Empty, string.Empty };
            yield return new object[] { new Control(), "text", "text" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Control_String_TestData))]
        public void Control_Ctor_Control_String(Control parent, string text, string expectedText)
        {
            using var control = new SubControl(parent, text);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
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
            Assert.Same(parent, control.Parent);
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
            Assert.Same(expectedText, control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(0, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Ctor_Control_String_Int_Int_Int_Int_TestData()
        {
            yield return new object[] { null, null, -1, -2, -3, -4, string.Empty };
            yield return new object[] { new Control(), string.Empty, 0, 0, 0, 0, string.Empty };
            yield return new object[] { new Control(), "text", 1, 2, 3, 4, "text" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_Control_String_Int_Int_Int_Int_TestData))]
        public void Control_Ctor_Control_String_Int_Int_Int_Int(Control parent, string text, int left, int top, int width, int height, string expectedText)
        {
            using var control = new SubControl(parent, text, left, top, width, height);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(top + height, control.Bottom);
            Assert.Equal(new Rectangle(left, top, width, height), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Rectangle(0, 0, width, height), control.ClientRectangle);
            Assert.Equal(new Size(width, height), control.ClientSize);
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
            Assert.Equal(new Rectangle(0, 0, width, height), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(height, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(left, control.Left);
            Assert.Equal(new Point(left, top), control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Same(parent, control.Parent);
            Assert.Equal(new Size(width, height), control.PreferredSize);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(left + width, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(width, height), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Same(expectedText, control.Text);
            Assert.Equal(top, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(width, control.Width);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_DefaultBackColor_Get_ReturnsExpected()
        {
            Assert.Equal(SystemColors.Control, Control.DefaultBackColor);
        }

        [WinFormsFact]
        public void Control_DefaultForeColor_Get_ReturnsExpected()
        {
            Assert.Equal(SystemColors.ControlText, Control.DefaultForeColor);
        }

        [WinFormsFact]
        public void Control_DefaultFont_Get_ReturnsExpected()
        {
            Font font = Control.DefaultFont;
            Assert.Equal(SystemFonts.MessageBoxFont, Control.DefaultFont);
            Assert.Same(font, Control.DefaultFont);
        }

        [WinFormsFact]
        public void Control_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubControl();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
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
        [InlineData(true, 0x10000)]
        [InlineData(false, 0)]
        public void Control_CreateParams_GetContainerControl_ReturnsExpected(bool containerControl, int expectedExStyle)
        {
            using var control = new SubControl();
            control.SetStyle(ControlStyles.ContainerControl, containerControl);

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
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
        [InlineData(true, 0x56010000)]
        [InlineData(false, 0x5E010000)]
        public void Control_CreateParams_GetEnabled_ReturnsExpected(bool enabled, int expectedStyle)
        {
            using var control = new SubControl
            {
                Enabled = enabled
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
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

        [WinFormsFact]
        public void Control_CreateParams_GetParent_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new SubControl
            {
                Parent = parent
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
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

        [WinFormsFact]
        public void Control_CreateParams_GetParentWithHandle_ReturnsExpected()
        {
            using var parent = new Control();
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            using var control = new SubControl
            {
                Parent = parent
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(0, createParams.Height);
            Assert.Equal(parent.Handle, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56010000, createParams.Style);
            Assert.Equal(0, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void Control_CreateParams_GetTopLevel_ReturnsExpected()
        {
            using var control = new SubControl();
            control.SetTopLevel(true);

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.True(createParams.Height > 0);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x12010000, createParams.Style);
            Assert.True(createParams.Width > 0);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(RightToLeft.Inherit, 0)]
        [InlineData(RightToLeft.Yes, 0x7000)]
        [InlineData(RightToLeft.No, 0)]
        public void Control_CreateParams_GetRightToLeft_ReturnsExpected(RightToLeft rightToLeft, int expectedExStyle)
        {
            using var control = new SubControl
            {
                RightToLeft = rightToLeft
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
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
        [InlineData(true, 0x56010000)]
        [InlineData(false, 0x56000000)]
        public void Control_CreateParams_GetTabStop_ReturnsExpected(bool tabStop, int expectedStyle)
        {
            using var control = new SubControl
            {
                TabStop = tabStop
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
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
        [InlineData(true, 0x56010000)]
        [InlineData(false, 0x46010000)]
        public void Control_CreateParams_GetVisible_ReturnsExpected(bool visible, int expectedStyle)
        {
            using var control = new SubControl
            {
                Visible = visible
            };

            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
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

        private class SubControl : Control
        {
            public SubControl() : base()
            {
            }

            public SubControl(string text) : base(text)
            {
            }

            public SubControl(string text, int left, int top, int width, int height) : base(text, left, top, width, height)
            {
            }

            public SubControl(Control parent, string text) : base(parent, text)
            {
            }

            public SubControl(Control parent, string text, int left, int top, int width, int height) : base(parent, text, left, top, width, height)
            {
            }

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

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new void AccessibilityNotifyClients(AccessibleEvents accEvent, int childID) => base.AccessibilityNotifyClients(accEvent, childID);

            public new void AccessibilityNotifyClients(AccessibleEvents accEvent, int objectID, int childID) => base.AccessibilityNotifyClients(accEvent, objectID, childID);

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new ControlCollection CreateControlsInstance() => base.CreateControlsInstance();

            public new void CreateHandle() => base.CreateHandle();

            public new void DestroyHandle() => base.DestroyHandle();

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) => base.GetScaledBounds(bounds, factor, specified);

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public new void InitLayout() => base.InitLayout();

            public new void InvokeGotFocus(Control toInvoke, EventArgs e) => base.InvokeGotFocus(toInvoke, e);

            public new void InvokeLostFocus(Control toInvoke, EventArgs e) => base.InvokeLostFocus(toInvoke, e);

            public new void InvokePaint(Control c, PaintEventArgs e) => base.InvokePaint(c, e);

            public new void InvokePaintBackground(Control c, PaintEventArgs e) => base.InvokePaintBackground(c, e);

            public new bool IsInputChar(char charCode) => base.IsInputChar(charCode);

            public new bool IsInputKey(Keys keyData) => base.IsInputKey(keyData);

            public new void NotifyInvalidate(Rectangle invalidatedArea) => base.NotifyInvalidate(invalidatedArea);

            public new void OnAutoSizeChanged(EventArgs e) => base.OnAutoSizeChanged(e);

            public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);

            public new void OnBackgroundImageChanged(EventArgs e) => base.OnBackgroundImageChanged(e);

            public new void OnBackgroundImageLayoutChanged(EventArgs e) => base.OnBackgroundImageLayoutChanged(e);

            public new void OnBindingContextChanged(EventArgs e) => base.OnBindingContextChanged(e);

            public new void OnCausesValidationChanged(EventArgs e) => base.OnCausesValidationChanged(e);

            public new void OnChangeUICues(UICuesEventArgs e) => base.OnChangeUICues(e);

            public new void OnClick(EventArgs e) => base.OnClick(e);

            public new void OnClientSizeChanged(EventArgs e) => base.OnClientSizeChanged(e);

            public new void OnContextMenuStripChanged(EventArgs e) => base.OnContextMenuStripChanged(e);

            public new void OnControlAdded(ControlEventArgs e) => base.OnControlAdded(e);

            public new void OnControlRemoved(ControlEventArgs e) => base.OnControlRemoved(e);

            public new void OnCreateControl() => base.OnCreateControl();

            public new void OnCursorChanged(EventArgs e) => base.OnCursorChanged(e);

            public new void OnDockChanged(EventArgs e) => base.OnDockChanged(e);

            public new void OnDoubleClick(EventArgs e) => base.OnDoubleClick(e);

            public new void OnDpiChangedAfterParent(EventArgs e) => base.OnDpiChangedAfterParent(e);

            public new void OnDpiChangedBeforeParent(EventArgs e) => base.OnDpiChangedBeforeParent(e);

            public new void OnDragDrop(DragEventArgs e) => base.OnDragDrop(e);

            public new void OnDragEnter(DragEventArgs e) => base.OnDragEnter(e);

            public new void OnDragLeave(EventArgs e) => base.OnDragLeave(e);

            public new void OnDragOver(DragEventArgs e) => base.OnDragOver(e);

            public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnFontChanged(EventArgs e) => base.OnFontChanged(e);

            public new void OnForeColorChanged(EventArgs e) => base.OnForeColorChanged(e);

            public new void OnGiveFeedback(GiveFeedbackEventArgs e) => base.OnGiveFeedback(e);

            public new void OnGotFocus(EventArgs e) => base.OnGotFocus(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnHelpRequested(HelpEventArgs e) => base.OnHelpRequested(e);

            public new void OnImeModeChanged(EventArgs e) => base.OnImeModeChanged(e);

            public new void OnInvalidated(InvalidateEventArgs e) => base.OnInvalidated(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnLayout(LayoutEventArgs e) => base.OnLayout(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnLocationChanged(EventArgs e) => base.OnLocationChanged(e);

            public new void OnLostFocus(EventArgs e) => base.OnLostFocus(e);

            public new void OnMarginChanged(EventArgs e) => base.OnMarginChanged(e);

            public new void OnMouseCaptureChanged(EventArgs e) => base.OnMouseCaptureChanged(e);

            public new void OnMouseClick(MouseEventArgs e) => base.OnMouseClick(e);

            public new void OnMouseDoubleClick(MouseEventArgs e) => base.OnMouseDoubleClick(e);

            public new void OnMouseDown(MouseEventArgs e) => base.OnMouseDown(e);

            public new void OnMouseEnter(EventArgs e) => base.OnMouseEnter(e);

            public new void OnMouseHover(EventArgs e) => base.OnMouseHover(e);

            public new void OnMouseLeave(EventArgs e) => base.OnMouseLeave(e);

            public new void OnMouseMove(MouseEventArgs e) => base.OnMouseMove(e);

            public new void OnMouseUp(MouseEventArgs e) => base.OnMouseUp(e);

            public new void OnMouseWheel(MouseEventArgs e) => base.OnMouseWheel(e);

            public new void OnMove(EventArgs e) => base.OnMove(e);

            public new void OnPaddingChanged(EventArgs e) => base.OnPaddingChanged(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnPaintBackground(PaintEventArgs e) => base.OnPaintBackground(e);

            public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

            public new void OnParentBackColorChanged(EventArgs e) => base.OnParentBackColorChanged(e);

            public new void OnParentBackgroundImageChanged(EventArgs e) => base.OnParentBackgroundImageChanged(e);

            public new void OnParentBindingContextChanged(EventArgs e) => base.OnParentBindingContextChanged(e);

            public new void OnParentCursorChanged(EventArgs e) => base.OnParentCursorChanged(e);

            public new void OnParentEnabledChanged(EventArgs e) => base.OnParentEnabledChanged(e);

            public new void OnParentFontChanged(EventArgs e) => base.OnParentFontChanged(e);

            public new void OnParentForeColorChanged(EventArgs e) => base.OnParentForeColorChanged(e);

            public new void OnParentVisibleChanged(EventArgs e) => base.OnParentVisibleChanged(e);

            public new void OnPrint(PaintEventArgs e) => base.OnPrint(e);

            public new void OnQueryContinueDrag(QueryContinueDragEventArgs e) => base.OnQueryContinueDrag(e);

            public new void OnRegionChanged(EventArgs e) => base.OnRegionChanged(e);

            public new void OnResize(EventArgs e) => base.OnResize(e);

            public new void OnRightToLeftChanged(EventArgs e) => base.OnRightToLeftChanged(e);

            public new void OnSizeChanged(EventArgs e) => base.OnSizeChanged(e);

            public new void OnStyleChanged(EventArgs e) => base.OnStyleChanged(e);

            public new void OnSystemColorsChanged(EventArgs e) => base.OnSystemColorsChanged(e);

            public new void OnTabIndexChanged(EventArgs e) => base.OnTabIndexChanged(e);

            public new void OnTabStopChanged(EventArgs e) => base.OnTabStopChanged(e);

            public new void OnTextChanged(EventArgs e) => base.OnTextChanged(e);

            public new void OnValidated(EventArgs e) => base.OnValidated(e);

            public new void OnValidating(CancelEventArgs e) => base.OnValidating(e);

            public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

            public new bool ProcessCmdKey(ref Message msg, Keys keyData) => base.ProcessCmdKey(ref msg, keyData);

            public new bool ProcessDialogChar(char charCode) => base.ProcessDialogChar(charCode);

            public new bool ProcessDialogKey(Keys keyData) => base.ProcessDialogKey(keyData);

            public new bool ProcessKeyEventArgs(ref Message m) => base.ProcessKeyEventArgs(ref m);

            public new bool ProcessKeyMessage(ref Message m) => base.ProcessKeyMessage(ref m);

            public new bool ProcessKeyPreview(ref Message m) => base.ProcessKeyPreview(ref m);

            public new bool ProcessMnemonic(char charCode) => base.ProcessMnemonic(charCode);

            public new void RecreateHandle() => base.RecreateHandle();

            public new void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) => base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            public new void ResetMouseEventArgs() => base.ResetMouseEventArgs();

            public new HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align) => base.RtlTranslateAlignment(align);

            public new LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align) => base.RtlTranslateAlignment(align);

            public new ContentAlignment RtlTranslateAlignment(ContentAlignment align) => base.RtlTranslateAlignment(align);

            public new ContentAlignment RtlTranslateContent(ContentAlignment align) => base.RtlTranslateContent(align);

            public new HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align) => base.RtlTranslateHorizontal(align);

            public new LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align) => base.RtlTranslateLeftRight(align);

            public new void SetAutoSizeMode(AutoSizeMode mode) => base.SetAutoSizeMode(mode);

            public new void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) => base.SetBoundsCore(x, y, width, height, specified);

            public new void SetClientSizeCore(int x, int y) => base.SetClientSizeCore(x, y);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);

            public new void SetTopLevel(bool value) => base.SetTopLevel(value);

            public new Size SizeFromClientSize(Size clientSize) => base.SizeFromClientSize(clientSize);

            public new void UpdateBounds() => base.UpdateBounds();

            public new void UpdateBounds(int x, int y, int width, int height) => base.UpdateBounds(x, y, width, height);

            public new void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight) => base.UpdateBounds(x, y, width, height, clientWidth, clientHeight);

            public new void UpdateStyles() => base.UpdateStyles();

            public new void UpdateZOrder() => base.UpdateZOrder();

            public new void WndProc(ref Message m) => base.WndProc(ref m);
        }
    }
}
