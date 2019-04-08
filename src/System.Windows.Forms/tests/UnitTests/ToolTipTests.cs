// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Tests
{
    public class ToolTipTests
    {
        [Fact]
        public void ToolTip_Constructor()
        {
            var tt = new ToolTip();

            Assert.NotNull(tt);
        }

        [Fact]
        public void ToolTip_ConstructorIContainer()
        {
            IContainer nullContainer = null;
            var mockContainer = new Mock<IContainer>(MockBehavior.Strict);
            mockContainer.Setup(x => x.Add(It.IsAny<ToolTip>())).Verifiable();

            // act & assert
            var ex = Assert.Throws<ArgumentNullException>(() => new ToolTip(nullContainer));
            Assert.Equal("cont", ex.ParamName);

            var tt = new ToolTip(mockContainer.Object);
            Assert.NotNull(tt);
            mockContainer.Verify(x => x.Add(tt));
        }
    }
}
