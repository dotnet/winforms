// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ErrorProvider
{
    /// <summary>
    ///  This represents the HRGN of icon. The region is calculate from the icon's mask.
    /// </summary>
    internal class IconRegion : IHandle<HICON>
    {
        private Region? _region;
        private readonly Icon _icon;

        public IconRegion(Icon icon)
        {
            _icon = new Icon(icon, ScaleHelper.LogicalSmallSystemIconSize);
        }

        /// <summary>
        ///  Returns the handle of the icon.
        /// </summary>
        public HICON Handle => (HICON)_icon.Handle;

        /// <summary>
        ///  Returns the handle of the region.
        /// </summary>
        public unsafe Region Region
        {
            get
            {
                if (_region is not null)
                {
                    return _region;
                }

                _region = new Region(new Rectangle(0, 0, 0, 0));

                Size size = _icon.Size;
                Bitmap bitmap = _icon.ToBitmap();
                using HBITMAP mask = (HBITMAP)ControlPaint.CreateHBitmapTransparencyMask(bitmap);
                bitmap.Dispose();

                // It is been observed that users can use non standard size icons
                // (not a 16 bit multiples for width and height) and GetBitmapBits method allocate bytes in multiple
                // of 16 bits for each row. Following calculation is to get right width in bytes.
                int bitmapBitsAllocationSize = 16;

                // If width is not multiple of 16, we need to allocate BitmapBitsAllocationSize for remaining bits.
                int widthInBytes = 2 * ((size.Width + 15) / bitmapBitsAllocationSize); // its in bytes.
                using BufferScope<byte> bits = new(widthInBytes * size.Height);
                fixed (void* pbits = bits)
                {
                    PInvoke.GetBitmapBits(mask, bits.Length, pbits);

                    for (int y = 0; y < size.Height; y++)
                    {
                        for (int x = 0; x < size.Width; x++)
                        {
                            // see if bit is set in mask. bits in byte are reversed. 0 is black (set).
                            if ((bits[y * widthInBytes + x / 8] & (1 << (7 - (x % 8)))) == 0)
                            {
                                _region.Union(new Rectangle(x, y, 1, 1));
                            }
                        }
                    }
                }

                _region.Intersect(new Rectangle(0, 0, size.Width, size.Height));

                return _region;
            }
        }

        /// <summary>
        ///  Return the size of the icon.
        /// </summary>
        public Size Size => _icon.Size;

        /// <summary>
        ///  Release any resources held by this Object.
        /// </summary>
        public void Dispose()
        {
            _region?.Dispose();
            _region = null;
        }
    }
}
