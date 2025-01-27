// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public partial class RichTextBoxTests
{
    [Collection("Sequential")]
    [UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
    public class ClipboardTests
    {
        [WinFormsFact]
        public void RichTextBox_OleObject_IncompleteOleObject_DoNothing()
        {
            using RichTextBox control = new();
            control.Handle.Should().NotBe(IntPtr.Zero);

            using MemoryStream memoryStream = new();
            using Bitmap bitmap = new(100, 100);
            bitmap.Save(memoryStream, Drawing.Imaging.ImageFormat.Png);
#pragma warning disable WFDEV005 // Type or member is obsolete
            Clipboard.SetData("Embed Source", memoryStream);
#pragma warning restore WFDEV005

            control.Text.Should().BeEmpty();
        }

        public static TheoryData<string> PlainTextData => new()
        {
            { "Hello World"},
            { new string('a', 10000) },
            { "Special characters: !@#$%^&*()" },
        };

        [WinFormsTheory]
        [MemberData(nameof(PlainTextData))]
        public void RichTextBox_Paste_PlainText_Data(string value)
        {
            using RichTextBox richTextBox1 = new();

            if (!string.IsNullOrEmpty(value))
            {
                Clipboard.SetText(value);
                richTextBox1.Paste(DataFormats.GetFormat(DataFormats.Text));

                richTextBox1.Text.Should().Be(value);
            }
        }

        [WinFormsFact]
        public void RichTextBox_Paste_EmptyString_Data()
        {
            using RichTextBox richTextBox1 = new();

            Clipboard.Clear();
            richTextBox1.Paste(DataFormats.GetFormat(DataFormats.Text));

            richTextBox1.Text.Should().BeEmpty();
        }

        public static TheoryData<string> RtfData => new()
        {
            { "{\\rtf Hello World}" },
            { "{\\rtf1\\ansi{Sample for {\\v HIDDEN }text}}" },
            { "{\\rtf1\\ansi{Invalid RTF data" },
        };

        [WinFormsTheory]
        [MemberData(nameof(RtfData))]
        public void RichTextBox_Paste_Rtf_Data(string rtf)
        {
            using RichTextBox richTextBox1 = new();

            if (!string.IsNullOrEmpty(rtf))
            {
                Clipboard.SetText(rtf);
                richTextBox1.Paste(DataFormats.GetFormat(DataFormats.Rtf));

                richTextBox1.Rtf.Should().StartWith("{\\rtf");
            }
        }
    }
}
