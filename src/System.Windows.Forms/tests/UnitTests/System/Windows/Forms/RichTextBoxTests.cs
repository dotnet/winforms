// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using Xunit;
using static Interop;

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
        public void RichTextBox_TextLength_GetWithoutHandle_ReturnsExpected()
        {
            using var control = new RichTextBox();
            Assert.Equal(0, control.TextLength);
            Assert.True(control.IsHandleCreated);

            // Call again.
            Assert.Equal(0, control.TextLength);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void RichTextBox_TextLength_GetWithHandle_ReturnsExpected()
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

        [WinFormsFact]
        public void RichTextBox_GetAutoSizeMode_Invoke_ReturnsExpected()
        {
            using var control = new SubRichTextBox();
            Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
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
        }
    }
}
