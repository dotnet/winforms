// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ListViewItem_ListViewItemDetailsAccessibleObjectTests
{
    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_Ctor_OwnerListViewItemCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ListViewItemDetailsAccessibleObject(null));
    }

    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_FragmentNavigate_FirstChild_ReturnsExpected()
    {
        using ListView control = new() { View = View.Details };
        ListViewItem item = new();
        control.Columns.Add(new ColumnHeader());
        control.Items.Add(item);

        AccessibleObject accessibleObject = item.AccessibilityObject;
        AccessibleObject expected = item.SubItems[0].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_FirstChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_FragmentNavigate_LastChild_ReturnsExpected()
    {
        using ListView control = new() { View = View.Details };
        ListViewItem item = new();
        control.Items.Add(item);
        control.Columns.AddRange((ColumnHeader[])[new(), new(), new()]);
        item.SubItems.AddRange(new ListViewSubItem[] { new(), new(), new(), new(), new() });

        AccessibleObject accessibleObject = item.AccessibilityObject;
        AccessibleObject expected = item.SubItems[control.Columns.Count - 1].AccessibilityObject;

        Assert.Equal(expected, accessibleObject.FragmentNavigate(NavigateDirection.NavigateDirection_LastChild));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_GetChild_ReturnsNull_IfIndexInvalid()
    {
        using ListView control = new() { View = View.Details };
        ListViewItem item = new();
        control.Columns.AddRange((ColumnHeader[])[new(), new(), new()]);
        int outRangeIndex = control.Columns.Count + 1;
        control.Items.Add(item);

        AccessibleObject accessibleObject = item.AccessibilityObject;

        Assert.Null(accessibleObject.GetChild(-1));
        Assert.Null(accessibleObject.GetChild(outRangeIndex));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_GetChild_ReturnsExpected()
    {
        using ListView control = new() { View = View.Details };
        ListViewItem item = new();
        control.Items.Add(item);
        control.Columns.AddRange((ColumnHeader[])[new(), new(), new()]);
        item.SubItems.AddRange(new ListViewSubItem[] { new(), new(), new(), new() });

        AccessibleObject accessibleObject = item.AccessibilityObject;

        Assert.Equal(item.SubItems[0].AccessibilityObject, accessibleObject.GetChild(0));
        Assert.Equal(item.SubItems[1].AccessibilityObject, accessibleObject.GetChild(1));
        Assert.Equal(item.SubItems[2].AccessibilityObject, accessibleObject.GetChild(2));
        Assert.Null(accessibleObject.GetChild(3));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_GetChildCount_ReturnsExpected_IfControlIsNotCreated()
    {
        using ListView control = new() { View = View.Details };
        ListViewItem item = new();
        control.Items.Add(item);

        AccessibleObject accessibleObject = item.AccessibilityObject;

        Assert.Equal(-1, accessibleObject.GetChildCount());
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_GetChildCount_ReturnsExpected()
    {
        using ListView control = new() { View = View.Details };
        ListViewItem item = new();
        control.Items.Add(item);
        control.Columns.AddRange((ColumnHeader[])[new(), new(), new()]);
        control.CreateControl();

        AccessibleObject accessibleObject = item.AccessibilityObject;

        Assert.Equal(control.Columns.Count, accessibleObject.GetChildCount());
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_SubItemAccessibleObjects_AreDisconnected_WhenItemIsReleased()
    {
        using ListView control = new() { View = View.Details };
        ListViewItem item = new();
        control.Items.Add(item);
        control.Columns.AddRange((ColumnHeader[])[new(), new(), new()]);

        // Enforce subitems' accessible objects creation.
        Assert.NotNull(item.AccessibilityObject.GetChild(0));
        Assert.NotNull(item.AccessibilityObject.GetChild(1));
        Assert.NotNull(item.AccessibilityObject.GetChild(2));
        Assert.NotEmpty(item.AccessibilityObject.TestAccessor().Dynamic._listViewSubItemAccessibleObjects);

        item.ReleaseUiaProvider();

        Assert.Empty(item.AccessibilityObject.TestAccessor().Dynamic._listViewSubItemAccessibleObjects);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListViewItemDetailsAccessibleObject_HasImage_ReturnsExpected(bool addImages)
    {
        using ListView listView = new()
        {
            View = View.Details
        };

        listView.Columns.Add(new ColumnHeader());

        using ImageList imageList = new();
        imageList.Images.Add(Form.DefaultIcon);

        if (addImages)
        {
            listView.SmallImageList = imageList;
        }

        ListViewItem listViewItem = new("1", 0);
        listView.Items.Add(listViewItem);

        var accessibleObject = (ListViewItemDetailsAccessibleObject)listView.Items[0].AccessibilityObject;
        Assert.Equal(addImages, accessibleObject.HasImage);
        Assert.False(listView.IsHandleCreated);
    }

    private ListView GetListView()
    {
        ListView listView1 = new();
        ColumnHeader columnHeader1 = new();
        ColumnHeader columnHeader2 = new();
        ColumnHeader columnHeader3 = new();

        ListViewItem listViewItem1 = new(["Item1", "sub1", "sub2"], 0);
        ListViewItem listViewItem2 = new(["Item2", "sub1", "sub2"], 0);

        listView1.Columns.AddRange((ColumnHeader[])[columnHeader1, columnHeader2, columnHeader3]);
        listView1.Items.AddRange((ListViewItem[])[listViewItem1, listViewItem2]);
        listView1.View = View.Details;

        columnHeader1.Text = "Col1";
        columnHeader2.Text = "Col2";
        columnHeader3.Text = "Col3";

        return listView1;
    }

    // Unit test for https://github.com/dotnet/winforms/issues/7492.
    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_WithImageColumnAfterRemoveColumnSubItemAccessibleObjects_ReturnsExpected()
    {
        using Form form = new();
        using ListView listView1 = GetListView();

        using ImageList imageList1 = new();
        using Image image = new Bitmap(1, 1);
        imageList1.Images.Add(image);
        listView1.LargeImageList = imageList1;
        listView1.SmallImageList = imageList1;

        form.Controls.Add(listView1);
        form.Show();

        AccessibleObject listItemAccessibleObject = listView1.AccessibilityObject.GetChild(1);

        Assert.Equal("sub1", listItemAccessibleObject.GetChild(2).Name);

        listView1.Columns.RemoveAt(0);

        Assert.Equal("sub2", listItemAccessibleObject.GetChild(2).Name);

        foreach (ColumnHeader col in listView1.Columns)
        {
            col.Dispose();
        }
    }

    // Unit test for https://github.com/dotnet/winforms/issues/7492.
    [WinFormsFact]
    public void ListViewItemDetailsAccessibleObject_AfterRemoveColumnSubItemAccessibleObjects_ReturnsExpected()
    {
        using Form form = new();
        using ListView listView1 = GetListView();

        form.Controls.Add(listView1);
        form.Show();

        AccessibleObject listItemAccessibleObject = listView1.AccessibilityObject.GetChild(1);

        Assert.Equal("sub1", listItemAccessibleObject.GetChild(1).Name);

        listView1.Columns.RemoveAt(0);

        Assert.Equal("sub2", listItemAccessibleObject.GetChild(1).Name);

        foreach (ColumnHeader col in listView1.Columns)
        {
            col.Dispose();
        }
    }
}
