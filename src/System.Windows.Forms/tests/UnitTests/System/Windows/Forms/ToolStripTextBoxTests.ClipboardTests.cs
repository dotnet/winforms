// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class ToolStripTextBoxTests
{
    [Collection("Sequential")]
    [UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
    public class ClipboardTests
    {
        [WinFormsFact]
        public void ToolStripTextBox_CopyPaste_Success()
        {
            using ToolStripTextBox toolStripTextBox = new();
            toolStripTextBox.Text = "Hello";
            toolStripTextBox.SelectAll();
            toolStripTextBox.Copy();

            using ToolStripTextBox anotherToolStripTextBox = new();
            anotherToolStripTextBox.Paste();
            anotherToolStripTextBox.Text.Should().Be("Hello");
        }

        [WinFormsFact]
        public void ToolStripTextBox_Cut_Success()
        {
            using ToolStripTextBox toolStripTextBox = new();
            toolStripTextBox.Text = "Hello";
            toolStripTextBox.SelectAll();
            toolStripTextBox.Cut();
            toolStripTextBox.Text.Should().BeEmpty();
        }

        [WinFormsFact]
        public void ToolStripTextBox_Paste_Success()
        {
            using ToolStripTextBox toolStripTextBox = new();
            string textToPaste = "Hello";
            Clipboard.SetText(textToPaste);
            toolStripTextBox.Paste();
            toolStripTextBox.Text.Should().Be(textToPaste);
        }
    }
}
