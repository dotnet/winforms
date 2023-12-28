// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests;

public class TableLayoutPanel_TableLayoutPanelAccessibilityObjectTests
{
    [WinFormsFact]
    public void TableLayoutPanelAccessibilityObject_Ctor_Default()
    {
        using TableLayoutPanel tableLayoutPanel = new();
        tableLayoutPanel.CreateControl();

        Assert.NotNull(tableLayoutPanel.AccessibilityObject);
        Assert.True(tableLayoutPanel.IsHandleCreated);
    }

    [WinFormsFact]
    public void TableLayoutPanelAccessibilityObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
    {
        using TableLayoutPanel tableLayoutPanel = new();
        tableLayoutPanel.CreateControl();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)tableLayoutPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId, actual);
        Assert.True(tableLayoutPanel.IsHandleCreated);
    }

    [WinFormsFact]
    public void TableLayoutPanelAccessibilityObject_Role_IsClient_ByDefault()
    {
        using TableLayoutPanel tableLayoutPanel = new();
        tableLayoutPanel.CreateControl();
        // AccessibleRole is not set = Default

        AccessibleRole actual = tableLayoutPanel.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Client, actual);
        Assert.True(tableLayoutPanel.IsHandleCreated);
    }

    public static IEnumerable<object[]> TableLayoutPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(TableLayoutPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void TableLayoutPanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using TableLayoutPanel tableLayoutPanel = new();
        tableLayoutPanel.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)tableLayoutPanel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(tableLayoutPanel.IsHandleCreated);
    }
}
