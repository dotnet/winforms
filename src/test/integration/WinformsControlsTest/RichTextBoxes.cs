// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Controls.RichEdit;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
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
\trowd\pard\intbl Test\cell Example\cell Meow\cell Woof\cell \row
}";

        // Allow setting custom links.
        richTextBox2.DetectUrls = false;

        MakeLink(richTextBox2, "#data#custom link");
        MakeLink(richTextBox2, "custom link#data#");
    }

    private unsafe void MakeLink(RichTextBox control, string text)
    {
        control.Select(control.Text.IndexOf(text, StringComparison.Ordinal), text.Length);

        var format = new Interop.Richedit.CHARFORMAT2W
        {
            cbSize = (uint)sizeof(Interop.Richedit.CHARFORMAT2W),
            dwMask = CFM_MASK.CFM_LINK,
            dwEffects = CFE_EFFECTS.CFE_LINK,
        };

        PInvokeCore.SendMessage(
            control,
            PInvokeCore.EM_SETCHARFORMAT,
            (WPARAM)PInvoke.SCF_SELECTION,
            ref format);

        control.Select(0, 0);
    }

    private string ReportLinkClickedEventArgs(object sender, LinkClickedEventArgs e)
    {
        RichTextBox control = (RichTextBox)sender;
        string prefix = control.Text[..e.LinkStart];
        string content = control.Text.Substring(e.LinkStart, e.LinkLength);
        string suffix = control.Text[(e.LinkStart + e.LinkLength)..];

        int index = prefix.LastIndexOf('\n');
        if (index >= 0)
        {
            prefix = prefix[(index + 1)..];
        }

        index = suffix.IndexOf('\n');
        if (index >= 0)
        {
            suffix = suffix[..index];
        }

        return
            $"""
                LinkText: {e.LinkText}
                LinkStart: {e.LinkStart}
                LinkLength: {e.LinkLength}

                Span prefix: {prefix}
                Span content: {content}
                Span suffix: {suffix}
                """;
    }

    private void RichTextKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is RichTextBox richTextBox)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    richTextBox.Select();
                    int location = richTextBox.Find(Prompt.ShowDialog(this, "", ""));
                    Debug.WriteLine(location.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }
    }

    private void LinkClicked(object sender, LinkClickedEventArgs e)
    {
        MessageBox.Show(this, ReportLinkClickedEventArgs(sender, e), "link clicked");
    }

    private static class Prompt
    {
        public static string ShowDialog(Form owner, string text, string caption)
        {
            Form prompt = new()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                Owner = owner,
            };
            Label textLabel = new() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
