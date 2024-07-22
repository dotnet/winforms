﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListBox;
using System.Drawing;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void ListBoxAccessibleObjectTests_Ctor_Default()
    {
        using ListBox listBox = InitializeListBoxWithItems();

        int childCount = listBox.AccessibilityObject.GetChildCount();

        for (int i = 0; i < childCount; i++)
        {
            var child = listBox.AccessibilityObject.GetChild(i);
            Assert.True(child.IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollItemPatternId));
        }
    }

    [WinFormsFact]
    public void ListBoxAccessibleObject_ControlType_IsList_IfAccessibleRoleIsDefault()
    {
        using ListBox listBox = new();
        // AccessibleRole is not set = Default

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)listBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ListControlTypeId, actual);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.List)]
    [InlineData(false, AccessibleRole.None)]
    public void ListBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using ListBox listBox = new();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            listBox.CreateControl();
        }

        AccessibleRole actual = listBox.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.Equal(createControl, listBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
    {
        Array roles = Enum.GetValues(typeof(AccessibleRole));

        foreach (AccessibleRole role in roles)
        {
            if (role == AccessibleRole.Default)
            {
                continue; // The test checks custom roles
            }

            yield return new object[] { role };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ListBox listBox = new();
        listBox.AccessibleRole = role;

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)listBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected()
    {
        using ListBox listBox = new();
        AccessibleObject accessibleObject = listBox.AccessibilityObject;

        Assert.Equal(VARIANT.Empty, accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBox_ReleaseUiaProvider_ClearsItemsAccessibleObjects()
    {
        using ListBox listBox = InitializeListBoxWithItems();
        ListBoxAccessibleObject accessibleObject = InitListBoxItemsAccessibleObjects(listBox);

        listBox.ReleaseUiaProvider(listBox.HWND);

        Assert.Equal(0, accessibleObject.TestAccessor().Dynamic._itemAccessibleObjects.Count);
    }

    [WinFormsFact]
    public void ListBoxItems_Clear_ClearsItemsAccessibleObjects()
    {
        using ListBox listBox = InitializeListBoxWithItems();
        ListBoxAccessibleObject accessibleObject = InitListBoxItemsAccessibleObjects(listBox);

        listBox.Items.Clear();

        Assert.Equal(0, accessibleObject.TestAccessor().Dynamic._itemAccessibleObjects.Count);
    }

    [WinFormsFact]
    public void ListBoxItems_Remove_RemovesItemAccessibleObject()
    {
        using ListBox listBox = InitializeListBoxWithItems();
        ListBoxAccessibleObject accessibleObject = InitListBoxItemsAccessibleObjects(listBox);
        ItemArray.Entry item = listBox.Items.InnerArray.Entries[0];
        Assert.True(accessibleObject.TestAccessor().Dynamic._itemAccessibleObjects.ContainsKey(item));

        listBox.Items.Remove(item);

        Assert.False(accessibleObject.TestAccessor().Dynamic._itemAccessibleObjects.ContainsKey(item));
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_HasKeyboardFocusPropertyId_ReturnsExpected()
    {
        using Form form = new();
        using ListBox listBox = new();

        form.Controls.Add(listBox);

        ListBoxAccessibleObject accessibleObject = new(listBox);
        form.Show();

        Assert.True(listBox.Focused);

        bool actual = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);
        Assert.True(actual);

        listBox.Items.Add(item: "testItem");
        actual = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.False(actual);

        listBox.Items.Clear();
        actual = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId);

        Assert.True(actual);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.One, true)]
    [InlineData(SelectionMode.None, false)]
    [InlineData(SelectionMode.MultiSimple, true)]
    [InlineData(SelectionMode.MultiExtended, true)]
    public unsafe void ListBoxItemAccessibleObject_GetPropertyValue_IsSelectionRequired(SelectionMode mode, bool expected)
    {
        using ListBox listBox = InitializeListBoxWithItems();
        listBox.SelectionMode = mode;
        listBox.CreateControl();

        var listBoxAccessibleObject = listBox.AccessibilityObject;
        var actual = listBoxAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SelectionIsSelectionRequiredPropertyId);

        Assert.Equal(expected, (bool)actual);

        ISelectionProvider.Interface provider = listBoxAccessibleObject;
        Assert.True(provider.get_IsSelectionRequired(out BOOL result).Succeeded);
        Assert.Equal(expected, (bool)result);
    }

    [WinFormsTheory]
    [InlineData(SelectionMode.One, false)]
    [InlineData(SelectionMode.None, false)]
    [InlineData(SelectionMode.MultiSimple, true)]
    [InlineData(SelectionMode.MultiExtended, true)]
    public unsafe void ListBoxItemAccessibleObject_GetPropertyValue_CanSelectMultiple(SelectionMode mode, bool expected)
    {
        using ListBox listBox = InitializeListBoxWithItems();
        listBox.SelectionMode = mode;
        listBox.CreateControl();

        var listBoxAccessibleObject = listBox.AccessibilityObject;
        var actual = listBoxAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SelectionCanSelectMultiplePropertyId);

        Assert.Equal(expected, (bool)actual);

        ISelectionProvider.Interface provider = listBoxAccessibleObject;
        Assert.True(provider.get_CanSelectMultiple(out BOOL result).Succeeded);
        Assert.Equal(expected, (bool)result);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_RemoveFromSelection()
    {
        using ListBox listBox = InitializeListBoxWithItems();
        listBox.SelectionMode = SelectionMode.MultiSimple;
        listBox.CreateControl();
        listBox.SetSelected(0, value: true);
        listBox.SetSelected(2, value: true);

        var listBoxAccessibleObject = listBox.AccessibilityObject;
        ISelectionItemProvider.Interface provider = listBoxAccessibleObject.GetChild(0);
        provider.RemoveFromSelection();

        var indices = listBox.SelectedIndices;
        Assert.Equal(1, indices.Count);
        Assert.True(indices.Contains(2));
    }

    private ListBox InitializeListBoxWithItems()
    {
        ListBox listBox = new();
        listBox.Items.AddRange((object[])
        [
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g"
        ]);

        return listBox;
    }

    private ListBoxAccessibleObject InitListBoxItemsAccessibleObjects(ListBox listBox)
    {
        ListBoxAccessibleObject accessibilityObject = (ListBoxAccessibleObject)listBox.AccessibilityObject;
        int childCount = accessibilityObject.GetChildCount();
        // Force items accessibility objects creation
        for (int i = 0; i < childCount; i++)
        {
            accessibilityObject.GetChild(i);
        }

        Assert.Equal(childCount, accessibilityObject.TestAccessor().Dynamic._itemAccessibleObjects.Count);

        return accessibilityObject;
    }

    [WinFormsTheory]
    [InlineData(false, AccessibleStates.Focusable)]
    [InlineData(true, AccessibleStates.Focused | AccessibleStates.Focusable)]
    public void ListBoxAccessibleObject_State_ShouldBeExpected(bool focusControl, AccessibleStates expectedState)
    {
        using Form form = new();
        using ListBox listBox = new();
        form.Controls.Add(listBox);

        using TextBox textBox = new();
        form.Controls.Add(textBox);

        form.Show();
        listBox.CreateControl();

        textBox.Focus();
        listBox.Focused.Should().BeFalse();

        if (focusControl)
        {
            listBox.Focus();
        }

        var state = listBox.AccessibilityObject.State;

        state.Should().Be(expectedState);
    }

    [WinFormsFact]
    public void ListBoxAccessibleObject_ShouldCorrectlyHandleChildrenAndSelection()
    {
        using ListBox listBox = new();
        listBox.Items.AddRange(new object[] { "Item 1", "Item 2" });
        var accessibleObject = listBox.AccessibilityObject;

        accessibleObject.GetSelected().Should().BeNull();

        listBox.SelectedIndex = 0;
        accessibleObject.GetSelected().Should().Be(accessibleObject.GetChild(0));

        listBox.SelectedIndex = 1;
        accessibleObject.GetSelected().Should().Be(accessibleObject.GetChild(1));

        listBox.ClearSelected();
        accessibleObject.GetSelected().Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(0, 0)]
    [InlineData(2, 2)]
    public void GetChildCount_ReturnsExpected(int numberOfItemsToAdd, int expectedCount)
    {
        using ListBox listBox = new();
        for (int i = 0; i < numberOfItemsToAdd; i++)
        {
            listBox.Items.Add($"Item {i + 1}");
        }

        var accessibleObject = new ListBox.ListBoxAccessibleObject(listBox);

        int childCount = accessibleObject.GetChildCount();

        childCount.Should().Be(expectedCount);
    }

    [WinFormsFact]
    public void TestGetFocused_ReturnsExpected()
    {
        using ListBox listBox = new();
        listBox.Items.AddRange(new object[] { "Item 1", "Item 2" });
        listBox.CreateControl();
        listBox.SelectedIndex = 1; // Focus the second item
        listBox.Focus();

        var accessibleObject = listBox.AccessibilityObject;
        var focusedObject = accessibleObject.GetFocused();

        focusedObject.Should().BeEquivalentTo(accessibleObject.GetChild(1));
    }

    [WinFormsTheory]
    [InlineData(-1, null)]
    [InlineData(1, 1)]
    [InlineData(-1, null, true)]
    [InlineData(0, 0, false, true)]
    public void TestGetSelected_VariousScenarios(int selectedIndex, int? expectedIndex, bool clearSelection = false, bool multipleSelection = false)
    {
        using ListBox listBox = new() { SelectionMode = multipleSelection ? SelectionMode.MultiExtended : SelectionMode.One };
        listBox.Items.AddRange(new object[] { "Item 1", "Item 2", "Item 3" });
        if (selectedIndex >= 0)
        {
            listBox.SelectedIndices.Add(selectedIndex);
            if (multipleSelection)
            {
                listBox.SelectedIndices.Add(2); // Select an additional item for multiple selection scenarios
            }
        }

        if (clearSelection)
        {
            listBox.ClearSelected();
        }

        var accessibleObject = new ListBox.ListBoxAccessibleObject(listBox);
        var selectedObject = accessibleObject.GetSelected();

        if (expectedIndex.HasValue)
        {
            selectedObject.Should().BeEquivalentTo(accessibleObject.GetChild(expectedIndex.Value));
        }
        else
        {
            selectedObject.Should().BeNull();
        }
    }

    [WinFormsTheory]
    [InlineData(false, 10, 10, "null")] // ListBox not created
    [InlineData(true, 0, 0, "self", true)] // Point inside ListBox bounds but outside items
    [InlineData(true, 0, 0, "null", false)] // Point outside ListBox bounds
    [InlineData(true, 0, 0, "child", true, true)] // Point inside child bounds
    public void TestHitTest_VariousScenarios(bool listBoxCreated, int xOffset, int yOffset, string expectedResult, bool insideBounds = true, bool insideChild = false)
    {
        using Form form = new();
        using ListBox listBox = new() { Parent = form, Items = { "Item 1", "Item 2" } };
        if (listBoxCreated)
        {
            listBox.CreateControl();
            form.Show();
        }

        var accessibleObject = listBox.AccessibilityObject;
        Point testPoint;
        if (listBoxCreated)
        {
            if (!insideBounds)
            {
                testPoint = new Point(listBox.Bounds.Right + 10, listBox.Bounds.Bottom + 10);
            }
            else if (insideChild)
            {
                listBox.SelectedIndex = 0;
                var itemBounds = listBox.GetItemRectangle(0);
                testPoint = listBox.PointToScreen(new Point(itemBounds.Left + 1, itemBounds.Top + 1));
            }
            else
            {
                testPoint = listBox.PointToScreen(new Point(1, listBox.ClientRectangle.Height - 1));
            }
        }
        else
        {
            testPoint = new Point(xOffset, yOffset);
        }

        var result = accessibleObject.HitTest(testPoint.X, testPoint.Y);

        switch (expectedResult)
        {
            case "self":
                result.Should().Be(accessibleObject);
                break;
            case "null":
                result.Should().BeNull();
                break;
            case "child":
                result.Should().Be(accessibleObject.GetChild(0));
                break;
            default:
                throw new ArgumentException($"Unexpected test result: {expectedResult}");
        }
    }
}
