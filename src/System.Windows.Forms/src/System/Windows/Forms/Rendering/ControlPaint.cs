// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
///  The ControlPaint class provides a series of methods that can be used to paint common Windows UI pieces. Many
///  Windows Forms controls use this class to paint their UI elements.
/// </summary>
public static unsafe partial class ControlPaint
{
    [ThreadStatic]
    private static Bitmap? t_checkImage;         // image used to render checkmarks

    [ThreadStatic]
    private static Pen? t_focusPen;              // pen used to draw a focus rectangle

    [ThreadStatic]
    private static Pen? t_focusPenInvert;        // pen used to draw a focus rectangle

    [ThreadStatic]
    private static Color t_focusPenColor;       // the last background color the focus pen was created with

    [ThreadStatic]
    private static bool t_hcFocusPen;           // cached focus pen intended for high contrast mode

    private static Pen? s_grabPenPrimary;        // pen used for primary grab handles
    private static Pen? s_grabPenSecondary;      // pen used for secondary grab handles
    private static Brush? s_grabBrushPrimary;    // brush used for primary grab handles
    private static Brush? s_grabBrushSecondary;  // brush used for secondary grab handles

    [ThreadStatic]
    private static Brush? t_frameBrushActive;    // brush used for the active selection frame

    private static Color s_frameColorActive;    // color of active frame brush

    [ThreadStatic]
    private static Brush? t_frameBrushSelected;  // brush used for the inactive selection frame

    private static Color s_frameColorSelected;  // color of selected frame brush

    [ThreadStatic]
    private static Brush? t_gridBrush;           // brush used to draw a grid

    private static Size s_gridSize;             // the dimensions of the grid dots
    private static bool s_gridInvert;           // true if the grid color is inverted

    [ThreadStatic]
    private static ImageAttributes? t_disabledImageAttr; // ImageAttributes used to render disabled images

    private const ContentAlignment AnyRight
        = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;
    private const ContentAlignment AnyBottom
        = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
    private const ContentAlignment AnyCenter
        = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
    private const ContentAlignment AnyMiddle
        = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;

    // This constant is needed in order to correctly select which pixels of the icon should be repainted.
    // Otherwise, we will recolor intermediate shades and the icon will look inconsistent (too bold).
    private const int MaximumLuminosityDifference = 20;

    internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Size imageSize, ImageLayout imageLayout)
    {
        Rectangle result = bounds;

        switch (imageLayout)
        {
            case ImageLayout.Stretch:
                result.Size = bounds.Size;
                break;

            case ImageLayout.None:
                result.Size = imageSize;
                break;

            case ImageLayout.Center:
                result.Size = imageSize;

                if (bounds.Width > result.Width)
                {
                    result.X = (bounds.Width - result.Width) / 2;
                }

                if (bounds.Height > result.Height)
                {
                    result.Y = (bounds.Height - result.Height) / 2;
                }

                break;

            case ImageLayout.Zoom:
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
                    // Height should fill the entire bounds.
                    result.Height = bounds.Height;

                    // Preserve the aspect ratio by multiplying the yRatio by the width, adding .5 to round to
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
    public static unsafe IntPtr CreateHBitmap16Bit(Bitmap bitmap, Color background)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        HBITMAP hbitmap;
        Size size = bitmap.Size;

        // Don't use the cached DC here as this isn't a common API and we're manipulating the state.
        using CreateDcScope screen = new(default);
        using CreateDcScope dc = new(screen);

        HPALETTE palette = PInvoke.CreateHalftonePalette(dc);
        PInvokeCore.GetObject(palette, out uint entryCount);

        using BufferScope<byte> bitmapInfoBuffer = new
            (checked((int)(sizeof(BITMAPINFOHEADER) + (sizeof(RGBQUAD) * entryCount))));

        // Create a DIB based on the screen DC to write into with a halftone palette
        fixed (byte* bi = bitmapInfoBuffer)
        {
            *((BITMAPINFOHEADER*)bi) = new BITMAPINFOHEADER
            {
                biSize = (uint)sizeof(BITMAPINFOHEADER),
                biWidth = bitmap.Width,
                biHeight = bitmap.Height,
                biPlanes = 1,
                biBitCount = 16,
                biCompression = (uint)BI_COMPRESSION.BI_RGB
            };

            Span<RGBQUAD> colors = new(bi + sizeof(BITMAPINFOHEADER), (int)entryCount);
            Span<PALETTEENTRY> entries = stackalloc PALETTEENTRY[(int)entryCount];
            PInvokeCore.GetPaletteEntries(palette, entries);

            // Set up color table
            for (int i = 0; i < entryCount; i++)
            {
                PALETTEENTRY entry = entries[i];
                colors[i] = new RGBQUAD
                {
                    rgbRed = entry.peRed,
                    rgbGreen = entry.peGreen,
                    rgbBlue = entry.peBlue
                };
            }

            PInvokeCore.DeleteObject(palette);

            void* bitsBuffer;
            hbitmap = PInvokeCore.CreateDIBSection(
                screen,
                (BITMAPINFO*)bi,
                DIB_USAGE.DIB_RGB_COLORS,
                &bitsBuffer,
                hSection: default,
                offset: 0);

            if (hbitmap.IsNull)
            {
                throw new Win32Exception();
            }
        }

        try
        {
            // Put our new bitmap handle (with the halftone palette) into the dc and use Graphics to
            // copy the Bitmap into it.

            HGDIOBJ previousBitmap = PInvoke.SelectObject(dc, hbitmap);
            if (previousBitmap.IsNull)
            {
                throw new Win32Exception();
            }

            PInvokeCore.DeleteObject(previousBitmap);

            using Graphics graphics = dc.CreateGraphics();
            using var brush = background.GetCachedSolidBrushScope();
            graphics.FillRectangle(brush, 0, 0, size.Width, size.Height);
            graphics.DrawImage(bitmap, 0, 0, size.Width, size.Height);
        }
        catch
        {
            // As we're throwing out, we can't return this and need to delete it.
            PInvokeCore.DeleteObject(hbitmap);
            throw;
        }

        // The caller is responsible for freeing the HBITMAP.
        return (IntPtr)hbitmap;
    }

    /// <summary>
    ///  Creates a Win32 HBITMAP out of the image. You are responsible for deleting the HBITMAP. If the image
    ///  uses transparency the background will be filled with the specified color.
    /// </summary>
    public static unsafe IntPtr CreateHBitmapTransparencyMask(Bitmap bitmap)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

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

        // This needs to be zeroed out so we cannot use the ArrayPool
        byte[] bits = new byte[monochromeStride * height];
        BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);

        Debug.Assert(data.Scan0 != 0, "BitmapData.Scan0 is null; check marshalling");

        ReadOnlySpan<ARGB> colors = new((ARGB*)data.Scan0, width * height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                ARGB color = colors[y * width + x];
                if (color.A == 0)
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
            return (IntPtr)PInvokeCore.CreateBitmap(size.Width, size.Height, nPlanes: 1, nBitCount: 1, pBits);
        }
    }

    /// <summary>
    ///  Creates a Win32 HBITMAP out of the image. You are responsible for deleting the HBITMAP. If the image uses
    ///  transparency, the background will be filled with the specified color.
    /// </summary>
    public static IntPtr CreateHBitmapColorMask(Bitmap bitmap, IntPtr monochromeMask)
    {
        ArgumentNullException.ThrowIfNull(bitmap);

        Size size = bitmap.Size;

        HBITMAP colorMask = (HBITMAP)bitmap.GetHbitmap();
        using GetDcScope screenDC = new(HWND.Null);
        using CreateDcScope sourceDC = new(screenDC);
        using CreateDcScope targetDC = new(screenDC);
        using SelectObjectScope sourceBitmapSelection = new(sourceDC, (HGDIOBJ)monochromeMask);
        using SelectObjectScope targetBitmapSelection = new(targetDC, (HGDIOBJ)colorMask.Value);

        // Now the trick is to make colorBitmap black wherever the transparent color is located, but keep the
        // original color everywhere else. We've already got the original bitmap, so all we need to do is to AND
        // with the inverse of the mask (ROP DSna). When going from monochrome to color, Windows sets all 1 bits
        // to the background color, and all 0 bits to the foreground color.

        PInvoke.SetBkColor(targetDC, (COLORREF)0x00ffffff);    // white
        PInvoke.SetTextColor(targetDC, (COLORREF)0x00000000);  // black
        PInvokeCore.BitBlt(targetDC, x: 0, y: 0, size.Width, size.Height, sourceDC, x1: 0, y1: 0, (ROP_CODE)0x220326);
        // RasterOp.SOURCE.Invert().AndWith(RasterOp.TARGET).GetRop());

        return (IntPtr)colorMask;
    }

    internal static unsafe HBRUSH CreateHalftoneHBRUSH()
    {
        short* grayPattern = stackalloc short[8];
        for (int i = 0; i < 8; i++)
        {
            grayPattern[i] = (short)(0x5555 << (i & 1));
        }

        using CreateBitmapScope hBitmap = new(8, 8, 1, 1, grayPattern);

        LOGBRUSH logicalBrush = new()
        {
            lbStyle = BRUSH_STYLE.BS_PATTERN,
            lbColor = default, // color is ignored since style is BS.PATTERN
            lbHatch = (nuint)(IntPtr)hBitmap
        };

        return PInvoke.CreateBrushIndirect(&logicalBrush);
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

    internal static bool IsDark(Color color) => color.GetBrightness() <= .5;

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
        ArgumentNullException.ThrowIfNull(graphics);

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
        ArgumentNullException.ThrowIfNull(g);

        if (backgroundImageLayout == ImageLayout.Tile)
        {
            using TextureBrush textureBrush = new(backgroundImage, WrapMode.Tile);

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

            Rectangle imageRectangle = CalculateBackgroundImageRectangle(bounds, backgroundImage.Size, backgroundImageLayout);

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
                if (backgroundImageLayout is ImageLayout.Stretch or ImageLayout.Zoom)
                {
                    imageRectangle.Intersect(clipRect);
                    g.DrawImage(backgroundImage, imageRectangle);
                }
                else if (backgroundImageLayout == ImageLayout.None)
                {
                    imageRectangle.Offset(clipRect.Location);
                    Rectangle imageRect = imageRectangle;
                    imageRect.Intersect(clipRect);
                    Rectangle partOfImageToDraw = new(Point.Empty, imageRect.Size);
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
                    Rectangle partOfImageToDraw = new(
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
                using ImageAttributes imageAttrib = new();
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
            }
        }
    }

    public static void DrawBorder(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
    {
        ArgumentNullException.ThrowIfNull(graphics);

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
        ArgumentNullException.ThrowIfNull(graphics);

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
        ArgumentOutOfRangeException.ThrowIfNegative(leftWidth);
        ArgumentOutOfRangeException.ThrowIfNegative(topWidth);
        ArgumentOutOfRangeException.ThrowIfNegative(rightWidth);
        ArgumentOutOfRangeException.ThrowIfNegative(bottomWidth);

        int totalData = (topWidth + leftWidth + bottomWidth + rightWidth) * 2;

        // Reasonable to put on the stack (40 * 8 bytes)
        using BufferScope<int> buffer = new(stackalloc int[40], totalData);
        Span<int> allData = buffer;
        Span<int> topLineLefts = allData[..topWidth];
        allData = allData[topWidth..];
        Span<int> topLineRights = allData[..topWidth];
        allData = allData[topWidth..];
        Span<int> leftLineTops = allData[..leftWidth];
        allData = allData[leftWidth..];
        Span<int> leftLineBottoms = allData[..leftWidth];
        allData = allData[leftWidth..];
        Span<int> bottomLineLefts = allData[..bottomWidth];
        allData = allData[bottomWidth..];
        Span<int> bottomLineRights = allData[..bottomWidth];
        allData = allData[bottomWidth..];
        Span<int> rightLineTops = allData[..rightWidth];
        allData = allData[rightWidth..];
        Span<int> rightLineBottoms = allData[..rightWidth];

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
                        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                        using CreatePenScope hpen = new(topColor);
                        for (int i = 0; i < topWidth; i++)
                        {
                            // Need to add one to the destination point for GDI to render the same as GDI+
                            hdc.DrawLine(hpen, topLineLefts[i], bounds.Y + i, topLineRights[i] + 1, bounds.Y + i);
                        }
                    }
                    else if (deviceContext.TryGetGraphics(create: true) is Graphics graphics)
                    {
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
                    HLSColor hlsColor = new(topColor);
                    float inc = InfinityToOne(1.0f / (topWidth - 1));
                    using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                    for (int i = 0; i < topWidth; i++)
                    {
                        using CreatePenScope hpen = new(
                            topStyle == ButtonBorderStyle.Inset
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
                        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                        using CreatePenScope hpen = new(leftColor);
                        for (int i = 0; i < leftWidth; i++)
                        {
                            // Need to add one to the destination point for GDI to render the same as GDI+
                            hdc.DrawLine(hpen, bounds.X + i, leftLineTops[i], bounds.X + i, leftLineBottoms[i] + 1);
                        }
                    }
                    else if (deviceContext.TryGetGraphics(create: true) is Graphics graphics)
                    {
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
                    HLSColor hlsColor = new(leftColor);
                    float inc = InfinityToOne(1.0f / (leftWidth - 1));
                    using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                    for (int i = 0; i < leftWidth; i++)
                    {
                        using CreatePenScope hpen = new(
                            leftStyle == ButtonBorderStyle.Inset
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
                        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                        using CreatePenScope hpen = new(bottomColor);
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
                    else if (deviceContext.TryGetGraphics(create: true) is Graphics graphics)
                    {
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
                    HLSColor hlsColor = new(bottomColor);
                    float inc = InfinityToOne(1.0f / (bottomWidth - 1));
                    using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                    for (int i = 0; i < bottomWidth; i++)
                    {
                        using CreatePenScope hpen = new(
                            bottomStyle != ButtonBorderStyle.Inset
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
                        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                        using CreatePenScope hpen = new(rightColor);
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
                    else if (deviceContext.TryGetGraphics(create: true) is Graphics graphics)
                    {
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
                    HLSColor hlsColor = new(rightColor);
                    float inc = InfinityToOne(1.0f / (rightWidth - 1));
                    using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
                    for (int i = 0; i < rightWidth; i++)
                    {
                        using CreatePenScope hpen = new(
                            rightStyle != ButtonBorderStyle.Inset
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
        ArgumentNullException.ThrowIfNull(graphics);

        DRAWEDGE_FLAGS edge = (DRAWEDGE_FLAGS)((uint)style & 0x0F);
        DRAW_EDGE_FLAGS flags = (DRAW_EDGE_FLAGS)sides | (DRAW_EDGE_FLAGS)((uint)style & ~0x0F);

        RECT rc = new Rectangle(x, y, width, height);

        // Windows just draws the border to size, and then shrinks the rectangle so the user can paint the client
        // area. We can't really do that, so we do the opposite: We pre-calculate the size of the border and enlarge
        // the rectangle so the client size is preserved.
        if (flags.HasFlag((DRAW_EDGE_FLAGS)Border3DStyle.Adjust))
        {
            Size sz = SystemInformation.Border3DSize;
            rc.left -= sz.Width;
            rc.right += sz.Width;
            rc.top -= sz.Height;
            rc.bottom += sz.Height;
            flags &= ~(DRAW_EDGE_FLAGS)Border3DStyle.Adjust;
        }

        // Get Win32 dc with Graphics properties applied to it.
        using DeviceContextHdcScope hdc = new(graphics);
        PInvoke.DrawEdge(hdc, ref rc, edge, flags);
    }

    /// <summary>
    ///  Helper function that draws a more complex border. This is used by DrawBorder for less common
    ///  rendering cases. We split DrawBorder into DrawBorderSimple and DrawBorderComplex so we maximize
    ///  the % of the function call. It is less performant to have large functions that do many things.
    /// </summary>
    private static void DrawBorderComplex(Graphics graphics, Rectangle bounds, Color color, ButtonBorderStyle style)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        if (style == ButtonBorderStyle.Inset)
        {
            // Button being pushed
            HLSColor hls = new(color);

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
            HLSColor hls = new(color);

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
        ArgumentNullException.ThrowIfNull(context);

        if (color.HasTransparency() || style != ButtonBorderStyle.Solid)
        {
            // GDI+ right and bottom DrawRectangle border are 1 greater than GDI
            bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);

            Graphics? graphics = context.TryGetGraphics(create: true);
            if (graphics is not null)
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

        using DeviceContextHdcScope hdc = context.ToHdcScope();
        using CreatePenScope hpen = new(color);
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
            DFC_TYPE.DFC_BUTTON,
            DFCS_STATE.DFCS_BUTTONPUSH | (DFCS_STATE)state,
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
            DFC_TYPE.DFC_CAPTION,
            (DFCS_STATE)button | (DFCS_STATE)state,
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
                DFC_TYPE.DFC_BUTTON,
                DFCS_STATE.DFCS_BUTTONCHECK | (DFCS_STATE)state,
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
            DFC_TYPE.DFC_SCROLL,
            DFCS_STATE.DFCS_SCROLLCOMBOBOX | (DFCS_STATE)state,
            Color.Empty,
            Color.Empty);

    /// <summary>
    ///  Draws a container control grab handle glyph inside the given rectangle.
    /// </summary>
    public static void DrawContainerGrabHandle(Graphics graphics, Rectangle bounds)
    {
        ArgumentNullException.ThrowIfNull(graphics);

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
        ArgumentNullException.ThrowIfNull(graphics);

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
        ArgumentNullException.ThrowIfNull(graphics);
        if (rectangle.Width < 0 || rectangle.Height < 0)
            throw new ArgumentOutOfRangeException(nameof(rectangle));

        Rectangle offsetRectangle = new(
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
                if (t_checkImage is not null)
                {
                    t_checkImage.Dispose();
                    t_checkImage = null;
                }

                // We draw the checkmark slightly off center to eliminate 3-D border artifacts and compensate below
                RECT rcCheck = new(rectangle.Size);
                Bitmap bitmap = new(rectangle.Width, rectangle.Height);
                using (Graphics g2 = Graphics.FromImage(bitmap))
                {
                    g2.Clear(Color.Transparent);
                    using DeviceContextHdcScope dc = new(g2, applyGraphicsState: false);
                    PInvoke.DrawFrameControl(dc, ref rcCheck, DFC_TYPE.DFC_MENU, DFCS_STATE.DFCS_MENUCHECK);
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

    internal static void DrawBlackWhiteFocusRectangle(Graphics graphics, Rectangle rectangle, Color color)
        => DrawFocusRectangle(graphics, rectangle, color, highContrast: false, blackAndWhite: true);

    private static void DrawFocusRectangle(Graphics graphics, Rectangle rectangle, Color color, bool highContrast, bool blackAndWhite = false)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        rectangle.Width--;
        rectangle.Height--;
        graphics.DrawRectangle(
            // We want the corner to be penned see GetFocusPen for more explanation
            GetFocusPen(color, (rectangle.X + rectangle.Y) % 2 == 1, highContrast, blackAndWhite),
            rectangle);
    }

    /// <summary>
    ///  Draws a win32 frame control.
    /// </summary>
    private static void DrawFrameControl(
        Graphics graphics,
        int x, int y, int width, int height,
        DFC_TYPE kind,
        DFCS_STATE state,
        Color foreColor,
        Color backColor)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);

        RECT rcFrame = new(0, 0, width, height);
        using Bitmap bitmap = new(width, height);
        using Graphics g2 = Graphics.FromImage(bitmap);
        g2.Clear(Color.Transparent);

        using (DeviceContextHdcScope hdc = new(g2, applyGraphicsState: false))
        {
            // Get Win32 dc with Graphics properties applied to it.
            PInvoke.DrawFrameControl(hdc, ref rcFrame, kind, state);
        }

        if (foreColor == Color.Empty || backColor == Color.Empty)
        {
            graphics.DrawImage(bitmap, x, y);
        }
        else
        {
            // Replace black/white with foreColor/backColor.
            ImageAttributes attributes = new();

            Span<(Color OldColor, Color NewColor)> map =
            [
                new(Color.Black, foreColor),
                new(Color.White, backColor)
            ];

            attributes.SetRemapTable(ColorAdjustType.Bitmap, map);
            graphics.DrawImage(
                bitmap,
                new Rectangle(x, y, width, height),
                0, 0, width, height,
                GraphicsUnit.Pixel,
                attributes,
                null,
                IntPtr.Zero);
        }
    }

    /// <summary>
    ///  Draws a standard selection grab handle with the given dimensions. Grab
    ///  handles are used by components to indicate to the user that they can
    ///  be directly manipulated.
    /// </summary>
    public static void DrawGrabHandle(Graphics graphics, Rectangle rectangle, bool primary, bool enabled)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        Pen pen = primary
            ? s_grabPenPrimary ??= Pens.Black
            : s_grabPenSecondary ??= Pens.White;

        Brush brush = primary
            ? enabled ? (s_grabBrushPrimary ??= Brushes.White) : SystemBrushes.Control
            : enabled ? (s_grabBrushSecondary ??= Brushes.Black) : SystemBrushes.Control;

        Rectangle fillRect = new(
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
        ArgumentNullException.ThrowIfNull(graphics);
        if (pixelsBetweenDots.Width <= 0 || pixelsBetweenDots.Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(pixelsBetweenDots));

        float intensity = backColor.GetBrightness();
        bool invert = (intensity < .5);

        if (t_gridBrush is null || s_gridSize.Width != pixelsBetweenDots.Width
            || s_gridSize.Height != pixelsBetweenDots.Height || invert != s_gridInvert)
        {
            if (t_gridBrush is not null)
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

            using Bitmap bitmap = new(width, height);

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
        ArgumentNullException.ThrowIfNull(graphics);

        using ImageAttributes attributes = new();
        attributes.SetColorMatrix(RemapBlackAndWhitePreserveTransparentMatrix(replaceBlack, Color.White));
        graphics.DrawImage(
            image,
            destination,
            0, 0, image.Width, image.Height,
            GraphicsUnit.Pixel,
            attributes,
            null,
            0);
    }

    internal static bool IsImageTransparent(Image? backgroundImage)
        => backgroundImage is not null && (backgroundImage.Flags & (int)ImageFlags.HasAlpha) > 0;

    // takes an image and replaces all the pixels of oldColor with newColor, drawing the new image into the rectangle on
    // the supplied Graphics object.
    internal static void DrawImageReplaceColor(Graphics g, Image image, Rectangle dest, Color oldColor, Color newColor)
    {
        using ImageAttributes attributes = new();

        (Color OldColor, Color NewColor) map = new(oldColor, newColor);
        attributes.SetRemapTable(ColorAdjustType.Bitmap, new ReadOnlySpan<(Color OldColor, Color NewColor)>(ref map));

        g.DrawImage(image, dest, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes, null, 0);
    }

    /// <summary>
    ///  Draws an image and makes it look disabled.
    /// </summary>
    public static void DrawImageDisabled(Graphics graphics, Image image, int x, int y, Color background)
        => DrawImageDisabled(graphics, image, new Rectangle(x, y, image.Width, image.Height), unscaledImage: false);

    /// <summary>
    ///  Draws an image and makes it look disabled.
    /// </summary>
    [SkipLocalsInit]
    internal static void DrawImageDisabled(Graphics graphics, Image image, Rectangle imageBounds, bool unscaledImage)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        ArgumentNullException.ThrowIfNull(image);

        Size imageSize = image.Size;

        if (t_disabledImageAttr is null)
        {
            // This ColorMatrix is set up to resemble Office 10 command bars, but still be able to deal with
            // hi-color (256+) icons and images.
            //
            // The idea is to scale everything down (more than just a grayscale does, therefore the small numbers
            // in the scaling part of matrix). White becomes some shade of gray and black stays black.
            //
            // Second part of the matrix is to translate everything, so all colors are a bit brighter. Grays become
            // lighter and washed out looking black becomes a shade of gray as well.

            Span<float> array =
            [
                0.2125f, 0.2125f, 0.2125f, 0, 0,
                0.2577f, 0.2577f, 0.2577f, 0, 0,
                0.0361f, 0.0361f, 0.0361f, 0, 0,
                0, 0, 0, 1, 0,
                0.38f, 0.38f, 0.38f, 0, 1
            ];

            ColorMatrix grayMatrix = new(array);

            t_disabledImageAttr = new ImageAttributes();
            t_disabledImageAttr.ClearColorKey();
            t_disabledImageAttr.SetColorMatrix(grayMatrix);
        }

        if (unscaledImage)
        {
            using Bitmap bitmap = new(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(
                    image,
                    new Rectangle(0, 0, imageSize.Width, imageSize.Height),
                    0, 0, imageSize.Width, imageSize.Height,
                    GraphicsUnit.Pixel,
                    t_disabledImageAttr);
            }

            graphics.DrawImageUnscaled(bitmap, imageBounds);
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
    ///  This method is needed as a workaround for the https://github.com/dotnet/winforms/issues/3043.
    ///  Since the row header and column header dividing lines drawn by the
    ///  https://docs.microsoft.com/dotnet/api/system.windows.forms.visualstyles.visualstylerenderer.drawbackground
    ///  method has low contrast, we draw a lines with a suitable contrast on top of it.
    /// </summary>
    internal static void EnforceHeaderCellDividerContrast(Graphics graphics, Rectangle bounds)
    {
        using Pen pen = new(SystemColors.WindowFrame, 1);

        // -1 when calculating X coordinates is necessary because without this the line will not be drawn.
        // This is most likely due to the fact that drawing the next cell overlaps the line
        // we have drawn and it is not displayed.
        // -1 when calculating Y coordinates, it is necessary that there are no gaps
        // between the dividing line and the upper line of the border.
        Point start = new(bounds.X + bounds.Width - 1, bounds.Y - 1);
        Point end = new(bounds.X + bounds.Width - 1, bounds.Y + bounds.Height);
        graphics.DrawLine(pen, start, end);
    }

    /// <summary>
    ///  Draws a locked selection frame around the given rectangle.
    /// </summary>
    public static void DrawLockedFrame(Graphics graphics, Rectangle rectangle, bool primary)
    {
        ArgumentNullException.ThrowIfNull(graphics);

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
        => DrawFrameControl(graphics, x, y, width, height, DFC_TYPE.DFC_MENU, (DFCS_STATE)glyph, Color.Empty, Color.Empty);

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
            DFC_TYPE.DFC_MENU,
            (DFCS_STATE)glyph,
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
            DFC_TYPE.DFC_BUTTON,
            DFCS_STATE.DFCS_BUTTON3STATE | (DFCS_STATE)state,
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
            DFC_TYPE.DFC_BUTTON,
            DFCS_STATE.DFCS_BUTTONRADIO | (DFCS_STATE)state,
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
        R2_MODE rop2;
        Color graphicsColor;

        if (backColor.GetBrightness() < .5)
        {
            rop2 = R2_MODE.R2_NOTXORPEN;
            graphicsColor = Color.White;
        }
        else
        {
            rop2 = R2_MODE.R2_XORPEN;
            graphicsColor = Color.Black;
        }

        using GetDcScope desktopDC = new(
            PInvokeCore.GetDesktopWindow(),
            HRGN.Null,
            GET_DCX_FLAGS.DCX_WINDOW | GET_DCX_FLAGS.DCX_LOCKWINDOWUPDATE | GET_DCX_FLAGS.DCX_CACHE);

        using ObjectScope pen = new(style switch
        {
            FrameStyle.Dashed => (HGDIOBJ)PInvoke.CreatePen(PEN_STYLE.PS_DOT, cWidth: 1, (COLORREF)(uint)ColorTranslator.ToWin32(backColor)).Value,
            FrameStyle.Thick => (HGDIOBJ)PInvoke.CreatePen(PEN_STYLE.PS_SOLID, cWidth: 2, (COLORREF)(uint)ColorTranslator.ToWin32(backColor)).Value,
            _ => default
        });

        using SetRop2Scope rop2Scope = new(desktopDC, rop2);
        using SelectObjectScope brushSelection = new(desktopDC, PInvokeCore.GetStockObject(GET_STOCK_OBJECT_FLAGS.NULL_BRUSH));
        using SelectObjectScope penSelection = new(desktopDC, pen);

        PInvoke.SetBkColor(desktopDC, (COLORREF)(uint)ColorTranslator.ToWin32(graphicsColor));
        PInvoke.Rectangle(desktopDC, rectangle.X, rectangle.Y, rectangle.Right, rectangle.Bottom);
    }

    /// <summary>
    ///  Draws a reversible line on the screen. A reversible line can be erased by just drawing over it again.
    /// </summary>
    public static unsafe void DrawReversibleLine(Point start, Point end, Color backColor)
    {
        R2_MODE rop2 = (R2_MODE)GetColorRop(backColor, (int)R2_MODE.R2_NOTXORPEN, (int)R2_MODE.R2_XORPEN);

        using GetDcScope desktopDC = new(
            PInvokeCore.GetDesktopWindow(),
            HRGN.Null,
            GET_DCX_FLAGS.DCX_WINDOW | GET_DCX_FLAGS.DCX_LOCKWINDOWUPDATE | GET_DCX_FLAGS.DCX_CACHE);

        using ObjectScope pen = new(PInvoke.CreatePen(PEN_STYLE.PS_SOLID, cWidth: 1, (COLORREF)(uint)ColorTranslator.ToWin32(backColor)));
        using SetRop2Scope ropScope = new(desktopDC, rop2);
        using SelectObjectScope brushSelection = new(desktopDC, PInvokeCore.GetStockObject(GET_STOCK_OBJECT_FLAGS.NULL_BRUSH));
        using SelectObjectScope penSelection = new(desktopDC, pen);

        PInvoke.MoveToEx(desktopDC, start.X, start.Y, lppt: null);
        PInvoke.LineTo(desktopDC, end.X, end.Y);
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
            DFC_TYPE.DFC_SCROLL,
            (DFCS_STATE)button | (DFCS_STATE)state,
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
        ArgumentNullException.ThrowIfNull(graphics);

        Brush frameBrush = active ? GetActiveBrush(backColor) : GetSelectedBrush(backColor);

        Region clip = graphics.Clip;
        graphics.ExcludeClip(insideRect);
        graphics.FillRectangle(frameBrush, outsideRect);
        graphics.Clip = clip;
    }

    /// <summary>
    ///  Draws a size grip at the given location. The color of the size grip is based on the given background color.
    /// </summary>
    public static void DrawSizeGrip(Graphics graphics, Color backColor, Rectangle bounds)
        => DrawSizeGrip(graphics, backColor, bounds.X, bounds.Y, bounds.Width, bounds.Height);

    /// <summary>
    ///  Draws a size grip at the given location. The color of the size grip is based on the given background color.
    /// </summary>
    public static void DrawSizeGrip(Graphics graphics, Color backColor, int x, int y, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(graphics);

        using var bright = GdiPlusCache.GetCachedPenScope(LightLight(backColor));
        using var dark = GdiPlusCache.GetCachedPenScope(Dark(backColor));

        int minDim = Math.Min(width, height);
        int right = x + width - 1;
        int bottom = y + height - 2;

        for (int i = 0; i < minDim - 4; i += 4)
        {
            graphics.DrawLine(dark, right - (i + 1) - 2, bottom, right, bottom - (i + 1) - 2);
            graphics.DrawLine(dark, right - (i + 2) - 2, bottom, right, bottom - (i + 2) - 2);
            graphics.DrawLine(bright, right - (i + 3) - 2, bottom, right, bottom - (i + 3) - 2);
        }
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

        using DeviceContextHdcScope hdc = deviceContext.ToHdcScope();
        using CreatePenScope hpenBright = new(LightLight(backColor));
        using CreatePenScope hpenDark = new(Dark(backColor));

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
        ArgumentNullException.ThrowIfNull(graphics);

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
        ArgumentNullException.ThrowIfNull(dc);

        // This must come before creating the scope.
        FONT_QUALITY quality = TextRenderer.FontQualityFromTextRenderingHint(dc);

        using DeviceContextHdcScope hdc = dc.ToHdcScope(TextRenderer.GetApplyStateFlags(dc, format));
        DrawStringDisabled(hdc, s, font, color, layoutRectangle, format, quality);
    }

    internal static void DrawStringDisabled(
        HDC dc,
        string s,
        Font font,
        Color color,
        Rectangle layoutRectangle,
        TextFormatFlags format,
        FONT_QUALITY quality = FONT_QUALITY.DEFAULT_QUALITY)
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
        ArgumentNullException.ThrowIfNull(graphics);

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
        ROP_CODE rop3 = (ROP_CODE)GetColorRop(
            backColor,
            0xa50065,   // RasterOp.BRUSH.Invert().XorWith(RasterOp.TARGET),
            0x5a0049);  // RasterOp.BRUSH.XorWith(RasterOp.TARGET));
        R2_MODE rop2 = R2_MODE.R2_NOT;

        using GetDcScope desktopDC = new(
            PInvokeCore.GetDesktopWindow(),
            HRGN.Null,
            GET_DCX_FLAGS.DCX_WINDOW | GET_DCX_FLAGS.DCX_LOCKWINDOWUPDATE | GET_DCX_FLAGS.DCX_CACHE);

        using ObjectScope brush = new(PInvoke.CreateSolidBrush((COLORREF)(uint)ColorTranslator.ToWin32(backColor)));
        using SetRop2Scope ropScope = new(desktopDC, rop2);
        using SelectObjectScope brushSelection = new(desktopDC, brush);

        // PatBlt must be the only Win32 function that wants height in width rather than x2,y2.
        PInvoke.PatBlt(desktopDC, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rop3);
    }

    /// <summary>
    ///  Converts the font into one where Font.Unit == Point. If the original font is in device-dependent units
    ///  (and it usually is), we interpret the size relative to the screen. This is not really a general-purpose
    ///  function -- when used on something not obtained from ChooseFont, it may round away some precision.
    /// </summary>
    internal static Font FontInPoints(Font font)
        => new(
            font.FontFamily,
            font.SizeInPoints,
            font.Style,
            GraphicsUnit.Point,
            font.GdiCharSet,
            font.GdiVerticalFont);

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
            if (t_frameBrushActive is not null)
            {
                t_frameBrushActive.Dispose();
                t_frameBrushActive = null;
            }

            s_frameColorActive = brushColor;

            int patternSize = 8;

            using Bitmap bitmap = new(patternSize, patternSize);

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
        }

        return t_frameBrushActive;
    }

    /// <summary>
    ///  Retrieves the pen used to draw a focus rectangle around a control. The focus rectangle is typically drawn
    ///  when the control has keyboard focus.
    /// </summary>
    private static Pen GetFocusPen(Color baseColor, bool odds, bool highContrast, bool blackAndWhite)
    {
        if (t_focusPen is null
            || t_hcFocusPen != highContrast
            || (!highContrast && t_focusPenColor.GetBrightness() <= .5 && baseColor.GetBrightness() <= .5)
            || t_focusPenColor.ToArgb() != baseColor.ToArgb())
        {
            if (t_focusPen is not null)
            {
                t_focusPen.Dispose();
                t_focusPen = null;
                t_focusPenInvert?.Dispose();
                t_focusPenInvert = null;
            }

            t_focusPenColor = baseColor;
            t_hcFocusPen = highContrast;

            using Bitmap b = new(2, 2);
            Color color1 = Color.Transparent;
            Color color2;
            if (highContrast)
            {
                // In highcontrast mode "baseColor" itself is used as the focus pen color.
                color2 = baseColor;
            }
            else if (blackAndWhite)
            {
                color1 = Color.White;
                color2 = Color.Black;
            }
            else
            {
                // In non-highcontrast mode "baseColor" is used to calculate the focus pen colors. In this mode
                // "baseColor" is expected to contain the background color of the control to do this calculation
                // properly.
                color2 = Color.Black;

                if (IsDark(baseColor))
                {
                    color1 = color2;
                    color2 = baseColor.InvertColor();
                }
                else if (baseColor == Color.Transparent)
                {
                    color1 = Color.White;
                }
            }

            // High contrast        Normal (dark)       Normal (light)     Normal (light, base transparent)
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

        return odds ? t_focusPen : t_focusPenInvert!;
    }

    /// <summary>
    ///  Retrieves the brush used to draw selected objects.
    /// </summary>
    private static Brush GetSelectedBrush(Color backColor)
    {
        Color brushColor = backColor.GetBrightness() <= .5 ? SystemColors.ControlLight : SystemColors.ControlDark;

        if (t_frameBrushSelected is null || !s_frameColorSelected.Equals(brushColor))
        {
            if (t_frameBrushSelected is not null)
            {
                t_frameBrushSelected.Dispose();
                t_frameBrushSelected = null;
            }

            s_frameColorSelected = brushColor;

            int patternSize = 8;

            using Bitmap bitmap = new(patternSize, patternSize);

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
        }

        return t_frameBrushSelected;
    }

    /// <summary>
    ///  Converts an infinite value to "1".
    /// </summary>
    private static float InfinityToOne(float value)
        => value is float.NegativeInfinity or float.PositiveInfinity ? 1.0f : value;

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

                // draw the shadow
                bound = new Rectangle(bound.X + 1, bound.Y + 1, bound.Width - 1, bound.Height - 1);
                g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X + bound.Width - 1, bound.Y);
                g.DrawLine(SystemPens.ControlDark, bound.X, bound.Y, bound.X, bound.Y + bound.Height - 1);
                g.DrawLine(SystemPens.Window, bound.X + bound.Width - 1, bound.Y, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);
                g.DrawLine(SystemPens.Window, bound.X, bound.Y + bound.Height - 1, bound.X + bound.Width - 1, bound.Y + bound.Height - 1);

                break;
        }
    }

    internal static void InvertForeColorIfNeeded(Bitmap bitmap, Color backgroundColor)
    {
        HLSColor backgroundColorWrapper = new(backgroundColor);

        for (int y = 0; y < bitmap.Height; ++y)
        {
            for (int x = 0; x < bitmap.Width; ++x)
            {
                var pixel = bitmap.GetPixel(x, y);
                if (pixel != backgroundColor)
                {
                    HLSColor pixelColorWrapper = new(pixel);
                    if (Math.Abs(pixelColorWrapper.Luminosity - backgroundColorWrapper.Luminosity) > MaximumLuminosityDifference)
                    {
                        bitmap.SetPixel(x, y, pixel.InvertColor());
                    }
                }
            }
        }
    }

    internal static Bitmap CreateBitmapWithInvertedForeColor(Bitmap bitmap, Color backgroundColor)
    {
        Bitmap result = new(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
        for (int y = 0; y < bitmap.Height; ++y)
        {
            for (int x = 0; x < bitmap.Width; ++x)
            {
                Color pixel = bitmap.GetPixel(x, y);
                result.SetPixel(x, y, pixel != backgroundColor ? pixel.InvertColor() : pixel);
            }
        }

        return result;
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

    internal static TextFormatFlags ConvertAlignmentToTextFormat(ContentAlignment alignment)
    {
        // These are both defaults and have values of 0 to match the underlying GDI settings.
        TextFormatFlags flags = TextFormatFlags.Top | TextFormatFlags.Left;

        if ((alignment & AnyBottom) != 0)
        {
            flags |= TextFormatFlags.Bottom;
        }
        else if ((alignment & AnyMiddle) != 0)
        {
            flags |= TextFormatFlags.VerticalCenter;
        }

        if ((alignment & AnyRight) != 0)
        {
            flags |= TextFormatFlags.Right;
        }
        else if ((alignment & AnyCenter) != 0)
        {
            flags |= TextFormatFlags.HorizontalCenter;
        }

        return flags;
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
        StringFormat stringFormat = new()
        {
            Alignment = (textAlign & AnyRight) != 0
                ? StringAlignment.Far
                : (textAlign & AnyCenter) != 0 ? StringAlignment.Center : StringAlignment.Near,
            LineAlignment = (textAlign & AnyBottom) != 0
                ? StringAlignment.Far
                : (textAlign & AnyMiddle) != 0 ? StringAlignment.Center : StringAlignment.Near
        };

        // Make sure that the text is contained within the label.

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
        ContentAlignment alignment,
        bool showEllipsis,
        bool useMnemonic)
    {
        alignment = control.RtlTranslateContent(alignment);
        TextFormatFlags flags = ConvertAlignmentToTextFormat(alignment);

        // The effect of the TextBoxControl flag is that in-word line breaking will occur if needed, this happens
        // when AutoSize is false and a one-word line still doesn't fit the binding box (width). The other effect
        // is that partially visible lines are clipped; this is how GDI+ works by default.
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
