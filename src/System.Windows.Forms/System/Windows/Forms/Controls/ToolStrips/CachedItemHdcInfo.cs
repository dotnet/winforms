// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal class CachedItemHdcInfo : IDisposable, IHandle<HDC>
{
    internal CachedItemHdcInfo()
    {
    }

    private HDC _cachedItemHDC;
    private Size _cachedHDCSize = Size.Empty;
    private HBITMAP _cachedItemBitmap;

    public HDC Handle => _cachedItemHDC;

    // this DC is cached and should only be deleted on Dispose or when the size changes.

    public HDC GetCachedItemDC(HDC toolStripHDC, Size bitmapSize)
    {
        if (_cachedHDCSize.Width < bitmapSize.Width
             || _cachedHDCSize.Height < bitmapSize.Height)
        {
            if (_cachedItemHDC.IsNull)
            {
                // Create a new DC - we don't have one yet.
                _cachedItemHDC = PInvokeCore.CreateCompatibleDC(toolStripHDC);
            }

            // Create compatible bitmap with the correct size.
            _cachedItemBitmap = PInvokeCore.CreateCompatibleBitmap(toolStripHDC, bitmapSize.Width, bitmapSize.Height);
            HGDIOBJ oldBitmap = PInvokeCore.SelectObject(_cachedItemHDC, _cachedItemBitmap);

            // Delete the old bitmap
            if (!oldBitmap.IsNull)
            {
                PInvokeCore.DeleteObject(oldBitmap);
            }

            // remember what size we created.
            _cachedHDCSize = bitmapSize;
        }

        return _cachedItemHDC;
    }

    public void Dispose()
    {
        if (!_cachedItemHDC.IsNull)
        {
            if (!_cachedItemBitmap.IsNull)
            {
                PInvokeCore.DeleteObject(_cachedItemBitmap);
            }

            PInvokeCore.DeleteDC(_cachedItemHDC);
        }

        _cachedItemHDC = default;
        _cachedItemBitmap = default;
        _cachedHDCSize = Size.Empty;

        GC.SuppressFinalize(this);
    }

    ~CachedItemHdcInfo()
    {
        Dispose();
    }
}
