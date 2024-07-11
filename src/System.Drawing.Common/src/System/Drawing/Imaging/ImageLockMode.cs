// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Imaging;

/// <summary>
///  Indicates the access mode for an <see cref='Image'/>.
/// </summary>
public enum ImageLockMode
{
    /// <summary>
    ///  Specifies the image is read-only.
    /// </summary>
    ReadOnly = GdiPlus.ImageLockMode.ImageLockModeRead,

    /// <summary>
    ///  Specifies the image is write-only.
    /// </summary>
    WriteOnly = GdiPlus.ImageLockMode.ImageLockModeWrite,

    /// <summary>
    ///  Specifies the image is read-write.
    /// </summary>
    ReadWrite = ReadOnly | WriteOnly,

    /// <summary>
    ///  Indicates the image resides in a user input buffer, to which the user controls access.
    /// </summary>
    UserInputBuffer = GdiPlus.ImageLockMode.ImageLockModeUserInputBuf
}
