// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    }
}
