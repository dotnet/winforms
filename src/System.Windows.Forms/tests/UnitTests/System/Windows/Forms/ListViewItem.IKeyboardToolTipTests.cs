// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using static System.Windows.Forms.ListViewItem;

namespace System.Windows.Forms.Tests;

public class ListViewItem_IKeyboardToolTipTests
{
    [WinFormsTheory]
    [InlineData(true, true, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, false, true)]
    public void ListViewItemKeyboardToolTip_InvokeAllowsToolTip_ReturnsExpected(bool insideListView, bool virtualMode, bool showItemToolTips)
    {
        ListViewItem listViewItem = new();
        using var listView = GetListView(virtualMode, showItemToolTips);
        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
        }

        Assert.True(((IKeyboardToolTip)listViewItem).AllowsToolTip());
    }

    [WinFormsTheory]
    [InlineData(true, true, true, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, false, true, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, false, true, false)]
    public void ListViewItemKeyboardToolTip_InvokeAllowsChildrenToShowToolTips_ReturnsExpected(
        bool insideListView,
        bool virtualMode,
        bool showItemToolTips,
        bool expectedResult)
    {
        ListViewItem listViewItem = new();
        using var listView = GetListView(virtualMode, showItemToolTips);
        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
        }

        Assert.Equal(expectedResult, ((IKeyboardToolTip)listViewItem).AllowsChildrenToShowToolTips());
    }

    [WinFormsTheory]
    [InlineData(true, true, true, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, false, true, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, false, true, false)]
    public void ListViewItemKeyboardToolTip_InvokeCanShowToolTipsNow_ReturnsExpected(
        bool insideListView,
        bool virtualMode,
        bool showItemToolTips,
        bool expectedResult)
    {
        ListViewItem listViewItem = new();
        using var listView = GetListView(virtualMode, showItemToolTips);
        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
        }

        Assert.Equal(expectedResult, ((IKeyboardToolTip)listViewItem).CanShowToolTipsNow());
    }

    [WinFormsTheory]
    [InlineData(true, true, true, "Test tooltip")]
    [InlineData(true, true, false, "Test tooltip")]
    [InlineData(true, false, true, "Test tooltip")]
    [InlineData(true, false, false, "Test tooltip")]
    [InlineData(false, false, false, "Test tooltip")]
    public void ListViewItemKeyboardToolTip_InvokeGetCaptionForTool_ReturnsExpected(
        bool insideListView,
        bool virtualMode,
        bool showItemToolTips,
        string toolTipText)
    {
        using ToolTip toolTip = new();
        ListViewItem listViewItem = new() { ToolTipText = toolTipText };
        using var listView = GetListView(virtualMode, showItemToolTips);
        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
        }

        Assert.Equal(toolTipText, ((IKeyboardToolTip)listViewItem).GetCaptionForTool(toolTip));
    }

    [WinFormsTheory]
    [InlineData(true, true, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    public void ListViewItemKeyboardToolTip_InvokeGetNativeScreenRectangle_ReturnsExpected(
        bool insideListView,
        bool virtualMode,
        bool rectangleIsEmpty)
    {
        ListViewItem listViewItem = new();
        using var listView = GetListView(virtualMode);
        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
        }

        Assert.Equal(rectangleIsEmpty, ((IKeyboardToolTip)listViewItem).GetNativeScreenRectangle().IsEmpty);
    }

    [WinFormsTheory]
    [InlineData(ListViewGroupCollapsedState.Default, false)]
    [InlineData(ListViewGroupCollapsedState.Expanded, false)]
    [InlineData(ListViewGroupCollapsedState.Collapsed, true)]
    public void ListViewItemKeyboardToolTip_InvokeGetNativeScreenRectangle_InsideGroup_ReturnsExpected(
        ListViewGroupCollapsedState collapsedState,
        bool rectangleIsEmpty)
    {
        using ListView listView = new();
        ListViewItem listViewItem = new();
        ListViewGroup listViewGroup = new();
        listView.Groups.Add(listViewGroup);
        listView.Items.Add(listViewItem);
        listViewGroup.Items.Add(listViewItem);
        listViewGroup.CollapsedState = collapsedState;

        Assert.Equal(rectangleIsEmpty, ((IKeyboardToolTip)listViewItem).GetNativeScreenRectangle().IsEmpty);
    }

    [WinFormsTheory]
    [InlineData(true, View.List, 50, 50)]
    [InlineData(false, View.List, 50, 50)]
    [InlineData(true, View.Details, 46, 48)]
    [InlineData(false, View.Details, 46, 48)]
    public void ListViewItemKeyboardToolTip_InvokeGetNativeScreenRectangle_LongText_ReturnsExpected(
        bool virtualMode,
        View view,
        int expectedWidthVisualStyleEnabled,
        int expectedWidthVisualStyleDisabled)
    {
        using var listView = GetListView(virtualMode, view: view);
        listView.Columns.Add(new ColumnHeader() { Width = 50 });
        ListViewItem listViewItem = AssignItemToListView(listView, new ListViewItem(new string('t', 500)));
        int expectedWidth = Application.UseVisualStyles
            ? expectedWidthVisualStyleEnabled
            : expectedWidthVisualStyleDisabled;

        Assert.Equal(expectedWidth, ((IKeyboardToolTip)listViewItem).GetNativeScreenRectangle().Width);
    }

    [WinFormsTheory]
    [InlineData(true, View.List)]
    [InlineData(false, View.List)]
    [InlineData(true, View.Details)]
    [InlineData(false, View.Details)]
    public void ListViewItemKeyboardToolTip_InvokeGetNativeScreenRectangle_ShortText_ReturnsExpected(
        bool virtualMode,
        View view)
    {
        int columnWidth = 50;
        using var listView = GetListView(virtualMode, view: view);
        listView.Columns.Add(new ColumnHeader() { Width = columnWidth });
        ListViewItem listViewItem = AssignItemToListView(listView, new ListViewItem(new string('t', 1)));

        int actualWidth = ((IKeyboardToolTip)listViewItem).GetNativeScreenRectangle().Width;

        Assert.True(columnWidth > ((IKeyboardToolTip)listViewItem).GetNativeScreenRectangle().Width);
    }

    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNativeScreenRectangle_ViewTile_LongListViewItemText_ReturnsExpected()
    {
        ListViewItem listViewItem = new(new string('t', 20));

        listViewItem.SubItems.Add(new ListViewSubItem(listViewItem, new string('t', 10)));
        using var listView = GetListView(virtualMode: false, view: View.Tile);
        AssignItemToListView(listView, listViewItem);

        int expectedWidth = Application.UseVisualStyles
            ? TextRenderer.MeasureText(listViewItem.Text, listViewItem.Font).Width
            : listView.GetItemRect(0, ItemBoundsPortion.Label).Width;

        Assert.Equal(expectedWidth, ((IKeyboardToolTip)listViewItem).GetNativeScreenRectangle().Width);
    }

    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNativeScreenRectangle_ViewTile_LongListViewSubItemText_ReturnsExpected()
    {
        ListViewItem listViewItem = new(new string('t', 10));
        ListViewSubItem listViewSubItem = new(listViewItem, new string('t', 20));
        listViewItem.SubItems.Add(listViewSubItem);
        using var listView = GetListView(virtualMode: false, view: View.Tile);
        AssignItemToListView(listView, listViewItem);

        int expectedWidth = Application.UseVisualStyles
            ? TextRenderer.MeasureText(listViewSubItem.Text, listViewSubItem.Font).Width
            : listView.GetItemRect(0, ItemBoundsPortion.Label).Width;

        Assert.Equal(expectedWidth, ((IKeyboardToolTip)listViewItem).GetNativeScreenRectangle().Width);
    }

    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_WithoutList_ReturnsEmptyList()
    {
        ListViewItem listViewItem = new();

        Assert.Empty(((IKeyboardToolTip)listViewItem).GetNeighboringToolsRectangles());
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewList_ReturnsEmptyList(bool virtualMode)
    {
        using var listView = GetListView(virtualMode, view: View.List);
        ListViewItem listViewItem = AssignItemToListView(listView, new ListViewItem());

        Assert.Empty(((IKeyboardToolTip)listViewItem).GetNeighboringToolsRectangles());
    }

    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewTile_ReturnsEmptyList()
    {
        using var listView = GetListView(virtualMode: false, view: View.Tile);
        ListViewItem listViewItem = AssignItemToListView(listView, new ListViewItem());

        Assert.Empty(((IKeyboardToolTip)listViewItem).GetNeighboringToolsRectangles());
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 0
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_FirstItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[0]).GetNeighboringToolsRectangles();
        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[1]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[3]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 1
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_SecondItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[1]).GetNeighboringToolsRectangles();
        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[0]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[2]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 2
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_ThirdItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[2]).GetNeighboringToolsRectangles();
        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[1]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[5]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 3
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_FourthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[3]).GetNeighboringToolsRectangles();
        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[0]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[6]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 4
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_FifthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[4]).GetNeighboringToolsRectangles();
        Assert.Equal(4, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[1]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[3]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[5]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[7]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 5
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_SixthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[5]).GetNeighboringToolsRectangles();
        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[2]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[8]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 6
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_SeventhItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[6]).GetNeighboringToolsRectangles();
        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[3]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[7]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 7
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_EighthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[7]).GetNeighboringToolsRectangles();
        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[6]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[8]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 8
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewLargeIcon_NinthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.LargeIcon, size: new Size(150, 150));
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[8]).GetNeighboringToolsRectangles();
        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[5]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[7]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 0
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_FirstItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[0]).GetNeighboringToolsRectangles();
        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[1]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[3]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 1
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_SecondItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[1]).GetNeighboringToolsRectangles();

        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[0]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[2]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 2
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_ThirdItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[2]).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[1]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[5]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 3
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_FourthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[3]).GetNeighboringToolsRectangles();

        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[0]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[6]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 4
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_FifthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[4]).GetNeighboringToolsRectangles();

        Assert.Equal(4, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[1]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[3]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[5]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[7]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 5
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_SixthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[5]).GetNeighboringToolsRectangles();

        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[2]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[8]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 6
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_SeventhItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[6]).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[3]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[7]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 7
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_EighthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[7]).GetNeighboringToolsRectangles();
        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[6]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[8]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[4]), neighboringToolsRectangles);
    }

    // The ListView is configured to display items as follows:
    // 0 1 2
    // 3 4 5
    // 6 7 8
    // This test checks item 8
    [WinFormsFact]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewSmallIcon_NinthItem_ReturnsExpected()
    {
        using var listView = GetListView(virtualMode: false, view: View.SmallIcon, size: new Size(220, 150));
        listView.Columns.Add(new ColumnHeader());
        AddListViewItems(listView, 9);
        listView.CreateControl();

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[8]).GetNeighboringToolsRectangles();
        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[5]), neighboringToolsRectangles);
        Assert.Contains(GetNativeScreenRectangle(listView.Items[7]), neighboringToolsRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewDetails_FirstItem_ReturnsExpected(bool virtualMode)
    {
        using var listView = GetListView(virtualMode: virtualMode, view: View.Details, virtualListSize: 3);
        listView.CreateControl();
        listView.Columns.Add(new ColumnHeader());
        AssignListItemsToListView(listView, 3);

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[0]).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(listView.Items[0].AccessibilityObject.Bounds, neighboringToolsRectangles);
        Assert.Contains(listView.Items[1].AccessibilityObject.Bounds, neighboringToolsRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewDetails_SecondItem_ReturnsExpected(bool virtualMode)
    {
        using var listView = GetListView(virtualMode: virtualMode, view: View.Details, virtualListSize: 3);
        listView.CreateControl();
        listView.Columns.Add(new ColumnHeader());
        AssignListItemsToListView(listView, 3);

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[1]).GetNeighboringToolsRectangles();

        Assert.Equal(3, neighboringToolsRectangles.Count);
        Assert.Contains(listView.Items[0].AccessibilityObject.Bounds, neighboringToolsRectangles);
        Assert.Contains(listView.Items[1].AccessibilityObject.Bounds, neighboringToolsRectangles);
        Assert.Contains(listView.Items[2].AccessibilityObject.Bounds, neighboringToolsRectangles);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListViewItemKeyboardToolTip_InvokeGetNeighboringToolsRectangles_ViewDetails_ThirdItem_ReturnsExpected(bool virtualMode)
    {
        using var listView = GetListView(virtualMode: virtualMode, view: View.Details, virtualListSize: 3);
        listView.CreateControl();
        listView.Columns.Add(new ColumnHeader());
        AssignListItemsToListView(listView, 3);

        IList<Rectangle> neighboringToolsRectangles = ((IKeyboardToolTip)listView.Items[2]).GetNeighboringToolsRectangles();

        Assert.Equal(2, neighboringToolsRectangles.Count);
        Assert.Contains(listView.Items[1].AccessibilityObject.Bounds, neighboringToolsRectangles);
        Assert.Contains(listView.Items[2].AccessibilityObject.Bounds, neighboringToolsRectangles);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void ListViewItemKeyboardToolTip_InvokeGetOwnerWindow_ReturnsExpected(
        bool insideListView,
        bool virtualMode)
    {
        ListViewItem listViewItem = new();
        using var listView = GetListView(virtualMode);
        IWin32Window expectedOwner = null;

        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
            expectedOwner = listView;
        }

        Assert.Equal(expectedOwner, ((IKeyboardToolTip)listViewItem).GetOwnerWindow());
    }

    [WinFormsTheory]
    [InlineData(true, true, RightToLeft.Yes, true)]
    [InlineData(true, true, RightToLeft.No, false)]
    [InlineData(true, true, RightToLeft.Inherit, false)]
    [InlineData(true, false, RightToLeft.Yes, true)]
    [InlineData(true, false, RightToLeft.No, false)]
    [InlineData(true, false, RightToLeft.Inherit, false)]
    [InlineData(false, true, RightToLeft.Yes, false)]
    public void ListViewItemKeyboardToolTip_InvokeHasRtlModeEnabled_ReturnsExpected(
        bool insideListView,
        bool virtualMode,
        RightToLeft rightToLeft,
        bool expected)
    {
        ListViewItem listViewItem = new();
        using var listView = GetListView(virtualMode, rightToLeft: rightToLeft);

        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
        }

        Assert.Equal(expected, ((IKeyboardToolTip)listViewItem).HasRtlModeEnabled());
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/12319")]
    [WinFormsTheory]
    // Comment the data out due to ActiveIssue "https://github.com/dotnet/winforms/issues/12319".
    // [InlineData(true, true, true, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, false, true, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, false, true, false)]
    public void ListViewItemKeyboardToolTip_InvokeIsHoveredWithMouse_ReturnsExpected(
        bool insideListView,
        bool virtualMode,
        bool isHovered,
        bool expected)
    {
        Point initialPosition = Cursor.Position;
        try
        {
            ListViewItem listViewItem = new();
            using var listView = GetListView(virtualMode);

            if (insideListView)
            {
                listViewItem = AssignItemToListView(listView, listViewItem);
            }

            listView.CreateControl();

            Point position = listView.AccessibilityObject.Bounds.Location;
            if (!isHovered)
            {
                position.X--;
                position.Y--;
            }

            Cursor.Position = position;
            Assert.Equal(expected, ((IKeyboardToolTip)listViewItem).IsHoveredWithMouse());
        }
        finally
        {
            Cursor.Position = initialPosition;
        }
    }

    [WinFormsTheory]
    [InlineData(true, true, true, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, false, true, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, false, true, false)]
    public void ListViewItemKeyboardToolTip_InvokeShowsOwnToolTip_ReturnsExpected(
        bool insideListView,
        bool virtualMode,
        bool showItemToolTips,
        bool expectedResult)
    {
        ListViewItem listViewItem = new();
        using var listView = GetListView(virtualMode, showItemToolTips);
        if (insideListView)
        {
            listViewItem = AssignItemToListView(listView, listViewItem);
        }

        Assert.Equal(expectedResult, ((IKeyboardToolTip)listViewItem).ShowsOwnToolTip());
    }

    private ListViewItem AssignItemToListView(ListView listView, ListViewItem listViewItem)
    {
        if (listView.VirtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    0 => listViewItem,
                    _ => new ListViewItem(),
                };
            };
        }
        else
        {
            listView.Items.Add(listViewItem);
        }

        return listView.Items[0];
    }

    private void AddListViewItems(ListView listView, int count)
    {
        for (int i = 0; i < count; i++)
        {
            listView.Items.Add(i.ToString());
        }
    }

    private void AssignListItemsToListView(ListView listView, int count)
    {
        List<ListViewItem> listViewItems = [];

        for (int i = 0; i < count; i++)
        {
            ListViewItem listViewItem = new(i.ToString());
            listViewItem.SubItems.Add(i.ToString());
            listViewItems.Add(listViewItem);
        }

        if (listView.VirtualMode)
        {
            listView.RetrieveVirtualItem += (s, e) =>
            {
                e.Item = e.ItemIndex switch
                {
                    _ => listViewItems[e.ItemIndex],
                };
            };
        }
        else
        {
            listView.Items.AddRange((ListViewItem[])[.. listViewItems]);
        }
    }

    private ListView GetListView(
        bool virtualMode,
        bool showItemToolTips = false,
        RightToLeft rightToLeft = RightToLeft.No,
        View view = View.LargeIcon,
        Size? size = null,
        int virtualListSize = 1)
    {
        return new ListView()
        {
            VirtualListSize = virtualListSize,
            VirtualMode = virtualMode,
            ShowItemToolTips = showItemToolTips,
            RightToLeft = rightToLeft,
            Size = size ?? new Size(50, 50),
            View = view
        };
    }

    private Rectangle GetNativeScreenRectangle(ListViewItem listView)
    {
        return ((IKeyboardToolTip)listView).GetNativeScreenRectangle();
    }
}
