// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ListViewGroup;
using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListView_ListViewAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListViewAccessibleObject_Ctor_Default()
        {
            using ListView listView = new ListView();

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.List, accessibleObject.Role);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_EmptyList_GetChildCount_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, accessibleObject.GetChildCount()); // listView doesn't have items
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetMultiViewProviderCurrentView_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.Equal((int)listView.View, accessibleObject.GetMultiViewProviderCurrentView());
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetMultiViewProviderSupportedViews_ReturnsExpected()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(new int[] { (int)View.Details }, accessibleObject.GetMultiViewProviderSupportedViews());
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetMultiViewProviderViewName_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            listView.View = View.Details;
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(((int)(listView.View)).ToString(), accessibleObject.GetMultiViewProviderViewName((int)View.Details));
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithOneItem_GetChildCount_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            listView.Items.Add(new ListViewItem());
            Assert.Equal(1, accessibleObject.GetChildCount()); // One item
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoGroups_GetChildCount_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            listView.Items.Add(new ListViewItem());
            ListViewItem item = new ListViewItem();
            ListViewItem item2 = new ListViewItem();
            ListViewGroup group = new ListViewGroup();
            item2.Group = group;
            item.Group = group;
            listView.Groups.Add(group);
            listView.Items.Add(item);
            listView.Items.Add(item2);

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(2, accessibleObject.GetChildCount()); // Default group and one specified group
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly()
        {
            using ListView listView = new ListView();
            listView.Items.Add(new ListViewItem());
            ListViewItem item = new ListViewItem();
            ListViewItem item2 = new ListViewItem();
            ListViewGroup group = new ListViewGroup();
            item2.Group = group;
            item.Group = group;
            listView.Groups.Add(group);
            listView.Items.Add(item);
            listView.Items.Add(item2);

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);

            AccessibleObject firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewGroupAccessibleObject>(firstChild);
            Assert.IsType<ListViewGroupAccessibleObject>(lastChild);
            Assert.NotEqual(firstChild, lastChild);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoItems_GetChildCount_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            listView.Items.Add(new ListViewItem());
            listView.Items.Add(new ListViewItem());

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(2, accessibleObject.GetChildCount()); // Two child items
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoItems_FragmentNavigateWorkCorrectly()
        {
            using ListView listView = new ListView();
            listView.Items.Add(new ListViewItem());
            listView.Items.Add(new ListViewItem());

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);

            AccessibleObject firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewItemAccessibleObject>(firstChild);
            Assert.IsType<ListViewItemAccessibleObject>(lastChild);
            Assert.NotEqual(firstChild, lastChild);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_EmptyList_GetChild_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, accessibleObject.GetChildCount()); // listView doesn't have items
            Assert.Null(accessibleObject.GetChild(0)); // GetChild method should not throw an exception
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetPropertyValue_returns_correct_values()
        {
            using var list = new ListView();
            list.Name = "List";
            list.AccessibleName = "ListView";
            AccessibleObject listAccessibleObject = list.AccessibilityObject;
            Assert.True(list.IsHandleCreated);

            object accessibleName = listAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);
            Assert.Equal("ListView", accessibleName);

            object automationId = listAccessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId);
            Assert.Equal("List", automationId);

            object accessibleControlType = listAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            Assert.Equal(UiaCore.UIA.ListControlTypeId, accessibleControlType);

            object controlType = listAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = UiaCore.UIA.ListControlTypeId;
            Assert.Equal(expected, controlType);

            Assert.True((bool)listAccessibleObject.GetPropertyValue(UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId));
        }
    }
}

