// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ToolStripSplitButton;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripSplitButton_ToolStripSplitButtonExAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_Ctor_OwnerToolStripSplitButtonCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripSplitButtonExAccessibleObject(null));
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_ControlType_ReturnsExpected()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        Assert.True(accessibleObject.IsIAccessibleExSupported());
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_DropDownItemsCount_ReturnsExpected_IfDropDownCollapsed()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, accessibleObject.ExpandCollapseState);
        Assert.Equal(0, accessibleObject.TestAccessor().Dynamic.DropDownItemsCount);
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Parent_ReturnsExpected()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        using ToolStrip toolStrip = new();
        toolStrip.Items.Add(toolStripSplitButton);
        toolStrip.PerformLayout();
        toolStrip.CreateControl();

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);
        AccessibleObject expected = toolStrip.AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Parent_ReturnsNull_IfHandleNotCreated()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        using ToolStrip toolStrip = new();
        toolStrip.Items.Add(toolStripSplitButton);

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(toolStrip.IsHandleCreated);
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsExpected()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        ToolStripItem item1 = toolStripSplitButton.DropDownItems.Add(string.Empty);
        ToolStripItem item2 = toolStripSplitButton.DropDownItems.Add(string.Empty);

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        toolStripSplitButton.DropDown.Show();

        Assert.Equal(item1.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(item2.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfDropDownNotOpened()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        toolStripSplitButton.DropDownItems.Add(string.Empty);

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsNull_IfNoDropDownItems()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }

    [WinFormsFact]
    public void ToolStripSplitButtonExAccessibleObject_FragmentNavigate_Child_ReturnsExpected_IfItemsAligned()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        ToolStripItem item1 = toolStripSplitButton.DropDownItems.Add(string.Empty);
        ToolStripItem item2 = toolStripSplitButton.DropDownItems.Add(string.Empty);

        item1.Alignment = ToolStripItemAlignment.Right;

        ToolStripSplitButtonExAccessibleObject accessibleObject = new(toolStripSplitButton);

        toolStripSplitButton.DropDown.Show();

        Assert.Equal(item1.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(item2.AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
    }
}
