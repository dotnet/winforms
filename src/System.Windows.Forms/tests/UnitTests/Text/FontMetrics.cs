// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Internal;
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

            using WindowsFont windowsFont = WindowsFont.FromFont(font, Gdi32.QUALITY.CLEARTYPE);
            Assert.Equal(height, windowsFont.Height);
        }

        [Theory]
        [InlineData("Arial", 9.0f, 1 /* DEFAULT_CHARSET */)]
        [InlineData("Arial", 12.0f, 1)]
        [InlineData("Microsoft Sans Serif", 16.0f, 1)]
        [InlineData("Times New Roman", 11.0f, 1)]
        [InlineData("MS Gothic", 11.0f, 1)]
        public void Font_GetCharSet(string family, float size, byte charset)
        {
            using Font font = new Font(family, size);
            if (font.Name != family)
            {
                // Not installed on this machine
                return;
            }

            using WindowsFont windowsFont = WindowsFont.FromFont(font, Gdi32.QUALITY.CLEARTYPE);
            Assert.Equal(charset, windowsFont.CharSet);
        }

        [Theory]
        [InlineData("Arial", 9.0f, 2.5f)]
        [InlineData("Arial", 12.0f, 3.0f)]
        [InlineData("Microsoft Sans Serif", 16.0f, 4.3333335f)]
        [InlineData("Times New Roman", 11.0f, 2.8333333f)]
        [InlineData("MS Gothic", 10.0f, 2.3333333f)]
        public void Font_GetOverhangPadding(string family, float size, float expected)
        {
            using Font font = new Font(family, size);
            if (font.Name != family)
            {
                // Not installed on this machine
                return;
            }

            using WindowsFont windowsFont = WindowsFont.FromFont(font, Gdi32.QUALITY.CLEARTYPE);
            WindowsGraphics graphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            Assert.Equal(expected, graphics.GetOverhangPadding(windowsFont));
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

            using WindowsFont windowsFont = WindowsFont.FromFont(font, Gdi32.QUALITY.CLEARTYPE);
            WindowsGraphics graphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            User32.DRAWTEXTPARAMS margins = graphics.GetTextMargins(windowsFont);
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

            using WindowsFont windowsFont = WindowsFont.FromFont(font, Gdi32.QUALITY.CLEARTYPE);
            WindowsGraphics graphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            Size extent = graphics.GetTextExtent("Whizzo Butter", windowsFont);
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

            using WindowsFont windowsFont = WindowsFont.FromFont(font, Gdi32.QUALITY.CLEARTYPE);
            WindowsGraphics graphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            Size measure = graphics.MeasureText("Windows Foundation Classes", windowsFont, proposedSize, (User32.DT)dt);
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

            using WindowsFont windowsFont = WindowsFont.FromFont(font, Gdi32.QUALITY.CLEARTYPE);
            WindowsGraphics graphics = WindowsGraphicsCacheManager.GetCurrentMeasurementGraphics();
            graphics.DeviceContext.SelectFont(windowsFont);
            User32.DRAWTEXTPARAMS param = default;
            Rectangle result = WindowsGraphics.AdjustForVerticalAlignment(
                graphics,
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
