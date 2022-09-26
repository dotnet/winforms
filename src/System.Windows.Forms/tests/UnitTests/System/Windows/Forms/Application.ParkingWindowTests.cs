// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.Application;

namespace System.Windows.Forms.Tests
{
    public class ParkingWindowTests
    {
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
            DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

            try
            {
                using (DpiHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE))
                {
                    using Control control = new();
                    ThreadContext ctx = GetContextForHandle(control);
                    Assert.NotNull(ctx);
                    ParkingWindow parkingWindow = ctx.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE);
                    Assert.NotNull(parkingWindow);

                    DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                    Assert.True(PInvoke.AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE, dpiContext));
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
                using (DpiHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                {
                    using Control control = new();
                    ThreadContext ctx = GetContextForHandle(control);
                    Assert.NotNull(ctx);
                    ParkingWindow parkingWindow = ctx.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE);
                    Assert.NotNull(parkingWindow);

                    DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                    Assert.True(PInvoke.AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE, dpiContext));
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

                ParkingWindow parkingWindow = ctx.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                Assert.NotNull(parkingWindow);

                DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                Assert.True(PInvoke.AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2, dpiContext));
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
                ParkingWindow parkingWindow = ctx.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                Assert.NotNull(parkingWindow);

                DPI_AWARENESS_CONTEXT dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                Assert.True(PInvoke.AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2, dpiContext));

                using (DpiHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
                {
                    using Control systemControl = new();
                    ctx = GetContextForHandle(systemControl);
                    Assert.NotNull(ctx);
                    parkingWindow = ctx.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE);
                    Assert.NotNull(parkingWindow);

                    dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                    Assert.True(PInvoke.AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE, dpiContext));

                    // check PMv2 parking window still available.
                    parkingWindow = ctx.GetParkingWindowForContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                    Assert.NotNull(parkingWindow);

                    dpiContext = PInvoke.GetWindowDpiAwarenessContext(parkingWindow.HWND);
                    Assert.True(PInvoke.AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2, dpiContext));
                }
            }
            finally
            {
                // reset back to original awareness context.
                PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
            }
        }
    }
}
