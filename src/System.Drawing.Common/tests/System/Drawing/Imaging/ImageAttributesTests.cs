// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Copyright (C) 2005-2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System.Drawing.Drawing2D;

namespace System.Drawing.Imaging.Tests;

public class ImageAttributesTests
{
    private readonly Rectangle _rectangle = new(0, 0, 64, 64);
    private readonly Color _actualYellow = Color.FromArgb(255, 255, 255, 0);
    private readonly Color _actualGreen = Color.FromArgb(255, 0, 255, 0);
    private readonly Color _expectedRed = Color.FromArgb(255, 255, 0, 0);
    private readonly Color _expectedBlack = Color.FromArgb(255, 0, 0, 0);
    private readonly ColorMatrix _greenComponentToZeroColorMatrix = new(
    [
        [1, 0, 0, 0, 0],
        [0, 0, 0, 0, 0],
        [0, 0, 1, 0, 0],
        [0, 0, 0, 1, 0],
        [0, 0, 0, 0, 0],
    ]);

    private readonly ColorMatrix _grayMatrix = new(
    [
        [1, 0, 0, 0, 0],
        [0, 2, 0, 0, 0],
        [0, 0, 3, 0, 0],
        [0, 0, 0, 1, 0],
        [0, 0, 0, 0, 0],
    ]);

    private readonly ColorMap[] _yellowToRedColorMap =
    [
        new() { OldColor = Color.FromArgb(255, 255, 255, 0), NewColor = Color.FromArgb(255, 255, 0, 0) }
    ];

