// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.Graphics.GdiPlus;

namespace Windows.Win32.System.Ole;

internal static unsafe class IPictureExtensions
{
    public static object CreateObjectFromImage(IPointer<GpImage> image)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPicture> picture = CreateFromImage(image);
        return Marshal.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPicture> CreateFromImage(IPointer<GpImage> image)
    {
        PICTDESC desc = image.CreatePICTDESC();
        ComScope<IPicture> picture = new(null);
        PInvokeCore.OleCreatePictureIndirect(&desc, IID.Get<IPicture>(), fOwn: true, picture).ThrowOnFailure();
        return picture;
    }

    public static object CreateObjectFromIcon(IIcon icon, bool copy)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPicture> picture = CreateFromIcon(icon, copy);
        return Marshal.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPicture> CreateFromIcon(IIcon icon, bool copy)
    {
        PICTDESC desc = icon.CreatePICTDESC(copy);
        ComScope<IPicture> picture = new(null);
        PInvokeCore.OleCreatePictureIndirect(&desc, IID.Get<IPicture>(), fOwn: copy, picture).ThrowOnFailure();
        return picture;
    }
}
