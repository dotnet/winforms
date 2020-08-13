// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  MetafileDCWrapper is used to wrap a metafile DC so that subsequent
        ///  paint operations are rendered to a temporary bitmap.  When the
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
            private Gdi32.HBITMAP _hBitmap;
            private Gdi32.HBITMAP _hOriginalBmp;
            private readonly Gdi32.HDC _hMetafileDC;
            private RECT _destRect;

            internal unsafe MetafileDCWrapper(Gdi32.HDC hOriginalDC, Size size)
            {
                Debug.Assert(Gdi32.GetObjectType(hOriginalDC) == Gdi32.OBJ.ENHMETADC,
                    "Why wrap a non-Enhanced MetaFile DC?");

                if (size.Width < 0 || size.Height < 0)
                {
                    throw new ArgumentException(SR.ControlMetaFileDCWrapperSizeInvalid, nameof(size));
                }

                _hMetafileDC = hOriginalDC;
                _destRect = new RECT(0, 0, size.Width, size.Height);
                HDC = Gdi32.CreateCompatibleDC((Gdi32.HDC)default);

                int planes = Gdi32.GetDeviceCaps(HDC, Gdi32.DeviceCapability.PLANES);
                int bitsPixel = Gdi32.GetDeviceCaps(HDC, Gdi32.DeviceCapability.BITSPIXEL);
                _hBitmap = Gdi32.CreateBitmap(size.Width, size.Height, (uint)planes, (uint)bitsPixel, null);
                _hOriginalBmp = (Gdi32.HBITMAP)Gdi32.SelectObject(HDC, _hBitmap);
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
                    success = DICopy(_hMetafileDC, HDC, _destRect, true);
                    Debug.Assert(success, "DICopy() failed.");
                    Gdi32.SelectObject(HDC, _hOriginalBmp);
                    success = Gdi32.DeleteObject(_hBitmap).IsTrue();
                    Debug.Assert(success, "DeleteObject() failed.");
                    success = Gdi32.DeleteDC(HDC).IsTrue();
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

            internal Gdi32.HDC HDC { get; private set; }

            // ported form VB6 (Ctls\PortUtil\StdCtl.cpp:6176)
            private unsafe bool DICopy(Gdi32.HDC hdcDest, Gdi32.HDC hdcSrc, RECT rect, bool bStretch)
            {
                long i;

                // Get the bitmap from the DC by selecting in a 1x1 pixel temp bitmap
                Gdi32.HBITMAP hNullBitmap = Gdi32.CreateBitmap(1, 1, 1, 1, null);
                if (hNullBitmap.IsNull)
                {
                    return false;
                }

                try
                {
                    Gdi32.HBITMAP hBitmap = (Gdi32.HBITMAP)Gdi32.SelectObject(hdcSrc, hNullBitmap);
                    if (hBitmap.IsNull)
                    {
                        return false;
                    }

                    // Restore original bitmap
                    Gdi32.SelectObject(hdcSrc, hBitmap);

                    if (!Gdi32.GetObjectW(hBitmap, out Gdi32.BITMAP bmp))
                    {
                        return false;
                    }

                    var lpbmi = new Gdi32.BITMAPINFO
                    {
                        bmiHeader = new Gdi32.BITMAPINFOHEADER
                        {
                            biSize = (uint)sizeof(Gdi32.BITMAPINFOHEADER),
                            biWidth = bmp.bmWidth,
                            biHeight = bmp.bmHeight,
                            biPlanes = 1,
                            biBitCount = bmp.bmBitsPixel,
                            biCompression = Gdi32.BI.RGB
                        },
                        bmiColors = new byte[Gdi32.BITMAPINFO.MaxColorSize * 4]
                    };

                    // Include the palette for 256 color bitmaps
                    long iColors = 1 << (bmp.bmBitsPixel * bmp.bmPlanes);
                    if (iColors <= 256)
                    {
                        byte[] aj = ArrayPool<byte>.Shared.Rent(sizeof(Gdi32.PALETTEENTRY) * 256);
                        try
                        {
                            Gdi32.GetSystemPaletteEntries(hdcSrc, 0, (uint)iColors, aj);

                            fixed (byte* pcolors = lpbmi.bmiColors)
                            {
                                fixed (byte* ppal = aj)
                                {
                                    Gdi32.RGBQUAD* prgb = (Gdi32.RGBQUAD*)pcolors;
                                    Gdi32.PALETTEENTRY* lppe = (Gdi32.PALETTEENTRY*)ppal;

                                    // Convert the palette entries to RGB quad entries
                                    for (i = 0; i < (int)iColors; i++)
                                    {
                                        prgb[i].rgbRed = lppe[i].peRed;
                                        prgb[i].rgbBlue = lppe[i].peBlue;
                                        prgb[i].rgbGreen = lppe[i].peGreen;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(aj);
                        }
                    }

                    // Allocate memory to hold the bitmap bits
                    long bitsPerScanLine = bmp.bmBitsPixel * (long)bmp.bmWidth;
                    long bytesPerScanLine = (bitsPerScanLine + 7) / 8;
                    long totalBytesReqd = bytesPerScanLine * bmp.bmHeight;
                    byte[] lpBits = new byte[totalBytesReqd];

                    // Get the bitmap bits
                    int diRet = Gdi32.GetDIBits(
                        hdcSrc,
                        hBitmap,
                        0,
                        (uint)bmp.bmHeight,
                        lpBits,
                        ref lpbmi,
                        Gdi32.DIB.RGB_COLORS);
                    if (diRet == 0)
                    {
                        return false;
                    }

                    // Set the destination coordiates depending on whether stretch-to-fit was chosen
                    int xDest, yDest, cxDest, cyDest;
                    if (bStretch)
                    {
                        xDest = rect.left;
                        yDest = rect.top;
                        cxDest = rect.right - rect.left;
                        cyDest = rect.bottom - rect.top;
                    }
                    else
                    {
                        xDest = rect.left;
                        yDest = rect.top;
                        cxDest = bmp.bmWidth;
                        cyDest = bmp.bmHeight;
                    }

                    // Paint the bitmap
                    int iRet = Gdi32.StretchDIBits(
                        hdcDest,
                        xDest,
                        yDest,
                        cxDest,
                        cyDest,
                        0,
                        0,
                        bmp.bmWidth,
                        bmp.bmHeight,
                        lpBits,
                        ref lpbmi,
                        Gdi32.DIB.RGB_COLORS,
                        Gdi32.ROP.SRCCOPY);
                    if (iRet == NativeMethods.GDI_ERROR)
                    {
                        return false;
                    }
                }
                finally
                {
                    Gdi32.DeleteObject(hNullBitmap);
                }

                return true;
            }
        }
    }
}
