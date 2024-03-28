// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripComboBox;
using static System.Windows.Forms.ToolStripComboBox.ToolStripComboBoxControl;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripComboBox_ToolStripComboBoxControl_ToolStripComboBoxControlAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripComboBoxControlAccessibleObject_ctor_default()
    {
        using ToolStripComboBox toolStripComboBox = new();
        ToolStripComboBoxControl control = (ToolStripComboBoxControl)toolStripComboBox.ComboBox;
        control.CreateControl();
        ToolStripComboBoxControlAccessibleObject accessibleObject = new(control);

        Assert.Equal(control, accessibleObject.Owner);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripComboBoxControlAccessibleObject_ControlType_IsComboBox_IfAccessibleRoleIsDefault()
    {
        using ToolStripComboBox toolStripComboBox = new();
        // AccessibleRole is not set = Default
        ToolStripComboBoxControl control = (ToolStripComboBoxControl)toolStripComboBox.ComboBox;
        control.CreateControl();

        AccessibleObject accessibleObject = toolStripComboBox.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(AccessibleRole.ComboBox, accessibleObject.Role);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ComboBoxControlTypeId, actual);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripComboBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripComboBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripComboBoxControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripComboBox toolStripComboBox = new();
        toolStripComboBox.AccessibleRole = role;
        ToolStripComboBoxControl control = (ToolStripComboBoxControl)toolStripComboBox.ComboBox;
        control.CreateControl();

        AccessibleObject accessibleObject = toolStripComboBox.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(role, accessibleObject.Role);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);
        Assert.Equal(expected, actual);
    }

    [WinFormsFact]
    public void ToolStripComboBoxControlAccessibleObject_FragmentNavigate_ChildrenAreExpected()
    {
        using ToolStripComboBoxControl control = new();
        control.CreateControl();

        object firstChild = control.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        object lastChild = control.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.Equal(control.ChildEditAccessibleObject, firstChild);
        Assert.Equal(((ToolStripComboBoxControlAccessibleObject)control.AccessibilityObject).DropDownButtonUiaProvider, lastChild);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripComboBoxControlAccessibleObject_FragmentNavigate_ParentIsToolStrip()
    {
        using NoAssertContext noAssertContext = new();
        using ToolStripComboBoxControl control = new();
        using ToolStripComboBox item = new();
        using ToolStrip toolStrip = new();
        control.Owner = item;
        item.Parent = toolStrip;

        object actual = control.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        Assert.Equal(toolStrip.AccessibilityObject, actual);
        Assert.False(control.IsHandleCreated);
        Assert.False(toolStrip.IsHandleCreated);
    }
}
