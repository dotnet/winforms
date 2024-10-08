// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripContainerDesignerTests : IDisposable
{
    private readonly Mock<IDesignerHost> _mockDesignerHost;
    private readonly Mock<ISelectionService> _mockSelectionService;
    private readonly ToolStripContainerDesigner _designer;
    private readonly ToolStripContainer _toolStripContainer;

    public ToolStripContainerDesignerTests()
    {
        _mockDesignerHost = new();
        _mockSelectionService = new();
        _toolStripContainer = new();
        InitializeMocks();

        _designer = new();
        _designer.Initialize(_toolStripContainer);

        void InitializeMocks()
        {
            Mock<ISite> mockSite = new();
            mockSite.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);
            mockSite.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_mockSelectionService.Object);
            _toolStripContainer.Site = mockSite.Object;
        }
    }

    [Fact]
    public void ActionLists_ShouldNotBeNull() => _designer.ActionLists.Should().NotBeNull();

    [Fact]
    public void ActionLists_ShouldContainToolStripContainerActionList()
    {
        var actionLists = _designer.ActionLists;
        actionLists[0].Should().BeOfType<ToolStripContainerActionList>();
    }

    [Fact]
    public void ActionLists_ToolStripContainerActionList_ShouldAutoShow()
    {
        var toolStripContainerActionList = _designer.ActionLists.OfType<ToolStripContainerActionList>().Single();
        toolStripContainerActionList.AutoShow.Should().BeTrue();
    }

    [Fact]
    public void SnapLines_ShouldReturnExpectedValue() => _designer.SnapLines.Should().NotBeNull();

    [Fact]
    public void InternalControlDesigner_ShouldReturnExpectedValue() => _designer.InternalControlDesigner(0).Should().BeNull();

    [Fact]
    public void AssociatedComponents_ShouldNotBeNull() => _designer.AssociatedComponents.Should().NotBeNull();

    [Fact]
    public void CanParent_ShouldReturnFalse()
    {
        using Control control = new();
        _designer.CanParent(control).Should().BeFalse();
    }

    [Fact]
    public void Initialize_ShouldSetToolStripContainer()
    {
        var field = typeof(ToolStripContainerDesigner).GetField("_toolStripContainer", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = field?.GetValue(_designer);
        value.Should().Be(_toolStripContainer);
    }

    [Fact]
    public void Initialize_ShouldSetPanels()
    {
        var field = typeof(ToolStripContainerDesigner).GetField("_panels", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = field?.GetValue(_designer) as Control[];

        value.Should().NotBeNull();
        value.Should().Contain(_toolStripContainer.TopToolStripPanel);
        value.Should().Contain(_toolStripContainer.BottomToolStripPanel);
        value.Should().Contain(_toolStripContainer.LeftToolStripPanel);
        value.Should().Contain(_toolStripContainer.RightToolStripPanel);
        value.Should().Contain(_toolStripContainer.ContentPanel);
    }

    [Fact]
    public void Initialize_ShouldSetShadowProperties()
    {
        AssertShadowProperties("TopToolStripPanelVisible", _toolStripContainer.TopToolStripPanelVisible);
        AssertShadowProperties("LeftToolStripPanelVisible", _toolStripContainer.LeftToolStripPanelVisible);
        AssertShadowProperties("RightToolStripPanelVisible", _toolStripContainer.RightToolStripPanelVisible);
        AssertShadowProperties("BottomToolStripPanelVisible", _toolStripContainer.BottomToolStripPanelVisible);

        void AssertShadowProperties(string propertyName, object expectedValue)
        {
            var property = typeof(ToolStripContainerDesigner).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            var actualValue = property?.GetValue(_designer);
            actualValue.Should().Be(expectedValue);
        }
    }

    public void Dispose() => ((IDisposable)_toolStripContainer).Dispose();
}
