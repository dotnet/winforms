// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripOverflow;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripOverflow_ToolStripOverflowAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_ctor_default()
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);
        ToolStripOverflowAccessibleObject accessibleObject = new(toolStripOverflow);

        Assert.Equal(toolStripOverflow, accessibleObject.Owner);
        Assert.False(toolStripOverflow.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_ControlType_IsToolBar_IfAccessibleRoleIsDefault()
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);
        // AccessibleRole is not set = Default

        AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_MenuControlTypeId, actual);
        Assert.False(toolStripOverflow.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_Role_IsToolBar_ByDefault()
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);
        // AccessibleRole is not set = Default

        object actual = toolStripOverflow.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.MenuPopup, actual);
        Assert.False(toolStripOverflow.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripOverflowAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripOverflowAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripOverflowAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);
        toolStripOverflow.AccessibleRole = role;

        AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(role, accessibleObject.Role);
        Assert.Equal(expected, actual);
        Assert.False(toolStripOverflow.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)NavigateDirection.NavigateDirection_NextSibling)]
    [InlineData((int)NavigateDirection.NavigateDirection_PreviousSibling)]
    [InlineData((int)NavigateDirection.NavigateDirection_FirstChild)]
    [InlineData((int)NavigateDirection.NavigateDirection_LastChild)]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleNotCreated(int navigateDirection)
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);

        AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
        AccessibleObject expected = toolStripItem.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate((NavigateDirection)navigateDirection));
        Assert.False(toolStripOverflow.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);

        AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;
        AccessibleObject expected = toolStripItem.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected()
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);

        AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
    {
        using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

        toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

        AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected_IfItemAligned()
    {
        using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

        AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
    {
        using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

        toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

        AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[4].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_IfItemAligned()
    {
        using ToolStrip toolStrip = CreateToolStripWithOverflow(5);

        toolStrip.Items[0].Alignment = ToolStripItemAlignment.Right;

        toolStrip.PerformLayout();

        toolStrip.OverflowButton.DropDown.CreateControl(ignoreVisible: true);

        AccessibleObject accessibleObject = toolStrip.OverflowButton.DropDown.AccessibilityObject;
        AccessibleObject expected = toolStrip.Items[4].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void ToolStripOverflowAccessibleObject_FragmentNavigate_Child_ReturnsNullIfOverflowEmpty()
    {
        using ToolStripButton toolStripItem = new();
        using ToolStripOverflow toolStripOverflow = new(toolStripItem);

        toolStripOverflow.CreateControl(ignoreVisible: true);

        AccessibleObject accessibleObject = toolStripOverflow.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    private static ToolStrip CreateToolStripWithOverflow(int itemCount)
    {
        ToolStrip toolStrip = new()
        {
            AutoSize = false,
            Size = new(20, 30),
            CanOverflow = true,
        };

        for (int i = 0; i < itemCount; i++)
        {
            toolStrip.Items.Add(CreateItem(ToolStripItemAlignment.Left));
        }

        return toolStrip;

        static ToolStripItem CreateItem(ToolStripItemAlignment align)
        {
            return new ToolStripButton
            {
                AutoSize = false,
                Size = new(25, 25),
                Alignment = align
            };
        }
    }
}
