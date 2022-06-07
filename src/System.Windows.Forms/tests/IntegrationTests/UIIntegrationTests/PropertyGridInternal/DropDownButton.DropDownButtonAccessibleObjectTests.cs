// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal.TestUtilities;
using System.Windows.Forms.UITests;
using Xunit;
using Xunit.Abstractions;
using static Interop.UiaCore;

namespace System.Windows.Forms.PropertyGridInternal.UITests;

public class DropDownButtonAccessibleObjectTests : ControlTestBase
{
    public DropDownButtonAccessibleObjectTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task DropDownButtonAccessibleObject_FragmentNavigate_Parent_IsSelectedEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            Assert.Equal(grid.SelectedEntry.AccessibilityObject,
                grid.GridView.DropDownButton.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task DropDownButtonAccessibleObject_FragmentNavigate_PreviousSibling_IsTextBoxAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            Assert.Equal(grid.GridView.EditAccessibleObject,
                grid.GridView.DropDownButton.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task DropDownButtonAccessibleObject_FragmentNavigate_NextSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            Assert.Null(grid.GridView.DropDownButton.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));

            return Task.CompletedTask;
        });
    }
}
