// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class CommonDialogTests : ControlTestBase
    {
        public CommonDialogTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsTheory]
        [InlineData(true, DialogResult.OK)]
        [InlineData(false, DialogResult.Cancel)]
        public void ShowDialog_NonControlOwnerWithVisualStyles_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
        {
            using var dialog = new SubCommonDialog
            {
                RunDialogResult = runDialogResult
            };
            var owner = new Mock<IWin32Window>(MockBehavior.Strict);
            owner
                .Setup(o => o.Handle)
                .Returns(IntPtr.Zero);
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner.Object));
        }

        [WinFormsTheory]
        [InlineData(true, DialogResult.OK)]
        [InlineData(false, DialogResult.Cancel)]
        public void ShowDialog_ControlOwnerWithVisualStyles_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
        {
            using var dialog = new SubCommonDialog
            {
                RunDialogResult = runDialogResult
            };
            using var owner = new Control();
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
        }

        [WinFormsTheory]
        [InlineData(true, DialogResult.OK)]
        [InlineData(false, DialogResult.Cancel)]
        public void ShowDialog_ControlOwnerWithHandleWithVisualStyles_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
        {
            using var dialog = new SubCommonDialog
            {
                RunDialogResult = runDialogResult
            };
            using var owner = new Control();
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
        }

        private class SubCommonDialog : CommonDialog
        {
            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public override void Reset()
            {
            }

            public bool RunDialogResult { get; set; }

            protected override bool RunDialog(IntPtr hwndOwner) => RunDialogResult;

            public new void OnHelpRequest(EventArgs e) => base.OnHelpRequest(e);

            public new IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
            {
                return base.HookProc(hWnd, msg, wparam, lparam);
            }

            public new IntPtr OwnerWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
            {
                return base.OwnerWndProc(hWnd, msg, wparam, lparam);
            }
        }
    }
}
