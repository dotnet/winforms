// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TreeViewLabelEditAccessibleObjectTests
{
    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(accessibilityObject.RuntimeId, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId));
        PInvoke.GetWindowRect(labelEdit, out RECT r);
        using SafeArrayScope<double> rectArray = UiaTextProvider.BoundingRectangleAsArray((Rectangle)r);
        Assert.Equal(((VARIANT)rectArray).ToObject(), accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId));
        Assert.Equal(Environment.ProcessId, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ProcessIdPropertyId));
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_EditControlTypeId, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
        Assert.Equal(accessibilityObject.Name, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId));
        Assert.Empty((string)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AccessKeyPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(treeView.Enabled, (bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId), accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId));
        Assert.Empty((string)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsContentElementPropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId));
        Assert.Equal(labelEdit.Handle, accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.True(treeView.IsHandleCreated);
        Assert.True(labelEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_FragmentNavigate_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.Nodes[0].AccessibilityObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        Assert.NotNull(accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_IsPatternSupported_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TextPatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TextPattern2Id));
        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_RuntimeId_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(new int[] { AccessibleObject.RuntimeIDFirstItem, PARAM.ToInt(labelEdit.Handle) }, accessibilityObject.RuntimeId);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.AccessibilityObject, accessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_HostRawElementProvider_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.NotNull(accessibilityObject.HostRawElementProvider);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_Name_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.Nodes[0].Text, accessibilityObject.Name);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new TreeViewLabelEditAccessibleObject(null, labelEdit));
    }

    [WinFormsFact]
    public void TreeViewLabelEditNativeWindow_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TreeViewLabelEditNativeWindow(null));
    }

    [WinFormsFact]
    public void TreeViewLabelEditUiaTextProvider_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(null, labelEdit, labelEdit.AccessibilityObject));
    }

    [WinFormsFact]
    public void TreeViewLabelEditUiaTextProvider_Ctor_NullChildEditAccessibilityObject_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(treeView, labelEdit, null));
    }

    private TreeView CreateTreeViewAndStartEditing()
    {
        TreeView treeView = new()
        {
            Size = new Size(300, 200),
            LabelEdit = true
        };
        TreeNode node = new("node1");
        treeView.Nodes.Add(node);
        treeView.CreateControl();
        node.BeginEdit();
        return treeView;
    }
}
