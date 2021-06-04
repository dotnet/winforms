// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.DPI
{
    public class FormDpiTests : IClassFixture<ThreadExceptionFixture>
    {
        private IntPtr TriggerDpiMessage(User32.WM MSG, Control control, int newDpi)
        {
            double factor = newDpi / DpiHelper.LogicalDpi;
            var wParam = PARAM.FromLowHigh(newDpi, newDpi);
            var suggestedRect = new RECT(0, 0, (int)Math.Round(control.Width * factor), (int)Math.Round(control.Height * factor));

            // Initialize unmanged memory to hold the struct.
            IntPtr rectPtr = Marshal.AllocHGlobal(Marshal.SizeOf(suggestedRect));
            Marshal.StructureToPtr(suggestedRect, rectPtr, true);
            User32.SendMessageW(control.Handle, MSG, wParam, rectPtr);

            return rectPtr;
        }

        [WinFormsTheory]
        [InlineData(2 * DpiHelper.LogicalDpi)]
        [InlineData(3.5 * DpiHelper.LogicalDpi)]
        public void Form_DpiChanged_ClientRectangle(int newDpi)
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

            // Allocating dummy memory. Which will be cleared and replaced in TriggerDpiMessage.
            IntPtr allocatedMemory = Marshal.AllocHGlobal(Marshal.SizeOf(originalAwarenessContext));
            try
            {
                using var form = new Form();
                form.AutoScaleMode = AutoScaleMode.Dpi;
                form.Show();
                Drawing.Rectangle initialClientRect = form.ClientRectangle;
                allocatedMemory = TriggerDpiMessage(User32.WM.DPICHANGED, form, newDpi);
                var factor = newDpi / DpiHelper.LogicalDpi;

                //Its difficult to calculate exact value in tests here. We need to account for native
                //portions of the form and their DpiAwareness. So, simulating expected values to be greater
                // than that factor multiplication.
                Assert.True(form.ClientRectangle.Width > Math.Round(initialClientRect.Width * factor));
                Assert.True(form.ClientRectangle.Height > Math.Round(initialClientRect.Height * factor));
                form.Close();
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(allocatedMemory);

                // reset back to original awareness context.
                User32.SetThreadDpiAwarenessContext(originalAwarenessContext);
            }
        }
    }
}
