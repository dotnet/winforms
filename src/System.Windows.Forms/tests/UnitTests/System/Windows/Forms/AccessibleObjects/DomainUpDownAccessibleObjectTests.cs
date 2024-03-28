// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DomainUpDownAccessibleObjectTests
{
    [WinFormsFact]
    public void DomainUpDownAccessibleObject_Ctor_Default()
    {
        using DomainUpDown domainUpDown = new();
        AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;
        Assert.NotNull(accessibleObject);
        Assert.False(domainUpDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void DomainUpDownAccessibleObject_ControlType_IsSpinner_IfAccessibleRoleIsDefault()
    {
        using DomainUpDown domainUpDown = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)domainUpDown.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_SpinnerControlTypeId, actual);
        Assert.False(domainUpDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void DomainUpDownAccessibleObject_Role_IsSpinButton_ByDefault()
    {
        using DomainUpDown domainUpDown = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = domainUpDown.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.SpinButton, actual);
        Assert.False(domainUpDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void DomainUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_ReturnsTrue()
    {
        using DomainUpDown domainUpDown = new();
        AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;

        bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId);
        Assert.True(isKeyboardFocusable);
    }

    [WinFormsFact]
    public void DomainUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_WhenDisabled_ReturnsFalse()
    {
        using DomainUpDown domainUpDown = new();
        AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;

        domainUpDown.Enabled = false;

        bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId);
        Assert.False(isKeyboardFocusable);
    }

    public static IEnumerable<object[]> DomainUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(DomainUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void DomainUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using DomainUpDown domainUpDown = new();
        domainUpDown.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)domainUpDown.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(domainUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId, (int)AccessibleRole.SpinButton)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleStatePropertyId, (int)AccessibleStates.None)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ValueValuePropertyId, null)]
    public void DomainUpDownAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
    {
        using DomainUpDown domainUpDown = new();
        AccessibleObject accessibleObject = domainUpDown.AccessibilityObject;
        VARIANT actual = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)property);
        if (expected is null)
        {
            Assert.Equal(VARIANT.Empty, actual);
        }
        else
        {
            Assert.Equal(expected, (int)actual);
        }

        Assert.False(domainUpDown.IsHandleCreated);
    }
}
