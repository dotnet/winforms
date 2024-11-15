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
    private readonly Mock<IHelpService> _mockHelpService;
    private readonly MaskedTextBox _maskedTextBox;
    private readonly MaskedTextBoxDesigner _designer;
    private readonly MaskedTextBoxDesignerActionList _actionList;

    public MaskedTextBoxDesignerActionListTests()
    {
        _mockTypeDiscoveryService = new Mock<ITypeDiscoveryService>();
        _mockUIService = new Mock<IUIService>();
        _mockHelpService = new Mock<IHelpService>();
        _maskedTextBox = new MaskedTextBox();

        InitializeMocks();

        _designer = new MaskedTextBoxDesigner();
        _designer.Initialize(_maskedTextBox);
        _actionList = new MaskedTextBoxDesignerActionList(_designer);

        void InitializeMocks()
        {
            Mock<ISite> mockSite = new Mock<ISite>();
            mockSite.Setup(s => s.GetService(typeof(ITypeDiscoveryService))).Returns(_mockTypeDiscoveryService.Object);
            mockSite.Setup(s => s.GetService(typeof(IUIService))).Returns(_mockUIService.Object);
            mockSite.Setup(s => s.GetService(typeof(IHelpService))).Returns(_mockHelpService.Object);
            _maskedTextBox.Site = mockSite.Object;
        }
    }

    public void Dispose()
    {
        _maskedTextBox?.Dispose();
        _designer?.Dispose();
    }

    [Fact]
    public void Constructor_InitializesServices()
    {
        MaskedTextBoxDesignerActionList actionList = new MaskedTextBoxDesignerActionList(_designer);

        actionList.Should().NotBeNull();
        actionList.Component.Should().Be(_maskedTextBox);
    }

    [Fact]
    public void SetMask_ValidMask_UpdatesMaskedTextBoxMask()
    {
        string expectedMask = "000-00-0000";
        _mockTypeDiscoveryService.Setup(s => s.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(new[] { typeof(string) });
        _mockUIService.Setup(s => s.ShowDialog(It.IsAny<Form>())).Returns(DialogResult.OK);

        string? resultMask = MaskPropertyEditor.EditMask(
            _mockTypeDiscoveryService.Object,
            _mockUIService.Object,
            _maskedTextBox,
            _mockHelpService.Object);

        resultMask.Should().NotBeNull();
        expectedMask = resultMask ?? expectedMask;

        _actionList.SetMask();

        _maskedTextBox.Mask.Should().Be(expectedMask);
    }

    [Fact]
    public void SetMask_NullMask_DoesNotUpdateMaskedTextBoxMask()
    {
        string initialMask = _maskedTextBox.Mask;
        _mockTypeDiscoveryService.Setup(s => s.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(new[] { typeof(string) });
        _mockUIService.Setup(s => s.ShowDialog(It.IsAny<Form>())).Returns(DialogResult.OK);

        _actionList.SetMask();

        _maskedTextBox.Mask.Should().Be(initialMask);
    }

    [Fact]
    public void SetMask_UiServiceReturnsCancel_DoesNotUpdateMaskedTextBoxMask()
    {
        string initialMask = _maskedTextBox.Mask;
        _mockTypeDiscoveryService.Setup(s => s.GetTypes(It.IsAny<Type>(), It.IsAny<bool>())).Returns(new[] { typeof(string) });
        _mockUIService.Setup(s => s.ShowDialog(It.IsAny<Form>())).Returns(DialogResult.Cancel);

        _actionList.SetMask();

        _maskedTextBox.Mask.Should().Be(initialMask);
    }

    [Fact]
    public void GetSortedActionItems_ReturnsCorrectItems()
    {
        DesignerActionItemCollection items = _actionList.GetSortedActionItems();

        items.Count.Should().Be(1);
        items[0].DisplayName.Should().Be(SR.MaskedTextBoxDesignerVerbsSetMaskDesc);
    }
}
