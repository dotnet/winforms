// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;
public sealed class SplitterPanelDesignerTests
{
    [Fact]
    public void Initialize_CanBeParentedTo_ReturnsExpectedResults()
    {
        using ToolStripContainerDesigner toolStripContainerDesigner = new();
        using SplitContainerDesigner splitContainerDesigner = new();
        using SplitContainer splitContainer = new();
        using SplitterPanel splitterPanel = new(splitContainer);
        using SplitterPanelDesigner splitterPanelDesigner = new();

        Mock<IDesignerHost> mockDesignerHost = new();
        mockDesignerHost.Setup(dh => dh.GetDesigner(splitterPanel.Parent!)).Returns(splitContainerDesigner);

        Mock<ISite> mockSite = new();
        mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(mockDesignerHost.Object);

        Mock<IComponentChangeService> mockChangeService = new();
        mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(mockChangeService.Object);
        mockDesignerHost.Setup(dh => dh.GetService(typeof(IComponentChangeService))).Returns(mockChangeService.Object);

        using ToolStripContainer toolStripContainer = new();
        toolStripContainer.Site = mockSite.Object;
        toolStripContainerDesigner.Initialize(toolStripContainer);

        splitContainer.Site = mockSite.Object;
        splitContainerDesigner.Initialize(splitContainer);

        splitterPanel.Site = mockSite.Object;

        mockChangeService.VerifyAdd(cs => cs.ComponentChanged += It.IsAny<ComponentChangedEventHandler>(), Times.Never());

        splitterPanelDesigner.Initialize(splitterPanel);

        mockChangeService.VerifyAdd(cs => cs.ComponentChanged += It.IsAny<ComponentChangedEventHandler>(), Times.Once());

        splitterPanelDesigner.CanBeParentedTo(splitContainerDesigner).Should().Be(true);
        splitterPanelDesigner.CanBeParentedTo(toolStripContainerDesigner).Should().Be(false);

        ((SplitterPanel)splitterPanelDesigner.TestAccessor().Dynamic._splitterPanel).Should().Be(splitterPanel);
        ((IDesignerHost)splitterPanelDesigner.TestAccessor().Dynamic._designerHost).Should().Be(mockDesignerHost.Object);
        ((SplitContainerDesigner)splitterPanelDesigner.TestAccessor().Dynamic._splitContainerDesigner).Should().Be(splitContainerDesigner);

        IList snapLines = splitterPanelDesigner.SnapLines;
        snapLines.Should().NotBeNull();
        snapLines.Should().BeOfType<ArrayList>();
        snapLines.Count.Should().BeGreaterThan(0);

        SelectionRules selectionRules = splitterPanelDesigner.SelectionRules;
        selectionRules.Should().Be(SelectionRules.None);
    }
}
