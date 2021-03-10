// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;
using static System.Windows.Forms.ListViewGroup;
using static System.Windows.Forms.ListViewItem;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class ListViewGroup_ListViewGroupAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
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
        public void ListViewGroupAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsCreated()
        {
            using ListView list = new ListView();
            list.CreateControl();
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
            Assert.True(list.IsHandleCreated);

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
        public void ListViewGroupAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsNotCreated()
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
            Assert.Null(defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.NextSibling));
            Assert.Null(group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling));

            // Parent
            Assert.Null(defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.Null(group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.Parent));

            // Childs
            Assert.Null(group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(group1AccObj.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.Null(defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(defaultGroupAccObj.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
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
        public static IEnumerable<object[]> ListViewGroupAccessibleObject_TestData()
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
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_GetChildCount_Invoke_ReturnExpected_IfHandleCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem(listGroup1);
            ListViewItem listItem3 = new ListViewItem();
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            ListViewItem listItem5 = new ListViewItem(listGroup2);
            ListViewItem listItem6 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
            listView.Items.Add(listItem3);
            listView.Items.Add(listItem4);
            listView.Items.Add(listItem5);
            listView.Items.Add(listItem6);
            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;
            Assert.Equal(2, group1AccObj.GetChildCount());
            Assert.Equal(3, group2AccObj.GetChildCount());
            Assert.Equal(1, defaultGroupAccObj.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_GetChildCount_Invoke_ReturnExpected_IfHandleNotCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem(listGroup1);
            ListViewItem listItem3 = new ListViewItem();
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            ListViewItem listItem5 = new ListViewItem(listGroup2);
            ListViewItem listItem6 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
            listView.Items.Add(listItem3);
            listView.Items.Add(listItem4);
            listView.Items.Add(listItem5);
            listView.Items.Add(listItem6);
            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;
            Assert.Equal(-1, group1AccObj.GetChildCount());
            Assert.Equal(-1, group2AccObj.GetChildCount());
            Assert.Equal(-1, defaultGroupAccObj.GetChildCount());
            Assert.False(listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewGroupAccessibleObject_VirtualMode_TestData()
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
        [MemberData(nameof(ListViewGroupAccessibleObject_VirtualMode_TestData))]
        public void ListViewGroupAccessibleObject_GetChildCount_Invoke_VirtualMode_ReturnExpected(View view, bool showGroups, bool createHandle)
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

            if (createHandle)
            {
                listView.CreateControl();
            }

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
            listItem3.SetItemIndex(listView, 2);
            listItem4.SetItemIndex(listView, 3);

            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listViewGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listViewGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;
            Assert.Equal(-1, group1AccObj.GetChildCount());
            Assert.Equal(-1, group2AccObj.GetChildCount());
            Assert.Equal(-1, defaultGroupAccObj.GetChildCount());
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewGroupAccessibleObject_GetChild_Invoke_TestData()
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
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_GetChild_Invoke_ReturnsExpected_IfHandleCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            Assert.NotEqual(IntPtr.Zero, listView.Handle);

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewGroup listGroup3 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem();
            ListViewItem listItem3 = new ListViewItem(listGroup2);
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);
            listView.Groups.Add(listGroup3);
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
            listView.Items.Add(listItem3);
            listView.Items.Add(listItem4);
            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject group3AccObj = (ListViewGroupAccessibleObject)listGroup3.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;

            Assert.Null(group1AccObj.GetChild(-1));
            Assert.Null(group1AccObj.GetChild(1));
            Assert.Equal(listItem1.AccessibilityObject, group1AccObj.GetChild(0));

            Assert.Null(group2AccObj.GetChild(-1));
            Assert.Null(group2AccObj.GetChild(2));
            Assert.Equal(listItem3.AccessibilityObject, group2AccObj.GetChild(0));
            Assert.Equal(listItem4.AccessibilityObject, group2AccObj.GetChild(1));

            Assert.Null(group3AccObj.GetChild(-1));
            Assert.Null(group3AccObj.GetChild(0));

            Assert.Null(defaultGroupAccObj.GetChild(-1));
            Assert.Null(defaultGroupAccObj.GetChild(1));
            Assert.Equal(listItem2.AccessibilityObject, defaultGroupAccObj.GetChild(0));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_GetChild_Invoke_ReturnsExpected_IfHandleIsNotCreated(View view, bool showGroups)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups
            };

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewGroup listGroup3 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem();
            ListViewItem listItem3 = new ListViewItem(listGroup2);
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);
            listView.Groups.Add(listGroup3);
            listView.Items.Add(listItem1);
            listView.Items.Add(listItem2);
            listView.Items.Add(listItem3);
            listView.Items.Add(listItem4);
            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject group3AccObj = (ListViewGroupAccessibleObject)listGroup3.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;

            Assert.Null(group1AccObj.GetChild(-1));
            Assert.Null(group1AccObj.GetChild(0));
            Assert.Null(group1AccObj.GetChild(1));

            Assert.Null(group2AccObj.GetChild(-1));
            Assert.Null(group2AccObj.GetChild(0));
            Assert.Null(group2AccObj.GetChild(1));
            Assert.Null(group2AccObj.GetChild(2));

            Assert.Null(group3AccObj.GetChild(-1));
            Assert.Null(group3AccObj.GetChild(0));

            Assert.Null(defaultGroupAccObj.GetChild(-1));
            Assert.Null(defaultGroupAccObj.GetChild(0));
            Assert.Null(defaultGroupAccObj.GetChild(1));
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_VirtualMode_TestData))]
        public void ListViewGroupAccessibleObject_GetChild_Invoke_VirtualMode_ReturnExpected(View view, bool showGroups, bool createHandle)
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

            if (createHandle)
            {
                listView.CreateControl();
            }

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 1);
            listItem3.SetItemIndex(listView, 2);
            listItem4.SetItemIndex(listView, 3);

            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listViewGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listViewGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;

            Assert.Null(group1AccObj.GetChild(-1));
            Assert.Null(group1AccObj.GetChild(0));
            Assert.Null(group1AccObj.GetChild(1));
            Assert.Null(group1AccObj.GetChild(2));

            Assert.Null(group2AccObj.GetChild(-1));
            Assert.Null(group2AccObj.GetChild(0));

            Assert.Null(defaultGroupAccObj.GetChild(-1));
            Assert.Null(defaultGroupAccObj.GetChild(0));
            Assert.Null(defaultGroupAccObj.GetChild(1));
            Assert.Null(defaultGroupAccObj.GetChild(2));
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        public static IEnumerable<object[]> ListViewGroup_GroupAddedWithItem_AccessibleObject_TestData()
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
        [MemberData(nameof(ListViewGroup_GroupAddedWithItem_AccessibleObject_TestData))]
        public void ListViewGroup_GroupAddedWithItem_AccessibleObject_DoesntThrowException(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using var listView = new ListView
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 1,
                VirtualMode = virtualMode
            };

            var listViewGroup = new ListViewGroup("Test Group");
            var listViewItem = new ListViewItem("Test item", listViewGroup);

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

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            Assert.Null(listViewGroup.ListView);
            Assert.NotNull(listViewGroup.AccessibilityObject);
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
        public void ListViewGroupAccessibleObject_ExpandCollapseState_ReturnExpected(View view, bool showGroups, bool createHandle)
        {
            using ListView listView = new ListView
            {
                View = view,
                ShowGroups = showGroups,
            };

            var lvgroup1 = new ListViewGroup
            {
                Header = "CollapsibleGroup1",
                CollapsedState = ListViewGroupCollapsedState.Expanded
            };

            listView.Groups.Add(lvgroup1);
            listView.Items.Add(new ListViewItem("Item1", lvgroup1));

            var lvgroup2 = new ListViewGroup
            {
                Header = "CollapsibleGroup2",
                CollapsedState = ListViewGroupCollapsedState.Collapsed
            };

            listView.Groups.Add(lvgroup2);
            listView.Items.Add(new ListViewItem("Item2", lvgroup2));

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            Assert.Equal(ExpandCollapseState.Expanded, lvgroup1.AccessibilityObject.ExpandCollapseState);
            Assert.Equal(ExpandCollapseState.Collapsed, lvgroup2.AccessibilityObject.ExpandCollapseState);
            Assert.Equal(ExpandCollapseState.Expanded, listView.DefaultGroup.AccessibilityObject.ExpandCollapseState);
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }
    }
}
