// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewItem_ListViewItemWithImageAccessibleObjectTests
{
    [WinFormsTheory]
    [MemberData(nameof(GetViewTheoryData))]
    public void ListViewItemListAccessibleObject_FragmentNavigate_Children_ReturnsNull_WithoutImage(View view)
    {
        using ListView control = new();
        control.View = view;
        control.Items.Add(new ListViewItem());

        AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

        Assert.Null(listViewItemAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.Null(listViewItemAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetViewTheoryData))]
    public void ListViewItemListAccessibleObject_FragmentNavigate_Children_IsExpected_WithImage(View view)
    {
        using ImageList imageCollection = new();
        imageCollection.Images.Add(Form.DefaultIcon);

        ListViewItem listViewItem = new("Test", 0);
        using ListView control = new()
        {
            View = view,
            SmallImageList = imageCollection,
            LargeImageList = imageCollection
        };
        if (!control.IsHandleCreated)
        {
            Assert.NotEqual(IntPtr.Zero, control.Handle);
        }

        control.Items.Add(listViewItem);

        AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;
        var firstChild = listViewItemAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild);
        var lastChild = listViewItemAccessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild);

        Assert.IsType<ListViewItemImageAccessibleObject>(firstChild);
        Assert.IsType<ListViewItemImageAccessibleObject>(lastChild);
        Assert.Same(firstChild, lastChild);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetViewTheoryData))]
    public void ListViewItemListAccessibleObject_GetChild_ReturnsNull_WithoutImage(View view)
    {
        using ListView control = new();
        control.View = view;
        control.Items.Add(new ListViewItem());

        AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

        Assert.Null(listViewItemAccessibleObject.GetChild(0));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetViewTheoryData))]
    public void ListViewItemListAccessibleObject_GetChild_IsExpected_WithImage(View view)
    {
        using ImageList imageCollection = new();
        imageCollection.Images.Add(Form.DefaultIcon);

        ListViewItem listViewItem = new("Test", 0);
        using ListView control = new()
        {
            View = view,
            SmallImageList = imageCollection,
            LargeImageList = imageCollection
        };
        control.Items.Add(listViewItem);

        AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

        Assert.IsType<ListViewItemImageAccessibleObject>(listViewItemAccessibleObject.GetChild(0));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetViewTheoryData))]
    public void ListViewItemListAccessibleObject_GetChildCount_WithoutImage(View view)
    {
        using ListView control = new();
        control.View = view;
        control.Items.Add(new ListViewItem());
        control.CreateControl();

        AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

        Assert.Equal(AccessibleObject.InvalidIndex, listViewItemAccessibleObject.GetChildCount());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(GetViewTheoryData))]
    public void ListViewItemListAccessibleObject_GetChildCount_WithImage(View view)
    {
        using ImageList imageCollection = new();
        imageCollection.Images.Add(Form.DefaultIcon);

        ListViewItem listViewItem = new("Test", 0);
        using ListView control = new()
        {
            View = view,
            SmallImageList = imageCollection,
            LargeImageList = imageCollection
        };
        control.Items.Add(listViewItem);
        control.CreateControl();

        AccessibleObject listViewItemAccessibleObject = control.Items[0].AccessibilityObject;

        Assert.Equal(1, listViewItemAccessibleObject.GetChildCount());
        Assert.True(control.IsHandleCreated);
    }

    public static TheoryData<View> GetViewTheoryData()
    {
        return
        [
            View.LargeIcon,
            View.SmallIcon
        ];
    }
}
