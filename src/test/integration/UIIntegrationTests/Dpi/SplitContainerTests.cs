// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.HiDpi;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests.Dpi;

public class SplitContainerTests : ControlTestBase
{
    public SplitContainerTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public void SplitContainer_Constructor()
    {
        using SplitContainer sc = new();

        Assert.NotNull(sc);
        Assert.NotNull(sc.Panel1);
        Assert.Equal(sc, sc.Panel1.Owner);
        Assert.NotNull(sc.Panel2);
        Assert.Equal(sc, sc.Panel2.Owner);
        Assert.False(sc.SplitterRectangle.IsEmpty);
    }

    [WinFormsTheory]
    [InlineData(3.5 * 96)]
    public void SplitContainer_Properties_HorizontalSplitter_Scaling(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        typeof(ScaleHelper).TestAccessor().Dynamic.InitializeStatics();
        try
        {
            using Form form = new();
            using SplitContainer splitContainer = new()
            {
                FixedPanel = FixedPanel.Panel1,
                Location = new Point(0, 0),
                Margin = new Padding(0),
                Name = "splitContainer2",
                Orientation = Orientation.Horizontal,
                Size = new Size(812, 619),
                SplitterDistance = 90,
                SplitterWidth = 2
            };

            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Controls.Add(splitContainer);
            form.Show();

            DpiMessageHelper.TriggerDpiMessage(PInvokeCore.WM_DPICHANGED_BEFOREPARENT, splitContainer, newDpi);
            DpiMessageHelper.TriggerDpiMessage(PInvokeCore.WM_DPICHANGED, form, newDpi);

            Assert.NotEqual(90, splitContainer.SplitterDistance);
            Assert.NotEqual(2, splitContainer.SplitterWidth);
            Assert.Equal(splitContainer.SplitterDistance, splitContainer.Panel1.Height);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }
}
