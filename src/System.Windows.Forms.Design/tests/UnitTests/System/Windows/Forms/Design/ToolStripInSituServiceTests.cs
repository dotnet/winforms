﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
    private bool _isInSituServiceDisposed;

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

    public void Dispose()
    {
        if (!_isInSituServiceDisposed)
        {
            _inSituService.Dispose();
        }
    }

    [Fact]
    public void Dispose_DisposesToolDesigner()
    {
        _inSituService.Dispose();
        _isInSituServiceDisposed = true;

        object toolDesignerValue = _inSituService.TestAccessor().Dynamic._toolDesigner;
        toolDesignerValue.Should().BeNull();
    }

    [Fact]
    public void Dispose_DisposesToolItemDesigner()
    {
        _inSituService.Dispose();
        _isInSituServiceDisposed = true;

        object toolItemDesignerValue = _inSituService.TestAccessor().Dynamic._toolItemDesigner;
        toolItemDesignerValue.Should().BeNull();
    }

    [Fact]
    public void Dispose_UnsubscribesFromComponentChangeService()
    {
        _inSituService.Dispose();
        _isInSituServiceDisposed = true;

        object componentChangeServiceValue = _inSituService.TestAccessor().Dynamic._componentChangeService;
        componentChangeServiceValue.Should().BeNull();
    }

    [Fact]
    public void ToolStripKeyBoardService_ReturnsServiceInstance()
    {
        object toolStripKeyBoardService = _inSituService.TestAccessor().Dynamic.ToolStripKeyBoardService;

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

    [Fact]
    public void IgnoreMessages_WhenPrimarySelectionIsNotIComponentAndSelectedDesignerControlIsNull_ReturnsFalse()
    {
        _inSituService.TestAccessor().Dynamic._toolDesigner = _mockToolStripDesigner.Object;
        bool result = _inSituService.IgnoreMessages;
        result.Should().BeFalse();
    }

    [Fact]
    public void IgnoreMessages_WhenComponentIsMenuStrip_ReturnsTrue()
    {
        Mock<MenuStrip> menuStripMock = new();
        Mock<ToolStrip> toolStripMock = new();
        _mockSelectionService.Setup(ss => ss.PrimarySelection).Returns(menuStripMock.Object);
        _mockDesignerHost.Setup(dh => dh.GetDesigner(menuStripMock.Object)).Returns(_mockToolStripDesigner.Object);
        bool result = _inSituService.IgnoreMessages;
        result.Should().BeTrue();
    }

    [Fact]
    public void IgnoreMessages_WhenComponentIsToolStripMenuItem_ReturnsTrue()
    {
        Mock<ToolStripMenuItem> toolStripMenuItemMock = new();
        Mock<DesignerToolStripControlHost> designerToolStripControlHostMock = new(toolStripMenuItemMock.Object);
        _mockSelectionService.Setup(ss => ss.PrimarySelection).Returns(toolStripMenuItemMock.Object);
        _mockDesignerHost.Setup(dh => dh.GetDesigner(toolStripMenuItemMock.Object)).Returns(_mockToolStripItemDesigner.Object);
        bool result = _inSituService.IgnoreMessages;
        result.Should().BeTrue();
    }

    // TODO: Uncomment when internalsVisibleTo is working again
    /*
    [Fact]
    public void IgnoreMessages_WhenComponentIsToolStripDropDown_ReturnsTrue()
    {
        Mock<ToolStripDropDown> toolStripDropDownMock = new();
        var toolStripDropDownDesignerMock = new Mock<ToolStripDropDownDesigner>();
        _mockSelectionService.Setup(ss => ss.PrimarySelection).Returns(toolStripDropDownMock.Object);
        _mockDesignerHost.Setup(dh => dh.GetDesigner(toolStripDropDownMock.Object)).Returns(toolStripDropDownDesignerMock.Object);
        object toolItemDesignerValue = _inSituService.TestAccessor().Dynamic._toolItemDesigner;
        toolItemDesignerValue = _mockToolStripItemDesigner.Object;

toolStripDropDownDesignerMock.Setup(tdd => tdd.DesignerMenuItem).Returns(new ToolStripMenuItem());
        bool result = _inSituService.IgnoreMessages;
        result.Should().BeTrue();
    }

    private class SubToolStripDropDownDesigner : ToolStripDropDownDesigner
    {
        public SubToolStripDropDownDesigner(ToolStripDropDown dropDown) : base()
        {
        }

        public new ToolStripMenuItem DesignerMenuItem => new();
    }

    [Fact]
    public void IgnoreMessages_WhenPrimarySelectionIsComponentAndIsDesignerToolStripControlHost_ReturnsTrue()
    {
        Mock<ToolStripDropDown> toolStripDropDownMock = new();
        Mock<ToolStrip> toolStripMock = new();
        Mock<SubDesignerToolStripControlHost> designerToolStripControlHostMock = new(toolStripDropDownMock.Object);
        _mockSelectionService.Setup(ss => ss.PrimarySelection).Returns(designerToolStripControlHostMock.Object);
        _inSituService.TestAccessor().Dynamic._toolDesigner = _mockToolStripDesigner.Object;
        object toolStripKeyBoardService = _inSituService.TestAccessor().Dynamic.ToolStripKeyBoardService;
        bool result = _inSituService.IgnoreMessages;
        result.Should().BeTrue();
    }

    internal class SubDesignerToolStripControlHost : DesignerToolStripControlHost
    {
        private readonly Control _parent;

        public SubDesignerToolStripControlHost(Control c) : base(c)
        {
            _parent = c;
        }

        public new Control GetCurrentParent()
        {
            return _parent;
        }
    }
    */

    [Fact]
    public void HandleKeyChar_WhenToolItemDesignerIsNotMenuDesigner_CallsShowEditNode()
    {
        _inSituService.TestAccessor().Dynamic._toolDesigner = null;

        _inSituService.HandleKeyChar();

        _mockToolStripItemDesigner.Verify(d => d.ShowEditNode(false), Times.Once);
    }


    // TODO: Ditto
    /* 
    [Fact]
    public void HandleKeyChar_WhenSelectionIsNull_UsesSelectedDesignerControl()
    {
        Mock<ToolStripMenuItemDesigner> mockMenuDesigner = new();
        _inSituService.TestAccessor().Dynamic._toolDesigner = null;
        _inSituService.TestAccessor().Dynamic._toolItemDesigner = mockMenuDesigner.Object;

        using ToolStripItem toolstrip = new ToolStripMenuItem();
        using ToolStripDropDown dropDown = new();
        dropDown.CreateControl();
        DesignerToolStripControlHost selectedControl = new(dropDown);
        _mockToolStripDesigner.Object.Items.Add(selectedControl);
_mockToolStripDesigner.Object.Initialize(selectedControl);

        _inSituService.TestAccessor().Dynamic.ToolStripKeyBoardService.SelectedDesignerControl = selectedControl;

        _mockSelectionService.Setup(s => s.PrimarySelection).Returns(null);

        _inSituService.HandleKeyChar();

        mockMenuDesigner.Verify(d => d.EditTemplateNode(false), Times.Once);

        _inSituService.TestAccessor().Dynamic.ToolStripKeyBoardService.SelectedDesignerControl = null;
    }
    */
}
