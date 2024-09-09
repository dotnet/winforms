// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace System.Windows.Forms.Design.Tests;

public sealed class ImageListImageTests
{
    [Fact]
    public void Constructor_WithImage_ShouldSetImageProperty()
    {
        using Bitmap bitmap = new(10, 10);
        ImageListImage imageListImage = new(bitmap);

        imageListImage.Image.Should().Be(bitmap);
        imageListImage.Name.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithImageAndName_ShouldSetImageAndNameProperties()
    {
        using Bitmap bitmap = new(10, 10);
        string name = "TestImage";
        ImageListImage imageListImage = new(bitmap, name);

        imageListImage.Image.Should().Be(bitmap);
        imageListImage.Name.Should().Be(name);
    }

    [Fact]
    public void NameProperty_ShouldGetAndSetCorrectly()
    {
        using Bitmap bitmap = new(10, 10);
        string name = "TestImage";
        ImageListImage imageListImage = new(bitmap)
        {
            Name = name
        };

        imageListImage.Name.Should().Be(name);
    }

    [Fact]
    public void NameProperty_ShouldReturnEmptyString_WhenNameIsNull()
    {
        using Bitmap bitmap = new(10, 10);
        ImageListImage imageListImage = new(bitmap)
        {
            Name = null
        };

        imageListImage.Name.Should().BeEmpty();
    }

    [Fact]
    public void ImageListImageFromStream_ShouldCreateImageListImageFromBitmapStream()
    {
        using Bitmap bitmap = new(10, 10);
        using MemoryStream memoryStream = new();
        bitmap.Save(memoryStream, Drawing.Imaging.ImageFormat.Bmp);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var result = ImageListImage.ImageListImageFromStream(memoryStream, imageIsIcon: false);

        result.Should().NotBeNull();
        result.Image.Should().NotBeNull();
        result.Image.Size.Should().Be(bitmap.Size);
        result.Image.RawFormat.Should().Be(expected: Drawing.Imaging.ImageFormat.Bmp);
    }

    [Fact]
    public void ImageListImageFromStream_ShouldCreateImageListImageFromIconStream()
    {
        using Icon icon = SystemIcons.Application;
        using MemoryStream memoryStream = new();
        icon.Save(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        var result = ImageListImage.ImageListImageFromStream(memoryStream, imageIsIcon: true);

        result.Should().NotBeNull();
        result.Image.Should().NotBeNull();
        result.Image.Size.Should().Be(icon.Size);
        result.Image.RawFormat.Should().Be(Drawing.Imaging.ImageFormat.MemoryBmp);
    }
}
