// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public sealed class ListViewActionListTests : IDisposable
{
    private readonly ListView _listView;
    private readonly ListViewDesigner _designer;
    private readonly ListViewActionList _actionList;

    public ListViewActionListTests()
    {
        _listView = new();
        _designer = new();
        _designer.Initialize(_listView);
        _actionList = new(_designer);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _listView.Dispose();
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        _actionList.Should().NotBeNull();
        _actionList.Component.Should().Be(_listView);
    }

    [Fact]
    public void View_Property_ShouldGetAndSet()
    {
        _actionList.View = View.Details;
        _actionList.View.Should().Be(View.Details);
    }

    [Fact]
    public void LargeImageList_Property_ShouldGetAndSet()
    {
        using ImageList largeImageList = new();
        _actionList.LargeImageList = largeImageList;
        _actionList.LargeImageList.Should().Be(largeImageList);
        _listView.LargeImageList.Should().Be(largeImageList);
    }

    [Fact]
    public void SmallImageList_Property_ShouldGetAndSet()
    {
        using ImageList smallImageList = new();
        _actionList.SmallImageList = smallImageList;
        _actionList.SmallImageList.Should().Be(smallImageList);
        _listView.SmallImageList.Should().Be(smallImageList);
    }

    [Fact]
    public void GetSortedActionItems_ShouldReturnItemsAndIncludeExpectedItems()
    {
        var items = _actionList.GetSortedActionItems();
        items.Should().NotBeNull();

        var enumerableItems = items.Cast<DesignerActionItem>();
        enumerableItems.Should().NotBeEmpty();

        var relevantItems = enumerableItems.Where(i =>
            i.DisplayName == SR.ListViewActionListEditItemsDisplayName ||
            i.DisplayName == SR.ListViewActionListEditColumnsDisplayName ||
            i.DisplayName == SR.ListViewActionListEditGroupsDisplayName
        ).ToList();

        relevantItems.Should().HaveCount(3);

        var itemNames = relevantItems.Select(i => i.DisplayName).ToList();
        itemNames.Should().Contain(SR.ListViewActionListEditItemsDisplayName);
        itemNames.Should().Contain(SR.ListViewActionListEditColumnsDisplayName);
        itemNames.Should().Contain(SR.ListViewActionListEditGroupsDisplayName);
    }
}
