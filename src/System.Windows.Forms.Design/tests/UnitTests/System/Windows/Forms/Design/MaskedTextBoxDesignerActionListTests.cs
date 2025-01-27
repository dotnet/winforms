// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public class MaskedTextBoxDesignerActionListTests : IDisposable
{
    private readonly Mock<ITypeDiscoveryService> _mockTypeDiscoveryService;
    private readonly Mock<IUIService> _mockUIService;
    private readonly MaskedTextBox _maskedTextBox;
    private readonly MaskedTextBoxDesigner _designer;
    private readonly MaskedTextBoxDesignerActionList _actionList;

    public MaskedTextBoxDesignerActionListTests()
    {
        _mockTypeDiscoveryService = new();
        _mockUIService = new();
        _maskedTextBox = new();

        InitializeMocks();

        _designer = new();
        _designer.Initialize(_maskedTextBox);
        _actionList = new(_designer);

        void InitializeMocks()
        {
            Mock<ISite> mockSite = new Mock<ISite>();
            mockSite.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(_mockTypeDiscoveryService.Object);
            mockSite.Setup(s => s.GetService(typeof(IUIService))).Returns(_mockUIService.Object);
            _maskedTextBox.Site = mockSite.Object;
        }
    }

    public void Dispose()
    {
        _designer.Dispose();
        _maskedTextBox.Dispose();
    }

    [Fact]
    public void Constructor_InitializesServices()
    {
        _actionList.Should().NotBeNull();
        _actionList.Component.Should().Be(_maskedTextBox);
    }

    [Fact]
    public void GetSortedActionItems_ReturnsCorrectItems()
    {
        DesignerActionItemCollection items = _actionList.GetSortedActionItems();

        items.Count.Should().Be(1);
        items[0].DisplayName.Should().Be(SR.MaskedTextBoxDesignerVerbsSetMaskDesc);
    }
}
