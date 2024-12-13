// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;
using Windows.Win32.System.Ole;

namespace Windows.Win32.Graphics.GdiPlus;

internal static unsafe class GpImageExtensions
{
    [SkipLocalsInit]
    internal static RectangleF GetImageBounds(this IPointer<GpImage> image)
    {
        RectangleF bounds;
        Unit unit;

        PInvokeGdiPlus.GdipGetImageBounds(image.GetPointer(), (RectF*)&bounds, &unit).ThrowIfFailed();
        GC.KeepAlive(image);
        return bounds;
    }

    [SkipLocalsInit]
    internal static PixelFormat GetPixelFormat(this IPointer<GpImage> image)
    {
        int format;

        Status status = PInvokeGdiPlus.GdipGetImagePixelFormat(image.GetPointer(), &format);
        GC.KeepAlive(image);
        return status == Status.Ok ? (PixelFormat)format : PixelFormat.Undefined;
    }

    /// <summary>
    ///  Create a <see cref="PICTDESC"/> struct describing the given <paramref name="image"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The image type isn't supported.</exception>
    public static PICTDESC CreatePICTDESC(this IPointer<GpImage> image) => image switch
    {
        IPointer<GpBitmap> bitmap => bitmap.CreatePICTDESC(),
        IPointer<GpMetafile> metafile => metafile.CreatePICTDESC(),
        _ => throw new InvalidOperationException()
    };

    public static IPictureDisp.Interface CreateIPictureDispRCW(this IPointer<GpImage> image)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPictureDisp> picture = image.CreateIPictureDisp();
        return (IPictureDisp.Interface)ComHelpers.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPictureDisp> CreateIPictureDisp(this IPointer<GpImage> image)
    {
        PICTDESC desc = image.CreatePICTDESC();
        ComScope<IPictureDisp> picture = new(null);
        PInvokeCore.OleCreatePictureIndirect(&desc, IID.Get<IPictureDisp>(), fOwn: true, picture).ThrowOnFailure();
        return picture;
    }

    public static object CreateIPictureRCW(this IPointer<GpImage> image)
    {
        // GetObjectForIUnknown increments the ref count so we need to dispose.
        using ComScope<IPicture> picture = image.CreateIPicture();
        return (IPicture.Interface)ComHelpers.GetObjectForIUnknown(picture);
    }

    public static ComScope<IPicture> CreateIPicture(this IPointer<GpImage> image)
    {
        PICTDESC desc = image.CreatePICTDESC();
        ComScope<IPicture> picture = new(null);
        PInvokeCore.OleCreatePictureIndirect(&desc, IID.Get<IPicture>(), fOwn: true, picture).ThrowOnFailure();
        return picture;
    }
}
