// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.Dpi
{
    public class ToolStripItemDpiTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData(2 * DpiHelper.LogicalDpi)]
        [InlineData(3.5 * DpiHelper.LogicalDpi)]
        public void ToolStripItems_FontScaling(int newDpi)
        {
            // Run tests only on Windows 10 versions that support thread dpi awareness.
            if (!PlatformDetection.IsWindows10Version1803OrGreater)
            {
                return;
            }

            // Set thread awareness context to PermonitorV2(PMv2).
            IntPtr originalAwarenessContext = User32.SetThreadDpiAwarenessContext(User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2);

            try
            {
                int clientWidth = 800;
                using var form = new Form();
                form.AutoScaleMode = AutoScaleMode.Dpi;
                form.ClientSize = new Size(clientWidth, 450);
                using var toolStrip = new ToolStrip();
                using var toolStripItemOpen = new ToolStripButton("Open");

                toolStrip.GripStyle = ToolStripGripStyle.Hidden;
                toolStrip.Items.Add(toolStripItemOpen);
                toolStrip.Location = new Point(0, 0);
                toolStrip.Name = "toolStrip1";
                toolStrip.Text = "toolStrip1";
                using Font initialFont = toolStrip.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);

                form.Show();

                DpiMessageHelper.TriggerDpiMessage(User32.WM.DPICHANGED_BEFOREPARENT, toolStrip, newDpi);
                var factor = newDpi / DpiHelper.LogicalDpi;

                Assert.Equal((float)initialFont.Size * factor, toolStrip.Font.Size, precision: 1);
                form.Close();
            }
            finally
            {
                // Reset back to original awareness context.
                User32.SetThreadDpiAwarenessContext(originalAwarenessContext);
            }
        }
    }
}
