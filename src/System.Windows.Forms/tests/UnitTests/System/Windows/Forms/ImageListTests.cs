// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ImageListTests
    {
        [Fact]
        public void ImageList_Ctor_Default()
        {
            var list = new ImageList();
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Equal(new Size(16, 16), list.ImageSize);
        }

        [Fact]
        public void ImageList_Ctor_IContainer()
        {
            var mockContainer = new Mock<IContainer>(MockBehavior.Strict);
            mockContainer.Setup(c => c.Add(It.IsAny<ImageList>())).Verifiable();

            var list = new ImageList(mockContainer.Object);
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Equal(new Size(16, 16), list.ImageSize);

            mockContainer.Verify(c => c.Add(list));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ColorDepth_SetWithoutHandle_GetReturnsExpected(ColorDepth value)
        {
            var list = new ImageList
            {
                ColorDepth = value
            };
            Assert.Equal(value, list.ColorDepth);

            // Set same.
            list.ColorDepth = value;
            Assert.Equal(value, list.ColorDepth);
        }

        [Fact]
        public void ImageList_ColorDepth_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
        {
            var list = new ImageList();

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(list, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            list.RecreateHandle += handler;

            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(0, callCount);

            // Set same.
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(0, callCount);

            // Set different.
            list.ColorDepth = ColorDepth.Depth16Bit;
            Assert.Equal(ColorDepth.Depth16Bit, list.ColorDepth);
            Assert.Equal(0, callCount);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(0, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ColorDepth_SetWithHandle_GetReturnsExpected(ColorDepth value)
        {
            var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            list.ColorDepth = value;
            Assert.Equal(value, list.ColorDepth);

            // Set same.
            list.ColorDepth = value;
            Assert.Equal(value, list.ColorDepth);
        }

        [Fact]
        public void ImageList_ColorDepth_SetWithHandleWithHandler_CallsRecreateHandle()
        {
            var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(list, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            list.RecreateHandle += handler;

            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(1, callCount);

            // Set same.
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(1, callCount);

            // Set different.
            list.ColorDepth = ColorDepth.Depth16Bit;
            Assert.Equal(ColorDepth.Depth16Bit, list.ColorDepth);
            Assert.Equal(2, callCount);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ColorDepth))]
        public void ImageList_ColorDepth_SetInvalid_ThrowsInvalidEnumArgumentException(ColorDepth value)
        {
            var list = new ImageList();
            Assert.Throws<InvalidEnumArgumentException>("value", () => list.ColorDepth = value);
        }

        public static IEnumerable<object[]> ImageSize_TestData()
        {
            yield return new object[] { new Size(16, 16) };
            yield return new object[] { new Size(17, 16) };
            yield return new object[] { new Size(16, 17) };
            yield return new object[] { new Size(24, 25) };
            yield return new object[] { new Size(256, 26) };
        }

        [Theory]
        [MemberData(nameof(ImageSize_TestData))]
        public void ImageList_ImageSize_SetWithoutHandle_GetReturnsExpected(Size value)
        {
            var list = new ImageList
            {
                ImageSize = value
            };
            Assert.Equal(value, list.ImageSize);

            // Set same.
            list.ImageSize = value;
            Assert.Equal(value, list.ImageSize);
        }

        [Fact]
        public void ImageList_ImageSize_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
        {
            var list = new ImageList();

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(list, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            list.RecreateHandle += handler;

            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(0, callCount);

            // Set same.
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(0, callCount);

            // Set different.
            list.ImageSize = new Size(11, 11);
            Assert.Equal(new Size(11, 11), list.ImageSize);
            Assert.Equal(0, callCount);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(0, callCount);
        }

        [Theory]
        [MemberData(nameof(ImageSize_TestData))]
        public void ImageList_ImageSize_SetWithHandle_GetReturnsExpected(Size value)
        {
            var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            list.ImageSize = value;
            Assert.Equal(value, list.ImageSize);

            // Set same.
            list.ImageSize = value;
            Assert.Equal(value, list.ImageSize);
        }

        [Fact]
        public void ImageList_ImageSize_SetWithHandleWithHandler_CallsRecreateHandle()
        {
            var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(list, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            list.RecreateHandle += handler;

            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(1, callCount);

            // Set same.
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(1, callCount);

            // Set different.
            list.ImageSize = new Size(11, 11);
            Assert.Equal(new Size(11, 11), list.ImageSize);
            Assert.Equal(2, callCount);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void ImageList_ImageSize_SetEmpty__ThrowsArgumentException()
        {
            var list = new ImageList();
            Assert.Throws<ArgumentException>("value", () => list.ImageSize = Size.Empty);
        }

        public static IEnumerable<object[]> ImageList_SetInvalidDimension_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { -1 };
            yield return new object[] { 257 };
        }

        [Theory]
        [MemberData(nameof(ImageList_SetInvalidDimension_TestData))]
        public void ImageList_ImageSize_SetInvalidWidth_ThrowsArgumentOutOfRangeException(int width)
        {
            var list = new ImageList();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => list.ImageSize = new Size(width, 1));
        }

        [Theory]
        [MemberData(nameof(ImageList_SetInvalidDimension_TestData))]
        public void ImageList_ImageSize_SetInvalidHeight_ThrowsArgumentOutOfRangeException(int width)
        {
            var list = new ImageList();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => list.ImageSize = new Size(width, 1));
        }
    }
}
