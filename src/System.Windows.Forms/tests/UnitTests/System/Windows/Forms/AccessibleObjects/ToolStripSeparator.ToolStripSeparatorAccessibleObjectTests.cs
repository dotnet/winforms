// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripSeparator;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripSeparator_ToolStripSeparatorAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripSeparatorAccessibleObject_Ctor_Default()
    {
        using ToolStripSeparator toolStripSeparator = new();
        ToolStripSeparatorAccessibleObject accessibleObject = new(toolStripSeparator);

        Assert.Equal(toolStripSeparator, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripSeparatorAccessibleObject_ControlType_IsSeparator_IfAccessibleRoleIsDefault()
    {
        using ToolStripSeparator toolStripSeparator = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripSeparator.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_SeparatorControlTypeId, actual);
    }

    [WinFormsFact]
    public void ToolStripSeparatorAccessibleObject_Role_IsSeparator_ByDefault()
    {
        using ToolStripSeparator toolStripSeparator = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripSeparator.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Separator, actual);
    }

    public static IEnumerable<object[]> ToolStripSeparatorAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripSeparatorAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripSeparatorAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripSeparator toolStripSeparator = new();
        toolStripSeparator.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripSeparator.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }
}
