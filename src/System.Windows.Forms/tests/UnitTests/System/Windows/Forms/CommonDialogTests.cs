// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;
using Microsoft.DotNet.RemoteExecutor;
using Moq;

namespace System.Windows.Forms.Tests;

public class CommonDialogTests
{
    [WinFormsFact]
    public void Ctor_Default()
    {
        using SubCommonDialog dialog = new();
        Assert.True(dialog.CanRaiseEvents);
        Assert.Null(dialog.Container);
        Assert.False(dialog.DesignMode);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.Null(dialog.Container);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void Tag_Set_GetReturnsExpected(object value)
    {
        using SubCommonDialog dialog = new()
        {
            Tag = value
        };
        Assert.Same(value, dialog.Tag);

        // Set same.
        dialog.Tag = value;
        Assert.Same(value, dialog.Tag);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void OnHelpRequest_Invoke_CallsHelpRequest(EventArgs eventArgs)
    {
        using SubCommonDialog dialog = new();

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
        yield return new object[] { PInvokeCore.WM_INITDIALOG };
        yield return new object[] { PInvokeCore.WM_SETFOCUS };

        const int CDM_SETDEFAULTFOCUS = (int)PInvokeCore.WM_USER + 0x51;
        yield return new object[] { CDM_SETDEFAULTFOCUS };

        yield return new object[] { 0 };
        yield return new object[] { -1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(HookProc_TestData))]
    public void HookProc_Invoke_ReturnsZero(int msg)
    {
        using SubCommonDialog dialog = new();
        Assert.Equal(IntPtr.Zero, dialog.HookProc(IntPtr.Zero, msg, IntPtr.Zero, IntPtr.Zero));
    }

    [WinFormsTheory]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_NoOwner_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
    {
        using SubCommonDialog dialog = new()
        {
            RunDialogResult = runDialogResult
        };
        Assert.Equal(expectedDialogResult, dialog.ShowDialog());
        Assert.Equal(expectedDialogResult, dialog.ShowDialog(null));
    }

    [WinFormsTheory]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_NonControlOwner_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
    {
        using SubCommonDialog dialog = new()
        {
            RunDialogResult = runDialogResult
        };
        var owner = new Mock<IWin32Window>(MockBehavior.Strict);
        owner
            .Setup(o => o.Handle)
            .Returns(IntPtr.Zero);
        Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner.Object));
    }

    [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_NonControlOwnerWithVisualStyles_ReturnsExpected(bool runDialogResultParam, DialogResult expectedDialogResultParam)
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke((runDialogResultString, expectedDialogResultString) =>
        {
            bool runDialogResult = bool.Parse(runDialogResultString);
            DialogResult expectedDialogResult = (DialogResult)Enum.Parse(typeof(DialogResult), expectedDialogResultString);

            Application.EnableVisualStyles();

            using SubCommonDialog dialog = new()
            {
                RunDialogResult = runDialogResult
            };
            var owner = new Mock<IWin32Window>(MockBehavior.Strict);
            owner
                .Setup(o => o.Handle)
                .Returns(IntPtr.Zero);
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner.Object));
        }, runDialogResultParam.ToString(), expectedDialogResultParam.ToString()).Dispose();
    }

    [WinFormsTheory]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_ControlOwner_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
    {
        using SubCommonDialog dialog = new()
        {
            RunDialogResult = runDialogResult
        };
        using Control owner = new();
        Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
    }

    [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_ControlOwnerWithVisualStyles_ReturnsExpected(bool runDialogResultParam, DialogResult expectedDialogResultParam)
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke((runDialogResultString, expectedDialogResultString) =>
        {
            bool runDialogResult = bool.Parse(runDialogResultString);
            DialogResult expectedDialogResult = (DialogResult)Enum.Parse(typeof(DialogResult), expectedDialogResultString);

            Application.EnableVisualStyles();

            using SubCommonDialog dialog = new()
            {
                RunDialogResult = runDialogResult
            };
            using Control owner = new();
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
        }, runDialogResultParam.ToString(), expectedDialogResultParam.ToString()).Dispose();
    }

    [WinFormsTheory]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_ControlOwnerWithHandle_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
    {
        using SubCommonDialog dialog = new()
        {
            RunDialogResult = runDialogResult
        };
        using Control owner = new();
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
    }

    [WinFormsTheory(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_ControlOwnerWithHandleWithVisualStyles_ReturnsExpected(bool runDialogResultParam, DialogResult expectedDialogResultParam)
    {
        // Run this from another thread as we call Application.EnableVisualStyles.
        RemoteExecutor.Invoke((runDialogResultString, expectedDialogResultString) =>
        {
            bool runDialogResult = bool.Parse(runDialogResultString);
            DialogResult expectedDialogResult = (DialogResult)Enum.Parse(typeof(DialogResult), expectedDialogResultString);

            Application.EnableVisualStyles();

            using SubCommonDialog dialog = new()
            {
                RunDialogResult = runDialogResult
            };
            using Control owner = new();
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
        }, runDialogResultParam.ToString(), expectedDialogResultParam.ToString()).Dispose();
    }

    [WinFormsFact]
    public void ShowDialog_NonControlOwnerWithHandle_ThrowsWin32Exception()
    {
        using SubCommonDialog dialog = new();
        var owner = new Mock<IWin32Window>(MockBehavior.Strict);
        owner
            .Setup(o => o.Handle)
            .Returns(1);
        Assert.Throws<Win32Exception>(() => dialog.ShowDialog(owner.Object));
    }

    [WinFormsFact]
    public void OwnerWndProc_HelpMessage_CallsHelpRequest()
    {
        using SubCommonDialog dialog = new();
        FieldInfo field = typeof(CommonDialog).GetField("s_helpMessage", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(field);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(dialog, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        dialog.HelpRequest += handler;
        Assert.Equal(IntPtr.Zero, dialog.OwnerWndProc(IntPtr.Zero, (int)(MessageId)field.GetValue(null), IntPtr.Zero, IntPtr.Zero));
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void OwnerWndProc_NonHelpMessage_DoesNotCallHelpRequest()
    {
        using SubCommonDialog dialog = new();
        FieldInfo field = typeof(CommonDialog).GetField("s_helpMessage", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(field);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(dialog, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        dialog.HelpRequest += handler;
        Assert.Equal(IntPtr.Zero, dialog.OwnerWndProc(IntPtr.Zero, (int)(MessageId)field.GetValue(null) + 1, IntPtr.Zero, IntPtr.Zero));
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
