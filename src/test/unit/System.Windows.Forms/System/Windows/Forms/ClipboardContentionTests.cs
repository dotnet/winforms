// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Microsoft.DotNet.RemoteExecutor;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Exercises real clipboard contention using the standard test STA and a native reader process.
/// </summary>
/// <remarks>
///  <para>
///   These tests share the ordinary desktop clipboard, like ClipboardTests. The child uses native clipboard
///   APIs without OLE initialization; the publisher uses the existing WinForms test-thread support.
///  </para>
/// </remarks>
[Collection("Sequential")]
[UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
public class ClipboardContentionTests
{
    private const int WaitMilliseconds = 10_000;
    private const uint QueuedMessage = 0x8011;

    [WinFormsTheory]
    [InlineData(false, 0, 0)]
    [InlineData(false, 2, 0)]
    [InlineData(true, 0, 20)]
    [InlineData(true, 2, 20)]
    public void SetDataObject_LockedClipboard_ThrowsAndRecovers(bool copy, int retryTimes, int retryDelay)
    {
        // Native contention exhausts even a zero retry budget, preserves the HRESULT, and permits recovery.
        string name = $"WinForms-Clipboard-{Guid.NewGuid():N}";
        using EventWaitHandle locked = new(false, EventResetMode.ManualReset, name + "-locked");
        using EventWaitHandle release = new(false, EventResetMode.ManualReset, name + "-release");
        using RemoteInvokeHandle child = RemoteExecutor.Invoke(HoldClipboard, name);

        try
        {
            Assert.True(locked.WaitOne(WaitMilliseconds), "The reader did not lock the clipboard.");
            DataObject replacement = new(DataFormats.UnicodeText, "replacement");
            ExternalException exception = Assert.Throws<ExternalException>(
                () => Clipboard.SetDataObject(replacement, copy, retryTimes, retryDelay));
            Assert.Equal((int)HRESULT.CLIPBRD_E_CANT_OPEN, exception.ErrorCode);
        }
        finally
        {
            release.Set();
        }

        Assert.True(child.Process.WaitForExit(WaitMilliseconds), "The clipboard lock holder did not exit.");
        Assert.Equal(RemoteExecutor.SuccessExitCode, child.ExitCode);

        try
        {
            Clipboard.SetDataObject(new DataObject(DataFormats.UnicodeText, "recovered"), copy, retryTimes: 0, retryDelay: 0);
            Assert.Equal("recovered", Clipboard.GetText());
        }
        finally
        {
            Clipboard.Clear();
        }
    }

    [WinFormsFact]
    public unsafe void SetDataObject_DelayedNativeRead_CompletesReplacementWithoutRemovingPostedMessage()
    {
        // Replacement retries service another process's delayed read while leaving posted input queued.
        const string Original = "original";
        const string Live = "changed after publication\r\n\u03a9";
        const string Replacement = "replacement";
        string name = $"WinForms-Clipboard-{Guid.NewGuid():N}";
        using EventWaitHandle locked = new(false, EventResetMode.ManualReset, name + "-locked");
        using EventWaitHandle read = new(false, EventResetMode.ManualReset, name + "-read");
        using Control messageTarget = new();
        HWND target = (HWND)messageTarget.Handle;
        DataObject original = new(DataFormats.UnicodeText, Original);

        try
        {
            Clipboard.SetDataObject(original, copy: false);
            original.SetData(DataFormats.UnicodeText, Live);
            AssertClipboardOwner();

            using RemoteInvokeHandle child = RemoteExecutor.Invoke(ReadDelayedClipboard, name, Live);
            try
            {
                Assert.True(locked.WaitOne(WaitMilliseconds), "The native reader did not lock the clipboard.");
                Assert.True(PInvokeCore.PostMessage(target, QueuedMessage, default, default));

                // Do not pump or wait between releasing the reader and entering the operation under test.
                read.Set();
                Clipboard.SetDataObject(
                    new DataObject(DataFormats.UnicodeText, Replacement),
                    copy: false,
                    retryTimes: 100,
                    retryDelay: 20);

                MSG message;
                Assert.True(PInvokeCore.PeekMessage(
                    &message, target, QueuedMessage, QueuedMessage, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE));
                AssertClipboardOwner();
                Assert.Equal(Replacement, Clipboard.GetText());
                Assert.True(child.Process.WaitForExit(WaitMilliseconds), "The delayed reader did not exit.");
                Assert.Equal(RemoteExecutor.SuccessExitCode, child.ExitCode);
            }
            finally
            {
                read.Set();
            }
        }
        finally
        {
            MSG message;
            PInvokeCore.PeekMessage(
                &message, target, QueuedMessage, QueuedMessage, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE);
            Clipboard.Clear();
        }
    }

    private static void HoldClipboard(string name)
    {
        using EventWaitHandle locked = EventWaitHandle.OpenExisting(name + "-locked");
        using EventWaitHandle release = EventWaitHandle.OpenExisting(name + "-release");
        Assert.True(OpenClipboard(HWND.Null));
        try
        {
            locked.Set();
            Assert.True(release.WaitOne(WaitMilliseconds), "The publisher did not release the reader.");
        }
        finally
        {
            Assert.True(CloseClipboard());
        }
    }

    private static unsafe void ReadDelayedClipboard(string name, string expected)
    {
        using EventWaitHandle locked = EventWaitHandle.OpenExisting(name + "-locked");
        using EventWaitHandle read = EventWaitHandle.OpenExisting(name + "-read");
        Assert.True(OpenClipboard(HWND.Null));
        try
        {
            locked.Set();
            Assert.True(read.WaitOne(WaitMilliseconds), "The publisher did not request the delayed read.");
            nint data = GetClipboardData((uint)CLIPBOARD_FORMAT.CF_UNICODETEXT);
            Assert.NotEqual(nint.Zero, data);
            void* buffer = PInvokeCore.GlobalLock((HGLOBAL)data);
            Assert.NotEqual(nint.Zero, (nint)buffer);
            try
            {
                Assert.Equal(expected, Marshal.PtrToStringUni((nint)buffer));
            }
            finally
            {
                PInvokeCore.GlobalUnlock((HGLOBAL)data);
            }
        }
        finally
        {
            Assert.True(CloseClipboard());
        }
    }

    private static unsafe void AssertClipboardOwner()
    {
        HWND owner = GetClipboardOwner();
        Assert.NotEqual(HWND.Null, owner);
        uint processId;
        Assert.NotEqual(0u, PInvokeCore.GetWindowThreadProcessId(owner, &processId));
        Assert.Equal((uint)Environment.ProcessId, processId);
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool OpenClipboard(HWND owner);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseClipboard();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint GetClipboardData(uint format);

    [DllImport("user32.dll")]
    private static extern HWND GetClipboardOwner();
}
