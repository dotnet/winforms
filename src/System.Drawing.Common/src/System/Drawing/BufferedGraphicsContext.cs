// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Drawing;

/// <summary>
///  The BufferedGraphicsContext class can be used to perform standard double buffer rendering techniques.
/// </summary>
public sealed unsafe class BufferedGraphicsContext : IDisposable
{
    private Size _maximumBuffer;
    private Size _bufferSize = Size.Empty;
    private Size _virtualSize;
    private Point _targetLoc;
    private HDC _compatDC;
    private HBITMAP _dib;
    private HBITMAP _oldBitmap;
    private Graphics? _compatGraphics;
    private BufferedGraphics? _buffer;
    private int _busy;
    private bool _invalidateWhenFree;

    private const int BufferFree = 0; // The graphics buffer is free to use.
    private const int BufferBusyPainting = 1; // The graphics buffer is busy being created/painting.
    private const int BufferBusyDisposing = 2; // The graphics buffer is busy disposing.

    /// <summary>
    /// Basic constructor.
    /// </summary>
    public BufferedGraphicsContext()
    {
        // By default, the size of our max buffer will be 3 x standard button size.
        _maximumBuffer.Width = 75 * 3;
        _maximumBuffer.Height = 32 * 3;
    }

    /// <summary>
    ///  Allows you to set the maximum width and height of the buffer that will be retained in memory.
    ///  You can allocate a buffer of any size, however any request for a buffer that would have a total
    ///  memory footprint larger that the maximum size will be allocated temporarily and then discarded
    ///  with the BufferedGraphics is released.
    /// </summary>
    public Size MaximumBuffer
    {
        get => _maximumBuffer;
        set
        {
            if (value.Width <= 0 || value.Height <= 0)
            {
                throw new ArgumentException(SR.Format(SR.InvalidArgumentValue, nameof(MaximumBuffer), value), nameof(value));
            }

            // If we've been asked to decrease the size of the maximum buffer,
            // then invalidate the older & larger buffer.
            if (value.Width * value.Height < _maximumBuffer.Width * _maximumBuffer.Height)
            {
                Invalidate();
            }

            _maximumBuffer = value;
        }
    }

    ~BufferedGraphicsContext() => Dispose(false);

    /// <summary>
    /// Returns a BufferedGraphics that is matched for the specified target Graphics object.
    /// </summary>
    public BufferedGraphics Allocate(Graphics targetGraphics, Rectangle targetRectangle)
    {
        if (ShouldUseTempManager(targetRectangle))
        {
            return AllocBufferInTempManager(targetGraphics, HDC.Null, targetRectangle);
        }

        return AllocBuffer(targetGraphics, HDC.Null, targetRectangle);
    }

    /// <summary>
    /// Returns a BufferedGraphics that is matched for the specified target HDC object.
    /// </summary>
    public BufferedGraphics Allocate(IntPtr targetDC, Rectangle targetRectangle)
    {
        if (ShouldUseTempManager(targetRectangle))
        {
            return AllocBufferInTempManager(null, (HDC)targetDC, targetRectangle);
        }

        return AllocBuffer(null, (HDC)targetDC, targetRectangle);
    }

    /// <summary>
    /// Returns a BufferedGraphics that is matched for the specified target HDC object.
    /// </summary>
    private BufferedGraphics AllocBuffer(Graphics? targetGraphics, HDC targetDC, Rectangle targetRectangle)
    {
        int oldBusy = Interlocked.CompareExchange(ref _busy, BufferBusyPainting, BufferFree);

        // In the case were we have contention on the buffer - i.e. two threads
        // trying to use the buffer at the same time, we just create a temp
        // buffer manager and have the buffer dispose of it when it is done.
        if (oldBusy != BufferFree)
        {
            return AllocBufferInTempManager(targetGraphics, targetDC, targetRectangle);
        }

        Graphics surface;
        _targetLoc = new Point(targetRectangle.X, targetRectangle.Y);

        try
        {
            if (targetGraphics is not null)
            {
                IntPtr destDc = targetGraphics.GetHdc();
                try
                {
                    surface = CreateBuffer((HDC)destDc, targetRectangle.Width, targetRectangle.Height);
                }
                finally
                {
                    targetGraphics.ReleaseHdcInternal(destDc);
                }
            }
            else
            {
                surface = CreateBuffer(targetDC, targetRectangle.Width, targetRectangle.Height);
            }

            _buffer = new BufferedGraphics(surface, this, targetGraphics, targetDC, _targetLoc, _virtualSize);
        }
        catch
        {
            // Free the buffer so it can be disposed.
            _busy = BufferFree;
            throw;
        }

        return _buffer;
    }

