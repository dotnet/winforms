// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem.ListViewSubItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewItem_ListViewSubItem_ListViewSubItemAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_GetChild_ReturnCorrectValue()
    {
        using ListView list = new();
        ListViewItem listViewItem1 = new(
        [
            "Test 1",
            "Item 1",
            "Something 1"
        ], -1);

        ColumnHeader columnHeader1 = new();
        ColumnHeader columnHeader2 = new();
        ColumnHeader columnHeader3 = new();

        list.Columns.AddRange(
        [
        columnHeader1,
        columnHeader2,
        columnHeader3
        ]);
        list.HideSelection = false;
        list.Items.Add(listViewItem1);
        list.View = View.Details;

        list.CreateControl();

        AccessibleObject accessibleObject = listViewItem1.AccessibilityObject.GetChild(0);
        Assert.NotNull(accessibleObject);
        Assert.IsType<ListViewSubItemAccessibleObject>(accessibleObject);
        Assert.True(list.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, 0)]
    [InlineData(true, 1)]
    [InlineData(false, 1)]
    public void ListViewSubItemAccessibleObject_GetPropertyValue_returns_correct_values(bool labelEdit, int childId)
    {
        using ListView list = new();
        ListViewItem listViewItem1 = new(
        [
        "Test 1",
        "Test 2",
        "Something 1"
        ], -1);

        ColumnHeader columnHeader1 = new();
        ColumnHeader columnHeader2 = new();
        ColumnHeader columnHeader3 = new();

        list.Columns.AddRange(
        [
        columnHeader1,
        columnHeader2,
        columnHeader3
        ]);
        list.HideSelection = false;
        list.Items.Add(listViewItem1);
        list.View = View.Details;
        list.LabelEdit = labelEdit;

        list.CreateControl();

        AccessibleObject accessibleObject = listViewItem1.AccessibilityObject.GetChild(childId);

        string accessibleName = ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree();
        Assert.Equal($"Test {childId + 1}", accessibleName);

        string automationId = ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree();
        Assert.Equal($"ListViewSubItem-{childId}", automationId);

        string frameworkId = ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_FrameworkIdPropertyId)).ToStringAndFree();
        Assert.Equal("WinForm", frameworkId);

        var controlType = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_TextControlTypeId, controlType);
        Assert.True(list.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewSubItemAccessibleObject_GetPropertyValue_GridTablePattern_ReturnsExpected(View view)
    {
        using ListView list = new() { View = view };
        list.Items.Add(new ListViewItem(["Test 1", "Test 2"]));

        AccessibleObject accessibleObject = list.Items[0].SubItems[1].AccessibilityObject;

        Assert.Equal(view == View.Details, (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId));
        Assert.Equal(view == View.Details, (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId));
    }

    public static IEnumerable<object[]> ListViewSubItemAccessibleObject_FragmentNavigate_TestData()
    {
        foreach (View view in new[] { View.List, View.LargeIcon, View.SmallIcon, View.Tile })
        {
            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool virtualMode in new[] { true, false })
                {
                    // View.Tile is not supported by ListView in virtual mode
                    if (view == View.Tile && virtualMode)
                    {
                        continue;
                    }

                    foreach (bool showGroups in new[] { true, false })
                    {
                        yield return new object[] { view, createControl, virtualMode, showGroups };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_ReturnsNull_List_Icon_Tile_View(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(view, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 2);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj1 = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObj2 = listViewItem.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObj3 = listViewItem.SubItems[2].AccessibilityObject;

        AccessibleObject subItemAccObjInGroup1 = listViewItemInGroup.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup2 = listViewItemInGroup.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup3 = listViewItemInGroup.SubItems[2].AccessibilityObject;

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_TestData))]
    public void ListViewSubItemAccessibleObject_Bounds_ReturnsEmptyRectangle_List_Icon_Tile_View(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(view, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 2);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj1 = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObj2 = listViewItem.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObj3 = listViewItem.SubItems[2].AccessibilityObject;

        AccessibleObject subItemAccObjInGroup1 = listViewItemInGroup.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup2 = listViewItemInGroup.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup3 = listViewItemInGroup.SubItems[2].AccessibilityObject;

        Assert.Equal(Rectangle.Empty, subItemAccObj1.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObj2.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObj3.Bounds);

        Assert.Equal(Rectangle.Empty, subItemAccObjInGroup1.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObjInGroup2.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObjInGroup3.Bounds);

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ListViewSubItemAccessibleObject_Bounds_ReturnsNotEmptyRectangle_Details_View_IfHandleIsCreated(bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl: true, virtualMode, showGroups, columnCount: 3, subItemCount: 2);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj1 = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObj2 = listViewItem.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObj3 = listViewItem.SubItems[2].AccessibilityObject;

        AccessibleObject subItemAccObjInGroup1 = listViewItemInGroup.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup2 = listViewItemInGroup.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup3 = listViewItemInGroup.SubItems[2].AccessibilityObject;

        Assert.NotEqual(Rectangle.Empty, subItemAccObj1.Bounds);
        Assert.NotEqual(Rectangle.Empty, subItemAccObj2.Bounds);
        Assert.NotEqual(Rectangle.Empty, subItemAccObj3.Bounds);

        Assert.NotEqual(Rectangle.Empty, subItemAccObjInGroup1.Bounds);
        Assert.NotEqual(Rectangle.Empty, subItemAccObjInGroup2.Bounds);
        Assert.NotEqual(Rectangle.Empty, subItemAccObjInGroup3.Bounds);

        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ListViewSubItemAccessibleObject_Bounds_ReturnsNotEmptyRectangle_Details_View_IfHandleIsNotCreated(bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl: false, virtualMode, showGroups, columnCount: 3, subItemCount: 2);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj1 = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObj2 = listViewItem.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObj3 = listViewItem.SubItems[2].AccessibilityObject;

        AccessibleObject subItemAccObjInGroup1 = listViewItemInGroup.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup2 = listViewItemInGroup.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup3 = listViewItemInGroup.SubItems[2].AccessibilityObject;

        Assert.Equal(Rectangle.Empty, subItemAccObj1.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObj2.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObj3.Bounds);

        Assert.Equal(Rectangle.Empty, subItemAccObjInGroup1.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObjInGroup2.Bounds);
        Assert.Equal(Rectangle.Empty, subItemAccObjInGroup3.Bounds);

        Assert.False(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData()
    {
        foreach (bool createControl in new[] { true, false })
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { createControl, virtualMode, showGroups };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_Child_ReturnsNull_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 2);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj1 = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObj2 = listViewItem.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObj3 = listViewItem.SubItems[2].AccessibilityObject;

        AccessibleObject subItemAccObjInGroup1 = listViewItemInGroup.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup2 = listViewItemInGroup.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup3 = listViewItemInGroup.SubItems[2].AccessibilityObject;

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_Child_ReturnsNull_Tile_View(bool createControl, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Tile, createControl, virtualMode: false, showGroups, columnCount: 3, subItemCount: 2);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj1 = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObj2 = listViewItem.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObj3 = listViewItem.SubItems[2].AccessibilityObject;

        AccessibleObject subItemAccObjInGroup1 = listViewItemInGroup.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup2 = listViewItemInGroup.SubItems[1].AccessibilityObject;
        AccessibleObject subItemAccObjInGroup3 = listViewItemInGroup.SubItems[2].AccessibilityObject;

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Null(subItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(subItemAccObjInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_Details_View_TwoColumn(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 2, subItemCount: 5);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccInGroup = listViewItemInGroup.SubItems[0].AccessibilityObject;

        AccessibleObject nextSubItemAccObj1 = (AccessibleObject)subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject nextSubItemAccObj2 = (AccessibleObject)nextSubItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

        AccessibleObject nextSubItemAccInGroup1 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject nextSubItemAccInGroup2 = (AccessibleObject)nextSubItemAccInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

        Assert.Same(listViewItem.SubItems[1].AccessibilityObject, nextSubItemAccObj1);
        Assert.Null(nextSubItemAccObj2);

        Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, nextSubItemAccInGroup1);
        Assert.Null(nextSubItemAccInGroup2);

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_Details_View_SingleSubItem(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 5, subItemCount: 0);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccInGroup = listViewItemInGroup.SubItems[0].AccessibilityObject;

        AccessibleObject nextSubItemAccObj1 = (AccessibleObject)subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject nextSubItemAccObj2 = (AccessibleObject)nextSubItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

        AccessibleObject nextSubItemAccInGroup1 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject nextSubItemAccInGroup2 = (AccessibleObject)nextSubItemAccInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

        Assert.NotNull(nextSubItemAccObj1);
        Assert.NotNull(nextSubItemAccObj2);

        Assert.NotNull(nextSubItemAccInGroup1);
        Assert.NotNull(nextSubItemAccInGroup2);

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsNull_Details_View_SingleSubItem(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 5, subItemCount: 0);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccInGroup = listViewItemInGroup.SubItems[0].AccessibilityObject;
        AccessibleObject previousSubItemAccObj = (AccessibleObject)subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup = (AccessibleObject)subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

        Assert.Null(previousSubItemAccObj);
        Assert.Null(previousSubItemAccInGroup);

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 4, subItemCount: 3);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = listViewItem.SubItems[3].AccessibilityObject;
        AccessibleObject subItemAccInGroup = listViewItemInGroup.SubItems[3].AccessibilityObject;

        AccessibleObject previousSubItemAccObj3 = (AccessibleObject)subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccObj2 = (AccessibleObject)previousSubItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccObj1 = (AccessibleObject)previousSubItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccObj0 = (AccessibleObject)previousSubItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

        AccessibleObject previousSubItemAccInGroup3 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup2 = (AccessibleObject)previousSubItemAccInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup1 = (AccessibleObject)previousSubItemAccInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup0 = (AccessibleObject)previousSubItemAccInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

        Assert.Same(listViewItem.SubItems[2].AccessibilityObject, previousSubItemAccObj3);
        Assert.Same(listViewItem.SubItems[1].AccessibilityObject, previousSubItemAccObj2);
        Assert.Same(listViewItem.SubItems[0].AccessibilityObject, previousSubItemAccObj1);
        Assert.Null(previousSubItemAccInGroup0);

        Assert.Same(listViewItemInGroup.SubItems[2].AccessibilityObject, previousSubItemAccInGroup3);
        Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, previousSubItemAccInGroup2);
        Assert.Same(listViewItemInGroup.SubItems[0].AccessibilityObject, previousSubItemAccInGroup1);
        Assert.Null(previousSubItemAccInGroup0);

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_AfterAddingItem_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 2, subItemCount: 0);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccInGroup = listViewItemInGroup.SubItems[0].AccessibilityObject;

        Assert.NotNull(subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.NotNull(subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        listViewItem.SubItems.Add("SubItem");
        listViewItemInGroup.SubItems.Add("SubItem");

        Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_AfterAddingItem_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 0);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = (AccessibleObject)listViewItem.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        AccessibleObject subItemAccInGroup = (AccessibleObject)listViewItemInGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.NotNull(subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.NotNull(subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        listViewItem.SubItems.Add("SubItem");
        listViewItemInGroup.SubItems.Add("SubItem");

        Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_AfterRemovingItem_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 2, subItemCount: 1);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = listViewItem.SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccInGroup = listViewItemInGroup.SubItems[0].AccessibilityObject;

        Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        listViewItem.SubItems.RemoveAt(1);
        listViewItemInGroup.SubItems.RemoveAt(1);

        Assert.NotNull(subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.NotNull(subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_AfterRemovingItem_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 1);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = (AccessibleObject)listViewItem.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        AccessibleObject subItemAccInGroup = (AccessibleObject)listViewItemInGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        listViewItem.SubItems.Add("SubItem");
        listViewItemInGroup.SubItems.Add("SubItem");

        Assert.NotNull(subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.NotNull(subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_WithoutSubItems_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 5, subItemCount: 0);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = (AccessibleObject)listViewItem.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        AccessibleObject subItemAccInGroup = (AccessibleObject)listViewItemInGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        AccessibleObject previousSubItemAccObj4 = (AccessibleObject)subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccObj3 = (AccessibleObject)previousSubItemAccObj4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccObj2 = (AccessibleObject)previousSubItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccObj1 = (AccessibleObject)previousSubItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccObj0 = (AccessibleObject)previousSubItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

        AccessibleObject previousSubItemAccInGroup4 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup3 = (AccessibleObject)previousSubItemAccInGroup4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup2 = (AccessibleObject)previousSubItemAccInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup1 = (AccessibleObject)previousSubItemAccInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);
        AccessibleObject previousSubItemAccInGroup0 = (AccessibleObject)previousSubItemAccInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling);

        Assert.NotNull(previousSubItemAccObj4);
        Assert.NotNull(previousSubItemAccObj3);
        Assert.NotNull(previousSubItemAccObj2);
        Assert.Same(listView.Items[0].SubItems[0].AccessibilityObject, previousSubItemAccObj1);
        Assert.Null(previousSubItemAccObj0);

        Assert.NotNull(previousSubItemAccInGroup4);
        Assert.NotNull(previousSubItemAccInGroup3);
        Assert.NotNull(previousSubItemAccInGroup2);
        Assert.Same(listViewItemInGroup.SubItems[0].AccessibilityObject, previousSubItemAccInGroup1);
        Assert.Null(previousSubItemAccInGroup0);

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewSubItemAccessibleObject_FragmentNavigate_NextSibling_ReturnsExpected_WithoutSubItems_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 5, subItemCount: 0);
        ListViewItem listViewItem = listView.Items[0];
        ListViewItem listViewItemInGroup = listView.Items[1];

        AccessibleObject subItemAccObj = listView.Items[0].SubItems[0].AccessibilityObject;
        AccessibleObject subItemAccInGroup = listViewItemInGroup.SubItems[0].AccessibilityObject;

        AccessibleObject previousSubItemAccObj1 = (AccessibleObject)subItemAccObj.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccObj2 = (AccessibleObject)previousSubItemAccObj1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccObj3 = (AccessibleObject)previousSubItemAccObj2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccObj4 = (AccessibleObject)previousSubItemAccObj3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccObj5 = (AccessibleObject)previousSubItemAccObj4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

        AccessibleObject previousSubItemAccInGroup1 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccInGroup2 = (AccessibleObject)previousSubItemAccInGroup1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccInGroup3 = (AccessibleObject)previousSubItemAccInGroup2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccInGroup4 = (AccessibleObject)previousSubItemAccInGroup3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);
        AccessibleObject previousSubItemAccInGroup5 = (AccessibleObject)previousSubItemAccInGroup4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling);

        Assert.NotNull(previousSubItemAccObj1);
        Assert.NotNull(previousSubItemAccObj2);
        Assert.NotNull(previousSubItemAccObj3);
        Assert.NotNull(previousSubItemAccObj4);
        Assert.Null(previousSubItemAccObj5);

        Assert.NotNull(previousSubItemAccInGroup1);
        Assert.NotNull(previousSubItemAccInGroup2);
        Assert.NotNull(previousSubItemAccInGroup3);
        Assert.NotNull(previousSubItemAccInGroup4);
        Assert.Null(previousSubItemAccInGroup5);

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_Bounds_ReturnCorrectValue()
    {
        using ListView list = new();
        ListViewItem listViewItem1 = new(
        [
        "Test 1",
        "Item 1",
        "Something 1"
        ], -1);

        ColumnHeader columnHeader1 = new();
        ColumnHeader columnHeader2 = new();
        ColumnHeader columnHeader3 = new();

        list.Columns.AddRange(
        [
        columnHeader1,
        columnHeader2,
        columnHeader3
        ]);
        list.HideSelection = false;
        list.Items.Add(listViewItem1);
        list.View = View.Details;

        list.CreateControl();

        ListViewItem.ListViewSubItem subItem = listViewItem1.SubItems[0];
        AccessibleObject accessibleObject = listViewItem1.AccessibilityObject.GetChild(0);

        int actualWidth = accessibleObject.Bounds.Width;
        int expectedWidth = listViewItem1.SubItems[1].Bounds.X - subItem.Bounds.X;
        Assert.Equal(expectedWidth, actualWidth);

        int actualHeight = accessibleObject.Bounds.Height;
        int expectedHeight = subItem.Bounds.Height;
        Assert.Equal(expectedHeight, actualHeight);

        Rectangle actualBounds = accessibleObject.Bounds;
        actualBounds.Location = new Point(0, 0);
        Rectangle expectedBounds = new(subItem.Bounds.X, subItem.Bounds.Y, expectedWidth, expectedHeight)
        {
            Location = new Point(0, 0)
        };
        Assert.Equal(expectedBounds, actualBounds);
        Assert.True(list.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListViewSubItemAccessibleObject_ColumnProperty_ReturnCorrectValue(bool createControl)
    {
        using ListView list = new() { View = View.Details };
        ListViewItem listViewItem1 = new(
        [
            "Test 1",
            "Item 1",
            "Something 1",
            "Something 2"
        ]);

        list.Columns.AddRange(
        [
            new(),
            new(),
            new()
        ]);

        list.Items.Add(listViewItem1);

        if (createControl)
        {
            list.CreateControl();
        }

        Assert.Equal(0, listViewItem1.SubItems[0].AccessibilityObject.Column);
        Assert.Equal(1, listViewItem1.SubItems[1].AccessibilityObject.Column);
        Assert.Equal(2, listViewItem1.SubItems[2].AccessibilityObject.Column);
        Assert.Equal(-1, listViewItem1.SubItems[3].AccessibilityObject.Column);
        Assert.Equal(createControl, list.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.LargeIcon, true)]
    [InlineData(View.LargeIcon, false)]
    [InlineData(View.List, true)]
    [InlineData(View.List, false)]
    [InlineData(View.SmallIcon, true)]
    [InlineData(View.SmallIcon, false)]
    [InlineData(View.Tile, true)]
    [InlineData(View.Tile, false)]
    public void ListViewSubItemAccessibleObject_ColumnProperty_ReturnMinusOne_ForNotTableView(View view, bool createControl)
    {
        using ListView list = new() { View = view };
        ListViewItem listViewItem1 = new(
        [
            "Test 1",
            "Item 1",
            "Something 1",
            "Something 2",
        ]);

        list.Columns.AddRange(
        [
            new(),
            new(),
            new()
        ]);

        list.Items.Add(listViewItem1);

        if (createControl)
        {
            list.CreateControl();
        }

        Assert.Equal(-1, listViewItem1.SubItems[0].AccessibilityObject.Column);
        Assert.Equal(-1, listViewItem1.SubItems[1].AccessibilityObject.Column);
        Assert.Equal(-1, listViewItem1.SubItems[2].AccessibilityObject.Column);
        Assert.Equal(-1, listViewItem1.SubItems[3].AccessibilityObject.Column);
        Assert.Equal(createControl, list.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_RowProperty_ReturnCorrectValue()
    {
        using ListView list = new();
        ListViewItem listViewItem1 = new(
        [
        "Test 1",
        "Item 1",
        "Something 1"
        ], -1);

        ColumnHeader columnHeader1 = new();
        ColumnHeader columnHeader2 = new();
        ColumnHeader columnHeader3 = new();

        list.Columns.AddRange(
        [
        columnHeader1,
        columnHeader2,
        columnHeader3
        ]);
        list.HideSelection = false;
        list.Items.Add(listViewItem1);
        list.View = View.Details;

        list.CreateControl();

        AccessibleObject subItemAccObj1 = listViewItem1.AccessibilityObject.GetChild(0);
        AccessibleObject subItemAccObj2 = listViewItem1.AccessibilityObject.GetChild(1);
        AccessibleObject subItemAccObj3 = listViewItem1.AccessibilityObject.GetChild(2);
        Assert.True(list.IsHandleCreated);

        Assert.Equal(0, subItemAccObj1.Row);
        Assert.Equal(0, subItemAccObj2.Row);
        Assert.Equal(0, subItemAccObj3.Row);
    }

    private ListView GetListViewWithSubItemData(
        View view,
        bool createControl,
        bool virtualMode,
        bool showGroups,
        int columnCount,
        int subItemCount)
    {
        ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualMode = virtualMode,
            VirtualListSize = 2
        };

        ListViewItem listItem1 = new("Item 1");
        ListViewItem listItem2 = new("Item 2");

        if (!virtualMode)
        {
            ListViewGroup listViewGroup = new("Test");
            listView.Groups.Add(listViewGroup);
            listItem2.Group = listViewGroup;
        }

        for (int i = 0; i < subItemCount; i++)
        {
            listItem1.SubItems.Add($"SubItem {i}");
            listItem2.SubItems.Add($"SubItem {i}");
        }

        for (int i = 0; i < columnCount; i++)
        {
            listView.Columns.Add(new ColumnHeader($"Column {i}"));
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    1 => listItem2,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 0);
        }
        else
        {
            listView.Items.AddRange((ListViewItem[])[listItem1, listItem2]);
        }

        if (createControl)
        {
            listView.CreateControl();
        }

        return listView;
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_ProcessId_ReturnCorrectValue()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new("Test item")
        {
            _listView = listView
        };
        ListViewItem.ListViewSubItem subItem = new(listViewItem, "Test subItem");

        int actual = (int)subItem.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ProcessIdPropertyId);

        Assert.Equal(Environment.ProcessId, actual);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_Bounds_Equals_ListViewItem_IfFullRowSelectIsTrue()
    {
        using ListView listView = new()
        {
            View = View.Details,
            FullRowSelect = true
        };

        listView.CreateControl();
        listView.Columns.AddRange([new() { Width = 120, Text = "Column 1" }, new() { Width = 120, Text = "Column 2" }]);
        listView.Items.Add(new ListViewItem("Test item 11"));
        listView.Items[0].SubItems.Add("Test item 12");

        var itemAccessibleObject = listView.Items[0].AccessibilityObject;
        var subItemAccessibleObject = listView.Items[0].SubItems[0].AccessibilityObject;

        Assert.Equal(itemAccessibleObject.Bounds, subItemAccessibleObject.Bounds);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_Bounds_NotEquals_ListViewItem_IfFullRowSelectIsFalse()
    {
        using ListView listView = new()
        {
            View = View.Details,
            FullRowSelect = false
        };

        listView.CreateControl();

        listView.Columns.AddRange([new() { Width = 120, Text = "Column 1" }, new() { Width = 120, Text = "Column 2" }]);
        listView.Items.Add(new ListViewItem("Test item 11"));
        listView.Items[0].SubItems.Add("Test item 12");

        var itemAccessibleObject = listView.Items[0].AccessibilityObject;
        var subItemAccessibleObject = listView.Items[0].SubItems[0].AccessibilityObject;

        Assert.NotEqual(itemAccessibleObject.Bounds, subItemAccessibleObject.Bounds);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_GetPropertyValue_RuntimeId_ReturnsExpected()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new("Test item");
        listView.Items.Add(listViewItem);
        ListViewItem.ListViewSubItem listViewSubItem = new(listViewItem, "Test subItem");
        using VARIANT actual = listViewSubItem.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId);

        Assert.Equal(listViewSubItem.AccessibilityObject.RuntimeId, actual.ToObject());
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_GetPropertyValue_BoundingRectangle_ReturnsExpected()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new("Test item");
        listView.Items.Add(listViewItem);
        ListViewItem.ListViewSubItem listViewSubItem = new(listViewItem, "Test subItem");
        ListViewSubItemAccessibleObject listViewSubItemAccessibleObject = new(listViewSubItem, listViewItem);
        using VARIANT actual = listViewSubItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId);
        double[] actualArray = (double[])actual.ToObject();
        Rectangle actualRectangle = new((int)actualArray[0], (int)actualArray[1], (int)actualArray[2], (int)actualArray[3]);
        Assert.Equal(listViewSubItem.AccessibilityObject.BoundingRectangle, actualRectangle);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_GetPropertyValue_IsOffscreen_ReturnsFalse()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new("Test item");
        listView.Items.Add(listViewItem);
        ListViewItem.ListViewSubItem listViewSubItem = new(listViewItem, "Test subItem");
        ListViewSubItemAccessibleObject listViewSubItemAccessibleObject = new(listViewSubItem, listViewItem);
        bool actual = (bool)listViewSubItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        Assert.False(actual);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsScrollPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId))]
    [InlineData(true, ((int)UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId))]
    [InlineData(false, ((int)UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId))]
    public void ListViewSubItemAccessibleObject_GetPropertyValue_Pattern_ReturnsExpected(bool expected, int propertyId)
    {
        using ListView listView = new() { View = View.Details };
        ListViewItem listViewItem = new("Test item");
        listView.Items.Add(listViewItem);
        ListViewItem.ListViewSubItem listViewSubItem = new(listViewItem, "Test subItem");
        ListViewSubItemAccessibleObject accessibleObject = new(listViewSubItem, listViewItem);
        var result = accessibleObject.GetPropertyValue((UIA_PROPERTY_ID)propertyId);
        Assert.Equal(expected, !result.IsEmpty && (bool)result);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_GetColumnHeaderItems_IsExpected()
    {
        using ListView control = GetListViewWithSubItemData(
            View.Details,
            createControl: false,
            virtualMode: false,
            showGroups: false,
            columnCount: 3,
            subItemCount: 2);

        ListViewSubItemAccessibleObject_GetColumnHeaderItems_IsExpected_Internal(control);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_GetColumnHeaderItems_IsExpected_WithImage()
    {
        using ImageList imageCollection = new();
        imageCollection.Images.Add(Form.DefaultIcon);
        using ListView control = GetListViewWithSubItemData(
            View.Details,
            createControl: false,
            virtualMode: false,
            showGroups: false,
            columnCount: 3,
            subItemCount: 2);
        control.SmallImageList = imageCollection;
        control.Items[0].ImageIndex = 0;

        ListViewSubItemAccessibleObject_GetColumnHeaderItems_IsExpected_Internal(control);
    }

    private void ListViewSubItemAccessibleObject_GetColumnHeaderItems_IsExpected_Internal(ListView control)
    {
        ListView.ColumnHeaderCollection columns = control.Columns;
        for (int i = 0; i < columns.Count; i++)
        {
            var subItemAccessibleObject = (ListViewSubItemAccessibleObject)control.Items[0].SubItems[i].AccessibilityObject;
            AccessibleObject columnHeaderAccessibleObject = columns[i].AccessibilityObject;
            IRawElementProviderSimple.Interface[] columnHeaderItems = subItemAccessibleObject.GetColumnHeaderItems();

            Assert.Single(columnHeaderItems);
            Assert.Same(columnHeaderAccessibleObject, columnHeaderItems[0]);
        }
    }

    [WinFormsTheory]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewSubItemAccessibleObject_GetColumnHeaderItems_ReturnsNull_NoColumsViews(View view)
    {
        using ListView control = GetListViewWithSubItemData(
            view,
            createControl: false,
            virtualMode: false,
            showGroups: false,
            columnCount: 3,
            subItemCount: 2);

        var subItemAccessibleObject = (ListViewSubItemAccessibleObject)control.Items[0].SubItems[1].AccessibilityObject;

        Assert.Null(subItemAccessibleObject.GetColumnHeaderItems());
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_State_ReturnsFocusable()
    {
        using ListView listview = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listview.Items.Add(listViewItem);

        Assert.Equal(AccessibleStates.Focusable, listViewSubItem.AccessibilityObject.State);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_IsDisconnected_WhenListViewReleasesUiaProvider()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listView.Items.Add(listViewItem);
        EnforceAccessibleObjectCreation(listViewItem);

        listView.ReleaseUiaProvider(listView.HWND);

        Assert.Null(listViewSubItem.TestAccessor().Dynamic._accessibilityObject);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_IsDisconnected_WhenListViewIsCleared()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listView.Items.Add(listViewItem);
        EnforceAccessibleObjectCreation(listViewItem);

        listView.Clear();

        Assert.Null(listViewSubItem.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_IsDisconnected_WhenOwningItemIsRemoved()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listView.Items.Add(listViewItem);
        EnforceAccessibleObjectCreation(listViewItem);

        listView.Items.RemoveAt(0);

        Assert.Null(listViewSubItem.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_IsDisconnected_WhenSubItemIsRemoved()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listView.Items.Add(listViewItem);
        EnforceAccessibleObjectCreation(listViewItem);

        listView.Items[0].SubItems.Remove(listViewSubItem);

        Assert.Null(listViewSubItem.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_IsDisconnected_WhenSubItemsAreCleared()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listView.Items.Add(listViewItem);
        EnforceAccessibleObjectCreation(listViewItem);

        listView.Items[0].SubItems.Clear();

        Assert.Null(listViewSubItem.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_IsDisconnected_WhenSubItemIsReplaced()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listView.Items.Add(listViewItem);
        EnforceAccessibleObjectCreation(listViewItem);

        listView.Items[0] = new ListViewItem();

        Assert.Null(listViewSubItem.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewSubItemAccessibleObject_IsDisconnected_WhenOwningItemIsReplaced()
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewItem.ListViewSubItem listViewSubItem = new();
        listViewItem.SubItems.Add(listViewSubItem);
        listView.Items.Add(listViewItem);
        EnforceAccessibleObjectCreation(listViewItem);

        int subItemIndex = listView.Items[0].SubItems.IndexOf(listViewSubItem);
        listView.Items[0].SubItems[subItemIndex] = new ListViewItem.ListViewSubItem();

        Assert.Null(listViewSubItem.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    private static void EnforceAccessibleObjectCreation(ListViewItem item)
    {
        _ = item.AccessibilityObject;
        Assert.NotNull(item.TestAccessor().Dynamic._accessibilityObject);
        foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
        {
            _ = subItem.AccessibilityObject;
            Assert.NotNull(subItem.TestAccessor().Dynamic._accessibilityObject);
        }
    }
}
