// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripLabel;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripLabel_ToolStripLabelAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripLabelAccessibleObject_Ctor_Default()
    {
        using ToolStripLabel toolStripLabel = new();
        ToolStripLabelAccessibleObject accessibleObject = new(toolStripLabel);

        Assert.Equal(toolStripLabel, accessibleObject.Owner);
    }

    [WinFormsTheory]
    [InlineData(true, (int)UIA_CONTROLTYPE_ID.UIA_HyperlinkControlTypeId)]
    [InlineData(false, (int)UIA_CONTROLTYPE_ID.UIA_TextControlTypeId)]
    public void ToolStripLabelAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool isLink, int expectedType)
    {
        using ToolStripLabel toolStripLabel = new();
        toolStripLabel.IsLink = isLink;
        // AccessibleRole is not set = Default

        int actual = (int)toolStripLabel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedType, actual);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Link)]
    [InlineData(false, AccessibleRole.StaticText)]
    public void ToolStripLabelAccessibleObject_Role_IsExpected_ByDefault(bool isLink, AccessibleRole expectedRole)
    {
        using ToolStripLabel toolStripLabel = new();
        toolStripLabel.IsLink = isLink;
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripLabel.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
    }

    public static IEnumerable<object[]> ToolStripLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripLabel toolStripLabel = new();
        toolStripLabel.AccessibleRole = role;

        Assert.Equal(AccessibleRoleControlTypeMap.GetControlType(role), (UIA_CONTROLTYPE_ID)(int)toolStripLabel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
        Assert.Equal(VARIANT.Empty, toolStripLabel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleStates.ReadOnly | AccessibleStates.Focusable)]
    [InlineData(false, AccessibleStates.ReadOnly)]
    public void ToolStripLabelAccessibleObject_GetPropertyValue_LegacyIAccessibleStatePropertyId_ReturnsExpected(bool isLink, AccessibleStates expectedState)
    {
        using ToolStripLabel toolStripLabel = new() { IsLink = isLink };
        var actual = (AccessibleStates)(int)toolStripLabel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleStatePropertyId);

        Assert.Equal(expectedState, actual);
    }
}
