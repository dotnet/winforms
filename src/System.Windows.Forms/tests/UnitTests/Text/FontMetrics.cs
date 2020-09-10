﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.Text
{
    public class FontMetrics
    {
        [Theory]
        [InlineData("Arial", 9.0f, 15)]
        [InlineData("Arial", 12.0f, 18)]
        [InlineData("Microsoft Sans Serif", 16.0f, 26)]
        [InlineData("Times New Roman", 11.0f, 17)]
        [InlineData("MS Gothic", 10.0f, 14)]
        public void Font_GetHeight(string family, float size, int height)
        {
            using Font font = new Font(family, size);
            if (font.Name != family)
            {
                // Not installed on this machine
                return;
            }

            using var hfont = GdiCache.GetHFONT(font, Gdi32.QUALITY.CLEARTYPE);
            Assert.Equal(height, hfont.Data.Height);
        }

        [Theory]
        [InlineData("Arial", 9.0f, 3, 4)]
        [InlineData("Arial", 12.0f, 3, 5)]
        [InlineData("Microsoft Sans Serif", 16.0f, 5, 7)]
        [InlineData("Times New Roman", 11.0f, 3, 5)]
        [InlineData("MS Gothic", 10.0f, 3, 4)]
        public void Font_GetTextMargins(string family, float size, int left, int right)
        {
            using Font font = new Font(family, size);
            if (font.Name != family)
            {
                // Not installed on this machine
                return;
            }

            using var hfont = GdiCache.GetHFONT(font, Gdi32.QUALITY.CLEARTYPE);
            User32.DRAWTEXTPARAMS margins = hfont.GetTextMargins();
            Assert.Equal(left, margins.iLeftMargin);
            Assert.Equal(right, margins.iRightMargin);
        }

        [Theory]
        [InlineData("Arial", 9.0f, 73, 15)]
        [InlineData("Arial", 12.0f, 95, 18)]
        [InlineData("Microsoft Sans Serif", 16.0f, 136, 26)]
        [InlineData("Times New Roman", 11.0f, 84, 17)]
        [InlineData("MS Gothic", 10.0f, 91, 14)]
        public void Font_GetTextExtent(string family, float size, int width, int height)
        {
            using Font font = new Font(family, size);
            if (font.Name != family)
            {
                // Not installed on this machine
                return;
            }

            using var hfont = GdiCache.GetHFONT(font, Gdi32.QUALITY.CLEARTYPE);
            using var screen = GdiCache.GetScreenHdc();
            Size extent = screen.HDC.GetTextExtent("Whizzo Butter", hfont);
            Assert.Equal(width, extent.Width);
            Assert.Equal(height, extent.Height);
        }

        [Theory]
        [MemberData(nameof(MeasureTextData))]
        public void Font_MeasureText(string family, float size, Size proposedSize, uint dt, Size expected)
        {
            using Font font = new Font(family, size);
            if (font.Name != family)
            {
                // Not installed on this machine
                return;
            }

            using var hfont = GdiCache.GetHFONT(font, Gdi32.QUALITY.CLEARTYPE);
            using var screen = GdiCache.GetScreenHdc();
            Size measure = screen.HDC.MeasureText("Windows Foundation Classes", hfont, proposedSize, (TextFormatFlags)dt);
            Assert.Equal(expected, measure);
        }

        public static TheoryData<string, float, Size, uint, Size> MeasureTextData =>
            new TheoryData<string, float, Size, uint, Size>
            {
                { "Arial", 9.0f, new Size(-1, -1), (uint)User32.DT.BOTTOM, new Size(173, 15) },
                { "Arial", 12.0f, new Size(-1, -1), (uint)User32.DT.BOTTOM, new Size(215, 18) },
                { "Microsoft Sans Serif", 16.0f, new Size(-1, -1), (uint)User32.DT.BOTTOM, new Size(299, 26) },
                { "Times New Roman", 11.0f, new Size(-1, -1), (uint)User32.DT.BOTTOM, new Size(179, 17) },
                { "MS Gothic", 10.0f, new Size(-1, -1), (uint)User32.DT.BOTTOM, new Size(189, 14) },
                { "Arial", 9.0f, new Size(0, 0), (uint)User32.DT.BOTTOM, new Size(173, 15) },
                { "Arial", 12.0f, new Size(0, 0), (uint)User32.DT.BOTTOM, new Size(215, 18) },
                { "Microsoft Sans Serif", 16.0f, new Size(0, 0), (uint)User32.DT.BOTTOM, new Size(299, 26) },
                { "Times New Roman", 11.0f, new Size(0, 0), (uint)User32.DT.BOTTOM, new Size(179, 17) },
                { "MS Gothic", 10.0f, new Size(0, 0), (uint)User32.DT.BOTTOM, new Size(189, 14) },
                { "Arial", 9.0f, new Size(1, 1), (uint)User32.DT.BOTTOM, new Size(173, 15) },
                { "Arial", 12.0f, new Size(1, 1), (uint)User32.DT.BOTTOM, new Size(215, 18) },
                { "Microsoft Sans Serif", 16.0f, new Size(1, 1), (uint)User32.DT.BOTTOM, new Size(299, 26) },
                { "Times New Roman", 11.0f, new Size(1, 1), (uint)User32.DT.BOTTOM, new Size(179, 17) },
                { "MS Gothic", 10.0f, new Size(1, 1), (uint)User32.DT.BOTTOM, new Size(189, 14) },
                { "Arial", 9.0f, new Size(300, 300), (uint)User32.DT.BOTTOM, new Size(173, 15) },
                { "Arial", 12.0f, new Size(300, 300), (uint)User32.DT.BOTTOM, new Size(215, 18) },
                { "Microsoft Sans Serif", 16.0f, new Size(300, 300), (uint)User32.DT.BOTTOM, new Size(299, 26) },
                { "Times New Roman", 11.0f, new Size(300, 300), (uint)User32.DT.BOTTOM, new Size(179, 17) },
                { "MS Gothic", 10.0f, new Size(300, 300), (uint)User32.DT.BOTTOM, new Size(189, 14) },
                { "Arial", 9.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.BOTTOM, new Size(173, 15) },
                { "Arial", 12.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.BOTTOM, new Size(215, 18) },
                { "Microsoft Sans Serif", 16.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.BOTTOM, new Size(299, 26) },
                { "Times New Roman", 11.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.BOTTOM, new Size(179, 17) },
                { "MS Gothic", 10.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.BOTTOM, new Size(189, 14) },
                { "Arial", 9.0f, new Size(1, 1), (uint)User32.DT.SINGLELINE, new Size(173, 15) },
                { "Arial", 12.0f, new Size(1, 1), (uint)User32.DT.SINGLELINE, new Size(215, 18) },
                { "Microsoft Sans Serif", 16.0f, new Size(1, 1), (uint)User32.DT.SINGLELINE, new Size(299, 26) },
                { "Times New Roman", 11.0f, new Size(1, 1), (uint)User32.DT.SINGLELINE, new Size(179, 17) },
                { "MS Gothic", 10.0f, new Size(1, 1), (uint)User32.DT.SINGLELINE, new Size(189, 14) },
                { "Arial", 9.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.SINGLELINE, new Size(173, 15) },
                { "Arial", 12.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.SINGLELINE, new Size(215, 18) },
                { "Microsoft Sans Serif", 16.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.SINGLELINE, new Size(299, 26) },
                { "Times New Roman", 11.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.SINGLELINE, new Size(179, 17) },
                { "MS Gothic", 10.0f, new Size(int.MaxValue, int.MaxValue), (uint)User32.DT.SINGLELINE, new Size(189, 14) },
            };

        [Theory]
        [MemberData(nameof(AdjustData))]
        public void Font_AdjustForVerticalAlignment(string family, float size, Rectangle bounds, uint dt, Rectangle expected)
        {
            using Font font = new Font(family, size);
            if (font.Name != family)
            {
                // Not installed on this machine
                return;
            }

            using var hfont = GdiCache.GetHFONT(font, Gdi32.QUALITY.CLEARTYPE);
            using var screen = GdiCache.GetScreenHdc();
            using var fontSelection = new Gdi32.SelectObjectScope(screen, hfont.Object);

            User32.DRAWTEXTPARAMS param = default;
            Rectangle result = screen.HDC.AdjustForVerticalAlignment(
                "Windows Foundation Classes",
                bounds,
                (User32.DT)dt,
                ref param);
            Assert.Equal(expected, result);
        }

        public static TheoryData<string, float, Rectangle, uint, Rectangle> AdjustData =>
            new TheoryData<string, float, Rectangle, uint, Rectangle>
            {
                { "Arial", 9.0f, new Rectangle(1, 1, 1, 1), (uint)User32.DT.BOTTOM, new Rectangle(1, 1, 1, 1) },
                { "Arial", 12.0f, new Rectangle(1, 1, 1, 1), (uint)User32.DT.BOTTOM, new Rectangle(1, 1, 1, 1) },
                { "Microsoft Sans Serif", 16.0f, new Rectangle(1, 1, 1, 1), (uint)User32.DT.BOTTOM, new Rectangle(1, 1, 1, 1) },
                { "Times New Roman", 11.0f, new Rectangle(1, 1, 1, 1), (uint)User32.DT.BOTTOM, new Rectangle(1, 1, 1, 1) },
                { "MS Gothic", 10.0f, new Rectangle(1, 1, 1, 1), (uint)User32.DT.BOTTOM, new Rectangle(1, 1, 1, 1) },
                { "Arial", 9.0f, new Rectangle(1, 1, 100, 100), (uint)User32.DT.BOTTOM, new Rectangle(1, 86, 100, 100) },
                { "Arial", 12.0f, new Rectangle(1, 1, 100, 100), (uint)User32.DT.BOTTOM, new Rectangle(1, 83, 100, 100) },
                { "Microsoft Sans Serif", 16.0f, new Rectangle(1, 1, 100, 100), (uint)User32.DT.BOTTOM, new Rectangle(1, 75, 100, 100) },
                { "Times New Roman", 11.0f, new Rectangle(1, 1, 100, 100), (uint)User32.DT.BOTTOM, new Rectangle(1, 84, 100, 100) },
                { "MS Gothic", 10.0f, new Rectangle(1, 1, 100, 100), (uint)User32.DT.BOTTOM, new Rectangle(1, 87, 100, 100) },
            };
    }
}
