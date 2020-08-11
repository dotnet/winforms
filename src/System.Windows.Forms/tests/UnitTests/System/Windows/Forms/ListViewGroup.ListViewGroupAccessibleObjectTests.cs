// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;
using static System.Windows.Forms.ListViewGroup;
using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewGroup_ListViewGroupAccessibleObjectTests
    {
        [WinFormsFact]
        public void ListViewGroupAccessibleObject_Ctor_ThrowsArgumentNullException()
        {
            using ListView list = new ListView();
            ListViewGroup listGroup = new ListViewGroup("Group1");
            listGroup.Items.Add(new ListViewItem());
            list.Groups.Add(listGroup);

            Type type = listGroup.AccessibilityObject.GetType();
            ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(ListViewGroup), typeof(bool) });
            Assert.NotNull(ctor);
            Assert.Throws<TargetInvocationException>(() => ctor.Invoke(new object[] { null, false }));

            // group without parent ListView
            ListViewGroup listGroupWithoutList = new ListViewGroup("Group2");
            Assert.Throws<TargetInvocationException>(() => ctor.Invoke(new object[] { listGroupWithoutList, false }));
        }

        [WinFormsFact]
        public void ListViewGroupAccessibleObject_Ctor_Default()
        {
            using ListView list = new ListView();
            ListViewGroup listGroup = new ListViewGroup("Group1");
            listGroup.Items.Add(new ListViewItem());
            list.Groups.Add(listGroup);

            AccessibleObject accessibleObject = listGroup.AccessibilityObject;
            Assert.False(list.IsHandleCreated);

            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.Grouping, accessibleObject.Role);
        }

        [WinFormsFact]
        public void ListViewGroupAccessibleObject_GetPropertyValue_returns_correct_values()
        {
            using ListView list = new ListView();
            ListViewGroup listGroup = new ListViewGroup("Group1");
            listGroup.Items.Add(new ListViewItem());
            list.Groups.Add(listGroup);

            AccessibleObject accessibleObject = listGroup.AccessibilityObject;
            Assert.False(list.IsHandleCreated);

            object accessibleName = accessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);
            Assert.Equal("Group1", accessibleName);

            object automationId = accessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId);
            Assert.Equal("ListViewGroup-0", automationId);

            object controlType = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = UiaCore.UIA.GroupControlTypeId;
            Assert.Equal(expected, controlType);

            Assert.True((bool)accessibleObject.GetPropertyValue(UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId));
        }

        [WinFormsFact]
        public void ListViewGroupAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly()
        {
            using ListView list = new ListView();
            ListViewGroup listGroup = new ListViewGroup("Group1");
            ListViewItem listItem1 = new ListViewItem();
            ListViewItem listItem2 = new ListViewItem();
            ListViewItem listItem3 = new ListViewItem();
            list.Groups.Add(listGroup);
            listItem1.Group = listGroup;
            listItem2.Group = listGroup;
            list.Items.Add(listItem1);
            list.Items.Add(listItem2);
            list.Items.Add(listItem3);
            AccessibleObject group1AccObj = listGroup.AccessibilityObject;
            AccessibleObject defaultGroupAccObj = list.DefaultGroup.AccessibilityObject;
            Assert.False(list.IsHandleCreated);

            // Next/Previous siblings test
            Assert.Null(defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            UiaCore.IRawElementProviderFragment defaultGroupNextSibling = defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            Assert.IsType<ListViewGroupAccessibleObject>(group1AccObj);
            Assert.Equal(group1AccObj, defaultGroupNextSibling);

            Assert.Null(group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            UiaCore.IRawElementProviderFragment group1PreviousSibling = group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.IsType<ListViewGroupAccessibleObject>(group1PreviousSibling);
            Assert.Equal(defaultGroupAccObj, group1PreviousSibling);

            // Parent
            Assert.Equal(defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.Parent), list.AccessibilityObject);
            Assert.Equal(group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.Parent), list.AccessibilityObject);

            // Childs
            AccessibleObject firstChild = group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewItemAccessibleObject>(firstChild);
            Assert.IsType<ListViewItemAccessibleObject>(lastChild);
            Assert.NotEqual(firstChild, lastChild);
            Assert.Equal(firstChild, listItem1.AccessibilityObject);
            Assert.Equal(lastChild, listItem2.AccessibilityObject);

            firstChild = defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            lastChild = defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewItemAccessibleObject>(firstChild);
            Assert.IsType<ListViewItemAccessibleObject>(lastChild);
            Assert.Equal(firstChild, lastChild);
            Assert.Equal(firstChild, listItem3.AccessibilityObject);
        }

        [WinFormsFact]
        public void ListViewGroupAccessibleObject_Bounds_ReturnsCorrectValue()
        {
            using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
            {
                Control.CheckForIllegalCrossThreadCalls = true;
                using Form form = new Form();

                using ListView list = new ListView();
                ListViewGroup listGroup = new ListViewGroup("Group1");
                ListViewItem listItem1 = new ListViewItem("Item1");
                ListViewItem listItem2 = new ListViewItem("Item2");
                list.Groups.Add(listGroup);
                listItem1.Group = listGroup;
                listItem2.Group = listGroup;
                list.Items.Add(listItem1);
                list.Items.Add(listItem2);
                list.CreateControl();
                form.Controls.Add(list);
                form.Show();

                AccessibleObject accessibleObject = list.AccessibilityObject;
                AccessibleObject group1AccObj = listGroup.AccessibilityObject;
                Assert.True(list.IsHandleCreated);

                RECT groupRect = new RECT();
                User32.SendMessageW(list, (User32.WM)ComCtl32.LVM.GETGROUPRECT, (IntPtr)list.Groups.IndexOf(listGroup), ref groupRect);

                int actualWidth = group1AccObj.Bounds.Width;
                int expectedWidth = groupRect.Width;
                Assert.Equal(expectedWidth, actualWidth);

                int actualHeight = group1AccObj.Bounds.Height;
                int expectedHeight = groupRect.Height;
                Assert.Equal(expectedHeight, actualHeight);

                Rectangle actualBounds = group1AccObj.Bounds;
                actualBounds.Location = new Point(0, 0);
                Rectangle expectedBounds = groupRect;
                Assert.Equal(expectedBounds, actualBounds);
            });
        }
    }
}
