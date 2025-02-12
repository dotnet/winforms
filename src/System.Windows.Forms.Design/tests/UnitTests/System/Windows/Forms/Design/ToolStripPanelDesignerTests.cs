// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Moq;

namespace System.Windows.Forms.Tests;

public class ToolStripPanelDesignerTests
{
    private readonly Mock<ISite> _mockSite = new();
    private ToolStripPanelDesigner? _designer;

    [Fact]
    public void Control_ReturnsCorrectType()
    {
        ToolStripPanel toolStripPanel = new();
        Mock<ISelectionService> mockSelectionService = new();
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        toolStripPanel.Site = _mockSite.Object;

        _designer = new ToolStripPanelDesigner();
        _designer.Initialize(toolStripPanel);

        ToolStripPanel control = _designer.Control;

        control.Should().BeOfType<ToolStripPanel>();
        control.Should().Be(toolStripPanel);
    }

    [Fact]
    public void ParticipatesWithSnapLines_ReturnsFalse()
    {
        ToolStripPanel toolStripPanel = new();
        Mock<ISelectionService> mockSelectionService = new();
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        toolStripPanel.Site = _mockSite.Object;

        _designer = new ToolStripPanelDesigner();
        _designer.Initialize(toolStripPanel);

        bool participatesWithSnapLines = _designer.ParticipatesWithSnapLines;

        participatesWithSnapLines.Should().BeFalse();
    }

    [Fact]
    public void CanParent_ReturnsTrue_WhenControlIsToolStrip()
    {
        ToolStripPanel toolStripPanel = new();
        Mock<ISelectionService> mockSelectionService = new();
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        toolStripPanel.Site = _mockSite.Object;

        _designer = new ToolStripPanelDesigner();
        _designer.Initialize(toolStripPanel);

        ToolStrip toolStrip = new();

        bool canParent = _designer.CanParent(toolStrip);

        canParent.Should().BeTrue();
    }

    [Fact]
    public void CanParent_ReturnsFalse_WhenControlIsNotToolStrip()
    {
        ToolStripPanel toolStripPanel = new();
        Mock<ISelectionService> mockSelectionService = new();
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        toolStripPanel.Site = _mockSite.Object;

        _designer = new ToolStripPanelDesigner();
        _designer.Initialize(toolStripPanel);

        Label label = new();

        bool canParent = _designer.CanParent(label);

        canParent.Should().BeFalse();
    }

    [Fact]
    public void CanBeParentedTo_ReturnsFalse_WhenParentIsToolStripContainer()
    {
        ToolStripContainer toolStripContainer = new();
        ToolStripPanel toolStripPanel = toolStripContainer.TopToolStripPanel;
        Mock<ISelectionService> mockSelectionService = new();
        Mock<IDesigner> mockDesigner = new();
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        toolStripPanel.Site = _mockSite.Object;

        _designer = new ToolStripPanelDesigner();
        _designer.Initialize(toolStripPanel);

        bool canBeParentedTo = _designer.CanBeParentedTo(mockDesigner.Object);

        canBeParentedTo.Should().BeFalse();
    }

    [Fact]
    public void CanBeParentedTo_ReturnsTrue_WhenParentIsNotToolStripContainer()
    {
        ToolStripPanel toolStripPanel = new();
        Mock<ISelectionService> mockSelectionService = new();
        Mock<IDesigner> mockDesigner = new();
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        toolStripPanel.Site = _mockSite.Object;

        _designer = new ToolStripPanelDesigner();
        _designer.Initialize(toolStripPanel);

        bool canBeParentedTo = _designer.CanBeParentedTo(mockDesigner.Object);

        canBeParentedTo.Should().BeTrue();
    }

    [Fact]
    public void Initialize_SetsUpCorrectly()
    {
        Mock<IDesignerHost> mockDesignerHost = new();
        Mock<ISelectionService> mockSelectionService = new();
        Mock<IComponentChangeService> mockComponentChangeService = new();
        ToolStripPanel toolStripPanel = new();
        _mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);
        _mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(mockSelectionService.Object);
        mockDesignerHost.Setup(dh => dh.GetService(typeof(IComponentChangeService))).Returns(mockComponentChangeService.Object);
        toolStripPanel.Site = _mockSite.Object;

        _designer = new ToolStripPanelDesigner();
        _designer.Initialize(toolStripPanel);

        mockDesignerHost.Verify(dh => dh.GetService(typeof(IComponentChangeService)), Times.Exactly(2));
        mockSelectionService.VerifyAdd(s => s.SelectionChanging += It.IsAny<EventHandler>(), Times.Once);
        mockSelectionService.VerifyAdd(s => s.SelectionChanged += It.IsAny<EventHandler>(), Times.Once);
        mockComponentChangeService.VerifyAdd(s => s.ComponentChanged += It.IsAny<ComponentChangedEventHandler>(), Times.Once);
    }
}
