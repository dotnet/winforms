// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.DateTimePicker;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class DateTimePicker_DateTimePickerAccessibleObjectTests
{
    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Ctor_Default()
    {
        using DateTimePicker dateTimePicker = new();

        DateTimePickerAccessibleObject accessibleObject = new(dateTimePicker);

        Assert.Equal(dateTimePicker, accessibleObject.Owner);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
    {
        using DateTimePicker dateTimePicker = new();
        // AccessibleRole is not set = Default

        VARIANT actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId, (UIA_CONTROLTYPE_ID)(int)actual);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Role_IsComboBox_ByDefault()
    {
        using DateTimePicker dateTimePicker = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = dateTimePicker.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.ComboBox, actual);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_LocalizedControlType_ReturnsExpected_IfAccessibleRoleIsDefault()
    {
        using DateTimePicker dateTimePicker = new();
        // AccessibleRole is not set = Default

        string actual = ((BSTR)dateTimePicker.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LocalizedControlTypePropertyId)).ToStringAndFree();
        string expected = SR.DateTimePickerLocalizedControlType;

        Assert.Equal(expected, actual);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    public static IEnumerable<object[]> DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using DateTimePicker dateTimePicker = new();
        dateTimePicker.AccessibleRole = role;

        UIA_CONTROLTYPE_ID actual = (UIA_CONTROLTYPE_ID)(int)dateTimePicker.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(DateTimePickerAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void DateTimePickerAccessibleObject_GetPropertyValue_LocalizedControlType_IsNull_ForCustomRole(AccessibleRole role)
    {
        using DateTimePicker dateTimePicker = new();
        dateTimePicker.AccessibleRole = role;

        VARIANT actual = dateTimePicker.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LocalizedControlTypePropertyId);

        Assert.Equal(VARIANT.Empty, actual);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();
        DateTime dt = new(2000, 1, 1);
        dateTimePicker.Value = dt;
        AccessibleObject accessibleObject = dateTimePicker.AccessibilityObject;

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId));
        Assert.False(dateTimePicker.IsHandleCreated);

        dateTimePicker.CreateControl();

        Assert.Equal(dt.ToLongDateString(), ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId)).ToStringAndFree());
    }

    [WinFormsTheory]
    [InlineData((int)ExpandCollapseState.ExpandCollapseState_Expanded)]
    [InlineData((int)ExpandCollapseState.ExpandCollapseState_Collapsed)]
    public void DateTimePickerAccessibleObject_ExpandCollapseState_ReturnsExpected(int expandCollapseState)
    {
        using DateTimePicker dateTimePicker = new();

        var expected = (ExpandCollapseState)expandCollapseState;
        var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;
        dateTimePicker.TestAccessor().Dynamic._expandCollapseState = expected;

        ExpandCollapseState actual = accessibleObject.ExpandCollapseState;

        Assert.Equal(expected, actual);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Expand_IfHandleIsNotCreated_NothingChanges()
    {
        using DateTimePicker dateTimePicker = new();

        var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

        // ExpandCollapseState is Collapsed before some actions
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);

        accessibleObject.Expand();

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Collapse_IfHandleIsNotCreated_NothingChanges()
    {
        using DateTimePicker dateTimePicker = new();

        var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

        // ExpandCollapseState is Collapsed before some actions
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);

        dateTimePicker.TestAccessor().Dynamic._expandCollapseState = ExpandCollapseState.ExpandCollapseState_Expanded;

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, accessibleObject.ExpandCollapseState);

        accessibleObject.Collapse();

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, accessibleObject.ExpandCollapseState);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Expand_IfControlAlreadyIsExpanded_NothingChanges()
    {
        using DateTimePicker dateTimePicker = new();

        dateTimePicker.CreateControl();
        var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

        // ExpandCollapseState is Collapsed before some actions
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);

        dateTimePicker.TestAccessor().Dynamic._expandCollapseState = ExpandCollapseState.ExpandCollapseState_Expanded;

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, accessibleObject.ExpandCollapseState);

        accessibleObject.Expand();

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, accessibleObject.ExpandCollapseState);
        Assert.True(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Collapse_IfControlAlreadyIsCollapsed_NothingChanges()
    {
        using DateTimePicker dateTimePicker = new();

        dateTimePicker.CreateControl();
        var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

        // ExpandCollapseState is Collapsed before some actions
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);

        accessibleObject.Collapse();

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);
        Assert.True(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Expand_IfHandleIsCreated_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();

        dateTimePicker.CreateControl();
        var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

        // ExpandCollapseState is Collapsed before some actions
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);

        accessibleObject.Expand();

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, accessibleObject.ExpandCollapseState);
        Assert.True(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Collapse_IfHandleIsCreated_ReturnsExpected()
    {
        using DateTimePicker dateTimePicker = new();

        dateTimePicker.CreateControl();
        var accessibleObject = (DateTimePickerAccessibleObject)dateTimePicker.AccessibilityObject;

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);

        // If don't call Expand() on this control and just change state value instead
        // then call Collapse() does't work correctly due to the control is not expanded factually
        accessibleObject.Expand();

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, accessibleObject.ExpandCollapseState);

        accessibleObject.Collapse();

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);
        Assert.True(dateTimePicker.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_ExpandCollapsePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    public void DateTimePickerAccessibleObject_IsPatternSupported_ReturnsExpected_IfDoNotShowCheckbox(int patternId)
    {
        using DateTimePicker dateTimePicker = new() { ShowCheckBox = false };

        AccessibleObject accessibleObject = dateTimePicker.AccessibilityObject;

        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TogglePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_ExpandCollapsePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    public void DateTimePickerAccessibleObject_IsPatternSupported_ReturnsExpected_IfShowCheckbox(int patternId)
    {
        using DateTimePicker dateTimePicker = new() { ShowCheckBox = true };

        AccessibleObject accessibleObject = dateTimePicker.AccessibilityObject;

        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Name_ReturnsEmptyString_IfControlAccessibleNameIsNotNull()
    {
        using DateTimePicker dateTimePicker = new();

        Assert.Equal(string.Empty, dateTimePicker.AccessibilityObject.Name);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    [WinFormsFact]
    public void DateTimePickerAccessibleObject_Name_ReturnsExpected_IfControlAccessibleNameIsNotNull()
    {
        string testAccessibleName = "TestDateTimePicker";
        using DateTimePicker dateTimePicker = new() { AccessibleName = testAccessibleName };

        Assert.Equal(testAccessibleName, dateTimePicker.AccessibilityObject.Name);
        Assert.False(dateTimePicker.IsHandleCreated);
    }

    public static IEnumerable<object[]> DateTimePickerAccessibleObject_DefaultAction_ReturnsExpected_TestData()
    {
        // Expanded dtp control has "Collapse" as a default action, else "Expand".
        yield return new object[] { true, SR.AccessibleActionCollapse };
        yield return new object[] { false, SR.AccessibleActionExpand };
    }

    [WinFormsTheory]
    [MemberData(nameof(DateTimePickerAccessibleObject_DefaultAction_ReturnsExpected_TestData))]
    public void DateTimePickerAccessibleObject_DefaultAction_ReturnsExpected(bool isExpanded, string expected)
    {
        using DateTimePicker dateTimePicker = new();
        dateTimePicker.CreateControl();

        AccessibleObject accessibleObject = dateTimePicker.AccessibilityObject;

        if (isExpanded)
        {
            accessibleObject.Expand();
        }

        Assert.Equal(expected, accessibleObject.DefaultAction);
        Assert.True(dateTimePicker.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, (int)ExpandCollapseState.ExpandCollapseState_Collapsed)]
    [InlineData(false, (int)ExpandCollapseState.ExpandCollapseState_Expanded)]
    public void DateTimePickerAccessibleObject_DoDefaultAction_IfHandleIsCreated_ReturnsExpected(bool isExpanded, int expected)
    {
        using DateTimePicker dateTimePicker = new();
        dateTimePicker.CreateControl();

        AccessibleObject accessibleObject = dateTimePicker.AccessibilityObject;

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);

        if (isExpanded)
        {
            accessibleObject.Expand();

            Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, accessibleObject.ExpandCollapseState);
        }

        accessibleObject.DoDefaultAction();

        Assert.Equal((ExpandCollapseState)expected, accessibleObject.ExpandCollapseState);
        Assert.True(dateTimePicker.IsHandleCreated);
    }

    // Unit Test for https://github.com/dotnet/winforms/issues/9281.
    [WinFormsFact]
    public void DateTimePickerAccessibleObject_KeyboardShortcut_ReturnsExpected()
    {
        using Form form = new();
        using DateTimePicker dateTimePicker1 = new();
        using Label label1 = new();
        using DateTimePicker dateTimePicker2 = new();

        dateTimePicker1.CustomFormat = "'Date&Time' hh:mm dd/MM";
        dateTimePicker1.Format = DateTimePickerFormat.Custom;
        dateTimePicker1.TabIndex = 0;

        label1.Text = "&Date";
        label1.TabIndex = 1;

        dateTimePicker2.TabIndex = 2;

        form.Controls.Add(dateTimePicker2);
        form.Controls.Add(label1);
        form.Controls.Add(dateTimePicker1);

        string keyboardShortcut = dateTimePicker1.AccessibilityObject.KeyboardShortcut;

        Assert.Null(keyboardShortcut);

        keyboardShortcut = dateTimePicker2.AccessibilityObject.KeyboardShortcut;

        Assert.Equal("Alt+d", keyboardShortcut);
    }
}
