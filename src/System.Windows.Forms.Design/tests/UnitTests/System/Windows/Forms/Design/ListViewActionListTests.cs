﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.Tests;

public class ListViewActionListTests
{
    private readonly ListView _listView;
    private readonly ComponentDesigner _designer;
    private readonly ListViewActionList _actionList;

    public ListViewActionListTests()
    {
        _listView = new();
        _designer = new();
        _designer.Initialize(_listView);
        _actionList = new(_designer);
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
        ImageList largeImageList = new();
        _actionList.LargeImageList = largeImageList;
        _actionList.LargeImageList.Should().Be(largeImageList);
    }

    [Fact]
    public void SmallImageList_Property_ShouldGetAndSet()
    {
        ImageList smallImageList = new();
        _actionList.SmallImageList = smallImageList;
        _actionList.SmallImageList.Should().Be(smallImageList);
    }

    [Fact]
    public void GetSortedActionItems_ShouldReturnItemsAndIncludeExpectedItems()
    {
        var items = _actionList.GetSortedActionItems();
        items.Should().NotBeNull();

        var enumerableItems = items.Cast<DesignerActionItem>();
        enumerableItems.Should().NotBeEmpty();
        enumerableItems.Should().HaveCountGreaterThan(0);

        var itemNames = enumerableItems.Select(i => i.DisplayName).ToList();
        itemNames.Should().Contain(SR.ListViewActionListEditItemsDisplayName);
        itemNames.Should().Contain(SR.ListViewActionListEditColumnsDisplayName);
        itemNames.Should().Contain(SR.ListViewActionListEditGroupsDisplayName);
    }

    [Fact]
    public void InvokeDialogs_ShouldNotThrow()
    {
        _actionList.Invoking(a => a.InvokeItemsDialog()).Should().NotThrow();
        _actionList.Invoking(a => a.InvokeColumnsDialog()).Should().NotThrow();
        _actionList.Invoking(a => a.InvokeGroupsDialog()).Should().NotThrow();
    }
}
