// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripDropDown;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripDropDown_ToolStripDropDownAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_ctor_default()
    {
        using ToolStripDropDown toolStripDropDown = new();
        ToolStripDropDownAccessibleObject accessibleObject = new(toolStripDropDown);

        Assert.Equal(toolStripDropDown, accessibleObject.Owner);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_ControlType_IsMenu_IfAccessibleRoleIsDefault()
    {
        using ToolStripDropDown toolStripDropDown = new();
        // AccessibleRole is not set = Default

        AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_MenuControlTypeId, actual);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_Role_IsMenuPopup_ByDefault()
    {
        using ToolStripDropDown toolStripDropDown = new();
        // AccessibleRole is not set = Default

        AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
        AccessibleRole actual = accessibleObject.Role;

        Assert.Equal(AccessibleRole.MenuPopup, actual);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }

    public static IEnumerable<object[]> ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
    [MemberData(nameof(ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
    public void ToolStripDropDownAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
    {
        using ToolStripDropDown toolStripDropDown = new();
        toolStripDropDown.AccessibleRole = role;

        AccessibleObject accessibleObject = toolStripDropDown.AccessibilityObject;
        var actual = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(role, accessibleObject.Role);
        Assert.Equal(expected, actual);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_Name_ContextMenuStrip_HasDefaultName()
    {
        // A context menu has no owner item to take the name from.
        using ContextMenuStrip contextMenuStrip = new();

        AccessibleObject accessibleObject = contextMenuStrip.AccessibilityObject;

        Assert.Equal(SR.ContextMenuStripDefaultAccessibleName, accessibleObject.Name);
        Assert.Equal(
            SR.ContextMenuStripDefaultAccessibleName,
            ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.False(contextMenuStrip.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("Custom name")]
    [InlineData("")]
    public void ToolStripDropDownAccessibleObject_Name_ContextMenuStrip_AccessibleNameWins(string accessibleName)
    {
        // Even an empty string: it is the only way to force the accessible name to be blank.
        using ContextMenuStrip contextMenuStrip = new() { AccessibleName = accessibleName };

        Assert.Equal(accessibleName, contextMenuStrip.AccessibilityObject.Name);
        Assert.False(contextMenuStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_Name_DropDownOfAnItem_IsTheNameOfTheItem()
    {
        using ToolStripMenuItem ownerItem = new("Owner item");
        ToolStripDropDown dropDown = ownerItem.DropDown;

        Assert.Equal(ownerItem.AccessibilityObject.Name, dropDown.AccessibilityObject.Name);
        Assert.False(dropDown.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_Name_ContextMenuStripWithOwnerItem_IsTheNameOfTheItem()
    {
        // A ContextMenuStrip assigned to ToolStripDropDownItem.DropDown gets its OwnerItem while it is shown.
        using ToolStripMenuItem ownerItem = new("Owner item");
        using ContextMenuStrip contextMenuStrip = new() { OwnerItem = ownerItem };

        Assert.Equal(ownerItem.AccessibilityObject.Name, contextMenuStrip.AccessibilityObject.Name);

        contextMenuStrip.OwnerItem = null;

        Assert.Equal(SR.ContextMenuStripDefaultAccessibleName, contextMenuStrip.AccessibilityObject.Name);
    }

    [WinFormsFact]
    public void ToolStripDropDownAccessibleObject_Name_PlainDropDownWithoutOwnerItem_IsNull()
    {
        using ToolStripDropDown toolStripDropDown = new();

        Assert.Null(toolStripDropDown.AccessibilityObject.Name);
        Assert.False(toolStripDropDown.IsHandleCreated);
    }
}
