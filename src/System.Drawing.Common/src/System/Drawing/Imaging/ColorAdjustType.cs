// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Specifies which GDI+ objects use color adjustment information.
/// </summary>
public enum ColorAdjustType
{
    /// <summary>
    ///  Defines color adjustment information that is used by all GDI+ objects that do not have their own color
    ///  adjustment information.
    /// </summary>
    Default = GdiPlus.ColorAdjustType.ColorAdjustTypeDefault,

    /// <summary>
    ///  Defines color adjustment information for <see cref='Drawing.Bitmap'/> objects.
    /// </summary>
    Bitmap = GdiPlus.ColorAdjustType.ColorAdjustTypeBitmap,

    /// <summary>
    ///  Defines color adjustment information for <see cref='Drawing.Brush'/> objects.
    /// </summary>
    Brush = GdiPlus.ColorAdjustType.ColorAdjustTypeBrush,

    /// <summary>
    ///  Defines color adjustment information for <see cref='Drawing.Pen'/> objects.
    /// </summary>
    Pen = GdiPlus.ColorAdjustType.ColorAdjustTypePen,

    /// <summary>
    ///  Defines color adjustment information for text.
    /// </summary>
    Text = GdiPlus.ColorAdjustType.ColorAdjustTypeText,

    /// <summary>
    ///  Specifies the number of types specified.
    /// </summary>
    Count = GdiPlus.ColorAdjustType.ColorAdjustTypeCount,

    /// <summary>
    ///  Specifies the number of types specified.
    /// </summary>
    Any = GdiPlus.ColorAdjustType.ColorAdjustTypeAny
}
