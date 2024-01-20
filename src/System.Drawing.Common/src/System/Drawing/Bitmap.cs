// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace System.Drawing;

[Editor($"System.Drawing.Design.BitmapEditor, {AssemblyRef.SystemDrawingDesign}",
        $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
[Serializable]
[Runtime.CompilerServices.TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public unsafe sealed class Bitmap : Image
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
                ? PInvoke.GdipCreateBitmapFromFileICM(fn, &bitmap)
                : PInvoke.GdipCreateBitmapFromFile(fn, &bitmap);
            status.ThrowIfFailed();
        }

        ValidateImage((GpImage*)bitmap);
        SetNativeImage((GpImage*)bitmap);
        EnsureSave(this, filename, null);
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
            ? PInvoke.GdipCreateBitmapFromStreamICM(iStream, &bitmap)
            : PInvoke.GdipCreateBitmapFromStream(iStream, &bitmap);
        status.ThrowIfFailed();

        ValidateImage((GpImage*)bitmap);
        SetNativeImage((GpImage*)bitmap);
        EnsureSave(this, null, stream);
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
        PInvoke.GdipCreateBitmapFromGraphics(width, height, g.NativeGraphics, &bitmap).ThrowIfFailed();
        SetNativeImage((GpImage*)bitmap);
        GC.KeepAlive(g);
    }

    public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
    {
        GpBitmap* bitmap;
        PInvoke.GdipCreateBitmapFromScan0(width, height, stride, (int)format, (byte*)scan0, &bitmap).ThrowIfFailed();
        SetNativeImage((GpImage*)bitmap);
    }

    public Bitmap(int width, int height, PixelFormat format)
    {
        GpBitmap* bitmap;
        PInvoke.GdipCreateBitmapFromScan0(width, height, 0, (int)format, null, &bitmap).ThrowIfFailed();
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

    internal GpBitmap* NativeBitmap => (GpBitmap*)_nativeImage;

    public static Bitmap FromHicon(IntPtr hicon)
    {
        GpBitmap* bitmap;
        PInvoke.GdipCreateBitmapFromHICON((HICON)hicon, &bitmap).ThrowIfFailed();
        return new Bitmap(bitmap);
    }

    public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
    {
        ArgumentNullException.ThrowIfNull(bitmapName);
        GpBitmap* bitmap = null;
        fixed (char* bn = bitmapName)
        {
            PInvoke.GdipCreateBitmapFromResource((HINSTANCE)hinstance, bn, &bitmap).ThrowIfFailed();
        }

        return new Bitmap(bitmap);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHbitmap() => GetHbitmap(Color.LightGray);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHbitmap(Color background)
    {
        HBITMAP hbitmap;
        Status status = PInvoke.GdipCreateHBITMAPFromBitmap(
            NativeBitmap,
            &hbitmap,
            (uint)ColorTranslator.ToWin32(background));

        if (status == Status.InvalidParameter && (Width >= short.MaxValue || Height >= short.MaxValue))
        {
            throw new ArgumentException(SR.GdiplusInvalidSize);
        }

        status.ThrowIfFailed();
        GC.KeepAlive(this);
        return hbitmap;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHicon()
    {
        HICON hicon;
        PInvoke.GdipCreateHICONFromBitmap(NativeBitmap, &hicon).ThrowIfFailed();
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

        Status status = PInvoke.GdipCloneBitmapArea(
            rect.X, rect.Y, rect.Width, rect.Height,
            (int)format,
            NativeBitmap,
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
        // thing that supports alpha.  (And that's what the image is initialized to -- transparent)
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
                null,
                IntPtr.Zero);
        }

        // Swap nativeImage pointers to make it look like we modified the image in place
        GpImage* temp = _nativeImage;
        _nativeImage = result._nativeImage;
        result._nativeImage = temp;
    }

    public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format) =>
        LockBits(rect, flags, format, new());

    public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
    {
        ArgumentNullException.ThrowIfNull(bitmapData);

        Rect nativeRect = rect;

        fixed (void* data = &bitmapData.GetPinnableReference())
        {
            PInvoke.GdipBitmapLockBits(
                NativeBitmap,
                &nativeRect,
                (uint)flags,
                (int)format,
                (GdiPlus.BitmapData*)data).ThrowIfFailed();
        }

        GC.KeepAlive(this);
        return bitmapData;
    }

    public void UnlockBits(BitmapData bitmapdata)
    {
        ArgumentNullException.ThrowIfNull(bitmapdata);

        fixed (void* data = &bitmapdata.GetPinnableReference())
        {
            PInvoke.GdipBitmapUnlockBits(NativeBitmap, (GdiPlus.BitmapData*)data).ThrowIfFailed();
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
        PInvoke.GdipBitmapGetPixel(NativeBitmap, x, y, &color).ThrowIfFailed();
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

        PInvoke.GdipBitmapSetPixel(NativeBitmap, x, y, (uint)color.ToArgb()).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public void SetResolution(float xDpi, float yDpi)
    {
        PInvoke.GdipBitmapSetResolution(NativeBitmap, xDpi, yDpi).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    public Bitmap Clone(Rectangle rect, PixelFormat format)
    {
        if (rect.Width == 0 || rect.Height == 0)
        {
            throw new ArgumentException(SR.Format(SR.GdiplusInvalidRectangle, rect.ToString()));
        }

        GpBitmap* clone;
        Status status = PInvoke.GdipCloneBitmapAreaI(
            rect.X, rect.Y, rect.Width, rect.Height,
            (int)format,
            NativeBitmap,
            &clone);

        if (status != Status.Ok || clone is null)
        {
            throw Gdip.StatusException(status);
        }

        GC.KeepAlive(this);
        return new Bitmap(clone);
    }
}
