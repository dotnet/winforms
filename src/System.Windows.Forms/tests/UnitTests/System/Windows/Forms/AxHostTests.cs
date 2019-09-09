// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class AxHostTests
    {
        [StaTheory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public void AxHost_Ctor_String(string clsid)
        {
            var control = new SubAxHost(clsid);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(23, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(75, 23), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.Null(control.ContainingControl);
            Assert.Null(control.ContextMenu);
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
            Assert.Equal(new Size(75, 23), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.EditMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasAboutBox);
            Assert.False(control.HasChildren);
            Assert.Equal(23, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Null(control.OcxState);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(75, control.Right);
            Assert.False(control.RightToLeft);
            Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(new Size(75, 23), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(75, control.Width);
        }

        [StaTheory]
        [InlineData("00000000-0000-0000-0000-000000000000", 0)]
        public void AxHost_Ctor_String_Int(string clsid, int flags)
        {
            var control = new SubAxHost(clsid, flags);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(23, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.Bounds);
            Assert.True(control.CanEnableIme);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(75, 23), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.Null(control.ContainingControl);
            Assert.Null(control.ContextMenu);
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
            Assert.Equal(new Size(75, 23), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 75, 23), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.EditMode);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.False(control.HasAboutBox);
            Assert.False(control.HasChildren);
            Assert.Equal(23, control.Height);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.Equal(0, control.Left);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Null(control.OcxState);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(75, control.Right);
            Assert.False(control.RightToLeft);
            Assert.Equal(RightToLeft.No, ((Control)control).RightToLeft);
            Assert.Null(control.Site);
            Assert.Equal(new Size(75, 23), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.True(control.Visible);
            Assert.Equal(75, control.Width);
        }

        [StaFact]
        public void AxHost_Ctor_NullClsid_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("g", () => new SubAxHost(null));
            Assert.Throws<ArgumentNullException>("g", () => new SubAxHost(null, 0));
        }

        [StaTheory]
        [InlineData("")]
        [InlineData("clsid")]
        public void AxHost_Ctor_EmptyClsid_ThrowsFormatException(string clsid)
        {
            Assert.Throws<FormatException>(() => new SubAxHost(clsid));
        }

        [StaFact]
        public void AxHost_CreateParams_GetDefault_ReturnsExpected()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
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
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBackColorTheoryData))]
        public void AxHost_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                BackColor = value
            };
            Assert.Equal(expected, control.BackColor);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void AxHost_BackgroundImage_Set_GetReturnsExpected(Image value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                BackgroundImage = value
            };
            Assert.Equal(value, control.BackgroundImage);

            // Set same.
            control.BackgroundImage = value;
            Assert.Equal(value, control.BackgroundImage);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void AxHost_BackgroundImageLayout_Set_GetReturnsExpected(ImageLayout value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                BackgroundImageLayout = value
            };
            Assert.Equal(value, control.BackgroundImageLayout);

            // Set same.
            control.BackgroundImageLayout = value;
            Assert.Equal(value, control.BackgroundImageLayout);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImageLayout))]
        public void AxHost_BackgroundImageLayout_SetInvalid_ThrowsInvalidEnumArgumentException(ImageLayout value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.BackgroundImageLayout = value);
        }

        [StaFact]
        public static void AxHost_ContainingControl_GetWithContainerControlGrandparent_ReturnsExpected()
        {
            var grandparent = new ContainerControl();
            var parent = new Control
            {
                Parent = grandparent
            };
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Parent = parent
            };
            Assert.Same(grandparent, control.ContainingControl);

            // Call again to check caching.
            Assert.Same(control.ContainingControl, control.ContainingControl);

            // Remove from chain.
            parent.Parent = null;
            Assert.Same(grandparent, control.ContainingControl);
        }

        [StaFact]
        public static void AxHost_ContainingControl_GetWithContainerControlParent_ReturnsExpected()
        {
            var parent = new ContainerControl();
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Parent = parent
            };
            Assert.Same(parent, control.ContainingControl);

            // Call again to check caching.
            Assert.Same(parent, control.ContainingControl);

            // Remove from chain.
            control.Parent = null;
            Assert.Same(parent, control.ContainingControl);
        }

        [StaFact]
        public static void AxHost_ContainingControl_GetWithNonContainerControlParent_ReturnsExpected()
        {
            var parent = new Control();
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Parent = parent
            };
            Assert.Null(control.ContainingControl);

            // Call again to check caching.
            Assert.Null(control.ContainingControl);

            // Modify.
            var grandparent = new ContainerControl();
            parent.Parent = grandparent;
            Assert.Same(grandparent, control.ContainingControl);

            // Call again to check caching.
            Assert.Same(grandparent, control.ContainingControl);

            // Remove from chain.
            parent.Parent = null;
            Assert.Same(grandparent, control.ContainingControl);
        }

        public static IEnumerable<object[]> ContainingControl_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContainerControl() };
        }

        [StaTheory]
        [MemberData(nameof(ContainingControl_Set_TestData))]
        public void AxHost_ContainingControl_Set_GetReturnsExpected(ContainerControl value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                ContainingControl = value
            };
            Assert.Same(value, control.ContainingControl);

            // Set same.
            control.ContainingControl = value;
            Assert.Same(value, control.ContainingControl);
        }

        public static IEnumerable<object[]> ContextMenu_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContextMenu() };
        }

        [StaTheory]
        [MemberData(nameof(ContextMenu_Set_TestData))]
        public void AxHost_ContextMenu_Set_GetReturnsExpected(ContextMenu value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                ContextMenu = value
            };
            Assert.Same(value, control.ContextMenu);

            // Set same.
            control.ContextMenu = value;
            Assert.Same(value, control.ContextMenu);
        }

        [StaTheory]
        [MemberData(nameof(ContextMenu_Set_TestData))]
        public void AxHost_ContextMenu_SetWithNonNullOldValue_GetReturnsExpected(ContextMenu value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                ContextMenu = new ContextMenu()
            };
            control.ContextMenu = value;
            Assert.Same(value, control.ContextMenu);

            // Set same.
            control.ContextMenu = value;
            Assert.Same(value, control.ContextMenu);
        }

        [StaFact]
        public void AxHost_ContextMenu_SetDisposeNew_RemovesContextMenu()
        {
            var menu = new ContextMenu();
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                ContextMenu = menu
            };
            Assert.Same(menu, control.ContextMenu);

            menu.Dispose();
            Assert.Null(control.ContextMenu);
        }

        [StaFact]
        public void AxHost_ContextMenu_SetDisposeOld_RemovesContextMenu()
        {
            var menu1 = new ContextMenu();
            var menu2 = new ContextMenu();
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                ContextMenu = menu1
            };
            Assert.Same(menu1, control.ContextMenu);

            control.ContextMenu = menu2;
            Assert.Same(menu2, control.ContextMenu);

            menu1.Dispose();
            Assert.Same(menu2, control.ContextMenu);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void AxHost_Cursor_Set_GetReturnsExpected(Cursor value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Cursor = value
            };
            Assert.Same(value ?? Cursors.Default, control.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void AxHost_Cursor_SetWithHandle_GetReturnsExpected(Cursor value)
        {
            var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void AxHost_Cursor_SetWithChildren_GetReturnsExpected(Cursor value)
        {
            var child1 = new Control();
            var child2 = new Control();
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(value ?? Cursors.Default, child1.Cursor);
            Assert.Same(value ?? Cursors.Default, child2.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(value ?? Cursors.Default, child1.Cursor);
            Assert.Same(value ?? Cursors.Default, child2.Cursor);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetCursorTheoryData))]
        public void AxHost_Cursor_SetWithChildrenWithCursor_GetReturnsExpected(Cursor value)
        {
            var cursor1 = new Cursor((IntPtr)1);
            var cursor2 = new Cursor((IntPtr)1);
            var child1 = new Control
            {
                Cursor = cursor1
            };
            var child2 = new Control
            {
                Cursor = cursor2
            };
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            control.Controls.Add(child1);
            control.Controls.Add(child2);

            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(cursor1, child1.Cursor);
            Assert.Same(cursor2, child2.Cursor);

            // Set same.
            control.Cursor = value;
            Assert.Same(value ?? Cursors.Default, control.Cursor);
            Assert.Same(cursor1, child1.Cursor);
            Assert.Same(cursor2, child2.Cursor);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void AxHost_Font_Set_GetReturnsExpected(Font value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void AxHost_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void AxHost_ImeMode_Set_GetReturnsExpected(ImeMode value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                ImeMode = value
            };
            Assert.Equal(value, control.ImeMode);

            // Set same.
            control.ImeMode = value;
            Assert.Equal(value, control.ImeMode);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void AxHost_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void AxHost_Enabled_Set_GetReturnsExpected(bool value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Enabled = value
            };
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = !value;
            Assert.Equal(!value, control.Enabled);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AxHost_Text_Set_GetReturnsExpected(string value)
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000")
            {
                Text = value
            };
            Assert.Equal(value, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(value, control.Text);
        }

        [StaTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AxHost_Text_SetWithHandle_GetReturnsExpected(string value)
        {
            var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2");
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Text = value;
            Assert.Equal(value, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(value, control.Text);
        }

        [StaFact]
        public void AxHost_BackColorChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackColorChanged += handler);
            control.BackColorChanged -= handler;
        }

        [StaFact]
        public void AxHost_BackgroundImageChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackgroundImageChanged += handler);
            control.BackgroundImageChanged -= handler;
        }

        [StaFact]
        public void AxHost_BackgroundImageLayoutChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BackgroundImageLayoutChanged += handler);
            control.BackgroundImageLayoutChanged -= handler;
        }

        [StaFact]
        public void AxHost_BindingContextChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.BindingContextChanged += handler);
            control.BindingContextChanged -= handler;
        }

        [StaFact]
        public void AxHost_Click_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Click += handler);
            control.Click -= handler;
        }

        [StaFact]
        public void AxHost_ContextMenuChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ContextMenuChanged += handler);
            control.ContextMenuChanged -= handler;
        }

        [StaFact]
        public void AxHost_CursorChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.CursorChanged += handler);
            control.CursorChanged -= handler;
        }

        [StaFact]
        public void AxHost_DoubleClick_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DoubleClick += handler);
            control.DoubleClick -= handler;
        }

        [StaFact]
        public void AxHost_DragDrop_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragDrop += handler);
            control.DragDrop -= handler;
        }

        [StaFact]
        public void AxHost_DragEnter_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            DragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.DragEnter += handler);
            control.DragEnter -= handler;
        }

        [StaFact]
        public void AxHost_EnabledChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.EnabledChanged += handler);
            control.EnabledChanged -= handler;
        }

        [StaFact]
        public void AxHostFontChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.FontChanged += handler);
            control.FontChanged -= handler;
        }

        [StaFact]
        public void AxHost_ForeColorChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ForeColorChanged += handler);
            control.ForeColorChanged -= handler;
        }

        [StaFact]
        public void AxHost_GiveFeedback_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            GiveFeedbackEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.GiveFeedback += handler);
            control.GiveFeedback -= handler;
        }

        [StaFact]
        public void AxHost_HelpRequested_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            HelpEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.HelpRequested += handler);
            control.HelpRequested -= handler;
        }

        [StaFact]
        public void AxHost_ImeModeChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.ImeModeChanged += handler);
            control.ImeModeChanged -= handler;
        }

        [StaFact]
        public void AxHost_KeyDown_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            KeyEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyDown += handler);
            control.KeyDown -= handler;
        }

        [StaFact]
        public void AxHost_KeyPress_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            KeyPressEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyPress += handler);
            control.KeyPress -= handler;
        }

        [StaFact]
        public void AxHost_KeyUp_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            KeyEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.KeyUp += handler);
            control.KeyUp -= handler;
        }

        [StaFact]
        public void AxHost_Layout_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            LayoutEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Layout += handler);
            control.Layout -= handler;
        }

        [StaFact]
        public void AxHost_MouseClick_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseClick += handler);
            control.MouseClick -= handler;
        }

        [StaFact]
        public void AxHost_MouseDoubleClick_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseDoubleClick += handler);
            control.MouseDoubleClick -= handler;
        }

        [StaFact]
        public void AxHost_MouseDown_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            MouseEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseDown += handler);
            control.MouseDown -= handler;
        }

        [StaFact]
        public void AxHost_MouseEnter_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseEnter += handler);
            control.MouseEnter -= handler;
        }

        [StaFact]
        public void AxHost_MouseHover_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseHover += handler);
            control.MouseHover -= handler;
        }

        [StaFact]
        public void AxHost_MouseLeave_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.MouseLeave += handler);
            control.MouseLeave -= handler;
        }

        [StaFact]
        public void AxHost_Paint_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            PaintEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.Paint += handler);
            control.Paint -= handler;
        }

        [StaFact]
        public void AxHost_QueryAccessibilityHelp__AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            QueryAccessibilityHelpEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.QueryAccessibilityHelp += handler);
            control.QueryAccessibilityHelp -= handler;
        }

        [StaFact]
        public void AxHost_QueryContinueDrag__AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            QueryContinueDragEventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.QueryContinueDrag += handler);
            control.QueryContinueDrag -= handler;
        }

        [StaFact]
        public void AxHost_RightToLeftChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.RightToLeftChanged += handler);
            control.RightToLeftChanged -= handler;
        }

        [StaFact]
        public void AxHost_TextChanged_AddRemove_ThrowsNotSupportedException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            EventHandler handler = (sender, e) => { };
            Assert.Throws<NotSupportedException>(() => control.TextChanged += handler);
            control.TextChanged -= handler;
        }

        [StaFact]
        public void AxHost_AttachInterfaces_Invoke_Nop()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            control.AttachInterfaces();
            control.AttachInterfaces();
        }

        [StaFact]
        public void AxHost_BeginInit_Invoke_Nop()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            control.BeginInit();
            control.BeginInit();
            Assert.False(control.Created);
        }

        [StaFact]
        public void AxHost_EndInit_Invoke_Nop()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            control.EndInit();
            control.EndInit();
            Assert.False(control.Created);
        }

        [StaFact]
        public void AxHost_EndInit_InvokeWithParent_CreatesControl()
        {
            var parent = new Control();
            var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2")
            {
                Parent = parent
            };
            control.EndInit();
            control.EndInit();
            Assert.True(parent.Created);
            Assert.True(control.Created);
        }

        [ComImport]
        [Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E")]
        public interface IWebBrowser2
        {
        }

        [StaFact]
        public void AxHost_CreateControl_ValidClsid_Success()
        {
            var control = new SubAxHost("8856f961-340a-11d0-a96b-00c04fd705a2");
            control.CreateControl();
            Assert.True(control.Created);
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            object ocx = control.GetOcx();
            Assert.True(ocx is IWebBrowser2);
        }

        [StaFact]
        public void AxHost_CreateControl_InvalidClsid_ThrowsCOMException()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            Assert.Throws<COMException>(() => control.CreateControl());
        }

        [StaFact]
        public void AxHost_CreateSink_Invoke_Nop()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            control.CreateSink();
            control.CreateSink();
            Assert.False(control.Created);
        }

        [StaFact]
        public void AxHost_DetachSink_Invoke_Nop()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            control.DetachSink();
            control.DetachSink();
            Assert.False(control.Created);
        }

        [StaFact]
        public void AxHost_GetOcx_NotCreated_ReturnsNull()
        {
            var control = new SubAxHost("00000000-0000-0000-0000-000000000000");
            Assert.Null(control.GetOcx());
        }

        private class SubAxHost : AxHost
        {
            public SubAxHost(string clsid) : base(clsid)
            {
            }

            public SubAxHost(string clsid, int flags) : base(clsid, flags)
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

            public new void AttachInterfaces() => base.AttachInterfaces();

            public new void CreateSink() => base.CreateSink();

            public new void DetachSink() => base.DetachSink();
        }
    }
}
