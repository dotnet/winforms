// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Xunit;
using static System.Windows.Forms.ListViewItem;
using static System.Windows.Forms.ListViewItem.ListViewSubItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListViewItem_ListViewItemAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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
            Assert.False(list.IsHandleCreated);
            Assert.NotNull(accessibleObject);
            Assert.Equal(AccessibleRole.ListItem, accessibleObject.Role);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_InGroup_Ctor_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        yield return new object[] { view, showGroups, createHandle };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_InGroup_Ctor_TestData))]
        public void ListViewItemAccessibleObject_InGroup_Ctor(View view, bool showGroups, bool createHandle)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            ListViewGroup listviewGroup = new ListViewGroup();
            ListViewItem listViewItem1 = new ListViewItem();
            ListViewItem listViewItem2 = new ListViewItem(listviewGroup);
            listviewGroup.Items.Add(listViewItem1);
            listView.Groups.Add(listviewGroup);
            listView.Items.Add(listViewItem2);

            AccessibleObject accessibleObject1 = listViewItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listViewItem2.AccessibilityObject;
            Assert.NotNull(accessibleObject1);
            Assert.NotNull(accessibleObject2);
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemAccessibleObject_GetPropertyValue_ReturnsExpected()
        {
            using var list = new ListView();
            ListViewItem listItem = new ListViewItem("ListItem");
            list.Items.Add(listItem);
            AccessibleObject listItemAccessibleObject = listItem.AccessibilityObject;

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
            Assert.False(list.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { view, showGroups };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_TestData))]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_ReturnsExpected_IfHandleCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewGroup listViewGroup = new ListViewGroup("Test");
            ListViewItem listItem1 = new ListViewItem(new string[] { "Item 1", "Item A" }, -1, listViewGroup);
            ListViewItem listItem2 = new ListViewItem("Group item 2", listViewGroup);
            ListViewItem listItem3 = new ListViewItem("Item 3");
            ListViewItem listItem4 = new ListViewItem(new string[] { "Item 4", "Item B" }, -1);

            listView.Groups.Add(listViewGroup);
            listView.Items.AddRange(new ListViewItem[] { listItem1, listItem2, listItem3, listItem4 });

            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;
            AccessibleObject accessibleObject4 = listItem4.AccessibilityObject;

            Assert.Equal(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling), accessibleObject2);
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewGroup.AccessibilityObject);
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Equal(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling), accessibleObject1);
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewGroup.AccessibilityObject);
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            AccessibleObject firstChildItem3 = accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChildItem3 = accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChildItem3);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChildItem3);
            Assert.Equal(firstChildItem3, lastChildItem3);

            Assert.Null(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            AccessibleObject firstChildItem2 = accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChildItem2 = accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChildItem2);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChildItem2);
            Assert.NotEqual(firstChildItem2, lastChildItem2);

            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_TestData))]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_ReturnsExpected_IfHandleNotCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            ListViewGroup listViewGroup = new ListViewGroup("Test");
            ListViewItem listItem1 = new ListViewItem(new string[] { "Item 1", "Item A" }, -1, listViewGroup);
            ListViewItem listItem2 = new ListViewItem("Group item 2", listViewGroup);
            ListViewItem listItem3 = new ListViewItem("Item 3");
            ListViewItem listItem4 = new ListViewItem(new string[] { "Item 4", "Item B" }, -1);

            listView.Groups.Add(listViewGroup);
            listView.Items.AddRange(new ListViewItem[] { listItem1, listItem2, listItem3, listItem4 });

            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;
            AccessibleObject accessibleObject4 = listItem4.AccessibilityObject;

            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewGroup.AccessibilityObject);
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewGroup.AccessibilityObject);
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            AccessibleObject firstChildItem3 = accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChildItem3 = accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChildItem3);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChildItem3);
            Assert.Equal(firstChildItem3, lastChildItem3);

            Assert.Null(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            AccessibleObject firstChildItem2 = accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChildItem2 = accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChildItem2);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChildItem2);
            Assert.NotEqual(firstChildItem2, lastChildItem2);

            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_VirtualMode_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        yield return new object[] { view, showGroups, createHandle };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_VirtualMode_TestData))]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListGroupWithItems_VirtualMode_ReturnsExpected(View view, bool showGroups, bool createHandle)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups,
                VirtualListSize = 4
            };

            ListViewGroup listViewGroup = new ListViewGroup("Test");
            listView.Groups.Add(listViewGroup);

            ListViewItem listItem1 = new ListViewItem(new string[] { "Item 1", "Item A" }, -1, listViewGroup);
            ListViewItem listItem2 = new ListViewItem("Group item 2", listViewGroup);
            ListViewItem listItem3 = new ListViewItem("Item 3");
            ListViewItem listItem4 = new ListViewItem(new string[] { "Item 4", "Item B" }, -1);

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    1 => listItem2,
                    2 => listItem3,
                    3 => listItem4,
                    _ => throw new NotImplementedException()
                };
            };

            if (createHandle)
            {
                listView.CreateControl();
            }

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
            listItem3.SetItemIndex(listView, 2);
            listItem4.SetItemIndex(listView, 3);

            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;
            AccessibleObject accessibleObject4 = listItem4.AccessibilityObject;

            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Equal(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewGroup.AccessibilityObject);
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.Parent), listViewGroup.AccessibilityObject);
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            AccessibleObject firstChildItem3 = accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChildItem3 = accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChildItem3);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChildItem3);
            Assert.Equal(firstChildItem3, lastChildItem3);

            Assert.Null(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Equal(accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            AccessibleObject firstChildItem2 = accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChildItem2 = accessibleObject4.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChildItem2);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChildItem2);
            Assert.NotEqual(firstChildItem2, lastChildItem2);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListWithItems_ReturnsExpected_IfHandleCreated()
        {
            using ListView listView = new ListView();
            listView.CreateControl();
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

            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListWithItems_ReturnsExpected_IfHandleNotCreated()
        {
            using ListView listView = new ListView();
            ListViewItem listItem1 = new ListViewItem(new string[] { "Test A", "Alpha" }, -1);
            ListViewItem listItem2 = new ListViewItem(new string[] { "Test B", "Beta" }, -1);
            ListViewItem listItem3 = new ListViewItem(new string[] { "Test C", "Gamma" }, -1);
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
            listView.Items.Add(listItem3);
            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;

            // List view items
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            // Childs
            AccessibleObject firstChild = accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.FirstChild) as AccessibleObject;
            AccessibleObject lastChild = accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.LastChild) as AccessibleObject;
            Assert.IsType<ListViewSubItemAccessibleObject>(firstChild);
            Assert.IsType<ListViewSubItemAccessibleObject>(lastChild);
            Assert.NotEqual(firstChild, lastChild);

            // Parent
            Assert.Equal(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            Assert.Equal(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);
            Assert.Equal(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.Parent), listView.AccessibilityObject);

            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_View_ShowGroups_VirtualMode_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { view, showGroups };
                    yield return new object[] { view, showGroups };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_View_ShowGroups_VirtualMode_TestData))]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListWithItems_VirtualMode_VirtualListSize1_ReturnsExpected(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups
            };

            listView.VirtualListSize = 1;

            ListViewItem listItem1 = new ListViewItem(new string[] {
                "Test A",
                "Alpha"}, -1);
            ListViewItem listItem2 = new ListViewItem(new string[] {
                "Test B",
                "Beta"}, -1);
            ListViewItem listItem3 = new ListViewItem(new string[] {
                "Test C",
                "Gamma"}, -1);

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    1 => listItem2,
                    2 => listItem3,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
            listItem3.SetItemIndex(listView, 2);

            listView.CreateControl();

            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;

            // First list view item
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            // Second list view item
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            // Third list view item
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

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

            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_View_ShowGroups_VirtualMode_TestData))]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListWithItems_VirtualMode_VirtualListSize3_ReturnsExpected(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups
            };

            listView.VirtualListSize = 3;

            ListViewItem listItem1 = new ListViewItem(new string[] {
                "Test A",
                "Alpha"}, -1);
            ListViewItem listItem2 = new ListViewItem(new string[] {
                "Test B",
                "Beta"}, -1);
            ListViewItem listItem3 = new ListViewItem(new string[] {
                "Test C",
                "Gamma"}, -1);

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    1 => listItem2,
                    2 => listItem3,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
            listItem3.SetItemIndex(listView, 2);

            listView.CreateControl();

            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;

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

            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_View_ShowGroups_VirtualMode_TestData))]
        public void ListViewItemAccessibleObject_FragmentNavigate_ListWithItems_VirtualMode_VirtualListSize3_ReturnsExpected_IfHandleNotCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups
            };

            listView.VirtualListSize = 3;

            ListViewItem listItem1 = new ListViewItem(new string[] {
                "Test A",
                "Alpha"}, -1);
            ListViewItem listItem2 = new ListViewItem(new string[] {
                "Test B",
                "Beta"}, -1);
            ListViewItem listItem3 = new ListViewItem(new string[] {
                "Test C",
                "Gamma"}, -1);

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    1 => listItem2,
                    2 => listItem3,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
            listItem3.SetItemIndex(listView, 2);

            AccessibleObject accessibleObject1 = listItem1.AccessibilityObject;
            AccessibleObject accessibleObject2 = listItem2.AccessibilityObject;
            AccessibleObject accessibleObject3 = listItem3.AccessibilityObject;

            // First list view item
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject1.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            // Second list view item
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

            // Third list view item
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));

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

            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_State_TestData()
        {
            AccessibleStates defaultStates = AccessibleStates.Selectable | AccessibleStates.Focusable | AccessibleStates.MultiSelectable;
            AccessibleStates selectedStates = defaultStates | AccessibleStates.Selected | AccessibleStates.Focused;

            foreach (View view in Enum.GetValues(typeof(View)))
            {
                foreach (bool selected in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        AccessibleStates expectedState = selected ? selectedStates : defaultStates;
                        yield return new object[] { view, selected, expectedState, createHandle };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_State_TestData))]
        public void ListViewItemAccessibleObject_State_ReturnExpected(View view, bool selected, AccessibleStates expectedAcessibleStates, bool createHandle)
        {
            using ListView listView = new ListView
            {
                View = view
            };

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            ListViewItem listItem1 = new ListViewItem(new string[] { "Test A", "Alpha" }, -1);
            listView.Items.Add(listItem1);
            listView.Items[0].Selected = selected;
            ListViewItemAccessibleObject accessibleObject = (ListViewItemAccessibleObject)listView.Items[0].AccessibilityObject;

            Assert.Equal(expectedAcessibleStates, accessibleObject.State);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_State_VirtualMode_TestData()
        {
            AccessibleStates defaultStates = AccessibleStates.Selectable | AccessibleStates.Focusable | AccessibleStates.MultiSelectable;
            AccessibleStates selectedStates = defaultStates | AccessibleStates.Selected | AccessibleStates.Focused;

            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool selected in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        AccessibleStates expectedState = selected ? selectedStates : defaultStates;
                        yield return new object[] { view, selected, expectedState, createHandle };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_State_VirtualMode_TestData))]
        public void ListViewItemAccessibleObject_State_Virtual_ModeReturnExpected(View view, bool selected, AccessibleStates expectedAcessibleStates, bool createHandle)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                VirtualListSize = 1
            };

            ListViewItem listItem1 = new ListViewItem(new string[] { "Test A", "Alpha" }, -1);

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            if (selected)
            {
                listView.Items[0].Selected = true;
            }

            ListViewItemAccessibleObject accessibleObject = (ListViewItemAccessibleObject)listView.Items[0].AccessibilityObject;

            Assert.Equal(expectedAcessibleStates, accessibleObject.State);
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_Bounds_TestData()
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                foreach (View view in Enum.GetValues(typeof(View)))
                {
                    // View.Tile is not supported by ListView in virtual mode
                    if (virtualMode == true && View.Tile == view)
                    {
                        continue;
                    }

                    foreach (bool showGroups in new[] { true, false })
                    {
                        foreach (bool createHandle in new[] { true, false })
                        {
                            yield return new object[] { view, showGroups, createHandle, virtualMode };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_Bounds_TestData))]
        public void ListViewItemAccessibleObject_Bounds_ReturnExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups,
                VirtualMode = virtualMode,
                VirtualListSize = 3
            };

            var lvgroup1 = new ListViewGroup
            {
                Header = "CollapsibleGroup1",
                CollapsedState = ListViewGroupCollapsedState.Expanded
            };

            listView.Groups.Add(lvgroup1);
            var listViewItem1 = new ListViewItem("Item1", lvgroup1);

            var lvgroup2 = new ListViewGroup
            {
                Header = "CollapsibleGroup2",
                CollapsedState = ListViewGroupCollapsedState.Collapsed
            };

            var listViewItem2 = new ListViewItem("Item2", lvgroup2);
            var listViewItem3 = new ListViewItem("Item3");
            listView.Groups.Add(lvgroup2);

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listViewItem1,
                        1 => listViewItem1,
                        2 => listViewItem2,
                        _ => throw new NotImplementedException()
                    };
                };

                listViewItem1.SetItemIndex(listView, 0);
                listViewItem2.SetItemIndex(listView, 1);
                listViewItem3.SetItemIndex(listView, 2);
            }
            else
            {
                listView.Items.Add(listViewItem1);
                listView.Items.Add(listViewItem2);
                listView.Items.Add(listViewItem3);
            }

            listView.Columns.Add(new ColumnHeader());

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            Assert.NotEqual(Rectangle.Empty, listViewItem1.AccessibilityObject.Bounds);
            Assert.Equal(Rectangle.Empty, listViewItem2.AccessibilityObject.Bounds);
            Assert.NotEqual(Rectangle.Empty, listViewItem3.AccessibilityObject.Bounds);
        }

        private void AddItemToListView(ListView listView, ListViewItem listViewItem, bool virtualMode)
        {
            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listViewItem,
                        _ => throw new NotImplementedException()
                    };
                };

                listViewItem.SetItemIndex(listView, 0);
            }
            else
            {
                listView.Items.Add(listViewItem);
            }
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_IsTogglePatternSupported_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile does not support enabled CheckBoxes
                if (View.Tile == view)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        foreach (bool virtualMode in new[] { true, false })
                        {
                            foreach (bool checkboxes in new[] { true, false })
                            {
                                yield return new object[] { view, showGroups, createHandle, virtualMode, checkboxes };
                            }
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_IsTogglePatternSupported_TestData))]
        public void ListViewItemAccessibleObject_IsTogglePatternSupported_ReturnExpected(View view, bool showGroups, bool createHandle, bool virtualMode, bool checkboxesEnabled)
        {
            using ListView listView = new ListView()
            {
                View = view,
                VirtualMode = virtualMode,
                VirtualListSize = 1,
                CheckBoxes = checkboxesEnabled,
                ShowGroups = showGroups
            };

            ListViewItem listViewItem = new ListViewItem("Item");
            AddItemToListView(listView, listViewItem, virtualMode);

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            Assert.Equal(checkboxesEnabled, listViewItem.AccessibilityObject.IsPatternSupported(UiaCore.UIA.TogglePatternId));
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewItemAccessibleObject_ToggleState_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile does not support enabled CheckBoxes
                if (View.Tile == view)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        foreach (bool virtualMode in new[] { true, false })
                        {
                            yield return new object[] { view, showGroups, createHandle, virtualMode };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_ToggleState_TestData))]
        public void ListViewItemAccessibleObject_ToggleState_ReturnExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new ListView()
            {
                View = view,
                VirtualMode = virtualMode,
                VirtualListSize = 1,
                CheckBoxes = true,
                ShowGroups = showGroups
            };

            ListViewItem listViewItem = new ListViewItem("Item");
            AddItemToListView(listView, listViewItem, virtualMode);

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            AccessibleObject listViewItemAccessibleObject = listViewItem.AccessibilityObject;

            Assert.Equal(UiaCore.ToggleState.Off, listViewItemAccessibleObject.ToggleState);

            listViewItem.Checked = true;
            Assert.Equal(UiaCore.ToggleState.On, listViewItemAccessibleObject.ToggleState);

            listViewItem.Checked = false;
            Assert.Equal(UiaCore.ToggleState.Off, listViewItemAccessibleObject.ToggleState);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewItemAccessibleObject_ToggleState_TestData))]
        public void ListViewItemAccessibleObject_Toggle_Invoke(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new ListView()
            {
                View = view,
                VirtualMode = virtualMode,
                VirtualListSize = 1,
                CheckBoxes = true,
                ShowGroups = showGroups
            };

            ListViewItem listViewItem = new ListViewItem("Item");
            AddItemToListView(listView, listViewItem, virtualMode);

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            AccessibleObject listViewItemaAccessibleObject = listViewItem.AccessibilityObject;

            Assert.Equal(UiaCore.ToggleState.Off, listViewItemaAccessibleObject.ToggleState);
            Assert.False(listViewItem.Checked);

            listViewItemaAccessibleObject.Toggle();

            Assert.Equal(UiaCore.ToggleState.On, listViewItemaAccessibleObject.ToggleState);
            Assert.True(listViewItem.Checked);

            // toggle again
            listViewItemaAccessibleObject.Toggle();

            Assert.Equal(UiaCore.ToggleState.Off, listViewItemaAccessibleObject.ToggleState);
            Assert.False(listViewItem.Checked);
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }
    }
}
