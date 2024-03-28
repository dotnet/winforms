// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Specifies available options for color-adjusting. GDI+ can adjust color data only, grayscale data only, or both.
/// </summary>
public enum ColorMatrixFlag
{
    /// <summary>
    ///  Both colors and grayscale are color-adjusted.
    /// </summary>
    Default = ColorMatrixFlags.ColorMatrixFlagsDefault,

    /// <summary>
    ///  Grayscale values are not color-adjusted.
    /// </summary>
    SkipGrays = ColorMatrixFlags.ColorMatrixFlagsSkipGrays,

    /// <summary>
    ///  Only grayscale values are color-adjusted.
    /// </summary>
    AltGrays = ColorMatrixFlags.ColorMatrixFlagsAltGray
}
