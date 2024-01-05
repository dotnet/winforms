// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripStatusLabel;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripStatusLabel_ToolStripStatusLabelAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripStatusLabelAccessibleObject_Ctor_Default()
    {
        using ToolStripStatusLabel toolStripStatusLabel = new();
        ToolStripStatusLabelAccessibleObject accessibleObject = new(toolStripStatusLabel);

        Assert.Equal(toolStripStatusLabel, accessibleObject.Owner);
    }

    [WinFormsTheory]
    [InlineData(true, (int)UIA_CONTROLTYPE_ID.UIA_HyperlinkControlTypeId)]
    [InlineData(false, (int)UIA_CONTROLTYPE_ID.UIA_TextControlTypeId)]
    public void ToolStripStatusLabelAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool isLink, int expectedType)
    {
        using ToolStripStatusLabel toolStripStatusLabel = new();
        toolStripStatusLabel.IsLink = isLink;
        // AccessibleRole is not set = Default

        int actual = (int)toolStripStatusLabel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedType, actual);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Link)]
    [InlineData(false, AccessibleRole.StaticText)]
    public void ToolStripStatusLabelAccessibleObject_Role_IsExpected_ByDefault(bool isLink, AccessibleRole expectedRole)
    {
        using ToolStripStatusLabel toolStripStatusLabel = new();
        toolStripStatusLabel.IsLink = isLink;
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripStatusLabel.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
    }

    public static IEnumerable<object[]> ToolStripStatusLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripStatusLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripStatusLabelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripStatusLabel toolStripStatusLabel = new();
        toolStripStatusLabel.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripStatusLabel.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }
}
