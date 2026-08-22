// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  The inputs the stroke resolver uses to produce a <see cref="ModernFieldStroke"/> for a control.
/// </summary>
/// <param name="BackColor">The control's effective, opaque background color.</param>
/// <param name="Enabled">Whether the control is enabled.</param>
/// <param name="ReadOnly">Whether the control is read-only.</param>
/// <param name="Focused">Whether the control has keyboard focus.</param>
/// <param name="Hovered">Whether the pointer is over the control.</param>
/// <param name="DarkMode">Whether dark mode is in effect.</param>
/// <param name="HighContrast">Whether High Contrast is in effect.</param>
/// <param name="AccentColor">The system accent color.</param>
/// <param name="DeviceDpi">The control's current device DPI.</param>
internal readonly record struct ModernFieldStrokeContext(
    Color BackColor,
    bool Enabled,
    bool ReadOnly,
    bool Focused,
    bool Hovered,
    bool DarkMode,
    bool HighContrast,
    Color AccentColor,
    int DeviceDpi);
