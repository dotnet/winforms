// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ContextMenuStripTests
    {
        [Fact]
        public void ContextMenuStrip_Constructor()
        {
            var cms = new ContextMenuStrip();

            Assert.NotNull(cms);
        }

        [Fact]
        public void ContextMenuStrip_ConstructorIContainer()
        {
            IContainer nullContainer = null;
            var mockContainer = new Mock<IContainer>(MockBehavior.Strict);
            mockContainer.Setup(x => x.Add(It.IsAny<ContextMenuStrip>())).Verifiable();

            // act & assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new ContextMenuStrip(nullContainer));
            Assert.Equal("container", ex.ParamName);

            var cms = new ContextMenuStrip(mockContainer.Object);
            Assert.NotNull(cms);
            mockContainer.Verify(x => x.Add(cms));
        }
    }
}
