// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class TextBoxBaseTests
{
    [Collection("Sequential")]
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
            control.Copy();

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();
            Assert.Equal("bcxt", control.Text);

            control.ClearUndo();
            control.Undo();
            Assert.Equal("bcxt", control.Text);
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
            Assert.Equal("abc", control.Text);
            Assert.True(control.IsHandleCreated);

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();
            Assert.Equal("bcxt", control.Text);
            Assert.True(control.CanUndo);
            Assert.True(control.Modified);
            Assert.True(control.IsHandleCreated);
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
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Copy();
            Assert.Equal("abc", control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();
            Assert.Equal("bcxt", control.Text);
            Assert.True(control.CanUndo);
            Assert.True(control.Modified);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
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
            Assert.Equal("a", control.Text);
            Assert.True(control.IsHandleCreated);

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();
            Assert.Equal("bcxt", control.Text);
            Assert.True(control.CanUndo);
            Assert.True(control.Modified);
            Assert.True(control.IsHandleCreated);
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
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Cut();
            Assert.Equal("a", control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            control.Text = "text";
            control.SelectionLength = 2;
            control.Paste();
            Assert.Equal("bcxt", control.Text);
            Assert.True(control.CanUndo);
            Assert.True(control.Modified);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void TextBoxBase_Paste_InvokeEmpty_Success()
        {
            using SubTextBox control = new();
            control.Paste();
            Assert.NotNull(control.Text);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBoxBase_Paste_InvokeNotEmpty_Success()
        {
            using SubTextBox control = new()
            {
                Text = "abc",
                SelectionStart = 1,
                SelectionLength = 2
            };
            control.Paste();
            Assert.Equal("abc", control.Text);
            Assert.True(control.IsHandleCreated);
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
            Assert.Equal("bcxt", control.Text);

            control.Undo();
            Assert.Equal("text", control.Text);
        }

        [WinFormsFact]
        public void TextBoxBase_Copy_PasteEmpty_Success()
        {
            using SubTextBox control = new();
            control.Copy();
            Assert.Empty(control.Text);
            Assert.True(control.IsHandleCreated);

            control.Text = "text";
            control.SelectionLength = 2;
            Assert.Equal("text", control.Text);
            Assert.True(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void TextBoxBase_Copy_PasteEmptyWithHandle_Success()
        {
            using SubTextBox control = new();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            control.Copy();
            Assert.Empty(control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            control.Text = "text";
            control.SelectionLength = 2;
            Assert.Equal("text", control.Text);
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
    }
}
