// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewItem_ListViewItemAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewItemAccessibleObject_Ctor_ThrowsArgumentNullException()
    {
        using ListView list = new();
        ListViewItem listItem = new();
        list.Items.Add(listItem);

        Type type = listItem.AccessibilityObject.GetType();
        ConstructorInfo ctor = type.GetConstructor([typeof(ListViewItem)]);

        Assert.NotNull(ctor);
        Assert.Throws<TargetInvocationException>(() => ctor.Invoke([null]));

        // item without parent ListView
        ListViewItem itemWithoutList = new();

        Assert.Throws<TargetInvocationException>(() => ctor.Invoke([itemWithoutList]));
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_Ctor_Default()
    {
        using ListView list = new();
        ListViewItem listItem = new();
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

        ListViewGroup listviewGroup = new();
        ListViewItem listViewItem1 = new();
        ListViewItem listViewItem2 = new(listviewGroup);
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
        using ListView list = new();
        ListViewItem listItem = new("ListItem");
        list.Items.Add(listItem);
        AccessibleObject listItemAccessibleObject = listItem.AccessibilityObject;

        string accessibleName = ((BSTR)listItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_NamePropertyId)).ToStringAndFree();

        Assert.Equal("ListItem", accessibleName);

        string automationId = ((BSTR)listItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_AutomationIdPropertyId)).ToStringAndFree();

        Assert.Equal("ListViewItem-0", automationId);

        var controlType = (UIA_CONTROLTYPE_ID)(int)listItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_ControlTypePropertyId);
        UIA_CONTROLTYPE_ID expected = UIA_CONTROLTYPE_ID.UIA_ListItemControlTypeId;

        Assert.Equal(expected, controlType);
        Assert.True((bool)listItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId));
        Assert.True((bool)listItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId));
        Assert.True((bool)listItemAccessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId));
        Assert.False(list.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_FragmentNavigate_Parent_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool virtualMode in new[] { true, false })
                {
                    // View.Tile is not supported by ListView in virtual mode
                    if (view == View.Tile && virtualMode)
                    {
                        continue;
                    }

                    foreach (bool showGroups in new[] { true, false })
                    {
                        yield return new object[] { view, createControl, virtualMode, showGroups };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Parent_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_Parent_ReturnExpected(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(view, createControl, virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Equal(GetExpectedParent(0), accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(GetExpectedParent(1), accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(GetExpectedParent(2), accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(GetExpectedParent(3), accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));

        AccessibleObject GetExpectedParent(int index)
        {
            return Application.UseVisualStyles
                && showGroups
                && !virtualMode
                && listView.View != View.List
                ? listView.Items[index].Group is not null
                    ? listView.Groups[0].AccessibilityObject
                    : listView.DefaultGroup.AccessibilityObject
                : listView.AccessibilityObject;
        }

        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_FragmentNavigate_Sibling_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                // View.Tile is not supported by ListView in virtual mode
                if (view == View.Tile && virtualMode)
                {
                    continue;
                }

                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { view, virtualMode, showGroups };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Sibling_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_NextSibling_ReturnExpected_IfHandleIsCreated(View view, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(view, true, virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        if (Application.UseVisualStyles && showGroups && !virtualMode && listView.View != View.List)
        {
            Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        }
        else
        {
            Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Equal(accessibleObject4, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
            Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        }

        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Sibling_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_NextSibling_ReturnExpected_IfHandleIsNotCreated(View view, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(view, false, virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Sibling_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnExpected_IfHandleIsCreated(View view, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(view, true, virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        if (Application.UseVisualStyles && showGroups && !virtualMode && listView.View != View.List)
        {
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        }
        else
        {
            Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
            Assert.Equal(accessibleObject3, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        }

        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Sibling_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_PreviousSibling_ReturnExpected_IfHandleIsNotCreated(View view, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(view, false, virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.False(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_FragmentNavigate_Child_TestData()
    {
        foreach (View view in new View[] { View.List, View.LargeIcon, View.SmallIcon })
        {
            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool virtualMode in new[] { true, false })
                {
                    foreach (bool showGroups in new[] { true, false })
                    {
                        yield return new object[] { view, createControl, virtualMode, showGroups };
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Child_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_FirstChild_ReturnExpected(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(view, createControl, virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ListViewItemAccessibleObject_FragmentNavigate_FirstChild_ReturnExpected_Tile_View(bool createControl, bool showGroups)
    {
        using ListView listView = GetListViewWithData(View.Tile, createControl, virtualMode: false, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ListViewItemAccessibleObject_FragmentNavigate_FirstChild_ReturnExpected_Details_View_IfHandleCreated(bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(View.Details, createControl: true, virtualMode: virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Equal(listView.Items[0].SubItems[0].AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Items[1].SubItems[0].AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Items[2].SubItems[0].AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Items[3].SubItems[0].AccessibilityObject, accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.True(listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_FragmentNavigate_Details_TestData()
    {
        foreach (bool createControl in new[] { true, false })
        {
            foreach (bool virtualMode in new[] { true, false })
            {
                foreach (bool showGroups in new[] { true, false })
                {
                    yield return new object[] { createControl, virtualMode, showGroups };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_FirstChildReturnExpected_Details_View_IfHandleNotCreated(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 3);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;

        Assert.Equal(listView.Items[0].SubItems[0].AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(listView.Items[1].SubItems[0].AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Child_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_LastChild_ReturnExpected(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithData(view, createControl, virtualMode, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ListViewItemAccessibleObject_FragmentNavigate_LastChild_ReturnExpected_Tile_View(bool createControl, bool showGroups)
    {
        using ListView listView = GetListViewWithData(View.Tile, createControl, virtualMode: false, showGroups);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;
        AccessibleObject accessibleObject4 = listView.Items[3].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Null(accessibleObject4.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_LastChild_ReturnExpected_Details_View(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 3);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;

        Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(listView.Items[1].SubItems[2].AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_LastChild_DoesNotReturnNull_WithoutSubItems_Detail_Views(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 0);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        IRawElementProviderFragment.Interface lastChild1 = accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        IRawElementProviderFragment.Interface lastChild2 = accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.NotNull(lastChild1);
        Assert.NotNull(lastChild2);
        Assert.NotEqual(listView.Items[0].SubItems[0].AccessibilityObject, lastChild1);
        Assert.NotEqual(listView.Items[1].SubItems[0].AccessibilityObject, lastChild2);
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_SingleColumn_Detail_Views(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 1, subItemCount: 5);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        IRawElementProviderFragment.Interface lastChild1 = accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        IRawElementProviderFragment.Interface lastChild2 = accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.Equal(listView.Items[0].SubItems[0].AccessibilityObject, lastChild1);
        Assert.Equal(listView.Items[1].SubItems[0].AccessibilityObject, lastChild2);
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_AfterAddingSubItem_Detail_Views(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 2, subItemCount: 0);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        IRawElementProviderFragment.Interface lastChild1 = accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        IRawElementProviderFragment.Interface lastChild2 = accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.NotNull(lastChild1);
        Assert.NotNull(lastChild2);
        Assert.Single(listView.Items[0].SubItems);
        Assert.Single(listView.Items[1].SubItems);

        listView.Items[0].SubItems.Add(new ListViewSubItem() { Text = $"SubItem 0" });
        listView.Items[1].SubItems.Add(new ListViewSubItem() { Text = $"SubItem 0" });

        lastChild1 = accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        lastChild2 = accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, lastChild1);
        Assert.Equal(listView.Items[1].SubItems[1].AccessibilityObject, lastChild2);
        Assert.Equal(2, listView.Items[0].SubItems.Count);
        Assert.Equal(2, listView.Items[1].SubItems.Count);
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Details_TestData))]
    public void ListViewItemAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected_AfterRemovingSubItem_Detail_Views(bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(View.Details, createControl, virtualMode, showGroups, columnCount: 2, subItemCount: 1);

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        IRawElementProviderFragment.Interface lastChild1 = accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        IRawElementProviderFragment.Interface lastChild2 = accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, lastChild1);
        Assert.Equal(listView.Items[1].SubItems[1].AccessibilityObject, lastChild2);
        Assert.Equal(2, listView.Items[0].SubItems.Count);
        Assert.Equal(2, listView.Items[1].SubItems.Count);

        listView.Items[0].SubItems.RemoveAt(1);
        listView.Items[1].SubItems.RemoveAt(1);

        lastChild1 = accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);
        lastChild2 = accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.NotNull(lastChild1);
        Assert.NotNull(lastChild2);
        Assert.Single(listView.Items[0].SubItems);
        Assert.Single(listView.Items[1].SubItems);
        Assert.Equal(createControl, listView.IsHandleCreated);
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

        ListViewItem listItem1 = new(
        [
            "Test A",
            "Alpha"
        ], -1);

        ListViewItem listItem2 = new(
        [
            "Test B",
            "Beta"
        ], -1);

        ListViewItem listItem3 = new(
        [
            "Test C",
            "Gamma"
        ], -1);

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
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        // Second list view item
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        // Third list view item
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        // Parent
        Assert.Equal(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent), listView.AccessibilityObject);
        Assert.Equal(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent), listView.AccessibilityObject);
        Assert.Equal(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent), listView.AccessibilityObject);
        Assert.True(listView.IsHandleCreated);
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
    public void ListViewItemAccessibleObject_State_ReturnExpected(View view, bool selected, AccessibleStates expectedAccessibleStates, bool createHandle)
    {
        using ListView listView = new ListView
        {
            View = view
        };

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        ListViewItem listItem1 = new(["Test A", "Alpha"], -1);
        listView.Items.Add(listItem1);
        listView.Items[0].Selected = selected;
        AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

        Assert.Equal(expectedAccessibleStates, accessibleObject.State);
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
    public void ListViewItemAccessibleObject_State_Virtual_ModeReturnExpected(View view, bool selected, AccessibleStates expectedAccessibleStates, bool createHandle)
    {
        using ListView listView = new ListView
        {
            View = view,
            VirtualMode = true,
            VirtualListSize = 1
        };

        ListViewItem listItem1 = new(["Test A", "Alpha"], -1);

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

        AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

        Assert.Equal(expectedAccessibleStates, accessibleObject.State);
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_Bounds_TestData()
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
                    yield return new object[] { view, showGroups, virtualMode };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_Bounds_TestData))]
    public void ListViewItemAccessibleObject_Bounds_ReturnExpected_IfHandleIsCreated(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetBoundsListView(view, showGroups, virtualMode);
        listView.CreateControl();

        Assert.NotEqual(Rectangle.Empty, listView.Items[0].AccessibilityObject.Bounds);
        if (listView.GroupsDisplayed)
        {
            Assert.Equal(Rectangle.Empty, listView.Items[1].AccessibilityObject.Bounds);
        }
        else
        {
            Assert.NotEqual(Rectangle.Empty, listView.Items[1].AccessibilityObject.Bounds);
        }

        Assert.NotEqual(Rectangle.Empty, listView.Items[2].AccessibilityObject.Bounds);

        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_Bounds_TestData))]
    public void ListViewItemAccessibleObject_Bounds_ReturnExpected_IfHandleIsNotCreated(View view, bool showGroups, bool virtualMode)
    {
        using ListView listView = GetBoundsListView(view, showGroups, virtualMode);

        Assert.Equal(Rectangle.Empty, listView.Items[0].AccessibilityObject.Bounds);
        Assert.Equal(Rectangle.Empty, listView.Items[1].AccessibilityObject.Bounds);
        Assert.Equal(Rectangle.Empty, listView.Items[2].AccessibilityObject.Bounds);

        Assert.False(listView.IsHandleCreated);
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
            if (view == View.Tile)
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
        using ListView listView = new()
        {
            View = view,
            VirtualMode = virtualMode,
            VirtualListSize = 1,
            CheckBoxes = checkboxesEnabled,
            ShowGroups = showGroups
        };

        ListViewItem listViewItem = new("Item");
        AddItemToListView(listView, listViewItem, virtualMode);

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        Assert.Equal(checkboxesEnabled, listViewItem.AccessibilityObject.IsPatternSupported(UIA_PATTERN_ID.UIA_TogglePatternId));
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_ToggleState_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            // View.Tile does not support enabled CheckBoxes
            if (view == View.Tile)
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
        using ListView listView = new()
        {
            View = view,
            VirtualMode = virtualMode,
            VirtualListSize = 1,
            CheckBoxes = true,
            ShowGroups = showGroups
        };

        ListViewItem listViewItem = new("Item");
        AddItemToListView(listView, listViewItem, virtualMode);

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        AccessibleObject listViewItemAccessibleObject = listViewItem.AccessibilityObject;

        Assert.Equal(ToggleState.ToggleState_Off, listViewItemAccessibleObject.ToggleState);

        listViewItem.Checked = true;

        Assert.Equal(ToggleState.ToggleState_On, listViewItemAccessibleObject.ToggleState);

        listViewItem.Checked = false;

        Assert.Equal(ToggleState.ToggleState_Off, listViewItemAccessibleObject.ToggleState);
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_ToggleState_TestData))]
    public void ListViewItemAccessibleObject_Toggle_Invoke(View view, bool showGroups, bool createHandle, bool virtualMode)
    {
        using ListView listView = new()
        {
            View = view,
            VirtualMode = virtualMode,
            VirtualListSize = 1,
            CheckBoxes = true,
            ShowGroups = showGroups
        };

        ListViewItem listViewItem = new("Item");
        AddItemToListView(listView, listViewItem, virtualMode);

        if (createHandle)
        {
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
        }

        AccessibleObject listViewItemAccessibleObject = listViewItem.AccessibilityObject;

        Assert.Equal(ToggleState.ToggleState_Off, listViewItemAccessibleObject.ToggleState);
        Assert.False(listViewItem.Checked);

        listViewItemAccessibleObject.Toggle();

        Assert.Equal(ToggleState.ToggleState_On, listViewItemAccessibleObject.ToggleState);
        Assert.True(listViewItem.Checked);

        // toggle again
        listViewItemAccessibleObject.Toggle();

        Assert.Equal(ToggleState.ToggleState_Off, listViewItemAccessibleObject.ToggleState);
        Assert.False(listViewItem.Checked);
        Assert.Equal(createHandle, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_InvisibleItems(View view)
    {
        using ListView listView = GetListViewItemWithInvisibleItems(view);

        Assert.Null(GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.True(listView.IsHandleCreated);

        AccessibleObject GetAccessibleObject(int index) => listView.Groups[0].Items[index].AccessibilityObject;
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_InvisibleItems_AfterAddingItems(View view)
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using ListView listView = GetListViewItemWithInvisibleItems(view);

        Assert.Null(GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        listView.Items.Add(listView.Groups[0].Items[0]);
        listView.Items.Add(listView.Groups[0].Items[3]);

        Assert.Null(GetAccessibleObject(0).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(0).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(0), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(3), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(3).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(GetAccessibleObject(3).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.True(listView.IsHandleCreated);

        AccessibleObject GetAccessibleObject(int index) => listView.Groups[0].Items[index].AccessibilityObject;
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_FragmentNavigate_Sibling_ReturnsExpected_InvisibleItems_AfterRemovingItems(View view)
    {
        using ListView listView = GetListViewItemWithInvisibleItems(view);

        Assert.Null(GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(GetAccessibleObject(2), GetAccessibleObject(1).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(GetAccessibleObject(1), GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(GetAccessibleObject(2).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        listView.Items.RemoveAt(1);

        Assert.Null(GetAccessibleObject(0).FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Null(GetAccessibleObject(0).FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.True(listView.IsHandleCreated);

        AccessibleObject GetAccessibleObject(int index) => listView.Groups[0].Items[index].AccessibilityObject;
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Parent_TestData))]
    public void ListViewItemAccessibleObject_GetChildCount_ReturnsExpected(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(view, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 1);

        // We return 3 for "Details" view because the number of children is equal to the number of columns
        // We return 0 for the "Tile" view because it is limited in the unit tests(all cases are covered in the MauiListViewTests)
        // We return -1 for the "List", "LargeIcon", "SmallIcon" view because they don't support subitems.
        int expectedCount = listView.IsHandleCreated && listView.SupportsListViewSubItems
            ? view == View.Details ? 3 : 0
            : -1;

        Assert.Equal(expectedCount, listView.Items[0].AccessibilityObject.GetChildCount());
        Assert.Equal(expectedCount, listView.Items[1].AccessibilityObject.GetChildCount());
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Parent_TestData))]
    public void ListViewItemAccessibleObject_GetChild_ReturnsExpected(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(view, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 1);
        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;

        Assert.Null(accessibleObject1.GetChild(-1));
        Assert.Null(accessibleObject2.GetChild(-1));
        if (view == View.Details)
        {
            Assert.Equal(listView.Items[0].SubItems[0].AccessibilityObject, accessibleObject1.GetChild(0));
            Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, accessibleObject1.GetChild(1));
            Assert.Equal(
                ((ListViewItemDetailsAccessibleObject)accessibleObject1).GetDetailsSubItemOrFake(2),
                accessibleObject1.GetChild(2));

            Assert.Equal(listView.Items[1].SubItems[0].AccessibilityObject, accessibleObject2.GetChild(0));
            Assert.Equal(listView.Items[1].SubItems[1].AccessibilityObject, accessibleObject2.GetChild(1));
            Assert.Equal(
                ((ListViewItemDetailsAccessibleObject)accessibleObject2).GetDetailsSubItemOrFake(2),
                accessibleObject2.GetChild(2));
        }
        else
        {
            Assert.Null(accessibleObject1.GetChild(0));
            Assert.Null(accessibleObject1.GetChild(1));
            Assert.Null(accessibleObject1.GetChild(2));

            Assert.Null(accessibleObject2.GetChild(0));
            Assert.Null(accessibleObject2.GetChild(1));
            Assert.Null(accessibleObject2.GetChild(2));
        }

        Assert.Null(accessibleObject1.GetChild(3));
        Assert.Null(accessibleObject2.GetChild(3));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_FragmentNavigate_Parent_TestData))]
    public void ListViewItemAccessibleObject_GetChild_ReturnsExpected_ForManySubItems(View view, bool createControl, bool virtualMode, bool showGroups)
    {
        using ListView listView = GetListViewWithSubItemData(view, createControl, virtualMode, showGroups, columnCount: 3, subItemCount: 10);
        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;

        Assert.Null(accessibleObject1.GetChild(-1));
        Assert.Null(accessibleObject2.GetChild(-1));
        if (view == View.Details)
        {
            Assert.Equal(listView.Items[0].SubItems[0].AccessibilityObject, accessibleObject1.GetChild(0));
            Assert.Equal(listView.Items[0].SubItems[1].AccessibilityObject, accessibleObject1.GetChild(1));
            Assert.Equal(listView.Items[0].SubItems[2].AccessibilityObject, accessibleObject1.GetChild(2));

            Assert.Equal(listView.Items[1].SubItems[0].AccessibilityObject, accessibleObject2.GetChild(0));
            Assert.Equal(listView.Items[1].SubItems[1].AccessibilityObject, accessibleObject2.GetChild(1));
            Assert.Equal(listView.Items[1].SubItems[2].AccessibilityObject, accessibleObject2.GetChild(2));
        }
        else
        {
            Assert.Null(accessibleObject1.GetChild(0));
            Assert.Null(accessibleObject1.GetChild(1));
            Assert.Null(accessibleObject1.GetChild(2));

            Assert.Null(accessibleObject2.GetChild(0));
            Assert.Null(accessibleObject2.GetChild(1));
            Assert.Null(accessibleObject2.GetChild(2));
        }

        Assert.Null(accessibleObject1.GetChild(3));
        Assert.Null(accessibleObject2.GetChild(3));
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    private ListView GetListViewWithSubItemData(
        View view,
        bool createControl,
        bool virtualMode,
        bool showGroups,
        int columnCount,
        int subItemCount)
    {
        ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualMode = virtualMode,
            VirtualListSize = 2
        };

        ListViewItem listItem1 = new("Item 1");
        ListViewItem listItem2 = new("Item 2");

        if (!virtualMode)
        {
            ListViewGroup listViewGroup = new("Test");
            listItem2.Group = listViewGroup;
            listView.Groups.Add(listViewGroup);
        }

        for (int i = 0; i < subItemCount; i++)
        {
            listItem1.SubItems.Add(new ListViewSubItem() { Text = $"SubItem {i}" });
            listItem2.SubItems.Add(new ListViewSubItem() { Text = $"SubItem {i}" });
        }

        for (int i = 0; i < columnCount; i++)
        {
            listView.Columns.Add(new ColumnHeader($"Column {i}"));
        }

        if (virtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listItem1,
                    1 => listItem2,
                    _ => throw new NotImplementedException()
                };
            };

            listItem1.SetItemIndex(listView, 0);
            listItem2.SetItemIndex(listView, 0);
        }
        else
        {
            listView.Items.AddRange((ListViewItem[])[listItem1, listItem2]);
        }

        if (createControl)
        {
            listView.CreateControl();
        }

        return listView;
    }

    private ListView GetListViewWithData(View view, bool createControl, bool virtualMode, bool showGroups, bool groupsEnabled = true, bool itemsInGroup = true, ListViewGroupCollapsedState state = ListViewGroupCollapsedState.Default)
    {
        ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualMode = virtualMode,
            VirtualListSize = 4
        };

        ListViewItem listItem1 = new(["Test Item 1", "Item A"], -1);
        ListViewItem listItem2 = new("Group item 2");
        ListViewItem listItem3 = new("Item 3");
        ListViewItem listItem4 = new(["Test Item 4", "Item B", "Item C", "Item D"], -1);

        if (!virtualMode && groupsEnabled)
        {
            ListViewGroup listViewGroup = new("Test") { CollapsedState = state };
            listView.Groups.Add(listViewGroup);
            if (itemsInGroup)
            {
                listItem1.Group = listViewGroup;
                listItem2.Group = listViewGroup;
            }
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

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_FragmentNavigate_Sibling_Parent_ReturnsExpected_AfterAddingGroup(View view)
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using ListView listView = new()
        {
            View = view,
            ShowGroups = true
        };

        listView.Items.Add(new ListViewItem("Item 1"));
        listView.Items.Add(new ListViewItem("Item 2"));
        listView.Items.Add(new ListViewItem("Item 3"));
        listView.Columns.Add(new ColumnHeader());

        listView.CreateControl();

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;

        Assert.Equal(listView.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        listView.Groups.Add(new ListViewGroup());
        listView.Items[1].Group = listView.Groups[0];

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.Groups[0].AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_FragmentNavigate_Sibling_Parent_ReturnsExpected_AfterRemovingGroup(View view)
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using ListView listView = new()
        {
            View = view,
            ShowGroups = true
        };

        listView.Groups.Add(new ListViewGroup());

        listView.Items.Add(new ListViewItem("Item 1"));
        listView.Items.Add(new ListViewItem("Item 2", group: listView.Groups[0]));
        listView.Items.Add(new ListViewItem("Item 3"));
        listView.Columns.Add(new ColumnHeader());

        listView.CreateControl();

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.Groups[0].AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        listView.Groups.RemoveAt(0);

        Assert.Equal(listView.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_FragmentNavigate_Sibling_Parent_ReturnsExpected_AfterUpdatingGroup(View view)
    {
        if (!Application.UseVisualStyles)
        {
            return;
        }

        using ListView listView = new()
        {
            View = view,
            ShowGroups = true
        };

        listView.Groups.Add(new ListViewGroup());

        listView.Items.Add(new ListViewItem("Item 1"));
        listView.Items.Add(new ListViewItem("Item 2", group: listView.Groups[0]));
        listView.Items.Add(new ListViewItem("Item 3"));
        listView.Columns.Add(new ColumnHeader());

        listView.CreateControl();

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.Groups[0].AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        listView.Groups[0].Items.Insert(0, listView.Items[0]);

        Assert.Equal(listView.Groups[0].AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.Groups[0].AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.DefaultGroup.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_FragmentNavigate_Sibling_Parent_ReturnsExpected_ListView()
    {
        using ListView listView = new()
        {
            View = View.List,
            ShowGroups = true
        };

        listView.Items.Add(new ListViewItem("Item 1"));
        listView.Items.Add(new ListViewItem("Item 2"));
        listView.Items.Add(new ListViewItem("Item 3"));
        listView.Columns.Add(new ColumnHeader());

        listView.CreateControl();

        AccessibleObject accessibleObject1 = listView.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = listView.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = listView.Items[2].AccessibilityObject;

        Assert.Equal(listView.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        // Since "List" mode does not support ListViewGroups, adding a ListViewGroup should not affect the operation
        // of availability objects
        listView.Groups.Add(new ListViewGroup());
        listView.Items[1].Group = listView.Groups[0];

        Assert.Equal(listView.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        // Since "List" mode does not support ListViewGroups, updating a ListViewGroup should not affect the operation
        // of availability objects
        listView.Groups[0].Items.Insert(0, listView.Items[0]);

        Assert.Equal(listView.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        // Since "List" mode does not support ListViewGroups, removing a ListViewGroup should not affect the operation
        // of availability objects
        listView.Groups.RemoveAt(0);

        Assert.Equal(listView.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.Equal(listView.AccessibilityObject, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_GetChildIndex_ReturnsExpected(View view)
    {
        using ListView listView = new() { View = view };
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 1", "SubItem 2"]));
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;

        if (view == View.Details)
        {
            Assert.Equal(0, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
            Assert.Equal(1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));
        }
        else
        {
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[1].AccessibilityObject));
            Assert.Equal(-1, accessibleObject.GetChildIndex(listView.Items[0].SubItems[2].AccessibilityObject));
        }

        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_GetChildIndex_ReturnsMinusOne_IfChildIsNull(View view)
    {
        using ListView listView = new() { View = view };
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 1", "SubItem 2"]));

        Assert.Equal(-1, listView.Items[0].AccessibilityObject.GetChildIndex(null));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_GetChildIndex_ReturnsMinusOne_IfSubItemNotExists(View view)
    {
        using ListView listView = new() { View = view };
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 11", "SubItem 12"]));
        listView.Items.Add(new ListViewItem(["Item 2", "SubItem 21", "SubItem 22"]));

        Assert.Equal(-1, listView.Items[0].AccessibilityObject.GetChildIndex(listView.Items[1].SubItems[1].AccessibilityObject));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_GetChildIndex_ReturnsExpected_ForFakeSubItem()
    {
        using ListView listView = new() { View = View.Details };
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        listView.Items.Add(new ListViewItem(["Item 1"]));
        ListViewItemDetailsAccessibleObject accessibleObject = (ListViewItemDetailsAccessibleObject)listView.Items[0].AccessibilityObject;

        Assert.Equal(0, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
        Assert.Equal(1, accessibleObject.GetChildIndex(accessibleObject.GetDetailsSubItemOrFake(1)));
        Assert.Equal(2, accessibleObject.GetChildIndex(accessibleObject.GetDetailsSubItemOrFake(2)));
        Assert.Equal(3, accessibleObject.GetChildIndex(accessibleObject.GetDetailsSubItemOrFake(3)));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void ListViewItemAccessibleObject_GetChildIndex_ReturnsExpected_Image(bool hasImage, int expectedFirstSubItemIndex)
    {
        using ImageList imageCollection = new();
        imageCollection.Images.Add(Form.DefaultIcon);
        using ListView listView = new()
        {
            View = View.Details,
            SmallImageList = imageCollection
        };

        listView.Columns.Add(new ColumnHeader());
        ListViewItem listViewItem = new("Item 1", imageIndex: hasImage ? 0 : -1);
        listView.Items.Add(listViewItem);
        var accessibleObject = (ListViewItemDetailsAccessibleObject)listView.Items[0].AccessibilityObject;

        Assert.Equal(expectedFirstSubItemIndex, accessibleObject.GetChildIndex(listView.Items[0].SubItems[0].AccessibilityObject));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_GetChildIndex_ReturnsMinusOne_ForInvalidChild(View view)
    {
        using ListView listView = new() { View = view };
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        listView.Columns.Add(new ColumnHeader());
        listView.Items.Add(new ListViewItem(["Item 1", "SubItem 11", "SubItem 12"]));

        Assert.Equal(-1, listView.Items[0].AccessibilityObject.GetChildIndex(listView.AccessibilityObject));
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(View.Details)]
    [InlineData(View.LargeIcon)]
    [InlineData(View.List)]
    [InlineData(View.SmallIcon)]
    [InlineData(View.Tile)]
    public void ListViewItemAccessibleObject_ReturnExpectedType(View view)
    {
        using ListView listView = new() { View = view };
        listView.Items.Add(new ListViewItem("Test"));

        switch (view)
        {
            case View.Details:
                Assert.IsType<ListViewItemDetailsAccessibleObject>(listView.Items[0].AccessibilityObject);
                break;
            case View.LargeIcon:
                Assert.IsType<ListViewItemLargeIconAccessibleObject>(listView.Items[0].AccessibilityObject);
                break;
            case View.List:
                Assert.IsType<ListViewItemListAccessibleObject>(listView.Items[0].AccessibilityObject);
                break;
            case View.SmallIcon:
                Assert.IsType<ListViewItemSmallIconAccessibleObject>(listView.Items[0].AccessibilityObject);
                break;
            case View.Tile:
                Assert.IsType<ListViewItemTileAccessibleObject>(listView.Items[0].AccessibilityObject);
                break;
        }
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_ReturnExpectedType_TestData()
    {
        foreach (View oldView in Enum.GetValues(typeof(View)))
        {
            foreach (View newView in Enum.GetValues(typeof(View)))
            {
                if (oldView != newView)
                {
                    yield return new object[] { oldView, newView };
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_ReturnExpectedType_TestData))]
    public void ListViewItemAccessibleObject_ReturnExpectedType_AfterChangingView(View oldView, View newView)
    {
        using ListView listView = new() { View = oldView };
        listView.Items.Add(new ListViewItem("Test"));
        CheckAccessibleObject();

        listView.View = newView;

        CheckAccessibleObject();

        void CheckAccessibleObject()
        {
            switch (listView.View)
            {
                case View.Details:
                    Assert.IsType<ListViewItemDetailsAccessibleObject>(listView.Items[0].AccessibilityObject);
                    break;
                case View.LargeIcon:
                    Assert.IsType<ListViewItemLargeIconAccessibleObject>(listView.Items[0].AccessibilityObject);
                    break;
                case View.List:
                    Assert.IsType<ListViewItemListAccessibleObject>(listView.Items[0].AccessibilityObject);
                    break;
                case View.SmallIcon:
                    Assert.IsType<ListViewItemSmallIconAccessibleObject>(listView.Items[0].AccessibilityObject);
                    break;
                case View.Tile:
                    Assert.IsType<ListViewItemTileAccessibleObject>(listView.Items[0].AccessibilityObject);
                    break;
            }
        }
    }

    [WinFormsTheory]
    [InlineData(View.Details, View.LargeIcon)]
    [InlineData(View.Details, View.List)]
    [InlineData(View.Details, View.SmallIcon)]
    [InlineData(View.Details, View.Tile)]
    [InlineData(View.LargeIcon, View.Details)]
    [InlineData(View.LargeIcon, View.List)]
    [InlineData(View.LargeIcon, View.SmallIcon)]
    [InlineData(View.LargeIcon, View.Tile)]
    [InlineData(View.List, View.Details)]
    [InlineData(View.List, View.LargeIcon)]
    [InlineData(View.List, View.SmallIcon)]
    [InlineData(View.List, View.Tile)]
    [InlineData(View.SmallIcon, View.Details)]
    [InlineData(View.SmallIcon, View.LargeIcon)]
    [InlineData(View.SmallIcon, View.List)]
    [InlineData(View.SmallIcon, View.Tile)]
    [InlineData(View.Tile, View.Details)]
    [InlineData(View.Tile, View.LargeIcon)]
    [InlineData(View.Tile, View.List)]
    [InlineData(View.Tile, View.SmallIcon)]
    public void ListViewItemAccessibleObject_GetChild_ReturnException_AfterChangingView(View oldView, View newView)
    {
        using ListView listView = new() { View = oldView };
        listView.Items.Add(new ListViewItem(["1", "2"]));
        AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
        Assert.Null(accessibleObject.GetChild(0));

        listView.View = newView;

        Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChild(0));
    }

    [WinFormsTheory]
    [InlineData(View.Details, View.LargeIcon)]
    [InlineData(View.Details, View.List)]
    [InlineData(View.Details, View.SmallIcon)]
    [InlineData(View.Details, View.Tile)]
    [InlineData(View.LargeIcon, View.Details)]
    [InlineData(View.LargeIcon, View.List)]
    [InlineData(View.LargeIcon, View.SmallIcon)]
    [InlineData(View.LargeIcon, View.Tile)]
    [InlineData(View.List, View.Details)]
    [InlineData(View.List, View.LargeIcon)]
    [InlineData(View.List, View.SmallIcon)]
    [InlineData(View.List, View.Tile)]
    [InlineData(View.SmallIcon, View.Details)]
    [InlineData(View.SmallIcon, View.LargeIcon)]
    [InlineData(View.SmallIcon, View.List)]
    [InlineData(View.SmallIcon, View.Tile)]
    [InlineData(View.Tile, View.Details)]
    [InlineData(View.Tile, View.LargeIcon)]
    [InlineData(View.Tile, View.List)]
    [InlineData(View.Tile, View.SmallIcon)]
    public void ListViewItemAccessibleObject_GetChildCount_ReturnException_AfterChangingView(View oldView, View newView)
    {
        using ListView listView = new() { View = oldView };
        listView.Items.Add(new ListViewItem(["1"]));
        AccessibleObject accessibleObject = listView.Items[0].AccessibilityObject;
        Assert.NotEqual(2, accessibleObject.GetChildCount());

        listView.View = newView;

        Assert.Throws<InvalidOperationException>(() => accessibleObject.GetChildCount());
    }

    public static IEnumerable<object[]> ListViewItemAccessibleObject_GetPropertyValue_TestData()
    {
        foreach (View view in Enum.GetValues(typeof(View)))
        {
            foreach (bool createControl in new[] { true, false })
            {
                foreach (bool virtualMode in new[] { true, false })
                {
                    // View.Tile is not supported by ListView in virtual mode
                    if (virtualMode && view == View.Tile)
                    {
                        continue;
                    }

                    foreach (bool showGroups in new[] { true, false })
                    {
                        foreach (bool groupsEnabled in new[] { true, false })
                        {
                            foreach (bool itemsInGroup in new[] { true, false })
                            {
                                foreach (ListViewGroupCollapsedState state in Enum.GetValues(typeof(ListViewGroupCollapsedState)))
                                {
                                    yield return new object[] { view, createControl, virtualMode, showGroups, groupsEnabled, itemsInGroup, state };
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(ListViewItemAccessibleObject_GetPropertyValue_TestData))]
    public void ListViewItemAccessibleObject_GetPropertyValue_ReturnsFalseWhenCollapsedOrOffScreen(View view, bool createControl, bool virtualMode, bool showGroups, bool groupsEnabled, bool itemsInGroup, ListViewGroupCollapsedState state)
    {
        using ListView listView = GetListViewWithData(view, createControl, virtualMode, showGroups, groupsEnabled, itemsInGroup, state);
        ListViewItemBaseAccessibleObject accessibleObject = (ListViewItemBaseAccessibleObject)listView.Items[0].AccessibilityObject;

        bool expected = listView.GroupsDisplayed && listView.Items[0].Group?.CollapsedState == ListViewGroupCollapsedState.Collapsed;
        bool actual = (bool)accessibleObject.GetPropertyValue(UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId);

        Assert.Equal(expected, actual);
        Assert.Equal(createControl, listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_IsDisconnected_WhenListViewReleasesUiaProvider()
    {
        using ListView listView = new();
        ListViewItem item = new("ListItem");
        listView.Items.Add(item);
        EnforceAccessibleObjectCreation(item);

        listView.ReleaseUiaProvider(listView.HWND);

        Assert.Null(item.TestAccessor().Dynamic._accessibilityObject);
        Assert.True(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_IsDisconnected_WhenListViewIsCleared()
    {
        using ListView listView = new();
        ListViewItem item = new("ListItem");
        listView.Items.Add(item);
        EnforceAccessibleObjectCreation(item);

        listView.Clear();

        Assert.Null(item.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_IsDisconnected_WhenItemsAreCleared()
    {
        using ListView listView = new();
        ListViewItem item = new("ListItem");
        listView.Items.Add(item);
        EnforceAccessibleObjectCreation(item);

        listView.Items.Clear();

        Assert.Null(item.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_IsDisconnected_WhenItemIsRemoved()
    {
        using ListView listView = new();
        ListViewItem item = new("ListItem");
        listView.Items.Add(item);
        EnforceAccessibleObjectCreation(item);

        listView.Items.Remove(item);

        Assert.Null(item.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemAccessibleObject_IsDisconnected_WhenItemIsReplaced()
    {
        using ListView listView = new();
        ListViewItem item = new("ListItem");
        listView.Items.Add(item);
        EnforceAccessibleObjectCreation(item);

        listView.Items[0] = new ListViewItem();

        Assert.Null(item.TestAccessor().Dynamic._accessibilityObject);
        Assert.False(listView.IsHandleCreated);
    }

    private static void EnforceAccessibleObjectCreation(ListViewItem listViewItem)
    {
        _ = listViewItem.AccessibilityObject;
        Assert.NotNull(listViewItem.TestAccessor().Dynamic._accessibilityObject);
    }

    private ListView GetBoundsListView(View view, bool showGroups, bool virtualMode)
    {
        ListView listView = new()
        {
            View = view,
            ShowGroups = showGroups,
            VirtualMode = virtualMode,
            VirtualListSize = 3
        };

        ListViewItem listViewItem1 = new("Item1");
        ListViewItem listViewItem2 = new("Item2");
        ListViewItem listViewItem3 = new("Item3");

        if (!virtualMode)
        {
            ListViewGroup lvgroup1 = new()
            {
                Header = "CollapsibleGroup1",
                CollapsedState = ListViewGroupCollapsedState.Expanded
            };

            ListViewGroup lvgroup2 = new()
            {
                Header = "CollapsibleGroup2",
                CollapsedState = ListViewGroupCollapsedState.Collapsed
            };

            listView.Groups.Add(lvgroup1);
            listView.Groups.Add(lvgroup2);

            listViewItem1.Group = lvgroup1;
            listViewItem2.Group = lvgroup2;
        }

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
        return listView;
    }
}
