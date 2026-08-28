// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  A fully resolved modern editable-control border for one <see cref="ModernFieldStrokeState"/>.
///  Paint paths receive only this; state precedence and color derivation stay inside the resolver.
/// </summary>
/// <param name="SideTopColor">Color of the top, left, and right edges.</param>
/// <param name="BottomColor">Color of the bottom edge.</param>
/// <param name="SurfaceColor">Fill color of the control surface for this state.</param>
/// <param name="SideTopThicknessDip">Thickness of the top, left, and right edges, in DIPs.</param>
/// <param name="BottomThicknessDip">Thickness of the bottom edge, in DIPs.</param>
/// <param name="HasFocusIndicator">Whether the bottom edge expands into the focus indicator.</param>
internal readonly record struct ModernFieldStroke(
    Color SideTopColor,
    Color BottomColor,
    Color SurfaceColor,
    float SideTopThicknessDip,
    float BottomThicknessDip,
    bool HasFocusIndicator);
