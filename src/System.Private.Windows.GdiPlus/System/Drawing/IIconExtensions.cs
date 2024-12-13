// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.System.Ole;

namespace System.Drawing;

public static unsafe class IIconExtensions
{
    internal static unsafe PICTDESC CreatePICTDESC(this IIcon icon, bool copy)
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

    internal static object CreateIPictureRCW(this IIcon icon, bool copy)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPicture> picture = icon.CreateIPicture(copy);
        return Marshal.GetObjectForIUnknown(picture);
    }

    internal static ComScope<IPicture> CreateIPicture(this IIcon icon, bool copy)
    {
        PICTDESC desc = icon.CreatePICTDESC(copy);
        ComScope<IPicture> picture = new(null);
        PInvokeCore.OleCreatePictureIndirect(&desc, IID.Get<IPicture>(), fOwn: copy, picture).ThrowOnFailure();
        return picture;
    }
}
