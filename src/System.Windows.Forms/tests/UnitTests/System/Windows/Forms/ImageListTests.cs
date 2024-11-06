// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // ImageList doesn't appear to behave well under stress in multi-threaded env
public class ImageListTests
{
    [WinFormsFact]
    public void ImageList_Ctor_Default()
    {
        using ImageList list = new();
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
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

    [WinFormsFact]
    public void ImageList_Ctor_IContainer()
    {
        Container container = new();
        using ImageList list = new(container);
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
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

    [WinFormsFact]
    public void ImageList_Ctor_NullContainer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("container", () => new ImageList(null));
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ColorDepth_SetWithoutHandle_GetReturnsExpected(ColorDepth value)
    {
        using ImageList list = new()
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

    [WinFormsFact]
    public void ImageList_ColorDepth_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
    {
        using ImageList list = new();
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

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ColorDepth_SetWithHandle_GetReturnsExpected(ColorDepth value)
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        list.ColorDepth = value;
        Assert.Equal(value, list.ColorDepth);
        Assert.True(list.HandleCreated);

        // Set same.
        list.ColorDepth = value;
        Assert.Equal(value, list.ColorDepth);
        Assert.True(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_ColorDepth_SetWithHandleWithHandler_CallsRecreateHandle()
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(list, sender);
            // Assert.Same(EventArgs.Empty, e);
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

    [WinFormsFact]
    public void ImageList_ColorDepth_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ColorDepth)];
        using ImageList list = new();
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.True(property.CanResetValue(list));

        list.ColorDepth = ColorDepth.Depth8Bit;
        Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
        Assert.True(property.CanResetValue(list));

        property.ResetValue(list);
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.True(property.CanResetValue(list));
    }

    [WinFormsFact]
    public void ImageList_ColorDepth_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ColorDepth)];
        using ImageList list = new();
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.True(property.ShouldSerializeValue(list));

        list.ColorDepth = ColorDepth.Depth8Bit;
        Assert.Equal(ColorDepth.Depth8Bit, list.ColorDepth);
        Assert.True(property.ShouldSerializeValue(list));

        property.ResetValue(list);
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.True(property.ShouldSerializeValue(list));

        // With images.
        list.Images.Add(new Bitmap(10, 10));
        Assert.False(property.ShouldSerializeValue(list));
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_Handle_Get_CreatesHandle(ColorDepth colorDepth)
    {
        using ImageList list = new()
        {
            ColorDepth = colorDepth
        };
        IntPtr handle = list.Handle;
        Assert.True(list.HandleCreated);
        Assert.NotEqual(IntPtr.Zero, handle);
        Assert.Equal(handle, list.Handle);
    }

    [WinFormsTheory]
    [InvalidEnumData<ColorDepth>]
    public void ImageList_ColorDepth_SetInvalid_ThrowsInvalidEnumArgumentException(ColorDepth value)
    {
        using ImageList list = new();
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

    [WinFormsTheory]
    [MemberData(nameof(ImageSize_TestData))]
    public void ImageList_ImageSize_SetWithoutHandle_GetReturnsExpected(Size value)
    {
        using ImageList list = new()
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

    [WinFormsFact]
    public void ImageList_ImageSize_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
    {
        using ImageList list = new();
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

    [WinFormsTheory]
    [MemberData(nameof(ImageSize_TestData))]
    public void ImageList_ImageSize_SetWithHandle_GetReturnsExpected(Size value)
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        list.ImageSize = value;
        Assert.Equal(value, list.ImageSize);
        Assert.True(list.HandleCreated);

        // Set same.
        list.ImageSize = value;
        Assert.Equal(value, list.ImageSize);
        Assert.True(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_ImageSize_SetWithHandleWithHandler_CallsRecreateHandle()
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(list, sender);
            // Assert.Same(EventArgs.Empty, e);
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

    [WinFormsFact]
    public void ImageList_ImageSize_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ImageSize)];
        using ImageList list = new();
        Assert.Equal(new Size(16, 16), list.ImageSize);
        Assert.True(property.CanResetValue(list));

        list.ImageSize = new Size(32, 32);
        Assert.Equal(new Size(32, 32), list.ImageSize);
        Assert.True(property.CanResetValue(list));

        property.ResetValue(list);
        Assert.Equal(new Size(16, 16), list.ImageSize);
        Assert.True(property.CanResetValue(list));
    }

    [WinFormsFact]
    public void ImageList_ImageSize_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.ImageSize)];
        using ImageList list = new();
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

    [WinFormsFact]
    public void ImageList_ImageSize_SetEmpty__ThrowsArgumentException()
    {
        using ImageList list = new();
        Assert.Throws<ArgumentException>("value", () => list.ImageSize = Size.Empty);
    }

    public static IEnumerable<object[]> ImageSize_SetInvalidDimension_TestData()
    {
        yield return new object[] { 0 };
        yield return new object[] { -1 };
        yield return new object[] { 257 };
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageSize_SetInvalidDimension_TestData))]
    public void ImageList_ImageSize_SetInvalidWidth_ThrowsArgumentOutOfRangeException(int width)
    {
        using ImageList list = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => list.ImageSize = new Size(width, 1));
    }

    [WinFormsTheory]
    [MemberData(nameof(ImageSize_SetInvalidDimension_TestData))]
    public void ImageList_ImageSize_SetInvalidHeight_ThrowsArgumentOutOfRangeException(int height)
    {
        using ImageList list = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => list.ImageSize = new Size(1, height));
    }

    [WinFormsFact]
    public void ImageList_ImageStream_GetWithImages_ReturnsExpected()
    {
        using ImageList list = new();
        list.Images.Add(new Bitmap(10, 10));
        Assert.NotNull(list.ImageStream);
        Assert.NotSame(list.ImageStream, list.ImageStream);
        Assert.False(list.HandleCreated);
    }

    private static T RoundtripSerialize<T>(T source)
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        formatter.Serialize(stream, source);
        stream.Position = 0;
        // cs/dangerous-binary-deserialization, cs/deserialization-unexpected-subtypes
        return (T)formatter.Deserialize(stream); // CodeQL [SM03722, SM02229] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
