// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    internal class CachedItemHdcInfo : IDisposable, IHandle
    {
        internal CachedItemHdcInfo()
        {
        }

        private Gdi32.HDC _cachedItemHDC;
        private Size _cachedHDCSize = Size.Empty;
        private Gdi32.HBITMAP _cachedItemBitmap;

        public IntPtr Handle => (IntPtr)_cachedItemHDC;

        // this DC is cached and should only be deleted on Dispose or when the size changes.

        public Gdi32.HDC GetCachedItemDC(Gdi32.HDC toolStripHDC, Size bitmapSize)
        {
            if (_cachedHDCSize.Width < bitmapSize.Width
                 || _cachedHDCSize.Height < bitmapSize.Height)
            {
                if (_cachedItemHDC.IsNull)
                {
                    // Create a new DC - we don't have one yet.
                    _cachedItemHDC = Gdi32.CreateCompatibleDC(toolStripHDC);
                }

                // Create compatible bitmap with the correct size.
                _cachedItemBitmap = Gdi32.CreateCompatibleBitmap(toolStripHDC, bitmapSize.Width, bitmapSize.Height);
                Gdi32.HGDIOBJ oldBitmap = Gdi32.SelectObject(_cachedItemHDC, _cachedItemBitmap);

                // Delete the old bitmap
                if (!oldBitmap.IsNull)
                {
                    Gdi32.DeleteObject(oldBitmap);
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
                    Gdi32.DeleteObject(_cachedItemBitmap);
                }

                // delete the DC itself.
                Gdi32.DeleteDC(_cachedItemHDC);
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
}
