// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Moq;

namespace System.Windows.Forms.Tests;

public class ToolStripPanelDesignerTests : IDisposable
{
    private readonly Mock<ISite> _mockSite = new();
    private readonly ToolStripPanelDesigner _designer;
    private readonly Mock<IDesignerHost> _mockDesignerHost = new();
    private readonly ToolStripPanel _toolStripPanel;
    private readonly Mock<ISelectionService> _mockSelectionService = new();

    public ToolStripPanelDesignerTests()
    {
        _designer = new();
        _toolStripPanel = InitializeToolStripPanel();
    }

    public void Dispose()
    {
        _toolStripPanel?.Dispose();
        _designer.Dispose();
    }

    private ToolStripPanel InitializeToolStripPanel()
    {
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_mockSelectionService.Object);
        _mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);

        Mock<IComponentChangeService> mockComponentChangeService = new();
        _mockDesignerHost.Setup(dh => dh.GetService(typeof(IComponentChangeService))).Returns(mockComponentChangeService.Object);

        ToolStripPanel toolStripPanel = new() { Site = _mockSite.Object };

        _designer.Initialize(toolStripPanel);

        return toolStripPanel;
    }

    [Fact]
    public void Control_ReturnsCorrectType()
    {
        _toolStripPanel.Should().NotBeNull();
        _toolStripPanel.Should().BeOfType<ToolStripPanel>();
    }

    [Fact]
    public void SelectionRules_ReturnsCorrectValue()
    {
        _mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);
        _mockDesignerHost.Setup(dh => dh.GetService(typeof(IComponentChangeService))).Returns(Mock.Of<IComponentChangeService>);

        SelectionRules selectionRules = _designer.SelectionRules;

        selectionRules.Should().Be(SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible);
    }

    [Fact]
    public void ToolStripPanelSelectorGlyph_ReturnsCorrectValue() => _designer.ToolStripPanelSelectorGlyph.Should().BeNull();

    [Fact]
    public void ParticipatesWithSnapLines_ReturnsFalse() => _designer.ParticipatesWithSnapLines.Should().BeFalse();

    [Fact]
    public void CanParent_ReturnsTrue_WhenControlIsToolStrip()
    {
        using ToolStrip toolStrip = new();

        _designer.CanParent(toolStrip).Should().BeTrue();
    }

    [Fact]
    public void CanParent_ReturnsFalse_WhenControlIsNotToolStrip()
    {
        using Label label = new();

        _designer.CanParent(label).Should().BeFalse();
    }

    [Fact]
    public void CanBeParentedTo_ReturnsFalse_WhenParentIsToolStripContainer()
    {
        using ToolStripContainer toolStripContainer = new();
        ToolStripPanel toolStripPanel = toolStripContainer.TopToolStripPanel;
        Mock<IDesigner> mockDesigner = new();
        toolStripPanel.Site = _mockSite.Object;

        _designer.Initialize(toolStripPanel);

        _designer.CanBeParentedTo(mockDesigner.Object).Should().BeFalse();
    }

    [Fact]
    public void CanBeParentedTo_ReturnsTrue_WhenParentIsNotToolStripContainer()
    {
        Mock<IDesigner> mockDesigner = new();

        _designer.CanBeParentedTo(mockDesigner.Object).Should().BeTrue();
    }

    [Fact]
    public void Initialize_SetsUpCorrectly()
    {
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<IComponentChangeService> mockComponentChangeService = new();
        _mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);
        mockDesignerHost.Setup(dh => dh.GetService(typeof(IComponentChangeService))).Returns(mockComponentChangeService.Object);

        _designer.Initialize(_toolStripPanel);

        mockDesignerHost.Verify(dh => dh.GetService(typeof(IComponentChangeService)), Times.Exactly(2));
        _mockSelectionService.VerifyAdd(s => s.SelectionChanging += It.IsAny<EventHandler>(), Times.Once);
        _mockSelectionService.VerifyAdd(s => s.SelectionChanged += It.IsAny<EventHandler>(), Times.Once);
        mockComponentChangeService.VerifyAdd(s => s.ComponentChanged += It.IsAny<ComponentChangedEventHandler>(), Times.Once);
    }
}
