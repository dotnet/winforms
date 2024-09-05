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
    Invalid = -1,

    /// <summary>
    ///  A red bisected circle such as that found on a "no smoking" sign.
    /// </summary>
    None = 0,

    /// <summary>
    ///  A plus sign (+) that indicates a copy operation.
    /// </summary>
    Copy = DragDropEffects.Copy,

    /// <summary>
    ///  An arrow that indicates a move operation.
    /// </summary>
    Move = DragDropEffects.Move,

    /// <summary>
    ///  An arrow that indicates a link.
    /// </summary>
    Link = DragDropEffects.Link,

    /// <summary>
    ///  A tag icon that indicates that the metadata will be changed.
    /// </summary>
    Label = 6,

    /// <summary>
    ///  A yellow exclamation mark that indicates that a problem has been encountered in the operation.
    /// </summary>
    Warning = 7,

    /// <summary>
    ///  Windows 7 and later. Use no drop image.
    /// </summary>
    NoImage = 8
}
