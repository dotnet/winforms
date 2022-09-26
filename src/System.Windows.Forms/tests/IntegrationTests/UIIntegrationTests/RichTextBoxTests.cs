// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.UITests
{
    public class RichTextBoxTests : ControlTestBase
    {
        private const int TomLink = unchecked((int)0x80000020);
        private const int TomHidden = unchecked((int)0x80000100);
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

                Point previousPosition = new();
                BOOL setOldCursorPos = GetPhysicalCursorPos(ref previousPosition);

                LinkClickedEventArgs? result = null;
                LinkClickedEventHandler handler = (sender, e) => result = e;
                richTextBox.LinkClicked += handler;
                try
                {
                    Point pt = richTextBox.PointToScreen(richTextBox.GetPositionFromCharIndex(richTextBox.Text.IndexOf("Click link #2")));

                    // Adjust point a bit to make sure we are clicking inside the character cell instead of on its edge.
                    pt.X += 2;
                    pt.Y += 2;
                    await MoveMouseAsync(form, pt);
                    await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Mouse.LeftButtonClick());
                    if (setOldCursorPos)
                    {
                        await MoveMouseAsync(form, previousPosition);
                    }

                    // Let the event queue drain after clicking, before moving the cursor back to the old position.
                    Application.DoEvents();
                }
                finally
                {
                    richTextBox.LinkClicked -= handler;

                    if (setOldCursorPos)
                    {
                        // Move cursor to the old position.
                        await InputSimulator.SendAsync(
                            form,
                            inputSimulator => inputSimulator.Mouse.MoveMouseTo(previousPosition.X, previousPosition.Y));
                        Application.DoEvents();
                    }
                }

                Assert.NotNull(result);

                Assert.True(result!.LinkStart + result.LinkLength <= richTextBox.Text.Length);
                Assert.Equal(result.LinkText, richTextBox.Text.Substring(result.LinkStart, result.LinkLength));

                // This assumes the input span is the hidden text of a "friendly name" URL,
                // which is what the native control will pass to the LinkClicked event instead
                // of the actual span of the clicked display text.
                var displayText = GetTextFromRange(richTextBox, result.LinkStart, result.LinkLength, range =>
                {
                    // Move the cursor to the end of the hidden area we are currently located in.
                    range.EndOf(TomHidden, 0);

                    // Extend the cursor to the end of the display text of the link.
                    range.EndOf(TomLink, 1);
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

                Point previousPosition = new();
                BOOL setOldCursorPos = GetPhysicalCursorPos(ref previousPosition);

                LinkClickedEventArgs? result = null;
                LinkClickedEventHandler handler = (sender, e) => result = e;
                richTextBox.LinkClicked += handler;
                try
                {
                    Point pt = richTextBox.PointToScreen(richTextBox.GetPositionFromCharIndex(richTextBox.Text.IndexOf("#link2#custom link")));

                    // Adjust point a bit to make sure we are clicking inside the character cell instead of on its edge.
                    pt.X += 2;
                    pt.Y += 2;
                    await MoveMouseAsync(form, pt);
                    await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Mouse.LeftButtonClick());
                    if (setOldCursorPos)
                    {
                        await MoveMouseAsync(form, previousPosition);
                    }

                    // Let the event queue drain after clicking, before moving the cursor back to the old position.
                    Application.DoEvents();
                }
                finally
                {
                    richTextBox.LinkClicked -= handler;

                    if (setOldCursorPos)
                    {
                        // Move cursor to the old position.
                        await InputSimulator.SendAsync(
                            form,
                            inputSimulator => inputSimulator.Mouse.MoveMouseTo(previousPosition.X, previousPosition.Y));
                        Application.DoEvents();
                    }
                }

                Assert.NotNull(result);

                Assert.True(result!.LinkStart + result.LinkLength <= richTextBox.Text.Length);
                Assert.Equal(result.LinkText, richTextBox.Text.Substring(result.LinkStart, result.LinkLength));

                // This assumes the input span is a custom link preceeded by hidden text.
                var hiddenText = GetTextFromRange(richTextBox, result.LinkStart, result.LinkLength, range =>
                {
                    // Move the cursor to the start of the link we are currently located in.
                    range.StartOf(TomLink, 0);

                    // Extend the cursor to the start of the hidden area preceeding the link.
                    range.StartOf(TomHidden, 1);
                });

                Assert.Equal("#link2#", hiddenText);
            });
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/6609")]
        [WinFormsFact(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/6609")]
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

                Point previousPosition = new();
                BOOL setOldCursorPos = GetPhysicalCursorPos(ref previousPosition);

                LinkClickedEventArgs? result = null;
                LinkClickedEventHandler handler = (sender, e) => result = e;
                richTextBox.LinkClicked += handler;
                try
                {
                    Point pt = richTextBox.PointToScreen(richTextBox.GetPositionFromCharIndex(richTextBox.Text.IndexOf("custom link#link2#")));

                    // Adjust point a bit to make sure we are clicking inside the character cell instead of on its edge.
                    pt.X += 2;
                    pt.Y += 2;
                    await MoveMouseAsync(form, pt);
                    await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Mouse.LeftButtonClick());
                    if (setOldCursorPos)
                    {
                        await MoveMouseAsync(form, previousPosition);
                    }

                    // Let the event queue drain after clicking, before moving the cursor back to the old position.
                    Application.DoEvents();
                }
                finally
                {
                    richTextBox.LinkClicked -= handler;

                    if (setOldCursorPos)
                    {
                        // Move cursor to the old position.
                        await InputSimulator.SendAsync(
                            form,
                            inputSimulator => inputSimulator.Mouse.MoveMouseTo(previousPosition.X, previousPosition.Y));
                        Application.DoEvents();
                    }
                }

                Assert.NotNull(result);

                Assert.True(result!.LinkStart + result.LinkLength <= richTextBox.Text.Length);
                Assert.Equal(result.LinkText, richTextBox.Text.Substring(result.LinkStart, result.LinkLength));

                // This assumes the input span is a custom link followed by hidden text.
                var hiddenText = GetTextFromRange(richTextBox, result.LinkStart, result.LinkLength, range =>
                {
                    // Move the cursor to the end of link we are currently located in.
                    range.EndOf(TomLink, 0);

                    // Extend the cursor to the end of the hidden area following the link.
                    range.EndOf(TomHidden, 1);
                });

                Assert.Equal("#link2#", hiddenText);
            });
        }

        private unsafe void MakeLink(RichTextBox control, string text)
        {
            control.Select(control.Text.IndexOf(text), text.Length);

            var format = new Richedit.CHARFORMAT2W
            {
                cbSize = (uint)sizeof(Richedit.CHARFORMAT2W),
                dwMask = Richedit.CFM.LINK,
                dwEffects = Richedit.CFE.LINK,
            };

            PInvoke.SendMessage(control, (WM)Richedit.EM.SETCHARFORMAT, (WPARAM)(uint)Richedit.SCF.SELECTION, ref format);

            control.Select(0, 0);
        }

        private unsafe string? GetTextFromRange(RichTextBox control, int start, int length, Action<Richedit.ITextRange> transform)
        {
            IntPtr pOleInterface = IntPtr.Zero;
            object? oleInterface = null;

            try
            {
                if (PInvoke.SendMessage(control, (WM)Richedit.EM.GETOLEINTERFACE, 0, ref pOleInterface) != 0 && pOleInterface != IntPtr.Zero)
                {
                    // This increments the RCW reference count, further casts do not increment it. It is important
                    // to capture the initial reference to the RCW so we can release it even if casts fail.
                    oleInterface = Marshal.GetObjectForIUnknown(pOleInterface);

                    if (oleInterface is Richedit.ITextDocument textDocument)
                    {
                        // This method returns a COM object, thus increments the RCW reference count and we want to release it later.
                        var range = textDocument.Range(start, start + length);
                        if (range != null)
                        {
                            try
                            {
                                transform?.Invoke(range);
                                return range.GetText();
                            }
                            finally
                            {
                                // release RCW reference count
                                Marshal.ReleaseComObject(range);
                            }
                        }
                    }
                }

                return null;
            }
            finally
            {
                // release RCW reference count
                if (oleInterface != null)
                {
                    Marshal.ReleaseComObject(oleInterface);
                }

                // release COM reference count
                if (pOleInterface != IntPtr.Zero)
                {
                    Marshal.Release(pOleInterface);
                }
            }
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
}
