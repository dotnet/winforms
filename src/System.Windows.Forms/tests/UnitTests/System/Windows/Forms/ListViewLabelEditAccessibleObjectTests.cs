// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests;

public class ListViewLabelEditAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
{
    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_GetPropertyValue_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(accessibilityObject.RuntimeId, accessibilityObject.GetPropertyValue(UiaCore.UIA.RuntimeIdPropertyId));
        PInvoke.GetWindowRect(labelEdit, out RECT r);
        Assert.Equal((Rectangle)r, accessibilityObject.GetPropertyValue(UiaCore.UIA.BoundingRectanglePropertyId));
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

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.Equal(listView.AccessibilityObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.Parent));
        Assert.Equal(listView.Items[0].AccessibilityObject, accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
        Assert.Null(accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
        Assert.Null(accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
        Assert.Null(accessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
    }

    [WinFormsFact]
    public void ListViewLabelEditAccessibleObject_IsPatternSupported_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
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
    public void ListViewLabelEditAccessibleObject_HostRawElementProvider_ReturnsExpected()
    {
        using ListView listView = CreateListViewAndStartEditing();

        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;
        ListViewLabelEditAccessibleObject accessibilityObject = (ListViewLabelEditAccessibleObject)labelEdit.AccessibilityObject;

        Assert.NotNull(accessibilityObject.HostRawElementProvider);
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

        Assert.Throws<ArgumentNullException>(() => new ListViewLabelEditUiaTextProvider(null, labelEdit, labelEdit.AccessibilityObject));
    }

    [WinFormsFact]
    public void ListViewLabelEditUiaTextProvider_Ctor_NullChildEditAccessibilityObject_ThrowsArgumentNullException()
    {
        using ListView listView = CreateListViewAndStartEditing();
        ListViewLabelEditNativeWindow labelEdit = listView.TestAccessor().Dynamic._labelEdit;

        Assert.Throws<ArgumentNullException>(() => new ListViewLabelEditUiaTextProvider(listView, labelEdit, null));
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

        ListViewItem item = new("Test");
        listView.Items.Add(item);

        listView.CreateControl();

        PInvoke.SetFocus(listView);

        PInvoke.SendMessage(listView, (User32.WM)PInvoke.LVM_EDITLABELW, wParam: 0);

        return listView;
    }

    private class SubListView : ListView
    {
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // End the label edit because ListView cannot be correctly disposed with an active label edit when AccessibilityObject is created for the ListView
                PInvoke.SendMessage(this, (User32.WM)PInvoke.LVM_CANCELEDITLABEL);
            }

            base.Dispose(disposing);
        }
    }
}
