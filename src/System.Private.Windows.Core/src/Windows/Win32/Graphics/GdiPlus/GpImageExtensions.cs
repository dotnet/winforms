// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

namespace Windows.Win32.Graphics.GdiPlus;

internal static unsafe class GpImageExtensions
{
    [SkipLocalsInit]
    internal static RectangleF GetImageBounds(this IPointer<GpImage> image)
    {
        RectangleF bounds;
        Unit unit;

        PInvokeCore.GdipGetImageBounds(image.Pointer, (RectF*)&bounds, &unit).ThrowIfFailed();
        GC.KeepAlive(image);
        return bounds;
    }

    [SkipLocalsInit]
    internal static PixelFormat GetPixelFormat(this IPointer<GpImage> image)
    {
        int format;

        Status status = PInvokeCore.GdipGetImagePixelFormat(image.Pointer, &format);
        GC.KeepAlive(image);
        return status == Status.Ok ? (PixelFormat)format : PixelFormat.Undefined;
    }
}
