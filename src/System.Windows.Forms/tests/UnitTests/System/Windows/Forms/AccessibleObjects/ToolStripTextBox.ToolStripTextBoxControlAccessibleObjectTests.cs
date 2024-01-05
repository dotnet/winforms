// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.Control;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripTextBox_ToolStripTextBoxControlAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripTextBoxControlAccessibleObject_ctor_default()
    {
        using ToolStripTextBox toolStripTextBox = new();
        TextBox textBox = toolStripTextBox.TextBox;
        Type type = toolStripTextBox.GetType().GetNestedType("ToolStripTextBoxControlAccessibleObject", BindingFlags.NonPublic);
        Assert.NotNull(type);
        ControlAccessibleObject accessibleObject = (ControlAccessibleObject)Activator.CreateInstance(type, textBox);
        Assert.Equal(textBox, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripTextBoxControlAccessibleObject_ctor_ThrowsException_IfOwnerIsNull()
    {
        using ToolStripTextBox toolStripTextBox = new();
        TextBox textBox = toolStripTextBox.TextBox;
        Type type = toolStripTextBox.GetType().GetNestedType("ToolStripTextBoxControlAccessibleObject", BindingFlags.NonPublic);
        Assert.NotNull(type);
        Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(type, (Control)null));
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToolStripTextBoxControlAccessibleObject_IsReadOnly_IsExpected(bool readOnly)
    {
        using ToolStripTextBox toolStripTextBox = new();
        TextBox textBox = toolStripTextBox.TextBox;
        textBox.ReadOnly = readOnly;
        AccessibleObject accessibleObject = textBox.AccessibilityObject;
        Assert.Equal(readOnly, accessibleObject.IsReadOnly);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId)]
    [InlineData((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId)]
    public void ToolStripTextBoxControlAccessibleObject_GetPropertyValue_PatternsSuported(int propertyID)
    {
        using ToolStripTextBox toolStripTextBox = new();
        TextBox textBox = toolStripTextBox.TextBox;
        AccessibleObject accessibleObject = textBox.AccessibilityObject;
        Assert.True((bool)accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyID));
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_ValuePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_TextPattern2Id)]
    public void ToolStripTextBoxControlAccessibleObject_IsPatternSupported_PatternsSuported(int patternId)
    {
        using ToolStripTextBox toolStripTextBox = new();
        TextBox textBox = toolStripTextBox.TextBox;
        AccessibleObject accessibleObject = textBox.AccessibilityObject;
        Assert.True(accessibleObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
    }

    [WinFormsTheory]
    [InlineData(true, (int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId)]
    [InlineData(false, (int)UIA_CONTROLTYPE_ID.UIA_PaneControlTypeId)]
    public void ToolStripTextBoxControlAccessibleObject_ControlType_IsExpected_IfAccessibleRoleIsDefault(bool createControl, int expectedType)
    {
        using ToolStripTextBox toolStripTextBox = new();
        // AccessibleRole is not set = Default
        TextBox toolStripTextBoxControl = toolStripTextBox.TextBox;

        if (createControl)
        {
            toolStripTextBoxControl.CreateControl();
        }

        int actual = (int)toolStripTextBox.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(expectedType, actual);
        Assert.Equal(createControl, toolStripTextBoxControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.Text)]
    [InlineData(false, AccessibleRole.None)]
    public void ToolStripTextBoxControlAccessibleObject_Default_Role_IsExpected(bool createControl, AccessibleRole expectedRole)
    {
        using ToolStripTextBox toolStripTextBox = new();
        // AccessibleRole is not set = Default
        TextBox toolStripTextBoxControl = toolStripTextBox.TextBox;

        if (createControl)
        {
            toolStripTextBoxControl.CreateControl();
        }

        object actual = toolStripTextBox.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.Equal(createControl, toolStripTextBoxControl.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripTextBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripTextBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripTextBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripTextBox toolStripTextBox = new();
        toolStripTextBox.AccessibleRole = role;

        TextBox toolStripTextBoxControl = toolStripTextBox.TextBox;
        AccessibleObject accessibleObject = toolStripTextBox.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(role, accessibleObject.Role);
        Assert.Equal(expected, actual);
        Assert.False(toolStripTextBoxControl.IsHandleCreated);
    }
}
