// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace System.Drawing.Tests;

public partial class GraphicsTests
{
    public static bool IsWindows7OrWindowsArm64 => PlatformDetection.IsWindows7 || (PlatformDetection.IsWindows && PlatformDetection.IsArm64Process);

    [Fact]
    public void GetHdc_FromHdc_Roundtrips()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        IntPtr hdc = graphics.GetHdc();
        try
        {
            Assert.NotEqual(IntPtr.Zero, hdc);

            using Graphics graphicsCopy = Graphics.FromHdc(hdc);
            VerifyGraphics(graphicsCopy, graphicsCopy.VisibleClipBounds);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void GetHdc_SameImage_ReturnsSame()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics1 = Graphics.FromImage(bitmap);
        using Graphics graphics2 = Graphics.FromImage(bitmap);
        try
        {
            Assert.Equal(graphics1.GetHdc(), graphics2.GetHdc());
        }
        finally
        {
            graphics1.ReleaseHdc();
            graphics2.ReleaseHdc();
        }
    }

    [Fact]
    public void GetHdc_NotReleased_ThrowsInvalidOperationException()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.GetHdc());
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void GetHdc_Disposed_ThrowsObjectDisposedException()
    {
        using Bitmap bitmap = new(10, 10);
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.GetHdc());
    }

    [Fact]
    public void FromHdc_ValidHdc_ReturnsExpected()
    {
        using var hdc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdc(hdc);
        Rectangle expected = Helpers.GetWindowDCRect(hdc);
        VerifyGraphics(graphics, expected);
    }

    [Fact]
    public void FromHdc_ValidHdcWithContext_ReturnsExpected()
    {
        using var hdc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdc(hdc, IntPtr.Zero);
        Rectangle expected = Helpers.GetWindowDCRect(hdc);
        VerifyGraphics(graphics, expected);
    }

    [Fact]
    public void FromHdcInternal_GetDC_ReturnsExpected()
    {
        using var hdc = GetDcScope.ScreenDC;
        using Graphics graphics = Graphics.FromHdcInternal(hdc);
        Rectangle expected = Helpers.GetWindowDCRect(hdc);
        VerifyGraphics(graphics, expected);
    }

    [Fact]
    public void FromHdc_ZeroHdc_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("hdc", () => Graphics.FromHdc(IntPtr.Zero));
    }

    [Fact]
    public void FromHdcInternal_ZeroHdc_ThrowsOutOfMemoryException()
    {
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromHdcInternal(IntPtr.Zero));
    }

    [Fact]
    public void FromHdc_ZeroHdc_ThrowsOutOfMemoryException()
    {
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromHdc(IntPtr.Zero, 10));
    }

    [Fact]
    public void FromHdc_InvalidHdc_ThrowsOutOfMemoryException()
    {
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromHwnd(10));
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromHwndInternal(10));
    }

    [Fact]
    public void ReleaseHdc_ValidHdc_ResetsHdc()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        IntPtr hdc = graphics.GetHdc();
        graphics.ReleaseHdc();
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdc(hdc));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdcInternal(hdc));

        hdc = graphics.GetHdc();
        graphics.ReleaseHdc(hdc);
        AssertExtensions.Throws<ArgumentException>(null, graphics.ReleaseHdc);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdcInternal(hdc));

        hdc = graphics.GetHdc();
        graphics.ReleaseHdcInternal(hdc);
        AssertExtensions.Throws<ArgumentException>(null, graphics.ReleaseHdc);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdcInternal(hdc));
    }

    [Fact]
    public void ReleaseHdc_NoSuchHdc_ResetsHdc()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        IntPtr hdc = graphics.GetHdc();
        graphics.ReleaseHdc(10);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdcInternal(10));

        hdc = graphics.GetHdc();
        graphics.ReleaseHdcInternal(10);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdc(10));
    }

    [Fact]
    public void ReleaseHdc_OtherGraphicsHdc_Success()
    {
        using Bitmap bitmap1 = new(10, 10);
        using Bitmap bitmap2 = new(10, 10);
        using Graphics graphics1 = Graphics.FromImage(bitmap1);
        using Graphics graphics2 = Graphics.FromImage(bitmap2);
        IntPtr hdc1 = graphics1.GetHdc();
        IntPtr hdc2 = graphics2.GetHdc();
        Assert.NotEqual(hdc1, hdc2);

        graphics1.ReleaseHdc(hdc2);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics1.ReleaseHdc(hdc1));

        graphics2.ReleaseHdc(hdc1);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics2.ReleaseHdc(hdc2));
    }

    [Fact]
    public void ReleaseHdc_NoHdc_ThrowsArgumentException()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        AssertExtensions.Throws<ArgumentException>(null, graphics.ReleaseHdc);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdc(IntPtr.Zero));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdcInternal(IntPtr.Zero));
    }

    [Fact]
    public void ReleaseHdc_Disposed_ThrowsObjectDisposedException()
    {
        using Bitmap bitmap = new(10, 10);
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, graphics.ReleaseHdc);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdc(IntPtr.Zero));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ReleaseHdcInternal(IntPtr.Zero));
    }

    public static IEnumerable<object[]> Hwnd_TestData()
    {
        yield return new object[] { (nint)0 };
        yield return new object[] { (nint)PInvokeCore.GetForegroundWindow() };
    }

    [Theory]
    [MemberData(nameof(Hwnd_TestData))]
    public void FromHwnd_ValidHwnd_ReturnsExpected(nint hwnd)
    {
        using Graphics graphics = Graphics.FromHwnd(hwnd);
        Rectangle expected = Helpers.GetHWndRect((HWND)hwnd);
        VerifyGraphics(graphics, expected);
    }

    [Theory]
    [MemberData(nameof(Hwnd_TestData))]
    public void FromHwndInternal_ValidHwnd_ReturnsExpected(nint hwnd)
    {
        using Graphics graphics = Graphics.FromHwnd(hwnd);
        Rectangle expected = Helpers.GetHWndRect((HWND)hwnd);
        VerifyGraphics(graphics, expected);
    }

    [Fact]
    public void FromHwnd_InvalidHwnd_ThrowsOutOfMemoryException()
    {
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromHdc(10));
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromHdcInternal(10));
    }

    [Theory]
    [InlineData(PixelFormat.Format16bppRgb555)]
    [InlineData(PixelFormat.Format16bppRgb565)]
    [InlineData(PixelFormat.Format24bppRgb)]
    [InlineData(PixelFormat.Format32bppArgb)]
    [InlineData(PixelFormat.Format32bppPArgb)]
    [InlineData(PixelFormat.Format32bppRgb)]
    [InlineData(PixelFormat.Format48bppRgb)]
    [InlineData(PixelFormat.Format64bppArgb)]
    [InlineData(PixelFormat.Format64bppPArgb)]
    public void FromImage_Bitmap_Success(PixelFormat format)
    {
        using Bitmap image = new(10, 10, format);
        using Graphics graphics = Graphics.FromImage(image);
        VerifyGraphics(graphics, new Rectangle(Point.Empty, image.Size));
    }

    [Fact]
    public void FromImage_NullImage_ThrowsArgumentNullException()
    {
        AssertExtensions.Throws<ArgumentNullException>("image", () => Graphics.FromImage(null));
    }

    [Theory]
    [InlineData(PixelFormat.Format1bppIndexed)]
    [InlineData(PixelFormat.Format4bppIndexed)]
    [InlineData(PixelFormat.Format8bppIndexed)]
    public void FromImage_IndexedImage_ThrowsException(PixelFormat format)
    {
        using Bitmap image = new(10, 10, format);
        Exception exception = AssertExtensions.Throws<ArgumentException, Exception>(() => Graphics.FromImage(image));
        if (exception is ArgumentException argumentException)
            Assert.Equal("image", argumentException.ParamName);
    }

    [Fact]
    public void FromImage_DisposedImage_ThrowsArgumentException()
    {
        Bitmap image = new(10, 10);
        image.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => Graphics.FromImage(image));
    }

    [Fact]
    public void FromImage_Metafile_ThrowsOutOfMemoryException()
    {
        using Metafile image = new(Helpers.GetTestBitmapPath("telescope_01.wmf"));
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromImage(image));
    }

    [Theory]
    [InlineData(PixelFormat.Format16bppArgb1555)]
    [InlineData(PixelFormat.Format16bppGrayScale)]
    public void FromImage_Invalid16BitFormat_ThrowsOutOfMemoryException(PixelFormat format)
    {
        using Bitmap image = new(10, 10, format);
        Assert.Throws<OutOfMemoryException>(() => Graphics.FromImage(image));
    }

    public static IEnumerable<object[]> CompositingMode_TestData()
    {
        yield return new object[] { CompositingMode.SourceCopy, Color.FromArgb(160, 255, 255, 255) };
        yield return new object[] { CompositingMode.SourceOver, Color.FromArgb(220, 185, 185, 185) };
    }

    [Theory]
    [MemberData(nameof(CompositingMode_TestData))]
    public void CompositingMode_Set_GetReturnsExpected(CompositingMode mode, Color expectedOverlap)
    {
        Color transparentBlack = Color.FromArgb(160, 0, 0, 0);
        Color transparentWhite = Color.FromArgb(160, 255, 255, 255);

        using SolidBrush transparentBlackBrush = new(transparentBlack);
        using SolidBrush transparentWhiteBrush = new(transparentWhite);
        using Bitmap image = new(3, 3);
        using Graphics graphics = Graphics.FromImage(image);
        using Bitmap targetImage = new(3, 3);
        using Graphics targetGraphics = Graphics.FromImage(targetImage);
        graphics.CompositingMode = mode;
        Assert.Equal(mode, graphics.CompositingMode);

        graphics.FillRectangle(transparentBlackBrush, new Rectangle(0, 0, 2, 2));
        graphics.FillRectangle(transparentWhiteBrush, new Rectangle(1, 1, 2, 2));

        targetGraphics.DrawImage(image, Point.Empty);
        Helpers.VerifyBitmap(targetImage,
        [
                [transparentBlack,   transparentBlack, Helpers.EmptyColor],
                [transparentBlack,   expectedOverlap,  transparentWhite],
                [Helpers.EmptyColor, transparentWhite, transparentWhite]
        ]);
    }

    [Theory]
    [InlineData(CompositingMode.SourceOver - 1)]
    [InlineData(CompositingMode.SourceCopy + 1)]
    public void CompositingMode_SetInvalid_ThrowsInvalidEnumArgumentException(CompositingMode compositingMode)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<InvalidEnumArgumentException>("value", () => graphics.CompositingMode = compositingMode);
    }

    [Fact]
    public void CompositingMode_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.CompositingMode);
            Assert.Throws<InvalidOperationException>(() => graphics.CompositingMode = CompositingMode.SourceCopy);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void CompositingMode_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CompositingMode);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CompositingMode = CompositingMode.SourceCopy);
    }

    public static IEnumerable<object[]> CompositingQuality_TestData()
    {
        Color transparentBlack = Color.FromArgb(160, 0, 0, 0);
        Color transparentWhite = Color.FromArgb(160, 255, 255, 255);
        var basicExpectedColors = new Color[][]
        {
            [transparentBlack,   transparentBlack,                   Helpers.EmptyColor],
            [transparentBlack,   Color.FromArgb(220, 185, 185, 185), transparentWhite],
            [Helpers.EmptyColor, transparentWhite,                   transparentWhite]
        };

        yield return new object[] { CompositingQuality.AssumeLinear, basicExpectedColors };
        yield return new object[] { CompositingQuality.Default, basicExpectedColors };
        yield return new object[] { CompositingQuality.HighSpeed, basicExpectedColors };
        yield return new object[] { CompositingQuality.Invalid, basicExpectedColors };

        var gammaCorrectedColors = new Color[][]
        {
            [Color.FromArgb(159, 0, 0, 0), Color.FromArgb(159, 0, 0, 0),       Color.FromArgb(0, 0, 0, 0)],
            [Color.FromArgb(159, 0, 0, 0), Color.FromArgb(219, 222, 222, 222), Color.FromArgb(159, 255, 255, 255)],
            [Color.FromArgb(0, 0, 0, 0),   Color.FromArgb(159, 255, 255, 255), Color.FromArgb(159, 255, 255, 255)]
        };
        yield return new object[] { CompositingQuality.GammaCorrected, gammaCorrectedColors };
        yield return new object[] { CompositingQuality.HighQuality, gammaCorrectedColors };
    }

    [Theory]
    [MemberData(nameof(CompositingQuality_TestData))]
    public void CompositingQuality_Set_GetReturnsExpected(CompositingQuality quality, Color[][] expectedIntersectionColor)
    {
        Color transparentBlack = Color.FromArgb(160, 0, 0, 0);
        Color transparentWhite = Color.FromArgb(160, 255, 255, 255);

        using SolidBrush transparentBlackBrush = new(transparentBlack);
        using SolidBrush transparentWhiteBrush = new(transparentWhite);
        using Bitmap image = new(3, 3);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.CompositingQuality = quality;
        Assert.Equal(quality, graphics.CompositingQuality);

        graphics.FillRectangle(transparentBlackBrush, new Rectangle(0, 0, 2, 2));
        graphics.FillRectangle(transparentWhiteBrush, new Rectangle(1, 1, 2, 2));

        Helpers.VerifyBitmap(image, expectedIntersectionColor);
    }

    [Theory]
    [InlineData(CompositingQuality.Invalid - 1)]
    [InlineData(CompositingQuality.AssumeLinear + 1)]
    public void CompositingQuality_SetInvalid_ThrowsInvalidEnumArgumentException(CompositingQuality compositingQuality)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<InvalidEnumArgumentException>("value", () => graphics.CompositingQuality = compositingQuality);
    }

    [Fact]
    public void CompositingQuality_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.CompositingQuality);
            Assert.Throws<InvalidOperationException>(() => graphics.CompositingQuality = CompositingQuality.AssumeLinear);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void CompositingQuality_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CompositingQuality);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CompositingQuality = CompositingQuality.AssumeLinear);
    }

    [Fact]
    public void Dispose_MultipleTimesWithoutHdc_Success()
    {
        using Bitmap bitmap = new(10, 10);
        var graphics = Graphics.FromImage(bitmap);
        graphics.Dispose();
        graphics.Dispose();

        // The backing image is not disposed.
        Assert.Equal(10, bitmap.Height);
    }

    [Fact]
    public void Dispose_MultipleTimesWithHdc_Success()
    {
        using Bitmap bitmap = new(10, 10);
        var graphics = Graphics.FromImage(bitmap);
        graphics.GetHdc();

        graphics.Dispose();
        graphics.Dispose();

        // The backing image is not disposed.
        Assert.Equal(10, bitmap.Height);
    }

    [Fact]
    public void DpiX_GetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DpiX);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DpiX_GetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DpiX);
    }

    [Fact]
    public void DpiY_GetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DpiX);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DpiY_GetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DpiX);
    }

    [Theory]
    [InlineData(FlushIntention.Flush)]
    [InlineData(FlushIntention.Sync)]
    [InlineData(FlushIntention.Flush - 1)] // Not in the range of valid values of FlushIntention.
    [InlineData(FlushIntention.Sync + 1)] // Not in the range of valid values of FlushIntention.
    public void Flush_MultipleTimes_Success(FlushIntention intention)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        if (intention == FlushIntention.Flush)
        {
            graphics.Flush();
            graphics.Flush();
        }

        graphics.Flush(intention);
        graphics.Flush(intention);
    }

    [Fact]
    public void Flush_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(graphics.Flush);
            Assert.Throws<InvalidOperationException>(() => graphics.Flush(FlushIntention.Sync));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Flush_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, graphics.Flush);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.Flush(FlushIntention.Flush));
    }

    [Theory]
    [InlineData(InterpolationMode.Bicubic, InterpolationMode.Bicubic)]
    [InlineData(InterpolationMode.Bilinear, InterpolationMode.Bilinear)]
    [InlineData(InterpolationMode.Default, InterpolationMode.Bilinear)]
    [InlineData(InterpolationMode.High, InterpolationMode.HighQualityBicubic)]
    [InlineData(InterpolationMode.HighQualityBicubic, InterpolationMode.HighQualityBicubic)]
    [InlineData(InterpolationMode.HighQualityBilinear, InterpolationMode.HighQualityBilinear)]
    [InlineData(InterpolationMode.Low, InterpolationMode.Bilinear)]
    [InlineData(InterpolationMode.NearestNeighbor, InterpolationMode.NearestNeighbor)]
    public void InterpolationMode_SetValid_GetReturnsExpected(InterpolationMode interpolationMode, InterpolationMode expectedInterpolationMode)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.InterpolationMode = interpolationMode;
        Assert.Equal(expectedInterpolationMode, graphics.InterpolationMode);
    }

    [Theory]
    [InlineData(InterpolationMode.Invalid - 1)]
    [InlineData(InterpolationMode.HighQualityBicubic + 1)]
    public void InterpolationMode_SetInvalid_ThrowsInvalidEnumArgumentException(InterpolationMode interpolationMode)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<InvalidEnumArgumentException>("value", () => graphics.InterpolationMode = interpolationMode);
    }

    [Fact]
    public void InterpolationMode_SetToInvalid_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.InterpolationMode = InterpolationMode.Invalid);
    }

    [Fact]
    public void InterpolationMode_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.InterpolationMode);
            Assert.Throws<InvalidOperationException>(() => graphics.InterpolationMode = InterpolationMode.HighQualityBilinear);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void InterpolationMode_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.InterpolationMode);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.InterpolationMode = InterpolationMode.HighQualityBilinear);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(1000000032)]
    [InlineData(float.NaN)]
    public void PageScale_SetValid_GetReturnsExpected(float pageScale)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.PageScale = pageScale;
        Assert.Equal(pageScale, graphics.PageScale);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1000000033)]
    [InlineData(float.NegativeInfinity)]
    [InlineData(float.PositiveInfinity)]
    public void PageScale_SetInvalid_ThrowsArgumentException(float pageScale)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PageScale = pageScale);
    }

    [Fact]
    public void PageScale_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.PageScale);
            Assert.Throws<InvalidOperationException>(() => graphics.PageScale = 10);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void PageScale_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PageScale);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PageScale = 10);
    }

    [Theory]
    [InlineData(GraphicsUnit.Display)]
    [InlineData(GraphicsUnit.Document)]
    [InlineData(GraphicsUnit.Inch)]
    [InlineData(GraphicsUnit.Millimeter)]
    [InlineData(GraphicsUnit.Pixel)]
    [InlineData(GraphicsUnit.Point)]
    public void PageUnit_SetValid_GetReturnsExpected(GraphicsUnit pageUnit)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.PageUnit = pageUnit;
        Assert.Equal(pageUnit, graphics.PageUnit);
    }

    [Theory]
    [InlineData(GraphicsUnit.World - 1)]
    [InlineData(GraphicsUnit.Millimeter + 1)]
    public void PageUnit_SetInvalid_ThrowsInvalidEnumArgumentException(GraphicsUnit pageUnit)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<InvalidEnumArgumentException>("value", () => graphics.PageUnit = pageUnit);
    }

    [Fact]
    public void PageUnit_SetWorld_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PageUnit = GraphicsUnit.World);
    }

    [Fact]
    public void PageUnit_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.PageUnit);
            Assert.Throws<InvalidOperationException>(() => graphics.PageUnit = GraphicsUnit.Document);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void PageUnit_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PageUnit);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PageUnit = GraphicsUnit.Document);
    }

    [Theory]
    [InlineData(PixelOffsetMode.Default)]
    [InlineData(PixelOffsetMode.Half)]
    [InlineData(PixelOffsetMode.HighQuality)]
    [InlineData(PixelOffsetMode.HighSpeed)]
    [InlineData(PixelOffsetMode.None)]
    public void PixelOffsetMode_SetValid_GetReturnsExpected(PixelOffsetMode pixelOffsetMode)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.PixelOffsetMode = pixelOffsetMode;
        Assert.Equal(pixelOffsetMode, graphics.PixelOffsetMode);
    }

    [Theory]
    [InlineData(PixelOffsetMode.Invalid - 1)]
    [InlineData(PixelOffsetMode.Half + 1)]
    public void PixelOffsetMode_SetInvalid_ThrowsInvalidEnumArgumentException(PixelOffsetMode pixelOffsetMode)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<InvalidEnumArgumentException>("value", () => graphics.PixelOffsetMode = pixelOffsetMode);
    }

    [Fact]
    public void PixelOffsetMode_SetToInvalid_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PixelOffsetMode = PixelOffsetMode.Invalid);
    }

    [Fact]
    public void PixelOffsetMode_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.PixelOffsetMode);
            Assert.Throws<InvalidOperationException>(() => graphics.PixelOffsetMode = PixelOffsetMode.Default);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void PixelOffsetMode_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PixelOffsetMode);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.PixelOffsetMode = PixelOffsetMode.Default);
    }

    public static IEnumerable<object[]> RenderingOrigin_TestData()
    {
        Color empty = Color.FromArgb(255, 0, 0, 0);
        Color red = Color.FromArgb(Color.Red.ToArgb());

        yield return new object[]
        {
            new Point(0, 0),
            new Color[][]
            {
                [red, red,   red],
                [red, empty, empty],
                [red, empty, empty]
            }
        };

        yield return new object[]
        {
            new Point(1, 1),
            new Color[][]
            {
                [empty, red, empty],
                [red,   red, red],
                [empty, red, empty]
            }
        };

        var allEmpty = new Color[][]
        {
            [empty, empty, empty],
            [empty, empty, empty],
            [empty, empty, empty]
        };

        yield return new object[] { new Point(-3, -3), allEmpty };
        yield return new object[] { new Point(3, 3), allEmpty };
    }

    [Theory]
    [MemberData(nameof(RenderingOrigin_TestData))]
    public void RenderingOrigin_SetToCustom_RendersExpected(Point renderingOrigin, Color[][] expectedRendering)
    {
        Color red = Color.FromArgb(Color.Red.ToArgb());

        using Bitmap image = new(3, 3);
        using Graphics graphics = Graphics.FromImage(image);
        using HatchBrush brush = new(HatchStyle.Cross, red);
        graphics.RenderingOrigin = renderingOrigin;
        Assert.Equal(renderingOrigin, graphics.RenderingOrigin);

        graphics.FillRectangle(brush, new Rectangle(0, 0, 3, 3));
        Helpers.VerifyBitmap(image, expectedRendering);
    }

    [Fact]
    public void RenderingOrigin_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.RenderingOrigin);
            Assert.Throws<InvalidOperationException>(() => graphics.RenderingOrigin = Point.Empty);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void RenderingOrigin_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.RenderingOrigin);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.RenderingOrigin = Point.Empty);
    }

    [Theory]
    [InlineData(SmoothingMode.AntiAlias, SmoothingMode.AntiAlias)]
    [InlineData(SmoothingMode.Default, SmoothingMode.None)]
    [InlineData(SmoothingMode.HighQuality, SmoothingMode.AntiAlias)]
    [InlineData(SmoothingMode.HighSpeed, SmoothingMode.None)]
    [InlineData(SmoothingMode.None, SmoothingMode.None)]
    public void SmoothingMode_SetValid_GetReturnsExpected(SmoothingMode smoothingMode, SmoothingMode expectedSmoothingMode)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.SmoothingMode = smoothingMode;
        Assert.Equal(expectedSmoothingMode, graphics.SmoothingMode);
    }

    [Theory]
    [InlineData(SmoothingMode.Invalid - 1)]
    [InlineData(SmoothingMode.AntiAlias + 1)]
    public void SmoothingMode_SetInvalid_ThrowsInvalidEnumArgumentException(SmoothingMode smoothingMode)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<InvalidEnumArgumentException>("value", () => graphics.SmoothingMode = smoothingMode);
    }

    [Fact]
    public void SmoothingMode_SetToInvalid_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.SmoothingMode = SmoothingMode.Invalid);
    }

    [Fact]
    public void SmoothingMode_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.SmoothingMode);
            Assert.Throws<InvalidOperationException>(() => graphics.SmoothingMode = SmoothingMode.AntiAlias);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void SmoothingMode_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.SmoothingMode);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.SmoothingMode = SmoothingMode.AntiAlias);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(12)]
    public void TextContrast_SetValid_GetReturnsExpected(int textContrast)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.TextContrast = textContrast;
        Assert.Equal(textContrast, graphics.TextContrast);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(13)]
    public void TextContrast_SetInvalid_ThrowsArgumentException(int textContrast)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TextContrast = textContrast);
    }

    [Fact]
    public void TextContrast_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.TextContrast);
            Assert.Throws<InvalidOperationException>(() => graphics.TextContrast = 5);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void TextContrast_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TextContrast);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TextContrast = 5);
    }

    [Theory]
    [InlineData(TextRenderingHint.AntiAlias)]
    [InlineData(TextRenderingHint.AntiAliasGridFit)]
    [InlineData(TextRenderingHint.ClearTypeGridFit)]
    [InlineData(TextRenderingHint.SingleBitPerPixel)]
    [InlineData(TextRenderingHint.SingleBitPerPixelGridFit)]
    [InlineData(TextRenderingHint.SystemDefault)]
    public void TextRenderingHint_SetValid_GetReturnsExpected(TextRenderingHint textRenderingHint)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.TextRenderingHint = textRenderingHint;
        Assert.Equal(textRenderingHint, graphics.TextRenderingHint);
    }

    [Theory]
    [InlineData(TextRenderingHint.SystemDefault - 1)]
    [InlineData(TextRenderingHint.ClearTypeGridFit + 1)]
    public void TextRenderingHint_SetInvalid_ThrowsInvalidEnumArgumentException(TextRenderingHint textRenderingHint)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<InvalidEnumArgumentException>("value", () => graphics.TextRenderingHint = textRenderingHint);
    }

    [Fact]
    public void TextRenderingHint_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.TextRenderingHint);
            Assert.Throws<InvalidOperationException>(() => graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void TextRenderingHint_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TextRenderingHint);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit);
    }

    [Fact]
    public void Transform_SetValid_GetReturnsExpected()
    {
        Color empty = Helpers.EmptyColor;
        Color red = Color.FromArgb(Color.Red.ToArgb());

        using Bitmap image = new(5, 5);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(red);
        using Matrix matrix = new();
        matrix.Scale(1f / 3, 2);
        matrix.Translate(2, 1);
        matrix.Rotate(270);

        graphics.Transform = matrix;
        graphics.FillRectangle(brush, new Rectangle(0, 0, 3, 2));
        Helpers.VerifyBitmap(image,
        [
                [empty, red,   empty, empty, empty],
                [empty, red,   empty, empty, empty],
                [empty, empty, empty, empty, empty],
                [empty, empty, empty, empty, empty],
                [empty, empty, empty, empty, empty]
        ]);
    }

    [Fact]
    public void Transform_SetNull_ThrowsNullReferenceException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<NullReferenceException>(() => graphics.Transform = null);
    }

    [Fact]
    public void Transform_SetDisposedMatrix_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Matrix matrix = new();
        matrix.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.Transform = matrix);
    }

    [Fact]
    public void Transform_SetNonInvertibleMatrix_ThrowsArgumentException()
    {
        using Bitmap image = new(5, 5);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix matrix = new(123, 24, 82, 16, 47, 30);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.Transform = matrix);
    }

    [Fact]
    public void Transform_GetSetWhenBusy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix matrix = new();
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.Transform);
            Assert.Throws<InvalidOperationException>(() => graphics.Transform = matrix);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Transform_GetSetWhenDisposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Matrix matrix = new();
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.Transform);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.Transform = matrix);
    }

    [Fact]
    public void ResetTransform_Invoke_SetsTransformToIdentity()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Assert.False(graphics.Transform.IsIdentity);

        graphics.ResetTransform();
        Assert.True(graphics.Transform.IsIdentity);
    }

    [Fact]
    public void ResetTransform_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(graphics.ResetTransform);
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void ResetTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, graphics.ResetTransform);
    }

    [Fact]
    public void MultiplyTransform_NoOrder_Success()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        using Matrix matrix = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Multiply(matrix);

        graphics.MultiplyTransform(matrix);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend)]
    [InlineData(MatrixOrder.Append)]
    public void MultiplyTransform_Order_Success(MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        using Matrix matrix = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Multiply(matrix, order);

        graphics.MultiplyTransform(matrix, order);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Fact]
    public void MultiplyTransform_NullMatrix_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => graphics.MultiplyTransform(null));
        AssertExtensions.Throws<ArgumentNullException>("matrix", () => graphics.MultiplyTransform(null, MatrixOrder.Append));
    }

    [Fact]
    public void MultiplyTransform_DisposedMatrix_Nop()
    {
        LinearGradientBrush brush = new(new Rectangle(1, 2, 3, 4), Color.Plum, Color.Red, 45, true);
        Matrix transform = brush.Transform;

        Matrix matrix = new();
        matrix.Dispose();

        brush.MultiplyTransform(matrix);
        brush.MultiplyTransform(matrix, MatrixOrder.Append);

        Assert.Equal(transform, brush.Transform);
    }

    [Fact]
    public void MultiplyTransform_NonInvertibleMatrix_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix matrix = new(123, 24, 82, 16, 47, 30);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.MultiplyTransform(matrix));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.MultiplyTransform(matrix, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void MultiplyTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix matrix = new();
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.MultiplyTransform(matrix, order));
    }

    [Fact]
    public void MultiplyTransform_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix matrix = new();
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.MultiplyTransform(matrix));
            Assert.Throws<InvalidOperationException>(() => graphics.MultiplyTransform(matrix, MatrixOrder.Append));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void MultiplyTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Matrix matrix = new();
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.MultiplyTransform(matrix));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.MultiplyTransform(matrix, MatrixOrder.Prepend));
    }

    [Theory]
    [InlineData(-1, -2)]
    [InlineData(0, 0)]
    [InlineData(1, 2)]
    public void TranslateTransform_NoOrder_Success(float dx, float dy)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Translate(dx, dy);

        graphics.TranslateTransform(dx, dy);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Theory]
    [InlineData(1, 1, MatrixOrder.Prepend)]
    [InlineData(1, 1, MatrixOrder.Append)]
    [InlineData(0, 0, MatrixOrder.Prepend)]
    [InlineData(0, 0, MatrixOrder.Append)]
    [InlineData(-1, -1, MatrixOrder.Prepend)]
    [InlineData(-1, -1, MatrixOrder.Append)]
    public void TranslateTransform_Order_Success(float dx, float dy, MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Translate(dx, dy, order);

        graphics.TranslateTransform(dx, dy, order);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void TranslateTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TranslateTransform(0, 0, order));
    }

    [Fact]
    public void TranslateTransform_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.TranslateTransform(0, 0));
            Assert.Throws<InvalidOperationException>(() => graphics.TranslateTransform(0, 0, MatrixOrder.Append));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void TranslateTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TranslateTransform(0, 0));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TranslateTransform(0, 0, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(-1, -2)]
    [InlineData(1, 2)]
    public void ScaleTransform_NoOrder_Success(float sx, float sy)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Scale(sx, sy);

        graphics.ScaleTransform(sx, sy);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Theory]
    [InlineData(1, 1, MatrixOrder.Prepend)]
    [InlineData(1, 1, MatrixOrder.Append)]
    [InlineData(-1, -1, MatrixOrder.Prepend)]
    [InlineData(-1, -1, MatrixOrder.Append)]
    public void ScaleTransform_Order_Success(float sx, float sy, MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Scale(sx, sy, order);

        graphics.ScaleTransform(sx, sy, order);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Fact]
    public void ScaleTransform_ZeroZero_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ScaleTransform(0, 0));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ScaleTransform(0, 0, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void ScaleTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ScaleTransform(0, 0, order));
    }

    [Fact]
    public void ScaleTransform_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.ScaleTransform(0, 0));
            Assert.Throws<InvalidOperationException>(() => graphics.ScaleTransform(0, 0, MatrixOrder.Append));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void ScaleTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ScaleTransform(0, 0));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.ScaleTransform(0, 0, MatrixOrder.Append));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(360)]
    public void RotateTransform_NoOrder_Success(float angle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Rotate(angle);

        graphics.RotateTransform(angle);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Theory]
    [InlineData(1, MatrixOrder.Prepend)]
    [InlineData(1, MatrixOrder.Append)]
    [InlineData(0, MatrixOrder.Prepend)]
    [InlineData(360, MatrixOrder.Append)]
    [InlineData(-1, MatrixOrder.Prepend)]
    [InlineData(-1, MatrixOrder.Append)]
    public void RotateTransform_Order_Success(float angle, MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;
        Matrix expectedTransform = graphics.Transform;
        expectedTransform.Rotate(angle, order);

        graphics.RotateTransform(angle, order);
        Assert.Equal(expectedTransform, graphics.Transform);
    }

    [Theory]
    [InlineData(MatrixOrder.Prepend - 1)]
    [InlineData(MatrixOrder.Append + 1)]
    public void RotateTransform_InvalidOrder_ThrowsArgumentException(MatrixOrder order)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.RotateTransform(0, order));
    }

    [Fact]
    public void RotateTransform_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.RotateTransform(0));
            Assert.Throws<InvalidOperationException>(() => graphics.RotateTransform(0, MatrixOrder.Append));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void RotateTransform_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.RotateTransform(0));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.RotateTransform(0, MatrixOrder.Append));
    }

    public static IEnumerable<object[]> CopyFromScreen_TestData()
    {
        yield return new object[] { 0, 0, 0, 0, new Size(0, 0) };
        yield return new object[] { int.MinValue, int.MinValue, 0, 0, new Size(1, 1) };
        yield return new object[] { int.MaxValue, int.MaxValue, 0, 0, new Size(1, 1) };
        yield return new object[] { int.MaxValue, int.MaxValue, 0, 0, new Size(1, 1) };
        yield return new object[] { 0, 0, -1, -1, new Size(1, 1) };
        yield return new object[] { 0, 0, int.MaxValue, int.MaxValue, new Size(1, 1) };
        yield return new object[] { 0, 0, 0, 0, new Size(-1, -1) };
    }

    [Theory]
    [MemberData(nameof(CopyFromScreen_TestData))]
    public void CopyFromScreen_OutOfRange_DoesNotAffectGraphics(int sourceX, int sourceY, int destinationX, int destinationY, Size size)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Color plum = Color.FromArgb(Color.Plum.ToArgb());
        image.SetPixel(0, 0, plum);

        graphics.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, size);
        graphics.CopyFromScreen(new Point(sourceX, sourceY), new Point(destinationX, destinationY), size);
        Assert.Equal(plum, image.GetPixel(0, 0));
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 10, 10)]
    [InlineData(0, 0, 0, 0, int.MaxValue, int.MaxValue)]
    [InlineData(1, 1, 2, 2, 3, 3)]
    public void CopyFromScreen_ValidRange_AffectsGraphics(int sourceX, int sourceY, int destinationX, int destinationY, int width, int height)
    {
        Color color = Color.FromArgb(2, 3, 4);
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(color);

        graphics.FillRectangle(brush, new Rectangle(0, 0, 10, 10));
        graphics.CopyFromScreen(sourceX, sourceY, destinationX, destinationY, new Size(width, height));

        Rectangle drawnRect = new(destinationX, destinationY, width, height);
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Color imageColor = image.GetPixel(x, y);
                if (!drawnRect.Contains(x, y))
                {
                    Assert.Equal(color, imageColor);
                }
                else
                {
                    Assert.NotEqual(color, imageColor);
                }
            }
        }
    }

    public static IEnumerable<object[]> CopyPixelOperation_TestData()
    {
        yield return new object[] { CopyPixelOperation.NoMirrorBitmap };
        yield return new object[] { CopyPixelOperation.Blackness };
        yield return new object[] { CopyPixelOperation.NotSourceErase };
        yield return new object[] { CopyPixelOperation.NotSourceCopy };
        yield return new object[] { CopyPixelOperation.SourceErase };
        yield return new object[] { CopyPixelOperation.DestinationInvert };
        yield return new object[] { CopyPixelOperation.PatInvert };
        yield return new object[] { CopyPixelOperation.SourceInvert };
        yield return new object[] { CopyPixelOperation.SourceAnd };
        yield return new object[] { CopyPixelOperation.MergePaint };
        yield return new object[] { CopyPixelOperation.MergeCopy };
        yield return new object[] { CopyPixelOperation.SourceCopy };
        yield return new object[] { CopyPixelOperation.SourcePaint };
        yield return new object[] { CopyPixelOperation.SourceCopy };
        yield return new object[] { CopyPixelOperation.PatCopy };
        yield return new object[] { CopyPixelOperation.PatPaint };
        yield return new object[] { CopyPixelOperation.Whiteness };
        yield return new object[] { CopyPixelOperation.CaptureBlt };
        yield return new object[] { CopyPixelOperation.CaptureBlt };
    }

    [Theory]
    [MemberData(nameof(CopyPixelOperation_TestData))]
    public void CopyFromScreen_IntsAndValidCopyPixelOperation_Success(CopyPixelOperation copyPixelOperation)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        // We don't know what the screen looks like at this point in time, so
        // just make sure that this doesn't fail.
        graphics.CopyFromScreen(0, 0, 0, 0, new Size(1, 1), copyPixelOperation);
    }

    [Theory]
    [MemberData(nameof(CopyPixelOperation_TestData))]
    public void CopyFromScreen_PointsAndValidCopyPixelOperation_Success(CopyPixelOperation copyPixelOperation)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        // We don't know what the screen looks like at this point in time, so
        // just make sure that this doesn't fail.
        graphics.CopyFromScreen(Point.Empty, Point.Empty, new Size(1, 1), copyPixelOperation);
    }

    [Theory]
    [InlineData(CopyPixelOperation.NoMirrorBitmap + 1)]
    [InlineData(CopyPixelOperation.Blackness - 1)]
    [InlineData(CopyPixelOperation.NotSourceErase - 1)]
    [InlineData(CopyPixelOperation.NotSourceCopy - 1)]
    [InlineData(CopyPixelOperation.SourceErase - 1)]
    [InlineData(CopyPixelOperation.DestinationInvert - 1)]
    [InlineData(CopyPixelOperation.PatInvert - 1)]
    [InlineData(CopyPixelOperation.SourceInvert - 1)]
    [InlineData(CopyPixelOperation.SourceAnd - 1)]
    [InlineData(CopyPixelOperation.MergePaint - 1)]
    [InlineData(CopyPixelOperation.MergeCopy - 1)]
    [InlineData(CopyPixelOperation.SourceCopy - 1)]
    [InlineData(CopyPixelOperation.SourcePaint - 1)]
    [InlineData(CopyPixelOperation.PatCopy - 1)]
    [InlineData(CopyPixelOperation.PatPaint - 1)]
    [InlineData(CopyPixelOperation.Whiteness - 1)]
    [InlineData(CopyPixelOperation.CaptureBlt - 1)]
    [InlineData(CopyPixelOperation.CaptureBlt + 1)]
    public void CopyFromScreen_InvalidCopyPixelOperation_ThrowsInvalidEnumArgumentException(CopyPixelOperation copyPixelOperation)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.ThrowsAny<ArgumentException>(() => graphics.CopyFromScreen(1, 2, 3, 4, Size.Empty, copyPixelOperation));
    }

    [Fact]
    public void CopyFromScreen_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);

        nint hdc = graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.CopyFromScreen(0, 0, 0, 0, Size.Empty));
            Assert.Throws<InvalidOperationException>(() => graphics.CopyFromScreen(0, 0, 0, 0, Size.Empty, CopyPixelOperation.DestinationInvert));
            Assert.Throws<InvalidOperationException>(() => graphics.CopyFromScreen(Point.Empty, Point.Empty, Size.Empty));
            Assert.Throws<InvalidOperationException>(() => graphics.CopyFromScreen(Point.Empty, Point.Empty, Size.Empty, CopyPixelOperation.DestinationInvert));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void CopyFromScreen_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CopyFromScreen(0, 0, 0, 0, Size.Empty));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CopyFromScreen(0, 0, 0, 0, Size.Empty, CopyPixelOperation.MergeCopy));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CopyFromScreen(Point.Empty, Point.Empty, Size.Empty));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.CopyFromScreen(Point.Empty, Point.Empty, Size.Empty, CopyPixelOperation.MergeCopy));
    }

    public static IEnumerable<object[]> TransformPoints_TestData()
    {
        yield return new object[]
        {
            CoordinateSpace.Device,
            CoordinateSpace.Page,
            new Point[] { new(1, 1), new(2, 2) },
            new Point[] { new(1, 1), new(2, 2) }
        };

        yield return new object[]
        {
           CoordinateSpace.Device,
            CoordinateSpace.World,
            new Point[] { new(1, 1), new(2, 2) },
            new Point[] { new(9, 12), new(13, 18) }
        };

        yield return new object[]
        {
            CoordinateSpace.Page,
            CoordinateSpace.Device,
            new Point[] { new(1, 1), new(2, 2) },
            new Point[] { new(1, 1), new(2, 2) }
        };

        yield return new object[]
        {
            CoordinateSpace.Page,
            CoordinateSpace.World,
            new Point[] { new(1, 1), new(2, 2) },
            new Point[] { new(9, 12), new(13, 18) }
        };

        yield return new object[]
        {
            CoordinateSpace.World,
            CoordinateSpace.Device,
            new Point[] { new(1, 1), new(2, 2) },
            new Point[] { new(1, -1), new(0, -1) }
        };

        yield return new object[]
        {
            CoordinateSpace.World,
            CoordinateSpace.Page,
            new Point[] { new(1, 1), new(2, 2) },
            new Point[] { new(1, -1), new(0, -1) }
        };
    }

    [Theory]
    [MemberData(nameof(TransformPoints_TestData))]
    public void TransformPoints_Points_Success(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] points, Point[] expected)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.PageScale = 10;
        graphics.Transform = transform;

        graphics.TransformPoints(destSpace, srcSpace, points);
        Assert.Equal(expected, points);
    }

    public static IEnumerable<object[]> TransformPointFs_TestData()
    {
        yield return new object[]
        {
            CoordinateSpace.Device,
            CoordinateSpace.Page,
            new PointF[] { new Point(1, 1), new Point(2, 2) },
            new PointF[] { new Point(1, 1), new Point(2, 2) }
        };

        yield return new object[]
        {
           CoordinateSpace.Device,
            CoordinateSpace.World,
            new PointF[] { new Point(1, 1), new Point(2, 2) },
            new PointF[] { new Point(9, 12), new Point(13, 18) }
        };

        yield return new object[]
        {
            CoordinateSpace.Page,
            CoordinateSpace.Device,
            new PointF[] { new Point(1, 1), new Point(2, 2) },
            new PointF[] { new Point(1, 1), new Point(2, 2) }
        };

        yield return new object[]
        {
            CoordinateSpace.Page,
            CoordinateSpace.World,
            new PointF[] { new Point(1, 1), new Point(2, 2) },
            new PointF[] { new Point(9, 12), new Point(13, 18) }
        };

        yield return new object[]
        {
            CoordinateSpace.World,
            CoordinateSpace.Device,
            new PointF[] { new Point(1, 1), new Point(2, 2) },
            new PointF[] { new(0.5f, -1.5f), new Point(0, -1) }
        };

        yield return new object[]
        {
            CoordinateSpace.World,
            CoordinateSpace.Page,
            new PointF[] { new Point(1, 1), new Point(2, 2) },
            new PointF[] { new(0.5f, -1.5f), new Point(0, -1) }
        };
    }

    [Theory]
    [MemberData(nameof(TransformPointFs_TestData))]
    public void TransformPoints_PointFs_Success(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] points, PointF[] expected)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.PageScale = 10;
        graphics.Transform = transform;

        graphics.TransformPoints(destSpace, srcSpace, points);
        Assert.Equal(expected, points);
    }

    [Theory]
    [InlineData(CoordinateSpace.Device)]
    [InlineData(CoordinateSpace.World)]
    [InlineData(CoordinateSpace.Page)]
    public void TransformPoints_PointsAndSameCoordinateSpace_DoesNothing(CoordinateSpace space)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;

        var points = new Point[] { new(1, 1) };
        graphics.TransformPoints(space, space, points);
        Assert.Equal([new(1, 1)], points);
    }

    [Theory]
    [InlineData(CoordinateSpace.Device)]
    [InlineData(CoordinateSpace.World)]
    [InlineData(CoordinateSpace.Page)]
    public void TransformPoints_PointFsAndSameCoordinateSpace_DoesNothing(CoordinateSpace space)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Matrix transform = new(1, 2, 3, 4, 5, 6);
        graphics.Transform = transform;

        var points = new PointF[] { new(1, 1) };
        graphics.TransformPoints(space, space, points);
        Assert.Equal([new(1, 1)], points);
    }

    [Theory]
    [InlineData(CoordinateSpace.World - 1)]
    [InlineData(CoordinateSpace.Device + 1)]
    public void TransformPoints_InvalidDestSpace_ThrowsArgumentException(CoordinateSpace destSpace)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(destSpace, CoordinateSpace.World, [new(1, 1)]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(destSpace, CoordinateSpace.World, new PointF[] { new(1, 1) }));
    }

    [Theory]
    [InlineData(CoordinateSpace.World - 1)]
    [InlineData(CoordinateSpace.Device + 1)]
    public void TransformPoints_InvalidSourceSpace_ThrowsArgumentException(CoordinateSpace srcSpace)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(CoordinateSpace.World, srcSpace, [new(1, 1)]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(CoordinateSpace.World, srcSpace, new PointF[] { new(1, 1) }));
    }

    [Fact]
    public void TransformPoints_NullPoints_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pts", () => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, (Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("pts", () => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, (PointF[])null));
    }

    [Fact]
    public void TransformPoints_EmptyPoints_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, Array.Empty<Point>()));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, Array.Empty<PointF>()));
    }

    [Fact]
    public void TransformPoints_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, [Point.Empty]));
            Assert.Throws<InvalidOperationException>(() => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, [PointF.Empty]));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void TransformPoints_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, [Point.Empty]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.TransformPoints(CoordinateSpace.Page, CoordinateSpace.Page, [PointF.Empty]));
    }

    public static IEnumerable<object[]> GetNearestColor_TestData()
    {
        yield return new object[] { PixelFormat.Format32bppArgb, Color.Red, Color.FromArgb(Color.Red.ToArgb()) };
        yield return new object[] { PixelFormat.Format16bppRgb555, Color.Red, Color.FromArgb(255, 248, 0, 0) };
    }

    [Theory]
    [MemberData(nameof(GetNearestColor_TestData))]
    public void GetNearestColor_Color_ReturnsExpected(PixelFormat pixelFormat, Color color, Color expected)
    {
        using Bitmap image = new(10, 10, pixelFormat);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Equal(expected, graphics.GetNearestColor(color));
    }

    [Fact]
    public void GetNearestColor_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.GetNearestColor(Color.Red));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void GetNearestColor_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.GetNearestColor(Color.Red));
    }

    [Fact]
    public void DrawArc_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawArc(null, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawArc(null, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawArc(null, new RectangleF(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawArc(null, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void DrawArc_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new RectangleF(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void DrawArc_ZeroWidth_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new Rectangle(0, 0, 0, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0, 0, 0, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new RectangleF(0, 0, 0, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0f, 0f, 0f, 1f, 0, 90));
    }

    [Fact]
    public void DrawArc_ZeroHeight_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new Rectangle(0, 0, 1, 0), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0, 0, 1, 0, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new RectangleF(0, 0, 1, 0), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0f, 0f, 1f, 0f, 0, 90));
    }

    [Fact]
    public void DrawArc_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawArc(pen, new Rectangle(0, 0, 1, 1), 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawArc(pen, 0, 0, 1, 1, 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawArc(pen, new RectangleF(0, 0, 1, 1), 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawArc(pen, 0f, 0f, 1f, 1f, 0, 90));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawArc_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new RectangleF(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void DrawRectangle_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawRectangle(null, new Rectangle(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawRectangle(null, 0, 0, 1, 1));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawRectangle(null, 0f, 0f, 1f, 1f));
    }

    [Fact]
    public void DrawRectangle_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, new Rectangle(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, 0, 0, 1, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, 0f, 0f, 1f, 1f));
    }

    [Fact]
    public void DrawRectangle_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawRectangle(pen, new Rectangle(0, 0, 1, 1)));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawRectangle(pen, 0, 0, 1, 1));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawRectangle(pen, 0f, 0f, 1f, 1f));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawRectangle_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, new Rectangle(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, 0, 0, 1, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangle(pen, 0f, 0f, 1f, 1f));
    }

    [Fact]
    public void DrawRectangles_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawRectangles(null, new Rectangle[2]));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawRectangles(null, new RectangleF[2]));
    }

    [Fact]
    public void DrawRectangles_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangles(pen, new Rectangle[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangles(pen, new RectangleF[2]));
    }

    [Fact]
    public void DrawRectangles_NullRectangles_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentNullException>("rects", () => graphics.DrawRectangles(pen, (Rectangle[])null));
        AssertExtensions.Throws<ArgumentNullException>("rects", () => graphics.DrawRectangles(pen, (RectangleF[])null));
    }

    [Fact]
    public void DrawRectangles_EmptyRectangles_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangles(pen, Array.Empty<Rectangle>()));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangles(pen, Array.Empty<RectangleF>()));
    }

    [Fact]
    public void DrawRectangles_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawRectangles(pen, new Rectangle[2]));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawRectangles(pen, new RectangleF[2]));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawRectangles_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangles(pen, new Rectangle[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawRectangles(pen, new RectangleF[2]));
    }

    [Fact]
    public void DrawEllipse_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawEllipse(null, new Rectangle(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawEllipse(null, 0, 0, 1, 1));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawEllipse(null, new RectangleF(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawEllipse(null, 0f, 0f, 1f, 1f));
    }

    [Fact]
    public void DrawEllipse_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, new Rectangle(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, 0, 0, 1, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, new RectangleF(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, 0f, 0f, 1f, 1f));
    }

    [Fact]
    public void DrawEllipse_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawEllipse(pen, new Rectangle(0, 0, 1, 1)));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawEllipse(pen, 0, 0, 1, 1));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawEllipse(pen, new RectangleF(0, 0, 1, 1)));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawEllipse(pen, 0f, 0f, 1f, 1f));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawEllipse_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, new Rectangle(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, 0, 0, 1, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, new RectangleF(0, 0, 1, 1)));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawEllipse(pen, 0f, 0f, 1f, 1f));
    }

    [Fact]
    public void DrawPie_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawPie(null, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawPie(null, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawPie(null, new RectangleF(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawPie(null, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void DrawPie_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, new RectangleF(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void DrawPie_ZeroWidth_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, new Rectangle(0, 0, 0, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, 0, 0, 0, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, new RectangleF(0, 0, 0, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, 0f, 0f, 0f, 1f, 0, 90));
    }

    [Fact]
    public void DrawPie_ZeroHeight_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new Rectangle(0, 0, 1, 0), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0, 0, 1, 0, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, new RectangleF(0, 0, 1, 0), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawArc(pen, 0f, 0f, 1f, 0f, 0, 90));
    }

    [Fact]
    public void DrawPie_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawPie(pen, new Rectangle(0, 0, 1, 1), 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawPie(pen, 0, 0, 1, 1, 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawPie(pen, new RectangleF(0, 0, 1, 1), 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawPie(pen, 0f, 0f, 1f, 1f, 0, 90));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawPie_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, new RectangleF(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPie(pen, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void DrawPolygon_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawPolygon(null, new Point[2]));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawPolygon(null, new PointF[2]));
    }

    [Fact]
    public void DrawPolygon_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPolygon(pen, new Point[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPolygon(pen, new PointF[2]));
    }

    [Fact]
    public void DrawPolygon_NullPoints_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawPolygon(pen, (Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawPolygon(pen, (PointF[])null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void DrawPolygon_InvalidPointsLength_ThrowsArgumentException(int length)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPolygon(pen, new Point[length]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPolygon(pen, new PointF[length]));
    }

    [Fact]
    public void DrawPolygon_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawPolygon(pen, new Point[2]));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawPolygon(pen, new PointF[2]));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawPolygon_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPolygon(pen, new Point[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPolygon(pen, new PointF[2]));
    }

    [Fact]
    public void DrawPath_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using GraphicsPath graphicsPath = new();
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawPath(null, graphicsPath));
    }

    [Fact]
    public void DrawPath_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using GraphicsPath graphicsPath = new();
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPath(pen, graphicsPath));
    }

    [Fact]
    public void DrawPath_NullPath_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentNullException>("path", () => graphics.DrawPath(pen, null));
    }

    [Fact]
    public void DrawPath_DisposedPath_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        GraphicsPath graphicsPath = new();
        graphicsPath.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPath(pen, graphicsPath));
    }

    [Fact]
    public void DrawPath_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        using GraphicsPath graphicsPath = new();
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawPath(pen, graphicsPath));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawPath_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        using GraphicsPath graphicsPath = new();
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawPath(pen, graphicsPath));
    }

    [Fact]
    public void DrawCurve_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawCurve(null, new Point[2]));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawCurve(null, new PointF[2]));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawCurve(null, new Point[2], 1));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawCurve(null, new PointF[2], 1));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawCurve(null, new PointF[2], 0, 2));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawCurve(null, new Point[2], 0, 2, 1));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawCurve(null, new PointF[2], 0, 2, 1));
    }

    [Fact]
    public void DrawCurve_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[2], 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2], 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2], 0, 2));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[2], 0, 2, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2], 0, 2, 1));
    }

    [Fact]
    public void DrawCurve_NullPoints_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawCurve(pen, (Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawCurve(pen, (PointF[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawCurve(pen, (Point[])null, 1));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawCurve(pen, (PointF[])null, 1));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawCurve(pen, null, 0, 2));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawCurve(pen, (Point[])null, 0, 2, 1));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawCurve(pen, (PointF[])null, 0, 2, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void DrawCurve_InvalidPointsLength_ThrowsArgumentException(int length)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[length]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[length]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[length], 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[length], 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[length], 0, length));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[length], 0, length, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[length], 0, length, 1));
    }

    [Theory]
    [InlineData(4, -1, 4)]
    [InlineData(4, 0, -1)]
    [InlineData(4, 4, 0)]
    [InlineData(4, 0, 5)]
    [InlineData(4, 3, 2)]
    public void DrawCurve_InvalidOffsetCount_ThrowsArgumentException(int length, int offset, int numberOfSegments)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[length], offset, numberOfSegments));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[length], offset, numberOfSegments, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[length], offset, numberOfSegments, 1));
    }

    [Fact]
    public void DrawCurve_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawCurve(pen, new Point[2]));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawCurve(pen, new PointF[2]));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawCurve(pen, new Point[2], 1));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawCurve(pen, new PointF[2], 1));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawCurve(pen, new PointF[2], 0, 2));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawCurve(pen, new Point[2], 0, 2, 1));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawCurve(pen, new PointF[2], 0, 2, 1));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawCurve_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[2], 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2], 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2], 0, 2));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new Point[2], 0, 2, 1));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawCurve(pen, new PointF[2], 0, 2, 1));
    }

    [Fact]
    public void DrawClosedCurve_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawClosedCurve(null, new Point[3]));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawClosedCurve(null, new Point[3], 1, FillMode.Winding));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawClosedCurve(null, new PointF[3]));
        AssertExtensions.Throws<ArgumentNullException>("pen", () => graphics.DrawClosedCurve(null, new PointF[3], 1, FillMode.Winding));
    }

    [Fact]
    public void DrawClosedCurve_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Pen pen = new(Color.Red);
        pen.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new Point[3]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new Point[3], 1, FillMode.Winding));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new PointF[3]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new PointF[3], 1, FillMode.Winding));
    }

    [Fact]
    public void DrawClosedCurve_NullPoints_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawClosedCurve(pen, (Point[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawClosedCurve(pen, (Point[])null, 1, FillMode.Winding));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawClosedCurve(pen, (PointF[])null));
        AssertExtensions.Throws<ArgumentNullException>("points", () => graphics.DrawClosedCurve(pen, (PointF[])null, 1, FillMode.Winding));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void DrawClosedCurve_InvalidPointsLength_ThrowsArgumentException(int length)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new Point[length]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new Point[length], 1, FillMode.Winding));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new PointF[length]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new PointF[length], 1, FillMode.Winding));
    }

    [Fact]
    public void DrawClosedCurve_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.DrawClosedCurve(pen, new Point[3]));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawClosedCurve(pen, new Point[3], 1, FillMode.Winding));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawClosedCurve(pen, new PointF[3]));
            Assert.Throws<InvalidOperationException>(() => graphics.DrawClosedCurve(pen, new PointF[3], 1, FillMode.Winding));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void DrawClosedCurve_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new Point[3]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new Point[3], 1, FillMode.Alternate));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new PointF[3]));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.DrawClosedCurve(pen, new PointF[3], 1, FillMode.Alternate));
    }

    [Fact]
    public void FillPie_NullPen_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        AssertExtensions.Throws<ArgumentNullException>("brush", () => graphics.FillPie(null, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("brush", () => graphics.FillPie(null, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentNullException>("brush", () => graphics.FillPie(null, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void FillPie_DisposedPen_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        SolidBrush brush = new(Color.Red);
        brush.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void FillPie_ZeroWidth_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new Rectangle(0, 0, 0, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0, 0, 0, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0f, 0f, 0f, 1f, 0, 90));
    }

    [Fact]
    public void FillPie_ZeroHeight_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new Rectangle(0, 0, 1, 0), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0, 0, 1, 0, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0f, 0f, 1f, 0f, 0, 90));
    }

    [Fact]
    public void FillPie_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.FillPie(brush, new Rectangle(0, 0, 1, 1), 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.FillPie(brush, 0, 0, 1, 1, 0, 90));
            Assert.Throws<InvalidOperationException>(() => graphics.FillPie(brush, 0f, 0f, 1f, 1f, 0, 90));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void FillPie_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using SolidBrush brush = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, new Rectangle(0, 0, 1, 1), 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0, 0, 1, 1, 0, 90));
        AssertExtensions.Throws<ArgumentException>(null, () => graphics.FillPie(brush, 0f, 0f, 1f, 1f, 0, 90));
    }

    [Fact]
    public void Clear_Color_Success()
    {
        Color color = Color.FromArgb(Color.Plum.ToArgb());

        using Bitmap image = new(2, 2);
        using Graphics graphics = Graphics.FromImage(image);
        using SolidBrush brush = new(color);
        graphics.FillRectangle(brush, new Rectangle(0, 0, 2, 2));

        graphics.Clear(color);
        Helpers.VerifyBitmap(image,
        [
                [color, color],
                [color, color]
        ]);
    }

    [Fact]
    public void Clear_Busy_ThrowsInvalidOperationException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        using Pen pen = new(Color.Red);
        graphics.GetHdc();
        try
        {
            Assert.Throws<InvalidOperationException>(() => graphics.Clear(Color.Red));
        }
        finally
        {
            graphics.ReleaseHdc();
        }
    }

    [Fact]
    public void Clear_Disposed_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Pen pen = new(Color.Red);
        Graphics graphics = Graphics.FromImage(image);
        graphics.Dispose();

        AssertExtensions.Throws<ArgumentException>(null, () => graphics.Clear(Color.Red));
    }

    [Fact]
    public void DrawString_DefaultFont_Succeeds()
    {
        using Bitmap image = new(50, 50);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.DrawString("Test text", SystemFonts.DefaultFont, Brushes.White, default(Point));
        Helpers.VerifyBitmapNotBlank(image);
    }

    [Fact]
    public void DrawString_CompositingModeSourceCopy_ThrowsArgumentException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        AssertExtensions.Throws<ArgumentException>(
            null,
            () => graphics.DrawString("Test text", SystemFonts.DefaultFont, Brushes.White, default(Point)));
    }

    private static void VerifyGraphics(Graphics graphics, RectangleF expectedVisibleClipBounds)
    {
        Assert.NotNull(graphics.Clip);
        Assert.Equal(new RectangleF(-4194304, -4194304, 8388608, 8388608), graphics.ClipBounds);
        Assert.Equal(CompositingMode.SourceOver, graphics.CompositingMode);
        Assert.Equal(CompositingQuality.Default, graphics.CompositingQuality);
        Assert.Equal(96, graphics.DpiX);
        Assert.Equal(96, graphics.DpiY);
        Assert.Equal(InterpolationMode.Bilinear, graphics.InterpolationMode);
        Assert.False(graphics.IsClipEmpty);
        Assert.False(graphics.IsVisibleClipEmpty);
        Assert.Equal(1, graphics.PageScale);
        Assert.Equal(GraphicsUnit.Display, graphics.PageUnit);
        Assert.Equal(PixelOffsetMode.Default, graphics.PixelOffsetMode);
        Assert.Equal(Point.Empty, graphics.RenderingOrigin);
        Assert.Equal(SmoothingMode.None, graphics.SmoothingMode);
        Assert.Equal(4, graphics.TextContrast);
        Assert.Equal(TextRenderingHint.SystemDefault, graphics.TextRenderingHint);
        Assert.Equal(new Matrix(), graphics.Transform);
        Assert.Equal(expectedVisibleClipBounds, graphics.VisibleClipBounds);
    }

    [Fact]
    public void IsVisible_AllOverloads_ReturnSameResult()
    {
        using Bitmap bitmap = new(100, 100);
        using Graphics graphics = Graphics.FromImage(bitmap);

        // Test points
        Point point = new(10, 10);
        PointF pointF = new(10.5f, 10.5f);
        int x = 10, y = 10;
        float fx = 10.5f, fy = 10.5f;

        // Test rectangles
        Rectangle rect = new(10, 10, 50, 50);
        RectangleF rectF = new(10.5f, 10.5f, 50.5f, 50.5f);
        int width = 50, height = 50;
        float fwidth = 50.5f, fheight = 50.5f;

        // Verify that all overloads return the same result for points
        bool result1 = graphics.IsVisible(x, y);
        bool result2 = graphics.IsVisible(point);
        bool result3 = graphics.IsVisible(fx, fy);
        bool result4 = graphics.IsVisible(pointF);

        result1.Should().Be(result2);
        result1.Should().Be(result3);
        result1.Should().Be(result4);

        // Verify that all overloads return the same result for rectangles
        bool result5 = graphics.IsVisible(x, y, width, height);
        bool result6 = graphics.IsVisible(rect);
        bool result7 = graphics.IsVisible(fx, fy, fwidth, fheight);
        bool result8 = graphics.IsVisible(rectF);

        result5.Should().Be(result6);
        result5.Should().Be(result7);
        result5.Should().Be(result8);
    }

