﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.PropertyGridInternal.TestUtilities;
using System.Windows.Forms.UITests;
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
                entry.Children.First().AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));

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

            Assert.Null(entry.Children.First().AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_PreviousSibling_IsChildEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];
            entry.Expanded = true;

            Assert.Equal(entry.Children.First().AccessibilityObject,
                entry.Children[1].AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_PreviousSibling_IsTextBoxAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Size)];
            grid.SelectedEntry.Expanded = true;

            Assert.Equal(grid.GridView.EditAccessibleObject,
                grid.SelectedEntry.Children.First().AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_PreviousSibling_IsDialogButtonAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Font)];
            grid.SelectedEntry.Expanded = true;

            Assert.Equal(grid.GridView.DialogButton.AccessibilityObject,
                grid.SelectedEntry.Children.First().AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));

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

            Assert.Null(entry.Children.Last().AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_NextSibling_IsChildEntryAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];
            entry.Expanded = true;

            Assert.Equal(entry.Children[1].AccessibilityObject,
                entry.Children.First().AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));

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

            Assert.Equal(entry.Children.First().AccessibilityObject,
                entry.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_FirstChild_IsNullAfterExpandCollapsedAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];

            entry.Expanded = true;
            entry.Expanded = false;

            Assert.Null(entry.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));

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
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_FirstChild_IsDropDownHolderAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
                Assert.Equal(grid.GridView.DropDownControlHolder.AccessibilityObject,
                    grid.SelectedEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild)));

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

            Assert.Equal(entry.Children.Last().AccessibilityObject,
                entry.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

            return Task.CompletedTask;
        });
    }

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_LastChild_IsNullAfterExpandCollapsedAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            GridEntry entry = grid[nameof(Button.Font)];

            entry.Expanded = true;
            entry.Expanded = false;

            Assert.Null(entry.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

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

    [WinFormsFact]
    public async Task PropertyDescriptorGridEntryAccessibleObject_FragmentNavigate_LastChild_IsDialogButtonAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.Font)];

            Assert.Equal(grid.GridView.DialogButton.AccessibilityObject,
                grid.SelectedEntry.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

            return Task.CompletedTask;
        });
    }
}
