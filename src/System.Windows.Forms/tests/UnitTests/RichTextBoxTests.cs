// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    using System.Drawing;
    using System.Text;

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

        [WinFormsFact]
        public void RichTextBox_SetAnsiRtf_DoesNotCorrupt()
        {
            // RichTextBox.Rtf treats input as code page specific (i.e. not Unicode). To see
            // that we're actually using the code page we'll set the text to 0xA0 (160 / nbsp) to see
            // that we can get it back out. If the encoding is not done in the codepage we'll likely
            // get multiple ASCII bytes back out. UTF-8, for example, encodes the .NET UTF-16 0x00A0 (160)
            // to 0xC2 0xA0.

            // Ultimately we should really update RichTextBox to always stream data in Unicode as
            // we're hard-coded to load 4.1 (see RichTextBox.CreateParams).

            using var control = new RichTextBox();

            Span<char> input = stackalloc char[] { (char)0xA0 };
            control.Rtf = $"{{\\rtf1\\ansi {input[0]}}}";

            Span<byte> output = stackalloc byte[16];

            int currentCodePage = (CodePagesEncodingProvider.Instance.GetEncoding(0) ?? Encoding.UTF8).CodePage;

            // The non-lossy conversion of nbsp only works single byte Windows code pages (e.g. not Japanese).
            if (currentCodePage >= 1250 && currentCodePage <= 1258)
            {
                Assert.Equal(input[0], control.Text[0]);
            }
        }
    }
}
