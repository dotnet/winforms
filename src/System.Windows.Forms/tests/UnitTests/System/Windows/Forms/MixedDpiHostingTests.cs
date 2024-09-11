// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class MixedDpiHostingTests
{
    [WinFormsFact]
    public void MixedHosting_Default()
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        // Set thread awareness context to PerMonitorV2(PMv2).
        // If process/thread is not in PMv2, calling 'EnterDpiAwarenessScope' is a no-op and that is by design.
        // In this case, we will be setting thread to PMv2 mode and then scope to UNAWARE
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        try
        {
            using Form form = new();
            using (ScaleHelper.EnterDpiAwarenessScope(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE))
            {
                using Control control = new();
                form.Controls.Add(control);
                form.Load += (_, _) =>
                {
                    form.Close();
                };

                form.ShowDialog();

                DPI_AWARENESS_CONTEXT controlDpiContext = PInvoke.GetWindowDpiAwarenessContext(control.HWND);
                DPI_AWARENESS_CONTEXT formDpiContext = PInvoke.GetWindowDpiAwarenessContext(form.HWND);
                Assert.True(controlDpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE));
                Assert.True(formDpiContext.IsEquivalent(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2));
            }
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }
}
