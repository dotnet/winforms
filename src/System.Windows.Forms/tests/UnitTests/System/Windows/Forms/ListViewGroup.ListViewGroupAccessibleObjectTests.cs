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
        public void ListViewGroupAccessibleObject_GetPropertyValue_ReturnsExpected_WithoutDefaultGroup()
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
            Assert.False(list.IsHandleCreated);
        }

        [WinFormsFact]
        public void ListViewGroupAccessibleObject_GetPropertyValue_ReturnsExpected_WithDefaultGroup()
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView list = new ListView();
            ListViewGroup listGroup = new ListViewGroup("Group1");
            listGroup.Items.Add(new ListViewItem());
            list.Items.Add(new ListViewItem());
            list.Groups.Add(listGroup);

            AccessibleObject defaultGroupAccessibleObject = list.DefaultGroup.AccessibilityObject;
            AccessibleObject groupAccessibleObject = listGroup.AccessibilityObject;

            Assert.Equal(list.DefaultGroup.Header, defaultGroupAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId));
            Assert.Equal("Group1", groupAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId));

            Assert.Equal("ListViewGroup-0", defaultGroupAccessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId));
            Assert.Equal("ListViewGroup-1", groupAccessibleObject.GetPropertyValue(UiaCore.UIA.AutomationIdPropertyId));

            Assert.Equal(UiaCore.UIA.GroupControlTypeId, groupAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
            Assert.Equal(UiaCore.UIA.GroupControlTypeId, defaultGroupAccessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));

            Assert.True((bool)defaultGroupAccessibleObject.GetPropertyValue(UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId));
            Assert.True((bool)groupAccessibleObject.GetPropertyValue(UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId));

            Assert.False(list.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_FragmentNavigate_ReturnsExpected_WithDefaultGroup(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 6,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
            };

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem(listGroup1);
            ListViewItem listItem3 = new ListViewItem();
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            ListViewItem listItem5 = new ListViewItem(listGroup2);
            ListViewItem listItem6 = new ListViewItem(listGroup2);
            ListViewItem listItem7 = new ListViewItem(listGroup2);
            ListViewItem listItem8 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listItem1,
                        1 => listItem2,
                        2 => listItem3,
                        3 => listItem4,
                        4 => listItem5,
                        5 => listItem6,
                        _ => throw new NotImplementedException()
                    };
                };

                listItem1.SetItemIndex(listView, 0);
                listItem2.SetItemIndex(listView, 1);
                listItem3.SetItemIndex(listView, 2);
                listItem4.SetItemIndex(listView, 3);
                listItem5.SetItemIndex(listView, 4);
                listItem6.SetItemIndex(listView, 5);
            }
            else
            {
                listView.Items.Add(listItem1);
                listView.Items.Add(listItem2);
                listView.Items.Add(listItem3);
                listView.Items.Add(listItem4);
                listView.Items.Add(listItem5);
                listView.Items.Add(listItem6);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            if (listView.IsHandleCreated && listView.GroupsDisplayed)
            {
                Assert.Equal(listView.AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Equal(listGroup1.AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Equal(listItem3.AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Equal(listItem3.AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Equal(listView.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Equal(listGroup2.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Equal(listView.DefaultGroup.AccessibilityObject, listView.Groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Equal(listItem1.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Equal(listItem2.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Equal(listView.AccessibilityObject, listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Equal(listGroup1.AccessibilityObject, listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Equal(listItem4.AccessibilityObject, listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Equal(listItem6.AccessibilityObject, listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));
            }
            else
            {
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_FragmentNavigate_ReturnsExpected_WithoutDefaultGroup(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 6,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
            };

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem(listGroup1);
            ListViewItem listItem3 = new ListViewItem(listGroup1);
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            ListViewItem listItem5 = new ListViewItem(listGroup2);
            ListViewItem listItem6 = new ListViewItem(listGroup2);
            ListViewItem listItem7 = new ListViewItem(listGroup2);
            ListViewItem listItem8 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listItem1,
                        1 => listItem2,
                        2 => listItem3,
                        3 => listItem4,
                        4 => listItem5,
                        5 => listItem6,
                        _ => throw new NotImplementedException()
                    };
                };

                listItem1.SetItemIndex(listView, 0);
                listItem2.SetItemIndex(listView, 1);
                listItem3.SetItemIndex(listView, 2);
                listItem4.SetItemIndex(listView, 3);
                listItem5.SetItemIndex(listView, 4);
                listItem6.SetItemIndex(listView, 5);
            }
            else
            {
                listView.Items.Add(listItem1);
                listView.Items.Add(listItem2);
                listView.Items.Add(listItem3);
                listView.Items.Add(listItem4);
                listView.Items.Add(listItem5);
                listView.Items.Add(listItem6);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            if (listView.IsHandleCreated && listView.GroupsDisplayed)
            {
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Equal(listView.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Equal(listGroup2.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listView.Groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Equal(listItem1.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Equal(listItem3.AccessibilityObject, listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Equal(listView.AccessibilityObject, listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Equal(listGroup1.AccessibilityObject, listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Equal(listItem4.AccessibilityObject, listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Equal(listItem6.AccessibilityObject, listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));
            }
            else
            {
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(listGroup1.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));

                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.Parent));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.NextSibling));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.PreviousSibling));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.FirstChild));
                Assert.Null(listGroup2.AccessibilityObject.FragmentNavigate(NavigateDirection.LastChild));
            }
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
        public void ListViewGroupAccessibleObject_GetChildIndex_ReturnsExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 6,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
            };

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem(listGroup1);
            ListViewItem listItem3 = new ListViewItem();
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            ListViewItem listItem5 = new ListViewItem(listGroup2);
            ListViewItem listItem6 = new ListViewItem(listGroup2);
            ListViewItem listItem7 = new ListViewItem(listGroup2);
            ListViewItem listItem8 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listItem1,
                        1 => listItem2,
                        2 => listItem3,
                        3 => listItem4,
                        4 => listItem5,
                        5 => listItem6,
                        _ => throw new NotImplementedException()
                    };
                };

                listItem1.SetItemIndex(listView, 0);
                listItem2.SetItemIndex(listView, 1);
                listItem3.SetItemIndex(listView, 2);
                listItem4.SetItemIndex(listView, 3);
                listItem5.SetItemIndex(listView, 4);
                listItem6.SetItemIndex(listView, 5);
            }
            else
            {
                listView.Items.Add(listItem1);
                listView.Items.Add(listItem2);
                listView.Items.Add(listItem3);
                listView.Items.Add(listItem4);
                listView.Items.Add(listItem5);
                listView.Items.Add(listItem6);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            if (listView.IsHandleCreated && listView.GroupsDisplayed)
            {
                Assert.Equal(0, listGroup1.AccessibilityObject.GetChildIndex(listItem1.AccessibilityObject));
                Assert.Equal(1, listGroup1.AccessibilityObject.GetChildIndex(listItem2.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem3.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem4.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem5.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem6.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem7.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem8.AccessibilityObject));

                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem1.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem2.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem3.AccessibilityObject));
                Assert.Equal(0, listGroup2.AccessibilityObject.GetChildIndex(listItem4.AccessibilityObject));
                Assert.Equal(1, listGroup2.AccessibilityObject.GetChildIndex(listItem5.AccessibilityObject));
                Assert.Equal(2, listGroup2.AccessibilityObject.GetChildIndex(listItem6.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem7.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem8.AccessibilityObject));

                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem1.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem2.AccessibilityObject));
                Assert.Equal(0, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem3.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem4.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem5.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem6.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem7.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem8.AccessibilityObject));
            }
            else
            {
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem1.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem2.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem3.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem4.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem5.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem6.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem7.AccessibilityObject));
                Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(listItem8.AccessibilityObject));

                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem1.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem2.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem3.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem4.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem5.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem6.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem7.AccessibilityObject));
                Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(listItem8.AccessibilityObject));

                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem1.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem2.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem3.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem4.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem5.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem6.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem7.AccessibilityObject));
                Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(listItem8.AccessibilityObject));
            }

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_GetChildIndex_ReturnsMinusOne_IfChildIsNull(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 6,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
            };

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem(listGroup1);
            ListViewItem listItem3 = new ListViewItem();
            ListViewItem listItem4 = new ListViewItem(listGroup2);
            ListViewItem listItem5 = new ListViewItem(listGroup2);
            ListViewItem listItem6 = new ListViewItem(listGroup2);
            ListViewItem listItem7 = new ListViewItem(listGroup2);
            ListViewItem listItem8 = new ListViewItem(listGroup2);
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listItem1,
                        1 => listItem2,
                        2 => listItem3,
                        3 => listItem4,
                        4 => listItem5,
                        5 => listItem6,
                        _ => throw new NotImplementedException()
                    };
                };

                listItem1.SetItemIndex(listView, 0);
                listItem2.SetItemIndex(listView, 1);
                listItem3.SetItemIndex(listView, 2);
                listItem4.SetItemIndex(listView, 3);
                listItem5.SetItemIndex(listView, 4);
                listItem6.SetItemIndex(listView, 5);
            }
            else
            {
                listView.Items.Add(listItem1);
                listView.Items.Add(listItem2);
                listView.Items.Add(listItem3);
                listView.Items.Add(listItem4);
                listView.Items.Add(listItem5);
                listView.Items.Add(listItem6);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            Assert.Equal(-1, listGroup1.AccessibilityObject.GetChildIndex(null));
            Assert.Equal(-1, listGroup2.AccessibilityObject.GetChildIndex(null));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(null));
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_GetChildCount_Invoke_ReturnsExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 6,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
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

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listItem1,
                        1 => listItem2,
                        2 => listItem3,
                        3 => listItem4,
                        4 => listItem5,
                        5 => listItem6,
                        _ => throw new NotImplementedException()
                    };
                };

                listItem1.SetItemIndex(listView, 0);
                listItem2.SetItemIndex(listView, 1);
                listItem3.SetItemIndex(listView, 2);
                listItem4.SetItemIndex(listView, 3);
                listItem5.SetItemIndex(listView, 4);
                listItem6.SetItemIndex(listView, 5);
            }
            else
            {
                listView.Items.Add(listItem1);
                listView.Items.Add(listItem2);
                listView.Items.Add(listItem3);
                listView.Items.Add(listItem4);
                listView.Items.Add(listItem5);
                listView.Items.Add(listItem6);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;
            bool supportsGetChild = listView.IsHandleCreated && listView.GroupsDisplayed;

            Assert.Equal(supportsGetChild ? 2 : -1, group1AccObj.GetChildCount());
            Assert.Equal(supportsGetChild ? 3 : -1, group2AccObj.GetChildCount());
            Assert.Equal(supportsGetChild ? 1 : -1, defaultGroupAccObj.GetChildCount());
            Assert.Equal(createHandle, listView.IsHandleCreated);
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

        public static IEnumerable<object[]> ListViewGroupAccessibleObject_GetChild_Invoke_TestData()
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
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_GetChild_Invoke_ReturnsExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 4,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
            };

            listView.Columns.Add(new ColumnHeader());

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

            if (virtualMode)
            {
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

                listItem1.SetItemIndex(listView, 0);
                listItem2.SetItemIndex(listView, 1);
                listItem3.SetItemIndex(listView, 2);
                listItem4.SetItemIndex(listView, 3);
            }
            else
            {
                listView.Items.Add(listItem1);
                listView.Items.Add(listItem2);
                listView.Items.Add(listItem3);
                listView.Items.Add(listItem4);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)listGroup1.AccessibilityObject;
            ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)listGroup2.AccessibilityObject;
            ListViewGroupAccessibleObject group3AccObj = (ListViewGroupAccessibleObject)listGroup3.AccessibilityObject;
            ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;
            bool supportsGetChild = listView.IsHandleCreated && listView.GroupsDisplayed;

            Assert.Null(group1AccObj.GetChild(-1));
            Assert.Null(group1AccObj.GetChild(1));

            Assert.Equal(supportsGetChild ? listItem1.AccessibilityObject : null, group1AccObj.GetChild(0));

            Assert.Null(group2AccObj.GetChild(-1));
            Assert.Null(group2AccObj.GetChild(2));
            Assert.Equal(supportsGetChild ? listItem3.AccessibilityObject : null, group2AccObj.GetChild(0));
            Assert.Equal(supportsGetChild ? listItem4.AccessibilityObject : null, group2AccObj.GetChild(1));

            Assert.Null(group3AccObj.GetChild(-1));
            Assert.Null(group3AccObj.GetChild(0));

            Assert.Null(defaultGroupAccObj.GetChild(-1));
            Assert.Null(defaultGroupAccObj.GetChild(1));
            Assert.Equal(supportsGetChild ? listItem2.AccessibilityObject : null, defaultGroupAccObj.GetChild(0));
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

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_InvisibleGroups(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            AccessibleObject listViewGroupWithItems1 = listView.Groups[1].AccessibilityObject;
            AccessibleObject listViewGroupWithItems2 = listView.Groups[2].AccessibilityObject;

            Assert.Null(listViewGroupWithItems1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(listViewGroupWithItems2, listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(listViewGroupWithItems1, listViewGroupWithItems2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(listViewGroupWithItems2.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_FragmentNavigate_ReturnsExpected_Sibling_InvisibleGroups_AfterAddingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;

            Assert.Null(GetAccessibleObject(1).FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NextSibling));

            ListViewItem listViewItem1 = new();
            ListViewItem listViewItem2 = new();
            listView.Items.Add(listViewItem1);
            listView.Items.Add(listViewItem2);
            listView.Groups[0].Items.Add(listViewItem1);
            listView.Groups[3].Items.Add(listViewItem2);

            Assert.Null(GetAccessibleObject(0).FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(0).FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(GetAccessibleObject(0), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(GetAccessibleObject(3), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(3).FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(GetAccessibleObject(3).FragmentNavigate(NavigateDirection.NextSibling));
            Assert.True(listView.IsHandleCreated);

            AccessibleObject GetAccessibleObject(int index) => listView.Groups[index].AccessibilityObject;
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_InvisibleGroups_AfterRemovingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithEmptyGroups(view);
            AccessibleObject accessibleObject = listView.AccessibilityObject;
            AccessibleObject listViewGroupWithItems1 = listView.Groups[1].AccessibilityObject;
            AccessibleObject listViewGroupWithItems2 = listView.Groups[2].AccessibilityObject;

            Assert.Null(listViewGroupWithItems1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Equal(listViewGroupWithItems2, listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.Equal(listViewGroupWithItems1, listViewGroupWithItems2.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(listViewGroupWithItems2.FragmentNavigate(NavigateDirection.NextSibling));

            listView.Groups[2].Items.RemoveAt(0);

            Assert.Equal(listView.DefaultGroup.AccessibilityObject, listViewGroupWithItems1.FragmentNavigate(NavigateDirection.PreviousSibling));
            Assert.Null(listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NextSibling));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_FragmentNavigate_Child_ReturnsExpected_InvisibleItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_FragmentNavigate_Child_ReturnsExpected_InvisibleItems_AfterAddingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

            listView.Items.Add(listView.Groups[0].Items[0]);
            listView.Items.Add(listView.Groups[0].Items[3]);

            Assert.Equal(listView.Groups[0].Items[0].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(listView.Groups[0].Items[3].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_FragmentNavigate_Child_ReturnsExpected_InvisibleItems_AfterRemovingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));

            listView.Items.RemoveAt(1);

            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.FirstChild));
            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.LastChild));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_GetChildCount_ReturnsExpected_InvisibleItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            Assert.Equal(2, accessibleObject.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_GetChildCount_ReturnsExpected_InvisibleItems_AfterAddingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            Assert.Equal(2, accessibleObject.GetChildCount());

            listView.Items.Add(listView.Groups[0].Items[0]);
            listView.Items.Add(listView.Groups[0].Items[3]);

            Assert.Equal(4, accessibleObject.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_GetChildCount_ReturnsExpected_InvisibleItems_AfterRemovingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            Assert.Equal(2, accessibleObject.GetChildCount());

            listView.Items.RemoveAt(1);

            Assert.Equal(1, accessibleObject.GetChildCount());

            listView.Items.RemoveAt(0);

            Assert.Equal(0, accessibleObject.GetChildCount());
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_GetChild_ReturnsExpected_InvisibleItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_GetChild_ReturnsExpected_InvisibleItems_AfterAddingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            listView.Items.Add(listView.Groups[0].Items[0]);
            listView.Items.Add(listView.Groups[0].Items[3]);

            Assert.Equal(listView.Groups[0].Items[0].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.GetChild(2));
            Assert.Equal(listView.Groups[0].Items[3].AccessibilityObject, accessibleObject.GetChild(3));
            Assert.Null(accessibleObject.GetChild(4));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_GetChild_ReturnsExpected_InvisibleItems_AfterRemovingItems(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewItemWithInvisibleItems(view);
            AccessibleObject accessibleObject = listView.Groups[0].AccessibilityObject;

            listView.Items.RemoveAt(1);

            Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Null(accessibleObject.GetChild(1));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
        public void ListViewGroupAccessibleObject_FragmentRoot_Returns_ListViewAccessibleObject(View view, bool showGroups, bool createHandle)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
            };

            ListViewGroup listViewGroup = new();
            listView.Groups.Add(listViewGroup);

            if (createHandle)
            {
                listView.CreateControl();
            }

            Assert.Equal(listView.AccessibilityObject, listViewGroup.AccessibilityObject.FragmentRoot);
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroup_GroupAddedWithItem_AccessibleObject_TestData))]
        public void ListViewGroupAccessibleObject_Bounds_ReturnsExpected(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = GetListViewWithGroups(view, showGroups, createHandle, virtualMode);
            ListView.ListViewAccessibleObject accessibleObject = listView.AccessibilityObject as ListView.ListViewAccessibleObject;
            bool showBounds = listView.IsHandleCreated && listView.GroupsDisplayed;

            Assert.Equal(showBounds, !listView.DefaultGroup.AccessibilityObject.Bounds.IsEmpty);
            Assert.Equal(showBounds, !listView.Groups[0].AccessibilityObject.Bounds.IsEmpty);
            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_Bounds_LocatedInsideListViewBounds(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = GetListViewWithGroups(view, showGroups: true, createHandle: true, virtualMode: false);
            ListView.ListViewAccessibleObject accessibleObject = listView.AccessibilityObject as ListView.ListViewAccessibleObject;
            Rectangle listViewBounds = listView.AccessibilityObject.Bounds;

            Assert.True(listViewBounds.Contains(listView.DefaultGroup.AccessibilityObject.Bounds));
            Assert.True(listViewBounds.Contains(listView.Groups[0].AccessibilityObject.Bounds));
            Assert.True(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(View.Details)]
        [InlineData(View.LargeIcon)]
        [InlineData(View.SmallIcon)]
        [InlineData(View.Tile)]
        public void ListViewGroupAccessibleObject_Bounds_ReturnEmptyRectangle_ForEmptyGroup(View view)
        {
            if (!Application.UseVisualStyles)
            {
                return;
            }

            using ListView listView = new ListView() { View = view, ShowGroups = true };
            listView.Columns.Add(new ColumnHeader());
            listView.CreateControl();
            listView.Groups.Add(new ListViewGroup());
            listView.Items.Add(new ListViewItem());

            Assert.False(listView.DefaultGroup.AccessibilityObject.Bounds.IsEmpty);
            Assert.True(listView.Groups[0].AccessibilityObject.Bounds.IsEmpty);
            Assert.True(listView.IsHandleCreated);
        }

        private ListView GetListViewItemWithEmptyGroups(View view)
        {
            ListView listView = new ListView() { View = view };
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

        [WinFormsTheory]
        [MemberData(nameof(ListViewGroupAccessibleObject_GetChild_Invoke_TestData))]
        public void ListViewGroupAccessibleObject_SetFocus_WorksCorrectly(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            using ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 3,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
            };

            ListViewGroup listGroup1 = new ListViewGroup("Group1");
            ListViewGroup listGroup2 = new ListViewGroup("Group2");
            ListViewItem listItem1 = new ListViewItem(listGroup1);
            ListViewItem listItem2 = new ListViewItem(listGroup1);
            ListViewItem listItem3 = new ListViewItem();
            listView.Groups.Add(listGroup1);
            listView.Groups.Add(listGroup2);

            if (virtualMode)
            {
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
            }
            else
            {
                listView.Items.Add(listItem1);
                listView.Items.Add(listItem2);
                listView.Items.Add(listItem3);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            bool setFocusSupported = listView.IsHandleCreated && listView.GroupsDisplayed;

            listGroup2.AccessibilityObject.SetFocus();

            Assert.Null(listView.FocusedGroup);

            listView.DefaultGroup.AccessibilityObject.SetFocus();

            Assert.Equal(setFocusSupported ? listView.DefaultGroup : null, listView.FocusedGroup);

            listGroup1.AccessibilityObject.SetFocus();

            Assert.Equal(setFocusSupported ? listGroup1 : null, listView.FocusedGroup);

            Assert.Equal(createHandle, listView.IsHandleCreated);
        }

        private ListView GetListViewItemWithInvisibleItems(View view)
        {
            ListView listView = new ListView() { View = view };
            listView.CreateControl();
            ListViewGroup listViewGroup = new("Test group");
            ListViewItem listViewInvisibleItem1 = new ListViewItem("Invisible item 1");
            ListViewItem listViewVisibleItem1 = new ListViewItem("Visible item 1");
            ListViewItem listViewInvisibleItem2 = new ListViewItem("Invisible item 1");
            ListViewItem listViewVisibleItem2 = new ListViewItem("Visible item 1");

            listView.Groups.Add(listViewGroup);
            listView.Items.AddRange(new ListViewItem[] { listViewVisibleItem1, listViewVisibleItem2 });
            listViewGroup.Items.AddRange(new ListViewItem[]
            {
                listViewInvisibleItem1, listViewVisibleItem1,
                listViewVisibleItem2, listViewInvisibleItem2
            });

            return listView;
        }

        private ListView GetListViewWithGroups(View view, bool showGroups, bool createHandle, bool virtualMode)
        {
            ListView listView = new()
            {
                View = view,
                ShowGroups = showGroups,
                VirtualListSize = 2,
                VirtualMode = virtualMode,
                Size = new Size(200, 200)
            };

            listView.Columns.Add(new ColumnHeader());

            var listViewGroup = new ListViewGroup("Test Group");
            listView.Groups.Add(listViewGroup);
            var listViewItem1 = new ListViewItem("Test item 1", listViewGroup);
            var listViewItem2 = new ListViewItem("Test item 2");

            if (virtualMode)
            {
                listView.RetrieveVirtualItem += (s, e) =>
                {
                    e.Item = e.ItemIndex switch
                    {
                        0 => listViewItem1,
                        1 => listViewItem2,
                        _ => throw new NotImplementedException()
                    };
                };

                listViewItem1.SetItemIndex(listView, 0);
                listViewItem2.SetItemIndex(listView, 1);
            }
            else
            {
                listView.Items.Add(listViewItem1);
                listView.Items.Add(listViewItem2);
            }

            if (createHandle)
            {
                Assert.NotEqual(IntPtr.Zero, listView.Handle);
            }

            return listView;
        }
    }
}
