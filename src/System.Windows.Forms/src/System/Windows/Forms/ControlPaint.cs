// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  The ControlPaint class provides a series of methods that can be used to paint common Windows UI pieces. Many
    ///  Windows Forms controls use this class to paint their UI elements.
    /// </summary>
    public static partial class ControlPaint
    {
        [ThreadStatic]
        private static Bitmap t_checkImage;         // image used to render checkmarks

        [ThreadStatic]
        private static Pen t_focusPen;              // pen used to draw a focus rectangle

        [ThreadStatic]
        private static Pen t_focusPenInvert;        // pen used to draw a focus rectangle

        [ThreadStatic]
        private static Color t_focusPenColor;       // the last background color the focus pen was created with

        [ThreadStatic]
        private static bool t_hcFocusPen;           // cached focus pen intended for high contrast mode

        private static Pen s_grabPenPrimary;        // pen used for primary grab handles
        private static Pen s_grabPenSecondary;      // pen used for secondary grab handles
        private static Brush s_grabBrushPrimary;    // brush used for primary grab handles
        private static Brush s_grabBrushSecondary;  // brush used for secondary grab handles

        [ThreadStatic]
        private static Brush t_frameBrushActive;    // brush used for the active selection frame

        private static Color s_frameColorActive;    // color of active frame brush

        [ThreadStatic]
        private static Brush t_frameBrushSelected;  // brush used for the inactive selection frame

        private static Color s_frameColorSelected;  // color of selected frame brush

        [ThreadStatic]
        private static Brush t_gridBrush;           // brush used to draw a grid

        private static Size s_gridSize;             // the dimensions of the grid dots
        private static bool s_gridInvert;           // true if the grid color is inverted

        [ThreadStatic]
        private static ImageAttributes t_disabledImageAttr; // ImageAttributes used to render disabled images

        private const ContentAlignment AnyRight
            = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
        private const ContentAlignment AnyBottom
            = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
        private const ContentAlignment AnyCenter
            = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
        private const ContentAlignment AnyMiddle
            = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;

        internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
        {
            Rectangle result = bounds;

            if (backgroundImage is null)
            {
                return result;
            }

            switch (imageLayout)
            {
                case ImageLayout.Stretch:
                    result.Size = bounds.Size;
                    break;

                case ImageLayout.None:
                    result.Size = backgroundImage.Size;
                    break;

                case ImageLayout.Center:
                    result.Size = backgroundImage.Size;
                    Size szCtl = bounds.Size;

                    if (szCtl.Width > result.Width)
                    {
                        result.X = (szCtl.Width - result.Width) / 2;
                    }
                    if (szCtl.Height > result.Height)
                    {
                        result.Y = (szCtl.Height - result.Height) / 2;
                    }
                    break;

                case ImageLayout.Zoom:
                    Size imageSize = backgroundImage.Size;
                    float xRatio = bounds.Width / (float)imageSize.Width;
                    float yRatio = bounds.Height / (float)imageSize.Height;
                    if (xRatio < yRatio)
                    {
                        // Width should fill the entire bounds.
                        result.Width = bounds.Width;

                        // Preserve the aspect ratio by multiplying the xRatio by the height, adding .5 to round to
                        // the nearest pixel.
                        result.Height = (int)((imageSize.Height * xRatio) + .5);
                        if (bounds.Y >= 0)
                        {
                            result.Y = (bounds.Height - result.Height) / 2;
                        }
                    }
                    else
                    {
                        // Width should fill the entire bounds.
                        result.Height = bounds.Height;

                        // Preserve the aspect ratio by multiplying the xRatio by the height, adding .5 to round to
                        // the nearest pixel.
                        result.Width = (int)((imageSize.Width * yRatio) + .5);
                        if (bounds.X >= 0)
                        {
                            result.X = (bounds.Width - result.Width) / 2;
                        }
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        ///  Returns a color appropriate for certain elements that are ControlDark in normal color schemes, but for
        ///  which ControlDark does not work in high contrast color schemes.
        /// </summary>
        public static Color ContrastControlDark
            => SystemInformation.HighContrast ? SystemColors.WindowFrame : SystemColors.ControlDark;

        /// <summary>
        ///  Creates a 16-bit color bitmap.
        ///  Sadly, this must be public for the designer to get at it.
        ///  From MSDN:
        ///    This member supports the framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public unsafe static IntPtr CreateHBitmap16Bit(Bitmap bitmap, Color background)
        {
            Gdi32.HBITMAP hbitmap;
            Size size = bitmap.Size;

            // Don't use the cached DC here as this isn't a common API and we're manipulating the state.
            using var screen = new Gdi32.CreateDcScope(default);
            using var dc = new Gdi32.CreateDcScope(screen);

            Gdi32.HPALETTE palette = Gdi32.CreateHalftonePalette(dc);
            Gdi32.GetObjectW(palette, out uint entryCount);

            byte[] imageBuffer = ArrayPool<byte>.Shared.Rent(bitmap.Width * bitmap.Height);

            byte[] bitmapInfoBuffer = ArrayPool<byte>.Shared
                .Rent(checked((int)(sizeof(Gdi32.BITMAPINFOHEADER) + (sizeof(Gdi32.RGBQUAD) * entryCount))));

            // Create a DIB based on the screen DC to write into with a halftone palette
            fixed (byte* bi = bitmapInfoBuffer)
            {
                *((Gdi32.BITMAPINFOHEADER*)bi) = new Gdi32.BITMAPINFOHEADER
                {
                    biSize = (uint)sizeof(Gdi32.BITMAPINFOHEADER),
                    biWidth = bitmap.Width,
                    biHeight = bitmap.Height,
                    biPlanes = 1,
                    biBitCount = 16,
                    biCompression = Gdi32.BI.RGB
                };

                var colors = new Span<Gdi32.RGBQUAD>(bi + sizeof(Gdi32.BITMAPINFOHEADER), (int)entryCount);
                Span<Gdi32.PALETTEENTRY> entries = stackalloc Gdi32.PALETTEENTRY[(int)entryCount];
                Gdi32.GetPaletteEntries(palette, entries);

                // Set up color table
                for (int i = 0; i < entryCount; i++)
                {
                    Gdi32.PALETTEENTRY entry = entries[i];
                    colors[i] = new Gdi32.RGBQUAD()
                    {
                        rgbRed = entry.peRed,
                        rgbGreen = entry.peGreen,
                        rgbBlue = entry.peBlue
                    };
                }

                Gdi32.DeleteObject(palette);

                hbitmap = Gdi32.CreateDIBSection(
                    screen,
                    (IntPtr)bi,
                    Gdi32.DIB.RGB_COLORS,
                    imageBuffer,
                    IntPtr.Zero,
                    0);

                if (hbitmap.IsNull)
                {
                    throw new Win32Exception();
                }

                ArrayPool<byte>.Shared.Return(bitmapInfoBuffer);
            }

            try
            {
                // Put our new bitmap handle (with the halftone palette) into the dc and use Graphics to
                // copy the Bitmap into it.

                Gdi32.HGDIOBJ previousBitmap = Gdi32.SelectObject(dc, hbitmap);
                if (previousBitmap.IsNull)
                {
                    throw new Win32Exception();
                }

                Gdi32.DeleteObject(previousBitmap);

                using Graphics graphics = dc.CreateGraphics();
                using var brush = background.GetCachedSolidBrushScope();
                graphics.FillRectangle(brush, 0, 0, size.Width, size.Height);
                graphics.DrawImage(bitmap, 0, 0, size.Width, size.Height);
            }
            catch
            {
                // As we're throwing out, we can't return this and need to delete it.
                Gdi32.DeleteObject(hbitmap);
                throw;
            }

            // The caller is responsible for freeing the HBITMAP.
            return (IntPtr)hbitmap;
        }

        /// <summary>
        ///  Creates a Win32 HBITMAP out of the image. You are responsible for deleting the HBITMAP. If the image
        ///  uses transparency the background will be filled with the specified color.
        /// </summary>
        public unsafe static IntPtr CreateHBitmapTransparencyMask(Bitmap bitmap)
        {
            if (bitmap is null)
                throw new ArgumentNullException(nameof(bitmap));

            Size size = bitmap.Size;
            int width = bitmap.Width;
            int height = bitmap.Height;

            int monochromeStride = width / 8;
            if ((width % 8) != 0)
            {
                // Want division to round up, not down
                monochromeStride++;
            }

            // Must be multiple of two -- i.e., bitmap scanlines must fall on double-byte boundaries.
            if ((monochromeStride % 2) != 0)
            {
                monochromeStride++;
            }

            byte[] bits = new byte[monochromeStride * height];
            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            Debug.Assert(data.Scan0 != IntPtr.Zero, "BitmapData.Scan0 is null; check marshalling");

            for (int y = 0; y < height; y++)
            {
                IntPtr scan = (IntPtr)((long)data.Scan0 + y * data.Stride);
                for (int x = 0; x < width; x++)
                {
                    int color = Marshal.ReadInt32(scan, x * 4);
                    if (color >> 24 == 0)
                    {
                        // Pixel is transparent; set bit to 1
                        int index = monochromeStride * y + x / 8;
                        bits[index] |= (byte)(0x80 >> (x % 8));
                    }
                }
            }

            bitmap.UnlockBits(data);

            // Create 1bpp.
            fixed (byte* pBits = bits)
            {
                return (IntPtr)Gdi32.CreateBitmap(size.Width, size.Height, 1, 1, pBits);
            }
        }

        /// <summary>
        ///  Creates a Win32 HBITMAP out of the image. You are responsible for deleting the HBITMAP. If the image uses
        ///  transparency, the background will be filled with the specified color.
        /// </summary>
        public static IntPtr CreateHBitmapColorMask(Bitmap bitmap, IntPtr monochromeMask)
        {
            Size size = bitmap.Size;

            Gdi32.HBITMAP colorMask = (Gdi32.HBITMAP)bitmap.GetHbitmap();
            using var screenDC = new User32.GetDcScope(IntPtr.Zero);
            using var sourceDC = new Gdi32.CreateDcScope(screenDC);
            using var targetDC = new Gdi32.CreateDcScope(screenDC);
            using var sourceBitmapSelection = new Gdi32.SelectObjectScope(sourceDC, (Gdi32.HBITMAP)monochromeMask);
            using var targetBitmapSelection = new Gdi32.SelectObjectScope(targetDC, colorMask);

            // Now the trick is to make colorBitmap black wherever the transparent color is located, but keep the
            // original color everywhere else. We've already got the original bitmap, so all we need to do is to AND
            // with the inverse of the mask (ROP DSna). When going from monochrome to color, Windows sets all 1 bits
            // to the background color, and all 0 bits to the foreground color.

            Gdi32.SetBkColor(targetDC, 0x00ffffff);    // white
            Gdi32.SetTextColor(targetDC, 0x00000000);  // black
            Gdi32.BitBlt(
                targetDC,
                0, 0, size.Width, size.Height,
                sourceDC,
                0, 0,
                (Gdi32.ROP)0x220326); // RasterOp.SOURCE.Invert().AndWith(RasterOp.TARGET).GetRop());

            return (IntPtr)colorMask;
        }

        internal unsafe static Gdi32.HBRUSH CreateHalftoneHBRUSH()
        {
            short* grayPattern = stackalloc short[8];
            for (int i = 0; i < 8; i++)
            {
                grayPattern[i] = (short)(0x5555 << (i & 1));
            }

            using var hBitmap = new Gdi32.CreateBitmapScope(8, 8, 1, 1, grayPattern);

            var lb = new Gdi32.LOGBRUSH
            {
                lbColor = Color.Black,
                lbStyle = Gdi32.BS.PATTERN,
                lbHatch = (IntPtr)hBitmap
            };

            return Gdi32.CreateBrushIndirect(ref lb);
        }

        /// <summary>
        ///  Draws a border of the specified style and color to the given graphics.
        /// </summary>
        private static DashStyle BorderStyleToDashStyle(ButtonBorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case ButtonBorderStyle.Dotted:
                    return DashStyle.Dot;
                case ButtonBorderStyle.Dashed:
                    return DashStyle.Dash;
                case ButtonBorderStyle.Solid:
                    return DashStyle.Solid;
                default:
                    Debug.Fail("border style has no corresponding dash style");
                    return DashStyle.Solid;
            }
        }

        /// <summary>
        ///  Creates a new color that is a object of the given color.
        /// </summary>
        public static Color Dark(Color baseColor, float percOfDarkDark) => new HLSColor(baseColor).Darker(percOfDarkDark);

        /// <summary>
        ///  Creates a new color that is a object of the given color.
        /// </summary>
        public static Color Dark(Color baseColor) => new HLSColor(baseColor).Darker(0.5f);

        /// <summary>
        ///  Creates a new darker color from <paramref name="baseColor"/>.
        /// </summary>
        public static Color DarkDark(Color baseColor) => new HLSColor(baseColor).Darker(1.0f);

        /// <summary>
        ///  Returns true if the luminosity of <paramref name="c1"/> is less than <paramref name="c2"/>.
        /// </summary>
        internal static bool IsDarker(Color c1, Color c2) => new HLSColor(c1).Luminosity < new HLSColor(c2).Luminosity;

        /// <summary>
        ///  Used by PrintToMetaFileRecursive overrides (Label, Panel) to manually paint borders for UserPaint controls
        ///  that were relying on their window style to provide their borders.
        /// </summary>
        internal static void PrintBorder(Graphics graphics, Rectangle bounds, BorderStyle style, Border3DStyle b3dStyle)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            switch (style)
            {
                case BorderStyle.FixedSingle:
                    DrawBorder(graphics, bounds, Color.FromKnownColor(KnownColor.WindowFrame), ButtonBorderStyle.Solid);
                    break;
                case BorderStyle.Fixed3D:
                    DrawBorder3D(graphics, bounds, b3dStyle);
                    break;
                case BorderStyle.None:
                    break;
                default:
                    Debug.Fail("Unsupported border style.");
                    break;
            }
        }

        internal static void DrawBackgroundImage(
            Graphics g,
            Image backgroundImage,
            Color backColor,
            ImageLayout backgroundImageLayout,
            Rectangle bounds,
            Rectangle clipRect,
            Point scrollOffset = default,
            RightToLeft rightToLeft = RightToLeft.No)
        {
            if (g is null)
                throw new ArgumentNullException(nameof(g));

            if (backgroundImageLayout == ImageLayout.Tile)
            {
                using TextureBrush textureBrush = new TextureBrush(backgroundImage, WrapMode.Tile);

                // Make sure the brush origin matches the display rectangle, not the client rectangle,
                // so the background image scrolls on AutoScroll forms.
                if (scrollOffset != Point.Empty)
                {
                    Matrix transform = textureBrush.Transform;
                    transform.Translate(scrollOffset.X, scrollOffset.Y);
                    textureBrush.Transform = transform;
                }

                g.FillRectangle(textureBrush, clipRect);
            }
            else
            {
                // Center, Stretch, Zoom

                Rectangle imageRectangle = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);

                // Flip the coordinates only if we don't do any layout, since otherwise the image should be at the
                // center of the displayRectangle anyway.

                if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None)
                {
                    imageRectangle.X += clipRect.Width - imageRectangle.Width;
                }

                // We fill the entire cliprect with the backcolor in case the image is transparent.
                // Also, if gdi+ can't quite fill the rect with the image, they will interpolate the remaining
                // pixels, and make them semi-transparent. This is another reason why we need to fill the entire rect.
                // If we didn't where ever the image was transparent, we would get garbage.
                using (var brush = backColor.GetCachedSolidBrushScope())
                {
                    g.FillRectangle(brush, clipRect);
                }

                if (!clipRect.Contains(imageRectangle))
                {
                    if (backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Zoom)
                    {
                        imageRectangle.Intersect(clipRect);
                        g.DrawImage(backgroundImage, imageRectangle);
                    }
                    else if (backgroundImageLayout == ImageLayout.None)
                    {
                        imageRectangle.Offset(clipRect.Location);
                        Rectangle imageRect = imageRectangle;
                        imageRect.Intersect(clipRect);
                        Rectangle partOfImageToDraw = new Rectangle(Point.Empty, imageRect.Size);
                        g.DrawImage(
                            backgroundImage,
                            imageRect,
                            partOfImageToDraw.X,
                            partOfImageToDraw.Y,
                            partOfImageToDraw.Width,
                            partOfImageToDraw.Height,
                            GraphicsUnit.Pixel);
                    }
                    else
                    {
                        Rectangle imageRect = imageRectangle;
                        imageRect.Intersect(clipRect);
                        Rectangle partOfImageToDraw = new Rectangle(
                            new Point(imageRect.X - imageRectangle.X, imageRect.Y - imageRectangle.Y),
                            imageRect.Size);

                        g.DrawImage(
                            backgroundImage,
                            imageRect,
                            partOfImageToDraw.X,
                            partOfImageToDraw.Y,
                            partOfImageToDraw.Width,
                            partOfImageToDraw.Height,
                            GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    ImageAttributes imageAttrib = new ImageAttributes();
                    imageAttrib.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(
                        backgroundImage,
                        imageRectangle,
                        0,
                        0,
                        backgroundImage.Width,
                        backgroundImage.Height,
                        GraphicsUnit.Pixel,
                        imageAttrib);

                    imageAttrib.Dispose();
                }
            }
        }

        public static void DrawBorder(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            switch (style)
            {
                case ButtonBorderStyle.None:
                    // nothing
                    break;
                case ButtonBorderStyle.Dotted:
                case ButtonBorderStyle.Dashed:
                case ButtonBorderStyle.Solid:
                    DrawBorderSimple(graphics, bounds, color, style);
                    break;
                case ButtonBorderStyle.Inset:
                case ButtonBorderStyle.Outset:
                    DrawBorderComplex(graphics, bounds, color, style);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        ///  Draws a border of the specified style and color to the given graphics.
        /// </summary>
        public static unsafe void DrawBorder(
            Graphics graphics,
            Rectangle bounds,
            Color leftColor, int leftWidth, ButtonBorderStyle leftStyle,
            Color topColor, int topWidth, ButtonBorderStyle topStyle,
            Color rightColor, int rightWidth, ButtonBorderStyle rightStyle,
            Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            DrawBorder(
                (IDeviceContext)graphics,
                bounds,
                leftColor, leftWidth, leftStyle,
                topColor, topWidth, topStyle,
                rightColor, rightWidth, rightStyle,
                bottomColor, bottomWidth, bottomStyle);
        }

        internal static unsafe void DrawBorder(
            IDeviceContext deviceContext,
            Rectangle bounds,
            Color leftColor, int leftWidth, ButtonBorderStyle leftStyle,
            Color topColor, int topWidth, ButtonBorderStyle topStyle,
            Color rightColor, int rightWidth, ButtonBorderStyle rightStyle,
            Color bottomColor, int bottomWidth, ButtonBorderStyle bottomStyle)
        {
            // Very general, and very slow
            if (leftWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(leftWidth));
            if (topWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(topWidth));
            if (rightWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(rightWidth));
            if (bottomWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(bottomWidth));

            int totalData = (topWidth + leftWidth + bottomWidth + rightWidth) * 2;
            Span<int> allData;

            if (totalData <= 40)
            {
                // Reasonable to put on the stack (40 * 8 bytes)
                int* data = stackalloc int[totalData];
                allData = new Span<int>(data, totalData);
            }
            else
            {
                allData = new int[totalData];
            }

            Span<int> topLineLefts = allData.Slice(0, topWidth);
            allData = allData.Slice(topWidth);
            Span<int> topLineRights = allData.Slice(0, topWidth);
            allData = allData.Slice(topWidth);
            Span<int> leftLineTops = allData.Slice(0, leftWidth);
            allData = allData.Slice(leftWidth);
            Span<int> leftLineBottoms = allData.Slice(0, leftWidth);
            allData = allData.Slice(leftWidth);
            Span<int> bottomLineLefts = allData.Slice(0, bottomWidth);
            allData = allData.Slice(bottomWidth);
            Span<int> bottomLineRights = allData.Slice(0, bottomWidth);
            allData = allData.Slice(bottomWidth);
            Span<int> rightLineTops = allData.Slice(0, rightWidth);
            allData = allData.Slice(rightWidth);
            Span<int> rightLineBottoms = allData.Slice(0, rightWidth);

            float topToLeft = 0.0f;
            float bottomToLeft = 0.0f;
            if (leftWidth > 0)
            {
                topToLeft = topWidth / ((float)leftWidth);
                bottomToLeft = bottomWidth / ((float)leftWidth);
            }

            float topToRight = 0.0f;
            float bottomToRight = 0.0f;
            if (rightWidth > 0)
            {
                topToRight = topWidth / ((float)rightWidth);
                bottomToRight = bottomWidth / ((float)rightWidth);
            }

            if (topWidth > 0)
            {
                int i = 0;
                for (; i < topWidth; i++)
                {
                    int leftOffset = topToLeft > 0 ? (int)(i / topToLeft) : 0;
                    int rightOffset = topToRight > 0 ? (int)(i / topToRight) : 0;

                    topLineLefts[i] = bounds.X + leftOffset;
                    topLineRights[i] = bounds.X + bounds.Width - rightOffset - 1;

                    if (leftWidth > 0)
                    {
                        leftLineTops[leftOffset] = bounds.Y + i + 1;
                    }

                    if (rightWidth > 0)
                    {
                        rightLineTops[rightOffset] = bounds.Y + i;
                    }
                }

                for (int j = i; j < leftWidth; j++)
                {
                    leftLineTops[j] = bounds.Y + i + 1;
                }

                for (int j = i; j < rightWidth; j++)
                {
                    rightLineTops[j] = bounds.Y + i;
                }
            }
            else
            {
                for (int i = 0; i < leftWidth; i++)
                {
                    leftLineTops[i] = bounds.Y;
                }

                for (int i = 0; i < rightWidth; i++)
                {
                    rightLineTops[i] = bounds.Y;
                }
            }

            if (bottomWidth > 0)
            {
                int i = 0;
                for (; i < bottomWidth; i++)
                {
                    int leftOffset = bottomToLeft > 0 ? (int)(i / bottomToLeft) : 0;
                    int rightOffset = bottomToRight > 0 ? (int)(i / bottomToRight) : 0;

                    bottomLineLefts[i] = bounds.X + leftOffset;
                    bottomLineRights[i] = bounds.X + bounds.Width - rightOffset - 1;

                    if (leftWidth > 0)
                    {
                        leftLineBottoms[leftOffset] = bounds.Y + bounds.Height - i - 1;
                    }

                    if (rightWidth > 0)
                    {
                        rightLineBottoms[rightOffset] = bounds.Y + bounds.Height - i - 1;
                    }
                }

                for (int j = i; j < leftWidth; j++)
                {
                    leftLineBottoms[j] = bounds.Y + bounds.Height - i - 1;
                }

                for (int j = i; j < rightWidth; j++)
                {
                    rightLineBottoms[j] = bounds.Y + bounds.Height - i - 1;
                }
            }
            else
            {
                for (int i = 0; i < leftWidth; i++)
                {
                    leftLineBottoms[i] = bounds.Y + bounds.Height - 1;
                }

                for (int i = 0; i < rightWidth; i++)
                {
                    rightLineBottoms[i] = bounds.Y + bounds.Height - 1;
                }
            }

            // Draw top border
            switch (topStyle)
            {
                case ButtonBorderStyle.None:
                    break;
                case ButtonBorderStyle.Dotted:
                case ButtonBorderStyle.Dashed:
                case ButtonBorderStyle.Solid:
                    {
                        if (!topColor.HasTransparency() && topStyle == ButtonBorderStyle.Solid)
                        {
                            using var hdc = new DeviceContextHdcScope(deviceContext);
                            using var hpen = new Gdi32.CreatePenScope(topColor);
                            for (int i = 0; i < topWidth; i++)
                            {
                                // Need to add one to the destination point for GDI to render the same as GDI+
                                hdc.DrawLine(hpen, topLineLefts[i], bounds.Y + i, topLineRights[i] + 1, bounds.Y + i);
                            }
                        }
                        else
                        {
                            Graphics graphics = deviceContext.TryGetGraphics(create: true);
                            using var pen = topColor.CreateStaticPen(
                                topStyle switch
                                {
                                    ButtonBorderStyle.Dotted => DashStyle.Dot,
                                    ButtonBorderStyle.Dashed => DashStyle.Dash,
                                    _ => DashStyle.Solid,
                                });

                            for (int i = 0; i < topWidth; i++)
                            {
                                graphics.DrawLine(pen, topLineLefts[i], bounds.Y + i, topLineRights[i], bounds.Y + i);
                            }
                        }

                        break;
                    }
                case ButtonBorderStyle.Inset:
                case ButtonBorderStyle.Outset:
                    {
                        HLSColor hlsColor = new HLSColor(topColor);
                        float inc = InfinityToOne(1.0f / (topWidth - 1));
                        using var hdc = new DeviceContextHdcScope(deviceContext);
                        for (int i = 0; i < topWidth; i++)
                        {
                            using var hpen = new Gdi32.CreatePenScope(topStyle == ButtonBorderStyle.Inset
                                ? hlsColor.Darker(1.0f - i * inc)
                                : hlsColor.Lighter(1.0f - i * inc));

                            // Need to add one to the destination point for GDI to render the same as GDI+
                            hdc.DrawLine(hpen, topLineLefts[i], bounds.Y + i, topLineRights[i] + 1, bounds.Y + i);
                        }
                        break;
                    }
            }

            // Draw left border
            switch (leftStyle)
            {
                case ButtonBorderStyle.None:
                    break;
                case ButtonBorderStyle.Dotted:
                case ButtonBorderStyle.Dashed:
                case ButtonBorderStyle.Solid:
                    {
                        if (!leftColor.HasTransparency() && leftStyle == ButtonBorderStyle.Solid)
                        {
                            using var hdc = new DeviceContextHdcScope(deviceContext);
                            using var hpen = new Gdi32.CreatePenScope(leftColor);
                            for (int i = 0; i < leftWidth; i++)
                            {
                                // Need to add one to the destination point for GDI to render the same as GDI+
                                hdc.DrawLine(hpen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i] + 1);
                            }
                        }
                        else
                        {
                            Graphics graphics = deviceContext.TryGetGraphics(create: true);
                            using var pen = leftColor.CreateStaticPen(
                                leftStyle switch
                                {
                                    ButtonBorderStyle.Dotted => DashStyle.Dot,
                                    ButtonBorderStyle.Dashed => DashStyle.Dash,
                                    _ => DashStyle.Solid,
                                });

                            for (int i = 0; i < leftWidth; i++)
                            {
                                graphics.DrawLine(pen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i]);
                            }
                        }
                        break;
                    }
                case ButtonBorderStyle.Inset:
                case ButtonBorderStyle.Outset:
                    {
                        HLSColor hlsColor = new HLSColor(leftColor);
                        float inc = InfinityToOne(1.0f / (leftWidth - 1));
                        using var hdc = new DeviceContextHdcScope(deviceContext);
                        for (int i = 0; i < leftWidth; i++)
                        {
                            using var hpen = new Gdi32.CreatePenScope(leftStyle == ButtonBorderStyle.Inset
                                ? hlsColor.Darker(1.0f - i * inc)
                                : hlsColor.Lighter(1.0f - i * inc));

                            // Need to add one to the destination point for GDI to render the same as GDI+
                            hdc.DrawLine(hpen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i] + 1);
                        }
                        break;
                    }
            }

            // Draw bottom border
            switch (bottomStyle)
            {
                case ButtonBorderStyle.None:
                    break;
                case ButtonBorderStyle.Dotted:
                case ButtonBorderStyle.Dashed:
                case ButtonBorderStyle.Solid:
                    {
                        if (!bottomColor.HasTransparency() && bottomStyle == ButtonBorderStyle.Solid)
                        {
                            using var hdc = new DeviceContextHdcScope(deviceContext);
                            using var hpen = new Gdi32.CreatePenScope(bottomColor);
                            for (int i = 0; i < bottomWidth; i++)
                            {
                                // Need to add one to the destination point for GDI to render the same as GDI+
                                hdc.DrawLine(
                                    hpen,
                                    bottomLineLefts[i],
                                    bounds.Y + bounds.Height - 1 - i,
                                    bottomLineRights[i] + 1,
                                    bounds.Y + bounds.Height - 1 - i);
                            }
                        }
                        else
                        {
                            Graphics graphics = deviceContext.TryGetGraphics(create: true);
                            using var pen = bottomColor.CreateStaticPen(
                                bottomStyle switch
                                {
                                    ButtonBorderStyle.Dotted => DashStyle.Dot,
                                    ButtonBorderStyle.Dashed => DashStyle.Dash,
                                    _ => DashStyle.Solid,
                                });

                            for (int i = 0; i < bottomWidth; i++)
                            {
                                graphics.DrawLine(
                                    pen,
                                    bottomLineLefts[i],
                                    bounds.Y + bounds.Height - 1 - i,
                                    bottomLineRights[i],
                                    bounds.Y + bounds.Height - 1 - i);
                            }
                        }
                        break;
                    }
                case ButtonBorderStyle.Inset:
                case ButtonBorderStyle.Outset:
                    {
                        HLSColor hlsColor = new HLSColor(bottomColor);
                        float inc = InfinityToOne(1.0f / (bottomWidth - 1));
                        using var hdc = new DeviceContextHdcScope(deviceContext);
                        for (int i = 0; i < bottomWidth; i++)
                        {
                            using var hpen = new Gdi32.CreatePenScope(bottomStyle != ButtonBorderStyle.Inset
                                ? hlsColor.Darker(1.0f - i * inc)
                                : hlsColor.Lighter(1.0f - i * inc));

                            // Need to add one to the destination point for GDI to render the same as GDI+
                            hdc.DrawLine(
                                hpen,
                                bottomLineLefts[i],
                                bounds.Y + bounds.Height - 1 - i,
                                bottomLineRights[i] + 1,
                                bounds.Y + bounds.Height - 1 - i);
                        }
                        break;
                    }
            }

            // Draw right border
            switch (rightStyle)
            {
                case ButtonBorderStyle.None:
                    break;
                case ButtonBorderStyle.Dotted:
                case ButtonBorderStyle.Dashed:
                case ButtonBorderStyle.Solid:
                    {
                        if (!rightColor.HasTransparency() && rightStyle == ButtonBorderStyle.Solid)
                        {
                            using var hdc = new DeviceContextHdcScope(deviceContext);
                            using var hpen = new Gdi32.CreatePenScope(rightColor);
                            for (int i = 0; i < rightWidth; i++)
                            {
                                // Need to add one to the destination point for GDI to render the same as GDI+
                                hdc.DrawLine(
                                    hpen,
                                    bounds.X + bounds.Width - 1 - i,
                                    rightLineTops[i],
                                    bounds.X + bounds.Width - 1 - i,
                                    rightLineBottoms[i] + 1);
                            }
                        }
                        else
                        {
                            Graphics graphics = deviceContext.TryGetGraphics(create: true);
                            using var pen = rightColor.CreateStaticPen(
                                rightStyle switch
                                {
                                    ButtonBorderStyle.Dotted => DashStyle.Dot,
                                    ButtonBorderStyle.Dashed => DashStyle.Dash,
                                    _ => DashStyle.Solid,
                                });

                            for (int i = 0; i < rightWidth; i++)
                            {
                                graphics.DrawLine(
                                    pen,
                                    bounds.X + bounds.Width - 1 - i,
                                    rightLineTops[i],
                                    bounds.X + bounds.Width - 1 - i,
                                    rightLineBottoms[i]);
                            }
                        }
                        break;
                    }
                case ButtonBorderStyle.Inset:
                case ButtonBorderStyle.Outset:
                    {
                        HLSColor hlsColor = new HLSColor(rightColor);
                        float inc = InfinityToOne(1.0f / (rightWidth - 1));
                        using var hdc = new DeviceContextHdcScope(deviceContext);
                        for (int i = 0; i < rightWidth; i++)
                        {
                            using var hpen = new Gdi32.CreatePenScope(rightStyle != ButtonBorderStyle.Inset
                                ? hlsColor.Darker(1.0f - i * inc)
                                : hlsColor.Lighter(1.0f - i * inc));

                            // Need to add one to the destination point for GDI to render the same as GDI+
                            hdc.DrawLine(hpen,
                                bounds.X + bounds.Width - 1 - i,
                                rightLineTops[i],
                                bounds.X + bounds.Width - 1 - i,
                                rightLineBottoms[i] + 1);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        ///  Draws a 3D style border at the given rectangle. The default 3D style of Etched is used.
        /// </summary>
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle)
            => DrawBorder3D(
                graphics,
                rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                Border3DStyle.Etched,
                Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);

        /// <summary>
        ///  Draws a 3D style border at the given rectangle. You may specify the style of the 3D appearance.
        /// </summary>
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style)
            => DrawBorder3D(
                graphics,
                rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                style,
                Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);

        /// <summary>
        ///  Draws a 3D style border at the given rectangle. You may specify the style of the 3D appearance, and which
        ///  sides of the 3D rectangle you wish to draw.
        /// </summary>
        public static void DrawBorder3D(Graphics graphics, Rectangle rectangle, Border3DStyle style, Border3DSide sides)
            => DrawBorder3D(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, style, sides);

        /// <summary>
        ///  Draws a 3D style border at the given rectangle. The default 3D style of ETCHED is used.
        /// </summary>
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height)
            => DrawBorder3D(
                graphics,
                x, y, width, height,
                Border3DStyle.Etched,
                Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);

        /// <summary>
        ///  Draws a 3D style border at the given rectangle. You may specify the style of the 3D appearance.
        /// </summary>
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style)
            => DrawBorder3D(
                graphics,
                x, y, width, height, style,
                Border3DSide.Left | Border3DSide.Top | Border3DSide.Right | Border3DSide.Bottom);

        /// <summary>
        ///  Draws a 3D style border at the given rectangle. You may specify the style of the 3D appearance, and which
        ///  sides of the 3D rectangle you wish to draw.
        /// </summary>
        public static void DrawBorder3D(Graphics graphics, int x, int y, int width, int height, Border3DStyle style, Border3DSide sides)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            User32.EDGE edge = (User32.EDGE)((uint)style & 0x0F);
            User32.BF flags = (User32.BF)sides | (User32.BF)((uint)style & ~0x0F);

            RECT rc = new Rectangle(x, y, width, height);

            // Windows just draws the border to size, and then shrinks the rectangle so the user can paint the client
            // area. We can't really do that, so we do the opposite: We precalculate the size of the border and enlarge
            // the rectangle so the client size is preserved.
            if ((flags & (User32.BF)Border3DStyle.Adjust) == (User32.BF)Border3DStyle.Adjust)
            {
                Size sz = SystemInformation.Border3DSize;
                rc.left -= sz.Width;
                rc.right += sz.Width;
                rc.top -= sz.Height;
                rc.bottom += sz.Height;
                flags &= ~(User32.BF)Border3DStyle.Adjust;
            }

            // Get Win32 dc with Graphics properties applied to it.
            using var hdc = new DeviceContextHdcScope(graphics);
            User32.DrawEdge(hdc, ref rc, edge, flags);
        }

        /// <summary>
        ///  Helper function that draws a more complex border. This is used by DrawBorder for less common
        ///  rendering cases. We split DrawBorder into DrawBorderSimple and DrawBorderComplex so we maximize
        ///  the % of the function call. It is less performant to have large functions that do many things.
        /// </summary>
        private static void DrawBorderComplex(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            if (style == ButtonBorderStyle.Inset)
            {
                // Button being pushed
                HLSColor hls = new HLSColor(color);

                // Top + left
                using var darkPen = hls.Darker(1.0f).GetCachedPenScope();
                graphics.DrawLine(darkPen, bounds.X, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y);
                graphics.DrawLine(darkPen, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1);

                // Bottom + right
                using var lightPen = hls.Lighter(1.0f).GetCachedPenScope();
                graphics.DrawLine(
                    lightPen,
                    bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
                graphics.DrawLine(
                    lightPen,
                    bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);

                // Top + left inset
                using var mediumPen = hls.Lighter(0.5f).GetCachedPenScope();
                graphics.DrawLine(mediumPen, bounds.X + 1, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + 1);
                graphics.DrawLine(mediumPen, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Y + bounds.Height - 2);

                // Bottom + right inset
                if (color.ToKnownColor() == SystemColors.Control.ToKnownColor())
                {
                    Pen pen = SystemPens.ControlLight;
                    graphics.DrawLine(
                        pen,
                        bounds.X + 1, bounds.Y + bounds.Height - 2, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                    graphics.DrawLine(
                        pen,
                        bounds.X + bounds.Width - 2, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                }
            }
            else
            {
                // Standard button
                Debug.Assert(style == ButtonBorderStyle.Outset, "Caller should have known how to use us.");

                bool stockColor = color.ToKnownColor() == SystemColors.Control.ToKnownColor();
                HLSColor hls = new HLSColor(color);

                // Top + left
                using var lightPen = (stockColor ? SystemColors.ControlLightLight : hls.Lighter(1.0f)).GetCachedPenScope();
                graphics.DrawLine(lightPen, bounds.X, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y);
                graphics.DrawLine(lightPen, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1);

                // Bottom + right
                using var darkPen = (stockColor ? SystemColors.ControlDarkDark : hls.Darker(1.0f)).GetCachedPenScope();

                graphics.DrawLine(
                    darkPen,
                    bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
                graphics.DrawLine(
                    darkPen,
                    bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);

                // Top + left inset
                using var topLeftPen = (!stockColor
                    ? color
                    : SystemInformation.HighContrast
                        ? SystemColors.ControlLightLight
                        : SystemColors.Control).GetCachedPenScope();

                graphics.DrawLine(topLeftPen, bounds.X + 1, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + 1);
                graphics.DrawLine(topLeftPen, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Y + bounds.Height - 2);

                // Bottom + right inset
                using var bottomRightPen = (stockColor ? SystemColors.ControlDark : hls.Darker(0.5f)).GetCachedPenScope();

                graphics.DrawLine(
                    bottomRightPen,
                    bounds.X + 1, bounds.Y + bounds.Height - 2, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
                graphics.DrawLine(
                    bottomRightPen,
                    bounds.X + bounds.Width - 2, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
            }
        }

        internal static void DrawBorderSimple(
            IDeviceContext context,
            Rectangle bounds,
            Color color,
            ButtonBorderStyle style = ButtonBorderStyle.Solid)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (color.HasTransparency() || style != ButtonBorderStyle.Solid)
            {
                // GDI+ right and bottom DrawRectangle border are 1 greater than GDI
                bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

                Graphics graphics = context.TryGetGraphics(create: true);
                if (graphics != null)
                {
                    if (style == ButtonBorderStyle.Solid)
                    {
                        using var pen = color.GetCachedPenScope();
                        graphics.DrawRectangle(pen, bounds);
                    }
                    else
                    {
                        using var pen = color.CreateStaticPen(BorderStyleToDashStyle(style));
                        graphics.DrawRectangle(pen, bounds);
                    }

                    return;
                }
            }

            using var hdc = new DeviceContextHdcScope(context);
            using var hpen = new Gdi32.CreatePenScope(color);
            hdc.DrawRectangle(bounds, hpen);
        }

        /// <summary>
        ///  Draws a Win32 button control in the given rectangle with the given state.
        /// </summary>
        public static void DrawButton(Graphics graphics, Rectangle rectangle, ButtonState state)
            => DrawButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        /// <summary>
        ///  Draws a Win32 button control in the given rectangle with the given state.
        /// </summary>
        public static void DrawButton(Graphics graphics, int x, int y, int width, int height, ButtonState state)
        {
            DrawFrameControl(
                graphics,
                x, y, width, height,
                User32.DFC.BUTTON,
                User32.DFCS.BUTTONPUSH | (User32.DFCS)state,
                Color.Empty,
                Color.Empty);
        }

        /// <summary>
        ///  Draws a Win32 window caption button in the given rectangle with the given state.
        /// </summary>
        public static void DrawCaptionButton(
            Graphics graphics,
            Rectangle rectangle,
            CaptionButton button,
            ButtonState state) => DrawCaptionButton(
                graphics,
                rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                button,
                state);

        /// <summary>
        ///  Draws a Win32 window caption button in the given rectangle with the given state.
        /// </summary>
        public static void DrawCaptionButton(
            Graphics graphics,
            int x, int y, int width, int height,
            CaptionButton button,
            ButtonState state) => DrawFrameControl(
                graphics,
                x, y, width, height,
                User32.DFC.CAPTION,
                (User32.DFCS)button | (User32.DFCS)state,
                Color.Empty,
                Color.Empty);

        /// <summary>
        ///  Draws a Win32 checkbox control in the given rectangle with the given state.
        /// </summary>
        public static void DrawCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
            => DrawCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        /// <summary>
        ///  Draws a Win32 checkbox control in the given rectangle with the given state.
        /// </summary>
        public static void DrawCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state)
        {
            // We overwrite the windows checkbox
            if ((state & ButtonState.Flat) == ButtonState.Flat)
            {
                DrawFlatCheckBox(graphics, new Rectangle(x, y, width, height), state);
            }
            else
            {
                DrawFrameControl(
                    graphics,
                    x, y, width, height,
                    User32.DFC.BUTTON,
                    User32.DFCS.BUTTONCHECK | (User32.DFCS)state,
                    Color.Empty,
                    Color.Empty);
            }
        }

        /// <summary>
        ///  Draws the drop down button of a Win32 combo box in the given rectangle with the given state.
        /// </summary>
        public static void DrawComboButton(Graphics graphics, Rectangle rectangle, ButtonState state)
            => DrawComboButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        /// <summary>
        ///  Draws the drop down button of a Win32 combo box in the given rectangle with the given state.
        /// </summary>
        public static void DrawComboButton(Graphics graphics, int x, int y, int width, int height, ButtonState state)
            => DrawFrameControl(
                graphics,
                x, y, width, height,
                User32.DFC.SCROLL,
                User32.DFCS.SCROLLCOMBOBOX | (User32.DFCS)state,
                Color.Empty,
                Color.Empty);

        /// <summary>
        ///  Draws a container control grab handle glyph inside the given rectangle.
        /// </summary>
        public static void DrawContainerGrabHandle(Graphics graphics, Rectangle bounds)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            Brush brush = Brushes.White;
            Pen pen = Pens.Black;

            graphics.FillRectangle(brush, bounds.Left + 1, bounds.Top + 1, bounds.Width - 2, bounds.Height - 2);

            // Draw the bounding rect w/o the four corners
            graphics.DrawLine(pen, bounds.X + 1, bounds.Y, bounds.Right - 2, bounds.Y);
            graphics.DrawLine(pen, bounds.X + 1, bounds.Bottom - 1, bounds.Right - 2, bounds.Bottom - 1);
            graphics.DrawLine(pen, bounds.X, bounds.Y + 1, bounds.X, bounds.Bottom - 2);
            graphics.DrawLine(pen, bounds.Right - 1, bounds.Y + 1, bounds.Right - 1, bounds.Bottom - 2);

            int midx = bounds.X + bounds.Width / 2;
            int midy = bounds.Y + bounds.Height / 2;

            // Vertical line
            graphics.DrawLine(pen, midx, bounds.Y, midx, bounds.Bottom - 2);

            // Horizontal line
            graphics.DrawLine(pen, bounds.X, midy, bounds.Right - 2, midy);

            // Top hash
            graphics.DrawLine(pen, midx - 1, bounds.Y + 2, midx + 1, bounds.Y + 2);
            graphics.DrawLine(pen, midx - 2, bounds.Y + 3, midx + 2, bounds.Y + 3);

            // Left hash
            graphics.DrawLine(pen, bounds.X + 2, midy - 1, bounds.X + 2, midy + 1);
            graphics.DrawLine(pen, bounds.X + 3, midy - 2, bounds.X + 3, midy + 2);

            // Right hash
            graphics.DrawLine(pen, bounds.Right - 3, midy - 1, bounds.Right - 3, midy + 1);
            graphics.DrawLine(pen, bounds.Right - 4, midy - 2, bounds.Right - 4, midy + 2);

            // Bottom hash
            graphics.DrawLine(pen, midx - 1, bounds.Bottom - 3, midx + 1, bounds.Bottom - 3);
            graphics.DrawLine(pen, midx - 2, bounds.Bottom - 4, midx + 2, bounds.Bottom - 4);
        }

        /// <summary>
        ///  Draws a flat checkbox.
        /// </summary>
        private static void DrawFlatCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            // Background color of checkbox

            Brush background = ((state & ButtonState.Inactive) == ButtonState.Inactive)
                ? SystemBrushes.Control
                : SystemBrushes.Window;
            Color foreground = ((state & ButtonState.Inactive) == ButtonState.Inactive)
                ? (SystemInformation.HighContrast ? SystemColors.GrayText : SystemColors.ControlDark)
                : SystemColors.ControlText;
            DrawFlatCheckBox(graphics, rectangle, foreground, background, state);
        }

        /// <summary>
        ///  Draws a Win32 checkbox control in the given rectangle with the given state. This draws a flat looking
        ///  check box that is suitable for use in list boxes, etc. We custom draw this as we want a better looking
        ///  render than Windows provides.
        /// </summary>
        private static void DrawFlatCheckBox(
            Graphics graphics,
            Rectangle rectangle,
            Color foreground,
            Brush background,
            ButtonState state)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));
            if (rectangle.Width < 0 || rectangle.Height < 0)
                throw new ArgumentOutOfRangeException(nameof(rectangle));

            Rectangle offsetRectangle = new Rectangle(
                rectangle.X + 1,
                rectangle.Y + 1,
                rectangle.Width - 2,
                rectangle.Height - 2);

            graphics.FillRectangle(background, offsetRectangle);

            // Checkmark

            if ((state & ButtonState.Checked) == ButtonState.Checked)
            {
                if (t_checkImage is null || t_checkImage.Width != rectangle.Width || t_checkImage.Height != rectangle.Height)
                {
                    if (t_checkImage != null)
                    {
                        t_checkImage.Dispose();
                        t_checkImage = null;
                    }

                    // We draw the checkmark slightly off center to eliminate 3-D border artifacts,
                    // and compensate below
                    RECT rcCheck = new RECT(0, 0, rectangle.Width, rectangle.Height);
                    Bitmap bitmap = new Bitmap(rectangle.Width, rectangle.Height);
                    using (Graphics g2 = Graphics.FromImage(bitmap))
                    {
                        g2.Clear(Color.Transparent);
                        using var dc = new DeviceContextHdcScope(g2, applyGraphicsState: false);
                        User32.DrawFrameControl(dc, ref rcCheck, User32.DFC.MENU, User32.DFCS.MENUCHECK);
                    }

                    bitmap.MakeTransparent();
                    t_checkImage = bitmap;
                }

                rectangle.X += 1;
                DrawImageColorized(graphics, t_checkImage, rectangle, foreground);
                rectangle.X -= 1;
            }

            // Surrounding border. We inset this by one pixel so we match how the 3D checkbox is drawn.

            Pen pen = SystemPens.ControlDark;
            graphics.DrawRectangle(pen, offsetRectangle.X, offsetRectangle.Y, offsetRectangle.Width - 1, offsetRectangle.Height - 1);
        }

        /// <summary>
        ///  Draws a focus rectangle. A focus rectangle is a dotted rectangle that Windows  uses to indicate what
        ///  control has the current keyboard focus.
        /// </summary>
        public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle)
            => DrawFocusRectangle(graphics, rectangle, SystemColors.ControlText, SystemColors.Control);

        /// <summary>
        ///  Draws a focus rectangle. A focus rectangle is a dotted rectangle that Windows uses to indicate what
        ///  control has the current keyboard focus.
        /// </summary>
        public static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor)
            => DrawFocusRectangle(graphics, rectangle, backColor, highContrast: false);

        internal static void DrawHighContrastFocusRectangle(Graphics graphics, Rectangle rectangle, Color color)
            => DrawFocusRectangle(graphics, rectangle, color, highContrast: true);

        private static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color color, bool highContrast)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            rectangle.Width--;
            rectangle.Height--;
            graphics.DrawRectangle(
                // We want the corner to be penned see GetFocusPen for more explanation
                GetFocusPen(color, (rectangle.X + rectangle.Y) % 2 == 1, highContrast),
                rectangle);
        }

        /// <summary>
        ///  Draws a win32 frame control.
        /// </summary>
        private static void DrawFrameControl(
            Graphics graphics,
            int x, int y, int width, int height,
            User32.DFC kind,
            User32.DFCS state,
            Color foreColor,
            Color backColor)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            RECT rcFrame = new RECT(0, 0, width, height);
            using Bitmap bitmap = new Bitmap(width, height);
            using Graphics g2 = Graphics.FromImage(bitmap);
            g2.Clear(Color.Transparent);

            using (var hdc = new DeviceContextHdcScope(g2, applyGraphicsState: false))
            {
                // Get Win32 dc with Graphics properties applied to it.
                User32.DrawFrameControl(hdc, ref rcFrame, kind, state);
            }

            if (foreColor == Color.Empty || backColor == Color.Empty)
            {
                graphics.DrawImage(bitmap, x, y);
            }
            else
            {
                // Replace black/white with foreColor/backColor.
                ImageAttributes attrs = new ImageAttributes();
                ColorMap cm1 = new ColorMap
                {
                    OldColor = Color.Black,
                    NewColor = foreColor
                };

                ColorMap cm2 = new ColorMap
                {
                    OldColor = Color.White,
                    NewColor = backColor
                };

                attrs.SetRemapTable(new ColorMap[2] { cm1, cm2 }, ColorAdjustType.Bitmap);
                graphics.DrawImage(
                    bitmap,
                    new Rectangle(x, y, width, height),
                    0, 0, width, height,
                    GraphicsUnit.Pixel,
                    attrs,
                    null,
                    IntPtr.Zero);
            }
        }

        /// <summary>
        ///  Draws a standard selection grab handle with the given dimensions. Grab
        ///  handles are used by components to indicate to the user that they can
        ///  be directly maniupulated.
        /// </summary>
        public static void DrawGrabHandle(Graphics graphics, Rectangle rectangle, bool primary, bool enabled)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            Pen pen = primary
                ? s_grabPenPrimary ??= Pens.Black
                : s_grabPenSecondary ??= Pens.White;

            Brush brush = primary
                ? enabled ? (s_grabBrushPrimary ??= Brushes.White) : SystemBrushes.Control
                : enabled ? (s_grabBrushSecondary ??= Brushes.Black) : SystemBrushes.Control;

            Rectangle fillRect = new Rectangle(
                rectangle.X + 1,
                rectangle.Y + 1,
                rectangle.Width - 1,
                rectangle.Height - 1);

            graphics.FillRectangle(brush, fillRect);
            rectangle.Width--;
            rectangle.Height--;
            graphics.DrawRectangle(pen, rectangle);
        }

        /// <summary>
        ///  Draws a grid of one pixel dots in the given rectangle.
        /// </summary>
        public static void DrawGrid(Graphics graphics, Rectangle area, Size pixelsBetweenDots, Color backColor)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));
            if (pixelsBetweenDots.Width <= 0 || pixelsBetweenDots.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(pixelsBetweenDots));

            float intensity = backColor.GetBrightness();
            bool invert = (intensity < .5);

            if (t_gridBrush is null || s_gridSize.Width != pixelsBetweenDots.Width
                || s_gridSize.Height != pixelsBetweenDots.Height || invert != s_gridInvert)
            {
                if (t_gridBrush != null)
                {
                    t_gridBrush.Dispose();
                    t_gridBrush = null;
                }

                s_gridSize = pixelsBetweenDots;
                int idealSize = 16;
                s_gridInvert = invert;
                Color foreColor = (s_gridInvert) ? Color.White : Color.Black;

                // Round size to a multiple of pixelsBetweenDots
                int width = ((idealSize / pixelsBetweenDots.Width) + 1) * pixelsBetweenDots.Width;
                int height = ((idealSize / pixelsBetweenDots.Height) + 1) * pixelsBetweenDots.Height;

                using Bitmap bitmap = new Bitmap(width, height);

                // draw the dots
                for (int x = 0; x < width; x += pixelsBetweenDots.Width)
                {
                    for (int y = 0; y < height; y += pixelsBetweenDots.Height)
                    {
                        bitmap.SetPixel(x, y, foreColor);
                    }
                }

                t_gridBrush = new TextureBrush(bitmap);
            }

            graphics.FillRectangle(t_gridBrush, area);
        }

        // Takes a black and transparent image, turns black pixels into some other color, and leaves transparent pixels alone
        internal static void DrawImageColorized(
            Graphics graphics,
            Image image,
            Rectangle destination,
            Color replaceBlack)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            using var attributes = new ImageAttributes();
            attributes.SetColorMatrix(RemapBlackAndWhitePreserveTransparentMatrix(replaceBlack, Color.White));
            graphics.DrawImage(
                image,
                destination,
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel,
                attributes,
                null,
                IntPtr.Zero);
        }

        internal static bool IsImageTransparent(Image backgroundImage)
            => backgroundImage != null && (backgroundImage.Flags & (int)ImageFlags.HasAlpha) > 0;

        // takes an image and replaces all the pixels of oldColor with newColor, drawing the new image into the rectangle on
        // the supplied Graphics object.
        internal static void DrawImageReplaceColor(Graphics g, Image image, Rectangle dest, Color oldColor, Color newColor)
        {
            ImageAttributes attrs = new ImageAttributes();

            ColorMap cm = new ColorMap
            {
                OldColor = oldColor,
                NewColor = newColor
            };

            attrs.SetRemapTable(new ColorMap[] { cm }, ColorAdjustType.Bitmap);

            g.DrawImage(image, dest, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attrs, null, IntPtr.Zero);
            attrs.Dispose();
        }

        /// <summary>
        ///  Draws an image and makes it look disabled.
        /// </summary>
