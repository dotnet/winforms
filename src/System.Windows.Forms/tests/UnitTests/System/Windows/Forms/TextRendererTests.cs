﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TextRendererTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Point_Color_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Point.Empty, Color.Red };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Red };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Black };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.White };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Transparent };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Empty };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Point_Color_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontPointColor_Success(TextRenderingHint textRenderingHint, string text, Font font, Point pt, Color foreColor)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, pt, foreColor);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, pt, foreColor);
        }

        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Point_Color_Color_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Point.Empty, Color.Red, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Red, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Black, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.White, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Transparent, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Empty, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Red };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Black };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.White };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Transparent };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Empty };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Point_Color_Color_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontPointColorColor_Success(TextRenderingHint textRenderingHint, string text, Font font, Point pt, Color foreColor, Color backColor)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, pt, foreColor, backColor);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, pt, foreColor, backColor);
        }

        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Point_Color_TextFormatFlags_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Point.Empty, Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Black, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.White, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Transparent, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Empty, TextFormatFlags.Default, };

                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, TextFormatFlags.HorizontalCenter };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, TextFormatFlags.Bottom | TextFormatFlags.Right };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, TextFormatFlags.SingleLine };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, (TextFormatFlags)1024 };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, TextFormatFlags.RightToLeft };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, (TextFormatFlags)int.MaxValue };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Point_Color_TextFormatFlags_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontPointColorTextFormatFlags_Success(TextRenderingHint textRenderingHint, string text, Font font, Point pt, Color foreColor, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, pt, foreColor, flags);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, pt, foreColor, flags);
        }

        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Point_Color_Color_TextFormatFlags_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Point.Empty, Color.Red, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Red, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Black, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.White, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Transparent, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Point(1, 2), Color.Empty, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Black, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.White, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Transparent, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Empty, TextFormatFlags.Default, };

                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, TextFormatFlags.Bottom | TextFormatFlags.Right };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, TextFormatFlags.SingleLine };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, (TextFormatFlags)1024 };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, TextFormatFlags.RightToLeft };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, (TextFormatFlags)int.MaxValue };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Point_Color_Color_TextFormatFlags_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontPointColorColorTextFormatFlags_Success(TextRenderingHint textRenderingHint, string text, Font font, Point pt, Color foreColor, Color backColor, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, pt, foreColor, backColor, flags);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, pt, foreColor, backColor, flags);
        }

        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Rectangle_Color_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Rectangle.Empty, Color.Red };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, -3, -4), Color.Red };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, int.MaxValue, int.MaxValue), Color.Red };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 3, 4), Color.Red };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Black };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.White };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Transparent };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Empty };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Rectangle_Color_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontRectangleColor_Success(TextRenderingHint textRenderingHint, string text, Font font, Rectangle bounds, Color foreColor)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, bounds, foreColor);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, bounds, foreColor);
        }

        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Rectangle_Color_Color_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Rectangle.Empty, Color.Red, Color.Blue };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, -3, -4), Color.Red, Color.Blue };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, int.MaxValue, int.MaxValue), Color.Red, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Black, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.White, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Transparent, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Empty, Color.Blue };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Red };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Black };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.White };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Transparent };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Empty };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Rectangle_Color_Color_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontRectangleColorColor_Success(TextRenderingHint textRenderingHint, string text, Font font, Rectangle rectangle, Color foreColor, Color backColor)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, rectangle, foreColor, backColor);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, rectangle, foreColor, backColor);
        }

        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Rectangle_Color_TextFormatFlags_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Rectangle.Empty, Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, -3, -4), Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, int.MaxValue, int.MaxValue), Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Black, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.White, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Transparent, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Empty, TextFormatFlags.Default, };

                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.HorizontalCenter };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.Bottom | TextFormatFlags.Right };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.SingleLine };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, int.MaxValue, int.MaxValue), Color.Red, TextFormatFlags.SingleLine };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, (TextFormatFlags)1024 };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.RightToLeft };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, (TextFormatFlags)int.MaxValue };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Rectangle_Color_TextFormatFlags_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontRectangleColorTextFormatFlags_Success(TextRenderingHint textRenderingHint, string text, Font font, Rectangle rectangle, Color foreColor, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, rectangle, foreColor, flags);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, rectangle, foreColor, flags);
        }

        public static IEnumerable<object[]> DrawText_IDeviceContext_String_Font_Rectangle_Color_Color_TextFormatFlags_TestData()
        {
            foreach (TextRenderingHint hint in Enum.GetValues(typeof(TextRenderingHint)))
            {
                foreach (string text in new string[] { null, string.Empty, "string" })
                {
                    yield return new object[] { hint, text, null, Rectangle.Empty, Color.Red, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, -3, -4), Color.Red, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, null, new Rectangle(1, 2, int.MaxValue, int.MaxValue), Color.Blue, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Black, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.White, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Transparent, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Empty, Color.Blue, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Red, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Black, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.White, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Transparent, TextFormatFlags.Default, };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Empty, TextFormatFlags.Default, };

                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, TextFormatFlags.Bottom | TextFormatFlags.Right };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, TextFormatFlags.SingleLine };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, int.MaxValue, int.MaxValue), Color.Red, Color.Blue, TextFormatFlags.SingleLine };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, (TextFormatFlags)1024 };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, TextFormatFlags.RightToLeft };
                    yield return new object[] { hint, text, SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, (TextFormatFlags)int.MaxValue };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_IDeviceContext_String_Font_Rectangle_Color_Color_TextFormatFlags_TestData))]
        public void TextRenderer_DrawText_InvokeIDeviceContextStringFontRectangleColorColorTextFormatFlags_Success(TextRenderingHint textRenderingHint, string text, Font font, Rectangle rectangle, Color foreColor, Color backColor, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = textRenderingHint;
            TextRenderer.DrawText(graphics, text, font, rectangle, foreColor, backColor, flags);

            // Call again to test caching.
            TextRenderer.DrawText(graphics, text, font, rectangle, foreColor, backColor, flags);
        }

        [WinFormsFact]
        public void TextRenderer_DrawText_Mocked_Success()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var mockDeviceContext = new Mock<IDeviceContext>(MockBehavior.Strict);
            mockDeviceContext
                .Setup(c => c.GetHdc())
                .Returns(() => graphics.GetHdc())
                .Verifiable();
            mockDeviceContext
                .Setup(c => c.ReleaseHdc())
                .Callback(() => graphics.ReleaseHdc())
                .Verifiable();

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Once());
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Once());

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(2));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(2));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(3));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(3));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(4));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(4));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(5));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(5));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(6));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(6));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(7));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(7));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(8));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(8));
        }

        public static IEnumerable<object[]> DrawText_InvalidHdc_TestData()
        {
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { (IntPtr)1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(DrawText_InvalidHdc_TestData))]
        public void TextRenderer_DrawText_MockedInvalid_Success(IntPtr hdc)
        {
            var mockDeviceContext = new Mock<IDeviceContext>(MockBehavior.Strict);
            mockDeviceContext
                .Setup(c => c.GetHdc())
                .Returns(() => hdc)
                .Verifiable();
            mockDeviceContext
                .Setup(c => c.ReleaseHdc())
                .Verifiable();

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Once());
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Once());

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(2));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(2));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(3));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(3));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, Point.Empty, Color.Red, Color.Blue, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(4));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(4));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(5));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(5));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(6));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(6));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(7));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(7));

            TextRenderer.DrawText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Rectangle(1, 2, 300, 400), Color.Red, Color.Blue, TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(8));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(8));
        }

        [WinFormsFact]
        public void TextRenderer_DrawText_NullDc_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("dc", () => TextRenderer.DrawText(null, "text", SystemFonts.MenuFont, Point.Empty, Color.Red));
        }

        public static IEnumerable<object[]> MeasureText_String_Font_TestData()
        {
            yield return new object[] { "string", null };
            yield return new object[] { "string", SystemFonts.MenuFont };
        }

        [WinFormsTheory]
        [MemberData(nameof(MeasureText_String_Font_TestData))]
        public void TextRenderer_MeasureText_InvokeStringFont_ReturnsExpected(string text, Font font)
        {
            Size result = TextRenderer.MeasureText(text, font);
            Assert.True(result.Width > 0);
            Assert.True(result.Height > 0);

            // Call again to test caching.
            Assert.Equal(result, TextRenderer.MeasureText(text, font));
        }

        public static IEnumerable<object[]> MeasureText_String_Font_Size_TestData()
        {
            yield return new object[] { "string", null, Size.Empty };
            yield return new object[] { "string", null, new Size(1, 2) };
            yield return new object[] { "string", null, new Size(100, 200) };
            yield return new object[] { "string", null, new Size(int.MaxValue, int.MaxValue) };
            yield return new object[] { "string", SystemFonts.MenuFont, Size.Empty };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(1, 2) };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200) };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(int.MaxValue, int.MaxValue) };
        }

        [WinFormsTheory]
        [MemberData(nameof(MeasureText_String_Font_Size_TestData))]
        public void TextRenderer_MeasureText_InvokeStringFontSize_ReturnsExpected(string text, Font font, Size proposedSize)
        {
            Size result = TextRenderer.MeasureText(text, font, proposedSize);
            Assert.True(result.Width > 0);
            Assert.True(result.Height > 0);

            // Call again to test caching.
            Assert.Equal(result, TextRenderer.MeasureText(text, font, proposedSize));
        }

        public static IEnumerable<object[]> MeasureText_String_Font_Size_TextFormatFlags_TestData()
        {
            yield return new object[] { "string", null, Size.Empty, TextFormatFlags.Default };
            yield return new object[] { "string", null, new Size(1, 2), TextFormatFlags.Default };
            yield return new object[] { "string", null, new Size(100, 200), TextFormatFlags.Default };
            yield return new object[] { "string", null, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.Default };
            yield return new object[] { "string", SystemFonts.MenuFont, Size.Empty, TextFormatFlags.Default };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(1, 2), TextFormatFlags.Default };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200), TextFormatFlags.Default };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.Default };

            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200), TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200), TextFormatFlags.Bottom | TextFormatFlags.Right };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200), TextFormatFlags.SingleLine };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200), (TextFormatFlags)1024 };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200), TextFormatFlags.RightToLeft };
            yield return new object[] { "string", SystemFonts.MenuFont, new Size(100, 200), (TextFormatFlags)int.MaxValue };
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(MeasureText_String_Font_Size_TextFormatFlags_TestData))]
        public void TextRenderer_MeasureText_InvokeStringFontSizeTextFormatFlags_ReturnsExpected(string text, Font font, Size proposedSize, TextFormatFlags flags)
        {
            Size result = TextRenderer.MeasureText(text, font, proposedSize, flags);
            Assert.True(result.Width > 0);
            Assert.True(result.Height > 0);

            // Call again to test caching.
            Assert.Equal(result, TextRenderer.MeasureText(text, font, proposedSize, flags));
        }

        [WinFormsTheory]
        [MemberData(nameof(MeasureText_String_Font_TestData))]
        public void TextRenderer_MeasureText_InvokeIDeviceContextStringFont_ReturnsExpected(string text, Font font)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

            Size result = TextRenderer.MeasureText(graphics, text, font);
            Assert.True(result.Width > 0);
            Assert.True(result.Height > 0);

            // Call again to test caching.
            Assert.Equal(result, TextRenderer.MeasureText(graphics, text, font));
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(MeasureText_String_Font_Size_TestData))]
        public void TextRenderer_MeasureText_InvokeIDeviceContextStringFontSize_ReturnsExpected(string text, Font font, Size proposedSize)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

            Size result = TextRenderer.MeasureText(graphics, text, font, proposedSize);
            Assert.True(result.Width > 0);
            Assert.True(result.Height > 0);

            // Call again to test caching.
            Assert.Equal(result, TextRenderer.MeasureText(graphics, text, font, proposedSize));
        }

        [ActiveIssue("https://github.com/dotnet/winforms/issues/3647")]
        [WinFormsTheory(Skip = "Flaky tests, see: https://github.com/dotnet/winforms/issues/3647")]
        [MemberData(nameof(MeasureText_String_Font_Size_TextFormatFlags_TestData))]
        public void TextRenderer_MeasureText_InvokeIDeviceContextStringFontSizeTextFormatFlags_ReturnsExpected(string text, Font font, Size proposedSize, TextFormatFlags flags)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);

            Size result = TextRenderer.MeasureText(graphics, text, font, proposedSize, flags);
            Assert.True(result.Width > 0);
            Assert.True(result.Height > 0);

            // Call again to test caching.
            Assert.Equal(result, TextRenderer.MeasureText(graphics, text, font, proposedSize, flags));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void TextRenderer_MeasureText_NullOrEmptyString_ReturnsEmpty(string text)
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            Assert.Equal(Size.Empty, TextRenderer.MeasureText(text, SystemFonts.MenuFont));
            Assert.Equal(Size.Empty, TextRenderer.MeasureText(text, SystemFonts.MenuFont, new Size(300, 400)));
            Assert.Equal(Size.Empty, TextRenderer.MeasureText(text, SystemFonts.MenuFont, new Size(300, 400), TextFormatFlags.Default));
            Assert.Equal(Size.Empty, TextRenderer.MeasureText(graphics, text, SystemFonts.MenuFont));
            Assert.Equal(Size.Empty, TextRenderer.MeasureText(graphics, text, SystemFonts.MenuFont, new Size(300, 400)));
            Assert.Equal(Size.Empty, TextRenderer.MeasureText(graphics, text, SystemFonts.MenuFont, new Size(300, 400), TextFormatFlags.Default));
        }

        [WinFormsFact]
        public void TextRenderer_MeasureText_Mocked_Success()
        {
            using var image = new Bitmap(10, 10);
            using Graphics graphics = Graphics.FromImage(image);
            var mockDeviceContext = new Mock<IDeviceContext>(MockBehavior.Strict);
            mockDeviceContext
                .Setup(c => c.GetHdc())
                .Returns(() => graphics.GetHdc())
                .Verifiable();
            mockDeviceContext
                .Setup(c => c.ReleaseHdc())
                .Callback(() => graphics.ReleaseHdc())
                .Verifiable();

            TextRenderer.MeasureText(mockDeviceContext.Object, "text", SystemFonts.MenuFont);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Once());
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Once());

            TextRenderer.MeasureText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Size(300, 400));
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(2));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(2));

            TextRenderer.MeasureText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Size(300, 400), TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(3));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(3));
        }

        public static IEnumerable<object[]> MeasureText_InvalidHdc_TestData()
        {
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { (IntPtr)1 };
        }

        [WinFormsTheory]
        [MemberData(nameof(MeasureText_InvalidHdc_TestData))]
        public void TextRenderer_MeasureText_MockedInvalid_Success(IntPtr hdc)
        {
            var mockDeviceContext = new Mock<IDeviceContext>(MockBehavior.Strict);
            mockDeviceContext
                .Setup(c => c.GetHdc())
                .Returns(() => hdc)
                .Verifiable();
            mockDeviceContext
                .Setup(c => c.ReleaseHdc())
                .Verifiable();

            TextRenderer.MeasureText(mockDeviceContext.Object, "text", SystemFonts.MenuFont);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Once());
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Once());

            TextRenderer.MeasureText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Size(300, 400));
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(2));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(2));

            TextRenderer.MeasureText(mockDeviceContext.Object, "text", SystemFonts.MenuFont, new Size(300, 400), TextFormatFlags.Default);
            mockDeviceContext.Verify(c => c.GetHdc(), Times.Exactly(3));
            mockDeviceContext.Verify(c => c.ReleaseHdc(), Times.Exactly(3));
        }

        [WinFormsFact]
        public void TextRenderer_MeasureText_NullDc_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("dc", () => TextRenderer.MeasureText(null, string.Empty, SystemFonts.MenuFont));
            Assert.Throws<ArgumentNullException>("dc", () => TextRenderer.MeasureText(null, string.Empty, SystemFonts.MenuFont, new Size(300, 400)));
            Assert.Throws<ArgumentNullException>("dc", () => TextRenderer.MeasureText(null, string.Empty, SystemFonts.MenuFont, new Size(300, 400), TextFormatFlags.Default));
        }
    }
}
