// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Internal;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using static Interop;

namespace System.Drawing;

[Editor($"System.Drawing.Design.IconEditor, {AssemblyRef.SystemDrawingDesign}",
        $"System.Drawing.Design.UITypeEditor, {AssemblyRef.SystemDrawing}")]
[TypeConverter(typeof(IconConverter))]
[Serializable]
[TypeForwardedFrom(AssemblyRef.SystemDrawing)]
public sealed partial class Icon : MarshalByRefObject, ICloneable, IDisposable, ISerializable
{
#if FINALIZATION_WATCH
    private string allocationSite = Graphics.GetAllocationStack();
#endif

    private static int s_bitDepth;

    // The PNG signature is specified at http://www.w3.org/TR/PNG/#5PNG-file-signature
    private const int PNGSignature1 = 137 + ('P' << 8) + ('N' << 16) + ('G' << 24);
    private const int PNGSignature2 = 13 + (10 << 8) + (26 << 16) + (10 << 24);

    // Icon data
    private readonly byte[]? _iconData;
    private uint _bestImageOffset;
    private uint _bestBitDepth;
    private uint _bestBytesInRes;
    private bool? _isBestImagePng;
    private Size _iconSize = Size.Empty;
    private IntPtr _handle = IntPtr.Zero;
    private readonly bool _ownHandle = true;

    private Icon() { }

    internal Icon(IntPtr handle) : this(handle, false)
    {
    }