#pragma warning disable IDE0060 // Remove unused parameter- public API
        public static void DrawImageDisabled(Graphics graphics, Image image, int x, int y, Color background)
#pragma warning restore IDE0060
            => DrawImageDisabled(graphics, image, new Rectangle(x, y, image.Width, image.Height), unscaledImage: false);

        /// <summary>
        ///  Draws an image and makes it look disabled.
        /// </summary>
        internal static void DrawImageDisabled(Graphics graphics, Image image, Rectangle imageBounds, bool unscaledImage)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));
            if (image is null)
                throw new ArgumentNullException(nameof(image));

            Size imageSize = image.Size;

            if (t_disabledImageAttr is null)
            {
                // This ColorMatrix is set up to resemble Office 10 commandbars, but still be able to deal with
                // hi-color (256+) icons and images.
                //
                // The idea is to scale everything down (more than just a grayscale does, therefore the small numbers
                // in the scaling part of matrix). White becomes some shade of gray and black stays black.
                //
                // Second part of the matrix is to translate everything, so all colors are a bit brigher. Grays become
                // lighter and washed out looking black becomes a shade of gray as well.

                float[][] array = new float[5][];
                array[0] = new float[5] { 0.2125f, 0.2125f, 0.2125f, 0, 0 };
                array[1] = new float[5] { 0.2577f, 0.2577f, 0.2577f, 0, 0 };
                array[2] = new float[5] { 0.0361f, 0.0361f, 0.0361f, 0, 0 };
                array[3] = new float[5] { 0, 0, 0, 1, 0 };
                array[4] = new float[5] { 0.38f, 0.38f, 0.38f, 0, 1 };

                ColorMatrix grayMatrix = new ColorMatrix(array);

                t_disabledImageAttr = new ImageAttributes();
                t_disabledImageAttr.ClearColorKey();
                t_disabledImageAttr.SetColorMatrix(grayMatrix);
            }

            if (unscaledImage)
            {
                using Bitmap bmp = new Bitmap(image.Width, image.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(
                        image,
                        new Rectangle(0, 0, imageSize.Width, imageSize.Height),
                        0, 0, imageSize.Width, imageSize.Height,
                        GraphicsUnit.Pixel,
                        t_disabledImageAttr);
                }
                graphics.DrawImageUnscaled(bmp, imageBounds);
            }
            else
            {
                graphics.DrawImage(
                    image,
                    imageBounds,
                    0, 0, imageSize.Width, imageSize.Height,
                    GraphicsUnit.Pixel,
                    t_disabledImageAttr);
            }
        }

        /// <summary>
        ///  Draws a locked selection frame around the given rectangle.
        /// </summary>
        public static void DrawLockedFrame(Graphics graphics, Rectangle rectangle, bool primary)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            Pen pen = primary ? Pens.White : Pens.Black;

            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
            rectangle.Inflate(-1, -1);
            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);

            pen = primary ? Pens.Black : Pens.White;
            rectangle.Inflate(-1, -1);
            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
        }

        /// <summary>
        ///  Draws a menu glyph for a Win32 menu in the given rectangle with the given state.
        /// </summary>
        public static void DrawMenuGlyph(Graphics graphics, Rectangle rectangle, MenuGlyph glyph)
            => DrawMenuGlyph(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, glyph);

        /// <summary>
        ///  Draws a menu glyph for a Win32 menu in the given rectangle with the given state. White color is replaced
        ///  with backColor, Black is replaced with foreColor.
        /// </summary>
        public static void DrawMenuGlyph(
            Graphics graphics,
            Rectangle rectangle,
            MenuGlyph glyph,
            Color foreColor,
            Color backColor) => DrawMenuGlyph(
                graphics,
                rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height,
                glyph,
                foreColor,
                backColor);

        /// <summary>
        ///  Draws a menu glyph for a Win32 menu in the given rectangle with the given state.
        /// </summary>
        public static void DrawMenuGlyph(Graphics graphics, int x, int y, int width, int height, MenuGlyph glyph)
            => DrawFrameControl(graphics, x, y, width, height, User32.DFC.MENU, (User32.DFCS)glyph, Color.Empty, Color.Empty);

        /// <summary>
        ///  Draws a menu glyph for a Win32 menu in the given rectangle with the given state. White color is replaced
        ///  with backColor, Black is replaced with foreColor.
        /// </summary>
        public static void DrawMenuGlyph(
            Graphics graphics,
            int x, int y, int width, int height,
            MenuGlyph glyph,
            Color foreColor,
            Color backColor) => DrawFrameControl(
                graphics,
                x, y, width, height,
                User32.DFC.MENU,
                (User32.DFCS)glyph,
                foreColor,
                backColor);

        /// <summary>
        ///  Draws a Win32 3-state checkbox control in the given rectangle with the given state.
        /// </summary>
        public static void DrawMixedCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
            => DrawMixedCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        public static void DrawMixedCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state)
        {
            DrawFrameControl(
                graphics,
                x, y, width, height,
                User32.DFC.BUTTON,
                User32.DFCS.BUTTON3STATE | (User32.DFCS)state,
                Color.Empty,
                Color.Empty);
        }

        /// <summary>
        ///  Draws a Win32 radio button in the given rectangle with the given state.
        /// </summary>
        public static void DrawRadioButton(Graphics graphics, Rectangle rectangle, ButtonState state)
            => DrawRadioButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);

        /// <summary>
        ///  Draws a Win32 radio button in the given rectangle with the given state.
        /// </summary>
        public static void DrawRadioButton(Graphics graphics, int x, int y, int width, int height, ButtonState state)
        {
            DrawFrameControl(
                graphics,
                x, y, width, height,
                User32.DFC.BUTTON,
                User32.DFCS.BUTTONRADIO | (User32.DFCS)state,
                Color.Empty,
                Color.Empty);
        }

        /// <summary>
        ///  Draws a rectangular frame on the screen. The operation of this can be "reversed" by drawing the same
        ///  rectangle again. This is similar to inverting a region of the screen except that it behaves better for
        ///  a wider variety of colors.
        /// </summary>
        public static void DrawReversibleFrame(Rectangle rectangle, Color backColor, FrameStyle style)
        {
            Gdi32.R2 rop2;
            Color graphicsColor;

            if (backColor.GetBrightness() < .5)
            {
                rop2 = Gdi32.R2.NOTXORPEN;
                graphicsColor = Color.White;
            }
            else
            {
                rop2 = Gdi32.R2.XORPEN;
                graphicsColor = Color.Black;
            }

            using var desktopDC = new User32.GetDcScope(
                User32.GetDesktopWindow(),
                IntPtr.Zero,
                User32.DCX.WINDOW | User32.DCX.LOCKWINDOWUPDATE | User32.DCX.CACHE);

            using var pen = new Gdi32.ObjectScope(style switch
            {
                FrameStyle.Dashed => Gdi32.CreatePen(Gdi32.PS.DOT, 1, ColorTranslator.ToWin32(backColor)),
                FrameStyle.Thick => Gdi32.CreatePen(Gdi32.PS.SOLID, 2, ColorTranslator.ToWin32(backColor)),
                _ => default
            });

            using var rop2Scope = new Gdi32.SetRop2Scope(desktopDC, rop2);
            using var brushSelection = new Gdi32.SelectObjectScope(desktopDC, Gdi32.GetStockObject(Gdi32.StockObject.NULL_BRUSH));
            using var penSelection = new Gdi32.SelectObjectScope(desktopDC, pen);

            Gdi32.SetBkColor(desktopDC, ColorTranslator.ToWin32(graphicsColor));
            Gdi32.Rectangle(desktopDC, rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
        }

        /// <summary>
        ///  Draws a reversible line on the screen. A reversible line can be erased by just drawing over it again.
        /// </summary>
        public static unsafe void DrawReversibleLine(Point start, Point end, Color backColor)
        {
            Gdi32.R2 rop2 = (Gdi32.R2)GetColorRop(backColor, (int)Gdi32.R2.NOTXORPEN, (int)Gdi32.R2.XORPEN);

            using var desktopDC = new User32.GetDcScope(
                User32.GetDesktopWindow(),
                IntPtr.Zero,
                User32.DCX.WINDOW | User32.DCX.LOCKWINDOWUPDATE | User32.DCX.CACHE);

            using var pen = new Gdi32.ObjectScope(Gdi32.CreatePen(Gdi32.PS.SOLID, 1, ColorTranslator.ToWin32(backColor)));
            using var ropScope = new Gdi32.SetRop2Scope(desktopDC, rop2);
            using var brushSelection = new Gdi32.SelectObjectScope(
                desktopDC,
                Gdi32.GetStockObject(Gdi32.StockObject.NULL_BRUSH));
            using var penSelection = new Gdi32.SelectObjectScope(desktopDC, pen);

            Gdi32.MoveToEx(desktopDC, start.X, start.Y, null);
            Gdi32.LineTo(desktopDC, end.X, end.Y);
        }

        /// <summary>
        ///  Draws a button for a Win32 scroll bar in the given rectangle with the given state.
        /// </summary>
        public static void DrawScrollButton(Graphics graphics, Rectangle rectangle, ScrollButton button, ButtonState state)
            => DrawScrollButton(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, button, state);

        /// <summary>
        ///  Draws a button for a Win32 scroll bar in the given rectangle with the given state.
        /// </summary>
        public static void DrawScrollButton(
            Graphics graphics,
            int x, int y, int width, int height,
            ScrollButton button,
            ButtonState state) => DrawFrameControl(
                graphics,
                x, y, width, height,
                User32.DFC.SCROLL,
                (User32.DFCS)button | (User32.DFCS)state,
                Color.Empty,
                Color.Empty);

        /// <summary>
        ///  Draws a standard selection frame. A selection frame is a frame that is
        ///  drawn around a selected component at design time.
        /// </summary>
        public static void DrawSelectionFrame(
            Graphics graphics,
            bool active,
            Rectangle outsideRect,
            Rectangle insideRect,
            Color backColor)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            Brush frameBrush = active ? GetActiveBrush(backColor) : GetSelectedBrush(backColor);

            Region clip = graphics.Clip;
            graphics.ExcludeClip(insideRect);
            graphics.FillRectangle(frameBrush, outsideRect);
            graphics.Clip = clip;
        }

        /// <summary>
        ///  Draws a size grip at the given location. The color of the size grip is based
        ///  on the given background color.
        /// </summary>
        public static void DrawSizeGrip(Graphics graphics, Color backColor, Rectangle bounds)
            => DrawSizeGrip(graphics, backColor, bounds.X, bounds.Y, bounds.Width, bounds.Height);

        /// <summary>
        ///  Draws a size grip at the given location. The color of the size grip is based on the given background color.
        /// </summary>
        public static void DrawSizeGrip(Graphics graphics, Color backColor, int x, int y, int width, int height)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            DrawSizeGrip((IDeviceContext)graphics, backColor, x, y, width, height);
        }

        internal static void DrawSizeGrip(
            IDeviceContext deviceContext,
            Color backColor,
            int x, int y, int width, int height)
        {
            // Note: We don't paint any background to facilitate transparency, background images, etc...

            // We only draw rectangular grips.
            int size = Math.Min(width, height);

            // Start one pixel in from the right and two up from the bottom
            int right = x + width - 1;
            int bottom = y + height - 2;

            using var hdc = new DeviceContextHdcScope(deviceContext);
            using var hpenBright = new Gdi32.CreatePenScope(LightLight(backColor));
            using var hpenDark = new Gdi32.CreatePenScope(Dark(backColor));

            // Moving from the lower right corner, draw as many groups of 4 diagonal lines as will fit
            // (skip a line, dark, dark, light)

            for (int i = 0; i < size - 4; i += 4)
            {
                hdc.DrawLine(hpenDark, right - (i + 1) - 2, bottom, right + 1, bottom - (i + 1) - 3);
                hdc.DrawLine(hpenDark, right - (i + 2) - 2, bottom, right + 1, bottom - (i + 2) - 3);
                hdc.DrawLine(hpenBright, right - (i + 3) - 2, bottom, right + 1, bottom - (i + 3) - 3);
            }
        }

        /// <summary>
        ///  Draws a string in the style appropriate for disabled items.
        /// </summary>
        public static void DrawStringDisabled(
            Graphics graphics,
            string s,
            Font font,
            Color color,
            RectangleF layoutRectangle,
            StringFormat format)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            if (SystemInformation.HighContrast)
            {
                // Ignore the foreground color argument and don't do shading in high contrast,
                // as colors should match the OS-defined ones.
                graphics.DrawString(s, font, SystemBrushes.GrayText, layoutRectangle, format);
            }
            else
            {
                layoutRectangle.Offset(1, 1);
                using var lightBrush = LightLight(color).GetCachedSolidBrushScope();
                graphics.DrawString(s, font, lightBrush, layoutRectangle, format);

                layoutRectangle.Offset(-1, -1);
                using var darkBrush = Dark(color).GetCachedSolidBrushScope();
                color = Dark(color);
                graphics.DrawString(s, font, darkBrush, layoutRectangle, format);
            }
        }

        /// <summary>
        ///  Draws a string in the style appropriate for disabled items, using GDI-based TextRenderer.
        /// </summary>
        public static void DrawStringDisabled(
            IDeviceContext dc,
            string s,
            Font font,
            Color color,
            Rectangle layoutRectangle,
            TextFormatFlags format)
        {
            if (dc is null)
                throw new ArgumentNullException(nameof(dc));

            // This must come before creating the scope.
            Gdi32.QUALITY quality = TextRenderer.FontQualityFromTextRenderingHint(dc);

            using var hdc = new DeviceContextHdcScope(dc);
            DrawStringDisabled(hdc, s, font, color, layoutRectangle, format, quality);
        }

        internal static void DrawStringDisabled(
            Gdi32.HDC dc,
            string s,
            Font font,
            Color color,
            Rectangle layoutRectangle,
            TextFormatFlags format,
            Gdi32.QUALITY quality = Gdi32.QUALITY.DEFAULT)
        {
            if (SystemInformation.HighContrast)
            {
                TextRenderer.DrawTextInternal(dc, s, font, layoutRectangle, SystemColors.GrayText, quality, format);
            }
            else
            {
                layoutRectangle.Offset(1, 1);
                Color paintcolor = LightLight(color);

                TextRenderer.DrawTextInternal(dc, s, font, layoutRectangle, paintcolor, quality, format);
                layoutRectangle.Offset(-1, -1);
                paintcolor = Dark(color);
                TextRenderer.DrawTextInternal(dc, s, font, layoutRectangle, paintcolor, quality, format);
            }
        }

        /// <summary>
        ///  Draws a string in the style appropriate for disabled items.
        /// </summary>
        public static void DrawVisualStyleBorder(Graphics graphics, Rectangle bounds)
        {
            if (graphics is null)
                throw new ArgumentNullException(nameof(graphics));

            using var borderPen = VisualStyles.VisualStyleInformation.TextControlBorder.GetCachedPenScope();
            graphics.DrawRectangle(borderPen, bounds);
        }

        /// <summary>
        ///  Draws a filled rectangle on the screen. The operation of this can be
        ///  "reversed" by drawing the same rectangle again. This is similar to
        ///  inverting a region of the screen except that it behaves better for
        ///  a wider variety of colors.
        /// </summary>
        public static void FillReversibleRectangle(Rectangle rectangle, Color backColor)
        {
            Gdi32.ROP rop3 = (Gdi32.ROP)GetColorRop(
                backColor,
                0xa50065,   // RasterOp.BRUSH.Invert().XorWith(RasterOp.TARGET),
                0x5a0049);  // RasterOp.BRUSH.XorWith(RasterOp.TARGET));
            Gdi32.R2 rop2 = Gdi32.R2.NOT;

            using var desktopDC = new User32.GetDcScope(
                User32.GetDesktopWindow(),
                IntPtr.Zero,
                User32.DCX.WINDOW | User32.DCX.LOCKWINDOWUPDATE | User32.DCX.CACHE);
            using var brush = new Gdi32.ObjectScope(Gdi32.CreateSolidBrush(ColorTranslator.ToWin32(backColor)));
            using var ropScope = new Gdi32.SetRop2Scope(desktopDC, rop2);
            using var brushSelection = new Gdi32.SelectObjectScope(desktopDC, brush);

            // PatBlt must be the only Win32 function that wants height in width rather than x2,y2.
            Gdi32.PatBlt(desktopDC, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rop3);
        }

        /// <summary>
        ///  Converts the font into one where Font.Unit == Point. If the original font is in device-dependent units
        ///  (and it usually is), we interpret the size relative to the screen. This is not really a general-purpose
        ///  function -- when used on something not obtained from ChooseFont, it may round away some precision.
        /// </summary>
        internal static Font FontInPoints(Font font)
            => new Font(
                font.FontFamily,
                font.SizeInPoints,
                font.Style,
                GraphicsUnit.Point,
                font.GdiCharSet,
                font.GdiVerticalFont);

        /// <summary>
        ///  Returns whether or not <paramref name="target"/> was changed.
        /// </summary>
        internal static bool FontToIFont(Font source, Ole32.IFont target)
        {
            bool changed = false;

            // We need to go through all the pain of the diff here because it looks like setting them all has different
            // results based on the order and each individual IFont implementor.

            string fontName = target.Name;
            if (!source.Name.Equals(fontName))
            {
                target.Name = source.Name;
                changed = true;
            }

            // This always seems to come back as the point size * 10000 (HIMETRIC?), regardless or ratio or mapping
            // mode despite what the documentation says.

            float fontSize = (float)target.Size / 10000;

            // Size must be in points
            float winformsSize = source.SizeInPoints;
            if (winformsSize != fontSize)
            {
                target.Size = (long)(winformsSize * 10000);
                changed = true;
            }

            User32.LOGFONTW logfont = User32.LOGFONTW.FromFont(source);

            short fontWeight = target.Weight;
            if (fontWeight != (short)logfont.lfWeight)
            {
                target.Weight = (short)logfont.lfWeight;
                changed = true;
            }

            bool fontBold = target.Bold.IsTrue();
            bool isBold = logfont.lfWeight >= Gdi32.FW.BOLD;
            if (fontBold != isBold)
            {
                target.Bold = isBold.ToBOOL();
                changed = true;
            }

            bool fontItalic = target.Italic.IsTrue();
            bool isItalic = logfont.lfItalic != 0;
            if (fontItalic != isItalic)
            {
                target.Italic = isItalic.ToBOOL();
                changed = true;
            }

            bool fontUnderline = target.Underline.IsTrue();
            bool isUnderline = logfont.lfUnderline != 0;
            if (fontUnderline != isUnderline)
            {
                target.Underline = isUnderline.ToBOOL();
                changed = true;
            }

            bool fontStrike = target.Strikethrough.IsTrue();
            bool isStrike = logfont.lfStrikeOut != 0;
            if (fontStrike != isStrike)
            {
                target.Strikethrough = isStrike.ToBOOL();
                changed = true;
            }

            short fontCharset = target.Charset;
            if (fontCharset != logfont.lfCharSet)
            {
                target.Charset = logfont.lfCharSet;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        ///  This makes a choice from a set of raster op codes, based on the color given. If the color is considered to
        ///  be "dark", the raster op provided by dark will be returned.
        /// </summary>
        private static int GetColorRop(Color color, int darkROP, int lightROP)
            => color.GetBrightness() < .5 ? darkROP : lightROP;

        /// <summary>
        ///  Retrieves the brush used to draw active objects.
        /// </summary>
        private static Brush GetActiveBrush(Color backColor)
        {
            Color brushColor = backColor.GetBrightness() <= .5 ? SystemColors.ControlLight : SystemColors.ControlDark;

            if (t_frameBrushActive is null || !s_frameColorActive.Equals(brushColor))
            {
                if (t_frameBrushActive != null)
                {
                    t_frameBrushActive.Dispose();
                    t_frameBrushActive = null;
                }

                s_frameColorActive = brushColor;

                int patternSize = 8;

                Bitmap bitmap = new Bitmap(patternSize, patternSize);

                // Bitmap does not initialize itself to be zero?

                for (int x = 0; x < patternSize; x++)
                {
                    for (int y = 0; y < patternSize; y++)
                    {
                        bitmap.SetPixel(x, y, Color.Transparent);
                    }
                }

                for (int y = 0; y < patternSize; y++)
                {
                    for (int x = -y; x < patternSize; x += 4)
                    {
                        if (x >= 0)
                        {
                            bitmap.SetPixel(x, y, brushColor);
                        }
                    }
                }

                t_frameBrushActive = new TextureBrush(bitmap);
                bitmap.Dispose();
            }

            return t_frameBrushActive;
        }

        /// <summary>
        ///  Retrieves the pen used to draw a focus rectangle around a control. The focus rectangle is typically drawn
        ///  when the control has keyboard focus.
        /// </summary>
        private static Pen GetFocusPen(Color baseColor, bool odds, bool highContrast)
        {
            if (t_focusPen is null
                || t_hcFocusPen != highContrast
                || (!highContrast && t_focusPenColor.GetBrightness() <= .5 && baseColor.GetBrightness() <= .5)
                || t_focusPenColor.ToArgb() != baseColor.ToArgb())
            {
                if (t_focusPen != null)
                {
                    t_focusPen.Dispose();
                    t_focusPen = null;
                    t_focusPenInvert.Dispose();
                    t_focusPenInvert = null;
                }

                t_focusPenColor = baseColor;
                t_hcFocusPen = highContrast;

                using Bitmap b = new Bitmap(2, 2);
                Color color1 = Color.Transparent;
                Color color2;
                if (highContrast)
                {
                    // In highcontrast mode "baseColor" itself is used as the focus pen color.
                    color2 = baseColor;
                }
                else
                {
                    // In non-highcontrast mode "baseColor" is used to calculate the focus pen colors. In this mode
                    // "baseColor" is expected to contain the background color of the control to do this calculation
                    // properly.
                    color2 = Color.Black;

                    if (baseColor.GetBrightness() <= .5)
                    {
                        color1 = color2;
                        color2 = baseColor.InvertColor();
                    }
                    else if (baseColor == Color.Transparent)
                    {
                        color1 = Color.White;
                    }
                }

                //  High contrast        Normal (dark)       Normal (light)     Normal (light, base transparent)
                //
                // | trnsp | black |    | black | invrt |   | base  | black |   | white | black |
                // | black | trnsp |    | invrt | black |   | black | base  |   | black | white |

                b.SetPixel(1, 0, color2);
                b.SetPixel(0, 1, color2);
                b.SetPixel(0, 0, color1);
                b.SetPixel(1, 1, color1);

                using (Brush brush = new TextureBrush(b))
                {
                    t_focusPen = brush.CreateStaticPen();
                }

                b.SetPixel(1, 0, color1);
                b.SetPixel(0, 1, color1);
                b.SetPixel(0, 0, color2);
                b.SetPixel(1, 1, color2);

                using (Brush brush = new TextureBrush(b))
                {
                    t_focusPenInvert = brush.CreateStaticPen();
                }
            }

            return odds ? t_focusPen : t_focusPenInvert;
        }

        /// <summary>
        ///  Retrieves the brush used to draw selected objects.
        /// </summary>
        private static Brush GetSelectedBrush(Color backColor)
        {
            Color brushColor = backColor.GetBrightness() <= .5 ? SystemColors.ControlLight : SystemColors.ControlDark;

            if (t_frameBrushSelected is null || !s_frameColorSelected.Equals(brushColor))
            {
                if (t_frameBrushSelected != null)
                {
                    t_frameBrushSelected.Dispose();
                    t_frameBrushSelected = null;
                }

                s_frameColorSelected = brushColor;

                int patternSize = 8;

                Bitmap bitmap = new Bitmap(patternSize, patternSize);

                // Bitmap does not initialize itself to be zero?

                for (int x = 0; x < patternSize; x++)
                {
                    for (int y = 0; y < patternSize; y++)
                    {
                        bitmap.SetPixel(x, y, Color.Transparent);
                    }
                }

                int start = 0;

                for (int x = 0; x < patternSize; x += 2)
                {
                    for (int y = start; y < patternSize; y += 2)
                    {
                        bitmap.SetPixel(x, y, brushColor);
                    }

                    start ^= 1;
                }

                t_frameBrushSelected = new TextureBrush(bitmap);
                bitmap.Dispose();
            }

            return t_frameBrushSelected;
        }

        /// <summary>
        ///  Converts an infinite value to "1".
        /// </summary>
        private static float InfinityToOne(float value)
            => value == float.NegativeInfinity || value == float.PositiveInfinity ? 1.0f : value;

        /// <summary>
        ///  Creates a new color that is a object of the given color.
        /// </summary>
        public static Color Light(Color baseColor, float percOfLightLight)
            => new HLSColor(baseColor).Lighter(percOfLightLight);

        /// <summary>
        ///  Creates a new color that is a object of the given color.
        /// </summary>
        public static Color Light(Color baseColor) => new HLSColor(baseColor).Lighter(0.5f);

        /// <summary>
        ///  Creates a new color that is a object of the given color.
        /// </summary>
        public static Color LightLight(Color baseColor) => new HLSColor(baseColor).Lighter(1.0f);

        /// <summary>
        ///  Multiply two 5x5 color matrices.
        /// </summary>
        internal static ColorMatrix MultiplyColorMatrix(float[][] matrix1, float[][] matrix2)
        {
            const int Size = 5;

            // Build up an empty 5x5 array for results.
            float[][] result = new float[Size][];
            for (int row = 0; row < Size; row++)
            {
                result[row] = new float[Size];
            }

            float[] column = new float[Size];
            for (int j = 0; j < Size; j++)
            {
                for (int k = 0; k < Size; k++)
                {
                    column[k] = matrix1[k][j];
                }

                for (int i = 0; i < Size; i++)
                {
                    float[] row = matrix2[i];
                    float s = 0;
                    for (int k = 0; k < Size; k++)
                    {
                        s += row[k] * column[k];
                    }
                    result[i][j] = s;
                }
            }

            return new ColorMatrix(result);
        }

        /// <summary>
        ///  Paint the border of a table.
        /// </summary>
        internal static void PaintTableControlBorder(
            TableLayoutPanelCellBorderStyle borderStyle,
            Graphics g,
            Rectangle bound)
        {
            int x = bound.X;
            int y = bound.Y;
            int right = bound.Right;
            int bottom = bound.Bottom;

            // Draw the outside bounding rectangle
            switch (borderStyle)
            {
                case TableLayoutPanelCellBorderStyle.None:
                case TableLayoutPanelCellBorderStyle.Single:
                    break;

                case TableLayoutPanelCellBorderStyle.Inset:
                case TableLayoutPanelCellBorderStyle.InsetDouble:
                    g.DrawLine(SystemPens.ControlDark, x, y, right - 1, y);
                    g.DrawLine(SystemPens.ControlDark, x, y, x, bottom - 1);
                    g.DrawLine(SystemPens.Window, right - 1, y, right - 1, bottom - 1);
                    g.DrawLine(SystemPens.Window, x, bottom - 1, right - 1, bottom - 1);
                    break;

                case TableLayoutPanelCellBorderStyle.Outset:
                case TableLayoutPanelCellBorderStyle.OutsetDouble:
                case TableLayoutPanelCellBorderStyle.OutsetPartial:
                    g.DrawLine(SystemPens.Window, x, y, right - 1, y);
                    g.DrawLine(SystemPens.Window, x, y, x, bottom - 1);
                    g.DrawLine(SystemPens.ControlDark, right - 1, y, right - 1, bottom - 1);
                    g.DrawLine(SystemPens.ControlDark, x, bottom - 1, right - 1, bottom - 1);
                    break;
            }
        }

        internal static void PaintTableCellBorder(TableLayoutPanelCellBorderStyle borderStyle, Graphics g, Rectangle bound)
        {
            // Paint the cell border
            switch (borderStyle)
            {
                case TableLayoutPanelCellBorderStyle.None:
                    break;

                case TableLayoutPanelCellBorderStyle.Single:
                    g.DrawRectangle(SystemPens.ControlDark, bound);
                    break;

                case TableLayoutPanelCellBorderStyle.Inset:
                    g.DrawLine(SystemPens.Window, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                    g.DrawLine(SystemPens.Window, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.ControlDark, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    break;

                case TableLayoutPanelCellBorderStyle.InsetDouble:
                    g.DrawRectangle(SystemPens.Control, bound);

                    // Draw the shadow
                    bound = new Rectangle(bound.X + 1, bound.Y + 1, bound.Width - 1, bound.Height - 1);
                    g.DrawLine(SystemPens.Window, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                    g.DrawLine(SystemPens.Window, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);

                    g.DrawLine(SystemPens.ControlDark, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    break;

                case TableLayoutPanelCellBorderStyle.Outset:
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.Window, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.Window, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    break;

                case TableLayoutPanelCellBorderStyle.OutsetDouble:
                case TableLayoutPanelCellBorderStyle.OutsetPartial:
                    g.DrawRectangle(SystemPens.Control, bound);

                    //draw the shadow
                    bound = new Rectangle(bound.X + 1, bound.Y + 1, bound.Width - 1, bound.Height - 1);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                    g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.Window, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                    g.DrawLine(SystemPens.Window, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);

                    break;
            }
        }

        /// <summary>
        ///  Takes a black and white image, and replaces those colors with the colors of your choice. The
        ///  <paramref name="replaceBlack"/> and <paramref name="replaceWhite"/> must have alpha = 255, because the
        ///  alpha value of the bitmap is preserved.
        /// </summary>
        private static ColorMatrix RemapBlackAndWhitePreserveTransparentMatrix(Color replaceBlack, Color replaceWhite)
        {
            Debug.Assert(replaceBlack.A == 255);
            Debug.Assert(replaceWhite.A == 255);

            // Normalize the colors to 1.0.

            float normBlackRed = replaceBlack.R / (float)255.0;
            float normBlackGreen = replaceBlack.G / (float)255.0;
            float normBlackBlue = replaceBlack.B / (float)255.0;

            float normWhiteRed = replaceWhite.R / (float)255.0;
            float normWhiteGreen = replaceWhite.G / (float)255.0;
            float normWhiteBlue = replaceWhite.B / (float)255.0;

            // Set up a matrix that will map white to replaceWhite and black to replaceBlack, using the source bitmap's
            // alpha value for the output
            //
            //                | -B  -B  -B   0   0 |
            //                |   r   g   b        |
            //                |                    |
            //                |  W   W   W   0   0 |
            //                |   r   g   b        |
            //                |                    |
            //  [ R G B A ] * |  0   0   0   0   0 | = [ R' G' B' A ]
            //                |                    |
            //                |                    |
            //                |  0   0   0   1   0 |
            //                |                    |
            //                |                    |
            //                |  B   B   B   0   1 |
            //                |   r   g   b        |

            ColorMatrix matrix = new ColorMatrix
            {
                Matrix00 = -normBlackRed,
                Matrix01 = -normBlackGreen,
                Matrix02 = -normBlackBlue,

                Matrix10 = normWhiteRed,
                Matrix11 = normWhiteGreen,
                Matrix12 = normWhiteBlue,

                Matrix33 = 1.0f,

                Matrix40 = normBlackRed,
                Matrix41 = normBlackGreen,
                Matrix42 = normBlackBlue,
                Matrix44 = 1.0f
            };

            return matrix;
        }

        internal static TextFormatFlags TextFormatFlagsForAlignmentGDI(ContentAlignment align)
        {
            TextFormatFlags output = new TextFormatFlags();
            output |= TranslateAlignmentForGDI(align);
            output |= TranslateLineAlignmentForGDI(align);
            return output;
        }

        internal static StringAlignment TranslateAlignment(ContentAlignment align)
        {
            StringAlignment result;
            if ((align & AnyRight) != 0)
            {
                result = StringAlignment.Far;
            }
            else if ((align & AnyCenter) != 0)
            {
                result = StringAlignment.Center;
            }
            else
            {
                result = StringAlignment.Near;
            }

            return result;
        }

        internal static TextFormatFlags TranslateAlignmentForGDI(ContentAlignment align)
        {
            TextFormatFlags result;
            if ((align & AnyBottom) != 0)
            {
                result = TextFormatFlags.Bottom;
            }
            else if ((align & AnyMiddle) != 0)
            {
                result = TextFormatFlags.VerticalCenter;
            }
            else
            {
                result = TextFormatFlags.Top;
            }

            return result;
        }

        internal static StringAlignment TranslateLineAlignment(ContentAlignment align)
        {
            StringAlignment result;
            if ((align & AnyBottom) != 0)
            {
                result = StringAlignment.Far;
            }
            else if ((align & AnyMiddle) != 0)
            {
                result = StringAlignment.Center;
            }
            else
            {
                result = StringAlignment.Near;
            }
            return result;
        }

        internal static TextFormatFlags TranslateLineAlignmentForGDI(ContentAlignment align)
        {
            TextFormatFlags result;
            if ((align & AnyRight) != 0)
            {
                result = TextFormatFlags.Right;
            }
            else if ((align & AnyCenter) != 0)
            {
                result = TextFormatFlags.HorizontalCenter;
            }
            else
            {
                result = TextFormatFlags.Left;
            }

            return result;
        }

        internal static StringFormat StringFormatForAlignment(ContentAlignment align)
        {
            StringFormat output = new StringFormat
            {
                Alignment = TranslateAlignment(align),
                LineAlignment = TranslateLineAlignment(align)
            };
            return output;
        }

        /// <summary>
        ///  Get StringFormat object for rendering text using GDI+ (Graphics).
        /// </summary>
        internal static StringFormat CreateStringFormat(
            Control control,
            ContentAlignment textAlign,
            bool showEllipsis,
            bool useMnemonic)
        {
            StringFormat stringFormat = StringFormatForAlignment(textAlign);

            // make sure that the text is contained within the label

            // Adjust string format for Rtl controls
            if (control.RightToLeft == RightToLeft.Yes)
            {
                stringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            if (showEllipsis)
            {
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                stringFormat.FormatFlags |= StringFormatFlags.LineLimit;
            }

            if (!useMnemonic)
            {
                stringFormat.HotkeyPrefix = Drawing.Text.HotkeyPrefix.None;
            }
            else if (control.ShowKeyboardCues)
            {
                stringFormat.HotkeyPrefix = Drawing.Text.HotkeyPrefix.Show;
            }
            else
            {
                stringFormat.HotkeyPrefix = Drawing.Text.HotkeyPrefix.Hide;
            }

            if (control.AutoSize)
            {
                stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            }

            return stringFormat;
        }

        /// <summary>
        ///  Get TextFormatFlags flags for rendering text using GDI (TextRenderer).
        /// </summary>
        internal static TextFormatFlags CreateTextFormatFlags(
            Control control,
            ContentAlignment textAlign,
            bool showEllipsis,
            bool useMnemonic)
        {
            textAlign = control.RtlTranslateContent(textAlign);
            TextFormatFlags flags = TextFormatFlagsForAlignmentGDI(textAlign);

            // The effect of the TextBoxControl flag is that in-word line breaking will occur if needed, this happens
            // when AutoSize is false and a one-word line still doesn't fit the binding box (width). The other effect
            // is that partially visiblelines are clipped; this is how GDI+ works by default.
            flags |= TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl;

            if (showEllipsis)
            {
                flags |= TextFormatFlags.EndEllipsis;
            }

            // Adjust string format for Rtl controls
            if (control.RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
            }

            if (!useMnemonic)
            {
                // Set NoPrefix as this will show the ampersand
                flags |= TextFormatFlags.NoPrefix;
            }
            else if (!control.ShowKeyboardCues)
            {
                // Set HidePrefix as this will hide ampersand if we don't press down the alt key
                flags |= TextFormatFlags.HidePrefix;
            }

            return flags;
        }
    }
}
