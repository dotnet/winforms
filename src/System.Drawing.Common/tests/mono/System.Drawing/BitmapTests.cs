// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Bitmap class testing unit
//
// Authors:
//  Jordi Mas i Hernandez (jordi@ximian.com)
//  Jonathan Gilbert <logic@deltaq.org>
//  Sebastien Pouliot  <sebastien@ximian.com>
//
// (C) 2004 Ximian, Inc. http://www.ximian.com
// Copyright (C) 2004,2006-2007 Novell, Inc (http://www.novell.com)
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

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace MonoTests.System.Drawing;

public class BitmapTests
{
    [Fact]
    public void TestPixels()
    {
        // Tests GetSetPixel/SetPixel
        Bitmap bmp = new(100, 100, PixelFormat.Format32bppRgb);
        bmp.SetPixel(0, 0, Color.FromArgb(255, 128, 128, 128));
        Color color = bmp.GetPixel(0, 0);

        Assert.Equal(Color.FromArgb(255, 128, 128, 128), color);

        bmp.SetPixel(99, 99, Color.FromArgb(255, 255, 0, 155));
        Color color2 = bmp.GetPixel(99, 99);
        Assert.Equal(Color.FromArgb(255, 255, 0, 155), color2);
    }

    [Fact]
    public void LockBits_IndexedWrite_NonIndexed()
    {
        using Bitmap bmp = new(100, 100, PixelFormat.Format8bppIndexed);
        Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
        BitmapData bd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        bmp.UnlockBits(bd);
    }

    [Fact]
    public void LockBits_NonIndexedWrite_ToIndexed()
    {
        using Bitmap bmp = new(100, 100, PixelFormat.Format32bppRgb);
        BitmapData bd = new();
        Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
        bd = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed, bd);

