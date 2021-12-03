// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ComboBoxTests : ControlTestBase
    {
        public ComboBoxTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public async Task ComboBox_Select_Item_By_UpArrowKeyAsync()
        {
            await RunComboBoxTestAsync(async (form, comboBox) =>
            {
                StringBuilder stringBuilder = new();
                int numEvents = 0;
                comboBox.SelectedIndexChanged += (x, y) => numEvents++;

                int numKeyPresses = 9;
                for (int i = 0; i < numKeyPresses; i++)
                {
                    await Task.Delay(100);
                    await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Keyboard
                        .Sleep(100)
                        .KeyPress(WindowsInput.Native.VirtualKeyCode.UP));

                    await Task.Delay(100);

                    stringBuilder.Append($"i:{i + 1}, numEvents:{numEvents}\n");
                    // TestOutputHelper.WriteLine($"i:{i + 1}, numEvents:{numEvents}");
                }

                TestOutputHelper.WriteLine($"{stringBuilder}");
                Assert.Equal(numKeyPresses, numEvents);
            },
            numberOfItemsToAdd: 10,
            selectedIndex: 9);
        }

        [WinFormsFact]
        public async Task ComboBox_Select_Item_By_DownArrowKeyAsync()
        {
            await RunComboBoxTestAsync(async (form, comboBox) =>
            {
                int numEvents = 0;
                comboBox.SelectedIndexChanged += (x, y) => numEvents++;

                int numKeyPresses = 9;
                for (int i = 0; i < numKeyPresses; i++)
                {
                    await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.DOWN));

                    Assert.Equal(i + 1, numEvents);
                }

                Assert.Equal(numKeyPresses, numEvents);
            },
            numberOfItemsToAdd: 10,
            selectedIndex: 0);
        }

        private void AddItems(ComboBox comboBox, int numToAdd)
        {
            for (int i = 0; i < numToAdd; i++)
            {
                comboBox.Items.Add($"Item {i}");
            }
        }

        private async Task RunComboBoxTestAsync(Func<Form, ComboBox, Task> runTest, int numberOfItemsToAdd, int selectedIndex)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    ComboBox control = new();
                    AddItems(control, numberOfItemsToAdd);
                    control.SelectedIndex = selectedIndex;
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
}
