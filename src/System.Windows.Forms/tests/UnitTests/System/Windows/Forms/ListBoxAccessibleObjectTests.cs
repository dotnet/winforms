// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListBox;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void ListBoxAccessibleObjectTests_Ctor_Default()
    {
        using ListBox listBox = CreateListBoxWithItems();

        var childCount = listBox.AccessibilityObject.GetChildCount();

        for (int i = 0; i < childCount; i++)
        {
            var child = listBox.AccessibilityObject.GetChild(i);
            Assert.True(child.IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollItemPatternId));
        }
    }

    [WinFormsFact]
    public void ListBoxAccessibleObject_ControlType_IsList_IfAccessibleRoleIsDefault()
    {
        using ListBox listBox = new ListBox();
        // AccessibleRole is not set = Default

        object actual = listBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ListControlTypeId, actual);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.List)]
    [InlineData(false, AccessibleRole.None)]
    public void ListBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using ListBox listBox = new ListBox();
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
        using ListBox listBox = new ListBox();
        listBox.AccessibleRole = role;

        object actual = listBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBoxItemAccessibleObject_GetPropertyValue_ValueValuePropertyId_ReturnsExpected()
    {
        using ListBox listBox = new ListBox();
        AccessibleObject accessibleObject = listBox.AccessibilityObject;

        Assert.Null(accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
        Assert.False(listBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListBox_ReleaseUiaProvider_ClearsItemsAccessibleObjects()
    {
        using ListBox listBox = CreateListBoxWithItems();
        ListBoxAccessibleObject accessibleObject = InitListBoxItemsAccessibleObjects(listBox);

        listBox.ReleaseUiaProvider(listBox.HWND);

        Assert.Equal(0, accessibleObject.TestAccessor().Dynamic._itemAccessibleObjects.Count);
    }

    [WinFormsFact]
    public void ListBoxItems_Clear_ClearsItemsAccessibleObjects()
    {
        using ListBox listBox = CreateListBoxWithItems();
        ListBoxAccessibleObject accessibleObject = InitListBoxItemsAccessibleObjects(listBox);

        listBox.Items.Clear();

        Assert.Equal(0, accessibleObject.TestAccessor().Dynamic._itemAccessibleObjects.Count);
    }

    [WinFormsFact]
    public void ListBoxItems_Remove_RemovesItemAccessibleObject()
    {
        using ListBox listBox = CreateListBoxWithItems();
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

        ListBox.ListBoxAccessibleObject accessibleObject = new(listBox);
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

    private ListBox CreateListBoxWithItems()
    {
        ListBox listBox = new();
        listBox.Items.AddRange(new object[]
        {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g"
        });

        return listBox;
    }

    private ListBoxAccessibleObject InitListBoxItemsAccessibleObjects(ListBox listBox)
    {
        ListBoxAccessibleObject accessibilityObject = (ListBoxAccessibleObject)listBox.AccessibilityObject;
        int childCount = accessibilityObject.GetChildCount();
        // Force items accessiblity objects creation
        for (int i = 0; i < childCount; i++)
        {
            accessibilityObject.GetChild(i);
        }

        Assert.Equal(childCount, accessibilityObject.TestAccessor().Dynamic._itemAccessibleObjects.Count);

        return accessibilityObject;
    }
}
