// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Tunable rendering options for the <see cref="FlatStyle.Popup"/> key-cap renderer.
/// </summary>
/// <remarks>
///  <para>
///   All geometric magnitudes are expressed as <see cref="PopupButtonMetric"/> values and only resolved to
///   device pixels inside the renderer, based on the DPI carried by the render context. <see cref="Shared"/>
///   is the process-wide instance used to tune the best all-purpose defaults. Border width and color are not
///   part of these options; they are read from the button's <see cref="FlatButtonAppearance"/>.
///  </para>
/// </remarks>
internal sealed class PopupButtonRenderOptions
{
    /// <summary>
    ///  Gets the process-wide shared options instance.
    /// </summary>
    public static PopupButtonRenderOptions Shared { get; } = new();

    /// <summary>
    ///  Gets or sets the corner radius magnitude of the key body.
    /// </summary>
    public PopupButtonMetric CornerRadius { get; set; } = PopupButtonMetric.Medium;

    /// <summary>
    ///  Gets or sets how deep the concave bowl of the key top appears.
    /// </summary>
    public PopupButtonMetric ConcavityDepth { get; set; } = PopupButtonMetric.Medium;

    /// <summary>
    ///  Gets or sets the strength of edge highlights.
    /// </summary>
    public PopupButtonMetric HighlightStrength { get; set; } = PopupButtonMetric.Medium;

    /// <summary>
    ///  Gets or sets the strength of edge and bowl shadows.
    /// </summary>
    public PopupButtonMetric ShadowStrength { get; set; } = PopupButtonMetric.Medium;

    /// <summary>
    ///  Gets or sets the base duration of state-change animations.
    /// </summary>
    public TimeSpan AnimationDuration { get; set; } = TimeSpan.FromMilliseconds(160);

    /// <summary>
    ///  Resolves <see cref="CornerRadius"/> to device-independent pixels (96 DPI).
    /// </summary>
    internal float GetCornerRadiusDip()
        => CornerRadius switch
        {
            PopupButtonMetric.Small => 4f,
            PopupButtonMetric.Large => 9f,
            _ => 6f
        };

    /// <summary>
    ///  Resolves <see cref="ConcavityDepth"/> to a fractional shading depth.
    /// </summary>
    internal float GetConcavityDepth()
        => ConcavityDepth switch
        {
            PopupButtonMetric.Small => 0.07f,
            PopupButtonMetric.Large => 0.17f,
            _ => 0.11f
        };

    /// <summary>
    ///  Resolves <see cref="HighlightStrength"/> to a multiplier.
    /// </summary>
    internal float GetHighlightMultiplier()
        => HighlightStrength switch
        {
            PopupButtonMetric.Small => 0.6f,
            PopupButtonMetric.Large => 1.5f,
            _ => 1f
        };

    /// <summary>
    ///  Resolves <see cref="ShadowStrength"/> to a multiplier.
    /// </summary>
    internal float GetShadowMultiplier()
        => ShadowStrength switch
        {
            PopupButtonMetric.Small => 0.6f,
            PopupButtonMetric.Large => 1.5f,
            _ => 1f
        };
}
