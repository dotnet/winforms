// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms.Primitives.Tests.Interop.User32
{
    public class GetWindowTextTests
    {
        [WinFormsFact]
        public void GetWindowText_DoesNotTruncateText()
        {
            CallGetWindowText(useBeforeGetTextLengthCallback: false);
        }

        [WinFormsFact]
        public void GetWindowText_DoesNotLoopInfinitely()
        {
            CallGetWindowText(useBeforeGetTextLengthCallback: true);
        }

        private void CallGetWindowText(bool useBeforeGetTextLengthCallback)
        {
            const string shortText = "A";

            // Use a long string that exceeds the initial buffer size (16).
            string longText = new string('X', 50);

            using var form = new ChangeWindowTextForm()
            {
                Text = shortText
            };

            // Creating the handle causes GetWindowText to be called,
            // so do it before setting the delegates.
            IntPtr formHandle = form.Handle;

            form.BeforeGetTextCallback = () => longText;
            if (useBeforeGetTextLengthCallback)
            {
                form.BeforeGetTextLengthCallback = () => shortText;
            }

            string result = GetWindowText(formHandle);
            Assert.Equal(longText, result);
        }

        private class ChangeWindowTextForm : Form
        {
            public Func<string>? BeforeGetTextLengthCallback
            {
                get;
                set;
            }

            public Func<string>? BeforeGetTextCallback
            {
                get;
                set;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WindowMessages.WM_GETTEXTLENGTH)
                {
                    string? text = BeforeGetTextLengthCallback?.Invoke();
                    if (text != null)
                    {
                        SetWindowTextW(m.HWnd, text);
                    }
                }
                else if (m.Msg == WindowMessages.WM_GETTEXT)
                {
                    string? text = BeforeGetTextCallback?.Invoke();
                    if (text != null)
                    {
                        SetWindowTextW(m.HWnd, text);
                    }
                }

                base.WndProc(ref m);
            }
        }
    }
}
