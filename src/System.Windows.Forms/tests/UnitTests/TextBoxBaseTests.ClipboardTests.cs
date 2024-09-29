// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class TextBoxBaseTests
{
    [Collection("Sequential")]
    [UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
    public class ClipboardTests
    {
        [WinFormsFact]
        public void TextBoxBase_ClearUndo_CanUndo_Success()
        {
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Focus();
            control.Copy();

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();

            control.Text.Should().Be("bcxt");

            control.ClearUndo();
            control.Undo();
            control.Text.Should().Be("bcxt");
        }

        [WinFormsFact]
        public void TextBoxBase_Copy_PasteNotEmpty_Success()
        {
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Copy();
            control.Text.Should().Be("abc");
            control.IsHandleCreated.Should().BeTrue();

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();

            control.Text.Should().Be("bcxt");
            control.CanUndo.Should().BeTrue();
            control.Modified.Should().BeTrue();
            control.IsHandleCreated.Should().BeTrue();
        }

        [WinFormsFact]
        public void TextBoxBase_Copy_PasteNotEmptyWithHandle_Success()
        {
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Handle.Should().NotBe(IntPtr.Zero);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Copy();
            control.Text.Should().Be("abc");
            control.IsHandleCreated.Should().BeTrue();
            invalidatedCallCount.Should().Be(0);
            styleChangedCallCount.Should().Be(0);
            createdCallCount.Should().Be(0);

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();

            control.Text.Should().Be("bcxt");
            control.CanUndo.Should().BeTrue();
            control.Modified.Should().BeTrue();
            control.IsHandleCreated.Should().BeTrue();
            invalidatedCallCount.Should().Be(0);
            styleChangedCallCount.Should().Be(0);
            createdCallCount.Should().Be(0);
        }

        [WinFormsFact]
        public void TextBoxBase_Cut_PasteNotEmpty_Success()
        {
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Cut();
            control.Text.Should().Be("a");
            control.IsHandleCreated.Should().BeTrue();

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();

            control.Text.Should().Be("bcxt");
            control.CanUndo.Should().BeTrue();
            control.Modified.Should().BeTrue();
            control.IsHandleCreated.Should().BeTrue();
        }

        [WinFormsFact]
        public void TextBoxBase_Cut_PasteNotEmptyWithHandle_Success()
        {
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Handle.Should().NotBe(IntPtr.Zero);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Cut();
            control.Text.Should().Be("a");
            control.IsHandleCreated.Should().BeTrue();
            invalidatedCallCount.Should().Be(0);
            styleChangedCallCount.Should().Be(0);
            createdCallCount.Should().Be(0);

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();

            control.Text.Should().Be("bcxt");
            control.CanUndo.Should().BeTrue();
            control.Modified.Should().BeTrue();
            control.IsHandleCreated.Should().BeTrue();
            invalidatedCallCount.Should().Be(0);
            styleChangedCallCount.Should().Be(0);
            createdCallCount.Should().Be(0);
        }

        [WinFormsFact]
        public void TextBoxBase_Paste_InvokeEmpty_Success()
        {
            Clipboard.Clear();
            using SubTextBox control = new();
            control.Paste();
            control.Text.Should().BeEmpty();
            control.IsHandleCreated.Should().BeTrue();
        }

        [WinFormsFact]
        public void TextBoxBase_Paste_InvokeNotEmpty_Success()
        {
            Clipboard.Clear();
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Paste();
            control.Text.Should().Be("a");
            control.IsHandleCreated.Should().BeTrue();
        }

        [WinFormsFact]
        public void TextBoxBase_Undo_CanUndo_Success()
        {
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Copy();

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();

            control.Text.Should().Be("bcxt");

            control.Undo();
            control.Text.Should().Be("text");
        }

        [WinFormsFact]
        public void TextBoxBase_Copy_PasteEmpty_Success()
        {
            using SubTextBox control = new();
            control.Copy();
            control.Text.Should().BeEmpty();
            control.IsHandleCreated.Should().BeTrue();

            control.Text = "text";
            control.SelectionLength = 2;
            control.Text.Should().Be("text");
            control.IsHandleCreated.Should().BeTrue();
        }

        [WinFormsFact]
        public void TextBoxBase_Copy_PasteEmptyWithHandle_Success()
        {
            using SubTextBox control = new();
            control.Handle.Should().NotBe(IntPtr.Zero);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Copy();
            control.Text.Should().BeEmpty();
            control.IsHandleCreated.Should().BeTrue();
            invalidatedCallCount.Should().Be(0);
            styleChangedCallCount.Should().Be(0);
            createdCallCount.Should().Be(0);

            control.Text = "text";
            control.SelectionLength = 2;
            control.Text.Should().Be("text");
            control.IsHandleCreated.Should().BeTrue();
            invalidatedCallCount.Should().Be(0);
            styleChangedCallCount.Should().Be(0);
            createdCallCount.Should().Be(0);
        }
    }
}
