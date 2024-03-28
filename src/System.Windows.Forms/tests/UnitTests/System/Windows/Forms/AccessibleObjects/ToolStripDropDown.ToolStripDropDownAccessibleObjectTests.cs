// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripDropDown;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripDropDown_ToolStripDropDownAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_ctor_default()
    {
        using ToolStripDropDown toolStripDropDown = new();
        ToolStripDropDownAccessibleObject accessibleObject = new(toolStripDropDown);

        Assert.Equal(toolStripDropDown, accessibleObject.Owner);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_ControlType_IsMenu_IfAccessibleRoleIsDefault()
    {
        using ToolStripDropDown toolStripDropDown = new();
        // AccessibleRole is not set = Default

        AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_MenuControlTypeId, actual);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_Role_IsMenuPopup_ByDefault()
    {
        using ToolStripDropDown toolStripDropDown = new();
        // AccessibleRole is not set = Default

        AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
        AccessibleRole actual = accessibleObject.Role;

        Assert.Equal(AccessibleRole.MenuPopup, actual);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripDropDown toolStripDropDown = new();
        toolStripDropDown.AccessibleRole = role;

        AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(role, accessibleObject.Role);
        Assert.Equal(expected, actual);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }
}
