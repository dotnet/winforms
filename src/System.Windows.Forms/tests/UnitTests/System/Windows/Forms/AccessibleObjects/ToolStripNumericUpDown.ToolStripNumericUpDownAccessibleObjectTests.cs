// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripControlHost;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripNumericUpDown_ToolStripNumericUpDownAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripNumericUpDownAccessibleObject_Ctor_Default()
    {
        using ToolStripNumericUpDown toolStripNumericUpDown = new();
        ToolStripHostedControlAccessibleObject accessibleObject = (ToolStripHostedControlAccessibleObject)toolStripNumericUpDown.Control.AccessibilityObject;

        ToolStripNumericUpDown actual = accessibleObject.TestAccessor().Dynamic._toolStripControlHost;

        Assert.Equal(toolStripNumericUpDown, actual);
    }

    [WinFormsFact]
    public void ToolStripNumericUpDownAccessibleObject_ControlType_IsSpinner_IfAccessibleRoleIsDefault()
    {
        using ToolStripNumericUpDown toolStripNumericUpDown = new();
        // AccessibleRole is not set = Default

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_SpinnerControlTypeId, (UIA_CONTROLTYPE_ID)(int)toolStripNumericUpDown.Control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
        Assert.Equal(VARIANT.Empty, toolStripNumericUpDown.Control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Client)]
    [InlineData(false, AccessibleRole.None)]
    public void ToolStripNumericUpDownAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using ToolStripNumericUpDown toolStripNumericUpDown = new();
        // AccessibleRole is not set = Default
        Control control = toolStripNumericUpDown.Control;

        if (createControl)
        {
            control.CreateControl();
        }

        AccessibleRole actual = toolStripNumericUpDown.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.Equal(createControl, control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripNumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripNumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripNumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripNumericUpDown toolStripNumericUpDown = new();
        toolStripNumericUpDown.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripNumericUpDown.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }
}
