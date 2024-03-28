// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripProgressBar;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripProgressBar_ToolStripProgressBarControl_ToolStripProgressBarControlAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripProgressBarControlAccessibleObject_ctor_default()
    {
        using ToolStripProgressBarControl toolStripProgressBarControl = new();
        ToolStripProgressBarControlAccessibleObject accessibleObject = new(toolStripProgressBarControl);

        Assert.Equal(toolStripProgressBarControl, accessibleObject.Owner);
        Assert.False(toolStripProgressBarControl.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripProgressBarControlAccessibleObject_ControlType_IsProgressBar_IfAccessibleRoleIsDefault()
    {
        using ToolStripProgressBarControl toolStripProgressBarControl = new();
        // AccessibleRole is not set = Default

        var actual = (UIA_CONTROLTYPE_ID)(int)toolStripProgressBarControl.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ProgressBarControlTypeId, actual);
        Assert.False(toolStripProgressBarControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.ProgressBar)]
    [InlineData(false, AccessibleRole.None)]
    public void ToolStripProgressBarControlAccessibleObject_Role_IsExpected_ByDefault(bool createControl, AccessibleRole expectedRole)
    {
        using ToolStripProgressBarControl toolStripProgressBarControl = new();
        // AccessibleRole is not set = Default

        if (createControl)
        {
            toolStripProgressBarControl.CreateControl();
        }

        object actual = toolStripProgressBarControl.AccessibilityObject.Role;

        Assert.Equal(expectedRole, actual);
        Assert.Equal(createControl, toolStripProgressBarControl.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripProgressBarControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripProgressBarControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripProgressBarControlAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripProgressBarControl toolStripProgressBarControl = new();
        toolStripProgressBarControl.AccessibleRole = role;

        AccessibleObject accessibleObject = toolStripProgressBarControl.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(role, accessibleObject.Role);
        Assert.Equal(expected, actual);
        Assert.False(toolStripProgressBarControl.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData((int)NavigateDirection.NavigateDirection_LastChild)]
    public void ToolStripProgressBarControlAccessibleObject_FragmentNavigate_ChildrenAreNull(int direction)
    {
        using ToolStripProgressBarControl control = new();

        object actual = control.AccessibilityObject.FragmentNavigate((NavigateDirection)direction);

        Assert.Null(actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripProgressBarControlAccessibleObject_FragmentNavigate_ParentIsToolStrip()
    {
        using NoAssertContext noAssertContext = new();
        using ToolStripProgressBarControl control = new();
        using ToolStripProgressBar item = new();
        using ToolStrip toolStrip = new();
        control.Owner = item;
        item.Parent = toolStrip;

        object actual = control.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        Assert.Equal(toolStrip.AccessibilityObject, actual);
        Assert.False(control.IsHandleCreated);
        Assert.False(toolStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripProgressBarControlAccessibleObject_FragmentNavigate_SiblingsAreExpected()
    {
        using NoAssertContext noAssertContext = new();
        using ToolStripProgressBarControl control = new();
        using ToolStripProgressBar item = new();
        using ToolStrip toolStrip = new() { Size = new Drawing.Size(500, 25) };
        using ToolStripLabel label = new() { Text = "Label" };
        using ToolStripButton button = new() { Text = "Button" };

        toolStrip.CreateControl();
        control.Owner = item;
        item.Parent = toolStrip;
        toolStrip.Items.Add(label);
        toolStrip.Items.Add(item);
        toolStrip.Items.Add(button);

        object nextSibling = control.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        object previousSibling = control.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

        Assert.Equal(button.AccessibilityObject, nextSibling);
        Assert.Equal(label.AccessibilityObject, previousSibling);
        Assert.False(control.IsHandleCreated);
        Assert.True(toolStrip.IsHandleCreated);
    }
}
