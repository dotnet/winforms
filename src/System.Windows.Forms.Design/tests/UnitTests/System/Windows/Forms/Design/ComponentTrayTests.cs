// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ComponentTrayTests : IDisposable
{
    private readonly ComponentTray _componentTray;
    private readonly Component _component = new Mock<ContextMenuStrip>().Object;

    public ComponentTrayTests()
    {
        _componentTray = new(new Mock<IDesigner>().Object, MockServiceProvider().Object);
        _componentTray.AddComponent(_component);
    }

    public void Dispose()
    {
        _component.Dispose();
        _componentTray.Dispose();
    }

    [Fact]
    public void Constructor_Default()
    {
        ComponentTray componentTray = new(new Mock<IDesigner>().Object, MockServiceProvider().Object);
        componentTray.Should().BeOfType<ComponentTray>();
        componentTray.Visible.Should().BeTrue();
        componentTray.AutoScroll.Should().BeTrue();
        componentTray.AllowDrop.Should().BeTrue();
        componentTray.Text.Should().Be("ComponentTray");
        componentTray.Controls.Count.Should().Be(0);
        componentTray.ComponentCount.Should().Be(0);
        componentTray.AutoArrange.Should().BeFalse();
    }

    [Fact]
    public void AutoArrange_SetsPropertiesProperly()
    {
        _componentTray.AutoArrange = true;
        _componentTray.AutoArrange.Should().BeTrue();
        _componentTray.ShowLargeIcons = true;
        _componentTray.ShowLargeIcons.Should().BeTrue();
    }

    [Fact]
    public void AddComponent_AddsComponent_DoesNotThrow()
    {
        Action action = () => _componentTray.AddComponent(new Mock<Component>().Object);

        action.Should().NotThrow<Exception>();
    }

    [Fact]
    public void CreateComponentFromTool_DoesNotThrow()
    {
        Action action = () => _componentTray.CreateComponentFromTool(new Mock<ToolboxItem>().Object);

        action.Should().NotThrow();
    }

    [Fact]
    public void GetNextComponent_WithOneComponent_ReturnsNull() =>
        _componentTray.GetNextComponent(_component, true).Should().BeNull();

    [Fact]
    public void GetNextComponent_WithOneAddedComponent_ReturnsIt() =>
        _componentTray.GetNextComponent(new Mock<Component>().Object, true).Should().Be(_component);

    [Fact]
    public void GetNextComponent_WithSameComponentAdded_ReturnsNull() =>
        _componentTray.GetNextComponent(_component, true).Should().BeNull();

    [Fact]
    public void GetNextComponent_WhenAddedDifferentThenSameThenNew_ReturnsTheFirstAddedOne()
    {
        Mock<Component> sameComponentMock = new();
        _componentTray.AddComponent(sameComponentMock.Object);
        Mock<Component> lastComponentMock = new();
        _componentTray.AddComponent(lastComponentMock.Object);
        _componentTray.GetNextComponent(sameComponentMock.Object, true).Should().Be(_component);
    }

    [Fact]
    public void GetLocation_ReturnsLocation() => _componentTray.GetLocation(new Mock<ContextMenuStrip>().Object).Should().Be(new Point() { X = 0, Y = 0 });

    [Fact]
    public void IsTrayComponent_WithoutAComponentView_ReturnsFalse() =>
        _componentTray.IsTrayComponent(_component).Should().BeFalse();

    [Fact]
    public void RemoveComponent_DoesNotThrow()
    {
        Action action = () =>
        {
            _componentTray.RemoveComponent(_component);
        };

        action.Should().NotThrow<Exception>();
    }

    [Fact]
    public void SetLocation_DoesNotThrow()
    {
        Action action = () => _componentTray.SetLocation(_component, new(0, 0));
        action.Should().NotThrow<Exception>();
    }

    private Mock<IServiceProvider> MockServiceProvider()
    {
        var designerHostMock = new Mock<IDesignerHost>();
        Dictionary<Type, object> serviceMap = new()
        {
            { typeof(IExtenderProviderService), new Mock<IExtenderProviderService>().Object},
            { typeof(IDesignerHost), designerHostMock.Object},
            { typeof(IEventHandlerService), new Mock<IEventHandlerService>().Object },
            { typeof(IMenuCommandService), new Mock<IMenuCommandService>().Object},
            { typeof(IComponentChangeService), new Mock<IComponentChangeService>().Object},
            { typeof(ISelectionService), new Mock<ISelectionService>().Object},
        };

        Mock<IServiceProvider> serviceProviderMock = new();

        foreach (KeyValuePair<Type, object> service in serviceMap)
        {
            serviceProviderMock
                .Setup(s => s.GetService(service.Key))
                .Returns(service.Value);
        }

        return serviceProviderMock;
    }

    [Fact]
    public void AddComponent_WithValidComponent_AddsToTray()
    {
        using Component component = new();
        int initialCount = _componentTray.ComponentCount;

        _componentTray.AddComponent(component);

        _componentTray.ComponentCount.Should().Be(initialCount + 1);
    }

    [Fact]
    public void AddComponent_WithNonDisplayableComponent_DoesNotAddToTray()
    {
        using Component component = new MyCustomComponent();
        Mock<ISelectionUIService> selectionUISvcMock = new();
        Mock<IServiceProvider> serviceProviderMock = MockServiceProvider();
        serviceProviderMock
            .Setup(s => s.GetService(typeof(ISelectionUIService)))
            .Returns(selectionUISvcMock.Object);

        ComponentTray componentTray = new(new Mock<IDesigner>().Object, serviceProviderMock.Object);
        componentTray.AddComponent(component);

        componentTray.ComponentCount.Should().Be(0);
    }

    [DesignTimeVisible(false)]
    public class MyCustomComponent : Component
    {
    }

    [Fact]
    public void AddComponent_WhenHostIsLoading_DoesNotPositionControl()
    {
        Mock<IDesignerHost> hostMock = new();
        hostMock.Setup(h => h.Loading).Returns(true);

        Mock<IServiceProvider> serviceProviderMock = MockServiceProvider();
        serviceProviderMock
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(hostMock.Object);

        ComponentTray componentTray = new(new Mock<IDesigner>().Object, serviceProviderMock.Object);
        Mock<Component> mockComponent = new();
        int initialCount = componentTray.ComponentCount;

        componentTray.AddComponent(mockComponent.Object);

        componentTray.ComponentCount.Should().Be(initialCount + 1);
    }

[Fact]
public void AutoArrange_TogglesCorrectly()
{
    _componentTray.AutoArrange = true;
    _componentTray.AutoArrange.Should().BeTrue();

    _componentTray.AutoArrange = false;
    _componentTray.AutoArrange.Should().BeFalse();
}

[Fact]
public void ShowLargeIcons_TogglesCorrectly()
{
    _componentTray.ShowLargeIcons = true;
    _componentTray.ShowLargeIcons.Should().BeTrue();

    _componentTray.ShowLargeIcons = false;
    _componentTray.ShowLargeIcons.Should().BeFalse();
}

[Fact]
public void ComponentCount_ReturnsCorrectCount()
{
    int initialCount = _componentTray.ComponentCount;

    using Component newComponent = new();
    _componentTray.AddComponent(newComponent);

    _componentTray.ComponentCount.Should().Be(initialCount + 1);
}

[Fact]
public void IsTrayComponent_WithComponentNotInTray_ReturnsFalse()
{
    Mock<IComponent> mockComponent = new();
    _componentTray.IsTrayComponent(mockComponent.Object).Should().BeFalse();
}

[Fact]
public void CreateComponentFromTool_WithValidTool_DoesNotThrow()
{
    Mock<ToolboxItem> mockToolboxItem = new();
    Action action = () => _componentTray.CreateComponentFromTool(mockToolboxItem.Object);

    action.Should().NotThrow();
}
}
