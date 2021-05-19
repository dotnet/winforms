// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static System.Windows.Forms.Application;
using static Interop;

namespace System.Windows.Forms.Tests.HDPI
{
    public partial class ParkingWindowTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void UnawareParkingWindow()
        {
            //set process awareness
            User32.SetProcessDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.UNAWARE))
            {
                var control = new Control();
                ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
                Assert.NotNull(cxt);
                ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.UNAWARE);
                Assert.NotNull(parkingWindow);

                var dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.UNAWARE, dpiContext));
            }
        }

        [WinFormsFact]
        public void SystemAwareParkingWindow()
        {
            //set process awareness
            User32.SetProcessDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
            {
                var control = new Control();
                ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
                Assert.NotNull(cxt);
                ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE);
                Assert.NotNull(parkingWindow);

                var dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE, dpiContext));
            }
        }

        [WinFormsFact]
        public void PMv2ParkingWindow()
        {
            //set process awareness
            User32.SetProcessDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);
            var control = new Control();

            ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
            Assert.NotNull(cxt);

            ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);
            Assert.NotNull(parkingWindow);

            var dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
            Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2, dpiContext));
        }

        [WinFormsFact]
        public void MultipleParkingWindows()
        {
            //set process awareness
            User32.SetProcessDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            var control = new Control();
            ThreadContext cxt = GetContextForHandle(new HandleRef(this, control.Handle));
            Assert.NotNull(cxt);
            ParkingWindow parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);
            Assert.NotNull(parkingWindow);

            var dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
            Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2, dpiContext));

            using (DpiHelper.EnterDpiAwarenessScope(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE))
            {
                var systemControl = new Control();
                cxt = GetContextForHandle(new HandleRef(this, systemControl.Handle));
                Assert.NotNull(cxt);
                parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE);
                Assert.NotNull(parkingWindow);

                dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE, dpiContext));

                //Checking PMv2 parking window still available.
                parkingWindow = cxt.GetParkingWindowForContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);
                Assert.NotNull(parkingWindow);

                dpiContext = User32.GetWindowDpiAwarenessContext(parkingWindow.Handle);
                Assert.True(User32.AreDpiAwarenessContextsEqual(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2, dpiContext));
            }
        }
    }
}
