// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignerHostTests
    {
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
                    .Setup(p => p.GetService(typeof(INameCreationService)))
                    .Returns(mockNameCreationService.Object);
                yield return new object[] { mockServiceProvider.Object, name };
            }
        }

        [Theory]
        [MemberData(nameof(Add_ComponentParentProvider_TestData))]
        public void Add_ComponentWithRootDesigner_Success(IServiceProvider parentProvider, string expectedName)
        {
            var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            var component1 = new RootDesignerComponent();
            var component2 = new RootDesignerComponent();
            var component3 = new DesignerComponent();
            var component4 = new Component();

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

        [Theory]
        [MemberData(nameof(Add_InvalidNameCreationServiceParentProvider_TestData))]
        public void Add_ComponentStringWithRootDesigner_Success(IServiceProvider parentProvider)
        {
            var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            var component1 = new RootDesignerComponent();
            var component2 = new RootDesignerComponent();
            var component3 = new DesignerComponent();
            var component4 = new Component();

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

        [Fact]
        public void Add_SameComponent_Success()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();

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

        [Fact]
        public void Add_ComponentWithNameCreationServiceWithoutName_CallsCreateName()
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
            var component = new RootDesignerComponent();

            host.Container.Add(component);
            Assert.Equal("name", component.Site.Name);
            mockNameCreationService.Verify(s => s.CreateName(host.Container, typeof(RootDesignerComponent)), Times.Once());

            host.Container.Add(component, null);
            Assert.Equal("name", component.Site.Name);
            mockNameCreationService.Verify(s => s.CreateName(host.Container, typeof(RootDesignerComponent)), Times.Once());
        }

        [Fact]
        public void Add_ComponentWithNameCreationServiceWithCustomReflectionType_CallsCreateName()
        {
            var component = new CustomTypeDescriptionProviderComponent();
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
        public void Add_ComponentWithNameCreationServiceWithName_CallsValidateName(string name)
        {
            var mockNameCreationService = new Mock<INameCreationService>(MockBehavior.Strict);
            mockNameCreationService
                .Setup(s => s.ValidateName(name))
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
            var component = new RootDesignerComponent();
            host.Container.Add(component, name);
            Assert.Same(name, component.Site.Name);
            mockNameCreationService.Verify(s => s.ValidateName(name), Times.Once());
        }

        [Fact]
        public void AddComponentIDictionaryServiceGetKey_NoDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Null(service.GetKey(null));
            Assert.Null(service.GetKey(new object()));
        }

        [Fact]
        public void AddComponentIDictionaryServiceGetKey_NoSuchKeyWithDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
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

        [Fact]
        public void AddComponentIDictionaryServiceGetValue_NoDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Null(service.GetValue(new object()));
        }

        [Fact]
        public void AddComponentIDictionaryServiceGetValue_NoSuchValueWithDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            var key1 = new object();
            var value1 = new object();
            service.SetValue(key1, value1);
            Assert.Same(key1, service.GetKey(value1));
            Assert.Same(value1, service.GetValue(key1));
    
            Assert.Null(service.GetValue(new object()));
        }

        [Fact]
        public void AddComponentIDictionaryServiceGetValue_NullValueNoDictionary_ReturnsNull()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Throws<ArgumentNullException>("key", () => service.GetValue(null));
        }

        [Fact]
        public void AddComponentIDictionaryServiceGetValue_NullValueWithDictionary_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);

            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            var key1 = new object();
            var value1 = new object();
            service.SetValue(key1, value1);
            Assert.Same(key1, service.GetKey(value1));
            Assert.Same(value1, service.GetValue(key1));
    
            Assert.Throws<ArgumentNullException>("key", () => service.GetValue(null));
        }

        [Fact]
        public void AddComponentIDictionaryServiceSetValue_Invoke_GetKeyValueReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
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

        [Fact]
        public void AddComponentIDictionaryServiceSetValue_NullKey_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            IDictionaryService service = Assert.IsAssignableFrom<IDictionaryService>(component.Site);
            Assert.Throws<ArgumentNullException>("key", () => service.SetValue(null, new object()));
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

        [Theory]
        [MemberData(nameof(AddComponentISiteName_Set_TestData))]
        public void AddComponentISiteName_SetRootComponent_GetReturnsExpected(IServiceProvider parentProvider, string oldName, string value, string expectedName)
        {
            var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component, oldName);
            component.Site.Name = value;
            Assert.Same(expectedName, component.Site.Name);
            Assert.Same(expectedName, host.RootComponentClassName);

            // Set same.
            component.Site.Name = value;
            Assert.Same(expectedName, component.Site.Name);
            Assert.Same(expectedName, host.RootComponentClassName);
        }

        [Fact]
        public void AddComponentISiteName_SetDifferentCase_GetReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component, "name");
            component.Site.Name = "NAME";
            Assert.Equal("NAME", component.Site.Name);

            // Set same.
            component.Site.Name = "NAME";
            Assert.Equal("NAME", component.Site.Name);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AddComponentISiteName_SetWithMultipleComponents_GetReturnsExpected(string value)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component1 = new RootDesignerComponent();
            var component2 = new RootDesignerComponent();
            var component3 = new RootDesignerComponent();
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

        [Theory]
        [MemberData(nameof(AddComponentISiteName_SetWithNamespaceInRootComponentClassName_TestData))]
        public void AddComponentISiteName_SetWithNamespaceInRootComponentClassName_GetReturnsExpected(string oldRootComponentClassName, string oldName, string value, string expectedName, string expectedRootComponentClassName)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();

            host.EndLoad(oldRootComponentClassName, true, null);
            Assert.Equal(oldRootComponentClassName, host.RootComponentClassName);
            Assert.Null(host.RootComponent);

            host.Container.Add(component, oldName);
            Assert.Equal(oldName, component.Site.Name);
            Assert.Equal(oldRootComponentClassName, host.RootComponentClassName);
            Assert.Same(component, host.RootComponent);

            component.Site.Name = value;
            Assert.Same(expectedName, component.Site.Name);
            Assert.Equal(expectedRootComponentClassName, host.RootComponentClassName);
            Assert.Same(component, host.RootComponent);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void AddComponentISiteName_SetNameWithComponentRename_CallsHandler(string value)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);
            var component = new RootDesignerComponent();

            int callCount = 0;
            ComponentRenameEventHandler handler = (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(component, e.Component);
                Assert.Same("oldName", e.OldName);
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

        [Theory]
        [InlineData(null, "", 1)]
        [InlineData("", "", 1)]
        [InlineData("newName", "newName", 1)]
        [InlineData("OLDNAME", "OLDNAME", 0)]
        public void AddComponentISite_SetNameWithINameCreateService_CallsValidateName(string value, string expectedName, int expectedCallCount)
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
            var component = new RootDesignerComponent();
            host.Container.Add(component, "oldName");
            Assert.Equal("oldName", component.Site.Name);
            mockNameCreationService.Verify(s => s.ValidateName("oldName"), Times.Once());

            component.Site.Name = value;
            Assert.Equal(expectedName, component.Site.Name);
            mockNameCreationService.Verify(s => s.ValidateName(expectedName), Times.Exactly(expectedCallCount));
        }

        [Fact]
        public void AddComponentISiteName_SetSameAsOtherComponent_GetReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component1 = new RootDesignerComponent();
            var component2 = new RootDesignerComponent();
            host.Container.Add(component1, "name1");
            host.Container.Add(component2, "name2");
            Assert.Throws<Exception>(() => component1.Site.Name = "name2");
            Assert.Throws<Exception>(() => component1.Site.Name = "NAME2");
        }

        [Fact]
        public void AddComponentISiteGetService_Invoke_ReturnsExpected()
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
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(service, component.Site.GetService(typeof(int)));
        }

        [Fact]
        public void AddComponentISiteGetService_InvokeWithNestedContainer_ReturnsNull()
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
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            INestedContainer nestedContainer = Assert.IsAssignableFrom<INestedContainer>(component.Site.GetService(typeof(INestedContainer)));
            Assert.Null(component.Site.GetService(typeof(int)));
            Assert.Same(component.Site, component.Site.GetService(typeof(IDictionaryService)));
            Assert.Same(nestedContainer, component.Site.GetService(typeof(INestedContainer)));
        }

        [Fact]
        public void AddComponentISiteGetService_INestedContainer_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            INestedContainer container = Assert.IsAssignableFrom<INestedContainer>(component.Site.GetService(typeof(INestedContainer)));
            Assert.Same(container, component.Site.GetService(typeof(INestedContainer)));
            Assert.Empty(container.Components);
            Assert.Same(component, container.Owner);
        }

        [Fact]
        public void AddComponentISiteGetServiceINestedContainerGetService_Invoke_ReturnsExpected()
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
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            NestedContainer container = Assert.IsAssignableFrom<NestedContainer>(component.Site.GetService(typeof(INestedContainer)));
            var nestedComponent = new Component();
            container.Add(nestedComponent);
            Assert.Same(service, nestedComponent.Site.GetService(typeof(int)));
        }

        [Fact]
        public void AddComponentISiteGetService_IDictionaryService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(component.Site, component.Site.GetService(typeof(IDictionaryService)));
        }

        [Fact]
        public void AddComponentISiteGetService_IServiceContainerReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(surface.ServiceContainer, component.Site.GetService(typeof(IServiceContainer)));
        }

        [Fact]
        public void AddComponentISiteGetService_IContainerReturnsExpected()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Same(host, component.Site.GetService(typeof(IContainer)));
        }

        [Fact]
        public void AddComponentISiteGetService_NullServiceType_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new RootDesignerComponent();
            host.Container.Add(component);
            Assert.Throws<ArgumentNullException>("service", () => component.Site.GetService(null));
        }

        [Fact]
        public void Add_IComponentWithComponentAddingAndAdded_CallsHandler()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            IComponentChangeService changeService = Assert.IsAssignableFrom<IComponentChangeService>(host);

            var component = new RootDesignerComponent();
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

        [Fact]
        public void Add_NullComponent_ThrowsArgumentNullException()
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

        [Theory]
        [MemberData(nameof(Add_NoRootDesigner_TestData))]
        public void Add_NoRootDesigner_ThrowsException(IComponent component)
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            Assert.Throws<Exception>(() => host.Container.Add(component));
            Assert.Throws<Exception>(() => host.Container.Add(component, "name"));
        }

        [Fact]
        public void Add_NonInitializingRootDesigner_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new NonInitializingDesignerComponent();
            Assert.Throws<InvalidOperationException>(() => host.Container.Add(component));
            Assert.Throws<InvalidOperationException>(() => host.Container.Add(component, "name"));
        }

        [Fact]
        public void Add_ThrowingInitializingRootDesigner_RethrowsException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new ThrowingInitializingDesignerComponent();
            Assert.Throws<DivideByZeroException>(() => host.Container.Add(component));
            Assert.Null(component.Container);
            Assert.Null(component.Site);

            Assert.Throws<DivideByZeroException>(() => host.Container.Add(component, "name"));
            Assert.Null(component.Container);
            Assert.Null(component.Site);
        }

        [Fact]
        public void Add_CheckoutExceptionThrowingInitializingRootDesigner_RethrowsException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var component = new CheckoutExceptionThrowingInitializingDesignerComponent();
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

            public void Dispose()
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

        [Designer(typeof(RootDesigner), typeof(IRootDesigner))]
        private class CustomTypeDescriptionProviderComponent : Component
        {
        }
    }
}
