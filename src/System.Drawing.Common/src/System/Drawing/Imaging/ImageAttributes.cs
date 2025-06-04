// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.CompilerServices;
#if NET9_0_OR_GREATER
using System.ComponentModel;
#endif

namespace System.Drawing.Imaging;

// sdkinc\GDIplusImageAttributes.h

// There are 5 possible sets of color adjustments:
//          ColorAdjustDefault,
//          ColorAdjustBitmap,
//          ColorAdjustBrush,
//          ColorAdjustPen,
//          ColorAdjustText,

// Bitmaps, Brushes, Pens, and Text will all use any color adjustments
// that have been set into the default ImageAttributes until their own
// color adjustments have been set. So as soon as any "Set" method is
// called for Bitmaps, Brushes, Pens, or Text, then they start from
// scratch with only the color adjustments that have been set for them.
// Calling Reset removes any individual color adjustments for a type
// and makes it revert back to using all the default color adjustments
// (if any). The SetToIdentity method is a way to force a type to
// have no color adjustments at all, regardless of what previous adjustments
// have been set for the defaults or for that type.

/// <summary>
///  Contains information about how image colors are manipulated during rendering.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public sealed unsafe class ImageAttributes : ICloneable, IDisposable
{
    private const int ColorMapStackSpace = 32;

    internal GpImageAttributes* _nativeImageAttributes;

    internal void SetNativeImageAttributes(GpImageAttributes* handle)
    {
        if (handle is null)
            throw new ArgumentNullException(nameof(handle));

        _nativeImageAttributes = handle;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref='ImageAttributes'/> class.
    /// </summary>
    public ImageAttributes()
    {
        GpImageAttributes* newImageAttributes;

        PInvokeGdiPlus.GdipCreateImageAttributes(&newImageAttributes).ThrowIfFailed();
        SetNativeImageAttributes(newImageAttributes);
    }

    internal ImageAttributes(GpImageAttributes* newNativeImageAttributes)
        => SetNativeImageAttributes(newNativeImageAttributes);

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='ImageAttributes'/>.
    /// </summary>
    public void Dispose()
    {
        if (_nativeImageAttributes is null)
        {
            return;
        }

        Status status = !Gdip.Initialized ? Status.Ok : PInvokeGdiPlus.GdipDisposeImageAttributes(_nativeImageAttributes);
        _nativeImageAttributes = null;
        Debug.Assert(status == Status.Ok, $"GDI+ returned an error status: {status}");
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///  Cleans up Windows resources for this <see cref='ImageAttributes'/>.
    /// </summary>
    ~ImageAttributes() => Dispose();

    /// <summary>
    ///  Creates an exact copy of this <see cref='ImageAttributes'/>.
    /// </summary>
    public object Clone()
    {
        GpImageAttributes* clone;
        PInvokeGdiPlus.GdipCloneImageAttributes(_nativeImageAttributes, &clone).ThrowIfFailed();
        GC.KeepAlive(this);

        return new ImageAttributes(clone);
    }

    /// <summary>
    ///  Sets the 5 X 5 color adjust matrix to the specified <see cref='Matrix'/>.
    /// </summary>
    public void SetColorMatrix(ColorMatrix newColorMatrix) =>
        SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);

    /// <summary>
    ///  Sets the 5 X 5 color adjust matrix to the specified 'Matrix' with the specified 'ColorMatrixFlags'.
    /// </summary>
    public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag flags) =>
        SetColorMatrix(newColorMatrix, flags, ColorAdjustType.Default);

    /// <summary>
    ///  Sets the 5 X 5 color adjust matrix to the specified 'Matrix' with the  specified 'ColorMatrixFlags'.
    /// </summary>
    public void SetColorMatrix(ColorMatrix newColorMatrix, ColorMatrixFlag mode, ColorAdjustType type) =>
        SetColorMatrices(newColorMatrix, null, mode, type);

    /// <summary>
    ///  Clears the color adjust matrix to all zeroes.
    /// </summary>
    public void ClearColorMatrix() => ClearColorMatrix(ColorAdjustType.Default);

    /// <summary>
    ///  Clears the color adjust matrix.
    /// </summary>
    public void ClearColorMatrix(ColorAdjustType type)
    {
        PInvokeGdiPlus.GdipSetImageAttributesColorMatrix(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            false,
            null,
            null,
            (ColorMatrixFlags)ColorMatrixFlag.Default).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Sets a color adjust matrix for image colors and a separate gray scale adjust matrix for gray scale values.
    /// </summary>
    public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix? grayMatrix) =>
        SetColorMatrices(newColorMatrix, grayMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);

    public void SetColorMatrices(ColorMatrix newColorMatrix, ColorMatrix? grayMatrix, ColorMatrixFlag flags) =>
        SetColorMatrices(newColorMatrix, grayMatrix, flags, ColorAdjustType.Default);

    public void SetColorMatrices(
        ColorMatrix newColorMatrix,
        ColorMatrix? grayMatrix,
        ColorMatrixFlag mode,
        ColorAdjustType type)
    {
        ArgumentNullException.ThrowIfNull(newColorMatrix);

        if (grayMatrix is not null)
        {
            fixed (void* cm = &newColorMatrix.GetPinnableReference())
            fixed (void* gm = &grayMatrix.GetPinnableReference())
            {
                PInvokeGdiPlus.GdipSetImageAttributesColorMatrix(
                    _nativeImageAttributes,
                    (GdiPlus.ColorAdjustType)type,
                    enableFlag: true,
                    (GdiPlus.ColorMatrix*)cm,
                    (GdiPlus.ColorMatrix*)gm,
                    (ColorMatrixFlags)mode).ThrowIfFailed();
            }
        }
        else
        {
            fixed (void* cm = &newColorMatrix.GetPinnableReference())
            {
                PInvokeGdiPlus.GdipSetImageAttributesColorMatrix(
                    _nativeImageAttributes,
                    (GdiPlus.ColorAdjustType)type,
                    enableFlag: true,
                    (GdiPlus.ColorMatrix*)cm,
                    null,
                    (ColorMatrixFlags)mode).ThrowIfFailed();
            }
        }

        GC.KeepAlive(this);
    }

    public void SetThreshold(float threshold) => SetThreshold(threshold, ColorAdjustType.Default);

    public void SetThreshold(float threshold, ColorAdjustType type) => SetThreshold(threshold, type, enableFlag: true);

    public void ClearThreshold() => ClearThreshold(ColorAdjustType.Default);

    public void ClearThreshold(ColorAdjustType type) => SetThreshold(0.0f, type, enableFlag: false);

    private void SetThreshold(float threshold, ColorAdjustType type, bool enableFlag)
    {
        PInvokeGdiPlus.GdipSetImageAttributesThreshold(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            enableFlag,
            threshold).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void SetGamma(float gamma) => SetGamma(gamma, ColorAdjustType.Default);

    public void SetGamma(float gamma, ColorAdjustType type) => SetGamma(gamma, type, enableFlag: true);

    public void ClearGamma() => ClearGamma(ColorAdjustType.Default);

    public void ClearGamma(ColorAdjustType type) => SetGamma(0.0f, type, enableFlag: false);

    private void SetGamma(float gamma, ColorAdjustType type, bool enableFlag)
    {
        PInvokeGdiPlus.GdipSetImageAttributesGamma(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            enableFlag,
            gamma).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void SetNoOp() => SetNoOp(ColorAdjustType.Default);

    public void SetNoOp(ColorAdjustType type) => SetNoOp(type, enableFlag: true);

    public void ClearNoOp() => ClearNoOp(ColorAdjustType.Default);

    public void ClearNoOp(ColorAdjustType type) => SetNoOp(type, enableFlag: false);

    private void SetNoOp(ColorAdjustType type, bool enableFlag)
    {
        PInvokeGdiPlus.GdipSetImageAttributesNoOp(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            enableFlag).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void SetColorKey(Color colorLow, Color colorHigh) =>
        SetColorKey(colorLow, colorHigh, ColorAdjustType.Default);

    public void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type) =>
        SetColorKey(colorLow, colorHigh, type, enableFlag: true);

    public void ClearColorKey() => ClearColorKey(ColorAdjustType.Default);

    public void ClearColorKey(ColorAdjustType type) => SetColorKey(Color.Empty, Color.Empty, type, enableFlag: false);

    private void SetColorKey(Color colorLow, Color colorHigh, ColorAdjustType type, bool enableFlag)
    {
        PInvokeGdiPlus.GdipSetImageAttributesColorKeys(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            enableFlag,
            (uint)colorLow.ToArgb(),
            (uint)colorHigh.ToArgb()).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void SetOutputChannel(ColorChannelFlag flags) => SetOutputChannel(flags, ColorAdjustType.Default);

    public void SetOutputChannel(ColorChannelFlag flags, ColorAdjustType type) =>
        SetOutputChannel(type, flags, enableFlag: true);

    public void ClearOutputChannel() => ClearOutputChannel(ColorAdjustType.Default);

    public void ClearOutputChannel(ColorAdjustType type) =>
        SetOutputChannel(type, ColorChannelFlag.ColorChannelLast, enableFlag: false);

    private void SetOutputChannel(ColorAdjustType type, ColorChannelFlag flags, bool enableFlag)
    {
        PInvokeGdiPlus.GdipSetImageAttributesOutputChannel(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            enableFlag,
            (ColorChannelFlags)flags).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void SetOutputChannelColorProfile(string colorProfileFilename) =>
        SetOutputChannelColorProfile(colorProfileFilename, ColorAdjustType.Default);

    public void SetOutputChannelColorProfile(string colorProfileFilename, ColorAdjustType type)
    {
        // Called in order to emulate exception behavior from .NET Framework related to invalid file paths.
        Path.GetFullPath(colorProfileFilename);

        fixed (char* n = colorProfileFilename)
        {
            PInvokeGdiPlus.GdipSetImageAttributesOutputChannelColorProfile(
                _nativeImageAttributes,
                (GdiPlus.ColorAdjustType)type,
                enableFlag: true,
                n).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }

    public void ClearOutputChannelColorProfile() => ClearOutputChannel(ColorAdjustType.Default);

    public void ClearOutputChannelColorProfile(ColorAdjustType type)
    {
        PInvokeGdiPlus.GdipSetImageAttributesOutputChannel(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            false,
            ColorChannelFlags.ColorChannelFlagsLast).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    /// <inheritdoc cref="SetRemapTable(ColorMap[], ColorAdjustType)"/>
    public void SetRemapTable(params ColorMap[] map) => SetRemapTable(map, ColorAdjustType.Default);

    /// <summary>
    ///  Sets the default color-remap table.
    /// </summary>
    /// <inheritdoc cref="SetRemapTable(ColorAdjustType, ReadOnlySpan{ColorMap})"/>
#if NET9_0_OR_GREATER
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public void SetRemapTable(ColorMap[] map, ColorAdjustType type)
    {
        ArgumentNullException.ThrowIfNull(map);
        SetRemapTable(type, map);
    }

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="SetRemapTable(ColorMap[], ColorAdjustType)"/>
    public void SetRemapTable(params ReadOnlySpan<ColorMap> map) => SetRemapTable(ColorAdjustType.Default, map);

    /// <inheritdoc cref="SetRemapTable(ColorMap[], ColorAdjustType)"/>
    public void SetRemapTable(params ReadOnlySpan<(Color OldColor, Color NewColor)> map) => SetRemapTable(ColorAdjustType.Default, map);
#endif

    /// <summary>
    ///  Sets the color-remap table for a specified category.
    /// </summary>
    /// <param name="type">
    ///  An element of <see cref="ColorAdjustType"/> that specifies the category for which the color-remap table is set.
    /// </param>
    /// <param name="map">
    ///  A series of color pairs mapping an existing color to a new color.
    /// </param>
#if NET9_0_OR_GREATER
    public
#else
    private
#endif
    void SetRemapTable(ColorAdjustType type, params ReadOnlySpan<ColorMap> map)
    {
        // Color is being generated incorrectly so we can't use GdiPlus.ColorMap directly.
        // https://github.com/microsoft/CsWin32/issues/1121

        StackBuffer stackBuffer = default;
        using BufferScope<(ARGB, ARGB)> buffer = new(stackBuffer, map.Length);

        for (int i = 0; i < map.Length; i++)
        {
            buffer[i] = (map[i].OldColor, map[i].NewColor);
        }

        fixed (void* m = buffer)
        {
            PInvokeGdiPlus.GdipSetImageAttributesRemapTable(
                _nativeImageAttributes,
                (GdiPlus.ColorAdjustType)type,
                enableFlag: true,
                (uint)map.Length,
                (GdiPlus.ColorMap*)m).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="SetRemapTable(ColorAdjustType, ReadOnlySpan{ColorMap})"/>
    public void SetRemapTable(ColorAdjustType type, params ReadOnlySpan<(Color OldColor, Color NewColor)> map)
    {
        StackBuffer stackBuffer = default;
        using BufferScope<(ARGB, ARGB)> buffer = new(stackBuffer, map.Length);

        for (int i = 0; i < map.Length; i++)
        {
            buffer[i] = (map[i].OldColor, map[i].NewColor);
        }

        fixed (void* m = buffer)
        {
            PInvokeGdiPlus.GdipSetImageAttributesRemapTable(
                _nativeImageAttributes,
                (GdiPlus.ColorAdjustType)type,
                enableFlag: true,
                (uint)map.Length,
                (GdiPlus.ColorMap*)m).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }
#endif

    [InlineArray(ColorMapStackSpace)]
    private struct StackBuffer
    {
        internal (ARGB, ARGB) _element0;
    }

    public void ClearRemapTable() => ClearRemapTable(ColorAdjustType.Default);

    public void ClearRemapTable(ColorAdjustType type)
    {
        PInvokeGdiPlus.GdipSetImageAttributesRemapTable(
            _nativeImageAttributes,
            (GdiPlus.ColorAdjustType)type,
            enableFlag: false,
            0,
            null).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void SetBrushRemapTable(params ColorMap[] map) => SetRemapTable(map, ColorAdjustType.Brush);

#if NET9_0_OR_GREATER
    /// <inheritdoc cref="SetRemapTable(ColorAdjustType, ReadOnlySpan{ColorMap})"/>
    public void SetBrushRemapTable(params ReadOnlySpan<ColorMap> map) => SetRemapTable(ColorAdjustType.Brush, map);

    /// <inheritdoc cref="SetRemapTable(ColorAdjustType, ReadOnlySpan{ColorMap})"/>
    public void SetBrushRemapTable(params ReadOnlySpan<(Color OldColor, Color NewColor)> map) => SetRemapTable(ColorAdjustType.Brush, map);
#endif

    public void ClearBrushRemapTable() => ClearRemapTable(ColorAdjustType.Brush);

    public void SetWrapMode(Drawing2D.WrapMode mode) => SetWrapMode(mode, default, clamp: false);

    public void SetWrapMode(Drawing2D.WrapMode mode, Color color) => SetWrapMode(mode, color, clamp: false);

    public void SetWrapMode(Drawing2D.WrapMode mode, Color color, bool clamp)
    {
        PInvokeGdiPlus.GdipSetImageAttributesWrapMode(
            _nativeImageAttributes,
            (WrapMode)mode,
            (uint)color.ToArgb(),
            clamp).ThrowIfFailed();

        GC.KeepAlive(this);
    }

    public void GetAdjustedPalette(ColorPalette palette, ColorAdjustType type)
    {
        using var buffer = palette.ConvertToBuffer();
        fixed (void* p = buffer)
        {
            PInvokeGdiPlus.GdipGetImageAttributesAdjustedPalette(
                _nativeImageAttributes,
                (GdiPlus.ColorPalette*)p,
                (GdiPlus.ColorAdjustType)type).ThrowIfFailed();
        }

        GC.KeepAlive(this);
    }
}
