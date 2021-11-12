// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class RichTextBoxTests
    {
        private const int TomLink = unchecked((int)0x80000020);
        private const int TomHidden = unchecked((int)0x80000100);

        [StaFact]
        public void RichTextBox_Click_On_Friendly_Name_Link_Provides_Hidden_Link_Span()
        {
            RunTest(richTextBox =>
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

                var e = ExecuteClickOnLink(richTextBox, richTextBox.Text.IndexOf("Click link #2"));

                Assert.NotNull(e);

                Assert.True(e!.LinkStart + e.LinkLength <= richTextBox.Text.Length);
                Assert.Equal(e.LinkText, richTextBox.Text.Substring(e.LinkStart, e.LinkLength));

                // This assumes the input span is the hidden text of a "friendly name" URL,
                // which is what the native control will pass to the LinkClicked event instead
                // of the actual span of the clicked display text.
                var displayText = GetTextFromRange(richTextBox, e.LinkStart, e.LinkLength, range =>
                {
                    // Move the cursor to the end of the hidden area we are currently located in.
                    range.EndOf(TomHidden, 0);

                    // Extend the cursor to the end of the display text of the link.
                    range.EndOf(TomLink, 1);
                });

                Assert.Equal("Click link #2", displayText);
            });
        }

        [StaFact]
        public void RichTextBox_Click_On_Custom_Link_Preceeded_By_Hidden_Text_Provides_Displayed_Link_Span()
        {
            RunTest(richTextBox =>
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

                var e = ExecuteClickOnLink(richTextBox, richTextBox.Text.IndexOf("#link2#custom link"));

                Assert.NotNull(e);

                Assert.True(e!.LinkStart + e.LinkLength <= richTextBox.Text.Length);
                Assert.Equal(e.LinkText, richTextBox.Text.Substring(e.LinkStart, e.LinkLength));

                // This assumes the input span is a custom link preceeded by hidden text.
                var hiddenText = GetTextFromRange(richTextBox, e.LinkStart, e.LinkLength, range =>
                {
                    // Move the cursor to the start of the link we are currently located in.
                    range.StartOf(TomLink, 0);

                    // Extend the cursor to the start of the hidden area preceeding the link.
                    range.StartOf(TomHidden, 1);
                });

                Assert.Equal("#link2#", hiddenText);
            });
        }

        [StaFact]
        public void RichTextBox_Click_On_Custom_Link_Followed_By_Hidden_Text_Provides_Displayed_Link_Span()
        {
            RunTest(richTextBox =>
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

                var e = ExecuteClickOnLink(richTextBox, richTextBox.Text.IndexOf("custom link#link2#"));

                Assert.NotNull(e);

                Assert.True(e!.LinkStart + e.LinkLength <= richTextBox.Text.Length);
                Assert.Equal(e.LinkText, richTextBox.Text.Substring(e.LinkStart, e.LinkLength));

                // This assumes the input span is a custom link followed by hidden text.
                var hiddenText = GetTextFromRange(richTextBox, e.LinkStart, e.LinkLength, range =>
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

            SendMessageW(control, (WM)Richedit.EM.SETCHARFORMAT, (nint)Richedit.SCF.SELECTION, ref format);

            control.Select(0, 0);
        }

        private unsafe string? GetTextFromRange(RichTextBox control, int start, int length, Action<Richedit.ITextRange> transform)
        {
            IntPtr pOleInterface = IntPtr.Zero;
            object? oleInterface = null;

            try
            {
                if (SendMessageW(control, (WM)Richedit.EM.GETOLEINTERFACE, 0, ref pOleInterface) != 0 && pOleInterface != IntPtr.Zero)
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

        private LinkClickedEventArgs? ExecuteClickOnLink(RichTextBox control, int characterIndex)
        {
            Point previousPosition = new Point();
            BOOL setOldCursorPos = GetPhysicalCursorPos(ref previousPosition);

            LinkClickedEventArgs? result = null;
            LinkClickedEventHandler handler = (sender, e) => result = e;
            control.LinkClicked += handler;

            try
            {
                Point pt = control.PointToScreen(control.GetPositionFromCharIndex(characterIndex));

                // Adjust point a bit to make sure we are clicking inside the character cell instead of on its edge.
                MouseHelper.SendClick(pt.X + 2, pt.Y + 2);

                // Let the event queue drain after clicking, before moving the cursor back to the old position.
                Application.DoEvents();
            }
            finally
            {
                control.LinkClicked -= handler;

                if (setOldCursorPos.IsTrue())
                {
                    // Move cursor to old position
                    MouseHelper.ChangeMousePosition(previousPosition.X, previousPosition.Y);
                    Application.DoEvents();
                }
            }

            return result;
        }

        private void RunTest(Action<RichTextBox> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    RichTextBox richTextBox = new()
                    {
                        Parent = form,
                        Size = new Size(439, 103),
                        DetectUrls = false,
                    };
                    return richTextBox;
                },
                runTestAsync: async richTextBox =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(richTextBox);
                });
        }
    }
}
