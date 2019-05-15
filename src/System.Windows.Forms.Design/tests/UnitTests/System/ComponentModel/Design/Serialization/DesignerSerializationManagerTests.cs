// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Serialization.Tests
{
    public class DesignerSerializationManagerTests
    {
        [Fact]
        public void DesignerSerializationManager_Ctor_Default()
        {
            var manager = new SubDesignerSerializationManager();
            Assert.Null(manager.Container);
            Assert.True(manager.PreserveNames);
            Assert.Empty(manager.Properties);
            Assert.Same(manager.Properties, manager.Properties);
            Assert.Null(manager.PropertyProvider);
            Assert.False(manager.RecycleInstances);
            Assert.True(manager.ValidateRecycledTypes);;
        }

        public static IEnumerable<object[]> Ctor_IServiceProvider_TestData()
        {
            var nullMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns((IDesignerHost)null);
            yield return new object[] { nullMockServiceProvider.Object, null };

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns(new object());
            yield return new object[] { invalidMockServiceProvider.Object, null };

            var container = new Container();
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.Container)
                .Returns(container);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            yield return new object[] { mockServiceProvider.Object, container };
        }

        [Theory]
        [MemberData(nameof(Ctor_IServiceProvider_TestData))]
        public void DesignerSerializationManager_Ctor_IServiceProvider(IServiceProvider provider, IContainer expectedContainer)
        {
            var manager = new SubDesignerSerializationManager(provider);
            Assert.Same(expectedContainer, manager.Container);
            Assert.Same(manager.Container, manager.Container);
            Assert.True(manager.PreserveNames);
            Assert.Empty(manager.Properties);
            Assert.Same(manager.Properties, manager.Properties);
            Assert.Null(manager.PropertyProvider);
            Assert.False(manager.RecycleInstances);
            Assert.True(manager.ValidateRecycledTypes);;
        }

        [Fact]
        public void DesignerSerializationManager_Ctor_NullProvider_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("provider", () => new DesignerSerializationManager(null));
        }

        public static IEnumerable<object[]> Container_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Container() };
        }

        [Theory]
        [MemberData(nameof(Container_Set_TestData))]
        public void DesignerSerializationManager_Container_Set_GetReturnsExpected(IContainer value)
        {
            var manager = new SubDesignerSerializationManager
            {
                Container = value
            };
            Assert.Same(value, manager.Container);
            Assert.Same(value, manager.GetService(typeof(IContainer)));
            
            // Set same.
            manager.Container = value;
            Assert.Same(value, manager.Container);
            Assert.Same(value, manager.GetService(typeof(IContainer)));
        }

        [Fact]
        public void DesignerSerializationManager_Container_SetWithSession_ThrowsInvalidOperationException()
        {
            var manager = new DesignerSerializationManager();
            manager.CreateSession();
            Assert.Throws<InvalidOperationException>(() => manager.Container = null);
        }

        [Fact]
        public void DesignerSerializationManager_Context_GetWithSession_ReturnsExpected()
        {
            var manager = new SubDesignerSerializationManager();
            IDisposable session = manager.CreateSession();
            ContextStack context = manager.Context;
            Assert.Null(context.Current);
            Assert.Same(context, manager.Context);
        }

        [Fact]
        public void DesignerSerializationManager_Context_GetNoSessionAfterGetting_ThrowsInvalidOperationException()
        {
            var manager = new SubDesignerSerializationManager();
            IDisposable session = manager.CreateSession();
            ContextStack context = manager.Context;
            Assert.NotNull(context);
            Assert.Same(context, manager.Context);
            session.Dispose();
            Assert.Throws<InvalidOperationException>(() => manager.Context);
        }

        [Fact]
        public void DesignerSerializationManager_Context_GetNoSession_ThrowsInvalidOperationException()
        {
            var manager = new SubDesignerSerializationManager();
            Assert.Throws<InvalidOperationException>(() => manager.Context);
        }

        [Fact]
        public void DesignerSerializationManager_Errors_GetWithSession_ReturnsExpected()
        {
            var manager = new DesignerSerializationManager();
            IDisposable session = manager.CreateSession();
            IList errors = manager.Errors;
            Assert.Empty(errors);
            Assert.Same(errors, manager.Errors);
            Assert.IsType<ArrayList>(errors);
        }

        [Fact]
        public void DesignerSerializationManager_Errors_NoSessionWithPreviousSession_ThrowsInvalidOperationException()
        {
            var manager = new DesignerSerializationManager();
            IDisposable session = manager.CreateSession();
            Assert.Empty(manager.Errors);
            session.Dispose();
            Assert.Throws<InvalidOperationException>(() => manager.Errors);
        }

        [Fact]
        public void DesignerSerializationManager_Errors_NoSession_ThrowsInvalidOperationException()
        {
            var manager = new DesignerSerializationManager();
            Assert.Throws<InvalidOperationException>(() => manager.Errors);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerSerializationManager_PreserveNames_Set_GetReturnsExpected(bool value)
        {
            var manager = new DesignerSerializationManager
            {
                PreserveNames = value
            };
            Assert.Equal(value, manager.PreserveNames);
            
            // Set same
            manager.PreserveNames = value;
            Assert.Equal(value, manager.PreserveNames);
            
            // Set different
            manager.PreserveNames = !value;
            Assert.Equal(!value, manager.PreserveNames);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerSerializationManager_PreserveNames_SetWithSession_ThrowsInvalidOperationException(bool value)
        {
            var manager = new DesignerSerializationManager();
            manager.CreateSession();
            Assert.Throws<InvalidOperationException>(() => manager.PreserveNames = value);
            Assert.True(manager.PreserveNames);
        }

        [Fact]
        public void DesignerSerializationManager_Properties_GetWithPropertyProvider_ReturnExpected()
        {
            var provider = new PropertyProvider();
            var manager = new SubDesignerSerializationManager
            {
                PropertyProvider = provider
            };
            PropertyDescriptorCollection properties = manager.Properties;
            Assert.Same(properties, manager.Properties);
            PropertyDescriptor property = Assert.IsAssignableFrom<PropertyDescriptor>(Assert.Single(properties));
            Assert.NotEmpty(property.Attributes);
            Assert.Equal("Category", property.Category);
            Assert.IsType<Int64Converter>(property.Converter);
            Assert.Equal(typeof(PropertyProvider), property.ComponentType);
            Assert.Equal("Description", property.Description);
            Assert.True(property.DesignTimeOnly);
            Assert.Equal("DisplayName", property.DisplayName);
            Assert.True(property.IsBrowsable);
            Assert.True(property.IsLocalizable);
            Assert.False(property.IsReadOnly);
            Assert.Equal("Value", property.Name);
            Assert.Equal(typeof(int), property.PropertyType);
            Assert.Equal(DesignerSerializationVisibility.Content, property.SerializationVisibility);
            Assert.False(property.SupportsChangeEvents);

            // Should be wrapped.
            Assert.False(property.CanResetValue(new Component()));
            Assert.Equal(0, property.GetValue(new Component()));
            property.SetValue(new Component(), 1);
            Assert.Equal(1, property.GetValue(new Component()));
            property.ResetValue(new Component());
            Assert.Equal(1, property.GetValue(new Component()));
            Assert.True(property.ShouldSerializeValue(new Component()));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DesignerSerializationManager_PropertyProvider_Set_GetReturnsExpected(string value)
        {
            var manager = new DesignerSerializationManager
            {
                PropertyProvider = value
            };
            Assert.Same(value, manager.PropertyProvider);
            
            // Set same
            manager.PropertyProvider = value;
            Assert.Same(value, manager.PropertyProvider);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void DesignerSerializationManager_PropertyProvider_SetWithSession_GetReturnsExpected(string value)
        {
            var manager = new DesignerSerializationManager();
            manager.CreateSession();
            
            manager.PropertyProvider = value;
            Assert.Same(value, manager.PropertyProvider);
            
            // Set same
            manager.PropertyProvider = value;
            Assert.Same(value, manager.PropertyProvider);
        }

        [Fact]
        public void DesignerSerializationManager_Properties_SetWithExistingProperties_Resets()
        {
            var manager = new SubDesignerSerializationManager
            {
                PropertyProvider = new PropertyProvider()
            };
            PropertyDescriptorCollection properties = manager.Properties;
            PropertyDescriptor property = Assert.IsAssignableFrom<PropertyDescriptor>(Assert.Single(properties));
            Assert.Equal(nameof(PropertyProvider.Value), property.Name);
            Assert.Same(properties, manager.Properties);
            
            var provider = new OtherPropertyProvider();
            manager.PropertyProvider = provider;
            Assert.Same(provider, manager.PropertyProvider);
            PropertyDescriptorCollection otherProperties = manager.Properties;
            PropertyDescriptor otherProperty = Assert.IsAssignableFrom<PropertyDescriptor>(Assert.Single(otherProperties));
            Assert.Equal(nameof(OtherPropertyProvider.OtherValue), otherProperty.Name);
            Assert.Same(otherProperties, manager.Properties);
            
            // Set same.
            manager.PropertyProvider = provider;
            Assert.Same(provider, manager.PropertyProvider);
            Assert.Same(otherProperties, manager.Properties);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerSerializationManager_RecycleInstances_Set_GetReturnsExpected(bool value)
        {
            var manager = new DesignerSerializationManager
            {
                RecycleInstances = value
            };
            Assert.Equal(value, manager.RecycleInstances);
            
            // Set same
            manager.RecycleInstances = value;
            Assert.Equal(value, manager.RecycleInstances);
            
            // Set different
            manager.RecycleInstances = !value;
            Assert.Equal(!value, manager.RecycleInstances);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerSerializationManager_RecycleInstances_SetWithSession_ThrowsInvalidOperationException(bool value)
        {
            var manager = new DesignerSerializationManager();
            manager.CreateSession();
            Assert.Throws<InvalidOperationException>(() => manager.RecycleInstances = value);
            Assert.False(manager.RecycleInstances);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerSerializationManager_ValidateRecycledTypes_Set_GetReturnsExpected(bool value)
        {
            var manager = new DesignerSerializationManager
            {
                ValidateRecycledTypes = value
            };
            Assert.Equal(value, manager.ValidateRecycledTypes);
            
            // Set same
            manager.ValidateRecycledTypes = value;
            Assert.Equal(value, manager.ValidateRecycledTypes);
            
            // Set different
            manager.ValidateRecycledTypes = !value;
            Assert.Equal(!value, manager.ValidateRecycledTypes);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignerSerializationManager_ValidateRecycledTypes_SetWithSession_ThrowsInvalidOperationException(bool value)
        {
            var manager = new DesignerSerializationManager();
            manager.CreateSession();
            Assert.Throws<InvalidOperationException>(() => manager.ValidateRecycledTypes = value);
            Assert.True(manager.ValidateRecycledTypes);
        }

        [Fact]
        public void DesignerSerializationManager_ResolveName_AddNoSession_ThrowsInvalidOperationException()
        {
            var manager = new SubDesignerSerializationManager();
            int callCount = 0;
            ResolveNameEventHandler handler = (sender, e) => callCount++;
            Assert.Throws<InvalidOperationException>(() => manager.ResolveName += handler);
            manager.ResolveName -= handler;
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void DesignerSerializationManager_SerializationComplete_AddNoSession_ThrowsInvalidOperationException()
        {
            var manager = new SubDesignerSerializationManager();
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            Assert.Throws<InvalidOperationException>(() => manager.SerializationComplete += handler);
            manager.SerializationComplete -= handler;
            Assert.Equal(0, callCount);
        }

        [Fact]
        public void DesignerSerializationManager_CreateSession_Invoke_Success()
        {
            var manager = new DesignerSerializationManager();
            IDisposable session = manager.CreateSession();
            Assert.NotNull(session);
            session.Dispose();

            // Get another.
            IDisposable session2 = manager.CreateSession();
            Assert.NotNull(session2);
            Assert.NotSame(session, session2);
            session2.Dispose();
        }

        [Fact]
        public void DesignerSerializationManager_CreateSession_InvokeWithSessionCreated_CallsHandler()
        {
            var manager = new DesignerSerializationManager();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(manager, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            manager.SessionCreated += handler;

            IDisposable session = manager.CreateSession();
            Assert.NotNull(session);
            Assert.Equal(1, callCount);
            session.Dispose();

            // Remove handler.
            manager.SessionCreated -= handler;
            session = manager.CreateSession();
            Assert.NotNull(session);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DesignerSerializationManager_CreateSessionDispose_InvokeWithSessionDisposed_CallsHandler()
        {
            var manager = new DesignerSerializationManager();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(manager, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            manager.SessionDisposed += handler;

            IDisposable session = manager.CreateSession();
            session.Dispose();
            Assert.Equal(1, callCount);

            // Call again.
            session = manager.CreateSession();
            session.Dispose();
            Assert.Equal(2, callCount);

            // Remove handler.
            manager.SessionDisposed -= handler;
            session = manager.CreateSession();
            session.Dispose();
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void DesignerSerializationManager_CreateSessionDispose_InvokeWithSerializationComplete_CallsHandler()
        {
            var manager = new SubDesignerSerializationManager();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(manager, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            IDisposable session = manager.CreateSession();
            manager.SerializationComplete += handler;
            session.Dispose();
            Assert.Equal(1, callCount);

            // Call again.
            session = manager.CreateSession();
            session.Dispose();
            Assert.Equal(1, callCount);

            // Remove handler.
            session = manager.CreateSession();
            manager.SerializationComplete += handler;
            manager.SerializationComplete -= handler;
            session.Dispose();
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void DesignerSerializationManager_CreateSession_InvokeWithSession_ThrowsInvalidOperationException()
        {
            var manager = new DesignerSerializationManager();
            manager.CreateSession();
            Assert.Throws<InvalidOperationException>(() => manager.CreateSession());
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(int))]
        public void DesignerSerializationManager_GetService_WithProvider_ReturnsExpected(Type serviceType)
        {
            var service = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(serviceType))
                .Returns(service)
                .Verifiable();
            var manager = new SubDesignerSerializationManager(mockServiceProvider.Object);
            Assert.Same(service, manager.GetService(serviceType));
            mockServiceProvider.Verify(p => p.GetService(serviceType), Times.Once());

            Assert.Same(service, ((IServiceProvider)manager).GetService(serviceType));
            mockServiceProvider.Verify(p => p.GetService(serviceType), Times.Exactly(2));
        }
        
        [Theory]
        [MemberData(nameof(Ctor_IServiceProvider_TestData))]
        public void DesignerSerializationManager_GetService_IContainer_ReturnsExpected(IServiceProvider provider, object expected)
        {
            var manager = new SubDesignerSerializationManager(provider);
            Assert.Same(expected, manager.GetService(typeof(IContainer)));
            Assert.Same(expected, ((IServiceProvider)manager).GetService(typeof(IContainer)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(int))]
        [InlineData(typeof(IContainer))]
        public void DesignerSerializationManager_GetService_NoProvider_ReturnsNull(Type serviceType)
        {
            var manager = new SubDesignerSerializationManager();
            Assert.Null(manager.GetService(serviceType));
            Assert.Null(((IServiceProvider)manager).GetService(serviceType));
        }

        public static IEnumerable<object[]> GetRuntimeType_ValidProvider_TestData()
        {
            foreach (string typeName in new string[] { null, string.Empty, "typeName" })
            {
                yield return new object[] { typeName, null };
                yield return new object[] { typeName, typeof(string) };
            }
        }

        [Theory]
        [MemberData(nameof(GetRuntimeType_ValidProvider_TestData))]
        public void DesignerSerializationManager_GetRuntimeType_ValidProvider_ReturnsExpected(string typeName, Type resolvedType)
        {
            var mockTypeResolutionService = new Mock<ITypeResolutionService>(MockBehavior.Strict);
            mockTypeResolutionService
                .Setup(s => s.GetType(typeName))
                .Returns(resolvedType)
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns((IDesignerHost)null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(mockTypeResolutionService.Object)
                .Verifiable();
            var manager = new DesignerSerializationManager(mockServiceProvider.Object);
            Assert.Same(resolvedType, manager.GetRuntimeType(typeName));
            mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
            mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Once());

            // Call again.
            Assert.Same(resolvedType, manager.GetRuntimeType(typeName));
            mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
            mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Exactly(2));
        }

        public static IEnumerable<object[]> GetRuntimeType_InvalidProvider_TestData()
        {
            var nullMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns((IDesignerHost)null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns((ITypeResolutionService)null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns((TypeDescriptionProviderService)null);
            yield return new object[] { nullMockServiceProvider.Object };

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(new object());
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(new object());
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(new object());
            yield return new object[] { invalidMockServiceProvider.Object };
        }

        [Theory]
        [MemberData(nameof(GetRuntimeType_InvalidProvider_TestData))]
        public void DesignerSerializationManager_GetRuntimeType_InvalidProvider_ReturnsExpected(IServiceProvider provider)
        {
            var manager = new DesignerSerializationManager(provider);
            Assert.Equal(typeof(int), manager.GetRuntimeType(typeof(int).FullName));
        }

        [Theory]
        [InlineData("System.Int32", typeof(int))]
        [InlineData("system.int32", null)]
        [InlineData("NoSuchType", null)]
        [InlineData("", null)]
        public void DesignerSerializationManager_GetRuntimeType_NoProvider_ReturnsExpected(string typeName, Type expected)
        {
            var manager = new DesignerSerializationManager();
            Assert.Same(expected, manager.GetRuntimeType(typeName));
        }

        [Fact]
        public void DesignerSerializationManager_GetRuntimeType_NullTypeName_ThrowsArgumentNullException()
        {
            var manager = new DesignerSerializationManager();
            Assert.Throws<ArgumentNullException>("typeName", () => manager.GetRuntimeType(null));
        }

        public static IEnumerable<object[]> GetType_ValidProvider_TestData()
        {
            foreach (string typeName in new string[] { null, string.Empty, "typeName" })
            {
                yield return new object[] { typeName, null, 0, true, null };
                yield return new object[] { typeName, null, 0, false, null };
                yield return new object[] { typeName, typeof(int), 1, true, typeof(int) };
                yield return new object[] { typeName, typeof(int), 1, false, null };
            }
        }

        [Theory]
        [MemberData(nameof(GetType_ValidProvider_TestData))]
        
        public void GetType_ValidProvider_ReturnsExpected(string typeName, Type resolvedType, int typeDescriptionProviderServiceCount, bool supportedType, Type expected)
        {
            var mockTypeResolutionService = new Mock<ITypeResolutionService>(MockBehavior.Strict);
            mockTypeResolutionService
                .Setup(s => s.GetType(typeName))
                .Returns(resolvedType)
                .Verifiable();
            var mockTypeDescriptionProvider = new Mock<TypeDescriptionProvider>(MockBehavior.Strict);
            mockTypeDescriptionProvider
                .Setup(p => p.IsSupportedType(resolvedType))
                .Returns(supportedType)
                .Verifiable();
            var mockTypeDescriptionProviderService = new Mock<TypeDescriptionProviderService>(MockBehavior.Strict);
            mockTypeDescriptionProviderService
                .Setup(s => s.GetProvider(resolvedType))
                .Returns(mockTypeDescriptionProvider.Object)
                .Verifiable();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns((IDesignerHost)null);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(mockTypeResolutionService.Object)
                .Verifiable();
            mockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(mockTypeDescriptionProviderService.Object)
                .Verifiable();
            var manager = new SubDesignerSerializationManager(mockServiceProvider.Object);
            Assert.Same(expected, manager.GetType(typeName));
            mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
            mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Once());
            mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Exactly(typeDescriptionProviderServiceCount));
            mockTypeDescriptionProviderService.Verify(s => s.GetProvider(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount));
            mockTypeDescriptionProvider.Verify(s => s.IsSupportedType(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount));

            // Call again.
            Assert.Same(expected, manager.GetType(typeName));
            mockServiceProvider.Verify(p => p.GetService(typeof(ITypeResolutionService)), Times.Once());
            mockTypeResolutionService.Verify(s => s.GetType(typeName), Times.Exactly(2));
            mockServiceProvider.Verify(p => p.GetService(typeof(TypeDescriptionProviderService)), Times.Exactly(typeDescriptionProviderServiceCount * 2));
            mockTypeDescriptionProviderService.Verify(s => s.GetProvider(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount * 2));
            mockTypeDescriptionProvider.Verify(s => s.IsSupportedType(resolvedType), Times.Exactly(typeDescriptionProviderServiceCount * 2));
        }

        public static IEnumerable<object[]> GetType_InvalidProvider_TestData()
        {
            var nullMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns((IDesignerHost)null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns((ITypeResolutionService)null);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns((TypeDescriptionProviderService)null);
            yield return new object[] { nullMockServiceProvider.Object };

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(new object());
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(new object());
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(new object());
            yield return new object[] { invalidMockServiceProvider.Object };

            var invalidMockTypeDescriptionProviderService = new Mock<TypeDescriptionProviderService>(MockBehavior.Strict);
            invalidMockTypeDescriptionProviderService
                .Setup(p => p.GetProvider(typeof(int)))
                .Returns((TypeDescriptionProvider)null);
            var invalidTypeDescriptionProviderServiceMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidTypeDescriptionProviderServiceMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            invalidTypeDescriptionProviderServiceMockServiceProvider
                .Setup(p => p.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            invalidTypeDescriptionProviderServiceMockServiceProvider
                .Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(invalidMockTypeDescriptionProviderService.Object);
            yield return new object[] { invalidTypeDescriptionProviderServiceMockServiceProvider.Object };
        }

        [Theory]
        [MemberData(nameof(GetType_InvalidProvider_TestData))]
        public void DesignerSerializationManager_GetType_InvalidProvider_ReturnsExpected(IServiceProvider provider)
        {
            var manager = new SubDesignerSerializationManager(provider);
            Assert.Equal(typeof(int), manager.GetType(typeof(int).FullName));
        }

        [Theory]
        [InlineData("System.Int32", typeof(int))]
        [InlineData("system.int32", null)]
        [InlineData("NoSuchType", null)]
        [InlineData("", null)]
        public void DesignerSerializationManager_GetType_NoProvider_ReturnsExpected(string typeName, Type expected)
        {
            var manager = new SubDesignerSerializationManager();
            Assert.Same(expected, manager.GetType(typeName));
        }

        [Fact]
        public void DesignerSerializationManager_GetType_NullTypeName_ThrowsArgumentNullException()
        {
            var manager = new SubDesignerSerializationManager();
            Assert.Throws<ArgumentNullException>("typeName", () => manager.GetType(null));
        }

        public static IEnumerable<object[]> ResolveNameEventArgs_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ResolveNameEventArgs("name") };
        }

        [Theory]
        [MemberData(nameof(ResolveNameEventArgs_TestData))]
        public void DesignerSerializationManager_OnResolveName_InvokeWithResolveName_CallsHandler(ResolveNameEventArgs eventArgs)
        {
            var manager = new SubDesignerSerializationManager();
            int callCount = 0;
            ResolveNameEventHandler handler = (sender, e) =>
            {
                Assert.Same(manager, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            IDisposable session = manager.CreateSession();
            manager.ResolveName += handler;
            manager.OnResolveName(eventArgs);
            Assert.Equal(1, callCount);
            session.Dispose();
            
            // Call again.
            session = manager.CreateSession();
            manager.OnResolveName(eventArgs);
            Assert.Equal(1, callCount);
            session.Dispose();

            // Remove handler.
            session = manager.CreateSession();
            manager.ResolveName += handler;
            manager.ResolveName -= handler;
            Assert.Equal(1, callCount);
            session.Dispose();
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        
        public void OnSessionDisposed_InvokeWithSessionDisposed_CallsHandler(EventArgs eventArgs)
        {
            var manager = new SubDesignerSerializationManager();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(manager, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            manager.SessionDisposed += handler;

            manager.OnSessionDisposed(eventArgs);
            Assert.Equal(1, callCount);

            // Call again.
            manager.OnSessionDisposed(eventArgs);
            Assert.Equal(2, callCount);

            // Remove handler.
            manager.SessionDisposed -= handler;
            manager.OnSessionDisposed(eventArgs);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DesignerSerializationManager_OnSessionDisposed_InvokeWithSerializationComplete_CallsHandler(EventArgs eventArgs)
        {
            var manager = new SubDesignerSerializationManager();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(manager, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            IDisposable session = manager.CreateSession();
            manager.SerializationComplete += handler;
            manager.OnSessionDisposed(eventArgs);
            Assert.Equal(1, callCount);
            session.Dispose();

            // Call again.
            session = manager.CreateSession();
            session.Dispose();
            Assert.Equal(1, callCount);

            // Remove handler.
            session = manager.CreateSession();
            manager.SerializationComplete += handler;
            manager.SerializationComplete -= handler;
            session.Dispose();
            Assert.Equal(1, callCount);
        }

        private class SubDesignerSerializationManager : DesignerSerializationManager
        {
            public SubDesignerSerializationManager() : base()
            {
            }

            public SubDesignerSerializationManager(IServiceProvider provider) : base(provider)
            {
            }

            public event ResolveNameEventHandler ResolveName
            {
                add => ((IDesignerSerializationManager)this).ResolveName += value;
                remove => ((IDesignerSerializationManager)this).ResolveName -= value;
            }

            public event EventHandler SerializationComplete
            {
                add => ((IDesignerSerializationManager)this).SerializationComplete += value;
                remove => ((IDesignerSerializationManager)this).SerializationComplete -= value;
            }

            public ContextStack Context => ((IDesignerSerializationManager)this).Context;

            public PropertyDescriptorCollection Properties => ((IDesignerSerializationManager)this).Properties;

            public new object GetService(Type serviceType) => base.GetService(serviceType);

            public new Type GetType(string typeName) => base.GetType(typeName);

            public new void OnResolveName(ResolveNameEventArgs e) => base.OnResolveName(e);

            public new void OnSessionDisposed(EventArgs e) => base.OnSessionDisposed(e);
        }

        private class PropertyProvider
        {
            [Category("Category")]
            [Description("Description")]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            [DesignOnly(true)]
            [DisplayName("DisplayName")]
            [Localizable(true)]
            [TypeConverter(typeof(Int64Converter))]
            public int Value { get; set; }
        }

        private class OtherPropertyProvider
        {
            public int OtherValue { get; set; }
        }
    }
}
