// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignSurfaceTests
    {
        [Fact]
        public void DesignSurface_Ctor_Default()
        {
            var surface = new SubDesignSurface();
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
            Assert.Null(surface.Host.TransactionDescription);
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.NotNull(surface.ServiceContainer);
            Assert.Throws<InvalidOperationException>(() => surface.View);
        }

        public static IEnumerable<object[]> Ctor_IServiceProvider_TestData()
        {
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ContainerFilterService)))
                .Returns(null);
            yield return new object[] { null };
            yield return new object[] { mockServiceProvider.Object };
        }

        [Theory]
        [MemberData(nameof(Ctor_IServiceProvider_TestData))]
        public void DesignSurface_Ctor_IServiceProvider(IServiceProvider parentProvider)
        {
            var surface = new SubDesignSurface(parentProvider);
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
            Assert.Null(surface.Host.TransactionDescription);
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.NotNull(surface.ServiceContainer);
            Assert.Throws<InvalidOperationException>(() => surface.View);
        }

        [Fact]
        public void DesignSurface_ComponentContainer_GetDisposed_ThrowsObjectDisposedException()
        {
            var surface = new DesignSurface();
            surface.Dispose();
            Assert.Throws<ObjectDisposedException>(() => surface.ComponentContainer);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void DesignSurface_DtelLoading_Set_GetReturnsExpected(bool value)
        {
            var surface = new DesignSurface()
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

        [Fact]
        public void DesignSurface_ServiceContainer_Get_ReturnsSame()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.NotNull(container);
            Assert.Same(container, surface.ServiceContainer);
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetISelectionService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            ISelectionService service = Assert.IsAssignableFrom<ISelectionService>(container.GetService(typeof(ISelectionService)));
            Assert.Null(service.PrimarySelection);
            Assert.Equal(0, service.SelectionCount);
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetIExtenderProviderService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<IExtenderProviderService>(container.GetService(typeof(IExtenderProviderService)));
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetIExtenderListService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<IExtenderListService>(container.GetService(typeof(IExtenderListService)));
            Assert.IsAssignableFrom<IExtenderProviderService>(container.GetService(typeof(IExtenderListService)));
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetITypeDescriptorFilterService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<ITypeDescriptorFilterService>(container.GetService(typeof(ITypeDescriptorFilterService)));
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetIReferenceService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<IReferenceService>(container.GetService(typeof(IReferenceService)));
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetDesignSurfaceService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.Same(surface, container.GetService(typeof(DesignSurface)));
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetInstanceTypeService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
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

        [Theory]
        [MemberData(nameof(ServiceContainer_FixedService_TestData))]
        public void DesignSurface_ServiceContainer_GetFixedService_ReturnsExpected(Type serviceType)
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.Same(surface.Host, container.GetService(serviceType));
        }

        [Theory]
        [MemberData(nameof(ServiceContainer_FixedService_TestData))]
        public void DesignSurface_ServiceContainer_RemoveFixedService_ThrowsInvalidOperationException(Type serviceType)
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.Throws<InvalidOperationException>(() => container.RemoveService(serviceType));
        }

        [Theory]
        [InlineData(typeof(ISelectionService))]
        [InlineData(typeof(IExtenderProviderService))]
        [InlineData(typeof(IExtenderListService))]
        [InlineData(typeof(ITypeDescriptorFilterService))]
        [InlineData(typeof(IReferenceService))]
        [InlineData(typeof(DesignSurface))]
        public void DesignSurface_ServiceContainer_RemoveNonFixedServiceType_ThrowsArgumentNullException(Type serviceType)
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.NotNull(container.GetService(serviceType));
            container.RemoveService(serviceType);
            Assert.Null(container.GetService(serviceType));

            // Remove again.
            container.RemoveService(serviceType);
            Assert.Null(container.GetService(serviceType));
        }

        [Fact]
        public void DesignSurface_ServiceContainer_RemoveNullServiceType_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.Throws<ArgumentNullException>("serviceType", () => container.RemoveService(null));
        }

        [Fact]
        public void DesignSurface_ServiceContainer_GetDisposed_ThrowsObjectDisposedException()
        {
            var surface = new SubDesignSurface();
            surface.Dispose();
            Assert.Throws<ObjectDisposedException>(() => surface.ServiceContainer);
        }

        [Fact]
        public void DesignSurface_View_GetDisposed_ThrowsObjectDisposedException()
        {
            var surface = new DesignSurface();
            surface.Dispose();
            Assert.Throws<ObjectDisposedException>(() => surface.View);
        }

        public static IEnumerable<object[]> BeginLoad_TestData()
        {
            yield return new object[] { null };

            var nullMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            nullMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(null);
            yield return new object[] { nullMockServiceProvider.Object };

            var invalidMockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            invalidMockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(new object());
            yield return new object[] { invalidMockServiceProvider.Object };

            var mockDesignerEventService = new Mock<IDesignerEventService>(MockBehavior.Strict);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(mockDesignerEventService.Object);
            yield return new object[] { mockServiceProvider.Object };
        }

        [Theory]
        [MemberData(nameof(BeginLoad_TestData))]
        public void DesignSurface_BeginLoad_Invoke_Success(IServiceProvider parentProvider)
        {
            var surface = new SubDesignSurface(parentProvider);
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host))
                .Verifiable();
            surface.BeginLoad(mockLoader.Object);
            Assert.False(surface.IsLoaded);
            Assert.Empty(surface.LoadErrors);
            Assert.True(surface.Host.Loading);
            mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        }

        [Fact]
        public void DesignSurface_BeginLoad_ThrowsException_SetsLoadErrors()
        {
            var exception = new Exception();
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void DesignSurface_BeginLoad_ThrowsExceptionWithoutMessage_SetsLoadErrors(string message)
        {
            var mockException = new Mock<Exception>(MockBehavior.Strict);
            mockException
                .Setup(e => e.Message)
                .Returns(message);
            mockException
                .Setup(e => e.ToString())
                .Returns("ExceptionText");
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host))
                .Throws(mockException.Object);
            mockLoader
                .Setup(l => l.Loading)
                .Returns(false);
            surface.BeginLoad(mockLoader.Object);
            Assert.False(surface.IsLoaded);
            Exception error = Assert.IsType<Exception>(Assert.Single(surface.LoadErrors));
            Assert.Contains("ExceptionText", error.Message);
            Assert.False(surface.Host.Loading);
        }

        [Fact]
        public void DesignSurface_BeginLoad_ThrowsTargetInvocationException_SetsLoadErrors()
        {
            var exception = new Exception();
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
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

        [Fact]
        public void DesignSurface_BeginLoad_InvokeWithLoading_CallsHandler()
        {
            var surface = new SubDesignSurface();
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

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
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
            
            // Begin again.
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

        [Fact]
        public void DesignSurface_BeginLoad_InvokeErrorWithUnloading_CallsHandler()
        {
            var surface = new SubDesignSurface();
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

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host))
                .Throws(new Exception())
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
            
            // Begin again.
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

        [Fact]
        public void DesignSurface_BeginLoad_InvokeWithoutIDesignerEventServiceWithActivated_CallsHandler()
        {
            var surface = new SubDesignSurface();
            int callCount = 0;
            IDesignerLoaderHost2 host = surface.Host;
            host.Activated += (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host))
                .Verifiable();
            surface.BeginLoad(mockLoader.Object);
            Assert.Equal(1, callCount);
            mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        }

        [Fact]
        public void DesignSurface_BeginLoad_InvokeWithIDesignerEventServiceWithActivated_DoesNotCallHandler()
        {
            var mockDesignerEventService = new Mock<IDesignerEventService>(MockBehavior.Strict);
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerEventService)))
                .Returns(mockDesignerEventService.Object);
            var surface = new SubDesignSurface(mockServiceProvider.Object);
            IDesignerLoaderHost2 host = surface.Host;
            int callCount = 0;
            host.Activated += (sender, e) =>
            {
                Assert.Same(host, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host))
                .Verifiable();
            surface.BeginLoad(mockLoader.Object);
            Assert.Equal(0, callCount);
            mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        }

        [Fact]
        public void DesignSurface_BeginLoad_DisposeInLoading_DoesCallFlush()
        {
            var surface = new SubDesignSurface();
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

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            surface.BeginLoad(mockLoader.Object);
            Assert.Equal(1, loadingCallCount);
            Assert.Equal(0, loadedCallCount);
            Assert.Equal(0, unloadingCallCount);
            Assert.Equal(0, unloadedCallCount);
            Assert.Equal(0, flushedCallCount);
        }

        [Fact]
        public void DesignSurface_BeginLoad_DisposeInBeginLoad_DoesCallFlush()
        {
            var surface = new SubDesignSurface();
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

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
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

        [Fact]
        public void DesignSurface_BeginLoad_DisposeInBeginLoadThrowsException_DoesCallFlush()
        {
            var surface = new SubDesignSurface();
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

            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
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

                    throw new Exception();
                });
            surface.BeginLoad(mockLoader.Object);
            Assert.Equal(1, loadingCallCount);
            Assert.Equal(0, loadedCallCount);
            Assert.Equal(0, unloadingCallCount);
            Assert.Equal(0, unloadedCallCount);
            Assert.Equal(0, flushedCallCount);
            mockLoader.Verify(l => l.BeginLoad(host), Times.Once());
        }
        
        [Fact]
        public void DesignSurface_BeginLoad_NullLoader_ThrowsArgumentNullException()
        {
            var surface = new DesignSurface();
            Assert.Throws<ArgumentNullException>("loader", () => surface.BeginLoad((DesignerLoader)null));
        }

        [Fact]
        public void DesignSurface_BeginLoad_AlreadyCalled_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host));
            surface.BeginLoad(mockLoader.Object);
            var otherMockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            otherMockLoader
                .Setup(l => l.BeginLoad(host));
            surface.BeginLoad(mockLoader.Object);
            Assert.Throws<InvalidOperationException>(() => surface.BeginLoad(otherMockLoader.Object));
        }

        [Fact]
        public void DesignSurface_BeginLoad_Disposed_ThrowsObjectDisposedException()
        {
            var surface = new DesignSurface();
            surface.Dispose();
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            Assert.Throws<ObjectDisposedException>(() => surface.BeginLoad(mockLoader.Object));
        }

        [Fact]
        public void DesignSurface_CreateDesigner_InvokeNoDesigner_ReturnsExpected()
        {
            var surface = new DesignSurface();
            var component = new Component();
            Assert.Null(surface.CreateDesigner(component, rootDesigner: true));
            Assert.Null(surface.CreateDesigner(component, rootDesigner: false));
        }

        [Fact]
        public void DesignSurface_CreateDesigner_InvokeIDesigner_ReturnsExpected()
        {
            var surface = new DesignSurface();
            var component = new DesignerComponent();
            Assert.Null(surface.CreateDesigner(component, rootDesigner: true));
            Assert.IsType<ComponentDesigner>(surface.CreateDesigner(component, rootDesigner: false));
        }

        [Fact]
        public void DesignSurface_CreateDesigner_InvokeIRootDesigner_ReturnsExpected()
        {
            var surface = new DesignSurface();
            var component = new RootDesignerComponent();
            Assert.IsType<RootComponentDesigner>(surface.CreateDesigner(component, rootDesigner: true));
            Assert.Null(surface.CreateDesigner(component, rootDesigner: false));
        }

        [Fact]
        public void DesignSurface_CreateDesigner_NullComponent_ThrowsArgumentNullException()
        {
            var surface = new DesignSurface();
            Assert.Throws<ArgumentNullException>("component", () => surface.CreateDesigner(null, true));
            Assert.Throws<ArgumentNullException>("component", () => surface.CreateDesigner(null, false));
        }

        [Fact]
        public void DesignSurface_CreateDesigner_Disposed_ThrowsObjectDisposedException()
        {
            var surface = new DesignSurface();
            surface.Dispose();
            Assert.Throws<ObjectDisposedException>(() => surface.CreateDesigner(new Component(), true));
            Assert.Throws<ObjectDisposedException>(() => surface.CreateDesigner(new Component(), false));
        }

        [Fact]
        public void DesignSurface_Dispose_MultipleTimes_Success()
        {
            var surface = new DesignSurface();
            surface.Dispose();
            surface.Dispose();
        }

        [Fact]
        public void DesignSurface_Dispose_HasHost_ThrowsInvalidOperationException()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
            mockLoader
                .Setup(l => l.BeginLoad(host))
                .Verifiable();
            surface.BeginLoad(mockLoader.Object);
            Assert.Throws<InvalidOperationException>(() => surface.Dispose());

            // Should not throw again.
            surface.Dispose();
        }

        [Fact]
        public void DesignSurface_Flush_InvokeWithHostWithLoader_CallsLoaderFlush()
        {
            var surface = new SubDesignSurface();
            IDesignerLoaderHost2 host = surface.Host;
            var mockLoader = new Mock<DesignerLoader>(MockBehavior.Strict);
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

        [Fact]
        public void DesignSurface_Flush_InvokeWithHostWithoutLoader_Nop()
        {
            var surface = new DesignSurface();
            surface.Flush();

            // Flush again.
            surface.Flush();
        }

        [Fact]
        public void DesignSurface_Flush_InvokeDisposed_Nop()
        {
            var surface = new DesignSurface();
            surface.Dispose();
            surface.Flush();

            // Flush again.
            surface.Flush();
        }

        [Fact]
        public void DesignSurface_Flush_InvokeWithFlushed_CallsHandler()
        {
            var surface = new DesignSurface();
            int callCount = 0;
            surface.Flushed += (sender, e) =>
            {
                Assert.Same(surface, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };

            surface.Flush();
            Assert.Equal(1, callCount);

            // Fush again.
            surface.Flush();
            Assert.Equal(2, callCount);

            // Fush when disposed.
            surface.Dispose();
            surface.Flush();
            Assert.Equal(3, callCount);
        }

        [Fact]
        public void DesignSurface_GetService_InvokeWithServiceProvider_ReturnsExpected()
        {
            var service = new object();
            var mockServiceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(int)))
                .Returns(service)
                .Verifiable();
            var surface = new DesignSurface(mockServiceProvider.Object);
            Assert.Same(service, surface.GetService(typeof(int)));
            mockServiceProvider.Verify(p => p.GetService(typeof(int)), Times.Once());
        }

        [Fact]
        public void DesignSurface_GetService_InvokeWithoutServiceProvider_ReturnsNull()
        {
            var surface = new DesignSurface();
            Assert.Null(surface.GetService(typeof(int)));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DesignSurface_OnLoading_Invoke_Success(EventArgs eventArgs)
        {
            var surface = new SubDesignSurface();

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

        [Theory]
        [MemberData(nameof(LoadedEventArgs_TestData))]
        public void DesignSurface_OnLoaded_Invoke_Success(LoadedEventArgs eventArgs)
        {
            var surface = new SubDesignSurface();

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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DesignSurface_OnUnloaded_Invoke_Success(EventArgs eventArgs)
        {
            var surface = new SubDesignSurface();

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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DesignSurface_OnUnloading_Invoke_Success(EventArgs eventArgs)
        {
            var surface = new SubDesignSurface();

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

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void DesignSurface_OnViewActivate_Invoke_Success(EventArgs eventArgs)
        {
            var surface = new SubDesignSurface();

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

            public new ServiceContainer ServiceContainer => base.ServiceContainer;

            public IDesignerLoaderHost2 Host => Assert.IsAssignableFrom<IDesignerLoaderHost2>(ComponentContainer);

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
            public ViewTechnology[] SupportedTechnologies { get; }
            public object GetView(ViewTechnology technology) => throw new NotImplementedException();
        }

        [Designer(typeof(RootComponentDesigner), typeof(IRootDesigner))]
        private class RootDesignerComponent : Component
        {
        }
    }
}
