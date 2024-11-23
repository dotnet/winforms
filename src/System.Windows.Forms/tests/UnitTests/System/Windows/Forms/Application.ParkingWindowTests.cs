// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DotNet.RemoteExecutor;
using static System.Windows.Forms.Application;

namespace System.Windows.Forms.Tests;

public class ParkingWindowTests
{
    [WinFormsFact(Skip = "Crash with AbandonedMutexException. See: https://github.com/dotnet/arcade/issues/5325")]
    public void ParkingWindow_DoesNotThrowOnGarbageCollecting()
    {
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
        {
            Control.CheckForIllegalCrossThreadCalls = true;

            Form form = InitFormWithControlToGarbageCollect();

            try
            {
                // Force garbage collecting to access ComboBox from another (GC) thread.
                GC.Collect();

                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                Assert.True(ex is null, $"Expected no exception, but got: {ex.Message}"); // Actually need to check whether GC.Collect() does not throw exception.
            }
        });

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }

    private static Form InitFormWithControlToGarbageCollect()
    {
        Form form = new();
        ComboBox comboBox = new()
        {
            DropDownStyle = ComboBoxStyle.DropDown
        };

        form.Controls.Add(comboBox);
        form.Show();

        // Park ComboBox handle in ParkingWindow.
        comboBox.Parent = null;

        // Recreate ComboBox handle to set parent to ParkingWindow.
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        return form;
    }

    [WinFormsFact]
    public void ParkingWindow_Unaware()
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        // set thread awareness context to PermonitorV2(PMv2).
        // if process/thread is not in PMv2, calling 'EnterDpiAwarenessScope' is a no-op and that is by design.
        // In this case, we will be setting thread to PMv2 mode and then scope to UNAWARE
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE))
            {
                using Control control = new();
                ThreadContext ctx = GetContextForHandle(control);
                Assert.NotNull(ctx);
                ParkingWindow parkingWindow = ctx.TestAccessor().Dynamic.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE);
                Assert.NotNull(parkingWindow);

                DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                Assert.True(dpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE));
            }
        }
        finally
        {
            // reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    [WinFormsFact]
    public void ParkingWindow_SystemAware()
    {
        // run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        // set thread awareness context to PermonitorV2(PMv2).
        // if process/thread is not in PMv2, calling 'EnterDpiAwarenessScope' is a no-op and that is by design.
        // In this case, we will be setting thread to PMv2 mode and then scope to UNAWARE
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
            {
                using Control control = new();
                ThreadContext ctx = GetContextForHandle(control);
                Assert.NotNull(ctx);
                ParkingWindow parkingWindow = ctx.TestAccessor().Dynamic.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE);
                Assert.NotNull(parkingWindow);

                DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                Assert.True(dpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE));
            }
        }
        finally
        {
            // reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    [WinFormsFact]
    public void ParkingWindow_PermonitorV2()
    {
        // run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        // set thread awareness context to PermonitorV2(PMv2).
        // if process/thread is not in PMv2, calling 'EnterDpiAwarenessScope' is a no-op and that is by design.
        // In this case, we will be setting thread to PMv2 mode and then scope to UNAWARE
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using Control control = new();
            ThreadContext ctx = GetContextForHandle(control);
            Assert.NotNull(ctx);

            ParkingWindow parkingWindow = ctx.TestAccessor().Dynamic.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            Assert.NotNull(parkingWindow);

            DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
            Assert.True(dpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2));
        }
        finally
        {
            // reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    [WinFormsFact]
    public void ParkingWindow_Multiple()
    {
        // run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        // set thread awareness context to PermonitorV2(PMv2).
        // if process/thread is not in PMv2, calling 'EnterDpiAwarenessScope' is a no-op and that is by design.
        // In this case, we will be setting thread to PMv2 mode and then scope to UNAWARE
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using Control control = new();
            ThreadContext ctx = GetContextForHandle(control);
            Assert.NotNull(ctx);
            ParkingWindow parkingWindow = ctx.TestAccessor().Dynamic.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            Assert.NotNull(parkingWindow);

            DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
            Assert.True(dpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2));

            using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
            {
                using Control systemControl = new();
                ctx = GetContextForHandle(systemControl);
                Assert.NotNull(ctx);
                parkingWindow = ctx.TestAccessor().Dynamic.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE);
                Assert.NotNull(parkingWindow);

                dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                Assert.True(dpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE));

                // check PMv2 parking window still available.
                parkingWindow = ctx.TestAccessor().Dynamic.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                Assert.NotNull(parkingWindow);

                dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                Assert.True(dpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2));
            }
        }
        finally
        {
            // reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }
}
