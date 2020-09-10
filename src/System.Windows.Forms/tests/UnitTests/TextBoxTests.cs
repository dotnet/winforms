// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class TextBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        private static int s_preferredHeight = Control.DefaultFont.Height + SystemInformation.BorderSize.Height * 4 + 3;

        [WinFormsFact]
        public void TextBox_Ctor_Default()
        {
            using var control = new SubTextBox();
            Assert.False(control.AcceptsReturn);
            Assert.False(control.AcceptsTab);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.Empty(control.AutoCompleteCustomSource);
            Assert.Same(control.AutoCompleteCustomSource, control.AutoCompleteCustomSource);
            Assert.Equal(AutoCompleteMode.None, control.AutoCompleteMode);
            Assert.Equal(AutoCompleteSource.None, control.AutoCompleteSource);
            Assert.True(control.AutoSize);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(control.PreferredHeight, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, control.PreferredHeight), control.Bounds);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.True(control.CanSelect);
            Assert.False(control.CanUndo);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(CharacterCasing.Normal, control.CharacterCasing);
            Assert.Equal(new Size(96, control.PreferredHeight - 4), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 96, control.PreferredHeight - 4), control.ClientRectangle);
            Assert.Null(control.Container);
            Assert.False(control.ContainsFocus);
            Assert.Null(control.ContextMenuStrip);
            Assert.Empty(control.Controls);
            Assert.Same(control.Controls, control.Controls);
            Assert.False(control.Created);
            Assert.Same(Cursors.IBeam, control.Cursor);
            Assert.Same(Cursors.IBeam, control.DefaultCursor);
            Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
            Assert.Equal(new Padding(3), control.DefaultMargin);
            Assert.Equal(Size.Empty, control.DefaultMaximumSize);
            Assert.Equal(Size.Empty, control.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, control.DefaultPadding);
            Assert.Equal(new Size(100, control.PreferredHeight), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 96, control.PreferredHeight - 4), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.True(control.Enabled);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(control.PreferredHeight, control.Height);
            Assert.True(control.HideSelection);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Empty(control.Lines);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(32767, control.MaxLength);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(control.Modified);
            Assert.False(control.Multiline);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.Equal(4, control.PreferredSize.Width);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.True(control.PreferredHeight > 0);
            Assert.False(control.ReadOnly);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(100, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(ScrollBars.None, control.ScrollBars);
            Assert.Empty(control.SelectedText);
            Assert.Equal(0, control.SelectionLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.ShortcutsEnabled);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(100, control.PreferredHeight), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(HorizontalAlignment.Left, control.TextAlign);
            Assert.Equal(0, control.TextLength);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.False(control.UseSystemPasswordChar);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(100, control.Width);
            Assert.True(control.WordWrap);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubTextBox();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("Edit", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(control.PreferredHeight, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x560100C0, createParams.Style);
            Assert.Equal(100, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_CanEnableIme_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.True(control.CanEnableIme);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.True(control.CanEnableIme);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_CanEnableIme_GetWithHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.True(control.CanEnableIme);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Get again.
            Assert.True(control.CanEnableIme);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TextBox_ImeMode_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_ImeMode_GetWithHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Get again.
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TextBox_ImeModeBase_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_ImeModeBase_GetWithHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Get again.
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TextBox_PasswordChar_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.Equal('\0', control.PasswordChar);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal('\0', control.PasswordChar);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_PasswordChar_GetWithHandle_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal('\0', control.PasswordChar);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Get again.
            Assert.Equal('\0', control.PasswordChar);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TextBox_PlaceholderText()
        {
            using var tb = new TextBox
            {
                PlaceholderText = "Enter your name"
            };
            Assert.False(string.IsNullOrEmpty(tb.PlaceholderText));
        }

        [WinFormsFact]
        public void TextBox_PlaceholderText_DefaultValue()
        {
            using var tb = new TextBox();
            Assert.Equal(string.Empty, tb.PlaceholderText);
        }

        [WinFormsFact]
        public void TextBox_PlaceholderText_When_InAccessibility_Doesnot_Raise_TextChanged()
        {
            using var tb = new SubTextBox();
            bool eventRaised = false;
            EventHandler handler = (o, e) => eventRaised = true;
            tb.TextChanged += handler;
            tb.CreateAccessibilityInstance();
            Assert.False(eventRaised);
            tb.TextChanged -= handler;
        }

        public static IEnumerable<object[]> TextBox_ShouldRenderPlaceHolderText_TestData()
        {
            // Test PlaceholderText
            var tb = new SubTextBox() { PlaceholderText = "", IsUserPaint = false, IsFocused = false, TextCount = 0 };
            var msg = new Message() { Msg = (int)User32.WM.PAINT };
            yield return new object[] { tb, msg, false };

            // Test PlaceholderText
            tb = new SubTextBox() { PlaceholderText = null, IsUserPaint = false, IsFocused = false, TextCount = 0 };
            msg = new Message() { Msg = (int)User32.WM.PAINT };
            yield return new object[] { tb, msg, false };

            // Test Message
            msg.Msg = (int)User32.WM.USER;
            tb = new SubTextBox() { PlaceholderText = "Text", IsUserPaint = false, IsFocused = false, TextCount = 0 };
            yield return new object[] { tb, msg, false };

            // Test UserPaint
            msg.Msg = (int)User32.WM.PAINT;
            tb = new SubTextBox() { PlaceholderText = "Text", IsUserPaint = true, IsFocused = false, TextCount = 0 };
            yield return new object[] { tb, msg, false };

            // Test Focused
            msg.Msg = (int)User32.WM.PAINT;
            tb = new SubTextBox() { PlaceholderText = "Text", IsUserPaint = false, IsFocused = true, TextCount = 0 };
            yield return new object[] { tb, msg, false };

            // Test TextLength
            msg.Msg = (int)User32.WM.PAINT;
            tb = new SubTextBox() { PlaceholderText = "Text", IsUserPaint = false, IsFocused = false, TextCount = 1 };
            yield return new object[] { tb, msg, false };

            // Test WM_PAINT
            tb = new SubTextBox() { PlaceholderText = "Text", IsUserPaint = false, IsFocused = false, TextCount = 0 };
            msg.Msg = (int)User32.WM.PAINT;
            yield return new object[] { tb, msg, true };

            // Test WM_KILLFOCUS
            tb = new SubTextBox() { PlaceholderText = "Text", IsUserPaint = false, IsFocused = false, TextCount = 0 };
            msg.Msg = (int)User32.WM.KILLFOCUS;
            yield return new object[] { tb, msg, true };
        }

        [WinFormsTheory]
        [MemberData(nameof(TextBox_ShouldRenderPlaceHolderText_TestData))]
        public void TextBox_ShouldRenderPlaceHolderText(TextBox textBox, Message m, bool expected)
        {
            bool result = textBox.TestAccessor().Dynamic.ShouldRenderPlaceHolderText(m);
            Assert.Equal(expected, result);
        }

        [WinFormsFact]
        public void TextBox_PlaceholderText_NullValue_CoercedTo_StringEmpty()
        {
            using var tb = new TextBox()
            {
                PlaceholderText = "Text"
            };

            tb.PlaceholderText = null;
            Assert.Equal(string.Empty, tb.PlaceholderText);
        }

        [WinFormsFact]
        public void TextBox_PlaceholderText_Overriden()
        {
            using var tb = new SubTextBox();

            Assert.NotNull(tb);
        }

        [WinFormsFact]
        public void TextBox_PlaceholderTextAlignments()
        {
            using var tb = new TextBox
            {
                PlaceholderText = "Enter your name"
            };

            HandleRef refHandle = new HandleRef(tb, tb.Handle);

            //Cover the Placeholder draw code path
            User32.SendMessageW(refHandle, User32.WM.PAINT, PARAM.FromBool(false));
            tb.TextAlign = HorizontalAlignment.Center;
            User32.SendMessageW(refHandle, User32.WM.PAINT, PARAM.FromBool(false));
            tb.TextAlign = HorizontalAlignment.Right;
            User32.SendMessageW(refHandle, User32.WM.PAINT, PARAM.FromBool(false));

            Assert.False(string.IsNullOrEmpty(tb.PlaceholderText));
        }

        [WinFormsFact]
        public void TextBox_PlaceholderTextAlignmentsInRightToLeft()
        {
            using var tb = new TextBox
            {
                PlaceholderText = "Enter your name",
                RightToLeft = RightToLeft.Yes
            };

            HandleRef refHandle = new HandleRef(tb, tb.Handle);

            //Cover the Placeholder draw code path in RightToLeft scenario
            User32.SendMessageW(refHandle, User32.WM.PAINT, PARAM.FromBool(false));
            tb.TextAlign = HorizontalAlignment.Center;
            User32.SendMessageW(refHandle, User32.WM.PAINT, PARAM.FromBool(false));
            tb.TextAlign = HorizontalAlignment.Right;
            User32.SendMessageW(refHandle, User32.WM.PAINT, PARAM.FromBool(false));

            Assert.False(string.IsNullOrEmpty(tb.PlaceholderText));
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Text)]
        [InlineData(false, AccessibleRole.None)]
        public void TextBox_CreateAccessibilityInstance_Invoke_ReturnsExpected(bool createControl, AccessibleRole expectedAccessibleRole)
        {
            using var control = new SubTextBox();
            if (createControl)
            {
                control.CreateControl();
            }

            Assert.Equal(createControl, control.IsHandleCreated);
            Control.ControlAccessibleObject instance = Assert.IsType<TextBoxBase.TextBoxBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.Equal(createControl, control.IsHandleCreated);
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(expectedAccessibleRole, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.Equal(createControl, control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_CreateAccessibilityInstance_InvokeWithCustomRole_ReturnsExpected()
        {
            using var control = new SubTextBox
            {
                AccessibleRole = AccessibleRole.HelpBalloon
            };
            Control.ControlAccessibleObject instance = Assert.IsType<TextBoxBase.TextBoxBaseAccessibleObject>(control.CreateAccessibilityInstance());
            Assert.NotNull(instance);
            Assert.Same(control, instance.Owner);
            Assert.Equal(AccessibleRole.HelpBalloon, instance.Role);
            Assert.NotSame(control.CreateAccessibilityInstance(), instance);
            Assert.NotSame(control.AccessibilityObject, instance);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBox_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, true)]
        [InlineData(ControlStyles.StandardClick, false)]
        [InlineData(ControlStyles.Selectable, true)]
        [InlineData(ControlStyles.UserMouse, false)]
        [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
        [InlineData(ControlStyles.StandardDoubleClick, false)]
        [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
        [InlineData(ControlStyles.CacheText, false)]
        [InlineData(ControlStyles.EnableNotifyMessage, false)]
        [InlineData(ControlStyles.DoubleBuffer, false)]
        [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
        [InlineData(ControlStyles.UseTextForAccessibility, false)]
        [InlineData((ControlStyles)0, true)]
        [InlineData((ControlStyles)int.MaxValue, false)]
        [InlineData((ControlStyles)(-1), false)]
        public void TextBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubTextBox();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsFact]
        public void TextBox_GetTopLevel_Invoke_ReturnsExpected()
        {
            using var control = new SubTextBox();
            Assert.False(control.GetTopLevel());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TextBox_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubTextBox();
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
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TextBox_OnHandleCreated_InvokeWithHandle_CallsHandleCreated(EventArgs eventArgs)
        {
            using var control = new SubTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
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
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.True(control.IsHandleCreated);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void TextBox_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            using var control = new SubTextBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> OnHandleDestroyed_TestData()
        {
            foreach (bool modified in new bool[] { true, false })
            {
                yield return new object[] { modified, null };
                yield return new object[] { modified, new EventArgs() };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(OnHandleDestroyed_TestData))]
        public void TextBox_OnHandleDestroyed_InvokeWithHandle_CallsHandleDestroyed(bool modified, EventArgs eventArgs)
        {
            using var control = new SubTextBox
            {
                Text = "Text",
                SelectionStart = 1,
                SelectionLength = 2,
                Modified = modified
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Equal(modified, control.Modified);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(s_preferredHeight, control.Height);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Equal(modified, control.Modified);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class SubTextBox : TextBox
        {
            public int TextCount;
            public bool IsFocused;
            public bool TextAccessed;

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

            public new bool IsHandleCreated => base.IsHandleCreated;

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public override string PlaceholderText { get => base.PlaceholderText; set => base.PlaceholderText = value; }

            public new AccessibleObject CreateAccessibilityInstance() => base.CreateAccessibilityInstance();

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new bool GetTopLevel() => base.GetTopLevel();

            public override bool Focused => IsFocused;
            public override int TextLength => TextCount;

            public override string Text
            {
                get
                {
                    TextAccessed = true;
                    return base.Text;
                }
                set => base.Text = value;
            }

            public bool IsUserPaint
            {
                get => GetStyle(ControlStyles.UserPaint);
                set => SetStyle(ControlStyles.UserPaint, value);
            }

            public new void CreateControl() => base.CreateControl();

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);
        }
    }
}
