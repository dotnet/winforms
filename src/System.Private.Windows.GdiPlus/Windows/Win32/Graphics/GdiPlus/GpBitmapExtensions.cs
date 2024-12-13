// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;
using DrawingColor = System.Drawing.Color;
using Windows.Win32.System.Ole;

namespace Windows.Win32.Graphics.GdiPlus;

internal static unsafe class GpBitmapExtensions
{
    public static void LockBits(
        this IPointer<GpBitmap> bitmap,
        Rectangle rect,
        ImageLockMode flags,
        PixelFormat format,
        ref BitmapData data)
    {
        // LockBits always creates a temporary copy of the data.
        PInvokeGdiPlus.GdipBitmapLockBits(
            bitmap.GetPointer(),
            rect.IsEmpty ? null : (Rect*)&rect,
            (uint)flags,
            (int)format,
            (BitmapData*)Unsafe.AsPointer(ref data)).ThrowIfFailed();

        GC.KeepAlive(bitmap);
    }

    public static void UnlockBits(this IPointer<GpBitmap> bitmap, ref BitmapData data)
    {
        PInvokeGdiPlus.GdipBitmapUnlockBits(bitmap.GetPointer(), (BitmapData*)Unsafe.AsPointer(ref data)).ThrowIfFailed();
        GC.KeepAlive(bitmap);
    }

    public static HBITMAP GetHBITMAP(this IPointer<GpBitmap> bitmap) => bitmap.GetHBITMAP(DrawingColor.LightGray);

    public static HBITMAP GetHBITMAP(this IPointer<GpBitmap> bitmap, DrawingColor background)
    {
        HBITMAP hbitmap;
        PInvokeGdiPlus.GdipCreateHBITMAPFromBitmap(
            bitmap.GetPointer(),
            &hbitmap,
            (uint)ColorTranslator.ToWin32(background)).ThrowIfFailed();

        GC.KeepAlive(bitmap);
        return hbitmap;
    }

    /// <summary>
    ///  Creates a <see cref="PICTDESC"/> structure from the specified <see cref="GpBitmap"/>.
    /// </summary>
    public static PICTDESC CreatePICTDESC(this IPointer<GpBitmap> bitmap, HPALETTE paletteHandle = default)
    {
        PICTDESC desc = new()
        {
            picType = PICTYPE.PICTYPE_BITMAP
        };

        desc.Anonymous.bmp.hbitmap = bitmap.GetHBITMAP();
        desc.Anonymous.bmp.hpal = paletteHandle;
        return desc;
    }
}
