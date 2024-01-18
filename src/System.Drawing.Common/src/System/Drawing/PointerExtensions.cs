// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;

namespace System.Drawing;

internal static unsafe class PointerExtensions
{
    public static GdiPlus.Matrix* Pointer(this Matrix? matrix) => matrix is null ? null : matrix.NativeMatrix;
    public static GpPen* Pointer(this Pen? pen) => pen is null ? null : pen.NativePen;
    public static GpStringFormat* Pointer(this StringFormat? format) => format is null ? null : format._nativeFormat;
    public static GpFontFamily* Pointer(this FontFamily? family) => family is null ? null : family.NativeFamily;
    public static GpPath* Pointer(this Drawing2D.GraphicsPath? path) => path is null ? null : path._nativePath;
    public static GpBrush* Pointer(this Brush? brush) => brush is null ? null : brush.NativeBrush;
    public static GpImageAttributes* Pointer(this ImageAttributes? imageAttr) => imageAttr is null ? null : imageAttr._nativeImageAttributes;
    public static GpGraphics* Pointer(this Graphics? graphics) => graphics is null ? null : graphics.NativeGraphics;
    public static GpFont* Pointer(this Font? font) => font is null ? null : font.NativeFont;
    public static GpBitmap* Pointer(this Bitmap? bitmap) => bitmap is null ? null : bitmap.NativeBitmap;
    public static GpMetafile* Pointer(this Metafile? metafile) => metafile is null ? null : (GpMetafile*)metafile._nativeImage;
}
