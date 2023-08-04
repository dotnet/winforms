﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Primitives;
using Windows.Win32.UI.HiDpi;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests.Dpi;

public class FormDpiTests : ControlTestBase
{
    public FormDpiTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsTheory]
    [InlineData(3.5 * DpiHelper.LogicalDpi)]
    public void Form_DpiChanged_Bounds(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        typeof(DpiHelper).TestAccessor().Dynamic.Initialize();
        try
        {
            using Form form = new();
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Show();

            Drawing.Rectangle initialBounds = form.Bounds;
            float initialFontSize = form.Font.Size;
            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, newDpi);

            // Lab machines giving strange values that I could not explain. for ex: on local machine,
            // I get 1050*1050 for factor 3.5. This is not same on lab machines ( ex, we get 1044). For now,
            // just verifying they are scaled.
            Assert.NotEqual(initialBounds.Width, form.Bounds.Width);
            Assert.NotEqual(initialBounds.Height, form.Bounds.Height);
            Assert.NotEqual(initialFontSize, form.Font.Size);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContextInternal(originalAwarenessContext);
        }
    }

    [WinFormsTheory]
    [InlineData(3.5 * DpiHelper.LogicalDpi)]
    public void Form_DpiChanged_MinMaxSizeNotChanged_Default(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        typeof(DpiHelper).TestAccessor().Dynamic.Initialize();
        try
        {
            var minSize = new Drawing.Size(100, 100);
            var maxSize = new Drawing.Size(500, 500);
            using var form = new Form();
            form.MinimumSize = minSize;
            form.MaximumSize = maxSize;
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Show();
            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, newDpi);

            Assert.Equal(form.MinimumSize, minSize);
            Assert.Equal(form.MaximumSize, maxSize);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContext(originalAwarenessContext);
        }
    }

    [WinFormsTheory]
    [InlineData(3.5 * DpiHelper.LogicalDpi)]
    public void Form_DpiChanged_MinMaxSizeChanged_WithRuntimeSetting(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        typeof(DpiHelper).TestAccessor().Dynamic.Initialize();
        try
        {
            var minSize = new Drawing.Size(100, 100);
            var maxSize = new Drawing.Size(500, 500);
            using var form = new Form();
            form.MinimumSize = minSize;
            form.MaximumSize = maxSize;
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Show();

            // Explicitly opt-in to resize min and max sizes with Dpi changed event.
            dynamic testAccessor = typeof(LocalAppContextSwitches).TestAccessor().Dynamic;
            testAccessor.s_scaleTopLevelFormMinMaxSizeForDpi = 1;

            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, newDpi);

            Assert.NotEqual(form.MinimumSize, minSize);
            Assert.NotEqual(form.MaximumSize, maxSize);

            // Reset switch.
            testAccessor.s_scaleTopLevelFormMinMaxSizeForDpi = -1;
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContext(originalAwarenessContext);
        }
    }

    [WinFormsTheory]
    [InlineData(3.5 * DpiHelper.LogicalDpi)]
    public void Form_DpiChanged_NonLinear_DesiredSize(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        typeof(DpiHelper).TestAccessor().Dynamic.Initialize();
        try
        {
            using var form = new Form();
            form.AutoScaleMode = AutoScaleMode.Font;
            form.Show();

            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, newDpi);
            Drawing.Size nonLInearSize = form.Size;

            // Rollback to 96 Dpi, and change AutoScaleMode to check with linear size
            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, (int)DpiHelper.LogicalDpi);
            form.AutoScaleMode = AutoScaleMode.Dpi;

            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, newDpi);
            Assert.NotEqual(form.Size, nonLInearSize);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContext(originalAwarenessContext);
        }
    }

    [WinFormsTheory]
    [InlineData(3.5 * DpiHelper.LogicalDpi)]
    public void Form_DpiChanged_FormCacheSize(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        typeof(DpiHelper).TestAccessor().Dynamic.Initialize();
        try
        {
            using var form = new Form();
            form.AutoScaleMode = AutoScaleMode.Font;
            form.Show();

            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, newDpi);

            dynamic fomrTestAccessor = form.TestAccessor().Dynamic;
            Assert.NotNull(fomrTestAccessor._dpiFormSizes);
            Assert.Equal(2, fomrTestAccessor._dpiFormSizes.Count);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContext(originalAwarenessContext);
        }
    }

    [WinFormsTheory]
    [InlineData(3.5 * DpiHelper.LogicalDpi)]
    public void Form_DpiChanged_AutoScaleMode_Dpi_FormDoesNotCacheSize(int newDpi)
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
        typeof(DpiHelper).TestAccessor().Dynamic.Initialize();
        try
        {
            using var form = new Form();
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Show();

            DpiMessageHelper.TriggerDpiMessage(PInvoke.WM_DPICHANGED, form, newDpi);
            Assert.Null(form.TestAccessor().Dynamic._dpiFormSizes);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContext(originalAwarenessContext);
        }
    }

    [WinFormsFact]
    public void Form_SizeNotCached_SystemAwareMode()
    {
        // Run tests only on Windows 10 versions that support thread dpi awareness.
        if (!PlatformDetection.IsWindows10Version1803OrGreater)
        {
            return;
        }

        DPI_AWARENESS_CONTEXT originalAwarenessContext = PInvoke.SetThreadDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE);
        typeof(DpiHelper).TestAccessor().Dynamic.Initialize();
        try
        {
            using var form = new Form();
            form.AutoScaleMode = AutoScaleMode.Font;
            form.Show();

            Assert.Null(form.TestAccessor().Dynamic._dpiFormSizes);
            form.Close();
        }
        finally
        {
            // Reset back to original awareness context.
            PInvoke.SetThreadDpiAwarenessContext(originalAwarenessContext);
        }
    }
}
