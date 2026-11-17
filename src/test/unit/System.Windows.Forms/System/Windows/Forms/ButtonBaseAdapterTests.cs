// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Verifies foreground image rendering shared by button adapters.
/// </summary>
public class ButtonBaseAdapterTests
{
    public static TheoryData<int, int, bool, bool> DrawImageCore_TestData()
    {
        TheoryData<int, int, bool, bool> data = new();
        foreach (int graphicsDpi in new[] { 96, 120, 144, 192 })
        {
            foreach (int imageDpi in new[] { 72, 96, 300 })
            {
                foreach (bool clipImagesToClient in new[] { false, true })
                {
                    foreach (bool squeezeImage in new[] { false, true })
                    {
                        data.Add(graphicsDpi, imageDpi, clipImagesToClient, squeezeImage);
                    }
                }
            }
        }

        return data;
    }

    [WinFormsTheory]
    [MemberData(nameof(DrawImageCore_TestData))]
    public void DrawImageCore_Png_PreservesPixelSizeAndClipping(
        int graphicsDpi,
        int imageDpi,
        bool clipImagesToClient,
        bool squeezeImage)
    {
        using Bitmap source = new(16, 12);
        source.SetResolution(imageDpi, imageDpi);
        using (Graphics sourceGraphics = Graphics.FromImage(source))
        {
            sourceGraphics.Clear(Color.Red);
        }

        source.SetPixel(0, 0, Color.Blue);
        source.SetPixel(source.Width - 1, source.Height - 1, Color.Gold);
        using MemoryStream stream = new();
        source.Save(stream, ImageFormat.Png);
        stream.Position = 0;
        using Bitmap image = new(stream);
        using Bitmap actual = new(64, 48);
        actual.SetResolution(graphicsDpi, graphicsDpi);
        using Graphics graphics = Graphics.FromImage(actual);
        using Bitmap expected = new(64, 48);
        expected.SetResolution(graphicsDpi, graphicsDpi);
        using Graphics expectedGraphics = Graphics.FromImage(expected);
        graphics.Clear(Color.White);
        expectedGraphics.Clear(Color.White);
        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        expectedGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;
        expectedGraphics.PixelOffsetMode = PixelOffsetMode.Half;
        Rectangle originalClip = new(1, 1, 62, 46);
        graphics.SetClip(originalClip);
        expectedGraphics.SetClip(originalClip);
        RectangleF originalClipBounds = graphics.ClipBounds;
        using Button button = new() { Size = actual.Size };
        ButtonStandardAdapter adapter = new(button);
        ButtonBaseAdapter.LayoutOptions options = new()
        {
            DotNetOneButtonCompat = false,
            ClipImagesToClient = clipImagesToClient
        };
        ButtonBaseAdapter.LayoutData layout = new(options)
        {
            Client = new Rectangle(22, 22, 12, 8)
        };
        Rectangle imageBounds = new(20, 20, image.Width, image.Height);
        if (squeezeImage)
        {
            imageBounds.Size = new Size(image.Width / 2, image.Height / 2);
        }

        Rectangle clippingBounds = clipImagesToClient
            ? layout.Client
            : new Rectangle(4, 4, button.Width - 8, button.Height - 8);
        expectedGraphics.SetClip(clippingBounds, CombineMode.Intersect);
        if (squeezeImage)
        {
            expectedGraphics.SetClip(imageBounds, CombineMode.Intersect);
        }

        expectedGraphics.DrawImage(image, imageBounds.X, imageBounds.Y, image.Width, image.Height);
        adapter.DrawImageCore(graphics, image, imageBounds, imageBounds.Location, layout);

        Assert.Equal(originalClipBounds, graphics.ClipBounds);
        for (int y = 0; y < actual.Height; y++)
        {
            for (int x = 0; x < actual.Width; x++)
            {
                Assert.Equal(expected.GetPixel(x, y).ToArgb(), actual.GetPixel(x, y).ToArgb());
            }
        }
    }
}
