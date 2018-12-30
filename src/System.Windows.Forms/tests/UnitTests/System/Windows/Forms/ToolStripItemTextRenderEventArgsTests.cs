// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripItemTextRenderEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_String_Rectangle_Color_Font_TextFormatFlags_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { graphics, new ToolStripButton(), null, Rectangle.Empty, Color.Empty, null, (TextFormatFlags)(TextFormatFlags.Top - 1) };
            yield return new object[] { graphics, new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, TextFormatFlags.Top };
            yield return new object[] { graphics, new ToolStripButton() { RightToLeft = RightToLeft.Yes }, "text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, TextFormatFlags.Bottom };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStripItem_String_Rectangle_Color_Font_TextFormatFlags_TestData))]
        public void Ctor_Graphics_ToolStripItem_String_Rectangle_Color_Font_TextFormatFlags(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format)
        {
            var e = new ToolStripItemTextRenderEventArgs(g, item, text, textRectangle, textColor, textFont, format);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(item, e.Item);
            Assert.Equal(text, e.Text);
            Assert.Equal(textRectangle, e.TextRectangle);
            Assert.Equal(textColor, e.TextColor);
            Assert.Equal(textFont, e.TextFont);
            Assert.Equal(format, e.TextFormat);
            Assert.Equal(item.TextDirection, e.TextDirection);
        }

        public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_String_Rectangle_Color_Font_ContentAlignment_TestData()
        {
            var image = new Bitmap(10, 10);
            Graphics graphics = Graphics.FromImage(image);

            yield return new object[] { graphics, new ToolStripButton(), null, Rectangle.Empty, Color.Empty, null, (ContentAlignment)(ContentAlignment.TopLeft - 1), TextFormatFlags.Default | TextFormatFlags.Top | TextFormatFlags.HidePrefix };
            yield return new object[] { graphics, new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, ContentAlignment.MiddleCenter, TextFormatFlags.Default | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
            yield return new object[] { graphics, new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, ContentAlignment.BottomRight, TextFormatFlags.Default | TextFormatFlags.Bottom | TextFormatFlags.Right | TextFormatFlags.HidePrefix  };
            yield return new object[] { graphics, new ToolStripButton() { RightToLeft = RightToLeft.Yes }, "text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, (ContentAlignment)(-1), TextFormatFlags.Default | TextFormatFlags.Bottom | TextFormatFlags.Right | TextFormatFlags.RightToLeft | TextFormatFlags.HidePrefix };
        }

        [Theory]
        [MemberData(nameof(Ctor_Graphics_ToolStripItem_String_Rectangle_Color_Font_ContentAlignment_TestData))]
        public void Ctor_Graphics_ToolStripItem_String_Rectangle_Color_Font_ContentAlignment(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, ContentAlignment textAlign, TextFormatFlags expectedTextFormat)
        {
            var e = new ToolStripItemTextRenderEventArgs(g, item, text, textRectangle, textColor, textFont, textAlign);
            Assert.Equal(g, e.Graphics);
            Assert.Equal(item, e.Item);
            Assert.Equal(text, e.Text);
            Assert.Equal(textRectangle, e.TextRectangle);
            Assert.Equal(textColor, e.TextColor);
            Assert.Equal(textFont, e.TextFont);
            Assert.Equal(expectedTextFormat, e.TextFormat);
            Assert.Equal(item.TextDirection, e.TextDirection);
        }

        [Fact]
        public void Ctor_NullItem_ThrowsNullReferenceException()
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Assert.Throws<NullReferenceException>(() => new ToolStripItemTextRenderEventArgs(graphics, null, "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, TextFormatFlags.Top));
                Assert.Throws<NullReferenceException>(() => new ToolStripItemTextRenderEventArgs(graphics, null, "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, ContentAlignment.TopLeft));
            }
        }

        public static IEnumerable<object[]> TextColor_TestData()
        {
            yield return new object[] { Color.Empty };
            yield return new object[] { Color.Red };
        }

        [Theory]
        [MemberData(nameof(TextColor_TestData))]
        public void TextColor_Set_GetReturnsExpected(Color value)
        {
            using (var image = new Bitmap(10, 10))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                var e = new ToolStripItemTextRenderEventArgs(graphics, new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Blue, SystemFonts.DefaultFont, TextFormatFlags.Top)
                {
                    TextColor = value
                };
                Assert.Equal(value, e.TextColor);
            }
        }
    }
}
