// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripSplitButton;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripSplitButton_ToolStripSplitButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripSplitButtonAccessibleObject_Ctor_Default()
    {
        using ToolStripSplitButton toolStripSplitButton = new();
        ToolStripSplitButtonAccessibleObject accessibleObject = new(toolStripSplitButton);

        Assert.Equal(toolStripSplitButton, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripSplitButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
    {
        using ToolStripSplitButton toolStripSplitButton = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripSplitButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, actual);
    }

    [WinFormsFact]
    public void ToolStripSplitButtonAccessibleObject_Role_IsMenuItem_ByDefault()
    {
        using ToolStripSplitButton toolStripSplitButton = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripSplitButton.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.MenuItem, actual);
    }

    public static IEnumerable<object[]> ToolStripSplitButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripSplitButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripSplitButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripSplitButton toolStripSplitButton = new();
        toolStripSplitButton.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripSplitButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }
}
