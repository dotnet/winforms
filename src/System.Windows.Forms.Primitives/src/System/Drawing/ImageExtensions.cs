// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;
using Windows.Win32.System.Com;
using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Ole;

internal static unsafe class ImageExtensions
{
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

    public static Image? ToImage(IPictureDisp* picture)
    {
        OLE_HANDLE paletteHandle = default;

        IDispatch* dispatch = (IDispatch*)picture;
        using VARIANT variant = default;
        dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_TYPE, &variant).ThrowOnFailure();
        PICTYPE type = (PICTYPE)variant.data.iVal;
        if (type == PICTYPE.PICTYPE_BITMAP)
        {
            dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_HPAL, &variant).ThrowOnFailure();
            paletteHandle = (OLE_HANDLE)variant.data.uintVal;
        }

        dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_HANDLE, &variant).ThrowOnFailure();
        OLE_HANDLE handle = (OLE_HANDLE)variant.data.uintVal;

        dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_WIDTH, &variant).ThrowOnFailure();
        int width = variant.data.intVal;

        dispatch->TryGetProperty((int)PInvokeCore.DISPID_PICT_HEIGHT, &variant).ThrowOnFailure();
        int height = variant.data.intVal;

        return OleHandleToImage(handle, type, paletteHandle, width, height);
    }

    public static Image? ToImage(IPicture* picture)
    {
        OLE_HANDLE paletteHandle = default;

        picture->get_Type(out PICTYPE type).ThrowOnFailure();
        if (type == PICTYPE.PICTYPE_BITMAP)
        {
            picture->get_hPal(out paletteHandle).ThrowOnFailure();
        }

        picture->get_Handle(out OLE_HANDLE handle).ThrowOnFailure();
        picture->get_Width(out int width).ThrowOnFailure();
        picture->get_Height(out int height).ThrowOnFailure();

        return OleHandleToImage(handle, type, paletteHandle, width, height);
    }
}
