// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.ViewManagement;

/// <summary>
///  WinRT ABI for <c>Windows.UI.ViewManagement.UIColorType</c>, identifying which system color to
///  retrieve through <see cref="IUISettings3.GetColorValue(UIColorType, UIColor*)"/>.
/// </summary>
/// <remarks>
///  <para>
///   Manually defined to mirror the WinRT enumeration without taking a CsWinRT projection dependency.
///  </para>
/// </remarks>
internal enum UIColorType
{
    /// <summary>
    ///  The background color.
    /// </summary>
    Background = 0,

    /// <summary>
    ///  The foreground color.
    /// </summary>
    Foreground = 1,

    /// <summary>
    ///  The darkest of the three accent shades.
    /// </summary>
    AccentDark3 = 2,

    /// <summary>
    ///  The second darkest accent shade.
    /// </summary>
    AccentDark2 = 3,

    /// <summary>
    ///  The lightest of the three dark accent shades.
    /// </summary>
    AccentDark1 = 4,

    /// <summary>
    ///  The base accent color.
    /// </summary>
    Accent = 5,

    /// <summary>
    ///  The darkest of the three light accent shades.
    /// </summary>
    AccentLight1 = 6,

    /// <summary>
    ///  The second lightest accent shade.
    /// </summary>
    AccentLight2 = 7,

    /// <summary>
    ///  The lightest of the three accent shades.
    /// </summary>
    AccentLight3 = 8,

    /// <summary>
    ///  The complement of the accent color.
    /// </summary>
    Complement = 9,
}
