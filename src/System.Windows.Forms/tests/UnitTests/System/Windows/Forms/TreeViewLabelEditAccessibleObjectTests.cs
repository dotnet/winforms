// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using static Interop;

namespace System.Windows.Forms.Tests;

public class TreeViewLabelEditAccessibleObjectTests
{
    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        TreeViewLabelEditAccessibleObject accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(accessibilityObject.RuntimeId, accessibilityObject.GetPropertyValue(UiaCore.UIA.RuntimeIdPropertyId));
        PInvoke.GetWindowRect(labelEdit, out RECT r);
        using SafeArrayScope<double> rectArray = UiaTextProvider.BoundingRectangleAsArray((Rectangle)r);
        Assert.Equal(((VARIANT)rectArray).ToObject(), accessibilityObject.GetPropertyValue(UiaCore.UIA.BoundingRectanglePropertyId));
        Assert.Equal(Environment.ProcessId, accessibilityObject.GetPropertyValue(UiaCore.UIA.ProcessIdPropertyId));
        Assert.Equal(UiaCore.UIA.EditControlTypeId, accessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        Assert.Equal(accessibilityObject.Name, accessibilityObject.GetPropertyValue(UiaCore.UIA.NamePropertyId));
        Assert.Empty((string)accessibilityObject.GetPropertyValue(UiaCore.UIA.AccessKeyPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.HasKeyboardFocusPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsKeyboardFocusablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
        Assert.Equal(treeView.Enabled, (bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
        Assert.Equal(accessibilityObject.GetPropertyValue(UiaCore.UIA.NamePropertyId), accessibilityObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId));
        Assert.Empty((string)accessibilityObject.GetPropertyValue(UiaCore.UIA.HelpTextPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsContentElementPropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsPasswordPropertyId));
        Assert.Equal(labelEdit.Handle, accessibilityObject.GetPropertyValue(UiaCore.UIA.NativeWindowHandlePropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsOffscreenPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsTextPatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsTextPattern2AvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsValuePatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.True(treeView.IsHandleCreated);
        Assert.True(labelEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_FragmentNavigate_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        TreeViewLabelEditAccessibleObject accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.Nodes[0].AccessibilityObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        Assert.NotNull(accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_IsPatternSupported_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        TreeViewLabelEditAccessibleObject accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.TextPatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.TextPattern2Id));
        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.ValuePatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId));
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_RuntimeId_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        TreeViewLabelEditAccessibleObject accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(new int[] { AccessibleObject.RuntimeIDFirstItem, PARAM.ToInt(labelEdit.Handle) }, accessibilityObject.RuntimeId);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        TreeViewLabelEditAccessibleObject accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.AccessibilityObject, accessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_HostRawElementProvider_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        TreeViewLabelEditAccessibleObject accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.NotNull(accessibilityObject.HostRawElementProvider);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_Name_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;
        TreeViewLabelEditAccessibleObject accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.Nodes[0].Text, accessibilityObject.Name);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();
        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;

        Assert.Throws<ArgumentNullException>(() => new TreeViewLabelEditAccessibleObject(null, labelEdit));
    }

    [WinFormsFact]
    public void TreeViewLabelEditNativeWindow_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LabelEditNativeWindow(null));
    }

    [WinFormsFact]
    public void TreeViewLabelEditUiaTextProvider_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();
        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;

        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(null, labelEdit, labelEdit.AccessibilityObject));
    }

    [WinFormsFact]
    public void TreeViewLabelEditUiaTextProvider_Ctor_NullChildEditAccessibilityObject_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();
        LabelEditNativeWindow labelEdit = treeView.TestAccessor().Dynamic._labelEdit;

        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(treeView, labelEdit, null));
    }

    private TreeView CreateTreeViewAndStartEditing()
    {
        TreeView treeView = new ()
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
