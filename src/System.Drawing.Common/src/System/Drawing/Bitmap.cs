// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
#if NET9_0_OR_GREATER
using System.Drawing.Imaging.Effects;
#endif
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Drawing;

[Editor($"System.Drawing.Design.BitmapEditor, {AssemblyRef.SystemDrawingDesign}",
        $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
[Serializable]
[TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public sealed unsafe class Bitmap : Image, IPointer<GpBitmap>
{
    private static readonly Color s_defaultTransparentColor = Color.LightGray;

    private Bitmap() { }

    internal Bitmap(GpBitmap* ptr) => SetNativeImage((GpImage*)ptr);

    public Bitmap(string filename) : this(filename, useIcm: false) { }

    public Bitmap(string filename, bool useIcm)
    {
        // GDI+ will read this file multiple times. Get the fully qualified path
        // so if the app's default directory changes we won't get an error.
        filename = Path.GetFullPath(filename);

        GpBitmap* bitmap;

        fixed (char* fn = filename)
        {
            Status status = useIcm
                ? PInvokeGdiPlus.GdipCreateBitmapFromFileICM(fn, &bitmap)
                : PInvokeGdiPlus.GdipCreateBitmapFromFile(fn, &bitmap);
            status.ThrowIfFailed();
        }

        ValidateImage((GpImage*)bitmap);
        SetNativeImage((GpImage*)bitmap);
        GetAnimatedGifRawData(this, filename, dataStream: null);
    }

    public Bitmap(Stream stream) : this(stream, false)
    {
    }

    public Bitmap(Stream stream, bool useIcm)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var iStream = stream.ToIStream(makeSeekable: true);

        GpBitmap* bitmap = null;
        Status status = useIcm
            ? PInvokeGdiPlus.GdipCreateBitmapFromStreamICM(iStream, &bitmap)
            : PInvokeGdiPlus.GdipCreateBitmapFromStream(iStream, &bitmap);
        status.ThrowIfFailed();

        ValidateImage((GpImage*)bitmap);
        SetNativeImage((GpImage*)bitmap);
        GetAnimatedGifRawData(this, filename: null, stream);
    }

    public Bitmap(Type type, string resource) : this(GetResourceStream(type, resource))
    {
    }

    private static Stream GetResourceStream(Type type, string resource)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(resource);

        return type.Module.Assembly.GetManifestResourceStream(type, resource)
            ?? throw new ArgumentException(SR.Format(SR.ResourceNotFound, type, resource));
    }

    public Bitmap(int width, int height) : this(width, height, PixelFormat.Format32bppArgb)
    {
    }

    public Bitmap(int width, int height, Graphics g)
    {
        ArgumentNullException.ThrowIfNull(g);

        GpBitmap* bitmap;
        PInvokeGdiPlus.GdipCreateBitmapFromGraphics(width, height, g.NativeGraphics, &bitmap).ThrowIfFailed();
        SetNativeImage((GpImage*)bitmap);
        GC.KeepAlive(g);
    }

    public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
    {
        GpBitmap* bitmap;
        PInvokeGdiPlus.GdipCreateBitmapFromScan0(width, height, stride, (int)format, (byte*)scan0, &bitmap).ThrowIfFailed();
        SetNativeImage((GpImage*)bitmap);
    }

    public Bitmap(int width, int height, PixelFormat format)
    {
        GpBitmap* bitmap;
        PInvokeGdiPlus.GdipCreateBitmapFromScan0(width, height, 0, (int)format, null, &bitmap).ThrowIfFailed();
        SetNativeImage((GpImage*)bitmap);
    }

    public Bitmap(Image original) : this(original, original.Width, original.Height)
    {
    }

    public Bitmap(Image original, Size newSize) : this(original, newSize.Width, newSize.Height)
    {
    }

    public Bitmap(Image original, int width, int height) : this(width, height, PixelFormat.Format32bppArgb)
    {
        ArgumentNullException.ThrowIfNull(original);
        using var g = Graphics.FromImage(this);
        g.Clear(Color.Transparent);
        g.DrawImage(original, 0, 0, width, height);
    }

    private Bitmap(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    nint IPointer<GpBitmap>.Pointer => (nint)((Image)this).Pointer();

    public static Bitmap FromHicon(IntPtr hicon)
    {
        GpBitmap* bitmap;
        PInvokeGdiPlus.GdipCreateBitmapFromHICON((HICON)hicon, &bitmap).ThrowIfFailed();
        return new Bitmap(bitmap);
    }

    public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
    {
        ArgumentNullException.ThrowIfNull(bitmapName);
        GpBitmap* bitmap = null;
        fixed (char* bn = bitmapName)
        {
            PInvokeGdiPlus.GdipCreateBitmapFromResource((HINSTANCE)hinstance, bn, &bitmap).ThrowIfFailed();
        }

        return new Bitmap(bitmap);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHbitmap() => GetHbitmap(Color.LightGray);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHbitmap(Color background)
    {
        try
        {
            return this.GetHBITMAP(background);
        }
        catch (ArgumentException)
        {
            if (Width >= short.MaxValue || Height >= short.MaxValue)
            {
                throw new ArgumentException(SR.GdiplusInvalidSize);
            }
            else
            {
                throw;
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHicon()
    {
        HICON hicon;
        PInvokeGdiPlus.GdipCreateHICONFromBitmap(this.Pointer(), &hicon).ThrowIfFailed();
        GC.KeepAlive(this);
        return hicon;
    }

    public Bitmap Clone(RectangleF rect, PixelFormat format)
    {
        if (rect.Width == 0 || rect.Height == 0)
        {
            throw new ArgumentException(SR.Format(SR.GdiplusInvalidRectangle, rect.ToString()));
        }

        GpBitmap* clone;

        Status status = PInvokeGdiPlus.GdipCloneBitmapArea(
            rect.X, rect.Y, rect.Width, rect.Height,
            (int)format,
            this.Pointer(),
            &clone);

        if (status != Status.Ok || clone is null)
        {
            throw Gdip.StatusException(status);
        }

        GC.KeepAlive(this);
        return new Bitmap(clone);
    }

    public void MakeTransparent()
    {
        Color transparent = s_defaultTransparentColor;
        if (Height > 0 && Width > 0)
        {
            transparent = GetPixel(0, Size.Height - 1);
        }

        if (transparent.A < 255)
        {
            // It's already transparent, and if we proceeded, we will do something
            // unintended like making black transparent
            return;
        }

        MakeTransparent(transparent);
    }

    public void MakeTransparent(Color transparentColor)
    {
        if (RawFormat.Guid == ImageFormat.Icon.Guid)
        {
            throw new InvalidOperationException(SR.CantMakeIconTransparent);
        }

        Size size = Size;

        // The new bitmap must be in 32bppARGB  format, because that's the only
        // thing that supports alpha. (And that's what the image is initialized to -- transparent)
        using Bitmap result = new(size.Width, size.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(result);

        graphics.Clear(Color.Transparent);
        Rectangle rectangle = new(0, 0, size.Width, size.Height);

        using (ImageAttributes attributes = new())
        {
            attributes.SetColorKey(transparentColor, transparentColor);
            graphics.DrawImage(
                this,
                rectangle,
                0, 0, size.Width, size.Height,
                GraphicsUnit.Pixel,
                attributes,
                callback: null,
                callbackData: 0);
        }

        // Swap nativeImage pointers to make it look like we modified the image in place
        GpBitmap* temp = this.Pointer();
        SetNativeImage((GpImage*)result.Pointer());
        result.SetNativeImage((GpImage*)temp);
    }

    public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format) =>
        LockBits(rect, flags, format, new());

    public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
    {
        ArgumentNullException.ThrowIfNull(bitmapData);

        fixed (void* data = &bitmapData.GetPinnableReference())
        {
            this.LockBits(
                rect,
                (GdiPlus.ImageLockMode)flags,
                (GdiPlus.PixelFormat)format,
                ref Unsafe.AsRef<GdiPlus.BitmapData>(data));
        }

        GC.KeepAlive(this);
        return bitmapData;
    }

    public void UnlockBits(BitmapData bitmapdata)
    {
        ArgumentNullException.ThrowIfNull(bitmapdata);

        fixed (void* data = &bitmapdata.GetPinnableReference())
        {
            this.UnlockBits(ref Unsafe.AsRef<GdiPlus.BitmapData>(data));
        }

        GC.KeepAlive(this);
    }

    public Color GetPixel(int x, int y)
    {
        if (x < 0 || x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x), SR.ValidRangeX);
        }

        if (y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y), SR.ValidRangeY);
        }

        uint color;
        PInvokeGdiPlus.GdipBitmapGetPixel(this.Pointer(), x, y, &color).ThrowIfFailed();
        GC.KeepAlive(this);
        return Color.FromArgb((int)color);
    }

    public void SetPixel(int x, int y, Color color)
    {
        if ((PixelFormat & PixelFormat.Indexed) != 0)
        {
            throw new InvalidOperationException(SR.GdiplusCannotSetPixelFromIndexedPixelFormat);
        }

        if (x < 0 || x >= Width)
        {
            throw new ArgumentOutOfRangeException(nameof(x), SR.ValidRangeX);
        }

        if (y < 0 || y >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(y), SR.ValidRangeY);
        }

        PInvokeGdiPlus.GdipBitmapSetPixel(this.Pointer(), x, y, (uint)color.ToArgb()).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void SetResolution(float xDpi, float yDpi)
    {
        PInvokeGdiPlus.GdipBitmapSetResolution(this.Pointer(), xDpi, yDpi).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public Bitmap Clone(Rectangle rect, PixelFormat format)
    {
        if (rect.Width == 0 || rect.Height == 0)
        {
            throw new ArgumentException(SR.Format(SR.GdiplusInvalidRectangle, rect.ToString()));
        }

        GpBitmap* clone;
        Status status = PInvokeGdiPlus.GdipCloneBitmapAreaI(
            rect.X, rect.Y, rect.Width, rect.Height,
            (int)format,
            this.Pointer(),
            &clone);

        if (status != Status.Ok || clone is null)
        {
            throw Gdip.StatusException(status);
        }

        GC.KeepAlive(this);
        return new Bitmap(clone);
    }

#if NET9_0_OR_GREATER
    /// <summary>
    ///  Alters the bitmap by applying the given <paramref name="effect"/>.
    /// </summary>
    /// <param name="effect">The effect to apply.</param>
    /// <param name="area">The area to apply to, or <see cref="Rectangle.Empty"/> for the entire image.</param>
    public void ApplyEffect(Effect effect, Rectangle area = default)
    {
        RECT rect = area;
        PInvokeGdiPlus.GdipBitmapApplyEffect(
            this.Pointer(),
            effect.Pointer(),
            area.IsEmpty ? null : &rect,
            useAuxData: false,
            auxData: null,
            auxDataSize: null).ThrowIfFailed();

        GC.KeepAlive(this);
        GC.KeepAlive(effect);
    }

    /// <summary>
    ///  Converts the bitmap to the specified <paramref name="format"/> using the given <paramref name="ditherType"/>.
    ///  The original pixel data is replaced with the new format.
    /// </summary>
    /// <param name="format">
    ///  <para>
    ///   The new pixel format. <see cref="PixelFormat.Format16bppGrayScale"/> is not supported.
    ///  </para>
    /// </param>
    /// <param name="ditherType">
    ///  <para>
    ///   The dithering algorithm. Pass <see cref="DitherType.None"/> when the conversion does not reduce the bit depth
    ///   of the pixel data.
    ///  </para>
    ///  <para>
    ///   This must be <see cref="DitherType.Solid"/> or <see cref="DitherType.ErrorDiffusion"/> if the <paramref name="paletteType"/>
    ///   is <see cref="PaletteType.Custom"/> or <see cref="PaletteType.FixedBlackAndWhite"/>.
    ///  </para>
    /// </param>
    /// <param name="paletteType">
    ///  <para>
    ///   The palette type to use when the pixel format is indexed. Ignored for non-indexed pixel formats.
    ///  </para>
    /// </param>
    /// <param name="palette">
    ///  <para>
    ///   Pointer to a <see cref="ColorPalette"/> that specifies the palette whose indexes are stored in the pixel data
    ///   of the converted bitmap. This must be specified for indexed pixel formats.
    ///  </para>
    ///  <para>
    ///   This palette (called the actual palette) does not have to have the type specified by
    ///   the <paramref name="paletteType"/> parameter. The <paramref name="paletteType"/> parameter specifies a standard
    ///   palette that can be used by any of the ordered or spiral dithering algorithms. If the actual palette has a type
    ///   other than that specified by the <paramref name="paletteType"/> parameter, then
    ///   <see cref="ConvertFormat(PixelFormat, DitherType, PaletteType, ColorPalette?, float)"/> performs a nearest-color
    ///   conversion from the standard palette to the actual palette.
    ///  </para>
    /// </param>
    /// <param name="alphaThresholdPercent">
    ///  <para>
    ///   Real number in the range 0 through 100 that specifies which pixels in the source bitmap will map to the
    ///   transparent color in the converted bitmap.
    ///  </para>
    ///  <para>
    ///   A value of 0 specifies that none of the source pixels map to the transparent color. A value of 100
    ///   specifies that any pixel that is not fully opaque will map to the transparent color. A value of t specifies
    ///   that any source pixel less than t percent of fully opaque will map to the transparent color. Note that for
    ///   the alpha threshold to be effective, the palette must have a transparent color. If the palette does not have
    ///   a transparent color, pixels with alpha values below the threshold will map to color that most closely
    ///   matches (0, 0, 0, 0), usually black.
    ///  </para>
    /// </param>
    /// <remarks>
    ///  <para>
    ///   <paramref name="paletteType"/> and <paramref name="palette"/> really only have relevance with indexed pixel
    ///   formats. You can pass a <see cref="ColorPalette"/> for non-indexed pixel formats, but it has no impact on the
    ///   transformation and will effective just call <see cref="Image.Palette"/> to set the palette when the conversion
    ///   is complete.
    ///  </para>
    /// </remarks>
    public void ConvertFormat(
        PixelFormat format,
        DitherType ditherType,
        PaletteType paletteType = PaletteType.Custom,
        ColorPalette? palette = null,
        float alphaThresholdPercent = 0.0f)
    {
        if (palette is null)
        {
            PInvokeGdiPlus.GdipBitmapConvertFormat(
                this.Pointer(),
                (int)format,
                (GdiPlus.DitherType)ditherType,
                (GdiPlus.PaletteType)paletteType,
                null,
                alphaThresholdPercent).ThrowIfFailed();
        }
        else
        {
            using var buffer = palette.ConvertToBuffer();
            fixed (void* b = buffer)
            {
                PInvokeGdiPlus.GdipBitmapConvertFormat(
                    this.Pointer(),
                    (int)format,
                    (GdiPlus.DitherType)ditherType,
                    (GdiPlus.PaletteType)paletteType,
                    (GdiPlus.ColorPalette*)b,
                    alphaThresholdPercent).ThrowIfFailed();
            }
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Converts the bitmap to the specified <paramref name="format"/>.
    ///  The original pixel data is replaced with the new format.
    /// </summary>
    /// <param name="format">
    ///  <para>
    ///   The new pixel format. <see cref="PixelFormat.Format16bppGrayScale"/> is not supported.
    ///  </para>
    /// </param>
    public void ConvertFormat(PixelFormat format)
    {
        PixelFormat currentFormat = PixelFormat;
        int targetSize = ((int)format >> 8) & 0xff;
        int sourceSize = ((int)currentFormat >> 8) & 0xff;

        if (!format.HasFlag(PixelFormat.Indexed))
        {
            ConvertFormat(format, targetSize > sourceSize ? DitherType.None : DitherType.Solid);
            return;
        }

        int paletteSize = targetSize switch { 1 => 2, 4 => 16, _ => 256 };
        bool hasAlpha = format.HasFlag(PixelFormat.Alpha);
        if (hasAlpha)
        {
            paletteSize++;
        }

        ColorPalette palette = ColorPalette.CreateOptimalPalette(paletteSize, hasAlpha, this);
        ConvertFormat(format, DitherType.ErrorDiffusion, PaletteType.Custom, palette, .25f);
    }
#endif
}
