// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class DesignSurfaceTests
    {
        [Fact]
        public void ServiceContainer_Get_ReturnsSame()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.NotNull(container);
            Assert.Same(container, surface.ServiceContainer);
        }

        [Fact]
        public void ServiceContainer_GetISelectionService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            ISelectionService service = Assert.IsAssignableFrom<ISelectionService>(container.GetService(typeof(ISelectionService)));
            Assert.Null(service.PrimarySelection);
            Assert.Equal(0, service.SelectionCount);
        }

        [Fact]
        public void ServiceContainer_GetIExtenderProviderService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<IExtenderProviderService>(container.GetService(typeof(IExtenderProviderService)));
        }

        [Fact]
        public void ServiceContainer_GetIExtenderListService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<IExtenderListService>(container.GetService(typeof(IExtenderListService)));
            Assert.IsAssignableFrom<IExtenderProviderService>(container.GetService(typeof(IExtenderListService)));
        }

        [Fact]
        public void ServiceContainer_GetITypeDescriptorFilterService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<ITypeDescriptorFilterService>(container.GetService(typeof(ITypeDescriptorFilterService)));
        }

        [Fact]
        public void ServiceContainer_GetIReferenceService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.IsAssignableFrom<IReferenceService>(container.GetService(typeof(IReferenceService)));
        }

        [Fact]
        public void ServiceContainer_GetDesignSurfaceService_ReturnsExpected()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.Same(surface, container.GetService(typeof(DesignSurface)));
        }

        [Fact]
        public void ServiceContainer_GetInstanceTypeService_ReturnsExpected()
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
        public void ServiceContainer_GetFixedService_ReturnsExpected(Type serviceType)
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.Same(surface.Host, container.GetService(serviceType));
        }

        [Theory]
        [MemberData(nameof(ServiceContainer_FixedService_TestData))]
        public void ServiceContainer_RemoveFixedService_ThrowsInvalidOperationException(Type serviceType)
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
        public void ServiceContainer_RemoveNonFixedServiceType_ThrowsArgumentNullException(Type serviceType)
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
        public void ServiceContainer_RemoveNullServiceType_ThrowsArgumentNullException()
        {
            var surface = new SubDesignSurface();
            ServiceContainer container = surface.ServiceContainer;
            Assert.Throws<ArgumentNullException>("serviceType", () => container.RemoveService(null));
        }

        [Fact]
        public void ServiceContainer_GetDisposed_ThrowsObjectDisposedException()
        {
            var surface = new SubDesignSurface();
            surface.Dispose();
            Assert.Throws<ObjectDisposedException>(() => surface.ServiceContainer);
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
    }
}
