// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;
using static System.Windows.Forms.Application;
using static Interop;

namespace System.Windows.Forms.Tests
{
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
                    // Force garbage collecting to access combobox from another (GC) thread.
                    GC.Collect();

                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {
                    Assert.True(ex is null, "Expected no exception, but got: " + ex.Message); // Actually need to check whether GC.Collect() does not throw exception.
                }
            });

            // verify the remote process succeeded
            Assert.Equal(0, invokerHandle.ExitCode);
        }

        private Form InitFormWithControlToGarbageCollect()
        {
            Form form = new Form();
            ComboBox comboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;

            form.Controls.Add(comboBox);
            form.Show();

            // Park combobox handle in ParkingWindow.
            comboBox.Parent = null;

            // Recreate combobox handle to set parent to ParkingWindow.
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Lose the reference to combobox to allow Garbage collecting combobox.
            comboBox = null;

            return form;
        }

        [WinFormsFact]
        public void ParkingWindow_Unaware()
        {
            // run tests only on Windows 10 versions that support thread dpi awareness.
            if (!PlatformDetection.IsWindows10Version1803OrGreater)
            {
                return;
            }

            // set thread awareness context to PermonitorV2(PMv2).
            // if process/thread is not in PMv2, calling 'EnterDpiAwarenessScope' is a no-op and that is by design.
            // In this case, we will be setting thread to PMv2 mode and then scope to UNAWARE
            IntPtr originalAwarenessContext = User32.SetThreadDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            try
            {
                using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.UNAWARE))
                {
                    using var control = new Control();
                    ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
                    Assert.NotNull(cxt);
                    ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.UNAWARE);
                    Assert.NotNull(parkingWindow);

                    IntPtr dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                    Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.UNAWARE, dpiContext));
                }
            }
            finally
            {
                // reset back to original awareness context.
                User32.SetThreadDpiAwarenessContext(originalAwarenessContext);
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
            IntPtr originalAwarenessContext = User32.SetThreadDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            try
            {
                using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
                {
                    using var control = new Control();
                    ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
                    Assert.NotNull(cxt);
                    ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE);
                    Assert.NotNull(parkingWindow);

                    IntPtr dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                    Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE, dpiContext));
                }
            }
            finally
            {
                // reset back to original awareness context.
                User32.SetThreadDpiAwarenessContext(originalAwarenessContext);
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
            IntPtr originalAwarenessContext = User32.SetThreadDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            try
            {
                using var control = new Control();
                ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
                Assert.NotNull(cxt);

                ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);
                Assert.NotNull(parkingWindow);

                IntPtr dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2, dpiContext));
            }
            finally
            {
                // reset back to original awareness context.
                User32.SetThreadDpiAwarenessContext(originalAwarenessContext);
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
            IntPtr originalAwarenessContext = User32.SetThreadDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            try
            {
                using var control = new Control();
                ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
                Assert.NotNull(cxt);
                ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);
                Assert.NotNull(parkingWindow);

                IntPtr dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2, dpiContext));

                using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
                {
                    using var systemControl = new Control();
                    cxt = GetContextForHandle(new HandleRef(this, systemControl.Handle));
                    Assert.NotNull(cxt);
                    parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE);
                    Assert.NotNull(parkingWindow);

                    dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                    Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE, dpiContext));

                    // check PMv2 parking window still available.
                    parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);
                    Assert.NotNull(parkingWindow);

                    dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                    Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2, dpiContext));
                }
            }
            finally
            {
                // reset back to original awareness context.
                User32.SetThreadDpiAwarenessContext(originalAwarenessContext);
            }
        }
    }
}
