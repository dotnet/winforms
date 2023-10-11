// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;
using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms.Tests;

public class ListViewLabelEditAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

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
        Assert.Equal(listView.Enabled, (bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsEnabledPropertyId));
        Assert.Equal("1", accessibilityObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId));
        Assert.Empty((string)accessibilityObject.GetPropertyValue(UiaCore.UIA.HelpTextPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsContentElementPropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsPasswordPropertyId));
        Assert.Equal(labelEdit.Handle, accessibilityObject.GetPropertyValue(UiaCore.UIA.NativeWindowHandlePropertyId));
        Assert.False((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsOffscreenPropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsTextPatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsTextPattern2AvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsValuePatternAvailablePropertyId));
        Assert.True((bool)accessibilityObject.GetPropertyValue(UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.True(listView.IsHandleCreated);
        Assert.True(labelEdit.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_FragmentNavigate_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(listView._listViewSubItem.AccessibilityObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        Assert.NotNull(accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_IsPatternSupported_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.TextPatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.TextPattern2Id));
        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.ValuePatternId));
        Assert.True(accessibilityObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId));
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_RuntimeId_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(new int[] { AccessibleObject.RuntimeIDFirstItem, PARAM.ToInt(labelEdit.Handle) }, accessibilityObject.RuntimeId);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_FragmentRoot_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(listView.AccessibilityObject, accessibilityObject.FragmentRoot);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_HostRawElementProvider_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.NotNull(accessibilityObject.HostRawElementProvider);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_Name_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(listView.Items[0].Text, accessibilityObject.Name);
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_Ctor_NullOwningListView_ThrowsArgumentNullException()
    {
        using ListView listView = CreateListViewAndStartEditing();
        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;

        Assert.Throws<ArgumentNullException>(() => new ListViewLabelEditAccessibleObject(null, labelEdit));
    }

    [WinFormsFact]
    public void ListViewLabelEditNativeWindow_Ctor_NullOwningListView_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LabelEditNativeWindow(null));
    }

    [WinFormsFact]
    public void ListViewLabelEditUiaTextProvider_Ctor_NullOwningListView_ThrowsArgumentNullException()
    {
        using ListView listView = CreateListViewAndStartEditing();
        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;

        Assert.Throws<ArgumentNullException>(() => new LabelEditUiaTextProvider(null, labelEdit, labelEdit.AccessibilityObject));
    }

    [WinFormsFact]
    public void ListViewLabelEditUiaTextProvider_Ctor_NullChildEditAccessibilityObject_ThrowsArgumentNullException()
    {
        using ListView listView = CreateListViewAndStartEditing();
        LabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;

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

        ListViewItem item = new("Test",0);
        ListViewSubItem subItem = new ListViewSubItem(item, "Test");
        item.SubItems.Add(subItem);
        listView.Items.Add(item);
        listView._listViewSubItem = subItem;
        listView.CreateControl();

        PInvoke.SetFocus(listView);

        PInvoke.SendMessage(listView, PInvoke.LVM_EDITLABELW, wParam: 0);

        return listView;
    }

    private class SubListView : ListView
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // End the label edit because ListView cannot be correctly disposed with an active label edit when AccessibilityObject is created for the ListView
                PInvoke.SendMessage(this, PInvoke.LVM_CANCELEDITLABEL);
            }

            base.Dispose(disposing);
        }
    }
}
