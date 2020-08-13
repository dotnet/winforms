// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class ExtenderProviderServiceTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ExtenderProviderService_GetExtenderProviders_Invoke_ReturnsEmpty()
        {
            var surface = new DesignSurface();
            object service = surface.GetService(typeof(IExtenderListService));
            IExtenderListService listService = Assert.IsAssignableFrom<IExtenderListService>(service);
            Assert.Empty(listService.GetExtenderProviders());
        }

        [Fact]
        public void ExtenderProviderService_AddExtenderProvider_Invoke_Success()
        {
            var surface = new DesignSurface();
            object service = surface.GetService(typeof(IExtenderListService));
            IExtenderListService listService = Assert.IsAssignableFrom<IExtenderListService>(service);
            IExtenderProviderService providerService = Assert.IsAssignableFrom<IExtenderProviderService>(service);
            var mockExtenderProvider1 = new Mock<IExtenderProvider>(MockBehavior.Strict);
            var mockExtenderProvider2 = new Mock<IExtenderProvider>(MockBehavior.Strict);

            providerService.AddExtenderProvider(mockExtenderProvider1.Object);
            Assert.Equal(new IExtenderProvider[] { mockExtenderProvider1.Object }, listService.GetExtenderProviders());

            // Add another.
            providerService.AddExtenderProvider(mockExtenderProvider2.Object);
            Assert.Equal(new IExtenderProvider[] { mockExtenderProvider1.Object, mockExtenderProvider2.Object }, listService.GetExtenderProviders());
        }

        [Fact]
        public void ExtenderProviderService_AddExtenderProvider_NullProvider_ThrowsArgumentNullException()
        {
            var surface = new DesignSurface();
            object service = surface.GetService(typeof(IExtenderListService));
            IExtenderProviderService providerService = Assert.IsAssignableFrom<IExtenderProviderService>(service);
            Assert.Throws<ArgumentNullException>("provider", () => providerService.AddExtenderProvider(null));
        }

        [Fact]
        public void ExtenderProviderService_AddExtenderProvider_DuplicateProvider_ThrowsArgumentException()
        {
            var surface = new DesignSurface();
            object service = surface.GetService(typeof(IExtenderListService));
            IExtenderListService listService = Assert.IsAssignableFrom<IExtenderListService>(service);
            IExtenderProviderService providerService = Assert.IsAssignableFrom<IExtenderProviderService>(service);
            var mockExtenderProvider = new Mock<IExtenderProvider>(MockBehavior.Strict);

            providerService.AddExtenderProvider(mockExtenderProvider.Object);
            Assert.Throws<ArgumentException>("provider", () => providerService.AddExtenderProvider(mockExtenderProvider.Object));
        }

        [Fact]
        public void ExtenderProviderService_RemoveExtenderProvider_InvokeWithProviders_Success()
        {
            var surface = new DesignSurface();
            object service = surface.GetService(typeof(IExtenderListService));
            IExtenderListService listService = Assert.IsAssignableFrom<IExtenderListService>(service);
            IExtenderProviderService providerService = Assert.IsAssignableFrom<IExtenderProviderService>(service);
            var mockExtenderProvider1 = new Mock<IExtenderProvider>(MockBehavior.Strict);
            var mockExtenderProvider2 = new Mock<IExtenderProvider>(MockBehavior.Strict);

            providerService.AddExtenderProvider(mockExtenderProvider1.Object);
            providerService.AddExtenderProvider(mockExtenderProvider2.Object);

            providerService.RemoveExtenderProvider(mockExtenderProvider1.Object);
            Assert.Equal(new IExtenderProvider[] { mockExtenderProvider2.Object }, listService.GetExtenderProviders());

            // Remove again.
            providerService.RemoveExtenderProvider(mockExtenderProvider1.Object);
            Assert.Equal(new IExtenderProvider[] { mockExtenderProvider2.Object }, listService.GetExtenderProviders());

            // Remove other.
            providerService.RemoveExtenderProvider(mockExtenderProvider2.Object);
            Assert.Empty(listService.GetExtenderProviders());
        }

        [Fact]
        public void ExtenderProviderService_RemoveExtenderProvider_InvokeWithoutProviders_Nop()
        {
            var surface = new DesignSurface();
            object service = surface.GetService(typeof(IExtenderListService));
            IExtenderListService listService = Assert.IsAssignableFrom<IExtenderListService>(service);
            IExtenderProviderService providerService = Assert.IsAssignableFrom<IExtenderProviderService>(service);
            var mockExtenderProvider = new Mock<IExtenderProvider>(MockBehavior.Strict);
            providerService.RemoveExtenderProvider(mockExtenderProvider.Object);
            Assert.Empty(listService.GetExtenderProviders());
        }

        [Fact]
        public void ExtenderProviderService_RemoveExtenderProvider_NullProvider_ThrowsArgumentNullException()
        {
            var surface = new DesignSurface();
            object service = surface.GetService(typeof(IExtenderListService));
            IExtenderProviderService providerService = Assert.IsAssignableFrom<IExtenderProviderService>(service);
            Assert.Throws<ArgumentNullException>("provider", () => providerService.RemoveExtenderProvider(null));
        }
    }
}
