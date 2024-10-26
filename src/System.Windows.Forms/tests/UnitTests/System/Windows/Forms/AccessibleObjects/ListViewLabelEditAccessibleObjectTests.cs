// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewLabelEditAccessibleObjectTests
{
    [WinFormsFact]
    public unsafe void ListViewLabelEditAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;
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
        Assert.Equal(listView.Enabled, (bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsEnabledPropertyId));
        Assert.Equal("1", ((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree());
        Assert.Empty(((BSTR)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_HelpTextPropertyId)).ToStringAndFree());
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsContentElementPropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsPasswordPropertyId));
        Assert.Equal((int)labelEdit.Handle, (int)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.True(listView.IsHandleCreated);
        Assert.True(labelEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_FragmentNavigate_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(listView._listViewSubItem.AccessibilityObject, accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.NotNull(accessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_IsPatternSupported_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TextPatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TextPattern2Id));
        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId));
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_RuntimeId_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(new int[] { AccessibleObject.RuntimeIDFirstItem, PARAM.ToInt(labelEdit.Handle) }, accessibilityObject.RuntimeId);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(listView.AccessibilityObject, accessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public unsafe void ListViewLabelEditAccessibleObject_HostRawElementProvider_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;
        using ComScope<IRawElementProviderSimple> elementProvider = new(accessibilityObject.HostRawElementProvider);
        Assert.False(elementProvider.IsNull);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_Name_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(listView.Items[0].Text, accessibilityObject.Name);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_Ctor_NullOwningListView_ThrowsArgumentNullException()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new ListViewLabelEditAccessibleObject(null, labelEdit));
    }

    [WinFormsFact]
    public void ListViewLabelEditNativeWindow_Ctor_NullOwningListView_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ListViewLabelEditNativeWindow(null));
    }

    [WinFormsFact]
    public void ListViewLabelEditUiaTextProvider_Ctor_NullOwningListView_ThrowsArgumentNullException()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(null, labelEdit, labelEdit.AccessibilityObject));
    }

    [WinFormsFact]
    public void ListViewLabelEditUiaTextProvider_Ctor_NullChildEditAccessibilityObject_ThrowsArgumentNullException()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(listView, labelEdit, null));
    }

    private ListView CreateListViewAndStartEditing()
    {
        SubListView listView = new()
        {
            Size = new Size(300, 200),
            View = View.Details,
            LabelEdit = true
        };

        listView.Columns.Add(new ColumnHeader() { Text = "Column 1", Width = 100 });

        ListViewItem item = new("Test", 0);
        ListViewSubItem subItem = new(item, "Test");
        item.SubItems.Add(subItem);
        listView.Items.Add(item);
        listView._listViewSubItem = subItem;
        listView.CreateControl();

        PInvoke.SetFocus(listView);

        PInvokeCore.SendMessage(listView, PInvoke.LVM_EDITLABELW, wParam: 0);

        return listView;
    }

    private class SubListView : ListView
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // End the label edit because ListView cannot be correctly disposed with an active label edit when
                // AccessibilityObject is created for the ListView
                PInvokeCore.SendMessage(this, PInvoke.LVM_CANCELEDITLABEL);
            }

            base.Dispose(disposing);
        }
    }
}
