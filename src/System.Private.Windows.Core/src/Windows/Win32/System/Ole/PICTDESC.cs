// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.Graphics.GdiPlus;

namespace Windows.Win32.System.Ole;

internal partial struct PICTDESC
{
    public static PICTDESC FromBitmap(IPointer<GpBitmap> bitmap, HPALETTE paletteHandle = default)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_BITMAP
        };

        desc.Anonymous.bmp.hbitmap = bitmap.GetHBITMAP();
        desc.Anonymous.bmp.hpal = paletteHandle;
        return desc;
    }

    public static PICTDESC FromIcon(IIcon icon, bool copy)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_ICON
        };

        Size size = icon.Size;
        desc.Anonymous.icon.hicon = copy ?
            (HICON)PInvokeCore.CopyImage(
                (HANDLE)icon.Handle,
                GDI_IMAGE_TYPE.IMAGE_ICON,
                size.Width,
                size.Height,
                IMAGE_FLAGS.LR_DEFAULTCOLOR).Value
            : icon.Handle;

        GC.KeepAlive(icon);
        return desc;
    }

    public static PICTDESC FromMetafile(IPointer<GpMetafile> metafile)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_ENHMETAFILE
        };

        desc.Anonymous.emf.hemf = metafile.GetHENHMETAFILE();
        return desc;
    }

    /// <summary>
    ///  Create a <see cref="PICTDESC"/> struct describing the given <paramref name="image"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The image type isn't supported.</exception>
    public static PICTDESC FromImage(IImage image) => image switch
    {
        IPointer<GpBitmap> bitmap => FromBitmap(bitmap),
        IPointer<GpMetafile> metafile => FromMetafile(metafile),
        _ => throw new InvalidOperationException()
    };
}
