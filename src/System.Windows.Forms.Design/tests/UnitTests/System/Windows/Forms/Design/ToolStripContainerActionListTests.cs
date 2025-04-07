// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design.Tests;

public sealed class ToolStripContainerActionListTests : IDisposable
{
    private readonly ToolStripContainer _toolStripContainer;
    private readonly Mock<IDesignerHost> _designerHostMock;
    private readonly Mock<ISite> _siteMock;
    private readonly ToolStripContainerActionList _actionList;

    public ToolStripContainerActionListTests()
    {
        _toolStripContainer = new();
        _designerHostMock = new();
        _siteMock = new();
        _siteMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _toolStripContainer.Site = _siteMock.Object;
        _actionList = new(_toolStripContainer);
    }

    public void Dispose()
    {
        _designerHostMock.Reset();
        _toolStripContainer.Dispose();
        _siteMock.Reset();
    }

    [Fact]
    public void SetDockToForm_SetsDockToFill()
    {
        _actionList.SetDockToForm();
        _toolStripContainer.Dock.Should().Be(DockStyle.Fill);
    }

    [Fact]
    public void SetDockToForm_AddsToolStripContainerToRootComponent()
    {
        using Control rootComponent = new();
        _designerHostMock.Setup(dh => dh.RootComponent).Returns(rootComponent);
        _actionList.SetDockToForm();
        rootComponent.Controls.Cast<Control>().Should().Contain(_toolStripContainer);
    }

    [Fact]
    public void ReparentControls_ReparentsControlsToContentPanel()
    {
        using Control rootComponent = new();
        using Control childControl1 = new();
        using Control childControl2 = new();
        rootComponent.Controls.Add(_toolStripContainer);
        rootComponent.Controls.Add(childControl1);
        rootComponent.Controls.Add(childControl2);

        _designerHostMock.Setup(dh => dh.RootComponent).Returns(rootComponent);
        Mock<IComponentChangeService> componentChangeServiceMock = new();
        _siteMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(componentChangeServiceMock.Object);

        _actionList.ReparentControls();

        _toolStripContainer.ContentPanel.Controls.Cast<Control>().Should().Contain(childControl1);
        _toolStripContainer.ContentPanel.Controls.Cast<Control>().Should().Contain(childControl2);
        rootComponent.Controls.Cast<Control>().Should().NotContain(childControl1);
        rootComponent.Controls.Cast<Control>().Should().NotContain(childControl2);
    }

