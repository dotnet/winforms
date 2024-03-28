// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripGrip;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripGrip_ToolStripGripAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripGripAccessibleObject_Ctor_Default()
    {
        using ToolStripGrip toolStripGrip = new();
        ToolStripGripAccessibleObject accessibleObject = new(toolStripGrip);

        Assert.Equal(toolStripGrip, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripGripAccessibleObject_ControlType_IsThumb_IfAccessibleRoleIsDefault()
    {
        using ToolStripGrip toolStripGrip = new();
        // AccessibleRole is not set = Default

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)toolStripGrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId, actual);
    }

    [WinFormsFact]
    public void ToolStripGripAccessibleObject_Role_IsGrip_ByDefault()
    {
        using ToolStripGrip toolStripGrip = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripGrip.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Grip, actual);
    }

    public static IEnumerable<object[]> ToolStripGripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripGripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripGripAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripGrip toolStripGrip = new();
        toolStripGrip.AccessibleRole = role;

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)toolStripGrip.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }
}
