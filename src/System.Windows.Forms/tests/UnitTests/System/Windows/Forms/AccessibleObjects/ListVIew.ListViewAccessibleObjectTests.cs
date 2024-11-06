// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewGroup;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListView_ListViewAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewAccessibleObject_Ctor_Default()
    {
        using ListView listView = new();

        AccessibleObject accessibleObject = listView.AccessibilityObject;
        Assert.NotNull(accessibleObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_EmptyList_GetChildCount_ReturnsCorrectValue()
    {
        using ListView listView = new();
        AccessibleObject accessibleObject = listView.AccessibilityObject;
        Assert.Equal(-1, accessibleObject.GetChildCount()); // listView doesn't have items
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, AccessibleRole.List)]
    [InlineData(false, AccessibleRole.None)]
    public void ListViewAccessibleObject_DefaultRole_ReturnsCorrectValue(bool createControl, AccessibleRole expectedAccessibleRole)
    {
        using ListView listView = new();
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
        using ListView listView = new();
        AccessibleObject accessibleObject = listView.AccessibilityObject;
        Assert.Equal((int)listView.View, accessibleObject.GetMultiViewProviderCurrentView());
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_GetMultiViewProviderSupportedViews_ReturnsExpected()
    {
        using ListView listView = new();
        AccessibleObject accessibleObject = listView.AccessibilityObject;
        Assert.Equal(new int[] { (int)View.Details }, accessibleObject.GetMultiViewProviderSupportedViews());
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_GetMultiViewProviderViewName_ReturnsCorrectValue()
    {
        using ListView listView = new();
        listView.View = View.Details;
        AccessibleObject accessibleObject = listView.AccessibilityObject;
        Assert.Equal(((int)(listView.View)).ToString(), accessibleObject.GetMultiViewProviderViewName((int)View.Details));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsCreated_VisualStylesEnabled()
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using ListView listView = new();
        listView.CreateControl();
        listView.Items.Add(new ListViewItem());
        ListViewItem item = new();
        ListViewItem item2 = new();
        ListViewGroup group = new();
        item2.Group = group;
        item.Group = group;
        listView.Groups.Add(group);
        listView.Items.Add(item);
        listView.Items.Add(item2);

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        AccessibleObject firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild) as AccessibleObject;
        AccessibleObject lastChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild) as AccessibleObject;
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

        using ListView listView = new();
        listView.CreateControl();
        listView.Items.Add(new ListViewItem());
        ListViewItem item = new();
        ListViewItem item2 = new();
        ListViewGroup group = new();
        item2.Group = group;
        item.Group = group;
        listView.Groups.Add(group);
        listView.Items.Add(item);
        listView.Items.Add(item2);

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        AccessibleObject firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild) as AccessibleObject;
        AccessibleObject lastChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild) as AccessibleObject;
        Assert.IsAssignableFrom<ListViewItemBaseAccessibleObject>(firstChild);
        Assert.IsAssignableFrom<ListViewItemBaseAccessibleObject>(lastChild);
        Assert.NotEqual(firstChild, lastChild);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_ListWithTwoGroups_FragmentNavigateWorkCorrectly_IfHandleIsNotCreated()
    {
        using ListView listView = new();
        listView.Items.Add(new ListViewItem());
        ListViewItem item = new();
        ListViewItem item2 = new();
        ListViewGroup group = new();
        item2.Group = group;
        item.Group = group;
        listView.Groups.Add(group);
        listView.Items.Add(item);
        listView.Items.Add(item2);

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        AccessibleObject firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild) as AccessibleObject;
        AccessibleObject lastChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild) as AccessibleObject;
        Assert.Null(firstChild);
        Assert.Null(lastChild);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_ListWithTwoItems_FragmentNavigateWorkCorrectly_IfHandleIsCreated()
    {
        using ListView listView = new();
        listView.CreateControl();
        listView.Items.Add(new ListViewItem());
        listView.Items.Add(new ListViewItem());

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        AccessibleObject firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild) as AccessibleObject;
        AccessibleObject lastChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild) as AccessibleObject;
        Assert.IsAssignableFrom<ListViewItemBaseAccessibleObject>(firstChild);
        Assert.IsAssignableFrom<ListViewItemBaseAccessibleObject>(lastChild);
        Assert.NotEqual(firstChild, lastChild);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_ListWithTwoItems_FragmentNavigateWorkCorrectly_IfHandleIsNotCreated()
    {
        using ListView listView = new();
        listView.Items.Add(new ListViewItem());
        listView.Items.Add(new ListViewItem());

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        AccessibleObject firstChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild) as AccessibleObject;
        AccessibleObject lastChild = accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild) as AccessibleObject;
        Assert.Null(firstChild);
        Assert.Null(lastChild);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_EmptyList_GetChild_ReturnsCorrectValue()
    {
        using ListView listView = new();
        AccessibleObject accessibleObject = listView.AccessibilityObject;
        Assert.Equal(-1, accessibleObject.GetChildCount()); // listView doesn't have items
        Assert.Null(accessibleObject.GetChild(0)); // GetChild method should not throw an exception
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_GetPropertyValue_returns_correct_values()
    {
        using ListView list = new();
        list.Name = "List";
        list.AccessibleName = "ListView";
        AccessibleObject listAccessibleObject = list.AccessibilityObject;

        string accessibleName = ((BSTR)listAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree();
        Assert.Equal("ListView", accessibleName);

        string automationId = ((BSTR)listAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree();
        Assert.Equal("List", automationId);

        var accessibleControlType = (UIA_CONTROLTYPE_ID)(int)listAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_ListControlTypeId, accessibleControlType); // If AccessibleRole is Default

        var controlType = (UIA_CONTROLTYPE_ID)(int)listAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_ListControlTypeId;
        Assert.Equal(expected, controlType);

        Assert.True((bool)listAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId));
        Assert.False(list.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewAccessibleObject_OwnerHasDefaultGroup_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                // View.Tile is not supported by ListView in Virtual mode
                if (view == View.Tile && virtualMode)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        foreach (bool createDefaultGroup in new[] { true, false })
                        {
                            yield return new object[] { view, virtualMode, showGroups, createHandle, createDefaultGroup };
                        }
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
    public void ListViewAccessibleObject_OwnerHasDefaultGroup_ReturnsExpected(View view, bool virtualMode, bool showGroups, bool createHandle, bool createDefaultGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 2,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewItem listViewItem1 = new("Test item 1");
        ListViewItem listViewItem2 = new("Test item 2");

        if (!virtualMode)
        {
            ListViewGroup listViewGroup = new("Test Group");
            listView.Groups.Add(listViewGroup);
            listViewItem1.Group = listViewGroup;
            listViewItem2.Group = createDefaultGroup ? null : listViewGroup;
        }

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
            listView.CreateControl();
        }

        bool expected = listView.GroupsDisplayed && createDefaultGroup;
        bool actual = ((ListView.ListViewAccessibleObject)listView.AccessibilityObject).OwnerHasDefaultGroup;

        Assert.Equal(expected, actual);
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewAccessibleObject_OwnerHasDefaultGroup_WithoutItems_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                // View.Tile is not supported by ListView in Virtual mode
                if (view == View.Tile && virtualMode)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createHandle in new[] { true, false })
                    {
                        yield return new object[] { view, virtualMode, showGroups, createHandle };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_WithoutItems_TestData))]
    public void ListViewAccessibleObject_OwnerHasDefaultGroup_ReturnsExpected_WithoutItems(View view, bool virtualMode, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 0,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        if (!virtualMode)
        {
            ListViewGroup listViewGroup = new("Test Group");
            listView.Groups.Add(listViewGroup);
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    _ => throw new NotImplementedException()
                };
            };
        }

        if (createHandle)
        {
            listView.CreateControl();
        }

        bool actual = ((ListView.ListViewAccessibleObject)listView.AccessibilityObject).OwnerHasDefaultGroup;

        Assert.False(actual);
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChildCount_ReturnsExpected(View view, bool virtualMode, bool showGroups, bool createHandle, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 3,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewItem listViewItem1 = new("Test item 1");
        ListViewItem listViewItem2 = new("Test item 2");
        ListViewItem listViewItem3 = new("Test item 3");

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group"));
            listViewItem2.Group = listView.Groups[0];
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
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

        if (createHandle)
        {
            listView.CreateControl();
        }

        int expected = createHandle
            ? listView.GroupsDisplayed ? 2 : 3
            : -1;

        Assert.Equal(expected, listView.AccessibilityObject.GetChildCount());
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChildCount_ReturnsExpected_WithoutItems(View view, bool virtualMode, bool showGroups, bool createHandle, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 0,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group"));
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    _ => throw new NotImplementedException()
                };
            };
        }

        if (createHandle)
        {
            listView.CreateControl();
        }

        Assert.Equal(createHandle ? 0 : -1, listView.AccessibilityObject.GetChildCount());
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewAccessibleObject_GetChild_DefaultGroup_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                // View.Tile is not supported by ListView in Virtual mode
                if (view == View.Tile && virtualMode)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    foreach (bool createDefaultGroup in new[] { true, false })
                    {
                        yield return new object[] { view, virtualMode, showGroups, createDefaultGroup };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_DefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChild_ReturnsExpected_IfHandleCreated(View view, bool virtualMode, bool showGroups, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 3,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewItem listViewItem1 = new("Test item 1");
        ListViewItem listViewItem2 = new("Test item 2");
        ListViewItem listViewItem3 = new("Test item 3");

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group"));
            listViewItem2.Group = listView.Groups[0];
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
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

        listView.CreateControl();

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        Assert.Null(accessibleObject.GetChild(-1));

        if (listView.GroupsDisplayed)
        {
            Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listView.Groups[0].AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
        }
        else
        {
            Assert.Equal(listViewItem1.AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(listViewItem2.AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Equal(listViewItem3.AccessibilityObject, accessibleObject.GetChild(2));
            Assert.Null(accessibleObject.GetChild(3));
        }

        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_DefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChild_ReturnsExpected_IfHandleNotCreated(View view, bool virtualMode, bool showGroups, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 3,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewItem listViewItem1 = new("Test item 1");
        ListViewItem listViewItem2 = new("Test item 2");
        ListViewItem listViewItem3 = new("Test item 3");

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group"));
            listViewItem2.Group = listView.Groups[0];
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
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

        Assert.Null(listView.AccessibilityObject.GetChild(-1));
        Assert.Null(listView.AccessibilityObject.GetChild(0));
        Assert.Null(listView.AccessibilityObject.GetChild(1));

        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChildIndex_ReturnsExpected_WithDefaultGroup(View view, bool virtualMode, bool showGroups, bool createHandle, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 3,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewItem listViewItem1 = new("Test item 1");
        ListViewItem listViewItem2 = new("Test item 2");
        ListViewItem listViewItem3 = new("Test item 3");

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group 1"));
            listView.Groups.Add(new ListViewGroup("Test Group 2"));
            listViewItem2.Group = listView.Groups[0];
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
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

        if (createHandle)
        {
            listView.CreateControl();
        }

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        if (listView.GroupsDisplayed)
        {
            Assert.Equal(0, accessibleObject.GetChildIndex(listView.DefaultGroup.AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Groups[0].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Groups[1].AccessibilityObject));

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[1].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[2].AccessibilityObject));
        }
        else
        {
            Assert.Equal(0, accessibleObject.GetChildIndex(listView.Items[0].AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[1].AccessibilityObject));
            Assert.Equal(2, accessibleObject.GetChildIndex(listView.Items[2].AccessibilityObject));

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.DefaultGroup.AccessibilityObject));
        }

        Assert.Equal(-1, accessibleObject.GetChildIndex(new AccessibleObject()));

        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChildIndex_ReturnsExpected_WithoutDefaultGroup(View view, bool virtualMode, bool showGroups, bool createHandle, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 3,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewItem listViewItem1 = new("Test item 1");
        ListViewItem listViewItem2 = new("Test item 2");
        ListViewItem listViewItem3 = new("Test item 3");

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group 1"));
            listView.Groups.Add(new ListViewGroup("Test Group 2"));
            listViewItem1.Group = listView.Groups[0];
            listViewItem2.Group = listView.Groups[0];
            listViewItem3.Group = listView.Groups[1];
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
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

        if (createHandle)
        {
            listView.CreateControl();
        }

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        if (listView.GroupsDisplayed)
        {
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.DefaultGroup.AccessibilityObject));
            Assert.Equal(0, accessibleObject.GetChildIndex(listView.Groups[0].AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Groups[1].AccessibilityObject));

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[1].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[2].AccessibilityObject));
        }
        else
        {
            Assert.Equal(0, accessibleObject.GetChildIndex(listView.Items[0].AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[1].AccessibilityObject));
            Assert.Equal(2, accessibleObject.GetChildIndex(listView.Items[2].AccessibilityObject));

            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.DefaultGroup.AccessibilityObject));
        }

        Assert.Equal(-1, accessibleObject.GetChildIndex(new AccessibleObject()));

        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChildIndex_ReturnsExpected_WithoutVisibleItems(View view, bool virtualMode, bool showGroups, bool createHandle, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 0,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group 1"));
            listView.Groups[0].Items.Add(new ListViewItem());
            listView.Groups.Add(new ListViewGroup("Test Group 2"));
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    _ => throw new NotImplementedException()
                };
            };
        }

        if (createHandle)
        {
            listView.CreateControl();
        }

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        if (listView.GroupsDisplayed)
        {
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.DefaultGroup.AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Groups[0].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Groups[1].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Groups[0].Items[0].AccessibilityObject));
        }

        Assert.Equal(-1, accessibleObject.GetChildIndex(new AccessibleObject()));

        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_OwnerHasDefaultGroup_TestData))]
    public void ListViewAccessibleObject_GetChildIndex_ReturnsMinusOne_IfChildIsNull(View view, bool virtualMode, bool showGroups, bool createHandle, bool createGroup)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 3,
            VirtualMode = virtualMode
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewItem listViewItem1 = new("Test item 1");
        ListViewItem listViewItem2 = new("Test item 2");
        ListViewItem listViewItem3 = new("Test item 3");

        if (!virtualMode && createGroup)
        {
            listView.Groups.Add(new ListViewGroup("Test Group 1"));
            listView.Groups.Add(new ListViewGroup("Test Group 2"));
            listViewItem2.Group = listView.Groups[0];
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem1,
                    1 => listViewItem2,
                    2 => listViewItem3,
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

        if (createHandle)
        {
            listView.CreateControl();
        }

        Assert.Equal(-1, listView.AccessibilityObject.GetChildIndex(null));
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
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups
        };

        listView.CreateControl();

        ListViewGroup listGroup1 = new("Group1");
        ListViewGroup listGroup2 = new("Group2");
        ListViewGroup listGroup3 = new("Group2");
        ListViewItem listItem1 = new(listGroup1);
        ListViewItem listItem2 = new();
        ListViewItem listItem3 = new(listGroup2);
        ListViewItem listItem4 = new(listGroup2);
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
        Assert.Equal(listItem1.AccessibilityObject, listSelection[0]);
        Assert.Equal(listItem2.AccessibilityObject, listSelection[1]);
        Assert.Equal(listItem4.AccessibilityObject, listSelection[2]);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_TestData))]
    public void ListViewAccessibleObject_GetSelectionInvoke_WithoutSelectedItems_ReturnsExpected(View view, bool showGroups)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups
        };

        listView.CreateControl();
        ListViewItem listItem1 = new();
        listView.Items.Add(listItem1);

        var listSelection = listView.AccessibilityObject.GetSelection();
        Assert.Empty(listSelection);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetSelectionInvoke_TestData))]
    public void ListViewAccessibleObject_GetSelectionInvoke_ReturnsExpected_IfHandleNotCreated(View view, bool showGroups)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups
        };

        ListViewItem listItem1 = new();
        listView.Items.Add(listItem1);

        var listSelection = listView.AccessibilityObject.GetSelection();
        Assert.Empty(listSelection);

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

        ListViewItem listItem1 = new(["Item 1", "Item A"], -1);
        ListViewItem listItem2 = new("Group item 2");
        ListViewItem listItem3 = new("Item 3");
        ListViewItem listItem4 = new(["Item 4", "Item B"], -1);

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
        Assert.Equal(listItem1.AccessibilityObject, listSelection[0]);
        Assert.Equal(listItem2.AccessibilityObject, listSelection[1]);
        Assert.Equal(listItem4.AccessibilityObject, listSelection[2]);
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

        ListViewItem listItem1 = new();

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
        Assert.Empty(listSelection);
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

        ListViewItem listItem1 = new();

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
        Assert.Empty(listSelection);

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
        using ListView listView = new();
        listView.AccessibleRole = role;

        var actual = (UIA_CONTROLTYPE_ID)(int)listView.AccessibilityObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = AccessibleRoleControlTypeMap.GetControlType(role);

        Assert.Equal(expected, actual);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewAccessibleObject_HitTest_DoesNotReturnNull_IfHandleIsCreated(View view)
    {
        using ListView listView = new();
        listView.View = view;
        listView.CreateControl();
        listView.Columns.Add(new ColumnHeader("Column 1") { Width = 70 });
        listView.Columns.Add(new ColumnHeader("Column 2") { Width = 70 });
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 1"]));
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 1"]));

        Assert.NotNull(HitTest(listView, GetItemLocation(0)));
        Assert.NotNull(HitTest(listView, GetItemLocation(1)));
        Assert.True(listView.IsHandleCreated);

        Point GetItemLocation(int itemIndex) =>
        listView.PointToScreen(listView.GetItemRect(0, ItemBoundsPortion.Label).Location);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewAccessibleObject_HitTest_ReturnsNull_IfHandleIsNotCreated(View view)
    {
        using ListView listView = new();
        listView.View = view;
        listView.Columns.Add(new ColumnHeader("Column 1") { Width = 70 });
        listView.Columns.Add(new ColumnHeader("Column 2") { Width = 70 });
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 1"]));
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 1"]));

        Point point = new(15, 55);

        Assert.Null(listView.AccessibilityObject.HitTest(point.X, point.Y));

        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_HitTest_ReturnsSubItem_DetailsView()
    {
        using ListView listView = new();
        listView.View = View.Details;
        listView.Size = new Size(200, 200);
        listView.CreateControl();
        listView.Columns.Add(new ColumnHeader("Column 1") { Width = 70 });
        listView.Columns.Add(new ColumnHeader("Column 2") { Width = 70 });
        listView.Columns.Add(new ColumnHeader("Column 3") { Width = 70 });
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 11", "SubItem 12"]));
        listView.Items.Add(new ListViewItem(["Item 2", "SubItem 21", "SubItem 22"]));

        AccessibleObject accessibleObject = listView.AccessibilityObject;

        Assert.Equal(listView.Items[0].SubItems[0].AccessibilityObject, HitTest(listView, GetSubItemLocation(0, 0)));
        Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, HitTest(listView, GetSubItemLocation(0, 1)));
        Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject, HitTest(listView, GetSubItemLocation(0, 2)));
        Assert.Equal(listView.Items[1].SubItems[0].AccessibilityObject, HitTest(listView, GetSubItemLocation(1, 0)));
        Assert.Equal(listView.Items[1].SubItems[1].AccessibilityObject, HitTest(listView, GetSubItemLocation(1, 1)));
        Assert.Equal(listView.Items[1].SubItems[2].AccessibilityObject, HitTest(listView, GetSubItemLocation(1, 2)));

        Point GetSubItemLocation(int itemIndex, int subItemIndex) =>
            listView.PointToScreen(listView.GetSubItemRect(itemIndex, subItemIndex, ItemBoundsPortion.Label).Location);
    }

    [WinFormsFact]
    public void ListViewAccessibleObject_HitTest_ReturnsFakeSubItem_DetailsView()
    {
        using ListView listView = new();
        listView.View = View.Details;
        listView.Size = new Size(200, 200);
        listView.CreateControl();
        listView.Columns.Add(new ColumnHeader("Column 1") { Width = 70 });
        listView.Columns.Add(new ColumnHeader("Column 2") { Width = 70 });
        listView.Columns.Add(new ColumnHeader("Column 3") { Width = 70 });
        listView.Items.Add(new ListViewItem(["Item 1"]));
        listView.Items.Add(new ListViewItem(["Item 2"]));

        Assert.Same(GetDetailsSubItemOrFake(0, 1), HitTest(listView, GetDetailsSubItemOrFake(0, 1).Bounds.Location));
        Assert.Same(GetDetailsSubItemOrFake(0, 2), HitTest(listView, GetDetailsSubItemOrFake(0, 2).Bounds.Location));
        Assert.Same(GetDetailsSubItemOrFake(1, 1), HitTest(listView, GetDetailsSubItemOrFake(1, 1).Bounds.Location));
        Assert.Same(GetDetailsSubItemOrFake(1, 2), HitTest(listView, GetDetailsSubItemOrFake(1, 2).Bounds.Location));

        AccessibleObject GetDetailsSubItemOrFake(int itemIndex, int subItemIndex) =>
            ((ListViewItemDetailsAccessibleObject)listView.Items[itemIndex].AccessibilityObject).GetDetailsSubItemOrFake(subItemIndex);
    }

    public static IEnumerable<object[]> ListViewAccessibleObject_GetChild_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                // View.Tile is not supported by ListView in Virtual mode
                if (virtualMode && view == View.Tile)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { view, showGroups, virtualMode };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_GetChildCount_ReturnsExpected_IfHandleIsNotCreated(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: false, virtualMode, showGroups);

        Assert.Equal(-1, listView.AccessibilityObject.GetChildCount());
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_GetChildCount_ReturnsExpected_IfHandleIsCreated(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: true, virtualMode, showGroups);

        // If the display of groups is allowed, we will return the number of groups (2 - custom group and default group),
        // otherwise the number of elements (4)
        int expectedCount = view != View.List && listView.GroupsEnabled ? 2 : 4;
        Assert.Equal(expectedCount, listView.AccessibilityObject.GetChildCount());
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_GetChild_ReturnsNull_IfHandleIsNotCreated(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: false, virtualMode, showGroups);

        Assert.Null(listView.AccessibilityObject.GetChild(0));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_GetChild_ReturnsNull_IfIndexIsNegative(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: true, virtualMode, showGroups);

        Assert.Null(listView.AccessibilityObject.GetChild(-1));
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_GetChild_ReturnsNull_IfIndexIsWrong(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: true, virtualMode, showGroups);

        Assert.Null(listView.AccessibilityObject.GetChild(10));
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_GetChild_ReturnsExpected_DetailsView(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: true, virtualMode, showGroups);
        List<AccessibleObject> expectedValues = [];

        if (listView.GroupsEnabled && view != View.List)
        {
            expectedValues.Add(listView.DefaultGroup.AccessibilityObject);
            expectedValues.Add(listView.Groups[0].AccessibilityObject);
            expectedValues.Add(null);
        }
        else
        {
            expectedValues.Add(listView.Items[0].AccessibilityObject);
            expectedValues.Add(listView.Items[1].AccessibilityObject);
            expectedValues.Add(listView.Items[2].AccessibilityObject);
        }

        Assert.Equal(expectedValues[0], listView.AccessibilityObject.GetChild(0));
        Assert.Equal(expectedValues[1], listView.AccessibilityObject.GetChild(1));
        Assert.Equal(expectedValues[2], listView.AccessibilityObject.GetChild(2));
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleIsNotCreated(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: false, virtualMode, showGroups);
        AccessibleObject accessibleObject = listView.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_FragmentNavigate_ReturnsNull_IfHandleIsCreated(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: false, virtualMode, showGroups);
        AccessibleObject accessibleObject = listView.AccessibilityObject;

        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewAccessibleObject_GetChild_TestData))]
    public void ListViewAccessibleObject_FragmentNavigate_Child_ReturnsExpected(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetListViewWithData(view, createControl: true, virtualMode, showGroups);

        AccessibleObject expectedFirstChild = listView.GroupsEnabled && view != View.List
            ? listView.DefaultGroup.AccessibilityObject
            : listView.Items[0].AccessibilityObject;

        AccessibleObject expectedLastChild = listView.GroupsEnabled && view != View.List
            ? listView.Groups[0].AccessibilityObject
            : listView.Items[3].AccessibilityObject;

        Assert.Equal(expectedFirstChild, listView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(expectedLastChild, listView.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.True(listView.IsHandleCreated);
    }

    private AccessibleObject HitTest(ListView listView, Point point) =>
        listView.AccessibilityObject.HitTest(point.X, point.Y);

    private ListView GetListViewWithData(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 4,
            VirtualMode = virtualMode
        };

        ListViewGroup listViewGroup = new("Test");
        ListViewItem listItem1 = new(["Test Item 1", "Item A"], -1, listViewGroup);
        ListViewItem listItem2 = new("Group item 2", listViewGroup);
        ListViewItem listItem3 = new("Item 3");
        ListViewItem listItem4 = new(["Test Item 4", "Item B", "Item C", "Item D"], -1);

        if (!virtualMode)
        {
            listView.Groups.Add(listViewGroup);
        }

        listView.Columns.Add(new ColumnHeader() { Name = "Column 1" });
        listView.Columns.Add(new ColumnHeader() { Name = "Column 2" });
        listView.Columns.Add(new ColumnHeader() { Name = "Column 3" });

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

        if (createControl)
        {
            listView.CreateControl();
        }

        return listView;
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
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Equal(listViewGroupWithItems2.AccessibilityObject,
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

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
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Equal(listView.Groups[2].AccessibilityObject,
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        ListViewItem listViewItem1 = new();
        ListViewItem listViewItem2 = new();
        listView.Items.Add(listViewItem1);
        listView.Items.Add(listViewItem2);
        listView.Groups[0].Items.Add(listViewItem1);
        listView.Groups[3].Items.Add(listViewItem2);

        Assert.Equal(listView.Groups[0].AccessibilityObject,
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Equal(listView.Groups[3].AccessibilityObject,
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

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
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Equal(listView.Groups[2].AccessibilityObject,
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        listView.Groups[1].Items.RemoveAt(0);
        listView.Groups[2].Items.RemoveAt(0);

        Assert.Equal(listView.DefaultGroup.AccessibilityObject,
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));

        Assert.Equal(listView.DefaultGroup.AccessibilityObject,
            accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

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
        ListView listView = new() { View = view };
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

    [WinFormsFact]
    public void ListViewAccessibleObject_BoundingRectangle_ReturnsCorrectWidth_IfListViewIsScrollable()
    {
        const int expectedWidth = 100;

        using ListView listView = new()
        {
            Size = new Size(expectedWidth, 150)
        };
        listView.Items.AddRange(Enumerable.Range(0, 11).Select(i => new ListViewItem()).ToArray());
        listView.CreateControl();

        IRawElementProviderFragment.Interface uiaProvider = listView.AccessibilityObject;
        Assert.True(uiaProvider.get_BoundingRectangle(out UiaRect actual).Succeeded);

        Assert.Equal(expectedWidth, actual.width);
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_GetPropertyValue_CanSelectMultiple()
    {
        using ListView listView = new();
        listView.MultiSelect = true;
        listView.CreateControl();

        var listViewAccessibleObject = listView.AccessibilityObject;
        var actual = listViewAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_SelectionCanSelectMultiplePropertyId);
        Assert.True((bool)actual);

        ISelectionProvider.Interface provider = listViewAccessibleObject;
        Assert.True(provider.get_CanSelectMultiple(out BOOL result).Succeeded);
        Assert.True(result);
    }
}
