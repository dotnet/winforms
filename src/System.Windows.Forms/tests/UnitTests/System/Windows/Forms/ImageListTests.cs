// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
            using var list = new ImageList();
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Null(list.Container);
            Assert.Empty(list.Images);
            Assert.Same(list.Images, list.Images);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.Null(list.ImageStream);
            Assert.Null(list.Site);
            Assert.Null(list.Tag);
            Assert.Equal(Color.Transparent, list.TransparentColor);

            Assert.False(list.HandleCreated);
        }

        [Fact]
        public void ImageList_Ctor_IContainer()
        {
            var container = new Container();
            using var list = new ImageList(container);
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Same(container, list.Container);
            Assert.Empty(list.Images);
            Assert.Same(list.Images, list.Images);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.Null(list.ImageStream);
            Assert.NotNull(list.Site);
            Assert.Null(list.Tag);
            Assert.Equal(Color.Transparent, list.TransparentColor);

            Assert.False(list.HandleCreated);
        }

        [Fact]
        public void ImageList_Ctor_NullContainer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("container", () => new ImageList(null));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ColorDepth_SetWithoutHandle_GetReturnsExpected(ColorDepth value)
        {
            using var list = new ImageList
            {
                ColorDepth = value
            };
            Assert.Equal(value, list.ColorDepth);
            Assert.False(list.HandleCreated);

            // Set same.
            list.ColorDepth = value;
            Assert.Equal(value, list.ColorDepth);
            Assert.False(list.HandleCreated);
        }

        [Fact]
        public void ImageList_ColorDepth_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
        {
            using var list = new ImageList();
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            list.RecreateHandle += handler;

            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.False(list.HandleCreated);
            Assert.Equal(0, callCount);

            // Set same.
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.False(list.HandleCreated);
            Assert.Equal(0, callCount);

            // Set different.
            list.ColorDepth = ColorDepth.Depth16Bit;
            Assert.Equal(ColorDepth.Depth16Bit, list.ColorDepth);
            Assert.False(list.HandleCreated);
            Assert.Equal(0, callCount);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.False(list.HandleCreated);
            Assert.Equal(0, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ColorDepth_SetWithHandle_GetReturnsExpected(ColorDepth value)
        {
            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            list.ColorDepth = value;
            Assert.Equal(value, list.ColorDepth);
            Assert.True(list.HandleCreated);

            // Set same.
            list.ColorDepth = value;
            Assert.Equal(value, list.ColorDepth);
            Assert.True(list.HandleCreated);
        }

        [Fact]
        public void ImageList_ColorDepth_SetWithHandleWithHandler_CallsRecreateHandle()
        {
            using var list = new ImageList();
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
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Set same.
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(1, callCount);
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Set different.
            list.ColorDepth = ColorDepth.Depth16Bit;
            Assert.Equal(ColorDepth.Depth16Bit, list.ColorDepth);
            Assert.Equal(2, callCount);
            Assert.True(list.HandleCreated);
            IntPtr handle3 = list.Handle;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ColorDepth = ColorDepth.Depth24Bit;
            Assert.Equal(ColorDepth.Depth24Bit, list.ColorDepth);
            Assert.Equal(2, callCount);
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);
        }

        [Fact]
        public void ImageList_ColorDepth_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ColorDepth)];
            using var list = new ImageList();
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.True(property.CanResetValue(list));

            list.ColorDepth = ColorDepth.Depth32Bit;
            Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
            Assert.True(property.CanResetValue(list));

            property.ResetValue(list);
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.True(property.CanResetValue(list));
        }

        [Fact]
        public void ImageList_ColorDepth_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ColorDepth)];
            using var list = new ImageList();
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.True(property.ShouldSerializeValue(list));

            list.ColorDepth = ColorDepth.Depth32Bit;
            Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
            Assert.True(property.ShouldSerializeValue(list));

            property.ResetValue(list);
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.True(property.ShouldSerializeValue(list));

            // With images.
            list.Images.Add(new Bitmap(10, 10));
            Assert.False(property.ShouldSerializeValue(list));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_Handle_Get_CreatesHandle(ColorDepth colorDepth)
        {
            using var list = new ImageList
            {
                ColorDepth = colorDepth
            };
            IntPtr handle = list.Handle;
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, handle);
            Assert.Equal(handle, list.Handle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ColorDepth))]
        public void ImageList_ColorDepth_SetInvalid_ThrowsInvalidEnumArgumentException(ColorDepth value)
        {
            using var list = new ImageList();
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
            using var list = new ImageList
            {
                ImageSize = value
            };
            Assert.Equal(value, list.ImageSize);
            Assert.False(list.HandleCreated);

            // Set same.
            list.ImageSize = value;
            Assert.Equal(value, list.ImageSize);
            Assert.False(list.HandleCreated);
        }

        [Fact]
        public void ImageList_ImageSize_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
        {
            using var list = new ImageList();
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            list.RecreateHandle += handler;

            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);

            // Set same.
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);

            // Set different.
            list.ImageSize = new Size(11, 11);
            Assert.Equal(new Size(11, 11), list.ImageSize);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);
        }

        [Theory]
        [MemberData(nameof(ImageSize_TestData))]
        public void ImageList_ImageSize_SetWithHandle_GetReturnsExpected(Size value)
        {
            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            list.ImageSize = value;
            Assert.Equal(value, list.ImageSize);
            Assert.True(list.HandleCreated);

            // Set same.
            list.ImageSize = value;
            Assert.Equal(value, list.ImageSize);
            Assert.True(list.HandleCreated);
        }

        [Fact]
        public void ImageList_ImageSize_SetWithHandleWithHandler_CallsRecreateHandle()
        {
            using var list = new ImageList();
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
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Set same.
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(1, callCount);
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Set different width.
            list.ImageSize = new Size(11, 10);
            Assert.Equal(new Size(11, 10), list.ImageSize);
            Assert.Equal(2, callCount);
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Set different height.
            list.ImageSize = new Size(11, 11);
            Assert.Equal(new Size(11, 11), list.ImageSize);
            Assert.Equal(3, callCount);
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Set different width and height.
            list.ImageSize = new Size(12, 12);
            Assert.Equal(new Size(12, 12), list.ImageSize);
            Assert.Equal(4, callCount);
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.ImageSize = new Size(10, 10);
            Assert.Equal(new Size(10, 10), list.ImageSize);
            Assert.Equal(4, callCount);
            Assert.True(list.HandleCreated);
            Assert.NotEqual(IntPtr.Zero, list.Handle);
        }

        [Fact]
        public void ImageList_ImageSize_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ImageSize)];
            using var list = new ImageList();
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.True(property.CanResetValue(list));

            list.ImageSize = new Size(32, 32);
            Assert.Equal(new Size(32, 32), list.ImageSize);
            Assert.True(property.CanResetValue(list));

            property.ResetValue(list);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.True(property.CanResetValue(list));
        }

        [Fact]
        public void ImageList_ImageSize_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ImageSize)];
            using var list = new ImageList();
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.True(property.ShouldSerializeValue(list));

            list.ImageSize = new Size(32, 32);
            Assert.Equal(new Size(32, 32), list.ImageSize);
            Assert.True(property.ShouldSerializeValue(list));

            property.ResetValue(list);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.True(property.ShouldSerializeValue(list));

            // With images.
            list.Images.Add(new Bitmap(10, 10));
            Assert.False(property.ShouldSerializeValue(list));
        }

        [Fact]
        public void ImageList_ImageSize_SetEmpty__ThrowsArgumentException()
        {
            using var list = new ImageList();
            Assert.Throws<ArgumentException>("value", () => list.ImageSize = Size.Empty);
        }

        public static IEnumerable<object[]> ImageSize_SetInvalidDimension_TestData()
        {
            yield return new object[] { 0 };
            yield return new object[] { -1 };
            yield return new object[] { 257 };
        }

        [Theory]
        [MemberData(nameof(ImageSize_SetInvalidDimension_TestData))]
        public void ImageList_ImageSize_SetInvalidWidth_ThrowsArgumentOutOfRangeException(int width)
        {
            using var list = new ImageList();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => list.ImageSize = new Size(width, 1));
        }

        [Theory]
        [MemberData(nameof(ImageSize_SetInvalidDimension_TestData))]
        public void ImageList_ImageSize_SetInvalidHeight_ThrowsArgumentOutOfRangeException(int width)
        {
            using var list = new ImageList();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => list.ImageSize = new Size(width, 1));
        }

        [Fact]
        public void ImageList_ImageStream_GetWithImages_ReturnsExpected()
        {
            using var list = new ImageList();
            list.Images.Add(new Bitmap(10, 10));
            Assert.NotNull(list.ImageStream);
            Assert.NotSame(list.ImageStream, list.ImageStream);
            Assert.False(list.HandleCreated);
        }

        private static T RoundtripSerialize<T>(T source)
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, source);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ImageStream_SetStreamerSerialized_UpdatesImages(ColorDepth colorDepth)
        {
            using var sourceList = new ImageList
            {
                ColorDepth = colorDepth,
                ImageSize = new Size(32, 32)
            };
            var image = new Bitmap(10, 10);
            sourceList.Images.Add(image);
            ImageListStreamer stream = RoundtripSerialize(sourceList.ImageStream);
            Assert.True(sourceList.HandleCreated);

            using var list = new ImageList();
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            list.RecreateHandle += handler;

            list.ImageStream = stream;
            Assert.Equal(colorDepth, list.ColorDepth);
            Assert.Equal(new Size(32, 32), ((Image)Assert.Single(list.Images)).Size);
            Assert.Equal(new Size(32, 32), list.ImageSize);
            Assert.Equal(0, callCount);
            Assert.True(list.HandleCreated);
            Assert.True(sourceList.HandleCreated);

            // Set same.
            list.ImageStream = stream;
            Assert.Equal(colorDepth, list.ColorDepth);
            Assert.Equal(new Size(32, 32), ((Image)Assert.Single(list.Images)).Size);
            Assert.Equal(new Size(32, 32), list.ImageSize);
            Assert.Equal(1, callCount);
            Assert.True(list.HandleCreated);
            Assert.True(sourceList.HandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ImageStream_SetStreamerSerializedDisposed_Nop(ColorDepth colorDepth)
        {
            using var sourceList = new ImageList
            {
                ColorDepth = colorDepth,
                ImageSize = new Size(32, 32)
            };
            var image = new Bitmap(10, 10);
            sourceList.Images.Add(image);
            ImageListStreamer stream = RoundtripSerialize(sourceList.ImageStream);
            Assert.True(sourceList.HandleCreated);
            stream.Dispose();
            Assert.True(sourceList.HandleCreated);

            using var list = new ImageList
            {
                ImageStream = stream
            };
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Empty(list.Images);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.False(list.HandleCreated);
            Assert.True(sourceList.HandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ImageStream_SetStreamerHasHandleNotSerialized_UpdatesImages(ColorDepth colorDepth)
        {
            using var sourceList = new ImageList
            {
                ColorDepth = colorDepth,
                ImageSize = new Size(32, 32)
            };
            var image = new Bitmap(10, 10);
            sourceList.Images.Add(image);
            Assert.NotEqual(IntPtr.Zero, sourceList.Handle);
            ImageListStreamer stream = sourceList.ImageStream;

            using var list = new ImageList
            {
                ImageStream = stream
            };
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Empty(list.Images);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.False(list.HandleCreated);
            Assert.True(sourceList.HandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ImageStream_SetWithHandleStreamerHasHandleNotSerialized_Nop(ColorDepth colorDepth)
        {
            using var sourceList = new ImageList
            {
                ColorDepth = colorDepth,
                ImageSize = new Size(32, 32)
            };
            var image = new Bitmap(10, 10);
            sourceList.Images.Add(image);
            Assert.NotEqual(IntPtr.Zero, sourceList.Handle);
            ImageListStreamer stream = sourceList.ImageStream;

            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            list.ImageStream = stream;
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Empty(list.Images);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.True(list.HandleCreated);
            Assert.True(sourceList.HandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ImageStream_SetStreamerHasNoHandleNotSerialized_Nop(ColorDepth colorDepth)
        {
            using var sourceList = new ImageList
            {
                ColorDepth = colorDepth,
                ImageSize = new Size(32, 32)
            };
            var image = new Bitmap(10, 10);
            sourceList.Images.Add(image);
            ImageListStreamer stream = sourceList.ImageStream;

            using var list = new ImageList
            {
                ImageStream = stream
            };
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Empty(list.Images);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.False(list.HandleCreated);
            Assert.False(sourceList.HandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageList_ImageStream_SetWithHandleStreamerHasNoHandleNotSerialized_Nop(ColorDepth colorDepth)
        {
            using var sourceList = new ImageList
            {
                ColorDepth = colorDepth,
                ImageSize = new Size(32, 32)
            };
            var image = new Bitmap(10, 10);
            sourceList.Images.Add(image);
            ImageListStreamer stream = sourceList.ImageStream;

            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            list.ImageStream = stream;
            Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
            Assert.Empty(list.Images);
            Assert.Equal(new Size(16, 16), list.ImageSize);
            Assert.True(list.HandleCreated);
            Assert.False(sourceList.HandleCreated);
        }

        [Fact]
        public void ImageList_ImageStream_SetNull_ClearsImages()
        {
            using var list = new ImageList();

            // Set null without images.
            list.ImageStream = null;
            Assert.Empty(list.Images);
            Assert.False(list.HandleCreated);

            // Set null with images.
            list.Images.Add(new Bitmap(10, 10));
            list.ImageStream = null;
            Assert.Empty(list.Images);
            Assert.False(list.HandleCreated);
        }

        [Fact]
        public void ImageList_ImageStream_SetNullWithHandle_ClearsImages()
        {
            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Set null without images.
            list.ImageStream = null;
            Assert.Empty(list.Images);
            Assert.False(list.HandleCreated);

            // Set null with images.
            Assert.NotEqual(IntPtr.Zero, list.Handle);
            list.Images.Add(new Bitmap(10, 10));
            list.ImageStream = null;
            Assert.Empty(list.Images);
            Assert.False(list.HandleCreated);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ImageList_Tag_Set_GetReturnsExpected(object value)
        {
            using var list = new ImageList
            {
                Tag = value
            };
            Assert.Equal(value, list.Tag);

            // Set same.
            list.Tag = value;
            Assert.Equal(value, list.Tag);
        }

        public static IEnumerable<object[]> TransparentColor_Set_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetColorWithEmptyTheoryData())
            {
                yield return testData;
            }

            yield return new object[] { Color.LightGray };
        }

        [Theory]
        [MemberData(nameof(TransparentColor_Set_TestData))]
        public void ImageList_TransparentColor_SetWithoutHandle_GetReturnsExpected(Color value)
        {
            using var list = new ImageList
            {
                TransparentColor = value
            };
            Assert.Equal(value, list.TransparentColor);
            Assert.False(list.HandleCreated);

            // Set same.
            list.TransparentColor = value;
            Assert.Equal(value, list.TransparentColor);
            Assert.False(list.HandleCreated);
        }

        [Fact]
        public void ImageList_TransparentColor_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
        {
            using var list = new ImageList();
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            list.RecreateHandle += handler;

            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);

            // Set same.
            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);

            // Set different.
            list.TransparentColor = Color.Blue;
            Assert.Equal(Color.Blue, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.False(list.HandleCreated);
        }

        [Theory]
        [MemberData(nameof(TransparentColor_Set_TestData))]
        public void ImageList_TransparentColor_SetWithHandle_GetReturnsExpected(Color value)
        {
            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            list.TransparentColor = value;
            Assert.Equal(value, list.TransparentColor);
            Assert.True(list.HandleCreated);

            // Set same.
            list.TransparentColor = value;
            Assert.Equal(value, list.TransparentColor);
            Assert.True(list.HandleCreated);
        }

        [Fact]
        public void ImageList_TransparentColor_SetWithHandleWithHandler_DoesNotCallRecreateHandle()
        {
            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) => callCount++;
            list.RecreateHandle += handler;

            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.True(list.HandleCreated);

            // Set same.
            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.True(list.HandleCreated);

            // Set different.
            list.TransparentColor = Color.Blue;
            Assert.Equal(Color.Blue, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.True(list.HandleCreated);

            // Remove handler.
            list.RecreateHandle -= handler;
            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.Equal(0, callCount);
            Assert.True(list.HandleCreated);
        }

        [Fact]
        public void ImageList_TransparentColor_ResetValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.TransparentColor)];
            using var list = new ImageList();
            Assert.Equal(Color.Transparent, list.TransparentColor);
            Assert.True(property.CanResetValue(list));

            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.True(property.CanResetValue(list));

            property.ResetValue(list);
            Assert.Equal(Color.LightGray, list.TransparentColor);
            Assert.False(property.CanResetValue(list));
        }

        [Fact]
        public void ImageList_TransparentColor_ShouldSerializeValue_Success()
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.TransparentColor)];
            using var list = new ImageList();
            Assert.Equal(Color.Transparent, list.TransparentColor);
            Assert.True(property.ShouldSerializeValue(list));

            list.TransparentColor = Color.Red;
            Assert.Equal(Color.Red, list.TransparentColor);
            Assert.True(property.ShouldSerializeValue(list));

            property.ResetValue(list);
            Assert.Equal(Color.LightGray, list.TransparentColor);
            Assert.False(property.ShouldSerializeValue(list));

            // With images.
            list.Images.Add(new Bitmap(10, 10));
            Assert.False(property.ShouldSerializeValue(list));
        }

        [Fact]
        public void ImageList_DisposeWithHandle_Nop()
        {
            var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);
            list.Dispose();
            Assert.False(list.HandleCreated);

            list.Dispose();
            Assert.False(list.HandleCreated);
        }

        [Fact]
        public void ImageList_DisposeWithoutHandle_Nop()
        {
            var list = new ImageList();
            list.Dispose();
            Assert.False(list.HandleCreated);

            list.Dispose();
            Assert.False(list.HandleCreated);
        }
    }
}
