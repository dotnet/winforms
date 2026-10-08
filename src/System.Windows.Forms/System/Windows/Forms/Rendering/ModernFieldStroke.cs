// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Resolved border presentation for one <see cref="ModernFieldStrokeState"/>.
///  Rendering consumes this result; the resolver owns precedence and color calculations.
/// </summary>
/// <param name="SideTopColor">Top, left, and right edge color.</param>
/// <param name="BottomColor">Bottom edge color.</param>
/// <param name="SurfaceColor">Control surface fill in this state.</param>
/// <param name="SideTopThicknessDip">Top, left, and right edge thickness in DIPs.</param>
/// <param name="BottomThicknessDip">Bottom edge thickness in DIPs.</param>
/// <param name="HasFocusIndicator">Whether the bottom edge is the focus indicator.</param>
internal readonly record struct ModernFieldStroke(
    Color SideTopColor,
    Color BottomColor,
    Color SurfaceColor,
    float SideTopThicknessDip,
    float BottomThicknessDip,
    bool HasFocusIndicator);
