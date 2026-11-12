// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms.UITests;

/// <summary>
///  Tests the <see cref="CheckedListBox"/> control with real mouse and keyboard input.
/// </summary>
public class CheckedListBoxTests : ControlTestBase
{
    public CheckedListBoxTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task CheckedListBox_ClickAfterKeyboardNavigation_RaisesClickAndChecksItemAsync()
    {
        await RunSingleControlTestAsync(
            async (form, control) =>
            {
                int clickCount = 0;
                control.Click += (sender, e) => clickCount++;
                Assert.True(control.Focus());

                await ClickItemAsync(form, control, 0);

                Assert.Equal(1, clickCount);
                Assert.Equal(0, control.SelectedIndex);
                Assert.True(control.GetItemChecked(0));

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard.KeyPress(VIRTUAL_KEY.VK_DOWN));

                Assert.Equal(1, control.SelectedIndex);
                Assert.False(control.GetItemChecked(1));

                await ClickItemAsync(form, control, 2);

                Assert.Equal(2, clickCount);
                Assert.Equal(2, control.SelectedIndex);
                Assert.True(control.GetItemChecked(2));
            },
            createControl: () =>
            {
                CheckedListBox control = new()
                {
                    CheckOnClick = true
                };
                control.Items.AddRange(["zero", "one", "two"]);

                return control;
            });
    }

    private async Task ClickItemAsync(Form form, CheckedListBox control, int index)
    {
        Rectangle itemRectangle = control.GetItemRectangle(index);
        Point itemCenter = control.PointToScreen(GetCenter(itemRectangle));
        await MoveMouseAsync(form, itemCenter);
        await InputSimulator.SendAsync(
            form,
            inputSimulator => inputSimulator.Mouse.LeftButtonClick());
    }
}
