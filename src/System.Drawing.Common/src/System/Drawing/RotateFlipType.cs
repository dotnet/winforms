// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

/// <summary>
///  Specifies the different patterns available 'RotateFlipType' objects.
/// </summary>
public enum RotateFlipType
{
    RotateNoneFlipNone = GdiPlus.RotateFlipType.RotateNoneFlipNone,
    Rotate90FlipNone = GdiPlus.RotateFlipType.Rotate90FlipNone,
    Rotate180FlipNone = GdiPlus.RotateFlipType.Rotate180FlipNone,
    Rotate270FlipNone = GdiPlus.RotateFlipType.Rotate270FlipNone,
    RotateNoneFlipX = GdiPlus.RotateFlipType.RotateNoneFlipX,
    Rotate90FlipX = GdiPlus.RotateFlipType.Rotate90FlipX,
    Rotate180FlipX = GdiPlus.RotateFlipType.Rotate180FlipX,
    Rotate270FlipX = GdiPlus.RotateFlipType.Rotate270FlipX,
    RotateNoneFlipY = Rotate180FlipX,
    Rotate90FlipY = Rotate270FlipX,
    Rotate180FlipY = RotateNoneFlipX,
    Rotate270FlipY = Rotate90FlipX,
    RotateNoneFlipXY = Rotate180FlipNone,
    Rotate90FlipXY = Rotate270FlipNone,
    Rotate180FlipXY = RotateNoneFlipNone,
    Rotate270FlipXY = Rotate90FlipNone
}
