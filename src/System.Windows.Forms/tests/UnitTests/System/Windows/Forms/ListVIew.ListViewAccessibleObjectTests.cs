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
            Assert.NotNull(accessibleObject);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_EmptyList_GetChildCount_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal(0, accessibleObject.GetChildCount()); // listView doesn't have items
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.List)]
        [InlineData(false, AccessibleRole.None)]
        public void ListViewAccessibleObject_DefaultRole_ReturnsCorrectValue(bool createControl, AccessibleRole expectedAccessibleRole)
        {
            using ListView listView = new ListView();
            if (createControl)
            {
                listView.CreateControl();
            }

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal(expectedAccessibleRole, accessibleObject.Role);
            Assert.Equal(createControl, listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetMultiViewProviderCurrentView_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal((int)listView.View, accessibleObject.GetMultiViewProviderCurrentView());
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetMultiViewProviderSupportedViews_ReturnsExpected()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal(new int[] { (int)View.Details }, accessibleObject.GetMultiViewProviderSupportedViews());
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetMultiViewProviderViewName_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            listView.View = View.Details;
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal(((int)(listView.View)).ToString(), accessibleObject.GetMultiViewProviderViewName((int)View.Details));
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ListViewAccessibleObject_ListWithOneItem_GetChildCount_ReturnsCorrectValue(bool createdControl, int expectedChildCount)
        {
            using ListView listView = new ListView();
            if (createdControl)
            {
                listView.CreateControl();
            }

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            listView.Items.Add(new ListViewItem());
            Assert.Equal(expectedChildCount, accessibleObject.GetChildCount()); // One item
            Assert.Equal(createdControl, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2)]
        [InlineData(false, 0)]
        public void ListViewAccessibleObject_ListWithTwoGroups_GetChildCount_ReturnsCorrectValue(bool createdControl, int expectedChildCount)
        {
            using ListView listView = new ListView();
            if (createdControl)
            {
                listView.CreateControl();
            }

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
            Assert.Equal(expectedChildCount, accessibleObject.GetChildCount()); // Default group and one specified group
            Assert.Equal(createdControl, listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsCreated()
        {
            using ListView listView = new ListView();
            listView.CreateControl();
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

            AccessibleObject firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewGroupAccessibleObject>(firstChild);
            Assert.IsType<ListViewGroupAccessibleObject>(lastChild);
            Assert.NotEqual(firstChild, lastChild);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsNotCreated()
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

            AccessibleObject firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.Null(firstChild);
            Assert.Null(lastChild);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, 2)]
        [InlineData(false, 0)]
        public void ListViewAccessibleObject_ListWithTwoItems_GetChildCount_ReturnsCorrectValue(bool createdControl, int expectedChildCount)
        {
            using ListView listView = new ListView();
            if (createdControl)
            {
                listView.CreateControl();
            }

            listView.Items.Add(new ListViewItem());
            listView.Items.Add(new ListViewItem());

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal(expectedChildCount, accessibleObject.GetChildCount()); // Two child items
            Assert.Equal(createdControl, listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoItems_FragmentNavigateWorkCorrectly_IfHandleIsCreated()
        {
            using ListView listView = new ListView();
            listView.CreateControl();
            listView.Items.Add(new ListViewItem());
            listView.Items.Add(new ListViewItem());

            AccessibleObject accessibleObject = listView.AccessibilityObject;

            AccessibleObject firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewItemAccessibleObject>(firstChild);
            Assert.IsType<ListViewItemAccessibleObject>(lastChild);
            Assert.NotEqual(firstChild, lastChild);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_ListWithTwoItems_FragmentNavigateWorkCorrectly_IfHandleIsNotCreated()
        {
            using ListView listView = new ListView();
            listView.Items.Add(new ListViewItem());
            listView.Items.Add(new ListViewItem());

            AccessibleObject accessibleObject = listView.AccessibilityObject;

            AccessibleObject firstChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.Null(firstChild);
            Assert.Null(lastChild);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_EmptyList_GetChild_ReturnsCorrectValue()
        {
            using ListView listView = new ListView();
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal(0, accessibleObject.GetChildCount()); // listView doesn't have items
            Assert.Null(accessibleObject.GetChild(0)); // GetChild method should not throw an exception
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewAccessibleObject_GetPropertyValue_returns_correct_values()
        {
            using var list = new ListView();
            list.Name = "List";
            list.AccessibleName = "ListView";
            AccessibleObject listAccessibleObject = list.AccessibilityObject;

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
            Assert.False(list.IsHandleCreated);
        }
    }
}

