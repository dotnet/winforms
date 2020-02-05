// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;
using static Interop;
using static Interop.Richedit;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class RichTextBoxTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void RichTextBox_Ctor_Default()
        {
            using var control = new SubRichTextBox();
            Assert.False(control.AcceptsTab);
            Assert.Null(control.AccessibleDefaultActionDescription);
            Assert.Null(control.AccessibleDescription);
            Assert.Null(control.AccessibleName);
            Assert.Equal(AccessibleRole.Default, control.AccessibleRole);
            Assert.False(control.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
            Assert.False(control.AutoSize);
            Assert.False(control.AutoWordSelection);
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Null(control.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
            Assert.Null(control.BindingContext);
            Assert.Equal(BorderStyle.Fixed3D, control.BorderStyle);
            Assert.Equal(96, control.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, 96), control.Bounds);
            Assert.Equal(0, control.BulletIndent);
            Assert.True(control.CanEnableIme);
            Assert.False(control.CanFocus);
            Assert.True(control.CanRaiseEvents);
            Assert.False(control.CanRedo);
            Assert.True(control.CanSelect);
            Assert.False(control.CanUndo);
            Assert.False(control.Capture);
            Assert.True(control.CausesValidation);
            Assert.Equal(new Size(96, 92), control.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 96, 92), control.ClientRectangle);
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
            Assert.Equal(new Size(100, 96), control.DefaultSize);
            Assert.False(control.DesignMode);
            Assert.True(control.DetectUrls);
            Assert.Equal(new Rectangle(0, 0, 96, 92), control.DisplayRectangle);
            Assert.Equal(DockStyle.None, control.Dock);
            Assert.False(control.DoubleBuffered);
            Assert.False(control.EnableAutoDragDrop);
            Assert.True(control.Enabled);
            Assert.False(control.EnableAutoDragDrop);
            Assert.NotNull(control.Events);
            Assert.Same(control.Events, control.Events);
            Assert.False(control.Focused);
            Assert.Equal(Control.DefaultFont, control.Font);
            Assert.Equal(control.Font.Height, control.FontHeight);
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.False(control.HasChildren);
            Assert.Equal(96, control.Height);
            Assert.True(control.HideSelection);
            Assert.Equal(ImeMode.NoControl, control.ImeMode);
            Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
            Assert.False(control.IsAccessible);
            Assert.False(control.IsMirrored);
            Assert.Equal(RichTextBoxLanguageOptions.AutoFont | RichTextBoxLanguageOptions.DualFont, control.LanguageOption);
            Assert.NotNull(control.LayoutEngine);
            Assert.Same(control.LayoutEngine, control.LayoutEngine);
            Assert.Equal(0, control.Left);
            Assert.Empty(control.Lines);
            Assert.Equal(Point.Empty, control.Location);
            Assert.Equal(new Padding(3), control.Margin);
            Assert.Equal(Size.Empty, control.MaximumSize);
            Assert.Equal(int.MaxValue, control.MaxLength);
            Assert.Equal(Size.Empty, control.MinimumSize);
            Assert.False(control.Modified);
            Assert.True(control.Multiline);
            Assert.Equal(Padding.Empty, control.Padding);
            Assert.Null(control.Parent);
            Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
            Assert.True(control.PreferredSize.Width > 0);
            Assert.True(control.PreferredSize.Height > 0);
            Assert.Equal(control.PreferredSize.Height, control.PreferredHeight);
            Assert.Empty(control.RedoActionName);
            Assert.False(control.ReadOnly);
            Assert.False(control.RecreatingHandle);
            Assert.Null(control.Region);
            Assert.False(control.ResizeRedraw);
            Assert.Equal(0, control.RightMargin);
            Assert.True(control.RichTextShortcutsEnabled);
            Assert.Equal(100, control.Right);
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Null(control.Rtf);
            Assert.Equal(RichTextBoxScrollBars.Both, control.ScrollBars);
            Assert.Equal(Color.Empty, control.SelectionBackColor);
            Assert.False(control.ShowSelectionMargin);
            Assert.True(control.ShowFocusCues);
            Assert.True(control.ShowKeyboardCues);
            Assert.Null(control.Site);
            Assert.Equal(new Size(100, 96), control.Size);
            Assert.Equal(0, control.TabIndex);
            Assert.True(control.TabStop);
            Assert.Empty(control.Text);
            Assert.Equal(0, control.Top);
            Assert.Null(control.TopLevelControl);
            Assert.Empty(control.UndoActionName);
            Assert.False(control.UseWaitCursor);
            Assert.True(control.Visible);
            Assert.Equal(100, control.Width);
            Assert.True(control.WordWrap);
            Assert.Equal(1.0f, control.ZoomFactor);

            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_CreateParams_GetDefault_ReturnsExpected()
        {
            using var control = new SubRichTextBox();
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("RICHEDIT50W", createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(0x200, createParams.ExStyle);
            Assert.Equal(96, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x56210044, createParams.Style);
            Assert.Equal(100, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_AutoWordSelection_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.False(control.AutoWordSelection);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call EM_SETOPTIONS.
            User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_SETOPTIONS, (IntPtr)RichTextBoxConstants.ECOOP_OR, (IntPtr)RichTextBoxConstants.ECO_AUTOWORDSELECTION);
            Assert.False(control.AutoWordSelection);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RichTextBox_AutoWordSelection_Set_GetReturnsExpected(bool value)
        {
            using var control = new RichTextBox
            {
                AutoWordSelection = value
            };
            Assert.Equal(value, control.AutoWordSelection);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.AutoWordSelection = value;
            Assert.Equal(value, control.AutoWordSelection);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.AutoWordSelection = !value;
            Assert.Equal(!value, control.AutoWordSelection);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RichTextBox_AutoWordSelection_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.AutoWordSelection = value;
            Assert.Equal(value, control.AutoWordSelection);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.AutoWordSelection = value;
            Assert.Equal(value, control.AutoWordSelection);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.AutoWordSelection = !value;
            Assert.Equal(!value, control.AutoWordSelection);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, 65)]
        [InlineData(false, 64)]
        public void RichTextBox_AutoWordSelection_GetOptions_Success(bool value, int expected)
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.AutoWordSelection = value;
            Assert.Equal((IntPtr)expected, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETOPTIONS));
        }

        public static IEnumerable<object[]> BackColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.Window };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_Set_TestData))]
        public void RichTextBox_BackColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new RichTextBox
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

        public static IEnumerable<object[]> BackColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, Control.DefaultBackColor, 0 };
            yield return new object[] { Color.Red, Color.Red, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_SetWithHandle_TestData))]
        public void Control_BackColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.BackColor = value;
            Assert.Equal(expected, control.BackColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_BackColor_SetWithHandler_CallsBackColorChanged()
        {
            using var control = new RichTextBox();
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
            Assert.Equal(SystemColors.Window, control.BackColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.BackColor = Color.Red;
            Assert.Equal(Color.Red, control.BackColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void RichTextBox_CanRedo_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.False(control.CanRedo);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> CanRedo_CustomCanRedo_TestData()
        {
            yield return new object[] { IntPtr.Zero, false };
            yield return new object[] { (IntPtr)1, true };
        }

        [WinFormsTheory]
        [MemberData(nameof(CanRedo_CustomCanRedo_TestData))]
        public void RichTextBox_CanRedo_CustomCanRedo_ReturnsExpected(IntPtr result, bool expected)
        {
            using var control = new CustomCanRedoRichTextBox
            {
                Result = result
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.CanRedo);
        }

        private class CustomCanRedoRichTextBox : RichTextBox
        {
            public IntPtr Result { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == RichEditMessages.EM_CANREDO)
                {
                    m.Result = Result;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void RichTextBox_DetectUrls_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.True(control.DetectUrls);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call EM_AUTOURLDETECT.
            User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_AUTOURLDETECT, IntPtr.Zero);
            Assert.True(control.DetectUrls);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RichTextBox_DetectUrls_Set_GetReturnsExpected(bool value)
        {
            using var control = new RichTextBox
            {
                DetectUrls = value
            };
            Assert.Equal(value, control.DetectUrls);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.DetectUrls = value;
            Assert.Equal(value, control.DetectUrls);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.DetectUrls = !value;
            Assert.Equal(!value, control.DetectUrls);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public void RichTextBox_DetectUrls_SetWithHandle_GetReturnsExpected(bool value, int expectedCreatedCallCount)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.DetectUrls = value;
            Assert.Equal(value, control.DetectUrls);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.DetectUrls = value;
            Assert.Equal(value, control.DetectUrls);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set different.
            control.DetectUrls = !value;
            Assert.Equal(!value, control.DetectUrls);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount + 1, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void RichTextBox_DetectUrls_GetAutoUrlDetect_Success(bool value, int expected)
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.DetectUrls = value;
            Assert.Equal((IntPtr)expected, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETAUTOURLDETECT));
        }

        public static IEnumerable<object[]> ForeColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3) };
            yield return new object[] { Color.White, Color.White };
            yield return new object[] { Color.Black, Color.Black };
            yield return new object[] { Color.Red, Color.Red };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_Set_TestData))]
        public void RichTextBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new RichTextBox
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.False(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> ForeColor_SetWithHandle_TestData()
        {
            yield return new object[] { Color.Empty, SystemColors.WindowText, 0 };
            yield return new object[] { Color.FromArgb(254, 1, 2, 3), Color.FromArgb(254, 1, 2, 3), 1 };
            yield return new object[] { Color.White, Color.White, 1 };
            yield return new object[] { Color.Black, Color.Black, 1 };
            yield return new object[] { Color.Red, Color.Red, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_SetWithHandle_TestData))]
        public void RichTextBox_ForeColor_SetWithHandle_GetReturnsExpected(Color value, Color expected, int expectedInvalidatedCallCount)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public unsafe void RichTextBox_ForeColor_GetCharFormat_Success()
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
            var format = new CHARFORMATW
            {
                cbSize = (uint)sizeof(CHARFORMATW)
            };
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETCHARFORMAT, (IntPtr)SCF.ALL, ref format));
            Assert.Equal(0x785634, format.crTextColor);
        }

        [WinFormsFact]
        public unsafe void RichTextBox_ForeColor_GetCharFormatWithTextColor_Success()
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.ForeColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
            var format = new CHARFORMATW
            {
                cbSize = (uint)sizeof(CHARFORMATW)
            };
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETCHARFORMAT, (IntPtr)SCF.ALL, ref format));
            Assert.Equal(0x785634, format.crTextColor);

            // Set different.
            control.ForeColor = Color.FromArgb(0x34, 0x56, 0x78, 0x90);
            format = new CHARFORMATW
            {
                cbSize = (uint)sizeof(CHARFORMATW)
            };
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETCHARFORMAT, (IntPtr)SCF.ALL, ref format));
            Assert.Equal(0x907856, format.crTextColor);
        }

        [WinFormsFact]
        public void RichTextBox_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            using var control = new RichTextBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ForeColorChanged += handler;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(SystemColors.WindowText, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void RichTextBox_MaxLength_GetWithHandle_ReturnsExpecte()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0x7FFFFFFF, control.MaxLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call EM_LIMITTEXT.
            User32.SendMessageW(control.Handle, (User32.WM)User32.EM.LIMITTEXT, IntPtr.Zero, (IntPtr)1);
            Assert.Equal(0x7FFFFFFF, control.MaxLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call EM_EXLIMITTEXT.
            User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_EXLIMITTEXT, IntPtr.Zero, (IntPtr)2);
            Assert.Equal(0x7FFFFFFF, control.MaxLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64000)]
        [InlineData(0x7FFFFFFE)]
        [InlineData(int.MaxValue)]
        public void RichTextBox_MaxLength_Set_GetReturnsExpected(int value)
        {
            using var control = new RichTextBox
            {
                MaxLength = value
            };
            Assert.Equal(value, control.MaxLength);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.MaxLength = value;
            Assert.Equal(value, control.MaxLength);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_MaxLength_SetWithLongText_Success()
        {
            using var control = new RichTextBox
            {
                Text = "Text",
                MaxLength = 2
            };
            Assert.Equal(2, control.MaxLength);
            Assert.Equal("Text", control.Text);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64000)]
        [InlineData(0x7FFFFFFE)]
        [InlineData(int.MaxValue)]
        public void RichTextBox_MaxLength_SetWithHandle_GetReturnsExpected(int value)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.MaxLength = value;
            Assert.Equal(value, control.MaxLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.MaxLength = value;
            Assert.Equal(value, control.MaxLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_MaxLength_SetWithLongTextWithHandle_Success()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = "Text";
            control.MaxLength = 2;
            Assert.Equal(2, control.MaxLength);
            Assert.Equal("Text", control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0, 0x10000)]
        [InlineData(1, 1)]
        [InlineData(64000, 64000)]
        [InlineData(0x7FFFFFFE, 0x7FFFFFFE)]
        [InlineData(int.MaxValue, 0x7FFFFFFF)]
        public void RichTextBox_MaxLength_GetLimitText_Success(int value, int expected)
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.MaxLength = value;
            Assert.Equal((IntPtr)expected, User32.SendMessageW(control.Handle, (User32.WM)User32.EM.GETLIMITTEXT));
        }

        [WinFormsFact]
        public void RichTextBox_MaxLength_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            using var control = new RichTextBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.MaxLength = -1);
        }

        [WinFormsFact]
        public void RichTextBox_RedoActionName_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Empty(control.RedoActionName);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> RedoActionName_CustomGetRedoName_TestData()
        {
            yield return new object[] { IntPtr.Zero, IntPtr.Zero, string.Empty };
            yield return new object[] { (IntPtr)1, IntPtr.Zero, "Unknown" };
            yield return new object[] { (IntPtr)1, (IntPtr)1, "Typing" };
            yield return new object[] { (IntPtr)1, (IntPtr)2, "Delete" };
            yield return new object[] { (IntPtr)1, (IntPtr)3, "Drag and Drop" };
            yield return new object[] { (IntPtr)1, (IntPtr)4, "Cut" };
            yield return new object[] { (IntPtr)1, (IntPtr)5, "Paste" };
            yield return new object[] { (IntPtr)1, (IntPtr)6, "Unknown" };
            yield return new object[] { (IntPtr)1, (IntPtr)7, "Unknown" };
        }

        [WinFormsTheory]
        [MemberData(nameof(RedoActionName_CustomGetRedoName_TestData))]
        public void RichTextBox_RedoActionName_CustomGetRedoName_ReturnsExpected(IntPtr canRedoResult, IntPtr getRedoNameResult, string expected)
        {
            using var control = new CustomGetRedoNameRichTextBox
            {
                CanRedoResult = canRedoResult,
                GetRedoNameResult = getRedoNameResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.RedoActionName);
        }

        private class CustomGetRedoNameRichTextBox : RichTextBox
        {
            public IntPtr CanRedoResult { get; set; }
            public IntPtr GetRedoNameResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == RichEditMessages.EM_CANREDO)
                {
                    m.Result = CanRedoResult;
                    return;
                }
                else if (m.Msg == RichEditMessages.EM_GETREDONAME)
                {
                    m.Result = GetRedoNameResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void RichTextBox_RightMargin_GetWithHandle_ReturnsExpecte()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.RightMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64000)]
        [InlineData(0x7FFFFFFE)]
        [InlineData(int.MaxValue)]
        public void RichTextBox_RightMargin_Set_GetReturnsExpected(int value)
        {
            using var control = new RichTextBox
            {
                RightMargin = value
            };
            Assert.Equal(value, control.RightMargin);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RightMargin = value;
            Assert.Equal(value, control.RightMargin);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64000)]
        [InlineData(0x7FFFFFFE)]
        [InlineData(int.MaxValue)]
        public void RichTextBox_RightMargin_SetWithCustomOldValue_GetReturnsExpected(int value)
        {
            using var control = new RichTextBox
            {
                RightMargin = 1
            };

            control.RightMargin = value;
            Assert.Equal(value, control.RightMargin);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.RightMargin = value;
            Assert.Equal(value, control.RightMargin);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(64000)]
        [InlineData(0x7FFFFFFE)]
        [InlineData(int.MaxValue)]
        public void RichTextBox_RightMargin_SetWithHandle_GetReturnsExpected(int value)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.RightMargin = value;
            Assert.Equal(value, control.RightMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.RightMargin = value;
            Assert.Equal(value, control.RightMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(64000, 0)]
        [InlineData(0x7FFFFFFE, 0)]
        [InlineData(int.MaxValue, 0)]
        public void RichTextBox_RightMargin_SetWithCustomOldValueWithHandle_GetReturnsExpected(int value,  int expectedCreatedCallCount)
        {
            using var control = new RichTextBox
            {
                RightMargin = 1
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.RightMargin = value;
            Assert.Equal(value, control.RightMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);

            // Set same.
            control.RightMargin = value;
            Assert.Equal(value, control.RightMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(expectedCreatedCallCount, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_RightMargin_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            using var control = new RichTextBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.RightMargin = -1);
        }

        [WinFormsFact]
        public void RichTextBox_SelectedText_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectedText_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Get again.
            Assert.Empty(control.SelectedText);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectedRtf_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotNull(control.SelectedRtf);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.NotNull(control.SelectedRtf);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectedRtf_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.NotNull(control.SelectedRtf);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.NotNull(control.SelectedRtf);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionAlignment_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionAlignment_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(HorizontalAlignment.Left, control.SelectionAlignment);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionBullet_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.False(control.SelectionBullet);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.False(control.SelectionBullet);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionBullet_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.False(control.SelectionBullet);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.False(control.SelectionBullet);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionCharOffset_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(0, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionCharOffset_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(0, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SelectionCharOffset_CustomGetCharFormat_TestData()
        {
            yield return new object[] { CFM.OFFSET, 0, 0 };
            yield return new object[] { CFM.OFFSET, 900, 60 };
            yield return new object[] { CFM.OFFSET, 30000, 2000 };
            yield return new object[] { CFM.OFFSET, 60000, 4000 };
            yield return new object[] { CFM.OFFSET, -900, -60 };

            yield return new object[] { 0, 0, 0 };
            yield return new object[] { 0, 900, 60 };
            yield return new object[] { 0, 30000, 2000 };
            yield return new object[] { 0, 60000, 4000 };
            yield return new object[] { 0, -900, -60 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionCharOffset_CustomGetCharFormat_TestData))]
        public void RichTextBox_SelectionCharOffset_CustomGetCharFormat_Success(uint mask, int yOffset, int expected)
        {
            using var control = new CustomGetCharFormatRichTextBox
            {
                ExpectedWParam = (IntPtr)SCF.SELECTION,
                GetCharFormatResult = new CHARFORMATW
                {
                    dwMask = (CFM)mask,
                    yOffset = yOffset
                }
            };

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.MakeCustom = true;
            Assert.Equal(expected, control.SelectionCharOffset);
        }

        [WinFormsTheory]
        [InlineData(2000)]
        [InlineData(10)]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(-2000)]
        public void RichTextBox_SelectionCharOffset_Set_GetReturnsExpected(int value)
        {
            using var control = new RichTextBox
            {
                SelectionCharOffset = value
            };
            Assert.Equal(value, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.SelectionCharOffset = value;
            Assert.Equal(value, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(2000)]
        [InlineData(10)]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(-2000)]
        public void RichTextBox_SelectionCharOffset_SetWithHandle_GetReturnsExpected(int value)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionCharOffset = value;
            Assert.Equal(value, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectionCharOffset = value;
            Assert.Equal(value, control.SelectionCharOffset);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public unsafe void RichTextBox_SelectionCharOffset_GetCharFormat_Success()
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.SelectionCharOffset = 60;
            var format = new CHARFORMATW
            {
                cbSize = (uint)sizeof(CHARFORMATW)
            };
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETCHARFORMAT, (IntPtr)SCF.SELECTION, ref format));
            Assert.Equal(900, format.yOffset);
        }

        [WinFormsTheory]
        [InlineData(2001)]
        [InlineData(-20001)]
        public void RichTextBox_SelectionCharOffset_SetInvalidValue_ThrowsArgumentOutOfRangeException(int value)
        {
            using var control = new RichTextBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.SelectionCharOffset = value);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionColor_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(Color.Black, control.SelectionColor);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(Color.Black, control.SelectionColor);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionColor_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(Color.Black, control.SelectionColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(Color.Black, control.SelectionColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SelectionColor_CustomGetCharFormat_TestData()
        {
            yield return new object[] { CFM.COLOR, 0x785634, Color.FromArgb(0xFF, 0x34, 0x56, 0x78) };
            yield return new object[] { CFM.COLOR, 0x78563412, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { CFM.COLOR, 0, Color.Black };

            yield return new object[] { 0, 0x785634, Color.Empty };
            yield return new object[] { 0, 0x78563412, Color.Empty };
            yield return new object[] { 0, 0, Color.Empty };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionColor_CustomGetCharFormat_TestData))]
        public void RichTextBox_SelectionColor_CustomGetCharFormat_Success(uint mask, int textColor, Color expected)
        {
            using var control = new CustomGetCharFormatRichTextBox
            {
                ExpectedWParam = (IntPtr)SCF.SELECTION,
                GetCharFormatResult = new CHARFORMATW
                {
                    dwMask = (CFM)mask,
                    crTextColor = textColor
                }
            };

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.MakeCustom = true;
            Assert.Equal(expected, control.SelectionColor);
        }

        public static IEnumerable<object[]> SelectionColor_Set_TestData()
        {
            yield return new object[] { Color.Empty, Color.Black };
            yield return new object[] { Color.Red, Color.Red };
            yield return new object[] { Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.FromArgb(0xFF, 0x34, 0x56, 0x78) };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionColor_Set_TestData))]
        public void RichTextBox_SelectionColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new RichTextBox
            {
                SelectionColor = value
            };
            Assert.Equal(expected, control.SelectionColor);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.SelectionColor = value;
            Assert.Equal(expected, control.SelectionColor);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionColor_Set_TestData))]
        public void RichTextBox_SelectionColor_SetWithHandle_GetReturnsExpected(Color value, Color expected)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionColor = value;
            Assert.Equal(expected, control.SelectionColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectionColor = value;
            Assert.Equal(expected, control.SelectionColor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public unsafe void RichTextBox_SelectionColor_GetCharFormat_Success()
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.SelectionColor = Color.FromArgb(0x12, 0x34, 0x56, 0x78);
            var format = new CHARFORMATW
            {
                cbSize = (uint)sizeof(CHARFORMATW)
            };
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETCHARFORMAT, (IntPtr)SCF.SELECTION, ref format));
            Assert.Equal(0x785634, format.crTextColor);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionFont_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Font result1 = control.SelectionFont;
            Assert.NotNull(result1);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Font result2 = control.SelectionFont;
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionFont_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Font result1 = control.SelectionFont;
            Assert.NotNull(result1);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Font result2 = control.SelectionFont;
            Assert.NotNull(result2);
            Assert.NotSame(result1, result2);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> SelectionFont_Set_TestData()
        {
            yield return new object[] { new Font("Arial", 8.25f), 1 };
            yield return new object[] { new Font("Arial", 8.25f, FontStyle.Bold | FontStyle.Italic | FontStyle.Regular | FontStyle.Strikeout | FontStyle.Underline, GraphicsUnit.Point, 10), 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionFont_Set_TestData))]
        public void RichTextBox_SelectionFont_Set_GetReturnsExpected(Font value, byte expectedGdiCharset)
        {
            using var control = new RichTextBox
            {
                SelectionFont = value
            };
            Font result1 = control.SelectionFont;
            Assert.NotSame(result1, value);
            Assert.Equal(value?.Name, result1.Name);
            Assert.Equal(value?.Size, result1.Size);
            Assert.Equal(value?.Style, result1.Style);
            Assert.Equal(expectedGdiCharset, result1.GdiCharSet);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.SelectionFont = value;
            Font result2 = control.SelectionFont;
            Assert.Equal(value?.Name, result2.Name);
            Assert.Equal(value?.Size, result2.Size);
            Assert.Equal(value?.Style, result2.Style);
            Assert.Equal(expectedGdiCharset, result2.GdiCharSet);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SelectionFont_Set_TestData))]
        public void RichTextBox_SelectionFont_SetWithHandle_GetReturnsExpected(Font value, byte expectedGdiCharset)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.SelectionFont = value;
            Font result1 = control.SelectionFont;
            Assert.NotSame(result1, value);
            Assert.Equal(value?.Name, result1.Name);
            Assert.Equal(value?.Size, result1.Size);
            Assert.Equal(value?.Style, result1.Style);
            Assert.Equal(expectedGdiCharset, result1.GdiCharSet);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.SelectionFont = value;
            Font result2 = control.SelectionFont;
            Assert.Equal(value?.Name, result2.Name);
            Assert.Equal(value?.Size, result2.Size);
            Assert.Equal(value?.Style, result2.Style);
            Assert.Equal(expectedGdiCharset, result2.GdiCharSet);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionFont_SetNull_ThrowsNullReferenceException()
        {
            using var control = new RichTextBox();
            Assert.Throws<NullReferenceException>(() => control.SelectionFont = null);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionHangingIndent_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.SelectionHangingIndent);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(0, control.SelectionHangingIndent);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionHangingIndent_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.SelectionHangingIndent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(0, control.SelectionHangingIndent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionIndent_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.SelectionIndent);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(0, control.SelectionIndent);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionIndent_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.SelectionIndent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(0, control.SelectionIndent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionLength_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionLength_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Get again.
            Assert.Equal(0, control.SelectionLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionProtected_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.False(control.SelectionProtected);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.False(control.SelectionProtected);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionProtected_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.False(control.SelectionProtected);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.False(control.SelectionProtected);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionRightIndent_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.SelectionRightIndent);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(0, control.SelectionRightIndent);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionRightIndent_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.SelectionRightIndent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(0, control.SelectionRightIndent);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionStart_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated);

            // Get again.
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionStart_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Get again.
            Assert.Equal(0, control.SelectionStart);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionTabs_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Empty(control.SelectionTabs);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Empty(control.SelectionTabs);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionTabs_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Empty(control.SelectionTabs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Empty(control.SelectionTabs);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionType_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_SelectionType_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(RichTextBoxSelectionTypes.Empty, control.SelectionType);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_ShowSelectionMargin_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.False(control.ShowSelectionMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call EM_SETOPTIONS.
            User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_SETOPTIONS, (IntPtr)RichTextBoxConstants.ECOOP_OR, (IntPtr)RichTextBoxConstants.ECO_SELECTIONBAR);
            Assert.False(control.ShowSelectionMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RichTextBox_ShowSelectionMargin_Set_GetReturnsExpected(bool value)
        {
            using var control = new RichTextBox
            {
                ShowSelectionMargin = value
            };
            Assert.Equal(value, control.ShowSelectionMargin);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ShowSelectionMargin = value;
            Assert.Equal(value, control.ShowSelectionMargin);
            Assert.False(control.IsHandleCreated);

            // Set different.
            control.ShowSelectionMargin = !value;
            Assert.Equal(!value, control.ShowSelectionMargin);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void RichTextBox_ShowSelectionMargin_SetWithHandle_GetReturnsExpected(bool value)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ShowSelectionMargin = value;
            Assert.Equal(value, control.ShowSelectionMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ShowSelectionMargin = value;
            Assert.Equal(value, control.ShowSelectionMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.ShowSelectionMargin = !value;
            Assert.Equal(!value, control.ShowSelectionMargin);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(true, 0x1000041)]
        [InlineData(false, 0x41)]
        public void RichTextBox_ShowSelectionMargin_GetOptions_Success(bool value, int expected)
        {
            using var control = new RichTextBox();

            Assert.NotEqual(IntPtr.Zero, control.Handle);
            control.ShowSelectionMargin = value;
            Assert.Equal((IntPtr)expected, User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_GETOPTIONS));
        }

        [WinFormsFact]
        public void RichTextBox_TextLength_GetDefaultWithoutHandle_Success()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.TextLength);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(0, control.TextLength);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_TextLength_GetDefaultWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.TextLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            Assert.Equal(0, control.TextLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData("", 0)]
        [InlineData("a\0b", 1)]
        [InlineData("a", 1)]
        [InlineData("\ud83c\udf09", 2)]
        public void RichTextBox_TextLength_GetSetWithHandle_Success(string text, int expected)
        {
            using var control = new RichTextBox
            {
                Text = text
            };
            Assert.Equal(expected, control.TextLength);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("", 0)]
        [InlineData("a\0b", 1)]
        [InlineData("a", 1)]
        [InlineData("\ud83c\udf09", 2)]
        public void RichTextBox_TextLength_GetWithHandle_ReturnsExpected(string text, int expected)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = text;
            Assert.Equal(expected, control.TextLength);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> TextLength_GetCustomGetTextLengthEx_TestData()
        {
            yield return new object[] { IntPtr.Zero, 0 };
            yield return new object[] { (IntPtr)(-1), -1 };
            yield return new object[] { (IntPtr)1, 1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(TextLength_GetCustomGetTextLengthEx_TestData))]
        public void RichTextBox_TextLength_GetCustomGetTextLengthEx_Success(IntPtr result, int expected)
        {
            using var control = new CustomGetTextLengthExRichTextBox
            {
                GetTextLengthExResult = result
            };
            Assert.Equal(expected, control.TextLength);
        }

        private class CustomGetTextLengthExRichTextBox : RichTextBox
        {
            public IntPtr GetTextLengthExResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == RichEditMessages.EM_GETTEXTLENGTHEX)
                {
                    GETTEXTLENGTHEX* gtl = (GETTEXTLENGTHEX*)m.WParam;
                    Assert.Equal(GTL.NUMCHARS, gtl->flags);
                    Assert.Equal(1200u, gtl->codepage);
                    Assert.Equal(IntPtr.Zero, m.LParam);
                    m.Result = GetTextLengthExResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        public static IEnumerable<object[]> Text_Set_TestData()
        {
            foreach (bool autoSize in new bool[] { true, false })
            {
                yield return new object[] { autoSize, null, string.Empty };
                yield return new object[] { autoSize, string.Empty, string.Empty };
                yield return new object[] { autoSize, "text", "text" };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_Set_TestData))]
        public void RichTextBox_Text_Set_GetReturnsExpected(bool autoSize, string value, string expected)
        {
            using var control = new RichTextBox
            {
                AutoSize = autoSize
            };
            int layoutCallCount = 0;
            control.Layout += (control, e) => layoutCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.Equal(0, layoutCallCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void RichTextBox_Text_SetWithRtfText_GetReturnsExpected(string value, string expected)
        {
            using var control = new RichTextBox
            {
                Rtf = "{\\rtf Hello World}",
                Text = value
            };
            Assert.Equal(expected, control.Text);
            Assert.Contains(expected, control.Rtf);
            Assert.DoesNotContain("Hello World", control.Rtf);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Contains(expected, control.Rtf);
            Assert.DoesNotContain("Hello World", control.Rtf);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);
        }

        public static IEnumerable<object[]> Text_SetWithParent_TestData()
        {
            yield return new object[] { true, null, string.Empty, 1 };
            yield return new object[] { true, string.Empty, string.Empty, 1 };
            yield return new object[] { true, "text", "text", 1 };

            yield return new object[] { false, null, string.Empty, 0 };
            yield return new object[] { false, string.Empty, string.Empty, 0 };
            yield return new object[] { false, "text", "text", 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithParent_TestData))]
        public void RichTextBox_Text_SetWithParent_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new RichTextBox
            {
                Parent = parent,
                AutoSize = autoSize
            };
            int layoutCallCount = 0;
            control.Layout += (control, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Text", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.False(control.Modified);
                Assert.False(control.CanUndo);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);

                // Set same.
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.False(control.Modified);
                Assert.False(control.CanUndo);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
                Assert.False(control.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Text_SetWithSelection_TestData()
        {
            yield return new object[] { string.Empty, 0, 2, null, string.Empty };
            yield return new object[] { string.Empty, 0, 2, "t", "t" };
            yield return new object[] { string.Empty, 0, 2, "text", "text" };

            yield return new object[] { string.Empty, 1, 2, null, string.Empty };
            yield return new object[] { string.Empty, 1, 2, "t", "t" };
            yield return new object[] { string.Empty, 1, 2, "text", "text" };

            yield return new object[] { "text", 0, 2, null, string.Empty };
            yield return new object[] { "text", 0, 2, "t", "t" };
            yield return new object[] { "text", 0, 2, "te", "te" };
            yield return new object[] { "text", 0, 2, "tex", "tex" };

            yield return new object[] { "text", 1, 2, null, string.Empty };
            yield return new object[] { "text", 1, 2, "t", "t" };
            yield return new object[] { "text", 1, 2, "te", "te" };
            yield return new object[] { "text", 1, 2, "tex", "tex" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithSelection_TestData))]
        public void RichTextBox_Text_SetWithSelection_GetReturnsExpected(string oldValue, int selectionStart, int selectionLength, string value, string expected)
        {
            using var control = new RichTextBox
            {
                Text = oldValue,
                SelectionStart = selectionStart,
                SelectionLength = selectionLength,
            };

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "", true)]
        [InlineData("", "", false)]
        [InlineData("text", "text", false)]
        public void RichTextBox_Text_SetModified_GetReturnsExpected(string value, string expected, bool expectedModified)
        {
            using var control = new RichTextBox
            {
                Modified = true,
                Text = value
            };
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.Equal(expectedModified, control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_Set_TestData))]
        public void RichTextBox_Text_SetWithHandle_GetReturnsExpected(bool autoSize, string value, string expected)
        {
            using var control = new RichTextBox
            {
                AutoSize = autoSize
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (control, e) => layoutCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.Equal(0, layoutCallCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void RichTextBox_Text_SetWithRtfTextWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new RichTextBox
            {
                Rtf = "{\\rtf Hello World}"
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Contains(expected, control.Rtf);
            Assert.DoesNotContain("Hello World", control.Rtf);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Contains(expected, control.Rtf);
            Assert.DoesNotContain("Hello World", control.Rtf);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithParent_TestData))]
        public void RichTextBox_Text_SetWithParentWithHandle_GetReturnsExpected(bool autoSize, string value, string expected, int expectedParentLayoutCallCount)
        {
            using var parent = new Control();
            using var control = new RichTextBox
            {
                Parent = parent,
                AutoSize = autoSize
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;
            int layoutCallCount = 0;
            control.Layout += (control, e) => layoutCallCount++;
            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Same(control, e.AffectedControl);
                Assert.Equal("Text", e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.False(control.Modified);
                Assert.False(control.CanUndo);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Set same.
                control.Text = value;
                Assert.Equal(expected, control.Text);
                Assert.False(control.Modified);
                Assert.False(control.CanUndo);
                Assert.Equal(0, layoutCallCount);
                Assert.Equal(expectedParentLayoutCallCount * 2, parentLayoutCallCount);
                Assert.True(control.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("text", "text")]
        public void RichTextBox_Text_SetModifiedWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var control = new RichTextBox
            {
                Modified = true
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Text_SetWithSelectionWithHandle_TestData()
        {
            yield return new object[] { string.Empty, 0, 2, null, string.Empty };
            yield return new object[] { string.Empty, 0, 2, "t", "t" };
            yield return new object[] { string.Empty, 0, 2, "text", "text" };

            yield return new object[] { string.Empty, 1, 2, null, string.Empty };
            yield return new object[] { string.Empty, 1, 2, "t", "t" };
            yield return new object[] { string.Empty, 1, 2, "text", "text" };

            yield return new object[] { "text", 0, 2, null, string.Empty };
            yield return new object[] { "text", 0, 2, "t", "t" };
            yield return new object[] { "text", 0, 2, "te", "te" };
            yield return new object[] { "text", 0, 2, "tex", "tex" };

            yield return new object[] { "text", 1, 2, null, string.Empty };
            yield return new object[] { "text", 1, 2, "t", "t" };
            yield return new object[] { "text", 1, 2, "te", "te" };
            yield return new object[] { "text", 1, 2, "tex", "tex" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Text_SetWithSelectionWithHandle_TestData))]
        public void RichTextBox_Text_SetWithSelectionWith_GetReturnsExpected(string oldValue, int selectionStart, int selectionLength, string value, string expected)
        {
            using var control = new RichTextBox
            {
                Text = oldValue,
                SelectionStart = selectionStart,
                SelectionLength = selectionLength,
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
            Assert.Equal(expected.Length, control.TextLength);
            Assert.Equal(0, control.SelectionStart);
            Assert.Equal(0, control.SelectionLength);
            Assert.Empty(control.SelectedText);
            Assert.False(control.Modified);
            Assert.False(control.CanUndo);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_Text_SetWithHandler_CallsTextChanged()
        {
            using var control = new RichTextBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(0, callCount);

            // Set same.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(0, callCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void RichTextBox_Text_SetWithHandlerWithHandle_CallsTextChanged()
        {
            using var control = new RichTextBox();
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
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(2, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(3, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Equal("text", control.Text);
            Assert.Equal(3, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_UndoActionName_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Empty(control.UndoActionName);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> UndoActionName_CustomGetUndoName_TestData()
        {
            yield return new object[] { IntPtr.Zero, IntPtr.Zero, string.Empty };
            yield return new object[] { (IntPtr)1, IntPtr.Zero, "Unknown" };
            yield return new object[] { (IntPtr)1, (IntPtr)1, "Typing" };
            yield return new object[] { (IntPtr)1, (IntPtr)2, "Delete" };
            yield return new object[] { (IntPtr)1, (IntPtr)3, "Drag and Drop" };
            yield return new object[] { (IntPtr)1, (IntPtr)4, "Cut" };
            yield return new object[] { (IntPtr)1, (IntPtr)5, "Paste" };
            yield return new object[] { (IntPtr)1, (IntPtr)6, "Unknown" };
            yield return new object[] { (IntPtr)1, (IntPtr)7, "Unknown" };
        }

        [WinFormsTheory]
        [MemberData(nameof(UndoActionName_CustomGetUndoName_TestData))]
        public void RichTextBox_UndoActionName_CustomGetUndoName_ReturnsExpected(IntPtr canUndoResult, IntPtr getUndoNameResult, string expected)
        {
            using var control = new CustomGetUndoNameRichTextBox
            {
                CanUndoResult = canUndoResult,
                GetUndoNameResult = getUndoNameResult
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.UndoActionName);
        }

        private class CustomGetUndoNameRichTextBox : RichTextBox
        {
            public IntPtr CanUndoResult { get; set; }
            public IntPtr GetUndoNameResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.EM.CANUNDO)
                {
                    m.Result = CanUndoResult;
                    return;
                }
                else if (m.Msg == RichEditMessages.EM_GETUNDONAME)
                {
                    m.Result = GetUndoNameResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void RichTextBox_ZoomFactor_GetWithHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(1, control.ZoomFactor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call EM_SETZOOM.
            User32.SendMessageW(control.Handle, (User32.WM)RichEditMessages.EM_SETZOOM, (IntPtr)2, (IntPtr)10);
            Assert.Equal(0.2f, control.ZoomFactor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(1, 1, 1.0f)]
        [InlineData(10, 1, 10.0f)]
        [InlineData(10, 2, 5.0f)]
        [InlineData(1, 10, 0.1f)]
        [InlineData(1, 0, 1f)]
        [InlineData(0, 1, 1f)]
        [InlineData(0, 0, 1f)]
        public void RichTextBox_ZoomFactor_CustomGetZoom_ReturnsExpected(int numerator, int denominator, float expected)
        {
            using var control = new CustomGetZoomRichTextBox
            {
                NumeratorResult = numerator,
                DenominatorResult = denominator
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.ZoomFactor);
        }

        private class CustomGetZoomRichTextBox : RichTextBox
        {
            public int NumeratorResult { get; set; }
            public int DenominatorResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == RichEditMessages.EM_GETZOOM)
                {
                    int* pNumerator = (int*)m.WParam;
                    int* pDenominator = (int*)m.LParam;

                    *pNumerator = NumeratorResult;
                    *pDenominator = DenominatorResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsTheory]
        [InlineData(0.015626f, 0.016f)]
        [InlineData(63.9f, 63.9f)]
        [InlineData(1.0f, 1.0f)]
        [InlineData(2.0f, 2.0f)]
        [InlineData(float.NaN, -2147483.75f)]
        public void RichTextBox_ZoomFactor_Set_GetReturnsExpected(float value, float expected)
        {
            using var control = new RichTextBox
            {
                ZoomFactor = value
            };
            Assert.Equal(expected, control.ZoomFactor, 2);
            Assert.False(control.IsHandleCreated);

            // Set same.
            control.ZoomFactor = value;
            Assert.Equal(expected, control.ZoomFactor, 2);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0.015626f, 0.016f)]
        [InlineData(63.9f, 63.9f)]
        [InlineData(1.0f, 1.0f)]
        [InlineData(2.0f, 2.0f)]
        [InlineData(float.NaN, 1.0f)]
        public void RichTextBox_ZoomFactor_SetWithHandle_GetReturnsExpected(float value, float expected)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.ZoomFactor = value;
            Assert.Equal(expected, control.ZoomFactor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            control.ZoomFactor = value;
            Assert.Equal(expected, control.ZoomFactor);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(0.015624f)]
        [InlineData(0.015625f)]
        [InlineData(64.0f)]
        [InlineData(64.1f)]
        [InlineData(float.PositiveInfinity)]
        [InlineData(float.NegativeInfinity)]
        public void RichTextBox_ZoomFactor_SetInvalidValue_ThrowsArgumentOutOfRangeException(float value)
        {
            using var control = new RichTextBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.ZoomFactor = value);
        }

        public static IEnumerable<object[]> CanPaste_TestData()
        {
            yield return new object[] { DataFormats.GetFormat(DataFormats.Palette), false };
            yield return new object[] { new DataFormats.Format("UnknownName", int.MaxValue), false };
        }

        [WinFormsTheory]
        [MemberData(nameof(CanPaste_TestData))]
        public void RichTextBox_CanPaste_Invoke_ReturnsExpected(DataFormats.Format format, bool expected)
        {
            using var control = new RichTextBox();
            Assert.Equal(expected, control.CanPaste(format));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CanPaste_TestData))]
        public void RichTextBox_CanPaste_InvokeWithHandle_ReturnsExpected(DataFormats.Format format, bool expected)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(expected, control.CanPaste(format));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> CanPaste_CustomCanPaste_TestData()
        {
            yield return new object[] { IntPtr.Zero, false };
            yield return new object[] { (IntPtr)1, true };
        }

        [WinFormsTheory]
        [MemberData(nameof(CanPaste_CustomCanPaste_TestData))]
        public void RichTextBox_CanPaste_CustomCanPaste_ReturnsExpected(IntPtr result, bool expected)
        {
            using var control = new CustomCanPasteRichTextBox
            {
                Result = result
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.CanPaste(DataFormats.GetFormat(DataFormats.Text)));
        }

        private class CustomCanPasteRichTextBox : RichTextBox
        {
            public IntPtr Result { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (m.Msg == RichEditMessages.EM_CANPASTE)
                {
                    m.Result = Result;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsFact]
        public void RichTextBox_CanPaste_NullFormat_ThrowsNullReferenceException()
        {
            using var control = new RichTextBox();
            Assert.Throws<NullReferenceException>(() => control.CanPaste(null));
        }

        [WinFormsFact]
        public void RichTextBox_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubRichTextBox();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void RichTextBox_GetLineFromCharIndex_InvokeEmpty_Success(int index)
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.GetLineFromCharIndex(index));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(5)]
        public void RichTextBox_GetLineFromCharIndex_InvokeNotEmpty_Success(int index)
        {
            using var control = new RichTextBox
            {
                Text = "text"
            };
            Assert.Equal(0, control.GetLineFromCharIndex(index));
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void RichTextBox_GetLineFromCharIndex_InvokeEmptyWithHandle_Success(int index)
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.GetLineFromCharIndex(index));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(5)]
        public void RichTextBox_GetLineFromCharIndex_InvokeNotEmptyWithHandle_Success(int index)
        {
            using var control = new RichTextBox
            {
                Text = "text"
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(0, control.GetLineFromCharIndex(index));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> GetLineFromCharIndex_CustomLineFromChar_TestData()
        {
            yield return new object[] { (IntPtr)(-1) };
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { (IntPtr)1 };
            yield return new object[] { (IntPtr)int.MaxValue };
            yield return new object[] { PARAM.FromLowHigh(1, 2) };
            yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue) };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetLineFromCharIndex_CustomLineFromChar_TestData))]
        public void RichTextBox_GetLineFromCharIndex_CustomLineFromChar_Success(IntPtr result)
        {
            using var control = new CustomLineFromCharRichTextBox
            {
                LineFromCharResult = (IntPtr)result
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(0, control.GetLineFromCharIndex(1));
        }

        private class CustomLineFromCharRichTextBox : RichTextBox
        {
            public IntPtr LineFromCharResult { get; set; }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.EM.LINEFROMCHAR)
                {
                    Assert.Equal((IntPtr)1, m.WParam);
                    Assert.Equal(IntPtr.Zero, m.LParam);
                    m.Result = LineFromCharResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        public static IEnumerable<object[]> GetLineFromCharIndex_CustomExLineFromChar_TestData()
        {
            yield return new object[] { (IntPtr)(-1), -1 };
            yield return new object[] { IntPtr.Zero, 0 };
            yield return new object[] { (IntPtr)1, 1 };
            yield return new object[] { (IntPtr)int.MaxValue, 0x7FFFFFFF };
            yield return new object[] { PARAM.FromLowHigh(1, 2), 0x20001 };
            yield return new object[] { PARAM.FromLowHigh(int.MaxValue, int.MaxValue), -1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetLineFromCharIndex_CustomExLineFromChar_TestData))]
        public void RichTextBox_GetLineFromCharIndex_CustomExLineFromChar_Success(IntPtr result, int expected)
        {
            using var control = new CustomExLineFromCharRichTextBox
            {
                ExLineFromCharResult = (IntPtr)result
            };
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            Assert.Equal(expected, control.GetLineFromCharIndex(1));
        }

        private class CustomExLineFromCharRichTextBox : RichTextBox
        {
            public IntPtr ExLineFromCharResult { get; set; }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == RichEditMessages.EM_EXLINEFROMCHAR)
                {
                    Assert.Equal(IntPtr.Zero, m.WParam);
                    Assert.Equal((IntPtr)1, m.LParam);
                    m.Result = ExLineFromCharResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        [WinFormsTheory]
        [InlineData(ControlStyles.ContainerControl, false)]
        [InlineData(ControlStyles.UserPaint, false)]
        [InlineData(ControlStyles.Opaque, false)]
        [InlineData(ControlStyles.ResizeRedraw, false)]
        [InlineData(ControlStyles.FixedWidth, false)]
        [InlineData(ControlStyles.FixedHeight, false)]
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
        public void RichTextBox_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
        {
            using var control = new SubRichTextBox();
            Assert.Equal(expected, control.GetStyle(flag));

            // Call again to test caching.
            Assert.Equal(expected, control.GetStyle(flag));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RichTextBox_OnBackColorChanged_Invoke_CallsBackColorChanged(EventArgs eventArgs)
        {
            using var control = new SubRichTextBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.BackColorChanged += handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void RichTextBox_OnBackColorChanged_InvokeWithHandle_CallsBackColorChanged(EventArgs eventArgs)
        {
            using var control = new SubRichTextBox();
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
            control.BackColorChanged += handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(1, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            control.BackColorChanged -= handler;
            control.OnBackColorChanged(eventArgs);
            Assert.Equal(1, callCount);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(2, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void RichTextBox_Redo_Invoke_Success()
        {
            using var control = new RichTextBox();
            control.Redo();
            Assert.True(control.IsHandleCreated);

            // Call again.
            control.Redo();
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_Redo_InvokeWithHandle_Success()
        {
            using var control = new RichTextBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Redo();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Call again.
            control.Redo();
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        private class CustomGetCharFormatRichTextBox : RichTextBox
        {
            public bool MakeCustom { get; set; }
            public IntPtr ExpectedWParam { get; set; }
            public CHARFORMATW GetCharFormatResult { get; set; }

            protected unsafe override void WndProc(ref Message m)
            {
                if (MakeCustom && m.Msg == RichEditMessages.EM_GETCHARFORMAT)
                {
                    CHARFORMATW* format = (CHARFORMATW*)m.LParam;
                    Assert.Equal(ExpectedWParam, m.WParam);
                    Assert.NotEqual(IntPtr.Zero, m.LParam);
                    *format = GetCharFormatResult;
                    return;
                }

                base.WndProc(ref m);
            }
        }

        public class SubRichTextBox : RichTextBox
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

            public new bool ResizeRedraw
            {
                get => base.ResizeRedraw;
                set => base.ResizeRedraw = value;
            }

            public new bool ShowFocusCues => base.ShowFocusCues;

            public new bool ShowKeyboardCues => base.ShowKeyboardCues;

            public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new void OnBackColorChanged(EventArgs e) => base.OnBackColorChanged(e);
        }
    }
}
