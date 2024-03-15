// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class ToolStripItemImageRenderEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Null_Graphics_ToolStripItem_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { null, null };
        yield return new object[] { null, new ToolStripButton() };
        yield return new object[] { graphics, null };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Null_Graphics_ToolStripItem_TestData))]
    public void ToolStripItemImageRenderEventArgs_Null_Graphics_ToolStripItem_ThrowsArgumentNullException(Graphics g, ToolStripItem toolStripItem)
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripItemImageRenderEventArgs(g, toolStripItem, Rectangle.Empty));
    }

    public static IEnumerable<object[]> Ctor_Graphics_ToolStripItem_Rectangle_TestData()
    {
        Bitmap image = new(10, 10);
        Graphics graphics = Graphics.FromImage(image);

        yield return new object[] { graphics, new ToolStripButton(), new Rectangle(1, 2, 3, 4), null };
        yield return new object[] { graphics, new ToolStripButton() { Image = image }, new Rectangle(1, 2, 3, 4), image };
        yield return new object[]
        {
            graphics, new ToolStripButton
            {
                RightToLeft = RightToLeft.Yes,
                Image = image
            }, new Rectangle(1, 2, 3, 4), image
        };
        yield return new object[]
        {
            graphics, new ToolStripButton
            {
                RightToLeftAutoMirrorImage = true,
                Image = image
            }, new Rectangle(1, 2, 3, 4), image
        };
        yield return new object[]
        {
            graphics, new ToolStripButton
            {
                RightToLeftAutoMirrorImage = true,
                RightToLeft = RightToLeft.Yes
            }, new Rectangle(1, 2, 3, 4), null
        };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_Graphics_ToolStripItem_Rectangle_TestData))]
    public void ToolStripItemImageRenderEventArgs_Ctor_Graphics_ToolStripItem_Rectangle(Graphics g, ToolStripItem item, Rectangle imageRectangle, Image expectedImage)
    {
        ToolStripItemImageRenderEventArgs e = new(g, item, imageRectangle);
        Assert.Equal(g, e.Graphics);
        Assert.Equal(item, e.Item);
        Assert.Equal(imageRectangle, e.ImageRectangle);
        Assert.Same(expectedImage, e.Image);
    }

    [WinFormsFact]
    public void ToolStripItemImageRenderEventArgs_Ctor_Graphics_ToolStripItem_Rectangle_MirroredImage()
    {
        using Bitmap image = new(10, 10);
        using Graphics g = Graphics.FromImage(image);
        ToolStripButton item = new()
        {
            RightToLeftAutoMirrorImage = true,
            RightToLeft = RightToLeft.Yes,
            Image = image
        };
        ToolStripItemImageRenderEventArgs e = new(g, item, new Rectangle(1, 2, 3, 4));
        Assert.Equal(g, e.Graphics);
        Assert.Equal(item, e.Item);
        Assert.Equal(new Rectangle(1, 2, 3, 4), e.ImageRectangle);
        Assert.NotSame(image, e.Image);
        Assert.Equal(image.Size, e.Image.Size);
    }

    public static IEnumerable<object[]> Ctor_ToolStripItem_Image_Rectangle_TestData()
    {
        yield return new object[] { new ToolStripButton(), new Bitmap(10, 10), new Rectangle(1, 2, 3, 4) };
        yield return new object[] { new ToolStripButton() { Image = new Bitmap(10, 10) }, new Bitmap(10, 10), new Rectangle(1, 2, 3, 4) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Ctor_ToolStripItem_Image_Rectangle_TestData))]
    public void ToolStripItemImageRenderEventArgs_Ctor_ToolStripItem_Image_Rectangle(ToolStripItem item, Image image, Rectangle imageRectangle)
    {
        using Graphics graphics = Graphics.FromImage(image);

        ToolStripItemImageRenderEventArgs e = new(graphics, item, image, imageRectangle);

        Assert.Equal(graphics, e.Graphics);
        Assert.Equal(item, e.Item);
        Assert.Equal(image, e.Image);
        Assert.Equal(imageRectangle, e.ImageRectangle);
    }
}
