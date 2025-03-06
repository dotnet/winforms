// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms.Design.Tests;

public class ImageListImageEditorTests
{
    [Fact]
    public void ImageListImageEditor_LoadImageFromStream_BitmapStream_ReturnsExpected()
    {
        ImageListImageEditor editor = new();
        var editor_LoadImageFromStream = editor.TestAccessor().CreateDelegate<Func<Stream, bool, ImageListImage>>("LoadImageFromStream");

        using MemoryStream stream = new();
        using Bitmap image = new(10, 10);
        image.Save(stream, ImageFormat.Bmp);
        stream.Position = 0;

        var result = Assert.IsType<ImageListImage>(editor_LoadImageFromStream(stream, false));
        var resultImage = Assert.IsType<Bitmap>(result.Image);
        Assert.Equal(new Size(10, 10), result.Size);
        Assert.Equal(new Size(10, 10), resultImage.Size);

        using MemoryStream resultStream = new();
        result.Image.Save(resultStream, ImageFormat.Bmp);
        Assert.Equal(stream.Length, resultStream.Length);
    }
}
