// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.PropertyGridInternal.TestUtilities;
using System.Windows.Forms.UITests;
using Windows.Win32.UI.Accessibility;
using Xunit.Abstractions;

namespace System.Windows.Forms.PropertyGridInternal.UITests;

public class PropertyGridView_GridViewTextBox_GridViewTextBoxAccessibleObjectTests : ControlTestBase
{
    public PropertyGridView_GridViewTextBox_GridViewTextBoxAccessibleObjectTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task GridViewTextBoxAccessibleObject_FragmentNavigate_Parent_IsSelectedEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Size)];

            Assert.Equal(grid.SelectedEntry.AccessibilityObject,
                grid.GridView.EditAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewTextBoxAccessibleObject_FragmentNavigate_PreviousSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Size)];

            Assert.Null(grid.GridView.EditAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewTextBoxAccessibleObject_FragmentNavigate_PreviousSibling_IsDropDownHolderAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.FlatStyle)];

            grid.PopupEditorAndClose(onClosingAction: () =>
                Assert.Equal(
                    grid.GridView.DropDownControlHolder!.AccessibilityObject,
                    grid.GridView.EditAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling)));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewTextBoxAccessibleObject_FragmentNavigate_NextSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Size)];

            Assert.Null(grid.GridView.EditAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewTextBoxAccessibleObject_FragmentNavigate_NextSibling_IsChildEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Size)];
            grid.SelectedEntry.Expanded = true;

            Assert.Equal(
                grid.SelectedEntry.Children.First().AccessibilityObject,
                grid.GridView.EditAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewTextBoxAccessibleObject_FragmentNavigate_NextSibling_IsDropDownButtonAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.FlatStyle)];

            Assert.Equal(grid.GridView.DropDownButton.AccessibilityObject,
                grid.GridView.EditAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewTextBoxAccessibleObject_FragmentNavigate_NextSibling_IsDialogButtonAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Font)];

            Assert.Equal(grid.GridView.DialogButton.AccessibilityObject,
                grid.GridView.EditAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

            return Task.CompletedTask;
        });
    }
}
