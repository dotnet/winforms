// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ControlCommandSetTests : IDisposable
{
    private readonly Mock<ISite> _siteMock;
    private readonly Mock<IDesignerHost> _designerHostMock;
    private readonly Mock<IMenuCommandService> _menuServiceMock;
    private readonly Mock<IEventHandlerService> _eventHandlerServiceMock;
    private readonly Mock<ISelectionService> _selectionServiceMock;
    private readonly Mock<IDictionaryService> _dictionaryServiceMock;
    private readonly ControlCommandSet _controlCommandSet;
    private readonly Control _baseControl;

    public ControlCommandSetTests()
    {
        _siteMock = new();
        _designerHostMock = new();
        _menuServiceMock = new();
        _eventHandlerServiceMock = new();
        _selectionServiceMock = new();
        _dictionaryServiceMock = new();
        _baseControl = new();

        _siteMock.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(_designerHostMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(IMenuCommandService))).Returns(_menuServiceMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(IEventHandlerService))).Returns(_eventHandlerServiceMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(ISelectionService))).Returns(_selectionServiceMock.Object);
        _siteMock.Setup(s => s.GetService(typeof(IDictionaryService))).Returns(_dictionaryServiceMock.Object);
        _designerHostMock.Setup(h => h.RootComponent).Returns(_baseControl);

        _controlCommandSet = new(_siteMock.Object);
    }

    public void Dispose()
    {
        _controlCommandSet.Dispose();
        _baseControl.Dispose();
    }

    [Fact]
    public void Constructor_ShouldInitializeCommandSet()
    {
        _controlCommandSet.Should().NotBeNull();

        StatusCommandUI statusCommandUI = _controlCommandSet.TestAccessor().Dynamic._statusCommandUI;
        statusCommandUI.Should().NotBeNull();

        MenuCommand[] commandSet = _controlCommandSet.TestAccessor().Dynamic._commandSet;
        commandSet.Should().NotBeNull();
        commandSet.Should().NotBeEmpty();

        TabOrder tabOrder = _controlCommandSet.TestAccessor().Dynamic._tabOrder;
        tabOrder.Should().BeNull();
    }

    [Fact]
    public void Dispose_ShouldRemoveCommandsFromMenuService()
    {
        _controlCommandSet.Dispose();

        _menuServiceMock.Verify(m => m.RemoveCommand(It.IsAny<MenuCommand>()), Times.AtLeastOnce);
    }

    [Fact]
    public void GetSnapInformation_ShouldRetrieveSnapInformation()
    {
        using Control component = new();
        Mock<IContainer> containerMock = new();
        _siteMock.Setup(s => s.Container).Returns(containerMock.Object);
        component.Site = _siteMock.Object;

        _designerHostMock.Setup(h => h.RootComponent).Returns(_baseControl);

        Mock<IDesigner> designerMock = new();
        designerMock.Setup(d => d.Component).Returns(component);
        _designerHostMock.Setup(h => h.GetDesigner(component)).Returns(designerMock.Object);

        _controlCommandSet.TestAccessor().Dynamic.GetSnapInformation(
            _designerHostMock.Object, component, out Size snapSize, out IComponent snapComponent, out PropertyDescriptor snapProperty);

        snapComponent.Should().Be(_baseControl);
        snapProperty.Should().BeNull();
        snapSize.Should().Be(Size.Empty);
    }

    [Fact]
    public void OnMenuLockControls_ShouldToggleLockControls()
    {
        using Control component = new();
        Mock<IContainer> containerMock = new();
        _siteMock.Setup(s => s.Container).Returns(containerMock.Object);
        component.Site = _siteMock.Object;

        _designerHostMock.Setup(h => h.RootComponent).Returns(_baseControl);
        _designerHostMock.Setup(h => h.Container.Components).Returns(new ComponentCollection([component]));

        CommandID commandID = new(Guid.NewGuid(), 0);
        MenuCommand menuCommand = new(null, commandID) { Checked = false };

        _controlCommandSet.TestAccessor().Dynamic.OnMenuLockControls(menuCommand, EventArgs.Empty);

        menuCommand.Checked.Should().BeTrue();
    }
}
