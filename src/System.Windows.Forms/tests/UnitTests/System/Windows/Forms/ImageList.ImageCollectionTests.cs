// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    public class ImageCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ImageCollection_Count_GetEmptyWithHandle_ReturnsExpected()
        {
            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            ImageList.ImageCollection collection = list.Images;
            Assert.Equal(0, collection.Count);
        }

        [WinFormsFact]
        public void ImageCollection_Empty_GetEmptyWithHandle_ReturnsExpected()
        {
            using var list = new ImageList();
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            ImageList.ImageCollection collection = list.Images;
            Assert.True(collection.Empty);
        }

        [WinFormsFact]
        public void ImageCollection_IsReadOnly_IsReadOnly_ReturnsExpected()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.False(collection.IsReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageCollection_Item_GetInt_InvokeWithoutHandle_ReturnsExpected(ColorDepth depth)
        {
            using var list = new ImageList
            {
                ColorDepth = depth
            };
            using var image1bppIndexed = new Bitmap(1, 2, PixelFormat.Format24bppRgb);
            using var image24bppRGb = new Bitmap(3, 4, PixelFormat.Format24bppRgb);
            using var image32bppRGb = new Bitmap(5, 6, PixelFormat.Format32bppRgb);
            using var image32bppArgbNotTransparent = new Bitmap(7, 8, PixelFormat.Format32bppArgb);
            using var image32bppArgbTransparent = new Bitmap(9, 10, PixelFormat.Format32bppArgb);
            image32bppArgbTransparent.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
            list.Images.Add(image1bppIndexed);
            list.Images.Add(image24bppRGb);
            list.Images.Add(image32bppRGb);
            list.Images.Add(image32bppArgbNotTransparent);
            list.Images.Add(image32bppArgbTransparent);
            ImageList.ImageCollection collection = list.Images;
            Assert.False(list.HandleCreated);

            Bitmap bitmap1 = Assert.IsType<Bitmap>(collection[0]);
            Assert.Equal(new Size(16, 16), bitmap1.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap1.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap2 = Assert.IsType<Bitmap>(collection[1]);
            Assert.Equal(new Size(16, 16), bitmap2.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap2.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap3 = Assert.IsType<Bitmap>(collection[2]);
            Assert.Equal(new Size(16, 16), bitmap3.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap3.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap4 = Assert.IsType<Bitmap>(collection[3]);
            Assert.Equal(new Size(16, 16), bitmap4.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap4.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap5 = Assert.IsType<Bitmap>(collection[4]);
            Assert.Equal(new Size(16, 16), bitmap5.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap5.PixelFormat);
            Assert.True(list.HandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ColorDepth))]
        public void ImageCollection_Item_GetInt_InvokeWithHandle_ReturnsExpected(ColorDepth depth)
        {
            using var list = new ImageList
            {
                ColorDepth = depth
            };
            using var image1bppIndexed = new Bitmap(1, 2, PixelFormat.Format24bppRgb);
            using var image24bppRGb = new Bitmap(3, 4, PixelFormat.Format24bppRgb);
            using var image32bppRGb = new Bitmap(5, 6, PixelFormat.Format32bppRgb);
            using var image32bppArgbNotTransparent = new Bitmap(7, 8, PixelFormat.Format32bppArgb);
            using var image32bppArgbTransparent = new Bitmap(9, 10, PixelFormat.Format32bppArgb);
            image32bppArgbTransparent.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
            list.Images.Add(image1bppIndexed);
            list.Images.Add(image24bppRGb);
            list.Images.Add(image32bppRGb);
            list.Images.Add(image32bppArgbNotTransparent);
            list.Images.Add(image32bppArgbTransparent);
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            Bitmap bitmap1 = Assert.IsType<Bitmap>(collection[0]);
            Assert.Equal(new Size(16, 16), bitmap1.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap1.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap2 = Assert.IsType<Bitmap>(collection[1]);
            Assert.Equal(new Size(16, 16), bitmap2.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap2.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap3 = Assert.IsType<Bitmap>(collection[2]);
            Assert.Equal(new Size(16, 16), bitmap3.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap3.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap4 = Assert.IsType<Bitmap>(collection[3]);
            Assert.Equal(new Size(16, 16), bitmap4.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap4.PixelFormat);
            Assert.True(list.HandleCreated);

            Bitmap bitmap5 = Assert.IsType<Bitmap>(collection[4]);
            Assert.Equal(new Size(16, 16), bitmap5.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap5.PixelFormat);
            Assert.True(list.HandleCreated);
        }

        [WinFormsFact]
        public void ImageCollection_Item_Get32bppColorDepth_Success()
        {
            using var list = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };
            ImageList.ImageCollection collection = list.Images;

            using var image1bppIndexedEmpty = new Bitmap(16, 16, PixelFormat.Format1bppIndexed);
            collection.Add(image1bppIndexedEmpty);

            using var image1bppIndexedCustom = new Bitmap(16, 16, PixelFormat.Format1bppIndexed);
            collection.Add(image1bppIndexedCustom);

            using var image24bppRgbEmpty = new Bitmap(16, 16, PixelFormat.Format24bppRgb);
            collection.Add(image24bppRgbEmpty);

            using var image24bppRgbCustom = new Bitmap(16, 16, PixelFormat.Format24bppRgb);
            image24bppRgbCustom.SetPixel(0, 0, Color.Red);
            image24bppRgbCustom.SetPixel(1, 0, Color.FromArgb(200, 50, 75, 100));
            collection.Add(image24bppRgbCustom);

            using var image32bppRgbEmpty = new Bitmap(16, 16, PixelFormat.Format32bppRgb);
            collection.Add(image32bppRgbEmpty);

            using var image32bppRgbCustom = new Bitmap(16, 16, PixelFormat.Format32bppRgb);
            image32bppRgbCustom.SetPixel(0, 0, Color.Red);
            image32bppRgbCustom.SetPixel(1, 0, Color.FromArgb(200, 50, 75, 100));
            collection.Add(image32bppRgbCustom);

            using var image32bppArgbEmpty = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
            collection.Add(image32bppArgbEmpty);

            using var image32bppArgbCustom = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
            image32bppArgbCustom.SetPixel(0, 0, Color.Red);
            image32bppArgbCustom.SetPixel(1, 0, Color.FromArgb(200, 50, 75, 100));
            collection.Add(image32bppArgbCustom);

            using var image32bppPargbEmpty = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
            collection.Add(image32bppPargbEmpty);

            using var image32bppPargbCustom = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
            image32bppPargbCustom.SetPixel(0, 0, Color.Red);
            image32bppPargbCustom.SetPixel(1, 0, Color.FromArgb(200, 50, 75, 100));
            collection.Add(image32bppPargbCustom);

            using Bitmap resultImage1bppEmpty = Assert.IsType<Bitmap>(collection[0]);
            Assert.NotSame(image1bppIndexedEmpty, resultImage1bppEmpty);
            Assert.Equal(new Size(16, 16), resultImage1bppEmpty.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage1bppEmpty.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage1bppEmpty.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage1bppEmpty.GetPixel(1, 0));

            using Bitmap resultImage1bppRgbCustom = Assert.IsType<Bitmap>(collection[1]);
            Assert.NotSame(image1bppIndexedCustom, resultImage1bppRgbCustom);
            Assert.Equal(new Size(16, 16), resultImage1bppRgbCustom.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage1bppRgbCustom.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage1bppRgbCustom.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage1bppRgbCustom.GetPixel(1, 0));

            using Bitmap resultImage24bppEmpty = Assert.IsType<Bitmap>(collection[4]);
            Assert.NotSame(image24bppRgbEmpty, resultImage24bppEmpty);
            Assert.Equal(new Size(16, 16), resultImage24bppEmpty.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage24bppEmpty.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage24bppEmpty.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage24bppEmpty.GetPixel(1, 0));

            using Bitmap resultImage24bppRgbCustom = Assert.IsType<Bitmap>(collection[5]);
            Assert.NotSame(image24bppRgbCustom, resultImage24bppRgbCustom);
            Assert.Equal(new Size(16, 16), resultImage24bppRgbCustom.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage24bppRgbCustom.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), resultImage24bppRgbCustom.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xFF, 50, 75, 100), resultImage24bppRgbCustom.GetPixel(1, 0));

            using Bitmap resultImage32bppEmpty = Assert.IsType<Bitmap>(collection[4]);
            Assert.NotSame(image32bppRgbEmpty, resultImage32bppEmpty);
            Assert.Equal(new Size(16, 16), resultImage32bppEmpty.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage32bppEmpty.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage32bppEmpty.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), resultImage32bppEmpty.GetPixel(1, 0));

            using Bitmap resultImage32bppRgbCustom = Assert.IsType<Bitmap>(collection[5]);
            Assert.NotSame(image32bppRgbCustom, resultImage32bppRgbCustom);
            Assert.Equal(new Size(16, 16), resultImage32bppRgbCustom.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage32bppRgbCustom.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), resultImage32bppRgbCustom.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xFF, 50, 75, 100), resultImage32bppRgbCustom.GetPixel(1, 0));

            using Bitmap resultImage32bppArgbEmpty = Assert.IsType<Bitmap>(collection[6]);
            Assert.NotSame(image32bppArgbEmpty, resultImage32bppArgbEmpty);
            Assert.Equal(new Size(16, 16), resultImage32bppArgbEmpty.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage32bppArgbEmpty.PixelFormat);
            Assert.Equal(Color.FromArgb(0x00, 0x00, 0x00, 0x00), resultImage32bppArgbEmpty.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0x00, 0x00, 0x00, 0x00), resultImage32bppArgbEmpty.GetPixel(1, 0));

            using Bitmap resultImage32bppArgbCustom = Assert.IsType<Bitmap>(collection[7]);
            Assert.NotSame(image32bppArgbCustom, resultImage32bppArgbCustom);
            Assert.Equal(new Size(16, 16), resultImage32bppArgbCustom.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage32bppArgbCustom.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), resultImage32bppArgbCustom.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xC8, 0x55, 0x68, 0x7B), resultImage32bppArgbCustom.GetPixel(1, 0));

            using Bitmap resultImage32bppPargbEmpty = Assert.IsType<Bitmap>(collection[8]);
            Assert.NotSame(image32bppPargbEmpty, resultImage32bppPargbEmpty);
            Assert.Equal(new Size(16, 16), resultImage32bppPargbEmpty.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage32bppPargbEmpty.PixelFormat);
            Assert.Equal(Color.FromArgb(0x00, 0x00, 0x00, 0x00), resultImage32bppPargbEmpty.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0x00, 0x00, 0x00, 0x00), resultImage32bppPargbEmpty.GetPixel(1, 0));

            using Bitmap resultImage32bppPargbCustom = Assert.IsType<Bitmap>(collection[9]);
            Assert.NotSame(image32bppPargbCustom, resultImage32bppPargbCustom);
            Assert.Equal(new Size(16, 16), resultImage32bppPargbCustom.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, resultImage32bppPargbCustom.PixelFormat);
            Assert.Equal(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), resultImage32bppPargbCustom.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(0xC8, 0x55, 0x68, 0x7B), resultImage32bppPargbCustom.GetPixel(1, 0));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageCollection_Item_GetIntInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ImageCollection_Item_GetIntInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
        }

        public static IEnumerable<object[]> Item_Set_TestData()
        {
            foreach (Color transparentColor in new Color[] { Color.Transparent, Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.Empty, Color.Black })
            {
                yield return new object[] { transparentColor, new Bitmap(10, 10) };
                yield return new object[] { transparentColor, new Bitmap(16, 16) };
                yield return new object[] { transparentColor, new Bitmap(32, 32) };
                yield return new object[] { transparentColor, new Bitmap(256, 256) };

                var bitmap = new Bitmap(16, 16);
                bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
                yield return new object[] { transparentColor, bitmap };

                yield return new object[] { transparentColor, new Bitmap("bitmaps/10x16_one_entry_32bit.ico") };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Item_Set_TestData))]
        public void ImageCollection_Item_Set_GetReturnsExpected(Color transparentColor, Image value)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            list.Images.Add(image);
            ImageList.ImageCollection collection = list.Images;

            collection[0] = value;
            Assert.True(list.HandleCreated);

            Bitmap bitmap = Assert.IsType<Bitmap>(collection[0]);
            Assert.NotSame(value, bitmap);
            Assert.Equal(new Size(16, 16), bitmap.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap.PixelFormat);
        }

        [WinFormsTheory]
        [MemberData(nameof(Item_Set_TestData))]
        public void ImageCollection_Item_SetWithHandle_GetReturnsExpected(Color transparentColor, Image value)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            list.Images.Add(image);
            Assert.NotEqual(IntPtr.Zero, list.Handle);
            ImageList.ImageCollection collection = list.Images;

            collection[0] = value;
            Assert.True(list.HandleCreated);

            Bitmap bitmap = Assert.IsType<Bitmap>(collection[0]);
            Assert.Equal(new Size(16, 16), bitmap.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap.PixelFormat);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageCollection_Item_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = null);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ImageCollection_Item_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = null);
        }

        [WinFormsFact]
        public void ImageCollection_Item_SetNullValue_ThrowsArgumentNullException()
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            ImageList.ImageCollection collection = list.Images;

            Assert.Throws<ArgumentNullException>("value", () => collection[0] = null);
        }

        [WinFormsFact]
        public void ImageCollection_Item_SetNonBitmapValue_ThrowsArgumentException()
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            ImageList.ImageCollection collection = list.Images;

            using var value = new Metafile("bitmaps/telescope_01.wmf");
            Assert.Throws<ArgumentException>(null, () => collection[0] = value);
        }

        [WinFormsTheory]
        [InlineData("name1", 0)]
        [InlineData("NAME1", 0)]
        [InlineData("name2", 1)]
        public void ImageCollection_Item_GetStringValidKey_ReturnsExpected(string key, int expectedIndex)
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("name1", image1);
            collection.Add("name2", image2);

            Bitmap result1 = Assert.IsType<Bitmap>(collection[key]);
            Assert.Equal(new Size(16, 16), result1.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, result1.PixelFormat);
            Assert.Equal(((Bitmap)collection[expectedIndex]).GetPixel(0, 0), result1.GetPixel(0, 0));

            // Call again.
            Bitmap result2 = Assert.IsType<Bitmap>(collection[key]);
            Assert.NotSame(result1, result2);
            Assert.Equal(new Size(16, 16), result1.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, result1.PixelFormat);
            Assert.Equal(result1.GetPixel(0, 0), result2.GetPixel(0, 0));
            Assert.Null(collection["NoSuchKey"]);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NoSuchName")]
        [InlineData("abcd")]
        [InlineData("abcde")]
        [InlineData("abcdef")]
        public void ImageCollection_Item_GetStringNoSuchKey_ReturnsNull(string key)
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            using var image2 = new Bitmap(3, 4);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("name1", image1);
            collection.Add("name2", image2);

            Assert.Null(collection[key]);

            // Call again.
            Assert.Null(collection[key]);
            Assert.Null(collection["NoSuchKey"]);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ImageCollection_Item_GetStringEmpty_ReturnsNull(string key)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;

            Assert.Null(collection[key]);

            // Call again.
            Assert.Null(collection[key]);
            Assert.Null(collection["NoSuchKey"]);
        }

        [WinFormsFact]
        public void ImageCollection_Keys_GetEmpty_ReturnsExpected()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Empty(collection.Keys);
            Assert.NotSame(collection.Keys, collection.Keys);
        }

        public static IEnumerable<object[]> Add_Image_TestData()
        {
            foreach (Color transparentColor in new Color[] { Color.Transparent, Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.Empty, Color.Black })
            {
                yield return new object[] { transparentColor, new Bitmap(10, 10) };
                yield return new object[] { transparentColor, new Bitmap(16, 16) };
                yield return new object[] { transparentColor, new Bitmap(32, 32) };
                yield return new object[] { transparentColor, new Bitmap(256, 256) };

                var bitmap = new Bitmap(16, 16);
                bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
                yield return new object[] { transparentColor, bitmap };

                //yield return new object[] { transparentColor, new Bitmap("bitmaps/10x16_one_entry_32bit.ico") };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_TestData))]
        public void ImageCollection_Add_InvokeStringImage_Success(Color transparentColor, Image value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;

            collection.Add("Key1", value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal("Key1", Assert.Single(collection.Keys));
            Assert.False(list.HandleCreated);

            // Add again.
            collection.Add("Key2", value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2" }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Add same.
            collection.Add("Key1", value);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1" }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Add null.
            collection.Add(null, value);
            Assert.Equal(4, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", "" }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Add empty.
            collection.Add(string.Empty, value);
            Assert.Equal(5, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_TestData))]
        public void ImageCollection_Add_InvokeStringImageWithHandle_Success(Color transparentColor, Image value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.Add("Key1", value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal("Key1", Assert.Single(collection.Keys));
            Assert.True(list.HandleCreated);

            // Add again.
            collection.Add("Key2", value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2" }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Add same.
            collection.Add("Key1", value);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1" }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Add null.
            collection.Add(null, value);
            Assert.Equal(4, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", "" }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Add empty.
            collection.Add(string.Empty, value);
            Assert.Equal(5, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_TestData))]
        public void ImageCollection_Add_InvokeImage_Success(Color transparentColor, Image value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;

            collection.Add(value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(string.Empty, Assert.Single(collection.Keys));
            Assert.False(list.HandleCreated);

            // Add again.
            collection.Add(value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_TestData))]
        public void ImageCollection_Add_InvokeImageWithHandle_Success(Color transparentColor, Image value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.Add(value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(string.Empty, Assert.Single(collection.Keys));
            Assert.True(list.HandleCreated);

            // Add again.
            collection.Add(value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        public static IEnumerable<object[]> Add_Image_Color_TestData()
        {
            foreach (Color listTransparentColor in new Color[] { Color.Transparent, Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.Empty, Color.Black })
            {
                foreach (Color transparentColor in new Color[] { Color.Transparent, Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.Empty, Color.Black })
                {
                    yield return new object[] { listTransparentColor, new Bitmap(10, 10), transparentColor };
                    yield return new object[] { listTransparentColor, new Bitmap(16, 16), transparentColor };
                    yield return new object[] { listTransparentColor, new Bitmap(32, 32), transparentColor };
                    yield return new object[] { listTransparentColor, new Bitmap(256, 256), transparentColor };

                    var bitmap = new Bitmap(16, 16);
                    bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
                    yield return new object[] { listTransparentColor, bitmap, transparentColor };

                    //yield return new object[] { listTransparentColor, new Bitmap("bitmaps/10x16_one_entry_32bit.ico"), transparentColor };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_Color_TestData))]
        public void ImageCollection_Add_InvokeImageColor_Success(Color listTransparentColor, Image value, Color transparentColor)
        {
            using var list = new ImageList
            {
                TransparentColor = listTransparentColor
            };
            ImageList.ImageCollection collection = list.Images;

            collection.Add(value, transparentColor);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(string.Empty, Assert.Single(collection.Keys));
            Assert.False(list.HandleCreated);

            // Add again.
            collection.Add(value, transparentColor);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_Color_TestData))]
        public void ImageCollection_Add_InvokeImageColorWithHandle_Success(Color listTransparentColor, Image value, Color transparentColor)
        {
            using var list = new ImageList
            {
                TransparentColor = listTransparentColor
            };
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.Add(value, transparentColor);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(string.Empty, Assert.Single(collection.Keys));
            Assert.True(list.HandleCreated);

            // Add again.
            collection.Add(value, transparentColor);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        public static IEnumerable<object[]> Add_Icon_TestData()
        {
            foreach (Color transparentColor in new Color[] { Color.Transparent, Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.Empty, Color.Black })
            {
                yield return new object[] { transparentColor, new Icon("bitmaps/10x16_one_entry_32bit.ico") };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Icon_TestData))]
        public void ImageCollection_Add_InvokeStringIcon_Success(Color transparentColor, Icon value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;

            collection.Add("Key1", value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal("Key1", Assert.Single(collection.Keys));
            Assert.False(list.HandleCreated);

            // Add again.
            collection.Add("Key2", value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2" }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Add same.
            collection.Add("Key1", value);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1" }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Add null.
            collection.Add(null, value);
            Assert.Equal(4, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", "" }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Add empty.
            collection.Add(string.Empty, value);
            Assert.Equal(5, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Icon_TestData))]
        public void ImageCollection_Add_InvokeStringIconWithHandle_Success(Color transparentColor, Icon value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.Add("Key1", value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal("Key1", Assert.Single(collection.Keys));
            Assert.True(list.HandleCreated);

            // Add again.
            collection.Add("Key2", value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2" }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Add same.
            collection.Add("Key1", value);
            Assert.Equal(3, collection.Count);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1" }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Add null.
            collection.Add(null, value);
            Assert.Equal(4, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", "" }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Add empty.
            collection.Add(string.Empty, value);
            Assert.Equal(5, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { "Key1", "Key2", "Key1", string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Icon_TestData))]
        public void ImageCollection_Add_InvokeIcon_Success(Color transparentColor, Icon value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;

            collection.Add(value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(string.Empty, Assert.Single(collection.Keys));
            Assert.False(list.HandleCreated);

            // Add again.
            collection.Add(value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Icon_TestData))]
        public void ImageCollection_Add_InvokeIconWithHandle_Success(Color transparentColor, Icon value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.Add(value);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(string.Empty, Assert.Single(collection.Keys));
            Assert.True(list.HandleCreated);

            // Add again.
            collection.Add(value);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        [WinFormsFact]
        public void ImageCollection_Add_NullImage_ThrowsArgumentNullException()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentNullException>("value", () => collection.Add((Image)null));
            Assert.Throws<ArgumentNullException>("value", () => collection.Add((Image)null, Color.Transparent));
            Assert.Throws<InvalidOperationException>(() => collection.Add("Key", (Image)null));
            Assert.Throws<ArgumentNullException>("value", () => collection.Add((Icon)null));
            Assert.Throws<InvalidOperationException>(() => collection.Add("Key", (Icon)null));
        }

        [WinFormsFact]
        public void ImageCollection_Add_NonBitmapImage_ThrowsArgumentException()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;

            using var value = new Metafile("bitmaps/telescope_01.wmf");
            Assert.Throws<ArgumentException>(null, () => collection.Add(value));
            Assert.Throws<ArgumentException>(null, () => collection.Add(value, Color.Transparent));
            Assert.Throws<ArgumentException>(null, () => collection.Add("Key", value));
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_TestData))]
        public void ImageCollection_AddRange_Invoke_Success(Color transparentColor, Image value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;

            collection.AddRange(new Image[] { value, value });
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Add empty.
            collection.AddRange(Array.Empty<Image>());
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_Image_TestData))]
        public void ImageCollection_AddRange_InvokeWithHandle_Success(Color transparentColor, Image value)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.AddRange(new Image[] { value, value });
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Add empty.
            collection.AddRange(Array.Empty<Image>());
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(new string[] { string.Empty, string.Empty }, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        [WinFormsFact]
        public void ImageCollection_AddRange_NullImages_ThrowsArgumentNullException()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentNullException>("images", () => collection.AddRange(null));
        }

        [WinFormsFact]
        public void ImageCollection_AddRange_NullImageInImages_ThrowsArgumentNullException()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentNullException>("value", () => collection.AddRange(new Image[] { null }));
        }

        [WinFormsFact]
        public void ImageCollection_AddRange_NonBitmapImage_ThrowsArgumentException()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;

            using var value = new Metafile("bitmaps/telescope_01.wmf");
            Assert.Throws<ArgumentException>(null, () => collection.AddRange(new Image[] { value }));
        }

        public static IEnumerable<object[]> AddStrip_TestData()
        {
            foreach (Color transparentColor in new Color[] { Color.Transparent, Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.Empty, Color.Black })
            {
                yield return new object[] { transparentColor, new Bitmap(16, 16), 1 };
                yield return new object[] { transparentColor, new Bitmap(32, 16), 2};
                yield return new object[] { transparentColor, new Bitmap(256, 16), 16 };

                var bitmap = new Bitmap(16, 16);
                bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
                yield return new object[] { transparentColor, bitmap, 1 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(AddStrip_TestData))]
        public void ImageCollection_AddStrip_Invoke_Success(Color transparentColor, Image value, int expectedCount)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;

            collection.AddStrip(value);
            Assert.Equal(expectedCount, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(Enumerable.Repeat(string.Empty, expectedCount), collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(AddStrip_TestData))]
        public void ImageCollection_AddStrip_InvokeWithHandle_Success(Color transparentColor, Image value, int expectedCount)
        {
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.AddStrip(value);
            Assert.Equal(expectedCount, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(Enumerable.Repeat(string.Empty, expectedCount), collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        [WinFormsFact]
        public void ImageCollection_AddStrip_NullValue_ThrowsArgumentNullException()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentNullException>("value", () => collection.AddStrip(null));
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(15)]
        [InlineData(17)]
        public void ImageCollection_AddStrip_InvalidWidth_ThrowsArgumentException(int width)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            using var image = new Bitmap(width, 16);
            Assert.Throws<ArgumentException>("value", () => collection.AddStrip(image));
        }

        [WinFormsTheory]
        [InlineData(1)]
        [InlineData(15)]
        [InlineData(17)]
        public void ImageCollection_AddStrip_InvalidHeight_ThrowsArgumentException(int width)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            using var image = new Bitmap(16, width);
            Assert.Throws<ArgumentException>("value", () => collection.AddStrip(image));
        }

        [WinFormsFact]
        public void ImageCollection_Clear_InvokeEmpty_Nop()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;

            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.False(list.HandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.False(list.HandleCreated);
        }

        [WinFormsFact]
        public void ImageCollection_Clear_InvokeNotEmpty_Success()
        {
            using var list = new ImageList();
            using var image = new Bitmap(10, 10);
            ImageList.ImageCollection collection = list.Images;
            collection.Add(image);

            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.False(list.HandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.False(list.HandleCreated);
        }

        [WinFormsFact]
        public void ImageCollection_Clear_InvokeEmptyWithHandle_Nop()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.True(list.HandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.True(list.HandleCreated);
        }

        [WinFormsFact]
        public void ImageCollection_Clear_InvokeNotEmptyWithHandle_Success()
        {
            using var list = new ImageList();
            using var image = new Bitmap(10, 10);
            ImageList.ImageCollection collection = list.Images;
            collection.Add(image);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.True(list.HandleCreated);

            // Clear again.
            collection.Clear();
            Assert.Empty(collection);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
            Assert.True(list.HandleCreated);
        }

        public static IEnumerable<object[]> Contains_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { new Metafile("bitmaps/telescope_01.wmf") };
        }

        [WinFormsTheory]
        [MemberData(nameof(Contains_TestData))]
        public void ImageCollection_Contains_ThrowsNotSupportedException(Image image)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<NotSupportedException>(() => collection.Contains(image));
        }

        [WinFormsTheory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("name1", true)]
        [InlineData("NAME1", true)]
        [InlineData("name2", true)]
        [InlineData("NoSuchName", false)]
        [InlineData("abcd", false)]
        [InlineData("abcde", false)]
        [InlineData("abcdef", false)]
        public void ImageCollection_ContainsKey_InvokeNotEmpty_ReturnsExpected(string key, bool expected)
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            using var image2 = new Bitmap(3, 4);
            using var image3 = new Bitmap(5, 6);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("name1", image1);
            collection.Add("name2", image2);
            collection.Add("name3", image3);

            Assert.Equal(expected, collection.ContainsKey(key));

            // Call again.
            Assert.Equal(expected, collection.ContainsKey(key));
            Assert.False(collection.ContainsKey("NoSuchKey"));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ImageCollection_ContainsKey_InvokeEmpty_ReturnsExpected(string key)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;

            Assert.False(collection.ContainsKey(key));

            // Call again.
            Assert.False(collection.ContainsKey(key));
            Assert.False(collection.ContainsKey("NoSuchKey"));
        }

        [WinFormsFact]
        public void ImageListCollection_GetEnumerator_InvokeWithoutHandleEmpty_Success()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;

            IEnumerator enumerator = collection.GetEnumerator();
            Assert.False(list.HandleCreated);
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ImageListCollection_GetEnumerator_InvokeWithHandleEmpty_Success()
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            IEnumerator enumerator = collection.GetEnumerator();
            Assert.True(list.HandleCreated);
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ImageListCollection_GetEnumerator_InvokeWithoutHandleNotEmpty_Success()
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            ImageList.ImageCollection collection = list.Images;
            collection.Add(image1);
            collection.Add(image2);

            IEnumerator enumerator = collection.GetEnumerator();
            Assert.True(list.HandleCreated);
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Bitmap result1 = Assert.IsType<Bitmap>(enumerator.Current);
                Assert.Equal(new Size(16, 16), result1.Size);
                Assert.Equal(PixelFormat.Format32bppArgb, result1.PixelFormat);
                Assert.Equal(((Bitmap)collection[0]).GetPixel(0, 0), result1.GetPixel(0, 0));

                Assert.True(enumerator.MoveNext());
                Bitmap result2 = Assert.IsType<Bitmap>(enumerator.Current);
                Assert.Equal(new Size(16, 16), result2.Size);
                Assert.Equal(PixelFormat.Format32bppArgb, result2.PixelFormat);
                Assert.Equal(((Bitmap)collection[1]).GetPixel(0, 0), result2.GetPixel(0, 0));

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                enumerator.Reset();
            }
        }

        [WinFormsFact]
        public void ImageListCollection_GetEnumerator_InvokeWithHandleNotEmpty_Success()
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            ImageList.ImageCollection collection = list.Images;
            collection.Add(image1);
            collection.Add(image2);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            IEnumerator enumerator = collection.GetEnumerator();
            Assert.True(list.HandleCreated);
            for (int i = 0; i < 2; i++)
            {
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                Assert.True(enumerator.MoveNext());
                Bitmap result1 = Assert.IsType<Bitmap>(enumerator.Current);
                Assert.Equal(new Size(16, 16), result1.Size);
                Assert.Equal(PixelFormat.Format32bppArgb, result1.PixelFormat);
                Assert.Equal(((Bitmap)collection[0]).GetPixel(0, 0), result1.GetPixel(0, 0));

                Assert.True(enumerator.MoveNext());
                Bitmap result2 = Assert.IsType<Bitmap>(enumerator.Current);
                Assert.Equal(new Size(16, 16), result2.Size);
                Assert.Equal(PixelFormat.Format32bppArgb, result2.PixelFormat);
                Assert.Equal(((Bitmap)collection[1]).GetPixel(0, 0), result2.GetPixel(0, 0));

                Assert.False(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => enumerator.Current);

                enumerator.Reset();
            }
        }

        [WinFormsTheory]
        [InlineData(null, -1)]
        [InlineData("", -1)]
        [InlineData("name1", 0)]
        [InlineData("NAME1", 0)]
        [InlineData("name2", 1)]
        [InlineData("NoSuchName", -1)]
        [InlineData("abcd", -1)]
        [InlineData("abcde", -1)]
        [InlineData("abcdef", -1)]
        public void ImageCollection_IndexOfKey_InvokeNotEmpty_ReturnsExpected(string key, int expected)
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            using var image2 = new Bitmap(3, 4);
            using var image3 = new Bitmap(5, 6);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("name1", image1);
            collection.Add("name2", image2);
            collection.Add("name3", image3);

            Assert.Equal(expected, collection.IndexOfKey(key));

            // Call again.
            Assert.Equal(expected, collection.IndexOfKey(key));
            Assert.Equal(-1, collection.IndexOfKey("NoSuchKey"));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ImageCollection_IndexOfKey_InvokeEmpty_ReturnsExpected(string key)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;

            Assert.Equal(-1, collection.IndexOfKey(key));

            // Call again.
            Assert.Equal(-1, collection.IndexOfKey(key));
            Assert.Equal(-1, collection.IndexOfKey("NoSuchKey"));
        }

        public static IEnumerable<object[]> IndexOf_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { new Metafile("bitmaps/telescope_01.wmf") };
        }

        [WinFormsTheory]
        [MemberData(nameof(IndexOf_TestData))]
        public void ImageCollection_IndexOf_ThrowsNotSupportedException(Image image)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<NotSupportedException>(() => collection.IndexOf(image));
        }

        public static IEnumerable<object[]> Remove_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { new Metafile("bitmaps/telescope_01.wmf") };
        }

        [WinFormsTheory]
        [MemberData(nameof(Remove_TestData))]
        public void ImageCollection_Remove_Invoke_ThrowsNotSupportedException(Image image)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<NotSupportedException>(() => collection.Remove(image));
        }

        [WinFormsFact]
        public void ImageListCollection_RemoveAt_InvokeWithoutHandleNotEmpty_Success()
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            using var image3 = new Bitmap(3, 4);
            image3.SetPixel(0, 0, Color.Yellow);
            ImageList.ImageCollection collection = list.Images;
            collection.Add(image1);
            collection.Add(image2);
            collection.Add(image3);

            // Remove middle.
            collection.RemoveAt(1);
            Assert.True(list.HandleCreated);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);

            // Remove first.
            collection.RemoveAt(0);
            Assert.True(list.HandleCreated);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);

            // Remove last.
            collection.RemoveAt(0);
            Assert.True(list.HandleCreated);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
        }

        [WinFormsFact]
        public void ImageListCollection_RemoveAt_InvokeWithHandleNotEmpty_Success()
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            using var image3 = new Bitmap(3, 4);
            image3.SetPixel(0, 0, Color.Yellow);
            ImageList.ImageCollection collection = list.Images;
            collection.Add(image1);
            collection.Add(image2);
            collection.Add(image3);
            Color color1 = ((Bitmap)collection[0]).GetPixel(0, 0);
            Color color2 = ((Bitmap)collection[1]).GetPixel(0, 0);
            Color color3 = ((Bitmap)collection[2]).GetPixel(0, 0);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Remove middle.
            collection.RemoveAt(1);
            Assert.True(list.HandleCreated);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(color1, ((Bitmap)collection[0]).GetPixel(0, 0));
            Assert.Equal(color3, ((Bitmap)collection[1]).GetPixel(0, 0));

            // Remove first.
            collection.RemoveAt(0);
            Assert.True(list.HandleCreated);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(color3, ((Bitmap)collection[0]).GetPixel(0, 0));

            // Remove last.
            collection.RemoveAt(0);
            Assert.True(list.HandleCreated);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ImageCollection_RemoveAt_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
        }

        [WinFormsFact]
        public void ImageListCollection_RemoveByKey_InvokeWithoutHandleNotEmpty_Success()
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            using var image3 = new Bitmap(3, 4);
            image3.SetPixel(0, 0, Color.Yellow);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("image1", image1);
            collection.Add("image2", image2);
            collection.Add("image3", image3);

            // Remove middle.
            collection.RemoveByKey("image2");
            Assert.True(list.HandleCreated);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);

            // Call again.
            collection.RemoveByKey("image2");
            Assert.True(list.HandleCreated);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);

            // Remove first.
            collection.RemoveByKey("image1");
            Assert.True(list.HandleCreated);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);

            // Remove last.
            collection.RemoveByKey("IMAGE3");
            Assert.True(list.HandleCreated);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
        }

        [WinFormsFact]
        public void ImageListCollection_RemoveByKey_InvokeWithHandleNotEmpty_Success()
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            using var image3 = new Bitmap(3, 4);
            image3.SetPixel(0, 0, Color.Yellow);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("image1", image1);
            collection.Add("image2", image2);
            collection.Add("image3", image3);
            Color color1 = ((Bitmap)collection[0]).GetPixel(0, 0);
            Color color2 = ((Bitmap)collection[1]).GetPixel(0, 0);
            Color color3 = ((Bitmap)collection[2]).GetPixel(0, 0);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            // Remove middle.
            collection.RemoveByKey("image2");
            Assert.True(list.HandleCreated);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(color1, ((Bitmap)collection[0]).GetPixel(0, 0));
            Assert.Equal(color3, ((Bitmap)collection[1]).GetPixel(0, 0));

            // Call again.
            collection.RemoveByKey("image2");
            Assert.True(list.HandleCreated);
            Assert.Equal(2, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(color1, ((Bitmap)collection[0]).GetPixel(0, 0));
            Assert.Equal(color3, ((Bitmap)collection[1]).GetPixel(0, 0));

            // Remove first.
            collection.RemoveByKey("image1");
            Assert.True(list.HandleCreated);
            Assert.Equal(1, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(color3, ((Bitmap)collection[0]).GetPixel(0, 0));

            // Remove last.
            collection.RemoveByKey("IMAGE3");
            Assert.True(list.HandleCreated);
            Assert.Equal(0, collection.Count);
            Assert.True(collection.Empty);
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("NoSuchImage")]
        public void ImageListCollection_RemoveByKey_InvokeNoSuchKey_Nop(string key)
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            image1.SetPixel(0, 0, Color.Red);
            using var image2 = new Bitmap(3, 4);
            image2.SetPixel(0, 0, Color.Blue);
            using var image3 = new Bitmap(3, 4);
            image3.SetPixel(0, 0, Color.Yellow);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("image1", image1);
            collection.Add("image2", image2);
            collection.Add("image3", image3);
            Color color1 = ((Bitmap)collection[0]).GetPixel(0, 0);
            Color color2 = ((Bitmap)collection[1]).GetPixel(0, 0);
            Color color3 = ((Bitmap)collection[2]).GetPixel(0, 0);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.RemoveByKey(key);
            Assert.True(list.HandleCreated);
            Assert.Equal(3, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(color1, ((Bitmap)collection[0]).GetPixel(0, 0));
            Assert.Equal(color2, ((Bitmap)collection[1]).GetPixel(0, 0));
            Assert.Equal(color3, ((Bitmap)collection[2]).GetPixel(0, 0));

            // Call again.
            collection.RemoveByKey(key);
            Assert.True(list.HandleCreated);
            Assert.Equal(3, collection.Count);
            Assert.False(collection.Empty);
            Assert.Equal(color1, ((Bitmap)collection[0]).GetPixel(0, 0));
            Assert.Equal(color2, ((Bitmap)collection[1]).GetPixel(0, 0));
            Assert.Equal(color3, ((Bitmap)collection[2]).GetPixel(0, 0));
        }

        public static IEnumerable<object[]> SetKeyName_TestData()
        {
            yield return new object[] { 0, null, new string[] { string.Empty, string.Empty } };
            yield return new object[] { 0, string.Empty, new string[] { string.Empty, string.Empty } };
            yield return new object[] { 0, "name", new string[] { "name", string.Empty } };

            yield return new object[] { 1, null, new string[] { "KeyName", string.Empty } };
            yield return new object[] { 1, string.Empty, new string[] { "KeyName", string.Empty } };
            yield return new object[] { 1, "name", new string[] { "KeyName", "name" } };
        }

        [WinFormsTheory]
        [MemberData(nameof(SetKeyName_TestData))]
        public void ImageCollection_SetKeyName_InvokeWithoutHandle_Success(int index, string name, string[] expectedKeys)
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            using var image2 = new Bitmap(3, 4);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("KeyName", image1);
            collection.Add(image2);

            collection.SetKeyName(index, name);
            Assert.Equal(expectedKeys, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);

            // Set again.
            collection.SetKeyName(index, name);
            Assert.Equal(expectedKeys, collection.Keys.Cast<string>());
            Assert.False(list.HandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(SetKeyName_TestData))]
        public void ImageCollection_SetKeyNameWithHandle_InvokeWithoutHandle_Success(int index, string name, string[] expectedKeys)
        {
            using var list = new ImageList();
            using var image1 = new Bitmap(1, 2);
            using var image2 = new Bitmap(3, 4);
            ImageList.ImageCollection collection = list.Images;
            collection.Add("KeyName", image1);
            collection.Add(image2);
            Assert.NotEqual(IntPtr.Zero, list.Handle);

            collection.SetKeyName(index, name);
            Assert.Equal(expectedKeys, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);

            // Set again.
            collection.SetKeyName(index, name);
            Assert.Equal(expectedKeys, collection.Keys.Cast<string>());
            Assert.True(list.HandleCreated);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageCollection_SetKeyName_InvalidIndexEmpty_ThrowsIndexOutOfRangeException(int index)
        {
            using var list = new ImageList();
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<IndexOutOfRangeException>(() => collection.SetKeyName(index, "name"));
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ImageCollection_SetKeyName_InvalidIndexNotEmpty_ThrowsIndexOutOfRangeException(int index)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            ImageList.ImageCollection collection = list.Images;
            Assert.Throws<IndexOutOfRangeException>(() => collection.SetKeyName(index, "name"));
        }

        [WinFormsFact]
        public void ImageCollection_IListIsFixedSize_GetReturnsExpected()
        {
            using var list = new ImageList();
            IList collection = list.Images;
            Assert.False(collection.IsFixedSize);
        }

        [WinFormsFact]
        public void ImageCollection_ICollectionIsSynchronized_GetReturnsExpected()
        {
            using var list = new ImageList();
            ICollection collection = list.Images;
            Assert.False(collection.IsSynchronized);
        }

        [WinFormsFact]
        public void ImageCollection_ICollectionSyncRoot_GetReturnsExpected()
        {
            using var list = new ImageList();
            ICollection collection = list.Images;
            Assert.Same(collection, collection.SyncRoot);
        }

        [WinFormsTheory]
        [MemberData(nameof(Item_Set_TestData))]
        public void ImageCollection_IListItem_Set_GetReturnsExpected(Color transparentColor, Image value)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            list.Images.Add(image);
            IList collection = list.Images;

            collection[0] = value;
            Assert.True(list.HandleCreated);

            Bitmap bitmap = Assert.IsType<Bitmap>(collection[0]);
            Assert.NotSame(value, bitmap);
            Assert.Equal(new Size(16, 16), bitmap.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap.PixelFormat);
        }

        [WinFormsTheory]
        [MemberData(nameof(Item_Set_TestData))]
        public void ImageCollection_IListItem_SetWithHandle_GetReturnsExpected(Color transparentColor, Image value)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList
            {
                TransparentColor = transparentColor
            };
            list.Images.Add(image);
            Assert.NotEqual(IntPtr.Zero, list.Handle);
            IList collection = list.Images;

            collection[0] = value;
            Assert.True(list.HandleCreated);

            Bitmap bitmap = Assert.IsType<Bitmap>(collection[0]);
            Assert.Equal(new Size(16, 16), bitmap.Size);
            Assert.Equal(PixelFormat.Format32bppArgb, bitmap.PixelFormat);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ImageCollection_IListItem_SetInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var list = new ImageList();
            IList collection = list.Images;

            using var value = new Bitmap(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = value);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ImageCollection_IListItem_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            IList collection = list.Images;

            using var value = new Bitmap(1, 2);
            Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = image);
        }

        public static IEnumerable<object[]> IListItem_SetNonImageValue_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListItem_SetNonImageValue_TestData))]
        public void ImageCollection_IListItem_SetNullValue_ThrowsArgumentException(object value)
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            IList collection = list.Images;

            Assert.Throws<ArgumentException>("value", () => collection[0] = value);
        }

        [WinFormsFact]
        public void ImageCollection_IListItem_SetNonBitmapValue_ThrowsArgumentException()
        {
            using var image = new Bitmap(10, 10);
            using var list = new ImageList();
            list.Images.Add(image);
            IList collection = list.Images;

            using var value = new Metafile("bitmaps/telescope_01.wmf");
            Assert.Throws<ArgumentException>(null, () => collection[0] = value);
        }

        public static IEnumerable<object[]> IListContains_Image_TestData()
        {
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { new Metafile("bitmaps/telescope_01.wmf") };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListContains_Image_TestData))]
        public void ImageCollection_IListContains_InvokeImage_ThrowsNotSupportedException(object value)
        {
            using var list = new ImageList();
            IList collection = list.Images;
            Assert.Throws<NotSupportedException>(() => collection.Contains(value));
        }

        public static IEnumerable<object[]> IListContains_NotImage_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new Icon("bitmaps/10x16_one_entry_32bit.ico") };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListContains_NotImage_TestData))]
        public void ImageCollection_IListContains_InvokeNotImage_ReturnsFalse(object value)
        {
            using var list = new ImageList();
            IList collection = list.Images;
            Assert.False(collection.Contains(value));
        }

        public static IEnumerable<object[]> IListIndexOf_Image_TestData()
        {
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { new Metafile("bitmaps/telescope_01.wmf") };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListIndexOf_Image_TestData))]
        public void ImageCollection_IListIndexOf_InvokeImage_ThrowsNotSupportedException(object value)
        {
            using var list = new ImageList();
            IList collection = list.Images;
            Assert.Throws<NotSupportedException>(() => collection.IndexOf(value));
        }

        public static IEnumerable<object[]> IListIndexOf_NotImage_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new Icon("bitmaps/10x16_one_entry_32bit.ico") };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListIndexOf_NotImage_TestData))]
        public void ImageCollection_IListIndexOf_InvokeNotImage_ReturnsExpected(object value)
        {
            using var list = new ImageList();
            IList collection = list.Images;
            Assert.Equal(-1, collection.IndexOf(value));
        }

        public static IEnumerable<object[]> IListInsert_TestData()
        {
            foreach (int index in new int[] { -1, 0, 1 })
            {
                yield return new object[] { index, null };
                yield return new object[] { index, new object() };
                yield return new object[] { index, new Bitmap(10, 10) };
                yield return new object[] { index, new Metafile("bitmaps/telescope_01.wmf") };
                yield return new object[] { index, new Icon("bitmaps/10x16_one_entry_32bit.ico") };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(IListInsert_TestData))]
        public void ImageCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
        {
            using var list = new ImageList();
            IList collection = list.Images;
            Assert.Throws<NotSupportedException>(() => collection.Insert(index, value));
        }

        public static IEnumerable<object[]> IListRemove_Image_TestData()
        {
            yield return new object[] { new Bitmap(10, 10) };
            yield return new object[] { new Metafile("bitmaps/telescope_01.wmf") };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListRemove_Image_TestData))]
        public void ImageCollection_IListRemove_InvokeImage_ThrowsNotSupportedException(object value)
        {
            using var list = new ImageList();
            IList collection = list.Images;
            Assert.Throws<NotSupportedException>(() => collection.Remove(value));
        }

        public static IEnumerable<object[]> IListRemove_NotImage_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new Icon("bitmaps/10x16_one_entry_32bit.ico") };
        }

        [WinFormsTheory]
        [MemberData(nameof(IListRemove_NotImage_TestData))]
        public void ImageCollection_IListRemove_InvokeNotImage_Nop(object value)
        {
            using var list = new ImageList();
            IList collection = list.Images;
            collection.Remove(value);
        }
    }
}