    /// <summary>
    /// Returns a BufferedGraphics that is matched for the specified target HDC object.
    /// </summary>
    private static BufferedGraphics AllocBufferInTempManager(Graphics? targetGraphics, HDC targetDC, Rectangle targetRectangle)
    {
        BufferedGraphicsContext? tempContext = null;
        BufferedGraphics? tempBuffer = null;

        try
        {
            tempContext = new BufferedGraphicsContext();
            tempBuffer = tempContext.AllocBuffer(targetGraphics, targetDC, targetRectangle);
            tempBuffer.DisposeContext = true;
        }
        finally
        {
            if (tempContext is not null && tempBuffer is not { DisposeContext: true })
            {
                tempContext.Dispose();
            }
        }

        return tempBuffer;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// This routine allows us to control the point were we start using throw away
    /// managers for painting. Since the buffer manager stays around (by default)
    /// for the life of the app, we don't want to consume too much memory
    /// in the buffer. However, re-allocating the buffer for small things (like
    /// buttons, labels, etc) will hit us on runtime performance.
    /// </summary>
    private bool ShouldUseTempManager(Rectangle targetBounds)
    {
        return (targetBounds.Width * targetBounds.Height) > (MaximumBuffer.Width * MaximumBuffer.Height);
    }

    /// <summary>
    ///  Fills in the fields of a BITMAPINFO so that we can create a bitmap
    ///  that matches the format of the display.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is done by creating a compatible bitmap and calling GetDIBits
    ///   to return the color masks. This is done with two calls. The first
    ///   call passes in biBitCount = 0 to GetDIBits which will fill in the
    ///   base BITMAPINFOHEADER data. The second call to GetDIBits (passing
    ///   in the BITMAPINFO filled in by the first call) will return the color
    ///   table or bitmasks, as appropriate.
    ///  </para>
    /// </remarks>
    /// <returns>True if successful, false otherwise.</returns>
    private bool FillBitmapInfo(HDC hdc, HPALETTE hpalette, BITMAPINFO* bitmapInfo)
    {
        // Create a dummy bitmap from which we can query color format info about the device surface.
        using CreateBitmapScope hbm = new(hdc, 1, 1);

        if (hbm.IsNull)
        {
#pragma warning disable CA2201 // Do not raise reserved exception types - for compat we'll leave this as is, should probably be InvalidOperationException
            throw new OutOfMemoryException(SR.GraphicsBufferQueryFail);
#pragma warning restore CA2201
        }

        bitmapInfo->bmiHeader.biSize = (uint)sizeof(BITMAPINFOHEADER);

        // Call first time to fill in BITMAPINFO header.
        PInvoke.GetDIBits(
            hdc,
            hbm,
            0,
            0,
            null,
            bitmapInfo,
            DIB_USAGE.DIB_RGB_COLORS);

        if (bitmapInfo->bmiHeader.biBitCount <= 8)
        {
            return FillColorTable(hdc, hpalette, bitmapInfo);
        }
        else
        {
            if (bitmapInfo->bmiHeader.biCompression == (uint)BI_COMPRESSION.BI_BITFIELDS)
            {
                // Call a second time to get the color masks.
                PInvoke.GetDIBits(
                    hdc,
                    hbm,
                    0,
                    (uint)bitmapInfo->bmiHeader.biHeight,
                    null,
                    bitmapInfo,
                    DIB_USAGE.DIB_RGB_COLORS);
            }
        }

        return true;
    }

    /// <summary>
    ///  Initialize the color table of the <see cref="BITMAPINFO"/>. Colors
    ///  are set to the current system palette.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Note: call only valid for displays of 8bpp or less.
    ///  </para>
    /// </remarks>
    /// <returns>True is successful, false otherwise.</returns>
    private bool FillColorTable(HDC hdc, HPALETTE hpalette, BITMAPINFO* bitmapInfo)
    {
        int colorCount = 1 << bitmapInfo->bmiHeader.biBitCount;
        if (colorCount > 256)
        {
            return false;
        }

        PALETTEENTRY* paletteEntries = stackalloc PALETTEENTRY[colorCount];
        Span<RGBQUAD> rgbQuads = new((RGBQUAD*)&bitmapInfo->bmiColors, colorCount);

        // Note: we don't support 4bpp displays.
        uint entries;

        if (hpalette.IsNull)
        {
            HPALETTE halftonePalette = (HPALETTE)Graphics.GetHalftonePalette();
            entries = PInvokeCore.GetPaletteEntries(halftonePalette, 0, (uint)colorCount, paletteEntries);
        }
        else
        {
            entries = PInvokeCore.GetPaletteEntries(hpalette, 0, (uint)colorCount, paletteEntries);
        }

        if (entries == 0)
        {
            return false;
        }

        for (int i = 0; i < colorCount; i++)
        {
            // Unfortunately, the structs' fields are not in the same order.
            rgbQuads[i].rgbRed = paletteEntries[i].peRed;
            rgbQuads[i].rgbGreen = paletteEntries[i].peGreen;
            rgbQuads[i].rgbBlue = paletteEntries[i].peBlue;
            rgbQuads[i].rgbReserved = 0;
        }

        return true;
    }

    /// <summary>
    ///  Returns a Graphics object representing a buffer.
    /// </summary>
    private Graphics CreateBuffer(HDC src, int width, int height)
    {
        // Create the compat DC.
        _busy = BufferBusyDisposing;
        DisposeDC();
        _busy = BufferBusyPainting;
        _compatDC = PInvokeCore.CreateCompatibleDC(src);

        // Recreate the bitmap if necessary.
        if (width > _bufferSize.Width || height > _bufferSize.Height)
        {
            int optWidth = Math.Max(width, _bufferSize.Width);
            int optHeight = Math.Max(height, _bufferSize.Height);

            _busy = BufferBusyDisposing;
            DisposeBitmap();
            _busy = BufferBusyPainting;

            _dib = CreateCompatibleDIB(src, HPALETTE.Null, optWidth, optHeight);
            _bufferSize = new Size(optWidth, optHeight);
        }

        // Select the bitmap.
        _oldBitmap = (HBITMAP)PInvokeCore.SelectObject(_compatDC, _dib);

        // Create compat graphics.
        _compatGraphics = Graphics.FromHdcInternal(_compatDC);
        _compatGraphics.TranslateTransform(-_targetLoc.X, -_targetLoc.Y);
        _virtualSize = new Size(width, height);

        GC.KeepAlive(this);
        return _compatGraphics;
    }

    /// <summary>
    /// Create a DIB section with an optimal format w.r.t. the specified hdc.
    ///
    /// If DIB &lt;= 8bpp, then the DIB color table is initialized based on the
    /// specified palette. If the palette handle is NULL, then the system
    /// palette is used.
    ///
    /// Note: The hdc must be a direct DC (not an info or memory DC).
    ///
    /// Note: On palettized displays, if the system palette changes the
    ///       UpdateDIBColorTable function should be called to maintain
    ///       the identity palette mapping between the DIB and the display.
    /// </summary>
    /// <returns>A valid bitmap handle if successful, IntPtr.Zero otherwise.</returns>
    private HBITMAP CreateCompatibleDIB(HDC hdc, HPALETTE hpalette, int ulWidth, int ulHeight)
    {
        if (hdc.IsNull)
        {
            throw new ArgumentNullException(nameof(hdc));
        }

        HBITMAP hbitmap = HBITMAP.Null;

        // Reserve enough space for all RGBQUADS
        byte* buffer = stackalloc byte[sizeof(BITMAPINFO) + 256 * sizeof(RGBQUAD)];
        BITMAPINFO* bitmapInfo = (BITMAPINFO*)buffer;

        // Validate hdc.
        OBJ_TYPE objType = (OBJ_TYPE)PInvokeCore.GetObjectType(hdc);
        switch (objType)
        {
            case OBJ_TYPE.OBJ_DC:
            case OBJ_TYPE.OBJ_METADC:
            case OBJ_TYPE.OBJ_MEMDC:
            case OBJ_TYPE.OBJ_ENHMETADC:
                break;
            default:
                throw new ArgumentException(SR.DCTypeInvalid);
        }

        if (FillBitmapInfo(hdc, hpalette, bitmapInfo))
        {
            // Change bitmap size to match specified dimensions.
            bitmapInfo->bmiHeader.biWidth = ulWidth;
            bitmapInfo->bmiHeader.biHeight = ulHeight;
            if (bitmapInfo->bmiHeader.biCompression == (uint)BI_COMPRESSION.BI_RGB)
            {
                bitmapInfo->bmiHeader.biSizeImage = 0;
            }
            else
            {
                if (bitmapInfo->bmiHeader.biBitCount == 16)
                {
                    bitmapInfo->bmiHeader.biSizeImage = (uint)(ulWidth * ulHeight * 2);
                }
                else if (bitmapInfo->bmiHeader.biBitCount == 32)
                {
                    bitmapInfo->bmiHeader.biSizeImage = (uint)(ulWidth * ulHeight * 4);
                }
                else
                {
                    bitmapInfo->bmiHeader.biSizeImage = 0;
                }
            }

            bitmapInfo->bmiHeader.biClrUsed = 0;
            bitmapInfo->bmiHeader.biClrImportant = 0;

            // Create the DIB section. Let Win32 allocate the memory and return a pointer to the bitmap surface.

            void* pvBits = null;
            hbitmap = PInvokeCore.CreateDIBSection(hdc, bitmapInfo, DIB_USAGE.DIB_RGB_COLORS, &pvBits, HANDLE.Null, 0);
            if (hbitmap.IsNull)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        return hbitmap;
    }

    /// <summary>
    ///  Disposes the DC, but leaves the bitmap alone.
    /// </summary>
    private void DisposeDC()
    {
        if (!_oldBitmap.IsNull && !_compatDC.IsNull)
        {
            PInvokeCore.SelectObject(_compatDC, _oldBitmap);
            _oldBitmap = HBITMAP.Null;
        }

        if (!_compatDC.IsNull)
        {
            PInvokeCore.DeleteDC(_compatDC);
            _compatDC = HDC.Null;
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Disposes the bitmap, will ASSERT if bitmap is being used (checks oldbitmap). if ASSERTed, call DisposeDC() first.
    /// </summary>
    private void DisposeBitmap()
    {
        if (!_dib.IsNull)
        {
            Debug.Assert(_oldBitmap.IsNull);

            PInvokeCore.DeleteObject(_dib);
            _dib = HBITMAP.Null;
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    /// Disposes of the Graphics buffer.
    /// </summary>
    private void Dispose(bool disposing)
    {
        int oldBusy = Interlocked.CompareExchange(ref _busy, BufferBusyDisposing, BufferFree);

        if (disposing)
        {
            if (oldBusy == BufferBusyPainting)
            {
                throw new InvalidOperationException(SR.GraphicsBufferCurrentlyBusy);
            }

            if (_compatGraphics is not null)
            {
                _compatGraphics.Dispose();
                _compatGraphics = null;
            }
        }

        DisposeDC();
        DisposeBitmap();

        if (_buffer is not null)
        {
            _buffer.Dispose();
            _buffer = null;
        }

        _bufferSize = Size.Empty;
        _virtualSize = Size.Empty;

        _busy = BufferFree;
    }

    /// <summary>
    /// Invalidates the cached graphics buffer.
    /// </summary>
    public void Invalidate()
    {
        int oldBusy = Interlocked.CompareExchange(ref _busy, BufferBusyDisposing, BufferFree);

        // If we're not busy with our buffer, lets clean it up now
        if (oldBusy == BufferFree)
        {
            Dispose();
            _busy = BufferFree;
        }
        else
        {
            // This will indicate to free the buffer as soon as it becomes non-busy.
            _invalidateWhenFree = true;
        }
    }

    /// <summary>
    /// Returns a Graphics object representing a buffer.
    /// </summary>
    internal void ReleaseBuffer()
    {
        _buffer = null;
        if (_invalidateWhenFree)
        {
            // Clears everything including the bitmap.
            _busy = BufferBusyDisposing;
            Dispose();
        }
        else
        {
            // Otherwise, just dispose the DC. A new one will be created next time.
            _busy = BufferBusyDisposing;

            // Only clears out the DC.
            DisposeDC();
        }

        _busy = BufferFree;
    }
}
