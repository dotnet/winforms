// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal.TestUtilities;
using System.Windows.Forms.UITests;
using Xunit;
using Xunit.Abstractions;
using static Interop.UiaCore;

namespace System.Windows.Forms.PropertyGridInternal.UITests;

public class PropertyDescriptorGridEntryAccessibleObjectTests : ControlTestBase
{
    public PropertyDescriptorGridEntryAccessibleObjectTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_Parent_IsParentEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];
            entry.Expanded = true;

            Assert.Equal(entry.AccessibilityObject,
                entry.Children.Cast<GridEntry>().First().AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_PreviousSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];
            entry.Expanded = true;

            Assert.Null(entry.Children.Cast<GridEntry>().First().AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_NextSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];
            entry.Expanded = true;

            Assert.Null(entry.Children.Cast<GridEntry>().Last().AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_FirstChild_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            Assert.Null(grid[nameof(Button.Font)].AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_FirstChild_IsChildEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];
            entry.Expanded = true;

            Assert.Equal(entry.Children.Cast<GridEntry>().First().AccessibilityObject,
                entry.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_FirstChild_IsTextBoxAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            Assert.Equal(grid.GridView.EditAccessibleObject,
                grid.SelectedEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_LastChild_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            Assert.Null(grid[nameof(Button.Font)].AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_LastChild_IsChildEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];
            entry.Expanded = true;

            Assert.Equal(entry.Children.Cast<GridEntry>().Last().AccessibilityObject,
                entry.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_LastChild_IsTextBoxAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Size)];

            Assert.Equal(grid.GridView.EditAccessibleObject,
                grid.SelectedEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_LastChild_IsDropDownButtonAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            Assert.Equal(grid.GridView.DropDownButton.AccessibilityObject,
                grid.SelectedEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

            return Task.CompletedTask;
        });
    }
}
