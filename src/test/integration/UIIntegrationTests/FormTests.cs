// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.UITests.Input;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests;

public class FormTests : ControlTestBase
{
    private static readonly TimeSpan s_snapTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan s_pollInterval = TimeSpan.FromMilliseconds(50);

    public FormTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsTheory]
    [InlineData(FormWindowState.Normal)]
    [InlineData(FormWindowState.Maximized)]
    public Task Form_SnapsLeftAsync(FormWindowState windowState)
        => VerifySnapAsync(windowState, VIRTUAL_KEY.VK_LEFT);

    [WinFormsTheory]
    [InlineData(FormWindowState.Normal)]
    [InlineData(FormWindowState.Maximized)]
    public Task Form_SnapsRightAsync(FormWindowState windowState)
        => VerifySnapAsync(windowState, VIRTUAL_KEY.VK_RIGHT);

    private async Task VerifySnapAsync(FormWindowState windowState, VIRTUAL_KEY direction)
    {
        if (!OsVersion.IsWindows11_OrGreater())
        {
            return;
        }

        await RunFormWithoutControlAsync(
            () => new Form
            {
                StartPosition = FormStartPosition.Manual,
                Bounds = new Rectangle(20, 21, 300, 310),
                TopMost = true
            },
            async form =>
            {
                form.WindowState = windowState;
                form.TopMost = false;
                Rectangle startingBounds = form.Bounds;
                Rectangle workingArea = Screen.FromControl(form).WorkingArea;
                HWND handle = form.HWND;
                Assert.False(HasSnappedBounds(form, workingArea, direction), "The test form is already at the expected snap bounds.");

                // Send the shortcut only while the form is the foreground window. It is
                // handled by the shell, so an idle WinForms message loop is insufficient.
                bool active = await WaitForAsync(() =>
                {
                    HWND current = PInvokeCore.GetForegroundWindow();
                    if (current == handle)
                    {
                        return true;
                    }

                    form.Activate();
                    return PInvokeCore.GetForegroundWindow() == handle;
                });

                HWND foreground = PInvokeCore.GetForegroundWindow();
                Assert.True(active && foreground == handle,
                    $"Form {handle} did not acquire the foreground. Foreground window: {foreground}.");

                SendSnapShortcut(direction);

                // Let the shell finish arranging the window without taking focus back.
                // RunFormWithoutControlAsync closes the test dialog after the assertion.
                bool snapped = await WaitForAsync(() => HasSnappedBounds(form, workingArea, direction));
                Assert.True(snapped,
                    $"Win+{direction} did not snap the {windowState} form within {s_snapTimeout}. "
                    + $"Starting bounds: {startingBounds}. Actual bounds: {form.Bounds}. "
                    + $"Client size: {form.ClientSize}. Working area: {workingArea}. "
                    + $"Foreground window: {PInvokeCore.GetForegroundWindow()}.");
            });
    }

    private static void SendSnapShortcut(VIRTUAL_KEY direction)
    {
        Span<INPUT> inputs =
        [
            InputBuilder.KeyDown(VIRTUAL_KEY.VK_LWIN),
            InputBuilder.KeyDown(direction),
            InputBuilder.KeyUp(direction),
            InputBuilder.KeyUp(VIRTUAL_KEY.VK_LWIN)
        ];

        uint inserted = PInvoke.SendInput(inputs, Marshal.SizeOf<INPUT>());
        int error = Marshal.GetLastPInvokeError();
        if (inserted == (uint)inputs.Length)
        {
            return;
        }

        string message = $"SendInput inserted {inserted} of {inputs.Length} events for Win+{direction}. Win32 error: {error}.";
        if (inserted > 0)
        {
            // Do not leave a modifier or arrow pressed if only part of the chord was sent.
            Span<INPUT> releases = inputs[(inserted == 2 ? 2 : 3)..];
            uint released = PInvoke.SendInput(releases, Marshal.SizeOf<INPUT>());
            message += $" Inserted {released} of {releases.Length} recovery key-up events.";
        }

        Assert.Fail(message);
    }

    private static bool HasSnappedBounds(Form form, Rectangle workingArea, VIRTUAL_KEY direction)
    {
        Rectangle bounds = form.Bounds;
        int borderSize = (bounds.Width - form.ClientSize.Width) / 2;
        int halfWidth = workingArea.Width / 2;
        int expectedLeft = direction == VIRTUAL_KEY.VK_LEFT ? workingArea.Left : workingArea.Left + halfWidth;

        return bounds.Left <= expectedLeft
            && bounds.Left >= expectedLeft - borderSize
            && bounds.Top <= workingArea.Top
            && bounds.Top >= workingArea.Top - borderSize
            && bounds.Height >= workingArea.Height
            && bounds.Height <= workingArea.Height + (borderSize * 2)
            && bounds.Width >= halfWidth
            && bounds.Width <= halfWidth + (borderSize * 2);
    }

    private static async Task<bool> WaitForAsync(Func<bool> condition)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (!condition())
        {
            if (stopwatch.Elapsed >= s_snapTimeout)
            {
                return false;
            }

            await Task.Delay(s_pollInterval);
        }

        return true;
    }

    [WinFormsFact]
    public void Form_MinimumSize_DoesNotChangeDisplayOrder()
    {
        // Initialize Form1 and Form2.
        using Form form1 = new Form { Text = "Form1" };
        using Form form2 = new Form { Text = "Form2" };

        // Display the forms.
        form1.Show();
        form2.Show();

        // Set initial hierarchy: Form2 should be displayed in front of Form1.
        form2.BringToFront();
        Assert.True(form2.TopMost || form2.Focused, "Form2 should be displayed in front of Form1");

        // Set the MinimumSize property of Form1.
        form1.MinimumSize = new Size(300, 300);

        // Verify the hierarchy remains unchanged.
        Assert.True(form2.TopMost || form2.Focused, "Form2 should still be displayed in front after setting MinimumSize");
    }
}
