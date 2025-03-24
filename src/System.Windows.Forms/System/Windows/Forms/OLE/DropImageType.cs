// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the drop description image type.
/// </summary>
public enum DropImageType
{
    /// <summary>
    ///  No drop image preference; use the default image.
    /// </summary>
    Invalid = DROPIMAGETYPE.DROPIMAGE_INVALID,

    /// <summary>
    ///  A red bisected circle such as that found on a "no smoking" sign.
    /// </summary>
    None = DROPIMAGETYPE.DROPIMAGE_NONE,

    /// <summary>
    ///  A plus sign (+) that indicates a copy operation.
    /// </summary>
    Copy = DROPIMAGETYPE.DROPIMAGE_COPY,

    /// <summary>
    ///  An arrow that indicates a move operation.
    /// </summary>
    Move = DROPIMAGETYPE.DROPIMAGE_MOVE,

    /// <summary>
    ///  An arrow that indicates a link.
    /// </summary>
    Link = DROPIMAGETYPE.DROPIMAGE_LINK,

    /// <summary>
    ///  A tag icon that indicates that the metadata will be changed.
    /// </summary>
    Label = DROPIMAGETYPE.DROPIMAGE_LABEL,

    /// <summary>
    ///  A yellow exclamation mark that indicates that a problem has been encountered in the operation.
    /// </summary>
    Warning = DROPIMAGETYPE.DROPIMAGE_WARNING,

    /// <summary>
    ///  Windows 7 and later. Use no drop image.
    /// </summary>
    NoImage = DROPIMAGETYPE.DROPIMAGE_NOIMAGE
}
