// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Windows.Win32.System.Com;

namespace System.Drawing;

/// <summary>
///  An abstract base class that provides functionality for 'Bitmap', 'Icon', 'Cursor', and 'Metafile' descended classes.
/// </summary>
[Editor($"System.Drawing.Design.ImageEditor, {AssemblyRef.SystemDrawingDesign}",
        $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
[ImmutableObject(true)]
[Serializable]
[Runtime.CompilerServices.TypeForwardedFrom(AssemblyRef.SystemDrawing)]
[TypeConverter(typeof(ImageConverter))]
public abstract unsafe class Image : MarshalByRefObject, IImage, IDisposable, ICloneable, ISerializable
{
    // The signature of this delegate is incorrect. The signature of the corresponding
    // native callback function is:
    // extern "C" {
    //     typedef BOOL (CALLBACK * ImageAbort)(VOID *);
    //     typedef ImageAbort DrawImageAbort;
    //     typedef ImageAbort GetThumbnailImageAbort;
    // }
    // However, as this delegate is not used in both GDI 1.0 and 1.1, we choose not
    // to modify it, in order to preserve compatibility.
    public delegate bool GetThumbnailImageAbort();

    nint IPointer<GpImage>.Pointer => (nint)_nativeImage;

    [NonSerialized]
    private GpImage* _nativeImage;

    private object? _userData;

    // Used to work around lack of animated gif encoder.
    private byte[]? _animatedGifRawData;
    ReadOnlySpan<byte> IRawData.Data => _animatedGifRawData;

    [Localizable(false)]
    [DefaultValue(null)]
    public object? Tag
    {
        get => _userData;
        set => _userData = value;
    }

    private protected Image() { }

    private protected Image(SerializationInfo info, StreamingContext context)
    {
        byte[] dat = (byte[])info.GetValue("Data", typeof(byte[]))!; // Do not rename (binary serialization)

        try
        {
            SetNativeImage(InitializeFromStream(new MemoryStream(dat)));
        }
        catch (Exception e) when (e is ExternalException
            or ArgumentException
            or OutOfMemoryException
            or InvalidOperationException
            or NotImplementedException
            or FileNotFoundException)
        {
        }
    }

    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
    {
        using MemoryStream stream = new();
        this.Save(stream);
        si.AddValue("Data", stream.ToArray(), typeof(byte[])); // Do not rename (binary serialization)
    }

    /// <summary>
    ///  Creates an <see cref='Image'/> from the specified file.
    /// </summary>
    public static Image FromFile(string filename) => FromFile(filename, false);

    public static Image FromFile(string filename, bool useEmbeddedColorManagement)
    {
        if (!File.Exists(filename))
        {
            // Throw a more specific exception for invalid paths that are null or empty,
            // contain invalid characters or are too long.
            filename = Path.GetFullPath(filename);
            throw new FileNotFoundException(filename);
        }

        // GDI+ will read this file multiple times. Get the fully qualified path
        // so if our app changes default directory we won't get an error
        filename = Path.GetFullPath(filename);

        GpImage* image = null;

        fixed (char* fn = filename)
        {
            if (useEmbeddedColorManagement)
            {
                PInvokeGdiPlus.GdipLoadImageFromFileICM(fn, &image).ThrowIfFailed();
            }
            else
            {
                PInvokeGdiPlus.GdipLoadImageFromFile(fn, &image).ThrowIfFailed();
            }
        }

        ValidateImage(image);

        Image img = CreateImageObject(image);
        GetAnimatedGifRawData(img, filename, dataStream: null);
        return img;
    }

    /// <summary>
    ///  Creates an <see cref='Image'/> from the specified data stream.
    /// </summary>
    public static Image FromStream(Stream stream) => FromStream(stream, useEmbeddedColorManagement: false);

    public static Image FromStream(Stream stream, bool useEmbeddedColorManagement) =>
        FromStream(stream, useEmbeddedColorManagement, true);

    public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
    {
        ArgumentNullException.ThrowIfNull(stream);
        GpImage* image = LoadGdipImageFromStream(stream, useEmbeddedColorManagement);

        if (validateImageData)
        {
            ValidateImage(image);
        }

        Image img = CreateImageObject(image);
        GetAnimatedGifRawData(img, filename: null, stream);
        return img;
    }

    // Used for serialization
    private GpImage* InitializeFromStream(Stream stream)
    {
        GpImage* image = LoadGdipImageFromStream(stream, useEmbeddedColorManagement: false);
        ValidateImage(image);
        _nativeImage = image;
        GdiPlus.ImageType type = default;
        PInvokeGdiPlus.GdipGetImageType(_nativeImage, &type).ThrowIfFailed();
        GetAnimatedGifRawData(this, filename: null, stream);
        return image;
    }

    private static GpImage* LoadGdipImageFromStream(Stream stream, bool useEmbeddedColorManagement)
    {
        using var iStream = stream.ToIStream(makeSeekable: true);
        return LoadGdipImageFromStream(iStream, useEmbeddedColorManagement);
    }

    private static unsafe GpImage* LoadGdipImageFromStream(IStream* stream, bool useEmbeddedColorManagement)
    {
        GpImage* image;

        if (useEmbeddedColorManagement)
        {
            PInvokeGdiPlus.GdipLoadImageFromStreamICM(stream, &image).ThrowIfFailed();
        }
        else
        {
            PInvokeGdiPlus.GdipLoadImageFromStream(stream, &image).ThrowIfFailed();
        }

        return image;
    }

    internal Image(GpImage* nativeImage) => SetNativeImage(nativeImage);

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='Image'/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='Image'/>.
    /// </summary>
    ~Image() => Dispose(disposing: false);

    /// <summary>
    ///  Creates an exact copy of this <see cref='Image'/>.
    /// </summary>
    public object Clone()
    {
        GpImage* cloneImage;
        PInvokeGdiPlus.GdipCloneImage(_nativeImage, &cloneImage).ThrowIfFailed();
        ValidateImage(cloneImage);
        GC.KeepAlive(this);
        return CreateImageObject(cloneImage);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_nativeImage is null)
        {
            return;
        }

        Status status = !Gdip.Initialized ? Status.Ok : PInvokeGdiPlus.GdipDisposeImage(_nativeImage);
        _nativeImage = null;
        Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
    }

    /// <summary>
    ///  Saves this <see cref='Image'/> to the specified file.
    /// </summary>
    public void Save(string filename) => Save(filename, RawFormat);

    /// <summary>
    ///  Saves this <see cref='Image'/> to the specified file in the specified format.
    /// </summary>
    public void Save(string filename, ImageFormat format)
    {
        ArgumentNullException.ThrowIfNull(format);

        Guid encoder = format.Encoder;
        if (encoder == Guid.Empty)
        {
            encoder = ImageCodecInfoHelper.GetEncoderClsid(PInvokeGdiPlus.ImageFormatPNG);
        }

        Save(filename, encoder, null);
    }

    /// <summary>
    ///  Saves this <see cref='Image'/> to the specified file in the specified format and with the specified encoder parameters.
    /// </summary>
    public void Save(string filename, ImageCodecInfo encoder, Imaging.EncoderParameters? encoderParams)
        => Save(filename, encoder.Clsid, encoderParams);

    private void Save(string filename, Guid encoder, Imaging.EncoderParameters? encoderParams)
    {
        ArgumentNullException.ThrowIfNull(filename);
        if (encoder == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(encoder));
        }

        ThrowIfDirectoryDoesntExist(filename);

        GdiPlus.EncoderParameters* nativeParameters = null;

        if (encoderParams is not null)
        {
            _animatedGifRawData = null;
            nativeParameters = encoderParams.ConvertToNative();
        }

        try
        {
            if (_animatedGifRawData is not null && RawFormat.Encoder == encoder)
            {
                // Special case for animated gifs. We don't have an encoder for them, so we just write the raw data.
                using var fs = File.OpenWrite(filename);
                fs.Write(_animatedGifRawData, 0, _animatedGifRawData.Length);
                return;
            }

            fixed (char* fn = filename)
            {
                PInvokeGdiPlus.GdipSaveImageToFile(_nativeImage, fn, &encoder, nativeParameters).ThrowIfFailed();
            }
        }
        finally
        {
            if (nativeParameters is not null)
            {
                Marshal.FreeHGlobal((nint)nativeParameters);
            }

            GC.KeepAlive(this);
            GC.KeepAlive(encoderParams);
        }
    }

    /// <summary>
    ///  Saves this <see cref='Image'/> to the specified stream in the specified format.
    /// </summary>
    public void Save(Stream stream, ImageFormat format)
    {
        ArgumentNullException.ThrowIfNull(format);
        this.Save(stream, format.Encoder, format.Guid, encoderParameters: null);
    }

    /// <summary>
    ///  Saves this <see cref='Image'/> to the specified stream in the specified format.
    /// </summary>
    public void Save(Stream stream, ImageCodecInfo encoder, Imaging.EncoderParameters? encoderParams)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(encoder);

        GdiPlus.EncoderParameters* nativeParameters = null;

        if (encoderParams is not null)
        {
            _animatedGifRawData = null;
            nativeParameters = encoderParams.ConvertToNative();
        }

        try
        {
            this.Save(stream, encoder.Clsid, encoder.FormatID, nativeParameters);
        }
        finally
        {
            if (nativeParameters is not null)
            {
                Marshal.FreeHGlobal((nint)nativeParameters);
            }

            GC.KeepAlive(this);
            GC.KeepAlive(encoderParams);
        }
    }

    /// <summary>
    ///  Adds an <see cref='EncoderParameters'/> to this <see cref='Image'/>.
    /// </summary>
    public void SaveAdd(Imaging.EncoderParameters? encoderParams)
    {
        GdiPlus.EncoderParameters* nativeParameters = null;
        if (encoderParams is not null)
        {
            nativeParameters = encoderParams.ConvertToNative();
        }

        _animatedGifRawData = null;

        try
        {
            PInvokeGdiPlus.GdipSaveAdd(_nativeImage, nativeParameters).ThrowIfFailed();
        }
        finally
        {
            if (nativeParameters is not null)
            {
                Marshal.FreeHGlobal((nint)nativeParameters);
            }

            GC.KeepAlive(this);
            GC.KeepAlive(encoderParams);
        }
    }

    /// <summary>
    ///  Adds an <see cref='EncoderParameters'/> to the specified <see cref='Image'/>.
    /// </summary>
    public void SaveAdd(Image image, Imaging.EncoderParameters? encoderParams)
    {
        ArgumentNullException.ThrowIfNull(image);

        GdiPlus.EncoderParameters* nativeParameters = null;

        if (encoderParams is not null)
        {
            nativeParameters = encoderParams.ConvertToNative();
        }

        _animatedGifRawData = null;

        try
        {
            PInvokeGdiPlus.GdipSaveAddImage(_nativeImage, image._nativeImage, nativeParameters).ThrowIfFailed();
        }
        finally
        {
            if (nativeParameters is not null)
            {
                Marshal.FreeHGlobal((nint)nativeParameters);
            }

            GC.KeepAlive(this);
            GC.KeepAlive(image);
            GC.KeepAlive(encoderParams);
        }
    }

    private static void ThrowIfDirectoryDoesntExist(string filename)
    {
        string? directoryPart = Path.GetDirectoryName(filename);
        if (!string.IsNullOrEmpty(directoryPart) && !Directory.Exists(directoryPart))
        {
            throw new DirectoryNotFoundException(SR.Format(SR.TargetDirectoryDoesNotExist, directoryPart, filename));
        }
    }

    /// <summary>
    ///  Gets the width and height of this <see cref='Image'/>.
    /// </summary>
    public SizeF PhysicalDimension
    {
        get
        {
            float width;
            float height;

            PInvokeGdiPlus.GdipGetImageDimension(_nativeImage, &width, &height).ThrowIfFailed();
            GC.KeepAlive(this);
            return new SizeF(width, height);
        }
    }

    /// <summary>
    ///  Gets the width and height of this <see cref='Image'/>.
    /// </summary>
    public Size Size => new(Width, Height);

    /// <summary>
    ///  Gets the width of this <see cref='Image'/>.
    /// </summary>
    [DefaultValue(false)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Width
    {
        get
        {
            uint width;
            PInvokeGdiPlus.GdipGetImageWidth(_nativeImage, &width).ThrowIfFailed();
            GC.KeepAlive(this);
            return (int)width;
        }
    }

    /// <summary>
    ///  Gets the height of this <see cref='Image'/>.
    /// </summary>
    [DefaultValue(false)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int Height
    {
        get
        {
            uint height;
            PInvokeGdiPlus.GdipGetImageHeight(_nativeImage, &height).ThrowIfFailed();
            GC.KeepAlive(this);
            return (int)height;
        }
    }

    /// <summary>
    ///  Gets the horizontal resolution, in pixels-per-inch, of this <see cref='Image'/>.
    /// </summary>
    public float HorizontalResolution
    {
        get
        {
            float horzRes;
            PInvokeGdiPlus.GdipGetImageHorizontalResolution(_nativeImage, &horzRes).ThrowIfFailed();
            GC.KeepAlive(this);
            return horzRes;
        }
    }

    /// <summary>
    ///  Gets the vertical resolution, in pixels-per-inch, of this <see cref='Image'/>.
    /// </summary>
    public float VerticalResolution
    {
        get
        {
            float vertRes;
            PInvokeGdiPlus.GdipGetImageVerticalResolution(_nativeImage, &vertRes).ThrowIfFailed();
            GC.KeepAlive(this);
            return vertRes;
        }
    }

    /// <summary>
    ///  Gets attribute flags for this <see cref='Image'/>.
    /// </summary>
    [Browsable(false)]
    public int Flags
    {
        get
        {
            uint flags;
            PInvokeGdiPlus.GdipGetImageFlags(_nativeImage, &flags).ThrowIfFailed();
            GC.KeepAlive(this);
            return (int)flags;
        }
    }

    /// <summary>
    ///  Gets the format of this <see cref='Image'/>.
    /// </summary>
    public ImageFormat RawFormat
    {
        get
        {
            Guid guid = default;
            PInvokeGdiPlus.GdipGetImageRawFormat(_nativeImage, &guid).ThrowIfFailed();
            GC.KeepAlive(this);
            return new ImageFormat(guid);
        }
    }

    /// <summary>
    ///  Gets the pixel format for this <see cref='Image'/>.
    /// </summary>
    public PixelFormat PixelFormat => (PixelFormat)this.GetPixelFormat();

    /// <summary>
    ///  Gets an array of the property IDs stored in this <see cref='Image'/>.
    /// </summary>
    [Browsable(false)]
    public int[] PropertyIdList
    {
        get
        {
            uint count;
            PInvokeGdiPlus.GdipGetPropertyCount(_nativeImage, &count).ThrowIfFailed();
            if (count == 0)
            {
                return [];
            }

            int[] propid = new int[count];
            fixed (int* pPropid = propid)
            {
                PInvokeGdiPlus.GdipGetPropertyIdList(_nativeImage, count, (uint*)pPropid).ThrowIfFailed();
            }

            GC.KeepAlive(this);
            return propid;
        }
    }

    /// <summary>
    ///  Gets an array of <see cref='PropertyItem'/> objects that describe this <see cref='Image'/>.
    /// </summary>
    [Browsable(false)]
    public Imaging.PropertyItem[] PropertyItems
    {
        get
        {
            uint size, count;
            PInvokeGdiPlus.GdipGetPropertySize(_nativeImage, &size, &count).ThrowIfFailed();

            if (size == 0 || count == 0)
            {
                return [];
            }

            Imaging.PropertyItem[] result = new Imaging.PropertyItem[(int)count];
            using BufferScope<byte> buffer = new((int)size);
            fixed (byte* b = buffer)
            {
                GdiPlus.PropertyItem* properties = (GdiPlus.PropertyItem*)b;
                PInvokeGdiPlus.GdipGetAllPropertyItems(_nativeImage, size, count, properties);

                for (int i = 0; i < count; i++)
                {
                    result[i] = Imaging.PropertyItem.FromNative(properties + i);
                }
            }

            GC.KeepAlive(this);
            return result;
        }
    }

    /// <summary>
    ///  Gets a bounding rectangle in the specified units for this <see cref='Image'/>.
    /// </summary>
    public RectangleF GetBounds(ref GraphicsUnit pageUnit)
    {
        // The Unit is hard coded to GraphicsUnit.Pixel in GDI+.
        RectangleF bounds = this.GetImageBounds();
        pageUnit = GraphicsUnit.Pixel;
        return bounds;
    }

    /// <summary>
    ///  Gets or sets the color palette used for this <see cref='Image'/>.
    /// </summary>
    [Browsable(false)]
    public ColorPalette Palette
    {
        get
        {
            // "size" is total byte size:
            // sizeof(ColorPalette) + (pal->Count-1)*sizeof(ARGB)

            int size;
            PInvokeGdiPlus.GdipGetImagePaletteSize(_nativeImage, &size).ThrowIfFailed();

            using BufferScope<uint> buffer = new(size / sizeof(uint));
            fixed (uint* b = buffer)
            {
                PInvokeGdiPlus.GdipGetImagePalette(_nativeImage, (GdiPlus.ColorPalette*)b, size).ThrowIfFailed();
                GC.KeepAlive(this);
                return ColorPalette.ConvertFromBuffer(buffer);
            }
        }
        set
        {
            using BufferScope<uint> buffer = value.ConvertToBuffer();
            fixed (uint* b = buffer)
            {
                PInvokeGdiPlus.GdipSetImagePalette(_nativeImage, (GdiPlus.ColorPalette*)b).ThrowIfFailed();
                GC.KeepAlive(this);
            }
        }
    }

    // Thumbnail support

    /// <summary>
    ///  Returns the thumbnail for this <see cref='Image'/>.
    /// </summary>
    public Image GetThumbnailImage(int thumbWidth, int thumbHeight, GetThumbnailImageAbort? callback, IntPtr callbackData)
    {
        GpImage* thumbImage;

        // GDI+ had to ignore the callback as System.Drawing didn't define it correctly so it was eventually removed
        // completely in Windows 7. As such, we don't need to pass it to GDI+.
        PInvokeGdiPlus.GdipGetImageThumbnail(
            this.Pointer(),
            (uint)thumbWidth,
            (uint)thumbHeight,
            &thumbImage,
            0,
            null).ThrowIfFailed();

        GC.KeepAlive(this);
        return CreateImageObject(thumbImage);
    }

    internal static void ValidateImage(GpImage* image)
    {
        try
        {
            PInvokeGdiPlus.GdipImageForceValidation(image).ThrowIfFailed();
        }
        catch
        {
            PInvokeGdiPlus.GdipDisposeImage(image);
            throw;
        }
    }

    /// <summary>
    ///  Returns the number of frames of the given dimension.
    /// </summary>
    public int GetFrameCount(FrameDimension dimension)
    {
        Guid dimensionID = dimension.Guid;
        uint count;
        PInvokeGdiPlus.GdipImageGetFrameCount(_nativeImage, &dimensionID, &count).ThrowIfFailed();
        GC.KeepAlive(this);
        return (int)count;
    }

    /// <summary>
    ///  Gets the specified property item from this <see cref='Image'/>.
    /// </summary>
    public Imaging.PropertyItem? GetPropertyItem(int propid)
    {
        uint size;
        PInvokeGdiPlus.GdipGetPropertyItemSize(_nativeImage, (uint)propid, &size).ThrowIfFailed();

        if (size == 0)
        {
            return null;
        }

        using BufferScope<byte> buffer = new((int)size);
        fixed (byte* b = buffer)
        {
            GdiPlus.PropertyItem* property = (GdiPlus.PropertyItem*)b;
            PInvokeGdiPlus.GdipGetPropertyItem(_nativeImage, (uint)propid, size, property).ThrowIfFailed();
            GC.KeepAlive(this);
            return Imaging.PropertyItem.FromNative(property);
        }
    }

    /// <summary>
    ///  Selects the frame specified by the given dimension and index.
    /// </summary>
    public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
    {
        Guid dimensionID = dimension.Guid;
        PInvokeGdiPlus.GdipImageSelectActiveFrame(_nativeImage, &dimensionID, (uint)frameIndex).ThrowIfFailed();
        GC.KeepAlive(this);
        return 0;
    }

    /// <summary>
    ///  Sets the specified property item to the specified value.
    /// </summary>
    public unsafe void SetPropertyItem(Imaging.PropertyItem propitem)
    {
        fixed (byte* propItemValue = propitem.Value)
        {
            GdiPlus.PropertyItem native = new()
            {
                id = (uint)propitem.Id,
                length = (uint)propitem.Len,
                type = (ushort)propitem.Type,
                value = propItemValue
            };

            PInvokeGdiPlus.GdipSetPropertyItem(_nativeImage, &native).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    public void RotateFlip(RotateFlipType rotateFlipType)
    {
        PInvokeGdiPlus.GdipImageRotateFlip(_nativeImage, (GdiPlus.RotateFlipType)rotateFlipType).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Removes the specified property item from this <see cref='Image'/>.
    /// </summary>
    public void RemovePropertyItem(int propid)
    {
        PInvokeGdiPlus.GdipRemovePropertyItem(_nativeImage, (uint)propid).ThrowIfFailed();
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Returns information about the codecs used for this <see cref='Image'/>.
    /// </summary>
    public unsafe Imaging.EncoderParameters? GetEncoderParameterList(Guid encoder)
    {
        Imaging.EncoderParameters parameters;

        uint size;
        PInvokeGdiPlus.GdipGetEncoderParameterListSize(_nativeImage, &encoder, &size).ThrowIfFailed();

        if (size <= 0)
        {
            return null;
        }

        using BufferScope<byte> buffer = new((int)size);
        fixed (byte* b = buffer)
        {
            PInvokeGdiPlus.GdipGetEncoderParameterList(
                _nativeImage,
                &encoder,
                size,
                (GdiPlus.EncoderParameters*)b).ThrowIfFailed();

            parameters = Imaging.EncoderParameters.ConvertFromNative((GdiPlus.EncoderParameters*)b);
            GC.KeepAlive(this);
        }

        return parameters;
    }

    /// <summary>
    ///  Creates a <see cref='Bitmap'/> from a Windows handle.
    /// </summary>
    public static Bitmap FromHbitmap(IntPtr hbitmap) => FromHbitmap(hbitmap, IntPtr.Zero);

    /// <summary>
    ///  Creates a <see cref='Bitmap'/> from the specified Windows handle with the specified color palette.
    /// </summary>
    public static Bitmap FromHbitmap(IntPtr hbitmap, IntPtr hpalette)
    {
        GpBitmap* bitmap;
        PInvokeGdiPlus.GdipCreateBitmapFromHBITMAP((HBITMAP)hbitmap, (HPALETTE)hpalette, &bitmap).ThrowIfFailed();
        return new Bitmap(bitmap);
    }

    /// <summary>
    ///  Returns a value indicating whether the pixel format is extended.
    /// </summary>
    public static bool IsExtendedPixelFormat(PixelFormat pixfmt) => (pixfmt & PixelFormat.Extended) != 0;

    /// <summary>
    ///  Returns a value indicating whether the pixel format is canonical.
    /// </summary>
    public static bool IsCanonicalPixelFormat(PixelFormat pixfmt)
    {
        // Canonical formats:
        //
        //  PixelFormat32bppARGB
        //  PixelFormat32bppPARGB
        //  PixelFormat64bppARGB
        //  PixelFormat64bppPARGB

        return (pixfmt & PixelFormat.Canonical) != 0;
    }

    internal void SetNativeImage(GpImage* handle)
    {
        if (handle is null)
            throw new ArgumentException(SR.NativeHandle0, nameof(handle));

        _nativeImage = handle;
    }

    // Multi-frame support

    /// <summary>
    ///  Gets an array of GUIDs that represent the dimensions of frames within this <see cref='Image'/>.
    /// </summary>
    [Browsable(false)]
    public unsafe Guid[] FrameDimensionsList
    {
        get
        {
            uint count;
            PInvokeGdiPlus.GdipImageGetFrameDimensionsCount(_nativeImage, &count).ThrowIfFailed();

            Debug.Assert(count >= 0, "FrameDimensionsList returns bad count");
            if (count <= 0)
            {
                return [];
            }

            Guid[] guids = new Guid[count];
            fixed (Guid* g = guids)
            {
                PInvokeGdiPlus.GdipImageGetFrameDimensionsList(_nativeImage, g, count).ThrowIfFailed();
            }

            GC.KeepAlive(this);
            return guids;
        }
    }

    /// <summary>
    ///  Returns the size of the specified pixel format.
    /// </summary>
    public static int GetPixelFormatSize(PixelFormat pixfmt) => ((int)pixfmt >> 8) & 0xFF;

    /// <summary>
    ///  Returns a value indicating whether the pixel format contains alpha information.
    /// </summary>
    public static bool IsAlphaPixelFormat(PixelFormat pixfmt) => (pixfmt & PixelFormat.Alpha) != 0;

    internal static Image CreateImageObject(GpImage* nativeImage)
    {
        GdiPlus.ImageType imageType = default;
        PInvokeGdiPlus.GdipGetImageType(nativeImage, &imageType);
        return imageType switch
        {
            GdiPlus.ImageType.ImageTypeBitmap => new Bitmap((GpBitmap*)nativeImage),
            GdiPlus.ImageType.ImageTypeMetafile => new Metafile((nint)nativeImage),
            _ => throw new ArgumentException(SR.InvalidImage),
        };
    }

    /// <summary>
    ///  If the image is an animated GIF, loads the raw data for the image into the _rawData field so we
    ///  can work around the lack of an animated GIF encoder.
    /// </summary>
    internal static unsafe void GetAnimatedGifRawData(Image image, string? filename, Stream? dataStream)
    {
        if (!image.RawFormat.Equals(ImageFormat.Gif))
        {
            return;
        }

        bool animatedGif = false;

        uint dimensions;
        PInvokeGdiPlus.GdipImageGetFrameDimensionsCount(image._nativeImage, &dimensions).ThrowIfFailed();
        if (dimensions <= 0)
        {
            return;
        }

        using BufferScope<Guid> guids = new(stackalloc Guid[16], (int)dimensions);

        fixed (Guid* g = guids)
        {
            PInvokeGdiPlus.GdipImageGetFrameDimensionsList(image._nativeImage, g, dimensions).ThrowIfFailed();
        }

        Guid timeGuid = FrameDimension.Time.Guid;
        for (int i = 0; i < dimensions; i++)
        {
            if (timeGuid == guids[i])
            {
                animatedGif = image.GetFrameCount(FrameDimension.Time) > 1;
                break;
            }
        }

        if (!animatedGif)
        {
            return;
        }

        try
        {
            Stream? created = null;
            long lastPos = 0;
            if (dataStream is not null)
            {
                lastPos = dataStream.Position;
                dataStream.Position = 0;
            }

            try
            {
                if (dataStream is null)
                {
                    created = dataStream = File.OpenRead(filename ?? throw new InvalidOperationException());
                }

                image._animatedGifRawData = new byte[(int)dataStream.Length];
                dataStream.ReadExactly(image._animatedGifRawData, 0, (int)dataStream.Length);
            }
            finally
            {
                if (created is not null)
                {
                    created.Close();
                }
                else
                {
                    dataStream!.Position = lastPos;
                }
            }
        }
        catch (Exception e) when (e
            // possible exceptions for reading the filename
            is UnauthorizedAccessException
            or DirectoryNotFoundException
            or IOException
            // possible exceptions for setting/getting the position inside dataStream
            or NotSupportedException
            or ObjectDisposedException
            // possible exception when reading stuff into dataStream
            or ArgumentException)
        {
        }
    }
}
