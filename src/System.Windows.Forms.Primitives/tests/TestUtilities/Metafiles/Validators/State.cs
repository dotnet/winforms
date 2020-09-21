// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Drawing;
using System.Numerics;
using static Interop;

namespace System.Windows.Forms.Metafiles
{
    internal static class State
    {
        internal static IStateValidator Brush(Color brushColor, Gdi32.BS brushStyle)
            => new BrushValidator(brushColor, brushStyle);
        internal static IStateValidator BrushColor(Color brushColor) => new BrushColorValidator(brushColor);
        internal static IStateValidator BrushStyle(Gdi32.BS brushStyle) => new BrushStyleValidator(brushStyle);
        internal static IStateValidator FontFace(string fontFaceName) => new FontFaceNameValidator(fontFaceName);
        internal static IStateValidator Pen(int penWidth, Color penColor, Gdi32.PS penStyle)
            => new PenValidator(penWidth, penColor, penStyle);
        internal static IStateValidator PenColor(Color penColor) => new PenColorValidator(penColor);
        internal static IStateValidator PenStyle(Gdi32.PS penStyle) => new PenStyleValidator(penStyle);
        internal static IStateValidator PenWidth(int penWidth) => new PenWidthValidator(penWidth);
        internal static IStateValidator Rop2(Gdi32.R2 rop2Mode) => new Rop2Validator(rop2Mode);
        internal static IStateValidator TextColor(Color textColor) => new TextColorValidator(textColor);
        internal static IStateValidator Transform(Matrix3x2 transform) => new TransformValidator(transform);
    }
}
