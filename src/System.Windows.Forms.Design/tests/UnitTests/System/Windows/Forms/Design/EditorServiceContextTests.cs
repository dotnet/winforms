// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class EditorServiceContextTests : IDisposable
{
    private readonly TestComponent _component;
    private readonly TestComponentDesigner _designer;
    private readonly Mock<PropertyDescriptor> _mockPropertyDescriptor;
    private readonly Mock<UITypeEditor> _mockEditor;
    private readonly Mock<IContainer> _mockContainer;
    private readonly Mock<ISite> _mockSite;
    private readonly Mock<IComponentChangeService> _mockChangeService;
    private readonly Mock<IUIService> _mockUIService;
    private readonly Mock<Form> _mockDialog;

    public EditorServiceContextTests()
    {
        _component = new();
        _designer = new(_component);
        _mockPropertyDescriptor = new("Items", Array.Empty<Attribute>());
        _mockEditor = new();
        _mockContainer = new();
        _mockSite = new();
        _mockChangeService = new();
        _mockUIService = new();
        _mockDialog = new();
    }

    public void Dispose()
    {
        _designer.Dispose();
        _component.Dispose();
        _mockDialog.Object.Dispose();
    }

    [Fact]
    public void EditValue_ShouldUpdatePropertyValue_WhenEditorReturnsNewValue()
    {
        List<string> oldValue = new() { "Old" };
        List<string> newValue = new() { "New" };

        _mockPropertyDescriptor.Setup(p => p.GetValue(_component)).Returns(oldValue);
        _mockPropertyDescriptor.Setup(p => p.SetValue(_component, newValue));
        _mockPropertyDescriptor.Setup(p => p.PropertyType).Returns(typeof(List<string>));
        _mockPropertyDescriptor.Setup(p => p.Name).Returns("Items");
        _mockPropertyDescriptor.Setup(p => p.Attributes).Returns(new AttributeCollection(null));

        _mockEditor
            .Setup(e => e.EditValue(It.IsAny<ITypeDescriptorContext>(), It.IsAny<IServiceProvider>(), oldValue))
            .Returns(newValue);

        _mockPropertyDescriptor
            .Setup(p => p.GetEditor(typeof(UITypeEditor)))
            .Returns(_mockEditor.Object);

        TypeDescriptor.AddProvider(new TypeDescriptionProviderMock(_mockPropertyDescriptor.Object), _component);

        object? result = EditorServiceContext.EditValue(_designer, _component, "Items");

        result.Should().BeEquivalentTo(newValue);
        _mockPropertyDescriptor.Verify(p => p.SetValue(_component, newValue), Times.Once);
    }

    [Fact]
    public void EditValue_ShouldNotUpdatePropertyValue_WhenEditorReturnsSameValue()
    {
        List<string> list = new() { "Same" };
        _component.Items = list;

        _mockPropertyDescriptor.Setup(p => p.GetValue(_component)).Returns(list);
        _mockPropertyDescriptor.Setup(p => p.PropertyType).Returns(typeof(List<string>));
        _mockPropertyDescriptor.Setup(p => p.Name).Returns("Items");
        _mockPropertyDescriptor.Setup(p => p.Attributes).Returns(new AttributeCollection(null));

        _mockEditor
            .Setup(e => e.EditValue(It.IsAny<ITypeDescriptorContext>(), It.IsAny<IServiceProvider>(), list))
            .Returns(list);

        _mockPropertyDescriptor
            .Setup(p => p.GetEditor(typeof(UITypeEditor)))
            .Returns(_mockEditor.Object);

        TypeDescriptor.AddProvider(new TypeDescriptionProviderMock(_mockPropertyDescriptor.Object), _component);

        object? result = EditorServiceContext.EditValue(_designer, _component, "Items");

        result.Should().BeSameAs(list);
        _mockPropertyDescriptor.Verify(p => p.SetValue(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public void Container_ShouldReturnNull_WhenComponentSiteIsNull()
    {
        EditorServiceContext context = new(_designer, _mockPropertyDescriptor.Object);

        using IContainer? container = ((ITypeDescriptorContext)context).Container;

        container.Should().BeNull();
    }

    [Fact]
    public void Container_ShouldReturnContainer_WhenComponentSiteHasContainer()
    {
        _mockSite.Setup(s => s.Container).Returns(_mockContainer.Object);
        _component.Site = _mockSite.Object;

        EditorServiceContext context = new(_designer, _mockPropertyDescriptor.Object);

        using IContainer? container = ((ITypeDescriptorContext)context).Container;

        container.Should().BeSameAs(_mockContainer.Object);
    }

    [Fact]
    public void OnComponentChanging_ShouldReturnTrue_WhenChangeServiceDoesNotThrow()
    {
        _mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_mockChangeService.Object);
        _component.Site = _mockSite.Object;

        EditorServiceContext context = new(_designer, _mockPropertyDescriptor.Object);

        bool result = ((ITypeDescriptorContext)context).OnComponentChanging();

        result.Should().BeTrue();
        _mockChangeService.Verify(cs => cs.OnComponentChanging(_component, _mockPropertyDescriptor.Object), Times.Once);
    }

    [Fact]
    public void OnComponentChanging_ShouldReturnFalse_WhenChangeServiceThrowsCheckoutExceptionCanceled()
    {
        _mockChangeService
            .Setup(cs => cs.OnComponentChanging(It.IsAny<object>(), It.IsAny<PropertyDescriptor>()))
            .Throws(CheckoutException.Canceled);

        _mockSite.Setup(s => s.GetService(typeof(IComponentChangeService))).Returns(_mockChangeService.Object);
        _component.Site = _mockSite.Object;

        EditorServiceContext context = new(_designer, _mockPropertyDescriptor.Object);

        bool result = ((ITypeDescriptorContext)context).OnComponentChanging();

        result.Should().BeFalse();
        _mockChangeService.Verify(cs => cs.OnComponentChanging(_component, _mockPropertyDescriptor.Object), Times.Once);
    }

    [Fact]
    public void ShowDialog_ShouldUseIUIService_WhenAvailable()
    {
        _mockSite.Setup(s => s.GetService(typeof(IUIService))).Returns(_mockUIService.Object);
        _component.Site = _mockSite.Object;

        EditorServiceContext context = new(_designer, _mockPropertyDescriptor.Object);

        _mockUIService.Setup(s => s.ShowDialog(_mockDialog.Object)).Returns(DialogResult.OK);

        DialogResult result = ((IWindowsFormsEditorService)context).ShowDialog(_mockDialog.Object);

        result.Should().Be(DialogResult.OK);
        _mockUIService.Verify(s => s.ShowDialog(_mockDialog.Object), Times.Once);
    }

    [Fact]
    public void ShowDialog_ShouldFallbackToFormShowDialog_WhenIUIServiceNotAvailable()
    {
        _mockSite.Setup(s => s.GetService(typeof(IUIService))).Returns((object?)null);
        _component.Site = _mockSite.Object;

        EditorServiceContext context = new(_designer, _mockPropertyDescriptor.Object);

        Mock<IWindowsFormsEditorService> mockEditorService = new();
        mockEditorService.Setup(es => es.ShowDialog(It.IsAny<Form>())).Returns(DialogResult.Cancel);

        DialogResult result = mockEditorService.Object.ShowDialog(new Form());

        result.Should().Be(DialogResult.Cancel);
    }

    private class TestComponent : Component
    {
        public List<string> Items { get; set; } = new();
    }

    private class TestComponentDesigner : ComponentDesigner
    {
        public TestComponentDesigner(IComponent component) => Initialize(component);
    }

    private class TypeDescriptionProviderMock : TypeDescriptionProvider
    {
        private readonly PropertyDescriptor _property;

        public TypeDescriptionProviderMock(PropertyDescriptor property) => _property = property;

        public override ICustomTypeDescriptor? GetTypeDescriptor(Type objectType, object? instance) => new TypeDescriptorStub(_property);

        private class TypeDescriptorStub : CustomTypeDescriptor
        {
            private readonly PropertyDescriptor _property;

            public TypeDescriptorStub(PropertyDescriptor property) => _property = property;

            public override PropertyDescriptorCollection GetProperties() => new PropertyDescriptorCollection([_property]);

            public override PropertyDescriptorCollection GetProperties(Attribute[]? attributes) => GetProperties();
        }
    }
}
