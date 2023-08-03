// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;

namespace Windows.Win32.System.Ole;

internal unsafe partial struct IPicture
{
    public static object CreateObjectFromImage(Image image)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPicture> picture = CreateFromImage(image);
        return Marshal.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPicture> CreateFromImage(Image image)
    {
        PICTDESC desc = PICTDESC.FromImage(image);
        ComScope<IPicture> picture = new(null);
        PInvoke.OleCreatePictureIndirect(&desc, IID.Get<IPicture>(), fOwn: true, picture).ThrowOnFailure();
        return picture;
    }

    public static object CreateObjectFromIcon(Icon icon, bool copy)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPicture> picture = CreateFromIcon(icon, copy);
        return Marshal.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPicture> CreateFromIcon(Icon icon, bool copy)
    {
        PICTDESC desc = PICTDESC.FromIcon(icon, copy);
        ComScope<IPicture> picture = new(null);
        PInvoke.OleCreatePictureIndirect(&desc, IID.Get<IPicture>(), fOwn: true, picture).ThrowOnFailure();
        return picture;
    }

    public Image? ToImage()
    {
        fixed (IPicture* picture = &this)
        {
            OLE_HANDLE paletteHandle = default;

            PICTYPE type = (PICTYPE)picture->Type;
            if (type == PICTYPE.PICTYPE_BITMAP)
            {
                paletteHandle = picture->hPal;
            }

            return OLE_HANDLE.OleHandleToImage(picture->Handle, type, paletteHandle, picture->Width, picture->Height);
        }
    }
}
