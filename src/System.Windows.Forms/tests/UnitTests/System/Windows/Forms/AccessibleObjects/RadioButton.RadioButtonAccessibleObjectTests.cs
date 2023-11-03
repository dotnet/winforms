// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using RadioButtonAccessibleObject = System.Windows.Forms.RadioButton.RadioButtonAccessibleObject;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class RadioButton_RadioButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void RadioButtonAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RadioButtonAccessibleObject(null));
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Ctor_Default()
    {
        using var radioButton = new RadioButton();
        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.NotNull(radioButtonAccessibleObject.Owner);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_DefaultAction_ReturnsExpected()
    {
        using var radioButton = new RadioButton
        {
            AccessibleDefaultActionDescription = "TestActionDescription"
        };

        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.Equal("TestActionDescription", radioButtonAccessibleObject.DefaultAction);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_GetProperyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected()
    {
        using var radioButton = new RadioButton
        {
            AccessibleDefaultActionDescription = "TestActionDescription"
        };

        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.Equal("TestActionDescription", ((BSTR)radioButtonAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Description_ReturnsExpected()
    {
        using var radioButton = new RadioButton
        {
            AccessibleDescription = "TestDescription"
        };

        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.Equal("TestDescription", radioButtonAccessibleObject.Description);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Name_ReturnsExpected()
    {
        using var radioButton = new RadioButton
        {
            AccessibleName = "TestName"
        };

        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.Equal("TestName", radioButtonAccessibleObject.Name);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Role_Custom_ReturnsExpected()
    {
        using var radioButton = new RadioButton
        {
            AccessibleRole = AccessibleRole.PushButton
        };

        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.Equal(AccessibleRole.PushButton, radioButtonAccessibleObject.Role);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Role_Default_ReturnsExpected()
    {
        using var radioButton = new RadioButton();
        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.Equal(AccessibleRole.RadioButton, radioButtonAccessibleObject.Role);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleStates.Focusable, AccessibleStates.Focusable | AccessibleStates.Checked)]
    [InlineData(false, AccessibleStates.None, AccessibleStates.None)]
    public void RadioButtonAccessibleObject_State_ReturnsExpected(bool createControl, AccessibleStates accessibleStatesFirstStage, AccessibleStates accessibleStatesSecondStage)
    {
        using var radioButton = new RadioButton();

        if (createControl)
        {
            radioButton.CreateControl();
        }

        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);
        Assert.Equal(accessibleStatesFirstStage, radioButtonAccessibleObject.State);

        radioButtonAccessibleObject.DoDefaultAction();

        Assert.Equal(accessibleStatesSecondStage, radioButtonAccessibleObject.State);
        Assert.Equal(createControl, radioButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void RadioButtonAccessibleObject_IsItemSelected_ReturnsExpected(bool createControl)
    {
        using var radioButton = new RadioButton();

        if (createControl)
        {
            radioButton.CreateControl();
        }

        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);
        Assert.False(radioButtonAccessibleObject.IsItemSelected);

        radioButtonAccessibleObject.DoDefaultAction();

        Assert.Equal(createControl, radioButtonAccessibleObject.IsItemSelected);
        Assert.Equal(createControl, radioButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId, (int)UIA_CONTROLTYPE_ID.UIA_RadioButtonControlTypeId)] // If AccessibleRole is Default
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "RadioButton1")]
    public void RadioButtonAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using var radioButton = new RadioButton
        {
            Name = "RadioButton1",
            AccessibleName = "TestName"
        };

        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);
        using VARIANT value = radioButtonAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);

        Assert.Equal(expected, value.ToObject());
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_SelectionItemPatternId)]
    public void RadioButtonAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected(int patternId)
    {
        using var radioButton = new RadioButton
        {
            Name = "RadioButton1"
        };

        Assert.False(radioButton.IsHandleCreated);
        var radioButtonAccessibleObject = new RadioButtonAccessibleObject(radioButton);

        Assert.True(radioButtonAccessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(radioButton.IsHandleCreated);
    }

    public static IEnumerable<object[]> RadioButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(RadioButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void RadioButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using RadioButton radioButton = new RadioButton();
        radioButton.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)radioButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(radioButton.IsHandleCreated);
    }
}
