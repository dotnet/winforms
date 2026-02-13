// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripKeyboardHandlingServiceTests
{
    private readonly Mock<ISelectionService> _selectionServiceMock;
    private readonly Mock<IDesignerHost> _designerHostMock;
    private readonly Mock<IComponentChangeService> _componentChangeServiceMock;
    private readonly DummyServiceProvider _provider;
    private readonly ToolStripKeyboardHandlingService _service;

    public ToolStripKeyboardHandlingServiceTests()
    {
        _selectionServiceMock = new();
        _designerHostMock = new();
        _componentChangeServiceMock = new();

        _provider = new(type =>
            type == typeof(ISelectionService) ? _selectionServiceMock.Object
            : type == typeof(IDesignerHost) ? _designerHostMock.Object
            : type == typeof(IComponentChangeService) ? _componentChangeServiceMock.Object
            : null);

        _designerHostMock.Setup(h => h.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);

        _service = new(_provider);
    }

    private class DummyServiceProvider : IServiceProvider
    {
        private readonly Func<Type, object?> _serviceResolver;

        public DummyServiceProvider(Func<Type, object?> serviceResolver)
        {
            _serviceResolver = serviceResolver;
        }

        public object? GetService(Type serviceType) => _serviceResolver(serviceType);
    }

    [Fact]
    public void Ctor_InitializesAndSubscribesToEvents()
    {
        _selectionServiceMock.VerifyAdd(s => s.SelectionChanging += It.IsAny<EventHandler>(), Times.Once());
        _selectionServiceMock.VerifyAdd(s => s.SelectionChanged += It.IsAny<EventHandler>(), Times.Once());
        _componentChangeServiceMock.VerifyAdd(s => s.ComponentRemoved += It.IsAny<ComponentEventHandler>(), Times.Once());
    }

    [Fact]
    public void AddCommands_DoesNotThrow_WhenNoMenuService() =>
        ((Action)_service.AddCommands).Should().NotThrow();

    [Fact]
    public void RestoreCommands_DoesNotThrow_WhenNoMenuService() =>
        ((Action)_service.RestoreCommands).Should().NotThrow();

    [Fact]
    public void RemoveCommands_DoesNotThrow_WhenNoMenuService() =>
        ((Action)_service.RemoveCommands).Should().NotThrow();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void OnContextMenu_ReturnsExpectedResult_BasedOnTemplateNodeActive(bool isActive)
    {
        _service.TemplateNodeActive = isActive;

        bool result = _service.OnContextMenu(10, 10);

        result.Should().Be(isActive);
    }

    [Theory]
    [InlineData(typeof(ToolStripItem))]
    [InlineData(typeof(Component))]
    public void OnContextMenu_ReturnsFalse_WhenInvalidSelection(Type mockType)
    {
        if (mockType == typeof(ToolStripItem))
        {
            Mock<ToolStripItem> toolStripItemMock = new();
            _selectionServiceMock.Setup(s => s.PrimarySelection).Returns((Component?)null);
            _service.SelectedDesignerControl = toolStripItemMock.Object;
        }
        else
        {
            Mock<Component> componentMock = new();
            _selectionServiceMock.Setup(s => s.PrimarySelection).Returns(componentMock.Object);
        }

        bool result = _service.OnContextMenu(100, 200);

        result.Should().BeFalse();
    }

    [Fact]
    public void ProcessKeySelect_DoesNotThrow_WhenNoSelectionService()
    {
        Action action = () => _service.ProcessKeySelect(false);

        action.Should().NotThrow();
    }

    [Fact]
    public void ProcessUpDown_DoesNotThrow_WhenNoSelectionService()
    {
        Action action = () => _service.ProcessUpDown(false);

        action.Should().NotThrow();
    }

    [Fact]
    public void RotateTab_DoesNotThrow_WhenNoSelectionService()
    {
        Action action = () => _service.RotateTab(false);

        action.Should().NotThrow();
    }
}
