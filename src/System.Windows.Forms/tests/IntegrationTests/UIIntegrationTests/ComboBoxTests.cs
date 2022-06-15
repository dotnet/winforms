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
        public async Task ComboBoxTest_ChangeAutoCompleteSource_DoesntThrowAsync()
        {
            await RunSingleControlTestAsync<ComboBox>((form, comboBox) =>
            {
                // Test case captured from here.
                // https://github.com/dotnet/winforms/issues/6953
                comboBox.AutoCompleteCustomSource.AddRange(new[]
                {
                    "_sss",
                    "_sss"
                });
                comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
                comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task ComboBox_Select_Item_By_UpArrowKeyAsync()
        {
            int expectedKeyPressesCount = 9;
            await RunComboBoxTestAsync(async (form, comboBox) =>
            {
                StringBuilder stringBuilder = new();

                // Reset EventsCount because the control gets selected right after the creation.
                comboBox.ResetEventsCount();
                for (int i = 0; i < expectedKeyPressesCount; i++)
                {
                    await Task.Delay(100);
                    await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Keyboard
                        .Sleep(100)
                        .KeyPress(WindowsInput.Native.VirtualKeyCode.UP));

                    await Task.Delay(100);
                    stringBuilder.AppendLine($"i:{i + 1}, eventsCount:{comboBox.EventsCount}");
                }

                TestOutputHelper.WriteLine($"{stringBuilder}");
                Assert.Equal(expectedKeyPressesCount, comboBox.EventsCount);
            },
            expectedKeyPressesCount,
            selectedIndex: 9);
        }

        [WinFormsFact]
        public async Task ComboBox_Select_Item_By_DownArrowKeyAsync()
        {
            int expectedKeyPressesCount = 9;
            await RunComboBoxTestAsync(async (form, comboBox) =>
            {
                // Reset EventsCount because the control gets selected right after the creation.
                comboBox.ResetEventsCount();
                for (int i = 0; i < expectedKeyPressesCount; i++)
                {
                    await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.DOWN));

                    Assert.Equal(i + 1, comboBox.EventsCount);
                }

                Assert.Equal(expectedKeyPressesCount, comboBox.EventsCount);
            },
            expectedKeyPressesCount,
            selectedIndex: 0);
        }

        private async Task RunComboBoxTestAsync(Func<Form, ComboBoxWithSelectCounter, Task> runTest, int numberOfItemsToAdd, int selectedIndex)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    ComboBoxWithSelectCounter control = new();
                    control.AddItems(numberOfItemsToAdd);
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

        private class ComboBoxWithSelectCounter : ComboBox
        {
            public int EventsCount { get; private set; }

            public void AddItems(int numToAdd)
            {
                for (int i = 0; i <= numToAdd; i++)
                {
                    Items.Add($"Item {i}");
                }
            }

            protected override void OnSelectedIndexChanged(EventArgs e)
            {
                base.OnSelectedIndexChanged(e);
                EventsCount++;
            }

            public void ResetEventsCount()
            {
                EventsCount = 0;
            }
        }
    }
}
