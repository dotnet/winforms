﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using WindowsInput.Native;
using Xunit;
using Xunit.Abstractions;
using static Interop;

namespace System.Windows.Forms.UITests
{
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
            await RunSingleControlTestAsync<Button>(async (form, button) =>
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

        [WinFormsFact]
        public async Task Button_DialogResult_SpaceToClickFocusedButtonAsync()
        {
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                button.DialogResult = DialogResult.OK;

                Assert.True(button.Focus());

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.SPACE));

                Assert.Equal(DialogResult.OK, form.DialogResult);
                Assert.False(form.Visible);
            });
        }

        [WinFormsFact]
        public async Task Button_DialogResult_EscapeDoesNotClickFocusedButtonAsync()
        {
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                button.DialogResult = DialogResult.OK;

                Assert.True(button.Focus());

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE));

                Assert.Equal(DialogResult.None, form.DialogResult);
                Assert.True(form.Visible);
            });
        }

        [WinFormsFact]
        public async Task Button_CancelButton_EscapeClicksCancelButtonAsync()
        {
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                form.CancelButton = button;

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard.KeyPress(VirtualKeyCode.ESCAPE));

                Assert.Equal(DialogResult.Cancel, form.DialogResult);
                Assert.False(form.Visible);
            });
        }

        [WinFormsFact]
        public async Task Button_AchorNone_NoResizeOnWindowSizeWiderAsync()
        {
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                var originalFormSize = form.DisplayRectangle.Size;
                var originalButtonPosition = button.DisplayRectangle;

                var mouseDragHandleOnForm = new Point(form.DisplayRectangle.Right, form.DisplayRectangle.Top + form.DisplayRectangle.Height / 2);
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
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                var originalFormSize = form.DisplayRectangle.Size;
                var originalButtonPosition = button.DisplayRectangle;

                var mouseDragHandleOnForm = new Point(form.DisplayRectangle.Left + form.DisplayRectangle.Width / 2, form.DisplayRectangle.Bottom + 1);
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
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                button.Anchor = AnchorStyles.Left | AnchorStyles.Right;

                var originalFormSize = form.DisplayRectangle.Size;
                var originalButtonPosition = button.DisplayRectangle;

                var mouseDragHandleOnForm = new Point(form.DisplayRectangle.Right, form.DisplayRectangle.Top + form.DisplayRectangle.Height / 2);
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
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                button.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;

                var originalFormSize = form.DisplayRectangle.Size;
                var originalButtonPosition = button.DisplayRectangle;

                var mouseDragHandleOnForm = new Point(form.DisplayRectangle.Left + form.DisplayRectangle.Width / 2, form.DisplayRectangle.Bottom + 1);
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
        public async Task Button_Click_DragAfterMouseDownAsync()
        {
            await RunControlPairTestAsync<Button>(async (form, controls) =>
            {
                (Button control1, Button control2) = controls;

                int control1ClickCount = 0;
                int control2ClickCount = 0;
                control1.Click += (sender, e) => control1ClickCount++;
                control2.Click += (sender, e) => control2ClickCount++;

                // Verify mouse press without moving causes a button click
                await MoveMouseToControlAsync(control1);
                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Mouse.LeftButtonDown().LeftButtonUp());
                Assert.Equal(1, control1ClickCount);
                Assert.Equal(0, control2ClickCount);

                // Verify mouse press without moving causes a button click
                await MoveMouseToControlAsync(control2);
                await InputSimulator.SendAsync(form, inputSimulator => inputSimulator.Mouse.LeftButtonDown().LeftButtonUp());
                Assert.Equal(1, control1ClickCount);
                Assert.Equal(1, control2ClickCount);

                // Verify that mouse press and then drag off the button does not cause a button click of either button
                await MoveMouseToControlAsync(control1);

                Rectangle rect = control2.DisplayRectangle;
                Point centerOfRect = new Point(rect.Left, rect.Top) + new Size(rect.Width / 2, rect.Height / 2);
                Point centerOnScreen = control2.PointToScreen(centerOfRect);
                int horizontalResolution = User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN);
                int verticalResolution = User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN);
                var virtualPoint = new Point((int)Math.Round(65535.0 / horizontalResolution * centerOnScreen.X), (int)Math.Round(65535.0 / verticalResolution * centerOnScreen.Y));

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse
                        .LeftButtonDown()
                        .MoveMouseTo(virtualPoint.X + 1, virtualPoint.Y + 1)
                        .LeftButtonUp());

                ////Assert.False(control1.MouseIsOver);
                ////Assert.True(control2.MouseIsOver);

                Assert.Equal(1, control1ClickCount);
                Assert.Equal(1, control2ClickCount);

                // Verify that mouse press and then drag off the button and back causes a button click
                await MoveMouseToControlAsync(control1);

                Rectangle rect1 = control1.DisplayRectangle;
                Point centerOfRect1 = new Point(rect1.Left, rect1.Top) + new Size(rect1.Width / 2, rect1.Height / 2);
                Point centerOnScreen1 = control1.PointToScreen(centerOfRect1);
                int horizontalResolution1 = User32.GetSystemMetrics(User32.SystemMetric.SM_CXSCREEN);
                int verticalResolution1 = User32.GetSystemMetrics(User32.SystemMetric.SM_CYSCREEN);
                var virtualPoint1 = new Point((int)Math.Round(65535.0 / horizontalResolution1 * centerOnScreen1.X), (int)Math.Round(65535.0 / verticalResolution1 * centerOnScreen1.Y));

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse
                        .LeftButtonDown()
                        .MoveMouseTo(virtualPoint.X + 1, virtualPoint.Y + 1)
                        .MoveMouseTo(virtualPoint1.X + 1, virtualPoint1.Y + 1)
                        .LeftButtonUp());

                ////Assert.False(control1.MouseIsOver);
                ////Assert.True(control2.MouseIsOver);

                Assert.Equal(2, control1ClickCount);
                Assert.Equal(1, control2ClickCount);
            });
        }

        [WinFormsFact]
        public async Task Button_PerformClick_Fires_OnClickAsync()
        {
            await RunSingleControlTestAsync<Button>((form, button) =>
            {
                bool wasClicked = false;
                button.Click += (x, y) => wasClicked = true;

                button.PerformClick();

                Assert.True(wasClicked);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task Button_Hotkey_Fires_OnClickAsync()
        {
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                bool wasClicked = false;

                button.Text = "&Click";
                button.Click += (x, y) => wasClicked = true;

                // Send the shortcut ALT+C (the same as SendKeys.SendWait("%C"))
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LMENU, VirtualKeyCode.VK_C));

                Assert.True(wasClicked);
            });
        }

        [WinFormsFact]
        public async Task Button_Hotkey_DoesNotFire_OnClickAsync()
        {
            await RunSingleControlTestAsync<Button>(async (form, button) =>
            {
                bool wasClicked = false;

                button.Text = "&Click";
                button.Click += (x, y) => wasClicked = true;

                // Send a random ALT+L (the same as SendKeys.SendWait("%l"))
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LMENU, VirtualKeyCode.VK_L));

                Assert.False(wasClicked);
            });
        }
    }
}
