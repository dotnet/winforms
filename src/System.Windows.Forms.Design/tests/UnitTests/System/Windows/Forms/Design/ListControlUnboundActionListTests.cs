// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class ListControlUnboundActionListTests : IDisposable
{
    private readonly ComponentDesigner _designer;
    private readonly Mock<IComponent> _componentMock;
    private readonly ListControlUnboundActionList _actionList;

    public ListControlUnboundActionListTests()
    {
        _designer = new();
        _componentMock = new();
        _designer.Initialize(_componentMock.Object);
        _actionList = new(_designer);
    }

    public void Dispose() => _designer.Dispose();

    [Fact]
    public void Constructor_ShouldInitializeDesigner() => _actionList.Should().NotBeNull();

    [Fact]
    public void GetSortedActionItems_ShouldReturnCorrectItems()
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
    public void InvokeItemsDialog_ShouldThrowException()
    {
        Action action = _actionList.InvokeItemsDialog;
        action.Should().Throw<Exception>();
    }
}
