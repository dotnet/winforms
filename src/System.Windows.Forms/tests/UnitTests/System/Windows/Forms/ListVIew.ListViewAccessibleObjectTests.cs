// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using Xunit;
using static System.Windows.Forms.ListViewGroup;
using static System.Windows.Forms.ListViewItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ListView_ListViewAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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
        public void ListViewAccessibleObject_ListWithTwoGroups_GetChildCount_ReturnsCorrectValue_UseVisualStylesEnabled(bool createdControl, int expectedChildCount)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

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

        [WinFormsTheory]
        [InlineData(true, 3)]
        [InlineData(false, 0)]
        public void ListViewAccessibleObject_ListWithTwoGroups_GetChildCount_ReturnsCorrectValue_UseVisualStylesDisabled(bool createdControl, int expectedChildCount)
        {
            if (Application.UseVisualStyles)
            {
                return;
            }

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
        public void ListViewAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsCreated_VisualStylesEnabled()
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

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
        public void ListViewAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsCreated_VisualStylesDisabled()
        {
            if (Application.UseVisualStyles)
            {
                return;
            }

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
            Assert.IsType<ListViewItemAccessibleObject>(firstChild);
            Assert.IsType<ListViewItemAccessibleObject>(lastChild);
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
            Assert.Equal(UiaCore.UIA.ListControlTypeId, accessibleControlType); // If AccessibleRole is Default

            object controlType = listAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = UiaCore.UIA.ListControlTypeId;
            Assert.Equal(expected, controlType);

            Assert.True((bool)listAccessibleObject.GetPropertyValue(UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId));
            Assert.False(list.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewAccessibleObject_OwnerHasDefaultGroup_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                foreach (bool virtualMode in new[] { true, false })
                {
                    foreach (bool showGroups in new[] { true, false })
                    {
                        foreach (bool createHandle in new[] { true, false })
                        {
                            yield return new object[] { virtualMode, showGroups, createHandle };
                        }
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
        public void ListViewAccessibleObject_OwnerHasDefaultGroup_ReturnsExpected_WithoutItems(bool virtualMode, bool showGroups, bool createHandle)
        {
            using ListView listView = new ListView
            {
                ShowGroups = showGroups,
                VirtualMode = virtualMode
            };

            if (createHandle)
            {
                listView.CreateControl();
            }

            AccessibleObject accessibleObject = listView.AccessibilityObject;

            bool actual = accessibleObject.TestAccessor().Dynamic.OwnerHasDefaultGroup;

            // Since the listview either
            //  1. isn't materialised, or
            //  2. is in virtual mode, or
            //  3. has no items
            // ...it doesn't have the default group
            Assert.False(actual);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewAccessibleObject_OwnerHasDefaultGroup_DefaultMode_WithAllItemsInGroups_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        yield return new object[] { showGroups, createHandle };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_DefaultMode_WithAllItemsInGroups_TestData))]
        public void ListViewAccessibleObject_OwnerHasDefaultGroup_DefaultMode_WithAllItemsInGroups_ReturnsExpected(bool showGroups, bool createHandle)
        {
            using ListView listView = new ListView
            {
                ShowGroups = showGroups,
                VirtualMode = false // default mode
            };

            if (createHandle)
            {
                listView.CreateControl();
            }

            ListViewGroup group1 = new ListViewGroup("First");
            ListViewItem item1 = new ListViewItem(group1);
            ListViewGroup group2 = new ListViewGroup("Second");
            ListViewItem item2 = new ListViewItem(group2);
            listView.Groups.Add(group1);
            listView.Groups.Add(group2);

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            bool actual = accessibleObject.TestAccessor().Dynamic.OwnerHasDefaultGroup;

            Assert.False(actual);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewAccessibleObject_OwnerHasDefaultGroup_DefaultMode_WithItemsNotInGroup_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        bool expected = showGroups;
                        yield return new object[] { showGroups, createHandle, expected };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_DefaultMode_WithItemsNotInGroup_TestData))]
        public void ListViewAccessibleObject_OwnerHasDefaultGroup_DefaultMode_ReturnsExpected_WithItemsNotInGroup(bool showGroups, bool createHandle, bool expected)
        {
            using ListView listView = new ListView
            {
                ShowGroups = showGroups,
                VirtualMode = false // default mode
            };

            if (createHandle)
            {
                listView.CreateControl();
            }

            ListViewGroup group1 = new ListViewGroup("First");
            listView.Groups.Add(group1);

            ListViewItem item1 = new ListViewItem(group1);
            listView.Items.Add(item1);
            ListViewItem item2 = new ListViewItem();
            listView.Items.Add(item2);

            AccessibleObject accessibleObject = listView.AccessibilityObject;
            bool actual = accessibleObject.TestAccessor().Dynamic.OwnerHasDefaultGroup;

            Assert.Equal(expected, actual);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewAccessibleObject_OwnerHasDefaultGroup_VirtualMode_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in Virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        yield return new object[] { showGroups, createHandle };
                    }
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_VirtualMode_TestData))]
        public void ListViewAccessibleObject_OwnerHasDefaultGroup_VirtualMode_ReturnsExpected_WithItems(bool showGroups, bool createHandle)
        {
            using ListView listView = new ListView
            {
                ShowGroups = showGroups,
                VirtualMode = true,
                VirtualListSize = 2 // we can't add items, just indicate how many we have
            };

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => new ListViewItem(),
                    1 => new ListViewItem(),
                    _ => new ListViewItem(),
                };
            };

            if (createHandle)
            {
                listView.CreateControl();
            }

            AccessibleObject accessibleObject = listView.AccessibilityObject;

            // Since the listview is in virtual mode - it doesn't have the default group
            Assert.False(accessibleObject.TestAccessor().Dynamic.OwnerHasDefaultGroup);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewAccessibleObject_GetSelectionInvoke_TestData()
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
        [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_TestData))]
        public void ListViewAccessibleObject_GetSelectionInvoke_ReturnsExpected(View view, bool showGroups)
        {
            using var listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            listView.CreateControl();

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewGroup listGroup3 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem();
            ListViewItem listItem3 = new ListViewItem(listGroup2);
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
            listView.Items.Add(listItem3);
            listView.Items.Add(listItem4);

            listView.Items[0].Selected = true;
            listView.Items[1].Selected = true;
            listView.Items[3].Selected = true;

            var listSelection = listView.AccessibilityObject.GetSelection();
            Assert.Equal(listItem1.AccessibilityObject, (ListViewItemAccessibleObject)listSelection[0]);
            Assert.Equal(listItem2.AccessibilityObject, (ListViewItemAccessibleObject)listSelection[1]);
            Assert.Equal(listItem4.AccessibilityObject, (ListViewItemAccessibleObject)listSelection[2]);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_TestData))]
        public void ListViewAccessibleObject_GetSelectionInvoke_WithoutSelectedItems_ReturnsExpected(View view, bool showGroups)
        {
            using var listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            listView.CreateControl();
            ListViewItem listItem1 = new ListViewItem();
            listView.Items.Add(listItem1);

            var listSelection = listView.AccessibilityObject.GetSelection();
            Assert.Equal(0, listSelection.Length);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_TestData))]
        public void ListViewAccessibleObject_GetSelectionInvoke_ReturnsExpected_IfHandleNotCreated(View view, bool showGroups)
        {
            using var listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            ListViewItem listItem1 = new ListViewItem();
            listView.Items.Add(listItem1);

            var listSelection = listView.AccessibilityObject.GetSelection();
            Assert.Equal(0, listSelection.Length);

            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewAccessibleObject_GetSelectionInvoke_VirtualMode_TestData()
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in Virtual mode
                if (view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { view, showGroups };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_VirtualMode_TestData))]
        public void ListViewAccessibleObject_GetSelectionInvoke_VirtualMode_ReturnsExpected(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups,
                VirtualListSize = 4
            };

            ListViewGroup listViewGroup1 = new ListViewGroup("Test1");
            ListViewGroup listViewGroup2 = new ListViewGroup("Test2");
            listView.Groups.Add(listViewGroup1);
            listView.Groups.Add(listViewGroup2);

            ListViewItem listItem1 = new ListViewItem(new string[] { "Item 1", "Item A" }, -1, listViewGroup1);
            ListViewItem listItem2 = new ListViewItem("Group item 2", listViewGroup1);
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

            listView.CreateControl();
            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
            listItem3.SetItemIndex(listView, 2);
            listItem4.SetItemIndex(listView, 3);

            listView.Items[0].Selected = true;
            listView.Items[1].Selected = true;
            listView.Items[3].Selected = true;

            var listSelection = listView.AccessibilityObject.GetSelection();
            Assert.Equal(listItem1.AccessibilityObject, (ListViewItemAccessibleObject)listSelection[0]);
            Assert.Equal(listItem2.AccessibilityObject, (ListViewItemAccessibleObject)listSelection[1]);
            Assert.Equal(listItem4.AccessibilityObject, (ListViewItemAccessibleObject)listSelection[2]);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_VirtualMode_TestData))]
        public void ListViewAccessibleObject_GetSelectionInvoke_WithoutSelectedItems_VirtualModeReturnsExpected(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups,
                VirtualListSize = 1
            };

            ListViewItem listItem1 = new ListViewItem();

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    _ => throw new NotImplementedException()
                };
            };

            listView.CreateControl();
            listItem1.SetItemIndex(listView, 0);

            var listSelection = listView.AccessibilityObject.GetSelection();
            Assert.Equal(0, listSelection.Length);
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_VirtualMode_TestData))]
        public void ListViewAccessibleObject_GetSelectionInvoke_VirtualMode_ReturnsExpected_IfHandleNotCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                VirtualMode = true,
                ShowGroups = showGroups,
                VirtualListSize = 1
            };

            ListViewItem listItem1 = new ListViewItem();

            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);

            var listSelection = listView.AccessibilityObject.GetSelection();
            Assert.Equal(0, listSelection.Length);

            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ListViewAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ListView listView = new ListView();
            listView.AccessibleRole = role;

            object actual = listView.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_FragmentNavigate_ReturnExpected_InvisibleGroups(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            ListViewGroup listViewGroupWithItems1 = listView.Groups[1];
            ListViewGroup listViewGroupWithItems2 = listView.Groups[2];
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(listViewGroupWithItems1.AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Equal(listViewGroupWithItems2.AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_FragmentNavigate_ReturnExpected_InvisibleGroups_AfterAddingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(listView.Groups[1].AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Equal(listView.Groups[2].AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            ListViewItem listViewItem1 = new();
            ListViewItem listViewItem2 = new();
            listView.Items.Add(listViewItem1);
            listView.Items.Add(listViewItem2);
            listView.Groups[0].Items.Add(listViewItem1);
            listView.Groups[3].Items.Add(listViewItem2);

            Assert.Equal(listView.Groups[0].AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Equal(listView.Groups[3].AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_FragmentNavigate_ReturnExpected_InvisibleGroups_AfterRemovingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(listView.Groups[1].AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Equal(listView.Groups[2].AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            listView.Groups[1].Items.RemoveAt(0);
            listView.Groups[2].Items.RemoveAt(0);

            Assert.Equal(listView.DefaultGroup.AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));

            Assert.Equal(listView.DefaultGroup.AccessibilityObject,
                accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_GetChildCount_ReturnExpected_InvisibleGroups(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(2, accessibleObject.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_GetChildCount_ReturnExpected_InvisibleGroups_AfterAddingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(2, accessibleObject.GetChildCount());

            ListViewItem listViewItem1 = new();
            ListViewItem listViewItem2 = new();
            listView.Items.Add(listViewItem1);
            listView.Items.Add(listViewItem2);
            listView.Groups[0].Items.Add(listViewItem1);
            listView.Groups[3].Items.Add(listViewItem2);

            Assert.Equal(4, accessibleObject.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_GetChildCount_ReturnExpected_InvisibleGroups_AfterRemovingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(2, accessibleObject.GetChildCount());

            listView.Groups[1].Items.RemoveAt(0);
            listView.Groups[2].Items.RemoveAt(0);

            Assert.Equal(1, accessibleObject.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_GetChildCount_ReturnExpected_GroupWithInvalidAccessibleObject(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            Assert.Equal(2, accessibleObject.GetChildCount());

            listView.Groups[1].TestAccessor().Dynamic._accessibilityObject = new AccessibleObject();

            Assert.Equal(1, accessibleObject.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_GetChild_ReturnExpected_InvisibleGroups(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            ListViewGroup listViewGroupWithItems1 = listView.Groups[1];
            ListViewGroup listViewGroupWithItems2 = listView.Groups[2];
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(listViewGroupWithItems1.AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listViewGroupWithItems2.AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_GetChild_ReturnExpected_InvisibleGroups_AfterAddingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(listView.Groups[1].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Groups[2].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));

            ListViewItem listViewItem1 = new();
            ListViewItem listViewItem2 = new();
            listView.Items.Add(listViewItem1);
            listView.Items.Add(listViewItem2);
            listView.Groups[0].Items.Add(listViewItem1);
            listView.Groups[3].Items.Add(listViewItem2);

            Assert.Equal(listView.Groups[0].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Groups[1].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Equal(listView.Groups[2].AccessibilityObject, accessibleObject.GetChild(2));
            Assert.Equal(listView.Groups[3].AccessibilityObject, accessibleObject.GetChild(3));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewAccessibleObject_GetChild_ReturnExpected_InvisibleGroups_AfterRemovingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Equal(listView.Groups[1].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Groups[2].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));

            listView.Groups[1].Items.RemoveAt(0);
            listView.Groups[2].Items.RemoveAt(0);

            Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
            Assert.True(listView.IsHandleCreated);
        }

        private ListView GetListViewItemWithEmptyGroups(View view)
        {
            ListView listView = new ListView() { View = view};
            listView.CreateControl();
            ListViewGroup listViewGroupWithoutItems = new("Group without items");
            ListViewGroup listViewGroupWithItems1 = new("Group with item 1");
            ListViewGroup listViewGroupWithItems2 = new("Group with item 2");
            ListViewGroup listViewGroupWithInvisibleItems = new("Group with invisible item");
            listView.Groups.Add(listViewGroupWithoutItems);
            listView.Groups.Add(listViewGroupWithItems1);
            listView.Groups.Add(listViewGroupWithItems2);
            listView.Groups.Add(listViewGroupWithInvisibleItems);
            ListViewItem listViewItem1 = new();
            ListViewItem listViewItem2 = new();
            ListViewItem listViewItem3 = new();
            listView.Items.Add(listViewItem1);
            listView.Items.Add(listViewItem2);
            listViewGroupWithItems1.Items.Add(listViewItem1);
            listViewGroupWithItems2.Items.Add(listViewItem2);
            listViewGroupWithInvisibleItems.Items.Add(listViewItem3);

            return listView;
        }
    }
}

