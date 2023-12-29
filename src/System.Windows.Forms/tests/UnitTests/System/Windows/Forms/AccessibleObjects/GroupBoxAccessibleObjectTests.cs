// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class GroupBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void GroupBoxAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
    {
        string testAccName = "Test group name";
        using GroupBox groupBox = new();
        AccessibleObject groupBoxAccessibleObject = groupBox.AccessibilityObject;

        Assert.Equal(VARIANT.Empty, groupBoxAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId));
        Assert.Equal(VARIANT.Empty, groupBoxAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId));

        groupBox.Text = "Some test groupBox text";
        groupBox.Name = "Group1";
        groupBox.AccessibleName = testAccName;

        Assert.Equal(testAccName, ((BSTR)groupBoxAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Equal(testAccName, ((BSTR)groupBoxAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId)).ToStringAndFree());
        Assert.False(groupBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void GroupBoxAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue()
    {
        using GroupBox groupBox = new();
        groupBox.Name = "Group1";
        groupBox.Text = "Some test groupBox text";
        var groupBoxAccessibleObject = new GroupBox.GroupBoxAccessibleObject(groupBox);

        Assert.False(groupBox.IsHandleCreated);

        bool supportsLegacyIAccessiblePatternId = groupBoxAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId);
        Assert.True(supportsLegacyIAccessiblePatternId);
    }

    [WinFormsFact]
    public void GroupBoxAccessibleObject_LegacyIAccessible_Role_ReturnsExpected()
    {
        using GroupBox groupBox = new();
        groupBox.Name = "Group1";
        groupBox.Text = "Some test groupBox text";
        groupBox.AccessibleRole = AccessibleRole.Link;
        var groupBoxAccessibleObject = new GroupBox.GroupBoxAccessibleObject(groupBox);

        Assert.False(groupBox.IsHandleCreated);
        Assert.Equal(AccessibleRole.Link, groupBoxAccessibleObject.Role);
    }

    [WinFormsFact]
    public void GroupBoxAccessibleObject_Role_IsGrouping_ByDefault()
    {
        using GroupBox groupBox = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = groupBox.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Grouping, actual);
        Assert.False(groupBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void GroupBoxAccessibleObject_LegacyIAccessible_Description_ReturnsExpected()
    {
        string testAccDescription = "Test description";
        using GroupBox groupBox = new();
        groupBox.Name = "Group1";
        groupBox.Text = "Some test groupBox text";
        groupBox.AccessibleDescription = testAccDescription;
        var groupBoxAccessibleObject = new GroupBox.GroupBoxAccessibleObject(groupBox);

        Assert.False(groupBox.IsHandleCreated);
        Assert.Equal(testAccDescription, groupBoxAccessibleObject.Description);
    }

    [WinFormsFact]
    public void GroupBoxAccessibleObject_ControlType_IsGroup_IfAccessibleRoleIsDefault()
    {
        using GroupBox groupBox = new();
        // AccessibleRole is not set = Default
        VARIANT actual = groupBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_GroupControlTypeId, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(groupBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> GroupBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(GroupBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void GroupBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using GroupBox groupBox = new();
        groupBox.AccessibleRole = role;

        VARIANT actual = groupBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(groupBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void GroupBoxAccessibleObject_GetPropertyValue_AutomationId_ReturnsExpected()
    {
        using GroupBox ownerControl = new() { Name = "test name" };
        string expected = ownerControl.Name;
        string actual = ((BSTR)ownerControl.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree();

        Assert.Equal(expected, actual);
        Assert.False(ownerControl.IsHandleCreated);
    }
}
