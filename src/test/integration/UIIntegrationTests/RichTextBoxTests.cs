// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Controls.RichEdit;
using Xunit.Abstractions;
using static Interop;

namespace System.Windows.Forms.UITests;

public class RichTextBoxTests : ControlTestBase
{
    public RichTextBoxTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task RichTextBox_Click_On_Friendly_Name_Link_Provides_Hidden_Link_SpanAsync()
    {
        await RunTestAsync(async (form, richTextBox) =>
        {
            richTextBox.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang4105{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 10.0.17134}\viewkind4\uc1 
{\field{\*\fldinst { HYPERLINK ""http://www.google.com"" }}{\fldrslt {Click link #1}}}
\pard\sa200\sl276\slmult1\f0\fs22\lang9  for more information.\par
{\field{\*\fldinst { HYPERLINK ""http://www.google.com"" }}{\fldrslt {Click link #2}}}
\pard\sa200\sl276\slmult1\f0\fs22\lang9  for more information.\par
{\field{\*\fldinst { HYPERLINK ""http://www.google.com"" }}{\fldrslt {Click link #3}}}
\pard\sa200\sl276\slmult1\f0\fs22\lang9  for more information.\par
}";

            LinkClickedEventArgs? result = null;
            LinkClickedEventHandler handler = (sender, e) => result = e;
            richTextBox.LinkClicked += handler;
            try
            {
                Point pt = richTextBox.PointToScreen(richTextBox.GetPositionFromCharIndex(richTextBox.Text.IndexOf("Click link #2", StringComparison.Ordinal)));

                // Adjust point a bit to make sure we are clicking inside the character cell instead of on its edge.
                pt.X += 2;
                pt.Y += 2;
                await MoveMouseAsync(form, pt);
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonClick());
            }
            finally
            {
                richTextBox.LinkClicked -= handler;
            }

            Assert.NotNull(result);

            Assert.True(result!.LinkStart + result.LinkLength <= richTextBox.Text.Length);
            Assert.Equal(result.LinkText, richTextBox.Text.Substring(result.LinkStart, result.LinkLength));

            // This assumes the input span is the hidden text of a "friendly name" URL,
            // which is what the native control will pass to the LinkClicked event instead
            // of the actual span of the clicked display text.
            string? displayText = GetTextFromRange(richTextBox, result.LinkStart, result.LinkLength, range =>
            {
                unsafe
                {
                    // Move the cursor to the end of the hidden area we are currently located in.
                    range.Value->EndOf((int)tomConstants.tomHidden, 0, out int _).ThrowOnFailure();

                    // Extend the cursor to the end of the display text of the link.
                    range.Value->EndOf((int)tomConstants.tomLink, 1, out int _).ThrowOnFailure();
                }
            });

            Assert.Equal("Click link #2", displayText);
        });
    }

    [WinFormsFact]
    public async Task RichTextBox_Click_On_Custom_Link_Preceeded_By_Hidden_Text_Provides_Displayed_Link_SpanAsync()
    {
        await RunTestAsync(async (form, richTextBox) =>
        {
            richTextBox.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang4105{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
{\*\generator Riched20 10.0.17134}\viewkind4\uc1\pard\sa200\sl276\slmult1\f0\fs22\lang9
This is hidden text preceeding a \v #link1#\v0 custom link.\par
This is hidden text preceeding a \v #link2#\v0 custom link.\par
This is hidden text preceeding a \v #link3#\v0 custom link.\par
}";

            MakeLink(richTextBox, "#link1#custom link");
            MakeLink(richTextBox, "#link2#custom link");
            MakeLink(richTextBox, "#link3#custom link");

            LinkClickedEventArgs? result = null;
            LinkClickedEventHandler handler = (sender, e) => result = e;
            richTextBox.LinkClicked += handler;
            try
            {
                Point pt = richTextBox.PointToScreen(richTextBox.GetPositionFromCharIndex(richTextBox.Text.IndexOf("#link2#custom link", StringComparison.Ordinal)));

                // Adjust point a bit to make sure we are clicking inside the character cell instead of on its edge.
                pt.X += 2;
                pt.Y += 2;
                await MoveMouseAsync(form, pt);
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonClick());
            }
            finally
            {
                richTextBox.LinkClicked -= handler;
            }

            Assert.NotNull(result);

            Assert.True(result!.LinkStart + result.LinkLength <= richTextBox.Text.Length);
            Assert.Equal(result.LinkText, richTextBox.Text.Substring(result.LinkStart, result.LinkLength));

            // This assumes the input span is a custom link preceeded by hidden text.
            string? hiddenText = GetTextFromRange(richTextBox, result.LinkStart, result.LinkLength, range =>
            {
                unsafe
                {
                    // Move the cursor to the start of the link we are currently located in.
                    range.Value->StartOf((int)tomConstants.tomLink, 0, out int _).ThrowOnFailure();

                    // Extend the cursor to the start of the hidden area preceeding the link.
                    range.Value->StartOf((int)tomConstants.tomHidden, 1, out int _).ThrowOnFailure();
                }
            });

            Assert.Equal("#link2#", hiddenText);
        });
    }

    [WinFormsFact]
    public async Task RichTextBox_Click_On_Custom_Link_Followed_By_Hidden_Text_Provides_Displayed_Link_SpanAsync()
    {
        await RunTestAsync(async (form, richTextBox) =>
        {
            // This needs to be sufficiently different from the previous test so we don't click on the same location twice,
            // otherwise the tests may execute fast enough for the second test to register as a double click.
            richTextBox.Rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang4105{\fonttbl{\f0\fnil\fcharset0 Calibri;}}
        {\*\generator Riched20 10.0.17134}\viewkind4\uc1\pard\sa200\sl276\slmult1\f0\fs22\lang9
        This is a custom link\v #link1#\v0  which is followed by hidden text.\par
        This is a custom link\v #link2#\v0  which is followed by hidden text.\par
        This is a custom link\v #link3#\v0  which is followed by hidden text.\par
        }";

            MakeLink(richTextBox, "custom link#link1#");
            MakeLink(richTextBox, "custom link#link2#");
            MakeLink(richTextBox, "custom link#link3#");

            LinkClickedEventArgs? result = null;
            LinkClickedEventHandler handler = (sender, e) => result = e;
            richTextBox.LinkClicked += handler;
            try
            {
                Point pt = richTextBox.PointToScreen(richTextBox.GetPositionFromCharIndex(
                    richTextBox.Text.IndexOf("custom link#link2#", StringComparison.Ordinal)));

                // Adjust point a bit to make sure we are clicking inside the character cell instead of on its edge.
                pt.X += 2;
                pt.Y += 2;
                await MoveMouseAsync(form, pt);
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.LeftButtonClick());
            }
            finally
            {
                richTextBox.LinkClicked -= handler;
            }

            Assert.NotNull(result);

            Assert.True(result!.LinkStart + result.LinkLength <= richTextBox.Text.Length);
            Assert.Equal(result.LinkText, richTextBox.Text.Substring(result.LinkStart, result.LinkLength));

            // This assumes the input span is a custom link followed by hidden text.
            string? hiddenText = GetTextFromRange(richTextBox, result.LinkStart, result.LinkLength, range =>
            {
                unsafe
                {
                    // Move the cursor to the end of link we are currently located in.
                    range.Value->EndOf((int)tomConstants.tomLink, 0, out int _).ThrowOnFailure();

                    // Extend the cursor to the end of the hidden area following the link.
                    range.Value->EndOf((int)tomConstants.tomHidden, 1, out int _).ThrowOnFailure();
                }
            });

            Assert.Equal("#link2#", hiddenText);
        });
    }

    private unsafe void MakeLink(RichTextBox control, string text)
    {
        control.Select(control.Text.IndexOf(text, StringComparison.Ordinal), text.Length);

        var format = new Richedit.CHARFORMAT2W
        {
            cbSize = (uint)sizeof(Richedit.CHARFORMAT2W),
            dwMask = CFM_MASK.CFM_LINK,
            dwEffects = CFE_EFFECTS.CFE_LINK,
        };

        PInvokeCore.SendMessage(control, PInvokeCore.EM_SETCHARFORMAT, (WPARAM)PInvoke.SCF_SELECTION, ref format);

        control.Select(0, 0);
    }

    private unsafe string? GetTextFromRange(RichTextBox control, int start, int length, Action<Pointer<ITextRange>>? transform)
    {
        using ComScope<IRichEditOle> richEdit = new(null);

        if (PInvokeCore.SendMessage(control, PInvokeCore.EM_GETOLEINTERFACE, 0, (void**)richEdit) != 0)
        {
            using var textDocument = richEdit.TryQuery<ITextDocument>(out HRESULT hr);

            if (hr.Succeeded)
            {
                using ComScope<ITextRange> range = new(null);
                textDocument.Value->Range(start, start + length, range).ThrowOnFailure();
                transform?.Invoke((ITextRange*)range);
                using BSTR text = default;
                range.Value->GetText(&text).ThrowOnFailure();
                return text.ToString();
            }
        }

        return null;
    }

    private async Task RunTestAsync(Func<Form, RichTextBox, Task> runTest)
    {
        await RunSingleControlTestAsync(
            testDriverAsync: runTest,
            createControl: () =>
            {
                RichTextBox control = new()
                {
                    Size = new Size(439, 103),
                    DetectUrls = false,
                };

                return control;
            },
            createForm: () =>
            {
                return new()
                {
                    Size = new(300, 300),
                    Location = new Point(100, 100),
                };
            });
    }
}
