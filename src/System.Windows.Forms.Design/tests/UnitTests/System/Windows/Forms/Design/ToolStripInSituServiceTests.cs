// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.ComponentModel.Design;
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
        _inSituService.TestAccessor().Dynamic._toolDesigner = _mockToolStripDesigner.Object;
        _inSituService.TestAccessor().Dynamic._toolItemDesigner = _mockToolStripItemDesigner.Object;
        _inSituService.TestAccessor().Dynamic._componentChangeService = _mockComponentChangeService.Object;
    }

    public void Dispose() => _inSituService.Dispose();

    [Fact]
    public void Dispose_DisposesToolDesigner()
    {
        _inSituService.Dispose();

        object? toolDesignerValue = _inSituService.TestAccessor().Dynamic._toolDesigner;
        toolDesignerValue.Should().BeNull();
    }

    [Fact]
    public void Dispose_DisposesToolItemDesigner()
    {
        _inSituService.Dispose();

        object? toolItemDesignerValue = _inSituService.TestAccessor().Dynamic._toolItemDesigner;
        toolItemDesignerValue.Should().BeNull();
    }

    [Fact]
    public void Dispose_UnsubscribesFromComponentChangeService()
    {
        _inSituService.Dispose();

        object? componentChangeServiceValue = _inSituService.TestAccessor().Dynamic._componentChangeService;
        componentChangeServiceValue.Should().BeNull();
    }

    [Fact]
    public void ToolStripKeyBoardService_ReturnsServiceInstance()
    {
        object? toolStripKeyBoardService = _inSituService.TestAccessor().Dynamic.ToolStripKeyBoardService;

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
        Mock<IContainer> mockContainer = new();
        ComponentCollection componentCollection = new(Array.Empty<IComponent>());

        mockContainer.Setup(c => c.Components).Returns(componentCollection);
        _mockDesignerHost.Setup(dh => dh.Container).Returns(mockContainer.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISupportInSituService))).Returns(_inSituService);

        _inSituService.TestAccessor().Dynamic.OnComponentRemoved(null, new ComponentEventArgs(new Component()));

        _mockDesignerHost.Verify(dh => dh.RemoveService(typeof(ISupportInSituService)), Times.Once);
    }

    [Fact]
    public void OnComponentRemoved_DoesNotRemoveService_WhenToolStripPresent()
    {
        Mock<ToolStrip> mockToolStrip = new();
        Mock<IContainer> mockContainer = new();
        ComponentCollection realComponentCollection = new([mockToolStrip.Object]);

        mockContainer.Setup(c => c.Components).Returns(realComponentCollection);
        _mockDesignerHost.Setup(dh => dh.Container).Returns(mockContainer.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(ISupportInSituService))).Returns(_inSituService);

        _inSituService.TestAccessor().Dynamic.OnComponentRemoved(null, new ComponentEventArgs(new Component()));

        _mockDesignerHost.Verify(dh => dh.RemoveService(typeof(ISupportInSituService)), Times.Never);
    }
}
