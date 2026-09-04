// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class TreeViewLabelEditAccessibleObjectTests
{
    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;
        using VARIANT runtimeId = accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);
        Assert.Equal(accessibilityObject.RuntimeId, runtimeId.ToObject());
        PInvokeCore.GetWindowRect(labelEdit, out RECT r);
        using VARIANT rectArrayVariant = accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId);
        double[] actualArray = (double[])rectArrayVariant.ToObject();
        Rectangle actualRectangle = new((int)actualArray[0], (int)actualArray[1], (int)actualArray[2], (int)actualArray[3]);
        Assert.Equal((Rectangle)r, actualRectangle);
        Assert.Equal(Environment.ProcessId, (int)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ProcessIdPropertyId));
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_EditControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
        Assert.Equal(accessibilityObject.Name, ((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Empty(((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AccessKeyPropertyId)).ToStringAndFree());
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(treeView.Enabled, (bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal(((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree(), ((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree());
        Assert.Empty(((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId)).ToStringAndFree());
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsContentElementPropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId));
        Assert.Equal((int)labelEdit.Handle, (int)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId));
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

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.Nodes[0].AccessibilityObject, accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.NotNull(accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_IsPatternSupported_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
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

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(new int[] { AccessibleObject.RuntimeIDFirstItem, PARAM.ToInt(labelEdit.Handle) }, accessibilityObject.RuntimeId);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.AccessibilityObject, accessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public unsafe void TreeViewLabelEditAccessibleObject_HostRawElementProvider_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;
        using ComScope<IRawElementProviderSimple> provider = new(accessibilityObject.HostRawElementProvider);
        Assert.False(provider.IsNull);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_Name_ReturnsExpected()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        var accessibilityObject = (TreeViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(treeView.Nodes[0].Text, accessibilityObject.Name);
    }

    [WinFormsFact]
    public void TreeViewLabelEditAccessibleObject_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new TreeViewLabelEditAccessibleObject(null, labelEdit));
    }

    [WinFormsFact]
    public void TreeViewLabelEditNativeWindow_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TreeViewLabelEditNativeWindow(null));
    }

    [WinFormsFact]
    public void LabelEditNativeWindow_OnHandleChange_DuringRelease_DoesNotReinstallHooks()
    {
        using TreeView treeView = new() { Size = new Size(300, 200) };
        treeView.CreateControl();

        TreeViewLabelEditNativeWindow labelEdit = new(treeView);
        labelEdit.AssignHandle(treeView.Handle);
        _ = treeView.AccessibilityObject;

        Assert.True(labelEdit.IsHandleCreated);

        labelEdit.TestAccessor.Dynamic._isReleasing = true;
        labelEdit.TestAccessor.Dynamic.OnHandleChange();

        Assert.False((bool)labelEdit.TestAccessor.Dynamic._winEventHooksInstalled);
    }

    [WinFormsFact]
    public void LabelEditNativeWindow_OnHandleChange_AfterHandleDestroyed_DoesNotReinstallHooks()
    {
        using TreeView treeView = new() { Size = new Size(300, 200) };
        treeView.CreateControl();

        TreeViewLabelEditNativeWindow labelEdit = new(treeView);
        labelEdit.AssignHandle(treeView.Handle);

        // Simulate installed hooks.
        labelEdit.TestAccessor.Dynamic._winEventHooksInstalled = true;

        // Simulate handle already destroyed.
        labelEdit.ReleaseHandle();

        labelEdit.TestAccessor.Dynamic._isReleasing = false;
        labelEdit.TestAccessor.Dynamic.OnHandleChange();

        Assert.False((bool)labelEdit.TestAccessor.Dynamic._winEventHooksInstalled);
        Assert.Null(labelEdit.TestAccessor.Dynamic._winEventProcCallback);
    }

    [WinFormsFact]
    public void TreeViewLabelEditNativeWindow_ReleaseHandle_UnhooksAndCleansUpState()
    {
        using TreeView treeView = new() { Size = new Size(300, 200) };
        treeView.CreateControl();

        TreeViewLabelEditNativeWindow labelEdit = new(treeView);
        labelEdit.AssignHandle(treeView.Handle);

        // Simulate hooks installed
        labelEdit.TestAccessor.Dynamic._winEventHooksInstalled = true;

        labelEdit.ReleaseHandle();

        Assert.False((bool)labelEdit.TestAccessor.Dynamic._winEventHooksInstalled);
        Assert.Null(labelEdit.TestAccessor.Dynamic._winEventProcCallback);
    }

    [WinFormsFact]
    public void TreeViewLabelEditUiaTextProvider_Ctor_NullOwningTreeView_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(null, labelEdit, labelEdit.AccessibilityObject));
    }

    [WinFormsFact]
    public void TreeViewLabelEditUiaTextProvider_Ctor_NullChildEditAccessibilityObject_ThrowsArgumentNullException()
    {
        using TreeView treeView = CreateTreeViewAndStartEditing();

        TreeViewLabelEditNativeWindow labelEdit = treeView.TestAccessor.Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(treeView, labelEdit, null));
    }

    [WinFormsFact]
    public void LabelEditNativeWindow_RepeatedDestroyRecreate_WithGCBetweenCycles_HooksCleanAfterEachRelease()
    {
        using TreeView treeView = new() { Size = new Size(300, 200) };
        treeView.CreateControl();
        _ = treeView.AccessibilityObject;

        TreeViewLabelEditNativeWindow labelEdit = new(treeView);

        for (int cycle = 0; cycle < 3; cycle++)
        {
            labelEdit.AssignHandle(treeView.Handle);
            Assert.True(labelEdit.IsHandleCreated);
            Assert.False((bool)labelEdit.TestAccessor.Dynamic._isReleasing);

            // Simulate hooks installed (UiaClientsAreListening() returns false in tests,
            // so we set the flag manually to replicate the real-world state).
            labelEdit.TestAccessor.Dynamic._winEventHooksInstalled = true;

            // Force GC to simulate the delegate becoming eligible for collection
            // while the hooks are still logically registered.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            labelEdit.ReleaseHandle();

            // After release both cleanup obligations must be fulfilled:
            // hooks unregistered (_winEventHooksInstalled=false) and delegate reference
            // dropped (_winEventProcCallback=null) so GC can collect it safely.
            Assert.False((bool)labelEdit.TestAccessor.Dynamic._winEventHooksInstalled);
            Assert.Null(labelEdit.TestAccessor.Dynamic._winEventProcCallback);
            Assert.False((bool)labelEdit.TestAccessor.Dynamic._isReleasing);
            Assert.False(labelEdit.IsHandleCreated);

            // Force GC again to ensure no outstanding finalizers from this cycle
            // could corrupt the next cycle.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    [WinFormsFact]
    public void LabelEditNativeWindow_LateWinEventCallback_AfterHandleReleased_IsSafelyIgnored()
    {
        using TreeView treeView = new() { Size = new Size(300, 200) };
        treeView.CreateControl();

        TreeViewLabelEditNativeWindow labelEdit = new(treeView);
        labelEdit.AssignHandle(treeView.Handle);

        // Simulate hooks installed.
        labelEdit.TestAccessor.Dynamic._winEventHooksInstalled = true;

        // Release the handle; this must uninstall hooks and null the callback.
        labelEdit.ReleaseHandle();
        Assert.False((bool)labelEdit.TestAccessor.Dynamic._winEventHooksInstalled);

        // Invoke the callback directly as if the OS delivered a late event after teardown.
        // The _winEventHooksInstalled == false guard must make this a no-op without throwing.
        labelEdit.TestAccessor.Dynamic.WinEventProcCallback(
            default(HWINEVENTHOOK),
            (uint)AccessibleEvents.ValueChange,
            (HWND)treeView.Handle,
            (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
            0,
            0u,
            0u);

        // State must remain consistent: no hooks, no callback reference.
        Assert.False((bool)labelEdit.TestAccessor.Dynamic._winEventHooksInstalled);
        Assert.Null(labelEdit.TestAccessor.Dynamic._winEventProcCallback);
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
