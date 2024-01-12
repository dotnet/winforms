// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Imaging;

namespace Windows.Win32.System.Ole;

internal partial struct PICTDESC
{
    public static PICTDESC FromBitmap(Bitmap bitmap, HPALETTE paletteHandle = default)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_BITMAP
        };

        desc.Anonymous.bmp.hbitmap = (HBITMAP)bitmap.GetHbitmap();
        desc.Anonymous.bmp.hpal = paletteHandle;
        return desc;
    }

    public static PICTDESC FromIcon(Icon icon, bool copy)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_ICON
        };

        desc.Anonymous.icon.hicon = copy ?
            (HICON)PInvokeCore.CopyImage(
                (HANDLE)icon.Handle,
                GDI_IMAGE_TYPE.IMAGE_ICON,
                icon.Width,
                icon.Height,
                IMAGE_FLAGS.LR_DEFAULTCOLOR).Value
            : (HICON)icon.Handle;

        GC.KeepAlive(icon);
        return desc;
    }

    public static PICTDESC FromMetafile(Metafile metafile)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_ENHMETAFILE
        };

        desc.Anonymous.emf.hemf = (HENHMETAFILE)metafile.GetHenhmetafile();
        return desc;
    }

    /// <summary>
    ///  Create a <see cref="PICTDESC"/> struct describing the given <paramref name="image"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The image type isn't supported.</exception>
    public static PICTDESC FromImage(Image image) => image switch
    {
        Bitmap bitmap => FromBitmap(bitmap),
        Metafile metafile => FromMetafile(metafile),
        _ => throw new InvalidOperationException()
    };
}
