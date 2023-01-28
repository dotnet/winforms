// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace WinformsControlsTest
{
    public partial class RichTextBoxes : Form
    {
        public RichTextBoxes()
        {
            InitializeComponent();

            richTextBox1.LoadFile(File.OpenRead(@"Data\example.rtf"), RichTextBoxStreamType.RichText);
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox2.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang4105{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 10.0.17134}\viewkind4\uc1 
{\field{\*\fldinst { HYPERLINK ""http://www.google.com"" }}{\fldrslt {Click here}}}
\pard\sa200\sl276\slmult1\f0\fs22\lang9  for more information.\par
This is a \v #data#\v0 custom link with hidden text before the link.\par
This is a custom link\v #data#\v0  with hidden text after the link.\par
}";

            // Allow setting custom links.
            richTextBox2.DetectUrls = false;

            MakeLink(richTextBox2, "#data#custom link");
            MakeLink(richTextBox2, "custom link#data#");
        }

        private unsafe void MakeLink(RichTextBox control, string text)
        {
            control.Select(control.Text.IndexOf(text), text.Length);

            var format = new Interop.Richedit.CHARFORMAT2W
            {
                cbSize = (uint)sizeof(Interop.Richedit.CHARFORMAT2W),
                dwMask = Interop.Richedit.CFM.LINK,
                dwEffects = Interop.Richedit.CFE.LINK,
            };

            PInvoke.SendMessage(
                control,
                (Interop.User32.WM)Interop.Richedit.EM.SETCHARFORMAT,
                (WPARAM)(uint)Interop.Richedit.SCF.SELECTION,
                ref format);

            control.Select(0, 0);
        }

        private string ReportLinkClickedEventArgs(object sender, LinkClickedEventArgs e)
        {
            var control = (RichTextBox)sender;
            var prefix = control.Text.Remove(e.LinkStart);
            var content = control.Text.Substring(e.LinkStart, e.LinkLength);
            var suffix = control.Text.Substring(e.LinkStart + e.LinkLength);

            var index = prefix.LastIndexOf('\n');
            if (index >= 0)
            {
                prefix = prefix.Substring(index + 1);
            }

            index = suffix.IndexOf('\n');
            if (index >= 0)
            {
                suffix = suffix.Remove(index);
            }

            return $"LinkText: {e.LinkText}\nLinkStart: {e.LinkStart}\nLinkLength: {e.LinkLength}\n\n"
                + $"Span prefix: {prefix}\nSpan content: {content}\nSpan suffix: {suffix}";
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            MessageBox.Show(this, ReportLinkClickedEventArgs(sender, e), "link clicked");
        }

        private void richTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            MessageBox.Show(this, ReportLinkClickedEventArgs(sender, e), "link clicked");
        }
    }
}
