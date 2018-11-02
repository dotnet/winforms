﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ErrorProviderTests
    {
        [Fact(Skip = "Need to fix: Error.ico not found error")]
        public void Constructor()
        {
            // act
            var ep = new ErrorProvider();

            // assert
            Assert.NotNull(ep);
            Assert.NotNull(ep.Icon);
        }

        [Fact(Skip = "Need to fix: Error.ico not found error")]
        public void ConstructorContainerControl()
        {
            // arrange
            var parent = new ContainerControl();

            // act
            var ep = new ErrorProvider(parent);

            // assert
            Assert.NotNull(ep);
            Assert.NotNull(ep.Icon);
        }

        [Fact(Skip = "Need to fix: Error.ico not found error")]
        public void ConstructorIContainer()
        {
            // arrange
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
