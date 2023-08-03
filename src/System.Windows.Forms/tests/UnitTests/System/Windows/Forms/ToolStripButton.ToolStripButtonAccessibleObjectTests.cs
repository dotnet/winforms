// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.Windows.Forms.ToolStripButton;
using static Interop;

namespace System.Windows.Forms.Tests;

public class ToolStripButton_ToolStripButtonAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_Ctor_Default()
    {
        using ToolStripButton toolStripButton = new();
        ToolStripButtonAccessibleObject accessibleObject = new(toolStripButton);

        Assert.Equal(toolStripButton, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_ControlType_IsButton_IfAccessibleRoleIsDefault()
    {
        using ToolStripButton toolStripButton = new();
        // AccessibleRole is not set = Default

        object actual = toolStripButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

        Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_Role_IsPushButton_ByDefault()
    {
        using ToolStripButton toolStripButton = new();
        // AccessibleRole is not set = Default

        AccessibleRole actual = toolStripButton.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.PushButton, actual);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_Role_IsCheckButton_IfCheckOnClick()
    {
        using ToolStripButton toolStripButton = new()
        {
            CheckOnClick = true
        };

        AccessibleRole actual = toolStripButton.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.CheckButton, actual);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_Role_IsCheckButton_IfChecked()
    {
        using ToolStripButton toolStripButton = new()
        {
            Checked = true
        };

        AccessibleRole actual = toolStripButton.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.CheckButton, actual);
    }

    public static IEnumerable<object[]> ToolStripButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripButtonAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripButton toolStripButton = new();
        toolStripButton.AccessibleRole = role;

        object actual = toolStripButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
        UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_GetPropertyValue_ControlType_IsCheckBox_IfCheckOnClick()
    {
        using ToolStripButton toolStripButton = new()
        {
            CheckOnClick = true
        };

        object actual = toolStripButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
        UiaCore.UIA expected = UiaCore.UIA.CheckBoxControlTypeId;

        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_GetPropertyValue_ControlType_IsCheckBox_IfChecked()
    {
        using ToolStripButton toolStripButton = new()
        {
            Checked = true
        };

        object actual = toolStripButton.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
        UiaCore.UIA expected = UiaCore.UIA.CheckBoxControlTypeId;

        Assert.Equal(expected, actual);
    }

    [WinFormsTheory]
    [InlineData(true, CheckState.Checked, true)]
    [InlineData(true, CheckState.Unchecked, true)]
    [InlineData(true, CheckState.Indeterminate, true)]
    [InlineData(false, CheckState.Checked, true)]
    [InlineData(false, CheckState.Unchecked, false)]
    [InlineData(false, CheckState.Indeterminate, true)]
    public void ToolStripButtonAccessibleObject_IsTogglePatternSupported_ReturnsExpected(bool checkOnClick, CheckState checkState, bool expected)
    {
        using ToolStripButton toolStripButton = new()
        {
            CheckOnClick = checkOnClick,
            CheckState = checkState
        };

        object actual = toolStripButton.AccessibilityObject.IsPatternSupported(UiaCore.UIA.TogglePatternId);

        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_IsTogglePatternSupported_ReturnsTrue_IfAccessibleRoleIsCheckButton()
    {
        using ToolStripButton toolStripButton = new()
        {
            AccessibleRole = AccessibleRole.CheckButton
        };

        object actual = toolStripButton.AccessibilityObject.IsPatternSupported(UiaCore.UIA.TogglePatternId);

        Assert.Equal(true, actual);
    }

    [WinFormsTheory]
    [InlineData(CheckState.Checked, (int)UiaCore.ToggleState.On)]
    [InlineData(CheckState.Unchecked, (int)UiaCore.ToggleState.Off)]
    [InlineData(CheckState.Indeterminate, (int)UiaCore.ToggleState.Indeterminate)]
    public void ToolStripButtonAccessibleObject_ToggleState_ReturnsExpected(CheckState checkState, int expectedToggleState)
    {
        using ToolStripButton toolStripButton = new()
        {
            CheckState = checkState
        };

        object actual = toolStripButton.AccessibilityObject.ToggleState;

        Assert.Equal((UiaCore.ToggleState)expectedToggleState, actual);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_Toggle_Invoke()
    {
        using ToolStripButton toolStripButton = new()
        {
            CheckOnClick = true
        };

        int clickCounter = 0;

        toolStripButton.Click += (s, e) => { clickCounter++; };

        Assert.Equal(UiaCore.ToggleState.Off, toolStripButton.AccessibilityObject.ToggleState);

        toolStripButton.AccessibilityObject.Toggle();

        Assert.Equal(UiaCore.ToggleState.On, toolStripButton.AccessibilityObject.ToggleState);

        toolStripButton.AccessibilityObject.Toggle();

        Assert.Equal(UiaCore.ToggleState.Off, toolStripButton.AccessibilityObject.ToggleState);

        Assert.Equal(0, clickCounter);
    }

    [WinFormsFact]
    public void ToolStripButtonAccessibleObject_Toggle_DoesNotChangeChecked_IfTogglePatternNotSupported()
    {
        using ToolStripButton toolStripButton = new();

        Assert.False(toolStripButton.AccessibilityObject.IsPatternSupported(UiaCore.UIA.TogglePatternId));
        Assert.False(toolStripButton.Checked);

        toolStripButton.AccessibilityObject.Toggle();

        Assert.False(toolStripButton.Checked);
    }
}
