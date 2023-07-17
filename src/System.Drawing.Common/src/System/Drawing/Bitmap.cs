﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.IO;
using System.Runtime.InteropServices;
using Gdip = System.Drawing.SafeNativeMethods.Gdip;
using System.Runtime.Serialization;

namespace System.Drawing;

[Editor($"System.Drawing.Design.BitmapEditor, {AssemblyRef.SystemDrawingDesign}",
        $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
[Serializable]
[System.Runtime.CompilerServices.TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public sealed class Bitmap : Image
{
    private static readonly Color s_defaultTransparentColor = Color.LightGray;

    private Bitmap() { }

    internal Bitmap(IntPtr ptr) => SetNativeImage(ptr);

    public Bitmap(string filename) : this(filename, useIcm: false) { }

    public Bitmap(string filename, bool useIcm)
    {
        // GDI+ will read this file multiple times. Get the fully qualified path
        // so if the app's default directory changes we won't get an error.
        filename = Path.GetFullPath(filename);

        IntPtr bitmap;
        int status;

        if (useIcm)
        {
            status = Gdip.GdipCreateBitmapFromFileICM(filename, out bitmap);
        }
        else
        {
            status = Gdip.GdipCreateBitmapFromFile(filename, out bitmap);
        }
        Gdip.CheckStatus(status);

        ValidateImage(bitmap);

        SetNativeImage(bitmap);
        EnsureSave(this, filename, null);
    }

    public Bitmap(Stream stream) : this(stream, false)
    {
    }

    public unsafe Bitmap(Stream stream, bool useIcm)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using DrawingCom.IStreamWrapper streamWrapper = DrawingCom.GetComWrapper(new GPStream(stream));

        IntPtr bitmap = IntPtr.Zero;
        if (useIcm)
        {
            Gdip.CheckStatus(Gdip.GdipCreateBitmapFromStreamICM(streamWrapper.Ptr, &bitmap));
        }
        else
        {
            Gdip.CheckStatus(Gdip.GdipCreateBitmapFromStream(streamWrapper.Ptr, &bitmap));
        }

        ValidateImage(bitmap);

        SetNativeImage(bitmap);
        EnsureSave(this, null, stream);
    }

    public Bitmap(Type type, string resource) : this(GetResourceStream(type, resource))
    {
    }

    private static Stream GetResourceStream(Type type, string resource)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(resource);

        Stream? stream = type.Module.Assembly.GetManifestResourceStream(type, resource);
        if (stream == null)
        {
            throw new ArgumentException(SR.Format(SR.ResourceNotFound, type, resource));
        }

        return stream;
    }

    public Bitmap(int width, int height) : this(width, height, PixelFormat.Format32bppArgb)
    {
    }

    public Bitmap(int width, int height, Graphics g)
    {
        ArgumentNullException.ThrowIfNull(g);

        IntPtr bitmap;
        int status = Gdip.GdipCreateBitmapFromGraphics(width, height, new HandleRef(g, g.NativeGraphics), out bitmap);
        Gdip.CheckStatus(status);

        SetNativeImage(bitmap);
    }

    public Bitmap(int width, int height, int stride, PixelFormat format, IntPtr scan0)
    {
        IntPtr bitmap;
        int status = Gdip.GdipCreateBitmapFromScan0(width, height, stride, unchecked((int)format), scan0, out bitmap);
        Gdip.CheckStatus(status);

        SetNativeImage(bitmap);
    }

    public Bitmap(int width, int height, PixelFormat format)
    {
        IntPtr bitmap;
        int status = Gdip.GdipCreateBitmapFromScan0(width, height, 0, unchecked((int)format), IntPtr.Zero, out bitmap);
        Gdip.CheckStatus(status);

        SetNativeImage(bitmap);
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

    public static Bitmap FromHicon(IntPtr hicon)
    {
        Gdip.CheckStatus(Gdip.GdipCreateBitmapFromHICON(hicon, out IntPtr bitmap));
        return new Bitmap(bitmap);
    }

    public static Bitmap FromResource(IntPtr hinstance, string bitmapName)
    {
        IntPtr name = Marshal.StringToHGlobalUni(bitmapName);
        try
        {
            Gdip.CheckStatus(Gdip.GdipCreateBitmapFromResource(hinstance, name, out IntPtr bitmap));
            return new Bitmap(bitmap);
        }
        finally
        {
            Marshal.FreeHGlobal(name);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHbitmap() => GetHbitmap(Color.LightGray);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHbitmap(Color background)
    {
        IntPtr hBitmap;
        int status = Gdip.GdipCreateHBITMAPFromBitmap(new HandleRef(this, _nativeImage), out hBitmap,
                                                         ColorTranslator.ToWin32(background));
        if (status == 2 /* invalid parameter*/ && (Width >= short.MaxValue || Height >= short.MaxValue))
        {
            throw new ArgumentException(SR.GdiplusInvalidSize);
        }

        Gdip.CheckStatus(status);

        return hBitmap;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IntPtr GetHicon()
    {
        IntPtr hIcon;
        int status = Gdip.GdipCreateHICONFromBitmap(new HandleRef(this, _nativeImage), out hIcon);
        Gdip.CheckStatus(status);

        return hIcon;
    }

    public Bitmap Clone(RectangleF rect, PixelFormat format)
    {
        if (rect.Width == 0 || rect.Height == 0)
        {
            throw new ArgumentException(SR.Format(SR.GdiplusInvalidRectangle, rect.ToString()));
        }

        IntPtr dstHandle;

        int status = Gdip.GdipCloneBitmapArea(
                                                rect.X,
                                                rect.Y,
                                                rect.Width,
                                                rect.Height,
                                                unchecked((int)format),
                                                new HandleRef(this, _nativeImage),
                                                out dstHandle);

        if (status != Gdip.Ok || dstHandle == IntPtr.Zero)
            throw Gdip.StatusException(status);

        return new Bitmap(dstHandle);
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
        using var result = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(result);

        graphics.Clear(Color.Transparent);
        Rectangle rectangle = new Rectangle(0, 0, size.Width, size.Height);

        using (var attributes = new ImageAttributes())
        {
            attributes.SetColorKey(transparentColor, transparentColor);
            graphics.DrawImage(this, rectangle,
                                0, 0, size.Width, size.Height,
                                GraphicsUnit.Pixel, attributes, null, IntPtr.Zero);
        }

        // Swap nativeImage pointers to make it look like we modified the image in place
        IntPtr temp = _nativeImage;
        _nativeImage = result._nativeImage;
        result._nativeImage = temp;
    }

    public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format)
    {
        return LockBits(rect, flags, format, new BitmapData());
    }

    public BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format, BitmapData bitmapData)
    {
        int status = Gdip.GdipBitmapLockBits(
            new HandleRef(this, _nativeImage), ref rect, flags, format, bitmapData);

        // libgdiplus has the wrong error code mapping for this state.
        if (status == 7)
        {
            status = 8;
        }
        Gdip.CheckStatus(status);

        return bitmapData;
    }

    public void UnlockBits(BitmapData bitmapdata)
    {
        int status = Gdip.GdipBitmapUnlockBits(new HandleRef(this, _nativeImage), bitmapdata);
        Gdip.CheckStatus(status);
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

        int color;
        int status = Gdip.GdipBitmapGetPixel(new HandleRef(this, _nativeImage), x, y, out color);
        Gdip.CheckStatus(status);

        return Color.FromArgb(color);
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

        int status = Gdip.GdipBitmapSetPixel(new HandleRef(this, _nativeImage), x, y, color.ToArgb());
        Gdip.CheckStatus(status);
    }

    public void SetResolution(float xDpi, float yDpi)
    {
        int status = Gdip.GdipBitmapSetResolution(new HandleRef(this, _nativeImage), xDpi, yDpi);
        Gdip.CheckStatus(status);
    }
    public Bitmap Clone(Rectangle rect, PixelFormat format)
    {
        if (rect.Width == 0 || rect.Height == 0)
        {
            throw new ArgumentException(SR.Format(SR.GdiplusInvalidRectangle, rect.ToString()));
        }

        IntPtr dstHandle;
        int status = Gdip.GdipCloneBitmapAreaI(
                                                 rect.X,
                                                 rect.Y,
                                                 rect.Width,
                                                 rect.Height,
                                                 unchecked((int)format),
                                                 new HandleRef(this, _nativeImage),
                                                 out dstHandle);

        if (status != Gdip.Ok || dstHandle == IntPtr.Zero)
            throw Gdip.StatusException(status);

        return new Bitmap(dstHandle);
    }
}
