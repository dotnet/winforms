// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  MetafileDCWrapper is used to wrap a metafile DC so that subsequent
    ///  paint operations are rendered to a temporary bitmap. When the
    ///  wrapper is disposed, it copies the bitmap back to the metafile DC.
    ///
    ///  Example:
    ///
    ///  using(MetafileDCWrapper dcWrapper = new MetafileDCWrapper(hDC, size) {
    ///  // ...use dcWrapper.HDC to do painting
    ///  }
    /// </summary>
    private class MetafileDCWrapper : IDisposable
    {
        private HBITMAP _hBitmap;
        private HBITMAP _hOriginalBmp;
        private readonly HDC _hMetafileDC;
        private RECT _destRect;

        internal unsafe MetafileDCWrapper(HDC hOriginalDC, Size size)
        {
            Debug.Assert((OBJ_TYPE)PInvokeCore.GetObjectType(hOriginalDC) == OBJ_TYPE.OBJ_ENHMETADC,
                "Why wrap a non-Enhanced MetaFile DC?");

            if (size.Width < 0 || size.Height < 0)
            {
                throw new ArgumentException(SR.ControlMetaFileDCWrapperSizeInvalid, nameof(size));
            }

            _hMetafileDC = hOriginalDC;
            _destRect = new(size);
            HDC = PInvokeCore.CreateCompatibleDC(default);

            int planes = PInvokeCore.GetDeviceCaps(HDC, GET_DEVICE_CAPS_INDEX.PLANES);
            int bitsPixel = PInvokeCore.GetDeviceCaps(HDC, GET_DEVICE_CAPS_INDEX.BITSPIXEL);
            _hBitmap = PInvokeCore.CreateBitmap(size.Width, size.Height, (uint)planes, (uint)bitsPixel, lpBits: null);
            _hOriginalBmp = (HBITMAP)PInvokeCore.SelectObject(HDC, _hBitmap);
        }

        ~MetafileDCWrapper()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (HDC.IsNull || _hMetafileDC.IsNull || _hBitmap.IsNull)
            {
                return;
            }

            bool success;

            try
            {
                success = DICopy(_hMetafileDC, HDC, _destRect, bStretch: true);
                Debug.Assert(success, "DICopy() failed.");
                PInvokeCore.SelectObject(HDC, _hOriginalBmp);
                success = PInvokeCore.DeleteObject(_hBitmap);
                Debug.Assert(success, "DeleteObject() failed.");
                success = PInvokeCore.DeleteDC(HDC);
                Debug.Assert(success, "DeleteObject() failed.");
            }
            finally
            {
                // Dispose is done. Set all the handles to IntPtr.Zero so this way the Dispose method executes only once.
                HDC = default;
                _hBitmap = default;
                _hOriginalBmp = default;

                GC.SuppressFinalize(this);
            }
        }

        internal HDC HDC { get; private set; }

        // ported form VB6 (Ctls\PortUtil\StdCtl.cpp:6176)
        private unsafe bool DICopy(HDC hdcDest, HDC hdcSrc, RECT rect, bool bStretch)
        {
            long i;

            // Get the bitmap from the DC by selecting in a 1x1 pixel temp bitmap
            HBITMAP hNullBitmap = PInvokeCore.CreateBitmap(1, 1, 1, 1, null);
            if (hNullBitmap.IsNull)
            {
                return false;
            }

            try
            {
                HBITMAP hBitmap = (HBITMAP)PInvokeCore.SelectObject(hdcSrc, hNullBitmap);
                if (hBitmap.IsNull)
                {
                    return false;
                }

                // Restore original bitmap
                PInvokeCore.SelectObject(hdcSrc, hBitmap);

                if (!PInvokeCore.GetObject(hBitmap, out BITMAP bmp))
                {
                    return false;
                }

                long colorEntryCount = 1 << (bmp.bmBitsPixel * bmp.bmPlanes);

                // Allocate memory to hold the bitmap bits
                long bitsPerScanLine = bmp.bmBitsPixel * (long)bmp.bmWidth;
                long bytesPerScanLine = (bitsPerScanLine + 7) / 8;
                long totalBytesReqd = bytesPerScanLine * bmp.bmHeight;
                using BufferScope<byte> imageBuffer = new((int)totalBytesReqd);
                using BufferScope<byte> bitmapInfoBuffer = new
                    ((int)checked((sizeof(BITMAPINFOHEADER) + (sizeof(RGBQUAD) * colorEntryCount))));

                fixed (byte* bi = bitmapInfoBuffer)
                fixed (byte* pib = imageBuffer)
                {
                    *((BITMAPINFOHEADER*)bi) = new BITMAPINFOHEADER
                    {
                        biSize = (uint)sizeof(BITMAPINFOHEADER),
                        biWidth = bmp.bmWidth,
                        biHeight = bmp.bmHeight,
                        biPlanes = 1,
                        biBitCount = bmp.bmBitsPixel,
                        biCompression = (uint)BI_COMPRESSION.BI_RGB
                    };

                    // Include the palette for 256 color bitmaps
                    if (colorEntryCount <= 256)
                    {
                        using BufferScope<byte> aj = new((int)(sizeof(PALETTEENTRY) * colorEntryCount));

                        fixed (byte* ppal = aj)
                        {
                            PInvoke.GetSystemPaletteEntries(hdcSrc, 0, (uint)colorEntryCount, (PALETTEENTRY*)ppal);
                            byte* pcolors = bi + sizeof(BITMAPINFOHEADER);
                            RGBQUAD* prgb = (RGBQUAD*)pcolors;
                            PALETTEENTRY* lppe = (PALETTEENTRY*)ppal;

                            // Convert the palette entries to RGB quad entries
                            for (i = 0; i < (int)colorEntryCount; i++)
                            {
                                prgb[i].rgbRed = lppe[i].peRed;
                                prgb[i].rgbBlue = lppe[i].peBlue;
                                prgb[i].rgbGreen = lppe[i].peGreen;
                            }
                        }
                    }

                    // Get the bitmap bits
                    int diRet = PInvoke.GetDIBits(
                        hdcSrc,
                        hBitmap,
                        start: 0,
                        (uint)bmp.bmHeight,
                        pib,
                        (BITMAPINFO*)bi,
                        DIB_USAGE.DIB_RGB_COLORS);

                    if (diRet == 0)
                    {
                        return false;
                    }

                    // Set the destination coordinates depending on whether stretch-to-fit was chosen
                    int xDest, yDest, cxDest, cyDest;
                    if (bStretch)
                    {
                        xDest = rect.left;
                        yDest = rect.top;
                        cxDest = rect.Width;
                        cyDest = rect.Height;
                    }
                    else
                    {
                        xDest = rect.left;
                        yDest = rect.top;
                        cxDest = bmp.bmWidth;
                        cyDest = bmp.bmHeight;
                    }

                    // Paint the bitmap
                    int iRet = PInvoke.StretchDIBits(
                        hdcDest,
                        xDest,
                        yDest,
                        cxDest,
                        cyDest,
                        0,
                        0,
                        bmp.bmWidth,
                        bmp.bmHeight,
                        pib,
                        (BITMAPINFO*)bi,
                        DIB_USAGE.DIB_RGB_COLORS,
                        ROP_CODE.SRCCOPY);

                    if (iRet == PInvoke.GDI_ERROR)
                    {
                        return false;
                    }
                }
            }
            finally
            {
                PInvokeCore.DeleteObject(hNullBitmap);
            }

            return true;
        }
    }
}
