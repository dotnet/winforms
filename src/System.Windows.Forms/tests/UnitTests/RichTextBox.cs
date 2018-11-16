// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    using System.Drawing;

    public class RichTextBoxTests
    {
        [Fact]
        public void RichTextBox_Constructor()
        {
            var rtb = new RichTextBox();

            Assert.NotNull(rtb);
            Assert.True(rtb.DetectUrls);
            Assert.Equal(RichTextBoxScrollBars.Both, rtb.ScrollBars);
            Assert.True(rtb.RichTextShortcutsEnabled);
            Assert.Equal(int.MaxValue, rtb.MaxLength);
            Assert.True(rtb.Multiline);
            Assert.False(rtb.AutoSize);
        }
    }
}
