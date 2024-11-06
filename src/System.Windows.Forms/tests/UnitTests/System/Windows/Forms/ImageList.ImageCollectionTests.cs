// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // ImageList doesn't appear to behave well under stress in multi-threaded env
public class ImageCollectionTests
{
    [WinFormsFact]
    public void ImageCollection_Count_GetEmptyWithHandle_ReturnsExpected()
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        ImageList.ImageCollection collection = list.Images;
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public void ImageCollection_Empty_GetEmptyWithHandle_ReturnsExpected()
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        ImageList.ImageCollection collection = list.Images;
        Assert.True(collection.Empty);
    }

    [WinFormsFact]
    public void ImageCollection_IsReadOnly_IsReadOnly_ReturnsExpected()
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.False(collection.IsReadOnly);
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageCollection_Item_GetInt_InvokeWithoutHandle_ReturnsExpected(ColorDepth depth)
    {
        using ImageList list = new()
        {
            ColorDepth = depth
        };
        using Bitmap image1bppIndexed = new(1, 2, PixelFormat.Format24bppRgb);
        using Bitmap image24bppRGb = new(3, 4, PixelFormat.Format24bppRgb);
        using Bitmap image32bppRGb = new(5, 6, PixelFormat.Format32bppRgb);
        using Bitmap image32bppArgbNotTransparent = new(7, 8, PixelFormat.Format32bppArgb);
        using Bitmap image32bppArgbTransparent = new(9, 10, PixelFormat.Format32bppArgb);
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
    [EnumData<ColorDepth>]
    public void ImageCollection_Item_GetInt_InvokeWithHandle_ReturnsExpected(ColorDepth depth)
    {
        using ImageList list = new()
        {
            ColorDepth = depth
        };
        using Bitmap image1bppIndexed = new(1, 2, PixelFormat.Format24bppRgb);
        using Bitmap image24bppRGb = new(3, 4, PixelFormat.Format24bppRgb);
        using Bitmap image32bppRGb = new(5, 6, PixelFormat.Format32bppRgb);
        using Bitmap image32bppArgbNotTransparent = new(7, 8, PixelFormat.Format32bppArgb);
        using Bitmap image32bppArgbTransparent = new(9, 10, PixelFormat.Format32bppArgb);
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

    public static IEnumerable<object[]> ImageCollection_VisualStyles_off_Item_Get32bppColorDepth_TestData()
    {
        var pixelFormats = new[]
        {
            PixelFormat.Format1bppIndexed,
            PixelFormat.Format24bppRgb,
            PixelFormat.Format32bppRgb,
            PixelFormat.Format32bppArgb,
            PixelFormat.Format32bppPArgb,
        };

        // SetPixel is not supported for images with indexed pixel formats.

        foreach (PixelFormat pixelFormat in pixelFormats)
        {
            yield return new object[] { pixelFormat, Color.Empty, Color.Empty };
            yield return new object[] { PixelFormat.Format24bppRgb, Color.Red, Color.FromArgb(200, 50, 75, 100) };
        }
    }

    public static IEnumerable<object[]> ImageCollection_VisualStyles_on_Item_Get32bppColorDepth_TestData()
    {
        // SetPixel is not supported for images with indexed pixel formats.
        yield return new object[] { PixelFormat.Format1bppIndexed, Color.Empty, Color.Empty, Color.FromArgb(255, 0, 0, 0) };

        // The actual colors are visually close to the originals, but no color fidelity.
        // Comment the following data out due to ActiveIssue "https://github.com/dotnet/winforms/issues/11226".
        if (ArchitectureDetection.Is64bit)
        {
            yield return new object[] { PixelFormat.Format24bppRgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(255, 50, 75, 100) };
            yield return new object[] { PixelFormat.Format32bppRgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(255, 50, 75, 100) };
            // yield return new object[] { PixelFormat.Format32bppArgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(200, 67, 81, 96) };
            // yield return new object[] { PixelFormat.Format32bppPArgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(200, 67, 81, 96) };
        }
        else
        {
            // yield return new object[] { PixelFormat.Format24bppRgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(255, 49, 74, 99) };
            // yield return new object[] { PixelFormat.Format32bppRgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(255, 49, 74, 99) };
            // yield return new object[] { PixelFormat.Format32bppArgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(200, 66, 81, 95) };
            // yield return new object[] { PixelFormat.Format32bppPArgb, Color.Red, Color.FromArgb(200, 50, 75, 100), Color.FromArgb(200, 66, 81, 95) };
        }
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11226")]
    [WinFormsTheory]
    [MemberData(nameof(ImageCollection_VisualStyles_on_Item_Get32bppColorDepth_TestData))]
    public void ImageCollection_Item_Get32bppColorDepth_Success(PixelFormat pixelFormat, Color pixel00Color, Color givenPixel10Color, Color expectedPixel10Color)
    {
        using Bitmap imageFiller1 = new(16, 16, pixelFormat);
        using Bitmap imageFiller2 = new(16, 16, pixelFormat);

        using Bitmap image = new(16, 16, pixelFormat);
        if (pixel00Color != Color.Empty)
            image.SetPixel(0, 0, pixel00Color);
        if (givenPixel10Color != Color.Empty)
            image.SetPixel(1, 0, givenPixel10Color);

        using ImageList list = new()
        {
            ColorDepth = ColorDepth.Depth32Bit
        };
        ImageList.ImageCollection collection = list.Images;
        collection.Add(imageFiller1);
        collection.Add(image);
        collection.Add(imageFiller2);

        // By getting a bitmap from the ImageListCollection ImageList will clone the original bitmap.
        // Assert that the new bitmap contains all the same properties.

        Bitmap resultImage = Assert.IsType<Bitmap>(collection[1]);

        Assert.Equal(image.Size, resultImage.Size);
        Assert.Equal(PixelFormat.Format32bppArgb, resultImage.PixelFormat);
        Assert.Equal(image.GetPixel(0, 0), resultImage.GetPixel(0, 0));
        Assert.Equal(expectedPixel10Color, resultImage.GetPixel(1, 0));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ImageCollection_Item_GetIntInvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index]);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ImageCollection_Item_GetIntInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
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

            Bitmap bitmap = new(16, 16);
            bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
            yield return new object[] { transparentColor, bitmap };

            yield return new object[] { transparentColor, new Bitmap("bitmaps/10x16_one_entry_32bit.ico") };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Item_Set_TestData))]
    public void ImageCollection_Item_Set_GetReturnsExpected(Color transparentColor, Image value)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new()
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
        using Bitmap image = new(10, 10);
        using ImageList list = new()
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
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = null);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ImageCollection_Item_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = null);
    }

    [WinFormsFact]
    public void ImageCollection_Item_SetNullValue_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        ImageList.ImageCollection collection = list.Images;

        Assert.Throws<ArgumentNullException>("value", () => collection[0] = null);
    }

    [WinFormsFact]
    public void ImageCollection_Item_SetNonBitmapValue_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        ImageList.ImageCollection collection = list.Images;

        using Metafile value = new("bitmaps/telescope_01.wmf");
        Assert.Throws<ArgumentException>(() => collection[0] = value);
    }

    [WinFormsTheory]
    [InlineData("name1", 0)]
    [InlineData("NAME1", 0)]
    [InlineData("name2", 1)]
    public void ImageCollection_Item_GetStringValidKey_ReturnsExpected(string key, int expectedIndex)
    {
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
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
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        using Bitmap image2 = new(3, 4);
        ImageList.ImageCollection collection = list.Images;
        collection.Add("name1", image1);
        collection.Add("name2", image2);

        Assert.Null(collection[key]);

        // Call again.
        Assert.Null(collection[key]);
        Assert.Null(collection["NoSuchKey"]);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ImageCollection_Item_GetStringEmpty_ReturnsNull(string key)
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;

        Assert.Null(collection[key]);

        // Call again.
        Assert.Null(collection[key]);
        Assert.Null(collection["NoSuchKey"]);
    }

    [WinFormsFact]
    public void ImageCollection_Keys_GetEmpty_ReturnsExpected()
    {
        using ImageList list = new();
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

            Bitmap bitmap = new(16, 16);
            bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
            yield return new object[] { transparentColor, bitmap };

            // yield return new object[] { transparentColor, new Bitmap("bitmaps/10x16_one_entry_32bit.ico") };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_Image_TestData))]
    public void ImageCollection_Add_InvokeStringImage_Success(Color transparentColor, Image value)
    {
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;

        collection.Add("Key1", value);
        collection.Count.Should().Be(1);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.Add("Key1", value);
        Assert.Single(collection);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;

        collection.Add(value);
        collection.Count.Should().Be(1);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.Add(value);
        Assert.Single(collection);
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

                Bitmap bitmap = new(16, 16);
                bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
                yield return new object[] { listTransparentColor, bitmap, transparentColor };

                // yield return new object[] { listTransparentColor, new Bitmap("bitmaps/10x16_one_entry_32bit.ico"), transparentColor };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_Image_Color_TestData))]
    public void ImageCollection_Add_InvokeImageColor_Success(Color listTransparentColor, Image value, Color transparentColor)
    {
        using ImageList list = new()
        {
            TransparentColor = listTransparentColor
        };
        ImageList.ImageCollection collection = list.Images;

        collection.Add(value, transparentColor);
        collection.Count.Should().Be(1);
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
        using ImageList list = new()
        {
            TransparentColor = listTransparentColor
        };
        ImageList.ImageCollection collection = list.Images;
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.Add(value, transparentColor);
        Assert.Single(collection);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;

        collection.Add("Key1", value);
        collection.Count.Should().Be(1);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.Add("Key1", value);
        Assert.Single(collection);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;

        collection.Add(value);
        collection.Count.Should().Be(1);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.Add(value);
        Assert.Single(collection);
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
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentNullException>("value", () => collection.Add((Image)null));
        Assert.Throws<ArgumentNullException>("value", () => collection.Add(null, Color.Transparent));
        Assert.Throws<InvalidOperationException>(() => collection.Add("Key", (Image)null));
        Assert.Throws<ArgumentNullException>("value", () => collection.Add((Icon)null));
        Assert.Throws<InvalidOperationException>(() => collection.Add("Key", (Icon)null));
    }

    [WinFormsFact]
    public void ImageCollection_Add_NonBitmapImage_ThrowsArgumentException()
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;

        using Metafile value = new("bitmaps/telescope_01.wmf");
        Assert.Throws<ArgumentException>(() => collection.Add(value));
        Assert.Throws<ArgumentException>(() => collection.Add(value, Color.Transparent));
        Assert.Throws<ArgumentException>(() => collection.Add("Key", value));
    }

    [WinFormsTheory]
    [MemberData(nameof(Add_Image_TestData))]
    public void ImageCollection_AddRange_Invoke_Success(Color transparentColor, Image value)
    {
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;

        collection.AddRange([value, value]);
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
        using ImageList list = new()
        {
            TransparentColor = transparentColor
        };
        ImageList.ImageCollection collection = list.Images;
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.AddRange([value, value]);
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
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentNullException>("images", () => collection.AddRange(null));
    }

    [WinFormsFact]
    public void ImageCollection_AddRange_NullImageInImages_ThrowsArgumentNullException()
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentNullException>("value", () => collection.AddRange([null]));
    }

    [WinFormsFact]
    public void ImageCollection_AddRange_NonBitmapImage_ThrowsArgumentException()
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;

        using Metafile value = new("bitmaps/telescope_01.wmf");
        Assert.Throws<ArgumentException>(() => collection.AddRange([value]));
    }

    public static IEnumerable<object[]> AddStrip_TestData()
    {
        foreach (Color transparentColor in new Color[] { Color.Transparent, Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.Empty, Color.Black })
        {
            yield return new object[] { transparentColor, new Bitmap(16, 16), 1 };
            yield return new object[] { transparentColor, new Bitmap(32, 16), 2 };
            yield return new object[] { transparentColor, new Bitmap(256, 16), 16 };

            Bitmap bitmap = new(16, 16);
            bitmap.SetPixel(0, 0, Color.FromArgb(0x12, 0x34, 0x56, 0x78));
            yield return new object[] { transparentColor, bitmap, 1 };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(AddStrip_TestData))]
    public void ImageCollection_AddStrip_Invoke_Success(Color transparentColor, Image value, int expectedCount)
    {
        using ImageList list = new()
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
        using ImageList list = new()
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
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentNullException>("value", () => collection.AddStrip(null));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(17)]
    public void ImageCollection_AddStrip_InvalidWidth_ThrowsArgumentException(int width)
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        using Bitmap image = new(width, 16);
        Assert.Throws<ArgumentException>("value", () => collection.AddStrip(image));
    }

    [WinFormsTheory]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(17)]
    public void ImageCollection_AddStrip_InvalidHeight_ThrowsArgumentException(int width)
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        using Bitmap image = new(16, width);
        Assert.Throws<ArgumentException>("value", () => collection.AddStrip(image));
    }

    [WinFormsFact]
    public void ImageCollection_Clear_InvokeEmpty_Nop()
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
        Assert.False(list.HandleCreated);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
        Assert.False(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageCollection_Clear_InvokeNotEmpty_Success()
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        ImageList.ImageCollection collection = list.Images;
        collection.Add(image);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
        Assert.False(list.HandleCreated);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
        Assert.False(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageCollection_Clear_InvokeEmptyWithHandle_Nop()
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
        Assert.True(list.HandleCreated);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
        Assert.True(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageCollection_Clear_InvokeNotEmptyWithHandle_Success()
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        ImageList.ImageCollection collection = list.Images;
        collection.Add(image);
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
        Assert.True(list.HandleCreated);

        // Clear again.
        collection.Clear();
        Assert.Empty(collection);
        Assert.Empty(collection);
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
        using ImageList list = new();
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
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        using Bitmap image2 = new(3, 4);
        using Bitmap image3 = new(5, 6);
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
    [StringWithNullData]
    public void ImageCollection_ContainsKey_InvokeEmpty_ReturnsExpected(string key)
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;

        Assert.False(collection.ContainsKey(key));

        // Call again.
        Assert.False(collection.ContainsKey(key));
        Assert.False(collection.ContainsKey("NoSuchKey"));
    }

    [WinFormsFact]
    public void ImageListCollection_GetEnumerator_InvokeWithoutHandleEmpty_Success()
    {
        using ImageList list = new();
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
        using ImageList list = new();
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
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
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
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
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
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        using Bitmap image2 = new(3, 4);
        using Bitmap image3 = new(5, 6);
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
    [StringWithNullData]
    public void ImageCollection_IndexOfKey_InvokeEmpty_ReturnsExpected(string key)
    {
        using ImageList list = new();
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
        using ImageList list = new();
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
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<NotSupportedException>(() => collection.Remove(image));
    }

    [WinFormsFact]
    public void ImageListCollection_RemoveAt_InvokeWithoutHandleNotEmpty_Success()
    {
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
        image2.SetPixel(0, 0, Color.Blue);
        using Bitmap image3 = new(3, 4);
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
        Assert.Single(collection);
        Assert.False(collection.Empty);

        // Remove last.
        collection.RemoveAt(0);
        Assert.True(list.HandleCreated);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
    }

    [WinFormsFact]
    public void ImageListCollection_RemoveAt_InvokeWithHandleNotEmpty_Success()
    {
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
        image2.SetPixel(0, 0, Color.Blue);
        using Bitmap image3 = new(3, 4);
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
        Assert.Single(collection);
        Assert.False(collection.Empty);
        Assert.Equal(color3, ((Bitmap)collection[0]).GetPixel(0, 0));

        // Remove last.
        collection.RemoveAt(0);
        Assert.True(list.HandleCreated);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ImageCollection_RemoveAt_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ImageCollection_RemoveAt_InvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection.RemoveAt(index));
    }

    [WinFormsFact]
    public void ImageListCollection_RemoveByKey_InvokeWithoutHandleNotEmpty_Success()
    {
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
        image2.SetPixel(0, 0, Color.Blue);
        using Bitmap image3 = new(3, 4);
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
        Assert.Single(collection);
        Assert.False(collection.Empty);

        // Remove last.
        collection.RemoveByKey("IMAGE3");
        Assert.True(list.HandleCreated);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
    }

    [WinFormsFact]
    public void ImageListCollection_RemoveByKey_InvokeWithHandleNotEmpty_Success()
    {
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
        image2.SetPixel(0, 0, Color.Blue);
        using Bitmap image3 = new(3, 4);
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
        Assert.Single(collection);
        Assert.False(collection.Empty);
        Assert.Equal(color3, ((Bitmap)collection[0]).GetPixel(0, 0));

        // Remove last.
        collection.RemoveByKey("IMAGE3");
        Assert.True(list.HandleCreated);
        Assert.Empty(collection);
        Assert.True(collection.Empty);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NoSuchImage")]
    public void ImageListCollection_RemoveByKey_InvokeNoSuchKey_Nop(string key)
    {
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        image1.SetPixel(0, 0, Color.Red);
        using Bitmap image2 = new(3, 4);
        image2.SetPixel(0, 0, Color.Blue);
        using Bitmap image3 = new(3, 4);
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
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        using Bitmap image2 = new(3, 4);
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
        using ImageList list = new();
        using Bitmap image1 = new(1, 2);
        using Bitmap image2 = new(3, 4);
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
        using ImageList list = new();
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<IndexOutOfRangeException>(() => collection.SetKeyName(index, "name"));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ImageCollection_SetKeyName_InvalidIndexNotEmpty_ThrowsIndexOutOfRangeException(int index)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        ImageList.ImageCollection collection = list.Images;
        Assert.Throws<IndexOutOfRangeException>(() => collection.SetKeyName(index, "name"));
    }

    [WinFormsFact]
    public void ImageCollection_IListIsFixedSize_GetReturnsExpected()
    {
        using ImageList list = new();
        IList collection = list.Images;
        Assert.False(collection.IsFixedSize);
    }

    [WinFormsFact]
    public void ImageCollection_ICollectionIsSynchronized_GetReturnsExpected()
    {
        using ImageList list = new();
        ICollection collection = list.Images;
        Assert.False(collection.IsSynchronized);
    }

    [WinFormsFact]
    public void ImageCollection_ICollectionSyncRoot_GetReturnsExpected()
    {
        using ImageList list = new();
        ICollection collection = list.Images;
        Assert.Same(collection, collection.SyncRoot);
    }

    [WinFormsTheory]
    [MemberData(nameof(Item_Set_TestData))]
    public void ImageCollection_IListItem_Set_GetReturnsExpected(Color transparentColor, Image value)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new()
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
        using Bitmap image = new(10, 10);
        using ImageList list = new()
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
        using ImageList list = new();
        IList collection = list.Images;

        using Bitmap value = new(1, 2);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = value);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ImageCollection_IListItem_SetInvalidIndexNotEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        IList collection = list.Images;

        using Bitmap value = new(1, 2);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => collection[index] = image);
    }

    public static IEnumerable<object[]> IListItem_SetNonImageValue_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListItem_SetNonImageValue_TestData))]
    public void ImageCollection_IListItem_SetNullValue_ThrowsArgumentException(object value)
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        IList collection = list.Images;

        Assert.Throws<ArgumentException>("value", () => collection[0] = value);
    }

    [WinFormsFact]
    public void ImageCollection_IListItem_SetNonBitmapValue_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using ImageList list = new();
        list.Images.Add(image);
        IList collection = list.Images;

        using Metafile value = new("bitmaps/telescope_01.wmf");
        Assert.Throws<ArgumentException>(() => collection[0] = value);
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
        using ImageList list = new();
        IList collection = list.Images;
        Assert.Throws<NotSupportedException>(() => collection.Contains(value));
    }

    public static IEnumerable<object[]> IListContains_NotImage_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Icon("bitmaps/10x16_one_entry_32bit.ico") };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListContains_NotImage_TestData))]
    public void ImageCollection_IListContains_InvokeNotImage_ReturnsFalse(object value)
    {
        using ImageList list = new();
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
        using ImageList list = new();
        IList collection = list.Images;
        Assert.Throws<NotSupportedException>(() => collection.IndexOf(value));
    }

    public static IEnumerable<object[]> IListIndexOf_NotImage_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Icon("bitmaps/10x16_one_entry_32bit.ico") };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListIndexOf_NotImage_TestData))]
    public void ImageCollection_IListIndexOf_InvokeNotImage_ReturnsExpected(object value)
    {
        using ImageList list = new();
        IList collection = list.Images;
        Assert.Equal(-1, collection.IndexOf(value));
    }

    public static IEnumerable<object[]> IListInsert_TestData()
    {
        foreach (int index in new int[] { -1, 0, 1 })
        {
            yield return new object[] { index, null };
            yield return new object[] { index, new() };
            yield return new object[] { index, new Bitmap(10, 10) };
            yield return new object[] { index, new Metafile("bitmaps/telescope_01.wmf") };
            yield return new object[] { index, new Icon("bitmaps/10x16_one_entry_32bit.ico") };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(IListInsert_TestData))]
    public void ImageCollection_IListInsert_Invoke_ThrowsNotSupportedException(int index, object value)
    {
        using ImageList list = new();
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
        using ImageList list = new();
        IList collection = list.Images;
        Assert.Throws<NotSupportedException>(() => collection.Remove(value));
    }

    public static IEnumerable<object[]> IListRemove_NotImage_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new Icon("bitmaps/10x16_one_entry_32bit.ico") };
    }

    [WinFormsTheory]
    [MemberData(nameof(IListRemove_NotImage_TestData))]
    public void ImageCollection_IListRemove_InvokeNotImage_Nop(object value)
    {
        using ImageList list = new();
        IList collection = list.Images;
        collection.Remove(value);
    }
}