    [Fact]
    public void ReparentControls_DoesNotReparentInheritedControls()
    {
        using Control rootComponent = new();
        using Control inheritedControl = new();
        TypeDescriptor.AddAttributes(inheritedControl, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
        rootComponent.Controls.Add(_toolStripContainer);
        rootComponent.Controls.Add(inheritedControl);

        _designerHostMock.Setup(dh => dh.RootComponent).Returns(rootComponent);
        Mock<IComponentChangeService> componentChangeServiceMock = new();
        _siteMock.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(componentChangeServiceMock.Object);

        _actionList.ReparentControls();

        rootComponent.Controls.Cast<Control>().Should().Contain(inheritedControl);
        _toolStripContainer.ContentPanel.Controls.Cast<Control>().Should().NotContain(inheritedControl);
    }

    [Fact]
    public void TopVisible_Get_ReturnsCorrectValue()
    {
        bool topVisible = _actionList.TopVisible;
        topVisible.Should().Be(_toolStripContainer.TopToolStripPanelVisible);
    }

    [Fact]
    public void TopVisible_Set_ChangesValue()
    {
        bool newValue = !_toolStripContainer.TopToolStripPanelVisible;
        _actionList.TopVisible = newValue;
        _toolStripContainer.TopToolStripPanelVisible.Should().Be(newValue);
    }

    [Fact]
    public void BottomVisible_Get_ReturnsCorrectValue()
    {
        bool bottomVisible = _actionList.BottomVisible;
        bottomVisible.Should().Be(_toolStripContainer.BottomToolStripPanelVisible);
    }

    [Fact]
    public void BottomVisible_Set_ChangesValue()
    {
        bool newValue = !_toolStripContainer.BottomToolStripPanelVisible;
        _actionList.BottomVisible = newValue;
        _toolStripContainer.BottomToolStripPanelVisible.Should().Be(newValue);
    }

    [Fact]
    public void LeftVisible_Get_ReturnsCorrectValue()
    {
        bool leftVisible = _actionList.LeftVisible;
        leftVisible.Should().Be(_toolStripContainer.LeftToolStripPanelVisible);
    }

    [Fact]
    public void LeftVisible_Set_ChangesValue()
    {
        bool newValue = !_toolStripContainer.LeftToolStripPanelVisible;
        _actionList.LeftVisible = newValue;
        _toolStripContainer.LeftToolStripPanelVisible.Should().Be(newValue);
    }

    [Fact]
    public void RightVisible_Get_ReturnsCorrectValue()
    {
        bool rightVisible = _actionList.RightVisible;
        rightVisible.Should().Be(_toolStripContainer.RightToolStripPanelVisible);
    }

    [Fact]
    public void RightVisible_Set_ChangesValue()
    {
        bool newValue = !_toolStripContainer.RightToolStripPanelVisible;
        _actionList.RightVisible = newValue;
        _toolStripContainer.RightToolStripPanelVisible.Should().Be(newValue);
    }

    [Fact]
    public void GetSortedActionItems_ReturnsCorrectItems_WhenDockNotFilled()
    {
        DesignerActionItemCollection items = _actionList.GetSortedActionItems();
        var displayNames = items.Cast<DesignerActionItem>()
                                .Select(i => i.DisplayName)
                                .ToList();

        displayNames.Should().ContainSingle(name => name == "Top");
        displayNames.Should().ContainSingle(name => name == "Bottom");
        displayNames.Should().ContainSingle(name => name == "Left");
        displayNames.Should().ContainSingle(name => name == "Right");
        displayNames.Should().ContainSingle(name => name == "Dock Fill in Form");
    }

    [Fact]
    public void GetSortedActionItems_ReturnsCorrectItems_WhenDockFilled()
    {
        PropertyDescriptor dockProp = TypeDescriptor.GetProperties(_toolStripContainer)["Dock"]!;
        dockProp.SetValue(_toolStripContainer, DockStyle.Fill);

        DesignerActionItemCollection items = _actionList.GetSortedActionItems();
        var displayNames = items.Cast<DesignerActionItem>()
                                .Select(i => i.DisplayName)
                                .ToList();

        displayNames.Should().ContainSingle(name => name == "Top");
        displayNames.Should().ContainSingle(name => name == "Bottom");
        displayNames.Should().ContainSingle(name => name == "Left");
        displayNames.Should().ContainSingle(name => name == "Right");
        displayNames.Should().NotContain(name => name == "Dock in Form");
    }

    [Fact]
    public void GetSortedActionItems_ReturnsCorrectItems_WhenProvideReparent()
    {
        using Control rootComponent = new();
        using Control childControl = new();
        rootComponent.Controls.Add(_toolStripContainer);
        rootComponent.Controls.Add(childControl);

        _designerHostMock.Setup(dh => dh.RootComponent).Returns(rootComponent);
        TypeDescriptor.GetProperties(_toolStripContainer)["Dock"]?.SetValue(_toolStripContainer, DockStyle.Fill);

        DesignerActionItemCollection items = _actionList.GetSortedActionItems();
        var displayNames = items.Cast<DesignerActionItem>()
                                .Select(i => i.DisplayName)
                                .ToList();

        displayNames.Should().ContainSingle(name => name == "Top");
        displayNames.Should().ContainSingle(name => name == "Bottom");
        displayNames.Should().ContainSingle(name => name == "Left");
        displayNames.Should().ContainSingle(name => name == "Right");
        displayNames.Should().ContainSingle(name => name == "Re-parent Controls");
    }
}
