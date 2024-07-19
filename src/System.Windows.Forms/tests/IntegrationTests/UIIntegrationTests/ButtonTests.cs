// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class ButtonTests : ControlTestBase
{
    public ButtonTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsTheory]
    // These results close the window
    [InlineData(DialogResult.Abort)]
    [InlineData(DialogResult.Cancel)]
    [InlineData(DialogResult.Ignore)]
    [InlineData(DialogResult.No)]
    [InlineData(DialogResult.OK)]
    [InlineData(DialogResult.Retry)]
    [InlineData(DialogResult.Yes)]
    // This result leaves the window open
    [InlineData(DialogResult.None)]
    public async Task Button_DialogResult_ClickDefaultButtonToCloseFormAsync(DialogResult dialogResult)
    {
        await RunTestAsync(async (form, button) =>
        {
            Assert.Equal(DialogResult.None, button.DialogResult);

            button.DialogResult = dialogResult;

            await MoveMouseToControlAsync(button);

            Assert.Equal(CloseReason.None, form.CloseReason);
            Assert.Equal(DialogResult.None, form.DialogResult);
            Assert.True(form.Visible);

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.LeftButtonClick());

            Assert.Equal(CloseReason.None, form.CloseReason);
            Assert.Equal(dialogResult, form.DialogResult);

            // The window will only still be visible for DialogResult.None
            Assert.Equal(dialogResult == DialogResult.None, form.Visible);
        });
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11324")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X64,
        "Flaky tests, see: https://github.com/dotnet/winforms/issues/11324")]
    public async Task Button_DialogResult_SpaceToClickFocusedButtonAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            button.DialogResult = DialogResult.OK;

            Assert.True(button.Focus());

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_SPACE));

            Assert.Equal(DialogResult.OK, form.DialogResult);
            Assert.False(form.Visible);
        });
    }

    [WinFormsFact]
    public async Task Button_DialogResult_EscapeDoesNotClickFocusedButtonAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            button.DialogResult = DialogResult.OK;

            Assert.True(button.Focus());

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_ESCAPE));

            Assert.Equal(DialogResult.None, form.DialogResult);
            Assert.True(form.Visible);
        });
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11326")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X64,
        "Flaky tests, see: https://github.com/dotnet/winforms/issues/11326")]
    public async Task Button_CancelButton_EscapeClicksCancelButtonAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            form.CancelButton = button;

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_ESCAPE));

            Assert.Equal(DialogResult.Cancel, form.DialogResult);
            Assert.False(form.Visible);
        });
    }

    [WinFormsFact]
    public async Task Button_AchorNone_NoResizeOnWindowSizeWiderAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            var originalFormSize = form.DisplayRectangle.Size;
            var originalButtonPosition = button.DisplayRectangle;

            Point mouseDragHandleOnForm = new(form.DisplayRectangle.Right, form.DisplayRectangle.Top + form.DisplayRectangle.Height / 2);
            await MoveMouseAsync(form, form.PointToScreen(mouseDragHandleOnForm));

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                    .LeftButtonDown()
                    .MoveMouseBy(form.DisplayRectangle.Width, 0)
                    .LeftButtonUp());

            Assert.True(form.DisplayRectangle.Width > originalFormSize.Width);
            Assert.Equal(originalFormSize.Height, form.DisplayRectangle.Height);
            Assert.Equal(originalButtonPosition, button.DisplayRectangle);
        });
    }

    [WinFormsFact]
    public async Task Button_AchorNone_NoResizeOnWindowSizeTallerAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            var originalFormSize = form.DisplayRectangle.Size;
            var originalButtonPosition = button.DisplayRectangle;

            Point mouseDragHandleOnForm = new(form.DisplayRectangle.Left + form.DisplayRectangle.Width / 2, form.DisplayRectangle.Bottom);
            await MoveMouseAsync(form, form.PointToScreen(mouseDragHandleOnForm));

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                    .LeftButtonDown()
                    .MoveMouseBy(0, form.DisplayRectangle.Height)
                    .LeftButtonUp());

            Assert.True(form.DisplayRectangle.Height > originalFormSize.Height);
            Assert.Equal(originalFormSize.Width, form.DisplayRectangle.Width);
            Assert.Equal(originalButtonPosition, button.DisplayRectangle);
        });
    }

    [WinFormsFact]
    public async Task Button_Anchor_ResizeOnWindowSizeWiderAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            button.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            var originalFormSize = form.DisplayRectangle.Size;
            var originalButtonPosition = button.DisplayRectangle;

            Point mouseDragHandleOnForm = new(form.DisplayRectangle.Right, form.DisplayRectangle.Top + form.DisplayRectangle.Height / 2);
            await MoveMouseAsync(form, form.PointToScreen(mouseDragHandleOnForm));

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                    .LeftButtonDown()
                    .MoveMouseBy(form.DisplayRectangle.Width, 0)
                    .LeftButtonUp());

            Assert.True(form.DisplayRectangle.Width > originalFormSize.Width);
            Assert.Equal(originalFormSize.Height, form.DisplayRectangle.Height);

            Assert.Equal(originalButtonPosition.Location, button.DisplayRectangle.Location);

            // Still anchored on right
            Assert.Equal(originalFormSize.Width - originalButtonPosition.Right, form.DisplayRectangle.Width - button.DisplayRectangle.Right);
        });
    }

    [WinFormsFact]
    public async Task Button_Anchor_ResizeOnWindowSizeTallerAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;

            var originalFormSize = form.DisplayRectangle.Size;
            var originalButtonPosition = button.DisplayRectangle;

            Point mouseDragHandleOnForm = new(form.DisplayRectangle.Left + form.DisplayRectangle.Width / 2, form.DisplayRectangle.Bottom);
            await MoveMouseAsync(form, form.PointToScreen(mouseDragHandleOnForm));

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                    .LeftButtonDown()
                    .MoveMouseBy(0, form.DisplayRectangle.Height)
                    .LeftButtonUp());

            Assert.True(form.DisplayRectangle.Height > originalFormSize.Height);
            Assert.Equal(originalFormSize.Width, form.DisplayRectangle.Width);

            Assert.Equal(originalButtonPosition.Location, button.DisplayRectangle.Location);

            // Still anchored on bottom
            Assert.Equal(originalFormSize.Height - originalButtonPosition.Bottom, form.DisplayRectangle.Height - button.DisplayRectangle.Bottom);
        });
    }

    [WinFormsFact]
    public async Task Button_Mouse_Press_Without_Moving_Causes_Button_ClickAsync()
    {
        await RunControlPairTestAsync<Button, Button>(async (form, controls) =>
        {
            (Button control1, Button control2) = controls;
            int control1ClickCount = 0;
            int control2ClickCount = 0;
            control1.Click += (sender, e) => control1ClickCount++;
            control2.Click += (sender, e) => control2ClickCount++;

            await MoveMouseToControlAsync(control1);
            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Mouse.LeftButtonClick());

            Assert.Equal(1, control1ClickCount);
            Assert.Equal(0, control2ClickCount);

            await MoveMouseToControlAsync(control2);
            await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Mouse.LeftButtonClick());

            Assert.Equal(1, control1ClickCount);
            Assert.Equal(1, control2ClickCount);
        });
    }

    [WinFormsFact]
    public async Task Button_Mouse_Press_With_Drag_Off_Button_Does_Not_Cause_Button_ClickAsync()
    {
        await RunControlPairTestAsync<Button, Button>(async (form, controls) =>
        {
            (Button control1, Button control2) = controls;
            int control1ClickCount = 0;
            int control2ClickCount = 0;
            control1.Click += (sender, e) => control1ClickCount++;
            control2.Click += (sender, e) => control2ClickCount++;

            await MoveMouseToControlAsync(control1);
            Rectangle rect = control2.DisplayRectangle;
            Point centerOfRect = GetCenter(rect);
            Point centerOnScreen = control2.PointToScreen(centerOfRect);
            Size primaryMonitor = SystemInformation.PrimaryMonitorSize;
            int horizontalResolution = primaryMonitor.Width;
            int verticalResolution = primaryMonitor.Height;
            Point virtualPoint = new((int)Math.Round(65535.0 / horizontalResolution * centerOnScreen.X),
                (int)Math.Round(65535.0 / verticalResolution * centerOnScreen.Y));

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                    .LeftButtonDown()
                    .MoveMouseTo(virtualPoint.X, virtualPoint.Y)
                    .LeftButtonUp());

            Assert.Equal(0, control1ClickCount);
            Assert.Equal(0, control2ClickCount);
        });
    }

    [WinFormsFact]
    public async Task Button_Mouse_Press_With_Drag_Off_Button_And_Back_Does_Cause_Button_ClickAsync()
    {
        await RunControlPairTestAsync<Button, Button>(async (form, controls) =>
        {
            (Button control1, Button control2) = controls;
            int control1ClickCount = 0;
            int control2ClickCount = 0;
            control1.Click += (sender, e) => control1ClickCount++;
            control2.Click += (sender, e) => control2ClickCount++;

            await MoveMouseToControlAsync(control1);
            Rectangle rect = control2.DisplayRectangle;
            Point centerOfRect = GetCenter(rect);
            Point centerOnScreen = control2.PointToScreen(centerOfRect);
            Rectangle rect1 = control1.DisplayRectangle;
            Point centerOfRect1 = GetCenter(rect1);
            Point centerOnScreen1 = control1.PointToScreen(centerOfRect1);
            Size primaryMonitor = SystemInformation.PrimaryMonitorSize;
            int horizontalResolution = primaryMonitor.Width;
            int verticalResolution = primaryMonitor.Height;
            Point virtualPoint = new((int)Math.Round(65535.0 / horizontalResolution * centerOnScreen.X), (int)Math.Round(65535.0 / verticalResolution * centerOnScreen.Y));
            Point virtualPoint1 = new((int)Math.Round(65535.0 / horizontalResolution * centerOnScreen1.X), (int)Math.Round(65535.0 / verticalResolution * centerOnScreen1.Y));
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                    .LeftButtonDown()
                    .MoveMouseTo(virtualPoint.X, virtualPoint.Y)
                    .MoveMouseTo(virtualPoint1.X, virtualPoint1.Y)
                    .LeftButtonUp());

            Assert.Equal(1, control1ClickCount);
            Assert.Equal(0, control2ClickCount);
        });
    }

    [WinFormsFact]
    public async Task Button_PerformClick_Fires_OnClickAsync()
    {
        await RunTestAsync((form, button) =>
        {
            bool wasClicked = false;
            button.Click += (x, y) => wasClicked = true;

            button.PerformClick();

            Assert.True(wasClicked);

            return Task.CompletedTask;
        });
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11327")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X64,
       "Flaky tests, see: https://github.com/dotnet/winforms/issues/11327")]
    public async Task Button_Press_Enter_Fires_OnClickAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            bool wasClicked = false;

            button.Text = "&Click";
            button.Click += (x, y) => wasClicked = true;
            await MoveMouseToControlAsync(button);

            // Send the Enter press
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_RETURN));

            Assert.True(wasClicked);
        });
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11325")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X64,
       "Flaky tests, see: https://github.com/dotnet/winforms/issues/11325")]
    public async Task Button_Hotkey_Fires_OnClickAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            Assert.True(InputLanguage.CurrentInputLanguage.LayoutName == "US", "Please, switch to the US input language");
            bool wasClicked = false;

            button.Text = "&Click";
            button.Click += (x, y) => wasClicked = true;

            // Send the shortcut ALT+C (the same as SendKeys.SendWait("%C"))
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VIRTUAL_KEY.VK_LMENU, VIRTUAL_KEY.VK_C));

            Assert.True(wasClicked);
        });
    }

    [WinFormsFact]
    public async Task Button_Hotkey_DoesNotFire_OnClickAsync()
    {
        await RunTestAsync(async (form, button) =>
        {
            bool wasClicked = false;

            button.Text = "&Click";
            button.Click += (x, y) => wasClicked = true;

            // Send a random ALT+L (the same as SendKeys.SendWait("%l"))
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VIRTUAL_KEY.VK_LMENU, VIRTUAL_KEY.VK_L));

            Assert.False(wasClicked);
        });
    }

    private async Task RunTestAsync(Func<Form, Button, Task> runTest)
    {
        await RunSingleControlTestAsync(
            testDriverAsync: runTest,
            createControl: () =>
            {
                Button control = new()
                {
                    Location = new Point(0, 0),
                };

                return control;
            },
            createForm: () =>
            {
                return new()
                {
                    Size = new(500, 300),
                };
            });
    }
}
