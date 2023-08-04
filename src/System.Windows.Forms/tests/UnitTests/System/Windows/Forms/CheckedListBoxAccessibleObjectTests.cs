﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms.Tests;

public class CheckedListBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void CheckedListBoxAccessibleObject_CheckBounds()
    {
        using CheckedListBox checkedListBox = new CheckedListBox();
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
    [InlineData(true, (int)UiaCore.UIA.ListControlTypeId)]
    [InlineData(false, (int)UiaCore.UIA.ListControlTypeId)]
    public void CheckedListBoxAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, int expectedType)
    {
        using CheckedListBox checkedListBox = new CheckedListBox();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            checkedListBox.CreateControl();
        }

        object actual = checkedListBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

        Assert.Equal((UiaCore.UIA)expectedType, actual);
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
        using CheckedListBox checkedListBox = new CheckedListBox();
        checkedListBox.AccessibleRole = role;

        object actual = checkedListBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
        UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(checkedListBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.List)]
    [InlineData(false, AccessibleRole.None)]
    public void CheckedListBoxAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using CheckedListBox checkedListBox = new CheckedListBox();
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

        Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
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

        Assert.Equal(expected, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
        Assert.False(checkedListBox.IsHandleCreated);
    }
}
