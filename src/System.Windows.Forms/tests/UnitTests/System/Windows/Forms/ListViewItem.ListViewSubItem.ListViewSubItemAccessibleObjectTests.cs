// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            using ListView list = new ListView();
            ListViewItem listViewItem1 = new ListViewItem(new string[] {
            "Test 1",
            "Item 1",
            "Something 1"}, -1);

            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();

            list.Columns.AddRange(new ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;

            AccessibleObject accessibleObject = listViewItem1.AccessibilityObject.GetChild(0);
            Assert.NotNull(accessibleObject);
            Assert.IsType<ListViewSubItemAccessibleObject>(accessibleObject);
            Assert.False(list.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_GetPropertyValue_returns_correct_values()
        {
            using ListView list = new ListView();
            ListViewItem listViewItem1 = new ListViewItem(new string[] {
            "Test 1",
            "Item 1",
            "Something 1"}, -1);

            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();

            list.Columns.AddRange(new ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;

            AccessibleObject accessibleObject = listViewItem1.AccessibilityObject.GetChild(0);

            object accessibleName = accessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);
            Assert.Equal("Test 1", accessibleName);

            object automationId = accessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId);
            Assert.Equal("ListViewSubItem-0", automationId);

            object frameworkId = accessibleObject.GetPropertyValue(UiaCore.UIA.FrameworkIdPropertyId);
            Assert.Equal("WinForm", frameworkId);

            object controlType = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = UiaCore.UIA.TextControlTypeId;
            Assert.Equal(expected, controlType);

            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsGridItemPatternAvailablePropertyId));
            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsTableItemPatternAvailablePropertyId));
            Assert.False(list.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_FragmentNavigate_WorkCorrectly()
        {
            using ListView list = new ListView();
            ListViewItem listViewItem1 = new ListViewItem(new string[] {
            "Test 1",
            "Item 1",
            "Something 1"}, -1);

            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();

            list.Columns.AddRange(new ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;

            AccessibleObject subItemAccObj1 = listViewItem1.AccessibilityObject.GetChild(0);
            AccessibleObject subItemAccObj2 = listViewItem1.AccessibilityObject.GetChild(1);
            AccessibleObject subItemAccObj3 = listViewItem1.AccessibilityObject.GetChild(2);

            Assert.Null(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            UiaCore.IRawElementProviderFragment subItem1NextSibling = subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            Assert.IsType<ListViewSubItemAccessibleObject>(subItem1NextSibling);
            Assert.NotNull(subItem1NextSibling);

            UiaCore.IRawElementProviderFragment subItem2NextSibling = subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            UiaCore.IRawElementProviderFragment subItem2PreviousSibling = subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.IsType<ListViewSubItemAccessibleObject>(subItem2NextSibling);
            Assert.IsType<ListViewSubItemAccessibleObject>(subItem2PreviousSibling);
            Assert.NotNull(subItem2NextSibling);
            Assert.NotNull(subItem2PreviousSibling);

            Assert.Null(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            UiaCore.IRawElementProviderFragment subItem3PreviousSibling = subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.IsType<ListViewSubItemAccessibleObject>(subItem3PreviousSibling);
            Assert.NotNull(subItem3PreviousSibling);

            // Parent
            Assert.Equal(subItemAccObj1.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewItem1.AccessibilityObject);
            Assert.Equal(subItemAccObj2.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewItem1.AccessibilityObject);
            Assert.Equal(subItemAccObj3.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewItem1.AccessibilityObject);

            Assert.False(list.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_Bounds_ReturnCorrectValue()
        {
            using ListView list = new ListView();
            ListViewItem listViewItem1 = new ListViewItem(new string[] {
            "Test 1",
            "Item 1",
            "Something 1"}, -1);

            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();

            list.Columns.AddRange(new ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;

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
            Rectangle expectedBounds = new Rectangle(subItem.Bounds.X, subItem.Bounds.Y, expectedWidth, expectedHeight);
            expectedBounds.Location = new Point(0, 0);
            Assert.Equal(expectedBounds, actualBounds);
            Assert.False(list.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_ColunmProperty_ReturnCorrectValue()
        {
            using ListView list = new ListView();
            ListViewItem listViewItem1 = new ListViewItem(new string[] {
            "Test 1",
            "Item 1",
            "Something 1"}, -1);

            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();

            list.Columns.AddRange(new ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;

            AccessibleObject subItemAccObj1 = listViewItem1.AccessibilityObject.GetChild(0);
            AccessibleObject subItemAccObj2 = listViewItem1.AccessibilityObject.GetChild(1);
            AccessibleObject subItemAccObj3 = listViewItem1.AccessibilityObject.GetChild(2);
            Assert.False(list.IsHandleCreated);

            Assert.Equal(0, subItemAccObj1.Column);
            Assert.Equal(1, subItemAccObj2.Column);
            Assert.Equal(2, subItemAccObj3.Column);
        }

        [WinFormsFact]
        public void ListViewSubItemAccessibleObject_RowProperty_ReturnCorrectValue()
        {
            using ListView list = new ListView();
            ListViewItem listViewItem1 = new ListViewItem(new string[] {
            "Test 1",
            "Item 1",
            "Something 1"}, -1);

            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();

            list.Columns.AddRange(new ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3});
            list.HideSelection = false;
            list.Items.Add(listViewItem1);
            list.View = View.Details;

            AccessibleObject subItemAccObj1 = listViewItem1.AccessibilityObject.GetChild(0);
            AccessibleObject subItemAccObj2 = listViewItem1.AccessibilityObject.GetChild(1);
            AccessibleObject subItemAccObj3 = listViewItem1.AccessibilityObject.GetChild(2);
            Assert.False(list.IsHandleCreated);

            Assert.Equal(0, subItemAccObj1.Row);
            Assert.Equal(0, subItemAccObj2.Row);
            Assert.Equal(0, subItemAccObj3.Row);
        }
    }
}
