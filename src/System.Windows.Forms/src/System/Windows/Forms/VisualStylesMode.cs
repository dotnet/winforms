// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Represents the version of the visual renderer.
/// </summary>
public enum VisualStylesMode : short
{
    /// <summary>
    ///  The visual renderer is not in use. See <see cref="UseVisualStyles"/>.
    /// </summary>
    Disabled = 0,

    /// <summary>
    ///  The classic version of the visual renderer (.NET 8 and earlier).
    /// </summary>
    Classic = 8,

    /// <summary>
    ///  The .NET 9/.NET 10 version of the visual renderer.
    /// </summary>
    Version10 = 10,

    /// <summary>
    ///  The latest version of the visual renderer.
    /// </summary>
    Latest = short.MaxValue
}
