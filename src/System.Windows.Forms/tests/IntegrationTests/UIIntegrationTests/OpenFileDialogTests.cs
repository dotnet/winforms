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

        // Regression test for https://github.com/dotnet/winforms/issues/8108
        [WinFormsFact]
        public void OpenFileDialogTests_OpenWithNonExistingInitDirectory_Success()
        {
            using DialogForm dialogOwnerForm = new(User32.WM.ENTERIDLE, User32.WM.CLOSE);
            using OpenFileDialog dialog = new();
            dialog.InitialDirectory = Guid.NewGuid().ToString();
            Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
        }

        [WinFormsFact]
        public void OpenFileDialogTests_OpenWithExistingInitDirectory_Success()
        {
            using DialogForm dialogOwnerForm = new(User32.WM.ENTERIDLE, User32.WM.CLOSE);
            using OpenFileDialog dialog = new();
            dialog.InitialDirectory = Path.GetTempPath();
            Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
        }
    }
}
