// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.DotNet.RemoteExecutor;

namespace System.Windows.Forms.Tests;

public partial class FormTests
{
    // DPI awareness is process wide and must be set before any window is created, so these tests
    // run in a child process (RemoteExecutor) and do not alter the test host's awareness mode.

    [WinFormsFact]
    public void Form_WmDpiChanged_RaisesEventsInOrderAndAppliesSuggestedRectangle()
    {
        // A top-level form receives a DPI change to a higher-DPI monitor. Children are notified
        // before the form's DpiChanged event and after it, with plain EventArgs. The form reports
        // the old/new DPI, moves to the suggested rectangle, and the child tree is rescaled.
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke(() =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();

            using DpiTestForm form = new() { AutoScaleMode = AutoScaleMode.Font };
            using DpiTestPanel panel = new() { Bounds = new Rectangle(10, 10, 200, 150) };
            using DpiTestButton button = new() { Bounds = new Rectangle(20, 30, 80, 40) };
            panel.Controls.Add(button);
            form.Controls.Add(panel);
            _ = form.Handle;
            _ = panel.Handle;
            _ = button.Handle;

            form.PerformAutoScale();
            SizeF dimensions = form.CurrentAutoScaleDimensions;
            int oldDpi = form.DeviceDpi;
            int newDpi = oldDpi * 2;
            Rectangle oldBounds = form.Bounds;
            Rectangle panelBounds = panel.Bounds;
            Rectangle buttonBounds = button.Bounds;
            Rectangle suggested = new(oldBounds.X + 30, oldBounds.Y + 40, 600, 500);

            List<string> log = [];
            DpiChangedEventArgs? formArgs = null;
            form.DpiChanged += (sender, e) =>
            {
                sender.Should().BeSameAs(form);
                formArgs = e;
                log.Add("form-changed");
            };

            panel.DpiChangedBeforeParent += (sender, e) =>
            {
                sender.Should().BeSameAs(panel);
                e.Should().BeSameAs(EventArgs.Empty);
                panel.DeviceDpi.Should().Be(newDpi);
                log.Add("panel-before");
            };
            button.DpiChangedBeforeParent += (sender, e) =>
            {
                sender.Should().BeSameAs(button);
                e.Should().BeSameAs(EventArgs.Empty);
                button.DeviceDpi.Should().Be(newDpi);
                log.Add("button-before");
            };
            panel.DpiChangedAfterParent += (sender, e) =>
            {
                sender.Should().BeSameAs(panel);
                e.Should().BeSameAs(EventArgs.Empty);
                log.Add("panel-after");
            };
            button.DpiChangedAfterParent += (sender, e) =>
            {
                sender.Should().BeSameAs(button);
                e.Should().BeSameAs(EventArgs.Empty);
                log.Add("button-after");
            };

            SendDpiChange(form, panel, button, newDpi, suggested);

            // Each message raises exactly one event when it is handled, and children already
            // report the new DPI when their "before parent" event fires (asserted in the handlers).
            log.Should().Equal("button-before", "panel-before", "form-changed", "panel-after", "button-after");

            formArgs.Should().NotBeNull();
            formArgs!.DeviceDpiOld.Should().Be(oldDpi);
            formArgs.DeviceDpiNew.Should().Be(newDpi);
            formArgs.SuggestedRectangle.Should().Be(suggested);
            formArgs.Cancel.Should().BeFalse();
            formArgs.ToString().Should().Be($"was: {oldDpi}, now: {newDpi}");

            form.DeviceDpi.Should().Be(newDpi);
            panel.DeviceDpi.Should().Be(newDpi);
            button.DeviceDpi.Should().Be(newDpi);
            form.Bounds.Should().Be(suggested);
            // Children are scaled exactly once by the font autoscale factor, not twice.
            SizeF factor = ContainerControl.GetCurrentAutoScaleFactor(form.CurrentAutoScaleDimensions, dimensions);
            factor.Width.Should().BeGreaterThan(1);
            factor.Height.Should().BeGreaterThan(1);
            panel.Bounds.Should().Be(Scale(panelBounds, factor));
            button.Bounds.Should().Be(Scale(buttonBounds, factor));
            form.IsHandleCreated.Should().BeTrue();
            panel.IsHandleCreated.Should().BeTrue();
            button.IsHandleCreated.Should().BeTrue();
        });

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [WinFormsFact]
    public void Form_WmDpiChanged_ReturnTripRestoresBoundsWithoutDrift()
    {
        // A form moves to a higher-DPI monitor and back, several times. Each return trip must
        // restore the original form size (served by the form's per-DPI size cache when the
        // OS asks for the scaled size) and child layout within one pixel, so repeated
        // monitor transitions do not accumulate scaling drift.
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke(() =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();

            using DpiTestForm form = new() { AutoScaleMode = AutoScaleMode.Font };
            using DpiTestPanel panel = new() { Bounds = new Rectangle(10, 10, 200, 150) };
            using DpiTestButton button = new() { Bounds = new Rectangle(20, 30, 80, 40) };
            panel.Controls.Add(button);
            form.Controls.Add(panel);
            _ = form.Handle;
            _ = panel.Handle;
            _ = button.Handle;

            int originalDpi = form.DeviceDpi;
            int higherDpi = originalDpi * 2;
            Rectangle originalBounds = form.Bounds;
            Rectangle originalPanel = panel.Bounds;
            Rectangle originalButton = button.Bounds;

            List<(int Old, int New)> transitions = [];
            form.DpiChanged += (_, e) => transitions.Add((e.DeviceDpiOld, e.DeviceDpiNew));

            for (int i = 0; i < 3; i++)
            {
                Size higherSize = GetDpiScaledSize(form, higherDpi);
                higherSize.Width.Should().BeGreaterThan(originalBounds.Width);
                higherSize.Height.Should().BeGreaterThan(originalBounds.Height);
                Rectangle higherBounds = new(originalBounds.Location, higherSize);
                SendDpiChange(form, panel, button, higherDpi, higherBounds);
                form.DeviceDpi.Should().Be(higherDpi);
                form.Bounds.Should().Be(higherBounds);

                // The size for the original DPI is remembered from before the first transition.
                Size returnSize = GetDpiScaledSize(form, originalDpi);
                returnSize.Should().Be(originalBounds.Size);
                SendDpiChange(form, panel, button, originalDpi, new Rectangle(originalBounds.Location, returnSize));
                form.DeviceDpi.Should().Be(originalDpi);
                panel.DeviceDpi.Should().Be(originalDpi);
                button.DeviceDpi.Should().Be(originalDpi);
                form.Bounds.Should().Be(originalBounds);
                AssertWithin(panel.Bounds, originalPanel, 1);
                AssertWithin(button.Bounds, originalButton, 1);
            }

            transitions.Should().Equal(
                (originalDpi, higherDpi),
                (higherDpi, originalDpi),
                (originalDpi, higherDpi),
                (higherDpi, originalDpi),
                (originalDpi, higherDpi),
                (higherDpi, originalDpi));
        });

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [WinFormsFact]
    public void Form_WmDpiChanged_CancelledByHandler_DoesNotApplySuggestedRectangle()
    {
        // A DpiChanged handler cancels the change: the form is not moved or resized to the
        // suggested rectangle and its children are not rescaled, but the new DPI is still recorded.
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke(() =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();

            using DpiTestForm form = new() { AutoScaleMode = AutoScaleMode.Font };
            using DpiTestButton button = new() { Bounds = new Rectangle(20, 30, 80, 40) };
            form.Controls.Add(button);
            _ = form.Handle;
            _ = button.Handle;

            int oldDpi = form.DeviceDpi;
            int newDpi = oldDpi * 2;
            Rectangle oldBounds = form.Bounds;
            Rectangle buttonBounds = button.Bounds;
            int callCount = 0;
            form.DpiChanged += (_, e) =>
            {
                callCount++;
                e.Cancel = true;
            };

            SendDpiChange(form, panel: null, button, newDpi, new Rectangle(0, 0, 700, 600));

            callCount.Should().Be(1);
            form.Bounds.Should().Be(oldBounds);
            button.Bounds.Should().Be(buttonBounds);
            form.DeviceDpi.Should().Be(newDpi);
        });

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    [WinFormsFact]
    public void Form_WmDpiChanged_SameDpi_DoesNotRaiseEventOrChangeBounds()
    {
        // A DPI notification that reports the current DPI is ignored: no DpiChanged event
        // is raised and the suggested rectangle is not applied.
        using RemoteInvokeHandle handle = RemoteExecutor.Invoke(() =>
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2).Should().BeTrue();

            using DpiTestForm form = new() { AutoScaleMode = AutoScaleMode.Font };
            _ = form.Handle;

            int dpi = form.DeviceDpi;
            Rectangle oldBounds = form.Bounds;
            int callCount = 0;
            form.DpiChanged += (_, _) => callCount++;

            SendDpiChange(form, panel: null, button: null, dpi, new Rectangle(0, 0, 700, 600));

            callCount.Should().Be(0);
            form.Bounds.Should().Be(oldBounds);
            form.DeviceDpi.Should().Be(dpi);
        });

        handle.ExitCode.Should().Be(RemoteExecutor.SuccessExitCode);
    }

    private static Rectangle Scale(Rectangle bounds, SizeF factor) => new(
        (int)Math.Round(bounds.X * factor.Width),
        (int)Math.Round(bounds.Y * factor.Height),
        (int)Math.Round(bounds.Width * factor.Width),
        (int)Math.Round(bounds.Height * factor.Height));

    private static void AssertWithin(Rectangle actual, Rectangle expected, int tolerance)
    {
        Math.Abs(actual.X - expected.X).Should().BeLessThanOrEqualTo(tolerance);
        Math.Abs(actual.Y - expected.Y).Should().BeLessThanOrEqualTo(tolerance);
        Math.Abs(actual.Width - expected.Width).Should().BeLessThanOrEqualTo(tolerance);
        Math.Abs(actual.Height - expected.Height).Should().BeLessThanOrEqualTo(tolerance);
    }

    // Asks the form for its size at another DPI, as the OS does before WM_DPICHANGED.
    private static unsafe Size GetDpiScaledSize(DpiTestForm form, int dpi)
    {
        SIZE size = default;
        Message m = Message.Create(form.Handle, (int)PInvokeCore.WM_GETDPISCALEDSIZE, dpi, (nint)(&size));
        form.WndProc(ref m);
        ((nint)m.Result).Should().Be(1);
        return new Size(size.cx, size.cy);
    }

    // Mirrors the OS sequence for a monitor change: children get the "before parent" message
    // (deepest first), the top-level window gets WM_DPICHANGED, then children get "after parent".
    private static unsafe void SendDpiChange(DpiTestForm form, DpiTestPanel? panel, DpiTestButton? button, int newDpi, Rectangle suggested)
    {
        nint dpiParam = (nint)PARAM.FromLowHigh(newDpi, newDpi);

        if (button is not null)
        {
            Message beforeButton = Message.Create(button.Handle, (int)PInvokeCore.WM_DPICHANGED_BEFOREPARENT, dpiParam, 0);
            button.WndProc(ref beforeButton);
        }

        if (panel is not null)
        {
            Message beforePanel = Message.Create(panel.Handle, (int)PInvokeCore.WM_DPICHANGED_BEFOREPARENT, dpiParam, 0);
            panel.WndProc(ref beforePanel);
        }

        RECT* rect = (RECT*)NativeMemory.Alloc((nuint)sizeof(RECT));
        try
        {
            *rect = suggested;
            Message dpiChanged = Message.Create(form.Handle, (int)PInvokeCore.WM_DPICHANGED, dpiParam, (nint)rect);
            form.WndProc(ref dpiChanged);
        }
        finally
        {
            NativeMemory.Free(rect);
        }

        if (panel is not null)
        {
            Message afterPanel = Message.Create(panel.Handle, (int)PInvokeCore.WM_DPICHANGED_AFTERPARENT, dpiParam, 0);
            panel.WndProc(ref afterPanel);
        }

        if (button is not null)
        {
            Message afterButton = Message.Create(button.Handle, (int)PInvokeCore.WM_DPICHANGED_AFTERPARENT, dpiParam, 0);
            button.WndProc(ref afterButton);
        }
    }

    private sealed class DpiTestPanel : Panel
    {
        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    private sealed class DpiTestButton : Button
    {
        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    private sealed class DpiTestForm : Form
    {
        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }
}
