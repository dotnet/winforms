// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TextBoxTests
    {
        [Fact]
        public void TextBox_Constructor()
        {
            var tb = new TextBox();

            Assert.NotNull(tb);
        }

        [Fact]
        public void TextBox_PlaceholderText()
        {
            var tb = new TextBox();
            tb.PlaceholderText = "Enter your name";
            Assert.False(string.IsNullOrEmpty(tb.PlaceholderText));
        }

        [Fact]
        public void TextBox_PlaceholderTextAlignments()
        {
            var tb = new TextBox();
            tb.PlaceholderText = "Enter your name";

            System.Runtime.InteropServices.HandleRef refHandle = new System.Runtime.InteropServices.HandleRef(tb, tb.Handle);
           
            //Cover the Placeholder draw code path 
            UnsafeNativeMethods.SendMessage(refHandle, Interop.WindowMessages.WM_PAINT, false, 0);
            tb.TextAlign = HorizontalAlignment.Center;
            UnsafeNativeMethods.SendMessage(refHandle, Interop.WindowMessages.WM_PAINT, false, 0);
            tb.TextAlign = HorizontalAlignment.Right;
            UnsafeNativeMethods.SendMessage(refHandle, Interop.WindowMessages.WM_PAINT, false, 0);

            Assert.False(string.IsNullOrEmpty(tb.PlaceholderText));
        }

        [Fact]
        public void TextBox_PlaceholderTextAlignmentsInRightToLeft()
        {
            var tb = new TextBox();
            tb.PlaceholderText = "Enter your name";
            tb.RightToLeft = RightToLeft.Yes;

            System.Runtime.InteropServices.HandleRef refHandle = new System.Runtime.InteropServices.HandleRef(tb, tb.Handle);

            //Cover the Placeholder draw code path in RightToLeft scenario
            UnsafeNativeMethods.SendMessage(refHandle, Interop.WindowMessages.WM_PAINT, false, 0);
            tb.TextAlign = HorizontalAlignment.Center;
            UnsafeNativeMethods.SendMessage(refHandle, Interop.WindowMessages.WM_PAINT, false, 0);
            tb.TextAlign = HorizontalAlignment.Right;
            UnsafeNativeMethods.SendMessage(refHandle, Interop.WindowMessages.WM_PAINT, false, 0);

            Assert.False(string.IsNullOrEmpty(tb.PlaceholderText));
        }

        private SubTextBoxBase CreateTextBoxForCtrlBackspace(string text = "", int cursorRelativeToEnd = 0)
        {
            var tb = new SubTextBoxBase();
            tb.Text = text;
            tb.Focus();
            tb.SelectionStart = tb.Text.Length + cursorRelativeToEnd;
            tb.SelectionLength = 0;
            return tb;
        }

        private void SendCtrlBackspace(SubTextBoxBase tb)
        {
            var message = new Message();
            tb.ProcessCmdKey(ref message, Keys.Control | Keys.Back);
        }

        [Fact]
        public void TextBox_CtrlBackspaceTextRemainsEmpty()
        {
            SubTextBoxBase tb = CreateTextBoxForCtrlBackspace();
            SendCtrlBackspace(tb);
            Assert.Equal("", tb.Text);
        }

        [Fact]
        public void TextBox_CtrlBackspaceReadOnlyTextUnchanged()
        {
            string text = "aaa";
            SubTextBoxBase tb = CreateTextBoxForCtrlBackspace(text);
            tb.ReadOnly = true;
            SendCtrlBackspace(tb);
            Assert.Equal(text, tb.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCtrlBackspaceData))]
        public void TextBox_CtrlBackspaceTextChanged(string value, string expected, int cursorRelativeToEnd)
        {
            SubTextBoxBase tb = CreateTextBoxForCtrlBackspace(value, cursorRelativeToEnd);
            SendCtrlBackspace(tb);
            Assert.Equal(expected, tb.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetCtrlBackspaceRepeatedData))]
        public void TextBox_CtrlBackspaceRepeatedTextChanged(string value, string expected, int repeats)
        {
            SubTextBoxBase tb = CreateTextBoxForCtrlBackspace(value);
            for (int i = 0; i < repeats; i++)
            {
                SendCtrlBackspace(tb);
            }
            Assert.Equal(expected, tb.Text);
        }

        [Fact]
        public void TextBox_CtrlBackspaceDeletesSelection()
        {
            SubTextBoxBase tb = CreateTextBoxForCtrlBackspace("123-5-7-9");
            tb.SelectionStart = 2;
            tb.SelectionLength = 5;
            SendCtrlBackspace(tb);
            Assert.Equal("12-9", tb.Text);
        }

        private class SubTextBoxBase : TextBoxBase
        {
            public new bool ProcessCmdKey(ref Message msg, Keys keyData) => base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
