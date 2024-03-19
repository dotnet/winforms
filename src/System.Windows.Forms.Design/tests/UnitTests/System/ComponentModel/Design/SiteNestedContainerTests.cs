// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using Moq;

namespace System.ComponentModel.Design.Tests;

public class SiteNestedContainerTests
{
    public static IEnumerable<object[]> CreateNestedContainer_TestData()
    {
        static ISite CreateSite(string name)
        {
            Mock<ISite> mockSite = new(MockBehavior.Strict);
            mockSite
                .Setup(s => s.Name)
                .Returns(name);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            return mockSite.Object;
        }

        foreach (ISite site in new ISite[] { null, CreateSite(null) })
        {
            yield return new object[] { site, null, null, null };
            yield return new object[] { site, null, string.Empty, string.Empty };
            yield return new object[] { site, null, "componentName", "componentName" };
            yield return new object[] { site, string.Empty, null, null };
            yield return new object[] { site, string.Empty, string.Empty, string.Empty };
            yield return new object[] { site, string.Empty, "componentName", "componentName" };
            yield return new object[] { site, "containerName", null, null };
            yield return new object[] { site, "containerName", string.Empty, ".containerName." };
            yield return new object[] { site, "containerName", "componentName", ".containerName.componentName" };
        }

        yield return new object[] { CreateSite(string.Empty), null, null, null };
        yield return new object[] { CreateSite(string.Empty), null, string.Empty, "." };
        yield return new object[] { CreateSite(string.Empty), null, "componentName", ".componentName" };
        yield return new object[] { CreateSite(string.Empty), string.Empty, null, null };
        yield return new object[] { CreateSite(string.Empty), string.Empty, string.Empty, "." };
        yield return new object[] { CreateSite(string.Empty), string.Empty, "componentName", ".componentName" };
        yield return new object[] { CreateSite(string.Empty), "containerName", null, null };
        yield return new object[] { CreateSite(string.Empty), "containerName", string.Empty, ".containerName." };
        yield return new object[] { CreateSite(string.Empty), "containerName", "componentName", ".containerName.componentName" };

        yield return new object[] { CreateSite("ownerName"), null, null, null };
        yield return new object[] { CreateSite("ownerName"), null, string.Empty, "ownerName." };
        yield return new object[] { CreateSite("ownerName"), null, "componentName", "ownerName.componentName" };
        yield return new object[] { CreateSite("ownerName"), string.Empty, null, null };
        yield return new object[] { CreateSite("ownerName"), string.Empty, string.Empty, "ownerName." };
        yield return new object[] { CreateSite("ownerName"), string.Empty, "componentName", "ownerName.componentName" };
        yield return new object[] { CreateSite("ownerName"), "containerName", null, null };
        yield return new object[] { CreateSite("ownerName"), "containerName", string.Empty, "ownerName.containerName." };
        yield return new object[] { CreateSite("ownerName"), "containerName", "componentName", "ownerName.containerName.componentName" };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateNestedContainer_TestData))]
    public void SiteNestedContainer_Add_Component_Success(ISite ownerSite, string containerName, string componentName, string expectedFullName)
    {
        using SubDesignSurface surface = new();
        using Component ownerComponent = new()
        {
            Site = ownerSite
        };
        IDesignerLoaderHost2 host = surface.Host;
        using INestedContainer container = surface.CreateNestedContainer(ownerComponent, containerName);
        using RootDesignerComponent component1 = new();
        container.Add(component1, componentName);
        Assert.Same(container, component1.Container);
        INestedSite nestedSite = Assert.IsAssignableFrom<INestedSite>(component1.Site);
        Assert.Same(component1, nestedSite.Component);
        Assert.Same(container, nestedSite.Container);
        Assert.True(nestedSite.DesignMode);
        Assert.Equal(expectedFullName, nestedSite.FullName);
        Assert.Same(componentName, component1.Site.Name);
        Assert.Same(component1, Assert.Single(container.Components));
        Assert.Empty(host.Container.Components);
        Assert.Equal(componentName, host.RootComponentClassName);

        // Add another.
        using RootDesignerComponent component2 = new();
        container.Add(component2, "otherComponent");
        Assert.Equal(new IComponent[] { component1, component2 }, container.Components.Cast<IComponent>());
        Assert.Empty(host.Container.Components);

        // Add again.
        container.Add(component1, "newName");
        if (component1.Site is not null)
        {
            Assert.Equal(componentName, component1.Site.Name);
        }

        Assert.Equal(new IComponent[] { component1, component2 }, container.Components.Cast<IComponent>());
        Assert.Empty(host.Container.Components);
    }

    public static IEnumerable<object[]> Add_InvalidNameCreationServiceParentProvider_TestData()
    {
        yield return new object[] { null };

        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(DesignerCommandSet)))
            .Returns(null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IInheritanceService)))
            .Returns(null);
        yield return new object[] { nullMockServiceProvider.Object };

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(new object());
        invalidMockServiceProvider
           .Setup(p => p.GetService(typeof(DesignerCommandSet)))
           .Returns(null);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(IInheritanceService)))
            .Returns(null);
        yield return new object[] { invalidMockServiceProvider.Object };
    }

    public static IEnumerable<object[]> Add_ComponentParentProvider_TestData()
    {
        foreach (object[] testData in Add_InvalidNameCreationServiceParentProvider_TestData())
        {
            yield return new object[] { testData[0] };
        }

        foreach (string name in new string[] { null, string.Empty, "name" })
        {
            Mock<INameCreationService> mockNameCreationService = new(MockBehavior.Strict);
            mockNameCreationService
                .Setup(s => s.CreateName(It.IsAny<IContainer>(), It.IsAny<Type>()))
                .Returns(name);
            Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
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
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(mockNameCreationService.Object);
            yield return new object[] { mockServiceProvider.Object };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_ComponentParentProvider_TestData))]
    public void SiteNestedContainer_Add_ComponentWithRootDesigner_Success(IServiceProvider parentProvider)
    {
        using SubDesignSurface surface = new(parentProvider);
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using RootDesignerComponent component1 = new();
        using RootDesignerComponent component2 = new();
        using DesignerComponent component3 = new();
        using Component component4 = new();

        container.Add(component1);
        Assert.Same(component1, Assert.Single(container.Components));
        Assert.Null(host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(container, component1.Container);
        Assert.Same(component1, component1.Site.Component);
        Assert.Same(container, component1.Site.Container);
        Assert.True(component1.Site.DesignMode);
        Assert.Null(component1.Site.Name);

        // Add different - root designer.
        container.Add(component2);
        Assert.Equal(new IComponent[] { component1, component2 }, container.Components.Cast<IComponent>());
        Assert.Null(host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(container, component2.Container);
        Assert.Same(component2, component2.Site.Component);
        Assert.Same(container, component2.Site.Container);
        Assert.True(component2.Site.DesignMode);
        Assert.Null(component2.Site.Name);

        // Add different - non root designer.
        container.Add(component3);
        Assert.Equal(new IComponent[] { component1, component2, component3 }, container.Components.Cast<IComponent>());
        Assert.Null(host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(container, component3.Container);
        Assert.Same(component3, component3.Site.Component);
        Assert.Same(container, component3.Site.Container);
        Assert.True(component3.Site.DesignMode);
        Assert.Null(component3.Site.Name);

        // Add different - non designer.
        container.Add(component4);
        Assert.Equal(new IComponent[] { component1, component2, component3, component4 }, container.Components.Cast<IComponent>());
        Assert.Null(host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(container, component4.Container);
        Assert.Same(component4, component4.Site.Component);
        Assert.Same(container, component4.Site.Container);
        Assert.True(component4.Site.DesignMode);
        Assert.Null(component4.Site.Name);
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_InvalidNameCreationServiceParentProvider_TestData))]
    public void SiteNestedContainer_Add_ComponentStringWithRootDesigner_Success(IServiceProvider parentProvider)
    {
        using SubDesignSurface surface = new(parentProvider);
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using RootDesignerComponent component1 = new();
        using RootDesignerComponent component2 = new();
        using DesignerComponent component3 = new();
        using Component component4 = new();

        container.Add(component1, "name1");
        Assert.Same(component1, Assert.Single(container.Components));
        Assert.Equal("name1", host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(container, component1.Container);
        Assert.Same(component1, component1.Site.Component);
        Assert.Same(container, component1.Site.Container);
        Assert.True(component1.Site.DesignMode);
        Assert.Equal("name1", component1.Site.Name);

        // Add different - root designer.
        container.Add(component2, "name2");
        Assert.Equal(new IComponent[] { component1, component2 }, container.Components.Cast<IComponent>());
        Assert.Same(container, component2.Container);
        Assert.Equal("name1", host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(component2, component2.Site.Component);
        Assert.Same(container, component2.Site.Container);
        Assert.True(component2.Site.DesignMode);
        Assert.Equal("name2", component2.Site.Name);

        // Add different - non root designer.
        container.Add(component3, string.Empty);
        Assert.Equal(new IComponent[] { component1, component2, component3 }, container.Components.Cast<IComponent>());
        Assert.Same(container, component3.Container);
        Assert.Equal("name1", host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(component3, component3.Site.Component);
        Assert.Same(container, component3.Site.Container);
        Assert.True(component3.Site.DesignMode);
        Assert.Empty(component3.Site.Name);

        // Add different - non designer.
        container.Add(component4, null);
        Assert.Equal(new IComponent[] { component1, component2, component3, component4 }, container.Components.Cast<IComponent>());
        Assert.Same(container, component4.Container);
        Assert.Equal("name1", host.RootComponentClassName);
        Assert.Same(component1, host.RootComponent);
        Assert.Same(component4, component4.Site.Component);
        Assert.Same(container, component4.Site.Container);
        Assert.True(component4.Site.DesignMode);
        Assert.Null(component4.Site.Name);
    }

    public static IEnumerable<object[]> Add_IExtenderProviderServiceWithoutDefault_TestData()
    {
        yield return new object[] { new RootDesignerComponent(), 0 };
        yield return new object[] { new RootExtenderProviderDesignerComponent(), 1 };

        RootExtenderProviderDesignerComponent readOnlyComponent = new();
        TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
        yield return new object[] { readOnlyComponent, 0 };

        RootExtenderProviderDesignerComponent inheritedComponent = new();
        TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
        yield return new object[] { inheritedComponent, 1 };

        RootExtenderProviderDesignerComponent notInheritedComponent = new();
        TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
        yield return new object[] { notInheritedComponent, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_IExtenderProviderServiceWithoutDefault_TestData))]
    public void SiteNestedContainer_Add_IExtenderProviderServiceWithoutDefault_Success(Component component, int expectedCallCount)
    {
        Mock<IExtenderProviderService> mockExtenderProviderService = new(MockBehavior.Strict);
        mockExtenderProviderService
            .Setup(s => s.AddExtenderProvider(component as IExtenderProvider))
            .Verifiable();
        mockExtenderProviderService
            .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider));
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(DesignerCommandSet)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IInheritanceService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderListService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object)
            .Verifiable();

        using SubDesignSurface surface = new(mockServiceProvider.Object);
        surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Null(component.Site.Name);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount));
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount));

        // Add again.
        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Null(component.Site.Name);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount * 2 + 1));
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount * 2));

        // Add again with name.
        container.Add(component, "name");
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Null(component.Site.Name);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount * 3 + 1));
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount * 3));
    }

    public static IEnumerable<object[]> Add_IExtenderProviderServiceWithDefault_TestData()
    {
        yield return new object[] { new RootDesignerComponent(), false };
        yield return new object[] { new RootExtenderProviderDesignerComponent(), true };

        RootExtenderProviderDesignerComponent readOnlyComponent = new();
        TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
        yield return new object[] { readOnlyComponent, false };

        RootExtenderProviderDesignerComponent inheritedComponent = new();
        TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
        yield return new object[] { inheritedComponent, true };

        RootExtenderProviderDesignerComponent notInheritedComponent = new();
        TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
        yield return new object[] { notInheritedComponent, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_IExtenderProviderServiceWithDefault_TestData))]
    public void SiteNestedContainer_Add_IExtenderProviderServiceWithDefault_DoesNotCallGetService(Component component, bool throws)
    {
        Mock<IExtenderProviderService> mockExtenderProviderService = new(MockBehavior.Strict);
        mockExtenderProviderService
            .Setup(s => s.AddExtenderProvider(component as IExtenderProvider))
            .Verifiable();
        mockExtenderProviderService
            .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider));
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(DesignerCommandSet)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IInheritanceService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object)
            .Verifiable();

        using DesignSurface surface = new(mockServiceProvider.Object);
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Null(component.Site.Name);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Never());

        // Add again.
        if (throws)
        {
            Assert.Throws<ArgumentException>("provider", () => container.Add(component));
            Assert.Empty(container.Components);
            Assert.Null(component.Site);
        }
        else
        {
            container.Add(component);
            Assert.Same(component, Assert.Single(container.Components));
            Assert.Null(component.Site.Name);
        }

        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Never());

        // Add again with name.
        container.Add(component, "name");
        Assert.Same(component, Assert.Single(container.Components));
        if (throws)
        {
            Assert.Equal("name", component.Site.Name);
        }
        else
        {
            Assert.Null(component.Site.Name);
        }

        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Never());
    }

    public static IEnumerable<object[]> InvalidIExtenderProviderService_TestData()
    {
        yield return new object[] { null };

        Mock<IServiceProvider> nullMockServiceProvider = new(MockBehavior.Strict);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        nullMockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(null)
            .Verifiable();
        yield return new object[] { nullMockServiceProvider };

        Mock<IServiceProvider> invalidMockServiceProvider = new(MockBehavior.Strict);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        invalidMockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(new object())
            .Verifiable();
        yield return new object[] { invalidMockServiceProvider };
    }

    [WinFormsTheory]
    [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
    public void SiteNestedContainer_Add_InvalidIExtenderProviderServiceWithoutDefault_CallsParentGetService(Mock<IServiceProvider> mockParentProvider)
    {
        using SubDesignSurface surface = new(mockParentProvider?.Object);
        surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using RootExtenderProviderDesignerComponent component = new();

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());
    }

    [WinFormsTheory]
    [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
    public void SiteNestedContainer_Add_InvalidIExtenderProviderServiceWithDefault_DoesNotCallParentGetService(Mock<IServiceProvider> mockParentProvider)
    {
        using SubDesignSurface surface = new(mockParentProvider?.Object);
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using RootExtenderProviderDesignerComponent component = new();

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
    }

    [WinFormsFact]
    public void SiteNestedContainer_Add_IComponentWithComponentAddingAndAdded_CallsHandler()
    {
        using SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

        using RootDesignerComponent component = new();
        int componentAddingCallCount = 0;
        ComponentEventHandler componentAddingHandler = (sender, e) =>
        {
            Assert.Same(container, sender);
            Assert.Same(component, e.Component);
            componentAddingCallCount++;
        };
        changeService.ComponentAdding += componentAddingHandler;
        int componentAddedCallCount = 0;
        ComponentEventHandler componentAddedHandler = (sender, e) =>
        {
            Assert.Same(container, sender);
            Assert.Same(component, e.Component);
            Assert.True(componentAddedCallCount < componentAddingCallCount);
            componentAddedCallCount++;
        };
        changeService.ComponentAdded += componentAddedHandler;

        // With handler.
        container.Add(component);
        Assert.Same(container, component.Container);
        Assert.Equal(1, componentAddingCallCount);
        Assert.Equal(1, componentAddedCallCount);

        // Add again.
        container.Add(component);
        Assert.Same(container, component.Container);
        Assert.Equal(2, componentAddingCallCount);
        Assert.Equal(2, componentAddedCallCount);

        // Remove handler.
        changeService.ComponentAdding -= componentAddingHandler;
        changeService.ComponentAdded -= componentAddedHandler;
        container.Add(component);
        Assert.Same(container, component.Container);
        Assert.Equal(2, componentAddingCallCount);
        Assert.Equal(2, componentAddedCallCount);
    }

    public static IEnumerable<object[]> Add_NoRootDesigner_TestData()
    {
        yield return new object[] { new Component() };
        yield return new object[] { new DesignerComponent() };
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_NoRootDesigner_TestData))]
    public void SiteNestedContainer_Add_NoRootDesigner_ThrowsException(IComponent component)
    {
        using DesignSurface surface = new();
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        Assert.Throws<NotImplementedException>(() => container.Add(component));
        Assert.Throws<NotImplementedException>(() => container.Add(component, "name"));
        Assert.Empty(container.Components);
    }

    [WinFormsFact]
    public void SiteNestedContainer_Add_CyclicRootDesigner_ThrowsException()
    {
        using SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using RootDesignerComponent component = new();
        container.Add(component, component.GetType().FullName);
        Assert.Equal(component.GetType().FullName, host.RootComponentClassName);
        Assert.Throws<InvalidOperationException>(() => container.Add(component));
        Assert.Throws<InvalidOperationException>(() => container.Add(new RootDesignerComponent(), host.RootComponentClassName));
    }

    [WinFormsFact]
    public void SiteNestedContainer_Add_NonInitializingRootDesigner_ThrowsInvalidOperationException()
    {
        using DesignSurface surface = new();
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using NonInitializingDesignerComponent component = new();
        Assert.Throws<InvalidOperationException>(() => container.Add(component));
        Assert.Throws<InvalidOperationException>(() => container.Add(component, "name"));
    }

    [WinFormsFact]
    public void SiteNestedContainer_Add_ThrowingInitializingRootDesigner_RethrowsException()
    {
        using DesignSurface surface = new();
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using ThrowingInitializingDesignerComponent component = new();
        Assert.Throws<DivideByZeroException>(() => container.Add(component));
        Assert.Null(component.Container);
        Assert.Null(component.Site);

        Assert.Throws<DivideByZeroException>(() => container.Add(component, "name"));
        Assert.Null(component.Container);
        Assert.Null(component.Site);
    }

    [WinFormsFact]
    public void SiteNestedContainer_Add_CheckoutExceptionThrowingInitializingRootDesigner_RethrowsException()
    {
        using DesignSurface surface = new();
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using CheckoutExceptionThrowingInitializingDesignerComponent component = new();
        // CheckoutException does not bubble up in xunit.
        bool threwCheckoutException = false;
        try
        {
            container.Add(component);
        }
        catch (CheckoutException)
        {
            threwCheckoutException = true;
        }

        Assert.True(threwCheckoutException);
        Assert.Same(container, component.Container);
        Assert.Null(component.Site.Name);

        container.Add(component, "name");
        Assert.Same(container, component.Container);
        Assert.Null(component.Site.Name);
    }

    [Fact(Skip = "Unstable test. See https://github.com/dotnet/winforms/issues/1151")]
    public void SiteNestedContainer_Add_Unloading_Nop()
    {
        using SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        surface.BeginLoad(typeof(RootDesignerComponent));
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        DisposingDesignerComponent component = new();
        container.Add(component);
        int callCount = 0;
        DisposingDesigner.s_disposed += (sender, e) =>
        {
            callCount++;
        };
        surface.Dispose();
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public void SiteNestedContainer_Remove_Invoke_Success()
    {
        using SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        using RootDesignerComponent rootComponent = new();
        using DesignerComponent component = new();
        container.Add(rootComponent);
        container.Add(component);
        container.Remove(rootComponent);
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Null(host.RootComponent);
        Assert.Null(host.RootComponentClassName);
        Assert.Null(rootComponent.Container);
        Assert.Null(rootComponent.Site);
        Assert.Same(container, component.Container);
        Assert.NotNull(component.Site);

        // Remove again.
        container.Remove(rootComponent);
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Null(host.RootComponent);
        Assert.Null(host.RootComponentClassName);
        Assert.Null(rootComponent.Container);
        Assert.Null(rootComponent.Site);
        Assert.Same(container, component.Container);
        Assert.NotNull(component.Site);

        // Remove other.
        container.Remove(component);
        Assert.Empty(container.Components);
        Assert.Null(host.RootComponent);
        Assert.Null(host.RootComponentClassName);
        Assert.Null(rootComponent.Container);
        Assert.Null(rootComponent.Site);
        Assert.Null(component.Container);
        Assert.Null(component.Site);
    }

    public static IEnumerable<object[]> Remove_IExtenderProviderServiceWithoutDefault_TestData()
    {
        yield return new object[] { new RootDesignerComponent(), 0, 0 };
        yield return new object[] { new RootExtenderProviderDesignerComponent(), 1, 1 };

        RootExtenderProviderDesignerComponent readOnlyComponent = new();
        TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
        yield return new object[] { readOnlyComponent, 0, 1 };

        RootExtenderProviderDesignerComponent inheritedComponent = new();
        TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
        yield return new object[] { inheritedComponent, 1, 1 };

        RootExtenderProviderDesignerComponent notInheritedComponent = new();
        TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
        yield return new object[] { notInheritedComponent, 1, 1 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Remove_IExtenderProviderServiceWithoutDefault_TestData))]
    public void SiteNestedContainer_Remove_IExtenderProviderServiceWithoutDefault_Success(Component component, int expectedAddCallCount, int expectedRemoveCallCount)
    {
        Mock<IExtenderProviderService> mockExtenderProviderService = new(MockBehavior.Strict);
        mockExtenderProviderService
            .Setup(s => s.AddExtenderProvider(component as IExtenderProvider));
        mockExtenderProviderService
            .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider))
            .Verifiable();
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object)
            .Verifiable();

        using SubDesignSurface surface = new(mockServiceProvider.Object);
        surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Same(container, component.Container);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedAddCallCount));

        // Remove.
        container.Remove(component);
        Assert.Empty(container.Components);
        Assert.Null(component.Container);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedAddCallCount + expectedRemoveCallCount));
        mockExtenderProviderService.Verify(s => s.RemoveExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedRemoveCallCount));

        // Remove again.
        container.Remove(component);
        Assert.Empty(container.Components);
        Assert.Null(component.Container);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedAddCallCount + expectedRemoveCallCount));
        mockExtenderProviderService.Verify(s => s.RemoveExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedRemoveCallCount));
    }

    public static IEnumerable<object[]> Remove_IExtenderProviderServiceWithDefault_TestData()
    {
        yield return new object[] { new RootDesignerComponent() };
        yield return new object[] { new RootExtenderProviderDesignerComponent() };

        RootExtenderProviderDesignerComponent readOnlyComponent = new();
        TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
        yield return new object[] { readOnlyComponent };

        RootExtenderProviderDesignerComponent inheritedComponent = new();
        TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
        yield return new object[] { inheritedComponent };

        RootExtenderProviderDesignerComponent notInheritedComponent = new();
        TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
        yield return new object[] { notInheritedComponent };
    }

    [WinFormsTheory]
    [MemberData(nameof(Remove_IExtenderProviderServiceWithDefault_TestData))]
    public void SiteNestedContainer_Remove_IExtenderProviderServiceWithDefault_Success(Component component)
    {
        Mock<IExtenderProviderService> mockExtenderProviderService = new(MockBehavior.Strict);
        mockExtenderProviderService
            .Setup(s => s.AddExtenderProvider(component as IExtenderProvider))
            .Verifiable();
        mockExtenderProviderService
            .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider));
        Mock<IServiceProvider> mockServiceProvider = new(MockBehavior.Strict);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(ITypeResolutionService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(INameCreationService)))
            .Returns(null);
        mockServiceProvider
            .Setup(p => p.GetService(typeof(IExtenderProviderService)))
            .Returns(mockExtenderProviderService.Object)
            .Verifiable();

        using DesignSurface surface = new(mockServiceProvider.Object);
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        Assert.Null(component.Site.Name);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Never());

        // Remove.
        container.Remove(component);
        Assert.Empty(container.Components);
        Assert.Null(component.Container);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockExtenderProviderService.Verify(s => s.RemoveExtenderProvider(component as IExtenderProvider), Times.Never());

        // Remove again.
        container.Remove(component);
        Assert.Empty(container.Components);
        Assert.Null(component.Container);
        mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        mockExtenderProviderService.Verify(s => s.RemoveExtenderProvider(component as IExtenderProvider), Times.Never());
    }

    [WinFormsTheory]
    [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
    public void SiteNestedContainer_Remove_InvalidIExtenderProviderServiceWithoutDefault_CallsParentGetService(Mock<IServiceProvider> mockParentProvider)
    {
        using SubDesignSurface surface = new(mockParentProvider?.Object);
        surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using RootExtenderProviderDesignerComponent component = new();

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());

        container.Remove(component);
        Assert.Empty(container.Components);
        mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(2));
    }

    [WinFormsTheory]
    [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
    public void SiteNestedContainer_Remove_InvalidIExtenderProviderServiceWithDefault_DoesNotCallParentGetService(Mock<IServiceProvider> mockParentProvider)
    {
        using DesignSurface surface = new(mockParentProvider?.Object);
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        using RootExtenderProviderDesignerComponent component = new();

        container.Add(component);
        Assert.Same(component, Assert.Single(container.Components));
        mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());

        container.Remove(component);
        Assert.Empty(container.Components);
        mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
    }

    [WinFormsFact]
    public void SiteNestedContainer_Remove_ComponentNotInContainerNonEmpty_Nop()
    {
        using DesignSurface surface = new();
        using Component component1 = new();
        using INestedContainer container1 = surface.CreateNestedContainer(component1, "containerName");
        using Component component2 = new();
        using INestedContainer container2 = surface.CreateNestedContainer(component2, "containerName");

        using RootDesignerComponent otherComponent = new();
        using RootDesignerComponent component = new();
        container1.Add(otherComponent);
        container2.Add(component);
        container2.Remove(otherComponent);
        container2.Remove(new Component());
        Assert.Same(otherComponent, Assert.Single(container1.Components));
        Assert.Same(component, Assert.Single(container2.Components));
    }

    [WinFormsFact]
    public void SiteNestedContainer_Remove_ComponentNotInContainerEmpty_Nop()
    {
        using DesignSurface surface = new();
        using Component component1 = new();
        using INestedContainer container1 = surface.CreateNestedContainer(component1, "containerName");
        using Component component2 = new();
        using INestedContainer container2 = surface.CreateNestedContainer(component2, "containerName");

        using RootDesignerComponent otherComponent = new();
        container1.Add(otherComponent);
        container2.Remove(otherComponent);
        container2.Remove(new Component());
        Assert.Same(otherComponent, Assert.Single(container1.Components));
        Assert.Empty(container2.Components);
    }

    [WinFormsFact]
    public void SiteNestedContainer_Remove_InvokeWithComponentRemoved_CallsHandler()
    {
        using SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

        using RootDesignerComponent component1 = new();
        using DesignerComponent component2 = new();
        int componentRemovingCallCount = 0;
        ComponentEventHandler componentRemovingHandler = (sender, e) =>
        {
            Assert.Same(host, sender);
            Assert.Same(component1, e.Component);
            Assert.NotNull(e.Component.Site);
            componentRemovingCallCount++;
        };
        changeService.ComponentRemoving += componentRemovingHandler;

        int componentRemovedCallCount = 0;
        ComponentEventHandler componentRemovedHandler = (sender, e) =>
        {
            Assert.Same(host, sender);
            Assert.Same(component1, e.Component);
            Assert.NotNull(e.Component.Site);
            Assert.True(componentRemovedCallCount < componentRemovingCallCount);
            componentRemovedCallCount++;
        };
        changeService.ComponentRemoved += componentRemovedHandler;

        container.Add(component1);
        container.Add(component2);

        // With handler.
        container.Remove(component1);
        Assert.Null(component1.Container);
        Assert.Null(component1.Site);
        Assert.Same(container, component2.Container);
        Assert.NotNull(component2.Site);
        Assert.Same(component2, Assert.Single(container.Components));
        Assert.Equal(1, componentRemovingCallCount);
        Assert.Equal(1, componentRemovedCallCount);

        // Remove again.
        container.Remove(component1);
        Assert.Null(component1.Container);
        Assert.Null(component1.Site);
        Assert.Same(container, component2.Container);
        Assert.NotNull(component2.Site);
        Assert.Same(component2, Assert.Single(container.Components));
        Assert.Equal(1, componentRemovingCallCount);
        Assert.Equal(1, componentRemovedCallCount);

        // Remove handler.
        changeService.ComponentRemoving -= componentRemovingHandler;
        changeService.ComponentRemoved -= componentRemovedHandler;
        container.Remove(component2);
        Assert.Null(component1.Container);
        Assert.Null(component1.Site);
        Assert.Null(component2.Container);
        Assert.Null(component2.Site);
        Assert.Empty(host.Container.Components);
        Assert.Equal(1, componentRemovingCallCount);
        Assert.Equal(1, componentRemovedCallCount);
    }

    [WinFormsFact]
    public void SiteNestedContainer_Remove_SetSiteToNullInComponentRemoving_Success()
    {
        using SubDesignSurface surface = new();
        IDesignerLoaderHost2 host = surface.Host;
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");
        IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

        using RootDesignerComponent component = new();
        int componentRemovingCallCount = 0;
        ComponentEventHandler componentRemovingHandler = (sender, e) =>
        {
            component.Site = null;
            componentRemovingCallCount++;
        };
        changeService.ComponentRemoving += componentRemovingHandler;

        container.Add(component);
        container.Remove(component);
        Assert.Null(component.Container);
        Assert.Null(component.Site);
    }

    [WinFormsFact]
    public void SiteNestedContainer_Remove_SiteHasDictionary_DoesNotClearDictionary()
    {
        using SubDesignSurface surface = new();
        using Component owningComponent = new();
        using INestedContainer container = surface.CreateNestedContainer(owningComponent, "containerName");

        using RootDesignerComponent component = new();
        container.Add(component);
        IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
        service.SetValue("key", "value");
        Assert.Equal("value", service.GetValue("key"));

        container.Remove(component);
        Assert.Equal("value", service.GetValue("key"));
    }

    private class SubDesignSurface : DesignSurface
    {
        public SubDesignSurface() : base()
        {
        }

        public SubDesignSurface(IServiceProvider parentProvider) : base(parentProvider)
        {
        }

        public new ServiceContainer ServiceContainer => base.ServiceContainer;

        public IDesignerLoaderHost2 Host => Assert.IsAssignableFrom<IDesignerLoaderHost2>(ComponentContainer);
    }

    private abstract class CustomDesigner : IDesigner
    {
        protected IComponent _initializedComponent;

        public IComponent Component => _initializedComponent;

        public DesignerVerbCollection Verbs { get; }

        public void DoDefaultAction()
        {
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
        }

        public abstract void Initialize(IComponent component);
    }

    private class Designer : CustomDesigner
    {
        public override void Initialize(IComponent component) => _initializedComponent = component;
    }

    [Designer(typeof(Designer))]
    private class DesignerComponent : Component
    {
    }

    private class RootDesigner : Designer, IRootDesigner
    {
        public ViewTechnology[] SupportedTechnologies { get; }

        public object GetView(ViewTechnology technology) => throw new NotImplementedException();
    }

    [Designer(typeof(RootDesigner), typeof(IRootDesigner))]
    private class RootDesignerComponent : Component
    {
    }

    [Designer(typeof(RootDesigner), typeof(IRootDesigner))]
    private class RootExtenderProviderDesignerComponent : Component, IExtenderProvider
    {
        public bool CanExtend(object extendee) => false;
    }

    private class NonInitializingDesigner : RootDesigner
    {
        public override void Initialize(IComponent component)
        {
        }
    }

    [Designer(typeof(NonInitializingDesigner), typeof(IRootDesigner))]
    private class NonInitializingDesignerComponent : Component
    {
    }

    private class ThrowingInitializingDesigner : RootDesigner
    {
        public override void Initialize(IComponent component)
        {
            throw new DivideByZeroException();
        }
    }

    [Designer(typeof(ThrowingInitializingDesigner), typeof(IRootDesigner))]
    private class ThrowingInitializingDesignerComponent : Component
    {
    }

    private class CheckoutExceptionThrowingInitializingDesigner : RootDesigner
    {
        public override void Initialize(IComponent component)
        {
            throw CheckoutException.Canceled;
        }
    }

    [Designer(typeof(CheckoutExceptionThrowingInitializingDesigner), typeof(IRootDesigner))]
    private class CheckoutExceptionThrowingInitializingDesignerComponent : Component
    {
    }

    private class DisposingDesigner : Designer
    {
        public static EventHandler s_disposed;

        protected override void Dispose(bool disposing)
        {
            s_disposed?.Invoke(this, EventArgs.Empty);
        }
    }

    [Designer(typeof(DisposingDesigner))]
    private class DisposingDesignerComponent : Component
    {
    }

    [Designer(typeof(RootDesigner), typeof(IRootDesigner))]
    private class CustomTypeDescriptionProviderComponent : Component
    {
    }
}
