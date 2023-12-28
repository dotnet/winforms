// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class CheckedListBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void CheckedListBoxAccessibleObject_CheckBounds()
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.Size = new Size(120, 100);
        checkedListBox.Items.Add("a");
        checkedListBox.Items.Add("b");
        checkedListBox.Items.Add("c");
        checkedListBox.Items.Add("d");
        checkedListBox.Items.Add("e");
        checkedListBox.Items.Add("f");
        checkedListBox.Items.Add("g");
        checkedListBox.Items.Add("h");
        checkedListBox.Items.Add("i");

        int listBoxHeight = checkedListBox.AccessibilityObject.Bounds.Height;
        int sumItemsHeight = 0;

        for (int i = 0; i < checkedListBox.Items.Count; i++)
        {
            AccessibleObject item = checkedListBox.AccessibilityObject.GetChild(i);
            sumItemsHeight += item.Bounds.Height;
        }

        Assert.Equal(listBoxHeight, sumItemsHeight);
    }

    [WinFormsTheory]
    [InlineData(true, (int)UIA_CONTROLTYPE_ID.UIA_ListControlTypeId)]
    [InlineData(false, (int)UIA_CONTROLTYPE_ID.UIA_ListControlTypeId)]
    public void CheckedListBoxAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, int expectedType)
    {
        using CheckedListBox checkedListBox = new();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            checkedListBox.CreateControl();
        }

        VARIANT actual = checkedListBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedType, (int)actual);
        Assert.Equal(createControl, checkedListBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> CheckedListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(CheckedListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void CheckedListBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using CheckedListBox checkedListBox = new();
        checkedListBox.AccessibleRole = role;

        VARIANT actual = checkedListBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.List)]
    [InlineData(false, AccessibleRole.None)]
    public void CheckedListBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using CheckedListBox checkedListBox = new();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            checkedListBox.CreateControl();
        }

        AccessibleRole actual = checkedListBox.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.Equal(createControl, checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(3)]
    public void CheckedListBoxAccessibleObject_GetChildCount_ReturnsExpected(int childCount)
    {
        using CheckedListBox checkedListBox = new();

        for (int i = 0; i < childCount; i++)
        {
            checkedListBox.Items.Add(i);
        }

        int actual = checkedListBox.AccessibilityObject.GetChildCount();

        Assert.Equal(childCount, actual);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxAccessibleObject_RuntimeId_NotNull()
    {
        using CheckedListBox checkedListBox = new();

        Assert.NotNull(checkedListBox.AccessibilityObject.RuntimeId);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxAccessibleObject_FragmentNavigate_NavigateToFirstChild_IsExpected()
    {
        using CheckedListBox checkedListBox = new();
        AccessibleObject accessibleObject = checkedListBox.AccessibilityObject;

        checkedListBox.Items.Add(0);
        checkedListBox.Items.Add(1);
        checkedListBox.Items.Add(2);

        AccessibleObject expected = accessibleObject.GetChild(0);

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckedListBoxAccessibleObject_FragmentNavigate_NavigateToLastChild_IsExpected()
    {
        using CheckedListBox checkedListBox = new();
        AccessibleObject accessibleObject = checkedListBox.AccessibilityObject;

        checkedListBox.Items.Add(0);
        checkedListBox.Items.Add(1);
        checkedListBox.Items.Add(2);

        AccessibleObject expected = accessibleObject.GetChild(2);

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(checkedListBox.IsHandleCreated);
    }
}
