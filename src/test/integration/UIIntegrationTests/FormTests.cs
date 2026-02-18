// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests;

public class FormTests : ControlTestBase
{
    // When using the keyboard for snap layout menu, there are various times when
    // a delay is needed. This value may need to be adjusted if tests fail
    // in CI/different environment
    private const int SnapLayoutDelayMS = 500;

    public FormTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsTheory]
    [InlineData(FormWindowState.Normal)]
    [InlineData(FormWindowState.Maximized)]
    public async Task Form_SnapsLeftAsync(FormWindowState windowState)
    {
        if (!OsVersion.IsWindows11_OrGreater())
        {
            return;
        }

        await RunEmptyFormTestAsync(async form =>
        {
            form.Location = new Point(20, 21);
            form.Size = new Size(300, 310);

            form.WindowState = windowState;

            // Snap left using Win+Left shortcut. This directly snaps the window
            // to the left half of the screen without requiring snap layout panel navigation.
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard
                    .ModifiedKeyStroke(VIRTUAL_KEY.VK_LWIN, VIRTUAL_KEY.VK_LEFT));

            await Task.Delay(SnapLayoutDelayMS);

            // After snapping left, Windows may display a panel containing all running
            // applications so the user can select one to dock next to our form. It also
            // takes the keyboard focus away. Dismiss it with Escape.
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_ESCAPE));

            await Task.Delay(SnapLayoutDelayMS);

            var screenWorkingArea = Screen.FromControl(form).WorkingArea;
            int borderSize = (form.Width - form.ClientRectangle.Width) / 2;

            Assert.True(form.Left <= screenWorkingArea.X);
            Assert.True(form.Left >= screenWorkingArea.X - borderSize);

            Assert.True(form.Top <= screenWorkingArea.Y);
            Assert.True(form.Top >= screenWorkingArea.Y - borderSize);

            Assert.True(form.Height >= screenWorkingArea.Height);
            Assert.True(form.Height <= screenWorkingArea.Height + (borderSize * 2));

            Assert.True(form.Width >= screenWorkingArea.Width / 2);
            Assert.True(form.Width <= (screenWorkingArea.Width / 2) + (borderSize * 2));
        });
    }

    [WinFormsTheory]
    [InlineData(FormWindowState.Normal)]
    [InlineData(FormWindowState.Maximized)]
    public async Task Form_SnapsRightAsync(FormWindowState windowState)
    {
        if (!OsVersion.IsWindows11_OrGreater())
        {
            return;
        }

        await RunEmptyFormTestAsync(async form =>
        {
            form.Location = new Point(20, 21);
            form.Size = new Size(300, 310);

            form.WindowState = windowState;

            // Snap right using Win+Right shortcut. This directly snaps the window
            // to the right half of the screen without requiring snap layout panel navigation.
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard
                    .ModifiedKeyStroke(VIRTUAL_KEY.VK_LWIN, VIRTUAL_KEY.VK_RIGHT));

            await Task.Delay(SnapLayoutDelayMS);

            // After snapping right, Windows may display a panel containing all running
            // applications so the user can select one to dock next to our form. It also
            // takes the keyboard focus away. Dismiss it with Escape.
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_ESCAPE));

            await Task.Delay(SnapLayoutDelayMS);

            var screenWorkingArea = Screen.FromControl(form).WorkingArea;
            int screenMiddleX = screenWorkingArea.X + (screenWorkingArea.Width / 2);
            int borderSize = (form.Width - form.ClientRectangle.Width) / 2;

            Assert.True(form.Left <= screenMiddleX);
            Assert.True(form.Left >= screenMiddleX - borderSize);

            Assert.True(form.Top <= screenWorkingArea.Y);
            Assert.True(form.Top >= screenWorkingArea.Y - borderSize);

            Assert.True(form.Height >= screenWorkingArea.Height);
            Assert.True(form.Height <= screenWorkingArea.Height + (borderSize * 2));

            Assert.True(form.Width >= screenWorkingArea.Width / 2);
            Assert.True(form.Width <= (screenWorkingArea.Width / 2) + (borderSize * 2));
        });
    }

    private async Task RunEmptyFormTestAsync(Func<Form, Task> testDriverAsync)
    {
        await RunFormWithoutControlAsync(
            () =>
            {
                Form form = new()
                {
                    TopMost = true
                };

                return form;
            },
            testDriverAsync);
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
