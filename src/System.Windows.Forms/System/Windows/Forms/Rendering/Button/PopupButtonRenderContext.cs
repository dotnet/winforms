// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Rendering.Button;

/// <summary>
///  Carries every piece of information the <see cref="FlatStyle.Popup"/> key-cap renderer needs, without a
///  dependency on a live control instance.
/// </summary>
/// <remarks>
///  <para>
///   Because the renderer receives all state through this context, it can be driven from a control, a designer
///   surface or a preview-image generator. A caller only needs a <see cref="Graphics"/> target and this context.
///  </para>
/// </remarks>
internal sealed class PopupButtonRenderContext
{
    /// <summary>
    ///  Gets the bounds to render into, in device pixels of the target <see cref="Graphics"/>.
    /// </summary>
    public required Rectangle Bounds { get; init; }

    /// <summary>
    ///  Gets the caption text. May be <see langword="null"/> or empty.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    ///  Gets the font used for the caption.
    /// </summary>
    public required Font Font { get; init; }

    /// <summary>
    ///  Gets the effective key face color.
    /// </summary>
    public Color BackColor { get; init; } = SystemColors.Control;

    /// <summary>
    ///  Gets the effective caption color.
    /// </summary>
    public Color ForeColor { get; init; } = SystemColors.ControlText;

    /// <summary>
    ///  Gets the color behind the rendered key, used to resolve translucent automatic-color surfaces.
    /// </summary>
    public Color SurfaceColor { get; init; } = SystemColors.Control;

    /// <summary>
    ///  Gets a value indicating whether the renderer should select a readable caption color from the final bowl color.
    /// </summary>
    public bool UseAutomaticForeColor { get; init; }

    /// <summary>
    ///  Gets the border color of the key body.
    /// </summary>
    public Color BorderColor { get; init; } = SystemColors.ControlDark;

    /// <summary>
    ///  Gets the border width in device pixels. <c>0</c> renders no border.
    /// </summary>
    public int BorderWidth { get; init; } = 1;

    /// <summary>
    ///  Gets a value indicating whether the key is enabled.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    ///  Gets a value indicating whether the key has keyboard focus and should show a focus cue.
    /// </summary>
    public bool Focused { get; init; }

    /// <summary>
    ///  Gets a value indicating whether the key is currently pressed. Used for the high-contrast fallback,
    ///  where continuous animation is not applied.
    /// </summary>
    public bool Pressed { get; init; }

    /// <summary>
    ///  Gets a value indicating whether the key is the default button of its dialog.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    ///  Gets a value indicating whether the application is using its dark color scheme.
    /// </summary>
    public bool IsDarkMode { get; init; }

    /// <summary>
    ///  Gets the animation progress snapshot.
    /// </summary>
    public PopupButtonAnimationState AnimationState { get; init; }

    /// <summary>
    ///  Gets the caption alignment within the key top.
    /// </summary>
    public ContentAlignment TextAlign { get; init; } = ContentAlignment.MiddleCenter;

    /// <summary>
    ///  Gets the size of the image rendered with the caption.
    /// </summary>
    public Size ImageSize { get; init; }

    /// <summary>
    ///  Gets the alignment of the image within the key surface.
    /// </summary>
    public ContentAlignment ImageAlign { get; init; } = ContentAlignment.MiddleCenter;

    /// <summary>
    ///  Gets the positional relationship between the image and caption.
    /// </summary>
    public TextImageRelation TextImageRelation { get; init; } = TextImageRelation.Overlay;

    /// <summary>
    ///  Gets the right-to-left setting for text rendering.
    /// </summary>
    public RightToLeft RightToLeft { get; init; } = RightToLeft.No;

    /// <summary>
    ///  Gets the padding applied around the caption inside the bowl.
    /// </summary>
    public Padding Padding { get; init; }

    /// <summary>
    ///  Gets the DPI of the target device. Used to scale all chrome metrics.
    /// </summary>
    public int DeviceDpi { get; init; } = 96;

    /// <summary>
    ///  Gets the caption relief effect.
    /// </summary>
    public PopupButtonTextEffect TextEffect { get; init; } = PopupButtonTextEffect.Raised;

    /// <summary>
    ///  Gets a value indicating whether keyboard cues (mnemonic underlines) should be shown.
    /// </summary>
    public bool ShowKeyboardCues { get; init; } = true;

    /// <summary>
    ///  Gets a value indicating whether a high-contrast accessibility theme is active.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   When <see langword="true"/>, the renderer falls back to a flat, higher-contrast style without material
    ///   emulation.
    ///  </para>
    /// </remarks>
    public bool HighContrast { get; init; } = SystemInformation.HighContrast;

    /// <summary>
    ///  Gets the rendering options to use. Defaults to the process-wide shared options.
    /// </summary>
    public PopupButtonRenderOptions Options { get; init; } = PopupButtonRenderOptions.Shared;
}
