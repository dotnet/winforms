// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using WindowsInput.Native;
using Xunit;
using Xunit.Abstractions;
using static System.Windows.Forms.MonthCalendar;
using static Interop;
using static Interop.ComCtl32;
using static Interop.Kernel32;
using static Interop.User32;

namespace System.Windows.Forms.UITests
{
    public class ListViewTests : ControlTestBase
    {
        public ListViewTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public async Task Click_On_Second_Column_Does_Not_Alter_CheckboxesAsync()
        {
            await RunTestAsync(async (form, listView) =>
            {
                InitializeItems(listView, View.Details, virtualModeEnabled: false, checkBoxesEnabled: true);

                foreach (ListViewItem item in listView.Items)
                {
                    Assert.Equal(0, item.StateImageIndex);
                }

                foreach (ListViewItem item in listView.Items)
                {
                    Assert.True(item.Selected);
                }

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Keyboard.KeyDown(VirtualKeyCode.SHIFT));

                var pt = MouseHelper.GetCenter(listView.RectangleToScreen(listView.Items[0].SubItems[1].Bounds));
                MouseHelper.SendClick(pt.X, pt.Y);

                pt = MouseHelper.GetCenter(listView.RectangleToScreen(listView.Items[2].SubItems[1].Bounds));
                MouseHelper.SendClick(pt.X, pt.Y);

                KeyboardHelper.SendKey(Keys.ShiftKey, false);
                Application.DoEvents();

                foreach (ListViewItem item in listView.Items)
                {
                    if (item.StateImageIndex != 0)
                        return new ScenarioResult(false, "All checkboxes must be unmarked");
                }

                foreach (ListViewItem item in listView.Items)
                {
                    if (!item.Selected)
                        return new ScenarioResult(false, "All items must be selected");
                }

                return new ScenarioResult(true);
            });
        }

        private void InitializeItems(ListView listView, View view, bool virtualModeEnabled, bool checkBoxesEnabled)
        {
            var columnHeader1 = new ColumnHeader { Text = "ColumnHeader1", Width = 140 };
            var columnHeader2 = new ColumnHeader { Text = "ColumnHeader2", Width = 140 };
            var columnHeader3 = new ColumnHeader { Text = "ColumnHeader3", Width = 140 };
            listView.Columns.AddRange(new[] { columnHeader1, columnHeader2, columnHeader3 });

            var listViewItem1 = new ListViewItem(new[] { "row1", "row1Col2", "row1Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem2 = new ListViewItem(new[] { "row2", "row2Col2", "row2Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem3 = new ListViewItem(new[] { "row3", "row3Col2", "row3Col3" }, -1) { StateImageIndex = 0 };

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
                    _ => listViewItem1,
                };
            };

            listView.Items.Clear();
            listView.CheckBoxes = checkBoxesEnabled;
            listView.FullRowSelect = true;
            listView.View = view;
            listView.VirtualMode = virtualModeEnabled;
            listView.VirtualListSize = 3;

            if (!virtualModeEnabled)
            {
                listView.Items.AddRange(new[] { listViewItem1, listViewItem2, listViewItem3 });
            }
        }

        private async Task RunTestAsync(Func<Form, ListView, Task> runTest)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    ListView control = new()
                    {
                        Location = new Point(0, 0),
                        Size = new Size(439, 103)
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
}
