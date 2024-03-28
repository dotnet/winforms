// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using ButtonAccessibleObject = System.Windows.Forms.Button.ButtonAccessibleObject;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class Button_ButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void ButtonAccessibleObject_Ctor_NullControl_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ButtonAccessibleObject(null));
    }

    [WinFormsFact]
    public void ButtonAccessibleObject_Ctor_InitializesOwner()
    {
        using Button button = new();
        Assert.False(button.IsHandleCreated);
        ButtonAccessibleObject buttonAccessibleObject = new(button);

        Assert.Same(button, buttonAccessibleObject.Owner);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.PushButton)]
    [InlineData(false, AccessibleRole.None)]
    public void ButtonAccessibleObject_AccessibleRole_Default_ReturnsExpected(bool createControl, AccessibleRole accessibleRole)
    {
        using Button button = new()
        {
            AccessibleRole = AccessibleRole.Default
        };

        if (createControl)
        {
            button.CreateControl();
        }

        ButtonAccessibleObject buttonAccessibleObject = new(button);

        Assert.Equal(accessibleRole, buttonAccessibleObject.Role);
        Assert.Equal(createControl, button.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonAccessibleObject_AccessibleRole_Custom_ReturnsExpected()
    {
        using Button button = new()
        {
            AccessibleRole = AccessibleRole.Link
        };

        Assert.False(button.IsHandleCreated);
        ButtonAccessibleObject buttonAccessibleObject = new(button);

        Assert.Equal(AccessibleRole.Link, buttonAccessibleObject.Role);
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId, (int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId)] // If AccessibleRole is Default
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "Button1")]
    public void ButtonAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using Button button = new()
        {
            Name = "Button1",
            AccessibleName = "TestName"
        };

        Assert.False(button.IsHandleCreated);
        ButtonAccessibleObject buttonAccessibleObject = new(button);
        using VARIANT value = buttonAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);

        Assert.Equal(expected, value.ToObject());
        Assert.False(button.IsHandleCreated);
    }

    [WinFormsFact]
    public void ButtonAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePatternId()
    {
        using Button button = new();

        Assert.False(button.IsHandleCreated);
        ButtonAccessibleObject buttonAccessibleObject = new(button);

        Assert.True(buttonAccessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
        Assert.False(button.IsHandleCreated);
    }

    public static IEnumerable<object[]> ButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using Button button = new();
        button.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)button.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(button.IsHandleCreated);
    }
}
