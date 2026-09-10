// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Inputs for creating a <see cref="ModernFieldStroke"/>.
/// </summary>
/// <param name="BackColor">Effective opaque background color.</param>
/// <param name="Enabled">Whether the control is enabled.</param>
/// <param name="ReadOnly">Whether the control is read-only.</param>
/// <param name="Focused">Whether the control has keyboard focus.</param>
/// <param name="Hovered">Whether the pointer is over the control.</param>
/// <param name="DarkMode">Whether dark mode is active.</param>
/// <param name="AccentColor">System accent color for focus.</param>
/// <param name="DeviceDpi">Current device DPI.</param>
internal readonly record struct ModernFieldStrokeContext(
    Color BackColor,
    bool Enabled,
    bool ReadOnly,
    bool Focused,
    bool Hovered,
    bool DarkMode,
    Color AccentColor,
    int DeviceDpi);
