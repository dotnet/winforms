// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TreeView;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TreeViewAccessibleObjectTests
{
    [WinFormsFact]
    public void TreeViewAccessibleObject_Ctor_Default()
    {
        using TreeView control = new();
        control.CreateControl();

        Assert.NotNull(control.AccessibilityObject);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_Ctor_ThrowsException_IfOwnerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TreeViewAccessibleObject(null));
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_ControlType_IsTree_IfAccessibleRoleIsDefault()
    {
        using TreeView control = new();

        // AccessibleRole is not set = Default
        var actual = (UIA_CONTROLTYPE_ID)(int)control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_TreeControlTypeId, actual);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_Role_IsOutline_ByDefault()
    {
        using TreeView control = new();
        control.CreateControl();

        // AccessibleRole is not set = Default
        AccessibleRole actual = control.AccessibilityObject.Role;

        Assert.Equal(AccessibleRole.Outline, actual);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using TreeView control = new();

        AccessibleObject accessibleObject = control.AccessibilityObject;

        Assert.Equal(accessibleObject, accessibleObject.FragmentRoot);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
    {
        using TreeView control = new();
        control.Nodes.AddRange([new("Node 1"), new("Node 2"), new("Node 3")]);

        AccessibleObject accessibleObject = control.AccessibilityObject;
        AccessibleObject expected = control.Nodes[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
    {
        using TreeView control = new();
        control.Nodes.AddRange([new("Node 1"), new("Node 2"), new("Node 3")]);

        AccessibleObject accessibleObject = control.AccessibilityObject;
        AccessibleObject expected = control.Nodes[^1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(10)]
    public void TreeViewAccessibleObject_FragmentNavigate_GetChild_ReturnsExpected(int index)
    {
        using TreeView control = new();
        control.Nodes.AddRange([new("Node 1"), new("Node 2"), new("Node 3")]);

        AccessibleObject expected = index >= 0 && index < control.Nodes.Count
            ? control.Nodes[index].AccessibilityObject
            : null;

        Assert.Equal(expected, control.AccessibilityObject.GetChild(index));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_FragmentNavigate_GetChildCount_ReturnsExpected()
    {
        using TreeView control = new();
        control.Nodes.AddRange([new("Node 1"), new("Node 2"), new("Node 3")]);

        int expected = 3;

        Assert.Equal(expected, control.AccessibilityObject.GetChildCount());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void TreeViewAccessibleObject_GetPropertyValue_IsEnabled_ReturnsExpected(bool isEnabled)
    {
        using TreeView control = new() { Enabled = isEnabled };

        Assert.Equal(isEnabled, (bool)control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsTrue_IfNoItems()
    {
        using TreeView control = new();

        Assert.True(control.Enabled);
        Assert.True((bool)control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsFalse_IfIsDisabled()
    {
        using TreeView control = new() { Enabled = false };

        Assert.False(control.Enabled);
        Assert.False((bool)control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_GetPropertyValue_HasKeyboardFocus_ReturnsFalse_IfContainsItems()
    {
        using TreeView control = new();
        control.Nodes.Add("Node 1");

        Assert.True(control.Enabled);
        Assert.False((bool)control.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData((int)UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)]
    [InlineData((int)UIA_PATTERN_ID.UIA_SelectionPatternId)]
    public void TreeViewAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
    {
        using TreeView control = new();

        Assert.True(control.AccessibilityObject.IsPatternSupported((UIA_PATTERN_ID)patternId));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_IsSelectionRequired_ReturnsTrue_IfControlHasItem()
    {
        using TreeView control = new();
        control.Nodes.Add("Item1");

        Assert.True(control.AccessibilityObject.IsSelectionRequired);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_IsSelectionRequired_ReturnsFalse_IfControlHasNoItem()
    {
        using TreeView control = new();

        Assert.False(control.AccessibilityObject.IsSelectionRequired);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_GetSelection_ReturnsEmptyArray_IfControlIsNotCreated()
    {
        using TreeView control = new();

        Assert.Empty(control.AccessibilityObject.GetSelection());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_GetSelection_ReturnsEmptyArray_IfNoSelectedNodes()
    {
        using TreeView control = new();
        control.CreateControl();

        Assert.Empty(control.AccessibilityObject.GetSelection());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewAccessibleObject_GetSelection_ReturnsExpected()
    {
        using TreeView control = new();
        control.CreateControl();
        control.Nodes.AddRange([new("Node 1"), new("Node 2"), new("Node 3")]);
        control.SelectedNode = control.Nodes[1];

        IRawElementProviderSimple.Interface[] expected = new[] { control.Nodes[1].AccessibilityObject };

        Assert.Equal(expected, control.AccessibilityObject.GetSelection());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNodeAccessibleObject_HitTest_ReturnsNull_IfControlDoesNotCreated()
    {
        using TreeView control = new() { Size = new Size(300, 100) };
        TreeNode node = new("First node.");
        control.Nodes.Add(node);

        AccessibleObject accessibleObject = control.AccessibilityObject;
        Point point = accessibleObject.Bounds.Location;

        Assert.Null(accessibleObject.HitTest(point.X, point.Y));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNodeAccessibleObject_HitTest_ReturnsTree()
    {
        using TreeView control = new() { Size = new Size(300, 100) };
        TreeNode node = new("First node.");
        control.Nodes.Add(node);
        control.CreateControl();

        AccessibleObject accessibleObject = control.AccessibilityObject;
        Point point = new(accessibleObject.Bounds.X, accessibleObject.Bounds.Bottom - 1);

        Assert.Equal(accessibleObject, accessibleObject.HitTest(point.X, point.Y));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeNodeAccessibleObject_HitTest_ReturnsNode()
    {
        using TreeView control = new() { Size = new Size(300, 100) };
        TreeNode node = new("First node.");
        control.Nodes.Add(node);
        control.CreateControl();

        Point point = node.AccessibilityObject.Bounds.Location;

        Assert.Equal(node.AccessibilityObject, control.AccessibilityObject.HitTest(point.X, point.Y));
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(new string[] { "Node 1", "Node 2", "Node 3" }, 3, false)]
    [InlineData(new string[] { }, 0, false)]
    [InlineData(new string[] { "Node 1", "Node 2", "Node 3" }, 3, true)]
    [InlineData(new string[] { }, 0, true)]
    public void TreeViewAccessibleObject_GetChildCount_ReturnsExpected(string[] nodeNames, int expected, bool isHandleCreated)
    {
        using TreeView control = new();
        control.Nodes.AddRange(nodeNames.Select(name => new TreeNode(name)).ToArray());

        if (isHandleCreated)
        {
            control.CreateControl();
        }

        control.AccessibilityObject.GetChildCount().Should().Be(expected);
        control.IsHandleCreated.Should().Be(isHandleCreated);
    }
}
