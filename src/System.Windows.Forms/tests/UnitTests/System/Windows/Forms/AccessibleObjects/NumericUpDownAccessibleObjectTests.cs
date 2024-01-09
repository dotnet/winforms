// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class NumericUpDownAccessibleObjectTests
{
    [WinFormsFact]
    public void NumericUpDownAccessibleObject_Ctor_Default()
    {
        using NumericUpDown numericUpDown = new();
        AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;
        Assert.NotNull(accessibleObject);
    }

    [WinFormsFact]
    public void NumericUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_ReturnsTrue()
    {
        using NumericUpDown numericUpDown = new();
        AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;

        bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId);
        Assert.True(isKeyboardFocusable);
    }

    [WinFormsFact]
    public void NumericUpDownAccessibleObject_GetPropertyValue_IsKeyboardFocusable_WhenDisabled_ReturnsFalse()
    {
        using NumericUpDown numericUpDown = new();
        AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;

        numericUpDown.Enabled = false;

        bool isKeyboardFocusable = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId);
        Assert.False(isKeyboardFocusable);
    }

    [WinFormsFact]
    public void NumericUpDownAccessibleObject_ControlType_IsSpinner_IfAccessibleRoleIsDefault()
    {
        using NumericUpDown numericUpDown = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)numericUpDown.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_SpinnerControlTypeId, actual);
        Assert.False(numericUpDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void NumericUpDownAccessibleObject_Role_IsSpinButton_ByDefault()
    {
        using NumericUpDown numericUpDown = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = numericUpDown.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.SpinButton, actual);
        Assert.False(numericUpDown.IsHandleCreated);
    }

    public static IEnumerable<object[]> NumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(NumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void NumericUpDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using NumericUpDown numericUpDown = new();
        numericUpDown.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)numericUpDown.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(numericUpDown.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId, (int)AccessibleRole.SpinButton)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleStatePropertyId, (int)AccessibleStates.None)]
    public void NumericUpDownAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
    {
        using NumericUpDown numericUpDown = new();
        AccessibleObject accessibleObject = numericUpDown.AccessibilityObject;
        int actual = (int)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)property);

        Assert.Equal(expected, actual);
        Assert.False(numericUpDown.IsHandleCreated);
    }
}
