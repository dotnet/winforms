// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.ViewManagement;

/// <summary>
///  WinRT ABI for <c>Windows.UI.Color</c>, the value returned by
///  <see cref="IUISettings3.GetColorValue(UIColorType, UIColor*)"/>.
/// </summary>
/// <remarks>
///  <para>
///   Manually defined to mirror the WinRT ABI layout (four sequential bytes: alpha, red, green, blue)
///   without taking a CsWinRT projection dependency.
///  </para>
/// </remarks>
internal struct UIColor
{
    /// <summary>
    ///  The alpha channel of the color.
    /// </summary>
    public byte A;

    /// <summary>
    ///  The red channel of the color.
    /// </summary>
    public byte R;

    /// <summary>
    ///  The green channel of the color.
    /// </summary>
    public byte G;

    /// <summary>
    ///  The blue channel of the color.
    /// </summary>
    public byte B;
}