#if NET8_0_OR_GREATER
    [Fact]
    public void DrawCachedBitmap_ThrowsArgumentNullException()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);

        Assert.Throws<ArgumentNullException>(() => graphics.DrawCachedBitmap(null!, 0, 0));
    }

    [Fact]
    public void DrawCachedBitmap_DisposedBitmap_ThrowsArgumentException()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        CachedBitmap cachedBitmap = new(bitmap, graphics);
        cachedBitmap.Dispose();

        Assert.Throws<ArgumentException>(() => graphics.DrawCachedBitmap(cachedBitmap, 0, 0));
    }

    [Fact]
    public void DrawCachedBitmap_Simple()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using CachedBitmap cachedBitmap = new(bitmap, graphics);

        graphics.DrawCachedBitmap(cachedBitmap, 0, 0);
    }

    [Fact]
    public void DrawCachedBitmap_Translate()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.TranslateTransform(1.0f, 8.0f);
        using CachedBitmap cachedBitmap = new(bitmap, graphics);

        graphics.DrawCachedBitmap(cachedBitmap, 0, 0);
    }

    [Fact]
    public void DrawCachedBitmap_Rotation_ThrowsInvalidOperation()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.RotateTransform(36.0f);
        using CachedBitmap cachedBitmap = new(bitmap, graphics);

        Assert.Throws<InvalidOperationException>(() => graphics.DrawCachedBitmap(cachedBitmap, 0, 0));
    }

    [Theory]
    [InlineData(PixelFormat.Format16bppRgb555, PixelFormat.Format32bppRgb, false)]
    [InlineData(PixelFormat.Format32bppRgb, PixelFormat.Format16bppRgb555, true)]
    [InlineData(PixelFormat.Format32bppArgb, PixelFormat.Format16bppRgb555, false)]
    public void DrawCachedBitmap_ColorDepthChange_ThrowsInvalidOperation(
        PixelFormat sourceFormat,
        PixelFormat destinationFormat,
        bool shouldSucceed)
    {
        using Bitmap bitmap = new(10, 10, sourceFormat);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using CachedBitmap cachedBitmap = new(bitmap, graphics);

        using Bitmap bitmap2 = new(10, 10, destinationFormat);
        using Graphics graphics2 = Graphics.FromImage(bitmap2);

        if (shouldSucceed)
        {
            graphics2.DrawCachedBitmap(cachedBitmap, 0, 0);
        }
        else
        {
            Assert.Throws<InvalidOperationException>(() => graphics2.DrawCachedBitmap(cachedBitmap, 0, 0));
        }
    }