#pragma warning restore SYSLIB0011
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ImageStream_SetStreamerSerialized_UpdatesImages(ColorDepth colorDepth)
    {
        using ImageList sourceList = new()
        {
            ColorDepth = colorDepth,
            ImageSize = new Size(32, 32)
        };
        using Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        using ImageListStreamer stream = RoundtripSerialize(sourceList.ImageStream);
        Assert.True(sourceList.HandleCreated);

        using ImageList list = new();
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

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ImageStream_SetStreamerSerializedDisposed_Nop(ColorDepth colorDepth)
    {
        using ImageList sourceList = new()
        {
            ColorDepth = colorDepth,
            ImageSize = new Size(32, 32)
        };
        Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        ImageListStreamer stream = RoundtripSerialize(sourceList.ImageStream);
        Assert.True(sourceList.HandleCreated);
        stream.Dispose();
        Assert.True(sourceList.HandleCreated);

        using ImageList list = new()
        {
            ImageStream = stream
        };
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.Empty(list.Images);
        Assert.Equal(new Size(16, 16), list.ImageSize);
        Assert.False(list.HandleCreated);
        Assert.True(sourceList.HandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ImageStream_SetStreamerHasHandleNotSerialized_UpdatesImages(ColorDepth colorDepth)
    {
        using ImageList sourceList = new()
        {
            ColorDepth = colorDepth,
            ImageSize = new Size(32, 32)
        };
        Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        Assert.NotEqual(IntPtr.Zero, sourceList.Handle);
        ImageListStreamer stream = sourceList.ImageStream;

        using ImageList list = new()
        {
            ImageStream = stream
        };
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.Empty(list.Images);
        Assert.Equal(new Size(16, 16), list.ImageSize);
        Assert.False(list.HandleCreated);
        Assert.True(sourceList.HandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ImageStream_SetWithHandleStreamerHasHandleNotSerialized_Nop(ColorDepth colorDepth)
    {
        using ImageList sourceList = new()
        {
            ColorDepth = colorDepth,
            ImageSize = new Size(32, 32)
        };
        Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        Assert.NotEqual(IntPtr.Zero, sourceList.Handle);
        ImageListStreamer stream = sourceList.ImageStream;

        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        list.ImageStream = stream;
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.Empty(list.Images);
        Assert.Equal(new Size(16, 16), list.ImageSize);
        Assert.True(list.HandleCreated);
        Assert.True(sourceList.HandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ImageStream_SetStreamerHasNoHandleNotSerialized_Nop(ColorDepth colorDepth)
    {
        using ImageList sourceList = new()
        {
            ColorDepth = colorDepth,
            ImageSize = new Size(32, 32)
        };
        Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        ImageListStreamer stream = sourceList.ImageStream;

        using ImageList list = new()
        {
            ImageStream = stream
        };
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.Empty(list.Images);
        Assert.Equal(new Size(16, 16), list.ImageSize);
        Assert.False(list.HandleCreated);
        Assert.False(sourceList.HandleCreated);
    }

    [WinFormsTheory]
    [EnumData<ColorDepth>]
    public void ImageList_ImageStream_SetWithHandleStreamerHasNoHandleNotSerialized_Nop(ColorDepth colorDepth)
    {
        using ImageList sourceList = new()
        {
            ColorDepth = colorDepth,
            ImageSize = new Size(32, 32)
        };
        Bitmap image = new(10, 10);
        sourceList.Images.Add(image);
        ImageListStreamer stream = sourceList.ImageStream;

        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        list.ImageStream = stream;
        Assert.Equal(ColorDepth.Depth32Bit, list.ColorDepth);
        Assert.Empty(list.Images);
        Assert.Equal(new Size(16, 16), list.ImageSize);
        Assert.True(list.HandleCreated);
        Assert.False(sourceList.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_ImageStream_SetNull_ClearsImages()
    {
        using ImageList list = new();

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

    [WinFormsFact]
    public void ImageList_ImageStream_SetNullWithHandle_ClearsImages()
    {
        using ImageList list = new();
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

    [WinFormsTheory]
    [StringWithNullData]
    public void ImageList_Tag_Set_GetReturnsExpected(object value)
    {
        using ImageList list = new()
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
        foreach (Color testData in CommonTestHelper.GetColorWithEmptyTheoryData())
        {
            yield return [testData];
        }

        yield return new object[] { Color.LightGray };
    }

    [WinFormsTheory]
    [MemberData(nameof(TransparentColor_Set_TestData))]
    public void ImageList_TransparentColor_SetWithoutHandle_GetReturnsExpected(Color value)
    {
        using ImageList list = new()
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

    [WinFormsFact]
    public void ImageList_TransparentColor_SetWithoutHandleWithHandler_DoesNotCallRecreateHandle()
    {
        using ImageList list = new();
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

    [WinFormsTheory]
    [MemberData(nameof(TransparentColor_Set_TestData))]
    public void ImageList_TransparentColor_SetWithHandle_GetReturnsExpected(Color value)
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        list.TransparentColor = value;
        Assert.Equal(value, list.TransparentColor);
        Assert.True(list.HandleCreated);

        // Set same.
        list.TransparentColor = value;
        Assert.Equal(value, list.TransparentColor);
        Assert.True(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_TransparentColor_SetWithHandleWithHandler_DoesNotCallRecreateHandle()
    {
        using ImageList list = new();
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

    [WinFormsFact]
    public void ImageList_TransparentColor_ResetValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.TransparentColor)];
        using ImageList list = new();
        Assert.Equal(Color.Transparent, list.TransparentColor);
        Assert.True(property.CanResetValue(list));

        list.TransparentColor = Color.Red;
        Assert.Equal(Color.Red, list.TransparentColor);
        Assert.True(property.CanResetValue(list));

        property.ResetValue(list);
        Assert.Equal(Color.LightGray, list.TransparentColor);
        Assert.False(property.CanResetValue(list));
    }

    [WinFormsFact]
    public void ImageList_TransparentColor_ShouldSerializeValue_Success()
    {
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ImageList))[nameof(ImageList.TransparentColor)];
        using ImageList list = new();
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

    [WinFormsFact]
    public void ImageList_Dispose_InvokeEmptyWithoutHandle_Nop()
    {
        using ImageList list = new();
        list.Dispose();
        Assert.Empty(list.Images);
        Assert.False(list.HandleCreated);

        list.Dispose();
        Assert.Empty(list.Images);
        Assert.False(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_Dispose_InvokeWithIconsWithoutHandle_Success()
    {
        using Icon icon = new("bitmaps/10x16_one_entry_32bit.ico");
        using Bitmap bitmap = new(10, 10);
        using ImageList list = new();
        list.Images.Add(icon);
        list.Images.Add(bitmap);

        list.Dispose();
        Assert.False(list.HandleCreated);
        Assert.Throws<ObjectDisposedException>(list.Images.GetEnumerator);
        list.Images.Count.Should().Be(0);
        Assert.True(list.HandleCreated);

        // Call again.
        list.Dispose();
        Assert.False(list.HandleCreated);
        Assert.Empty(list.Images);
        Assert.False(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_Dispose_InvokeWithoutIconsWithoutHandle_Success()
    {
        using Bitmap bitmap1 = new(10, 10);
        using Bitmap bitmap2 = new(10, 10);
        using ImageList list = new();
        list.Images.Add(bitmap1);
        list.Images.Add(bitmap2);

        list.Dispose();
        Assert.Equal(2, list.Images.Count);
        Assert.False(list.HandleCreated);

        // Call again.
        list.Dispose();
        Assert.Equal(2, list.Images.Count);
        Assert.False(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_Dispose_InvokeEmptyWithHandle_Nop()
    {
        using ImageList list = new();
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        list.Dispose();
        Assert.Empty(list.Images);
        Assert.False(list.HandleCreated);

        list.Dispose();
        Assert.Empty(list.Images);
        Assert.False(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_Dispose_InvokeNotEmptyWithHandle_Success()
    {
        using Icon icon = new("bitmaps/10x16_one_entry_32bit.ico");
        using Bitmap bitmap = new(10, 10);
        using ImageList list = new();
        list.Images.Add(icon);
        list.Images.Add(bitmap);
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        list.Dispose();
        Assert.Empty(list.Images);
        Assert.False(list.HandleCreated);

        // Call again.
        list.Dispose();
        Assert.Empty(list.Images);
        Assert.False(list.HandleCreated);
    }

#if DEBUG
    [WinFormsFact]
    public void ImageList_Dispose_SetsIsDisposed()
    {
        using ImageList list = new();
        Assert.False(list.IsDisposed);

        list.Dispose();
        Assert.True(list.IsDisposed);
    }
#endif

    public static IEnumerable<object[]> Draw_Point_TestData()
    {
        yield return new object[] { Point.Empty };
        yield return new object[] { new Point(1, 2) };
        yield return new object[] { new Point(-1, -2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_Point_TestData))]
    public void ImageList_Draw_InvokeGraphicsPointIntWithoutHandle_Success(Point pt)
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        list.Draw(graphics, pt, 0);
        Assert.True(list.HandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_Point_TestData))]
    public void ImageList_Draw_InvokeGraphicsPointIntWithHandle_Success(Point pt)
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        list.Draw(graphics, pt, 0);
        Assert.True(list.HandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_Point_TestData))]
    public void ImageList_Draw_InvokeGraphicsIntIntIntWithoutHandle_Success(Point pt)
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        list.Draw(graphics, pt, 0);
        Assert.True(list.HandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_Point_TestData))]
    public void ImageList_Draw_InvokeGraphicsIntIntIntWithHandle_Success(Point pt)
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        list.Draw(graphics, pt.X, pt.Y, 0);
        Assert.True(list.HandleCreated);
    }

    public static IEnumerable<object[]> Draw_WithSize_TestData()
    {
        yield return new object[] { 0, 0, 10, 10 };
        yield return new object[] { 1, 2, 3, 4 };
        yield return new object[] { -1, -2, 3, 4 };
        yield return new object[] { -1, -2, 0, 0 };
        yield return new object[] { -1, -2, -3, -4 };
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_WithSize_TestData))]
    public void ImageList_Draw_InvokeGraphicsIntIntIntIntIntWithoutHandle_Success(int x, int y, int width, int height)
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        list.Draw(graphics, x, y, width, height, 0);
        Assert.True(list.HandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(Draw_WithSize_TestData))]
    public void ImageList_Draw_InvokeGraphicsIntIntIntIntIntWithHandle_Success(int x, int y, int width, int height)
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);
        Assert.NotEqual(IntPtr.Zero, list.Handle);

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        list.Draw(graphics, x, y, width, height, 0);
        Assert.True(list.HandleCreated);
    }

    [WinFormsFact]
    public void ImageList_Draw_NullG_ThrowsNullReferenceException()
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);
        Assert.Throws<NullReferenceException>(() => list.Draw(null, Point.Empty, 0));
        Assert.Throws<NullReferenceException>(() => list.Draw(null, 0, 0, 0));
        Assert.Throws<NullReferenceException>(() => list.Draw(null, 0, 0, 10, 10, 0));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void ImageList_Draw_InvalidIndexEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ImageList list = new();

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Draw(graphics, Point.Empty, index));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Draw(graphics, 0, 0, index));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Draw(graphics, 0, 0, 10, 10, index));
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ImageList_Draw_InvalidIndexNonEmpty_ThrowsArgumentOutOfRangeException(int index)
    {
        using ImageList list = new();
        using Bitmap image = new(10, 10);
        list.Images.Add(image);

        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Draw(graphics, Point.Empty, index));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Draw(graphics, 0, 0, index));
        Assert.Throws<ArgumentOutOfRangeException>("index", () => list.Draw(graphics, 0, 0, 10, 10, index));
    }
}
