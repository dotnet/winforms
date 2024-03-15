// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewItem_ListViewItemListAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewItemListAccessibleObject_Ctor_OwnerListViewItemCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ListViewItemListAccessibleObject(null));
    }

    [WinFormsFact]
    public void ListViewItemListAccessibleObject_Ctor_OwnerListViewCannotBeNull()
    {
        Assert.Throws<InvalidOperationException>(() => new ListViewItemListAccessibleObject(new ListViewItem()));
    }

    [WinFormsFact]
    public void ListViewItemListAccessibleObject_Bounds_IsEmptyRectangle_IfOwningControlNotCreated()
    {
        using ListView control = new();
        control.View = View.List;
        control.Items.Add(new ListViewItem());

        Assert.Equal(Rectangle.Empty, control.Items[0].AccessibilityObject.Bounds);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemListAccessibleObject_FragmentNavigate_Parent()
    {
        using ListView control = new();
        control.View = View.List;
        control.Items.Add(new ListViewItem());
        AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;

        Assert.Equal(control.AccessibilityObject, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_Parent));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemListAccessibleObject_FragmentNavigate_PreviousSibling()
    {
        using ListView control = new();
        control.View = View.List;
        control.Items.AddRange((ListViewItem[])[new(), new(), new()]);
        control.CreateControl();

        AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Items[2].AccessibilityObject;

        Assert.Null(accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject1, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));
        Assert.Equal(accessibleObject2, accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_PreviousSibling));

        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemListAccessibleObject_FragmentNavigate_NextSibling()
    {
        using ListView control = new();
        control.View = View.List;
        control.Items.AddRange((ListViewItem[])[new(), new(), new()]);
        control.CreateControl();

        AccessibleObject accessibleObject1 = control.Items[0].AccessibilityObject;
        AccessibleObject accessibleObject2 = control.Items[1].AccessibilityObject;
        AccessibleObject accessibleObject3 = control.Items[2].AccessibilityObject;

        Assert.Equal(accessibleObject2, accessibleObject1.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Equal(accessibleObject3, accessibleObject2.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));
        Assert.Null(accessibleObject3.FragmentNavigate(NavigateDirection.NavigateDirection_NextSibling));

        Assert.True(control.IsHandleCreated);
    }

    // More tests for this class has been created already in ListViewItem_ListViewItemAccessibleObjectTests
}
