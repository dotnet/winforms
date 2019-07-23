// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using Moq;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class SiteNestedContainerTests
    {
        public static IEnumerable<object[]> CreateNestedContainer_TestData()
        {
            ISite CreateSite(string name)
            {
                var mockSite = new Mock<ISite>(MockBehavior.Strict);
                mockSite
                    .Setup(s => s.Name)
                    .Returns(name);
                mockSite
                    .Setup(s => s.GetService(typeof(ContainerFilterService)))
                    .Returns(null);
                return mockSite.Object;
            }

            foreach (ISite site in new ISite[] { null, CreateSite(null )})
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

        [Theory]
        [MemberData(nameof(CreateNestedContainer_TestData))]
        public void SiteNestedContainer_Add_Component_Success(ISite ownerSite, string containerName, string componentName, string expectedFullName)
        {
            var surface = new SubDesignSurface();
            var ownerComponent = new Component
            {
                Site = ownerSite
            };
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(ownerComponent, containerName);
            var component1 = new RootDesignerComponent();
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
            var component2 = new RootDesignerComponent();
            container.Add(component2, "otherComponent");
            Assert.Equal(new IComponent[] { component1, component2 }, container.Components.Cast<IComponent>());
            Assert.Empty(host.Container.Components);

            // Add again.
            container.Add(component1, "newName");
            if (component1.Site != null)
            {
                Assert.Equal(componentName, component1.Site.Name);
            }
            Assert.Equal(new IComponent[] { component1, component2 }, container.Components.Cast<IComponent>());
            Assert.Empty(host.Container.Components);
        }

        public static IEnumerable<object[]> Add_InvalidNameCreationServiceParentProvider_TestData()
        {
            yield return new object[] { null };

            var nullMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            yield return new object[] { nullMockServiceProvider.Object };

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(new object());
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
                var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Strict);
                mockNameCreationService
                    .Setup(s => s.CreateName(It.IsAny<IContainer>(), It.IsAny<Type>()))
                    .Returns(name);
                var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
                mockServiceProvider
                    .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                    .Returns(null);
                mockServiceProvider
                    .Setup(p => p.GetService(typeof(INameCreationService)))
                    .Returns(mockNameCreationService.Object);
                yield return new object[] { mockServiceProvider.Object };
            }
        }

        [Theory]
        [MemberData(nameof(Add_ComponentParentProvider_TestData))]
        public void SiteNestedContainer_Add_ComponentWithRootDesigner_Success(IServiceProvider parentProvider)
        {
            var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component1 = new RootDesignerComponent();
            var component2 = new RootDesignerComponent();
            var component3 = new DesignerComponent();
            var component4 = new Component();

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

        [Theory]
        [MemberData(nameof(Add_InvalidNameCreationServiceParentProvider_TestData))]
        public void SiteNestedContainer_Add_ComponentStringWithRootDesigner_Success(IServiceProvider parentProvider)
        {
            var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component1 = new RootDesignerComponent();
            var component2 = new RootDesignerComponent();
            var component3 = new DesignerComponent();
            var component4 = new Component();

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

            var readOnlyComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
            yield return new object[] { readOnlyComponent, 0 };

            var inheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
            yield return new object[] { inheritedComponent, 1 };

            var notInheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
            yield return new object[] { notInheritedComponent, 1 };
        }

        [Theory]
        [MemberData(nameof(Add_IExtenderProviderServiceWithoutDefault_TestData))]
        public void SiteNestedContainer_Add_IExtenderProviderServiceWithoutDefault_Success(Component component, int expectedCallCount)
        {
            var mockExtenderProviderService = new Mock<IExtenderProviderService>(MockBehavior.Strict);
            mockExtenderProviderService
                .Setup(s => s.AddExtenderProvider(component as IExtenderProvider))
                .Verifiable();
            mockExtenderProviderService
                .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider));
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
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

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");

            container.Add(component);
            Assert.Same(component, Assert.Single(container.Components));
            Assert.Null(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount));
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount));

            // Add again.
            container.Add(component);
            Assert.Same(component, Assert.Single(container.Components));
            Assert.Null(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount * 2));
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount * 2));

            // Add again with name.
            container.Add(component, "name");
            Assert.Same(component, Assert.Single(container.Components));
            Assert.Null(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount * 3));
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount * 3));
        }

        public static IEnumerable<object[]> Add_IExtenderProviderServiceWithDefault_TestData()
        {
            yield return new object[] { new RootDesignerComponent(), false };
            yield return new object[] { new RootExtenderProviderDesignerComponent(), true };

            var readOnlyComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
            yield return new object[] { readOnlyComponent, false };

            var inheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
            yield return new object[] { inheritedComponent, true };

            var notInheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
            yield return new object[] { notInheritedComponent, true };
        }

        [Theory]
        [MemberData(nameof(Add_IExtenderProviderServiceWithDefault_TestData))]
        public void SiteNestedContainer_Add_IExtenderProviderServiceWithDefault_DoesNotCallGetService(Component component, bool throws)
        {
            var mockExtenderProviderService = new Mock<IExtenderProviderService>(MockBehavior.Strict);
            mockExtenderProviderService
                .Setup(s => s.AddExtenderProvider(component as IExtenderProvider))
                .Verifiable();
            mockExtenderProviderService
                .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider));
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
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

            var surface = new DesignSurface(mockServiceProvider.Object);
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");

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

            var nullMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
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

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
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

        [Theory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void SiteNestedContainer_Add_InvalidIExtenderProviderServiceWithoutDefault_CallsParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            var surface = new SubDesignSurface(mockParentProvider?.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new RootExtenderProviderDesignerComponent();

            container.Add(component);
            Assert.Same(component, Assert.Single(container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());
        }

        [Theory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void SiteNestedContainer_Add_InvalidIExtenderProviderServiceWithDefault_DoesNotCallParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            var surface = new SubDesignSurface(mockParentProvider?.Object);
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new RootExtenderProviderDesignerComponent();

            container.Add(component);
            Assert.Same(component, Assert.Single(container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        }

        [Fact]
        public void SiteNestedContainer_Add_IComponentWithComponentAddingAndAdded_CallsHandler()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            var component = new RootDesignerComponent();
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

        [Fact]
        public void SiteNestedContainer_Add_NullComponent_ThrowsArgumentNullException()
        {
            var surface = new DesignSurface();
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            Assert.Throws<ArgumentNullException>("component", () => container.Add(null, "name"));
        }

        public static IEnumerable<object[]> Add_NoRootDesigner_TestData()
        {
            yield return new object[] { new Component() };
            yield return new object[] { new DesignerComponent() };
        }

        [Theory]
        [MemberData(nameof(Add_NoRootDesigner_TestData))]
        public void SiteNestedContainer_Add_NoRootDesigner_ThrowsException(IComponent component)
        {
            var surface = new DesignSurface();
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            Assert.Throws<Exception>(() => container.Add(component));
            Assert.Throws<Exception>(() => container.Add(component, "name"));
            Assert.Empty(container.Components);
        }

        [Fact]
        public void SiteNestedContainer_Add_CyclicRootDesigner_ThrowsException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new RootDesignerComponent();
            container.Add(component, component.GetType().FullName);
            Assert.Equal(component.GetType().FullName, host.RootComponentClassName);
            Assert.Throws<Exception>(() => container.Add(component));
            Assert.Throws<Exception>(() => container.Add(new RootDesignerComponent(), host.RootComponentClassName));
        }

        [Fact]
        public void SiteNestedContainer_Add_NonInitializingRootDesigner_ThrowsInvalidOperationException()
        {
            var surface = new DesignSurface();
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new NonInitializingDesignerComponent();
            Assert.Throws<InvalidOperationException>(() => container.Add(component));
            Assert.Throws<InvalidOperationException>(() => container.Add(component, "name"));
        }

        [Fact]
        public void SiteNestedContainer_Add_ThrowingInitializingRootDesigner_RethrowsException()
        {
            var surface = new DesignSurface();
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new ThrowingInitializingDesignerComponent();
            Assert.Throws<DivideByZeroException>(() => container.Add(component));
            Assert.Null(component.Container);
            Assert.Null(component.Site);

            Assert.Throws<DivideByZeroException>(() => container.Add(component, "name"));
            Assert.Null(component.Container);
            Assert.Null(component.Site);
        }

        [Fact]
        public void SiteNestedContainer_Add_CheckoutExceptionThrowingInitializingRootDesigner_RethrowsException()
        {
            var surface = new DesignSurface();
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new CheckoutExceptionThrowingInitializingDesignerComponent();
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

        // Commenting out failing test
        // Tracked by https://github.com/dotnet/winforms/issues/1151
        // [Fact]
        // public void SiteNestedContainer_Add_Unloading_Nop()
        // {
        //     var surface = new SubDesignSurface();
        //     IDesignerLoaderHost2 host = surface.Host;
        //     surface.BeginLoad(typeof(RootDesignerComponent));
        //     INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");

        //     var component = new DisposingDesignerComponent();
        //     container.Add(component);
        //     int callCount = 0;
        //     DisposingDesigner.Disposed += (sender, e) =>
        //     {
        //         callCount++;
        //     };
        //     surface.Dispose();
        //     Assert.Equal(0, callCount);
        // }

        [Fact]
        public void SiteNestedContainer_Remove_Invoke_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");

            var rootComponent = new RootDesignerComponent();
            var component = new DesignerComponent();
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

            var readOnlyComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
            yield return new object[] { readOnlyComponent, 0, 1 };

            var inheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
            yield return new object[] { inheritedComponent, 1, 1 };

            var notInheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
            yield return new object[] { notInheritedComponent, 1, 1 };
        }

        [Theory]
        [MemberData(nameof(Remove_IExtenderProviderServiceWithoutDefault_TestData))]
        public void SiteNestedContainer_Remove_IExtenderProviderServiceWithoutDefault_Success(Component component, int expectedAddCallCount, int expectedRemoveCallCount)
        {
            var mockExtenderProviderService = new Mock<IExtenderProviderService>(MockBehavior.Strict);
            mockExtenderProviderService
                .Setup(s => s.AddExtenderProvider(component as IExtenderProvider));
            mockExtenderProviderService
                .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider))
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
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

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");

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

            var readOnlyComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(readOnlyComponent, new InheritanceAttribute(InheritanceLevel.InheritedReadOnly));
            yield return new object[] { readOnlyComponent };

            var inheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(inheritedComponent, new InheritanceAttribute(InheritanceLevel.Inherited));
            yield return new object[] { inheritedComponent };

            var notInheritedComponent = new RootExtenderProviderDesignerComponent();
            TypeDescriptor.AddAttributes(notInheritedComponent, new InheritanceAttribute(InheritanceLevel.NotInherited));
            yield return new object[] { notInheritedComponent };
        }

        [Theory]
        [MemberData(nameof(Remove_IExtenderProviderServiceWithDefault_TestData))]
        public void SiteNestedContainer_Remove_IExtenderProviderServiceWithDefault_Success(Component component)
        {
            var mockExtenderProviderService = new Mock<IExtenderProviderService>(MockBehavior.Strict);
            mockExtenderProviderService
                .Setup(s => s.AddExtenderProvider(component as IExtenderProvider))
                .Verifiable();
            mockExtenderProviderService
                .Setup(s => s.RemoveExtenderProvider(component as IExtenderProvider));
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
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

            var surface = new DesignSurface(mockServiceProvider.Object);
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");

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

        [Theory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void SiteNestedContainer_Remove_InvalidIExtenderProviderServiceWithoutDefault_CallsParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            var surface = new SubDesignSurface(mockParentProvider?.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new RootExtenderProviderDesignerComponent();

            container.Add(component);
            Assert.Same(component, Assert.Single(container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());

            container.Remove(component);
            Assert.Empty(container.Components);
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void SiteNestedContainer_Remove_InvalidIExtenderProviderServiceWithDefault_DoesNotCallParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            var surface = new DesignSurface(mockParentProvider?.Object);
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            var component = new RootExtenderProviderDesignerComponent();

            container.Add(component);
            Assert.Same(component, Assert.Single(container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());

            container.Remove(component);
            Assert.Empty(container.Components);
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        }

        [Fact]
        public void SiteNestedContainer_Remove_ComponentNotInContainerNonEmpty_Nop()
        {
            var surface = new DesignSurface();
            INestedContainer container1 = surface.CreateNestedContainer(new Component(), "containerName");
            INestedContainer container2 = surface.CreateNestedContainer(new Component(), "containerName");

            var otherComponent = new RootDesignerComponent();
            var component = new RootDesignerComponent();
            container1.Add(otherComponent);
            container2.Add(component);
            container2.Remove(otherComponent);
            container2.Remove(new Component());
            Assert.Same(otherComponent, Assert.Single(container1.Components));
            Assert.Same(component, Assert.Single(container2.Components));
        }

        [Fact]
        public void SiteNestedContainer_Remove_ComponentNotInContainerEmpty_Nop()
        {
            var surface = new DesignSurface();
            INestedContainer container1 = surface.CreateNestedContainer(new Component(), "containerName");
            INestedContainer container2 = surface.CreateNestedContainer(new Component(), "containerName");

            var otherComponent = new RootDesignerComponent();
            container1.Add(otherComponent);
            container2.Remove(otherComponent);
            container2.Remove(new Component());
            Assert.Same(otherComponent, Assert.Single(container1.Components));
            Assert.Empty(container2.Components);
        }

        [Fact]
        public void SiteNestedContainer_Remove_InvokeWithComponentRemoved_CallsHandler()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            var component1 = new RootDesignerComponent();
            var component2 = new DesignerComponent();
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

        [Fact]
        public void SiteNestedContainer_Remove_SetSiteToNullInComponentRemoving_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            var component = new RootDesignerComponent();
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

        [Fact]
        public void SiteNestedContainer_Remove_SiteHasDictionary_DoesNotClearDictionary()
        {
            var surface = new SubDesignSurface();
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");

            var component = new RootDesignerComponent();
            container.Add(component);
            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            service.SetValue("key", "value");
            Assert.Equal("value", service.GetValue("key"));

            container.Remove(component);
            Assert.Equal("value", service.GetValue("key"));
        }

        [Fact]
        public void SiteNestedContainer_Remove_NullComponent_ThrowsArgumentNullException()
        {
            var surface = new DesignSurface();
            INestedContainer container = surface.CreateNestedContainer(new Component(), "containerName");
            Assert.Throws<ArgumentNullException>("component", () => container.Remove(null));
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
            public bool CanExtend(object extendee) => throw new NotImplementedException();
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
            public static EventHandler Disposed = null;

            protected override void Dispose(bool disposing)
            {
                Disposed?.Invoke(this, EventArgs.Empty);
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
}
