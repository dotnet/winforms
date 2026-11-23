// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Moq;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests;

public class Com2PropertyPageUITypeEditorTests
{
    private static readonly Guid s_pageGuid = new("2D6A3B3A-2C6B-4B0B-9E36-0C1A5E7A9D11");

    // Scenario: The property page editor opens as a modal dialog editor.
    [StaFact]
    public void GetEditStyle_ReturnsModal()
    {
        FixturePropertyDescriptor descriptor = new(new object());
        Com2PropertyPageUITypeEditor editor = new(descriptor, s_pageGuid, baseEditor: null);

        Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context: null));
    }

    // Scenario: Editing a value asks the registered property page service to show the page for the property's
    // target object, with the property name, DISPID and page GUID, and returns the value unchanged.
    [StaFact]
    public void EditValue_ServiceProvided_ShowsPageForTargetAndReturnsValueUnchanged()
    {
        object target = new();
        FixturePropertyDescriptor descriptor = new(target, dispid: 7, name: "Caption");
        Com2PropertyPageUITypeEditor editor = new(descriptor, s_pageGuid, baseEditor: null);

        Mock<ICom2PropertyPageDisplayService> service = new(MockBehavior.Strict);
        service.Setup(s => s.ShowPropertyPage("Caption", target, 7, s_pageGuid, It.IsAny<IntPtr>()));

        object value = new();
        object? result = editor.EditValue(new TestContext(instance: new object()), new TestServiceProvider(service.Object), value);

        Assert.Same(value, result);
        service.Verify(s => s.ShowPropertyPage("Caption", target, 7, s_pageGuid, It.IsAny<IntPtr>()), Times.Once);
    }

    // Scenario: When the property owner is described by an ICustomTypeDescriptor, the page is shown for the owner it
    // reports rather than the wrapper.
    [StaFact]
    public void EditValue_TargetIsCustomTypeDescriptor_ShowsPageForPropertyOwner()
    {
        object owner = new();
        FixturePropertyDescriptor descriptor = new(target: null, dispid: 3, name: "Font");
        Mock<ICustomTypeDescriptor> wrapper = new();
        wrapper.Setup(w => w.GetPropertyOwner(descriptor)).Returns(owner);
        descriptor.Target = wrapper.Object;

        Com2PropertyPageUITypeEditor editor = new(descriptor, s_pageGuid, baseEditor: null);

        Mock<ICom2PropertyPageDisplayService> service = new(MockBehavior.Strict);
        service.Setup(s => s.ShowPropertyPage("Font", owner, 3, s_pageGuid, It.IsAny<IntPtr>()));

        editor.EditValue(new TestContext(instance: new object()), new TestServiceProvider(service.Object), "value");

        service.Verify(s => s.ShowPropertyPage("Font", owner, 3, s_pageGuid, It.IsAny<IntPtr>()), Times.Once);
    }

    // Scenario: When the edited instance is an array (multi-selection), the array is passed to the service as is.
    [StaFact]
    public void EditValue_ContextInstanceIsArray_PassesArrayToService()
    {
        FixturePropertyDescriptor descriptor = new(new object());
        Com2PropertyPageUITypeEditor editor = new(descriptor, s_pageGuid, baseEditor: null);

        object[] selection = [new object(), new object()];
        Mock<ICom2PropertyPageDisplayService> service = new(MockBehavior.Strict);
        service.Setup(s => s.ShowPropertyPage("Value", selection, 42, s_pageGuid, It.IsAny<IntPtr>()));

        editor.EditValue(new TestContext(selection), new TestServiceProvider(service.Object), "value");

        service.Verify(s => s.ShowPropertyPage("Value", selection, 42, s_pageGuid, It.IsAny<IntPtr>()), Times.Once);
    }

    // Scenario: A failure while showing the property page is reported through the UI service and the original value
    // is returned.
    [StaFact]
    public void EditValue_ServiceThrows_ReportsErrorToUIServiceAndReturnsValue()
    {
        FixturePropertyDescriptor descriptor = new(new object());
        Com2PropertyPageUITypeEditor editor = new(descriptor, s_pageGuid, baseEditor: null);

        InvalidOperationException failure = new("page failed");
        Mock<ICom2PropertyPageDisplayService> service = new();
        service
            .Setup(s => s.ShowPropertyPage(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<IntPtr>()))
            .Throws(failure);

        Mock<IUIService> uiService = new(MockBehavior.Strict);
        uiService.Setup(u => u.ShowError(failure, It.IsAny<string>()));

        object value = new();
        object? result = editor.EditValue(
            new TestContext(instance: new object()),
            new TestServiceProvider(service.Object, uiService.Object),
            value);

        Assert.Same(value, result);
        uiService.Verify(u => u.ShowError(failure, It.IsAny<string>()), Times.Once);
    }

    // Scenario: A failure while showing the property page without a UI service is swallowed and the original value
    // is returned.
    [StaFact]
    public void EditValue_ServiceThrowsWithoutUIService_ReturnsValue()
    {
        FixturePropertyDescriptor descriptor = new(new object());
        Com2PropertyPageUITypeEditor editor = new(descriptor, s_pageGuid, baseEditor: null);

        Mock<ICom2PropertyPageDisplayService> service = new();
        service
            .Setup(s => s.ShowPropertyPage(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<IntPtr>()))
            .Throws<InvalidOperationException>();

        object value = new();
        Assert.Same(
            value,
            editor.EditValue(new TestContext(instance: new object()), new TestServiceProvider(service.Object), value));
    }

    private sealed class TestServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = [];

        public TestServiceProvider(ICom2PropertyPageDisplayService? pageService, IUIService? uiService = null)
        {
            if (pageService is not null)
            {
                _services[typeof(ICom2PropertyPageDisplayService)] = pageService;
            }

            if (uiService is not null)
            {
                _services[typeof(IUIService)] = uiService;
            }
        }

        public object? GetService(Type serviceType) => _services.GetValueOrDefault(serviceType);
    }

    private sealed class TestContext : ITypeDescriptorContext
    {
        public TestContext(object? instance) => Instance = instance;

        public IContainer? Container => null;

        public object? Instance { get; }

        public PropertyDescriptor? PropertyDescriptor => null;

        public object? GetService(Type serviceType) => null;

        public void OnComponentChanged()
        {
        }

        public bool OnComponentChanging() => true;
    }
}