#endif

    internal static unsafe void VerifyBitmapEmpty(Bitmap bitmap)
    {
        BitmapData data = bitmap.LockBits(default, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            ReadOnlySpan<byte> bytes = new((byte*)data.Scan0, data.Stride * data.Height);
            bytes.IndexOfAnyExcept((byte)0).Should().Be(-1);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }

    internal static unsafe void VerifyBitmapNotEmpty(Bitmap bitmap)
    {
        BitmapData data = bitmap.LockBits(default, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            ReadOnlySpan<byte> bytes = new((byte*)data.Scan0, data.Stride * data.Height);
            bytes.IndexOfAnyExcept((byte)0).Should().NotBe(-1);
        }
        finally
        {
            bitmap.UnlockBits(data);
        }
    }

#if NET9_0_OR_GREATER
    [Fact]
    public void Graphics_DrawRoundedRectangle_Integer()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.DrawRoundedRectangle(Pens.Red, new(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }

    [Fact]
    public void Graphics_DrawRoundedRectangle_Float()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.DrawRoundedRectangle(Pens.Red, new RectangleF(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }

    [Fact]
    public void Graphics_FillRoundedRectangle_Integer()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.FillRoundedRectangle(Brushes.Green, new(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }

    [Fact]
    public void Graphics_FillRoundedRectangle_Float()
    {
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.FillRoundedRectangle(Brushes.Green, new RectangleF(0, 0, 10, 10), new(2, 2));
        VerifyBitmapNotEmpty(bitmap);
    }
#endif
}
