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
        using RadioButton radioButton = new();
        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        Assert.NotNull(radioButtonAccessibleObject.Owner);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_DefaultAction_ReturnsExpected()
    {
        using RadioButton radioButton = new()
        {
            AccessibleDefaultActionDescription = "TestActionDescription"
        };

        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        Assert.Equal("TestActionDescription", radioButtonAccessibleObject.DefaultAction);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_GetProperyValue_LegacyIAccessibleDefaultActionPropertyId_ReturnsExpected()
    {
        using RadioButton radioButton = new()
        {
            AccessibleDefaultActionDescription = "TestActionDescription"
        };

        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        Assert.Equal("TestActionDescription", ((BSTR)radioButtonAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId)).ToStringAndFree());
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Description_ReturnsExpected()
    {
        using RadioButton radioButton = new()
        {
            AccessibleDescription = "TestDescription"
        };

        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        Assert.Equal("TestDescription", radioButtonAccessibleObject.Description);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Name_ReturnsExpected_AccessibleName()
    {
        using RadioButton radioButton = new()
        {
            AccessibleName = "TestName"
        };

        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        Assert.Equal("TestName", radioButtonAccessibleObject.Name);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Name_ReturnsExpected_FromText()
    {
        using RadioButton radioButton = new() { Text = "TestText" };
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        radioButtonAccessibleObject.Name.Should().Be("TestText");
    }

    [WinFormsTheory]
    [InlineData(true, "TestText")]
    [InlineData(false, "&TestText")]
    public void RadioButtonAccessibleObject_Name_ReturnsExpected_FromText_WithOrWithoutMnemonics(bool useMnemonic, string expectedName)
    {
        using RadioButton radioButton = new() { Text = "&TestText", UseMnemonic = useMnemonic };
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        radioButtonAccessibleObject.Name.Should().Be(expectedName);
    }

    [WinFormsTheory]
    [InlineData(true, "Alt+r")]
    [InlineData(false, null)]
    public void RadioButtonAccessibleObject_KeyboardShortcut_ReturnsExpected(bool useMnemonic, string expectedShortcut)
    {
        using RadioButton radioButton = new() { Text = "&RadioButton", UseMnemonic = useMnemonic };
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        radioButtonAccessibleObject.KeyboardShortcut.Should().Be(expectedShortcut);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Role_Custom_ReturnsExpected()
    {
        using RadioButton radioButton = new()
        {
            AccessibleRole = AccessibleRole.PushButton
        };

        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        Assert.Equal(AccessibleRole.PushButton, radioButtonAccessibleObject.Role);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsFact]
    public void RadioButtonAccessibleObject_Role_Default_ReturnsExpected()
    {
        using RadioButton radioButton = new();
        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

        Assert.Equal(AccessibleRole.RadioButton, radioButtonAccessibleObject.Role);
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleStates.Focusable, AccessibleStates.Focusable | AccessibleStates.Checked)]
    [InlineData(false, AccessibleStates.None, AccessibleStates.None)]
    public void RadioButtonAccessibleObject_State_ReturnsExpected(bool createControl, AccessibleStates accessibleStatesFirstStage, AccessibleStates accessibleStatesSecondStage)
    {
        using RadioButton radioButton = new();

        if (createControl)
        {
            radioButton.CreateControl();
        }

        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);
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
        using RadioButton radioButton = new();

        if (createControl)
        {
            radioButton.CreateControl();
        }

        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);
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
        using RadioButton radioButton = new()
        {
            Name = "RadioButton1",
            AccessibleName = "TestName"
        };

        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);
        using VARIANT value = radioButtonAccessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID);

        Assert.Equal(expected, value.ToObject());
        Assert.False(radioButton.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_SelectionItemPatternId)]
    public void RadioButtonAccessibleObject_IsPatternSupported_Invoke_ReturnsExpected(int patternId)
    {
        using RadioButton radioButton = new()
        {
            Name = "RadioButton1"
        };

        Assert.False(radioButton.IsHandleCreated);
        RadioButtonAccessibleObject radioButtonAccessibleObject = new(radioButton);

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
        using RadioButton radioButton = new();
        radioButton.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)radioButton.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(radioButton.IsHandleCreated);
    }
}
