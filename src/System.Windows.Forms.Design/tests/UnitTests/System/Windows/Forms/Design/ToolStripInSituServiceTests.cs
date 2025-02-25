// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public sealed class ToolStripInSituServiceTests : IDisposable
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IDesignerHost> _mockDesignerHost;
    private readonly Mock<IComponentChangeService> _mockComponentChangeService;
    private readonly Mock<ToolStripDesigner> _mockToolStripDesigner;
    private readonly Mock<ToolStripItemDesigner> _mockToolStripItemDesigner;
    private readonly Mock<ISelectionService> _mockSelectionService;
    private readonly Mock<ToolStripKeyboardHandlingService> _mockToolStripKeyboardHandlingService;
    private readonly ToolStripInSituService _inSituService;

    public ToolStripInSituServiceTests()
    {
        _mockServiceProvider = new();
        _mockDesignerHost = new();
        _mockComponentChangeService = new();
        _mockToolStripDesigner = new();
        _mockToolStripItemDesigner = new();
        _mockSelectionService = new();
        _mockToolStripKeyboardHandlingService = new(_mockServiceProvider.Object);

        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IDesignerHost))).Returns(_mockDesignerHost.Object);
        _mockDesignerHost.Setup(dh => dh.GetService(typeof(IComponentChangeService))).Returns(_mockComponentChangeService.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISelectionService))).Returns(_mockSelectionService.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ToolStripKeyboardHandlingService))).Returns(_mockToolStripKeyboardHandlingService.Object);

        _inSituService = new(_mockServiceProvider.Object);
        typeof(ToolStripInSituService).GetField("_toolDesigner", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(_inSituService, _mockToolStripDesigner.Object);
        typeof(ToolStripInSituService).GetField("_toolItemDesigner", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(_inSituService, _mockToolStripItemDesigner.Object);
        typeof(ToolStripInSituService).GetField("_componentChangeService", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(_inSituService, _mockComponentChangeService.Object);
    }

    public void Dispose() => _inSituService.Dispose();

    [Fact]
    public void Dispose_DisposesToolDesigner()
    {
        _inSituService.Dispose();

        FieldInfo? toolDesignerField = typeof(ToolStripInSituService).GetField("_toolDesigner", BindingFlags.NonPublic | BindingFlags.Instance);
        object? toolDesignerValue = toolDesignerField?.GetValue(_inSituService);
        toolDesignerValue.Should().BeNull();
    }

    [Fact]
    public void Dispose_DisposesToolItemDesigner()
    {
        _inSituService.Dispose();

        FieldInfo? toolItemDesignerField = typeof(ToolStripInSituService).GetField("_toolItemDesigner", BindingFlags.NonPublic | BindingFlags.Instance);
        object? toolItemDesignerValue = toolItemDesignerField?.GetValue(_inSituService);
        toolItemDesignerValue.Should().BeNull();
    }

    [Fact]
    public void Dispose_UnsubscribesFromComponentChangeService()
    {
        _inSituService.Dispose();

        FieldInfo? componentChangeServiceField = typeof(ToolStripInSituService).GetField("_componentChangeService", BindingFlags.NonPublic | BindingFlags.Instance);
        object? componentChangeServiceValue = componentChangeServiceField?.GetValue(_inSituService);
        componentChangeServiceValue.Should().BeNull();
    }

    [Fact]
    public void ToolStripKeyBoardService_ReturnsServiceInstance()
    {
        PropertyInfo? toolStripKeyBoardServiceProperty = typeof(ToolStripInSituService).GetProperty("ToolStripKeyBoardService", BindingFlags.NonPublic | BindingFlags.Instance);
        object? toolStripKeyBoardService = toolStripKeyBoardServiceProperty?.GetValue(_inSituService);

        toolStripKeyBoardService.Should().NotBeNull();
        toolStripKeyBoardService.Should().BeAssignableTo<ToolStripKeyboardHandlingService>();
        toolStripKeyBoardService.Should().Be(_mockToolStripKeyboardHandlingService.Object);
    }

    [Fact]
    public void IgnoreMessages_ReturnsFalse_WhenSelectionServiceIsNull()
    {
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISelectionService))).Returns(null!);
        bool result = _inSituService.IgnoreMessages;
        result.Should().BeFalse();
    }

    [Fact]
    public void IgnoreMessages_ReturnsFalse_WhenDesignerHostIsNull()
    {
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IDesignerHost))).Returns(null!);
        bool result = _inSituService.IgnoreMessages;
        result.Should().BeFalse();
    }

    [Fact]
    public void GetEditWindow_ReturnsZero_WhenNoDesignerIsNotNull()
    {
        IntPtr result = _inSituService.GetEditWindow();
        result.Should().Be(IntPtr.Zero);
    }

    [Fact]
    public void OnComponentRemoved_RemovesService_WhenNoToolStripPresent()
    {
        Mock<IComponent> mockComponent = new();
        Mock<IContainer> mockContainer = new();
        ComponentCollection componentCollection = new(Array.Empty<IComponent>());

        mockContainer.Setup(c => c.Components).Returns(componentCollection);
        _mockDesignerHost.Setup(dh => dh.Container).Returns(mockContainer.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISupportInSituService))).Returns(_inSituService);

        _inSituService.GetType().GetMethod("OnComponentRemoved", BindingFlags.NonPublic | BindingFlags.Instance)
                      ?.Invoke(_inSituService, [null, new ComponentEventArgs(mockComponent.Object)]);

        _mockDesignerHost.Verify(dh => dh.RemoveService(typeof(ISupportInSituService)), Times.Once);
    }

    [Fact]
    public void OnComponentRemoved_DoesNotRemoveService_WhenToolStripPresent()
    {
        Mock<IComponent> mockComponent = new();
        Mock<ToolStrip> mockToolStrip = new();
        Mock<IContainer> mockContainer = new();
        ComponentCollection realComponentCollection = new([mockToolStrip.Object]);

        mockContainer.Setup(c => c.Components).Returns(realComponentCollection);
        _mockDesignerHost.Setup(dh => dh.Container).Returns(mockContainer.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISupportInSituService))).Returns(_inSituService);

        _inSituService.GetType().GetMethod("OnComponentRemoved", BindingFlags.NonPublic | BindingFlags.Instance)
                      ?.Invoke(_inSituService, [null, new ComponentEventArgs(mockComponent.Object)]);

        _mockDesignerHost.Verify(dh => dh.RemoveService(typeof(ISupportInSituService)), Times.Never);
    }
}
