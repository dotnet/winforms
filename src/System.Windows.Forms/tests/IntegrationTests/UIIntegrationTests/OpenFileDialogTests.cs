// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;
using static Interop;

namespace System.Windows.Forms.UITests
{
    public class OpenFileDialogTests : ControlTestBase
    {
        public OpenFileDialogTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public void OpenFileDialogTests_OpenWithNonExistingInitDirectory_Success()
        {
            using TestDialogForm dialogOwnerForm = new();
            using OpenFileDialog dialog = new();
            dialog.InitialDirectory = Guid.NewGuid().ToString();
            Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
        }

        [WinFormsFact]
        public void OpenFileDialogTests_OpenWithExistingInitDirectory_Success()
        {
            using TestDialogForm dialogOwnerForm = new();
            using OpenFileDialog dialog = new();
            dialog.InitialDirectory = Path.GetTempPath();
            Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
        }

        private class TestDialogForm : Form
        {
            private const int MSGF_DIALOGBOX = 0;

            protected override void WndProc(ref Message m)
            {
                if (m.MsgInternal == User32.WM.ENTERIDLE && m.WParamInternal == MSGF_DIALOGBOX)
                {
                    HWND dialogHandle = (HWND)m.LParamInternal;
                    PInvoke.PostMessage(dialogHandle, User32.WM.CLOSE);
                }

                base.WndProc(ref m);
            }
        }
    }
}
