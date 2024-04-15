// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripItemTextRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_ToolStripItem_String_Rectangle_Color_Font_TextFormatFlags_TestData()
    {
        yield return new object[] { new ToolStripButton(), null, Rectangle.Empty, Color.Empty, null, TextFormatFlags.Top - 1 };
        yield return new object[] { new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, TextFormatFlags.Top };
        yield return new object[] { new ToolStripButton() { RightToLeft = RightToLeft.Yes }, "text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, TextFormatFlags.Bottom };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_ToolStripItem_String_Rectangle_Color_Font_TextFormatFlags_TestData))]
    public void Ctor_ToolStripItem_String_Rectangle_Color_Font_TextFormatFlags(ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format)
    {
        using Bitmap image = new(10, 10);
        using Graphics g = Graphics.FromImage(image);

        ToolStripItemTextRenderEventArgs e = new(g, item, text, textRectangle, textColor, textFont, format);
        Assert.Equal(g, e.Graphics);
        Assert.Equal(item, e.Item);
        Assert.Equal(text, e.Text);
        Assert.Equal(textRectangle, e.TextRectangle);
        Assert.Equal(textColor, e.TextColor);
        Assert.Equal(textFont, e.TextFont);
        Assert.Equal(format, e.TextFormat);
        Assert.Equal(item.TextDirection, e.TextDirection);
    }

    public static IEnumerable<object[]> Ctor_ToolStripItem_String_Rectangle_Color_Font_ContentAlignment_TestData()
    {
        yield return new object[] { new ToolStripButton(), null, Rectangle.Empty, Color.Empty, null, ContentAlignment.TopLeft - 1, TextFormatFlags.Default | TextFormatFlags.Top | TextFormatFlags.HidePrefix };
        yield return new object[] { new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, ContentAlignment.MiddleCenter, TextFormatFlags.Default | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.HidePrefix };
        yield return new object[] { new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, ContentAlignment.BottomRight, TextFormatFlags.Default | TextFormatFlags.Bottom | TextFormatFlags.Right | TextFormatFlags.HidePrefix };
        yield return new object[] { new ToolStripButton() { RightToLeft = RightToLeft.Yes }, "text", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, (ContentAlignment)(-1), TextFormatFlags.Default | TextFormatFlags.Bottom | TextFormatFlags.Right | TextFormatFlags.RightToLeft | TextFormatFlags.HidePrefix };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_ToolStripItem_String_Rectangle_Color_Font_ContentAlignment_TestData))]
    public void Ctor_ToolStripItem_String_Rectangle_Color_Font_ContentAlignment(ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, ContentAlignment textAlign, TextFormatFlags expectedTextFormat)
    {
        using Bitmap image = new(10, 10);
        using Graphics g = Graphics.FromImage(image);

        ToolStripItemTextRenderEventArgs e = new(g, item, text, textRectangle, textColor, textFont, textAlign);
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
    public void Ctor_NullItem_ThrowsArgumentNullException()
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        Assert.Throws<ArgumentNullException>("item", () => new ToolStripItemTextRenderEventArgs(graphics, null, "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, TextFormatFlags.Top));
        Assert.Throws<ArgumentNullException>("item", () => new ToolStripItemTextRenderEventArgs(graphics, null, "", new Rectangle(1, 2, 3, 4), Color.Red, SystemFonts.DefaultFont, ContentAlignment.TopLeft));
    }

    public static IEnumerable<object[]> TextColor_TestData()
    {
        yield return new object[] { Color.Empty };
        yield return new object[] { Color.Red };
    }

    [WinFormsTheory]
    [MemberData(nameof(TextColor_TestData))]
    public void TextColor_Set_GetReturnsExpected(Color value)
    {
        using Bitmap image = new(10, 10);
        using Graphics graphics = Graphics.FromImage(image);
        ToolStripItemTextRenderEventArgs e = new(graphics, new ToolStripButton(), "", new Rectangle(1, 2, 3, 4), Color.Blue, SystemFonts.DefaultFont, TextFormatFlags.Top)
        {
            TextColor = value
        };
        Assert.Equal(value, e.TextColor);
    }
}
