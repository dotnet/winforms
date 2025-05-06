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

    [Fact]
    public void GetSnapInformation_WithSnapToGridPropertyInParent_ShouldReturnThatProperty()
    {
        using Control childComponent = new();
        using Control parentComponent = new();

        childComponent.Parent = parentComponent;

        Mock<IContainer> containerMock = new();
        _siteMock.Setup(s => s.Container).Returns(containerMock.Object);
        childComponent.Site = _siteMock.Object;
        parentComponent.Site = _siteMock.Object;

        _designerHostMock.Setup(h => h.RootComponent).Returns(_baseControl);

        Mock<IDesigner> designerMock = new();
        designerMock.Setup(d => d.Component).Returns(childComponent);
        _designerHostMock.Setup(h => h.GetDesigner(childComponent)).Returns(designerMock.Object);

        Mock<PropertyDescriptor> snapPropertyDescriptorMock = new("SnapToGrid", Array.Empty<Attribute>());
        snapPropertyDescriptorMock.Setup(p => p.PropertyType).Returns(typeof(bool));
        snapPropertyDescriptorMock.Setup(p => p.ComponentType).Returns(typeof(Control));
        snapPropertyDescriptorMock.Setup(p => p.Name).Returns("SnapToGrid");

        var originalProvider = TypeDescriptor.GetProvider(parentComponent);
        var mockProvider = new MockTypeDescriptionProvider(originalProvider, snapPropertyDescriptorMock.Object);
        TypeDescriptor.AddProvider(mockProvider, parentComponent);

        try
        {
            _controlCommandSet.TestAccessor().Dynamic.GetSnapInformation(
                _designerHostMock.Object,
                childComponent,
                out Size snapSize,
                out IComponent snapComponent,
                out PropertyDescriptor snapProperty);

            snapComponent.Should().Be(parentComponent);
            snapProperty.Should().NotBeNull();
            snapProperty.Name.Should().Be("SnapToGrid");
            snapSize.Should().Be(Size.Empty);
        }
        finally
        {
            TypeDescriptor.RemoveProvider(mockProvider, parentComponent);
        }
    }

    /// <summary>
    ///  Helper class to provide mock property descriptors.
    /// </summary>
    private class MockTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider _baseProvider;
        private readonly PropertyDescriptor _snapToGridProperty;

        public MockTypeDescriptionProvider(TypeDescriptionProvider baseProvider, PropertyDescriptor snapToGridProperty)
        {
            _baseProvider = baseProvider;
            _snapToGridProperty = snapToGridProperty;
        }

        public override ICustomTypeDescriptor? GetTypeDescriptor(Type objectType, object? instance)
        {
            var baseDescriptor = _baseProvider.GetTypeDescriptor(objectType, instance);
            return baseDescriptor is not null ? new MockCustomTypeDescriptor(baseDescriptor, _snapToGridProperty) : null;
        }
    }

    private class MockCustomTypeDescriptor : CustomTypeDescriptor
    {
        private readonly PropertyDescriptor _snapToGridProperty;

        public MockCustomTypeDescriptor(ICustomTypeDescriptor parent, PropertyDescriptor snapToGridProperty)
            : base(parent)
        {
            _snapToGridProperty = snapToGridProperty;
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection baseProperties = base.GetProperties();

            PropertyDescriptor[] allProperties = new PropertyDescriptor[baseProperties.Count + 1];
            baseProperties.CopyTo(allProperties, 0);
            allProperties[baseProperties.Count] = _snapToGridProperty;

            return new PropertyDescriptorCollection(allProperties);
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
        {
            PropertyDescriptorCollection baseProperties = base.GetProperties(attributes);

            PropertyDescriptor[] allProperties = new PropertyDescriptor[baseProperties.Count + 1];
            baseProperties.CopyTo(allProperties, 0);
            allProperties[baseProperties.Count] = _snapToGridProperty;

            return new PropertyDescriptorCollection(allProperties);
        }
    }
}
