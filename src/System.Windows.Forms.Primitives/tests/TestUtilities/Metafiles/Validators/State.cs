// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Numerics;

namespace System.Windows.Forms.Metafiles;

internal static class State
{
    internal static IStateValidator BackgroundMode(BACKGROUND_MODE backgroundMode) => new BackgroundModeValidator(backgroundMode);
    internal static IStateValidator Brush(Color brushColor, BRUSH_STYLE brushStyle)
        => new BrushValidator(brushColor, brushStyle);
    internal static IStateValidator BrushColor(Color brushColor) => new BrushColorValidator(brushColor);
    internal static IStateValidator BrushStyle(BRUSH_STYLE brushStyle) => new BrushStyleValidator(brushStyle);
    internal static IStateValidator Clipping(RECT[] clippingRectangles) => new ClippingValidator(clippingRectangles);
    internal static IStateValidator FontFace(string fontFaceName) => new FontFaceNameValidator(fontFaceName);
    internal static IStateValidator Pen(int penWidth, Color penColor, PEN_STYLE penStyle)
        => new PenValidator(penWidth, penColor, penStyle);
    internal static IStateValidator PenColor(Color penColor) => new PenColorValidator(penColor);
    internal static IStateValidator PenStyle(PEN_STYLE penStyle) => new PenStyleValidator(penStyle);
    internal static IStateValidator PenWidth(int penWidth) => new PenWidthValidator(penWidth);
    internal static IStateValidator Rop2(R2_MODE rop2Mode) => new Rop2Validator(rop2Mode);
    internal static IStateValidator TextColor(Color textColor) => new TextColorValidator(textColor);
    internal static IStateValidator Transform(Matrix3x2 transform) => new TransformValidator(transform);
}