        try
        {
            Assert.NotEqual(nint.Zero, bd.Scan0);
        }
        finally
        {
            bmp.UnlockBits(bd);
        }
    }

    [Fact]
    public void LockBits_ImageLockMode_Invalid()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format24bppRgb);
        Rectangle r = new(4, 4, 4, 4);
        BitmapData data = bmp.LockBits(r, 0, PixelFormat.Format24bppRgb);
        try
        {
            Assert.Equal(4, data.Height);
            Assert.Equal(4, data.Width);
            Assert.True(data.Stride >= 12);
            Assert.Equal(PixelFormat.Format24bppRgb, data.PixelFormat);
            Assert.False(nint.Zero.Equals(data.Scan0));
        }
        finally
        {
            bmp.UnlockBits(data);
        }
    }

    [Fact]
    public void LockBits_Double()
    {
        using Bitmap bmp = new(10, 10, PixelFormat.Format24bppRgb);
        Rectangle r = new(4, 4, 4, 4);
        BitmapData data = bmp.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        try
        {
            Assert.Throws<InvalidOperationException>(() => bmp.LockBits(r, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb));
        }
        finally
        {
            bmp.UnlockBits(data);
        }
    }

    [Fact]
    public void Format1bppIndexed()
    {
        using Bitmap bmp = new(1, 1, PixelFormat.Format1bppIndexed);
        Color c = bmp.GetPixel(0, 0);
        Assert.Equal(-16777216, c.ToArgb());
        Assert.Throws<InvalidOperationException>(() => bmp.SetPixel(0, 0, c));
    }

    [Fact]
    public void Format4bppIndexed()
    {
        using Bitmap bmp = new(1, 1, PixelFormat.Format4bppIndexed);
        Color c = bmp.GetPixel(0, 0);
        Assert.Equal(-16777216, c.ToArgb());
        Assert.Throws<InvalidOperationException>(() => bmp.SetPixel(0, 0, c));
    }

    [Fact]
    public void Format8bppIndexed()
    {
        using Bitmap bmp = new(1, 1, PixelFormat.Format8bppIndexed);
        Color c = bmp.GetPixel(0, 0);
        Assert.Equal(-16777216, c.ToArgb());
        Assert.Throws<InvalidOperationException>(() => bmp.SetPixel(0, 0, c));
    }

    private static void FormatTest(PixelFormat format)
    {
        bool alpha = Image.IsAlphaPixelFormat(format);
        int size = Image.GetPixelFormatSize(format) / 8 * 2;
        using Bitmap bmp = new(2, 1, format);
        Color a = Color.FromArgb(128, 64, 32, 16);
        Color b = Color.FromArgb(192, 96, 48, 24);
        bmp.SetPixel(0, 0, a);
        bmp.SetPixel(1, 0, b);
        Color c = bmp.GetPixel(0, 0);
        Color d = bmp.GetPixel(1, 0);
        if (size == 4)
        {
            Assert.Equal(255, c.A);
            Assert.Equal(66, c.R);
            if (format == PixelFormat.Format16bppRgb565)
            {
                Assert.Equal(32, c.G);
            }
            else
            {
                Assert.Equal(33, c.G);
            }

            Assert.Equal(16, c.B);

            Assert.Equal(255, d.A);
            Assert.Equal(99, d.R);
            if (format == PixelFormat.Format16bppRgb565)
            {
                Assert.Equal(48, d.G);
            }
            else
            {
                Assert.Equal(49, d.G);
            }

            Assert.Equal(24, d.B);
        }
        else if (alpha)
        {
            if (format == PixelFormat.Format32bppPArgb)
            {
                Assert.Equal(a.A, c.A);
                // note sure why the -1
                Assert.Equal(a.R - 1, c.R);
                Assert.Equal(a.G - 1, c.G);
                Assert.Equal(a.B - 1, c.B);

                Assert.Equal(b.A, d.A);
                // note sure why the -1
                Assert.Equal(b.R - 1, d.R);
                Assert.Equal(b.G - 1, d.G);
                Assert.Equal(b.B - 1, d.B);
            }
            else
            {
                Assert.Equal(a, c);
                Assert.Equal(b, d);
            }
        }
        else
        {
            Assert.Equal(Color.FromArgb(255, 64, 32, 16), c);
            Assert.Equal(Color.FromArgb(255, 96, 48, 24), d);
        }

        BitmapData bd = bmp.LockBits(new Rectangle(0, 0, 2, 1), ImageLockMode.ReadOnly, format);
        try
        {
            byte[] data = new byte[size];
            Marshal.Copy(bd.Scan0, data, 0, size);
            if (format == PixelFormat.Format32bppPArgb)
            {
                Assert.Equal(Math.Ceiling((float)c.B * c.A / 255), data[0]);
                Assert.Equal(Math.Ceiling((float)c.G * c.A / 255), data[1]);
                Assert.Equal(Math.Ceiling((float)c.R * c.A / 255), data[2]);
                Assert.Equal(c.A, data[3]);
                Assert.Equal(Math.Ceiling((float)d.B * d.A / 255), data[4]);
                Assert.Equal(Math.Ceiling((float)d.G * d.A / 255), data[5]);
                Assert.Equal(Math.Ceiling((float)d.R * d.A / 255), data[6]);
                Assert.Equal(d.A, data[7]);
            }
            else if (size == 4)
            {
                int n = 0;
                switch (format)
                {
                    case PixelFormat.Format16bppRgb565:
                        Assert.Equal(2, data[n++]);
                        Assert.Equal(65, data[n++]);
                        Assert.Equal(131, data[n++]);
                        Assert.Equal(97, data[n++]);
                        break;
                    case PixelFormat.Format16bppArgb1555:
                        Assert.Equal(130, data[n++]);
                        Assert.Equal(160, data[n++]);
                        Assert.Equal(195, data[n++]);
                        Assert.Equal(176, data[n++]);
                        break;
                    case PixelFormat.Format16bppRgb555:
                        Assert.Equal(130, data[n++]);
                        Assert.Equal(32, data[n++]);
                        Assert.Equal(195, data[n++]);
                        Assert.Equal(48, data[n++]);
                        break;
                }
            }
            else
            {
                int n = 0;
                Assert.Equal(c.B, data[n++]);
                Assert.Equal(c.G, data[n++]);
                Assert.Equal(c.R, data[n++]);
                if (format != PixelFormat.Format32bppRgb)
                {
                    if (size % 4 == 0)
                        Assert.Equal(c.A, data[n++]);
                    Assert.Equal(d.B, data[n++]);
                    Assert.Equal(d.G, data[n++]);
                    Assert.Equal(d.R, data[n++]);
                    if (size % 4 == 0)
                        Assert.Equal(d.A, data[n++]);
                }
            }
        }
        finally
        {
            bmp.UnlockBits(bd);
        }
    }

    [Fact]
    public void Format32bppArgb() => FormatTest(PixelFormat.Format32bppArgb);

    [Fact]
    public void Format32bppRgb() => FormatTest(PixelFormat.Format32bppRgb);

    [Fact]
    public void Format24bppRgb() => FormatTest(PixelFormat.Format24bppRgb);

    /* Get the output directory depending on the runtime and location*/
    public static string getOutSubDir()
    {
        string sSub, sRslt;

        sSub = Environment.GetEnvironmentVariable("MSNet") is null ? "mono/" : "MSNet/";

        sRslt = Path.GetFullPath(sSub);

        if (!Directory.Exists(sRslt))
        {
            sRslt = $"Test/System.Drawing/{sSub}";
        }

        if (sRslt.Length > 0)
        {
            if (sRslt[^1] is not '\\' and not '/')
            {
                sRslt += "/";
            }
        }

        return sRslt;
    }

    [Fact]
    public void Clone()
    {
        string sInFile = Helpers.GetTestBitmapPath("almogaver24bits.bmp");
        Rectangle rect = new(0, 0, 50, 50);
        using Bitmap bmp = new(sInFile);
        using Bitmap bmpNew = bmp.Clone(rect, PixelFormat.Format32bppArgb);
        Color colororg0 = bmp.GetPixel(0, 0);
        Color colororg50 = bmp.GetPixel(49, 49);
        Color colornew0 = bmpNew.GetPixel(0, 0);
        Color colornew50 = bmpNew.GetPixel(49, 49);

        Assert.Equal(colororg0, colornew0);
        Assert.Equal(colororg50, colornew50);
    }

    [Fact]
    public void CloneImage()
    {
        string sInFile = Helpers.GetTestBitmapPath("almogaver24bits.bmp");
        using Bitmap bmp = new(sInFile);
        using Bitmap bmpNew = (Bitmap)bmp.Clone();
        Assert.Equal(bmp.Width, bmpNew.Width);
        Assert.Equal(bmp.Height, bmpNew.Height);
        Assert.Equal(bmp.PixelFormat, bmpNew.PixelFormat);
    }

    [Fact]
    public void Frames()
    {
        string sInFile = Helpers.GetTestBitmapPath("almogaver24bits.bmp");
        using Bitmap bmp = new(sInFile);
        int cnt = bmp.GetFrameCount(FrameDimension.Page);
        int active = bmp.SelectActiveFrame(FrameDimension.Page, 0);

        Assert.Equal(1, cnt);
        Assert.Equal(0, active);
    }

    [Fact]
    public void FileDoesNotExists() => Assert.Throws<ArgumentException>(() => new Bitmap("FileDoesNotExists.jpg"));

    private static string ByteArrayToString(byte[] arrInput)
    {
        StringBuilder sOutput = new(arrInput.Length);
        for (int i = 0; i < arrInput.Length; i++)
        {
            sOutput.Append(arrInput[i].ToString("X2"));
        }

        return sOutput.ToString();
    }

    public static string RotateBmp(Bitmap src, RotateFlipType rotate)
    {
        int width = 150, height = 150, index = 0;
        byte[] pixels = new byte[width * height * 3];
        byte[] hash;
        Color clr;

        using Bitmap bmp_rotate = src.Clone(new RectangleF(0, 0, width, height), PixelFormat.Format32bppArgb);
        bmp_rotate.RotateFlip(rotate);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                clr = bmp_rotate.GetPixel(x, y);
                pixels[index++] = clr.R;
                pixels[index++] = clr.G;
                pixels[index++] = clr.B;
            }
        }

        hash = SHA256.HashData(pixels);
        return ByteArrayToString(hash);
    }

    public static string RotateIndexedBmp(Bitmap src, RotateFlipType type)
    {
        int pixels_per_byte = src.PixelFormat switch
        {
            PixelFormat.Format1bppIndexed => 8,
            PixelFormat.Format4bppIndexed => 2,
            PixelFormat.Format8bppIndexed => 1,
            _ => throw new InvalidOperationException($"Cannot pass a bitmap of format {src.PixelFormat} to RotateIndexedBmp"),
        };

        using Bitmap test = src.Clone() as Bitmap;
        test.RotateFlip(type);

        BitmapData data = null;
        byte[] pixel_data;

        try
        {
            data = test.LockBits(new Rectangle(0, 0, test.Width, test.Height), ImageLockMode.ReadOnly, test.PixelFormat);

            int scan_size = (data.Width + pixels_per_byte - 1) / pixels_per_byte;
            pixel_data = new byte[data.Height * scan_size];

            for (int y = 0; y < data.Height; y++)
            {
                nint src_ptr = (nint)(y * data.Stride + data.Scan0.ToInt64());
                int dest_offset = y * scan_size;
                for (int x = 0; x < scan_size; x++)
                    pixel_data[dest_offset + x] = Marshal.ReadByte(src_ptr, x);
            }
        }
        finally
        {
            if (test is not null && data is not null)
            {
                try
                { test.UnlockBits(data); }
                catch { }
            }
        }

        byte[] hash = SHA256.HashData(pixel_data);
        return ByteArrayToString(hash);
    }

    // Rotate bitmap in different ways, and check the result
    // pixels using SHA256
    [Fact]
    public void Rotate()
    {
        string sInFile = Helpers.GetTestBitmapPath("almogaver24bits.bmp");
        using Bitmap bmp = new(sInFile);
        Assert.Equal("7256C44FB1450981DADD4CA7D0B8E94E8EC8A76D1D6F1839A8B5DB237DDD852D", RotateBmp(bmp, RotateFlipType.Rotate90FlipNone));
        Assert.Equal("D90EDBC221EF524E98D23B8E2B32AECBCEF14FCE5E402E602AFCCBAF0DC581B1", RotateBmp(bmp, RotateFlipType.Rotate180FlipNone));
        Assert.Equal("9110140D17122EC778F5861ED186C28E839FA218955BFA088A5801DDFDF420DE", RotateBmp(bmp, RotateFlipType.Rotate270FlipNone));
        Assert.Equal("29B195F4387B399930BAA50399B0F98EEA8115147FF47A4088E9DA7D29B48DF1", RotateBmp(bmp, RotateFlipType.RotateNoneFlipX));
        Assert.Equal("27D1EA173316A50C9F4186BBA03F4EFE78C1D5FEBC4D973EB9ED87620930AB89", RotateBmp(bmp, RotateFlipType.Rotate90FlipX));
        Assert.Equal("1687AFD5202BCF470B91AFA2E6A43793FA316679B0CE4EB1BC956B230A063A5C", RotateBmp(bmp, RotateFlipType.Rotate180FlipX));
        Assert.Equal("297D4E905D773277CEA86276B15AC70EB02BE4B7FE06120330DABBE92D1DF4E2", RotateBmp(bmp, RotateFlipType.Rotate270FlipX));
    }

    private static Bitmap CreateBitmap(int width, int height, PixelFormat fmt)
    {
        Bitmap bmp = new(width, height, fmt);
        using (Graphics gr = Graphics.FromImage(bmp))
        {
            Color c = Color.FromArgb(255, 100, 200, 250);
            for (int x = 1; x < 80; x++)
            {
                bmp.SetPixel(x, 1, c);
                bmp.SetPixel(x, 2, c);
                bmp.SetPixel(x, 78, c);
                bmp.SetPixel(x, 79, c);
            }

            for (int y = 3; y < 78; y++)
            {
                bmp.SetPixel(1, y, c);
                bmp.SetPixel(2, y, c);
                bmp.SetPixel(78, y, c);
                bmp.SetPixel(79, y, c);
            }
        }

        return bmp;
    }

    private static byte[] HashPixels(Bitmap bmp)
    {
        int len = bmp.Width * bmp.Height * 4;
        int index = 0;
        byte[] pixels = new byte[len];

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color clr = bmp.GetPixel(x, y);
                pixels[index++] = clr.R;
                pixels[index++] = clr.G;
                pixels[index++] = clr.B;
            }
        }

        return SHA256.HashData(pixels);
    }

    private static byte[] HashLock(Bitmap bmp, int width, int height, PixelFormat fmt, ImageLockMode mode)
    {
        int len = bmp.Width * bmp.Height * 4;
        byte[] pixels = new byte[len];
        BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), mode, fmt);
        try
        {
            int index = 0;
            int bbps = Image.GetPixelFormatSize(fmt);
            long pos = bd.Scan0.ToInt64();
            byte[] btv = new byte[1];
            for (int y = 0; y < bd.Height; y++)
            {
                for (int x = 0; x < bd.Width; x++)
                {
                    // Read the pixels
                    for (int bt = 0; bt < bbps / 8; bt++, index++)
                    {
                        long cur = pos;
                        cur += y * bd.Stride;
                        cur += x * bbps / 8;
                        cur += bt;
                        Marshal.Copy((nint)cur, btv, 0, 1);
                        pixels[index] = btv[0];

                        // Make change of all the colors = 250 to 10
                        if (btv[0] == 250)
                        {
                            btv[0] = 10;
                            Marshal.Copy(btv, 0, (nint)cur, 1);
                        }
                    }
                }
            }

            for (int i = index; i < len; i++)
                pixels[index] = 0;
        }
        finally
        {
            bmp.UnlockBits(bd);
        }

        return SHA256.HashData(pixels);
    }

    // Tests the LockBitmap functions. Makes a hash of the block of pixels that it returns
    // firsts, changes them, and then using GetPixel does another check of the changes.
    // The results match the .NET Framework
    private static readonly byte[] s_defaultBitmapHash = [0x29, 0x39, 0x25, 0xCE, 0x10, 0x9C, 0x6A, 0xB, 0x2D, 0xCD, 0xAA, 0xD9,
        0x18, 0xBE, 0xF3, 0xA5, 0x58, 0xA5, 0x4D, 0xD9, 0xFA, 0x41, 0x70, 0x76, 0x83, 0x3, 0x9C, 0x41, 0x5A, 0x25, 0xCE, 0xCB];
    private static readonly byte[] s_finalWholeBitmapHash = [0xDE, 0xBF, 0xCF, 0xFB, 0xE, 0x9E, 0xA7, 0xC1, 0x23, 0xC, 0x9E, 0x7E,
        0xE3, 0xC1, 0xFC, 0x14, 0x37, 0x21, 0xE2, 0x30, 0x2A, 0x6D, 0xD0, 0xDB, 0xBE, 0xE, 0x1C, 0x1F, 0xC2, 0xB7, 0xBD, 0xC4];

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotArm64Process))] // [ActiveIssue("https://github.com/dotnet/winforms/issues/8817")]
    public void LockBitmap_Format32bppArgb_Format32bppArgb_ReadWrite_Whole()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));

        byte[] expected = [0x26, 0x6, 0x8E, 0x48, 0xE8, 0xD2, 0x7A, 0x5, 0x6E, 0x35, 0xAC, 0x55, 0xA1, 0x3A, 0xBB,
            0x37, 0x72, 0xFB, 0x66, 0x21, 0xB5, 0x3D, 0x32, 0x6A, 0xD9, 0x54, 0x53, 0xD, 0x1A, 0x3D, 0x67, 0x3D];
        byte[] actual = HashLock(bmp, bmp.Width, bmp.Height, PixelFormat.Format32bppArgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalWholeBitmapHash, HashPixels(bmp));
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotArm64Process))] // [ActiveIssue("https://github.com/dotnet/winforms/issues/8817")]
    public void LockBitmap_Format32bppArgb_Format32bppPArgb_ReadWrite_Whole()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));
        byte[] expected = [0x26, 0x6, 0x8E, 0x48, 0xE8, 0xD2, 0x7A, 0x5, 0x6E, 0x35, 0xAC, 0x55, 0xA1, 0x3A, 0xBB,
            0x37, 0x72, 0xFB, 0x66, 0x21, 0xB5, 0x3D, 0x32, 0x6A, 0xD9, 0x54, 0x53, 0xD, 0x1A, 0x3D, 0x67, 0x3D];
        byte[] actual = HashLock(bmp, bmp.Width, bmp.Height, PixelFormat.Format32bppPArgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalWholeBitmapHash, HashPixels(bmp));
    }

    [Fact]
    public void LockBitmap_Format32bppArgb_Format32bppRgb_ReadWrite_Whole()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));
        byte[] expected = [0x26, 0x6, 0x8E, 0x48, 0xE8, 0xD2, 0x7A, 0x5, 0x6E, 0x35, 0xAC, 0x55, 0xA1, 0x3A, 0xBB, 0x37, 0x72, 0xFB, 0x66,
            0x21, 0xB5, 0x3D, 0x32, 0x6A, 0xD9, 0x54, 0x53, 0xD, 0x1A, 0x3D, 0x67, 0x3D];
        byte[] actual = HashLock(bmp, bmp.Width, bmp.Height, PixelFormat.Format32bppRgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalWholeBitmapHash, HashPixels(bmp));
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotArm64Process))] // [ActiveIssue("https://github.com/dotnet/winforms/issues/8817")]
    public void LockBitmap_Format32bppArgb_Format24bppRgb_ReadWrite_Whole()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));
        byte[] expected = [0xC5, 0x2D, 0xED, 0x16, 0xEE, 0xD3, 0x24, 0x31, 0x92, 0x86, 0xE6, 0x7F, 0x75, 0xAD, 0xFE, 0x86, 0x94, 0x28, 0xFC,
            0x57, 0x91, 0x71, 0xB5, 0x10, 0x3D, 0x4, 0x7E, 0xBD, 0x7E, 0xFD, 0x9D, 0x92];
        byte[] actual = HashLock(bmp, bmp.Width, bmp.Height, PixelFormat.Format24bppRgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalWholeBitmapHash, HashPixels(bmp));
    }

    private static readonly byte[] s_finalPartialBitmapHash = [0xBC, 0x10, 0x54, 0x9D, 0xE8, 0xCB, 0x98, 0x82, 0x14, 0xE6, 0x38, 0xC1, 0xA5, 0x8E, 0x20, 0x4F,
        0xEA, 0x25, 0x36, 0x44, 0xE5, 0x4E, 0x33, 0x61, 0xD1, 0x79, 0x41, 0x16, 0xCE, 0x62, 0x4D, 0x9D];

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotArm64Process))] // [ActiveIssue("https://github.com/dotnet/winforms/issues/8817")]
    public void LockBitmap_Format32bppArgb_Format32bppArgb_ReadWrite_Partial()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));
        byte[] expected = [0x53, 0xC6, 0x10, 0x31, 0xB6, 0x7C, 0x64, 0x33, 0x28, 0x32, 0x52, 0xC7, 0xE0, 0xA8, 0xA4, 0xF6, 0x49,
            0xBB, 0xA0, 0x1A, 0x36, 0xBD, 0x20, 0x4C, 0x9A, 0xB4, 0x82, 0xE1, 0x4A, 0xA5, 0x6, 0x84];
        byte[] actual = HashLock(bmp, 50, 50, PixelFormat.Format32bppArgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalPartialBitmapHash, HashPixels(bmp));
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotArm64Process))] // [ActiveIssue("https://github.com/dotnet/winforms/issues/8817")]
    public void LockBitmap_Format32bppArgb_Format32bppPArgb_ReadWrite_Partial()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));
        byte[] expected = [0x53, 0xC6, 0x10, 0x31, 0xB6, 0x7C, 0x64, 0x33, 0x28, 0x32, 0x52, 0xC7, 0xE0, 0xA8, 0xA4, 0xF6, 0x49,
            0xBB, 0xA0, 0x1A, 0x36, 0xBD, 0x20, 0x4C, 0x9A, 0xB4, 0x82, 0xE1, 0x4A, 0xA5, 0x6, 0x84];
        byte[] actual = HashLock(bmp, 50, 50, PixelFormat.Format32bppPArgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalPartialBitmapHash, HashPixels(bmp));
    }

    [Fact]
    public void LockBitmap_Format32bppArgb_Format32bppRgb_ReadWrite_Partial()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));
        byte[] expected = [0x53, 0xC6, 0x10, 0x31, 0xB6, 0x7C, 0x64, 0x33, 0x28, 0x32, 0x52, 0xC7, 0xE0, 0xA8, 0xA4, 0xF6, 0x49,
            0xBB, 0xA0, 0x1A, 0x36, 0xBD, 0x20, 0x4C, 0x9A, 0xB4, 0x82, 0xE1, 0x4A, 0xA5, 0x6, 0x84];
        byte[] actual = HashLock(bmp, 50, 50, PixelFormat.Format32bppRgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalPartialBitmapHash, HashPixels(bmp));
    }

    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotArm64Process))] // [ActiveIssue("https://github.com/dotnet/winforms/issues/8817")]
    public void LockBitmap_Format32bppArgb_Format24bppRgb_ReadWrite_Partial()
    {
        using Bitmap bmp = CreateBitmap(100, 100, PixelFormat.Format32bppArgb);
        Assert.Equal(s_defaultBitmapHash, HashPixels(bmp));
        byte[] expected = [0x3, 0xCD, 0x13, 0x64, 0x66, 0xA3, 0xA, 0x66, 0xD3, 0xF8, 0xD8, 0xE1, 0xF4, 0xF7, 0xDE, 0xE9, 0x96, 0xDC,
            0xA6, 0x2, 0x6, 0xF3, 0xEA, 0xB5, 0xC9, 0x72, 0xD9, 0x48, 0x9A, 0x7F, 0xD6, 0x7B];
        byte[] actual = HashLock(bmp, 50, 50, PixelFormat.Format24bppRgb, ImageLockMode.ReadWrite);

        Assert.Equal(expected, actual);
        Assert.Equal(s_finalPartialBitmapHash, HashPixels(bmp));
    }

    // Tests the LockBitmap and UnlockBitmap functions, specifically the copying
    // of bitmap data in the directions indicated by the ImageLockMode.
    [ConditionalFact(typeof(PlatformDetection), nameof(PlatformDetection.IsNotArm64Process))] // [ActiveIssue("https://github.com/dotnet/winforms/issues/8817")]
    public void LockUnlockBitmap()
    {
        BitmapData data;
        int pixel_value;
        Color pixel_colour;

        Color red = Color.FromArgb(Color.Red.A, Color.Red.R, Color.Red.G, Color.Red.B);
        Color blue = Color.FromArgb(Color.Blue.A, Color.Blue.R, Color.Blue.G, Color.Blue.B);

        using (Bitmap bmp = new(1, 1, PixelFormat.Format32bppRgb))
        {
            bmp.SetPixel(0, 0, red);
            pixel_colour = bmp.GetPixel(0, 0);
            Assert.Equal(red, pixel_colour);

            data = bmp.LockBits(new Rectangle(0, 0, 1, 1), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                pixel_value = Marshal.ReadByte(data.Scan0, 0);
                pixel_value |= Marshal.ReadByte(data.Scan0, 1) << 8;
                pixel_value |= Marshal.ReadByte(data.Scan0, 2) << 16;
                pixel_value |= Marshal.ReadByte(data.Scan0, 3) << 24;

                pixel_colour = Color.FromArgb(pixel_value);
                // Disregard alpha information in the test
                pixel_colour = Color.FromArgb(red.A, pixel_colour.R, pixel_colour.G, pixel_colour.B);
                Assert.Equal(red, pixel_colour);

                // write blue but we're locked in read-only...
                Marshal.WriteByte(data.Scan0, 0, blue.B);
                Marshal.WriteByte(data.Scan0, 1, blue.G);
                Marshal.WriteByte(data.Scan0, 2, blue.R);
                Marshal.WriteByte(data.Scan0, 3, blue.A);
            }
            finally
            {
                bmp.UnlockBits(data);
                pixel_colour = bmp.GetPixel(0, 0);
                // Disregard alpha information in the test
                pixel_colour = Color.FromArgb(red.A, pixel_colour.R, pixel_colour.G, pixel_colour.B);
                // ...so we still read red after unlocking
                Assert.Equal(red, pixel_colour);
            }

            data = bmp.LockBits(new Rectangle(0, 0, 1, 1), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                // write blue
                Marshal.WriteByte(data.Scan0, 0, blue.B);
                Marshal.WriteByte(data.Scan0, 1, blue.G);
                Marshal.WriteByte(data.Scan0, 2, blue.R);
                Marshal.WriteByte(data.Scan0, 3, blue.A);
            }
            finally
            {
                bmp.UnlockBits(data);
                pixel_colour = bmp.GetPixel(0, 0);
                // Disregard alpha information in the test
                pixel_colour = Color.FromArgb(blue.A, pixel_colour.R, pixel_colour.G, pixel_colour.B);
                // read blue
                Assert.Equal(blue, pixel_colour);
            }
        }

        using (Bitmap bmp = new(1, 1, PixelFormat.Format32bppArgb))
        {
            bmp.SetPixel(0, 0, red);

            data = bmp.LockBits(new Rectangle(0, 0, 1, 1), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                byte b = Marshal.ReadByte(data.Scan0, 0);
                byte g = Marshal.ReadByte(data.Scan0, 1);
                byte r = Marshal.ReadByte(data.Scan0, 2);
                pixel_colour = Color.FromArgb(red.A, r, g, b);
                Assert.Equal(red, pixel_colour);
                // write blue but we're locked in read-only...
                Marshal.WriteByte(data.Scan0, 0, blue.B);
                Marshal.WriteByte(data.Scan0, 1, blue.G);
                Marshal.WriteByte(data.Scan0, 2, blue.R);
            }
            finally
            {
                bmp.UnlockBits(data);
                // ...so we still read red after unlocking
                Assert.Equal(red, bmp.GetPixel(0, 0));
            }

            data = bmp.LockBits(new Rectangle(0, 0, 1, 1), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            try
            {
                // write blue
                Marshal.WriteByte(data.Scan0, 0, blue.B);
                Marshal.WriteByte(data.Scan0, 1, blue.G);
                Marshal.WriteByte(data.Scan0, 2, blue.R);
            }
            finally
            {
                bmp.UnlockBits(data);
                // read blue
                Assert.Equal(blue, bmp.GetPixel(0, 0));
            }
        }
    }

    [Fact]
    public void DefaultFormat1()
    {
        using Bitmap bmp = new(20, 20);
        Assert.Equal(ImageFormat.MemoryBmp, bmp.RawFormat);
    }

    [Fact]
    public void DefaultFormat2()
    {
        string filename = Path.GetTempFileName();
        using (Bitmap bmp = new(20, 20))
        {
            bmp.Save(filename);
        }

        using (Bitmap other = new(filename))
        {
            Assert.Equal(ImageFormat.Png, other.RawFormat);
        }

        File.Delete(filename);
    }

    [Fact]
    public void BmpDataStride1()
    {
        Bitmap bmp = new(184, 184, PixelFormat.Format1bppIndexed);
        BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
        try
        {
            Assert.Equal(24, data.Stride);
        }
        finally
        {
            bmp.UnlockBits(data);
            bmp.Dispose();
        }
    }

    private static readonly int[] s_palette1 =
    [
        -16777216,
        -1,
    ];

    [Fact]
    public void Format1bppIndexed_Palette()
    {
        using Bitmap bmp = new(1, 1, PixelFormat.Format1bppIndexed);
        ColorPalette pal = bmp.Palette;
        Assert.Equal(2, pal.Entries.Length);
        for (int i = 0; i < pal.Entries.Length; i++)
        {
            Assert.Equal(s_palette1[i], pal.Entries[i].ToArgb());
        }

        Assert.Equal(2, pal.Flags);
    }

    private static readonly int[] s_palette16 =
    [
        -16777216,
        -8388608,
        -16744448,
        -8355840,
        -16777088,
        -8388480,
        -16744320,
        -8355712,
        -4144960,
        -65536,
        -16711936,
        -256,
        -16776961,
        -65281,
        -16711681,
        -1,
    ];

    [Fact]
    public void Format4bppIndexed_Palette()
    {
        using Bitmap bmp = new(1, 1, PixelFormat.Format4bppIndexed);
        ColorPalette pal = bmp.Palette;
        Assert.Equal(16, pal.Entries.Length);
        for (int i = 0; i < pal.Entries.Length; i++)
        {
            Assert.Equal(s_palette16[i], pal.Entries[i].ToArgb());
        }

        Assert.Equal(0, pal.Flags);
    }

    private static readonly int[] s_palette256 =
    [
        -16777216,
        -8388608,
        -16744448,
        -8355840,
        -16777088,
        -8388480,
        -16744320,
        -8355712,
        -4144960,
        -65536,
        -16711936,
        -256,
        -16776961,
        -65281,
        -16711681,
        -1,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        -16777216,
        -16777165,
        -16777114,
        -16777063,
        -16777012,
        -16776961,
        -16764160,
        -16764109,
        -16764058,
        -16764007,
        -16763956,
        -16763905,
        -16751104,
        -16751053,
        -16751002,
        -16750951,
        -16750900,
        -16750849,
        -16738048,
        -16737997,
        -16737946,
        -16737895,
        -16737844,
        -16737793,
        -16724992,
        -16724941,
        -16724890,
        -16724839,
        -16724788,
        -16724737,
        -16711936,
        -16711885,
        -16711834,
        -16711783,
        -16711732,
        -16711681,
        -13434880,
        -13434829,
        -13434778,
        -13434727,
        -13434676,
        -13434625,
        -13421824,
        -13421773,
        -13421722,
        -13421671,
        -13421620,
        -13421569,
        -13408768,
        -13408717,
        -13408666,
        -13408615,
        -13408564,
        -13408513,
        -13395712,
        -13395661,
        -13395610,
        -13395559,
        -13395508,
        -13395457,
        -13382656,
        -13382605,
        -13382554,
        -13382503,
        -13382452,
        -13382401,
        -13369600,
        -13369549,
        -13369498,
        -13369447,
        -13369396,
        -13369345,
        -10092544,
        -10092493,
        -10092442,
        -10092391,
        -10092340,
        -10092289,
        -10079488,
        -10079437,
        -10079386,
        -10079335,
        -10079284,
        -10079233,
        -10066432,
        -10066381,
        -10066330,
        -10066279,
        -10066228,
        -10066177,
        -10053376,
        -10053325,
        -10053274,
        -10053223,
        -10053172,
        -10053121,
        -10040320,
        -10040269,
        -10040218,
        -10040167,
        -10040116,
        -10040065,
        -10027264,
        -10027213,
        -10027162,
        -10027111,
        -10027060,
        -10027009,
        -6750208,
        -6750157,
        -6750106,
        -6750055,
        -6750004,
        -6749953,
        -6737152,
        -6737101,
        -6737050,
        -6736999,
        -6736948,
        -6736897,
        -6724096,
        -6724045,
        -6723994,
        -6723943,
        -6723892,
        -6723841,
        -6711040,
        -6710989,
        -6710938,
        -6710887,
        -6710836,
        -6710785,
        -6697984,
        -6697933,
        -6697882,
        -6697831,
        -6697780,
        -6697729,
        -6684928,
        -6684877,
        -6684826,
        -6684775,
        -6684724,
        -6684673,
        -3407872,
        -3407821,
        -3407770,
        -3407719,
        -3407668,
        -3407617,
        -3394816,
        -3394765,
        -3394714,
        -3394663,
        -3394612,
        -3394561,
        -3381760,
        -3381709,
        -3381658,
        -3381607,
        -3381556,
        -3381505,
        -3368704,
        -3368653,
        -3368602,
        -3368551,
        -3368500,
        -3368449,
        -3355648,
        -3355597,
        -3355546,
        -3355495,
        -3355444,
        -3355393,
        -3342592,
        -3342541,
        -3342490,
        -3342439,
        -3342388,
        -3342337,
        -65536,
        -65485,
        -65434,
        -65383,
        -65332,
        -65281,
        -52480,
        -52429,
        -52378,
        -52327,
        -52276,
        -52225,
        -39424,
        -39373,
        -39322,
        -39271,
        -39220,
        -39169,
        -26368,
        -26317,
        -26266,
        -26215,
        -26164,
        -26113,
        -13312,
        -13261,
        -13210,
        -13159,
        -13108,
        -13057,
        -256,
        -205,
        -154,
        -103,
        -52,
        -1,
    ];

    [Fact]
    public void Format8bppIndexed_Palette()
    {
        using Bitmap bmp = new(1, 1, PixelFormat.Format8bppIndexed);
        ColorPalette pal = bmp.Palette;
        Assert.Equal(256, pal.Entries.Length);
        for (int i = 0; i < pal.Entries.Length; i++)
        {
            Assert.Equal(s_palette256[i], pal.Entries[i].ToArgb());
        }

        Assert.Equal(4, pal.Flags);
    }

    [Fact]
    public void XmlSerialization() => new XmlSerializer(typeof(Bitmap));

    private static void SetResolution(float x, float y)
    {
        using Bitmap bmp = new(1, 1);
        bmp.SetResolution(x, y);
    }

    [Fact]
    public void SetResolution_Zero() => Assert.Throws<ArgumentException>(() => SetResolution(0.0f, 0.0f));

    [Fact]
    public void SetResolution_Negative_X() => Assert.Throws<ArgumentException>(() => SetResolution(-1.0f, 1.0f));

    [Fact]
    public void SetResolution_Negative_Y() => Assert.Throws<ArgumentException>(() => SetResolution(1.0f, -1.0f));

    [Fact]
    public void SetResolution_MaxValue() => SetResolution(float.MaxValue, float.MaxValue);

    [Fact]
    public void SetResolution_PositiveInfinity() => SetResolution(float.PositiveInfinity, float.PositiveInfinity);

    [Fact]
    public void SetResolution_NaN() => Assert.Throws<ArgumentException>(() => SetResolution(float.NaN, float.NaN));

    [Fact]
    public void SetResolution_NegativeInfinity() => Assert.Throws<ArgumentException>(() => SetResolution(float.NegativeInfinity, float.NegativeInfinity));

    [Fact]
    public void BitmapIntIntIntPixelFormatIntPtrCtor() => new Bitmap(1, 1, 1, PixelFormat.Format1bppIndexed, nint.Zero);

    // BitmapFromHicon## is *almost* the same as IconTest.Icon##ToBitmap except
    // for the Flags property

    private static void HiconTest(string msg, Bitmap b, int size)
    {
        b.PixelFormat.Should().Be(PixelFormat.Format32bppArgb, msg);

        // Unlike the GDI+ icon decoder the palette isn't kept
        Assert.Empty(b.Palette.Entries);
        Assert.Equal(size, b.Height);
        Assert.Equal(size, b.Width);
        Assert.Equal(b.RawFormat, ImageFormat.MemoryBmp);
        Assert.Equal(335888, b.Flags);
    }

    [Fact]
    public void Hicon16()
    {
        nint hicon;
        int size;
        using (Icon icon = new(Helpers.GetTestBitmapPath("16x16_one_entry_4bit.ico")))
        {
            size = icon.Width;
            using Bitmap bitmap = Bitmap.FromHicon(icon.Handle);
            HiconTest("Icon.Handle/FromHicon", bitmap, size);
            hicon = bitmap.GetHicon();
        }

        using Bitmap bitmap2 = Bitmap.FromHicon(hicon);
        // hicon survives bitmap and icon disposal
        HiconTest("GetHicon/FromHicon", bitmap2, size);
    }

    [Fact]
    public void Hicon32()
    {
        nint hicon;
        int size;
        using (Icon icon = new(Helpers.GetTestBitmapPath("32x32_one_entry_4bit.ico")))
        {
            size = icon.Width;
            using Bitmap bitmap = Bitmap.FromHicon(icon.Handle);
            HiconTest("Icon.Handle/FromHicon", bitmap, size);
            hicon = bitmap.GetHicon();
        }

        using Bitmap bitmap2 = Bitmap.FromHicon(hicon);
        // hicon survives bitmap and icon disposal
        HiconTest("GetHicon/FromHicon", bitmap2, size);
    }

    [Fact]
    public void Hicon64()
    {
        nint hicon;
        int size;
        using (Icon icon = new(Helpers.GetTestBitmapPath("64x64_one_entry_8bit.ico")))
        {
            size = icon.Width;
            using Bitmap bitmap = Bitmap.FromHicon(icon.Handle);
            HiconTest("Icon.Handle/FromHicon", bitmap, size);
            hicon = bitmap.GetHicon();
        }

        using Bitmap bitmap2 = Bitmap.FromHicon(hicon);
        // hicon survives bitmap and icon disposal
        HiconTest("GetHicon/FromHicon", bitmap2, size);
    }

    [Fact]
    public void Hicon96()
    {
        nint hicon;
        int size;
        using (Icon icon = new(Helpers.GetTestBitmapPath("96x96_one_entry_8bit.ico")))
        {
            size = icon.Width;
            using Bitmap bitmap = Bitmap.FromHicon(icon.Handle);
            HiconTest("Icon.Handle/FromHicon", bitmap, size);
            hicon = bitmap.GetHicon();
        }

        using Bitmap bitmap2 = Bitmap.FromHicon(hicon);
        // hicon survives bitmap and icon disposal
        HiconTest("GetHicon/FromHicon", bitmap2, size);
    }

    [Fact]
    public void HBitmap()
    {
        nint hbitmap;
        string sInFile = Helpers.GetTestBitmapPath("almogaver24bits.bmp");
        using (Bitmap bitmap = new(sInFile))
        {
            Assert.Equal(PixelFormat.Format24bppRgb, bitmap.PixelFormat);
            Assert.Empty(bitmap.Palette.Entries);
            Assert.Equal(183, bitmap.Height);
            Assert.Equal(173, bitmap.Width);
            Assert.Equal(73744, bitmap.Flags);
            Assert.Equal(bitmap.RawFormat, ImageFormat.Bmp);
            hbitmap = bitmap.GetHbitmap();
        }

        // hbitmap survives original bitmap disposal
        using Image image = Image.FromHbitmap(hbitmap);
        // Assert.Equal (PixelFormat.Format32bppRgb, image.PixelFormat);
        Assert.Empty(image.Palette.Entries);
        Assert.Equal(183, image.Height);
        Assert.Equal(173, image.Width);
        Assert.Equal(335888, image.Flags);
        Assert.Equal(image.RawFormat, ImageFormat.MemoryBmp);
    }

    [Fact]
    public void CreateMultipleBitmapFromSameHBITMAP()
    {
        nint hbitmap;
        string sInFile = Helpers.GetTestBitmapPath("almogaver24bits.bmp");
        using (Bitmap bitmap = new(sInFile))
        {
            Assert.Equal(PixelFormat.Format24bppRgb, bitmap.PixelFormat);
            Assert.Empty(bitmap.Palette.Entries);
            Assert.Equal(183, bitmap.Height);
            Assert.Equal(173, bitmap.Width);
            Assert.Equal(73744, bitmap.Flags);
            Assert.Equal(bitmap.RawFormat, ImageFormat.Bmp);
            hbitmap = bitmap.GetHbitmap();
        }

        // hbitmap survives original bitmap disposal
        using (Image image = Image.FromHbitmap(hbitmap))
        {
            // Assert.Equal (PixelFormat.Format32bppRgb, image.PixelFormat);
            Assert.Empty(image.Palette.Entries);
            Assert.Equal(183, image.Height);
            Assert.Equal(173, image.Width);
            Assert.Equal(335888, image.Flags);
            Assert.Equal(image.RawFormat, ImageFormat.MemoryBmp);
        }

        using Image image2 = Image.FromHbitmap(hbitmap);
        // Assert.Equal (PixelFormat.Format32bppRgb, image2.PixelFormat);
        Assert.Empty(image2.Palette.Entries);
        Assert.Equal(183, image2.Height);
        Assert.Equal(173, image2.Width);
        Assert.Equal(335888, image2.Flags);
        Assert.Equal(image2.RawFormat, ImageFormat.MemoryBmp);
    }
}
