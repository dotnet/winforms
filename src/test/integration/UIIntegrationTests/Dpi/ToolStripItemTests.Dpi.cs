// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.HiDpi;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests.Dpi;

public class ToolStripItemDpiTests : ControlTestBase
{
    public ToolStripItemDpiTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsTheory]
    [InlineData(2 * ScaleHelper.OneHundredPercentLogicalDpi)]
    [InlineData(3.5 * ScaleHelper.OneHundredPercentLogicalDpi)]
    public void ToolStripItems_FontScaling(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        // Set thread awareness context to PermonitorV2(PMv2).
        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        typeof(ScaleHelper).TestAccessor().Dynamic.InitializeStatics();
        try
        {
            int clientWidth = 800;
            using Form form = new();
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.ClientSize = new Size(clientWidth, 450);
            using ToolStrip toolStrip = new();
            using ToolStripButton toolStripItemOpen = new("Open");

            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.Add(toolStripItemOpen);
            toolStrip.Location = new Point(0, 0);
            toolStrip.Name = "toolStrip1";
            toolStrip.Text = "toolStrip1";
            using Font initialFont = toolStrip.Font = new("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);

            form.Show();

            DpiMessageHelper.TriggerDpiMessage(PInvokeCore.WM_DPICHANGED_BEFOREPARENT, toolStrip, newDpi);
            float factor = (float)newDpi / form.DeviceDpi;

            Assert.Equal((float)initialFont.Size * factor, toolStrip.Font.Size, precision: 1);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }
}
