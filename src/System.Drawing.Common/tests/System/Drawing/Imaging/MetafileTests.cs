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

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging.Tests;

public class MetafileTests
{
    private const string WmfFile = "telescope_01.wmf";
    private const string BmpFile = "bitmap_173x183_indexed_8bit.bmp";
    private readonly Rectangle _rectangle = new(0, 0, 64, 64);
    private readonly RectangleF _rectangleF = new(0, 0, 64, 64);

    [Fact]
    public void Ctor_IntPtrZero_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(IntPtr.Zero, false));
    }

    [Fact]
    public void Ctor_IntPtrToWmf_ThrowsExternalException()
    {
        using Metafile metafile = new(GetPath(WmfFile));
        Assert.Throws<ExternalException>(() => new Metafile(metafile.GetHenhmetafile(), false));
    }

    [Fact]
    public void Ctor_String_Success()
    {
        using Metafile metafile = new(GetPath(WmfFile));
        AssertMetafile(metafile);
    }

    [Fact]
    public void Ctor_Bitmap_ThrowsExternalException()
    {
        Assert.Throws<ExternalException>(() => new Metafile(GetPath(BmpFile)));
    }

    [Fact]
    public void Ctor_NullString_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("path", () => new Metafile((string)null));
    }

    [Fact]
    public void Ctor_InvalidPath_ThrowsExternalException()
    {
        Assert.Throws<ExternalException>(() => new Metafile("fileNotExist"));
    }

    public static IEnumerable<object[]> InvalidPath_TestData()
    {
        yield return new object[] { new string('a', 261) };
        yield return new object[] { @"fileNo*-//\\#@(found" };
        yield return new object[] { string.Empty };
    }

    [Theory]
    [InlineData("bad\0name")]
    [InlineData("")]
    public void Ctor_InvalidPath_ThrowsArgumentException(string path)
    {
        AssertExtensions.Throws<ArgumentException>("path", null, () => new Metafile(path));
    }

    [Fact]
    public void Ctor_Stream_Success()
    {
        using FileStream stream = File.OpenRead(GetPath(WmfFile));
        using Metafile metafile = new(stream);
        AssertMetafile(metafile);
    }

    [Fact]
    public void Ctor_NullStream_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentNullException, ArgumentException>("stream", null, () => new Metafile((Stream)null));
    }

    [Fact]
    public void Ctor_EmptyStream_ThrowsExternalException()
    {
        using MemoryStream stream = new();
        Assert.Throws<ExternalException>(() => new Metafile(stream));
    }

    public static IEnumerable<object[]> EmfType_TestData()
    {
        yield return new object[] { EmfType.EmfOnly };
        yield return new object[] { EmfType.EmfPlusDual };
        yield return new object[] { EmfType.EmfPlusOnly };
    }

    [Theory]
    [MemberData(nameof(EmfType_TestData))]
    public void Ctor_IntPtrEmfType_Success(EmfType emfType)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), emfType);
        AssertMetafileIsBlank(metafile);
        AssertEmfType(metafile.GetMetafileHeader(), emfType);
    }

    public static IEnumerable<object[]> EmfType_Invalid_TestData()
    {
        yield return new object[] { (EmfType.EmfOnly - 1) };
        yield return new object[] { (EmfType.EmfPlusDual + 1) };
        yield return new object[] { (EmfType)int.MaxValue };
        yield return new object[] { (EmfType)int.MinValue };
    }

    [Theory]
    [MemberData(nameof(EmfType_Invalid_TestData))]
    public void Ctor_IntPtrInvalidEmfType_ThrowsArgumentException(EmfType emfType)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(g.GetHdc(), emfType));
    }

    [Fact]
    public void Ctor_NullEmfType_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile((IntPtr)null, EmfType.EmfOnly));
    }

    [Fact]
    public void Ctor_ZeroPointerEmfType_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(IntPtr.Zero, EmfType.EmfOnly));
    }

    public static IEnumerable<object[]> Description_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { "description" };
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_IntPtrEmfTypeString_Success(string description)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), EmfType.EmfOnly, description);
        AssertMetafileIsBlank(metafile);
    }

    [Theory]
    [MemberData(nameof(InvalidPath_TestData))]
    public void Ctor_ZeroPointerEmfTypeInvalidString_ThrowsArgumentException(string description)
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(IntPtr.Zero, EmfType.EmfOnly, description));
    }

    [Fact]
    public void Ctor_IntPtrRectangleF_Success()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangleF);
        AssertMetafileIsBlank(metafile);
    }

    public static IEnumerable<object[]> MetafileFrameUnit_TestData()
    {
        yield return new object[] { MetafileFrameUnit.Pixel };
        yield return new object[] { MetafileFrameUnit.Point };
        yield return new object[] { MetafileFrameUnit.Inch };
        yield return new object[] { MetafileFrameUnit.Document };
        yield return new object[] { MetafileFrameUnit.Millimeter };
        yield return new object[] { MetafileFrameUnit.GdiCompatible };
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_TestData))]
    public void Ctor_IntPtrRectangleFMetafileFrameUnit_Success(MetafileFrameUnit frameUnit)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangleF, frameUnit);
        AssertMetafileIsBlank(metafile);
    }

    [Theory]
    [MemberData(nameof(EmfType_TestData))]
    public void Ctor_IntPtrRectangleFMetafileFrameUnitEmfType_Success(EmfType emfType)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangleF, MetafileFrameUnit.Pixel, emfType);
        AssertMetafileIsBlank(metafile);
        AssertEmfType(metafile.GetMetafileHeader(), emfType);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_IntPtrRectangleFMetafileFrameUnitEmfTypeString_Success(string description)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangleF, MetafileFrameUnit.Pixel, EmfType.EmfOnly, description);
        AssertMetafileIsBlank(metafile);
    }

    [Fact]
    public void Ctor_IntPtrRectangle_Success()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangle);
        AssertMetafileIsBlank(metafile);
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_TestData))]
    public void Ctor_IntPtrRectangleMetafileFrameUnit_Success(MetafileFrameUnit frameUnit)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangle, frameUnit);
        AssertMetafileIsBlank(metafile);
    }

    [Theory]
    [MemberData(nameof(EmfType_TestData))]
    public void Ctor_IntPtrRectangleMetafileFrameUnitEmfType_Success(EmfType emfType)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangle, MetafileFrameUnit.Pixel, emfType);
        AssertMetafileIsBlank(metafile);
        AssertEmfType(metafile.GetMetafileHeader(), emfType);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_IntPtrRectangleMetafileFrameUnitEmfTypeString_Success(string description)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using Metafile metafile = new(g.GetHdc(), _rectangle, MetafileFrameUnit.Pixel, EmfType.EmfOnly, description);
        AssertMetafileIsBlank(metafile);
    }

    [Fact]
    public void Ctor_IntPtrZeroI_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(IntPtr.Zero, _rectangleF));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(IntPtr.Zero, _rectangleF, MetafileFrameUnit.Pixel));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(IntPtr.Zero, _rectangleF, MetafileFrameUnit.Pixel, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(IntPtr.Zero, _rectangleF, MetafileFrameUnit.Pixel, EmfType.EmfOnly, "description"));

        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(IntPtr.Zero, _rectangle));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(IntPtr.Zero, _rectangle, MetafileFrameUnit.Pixel));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(IntPtr.Zero, _rectangle, MetafileFrameUnit.Pixel, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(IntPtr.Zero, _rectangle, MetafileFrameUnit.Pixel, EmfType.EmfOnly, "description"));
    }

    public static IEnumerable<object[]> MetafileFrameUnit_Invalid_TestData()
    {
        yield return new object[] { (MetafileFrameUnit.Pixel - 1) };
        yield return new object[] { (MetafileFrameUnit.GdiCompatible + 1) };
        yield return new object[] { (MetafileFrameUnit)int.MaxValue };
        yield return new object[] { (MetafileFrameUnit)int.MinValue };
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_Invalid_TestData))]
    public void Ctor_InvalidMetafileFrameUnit_ThrowsArgumentException(MetafileFrameUnit farameUnit)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(referenceHdc, _rectangleF, farameUnit));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(referenceHdc, _rectangleF, farameUnit, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(referenceHdc, _rectangleF, farameUnit, EmfType.EmfOnly, "description"));

        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(referenceHdc, _rectangle, farameUnit));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(referenceHdc, _rectangle, farameUnit, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(referenceHdc, _rectangle, farameUnit, EmfType.EmfOnly, "description"));
    }

    [Theory]
    [MemberData(nameof(EmfType_Invalid_TestData))]
    public void Ctor_InvalidEmfType_ThrowsArgumentException(EmfType emfType)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, emfType));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, emfType, "description"));

        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(referenceHdc, _rectangle, MetafileFrameUnit.GdiCompatible, emfType));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(referenceHdc, _rectangle, MetafileFrameUnit.GdiCompatible, emfType, "description"));
    }

    [Fact]
    public void Ctor_StringIntPtr_Success()
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc()))
        {
            AssertMetafileIsBlank(metafile);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(EmfType_TestData))]
    public void Ctor_StringIntPtrEmfType_Success(EmfType emfType)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), emfType))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), emfType);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_StringIntPtrEmfTypeDescription_Success(string description)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), EmfType.EmfPlusDual, description))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), EmfType.EmfPlusDual);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Fact]
    public void Ctor_IntPtrZeroII_ThrowsArgumentException()
    {
        string fileName = GetPath("newTestImage.wmf");
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, IntPtr.Zero));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, IntPtr.Zero, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, IntPtr.Zero, EmfType.EmfOnly, "description"));
        DeleteFile(fileName);
    }

    [Theory]
    [MemberData(nameof(EmfType_Invalid_TestData))]
    public void Ctor_InvalidEmfTypeI_ThrowsArgumentException(EmfType emfType)
    {
        string fileName = GetPath("newTestImage.wmf");
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, referenceHdc, emfType));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, referenceHdc, emfType, "description"));
        DeleteFile(fileName);
    }

    [Fact]
    public void Ctor_NullPath_ThrowsArgumentNullException()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentNullException>("path", () => new Metafile((string)null, referenceHdc));
        AssertExtensions.Throws<ArgumentNullException>("path", () => new Metafile((string)null, referenceHdc, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentNullException>("path", () => new Metafile((string)null, referenceHdc, EmfType.EmfOnly, "description"));
    }

    [Theory]
    [InlineData("bad\0path")]
    [InlineData("")]
    public void Ctor_InvalidPathI_ThrowsArgumentException(string fileName)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>("path", null, () => new Metafile(fileName, referenceHdc));
        AssertExtensions.Throws<ArgumentException>("path", null, () => new Metafile(fileName, referenceHdc, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>("path", null, () => new Metafile(fileName, referenceHdc, EmfType.EmfOnly, "description"));
    }

    [Fact]
    public void Ctor_PathTooLong_ThrowsPathTooLongException()
    {
        string fileName = GetPath(new string('a', short.MaxValue));
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        Assert.Throws<PathTooLongException>(() => new Metafile(fileName, referenceHdc));
        Assert.Throws<PathTooLongException>(() => new Metafile(fileName, referenceHdc, EmfType.EmfOnly));
        Assert.Throws<PathTooLongException>(() => new Metafile(fileName, referenceHdc, EmfType.EmfOnly, "description"));
        DeleteFile(fileName);
    }

    [Fact]
    public void Ctor_StringIntPtrRectangleF_Success()
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), _rectangleF))
        {
            AssertMetafileIsBlank(metafile);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_TestData))]
    public void Ctor_StringIntPtrRectangleFMetafileFrameUnit_Success(MetafileFrameUnit frameUnit)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), _rectangleF, frameUnit))
        {
            AssertMetafileIsBlank(metafile);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(EmfType_TestData))]
    public void Ctor_StringIntPtrRectangleFMetafileFrameUnitEmfType_Success(EmfType emfType)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), _rectangleF, MetafileFrameUnit.GdiCompatible, emfType))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), emfType);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_StringIntPtrRectangleFMetafileFrameUnitEmfTypeString_Success(string description)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(
            fileName, g.GetHdc(), _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, description))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), EmfType.EmfOnly);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_RectangleFEmpty_Success(string description)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(
            fileName, g.GetHdc(), default(RectangleF), MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, description))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), EmfType.EmfOnly);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Fact]
    public void Ctor_StringIntPtrRectangle_Success()
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), _rectangle))
        {
            AssertMetafileIsBlank(metafile);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_TestData))]
    public void Ctor_StringIntPtrRectangleMetafileFrameUnit_Success(MetafileFrameUnit frameUnit)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), _rectangle, frameUnit))
        {
            AssertMetafileIsBlank(metafile);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(EmfType_TestData))]
    public void Ctor_StringIntPtrRectangleMetafileFrameUnitEmfType_Success(EmfType emfType)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(fileName, g.GetHdc(), _rectangle, MetafileFrameUnit.GdiCompatible, emfType))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), emfType);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_StringIntPtrRectangleMetafileFrameUnitEmfTypeString_Success(string description)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(
            fileName, g.GetHdc(), _rectangle, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, description))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), EmfType.EmfOnly);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_RectangleEmpty_Success(string description)
    {
        string fileName = GetPath("newTestImage.wmf");
        using (Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb))
        using (Graphics g = Graphics.FromImage(bmp))
        using (Metafile metafile = new(
            fileName, g.GetHdc(), default, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, description))
        {
            AssertMetafileIsBlank(metafile);
            AssertEmfType(metafile.GetMetafileHeader(), EmfType.EmfOnly);
            Assert.True(File.Exists(fileName));
        }

        File.Delete(fileName);
    }

    [Fact]
    public void Ctor_IntPtrZeroIII_ThrowsArgumentException()
    {
        string fileName = GetPath("newTestImage.wmf");
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, IntPtr.Zero, _rectangleF));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, IntPtr.Zero, _rectangleF, MetafileFrameUnit.GdiCompatible));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, IntPtr.Zero, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, IntPtr.Zero, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, "description"));

        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, IntPtr.Zero, _rectangle));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, IntPtr.Zero, _rectangle, MetafileFrameUnit.GdiCompatible));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, IntPtr.Zero, _rectangle, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, IntPtr.Zero, _rectangle, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, "description"));
        DeleteFile(fileName);
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_Invalid_TestData))]
    public void Ctor_InvalidFrameUnit_ThrowsArgumentException(MetafileFrameUnit frameUnit)
    {
        string fileName = GetPath("newTestImage.wmf");
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, referenceHdc, _rectangleF, frameUnit));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, referenceHdc, _rectangleF, frameUnit, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, referenceHdc, _rectangleF, frameUnit, EmfType.EmfOnly, "description"));

        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(fileName, referenceHdc, _rectangle, frameUnit));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, referenceHdc, _rectangle, frameUnit, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, referenceHdc, _rectangle, frameUnit, EmfType.EmfOnly, "description"));
        DeleteFile(fileName);
    }

    [Theory]
    [MemberData(nameof(EmfType_Invalid_TestData))]
    public void Ctor_InvalidEmfTypeII_ThrowsArgumentException(EmfType emfType)
    {
        string fileName = GetPath("newTestImage.wmf");
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, emfType));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, emfType, "description"));

        AssertExtensions.Throws<ArgumentException>(null, () =>
           new Metafile(fileName, referenceHdc, _rectangle, MetafileFrameUnit.GdiCompatible, emfType));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(fileName, referenceHdc, _rectangle, MetafileFrameUnit.GdiCompatible, emfType, "description"));
        DeleteFile(fileName);
    }

    [Fact]
    public void Ctor_NullPathI_ThrowsArgumentNullException()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentNullException>("path", () => new Metafile((string)null, referenceHdc, _rectangleF));
        AssertExtensions.Throws<ArgumentNullException>("path", () =>
            new Metafile((string)null, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible));
        AssertExtensions.Throws<ArgumentNullException>("path", () =>
            new Metafile((string)null, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentNullException>("path", () =>
            new Metafile((string)null, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, "description"));
    }

    [Theory]
    [InlineData("bad\0path")]
    [InlineData("")]
    public void Ctor_InvalidPathII_ThrowsArgumentException(string fileName)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException, ArgumentException>("path", null, () => new Metafile(fileName, referenceHdc, _rectangleF));
        AssertExtensions.Throws<ArgumentException, ArgumentException>("path", null, () =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible));
        AssertExtensions.Throws<ArgumentException, ArgumentException>("path", null, () =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException, ArgumentException>("path", null, () =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, "description"));
    }

    [Fact]
    public void Ctor_PathTooLongI_ThrowsPathTooLongException()
    {
        string fileName = GetPath(new string('a', 261));
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        Assert.Throws<PathTooLongException>(() => new Metafile(fileName, referenceHdc, _rectangleF));
        Assert.Throws<PathTooLongException>(() =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible));
        Assert.Throws<PathTooLongException>(() =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly));
        Assert.Throws<PathTooLongException>(() =>
            new Metafile(fileName, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, "description"));
        DeleteFile(fileName);
    }

    [Fact]
    public void Ctor_StreamIntPtrRectangle_Success()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using MemoryStream stream = new();
        using Metafile metafile = new(stream, g.GetHdc(), _rectangle);
        AssertMetafileIsBlank(metafile);
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_TestData))]
    public void Ctor_StreamIntPtrRectangleMetafileFrameUnit_Success(MetafileFrameUnit frameUnit)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using MemoryStream stream = new();
        using Metafile metafile = new(stream, g.GetHdc(), _rectangle, frameUnit);
        AssertMetafileIsBlank(metafile);
    }

    [Theory]
    [MemberData(nameof(EmfType_TestData))]
    public void Ctor_StreamIntPtrRectangleMetafileFrameUnitEmfType_Success(EmfType emfType)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using MemoryStream stream = new();
        using Metafile metafile = new(stream, g.GetHdc(), _rectangle, MetafileFrameUnit.GdiCompatible, emfType);
        AssertMetafileIsBlank(metafile);
        AssertEmfType(metafile.GetMetafileHeader(), emfType);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_StreamIntPtrRectangleMetafileFrameUnitEmfTypeString_Success(string description)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using MemoryStream stream = new();
        using Metafile metafile = new(
            stream, g.GetHdc(), _rectangle, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, description);
        AssertMetafileIsBlank(metafile);
        AssertEmfType(metafile.GetMetafileHeader(), EmfType.EmfOnly);
    }

    [Theory]
    [MemberData(nameof(Description_TestData))]
    public void Ctor_RectangleEmptyI_Success(string description)
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        using MemoryStream stream = new();
        using Metafile metafile = new(
            stream, g.GetHdc(), default, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, description);
        AssertMetafileIsBlank(metafile);
        AssertEmfType(metafile.GetMetafileHeader(), EmfType.EmfOnly);
    }

    [Fact]
    public void Ctor_IntPtrZeroIV_ThrowsArgumentException()
    {
        using MemoryStream stream = new();
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(stream, IntPtr.Zero, _rectangle));
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(stream, IntPtr.Zero, _rectangle, MetafileFrameUnit.GdiCompatible));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(stream, IntPtr.Zero, _rectangle, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(stream, IntPtr.Zero, _rectangle, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, "description"));
    }

    [Theory]
    [MemberData(nameof(MetafileFrameUnit_Invalid_TestData))]
    public void Ctor_InvalidFrameUnitIII_ThrowsArgumentException(MetafileFrameUnit frameUnit)
    {
        using MemoryStream stream = new();
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>(null, () => new Metafile(stream, referenceHdc, _rectangle, frameUnit));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(stream, referenceHdc, _rectangle, frameUnit, EmfType.EmfOnly));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(stream, referenceHdc, _rectangle, frameUnit, EmfType.EmfOnly, "description"));
    }

    [Theory]
    [MemberData(nameof(EmfType_Invalid_TestData))]
    public void Ctor_InvalidEmfTypeIII_ThrowsArgumentException(EmfType emfType)
    {
        using MemoryStream stream = new();
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        AssertExtensions.Throws<ArgumentException>(null, () =>
           new Metafile(stream, referenceHdc, _rectangle, MetafileFrameUnit.GdiCompatible, emfType));
        AssertExtensions.Throws<ArgumentException>(null, () =>
            new Metafile(stream, referenceHdc, _rectangle, MetafileFrameUnit.GdiCompatible, emfType, "description"));
    }

    [Fact]
    public void Ctor_NullStream_ThrowsArgumentNullException()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(bmp);
        IntPtr referenceHdc = g.GetHdc();
        Assert.Throws<ArgumentNullException>(() => new Metafile((Stream)null, referenceHdc, _rectangleF));
        Assert.Throws<ArgumentNullException>(() => new Metafile((Stream)null, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible));
        Assert.Throws<ArgumentNullException>(() =>
            new Metafile((Stream)null, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly));
        Assert.Throws<ArgumentNullException>(() =>
            new Metafile((Stream)null, referenceHdc, _rectangleF, MetafileFrameUnit.GdiCompatible, EmfType.EmfOnly, "description"));
    }

    [Fact]
    public void Static_GetMetafileHeader_String_ReturnsExpected()
    {
        MetafileHeader header = Metafile.GetMetafileHeader(GetPath(WmfFile));
        AssertMetafileHeader(header);
    }

    [Fact]
    public void Static_GetMetafileHeader_IntPtr_ThrowsArgumentException()
    {
        AssertExtensions.Throws<ArgumentException>(null, () => Metafile.GetMetafileHeader(IntPtr.Zero));
        using Metafile metafile = new(GetPath(WmfFile));
        AssertExtensions.Throws<ArgumentException>(null, () => Metafile.GetMetafileHeader(metafile.GetHenhmetafile()));
    }

    [Theory]
    [InlineData("bad\0path")]
    [InlineData("")]
    public void Static_GetMetafileHeader_InvalidPath_ThrowsArgumentException(string fileName)
    {
        AssertExtensions.Throws<ArgumentException>("path", null, () => Metafile.GetMetafileHeader(fileName));
    }

    [Fact]
    public void Static_GetMetafileHeader_NullString_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("path", () => Metafile.GetMetafileHeader((string)null));
    }

    [Fact]
    public void Static_GetMetafileHeader_Stream_ReturnsExpected()
    {
        using FileStream stream = File.OpenRead(GetPath(WmfFile));
        MetafileHeader header = Metafile.GetMetafileHeader(stream);
        AssertMetafileHeader(header);
    }

    [Fact]
    public void Static_GetMetafileHeader_NullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Metafile.GetMetafileHeader((Stream)null));
    }

    [Fact]
    public void Static_GetMetafileHeader_EmptyStream_ArgumentException()
    {
        using MemoryStream stream = new();
        AssertExtensions.Throws<ArgumentException>(null, () => Metafile.GetMetafileHeader(stream));
    }

    [Fact]
    public void GetMetafileHeader_ReturnsExpected()
    {
        using Metafile metafile = new(GetPath(WmfFile));
        MetafileHeader headerA = metafile.GetMetafileHeader();
        MetafileHeader headerB = metafile.GetMetafileHeader();
        AssertMetafileHeader(headerA);
        Assert.NotSame(headerA, headerB);
    }

    [Fact]
    public void GetMetafileHeader_Disposed_ThrowsArgumentException()
    {
        Metafile metafile = new(GetPath(WmfFile));
        metafile.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, metafile.GetMetafileHeader);
    }

    [Fact]
    public void GetHenhmetafile_ReturnsExpected()
    {
        using Metafile metafile = new(GetPath(WmfFile));
        Assert.NotEqual(IntPtr.Zero, metafile.GetHenhmetafile());
    }

    [Fact]
    public void GetHenhmetafile_Disposed_ThrowsArgumentException()
    {
        Metafile metafile = new(GetPath(WmfFile));
        metafile.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => metafile.GetHenhmetafile());
    }

    [Fact]
    public void PlayRecord_Disposed_ThrowsArgumentException()
    {
        Metafile metafile = new(GetPath(WmfFile));
        metafile.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () =>
            metafile.PlayRecord(EmfPlusRecordType.BeginContainer, 0, 1, new byte[1]));
    }

    private static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static string GetPath(string fileName)
    {
        return Helpers.GetTestBitmapPath(fileName);
    }

    private static void AssertEmfType(MetafileHeader metafileHeader, EmfType emfType)
    {
        switch (emfType)
        {
            case EmfType.EmfOnly:
                Assert.True(metafileHeader.IsEmf());
                break;
            case EmfType.EmfPlusDual:
                Assert.True(metafileHeader.IsEmfPlusDual());
                break;
            case EmfType.EmfPlusOnly:
                Assert.True(metafileHeader.IsEmfPlusOnly());
                break;
        }
    }

    private void AssertMetafileIsBlank(Metafile metafile)
    {
        GraphicsUnit graphicsUnit = (GraphicsUnit)int.MaxValue;

        AssertMetafileHeaderIsBlank(metafile.GetMetafileHeader());

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // This values are incorrect on libgdiplus.
            Assert.Equal(new Rectangle(0, 0, 1, 1), metafile.GetBounds(ref graphicsUnit));
            Assert.Equal(GraphicsUnit.Pixel, graphicsUnit);
        }
    }

    private static void AssertMetafileHeaderIsBlank(MetafileHeader metafileHeader)
    {
        Assert.Equal(new Rectangle(0, 0, 0, 0), metafileHeader.Bounds);
        Assert.Equal(0, metafileHeader.MetafileSize);
    }

    private void AssertMetafile(Metafile metafile)
    {
        GraphicsUnit graphicsUnit = (GraphicsUnit)int.MaxValue;

        AssertMetafileHeader(metafile.GetMetafileHeader());
        Assert.Equal(new Rectangle(-30, -40, 3096, 4127), metafile.GetBounds(ref graphicsUnit));
        Assert.Equal(GraphicsUnit.Pixel, graphicsUnit);
    }

    private static void AssertMetafileHeader(MetafileHeader header)
    {
        Assert.Equal(MetafileType.WmfPlaceable, header.Type);
        Assert.Equal(0x300, header.Version);
        Assert.Equal(new Rectangle(-30, -40, 3096, 4127), header.Bounds);
        Assert.Equal(606, header.DpiX);
        Assert.Equal(606, header.DpiY);
        Assert.Equal(0, header.EmfPlusHeaderSize);
        Assert.Equal(0, header.LogicalDpiX);
        Assert.Equal(0, header.LogicalDpiY);
        Assert.Equal(3474, header.MetafileSize);
        Assert.NotNull(header.WmfHeader);
        Assert.False(header.IsDisplay());
        Assert.False(header.IsEmf());
        Assert.False(header.IsEmfOrEmfPlus());
        Assert.False(header.IsEmfPlus());
        Assert.False(header.IsEmfPlusDual());
        Assert.False(header.IsEmfPlusOnly());
        Assert.True(header.IsWmf());
        Assert.True(header.IsWmfPlaceable());

        Assert.Equal(9, header.WmfHeader.HeaderSize);
        Assert.Equal(98, header.WmfHeader.MaxRecord);
        Assert.Equal(3, header.WmfHeader.NoObjects);
        Assert.Equal(0, header.WmfHeader.NoParameters);
        Assert.Equal(1737, header.WmfHeader.Size);
        Assert.Equal((int)MetafileType.Wmf, header.WmfHeader.Type);
        Assert.Equal(0x300, header.WmfHeader.Version);
    }
}
