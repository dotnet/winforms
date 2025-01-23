// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using Microsoft.DotNet.RemoteExecutor;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.ListViewGroup;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewGroup_ListViewGroupAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewGroupAccessibleObject_Ctor_ThrowsArgumentNullException()
    {
        using ListView list = new();
        ListViewGroup listGroup = new("Group1");
        listGroup.Items.Add(new ListViewItem());
        list.Groups.Add(listGroup);

        Type type = listGroup.AccessibilityObject.GetType();
        ConstructorInfo ctor = type.GetConstructor([typeof(ListViewGroup), typeof(bool)]);
        Assert.NotNull(ctor);
        Assert.Throws<TargetInvocationException>(() => ctor.Invoke([null, false]));

        // group without parent ListView
        ListViewGroup listGroupWithoutList = new("Group2");
        Assert.Throws<TargetInvocationException>(() => ctor.Invoke([listGroupWithoutList, false]));
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_Ctor_Default()
    {
        using ListView list = new();
        ListViewGroup listGroup = new("Group1");
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
        using ListView list = new();
        ListViewGroup listGroup = new("Group1");
        listGroup.Items.Add(new ListViewItem());
        list.Groups.Add(listGroup);

        AccessibleObject accessibleObject = listGroup.AccessibilityObject;
        Assert.False(list.IsHandleCreated);

        string accessibleName = ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree();
        Assert.Equal("Group1", accessibleName);

        string automationId = ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree();
        Assert.Equal("ListViewGroup-0", automationId);

        var controlType = (UIA_CONTROLTYPE_ID)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_GroupControlTypeId;
        Assert.Equal(expected, controlType);

        Assert.True((bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.Equal(AccessibleRole.Grouping, (AccessibleRole)(int)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId));
        Assert.False(list.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_GetPropertyValue_ReturnsExpected_WithDefaultGroup()
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using ListView list = new();
        ListViewGroup listGroup = new("Group1");
        listGroup.Items.Add(new ListViewItem());
        list.Items.Add(new ListViewItem());
        list.Groups.Add(listGroup);

        AccessibleObject defaultGroupAccessibleObject = list.DefaultGroup.AccessibilityObject;
        AccessibleObject groupAccessibleObject = listGroup.AccessibilityObject;

        Assert.Equal(list.DefaultGroup.Header, ((BSTR)defaultGroupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Equal("Group1", ((BSTR)groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree());
        Assert.Equal("Group1", ((BSTR)groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId)).ToStringAndFree());

        Assert.Equal("ListViewGroup-0", ((BSTR)defaultGroupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree());
        Assert.Equal("ListViewGroup-1", ((BSTR)groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree());

        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_GroupControlTypeId, (UIA_CONTROLTYPE_ID)(int)groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));
        Assert.Equal(UIA_CONTROLTYPE_ID.UIA_GroupControlTypeId, (UIA_CONTROLTYPE_ID)(int)defaultGroupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId));

        Assert.True((bool)defaultGroupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.True((bool)groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId));
        Assert.Equal(AccessibleRole.Grouping, (AccessibleRole)(int)defaultGroupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId));
        Assert.Equal(AccessibleRole.Grouping, (AccessibleRole)(int)groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId));
        Assert.Equal(VARIANT.Empty, groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ValueValuePropertyId));
        Assert.Equal("WinForm", ((BSTR)groupAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_FrameworkIdPropertyId)).ToStringAndFree());
        Assert.False(list.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_GetPropertyValue_ReturnsExpected_WithSubtitle()
    {
        using ListView list = new();
        const string name = "Group1";
        const string subtitle = "Subtitle";
        ListViewGroup listGroup = new(name) { Subtitle = subtitle };
        listGroup.Items.Add(new ListViewItem());
        list.Groups.Add(listGroup);

        AccessibleObject accessibleObject = listGroup.AccessibilityObject;
        Assert.False(list.IsHandleCreated);

        string accessibleName = ((BSTR)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree();
        Assert.Equal($"{name}. {subtitle}", accessibleName);

        Assert.False(list.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_FragmentNavigate_ReturnsExpected_WithDefaultGroup(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            Size = new Size(200, 200)
        };

        ListViewGroupCollection groups = listView.Groups;
        ListViewItemCollection items = listView.Items;

        groups.Add(new ListViewGroup("Group1"));
        groups.Add(new ListViewGroup("Group2"));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem());
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        if (listView.IsHandleCreated && listView.GroupsDisplayed)
        {
            Assert.Equal(listView.AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Equal(groups[0].AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(items[2].AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Equal(items[2].AccessibilityObject, listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Equal(listView.AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Equal(groups[1].AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Equal(listView.DefaultGroup.AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(items[0].AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Equal(items[1].AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Equal(listView.AccessibilityObject, groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Equal(groups[0].AccessibilityObject, groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(items[3].AccessibilityObject, groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Equal(items[5].AccessibilityObject, groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        }
        else
        {
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Null(listView.Groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(listView.Groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(listView.Groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(listView.Groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(listView.Groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Null(listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_FragmentNavigate_ReturnsExpected_WithoutDefaultGroup(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            Size = new Size(200, 200)
        };

        ListViewGroupCollection groups = listView.Groups;
        ListViewItemCollection items = listView.Items;
        groups.Add(new ListViewGroup("Group1"));
        groups.Add(new ListViewGroup("Group2"));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        if (listView.IsHandleCreated && listView.GroupsDisplayed)
        {
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Equal(listView.AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Equal(groups[1].AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(items[0].AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Equal(items[2].AccessibilityObject, groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Equal(listView.AccessibilityObject, listView.Groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Equal(groups[0].AccessibilityObject, groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(items[3].AccessibilityObject, groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Equal(items[5].AccessibilityObject, groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        }
        else
        {
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(listView.DefaultGroup.AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Null(groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(groups[0].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

            Assert.Null(groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
            Assert.Null(groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
            Assert.Null(groups[1].AccessibilityObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        }
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_Bounds_ReturnsCorrectValue()
    {
        using RemoteInvokeHandle invokerHandle = RemoteExecutor.Invoke(() =>
        {
            Control.CheckForIllegalCrossThreadCalls = true;
            using Form form = new();

            using ListView list = new();
            ListViewGroup listGroup = new("Group1");
            ListViewItem listItem1 = new("Item1");
            ListViewItem listItem2 = new("Item2");
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

            RECT groupRect = default;
            PInvokeCore.SendMessage(list, PInvoke.LVM_GETGROUPRECT, (WPARAM)listGroup.ID, ref groupRect);

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

        // verify the remote process succeeded
        Assert.Equal(RemoteExecutor.SuccessExitCode, invokerHandle.ExitCode);
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_Bounds_ReturnsCorrectValue_PostHandle()
    {
        using Form form = new();
        using ListView listView = new();

        form.Controls.Add(listView);
        form.Show();

        Assert.True(listView.IsHandleCreated);

        ListViewGroup group = new();
        listView.Groups.Add(group);
        listView.Items.Add(new ListViewItem("a", group));

        RECT groupRect = default;
        PInvokeCore.SendMessage(listView, PInvoke.LVM_GETGROUPRECT, (WPARAM)group.ID, ref groupRect);

        AccessibleObject groupAccObj = group.AccessibilityObject;

        int actualWidth = groupAccObj.Bounds.Width;
        int expectedWidth = groupRect.Width;
        Assert.Equal(expectedWidth, actualWidth);

        int actualHeight = groupAccObj.Bounds.Height;
        int expectedHeight = groupRect.Height;
        Assert.Equal(expectedHeight, actualHeight);

        Rectangle actualBounds = groupAccObj.Bounds;
        actualBounds.Location = new Point(0, 0);
        Rectangle expectedBounds = groupRect;
        Assert.Equal(expectedBounds, actualBounds);
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
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_GetChildIndex_ReturnsExpected(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            Size = new Size(200, 200)
        };

        ListViewGroupCollection groups = listView.Groups;
        ListViewItemCollection items = listView.Items;

        groups.Add(new ListViewGroup("Group1"));
        groups.Add(new ListViewGroup("Group2"));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem());
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));

        ListViewItem itemWithoutListView1 = new(groups[1]);
        ListViewItem itemWithoutListView2 = new(groups[1]);

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        if (listView.IsHandleCreated && listView.GroupsDisplayed)
        {
            Assert.Equal(0, groups[0].AccessibilityObject.GetChildIndex(items[0].AccessibilityObject));
            Assert.Equal(1, groups[0].AccessibilityObject.GetChildIndex(items[1].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[2].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[3].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[4].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[5].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(itemWithoutListView1.AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(itemWithoutListView2.AccessibilityObject));

            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[0].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[1].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[2].AccessibilityObject));
            Assert.Equal(0, groups[1].AccessibilityObject.GetChildIndex(items[3].AccessibilityObject));
            Assert.Equal(1, groups[1].AccessibilityObject.GetChildIndex(items[4].AccessibilityObject));
            Assert.Equal(2, groups[1].AccessibilityObject.GetChildIndex(items[5].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(itemWithoutListView1.AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(itemWithoutListView2.AccessibilityObject));

            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[0].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[1].AccessibilityObject));
            Assert.Equal(0, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[2].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[3].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[4].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[5].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(itemWithoutListView1.AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(itemWithoutListView2.AccessibilityObject));
        }
        else
        {
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[0].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[1].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[2].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[3].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[4].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(items[5].AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(itemWithoutListView1.AccessibilityObject));
            Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(itemWithoutListView2.AccessibilityObject));

            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[0].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[1].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[2].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[3].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[4].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(items[5].AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(itemWithoutListView1.AccessibilityObject));
            Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(itemWithoutListView2.AccessibilityObject));

            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[0].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[1].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[2].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[3].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[4].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(items[5].AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(itemWithoutListView1.AccessibilityObject));
            Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(itemWithoutListView2.AccessibilityObject));
        }

        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_GetChildIndex_ReturnsMinusOne_IfChildIsNull(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            Size = new Size(200, 200)
        };

        ListViewGroupCollection groups = listView.Groups;
        ListViewItemCollection items = listView.Items;

        groups.Add(new ListViewGroup("Group1"));
        groups.Add(new ListViewGroup("Group2"));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem());
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        Assert.Equal(-1, groups[0].AccessibilityObject.GetChildIndex(null));
        Assert.Equal(-1, groups[1].AccessibilityObject.GetChildIndex(null));
        Assert.Equal(-1, listView.DefaultGroup.AccessibilityObject.GetChildIndex(null));
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_GetChildCount_Invoke_ReturnsExpected(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            Size = new Size(200, 200)
        };

        ListViewGroupCollection groups = listView.Groups;
        ListViewItemCollection items = listView.Items;

        groups.Add(new ListViewGroup("Group1"));
        groups.Add(new ListViewGroup("Group2"));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem());
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)groups[0].AccessibilityObject;
        ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)groups[1].AccessibilityObject;
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
                if (virtualMode && view == View.Tile)
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
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_GetChild_Invoke_ReturnsExpected(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            Size = new Size(200, 200)
        };

        listView.Columns.Add(new ColumnHeader());

        ListViewGroupCollection groups = listView.Groups;
        ListViewItemCollection items = listView.Items;

        groups.Add(new ListViewGroup("Group1"));
        groups.Add(new ListViewGroup("Group2"));
        groups.Add(new ListViewGroup("Group3"));
        items.Add(new ListViewItem(groups[0]));
        items.Add(new ListViewItem());
        items.Add(new ListViewItem(groups[1]));
        items.Add(new ListViewItem(groups[1]));

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        ListViewGroupAccessibleObject group1AccObj = (ListViewGroupAccessibleObject)groups[0].AccessibilityObject;
        ListViewGroupAccessibleObject group2AccObj = (ListViewGroupAccessibleObject)groups[1].AccessibilityObject;
        ListViewGroupAccessibleObject group3AccObj = (ListViewGroupAccessibleObject)groups[2].AccessibilityObject;
        ListViewGroupAccessibleObject defaultGroupAccObj = (ListViewGroupAccessibleObject)listView.DefaultGroup.AccessibilityObject;
        bool supportsGetChild = listView.IsHandleCreated && listView.GroupsDisplayed;

        Assert.Null(group1AccObj.GetChild(-1));
        Assert.Null(group1AccObj.GetChild(1));

        Assert.Equal(supportsGetChild ? items[0].AccessibilityObject : null, group1AccObj.GetChild(0));

        Assert.Null(group2AccObj.GetChild(-1));
        Assert.Null(group2AccObj.GetChild(2));
        Assert.Equal(supportsGetChild ? items[2].AccessibilityObject : null, group2AccObj.GetChild(0));
        Assert.Equal(supportsGetChild ? items[3].AccessibilityObject : null, group2AccObj.GetChild(1));

        Assert.Null(group3AccObj.GetChild(-1));
        Assert.Null(group3AccObj.GetChild(0));

        Assert.Null(defaultGroupAccObj.GetChild(-1));
        Assert.Null(defaultGroupAccObj.GetChild(1));
        Assert.Equal(supportsGetChild ? items[1].AccessibilityObject : null, defaultGroupAccObj.GetChild(0));
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewGroup_GroupAddedWithItem_AccessibleObject_TestData()
    {
        foreach (bool virtualMode in new[] { true, false })
        {
            foreach (View view in Enum.GetValues(typeof(View)))
            {
                // View.Tile is not supported by ListView in virtual mode
                if (virtualMode && view == View.Tile)
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
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualListSize = 1,
            VirtualMode = virtualMode
        };

        ListViewGroup listViewGroup = new("Test Group");
        ListViewItem listViewItem = new("Test item", listViewGroup);

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

        ListViewGroup lvgroup1 = new()
        {
            Header = "CollapsibleGroup1",
            CollapsedState = ListViewGroupCollapsedState.Expanded
        };

        listView.Groups.Add(lvgroup1);
        listView.Items.Add(new ListViewItem("Item1", lvgroup1));

        ListViewGroup lvgroup2 = new()
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

        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, lvgroup1.AccessibilityObject.ExpandCollapseState);
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, lvgroup2.AccessibilityObject.ExpandCollapseState);
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, listView.DefaultGroup.AccessibilityObject.ExpandCollapseState);
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_ExpandCollapseStateFromKeyboard_ReturnsExpected()
    {
        using ListView listView = new() { ShowGroups = true };

        ListViewGroup listViewGroup = new()
        {
            Header = "CollapsibleGroup",
            CollapsedState = ListViewGroupCollapsedState.Expanded
        };

        listView.Groups.Add(listViewGroup);
        listView.Items.Add(new ListViewItem("Group Item 1", listViewGroup));
        listView.Items.Add(new ListViewItem("Group Item 2", listViewGroup));
        listView.CreateControl();
        listViewGroup.Items[0].Selected = true;
        listViewGroup.Items[0].Focused = true;
        AccessibleObject accessibleObject = listView.SelectedItems[0].AccessibilityObject;
        ListViewGroupAccessibleObject groupAccObj = (ListViewGroupAccessibleObject)accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_Parent);

        groupAccObj.SetFocus();

        Assert.True(listView.IsHandleCreated);
        Assert.NotEqual(IntPtr.Zero, listView.Handle);

        KeyboardSimulator.KeyPress(listView, Keys.Up);
        KeyboardSimulator.KeyPress(listView, Keys.Left);

        Assert.Equal(ListViewGroupCollapsedState.Collapsed, listViewGroup.GetNativeCollapsedState());
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, listViewGroup.AccessibilityObject.ExpandCollapseState);

        KeyboardSimulator.KeyPress(listView, Keys.Left);

        // The second left key pressing should not change Collapsed state
        Assert.Equal(ListViewGroupCollapsedState.Collapsed, listViewGroup.GetNativeCollapsedState());
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Collapsed, listViewGroup.AccessibilityObject.ExpandCollapseState);

        KeyboardSimulator.KeyPress(listView, Keys.Right);

        Assert.Equal(ListViewGroupCollapsedState.Expanded, listViewGroup.GetNativeCollapsedState());
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, listViewGroup.AccessibilityObject.ExpandCollapseState);

        KeyboardSimulator.KeyPress(listView, Keys.Right);

        // The second right key pressing should not change Expanded state
        Assert.Equal(ListViewGroupCollapsedState.Expanded, listViewGroup.GetNativeCollapsedState());
        Assert.Equal(ExpandCollapseState.ExpandCollapseState_Expanded, listViewGroup.AccessibilityObject.ExpandCollapseState);
    }

    [WinFormsTheory]
    [InlineData(ListViewGroupCollapsedState.Collapsed)]
    [InlineData(ListViewGroupCollapsedState.Expanded)]
    public void ListViewGroupAccessibleObject_GroupCollapsedStateChanged_IsExpected_ForMultipleSelection(ListViewGroupCollapsedState firstGroupSate)
    {
        using ListView listView = new() { ShowGroups = true };

        // This test checks the case of collapsing of the second group.
        // The first group state might affect behavior, so check both states.
        ListViewGroup group1 = new("Group 1") { CollapsedState = firstGroupSate };
        ListViewGroup group2 = new("Group 2") { CollapsedState = ListViewGroupCollapsedState.Expanded };
        ListViewGroup group3 = new("Group 3") { CollapsedState = ListViewGroupCollapsedState.Expanded };
        listView.Groups.AddRange((ListViewGroup[])[group1, group2, group3]);
        ListViewItem item1 = new("Item 1", group1);
        ListViewItem item2 = new("Item 2", group2);
        ListViewItem item3 = new("Item 2", group3);
        listView.Items.AddRange((ListViewItem[])[item1, item2, item3]);
        listView.CreateControl();
        item1.Focused = true;

        // Keep indices of groups, that GroupCollapsedStateChanged event was raised for.
        // It will help to understand if the event was raised for a correct group and was raised at all.
        List<int> eventGroupIndices = [];
        listView.GroupCollapsedStateChanged += (_, e) => eventGroupIndices.Add(e.GroupIndex);

        // Navigate to the second group
        KeyboardSimulator.KeyPress(listView, Keys.Down);

        if (firstGroupSate == ListViewGroupCollapsedState.Collapsed)
        {
            // This action is necessary to navigate to the second group correctly in this specific case.
            KeyboardSimulator.KeyPress(listView, Keys.Up);
        }

        // Simulate multiple selection of several groups to test a specific case,
        // described in https://github.com/dotnet/winforms/issues/6708
        item1.Selected = true;
        item2.Selected = true;
        item3.Selected = true;

        // Simulate the second group collapse action via keyboard.
        KeyboardSimulator.KeyPress(listView, Keys.Left);

        Assert.Equal(firstGroupSate, group1.GetNativeCollapsedState());
        Assert.Equal(firstGroupSate, group1.CollapsedState);
        Assert.Equal(ListViewGroupCollapsedState.Collapsed, group2.GetNativeCollapsedState());
        Assert.Equal(ListViewGroupCollapsedState.Collapsed, group2.CollapsedState);
        Assert.Equal(ListViewGroupCollapsedState.Expanded, group3.GetNativeCollapsedState());
        Assert.Equal(ListViewGroupCollapsedState.Expanded, group3.CollapsedState);

        // GroupCollapsedStateChanged event should be raised
        // for the second group only in this specific case.
        Assert.Single(eventGroupIndices);
        Assert.Equal(1, eventGroupIndices[0]);

        // Make sure that we really cheched multiple selection case and the items
        // are still selected after keyboard navigation simulations.
        Assert.Equal(3, listView.SelectedItems.Count);
        Assert.True(listView.IsHandleCreated);
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

        Assert.Null(listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(listViewGroupWithItems2, listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(listViewGroupWithItems1, listViewGroupWithItems2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(listViewGroupWithItems2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
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

        Assert.Null(GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        ListViewItem listViewItem1 = new();
        ListViewItem listViewItem2 = new();
        listView.Items.Add(listViewItem1);
        listView.Items.Add(listViewItem2);
        listView.Groups[0].Items.Add(listViewItem1);
        listView.Groups[3].Items.Add(listViewItem2);

        Assert.Null(GetAccessibleObject(0).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(0).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(0), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(3), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(3).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(GetAccessibleObject(3).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
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

        Assert.Null(listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(listViewGroupWithItems2, listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(listViewGroupWithItems1, listViewGroupWithItems2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(listViewGroupWithItems2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        listView.Groups[2].Items.RemoveAt(0);

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(listViewGroupWithItems1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
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

        Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
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

        Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        listView.Items.Add(listView.Groups[0].Items[0]);
        listView.Items.Add(listView.Groups[0].Items[3]);

        Assert.Equal(listView.Groups[0].Items[0].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Groups[0].Items[3].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
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

        Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Groups[0].Items[2].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));

        listView.Items.RemoveAt(1);

        Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Groups[0].Items[1].AccessibilityObject, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
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
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_Bounds_ReturnsExpected(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = GetListViewWithGroups(view, showGroups, createHandle, virtualMode: false);
        ListViewAccessibleObject accessibleObject = listView.AccessibilityObject as ListViewAccessibleObject;
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
        ListViewAccessibleObject accessibleObject = listView.AccessibilityObject as ListViewAccessibleObject;
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

        using ListView listView = new() { View = view, ShowGroups = true };
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

    [WinFormsTheory]
    [MemberData(nameof(ListViewGroupAccessibleObject_TestData))]
    public void ListViewGroupAccessibleObject_SetFocus_WorksCorrectly(View view, bool showGroups, bool createHandle)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            Size = new Size(200, 200)
        };

        ListViewGroup listGroup1 = new("Group1");
        ListViewGroup listGroup2 = new("Group2");
        ListViewItem listItem1 = new(listGroup1);
        ListViewItem listItem2 = new(listGroup1);
        ListViewItem listItem3 = new();
        listView.Groups.Add(listGroup1);
        listView.Groups.Add(listGroup2);

        listView.Items.Add(listItem1);
        listView.Items.Add(listItem2);
        listView.Items.Add(listItem3);

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

    public static IEnumerable<object[]> ListViewGroupAccessibleObject_IsPatternSupported_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool showGroups in new[] { true, false })
            {
                foreach (bool createHandle in new[] { true, false })
                {
                    foreach (ListViewGroupCollapsedState listViewGroupCollapsedState in Enum.GetValues(typeof(ListViewGroupCollapsedState)))
                    {
                        yield return new object[] { view, showGroups, createHandle, listViewGroupCollapsedState };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewGroupAccessibleObject_IsPatternSupported_TestData))]
    public void ListViewGroupAccessibleObject_IsPatternSupported_ReturnFalse_ForCollapsedStateDefault(View view, bool showGroups, bool createHandle, ListViewGroupCollapsedState listViewGroupCollapsedState)
    {
        using ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
        };

        ListViewGroup listGroup = new("Test")
        {
            CollapsedState = listViewGroupCollapsedState,
        };

        listView.Groups.Add(listGroup);
        ListViewItem item = new("Test")
        {
            Group = listGroup
        };
        listView.Items.Add(item);
        ListViewGroupAccessibleObject accessibleObject = new(listGroup, false);

        if (createHandle)
        {
            listView.CreateControl();
        }

        bool expectedPatternSupported = listViewGroupCollapsedState != ListViewGroupCollapsedState.Default;

        Assert.Equal(expectedPatternSupported, accessibleObject.IsPatternSupported(UIA_PATTERN_ID.UIA_ExpandCollapsePatternId));
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_IsDisconnected_WhenListViewReleasesUiaProvider()
    {
        using ListView listView = new();
        ListViewGroup group = new();
        listView.Groups.Add(group);
        EnforceAccessibleObjectCreation(group);
        EnforceAccessibleObjectCreation(listView.DefaultGroup);

        listView.ReleaseUiaProvider(listView.HWND);

        Assert.Null(group.TestAccessor().Dynamic._accessibilityObject);
        Assert.Null(listView.DefaultGroup.TestAccessor().Dynamic._accessibilityObject);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_IsDisconnected_WhenGroupsAreCleared()
    {
        using ListView listView = new();
        ListViewGroup group = new();
        listView.Groups.Add(group);
        EnforceAccessibleObjectCreation(group);

        listView.Groups.Clear();

        Assert.Null(group.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewGroupAccessibleObject_IsDisconnected_WhenGroupIsRemoved()
    {
        using ListView listView = new();
        ListViewGroup group = new();
        listView.Groups.Add(group);
        EnforceAccessibleObjectCreation(group);

        listView.Groups.Remove(group);

        Assert.Null(group.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    private static void EnforceAccessibleObjectCreation(ListViewGroup group)
    {
        _ = group.AccessibilityObject;
        Assert.NotNull(group.TestAccessor().Dynamic._accessibilityObject);
    }

    private ListView GetListViewItemWithInvisibleItems(View view)
    {
        ListView listView = new() { View = view };
        listView.CreateControl();
        ListViewGroup listViewGroup = new("Test group");
        ListViewItem listViewInvisibleItem1 = new("Invisible item 1");
        ListViewItem listViewVisibleItem1 = new("Visible item 1");
        ListViewItem listViewInvisibleItem2 = new("Invisible item 1");
        ListViewItem listViewVisibleItem2 = new("Visible item 1");

        listView.Groups.Add(listViewGroup);
        listView.Items.AddRange((ListViewItem[])[listViewVisibleItem1, listViewVisibleItem2]);
        listViewGroup.Items.AddRange((ListViewItem[])
        [
            listViewInvisibleItem1, listViewVisibleItem1,
            listViewVisibleItem2, listViewInvisibleItem2
        ]);

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

        ListViewItem listViewItem1 = new("Test item 1");
        if (!virtualMode)
        {
            ListViewGroup listViewGroup = new("Test Group");
            listView.Groups.Add(listViewGroup);
            listViewItem1.Group = listViewGroup;
        }

        ListViewItem listViewItem2 = new("Test item 2");

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
