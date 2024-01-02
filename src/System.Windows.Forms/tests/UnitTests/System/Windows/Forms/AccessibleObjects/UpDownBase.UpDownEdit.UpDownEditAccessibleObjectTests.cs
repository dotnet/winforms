// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class UpDownEditAccessibleObjectTests
{
    [WinFormsFact]
    public void UpDownEditAccessibleObject_ctor_default()
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        UpDownBase.UpDownEdit.UpDownEditAccessibleObject accessibleObject = new(upDownEdit, upDown);
        Assert.Equal(upDownEdit, accessibleObject.Owner);
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_ctor_ThrowsException_IfParentIsNull()
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        Assert.Throws<ArgumentNullException>(() => new UpDownBase.UpDownEdit.UpDownEditAccessibleObject(upDownEdit, null));
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
    {
        using UpDownBase upDown = new SubUpDownBase();
        Assert.Throws<ArgumentNullException>(() => new UpDownBase.UpDownEdit.UpDownEditAccessibleObject(null, upDown));
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_IsIAccessibleExSupported_ReturnsTrue()
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.True(accessibleObject.IsIAccessibleExSupported());
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_Custom_Name_ReturnsCorrectValue()
    {
        using UpDownBase upDown = new SubUpDownBase();
        string name = "Custom name";
        upDown.AccessibleName = name;
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.Equal(name, accessibleObject.Name);
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_Default_Name_ReturnsNull()
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.Null(accessibleObject.Name);
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_Default_Name_ReturnsExpected_NumericUpDown()
    {
        using NumericUpDown upDown = new();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.Equal(SR.EditDefaultAccessibleName, accessibleObject.Name);
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_Default_Name_ReturnsExpected_DomainUpDown()
    {
        using DomainUpDown upDown = new();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.Equal(SR.EditDefaultAccessibleName, accessibleObject.Name);
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void UpDownEditAccessibleObject_KeyboardShortcut_ReturnsParentsKeyboardShortcut()
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.Equal(upDown.AccessibilityObject.KeyboardShortcut, accessibleObject.KeyboardShortcut);
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void UpDownEditAccessibleObject_IsReadOnly_IsExpected(bool readOnly)
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        upDownEdit.ReadOnly = readOnly;
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.Equal(readOnly, accessibleObject.IsReadOnly);
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId)]
    public void UpDownEditAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.True((bool)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID));
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPattern2Id)]
    public void UpDownEditAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(upDown.IsHandleCreated);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Text, (int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId)]
    [InlineData(false, AccessibleRole.None, (int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId)]
    public void UpDownEditAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, AccessibleRole expectedRole, int expectedType)
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        // AccessibleRole is not set = Default

        if (createControl)
        {
            upDownEdit.CreateControl();
        }

        AccessibleObject accessibleObject = upDownEdit.AccessibilityObject;
        int actual = (int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedRole, accessibleObject.Role);
        Assert.Equal(expectedType, actual);
        Assert.Equal(createControl, upDownEdit.IsHandleCreated);
    }

    public static IEnumerable<object[]> UpDownEditAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(UpDownEditAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void UpDownEditAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using UpDownBase upDown = new SubUpDownBase();
        using UpDownBase.UpDownEdit upDownEdit = new(upDown);
        upDownEdit.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)upDownEdit.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(upDownEdit.IsHandleCreated);
    }

    private class SubUpDownBase : UpDownBase
    {
        public override void DownButton() { }

        public override void UpButton() { }

        protected override void UpdateEditText() { }
    }
}
