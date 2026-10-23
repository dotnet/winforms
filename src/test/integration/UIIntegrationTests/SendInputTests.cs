// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.UITests;

/// <summary>
///  Tests foreground validation with synthetic handles, without activating windows.
/// </summary>
public class SendInputTests
{
    [Fact]
    public void VerifyForegroundWindow_RequestedWindowIsForeground_DoesNotThrow()
    {
        var window = (HWND)(nint)1;
        uint processId = (uint)Environment.ProcessId;

        SendInput.VerifyForegroundWindow(window, processId, window, processId);
    }

    [Theory]
    [InlineData(1, 2, true)]
    [InlineData(1, 2, false)]
    [InlineData(1, 0, false)]
    [InlineData(0, 0, true)]
    [InlineData(1, 1, false)]
    public void VerifyForegroundWindow_UnexpectedForeground_Throws(int requested, int foreground, bool sameProcess)
    {
        var requestedWindow = (HWND)requested;
        var foregroundWindow = (HWND)foreground;
        uint requestedProcessId = (uint)Environment.ProcessId;
        uint foregroundProcessId = sameProcess ? requestedProcessId : requestedProcessId + 1;

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => SendInput.VerifyForegroundWindow(requestedWindow, requestedProcessId, foregroundWindow, foregroundProcessId));

        Assert.Contains($"requested HWND: {requestedWindow}, PID: {requestedProcessId}", exception.Message);
        Assert.Contains($"actual foreground HWND: {foregroundWindow}, PID: {foregroundProcessId}", exception.Message);
        Assert.Contains($"test PID: {Environment.ProcessId}", exception.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void VerifyForegroundWindow_ProcessLookupFailed_Throws(bool requestedProcessLookupFailed)
    {
        var window = (HWND)(nint)1;
        uint processId = (uint)Environment.ProcessId;

        Assert.Throws<InvalidOperationException>(() => SendInput.VerifyForegroundWindow(
            window,
            requestedProcessLookupFailed ? 0 : processId,
            window,
            requestedProcessLookupFailed ? processId : 0));
    }
}
