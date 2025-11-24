// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class ListControlUnboundActionListTests : IDisposable
{
    private readonly ComponentDesigner _designer;
    private readonly ListBox _listBox;
    private readonly ListControlUnboundActionList _actionList;

    public ListControlUnboundActionListTests()
    {
        _designer = new();
        _listBox = new();
        _designer.Initialize(_listBox);
        _actionList = new(_designer);
    }

    public void Dispose()
    {
        _designer.Dispose();
        _listBox.Dispose();
    }

    [Fact]
    public void Constructor_ShouldInitializeDesigner() => _actionList.Should().NotBeNull();

    [Fact]
    public void GetSortedActionItems_ShouldReturnCorrectItems_WhenDataSourceIsNull()
    {
        DesignerActionItemCollection items = _actionList.GetSortedActionItems();

        items.Should().NotBeNull();
        items.Count.Should().Be(1);
        items[0].Should().BeOfType<DesignerActionMethodItem>();
        DesignerActionMethodItem methodItem = (DesignerActionMethodItem)items[0];
        methodItem.DisplayName.Should().Be(SR.ListControlUnboundActionListEditItemsDisplayName);
        methodItem.Category.Should().Be(SR.ItemsCategoryName);
        methodItem.Description.Should().Be(SR.ListControlUnboundActionListEditItemsDescription);
    }

    [Fact]
    public void GetSortedActionItems_ShouldReturnEmpty_WhenDataSourceIsSet()
    {
        _listBox.DataSource = new List<string> { "Item1", "Item2" };

        DesignerActionItemCollection items = _actionList.GetSortedActionItems();

        items.Should().NotBeNull();
        items.Count.Should().Be(0);
    }
}
