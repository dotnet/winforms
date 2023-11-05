// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ColumnHeader;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ColumnHeader_ListViewColumnHeaderAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewColumnHeaderAccessibleObject_Ctor_OwnerColumnHeaderCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ListViewColumnHeaderAccessibleObject(null));
    }

    [WinFormsFact]
    public void ListViewColumnHeaderAccessibleObject_GetPropertyValue_ControlType_ReturnsExpected()
    {
        using ColumnHeader columnHeader = new();

        ListViewColumnHeaderAccessibleObject accessibleObject = new(columnHeader);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_HeaderItemControlTypeId, (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
    }

    [WinFormsFact]
    public void ListViewColumnHeaderAccessibleObject_GetPropertyValue_Name_ReturnsExpected()
    {
        string testText = "This is a simple text for testing.";
        using ColumnHeader columnHeader = new() { Text = testText };

        ListViewColumnHeaderAccessibleObject accessibleObject = new(columnHeader);

        Assert.Equal(testText, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Equal(testText, ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId)).ToStringAndFree());
        Assert.Equal(VARIANT.Empty, accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId));
    }

    [WinFormsFact]
    public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenListViewReleasesUiaProvider()
    {
        using ListView listView = new();
        using ColumnHeader columnHeader = new();
        listView.Columns.Add(columnHeader);
        EnforceAccessibleObjectCreation(columnHeader);
        _ = listView.AccessibilityObject;

        listView.ReleaseUiaProvider(listView.HWND);

        Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenListViewIsCleared()
    {
        using ListView listView = new();
        using ColumnHeader columnHeader = new();
        listView.Columns.Add(columnHeader);
        EnforceAccessibleObjectCreation(columnHeader);

        listView.Clear();

        Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenColumnsAreCleared()
    {
        using ListView listView = new();
        using ColumnHeader columnHeader = new();
        listView.Columns.Add(columnHeader);
        EnforceAccessibleObjectCreation(columnHeader);

        listView.Columns.Clear();

        Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewColumnHeaderAccessibleObject_IsDisconnected_WhenColumnIsRemoved()
    {
        using ListView listView = new();
        using ColumnHeader columnHeader = new();
        listView.Columns.Add(columnHeader);
        EnforceAccessibleObjectCreation(columnHeader);

        listView.Columns.Remove(columnHeader);

        Assert.Null(columnHeader.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    private static void EnforceAccessibleObjectCreation(ColumnHeader columnHeader)
    {
        _ = columnHeader.AccessibilityObject;
        Assert.NotNull(columnHeader.TestAccessor().Dynamic._accessibilityObject);
    }
}
