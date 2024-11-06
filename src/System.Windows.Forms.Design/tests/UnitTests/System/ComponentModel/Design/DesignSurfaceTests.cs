// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows.Forms.Design;
using Moq;

namespace System.ComponentModel.Design.Tests;

public class DesignSurfaceTests
{
    [WinFormsFact]
    public void DesignSurface_Ctor_Default()
    {
        using SubDesignSurface surface = new();
        Assert.NotNull(surface.ComponentContainer);
        Assert.Empty(surface.ComponentContainer.Components);
        Assert.False(surface.DtelLoading);
        Assert.False(surface.Host.CanReloadWithErrors);
        Assert.Same(surface.Host, surface.Host.Container);
        Assert.False(surface.Host.Loading);
        Assert.False(surface.Host.IgnoreErrorsDuringReload);
        Assert.False(surface.Host.InTransaction);
        Assert.False(((IDesignerHostTransactionState)surface.Host).IsClosingTransaction);
        Assert.Null(surface.Host.RootComponent);
        Assert.Null(surface.Host.RootComponentClassName);
        surface.Host.TransactionDescription.Should().BeNullOrEmpty();
        Assert.False(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.NotNull(surface.ServiceContainer);
        Assert.Throws<InvalidOperationException>(() => surface.View);
    }

    public static IEnumerable<object[]> Ctor_IServiceProvider_TestData()
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ContainerFilterService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IHelpService)))
            .Returns(null);
        yield return new object[] { null };
        yield return new object[] { mockServiceProvider.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_IServiceProvider_TestData))]
    public void DesignSurface_Ctor_IServiceProvider(IServiceProvider parentProvider)
    {
        using SubDesignSurface surface = new(parentProvider);
        Assert.NotNull(surface.ComponentContainer);
        Assert.Empty(surface.ComponentContainer.Components);
        Assert.False(surface.DtelLoading);
        Assert.False(surface.Host.CanReloadWithErrors);
        Assert.Same(surface.Host, surface.Host.Container);
        Assert.False(surface.Host.Loading);
        Assert.False(surface.Host.IgnoreErrorsDuringReload);
        Assert.False(surface.Host.InTransaction);
        Assert.False(((IDesignerHostTransactionState)surface.Host).IsClosingTransaction);
        Assert.Null(surface.Host.RootComponent);
        Assert.Null(surface.Host.RootComponentClassName);
        surface.Host.TransactionDescription.Should().BeNullOrEmpty();
        Assert.False(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.NotNull(surface.ServiceContainer);
        Assert.Throws<InvalidOperationException>(() => surface.View);
    }

    [WinFormsFact]
    public void DesignSurface_Ctor_Type()
    {
        using SubDesignSurface surface = new(typeof(RootDesignerComponent));
        Assert.NotNull(surface.ComponentContainer);
        Assert.Single(surface.ComponentContainer.Components);
        Assert.False(surface.DtelLoading);
        Assert.False(surface.Host.CanReloadWithErrors);
        Assert.Same(surface.Host, surface.Host.Container);
        Assert.False(surface.Host.Loading);
        Assert.False(surface.Host.IgnoreErrorsDuringReload);
        Assert.False(surface.Host.InTransaction);
        Assert.False(((IDesignerHostTransactionState)surface.Host).IsClosingTransaction);
        Assert.IsType<RootDesignerComponent>(surface.Host.RootComponent);
        Assert.Equal(typeof(RootDesignerComponent).FullName, surface.Host.RootComponentClassName);
        surface.Host.TransactionDescription.Should().BeNullOrEmpty();
        Assert.True(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.NotNull(surface.ServiceContainer);
        Assert.Same(RootComponentDesigner.View, surface.View);
    }

    [WinFormsFact]
    public void DesignSurface_Ctor_IServiceProvider_Type_NullParentProvider()
    {
        using SubDesignSurface surface = new(null, typeof(RootDesignerComponent));
        Assert.NotNull(surface.ComponentContainer);
        Assert.Single(surface.ComponentContainer.Components);
        Assert.False(surface.DtelLoading);
        Assert.False(surface.Host.CanReloadWithErrors);
        Assert.Same(surface.Host, surface.Host.Container);
        Assert.False(surface.Host.Loading);
        Assert.False(surface.Host.IgnoreErrorsDuringReload);
        Assert.False(surface.Host.InTransaction);
        Assert.False(((IDesignerHostTransactionState)surface.Host).IsClosingTransaction);
        Assert.IsType<RootDesignerComponent>(surface.Host.RootComponent);
        Assert.Equal(typeof(RootDesignerComponent).FullName, surface.Host.RootComponentClassName);
        surface.Host.TransactionDescription.Should().BeNullOrEmpty();
        Assert.True(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.NotNull(surface.ServiceContainer);
        Assert.Same(RootComponentDesigner.View, surface.View);
    }

    [WinFormsFact]
    public void DesignSurface_Ctor_IServiceProvider_Type_CustomParentProvider()
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(TypeDescriptionProvider)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(DesignerCommandSet)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IInheritanceService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IHelpService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ContainerFilterService)))
            .Returns(null);
        using SubDesignSurface surface = new(mockServiceProvider.Object, typeof(RootDesignerComponent));
        Assert.NotNull(surface.ComponentContainer);
        Assert.Single(surface.ComponentContainer.Components);
        Assert.False(surface.DtelLoading);
        Assert.False(surface.Host.CanReloadWithErrors);
        Assert.Same(surface.Host, surface.Host.Container);
        Assert.False(surface.Host.Loading);
        Assert.False(surface.Host.IgnoreErrorsDuringReload);
        Assert.False(surface.Host.InTransaction);
        Assert.False(((IDesignerHostTransactionState)surface.Host).IsClosingTransaction);
        Assert.IsType<RootDesignerComponent>(surface.Host.RootComponent);
        Assert.Equal(typeof(RootDesignerComponent).FullName, surface.Host.RootComponentClassName);
        surface.Host.TransactionDescription.Should().BeNullOrEmpty();
        Assert.True(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.NotNull(surface.ServiceContainer);
        Assert.Same(RootComponentDesigner.View, surface.View);
    }

    [WinFormsFact]
    public void DesignSurface_Ctor_NullRootComponentType_ThrowsArgumentNullException()
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        Assert.Throws<ArgumentNullException>("rootComponentType", () => new DesignSurface((Type)null));
        Assert.Throws<ArgumentNullException>("rootComponentType", () => new DesignSurface(mockServiceProvider.Object, null));
    }

    [WinFormsFact]
    public void DesignSurface_ComponentContainer_GetDisposed_ThrowsObjectDisposedException()
    {
        using DesignSurface surface = new();
        surface.Dispose();
        Assert.Throws<ObjectDisposedException>(() => surface.ComponentContainer);
    }

    [WinFormsTheory]
    [BoolData]
    public void DesignSurface_DtelLoading_Set_GetReturnsExpected(bool value)
    {
        using DesignSurface surface = new()
        {
            DtelLoading = value
        };
        Assert.Equal(value, surface.DtelLoading);

        // Set same
        surface.DtelLoading = value;
        Assert.Equal(value, surface.DtelLoading);

        // Set different
        surface.DtelLoading = !value;
        Assert.Equal(!value, surface.DtelLoading);
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_Get_ReturnsSame()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.NotNull(container);
        Assert.Same(container, surface.ServiceContainer);
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetISelectionService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        ISelectionService service = Assert.IsAssignableFrom<ISelectionService>(container.GetService(typeof(ISelectionService)));
        Assert.Null(service.PrimarySelection);
        Assert.Equal(0, service.SelectionCount);
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetIExtenderProviderService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.IsAssignableFrom<IExtenderProviderService>(container.GetService(typeof(IExtenderProviderService)));
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetIExtenderListService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.IsAssignableFrom<IExtenderListService>(container.GetService(typeof(IExtenderListService)));
        Assert.IsAssignableFrom<IExtenderProviderService>(container.GetService(typeof(IExtenderListService)));
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetITypeDescriptorFilterService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.IsAssignableFrom<ITypeDescriptorFilterService>(container.GetService(typeof(ITypeDescriptorFilterService)));
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetIReferenceService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.IsAssignableFrom<IReferenceService>(container.GetService(typeof(IReferenceService)));
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetDesignSurfaceService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Same(surface, container.GetService(typeof(DesignSurface)));
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetInstanceTypeService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Same(container, container.GetService(container.GetType()));
    }

    public static IEnumerable<object[]> ServiceContainer_FixedService_TestData()
    {
        yield return new object[] { typeof(IDesignerHost) };
        yield return new object[] { typeof(IContainer) };
        yield return new object[] { typeof(IComponentChangeService) };
        yield return new object[] { typeof(IDesignerLoaderHost2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(ServiceContainer_FixedService_TestData))]
    public void DesignSurface_ServiceContainer_GetFixedService_ReturnsExpected(Type serviceType)
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Same(surface.Host, container.GetService(serviceType));
    }

    [WinFormsTheory]
    [MemberData(nameof(ServiceContainer_FixedService_TestData))]
    public void DesignSurface_ServiceContainer_RemoveFixedService_ThrowsInvalidOperationException(Type serviceType)
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Throws<InvalidOperationException>(() => container.RemoveService(serviceType));
    }

    [WinFormsTheory]
    [InlineData(typeof(ISelectionService))]
    [InlineData(typeof(IExtenderProviderService))]
    [InlineData(typeof(IExtenderListService))]
    [InlineData(typeof(ITypeDescriptorFilterService))]
    [InlineData(typeof(IReferenceService))]
    [InlineData(typeof(DesignSurface))]
    public void DesignSurface_ServiceContainer_RemoveNonFixedServiceType_ThrowsArgumentNullException(Type serviceType)
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.NotNull(container.GetService(serviceType));
        container.RemoveService(serviceType);
        Assert.Null(container.GetService(serviceType));

        // Remove again.
        container.RemoveService(serviceType);
        Assert.Null(container.GetService(serviceType));
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_RemoveNullServiceType_ThrowsArgumentNullException()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Throws<ArgumentNullException>("serviceType", () => container.RemoveService(null));
    }

    [WinFormsFact]
    public void DesignSurface_ServiceContainer_GetDisposed_ThrowsObjectDisposedException()
    {
        using SubDesignSurface surface = new();
        surface.Dispose();
        Assert.Throws<ObjectDisposedException>(() => surface.ServiceContainer);
    }

    [WinFormsTheory]
    [InlineData(typeof(NullSupportedTechnologiesRootDesignerComponent))]
    [InlineData(typeof(EmptySupportedTechnologiesRootDesignerComponent))]
    public void DesignSurface_View_GetWithInvalidSupportedTechnologies_ThrowsNotSupportedException(Type rootComponentType)
    {
        using SubDesignSurface surface = new();
        surface.BeginLoad(rootComponentType);
        Assert.Throws<NotSupportedException>(() => surface.View);
    }

    [WinFormsFact]
    public void DesignSurface_View_GetDisposed_ThrowsObjectDisposedException()
    {
        using DesignSurface surface = new();
        surface.Dispose();
        Assert.Throws<ObjectDisposedException>(() => surface.View);
    }

    [WinFormsFact]
    public void DesignSurface_View_GetWithExceptionLoadErrors_ThrowsInvalidOperationException()
    {
        InvalidOperationException exception = new();
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Throws(exception);
        surface.BeginLoad(mockLoader.Object);
        Assert.Same(exception, Assert.Throws<InvalidOperationException>(() => surface.View).InnerException);
    }

    public static IEnumerable<object[]> View_GetLoadError_TestData()
    {
        yield return new object[] { Array.Empty<object>() };
        yield return new object[] { new object[] { new InvalidOperationException() } };
        yield return new object[] { new object[] { "Error" } };
        yield return new object[] { new object[] { null } };
    }

    [WinFormsTheory]
    [MemberData(nameof(View_GetLoadError_TestData))]
    public void DesignSurface_View_GetWithLoadErrors_ThrowsInvalidOperationException(object[] errorCollection)
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        host.EndLoad("BaseClassName", false, errorCollection);
        Assert.Throws<InvalidOperationException>(() => surface.View);
    }

    public static IEnumerable<object[]> BeginLoad_TestData()
    {
        yield return new object[] { null };

        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(null);
        yield return new object[] { nullMockServiceProvider.Object };

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(new object());
        yield return new object[] { invalidMockServiceProvider.Object };

        Mock<IDesignerEventService> mockDesignerEventService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(mockDesignerEventService.Object);
        yield return new object[] { mockServiceProvider.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(BeginLoad_TestData))]
    public void DesignSurface_BeginLoad_Invoke_Success(IServiceProvider parentProvider)
    {
        SubDesignSurface surface = new(parentProvider);
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Verifiable();
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.True(surface.Host.Loading);
        Assert.True(host.Loading);
        Assert.Null(host.RootComponent);
        Assert.Null(host.RootComponentClassName);
        Assert.Throws<InvalidOperationException>(() => surface.View);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_ThrowsException_SetsLoadErrors()
    {
        InvalidOperationException exception = new();
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Throws(exception);
        mockLoader
            .Setup(l => l.Loading)
            .Returns(false);
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Assert.False(surface.Host.Loading);
        Assert.Same(exception, Assert.Single(surface.LoadErrors));
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void DesignSurface_BeginLoad_ThrowsExceptionWithoutMessage_SetsLoadErrors(string message)
    {
        Mock<Exception> mockException = new(MockBehavior.Strict);
        mockException
            .Setup(e => e.Message)
            .Returns(message);
        mockException
            .Setup(e => e.ToString())
            .Returns("ExceptionText");
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Throws(mockException.Object);
        mockLoader
            .Setup(l => l.Loading)
            .Returns(false);
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Exception error = Assert.IsType<InvalidOperationException>(Assert.Single(surface.LoadErrors));
        Assert.Contains("ExceptionText", error.Message);
        Assert.False(surface.Host.Loading);
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_ThrowsTargetInvocationException_SetsLoadErrors()
    {
        InvalidOperationException exception = new();
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Throws(new TargetInvocationException(exception));
        mockLoader
            .Setup(l => l.Loading)
            .Returns(false);
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Assert.Same(exception, Assert.Single(surface.LoadErrors));
        Assert.False(surface.Host.Loading);
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_InvokeWithLoading_CallsHandler()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;

        int loadingCallCount = 0;
        surface.Loading += (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Same(EventArgs.Empty, e);
            loadingCallCount++;
        };
        int loadedCallCount = 0;
        surface.Loaded += (sender, e) => loadedCallCount++;
        int unloadingCallCount = 0;
        surface.Unloading += (sender, e) => unloadingCallCount++;
        int unloadedCallCount = 0;
        surface.Unloaded += (sender, e) => unloadedCallCount++;
        int flushedCallCount = 0;
        surface.Flushed += (sender, e) => flushedCallCount++;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Verifiable();
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.True(surface.Host.Loading);
        Assert.Equal(1, loadingCallCount);
        Assert.Equal(0, loadedCallCount);
        Assert.Equal(0, unloadingCallCount);
        Assert.Equal(0, unloadedCallCount);
        Assert.Equal(0, flushedCallCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());

        // Reload.
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.True(surface.Host.Loading);
        Assert.Equal(2, loadingCallCount);
        Assert.Equal(0, loadedCallCount);
        Assert.Equal(0, unloadingCallCount);
        Assert.Equal(0, unloadedCallCount);
        Assert.Equal(0, flushedCallCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Exactly(2));
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_InvokeErrorWithUnloading_CallsHandler()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;

        int unloadingCallCount = 0;
        surface.Unloading += (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Same(EventArgs.Empty, e);
            unloadingCallCount++;
        };
        int unloadedCallCount = 0;
        surface.Unloaded += (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Same(EventArgs.Empty, e);
            Assert.True(unloadedCallCount < unloadingCallCount);
            unloadedCallCount++;
        };
        int loadedCallCount = 0;
        surface.Loaded += (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.False(e.HasSucceeded);
            Assert.Same(surface.LoadErrors, e.Errors);
            Assert.True(loadedCallCount < unloadingCallCount);
            loadedCallCount++;
        };
        int flushedCallCount = 0;
        surface.Flushed += (sender, e) => flushedCallCount++;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Throws(new InvalidOperationException())
            .Verifiable();
        mockLoader
            .Setup(l => l.Loading)
            .Returns(false);
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Assert.Single(surface.LoadErrors);
        Assert.False(surface.Host.Loading);
        Assert.Equal(1, unloadingCallCount);
        Assert.Equal(1, unloadedCallCount);
        Assert.Equal(1, loadedCallCount);
        Assert.Equal(0, flushedCallCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());

        // Reload.
        surface.BeginLoad(mockLoader.Object);
        Assert.False(surface.IsLoaded);
        Assert.False(surface.Host.Loading);
        Assert.Single(surface.LoadErrors);
        Assert.Equal(2, unloadingCallCount);
        Assert.Equal(2, unloadedCallCount);
        Assert.Equal(2, loadedCallCount);
        Assert.Equal(0, flushedCallCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Exactly(2));
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_InvokeDefaultIExtenderProvider_Success()
    {
        Mock<IExtenderProviderService> mockExtenderProviderService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(null)
            .Verifiable();
        SubDesignSurface surface = new(mockServiceProvider.Object);
        IExtenderListService defaultProviderService = (IExtenderListService)surface.GetService(typeof(IExtenderListService));
        IDesignerLoaderHost2 host = surface.Host;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Verifiable();
        var mockExtenderProvider = mockLoader.As<IExtenderProvider>();
        surface.BeginLoad(mockLoader.Object);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());
        Assert.Same(mockExtenderProvider.Object, Assert.Single(defaultProviderService.GetExtenderProviders()));

        // Reload.
        surface.BeginLoad(mockLoader.Object);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());
        Assert.Same(mockExtenderProvider.Object, Assert.Single(defaultProviderService.GetExtenderProviders()));
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_InvokeValidIExtenderProvider_CallsAddExtenderProvider()
    {
        Mock<IExtenderProviderService> mockExtenderProviderService = new(MockBehavior.Strict);
        mockExtenderProviderService
            .Setup(s => s.AddExtenderProvider(It.IsAny<IExtenderProvider>()))
            .Verifiable();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(null)
            .Verifiable();
        SubDesignSurface surface = new(mockServiceProvider.Object);
        IExtenderListService defaultProviderService = (IExtenderListService)surface.GetService(typeof(IExtenderListService));
        surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
        IDesignerLoaderHost2 host = surface.Host;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Verifiable();
        var mockExtenderProvider = mockLoader.As<IExtenderProvider>();
        surface.BeginLoad(mockLoader.Object);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(mockExtenderProvider.Object), Times.Once());
        Assert.Empty(defaultProviderService.GetExtenderProviders());

        // Reload.
        surface.BeginLoad(mockLoader.Object);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(mockExtenderProvider.Object), Times.Once());
        Assert.Empty(defaultProviderService.GetExtenderProviders());
    }

    public static IEnumerable<object[]> BeginLoad_InvalidIExtenderProvider_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [WinFormsTheory]
    [MemberData(nameof(BeginLoad_InvalidIExtenderProvider_TestData))]
    public void DesignSurface_BeginLoad_InvokeInvalidIExtenderProvider_Success(object service)
    {
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(service)
            .Verifiable();
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(null)
            .Verifiable();
        SubDesignSurface surface = new(mockServiceProvider.Object);
        IExtenderListService defaultProviderService = (IExtenderListService)surface.GetService(typeof(IExtenderListService));
        surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
        IDesignerLoaderHost2 host = surface.Host;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Verifiable();
        var mockExtenderProvider = mockLoader.As<IExtenderProvider>();
        surface.BeginLoad(mockLoader.Object);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());
        Assert.Empty(defaultProviderService.GetExtenderProviders());

        // Reload.
        surface.BeginLoad(mockLoader.Object);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());
        Assert.Empty(defaultProviderService.GetExtenderProviders());
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_InvokeWithoutIDesignerEventServiceWithActivated_CallsHandler()
    {
        SubDesignSurface surface = new();
        int callCount = 0;
        IDesignerLoaderHost2 host = surface.Host;
        host.Activated += (sender, e) =>
        {
            Assert.Same(host, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Verifiable();
        surface.BeginLoad(mockLoader.Object);
        Assert.Equal(1, callCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());

        // Reload.
        surface.BeginLoad(mockLoader.Object);
        Assert.Equal(2, callCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Exactly(2));
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_InvokeWithIDesignerEventServiceWithActivated_DoesNotCallHandler()
    {
        Mock<IDesignerEventService> mockDesignerEventService = new(MockBehavior.Strict);
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IDesignerEventService)))
            .Returns(mockDesignerEventService.Object)
            .Verifiable();
        SubDesignSurface surface = new(mockServiceProvider.Object);
        IDesignerLoaderHost2 host = surface.Host;
        int callCount = 0;
        host.Activated += (sender, e) =>
        {
            Assert.Same(host, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Verifiable();
        surface.BeginLoad(mockLoader.Object);
        Assert.Equal(0, callCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());

        // Reload.
        surface.BeginLoad(mockLoader.Object);
        Assert.Equal(0, callCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Exactly(2));
        mockServiceProvider.Verify(p => p.GetService(typeof(IDesignerEventService)), Times.Once());
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_DisposeInLoading_DoesCallFlush()
    {
        using SubDesignSurface surface = new();
        int loadingCallCount = 0;
        surface.Loading += (sender, e) =>
        {
            // Catch the InvalidOperationException thrown.
            try
            {
                surface.Dispose();
            }
            catch
            {
            }

            Assert.Same(surface, sender);
            Assert.Same(EventArgs.Empty, e);
            loadingCallCount++;
        };
        int loadedCallCount = 0;
        surface.Loaded += (sender, e) => loadedCallCount++;
        int unloadingCallCount = 0;
        surface.Unloading += (sender, e) => unloadingCallCount++;
        int unloadedCallCount = 0;
        surface.Unloaded += (sender, e) => unloadedCallCount++;
        int flushedCallCount = 0;
        surface.Flushed += (sender, e) => flushedCallCount++;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        surface.BeginLoad(mockLoader.Object);
        Assert.Equal(1, loadingCallCount);
        Assert.Equal(0, loadedCallCount);
        Assert.Equal(0, unloadingCallCount);
        Assert.Equal(0, unloadedCallCount);
        Assert.Equal(0, flushedCallCount);
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_DisposeInBeginLoad_DoesCallFlush()
    {
        using SubDesignSurface surface = new();
        int loadingCallCount = 0;
        surface.Loading += (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Same(EventArgs.Empty, e);
            loadingCallCount++;
        };
        int loadedCallCount = 0;
        surface.Loaded += (sender, e) => loadedCallCount++;
        int unloadingCallCount = 0;
        surface.Unloading += (sender, e) => unloadingCallCount++;
        int unloadedCallCount = 0;
        surface.Unloaded += (sender, e) => unloadedCallCount++;
        int flushedCallCount = 0;
        surface.Flushed += (sender, e) => flushedCallCount++;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        IDesignerLoaderHost2 host = surface.Host;
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Callback(() =>
            {
                // Catch the InvalidOperationException thrown.
                try
                {
                    surface.Dispose();
                }
                catch
                {
                }
            });
        surface.BeginLoad(mockLoader.Object);
        Assert.Equal(1, loadingCallCount);
        Assert.Equal(0, loadedCallCount);
        Assert.Equal(0, unloadingCallCount);
        Assert.Equal(0, unloadedCallCount);
        Assert.Equal(0, flushedCallCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_DisposeInBeginLoadThrowsException_DoesCallFlush()
    {
        using SubDesignSurface surface = new();
        int loadingCallCount = 0;
        surface.Loading += (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Same(EventArgs.Empty, e);
            loadingCallCount++;
        };
        int loadedCallCount = 0;
        surface.Loaded += (sender, e) => loadedCallCount++;
        int unloadingCallCount = 0;
        surface.Unloading += (sender, e) => unloadingCallCount++;
        int unloadedCallCount = 0;
        surface.Unloaded += (sender, e) => unloadedCallCount++;
        int flushedCallCount = 0;
        surface.Flushed += (sender, e) => flushedCallCount++;

        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        IDesignerLoaderHost2 host = surface.Host;
        mockLoader
            .Setup(l => l.BeginLoad(host))
            .Callback(() =>
            {
                // Catch the InvalidOperationException thrown.
                try
                {
                    surface.Dispose();
                }
                catch
                {
                }

                throw new InvalidOperationException();
            });
        surface.BeginLoad(mockLoader.Object);
        Assert.Equal(1, loadingCallCount);
        Assert.Equal(0, loadedCallCount);
        Assert.Equal(0, unloadingCallCount);
        Assert.Equal(0, unloadedCallCount);
        Assert.Equal(0, flushedCallCount);
        mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_NullLoader_ThrowsArgumentNullException()
    {
        using DesignSurface surface = new();
        Assert.Throws<ArgumentNullException>("loader", () => surface.BeginLoad((DesignerLoader)null));
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_InvokeType_Success()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        surface.BeginLoad(typeof(RootDesignerComponent));
        Assert.True(surface.IsLoaded);
        Assert.Empty(surface.LoadErrors);
        Assert.False(surface.Host.Loading);
        Assert.False(host.Loading);
        Assert.IsType<RootDesignerComponent>(host.RootComponent);
        Assert.Equal(typeof(RootDesignerComponent).FullName, host.RootComponentClassName);
        Assert.Same(RootComponentDesigner.View, surface.View);
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_NullRootComponentType_ThrowsArgumentNullException()
    {
        using DesignSurface surface = new();
        Assert.Throws<ArgumentNullException>("rootComponentType", () => surface.BeginLoad((Type)null));
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_AlreadyCalled_ThrowsInvalidOperationException()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        Mock<DesignerLoader> otherMockLoader = new(MockBehavior.Strict);
        otherMockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        Assert.Throws<InvalidOperationException>(() => surface.BeginLoad(typeof(RootDesignerComponent)));
        Assert.Throws<InvalidOperationException>(() => surface.BeginLoad(otherMockLoader.Object));
    }

    [WinFormsFact]
    public void DesignSurface_BeginLoad_Disposed_ThrowsObjectDisposedException()
    {
        using DesignSurface surface = new();
        surface.Dispose();
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        Assert.Throws<ObjectDisposedException>(() => surface.BeginLoad(typeof(RootDesignerComponent)));
        Assert.Throws<ObjectDisposedException>(() => surface.BeginLoad(mockLoader.Object));
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void DesignSurface_CreateDesigner_InvokeNoDesigner_ReturnsExpected(bool rootDesigner)
    {
        using SubDesignSurface surface = new();
        using Component component = new();

        Assert.NotNull(surface.CreateDesigner(component, rootDesigner));
    }

    [WinFormsFact]
    public void DesignSurface_CreateDesigner_InvokeIDesigner_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        using DesignerComponent component = new();

        Assert.IsType<ComponentDocumentDesigner>(surface.CreateDesigner(component, rootDesigner: true));
        Assert.IsType<ComponentDesigner>(surface.CreateDesigner(component, rootDesigner: false));
    }

    [WinFormsFact]
    public void DesignSurface_CreateDesigner_InvokeIRootDesigner_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        using RootDesignerComponent component = new();
        Assert.IsType<RootComponentDesigner>(surface.CreateDesigner(component, rootDesigner: true));
        Assert.IsType<ComponentDesigner>(surface.CreateDesigner(component, rootDesigner: false));
    }

    [WinFormsFact]
    public void DesignSurface_CreateDesigner_NullComponent_ThrowsArgumentNullException()
    {
        using SubDesignSurface surface = new();
        Assert.Throws<ArgumentNullException>("component", () => surface.CreateDesigner(null, true));
        Assert.Throws<ArgumentNullException>("component", () => surface.CreateDesigner(null, false));
    }

    [WinFormsFact]
    public void DesignSurface_CreateDesigner_Disposed_ThrowsObjectDisposedException()
    {
        using SubDesignSurface surface = new();
        surface.Dispose();
        Assert.Throws<ObjectDisposedException>(() => surface.CreateDesigner(new Component(), true));
        Assert.Throws<ObjectDisposedException>(() => surface.CreateDesigner(new Component(), false));
    }

    [WinFormsFact]
    public void DesignSurface_CreateComponent_IComponentWithPublicDefaultConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ComponentWithPublicConstructor instance = Assert.IsType<ComponentWithPublicConstructor>(surface.CreateComponent(typeof(ComponentWithPublicConstructor)));
        Assert.Null(instance.Container);
    }

    [WinFormsFact]
    public void DesignSurface_CreateComponent_IComponentWithPrivateDefaultConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ComponentWithPrivateDefaultConstructor instance = Assert.IsType<ComponentWithPrivateDefaultConstructor>(surface.CreateComponent(typeof(ComponentWithPrivateDefaultConstructor)));
        Assert.Null(instance.Container);
    }

    [WinFormsFact]
    public void DesignSurface_CreateComponent_IComponentWithIContainerConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ComponentWithIContainerConstructor instance = Assert.IsType<ComponentWithIContainerConstructor>(surface.CreateComponent(typeof(ComponentWithIContainerConstructor)));
        Assert.Same(surface.ComponentContainer, instance.Container);
    }

    [WinFormsTheory]
    [InlineData(typeof(ClassWithPublicConstructor))]
    [InlineData(typeof(ClassWithPrivateDefaultConstructor))]
    public void DesignSurface_CreateComponent_NonIComponent_ReturnsNull(Type type)
    {
        using SubDesignSurface surface = new();
        Assert.Null(surface.CreateComponent(type));
    }

    [WinFormsTheory]
    [InlineData(typeof(ClassWithIContainerConstructor))]
    [InlineData(typeof(ClassWithNoMatchingConstructor))]
    [InlineData(typeof(ComponentWithPrivateIContainerConstructor))]
    [InlineData(typeof(ComponentWithNoMatchingConstructor))]
    public void DesignSurface_CreateComponent_TypeWithNoMatchingConstructor_ThrowsMissingMethodException(Type type)
    {
        using SubDesignSurface surface = new();
        Assert.Throws<MissingMethodException>(() => surface.CreateComponent(type));
    }

    [WinFormsFact]
    public void DesignSurface_CreateComponent_NullType_ThrowsArgumentNullException()
    {
        using SubDesignSurface surface = new();
        Assert.Throws<ArgumentNullException>("type", () => surface.CreateComponent(null));
    }

    [WinFormsFact]
    public void DesignSurface_CreateInstance_NonIComponentWithPublicDefaultConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        Assert.IsType<ClassWithPublicConstructor>(surface.CreateInstance(typeof(ClassWithPublicConstructor)));
    }

    [WinFormsFact]
    public void DesignSurface_CreateInstance_NonIComponentWithPrivateDefaultConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        Assert.IsType<ClassWithPrivateDefaultConstructor>(surface.CreateInstance(typeof(ClassWithPrivateDefaultConstructor)));
    }

    [WinFormsFact]
    public void DesignSurface_CreateInstance_IComponentWithPublicDefaultConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ComponentWithPublicConstructor instance = Assert.IsType<ComponentWithPublicConstructor>(surface.CreateInstance(typeof(ComponentWithPublicConstructor)));
        Assert.Null(instance.Container);
    }

    [WinFormsFact]
    public void DesignSurface_CreateInstance_IComponentWithPrivateDefaultConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ComponentWithPrivateDefaultConstructor instance = Assert.IsType<ComponentWithPrivateDefaultConstructor>(surface.CreateInstance(typeof(ComponentWithPrivateDefaultConstructor)));
        Assert.Null(instance.Container);
    }

    [WinFormsFact]
    public void DesignSurface_CreateInstance_IComponentWithIContainerConstructor_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ComponentWithIContainerConstructor instance = Assert.IsType<ComponentWithIContainerConstructor>(surface.CreateInstance(typeof(ComponentWithIContainerConstructor)));
        Assert.Same(surface.ComponentContainer, instance.Container);
    }

    [WinFormsTheory]
    [InlineData(typeof(ClassWithIContainerConstructor))]
    [InlineData(typeof(ClassWithNoMatchingConstructor))]
    [InlineData(typeof(ComponentWithPrivateIContainerConstructor))]
    [InlineData(typeof(ComponentWithNoMatchingConstructor))]
    public void DesignSurface_CreateInstance_TypeWithNoMatchingConstructor_ThrowsMissingMethodException(Type type)
    {
        using SubDesignSurface surface = new();
        Assert.Throws<MissingMethodException>(() => surface.CreateInstance(type));
    }

    [WinFormsFact]
    public void DesignSurface_CreateInstance_NullType_ThrowsArgumentNullException()
    {
        using SubDesignSurface surface = new();
        Assert.Throws<ArgumentNullException>("type", () => surface.CreateInstance(null));
    }

    [WinFormsFact]
    public void DesignSurface_CreateNestedContainer_InvokeObject_ReturnsExpected()
    {
        using DesignSurface surface = new();
        using Component ownerComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(ownerComponent);
        Assert.Empty(container.Components);
        Assert.Same(ownerComponent, container.Owner);
    }

    [WinFormsTheory]
    [StringData]
    public void DesignSurface_CreateNestedContainer_InvokeObjectString_ReturnsExpected(string containerName)
    {
        using DesignSurface surface = new();
        using Component ownerComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(ownerComponent, containerName);
        Assert.Empty(container.Components);
        Assert.Same(ownerComponent, container.Owner);
    }

    [WinFormsFact]
    public void DesignSurface_CreateNestedContainer_NullOwningComponent_ThrowsArgumentNullException()
    {
        using DesignSurface surface = new();
        Assert.Throws<ArgumentNullException>("owningComponent", () => surface.CreateNestedContainer(null));
        Assert.Throws<ArgumentNullException>("owningComponent", () => surface.CreateNestedContainer(null, "name"));
    }

    [WinFormsFact]
    public void DesignSurface_CreateNestedContainer_Disposed_ThrowsObjectDisposedException()
    {
        using DesignSurface surface = new();
        surface.Dispose();
        Assert.Throws<ObjectDisposedException>(() => surface.CreateNestedContainer(null, "name"));
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeMultipleTimes_Success()
    {
        using DesignSurface surface = new();
        surface.Dispose();
        surface.Dispose();
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_Invoke_RemovesDesignSurfaceService()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Same(surface, container.GetService(typeof(DesignSurface)));

        surface.Dispose();
        Assert.Null(container.GetService(typeof(DesignSurface)));
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeHasLoader_ThrowsInvalidOperationException()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        Assert.Throws<InvalidOperationException>(surface.Dispose);
        Assert.True(host.Loading);

        // Should not throw again.
        surface.Dispose();
        Assert.True(host.Loading);
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeHasHostWithTransactions_ThrowsInvalidOperationException()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        DesignerTransaction transaction = host.CreateTransaction("Transaction1");
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);
        Assert.Throws<InvalidOperationException>(surface.Dispose);
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);

        // Should not throw again.
        surface.Dispose();
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);
    }

    [WinFormsFact]
    public void Dispose_InvokeWithDisposed_CallsHandler()
    {
        using DesignSurface surface = new();
        surface.Dispose();

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        surface.Disposed += handler;

        // Call with handler.
        surface.Dispose();
        Assert.Equal(1, callCount);

        // Call again.
        surface.Dispose();
        Assert.Equal(2, callCount);

        // Remove handler.
        surface.Disposed -= handler;
        surface.Dispose();
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void DesignSurface_Dispose_InvokeDisposingMultipleTimes_Success(bool disposing)
    {
        using SubDesignSurface surface = new();
        surface.Dispose(disposing);
        surface.Dispose(disposing);
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeDisposing_RemovesDesignSurfaceService()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Same(surface, container.GetService(typeof(DesignSurface)));

        surface.Dispose(true);
        Assert.Null(container.GetService(typeof(DesignSurface)));
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeNotDisposing_DoesNotRemoveDesignSurfaceService()
    {
        using SubDesignSurface surface = new();
        ServiceContainer container = surface.ServiceContainer;
        Assert.Same(surface, container.GetService(typeof(DesignSurface)));

        surface.Dispose(false);
        Assert.Same(surface, container.GetService(typeof(DesignSurface)));
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeDisposingHasLoader_ThrowsInvalidOperationException()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        Assert.Throws<InvalidOperationException>(() => surface.Dispose(true));
        Assert.True(host.Loading);

        // Should not throw again.
        surface.Dispose(true);
        Assert.True(host.Loading);
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeNotDisposingHasLoader_Nop()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        surface.Dispose(false);
        Assert.True(host.Loading);

        // Should not throw again.
        surface.Dispose(false);
        Assert.True(host.Loading);
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeDisposingHasHostWithTransactions_ThrowsInvalidOperationException()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        DesignerTransaction transaction = host.CreateTransaction("Transaction1");
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);
        Assert.Throws<InvalidOperationException>(() => surface.Dispose(true));
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);

        // Should not throw again.
        surface.Dispose(true);
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);
    }

    [WinFormsFact]
    public void DesignSurface_Dispose_InvokeNotDisposingHasHostWithTransactions_Nop()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        DesignerTransaction transaction = host.CreateTransaction("Transaction1");
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        surface.BeginLoad(mockLoader.Object);
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);
        surface.Dispose(false);
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);

        // Should not throw again.
        surface.Dispose(false);
        Assert.True(host.Loading);
        Assert.True(host.InTransaction);
        Assert.Equal("Transaction1", host.TransactionDescription);
    }

    [WinFormsTheory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Dispose_InvokeDisposingWithDisposed_CallsHandler(bool disposing, int expectedCallCount)
    {
        using SubDesignSurface surface = new();
        surface.Dispose();

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        surface.Disposed += handler;

        // Call with handler.
        surface.Dispose(disposing);
        Assert.Equal(expectedCallCount, callCount);

        // Call again.
        surface.Dispose(disposing);
        Assert.Equal(expectedCallCount * 2, callCount);

        // Remove handler.
        surface.Disposed -= handler;
        surface.Dispose(disposing);
        Assert.Equal(expectedCallCount * 2, callCount);
    }

    [WinFormsFact]
    public void DesignSurface_Flush_InvokeWithHostWithLoader_CallsLoaderFlush()
    {
        SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        Mock<DesignerLoader> mockLoader = new(MockBehavior.Strict);
        mockLoader
            .Setup(l => l.BeginLoad(host));
        mockLoader
            .Setup(l => l.Flush())
            .Verifiable();
        surface.BeginLoad(mockLoader.Object);

        surface.Flush();
        mockLoader.Verify(l => l.Flush(), Times.Once());

        // Flush again.
        surface.Flush();
        mockLoader.Verify(l => l.Flush(), Times.Exactly(2));
    }

    [WinFormsFact]
    public void DesignSurface_Flush_InvokeWithHostWithoutLoader_Nop()
    {
        using DesignSurface surface = new();
        surface.Flush();

        // Flush again.
        surface.Flush();
    }

    [WinFormsFact]
    public void DesignSurface_Flush_InvokeDisposed_Nop()
    {
        using DesignSurface surface = new();
        surface.Dispose();
        surface.Flush();

        // Flush again.
        surface.Flush();
    }

    [WinFormsFact]
    public void DesignSurface_Flush_InvokeWithFlushed_CallsHandler()
    {
        using DesignSurface surface = new();
        int callCount = 0;
        surface.Flushed += (sender, e) =>
        {
            Assert.Same(surface, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };

        surface.Flush();
        Assert.Equal(1, callCount);

        // Flush again.
        surface.Flush();
        Assert.Equal(2, callCount);

        // Flush when disposed.
        surface.Dispose();
        surface.Flush();
        Assert.Equal(3, callCount);
    }

    [WinFormsFact]
    public void DesignSurface_GetService_InvokeWithServiceProvider_ReturnsExpected()
    {
        object service = new();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(int)))
            .Returns(service)
            .Verifiable();
        using DesignSurface surface = new(mockServiceProvider.Object);
        Assert.Same(service, surface.GetService(typeof(int)));
        mockServiceProvider.Verify(p => p.GetService(typeof(int)), Times.Once());
    }

    [WinFormsFact]
    public void DesignSurface_GetService_GetISelectionService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        ISelectionService service = Assert.IsAssignableFrom<ISelectionService>(surface.GetService(typeof(ISelectionService)));
        Assert.Null(service.PrimarySelection);
        Assert.Equal(0, service.SelectionCount);
    }

    [WinFormsFact]
    public void DesignSurface_GetService_GetIExtenderProviderService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        Assert.IsAssignableFrom<IExtenderProviderService>(surface.GetService(typeof(IExtenderProviderService)));
    }

    [WinFormsFact]
    public void DesignSurface_GetService_GetIExtenderListService_ReturnsExpected()
    {
        using DesignSurface surface = new();
        Assert.IsAssignableFrom<IExtenderListService>(surface.GetService(typeof(IExtenderListService)));
        Assert.IsAssignableFrom<IExtenderProviderService>(surface.GetService(typeof(IExtenderListService)));
    }

    [WinFormsFact]
    public void DesignSurface_GetService_GetITypeDescriptorFilterService_ReturnsExpected()
    {
        using DesignSurface surface = new();
        Assert.IsAssignableFrom<ITypeDescriptorFilterService>(surface.GetService(typeof(ITypeDescriptorFilterService)));
    }

    [WinFormsFact]
    public void DesignSurface_GetService_GetIReferenceService_ReturnsExpected()
    {
        using DesignSurface surface = new();
        Assert.IsAssignableFrom<IReferenceService>(surface.GetService(typeof(IReferenceService)));
    }

    [WinFormsFact]
    public void DesignSurface_GetService_GetDesignSurfaceService_ReturnsExpected()
    {
        using DesignSurface surface = new();
        Assert.Same(surface, surface.GetService(typeof(DesignSurface)));
    }

    [WinFormsFact]
    public void DesignSurface_GetService_GetInstanceTypeService_ReturnsExpected()
    {
        using SubDesignSurface surface = new();
        Assert.Same(surface.ServiceContainer, surface.GetService(surface.ServiceContainer.GetType()));
    }

    [WinFormsTheory]
    [InlineData(typeof(IServiceContainer))]
    [InlineData(typeof(ServiceContainer))]
    public void DesignSurface_GetService_IServiceContainer_ReturnsExpected(Type serviceType)
    {
        using SubDesignSurface surface = new();
        Assert.Same(surface.ServiceContainer, surface.GetService(serviceType));
    }

    [WinFormsTheory]
    [MemberData(nameof(ServiceContainer_FixedService_TestData))]
    public void DesignSurface_GetService_GetFixedService_ReturnsExpected(Type serviceType)
    {
        using SubDesignSurface surface = new();
        Assert.Same(surface.Host, surface.GetService(serviceType));
    }

    [WinFormsFact]
    public void DesignSurface_GetService_InvokeWithoutServiceProvider_ReturnsNull()
    {
        using DesignSurface surface = new();
        Assert.Null(surface.GetService(typeof(int)));
    }

    [WinFormsTheory]
    [MemberData(nameof(ServiceContainer_FixedService_TestData))]
    public void DesignSurface_GetService_Disposed_ReturnsNull(Type serviceType)
    {
        using SubDesignSurface surface = new();
        surface.Dispose();
        Assert.Null(surface.GetService(serviceType));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void DesignSurface_OnLoading_Invoke_Success(EventArgs eventArgs)
    {
        using SubDesignSurface surface = new();

        // No handler.
        surface.OnLoading(eventArgs);

        // Handler.
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(surface, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        surface.Loading += handler;
        surface.OnLoading(eventArgs);
        Assert.Equal(1, callCount);

        // Should not call if the handler is removed.
        surface.Loading -= handler;
        surface.OnLoading(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> LoadedEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new LoadedEventArgs(false, null) };
    }

    [WinFormsTheory]
    [MemberData(nameof(LoadedEventArgs_TestData))]
    public void DesignSurface_OnLoaded_Invoke_Success(LoadedEventArgs eventArgs)
    {
        using SubDesignSurface surface = new();

        // No handler.
        surface.OnLoaded(eventArgs);

        // Handler.
        int callCount = 0;
        LoadedEventHandler handler = (sender, e) =>
        {
            Assert.Equal(surface, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        surface.Loaded += handler;
        surface.OnLoaded(eventArgs);
        Assert.Equal(1, callCount);

        // Should not call if the handler is removed.
        surface.Loaded -= handler;
        surface.OnLoaded(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void DesignSurface_OnUnloaded_Invoke_Success(EventArgs eventArgs)
    {
        using SubDesignSurface surface = new();

        // No handler.
        surface.OnUnloaded(eventArgs);

        // Handler.
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(surface, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        surface.Unloaded += handler;
        surface.OnUnloaded(eventArgs);
        Assert.Equal(1, callCount);

        // Should not call if the handler is removed.
        surface.Unloaded -= handler;
        surface.OnUnloaded(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void DesignSurface_OnUnloading_Invoke_Success(EventArgs eventArgs)
    {
        using SubDesignSurface surface = new();

        // No handler.
        surface.OnUnloading(eventArgs);

        // Handler.
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(surface, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        surface.Unloading += handler;
        surface.OnUnloading(eventArgs);
        Assert.Equal(1, callCount);

        // Should not call if the handler is removed.
        surface.Unloading -= handler;
        surface.OnUnloading(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void DesignSurface_OnViewActivate_Invoke_Success(EventArgs eventArgs)
    {
        using SubDesignSurface surface = new();

        // No handler.
        surface.OnViewActivate(eventArgs);

        // Handler.
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(surface, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        surface.ViewActivated += handler;
        surface.OnViewActivate(eventArgs);
        Assert.Equal(1, callCount);

        // Should not call if the handler is removed.
        surface.ViewActivated -= handler;
        surface.OnViewActivate(eventArgs);
        Assert.Equal(1, callCount);
    }

    private class SubDesignSurface : DesignSurface
    {
        public SubDesignSurface() : base()
        {
        }

        public SubDesignSurface(IServiceProvider parentProvider) : base(parentProvider)
        {
        }

        public SubDesignSurface(Type rootComponentType) : base(rootComponentType)
        {
        }

        public SubDesignSurface(IServiceProvider parentProvider, Type rootComponentType) : base(parentProvider, rootComponentType)
        {
        }

        public new ServiceContainer ServiceContainer => base.ServiceContainer;

        public IDesignerLoaderHost2 Host => Assert.IsAssignableFrom<IDesignerLoaderHost2>(ComponentContainer);

#pragma warning disable 0618
        public new IComponent CreateComponent(Type componentType) => base.CreateComponent(componentType);
#pragma warning restore 0618

        public new IDesigner CreateDesigner(IComponent component, bool rootDesigner) => base.CreateDesigner(component, rootDesigner);

        public new object CreateInstance(Type type) => base.CreateInstance(type);

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new void OnLoading(EventArgs e) => base.OnLoading(e);

        public new void OnLoaded(LoadedEventArgs e) => base.OnLoaded(e);

        public new void OnUnloading(EventArgs e) => base.OnUnloading(e);

        public new void OnUnloaded(EventArgs e) => base.OnUnloaded(e);

        public new void OnViewActivate(EventArgs e) => base.OnViewActivate(e);
    }

    [Designer(typeof(ComponentDesigner))]
    private class DesignerComponent : Component
    {
    }

    private class RootComponentDesigner : ComponentDesigner, IRootDesigner
    {
        public static object View { get; } = new();

        public ViewTechnology[] SupportedTechnologies => [ViewTechnology.Default + 1];
        public object GetView(ViewTechnology technology)
        {
            Assert.Equal(ViewTechnology.Default + 1, technology);
            return View;
        }
    }

    [Designer(typeof(RootComponentDesigner), typeof(IRootDesigner))]
    private class RootDesignerComponent : Component
    {
    }

    private class NullSupportedTechnologiesRootComponentDesigner : ComponentDesigner, IRootDesigner
    {
        public ViewTechnology[] SupportedTechnologies => null;
        public object GetView(ViewTechnology technology) => throw new NotImplementedException();
    }

    [Designer(typeof(NullSupportedTechnologiesRootComponentDesigner), typeof(IRootDesigner))]
    private class NullSupportedTechnologiesRootDesignerComponent : Component
    {
    }

    private class EmptySupportedTechnologiesRootComponentDesigner : ComponentDesigner, IRootDesigner
    {
        public ViewTechnology[] SupportedTechnologies => Array.Empty<ViewTechnology>();
        public object GetView(ViewTechnology technology) => throw new NotImplementedException();
    }

    [Designer(typeof(EmptySupportedTechnologiesRootComponentDesigner), typeof(IRootDesigner))]
    private class EmptySupportedTechnologiesRootDesignerComponent : Component
    {
    }

    private class ClassWithPublicConstructor
    {
        public ClassWithPublicConstructor()
        {
        }

        public ClassWithPublicConstructor(IContainer container)
        {
            throw new NotImplementedException();
        }
    }

    private class ComponentWithPublicConstructor : Component
    {
        public ComponentWithPublicConstructor()
        {
        }

        public ComponentWithPublicConstructor(IContainer container)
        {
            throw new NotImplementedException();
        }
    }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0051 // Remove unused private members

    private class ClassWithPrivateDefaultConstructor
    {
        private ClassWithPrivateDefaultConstructor()
        {
        }

        private ClassWithPrivateDefaultConstructor(IContainer container)
        {
            throw new NotImplementedException();
        }
    }

    private class ComponentWithPrivateDefaultConstructor : Component
    {
        private ComponentWithPrivateDefaultConstructor()
        {
        }

        private ComponentWithPrivateDefaultConstructor(IContainer container)
        {
            throw new NotImplementedException();
        }
    }

    private class ComponentWithIContainerConstructor : Component
    {
        public new IContainer Container { get; set; }

        public ComponentWithIContainerConstructor(IContainer container)
        {
            Container = container;
        }
    }

    private class ClassWithIContainerConstructor
    {
        public IContainer Container { get; set; }

        public ClassWithIContainerConstructor(IContainer container)
        {
            Container = container;
        }
    }

    private class ClassWithNoMatchingConstructor
    {
        private ClassWithNoMatchingConstructor(bool value)
        {
        }
    }

    private class ComponentWithNoMatchingConstructor : Component
    {
        private ComponentWithNoMatchingConstructor(bool value)
        {
        }
    }

    private class ClassWithPrivateIContainerConstructor
    {
        private ClassWithPrivateIContainerConstructor(IContainer container)
        {
        }
    }

    private class ComponentWithPrivateIContainerConstructor : Component
    {
        private ComponentWithPrivateIContainerConstructor(IContainer container)
        {
        }
    }
#pragma warning restore IDE0051
#pragma warning restore IDE0060
}
