// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  A DPI-neutral, generic magnitude used to tune the <see cref="FlatStyle.Popup"/> key-cap renderer
///  (corner radius, concavity depth, highlight/shadow strength).
/// </summary>
/// <remarks>
///  <para>
///   Using generic magnitudes instead of pixel values allows the renderer to resolve the actual device
///   values per call, which keeps high-DPI scenarios trivially correct.
///  </para>
/// </remarks>
internal enum PopupButtonMetric
{
    /// <summary>
    ///  A small magnitude.
    /// </summary>
    Small = 0,

    /// <summary>
    ///  A medium magnitude. This is the default.
    /// </summary>
    Medium = 1,

    /// <summary>
    ///  A large magnitude.
    /// </summary>
    Large = 2
}
