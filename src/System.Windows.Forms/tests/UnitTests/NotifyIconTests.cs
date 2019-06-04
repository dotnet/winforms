// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class NotifyIconTests
    {
        [Fact]
        public void NotifyIcon_Constructor()
        {
            var icon = new NotifyIcon();

            Assert.NotNull(icon);
        }

        [Fact]
        public void NotifyIcon_ConstructorIContainer()
        {
            IContainer nullContainer = null;
            var mockContainer = new Mock<IContainer>(MockBehavior.Strict);
            mockContainer.Setup(x => x.Add(It.IsAny<NotifyIcon>())).Verifiable();

            // act & assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new NotifyIcon(nullContainer));
            Assert.Equal("container", ex.ParamName);

            var icon = new NotifyIcon(mockContainer.Object);
            Assert.NotNull(icon);
            mockContainer.Verify(x => x.Add(icon));
        }
    }
}
