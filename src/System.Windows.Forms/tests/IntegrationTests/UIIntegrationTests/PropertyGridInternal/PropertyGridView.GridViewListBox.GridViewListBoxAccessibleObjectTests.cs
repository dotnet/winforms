// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.PropertyGridInternal.TestUtilities;
using System.Windows.Forms.UITests;
using Windows.Win32.UI.Accessibility;
using Xunit.Abstractions;

namespace System.Windows.Forms.PropertyGridInternal.UITests;

public class PropertyGridView_GridViewListBox_GridViewListBoxAccessibleObjectTests : ControlTestBase
{
    public PropertyGridView_GridViewListBox_GridViewListBoxAccessibleObjectTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task GridViewListBoxAccessibleObject_FragmentNavigate_Parent_IsDropDownHolderAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
            {
                Assert.Equal(
                    grid.GridView.DropDownControlHolder!.AccessibilityObject,
                    grid.GridView.DropDownListBoxAccessibleObject!.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            });

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewListBoxAccessibleObject_FragmentNavigate_PreviousSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
            {
                Assert.Null(grid.GridView.DropDownListBoxAccessibleObject!.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            });

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task GridViewListBoxAccessibleObject_FragmentNavigate_NextSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
            {
                Assert.Null(grid.GridView.DropDownListBoxAccessibleObject!.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            });

            return Task.CompletedTask;
        });
    }
}
