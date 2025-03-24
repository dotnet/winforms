// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Private.Windows.Graphics;

/// <summary>
///  Platform agnostic interface for a bitmap.
/// </summary>
internal interface IBitmap
{
    /// <summary>
    ///  Gets a Win32 HBITMAP handle for this bitmap.
    /// </summary>
    HBITMAP GetHbitmap();

    /// <summary>
    ///  Size of the bitmap in pixels.
    /// </summary>
    Size Size { get; }
}
