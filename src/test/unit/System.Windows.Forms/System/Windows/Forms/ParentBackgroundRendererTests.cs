// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.Tests;

public class ParentBackgroundRendererTests
{
    [WinFormsFact]
    public void Paint_PaintsPatternInRoundedCutout_LeavesBodyUnchanged()
    {
        using PatternControl parent = new() { Size = new Size(40, 30) };
        using Control child = new() { Size = parent.Size };
        parent.Controls.Add(child);

        using Bitmap bitmap = new(child.Width, child.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Lime);

        using GraphicsPath body = new();
        body.AddRoundedRectangle(child.ClientRectangle, new Size(8, 8));

        ParentBackgroundRenderer.Paint(child, graphics, child.ClientRectangle, body, Color.Magenta);

        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(1, 1).ToArgb());
        Assert.Equal(Color.Blue.ToArgb(), bitmap.GetPixel(1, 29).ToArgb());
        Assert.Equal(Color.Lime.ToArgb(), bitmap.GetPixel(20, 15).ToArgb());
    }

    [WinFormsFact]
    public void Paint_TextBoxBodyRemainsUnpaintedWhenParentPatternFillsCorners()
    {
        using PatternControl parent = new() { Size = new Size(40, 30) };
        using TextBox textBox = new() { Size = parent.Size };
        parent.Controls.Add(textBox);

        using Bitmap bitmap = new(textBox.Width, textBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Yellow);

        using GraphicsPath body = new();
        body.AddRoundedRectangle(textBox.ClientRectangle, new Size(8, 8));

        ParentBackgroundRenderer.Paint(textBox, graphics, textBox.ClientRectangle, body, Color.Magenta);

        Assert.Equal(Color.Red.ToArgb(), bitmap.GetPixel(1, 1).ToArgb());
        Assert.Equal(Color.Yellow.ToArgb(), bitmap.GetPixel(20, 15).ToArgb());
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