    internal Icon(IntPtr handle, bool takeOwnership)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException(SR.Format(SR.InvalidGDIHandle, nameof(Icon)));
        }

        _handle = handle;
        _ownHandle = takeOwnership;
    }

    public Icon(string fileName) : this(fileName, 0, 0)
    {
    }

    public Icon(string fileName, Size size) : this(fileName, size.Width, size.Height)
    {
    }

    public Icon(string fileName, int width, int height) : this()
    {
        using (FileStream f = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            Debug.Assert(f is not null, "File.OpenRead returned null instead of throwing an exception");
            _iconData = new byte[(int)f.Length];
            f.Read(_iconData, 0, _iconData.Length);
        }

        Initialize(width, height);
    }

    public Icon(Icon original, Size size) : this(original, size.Width, size.Height)
    {
    }

    public Icon(Icon original, int width, int height) : this()
    {
        ArgumentNullException.ThrowIfNull(original);

        _iconData = original._iconData;

        if (_iconData is null)
        {
            _iconSize = original.Size;
            _handle = User32.CopyImage(new HandleRef(original, original.Handle), SafeNativeMethods.IMAGE_ICON, _iconSize.Width, _iconSize.Height, 0);
        }
        else
        {
            Initialize(width, height);
        }
    }

    public Icon(Type type, string resource) : this()
    {
        ArgumentNullException.ThrowIfNull(resource);

        Stream? stream = type.Module.Assembly.GetManifestResourceStream(type, resource);
        if (stream is null)
        {
            throw new ArgumentException(SR.Format(SR.ResourceNotFound, type, resource));
        }

        _iconData = new byte[(int)stream.Length];
        stream.Read(_iconData, 0, _iconData.Length);
        Initialize(0, 0);
    }

    public Icon(Stream stream) : this(stream, 0, 0)
    {
    }

    public Icon(Stream stream, Size size) : this(stream, size.Width, size.Height)
    {
    }

    public Icon(Stream stream, int width, int height) : this()
    {
        ArgumentNullException.ThrowIfNull(stream);

        _iconData = new byte[(int)stream.Length];
#if NET7_0_OR_GREATER
        stream.ReadExactly(_iconData);
#else
        int totalRead = 0;
        while (totalRead < _iconData.Length)
        {
            int bytesRead = stream.Read(_iconData, totalRead, _iconData.Length - totalRead);
            if (bytesRead <= 0)
            {
                throw new EndOfStreamException();
            }
            totalRead += bytesRead;
        }
#endif
        Initialize(width, height);
    }

    private Icon(SerializationInfo info, StreamingContext context)
    {
        _iconData = (byte[])info.GetValue("IconData", typeof(byte[]))!; // Do not rename (binary serialization)
        _iconSize = (Size)info.GetValue("IconSize", typeof(Size))!; // Do not rename (binary serialization)
        Initialize(_iconSize.Width, _iconSize.Height);
    }

    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
    {
        if (_iconData is not null)
        {
            si.AddValue("IconData", _iconData, typeof(byte[])); // Do not rename (binary serialization)
        }
        else
        {
            MemoryStream stream = new();
            Save(stream);
            si.AddValue("IconData", stream.ToArray(), typeof(byte[])); // Do not rename (binary serialization)
        }

        si.AddValue("IconSize", _iconSize, typeof(Size)); // Do not rename (binary serialization)
    }

    public static Icon? ExtractAssociatedIcon(string filePath) => ExtractAssociatedIcon(filePath, 0);

    private static unsafe Icon? ExtractAssociatedIcon(string filePath, int index)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException(SR.NullOrEmptyPath, nameof(filePath));

        filePath = Path.GetFullPath(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(message: null, fileName: filePath);
        }

        // ExtractAssociatedIcon copies the loaded path into the buffer that it is passed.
        // It isn't clear what the exact semantics are for copying back the path, a quick
        // look at the code it might be hard coded to 128 chars for some cases. Leaving the
        // historical MAX_PATH as a minimum to be safe.

        char[] buffer = ArrayPool<char>.Shared.Rent(Math.Max(NativeMethods.MAX_PATH, filePath.Length));
        filePath.CopyTo(0, buffer, 0, filePath.Length);
        buffer[filePath.Length] = '\0';

        IntPtr hIcon;
        fixed (char* b = buffer)
        {
            hIcon = Shell32.ExtractAssociatedIcon(NativeMethods.NullHandleRef, b, ref index);
        }

        ArrayPool<char>.Shared.Return(buffer);

        if (hIcon != IntPtr.Zero)
        {
            return new Icon(hIcon, true);
        }

        return null;
    }

    [Browsable(false)]
    public IntPtr Handle
    {
        get
        {
            if (_handle == IntPtr.Zero)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            return _handle;
        }
    }

    [Browsable(false)]
    public int Height => Size.Height;

    public unsafe Size Size
    {
        get
        {
            if (_iconSize.IsEmpty)
            {
                User32.ICONINFO info = default;
                User32.GetIconInfo(new HandleRef(this, Handle), ref info);
                Gdi32.BITMAP bitmap = default;

                if (info.hbmColor != IntPtr.Zero)
                {
                    Gdi32.GetObject(
                        new HandleRef(null, info.hbmColor),
                        sizeof(Gdi32.BITMAP),
                        ref bitmap);
                    Gdi32.DeleteObject(info.hbmColor);
                    _iconSize = new Size((int)bitmap.bmWidth, (int)bitmap.bmHeight);
                }
                else if (info.hbmMask != IntPtr.Zero)
                {
                    Gdi32.GetObject(
                        new HandleRef(null, info.hbmMask),
                        sizeof(Gdi32.BITMAP),
                        ref bitmap);
                    _iconSize = new Size((int)bitmap.bmWidth, (int)(bitmap.bmHeight / 2));
                }

                if (info.hbmMask != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(info.hbmMask);
                }
            }

            return _iconSize;
        }
    }

    [Browsable(false)]
    public int Width => Size.Width;

    public object Clone() => new Icon(this, Size.Width, Size.Height);

    // Called when this object is going to destroy it's Win32 handle.  You
    // may override this if there is something special you need to do to
    // destroy the handle.  This will be called even if the handle is not
    // owned by this object, which is handy if you want to create a
    // derived class that has it's own create/destroy semantics.
    //
    // The default implementation will call the appropriate Win32
    // call to destroy the handle if this object currently owns the
    // handle.  It will do nothing if the object does not currently
    // own the handle.
    internal void DestroyHandle()
    {
        if (_ownHandle)
        {
            User32.DestroyIcon(new HandleRef(this, _handle));
            _handle = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_handle != IntPtr.Zero)
        {
#if FINALIZATION_WATCH
            Debug.WriteLineIf(!disposing, $"""
                **********************
                Disposed through finalization:
                {allocationSite}
                """);
#endif
            DestroyHandle();
        }
    }

    // Draws this image to a graphics object.  The drawing command originates on the graphics
    // object, but a graphics object generally has no idea how to render a given image.  So,
    // it passes the call to the actual image.  This version crops the image to the given
    // dimensions and allows the user to specify a rectangle within the image to draw.
    private void DrawIcon(IntPtr dc, Rectangle imageRect, Rectangle targetRect, bool stretch)
    {
        int imageX = 0;
        int imageY = 0;
        int imageWidth;
        int imageHeight;
        int targetX = 0;
        int targetY = 0;
        int targetWidth;
        int targetHeight;

        Size cursorSize = Size;

        // Compute the dimensions of the icon if needed.
        if (!imageRect.IsEmpty)
        {
            imageX = imageRect.X;
            imageY = imageRect.Y;
            imageWidth = imageRect.Width;
            imageHeight = imageRect.Height;
        }
        else
        {
            imageWidth = cursorSize.Width;
            imageHeight = cursorSize.Height;
        }

        if (!targetRect.IsEmpty)
        {
            targetX = targetRect.X;
            targetY = targetRect.Y;
            targetWidth = targetRect.Width;
            targetHeight = targetRect.Height;
        }
        else
        {
            targetWidth = cursorSize.Width;
            targetHeight = cursorSize.Height;
        }

        int drawWidth, drawHeight;
        int clipWidth, clipHeight;

        if (stretch)
        {
            drawWidth = cursorSize.Width * targetWidth / imageWidth;
            drawHeight = cursorSize.Height * targetHeight / imageHeight;
            clipWidth = targetWidth;
            clipHeight = targetHeight;
        }
        else
        {
            drawWidth = cursorSize.Width;
            drawHeight = cursorSize.Height;
            clipWidth = targetWidth < imageWidth ? targetWidth : imageWidth;
            clipHeight = targetHeight < imageHeight ? targetHeight : imageHeight;
        }

        // The ROP is SRCCOPY, so we can be simple here and take
        // advantage of clipping regions.  Drawing the cursor
        // is merely a matter of offsetting and clipping.
        IntPtr hSaveRgn = SaveClipRgn(dc);
        try
        {
            Gdi32.IntersectClipRect(new HandleRef(this, dc), targetX, targetY, targetX + clipWidth, targetY + clipHeight);
            User32.DrawIconEx(new HandleRef(null, dc),
                                        targetX - imageX,
                                        targetY - imageY,
                                        new HandleRef(this, _handle),
                                        drawWidth,
                                        drawHeight,
                                        0,
                                        NativeMethods.NullHandleRef,
                                        SafeNativeMethods.DI_NORMAL);
        }
        finally
        {
            Gdi32.SelectClipRgn(dc, hSaveRgn);
            // We need to delete the region handle after restoring the region as GDI+ uses a copy of the handle.
            Gdi32.DeleteObject(hSaveRgn);
        }
    }

    private static IntPtr SaveClipRgn(IntPtr hDC)
    {
        IntPtr hTempRgn = Gdi32.CreateRectRgn(0, 0, 0, 0);
        IntPtr hSaveRgn = IntPtr.Zero;

        int result = Gdi32.GetClipRgn(hDC, hTempRgn);
        if (result > 0)
        {
            hSaveRgn = hTempRgn;
        }
        else
        {
            // if we fail to get the clip region delete the handle.
            Gdi32.DeleteObject(hTempRgn);
        }

        return hSaveRgn;
    }

    internal void Draw(Graphics graphics, int x, int y)
    {
        Size size = Size;
        Draw(graphics, new Rectangle(x, y, size.Width, size.Height));
    }

    // Draws this image to a graphics object.  The drawing command originates on the graphics
    // object, but a graphics object generally has no idea how to render a given image.  So,
    // it passes the call to the actual image.  This version stretches the image to the given
    // dimensions and allows the user to specify a rectangle within the image to draw.
    internal void Draw(Graphics graphics, Rectangle targetRect)
    {
        Rectangle copy = targetRect;

        using Matrix transform = graphics.Transform;
        PointF offset = transform.Offset;
        copy.X += (int)offset.X;
        copy.Y += (int)offset.Y;

        using var wg = WindowsGraphics.FromGraphics(graphics, ApplyGraphicsProperties.Clipping);

        IntPtr dc = wg.GetHdc();
        DrawIcon(dc, Rectangle.Empty, copy, true);
    }

    // Draws this image to a graphics object.  The drawing command originates on the graphics
    // object, but a graphics object generally has no idea how to render a given image.  So,
    // it passes the call to the actual image.  This version crops the image to the given
    // dimensions and allows the user to specify a rectangle within the image to draw.
    internal void DrawUnstretched(Graphics graphics, Rectangle targetRect)
    {
        Rectangle copy = targetRect;
        using Matrix transform = graphics.Transform;
        PointF offset = transform.Offset;
        copy.X += (int)offset.X;
        copy.Y += (int)offset.Y;

        using var wg = WindowsGraphics.FromGraphics(graphics, ApplyGraphicsProperties.Clipping);

        IntPtr dc = wg.GetHdc();
        DrawIcon(dc, Rectangle.Empty, copy, false);
    }

    ~Icon() => Dispose(false);

    public static Icon FromHandle(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
            throw new ArgumentException(null, nameof(handle));

        return new Icon(handle);
    }

    // Initializes this Image object.  This is identical to calling the image's
    // constructor with picture, but this allows non-constructor initialization,
    // which may be necessary in some instances.
    private unsafe void Initialize(int width, int height)
    {
        if (_iconData is null || _handle != IntPtr.Zero)
        {
            throw new InvalidOperationException(SR.Format(SR.IllegalState, GetType().Name));
        }

        if (_iconData.Length < sizeof(SafeNativeMethods.ICONDIR))
        {
            throw new ArgumentException(SR.Format(SR.InvalidPictureType, "picture", nameof(Icon)));
        }

        // Get the correct width and height.
        if (width == 0)
        {
            width = User32.GetSystemMetrics(SafeNativeMethods.SM_CXICON);
        }

        if (height == 0)
        {
            height = User32.GetSystemMetrics(SafeNativeMethods.SM_CYICON);
        }

        if (s_bitDepth == 0)
        {
            IntPtr dc = User32.GetDC(IntPtr.Zero);
            s_bitDepth = Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.BITSPIXEL);
            s_bitDepth *= Gdi32.GetDeviceCaps(dc, Gdi32.DeviceCapability.PLANES);
            User32.ReleaseDC(IntPtr.Zero, dc);

            // If the bitdepth is 8, make it 4 because windows does not
            // choose a 256 color icon if the display is running in 256 color mode
            // due to palette flicker.
            if (s_bitDepth == 8)
            {
                s_bitDepth = 4;
            }
        }

        fixed (byte* b = _iconData)
        {
            SafeNativeMethods.ICONDIR* dir = (SafeNativeMethods.ICONDIR*)b;

            if (dir->idReserved != 0 || dir->idType != 1 || dir->idCount == 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidPictureType, "picture", nameof(Icon)));
            }

            byte bestWidth = 0;
            byte bestHeight = 0;

            if (sizeof(SafeNativeMethods.ICONDIRENTRY) * (dir->idCount - 1) + sizeof(SafeNativeMethods.ICONDIR)
                > _iconData.Length)
            {
                throw new ArgumentException(SR.Format(SR.InvalidPictureType, "picture", nameof(Icon)));
            }

            var entries = new ReadOnlySpan<SafeNativeMethods.ICONDIRENTRY>(&dir->idEntries, dir->idCount);
            foreach (SafeNativeMethods.ICONDIRENTRY entry in entries)
            {
                bool fUpdateBestFit = false;
                uint iconBitDepth;
                if (entry.bColorCount != 0)
                {
                    iconBitDepth = 4;
                    if (entry.bColorCount < 0x10)
                    {
                        iconBitDepth = 1;
                    }
                }
                else
                {
                    iconBitDepth = entry.wBitCount;
                }

                // If it looks like if nothing is specified at this point then set the bits per pixel to 8.
                if (iconBitDepth == 0)
                {
                    iconBitDepth = 8;
                }

                // Windows rules for specifying an icon:
                //
                //  1.  The icon with the closest size match.
                //  2.  For matching sizes, the image with the closest bit depth.
                //  3.  If there is no color depth match, the icon with the closest color depth that does not exceed the display.
                //  4.  If all icon color depth > display, lowest color depth is chosen.
                //  5.  color depth of > 8bpp are all equal.
                //  6.  Never choose an 8bpp icon on an 8bpp system.

                if (_bestBytesInRes == 0)
                {
                    fUpdateBestFit = true;
                }
                else
                {
                    int bestDelta = Math.Abs(bestWidth - width) + Math.Abs(bestHeight - height);
                    int thisDelta = Math.Abs(entry.bWidth - width) + Math.Abs(entry.bHeight - height);

                    if ((thisDelta < bestDelta) ||
                        (thisDelta == bestDelta && ((iconBitDepth <= s_bitDepth && iconBitDepth > _bestBitDepth) || (_bestBitDepth > s_bitDepth && iconBitDepth < _bestBitDepth))))
                    {
                        fUpdateBestFit = true;
                    }
                }

                if (fUpdateBestFit)
                {
                    bestWidth = entry.bWidth;
                    bestHeight = entry.bHeight;
                    _bestImageOffset = entry.dwImageOffset;
                    _bestBytesInRes = entry.dwBytesInRes;
                    _bestBitDepth = iconBitDepth;
                }
            }

            if (_bestImageOffset > int.MaxValue)
            {
                throw new ArgumentException(SR.Format(SR.InvalidPictureType, "picture", nameof(Icon)));
            }

            if (_bestBytesInRes > int.MaxValue)
            {
                throw new Win32Exception(SafeNativeMethods.ERROR_INVALID_PARAMETER);
            }

            uint endOffset;
            try
            {
                endOffset = checked(_bestImageOffset + _bestBytesInRes);
            }
            catch (OverflowException)
            {
                throw new Win32Exception(SafeNativeMethods.ERROR_INVALID_PARAMETER);
            }

            if (endOffset > _iconData.Length)
            {
                throw new ArgumentException(SR.Format(SR.InvalidPictureType, "picture", nameof(Icon)));
            }

            // Copy the bytes into an aligned buffer if needed.
            if ((_bestImageOffset % IntPtr.Size) != 0)
            {
                // Beginning of icon's content is misaligned.
                byte[] alignedBuffer = ArrayPool<byte>.Shared.Rent((int)_bestBytesInRes);
                Array.Copy(_iconData, _bestImageOffset, alignedBuffer, 0, _bestBytesInRes);

                fixed (byte* pbAlignedBuffer = alignedBuffer)
                {
                    _handle = User32.CreateIconFromResourceEx(pbAlignedBuffer, _bestBytesInRes, true, 0x00030000, 0, 0, 0);
                }

                ArrayPool<byte>.Shared.Return(alignedBuffer);
            }
            else
            {
                try
                {
                    _handle = User32.CreateIconFromResourceEx(checked(b + _bestImageOffset), _bestBytesInRes, true, 0x00030000, 0, 0, 0);
                }
                catch (OverflowException)
                {
                    throw new Win32Exception(SafeNativeMethods.ERROR_INVALID_PARAMETER);
                }
            }

            if (_handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }
    }

    private unsafe void CopyBitmapData(BitmapData sourceData, BitmapData targetData)
    {
        byte* srcPtr = (byte*)sourceData.Scan0;
        byte* destPtr = (byte*)targetData.Scan0;

        Debug.Assert(sourceData.Height == targetData.Height, "Unexpected height. How did this happen?");
        int height = Math.Min(sourceData.Height, targetData.Height);
        long bytesToCopyEachIter = Math.Abs(targetData.Stride);

        for (int i = 0; i < height; i++)
        {
            Buffer.MemoryCopy(srcPtr, destPtr, bytesToCopyEachIter, bytesToCopyEachIter);
            srcPtr += sourceData.Stride;
            destPtr += targetData.Stride;
        }

        GC.KeepAlive(this); // finalizer mustn't deallocate data blobs while this method is running
    }

    private static bool BitmapHasAlpha(BitmapData bmpData)
    {
        bool hasAlpha = false;
        for (int i = 0; i < bmpData.Height; i++)
        {
            for (int j = 3; j < Math.Abs(bmpData.Stride); j += 4)
            {
                // Stride here is fine since we know we're doing this on the whole image.
                unsafe
                {
                    byte* candidate = unchecked(((byte*)bmpData.Scan0.ToPointer()) + (i * bmpData.Stride) + j);
                    if (*candidate != 0)
                    {
                        hasAlpha = true;
                        return hasAlpha;
                    }
                }
            }
        }

        return false;
    }

    public Bitmap ToBitmap()
    {
        // DontSupportPngFramesInIcons is true when the application is targeting framework version below 4.6
        // and false when the application is targeting 4.6 and above. Downlevel application can also set the following switch
        // to false in the .config file's runtime section in order to opt-in into the new behavior:
        // <AppContextSwitchOverrides value="Switch.System.Drawing.DontSupportPngFramesInIcons=false" />
        if (HasPngSignature() && !LocalAppContextSwitches.DontSupportPngFramesInIcons)
        {
            return PngFrame();
        }

        return BmpFrame();
    }

    private unsafe Bitmap BmpFrame()
    {
        Bitmap? bitmap = null;
        if (_iconData is not null && _bestBitDepth == 32)
        {
            // GDI+ doesnt handle 32 bpp icons with alpha properly
            // we load the icon ourself from the byte table
            bitmap = new Bitmap(Size.Width, Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Debug.Assert(_bestImageOffset >= 0 && (_bestImageOffset + _bestBytesInRes) <= _iconData.Length, "Illegal offset/length for the Icon data");

            unsafe
            {
                BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, Size.Width, Size.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);
                try
                {
                    uint* pixelPtr = (uint*)bmpdata.Scan0.ToPointer();

                    // jumping the image header
                    int newOffset = (int)(_bestImageOffset + sizeof(NativeMethods.BITMAPINFOHEADER));
                    // there is no color table that we need to skip since we're 32bpp

                    int lineLength = Size.Width * 4;
                    int width = Size.Width;
                    for (int j = (Size.Height - 1) * 4; j >= 0; j -= 4)
                    {
                        Marshal.Copy(_iconData, newOffset + j * width, (IntPtr)pixelPtr, lineLength);
                        pixelPtr += width;
                    }

                    // note: we ignore the mask that's available after the pixel table
                }
                finally
                {
                    bitmap.UnlockBits(bmpdata);
                }
            }
        }
        else if (_bestBitDepth == 0 || _bestBitDepth == 32)
        {
            // This may be a 32bpp icon or an icon without any data.
            User32.ICONINFO info = default;
            User32.GetIconInfo(new HandleRef(this, _handle), ref info);
            Gdi32.BITMAP bmp = default;
            try
            {
                if (info.hbmColor != IntPtr.Zero)
                {
                    Gdi32.GetObject(new HandleRef(null, info.hbmColor), sizeof(Gdi32.BITMAP), ref bmp);
                    if (bmp.bmBitsPixel == 32)
                    {
                        Bitmap? tmpBitmap = null;
                        BitmapData? bmpData = null;
                        BitmapData? targetData = null;
                        try
                        {
                            tmpBitmap = Image.FromHbitmap(info.hbmColor);

                            // In GDI+ the bits are there but the bitmap was created with no alpha channel
                            // so copy the bits by hand to a new bitmap
                            // we also need to go around a limitation in the way the ICON is stored (ie if it's another bpp
                            // but stored in 32bpp all pixels are transparent and not opaque)
                            // (Here you mostly need to remain calm....)
                            bmpData = tmpBitmap.LockBits(new Rectangle(0, 0, tmpBitmap.Width, tmpBitmap.Height), ImageLockMode.ReadOnly, tmpBitmap.PixelFormat);

                            // we need do the following if the image has alpha because otherwise the image is fully transparent even though it has data
                            if (BitmapHasAlpha(bmpData))
                            {
                                bitmap = new Bitmap(bmpData.Width, bmpData.Height, PixelFormat.Format32bppArgb);
                                targetData = bitmap.LockBits(new Rectangle(0, 0, bmpData.Width, bmpData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                                CopyBitmapData(bmpData, targetData);
                            }
                        }
                        finally
                        {
                            if (tmpBitmap is not null && bmpData is not null)
                            {
                                tmpBitmap.UnlockBits(bmpData);
                            }

                            if (bitmap is not null && targetData is not null)
                            {
                                bitmap.UnlockBits(targetData);
                            }
                        }

                        tmpBitmap.Dispose();
                    }
                }
            }
            finally
            {
                if (info.hbmColor != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(info.hbmColor);
                }

                if (info.hbmMask != IntPtr.Zero)
                {
                    Gdi32.DeleteObject(info.hbmMask);
                }
            }
        }

        if (bitmap is null)
        {
            // last chance... all the other cases (ie non 32 bpp icons coming from a handle or from the bitmapData)

            // we have to do this rather than just return Bitmap.FromHIcon because
            // the bitmap returned from that, even though it's 32bpp, just paints where the mask allows it
            // seems like another GDI+ weirdness. might be interesting to investigate further. In the meantime
            // this looks like the right thing to do and is not more expansive that what was present before.

            Size size = Size;
            bitmap = new Bitmap(size.Width, size.Height); // initialized to transparent
            Graphics? graphics;
            using (graphics = Graphics.FromImage(bitmap))
            {
                try
                {
                    using var tmpBitmap = Bitmap.FromHicon(Handle);
                    graphics.DrawImage(tmpBitmap, new Rectangle(0, 0, size.Width, size.Height));
                }
                catch (ArgumentException)
                {
                    // Sometimes FromHicon will crash with no real reason.
                    // The backup plan is to just draw the image like we used to.
                    // NOTE: FromHIcon is also where we have the buffer overrun
                    // if width and height are mismatched.
                    Draw(graphics, new Rectangle(0, 0, size.Width, size.Height));
                }
            }

            // GDI+ fills the surface with a sentinel color for GetDC, but does
            // not correctly clean it up again, so we have to do it.
            Color fakeTransparencyColor = Color.FromArgb(0x0d, 0x0b, 0x0c);
            bitmap.MakeTransparent(fakeTransparencyColor);
        }

        Debug.Assert(bitmap is not null, "Bitmap cannot be null");
        return bitmap;
    }

    private Bitmap PngFrame()
    {
        Debug.Assert(_iconData is not null);
        using MemoryStream stream = new();
        stream.Write(_iconData, (int)_bestImageOffset, (int)_bestBytesInRes);
        return new Bitmap(stream);
    }

    private bool HasPngSignature()
    {
        if (!_isBestImagePng.HasValue)
        {
            if (_iconData is not null && _iconData.Length >= _bestImageOffset + 8)
            {
                int iconSignature1 = BitConverter.ToInt32(_iconData, (int)_bestImageOffset);
                int iconSignature2 = BitConverter.ToInt32(_iconData, (int)_bestImageOffset + 4);
                _isBestImagePng = (iconSignature1 == PNGSignature1) && (iconSignature2 == PNGSignature2);
            }
            else
            {
                _isBestImagePng = false;
            }
        }

        return _isBestImagePng.Value;
    }

    public override string ToString() => SR.toStringIcon;

    internal static class Ole
    {
        public const int PICTYPE_ICON = 3;
    }

#if NET8_0_OR_GREATER
    /// <summary>
    ///  Extracts a specified icon from the given <paramref name="filePath"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Unlike the <see cref="Icon(string)">constructors that take a path</see> this method and the
    ///   <see cref="ExtractAssociatedIcon(string)"/> methods do not retain all of the resource data or modify the
    ///   original data (outside of resizing if necessary). As such, the <see cref="Icon"/> only uses as much
    ///   memory as is needed for the requested size (mostly native memory).
    ///  </para>
    ///  <para>
    ///   Without the original source data the <see cref="Icon(Icon, Size)">copy constructors</see> have to resample
    ///   the current icon's bitmap to change sizes. For best image quality, if different sizes for an <see cref="Icon"/>
    ///   are desired you should create separate instances with this method and avoid the copy constructors.
    ///  </para>
    /// </remarks>
    /// <param name="filePath">Path to an icon or PE (.dll, .exe) file.</param>
    /// <param name="id">
    ///  Positive numbers refer to an icon index in the given file. Negative numbers refer to a specific native resource
    ///  identifier in a PE (.dll, .exe) file.
    /// </param>
    /// <param name="size">
    ///  The desired size. If the specified size does not exist, an existing size will be resampled to give the
    ///  requested size.
    /// </param>
    /// <returns>
    ///  An <see cref="Icon"/>, or <see langword="null"/> if an icon can't be found with the specified
    ///  <paramref name="id"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  <paramref name="size"/> is negative or larger than <see cref="ushort.MaxValue"/>.
    /// </exception>
    /// <exception cref="IOException">
    ///  There given <paramref name="filePath"/> could not be accessed.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="filePath"/> is null.
    /// </exception>
    public static unsafe Icon? ExtractIcon(string filePath, int id, int size)
        => size is <= 0 or > ushort.MaxValue
            ? throw new ArgumentOutOfRangeException(nameof(size))
            : ExtractIcon(filePath, id, size, smallIcon: false);

    /// <param name="smallIcon">
    ///  If <see langword="true"/>, gets the <see cref="Icon"/> at the current system small icon size setting. If
    ///  <see langword="false"/>, gets the <see cref="Icon"/> at the current system large icon size setting. Default is
    ///  <see langword="false"/>.
    /// </param>
    /// <inheritdoc cref="ExtractIcon(string, int, int)" />
    public static unsafe Icon? ExtractIcon(string filePath, int id, bool smallIcon = false)
        => ExtractIcon(filePath, id, 0, smallIcon);

    private static unsafe Icon? ExtractIcon(string filePath, int id, int size, bool smallIcon = false)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        Debug.Assert(size is >= 0 and <= ushort.MaxValue);

        nint hicon = 0;
        HRESULT result;
        fixed (char* c = filePath)
        {
            result = Shell32.SHDefExtractIcon(
                c,
                id,
                0,
                smallIcon ? null : &hicon,
                smallIcon ? &hicon : null,
                (uint)(ushort)size << 16 | (ushort)size);
        }

        if (result == HRESULT.S_FALSE)
        {
            // Icon wasn't found
            return null;
        }

        // This only throws if there is an error.
        try
        {
            Marshal.ThrowExceptionForHR((int)result);
        }
        catch (COMException ex)
        {
            // This API is only documented to return E_FAIL, which surfaces as COMException. Wrap in a "nicer"
            // ArgumentException.
            throw new IOException(SR.IconCouldNotBeExtracted, ex);
        }

        return new Icon(hicon, takeOwnership: true);
    }
#endif
}
