// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;

namespace System.Windows.Forms.Tests;

public partial class ControlPaintTests
{
    public static IEnumerable<object[]> ControlCreateHBitmap16Bit_TestData()
    {
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format1bppIndexed), Color.Empty };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), Color.Empty };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), Color.Empty };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format1bppIndexed), Color.Red };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), Color.Red };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), Color.Red };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format1bppIndexed), Color.Transparent };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), Color.Transparent };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), Color.Transparent };
        yield return new object[] { new Bitmap(11, 11, PixelFormat.Format1bppIndexed), Color.Red };
        yield return new object[] { new Bitmap(11, 11, PixelFormat.Format32bppRgb), Color.Red };
        yield return new object[] { new Bitmap(11, 11, PixelFormat.Format32bppArgb), Color.Red };
        yield return new object[] { new Bitmap(16, 24, PixelFormat.Format1bppIndexed), Color.Red };
        yield return new object[] { new Bitmap(16, 24, PixelFormat.Format32bppRgb), Color.Red };
        yield return new object[] { new Bitmap(16, 24, PixelFormat.Format32bppArgb), Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(ControlCreateHBitmap16Bit_TestData))]
    public void ControlPaint_CreateHBitmap16Bit_Invoke_ReturnsExpected(Bitmap bitmap, Color background)
    {
        HBITMAP hBitmap = (HBITMAP)ControlPaint.CreateHBitmap16Bit(bitmap, background);
        try
        {
            Assert.False(hBitmap.IsNull);
            Assert.Equal(OBJ_TYPE.OBJ_BITMAP, (OBJ_TYPE)PInvokeCore.GetObjectType(hBitmap));

            using Bitmap result = Image.FromHbitmap((IntPtr)hBitmap);
            Assert.Equal(PixelFormat.Format16bppRgb555, result.PixelFormat);
            Assert.Empty(result.Palette.Entries);
            Assert.Equal(bitmap.Size, result.Size);
        }
        finally
        {
            PInvokeCore.DeleteObject(hBitmap);
        }
    }

    [WinFormsFact]
    public void ControlPaint_CreateHBitmap16Bit_InvokeSpecificPixels_Success()
    {
        using Bitmap bitmap = new(3, 1);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 50, 100, 150));
        bitmap.SetPixel(1, 0, Color.FromArgb(1, 50, 100, 150));
        bitmap.SetPixel(2, 0, Color.FromArgb(0, 50, 100, 150));

        HBITMAP hBitmap = (HBITMAP)ControlPaint.CreateHBitmap16Bit(bitmap, Color.Red);
        try
        {
            Assert.False(hBitmap.IsNull);
            Assert.Equal(OBJ_TYPE.OBJ_BITMAP, (OBJ_TYPE)PInvokeCore.GetObjectType(hBitmap));

            using Bitmap result = Image.FromHbitmap((IntPtr)hBitmap);
            Assert.Equal(PixelFormat.Format16bppRgb555, result.PixelFormat);
            Assert.Empty(result.Palette.Entries);
            Assert.Equal(bitmap.Size, result.Size);
            Assert.Equal(Color.FromArgb(255, 49, 99, 148), result.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(255, 255, 0, 0), result.GetPixel(1, 0));
            Assert.Equal(Color.FromArgb(255, 255, 0, 0), result.GetPixel(2, 0));
        }
        finally
        {
            PInvokeCore.DeleteObject(hBitmap);
        }
    }

    [WinFormsFact]
    public void ControlPaint_CreateHBitmap16Bit_NullBitmap_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ControlPaint.CreateHBitmap16Bit(null, Color.Red));
    }

    public static IEnumerable<object[]> CreateHBitmapColorMask_TestData()
    {
        foreach (IntPtr monochromeMask in new IntPtr[] { IntPtr.Zero, 1 })
        {
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format1bppIndexed), monochromeMask };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb), monochromeMask };
            yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb), monochromeMask };
            yield return new object[] { new Bitmap(16, 24, PixelFormat.Format1bppIndexed), monochromeMask };
            yield return new object[] { new Bitmap(16, 24, PixelFormat.Format32bppRgb), monochromeMask };
            yield return new object[] { new Bitmap(16, 24, PixelFormat.Format32bppArgb), monochromeMask };
            yield return new object[] { new Bitmap(11, 11, PixelFormat.Format1bppIndexed), monochromeMask };
            yield return new object[] { new Bitmap(11, 11, PixelFormat.Format32bppRgb), monochromeMask };
            yield return new object[] { new Bitmap(11, 11, PixelFormat.Format32bppArgb), monochromeMask };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateHBitmapColorMask_TestData))]
    public void ControlPaint_CreateHBitmapColorMask_Invoke_ReturnsExpected(Bitmap bitmap, IntPtr monochromeMask)
    {
        HBITMAP hBitmap = (HBITMAP)ControlPaint.CreateHBitmapColorMask(bitmap, monochromeMask);
        try
        {
            Assert.False(hBitmap.IsNull);
            Assert.Equal(OBJ_TYPE.OBJ_BITMAP, (OBJ_TYPE)PInvokeCore.GetObjectType(hBitmap));

            using Bitmap result = Image.FromHbitmap((IntPtr)hBitmap);
            Assert.Equal(PixelFormat.Format32bppRgb, result.PixelFormat);
            Assert.Empty(result.Palette.Entries);
            Assert.Equal(bitmap.Size, result.Size);
        }
        finally
        {
            PInvokeCore.DeleteObject(hBitmap);
        }
    }

    [WinFormsFact]
    public void ControlPaint_CreateHBitmapColorMask_InvokeSpecificPixelsWithMonochromeMask_Success()
    {
        using Bitmap mask = new(3, 1);
        mask.SetPixel(0, 0, Color.FromArgb(255, 255, 0, 0));
        mask.SetPixel(1, 0, Color.FromArgb(255, 0, 255, 0));
        mask.SetPixel(2, 0, Color.FromArgb(0, 0, 0, 255));
        HBITMAP monochromeMask = (HBITMAP)mask.GetHbitmap();
        try
        {
            using Bitmap bitmap = new(3, 1);
            bitmap.SetPixel(0, 0, Color.FromArgb(255, 50, 100, 150));
            bitmap.SetPixel(1, 0, Color.FromArgb(1, 50, 100, 150));
            bitmap.SetPixel(2, 0, Color.FromArgb(0, 50, 100, 150));

            HBITMAP hBitmap = (HBITMAP)ControlPaint.CreateHBitmapColorMask(bitmap, (IntPtr)monochromeMask);
            try
            {
                Assert.False(hBitmap.IsNull);
                Assert.Equal(OBJ_TYPE.OBJ_BITMAP, (OBJ_TYPE)PInvokeCore.GetObjectType(hBitmap));

                using Bitmap result = Image.FromHbitmap((IntPtr)hBitmap);
                Assert.Equal(PixelFormat.Format32bppRgb, result.PixelFormat);
                Assert.Empty(result.Palette.Entries);
                Assert.Equal(bitmap.Size, result.Size);
                Assert.Equal(Color.FromArgb(255, 0, 100, 150), result.GetPixel(0, 0));
                Assert.Equal(Color.FromArgb(255, 210, 0, 211), result.GetPixel(1, 0));
                Assert.Equal(Color.FromArgb(255, 0, 0, 0), result.GetPixel(2, 0));
            }
            finally
            {
                PInvokeCore.DeleteObject(hBitmap);
            }
        }
        finally
        {
            PInvokeCore.DeleteObject(monochromeMask);
        }
    }

    [WinFormsFact]
    public void ControlPaint_CreateHBitmapColorMask_InvokeSpecificPixelsWithoutMonochromeMask_Success()
    {
        using Bitmap bitmap = new(3, 1);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 50, 100, 150));
        bitmap.SetPixel(1, 0, Color.FromArgb(1, 50, 100, 150));
        bitmap.SetPixel(2, 0, Color.FromArgb(0, 50, 100, 150));

        HBITMAP hBitmap = (HBITMAP)ControlPaint.CreateHBitmapColorMask(bitmap, IntPtr.Zero);
        try
        {
            Assert.False(hBitmap.IsNull);
            Assert.Equal(OBJ_TYPE.OBJ_BITMAP, (OBJ_TYPE)PInvokeCore.GetObjectType(hBitmap));

            using Bitmap result = Image.FromHbitmap((IntPtr)hBitmap);
            Assert.Equal(PixelFormat.Format32bppRgb, result.PixelFormat);
            Assert.Empty(result.Palette.Entries);
            Assert.Equal(bitmap.Size, result.Size);
            Assert.Equal(Color.FromArgb(255, 50, 100, 150), result.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(255, 210, 210, 211), result.GetPixel(1, 0));
            Assert.Equal(Color.FromArgb(255, 211, 211, 211), result.GetPixel(2, 0));
        }
        finally
        {
            PInvokeCore.DeleteObject(hBitmap);
        }
    }

    [WinFormsFact]
    public void ControlPaint_CreateHBitmapColorMask_NullBitmap_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ControlPaint.CreateHBitmapColorMask(null, IntPtr.Zero));
    }

    public static IEnumerable<object[]> CreateHBitmapTransparencyMask_TestData()
    {
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format1bppIndexed) };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppRgb) };
        yield return new object[] { new Bitmap(10, 10, PixelFormat.Format32bppArgb) };
        yield return new object[] { new Bitmap(16, 24, PixelFormat.Format1bppIndexed) };
        yield return new object[] { new Bitmap(16, 24, PixelFormat.Format32bppRgb) };
        yield return new object[] { new Bitmap(16, 24, PixelFormat.Format32bppArgb) };
        yield return new object[] { new Bitmap(11, 11, PixelFormat.Format1bppIndexed) };
        yield return new object[] { new Bitmap(11, 11, PixelFormat.Format32bppRgb) };
        yield return new object[] { new Bitmap(11, 11, PixelFormat.Format32bppArgb) };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateHBitmapTransparencyMask_TestData))]
    public void ControlPaint_CreateHBitmapTransparencyMask_Invoke_ReturnsExpected(Bitmap bitmap)
    {
        HBITMAP hBitmap = (HBITMAP)ControlPaint.CreateHBitmapTransparencyMask(bitmap);
        try
        {
            Assert.False(hBitmap.IsNull);
            Assert.Equal(OBJ_TYPE.OBJ_BITMAP, (OBJ_TYPE)PInvokeCore.GetObjectType(hBitmap));

            using Bitmap result = Image.FromHbitmap((IntPtr)hBitmap);
            Assert.Equal(PixelFormat.Format1bppIndexed, result.PixelFormat);
            Assert.Equal(new Color[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 255, 255, 255) }, result.Palette.Entries);
            Assert.Equal(bitmap.Size, result.Size);
        }
        finally
        {
            PInvokeCore.DeleteObject(hBitmap);
        }
    }

    [WinFormsFact]
    public void ControlPaint_CreateHBitmapTransparencyMask_InvokeSpecificPixels_Success()
    {
        using Bitmap bitmap = new(3, 1);
        bitmap.SetPixel(0, 0, Color.FromArgb(255, 50, 100, 150));
        bitmap.SetPixel(1, 0, Color.FromArgb(1, 50, 100, 150));
        bitmap.SetPixel(2, 0, Color.FromArgb(0, 50, 100, 150));

        HBITMAP hBitmap = (HBITMAP)ControlPaint.CreateHBitmapTransparencyMask(bitmap);
        try
        {
            Assert.False(hBitmap.IsNull);
            Assert.Equal(OBJ_TYPE.OBJ_BITMAP, (OBJ_TYPE)PInvokeCore.GetObjectType(hBitmap));

            using Bitmap result = Image.FromHbitmap((IntPtr)hBitmap);
            Assert.Equal(PixelFormat.Format1bppIndexed, result.PixelFormat);
            Assert.Equal(new Color[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 255, 255, 255) }, result.Palette.Entries);
            Assert.Equal(bitmap.Size, result.Size);
            Assert.Equal(Color.FromArgb(255, 0, 0, 0), result.GetPixel(0, 0));
            Assert.Equal(Color.FromArgb(255, 0, 0, 0), result.GetPixel(1, 0));
            Assert.Equal(Color.FromArgb(255, 255, 255, 255), result.GetPixel(2, 0));
        }
        finally
        {
            PInvokeCore.DeleteObject(hBitmap);
        }
    }

    [WinFormsFact]
    public void ControlPaint_CreateHBitmapTransparencyMask_NullBitmap_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("bitmap", () => ControlPaint.CreateHBitmapTransparencyMask(null));
    }

    public static IEnumerable<object[]> CreateBitmapWithInvertedForeColor_TestData()
    {
        yield return new object[] { Color.White, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.Black, Color.FromArgb(255, 255, 255, 255) };
    }

    [WinFormsTheory]
    [MemberData(nameof(CreateBitmapWithInvertedForeColor_TestData))]
    public void ControlPaint_CreateBitmapWithInvertedForeColor(Color imageColor, Color expected)
    {
        using Bitmap bitmap = new(1, 1);
        bitmap.SetPixel(0, 0, imageColor);

        using Bitmap invertedBitmap = ControlPaint.CreateBitmapWithInvertedForeColor(bitmap, Color.LightGray);
        Color newColor = invertedBitmap.GetPixel(0, 0);

        Assert.Equal(expected, newColor);
    }

    public static IEnumerable<object[]> Dark_Color_TestData()
    {
        yield return new object[] { Color.FromArgb(255, 255, 0, 0), Color.FromArgb(255, 85, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), Color.FromArgb(255, 0, 85, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 0, 0, 85) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), Color.FromArgb(255, 85, 85, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), Color.FromArgb(255, 85, 0, 85) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), Color.FromArgb(255, 0, 85, 85) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 85, 85, 85) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), Color.FromArgb(255, 0, 28, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), Color.FromArgb(255, 0, 9, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), Color.FromArgb(255, 0, 2, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), Color.FromArgb(255, 41, 14, 67) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), Color.FromArgb(255, 151, 0, 151) };
        yield return new object[] { SystemColors.ControlDarkDark, Color.FromArgb(255, 35, 35, 35) };
        yield return new object[] { SystemColors.ControlDark, Color.FromArgb(255, 53, 53, 53) };
        yield return new object[] { SystemColors.Control, Color.FromArgb(255, 133, 133, 133) };
        yield return new object[] { SystemColors.ControlLight, Color.FromArgb(255, 75, 75, 75) };
        yield return new object[] { SystemColors.ControlLightLight, Color.FromArgb(255, 85, 85, 85) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Dark_Color_TestData))]
    public void ControlPaint_Dark_InvokeColor_ReturnsExpected(Color baseColor, Color expected)
    {
        Assert.Equal(expected, ControlPaint.Dark(baseColor));

        // Call again to test caching.
        Assert.Equal(expected, ControlPaint.Dark(baseColor));
    }

    public static IEnumerable<object[]> Dark_Color_Float_TestData()
    {
        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -1.5f, Color.FromArgb(255, 255, 170, 170) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -1.5f, Color.FromArgb(255, 170, 255, 170) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -1.5f, Color.FromArgb(255, 170, 170, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -1.5f, Color.FromArgb(255, 255, 255, 170) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -1.5f, Color.FromArgb(255, 255, 170, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -1.5f, Color.FromArgb(255, 170, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -1.5f, Color.FromArgb(255, 169, 169, 169) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -1.5f, Color.FromArgb(255, 0, 138, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -1.5f, Color.FromArgb(255, 0, 43, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -1.5f, Color.FromArgb(255, 0, 11, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -1.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -1.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -1.5f, Color.FromArgb(255, 201, 162, 236) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -1.5f, Color.FromArgb(255, 255, 243, 0) };
        yield return new object[] { SystemColors.ControlDarkDark, -1.5f, Color.FromArgb(255, 175, 175, 175) };
        yield return new object[] { SystemColors.ControlDark, -1.5f, Color.FromArgb(255, 9, 9, 9) };
        yield return new object[] { SystemColors.Control, -1.5f, Color.FromArgb(255, 242, 242, 242) };
        yield return new object[] { SystemColors.ControlLight, -1.5f, Color.FromArgb(255, 121, 121, 121) };
        yield return new object[] { SystemColors.ControlLightLight, -1.5f, Color.FromArgb(255, 169, 169, 169) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -1f, Color.FromArgb(255, 255, 85, 85) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -1f, Color.FromArgb(255, 85, 255, 85) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -1f, Color.FromArgb(255, 85, 85, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -1f, Color.FromArgb(255, 255, 255, 85) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -1f, Color.FromArgb(255, 255, 85, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -1f, Color.FromArgb(255, 85, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -1f, Color.FromArgb(255, 84, 84, 84) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -1f, Color.FromArgb(255, 0, 111, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -1f, Color.FromArgb(255, 0, 34, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -1f, Color.FromArgb(255, 0, 9, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -1f, Color.FromArgb(255, 163, 96, 223) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -1f, Color.FromArgb(255, 255, 93, 0) };
        yield return new object[] { SystemColors.ControlDarkDark, -1f, Color.FromArgb(255, 140, 140, 140) };
        yield return new object[] { SystemColors.ControlDark, -1f, Color.FromArgb(255, 212, 212, 212) };
        yield return new object[] { SystemColors.Control, -1f, Color.FromArgb(255, 215, 215, 215) };
        yield return new object[] { SystemColors.ControlLight, -1f, Color.FromArgb(255, 45, 45, 45) };
        yield return new object[] { SystemColors.ControlLightLight, -1f, Color.FromArgb(255, 84, 84, 84) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -0.5f, Color.FromArgb(255, 255, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -0.5f, Color.FromArgb(255, 0, 255, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -0.5f, Color.FromArgb(255, 0, 0, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -0.5f, Color.FromArgb(255, 255, 255, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -0.5f, Color.FromArgb(255, 255, 0, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -0.5f, Color.FromArgb(255, 0, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -0.5f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -0.5f, Color.FromArgb(255, 0, 83, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -0.5f, Color.FromArgb(255, 0, 26, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -0.5f, Color.FromArgb(255, 0, 6, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -0.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -0.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -0.5f, Color.FromArgb(255, 123, 39, 199) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -0.5f, Color.FromArgb(255, 255, 198, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, -0.5f, Color.FromArgb(255, 105, 105, 105) };
        yield return new object[] { SystemColors.ControlDark, -0.5f, Color.FromArgb(255, 159, 159, 159) };
        yield return new object[] { SystemColors.Control, -0.5f, Color.FromArgb(255, 187, 187, 187) };
        yield return new object[] { SystemColors.ControlLight, -0.5f, Color.FromArgb(255, 226, 226, 226) };
        yield return new object[] { SystemColors.ControlLightLight, -0.5f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -0.25f, Color.FromArgb(255, 213, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -0.25f, Color.FromArgb(255, 0, 213, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -0.25f, Color.FromArgb(255, 0, 0, 213) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -0.25f, Color.FromArgb(255, 213, 213, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -0.25f, Color.FromArgb(255, 213, 0, 213) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -0.25f, Color.FromArgb(255, 0, 213, 213) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -0.25f, Color.FromArgb(255, 212, 212, 212) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -0.25f, Color.FromArgb(255, 0, 68, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -0.25f, Color.FromArgb(255, 0, 21, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -0.25f, Color.FromArgb(255, 0, 4, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -0.25f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -0.25f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -0.25f, Color.FromArgb(255, 102, 33, 165) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -0.25f, Color.FromArgb(255, 255, 121, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, -0.25f, Color.FromArgb(255, 87, 87, 87) };
        yield return new object[] { SystemColors.ControlDark, -0.25f, Color.FromArgb(255, 132, 132, 132) };
        yield return new object[] { SystemColors.Control, -0.25f, Color.FromArgb(255, 173, 173, 173) };
        yield return new object[] { SystemColors.ControlLight, -0.25f, Color.FromArgb(255, 188, 188, 188) };
        yield return new object[] { SystemColors.ControlLightLight, -0.25f, Color.FromArgb(255, 212, 212, 212) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 0f, Color.FromArgb(255, 170, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 0f, Color.FromArgb(255, 0, 170, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 0f, Color.FromArgb(255, 0, 0, 170) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 0f, Color.FromArgb(255, 170, 170, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 0f, Color.FromArgb(255, 170, 0, 170) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 0f, Color.FromArgb(255, 0, 170, 170) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 0f, Color.FromArgb(255, 170, 170, 170) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 0f, Color.FromArgb(255, 0, 55, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 0f, Color.FromArgb(255, 0, 17, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 0f, Color.FromArgb(255, 0, 4, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 0f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 0f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 0f, Color.FromArgb(255, 83, 27, 133) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 0f, Color.FromArgb(255, 255, 47, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, 0f, Color.FromArgb(255, 70, 70, 70) };
        yield return new object[] { SystemColors.ControlDark, 0f, Color.FromArgb(255, 106, 106, 106) };
        yield return new object[] { SystemColors.Control, 0f, SystemColors.ControlDark };
        yield return new object[] { SystemColors.ControlLight, 0f, Color.FromArgb(255, 150, 150, 150) };
        yield return new object[] { SystemColors.ControlLightLight, 0f, Color.FromArgb(255, 170, 170, 170) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 0.25f, Color.FromArgb(255, 128, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 0.25f, Color.FromArgb(255, 0, 128, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 0.25f, Color.FromArgb(255, 0, 0, 128) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 0.25f, Color.FromArgb(255, 128, 128, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 0.25f, Color.FromArgb(255, 128, 0, 128) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 0.25f, Color.FromArgb(255, 0, 128, 128) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 0.25f, Color.FromArgb(255, 127, 127, 127) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 0.25f, Color.FromArgb(255, 0, 43, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 0.25f, Color.FromArgb(255, 0, 13, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 0.25f, Color.FromArgb(255, 0, 4, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 0.25f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 0.25f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 0.25f, Color.FromArgb(255, 63, 20, 101) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 0.25f, Color.FromArgb(255, 227, 0, 227) };
        yield return new object[] { SystemColors.ControlDarkDark, 0.25f, Color.FromArgb(255, 53, 53, 53) };
        yield return new object[] { SystemColors.ControlDark, 0.25f, Color.FromArgb(255, 79, 79, 79) };
        yield return new object[] { SystemColors.Control, 0.25f, Color.FromArgb(255, 147, 147, 147) };
        yield return new object[] { SystemColors.ControlLight, 0.25f, Color.FromArgb(255, 113, 113, 113) };
        yield return new object[] { SystemColors.ControlLightLight, 0.25f, Color.FromArgb(255, 127, 127, 127) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 0.5f, Color.FromArgb(255, 85, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 0.5f, Color.FromArgb(255, 0, 85, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 0.5f, Color.FromArgb(255, 0, 0, 85) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 0.5f, Color.FromArgb(255, 85, 85, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 0.5f, Color.FromArgb(255, 85, 0, 85) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 0.5f, Color.FromArgb(255, 0, 85, 85) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 0.5f, Color.FromArgb(255, 85, 85, 85) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 0.5f, Color.FromArgb(255, 0, 28, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 0.5f, Color.FromArgb(255, 0, 9, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 0.5f, Color.FromArgb(255, 0, 2, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 0.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 0.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 0.5f, Color.FromArgb(255, 41, 14, 67) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 0.5f, Color.FromArgb(255, 151, 0, 151) };
        yield return new object[] { SystemColors.ControlDarkDark, 0.5f, Color.FromArgb(255, 35, 35, 35) };
        yield return new object[] { SystemColors.ControlDark, 0.5f, Color.FromArgb(255, 53, 53, 53) };
        yield return new object[] { SystemColors.Control, 0.5f, Color.FromArgb(255, 133, 133, 133) };
        yield return new object[] { SystemColors.ControlLight, 0.5f, Color.FromArgb(255, 75, 75, 75) };
        yield return new object[] { SystemColors.ControlLightLight, 0.5f, Color.FromArgb(255, 85, 85, 85) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.ControlDarkDark, 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.ControlDark, 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.Control, 1f, SystemColors.ControlDarkDark };
        yield return new object[] { SystemColors.ControlLight, 1f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.ControlLightLight, 1f, Color.FromArgb(255, 0, 0, 0) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 1.5f, Color.FromArgb(255, 173, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 1.5f, Color.FromArgb(255, 0, 173, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 1.5f, Color.FromArgb(255, 0, 0, 173) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 1.5f, Color.FromArgb(255, 174, 173, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 1.5f, Color.FromArgb(255, 173, 0, 174) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 1.5f, Color.FromArgb(255, 0, 174, 173) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 1.5f, Color.FromArgb(255, 171, 171, 171) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 1.5f, Color.FromArgb(255, 0, 230, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 1.5f, Color.FromArgb(255, 0, 250, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 1.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 1.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 1.5f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 1.5f, Color.FromArgb(255, 218, 243, 192) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 1.5f, Color.FromArgb(255, 107, 0, 108) };
        yield return new object[] { SystemColors.ControlDarkDark, 1.5f, Color.FromArgb(255, 221, 221, 221) };
        yield return new object[] { SystemColors.ControlDark, 1.5f, Color.FromArgb(255, 203, 203, 203) };
        yield return new object[] { SystemColors.Control, 1.5f, Color.FromArgb(255, 78, 78, 78) };
        yield return new object[] { SystemColors.ControlLight, 1.5f, Color.FromArgb(255, 181, 181, 181) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Dark_Color_Float_TestData))]
    public void ControlPaint_Dark_InvokeColorFloat_ReturnsExpected(Color baseColor, float percOfDarkDark, Color expected)
    {
        Assert.Equal(expected, ControlPaint.Dark(baseColor, percOfDarkDark));

        // Call again to test caching.
        Assert.Equal(expected, ControlPaint.Dark(baseColor, percOfDarkDark));
    }

    public static IEnumerable<object[]> DarkDark_TestData()
    {
        yield return new object[] { Color.FromArgb(255, 255, 0, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.ControlDarkDark, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.ControlDark, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.Control, SystemColors.ControlDarkDark };
        yield return new object[] { SystemColors.ControlLight, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { SystemColors.ControlLightLight, Color.FromArgb(255, 0, 0, 0) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DarkDark_TestData))]
    public void ControlPaint_DarkDark_InvokeColorFloat_ReturnsExpected(Color baseColor, Color expected)
    {
        Assert.Equal(expected, ControlPaint.DarkDark(baseColor));
    }

    public static IEnumerable<object[]> DrawBorder_Graphics_Rectangle_Color_ButtonBorderStyle_TestData()
    {
        foreach (ButtonBorderStyle style in new ButtonBorderStyle[] { ButtonBorderStyle.Dashed, ButtonBorderStyle.Dotted, ButtonBorderStyle.Inset, ButtonBorderStyle.None, ButtonBorderStyle.Outset, ButtonBorderStyle.Solid, ButtonBorderStyle.None - 1, ButtonBorderStyle.Outset + 1 })
        {
            yield return new object[] { Rectangle.Empty, Color.Empty, style };
            yield return new object[] { new Rectangle(1, 2, -3, -4), Color.Empty, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), SystemColors.ControlLight, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), SystemColors.Control, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), SystemColors.ControlDark, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Black, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.White, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Transparent, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Empty, style };
            yield return new object[] { new Rectangle(1, 2, 4, 3), Color.Red, style };
            yield return new object[] { new Rectangle(1, 2, 3, 3), Color.Red, style };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder_Graphics_Rectangle_Color_ButtonBorderStyle_TestData))]
    public void ControlPaint_DrawBorder_GraphicsRectangleColorButtonBorderStyle_Success(Rectangle bounds, Color color, ButtonBorderStyle style)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder(graphics, bounds, color, style);

        // Call again to test caching.
        ControlPaint.DrawBorder(graphics, bounds, color, style);
    }

    [WinFormsTheory]
    [InvalidEnumData<ButtonBorderStyle>]
    [InlineData(ButtonBorderStyle.None)]
    public void ControlPaint_DrawBorder_GraphicsRectangleColorButtonBorderStyleInvalidStyle_Nop(ButtonBorderStyle style)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder(graphics, new Rectangle(1, 2, 3, 4), Color.Red, style);

        // Call again to test caching.
        ControlPaint.DrawBorder(graphics, new Rectangle(1, 2, 3, 4), Color.Red, style);
    }

    public static IEnumerable<object[]> DrawBorder_Graphics_Rectangle_Color_Int_ButtonBorderStyle_Color_Int_ButtonBorderStyle_Color_Int_ButtonBorderStyle_Color_Int_ButtonBorderStyle_TestData()
    {
        foreach (ButtonBorderStyle style in new ButtonBorderStyle[] { ButtonBorderStyle.Dashed, ButtonBorderStyle.Dotted, ButtonBorderStyle.Inset, ButtonBorderStyle.None, ButtonBorderStyle.Outset, ButtonBorderStyle.Solid, ButtonBorderStyle.None - 1, ButtonBorderStyle.Outset + 1 })
        {
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { Rectangle.Empty, Color.Red, 1, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, -3, -4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, 4, style };

            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 0, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 0, style, Color.Blue, 0, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 0, style, Color.Blue, 3, style, Color.Yellow, 0, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 0, style, Color.Blue, 0, style, Color.Yellow, 0, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 2, style, Color.Blue, 0, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 2, style, Color.Blue, 0, style, Color.Yellow, 0, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 0, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, 0, style };

            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 0, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 0, style, Color.Blue, 0, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 0, style, Color.Blue, 3, style, Color.Yellow, 0, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 0, style, Color.Blue, 0, style, Color.Yellow, 0, style };

            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, 0, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, 0, style, Color.Yellow, 0, style };

            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, 0, style };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder_Graphics_Rectangle_Color_Int_ButtonBorderStyle_Color_Int_ButtonBorderStyle_Color_Int_ButtonBorderStyle_Color_Int_ButtonBorderStyle_TestData))]
    public void ControlPaint_DrawBorder_GraphicsRectangleColorIntButtonBorderStyleColorIntButtonBorderStyleColorIntButtonBorderStyleColorIntButtonBorderStyle_Success(
        Rectangle bounds,
        Color leftColor, int leftWidth, ButtonBorderStyle leftStyle,
        Color topColor, int topWidth, ButtonBorderStyle topStyle,
        Color rightColor, int rightWidth, ButtonBorderStyle rightStyle,
        Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder(graphics, bounds, leftColor, leftWidth, leftStyle, topColor, topWidth, topStyle, rightColor, rightWidth, rightStyle, bottomColor, bottomWidth, bottomStyle);

        // Call again to test caching.
        ControlPaint.DrawBorder(graphics, bounds, leftColor, leftWidth, leftStyle, topColor, topWidth, topStyle, rightColor, rightWidth, rightStyle, bottomColor, bottomWidth, bottomStyle);
    }

    public static IEnumerable<object[]> DrawBorder_OutOfRange_TestData()
    {
        foreach (ButtonBorderStyle style in new ButtonBorderStyle[] { ButtonBorderStyle.Dashed, ButtonBorderStyle.Dotted, ButtonBorderStyle.Inset, ButtonBorderStyle.None, ButtonBorderStyle.Outset, ButtonBorderStyle.Solid, ButtonBorderStyle.None - 1, ButtonBorderStyle.Outset + 1 })
        {
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, -1, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, -1, style, Color.Blue, -1, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, -1, style, Color.Blue, 3, style, Color.Yellow, -1, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, -1, style, Color.Blue, -1, style, Color.Yellow, -1, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, 2, style, Color.Blue, -1, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, 2, style, Color.Blue, -1, style, Color.Yellow, -1, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, -1, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, -1, style };

            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, -1, style, Color.Blue, 3, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, -1, style, Color.Blue, -1, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, -1, style, Color.Blue, 3, style, Color.Yellow, -1, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, -1, style, Color.Blue, -1, style, Color.Yellow, -1, style };

            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, -1, style, Color.Yellow, 4, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, -1, style, Color.Yellow, -1, style };

            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Green, 2, style, Color.Blue, 3, style, Color.Yellow, -1, style };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder_OutOfRange_TestData))]
    public void ControlPaint_DrawBorder_OutOfRange_ThrowsOutOfRangeException(
        Rectangle bounds,
        Color leftColor, int leftWidth, ButtonBorderStyle leftStyle,
        Color topColor, int topWidth, ButtonBorderStyle topStyle,
        Color rightColor, int rightWidth, ButtonBorderStyle rightStyle,
        Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>(() => ControlPaint.DrawBorder(graphics, bounds, leftColor, leftWidth, leftStyle, topColor, topWidth, topStyle, rightColor, rightWidth, rightStyle, bottomColor, bottomWidth, bottomStyle));
    }

    [WinFormsTheory]
    [InlineData(ButtonBorderStyle.Dashed)]
    [InlineData(ButtonBorderStyle.Dotted)]
    [InlineData(ButtonBorderStyle.Inset)]
    [InlineData(ButtonBorderStyle.Outset)]
    [InlineData(ButtonBorderStyle.Solid)]
    public void ControlPaint_DrawBorder_NullGraphics_ThrowsArgumentNullException(ButtonBorderStyle style)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder(null, new Rectangle(1, 2, 3, 4), Color.Red, style));
    }

    [WinFormsTheory]
    [EnumData<ButtonBorderStyle>]
    [InvalidEnumData<ButtonBorderStyle>]
    public void ControlPaint_DrawBorder_NullGraphicsComplex_ThrowsArgumentNullException(ButtonBorderStyle style)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder(null, new Rectangle(1, 2, 3, 4), Color.Red, 1, style, Color.Red, 1, style, Color.Red, 1, style, Color.Red, 1, style));
    }

    public static IEnumerable<object[]> DrawBorder3D_Graphics_Rectangle_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, -3, -4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 4, 3) };
        yield return new object[] { new Rectangle(1, 2, 3, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder3D_Graphics_Rectangle_TestData))]
    public void ControlPaint_DrawBorder3D_InvokeGraphicsRectangle_Success(Rectangle rectangle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder3D(graphics, rectangle);

        // Call again to test caching.
        ControlPaint.DrawBorder3D(graphics, rectangle);
    }

    public static IEnumerable<object[]> DrawBorder3D_Graphics_RectangleBorder3DStyle_TestData()
    {
        foreach (Border3DStyle style in Enum.GetValues(typeof(Border3DStyle)))
        {
            yield return new object[] { Rectangle.Empty, style };
            yield return new object[] { new Rectangle(1, 2, -3, -4), style };
            yield return new object[] { new Rectangle(0, 0, 3, 4), style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), style };
            yield return new object[] { new Rectangle(1, 2, 4, 3), style };
            yield return new object[] { new Rectangle(1, 2, 3, 3), style };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder3D_Graphics_RectangleBorder3DStyle_TestData))]
    public void ControlPaint_DrawBorder3D_InvokeGraphicsRectangleBorder3DStyle_Success(Rectangle rectangle, Border3DStyle style)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder3D(graphics, rectangle, style);

        // Call again to test caching.
        ControlPaint.DrawBorder3D(graphics, rectangle, style);
    }

    public static IEnumerable<object[]> DrawBorder3D_Graphics_RectangleBorder3DStyleBorder3DSide_TestData()
    {
        foreach (Border3DStyle style in Enum.GetValues(typeof(Border3DStyle)))
        {
            foreach (Border3DSide side in Enum.GetValues(typeof(Border3DSide)))
            {
                yield return new object[] { Rectangle.Empty, style, side };
                yield return new object[] { new Rectangle(1, 2, -3, -4), style, side };
                yield return new object[] { new Rectangle(0, 0, 3, 4), style, side };
                yield return new object[] { new Rectangle(1, 2, 3, 4), style, side };
                yield return new object[] { new Rectangle(1, 2, 4, 3), style, side };
                yield return new object[] { new Rectangle(1, 2, 3, 3), style, side };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder3D_Graphics_RectangleBorder3DStyleBorder3DSide_TestData))]
    public void ControlPaint_DrawBorder3D_InvokeGraphicsRectangleBorder3DStyleBorder3DSide_Success(Rectangle rectangle, Border3DStyle style, Border3DSide side)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder3D(graphics, rectangle, style, side);

        // Call again to test caching.
        ControlPaint.DrawBorder3D(graphics, rectangle, style, side);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder3D_Graphics_Rectangle_TestData))]
    public void ControlPaint_DrawBorder3D_InvokeGraphicsIntIntIntInt_Success(Rectangle rectangle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        // Call again to test caching.
        ControlPaint.DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder3D_Graphics_RectangleBorder3DStyle_TestData))]
    public void ControlPaint_DrawBorder3D_InvokeGraphicsIntIntIntIntBorder3DStyleSuccess(Rectangle rectangle, Border3DStyle style)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style);

        // Call again to test caching.
        ControlPaint.DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawBorder3D_Graphics_RectangleBorder3DStyleBorder3DSide_TestData))]
    public void ControlPaint_DrawBorder3D_InvokeGraphicsIntIntIntIntBorder3DStyleBorder3DSide_Success(Rectangle rectangle, Border3DStyle style, Border3DSide side)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style, side);

        // Call again to test caching.
        ControlPaint.DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style, side);
    }

    [WinFormsTheory]
    [EnumData<Border3DStyle>]
    [InvalidEnumData<Border3DStyle>]
    public void ControlPaint_DrawBorder3D_NullGraphics_ThrowsArgumentNullException(Border3DStyle style)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder3D(null, new Rectangle(1, 2, 3, 4)));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder3D(null, new Rectangle(1, 2, 3, 4), style));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder3D(null, new Rectangle(1, 2, 3, 4), style, Border3DSide.All));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder3D(null, 1, 2, 3, 4));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder3D(null, 1, 2, 3, 4, style));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawBorder3D(null, 1, 2, 3, 4, style, Border3DSide.All));
    }

    public static IEnumerable<object[]> DrawButton_Graphics_Rectangle_ButtonState_TestData()
    {
        foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
        {
            yield return new object[] { new Rectangle(0, 0, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 4, 3), state };
            yield return new object[] { new Rectangle(1, 2, 3, 3), state };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawButton_InvokeGraphicsRectangleButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawButton(graphics, rectangle, state);

        // Call again to test caching.
        ControlPaint.DrawButton(graphics, rectangle, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawButton_InvokeGraphicsIntIntIntIntButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        // Call again to test caching.
        ControlPaint.DrawButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawButton_NullGraphics_ThrowsArgumentNullException(ButtonState state)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawButton(null, new Rectangle(1, 2, 3, 4), state));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawButton(null, 1, 2, 3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(0, 4, ButtonState.All)]
    [InlineData(0, 4, ButtonState.Normal)]
    [InlineData(3, 0, ButtonState.All)]
    [InlineData(3, 0, ButtonState.Normal)]
    [InlineData(0, 0, ButtonState.All)]
    [InlineData(0, 0, ButtonState.Normal)]
    public void ControlPaint_DrawButton_EmptyRectangle_ThrowsArgumentException(int width, int height, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawButton(graphics, new Rectangle(0, 0, width, height), state));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawButton(graphics, 0, 0, width, height, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawButton_NegativeWidth_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawButton(graphics, new Rectangle(0, 0, -3, 4), state));
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawButton(graphics, 0, 0, -3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawButton_NegativeHeight_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawButton(graphics, new Rectangle(0, 0, 3, -4), state));
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawButton(graphics, 0, 0, 3, -4, state));
    }

    public static IEnumerable<object[]> DrawCaptionButton_Graphics_Rectangle_ButtonState_TestData()
    {
        foreach (CaptionButton button in Enum.GetValues(typeof(CaptionButton)))
        {
            foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
            {
                yield return new object[] { new Rectangle(0, 0, 3, 4), button, state };
                yield return new object[] { new Rectangle(1, 2, 3, 4), button, state };
                yield return new object[] { new Rectangle(1, 2, 4, 3), button, state };
                yield return new object[] { new Rectangle(1, 2, 3, 3), button, state };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawCaptionButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawCaptionButton_InvokeGraphicsRectangleButtonState_Success(Rectangle rectangle, CaptionButton button, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawCaptionButton(graphics, rectangle, button, state);

        // Call again to test caching.
        ControlPaint.DrawCaptionButton(graphics, rectangle, button, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawCaptionButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawCaptionButton_InvokeGraphicsIntIntIntIntButtonState_Success(Rectangle rectangle, CaptionButton button, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawCaptionButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, button, state);

        // Call again to test caching.
        ControlPaint.DrawCaptionButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, button, state);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawCaptionButton_NullGraphics_ThrowsArgumentNullException(ButtonState state)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawCaptionButton(null, new Rectangle(1, 2, 3, 4), CaptionButton.Close, state));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawCaptionButton(null, 1, 2, 3, 4, CaptionButton.Close, state));
    }

    [WinFormsTheory]
    [InlineData(0, 4, ButtonState.All)]
    [InlineData(0, 4, ButtonState.Normal)]
    [InlineData(3, 0, ButtonState.All)]
    [InlineData(3, 0, ButtonState.Normal)]
    [InlineData(0, 0, ButtonState.All)]
    public void ControlPaint_DrawCaptionButton_EmptyRectangle_ThrowsArgumentException(int width, int height, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawCaptionButton(graphics, new Rectangle(0, 0, width, height), CaptionButton.Close, state));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawCaptionButton(graphics, 0, 0, width, height, CaptionButton.Close, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawCaptionButton_NegativeWidth_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawCaptionButton(graphics, new Rectangle(0, 0, -3, 4), CaptionButton.Close, state));
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawCaptionButton(graphics, 0, 0, -3, 4, CaptionButton.Close, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawCaptionButton_NegativeHeight_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawCaptionButton(graphics, new Rectangle(0, 0, 3, -4), CaptionButton.Close, state));
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawCaptionButton(graphics, 0, 0, 3, -4, CaptionButton.Close, state));
    }

    public static IEnumerable<object[]> DrawCheckBox_Graphics_Rectangle_ButtonState_TestData()
    {
        foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
        {
            yield return new object[] { new Rectangle(0, 0, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 4, 3), state };
            yield return new object[] { new Rectangle(1, 2, 3, 3), state };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawCheckBox_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawCheckBox_InvokeGraphicsRectangleButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawCheckBox(graphics, rectangle, state);

        // Call again to test caching.
        ControlPaint.DrawCheckBox(graphics, rectangle, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawCheckBox_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawCheckBox_InvokeGraphicsIntIntIntIntButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        // Call again to test caching.
        ControlPaint.DrawCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawCheckBox_NullGraphics_ThrowsArgumentNullException(ButtonState state)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawCheckBox(null, new Rectangle(1, 2, 3, 4), state));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawCheckBox(null, 1, 2, 3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(0, 4, ButtonState.All)]
    [InlineData(0, 4, ButtonState.Normal)]
    [InlineData(3, 0, ButtonState.All)]
    [InlineData(3, 0, ButtonState.Normal)]
    [InlineData(0, 0, ButtonState.All)]
    [InlineData(0, 0, ButtonState.Normal)]
    public void ControlPaint_DrawCheckBox_EmptyRectangle_ThrowsArgumentException(int width, int height, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawCheckBox(graphics, new Rectangle(0, 0, width, height), state));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawCheckBox(graphics, 0, 0, width, height, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All, "rectangle")]
    [InlineData(ButtonState.Normal, "width")]
    public void ControlPaint_DrawCheckBox_NegativeWidth_ThrowsArgumentOutOfRangeException(ButtonState state, string expectedParamName)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>(expectedParamName, () => ControlPaint.DrawCheckBox(graphics, new Rectangle(0, 0, -3, 4), state));
        Assert.Throws<ArgumentOutOfRangeException>(expectedParamName, () => ControlPaint.DrawCheckBox(graphics, 0, 0, -3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All, "rectangle")]
    [InlineData(ButtonState.Normal, "height")]
    public void ControlPaint_DrawCheckBox_NegativeHeight_ThrowsArgumentOutOfRangeException(ButtonState state, string expectedParamName)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>(expectedParamName, () => ControlPaint.DrawCheckBox(graphics, new Rectangle(0, 0, 3, -4), state));
        Assert.Throws<ArgumentOutOfRangeException>(expectedParamName, () => ControlPaint.DrawCheckBox(graphics, 0, 0, 3, -4, state));
    }

    public static IEnumerable<object[]> DrawComboButton_Graphics_Rectangle_ButtonState_TestData()
    {
        foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
        {
            yield return new object[] { new Rectangle(0, 0, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 4, 3), state };
            yield return new object[] { new Rectangle(1, 2, 3, 3), state };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawComboButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawComboButton_InvokeGraphicsRectangleButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawComboButton(graphics, rectangle, state);

        // Call again to test caching.
        ControlPaint.DrawComboButton(graphics, rectangle, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawComboButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawComboButton_InvokeGraphicsIntIntIntIntButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawComboButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        // Call again to test caching.
        ControlPaint.DrawComboButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawComboButton_NullGraphics_ThrowsArgumentNullException(ButtonState state)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawComboButton(null, new Rectangle(1, 2, 3, 4), state));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawComboButton(null, 1, 2, 3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(0, 4, ButtonState.All)]
    [InlineData(0, 4, ButtonState.Normal)]
    [InlineData(3, 0, ButtonState.All)]
    [InlineData(3, 0, ButtonState.Normal)]
    [InlineData(0, 0, ButtonState.All)]
    public void ControlPaint_DrawComboButton_EmptyRectangle_ThrowsArgumentException(int width, int height, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawComboButton(graphics, new Rectangle(0, 0, width, height), state));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawComboButton(graphics, 0, 0, width, height, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawComboButton_NegativeWidth_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawComboButton(graphics, new Rectangle(0, 0, -3, 4), state));
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawComboButton(graphics, 0, 0, -3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawComboButton_NegativeHeight_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawComboButton(graphics, new Rectangle(0, 0, 3, -4), state));
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawComboButton(graphics, 0, 0, 3, -4, state));
    }

    public static IEnumerable<object[]> DrawContainerGrabHandle_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, -3, -4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 4, 3) };
        yield return new object[] { new Rectangle(1, 2, 3, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawContainerGrabHandle_TestData))]
    public void ControlPaint_DrawContainerGrabHandle_Invoke_Success(Rectangle rectangle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawContainerGrabHandle(graphics, rectangle);

        // Call again to test caching.
        ControlPaint.DrawContainerGrabHandle(graphics, rectangle);
    }

    [WinFormsFact]
    public void ControlPaint_DrawContainerGrabHandle_NullGraphics_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawContainerGrabHandle(null, new Rectangle(1, 2, 3, 4)));
    }

    public static IEnumerable<object[]> DrawFocusRectangle_Graphics_Rectangle_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, -3, -4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 4, 3) };
        yield return new object[] { new Rectangle(1, 2, 3, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawFocusRectangle_Graphics_Rectangle_TestData))]
    public void ControlPaint_DrawFocusRectangle_InvokeGraphicsRectangle_Success(Rectangle rectangle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawFocusRectangle(graphics, rectangle);

        // Call again to test caching.
        ControlPaint.DrawFocusRectangle(graphics, rectangle);
    }

    public static IEnumerable<object[]> DrawFocusRectangle_Graphics_Rectangle_Color_Color_TestData()
    {
        yield return new object[] { Rectangle.Empty, Color.Red, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, -3, -4), Color.Red, Color.Blue };
        yield return new object[] { new Rectangle(0, 0, 3, 4), Color.Red, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Black, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.White, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Empty, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Transparent, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.Black };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.White };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.Transparent };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, Color.Red };
        yield return new object[] { new Rectangle(1, 2, 4, 3), Color.Red, Color.Blue };
        yield return new object[] { new Rectangle(1, 2, 3, 3), Color.Red, Color.Blue };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawFocusRectangle_Graphics_Rectangle_Color_Color_TestData))]
    public void ControlPaint_DrawFocusRectangle_Invoke_Success(Rectangle rectangle, Color foreColor, Color backColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawFocusRectangle(graphics, rectangle, foreColor, backColor);

        // Call again to test caching.
        ControlPaint.DrawFocusRectangle(graphics, rectangle, foreColor, backColor);
    }

    [WinFormsFact]
    public void ControlPaint_DrawFocusRectangle_NullGraphics_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawFocusRectangle(null, new Rectangle(1, 2, 3, 4)));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawFocusRectangle(null, new Rectangle(1, 2, 3, 4), Color.Red, Color.Blue));
    }

    public static IEnumerable<object[]> DrawGrabHandle_TestData()
    {
        foreach (bool primary in new bool[] { true, false })
        {
            foreach (bool enabled in new bool[] { true, false })
            {
                yield return new object[] { Rectangle.Empty, primary, enabled };
                yield return new object[] { new Rectangle(1, 2, -3, -4), primary, enabled };
                yield return new object[] { new Rectangle(0, 0, 3, 4), primary, enabled };
                yield return new object[] { new Rectangle(1, 2, 3, 4), primary, enabled };
                yield return new object[] { new Rectangle(1, 2, 4, 3), primary, enabled };
                yield return new object[] { new Rectangle(1, 2, 3, 3), primary, enabled };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawGrabHandle_TestData))]
    public void ControlPaint_DrawGrabHandle_Invoke_Success(Rectangle rectangle, bool primary, bool enabled)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawGrabHandle(graphics, rectangle, primary, enabled);

        // Call again to test caching.
        ControlPaint.DrawGrabHandle(graphics, rectangle, primary, enabled);
    }

    [WinFormsTheory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void ControlPaint_DrawGrabHandle_NullGraphics_ThrowsArgumentNullException(bool primary, bool enabled)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawGrabHandle(null, new Rectangle(1, 2, 3, 4), primary, enabled));
    }

    public static IEnumerable<object[]> DrawGrid_TestData()
    {
        yield return new object[] { Rectangle.Empty, new Size(1, 1), Color.Red };
        yield return new object[] { new Rectangle(1, 2, -3, -4), new Size(1, 1), Color.Red };
        yield return new object[] { new Rectangle(0, 0, 3, 4), new Size(1, 1), Color.Red };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), Color.Red };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), Color.Black };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), Color.White };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), Color.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(1, 1), Color.Transparent };
        yield return new object[] { new Rectangle(1, 2, 3, 4), new Size(10, 20), Color.Red };
        yield return new object[] { new Rectangle(1, 2, 4, 3), new Size(10, 20), Color.Red };
        yield return new object[] { new Rectangle(1, 2, 3, 3), new Size(10, 20), Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawGrid_TestData))]
    public void ControlPaint_DrawGrid_Invoke_Success(Rectangle area, Size pixelsBetweenDots, Color backColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawGrid(graphics, area, pixelsBetweenDots, backColor);

        // Call again to test caching.
        ControlPaint.DrawGrid(graphics, area, pixelsBetweenDots, backColor);
    }

    [WinFormsFact]
    public void ControlPaint_DrawGrid_NullGraphics_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawGrid(null, new Rectangle(1, 2, 3, 4), new Size(1, 1), Color.Red));
    }

    [WinFormsTheory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void ControlPaint_DrawGrid_InvalidPixelsBetweenDots_ThrowsArgumentOutOfRangeException(int width, int height)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("pixelsBetweenDots", () => ControlPaint.DrawGrid(graphics, new Rectangle(0, 0, width, height), new Size(width, height), Color.Red));
    }

    public static IEnumerable<object[]> DrawImageDisabled_TestData()
    {
        yield return new object[] { new Bitmap(10, 10), -10, -20, Color.Red };
        yield return new object[] { new Bitmap(10, 10), 0, 0, Color.Red };
        yield return new object[] { new Bitmap(10, 10), 1, 2, Color.Red };
        yield return new object[] { new Bitmap(10, 10), 0, 0, Color.Black };
        yield return new object[] { new Bitmap(10, 10), 0, 0, Color.White };
        yield return new object[] { new Bitmap(10, 10), 0, 0, Color.Transparent };
        yield return new object[] { new Bitmap(10, 10), 0, 0, Color.Empty };

        yield return new object[] { new Metafile("bitmaps/telescope_01.wmf"), -10, -20, Color.Red };
        yield return new object[] { new Metafile("bitmaps/telescope_01.wmf"), 0, 0, Color.Red };
        yield return new object[] { new Metafile("bitmaps/telescope_01.wmf"), 1, 2, Color.Red };
        yield return new object[] { new Metafile("bitmaps/telescope_01.wmf"), 0, 0, Color.Black };
        yield return new object[] { new Metafile("bitmaps/telescope_01.wmf"), 0, 0, Color.White };
        yield return new object[] { new Metafile("bitmaps/telescope_01.wmf"), 0, 0, Color.Transparent };
        yield return new object[] { new Metafile("bitmaps/telescope_01.wmf"), 0, 0, Color.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawImageDisabled_TestData))]
    public void ControlPaint_DrawImageDisabled_Invoke_Success(Image image, int x, int y, Color background)
    {
        using Bitmap sourceImage = new(10, 10);
        using Graphics graphics = Graphics.FromImage(sourceImage);
        ControlPaint.DrawImageDisabled(graphics, image, x, y, background);

        // Call again to test caching.
        ControlPaint.DrawImageDisabled(graphics, image, x, y, background);
    }

    [WinFormsFact]
    public void ControlPaint_DrawImageDisabled_NullGraphics_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawImageDisabled(null, image, 0, 0, Color.Red));
    }

    [WinFormsFact]
    public void ControlPaint_DrawImageDisabled_NullImage_ThrowsNullReferenceException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<NullReferenceException>(() => ControlPaint.DrawImageDisabled(graphics, null, 0, 0, Color.Red));
    }

    public static IEnumerable<object[]> DrawLockedFrame_TestData()
    {
        foreach (bool primary in new bool[] { true, false })
        {
            yield return new object[] { Rectangle.Empty, primary };
            yield return new object[] { new Rectangle(1, 2, -3, -4), primary };
            yield return new object[] { new Rectangle(0, 0, 3, 4), primary };
            yield return new object[] { new Rectangle(1, 2, 3, 4), primary };
            yield return new object[] { new Rectangle(1, 2, 4, 3), primary };
            yield return new object[] { new Rectangle(1, 2, 3, 3), primary };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawLockedFrame_TestData))]
    public void ControlPaint_DrawLockedFrame_Invoke_Success(Rectangle rectangle, bool primary)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawLockedFrame(graphics, rectangle, primary);

        // Call again to test caching.
        ControlPaint.DrawLockedFrame(graphics, rectangle, primary);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ControlPaint_DrawLockedFrame_NullGraphics_ThrowsArgumentNullException(bool primary)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawLockedFrame(null, new Rectangle(1, 2, 3, 4), primary));
    }

    public static IEnumerable<object[]> DrawMenuGlyph_Graphics_Rectangle_MenuGlyph_TestData()
    {
        foreach (MenuGlyph glyph in Enum.GetValues(typeof(MenuGlyph)))
        {
            yield return new object[] { new Rectangle(0, 0, 3, 4), glyph };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph };
            yield return new object[] { new Rectangle(1, 2, 4, 3), glyph };
            yield return new object[] { new Rectangle(1, 2, 3, 3), glyph };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawMenuGlyph_Graphics_Rectangle_MenuGlyph_TestData))]
    public void ControlPaint_DrawMenuGlyph_InvokeGraphicsRectangleMenuGlyph_Success(Rectangle rectangle, MenuGlyph glyph)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawMenuGlyph(graphics, rectangle, glyph);

        // Call again to test caching.
        ControlPaint.DrawMenuGlyph(graphics, rectangle, glyph);
    }

    public static IEnumerable<object[]> DrawMenuGlyph_Graphics_Rectangle_MenuGlyph_Color_Color_TestData()
    {
        foreach (MenuGlyph glyph in Enum.GetValues(typeof(MenuGlyph)))
        {
            yield return new object[] { new Rectangle(0, 0, 3, 4), glyph, Color.Red, Color.Blue };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Red, Color.Blue };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Black, Color.Blue };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.White, Color.Blue };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Empty, Color.Blue };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Transparent, Color.Blue };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Red, Color.Black };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Red, Color.White };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Red, Color.Empty };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Red, Color.Transparent };
            yield return new object[] { new Rectangle(1, 2, 3, 4), glyph, Color.Red, Color.Red };
            yield return new object[] { new Rectangle(1, 2, 4, 3), glyph, Color.Red, Color.Red };
            yield return new object[] { new Rectangle(1, 2, 3, 3), glyph, Color.Red, Color.Red };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawMenuGlyph_Graphics_Rectangle_MenuGlyph_Color_Color_TestData))]
    public void ControlPaint_DrawMenuGlyph_InvokeGraphicsRectangleMenuGlyphColorColor_Success(Rectangle rectangle, MenuGlyph glyph, Color foreColor, Color backColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawMenuGlyph(graphics, rectangle, glyph, foreColor, backColor);

        // Call again to test caching.
        ControlPaint.DrawMenuGlyph(graphics, rectangle, glyph, foreColor, backColor);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawMenuGlyph_Graphics_Rectangle_MenuGlyph_TestData))]
    public void ControlPaint_DrawMenuGlyph_InvokeGraphicsIntIntIntIntMenuGlyph_Success(Rectangle rectangle, MenuGlyph glyph)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawMenuGlyph(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, glyph);

        // Call again to test caching.
        ControlPaint.DrawMenuGlyph(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, glyph);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawMenuGlyph_Graphics_Rectangle_MenuGlyph_Color_Color_TestData))]
    public void ControlPaint_DrawMenuGlyph_InvokeGraphicsIntIntIntIntMenuGlyphColorColor_Success(Rectangle rectangle, MenuGlyph glyph, Color foreColor, Color backColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawMenuGlyph(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, glyph, foreColor, backColor);

        // Call again to test caching.
        ControlPaint.DrawMenuGlyph(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, glyph, foreColor, backColor);
    }

    [WinFormsTheory]
    [InlineData(MenuGlyph.Arrow)]
    [InlineData(MenuGlyph.Bullet)]
    [InlineData(MenuGlyph.Checkmark)]
    public void ControlPaint_DrawMenuGlyph_NullGraphics_ThrowsArgumentNullException(MenuGlyph glyph)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawMenuGlyph(null, new Rectangle(1, 2, 3, 4), glyph));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawMenuGlyph(null, 1, 2, 3, 4, glyph));
    }

    [WinFormsTheory]
    [InlineData(0, 4, MenuGlyph.Arrow)]
    [InlineData(0, 4, MenuGlyph.Bullet)]
    [InlineData(0, 4, MenuGlyph.Checkmark)]
    [InlineData(3, 0, MenuGlyph.Arrow)]
    [InlineData(3, 0, MenuGlyph.Bullet)]
    [InlineData(3, 0, MenuGlyph.Checkmark)]
    [InlineData(0, 0, MenuGlyph.Arrow)]
    [InlineData(0, 0, MenuGlyph.Bullet)]
    [InlineData(0, 0, MenuGlyph.Checkmark)]
    public void ControlPaint_DrawMenuGlyph_EmptyRectangle_ThrowsArgumentException(int width, int height, MenuGlyph glyph)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawMenuGlyph(graphics, new Rectangle(0, 0, width, height), glyph));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawMenuGlyph(graphics, 0, 0, width, height, glyph));
    }

    [WinFormsTheory]
    [InlineData(MenuGlyph.Arrow)]
    [InlineData(MenuGlyph.Bullet)]
    [InlineData(MenuGlyph.Checkmark)]
    public void ControlPaint_DrawMenuGlyph_NegativeWidth_ThrowsArgumentOutOfRangeException(MenuGlyph glyph)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawMenuGlyph(graphics, new Rectangle(0, 0, -3, 4), glyph));
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawMenuGlyph(graphics, 0, 0, -3, 4, glyph));
    }

    [WinFormsTheory]
    [InlineData(MenuGlyph.Min)]
    [InlineData(MenuGlyph.Bullet)]
    [InlineData(MenuGlyph.Checkmark)]
    public void ControlPaint_DrawMenuGlyph_NegativeHeight_ThrowsArgumentOutOfRangeException(MenuGlyph glyph)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawMenuGlyph(graphics, new Rectangle(0, 0, 3, -4), glyph));
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawMenuGlyph(graphics, 0, 0, 3, -4, glyph));
    }

    public static IEnumerable<object[]> DrawMixedCheckBox_Graphics_Rectangle_ButtonState_TestData()
    {
        foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
        {
            yield return new object[] { new Rectangle(0, 0, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 4, 3), state };
            yield return new object[] { new Rectangle(1, 2, 3, 3), state };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawMixedCheckBox_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawMixedCheckBox_InvokeGraphicsRectangleButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawMixedCheckBox(graphics, rectangle, state);

        // Call again to test caching.
        ControlPaint.DrawMixedCheckBox(graphics, rectangle, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawMixedCheckBox_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawMixedCheckBox_InvokeGraphicsIntIntIntIntButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawMixedCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        // Call again to test caching.
        ControlPaint.DrawMixedCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawMixedCheckBox_NullGraphics_ThrowsArgumentNullException(ButtonState state)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawMixedCheckBox(null, new Rectangle(1, 2, 3, 4), state));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawMixedCheckBox(null, 1, 2, 3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(0, 4, ButtonState.All)]
    [InlineData(0, 4, ButtonState.Normal)]
    [InlineData(3, 0, ButtonState.All)]
    [InlineData(3, 0, ButtonState.Normal)]
    [InlineData(0, 0, ButtonState.All)]
    public void ControlPaint_DrawMixedCheckBox_EmptyRectangle_ThrowsArgumentException(int width, int height, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawMixedCheckBox(graphics, new Rectangle(0, 0, width, height), state));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawMixedCheckBox(graphics, 0, 0, width, height, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawMixedCheckBox_NegativeWidth_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawMixedCheckBox(graphics, new Rectangle(0, 0, -3, 4), state));
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawMixedCheckBox(graphics, 0, 0, -3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawMixedCheckBox_NegativeHeight_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawMixedCheckBox(graphics, new Rectangle(0, 0, 3, -4), state));
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawMixedCheckBox(graphics, 0, 0, 3, -4, state));
    }

    public static IEnumerable<object[]> DrawRadioButton_Graphics_Rectangle_ButtonState_TestData()
    {
        foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
        {
            yield return new object[] { new Rectangle(0, 0, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 3, 4), state };
            yield return new object[] { new Rectangle(1, 2, 4, 3), state };
            yield return new object[] { new Rectangle(1, 2, 3, 3), state };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawRadioButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawRadioButton_InvokeGraphicsRectangleButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawRadioButton(graphics, rectangle, state);

        // Call again to test caching.
        ControlPaint.DrawRadioButton(graphics, rectangle, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawRadioButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawRadioButton_InvokeGraphicsIntIntIntIntButtonState_Success(Rectangle rectangle, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawRadioButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        // Call again to test caching.
        ControlPaint.DrawRadioButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawRadioButton_NullGraphics_ThrowsArgumentNullException(ButtonState state)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawRadioButton(null, new Rectangle(1, 2, 3, 4), state));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawRadioButton(null, 1, 2, 3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(0, 4, ButtonState.All)]
    [InlineData(0, 4, ButtonState.Normal)]
    [InlineData(3, 0, ButtonState.All)]
    [InlineData(3, 0, ButtonState.Normal)]
    [InlineData(0, 0, ButtonState.All)]
    public void ControlPaint_DrawRadioButton_EmptyRectangle_ThrowsArgumentException(int width, int height, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawRadioButton(graphics, new Rectangle(0, 0, width, height), state));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawRadioButton(graphics, 0, 0, width, height, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawRadioButton_NegativeWidth_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawRadioButton(graphics, new Rectangle(0, 0, -3, 4), state));
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawRadioButton(graphics, 0, 0, -3, 4, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawRadioButton_NegativeHeight_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawRadioButton(graphics, new Rectangle(0, 0, 3, -4), state));
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawRadioButton(graphics, 0, 0, 3, -4, state));
    }

    public static IEnumerable<object[]> DrawReversibleFrame_TestData()
    {
        foreach (FrameStyle style in new FrameStyle[] { FrameStyle.Dashed, FrameStyle.Thick, FrameStyle.Dashed - 1, FrameStyle.Thick + 1 })
        {
            yield return new object[] { Rectangle.Empty, Color.Empty, style };
            yield return new object[] { new Rectangle(1, 2, -3, -4), Color.Empty, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), SystemColors.ControlLight, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), SystemColors.Control, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), SystemColors.ControlDark, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Black, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.White, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Transparent, style };
            yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Empty, style };
            yield return new object[] { new Rectangle(1, 2, 4, 3), Color.Red, style };
            yield return new object[] { new Rectangle(1, 2, 3, 3), Color.Red, style };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawReversibleFrame_TestData))]
    public void ControlPaint_DrawReversibleFrame_Invoke_Success(Rectangle rectangle, Color backColor, FrameStyle style)
    {
        ControlPaint.DrawReversibleFrame(rectangle, backColor, style);

        // Call again to test caching.
        ControlPaint.DrawReversibleFrame(rectangle, backColor, style);
    }

    public static IEnumerable<object[]> DrawReversibleLine_TestData()
    {
        yield return new object[] { Point.Empty, Point.Empty, Color.Red };
        yield return new object[] { new Point(1, 2), new Point(1, 2), Color.Red };
        yield return new object[] { new Point(2, 3), new Point(1, 2), Color.Red };
        yield return new object[] { new Point(1, 2), new Point(2, 3), Color.Empty };
        yield return new object[] { new Point(1, 2), new Point(2, 3), Color.Red };
        yield return new object[] { new Point(1, 2), new Point(2, 3), SystemColors.ControlLight };
        yield return new object[] { new Point(1, 2), new Point(2, 3), SystemColors.Control };
        yield return new object[] { new Point(1, 2), new Point(2, 3), SystemColors.ControlDark };
        yield return new object[] { new Point(1, 2), new Point(2, 3), Color.Black };
        yield return new object[] { new Point(1, 2), new Point(2, 3), Color.White };
        yield return new object[] { new Point(1, 2), new Point(2, 3), Color.Transparent };
        yield return new object[] { new Point(1, 2), new Point(2, 3), Color.Empty };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawReversibleLine_TestData))]
    public void ControlPaint_DrawReversibleLine_Invoke_Success(Point start, Point end, Color backColor)
    {
        ControlPaint.DrawReversibleLine(start, end, backColor);

        // Call again to test caching.
        ControlPaint.DrawReversibleLine(start, end, backColor);
    }

    public static IEnumerable<object[]> DrawScrollButton_Graphics_Rectangle_ButtonState_TestData()
    {
        foreach (ScrollButton button in Enum.GetValues(typeof(ScrollButton)))
        {
            foreach (ButtonState state in Enum.GetValues(typeof(ButtonState)))
            {
                yield return new object[] { new Rectangle(0, 0, 3, 4), button, state };
                yield return new object[] { new Rectangle(1, 2, 3, 4), button, state };
                yield return new object[] { new Rectangle(1, 2, 4, 3), button, state };
                yield return new object[] { new Rectangle(1, 2, 3, 3), button, state };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawScrollButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawScrollButton_InvokeGraphicsRectangleButtonState_Success(Rectangle rectangle, ScrollButton button, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawScrollButton(graphics, rectangle, button, state);

        // Call again to test caching.
        ControlPaint.DrawScrollButton(graphics, rectangle, button, state);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawScrollButton_Graphics_Rectangle_ButtonState_TestData))]
    public void ControlPaint_DrawScrollButton_InvokeGraphicsIntIntIntIntButtonState_Success(Rectangle rectangle, ScrollButton button, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawScrollButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, button, state);

        // Call again to test caching.
        ControlPaint.DrawScrollButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, button, state);
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawScrollButton_NullGraphics_ThrowsArgumentNullException(ButtonState state)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawScrollButton(null, new Rectangle(1, 2, 3, 4), ScrollButton.Up, state));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawScrollButton(null, 1, 2, 3, 4, ScrollButton.Up, state));
    }

    [WinFormsTheory]
    [InlineData(0, 4, ButtonState.All)]
    [InlineData(0, 4, ButtonState.Normal)]
    [InlineData(3, 0, ButtonState.All)]
    [InlineData(3, 0, ButtonState.Normal)]
    [InlineData(0, 0, ButtonState.All)]
    public void ControlPaint_DrawScrollButton_EmptyRectangle_ThrowsArgumentException(int width, int height, ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawScrollButton(graphics, new Rectangle(0, 0, width, height), ScrollButton.Up, state));
        Assert.Throws<ArgumentException>(() => ControlPaint.DrawScrollButton(graphics, 0, 0, width, height, ScrollButton.Up, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawScrollButton_NegativeWidth_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawScrollButton(graphics, new Rectangle(0, 0, -3, 4), ScrollButton.Up, state));
        Assert.Throws<ArgumentOutOfRangeException>("width", () => ControlPaint.DrawScrollButton(graphics, 0, 0, -3, 4, ScrollButton.Up, state));
    }

    [WinFormsTheory]
    [InlineData(ButtonState.All)]
    [InlineData(ButtonState.Normal)]
    public void ControlPaint_DrawScrollButton_NegativeHeight_ThrowsArgumentOutOfRangeException(ButtonState state)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawScrollButton(graphics, new Rectangle(0, 0, 3, -4), ScrollButton.Up, state));
        Assert.Throws<ArgumentOutOfRangeException>("height", () => ControlPaint.DrawScrollButton(graphics, 0, 0, 3, -4, ScrollButton.Up, state));
    }

    public static IEnumerable<object[]> DrawSelectionFrame_TestData()
    {
        foreach (bool active in new bool[] { true, false })
        {
            yield return new object[] { active, Rectangle.Empty, Rectangle.Empty, Color.Red };
            yield return new object[] { active, new Rectangle(1, 2, -3, -4), new Rectangle(1, 2, -3, -4), Color.Red };
            yield return new object[] { active, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), Color.Red };
            yield return new object[] { active, new Rectangle(1, 2, 3, 4), new Rectangle(0, 1, 4, 5), Color.Red };
            yield return new object[] { active, new Rectangle(0, 1, 4, 5), new Rectangle(1, 2, 3, 4), Color.Red };
            yield return new object[] { active, new Rectangle(0, 1, 4, 5), new Rectangle(1, 2, 3, 4), Color.Black };
            yield return new object[] { active, new Rectangle(0, 1, 4, 5), new Rectangle(1, 2, 3, 4), Color.White };
            yield return new object[] { active, new Rectangle(0, 1, 4, 5), new Rectangle(1, 2, 3, 4), Color.Transparent };
            yield return new object[] { active, new Rectangle(0, 1, 4, 5), new Rectangle(1, 2, 3, 4), Color.Empty };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawSelectionFrame_TestData))]
    public void ControlPaint_DrawSelectionFrame_Invoke_Success(bool active, Rectangle outsideRect, Rectangle insideRect, Color backColor)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawSelectionFrame(graphics, active, outsideRect, insideRect, backColor);

        // Call again to test caching.
        ControlPaint.DrawSelectionFrame(graphics, active, outsideRect, insideRect, backColor);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void ControlPaint_DrawSelectionFrame_NullGraphics_ThrowsArgumentNullException(bool active)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawSelectionFrame(null, active, new Rectangle(1, 2, 3, 4), new Rectangle(1, 2, 3, 4), Color.Red));
    }

    public static IEnumerable<object[]> DrawSizeGrip_Graphics_Color_Rectangle_TestData()
    {
        yield return new object[] { Color.Red, Rectangle.Empty };
        yield return new object[] { Color.Red, new Rectangle(1, 2, -3, -4) };
        yield return new object[] { Color.Red, new Rectangle(0, 0, 3, 4) };
        yield return new object[] { Color.Red, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { Color.Black, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { Color.White, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { Color.Empty, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { Color.Transparent, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { Color.Red, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { Color.Red, new Rectangle(1, 2, 3, 4) };
        yield return new object[] { Color.Red, new Rectangle(1, 2, 4, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawSizeGrip_Graphics_Color_Rectangle_TestData))]
    public void ControlPaint_DrawSizeGrip_InvokeGraphicsColorRectangle_Success(Color backColor, Rectangle rectangle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawSizeGrip(graphics, backColor, rectangle);

        // Call again to test caching.
        ControlPaint.DrawSizeGrip(graphics, backColor, rectangle);
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawSizeGrip_Graphics_Color_Rectangle_TestData))]
    public void ControlPaint_DrawSizeGrip_InvokeGraphicsColorIntIntIntInt_Success(Color backColor, Rectangle rectangle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawSizeGrip(graphics, backColor, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);

        // Call again to test caching.
        ControlPaint.DrawSizeGrip(graphics, backColor, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    [WinFormsFact]
    public void ControlPaint_DrawSizeGrip_NullGraphics_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawSizeGrip(null, Color.Red, new Rectangle(1, 2, 3, 4)));
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawSizeGrip(null, Color.Red, 1, 2, 3, 4));
    }

    public static IEnumerable<object[]> DrawStringDisabled_Graphics_String_Font_Color_RectangleF_StringFormat_TestData()
    {
        foreach (string s in new string[] { null, string.Empty, "string" })
        {
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, new RectangleF(1, 2, 3, 4), null };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, RectangleF.Empty, new StringFormat() };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, new RectangleF(1, 2, -3, -4), new StringFormat() };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, new RectangleF(1, 2, 3, 4), new StringFormat() };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Black, new RectangleF(1, 2, 3, 4), new StringFormat() };
            yield return new object[] { s, SystemFonts.MenuFont, Color.White, new RectangleF(1, 2, 3, 4), new StringFormat() };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Transparent, new RectangleF(1, 2, 3, 4), new StringFormat() };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Empty, new RectangleF(1, 2, 3, 4), new StringFormat() };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawStringDisabled_Graphics_String_Font_Color_RectangleF_StringFormat_TestData))]
    public void ControlPaint_DrawStringDisabled_InvokeGraphicsStringFontColorRectangleFStringFormat_Success(
        string s, Font font, Color color,
        RectangleF layoutRectangle, StringFormat format)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawStringDisabled(graphics, s, font, color, layoutRectangle, format);

        // Call again to test caching.
        ControlPaint.DrawStringDisabled(graphics, s, font, color, layoutRectangle, format);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public void ControlPaint_DrawStringDisabled_NullFontWithNullOrEmptyS_Nop(string s)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawStringDisabled(graphics, s, null, Color.Red, new RectangleF(1, 2, 3, 4), null);

        // Call again to test caching.
        ControlPaint.DrawStringDisabled(graphics, s, null, Color.Red, new RectangleF(1, 2, 3, 4), null);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ControlPaint_DrawStringDisabled_NullGraphics_ThrowsArgumentNullException(string s)
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawStringDisabled(null, s, SystemFonts.MenuFont, Color.Red, new RectangleF(1, 2, 3, 4), new StringFormat()));
    }

    public static IEnumerable<object[]> DrawStringDisabled_IDeviceContext_String_Font_Color_RectangleF_TextFormatFlags_TestData()
    {
        foreach (string s in new string[] { null, string.Empty, "string" })
        {
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, new Rectangle(1, 2, 3, 4), TextFormatFlags.Default };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, Rectangle.Empty, TextFormatFlags.VerticalCenter };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, new Rectangle(1, 2, -3, -4), TextFormatFlags.VerticalCenter };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Red, new Rectangle(1, 2, 3, 4), TextFormatFlags.VerticalCenter };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Black, new Rectangle(1, 2, 3, 4), TextFormatFlags.VerticalCenter };
            yield return new object[] { s, SystemFonts.MenuFont, Color.White, new Rectangle(1, 2, 3, 4), TextFormatFlags.VerticalCenter };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Transparent, new Rectangle(1, 2, 3, 4), TextFormatFlags.VerticalCenter };
            yield return new object[] { s, SystemFonts.MenuFont, Color.Empty, new Rectangle(1, 2, 3, 4), TextFormatFlags.VerticalCenter };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawStringDisabled_IDeviceContext_String_Font_Color_RectangleF_TextFormatFlags_TestData))]
    public void ControlPaint_DrawStringDisabled_InvokeIDeviceContextStringFontColorRectangleTextFormatFlags_Success(
       string s, Font font, Color color,
       Rectangle layoutRectangle, TextFormatFlags format)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawStringDisabled(graphics, s, font, color, layoutRectangle, format);

        // Call again to test caching.
        ControlPaint.DrawStringDisabled(graphics, s, font, color, layoutRectangle, format);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ControlPaint_DrawStringDisabled_NullDc_ThrowsArgumentNullException(string s)
    {
        Assert.Throws<ArgumentNullException>("dc", () => ControlPaint.DrawStringDisabled(null, s, SystemFonts.MenuFont, Color.Red, new Rectangle(1, 2, 3, 4), TextFormatFlags.Default));
    }

    public static IEnumerable<object[]> DrawVisualStyleBorder_TestData()
    {
        yield return new object[] { Rectangle.Empty };
        yield return new object[] { new Rectangle(1, 2, -3, -4) };
        yield return new object[] { new Rectangle(0, 0, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new Rectangle(1, 2, 4, 3) };
        yield return new object[] { new Rectangle(1, 2, 3, 3) };
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawVisualStyleBorder_TestData))]
    public void ControlPaint_DrawVisualStyleBorder_Invoke_Success(Rectangle rectangle)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ControlPaint.DrawVisualStyleBorder(graphics, rectangle);

        // Call again to test caching.
        ControlPaint.DrawVisualStyleBorder(graphics, rectangle);
    }

    [WinFormsFact]
    public void ControlPaint_DrawVisualStyleBorder_NullGraphics_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("graphics", () => ControlPaint.DrawVisualStyleBorder(null, new Rectangle(1, 2, 3, 4)));
    }

    public static IEnumerable<object[]> FillReversibleRectangle_TestData()
    {
        yield return new object[] { Rectangle.Empty, Color.Red };
        yield return new object[] { new Rectangle(1, 2, -3, -4), Color.Red };
        yield return new object[] { new Rectangle(0, 0, 3, 4), Color.Red };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Red };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Black };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.White };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Empty };
        yield return new object[] { new Rectangle(1, 2, 3, 4), Color.Transparent };
        yield return new object[] { new Rectangle(1, 2, 4, 3), Color.Red };
        yield return new object[] { new Rectangle(1, 2, 3, 3), Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(FillReversibleRectangle_TestData))]
    public void ControlPaint_FillReversibleRectangle_Invoke_Success(Rectangle rectangle, Color backColor)
    {
        ControlPaint.FillReversibleRectangle(rectangle, backColor);

        // Call again to test caching.
        ControlPaint.FillReversibleRectangle(rectangle, backColor);
    }

    public static IEnumerable<object[]> IsDark_TestData()
    {
        yield return new object[] { Color.White, false };
        yield return new object[] { Color.Black, true };
    }

    [WinFormsTheory]
    [MemberData(nameof(IsDark_TestData))]
    public void ControlPaint_IsDark(Color color, bool expected)
    {
        bool result = ControlPaint.IsDark(color);

        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> Light_Color_TestData()
    {
        yield return new object[] { Color.FromArgb(255, 255, 0, 0), Color.FromArgb(255, 255, 64, 64) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), Color.FromArgb(255, 64, 255, 64) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 64, 64, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), Color.FromArgb(255, 255, 255, 64) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), Color.FromArgb(255, 255, 64, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), Color.FromArgb(255, 64, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), Color.FromArgb(255, 0, 191, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), Color.FromArgb(255, 0, 149, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), Color.FromArgb(255, 0, 134, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), Color.FromArgb(255, 0, 130, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 63, 63, 63) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), Color.FromArgb(255, 157, 87, 221) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), Color.FromArgb(255, 255, 213, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, Color.FromArgb(255, 142, 142, 142) };
        yield return new object[] { SystemColors.ControlDark, Color.FromArgb(255, 183, 183, 183) };
        yield return new object[] { SystemColors.Control, Color.FromArgb(255, 241, 241, 241) };
        yield return new object[] { SystemColors.ControlLight, Color.FromArgb(255, 233, 233, 233) };
        yield return new object[] { SystemColors.ControlLightLight, Color.FromArgb(255, 255, 255, 255) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Light_Color_TestData))]
    public void ControlPaint_Light_InvokeColor_ReturnsExpected(Color baseColor, Color expected)
    {
        Assert.Equal(expected, ControlPaint.Light(baseColor));

        // Call again to test caching.
        Assert.Equal(expected, ControlPaint.Light(baseColor));
    }

    public static IEnumerable<object[]> Light_Color_Float_TestData()
    {
        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -1.5f, Color.FromArgb(255, 64, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -1.5f, Color.FromArgb(255, 0, 64, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -1.5f, Color.FromArgb(255, 0, 0, 64) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -1.5f, Color.FromArgb(255, 64, 64, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -1.5f, Color.FromArgb(255, 64, 0, 64) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -1.5f, Color.FromArgb(255, 0, 64, 64) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -1.5f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -1.5f, Color.FromArgb(255, 0, 24, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -1.5f, Color.FromArgb(255, 0, 178, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -1.5f, Color.FromArgb(255, 0, 146, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -1.5f, Color.FromArgb(255, 0, 134, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -1.5f, Color.FromArgb(255, 65, 65, 65) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -1.5f, Color.FromArgb(255, 19, 6, 30) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -1.5f, Color.FromArgb(255, 255, 159, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, -1.5f, Color.FromArgb(255, 249, 249, 249) };
        yield return new object[] { SystemColors.ControlDark, -1.5f, Color.FromArgb(255, 89, 89, 89) };
        yield return new object[] { SystemColors.Control, -1.5f, Color.FromArgb(255, 185, 185, 185) };
        yield return new object[] { SystemColors.ControlLight, -1.5f, Color.FromArgb(255, 207, 207, 207) };
        yield return new object[] { SystemColors.ControlLightLight, -1.5f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -1f, Color.FromArgb(255, 128, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -1f, Color.FromArgb(255, 0, 128, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -1f, Color.FromArgb(255, 0, 0, 128) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -1f, Color.FromArgb(255, 128, 128, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -1f, Color.FromArgb(255, 128, 0, 128) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -1f, Color.FromArgb(255, 0, 128, 128) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -1f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -1f, Color.FromArgb(255, 0, 131, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -1f, Color.FromArgb(255, 0, 43, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -1f, Color.FromArgb(255, 0, 16, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -1f, Color.FromArgb(255, 0, 5, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -1f, Color.FromArgb(255, 129, 129, 129) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -1f, Color.FromArgb(255, 54, 17, 87) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -1f, Color.FromArgb(255, 255, 172, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, -1f, Color.FromArgb(255, 29, 29, 29) };
        yield return new object[] { SystemColors.ControlDark, -1f, Color.FromArgb(255, 112, 112, 112) };
        yield return new object[] { SystemColors.Control, -1f, Color.FromArgb(255, 199, 199, 199) };
        yield return new object[] { SystemColors.ControlLight, -1f, Color.FromArgb(255, 213, 213, 213) };
        yield return new object[] { SystemColors.ControlLightLight, -1f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -0.5f, Color.FromArgb(255, 191, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -0.5f, Color.FromArgb(255, 0, 191, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -0.5f, Color.FromArgb(255, 0, 0, 191) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -0.5f, Color.FromArgb(255, 191, 191, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -0.5f, Color.FromArgb(255, 191, 0, 191) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -0.5f, Color.FromArgb(255, 0, 191, 191) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -0.5f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -0.5f, Color.FromArgb(255, 0, 237, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -0.5f, Color.FromArgb(255, 0, 165, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -0.5f, Color.FromArgb(255, 0, 141, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -0.5f, Color.FromArgb(255, 0, 133, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -0.5f, Color.FromArgb(255, 193, 193, 193) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -0.5f, Color.FromArgb(255, 89, 29, 143) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -0.5f, Color.FromArgb(255, 255, 187, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, -0.5f, Color.FromArgb(255, 68, 68, 68) };
        yield return new object[] { SystemColors.ControlDark, -0.5f, Color.FromArgb(255, 137, 137, 137) };
        yield return new object[] { SystemColors.Control, -0.5f, Color.FromArgb(255, 213, 213, 213) };
        yield return new object[] { SystemColors.ControlLight, -0.5f, Color.FromArgb(255, 221, 221, 221) };
        yield return new object[] { SystemColors.ControlLightLight, -0.5f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), -0.25f, Color.FromArgb(255, 223, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), -0.25f, Color.FromArgb(255, 0, 223, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), -0.25f, Color.FromArgb(255, 0, 0, 223) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), -0.25f, Color.FromArgb(255, 223, 223, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), -0.25f, Color.FromArgb(255, 223, 0, 223) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), -0.25f, Color.FromArgb(255, 0, 223, 223) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), -0.25f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), -0.25f, Color.FromArgb(255, 0, 32, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), -0.25f, Color.FromArgb(255, 0, 226, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), -0.25f, Color.FromArgb(255, 0, 205, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), -0.25f, Color.FromArgb(255, 0, 196, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), -0.25f, Color.FromArgb(255, 225, 225, 225) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), -0.25f, Color.FromArgb(255, 106, 34, 172) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), -0.25f, Color.FromArgb(255, 255, 193, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, -0.25f, Color.FromArgb(255, 87, 87, 87) };
        yield return new object[] { SystemColors.ControlDark, -0.25f, Color.FromArgb(255, 148, 148, 148) };
        yield return new object[] { SystemColors.Control, -0.25f, Color.FromArgb(255, 220, 220, 220) };
        yield return new object[] { SystemColors.ControlLight, -0.25f, Color.FromArgb(255, 224, 224, 224) };
        yield return new object[] { SystemColors.ControlLightLight, -0.25f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 0f, Color.FromArgb(255, 255, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 0f, Color.FromArgb(255, 0, 255, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 0f, Color.FromArgb(255, 0, 0, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 0f, Color.FromArgb(255, 255, 255, 0) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 0f, Color.FromArgb(255, 255, 0, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 0f, Color.FromArgb(255, 0, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 0f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 0f, Color.FromArgb(255, 0, 85, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 0f, Color.FromArgb(255, 0, 28, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 0f, Color.FromArgb(255, 0, 9, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 0f, Color.FromArgb(255, 0, 2, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 0f, Color.FromArgb(255, 0, 0, 0) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 0f, Color.FromArgb(255, 124, 40, 200) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 0f, Color.FromArgb(255, 255, 200, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, 0f, Color.FromArgb(255, 105, 105, 105) };
        yield return new object[] { SystemColors.ControlDark, 0f, Color.FromArgb(255, 160, 160, 160) };
        yield return new object[] { SystemColors.Control, 0f, SystemColors.ControlLight };
        yield return new object[] { SystemColors.ControlLight, 0f, Color.FromArgb(255, 227, 227, 227) };
        yield return new object[] { SystemColors.ControlLightLight, 0f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 0.25f, Color.FromArgb(255, 255, 32, 32) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 0.25f, Color.FromArgb(255, 32, 255, 32) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 0.25f, Color.FromArgb(255, 32, 32, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 0.25f, Color.FromArgb(255, 255, 255, 32) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 0.25f, Color.FromArgb(255, 255, 32, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 0.25f, Color.FromArgb(255, 32, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 0.25f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 0.25f, Color.FromArgb(255, 0, 138, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 0.25f, Color.FromArgb(255, 0, 87, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 0.25f, Color.FromArgb(255, 0, 70, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 0.25f, Color.FromArgb(255, 0, 66, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 0.25f, Color.FromArgb(255, 31, 31, 31) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 0.25f, Color.FromArgb(255, 141, 58, 216) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 0.25f, Color.FromArgb(255, 255, 206, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, 0.25f, Color.FromArgb(255, 123, 123, 123) };
        yield return new object[] { SystemColors.ControlDark, 0.25f, Color.FromArgb(255, 172, 172, 172) };
        yield return new object[] { SystemColors.Control, 0.25f, Color.FromArgb(255, 234, 234, 234) };
        yield return new object[] { SystemColors.ControlLight, 0.25f, Color.FromArgb(255, 230, 230, 230) };
        yield return new object[] { SystemColors.ControlLightLight, 0.25f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 0.5f, Color.FromArgb(255, 255, 64, 64) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 0.5f, Color.FromArgb(255, 64, 255, 64) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 0.5f, Color.FromArgb(255, 64, 64, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 0.5f, Color.FromArgb(255, 255, 255, 64) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 0.5f, Color.FromArgb(255, 255, 64, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 0.5f, Color.FromArgb(255, 64, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 0.5f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 0.5f, Color.FromArgb(255, 0, 191, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 0.5f, Color.FromArgb(255, 0, 149, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 0.5f, Color.FromArgb(255, 0, 134, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 0.5f, Color.FromArgb(255, 0, 130, 0) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 0.5f, Color.FromArgb(255, 63, 63, 63) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 0.5f, Color.FromArgb(255, 157, 87, 221) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 0.5f, Color.FromArgb(255, 255, 213, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, 0.5f, Color.FromArgb(255, 142, 142, 142) };
        yield return new object[] { SystemColors.ControlDark, 0.5f, Color.FromArgb(255, 183, 183, 183) };
        yield return new object[] { SystemColors.Control, 0.5f, Color.FromArgb(255, 241, 241, 241) };
        yield return new object[] { SystemColors.ControlLight, 0.5f, Color.FromArgb(255, 233, 233, 233) };
        yield return new object[] { SystemColors.ControlLightLight, 0.5f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 1f, Color.FromArgb(255, 255, 128, 128) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 1f, Color.FromArgb(255, 128, 255, 128) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 1f, Color.FromArgb(255, 128, 128, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 1f, Color.FromArgb(255, 255, 255, 128) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 1f, Color.FromArgb(255, 255, 128, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 1f, Color.FromArgb(255, 128, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 1f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 1f, Color.FromArgb(255, 43, 255, 43) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 1f, Color.FromArgb(255, 15, 255, 15) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 1f, Color.FromArgb(255, 4, 255, 4) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 1f, Color.FromArgb(255, 2, 255, 2) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 1f, Color.FromArgb(255, 127, 127, 127) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 1f, Color.FromArgb(255, 190, 143, 233) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 1f, Color.FromArgb(255, 255, 227, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, 1f, Color.FromArgb(255, 180, 180, 180) };
        yield return new object[] { SystemColors.ControlDark, 1f, Color.FromArgb(255, 208, 208, 208) };
        yield return new object[] { SystemColors.Control, 1f, SystemColors.ControlLightLight };
        yield return new object[] { SystemColors.ControlLight, 1f, Color.FromArgb(255, 241, 241, 241) };
        yield return new object[] { SystemColors.ControlLightLight, 1f, Color.FromArgb(255, 255, 255, 255) };

        yield return new object[] { Color.FromArgb(255, 255, 0, 0), 1.5f, Color.FromArgb(255, 255, 191, 191) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), 1.5f, Color.FromArgb(255, 191, 255, 191) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), 1.5f, Color.FromArgb(255, 191, 191, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), 1.5f, Color.FromArgb(255, 255, 255, 191) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), 1.5f, Color.FromArgb(255, 255, 191, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), 1.5f, Color.FromArgb(255, 191, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), 1.5f, Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), 1.5f, Color.FromArgb(255, 149, 255, 149) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), 1.5f, Color.FromArgb(255, 136, 255, 136) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), 1.5f, Color.FromArgb(255, 130, 255, 130) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), 1.5f, Color.FromArgb(255, 130, 255, 130) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), 1.5f, Color.FromArgb(255, 191, 191, 191) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), 1.5f, Color.FromArgb(255, 223, 200, 244) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), 1.5f, Color.FromArgb(255, 255, 240, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, 1.5f, Color.FromArgb(255, 217, 217, 217) };
        yield return new object[] { SystemColors.ControlDark, 1.5f, Color.FromArgb(255, 231, 231, 231) };
        yield return new object[] { SystemColors.Control, 1.5f, Color.FromArgb(255, 13, 13, 13) };
        yield return new object[] { SystemColors.ControlLight, 1.5f, Color.FromArgb(255, 247, 247, 247) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Light_Color_Float_TestData))]
    public void ControlPaint_Light_InvokeColorFloat_ReturnsExpected(Color baseColor, float percOfLightLight, Color expected)
    {
        Assert.Equal(expected, ControlPaint.Light(baseColor, percOfLightLight));

        // Call again to test caching.
        Assert.Equal(expected, ControlPaint.Light(baseColor, percOfLightLight));
    }

    public static IEnumerable<object[]> LightLight_TestData()
    {
        yield return new object[] { Color.FromArgb(255, 255, 0, 0), Color.FromArgb(255, 255, 128, 128) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 0), Color.FromArgb(255, 128, 255, 128) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 255), Color.FromArgb(255, 128, 128, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 0), Color.FromArgb(255, 255, 255, 128) };
        yield return new object[] { Color.FromArgb(255, 255, 0, 255), Color.FromArgb(255, 255, 128, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 255, 255), Color.FromArgb(255, 128, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 255, 255, 255), Color.FromArgb(255, 255, 255, 255) };
        yield return new object[] { Color.FromArgb(255, 0, 85, 0), Color.FromArgb(255, 43, 255, 43) };
        yield return new object[] { Color.FromArgb(255, 0, 28, 0), Color.FromArgb(255, 15, 255, 15) };
        yield return new object[] { Color.FromArgb(255, 0, 9, 0), Color.FromArgb(255, 4, 255, 4) };
        yield return new object[] { Color.FromArgb(255, 0, 2, 0), Color.FromArgb(255, 2, 255, 2) };
        yield return new object[] { Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 127, 127, 127) };
        yield return new object[] { Color.FromArgb(255, 125, 40, 200), Color.FromArgb(255, 190, 143, 233) };
        yield return new object[] { Color.FromArgb(0, 255, 200, 255), Color.FromArgb(255, 255, 227, 255) };
        yield return new object[] { SystemColors.ControlDarkDark, Color.FromArgb(255, 180, 180, 180) };
        yield return new object[] { SystemColors.ControlDark, Color.FromArgb(255, 208, 208, 208) };
        yield return new object[] { SystemColors.Control, SystemColors.ControlLightLight };
        yield return new object[] { SystemColors.ControlLight, Color.FromArgb(255, 241, 241, 241) };
        yield return new object[] { SystemColors.ControlLightLight, Color.FromArgb(255, 255, 255, 255) };
    }

    [WinFormsTheory]
    [MemberData(nameof(LightLight_TestData))]
    public void ControlPaint_LightLight_InvokeColor_ReturnsExpected(Color baseColor, Color expected)
    {
        Assert.Equal(expected, ControlPaint.LightLight(baseColor));

        // Call again to test caching.
        Assert.Equal(expected, ControlPaint.LightLight(baseColor));
    }
}
