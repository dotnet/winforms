// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal.TestUtilities;
using System.Windows.Forms.UITests;
using Xunit;
using Xunit.Abstractions;
using static Interop.UiaCore;

namespace System.Windows.Forms.PropertyGridInternal.UITests;

public class PropertyGridView_DropDownHolder_DropDownHolderAccessibleObjectTests : ControlTestBase
{
    public PropertyGridView_DropDownHolder_DropDownHolderAccessibleObjectTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task DropDownHolderAccessibleObject_FragmentNavigate_Parent_IsSelectedEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
                Assert.Equal(grid.SelectedEntry.AccessibilityObject,
                    grid.GridView.DropDownControlHolder.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent)));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task DropDownHolderAccessibleObject_FragmentNavigate_PreviousSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
                Assert.Null(grid.GridView.DropDownControlHolder.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling)));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task DropDownHolderAccessibleObject_FragmentNavigate_NextSibling_IsTextBoxAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
                Assert.Equal(grid.GridView.EditAccessibleObject,
                    grid.GridView.DropDownControlHolder.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling)));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task DropDownHolderAccessibleObject_FragmentNavigate_FirstLastChild_IsListBoxAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
            {
                Assert.Equal(grid.GridView.DropDownListBoxAccessibleObject,
                    grid.GridView.DropDownControlHolder.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Equal(grid.GridView.DropDownListBoxAccessibleObject,
                    grid.GridView.DropDownControlHolder.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));
            });

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task DropDownHolderAccessibleObject_FragmentNavigate_FirstLastChild_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Anchor)];

            grid.PopupEditorAndClose(() =>
            {
                Assert.Null(grid.GridView.DropDownControlHolder.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(grid.GridView.DropDownControlHolder.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));
            });

            return Task.CompletedTask;
        });
    }
}
