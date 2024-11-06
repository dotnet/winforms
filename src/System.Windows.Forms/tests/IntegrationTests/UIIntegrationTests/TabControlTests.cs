// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class TabControlTests : ControlTestBase
{
    public TabControlTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task TabControl_TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_FirstTabAsync()
    {
        await RunTestAsync(async (form, tabControl) =>
        {
            // TODO: find the way to determin the tab's dimensions and use those instead of (10, 10)
            bool result = await IsHoveredWithMouseAsync(form, tabControl, new(10, 10));

            Assert.True(result);
        });
    }

    [WinFormsFact]
    public async Task TabControl_TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_SecondTabAsync()
    {
        await RunTestAsync(async (form, tabControl) =>
        {
            // TODO: find the way to determin the tab's dimensions and use those instead of (60, 10)
            bool result = await IsHoveredWithMouseAsync(form, tabControl, new(60, 10));

            Assert.True(result);
        });
    }

    [WinFormsFact]
    public async Task TabControl_TabPage_IsHoveredWithMouse_IsFalse_WhenMouseIs_OutsideControlAsync()
    {
        await RunTestAsync(async (form, tabControl) =>
        {
            // TODO: find the way to determin the tab's dimensions and use those instead of (300, 300)
            bool result = await IsHoveredWithMouseAsync(form, tabControl, new(300, 300));

            Assert.False(result);
        });
    }

    [WinFormsFact]
    public async Task TabControl_TabPage_IsHoveredWithMouse_IsFalse_WhenMouseIs_OutsideMainScreenAsync()
    {
        await RunTestAsync(async (form, tabControl) =>
        {
            TestOutputHelper.WriteLine($"Primary screen: {Screen.PrimaryScreen?.WorkingArea}");

            // We can't physically move the mouse outside the desktop area,
            // so the mouse cursor will be stuck at the bottom right corner of the desktop.
            bool result = await IsHoveredWithMouseAsync(form, tabControl, new(1000, 1000), assertCorrectLocation: false);

            Assert.False(result);
        });
    }

    private async Task<bool> IsHoveredWithMouseAsync(Form form, TabControl tabControl, Point point, bool assertCorrectLocation = true)
    {
        await MoveMouseAsync(form, tabControl.PointToScreen(point), assertCorrectLocation);

        bool result = ((IKeyboardToolTip)tabControl).IsHoveredWithMouse();
        bool resultOfPage1 = ((IKeyboardToolTip)tabControl.TabPages[0]).IsHoveredWithMouse();
        bool resultOfPage2 = ((IKeyboardToolTip)tabControl.TabPages[1]).IsHoveredWithMouse();
        return result && resultOfPage1 && resultOfPage2;
    }

    private async Task RunTestAsync(Func<Form, TabControl, Task> runTest)
    {
        await RunSingleControlTestAsync(
            testDriverAsync: runTest,
            createControl: () =>
            {
                TabControl tabControl = new()
                {
                    Location = new Point(0, 0)
                };

                TabPage tabPage1 = new();
                TabPage tabPage2 = new();
                tabControl.TabPages.Add(tabPage1);
                tabControl.TabPages.Add(tabPage2);

                return tabControl;
            });
    }

    [WinFormsFact]
    public async Task TabControl_ControlsDoNotReorderWhenSelectedIndexChanges()
    {
        // Validates the following bug fix: https://github.com/dotnet/winforms/issues/7837
        await RunSingleControlTestAsync(
            testDriverAsync: async (form, tabControl) =>
            {
                var originalPages = tabControl.TabPages.Cast<TabPage>().ToArray();

                tabControl.SelectedIndex--;
                await Task.Yield();

                // The changes we made to SelectedIndex will result in a WM_WINDOWPOSCHANGED message which will
                // fire TabPage.WmWindowPosChanged, which will call TabControl.UpdateChildControlIndex.

                // This test ensures that the latter method short-circuits and does not call Controls.SetChildIndex
                // which would inappropriately reorder the Controls collection, resulting in TabPages and Controls
                // becoming out of sync.

                for (int i = 0; i < originalPages.Length; i++)
                {
                    Assert.Equal(tabControl.Controls[i], tabControl.TabPages[i]);
                }

                // The following test proves that it *matters* that the Controls and TabPages collection remain in sync.

                // Remove the first page by position:
                tabControl.TabPages.RemoveAt(0);

                // We should be left with pages[1] and pages[2]. This will not be the case if the Controls and TabPages
                // collections are out of sync.
                Assert.Equal(originalPages[1], tabControl.TabPages[0]);
                Assert.Equal(originalPages[2], tabControl.TabPages[1]);
            },
            createControl: () =>
            {
                SubclassedTabControl tabControl = new()
                {
                    Location = new Point(0, 0)
                };

                TabPage tabPage1 = new() { Text = "Page 1" };
                TabPage tabPage2 = new() { Text = "Page 2" };
                TabPage tabPage3 = new() { Text = "Page 3" };

                tabControl.TabPages.Add(tabPage1);
                tabControl.TabPages.Add(tabPage2);
                tabControl.TabPages.Add(tabPage3);

                // Start with the Selected Page at the end:
                tabControl.SelectedIndex = tabControl.TabPages.Count - 1;

                return tabControl;
            });
    }

    // Bug https://github.com/dotnet/winforms/issues/7837 occured only when TabControl was subclassed.
    private class SubclassedTabControl : TabControl { }
}
