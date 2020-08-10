// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;
using static System.Windows.Forms.ListViewItem;
using static System.Windows.Forms.ListViewItem.ListViewSubItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewItem_ListViewItemAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListViewItemAccessibleObject_Ctor_ThrowsArgumentNullException()
        {
            using ListView list = new ListView();
            ListViewItem listItem = new ListViewItem();
            list.Items.Add(listItem);

            Type type = listItem.AccessibilityObject.GetType();
            ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(ListViewItem), typeof(ListViewGroup) });
            Assert.NotNull(ctor);
            Assert.Throws<TargetInvocationException>(() => ctor.Invoke(new object[] { null, null }));

            // item without parent ListView
            ListViewItem itemWithoutList = new ListViewItem();
            Assert.Throws<TargetInvocationException>(() => ctor.Invoke(new object[] { itemWithoutList, null }));
        }

        [WinFormsFact]
        public void ListViewItemAccessibleObject_Ctor_Default()
        {
            using ListView list = new ListView();
            ListViewItem listItem = new ListViewItem();
            list.Items.Add(listItem);

            AccessibleObject accessibleObject = listItem.AccessibilityObject;
            Assert.True(list.IsHandleCreated);
            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.ListItem, accessibleObject.Role);
        }

        [WinFormsFact]
        public void ListViewItemAccessibleObject_GetPropertyValue_returns_correct_values()
        {
            using var list = new ListView();
            ListViewItem listItem = new ListViewItem("ListItem");
            list.Items.Add(listItem);
            AccessibleObject listItemAccessibleObject = listItem.AccessibilityObject;
            Assert.True(list.IsHandleCreated);

            object accessibleName = listItemAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);
            Assert.Equal("ListItem", accessibleName);

            object automationId = listItemAccessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId);
            Assert.Equal("ListViewItem-0", automationId);

            object controlType = listItemAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = UiaCore.UIA.ListItemControlTypeId;
            Assert.Equal(expected, controlType);

            Assert.True((bool)listItemAccessibleObject.GetPropertyValue(UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId));
            Assert.True((bool)listItemAccessibleObject.GetPropertyValue(UiaCore.UIA.IsScrollItemPatternAvailablePropertyId));
            Assert.True((bool)listItemAccessibleObject.GetPropertyValue(UiaCore.UIA.IsInvokePatternAvailablePropertyId));
        }

        [WinFormsFact]
        public void ListViewItemAccessibleObject_ListWithTwoItems_FragmentNavigateWorkCorrectly()
        {
            using ListView listView = new ListView();
            ListViewItem listItem1 = new ListViewItem(new string[] {
                "Test A",
                "Alpha"}, -1);
            ListViewItem listItem2 = new ListViewItem(new string[] {
                "Test B",
                "Beta"}, -1);
            ListViewItem listItem3 = new ListViewItem(new string[] {
                "Test C",
                "Gamma"}, -1);
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
            listView.Items.Add(listItem3);
            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;
            Assert.True(listView.IsHandleCreated);

            // First list view item
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            UiaCore.IRawElementProviderFragment listItem1NextSibling = accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            Assert.IsType<ListViewItemAccessibleObject>(listItem1NextSibling);
            Assert.Equal(accessibleObject2, listItem1NextSibling);

            // Second list view item
            UiaCore.IRawElementProviderFragment listItem2PreviousSibling = accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            UiaCore.IRawElementProviderFragment listItem2NextSibling = accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            Assert.IsType<ListViewItemAccessibleObject>(listItem2PreviousSibling);
            Assert.IsType<ListViewItemAccessibleObject>(listItem2NextSibling);
            Assert.Equal(accessibleObject1, listItem2PreviousSibling);
            Assert.Equal(accessibleObject3, listItem2NextSibling);

            // Third list view item
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            UiaCore.IRawElementProviderFragment listItem3PreviousSibling = accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.IsType<ListViewItemAccessibleObject>(listItem3PreviousSibling);
            Assert.Equal(accessibleObject2, listItem3PreviousSibling);

            // Parent
            Assert.Equal(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            Assert.Equal(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            Assert.Equal(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);

            // Childs
            AccessibleObject firstChild = accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChild);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChild);
            Assert.NotEqual(firstChild, lastChild);
        }
    }
}
