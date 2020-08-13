// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignerHostTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerHost_CanReloadWithErrors_Set_GetReturnsExpected(bool value)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            host.CanReloadWithErrors = value;
            Assert.Equal(value, host.CanReloadWithErrors);

            // Set same.
            host.CanReloadWithErrors = value;
            Assert.Equal(value, host.CanReloadWithErrors);

            // Set different.
            host.CanReloadWithErrors = !value;
            Assert.Equal(!value, host.CanReloadWithErrors);
        }

        [WinFormsFact]
        public void DesignerHost_Container_Get_ReturnsHost()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Same(host, host.Container);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerHost_IgnoreErrorsDuringReload_Set_GetReturnsExpected(bool value)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            host.IgnoreErrorsDuringReload = value;
            Assert.False(host.IgnoreErrorsDuringReload);

            // Set same.
            host.IgnoreErrorsDuringReload = value;
            Assert.False(host.IgnoreErrorsDuringReload);

            // Set different.
            host.IgnoreErrorsDuringReload = !value;
            Assert.False(host.IgnoreErrorsDuringReload);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerHost_IgnoreErrorsDuringReload_SetWithCanReloadWithErrors_GetReturnsExpected(bool value)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            host.CanReloadWithErrors = true;

            host.IgnoreErrorsDuringReload = value;
            Assert.Equal(value, host.IgnoreErrorsDuringReload);

            // Set same.
            host.IgnoreErrorsDuringReload = value;
            Assert.Equal(value, host.IgnoreErrorsDuringReload);

            // Set different.
            host.IgnoreErrorsDuringReload = !value;
            Assert.Equal(!value, host.IgnoreErrorsDuringReload);
        }

        [WinFormsFact]
        public void DesignerHost_InTransaction_GetWithoutTransactions_ReturnsFalse()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.False(host.InTransaction);
        }

        [WinFormsFact]
        public void DesignerHost_IsClosingTransaction_GetWithoutTransaction_ReturnsFalse()
        {
            using var surface = new SubDesignSurface();
            IDesignerHostTransactionState hostTransactionState = Assert.IsAssignableFrom<IDesignerHostTransactionState>(surface.Host);
            Assert.False(hostTransactionState.IsClosingTransaction);
        }

        [WinFormsFact]
        public void DesignerHost_Loading_GetWithoutComponent_ReturnsFalse()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.False(host.Loading);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerHost_Loading_GetWithLoader_ReturnsExpected(bool loading)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host));
            mockLoader
                .Setup(l => l.Loading)
                .Returns(loading);
            surface.BeginLoad(mockLoader.Object);
            Assert.True(host.Loading);
            mockLoader.Verify(l => l.Loading, Times.Never());

            host.EndLoad(null, true, null);
            Assert.Equal(loading, host.Loading);
            mockLoader.Verify(l => l.Loading, Times.Once());
        }

        [WinFormsFact]
        public void DesignerHost_RootComponent_GetWithoutComponent_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Null(host.RootComponent);
        }

        [WinFormsFact]
        public void DesignerHost_RootComponentClassName_GetWithoutComponent_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Null(host.RootComponentClassName);
        }

        [WinFormsFact]
        public void DesignerHost_TransactionDescription_GetWithoutTransactions_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Null(host.TransactionDescription);
        }

        [WinFormsFact]
        public void DesignerHost_Activate_Invoke_CallsViewActivated()
        {
            using var surface = new SubDesignSurface();
            int viewActivatedCallCount = 0;
            int activatedCallCount = 0;
            surface.ViewActivated += (sender, e) =>
            {
                Assert.Same(surface, sender);
                Assert.Same(EventArgs.Empty, e);
                viewActivatedCallCount++;
            };
            IDesignerLoaderHost2 host = surface.Host;
            host.Activated += (sender, e) => activatedCallCount++;
            host.Activate();
            Assert.Equal(1, viewActivatedCallCount);
            Assert.Equal(0, activatedCallCount);
        }

        [WinFormsFact]
        public void DesignerHost_Activate_InvokeDisposed_Nop()
        {
            using var surface = new SubDesignSurface();
            int viewActivatedCallCount = 0;
            int activatedCallCount = 0;
            surface.ViewActivated += (sender, e) =>
            {
                Assert.Same(surface, sender);
                Assert.Same(EventArgs.Empty, e);
                viewActivatedCallCount++;
            };
            IDesignerLoaderHost2 host = surface.Host;
            surface.Dispose();
            host.Activated += (sender, e) => activatedCallCount++;
            host.Activate();
            Assert.Equal(0, viewActivatedCallCount);
            Assert.Equal(0, activatedCallCount);
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
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(DesignerCommandSet)))
                .Returns(new object());
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(IInheritanceService)))
                .Returns(new object());
            yield return new object[] { nullMockServiceProvider.Object };

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(new object());
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(DesignerCommandSet)))
                .Returns(new object());
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(IInheritanceService)))
                .Returns(new object());
            yield return new object[] { invalidMockServiceProvider.Object };
        }

        public static IEnumerable<object[]> Add_ComponentParentProvider_TestData()
        {
            foreach (object[] testData in Add_InvalidNameCreationServiceParentProvider_TestData())
            {
                yield return new object[] { testData[0], string.Empty };
            }

            foreach (string name in new string[] { null, string.Empty, "name" })
            {
                var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Strict);
                mockNameCreationService
                    .Setup(s => s.CreateName(It.IsAny<IContainer>(), It.IsAny<Type>()))
                    .Returns(name);
                var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
                mockServiceProvider
                    .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                    .Returns(null);
                mockServiceProvider
                    .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                    .Returns(null);
                mockServiceProvider
                    .Setup(p => p.GetService(typeof(ContainerFilterService)))
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
                yield return new object[] { mockServiceProvider.Object, name };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_ComponentParentProvider_TestData))]
        public void DesignerHost_Add_ComponentWithRootDesigner_Success(IServiceProvider parentProvider, string expectedName)
        {
            using var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            using var component1 = new RootDesignerComponent();
            using var component2 = new RootDesignerComponent();
            using var component3 = new DesignerComponent();
            using var component4 = new Component();

            host.Container.Add(component1);
            Assert.Same(component1, Assert.Single(host.Container.Components));
            Assert.Same(expectedName, host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(host, component1.Container);
            Assert.Same(component1, component1.Site.Component);
            Assert.Same(host, component1.Site.Container);
            Assert.True(component1.Site.DesignMode);
            Assert.Equal(expectedName, component1.Site.Name);

            // Add different - root designer.
            host.Container.Add(component2);
            Assert.Equal(new IComponent[] { component1, component2 }, host.Container.Components.Cast<IComponent>());
            Assert.Same(expectedName, host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(host, component2.Container);
            Assert.Same(component2, component2.Site.Component);
            Assert.Same(host, component2.Site.Container);
            Assert.True(component2.Site.DesignMode);
            Assert.Equal(expectedName, component2.Site.Name);

            // Add different - non root designer.
            host.Container.Add(component3);
            Assert.Equal(new IComponent[] { component1, component2, component3 }, host.Container.Components.Cast<IComponent>());
            Assert.Same(expectedName, host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(host, component3.Container);
            Assert.Same(component3, component3.Site.Component);
            Assert.Same(host, component3.Site.Container);
            Assert.True(component3.Site.DesignMode);
            Assert.Equal(expectedName, component3.Site.Name);

            // Add different - non designer.
            host.Container.Add(component4);
            Assert.Equal(new IComponent[] { component1, component2, component3, component4 }, host.Container.Components.Cast<IComponent>());
            Assert.Same(expectedName, host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(host, component4.Container);
            Assert.Same(component4, component4.Site.Component);
            Assert.Same(host, component4.Site.Container);
            Assert.True(component4.Site.DesignMode);
            Assert.Equal(expectedName, component4.Site.Name);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_InvalidNameCreationServiceParentProvider_TestData))]
        public void DesignerHost_Add_ComponentStringWithRootDesigner_Success(IServiceProvider parentProvider)
        {
            using var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            using var component1 = new RootDesignerComponent();
            using var component2 = new RootDesignerComponent();
            using var component3 = new DesignerComponent();
            using var component4 = new Component();

            host.Container.Add(component1, "name1");
            Assert.Same(component1, Assert.Single(host.Container.Components));
            Assert.Equal("name1", host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(host, component1.Container);
            Assert.Same(component1, component1.Site.Component);
            Assert.Same(host, component1.Site.Container);
            Assert.True(component1.Site.DesignMode);
            Assert.Equal("name1", component1.Site.Name);

            // Add different - root designer.
            host.Container.Add(component2, "name2");
            Assert.Equal(new IComponent[] { component1, component2 }, host.Container.Components.Cast<IComponent>());
            Assert.Same(host, component2.Container);
            Assert.Equal("name1", host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(component2, component2.Site.Component);
            Assert.Same(host, component2.Site.Container);
            Assert.True(component2.Site.DesignMode);
            Assert.Equal("name2", component2.Site.Name);

            // Add different - non root designer.
            host.Container.Add(component3, string.Empty);
            Assert.Equal(new IComponent[] { component1, component2, component3 }, host.Container.Components.Cast<IComponent>());
            Assert.Same(host, component3.Container);
            Assert.Equal("name1", host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(component3, component3.Site.Component);
            Assert.Same(host, component3.Site.Container);
            Assert.True(component3.Site.DesignMode);
            Assert.Empty(component3.Site.Name);

            // Add different - non designer.
            host.Container.Add(component4, null);
            Assert.Equal(new IComponent[] { component1, component2, component3, component4 }, host.Container.Components.Cast<IComponent>());
            Assert.Same(host, component4.Container);
            Assert.Equal("name1", host.RootComponentClassName);
            Assert.Same(component1, host.RootComponent);
            Assert.Same(component4, component4.Site.Component);
            Assert.Same(host, component4.Site.Container);
            Assert.True(component4.Site.DesignMode);
            Assert.Empty(component4.Site.Name);
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

        [WinFormsTheory]
        [MemberData(nameof(Add_IExtenderProviderServiceWithoutDefault_TestData))]
        public void DesignerHost_Add_IExtenderProviderServiceWithoutDefault_Success(Component component, int expectedCallCount)
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
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IExtenderProviderService)))
                .Returns(mockExtenderProviderService.Object)
                .Verifiable();

            using var surface = new SubDesignSurface(mockServiceProvider.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            IDesignerLoaderHost2 host = surface.Host;

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Empty(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount));
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount));

            // Add again.
            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Empty(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount));
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount));

            // Add again with name.
            host.Container.Add(component, "name");
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Equal("name", component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedCallCount));
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedCallCount));
        }

        public static IEnumerable<object[]> Add_IExtenderProviderServiceWithDefault_TestData()
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

        [WinFormsTheory]
        [MemberData(nameof(Add_IExtenderProviderServiceWithDefault_TestData))]
        public void DesignerHost_Add_IExtenderProviderServiceWithDefault_DoesNotCallGetService(Component component)
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
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IExtenderProviderService)))
                .Returns(mockExtenderProviderService.Object)
                .Verifiable();

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Empty(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Never());

            // Add again.
            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Empty(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Never());

            // Add again with name.
            host.Container.Add(component, "name");
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Equal("name", component.Site.Name);
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
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
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
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(IExtenderProviderService)))
                .Returns(new object())
                .Verifiable();
            yield return new object[] { invalidMockServiceProvider };
        }

        [WinFormsTheory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void DesignerHost_Add_InvalidIExtenderProviderServiceWithoutDefault_CallsParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            using var surface = new SubDesignSurface(mockParentProvider?.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootExtenderProviderDesignerComponent();

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());
        }

        [WinFormsTheory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void DesignerHost_Add_InvalidIExtenderProviderServiceWithDefault_DoesNotCallParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            using var surface = new SubDesignSurface(mockParentProvider?.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootExtenderProviderDesignerComponent();

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        }

        [WinFormsFact]
        public void DesignerHost_Add_SameComponent_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Empty(component.Site.Name);

            // Add again.
            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Empty(component.Site.Name);

            // Add again with name.
            host.Container.Add(component, "name");
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Equal("name", component.Site.Name);
        }

        [WinFormsFact]
        public void DesignerHost_Add_ComponentWithNameCreationServiceWithoutName_CallsCreateName()
        {
            var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Strict);
            mockNameCreationService
                .Setup(s => s.CreateName(It.IsAny<IContainer>(), It.IsAny<Type>()))
                .Returns("name")
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(mockNameCreationService.Object);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();

            host.Container.Add(component);
            Assert.Equal("name", component.Site.Name);
            mockNameCreationService.Verify(s => s.CreateName(host.Container, typeof(RootDesignerComponent)), Times.Once());

            host.Container.Add(component, null);
            Assert.Equal("name", component.Site.Name);
            mockNameCreationService.Verify(s => s.CreateName(host.Container, typeof(RootDesignerComponent)), Times.Once());
        }

        [WinFormsFact]
        public void DesignerHost_Add_ComponentWithNameCreationServiceWithCustomReflectionType_CallsCreateName()
        {
            using var component = new CustomTypeDescriptionProviderComponent();
            var mockCustomTypeDescriptor = new Mock<ICustomTypeDescriptor>(MockBehavior.Strict);
            mockCustomTypeDescriptor
                .Setup(d => d.GetAttributes())
                .Returns(TypeDescriptor.GetAttributes(typeof(CustomTypeDescriptionProviderComponent)));
            var mockProvider = new Mock<TypeDescriptionProvider>(MockBehavior.Strict);
            mockProvider
                .Setup(p => p.GetReflectionType(typeof(CustomTypeDescriptionProviderComponent), component))
                .Returns(typeof(RootDesignerComponent))
                .Verifiable();
            mockProvider
                .Setup(p => p.GetTypeDescriptor(typeof(CustomTypeDescriptionProviderComponent), component))
                .Returns(mockCustomTypeDescriptor.Object);
            mockProvider
                .Setup(p => p.GetCache(component))
                .CallBase();
            mockProvider
                .Setup(p => p.GetExtendedTypeDescriptor(component))
                .CallBase();
            TypeDescriptor.AddProvider(mockProvider.Object, component);

            var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Strict);
            mockNameCreationService
                .Setup(s => s.CreateName(It.IsAny<IContainer>(), It.IsAny<Type>()))
                .Returns("name")
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(mockNameCreationService.Object);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;

            host.Container.Add(component);
            Assert.Equal("name", component.Site.Name);
            mockNameCreationService.Verify(s => s.CreateName(host.Container, typeof(RootDesignerComponent)), Times.Once());

            host.Container.Add(component, null);
            Assert.Equal("name", component.Site.Name);
            mockNameCreationService.Verify(s => s.CreateName(host.Container, typeof(RootDesignerComponent)), Times.Once());
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void DesignerHost_Add_ComponentWithNameCreationServiceWithName_CallsValidateName(string name)
        {
            var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Strict);
            mockNameCreationService
                .Setup(s => s.ValidateName(It.IsAny<string>()))
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(mockNameCreationService.Object)
                .Verifiable();

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component1 = new RootDesignerComponent();
            host.Container.Add(component1, name);
            Assert.Same(name, component1.Site.Name);
            mockNameCreationService.Verify(s => s.ValidateName(name), Times.Once());
            mockServiceProvider.Verify(p => p.GetService(typeof(INameCreationService)), Times.Once());

            // Add another.
            using var component2 = new DesignerComponent();
            host.Container.Add(component2, "name2");
            Assert.Equal("name2", component2.Site.Name);
            mockNameCreationService.Verify(s => s.ValidateName("name2"), Times.Once());
            mockServiceProvider.Verify(p => p.GetService(typeof(INameCreationService)), Times.Exactly(2));
        }

        [WinFormsFact]
        public void DesignerHost_Add_ComponentWithTypeDescriptionProviderServiceWithoutProjectTargetFrameworkAttribute_AddsProvider()
        {
            ICustomTypeDescriptor descriptor = TypeDescriptor.GetProvider(typeof(RootDesignerComponent)).GetTypeDescriptor(typeof(RootDesignerComponent));
            var mockTypeDescriptionProvider = new Mock<TypeDescriptionProvider>(MockBehavior.Strict);
            mockTypeDescriptionProvider
                .Setup(p => p.IsSupportedType(typeof(int)))
                .Returns(false)
                .Verifiable();
            mockTypeDescriptionProvider
                .Setup(p => p.GetTypeDescriptor(It.IsAny<Type>(), It.IsAny<object>()))
                .Returns(descriptor);
            mockTypeDescriptionProvider
                .Setup(p => p.GetCache(It.IsAny<object>()))
                .CallBase();
            mockTypeDescriptionProvider
                .Setup(p => p.GetExtendedTypeDescriptor(It.IsAny<object>()))
                .CallBase();
            var mockTypeDescriptionProviderService = new Mock<TypeDescriptionProviderService>(MockBehavior.Strict);
            mockTypeDescriptionProviderService
                .Setup(s => s.GetProvider(It.IsAny<object>()))
                .Returns(mockTypeDescriptionProvider.Object)
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(mockTypeDescriptionProviderService.Object)
                .Verifiable();
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
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

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component1 = new RootDesignerComponent();
            host.Container.Add(component1, "name1");
            Assert.Equal("name1", component1.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Once());
            mockTypeDescriptionProviderService.Verify(s => s.GetProvider(component1), Times.Once());

            // Make sure we added the TypeDescriptionProvider.
            Assert.False(TypeDescriptor.GetProvider(component1).IsSupportedType(typeof(int)));
            mockTypeDescriptionProvider.Verify(p => p.IsSupportedType(typeof(int)), Times.Once());

            // Add again.
            using var component2 = new DesignerComponent();
            host.Container.Add(component2, "name2");
            Assert.Equal("name2", component2.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Once());
            mockTypeDescriptionProviderService.Verify(s => s.GetProvider(component2), Times.Once());

            // Make sure we added the TypeDescriptionProvider.
            Assert.False(TypeDescriptor.GetProvider(component2).IsSupportedType(typeof(int)));
            mockTypeDescriptionProvider.Verify(p => p.IsSupportedType(typeof(int)), Times.Exactly(2));
        }

        [WinFormsFact]
        public void DesignerHost_Add_ComponentWithNullTypeDescriptionProviderService_Success()
        {
            var mockTypeDescriptionProviderService = new Mock<TypeDescriptionProviderService>(MockBehavior.Strict);
            mockTypeDescriptionProviderService
                .Setup(s => s.GetProvider(It.IsAny<object>()))
                .Returns<TypeDescriptionProvider>(null)
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(mockTypeDescriptionProviderService.Object)
                .Verifiable();
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component, "name1");
            Assert.Equal("name1", component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Once());
            mockTypeDescriptionProviderService.Verify(s => s.GetProvider(component), Times.Once());
        }

        [WinFormsFact]
        public void DesignerHost_Add_ComponentWithTypeDescriptionProviderServiceWithProjectTargetFrameworkAttribute_DoesNotAddProvider()
        {
            using var component = new RootDesignerComponent();
            ICustomTypeDescriptor descriptor = TypeDescriptor.GetProvider(typeof(RootDesignerComponent)).GetTypeDescriptor(typeof(RootDesignerComponent));
            var mockTypeDescriptionProvider = new Mock<TypeDescriptionProvider>(MockBehavior.Strict);
            mockTypeDescriptionProvider
                .Setup(p => p.GetTypeDescriptor(It.IsAny<Type>(), It.IsAny<object>()))
                .Returns(descriptor);
            mockTypeDescriptionProvider
                .Setup(p => p.GetCache(component))
                .CallBase();
            mockTypeDescriptionProvider
                .Setup(p => p.GetExtendedTypeDescriptor(It.IsAny<object>()))
                .CallBase();
            mockTypeDescriptionProvider
                .Setup(p => p.GetReflectionType(typeof(object), null))
                .Returns(typeof(ClassWithProjectTargetFrameworkAttribute));
            TypeDescriptor.AddProvider(mockTypeDescriptionProvider.Object, component);

            var mockTypeDescriptionProviderService = new Mock<TypeDescriptionProviderService>(MockBehavior.Strict);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(mockTypeDescriptionProviderService.Object)
                .Verifiable();
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            host.Container.Add(component, "name1");
            Assert.Equal("name1", component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Once());
            mockTypeDescriptionProviderService.Verify(s => s.GetProvider(component), Times.Never());
        }

        [WinFormsFact(Skip = "Unstable test, see: https://github.com/dotnet/winforms/issues/1460")]
        public void DesignerHost_Add_DuringUnload_ThrowsException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            surface.BeginLoad(typeof(RootDesignerComponent));

            using var component = new DesignerComponent();
            host.Container.Add(component);
            int callCount = 0;
            component.Disposed += (sender, e) =>
            {
                using var newComponent = new DesignerComponent();
                Assert.Throws<Exception>(() => host.Container.Add(newComponent));
                callCount++;
            };
            surface.Dispose();
            Assert.Equal(1, callCount);
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.False(host.Loading);
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceGetKey_NoDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Null(service.GetKey(null));
            Assert.Null(service.GetKey(new object()));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceGetKey_NoSuchKeyWithDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            var key1 = new object();
            var value1 = new object();
            service.SetValue(key1, value1);
            Assert.Same(key1, service.GetKey(value1));
            Assert.Same(value1, service.GetValue(key1));

            Assert.Null(service.GetKey(null));
            Assert.Null(service.GetKey(new object()));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceGetValue_NoDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Null(service.GetValue(new object()));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceGetValue_NoSuchValueWithDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            var key1 = new object();
            var value1 = new object();
            service.SetValue(key1, value1);
            Assert.Same(key1, service.GetKey(value1));
            Assert.Same(value1, service.GetValue(key1));

            Assert.Null(service.GetValue(new object()));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceGetValue_NullValueNoDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Throws<ArgumentNullException>("key", () => service.GetValue(null));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceGetValue_NullValueWithDictionary_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            var key1 = new object();
            var value1 = new object();
            service.SetValue(key1, value1);
            Assert.Same(key1, service.GetKey(value1));
            Assert.Same(value1, service.GetValue(key1));

            Assert.Throws<ArgumentNullException>("key", () => service.GetValue(null));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceSetValue_Invoke_GetKeyValueReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);

            var key1 = new object();
            var value1 = new object();
            service.SetValue(key1, value1);
            Assert.Same(key1, service.GetKey(value1));
            Assert.Same(value1, service.GetValue(key1));

            // Add another.
            var key2 = new object();
            var value2 = new object();
            service.SetValue(key2, value2);
            Assert.Same(key2, service.GetKey(value2));
            Assert.Same(value2, service.GetValue(key2));

            // Add same.
            var value3 = new object();
            service.SetValue(key1, value3);
            Assert.Same(key1, service.GetKey(value3));
            Assert.Null(service.GetKey(value1));
            Assert.Same(value3, service.GetValue(key1));

            // Add null value.
            service.SetValue(key1, null);
            Assert.Null(service.GetKey(null));
            Assert.Null(service.GetKey(value3));
            Assert.Null(service.GetValue(key1));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIDictionaryServiceSetValue_NullKey_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Throws<ArgumentNullException>("key", () => service.SetValue(null, new object()));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIServiceContainerAddService_InvokeObject_ReturnsExpected()
        {
            var service = new object();
            var otherService = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(object)))
                .Returns(otherService);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IServiceContainer container = Assert.IsAssignableFrom<IServiceContainer>(component.Site);
            container.AddService(typeof(object), service);
            Assert.Same(service, container.GetService(typeof(object)));
            Assert.Same(otherService, surface.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIServiceContainerAddService_InvokeObjectPromote_ReturnsExpected()
        {
            var service = new object();
            var otherService = new object();
            var otherContainer = new ServiceContainer();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(object)))
                .Returns(otherService);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IServiceContainer)))
                .Returns(otherContainer);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IServiceContainer container = Assert.IsAssignableFrom<IServiceContainer>(component.Site);
            container.AddService(typeof(object), service, true);
            Assert.Same(otherService, container.GetService(typeof(object)));
            Assert.Same(otherService, surface.GetService(typeof(object)));
            Assert.Same(service, otherContainer.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIServiceContainerAddService_InvokeObjectNoPromote_ReturnsExpected()
        {
            var service = new object();
            var otherService = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(object)))
                .Returns(otherService);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IServiceContainer container = Assert.IsAssignableFrom<IServiceContainer>(component.Site);
            container.AddService(typeof(object), service, false);
            Assert.Same(service, container.GetService(typeof(object)));
            Assert.Same(otherService, surface.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIServiceContainerAddService_InvokeServiceCreatorCallback_ReturnsExpected()
        {
            var service = new object();
            ServiceCreatorCallback callback = (c, serviceType) =>
            {
                Assert.Same(typeof(object), serviceType);
                return service;
            };
            var otherService = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(object)))
                .Returns(otherService);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IServiceContainer container = Assert.IsAssignableFrom<IServiceContainer>(component.Site);
            container.AddService(typeof(object), callback);
            Assert.Same(service, container.GetService(typeof(object)));
            Assert.Same(otherService, surface.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIServiceContainerAddService_InvokeServiceCreatorCallbackPromote_ReturnsExpected()
        {
            var service = new object();
            ServiceCreatorCallback callback = (c, serviceType) =>
            {
                Assert.Same(typeof(object), serviceType);
                return service;
            };
            var otherService = new object();
            var otherContainer = new ServiceContainer();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(object)))
                .Returns(otherService);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IServiceContainer)))
                .Returns(otherContainer);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IServiceContainer container = Assert.IsAssignableFrom<IServiceContainer>(component.Site);
            container.AddService(typeof(object), callback, true);
            Assert.Same(otherService, container.GetService(typeof(object)));
            Assert.Same(otherService, surface.GetService(typeof(object)));
            Assert.Same(service, otherContainer.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentIServiceContainerAddService_InvokeServiceCreatorCallbackNoPromote_ReturnsExpected()
        {
            var service = new object();
            ServiceCreatorCallback callback = (c, serviceType) =>
            {
                Assert.Same(typeof(object), serviceType);
                return service;
            };
            var otherService = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(object)))
                .Returns(otherService);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IServiceContainer container = Assert.IsAssignableFrom<IServiceContainer>(component.Site);
            container.AddService(typeof(object), callback, false);
            Assert.Same(service, container.GetService(typeof(object)));
            Assert.Same(otherService, surface.GetService(typeof(object)));
        }

        public static IEnumerable<object[]> AddComponentISiteName_Set_TestData()
        {
            foreach (object[] testData in Add_InvalidNameCreationServiceParentProvider_TestData())
            {
                foreach (string oldName in new string[] { null, string.Empty, "oldName" })
                {
                    yield return new object[] { testData[0], oldName, null, string.Empty };
                    yield return new object[] { testData[0], oldName, string.Empty, string.Empty };
                    yield return new object[] { testData[0], oldName, "name", "name" };
                    yield return new object[] { testData[0], oldName, "ldName", "ldName" };
                    yield return new object[] { testData[0], oldName, "ldName", "ldName" };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(AddComponentISiteName_Set_TestData))]
        public void DesignerHost_AddComponentISiteName_SetRootComponent_GetReturnsExpected(IServiceProvider parentProvider, string oldName, string value, string expectedName)
        {
            using var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component, oldName);
            component.Site.Name = value;
            Assert.Same(expectedName, component.Site.Name);
            Assert.Same(expectedName, host.RootComponentClassName);

            // Set same.
            component.Site.Name = value;
            Assert.Same(expectedName, component.Site.Name);
            Assert.Same(expectedName, host.RootComponentClassName);
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteName_SetDifferentCase_GetReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component, "name");
            component.Site.Name = "NAME";
            Assert.Equal("NAME", component.Site.Name);

            // Set same.
            component.Site.Name = "NAME";
            Assert.Equal("NAME", component.Site.Name);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DesignerHost_AddComponentISiteName_SetWithMultipleComponents_GetReturnsExpected(string value)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component1 = new RootDesignerComponent();
            using var component2 = new RootDesignerComponent();
            using var component3 = new RootDesignerComponent();
            host.Container.Add(component1);
            host.Container.Add(component2, null);
            host.Container.Add(component3, "name3");

            component1.Site.Name = value;
            Assert.Same(value ?? string.Empty, component1.Site.Name);

            // Set same.
            component1.Site.Name = value;
            Assert.Same(value ?? string.Empty, component1.Site.Name);
        }

        public static IEnumerable<object[]> AddComponentISiteName_SetWithNamespaceInRootComponentClassName_TestData()
        {
            yield return new object[] { string.Empty, "oldName", null, string.Empty, string.Empty };
            yield return new object[] { string.Empty, "oldName", string.Empty, string.Empty, string.Empty };
            yield return new object[] { string.Empty, "oldName", "newName", "newName", "newName" };

            yield return new object[] { "oldName", "oldName", null, string.Empty, string.Empty };
            yield return new object[] { "oldName", "oldName", string.Empty, string.Empty, string.Empty };
            yield return new object[] { "oldName", "oldName", "newName", "newName", "newName" };
            yield return new object[] { "oldName", "oldName", "oldName", "oldName", "oldName" };

            yield return new object[] { "namespace.oldName", "oldName", null, string.Empty, "namespace." };
            yield return new object[] { "namespace.oldName", "oldName", string.Empty, string.Empty, "namespace." };
            yield return new object[] { "namespace.oldName", "oldName", "newName", "newName", "namespace.newName" };
            yield return new object[] { "namespace.oldName", "oldName", "oldName", "oldName", "namespace.oldName" };
            yield return new object[] { "namespace.oldName", "ldName", "ldName", "ldName", "namespace.oldName" };

            yield return new object[] { "namespace.oldName", "namespace.oldName", null, string.Empty, string.Empty };
            yield return new object[] { "namespace.oldName", "namespace.oldName", string.Empty, string.Empty, string.Empty };
            yield return new object[] { "namespace.oldName", "namespace.oldName", "newName", "newName", "newName" };
            yield return new object[] { "namespace.oldName", "namespace.oldName", "oldName", "oldName", "oldName" };
            yield return new object[] { "namespace.oldName", "namespace.oldName", "ldName", "ldName", "ldName" };

            yield return new object[] { "namespace.oldName", "name", null, string.Empty, string.Empty };
            yield return new object[] { "namespace.oldName", "name", string.Empty, string.Empty, string.Empty };
            yield return new object[] { "namespace.oldName", "name", "newName", "newName", "newName" };
            yield return new object[] { "namespace.oldName", "name", "oldName", "oldName", "oldName" };
        }

        [WinFormsTheory]
        [MemberData(nameof(AddComponentISiteName_SetWithNamespaceInRootComponentClassName_TestData))]
        public void DesignerHost_AddComponentISiteName_SetWithNamespaceInRootComponentClassName_GetReturnsExpected(string oldRootComponentClassName, string oldName, string value, string expectedName, string expectedRootComponentClassName)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();

            host.EndLoad(oldRootComponentClassName, true, null);
            Assert.Equal(oldRootComponentClassName, host.RootComponentClassName);
            Assert.Null(host.RootComponent);

            host.Container.Add(component, oldName);
            Assert.Equal(oldName, component.Site.Name);
            Assert.Equal(oldRootComponentClassName, host.RootComponentClassName);
            Assert.Same(component, host.RootComponent);

            component.Site.Name = value;
            Assert.Equal(expectedName, component.Site.Name);
            Assert.Equal(expectedRootComponentClassName, host.RootComponentClassName);
            Assert.Same(component, host.RootComponent);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DesignerHost_AddComponentISiteName_SetNameWithComponentRename_CallsHandler(string value)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);
            using var component = new RootDesignerComponent();

            int callCount = 0;
            ComponentRenameEventHandler handler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(component, e.Component);
                Assert.Equal("oldName", e.OldName);
                Assert.Same(value ?? string.Empty, e.NewName);
                callCount++;
            };
            changeService.ComponentRename += handler;
            host.Container.Add(component, "oldName");

            component.Site.Name = value;
            Assert.Same(value ?? string.Empty, component.Site.Name);
            Assert.Equal(1, callCount);

            // Set same.
            component.Site.Name = value;
            Assert.Same(value ?? string.Empty, component.Site.Name);
            Assert.Equal(1, callCount);

            // Remove handler.
            changeService.ComponentRename -= handler;
            component.Site.Name = "name";
            Assert.Equal("name", component.Site.Name);
            Assert.Equal(1, callCount);
        }

        [WinFormsTheory]
        [InlineData(null, "", 1)]
        [InlineData("", "", 1)]
        [InlineData("newName", "newName", 1)]
        [InlineData("OLDNAME", "OLDNAME", 0)]
        public void DesignerHost_AddComponentISite_SetNameWithINameCreateService_CallsValidateName(string value, string expectedName, int expectedCallCount)
        {
            var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Strict);
            mockNameCreationService
                .Setup(s => s.ValidateName("oldName"))
                .Verifiable();
            mockNameCreationService
                .Setup(s => s.ValidateName(expectedName))
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(mockNameCreationService.Object);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component, "oldName");
            Assert.Equal("oldName", component.Site.Name);
            mockNameCreationService.Verify(s => s.ValidateName("oldName"), Times.Once());

            component.Site.Name = value;
            Assert.Equal(expectedName, component.Site.Name);
            mockNameCreationService.Verify(s => s.ValidateName(expectedName), Times.Exactly(expectedCallCount));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteName_SetSameAsOtherComponent_GetReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component1 = new RootDesignerComponent();
            using var component2 = new RootDesignerComponent();
            host.Container.Add(component1, "name1");
            host.Container.Add(component2, "name2");
            Assert.Throws<Exception>(() => component1.Site.Name = "name2");
            Assert.Throws<Exception>(() => component1.Site.Name = "NAME2");
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetService_Invoke_ReturnsExpected()
        {
            var service = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(int)))
                .Returns(service);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(service, component.Site.GetService(typeof(int)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetService_InvokeWithNestedContainer_ReturnsService()
        {
            var service = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(int)))
                .Returns(service);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            INestedContainer nestedContainer = Assert.IsAssignableFrom<INestedContainer>(component.Site.GetService(typeof(INestedContainer)));
            Assert.Same(service, component.Site.GetService(typeof(int)));
            Assert.Same(component.Site, component.Site.GetService(typeof(IDictionaryService)));
            Assert.Same(nestedContainer, component.Site.GetService(typeof(INestedContainer)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetService_INestedContainer_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            INestedContainer container = Assert.IsAssignableFrom<INestedContainer>(component.Site.GetService(typeof(INestedContainer)));
            Assert.Same(container, component.Site.GetService(typeof(INestedContainer)));
            Assert.Empty(container.Components);
            Assert.Same(component, container.Owner);
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetServiceINestedContainerGetService_Invoke_ReturnsExpected()
        {
            var service = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
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
                .Setup(p => p.GetService(typeof(int)))
                .Returns(service);

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            NestedContainer container = Assert.IsAssignableFrom<NestedContainer>(component.Site.GetService(typeof(INestedContainer)));
            var nestedComponent = new Component();
            container.Add(nestedComponent);
            Assert.Same(service, nestedComponent.Site.GetService(typeof(int)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetService_IDictionaryService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(component.Site, component.Site.GetService(typeof(IDictionaryService)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetService_IServiceContainerReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(surface.ServiceContainer, component.Site.GetService(typeof(IServiceContainer)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetService_IContainerReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(host, component.Site.GetService(typeof(IContainer)));
        }

        [WinFormsFact]
        public void DesignerHost_AddComponentISiteGetService_NullServiceType_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Throws<ArgumentNullException>("service", () => component.Site.GetService(null));
        }

        [WinFormsFact]
        public void DesignerHost_Add_IComponentWithComponentAddingAndAdded_CallsHandler()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            using var component = new RootDesignerComponent();
            int componentAddingCallCount = 0;
            ComponentEventHandler componentAddingHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(component, e.Component);
                componentAddingCallCount++;
            };
            changeService.ComponentAdding += componentAddingHandler;
            int componentAddedCallCount = 0;
            ComponentEventHandler componentAddedHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(component, e.Component);
                Assert.True(componentAddedCallCount < componentAddingCallCount);
                componentAddedCallCount++;
            };
            changeService.ComponentAdded += componentAddedHandler;

            // With handler.
            host.Container.Add(component);
            Assert.Same(host.Container, component.Container);
            Assert.Equal(1, componentAddingCallCount);
            Assert.Equal(1, componentAddedCallCount);

            // Add again.
            host.Container.Add(component);
            Assert.Same(host.Container, component.Container);
            Assert.Equal(1, componentAddingCallCount);
            Assert.Equal(1, componentAddedCallCount);

            // Remove handler.
            changeService.ComponentAdding -= componentAddingHandler;
            changeService.ComponentAdded -= componentAddedHandler;
            host.Container.Add(component);
            Assert.Same(host.Container, component.Container);
            Assert.Equal(1, componentAddingCallCount);
            Assert.Equal(1, componentAddedCallCount);
        }

        [WinFormsFact]
        public void DesignerHost_Add_NullComponent_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Throws<ArgumentNullException>("component", () => host.Container.Add(null));
            Assert.Throws<ArgumentNullException>("component", () => host.Container.Add(null, "name"));
        }

        public static IEnumerable<object[]> Add_NoRootDesigner_TestData()
        {
            yield return new object[] { new Component() };
            yield return new object[] { new DesignerComponent() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_NoRootDesigner_TestData))]
        public void DesignerHost_Add_NoRootDesigner_ThrowsException(IComponent component)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Throws<Exception>(() => host.Container.Add(component));
            Assert.Throws<Exception>(() => host.Container.Add(component, "name"));
            Assert.Empty(host.Container.Components);
        }

        [WinFormsFact]
        public void DesignerHost_Add_CyclicRootDesigner_ThrowsException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component, component.GetType().FullName);
            Assert.Equal(component.GetType().FullName, host.RootComponentClassName);
            Assert.Throws<Exception>(() => host.Container.Add(component));
            Assert.Throws<Exception>(() => host.Container.Add(new RootDesignerComponent(), host.RootComponentClassName));
        }

        [WinFormsFact]
        public void DesignerHost_Add_NonInitializingRootDesigner_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new NonInitializingDesignerComponent();
            Assert.Throws<InvalidOperationException>(() => host.Container.Add(component));
            Assert.Throws<InvalidOperationException>(() => host.Container.Add(component, "name"));
        }

        [WinFormsFact]
        public void DesignerHost_Add_ThrowingInitializingRootDesigner_RethrowsException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new ThrowingInitializingDesignerComponent();
            Assert.Throws<DivideByZeroException>(() => host.Container.Add(component));
            Assert.Null(component.Container);
            Assert.Null(component.Site);

            Assert.Throws<DivideByZeroException>(() => host.Container.Add(component, "name"));
            Assert.Null(component.Container);
            Assert.Null(component.Site);
        }

        [WinFormsFact]
        public void DesignerHost_Add_CheckoutExceptionThrowingInitializingRootDesigner_RethrowsException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new CheckoutExceptionThrowingInitializingDesignerComponent();
            // CheckoutException does not bubble up in xunit.
            bool threwCheckoutException = false;
            try
            {
                host.Container.Add(component);
            }
            catch (CheckoutException)
            {
                threwCheckoutException = true;
            }
            Assert.True(threwCheckoutException);
            Assert.Same(host.Container, component.Container);
            Assert.Empty(component.Site.Name);

            host.Container.Add(component, "name");
            Assert.Same(host.Container, component.Container);
            Assert.Equal("name", component.Site.Name);
        }

        [WinFormsFact]
        public void DesignerHost_AddService_InvokeObject_GetServiceReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            object service = new object();
            host.AddService(typeof(object), service);
            Assert.Same(service, surface.ServiceContainer.GetService(typeof(object)));
            Assert.Same(service, surface.GetService(typeof(object)));
            Assert.Same(service, host.GetService(typeof(object)));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerHost_AddService_InvokeObjectBool_GetServiceReturnsExpected(bool promote)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            object service = new object();
            host.AddService(typeof(object), service, promote);
            Assert.Same(service, surface.ServiceContainer.GetService(typeof(object)));
            Assert.Same(service, surface.GetService(typeof(object)));
            Assert.Same(service, host.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_AddService_InvokeCallback_GetServiceReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            object service = new object();
            ServiceCreatorCallback callback = (container, serviceType) => service;
            host.AddService(typeof(object), callback);
            Assert.Same(service, surface.ServiceContainer.GetService(typeof(object)));
            Assert.Same(service, surface.GetService(typeof(object)));
            Assert.Same(service, host.GetService(typeof(object)));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerHost_AddService_InvokeObjectCallback_GetServiceReturnsExpected(bool promote)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            object service = new object();
            ServiceCreatorCallback callback = (container, serviceType) => service;
            host.AddService(typeof(object), callback, promote);
            Assert.Same(service, surface.ServiceContainer.GetService(typeof(object)));
            Assert.Same(service, surface.GetService(typeof(object)));
            Assert.Same(service, host.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_AddService_InvokeDisposed_ThrowsObjectDisposedException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            surface.Dispose();
            ServiceCreatorCallback callback = (container, service) => new object();
            Assert.Throws<ObjectDisposedException>(() => host.AddService(typeof(object), new object()));
            Assert.Throws<ObjectDisposedException>(() => host.AddService(typeof(object), new object(), true));
            Assert.Throws<ObjectDisposedException>(() => host.AddService(typeof(object), new object(), false));
            Assert.Throws<ObjectDisposedException>(() => host.AddService(typeof(object), callback));
            Assert.Throws<ObjectDisposedException>(() => host.AddService(typeof(object), callback, true));
            Assert.Throws<ObjectDisposedException>(() => host.AddService(typeof(object), callback, false));
        }

        [WinFormsFact]
        public void DesignerHost_Dispose_Invoke_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IDisposable disposable = Assert.IsAssignableFrom<IDisposable>(host);
            Assert.Throws<InvalidOperationException>(() => disposable.Dispose());
        }

        [WinFormsFact(Skip = "Unstable test, see: https://github.com/dotnet/winforms/issues/1460")]
        public void DesignerHost_Add_DesignerDisposeThrowsDuringUnloadingDispose_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            surface.BeginLoad(typeof(RootDesignerComponent));

            using var component = new ThrowingDesignerDisposeComponent();
            host.Container.Add(component);
            Assert.Throws<InvalidOperationException>(() => surface.Dispose());
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.False(host.Loading);
        }

        [WinFormsFact(Skip = "Unstable test, see: https://github.com/dotnet/winforms/issues/1151")]
        public void DesignerHost_Add_ComponentDisposeThrowsDuringUnloadingDispose_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            surface.BeginLoad(typeof(RootDesignerComponent));

            using var component = new ThrowingDisposeDesignerComponent();
            host.Container.Add(component);
            Assert.Throws<InvalidOperationException>(() => surface.Dispose());
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.False(host.Loading);
        }

        [WinFormsFact(Skip = "Unstable test, see: https://github.com/dotnet/winforms/issues/1460")]
        public void DesignerHost_Add_RootDesignerDisposeThrowsDuringUnloadingDispose_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            surface.BeginLoad(typeof(ThrowingRootDesignerDisposeComponent));

            Assert.Throws<InvalidOperationException>(() => surface.Dispose());
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.False(host.Loading);
        }

        [WinFormsFact(Skip = "Unstable test, see: https://github.com/dotnet/winforms/issues/1460")]
        public void DesignerHost_Add_RootComponentDisposeThrowsDuringUnloadingDispose_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            surface.BeginLoad(typeof(ThrowingDisposeRootDesignerComponent));
            Assert.Throws<InvalidOperationException>(() => surface.Dispose());
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.False(host.Loading);
        }

        [WinFormsFact]
        public void DesignerHost_CreateComponent_NullComponentType_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Throws<ArgumentNullException>("componentType", () => host.CreateComponent(null));
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_Invoke_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction();
            Assert.False(transaction1.Canceled);
            Assert.False(transaction1.Committed);
            Assert.Equal("No Description Available", transaction1.Description);
            Assert.True(host.InTransaction);
            Assert.Equal("No Description Available", host.TransactionDescription);

            DesignerTransaction transaction2 = host.CreateTransaction();
            Assert.False(transaction2.Canceled);
            Assert.False(transaction2.Committed);
            Assert.Equal("No Description Available", transaction2.Description);
            Assert.True(host.InTransaction);
            Assert.Equal("No Description Available", host.TransactionDescription);

            transaction2.Cancel();
        }

        [WinFormsTheory]
        [InlineData(null, "No Description Available")]
        [InlineData("", "")]
        [InlineData("Description", "Description")]
        public void DesignerHost_CreateTransaction_InvokeWithDescription_ReturnsExpected(string description, string expectedDescription)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction(description);
            Assert.False(transaction1.Canceled);
            Assert.False(transaction1.Committed);
            Assert.Equal(expectedDescription, transaction1.Description);
            Assert.True(host.InTransaction);
            Assert.Equal(expectedDescription, host.TransactionDescription);

            DesignerTransaction transaction2 = host.CreateTransaction("CustomDescription");
            Assert.False(transaction2.Canceled);
            Assert.False(transaction2.Committed);
            Assert.Equal("CustomDescription", transaction2.Description);
            Assert.True(host.InTransaction);
            Assert.Equal("CustomDescription", host.TransactionDescription);

            transaction2.Cancel();
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_InvokeWithTransactionOpeningAndTransactionOpened_CallsHandlers()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            int openingCallCount = 0;
            EventHandler openingHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(EventArgs.Empty, e);
                openingCallCount++;
            };
            int openedCallCount = 0;
            EventHandler openedHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(EventArgs.Empty, e);
                Assert.True(openedCallCount < openingCallCount);
                openedCallCount++;
            };
            host.TransactionOpening += openingHandler;
            host.TransactionOpened += openedHandler;

            // With handler.
            DesignerTransaction transaction1 = host.CreateTransaction("Description1");
            Assert.Equal(1, openingCallCount);
            Assert.Equal(1, openedCallCount);

            // Create again.
            DesignerTransaction transaction2 = host.CreateTransaction("Description2");
            Assert.Equal(2, openingCallCount);
            Assert.Equal(2, openedCallCount);

            // Remove handler.
            host.TransactionOpening -= openingHandler;
            host.TransactionOpened -= openedHandler;
            DesignerTransaction transaction3 = host.CreateTransaction("Description2");
            Assert.Equal(2, openingCallCount);
            Assert.Equal(2, openedCallCount);

            transaction3.Cancel();
            transaction2.Cancel();
            transaction1.Cancel();
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_Cancel_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction("Description1");
            DesignerTransaction transaction2 = host.CreateTransaction("Description2");

            // Cancel first.
            transaction2.Cancel();
            Assert.False(transaction1.Canceled);
            Assert.False(transaction1.Committed);
            Assert.Equal("Description1", transaction1.Description);
            Assert.True(transaction2.Canceled);
            Assert.False(transaction2.Committed);
            Assert.Equal("Description2", transaction2.Description);
            Assert.True(host.InTransaction);
            Assert.Equal("Description1", host.TransactionDescription);

            // Cancel again.
            transaction2.Cancel();
            Assert.False(transaction1.Canceled);
            Assert.False(transaction1.Committed);
            Assert.Equal("Description1", transaction1.Description);
            Assert.True(transaction2.Canceled);
            Assert.False(transaction2.Committed);
            Assert.Equal("Description2", transaction2.Description);
            Assert.True(host.InTransaction);
            Assert.Equal("Description1", host.TransactionDescription);

            // Cancel second.
            transaction1.Cancel();
            Assert.True(transaction1.Canceled);
            Assert.False(transaction1.Committed);
            Assert.Equal("Description1", transaction1.Description);
            Assert.True(transaction2.Canceled);
            Assert.False(transaction2.Committed);
            Assert.Equal("Description2", transaction2.Description);
            Assert.False(host.InTransaction);
            Assert.Null(host.TransactionDescription);
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_CancelWithTransactionClosingAndTransactionClosed_CallsHandlers()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction("Description1");
            DesignerTransaction transaction2 = host.CreateTransaction("Description2");

            bool expectedLastTransaction = false;
            int closingCallCount = 0;
            DesignerTransactionCloseEventHandler closingHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.False(e.TransactionCommitted);
                Assert.Equal(expectedLastTransaction, e.LastTransaction);
                closingCallCount++;
            };
            int closedCallCount = 0;
            DesignerTransactionCloseEventHandler closedHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.False(e.TransactionCommitted);
                Assert.Equal(expectedLastTransaction, e.LastTransaction);
                Assert.True(closedCallCount < closingCallCount);
                closedCallCount++;
            };
            host.TransactionClosing += closingHandler;
            host.TransactionClosed += closedHandler;

            // With handler.
            transaction2.Cancel();
            Assert.False(transaction1.Canceled);
            Assert.True(transaction2.Canceled);
            Assert.Equal(1, closingCallCount);
            Assert.Equal(1, closedCallCount);

            // Cancel again.
            transaction2.Cancel();
            Assert.False(transaction1.Canceled);
            Assert.True(transaction2.Canceled);
            Assert.Equal(1, closingCallCount);
            Assert.Equal(1, closedCallCount);

            // Cancel second.
            expectedLastTransaction = true;
            transaction1.Cancel();
            Assert.True(transaction1.Canceled);
            Assert.True(transaction2.Canceled);
            Assert.Equal(2, closingCallCount);
            Assert.Equal(2, closedCallCount);

            // Remove handler.
            host.TransactionClosing -= closingHandler;
            host.TransactionClosed -= closedHandler;
            DesignerTransaction transaction3 = host.CreateTransaction("Description1");
            transaction3.Cancel();
            Assert.True(transaction3.Canceled);
            Assert.Equal(2, closingCallCount);
            Assert.Equal(2, closedCallCount);
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_CancelNested_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction("Description1");
            DesignerTransaction transaction2 = host.CreateTransaction("Description2");
            Assert.Throws<InvalidOperationException>(() => transaction1.Cancel());
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_Commit_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction("Description1");
            DesignerTransaction transaction2 = host.CreateTransaction("Description2");

            // Commit first.
            transaction2.Commit();
            Assert.False(transaction1.Canceled);
            Assert.False(transaction1.Committed);
            Assert.Equal("Description1", transaction1.Description);
            Assert.False(transaction2.Canceled);
            Assert.True(transaction2.Committed);
            Assert.Equal("Description2", transaction2.Description);
            Assert.True(host.InTransaction);
            Assert.Equal("Description1", host.TransactionDescription);

            // Commit again.
            transaction2.Commit();
            Assert.False(transaction1.Canceled);
            Assert.False(transaction1.Committed);
            Assert.Equal("Description1", transaction1.Description);
            Assert.False(transaction2.Canceled);
            Assert.True(transaction2.Committed);
            Assert.Equal("Description2", transaction2.Description);
            Assert.True(host.InTransaction);
            Assert.Equal("Description1", host.TransactionDescription);

            // Commit second.
            transaction1.Commit();
            Assert.False(transaction1.Canceled);
            Assert.True(transaction1.Committed);
            Assert.Equal("Description1", transaction1.Description);
            Assert.False(transaction2.Canceled);
            Assert.True(transaction2.Committed);
            Assert.Equal("Description2", transaction2.Description);
            Assert.False(host.InTransaction);
            Assert.Null(host.TransactionDescription);
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_CommitWithTransactionClosingAndTransactionClosed_CallsHandlers()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction("Description1");
            DesignerTransaction transaction2 = host.CreateTransaction("Description2");

            bool expectedLastTransaction = false;
            int closingCallCount = 0;
            DesignerTransactionCloseEventHandler closingHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.True(e.TransactionCommitted);
                Assert.Equal(expectedLastTransaction, e.LastTransaction);
                closingCallCount++;
            };
            int closedCallCount = 0;
            DesignerTransactionCloseEventHandler closedHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.True(e.TransactionCommitted);
                Assert.Equal(expectedLastTransaction, e.LastTransaction);
                Assert.True(closedCallCount < closingCallCount);
                closedCallCount++;
            };
            host.TransactionClosing += closingHandler;
            host.TransactionClosed += closedHandler;

            // With handler.
            transaction2.Commit();
            Assert.False(transaction1.Committed);
            Assert.True(transaction2.Committed);
            Assert.Equal(1, closingCallCount);
            Assert.Equal(1, closedCallCount);

            // Commit again.
            transaction2.Commit();
            Assert.False(transaction1.Committed);
            Assert.True(transaction2.Committed);
            Assert.Equal(1, closingCallCount);
            Assert.Equal(1, closedCallCount);

            // Commit second.
            expectedLastTransaction = true;
            transaction1.Commit();
            Assert.True(transaction1.Committed);
            Assert.True(transaction2.Committed);
            Assert.Equal(2, closingCallCount);
            Assert.Equal(2, closedCallCount);

            // Remove handler.
            host.TransactionClosing -= closingHandler;
            host.TransactionClosed -= closedHandler;
            DesignerTransaction transaction3 = host.CreateTransaction("Description1");
            transaction3.Commit();
            Assert.True(transaction3.Committed);
            Assert.Equal(2, closingCallCount);
            Assert.Equal(2, closedCallCount);
        }

        [WinFormsFact]
        public void DesignerHost_CreateTransaction_CommitNested_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            DesignerTransaction transaction1 = host.CreateTransaction("Description1");
            DesignerTransaction transaction2 = host.CreateTransaction("Description2");
            Assert.Throws<InvalidOperationException>(() => transaction1.Commit());
        }

        public static IEnumerable<object[]> DesignerHost_EndLoad_TestData()
        {
            yield return new object[] { null, true, null };
            yield return new object[] { null, true, Array.Empty<object>() };
            yield return new object[] { null, true, new object[] { new Exception() } };
            yield return new object[] { null, true, new object[] { "abc" } };
            yield return new object[] { null, true, new object[] { null } };
            yield return new object[] { null, false, null };
            yield return new object[] { null, false, Array.Empty<object>() };
            yield return new object[] { null, false, new object[] { new Exception() } };
            yield return new object[] { null, false, new object[] { "abc" } };
            yield return new object[] { null, false, new object[] { null } };
            yield return new object[] { string.Empty, true, null };
            yield return new object[] { string.Empty, true, Array.Empty<object>() };
            yield return new object[] { string.Empty, true, new object[] { new Exception() } };
            yield return new object[] { string.Empty, true, new object[] { "abc" } };
            yield return new object[] { string.Empty, true, new object[] { null } };
            yield return new object[] { string.Empty, false, null };
            yield return new object[] { string.Empty, false, Array.Empty<object>() };
            yield return new object[] { string.Empty, false, new object[] { new Exception() } };
            yield return new object[] { string.Empty, false, new object[] { "abc" } };
            yield return new object[] { string.Empty, false, new object[] { null } };
            yield return new object[] { "baseClassName", true, null };
            yield return new object[] { "baseClassName", true, new object[] { new Exception() } };
            yield return new object[] { "baseClassName", true, new object[] { "abc" } };
            yield return new object[] { "baseClassName", true, new object[] { null } };
            yield return new object[] { "baseClassName", false, null };
            yield return new object[] { "baseClassName", false, new object[] { new Exception() } };
            yield return new object[] { "baseClassName", false, new object[] { "abc" } };
            yield return new object[] { "baseClassName", false, new object[] { null } };
        }

        [WinFormsTheory]
        [MemberData(nameof(DesignerHost_EndLoad_TestData))]
        public void DesignerHost_EndLoad_NotCalledBeginLoad_Success(string baseClassName, bool successful, ICollection errorCollection)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            host.EndLoad(baseClassName, successful, errorCollection);
            Assert.False(surface.IsLoaded);
            Assert.False(host.Loading);
            Assert.Empty(surface.LoadErrors);
            Assert.Null(host.RootComponent);
            Assert.Same(baseClassName, host.RootComponentClassName);
        }

        [WinFormsFact]
        public void DesignerHost_GetComponents_Invoke_ReturnsFiltered()
        {
            var collection = new ComponentCollection(new Component[] { new Component() });
            var mockFilterService = new Mock<ContainerFilterService>(MockBehavior.Strict);
            mockFilterService
                .Setup(f => f.FilterComponents(new ComponentCollection(Array.Empty<IComponent>())))
                .Returns(collection);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(mockFilterService.Object);
            using var surface = new SubDesignSurface(mockServiceProvider.Object);
            Assert.Same(collection, surface.ComponentContainer.Components);
        }

        [WinFormsFact]
        public void DesignerHost_GetDesigner_InvokeNonEmpty_ReturnsExpected()
        {
            using var surface = new SubDesignSurface();
            IDesignerHost host = surface.Host;
            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.IsType<RootDesigner>(host.GetDesigner(component));
            Assert.Null(host.GetDesigner(new Component()));
            Assert.Null(host.GetDesigner(new RootDesignerComponent()));
        }

        [WinFormsFact]
        public void DesignerHost_GetDesigner_InvokeEmpty_ReturnsExpected()
        {
            using var surface = new SubDesignSurface();
            IDesignerHost host = surface.Host;
            Assert.Null(host.GetDesigner(new Component()));
            Assert.Null(host.GetDesigner(new RootDesignerComponent()));
        }

        [WinFormsFact]
        public void DesignerHost_GetDesigner_NullComponent_ThrowsArgumentNullException()
        {
            using var surface = new SubDesignSurface();
            IDesignerHost host = surface.Host;
            Assert.Throws<ArgumentNullException>("component", () => host.GetDesigner(null));
        }

        public static IEnumerable<object[]> GetService_InvalidLoader_TestData()
        {
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(It.IsAny<IDesignerLoaderHost>()));
            yield return new object[] { mockLoader.Object, null };

            var mockNullServiceProviderLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockNullServiceProviderLoader
                .Setup(l => l.BeginLoad(It.IsAny<IDesignerLoaderHost>()));
            mockNullServiceProviderLoader
                .As<IServiceProvider>()
                .Setup(p => p.GetService(typeof(IMultitargetHelperService)))
                .Returns(null);
            yield return new object[] { mockNullServiceProviderLoader.Object, null };

            var o = new object();
            var mockInvalidServiceProviderLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockInvalidServiceProviderLoader
                .Setup(l => l.BeginLoad(It.IsAny<IDesignerLoaderHost>()));
            mockInvalidServiceProviderLoader
                .As<IServiceProvider>()
                .Setup(p => p.GetService(typeof(IMultitargetHelperService)))
                .Returns(o);
            yield return new object[] { mockInvalidServiceProviderLoader.Object, o };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetService_InvalidLoader_TestData))]
        public void DesignerHost_GetService_IMultitargetHelperServiceWithLoader_ReturnsExpected(DesignerLoader loader, object expected)
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(null);
            var surface = new SubDesignSurface(mockServiceProvider.Object);
            surface.BeginLoad(loader);

            IDesignerLoaderHost2 host = surface.Host;
            Assert.Same(expected, host.GetService(typeof(IMultitargetHelperService)));
        }

        [WinFormsFact]
        public void DesignerHost_GetServiceIMultitargetHelperServiceWithoutLoader_ReturnsNull()
        {
            var service = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Null(host.GetService(typeof(IMultitargetHelperService)));
        }

        [WinFormsFact]
        public void DesignerHost_GetService_InvokeWithServiceProvider_ReturnsExpected()
        {
            var service = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(int)))
                .Returns(service)
                .Verifiable();
            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Same(service, host.GetService(typeof(int)));
            mockServiceProvider.Verify(p => p.GetService(typeof(int)), Times.Once());
        }

        [WinFormsFact]
        public void DesignerHost_GetService_InvokeWithoutServiceProvider_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Null(host.GetService(typeof(int)));
        }

        [WinFormsFact]
        public void DesignerHost_GetService_IContainer_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Same(host, host.GetService(typeof(IContainer)));
        }

        [WinFormsTheory]
        [InlineData(typeof(IServiceContainer))]
        [InlineData(typeof(ServiceContainer))]
        public void DesignerHost_GetService_IServiceContainer_ReturnsExpected(Type serviceType)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Same(surface.ServiceContainer, host.GetService(serviceType));
        }

        [WinFormsFact]
        public void DesignerHost_GetService_NullType_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Throws<ArgumentNullException>("service", () => host.GetService(null));
        }

        [WinFormsTheory]
        [InlineData("", null)]
        [InlineData("", typeof(int))]
        [InlineData("typeName", null)]
        [InlineData("typeName", typeof(int))]
        [InlineData("System.Object", null)]
        [InlineData("System.Object", typeof(int))]
        public void DesignerHost_GetType_InvokeWithTypeResolutionService_ReturnsExpected(string typeName, Type expected)
        {
            var mockTypeResolutionService = new Mock<ITypeResolutionService>(MockBehavior.Strict);
            mockTypeResolutionService
                .Setup(s => s.GetType(typeName))
                .Returns(expected)
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(mockTypeResolutionService.Object)
                .Verifiable();
            using var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerHost host = surface.Host;
            Assert.Equal(expected, host.GetType(typeName));
            mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
            mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Once());
        }

        public static IEnumerable<object[]> GetType_InvalidTypeResolutionService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetType_InvalidTypeResolutionService_TestData))]
        public void DesignerHost_GetType_InvokeWithInvalidTypeResolutionService_ReturnsExpected(object service)
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(service)
                .Verifiable();
            using var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerHost host = surface.Host;
            Assert.Equal(typeof(int), host.GetType(typeof(int).FullName));
            mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
        }

        [WinFormsFact]
        public void DesignerHost_GetType_NullTypeName_ThrowsArgumentNullException()
        {
            using var surface = new SubDesignSurface();
            IDesignerHost host = surface.Host;
            Assert.Throws<ArgumentNullException>("typeName", () => host.GetType(null));
        }

        private static readonly IDesignerHost s_placeholderHost = new Mock<IDesignerHost>(MockBehavior.Strict).Object;

        public static IEnumerable<object[]> ChangeActiveDesigner_TestData()
        {
            IDesignerLoaderHost2 otherHost = new SubDesignSurface().Host;

            yield return new object[] { null, 0, 0, 0 };

            yield return new object[] { new ActiveDesignerEventArgs(s_placeholderHost, s_placeholderHost), 0, 1, 1 };
            yield return new object[] { new ActiveDesignerEventArgs(s_placeholderHost, otherHost), 0, 1, 1 };
            yield return new object[] { new ActiveDesignerEventArgs(s_placeholderHost, null), 0, 1, 1 };

            yield return new object[] { new ActiveDesignerEventArgs(otherHost, s_placeholderHost), 1, 0, 0 };
            yield return new object[] { new ActiveDesignerEventArgs(otherHost, otherHost), 0, 0, 0 };
            yield return new object[] { new ActiveDesignerEventArgs(otherHost, null), 0, 0, 0 };

            yield return new object[] { new ActiveDesignerEventArgs(null, s_placeholderHost), 1, 0, 0 };
            yield return new object[] { new ActiveDesignerEventArgs(null, otherHost), 0, 0, 0 };
            yield return new object[] { new ActiveDesignerEventArgs(null, null), 0, 0, 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(ChangeActiveDesigner_TestData))]
        public void DesignerHost_ChangeActiveDesigner_Invoke_Success(ActiveDesignerEventArgs eventArgs, int expectedActivatedCallCount, int expectedDeactivatedCallCount, int expectedFlushCount)
        {
            var mockDesignerEventService = new Mock<IDesignerEventService>(MockBehavior.Strict);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(mockDesignerEventService.Object);
            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;

            int activatedCallCount = 0;
            EventHandler activatedHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(EventArgs.Empty, e);
                activatedCallCount++;
            };
            host.Activated += activatedHandler;
            int deactivatedCallCount = 0;
            EventHandler deactivatedHandler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(EventArgs.Empty, e);
                deactivatedCallCount++;
            };
            host.Deactivated += deactivatedHandler;
            int flushCallCount = 0;
            EventHandler flushedHandler = (sender, e) =>
            {
                Assert.Same(surface, sender);
                Assert.Same(EventArgs.Empty, e);
                flushCallCount++;
            };
            surface.Flushed += flushedHandler;

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad((IDesignerLoaderHost)surface.ComponentContainer));
            mockLoader
                .Setup(l => l.Flush());
            surface.BeginLoad(mockLoader.Object);
            Assert.Equal(0, activatedCallCount);
            Assert.Equal(0, deactivatedCallCount);
            Assert.Equal(0, flushCallCount);

            // Replace placeholders for "this"
            ActiveDesignerEventArgs actualEventArgs = null;
            if (eventArgs != null)
            {
                IDesignerHost newDesigner = eventArgs.NewDesigner == s_placeholderHost ? host : eventArgs.NewDesigner;
                IDesignerHost oldDesigner = eventArgs.OldDesigner == s_placeholderHost ? host : eventArgs.OldDesigner;
                actualEventArgs = new ActiveDesignerEventArgs(oldDesigner, newDesigner);
            }
            mockDesignerEventService.Raise(s => s.ActiveDesignerChanged += null, actualEventArgs);
            Assert.Equal(expectedActivatedCallCount, activatedCallCount);
            Assert.Equal(expectedDeactivatedCallCount, deactivatedCallCount);
            Assert.Equal(expectedFlushCount, flushCallCount);

            // Should not invoke if removed.
            host.Activated -= activatedHandler;
            host.Deactivated -= deactivatedHandler;
            surface.Flushed -= flushedHandler;
            mockDesignerEventService.Raise(s => s.ActiveDesignerChanged += null, actualEventArgs);
            Assert.Equal(expectedActivatedCallCount, activatedCallCount);
            Assert.Equal(expectedDeactivatedCallCount, deactivatedCallCount);
            Assert.Equal(expectedFlushCount, flushCallCount);
        }

        [WinFormsFact]
        public void DesignerHost_Remove_Invoke_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            var rootComponent = new RootDesignerComponent();
            using var component = new DesignerComponent();
            host.Container.Add(rootComponent);
            host.Container.Add(component);
            host.Container.Remove(rootComponent);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Null(host.RootComponent);
            Assert.Null(host.RootComponentClassName);
            Assert.Null(host.GetDesigner(rootComponent));
            Assert.NotNull(host.GetDesigner(component));
            Assert.Null(rootComponent.Container);
            Assert.Null(rootComponent.Site);
            Assert.Same(host.Container, component.Container);
            Assert.NotNull(component.Site);

            // Remove again.
            host.Container.Remove(rootComponent);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Null(host.RootComponent);
            Assert.Null(host.RootComponentClassName);
            Assert.Null(host.GetDesigner(rootComponent));
            Assert.NotNull(host.GetDesigner(component));
            Assert.Null(rootComponent.Container);
            Assert.Null(rootComponent.Site);
            Assert.Same(host.Container, component.Container);
            Assert.NotNull(component.Site);

            // Remove other.
            host.Container.Remove(component);
            Assert.Empty(host.Container.Components);
            Assert.Null(host.RootComponent);
            Assert.Null(host.RootComponentClassName);
            Assert.Null(host.GetDesigner(rootComponent));
            Assert.Null(host.GetDesigner(component));
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

        [WinFormsTheory]
        [MemberData(nameof(Remove_IExtenderProviderServiceWithoutDefault_TestData))]
        public void DesignerHost_Remove_IExtenderProviderServiceWithoutDefault_Success(Component component, int expectedAddCallCount, int expectedRemoveCallCount)
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
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IExtenderProviderService)))
                .Returns(mockExtenderProviderService.Object)
                .Verifiable();

            using var surface = new SubDesignSurface(mockServiceProvider.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            IDesignerLoaderHost2 host = surface.Host;

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Same(host, component.Container);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedAddCallCount));

            // Remove.
            host.Container.Remove(component);
            Assert.Empty(host.Container.Components);
            Assert.Null(component.Container);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(expectedAddCallCount + expectedRemoveCallCount));
            mockExtenderProviderService.Verify(s => s.RemoveExtenderProvider(component as IExtenderProvider), Times.Exactly(expectedRemoveCallCount));

            // Remove again.
            host.Container.Remove(component);
            Assert.Empty(host.Container.Components);
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

        [WinFormsTheory]
        [MemberData(nameof(Remove_IExtenderProviderServiceWithDefault_TestData))]
        public void DesignerHost_Remove_IExtenderProviderServiceWithDefault_Success(Component component)
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
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(INameCreationService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IExtenderProviderService)))
                .Returns(mockExtenderProviderService.Object)
                .Verifiable();

            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            Assert.Empty(component.Site.Name);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
            mockExtenderProviderService.Verify(s => s.AddExtenderProvider(component as IExtenderProvider), Times.Never());

            // Remove.
            host.Container.Remove(component);
            Assert.Empty(host.Container.Components);
            Assert.Null(component.Container);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
            mockExtenderProviderService.Verify(s => s.RemoveExtenderProvider(component as IExtenderProvider), Times.Never());

            // Remove again.
            host.Container.Remove(component);
            Assert.Empty(host.Container.Components);
            Assert.Null(component.Container);
            mockServiceProvider.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
            mockExtenderProviderService.Verify(s => s.RemoveExtenderProvider(component as IExtenderProvider), Times.Never());
        }

        [WinFormsTheory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void DesignerHost_Remove_InvalidIExtenderProviderServiceWithoutDefault_CallsParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            using var surface = new SubDesignSurface(mockParentProvider?.Object);
            surface.ServiceContainer.RemoveService(typeof(IExtenderProviderService));
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootExtenderProviderDesignerComponent();

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Once());

            host.Container.Remove(component);
            Assert.Empty(host.Container.Components);
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Exactly(2));
        }

        [WinFormsTheory]
        [MemberData(nameof(InvalidIExtenderProviderService_TestData))]
        public void DesignerHost_Remove_InvalidIExtenderProviderServiceWithDefault_DoesNotCallParentGetService(Mock<IServiceProvider> mockParentProvider)
        {
            using var surface = new SubDesignSurface(mockParentProvider?.Object);
            IDesignerLoaderHost2 host = surface.Host;
            using var component = new RootExtenderProviderDesignerComponent();

            host.Container.Add(component);
            Assert.Same(component, Assert.Single(host.Container.Components));
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());

            host.Container.Remove(component);
            Assert.Empty(host.Container.Components);
            mockParentProvider?.Verify(p => p.GetService(typeof(IExtenderProviderService)), Times.Never());
        }

        [WinFormsFact]
        public void Remove_ComponentNotInContainerNonEmpty_Nop()
        {
            using var surface1 = new SubDesignSurface();
            using var surface2 = new SubDesignSurface();
            IDesignerLoaderHost2 host1 = surface1.Host;
            IDesignerLoaderHost2 host2 = surface2.Host;

            var otherComponent = new RootDesignerComponent();
            using var component = new RootDesignerComponent();
            host1.Container.Add(otherComponent);
            host2.Container.Add(component);
            host2.Container.Remove(otherComponent);
            host2.Container.Remove(new Component());
            Assert.Same(otherComponent, Assert.Single(host1.Container.Components));
            Assert.Same(component, Assert.Single(host2.Container.Components));
        }

        [WinFormsFact]
        public void Remove_ComponentNotInContainerEmpty_Nop()
        {
            using var surface1 = new SubDesignSurface();
            using var surface2 = new SubDesignSurface();
            IDesignerLoaderHost2 host1 = surface1.Host;
            IDesignerLoaderHost2 host2 = surface2.Host;

            var otherComponent = new RootDesignerComponent();
            host1.Container.Add(otherComponent);
            host2.Container.Remove(otherComponent);
            host2.Container.Remove(new Component());
            Assert.Same(otherComponent, Assert.Single(host1.Container.Components));
            Assert.Empty(host2.Container.Components);
        }

        [WinFormsFact]
        public void Remove_InvokeWithComponentRemoved_CallsHandler()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            using var component1 = new RootDesignerComponent();
            using var component2 = new DesignerComponent();
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

            host.Container.Add(component1);
            host.Container.Add(component2);

            // With handler.
            host.Container.Remove(component1);
            Assert.Null(component1.Container);
            Assert.Null(component1.Site);
            Assert.Same(host.Container, component2.Container);
            Assert.NotNull(component2.Site);
            Assert.Same(component2, Assert.Single(host.Container.Components));
            Assert.Equal(1, componentRemovingCallCount);
            Assert.Equal(1, componentRemovedCallCount);

            // Remove again.
            host.Container.Remove(component1);
            Assert.Null(component1.Container);
            Assert.Null(component1.Site);
            Assert.Same(host.Container, component2.Container);
            Assert.NotNull(component2.Site);
            Assert.Same(component2, Assert.Single(host.Container.Components));
            Assert.Equal(1, componentRemovingCallCount);
            Assert.Equal(1, componentRemovedCallCount);

            // Remove handler.
            changeService.ComponentRemoving -= componentRemovingHandler;
            changeService.ComponentRemoved -= componentRemovedHandler;
            host.Container.Remove(component2);
            Assert.Null(component1.Container);
            Assert.Null(component1.Site);
            Assert.Null(component2.Container);
            Assert.Null(component2.Site);
            Assert.Empty(host.Container.Components);
            Assert.Equal(1, componentRemovingCallCount);
            Assert.Equal(1, componentRemovedCallCount);
        }

        [WinFormsFact]
        public void DesignerHost_Remove_SetSiteToNullInComponentRemoving_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            using var component = new RootDesignerComponent();
            int componentRemovingCallCount = 0;
            ComponentEventHandler componentRemovingHandler = (sender, e) =>
            {
                component.Site = null;
                componentRemovingCallCount++;
            };
            changeService.ComponentRemoving += componentRemovingHandler;

            host.Container.Add(component);
            host.Container.Remove(component);
            Assert.Null(component.Container);
            Assert.Null(component.Site);
        }

        [WinFormsFact]
        public void DesignerHost_Remove_SiteHasDictionary_ClearsDictionary()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            using var component = new RootDesignerComponent();
            host.Container.Add(component);
            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            service.SetValue("key", "value");
            Assert.Equal("value", service.GetValue("key"));

            host.Container.Remove(component);
            Assert.Null(service.GetValue("key"));
        }

        [WinFormsFact]
        public void DesignerHost_Remove_NullComponent_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Throws<ArgumentNullException>("component", () => host.Container.Remove(null));
        }

        [WinFormsFact]
        public void DesignerHost_RemoveService_Invoke_GetServiceReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            host.AddService(typeof(object), new object());
            host.RemoveService(typeof(object));
            Assert.Null(surface.ServiceContainer.GetService(typeof(object)));
            Assert.Null(surface.GetService(typeof(object)));
            Assert.Null(host.GetService(typeof(object)));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerHost_RemoveService_InvokeBool_GetServiceReturnsExpected(bool promote)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;

            host.AddService(typeof(object), new object());
            host.RemoveService(typeof(object), promote);
            Assert.Null(surface.ServiceContainer.GetService(typeof(object)));
            Assert.Null(surface.GetService(typeof(object)));
            Assert.Null(host.GetService(typeof(object)));
        }

        [WinFormsFact]
        public void DesignerHost_RemoveService_InvokeDisposed_ThrowsObjectDisposedException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            surface.Dispose();
            ServiceCreatorCallback callback = (container, service) => new object();
            Assert.Throws<ObjectDisposedException>(() => host.RemoveService(typeof(object)));
            Assert.Throws<ObjectDisposedException>(() => host.RemoveService(typeof(object), true));
            Assert.Throws<ObjectDisposedException>(() => host.RemoveService(typeof(object), false));
        }

        public static IEnumerable<object[]> OnComponentChanging_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new object(), TypeDescriptor.GetProperties(typeof(string))[0] };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnComponentChanging_TestData))]
        public void DesignerHost_IComponentChangeServiceOnComponentChanging_Invoke_CallsComponentChanging(object component, MemberDescriptor member)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            int callCount = 0;
            ComponentChangingEventHandler handler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(component, e.Component);
                Assert.Same(member, e.Member);
                callCount++;
            };
            changeService.ComponentChanging += handler;
            Assert.False(host.Loading);

            // With handler.
            changeService.OnComponentChanging(component, member);
            Assert.Equal(1, callCount);

            // Call again.
            changeService.OnComponentChanging(component, member);
            Assert.Equal(2, callCount);

            // Remove handler.
            changeService.ComponentChanging -= handler;
            changeService.OnComponentChanging(component, member);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnComponentChanging_TestData))]
        public void DesignerHost_IComponentChangeServiceOnComponentChanging_InvokeLoading_DoesNotCallHandler(object component, MemberDescriptor member)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            int callCount = 0;
            ComponentChangingEventHandler handler = (sender, e) => callCount++;
            changeService.ComponentChanging += handler;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host));
            surface.BeginLoad(mockLoader.Object);
            Assert.True(host.Loading);

            // With handler.
            changeService.OnComponentChanging(component, member);
            Assert.Equal(0, callCount);

            // Call again.
            changeService.OnComponentChanging(component, member);
            Assert.Equal(0, callCount);

            // Remove handler.
            changeService.ComponentChanging -= handler;
            changeService.OnComponentChanging(component, member);
            Assert.Equal(0, callCount);
        }

        public static IEnumerable<object[]> OnComponentChanged_TestData()
        {
            yield return new object[] { null, null, null, null };
            yield return new object[] { new object(), TypeDescriptor.GetProperties(typeof(string))[0], new object(), new object() };
        }

        [WinFormsTheory]
        [MemberData(nameof(OnComponentChanged_TestData))]
        public void DesignerHost_IComponentChangeServiceOnComponentChanged_Invoke_CallsComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            int callCount = 0;
            ComponentChangedEventHandler handler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(component, e.Component);
                Assert.Same(member, e.Member);
                Assert.Same(oldValue, e.OldValue);
                Assert.Same(newValue, e.NewValue);
                callCount++;
            };
            changeService.ComponentChanged += handler;
            Assert.False(host.Loading);

            // With handler.
            changeService.OnComponentChanged(component, member, oldValue, newValue);
            Assert.Equal(1, callCount);

            // Call again.
            changeService.OnComponentChanged(component, member, oldValue, newValue);
            Assert.Equal(2, callCount);

            // Remove handler.
            changeService.ComponentChanged -= handler;
            changeService.OnComponentChanged(component, member, oldValue, newValue);
            Assert.Equal(2, callCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(OnComponentChanged_TestData))]
        public void DesignerHost_IComponentChangeServiceOnComponentChanged_InvokeLoading_DoesNotCallHandler(object component, MemberDescriptor member, object oldValue, object newValue)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            int callCount = 0;
            ComponentChangedEventHandler handler = (sender, e) => callCount++;
            changeService.ComponentChanged += handler;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host));
            surface.BeginLoad(mockLoader.Object);
            Assert.True(host.Loading);

            // With handler.
            changeService.OnComponentChanged(component, member, oldValue, newValue);
            Assert.Equal(0, callCount);

            // Call again.
            changeService.OnComponentChanged(component, member, oldValue, newValue);
            Assert.Equal(0, callCount);

            // Remove handler.
            changeService.ComponentChanged -= handler;
            changeService.OnComponentChanged(component, member, oldValue, newValue);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_UnderlyingSystemType_ReturnsExpected()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost), reflect.UnderlyingSystemType);
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetField_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetField(nameof(IDesignerHost.Activate)), reflect.GetField(nameof(IDesignerHost.Activate), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetFields_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetFields(), reflect.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetMember_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetMember(nameof(IDesignerHost.Container)), reflect.GetMember(nameof(IDesignerHost.Container), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetMembers_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetMembers(), reflect.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetMethod_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetMethod(nameof(IDesignerHost.Activate)), reflect.GetMethod(nameof(IDesignerHost.Activate), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
            Assert.Equal(typeof(IDesignerHost).GetMethod(nameof(IDesignerHost.Activate)), reflect.GetMethod(nameof(IDesignerHost.Activate), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, Array.Empty<Type>(), Array.Empty<ParameterModifier>()));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetMethods_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetMethods(), reflect.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetProperty_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetProperty(nameof(IDesignerHost.Container)), reflect.GetProperty(nameof(IDesignerHost.Container), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
            Assert.Equal(typeof(IDesignerHost).GetProperty(nameof(IDesignerHost.Container)), reflect.GetProperty(nameof(IDesignerHost.Container), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, typeof(IContainer), Array.Empty<Type>(), Array.Empty<ParameterModifier>()));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_GetProperties_Success()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(typeof(IDesignerHost).GetProperties(), reflect.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public));
        }

        [WinFormsFact]
        public void DesignerHost_IReflect_InvokeMember_ReturnsExpected()
        {
            using var surface = new SubDesignSurface();
            IReflect reflect = Assert.IsAssignableFrom<IReflect>(surface.Host);
            Assert.Equal(surface.Host.Container, reflect.InvokeMember(nameof(IDesignerHost.Container), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, surface.Host, Array.Empty<object>(), Array.Empty<ParameterModifier>(), null, Array.Empty<string>()));
        }

        private class SubDesignSurface : DesignSurface
        {
            public SubDesignSurface() : base()
            {
            }

            public SubDesignSurface(IServiceProvider parentProvider) : base(parentProvider)
            {
            }

            public IDesignerLoaderHost2 Host => Assert.IsAssignableFrom<IDesignerLoaderHost2>(ComponentContainer);

            public new ServiceContainer ServiceContainer => base.ServiceContainer;
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

        [ProjectTargetFramework("ProjectTargetFramework")]
        private class ClassWithProjectTargetFrameworkAttribute
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

        private class ThrowingDisposeComponentDesigner : Designer
        {
            protected override void Dispose(bool disposing)
            {
                throw new NotImplementedException();
            }
        }

        [Designer(typeof(ThrowingDisposeComponentDesigner))]
        private class ThrowingDesignerDisposeComponent : Component
        {
        }

        [Designer(typeof(Designer))]
        private class ThrowingDisposeDesignerComponent : Component
        {
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    throw new NotImplementedException();
                }
            }
        }

        private class ThrowingDisposeRootComponentDesigner : RootDesigner
        {
            protected override void Dispose(bool disposing)
            {
                throw new NotImplementedException();
            }
        }

        [Designer(typeof(ThrowingDisposeRootComponentDesigner), typeof(IRootDesigner))]
        private class ThrowingRootDesignerDisposeComponent : Component
        {
        }

        [Designer(typeof(RootDesigner), typeof(IRootDesigner))]
        private class ThrowingDisposeRootDesignerComponent : Component
        {
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    throw new NotImplementedException();
                }
            }
        }

        [Designer(typeof(RootDesigner), typeof(IRootDesigner))]
        private class CustomTypeDescriptionProviderComponent : Component
        {
        }
    }
}
