// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using CheckBoxAccessibleObject = System.Windows.Forms.CheckBox.CheckBoxAccessibleObject;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class CheckBox_CheckBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void CheckBoxAccessibleObject_Ctor_NullControl_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CheckBoxAccessibleObject(null));
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_Ctor_InvalidTypeControl_ThrowsArgumentException()
    {
        using TextBox textBox = new();
        Assert.Throws<ArgumentException>(() => new CheckBoxAccessibleObject(textBox));
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_Ctor_Default()
    {
        using CheckBox checkBox = new();
        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);

        Assert.NotNull(checkBoxAccessibleObject.Owner);
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_DefaultAction_ReturnsExpected()
    {
        using CheckBox checkBox = new()
        {
            Name = "CheckBox1",
            AccessibleDefaultActionDescription = "TestActionDescription"
        };

        checkBox.IsHandleCreated.Should().BeFalse();
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);

        checkBox.AccessibilityObject.DefaultAction.Should().Be("TestActionDescription");
        checkBox.IsHandleCreated.Should().BeFalse();
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_DefaultAction_CheckedIsTrue_ReturnsExpected()
    {
        using CheckBox checkBox = new();

        checkBox.IsHandleCreated.Should().BeFalse();
        checkBox.AccessibilityObject.DefaultAction.Should().Be("Check");

        checkBox.Checked = true;

        checkBox.AccessibilityObject.DefaultAction.Should().Be("Uncheck");
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_DefaultRole_ReturnsExpected()
    {
        using CheckBox checkBox = new();
        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);

        Assert.Equal(AccessibleRole.CheckButton, checkBoxAccessibleObject.Role);
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_CustomRole_ReturnsExpected()
    {
        using CheckBox checkBox = new()
        {
            AccessibleRole = AccessibleRole.PushButton
        };

        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);

        Assert.Equal(AccessibleRole.PushButton, checkBoxAccessibleObject.Role);
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleStates.Focusable, AccessibleStates.Focusable | AccessibleStates.Checked)]
    [InlineData(false, AccessibleStates.None, AccessibleStates.None)]
    public void CheckBoxAccessibleObject_State_ReturnsExpected(bool createControl, AccessibleStates accessibleStatesFirstStage, AccessibleStates accessibleStatesSecondStage)
    {
        using CheckBox checkBox = new();

        if (createControl)
        {
            checkBox.CreateControl();
        }

        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);
        checkBoxAccessibleObject.State.Should().Be(accessibleStatesFirstStage);

        checkBoxAccessibleObject.DoDefaultAction();

        checkBoxAccessibleObject.State.Should().Be(accessibleStatesSecondStage);
        checkBox.IsHandleCreated.Should().Be(createControl);
    }

    [WinFormsTheory]
    [InlineData(true, CheckState.Checked, (int)ToggleState.ToggleState_On)]
    [InlineData(true, CheckState.Unchecked, (int)ToggleState.ToggleState_Off)]
    [InlineData(true, CheckState.Indeterminate, (int)ToggleState.ToggleState_Indeterminate)]
    [InlineData(false, CheckState.Checked, (int)ToggleState.ToggleState_On)]
    [InlineData(false, CheckState.Unchecked, (int)ToggleState.ToggleState_Off)]
    [InlineData(false, CheckState.Indeterminate, (int)ToggleState.ToggleState_Indeterminate)]
    public void CheckBoxAccessibleObject_ToggleState_ReturnsExpected(bool createControl, CheckState checkState, int toggleState)
    {
        using CheckBox checkBox = new() { CheckState = checkState };

        if (createControl)
        {
            checkBox.CreateControl();
        }

        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);
        Assert.Equal((ToggleState)toggleState, checkBoxAccessibleObject.ToggleState);

        Assert.Equal(createControl, checkBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> CheckBoxAccessibleObject_DoDefaultAction_Success_Data()
    {
        yield return new object[] { true, false, CheckState.Checked, (int)ToggleState.ToggleState_Off };
        yield return new object[] { true, false, CheckState.Indeterminate, (int)ToggleState.ToggleState_Off };
        yield return new object[] { true, false, CheckState.Unchecked, (int)ToggleState.ToggleState_On };

        yield return new object[] { true, true, CheckState.Checked, (int)ToggleState.ToggleState_Indeterminate };
        yield return new object[] { true, true, CheckState.Indeterminate, (int)ToggleState.ToggleState_Off };
        yield return new object[] { true, true, CheckState.Unchecked, (int)ToggleState.ToggleState_On };

        yield return new object[] { false, true, CheckState.Checked, (int)ToggleState.ToggleState_On };
        yield return new object[] { false, true, CheckState.Indeterminate, (int)ToggleState.ToggleState_Indeterminate };
        yield return new object[] { false, true, CheckState.Unchecked, (int)ToggleState.ToggleState_Off };

        yield return new object[] { false, false, CheckState.Checked, (int)ToggleState.ToggleState_On };
        yield return new object[] { false, false, CheckState.Indeterminate, (int)ToggleState.ToggleState_Indeterminate };
        yield return new object[] { false, false, CheckState.Unchecked, (int)ToggleState.ToggleState_Off };
    }

    [WinFormsTheory]
    [MemberData(nameof(CheckBoxAccessibleObject_DoDefaultAction_Success_Data))]
    public void CheckBoxAccessibleObject_DoDefaultAction_Success(bool createControl, bool threeState, CheckState checkState, int toggleState)
    {
        using CheckBox checkBox = new()
        {
            ThreeState = threeState,
            CheckState = checkState
        };

        if (createControl)
        {
            checkBox.CreateControl();
        }

        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);
        checkBoxAccessibleObject.DoDefaultAction();

        Assert.Equal((ToggleState)toggleState, checkBoxAccessibleObject.ToggleState);
        Assert.Equal(createControl, checkBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_Description_ReturnsExpected()
    {
        using CheckBox checkBox = new()
        {
            AccessibleDescription = "TestDescription"
        };

        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);

        Assert.Equal("TestDescription", checkBoxAccessibleObject.Description);
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_Name_ReturnsExpected()
    {
        using CheckBox checkBox = new()
        {
            AccessibleName = "TestName"
        };

        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);

        Assert.Equal("TestName", checkBoxAccessibleObject.Name);
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_NamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId, "TestName")]
    [InlineData((int)UIA_PROPERTY_ID.UIA_ControlTypePropertyId, (int)UIA_CONTROLTYPE_ID.UIA_CheckBoxControlTypeId)] // If AccessibleRole is Default
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId, true)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_AutomationIdPropertyId, "CheckBox1")]
    public void CheckBoxAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
    {
        using CheckBox checkBox = new()
        {
            Name = "CheckBox1",
            AccessibleName = "TestName"
        };

        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);
        using VARIANT value = checkBoxAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);

        Assert.Equal(expected, value.ToObject());
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TogglePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    public void CheckBoxAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected(int patternId)
    {
        using CheckBox checkBox = new();
        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);

        Assert.True(checkBoxAccessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_Toggle_Invoke_Success()
    {
        using CheckBox checkBox = new();
        Assert.False(checkBox.IsHandleCreated);
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);
        Assert.False(checkBox.Checked);

        checkBoxAccessibleObject.Toggle();
        Assert.True(checkBox.Checked);

        // toggle again
        checkBoxAccessibleObject.Toggle();

        Assert.False(checkBox.Checked);
        Assert.False(checkBox.IsHandleCreated);
    }

    [WinFormsFact]
    public void CheckBoxAccessibleObject_Toggle_Invoke_ThreeState_Success()
    {
        using CheckBox checkBox = new() { ThreeState = true };
        CheckBoxAccessibleObject checkBoxAccessibleObject = new(checkBox);
        Assert.Equal(CheckState.Unchecked, checkBox.CheckState);

        checkBoxAccessibleObject.Toggle();
        Assert.Equal(CheckState.Checked, checkBox.CheckState);

        // toggle again
        checkBoxAccessibleObject.Toggle();
        Assert.Equal(CheckState.Indeterminate, checkBox.CheckState);

        // toggle again
        checkBoxAccessibleObject.Toggle();
        Assert.Equal(CheckState.Unchecked, checkBox.CheckState);

        Assert.False(checkBox.IsHandleCreated);
    }

    public static IEnumerable<object[]> CheckBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(CheckBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void CheckBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using CheckBox checkBox = new();
        checkBox.AccessibleRole = role;
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, (UIA_CONTROLTYPE_ID)(int)checkBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
        Assert.Equal(checkBox.AccessibilityObject.DefaultAction, ((BSTR)checkBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.False(checkBox.IsHandleCreated);
    }
}
