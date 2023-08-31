// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;

namespace Windows.Win32.System.Ole;

internal partial struct OLE_HANDLE
{
    // HANDLE objects are nints and need to be sign extended.

    public static explicit operator HICON(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HBITMAP(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HPALETTE(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HMETAFILE(OLE_HANDLE handle) => new((int)handle.Value);
    public static explicit operator HENHMETAFILE(OLE_HANDLE handle) => new((int)handle.Value);

    /// <summary>
    ///  Convert the given <paramref name="handle"/> to an <see cref="Image"/> of the given <paramref name="type"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="type"/> is not a known type.</exception>
    public static Image? OleHandleToImage(
        OLE_HANDLE handle,
        PICTYPE type,
        OLE_HANDLE paletteHandle,
        int width,
        int height)
    {
        switch (type)
        {
            case PICTYPE.PICTYPE_ICON:
                return (Image)(Icon.FromHandle((HICON)handle).Clone());
            case PICTYPE.PICTYPE_METAFILE:
                WmfPlaceableFileHeader header = new()
                {
                    BboxRight = (short)width,
                    BboxBottom = (short)height
                };

                using (Metafile metafile = new((HMETAFILE)handle, header, deleteWmf: false))
                {
                    return (Image)metafile.Clone();
                }

            case PICTYPE.PICTYPE_ENHMETAFILE:
                using (Metafile metafile = new((HENHMETAFILE)handle, deleteEmf: false))
                {
                    return (Image)metafile.Clone();
                }

            case PICTYPE.PICTYPE_BITMAP:
                return Image.FromHbitmap((HBITMAP)handle, (HPALETTE)paletteHandle);
            case PICTYPE.PICTYPE_NONE:
                // MSDN says this should not be a valid value, but comctl32 returns it.
                return null;
            case PICTYPE.PICTYPE_UNINITIALIZED:
                return null;
            default:
                Debug.Fail($"Invalid image type {type}");
                throw new InvalidOperationException(nameof(type));
        }
    }
}
