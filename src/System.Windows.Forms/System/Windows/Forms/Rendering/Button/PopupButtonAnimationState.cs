// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Immutable snapshot of the animation progress of a <see cref="FlatStyle.Popup"/> key-cap button.
/// </summary>
/// <remarks>
///  <para>
///   The control owns the animation <em>timing</em> (driven by the shared animation manager and its
///   high-precision timer); the renderer only consumes the resulting progress values. This keeps the
///   renderer usable outside a live control (designer surfaces, preview bitmaps) where no timer exists —
///   simply pass fixed progress values.
///  </para>
/// </remarks>
/// <param name="hoverProgress">Hover progress, <c>0</c> (not hovered) to <c>1</c> (fully hovered).</param>
/// <param name="pressProgress">Press progress, <c>0</c> (released) to <c>1</c> (fully pressed).</param>
internal readonly struct PopupButtonAnimationState(float hoverProgress, float pressProgress)
{
    /// <summary>
    ///  Gets a state with no hover and no press applied.
    /// </summary>
    public static PopupButtonAnimationState None => default;

    /// <summary>
    ///  Gets the hover progress in the range <c>0</c>-<c>1</c>.
    /// </summary>
    public float HoverProgress { get; } = Math.Clamp(hoverProgress, 0f, 1f);

    /// <summary>
    ///  Gets the press progress in the range <c>0</c>-<c>1</c>.
    /// </summary>
    public float PressProgress { get; } = Math.Clamp(pressProgress, 0f, 1f);
}
