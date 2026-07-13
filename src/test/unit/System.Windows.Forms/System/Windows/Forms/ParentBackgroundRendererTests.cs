// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Tests;

public class ParentBackgroundRendererTests
{
    [WinFormsFact]
    public void Paint_PaintsPatternAcrossBounds()
    {
        using PatternControl parent = new() { Size = new Size(40, 30) };
        using Control child = new() { Size = parent.Size };
        parent.Controls.Add(child);

        using Bitmap bitmap = new(child.Width, child.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Lime);

        ParentBackgroundRenderer.Paint(child, graphics, child.ClientRectangle, Color.Magenta);

        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(1, 1).ToArgb());
        Assert.Equal(Color.Blue.ToArgb(), bitmap.GetPixel(1, 29).ToArgb());
        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(20, 15).ToArgb());
    }

    [WinFormsFact]
    public void Paint_TextBoxReceivesPatternAcrossBounds()
    {
        using PatternControl parent = new() { Size = new Size(40, 30) };
        using TextBox textBox = new() { Size = parent.Size };
        parent.Controls.Add(textBox);

        using Bitmap bitmap = new(textBox.Width, textBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Yellow);

        ParentBackgroundRenderer.Paint(textBox, graphics, textBox.ClientRectangle, Color.Magenta);

        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(1, 1).ToArgb());
        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(20, 15).ToArgb());
    }

    [WinFormsFact]
    public void Paint_PaintsParentBackgroundImage()
    {
        using Bitmap backgroundImage = new(2, 1);
        backgroundImage.SetPixel(0, 0, Color.Red);
        backgroundImage.SetPixel(1, 0, Color.Blue);

        using Panel parent = new()
        {
            BackgroundImage = backgroundImage,
            BackgroundImageLayout = ImageLayout.Tile,
            Size = new Size(40, 30)
        };
        using Control child = new() { Size = parent.Size };
        parent.Controls.Add(child);
        parent.CreateControl();
        child.CreateControl();

        using Bitmap bitmap = new(child.Width, child.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Lime);

        ParentBackgroundRenderer.Paint(child, graphics, child.ClientRectangle, Color.Magenta);

        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(0, 0).ToArgb());
        Assert.Equal(Color.Blue.ToArgb(), bitmap.GetPixel(1, 0).ToArgb());
        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(20, 15).ToArgb());
    }

    [WinFormsFact]
    public void Paint_ProvidesParentPixelsBeneathAntialiasedRoundedBody()
    {
        using PatternControl parent = new() { Size = new Size(40, 30) };
        using Control child = new() { Size = parent.Size };
        parent.Controls.Add(child);

        using Bitmap bitmap = new(child.Width, child.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Lime);

        ParentBackgroundRenderer.Paint(child, graphics, child.ClientRectangle, Color.Magenta);

        using GraphicsPath body = new();
        body.AddRoundedRectangle(child.ClientRectangle, new Size(8, 8));
        using SolidBrush bodyBrush = new(Color.White);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.FillPath(bodyBrush, body);

        Assert.NotEqual(Color.Lime.ToArgb(), bitmap.GetPixel(2, 2).ToArgb());
        Assert.Equal(Color.White.ToArgb(), bitmap.GetPixel(20, 15).ToArgb());
    }

    private sealed class PatternControl : Control
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle clip = e.ClipRectangle;
            for (int y = clip.Top; y < clip.Bottom; y++)
            {
                for (int x = clip.Left; x < clip.Right; x++)
                {
                    using SolidBrush brush = new(((x / 4 + y / 4) & 1) == 0 ? Color.Red : Color.Blue);
                    e.Graphics.FillRectangle(brush, x, y, 1, 1);
                }
            }
        }
    }
}
