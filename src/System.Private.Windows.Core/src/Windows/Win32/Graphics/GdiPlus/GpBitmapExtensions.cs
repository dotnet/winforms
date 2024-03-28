// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.CompilerServices;

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
        PInvokeCore.GdipBitmapLockBits(
            bitmap.Pointer,
            rect.IsEmpty ? null : (Rect*)&rect,
            (uint)flags,
            (int)format,
            (BitmapData*)Unsafe.AsPointer(ref data)).ThrowIfFailed();

        GC.KeepAlive(bitmap);
    }

    public static void UnlockBits(this IPointer<GpBitmap> bitmap, ref BitmapData data)
    {
        PInvokeCore.GdipBitmapUnlockBits(bitmap.Pointer, (BitmapData*)Unsafe.AsPointer(ref data)).ThrowIfFailed();
        GC.KeepAlive(bitmap);
    }

    public static HBITMAP GetHBITMAP(this IPointer<GpBitmap> bitmap) => bitmap.GetHBITMAP(Color.LightGray);

    public static HBITMAP GetHBITMAP(this IPointer<GpBitmap> bitmap, Color background)
    {
        HBITMAP hbitmap;
        PInvokeCore.GdipCreateHBITMAPFromBitmap(
            bitmap.Pointer,
            &hbitmap,
            (uint)ColorTranslator.ToWin32(background)).ThrowIfFailed();

        GC.KeepAlive(bitmap);
        return hbitmap;
    }
}