    [Fact]
    public void Ctor_Default_Success()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();
    }

    [Fact]
    public void Clone_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix);

        using ImageAttributes clone = Assert.IsAssignableFrom<ImageAttributes>(imageAttr.Clone());
        bitmap.SetPixel(0, 0, _actualYellow);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, clone);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void Clone_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.Clone);
    }

    [Fact]
    public void SetColorMatrix_ColorMatrix_Success()
    {
        using SolidBrush brush = new(_actualGreen);
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix);
        bitmap.SetPixel(0, 0, _actualYellow);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));

        graphics.FillRectangle(brush, _rectangle);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedBlack, bitmap.GetPixel(0, 0));
    }

    public static IEnumerable<object[]> ColorMatrix_DropShadowRepaintWhenAreaIsSmallerThanTheFilteredElement_TestData()
    {
        yield return new object[] { Color.FromArgb(100, 255, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 155, 155) };
    }

    [Theory]
    [MemberData(nameof(ColorMatrix_DropShadowRepaintWhenAreaIsSmallerThanTheFilteredElement_TestData))]
    public void SetColorMatrix_ColorMatrixI_Success(Color color)
    {
        ColorMatrix colorMatrix = new(
        [
            [1, 0, 0, 0, 0],
            [0, 1, 0, 0, 0],
            [0, 0, 1, 0, 0],
            [0, 0, 0, 0.5f, 0],
            [0, 0, 0, 0, 1],
        ]);

        using SolidBrush brush = new(color);
        using Bitmap bitmapBig = new(200, 100);
        using Bitmap bitmapSmall = new(100, 100);
        using var graphicsSmallBitmap = Graphics.FromImage(bitmapSmall);
        using var graphicsBigBitmap = Graphics.FromImage(bitmapBig);
        using ImageAttributes imageAttr = new();
        graphicsSmallBitmap.FillRectangle(Brushes.White, 0, 0, 100, 100);
        graphicsSmallBitmap.FillEllipse(brush, 0, 0, 100, 100);
        graphicsBigBitmap.FillRectangle(Brushes.White, 0, 0, 200, 100);
        imageAttr.SetColorMatrix(colorMatrix);
        graphicsBigBitmap.DrawImage(bitmapSmall, new Rectangle(0, 0, 100, 100), 0, 0, 100, 100, GraphicsUnit.Pixel, null);
        graphicsBigBitmap.DrawImage(bitmapSmall, new Rectangle(100, 0, 100, 100), 0, 0, 100, 100, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 255, 155, 155), bitmapBig.GetPixel(50, 50));
        Assert.Equal(Color.FromArgb(255, 255, 205, 205), bitmapBig.GetPixel(150, 50));
    }

    [Fact]
    public void SetColorMatrix_ColorMatrixFlags_Success()
    {
        var grayShade = Color.FromArgb(255, 100, 100, 100);

        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        bitmap.SetPixel(0, 0, _actualYellow);
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));

        bitmap.SetPixel(0, 0, grayShade);
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.SkipGrays);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(grayShade, bitmap.GetPixel(0, 0));
    }

    public static IEnumerable<object[]> ColorAdjustType_TestData()
    {
        yield return new object[] { ColorAdjustType.Default };
        yield return new object[] { ColorAdjustType.Bitmap };
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_TestData))]
    public void SetColorMatrix_ColorMatrixDefaultFlagType_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using SolidBrush brush = new(_actualYellow);
        using Pen pen = new(brush);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, type);

        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedBlack, bitmap.GetPixel(0, 0));

        graphics.FillRectangle(brush, _rectangle);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));

        graphics.DrawRectangle(pen, _rectangle);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));
    }

    public static IEnumerable<object[]> ColorAdjustTypeI_TestData()
    {
        yield return new object[] { ColorAdjustType.Brush };
        yield return new object[] { ColorAdjustType.Pen };
        yield return new object[] { ColorAdjustType.Text };
    }

    [Theory]
    [MemberData(nameof(ColorAdjustTypeI_TestData))]
    public void SetColorMatrix_ColorMatrixDefaultFlagTypeI_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using SolidBrush brush = new(_actualYellow);
        using Pen pen = new(brush);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, type);

        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualGreen, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetColorMatrix_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default));
    }

    [Fact]
    public void SetColorMatrix_NullMatrix_ThrowsArgumentException()
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentNullException>("newColorMatrix", () => imageAttr.SetColorMatrix(null));
        AssertExtensions.Throws<ArgumentNullException>("newColorMatrix", () => imageAttr.SetColorMatrix(null, ColorMatrixFlag.Default));
        AssertExtensions.Throws<ArgumentNullException>("newColorMatrix", () =>
            imageAttr.SetColorMatrix(null, ColorMatrixFlag.Default, ColorAdjustType.Default));
    }

    public static IEnumerable<object[]> ColorAdjustType_InvalidTypes_TestData()
    {
        yield return new object[] { (ColorAdjustType.Default - 1) };
        yield return new object[] { ColorAdjustType.Count };
        yield return new object[] { ColorAdjustType.Any };
        yield return new object[] { (ColorAdjustType.Any + 1) };
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetColorMatrix_InvalidTypes_ThrowsInvalidEnumArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, type));
    }

    public static IEnumerable<object[]> ColorMatrixFlag_InvalidFlags_TestData()
    {
        yield return new object[] { (ColorMatrixFlag.Default - 1) };
        yield return new object[] { ColorMatrixFlag.AltGrays };
        yield return new object[] { (ColorMatrixFlag.AltGrays + 1) };
        yield return new object[] { (ColorMatrixFlag)int.MinValue };
        yield return new object[] { (ColorMatrixFlag)int.MaxValue };
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetColorMatrix_InvalidFlags_ThrowsArgumentException(ColorMatrixFlag flag)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, flag));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, flag, ColorAdjustType.Default));
    }

    [Fact]
    public void ClearColorMatrix_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix);
        imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix);
        imageAttr.ClearColorMatrix();

        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualGreen, bitmap.GetPixel(0, 0));
    }

    public static IEnumerable<object[]> ColorAdjustType_AllTypesAllowed_TestData()
    {
        yield return new object[] { ColorAdjustType.Default };
        yield return new object[] { ColorAdjustType.Bitmap };
        yield return new object[] { ColorAdjustType.Brush };
        yield return new object[] { ColorAdjustType.Pen };
        yield return new object[] { ColorAdjustType.Text };
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void ClearColorMatrix_DefaultFlagType_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using SolidBrush brush = new(_actualYellow);
        using Pen pen = new(brush);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, type);
        imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, ColorMatrixFlag.Default, type);
        imageAttr.ClearColorMatrix(type);

        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualGreen, bitmap.GetPixel(0, 0));

        graphics.FillRectangle(brush, _rectangle);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualYellow, bitmap.GetPixel(0, 0));

        graphics.DrawRectangle(pen, _rectangle);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualYellow, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearColorMatrix_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.ClearColorMatrix);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearColorMatrix(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearColorMatrix_InvalidTypes_ThrowsInvalidEnumArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearColorMatrix(type));
    }

    [Fact]
    public void SetColorMatrices_ColorMatrixGrayMatrix_Success()
    {
        using SolidBrush brush = new(_actualGreen);
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix);
        bitmap.SetPixel(0, 0, _actualYellow);
        bitmap.SetPixel(1, 1, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));
        Assert.Equal(Color.FromArgb(255, 100, 0, 100), bitmap.GetPixel(1, 1));
    }

    public static IEnumerable<object[]> SetColorMatrices_Flags_TestData()
    {
        yield return new object[] { ColorMatrixFlag.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 0, 100) };
        yield return new object[] { ColorMatrixFlag.SkipGrays, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.AltGrays, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 200, 255) };
    }

    [Theory]
    [MemberData(nameof(SetColorMatrices_Flags_TestData))]
    public void SetColorMatrices_ColorMatrixGrayMatrixFlags_Success(ColorMatrixFlag flag, Color grayShade, Color expectedGrayShade)
    {
        using SolidBrush brush = new(_actualGreen);
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, flag);
        bitmap.SetPixel(0, 0, _actualYellow);
        bitmap.SetPixel(1, 1, grayShade);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));
        Assert.Equal(expectedGrayShade, bitmap.GetPixel(1, 1));
    }

    public static IEnumerable<object[]> SetColorMatrices_FlagsTypes_TestData()
    {
        yield return new object[] { ColorMatrixFlag.Default, ColorAdjustType.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 0, 100) };
        yield return new object[] { ColorMatrixFlag.SkipGrays, ColorAdjustType.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.AltGrays, ColorAdjustType.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 200, 255) };
        yield return new object[] { ColorMatrixFlag.Default, ColorAdjustType.Bitmap, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 0, 100) };
        yield return new object[] { ColorMatrixFlag.SkipGrays, ColorAdjustType.Bitmap, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.AltGrays, ColorAdjustType.Bitmap, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 100, 200, 255) };
    }

    [Theory]
    [MemberData(nameof(SetColorMatrices_FlagsTypes_TestData))]
    public void SetColorMatrices_ColorMatrixGrayMatrixFlagsTypes_Success
        (ColorMatrixFlag flag, ColorAdjustType type, Color grayShade, Color expectedGrayShade)
    {
        using SolidBrush brush = new(_actualGreen);
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, flag, type);
        bitmap.SetPixel(0, 0, _actualYellow);
        bitmap.SetPixel(1, 1, grayShade);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedRed, bitmap.GetPixel(0, 0));
        Assert.Equal(expectedGrayShade, bitmap.GetPixel(1, 1));
    }

    public static IEnumerable<object[]> SetColorMatrices_FlagsTypesI_TestData()
    {
        yield return new object[] { ColorMatrixFlag.Default, ColorAdjustType.Pen, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.SkipGrays, ColorAdjustType.Pen, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.AltGrays, ColorAdjustType.Pen, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.Default, ColorAdjustType.Brush, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.SkipGrays, ColorAdjustType.Brush, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.AltGrays, ColorAdjustType.Brush, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.Default, ColorAdjustType.Text, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.SkipGrays, ColorAdjustType.Text, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorMatrixFlag.AltGrays, ColorAdjustType.Text, Color.FromArgb(255, 100, 100, 100) };
    }

    [Theory]
    [MemberData(nameof(SetColorMatrices_FlagsTypesI_TestData))]
    public void SetColorMatrices_ColorMatrixGrayMatrixFlagsTypesI_Success(ColorMatrixFlag flag, ColorAdjustType type, Color grayShade)
    {
        using SolidBrush brush = new(_actualGreen);
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, flag, type);
        bitmap.SetPixel(0, 0, _actualYellow);
        bitmap.SetPixel(1, 1, grayShade);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualYellow, bitmap.GetPixel(0, 0));
        Assert.Equal(grayShade, bitmap.GetPixel(1, 1));
    }

    [Fact]
    public void SetColorMatrices_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, ColorMatrixFlag.Default));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default));
    }

    [Fact]
    public void SetColorMatrices_NullMatrices_ThrowsArgumentException()
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentNullException>("newColorMatrix", () => imageAttr.SetColorMatrices(null, _grayMatrix));
        AssertExtensions.Throws<ArgumentNullException>("newColorMatrix", () => imageAttr.SetColorMatrices(null, _grayMatrix, ColorMatrixFlag.Default));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, null, ColorMatrixFlag.AltGrays));
        AssertExtensions.Throws<ArgumentNullException>("newColorMatrix", () =>
            imageAttr.SetColorMatrices(null, _grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, null, ColorMatrixFlag.AltGrays, ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetColorMatrices_InvalidTypes_ThrowsInvalidEnumArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, ColorMatrixFlag.Default, type));
    }

    [Theory]
    [InlineData(ColorMatrixFlag.Default - 1)]
    [InlineData(ColorMatrixFlag.AltGrays + 1)]
    [InlineData((ColorMatrixFlag)int.MinValue)]
    [InlineData((ColorMatrixFlag)int.MaxValue)]
    public void SetColorMatrices_InvalidFlags_ThrowsArgumentException(ColorMatrixFlag flag)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, flag));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetColorMatrices(_greenComponentToZeroColorMatrix, _grayMatrix, flag, ColorAdjustType.Default));
    }

    [Fact]
    public void SetThreshold_Threshold_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetThreshold(0.7f);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 230, 50, 220));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 255, 0, 255), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_TestData))]
    public void SetThreshold_ThresholdType_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetThreshold(0.7f, type);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 230, 50, 220));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 255, 0, 255), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustTypeI_TestData))]
    public void SetThreshold_ThresholdTypeI_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetThreshold(0.7f, type);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 230, 50, 220));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 230, 50, 220), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetThreshold_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetThreshold(0.5f));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetThreshold_InvalidType_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetThreshold(0.5f, type));
    }

    [Fact]
    public void ClearThreshold_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetThreshold(0.7f);
        imageAttr.ClearThreshold();
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 230, 50, 220));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 230, 50, 220), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void ClearThreshold_ThresholdTypeI_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetThreshold(0.7f, type);
        imageAttr.ClearThreshold(type);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 230, 50, 220));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 230, 50, 220), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearThreshold_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearThreshold(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearThreshold_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearThreshold(type));
    }

    [Fact]
    public void SetGamma_Gamma_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 255, 0));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 33, 255, 0), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_TestData))]
    public void SetGamma_GammaType_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f, type);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 255, 0));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 33, 255, 0), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustTypeI_TestData))]
    public void SetGamma_GammaTypeI_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f, type);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 255, 0));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 255, 0), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetGamma_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetGamma(2.2f));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetGamma(2.2f, ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetGamma_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetGamma(2.2f, type));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void ClearGamma_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f, type);
        imageAttr.ClearGamma(type);

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 255, 0));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 255, 0), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearGamma_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearGamma(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearGamma_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearGamma(type));
    }

    [Fact]
    public void SetNoOp_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f);
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix);
        imageAttr.SetNoOp();
        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualGreen, bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void SetNoOp_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f, type);
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, type);
        imageAttr.SetNoOp(type);

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 255, 0));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 255, 0), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetNoOp_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.SetNoOp);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetNoOp(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetNoOp_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetNoOp(type));
    }

    [Fact]
    public void ClearNoOp_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f);
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix);
        imageAttr.SetNoOp();
        imageAttr.ClearNoOp();

        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_expectedBlack, bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_TestData))]
    public void ClearNoOp_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f, type);
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, type);
        imageAttr.SetNoOp(type);
        imageAttr.ClearNoOp(type);

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 255, 0));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 33, 0, 0), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustTypeI_TestData))]
    public void ClearNoOp_TypeI_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetGamma(2.2f, type);
        imageAttr.SetColorMatrix(_greenComponentToZeroColorMatrix, ColorMatrixFlag.Default, type);
        imageAttr.SetNoOp(type);
        imageAttr.ClearNoOp(type);

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 255, 0));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 255, 0), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearNoOp_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.ClearNoOp);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearNoOp(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearNoOp_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearNoOp(type));
    }

    [Fact]
    public void SetColorKey_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();

        imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150));

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 100, 100), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_TestData))]
    public void SetColorKey_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();

        imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150), type);

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 100, 100), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustTypeI_TestData))]
    public void SetColorKey_TypeI_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150), type);

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 100, 100), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetColorKey_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150)));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150), ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetColorKey_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150), type));
    }

    [Fact]
    public void ClearColorKey_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150));
        imageAttr.ClearColorKey();

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 100, 100), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void ClearColorKey_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetColorKey(Color.FromArgb(50, 50, 50), Color.FromArgb(150, 150, 150), type);
        imageAttr.ClearColorKey(type);

        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 100, 100), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearColorKey_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.ClearColorKey);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearColorKey(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearColorKey_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearColorKey(type));
    }

    public static IEnumerable<object[]> SetOutputChannel_ColorChannelFlag_TestData()
    {
        yield return new object[] { ColorChannelFlag.ColorChannelC, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 198, 198, 198) };
        yield return new object[] { ColorChannelFlag.ColorChannelK, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 108, 108, 108) };
        yield return new object[] { ColorChannelFlag.ColorChannelM, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 204, 204, 204) };
        yield return new object[] { ColorChannelFlag.ColorChannelY, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 207, 207, 207) };
    }

    [Theory]
    [MemberData(nameof(SetOutputChannel_ColorChannelFlag_TestData))]
    public void SetOutputChannel_Flag_Success(ColorChannelFlag flag, Color actualColor, Color expectedColor)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(flag);

        bitmap.SetPixel(0, 0, actualColor);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(expectedColor, bitmap.GetPixel(0, 0));
    }

    public static IEnumerable<object[]> SetOutputChannel_ColorChannelFlagType_TestData()
    {
        yield return new object[] { ColorChannelFlag.ColorChannelC, ColorAdjustType.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 198, 198, 198) };
        yield return new object[] { ColorChannelFlag.ColorChannelK, ColorAdjustType.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 108, 108, 108) };
        yield return new object[] { ColorChannelFlag.ColorChannelM, ColorAdjustType.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 204, 204, 204) };
        yield return new object[] { ColorChannelFlag.ColorChannelY, ColorAdjustType.Default, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 207, 207, 207) };
        yield return new object[] { ColorChannelFlag.ColorChannelC, ColorAdjustType.Bitmap, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 198, 198, 198) };
        yield return new object[] { ColorChannelFlag.ColorChannelK, ColorAdjustType.Bitmap, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 108, 108, 108) };
        yield return new object[] { ColorChannelFlag.ColorChannelM, ColorAdjustType.Bitmap, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 204, 204, 204) };
        yield return new object[] { ColorChannelFlag.ColorChannelY, ColorAdjustType.Bitmap, Color.FromArgb(255, 100, 100, 100), Color.FromArgb(255, 207, 207, 207) };
    }

    [Theory]
    [MemberData(nameof(SetOutputChannel_ColorChannelFlagType_TestData))]
    public void SetOutputChannel_FlagType_Success(ColorChannelFlag flag, ColorAdjustType type, Color actualColor, Color expectedColor)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(flag, type);

        bitmap.SetPixel(0, 0, actualColor);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(expectedColor, bitmap.GetPixel(0, 0));
    }

    public static IEnumerable<object[]> SetOutputChannel_ColorChannelFlagTypeI_TestData()
    {
        yield return new object[] { ColorChannelFlag.ColorChannelC, ColorAdjustType.Brush, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelK, ColorAdjustType.Brush, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelM, ColorAdjustType.Brush, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelY, ColorAdjustType.Brush, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelC, ColorAdjustType.Pen, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelK, ColorAdjustType.Pen, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelM, ColorAdjustType.Pen, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelY, ColorAdjustType.Pen, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelC, ColorAdjustType.Text, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelK, ColorAdjustType.Text, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelM, ColorAdjustType.Text, Color.FromArgb(255, 100, 100, 100) };
        yield return new object[] { ColorChannelFlag.ColorChannelY, ColorAdjustType.Text, Color.FromArgb(255, 100, 100, 100) };
    }

    [Theory]
    [MemberData(nameof(SetOutputChannel_ColorChannelFlagTypeI_TestData))]
    public void SetOutputChannel_FlagTypeI_Success(ColorChannelFlag flag, ColorAdjustType type, Color color)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(flag, type);

        bitmap.SetPixel(0, 0, color);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(color, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetOutputChannel_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelY));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelY, ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetOutputChannel_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelY, type));
    }

    public static IEnumerable<object[]> SetOutputChannel_InvalidColorChannelFlags_TestData()
    {
        yield return new object[] { (ColorChannelFlag)int.MinValue };
        yield return new object[] { ColorChannelFlag.ColorChannelC - 1 };
        yield return new object[] { ColorChannelFlag.ColorChannelLast };
        yield return new object[] { ColorChannelFlag.ColorChannelLast + 1 };
        yield return new object[] { (ColorChannelFlag)int.MaxValue };
    }

    [Theory]
    [MemberData(nameof(SetOutputChannel_InvalidColorChannelFlags_TestData))]
    public void SetOutputChannel_InvalidFlags_ThrowsArgumentException(ColorChannelFlag flag)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetOutputChannel(flag));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetOutputChannel(flag, ColorAdjustType.Default));
    }

    [Fact]
    public void ClearOutputChannel_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelC);
        imageAttr.ClearOutputChannel();

        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualGreen, bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void ClearOutputChannel_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelC, type);
        imageAttr.ClearOutputChannel(type);

        bitmap.SetPixel(0, 0, _actualGreen);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_actualGreen, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearOutputChannel_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.ClearOutputChannel);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearOutputChannel(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearOutputChannel_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearOutputChannel(type));
    }

    [Fact]
    public void SetOutputChannelColorProfile_Name_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelC);
        imageAttr.SetOutputChannelColorProfile(Helpers.GetTestColorProfilePath("RSWOP.icm"));
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 198, 198, 198), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetOutputChannelColorProfile_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetOutputChannelColorProfile(Helpers.GetTestColorProfilePath("RSWOP.icm")));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            imageAttr.SetOutputChannelColorProfile(Helpers.GetTestColorProfilePath("RSWOP.icm"), ColorAdjustType.Default));
    }

    [Fact]
    public void SetOutputChannelColorProfile_Null_ThrowsArgumentNullException()
    {
        using ImageAttributes imageAttr = new();
        Assert.Throws<ArgumentNullException>(() => imageAttr.SetOutputChannelColorProfile(null));
        Assert.Throws<ArgumentNullException>(() => imageAttr.SetOutputChannelColorProfile(null, ColorAdjustType.Default));
    }

    [Fact]
    public void SetOutputChannelColorProfile_InvalidPath_ThrowsArgumentException()
    {
        using ImageAttributes imageAttr = new();
        Assert.Throws<ArgumentException>(() => imageAttr.SetOutputChannelColorProfile(string.Empty));
        Assert.Throws<ArgumentException>(() => imageAttr.SetOutputChannelColorProfile(string.Empty, ColorAdjustType.Default));
    }

    [Fact]
    public void SetOutputChannelColorProfile_InvalidPath_ThrowsOutOfMemoryException()
    {
        using ImageAttributes imageAttr = new();
        Assert.Throws<OutOfMemoryException>(() => imageAttr.SetOutputChannelColorProfile("invalidPath"));
        Assert.Throws<OutOfMemoryException>(() => imageAttr.SetOutputChannelColorProfile("invalidPath", ColorAdjustType.Default));
    }

    [Fact]
    public void SetOutputChannelColorProfile_InvalidPath_ThrowsPathTooLongException()
    {
        string fileNameTooLong = new('a', short.MaxValue);
        using ImageAttributes imageAttr = new();
        Assert.Throws<PathTooLongException>(() => imageAttr.SetOutputChannelColorProfile(fileNameTooLong));
        Assert.Throws<PathTooLongException>(() => imageAttr.SetOutputChannelColorProfile(fileNameTooLong, ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetOutputChannelColorProfile_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetOutputChannelColorProfile("path", type));
    }

    [Fact]
    public void ClearOutputChannelColorProfile_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelC);
        imageAttr.SetOutputChannelColorProfile(Helpers.GetTestColorProfilePath("RSWOP.icm"));
        imageAttr.ClearOutputChannelColorProfile();
        imageAttr.ClearOutputChannel();
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 100, 100), bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void ClearOutputChannelColorProfile_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetOutputChannel(ColorChannelFlag.ColorChannelC, type);
        imageAttr.SetOutputChannelColorProfile(Helpers.GetTestColorProfilePath("RSWOP.icm"), type);
        imageAttr.ClearOutputChannelColorProfile(type);
        imageAttr.ClearOutputChannel(type);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 100, 100, 100));
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(Color.FromArgb(255, 100, 100, 100), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearOutputChannelColorProfile_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.ClearOutputChannelColorProfile);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearOutputChannelColorProfile(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearOutputChannelColorProfile_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearOutputChannelColorProfile(type));
    }

    [Fact]
    public void SetRemapTable_Map_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetRemapTable(_yellowToRedColorMap);
        bitmap.SetPixel(0, 0, _yellowToRedColorMap[0].OldColor);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_yellowToRedColorMap[0].NewColor, bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_TestData))]
    public void SetRemapTable_MapType_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetRemapTable(_yellowToRedColorMap, type);
        bitmap.SetPixel(0, 0, _yellowToRedColorMap[0].OldColor);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_yellowToRedColorMap[0].NewColor, bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustTypeI_TestData))]
    public void SetRemapTable_MapTypeI_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetRemapTable(_yellowToRedColorMap, type);
        bitmap.SetPixel(0, 0, _yellowToRedColorMap[0].OldColor);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_yellowToRedColorMap[0].OldColor, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void SetRemapTable_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetRemapTable(_yellowToRedColorMap));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetRemapTable(_yellowToRedColorMap, ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void SetRemapTable_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.ThrowsAny<ArgumentNullException, ArgumentException>(() => imageAttr.SetRemapTable(_yellowToRedColorMap, type));
    }

    [Fact]
    public void SetRemapTable_NullMap_ThrowsArgumentNullException()
    {
        using ImageAttributes imageAttr = new();
        Assert.Throws<ArgumentNullException>(() => imageAttr.SetRemapTable(null, ColorAdjustType.Default));
    }

    [Fact]
    public void SetRemapTable_NullMapMeber_ThrowsNullReferenceException()
    {
        using ImageAttributes imageAttr = new();
        Assert.Throws<NullReferenceException>(() => imageAttr.SetRemapTable([null], ColorAdjustType.Default));
    }

    [Fact]
    public void SetRemapTable_EmptyMap_ThrowsArgumentException()
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetRemapTable([], ColorAdjustType.Default));
    }

    [Fact]
    public void ClearRemapTable_Success()
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetRemapTable(_yellowToRedColorMap);
        imageAttr.ClearRemapTable();
        bitmap.SetPixel(0, 0, _yellowToRedColorMap[0].OldColor);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_yellowToRedColorMap[0].OldColor, bitmap.GetPixel(0, 0));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_AllTypesAllowed_TestData))]
    public void ClearRemapTable_Type_Success(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using ImageAttributes imageAttr = new();
        imageAttr.SetRemapTable(_yellowToRedColorMap, type);
        imageAttr.ClearRemapTable(type);
        bitmap.SetPixel(0, 0, _yellowToRedColorMap[0].OldColor);
        graphics.DrawImage(bitmap, _rectangle, _rectangle.X, _rectangle.Y, _rectangle.Width, _rectangle.Height, GraphicsUnit.Pixel, imageAttr);
        Assert.Equal(_yellowToRedColorMap[0].OldColor, bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void ClearRemapTable_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, imageAttr.ClearRemapTable);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearRemapTable(ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void ClearRemapTable_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.ClearRemapTable(type));
    }

    [Fact]
    public void SetWrapMode_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetWrapMode(WrapMode.Clamp));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetWrapMode(WrapMode.Clamp, Color.Black));
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.SetWrapMode(WrapMode.Clamp, Color.Black, true));
    }

    [Fact]
    public void GetAdjustedPalette_Disposed_ThrowsArgumentException()
    {
        ImageAttributes imageAttr = new();
        imageAttr.Dispose();

        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.GetAdjustedPalette(bitmap.Palette, ColorAdjustType.Default));
    }

    [Fact]
    public void GetAdjustedPalette_NullPallete_ThrowsNullReferenceException()
    {
        using ImageAttributes imageAttr = new();
        Assert.Throws<NullReferenceException>(() => imageAttr.GetAdjustedPalette(null, ColorAdjustType.Default));
    }

    [Theory]
    [MemberData(nameof(ColorAdjustType_InvalidTypes_TestData))]
    public void GetAdjustedPalette_InvalidTypes_ThrowsArgumentException(ColorAdjustType type)
    {
        using Bitmap bitmap = new(_rectangle.Width, _rectangle.Height);
        using ImageAttributes imageAttr = new();
        AssertExtensions.Throws<ArgumentException>(null, () => imageAttr.GetAdjustedPalette(bitmap.Palette, type));
    }
}
