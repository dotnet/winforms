// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripDropDownItemAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_Ctor_OwnerToolStripDropDownItemCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripDropDownItemAccessibleObject(null));
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_Ctor_Default()
    {
        using SubToolStripDropDownItem control = new();
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.NotNull(accessibleObject);
        Assert.Equal(control, accessibleObject.Owner);
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_Role_ReturnsExpected_ControlHasDefaultRole()
    {
        using SubToolStripDropDownItem control = new();
        control.AccessibleRole = AccessibleRole.Default;
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.Equal(AccessibleRole.MenuItem, accessibleObject.Role);
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_Role_ReturnsExpected_ControlHasNotDefaultRole()
    {
        AccessibleRole testRole = AccessibleRole.Cell;
        using SubToolStripDropDownItem control = new();
        control.AccessibleRole = testRole;
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.Equal(testRole, accessibleObject.Role);
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
    {
        using SubToolStripDropDownItem control = new();
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.True(accessibleObject.IsIAccessibleExSupported());
    }

    [WinFormsTheory]
    [InlineData(false, ((int)ExpandCollapseState.ExpandCollapseState_Collapsed))]
    [InlineData(true, ((int)ExpandCollapseState.ExpandCollapseState_Expanded))]
    public void ToolStripDropDownItemAccessibleObject_ExpandCollapseState_ReturnsExpected(bool visible, int expected)
    {
        using SubToolStripDropDownItem control = new();
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;
        control.DropDown.Items.Add(new SubToolStripDropDownItem());
        control.DropDown.Visible = visible;

        Assert.Equal((ExpandCollapseState)expected, accessibleObject.ExpandCollapseState);
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_GetChildCount_ReturnsExpected_IfNoDropDownItems()
    {
        using SubToolStripDropDownItem control = new();
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.Equal(-1, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_GetChildCount_ReturnsExpected_IfCollapsed()
    {
        using SubToolStripDropDownItem control = new();
        control.DropDown.Items.Add(new SubToolStripDropDownItem());
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.Equal(0, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_GetChildCount_ReturnsExpected()
    {
        int testCount = 2;
        using SubToolStripDropDownItem control = new();
        for (int i = 0; i < testCount; i++)
        {
            control.DropDown.Items.Add(new SubToolStripDropDownItem() { Available = true });
        }

        control.DropDown.Visible = true;
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.Equal(testCount, accessibleObject.GetChildCount());
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_GetChildCount_ReturnsNull_IfNoDropDownItems()
    {
        using SubToolStripDropDownItem control = new();
        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.Null(accessibleObject.GetChild(0));
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_GetChild_ReturnsExpected()
    {
        using NoAssertContext context = new();
        using SubToolStripDropDownItem control = new();
        using SubToolStripDropDownItem item1 = new() { Available = true };
        using SubToolStripDropDownItem item2 = new() { Available = true };
        control.DropDown.Items.AddRange(new[] { item1, item2 });

        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Equal(item1.AccessibilityObject, accessibleObject.GetChild(0));
        Assert.Equal(item2.AccessibilityObject, accessibleObject.GetChild(1));
        Assert.Null(accessibleObject.GetChild(2));
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected()
    {
        using NoAssertContext context = new();
        using SubToolStripDropDownItem control = new();
        using SubToolStripDropDownItem item1 = new();
        using SubToolStripDropDownItem item2 = new();
        using SubToolStripDropDownItem notItem = new();
        control.DropDown.Items.AddRange(new[] { item1, item2 });

        var accessibleObjectItem1 = (ToolStripDropDownItemAccessibleObject)item1.AccessibilityObject;
        var accessibleObjectItem2 = (ToolStripDropDownItemAccessibleObject)item2.AccessibilityObject;
        var accessibleObjectNotItem = (ToolStripDropDownItemAccessibleObject)notItem.AccessibilityObject;

        Assert.Equal(accessibleObjectItem2, accessibleObjectItem1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObjectItem2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObjectNotItem.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected()
    {
        using NoAssertContext context = new();
        using SubToolStripDropDownItem control = new();
        using SubToolStripDropDownItem item1 = new();
        using SubToolStripDropDownItem item2 = new();
        using SubToolStripDropDownItem notItem = new();
        control.DropDown.Items.AddRange(new[] { item1, item2 });

        var accessibleObjectItem1 = (ToolStripDropDownItemAccessibleObject)item1.AccessibilityObject;
        var accessibleObjectItem2 = (ToolStripDropDownItemAccessibleObject)item2.AccessibilityObject;
        var accessibleObjectNotItem = (ToolStripDropDownItemAccessibleObject)notItem.AccessibilityObject;

        Assert.Null(accessibleObjectItem1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObjectItem1, accessibleObjectItem2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObjectNotItem.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_GetChildFragmentCount_ReturnsExpected()
    {
        int testCount = 2;
        using SubToolStripDropDownItem control = new();
        for (int i = 0; i < testCount; i++)
        {
            control.DropDownItems.Add(new SubToolStripDropDownItem() { Available = true });
        }

        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;
        accessibleObject.GetChildCount();

        Assert.Equal(testCount, accessibleObject.GetChildFragmentCount());
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_GetChildFragmentIndex_ReturnsExpected()
    {
        using NoAssertContext context = new();
        using SubToolStripDropDownItem control = new();

        using SubToolStripDropDownItem item1 = new() { Available = true };
        using SubToolStripDropDownItem item2 = new() { Available = true };
        using SubToolStripDropDownItem notItem = new();
        control.DropDownItems.AddRange(new[] { item1, item2 });

        var accessibleObject = (ToolStripDropDownItemAccessibleObject)control.AccessibilityObject;
        var accessibleObjectItem1 = (ToolStripDropDownItemAccessibleObject)item1.AccessibilityObject;
        var accessibleObjectItem2 = (ToolStripDropDownItemAccessibleObject)item2.AccessibilityObject;
        var accessibleObjectNotItem = (ToolStripDropDownItemAccessibleObject)notItem.AccessibilityObject;

        Assert.Equal(0, accessibleObject.GetChildFragmentIndex(accessibleObjectItem1));
        Assert.Equal(1, accessibleObject.GetChildFragmentIndex(accessibleObjectItem2));
        Assert.Equal(-1, accessibleObject.GetChildFragmentIndex(accessibleObjectNotItem));
    }

    [WinFormsFact]
    public void ToolStripDropDownItemAccessibleObject_ReleaseUiaProvider_DropDown()
    {
        using NoAssertContext context = new();
        using SubToolStripDropDownItem toolStripDropDownItem = new();

        _ = toolStripDropDownItem.AccessibilityObject;
        _ = toolStripDropDownItem.DropDown.AccessibilityObject;

        Assert.True(toolStripDropDownItem.DropDown.IsAccessibilityObjectCreated);

        toolStripDropDownItem.ReleaseUiaProvider();

        Assert.False(toolStripDropDownItem.DropDown.IsAccessibilityObjectCreated);
    }

    private class SubToolStripDropDownItem : ToolStripDropDownItem
    {
        public SubToolStripDropDownItem() : base() { }
    }
}
