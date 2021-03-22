// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;
using static System.Windows.Forms.ListViewItem.ListViewSubItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewItem_ListViewSubItem_ListViewSubItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_GetChild_ReturnCorrectValue()
        {
            using ListView list = new();
            ListViewItem listViewItem1 = new(new string[]
            {
            "Test 1",
            "Item 1",
            "Something 1"
            }, -1);

            ColumnHeader columnHeader1 = new();
            ColumnHeader columnHeader2 = new();
            ColumnHeader columnHeader3 = new();

            list.Columns.AddRange(new ColumnHeader[]
            {
            columnHeader1,
            columnHeader2,
            columnHeader3
            });
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
        [InlineData(true, 0, (int)UiaCore.UIA.EditControlTypeId)]
        [InlineData(false, 0, (int)UiaCore.UIA.TextControlTypeId)]
        [InlineData(true, 1, (int)UiaCore.UIA.TextControlTypeId)]
        [InlineData(false, 1, (int)UiaCore.UIA.TextControlTypeId)]
        public void ListViewSubItemAccessibleObject_GetPropertyValue_returns_correct_values(bool labelEdit, int childId, int expectedControlType)
        {
            using ListView list = new();
            ListViewItem listViewItem1 = new(new string[]
            {
            "Test 1",
            "Test 2",
            "Something 1"
            }, -1);

            ColumnHeader columnHeader1 = new();
            ColumnHeader columnHeader2 = new();
            ColumnHeader columnHeader3 = new();

            list.Columns.AddRange(new ColumnHeader[]
            {
            columnHeader1,
            columnHeader2,
            columnHeader3
            });
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;
            list.LabelEdit = labelEdit;

            list.CreateControl();

            AccessibleObject accessibleObject = listViewItem1.AccessibilityObject.GetChild(childId);

            object accessibleName = accessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);
            Assert.Equal($"Test {childId + 1}", accessibleName);

            object automationId = accessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId);
            Assert.Equal($"ListViewSubItem-{childId}", automationId);

            object frameworkId = accessibleObject.GetPropertyValue(UiaCore.UIA.FrameworkIdPropertyId);
            Assert.Equal("WinForm", frameworkId);

            object controlType = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = (UiaCore.UIA)expectedControlType;
            Assert.Equal(expected, controlType);

            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsGridItemPatternAvailablePropertyId));
            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsTableItemPatternAvailablePropertyId));
            Assert.True(list.IsHandleCreated);
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

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

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

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

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

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.Null(subItemAccObjInGroup3.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

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

            AccessibleObject nextSubItemAccObj1 = (AccessibleObject)subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject nextSubItemAccObj2 = (AccessibleObject)nextSubItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

            AccessibleObject nextSubItemAccInGroup1 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject nextSubItemAccInGroup2 = (AccessibleObject)nextSubItemAccInGroup1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

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

            AccessibleObject nextSubItemAccObj1 = (AccessibleObject)subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject nextSubItemAccObj2 = (AccessibleObject)nextSubItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

            AccessibleObject nextSubItemAccInGroup1 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject nextSubItemAccInGroup2 = (AccessibleObject)nextSubItemAccInGroup1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

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
            AccessibleObject previousSubItemAccObj = (AccessibleObject)subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup = (AccessibleObject)subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);

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

            AccessibleObject previousSubItemAccObj3 = (AccessibleObject)subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccObj2 = (AccessibleObject)previousSubItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccObj1 = (AccessibleObject)previousSubItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccObj0 = (AccessibleObject)previousSubItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);

            AccessibleObject previousSubItemAccInGroup3 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup2 = (AccessibleObject)previousSubItemAccInGroup3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup1 = (AccessibleObject)previousSubItemAccInGroup2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup0 = (AccessibleObject)previousSubItemAccInGroup1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);

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

            Assert.NotNull(subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.NotNull(subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            listViewItem.SubItems.Add("SubItem");
            listViewItemInGroup.SubItems.Add("SubItem");

            Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.Equal(createControl, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
        public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_AfterAddingItem_Details_View(bool createControl, bool virtualMode, bool showGroups)
        {
            using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 0);
            ListViewItem listViewItem = listView.Items[0];
            ListViewItem listViewItemInGroup = listView.Items[1];

            AccessibleObject subItemAccObj = (AccessibleObject)listViewItem.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
            AccessibleObject subItemAccInGroup = (AccessibleObject)listViewItemInGroup.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            Assert.NotNull(subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.NotNull(subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            listViewItem.SubItems.Add("SubItem");
            listViewItemInGroup.SubItems.Add("SubItem");

            Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

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

            Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            listViewItem.SubItems.RemoveAt(1);
            listViewItemInGroup.SubItems.RemoveAt(1);

            Assert.NotNull(subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.NotNull(subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            Assert.Equal(createControl, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
        public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_AfterRemovingItem_Details_View(bool createControl, bool virtualMode, bool showGroups)
        {
            using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 1);
            ListViewItem listViewItem = listView.Items[0];
            ListViewItem listViewItemInGroup = listView.Items[1];

            AccessibleObject subItemAccObj = (AccessibleObject)listViewItem.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
            AccessibleObject subItemAccInGroup = (AccessibleObject)listViewItemInGroup.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            Assert.Same(listViewItem.SubItems[1].AccessibilityObject, subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Same(listViewItemInGroup.SubItems[1].AccessibilityObject, subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            listViewItem.SubItems.Add("SubItem");
            listViewItemInGroup.SubItems.Add("SubItem");

            Assert.NotNull(subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.NotNull(subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            Assert.Equal(createControl, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewSubItemAccessibleObject_FragmentNavigate_Details_TestData))]
        public void ListViewSubItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnsExpected_WithoutSubItems_Details_View(bool createControl, bool virtualMode, bool showGroups)
        {
            using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 5, subItemCount: 0);
            ListViewItem listViewItem = listView.Items[0];
            ListViewItem listViewItemInGroup = listView.Items[1];

            AccessibleObject subItemAccObj = (AccessibleObject)listViewItem.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);
            AccessibleObject subItemAccInGroup = (AccessibleObject)listViewItemInGroup.AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild);

            AccessibleObject previousSubItemAccObj4 = (AccessibleObject)subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccObj3 = (AccessibleObject)previousSubItemAccObj4.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccObj2 = (AccessibleObject)previousSubItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccObj1 = (AccessibleObject)previousSubItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccObj0 = (AccessibleObject)previousSubItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);

            AccessibleObject previousSubItemAccInGroup4 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup3 = (AccessibleObject)previousSubItemAccInGroup4.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup2 = (AccessibleObject)previousSubItemAccInGroup3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup1 = (AccessibleObject)previousSubItemAccInGroup2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            AccessibleObject previousSubItemAccInGroup0 = (AccessibleObject)previousSubItemAccInGroup1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);

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

            AccessibleObject previousSubItemAccObj1 = (AccessibleObject)subItemAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccObj2 = (AccessibleObject)previousSubItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccObj3 = (AccessibleObject)previousSubItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccObj4 = (AccessibleObject)previousSubItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccObj5 = (AccessibleObject)previousSubItemAccObj4.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

            AccessibleObject previousSubItemAccInGroup1 = (AccessibleObject)subItemAccInGroup.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccInGroup2 = (AccessibleObject)previousSubItemAccInGroup1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccInGroup3 = (AccessibleObject)previousSubItemAccInGroup2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccInGroup4 = (AccessibleObject)previousSubItemAccInGroup3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            AccessibleObject previousSubItemAccInGroup5 = (AccessibleObject)previousSubItemAccInGroup4.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);

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
            ListViewItem listViewItem1 = new(new string[]
            {
            "Test 1",
            "Item 1",
            "Something 1"
            }, -1);

            ColumnHeader columnHeader1 = new();
            ColumnHeader columnHeader2 = new();
            ColumnHeader columnHeader3 = new();

            list.Columns.AddRange(new ColumnHeader[]
            {
            columnHeader1,
            columnHeader2,
            columnHeader3
            });
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
            Rectangle expectedBounds = new(subItem.Bounds.X, subItem.Bounds.Y, expectedWidth, expectedHeight);
            expectedBounds.Location = new Point(0, 0);
            Assert.Equal(expectedBounds, actualBounds);
            Assert.True(list.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_ColumnProperty_ReturnCorrectValue()
        {
            using ListView list = new();
            ListViewItem listViewItem1 = new(new string[]
            {
            "Test 1",
            "Item 1",
            "Something 1"
            }, -1);

            ColumnHeader columnHeader1 = new();
            ColumnHeader columnHeader2 = new();
            ColumnHeader columnHeader3 = new();

            list.Columns.AddRange(new ColumnHeader[]
            {
            columnHeader1,
            columnHeader2,
            columnHeader3
            });
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;

            list.CreateControl();

            AccessibleObject subItemAccObj1 = listViewItem1.AccessibilityObject.GetChild(0);
            AccessibleObject subItemAccObj2 = listViewItem1.AccessibilityObject.GetChild(1);
            AccessibleObject subItemAccObj3 = listViewItem1.AccessibilityObject.GetChild(2);
            Assert.True(list.IsHandleCreated);

            Assert.Equal(0, subItemAccObj1.Column);
            Assert.Equal(1, subItemAccObj2.Column);
            Assert.Equal(2, subItemAccObj3.Column);
        }

        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_RowProperty_ReturnCorrectValue()
        {
            using ListView list = new();
            ListViewItem listViewItem1 = new(new string[]
            {
            "Test 1",
            "Item 1",
            "Something 1"
            }, -1);

            ColumnHeader columnHeader1 = new();
            ColumnHeader columnHeader2 = new();
            ColumnHeader columnHeader3 = new();

            list.Columns.AddRange(new ColumnHeader[]
            {
            columnHeader1,
            columnHeader2,
            columnHeader3
            });
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

            ListViewGroup listViewGroup = new("Test");
            listView.Groups.Add(listViewGroup);

            ListViewItem listItem1 = new("Item 1");
            ListViewItem listItem2 = new("Item 2", group: listViewGroup);

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
                listView.Items.AddRange(new ListViewItem[] { listItem1, listItem2 });
            }

            if (createControl)
            {
                listView.CreateControl();
            }

            return listView;
        }
    }
}
