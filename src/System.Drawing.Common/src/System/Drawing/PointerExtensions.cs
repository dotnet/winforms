// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;
#if NET9_0_OR_GREATER
using System.Drawing.Imaging.Effects;
#endif

namespace System.Drawing;

internal static unsafe class PointerExtensions
{
    public static GdiPlus.Matrix* Pointer(this Matrix? matrix) => matrix is null ? null : matrix.NativeMatrix;
    public static GpPen* Pointer(this Pen? pen) => pen is null ? null : pen.NativePen;
    public static GpStringFormat* Pointer(this StringFormat? format) => format is null ? null : format._nativeFormat;

    internal static GpFontFamily* Pointer(this IPointer<GpFontFamily>? family) => family is null ? null : family.GetPointer();

    internal static GpFontCollection* Pointer(this IPointer<GpFontCollection>? fontCollection) =>
        fontCollection is null ? null : fontCollection.GetPointer();

    public static GpPath* Pointer(this Drawing2D.GraphicsPath? path) => path is null ? null : path._nativePath;
    public static GpBrush* Pointer(this Brush? brush) => brush is null ? null : brush.NativeBrush;
    public static GpImageAttributes* Pointer(this ImageAttributes? imageAttr) => imageAttr is null ? null : imageAttr._nativeImageAttributes;
    public static GpGraphics* Pointer(this Graphics? graphics) => graphics is null ? null : graphics.NativeGraphics;
    public static GpFont* Pointer(this Font? font) => font is null ? null : font.NativeFont;
    public static GpBitmap* Pointer(this Bitmap? bitmap) => bitmap is null ? null : ((IPointer<GpBitmap>)bitmap).GetPointer();
    public static GpMetafile* Pointer(this Metafile? metafile) => metafile is null ? null : (GpMetafile*)((Image)metafile).Pointer();
    public static GpImage* Pointer(this Image? image) => image is null ? null : image.GetPointer();
#if NET9_0_OR_GREATER
    public static CGpEffect* Pointer(this Effect? effect) => effect is null ? null : effect.NativeEffect;
#endif
}
