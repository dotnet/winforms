// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum WrapMode
{
    /// <summary>
    ///  Tiles the gradient or texture.
    /// </summary>
    Tile = GdiPlus.WrapMode.WrapModeTile,

    /// <summary>
    ///  Reverses the texture or gradient horizontally and then tiles the texture or gradient.
    /// </summary>
    TileFlipX = GdiPlus.WrapMode.WrapModeTileFlipX,

    /// <summary>
    ///  Reverses the texture or gradient vertically and then tiles the texture or gradient.
    /// </summary>
    TileFlipY = GdiPlus.WrapMode.WrapModeTileFlipY,

    /// <summary>
    ///  Reverses the texture or gradient horizontally and vertically and then tiles the texture or gradient.
    /// </summary>
    TileFlipXY = GdiPlus.WrapMode.WrapModeTileFlipXY,

    /// <summary>
    ///  The texture or gradient is not tiled.
    /// </summary>
    Clamp = GdiPlus.WrapMode.WrapModeClamp
}
