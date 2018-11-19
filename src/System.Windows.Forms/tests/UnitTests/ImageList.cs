// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ImageListTests
    {
        [Fact]
        public void ImageList_Constructor()
        {
            var il = new ImageList();

            Assert.NotNull(il);
        }

        [Fact]
        public void ImageList_ConstructorIContainer()
        {
            IContainer nullContainer = null;
            var mockContainer = new Mock<IContainer>(MockBehavior.Strict);
            mockContainer.Setup(x => x.Add(It.IsAny<ImageList>())).Verifiable();

            // act & assert
            var ex = Assert.Throws<ArgumentNullException>(() => new ImageList(nullContainer));
            Assert.Equal("container", ex.ParamName);

            var il = new ImageList(mockContainer.Object);
            Assert.NotNull(il);
            mockContainer.Verify(x => x.Add(il));
        }
    }
}
