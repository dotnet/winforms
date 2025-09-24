// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripKeyboardHandlingServiceTests
{
    private readonly Mock<ISelectionService> _selectionServiceMock;
    private readonly Mock<IDesignerHost> _designerHostMock;
    private readonly Mock<IComponentChangeService> _componentChangeServiceMock;
    private readonly DummyServiceProvider _provider;

    public ToolStripKeyboardHandlingServiceTests()
    {
        _selectionServiceMock = new();
        _designerHostMock = new();
        _componentChangeServiceMock = new();

        _provider = new(type =>
            type == typeof(ISelectionService) ? _selectionServiceMock.Object :
            type == typeof(IDesignerHost) ? _designerHostMock.Object :
            type == typeof(IComponentChangeService) ? _componentChangeServiceMock.Object :
            null);

        _designerHostMock.Setup(h => h.GetService(typeof(IComponentChangeService))).Returns(_componentChangeServiceMock.Object);
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
        ToolStripKeyboardHandlingService service = new(_provider);

        _selectionServiceMock.VerifyAdd(s => s.SelectionChanging += It.IsAny<EventHandler>(), Times.Once());
        _selectionServiceMock.VerifyAdd(s => s.SelectionChanged += It.IsAny<EventHandler>(), Times.Once());
        _componentChangeServiceMock.VerifyAdd(s => s.ComponentRemoved += It.IsAny<ComponentEventHandler>(), Times.Once());
    }

    [Fact]
    public void AddCommands_DoesNotThrow_WhenNoMenuService()
    {
        ToolStripKeyboardHandlingService service = new(_provider);
        service.AddCommands();
    }

    [Fact]
    public void RestoreCommands_DoesNotThrow_WhenNoMenuService()
    {
        ToolStripKeyboardHandlingService service = new(_provider);
        service.RestoreCommands();
    }

    [Fact]
    public void RemoveCommands_DoesNotThrow_WhenNoMenuService()
    {
        ToolStripKeyboardHandlingService service = new(_provider);
        service.RemoveCommands();
    }

    [Fact]
    public void OnContextMenu_ReturnsTrue_WhenTemplateNodeActive()
    {
        ToolStripKeyboardHandlingService service = new(_provider)
        {
            TemplateNodeActive = true
        };
        service.OnContextMenu(10, 10).Should().BeTrue();
    }

    [Fact]
    public void OnContextMenu_ReturnsFalse_WhenNotTemplateNodeActive()
    {
        ToolStripKeyboardHandlingService service = new(_provider)
        {
            TemplateNodeActive = false
        };
        service.OnContextMenu(10, 10).Should().BeFalse();
    }

    [Fact]
    public void ProcessKeySelect_DoesNotThrow_WhenNoSelectionService()
    {
        ToolStripKeyboardHandlingService service = new(_provider);
        service.ProcessKeySelect(false);
    }

    [Fact]
    public void ProcessUpDown_DoesNotThrow_WhenNoSelectionService()
    {
        ToolStripKeyboardHandlingService service = new(_provider);
        service.ProcessUpDown(false);
    }

    [Fact]
    public void RotateTab_DoesNotThrow_WhenNoSelectionService()
    {
        ToolStripKeyboardHandlingService service = new(_provider);
        service.RotateTab(false);
    }
}
