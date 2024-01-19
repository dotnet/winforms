// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum WrapMode
{
    Tile = GdiPlus.WrapMode.WrapModeTile,
    TileFlipX = GdiPlus.WrapMode.WrapModeTileFlipX,
    TileFlipY = GdiPlus.WrapMode.WrapModeTileFlipY,
    TileFlipXY = GdiPlus.WrapMode.WrapModeTileFlipXY,
    Clamp = GdiPlus.WrapMode.WrapModeClamp,
}
