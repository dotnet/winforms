// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;


namespace System.Windows.Forms.Tests
{
    public class CommonDialogTests
    {
        [Fact]
        public void Ctor_Default()
        {
            var dialog = new SubCommonDialog();
            Assert.True(dialog.CanRaiseEvents);
            Assert.Null(dialog.Container);
            Assert.False(dialog.DesignMode);
            Assert.NotNull(dialog.Events);
            Assert.Same(dialog.Events, dialog.Events);
            Assert.Null(dialog.Container);
            Assert.Null(dialog.Site);
            Assert.Null(dialog.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void Tag_Set_GetReturnsExpected(object value)
        {
            var dialog = new SubCommonDialog()
            {
                Tag = value
            };
            Assert.Same(value, dialog.Tag);

            // Set same.
            dialog.Tag = value;
            Assert.Same(value, dialog.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void OnHelpRequest_Invoke_CallsHelpRequest(EventArgs eventArgs)
        {
            var dialog = new SubCommonDialog();

            // No handler.
            dialog.OnHelpRequest(eventArgs);

            // Handler.
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(dialog, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            dialog.HelpRequest += handler;
            dialog.OnHelpRequest(eventArgs);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            dialog.HelpRequest -= handler;
            dialog.OnHelpRequest(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> HookProc_TestData()
        {
            yield return new object[] { WindowMessages.WM_INITDIALOG };
            yield return new object[] { WindowMessages.WM_SETFOCUS };

            const int CDM_SETDEFAULTFOCUS = WindowMessages.WM_USER + 0x51;
            yield return new object[] { CDM_SETDEFAULTFOCUS };

            yield return new object[] { 0 };
            yield return new object[] { -1 };
        }

        [Theory]
        [MemberData(nameof(HookProc_TestData))]
        public void HookProc_Invoke_ReturnsZero(int msg)
        {
            var dialog = new SubCommonDialog();
            Assert.Equal(IntPtr.Zero, dialog.HookProc(IntPtr.Zero, msg, IntPtr.Zero, IntPtr.Zero));
        }

        [Theory]
        [InlineData(true, DialogResult.OK)]
        [InlineData(false, DialogResult.Cancel)]
        public void ShowDialog_NoOwner_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
        {
            var dialog = new SubCommonDialog
            {
                RunDialogResult = runDialogResult
            };
            Assert.Equal(expectedDialogResult, dialog.ShowDialog());
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(null));
        }

        [Theory]
        [InlineData(true, DialogResult.OK)]
        [InlineData(false, DialogResult.Cancel)]
        public void ShowDialog_NonControlOwner_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
        {
            var dialog = new SubCommonDialog
            {
                RunDialogResult = runDialogResult
            };
            var owner = new Mock<IWin32Window>(MockBehavior.Strict);
            owner
                .Setup(o => o.Handle)
                .Returns(IntPtr.Zero);
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner.Object));
        }

        [Theory]
        [InlineData(true, DialogResult.OK)]
        [InlineData(false, DialogResult.Cancel)]
        public void ShowDialog_ControlOwner_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
        {
            var dialog = new SubCommonDialog
            {
                RunDialogResult = runDialogResult
            };
            var owner = new Control();
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
        }

        [Theory]
        [InlineData(true, DialogResult.OK)]
        [InlineData(false, DialogResult.Cancel)]
        public void ShowDialog_ControlOwnerWithHandle_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
        {
            var dialog = new SubCommonDialog
            {
                RunDialogResult = runDialogResult
            };
            var owner = new Control();
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
        }

        [Fact]
        public void ShowDialog_NonControlOwnerWithHandle_ThrowsWin32Exception()
        {
            var dialog = new SubCommonDialog();
            var owner = new Mock<IWin32Window>(MockBehavior.Strict);
            owner
                .Setup(o => o.Handle)
                .Returns((IntPtr)1);
            Assert.Throws<Win32Exception>(() => dialog.ShowDialog(owner.Object));
        }

        [Fact]
        public void OwnerWndProc_HelpMessage_CallsHelpRequest()
        {
            var dialog = new SubCommonDialog();
            FieldInfo field = typeof(CommonDialog).GetField("s_helpMsg", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(field);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(dialog, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            dialog.HelpRequest += handler;
            Assert.Equal(IntPtr.Zero, dialog.OwnerWndProc(IntPtr.Zero, (int)field.GetValue(null), IntPtr.Zero, IntPtr.Zero));
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void OwnerWndProc_NonHelpMessage_DoesNotCallHelpRequest()
        {
            var dialog = new SubCommonDialog();
            FieldInfo field = typeof(CommonDialog).GetField("s_helpMsg", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(field);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(dialog, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            dialog.HelpRequest += handler;
            Assert.Equal(IntPtr.Zero, dialog.OwnerWndProc(IntPtr.Zero, (int)field.GetValue(null) + 1, IntPtr.Zero, IntPtr.Zero));
            Assert.Equal(0, callCount);
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
