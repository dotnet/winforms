// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ErrorProviderTests
    {
        [Fact]
        public void ErrorProvider_Constructor()
        {
            var ep = new ErrorProvider();

            Assert.NotNull(ep);
            Assert.NotNull(ep.Icon);
        }
        
        [Fact]
        public void ErrorProvider_ConstructorContainerControl()
        {
            var parent = new ContainerControl();

            var ep = new ErrorProvider(parent);

            Assert.NotNull(ep);
            Assert.NotNull(ep.Icon);
        }
        
        [Fact]
        public void ErrorProvider_ConstructorIContainer()
        {
            IContainer nullContainer = null;
            var mockContainer = new Mock<IContainer>(MockBehavior.Strict);
            mockContainer.Setup(x => x.Add(It.IsAny<ErrorProvider>())).Verifiable();

            // act & assert
            var ex = Assert.Throws<ArgumentNullException>(() => new ErrorProvider(nullContainer));
            Assert.Equal("container", ex.ParamName);

            var ep = new ErrorProvider(mockContainer.Object);
            Assert.NotNull(ep);
            mockContainer.Verify(x => x.Add(ep));
        }
    }
}
